"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

import numpy as np

from hyram.qra import defaults


class TestGetDefaultIgnitionProbabilities(unittest.TestCase):
    """
    Tests for getting default ignition probabilities for different species
    """
    def setUp(self):
        self.h2_ignition_probs = {
            'flow_thresholds': [0.125, 6.25],  # kg/s
            'immed_ign_probs': [0.008, 0.053, 0.23],
            'delayed_ign_probs': [0.004, 0.027, 0.12]
        }
        self.non_h2_ignition_probs = {
            'flow_thresholds': [1, 50],  # kg/s
            'immed_ign_probs': [0.007, 0.047, 0.2],
            'delayed_ign_probs': [0.003, 0.023, 0.1]
        }

    def test_reject_bad_species_type(self):
        bad_species = ['h2', 'ch4']

        self.assertRaises(TypeError,
                          defaults.get_default_ignition_probs,
                          bad_species)

    def test_get_default_ign_probs_pure_h2(self):
        species = 'h2'
        default_ign_probs = defaults.get_default_ignition_probs(species)

        self.assertEqual(default_ign_probs, self.h2_ignition_probs)

    def test_get_default_ign_probs_pure_non_h2(self):
        species = 'ch4'
        default_ign_probs = defaults.get_default_ignition_probs(species)

        self.assertEqual(default_ign_probs, self.non_h2_ignition_probs)

    def test_get_default_ign_probs_h2_mix(self):
        species = {'h2': 0.1, 'ch4': 0.9}
        default_ign_probs = defaults.get_default_ignition_probs(species)

        self.assertEqual(default_ign_probs, self.h2_ignition_probs)

    def test_get_default_ign_probs_non_h2_mix(self):
        species = {'c3h8': 0.1, 'ch4': 0.9}
        default_ign_probs = defaults.get_default_ignition_probs(species)

        self.assertEqual(default_ign_probs, self.non_h2_ignition_probs)


class TestGetDefaultComponentParameters(unittest.TestCase):
    """
    Tests for getting default component parameters for different species
    """

    def setUp(self):
        self.def_components_gas_h2 = ['compressor', 'vessel', 'filter',
                                      'hose', 'joint', 'pipe',
                                      'valve', 'instrument']
        self.def_components_liq_h2 = ['vessel', 'flange', 'hose',
                                      'pipe', 'valve']
        self.def_components_gas_ch4 = ['compressor', 'vessel', 'filter',
                                       'hose', 'joint', 'pipe',
                                       'valve', 'instrument', 'exchanger']
        self.def_components_liq_ch4 = ['vessel', 'flange', 'hose',
                                       'pipe', 'valve', 'exchanger']
        self.def_components_c3h8 = ['compressor', 'vessel', 'filter',
                                    'flange', 'hose', 'pipe', 'valve']

    def test_get_default_leak_frequencies_default_sizes(self):
        species = 'h2'
        gas_h2_pipe_default_freqs = [8.02378743e-06,
                                     3.69732860e-06,
                                     9.56390055e-07,
                                     4.61261827e-07,
                                     1.46623172e-07]
        test_default_freqs = defaults.get_default_leak_frequencies(species)

        self.assertTrue(
            np.allclose(test_default_freqs['pipe'], gas_h2_pipe_default_freqs))

    def test_get_default_leak_frequencies_subset_of_categories(self):
        species = 'h2'
        categories = ['valve', 'pipe']
        test_default_freqs = defaults.get_default_leak_frequencies(
            species, categories=categories
        )

        self.assertTrue(list(test_default_freqs.keys()) == categories)

    def test_reject_bad_species_type(self):
        bad_species = 'h20'

        self.assertRaises(ValueError,
                          defaults.create_default_component_parameters,
                          bad_species)

    def test_get_default_component_parameters_gas_h2(self):
        species = 'h2'
        def_params = defaults.create_default_component_parameters(species)

        self.assertTrue(all(key in self.def_components_gas_h2
                            for key in def_params))

    def test_get_default_component_parameters_liq_h2(self):
        species = 'h2'
        phase = 'liquid'
        def_params = defaults.create_default_component_parameters(species,
                                                                  phase)
        self.assertTrue(all(key in self.def_components_liq_h2
                            for key in def_params))

    def test_get_default_component_parameters_gas_ch4(self):
        species = 'ch4'
        def_params = defaults.create_default_component_parameters(species)

        self.assertTrue(all(key in self.def_components_gas_ch4
                            for key in def_params))

    def test_get_default_component_parameters_liq_ch4(self):
        species = 'ch4'
        phase = 'liquid'
        def_params = defaults.create_default_component_parameters(species,
                                                                  phase)

        self.assertTrue(all(key in self.def_components_liq_ch4
                            for key in def_params))

    def test_get_default_component_parameters_c3h8(self):
        species = 'c3h8'
        def_params = defaults.create_default_component_parameters(species)

        self.assertTrue(all(key in self.def_components_c3h8
                            for key in def_params))
