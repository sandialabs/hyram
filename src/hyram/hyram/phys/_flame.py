"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from __future__ import print_function, absolute_import, division

import os
import warnings
import copy

import matplotlib.pyplot as plt
import numpy as np
from mpl_toolkits.axes_grid1 import ImageGrid
from scipy import constants as const
from scipy import integrate, interpolate, optimize

from ._jet import DevelopingFlow
from ._therm import Combustion
from ._comps import Fluid
from ..utilities.custom_warnings import PhysicsWarning


class Flame:
    def __init__(self, fluid, orifice, ambient, mdot=None,
                 theta0=0., x0=0, y0=0,
                 nn_conserve_momentum=True, nn_T='solve_energy',
                 chem=None,
                 lamf=1.24, lamv=1.24, betaA=3.42e-2, alpha_buoy=5.75e-4, af=0.23,
                 T_establish_min=-1, verbose=False,
                 Smax=np.inf, dS=None, tol=1e-6, 
                 numB=5, n_pts_integral=100, 
                 wind_speed = 0):
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
        mdot: float, optional 
            should only be specified for subsonic release, mass flow rate (kg/s)
        theta0 : float, optional
            angle of release (rad) default value of 0 is horizontal
        x0 : float, optional
            horizontal starting point (m)
        y0 : float, optional
            vertical starting point (m)
        nn_conserve_momentum: boolean, optional
            whether notional nozzle model should conserve mass and momentum, or mass only,
            together with nn_T determines which notional nozzle model to use (see below)
        nn_T: string, optional
            either 'solve_energy', 'Tthroat' or specified temperature (stagnation temperature) 
            with nn_conserve_momentum leads to one of the following notinoal nozzle models:
            YuceilOtugen - conserve_momentum = True, T = 'solve_energy'
            EwanMoodie - conserve_momentum = False, T = 'Tthroat'
            Birch - conserve_momentum = False, T = T0
            Birch2 - conserve_momentum = True, T = T0
            Molkov - conserve_momentum = False, T = 'solve_energy'
        chem : chemistry class (see hc_therm for usage), optional
            if none given, will initialize new chemisty class
        lamf : float
            spreading ratio for mixture fraction Gaussian profile
        lamv : float
            spreading ratio for velocity Gaussian profile
        betaA : float
            momentum entrainment coefficient
        alpha_buoy : float
            buoyancy entrainment coefficient    
        af : float
            Plank's mean absorption coefficient for an optically thin flame
        T_establish_min: float, optional
            minimum temperature for start of integral model
        verbose : bool, optional
            If True, extra output will be printed (default True)
        Smax: float, optional
            limit of integration, integrator will stop when it reaches minimum of Flame.length() or Smax
        dS: float, optional
            integrator step size, if None, defaults to Smax/Flame.length() solver adds steps in high gradient areas
        tol: float, optional
            relative and absolute tolerance for integrator
        max_steps: float, optional
            maximum steps for integrator
        numB: float, optional
            maximum number of halfwidths (B) considered to be infinity - for integration in equations
        n_pts_integral: int, optional
            maximum number of points in integration (from 0 to numB)
        '''
        self.x, self.y, self.S = [], [], []
        self.developing_flow = DevelopingFlow(fluid, orifice, ambient, mdot,
                                              theta0=theta0, x0=x0, y0=y0,
                                              lam=lamf, betaA=betaA,
                                              nn_conserve_momentum=nn_conserve_momentum, nn_T=nn_T,
                                              T_establish_min=T_establish_min,
                                              verbose=verbose)
        self.initial_node = self.developing_flow.initial_node
        self.mass_flow_rate = self.developing_flow.mass_flow_rate
        expanded_plug_node = self.developing_flow.expanded_plug_node

        self.fluid, self.ambient = fluid, ambient
        self.Emom = betaA * np.sqrt(const.pi / 4.0 * expanded_plug_node.d ** 2 *
                                    expanded_plug_node.rho * expanded_plug_node.v ** 2 / ambient.rho)
        self.lamf, self.lamv, self.alpha_buoy = lamf, lamv, alpha_buoy
        self.chem = chem
        self.af = af
        self.verbose = verbose
        self.wind_speed = wind_speed
        self.solve(Smax, dS, tol, numB, n_pts_integral)

    def _govEqns(self, S, ind_vars, numB=5, n_pts_integral=100):
        '''
        Governing equations for a flame, written in terms of d/dS of (V_cl, B, theta, f_cl, x, and y).
        
        A matrix soluition to the continuity, x-momentum, y-mometum and mixture fraction equations
        solves for d/dS of the dependent variables V_cl, B, theta, and f_cl.  Numerically integrated
        to infinity = numB * B(S) using numpts discrete points.'''
        
        # break independent variables out of ind_vars
        [V_cl, B, theta, f_cl, x, y] = ind_vars
        
        # needed to integrate to infinity (numB*B):
        r = np.zeros(n_pts_integral)
        r[1:] = np.logspace(-7, np.log10(numB * B), n_pts_integral - 1)

        # mixture fraction and velocity have Gaussian shapes
        f = f_cl * np.exp(-(r / (self.lamf * B)) ** 2)
        V = V_cl * np.exp(-(r / (self.lamv * B)) ** 2)

        # density isn't a nice Gaussian, due to combustion 
        try:
            rho = self.chem.rho_prod(f)
            drhodf = self.chem.drhodf(f)
        except:
            warnings.warn('Clipping f - something has gone wrong.', category=PhysicsWarning)
            f = np.clip(f, 0, 1)
            rho = self.chem.rho_prod(f)
            drhodf = self.chem.drhodf(f)

        rho_int = integrate.trapz(self.ambient.rho - rho, r)

        Ebuoy = (2 * np.pi * self.alpha_buoy * np.sin(theta) * 
                    const.g * (rho_int) / (B * V_cl * self.developing_flow.fluid_exp.rho))  # m**2/s
        E = self.Emom + Ebuoy

        # right-hand side of governing equations:
        RHS = np.array([self.ambient.rho * E / (2 * const.pi),  # continuity
                        self.wind_speed * self.ambient.rho * E / (2 * const.pi),  # x-momentum
                        integrate.trapz((self.ambient.rho - rho) * const.g * r, r),  # y-momentum
                        0])  # mixture fraction

        zero = np.zeros_like(r)
        dfdS = np.array([zero, 2 * r ** 2 / self.lamf ** 2 / B ** 3 * f, zero, f / f_cl])
        dVdS = np.array([V / V_cl, 2 * r ** 2 / self.lamv ** 2 / B ** 3 * V, zero, zero])
        drhodS = drhodf * dfdS
        dthetadS = np.array([zero, zero, np.ones_like(r), zero])

        # left-hand side of governing equations:
        LHS = np.array([drhodS * V * r + rho * dVdS * r,  # continuity
                        drhodS * V ** 2 * np.cos(theta) * r + 2 * rho * V * dVdS * np.cos(
                            theta) * r + rho * V ** 2 * -np.sin(theta) * dthetadS * r,  # x-momentum
                        drhodS * V ** 2 * np.sin(theta) * r + 2 * rho * V * dVdS * np.sin(
                            theta) * r + rho * V ** 2 * np.cos(theta) * dthetadS * r,  # y-momentum
                        drhodS * V * f * r + rho * dVdS * f * r + rho * V * dfdS * r])  # mixture fraction
        LHS = integrate.trapz(LHS, r)
        
        dz = np.append(np.linalg.solve(LHS, RHS), np.array([np.cos(theta), np.sin(theta)]), axis=0)

        return dz

    def solve(self, Smax=np.inf, dS=None, tol=1e-6,
              numB=5, n_pts_integral=100):
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
        #ESH note: self.developing_flow.fluid_exp is at a much lower temperature than ambient and gives funky heat flux numbers if used in the Combustion object, hence initilization at ambient T and P - could be improved. 
        try:
            if self.chem.Treac != self.ambient.T or abs(self.chem.P / self.ambient.P - 1) > 1e-10:
                self.chem.reinitilize(Fluid(T = self.ambient.T, P = self.ambient.P, species = self.fluid.species))
        except:
            self.chem = Combustion(Fluid(T = self.ambient.T, P = self.ambient.P, species = self.fluid.species))

        if self.verbose:
            print('solving for the flame...', end='')
        Smax = min(Smax, self.length())

        Y_cl0 = self.initial_node.Y_cl
        f_cl0 = optimize.newton(lambda f: Y_cl0 - self.chem._Yreac(f)[self.chem.reac],
                                Y_cl0)  # ESH-04/05/21 - updated _Yprod in line above to _Yreac

        if dS == None:
            max_step = np.inf
            first_step = None
        else:
            max_step = dS
            first_step = dS
        sol = integrate.solve_ivp(self._govEqns, [self.initial_node.S, Smax], 
                                  np.array([self.initial_node.v_cl, self.initial_node.B, self.initial_node.theta, f_cl0, 
                                            self.initial_node.x, self.initial_node.y]),
                                  max_step = max_step, first_step = first_step,
                                  args = (numB, n_pts_integral),
                                  atol=tol, rtol=tol,
                                  method = 'LSODA'
                                  )

        result = dict(zip(['V_cl', 'B', 'theta', 'f_cl', 'x', 'y'], sol.y))
        result['S'] = sol.t
        for k, v in result.items():
            self.__dict__[k] = v
        if self.verbose:
            print('done.')
        return result

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
            if self.chem.Treac != self.ambient.T or abs(self.chem.P / self.ambient.P - 1) > 1e-10:
                self.chem.reinitilize(Fluid(T = self.ambient.T, P = self.ambient.P, species = self.fluid.species))
        except:
            self.chem = Combustion(Fluid(T = self.ambient.T, P = self.ambient.P, species = self.fluid.species))
        fs, Tad = self.chem.fstoich, self.chem.T_prod(self.chem.fstoich)
        Tamb = self.ambient.T
        rhoair, rhof = self.ambient.rho, self.chem.rho_prod(self.chem.fstoich)
        orifice1, gas1 = self.developing_flow.orifice_exp, self.developing_flow.fluid_exp
        Deff, rhoeff = orifice1.d, gas1.rho

        # Compute the flame Froude number
        Frf = (gas1.v * fs ** 1.5) / (((rhoeff / rhoair) ** 0.25) * np.sqrt(((Tad - Tamb) / Tamb) * const.g * Deff))
        self.Frf = Frf

        # Compute visible flame length
        Lstar = ((13.5 * Frf ** 0.4) / (1 + 0.07 * Frf ** 2) ** 0.2) * (Frf < 5) + 23 * (Frf >= 5)

        dstar = Deff * (rhoeff / rhoair) ** 0.5
        self.dstar = dstar

        self.Lvis = Lstar * dstar / fs  # visible flame length [m]
        self.Wf = 0.17 * self.Lvis
        # flame residence time [ms]

        self.tauf = (const.pi / 12) * (rhof * (self.Wf ** 2) * self.Lvis * fs) / (orifice1.mdot(gas1)) * 1000
        # self.Xrad = (0.08916*np.log10(self.tauf*self.af*Tad**4) - 1.2172) # comes from Molina et al.
        self.Xrad = 9.45e-9 * (self.tauf * self.af * Tad ** 4) ** 0.47  # see Panda, Hecht, IJHE 2016
        mdot = self.developing_flow.orifice_exp.mdot(self.developing_flow.fluid_exp)  # mass flow rate [kg/s]
        self.Srad = self.Xrad * mdot * self.chem.DHc

        return self.Lvis

    def Qrad_multi(self, x, y, z, RH, WaistLoc=0.75, N=50):
        '''
        MultiSource radiation model
        follows Hankinson & Lowesmith, CNF 159, 2012: 1165-1177       
        '''
        obsOrg = np.array([x, y, z]).T

        try:
            Lvis = self.Lvis  # length of visible flame [m]
        except:
            Lvis = self.length()
        T = self.ambient.T

        n = int(WaistLoc * N)
        w = np.arange(1, N + 1, dtype=float)
        w[n:] = (n - ((n - 1) / (N - (n + 1))) * (w[n:] - (n + 1)))
        w /= np.sum(w)

        try:
            S = np.linspace(self.S[0], min([self.S[-1], self.Lvis]), N)
            X = interpolate.interp1d(self.S, self.x)(S)
            Y = interpolate.interp1d(self.S, self.y)(S)
        except:
            warnings.warn('Running flame model with default parameters.', category=PhysicsWarning)
            self.solve()
            S = np.linspace(self.S[0], min([self.S[-1], self.Lvis]), N)
            X = interpolate.interp1d(self.S, self.x)(S)
            Y = interpolate.interp1d(self.S, self.y)(S)

        sourceOrg = np.array([X, Y, np.zeros_like(X)]).T

        Qrad = np.zeros(obsOrg.shape[:-1])

        for j in range(len(w)):
            v = sourceOrg[j] - obsOrg
            if obsOrg.ndim > 1:
                obsNorm = v / (np.linalg.norm(v, axis=1) + 1e-99)[:, np.newaxis]
            else:
                obsNorm = v / (np.linalg.norm(v) + 1e-99)

            if obsOrg.ndim == 4:
                len_v = np.linalg.norm(v, axis=3)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm, v), axis=3) / len_v)
            elif obsOrg.ndim == 3:
                len_v = np.linalg.norm(v, axis=2)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm, v), axis=2) / len_v)
            elif obsOrg.ndim == 2:
                len_v = np.linalg.norm(v, axis=1)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm, v), axis=1) / len_v)
            elif obsOrg.ndim == 1:
                len_v = np.linalg.norm(v)
                phi = np.arcsin(np.linalg.norm(np.cross(obsNorm, v)) / len_v)
            Qrad += w[j] / (4 * const.pi * len_v ** 2) * np.cos(phi) * self.Srad * calc_transmissivity(len_v, T, RH)

        return Qrad.T

    def _contourdata(self):
        iS = np.arange(len(self.S))
        r = np.append(
            np.append(-np.logspace(np.log10(10 * max(self.B)), -2), np.linspace(-10 ** -2.1, 10 ** -2.1, num=10)),
            np.logspace(-2, np.log10(10 * max(self.B))))
        r, iS = np.meshgrid(r, iS)
        B = self.B[iS]
        f_cl = self.f_cl[iS]

        fvals = f_cl * np.exp(-(r / (self.lamf * B)) ** 2)
        Tvals = self.chem.T_prod(fvals)

        x = self.x[iS] + r * np.sin(self.theta[iS])
        y = self.y[iS] - r * np.cos(self.theta[iS])
        return x, y, Tvals

    def plot_Ts(self, mark=None, mcolors='w',
                xlims=None, ylims=None,
                xlab='x (m)', ylab='y (m)',
                cp_params={}, levels=100,
                addColorBar=True, aspect=1, plot_title=None,
                fig_params={}, subplots_params={}, ax=None):
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
        else:
            fig = ax.figure

        x, y, T = self._contourdata()
        clrmap = 'plasma'
        ax.set_facecolor(plt.cm.get_cmap(clrmap)(0))
        cp = ax.contourf(x, y, T, levels, cmap=clrmap, **cp_params)
        if mark is not None:
            cp2 = ax.contour(x, y, T, levels=mark, colors=mcolors, linewidths=1.5, **cp_params)

        if xlims is not None:
            ax.set_xlim(*xlims)
        if ylims is not None:
            ax.set_ylim(*ylims)

        if addColorBar:
            cb = plt.colorbar(cp)
            cb.set_label('Temperature (K)', rotation=-90, va='bottom')
        else:
            cb = None

        ax.set_xlabel(xlab)
        ax.set_ylabel(ylab)
        if aspect is not None:
            ax.set_aspect(aspect)
        if plot_title is not None:
            ax.set_title(plot_title)
        fig.tight_layout()

        return fig, cb

    def plot_heat_flux_sliced(self, title='',
                              filename='HeatFluxSliced.png',
                              directory=os.getcwd(),
                              RH=0.89,
                              contours=None,
                              nx=50, ny=50, nz=50,
                              xlims=None, ylims=None, zlims=None,
                              WaistLoc=0.75,
                              savefig=True):
        '''
        plots slices of heat flux levels

        Parameters
        ----------
        title : string (optional)
            title shown on plot
        filename : string, optional
            file name to write
        directory : string, optional
            directory in which to save file
        RH : float
            relative humidity
        contours : ndarray or list (optional)
            contour levels shown on plot in kW/m^2 (default values are 2012 International Fire Code (IFC) exposure limits 
            for property lines (1.577 kW/m2), employees (4.732 kW/m2), and non-combustible equipment (25.237 kW/m2))
        nx, ny, nz: float (optional)
            number of points to solve for the heat flux in the x, y, and z directions
        xlims, ylims, zlims: tuple (optional)
            plot limits of (min, max) in each dimension 
        WaistLoc: float (optoinal)
            value between 0 and 1 along flame length at which to make xz slice
        savefig: boolean (optional)
            whether to save the figure as filename

        Returns
        -------
        If savefig is True, returns full filepath of saved figure;
        if savefig is False, returns figure object.
        '''
        if contours is None:
            contours = [1.577, 4.732, 25.237]
        Lvis = self.length()
        flameCen = np.array([np.interp(Lvis*WaistLoc, self.S, self.x),
                             np.interp(Lvis*WaistLoc, self.S, self.y),
                             0])
        if xlims is None:
            dx = 4.5 * Lvis / nx
            x0 = slice(self.x[0] - 1.5 * Lvis, (self.x[0] + 3 * Lvis), dx)
        else:
            dx = (xlims[1] - xlims[0]) / nx
            x0 = slice(xlims[0], xlims[1], dx)

        if ylims is None:
            dy = (self.y[0] + Lvis) / ny
            y0 = slice(0, (self.y[0] + Lvis), dy)
        else:
            dy = (ylims[1] - ylims[0]) / ny
            y0 = slice(ylims[0], ylims[1], dy)

        if zlims is None:
            dz = 4. * Lvis / nz
            z0 = slice(-2 * Lvis, (2 * Lvis), dz)
        else:
            dz = (zlims[1] - zlims[0]) / nz
            z0 = slice(zlims[0], zlims[1], dz)

        x, y, z = np.mgrid[x0, y0, z0]
        x_z, y_z = np.mgrid[x0, y0]
        x_y, z_y = np.mgrid[x0, z0]
        y_x, z_x = np.mgrid[y0, z0]

        fig = plt.figure(figsize=(8, 4.5))
        fig.subplots_adjust(top=0.967,
                            bottom=0.378)  # not sure if this will always be right---tight_layout incompatible with ImageGrid
        grid = ImageGrid(fig, 111,  # similar to subplot(111)
                         nrows_ncols=(2, 2),  # creates 2x2 grid of axes
                         axes_pad=0.1,  # pad between axes in inch.
                         label_mode="L",
                         cbar_mode='edge',
                         cbar_location='bottom',
                         cbar_size='10%',
                         cbar_pad=-1)
        ax_xy, ax_zy, ax_xz = grid[0], grid[1], grid[2]
        for ax in [ax_xy, ax_zy, ax_xz]:
            ax.cax.set_visible(False)
        ax_zy.axis['bottom'].toggle(all=True)
        grid[3].set_frame_on(False)
        grid[3].set_axis_off()
        ax_cb = grid[3].cax
        ax_cb.set_visible(True)

        fxy = self.Qrad_multi(x_z, y_z, flameCen[2] * np.ones_like(x_z), RH)
        fxz = self.Qrad_multi(x_y, flameCen[1] * np.ones_like(x_y), z_y, RH)
        fzy = self.Qrad_multi(flameCen[0] * np.ones_like(z_x), y_x, z_x, RH)
        fxy = fxy / 1000  # kW/m2 from W/m2
        fxz = fxz / 1000  # kW/m2 from W/m2
        fzy = fzy / 1000  # kW/m2 from W/m2

        ClrMap = copy.copy(plt.cm.get_cmap('RdYlGn_r'))
        ClrMap.set_under('white')
        
        if len(contours) == 2:
            vmin = min(contours) - abs(np.diff(contours)[0])
        else:
            vmin = min(contours)
        
        # xy axis:
        ax_xy.contourf(x_z, y_z, fxy, cmap=ClrMap,
                        levels=contours, extend='both', vmin = vmin)
        ax_xy.plot(self.x[self.S<=self.Lvis], self.y[self.S<=self.Lvis], color='k',  
                   linewidth=3, solid_capstyle='round')
        ax_xy.set_ylabel('Height (y) [m]')
        ax_xy.annotate('z = %0.2f' % flameCen[2], xy=(0.02, .98),
                        xycoords='axes fraction', va='top', color='k')
        # xz axis:
        ax_xz.contourf(x_y, z_y, fxz, cmap=ClrMap,
                        levels=contours, extend='both', vmin = vmin)
        ax_xz.plot(self.x[self.S<=self.Lvis], np.ones_like(self.x)[self.S<=self.Lvis] * flameCen[2], color='k', 
                   linewidth=3, solid_capstyle='round')
        ax_xz.set_xlabel('Horizontal Distance (x) [m]')
        ax_xz.set_ylabel('Perpendicular Distance (z) [m]')
        ax_xz.annotate('y = %0.2f' % flameCen[1], xy=(0.02, .98),
                        xycoords='axes fraction', va='top', color='k')
        # zy axis
        im = ax_zy.contourf(z_x, y_x, fzy, cmap=ClrMap,
                            levels=contours, extend='both', vmin = vmin)
        ax_zy.plot(np.ones_like(self.y[self.S<=self.Lvis]) * flameCen[2], self.y[self.S<=self.Lvis], color='k',
                   linewidth=3, solid_capstyle='round')
        ax_zy.set_xlabel('Perpendicular Distance (z) [m]')
        ax_zy.annotate('x = %0.2f' % flameCen[0], xy=(0.02, .98), xycoords='axes fraction',
                        va='top', color='k')
        # colorbar
        cb = plt.colorbar(im, cax=ax_cb, orientation='horizontal', extendfrac='auto')
        cb.set_label('Heat Flux [kW/m$^2$]')
        for ax in [ax_xy, ax_xz, ax_zy]:
            ax.minorticks_on()
            ax.grid(alpha=.2, color='k')
            ax.grid(which='minor', alpha=.1, color='k')
            ax.set_aspect(1)

        filepath = os.path.join(directory, filename)
        # fig.tight_layout(rect = [0, .08, 1, 1]) # tightlayout not compatable with ImageGrid
        if savefig:
            fig.savefig(filepath, dpi=200)
            plt.close(fig)
            return filepath
        else:
            return fig

    def generate_positional_flux(self, flux_coordinates, rel_humid):
        """ Calculate flux at positions according to radiative source model

        Parameters
        ----------
        flux_coordinates : list of locations
            List of locations at which to determine flux,
            each location is a tuple of 3 coordinates (m):
            [(x1, y1, z1), (x2, y2, z2), ...]

        rel_humid : flat
            relative humitidy

        Returns
        -------
        flux : ndarray
            flux values at specified positions (W/m^2)
        """
        
        x_values = [flux_coordinate[0] for flux_coordinate in flux_coordinates]
        y_values = [flux_coordinate[1] for flux_coordinate in flux_coordinates]
        z_values = [flux_coordinate[2] for flux_coordinate in flux_coordinates]
        flux = self.Qrad_multi(x_values, y_values, z_values, rel_humid)
        return flux

    def get_srad(self):
        """
        Returns : float
            Total emitted radiative power for flame (W)
        """
        return self.Srad

    def get_visible_length(self):
        """
        Returns : float
            Visible flame length (m)
        """
        return self.Lvis
    
    @property    
    def birds_eye_flame_length(self):
        '''
        returns : float 
            flame length from above
        '''
        return np.interp(self.Lvis, self.S, self.x)

    def x_distance_to_heat_flux_val(self, heat_flux_level, RH = 0.89, WaistLoc=0.75, xmax = 500):
        '''
        Parameters-
        ----------
        heat flux level: float
          heat flux level for which one wants to know the distance to (W/m^2)
        RH : float
            relative humidity
        WaistLoc: float  
          distance along flame at which to look for the x-distance
        xmax: float (optional)
          maximum distance to look for heat flux level
        
        Returns
        -------
        distance: float
          x distance to heat_flux_level (m)
        '''
        ycen = np.interp(self.Lvis*WaistLoc, self.S, self.y)
        xvals = np.linspace(xmax, 0, 10000)
        Q = self.Qrad_multi(xvals, ycen*np.ones_like(xvals), np.zeros_like(xvals), RH = RH)
        imax = np.argmax(Q)
        return np.interp(heat_flux_level, Q[:imax], xvals[:imax])


def calc_transmissivity(path_length, ambient_temperature, relative_humidity, atmospheric_CO2_ppm=335):
    '''
    Calculates atmospheric transmissivity from:
    Wayne, J. Loss Prev. Proc. Ind. 1991

    Parameters
    ----------
    path_length : float
        Path length (m) through which radiative light must travel

    ambient_temperature : float
        Ambient temperature (K)

    relative_humidity : float
        Fractional relative humidity (0-1)

    atmospheric_CO2_ppm : float
        Atmospheric CO2 concentration (ppm),
        default is 335 ppm
    
    Returns
    -------
    transmissivity : float
        Atmospheric transmissivity 
    '''
    sat_water_vap_pressure_mmHg = np.exp(20.386 - 5132 / ambient_temperature)  # mmHg, saturated vapor pressure from Wikipedia
    XH2O = relative_humidity * path_length * sat_water_vap_pressure_mmHg * 2.88651e2 / ambient_temperature
    XCO2 = path_length * 273. / ambient_temperature * atmospheric_CO2_ppm / 335.
    transmissivity = (1.006 - 0.01171 * np.log10(XH2O) - 0.02368 * np.log10(XH2O) ** 2
                       - 0.03188 * np.log10(XCO2) + 0.001164 * np.log10(XCO2) ** 2)
    return transmissivity
