"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import warnings

from CoolProp import CoolProp
import numpy as np
from scipy import optimize, interpolate
from scipy import constants as const

from ._fuel_props import FuelProperties
from ..utilities.custom_warnings import PhysicsWarning



class CoolPropWrapper:
    def __init__(self, species):
        '''
        Class that uses CoolProp for equation of state calculations.
        
        phase: 'gas' or 'liquid' for fluid at saturated vapor pressure
        '''
        self._cp = CoolProp
        self.spec = species
        self.MW = self._cp.PropsSI(self.spec, 'molemass')        
    
    def _init_fluid_params(self, params):
        '''
        Initializes Fluid properties.
        Calculates 2 remaining properties given 2 of rho, pressure, temperature, or phase
        
        Parameters
        ----------
        params : dict
            Dict of Fluid parameters keyed by the labels 'D', 'P', 'T', and 'phase'
            Valid input requires 2 of the parameters to be defined and 2 to be None

        Returns
        -------
        params : dict
            Returns the same dict with all values filled in
        '''
        # Set input and output values
        output = [key for key, val in params.items() if val is None]
        input_dict = {key:val for key, val in params.items() if val is not None}

        if len(output) != 2 or len(input_dict) != 2:
            raise ValueError("Fluid not properly defined - too many or too few fluid initialization variables")

        # Put keys and values in seperate tuples
        input_params, input_values = zip(*input_dict.items())

        if 'phase' in input_params:
            self.phase = input_dict.get('phase')
            # Convert phase string to Q value
            input_dict.pop('phase')
            input_dict.update({'Q':{'gas':1, 'liquid':0}[self.phase]})
            props = self.PropsSI(output, **input_dict)
        else:
            # Change phase output to Q for CoolProp to handle
            idx = output.index('phase')
            output[idx] = 'Q'

            # Handle special case for calculating density at saturation
            if 'D' in output and 'Q' in output:
                try: 
                    props = self.PropsSI(output, **input_dict)
                except ValueError:
                    print('exception')
                    warnings.warn('Assuming gas phase to calculate density.', category=PhysicsWarning)
                    self.phase = 'gas'
                    rho = self._cp.PropsSI('D', input_params[0]+self.phase, input_values[0], input_params[1], input_values[1], self.spec)
                    props = [rho, self.phase]
            else:
                props = self.PropsSI(output, **input_dict)

            try:
                if '&' in self.spec: 
                    raise ValueError
                self.phase = self._cp.PhaseSI(input_params[0], input_values[0], input_params[1], input_values[1], self.spec)
            except ValueError:
                Q = props[idx]
                if (Q < 1) and (Q > 0):
                    self.phase = 'twophase'
                elif Q == 1:
                    self.phase = 'vapor'
                elif Q == 0:
                    self.phase = 'liquid'
                else:
                    self.phase = ''

        # Put calculated values into the params dict
        params[output[0]] = props[0]
        params[output[1]] = props[1]

        # Overwrite phase info with string
        params['phase'] = self.phase
        return params
    
    def get_property(self, output, **kwargs):
        '''
        General method for calculating one or more fluid properties

        Parameters
        ----------
        output : string or list of strings
            Same output parameters accepted by CoolProp.PropsSI (e.g., T, P, S, D) with additional custom keywords
                - 'H0' calculates total enthalpy using 'P', 'D', and 'v'

        keyword args : input parameters
            Pair of known parameters entered as individual params (e.g., T=273, P=1e6, etc.)
            An extra 3rd parameter can be added to define either 'phase' or 'v'

        Returns
        -------
        props : float or list of floats
            Return values based on the 'output' parameter

        Examples
        --------
        total_enthalpy = get_property('H0', P=fluid.P, D=rho, v=v)
        rho = get_property('D', T=T, P=P, phase=liquid)
        rho = get_property('D', T=T, P=P)
        s = get_property('S', D=fluid.rho, T = fluid.T)
        h, rho = get_property(['H', 'D'], P=P, S=s0)
        '''
        input_dict = {key:val for key, val in kwargs.items() if val is not None}

        # H0 keyword
        if output == 'H0':
            try:
                v = input_dict.pop('v')
                h = self.PropsSI('H', **input_dict)
            except Exception as err:
                if self.phase == '':
                    raise ValueError(f'Unable to find enthalpy of fluid with {input_dict}')
                warnings.warn(f'Recalculating with specification of phase {self.phase}.')
                h = self.PropsSI('H', phase=self.phase, **input_dict)
            return h + v**2/2

        # General case
        try:
            out = self.PropsSI(output, **input_dict)
        except:
            raise ValueError(f'Unable to calculate {output} using input parameters {input_dict}')
        return out

    def PropsSI(self, output, **kwargs):
        '''
        Wrapper on CoolProps PropsSI
        
        Parameters 
        ----------
        output : list of strings
            Same parameters accepted by CoolProp.PropsSI (e.g., T, P, S, D)
        
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

            try:
                out = self._cp.PropsSI(output, k1, v1, k2, v2, self.spec)
            except:
                warnings.warn('PropsSI could not find a solution - estimating', category=PhysicsWarning)

                # Estimate using the first input property
                for i in range(0, 5000):
                    try:
                        result_high = self._cp.PropsSI(output, k1, v1+i, k2, v2, self.spec)
                        result_low = self._cp.PropsSI(output, k1, v1-i, k2, v2, self.spec)
                        break
                    except:
                        continue

                if len(output) == 1:
                    out = (result_high + result_low) / 2
                else:
                    out = []
                    for idx in range(len(output)):
                        high = result_high[idx]
                        low = result_low[idx]
                        out.append((high + low) / 2)
                    
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

        self.Treac, self.Preac, self.rho_reac = fluid.T, fluid.P, fluid.rho
        # TODO: # Tair, P = ambient.T, ambient.P
        if verbose:
            print('initializing chemistry...', end = '')
        self.reac = fluid.species
        self._nC, self._nH, self._nO = fuel_props.nC, fuel_props.nH, fuel_props.nO

        self.DHc = fuel_props.dHc # heat of combustion, J/kg

        self.therm = {spec:CoolPropWrapper(spec) for spec in ['O2', 'N2', 'H2O', 'CO2', self.reac]}
        self.MW = {spec:therm.MW for spec, therm in self.therm.items()}
        self.fstoich = self.MW[self.reac]/(self.MW[self.reac] + self.xO2stoich * (self.MW['O2'] + self.MW['N2']*3.76))
        ifstoich = int(max(numpoints*self.fstoich, 5))
        fvals = np.append(np.linspace(0, self.fstoich, int(max(numpoints*self.fstoich, 5))), 
                          np.linspace(self.fstoich, 1, int(max(numpoints*(1-self.fstoich), 5))))
        T = self._T_combustion(self.Treac, fvals)
        MWvals = self._MWmix(self._Yprod(fvals))

        # Creates some interpolating functions - only create them once during initialization to save computational time later
        self.MW_prod = lambda f: self._MWmix(self._Yprod(f))
        self.T_prod = interpolate.interp1d(fvals, T)
        self.rho_prod = lambda f: self.Preac*self.MW_prod(f)/(const.R*self.T_prod(f))
        self.drhodf = interpolate.interp1d(fvals,
                                           self.Preac/(const.R*T)*(np.append(np.gradient(MWvals[:ifstoich], fvals[:ifstoich]),
                                                                   np.gradient(MWvals[ifstoich:], fvals[ifstoich:])) - 
                                                                   np.append(np.gradient(T[:ifstoich], fvals[:ifstoich]),
                                                                   np.gradient(T[ifstoich:], fvals[ifstoich:]))/T*MWvals))
        self.absorption_coeff = self._plank_mean_absorption_coefficient_stoich()
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
        for spec in self.therm.keys():
            T = np.linspace(np.max([Tmin, self.therm[spec]._cp.PropsSI('T_min', spec)+0.1]), Tmax, npoints)
            try:
                Hdict[spec] = interpolate.interp1d(T, self.therm[spec].get_property('H', T = T, P = self.Preac), 
                                                   fill_value = 'extrapolate')
            except:
                Hdict[spec] = interpolate.interp1d(T, [self.therm[spec].get_property('H', T = Tval, P = self.Preac) for Tval in T], 
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
        
    def _mean_absorption_coefficient(self, species, temperature, pressure=const.atm):
        '''
        Calculates mean absorption coefficient m^-1 for a species at a temperature and pressure

        Parameters
        ----------
        species: string
            species of interest (either H2O or CO2)
        temperaure: float
            temperature (K)
        pressure: float
            pressure (Pa)

        Returns
        -------
        mean_absorption_coefficient: float
            mean absorption coefficient (m)^-1
            
        Note: Formulas and data from Chmielewski and Gieras, "Planck Mean Absorption Coefficients of H2O, CO2, CO and NO 
        for radiation numerical modeling in combusting flows," Journal of Power Technogies 95 (2) 2015: 97-104. 
        Calculations agree with https://doi.org/10.1016/S0022-4073(01)00178-9
        '''
        data = {'H2O':[[63.87, 149.5, 174.3],
                       [2.629, 493.6, 111.1],
                       [222.9, -2110, 1592],
                       [0.991, 1700, 619.2]],
                'CO2':[[34.57, 45.32, 838.4],
                       [1.22, 930.2, 184.9],
                       [-22.32, 323.9, 217.9],
                       [17.14, 92.98, 1657]]}
        
        coeffs = data[species]
        ap_mean = sum([coeff[0] * np.exp(-((temperature - coeff[1]) / coeff[2]) ** 2)
                       for coeff in coeffs])
        return ap_mean * pressure / const.bar
        
    def _plank_mean_absorption_coefficient(self, temperature, x_H2O, x_CO2, pressure=const.atm):
        '''
        Calculates Plank mean absorption coefficient m^-1 for a mixture of water vapor and CO2

        Parameters
        ----------
        temperaure: float
            temperature (K)
        x_H2O: float
            mole fraction water
        x_CO2: float
            mole fraction CO2
        pressure: float
            pressure (Pa)

        Returns
        -------
        plank_mean_absorption_coefficient: float
            Plank mean absorption coefficient (m)^-1
        '''
        return sum([x * self._mean_absorption_coefficient(spec, temperature, pressure) 
                    for x, spec in zip([x_H2O, x_CO2], ['H2O', 'CO2'])])
                    
    def _plank_mean_absorption_coefficient_stoich(self):
        '''
        Calculates Plank mean absorption coefficient at stiochiometric combustion conditions - used in radiation calculations
        
        Returns
        -------
        plank_mean_absorption_coefficient: float
            Plank mean absorption coefficient (m)^-1 at stoichiometric conditions
        '''
        Y = self._Yprod(self.fstoich)
        x_H2O = Y['H2O'] / self.MW['H2O'] * self._MWmix(Y)
        x_CO2 = Y['CO2'] / self.MW['CO2'] * self._MWmix(Y)
        T = self.T_prod(self.fstoich)
        return self._plank_mean_absorption_coefficient(T, x_H2O, x_CO2, self.Preac)
        
    def dP_expansion(self, enclosure, mass):
        '''
        Pressure due to the expansion of gas from combustion in an enclosure
        
        Parameters
        ----------
        enclosure: object 
            object that contains information about the enclosure including the volume
        mass : float
           mass of combustible gas in enclosure

        Returns
        -------
        P : float
           pressure upon expansion
        '''
        Vol_total = enclosure.V
        Vol_gas   = mass/self.rho_reac
        
        X_reac_stoich = self._Yreac(self.fstoich)[self.reac]*self._MWmix(self._Yreac(self.fstoich))/self.MW[self.reac]
        sigma = ((self._MWmix(self._Yreac(self.fstoich))/self.Treac) /
                 (self._MWmix(self._Yprod(self.fstoich))/self.T_prod(self.fstoich)))
        cp, cv = self.therm[self.reac].get_property(['CPMASS', 'CVMASS'], T = self.Treac, P = self.Preac)
        try:
            gamma = cp/cv
        except:
            gamma = np.nan
        
        VolStoich = Vol_gas/X_reac_stoich

        deltaP  = self.Preac*((((Vol_total+Vol_gas)/Vol_total)*((Vol_total+VolStoich*(sigma-1))/Vol_total))**gamma-1)
        return deltaP
