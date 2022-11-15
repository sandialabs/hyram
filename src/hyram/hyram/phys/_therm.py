"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import warnings
import logging

from CoolProp import CoolProp
import numpy as np
from scipy import optimize, interpolate
from scipy import constants as const

from ._fuel_props import FuelProperties
from ..utilities.custom_warnings import PhysicsWarning


log = logging.getLogger(__name__)


class CoolPropWrapper:
    def __init__(self, species):
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
            if '&' in self.spec:
                raise ValueError

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
            if '&' in self.spec: raise ValueError
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
        returns the density given the temperature and pressure - if at saturation conditions,
        requires phase to already be set
        
        Parameters
        ----------
        T: float
            temperature (K)
        P: float
            pressure (Pa)
        
        Returns
        -------
        rho:
            density (kg/m^3)
        '''
        try:
            rho, Q = self.PropsSI(['D', 'Q'], T = T, P = P)
            try:
                if '&' in self.spec: raise ValueError
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
            print('exception')
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
            temperature (K)
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
            temperature (K)
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
            temperature (K)
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
        
    def _total_enthalpy(self, P, rho, v):
        '''
        total enthalpy (J/kg) for a fluid at a given pressure, density and velocity
        '''
        try:
            h = self.PropsSI('H', P = P, D = rho)
        except:
            if self.phase == '':
                raise warnings.warn(f'Unable to find enthalpy of fluid with P = {P} and rho = {rho}', category=PhysicsWarning)
                return None
            raise warnings.warn(f'exception in enthalpy calculation with P = {P} and rho = {rho} - recalculating with specificaiton of phase = {self.phase}')
            h = self.PropsSI('H', **{'P|'+self.phase: P, 'D':rho})
        return h + v**2/2

    def _X(self, Y, other = 'air'):
        MW = self._cp.PropsSI('M', self.spec)
        MW_other = self._cp.PropsSI('M', other)
        return Y/MW/(Y/MW + (1-Y)/MW_other)
    
    def a(self, T = None, P = None, S = None):
        '''
        returns the speed of sound given the temperature and entropy or pressure and entropy
          note: 2-phase calculations no longer supported by this method see git history if
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
            phase =  '|' + kwargs.pop('phase')
        else:
            phase = ''        
        try:
            if '&' in self.spec:
                raise ValueError

            (k1, v1), (k2, v2) = kwargs.items()
            k1 += phase
            if type(v1) == list or type(v1) == np.ndarray: 
                if len(v1) == 1:
                    v1 = float(v1)
            if type(v2) == list or type(v2) == np.ndarray:
                if len(v2) == 1:
                    v2 = float(v2)
            out = self._cp.PropsSI(output, k1, v1, k2, v2, self.spec)
            return out

        except ValueError:  # likely blend
            if '&' not in self.spec:
                return ValueError  # not a blend - some other issue

            spec_names = '&'.join([s.split('[')[0] for s in self.spec.split('&')])
            molefracs = [float(s.split('[')[1][:-1]) for s in self.spec.split('&')]
            eos = self._cp.AbstractState('HEOS', spec_names)
            eos.set_mole_fractions(molefracs)
            guess = self._cp.PyGuessesStructure()

            def out(k):
                if   k == 'D': return eos.rhomass()
                elif k == 'S': return eos.smass()
                elif k == 'H': return eos.hmass()
                elif k == 'U': return eos.umass()
            if ('P' in kwargs) and ('T' in kwargs):
                P = kwargs['P']; T = kwargs['T']
                try:
                    eos.update(self._cp.PT_INPUTS, P, T)
                except:
                    for i in range(10):
                        guess.rhomolar = (i+1)*2*P/(8.314*T) # unclear what the 'right' guess is (hence the loop)
                        try:
                            eos.update_with_guesses(self._cp.PT_INPUTS, P, T, guess)
                            break
                        except:
                            pass
            elif ('T' in kwargs):
                T = kwargs.pop('T')
                k, v = list(kwargs.items())[0]
                def err(P):
                    try:
                        eos.update(self._cp.PT_INPUTS, P, T)
                    except:
                        for i in range(10):
                            guess.rhomolar = (i+1)*2*P/(8.314*T) # unclear what the 'right' guess is (hence the loop)
                            try:
                                eos.update_with_guesses(self._cp.PT_INPUTS, P, T, guess)
                                break
                            except:
                                pass
                    return v - out(k)
                P = optimize.root(err, 101325.)['x'] # somewhat arbitrarily starts at 1 atm
            elif ('P' in kwargs):
                P = kwargs.pop('P')
                k, v = list(kwargs.items())[0]
                def err(T):
                    try:
                        eos.update(self._cp.PT_INPUTS, P, T)
                    except:
                        for i in range(10):
                            guess.rhomolar = (2*i+1)*P/(8.314*T) # unclear what the 'right' guess is (hence the loop)
                            try:
                                eos.update_with_guesses(self._cp.PT_INPUTS, P, T, guess)
                                break
                            except:
                                pass

                    return v - out(k)
                T = optimize.brentq(err, eos.Tmin(), eos.Tmax()) # not the most efficient here, but works
            else:
                (k1, v1), (k2, v2) = list(kwargs.items())
                warnings.warn('Only PT_inputs allowed for blends (this may not work) trying to solve for %s, %s inputs' % (k1, k2), 
                              category=PhysicsWarning)
                def err(TP):
                    T, P = TP
                    try:
                        eos.update(self._cp.PT_INPUTS, P, T)
                    except:
                        for i in range(10):
                            guess.rhomolar = (i+1)*2*P/(8.314*T) # unclear what the 'right' guess is (hence the loop)
                            try:
                                eos.update_with_guesses(self._cp.PT_INPUTS, P, T, guess)
                                break
                            except:
                                pass
                    return [v1 - out(k1), v2 - out(k2)]
                T, P = optimize.root(err, [(eos.Tmin() + eos.Tmax())/2, 101325])['x']              

            def map_outputs(key_or_keys, eos):
                def out(k):
                    if   k == 'D': return eos.rhomass()
                    elif k == 'T': return eos.T()
                    elif k == 'P': return eos.p()
                    elif k == 'S': return eos.smass()
                    elif k == 'H': return eos.hmass()
                    elif k == 'U': return eos.umass()
                    elif k == 'Q': return eos.Q()
                    elif k == 'A': return eos.speed_sound()
                    elif k == 'C': return eos.cpmass()
                    elif k == 'V': return eos.viscosity()
                    elif k == 'isobaric_expansion_coefficient': return eos.isobaric_expansion_coefficient()
                if type(key_or_keys) == list:
                    return [out(k) for k in key_or_keys]
                else:
                    return out(key_or_keys)
            return map_outputs(output, eos)
        except:
            raise warnings.warn('system not properly defined')

            
    def s(self, T = None, P = None, rho = None, phase = None):
        '''
        entropy (J/kg-K) of a fluid at temperature T (K) and pressure P (Pa)
        
        Parameters
        ----------
        T: float
            temperature (K)
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
        fuel_props = FuelProperties(fluid.species)
        self.xO2stoich = fuel_props.nC + fuel_props.nH/4 - fuel_props.nO/2

        Treac, P = fluid.T, fluid.P
        # TODO: # Tair, P = ambient.T, ambient.P
        if verbose:
            print('initializing chemistry...', end = '')
        self._myPropsSI = fluid.therm.PropsSI
        self._PropsSI = fluid.therm._cp.PropsSI
        #reac = ('C%dH%d' % (nC, 2*nC+2)).replace('C0', '').replace('C1', 'C')
        #self.reac = reac 
        self.reac = fluid.species
        self._nC, self._nH, self._nO = fuel_props.nC, fuel_props.nH, fuel_props.nO

        self.DHc = fuel_props.dHc # heat of combustion, J/kg

        self.Treac, self.P = Treac, P

        MW = dict([[spec, self._PropsSI('M', spec)] for spec in ['O2', 'N2', 'H2O', 'CO2']])
        MW[self.reac] = fluid.therm.MW
        self.MW = MW
        self.fstoich = MW[self.reac]/(MW[self.reac] + self.xO2stoich * (MW['O2'] + MW['N2']*3.76))
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
        cp, cv = self._PropsSI(['CPMASS', 'CVMASS'], 'T', Treac, 'P', P, self.reac)
        self.gamma_reac = cp/cv
        if verbose:
            print('done.')

    def reinitilize(self, fluid, numpoints = 100):
        '''
        Reinitializes class to new temperature, pressure, etc.  Can be used rather
        than creating a new instance taking up additional memory.'''
        self.__init__(fluid, numpoints)

    def _MWmix(self, Y):
        '''returns the mixture averaged molecular weight, given a mass fraction'''
        MWmix = 0
        for spec, Yval in Y.items():
            MWmix += Yval/self.MW[spec]
        MWmix = 1/MWmix
        return MWmix
   
    def _eta(self, f):
        '''returns eta, given a mixture fraction'''
        MW = self.MW
        eta = 1/self.xO2stoich*(MW[self.reac]/(1.e-99*np.ones_like(f) +f) - MW[self.reac])/(MW['O2'] + 3.76*MW['N2'])
        return eta
    
    def _Yprod(self, f):
        '''
        the mass fractions of combustion products as a function of the mixture fraction
        todo: needs to be fixed for when CO2 (?and N2?) is in the reactants

        Parameters
        ----------
        f = mixture fraction

        Returns
        -------
        Y - dictionary of mass fractions (kg/kg)
        '''
        eta = self._eta(f)
        nCreac, nHreac, nOreac = self._nC, self._nH, self._nO
        Y = {self.reac:(1-eta)*(eta < 1)*self.MW[self.reac],
             'CO2':nCreac*(eta*(eta <= 1) + (eta > 1))*self.MW['CO2'],
             'H2O':nHreac/2*(eta*(eta <= 1) + (eta > 1))*self.MW['H2O'],
             'O2':(eta-1)*self.xO2stoich*(eta > 1)*self.MW['O2'],
             'N2':eta*self.xO2stoich*3.76*self.MW['N2']}
        s = sum(list(Y.values()))
        for k in Y.keys():
            Y[k] /= s
        return Y
    
    def _Yreac(self, f):
        '''
        the mass fractions of combustion reactants as a function of the mixture fraction
        (assumes that there is no H2O or CO2 as a reactant)
        '''
        Y = {self.reac: self.MW[self.reac], 
             'CO2':     0,
             'H2O':     0,
             'O2':      self._eta(f)*self.xO2stoich*self.MW['O2'],
             'N2':      self._eta(f)*self.xO2stoich*3.76*self.MW['N2']}
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
            T = np.linspace(np.max([Tmin, self._PropsSI('T_min', spec)+0.1]), Tmax, npoints)
            if spec == self.reac:
                Hdict[spec] = interpolate.interp1d(T, [self._myPropsSI('H', T = Tval, P = self.P) for Tval in T], 
                                                   fill_value = 'extrapolate')
            else:
                Hdict[spec] = interpolate.interp1d(T, self._PropsSI('H', 'T', T, 'P', self.P, spec), 
                                                   fill_value = 'extrapolate')
        return Hdict

    def _T_combustion(self, T_reac, f, numpoints = 500):
        '''combustion temperature (K)'''
        DHc = self.DHc*self._Yprod(f)['H2O']/(self._nH/2)*self.MW[self.reac]/self.MW['H2O'] # heat of combustion [J/kg_total_products]
        Hdict = self._Hdict(T_reac, npoints = numpoints)
        H0 = self._H(T_reac, self._Yreac(f), Hdict)
        H0 *= self._MWmix(self._Yreac(f))/self._MWmix(self._Yprod(f)) #J/kg
        H = H0 + DHc
        T = optimize.root(lambda T: self._H(T, self._Yprod(f), Hdict) - H, T_reac*np.ones_like(np.array(f)))['x']
        return T
