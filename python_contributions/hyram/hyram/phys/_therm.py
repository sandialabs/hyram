# coding: utf-8

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

import abc
import sys
import warnings

import numpy as np
from scipy import optimize, interpolate
import scipy.constants as const
import scipy.constants as const
import dill as pickle


# Abstract class for EOS, used to avoid copying mutual code everywhere
class EOS:
    '''
    Abstract class for equations of state EOS.  Since it is abstract,
    it cannot be instantiated.  It is used to avoid copying mutual
    code to determine 3rd variable everywhere.  Requires three
    functions to be defined.
    '''
    __metaclass__ = abc.ABCMeta


    def set_third(self, T, P, rho):
        '''
        Takes temperature, pressure, and density, two of which should
        not be None.  Determines 3rd from 2 specified or warns about 
        not properly specifying system if not enough supplied.  If all
        3 are specified, it will recompute one of them from the others.

        Parameters
        ----------
        T: float, temperature
        P: float, pressure
        rho: float, density

        Returns
        -------
        tuple (T, P, rho)
        '''
        if T is not None and P is not None:
            rho = self.rho(T, P)
        elif T is not None and rho is not None:
            P = self.P(T, rho)
        elif P is not None and rho is not None:
            T = self.T(P, rho)
        else:
            warnings.warn('system not properly defined')
        
        return T, P, rho

    @abc.abstractmethod
    def rho(self, T, P):
        pass

    @abc.abstractmethod
    def P(self, T, rho):
        pass

    @abc.abstractmethod
    def T(self, P, rho):
        pass




class AbelNoble(EOS):
    def __init__(self, b = 7.6921e-3, gamma = 1.4, MW = 2.016):
        '''
        Class used to solve the Abel-Nobel equation of state for different thermodynamic parameters.
        See Schefer et al. International Journal of Hydrogen Energy 32 (2007) 2081-2093.
        Note: for ideal gas, can be used with b = 0.
        
        Parameters
        ----------
        b: float, optional
            co-volume constant (default is 7.6921e-3 m^3/kg, valid for H2)
        gamma: float, optional
            specific heat ratio (default is 1.4, valid for H2)
        MW: float, optional
            molecular weight of gas (default is 2.016 g/mol, valid for H2)
        '''
        self.gamma = gamma
        self.b = b
        self.MW = MW
        
    def choke(self, T = None, P = None, rho = None):
        '''
        returns the thermodynamic state (T, P, rho) and velocity of a choked flow, for given stagnation conditions
        
        Parameters
        ----------
        T - temperature (K)
        P - Pressure (Pa)
        rho - density (kg/m^3)
        user must specify 2 of three thermodynamic parameters of the stagnant gas
        
        Returns
        -------
        dictionary of T, P, rho, v at the throat (orifice)
        '''
        T, P, rho = self.set_third(T, P, rho)
        rho_throat = self.rho_Iflow(rho)
        T_throat = self.T_IE(T, rho, rho_throat)
        v_throat = self.a(T_throat, rho_throat)
        P_throat = self.P(T_throat, rho_throat)
        class Result:
            pass
        throat = Result()
        throat.__dict__ = {'T': T_throat, 'P': P_throat, 'rho': rho_throat, 'v': v_throat}
        return throat
    
    def h(self, T, P):
        '''
        enthalpy (J/kg) of a gas at temperature T (K) and pressure P (Pa)
        
        Parameters
        ----------
        T: float
            tempearture (K)
        P: float
            pressure (Pa)
        
        Returns
        -------
        h: float
            heat capacity (J/kg)
        '''
        cp = (self.gamma*const.R*1000./self.MW)/(self.gamma-1)
        return cp*T
    
    def u(self, T = None, P = None, rho = None):
        T, P, rho = self.set_third(T, P, rho)
        return self.h(T, P) - P/rho
    
    def errS(self, T0, rho0, T1, rho1):
        '''
        returns the difference in entropy (J/kg) between 2 states specified by the 
        temperatures and densities.
        
        Parameters
        ----------
        T0: float
            temperature of gas at point 0 (K)
        rho0: float
            density of gas at point 0 (kg/m^3)
        T1: float
            temperature of gas at point 1 (K)
        rho1: float
            density of gas at point 1 (kg/m^3)
            
        Returns
        -------
        errS: float
            error in enthalpy between the two different states (J/kg)
        '''
        b, g = self.b, self.gamma
        return (T1*((1-b*rho1)/rho1)**(g-1)) / (T0*((1-b*rho0)/rho0)**(g-1)) - 1    
    
    def T_IflowV(self, T0, V0, P0, V, P):
        '''
       temperature of gas after isentropic flow expansion
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        V0: float
            velocity before expansion (m/s)
        P0: float
            pressure before expansion (Pa)
        V: float
            velocity after expansion (m/s)
        P: float
            pressure after expansion (Pa)
        
        Returns
        -------
        T: float
            temperature after expansion(K)
        '''
        g, b = self.gamma, self.b
        cp = (g*const.R*1000./self.MW)/(g-1)
        return T0 + b*(P0 - P)/cp + 1./2*(V0**2 - V**2)/cp
        
    def rho_Iflow(self, rho0, Ma = 1, Ma0 = 0):
        '''
        density of gas after isentropic flow expansion
        
        Parameters
        ----------
        rho0: float
            density before exansion (kg/m^3)
        Ma: float, optional
            mach number after epansion (default assumed to be 1, choked)
        Ma0: float, optional
            Mach number before expansion (default assumed to be 0, stagnant)
        
        Returns
        -------
        rho: float
            density after expansion (kg/m^3)
        '''
        g = self.gamma
        b = self.b
        fun = lambda rho: rho0/(1-b*rho0) - rho/(1-b*rho)*((1+((g-1)/(2*(1-b*rho)**2)*Ma**2))/
                                                           (1+((g-1)/(2*(1-b*rho0)**2)*Ma0**2)))**(1/(g-1))
        return optimize.fsolve(fun, rho0)[0]
    
    def T_Iflow(self, T0, rho, Ma = 1):
        '''
        temperature of gas after isentropic flow expansion
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        rho: float
            density after expansion (kg/m^3)
        Ma: float, optional
            mach number after expansion (default assumed to be 1, choked)
        
        Returns
        -------
        T: float
            temperature after expansion (K)
        '''
        g, b = self.gamma, self.b
        return T0/(1+((g-1)/(2*(1-b*rho)**2))*Ma**2)
    
    def P_Iflow(self, P0, rho, Ma = 1):
        '''
        pressure of gas after isentropic flow expansion
        
        Parameters
        ----------
        P0: float
            pressure before expansion (Pa)
        rho: float
            density after expansion (kg/m^3)
        Ma: float, optional
            mach number after expansion (default assumed to be 1, choked)
        
        Returns
        -------
        P: float
            pressure after expansion (Pa)
        '''
        g, b = self.gamma, self.b
        return P0/(1+((g-1)/(2*(1-b*rho)**2))*Ma**2)**(g/(g-1))
    
    def T_IE(self, T0, rho0, rho):
        '''
        temperature of gas after isentropic expansion, given an old and new density
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        rho0: float
            density before expansion (kg/m^3)
        rho: float
            density after expansion (kg/m^3)
        
        Returns
        -------
        T: 
            temperature after expansion (K)
        '''
        g, b = self.gamma, self.b
        return T0*((1-b*rho0)*rho/((1-b*rho)*rho0))**(g-1)
    
    def P(self, T, rho):
        '''
        returns the pressure given the temperature and density
        
        Parameters
        ----------
        T: float
            temperature (K)
        rho: float
            density (kg/m^3)
        
        Returns
        -------
        P: float
            pressure (Pa)
        '''
        return const.R*1000/self.MW*rho*T/(1-self.b*rho)
    
    def T(self, P, rho):
        '''
        returns the temperature given the pressure and density
        
        Parameters
        ----------
        P: float
            pressure (Pa)
        rho: float
            density (kg/m^3)
        
        Returns
        -------
        T: float
            temperature (K)
        '''
        
        return P*(1-self.b*rho)/(rho*const.R*1000/self.MW)
    
    def rho(self, T, P):
        '''
        returns the density given the temperature and pressure
        
        Parameters
        ----------
        T: float
            temperature (K)
        P: flaot
            pressure (Pa)
        
        Returns
        -------
        rho:
            density (kg/m^3)
        '''
        return P/(const.R*1000/self.MW*T+P*self.b)
        
    def a(self, T, rho):
        '''
        returns the speed of sound given the temperature and density
        
        Parameters
        ----------
        T: float
            temperature (K)
        rho: float
            density (kg/m^3)
        
        Returns
        -------
        a: float
            speed of sound (m/s)
        '''
        return 1/(1-self.b*rho)*np.sqrt(self.gamma*const.R*1000/self.MW*T)
   
    def T_IflowSonic(self, T0, P0, P):
        '''
        solves for the temperature of gas after an adiabatic flow, assuming that the entrance
        and exit velocities are sonic
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        P0: float
            pressure before expansion (Pa)
        P: float
            pressure after expansion (Pa)
        
        Returns
        -------
        T: float
            temperature after expansion(K)
        '''
        v0 = self.a(T0, self.rho(T0, P0))
        cp = const.R*1000./self.MW*(self.gamma/(self.gamma - 1))
        def err(T):
            v = self.a(T, self.rho(T, P))
            return cp*T0 + v0**2/2. - cp*T - v**2/2.
        T = optimize.newton(err, T0)
        return T
    
    def critical_ratio(self):
        '''
        calculates the critical ratio for choked flow
        
        Returns
        -------
        CR: float
            critical ratio of upstream to downstream pressures (dimensionless)
        '''
        gamma = self.gamma
        CR = (2 / (gamma + 1)) ** (-gamma / (gamma - 1))
        return CR



class IdealGas(AbelNoble):
    def __init__(self, gamma = 1.4, MW = 2.016):
        '''
        Class used to solve the Ideal-Gas equation of state for
            different thermodynamic parameters.
        Note: Mostly just AbelNoble with b = 0, but one function is
            optimized for ideal gas (non-ideal solves nonlinear eq.)
        
        Parameters
        ----------
        gamma: float, optional
            specific heat ratio (default is 1.4, valid for H2)
        MW: float, optional
            molecular weight of gas (default is 2.016 g/mol, valid for H2)
        '''
        AbelNoble.__init__(self, b=0, gamma=gamma, MW=MW)
        
    def rho_Iflow(self, rho0, Ma = 1, Ma0 = 0):
        '''
        density of gas after isentropic flow expansion
        
        Parameters
        ----------
        rho0: float
            density before exansion (kg/m^3)
        Ma: float, optional
            mach number after epansion (default assumed to be 1, choked)
        Ma0: float, optional
            Mach number before expansion (default assumed to be 0, stagnant)
        
        Returns
        -------
        rho: float
            density after expansion (kg/m^3)
        '''
        g = self.gamma
        return rho0*((1 + (g-1)/2.)*Ma**2)**(-1/(g-1))

    def T_IflowSonic(self, T0, P0, P):
        raise NotImplementedError("T_IflowSonic() is not implemented for an ideal gas.")
    
    def critical_ratio(self):
        '''
        calculates the critical ratio for choked flow
        
        Returns
        -------
        CR: float
            critical ratio of upstream to downstream pressures (dimensionless)
        '''
        gamma = self.gamma
        CR = (2 / (gamma + 1)) ** (-gamma / (gamma - 1))
        return CR



class CoolProp(EOS):
    def __init__(self, species = ['hydrogen'], x = [1.]):
        '''
        Class that uses CoolProp for equation of state calculations.
        '''
        from CoolProp import CoolProp
        self.cp = CoolProp
        if len(species) == 1:
            self.spec = species[0]
        else:
            if len(x) != len(species):
                warnings.warn('mole fractions and species lists not the same length')
            s = 'REFPROP-MIX:'
            for g, xspec in zip(species, x):
                s += g + '[' + str(xspec) + ']' + '&'
            self.spec = s[:-1]
        self.MW = self.cp.PropsSI(self.spec, 'molemass')

    def choke(self, T = None, P = None, rho = None):
        '''
        returns the thermodynamic state (T, P, rho) and velocity of a choked flow, for given stagnation conditions
        
        Parameters
        ----------
        T - temperature (K)
        P - Pressure (Pa)
        rho - density (kg/m^3)
        user must specify 2 of three thermodynamic parameters of the stagnant gas
        
        Returns
        -------
        dictionary of T, P, rho, v at the throat (orifice)
        '''
        T, P, rho = self.set_third(T, P, rho)
        gas = self.cp.State(self.spec, {'T':T, 'D':rho})
        S0, H0 = gas.get_s(), gas.get_h()*1000
        def errV(v):
            gas.update({'S':S0, 'H':(H0 - v**2/2)/1000.})
            return v - gas.get_speed_sound()
        v = optimize.brentq(errV, 0, gas.get_speed_sound())
        #h0 = h + 1/2v^2
        final = gas.update({'S':S0, 'H':(H0 - v**2/2)/1000.})
        T_throat, P_throat, rho_throat, v_throat = gas.get_T(), gas.get_p()*1000, gas.get_rho(), gas.get_speed_sound()
        class Result:
            pass
        throat = Result()
        throat.__dict__ = {'T': T_throat, 'P': P_throat, 'rho': rho_throat, 'v': v_throat}
        return throat
        
    def h(self, T, P):
        '''
        enthalpy (J/kg) of a gas at temperature T (K) and pressure P (Pa)
        
        Parameters
        ----------
        T: float
            tempearture (K)
        P: float
            pressure (Pa)
        
        Returns
        -------
        h: float
            heat capacity (J/kg)
        '''
        return self.cp.PropsSI('H', 'T', T, 'P', P, self.spec)
    
    def u(self, T = None, P = None, rho = None):
        T, P, rho = self.set_third(T, P, rho)
        gas = self.cp.State(self.spec, {'T':T, 'D':rho})
        return gas.get_u()*1000
            
    def errS(self, T0, rho0, T1, rho1):
        '''
        returns the difference in entropy (J/kg) between 2 states specified by the 
        temperatures and densities.
        
        Parameters
        ----------
        T0: float
            temperature of gas at point 0 (K)
        rho0: float
            density of gas at point 0 (kg/m^3)
        T1: float
            temperature of gas at point 1 (K)
        rho1: float
            density of gas at point 1 (kg/m^3)
            
        Returns
        -------
        errS: float
            error in enthalpy between the two different states (J/kg)
        '''
        s0 = self.cp.PropsSI('S', 'T', T0, 'D', rho0, self.spec)
        s1 = self.cp.PropsSI('S', 'T', T1, 'D', rho1, self.spec)
        return s1 / s0 - 1.
    
    def T_IflowV(self, T0, V0, P0, V, P):
        '''
        temperature of gas after isentropic flow expansion
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        V0: float
            velocity before expansion (m/s)
        P0: float
            pressure before expansion (Pa)
        V: float
            velocity after expansion (m/s)
        P: float
            pressure after expansion (Pa)
        
        Returns
        -------
        T: float
            temperature after expansion(K)
        '''
        h = self.cp.PropsSI('H', 'T', T0, 'P', P0, self.spec) + V0**2/2. - V**2/2.
        return self.cp.PropsSI('T', 'P', P, 'H', h, self.spec)
    
    def rho_Iflow(self, rho0, Ma = 1, Ma0 = 0):
        '''
        density of gas after isentropic flow expansion
        
        Parameters
        ----------
        rho0: float
            density before exansion (kg/m^3)
        Ma: float, optional
            mach number after epansion (default assumed to be 1, choked)
        Ma0: float, optional
            Mach number before expansion (default assumed to be 0, stagnant)
        
        Returns
        -------
        rho: float
            density after expansion (kg/m^3)
        '''
        T0 = 273.
        sf = self.cp.State(self.spec, {'T':T0, 'D':rho0})
        def errT(T0):
            s0 = self.cp.State(self.spec, {'T':T0, 'D':rho0})
            def errh(h):
                sf.update({'S':s0.get_s(), 'H':h})
                hf = s0.get_h()*1e3 + (Ma0*s0.get_speed_sound())**2/2 - (Ma*sf.get_speed_sound())**2/2.
                return hf/1000. - h
            hf = optimize.root(errh, s0.get_h())
            Tf = self.T_IflowV(T0, 0, s0.p*1e3, Ma*sf.get_speed_sound(), sf.p*1e3)
            return Tf - sf.T
        T0 = optimize.root(errT, T0)
        return sf.get_rho()
       
    def T_Iflow(self, T0, rho, Ma = 1, Ma0 = 0):
        '''
        temperature of gas after isentropic flow expansion
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        rho: float
            density after expansion (kg/m^3)
        Ma: float, optional
            mach number after expansion (default assumed to be 1, choked)
        
        Returns
        -------
        T: float
            temperature after expansion (K)
        '''
        def errS(x):
            [P0, Pf] = x
            P0, Pf = float(P0), float(Pf)
            s0 = self.cp.State(self.spec, {'P':P0/1e3, 'T':T0})
            sf = self.cp.State(self.spec, {'P':Pf/1e3, 'D':rho})
            hf = s0.get_h()*1e3 + (Ma0*s0.get_speed_sound())**2/2. - (Ma*sf.get_speed_sound())**2/2.
            return np.array([sf.get_s() - s0.get_s(), sf.get_h()*1e3 - hf])
        P0, Pf = optimize.fsolve(errS, np.array([101325., 101325.]))
        return self.cp.PropsSI('T', 'P', Pf, 'D', rho, self.spec)
    
    def P_Iflow(self, P0, rho, Ma = 1, Ma0 = 0):
        '''
        pressure of gas after isentropic flow expansion
        
        Parameters
        ----------
        P0: float
            pressure before expansion (Pa)
        rho: float
            density after expansion (kg/m^3)
        Ma: float, optional
            mach number after expansion (default assumed to be 1, choked)
        
        Returns
        -------
        P: float
            pressure after expansion (Pa)
        '''
        def errS(x):
            [T0, Pf] = x
            T0, Pf = float(T0), float(Pf)
            s0 = self.cp.State(self.spec, {'P':P0/1e3, 'T':T0})
            sf = self.cp.State(self.spec, {'P':Pf/1e3, 'D':rho})
            hf = s0.get_h()*1e3 + (Ma0*s0.get_speed_sound())**2/2. - (Ma*sf.get_speed_sound())**2/2.
            return np.array([sf.get_s() - s0.get_s(), sf.get_h()*1e3 - hf])
        T0, Pf = optimize.fsolve(errS, np.array([273., 101325.]))
        return Pf
    
    def T_IE(self, T0, rho0, rho):
        '''
        temperature of gas after isentropic expansion, given an old and new density
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        rho0: float
            density before expansion (kg/m^3)
        rho: float
            density after expansion (kg/m^3)
        
        Returns
        -------
        T: 
            temperature after expansion (K)
        '''
        S0 = self.cp.PropsSI('S', 'T', T0, 'D', rho0, self.spec)
        def errD(T):
            return rho - self.cp.PropsSI('D', 'T', T, 'S', S0, self.spec)
        return optimize.newton(errD, T0)
    
    def P(self, T, rho):
        '''
        returns the pressure given the temperature and density
        
        Parameters
        ----------
        T: float
            temperature (K)
        rho: flaot
            density density (kg/m^3)
        
        Returns
        -------
        P: float
            pressure (Pa)
        '''
        return self.cp.PropsSI('P', 'T', T, 'D', rho, self.spec)
    
    def T(self, P, rho):
        '''
        returns the temperature given the pressure and density
        
        Parameters
        ----------
        P: float
            pressure (Pa)
        rho: float
            density (kg/m^3)
        
        Returns
        -------
        T: float
            temperature (K)
        '''
        return self.cp.PropsSI('T', 'D', rho, 'P', P, self.spec)
    
    def rho(self, T, P):
        '''
        returns the denstiy given the temperature and pressure
        
        Parameters
        ----------
        T: float
            temperature (K)
        P: flaot
            pressure (Pa)
        
        Returns
        -------
        rho:
            density (kg/m^3)
        '''
        return self.cp.PropsSI('D', 'T', T, 'P', P, self.spec)
 
    def a(self, T, rho):
        '''
        returns the speed of sound given the temperature and density
        
        Parameters
        ----------
        T: float
            temperature (K)
        rho: float
            density (kg/m^3)
        
        Returns
        -------
        a: float
            speed of sound (m/s)
        '''
        return self.cp.PropsSI('A', 'T', T, 'D', rho, self.spec)
    
    def T_IflowSonic(self, T0, P0, P):
        '''
        solves for the temperature of gas after an adiabatic flow, assuming that the entrance
        and exit velocities are sonic
        
        Parameters
        ----------
        T0: float
            temperature before expansion (K)
        P0: float
            pressure before expansion (Pa)
        P: float
            pressure after expansion (Pa)
        
        Returns
        -------
        T: float
            temperature after expansion(K)
        '''
        V0 = self.a(T0, self.rho(T0, P0))
        def errH(T):
            V = self.a(T, self.rho(T, P))
            h = self.cp.PropsSI('H', 'T', T0, 'P', P0, self.spec) + V0**2/2. - V**2/2.
            return h - self.cp.PropsSI('H', 'T', T, 'P', P, self.spec)
        T = optimize.newton(errH, T0)
        return T
    
    def T_AdFlow(self, T0, P0, Ma0, P, Ma):
        '''
        solves for the temperature of gas after an isentropic flow, assuming that the entrance
        and exit velocities are sonic.
        
        Parameters
        ----------
        T0 : float
            temperature before expansion (K)
        P0 : float
            pressure before expansion (Pa)
        P : float
            pressure after expansion (Pa)
        
        Returns
        -------
        T : float
            temperature after expansion(K)
        '''
        V0 = Ma0*self.a(T0, self.rho(T0, P0))
        def errH(T):
            V = Ma*self.a(T, self.rho(T, P))
            h = self.cp.PropsSI('H', 'T', T0, 'P', P0, self.spec) + V0**2/2. - V**2/2.
            return h - self.cp.PropsSI('H', 'T', T, 'P', P, self.spec)
        T = optimize.newton(errH, T0)
        return T
    
    def critical_ratio(self):
        '''
        calculates the critical ratio for choked flow
        
        Returns
        -------
        CR: float
            critical ratio of upstream to downstream pressures (dimensionless)
        '''
        raise NameError('Critical ratio calculation not yet implemented for CoolProp')


# ##H2_Combustion
# 
# A module that performs H<sub>2</sub> combustion calculations.  Initilizes some functions of mass fraction for product temperaure, density, molecular weight, and the derivative of the density with respect to mass fraction.
# 
# 
# H<sub>2</sub> + $\frac{\eta}{2}$ (O<sub>2</sub> + 3.76 N<sub>2</sub>) -> $\max(0, 1-\eta)$ H<sub>2</sub> + $\min(1, \eta)$ H<sub>2</sub>O + $\max\left(0, \frac{\eta-1}{2}\right)$ O<sub>2</sub> + $3.76\frac{\eta}{2}$ N<sub>2</sub>'


class H2_Combustion:
    def __init__(self, Treac, P = 101325., numpoints = 5000,
                 MW = {'H2':2.016, 'O2':31.999, 'N2':28.013, 'H2O':18.015},
                 DHc = 118.83e6, verbose = True):
        '''
        Class that performs H2 combustion chemistry calculations.
        
        Initilizes some interpolating functions for T_prod, MW_prod, rho_prod, and drhodf
        
        Parameters
        ----------
        Treac : float
            temperature of reactants (K)
        P : float
            pressure (Pa), default value is 101325.
        numpoints : int
            number of points to solve for temperature to create 
            interpolating functions, default value is 5000
        MW : dict
            molecular weights of species (H2, O2, N2, and H2O)
        DHc : float
            heat of combustion for hydrogen (J/kg), default value of 188.83e6
            
        Contents
        --------
        self.T_prod(f): array_like
            temperature of products (K) at a mass fraction, f
        self.MW_prod:
            mixture averaged molecular weight of products (g/mol) at a mass fraction, f 
        '''
        if verbose:
            print('initializing chemistry...', end = '')
            sys.stdout.flush()
        self.MW = MW
        self.DHc = DHc
        self.Treac, self.P = Treac, P
        self.fstoich = 1/(1.5+3.76/2)*MW['H2']/(MW['H2']*1/(1.5+3.76/2) +
                                                MW['O2']*0.5/(1.5+3.76/2) + 
                                                MW['N2']*3.76/2/(1.5+3.76/2) )
        fvals = np.linspace(0, 1, numpoints)
        T = np.array(list(map(lambda fval: self._T_combustion(Treac, fval), fvals)))
        MWvals = np.array(list(map(lambda f: self._MWmix(self._Yprod(f)), fvals)))
        self.MW_prod = lambda f: self._MWmix(self._Yprod(f))
        
        # Creates a couple of interpolating functions
        # Only create them once, then just use them each time
        self._temp_prod_interp = interpolate.interp1d(fvals, T)
        self._drhodf_interp = interpolate.interp1d(
            fvals,
            P/(1000*const.R*T)*(self._D(fvals, MWvals) - self._D(fvals, T)/T*MWvals))

        self.T_prod = lambda f: self._temp_prod_interp(f)
        self.rho_prod = lambda f: P*self.MW_prod(f)/(1000*const.R*self.T_prod(f))
        self.drhodf = lambda f: self._drhodf_interp(f)
        
        if verbose:
            print('done.')
            sys.stdout.flush()
     
    def reinitilize(self, Treac, P = 101325., numpoints = 1000,
                    MW = {'H2':2.016, 'O2':31.999, 'N2':28.013, 'H2O':18.015}, DHc = 118.83e6):
        '''
        Reinitilizes class to new temperature, pressure, etc.  Can be used rather 
        than creating a new instance taking up additional memory.'''
        self.__init__(Treac, P, numpoints, MW, DHc)
    
    def _D(self, x, y):
        '''numerical derivative of y WRT x'''
        return np.append(np.append((y[1] - y[0])/(x[1] -  x[0]),
                             (y[2:] - y[:-2])/(x[2:] - x[:-2])),
                      (y[-2] - y[-1])/(x[-2] - x[-1]))
    
    def _MWtot(self, eta):
        ''''
        total molecular weight (g/mol), given eta
        '''
        return self.MW['H2'] + eta/2.*(self.MW['O2'] + 3.76*self.MW['N2'])

    def _MWmix(self, Y):
        '''returns the mixture averaged molecular weight, given a mass fraction'''
        MWmix = 0
        for spec, Yval in Y.items():
            MWmix += Yval/self.MW[spec]
        MWmix = 1./MWmix
        return MWmix
   
    def _eta(self, f):
        '''returns eta, given a mixutre fraction'''
        MW = self.MW
        eta = 2*(MW['H2']/(1.e-99*np.ones_like(f) +f) - MW['H2'])/(MW['O2'] + 3.76*MW['N2'])
        return eta
    
    def _Yprod(self, f):
        '''
        the mass fractions of combustion products as a function of the mixture fraction

        Parameters
        ----------
        f = mixture fraction

        Returns
        -------
        Y - dictionary of mass fractions (kg/kg)
        '''
        eta = self._eta(f)
        return {'H2':(1-eta)*((1-eta) > 0)*self.MW['H2']/self._MWtot(eta),
                'H2O':(eta*(eta < 1) + (eta > 1))*self.MW['H2O']/self._MWtot(eta),
                'O2':((eta-1)/2.*((eta-1) > 0))*self.MW['O2']/self._MWtot(eta),
                'N2':eta/2.*3.76*self.MW['N2']/self._MWtot(eta)}
    
    def _Yreac(self, f):
        '''
        the mass fractions of combustion reactants as a function of the mixture fraction
        (assumes that there is no H2O as a reactant)
        '''
        Y = {'H2':self.MW['H2']/self._MWtot(self._eta(f)), 
             'H2O':0.*f, 
             'O2':self._eta(f)/2.*self.MW['O2']/self._MWtot(self._eta(f)),
             'N2':self._eta(f)/2*3.76*self.MW['N2']/self._MWtot(self._eta(f))}
        return Y

    def _cp_mol(self, T): 
        '''
        heat capactiy of posible species as a function of temperature
        data from NIST chemistry webbook on 5/7/14

        Parameters
        ----------
        T: float
            temperature (K)

        Returns
        -------
        cp: dict
            dictionary of species name, heat capacity [J/(mol K)]
        '''
        t = T/1000.
        f = np.array([1., t, t**2, t**3, 1/t**2])

        if (T > 100.) and (T <= 700):
            O2 = np.array([31.32234, -20.23531, 57.86644, -36.50624, -0.007374])
        elif (T > 700) and (T <= 2000):
            O2 = np.array([30.03235, 8.772972, -3.988133, 0.788313, -0.741599])
        elif (T > 2000) and (T <= 6000):
            O2 = np.array([20.91111, 10.72071, -2.020498, 0.146449, 9.245722])
        else:
            O2 = np.array(5*[np.nan])

        if (T > 100.) and (T <= 500):
            N2 = np.array([28.98641,  1.853978,  -9.647459,  16.63537,  0.000117])
        elif (T > 500) and (T <= 2000.):
            N2 = np.array([19.50583,  19.88705,  -8.598535,  1.369784,  0.527601])
        elif (T > 2000) and (T <= 6000):
            N2 = np.array([35.51872,  1.128728,  -0.196103,  0.014662,  -4.55376])
        else:
            N2 = np.array(5*[np.nan])

        if (T > 273.) and (T <= 1000):#Webbook says this is only valid to 298, but I lowered it a bit
            H2 = np.array([33.066178, -11.363417, 11.432816, -2.772874, -0.158558])
        elif (T > 1000) and (T <= 2500):
            H2 = np.array([18.563083, 12.257357, -2.859786, 0.268238, 1.97799])
        elif (T > 2500) and (T <= 6000):
            H2 = np.array([43.41356, -4.293079, 1.272428, -0.096876, -20.533862])
        else:
            H2 = np.array(5*[np.nan])


        if (T > 500) and (T <= 1700):
            H2O = np.array([30.092, 6.832514, 6.793435, -2.53448, 0.082139])
        elif (T > 1700) and (T <= 6000):
            H2O = np.array([41.96426, 8.622053, -1.49978, 0.098119, -11.15764])
        else:
            H2O = np.array(5*[0.]) # really should be error here - need to deal with at some point

        cp = {'O2': np.dot(f, O2), 'N2': np.dot(f, N2), 'H2': np.dot(f, H2), 'H2O': np.dot(f, H2O)}
        return cp

    def _cp_mass(self, T, Y):
        '''heat capacity of a mixture J/kg-K

        Parameters
        ----------
        T : float
            Temperature (K)
        Y : dict
            dictionary of species, mass fraction values
        
        Returns
        -------
        cp : float
            heat capacity of mixture (J/kg-K)
        '''
        cp = 0.
        for species, Yval in Y.items():
            if Yval >= 1e-4:
                cp += self._cp_mol(T)[species]/self.MW[species]*1000*Yval
        return cp

    def _T_combustion(self, T_reac, f):
        '''combustion temperature (K)'''
        DHc = self.DHc*self._Yprod(f)['H2O']*self.MW['H2']/self.MW['H2O'] # heat of combustion [J/kg_total]
        H0 = self._cp_mass(T_reac, self._Yreac(f))*T_reac #J/kg
        H0 *= self._MWmix(self._Yreac(f))/self._MWmix(self._Yprod(f)) #J/kg
        H = H0 + DHc
        T = optimize.brentq(lambda T: T*self._cp_mass(T, self._Yprod(f)) - H, T_reac, 6000)
        return T
    
    def save(self, fname):
        with open(fname, 'wb') as f:
            pickle.dump(self, f)
            
def load_object(fname):
    with open(fname, 'rb') as f:
        return pickle.load(f)            
