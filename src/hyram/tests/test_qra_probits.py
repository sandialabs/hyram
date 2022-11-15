"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.qra import probits


class TestFatalityProbabilityCalc(unittest.TestCase):
    """
    Tests of calculation of fatality probability calculation
    """
    def test_zero_probability(self):
        value_much_lower_than_5 = -100
        self.assertAlmostEqual(probits.calculate_fatality_probability(value_much_lower_than_5), 0)

    def test_100pct_probability(self):
        value_much_grater_than_5 = 100
        self.assertAlmostEqual(probits.calculate_fatality_probability(value_much_grater_than_5), 1)

    def test_50pct_probability(self):
        self.assertAlmostEqual(probits.calculate_fatality_probability(5), 0.5)


class TestThermalProbits(unittest.TestCase):
    """
    Tests of thermal probit calculations
    """
    def test_calculate_thermal_dose(self):
        # Hand-calculation comparison
        heat_flux = 5  # W/m^2
        exposure_time = 30  # seconds
        self.assertAlmostEqual(probits.calculate_thermal_dose(heat_flux, exposure_time), 256.4963920)

    def test_calculate_zero_thermal_dose(self):
        heat_flux = 5  # W/m^2
        exposure_time = 30  # seconds
        self.assertAlmostEqual(probits.calculate_thermal_dose(0, exposure_time), 0)
        self.assertAlmostEqual(probits.calculate_thermal_dose(heat_flux, 0), 0)

    def test_thermal_eisenberg(self):
        # Hand-calculation comparison
        thermal_dose = 1e7  # (W/m^2)^(4/3)*s
        self.assertAlmostEqual(probits.thermal_eisenberg(thermal_dose), 2.7823249)

    def test_thermal_tsao_perry(self):
        # Hand-calculation comparison
        thermal_dose = 1e7  # (W/m^2)^(4/3)*s
        self.assertAlmostEqual(probits.thermal_tsao_perry(thermal_dose), 4.8823249)

    def test_thermal_tno(self):
        # Hand-calculation comparison
        thermal_dose = 1e7  # (W/m^2)^(4/3)*s
        self.assertAlmostEqual(probits.thermal_tno(thermal_dose), 4.0323249)

    def test_thermal_lees(self):
        # Hand-calculation comparison
        thermal_dose = 1e7  # (W/m^2)^(4/3)*s
        self.assertAlmostEqual(probits.thermal_lees(thermal_dose), 1.6756475)

    def test_thermal_fatality_prob(self):
        # Heat flux is hand-calculated below to obtrain a 50% probability of fatality with these inputs
        model_ref = 'eise'  # Reference string for Eisenberg thermal probit model
        heat_flux = 15796.170691  # W/m^2
        exposure_time = 60  # seconds
        self.assertAlmostEqual(probits.compute_thermal_fatality_prob(model_ref, heat_flux, exposure_time), 0.5)

    def test_zero_thermal_dose_gives_zero_fatality_prob(self):
        model_ref = 'eise'  # Reference string for Eisenberg thermal probit model
        heat_flux = 0  # W/m^2
        exposure_time = 60  # seconds
        self.assertAlmostEqual(probits.compute_thermal_fatality_prob(model_ref, heat_flux, exposure_time), 0)


class TestOverpressureProbits(unittest.TestCase):
    """
    Tests of overpressure probit calculations
    """
    def test_overpressure_eisenberg(self):
        # Hand-calculation comparison
        peak_overpressure = 1e5  # Pa
        self.assertAlmostEqual(probits.overp_eisenberg(peak_overpressure), 2.4543150)

    def test_overpressure_hse(self):
        # Hand-calculation comparison
        peak_overpressure = 1e5  # Pa
        self.assertAlmostEqual(probits.overp_hse(peak_overpressure), 5.1300000)

    def test_overpressure_tno_head(self):
        # Hand-calculation comparison
        peak_overpressure = 1e5  # Pa
        impulse = 1e4  # Pa*s
        self.assertAlmostEqual(probits.overp_tno_head(peak_overpressure, impulse), 12.2786003)

    def test_overpressure_tno_struct_collapse(self):
        # Hand-calculation comparison
        peak_overpressure = 1e5  # Pa
        impulse = 1e4  # Pa*s
        self.assertAlmostEqual(probits.overp_tno_struct_collapse(peak_overpressure, impulse), 6.4917213)

    def test_overpressure_fatality_prob(self):
        # Peak overpressure is hand-calculated below to obtrain a 50% probability of fatality with these inputs
        model_ref = 'leis'  # Reference string for Eisenburg Lung Hemmorrhage overpressure probit model
        peak_overpressure = 144542.867547  # Pa
        self.assertAlmostEqual(probits.compute_overpressure_fatality_prob(model_ref, peak_overpressure), 0.5)

    def test_zero_overpressure_gives_zero_fatality_prob(self):
        model_ref = 'leis'  # Reference string for Eisenburg Lung Hemmorrhage overpressure probit model
        peak_overpressure = 0
        self.assertAlmostEqual(probits.compute_overpressure_fatality_prob(model_ref, peak_overpressure), 0)

if __name__ == "__main__":
    unittest.main()
