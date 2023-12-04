"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.qra import ignition_probs


class TestIgnitionProbability(unittest.TestCase):
    """
    Test estimation of ignition probability
    """
    def setUp(self):
        self.mass_flow_rate = 1  # kg/s
        self.ign_thresholds = [.125, 6.25]  # kg/s
        self.immed_ign_probs = [.008, .053, .23]
        self.delayed_ign_probs = [.004, .027, .12]

    def test_reject_bad_thresholds(self):
        unsorted_ignition_thresholds = [1.1, 3.1, 2.1]  # kg/s
        self.assertRaises(ValueError,
                          ignition_probs.get_ignition_probability,
                          self.mass_flow_rate,
                          unsorted_ignition_thresholds,
                          self.immed_ign_probs,
                          self.delayed_ign_probs)

    def test_reject_wrong_prob_length(self):
        bad_immed_ign_probs = [.008, .053, .23, 123]
        self.assertRaises(ValueError,
                          ignition_probs.get_ignition_probability,
                          self.mass_flow_rate,
                          self.ign_thresholds,
                          bad_immed_ign_probs,
                          self.delayed_ign_probs)

    def test_get_first_ignition_probability(self):
        mass_flow_rate = 0.01  # kg/s
        (immediate_ignition_prob, delayed_ignition_prob) = ignition_probs.get_ignition_probability(mass_flow_rate,
                                                                                                   self.ign_thresholds,
                                                                                                   self.immed_ign_probs,
                                                                                                   self.delayed_ign_probs)
        self.assertAlmostEqual(immediate_ignition_prob, 0.008)
        self.assertAlmostEqual(delayed_ignition_prob, 0.004)

    def test_get_middle_ignition_probability(self):
        mass_flow_rate = 1  # kg/s
        (immediate_ignition_prob, delayed_ignition_prob) = ignition_probs.get_ignition_probability(mass_flow_rate,
                                                                                                   self.ign_thresholds,
                                                                                                   self.immed_ign_probs,
                                                                                                   self.delayed_ign_probs)
        self.assertAlmostEqual(immediate_ignition_prob, 0.053)
        self.assertAlmostEqual(delayed_ignition_prob, 0.027)

    def test_get_last_ignition_probability(self):
        mass_flow_rate = 7  # kg/s
        (immediate_ignition_prob, delayed_ignition_prob) = ignition_probs.get_ignition_probability(mass_flow_rate,
                                                                                                   self.ign_thresholds,
                                                                                                   self.immed_ign_probs,
                                                                                                   self.delayed_ign_probs)
        self.assertAlmostEqual(immediate_ignition_prob, 0.23)
        self.assertAlmostEqual(delayed_ignition_prob, 0.12)


if __name__ == "__main__":
    unittest.main()
