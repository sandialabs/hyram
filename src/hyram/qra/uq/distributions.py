"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from copy import deepcopy

import scipy.stats as sps
import numpy as np
import matplotlib.pyplot as plt


""" Module for defining uncertainty distributions and capabilities """


def specify_distribution(parameter_specification):
    """function to generate specified distribution type object
    Parameters
    ----------
    parameter_specification : dict
        dictionary containing specification of desired uncertainty
        distribution. Required keys include:

          - 'name': Name of the parameter
          - 'distribution_type': Type of distribution for the parameter. Valid
          entries are 'deterministic', 'beta', 'normal',
          'log_normal', 'trunc_normal', and 'uniform'

        Additionally, each `distribution_type` requires distribution-specific
        inputs to properly define the distribution:
          - `'deterministic'`: 'value'
          - `'beta'`: 'uncertainty_type', 'a', 'b'
          - `'normal'`: 'uncertainty_type', 'mean', 'std_deviation'
          - `'log_normal'`: 'uncertainty_type', 'mu', 'sigma'
          - `'trunc_normal'`: 'uncertainty_type', 'mean', 'std_deviation',
            'lower_bound', 'upper_bound'
          - `'uniform'`: 'uncertainty_type', 'lower_bound', 'upper_bound'


    Returns
    -------
    dist : UncertaintyCharacterization
        specified distribution type object

    """
    distribution_type = parameter_specification['distribution_type']
    parameter_name = parameter_specification['name']
    if distribution_type != 'deterministic':
        uncertainty_type = parameter_specification['uncertainty_type']

    if distribution_type == 'deterministic':
        dist = DeterministicCharacterization(name=parameter_name,
                                             value=parameter_specification['value'])
    elif distribution_type == 'beta':
        dist = BetaDistribution(name=parameter_name,
                                uncertainty_type=uncertainty_type,
                                a=parameter_specification['a'],
                                b=parameter_specification['b'])
    elif distribution_type == 'normal':
        dist = NormalDistribution(name=parameter_name,
                                  uncertainty_type=uncertainty_type,
                                  mean=parameter_specification['mean'],
                                  std_deviation=parameter_specification['std_deviation'],)
    elif distribution_type == 'log_normal':
        dist = LognormalDistribution(name=parameter_name,
                                     uncertainty_type=uncertainty_type,
                                     mu=parameter_specification['mu'],
                                     sigma=parameter_specification['sigma'])
    elif distribution_type == 'trunc_normal':
        dist = TruncatedNormalDistribution(name=parameter_name,
                                           uncertainty_type=uncertainty_type,
                                           mean=parameter_specification['mean'],
                                           std_deviation=parameter_specification['std_deviation'],
                                           lower_bound=parameter_specification['lower_bound'],
                                           upper_bound=parameter_specification['upper_bound'])
    elif distribution_type == 'trunc_lognormal':
        dist = TruncatedLognormalDistribution(name=parameter_name,
                                              uncertainty_type=uncertainty_type,
                                              mu=parameter_specification['mean'],
                                              sigma=parameter_specification['std_deviation'],
                                              lower_bound=parameter_specification['lower_bound'],
                                              upper_bound=parameter_specification['upper_bound'])

    elif distribution_type == 'uniform':
        dist = UniformDistribution(name=parameter_name,
                                   uncertainty_type=uncertainty_type,
                                   lower_bound=parameter_specification['lower_bound'],
                                   upper_bound=parameter_specification['upper_bound'])
    else:
        return ValueError(f'Distribution type {distribution_type} not supported, ' +
                        'supported: deterministic, beta, normal, log_normal, ' +
                        'trunc_normal, uniform')

    return dist


def convert_distributions_to_deterministic(distributions:dict):
    """Converts all uncertain parameters into deterministic
     inputs based on their mean values.

    Parameters
    ----------
    distributions : dict
        Distributions to be included in the uncertainty
        quantification study, where each value is either a
        `DeterministicCharacterization` or `UncertainyCharacterization`
        (or a subclass thereof).

    Returns
    -------
    deterministic_parameters : dict
        Contains uncertain parameters, now all deterministic
    """
    deterministic_distributions = deepcopy(distributions)
    for name, distribution_def in distributions.items():
        if isinstance(distribution_def, UncertaintyCharacterization):
            deterministic_distributions[name] = DeterministicCharacterization(
                name=name, value=distribution_def.distribution.mean()
            )
    return deterministic_distributions


class DeterministicCharacterization:
    """Generic Deterministic Parameter Class

    Parameters
    -----------
    name : str
    value : float
    """
    def __init__(self,
                 name:str,
                 value:float):
        self.name = name
        self.value = value

    def __str__(self):
        return f'{self.name} is a deterministic variable with value {self.value}'

    def __repr__(self):
        return f'{self.name}, deterministic, value={self.value}'

    def generate_values(self, sample_size):
        """
        Function to generate deterministic samples (same values)
        """
        return np.ones(sample_size)*self.value

    def plot_distribution(self, alternative_name=False):
        """
        Function to create point plot of deterministic value
        """
        name = alternative_name if alternative_name else self.name
        _, ax = plt.subplots(figsize=(4, 4))
        ax.plot(self.value, 0, 'ks')
        ax.grid()
        ax.set_xlabel(name)
        ax.set_yticks([])
        ax.set_xticks([self.value])


class UncertaintyCharacterization:
    """Generic Uncertainty Distribution Class

    Parameters
    ------------
    name : str
    uncertainty_type : str
    distribution : str
    parameters : dict
    """
    def __init__(self,
                 name:str,
                 uncertainty_type:str,
                 distribution:str,
                 parameters:dict):
        self.name = name
        self.uncertainty_type = uncertainty_type
        self.distribution = distribution(**parameters)
        self.parameters = parameters

    def __str__(self):
        return f'{self.name} is a probabilistic variable represented with a ' +\
               f'{self.distribution.dist.name} distribution and parameters {str(self.parameters)}'

    def __repr__(self):
        repr_part1 = (f'{self.name}, {self.distribution.dist.name}, ' +
                      f'mean = {self.distribution.mean()}, ' +
                      f'{self.uncertainty_type} uncertainty, ' +
                      f'parameters: ')
        repr_part2 = ', '.join("{}={}".format(*i) for i in self.parameters.items())
        return repr_part1 + repr_part2

    def generate_samples(self,
                         sample_size:int,
                         random_state=np.random.default_rng()):
        """Function to sample from uncertainty distributions

        Parameters
        ------------
            sample_size : int
                number of samples
            random_state : generator
                np default_rng instance or will be used to create randomState instance
        """
        return self.distribution.rvs(size=sample_size,
                                     random_state=random_state)

    def plot_distribution(self,
                          alternative_name=False,
                          plot_limits=False):
        """Function to create plot of uncertainty distribution"""
        name = alternative_name if alternative_name else self.name
        _, ax = plt.subplots(figsize=(4, 4))
        if not plot_limits:
            plot_spread = self.distribution.std()*3
            plot_limits = (self.distribution.mean() - plot_spread,
                        self.distribution.mean() + plot_spread)
        x_points = np.linspace(plot_limits[0], plot_limits[1], 100)
        y_points = self.distribution.pdf(x_points)

        ax.plot(x_points, y_points)
        ax.grid()
        ax.set_xlabel(name)
        ax.set_ylabel('PDF')


class BetaDistribution(UncertaintyCharacterization):
    """Beta Distribution Uncertainty Class

    Parameters
    ----------
    name : str
    uncertainty_type : str
    a : float
    b : float
    loc : float
    scale : float
    """
    def __init__(self,
                 name:str,
                 uncertainty_type:str,
                 a:float,
                 b:float,
                 loc:float=0,
                 scale:float=1):
        parameters = {'a': a,
                      'b': b,
                      'loc': loc,
                      'scale': scale}
        super().__init__(name=name,
                         uncertainty_type=uncertainty_type,
                         distribution=sps.beta,
                         parameters=parameters)


class NormalDistribution(UncertaintyCharacterization):
    """Normal Distribution Uncertainty Class

    Parameters
    ----------
    name:str
    uncertainty_type:str
    mean:float
    std_deviation:float
    """
    def __init__(self,
                 name:str,
                 uncertainty_type:str,
                 mean:float,
                 std_deviation:float):
        parameters = {'loc': mean,
                      'scale': std_deviation}
        super().__init__(name=name,
                         uncertainty_type=uncertainty_type,
                         distribution=sps.norm,
                         parameters=parameters)


class LognormalDistribution(UncertaintyCharacterization):
    """Log-Normal Distribution Uncertainty Class

    Parameters
    -----------
    name : str
    uncertainty_type : str
    mu : float
    sigma : float
    """
    def __init__(self,
                 name:str,
                 uncertainty_type:str,
                 mu:float,
                 sigma:float):
        parameters = {'scale': np.exp(mu),
                      's': sigma}
        super().__init__(name=name,
                         uncertainty_type=uncertainty_type,
                         distribution=sps.lognorm,
                         parameters=parameters)


class TruncatedNormalDistribution(UncertaintyCharacterization):
    """Truncated Normal Distribution Uncertainty Class

    Parameters
    ------------
    name : str
    uncertainty_type : str
    mean : float
    std_deviation : float
    lower_bound : float
    upper_bound : float
    """
    def __init__(self,
                 name:str,
                 uncertainty_type:str,
                 mean:float,
                 std_deviation:float,
                 lower_bound:float,
                 upper_bound:float):
        parameters = {'loc': mean,
                      'scale': std_deviation,
                      'a': (lower_bound - mean)/std_deviation,
                      'b': (upper_bound - mean)/std_deviation}
        super().__init__(name=name,
                         uncertainty_type=uncertainty_type,
                         distribution=sps.truncnorm,
                         parameters=parameters)


class TruncatedLognormalDistribution(UncertaintyCharacterization):
    """
    Truncated Lognormal Distribution Uncertainty Class

    Parameters
    ------------
    name : str
    uncertainty_type : str
    mu : float
    sigma : float
    lower_bound : float
    upper_bound : float
    """
    def __init__(self,
                 name:str,
                 uncertainty_type:str,
                 mu:float,
                 sigma:float,
                 lower_bound:float,
                 upper_bound:float):
        self.upper_bound = upper_bound
        self.lower_bound = lower_bound
        parameters = {'loc': mu,
                      'scale': sigma,
                      'a': (np.log(lower_bound) - mu)/sigma,
                      'b': (np.log(upper_bound) - mu)/sigma}
        super().__init__(name=name,
                         uncertainty_type=uncertainty_type,
                         distribution=sps.truncnorm,
                         parameters=parameters)

    def generate_samples(self,
                         sample_size:int,
                         random_state=np.random.default_rng()):
        """
        Function to sample from a lognormal uncertainty distribution.
        Scipy Stats library does not contain a truncated lognormal distribution,
        so this sampling function corrects for the use of the truncated normal distribution.

        Parameters
        ------------
            sample_size: int
                number of samples
            random_state: generator
                np default_rng instance or will be used to create randomState instance
        """
        normal_samples =  self.distribution.rvs(size=sample_size,
                                                random_state=random_state)
        return np.exp(normal_samples)

    def plot_distribution(self,
                          alternative_name=False,
                          plot_limits=False):
        """
        Function to create plot of a lognormal uncertainty distribution.
        Scipy Stats library does not contain a truncated lognormal
        distribution, so this plotting function corrects for the use of the
        truncated normal distribution.
        """
        name = alternative_name if alternative_name else self.name
        _, ax = plt.subplots(figsize=(4, 4))
        if not plot_limits:
            plot_limits = (self.lower_bound*.9, self.upper_bound*1.1)
        x_points = np.linspace(plot_limits[0], plot_limits[1], 100)
        y_points = self.distribution.pdf(np.log(x_points))/(x_points)

        ax.plot(x_points, y_points)
        ax.grid()
        ax.set_xlabel(name)
        ax.set_ylabel('PDF')


class UniformDistribution(UncertaintyCharacterization):
    """Uniform Distribution Uncertainty Class

    Parameters
    ------------
    name : str
    uncertainty_type : str
    lower_bound : float
    upper_bound : float
    """
    def __init__(self,
                 name:str,
                 uncertainty_type:str,
                 lower_bound:float,
                 upper_bound:float):
        if lower_bound > upper_bound:
            raise ValueError(f'parameter {name} lower bound {lower_bound}'
                             f' is greater the upper bound {upper_bound}')

        parameters = {'loc': lower_bound,
                      'scale': upper_bound - lower_bound}
        super().__init__(name=name,
                         uncertainty_type=uncertainty_type,
                         distribution=sps.uniform,
                         parameters=parameters)
