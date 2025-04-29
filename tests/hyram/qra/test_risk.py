"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

import numpy as np

from hyram.qra import risk


class TestRiskMetricCalcs(unittest.TestCase):
    """
    Test basic calculation of risk metrics
    """
    def test_calc_far(self):
        pll = 2.5e-5  # fatalties/year
        total_occupants = 9
        far = risk.calc_far(pll, total_occupants)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(far, 0.0317098)

    def test_zero_far_for_zero_pll(self):
        pll = 0  # fatalties/year
        total_occupants = 0
        far = risk.calc_far(pll, total_occupants)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(far, 0)

    def test_zero_far_for_zero_occupants(self):
        pll = 0.1
        far = risk.calc_far(pll=pll, total_occupants=0)
        self.assertEqual(far, 0)

    def test_calc_air(self):
        far = 0.03  # fatalties per 10^8 person*hours
        exposed_hours_per_year = 2000
        air = risk.calc_air(far, exposed_hours_per_year)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(air, 6e-7)


class TestScenarioRiskCalcs(unittest.TestCase):
    """
    Test calculation of risk (PLL) for all scenarios
    """
    def test_calc_all_plls(self):
        frequencies = np.array([1e-5, 2e-5])
        fatalities = np.array([0.1, 0.2])
        plls = risk.calc_all_plls(frequencies, fatalities)
        # Hand-calculation of above numbers
        hand_calc_plls = [1e-6, 4e-6]
        for calc_pll, test_pll in zip(plls, hand_calc_plls):
            self.assertAlmostEqual(calc_pll, test_pll)

    def test_calc_risk_contributions(self):
        plls = np.array([1e-6, 4e-6])
        total_pll, pll_contributions = risk.calc_risk_contributions(plls)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(total_pll, 5e-6)
        hand_calc_contributions = [0.2, 0.8]
        for calc_value, test_value in zip(pll_contributions, hand_calc_contributions):
            self.assertAlmostEqual(calc_value, test_value)

    def test_calc_risk_contributions_handles_zeros(self):
        plls = np.array([0, 0])
        total_pll, pll_contributions = risk.calc_risk_contributions(plls)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(total_pll, 0)
        hand_calc_contributions = [0, 0]
        for calc_value, test_value in zip(pll_contributions, hand_calc_contributions):
            self.assertAlmostEqual(calc_value, test_value)
