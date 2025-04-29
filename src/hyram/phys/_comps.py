"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import copy
import warnings
import time

import numpy as np
import matplotlib.pyplot as plt
from scipy import integrate, optimize

from ._therm import CoolPropWrapper
from ..utilities.custom_warnings import PhysicsWarning
from ..utilities import misc_utils



class Fluid:
    def __init__(self, species=None, T=None, P=None, rho=None, phase=None, v=0, therm=None):
        """
        class used to describe a fluid (usually a gas)
        two of four (T, P, rho, phase) are needed to fully define the fluid

        Parameters
        ----------
        species: string or dictionary
            species (either formula or name - see CoolProp documentation) or dictionary of {species:molefraction}
        T : float
            temperature (K)
        P: float
            pressure (Pa)
        rho: float
            density (kg/m^3)
        v: float
            velocity (m/s)
        phase: {None, 'gas', 'liquid'}
            either 'gas' or 'liquid' if fluid is at the saturated state.
        therm : thermodynamic class
            a thermodynamic class that is used to relate state variables

        """
        if misc_utils.is_parsed_blend_string(species):
            self.species = species

        else:
            if type(species) == str:
                cleaned = misc_utils.parse_fluid_key(species)
                if misc_utils.is_valid_fluid_string(cleaned):
                    self.species = cleaned
                else:
                    raise ValueError(f"'{cleaned}' is not a valid fluid name")

            elif type(species) == dict:
                single_species = len(species) == 1
                # Rebuild dict to parse and validate fluid names
                species = misc_utils.parse_blend_dict_into_coolprop_dict(species)
                self.species = misc_utils.parse_coolprop_dict_into_string(species)
                if single_species:  # e.g. Hydrogen[1.00] -> Hydrogen
                    self.species = self.species.split('[')[0]

        if therm is None:
            therm = CoolPropWrapper(self.species)

        params = therm._init_fluid_params({'D':rho, 'P':P, 'T':T, 'phase':phase})

        self.rho, self.P, self.T, self.phase = params['D'], params['P'], params['T'], params['phase']
        self.therm = therm
        self.v = v

    def update(self, T=None, P=None, rho=None, v=None):
        if v != None:
            self.v = v

        if T is not None or P is not None or rho is not None:
            params = self.therm._init_fluid_params({'D':rho, 'P':P, 'T':T, 'phase':None})
            self.rho = params['D']
            self.P = params['P']
            self.T = params['T']
            self.phase = params['phase']

    def __repr__(self):
        return 'Gas\n%s\n  P = %.3f bar\n  T = %0.1f K\n  rho = %.3f kg/m^3)\n  v = %.1f m/s' % (
            30 * '-', self.P * 1e-5, self.T, self.rho, self.v)


class Orifice:
    def __init__(self, d, Cd=1):
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
        self.d, self.Cd, self.A = d, Cd, np.pi / 4 * d ** 2

    def __repr__(self):
        return 'orifice\n%s\ndiameter = %.2f mm\ndischarge coefficient = %.2f' % (30 * '-', self.d * 1e3, self.Cd)

class NozzleFlow:
    def __init__(self, upstream_fluid, orifice, downstream_P, mdot = None, suppressWarnings = True):
        '''
        Class used to calculate isentropic flow through an orifice
        
        Parameters
        ----------
        upstream_fluid - fluid object upstream of orifice
        orifice - Orifice object through which upstream fluid flows
        downstream_P - downstream pressure (Pa)
        mdot - mass flow rate (if fluid is unchoked, it is recommended to provide) [kg/s]
        suppressWarning - boolean whether to suppress warnings or not
        
        Returns
        -------
        fluid - fluid at the nozzle with state (pressure, temperature, density) and velocity
        '''
        self.upstream_fluid, self._orifice, self.downstream_P = upstream_fluid, orifice, downstream_P
        h0 = upstream_fluid.therm.get_property('H0', P=upstream_fluid.P, D=upstream_fluid.rho, v=upstream_fluid.v)
        if upstream_fluid.v > 0:
            s0 = upstream_fluid.therm.get_property('S', H = h0, D = np.round(upstream_fluid.rho, 12))
        else: #LH2 simulations were giving weird results when calculating entropy from enthalpy
            s0 = upstream_fluid.therm.get_property('S', D = upstream_fluid.rho, T = upstream_fluid.T)

        fluid = copy.copy(upstream_fluid)

        if fluid.P < downstream_P:
            raise ValueError('Downstream pressure is higher than upstream pressure.  Nonphysical.')
        if fluid.P == downstream_P:
            if mdot is not None:
                v = mdot/(orifice.Cd*fluid.rho*orifice.A)
                fluid.update(v=v)
                self.choked = False
                self.fluid = fluid
                return
            else:
                raise ValueError('Downstream pressure is the same as upstream pressure.  Need to specify mass flow rate (mdot).')

        def negflux(P):
            h, rho = fluid.therm.get_property(['H', 'D'], P=P, S=s0)
            return -rho*np.sqrt(2 * (h0 - h))

        P = optimize.minimize_scalar(negflux, bounds = (downstream_P, upstream_fluid.P), method = 'bounded')['x']
        h, rho = fluid.therm.get_property(['H', 'D'], P = P, S = s0)
        fluid.update(rho = rho, P = P, v = np.sqrt(2 * (h0 - h)))
        if P - downstream_P > .01:
            if mdot is not None:
                if not suppressWarnings:
                    warnings.warn('Fluid choked. Ignoring mdot specification and using choked flow calculation.', category=PhysicsWarning)
            self.choked = True
        else: 
            self.choked = False
            if mdot is None:
                if not suppressWarnings:
                    warnings.warn('Fluid unchoked. Provide a mass flow rate.', category=PhysicsWarning)
            else:
                v = mdot/(orifice.Cd*rho*orifice.A)
                fluid.update(rho=rho, P=(P*(P - downstream_P > .01) + 
                                         downstream_P*(P - downstream_P <= .01)), v=v)
        self.fluid = fluid

    @property
    def orifice(self):
        return self._orifice

    @orifice.setter
    def orifice(self, new_orifice):
        if self.choked: #choked - P, rho, T, v remain constant at orifice, only ._orifice needs to be updated so mass flow rate is properly calculated
            self._orifice = new_orifice
        else: #unchoked - previously specified mass flow rate through orifice assumed constant; fluid velocity must also be updated
            self.fluid.v = self.mdot/(self.fluid.rho*new_orifice.Cd*new_orifice.A)
            self._orifice = new_orifice
    
    @property
    def mdot(self):
        '''
        mass flow rate through the nozzle
        
        Returns
        -------
        mdot - mass flow rate (kg/s)
        '''
        return self.fluid.rho * self.fluid.v * self.orifice.A * self.orifice.Cd

class Source(object):
    """
    Used to describe a source (tank) that contains a fluid

    Attributes
    ----------
    mass : float
        mass of source (kg)

    """

    def __init__(self, V, fluid):
        '''
        Initializes source based on the volume and the fluid object in the source (tank)

        Parameters
        ----------
        V: float
            volume of source (tank) (m^3)
        fluid : Fluid
            fluid object in the source (tank)

        Returns
        -------
        source: object
            object containing .fluid (fluid obejct), .V (volume, m^3), and .m (mass (kg))
        '''
        self.fluid = fluid
        self.V = V
        self.m = self.mass = fluid.rho * V

    @classmethod
    def fromMass(cls, m, fluid):
        '''
        Initilization method based on the mass and the fluid object in the source (tank)

        Parameters
        ----------
        m: float
            mass of source (tank) (kg)
        fluid: object
            fluid object in the source (tank)

        Returns
        -------
        source: object
            object containing .fluid (fluid obejct), .V (volume, m^3), and .m (mass (kg))
        '''
        V = m / fluid.rho
        return cls(V, fluid)

    @classmethod
    def fromMass_Vol(cls, species, m, V, T=None, P=None):
        '''
        Initilization method based on the mass, volume, and either the temperature or pressure of the fluid
        in the source (tank).

        Parameters
        ----------
        species: string
            species (either formula or name - see CoolProp documentation)
        m: float
            mass of source (tank) (kg)
        V: float
            volume of source (tank) (m^3)
        therm: object
            thermodynamic class used to relate pressure, temperature and density
        T: float (optional)
            temperature (K)
        P: float (optional)
            pressure (Pa)

        Returns
        -------
        source: object
            object containing .fluid (fluid object), .V (volume, m^3), and .m (mass (kg)).
        returns none if either underspecified (neither T or P given) or overspecified (both T and P given)
        '''
        rho = m / V
        if T is not None and P is None:
            fluid = Fluid(species=species, rho=rho, T=T)
        elif T is None and P is not None:
            fluid = Fluid(species=species, rho=rho, P=P)
        else:
            return None
        return cls(V, fluid)

    def _blowdown_gov_eqns(self, t, ind_vars, Vol, orifice, heat_flux, ambient_P, interp):
        '''
        governing equations for energy balance on a tank (https://doi.org/10.1016/j.ijhydene.2011.12.047)

        Parameters
        ----------
        t - time (s)
        ind_vars - array of mass (kg), internal energy (J/kg) in tank
        Vol - float, volume of tank (m^3)
        orifice - orifice object
        heat_flux - float, heat flow into tank (W)
        interp - dictionary of interpolating functions for isentropic blowdown or None if not isentropic

        Returns
        -------
        [dm_dt, du_dt] = array of [d(mass)/dt (kg/s), d(internal energy)/dt (J/kg-s)]
                       = [-rho_throat*v_throat*A_throat, 1/m*(Q_in + mdot_out*(u - h_out))]
        '''
        therm = self.fluid.therm
        m, U = ind_vars
        m, U = float(m), float(U)
        rho = m / Vol
        if interp is not None:
            def err_U(P):
                return interp['U'](P) - U
            P = optimize.root(err_U, ambient_P)['x'][0]
            T, rho_interp, h = [float(interp[k](P)) for k in ['T', 'D', 'H']]
            fluid = copy.copy(self.fluid)
            fluid.update(T=T, rho=rho)
        else:
            T = therm.get_property('T', U=U, D=np.round(rho, 10))
            fluid = copy.copy(self.fluid)
            fluid.update(T=T, rho=rho)
            h = therm.get_property('H', T=fluid.T, D=fluid.rho)

        if fluid.P < ambient_P:
            return np.array([0, 0])
        throat = NozzleFlow(fluid, orifice, ambient_P)

        dm_dt = -throat.mdot
        du_dt = 1 / m * (heat_flux + (h - U) * dm_dt)
        return np.array([dm_dt, du_dt])

    def empty(self, orifice, ambient_P = 101325,
              heat_flux = 0, nmax = 1000,
              m_empty = 1e-6, p_empty_percent = 0.01,
              t_empty = 3600*24,
              timeout = 600):
        '''
        integrates the governing equations for an energy balance on a tank

        Parameters
        ----------
        orifice : Orifice
            Orifice object through which the source is emptying
        ambient_P : float, optional
            Ambient pressure into which leak occurs (Pa), defaults to 1 atm
        heat_flux : float, optional
            Heat flow (W) into tank.  Assumed to be 0 (adiabatic)
        nmax : int, optional
            Maximum number of iterations, defaults to 1000
        m_empty : float, optional
            Mass when considered empty (kg), defaults to 1e-6
        p_empty_percent : float, optional
            Percent of ambient pressure when considered empty, defaults to 0.01%
        t_empty : int, optional
            maximum blowdown time (sec), defaults to 24 hours
        timeout : int, optional
            maximum computer run time (sec) until the integration terminates, defaults to 600s

        Returns
        -------
        tuple of (mdot, fluid_list, t, solution_array) =
                 (list of mass flow rates (kg/s), list of fluid objects at each time step,
                  array of times (s), 2D array of [mass, internal energy] at each time step)
        '''
        therm = self.fluid.therm
        m0 = self.m
        volume = self.V

        is_isentropic = (heat_flux == 0)
        if is_isentropic and 'interp' not in self.fluid.therm.__dict__:
            therm.make_isentropic_interpolating_functions(self.fluid, ambient_P, self.fluid.P)
        if is_isentropic:
            interp = self.fluid.therm.interp
            u0 = float(interp['U'](self.fluid.P))
        else:
            interp = None
            u0 = therm.get_property('U', T=self.fluid.T, D=self.fluid.rho)

        # Define ending state events
        start_time = time.time()
        n_steps = 0
        ambient_P_threshold = ambient_P * (1 + p_empty_percent/100)

        def fluid_from_m_U(m, U):
            '''fluid in tank, given mass (m) and internal energy (U)'''
            rho = m / volume
            if interp is not None:
                def err_U(P):
                    return interp['U'](P) - U
                P = optimize.root(err_U, ambient_P)['x'][0]
                T, rho_interp, h = [float(interp[k](P)) for k in ['T', 'D', 'H']]
                fluid = copy.copy(self.fluid)
                fluid.update(T=T, rho=rho)
            else:
                T = therm.get_property('T', U=U, D=np.round(rho, 10))
                fluid = copy.copy(self.fluid)
                fluid.update(T=T, rho=rho)
            return fluid

        # Set function attribute 'terminal' to end integration when one of these events is met
        def eventAttr():
            def decorator(f):
                f.terminal = True
                return f
            return decorator

        @eventAttr()
        def time_out(*args):
            current = time.time() - start_time
            return timeout - current

        @eventAttr()
        def max_steps(*args):
            nonlocal n_steps
            n_steps += 1
            return nmax - n_steps

        @eventAttr()
        def residual_mass(*args):
            return m_empty - args[1][0]

        @eventAttr()
        def residual_pressure(*args):
            tank_P = fluid_from_m_U(*args[1]).P
            return tank_P - ambient_P_threshold

        sol = integrate.solve_ivp(self._blowdown_gov_eqns, (0, t_empty), [m0, u0],
                                  method='BDF',
                                  events=(time_out, max_steps, residual_mass, residual_pressure),
                                  args=(volume, orifice, heat_flux, ambient_P, interp))

        if sol.status == 0:
            warnings.warn("Blowdown not complete after time limit")

        times = list(sol['t'])
        tank_sol = sol['y'] # list of [[mass], [internal energy]]
        tank_states = tank_sol.T  # turn into list of tuples [(mass, internal energy)]

        # Generate lists of fluids and mdot values
        fluid_list, mdot = [], []
        for state in tank_states:
            fluid = fluid_from_m_U(*state)
            throat = NozzleFlow(fluid, orifice, ambient_P)
            fluid_list.append(fluid)
            mdot.append(throat.mdot)

        if is_isentropic:
            delattr(self.fluid.therm, 'interp')

        self.mdot, self.fluid_list, self.ts, self.sol = mdot, fluid_list, times, tank_sol
        return mdot, fluid_list, times, tank_sol

    def plot_time_to_empty(self):
        try:
            x = self.sol[0]
        except:
            return None
        fig, axs = plt.subplots(4, 1, sharex=True, squeeze=True, figsize=(4, 7))
        axs[0].plot(self.ts, self.sol[0])
        axs[0].set_ylabel('Mass [kg]')
        axs[1].plot(self.ts, np.array([f.P for f in self.fluid_list]) * 1e-5)
        axs[1].set_ylabel('Pressure [bar]')
        axs[2].plot(self.ts, self.mdot)
        axs[2].set_ylabel('Flow Rate [kg/s]')
        axs[3].plot(self.ts, [f.T for f in self.fluid_list])
        axs[3].set_ylabel('Temperature [K]')
        axs[3].set_xlabel('Time [s]')
        [a.minorticks_on() for a in axs]
        [a.grid(which='major', color='k', dashes=(2, 2), alpha=.5) for a in axs]
        [a.grid(which='minor', color='k', alpha=.1) for a in axs]
        return fig

class Enclosure:
    '''
    Enclosure used in the overpressure modeling
    '''

    def __init__(self, H, A, H_release, ceiling_vent, floor_vent, Xwall=np.inf):
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
        self.V = H * A


class Vent:
    '''
    Vent used in overpressure modeling
    '''

    def __init__(self, A, H, Cd=1, vol_flow_rate=0):
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
        self.Qw = Cd * vol_flow_rate / np.sqrt(2)  # See Lowesmith et al IJHE 2009
