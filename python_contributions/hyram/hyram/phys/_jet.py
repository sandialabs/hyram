#  Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
#  Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
#  .
#  This file is part of HyRAM (Hydrogen Risk Assessment Models).
#  .
#  HyRAM is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#  .
#  HyRAM is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#  .
#  You should have received a copy of the GNU General Public License
#  along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

from __future__ import print_function, absolute_import, division

import sys
import warnings

# Note: non-interactive agg backend set in GUI. Set MPLBACKEND env variable to change.
import matplotlib.pyplot as plt
import numpy as np
from scipy import integrate, optimize
from scipy.special import erf
import scipy.constants as const

from ._nn import NotionalNozzle


class PlugNode:
    def __init__(self, d, v, rho, Y, theta, x = 0, y = 0, S = 0):
        '''Plug flow node
        
        Parameters
        ----------
        d: float, diameter (m)
        v: float, velocity (m/s)
        rho: float, density (kg/m^3)
        Y: float, mass fraction H2
        theta: float, release angle from horizontal (rad)
        x: float, x position (m),
        y: float, y position (m),
        S: float, distance along streamline (m)
        '''
        self.d, self.v, self.rho, self.Y = d, v, rho, Y
        self.theta, self.x, self.y, self.S = theta, x, y, S
    
    def Froude(self, rho_amb):
        '''Froude number'''
        return self.v/np.sqrt(const.g*self.d*(rho_amb - self.rho)/self.rho)
    
    def establish(self, betaS, betaB, lam, MW_gas, MW_ambient, rho_ambient, 
                  newMethod = True):
        '''
        returns the gaussian node after flow establishment
        '''
        if newMethod:
            SE = betaS * self.d
            BE = betaB * self.d
            Y_clE = (lam ** 2 + 1) / (2 * lam ** 2)
            rho_clE = rho_ambient/(Y_clE*(MW_ambient-MW_gas)/MW_gas + 1)
            V_clE = (lam**2 + 1)/lam**2*self.rho*self.d**2*self.v/(4*rho_clE*Y_clE*BE**2)
            thetaE = self.theta
            xE = self.x + SE*np.cos(thetaE)
            yE = self.y + SE*np.sin(thetaE)
            SE = self.S + SE
        else:
            #old establishment parameters
            V_clE = self.v
            BE = self.d/np.sqrt(2*(2*lam**2+1)/(lam**2*self.rho/rho_ambient + lam**2 + 1))
            SE = 6.2*self.d
            Y_clE = (lam**2+1.)/(2.*lam**2)
            rho_clE = ((MW_ambient - MW_gas)/MW_gas*Y_clE + 1.)**-1*rho_ambient
            xE = self.x + SE*np.cos(self.theta)
            yE = self.y + SE*np.sin(self.theta)
            SE = self.S + SE
            thetaE = self.theta            
        return GaussianNode(BE, V_clE, rho_clE, Y_clE, thetaE, xE, yE, SE)

class GaussianNode:
    def __init__(self, B, v, rho, Y, theta, x = 0, y = 0, S = 0):
        '''
        Describes the properties of a node 
        
        Parameters
        ----------
        B: float
            Gaussian halfwidth (m)
        v : float
            velocity at centerline (m/s)
        rho : float
            density at centerline (kg/m^3)
        Y : float
            mass fraction at centerline (kg/kg)
        theta: float
            angle of jet (rad, 0 is horizontal)
        x: float
            horizontal postion of node (m)
        y: float
            vertical postion of node (m)
        S : float (optional)
            length along jet(m)
        '''
        self.B, self.v, self.rho, self.Y, self.S, self.theta, self.x, self.y = B, v, rho, Y, S, theta, x, y
        
    def _conditions(self):
        return np.array([self.v, self.B, self.rho, self.Y, self.theta, self.x, self.y])    
        
    def Ebuoy(self, rho_amb, Fr):
        '''
        returns the entrainment rate at a node due to buoyancy
        
        Parameters
        ----------
        rho_amb: float
            ambient density (kg/m^3)
        Fr: float
            Froude number (dimensionless)
            
        Returns
        -------
        Ebuoy: float
            buoyancy driven entrainment rate (m^2/s)
        '''
        # Definition of Local Froude number from Bill Houf's code
        if Fr < 268: 
            alpha_buoy = 17.313-0.11665*Fr+(2.0771e-4)*Fr**2 
        else:
            alpha_buoy = 0.97
        
        FrL     = self.v**2*self.rho/(const.g*self.B*(rho_amb-self.rho))
        Ebuoy   = alpha_buoy/FrL*(2*const.pi*self.v*self.B)*np.sin(self.theta)  # m**2/s
        return Ebuoy
    
    def Etot(self, Emom, rho_amb, Fr, alpha, onOff):
        '''
        returns the total entrainment rate at a node
        
        Parameters
        ----------
        Emom: float
            momentum driven entrainment rate
        rho_amb: float
            ambient density (kg/m^3)
        Fr: float
            Froude number (dimensionless)
        alpha: float
            number that controls rate of entrainment
        onOff: object (contains val)
            object that keeps track of entrainment rate
            
        Returns
        -------
        E: float
            total entrainment rate (m^2/s)
        '''
        E = Emom + self.Ebuoy(rho_amb, Fr)
        alphatest = E/(2*const.pi*self.B*self.v)
        if alphatest > alpha:
            onOff.val = 1
        if onOff.val == 1:
            E = (2*const.pi*self.B*self.v)*alpha
        return E

class Jet:
    def __init__(self, gas, orifice, ambient, Q = None,
                 theta0 = 0, x0 = 0., y0 = 0.,
                 lam  = 1.5, betaA = 0.28, betaS = 8.6, betaB = 0.8,
                 nnmodel = 'YuceilOtugen', 
                 Ymin = 1e-4, max_steps = 5000, tol = 1e-5, Smax = np.inf,
                 dS = None, verbose = True):
        '''
        Class for solving for a 2D gas jet. Gas pressure should be >= 2 x ambient pressure.
        If gas pressure is <= 2 x ambient pressure, use subsonic initilization.
        Uses the relationships in Li, Hecht, Christopher, IJHE 2015 for establishment.
                
        Parameters
        ----------
        gas: gas object
            the gas that is being released
        orifice: orifice object
            the release
        ambient: gas object
            the gas into which the release occurs
        Q: float, optional defaults to None
            volumetric flow rate (m^3/s). Only used if flow is not choked.
        theta0 : float, optional defaults to horizontal
            angle of release, radians (0 is horizontal, pi/2 is vertical)
        x0: float, optional
            horizontal starting location (m)
        y0: float, optional
            vertical starting location (m)     
        lam : float, optional
            Relative spreading ratio of concentration to velocity
        betaA : float, optional
            proportionality constant for air entrainment (see Li et al IJHE 2015)
        betaS : float, optional
            proportionality constant for starting location (see Li et al IJHE 2015)
        betaB : float, optional
            proportionality constant for starting width (see Li et al IJHE 2015)
        nnmodel : string
            notional nozzle model to use (default is YuceilOtugen)   
        Ymin: float, optional
            minimum mass fraction out to which model will integrate
        max_steps: float, optional
            maximum steps along the S axis
        tol: float, optional
            tolerance for Runga-Kutta integration 
        Smax: float, optional
            maximum distance out to which will proceed (integration stops after S > Smax or Y_cl < Ymin)
        dS: float, optional
            integration step (default is notional nozzle or real nozzle diameter)
        '''
        self.verbose = verbose
        S0 = 0
        CriticalRatio = gas.therm.critical_ratio()
        if gas.P >= CriticalRatio*ambient.P and Q is not None:
            warnings.warn('overspecified...calculating for choked flow')
            Q = None
        if gas.P < CriticalRatio*ambient.P and Q is None:
            warnings.warn('underspecified, unchoked, please specify flow rate')
            return
        if gas.P >= CriticalRatio*ambient.P and Q is None:
            # Notional nozzle:
            nn = NotionalNozzle(gas, orifice, ambient)
            rho0, T0, v0, mdot0, p0 = nn.throat()
            self.gas1, self.orifice1, self.V1 = nn.calculate(nnmodel)
        else:
            rho0, T0, v0, mdot0, p0 = gas.rho, gas.T, Q/orifice.A, Q*gas.rho, gas.P
            self.gas1, self.orifice1, self.V1 = gas, orifice, v0
        
        # Testing difference between new and old establishment method--doesn't seem to make much difference in this context.
        newMethod = True
        
        Y1 = gas.therm.MW/(ambient.therm.MW - gas.therm.MW)*(ambient.rho/self.gas1.rho - 1)
        self.node1 = PlugNode(self.orifice1.d, self.V1, self.gas1.rho, Y1, theta0, x0, y0, S0)
        
        Fr = v0/np.sqrt(const.g*orifice.d*abs(ambient.rho - rho0)/rho0)
        if Fr < 268:
            self.alpha_buoy = 17.313-0.11665*Fr+(2.0771e-4)*Fr**2
        else:
            self.alpha_buoy = 0.97
        self.Emom = betaA*np.sqrt(const.pi/4.0*orifice.d**2*rho0*v0**2/ambient.rho)
        
        # From plug flow generate boundary conditions:
        node2 = self.node1.establish(betaS, betaB, lam, gas.therm.MW, ambient.therm.MW, ambient.rho,
                                     newMethod = newMethod)
        
        class OnOff:
            def __init__(self):
                self.val = 0
        self._onOff = OnOff()
        
        self.initial_node = node2
        self.Fr, self.lam, self.gas, self.ambient = Fr, lam, gas, ambient
        self.mdot0, self.rho0 = mdot0, rho0
        self.V_cl, self.B, self.rho_cl, self.Y_cl = [], [], [], []
        self.theta, self.x, self.y, self.S = [], [], [], []
        if dS is None:
            dS = self.orifice1.d
        self.solve(Ymin, dS, max_steps, tol, Smax = Smax)
    
    def solve(self, Ymin, dS, max_steps, tol, model = 'no_energy', Smax = np.inf):
        '''
        solves (integrates) the model equations from the initial node out to limit
        '''

        if self.verbose:
            print('solving for the plume... ', end='')
            sys.stdout.flush()
        
        self._onOff.val = 0
        if model == 'no_energy':
            r = integrate.ode(self._govEqns_no_energy).set_integrator('dopri5', atol = tol, rtol = tol)
        elif model == 'energy':
            r = integrate.ode(self._govEqns_energy).set_integrator('dopri5', atol = tol, rtol = tol)
        #print(self.initial_node._conditions())
        r.set_initial_value(self.initial_node._conditions(), self.initial_node.S)
        
        i = 0
        T, Y = [r.t], [r.y]
        # note r.y[3] is Y_cl
        while r.successful() and r.y[3] > Ymin and i < max_steps and r.t < Smax:
            r.integrate(r.t + dS)
            T.append(r.t)
            Y.append(r.y)
            i += 1
        
        Y = np.array(Y)
        for key, val in zip(['V_cl', 'B', 'rho_cl', 'Y_cl', 'theta', 'x', 'y'], Y.T):
            self.__dict__[key] = val
        self.__dict__['S'] = np.array(T)
        
        if self.verbose:
            print('done.')
            #print('done.', end='')
            sys.stdout.flush()

        return self
    
    def solveLS(self, U0R0rho0, Z0, limit, numpoints, tol):
        '''
        solves (integrates) the model equations from the initial node out to limit
        '''
        
        r = integrate.ode(self._govEqns_Lane_Serff).set_integrator('dopri5', atol = tol, rtol = tol)
        
        r.set_initial_value(U0R0rho0, Z0)
        
        dt = limit/numpoints
        T, Y = [r.t], [r.y]
        while r.successful() and r.t < limit:
            r.integrate(r.t + dt)
            T.append(r.t)
            Y.append(r.y)
        
        Y = np.array(Y)
        for key, val in zip(['V_cl', 'B', 'rho_cl'], Y.T):
            self.__dict__[key] = val
        self.__dict__['y'] = np.array(T)
        self.__dict__['S'] = np.array(T)
        self.__dict__['x'] = np.zeros_like(np.array(T))
        return self
    
    def _govEqns_Lane_Serff(self, S, ind_vars, alpha = 0.05, Yamb = 0., numB = 5, numpts = 3000):
        
        #Governing equations for a plume, written in terms of d/dS of (Uj, R, rho).
        # But only for straight up: theta = pi/2.
        
        #From the paper: "Forced, angled plumes" by Lane-Serff, Linden, Hillel
        #A matrix solution to the continuity, (x-momentum=0), y-momentum, (no species), and buoyancy
        #equations solves for d/dS of the dependent variables V_cl, B, rho_cl, (no Y_cl,  and Theta).
        
        # break independent variables out of ind_vars
        [Uj, R, rho] = ind_vars
        
        rho_amb, Emom, MW_air, MW_gas = self.ambient.rho, self.Emom, self.ambient.therm.MW, self.gas.therm.MW
        Fr, onOff, lam = self.Fr, self._onOff, self.lam
        lam=1.1 #overwrite to match paper
        theta = const.pi/2. #overwrite to match paper
        gp = (rho_amb - rho)/rho_amb*const.g
        
        eq1 = np.array([R**2,  #dUj/dS
                     2*Uj*R, #dR/dS
                     0      #drho/ds
                     ])
        
        eq2 = np.array([2*Uj*R**2, #dUj/dS
                     2*Uj**2*R, #dUj/dS
                     0
                    ])
        eq3 = np.array([gp*R**2, #dUj/dS
                     gp*Uj*2*R, #dR/dS
                     -Uj*R**2*const.g/rho_amb #drho/ds
                    ])
        lhs = np.array([eq1, eq2, eq3])
        rhs = np.array([2*R*alpha*Uj, gp*(lam*R)**2, 0.])
        
        dz = np.linalg.solve(lhs,rhs)
        
        return dz
    
    def _govEqns_no_energy(self, S, ind_vars, alpha = 0.082, Yamb = 0., numB = 5, numpts = 3000):
        '''
        Governing equations for a plume, written in terms of d/dS of (V_cl, B, rho_cl, Y_cl, 
        theta, x, and y).
        
        A matrix solution to the continuity, x-momentum, y-momentum, species, and rho*Y 
        equations solves for d/dS of the dependent variables V_cl, B, rho_cl, Y_cl,  and Theta.
        '''
        
        # break independent variables out of ind_vars
        [V_cl, B, rho_cl, Y_cl, theta, x, y] = ind_vars
        node_in = GaussianNode(B, V_cl, rho_cl, Y_cl, theta, S = S)
        
        rho_amb, Emom, MW_air, MW_gas = self.ambient.rho, self.Emom, self.ambient.therm.MW, self.gas.therm.MW
        Fr, onOff, lam = self.Fr, self._onOff, self.lam
        
        E = node_in.Etot(Emom, rho_amb, Fr, alpha, onOff)
        
        # governing equations:
        LHScont = np.array([(lam**2*rho_cl + rho_amb)*B**2,                           #d/dS(V_cl)
                         2*(lam**2*rho_cl + rho_amb)*B*V_cl,                          #d/dS(B)
                         lam**2*B**2*V_cl,                                            #d/dS(rho_cl)
                         0.                                                           #d/dS(theta)
                         ])*const.pi/(lam**2 + 1)
        RHScont = rho_amb*E
        
        LHSxmom = np.array([(2*lam**2*rho_cl+rho_amb)*B**2*V_cl*np.cos(theta),           #d/dS(V_cl)
                         (2*lam**2*rho_cl+rho_amb)*B*V_cl**2*np.cos(theta),              #d/dS(B)
                         lam**2*B**2*V_cl**2*np.cos(theta),                              #d/dS(rho_cl)
                         -(2*lam**2*rho_cl+rho_amb)*(B*V_cl)**2*np.sin(theta)/2          #d/dS(theta)
                         ])*const.pi/(2*lam**2+1)        
        RHSxmom = 0.
        
        LHSymom = np.array([(2*lam**2*rho_cl+rho_amb)*B**2*V_cl*np.sin(theta),            #d/dS(V_cl)
                         (2*lam**2*rho_cl+rho_amb)*B*V_cl**2*np.sin(theta),               #d/dS(B)
                         lam**2*B**2*V_cl**2*np.sin(theta),                               #d/dS(rho_cl)
                         (2*lam**2*rho_cl+rho_amb)*(B*V_cl)**2*np.cos(theta)/2            #d/dS(theta)
                         ])*const.pi/(2*lam**2+1)                                 
        RHSymom = -const.pi*lam**2*const.g*(rho_cl - rho_amb)*B**2
        
        LHSspec = np.array([B**2*(rho_amb - rho_cl),                                   #d/dS(V_cl)
                         2*B*V_cl*(rho_amb - rho_cl),                                  #d/dS(B)
                         -B**2*V_cl,                                                   #d/dS(rho_cl)
                         0.                                                            #d/dS(theta)
                         ])*const.pi*lam**2/(lam**2 + 1)*(MW_gas/(MW_air-MW_gas))                        
        RHSspec = Yamb*rho_amb*E
        
        LHS = np.array([LHScont,
                     LHSxmom,
                     LHSymom,
                     LHSspec])
        RHS = np.array([RHScont,
                     RHSxmom,
                     RHSymom,
                     RHSspec])
        
        dz = np.append(np.linalg.solve(LHS,RHS), np.array([np.cos(theta), np.sin(theta)]), axis = 0)
        drho_cl_dS = dz[2]
        dY_cl_dS = -rho_amb/rho_cl**2*MW_gas/(MW_air - MW_gas)*drho_cl_dS
        dz = np.insert(dz, 3, dY_cl_dS)
        
        return dz
    
    def _govEqns_energy(self, S, ind_vars, 
                        alpha = 0.082, Yamb = 0., numB = 5, numpts = 500):
        '''
        Governing equations for a plume, written in terms of d/dS of (V_cl, B, rho_cl, Y_cl, 
        theta, x, and y).
        
        A matrix solution to the continuity, x-momentum, y-momentum, species, and energy 
        equations solves for d/dS of the dependent variables V_cl, B, rho_cl, Y_cl,  and Theta.  
        Numerically integrated to infinity = numB * B(S) using numpts discrete points.
        '''
        # break independent variables out of ind_vars
        [V_cl, B, rho_cl, Y_cl, theta, x, y] = ind_vars
        node_in = GaussianNode(B, V_cl, rho_cl, Y_cl, theta, S = S)
        
        rho_amb, Emom, MW_air, MW_gas = self.ambient.rho, self.Emom, self.ambient.therm.MW, self.gas.therm.MW
        Fr, onOff, lam = self.Fr, self._onOff, self.lam
        Pamb, h_amb = self.ambient.P, self.ambient.therm.h(self.ambient.T, self.ambient.T)
        Cp_gas, Cp_air = self.gas.therm.h(self.gas.T, self.gas.P)/self.gas.T, h_amb/self.ambient.T
        
        E = node_in.Etot(Emom, rho_amb, Fr, alpha, onOff)
        
        # some stuff needed to integrate to infinity (numB*B):
        #r = linspace(0, numB*B, numpts)
        r = np.append(np.array([0]), np.logspace(-3, np.log10(numB*B), numpts))
        zero = np.zeros_like(r)
        V       = V_cl*np.exp(-(r**2)/(B**2))
        dVdS = np.array([V/V_cl,                                              #d/dS(V_cl)
                      2*V*r**2/B**3,                                          #d/dS(B)
                      zero,                                                   #d/dS(rho_cl) 
                      zero,                                                   #d/dS(Y_cl)
                      zero])                                                  #d/dS(theta)
        rho     = (rho_cl - rho_amb)*np.exp(-(r**2)/((lam*B)**2))+rho_amb
        Y       = Y_cl*rho_cl*np.exp(-(r**2)/((lam*B)**2))/rho
        dYdS = np.array([zero,                                                #d/dS(V_cl)
                      (2*Y**2*rho_amb*r**2*np.exp(r**2/(lam*B)**2)/
                       (lam**2*B**3*Y_cl*rho_cl)),                            #d/dS(B)
                      Y**2*rho_amb*(np.exp(r**2/(lam*B)**2)-1)/(Y_cl*rho_cl**2), #d/dS(rho_cl)
                      Y/Y_cl,                                                 #d/dS(Y_cl)
                      zero])                                                  #d/dS(theta)
        Cp      = Y*(Cp_gas-Cp_air) + Cp_air
        dCpdS   = (Cp_gas - Cp_air)*dYdS
        MW      = MW_air*MW_gas/(Y*(MW_air-MW_gas) + MW_gas)
        dMWdS   = (MW*(MW_air - MW_gas)/(MW_gas*(Y-1) - MW_air*Y))*dYdS   
        rhoh    = Pamb/const.R*MW/1000*Cp
        drhohdS = Pamb/const.R*(MW*dCpdS + Cp*dMWdS)/1000
        
        # governing equations:
        LHScont = np.array([(lam**2*rho_cl + rho_amb)*B**2,                   #d/dS(V_cl)
                         2*(lam**2*rho_cl + rho_amb)*B*V_cl,                  #d/dS(B)
                         lam**2*B**2*V_cl,                                    #d/dS(rho_cl)
                         0.,                                                  #d/dS(Y_cl)
                         0.])*const.pi/(lam**2 + 1)                           #d/dS(theta)
        RHScont = rho_amb*E
        
        LHSxmom = np.array([(2*lam**2*rho_cl+rho_amb)*B**2*V_cl*np.cos(theta), #d/dS(V_cl)
                         (2*lam**2*rho_cl+rho_amb)*B*V_cl**2*np.cos(theta),   #d/dS(B)
                         lam**2*B**2*V_cl**2*np.cos(theta),                   #d/dS(rho_cl)
                         0.,                                                  #d/dS(Y_cl)
                         -(2*lam**2*rho_cl+rho_amb)*(B*V_cl)**2*np.sin(theta)/2  #d/dS(theta)
                         ])*const.pi/(2*lam**2+1)        
        RHSxmom = 0.
        
        LHSymom = np.array([(2*lam**2*rho_cl+rho_amb)*B**2*V_cl*np.sin(theta), #d/dS(V_cl)
                         (2*lam**2*rho_cl+rho_amb)*B*V_cl**2*np.sin(theta),   #d/dS(B)
                         lam**2*B**2*V_cl**2*np.sin(theta),                   #d/dS(rho_cl)
                         0.,                                                  #d/dS(Y_cl)
                         (2*lam**2*rho_cl+rho_amb)*(B*V_cl)**2*np.cos(theta)/2 #d/dS(theta)
                         ])*const.pi/(2*lam**2+1)                                 
        RHSymom = -const.pi*lam**2*const.g*(rho_cl - rho_amb)*B**2
        
        LHSspec = np.array([B**2*Y_cl*rho_cl,                                 #d/dS(V_cl)
                         2*B*V_cl*Y_cl*rho_cl,                                #d/dS(B)
                         B**2*V_cl*Y_cl,                                      #d/dS(rho_cl)
                         B**2*V_cl*rho_cl,                                    #d/dS(Y_cl)
                         0.,                                                  #d/dS(theta)
                         ])*const.pi*lam**2/(lam**2 + 1)                        
        RHSspec = Yamb*rho_amb*E
        
        LHSener = 2*const.pi*integrate.trapz(V*drhohdS*r + rhoh*dVdS*r, r)
        LHSener += [const.pi/(6*lam**2 + 2)*(2*lam**2*rho_cl+rho_amb)*B**2*V_cl**2, #d/dS(V_cl)
                    const.pi/(9*lam**2 + 3)*(3*lam**2*rho_cl+rho_amb)*V_cl**3*B, #d/dS(B)
                    const.pi/(6*lam**2 + 2)*lam**2*B**2*V_cl**3,              #d/dS(rho_cl)
                    0.,                                                       #d/dS(Y_cl)
                    0.]                                                       #d/dS(theta)
        
        RHSener = h_amb*RHScont
        
        LHS = np.array([LHScont,
                     LHSxmom,
                     LHSymom,
                     LHSspec,
                     LHSener
                     ])
        RHS = np.array([RHScont,
                     RHSxmom,
                     RHSymom,
                     RHSspec,
                     RHSener])
        
        dz = np.append(np.linalg.solve(LHS,RHS), np.array([np.cos(theta), np.sin(theta)]), axis = 0)
        
        return dz
    
    def reshape(self, enclosure, showPlot = False):
        '''
        reshapes the plume to turn, should it hit the enclosure wall, and crops it so it stops at the ceiling
        '''
        if np.any(self.x > enclosure.Xwall):
            iwall = np.argmax(self.x > enclosure.Xwall)
            dydx = (self.y[iwall] - self.y[iwall-1])/(self.x[iwall] - self.x[iwall-1])
            dSdx = (self.S[iwall] - self.S[iwall-1])/(self.x[iwall] - self.x[iwall-1])
            self.y[iwall] = self.y[iwall-1] + (enclosure.Xwall - self.x[iwall-1])*dydx
            self.S[iwall] = self.S[iwall-1] + (enclosure.Xwall - self.x[iwall-1])*dSdx
            self.y[iwall+1:] = self.y[iwall] + self.S[iwall+1:] - self.S[iwall]
            self.x[iwall:] = enclosure.Xwall
        if np.any(self.y > enclosure.H):
            iceil = np.argmax(self.y > enclosure.H)
            percent = (enclosure.H - self.y[iceil - 1]) / (self.y[iceil] - self.y[iceil - 1])
            for k in ['S', 'rho_cl', 'V_cl', 'Y_cl', 'B', 'theta', 'x', 'y']:
                self.__dict__[k][iceil] = self.__dict__[k][iceil - 1] + (self.__dict__[k][iceil] - 
                                                                         self.__dict__[k][iceil - 1])*percent
                self.__dict__[k] = self.__dict__[k][:iceil+1]
        if showPlot == True:
            plt.plot(self.x,self.y)
        return self
    
    
    def m_flammable(self, H = np.inf, Xi_lean = 0.04):
        '''
        calculates the amount of mass in the plume that is above the LFL
        
        Parameters
        ----------
        plume: plume class
          class of plume results
        H: max height [m]
        
        Outputs
        -------
        mass: float
          flammable mass in plume, up to height H (kg)
        '''   
        
        MW_gas = self.gas.therm.MW
        MW_air = self.ambient.therm.MW
        Ylean  = Xi_lean*MW_gas/(Xi_lean*MW_gas+(1-Xi_lean)*MW_air)
        
        # First, need to trim the plume down to below H:    
        S = np.copy(self.S)
        
        itop = np.argmax(self.y > H)
        if itop == 0: # the enclosure is full
            return 0.
        
        dSdy    = (self.S[itop] - self.S[itop-1])/(self.y[itop] - self.y[itop-1])
        S[itop] = self.S[itop-1] + (H - self.y[itop-1])*dSdy
        # Now trim it to only where Y_cl > the lean limit
        iflam   = np.argwhere(self.Y_cl[:itop] > Ylean)[-1][0]
        S       = self.S[:iflam]
        ivals = np.arange(1, len(S))
        
        def Y_calc(r, i):
            'mole fraction at radius: r, for plume: plume, at node: i'
            rho = (self.rho_cl[i]-self.ambient.rho)*np.exp(-r**2/(self.lam*self.B[i])**2)+self.ambient.rho
            Y = self.rho_cl[i]*self.Y_cl[i]*np.exp(-r**2/(self.lam*self.B[i])**2)/rho
            return Y
        
        # radius of flammable concentration at each node:
        r = np.array(list(map(lambda i: optimize.brentq(lambda r: Y_calc(r, i) - Ylean, 0, 10*self.B[i]), ivals)))
        
        ### not quite sure how this MEAN calculation works---copied from Isaac:
        MEAN = 0.5*np.sqrt(const.pi)*self.B[ivals]*self.lam*erf(r/(self.B[ivals]*self.lam))/r
        #MEAN = integral(np.exp(-(r/(lam*B))**2), (r, 0, r))/r
        rhoavg = (self.rho_cl[ivals]-self.ambient.rho)*MEAN + self.ambient.rho
        Yavg = self.rho_cl[ivals]*self.Y_cl[ivals]/rhoavg*MEAN
        
        #each node is taken to be a cylinder:
        dS = S[ivals] - S[ivals - 1]
        mass = rhoavg*Yavg*const.pi*r**2*dS
        
        return mass.sum()
    
    def _contourdata(self, includev = False):
        iS = np.arange(len(self.S))
        
        # Calculates logspaced points around 0 out to np.log10(3*np.max(self.B))
        # poshalf[::-1] just notation for reversing a numpy array
        poshalf = np.logspace(-5, np.log10(3*np.max(self.B)))
        r = np.concatenate((-1.0 * poshalf[::-1], [0], poshalf))
        
        r, iS = np.meshgrid(r, iS)
        B = self.B[iS]
        rho_cl = self.rho_cl[iS]
        Y_cl = self.Y_cl[iS]
        V_cl = self.V_cl[iS]                    
        
        rho_amb, Tamb, Pamb = self.ambient.rho, self.ambient.T, self.ambient.P
        MW_gas, MW_air = self.gas.therm.MW, self.ambient.therm.MW       
        
        rho = rho_amb + (rho_cl - rho_amb)*np.exp(-r**2/self.lam**2/B**2)
        Y   = Y_cl*rho_cl*np.exp(-(r**2)/((self.lam*B)**2))/rho
        MW  = MW_air*MW_gas/(Y*(MW_air-MW_gas) + MW_gas)
        
        X = Y*MW/MW_gas
        v = V_cl*np.exp(-(r**2)/(B**2))                               
        
        x = self.x[iS] + r*np.sin(self.theta[iS])
        y = self.y[iS] - r*np.cos(self.theta[iS])
        if not includev:
            return x, y, X, Y
        else:
            return x, y, X, Y, v
    
    def plot_moleFrac_Contour(self, mark = [0.04], mcolors = 'w', xlims = None, 
                              ylims = None, xlab = 'x (m)', ylab = 'y (m)', 
                              plot_title = None, vmin = 0, vmax = 0.1, 
                              addColorBar = True, aspect = 1, fig_params = {}, 
                              subplots_params = {}, ax = None):
        '''
        makes mole fraction contour plot
        
        Parameters
        ----------
        mark: list, optional
            levels to draw contour lines (mole fractions, or None if None desired)
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
        
        # Make figure and axis if not specified
        if ax is None:
            fig, ax = plt.subplots(**fig_params)
            plt.subplots_adjust(**subplots_params)
        
        # Get background color for contour
        ax.set_facecolor(plt.cm.get_cmap()(0)) #old matplotlib: ax.set_axis_bgcolor
        
        # Get contour data to plot
        x, y, X, __ = self._contourdata()
        
        # Plot contour data
        if np.amax(X) > vmax and np.amin(X) < vmin:
            ExtStr = 'both'
        elif np.amax(X) > vmax and np.amin(X) >= vmin:
            ExtStr = 'max'
        elif np.amax(X) <= vmax and np.amin(X) < vmin:
            ExtStr = 'min'
        else:
            ExtStr = 'neither'
        contourstep = 0.001
        contourlevels = np.arange(vmin, vmax + contourstep, contourstep)
        cp = ax.contourf(x, y, X, contourlevels, extend = ExtStr)
        
        # Add specific contours if desired
        if mark is not None:
            ax.contour(x, y, X, levels = mark, colors = mcolors, linewidths = 1.5)
            if len(mark)==2:
                LabelStr = 'White contours \n are at {} and {}'.format(mark[0],mark[1])
            elif len(mark)==1:
                LabelStr = r'White contour is at {}'.format(mark[0])
            ax.text(0.5, 0.9, LabelStr, color = 'white',
                    horizontalalignment = 'center', 
                    verticalalignment = 'center', 
                    transform = ax.transAxes)
        
        # Change axis limits if specified
        if xlims is not None:
            ax.set_xlim(*xlims)
        if ylims is not None:
            ax.set_ylim(*ylims)
        
        # Add colorbar if desired
        if addColorBar:
            cb = plt.colorbar(cp)
            cb.set_label('Hydrogen Mole Fraction', rotation = -90, va = 'bottom')
        
        # Set axis labels
        ax.set_xlabel(xlab)
        ax.set_ylabel(ylab)
        
        # Set aspect ratio if specified
        if aspect is not None:
            ax.set_aspect(aspect)
        
        # Set plot title if specified
        if plot_title is not None:
            ax.set_title(plot_title)
        
        return fig
    
    def plot_massFrac_Contour(self, mark = None, mcolors = 'w', 
                              xlims = None, ylims = None,
                              xlab = 'x (m)', ylab = 'y (m)',
                              vmin = 0, vmax = 1, levels = 100,
                              addColorBar = True, aspect = 1, 
                              fig_params = {}, subplots_params = {}, ax = None):
        '''
        makes mole fraction contour plot
        
        Parameters
        ----------
        mark: list, optional
            levels to draw contour lines (mass fractions, or None if None desired)
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
        ax.set_facecolor(plt.cm.get_cmap()(0)) #old matplotlib: ax.set_axis_bgcolor
        x, y, __, Y = self._contourdata()
        cp = ax.contourf(x, y, Y, levels, vmin = vmin, vmax = vmax)
        if mark is not None:
            ax.contour(x, y, Y, levels = mark, colors = mcolors, linewidths = 1.5)
        
        if xlims is not None:
            ax.set_xlim(*xlims)
        if ylims is not None:
            ax.set_ylim(*ylims)
        
        if addColorBar:
            cb = plt.colorbar(cp)
            cb.set_label('Hydrogen Mass Fraction', rotation = -90, va = 'bottom')
        ax.set_xlabel(xlab)
        ax.set_ylabel(ylab)
        if aspect is not None:
            ax.set_aspect(aspect)
        return fig
    
    def plot_velocity_Contour(self, mark = None, mcolors = 'w', 
                              xlims = None, ylims = None,
                              xlab = 'x (m)', ylab = 'y (m)',
                              levels = 100,
                              addColorBar = True, aspect = 1, 
                              fig_params = {}, subplots_params = {}, ax = None, **kwargs):
        '''
        makes mole fraction contour plot
        
        Parameters
        ----------
        mark: list, optional
            levels to draw contour lines (mass fractions, or None if None desired)
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
        ax.set_axis_bgcolor(plt.cm.get_cmap()(0))
        x, y, X, Y, v = self._contourdata(True)
        cp = ax.contourf(x, y, v, levels, **kwargs);
        if mark is not None:
            cp2 = ax.contour(x, y, Y, levels = mark, colors = mcolors, lw = 1.5, **kwargs)
        
        if xlims is not None:
            ax.set_xlim(*xlims);
        if ylims is not None:
            ax.set_ylim(*ylims);
        
        if addColorBar:
            cb = plt.colorbar(cp)
            cb.set_label('Hydrogen Mass Fraction', rotation = -90, va = 'bottom')
        ax.set_xlabel(xlab); ax.set_ylabel(ylab);
        if aspect is not None:
            ax.set_aspect(aspect)
        return plt.gcf()

