"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import copy
import warnings

import matplotlib.pyplot as plt
from matplotlib.ticker import MaxNLocator
import numpy as np
from scipy import integrate, optimize
import scipy.constants as const

from ._fuel_props import FuelProperties
from ._notional_nozzle import NotionalNozzle
from ..utilities.custom_warnings import PhysicsWarning

########################################################################
# TODO: untested for alternate fuels
########################################################################


class DevelopingFlow:
    def __init__(self, fluid, orifice, ambient, mdot=None,
                 theta0=0, x0=0, y0=0,
                 lam=1.16, betaA=0.28,
                 nn_conserve_momentum=True, nn_T='solve_energy', 
                 T_establish_min=-1,  
                 suppressWarnings=False, verbose=False):
        '''
        Engineering correlations to calculate the Gaussian profile boundary conditions for
        the flow through an orifice
        '''
        self.verbose = verbose
        S0 = 0 # S always starts at 0. x and y may start somewhere else.
        Y0 = 1 # pure fluid
        
        # Orifice flow
        self.orifice = orifice
        if self.verbose:
            print('solving for orifice flow... ', end='')
        self.fluid_orifice = self.orifice.flow(fluid, ambient.P, mdot, suppressWarnings) # plug node at orifice exit
        self.mass_flow_rate = self.orifice.mdot(self.fluid_orifice)
        if self.verbose:
            print('done')
        self.d0 = orifice.d
        self.orifice_node = PlugNode(orifice.d*np.sqrt(orifice.Cd), self.fluid_orifice.v, self.fluid_orifice.rho,# diameter scaled to account for Cd
                                     1, self.fluid_orifice.T, theta0, x0, y0, S0)

        # Underexpanded jet (if needed: gets fluid to atmospheric pressure)
        self.fluid_exp, self.orifice_exp = self._expand(orifice, ambient, nn_T, nn_conserve_momentum)
 
        # Initial entrainment and heating (if needed: warms fluid to good T for thermodynamics)
        self.expanded_plug_node = self._dev_plug(self.fluid_exp, self.orifice_exp, ambient, Y0, theta0, x0, y0, S0, 
                                                 T_establish_min, betaA)

        # Develop into established Gaussian profile from plug flow

        self.initial_node = self.expanded_plug_node.establish(ambient, self.fluid_exp, lam)
    
    def _expand(self, orifice, ambient, nn_T, nn_conserve_momentum):
        '''
        expands an underexpanded jet, if needed
        '''
        if self.fluid_orifice.P > ambient.P: # use notional nozzle model
            if self.verbose:
                print('solving for notional nozzle... ', end='')
            nn = NotionalNozzle(self.fluid_orifice, orifice, ambient)
            g, o = nn.calculate(nn_T, nn_conserve_momentum)
            if self.verbose:
                print('done.')
        else:
            g, o = self.fluid_orifice, orifice
        return g, o

    def _dev_plug(self, fluid, orifice, ambient, Y0, theta0, x0, y0, S0, 
                  T_establish_min, betaA):
        '''
        zone of initial entrainment and heating (if needed)
        conservation of energy and mass while air being entrained
        '''
        if fluid.T > T_establish_min:
            plug_node = PlugNode(orifice.d, fluid.v, fluid.rho, Y0, fluid.T, theta0, x0, y0, S0)
        else:
            if self.verbose:
                print('solving for zone of initial entrainment and heating... ', end='')
            air_out = copy.copy(ambient)
            air_out.update(T = T_establish_min, P = air_out.P) 
            fluid_out = copy.copy(fluid)
            fluid_out.update(T = T_establish_min, P = fluid_out.P)
            mdot_in = orifice.mdot(fluid)
            h_in = fluid.therm.h(T = fluid.T, D = fluid.rho) + fluid.v**2/2 # assumes mdot_in is pure
            h_air_in = ambient.therm.h(T = ambient.T, P = ambient.P)
            h_air_out = air_out.therm.h(T = air_out.T, D = air_out.rho)
            h_fluid_out = fluid_out.therm.h(T = fluid_out.T, D = fluid_out.rho)
            def errH(Y):
                h_out = (1-Y)*h_air_out + Y*h_fluid_out
                rho_out = 1/((1-Y)/air_out.rho + Y/fluid_out.rho)
                mdot_out = mdot_in + (1-Y)/Y*mdot_in # assumes mdot_in is pure
                v_out = fluid.v*mdot_in/mdot_out # need to think about whether this is right
                h_out = h_out + v_out**2/2
                return mdot_in*h_in + (mdot_out - mdot_in)*h_air_in - mdot_out*h_out
            Y = optimize.brentq(errH, 1.e-6, 1)
            rho_out = 1/((1-Y)/air_out.rho + Y/fluid_out.rho)
            mdot_out = mdot_in + (1-Y)/Y*mdot_in
            v_out = fluid.v*mdot_in/mdot_out
            E = betaA*np.sqrt(orifice.mdot(fluid)*fluid.v/ambient.rho) 
            S_out = (1-Y)*(mdot_out - mdot_in)/E/ambient.rho
            d_out = np.sqrt(mdot_out/(rho_out*v_out)*4/np.pi)
            plug_node = PlugNode(d_out, v_out, rho_out, Y, fluid_out.T, theta0, 
                                 x0 + S_out*np.cos(theta0), y0 + S_out*np.sin(theta0), S0 + S_out)
            if self.verbose:
                print('done.')
        return plug_node

class PlugNode:
    def __init__(self, d, v, rho, Y, T, theta, x = 0, y = 0, S = 0):
        '''Plug flow node
        
        Parameters
        ----------
        d: float, diameter (m)
        v: float, velocity (m/s)
        rho: float, density (kg/m^3)
        Y: float, mass fraction fuel
        theta: float, release angle from horizontal (rad)
        x: float, x position (m),
        y: float, y position (m),
        S: float, distance along streamline (m)
        '''
        self.d, self.v, self.rho, self.Y = d, v, rho, Y
        self.theta, self.x, self.y, self.S = theta, x, y, S
        self.T = T
    
    def Froude(self, rho_amb):
        '''Froude number'''
        return self.v/np.sqrt(const.g*self.d*abs(rho_amb - self.rho)/self.rho) #ESH - 7/23/20 - added abs to work with negatively buoyant jets
    
    def establish(self, ambient, fluid, lam):
        '''
        returns the gaussian node after flow establishment
        '''
        V_clE = self.v
        BE = self.d/np.sqrt(2*(2*lam**2+1)/(lam**2*self.rho/ambient.rho + lam**2 + 1))
        Y_clE = self.Y*(lam**2+1.)/(2.*lam**2)
        h_amb, cp_ambient = ambient.therm.PropsSI(['H', 'C'], T = ambient.T, P = ambient.P)

        cp_gas = fluid.therm.PropsSI('C', T = (self.T + ambient.T)/2., P = ambient.P)
        MW_clE = 1 / (Y_clE / fluid.therm.MW + (1 - Y_clE) / ambient.therm.MW)
        h_amb = ambient.T*cp_ambient
        cp_s3 = self.Y * cp_gas + (1 - self.Y) * cp_ambient # this is just cp_gas - using self.Y screws it up
        
        T_s3 = self.T
        # TODO: Use ThermoProps to calculate enthalpy
        h_s3 = cp_s3 * T_s3
        h_clE = h_amb + (lam**2+1)/(2*lam**2)*(h_s3 - h_amb)
        cp_clE = Y_clE * cp_gas + (1 - Y_clE) * cp_ambient
        T_clE = h_clE / cp_clE

        rho_clE = ambient.P*MW_clE/(const.R*T_clE)
        #######################################################################################
        # Note: correlation below is for Fr**2 > 40, which is pretty much always true for these 
        # gases - could add Froude number correlation if desired
        #######################################################################################
        SE = 6.2*self.d
        xE = self.x + SE*np.cos(self.theta)
        yE = self.y + SE*np.sin(self.theta)
        SE = self.S + SE
        thetaE = self.theta
        return GaussianNode(BE, V_clE, rho_clE, Y_clE, thetaE, xE, yE, SE)

class GaussianNode:
    def __init__(self, B, v_cl, rho_cl, Y_cl, theta, x = 0, y = 0, S = 0):
        '''
        Describes the properties of a node 
        
        Parameters
        ----------
        B: float
            Gaussian halfwidth (m)
        v_cl : float
            velocity at centerline (m/s)
        rho_cl : float
            density at centerline (kg/m^3)
        Y_cl : float
            mass fraction at centerline (kg/kg)
        theta: float
            angle of jet (rad, 0 is horizontal)
        x: float
            horizontal position of node (m)
        y: float
            vertical position of node (m)
        S : float (optional)
            length along jet(m)
        '''
        self.B, self.v_cl, self.rho_cl, self.Y_cl = B, v_cl, rho_cl, Y_cl
        self.S, self.theta, self.x, self.y = S, theta, x, y
    
    @property
    def conditions(self):
        return np.array([self.v_cl, self.B, self.rho_cl, self.Y_cl, self.theta, self.x, self.y])    

    def entrainment(self, Emom, rho_amb, alpha_buoy, alpha):
        FrL = self.v_cl**2*self.rho_cl/(const.g*self.B*abs(rho_amb-self.rho_cl)) # ESH: added absolute value 07/23/20 - for negatively buoyant jets, this was a negative number - might need justification
        E_buoy = alpha_buoy/FrL*(2*const.pi*self.v_cl*self.B)*np.sin(self.theta)  # m**2/s
        E = Emom + E_buoy
        alphatest = E/(2*const.pi*self.v_cl*self.B)
        if alphatest > alpha:
            E = alpha*2*const.pi*self.B*self.v_cl
        return E        


class Jet:
    def __init__(self, fluid, orifice, ambient, mdot=None,
                 theta0= 0, x0=0, y0=0,
                 lam=1.16, betaA=0.28,
                 nn_conserve_momentum=True, nn_T='solve_energy', 
                 T_establish_min=-1, 
                 Ymin=7e-4, dS=None, Smax=np.inf, 
                 max_steps=5000, tol=1e-8,
                 alpha=0.082, Yamb=0, numB=5, numpts=500, 
                 suppressWarnings=False, verbose=False):
        '''
        Class for solving for a 2D jet. 
        If fluid pressure is <= 2 x ambient pressure, use subsonic initialization (specify mdot).
               
        Parameters
        ----------
        fluid: fluid object
            the fluid (fluid) that is being released
        orifice: orifice object
            the release
        ambient: fluid object
            the fluid into which the release occurs
        mdot: float, optional defaults to None
            mass flow rate (kg/s). Only used if flow is not choked.
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
        nn_conserve_momentum: boolean, optional
            whether notional nozzle model should conserve mass and momentum, or mass only,
            together with nn_T determines which notional nozzle model to use (see below)
        nn_T: string, optional
            either 'solve_energy', 'Tthroat' or specified temperature (stagnation temperature) 
            with nn_conserve_momentum leads to one of the following notional nozzle models:
            YuceilOtugen - conserve_momentum = True, T = 'solve_energy'
            EwanMoodie - conserve_momentum = False, T = 'Tthroat'
            Birch - conserve_momentum = False, T = T0
            Birch2 - conserve_momentum = True, T = T0
            Molkov - conserve_momentum = False, T = 'solve_energy'
        T_establish_min: float, optional
            minimum temperature (K) at the zone of flow establishment, if specified, will implement the 
            zone of initial entrainment and heating if temperature after notional nozzle model is lower
        Ymin: float, optional
            minimum mass fraction to integrate to (default is about 1 mol%)
        dS: float, optional
            integrator step size, if None, defaults to 500 diameters, solver adds steps in high gradient areas
        Smax: float, optional
            maximum limit of integration, integrator will stop when it reaches Ymin or Smax
        max_steps: float, optional
            maximum steps for integrator
        tol: float, optional
            relative and absolute tolerance for integrator
        alpha: float, optional
            empirical buoyancy induced entrainment constant
        Yamb: float, optional
            mass fraction of fluid in the ambient air
        numB: float, optional
            maximum number of halfwidths (B) considered to be infinity - for integration in energy equations
        numpts: int, optional
            maximum number of points in energy integration (from 0 to numB)
        suppressWarnings: boolean, optional
            whether to display warnings about fluid being under-/over-specified in DevelopingFlow object
        verbose: boolean, optional
            whether to include print statements about the model actions
        There are up to 4 engineering models that give initial conditions to an 
        integral model:
        1) flow through the orifice - choked if pressure above critical pressure, assumed
           to be plug flow at volumetric flow rate if not
        2) if choked, underexpanded (notional nozzle)
        3) if choked and < 60K, initial entrainment and heating
        4) flow establishment

        Properties
        ----------
        S : ndarray of floats
            distance (m) along jet?
        '''
        self.verbose = verbose
               
        self.developing_flow = DevelopingFlow(fluid, orifice, ambient, mdot,
                                              theta0=theta0, x0=x0, y0=y0,
                                              lam=lam, betaA=betaA,
                                              nn_conserve_momentum=nn_conserve_momentum,nn_T=nn_T, 
                                              T_establish_min=T_establish_min,  
                                              suppressWarnings=suppressWarnings,
                                              verbose=verbose)
        self.initial_node = self.developing_flow.initial_node
        self.mass_flow_rate = self.developing_flow.mass_flow_rate
        
        # Set up some entrainment parameters:
        Fr = self.developing_flow.expanded_plug_node.Froude(ambient.rho)
        self.Fr = Fr
        if Fr < 268:
            self._alpha_buoy = 17.313-0.11665*Fr+(2.0771e-4)*Fr**2 
        else:
            self._alpha_buoy = 0.97
        expanded_plug_node = self.developing_flow.expanded_plug_node
        self._Emom = betaA*np.sqrt(const.pi/4*expanded_plug_node.d**2*
                                   expanded_plug_node.rho*expanded_plug_node.v**2/ambient.rho)
        # some other objects and parameters that I don't want to keep recalculating
        # note: method of calculating heat capacity and enthalpy makes a significant difference in terms
        # of trajectory and density in the calculations...
        self.lam, self.fluid, self.ambient = lam, fluid, ambient
        #####################################################################################################
        # TODO: account for variations in heat capacity as a function of temperature - used when getting rid of
        #       heat capacity in _gov_eqns
        MW_air, MW_fluid = ambient.therm.MW, fluid.therm.MW
        MW_cl0 = MW_air*MW_fluid/(self.initial_node.Y_cl*(MW_air - MW_fluid) + MW_fluid)
        T_cl0 = ambient.P*MW_cl0/(const.R*self.initial_node.rho_cl)
        # TODO: determine if _Cp_fluid should be at ambient T, or T_cl0
        self._Cp_fluid = self.fluid.therm.PropsSI('C', T = ambient.T, P = ambient.P)
        self._Cp_air, self._h_amb0 = ambient.therm.PropsSI(['C', 'H'], T = ambient.T, P = ambient.P)

        
        # Integrate in the zone of established flow
        self.solve(Ymin, dS, Smax, max_steps, tol, alpha, Yamb, numB, numpts)
    
    def solve(self, Ymin = 7e-4, dS = None, Smax = np.inf, 
              max_steps = 5000, tol = 1e-8,
              alpha = 0.082, Yamb = 0, numB = 5, numpts = 500):
        '''
        solves (integrates) the model equations from the initial node out to limit
        '''
        if self.verbose:
            print('integrating... ', end='')

        if dS is None and Smax == np.inf:
            dS = 500*self.developing_flow.expanded_plug_node.d # somewhat arbitrary - solver will add points anyway 
                                        # may integrate past Ymin 
        elif dS is None:
            dS = Smax

        r = integrate.ode(self._govEqns).set_f_params(alpha, Yamb, numB, numpts)
        r.set_integrator('dopri5', atol = tol, rtol = tol)
        
        T, Y = [], []
        def solout(t, y):
            T.append(t)
            Y.append(np.array(y))
        r.set_solout(solout)
        r.set_initial_value(self.initial_node.conditions, self.initial_node.S)
        
        i = 0
        while r.successful() and r.y[3] > Ymin and i < max_steps and r.t < Smax:
            r.integrate(r.t + dS)
            i += 1
            
        Y = np.array(Y)
        
        for key, val in zip(['V_cl', 'B', 'rho_cl', 'Y_cl', 'theta', 'x', 'y'], Y.T):
            self.__dict__[key] = val
        self.__dict__['S'] = np.array(T)

        MW_fluid, MW_air = self.fluid.therm.MW, self.ambient.therm.MW
        MW_cl  = MW_air*MW_fluid/(self.Y_cl*(MW_air-MW_fluid) + MW_fluid)
        self.X_cl = self.Y_cl*MW_cl/MW_fluid
        self.T_cl = self.ambient.P*MW_cl/(const.R*self.rho_cl)
        
        if self.verbose:
            print('done.')

        return self
    
    def _govEqns(self, S, ind_vars, alpha = 0.082, Yamb = 0, numB = 5, numpts = 500):
        '''
        Governing equations for a plume, written in terms of d/dS of (V_cl, B, rho_cl, Y_cl, 
        theta, x, and y).
        
        A matrix solution to the continuity, x-momentum, y-momentum, species, and energy 
        equations solves for d/dS of the dependent variables V_cl, B, rho_cl, Y_cl,  and Theta.  
        Numerically integrated to infinity = numB * B(S) using numpts discrete points.
        '''
        # break independent variables out of ind_vars, then put them into node_in
        [V_cl, B, rho_cl, Y_cl, theta, x, y] = ind_vars
        node_in = GaussianNode(B, V_cl, rho_cl, Y_cl, theta, S = S)
        
        # pull some parameters out of objects so their definition isn't so long
        rho_amb, MW_air, MW_fluid = self.ambient.rho, self.ambient.therm.MW, self.fluid.therm.MW
        lam = self.lam
        Pamb = self.ambient.P
        h_amb0, Cp_fluid, Cp_air = self._h_amb0, self._Cp_fluid, self._Cp_air
        h_amb0 = Cp_air * self.ambient.T
        E = node_in.entrainment(self._Emom, rho_amb, self._alpha_buoy, alpha = alpha)
        
        # some stuff needed to integrate to infinity (numB*B):
        r = np.append(np.array([0]), np.logspace(-5, np.log10(numB*max(B, 1e-99)), numpts))
        zero = np.zeros_like(r)
        V       = V_cl*np.exp(-(r**2)/(B**2))
        dVdS = np.array([V/V_cl,                                                 #d/dS(V_cl)
                         2*V*r**2/B**3,                                          #d/dS(B)
                         zero,                                                   #d/dS(rho_cl) 
                         zero,                                                   #d/dS(Y_cl)
                         zero])                                                  #d/dS(theta)
        rho     = (rho_cl - rho_amb)*np.exp(-(r**2)/((lam*B)**2))+rho_amb
        Y       = Y_cl*rho_cl/rho*np.exp(-r**2/(lam*B)**2)
        dYdS = np.array([zero,                                                      #d/dS(V_cl)
                         (2*Y**2*rho_amb*r**2*np.exp(r**2/(lam*B)**2)/
                         (lam**2*B**3*Y_cl*rho_cl)),                                #d/dS(B)
                         Y**2*rho_amb*(np.exp(r**2/(lam*B)**2)-1)/(Y_cl*rho_cl**2), #d/dS(rho_cl)
                         Y/Y_cl,                                                    #d/dS(Y_cl)
                         zero])                                                     #d/dS(theta)
        MW      = MW_air*MW_fluid/(Y*(MW_air - MW_fluid) + MW_fluid)
        dMWdS   = (MW*(MW_air - MW_fluid)/(MW_fluid*(Y-1) - MW_air*Y))*dYdS
        Cp      = Y*(Cp_fluid - Cp_air) + Cp_air 
        dCpdS   = (Cp_fluid - Cp_air)*dYdS
        rhoh    = Pamb/const.R*MW*Cp
        drhohdS = Pamb/const.R*(MW*dCpdS + Cp*dMWdS)
        ##########################################################
        # TODO: integrating the energy equation without involving Cp - not sure what the isssue is in the code below
        # drhodS  = np.array([zero,                                                #d/dS(V_cl)
                            # -2*r**2*(rho_amb - rho_cl)*np.exp(r**2/(lam*B)**2),  #d/dS(B)
                            # lam**2*B**3*np.exp(r**2/(lam*B)**2),                 #d/dS(rho_cl)
                            # zero,                                                #d/dS(Y_cl)
                            # zero                                                 #d/dS(theta)
                            # ])*1./(lam**2*B**3)
        # # TODO: remove ideal gas assumption here (low priority)
        # T = Pamb*MW/(const.R*rho)
        # dTdS = Pamb/(const.R*rho)*dMWdS - Pamb*MW/(const.R*rho**2)*drhodS
        # h_amb = self._h_amb(T) #self.ambient.therm.h(T = T, P = Pamb)
        # d_h_amb_dT = self._dh_amb_dT(T)
        # h_fluid = self._h_fluid(T)
        # d_h_fluid_dT = self._dh_fluid_dT(T)
        # h = Y*h_fluid - Y*h_amb + h_amb
        # dhdS = (h_fluid - h_amb)*dYdS + Y*(d_h_fluid_dT - d_h_amb_dT)*dTdS + d_h_amb_dT*dTdS
        
        # rhoh = rho*h
        # drhohdS = h*drhodS + rho*dhdS
        # #########################################################
        
        
        # governing equations:
        LHScont = np.array([(lam**2*rho_cl + rho_amb)*B**2,                        #d/dS(V_cl)
                            2*(lam**2*rho_cl + rho_amb)*B*V_cl,                    #d/dS(B)
                            lam**2*B**2*V_cl,                                      #d/dS(rho_cl)
                            0,                                                    #d/dS(Y_cl)
                            0])*const.pi/(lam**2 + 1)                             #d/dS(theta)
        RHScont = rho_amb*E
        
        LHSxmom = np.array([(2*lam**2*rho_cl+rho_amb)*B**2*V_cl*np.cos(theta),     #d/dS(V_cl)
                            (2*lam**2*rho_cl+rho_amb)*B*V_cl**2*np.cos(theta),     #d/dS(B)
                            lam**2*B**2*V_cl**2*np.cos(theta),                     #d/dS(rho_cl)
                            0,                                                    #d/dS(Y_cl)
                            -(2*lam**2*rho_cl+rho_amb)*(B*V_cl)**2*np.sin(theta)/2 #d/dS(theta)
                            ])*const.pi/(2*lam**2+1)        
        RHSxmom = 0
        
        LHSymom = np.array([(2*lam**2*rho_cl+rho_amb)*B**2*V_cl*np.sin(theta),     #d/dS(V_cl)
                            (2*lam**2*rho_cl+rho_amb)*B*V_cl**2*np.sin(theta),     #d/dS(B)
                            lam**2*B**2*V_cl**2*np.sin(theta),                     #d/dS(rho_cl)
                            0,                                                    #d/dS(Y_cl)
                            (2*lam**2*rho_cl+rho_amb)*(B*V_cl)**2*np.cos(theta)/2  #d/dS(theta)
                            ])*const.pi/(2*lam**2+1)                                 
        RHSymom = -const.pi*lam**2*const.g*(rho_cl - rho_amb)*B**2
        
        LHSspec = np.array([B*Y_cl*rho_cl,                                         #d/dS(V_cl)
                            2*V_cl*Y_cl*rho_cl,                                    #d/dS(B)
                            B*V_cl*Y_cl,                                           #d/dS(rho_cl)
                            B*V_cl*rho_cl,                                         #d/dS(Y_cl)
                            0,                                                    #d/dS(theta)
                            ])*const.pi*lam**2*B/(lam**2 + 1)                
        RHSspec = Yamb*RHScont
        
        LHSener = 2*const.pi*integrate.trapz(V*drhohdS*r + rhoh*dVdS*r, r)
        LHSener += [const.pi/(6*lam**2 + 2)*(3*lam**2*rho_cl+rho_amb)*B**2*V_cl**2, #d/dS(V_cl)
                    const.pi/(9*lam**2 + 3)*(3*lam**2*rho_cl+rho_amb)*V_cl**3*B,    #d/dS(B)
                    const.pi/(6*lam**2 + 2)*lam**2*B**2*V_cl**3,                    #d/dS(rho_cl)
                    0,                                                             #d/dS(Y_cl)
                    0]                                                             #d/dS(theta)
        
        RHSener = h_amb0*RHScont
        
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
        reshapes the plume to turn upwards, should it hit the enclosure wall, 
        and crops it so it stops at the ceiling
        '''
        if np.any(self.x > enclosure.Xwall):
            iwall = np.argmax(self.x > enclosure.Xwall)
            for k in ['S', 'rho_cl', 'V_cl', 'Y_cl', 'B', 'theta', 'y', 'x']:
                self.__dict__[k] = np.append(np.append(self.__dict__[k][:iwall],
                                                       np.interp(enclosure.Xwall, self.x, self.__dict__[k])),
                                             self.__dict__[k][iwall:])
            self.y[iwall+1:] = self.y[iwall] + self.S[iwall+1:] - self.S[iwall]
            self.x[iwall:] = enclosure.Xwall
            self.theta[iwall:] = np.pi/2
        if np.any(self.y > enclosure.H):
            iceil = np.argmax(self.y > enclosure.H)
            for k in ['S', 'rho_cl', 'V_cl', 'Y_cl', 'B', 'theta', 'x', 'y']:
                np.append(self.__dict__[k][:iceil],
                          np.interp(enclosure.H, self.y, self.__dict__[k]))
        if showPlot == True:
            plt.plot(self.x,self.y)
        return self

    def m_flammable(self, X_lean=None, X_rich=None, Hmax=np.inf):
        '''
        Calculates the amount of mass in the plume
        that is within the flammability limits
        
        Parameters
        ----------
        plume : Plume class
            Class of plume results
        X_lean : float, optional
            Mole fraction lower flammability limit
            Default is None: will use default LFL for fuel
        X_rich : float, optional
            Mole fraction upper flammability limit
            Default is None: will use default UFL for fuel
        Hmax : float, optional
            Maximum height for integration
            Default is infinity (np.inf)
        
        Outputs
        -------
        mass : float
            Flammable mass in plume, up to height H (kg)
        '''
        MW_fluid = self.fluid.therm.MW
        MW_air = self.ambient.therm.MW
        if X_lean is None:
            fuel_props = FuelProperties(self.fluid.species)
            X_lean = fuel_props.LFL
        if X_rich is None:
            fuel_props = FuelProperties(self.fluid.species)
            X_rich = fuel_props.UFL
        Ylean = X_lean * MW_fluid / (X_lean * MW_fluid + (1 - X_lean) * MW_air)
        Yrich = X_rich * MW_fluid / (X_rich * MW_fluid + (1 - X_rich) * MW_air)

        # Trim the plume down to below Hmax:
        S = np.copy(self.S)
        Y_cl = np.copy(self.Y_cl)
        B = np.copy(self.B)
        rho_cl = np.copy(self.rho_cl)
        if np.all(self.y > Hmax) or np.all(self.Y_cl < Ylean) or np.all(self.Y_cl > Yrich): # no flammable mass 
            return 0
        elif np.any(self.y > Hmax): # trim arrays to Hmax
            S = np.append(S[np.argwhere(self.y < Hmax)].T[0], np.interp(Hmax, self.y, S))
            Y_cl = np.append(Y_cl[np.argwhere(self.y < Hmax)].T[0], np.interp(Hmax, self.y, Y_cl))
            B = np.append(B[np.argwhere(self.y < Hmax)].T[0], np.interp(Hmax, self.y, B))
            rho_cl = np.append(rho_cl[np.argwhere(self.y < Hmax)].T[0], np.interp(Hmax, self.y, rho_cl))
                
        def rhoY(r, i):
            'mass fraction and density at radius: r, for plume at node: i'
            rho = (rho_cl[i]-self.ambient.rho)*np.exp(-r**2/(self.lam*B[i]**2))+self.ambient.rho
            Y = rho_cl[i]*Y_cl[i]*np.exp(-r**2/(self.lam*B[i])**2)/rho
            return rho, Y

        Slean = np.interp(Ylean, Y_cl[::-1], S[::-1])
        Srich = np.interp(Yrich, Y_cl[::-1], S[::-1])
        ivals = slice(np.argmax(Y_cl <= Yrich), np.argmax(Y_cl <= Ylean))

        Y_cl, B, rho_cl, S = [np.append(np.append(np.interp(Srich, S, var), var[ivals]), np.interp(Slean, S, var)) for var in [Y_cl, B, rho_cl, S]]
        # radius of flammable concentration at each node:
        r_lean = np.array([0 if rhoY(0, i)[1] <= Ylean else 
                           optimize.brentq(lambda r: rhoY(r, i)[1] - Ylean, 0, 100*B[i])
                           for i in range(len(S))])
        r_rich = np.array([0 if rhoY(0, i)[1] <= Yrich else 
                           optimize.brentq(lambda r: rhoY(r, i)[1] - Yrich, 0, 100*B[i])
                           for i in range(len(S))])

        # integrate to find the mass/length at each node
        mass_per_len = np.array([integrate.quad(lambda r: np.prod(rhoY(r, i))*2*const.pi*r, 
                                 r_r, r_l)[0] for i, r_r, r_l in zip(range(len(S)), r_rich, r_lean)])
        # integrate to find the total mass
        return integrate.trapz(mass_per_len, S)
    
    @property
    def _contourdata(self):
        """

        Returns
        -------
        x : ndarray
            horizontal positions along jet (m)
        y : ndarray
            vertical positions along jet (m)
        X : ndarray
            mole fractions
        Y : ndarray
            mass fractions
        v : ndarray
            velocities
        T : ndarray
            temperatures
        """
        iS = np.arange(len(self.S))
        
        # Calculates logspaced points around 0 out to np.log10(3*np.max(self.B))
        # poshalf[::-1] just notation for reversing a numpy array
        poshalf = np.logspace(-5, np.log10(3*np.max(self.B)))
        r = np.concatenate((-1 * poshalf[::-1], [0], poshalf))
        
        r, iS = np.meshgrid(r, iS)
        B = self.B[iS]
        rho_cl = self.rho_cl[iS]
        Y_cl = self.Y_cl[iS]
        V_cl = self.V_cl[iS]                    
        
        rho_amb, Tamb, Pamb = self.ambient.rho, self.ambient.T, self.ambient.P
        MW_fluid, MW_air = self.fluid.therm.MW, self.ambient.therm.MW
        
        rho = rho_amb + (rho_cl - rho_amb)*np.exp(-r**2/self.lam**2/B**2)
        Y   = Y_cl*rho_cl*np.exp(-(r**2)/((self.lam*B)**2))/rho
        MW  = MW_air*MW_fluid/(Y*(MW_air-MW_fluid) + MW_fluid)
        
        X = Y*MW/MW_fluid
        v = V_cl*np.exp(-(r**2)/(B**2))
        T = Pamb*MW/(const.R*rho)
        
        x = self.x[iS] + r*np.sin(self.theta[iS])
        y = self.y[iS] - r*np.cos(self.theta[iS])
        return x, y, X, Y, v, T
    
    def _radial_profile(self, distance, ind_var = 'Y', nB = 3):
        '''
        returns radial profile at a certain distance along the jet
        Parameters
        ----------
        distance: float
            distance along jet to plot profile
        ind_var: string
            independent variable to plot, either mass fraction ('Y'), mole fraction ('X') or velocity ('v')
        nB: int
            number of halfwidths to plot

        Returns
        -------
        [r, ind_var]: radial profile of independent variable from -nB*B to nB*B
        '''
        B = np.interp(distance, self.S, self.B)
        rho_cl = np.interp(distance, self.S, self.rho_cl)
        Y_cl = np.interp(distance, self.S, self.Y_cl)
        V_cl = np.interp(distance, self.S, self.V_cl)

        r = np.logspace(-5, np.log10(nB*B))
        r = np.concatenate((-1*r[::-1], [0], r))

        rho_amb, Tamb, Pamb = self.ambient.rho, self.ambient.T, self.ambient.P
        MW_fluid, MW_air = self.fluid.therm.MW, self.ambient.therm.MW

        rho = rho_amb + (rho_cl - rho_amb)*np.exp(-r**2/self.lam**2/B**2)
        Y   = Y_cl*rho_cl*np.exp(-(r**2)/((self.lam*B)**2))/rho
        MW  = MW_air*MW_fluid/(Y*(MW_air-MW_fluid) + MW_fluid)

        X = Y*MW/MW_fluid
        v = V_cl*np.exp(-(r**2)/(B**2))
        T = Pamb*MW/(const.R*rho)
        if ind_var == 'Y':
            return [r, Y]
        if ind_var == 'X':
            return [r, X]
        if ind_var == 'v':
            return [r, v]
        if ind_var == 'T':
            return [r, T]

    def get_contour_data(self):
        return self._contourdata

    def plot_moleFrac_Contour(self, mark=None, mcolors = 'w', xlims = None,
                              ylims = None, xlab = 'x (m)', ylab = 'y (m)', 
                              plot_title = None, vmin = 0, vmax = 0.1, 
                              addColorBar = True, aspect = 1, fig_params=None,
                              subplots_params=None, ax = None, fig = None, contlvls = False):
        '''
        makes mole fraction contour plot
        
        Parameters
        ----------
        mark: list or None, optional
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
        fig_params: dict or None, optional
            dictionary of figure parameters (e.g. figsize)
        subplots_params: dict or None, optional
            dictionary of subplots_adjust parameters (e.g. top)
        ax: optional
            axes on which to make the plot
        '''
        # Initialize missing params
        if subplots_params is None:
            subplots_params = {}
        if fig_params is None:
            fig_params = {}
        # Make figure and axis if not specified
        if ax is None and fig is None:
            fig, ax = plt.subplots(**fig_params)
            plt.subplots_adjust(**subplots_params)
        elif ax is None and fig is not None:
            ax = fig.subplots(**fig_params)
            plt.subplots_adjust(**subplots_params)
        
        # Get background color for contour
        ax.set_facecolor(plt.cm.get_cmap()(0)) #old matplotlib: ax.set_axis_bgcolor
        
        # Get contour data to plot
        x, y, X, __, __, __ = self._contourdata
        
        # Plot contour data
        if np.amax(X) > vmax and np.amin(X) < vmin:
            ExtStr = 'both'
        elif np.amax(X) > vmax and np.amin(X) >= vmin:
            ExtStr = 'max'
        elif np.amax(X) <= vmax and np.amin(X) < vmin:
            ExtStr = 'min'
        else:
            ExtStr = 'neither'
        if contlvls:
            contourlevels = np.linspace(vmin, vmax, 11)
            cp = ax.contourf(x, y, X, contourlevels, extend = ExtStr,alpha=0.5)
        else:
            contourlevels = np.linspace(vmin, vmax, 101)
            cp = ax.contourf(x, y, X, contourlevels, extend = ExtStr)

        # Change axis limits if specified
        if xlims is not None:
            ax.set_xlim(*xlims)
        if ylims is not None:
            ax.set_ylim(*ylims)

        # Set aspect ratio if specified
        if aspect is not None:
            ax.set_aspect(aspect)

        # Add specific contours if desired
        if mark is not None:
            mark = [FuelProperties(self.fluid.species).LFL if value == 'LFL' else value for value in mark]
            mark = [FuelProperties(self.fluid.species).UFL if value == 'UFL' else value for value in mark]
            CS = ax.contour(x, y, X, levels = np.sort(mark), colors = mcolors, linewidths = 1.5)
            fig.canvas.draw_idle()
            if xlims is not None and ylims is not None:
                manual = [self.streamline_distance_to_mole_fraction(m)*np.array([np.cos(self.theta[0]), np.sin(self.theta[0])])*.7
                          for m in mark]
            else:
                manual = False
            ax.clabel(CS, levels = CS.levels, inline = True, manual = manual, inline_spacing = 2)

        # Add colorbar if desired
        if addColorBar:
            axsize = ax.get_window_extent()
            if axsize.width <= axsize.height:
                cb_kwargs = {}
                cb_label_kwargs = {'rotation':-90, 'va':'bottom'}
            else:
                cb_kwargs = {'orientation':'horizontal'}
                cb_label_kwargs = {}
            cb = plt.colorbar(cp, ticks = MaxNLocator().tick_values(vmin, vmax), **cb_kwargs)
            cb.set_label('Mole Fraction', **cb_label_kwargs)
        
        # Set axis labels
        ax.set_xlabel(xlab)
        ax.set_ylabel(ylab)

        # Set plot title if specified
        if plot_title is not None:
            ax.set_title(plot_title)
        
        return fig
    
    def plot_massFrac_Contour(self, mark = None, mcolors = 'w', 
                              xlims = None, ylims = None,
                              xlab = 'x (m)', ylab = 'y (m)',
                              vmin = 0, vmax = 1, levels = 100,
                              addColorBar = True, aspect = 1, 
                              fig_params=None, subplots_params=None, ax = None):
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
        fig_params: dict or None, optional
            dictionary of figure parameters (e.g. figsize)
        subplots_params: dict or None, optional
            dictionary of subplots_adjust parameters (e.g. top)
        ax: optional
            axes on which to make the plot
        '''
        if subplots_params is None:
            subplots_params = {}
        if fig_params is None:
            fig_params = {}
        if ax is None:
            fig, ax = plt.subplots(**fig_params)
            plt.subplots_adjust(**subplots_params)

        ax.set_facecolor(plt.cm.get_cmap()(0))
        x, y, __, Y, __, __ = self._contourdata
        cp = ax.contourf(x, y, Y, levels, vmin = vmin, vmax = vmax)
        if mark is not None:
            ax.contour(x, y, Y, levels = mark, colors = mcolors, linewidths = 1.5)
        
        if xlims is not None:
            ax.set_xlim(*xlims)
        if ylims is not None:
            ax.set_ylim(*ylims)
        
        if addColorBar:
            cb = plt.colorbar(cp)
            cb.set_label('Mass Fraction', rotation = -90, va = 'bottom')
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
                              fig_params=None, subplots_params=None, ax=None, **kwargs):
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
        fig_params: dict or None, optional
            dictionary of figure parameters (e.g. figsize)
        subplots_params: dict or None, optional
            dictionary of subplots_adjust parameters (e.g. top)
        ax: optional
            axes on which to make the plot
        '''
        if subplots_params is None:
            subplots_params = {}
        if fig_params is None:
            fig_params = {}
        if ax is None:
            fig, ax = plt.subplots(**fig_params)
            plt.subplots_adjust(**subplots_params)
        ax.set_facecolor(plt.cm.get_cmap()(0))
        x, y, __, __, v, __ = self._contourdata
        cp = ax.contourf(x, y, v, levels, **kwargs)
        if mark is not None:
            cp2 = ax.contour(x, y, v, levels = mark, colors = mcolors, lw = 1.5, **kwargs)
        
        if xlims is not None:
            ax.set_xlim(*xlims)
        if ylims is not None:
            ax.set_ylim(*ylims)
        
        if addColorBar:
            cb = plt.colorbar(cp)
            cb.set_label('Velocity', rotation = -90, va = 'bottom')
        ax.set_xlabel(xlab)
        ax.set_ylabel(ylab)
        if aspect is not None:
            ax.set_aspect(aspect)
        return plt.gcf()
    
    def streamline_distance_to_mole_fraction(self, X = 0.08):
        '''
        returns the streamline distance to a given mole fraction
        
        Parameters
        ----------
        X: float
          mole fraction

        Returns
        -------
        streamline distance to X_cl = X
        '''
        return np.interp(X, self.X_cl[::-1], self.S[::-1])
    
