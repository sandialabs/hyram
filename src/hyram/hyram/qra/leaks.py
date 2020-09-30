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

import logging
from collections import OrderedDict

import numpy as np
import pandas as pd

from .distributions import LogNormDistribution
from ..utilities import constants

#pd.set_option('max_colwidth', None)
log = logging.getLogger('hyram.qra')


class Leak(object):
    """
    Random Hydrogen release (leak) chance, defined by an orifice diameter and probability of occurrence.

    Parameters
    ----------
    description : str
        Optional description of leak instance.
    size : float
        Release size, as percentage of total orifice diameter. Current hard-coded options are 0.01, 0.1, 1, 10, 100
    mu : float
        Mean of associated normal distribution
    sigma : float
        Standard deviation of associated normal distribution

    Attributes
    ----------
    size : float
    description : str
    probability : scipy rv_continuous object
        Object representation of lognormal distribution
    mean : float
        mean parameter of lognormal distribution of this leak probability
    variance : float
        variance parameter of lognormal distribution of this leak probability

    Methods
    ----------
    _distribution(mu, sigma, mean, variance)
        Compute lognormal distribution for chance of leak occurring.
        Also available as property.

    """

    def __init__(self, size, mu, sigma, description=''):
        self.description = str(description)

        valid, error_msg = self.parameters_valid(size, mu, sigma)
        if not valid:
            log.error(error_msg)
            raise ValueError(error_msg)

        # Log normal distribution object for this leak size if mean/variance not given
        self.size = size
        self.mu = mu
        self.sigma = sigma
        self.prob = self.p = self.probability = LogNormDistribution(mu=self.mu, sigma=self.sigma)
        self.mean = self.prob.mean
        self.variance = self.prob.variance

    def parameters_valid(self, size, mu, sigma):
        """ Check whether incoming parameters are valid. """
        valid = True
        msg = ""
        if size < 0. or size > 100.:
            valid = False
            msg = "Leak size {} is not valid. Size must be between 0 and 100%".format(size)
        elif mu in [None, "", np.nan] or sigma in [None, "", np.nan]:
            valid = False
            msg = ("Leak probability parameters for size {}% invalid. Must include valid mu and sigma."
                   "Passed values for mu/sigma/mean/variance are {}, {}."
                   ).format(size, mu, sigma)

        return valid, msg

    def __str__(self):
        return 'Leak {} mu, {} sigma, {} mean, {} var'.format(self.mu, self.sigma, self.mean, self.variance)

    def _get_df(self):
        """
        Output leak params as dataframe.

        Returns
        -------
        df : pandas DataFrame
            frame of leak properties
        """
        df = pd.DataFrame(columns=['description', 'size (%)', 'probability'])
        # Set dataframe data
        df.loc[0] = [self.description, self.size, self.prob]
        return df

    def _repr_html_(self):
        """
        Output leak parameters as string in HTML format (via dataframe).
        """
        df = self._get_df()
        return df.to_html(index=False)

    def __str__(self):
        """
        Output leak as string structured by dataframe.
        """
        df = self._get_df()
        return df.to_string(index=False)


class LeakSet(object):
    """
    Instantiate and store multiple leak objects for single component.

    Parameters
    ----------
    component_leak_probs : list
        List of dicts containing probability data for this component in order of increasing leak size
        [{mu, sigma}, ...]

    leak_sizes: list
        List of five leak sizes as floats

    Attributes
    ----------
    leaks
        dict of leaks accessed by size, in percentage. 0.01, 0.1, 1, 10, 100

    """

    def __init__(self, parent, component_leak_probs, leak_sizes=None):
        self.leaks = OrderedDict()
        self.parent = parent

        if leak_sizes is None:
            leak_sizes = constants.LEAK_SIZES

        # Create leak representations for each size
        for i, leak_size in enumerate(leak_sizes):
            component_probability = component_leak_probs[i]
            leak = Leak(leak_size, component_probability['mu'], component_probability['sigma'])
            self.leaks[leak.size] = leak

    def get_leak(self, leak_size):
        """ Get leak object corresponding to leak size. """
        if leak_size in self.leaks.keys():
            return self.leaks[leak_size]
        else:
            raise KeyError('Leak of specified size not found')

    def get_means(self):
        """
        Gather means from all leaks into ndarray

        Returns
        -------
        means : ndarray
            Means from distribution of all leaks, in order of ascending leak size.
        """
        leaks_list = self._as_list()
        return np.array([leak.mean for leak in leaks_list])

    def get_variances(self):
        """
        Gather variances from all leaks into ndarray

        Returns
        -------
        variances : ndarray
            Variances from distribution of all leaks, in order of ascending leak size.
        """
        leaks_list = self._as_list()
        return np.array([leak.variance for leak in leaks_list])

    def _as_list(self):
        """
        Return leaks in list format, sorted by leak size.
        """
        return sorted(self.leaks.values(), key=lambda x: x.size)

    def _get_df(self):
        """
        Get all leaks in single dataframe.
        """
        df = pd.DataFrame(columns=['size (%)', 'probability'])
        leak_list = self._as_list()
        for i, leak in enumerate(leak_list):
            df.loc[i] = [leak.size, leak.prob]
        return df

    def _repr_html_(self):
        """
        Output all leak parameters as string in HTML format (via dataframe).
        """
        df = self._get_df()
        return df.to_html(index=False)

    def __str__(self):
        """
        Output all leak as string structured by dataframe.
        """
        df = self._get_df()
        return df.to_string(index=False)
