"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.qra import event_tree


class TestEventTreeCalcs(unittest.TestCase):
    """
    Tests of calculations for event trees
    """
    def test_negative_probability_value_error(self):
        self.assertRaises(ValueError, event_tree.check_probability_value, -1)

    def test_probability_value_too_large_error(self):
        self.assertRaises(ValueError, event_tree.check_probability_value, 1.1)

    def test_probability_not_occur(self):
        probability = 0.7
        probability_not_occur = 0.3
        calculated_not_probability = event_tree.calc_probability_not_occur(probability)
        self.assertAlmostEqual(calculated_not_probability, probability_not_occur)

    def test_calc_end_state_frequencies(self):
        initiating_event_frequency = 1e-5
        end_state_probabilities = [0.1, 0.2, 0.3, 0.4]
        end_state_frequencies = event_tree.calc_end_state_frequencies(initiating_event_frequency, end_state_probabilities)
        hand_calc_end_state_freqs = [1e-6, 2e-6, 3e-6, 4e-6]
        for calc_value, hand_calc_value in zip(end_state_frequencies, hand_calc_end_state_freqs):
            self.assertAlmostEqual(calc_value, hand_calc_value)


class TestBuildEventTree(unittest.TestCase):
    """
    Tests of building an event tree calculation
    """
    def test_calc_end_state_probabilities(self):
        event_probabilities = [0.1, 0.2, 0.3]
        end_state_probabilities = event_tree.calc_end_state_probabilities(event_probabilities)
        hand_calc_end_state_probabilities = [0.1, 0.18, 0.216, 0.504]
        for calc_value, hand_calc in zip(end_state_probabilities, hand_calc_end_state_probabilities):
            self.assertAlmostEqual(calc_value, hand_calc)

    def test_end_state_probabilities_sum_to_1(self):
        event_probabilities = [0.1, 0.2, 0.3]
        end_state_probabilities = event_tree.calc_end_state_probabilities(event_probabilities)
        sum_of_end_state_probabilities = sum(end_state_probabilities)
        self.assertAlmostEqual(sum_of_end_state_probabilities, 1)

    def test_reject_bad_probabilities(self):
        bad_event_probabilities = [0.1, 1.2, 0.3]
        self.assertRaises(ValueError, event_tree.calc_end_state_probabilities, bad_event_probabilities)


if __name__ == "__main__":
    unittest.main()
