"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import random


class PositionGenerator:
    """
    Generates positions from a list of location distributions and parameters
    """
    def __init__(self, loc_distributions, exclusion_radius=0.01, seed=None):
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
                dist_type : str
                    One of 'deterministic', 'uniform', or 'normal'
                param_a and param_b depend on the distribution type:
                    For 'deterministic': param_a = value, param_b = None.
                    For 'uniform': param_a = min_val, param_b = maxval
                    For 'normal': param_a = mu, param_b = sigma
        exclusion_radius : float (optional)
            Minimum distance in meters from origin (0,0,0)
            in any direction that all generated positions must be;
            default value is 0.01 m
        seed : int or None (optional)
            Seed for random number generator;
            default value is None, will generate new seed

        Calculated
        ----------
        randgen : Random object
            Random number generator based on seed value
        totworkers : float
            Total number of workers for all distributions
        locs : list of locations
            List of locations of interest, one for each occupant,
            each location is a tuple of 3 coordinates (m):
            [(x1, y1, z1), (x2, y2, z2), ...]
        """
        self.seed = seed
        self.randgen = random.Random(seed)
        self.exclusion_radius = exclusion_radius
        self.loc_distributions = loc_distributions
        self.totworkers = self.calc_total_workers()
        self.gen_positions()

    def calc_total_workers(self):
        total_workers = 0
        for dist_group in self.loc_distributions:
            n = dist_group[0]
            if n < 0 or not isinstance(n, int):
                raise ValueError(f'Number of wokers ({n}) must be a non-negative integer')
            total_workers += n
        return total_workers

    def gen_positions(self):
        locations = []
        for dist_group in self.loc_distributions:
            n = dist_group[0]
            xyz_dists = dist_group[1:]
            for _ in range(n):
                (x, y, z) = self._gen_valid_position(xyz_dists)
                locations.append((x, y, z))
        self.locs = locations

    def _gen_valid_position(self, xyz_dists):
        max_guesses = 500
        for _ in range(max_guesses):
            (x, y, z) = self._gen_xyz_loc(xyz_dists)
            distance = (x**2 + y**2 + z**2)**(1/2)
            if distance > self.exclusion_radius:
                return x, y, z
        raise ValueError('Unable to produce desired number of valid positions outside exclusion radius')

    def _gen_xyz_loc(self, xyz_dists):
        """
        Generates an (x, y, z) location for one distribution-set

        Parameters
        ----------
        xyz_dists : list
            Distribution information for x, y, and z in that order

        Returns
        -------
        locs : list
            List of lists for x, y, and z locations
        """
        if len(xyz_dists) != 3:
            raise ValueError('List of distributions for a single location must contain 3 items')
        locs = []
        for dist in xyz_dists:
            locs.append(self._gen_loc(dist))
        return locs

    def _gen_loc(self, dist_params):
        """
        Generates a location on single dimension (x, y, or z) from distribution

        Parameters
        ----------
        dist_params : list or tuple
            Distribution information as
            (dist_type, param_a, param_b)
        """
        dist_type = dist_params[0]
        if dist_type in ['dete', 'det', 'deterministic']:
            return self._gen_deterministic(dist_params[1])
        elif dist_type in ['norm', 'normal']:
            return self._gen_normal(dist_params[1], dist_params[2])
        elif dist_type in ['unif', 'uni', 'uniform']:
            return self._gen_uniform(dist_params[1], dist_params[2])
        else:
            raise NotImplementedError(dist_type + ' distribution not implemented')
        
    def _gen_normal(self, mu, sigma):
        """
        Generates a number from normal distribution

        Parameters
        ----------
        mu : float
            Mean of normal distribution
        sigma : float
            Standard deviation of normal distribution

        Returns
        -------
        number : float
            Normally distributed number
        """
        return self.randgen.gauss(mu, sigma)

    def _gen_uniform(self, min_pos, max_pos):
        """
        Generates a number from uniform distribution

        Parameters
        ----------
        min_pos : float
            Minimum of range to generate from
        max_pos : float
            Maximum of range to generate from

        Returns
        -------
        number : float
            Uniformly distributed number
        """
        return self.randgen.uniform(min_pos, max_pos)
    
    def _gen_deterministic(self, val):
        """
        Generates a number of deterministic value
        
        Parameters
        ----------
        val : float
            Value to generate

        Returns
        -------
        number : float
            Deterministic number
        """
        return val

    def get_xlocs(self):
        """Get x locations"""
        return [location[0] for location in self.locs]

    def get_ylocs(self):
        """Get y locations"""
        return [location[1] for location in self.locs]

    def get_zlocs(self):
        """Get z locations"""
        return [location[2] for location in self.locs]
