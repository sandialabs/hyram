"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


import numpy as np
from .distributions import LogNormDistribution



class Leak:
    """
    Random release (leak) chance, defined by an orifice diameter and probability of occurrence.

    Attributes
    ----------
    size : float
        Release size, as percentage of total orifice diameter
    mu : float
        Mean of associated normal distribution
    sigma : float
        Standard deviation of associated normal distribution

    """

    def __init__(self, size, mu, sigma):
        if size < 0 or size > 100:
            raise ValueError(f"Provide a valid leak size between 0 and 100% ({size} provided)")
        if mu in [None, "", np.nan] or sigma in [None, "", np.nan]:
            raise ValueError(f"Provide valid values for size {size}% parameters: mu {mu}, sigma {sigma}")

        self.size = size
        self.mu = mu
        self.sigma = sigma

    def get_leak_freq_mean(self):
        leak_freq_dist = LogNormDistribution(mu=self.mu, sigma=self.sigma)
        return leak_freq_dist.mean

    def __str__(self):
        result = f'Leak {self.size}%, mu: {self.mu}, sigma: {self.sigma}'
        return result


