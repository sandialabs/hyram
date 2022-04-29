"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

import hyram.qra.fatalities as hyram_fatalities


class TestThermalFatalitiesCalc(unittest.TestCase):
    """
    Tests of calculation of thermal fatality probabilities
    """
    def test_calc_thermal_fatality_probabilities(self):
        qrads = [1000, 2000, 3000, 4000, 5000,
                 6000, 7000, 8000, 9000, 10000]  # W/m2
        probit_thermal_id = 'eise'  # Eisenberg thermal probit
        exposure_time = 60  # seconds
        num_leak_sizes = 5
        total_occupants = 2
        thermal_fatality_probs_per_leak = hyram_fatalities.calc_thermal_fatality_probabilities(qrads=qrads,
                                                                                               probit_thermal_id=probit_thermal_id,
                                                                                               exposure_time=exposure_time,
                                                                                               num_leak_sizes=num_leak_sizes,
                                                                                               total_occupants=total_occupants)
        self.assertEqual(len(thermal_fatality_probs_per_leak),
                         num_leak_sizes)


class TestOverpressureFatalitiesCalc(unittest.TestCase):
    """
    Tests of calculation of overpressure fatality probabilities
    """
    def test_calc_overpressure_fatality_probabilities(self):
        overpressures = [1000, 2000, 3000, 4000, 5000,
                         6000, 7000, 8000, 9000, 10000]  # Pa
        impulses = [1100, 2100, 3100, 4100, 5100,
                    6100, 7100, 8100, 9100, 11000]  # Pa*s
        overpressure_probit_id = 'head'  # TNO Heat Impact overpressure probit
        num_leak_sizes = 5
        total_occupants = 2
        overpressure_fatality_probs_per_leak = hyram_fatalities.calc_overpressure_fatality_probabilities(overpressures=overpressures,
                                                                                                         impulses=impulses,
                                                                                                         overpressure_probit_id=overpressure_probit_id,
                                                                                                         num_leak_sizes=num_leak_sizes,
                                                                                                         total_occupants=total_occupants)
        self.assertEqual(len(overpressure_fatality_probs_per_leak),
                         num_leak_sizes)



if __name__ == "__main__":
    unittest.main()
