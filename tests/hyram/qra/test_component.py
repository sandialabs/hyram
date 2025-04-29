"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

from hyram.qra import component


class ComponentTestCase(unittest.TestCase):
    """
    Evaluate creation of Component objects
    """
    def setUp(self):
        self.category = 'compressor'
        self.quantity = 2
        self.leak_freqs = [0.05, 0.04, 0.03, 0.02, 0.01]
        self.component = component.Component(
            self.category, self.quantity, self.leak_freqs)

    def test_component_attributes(self):
        self.assertIsInstance(self.component, component.Component)
        self.assertEqual(self.component.category, 'compressor')
        self.assertEqual(self.component.quantity, 2)
        self.assertListEqual(self.component.leak_freqs, [0.05, 0.04, 0.03, 0.02, 0.01])

    def test_component_str_print(self):
        exp_str_print = 'Component compressor (2) freqs: [0.05, 0.04, 0.03, 0.02, 0.01]'
        self.assertEqual(str(self.component), exp_str_print)

    def test_get_random_leak_frequencies_at_size(self):
        test_freqs = self.component.get_random_leak_frequencies_at_size(0.1)
        self.assertEqual(test_freqs, [0.04, 0.04])

    def test_reject_invalid_category(self):
        bad_category = 'bad_component'
        self.assertRaises(ValueError, component.Component,
                          bad_category, self.quantity, self.leak_freqs)

    def test_reject_invalid_quantity(self):
        bad_quantity = 4.5
        self.assertRaises(ValueError, component.Component,
                          self.category, bad_quantity, self.leak_freqs)
        bad_quantity = -2
        self.assertRaises(ValueError, component.Component,
                          self.category, bad_quantity, self.leak_freqs)

    def test_reject_invalid_leak_freqs_length(self):
        bad_leak_freqs = [0.03, 0.02, 0.01]
        self.assertRaises(ValueError, component.Component,
                          self.category, self.quantity, bad_leak_freqs)

    def test_get_random_leak_frequencies_at_size_reject_invalid_size(self):
        self.assertRaises(ValueError, self.component.get_random_leak_frequencies_at_size, 0.2)


class GetLeakFreqsAtSizeForSetTestCase(unittest.TestCase):
    """
    Evaluate `get_leak_frequencies_at_size_for_set` function operation
    """
    def setUp(self):
        self.component_set = [
            component.Component(
                'compressor', 2, [0.05, 0.04, 0.03, 0.02, 0.01]),
            component.Component(
                'valve', 3, [0.005, 0.004, 0.003, 0.002, 0.001]),
        ]

    def test_get_leak_frequencies_at_size_for_set(self):
        test_leak_freqs = component.get_leak_frequencies_at_size_for_set(
            self.component_set, 0.1)
        expected_leak_freqs = {'compressor': 0.08, 'valve': 0.012}
        self.assertDictEqual(test_leak_freqs, expected_leak_freqs)

    def test_get_leak_frequencies_at_size_for_set_reject_unconfigured_size(self):
        self.assertRaises(ValueError, component.get_leak_frequencies_at_size_for_set,
                          self.component_set, 0.2)


class CreateComponentSetTestCase(unittest.TestCase):
    """
    Evaluate `create_component_set` function operation
    """
    def setUp(self):
        self.categories = ['compressor', 'valve']
        self.quantities = [2, 3]
        self.species = 'h2'
        self.frequencies = [[0.05, 0.04, 0.03, 0.02, 0.01],
                            [0.005, 0.004, 0.003, 0.002, 0.001]]

    def test_create_component_set_basic(self):
        component_set = component.create_component_set(
            categories=self.categories,
            quantities=self.quantities,
            frequencies=self.frequencies)
        self.assertEqual(len(component_set), 2)
        for idx, comp in enumerate(component_set):
            self.assertIsInstance(comp, component.Component)
            self.assertEqual(comp.category, self.categories[idx])
            self.assertEqual(comp.quantity, self.quantities[idx])
            self.assertListEqual(comp.leak_freqs, self.frequencies[idx])

    def test_create_component_set_reject_missing_species(self):
        no_species = None
        self.assertRaises(ValueError,
                          component.create_component_set,
                          self.categories,
                          self.quantities,
                          no_species)

    def test_create_component_set_default_freqs_gh2(self):
        component_set = component.create_component_set(
            categories=self.categories,
            quantities=self.quantities,
            species=self.species)
        gh2_valve_0p1_freq = 5.9e-4
        self.assertAlmostEqual(component_set[1].leak_freqs[1],
                               gh2_valve_0p1_freq, places=5)

    def test_create_component_set_default_freqs_lh2(self):
        component_set = component.create_component_set(
            categories=self.categories,
            quantities=self.quantities,
            species=self.species,
            saturated_phase='liquid')
        lh2_valve_0p1_freq = 5.9e-4
        self.assertAlmostEqual(component_set[1].leak_freqs[1],
                               lh2_valve_0p1_freq, places=5)

    def test_create_component_set_invalid_category(self):
        categories_w_invalid = ['compressor', 'dummy']
        self.assertRaises(ValueError,
                          component.create_component_set,
                          categories_w_invalid,
                          self.quantities)


class CreateDefaultComponentSetTestCase(unittest.TestCase):
    """
    Evaluate `create_default_component_set` function operation
    """
    def setUp(self):
        pass

    def test_create_default_component_set_gh2(self):
        def_gh2_valve_0p1_freq = 5.9e-4
        def_gh2_num_valves = 7
        def_component_set = component.create_default_component_set('h2')
        for comp in def_component_set:
            self.assertIsInstance(comp, component.Component)
        self.assertAlmostEqual(def_component_set[6].leak_freqs[1],
                               def_gh2_valve_0p1_freq, places=5)
        self.assertEqual(def_component_set[6].quantity,
                               def_gh2_num_valves)

    def test_create_default_component_set_lh2(self):
        def_lh2_valve_0p1_freq = 5.9e-4
        def_lh2_num_valves = 44
        def_component_set = component.create_default_component_set('h2', 'liquid')
        for comp in def_component_set:
            self.assertIsInstance(comp, component.Component)
        self.assertAlmostEqual(def_component_set[4].leak_freqs[1],
                               def_lh2_valve_0p1_freq, places=5)
        self.assertEqual(def_component_set[4].quantity,
                               def_lh2_num_valves)
