"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

import numpy as np

from hyram.qra import component_set, leak_size_results


class LeakResultTestCase(unittest.TestCase):
    """ Evaluate creation of Leak Results objects. """

    def setUp(self):
        self.leak_sizes = [0.01, 0.1, 1, 10, 100]
        self.rel_species = 'h2'
        self.rel_phase = None
        self.comp_sets = component_set.init_component_sets(species=self.rel_species, phase=self.rel_phase, leak_sizes=self.leak_sizes)

    def test_leaks_without_overrides(self):
        overrides = [None, None, None, None, None]
        leak_results = leak_size_results.init_leak_results(self.leak_sizes, overrides, self.comp_sets)
        for i, result in enumerate(leak_results):
            self.assertGreater(np.sum(list(result.f_component_leaks.values())), 0)
            self.assertFalse(result.use_override)

            for component in self.comp_sets:
                # ignore when component unused
                if component.num_components:
                    self.assertTrue(result.f_release > 0)

    def test_leaks_with_overrides(self):
        overrides = [0.1, 0.2, 0.3, 0.4, 0.5]
        leak_results = leak_size_results.init_leak_results(self.leak_sizes, overrides, self.comp_sets)

        for i, result in enumerate(leak_results):
            self.assertEqual(result.f_release, overrides[i])
            self.assertTrue(result.use_override)
            self.assertEqual(np.sum(list(result.f_component_leaks.values())), 0)

    def test_leak_freqs_dont_match(self):
        overrides = [None, None, None, None, None]
        leak_results = leak_size_results.init_leak_results(self.leak_sizes, overrides, self.comp_sets)
        leak_000d01_freq = np.sum(list(leak_results[0].f_component_leaks.values()))
        leak_000d10_freq = np.sum(list(leak_results[1].f_component_leaks.values()))
        leak_001d00_freq = np.sum(list(leak_results[2].f_component_leaks.values()))
        leak_010d00_freq = np.sum(list(leak_results[3].f_component_leaks.values()))
        leak_100d00_freq = np.sum(list(leak_results[4].f_component_leaks.values()))

        self.assertNotAlmostEqual(leak_000d01_freq, leak_000d10_freq)
        self.assertNotAlmostEqual(leak_000d10_freq, leak_001d00_freq)
        self.assertNotAlmostEqual(leak_001d00_freq, leak_010d00_freq)
        self.assertNotAlmostEqual(leak_100d00_freq, leak_010d00_freq)

    def test_leaks_with_unchoked_mass_flows(self):
        leak_results = leak_size_results.init_leak_results(self.leak_sizes, None, self.comp_sets, mass_flow_leak_size=1, mass_flow=0.44)
        flows = [0.44, 4.4, 44., 440, 4400]
        for i, result in enumerate(leak_results):
            self.assertAlmostEqual(result.mass_flow_override, flows[i], places=4)

        leak_results = leak_size_results.init_leak_results(self.leak_sizes, None, self.comp_sets, mass_flow_leak_size=10, mass_flow=5.9)
        flows = [0.59, 5.9, 59, 590, 5900]
        for i, result in enumerate(leak_results):
            self.assertAlmostEqual(result.mass_flow_override, flows[i], places=4)

        leak_results = leak_size_results.init_leak_results(self.leak_sizes, None, self.comp_sets, mass_flow_leak_size=100, mass_flow=0.44)
        flows = [0.0044, 0.044, 0.44, 4.4, 44.]
        for i, result in enumerate(leak_results):
            self.assertAlmostEqual(result.mass_flow_override, flows[i], places=4)

        leak_results = leak_size_results.init_leak_results(self.leak_sizes, None, self.comp_sets, mass_flow_leak_size=1000, mass_flow=0.01)
        flows = [0.00001, 0.0001, 0.001, 0.01, 0.1]
        for i, result in enumerate(leak_results):
            self.assertAlmostEqual(result.mass_flow_override, flows[i], places=4)

        leak_results = leak_size_results.init_leak_results(self.leak_sizes, None, self.comp_sets, mass_flow_leak_size=10000, mass_flow=1)
        flows = [0.0001, 0.001, 0.01, 0.1, 1]
        for i, result in enumerate(leak_results):
            self.assertAlmostEqual(result.mass_flow_override, flows[i], places=4)

    def test_leaks_with_unrecognized_mass_flow_size_raises_error(self):
        with self.assertRaises(ValueError):
            leak_results = leak_size_results.init_leak_results(self.leak_sizes, None, self.comp_sets, mass_flow_leak_size=4, mass_flow=4)

        with self.assertRaises(ValueError):
            leak_results = leak_size_results.init_leak_results(self.leak_sizes, None, self.comp_sets, mass_flow_leak_size=0.1, mass_flow=4)
