"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest
from copy import deepcopy

import numpy as np

from hyram.qra.uq import sampling
from hyram.qra.uq import distributions

class SamplingTestCase(unittest.TestCase):
    """Class for unit tests of sampling module"""

    def setUp(self):
        """ function to specify common sampling inputs """
        self.parameters = {}
        self.parameters['var_a1'] = \
            distributions.NormalDistribution(name='var_a1',
                                           mean=.5,
                                           std_deviation=2,
                                           uncertainty_type='aleatory')
        self.parameters['var_a2'] = \
            distributions.NormalDistribution(name='var_a2',
                                           mean=.57,
                                           std_deviation=1,
                                           uncertainty_type='aleatory')
        self.parameters['var_e1'] = \
            distributions.UniformDistribution(name='var_e1',
                                            lower_bound=0,
                                            upper_bound=1,
                                            uncertainty_type='epistemic')
        self.parameters['var_e2'] = \
            distributions.UniformDistribution(name='var_e2',
                                            lower_bound=-1,
                                            upper_bound=3,
                                            uncertainty_type='epistemic')
        self.parameters['var_e3'] = \
            distributions.NormalDistribution(name='var_e3',
                                           mean=.5,
                                           std_deviation=4,
                                           uncertainty_type='epistemic')

        self.parameters['var_d1'] = \
            distributions.DeterministicCharacterization(name='var_d1',
                                                        value=42)
        self.num_aleatory_samples = 2
        self.num_epistemic_samples = 3
        self.random_seed = 123
        self.uncertainty_study = sampling.UncertaintyStudy(
            num_aleatory_samples=self.num_aleatory_samples,
            num_epistemic_samples=self.num_epistemic_samples,
            random_seed=self.random_seed)

        self.samples = {'sample1': np.array([1, 11, 2, 22, 3], dtype=float),
                        'sample2': np.array([4, 44, 5, 55, 6], dtype=float)}

    def test_get_parameter_names(self):
        self.uncertainty_study.add_variables(self.parameters)
        exp_parameter_names = ['var_d1', 'var_a1', 'var_a2',
                               'var_e1', 'var_e2', 'var_e3']
        test_parameter_names = self.uncertainty_study.get_parameter_names()

        self.assertListEqual(test_parameter_names, exp_parameter_names)

    def test_get_uncertain_parameter_names(self):
        self.uncertainty_study.add_variables(self.parameters)
        exp_parameter_names = ['var_a1', 'var_a2',
                               'var_e1', 'var_e2', 'var_e3']
        test_parameter_names = (
            self.uncertainty_study.get_uncertain_parameter_names())

        self.assertListEqual(test_parameter_names, exp_parameter_names)

    def test_add_variables_check_keys(self):
        self.uncertainty_study.add_variables(self.parameters)
        exp_aleatory_keys = ['var_a1', 'var_a2']
        exp_epistemic_keys = ['var_e1', 'var_e2', 'var_e3']
        exp_deterministic_keys = ['var_d1']

        self.assertListEqual(
            list(self.uncertainty_study.aleatory_variables.keys()),
            exp_aleatory_keys)
        self.assertListEqual(
            list(self.uncertainty_study.epistemic_variables.keys()),
            exp_epistemic_keys)
        self.assertListEqual(
            list(self.uncertainty_study.deterministic_variables.keys()),
            exp_deterministic_keys)

    def test_add_variables_check_value_types(self):
        self.uncertainty_study.add_variables(self.parameters)
        exp_aleatory_types = [distributions.NormalDistribution] * 2
        exp_epistemic_types = [distributions.UniformDistribution,
                               distributions.UniformDistribution,
                               distributions.NormalDistribution]
        exp_deterministic_types = [distributions.DeterministicCharacterization]
        self.assertListEqual(
            [type(aleatory_var) for aleatory_var
             in self.uncertainty_study.aleatory_variables.values()],
            exp_aleatory_types)
        self.assertListEqual(
            [type(epistemic_var) for epistemic_var
             in self.uncertainty_study.epistemic_variables.values()],
            exp_epistemic_types)
        self.assertListEqual(
            [type(deterministic_var) for deterministic_var
             in self.uncertainty_study.deterministic_variables.values()],
            exp_deterministic_types)

    def test_add_variables_bad_parameter(self):
        study = sampling.UncertaintyStudy(
            num_aleatory_samples=self.num_aleatory_samples,
            num_epistemic_samples=self.num_epistemic_samples,
            random_seed=self.random_seed)
        bad_parameter = {'bad': 'not a distribution'}
        parameters_w_bad = deepcopy(self.parameters)
        parameters_w_bad.update(bad_parameter)

        with self.assertRaises(ValueError) as error_msg:
            study.add_variables(parameters_w_bad)

        self.assertEqual(str(error_msg.exception),
                         'input parameter bad must be specified as ' +
                         'uncertainty distribution or as deterministic')

    def test_check_unique_var_names_aleatory_w_epistemic(self):
        var1 = distributions.NormalDistribution(name='var',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='aleatory')
        var2 = distributions.NormalDistribution(name='var',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='epistemic')

        with self.assertRaises(ValueError) as error_msg:
            self.uncertainty_study.add_variables(
                {'var_a1': var1,
                 'var_e1': var2})
        self.assertEqual(str(error_msg.exception),
                         r"{'var'} uncertain variables specified for " +
                         "both uncertainty types")

    def test_check_unique_var_names_uncertain_w_deterministic(self):
        var1 = distributions.NormalDistribution(name='var',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='aleatory')
        var2 = distributions.DeterministicCharacterization(name='var',
                                                           value=.5)

        with self.assertRaises(ValueError) as error_msg:
            self.uncertainty_study.add_variables(
                {'var_a1': var1,
                 'var_d1': var2})
        self.assertEqual(str(error_msg.exception),
                         r"{'var'} variables specified for both uncertain " +
                         "and deterministic")

    def test_check_distribution_and_sample_size_no_dist(self):
        var_a1 = distributions.NormalDistribution(name='var_a1',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='aleatory')
        var_e1 = distributions.NormalDistribution(name='var_e1',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='epistemic')
        random_study = sampling.RandomStudy(
            num_aleatory_samples=1,
            num_epistemic_samples=1,
            random_seed=self.random_seed)

        with self.assertRaises(ValueError) as error_msg:
            study = deepcopy(random_study)
            study.add_variables({'var_a1': var_a1})
        self.assertEqual(str(error_msg.exception),
                         'A number of epistemic samples is defined ' +
                         'without epistemic variables')

        with self.assertRaises(ValueError) as error_msg:
            study = deepcopy(random_study)
            study.add_variables({'var_e1': var_e1})
        self.assertEqual(str(error_msg.exception),
                         'A number of aleatory samples is defined ' +
                         'without aleatory variables')

    def test_check_distribution_and_sample_size_no_sample_size(self):
        var_a1 = distributions.NormalDistribution(name='var_a1',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='aleatory')
        var_e1 = distributions.NormalDistribution(name='var_e1',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='epistemic')

        random_study_no_aleatory = sampling.RandomStudy(
            num_aleatory_samples=0,
            num_epistemic_samples=1,
            random_seed=self.random_seed)

        random_study_no_epistemic = sampling.RandomStudy(
            num_aleatory_samples=1,
            num_epistemic_samples=0,
            random_seed=self.random_seed)

        with self.assertRaises(ValueError) as error_msg:
            random_study_no_aleatory.add_variables({'var_a1': var_a1,
                                                    'var_e1': var_e1})
        self.assertEqual(str(error_msg.exception),
                         'Aleatory variables are defined without ' +
                         'a number of aleatory samples')

        with self.assertRaises(ValueError) as error_msg:
            random_study_no_epistemic.add_variables({'var_a1': var_a1,
                                                     'var_e1': var_e1})
        self.assertEqual(str(error_msg.exception),
                         'Epistemic variables are defined without ' +
                         'a number of epistemic samples')

    def test_check_distribution_if_samples_present(self):
        var_a1 = distributions.NormalDistribution(name='var_a1',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='aleatory')
        var_e1 = distributions.NormalDistribution(name='var_e1',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='epistemic')
        uncertainty_study = sampling.UncertaintyStudy(
            num_aleatory_samples=1,
            num_epistemic_samples=1,
            random_seed=self.random_seed)

        with self.assertRaises(ValueError) as error_msg:
            study = deepcopy(uncertainty_study)
            study.add_variables({'var_a1': var_a1})
        self.assertEqual(str(error_msg.exception),
                         'A number of epistemic samples is defined ' +
                         'without epistemic variables')

        with self.assertRaises(ValueError) as error_msg:
            study = deepcopy(uncertainty_study)
            study.add_variables({'var_e1': var_e1})
        self.assertEqual(str(error_msg.exception),
                         'A number of aleatory samples is defined ' +
                         'without aleatory variables')

    def test_add_variable_name_default(self):
        var_a1 = distributions.NormalDistribution(name='var_a1',
                                                mean=.5,
                                                std_deviation=2,
                                                uncertainty_type='aleatory')
        exp_var_dict = {'var_a1': var_a1}
        self.uncertainty_study.add_variable_name(
            var_a1, self.uncertainty_study.aleatory_variables)
        self.assertDictEqual(self.uncertainty_study.aleatory_variables,
                             exp_var_dict)

    def test_add_variable_name_duplicate(self):
        var_a1 = distributions.NormalDistribution(
            name='var_a1',
            mean=.5,
            std_deviation=2,
            uncertainty_type='aleatory')
        var_a1_repeat = distributions.UniformDistribution(
            name='var_a1',
            lower_bound=0,
            upper_bound=1,
            uncertainty_type='aleatory')
        self.uncertainty_study.add_variable_name(
            var_a1, self.uncertainty_study.aleatory_variables)
        with self.assertRaises(ValueError) as error_msg:
            self.uncertainty_study.add_variable_name(
            var_a1_repeat, self.uncertainty_study.aleatory_variables)
        self.assertEqual(str(error_msg.exception),
                         'Two variables with same name')

    def test_generate_lhs_samples(self):
        num_variables = 3
        num_samples = 4
        test_lhs_samples = self.uncertainty_study.generate_lhs_samples(
            num_variables, num_samples)
        exp_lhs_sample_size = num_variables * num_samples
        exp_lhs_sample_shape = (4, 3)
        self.assertEqual(np.size(np.unique(test_lhs_samples)),
                         exp_lhs_sample_size)
        self.assertEqual(np.shape(test_lhs_samples),
                         exp_lhs_sample_shape)

    def test_add_aleatory_samples_to_sample_sheet_no_epistemic(self):
        uncertainty_study = deepcopy(self.uncertainty_study)
        uncertainty_study.num_epistemic_samples = 0
        uncertainty_study.add_aleatory_samples_to_sample_sheet(self.samples)
        test_sample_sheet_values = list(
            self.uncertainty_study.sample_sheet.values())
        exp_sample_sheet_values = list(self.samples.values())

        self.assertTrue(all(
            [np.allclose(test_sample, exp_sample) for (test_sample, exp_sample)
             in zip(test_sample_sheet_values, exp_sample_sheet_values)]))

    def test_add_aleatory_samples_to_sample_sheet_w_epistemic(self):
        self.uncertainty_study.add_aleatory_samples_to_sample_sheet(
            self.samples)
        exp_sample_sheet = {
            'sample1': np.array([1, 11, 2, 22, 3,
                                 1, 11, 2, 22, 3,
                                 1, 11, 2, 22, 3], dtype=float),
            'sample2': np.array([4, 44, 5, 55, 6,
                                 4, 44, 5, 55, 6,
                                 4, 44, 5, 55, 6], dtype=float)}
        test_sample_sheet_values = list(
            self.uncertainty_study.sample_sheet.values())
        exp_sample_sheet_values = list(exp_sample_sheet.values())

        self.assertTrue(all(
            [np.allclose(test_sample, exp_sample) for (test_sample, exp_sample)
             in zip(test_sample_sheet_values, exp_sample_sheet_values)]))

    def test_add_epistemic_samples_to_sample_sheet_no_aleatory(self):
        uncertainty_study = deepcopy(self.uncertainty_study)
        uncertainty_study.num_aleatory_samples = 0
        uncertainty_study.add_epistemic_samples_to_sample_sheet(self.samples)
        test_sample_sheet_values = list(
            self.uncertainty_study.sample_sheet.values())
        exp_sample_sheet_values = list(self.samples.values())

        self.assertTrue(all(
            [np.allclose(test_sample, exp_sample) for (test_sample, exp_sample)
             in zip(test_sample_sheet_values, exp_sample_sheet_values)]))

    def test_add_epistemic_samples_to_sample_sheet_w_aleatory(self):
        self.uncertainty_study.add_epistemic_samples_to_sample_sheet(
            self.samples)
        exp_sample_sheet = {
            'sample1': np.array([1, 1, 11, 11, 2, 2, 22, 22, 3, 3],
                                dtype=float),
            'sample2': np.array([4, 4, 44, 44, 5, 5, 55, 55, 6, 6],
                                dtype=float)}
        test_sample_sheet_values = list(
            self.uncertainty_study.sample_sheet.values())
        exp_sample_sheet_values = list(exp_sample_sheet.values())

        self.assertTrue(all(
            [np.allclose(test_sample, exp_sample) for (test_sample, exp_sample)
             in zip(test_sample_sheet_values, exp_sample_sheet_values)]))

    def test_add_deterministic_values_to_sample_sheet(self):
        num_samples = 3
        det_dist1 = distributions.DeterministicCharacterization(
            name='var_d1', value=42)
        det_dist2 = distributions.DeterministicCharacterization(
            name='var_d2', value=43)
        det_variables = {'var_d1': det_dist1, 'var_d2': det_dist2}
        self.uncertainty_study.add_deterministic_values_to_sample_sheet(
            det_variables, num_samples)
        exp_sample_sheet = {
            'var_d1': np.array([42, 42, 42], dtype=float),
            'var_d2': np.array([43, 43, 43], dtype=float)}
        test_sample_sheet_values = list(
            self.uncertainty_study.sample_sheet.values())
        exp_sample_sheet_values = list(exp_sample_sheet.values())

        self.assertTrue(all(
            [np.allclose(test_sample, exp_sample) for (test_sample, exp_sample)
             in zip(test_sample_sheet_values, exp_sample_sheet_values)]))

    def test_calc_total_num_uncertain_variables(self):
        self.uncertainty_study.add_variables(self.parameters)
        test_total_num_uncertain_variables = (
            self.uncertainty_study.calc_total_num_uncertain_variables())
        exp_total_num_uncertain_variables = (
            self.num_aleatory_samples + self.num_epistemic_samples)

        self.assertEqual(test_total_num_uncertain_variables,
                         exp_total_num_uncertain_variables)

    def test_calc_num_variables(self):
        test_dict = {'a': 1, 'b': 2, 'c': 3}
        test_num_variables = self.uncertainty_study.calc_num_variables(
            test_dict)
        exp_num_variables = 3

        self.assertEqual(test_num_variables, exp_num_variables)

    def test_get_total_double_loop_sample_size_default(self):
        test_sample_size = (
            self.uncertainty_study.get_total_double_loop_sample_size())
        exp_sample_size = (self.num_aleatory_samples
                         * self.num_epistemic_samples)

        self.assertEqual(test_sample_size, exp_sample_size)

    def test_get_total_double_loop_sample_size_no_epistemic(self):
        uncertainty_study = deepcopy(self.uncertainty_study)
        uncertainty_study.num_epistemic_samples = 0
        test_sample_size = (
            uncertainty_study.get_total_double_loop_sample_size())
        exp_sample_size = self.num_aleatory_samples

        self.assertEqual(test_sample_size, exp_sample_size)

    def test_lhsstudy_collect_samples(self):
        self.lhs_study = sampling.LHSStudy(
            num_aleatory_samples=self.num_aleatory_samples,
            num_epistemic_samples=self.num_epistemic_samples,
            random_seed=self.random_seed)
        parameters = deepcopy(self.parameters)
        del parameters['var_d1']
        self.lhs_study.add_variables(parameters)
        num_samples = 3
        test_samples = self.lhs_study.collect_samples(
            parameters, num_samples)
        exp_samples_keys = ['var_a1', 'var_a2', 'var_e1', 'var_e2', 'var_e3']

        self.assertEqual(list(test_samples.keys()), exp_samples_keys)
        self.assertEqual(len(test_samples['var_a1']), num_samples)

    def test_lhs_random_seed_specification(self):
        """unit test for random seed specification"""
        samples = 5
        seed_1 = 123
        seed_2 = 321
        test_study_1 = sampling.LHSStudy(num_aleatory_samples=samples,
                                         num_epistemic_samples=samples,
                                         random_seed=seed_1)

        test_study_1.add_variables(input_parameters=self.parameters)
        study_samples_1 = test_study_1.create_variable_sample_sheet()

        test_study_2 = sampling.LHSStudy(num_aleatory_samples=samples,
                                         num_epistemic_samples=samples,
                                         random_seed=seed_1)
        test_study_2.add_variables(input_parameters=self.parameters)
        study_samples_2 = test_study_2.create_variable_sample_sheet()

        test_study_3 = sampling.LHSStudy(num_aleatory_samples=samples,
                                         num_epistemic_samples=samples,
                                         random_seed=seed_2)
        test_study_3.add_variables(input_parameters=self.parameters)
        study_samples_3 = test_study_3.create_variable_sample_sheet()

        self.assertIsNone(np.testing.assert_array_equal(
            study_samples_1['var_a1'], study_samples_2['var_a1']))
        self.assertIsNone(np.testing.assert_array_equal(
            study_samples_1['var_e1'], study_samples_2['var_e1']))

        with self.assertRaises(Exception):
            np.testing.assert_array_equal(
                study_samples_1['var_a2'], study_samples_3['var_a2'])

        with self.assertRaises(Exception):
            np.testing.assert_array_equal(
                study_samples_1['var_e2'], study_samples_3['var_e2'])


class RandomStudyTestCase(unittest.TestCase):
    """Class for unit tests of RandomStudy object"""
    def setUp(self):
        self.parameters = {}
        self.parameters['var_a1'] = \
            distributions.NormalDistribution(name='var_a1',
                                           mean=.5,
                                           std_deviation=2,
                                           uncertainty_type='aleatory')
        self.parameters['var_a2'] = \
            distributions.NormalDistribution(name='var_a2',
                                           mean=.57,
                                           std_deviation=1,
                                           uncertainty_type='aleatory')
        self.parameters['var_e1'] = \
            distributions.UniformDistribution(name='var_e1',
                                            lower_bound=0,
                                            upper_bound=1,
                                            uncertainty_type='epistemic')
        self.parameters['var_e2'] = \
            distributions.UniformDistribution(name='var_e2',
                                            lower_bound=-1,
                                            upper_bound=3,
                                            uncertainty_type='epistemic')
        self.parameters['var_e3'] = \
            distributions.NormalDistribution(name='var_e3',
                                           mean=.5,
                                           std_deviation=4,
                                           uncertainty_type='epistemic')

        self.parameters['var_d1'] = \
            distributions.DeterministicCharacterization(name='var_d1',
                                                        value=42)
        self.num_aleatory_samples = 2
        self.num_epistemic_samples = 3
        self.random_seed = 123
        self.random_study = sampling.RandomStudy(
            num_aleatory_samples=self.num_aleatory_samples,
            num_epistemic_samples=self.num_epistemic_samples,
            random_seed=self.random_seed)

    def test_collect_samples(self):
        parameters = deepcopy(self.parameters)
        del parameters['var_d1']
        self.random_study.add_variables(parameters)
        num_samples = 3
        test_samples = self.random_study.collect_samples(
            parameters, num_samples)
        exp_samples_keys = ['var_a1', 'var_a2', 'var_e1', 'var_e2', 'var_e3']

        self.assertEqual(list(test_samples.keys()), exp_samples_keys)
        self.assertEqual(len(test_samples['var_a1']), num_samples)

    def test_create_variable_sample_sheet_general_format(self):
        self.random_study.add_variables(input_parameters=self.parameters)
        test_samples = self.random_study.create_variable_sample_sheet()

        self.assertTrue(isinstance(test_samples, dict))
        self.assertEqual(len(test_samples), 6)

    def test_create_variable_sample_sheet_sample_size(self):
        self.random_study.add_variables(input_parameters=self.parameters)
        test_samples = self.random_study.create_variable_sample_sheet()
        sample_lens = [len(var) for var in test_samples.values()]
        exp_lens = [self.num_aleatory_samples*self.num_epistemic_samples] * 6

        self.assertEqual(sample_lens, exp_lens)

    def test_create_variable_sample_sheet_aleatory_format(self):
        self.random_study.add_variables(input_parameters=self.parameters)
        test_samples = self.random_study.create_variable_sample_sheet()

        self.assertEqual(test_samples['var_a2'][0],
                         test_samples['var_a2'][self.num_aleatory_samples])
        self.assertNotEqual(test_samples['var_a2'][0],
                            test_samples['var_a2'][self.num_aleatory_samples-1])
        self.assertEqual(len(np.unique(test_samples['var_a2'])),
                         self.num_aleatory_samples)

    def test_create_variable_sample_sheet_epistemic_format(self):
        self.random_study.add_variables(input_parameters=self.parameters)
        test_samples = self.random_study.create_variable_sample_sheet()

        self.assertEqual(test_samples['var_e3'][0],
                         test_samples['var_e3'][self.num_aleatory_samples-1])
        self.assertNotEqual(test_samples['var_e3'][0],
                            test_samples['var_e3'][self.num_aleatory_samples])
        self.assertEqual(len(np.unique(test_samples['var_e2'])),
                         self.num_epistemic_samples)

    def test_create_variable_sample_sheet_deterministic_format(self):
        self.random_study.add_variables(input_parameters=self.parameters)
        test_samples = self.random_study.create_variable_sample_sheet()
        exp_samples = np.ones(6) * 42

        self.assertTrue(np.allclose(test_samples['var_d1'], exp_samples))

    def test_monte_carlo_random_seed_specification(self):
        """unit test for random seed specification"""
        samples = 5
        seed_1 = 123
        seed_2 = 321
        test_study_1 = sampling.RandomStudy(
            num_aleatory_samples=samples,
            num_epistemic_samples=samples,
            random_seed=seed_1)

        test_study_1.add_variables(input_parameters=self.parameters)
        study_samples_1 = test_study_1.create_variable_sample_sheet()

        test_study_2 = sampling.RandomStudy(num_aleatory_samples=samples,
                                            num_epistemic_samples=samples,
                                            random_seed=seed_1)
        test_study_2.add_variables(input_parameters=self.parameters)
        study_samples_2 = test_study_2.create_variable_sample_sheet()

        test_study_3 = sampling.RandomStudy(num_aleatory_samples=samples,
                                            num_epistemic_samples=samples,
                                            random_seed=seed_2)
        test_study_3.add_variables(input_parameters=self.parameters)
        study_samples_3 = test_study_3.create_variable_sample_sheet()

        self.assertTrue(np.allclose(
            study_samples_1['var_a1'], study_samples_2['var_a1']))
        self.assertTrue(np.allclose(
            study_samples_1['var_e1'], study_samples_2['var_e1']))

        with self.assertRaises(Exception):
            np.testing.assert_array_equal(
                study_samples_1['var_a2'], study_samples_3['var_a2'])

        with self.assertRaises(Exception):
            np.testing.assert_array_equal(
                study_samples_1['var_e2'], study_samples_3['var_e2'])
