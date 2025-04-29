"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.qra import consequence


class TestThermalFatalitiesCalc(unittest.TestCase):
    """
    Tests of calculation of thermal fatality probabilities
    """
    def test_calc_thermal_fatality_probabilities(self):
        consequence_type = 'thermal'
        num_leak_sizes = 5
        total_occupants = 2
        physical_responses = {'qrads': [1000, 2000, 3000, 4000, 5000,
                                        6000, 7000, 8000, 9000, 10000]}  # W/m2
        consequence_modeling_decisions = {'probit_thermal_id': 'eise',  # Eisenberg thermal probit
                                          'exposure_time': 60}  # seconds
        thermal_fatality_probs_per_leak = consequence.calculate_event_consequence(consequence_type=consequence_type,
                                                                                  num_leak_sizes=num_leak_sizes,
                                                                                  total_occupants=total_occupants,
                                                                                  physical_responses=physical_responses,
                                                                                  consequence_modeling_decisions=consequence_modeling_decisions)
        self.assertEqual(len(thermal_fatality_probs_per_leak),
                         num_leak_sizes)

    def test_zero_fatalities_for_zero_occupants(self):
        consequence_type = 'thermal'
        num_leak_sizes = 5
        total_occupants = 0
        physical_responses = {'qrads': [1000, 2000, 3000, 4000, 5000,
                                        6000, 7000, 8000, 9000, 10000]}  # W/m2
        consequence_modeling_decisions = {'probit_thermal_id': 'eise',  # Eisenberg thermal probit
                                          'exposure_time': 60}  # seconds
        thermal_fatality_probs_per_leak = consequence.calculate_event_consequence(consequence_type=consequence_type,
                                                                                  num_leak_sizes=num_leak_sizes,
                                                                                  total_occupants=total_occupants,
                                                                                  physical_responses=physical_responses,
                                                                                  consequence_modeling_decisions=consequence_modeling_decisions)
        for leak_size in range(num_leak_sizes):
            self.assertEqual(thermal_fatality_probs_per_leak[leak_size], 0)


class TestOverpressureFatalitiesCalc(unittest.TestCase):
    """
    Tests of calculation of overpressure fatality probabilities
    """
    def test_calc_overpressure_fatality_probabilities(self):
        consequence_type = 'overp'
        num_leak_sizes = 5
        total_occupants = 2
        physical_responses = {'overpressures': [1000, 2000, 3000, 4000, 5000,
                                                6000, 7000, 8000, 9000, 10000], # Pa
                              'impulses': [1100, 2100, 3100, 4100, 5100,
                                           6100, 7100, 8100, 9100, 11000]}  # Pa*s
        consequence_modeling_decisions = {'probit_overp_id': 'head'}  # TNO Heat Impact overpressure probit
        overpressure_fatality_probs_per_leak = consequence.calculate_event_consequence(consequence_type=consequence_type,
                                                                                       num_leak_sizes=num_leak_sizes,
                                                                                       total_occupants=total_occupants,
                                                                                       physical_responses=physical_responses,
                                                                                       consequence_modeling_decisions=consequence_modeling_decisions)
        self.assertEqual(len(overpressure_fatality_probs_per_leak),
                         num_leak_sizes)


class TestGenerateEventResults(unittest.TestCase):
    """
    Tests of dictionary generation for event results from consequences
    """
    def test_generate_event_results(self):
        event_names = {'Event1': 'thermal', 'Event2': 'overp'}
        event_keys = {'Event1': 'jetf', 'Event2': 'expl'}
        end_state_probs = [0.7, 0.3]
        event_occurrence = [1e-4, 1e-5]
        pll_contrib = [0.4, 0.6]
        test_event_results = consequence.generate_event_results(
            event_names, event_keys, end_state_probs, event_occurrence, pll_contrib)
        exp_event_results = [
            {'label': 'Event1', 'key': 'jetf',
             'prob': 0.7, 'events': 1e-4, 'pll': 0.4},
            {'label': 'Event2', 'key': 'expl',
             'prob': 0.3, 'events': 1e-5, 'pll': 0.6},
            {'label': 'TOTAL', 'key': 'tot',
             'prob': 1.0, 'events': 1.1e-4, 'pll': 1.0}
        ]

        self.assertEqual(test_event_results, exp_event_results)
