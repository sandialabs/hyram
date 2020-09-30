from __future__ import print_function, absolute_import, division

import os
import sys
import warnings

import matplotlib.pyplot as plt
import numpy as np
from scipy import integrate, interpolate, optimize
from scipy import constants as const
# TODO remove skimage dependency?
from skimage import measure

from mpl_toolkits.mplot3d import axes3d
from mpl_toolkits.axes_grid1 import ImageGrid

from ._nn import NotionalNozzle
from ._therm import Combustion
from ._jet import DevelopingFlow

class Flame:
    def __init__(self, fluid, orifice, ambient, nC = 0, Q = None, 
                 theta0 = 0., x0 = 0, y0 = 0, 
                 nn_conserve_momentum = True, nn_T = 'solve_energy',
                 chem = None, 
                 lamf = 1.24, lamv = 1.24, am = 3.42e-2, ab = 5.75e-4, af = 0.23,
                 numpts = 100, numB = 6, numBpts = 10000,
                 betaS = 8.6, betaB = 0.8
                 ):
        '''
        class for calculating the characteristics of a 2-D flame, without wind 
        see Ekoto et al. International Journal of Hydrogen Energy, 39, 2014 (20570-20577)
        
        Parameters
        ----------
        fluid: Fluid object (hc_comps)
            fluid that is being released
        orifice: Orifice object (h2_comps)
            orifice through which fluid is being released
        ambient: Fluid object (hc_comps)
            fluid into which release is occuring
        nC: int
            number of carbon atoms (0 for H2, 1 for CH4, 2 for C2H6, 3 for C3H8)
        Q: float, optional 
            should only be specified for subsonic release, calculated from fluid.P if unspecified (m^3/s)
        theta0 : float, optional
            angle of release (rad) default value of 0 is horizontal
        x0 : float, optional
            horizontal starting point (m)
        y0 : float, optional
            vertical starting point (m)
        chem : chemistry class (see hc_therm for usage), optional
            if none given, will initialize new chemisty class
        lamf : float
            spreading ratio for mixture fraction Gaussian profile
        lamv : float
            spreading ratio for velocity Gaussian profile
        am : float
            momentum entrainment coefficient
        ab : float
            buoyancy entrainment coefficient    
        af : float
            Plank's mean absorption coefficient for H2O
        nnmodel : string
            notional nozzle model to use for high-pressure release (see hc_nn for usage)
        numpts: float, optional
            number of points at which to solve for the flame
        numB : float, optional
            number of halfwidths to integrate to
        numBpts : float, optional
            number of points to use in integral model at each step
        betaS : float, optional
            proportionality constant for start of integral model relative to d* (see Li, et al. IJHE 2015)
        betaB : float, optional
            proportionality constant for width at start of integral model relative to d* (see Li, et al. IJHE 2015)
        '''
        S0 = 0;
        if Q != None:
            warnings.warn('assuming unchoked -- no notional nozzle model')
            rho0, T0, v0, mdot0, p0 = fluid.rho, fluid.T, Q/orifice.A, Q*fluid.rho, fluid.P
            self.gas1, self.orifice1 = fluid, orifice
            self.gas1.v = v0
            betaS = 0.
        else:
            # Notional nozzle:
            throat = orifice.flow(fluid, ambient.P)
            nn = NotionalNozzle(throat, orifice, ambient)
            self.gas1, self.orifice1 = nn.calculate(nn_T, nn_conserve_momentum)
            rho0, T0, v0, mdot0, p0 = throat.rho, throat.T, throat.v, self.orifice1.mdot(throat), throat.P

        self.amb = ambient
        self.Emom = am*np.sqrt(self.orifice1.A*(self.gas1.rho/self.amb.rho)*self.gas1.v**2) 
        self.theta0, self.lamf, self.lamv, self.ab = theta0, lamf, lamv, ab
        self.chem = chem
        self.af = af
        self.nC = nC
        self.numpts, self.numB, self.numBpts = numpts, numB, numBpts
        self.x0, self.y0, self.S0 = x0, y0, S0
        self.betaS, self.betaB = betaS, betaB
        
    def _govEqns(self, S, ind_vars, numB = 5, numpts = 500):
        '''
        Governing equations for a flame, written in terms of d/dS of (V_cl, B, theta, f_cl, x, and y).
        
        A matrix soluition to the continuity, x-momentum, y-mometum and mixture fraction equations
        solves for d/dS of the dependent variables V_cl, B, theta, and f_cl.  Numerically integrated
        to infinity = numB * B(S) using numpts discrete points.'''

        # break independent variables out of ind_vars
        [V_cl, B, theta, f_cl, x, y] = ind_vars

        # needed to integrate to infinity (6B):
        r = np.zeros(numpts)
        r[1:] = np.logspace(-7, np.log10(numB*B), numpts-1)
        
        # mass fraction and velocity have Gaussian shapes
        f = f_cl*np.exp(-(r/(self.lamf*B))**2)
        V = V_cl*np.exp(-(r/(self.lamv*B))**2)

        # density isn't a nice Gaussian, due to combustion 

        rho = self.chem.rho_prod(f)
        drhodf = self.chem.drhodf(f)
        
        rho_int = integrate.trapz(self.amb.rho - rho, r)
        
        Ebuoy = 2*np.pi*self.ab*np.sin(theta)*(const.g*(rho_int)/(B*V_cl*self.gas1.rho))# m**2/s
        E = self.Emom + Ebuoy

        # right-hand side of governing equations:
        RHS = np.array([self.amb.rho*E/(2*const.pi), # continuity
                     0, # x-momentum
                     integrate.trapz((self.amb.rho-rho)*const.g*r, r), # y-momentum
                     0]) # mixture fraction

        # some terms that are needed
        zero = np.zeros_like(r)
        dfdS = np.array([zero, 2*r**2/self.lamf**2/B**3*f, zero, f/f_cl])
        dVdS = np.array([V/V_cl, 2*r**2/self.lamv**2/B**3*V, zero, zero])
        drhodS = drhodf*dfdS
        dthetadS = np.array([zero, zero, np.ones_like(r), zero])
        
        # left-hand side of governing equations:
        LHS = np.array([drhodS*V*r + rho*dVdS*r, # continuity
                        drhodS*V**2*np.cos(theta)*r + 2*rho*V*dVdS*np.cos(theta)*r + rho*V**2*-np.sin(theta)*dthetadS*r, # x-momentum
                        drhodS*V**2*np.sin(theta)*r + 2*rho*V*dVdS*np.sin(theta)*r + rho*V**2*np.cos(theta)*dthetadS*r, # y-momentum 
                        drhodS*V*f*r + rho*dVdS*f*r + rho*V*dfdS*r]) # mixture fraction
        LHS = integrate.trapz(LHS, r)

        dz = np.append(np.linalg.solve(LHS,RHS), np.array([np.cos(theta), np.sin(theta)]), axis = 0)
        
        return dz
    
    def solve(self, Smax = None, tol = 1e-5, establish = 'old'):
        '''
        Solves for a flame. Returns a dictionary of flame results.  Also updates the Flame class with those results.
        
        Parameters
        ----------
        Smax : float, optional
            endopoint along curved flame for integration (m) default will calculate visible length of flame
        
        Returns
        -------
        res : dict
            dictionary of flame results
        '''
        try:
            if self.chem.Treac != self.amb.T or abs(self.chem.P/self.amb.P - 1) > 1e-10:
                self.chem.reinitilize(self.amb.T, self.nC, self.amb.P)
        except:
            self.chem = Combustion(self.amb.T, self.nC, self.amb.P, self.numpts)
        print('solving for the flame...', end = '')
        sys.stdout.flush()
        if Smax is None:
            Smax = self.S0 + self.length()
        if establish == 'old':
            V_clE = self.gas1.v
            BE = self.orifice1.d/np.sqrt(2.*(2.*self.lamf**2 + 1)/(self.lamf**2*self.gas1.rho/self.amb.rho + self.lamf**2 + 1))
            Y_clE = (self.lamf**2+1)/(2*self.lamf**2)
            Fr = V_clE/(const.g*self.orifice1.d*abs(self.amb.rho-self.gas1.rho)/self.gas1.rho)**0.5 # Densimetric Froude #
            if Fr**2 < 1:
                SE = 0.
            elif Fr**2 < 5:
                SE = (2.075 + 0.425*Fr**2)*self.orifice1.d
            elif Fr**2 < 40:
                SE = (3.9 + 0.057*Fr**2)*self.orifice1.d
            else:
                SE  = 6.2*self.orifice1.d
        elif establish == 'new':
            SE = self.betaS*self.orifice1.d
            BE = self.betaB*self.orifice1.d
            Y_clE = (self.lamf**2 + 1)/(2*self.lamf**2)
            rho_clE = self.amb.rho/(f_clE*(self.amb.therm.MW-self.gas1.therm.MW)/self.gas1.therm.MW + 1)
            V_clE = (self.lamv**2 + 1)/self.lamv**2*self.gas1.rho*self.orifice1.d**2*self.gas1.v/(4*rho_clE*f_clE*BE**2)
        f_clE = optimize.newton(lambda f: Y_clE - self.chem._Yprod(f)[self.chem.reac], Y_clE)
        thetaE = self.theta0
        xE = self.x0 + SE*np.cos(thetaE)
        yE = self.y0 + SE*np.sin(thetaE)
        SE = self.S0 + SE
        # Smax += SE
        r = integrate.ode(self._govEqns).set_integrator('dopri5', atol = tol, rtol = tol)
        r.set_f_params(self.numB, self.numBpts)
        r.set_initial_value(np.array([V_clE, BE, thetaE, f_clE, xE, yE]), SE)
        dS = float(Smax - SE)/self.numpts
        S, Z = [r.t], [r.y]
        while r.successful() and r.t < Smax:
            r.integrate(r.t + dS)
            S.append(r.t)
            Z.append(r.y)
        
        S, Z = np.array(S), np.array(Z)
        res = dict(zip(['V_cl', 'B', 'theta', 'f_cl', 'x', 'y'], Z.T))
        res['S'] = S
        for k, v in res.items():
            self.__dict__[k] = v
        print('done.')
        sys.stdout.flush()
        return res
    
    def length(self):
        '''
        These correlations come from Schefer et al. IJHE 31 (2006): 1332-1340
        and Molina et al PCI 31 (2007): 2565-2572
        
        returns the visible flame length
        also updates the flame class with 
        
        .Lvis (flame length), 
        .Wf (flame width), 
        .tauf (flame residence time)
        .Xrad (radiant fraction)
        '''
        try:
            if self.chem.Treac != self.amb.T or abs(self.chem.P/self.amb.P - 1) > 1e-10:
                self.chem.reinitilize(self.amb.T, self.nC, self.amb.P)
        except:
            self.chem = Combustion(self.amb.T, self.nC, self.amb.P, self.numpts)
        fs, Tad = self.chem.fstoich, self.chem.T_prod(self.chem.fstoich)
        Tamb = self.amb.T
        rhoair, rhof = self.amb.rho, self.chem.rho_prod(self.chem.fstoich)
        Deff, rhoeff = self.orifice1.d, self.gas1.rho
        
        # Compute the flame Froude number
        Frf = (self.gas1.v*fs**1.5)/(((rhoeff/rhoair)**0.25)*np.sqrt(((Tad - Tamb)/Tamb)*const.g*Deff))
        
        # Compute visible flame length
        Lstar = ((13.5*Frf**0.4)/(1+0.07*Frf**2)**0.2)*(Frf < 5) + 23*(Frf >= 5)

        dstar = Deff*(rhoeff/rhoair)**0.5 
        self.Lvis = Lstar*dstar/fs # visible flame length [m]
        self.Wf = 0.17*self.Lvis 
        # flame residence time [ms]
        
        self.tauf = (const.pi/12)*(rhof*(self.Wf**2)*self.Lvis*fs)/(self.orifice1.mdot(self.gas1))*1000
        #self.Xrad = (0.08916*np.log10(self.tauf*self.af*Tad**4) - 1.2172) # comes from Molina et al.
        self.Xrad = 9.45e-9 * (self.tauf*self.af*Tad**4)**0.47 # see Panda, Hecht, IJHE 2016
        return self.Lvis
    
    def Qrad_multi(self, x, y, z, RH, WaistLoc = 0.75, N = 50):
        '''
        MultiSource radiation model -- follows Hankinson & Lowesmith, CNF 159, 2012: 1165-1177       
        '''
        obsOrg = np.array([x, y, z]).T
        
        DHc = self.chem.DHc  # heat of combustion [J/kg]
        try:
            Lvis = self.Lvis  # length of visible flame [m]
        except:
            Lvis = self.length()
        Xrad = self.Xrad
        mdot = self.orifice1.mdot(self.gas1)  # mass flow rate [kg/s]
        T = self.amb.T


        n = int(WaistLoc*N)
        w = np.arange(1, N+1, dtype = float)
        w[n:] = (n-((n-1)/(N-(n+1)))*(w[n:]-(n+1)))
        w /= np.sum(w)

        try:
            S = np.linspace(self.S[0], self.S[-1], N)
            X = interpolate.interp1d(self.S,self.x)(S)
            Y = interpolate.interp1d(self.S,self.y)(S)

        except:
            warnings.warn('running flame model with default parameters')
            self.solve()
            S = np.linspace(self.S[0], self.S[-1], N)
            X = interpolate.interp1d(self.S,self.x)(S)
            Y = interpolate.interp1d(self.S,self.y)(S)

        sourceOrg = np.array([X,Y,np.zeros_like(X)]).T

        Qrad = np.zeros(obsOrg.shape[:-1])

        def tau(L, T, RH, CO2_ppm = 400):
            ''' transmissivity from Wayne, J. Loss Prev. Proc. Ind. 1991
            Parameters
            ----------
            L : path length (m)
            T : atmospheric temperature (K)
            RH: fractional relative humidity (0-1)
            CO2_ppm: atmospheric CO2 (ppm) - assumed to be 400 
            
            Returns
            -------
            tau - transmissivity
            '''
            Smm = np.exp(20.386-5132/T) # saturated vapor pressure [mmHg] - Wikipedia
            XH2O = RH*L*Smm*288.651/T
            XCO2 = L*273./T*CO2_ppm/335. # m
            tau = (1.006 - 0.01171*np.log10(XH2O) - 0.02368*(np.log10(XH2O))**2 - 
                   0.03188*np.log10(XCO2) + 0.001164*np.log10(XCO2)**2)
            return tau

        for j in range(len(w)):
            v = sourceOrg[j] - obsOrg
            if obsOrg.ndim > 1:
                obsNorm = v/(np.linalg.norm(v, axis = 1)+1e-99)[:, np.newaxis]
            else:
                obsNorm = v/(np.linalg.norm(v)+1e-99)
        
            if obsOrg.ndim == 4:
                len_v = np.linalg.norm(v, axis = 3)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm,v), axis = 3)/len_v)
            elif obsOrg.ndim == 3:
                len_v = np.linalg.norm(v, axis = 2)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm,v), axis = 2)/len_v)
            elif obsOrg.ndim == 2:
                len_v = np.linalg.norm(v, axis = 1)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm,v), axis = 1)/len_v)
            elif obsOrg.ndim == 1:
                len_v = np.linalg.norm(v)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm,v))/len_v)
            Qrad += w[j]/(4*const.pi*len_v**2)*np.cos(phi)*Xrad*mdot*DHc*tau(len_v, T, RH)

        return Qrad.T/1000

    def Qrad_single(self, x, y, z, flameCen, RH):
        '''
        single point radiation model
        '''
        obsOrg = np.array([x, y, z]).T
        DHc = self.chem.DHc  # heat of combustion [J/kg]
        try:
            Lvis = self.Lvis  # length of visible flame [m]
        except:
            Lvis = self.length()
        Xrad = self.Xrad 
        mdot = self.orifice1.mdot(self.gas1)# mass flow rate [kg/s]
        
        Srad = Xrad*mdot*DHc
        # Fit to normalized radiant power curve, C*

        Distance = obsOrg - flameCen
        if np.size(obsOrg.shape) == 4:
            Xbar_temp = obsOrg[:, :, :, 0]/Lvis
            Rad = np.linalg.norm(Distance[:, :, :, 1:], axis = 3)
        elif np.size(obsOrg.shape) == 3:
            Xbar_temp = obsOrg[:, :, 0]/Lvis
            Rad = np.linalg.norm(Distance[:, :, 1:], axis = 2)
        elif np.size(obsOrg.shape) == 2:
            Xbar_temp = obsOrg[:, 0]/Lvis 
            Rad = np.linalg.norm(Distance[:, 1:], axis = 1)
        elif np.size(obsOrg.shape) == 1:
            Xbar_temp = obsOrg[0]/Lvis
            Rad = np.linalg.norm(Distance[1:])

        val1 = -2.7579*(np.abs(Xbar_temp-0.6352))
        Cstar = 0.85985*np.exp(val1)
        # Radiative heat flux [kW/s]
        VF = Cstar/(4*const.pi*Rad**2)/1000
        
        def tau(L, T, RH, CO2_ppm = 400):
            ''' transmissivity from Wayne, J. Loss Prev. Proc. Ind. 1991
            Parameters
            ----------
            L : path length (m)
            T : atmospheric temperature (K)
            RH: fractional relative humidity (0-1)
            CO2_ppm: atmospheric CO2 (ppm) - assumed to be 400 
            
            Returns
            -------
            tau - transmissivity
            '''
            Smm = np.exp(20.386-5132/T) # saturated vapor pressure [mmHg] - Wikipedia
            XH2O = RH*L*Smm*288.651/T
            XCO2 = L*273./T*CO2_ppm/335. # m
            tau = (1.006 - 0.01171*np.log10(XH2O) - 0.02368*(np.log10(XH2O))**2 - 
                   0.03188*np.log10(XCO2) + 0.001164*np.log10(XCO2)**2)
            return tau
        
        Qrad = VF*Srad*tau(Rad, self.amb.T, RH)
        return Qrad.T
    
    def _contourdata(self):
        iS = np.arange(len(self.S))
        r = np.append(np.append(-np.logspace(np.log10(10*max(self.B)), -2), np.linspace(-10**-2.1, 10**-2.1, num = 10)), np.logspace(-2, np.log10(10*max(self.B))))
        r, iS = np.meshgrid(r, iS)
        B = self.B[iS]
        f_cl = self.f_cl[iS]
        
        fvals = f_cl*np.exp(-(r/(self.lamf*B))**2)
        Tvals = self.chem.T_prod(fvals)
        
        x = self.x[iS] + r*np.sin(self.theta[iS])
        y = self.y[iS] - r*np.cos(self.theta[iS])
        
        return x, y, Tvals
    
    def plot_Ts(self, mark = None, mcolors = 'w', 
                xlims = None, ylims = None,
                xlab = 'x (m)', ylab = 'y (m)',
                cp_params = {}, levels = 100, 
                addColorBar = True, aspect = 1, plot_title = None,
                fig_params = {}, subplots_params = {}, ax = None):
        '''
        makes temperature contour plot
        
        Parameters
        ----------
        mark: list, optional
            levels to draw contour lines (Temperatures, or None if None desired)
        mcolors: color or list of colors, optional
            colors of marked contour leves
        xlims: tuple, optional
            tuple of (xmin, xmax) for contour plot
        ylims: tuple, optional
            tuple of (ymin, ymax) for contour plot
        vmin: float, optional
            minimum mole fraction for contour plot
        vmax: float, optional
            maximum mole fraction for contour plot
        levels: int, optional
            number of contours levels to draw
        addColorBar: boolean, optional
            whether to add a colorbar to the plot
        aspect: float, optional
            aspect ratio of plot
        fig_parameters: optional
            dictionary of figure parameters (e.g. figsize)
        subplots_params: optional
            dictionary of subplots_adjust parameters (e.g. top)
        ax: optional
            axes on which to make the plot
        '''
        if ax is None:
            fig, ax = plt.subplots(**fig_params)
            plt.subplots_adjust(**subplots_params)
        x, y, T = self._contourdata()
        #clrmap = 'Spectral_r'
        clrmap = 'plasma'
        ax.set_facecolor(plt.cm.get_cmap(clrmap)(0)) #old matplotlib: ax.set_axis_bgcolor
        cp = ax.contourf(x, y, T, levels, cmap=clrmap, **cp_params)
        if mark is not None:
            cp2 = ax.contour(x, y, T, levels = mark, colors = mcolors, linewidths = 1.5, **cp_params)

        if xlims is not None:
            ax.set_xlim(*xlims)
        if ylims is not None:
            ax.set_ylim(*ylims)

        if addColorBar:
            cb = plt.colorbar(cp)
            cb.set_label('Temperature (K)', rotation = -90, va = 'bottom')
        ax.set_xlabel(xlab)
        ax.set_ylabel(ylab)
        if aspect is not None:
            ax.set_aspect(aspect)
        if plot_title is not None:
            ax.set_title(plot_title)
        fig.tight_layout()
        return fig
    
    def iso_heat_flux_plot_sliced(self, title = '', 
                                  fnames = ['3DisoPlot.png', '2DcutsIsoPlot.png'], 
                                  directory = os.getcwd(),
                                  smodel = 'multi',
                                  RH = 0.89,
                                  contours = [1.577, 4.732, 25.237],
                                  nx = 50, ny = 50, nz = 50):
        '''
        plots slices of heat flux levels

        Parameters
        ----------
        title: string (optional)
            title shown on plot
        fname: string, optional
            file name to write
        directory: string, optional
            directory in which to save file
        smodel: string, optional
            'multi' or 'single' defining radiaion model used
        RH: float
            relative humidity
        contours: ndarray or list (optional)
            contour levels shown on plot (default values are 2012 International Fire Code (IFC) exposure limits 
            for property lines (1.577 kW/m2), employees (4.732 kW/ m2), and non-combustible equipment (25.237 kW/m2))
        nx, ny, nz: float (optional)
            number of points to solve for the heat flux in the x, y, and z directions
        
        Returns
        -------
        fname - string of file name saved
        '''
        Lvis = self.length()
        flameCen= np.array([self.x[np.argmin(np.abs(self.S - Lvis/2))],
                            self.y[np.argmin(np.abs(self.S - Lvis/2))],
                            0])
        dx, dy, dz = 4.5*Lvis/nx, (self.y[0] + Lvis)/ny, 4.*Lvis/nz
        
        x, y, z = np.mgrid[self.x[0]-1.5*Lvis:(self.x[0]+3*Lvis):dx, 
                           0:(self.y[0] + Lvis):dy, 
                           -2*Lvis:(2*Lvis):dz]
        x_z, y_z = np.mgrid[self.x[0]-1.5*Lvis:(self.x[0]+3*Lvis):dx, 
                            0:(self.y[0] + Lvis):dy]
        x_y, z_y = np.mgrid[self.x[0]-1.5*Lvis:(self.x[0]+3*Lvis):dx, 
                            -2*Lvis:(2*Lvis):dz]
        y_x, z_x = np.mgrid[0:(self.y[0] + Lvis):dy, 
                            -2*Lvis:(2*Lvis):dz]
        #3D figure:
        fig_iso = plt.figure(figsize = (8,3))
        if smodel == 'multi':
            flux = self.Qrad_multi(x, y, z, RH)
            fluxy0 = self.Qrad_multi(x_y, np.ones_like(x_y)*y.min(), z_y, RH)
        elif smodel == 'single':
            flux = self.Qrad_single(x, y, z, flameCen, RH)
            fluxy0 = self.Qrad_single(x_y, np.ones_like(x_y)*y.min(), z_y, flameCen, RH)
        ax = fig_iso.gca(projection='3d')
        for contour in contours:
            try:
                verts, faces, normals, values = measure.marching_cubes(flux, contour, 
                                                                    spacing = (dx, dy, dz))
            except AttributeError:
                verts, faces, normals, values = measure.marching_cubes_lewiner(flux, contour, 
                                                                               spacing = (dx, dy, dz))
            px, py, pz = verts.T
            ax.plot(self.x, self.y, color = 'k')
            ax.plot_trisurf(px + x.min(), 
                            py + y.min(),
                            faces,
                            pz + z.min(), 
                            alpha = 0.2,
                            color = plt.cm.get_cmap()(contour/max(contours))
                           )
            ax.set_xlabel('x [m]')
            ax.set_ylabel('y [m]')
            ax.set_zlabel('z [m]')
        ax.view_init(120, -95)
        ax.set_xlim3d(x.min(), x.max())
        ax.set_ylim3d(y.min(), y.max())
        ax.set_zlim3d(z.min(), z.max())
        ax.set_aspect(1)
        ax.contourf(x_y, fluxy0,  z_y, zdir = 'y', offset = y.min(), alpha = .3,
                    levels = np.append(0, contours))
        fig_iso.tight_layout()
        fname0 = os.path.join(directory, fnames[0])
        plt.savefig(fname0, dpi = 200)
        #2D cuts (new figure)
        fig = plt.figure(figsize = (8,4.5))
        fig.subplots_adjust(top=0.967, bottom=0.378) # not sure if this will always be right---tight_layout incompatible with ImageGrid
        grid = ImageGrid(fig, 111,  # similar to subplot(111)
                         nrows_ncols=(2, 2),  # creates 2x2 grid of axes
                         axes_pad=0.1,  # pad between axes in inch.
                         label_mode = "L",
                         cbar_mode = 'edge', 
                         cbar_location = 'bottom',
                         cbar_size = '10%',
                         cbar_pad = -1.5
                        )
        ax_xy, ax_zy, ax_xz = grid[0], grid[1], grid[2]
        for ax in [ax_xy, ax_zy, ax_xz]:
            ax.cax.set_visible(False)
        ax_zy.axis['bottom'].toggle(all=True) 
        grid[3].set_frame_on(False)
        grid[3].set_axis_off()
        ax_cb = grid[3].cax
        ax_cb.set_visible(True)
        
        if smodel == 'multi':
            fxy = self.Qrad_multi(x_z, y_z, flameCen[2]*np.ones_like(x_z), RH)
            fxz = self.Qrad_multi(x_y, flameCen[1]*np.ones_like(x_y), z_y, RH)
            fzy =  self.Qrad_multi(flameCen[0]*np.ones_like(z_x), y_x, z_x, RH)
        elif smodel == 'single':
            fxy = self.Qrad_single(x_z, y_z, flameCen[2]*np.ones_like(x_z), flameCen, RH)
            fxz = self.Qrad_single(x_y, flameCen[1]*np.ones_like(x_y), z_y, flameCen, RH)
            fzy = self.Qrad_single(flameCen[0]*np.ones_like(z_x), y_x, z_x, flameCen, RH)  

        ClrMap = plt.cm.get_cmap('RdYlGn_r')
        ClrMap.set_under('white')
        
        #xy axis:
        ax_xy.contourf(x_z, y_z, fxy, cmap=ClrMap,
                       levels = contours, extend = 'both')
        ax_xy.plot(self.x, self.y, color = 'k', marker = '.', linewidth = 1.5)
        ax_xy.set_ylabel('Height (y) [m]')
        ax_xy.annotate('z = %0.2f' % flameCen[2], xy = (0.02, .98), 
                       xycoords = 'axes fraction', va = 'top', color = 'k')
        #xz axis:
        ax_xz.contourf(x_y, z_y, fxz, cmap=ClrMap,
                       levels = contours, extend = 'both')
        ax_xz.plot(self.x, np.ones_like(self.x)*flameCen[2], color = 'k',
                   marker = '.', linewidth = 1.5)
        ax_xz.set_xlabel('Horizontal Distance (x) [m]')
        ax_xz.set_ylabel('Perpendicular Distance (z) [m]')
        ax_xz.annotate('y = %0.2f' % flameCen[1], xy = (0.02, .98), 
                       xycoords = 'axes fraction', va = 'top', color = 'k')
        #zy axis
        im = ax_zy.contourf(z_x, y_x, fzy, cmap=ClrMap,
                            levels = contours, extend = 'both')
        ax_zy.plot(np.ones_like(self.y)*flameCen[2], self.y, color = 'k',
                   marker = '.', linewidth = 1.5)
        ax_zy.set_xlabel('Perpendicular Distance (z) [m]')
        ax_zy.annotate('x = %0.2f' % flameCen[0], xy = (0.02, .98), xycoords = 'axes fraction', 
                       va = 'top', color = 'k')
        #colorbar
        cb = plt.colorbar(im, cax=ax_cb, orientation = 'horizontal', extendfrac='auto')
        cb.set_label('Heat Flux [kW/m^2]')
        for ax in [ax_xy, ax_xz, ax_zy]:
            ax.minorticks_on()
            ax.grid(alpha = .2, color = 'k')
            ax.grid(which = 'minor', alpha = .1, color = 'k')
            ax.set_aspect(1)
        fname1 = os.path.join(directory, fnames[1])
        #for some reason, bottom axis label was getting cut off without rect within tight_layout
        #fig.tight_layout(rect = [0, .08, 1, 1]) # tightlayout not compatable with ImageGrid
        plt.savefig(fname1, dpi = 200)
        return fname0, fname1