"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import json
import unittest
from copy import deepcopy

from cs_api import qra
from hyram.qra.data import component_data
from hyram.utilities import misc_utils


"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

log = None
VERBOSE = False

DEFAULT_OUTPUT_DIR = misc_utils.get_temp_folder()

DEFAULT_OCCUPANT_DATA = [
    {"NumTargets": 9, "Desc": "Group 1", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 1,
     "XLocParamB": 20, "YLocDistribution": 2, "YLocParamA": 0, "YLocParamB": 0, "ZLocParamA": 1,
     "ZLocParamB": 12, "ParamUnitType": 0, "ExposureHours": 2000}]

DEFAULT_PARAMS = {
    'n_compressors': 0,
    'n_vessels': 0,
    'n_filters': 0,
    'n_flanges': 0,
    'n_hoses': 1,
    'n_joints': 35,
    'n_pipes': 20,
    'n_valves': 5,
    'n_instruments': 3,
    'n_exchangers': 0,
    'n_vaporizers': 0,
    'n_arms': 0,
    'n_extra_comp1': 0,
    'n_extra_comp2': 0,
    'facil_length': 20,
    'facil_width': 12,
    'pipe_outer_diam': .00952501905,
    'pipe_thickness': 0.001650033,
    'mass_flow': None,
    # 'mass_flow_leak_size': 1,
    'mass_flow_leak_size': None,

    'rel_species': 'H2',
    'rel_temp': 287.8,
    'rel_pres': 35000000,
    'rel_phase': 'none',
    'amb_temp': 288,
    'amb_pres': 101325,
    'discharge_coeff': 1,
    'n_vehicles': 20,
    'daily_fuelings': 2,
    'vehicle_days': 250,

    'immed_ign_probs': [.008, .053, .23],
    'delayed_ign_probs': [.004, .027, .12],
    'ign_thresholds': [.125, 6.25],

    'detection_credit': .9,
    'overp_method': 'bst',
    'tnt_factor': 0.03,
    'bst_flame_speed': 0.35,
    'probit_thermal_id': 'eis',
    'exposure_time': 60,
    'probit_overp_id': 'col',

    'nozzle_model': 'yuce',
    'release_angle': 0,
    'excl_radius': 0.01,
    'rand_seed': 3632850,
    'rel_humid': .89,

    'occupant_dist_json': json.dumps(DEFAULT_OCCUPANT_DATA),
    'compressor_ps': component_data.h2_gas_params['compressor'],
    'vessel_ps': component_data.h2_gas_params['vessel'],
    'valve_ps': component_data.h2_gas_params['valve'],
    'instrument_ps': component_data.h2_gas_params['instrument'],
    'pipe_ps': component_data.h2_gas_params['pipe'],
    'joint_ps': component_data.h2_gas_params['joint'],
    'hose_ps': component_data.h2_gas_params['hose'],
    'filter_ps': component_data.h2_gas_params['filter'],
    'flange_ps': component_data.h2_gas_params['flange'],
    'exchanger_ps': component_data.h2_gas_params['exchanger'],
    'vaporizer_ps': component_data.h2_gas_params['vaporizer'],
    'arm_ps': component_data.h2_gas_params['arm'],
    'extra_comp1_ps': component_data.h2_gas_params['extra1'],
    'extra_comp2_ps': component_data.h2_gas_params['extra2'],

    'noz_po_dist': 'Beta',
    'noz_po_a': .5,
    'noz_po_b': 610415.5,
    'noz_ftc_dist': 'ExpectedValue',
    'noz_ftc_a': .002,
    'noz_ftc_b': 0,
    'mvalve_ftc_dist': 'ExpectedValue',
    'mvalve_ftc_a': .001,
    'mvalve_ftc_b': 0,
    'svalve_ftc_dist': 'ExpectedValue',
    'svalve_ftc_a': .002,
    'svalve_ftc_b': 0,
    'svalve_ccf_dist': 'ExpectedValue',
    'svalve_ccf_a': 0.000128,
    'svalve_ccf_b': 0,
    'overp_dist': 'Beta',
    'overp_a': 3.5,
    'overp_b': 310289.5,
    'pvalve_fto_dist': 'LogNormal',
    'pvalve_fto_a': -11.7359368859313,
    'pvalve_fto_b': 0.67,
    'driveoff_dist': 'Beta',
    'driveoff_a': 31.5,
    'driveoff_b': 610384.5,
    'coupling_ftc_dist': 'Beta',
    'coupling_ftc_a': .5,
    'coupling_ftc_b': 5031,
    'f_release_000d01': None,
    'f_release_000d10': None,
    'f_release_001d00': None,
    'f_release_010d00': None,
    'f_release_100d00': None,
    'f_failure': None,
    'output_dir': DEFAULT_OUTPUT_DIR,
    'verbose': VERBOSE,
}


def set_leak_freq_data(param_dict, leak_freq_dict):
    """ Parse data dict of leak frequency probability params into dict format for analysis. """
    param_dict['compressor_ps'] = leak_freq_dict['compressor']
    param_dict['vessel_ps'] = leak_freq_dict['vessel']
    param_dict['valve_ps'] = leak_freq_dict['valve']
    param_dict['instrument_ps'] = leak_freq_dict['instrument']
    param_dict['pipe_ps'] = leak_freq_dict['pipe']
    param_dict['joint_ps'] = leak_freq_dict['joint']
    param_dict['hose_ps'] = leak_freq_dict['hose']
    param_dict['filter_ps'] = leak_freq_dict['filter']
    param_dict['flange_ps'] = leak_freq_dict['flange']
    param_dict['exchanger_ps'] = leak_freq_dict['exchanger']
    param_dict['vaporizer_ps'] = leak_freq_dict['vaporizer']
    param_dict['arm_ps'] = leak_freq_dict['arm']
    param_dict['extra_comp1_ps'] = leak_freq_dict['extra1']
    param_dict['extra_comp2_ps'] = leak_freq_dict['extra2']
    return param_dict


def set_component_counts(param_dict, n_compressors=0, n_vessels=0, n_filters=0, n_flanges=0, n_hoses=0,
                         n_joints=0, n_pipes=0, n_valves=0, n_instruments=0, n_exchangers=0,
                         n_vaporizers=0, n_arms=0, n_extra1=0, n_extra2=0):
    param_dict['n_compressors'] = n_compressors
    param_dict['n_vessels'] = n_vessels
    param_dict['n_filters'] = n_filters
    param_dict['n_flanges'] = n_flanges
    param_dict['n_hoses'] = n_hoses
    param_dict['n_joints'] = n_joints
    param_dict['n_pipes'] = n_pipes
    param_dict['n_valves'] = n_valves
    param_dict['n_instruments'] = n_instruments
    param_dict['n_exchangers'] = n_exchangers
    param_dict['n_vaporizers'] = n_vaporizers
    param_dict['n_arms'] = n_arms
    param_dict['n_extra_comp1'] = n_extra1
    param_dict['n_extra_comp2'] = n_extra2
    return param_dict


class HydrogenTestCase(unittest.TestCase):
    """
    Test QRA analysis of hydrogen fuel.
    """

    def setUp(self):
        self.output_dir = DEFAULT_OUTPUT_DIR
        self.qra_params = deepcopy(DEFAULT_PARAMS)

    def tearDown(self):
        pass

    def test_default_gas(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.h2_gas_params)
        self.qra_params = set_component_counts(self.qra_params,
                                               n_compressors=0, n_vessels=0, n_filters=0, n_flanges=0,
                                               n_hoses=1, n_joints=35, n_pipes=20, n_valves=5,
                                               n_instruments=3, n_exchangers=0, n_vaporizers=0, n_arms=0,
                                               n_extra1=0, n_extra2=0)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        data = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)
        self.assertTrue('air' in data)
        self.assertTrue('total_pll' in data)
        self.assertTrue('far' in data)
        self.assertTrue('positions' in data)

        self.assertTrue('position_qrads' in data)
        self.assertTrue('qrad_plot_files' in data)

        self.assertTrue('position_overps' in data)
        self.assertTrue('overp_plot_files' in data)

        self.assertTrue('position_impulses' in data)
        self.assertTrue('impulse_plot_files' in data)

        self.assertTrue('far' in data)
        self.assertTrue('leak_results' in data)

        leak_results = data['leak_results']
        self.assertTrue(len(leak_results) == 5)
        for res in leak_results:
            self.assertTrue('event_dicts' in res)
            self.assertTrue('list_p_events' in res)
            self.assertTrue('f_component_leaks' in res)
            self.assertTrue('leak_size' in res)
            self.assertTrue('mass_flow_rate' in res)
            self.assertTrue('leak_diam' in res)
            self.assertTrue('f_release' in res)
            self.assertTrue('f_release' in res)

        leak100 = data['leak_results'][-1]
        self.assertTrue('use_failure_override' in leak100)
        self.assertFalse(leak100['use_failure_override'])
        self.assertTrue('f_failure' in leak100)
        self.assertTrue('p_overp_rupture' in leak100)
        self.assertTrue('f_overp_rupture' in leak100)
        self.assertTrue('p_driveoff' in leak100)
        self.assertTrue('f_driveoff' in leak100)
        self.assertTrue('p_sol_valves_ftc' in leak100)
        self.assertTrue('f_sol_valves_ftc' in leak100)
        self.assertTrue('p_mvalve_ftc' in leak100)
        self.assertTrue('f_mvalve_ftc' in leak100)
        self.assertTrue('p_nozzle_release' in leak100)
        self.assertTrue('f_nozzle_release' in leak100)

    def test_invalid_occupant_sets_raises_error(self):
        qra.setup(self.output_dir, VERBOSE)
        occupant_dicts = [
            {"NumTargets": 9, "Desc": "Group 1", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 1,
             "XLocParamB": 20, "YLocDistribution": 2, "YLocParamA": 1, "YLocParamB": 0, "ZLocParamA": 1,
             "ZLocParamB": 12, "ParamUnitType": 0, "ExposureHours": 2000},
            {"NumTargets": 5, "Desc": "Group 2", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 0,
             "XLocParamB": 0, "YLocDistribution": 1, "YLocParamA": 0, "YLocParamB": 0, "ZLocParamA": 0,
             "ZLocParamB": 0, "ParamUnitType": 0, "ExposureHours": 50},
            {"NumTargets": 4, "Desc": "Group 3", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 0,
             "XLocParamB": 0, "YLocDistribution": 1, "YLocParamA": 0, "YLocParamB": 0, "ZLocParamA": 0,
             "ZLocParamB": 0, "ParamUnitType": 0, "ExposureHours": 50}
        ]
        self.qra_params['occupant_dist_json'] = json.dumps(occupant_dicts)

        result_dict = qra.c_request_analysis(**self.qra_params)

        status = result_dict['status']
        msg = result_dict['message']

        self.assertFalse(status)
        self.assertFalse(msg is None)

    def test_sat_vapor(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params['rel_phase'] = 'gas'
        self.qra_params['rel_temp'] = None
        self.qra_params['rel_pres'] = 1e6
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.h2_liquid_params)
        self.qra_params = set_component_counts(self.qra_params, n_vessels=1, n_flanges=1, n_hoses=1,
                                               n_joints=35, n_pipes=20, n_valves=5)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)

    def test_sat_liquid(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params['rel_phase'] = 'liquid'
        self.qra_params['rel_temp'] = None
        self.qra_params['rel_pres'] = 1e6
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.h2_liquid_params)
        self.qra_params = set_component_counts(self.qra_params, n_vessels=1, n_flanges=1, n_hoses=1,
                                               n_joints=35, n_pipes=20, n_valves=5)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)

    def test_mass_flow1(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.h2_gas_params)
        self.qra_params = set_component_counts(self.qra_params,
                                               n_compressors=0, n_vessels=0, n_filters=0, n_flanges=0,
                                               n_hoses=1, n_joints=35, n_pipes=20, n_valves=5,
                                               n_instruments=3, n_exchangers=0, n_vaporizers=0, n_arms=0,
                                               n_extra1=0, n_extra2=0)
        self.qra_params['mass_flow'] = 0.44
        self.qra_params['mass_flow_leak_size'] = 1
        result_dict = qra.c_request_analysis(**self.qra_params)

        status = result_dict['status']
        self.assertTrue(status)

        msg = result_dict['message']
        self.assertTrue(msg is None)

        data = result_dict['data']
        leak_results = data['leak_results']
        self.assertTrue(len(leak_results) == 5)

        leak100 = data['leak_results'][-1]
        self.assertTrue('list_p_events' in leak100)


class MethaneTestCase(unittest.TestCase):
    """
    Test QRA analysis of CH4.
    """

    def setUp(self):
        self.output_dir = DEFAULT_OUTPUT_DIR
        self.qra_params = deepcopy(DEFAULT_PARAMS)
        self.qra_params['rel_species'] = 'ch4'

    def tearDown(self):
        pass

    def test_default_gas(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.ch4_gas_params)
        self.qra_params = set_component_counts(self.qra_params, n_compressors=1, n_vessels=1, n_filters=1,
                                               n_flanges=1, n_hoses=1, n_joints=35, n_pipes=20, n_valves=1)

        print(self.qra_params)
        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)

    def test_sat_vapor(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params['rel_phase'] = 'gas'
        self.qra_params['rel_temp'] = None
        self.qra_params['rel_pres'] = 4e6
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.ch4_liquid_params)
        self.qra_params = set_component_counts(self.qra_params, n_vessels=1, n_flanges=1,
                                               n_hoses=1, n_joints=35, n_pipes=20, n_valves=1,
                                               n_exchangers=1, n_arms=1, n_vaporizers=1)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)

    def test_sat_liquid(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params['rel_phase'] = 'liquid'
        self.qra_params['rel_temp'] = None
        self.qra_params['rel_pres'] = 4e6
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.ch4_liquid_params)
        self.qra_params = set_component_counts(self.qra_params, n_vessels=1, n_flanges=1,
                                               n_hoses=1, n_joints=35, n_pipes=20, n_valves=1,
                                               n_exchangers=1, n_arms=1, n_vaporizers=1)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)


class PropaneTestCase(unittest.TestCase):
    """
    Test QRA analysis of C3H8.
    """

    def setUp(self):
        self.output_dir = DEFAULT_OUTPUT_DIR
        self.qra_params = deepcopy(DEFAULT_PARAMS)
        self.qra_params['rel_species'] = 'c3h8'

    def tearDown(self):
        pass

    def test_default_gui_request(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.c3h8_gas_params)
        self.qra_params = set_component_counts(self.qra_params, n_compressors=1, n_vessels=1, n_filters=1,
                                               n_flanges=1, n_hoses=1, n_joints=35, n_pipes=20, n_valves=1)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)

    def test_sat_vapor(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params['rel_phase'] = 'gas'
        self.qra_params['rel_temp'] = None
        self.qra_params['rel_pres'] = 4e6
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.c3h8_liquid_params)
        self.qra_params = set_component_counts(self.qra_params, n_compressors=1, n_vessels=1, n_filters=1,
                                               n_flanges=1, n_hoses=1, n_joints=35, n_pipes=20, n_valves=1)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)

    def test_sat_liquid(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params['rel_phase'] = 'liquid'
        self.qra_params['rel_temp'] = None
        self.qra_params['rel_pres'] = 4e6
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.c3h8_liquid_params)
        self.qra_params = set_component_counts(self.qra_params, n_compressors=1, n_vessels=1, n_filters=1,
                                               n_flanges=1, n_hoses=1, n_joints=35, n_pipes=20, n_valves=1)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)


class RegressionTestCase(unittest.TestCase):
    """
    Test prior analysis results of QRA analysis
    """

    def setUp(self):
        self.output_dir = DEFAULT_OUTPUT_DIR
        self.qra_params = deepcopy(DEFAULT_PARAMS)

    def tearDown(self):
        pass

    def test_default_h2_gas(self):
        qra.setup(self.output_dir, VERBOSE)
        self.qra_params = set_leak_freq_data(self.qra_params, component_data.h2_gas_params)
        self.qra_params['rand_seed'] = 3632850
        self.qra_params = set_component_counts(self.qra_params,
                                               n_compressors=0, n_vessels=0, n_filters=0, n_flanges=0,
                                               n_hoses=1, n_joints=35, n_pipes=20, n_valves=5,
                                               n_instruments=3, n_exchangers=0, n_vaporizers=0, n_arms=0,
                                               n_extra1=0, n_extra2=0)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        data = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)
        self.assertTrue('leak_results' in data)

        self.assertAlmostEqual(data['total_pll'], 7.51e-6, places=8)
        self.assertAlmostEqual(data['far'], 9.526e-3, places=5)
        self.assertAlmostEqual(data['air'], 1.91e-7, places=8)

        leak_results = data['leak_results']
        leak100 = data['leak_results'][-1]
        self.assertAlmostEqual(leak100['f_release'], 4.78e-4, places=6)
        self.assertAlmostEqual(leak100['f_failure'], 5.47e-5, places=7)


if __name__ == "__main__":
    unittest.main()
