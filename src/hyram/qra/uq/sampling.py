"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np
from scipy.stats import qmc

from .distributions import (UncertaintyCharacterization,
                           DeterministicCharacterization)

""" module for sampling uncertainty definitions """


class UncertaintyStudy:
    """
    Generic Uncertainty Study Class

    Parameters
    ------------
    num_aleatory_samples:int
    num_epistemic_samples:int
    random_seed
    """
    def __init__(self,
                 num_aleatory_samples:int,
                 num_epistemic_samples:int,
                 random_seed):

        self.num_aleatory_samples = num_aleatory_samples
        self.num_epistemic_samples = num_epistemic_samples
        self.aleatory_variables = {}
        self.epistemic_variables = {}
        self.deterministic_variables = {}
        self.sample_sheet = {}
        self.total_sample_size = None
        self.random_seed = random_seed

    def get_parameter_names(self):
        """function to both deterministic and probabilistic variable names"""
        return (list(self.deterministic_variables.keys())
              + list(self.aleatory_variables.keys())
              + list(self.epistemic_variables.keys()))

    def get_uncertain_parameter_names(self):
        """function to determine total number of uncertain variables in study"""
        return (list(self.aleatory_variables.keys())
              + list(self.epistemic_variables.keys()))

    def add_variables(self, input_parameters):
        """
        Function to add all uncertain variables to uncertainty study
        """
        distributions = []
        deterministic = []
        for parameter_name, parameter_value in input_parameters.items():
            if isinstance(parameter_value, UncertaintyCharacterization):
                distributions.append(parameter_value)
            elif isinstance(parameter_value, DeterministicCharacterization):
                deterministic.append(parameter_value)
            else:
                raise ValueError(
                    f"input parameter {parameter_name} must be specified as " +
                     "uncertainty distribution or as deterministic")

        self.add_probabilistic_variables(distributions)
        self.add_deterministic_variables(deterministic)
        self.relevant_error_checks()

    def relevant_error_checks(self):
        """function to perform error checks on specified study details"""
        self.check_unique_var_names()
        self.check_distribution_if_samples_present()

    def add_probabilistic_variables(self, distribution_set):
        """function to add probabilistic variables to study"""
        for variable_distribution in distribution_set:
            if variable_distribution.uncertainty_type == 'aleatory':
                self.add_variable_name(variable_distribution,
                                       self.aleatory_variables)
            elif variable_distribution.uncertainty_type == 'epistemic':
                self.add_variable_name(variable_distribution,
                                       self.epistemic_variables)
            else:
                raise ValueError(f"{variable_distribution.name} not " +
                                 "specified as aleatory or epistemic")


    def add_deterministic_variables(self, deterministic_set):
        """function to add deterministic variables to study"""
        for variable in deterministic_set:
            self.add_variable_name(variable, self.deterministic_variables)

    def check_unique_var_names(self):
        """function to check that all variables have unique names"""
        shared_uncertainty_keys = \
            set(self.aleatory_variables).intersection(self.epistemic_variables)
        if shared_uncertainty_keys:
            raise ValueError(
                f"{shared_uncertainty_keys} uncertain variables specified " +
                 "for both uncertainty types")

        uncertainty_keys = set(self.aleatory_variables
                               ).union(set(self.epistemic_variables))
        shared_keys = set(self.deterministic_variables
                          ).intersection(uncertainty_keys)
        if shared_keys:
            raise ValueError(f"{shared_keys} variables specified " +
                             "for both uncertain and deterministic")

    def check_distribution_and_sample_size(self):
        """
        function to check that both an uncertainty distribution exists
        and sample size has been specified, not one or the other
        """
        has_aleatory_variables = bool(self.aleatory_variables)
        has_epistemic_variables = bool(self.epistemic_variables)
        has_aleatory_samples = self.num_aleatory_samples > 0
        has_epistemic_samples = self.num_epistemic_samples > 0

        # Throw error if variables or samples are specified without the other
        if has_aleatory_variables ^ has_aleatory_samples:
            if not has_aleatory_variables:
                raise ValueError(
                    'A number of aleatory samples is defined without ' +
                    'aleatory variables')
            elif not has_aleatory_samples:
                raise ValueError(
                    'Aleatory variables are defined without a number of ' +
                    'aleatory samples')
        if has_epistemic_variables ^ has_epistemic_samples:
            if not has_epistemic_variables:
                raise ValueError(
                    'A number of epistemic samples is defined without ' +
                    'epistemic variables')
            elif not has_epistemic_samples:
                raise ValueError(
                    'Epistemic variables are defined without a number of ' +
                    'epistemic samples')

    def check_distribution_if_samples_present(self):
        """
        function to check that an uncertainty distribution exists
        IF a sample size chas been specified (less restrictive than
        `check_distribution_and_sample_size`, which also checks for a sample
        size if an uncertainty distribution is defined)
        """
        has_aleatory_variables = bool(self.aleatory_variables)
        has_epistemic_variables = bool(self.epistemic_variables)
        has_aleatory_samples = self.num_aleatory_samples > 0
        has_epistemic_samples = self.num_epistemic_samples > 0

        # Throw error if variables are specified but no samples are
        if has_aleatory_samples and not has_aleatory_variables:
            raise ValueError(
                    'A number of aleatory samples is defined without ' +
                    'aleatory variables')
        if has_epistemic_samples and not has_epistemic_variables:
            raise ValueError(
                    'A number of epistemic samples is defined without ' +
                    'epistemic variables')

    @staticmethod
    def add_variable_name(variable_distribution, location):
        """
        function to ensure two variables don't have the same name
        """
        if location.setdefault(variable_distribution.name,
                               variable_distribution) != variable_distribution:
            raise ValueError('Two variables with same name')

    def generate_lhs_samples(self,
                             num_variables:int,
                             num_samples:int,
                             optimization_method="random-cd"):
        """
        function to generate Latin hypercube samples (LHS) using
        Scipy's Quasi-Monte Carlo submodule

        Parameters
        -----------
            num_variables: int
                 how many variables to generate samples for
            num_samples: int
                 how many LHS samples to generate

        Returns
        --------
                lhs_samples (np.array(num_samples, num_variables))
                - LHS samples
        """
        if num_variables == 1:
            optimization_method = None

        lhs_sampler = qmc.LatinHypercube(d=num_variables,
                                         seed=self.random_seed,
                                         optimization=optimization_method)
        lhs_samples = lhs_sampler.random(n=num_samples)
        return lhs_samples

    def add_aleatory_samples_to_sample_sheet(self, samples):
        """
        function to add aleatory variable samples to uncertainty study sample sheet
        """
        if self.num_epistemic_samples == 0:
            num_epistemic_samples = 1
        else:
            num_epistemic_samples = self.num_epistemic_samples

        for var_name, var_values in samples.items():
            self.sample_sheet[var_name] = np.array(
                var_values.tolist()*num_epistemic_samples)

    def add_epistemic_samples_to_sample_sheet(self, samples):
        """
        function to add epistemic variable samples to uncertainty study sample sheet
        """
        if self.num_aleatory_samples == 0:
            num_aleatory_samples = 1
        else:
            num_aleatory_samples = self.num_aleatory_samples

        for var_name, var_values in samples.items():
            self.sample_sheet[var_name] = np.array(
                [[var_value]*num_aleatory_samples
                 for var_value in var_values]).flatten()

    def add_deterministic_values_to_sample_sheet(self, parameter, sample_size):
        """
        function to add deterministic samples to sample sheet
        """
        for var_name, var_values in parameter.items():
            self.sample_sheet[var_name] = var_values.generate_values(sample_size)

    def calc_total_num_uncertain_variables(self):
        """
        function to determine total number of uncertain variables in uncertainty study
        """
        aleatory_variables = self.calc_num_variables(self.aleatory_variables)
        epistemic_variables = self.calc_num_variables(self.epistemic_variables)
        return aleatory_variables + epistemic_variables

    @staticmethod
    def calc_num_variables(variable_dict):
        """
        function to calculate the number of variables in a dictionary
        """
        return len(variable_dict)

    def get_total_double_loop_sample_size(self):
        """
        function to get sample size for double loop style uncertainty study
        """
        return max(self.num_aleatory_samples, 1)*max(self.num_epistemic_samples, 1)


class RandomStudy(UncertaintyStudy):
    """
    Random Sampling Uncertainty Study Class

    Parameters
    ------------
    num_aleatory_samples:int
    num_epistemic_samples:int
    random_seed
    """
    def relevant_error_checks(self):
        """function to perform error checks on specified study details"""
        self.check_unique_var_names()
        self.check_distribution_and_sample_size()

    def collect_samples(self, variable_distribution_dict, num_samples):
        """
        function to collect variable samples for random sampling study
        """
        samples = {}
        for var_name, var_dist in variable_distribution_dict.items():
            samples[var_name] = var_dist.generate_samples(num_samples,
                                                          self.random_seed)
        return samples

    def create_variable_sample_sheet(self):
        """
        function to create sample sheet and add samples for random sampling study
        """
        sample_func = self.collect_samples
        aleatory_samples = sample_func(self.aleatory_variables,
                                       self.num_aleatory_samples)
        epistemic_samples = sample_func(self.epistemic_variables,
                                        self.num_epistemic_samples)

        self.add_aleatory_samples_to_sample_sheet(aleatory_samples)
        self.add_epistemic_samples_to_sample_sheet(epistemic_samples)
        self.total_sample_size = max(self.num_aleatory_samples, 1)*\
            max(self.num_epistemic_samples, 1)
        self.add_deterministic_values_to_sample_sheet(self.deterministic_variables,
                                                      self.total_sample_size)

        return self.sample_sheet


class LHSStudy(RandomStudy):
    """
    LHS Uncertainty Study Class

    Parameters
    ------------
    num_aleatory_samples:int
    num_epistemic_samples:int
    random_seed
    """
    def collect_samples(self, variable_distribution_dict, num_samples):
        """
        function to collect variable samples for LHS study
        """
        samples = {}
        num_variables = self.calc_total_num_uncertain_variables()
        equal_probability_cdf_pts = self.generate_lhs_samples(num_variables,
                                                              num_samples)
        for i, (var_name, var_dist) in enumerate(variable_distribution_dict.items()):
            samples[var_name] = var_dist.distribution.ppf(equal_probability_cdf_pts[:, i])

        return samples
