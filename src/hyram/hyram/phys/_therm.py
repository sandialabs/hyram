"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from __future__ import print_function, absolute_import, division

import warnings
import logging

from CoolProp import CoolProp
import numpy as np
from scipy import optimize, interpolate
from scipy import constants as const

from ._fuel_props import Fuel_Properties
from ..utilities.custom_warnings import PhysicsWarning


log = logging.getLogger(__name__)


class CoolPropWrapper:
    def __init__(self, species = 'hydrogen'):
        '''
        Class that uses CoolProp for equation of state calculations.
        
        phase: 'gas' or 'liquid' for fluid at saturated vapor pressure
        '''
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
        P, Q = self.PropsSI(['P', 'Q'], D=rho, T = T)
        try:
            self.phase = self._cp.PhaseSI('D', rho, 'P', P, self.spec)
        except ValueError:
            if (Q < 1) and (Q > 0):
                self.phase = 'twophase'
            elif Q == 1:
                self.phase = 'vapor'
            elif Q == 0:
                self.phase = 'liquid'
            else:
                self.phase = ''
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
        T, Q = self.PropsSI(['T', 'Q'], D=rho, P=P)
        try:
            self.phase = self._cp.PhaseSI('D', rho, 'P', P, self.spec)
        except ValueError:
            if (Q < 1) and (Q > 0):
                self.phase = 'twophase'
            elif Q == 1:
                self.phase = 'vapor'
            elif Q == 0:
                self.phase = 'liquid'
            else:
                self.phase = ''
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
            rho, Q = self.PropsSI(['D', 'Q'], T = T, P = P)
            try:
                self.phase = self._cp.PhaseSI('D', rho, 'P', P, self.spec)
            except ValueError:
                if (Q < 1) and (Q > 0):
                    self.phase = 'twophase'
                elif Q == 1:
                    self.phase = 'vapor'
                elif Q == 0:
                    self.phase = 'liquid'
                else:
                    self.phase = ''
            return rho
        except:
            try:
                warnings.warn('Using {} phase information to calculate density.'.format(self.phase),
                              category=PhysicsWarning)
            except:
                warnings.warn('Assuming gas phase to calculate density.', category=PhysicsWarning)
                self.phase = 'gas'
            rho = self._cp.PropsSI('D', 'T|'+self.phase, T, 'P', P, self.spec)

            return rho
            
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
            h1 = self.PropsSI('H', P = P1, D = rho1)
        except:
            print(f'exception P1:{P1}, rho1:{rho1}, {self.phase}')
            h1 = self.PropsSI('H', **{'P|'+self.phase: P1, 'D':rho1})
        try:
            h2 = self.PropsSI('H', P = P2, D = rho2)
        except:
            print(f'exception P2:{P2}, rho2:{rho2}, {self.phase}')
            h2 = self.PropsSI('H', **{'P|'+self.phase: P2, 'D': rho2})
        return h1 + v1**2/2. - (h2 + v2**2/2.)
        
    def _X(self, Y, other = 'air'):
        MW = self._cp.PropsSI('M', self.spec)
        MW_other = self._cp.PropsSI('M', other)
        return Y/MW/(Y/MW + (1-Y)/MW_other)
    
    def a(self, T = None, P = None, S = None):
        '''
        returns the speed of sound given the temperature and entropy or pressure and entropy
          note: 2-phase calcualtions no longer supported by this method see git history if
                interested in 2-phase speed of sound calculations
        
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
        if S == None and P != None and T != None: 
            a = self.PropsSI('A', T = T, P = P)
        elif P == None and S != None and T != None:
            a, Q, P = self.PropsSI(['A', 'Q', 'P'], T=T, S=S)
        elif T == None and P != None and S != None:
            a, Q, T = self.PropsSI(['A', 'Q', 'T'], P=P, S=S)
        else:
            raise warnings.warn('Under-defined - need 2 of T, P, S', category=PhysicsWarning)
            return None
        return a
    
    def PropsSI(self, output, **kwargs):
        '''wrapper on CoolProps PropsSI
        
        Parameters 
        ----------
        those accepted by CoolProp.PropsSI (e.g., T, P, S, D - with the addition of the keyword 'phase')
        
        Returns
        -------
        Outputs from CoolProp listed within output (could be single value or list)
        '''
        if 'phase' in kwargs:
            phase =  kwargs.pop('phase')
        else:
            phase = ''        
        try:
            (k1, v1), (k2, v2)  = kwargs.items()
            k1 += phase
            out = self._cp.PropsSI(output, k1, v1, k2, v2, self.spec)
            return out
        except ValueError:
            if ('T' in kwargs) and ('D' in kwargs):
                T = kwargs['T']
                D = kwargs['D']
                def err(P):
                    return D - self._cp.PropsSI('D', 'T', T, 'P', P, self.spec)
                P = optimize.root(err, D*8.314*T/self.MW)['x']
            elif ('P' in kwargs) and ('D' in kwargs):
                P = kwargs['P']
                D = kwargs['D']
                Tmin = self._cp.PropsSI('Tmin', self.spec)
                def err(T):
                    return D - self._cp.PropsSI('D', 'T', max(T, Tmin), 'P', P, self.spec)
                #dT = 5
                #err0 = err(P*self.MW/(D*8.314))
                #err1 = err(P*self.MW/(D*8.314) + dT)
                #Tnext = P*self.MW/(D*8.314) + dT/(err0 - err1)*err0 #- 20*np.sign(err0 - err1)
                #T = optimize.brenth(err, P*self.MW/(D*8.314), Tnext)
                T = optimize.root(err, max(P*self.MW/(D*8.314), Tmin))['x']
            elif 'P' in kwargs:
                P = kwargs.pop('P')
                k, v = list(kwargs.items())[0]
                def err(T):
                    err = v - self._cp.PropsSI(k, 'T', T, 'P', P, self.spec)
                    return err
                T = optimize.root(err, 150)['x']
            elif 'T' in kwargs:
                T = kwargs.pop('T')
                k, v = list(kwargs.items())[0]
                def err(P):
                    return v - self._cp.PropsSI(k, 'T', T, 'P', P, self.spec)
                P = optimize.root(err, 101325.)['x'] 
            out = self._cp.PropsSI(output, 'T', T, 'P', P, self.spec)
            return out
        except:
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

            

class Combustion:
    def __init__(self, fluid, #ambient, # TODO: add ambient object
                 numpoints = 100, verbose = False):
        '''
        Class that performs combustion chemistry calculations.
        Stoichiometry: C_nH_(2n) + eta/2 (O_2 + 3.76N_2) -> max(0, 1-eta) C_nH_(2n) + min(1, eta) H_2O + min(n*eta, n)CO_2 + max(0, (eta-1)/2) O2 + 3.76 eta/2 N2
        
        Initilizes some interpolating functions for T_prod, MW_prod, rho_prod, and drhodf
        
        Parameters
        ----------
        fluid : hyram.phys.Fluid object
            fluid being combusted
        ambient: hyram.phys.Fluid object with ambient air
            air with with the fluid is being combusted
        numpoints : int
            number of points to solve for temperature to create 
            interpolating functions, default value is 100
        verbose: boolean
            whether to include some print statements
            
        Contents
        --------
        self.T_prod(f): array_like
            temperature of products (K) at a mixture fraction, f
        self.MW_prod:
            mixture averaged molecular weight of products (g/mol) at a mixture fraction, f 
        '''
        fuel_props = Fuel_Properties(fluid.species)
        nC = fuel_props.nC

        Treac, P = fluid.T, fluid.P
        # TODO: # Tair, P = ambient.T, ambient.P
        if verbose:
            print('initializing chemistry...', end = '')
        from CoolProp.CoolProp import PropsSI
        self.PropsSI = PropsSI
        reac = ('C%dH%d' % (nC, 2*nC+2)).replace('C0', '').replace('C1', 'C')
        self.reac = reac 
        self._nC = nC

        self.DHc = fuel_props.dHc # heat of combustion, J/kg

        self.Treac, self.P = Treac, P
        MW = dict([[spec, PropsSI('M', spec)] for spec in ['O2', 'N2', 'H2O', 'CO2', reac]])
        self.MW = MW
        self.fstoich = MW[reac]/(MW[reac] + (3*nC+1)/2. * (MW['O2'] + MW['N2']*3.76))
        ifstoich = int(max(numpoints*self.fstoich, 5))
        fvals = np.append(np.linspace(0, self.fstoich, int(max(numpoints*self.fstoich, 5))), 
                          np.linspace(self.fstoich, 1, int(max(numpoints*(1-self.fstoich), 5))))
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

        
        self.X_reac_stoich = self._Yreac(self.fstoich)[self.reac]*self._MWmix(self._Yreac(self.fstoich))/self.MW[self.reac]
        self.sigma = ((self._MWmix(self._Yreac(self.fstoich))/Treac) /
                      (self._MWmix(self._Yprod(self.fstoich))/self.T_prod(self.fstoich)))
        cp, cv = PropsSI(['CPMASS', 'CVMASS'], 'T', Treac, 'P', P, reac)
        self.gamma_reac = cp/cv
        if verbose:
            print('done.')

    def reinitilize(self, fluid, numpoints = 100):
        '''
        Reinitilizes class to new temperature, pressure, etc.  Can be used rather 
        than creating a new instance taking up additional memory.'''
        self.__init__(fluid, numpoints)

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
        s = sum(list(Y.values()))
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
        s = sum(list(Y.values()))
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
