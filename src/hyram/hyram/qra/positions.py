"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from __future__ import print_function, absolute_import, division

import numpy as np


class PositionGenerator:
    """
    Class used to generate positions.

    Generates positions from a list of location distributions and parameters
    """

    def __init__(self, loc_distributions, exclusion_radius, seed):
        """
        Initializes position generator

        Parameters
        ----------
        loc_distributions : list
            List of location distributions
            Each location distribution within this list should be a list like:
                [n,
                 (xdist_type, xparam_a, xparam_b),
                 (ydist_type, yparam_a, yparam_b),
                 (zdist_type, zparam_a, zparam_b)]
            where
                n : int
                    Number of occupants for this distribution
                *dist_type : str
                    One of 'deterministic', 'uniform', or 'normal'
                param_a and param_b depend on the distribution type:
                    For 'deterministic': param_a = value, param_b = None.
                    For 'uniform': param_a = min_val, param_b = maxval
                    For 'normal': param_a = mu, param_b = sigma
        exclusion_radius : float
            Minimum distance in meters from (0,0,0)
            in any direction that all generated positions must be
        seed : int
            Seed for random number generator
        
        Calculated
        ----------
        randgen : NumPy RandomState object
            Random number generator based on seed value
        totworkers : float
            Total number of workers for all distributions
        locs : list of locations
            List of locations of interest, one for each occupant,
            each location is a tuple of 3 coordinates (m):
            [(x1, y1, z1), (x2, y2, z2), ...]
        """
        self.seed = seed
        self.randgen = np.random.RandomState(seed)
        self.exclusion_radius = exclusion_radius
        self.loc_distributions = loc_distributions
        
        # Compute total number of workers (all distributions)
        self.totworkers = sum([dist[0] for dist in loc_distributions])

        # Generate locations
        self.gen_positions()

    def gen_positions(self):
        locations = []
        for dist in self.loc_distributions:
            # get a valid position with the given distribution
            def get_position():
                (x, y, z) = (0, 0, 0)
                counter = 0
                while x**2 + y**2 + z**2 <= self.exclusion_radius**2:
                   (x, y, z) = self._gen_xyz_locs(1, dist[1:])
                   counter += 1
                   if counter > 500:
                       raise ValueError('Unable to produce desired number of valid positions outside exclusion radius')
                return x, y, z
            # generate n positions with this distribution
            n = dist[0]
            for _ in range(n):
                (x, y, z) = get_position()
                locations.append((float(x), float(y), float(z)))
        self.locs = locations

    def _gen_xyz_locs(self, n, dist_info):
        """
        Generates x, y, and z locations for one distribution

        Parameters
        ----------
        n : int
            Number to generate
        dist_info : list
            Distribution information for x, y, and z in that order

        Returns
        -------
        list
            List of arrays for x, y, and z locations
        """
        locs = []
        for i in range(3):
            locs.append(self._gen_locs(n, dist_info[i]))
        return locs

    def _gen_normal(self, n, mu, sigma):
        """
        Generates numbers from normal distribution
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
        Generates numbers from uniform distribution
        Simply wraps numpy random generator

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
        Generates array of deterministic value
        
        Parameters
        ----------
        n : int
            Number to generate
        val :
            Value to generate

        Returns
        -------
        array
            Filled with deterministic value
        """
        return np.ones(n, dtype=float)*val

    def _gen_locs(self, n, dist_params):
        """
        Generates array of locations on single coordinate (x, y, or z) from distribution

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
        
    def get_xlocs(self):
        """Get x locations"""
        return [location[0] for location in self.locs]

    def get_ylocs(self):
        """Get y locations"""
        return [location[1] for location in self.locs]

    def get_zlocs(self):
        """Get z locations"""
        return [location[2] for location in self.locs]
