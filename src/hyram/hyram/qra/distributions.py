"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

"""
Probability distribution classes and helper funcs for convenience.
This file should NOT import from any QRA-specific code.
"""

import numpy as np
from scipy.stats import beta, lognorm, uniform, norm


class DistributionWrapper(object):
    """
    Convenience class to build similar distributions (i.e. scipy continuous_ev).

    Parameters
    ----------
    a :
    b :
    distr_class : class
        scipy class of distribution, e.g. norm
    loc :
    scale :

    """

    def __init__(self, a, b, distr_class, loc=0, scale=1):
        self.name = None
        self.a = a
        self.b = b
        self.loc = loc
        self.scale = scale

        if distr_class == uniform:
            self.distribution = distr_class(a, b)
        else:
            self.distribution = distr_class(a, b, loc=self.loc, scale=self.scale)

        self.mean = float(self.distribution.mean())
        self.var = self.variance = float(self.distribution.var())
        self.rvs = self.distribution.rvs

    def __str__(self):
        return '{}, a={:.3g}, b={:.3g}'.format(self.name, self.a, self.b)


class BetaDistribution(DistributionWrapper):
    """ Beta distribution setup """

    def __init__(self, a, b, loc=0, scale=1):
        if a <= 0. or b <= 0.:
            raise ValueError('Invalid beta distribution - parameters must be greater than 0')
        super().__init__(a, b, loc=loc, scale=scale, distr_class=beta)
        self.name = "Beta"


class UniformDistribution(DistributionWrapper):
    """ Uniform distribution setup """
    """

    """

    def __init__(self, a, b):
        if a >= b:
            raise ValueError('Uniform distribution parameter b must be >= a')
        super().__init__(a, b, distr_class=uniform)
        self.name = "Uniform"


class NormalDistribution(DistributionWrapper):
    """ Normal distribution setup """

    def __init__(self, a, b, loc=0, scale=1):
        if b <= 0.:
            raise ValueError('Normal distribution parameter b must be > 0')
        super().__init__(a, b, loc=loc, scale=scale, distr_class=norm)
        self.name = "Normal"


class EVDistribution:
    """
    Expected Value distribution.

    Parameters
    ----------
    value :
    """

    def __init__(self, value, *args, **kwargs):
        self.name = "Expected Value"
        self.value = value
        self.distribution = ExpectedValue(value)

        self.mean = float(self.distribution.mean())
        self.var = self.variance = float(self.distribution.var())

    def __str__(self):
        return 'Expected Value = {:.3g}'.format(self.value)


class ExpectedValue:
    def __init__(self, value, *args, **kwargs):
        self.value = float(value)

    # def __getattribute__(self, attr):
    #     if attr == 'mean':
    #         return self.mean
    #     elif attr == 'ppf':
    #         return self.ppf
    #     elif attr == 'pdf':
    #         return self.pdf
    #     elif attr == 'cdf':
    #         return self.cdf
    #     elif attr == 'var':
    #         return self.var

    def mean(self):
        return self.value

    def ppf(self, percentile=None):
        return self.value

    def pdf(self, x):
        return x == self.value

    def cdf(self, x):
        return x >= self.value

    def var(self):
        return 0.


class LogNormDistribution(object):
    """

    """

    def __init__(self, mu, sigma):
        self.name = "Lognormal"
        self.mu = mu
        self.sigma = sigma

        # NOTE: does not use scipy lognorm since that can't accommodate s==shape==sigma==0
        self.distribution = None
        # NOTE: lognormal distribution will use geometric mean (median) rather than arithmetic mean
        self.mean = np.exp(self.mu)
        self.var = self.variance = (np.exp(self.sigma ** 2) - 1) * np.exp(2 * self.mu + self.sigma ** 2.)
        self.rvs = None

    def __str__(self):
        return 'Lognormal, mu={:.3f}, sigma={:.3f}'.format(self.mu, self.sigma)


DISTRIBUTIONS = {
    'logn': LogNormDistribution,
    'beta': BetaDistribution,
    'expv': EVDistribution,
    'unif': UniformDistribution,
    'norm': NormalDistribution,
}


def get_distribution_class(distr_name):
    """ Retrieve correct distribution class based on string name """
    lname = str(distr_name).lower()
    if lname in ['lognorm', 'lognormal', 'logn']:
        return DISTRIBUTIONS['logn']

    elif lname in ['beta', 'bt']:
        return DISTRIBUTIONS['beta']

    elif lname in ['expv', 'ev', 'expectedvalue', 'expected_value', 'expected_val', 'exp_val']:
        return DISTRIBUTIONS['expv']

    elif lname in ['unif', 'uniform', 'uni']:
        return DISTRIBUTIONS['unif']

    elif lname in ['norm', 'normal', 'norml']:
        return DISTRIBUTIONS['norm']

    else:
        raise KeyError("Distribution {} not found. Available options are {}".format(distr_name, DISTRIBUTIONS.keys()))


def has_distribution(distr_name):
    """ Only checks whether ID matches a distribution; does not return it"""
    try:
        distr = get_distribution_class(distr_name)
        return True
    except Exception:
        return False


def get_distribution_options():
    return DISTRIBUTIONS.keys()
