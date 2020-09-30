# coding: utf-8
from __future__ import print_function, absolute_import, division

import abc
import sys

import numpy as np
from scipy import optimize, interpolate

from scipy import constants as const
import dill as pickle
import warnings
import pkg_resources
import pandas as pd

class CoolProp:
    def __init__(self, species = 'hydrogen'):
        '''
        Class that uses CoolProp for equation of state calculations.
        
        phase: 'gas' or 'liquid' for fluid at saturated vapor pressure
        '''
        from CoolProp import CoolProp
        self._cp = CoolProp
        self.spec = species
        self.MW = self._cp.PropsSI(self.spec, 'molemass')        
        
    def P(self, T, rho):
        '''
        returns the temperature given the pressure and density (and sets the phase)
        
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
        P, Q = self._cp.PropsSI(['P', 'Q'], 'D', rho, 'T', T, self.spec)
        self.phase = self._cp.PhaseSI('D', rho, 'T', T, self.spec)
        return P
    
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
        T, Q = self._cp.PropsSI(['T', 'Q'], 'D', rho, 'P', P, self.spec)
        self.phase = self._cp.PhaseSI('D', rho, 'P', P, self.spec)
        return T
    
    def rho(self, T, P):
        '''
        returns the denstiy given the temperature and pressure - if at saturation conditions, 
        requires phase to already be set
        
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
        try:
            rho, Q = self._cp.PropsSI(['D', 'Q'], 'T', T, 'P', P, self.spec)
            self.phase = self._cp.PhaseSI('T', T, 'P', P, self.spec)
            return rho
        except:
            try:
                warnings.warn('using %s phase information to calculate density'%self.phase)
            except:
                warnings.warn('assuming gas phase to calculate density')
                self.phase = 'gas'
            return self._cp.PropsSI('D', 'T|'+self.phase, T, 'P', P, self.spec)
            
    def rho_P(self, T, phase):
        '''
        returns the density and pressure given the temperature and phase
        
        Parameters
        ----------
        T: float
            temperautre (K)
        phase: string
            'gas' or 'liquid'
        
        Returns
        -------
        (rho, P): tuple of floats 
            rho - density (kg/m^3)
            P - pressure (Pa)
        '''
        rho, P = self._cp.PropsSI(['D', 'P'], 'T', T, 'Q', {'gas':1, 'liquid':0}[phase], self.spec)
        self.phase = phase
        return rho, P
        
    def rho_T(self, P, phase):
        '''
        returns the density and temperature given the pressure and phase
        
        Parameters
        ----------
        P: float
            temperautre (K)
        phase: string
            'gas' or 'liquid'
        
        Returns
        -------
        (rho, P): tuple of floats 
            rho - density (kg/m^3)
            P - pressure (Pa)
        '''
        rho, T = self._cp.PropsSI(['D', 'T'], 'P', P, 'Q', {'gas':1, 'liquid':0}[phase], self.spec)
        self.phase = phase
        return rho, T
        
    def P_T(self, rho, phase):
        '''
        returns the pressure and temperature given the density and phase
        
        Parameters
        ----------
        T: float
            temperautre (K)
        phase: string
            'gas' or 'liquid'
        
        Returns
        -------
        (rho, P): tuple of floats 
            rho - density (kg/m^3)
            P - pressure (Pa)
        '''
        P, T = self._cp.PropsSI(['P', 'T'], 'D', rho, 'Q', {'gas':1, 'liquid':0}[phase], self.spec)
        self.phase = phase
        return P, T
        
    def _err_H(self, T1, P1, v1, T2, P2, v2, usePhase1 = False, usePhase2 = False):
        '''
        error in total enthalpy (J/kg) for a gas at two different states and velocities
        
        Parameters
        ----------
        T1: float
            tempearture (K) at state 1
        P1: float
            pressure (Pa) at state 1
        v1: float
            velocity (m/s) at state 1
        T2: float
            tempearture (K) at state 2
        P2: float
            pressure (Pa) at state 2
        v2: float
            velocity (m/s) at state 2
        
        Returns
        -------
        err_h: float
            error in enthalpy (J/kg)
        '''
        try:
            h1 = self._cp.PropsSI('H', 'T', T1, 'P', P1, self.spec)
        except:
            print('tp')
            h1 = self._cp.PropsSI('H', 'T|'+self.phase, T1, 'P', P1, self.spec)
        try:
            h2 = self._cp.PropsSI('H', 'T', T2, 'P', P2, self.spec)
        except:
            print('tp')
            h2 = self._cp.PropsSI('H', 'T|'+self.phase, T2, 'P', P2, self.spec)
        return h1 + v1**2/2. - (h2 + v2**2/2.)
        
    def _err_H_P_rho(self, P1, rho1, v1, P2, rho2, v2):
        '''
        error in total enthalpy (J/kg) for a gas at two different states and velocities
        
        Parameters
        ----------
        T1: float
            tempearture (K) at state 1
        P1: float
            pressure (Pa) at state 1
        v1: float
            velocity (m/s) at state 1
        T2: float
            tempearture (K) at state 2
        P2: float
            pressure (Pa) at state 2
        v2: float
            velocity (m/s) at state 2
        
        Returns
        -------
        err_h: float
            error in enthalpy (J/kg)
        '''
        try:
            h1 = self._cp.PropsSI('H', 'P', P1, 'D', rho1, self.spec)
        except:
            print('tp')
            h1 = self._cp.PropsSI('H', 'P|'+self.phase, P1, 'D', rho1, self.spec)
        try:
            h2 = self._cp.PropsSI('H', 'P', P2, 'D', rho2, self.spec)
        except:
            print('tp')
            h2 = self._cp.PropsSI('H', 'P|'+self.phase, P2, 'D', rho2, self.spec)
        return h1 + v1**2/2. - (h2 + v2**2/2.)
    
    def _err_S(self, T1, P1, T2, P2):
        '''
        returns the difference in entropy (J/kg) between 2 states specified by the 
        temperatures and pressures.
        
        Parameters
        ----------
        T1: float
            temperature of gas at point 1 (K)
        P1: float
            pressure of gas at point 1 (Pa)
        T2: float
            temperature of gas at point 2 (K)
        P2: float
            Pressure of gas at point 2 (Pa)
            
        Returns
        -------
        err_S: float
            error in enthalpy between the two different states (J/kg)
        '''
        try:
            s1 = self._cp.PropsSI('S', 'T', T1, 'P', P1, self.spec)
        except:
            print('tp')
            s1 = self._cp.PropsSI('S', 'T|'+self.phase, T1, 'P', P1, self.spec)
        try:
            s2 = self._cp.PropsSI('S', 'T', T2, 'P', P2, self.spec)
        except:
            print('tp')
            S2 = self._cp.PropsSI('S', 'T|'+self.phase, T2, 'P', P2, self.spec)
        return s1 - s2
        
    def _X(self, Y, other = 'air'):
        MW = self._cp.PropsSI('M', self.spec)
        MW_other = self._cp.PropsSI('M', other)
        return Y/MW/(Y/MW + (1-Y)/MW_other)
    
    # def _err_adiabatic_out(self, T1, P1, T2, P2, dm_per_m):
        # '''
        # returns the difference in internal energy (J/kg) between 2 states specified by the 
        # temperatures and pressures.
        
        # Parameters
        # ----------
        # T1: float
            # temperature of gas at point 1 (K)
        # P1: float
            # pressure of gas at point 1 (Pa)
        # T2: float
            # temperature of gas at point 2 (K)
        # P2: float
            # Pressure of gas at point 2 (Pa)
        # dm_per_m: float
            # relative mass difference between state 2 and 1 (m2 - m1)/m1
        # Returns
        # -------
        # err_adiabatic: float
            # error in energy balance between the two different states
        # '''
        # U1, H1, rho1 = self._cp.PropsSI(['U', 'H', 'D'], 'T', T1, 'P', P1, self.spec)
        # U2, rho2 = self._cp.PropsSI(['U', 'D'], 'T', T2, 'P', P2, self.spec)
        # return (rho2*U2 - rho1*U1) / (dm_per_m*rho1*H1)-1
            
    def a(self, T, P, S = None):
        '''
        returns the speed of sound given the temperature and pressure, or temperature and entropy
        
        Parameters
        ----------
        T: float
            temperature (K)
        P: float
            Pressure (Pa)
        S: float
            Entropy (J/K)
        
        Returns
        -------
        a: float
            speed of sound (m/s)
        '''
        try:
            a = self._cp.PropsSI('A', 'T', T, 'P', P, self.spec)
            if not np.isfinite(a):
                P_pterb = 0.1
                rho = upstream_fluid.therm._cp.PropsSI('D', 'P|'+self.phase,  [P, P + P_pterb], 'T', T, self.spec)
                a = np.sqrt(P_pterb/np.gradient(rho))[0]
            return a
        except:
            csg, rhog = self._cp.PropsSI(['A', 'D'], 'T|gas', T, 'P', P, self.spec)
            csl, rhol = self._cp.PropsSI(['A', 'D'], 'T|liquid', T, 'P', P, self.spec)
            rho, quality = self._cp.PropsSI(['D', 'Q'], 'T', T, 'S', S, self.spec)
             # calculate gas/liquid void fractions
            void_frac_gas = rho * quality / rhog
            void_frac_liquid = 1.0 - void_frac_gas

            # calculate mixture sound speed
            term = np.sqrt(rhog * csg ** 2 / (void_frac_liquid * rhog * csg ** 2 + void_frac_gas * rhol * csl ** 2))
            cs = csg * csl * term / (void_frac_liquid * csg + void_frac_gas * csl * term)
            return cs
        
    def h(self, T = None, P = None, rho = None, phase = None):
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
        if phase is None:
            str = '|not_imposed'
        else:
            str = '|' + phase        
        if T is not None and P is not None:
            return self._cp.PropsSI('H', 'T' + str, T, 'P', P, self.spec)
        elif T is not None and rho is not None:
            return self._cp.PropsSI('H', 'T', T, 'D', rho, self.spec)
        elif P is not None and rho is not None:
            return self._cp.PropsSI('H', 'D', rho, 'P', P, self.spec)
        else:
            raise warnings.warn('system not properly defined')
            
    def s(self, T = None, P = None, rho = None, phase = None):
        '''
        entropy (J/kg-K) of a fluid at temperature T (K) and pressure P (Pa)
        
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
        if phase is None:
            str = '|not_imposed'
        else:
            str = '|' + phase
        if T is not None and P is not None:
            return self._cp.PropsSI('S', 'T' + str, T, 'P', P, self.spec)
        elif T is not None and rho is not None:
            return self._cp.PropsSI('S', 'T' + str, T, 'D', rho, self.spec)
        elif P is not None and rho is not None:
            return self._cp.PropsSI('S', 'D' + str, rho, 'P', P, self.spec)
        else:
            raise warnings.warn('system not properly defined')

    # def critical_ratio(self):
        # '''
        # calculates the critical ratio for choked flow
        
        # Returns
        # -------
        # CR: float
            # critical ratio of upstream to downstream pressures (dimensionless)
        # '''
        # gamma =self._cp.PropsSI('CPMASS', 'P', const.atm, 'T', 295.,  self.spec
                                # )/self._cp.PropsSI('CVMASS', 'P', const.atm, 'T', 295.,  self.spec)
        # CR = (2 / (gamma + 1)) ** (-gamma / (gamma - 1))
        # return CR

            
#########################################################################################            
class Combustion:
    def __init__(self, Treac, nC = 0,  P = 101325., numpoints = 100,
                 verbose = False):
        '''
        Class that performs combustion chemistry calculations.
        Stoichiometry: C_nH_(2n) + eta/2 (O_2 + 3.76N_2) -> max(0, 1-eta) C_nH_(2n) + min(1, eta) H_2O + min(n*eta, n)CO_2 + max(0, (eta-1)/2) O2 + 3.76 eta/2 N2
        
        Initilizes some interpolating functions for T_prod, MW_prod, rho_prod, and drhodf
        
        Parameters
        ----------
        Treac : float
            temperature of reactants (K)
        nC : int
            number of carbon molecules in the fuel (assumed to be a normal alkane with 2*Nc + 2 hydrogens)
            default value of 0 is for hydrogen
        P : float
            pressure (Pa), default value is 101325. (this is unused as of now, lacking heat capacity data for alternate pressures)
        numpoints : int
            number of points to solve for temperature to create 
            interpolating functions, default value is 100
        MW : dict
            molecular weights of elements (C, H, O, N)
            
        Contents
        --------
        self.T_prod(f): array_like
            temperature of products (K) at a mixture fraction, f
        self.MW_prod:
            mixture averaged molecular weight of products (g/mol) at a mixture fraction, f 
        '''
        if verbose:
            print('initializing chemistry...', end = '')
        from CoolProp.CoolProp import PropsSI
        self.PropsSI = PropsSI
        reac = ('C%dH%d' % (nC, 2*nC+2)).replace('C0', '').replace('C1', 'C')
        self.reac = reac 
        self._nC = nC
        stream = pkg_resources.resource_stream(__name__, 'data/fuel_prop.dat') 
        fuel_props = pd.read_csv(stream, index_col = 0, skipinitialspace=True)
        self.DHc = fuel_props.dHc[reac] # heat of combustion

        self.Treac, self.P = Treac, P
        MW = dict([[spec, PropsSI('M', spec)] for spec in ['O2', 'N2', 'H2O', 'CO2', reac]])
        self.MW = MW
        self.fstoich = MW[reac]/(MW[reac] + (3*nC+1)/2. * (MW['O2'] + MW['N2']*3.76))
        ifstoich = int(max(numpoints*self.fstoich, 5))
        fvals = np.append(np.linspace(0, self.fstoich, int(max(numpoints*self.fstoich, 5))), 
                          np.linspace(self.fstoich, 1, int(max(numpoints*(1-self.fstoich), 5))));
        T = self._T_combustion(Treac, fvals)
        MWvals = self._MWmix(self._Yprod(fvals))
        self.MW_prod = lambda f: self._MWmix(self._Yprod(f))
        # Creates a couple of interpolating functions
        # Only create them once during initialization, to use as a lookup value
        self.T_prod = interpolate.interp1d(fvals, T)
        self.rho_prod = lambda f: P*self.MW_prod(f)/(const.R*self.T_prod(f))
        self.drhodf = interpolate.interp1d(fvals,
                                           P/(const.R*T)*(np.append(np.gradient(MWvals[:ifstoich], fvals[:ifstoich]),
                                                          np.gradient(MWvals[ifstoich:], fvals[ifstoich:])) - 
                                                          np.append(np.gradient(T[:ifstoich], fvals[:ifstoich]),
                                                          np.gradient(T[ifstoich:], fvals[ifstoich:]))/T*MWvals))

        if verbose:
            print('done.')

    def reinitilize(self, Treac, nC = 0, P = 101325., numpoints = 100):
        '''
        Reinitilizes class to new temperature, pressure, etc.  Can be used rather 
        than creating a new instance taking up additional memory.'''
        self.__init__(Treac, nC, P, numpoints)

    def _MWmix(self, Y):
        '''returns the mixture averaged molecular weight, given a mass fraction'''
        MWmix = 0
        for spec, Yval in Y.items():
            MWmix += Yval/self.MW[spec]
        MWmix = 1./MWmix
        return MWmix
   
    def _eta(self, f):
        '''returns eta, given a mixture fraction'''
        MW = self.MW
        eta = 2./(3*self._nC+1)*(MW[self.reac]/(1.e-99*np.ones_like(f) +f) - MW[self.reac])/(MW['O2'] + 3.76*MW['N2'])
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
        nC = self._nC
        Y = {self.reac:(1-eta)*((1-eta) > 0)*self.MW[self.reac],
             'CO2':nC*(eta*(nC>=eta*nC) + (nC<eta*nC))*self.MW['CO2'],
             'H2O':(nC+1)*(eta*(nC+1 >= eta*(nC+1)) + (nC+1 < eta*(nC+1)))*self.MW['H2O'],
             'O2':((eta-1)/2.*(3*nC+1)*((eta-1)/2.*(3.*nC+1) > 0))*self.MW['O2'],
             'N2':eta/2.*(3*nC+1)*3.76*self.MW['N2']}
        s = sum(Y.values())
        for k in Y.keys():
            Y[k] /= s
        return Y
    
    def _Yreac(self, f):
        '''
        the mass fractions of combustion reactants as a function of the mixture fraction
        (assumes that there is no H2O or CO2 as a reactant)
        '''
        Y = {self.reac:self.MW[self.reac], 
             'CO2': 0.*f,
             'H2O':0.*f, 
             'O2':self._eta(f)/2.*(3*self._nC+1)*self.MW['O2'],
             'N2':self._eta(f)/2*(3*self._nC+1)*3.76*self.MW['N2']}
        s = sum(Y.values())
        for k in Y.keys():
            Y[k] /= s
        return Y
    
    def _H(self, T, Y, Hdict):
        '''enthalpy of a mixture at a given temperature and pressure'''
        H = np.zeros_like(list(Y.values())[0])
        for species, Yval in Y.items():
            H += Hdict[species](T)*Yval
        return H
        
    def _Hdict(self, Tmin, Tmax = 6000, npoints = 500):
        '''returns dictionary of interpolating enthalpy functions from Tmin to Tmax'''
        Hdict = {}
        for spec in self.MW.keys():
            T = np.linspace(np.max([Tmin, self.PropsSI('T_min', spec)+0.1]), Tmax, npoints)
            Hdict[spec] = interpolate.interp1d(T, self.PropsSI('H', 'T', T, 'P', self.P, spec), 
                                               fill_value = 'extrapolate')
        return Hdict

    def _T_combustion(self, T_reac, f, numpoints = 500):
        '''combustion temperature (K)'''
        DHc = self.DHc*self._Yprod(f)['H2O']/(self._nC+1)*self.MW[self.reac]/self.MW['H2O'] # heat of combustion [J/kg_total]
        Hdict = self._Hdict(T_reac, npoints = numpoints)
        H0 = self._H(T_reac, self._Yreac(f), Hdict)
        H0 *= self._MWmix(self._Yreac(f))/self._MWmix(self._Yprod(f)) #J/kg
        H = H0 + DHc
        T = optimize.root(lambda T: self._H(T, self._Yprod(f), Hdict) - H, T_reac*np.ones_like(np.array(f)))['x']
        return T
    
    def save(self, fname):
        with open(fname, 'wb') as f:
            pickle.dump(self, f)

def load_object(fname):
    with open(fname, 'rb') as f:
        return pickle.load(f)            
