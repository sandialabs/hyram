"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
import warnings

import matplotlib as mpl
import matplotlib.pyplot as plt
import numpy as np
import scipy.constants as const
from matplotlib.collections import LineCollection
from scipy import interpolate

from ._fuel_props import FuelProperties
from ._comps import Fluid, Orifice
from ._layer import LayeringJet
from ._therm import Combustion
from ..utilities import misc_utils
from ..utilities.custom_warnings import PhysicsWarning

log = logging.getLogger(__name__)


########################################################################
# TODO: untested for alternate fuels
########################################################################
class IndoorRelease:
    '''
    Class used to calculate physics of an indoor release
    '''
    def __init__(self, source, orifice, ambient, enclosure, tmax=None,
                 heat_flux=0, nmax=1000, m_empty=1e-6, p_empty_percent=1e-2,
                 release_area=None, theta0=0, x0=0, y0=0,
                 nn_conserve_momentum=True, nn_T='solve_energy',
                 lam=1.16, X_lean=None, X_rich=None, tol=1e-5,
                 max_steps=1e5, steady=False, nsteady=5,
                 verbose=False):
        '''
        Initialization for an indoor release
        
        Parameters
        ----------
        source : class
            source for fluid (must contain empty function)
        orifice : class
            orifice through which source is flowing
        ambient : class
            fluid initially contained in enclosure
        enclosure : class
            enclosure into which release is occurring
        tmax : float (optional)
            total time for simulation (s), unless blowdown not complete
        heat_flux: float (optional)
            Heat flow (W) into source tank.  Assumed to be 0 (adiabatic)
        nmax: int (optional)
            maximum number of iterations for blowdown integration
        m_empty: float (optional)
            mass when source considered empty (kg)
        p_empty_percent: float (optional)
            percent of ambient pressure when source considered empty
        release_area : float or None (optional)
            secondary containment area for release (m^2)
        theta0 : float (optional)
            angle of release (rad, 0 is horizontal, pi/2 is vertical)
            Default is 0
        x0 : float (optional)
            x-coordinate of release (m), default is 0
        y0 : float (optional)
            y-coordinate of release (m), default is 0
        lam : float (optional)
            lambda value, default is 1.16
        X_lean : float (optional)
            molar lower flammability limit,
            default is None: will use default LFL for fuel
        X_rich : float (optional)
            molar upper flammability limit,
            default is None: will use default UFL for fuel
        tol : float (optional)
            Tolerance for h2_jet integration, default is 1e-5
        max_steps : integer (optional)
            Maximum steps along the S axis for h2_jet integration
            Default is 1e5
        steady : Boolean (optional)
            Option for a blowdown release or a steady-flowrate release
            Default is False, for a blowdown
        nsteady: integer (optional)
            number of time divisions to get to steady state
        '''
        if X_lean is None:
            fuel_props = FuelProperties(source.fluid.species)
            X_lean = fuel_props.LFL
        if X_rich is None:
            fuel_props = FuelProperties(source.fluid.species)
            X_rich = fuel_props.UFL

        params = locals()
        log.info(misc_utils.params_as_str(params))

        if verbose:
            print('Performing indoor release calculations...')
        
        # Calculate time steps and mass flow history
        if steady:
            # Single jet/plume for steady release, so single time step
            if tmax is None:
                tmax = 30 # this is an arbitrary number - should be long enough to ensure steady-state has been reached
            ts = np.linspace(0, tmax, nsteady) 
            steady_mdot = orifice.mdot(orifice.flow(source.fluid, ambient.P))
            mdots = np.ones(len(ts)) * steady_mdot
            gas_list = [source.fluid for i in range(len(ts))]
        else:
            # Different jet/plume at each time step for blowdown
            mdots, gas_list, ts, _ = source.empty(orifice, ambient.P,
                                                  heat_flux, nmax, m_empty, p_empty_percent)
            gas_list = gas_list.copy()
            if tmax is not None:
                if tmax > ts[-1]:
                    ts = np.append(ts, tmax)
                    gas_list.append(Fluid(species = source.fluid.species, T = ambient.T, P = ambient.P))
                    mdots = np.append(mdots, 1e-10)
                else:
                    i = np.argmax(np.array(ts) > tmax)+1
                    ts = ts[:i]
                    gas_list = gas_list[:i]
                    mdots = mdots[:i]
        # Source fluid at ambient conditions
        gas = Fluid(species = source.fluid.species, T = ambient.T, P = ambient.P)
        self.comb = Combustion(gas)
        self.enclosure = enclosure
        
        if release_area is not None:
            if release_area > orifice.A:
                # switch gas to ambient pressure and change orifice size for release
                gas_list = len(gas_list)*[gas]
                orifice = Orifice(np.sqrt(release_area*4/const.pi))
            else:
                warnings.warn('Secondary containment release area must be bigger than orifice area - assuming single orifice.',
                              category=PhysicsWarning)
        jets = []
        LIM = enclosure.Xwall + enclosure.H # Limit of maximum distance jet can extend
        Ymin = X_lean*gas.therm.MW/(X_lean*gas.therm.MW + (1-X_lean)*ambient.therm.MW)
        for g, mdot in zip(gas_list, mdots):
            jets.append(LayeringJet(g, orifice, ambient, theta0 = theta0, y0 = y0,
                                    nn_conserve_momentum=nn_conserve_momentum, nn_T=nn_T,
                                    x0 = x0, lam = lam, mdot = mdot, Smax = LIM, Ymin = Ymin,
                                    max_steps = max_steps, tol = tol, suppressWarnings = True, verbose = verbose))
            jets[-1].Q_jet = mdot/gas.rho # needed for layer model
        # Reshape jet if needed
        [jet.reshape(enclosure, showPlot=False) for jet in jets]

        # Initial 'layer' is between vent and ceiling
        vol_layer = (enclosure.H - enclosure.ceiling_vent.H) * enclosure.A

        # Set initial volume and concentration (0) for layer model
        Vol_conc0 = np.array([vol_layer, 0])

        # Initialize solutions
        x_layer, H_layer, m_jet, m_layer, dP_layer, dP_tot = 6*[np.array([0])]
        Vol_layer, t_layer = 2*[np.array([0])]
        
        jet_mass_last = jets[0].m_flammable(X_lean = X_lean, X_rich = X_rich, Hmax = enclosure.H)
        # Run layer model at each time step for each plume
        for i in range(0, len(jets)-1):
            # Compute volumetric distribution of gas along the ceiling
            t, vol_layer, c = jets[i].layer_accumulation([ts[i], ts[i+1]], Vol_conc0, enclosure)
            # Ensure volume and mole fraction values are realistic
            if np.any(vol_layer < 0): raise ValueError('Layer volume has returned a negative value')
            if np.any(vol_layer > enclosure.V): raise ValueError('Layer volume has exceeded enclosure volume')
            if np.any(c < 0): raise ValueError('Layer concentration has returned a negative value')
            if np.any(c > 1): raise ValueError('Layer concentration has exceeded 100%')
            
            # Calculate flammable mass in layer
            MW_layer = c*gas.therm.MW + (1 - c)*ambient.therm.MW
            rho_layer = ambient.P/(ambient.T*const.R/MW_layer)
            Y_layer = c * gas.therm.MW / MW_layer
            layer_mass = vol_layer*rho_layer*Y_layer*(c >= X_lean)*(c <= X_rich)
            
            # Calculate height of layer
            layer_height = vol_layer / enclosure.A

            # Calculate flammable mass in jet
            jet_mass = jets[i+1].m_flammable(X_lean = X_lean, X_rich = X_rich, Hmax = enclosure.H - layer_height[-1])
            jet_mass_array = np.interp(t, [ts[i], ts[i+1]], [jet_mass_last, jet_mass])
            jet_mass_last = jet_mass

            # Calculate total overpressure
            dP_total = self.dP_expansion(jet_mass_array + layer_mass, gas)

            # Calculate overpressure in layer
            dP_lay = self.dP_expansion(layer_mass, gas)

            # Assign outputs for this plume-timestep
            x_layer = np.append(x_layer, c[1:])
            Vol_layer = np.append(Vol_layer, vol_layer[1:])
            m_layer = np.append(m_layer, layer_mass[1:])
            H_layer = np.append(H_layer, layer_height[1:])
            m_jet = np.append(m_jet, jet_mass_array[1:])
            dP_tot = np.append(dP_tot, dP_total[1:])
            dP_layer = np.append(dP_layer, dP_lay[1:])
            t_layer = np.append(t_layer, t[1:])

            # Update initial volume and concentration for layer model
            Vol_conc0 = np.array([vol_layer[-1], c[-1]])
        
        if verbose:
            print('done')
        
        # Assign outputs
        self.ts, self.mdots = ts, mdots
        self.t_layer = t_layer
        self.x_layer, self.H_layer = x_layer, H_layer
        self.m_jet, self.m_layer = m_jet, m_layer
        self.dP_layer, self.dP_tot = dP_layer, dP_tot
        self.Vol_layer, self.plumes = Vol_layer, jets

    def plot_trajectories(self):
        fig, ax = plt.subplots()
        lines = [list(zip(pl.x, pl.y)) for pl in self.plumes]
        line_segments = LineCollection(lines, norm = mpl.colors.LogNorm())
        line_segments.set_array(np.array(self.ts))
        ax.add_collection(line_segments)
        ax.autoscale()
        axcb = fig.colorbar(line_segments)
        axcb.set_label('Time [s]')
        ax.set_xlabel('x [m]')
        ax.set_ylabel('y [m]')
        ax.set_title('Release Path Trajectories Over Time')
        return fig

    def plot_mass_flows(self):
        fig, ax = plt.subplots()
        ax.semilogy(self.ts, self.mdots)
        ax.set_xlabel('Time [s]')
        ax.set_ylabel('Fuel Mass Flow Rate [kg/s]')
        return fig
    
    def plot_layer(self):
        fig, ax = plt.subplots(2, 1, sharex = True)
        l1 = ax[0].plot(self.t_layer, np.array(self.x_layer)*100,
                     label = 'Mole Fraction Fuel')
        ax[0].set_ylabel('%Fuel in Layer\n(Molar or Volume)')
        i = np.argmin(np.abs(self.t_layer - (np.max(self.t_layer)
                                        - np.min(self.t_layer)) / 2))
        l2 = ax[1].plot(self.t_layer, self.H_layer,
                      'g', label="Height of Layer")
        ax[1].set_ylabel('Layer Thickness  [m]\n(From Ceiling)')
        ax[1].set_xlabel('Time [s]')
        return fig

    def plot_mass(self):
        fig, ax = plt.subplots()
        ax.plot(self.t_layer, self.m_jet, label = 'Plume')
        ax.plot(self.t_layer, self.m_layer, label = 'Layer')
        ax.plot(self.t_layer, np.array(self.m_jet)+np.array(self.m_layer),
                 label='Combined')
        ax.set_xlabel('Time [s]')
        ax.set_ylabel('Flammable Fuel Mass [kg]')
        ax.legend(ncol=3, loc='lower center', bbox_to_anchor=(0.5, 1),
                  fancybox=True)
        return fig

    def plot_overpressure(self, data=None, limit=None):
        fig, ax = plt.subplots()
        ax.plot(self.t_layer, np.array(self.dP_layer)/1000, label='Layer')
        ax.plot(self.t_layer, np.array(self.dP_tot)/1000, label='Combined')
        if data is not None:
            i = 0
            cs = ['b', 'g', 'r', 'c', 'm', 'y', 'k']
            for d in data:
                ax.plot([d[0]], [d[1]], 'o', color=cs[i])
                i += 1
        if limit is not None:
            for l in limit:
                ax.axhline(l, color='k', dashes=(1, 1))
        ax.set_xlabel('Ignition Delay Time [s]')
        ax.set_ylabel('Overpressure [kPa]')
        ax.legend(ncol=2, loc='lower center', bbox_to_anchor=(0.5, 1),
                  fancybox=True)
        return fig

    def pressure(self, t):
        '''
        Returns pressure at time t (or times ts)
        
        Parameters
        -----------
        t : ndarray
           time(s) at which to return the pressure (s)
        
        Returns
        -------
        dP : ndarray
           overpressure(s) at time t (Pa)
        '''
        dp = interpolate.interp1d(self.t_layer, self.dP_tot, bounds_error = False)
        return dp(t)

    def layer_depth(self, t):
        '''
        Returns depth of layer at time t (or times ts)
        
        Parameters
        -----------
        t : ndarray
           time(s) at which to return the depth (s)
        
        Returns
        -------
        ld : ndarray
           layer height(s) at time t (m)
        '''
        ld = interpolate.interp1d(self.t_layer, self.H_layer, bounds_error = False)
        return ld(t)

    def concentration(self, t):
        '''
        Returns layer concentration at time t (or times ts)
        
        Parameters
        -----------
        t : ndarray
           time(s) at which to return the concentration(s)
        
        Returns
        -------
        lc : ndarray
           concentrations(s) at time t (%)
        '''
        lc = interpolate.interp1d(self.t_layer, self.x_layer, bounds_error = False)
        return 100 * lc(t)

    def max_p_t(self):
        '''
        Returns the maximum overpressure and time at it occurs
        
        Returns
        -------
        p_t : tuple
           maximum overpressure (Pa) and time when it occurs (s)
        '''
        imax = np.argmax(self.dP_tot)
        return self.dP_tot[imax], self.t_layer[imax]
        
    def dP_expansion(self, mass, fluid):
        '''
        Pressure due to the expansion of gas from combustion in an enclosure
        
        Parameters
        ----------
        mass : float
           mass of combustible gas in enclosure
        fluid : object
           gas being combusted (at the temperature and pressure of the gas in the enclosure)
           
        Returns
        -------
        P : float
           pressure upon expansion
        '''
        Vol_total = self.enclosure.V
        Vol_gas   = mass/fluid.rho
        
        X_u, sigma, gamma = self.comb.X_reac_stoich, self.comb.sigma, self.comb.gamma_reac
        
        VolStoich = Vol_gas/X_u

        deltaP  = fluid.P*((((Vol_total+Vol_gas)/Vol_total)*((Vol_total+VolStoich*(sigma-1))/Vol_total))**gamma-1)
        return deltaP
