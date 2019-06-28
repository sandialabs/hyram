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

import matplotlib.pyplot as plt
import matplotlib as mpl
from matplotlib.collections import LineCollection
import numpy as np
from scipy import interpolate
import scipy.constants as const

from ._comps import Gas, Orifice
from ._jet import Jet
from ._layer import runLayerModel
from ._overpressure import dP_expansion


class IndoorRelease:
    '''
    Class used to calculate physics of an indoor release
    '''
    def __init__(self, source, orifice, ambient, enclosure, tmax = 30, 
                 release_area = None, theta0 = 0, x0 = 0, y0 = 0, S0 = 0, 
                 lam = 1.16, Xi_lean = 0.04, tol = 1e-5, n_pts_plume = 50, 
                 n_pts_layer = 200, numplume = 200, layerMod = 'Lowesmith', 
                 Ymin = 0.0, max_steps = 1e5, steady = False, 
                 verbose = True):
        '''
        Initialization for an indoor release
        
        Parameters
        ----------
        source : class
            source for gas (must contain blowdown function)
        orifice : class
            orifice through which source is flowing
        ambient : class
            gas initially contained in enclosure
        enclosure : class
            enclosure into which release is occurring
        tmax : float (optional)
            maximum time for simulation (s)
        release_area : float (optional)
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
        Xi_lean : float (optional)
            molar lower flammability limit, default is 0.04
        Ymin : float (optional)
            Minimum mole fraction out to which h2_jet will integrate
            Default is 0
        max_steps : integer (optional)
            Maximum steps along the S axis for h2_jet integration
            Default is 1e5
        tol : float (optional)
            Tolerance for h2_jet integration, default is 1e-5
        n_pts_plume : integer (optional)
            Number of points along jet/plume for h2_jet integration
            Default is 50
        n_pts_layer : integer (optional)
            Number of points for layer (h2_overpressure) integegration 
            Equivalent to number of time steps for steady release
            Each jet/plume for blowdown uses this many time steps
            Default is 200
        numplume : integer (optional)
            Used for blowdown calcualtion only
            Number of jet/plumes, equivalent to number of time steps
            Default is 200 for blowdown, not used for steady release
        layerMod : string (optional)
            Layer model to use, default is 'Lowesmith'
        steady : Boolean (optional)
            Option for a blowdown release or a steady-flowrate release
            Default is False, for a blowdown
        '''
        if verbose:
            print('Performing indoor release calculations...')
        
        # Calculate time steps and mass flow history
        if steady:
            # Single jet/plume for steady release, so single time step
            ts = [0, tmax]
            steady_mdot = source.mdot(orifice)
            mdots = np.ones(len(ts)) * steady_mdot
            gas_list = [source.gas for i in range(len(ts))]
        else:
            # Different jet/plume at each time step for blowdown
            ts = np.zeros(numplume)
            ts[1:] = np.logspace(np.log10(tmax / 1e5), 
                                 np.log10(tmax), numplume - 1)
            mdots, gas_list = source.blowdown(ts, orifice)
        
        # Source gas at ambient conditions
        gas = Gas(source.gas.therm, T = ambient.T, P = ambient.P)
        
        if release_area is not None:
            # switch gas to ambient pressure (subsonic in jet below)
            gas_list = len(gas_list)*[gas]
            orifice = Orifice(np.sqrt(release_area*4/const.pi))
        
        # Initialize iteration counter
        i = 0
        
        # Initial 'layer' is between vent and ceiling
        Vol = (enclosure.H - enclosure.ceiling_vent.H) * enclosure.A
        # Initial layer concentration is 0
        c = 0
        
        # Limit of maximum distance jet can extend
        LIM = enclosure.Xwall + enclosure.H
        
        # Initialize solutions
        x_layer, H_layer, m_jet = [0], [0], [0]
        m_layer, dP_layer, dP_tot = [0], [0], [0]
        Vol_layer, plumes = [Vol], []
        
        # Initialize time step
        t_old = ts[0]
        
        # Run layer model at each time step for each plume
        for mdot, t, gas_i in zip(mdots[1:], ts[1:], gas_list[1:]):
            # Check first step, or if jet at previous step is the same
            if i == 0 or mdot != mdots[i]:
                # Calculate release jet
                # changed gas_i to gas--makes all subsonic releases
                jet = Jet(gas, orifice, ambient, theta0 = theta0, y0 = y0, 
                          x0 = x0, lam = lam, Q = mdot/gas.rho, Smax = LIM, 
                          dS = LIM/n_pts_plume, Ymin = Ymin, 
                          max_steps = max_steps, tol = tol, verbose = verbose)
            else:
                # Jet is the same as previous step
                jet = plumes[i-1]
            plumes.append(jet)
            
            # Reshape jet if needed
            res = jet.reshape(enclosure, showPlot=False)
            
            # Update initial volume and concentration for layer model
            Vol_conc0 = np.array([Vol, c])
            
            # Make time series for layer model integration
            tsi = np.ones(n_pts_layer) * t_old
            tsi[1:] = np.logspace(np.log10(t_old + (t / 1e5)), 
                                  np.log10(t), n_pts_layer - 1)
            
            # Compute volumetric distribution of gas along the ceiling
            Vol, c = runLayerModel(tsi, Vol_conc0, res, 
                                   enclosure, model = layerMod)
            
            # Ensure volume and mole fraction values are realistic
            if any(Voli < 0 for Voli in Vol):
                raise ValueError('Layer volume has returned a negative value')
            if any(Voli > enclosure.V for Voli in Vol):
                raise ValueError('Layer volume has exceeded enclosure volume')
            if any(ci < 0 for ci in c):
                raise ValueError('Layer concentration has returned a negative value')
            if any(ci > 1 for ci in c):
                raise ValueError('Layer concentration has exceeded 100%')
            
            # Single value for blowdown, entire array for steady
            if not steady:
                Vol = Vol[-1]
                c = c[-1]
            
            # Calculate flammable mass in layer
            MW_layer = c * gas.therm.MW + (1 - c) * ambient.therm.MW
            rho_layer = ambient.P / (ambient.T * const.R * 1000.0 
                                     / MW_layer)
            Y_layer = c * gas.therm.MW / MW_layer
            m_layer_i = Vol * rho_layer * Y_layer * (c >= Xi_lean)
            
            # Calculate height of layer
            H_layer_i = Vol / enclosure.A
            
            # Make layer flam. mass and height iterable if not already
            try:
                Hm_layer = zip(H_layer_i, m_layer_i)
            except TypeError:
                Hm_layer = zip([H_layer_i], [m_layer_i])
            
            m_jet_i, dP_tot_i, dP_layer_i = [], [], []
            for Hi, mli in Hm_layer:
                # Calculate flammable mass in jet
                m_jet_ij = res.m_flammable(enclosure.H - Hi, Xi_lean)
                m_jet_i.append(m_jet_ij)
                
                # Calculate total overpressure
                dP_tot_ij = dP_expansion(enclosure, m_jet_ij+mli, gas)
                dP_tot_i.append(dP_tot_ij)
                
                # Calculate overpressure in layer
                dP_layer_ij = dP_expansion(enclosure, mli, gas)
                dP_layer_i.append(dP_layer_ij)
            
            # Assign outputs for this plume-timestep
            if steady:
                x_layer = c
                Vol_layer = Vol
                m_layer = m_layer_i
                H_layer = H_layer_i
                m_jet = m_jet_i
                dP_tot = dP_tot_i
                dP_layer = dP_layer_i
            else:
                x_layer.append(c)
                Vol_layer.append(Vol)
                m_layer.append(m_layer_i)
                H_layer.append(H_layer_i)
                m_jet.append(m_jet_i[0])
                dP_tot.append(dP_tot_i[0])
                dP_layer.append(dP_layer_i[0])
            
            # Update time step and iteration
            t_old = t
            i += 1
        
        # Assign overall integraiton time history for steady release
        if steady:
            ts = tsi
            mdots = np.ones(len(ts)) * steady_mdot
        
        if verbose:
            print('')
        
        # Assign outputs
        self.enclosure = enclosure
        self.ts, self.mdots = ts, mdots
        self.x_layer, self.H_layer = x_layer, H_layer
        self.m_jet, self.m_layer = m_jet, m_layer 
        self.dP_layer, self.dP_tot = dP_layer, dP_tot
        self.Vol_layer, self.plumes = Vol_layer, plumes
    
    
    def plot_trajectories(self):
        fig, ax = plt.subplots()
        lines = [list(zip(pl.x, pl.y)) for pl in self.plumes]
        line_segments = LineCollection(lines, norm = mpl.colors.LogNorm())
        line_segments.set_array(self.ts)
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
        ax.set_ylabel('Hydrogen Mass Flow Rate [kg/s]')
        return fig
    
    
    def plot_layer(self):
        fig, ax = plt.subplots()
        l1 = ax.plot(self.ts, np.array(self.x_layer)*100, 
                     label = 'Mole Fraction Hydrogen')
        ax.set_ylabel(r'% (Molar or Volume)')
        ax.set_xlabel('Time [s]')
        i = np.argmin(np.abs(self.ts - (np.max(self.ts) 
                                        - np.min(self.ts)) / 2.0))
        ax.annotate('', xy = (self.ts[i], self.x_layer[i]*100), 
                    xycoords = 'data', xytext = (-30, -10),
                    textcoords = 'offset points',
                    arrowprops = dict(arrowstyle = '<-', 
                                      connectionstyle="angle,angleA=0,angleB=45,rad=5",
                                      color = 'b'))
        ax2 = ax.twinx()
        l2 = ax2.plot(self.ts, self.enclosure.H - np.array(self.H_layer), 
                      'g', label="Height of Layer") 
        ax2.set_ylabel('Height From Floor of Layer  [m]', color = 'g', 
                       rotation = -90, va = 'bottom')
        ax2.annotate('', xy = (self.ts[i], self.enclosure.H - self.H_layer[i]), 
                     xycoords = 'data', xytext = (30, 10), 
                     textcoords = 'offset points',
                     arrowprops = dict(arrowstyle = '<-', 
                                       connectionstyle="angle,angleA=0,angleB=45,rad=5",
                                       color = 'g'))
        lns = l1 + l2
        labs = [l.get_label() for l in lns]
        ax.legend(lns, labs, ncol = 2, loc='lower center', 
                  bbox_to_anchor=(0.5, 1.0), fancybox=True)
        return fig
    
    
    def plot_mass(self):
        fig, ax = plt.subplots()
        ax.plot(self.ts, self.m_jet, label = 'Plume')
        ax.plot(self.ts, self.m_layer, label = 'Layer')
        ax.plot(self.ts, np.array(self.m_jet)+np.array(self.m_layer), 
                 label='Combined')
        ax.set_xlabel('Time [s]')
        ax.set_ylabel('Flammable Hydrogen Mass [kg]')
        ax.legend(ncol=3, loc='lower center', bbox_to_anchor=(0.5, 1.0), 
                  fancybox=True)
        return fig
    
    
    def plot_overpressure(self, data = None, limit = None):
        fig, ax = plt.subplots()
        ax.plot(self.ts, np.array(self.dP_layer)/1000., label='Layer')
        ax.plot(self.ts, np.array(self.dP_tot)/1000., label='Combined')
        if data is not None:
            i = 0
            cs = ['b', 'g', 'r', 'c', 'm', 'y', 'k']
            for d in data:
                ax.plot([d[0]], [d[1]], 'o', color = cs[i])
                i += 1
        if limit is not None:
            for l in limit:
                ax.axhline(l, color = 'k', dashes = (1,1))
        ax.set_xlabel('Ignition Delay Time [s]')
        ax.set_ylabel('Overpressure [kPa]')
        ax.legend(ncol=2, loc='lower center', bbox_to_anchor=(0.5, 1.0), 
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
        dp = interpolate.interp1d(self.ts, self.dP_tot)
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
        ld = interpolate.interp1d(self.ts, self.H_layer)
        return ld(t)
    
    
    def concentration(self, t):
        '''
        Returns layer concentration at time t (or times ts)
        
        Parameters
        -----------
        t : ndarray
           time(s) at which to return the pressure (s)
        
        Returns
        -------
        lc : ndarray
           concentrations(s) at time t (%)
        '''
        lc = interpolate.interp1d(self.ts, self.x_layer)
        return 100.0 * lc(t)
    
    
    def max_p_t(self):
        '''
        Returns the maximum overpressure and time at it occurs
        
        Returns
        -------
        p_t : tuple
           maximum overpressure (Pa) and time when it occurs (s)
        '''
        imax = np.argmax(self.dP_tot)
        return self.dP_tot[imax], self.ts[imax]

