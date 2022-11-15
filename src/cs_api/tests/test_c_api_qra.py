"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    'num_compressors': 0,
    'num_vessels': 0,
    'num_filters': 0,
    'num_flanges': 0,
    'num_hoses': 1,
    'num_joints': 35,
    'pipe_length': 20,
    'num_valves': 5,
    'num_instruments': 3,
    'num_exchangers': 0,
    'num_vaporizers': 0,
    'num_arms': 0,
    'num_extra_comp1': 0,
    'num_extra_comp2': 0,
    'facil_length': 20,
    'facil_width': 12,
    'pipe_outer_diam': .00952501905,
    'pipe_thickness': 0.001650033,

    'rel_species': 'H2',
    'rel_temp': 287.8,
    'rel_pres': 35000000,
    'rel_phase': 'none',
    'amb_temp': 288,
    'amb_pres': 101325,
    'discharge_coeff': 1,
    'num_vehicles': 20,
    'daily_fuelings': 2,
    'vehicle_days': 250,

    'immed_ign_probs': [.008, .053, .23],
    'delayed_ign_probs': [.004, .027, .12],
    'ign_thresholds': [.125, 6.25],

    'detection_credit': .9,
    'overp_method': 'bst',
    'TNT_equivalence_factor': 0.03,
    'BST_mach_flame_speed': 0.35,
    'probit_thermal_id': 'eis',
    'exposure_time': 60,
    'probit_overp_id': 'col',

    'nozzle_model': 'yuce',
    'release_angle': 0,
    'excl_radius': 0.01,
    'rand_seed': 3632850,
    'rel_humid': .89,

    'occupant_dist_json': json.dumps(DEFAULT_OCCUPANT_DATA),
    'compressor_leak_probs': component_data.h2_gas_params['compressor'],
    'vessel_leak_probs': component_data.h2_gas_params['vessel'],
    'valve_leak_probs': component_data.h2_gas_params['valve'],
    'instrument_leak_probs': component_data.h2_gas_params['instrument'],
    'pipe_leak_probs': component_data.h2_gas_params['pipe'],
    'joint_leak_probs': component_data.h2_gas_params['joint'],
    'hose_leak_probs': component_data.h2_gas_params['hose'],
    'filter_leak_probs': component_data.h2_gas_params['filter'],
    'flange_leak_probs': component_data.h2_gas_params['flange'],
    'exchanger_leak_probs': component_data.h2_gas_params['exchanger'],
    'vaporizer_leak_probs': component_data.h2_gas_params['vaporizer'],
    'arm_leak_probs': component_data.h2_gas_params['arm'],
    'extra_comp1_leak_probs': component_data.h2_gas_params['extra1'],
    'extra_comp2_leak_probs': component_data.h2_gas_params['extra2'],

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
    'rel_freq_000d01': -1,
    'rel_freq_000d10': -1,
    'rel_freq_001d00': -1,
    'rel_freq_010d00': -1,
    'rel_freq_100d00': -1,
    'fueling_fail_freq_override': -1,
    'output_dir': DEFAULT_OUTPUT_DIR,
    'verbose': VERBOSE,
}


def set_leak_freq_data(param_dict, leak_freq_dict):
    param_dict['compressor_leak_probs'] = leak_freq_dict['compressor']
    param_dict['vessel_leak_probs'] = leak_freq_dict['vessel']
    param_dict['valve_leak_probs'] = leak_freq_dict['valve']
    param_dict['instrument_leak_probs'] = leak_freq_dict['instrument']
    param_dict['pipe_leak_probs'] = leak_freq_dict['pipe']
    param_dict['joint_leak_probs'] = leak_freq_dict['joint']
    param_dict['hose_leak_probs'] = leak_freq_dict['hose']
    param_dict['filter_leak_probs'] = leak_freq_dict['filter']
    param_dict['flange_leak_probs'] = leak_freq_dict['flange']
    param_dict['exchanger_leak_probs'] = leak_freq_dict['exchanger']
    param_dict['vaporizer_leak_probs'] = leak_freq_dict['vaporizer']
    param_dict['arm_leak_probs'] = leak_freq_dict['arm']
    param_dict['extra_comp1_leak_probs'] = leak_freq_dict['extra1']
    param_dict['extra_comp2_leak_probs'] = leak_freq_dict['extra2']
    return param_dict


def set_component_counts(param_dict, num_compressors=0, num_vessels=0, num_filters=0, num_flanges=0, num_hoses=0,
                         num_joints=0, num_pipes=0, num_valves=0, num_instruments=0, num_exchangers=0,
                         num_vaporizers=0, num_arms=0, num_extra1=0, num_extra2=0):
    param_dict['num_compressors'] = num_compressors
    param_dict['num_vessels'] = num_vessels
    param_dict['num_filters'] = num_filters
    param_dict['num_flanges'] = num_flanges
    param_dict['num_hoses'] = num_hoses
    param_dict['num_joints'] = num_joints
    param_dict['pipe_length'] = num_pipes
    param_dict['num_valves'] = num_valves
    param_dict['num_instruments'] = num_instruments
    param_dict['num_exchangers'] = num_exchangers
    param_dict['num_vaporizers'] = num_vaporizers
    param_dict['num_arms'] = num_arms
    param_dict['num_extra_comp1'] = num_extra1
    param_dict['num_extra_comp2'] = num_extra2
    return param_dict


class TestHydrogen(unittest.TestCase):
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
                                               num_compressors=0, num_vessels=0, num_filters=0, num_flanges=0,
                                               num_hoses=1, num_joints=35, num_pipes=20, num_valves=5,
                                               num_instruments=3, num_exchangers=0, num_vaporizers=0, num_arms=0,
                                               num_extra1=0, num_extra2=0)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        print(result_dict)
        self.assertTrue(status)
        self.assertTrue(msg is None)

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
        self.qra_params = set_component_counts(self.qra_params, num_vessels=1, num_flanges=1, num_hoses=1,
                                               num_joints=35, num_pipes=20, num_valves=5)

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
        self.qra_params = set_component_counts(self.qra_params, num_vessels=1, num_flanges=1, num_hoses=1,
                                               num_joints=35, num_pipes=20, num_valves=5)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)


class TestMethane(unittest.TestCase):
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
        self.qra_params = set_component_counts(self.qra_params, num_compressors=1, num_vessels=1, num_filters=1,
                                               num_flanges=1, num_hoses=1, num_joints=35, num_pipes=20, num_valves=1)

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
        self.qra_params = set_component_counts(self.qra_params, num_vessels=1, num_flanges=1,
                                               num_hoses=1, num_joints=35, num_pipes=20, num_valves=1,
                                               num_exchangers=1, num_arms=1, num_vaporizers=1)

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
        self.qra_params = set_component_counts(self.qra_params, num_vessels=1, num_flanges=1,
                                               num_hoses=1, num_joints=35, num_pipes=20, num_valves=1,
                                               num_exchangers=1, num_arms=1, num_vaporizers=1)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)


class TestPropane(unittest.TestCase):
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
        self.qra_params = set_component_counts(self.qra_params, num_compressors=1, num_vessels=1, num_filters=1,
                                               num_flanges=1, num_hoses=1, num_joints=35, num_pipes=20, num_valves=1)

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
        self.qra_params = set_component_counts(self.qra_params, num_compressors=1, num_vessels=1, num_filters=1,
                                               num_flanges=1, num_hoses=1, num_joints=35, num_pipes=20, num_valves=1)

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
        self.qra_params = set_component_counts(self.qra_params, num_compressors=1, num_vessels=1, num_filters=1,
                                               num_flanges=1, num_hoses=1, num_joints=35, num_pipes=20, num_valves=1)

        result_dict = qra.c_request_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        self.assertTrue(status)
        self.assertTrue(msg is None)


if __name__ == "__main__":
    unittest.main()
