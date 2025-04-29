"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

import scipy.constants as spc
import numpy as np

from hyram.qra import analysis
from hyram.qra.component_failure import create_failure_set
from hyram.qra.component import create_component_set
from hyram.qra.defaults import default_failure_set


VERBOSE = False
OUTPUT_DIR = 'out'


class GetTotalLeakFreqAtSizeTestCase(unittest.TestCase):
    """
    Test `get_total_leak_frequency_at_size` function
    """
    def setUp(self):
        self.random_leak_frequency_set = {
            'valve': 0.01,
            'hose': 0.02,
            'pipe': 0.03
        }

    def test_get_total_leak_frequency_at_size_non100(self):
        failure_set = default_failure_set
        leak_size = 10
        test_leak_freq = analysis.get_total_leak_frequency_at_size(
            self.random_leak_frequency_set,
            failure_set,
            leak_size
        )
        self.assertEqual(test_leak_freq, 0.06)

    def test_get_total_leak_frequency_at_size_100_wo_fs(self):
        failure_set = None
        leak_size = 100
        test_leak_freq = analysis.get_total_leak_frequency_at_size(
            self.random_leak_frequency_set,
            failure_set,
            leak_size
        )
        self.assertEqual(test_leak_freq, 0.06)

    def test_get_total_leak_frequency_at_size_100_w_fs(self):
        failure_set = default_failure_set
        leak_size = 100
        test_leak_freq = analysis.get_total_leak_frequency_at_size(
            self.random_leak_frequency_set,
            failure_set,
            leak_size
        )
        exp_leak_freq = 0.06 + failure_set.freq_failure
        self.assertEqual(test_leak_freq, exp_leak_freq)


class ConductAnalysisHydrogenTestCase(unittest.TestCase):
    """
    Test calculation of QRA analysis
    """
    def setUp(self):
        rel_species = "h2"
        rel_phase = None
        occupant_hours = [2000, 2000, 2000,
                          2000, 2000, 2000,
                          2000, 2000, 2000]
        locations = [
            (2.4, 0.0, 2.1),
            (17.9, 0.0, 5.1),
            (14.9, 0.0, 2.0),
            (14.9, 0.0, 4.1),
            (8.6, 0.0, 2.7),
            (15.2, 0.0, 4.4),
            (6.5, 0.0, 5.3),
            (15.6, 0.0, 1.6),
            (2.3, 0.0, 4.7)
        ]
        self.def_component_types = ['valve', 'instrument', 'joint',
                                    'hose', 'pipe']
        self.def_component_counts = [5, 3, 35, 1, 20]
        self.failure_set = default_failure_set
        self.def_component_set = create_component_set(
            categories=self.def_component_types,
            quantities=self.def_component_counts,
            species=rel_species,
            saturated_phase=rel_phase
        )

        sat_component_types = ['vessel', 'flange', 'hose', 'pipe', 'valve']
        sat_component_counts = [1, 8, 1, 30, 44]
        self.sat_component_set = create_component_set(
            categories=sat_component_types,
            quantities=sat_component_counts,
            species='h2',
            saturated_phase='gas'
        )

        self.params = {
            "pipe_inner_diam": 0.245 * spc.inch,  # m
            "amb_temp": 15 + spc.zero_Celsius,  # K
            "amb_pres": 101325,  # Pa
            "rel_temp": 15 + spc.zero_Celsius,  # K
            "rel_pres": 35  * spc.mega,  # Pa
            "rel_species": rel_species,
            "locations": locations,
            "rel_phase": rel_phase,
            "component_set": self.def_component_set,
            "failure_set": self.failure_set,
            "occupant_hours": occupant_hours,
            "ft_overrides": None,
            "ign_probs": None,
            "discharge_coeff": 1,
            "mass_flow_rates": None,
            "detection_credit": 0.9,
            "overp_method": 'bst',
            "tnt_factor": 0.03,
            "bst_flame_speed": 0.35,
            "probit_thermal_id": 'eise',
            "exposure_time": 30,  # s
            "probit_overp_id": 'head',
            "nozzle_model": 'yuce',
            "rel_angle": 0,
            "rel_humid": 0.89,
            "verbose": False,
            "output_dir": OUTPUT_DIR,
            "create_plots": False,
        }

    def test_reject_bauwens_with_impulse_probits(self):
        for probit_overp_id in ['head', 'coll']:
            self.params['overp_method'] = 'bau'
            self.params['probit_overp_id'] = probit_overp_id
            with self.assertRaises(ValueError):
                analysis.conduct_analysis(**self.params)

    def test_conduct_analysis_regression(self):
        results = analysis.conduct_analysis(**self.params)
        test_risk_value = results['total_pll']
        # regression test value based on existing develop branch
        exp_risk_value = 3.494e-6
        self.assertAlmostEqual(test_risk_value, exp_risk_value, places=9)

    def test_sat_vapor(self):
        self.params['rel_phase'] = 'gas'
        self.params['rel_temp'] = None
        self.params['rel_pres'] = 1e6
        self.params['component_set'] = self.sat_component_set
        results = analysis.conduct_analysis(**self.params)
        # TODO: come up with a better test assertion
        # runs without error
        self.assertTrue(True)

    def test_sat_liquid(self):
        self.params['rel_phase'] = 'liquid'
        self.params['rel_temp'] = None
        self.params['rel_pres'] = 1e6
        self.params['component_set'] = self.sat_component_set
        results = analysis.conduct_analysis(**self.params)
        # TODO: come up with a better test assertion
        # runs without error
        self.assertTrue(True)

    def test_unchoked_flow(self):
        self.params['rel_pres'] = 151325  # Pa
        self.params['mass_flow_rates'] = [0.1, 0.2, 0.3, 0.4, 0.5]  # kg/s
        results = analysis.conduct_analysis(**self.params)
        leak_results = results['leak_results']
        for lr_idx, leak_result in enumerate(leak_results):
            self.assertAlmostEqual(leak_result['discharge_rates'],
                                   self.params['mass_flow_rates'][lr_idx])

    def test_ft_override(self):
        self.params['ft_overrides'] = [None, 1.1e-6, 1.1e-7, 1.1e-8, 1.1e-9]  # leaks/year
        results = analysis.conduct_analysis(**self.params)
        leak_results = results['leak_results']
        for lr_idx, leak_result in enumerate(leak_results):
            if lr_idx != 0:  # first override not used above
                self.assertAlmostEqual(leak_result['frequency'],
                                       self.params['ft_overrides'][lr_idx])


class ConductAnalysisBlendsTestCase(unittest.TestCase):
    """
    Test QRA analysis with blends
    """

    def setUp(self):
        rel_species = {"ch4": 0.96, 'n2': 0.04}
        rel_phase = None
        occupant_hours = [2000, 2000, 2000,
                          2000, 2000, 2000,
                          2000, 2000, 2000]
        locations = [
            (2.4, 0.0, 2.1),
            (17.9, 0.0, 5.1),
            (14.9, 0.0, 2.0),
            (14.9, 0.0, 4.1),
            (8.6, 0.0, 2.7),
            (15.2, 0.0, 4.4),
            (6.5, 0.0, 5.3),
            (15.6, 0.0, 1.6),
            (2.3, 0.0, 4.7)
        ]

        blend_component_types = ['valve', 'instrument', 'joint',
                                 'hose', 'pipe']
        blend_component_counts = [5, 3, 35, 1, 20]
        blend_component_frequencies = [# derived from default h2_gas_params
            [0.002866682044628164, # valve
            0.0005858188182899353,
            5.440796496620691e-05,
            2.471994871531784e-05,
            4.815840352042724e-06],
            [0.0006236008859994445, # instrument
            0.0001954902601469749,
            0.00011166580849011478,
            0.00010003404299092957,
            3.680046783362321e-05],
            [3.5037207139347053e-05, # joint
            4.68895882071147e-06,
            7.8594022817001e-06,
            7.533882837374821e-06,
            6.401360577233544e-06],
            [0.0005794680676392204, # hose
            0.00020324467685797223,
            0.0001648787834179641,
            0.00015071800254160724,
            6.16893038164071e-05],
            [8.023787425428275e-06, # pipe
            3.6973285966044462e-06,
            9.56390054794557e-07,
            4.612618273357951e-07,
            1.4662317159365777e-07]
            ]
        self.failure_set = default_failure_set
        self.blend_component_set = create_component_set(
            categories=blend_component_types,
            quantities=blend_component_counts,
            species=rel_species,
            saturated_phase=rel_phase,
            frequencies=blend_component_frequencies
        )

        self.inputs = {
            "pipe_inner_diam": 0.245 * spc.inch,  # m
            "amb_temp": 15 + spc.zero_Celsius,  # K
            "amb_pres": 101325,  # Pa
            "rel_temp": 15 + spc.zero_Celsius,  # K
            "rel_pres": 35 * spc.mega,  # Pa
            "rel_species": rel_species,
            "locations": locations,
            "rel_phase": rel_phase,
            "component_set": self.blend_component_set,
            "failure_set": self.failure_set,
            "occupant_hours": occupant_hours,
            "ign_probs": None,
            "discharge_coeff": 1,
            "mass_flow_rates": None,
            "detection_credit": 0.9,
            "overp_method": 'bst',
            "tnt_factor": 0.03,
            "bst_flame_speed": 0.35,
            "probit_thermal_id": 'eise',
            "exposure_time": 30,
            "probit_overp_id": 'head',
            "nozzle_model": 'yuce',
            "rel_angle": 0,
            "rel_humid": 0.89,
            "verbose": VERBOSE,
            "output_dir": OUTPUT_DIR,
            "create_plots": False,
        }

    def test_basic_ch4_n2(self):
        results = analysis.conduct_analysis(**self.inputs)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 1.05e-5, places=2)


class ConductAnalysisZeroRiskTestCase(unittest.TestCase):
    def setUp(self):
        self.rel_species = "h2"
        rel_phase = None

        self.component_types = ['valve', 'instrument', 'joint',
                                    'hose', 'pipe']
        self.component_counts = [5, 3, 35, 1, 20]
        self.failure_set = default_failure_set
        self.component_set = create_component_set(
            categories=self.component_types,
            quantities=self.component_counts,
            species=self.rel_species,
            saturated_phase=rel_phase,
        )

        self.inputs = {
            "pipe_inner_diam": 0.245 * spc.inch,  # m
            "amb_temp": 15 + spc.zero_Celsius,  # K
            "amb_pres": 101325,  # Pa
            "rel_temp": 15 + spc.zero_Celsius,  # K
            "rel_pres": 35 * spc.mega,  # Pa
            "rel_phase": rel_phase,
            "locations": [
                (2.4, 0.0, 2.1),
                (17.9, 0.0, 5.1),
                (14.9, 0.0, 2.0),
                (14.9, 0.0, 4.1),
                (8.6, 0.0, 2.7),
                (15.2, 0.0, 4.4),
                (6.5, 0.0, 5.3),
                (15.6, 0.0, 1.6),
                (2.3, 0.0, 4.7)
            ],
            "component_set": self.component_set,
            "failure_set": self.failure_set,
            "rel_species": self.rel_species,
            "ign_probs": None,
            "discharge_coeff": 1,
            "mass_flow_rates": None,
            "detection_credit": 0.9,
            "overp_method": 'bst',
            "tnt_factor": None,
            "bst_flame_speed": 0.35,
            "probit_thermal_id": 'eise', "exposure_time": 60, "probit_overp_id": 'head',
            "nozzle_model": 'yuce',
            "rel_angle": 0, "rel_humid": 0.89,
            "verbose": VERBOSE, "output_dir": OUTPUT_DIR, "create_plots": False,
        }

    def test_zero_leak_frequency(self):
        nofreq_component_set = create_component_set(
            categories=self.component_types,
            quantities=self.component_counts,
            frequencies=[
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0],
                [0, 0, 0, 0, 0]
            ]
        )
        zero_failure_set = create_failure_set(
            num_vehicles=0,
            daily_fuelings=0,
            vehicle_days=0
        )
        self.inputs['component_set'] = nofreq_component_set
        self.inputs['failure_set'] = zero_failure_set
        results = analysis.conduct_analysis(**self.inputs)
        risk_value = results['total_pll']
        self.assertEqual(risk_value, 0)

    def test_zero_components_zero_refuelings(self):
        zero_component_set = create_component_set(
            categories=self.component_types,
            quantities=[0, 0, 0, 0, 0],
            species=self.rel_species
        )
        zero_failure_set = create_failure_set(
                num_vehicles=0,
                daily_fuelings=0,
                vehicle_days=0
        )
        self.inputs['component_set'] = zero_component_set
        self.inputs['failure_set'] = zero_failure_set
        results = analysis.conduct_analysis(**self.inputs)
        risk_value = results['total_pll']

        self.assertAlmostEqual(risk_value, 0)

    def test_zero_occupants(self):
        self.inputs['locations'] = []
        results = analysis.conduct_analysis(**self.inputs)
        risk_value = results['total_pll']
        self.assertEqual(risk_value, 0)

    def test_detection_isolation(self):
        self.inputs['detection_credit'] = 1.0
        results = analysis.conduct_analysis(**self.inputs)
        risk_value = results['total_pll']
        self.assertEqual(risk_value, 0)

    def test_zero_ignition_probability(self):
        self.inputs['ign_probs'] = {
            'flow_thresholds': [0.125, 6.25],
            'immed_ign_probs': [0, 0, 0],
            'delayed_ign_probs': [0, 0, 0]
        }
        results = analysis.conduct_analysis(**self.inputs)
        risk_value = results['total_pll']
        self.assertEqual(risk_value, 0)

    def test_zero_overpressure(self):
        self.inputs['ign_probs'] = {
            'flow_thresholds': [0.125, 6.25],
            'immed_ign_probs': [1, 1, 1],
            'delayed_ign_probs': [0, 0, 0]
        }
        results = analysis.conduct_analysis(**self.inputs)
        self.assertTrue(np.all(results['position_overps'] == 0))
        self.assertTrue(np.any(results['position_qrads'] > 0))
