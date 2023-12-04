"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

import scipy.constants as spc

from hyram.qra import analysis as qra_analysis
from hyram.qra import component_failure
from hyram.qra import component_set
from hyram.qra.data import component_data


class HydrogenTestCase(unittest.TestCase):
    """
    Test calculation of QRA analysis
    """
    def setUp(self):
        rel_species = "h2"
        rel_phase = None
        failure_set = component_failure.ComponentFailureSet(f_failure_override=None,
                                                            num_vehicles=20, daily_fuelings=2, vehicle_days=250,
                                                            noz_po_dist='beta', noz_po_a=0.5, noz_po_b=610415.5,
                                                            noz_ftc_dist='expv', noz_ftc_a=0.002, noz_ftc_b=None,
                                                            mvalve_ftc_dist='expv', mvalve_ftc_a=0.001, mvalve_ftc_b=None,
                                                            svalve_ftc_dist='expv', svalve_ftc_a=0.002, svalve_ftc_b=None,
                                                            svalve_ccf_dist='expv', svalve_ccf_a=0.000128, svalve_ccf_b=None,
                                                            overp_dist='beta', overp_a=3.5, overp_b=310289.5,
                                                            pvalve_fto_dist='logn', pvalve_fto_a=-11.74, pvalve_fto_b=0.67,
                                                            driveoff_dist='beta', driveoff_a=31.5, driveoff_b=610384.5,
                                                            coupling_ftc_dist='beta', coupling_ftc_a=0.5, coupling_ftc_b=5031)
        self.component_sets = component_set.init_component_sets(species=rel_species, phase=rel_phase,
                                                                n_valves=5, n_instruments=3, n_joints=35, n_hoses=1, n_pipes=20)
        occupant_input_list = [{
            'count': 9,
            'descrip': 'workers',
            'xdistr': 'uniform', 'xa': 1, 'xb': 20,
            'ydistr': 'dete', 'ya': 1, 'yb': None,
            'zdistr': 'unif', 'za': 1, 'zb': 12,
            'hours': 2000,
        }]

        self.params = {
            "pipe_outer_diam": 0.375 * spc.inch,  # m
            "pipe_thickness": 0.065 * spc.inch,  # m
            "amb_temp": 15 + spc.zero_Celsius,  # K
            "amb_pres": 101325,  # Pa
            "rel_temp": 15 + spc.zero_Celsius,  # K
            "rel_pres": 35  * spc.mega,  # Pa
            "rel_phase": None,
            "facil_length": 20,  # m
            "facil_width": 12,  # m
            "immed_ign_probs": [0.008, 0.053, 0.23],
            "delayed_ign_probs": [0.004, 0.027, 0.12],
            "ign_thresholds": [0.125, 6.25],
            "occupant_input_list": occupant_input_list,
            "component_sets": self.component_sets,
            "component_failure_set": failure_set,
            "leak_sizes": None,
            "rel_species": 'h2',
            "discharge_coeff": 1,
            "detection_credit": 0.9,
            "overp_method": 'bst',
            "tnt_factor": None,
            "bst_flame_speed": 0.35,
            "probit_thermal_id": 'eise',
            "exposure_time": 60,  # s
            "probit_overp_id": 'head',
            "nozzle_model": 'yuce',
            "rel_angle": 0,
            "rel_humid": 0.89,
            "rand_seed": 3632850,
            "excl_radius": 0.01,
            "f_release_overrides": None,
            "verbose": False,
            "output_dir": None,  # Use default "temp"
            "create_plots": False,
        }

    def test_zero_risk_for_zero_components_zero_refuelings(self):
        zero_component_sets = []
        for component_set in self.component_sets:
            component_set.num_components = 0
            zero_component_sets.append(component_set)
        zero_refuelings_component_failure_set = component_failure.ComponentFailureSet(num_vehicles=0)
        self.params['component_sets'] = zero_component_sets
        self.params['component_failure_set'] = zero_refuelings_component_failure_set

        results = qra_analysis.conduct_analysis(**self.params)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 0)

    def test_zero_risk_for_zero_occupants(self):
        zero_occupant_input_list = [{
            'count': 0,
            'descrip': 'workers',
            'xdistr': 'uniform', 'xa': 1, 'xb': 20,
            'ydistr': 'dete', 'ya': 1, 'yb': None,
            'zdistr': 'unif', 'za': 1, 'zb': 12,
            'hours': 2000,
        }]
        self.params['occupant_input_list'] = zero_occupant_input_list
        results = qra_analysis.conduct_analysis(**self.params)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 0)

    def test_zero_risk_for_100pct_detection(self):
        detection_credit_100pct = 1
        self.params['detection_credit'] = detection_credit_100pct
        results = qra_analysis.conduct_analysis(**self.params)

        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 0)

    def test_reject_bauwens_with_impulse_probits(self):
        for probit_overp_id in ['head', 'coll']:
            self.params['overp_method'] = 'bauwens'
            self.params['probit_overp_id'] = probit_overp_id
            with self.assertRaises(ValueError):
                results = qra_analysis.conduct_analysis(**self.params)

    def test_flexible_leak_size_specification(self):
        self.params['leak_sizes'] = [0.1, 10]
        results1 = qra_analysis.conduct_analysis(**self.params)
        risk_value1 = results1['total_pll']

        self.params['leak_sizes'] = [100, 10, 0.1]
        results2 = qra_analysis.conduct_analysis(**self.params)
        risk_value2 = results2['total_pll']

        self.assertEqual(risk_value1, risk_value2)
        self.assertEqual(len(results1['leak_results']), len(results2['leak_results']))

    def test_overriding_leak_freq(self):
        self.params['f_release_overrides'] = [0, 0, 0, 0, 0]
        self.params['component_failure_set'] = component_failure.ComponentFailureSet(f_failure_override=0.0)
        results = qra_analysis.conduct_analysis(**self.params)
        risk_value = results['total_pll']
        self.assertEqual(risk_value, 0)

    def test_sat_vapor(self):
        self.params['rel_phase'] = 'gas'
        self.params['rel_temp'] = None
        self.params['rel_pres'] = 1e6
        self.params['component_sets'] = component_set.init_component_sets(species='h2', phase='gas',
                                                                          n_vessels=1, n_flanges=1, n_hoses=1,
                                                                          n_joints=35, n_pipes=20, n_valves=5, n_instruments=0,
                                                                          fuel_component_data=component_data.h2_liquid_params)
        results = qra_analysis.conduct_analysis(**self.params)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 0)

    def test_sat_liquid(self):
        self.params['rel_phase'] = 'liquid'
        self.params['rel_temp'] = None
        self.params['rel_pres'] = 1e6
        self.params['component_sets'] = component_set.init_component_sets(species='h2', phase='gas',
                                                                          n_vessels=1, n_flanges=1, n_hoses=1,
                                                                          n_joints=35, n_pipes=20, n_valves=5, n_instruments=0,
                                                                          fuel_component_data=component_data.h2_liquid_params)
        results = qra_analysis.conduct_analysis(**self.params)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 0)


if __name__ == "__main__":
    unittest.main()
