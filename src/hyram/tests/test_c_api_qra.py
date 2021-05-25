"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

import copy
import json
import os
import unittest

import numpy as np

from hyram.qra import c_api
from hyram.utilities import exceptions

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

log = None
VERBOSE = False


# @unittest.skip
class TestQRA(unittest.TestCase):
    """
    Test QRA analysis.
    """

    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')

        # distr_labels = ['norm', 'unif', 'dete']  are 0, 1, 2
        occupant_dicts = [
            {"NumTargets": 9, "Desc": "Group 1", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 1.0,
             "XLocParamB": 20.0, "YLocDistribution": 2, "YLocParamA": 1.0, "YLocParamB": 0.0, "ZLocParamA": 1.0,
             "ZLocParamB": 12.0, "ParamUnitType": 0, "ExposureHours": 2000.0}]

        # TODO: update these numbers to new defaults
        self.compressor_leak_probs = [[-1.7198, 0.2143],
                                      [-3.9185, 0.4841],
                                      [-5.1394, 0.7898],
                                      [-8.8408, 0.8381],
                                      [-11.3365, 1.3689]]
        self.cylinder_leak_probs = [[-13.8364, 0.6156], [-14.001, 0.6065],
                                    [-14.3953, 0.6232], [-14.9562, 0.629],
                                    [-15.6047, 0.6697]]
        self.valve_leak_probs = [[-5.1796, 0.1728], [-7.2748, 0.3983],
                                 [-9.6802, 0.9607], [-10.323, 0.6756],
                                 [-11.996, 1.3304]]
        self.instrument_leak_probs = [[-7.3205, 0.6756], [-8.5018, 0.7938],
                                      [-9.0619, 0.8952], [-9.1711, 1.0674],
                                      [-10.1962, 1.4795]]
        self.pipe_leak_probs = [[-11.8584, 0.657], [-12.5337, 0.6884],
                                [-13.8662, 1.1276], [-14.5757, 1.1555],
                                [-15.7261, 1.714]]
        self.joint_leak_probs = [[-9.5738, 0.1638], [-12.8316, 0.7575],
                                 [-11.8743, 0.475], [-12.0156, 0.5302],
                                 [-12.1486, 0.5652]]
        self.hose_leak_probs = [[-6.8061, 0.2682], [-8.6394, 0.552],
                                [-8.774, 0.5442], [-8.8926, 0.5477],
                                [-9.86, 0.8457]]
        self.filter_leak_probs = [[-5.2471, 1.9849], [-5.2884, 1.518],
                                  [-5.3389, 1.4806], [-5.3758, 0.8886],
                                  [-5.4257, 0.9544]]
        self.flange_leak_probs = [[-3.9236, 1.6611], [-6.1211, 1.2533],
                                  [-8.3307, 2.2024], [-10.5399, 0.8332],
                                  [-12.7453, 1.8274]]
        self.extra_comp1_leak_probs = [[0.0, 0.0], [0.0, 0.0], [0.0, 0.0],
                                       [0.0, 0.0], [0.0, 0.0]]
        self.extra_comp2_leak_probs = [[0.0, 0.0], [0.0, 0.0], [0.0, 0.0],
                                       [0.0, 0.0], [0.0, 0.0]]

        self.qra_params = {
            'pipe_length': 20.,
            'num_compressors': 0,
            'num_cylinders': 0,
            'num_valves': 5,
            'num_instruments': 3,
            'num_joints': 35,
            'num_hoses': 1,
            'num_filters': 0,
            'num_flanges': 0,
            'num_extra_comp1': 0,
            'num_extra_comp2': 0,
            'facil_length': 20.,
            'facil_width': 12.,
            'facil_height': 5.,
            'pipe_outer_diam': .00952501905,
            'pipe_thickness': 0.001650033,

            'rel_species': 'H2',
            'rel_temp': 288.15,
            'rel_pres': 35000000,
            'rel_phase': 'default',
            'amb_temp': 288.15,
            'amb_pres': 101325.,
            'discharge_coeff': 1.,
            'num_vehicles': 20.,
            'daily_fuelings': 2,
            'vehicle_days': 250.,

            'immed_ign_probs': [.008, .053, .23],
            'delayed_ign_probs': [.004, .027, .12],
            'ign_thresholds': [.125, 6.25],

            'detect_gas_flame': True,
            'detection_credit': .9,
            'probit_thermal_id': 'eis',
            'exposure_time': 60.,
            'probit_rel_id': 'col',
            'peak_overp_list': [2500., 2500., 5000., 16000., 30000.],
            'overp_impulse_list': [250., 500., 1000., 2000., 4000.],
            'overp_frag_mass': 0.,
            'overp_velocity': 0.,
            'overp_total_mass': 0.,

            'nozzle_model': 'yuce',
            'leak_height': 0.,
            'release_angle': 0.,
            'excl_radius': 0.01,
            'rand_seed': 3632850,
            'rel_humid': .89,

            'occupant_dist_json': json.dumps(occupant_dicts),

            'compressor_leak_probs': self.compressor_leak_probs,
            'cylinder_leak_probs': self.cylinder_leak_probs,
            'valve_leak_probs': self.valve_leak_probs,
            'instrument_leak_probs': self.instrument_leak_probs,
            'pipe_leak_probs': self.pipe_leak_probs,
            'joint_leak_probs': self.joint_leak_probs,
            'hose_leak_probs': self.hose_leak_probs,
            'filter_leak_probs': self.filter_leak_probs,
            'flange_leak_probs': self.flange_leak_probs,
            'extra_comp1_leak_probs': self.extra_comp1_leak_probs,
            'extra_comp2_leak_probs': self.extra_comp2_leak_probs,

            'noz_po_dist': 'Beta',
            'noz_po_a': .5,
            'noz_po_b': 610415.5,
            'noz_ftc_dist': 'ExpectedValue',
            'noz_ftc_a': .002,
            'noz_ftc_b': 0.,
            'mvalve_ftc_dist': 'ExpectedValue',
            'mvalve_ftc_a': .001,
            'mvalve_ftc_b': 0.,
            'svalve_ftc_dist': 'ExpectedValue',
            'svalve_ftc_a': .002,
            'svalve_ftc_b': 0.,
            'svalve_ccf_dist': 'ExpectedValue',
            'svalve_ccf_a': 0.00012766,
            'svalve_ccf_b': 0.,
            'overp_dist': 'Beta',
            'overp_a': 3.5,
            'overp_b': 310289.5,
            'pvalve_fto_dist': 'LogNormal',
            'pvalve_fto_a': -11.7359368859313,
            'pvalve_fto_b': 0.667849415603714,
            'driveoff_dist': 'Beta',
            'driveoff_a': 31.5,
            'driveoff_b': 610384.5,
            'coupling_ftc_dist': 'Beta',
            'coupling_ftc_a': .5,
            'coupling_ftc_b': 5031.,
            'rel_freq_000d01': -1.,
            'rel_freq_000d10': -1.,
            'rel_freq_001d00': -1.,
            'rel_freq_010d00': -1.,
            'rel_freq_100d00': -1.,
            'fueling_fail_freq_override': -1.,
            'output_dir': self.output_dir,
            'verbose': VERBOSE,
        }

    def tearDown(self):
        pass

    # @unittest.skip
    def test_default(self):
        c_api.setup(self.output_dir, VERBOSE)
        result_dict = c_api.qra_analysis(**self.qra_params)
        status = result_dict['status']
        results = result_dict['data']
        msg = result_dict['message']

        np.testing.assert_almost_equal(results['total_pll'], 1.1e-5, decimal=6)
        np.testing.assert_almost_equal(results['far'], 0.01395, decimal=5)
        np.testing.assert_almost_equal(results['air'], 2.8e-7, decimal=8)
        self.assertTrue(status)
        self.assertTrue(msg is None)

    # @unittest.skip
    def test_invalid_occupant_sets_raises_error(self):
        c_api.setup(self.output_dir, VERBOSE)
        occupant_dicts = [
            {"NumTargets": 9, "Desc": "Group 1", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 1.0,
             "XLocParamB": 20.0, "YLocDistribution": 2, "YLocParamA": 1.0, "YLocParamB": 0.0, "ZLocParamA": 1.0,
             "ZLocParamB": 12.0, "ParamUnitType": 0, "ExposureHours": 2000.0},
            {"NumTargets": 5, "Desc": "Group 2", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 0.0,
             "XLocParamB": 0.0, "YLocDistribution": 1, "YLocParamA": 0.0, "YLocParamB": 0.0, "ZLocParamA": 0.0,
             "ZLocParamB": 0.0, "ParamUnitType": 0, "ExposureHours": 50.0},
            {"NumTargets": 4, "Desc": "Group 3", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 0.0,
             "XLocParamB": 0.0, "YLocDistribution": 1, "YLocParamA": 0.0, "YLocParamB": 0.0, "ZLocParamA": 0.0,
             "ZLocParamB": 0.0, "ParamUnitType": 0, "ExposureHours": 50.0}
        ]
        self.qra_params['occupant_dist_json'] = json.dumps(occupant_dicts)

        result_dict = c_api.qra_analysis(**self.qra_params)

        status = result_dict['status']
        msg = result_dict['message']

        self.assertFalse(status)
        self.assertFalse(msg is None)


if __name__ == "__main__":
    unittest.main()
