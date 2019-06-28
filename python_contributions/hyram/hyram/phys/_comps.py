
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

# coding: utf-8

# ### Components in H<sub>2</sub> Behaviors Models

from __future__ import print_function, absolute_import, division

import sys
import warnings

import numpy as np
from scipy import integrate, optimize


class Gas:
    def __init__(self, therm, T = None, P = None, rho = None):
        '''class used to describe a gas
        
        Parameters
        ----------
        therm : thermodynamic class
            a thermodynamic class that is used to relate state variables
        T : float
            temperature (K)
        P: float
            pressure (Pa)
        rho: float
            density (kg/m^3)
        two of three (T, P, rho) are needed to fully define the gas
        '''
        if T != None and P != None:
            self.T = T
            self.rho = therm.rho(T, P)
            self.P = P
        elif T != None and rho != None:
            self.T = T
            self.rho = rho
            self.P = therm.P(T, rho)
        elif P != None and rho != None:
            self.T = therm.T(P, rho)
            self.rho = rho
            self.P = P
        else:
            warnings.warn('system not properly defined')
            self.T, self.P, self.rho = T, P, rho
        self.therm = therm
    def update(self, T = None, P = None, rho = None):
        if T != None and P != None:
            self.T = T
            self.rho = self.therm.rho(T, P)
            self.P = P
        elif T != None and rho != None:
            self.T = T
            self.rho = rho
            self.P = self.therm.P(T, rho)
        elif P != None and rho != None:
            self.T = self.therm.T(P, rho)
            self.rho = rho
            self.P = P
        else:
            warnings.warn('system not properly defined')
            self.T, self.P, self.rho = T, P, rho


class Orifice:
    def __init__(self, d, Cd = 1.):
        '''
        class used to describe a circular orifice
        
        future versions may be expanded to give effective area for other shapes
        
        Parameters
        ----------
        d - orifice diameter (m)
        Cd - discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow)
        
        Contains
        --------
        d - diameter (m)
        Cd- discharge coefficient 
        A - effective area (m^2)
        '''
        self.d, self.Cd, self.A = d, Cd, np.pi/4*d**2
    
    def mdot(self, rho, v):
        '''
        mass flow rate through orifice given gas density and velocity
        
        Parameters
        ----------
        rho - density (kg/m^3)
        v - velocity (m/s)
        
        Returns
        -------
        mdot - mass flow rate (kg/s)
        '''
        return rho*v*self.A*self.Cd
    
    def mdot_choked(self, gas):
        '''
        choked mass flow rate through orifice given the stagnation gas 
        
        Parameters
        ----------
        gas - stagnant gas object
        
        Returns
        -------
        mdot, gas - tuple, mass flow rate (kg/s), gas object at throat
        '''
        throat = gas.therm.choke(T = gas.T, P = gas.P)
        return throat.rho*throat.v*self.A*self.Cd


class Source:
    '''used to describe a source (tank) that contains gas'''
    def __init__(self, V, gas):
        '''
        Initializes source based on the volume and the gas object in the source (tank)
        
        Parameters
        ----------
        V: float
            volume of source (tank) (m^3)
        gas: object
            gas object in the source (tank)
        
        Returns
        -------
        source: object
            object containing .gas (gas obejct), .V (volume, m^3), and .m (mass (kg))
        '''
        self.gas = gas
        self.V = V
        self.m = gas.rho*V

    @classmethod
    def fromMass(cls, m, gas):
        '''
        Initilization method based on the mass and the gas object in the source (tank)
        
        Parameters
        ----------
        m: float
            mass of source (tank) (kg)
        gas: object
            gas object in the source (tank)
        
        Returns
        -------
        source: object
            object containing .gas (gas obejct), .V (volume, m^3), and .m (mass (kg))
        '''
        V = m/gas.rho
        return cls(V, gas)

    @classmethod
    def fromMass_Vol(cls, m, V, therm, T = None, P = None):
        '''
        Initilization method based on the mass, volume, and either the temperature or pressure of the gas 
        in the source (tank).
        
        Parameters
        ----------
        m: float
            mass of source (tank) (kg)
        V: float
            volume of source (tank) (m^3)
        therm: object
            thermodynamic class used to releate pressure, temperature and density
        T: float (optional)
            temperature (K)
        P: float (optional)
            pressure (Pa)
        
        Returns
        -------
        source: object
            object containing .gas (gas obejct), .V (volume, m^3), and .m (mass (kg)).
        returns none if eitehr underspecified (neither T or P given) or overspecified (both T and P given)
        '''
        rho = m/V
        if T is not None and P is None:
            gas = Gas(therm, rho = rho, T = T)
        elif T is None and P is not None:
            gas = Gas(therm, rho = rho, P = P)
        else:
            return None
        return cls(V, gas)

    def mdot(self, orifice, Ma = 1):
        '''returns the mass flow rate through an orifice, from the current tank conditions'''
        rho_throat = self.gas.therm.rho_Iflow(self.gas.therm.rho(self.gas.T, self.gas.P), Ma = Ma)
        T_throat = self.gas.therm.T_Iflow(self.gas.T, rho_throat, Ma = Ma)
        return orifice.mdot(rho_throat, self.gas.therm.a(T_throat, rho_throat)*Ma)

    def blowdown(self, t, orifice, Ma = 1):
        '''Returns the mass flow rate and static gas history over time for a storage tank with an orifice.
        '''
        dt_array = t[1:] - t[:-1]
        mdot = np.empty(dt_array.size + 1)
        mdot[0] = self.mdot(orifice, Ma)
        gas_list = [Gas(self.gas.therm, T = self.gas.T, P = self.gas.P)]
        mtot = self.gas.rho*self.V
        for i in range(len(dt_array)):
            dt = dt_array[i]
            mtot -= self.mdot(orifice, Ma)*dt
            T = self.gas.therm.T_IE(self.gas.T, self.gas.rho, mtot/self.V)
            P = self.gas.therm.P(self.gas.T, mtot/self.V)
            self.gas.update(T = T, P = P)
            mdot[i+1] = self.mdot(orifice, Ma)
            gas_list.append(Gas(self.gas.therm, T = self.gas.T, P = self.gas.P))
        return mdot, gas_list

    def empty(self, orifice, Ma = 1, P_empty = 101325., nsteps = 1000):
        '''empties for a storage tank through an orifice.
        '''
        mtot = self.gas.rho*self.V
        dmass = mtot / nsteps
        mdot = [self.mdot(orifice, Ma)]
        t = [0.]
        dt = dmass/mdot[-1]
        gas_list = [Gas(self.gas.therm, T = self.gas.T, P = self.gas.P)]
        
        i = 0
        while (gas_list[-1].P > P_empty) and i < nsteps + 1:
            i += 1
            t.append(t[-1] + dt)
            mtot -= self.mdot(orifice, Ma)*dt
            T = self.gas.therm.T_IE(self.gas.T, self.gas.rho, mtot/self.V)
            self.gas.update(T = T, rho = mtot/self.V)
            
            mdot.append(self.mdot(orifice, Ma))
            dt = dmass/mdot[-1]
            gas_list.append(Gas(self.gas.therm, T = self.gas.T, P = self.gas.P))
        return np.array(mdot), gas_list, np.array(t)
    
    def _T(self, u, rho, therm):
        '''
        returns gas temperature given an internal energy and density
        
        Parameters
        ----------
        u - internal energy (J/kg)
        rho - density (kg/m^3)
        therm - thermodynamic object that can calculate the internal energy
        
        Returns
        -------
        T - gas temperature (K)
        '''
        u_err = lambda T: therm.u(T = T, rho = rho) - u
        return optimize.newton(u_err, self.gas.T)
    
    def _blowdown_gov_eqns(self, t, ind_vars, Vol, A_orifice, Q, therm):
        '''governing equations for energy balance on a tank
        
        Parameters
        ----------
        t - time (s)
        ind_vars - array of mass (kg), internal energy (J/kg) in tank
        Vol - float, volume of tank (m^3)
        A_orifice - float, cross-sectional area of orifice (m^2)
        Q - float, heat flow into tank (W)
        therm - object, thermodynamic object that can return the enthalpy h, the choke conditions
        
        Returns
        -------
        [dm_dt, du_dt] = array of [d(mass)/dt (kg/s), d(internal energy)/dt (J/kg-s)]
                       = [-rho_throat*v_throat*A_throat, 1/m*(Q_in + mdot_out*(u - h_out))]
        '''
        m, u_val = ind_vars
        m, u_val = float(m), float(u_val)
        rho = m/Vol
        T_val = self._T(u_val, rho, therm)
        throat = therm.choke(T = T_val, rho = rho)
        h = therm.h(T_val, therm.P(T_val, rho))
        #h = therm.h(throat['T'], throat['P'])
        dm_dt = -throat.rho*throat.v*A_orifice
        du_dt = 1./m*(Q + (h - u_val)*dm_dt)
        return np.array([dm_dt, du_dt])
   
    def empty_energy(self, orifice, P_empty = 101325., dt = 10, Q = 0):
        '''
        integrates the governing equations for an energy balance on a tank
        
        Parameters
        ----------
        orifice - orifice object through which the source is emptying
        P_empty - pressure at which tank is considered empty (Pa)
        dt - time step (s)
        Q - Heat flow (W) into tank.  Assumed to be 0 (adiabatic)
        
        Returns
        -------
        tuple of (mdot, gas_list, t) =  
                 (list of mass flow rates (kg/s), list of gas objects at each time step, list of times (s))
        '''
        therm = self.gas.therm
        m0 = self.m
        volume = self.V
        A_orifice = orifice.A*orifice.Cd
        T0, rho0, P = self.gas.T, self.gas.rho, self.gas.P
        u0 = therm.u(T0, rho0)
        r = integrate.ode(self._blowdown_gov_eqns).set_integrator('dopri5')
        r.set_initial_value([m0, u0]).set_f_params(volume, A_orifice, Q, therm)
        throat = therm.choke(T = T0, rho = rho0)
        t = [r.t]
        gas_list = [Gas(therm, T = T0, rho = rho0)]
        mdot = [throat.rho*throat.v*A_orifice]
        while(r.successful() and P > 2*101325.):
            r.integrate(r.t + dt)
            t.append(r.t)
            rho = r.y[0]/volume
            T = self._T(r.y[1], rho, therm)
            throat = therm.choke(T = T, rho = rho)
            gas = Gas(therm, T = T, rho = rho)
            P = gas.P
            gas_list.append(gas)
            mdot.append(throat.rho*throat.v*A_orifice)
            sys.stdout.flush()
        return mdot, gas_list, t


# Governning equations:
# 
# $$\frac{dm}{dt} = - \dot{m}_{\rm out} = -\rho_{\rm throat} v_{\rm throat} A_{\rm throat}$$
# $$\frac{d(mu)}{dt} = Q_{\rm in} - \dot{m}_{\rm out} h_{\rm out}$$
# 
# product rule, substitute:
# $$\frac{du}{dt} = \frac{1}{m}\left[Q_{\rm in} + \dot{m}_{\rm out}\left(u - h_{\rm out}\right)\right]$$


class Enclosure:
    '''
    Enclosure used in the overpressure modeling
    '''
    def __init__(self, H, A, H_release, ceiling_vent, floor_vent, Xwall = np.inf):
        '''
        Describes the enclosure
        
        Parameters
        ----------
        H : encosure height (m)
        A : area of floor and ceiling (m^2) 
        H_release : height of release (m)
        ceiling_vent : vent class containing vent information for ceiling vent
        floor_vent : vent class containing vent information for floor vent
        Xwall : perpendicular from jet to wall (m)
        '''
        self.H, self.A, self.ceiling_vent, self.floor_vent = H, A, ceiling_vent, floor_vent
        self.H_release, self.Xwall = H_release, Xwall
        self.V = H*A
        
class Vent:
    '''
    Vent used in overpressure modeling
    '''
    def __init__(self, A, H, Cd = 1, vol_flow_rate = 0):
        '''
        Describes the vent
        
        Parameters
        ----------
        A : vent cross-sectional area (m^2)
        H : vent height from floor (m)
        Cd: discharge coefficient of vent
        vol_flow_rate: volumetric flow rate through the vent (m^3/s)
        '''
        self.A, self.H, self.Cd, self.vol_flow_rate = A, H, Cd, vol_flow_rate
        #self.Qw = Cd*A*U_wind/np.sqrt(2) #See Lowesmith et al IJHE 2009
        self.Qw = Cd*vol_flow_rate/np.sqrt(2) #See Lowesmith et al IJHE 2009

