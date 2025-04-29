"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.qra import ignition_probs


class TestGetIgnitionProbability(unittest.TestCase):
    """
    Test obtaining of ignition probability based on mass flow rate
    """
    def setUp(self):
        self.ignition_probs = {
            'flow_thresholds': [0.125, 6.25],  # kg/s
            'immed_ign_probs': [0.008, 0.053, 0.23],
            'delayed_ign_probs': [0.004, 0.027, 0.12]
        }

    def test_reject_ign_probs_type(self):
        bad_ign_probs = [0.008, 0.053, 0.23]  # not dictionary
        self.assertRaises(TypeError,
                          ignition_probs.get_ignition_probability,
                          1, bad_ign_probs)

    def test_reject_unsorted_thresholds(self):
        bad_ign_probs = {
            'flow_thresholds': [6.25, 0.125],  # values not ascending
            'immed_ign_probs': [0.008, 0.053, 0.23],
            'delayed_ign_probs': [0.004, 0.027, 0.12]
        }
        self.assertRaises(ValueError,
                          ignition_probs.get_ignition_probability,
                          1, bad_ign_probs)

    def test_reject_too_many_thresholds(self):
        bad_ign_probs = {
            'flow_thresholds': [0.125, 6.25, 10],  # too many values
            'immed_ign_probs': [0.008, 0.053, 0.23],
            'delayed_ign_probs': [0.004, 0.027, 0.12]
        }
        self.assertRaises(ValueError,
                          ignition_probs.get_ignition_probability,
                          1, bad_ign_probs)

    def test_reject_wrong_prob_length(self):
        bad_ign_probs = {
            'flow_thresholds': [0.125, 6.25, 10],
            'immed_ign_probs': [0.008, 0.053, 0.23, 0.5],  # too many values
            'delayed_ign_probs': [0.004, 0.027, 0.12]
        }
        self.assertRaises(ValueError,
                          ignition_probs.get_ignition_probability,
                          1, bad_ign_probs)

    def test_get_first_ignition_probability(self):
        mass_flow_rate = 0.01  # kg/s
        (immediate_ign_prob, delayed_ign_prob) = ignition_probs.get_ignition_probability(mass_flow_rate,
                                                                                         self.ignition_probs)
        self.assertAlmostEqual(immediate_ign_prob, 0.008)
        self.assertAlmostEqual(delayed_ign_prob, 0.004)

    def test_get_middle_ignition_probability(self):
        mass_flow_rate = 1  # kg/s
        (immediate_ign_prob, delayed_ign_prob) = ignition_probs.get_ignition_probability(mass_flow_rate,
                                                                                         self.ignition_probs)
        self.assertAlmostEqual(immediate_ign_prob, 0.053)
        self.assertAlmostEqual(delayed_ign_prob, 0.027)

    def test_get_last_ignition_probability(self):
        mass_flow_rate = 7  # kg/s
        (immediate_ign_prob, delayed_ign_prob) = ignition_probs.get_ignition_probability(mass_flow_rate,
                                                                                         self.ignition_probs)
        self.assertAlmostEqual(immediate_ign_prob, 0.23)
        self.assertAlmostEqual(delayed_ign_prob, 0.12)


class TestCalcTotalIgnitionProbability(unittest.TestCase):
    """
    Test calculation of total ignition probability
    based on immediate and delayed ignition probabilities
    """
    def setUp(self):
        self.immed_ign_prob = 0.08
        self.delayed_ign_prob = 0.04

    def test_reject_invalid_probs(self):
        bad_probability = 1.1
        self.assertRaises(ValueError, ignition_probs.calc_total_ign_prob,
                          self.immed_ign_prob, bad_probability)
        self.assertRaises(ValueError, ignition_probs.calc_total_ign_prob,
                          bad_probability, self.delayed_ign_prob)

    def test_calc_cond_immed_ign_prob(self):
        total_ign_prob = ignition_probs.calc_total_ign_prob(self.immed_ign_prob,
                                                            self.delayed_ign_prob)
        self.assertAlmostEqual(total_ign_prob, 0.12)


class TestCalcConditionalImmediateIgnitionProbability(unittest.TestCase):
    """
    Test calculation of conditional immediate ignition probability
    based on absolute ignition probabilities
    """
    def setUp(self):
        self.abs_immed_ign_prob = 0.08
        self.total_ign_prob = 0.12

    def test_reject_invalid_probs(self):
        bad_probability = 1.1
        self.assertRaises(ValueError, ignition_probs.calc_cond_immed_ign_prob,
                          self.abs_immed_ign_prob, bad_probability)
        self.assertRaises(ValueError, ignition_probs.calc_cond_immed_ign_prob,
                          bad_probability, self.total_ign_prob)

    def test_calc_cond_immed_ign_prob(self):
        cond_immed_ign_prob = ignition_probs.calc_cond_immed_ign_prob(self.abs_immed_ign_prob,
                                                                      self.total_ign_prob)
        self.assertAlmostEqual(cond_immed_ign_prob, 0.6666667)

    def test_handle_zero_probs(self):
        cond_immed_ign_prob = ignition_probs.calc_cond_immed_ign_prob(0, 0)
        self.assertEqual(cond_immed_ign_prob, 0)
