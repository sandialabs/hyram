"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

import numpy as np
import scipy.constants as spc

from hyram.qra.uq.sampling import RandomStudy
from hyram.qra.uq.distributions import (LognormalDistribution,
                                              BetaDistribution)
from hyram.qra.component_failure import ComponentFailure, ComponentFailureSet
from hyram.qra import uncertainty

OUTPUT_DIR = 'out'


class HydrogenTestCase(unittest.TestCase):
    """
    Test calculation of UQ framework.
    """
    def setUp(self):
        # General UQ variables
        self.study_type = 'random'
        self.rel_species = 'h2'
        self.rel_phase = None
        self.leak_sizes = [0.01, 0.1, 1, 10, 100]
        self.mass_flow_rates = [0.44, 4.4, 44., 440, 4400]
        self.uncertainty_type = 'aleatory'
        self.num_samples = 3
        self.num_vehicles = 3
        self.daily_fuelings = 2
        self.vehicle_days = 5
        self.static_parameters = {
            "pipe_inner_diam": 0.245 * spc.inch,  # m
            "amb_temp": 15 + spc.zero_Celsius,  # K
            "amb_pres": 101325,  # Pa
            "rel_temp": 15 + spc.zero_Celsius,  # K
            "rel_pres": 35  * spc.mega,  # Pa
            "rel_phase": self.rel_phase,
            "ign_probs": None,
            "occupant_hours": 2000,
            "ft_overrides": None,
            "rel_species": self.rel_species,
            "discharge_coeff": 1,
            "mass_flow_rates": None,
            "detection_credit": 0.9,
            "num_vehicles": 20,
            "daily_fuelings": 2,
            "vehicle_days": 250,
            "overp_method": 'bst',
            "tnt_factor": 0.03,
            "bst_flame_speed": 0.35,
            "probit_thermal_id": 'eise',
            "exposure_time": 60,  # s
            "probit_overp_id": 'head',
            "nozzle_model": 'yuce',
            "rel_angle": 0,
            "rel_humid": 0.89,
            "verbose": False,
            "output_dir": OUTPUT_DIR,
            "create_plots": False,
        }

        # Dummy distributions
        self.dummy_logn_dist_params = {
            'mu': 0.1,
            'sigma': 1
        }
        self.dummy_deter_dist_params = {
            'value': 5
        }
        self.dummy_beta_dist_params = {
            'a': 0.5,
            'b': 5e5
        }

        # Fueling failure variables
        self.fueling_failure_definitions = {
            'Fueling Failure: test_component1, mode1': {
                'name': 'Fueling Failure: test_component1, mode1',
                'distribution_type': 'log_normal',
                'mu': self.dummy_logn_dist_params['mu'],
                'sigma': self.dummy_logn_dist_params['sigma'],
                'uncertainty_type': self.uncertainty_type
            },
            'Fueling Failure: test_component1, mode2': {
                'name': 'Fueling Failure: test_component1, mode2',
                'distribution_type': 'beta',
                'a': self.dummy_beta_dist_params['a'],
                'b': self.dummy_beta_dist_params['b'],
                'uncertainty_type': self.uncertainty_type
            }
        }

        # Leak frequency variables
        self.leak_logn_dist_params = [
            {'mu': 0.1, 'sigma': 1},
            {'mu': 0.2, 'sigma': 2},
            {'mu': 0.3, 'sigma': 3},
            {'mu': 0.4, 'sigma': 4},
            {'mu': 0.5, 'sigma': 5}
        ]
        self.leak_mixed_dists = ['deterministic', 'beta', 'normal',
                                 'log_normal', 'uniform']
        self.leak_mixed_dist_params = [
            {'value': 0.01},
            {'a': 0.2, 'b': 200},
            {'mean': 0.03, 'std_deviation': 0.03},
            {'mu': 0.4, 'sigma': 4},
            {'lower_bound': 0.005, 'upper_bound': 0.5}
        ]
        self.component_parameters = {
            'logn_component': {
                'leak_sizes': self.leak_sizes,
                'mass_flow_rates': self.mass_flow_rates,
                'quantity': 3,
                'distribution_type': 'log_normal',
                'distribution_parameters': self.leak_logn_dist_params
            },
            'mixed_component': {
                'leak_sizes': self.leak_sizes,
                'quantity': 2,
                'distribution_type': self.leak_mixed_dists,
                'distribution_parameters': self.leak_mixed_dist_params
            }
        }

        # Occupant location variables
        self.occupant_location_parameters = {
            'mixed_group': {
                'quantity': 2,
                'distribution_type': ('beta', 'log_normal', 'deterministic'),
                'distribution_parameters': (self.dummy_beta_dist_params,
                                            self.dummy_logn_dist_params,
                                            self.dummy_deter_dist_params)
            },
            'beta_group': {
                'quantity': 1,
                'distribution_type': 'beta',
                'distribution_parameters': self.dummy_beta_dist_params
            }
        }

    def test_substitute_defaults(self):
        """Check that defaults are properly overwritten"""
        default_dict = {'a': 1, 'b': 2, 'c': 3}
        sub_dict = {'b': 4, 'c': 5}
        exp_dict = {'a': 1, 'b': 4, 'c': 5}
        test_dict = uncertainty.substitute_defaults(default_dict, sub_dict)

        self.assertDictEqual(test_dict, exp_dict)

    def test_add_dist_definitions(self):
        """Check that the distribution definitions are created with the
        proper format"""
        test_definitions = dict()
        names = ['def1', 'def2']
        dist_types = ['deterministic', 'beta']
        dist_parameters = [self.dummy_deter_dist_params,
                           self.dummy_beta_dist_params]
        uncertainty_type = self.uncertainty_type
        exp_definitions = {
            'def1': {
                'name': 'def1',
                'distribution_type': 'deterministic',
                'value': self.dummy_deter_dist_params['value'],
            },
            'def2': {
                'name': 'def2',
                'distribution_type': 'beta',
                'a': self.dummy_beta_dist_params['a'],
                'b': self.dummy_beta_dist_params['b'],
                'uncertainty_type': uncertainty_type
            }
        }
        test_definitions = uncertainty.add_dist_definitions(
            test_definitions, names,
            dist_types, dist_parameters, uncertainty_type)

        self.assertDictEqual(test_definitions, exp_definitions)

    def test_create_fueling_failure_dist_name(self):
        """Check to make sure the fueling failure distribution name has the
        proper format
        """
        failure_type = 'Accident'
        failure_mode = 'Driveoff'
        exp_name = 'Fueling Failure: Accident, Driveoff'
        test_name = uncertainty.create_fueling_failure_dist_name(
            failure_type, failure_mode)

        self.assertEqual(test_name, exp_name)

    def test_set_fueling_failure_definitions(self):
        """Check that specifications are properly converted to distribution
        definitions for fueling failures
        """
        fueling_failure_params = {
            'test_component1': {
                'mode': ['mode1', 'mode2'],
                'distribution_type': ['log_normal', 'beta'],
                'distribution_parameters': [self.dummy_logn_dist_params,
                                            self.dummy_beta_dist_params]
            }
        }
        test_definition = uncertainty.set_fueling_failure_definitions(
            fueling_failure_params,
            self.uncertainty_type
        )
        exp_definition = self.fueling_failure_definitions

        self.assertDictEqual(
            test_definition['Fueling Failure: test_component1, mode2'],
            exp_definition['Fueling Failure: test_component1, mode2'])

    def test_create_leak_freq_dist_name(self):
        """Check to make sure the leak frequency distribution name has the
        proper format
        """
        component = 'test_component'
        num = 1
        leak_size = 0.1
        mass_flow_rate = 5

        exp_name1 = 'Component: test_component #1, 0.1% leak'
        exp_name2 = 'Component: test_component #1, 0.1% leak, MFR=5 kg/s'

        test_name1 = uncertainty.create_leak_freq_dist_name(
            component, num, leak_size, None)
        test_name2 = uncertainty.create_leak_freq_dist_name(
            component, num, leak_size, mass_flow_rate)

        self.assertEqual(test_name1, exp_name1)
        self.assertEqual(test_name2, exp_name2)

    def test_set_leak_frequency_definitions_w_mfr(self):
        """Check that specifications are properly converted to distribution
        definitions for leaks with defined mass flow rates
        """
        test_def = uncertainty.set_leak_frequency_definitions(
            uncertain_parameters=self.component_parameters,
            uncertainty_type=self.uncertainty_type,
            species=self.rel_species,
            saturated_phase=self.rel_phase,
            include_defaults=False
        )
        exp_definition = {
                'name': 'Component: logn_component #1, 1% leak, MFR=44.0 kg/s',
                'distribution_type': 'log_normal',
                'mu': self.leak_logn_dist_params[2]['mu'],
                'sigma': self.leak_logn_dist_params[2]['sigma'],
                'uncertainty_type': self.uncertainty_type
        }

        self.assertDictEqual(
            test_def['Component: logn_component #1, 1% leak, MFR=44.0 kg/s'],
            exp_definition)

    def test_set_leak_frequency_definitions_w_mfr(self):
        """Check that specifications are properly converted to distribution
        definitions for leaks with defined mass flow rates
        """
        test_def = uncertainty.set_leak_frequency_definitions(
            uncertain_parameters=self.component_parameters,
            uncertainty_type=self.uncertainty_type,
            species=self.rel_species,
            saturated_phase=self.rel_phase,
            include_defaults=False
        )
        exp_definition = {
                'name': 'Component: mixed_component #1, 0.1% leak',
                'distribution_type': 'beta',
                'a': self.leak_mixed_dist_params[1]['a'],
                'b': self.leak_mixed_dist_params[1]['b'],
                'uncertainty_type': self.uncertainty_type
        }

        self.assertDictEqual(
            test_def['Component: mixed_component #1, 0.1% leak'],
            exp_definition)

    def test_create_occupant_location_dist_name(self):
        """Check to make sure the occupant location distribution name has
        the proper format
        """
        group_name = 'Group A'
        direction = 'y'
        number = 4
        exp_name = 'Occupant Location: Group A #4, y-direction'
        test_name = uncertainty.create_occupant_location_dist_name(
            group_name, direction, number)

        self.assertEqual(test_name, exp_name)

    def test_set_occupant_location_definitions_uniform_def(self):
        """Check that specifications are properly converted to distribution
        definitions for occupant location when a uniform definition is given
        for all x/y/z directions
        """
        test_definition = uncertainty.set_occupant_location_definitions(
            self.occupant_location_parameters,
            self.uncertainty_type
        )
        exp_definition = {
                'name': 'Occupant Location: beta_group #1, x-direction',
                'distribution_type': 'beta',
                'a': self.dummy_beta_dist_params['a'],
                'b': self.dummy_beta_dist_params['b'],
                'uncertainty_type': self.uncertainty_type
            }

        self.assertDictEqual(
            test_definition['Occupant Location: beta_group #1, x-direction'],
            exp_definition)

    def test_set_occupant_location_definitions_mixed_def(self):
        """Check that specifications are properly converted to distribution
        definitions for occupant location when separate definitions are given
        for the x/y/z directions
        """
        test_definition = uncertainty.set_occupant_location_definitions(
            self.occupant_location_parameters,
            self.uncertainty_type
        )
        exp_definition_y = {
                'name': 'Occupant Location: mixed_group #1, y-direction',
                'distribution_type': 'log_normal',
                'mu': self.dummy_logn_dist_params['mu'],
                'sigma': self.dummy_logn_dist_params['sigma'],
                'uncertainty_type': self.uncertainty_type
        }
        exp_definition_z = {
                'name': 'Occupant Location: mixed_group #2, z-direction',
                'distribution_type': 'deterministic',
                'value': self.dummy_deter_dist_params['value'],
        }

        self.assertDictEqual(
            test_definition['Occupant Location: mixed_group #1, y-direction'],
            exp_definition_y)
        self.assertDictEqual(
            test_definition['Occupant Location: mixed_group #2, z-direction'],
            exp_definition_z)

    def test_stack_locations_xyz(self):
        """Check that the separated x/y/z sampled parameters are correctly
        stacked into (x, y, z) format
        """
        locations = {
            'GroupA #1, x-direction': 1,
            'GroupA #1, y-direction': 11,
            'GroupA #1, z-direction': 6,
            'GroupA #2, x-direction': 1.5,
            'GroupA #2, y-direction': 11.5,
            'GroupA #2, z-direction': 5.75,
            'GroupB #1, x-direction': 20,
            'GroupB #1, y-direction': 15,
            'GroupB #1, z-direction': 5.9
        }
        test_stacked_locations = uncertainty.stack_locations_xyz(locations)
        exp_stacked_locations = [(1, 11, 6), (1.5, 11.5, 5.75), (20, 15, 5.9)]

        self.assertListEqual(test_stacked_locations, exp_stacked_locations)


    def test_generate_distributions(self):
        """Check that a dictionary of distributions is created"""
        test_dists = uncertainty.generate_distributions(
            definitions=self.fueling_failure_definitions,
            fix_to_mean=False
        )

        self.assertTrue(isinstance(
            test_dists['Fueling Failure: test_component1, mode1'],
            LognormalDistribution))
        self.assertTrue(isinstance(
            test_dists['Fueling Failure: test_component1, mode2'],
            BetaDistribution))

    def test_get_inputs_per_sample(self):
        """Check that the proper inputs are parsed from a list of samples
        for multiple parameters
        """
        samples = {'param1': np.array([1, 2, 3]),
                   'param2': np.array([4, 5, 6])}
        num_samples = 3
        exp_inputs_per_sample = [{'param1': 1, 'param2': 4},
                                 {'param1': 2, 'param2': 5},
                                 {'param1': 3, 'param2': 6}]
        test_inputs_per_sample = uncertainty.get_inputs_per_sample(
            samples, num_samples)

        self.assertListEqual(test_inputs_per_sample, exp_inputs_per_sample)

    def test_parse_uncertain_parameter_type(self):
        """Check that the generalized parsing returns the proper keys"""
        params = {'Type1: optionA': 1,
                  'Type1: optionB': 2,
                  'Type2: option1': 'a',
                  'Type2: option2': 'b'}
        type = 'Type2'
        test_parsed_params = uncertainty.parse_uncertain_parameter_type(
            params, type)
        exp_parsed_params = {'Type2: option1': 'a',
                             'Type2: option2': 'b'}

        self.assertDictEqual(test_parsed_params, exp_parsed_params)

    def test_parse_fueling_failures(self):
        """Check that the parameters for fueling failures are parsed into
        its subcomponents correctly
        """
        parameters = {'Fueling Failure: Manual Valve, Failure to close': 0.01,
                      'Fueling Failure: Accident, Driveoff': 0.02,
                      'Different Uncertain Parameter': 0.03}
        exp_types = ['Manual Valve', 'Accident']
        exp_modes = ['Failure to close', 'Driveoff']
        exp_probs = [0.01, 0.02]
        (test_types,
         test_modes,
         test_probs) = uncertainty.parse_fueling_failures(parameters)

        self.assertEqual(exp_types, test_types)
        self.assertEqual(exp_modes, test_modes)
        self.assertEqual(exp_probs, test_probs)

    def test_parse_components(self):
        """Check that the parameters for leak frequencies are parsed into
        its subcomponents correctly
        """
        parameters = {'Component: Compressor #1, 0.1% leak': 0.01,
                      'Component: Compressor #1, 1% leak': 0.02,
                      'Component: Pipe #1, 0.1% leak, MFR=0.4 kg/s': 0.03,
                      'Different Uncertain Parameter': 0.04}
        exp_categories = ['Compressor', 'Pipe']
        exp_counts = [1, 1]
        exp_leak_sizes = np.array([np.array([0.1, 1]),
                                   np.array([0.1])], dtype=object)
        exp_frequencies = np.array([np.array([0.01, 0.02]),
                                    np.array([0.03])], dtype=object)
        exp_mass_flow_rates = np.array([None,
                                        np.array([0.4])], dtype=object)
        (test_categories,
         test_counts,
         test_leak_sizes,
         test_frequencies,
         test_mass_flow_rates) = uncertainty.parse_components(parameters)

        self.assertTrue(np.array_equal(test_categories, exp_categories))
        self.assertTrue(np.array_equal(test_counts, exp_counts))
        self.assertTrue(np.array_equal(test_leak_sizes[0], exp_leak_sizes[0]))
        self.assertTrue(np.array_equal(
            test_frequencies['Compressor'], exp_frequencies[0]))
        self.assertTrue(np.array_equal(
            test_mass_flow_rates[1], exp_mass_flow_rates[1]))

    def test_set_fueling_failures(self):
        """Check that the component failure set is properly created for
        given failure probabilities
        """
        uncertain_params = {
            'Fueling Failure: test_component1, mode1': 0.1,
            'Fueling Failure: test_component1, mode2': 0.2
        }
        test_failure_set = uncertainty.set_fueling_failures(
            uncertain_parameters=uncertain_params,
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days
        )
        exp_failure_set = ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=[ComponentFailure('test_component1', 'mode1', 0.1),
                      ComponentFailure('test_component1', 'mode2', 0.2)]
        )

        self.assertEqual(repr(test_failure_set), repr(exp_failure_set))

    def test_create_study(self):
        """Check that the properly study object is created with the correct
        number of samples
        """
        test_study = uncertainty.create_study(
            study_type=self.study_type,
            uncertainty_type=self.uncertainty_type,
            num_samples=self.num_samples
        )

        self.assertTrue(isinstance(test_study, RandomStudy))
        self.assertEqual(test_study.num_aleatory_samples, self.num_samples)

    def test_evaluate_qra_uq(self):
        """Check to ensure sampling study runs successfully"""
        component_parameters = {
            'Compressor': {
                'leak_sizes': self.leak_sizes,
                'quantity': 2,
                'distribution_type': self.leak_mixed_dists,
                'distribution_parameters': self.leak_mixed_dist_params
            }
        }
        fueling_parameters = {
            'Nozzle': {
                'mode': ['Pop-off', 'Failure to close'],
                'distribution_type': ['normal', 'deterministic'],
                'distribution_parameters': [
                    {'mean': 0.03, 'std_deviation': 0.01},
                    {'value': 0.002}
                ]
            },
        }
        location_parameters = self.occupant_location_parameters
        results, _ = uncertainty.evaluate_qra_uq(
            species=self.rel_species,
            study_type=self.study_type,
            num_samples=self.num_samples,
            fueling_parameters=fueling_parameters,
            component_parameters=component_parameters,
            location_parameters=location_parameters,
            static_parameters=self.static_parameters,
            uncertainty_type=self.uncertainty_type,
        )
        all_total_pll_values = []
        for result in results:
            all_total_pll_values.append(result['total_pll'])
        all_total_pll_values = np.array(all_total_pll_values)

        self.assertEqual(self.num_samples,
                         len(all_total_pll_values))
        self.assertEqual(self.num_samples,
                         len(np.unique(all_total_pll_values)))
