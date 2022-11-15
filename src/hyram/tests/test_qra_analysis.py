"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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


class TestAnalysis(unittest.TestCase):
    """
    Test calculation of QRA analysis
    """
    def setUp(self):
        self.pipe_outer_diam = 0.375 * spc.inch  # m
        self.pipe_thickness = 0.065 * spc.inch  # m
        self.amb_temp = 15 + spc.zero_Celsius  # K
        self.amb_pres = 101325  # Pa
        self.rel_temp = 15 + spc.zero_Celsius  # K
        self.rel_pres = 35  * spc.mega  # Pa
        self.rel_phase = None
        self.facil_length = 20  # m
        self.facil_width = 12  # m
        self.immed_ign_probs = [0.008, 0.053, 0.23]
        self.delayed_ign_probs = [0.004, 0.027, 0.12]
        self.ign_thresholds = [0.125, 6.25]
        self.occupant_input_list = [{
            'count': 9,
            'descrip': 'workers',
            'xdistr': 'uniform', 'xa': 1, 'xb': 20,
            'ydistr': 'dete', 'ya': 1, 'yb': None,
            'zdistr': 'unif', 'za': 1, 'zb': 12,
            'hours': 2000,
        }]
        self.component_failure_set = component_failure.ComponentFailureSet(
                f_failure_override=None,
                num_vehicles=20,
                daily_fuelings=2,
                vehicle_days=250,
                noz_po_dist='beta',
                noz_po_a=0.5,
                noz_po_b=610415.5,
                noz_ftc_dist='expv',
                noz_ftc_a=0.002,
                noz_ftc_b=None,
                mvalve_ftc_dist='expv',
                mvalve_ftc_a=0.001,
                mvalve_ftc_b=None,
                svalve_ftc_dist='expv',
                svalve_ftc_a=0.002,
                svalve_ftc_b=None,
                svalve_ccf_dist='expv',
                svalve_ccf_a=0.000128,
                svalve_ccf_b=None,
                overp_dist='beta',
                overp_a=3.5,
                overp_b=310289.5,
                pvalve_fto_dist='logn',
                pvalve_fto_a=-11.74,
                pvalve_fto_b=0.67,
                driveoff_dist='beta',
                driveoff_a=31.5,
                driveoff_b=610384.5,
                coupling_ftc_dist='beta',
                coupling_ftc_a=0.5,
                coupling_ftc_b=5031)
        self.leak_sizes = None  # Use default: [0.01, 0.10, 1, 10, 100]
        self.rel_species = 'h2'
        self.discharge_coeff = 1
        self.detection_credit = 0.9
        self.overp_method = 'bst'
        self.TNT_equivalence_factor = None
        self.BST_mach_flame_speed = 0.35
        self.probit_thermal_id = 'eise'
        self.exposure_time = 60  # s
        self.probit_overp_id = 'head'
        self.nozzle_model = 'yuce'
        self.rel_angle = 0
        self.rel_humid = 0.89
        self.rand_seed = 3632850
        self.excl_radius = 0.01
        self.release_freq_overrides = None
        self.event_tree_override = None
        self.verbose = False
        self.output_dir = None  # Use default "temp"
        self.create_plots = False
        number_of_compressors = 0
        number_of_vessels = 0
        number_of_valves = 5
        number_of_instruments = 3
        number_of_joints = 35
        number_of_hoses = 1
        number_of_pipes = 20  # m
        number_of_filters = 0
        number_of_flanges = 0
        number_of_heat_exchangers = 0
        number_of_vaporizers = 0
        number_of_transfer_arms = 0
        number_of_extra1s = 0
        number_of_extra2s = 0
        self.component_sets = [
            component_set.ComponentSet(category='compressor',
                                       num_components=number_of_compressors,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['compressor'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='vessel',
                                       num_components=number_of_vessels,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['vessel'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='valve',
                                       num_components=number_of_valves,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['valve'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='instrument',
                                       num_components=number_of_instruments,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['instrument'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='joint',
                                       num_components=number_of_joints,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['joint'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='hose',
                                       num_components=number_of_hoses,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['hose'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='pipe',
                                       num_components=number_of_pipes,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['pipe'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='filter',
                                       num_components=number_of_filters,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['filter'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='flange',
                                       num_components=number_of_flanges,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['flange'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='exchanger',
                                       num_components=number_of_heat_exchangers,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['exchanger'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='vaporizer',
                                       num_components=number_of_vaporizers,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['vaporizer'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='arm',
                                       num_components=number_of_transfer_arms,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['arm'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='extra1',
                                       num_components=number_of_extra1s,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['extra1'],
                                       leak_sizes=self.leak_sizes),
            component_set.ComponentSet(category='extra2',
                                       num_components=number_of_extra2s,
                                       species=self.rel_species,
                                       saturated_phase=self.rel_phase,
                                       leak_frequency_lists=component_data.h2_gas_params['extra2'],
                                       leak_sizes=self.leak_sizes)
        ]

    def test_zero_risk_for_zero_components_zero_refuelings(self):
        zero_component_sets = []
        for component_set in self.component_sets:
            component_set.num_components = 0
            zero_component_sets.append(component_set)
        zero_refuelings_component_failure_set = component_failure.ComponentFailureSet(num_vehicles=0)
        results = qra_analysis.conduct_analysis(self.pipe_outer_diam,
                                                self.pipe_thickness,
                                                self.amb_temp,
                                                self.amb_pres,
                                                self.rel_temp,
                                                self.rel_pres,
                                                self.rel_phase,
                                                self.facil_length,
                                                self.facil_width,
                                                self.immed_ign_probs,
                                                self.delayed_ign_probs,
                                                self.ign_thresholds,
                                                self.occupant_input_list,
                                                zero_component_sets,
                                                zero_refuelings_component_failure_set,
                                                self.rel_species,
                                                self.leak_sizes,
                                                self.discharge_coeff,
                                                self.detection_credit,
                                                self.overp_method,
                                                self.TNT_equivalence_factor,
                                                self.BST_mach_flame_speed,
                                                self.probit_thermal_id,
                                                self.exposure_time,
                                                self.probit_overp_id,
                                                self.nozzle_model,
                                                self.rel_angle,
                                                self.rel_humid,
                                                self.rand_seed,
                                                self.excl_radius,
                                                self.release_freq_overrides,
                                                self.event_tree_override,
                                                self.verbose,
                                                self.output_dir,
                                                self.create_plots)
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
        results = qra_analysis.conduct_analysis(self.pipe_outer_diam,
                                                self.pipe_thickness,
                                                self.amb_temp,
                                                self.amb_pres,
                                                self.rel_temp,
                                                self.rel_pres,
                                                self.rel_phase,
                                                self.facil_length,
                                                self.facil_width,
                                                self.immed_ign_probs,
                                                self.delayed_ign_probs,
                                                self.ign_thresholds,
                                                zero_occupant_input_list,
                                                self.component_sets,
                                                self.component_failure_set,
                                                self.rel_species,
                                                self.leak_sizes,
                                                self.discharge_coeff,
                                                self.detection_credit,
                                                self.overp_method,
                                                self.TNT_equivalence_factor,
                                                self.BST_mach_flame_speed,
                                                self.probit_thermal_id,
                                                self.exposure_time,
                                                self.probit_overp_id,
                                                self.nozzle_model,
                                                self.rel_angle,
                                                self.rel_humid,
                                                self.rand_seed,
                                                self.excl_radius,
                                                self.release_freq_overrides,
                                                self.event_tree_override,
                                                self.verbose,
                                                self.output_dir,
                                                self.create_plots)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 0)

    def test_zero_risk_for_100pct_detection(self):
        detection_credit_100pct = 1
        results = qra_analysis.conduct_analysis(self.pipe_outer_diam,
                                                self.pipe_thickness,
                                                self.amb_temp,
                                                self.amb_pres,
                                                self.rel_temp,
                                                self.rel_pres,
                                                self.rel_phase,
                                                self.facil_length,
                                                self.facil_width,
                                                self.immed_ign_probs,
                                                self.delayed_ign_probs,
                                                self.ign_thresholds,
                                                self.occupant_input_list,
                                                self.component_sets,
                                                self.component_failure_set,
                                                self.rel_species,
                                                self.leak_sizes,
                                                self.discharge_coeff,
                                                detection_credit_100pct,
                                                self.overp_method,
                                                self.TNT_equivalence_factor,
                                                self.BST_mach_flame_speed,
                                                self.probit_thermal_id,
                                                self.exposure_time,
                                                self.probit_overp_id,
                                                self.nozzle_model,
                                                self.rel_angle,
                                                self.rel_humid,
                                                self.rand_seed,
                                                self.excl_radius,
                                                self.release_freq_overrides,
                                                self.event_tree_override,
                                                self.verbose,
                                                self.output_dir,
                                                self.create_plots)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 0)

    def test_reject_bauwens_with_impulse_probits(self):
        overp_method = 'bauwens'
        probit_overp_ids = ['head', 'coll']
        for probit_overp_id in probit_overp_ids:
            self.assertRaises(ValueError,
                              qra_analysis.conduct_analysis,
                              self.pipe_outer_diam,
                              self.pipe_thickness,
                              self.amb_temp,
                              self.amb_pres,
                              self.rel_temp,
                              self.rel_pres,
                              self.rel_phase,
                              self.facil_length,
                              self.facil_width,
                              self.immed_ign_probs,
                              self.delayed_ign_probs,
                              self.ign_thresholds,
                              self.occupant_input_list,
                              self.component_sets,
                              self.component_failure_set,
                              self.leak_sizes,
                              self.rel_species,
                              self.discharge_coeff,
                              self.detection_credit,
                              overp_method,
                              self.TNT_equivalence_factor,
                              self.BST_mach_flame_speed,
                              self.probit_thermal_id,
                              self.exposure_time,
                              probit_overp_id,
                              self.nozzle_model,
                              self.rel_angle,
                              self.rel_humid,
                              self.rand_seed,
                              self.excl_radius,
                              self.release_freq_overrides,
                              self.event_tree_override,
                              self.verbose,
                              self.output_dir,
                              self.create_plots)

    def test_flexible_leak_size_specification(self):
        leak_sizes1 = [0.1, 10]
        results1 = qra_analysis.conduct_analysis(self.pipe_outer_diam,
                                                self.pipe_thickness,
                                                self.amb_temp,
                                                self.amb_pres,
                                                self.rel_temp,
                                                self.rel_pres,
                                                self.rel_phase,
                                                self.facil_length,
                                                self.facil_width,
                                                self.immed_ign_probs,
                                                self.delayed_ign_probs,
                                                self.ign_thresholds,
                                                self.occupant_input_list,
                                                self.component_sets,
                                                self.component_failure_set,
                                                self.rel_species,
                                                leak_sizes1,
                                                self.discharge_coeff,
                                                self.detection_credit,
                                                self.overp_method,
                                                self.TNT_equivalence_factor,
                                                self.BST_mach_flame_speed,
                                                self.probit_thermal_id,
                                                self.exposure_time,
                                                self.probit_overp_id,
                                                self.nozzle_model,
                                                self.rel_angle,
                                                self.rel_humid,
                                                self.rand_seed,
                                                self.excl_radius,
                                                self.release_freq_overrides,
                                                self.event_tree_override,
                                                self.verbose,
                                                self.output_dir,
                                                self.create_plots)
        risk_value1 = results1['total_pll']
        leak_sizes2 = [100, 10, 0.1]
        results2 = qra_analysis.conduct_analysis(self.pipe_outer_diam,
                                                self.pipe_thickness,
                                                self.amb_temp,
                                                self.amb_pres,
                                                self.rel_temp,
                                                self.rel_pres,
                                                self.rel_phase,
                                                self.facil_length,
                                                self.facil_width,
                                                self.immed_ign_probs,
                                                self.delayed_ign_probs,
                                                self.ign_thresholds,
                                                self.occupant_input_list,
                                                self.component_sets,
                                                self.component_failure_set,
                                                self.rel_species,
                                                leak_sizes2,
                                                self.discharge_coeff,
                                                self.detection_credit,
                                                self.overp_method,
                                                self.TNT_equivalence_factor,
                                                self.BST_mach_flame_speed,
                                                self.probit_thermal_id,
                                                self.exposure_time,
                                                self.probit_overp_id,
                                                self.nozzle_model,
                                                self.rel_angle,
                                                self.rel_humid,
                                                self.rand_seed,
                                                self.excl_radius,
                                                self.release_freq_overrides,
                                                self.event_tree_override,
                                                self.verbose,
                                                self.output_dir,
                                                self.create_plots)
        risk_value2 = results2['total_pll']
        self.assertEqual(risk_value1, risk_value2)
        self.assertEqual(len(results1['leak_results']), len(results2['leak_results']))

    def test_overriding_leak_freq(self):
        release_freq_overrides = [0, 0, 0, 0, 0]
        component_failure_set = component_failure.ComponentFailureSet(f_failure_override=0.0)
        results = qra_analysis.conduct_analysis(self.pipe_outer_diam,
                                                self.pipe_thickness,
                                                self.amb_temp,
                                                self.amb_pres,
                                                self.rel_temp,
                                                self.rel_pres,
                                                self.rel_phase,
                                                self.facil_length,
                                                self.facil_width,
                                                self.immed_ign_probs,
                                                self.delayed_ign_probs,
                                                self.ign_thresholds,
                                                self.occupant_input_list,
                                                self.component_sets,
                                                component_failure_set,
                                                self.rel_species,
                                                self.leak_sizes,
                                                self.discharge_coeff,
                                                self.detection_credit,
                                                self.overp_method,
                                                self.TNT_equivalence_factor,
                                                self.BST_mach_flame_speed,
                                                self.probit_thermal_id,
                                                self.exposure_time,
                                                self.probit_overp_id,
                                                self.nozzle_model,
                                                self.rel_angle,
                                                self.rel_humid,
                                                self.rand_seed,
                                                self.excl_radius,
                                                release_freq_overrides,
                                                self.event_tree_override,
                                                self.verbose,
                                                self.output_dir,
                                                self.create_plots)
        risk_value = results['total_pll']
        self.assertEqual(risk_value, 0)


if __name__ == "__main__":
    unittest.main()
