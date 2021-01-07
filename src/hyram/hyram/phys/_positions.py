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

import numpy as np

# Note: non-interactive agg backend set in GUI. Set MPLBACKEND env variable to change.
import matplotlib.pyplot as plt
import matplotlib.patches as mplpatch


class PositionGenerator:
    """
    Class used to generate positions.

    Generates positions from a list of location distributions.
    Each location distribution should be a list of
    The meaning of param_a and param_b depend on th given distribution.

    Exclusion radius is the minimum distance from (0,0,0)
    that any generated poistion must be
    """

    def __init__(self, loc_distributions, exclusion_radius, seed):
        """
        Initializes position generator.

        Parameters
        ----------
        loc_distributions : list
            List of location distributions.  Each location distribution
            should be a list like
                [n, (xdist_type, xparam_a, xparam_b),
                 (ydist_type, yparam_a, yparam_b),
                 (zdist_type, zparam_a, zparam_b)]
            where *dist_type is one of 'deterministic', 'uniform', or 
            'normal' and param_a and param_b depend on the distribution type.
            For 'deterministic', param_a = value, param_b = None.
            For 'uniform', param_a = minval, param_b = maxval.
            For 'normal', param_a = mu, param_b = sigma.
        exclusion_radius : float
            Minimum distance from (0,0,0) that all generated
            positions must be.
        seed : int
            Seed for random number generator
        """

        self.randgen = np.random.RandomState(seed)
        self.exclusion_radius = exclusion_radius
        self.loc_distributions = loc_distributions
        # Number of extra to generate in case some inside exclusion zone
        self.extra = 3
        
        # Compute total number of workers (all distributions)
        self.totworkers = sum([dist[0] for dist in loc_distributions])

        # Initialize array for actual locations
        self.locs = np.zeros((3, self.totworkers), dtype=float)

    def get_xlocs(self):
        """Get x locations"""
        return self.locs[0,:]

    def get_ylocs(self):
        """Get y locations"""
        return self.locs[1,:]

    def get_zlocs(self):
        """Get z locations"""
        return self.locs[2,:]
        
    def gen_positions(self):
        """Generate positions"""
        curidx = 0
        for dist in self.loc_distributions:
            nworkers = dist[0]

            valid_list = [[],[],[]]
            # Small function to identify valid locations and add
            def get_valid(locs, num_needed):
                # Find all the ones that were valid (x**2 + y**2 + z**2 < r**2)
                valid_idx = np.where(locs[0]**2 + locs[1]**2 + locs[2]**2 > self.exclusion_radius**2)

                # Get num_needed or num_valid if less than num_needed
                nvalid = np.size(valid_idx)
                if nvalid > num_needed:
                    endidx = num_needed
                else:
                    endidx = nvalid
                for i in range(3):
                    valid_list[i].append(locs[i][valid_idx][:endidx])

                return endidx

 
            # Generate x, y, and z
            # Generate a few extra in case some are inside exclusion zone
            locs = self._gen_xyz_locs(nworkers + self.extra, dist[1:])
            numvalid = get_valid(locs, nworkers)

            # Just in case, go through the remaining and generate some more
            counter = 0
            remaining = nworkers - numvalid
            while remaining > 0:
                addl_locs = self._gen_xyz_locs(remaining, dist[1:])
                numvalid = get_valid(addl_locs, remaining)
                remaining -= numvalid
                counter += 1
                if counter > 500:
                    raise ValueError('Unable to produce desired number of valid positions outside exclusion radius')

            # Set actual locs to contain needed number of valid ones
            for i in range(3):
                self.locs[i,curidx:curidx+nworkers] = np.array(valid_list[i]).flatten()
            curidx += nworkers

    def _gen_xyz_locs(self, n, dist_info):
        """
        Generates appropriate x, y, and z locations for one distribution

        Parameters
        ----------
        n : int
            Number to generate
        dist_info : list
            Distribution information for x, y, and z in that order.

        Returns
        -------
        list
            List of arrays for x, y, and z locations.
        """
        locs = []
        for i in range(3):
            locs.append(self._gen_locs(n, dist_info[i]))
        return locs

    def _gen_normal(self, n, mu, sigma):
        """
        Generates numbers from normal distribution.
        Simply wraps numpy random generator

        Parameters
        ----------
        n : int
            Number to generate
        mu : float
            Mean of normal distribution
        sigma : float
            Standard deviation of normal distribution

        Returns
        -------
        array
            Normally distributed numbers
        """
        return self.randgen.normal(mu, sigma, n)

    def _gen_uniform(self, n, minpos, maxpos):
        """
        Generates numbers from uniform distribution.
        Simply wraps numpy random generator.

        Parameters
        ----------
        n : int
            Number to generate
        minpos : float
            Minimum of range to generate from
        maxpos : float
            Maximum of range to generate from

        Returns
        -------
        array
            Normally distributed numbers
        """
        return self.randgen.uniform(minpos, maxpos, n)
    
    def _gen_deterministic(self, n, val):
        """
        Generates array of deterministic value.
        
        Parameters
        ----------
        n : int
            Number to generate
        val :
            Value to generate

        Returns
        -------
        array
            Filled with deterministic value.
        """
        return np.ones(n, dtype=float)*val

    def _gen_locs(self, n, dist_params):
        """
        Generates array of x, y, or z coordinate from distribution.

        Parameters
        ----------
        n : int
            Number to generate
        dist_params : list or tuple
            Distribution information as
            (dist_type, param_a, param_b)
        """
        dist_type = dist_params[0]
        
        if dist_type in ['dete', 'det', 'deterministic']:
            return self._gen_deterministic(n, dist_params[1])
        elif dist_type in ['norm', 'normal']:
            return self._gen_normal(n, dist_params[1], dist_params[2])
        elif dist_type in ['unif', 'uni', 'uniform']:
            if dist_params[1] > dist_params[2]:
                raise ValueError('First parameter cannot be larger than second parameter for uniform distribution')
            return self._gen_uniform(n, dist_params[1], dist_params[2])
        else:
            raise NotImplementedError(dist_type + ' distribution not implemented')
        
    def plot_positions(self, filename, title, qrad, length, width):
        """
        Plots positions on facility grid colored by heatflux

        Parameters
        ----------
        filename : string
            filename for saving plot
        title : string
            title for plot
        qrad : array
            Heatflux at each position for coloring points
        length : float
            Facility length
        width : float
            Facility width
        """
        fig, ax = plt.subplots()
        
        # Blue square represents hydrogen release source
        ax.plot(0, 0, 'bs', markersize=8, label='Hydrogen leak source')
        # Plot facility border
        ax.add_patch(mplpatch.Rectangle((-.2, -.2), length+.2,
                                         width+.2, fill=None))
        # Arrow representing flame direction, does not represent length/width
        ax.arrow(0, 0, length/4.0, 0, head_width=0.7, alpha=1.0,
                 head_length=0.7, linewidth=0.3, fc='blue', ec='blue')
        # Plot actual occupant positions
        heatflux_ln = ax.scatter(self.locs[0,:], self.locs[2,:], s=36, c=qrad,
                                 linewidths=0.5, edgecolors='black',
                                 cmap=plt.cm.get_cmap('plasma'))
        # Plot formatting, colorbar, and saving
        ax.set_aspect('equal') 
        for spine in ['top', 'bottom', 'left', 'right']:
            ax.spines[spine].set_visible(False)
        ax.set_xlabel('Length (m)')
        ax.set_ylabel('Width (m)')
        ax.set_title(title)
        cb = fig.colorbar(heatflux_ln)
        cb.set_label('Radiative Heat Flux (kW/m^2)')
        plt.savefig(filename, bbox_inches='tight')
        plt.close()
 
