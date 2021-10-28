"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest

import numpy as np
from scipy import constants as const

from hyram.phys import c_api

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = True


class OverpressureTestCase(unittest.TestCase):

    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        c_api.setup(self.output_dir, verbose=VERBOSE)

        self.default_params = {
            'amb_temp': 288.,
            'amb_pres': 101325.,
            'rel_species': 'H2',
            'rel_temp': 287.8,
            'rel_pres': 35000000,
            'rel_phase': 'none',
            'orif_diam': 0.00356,
            'rel_angle': 0.,
            'discharge_coeff': 1.0,
            'nozzle_model': 'yuce',
            'method': 'BST',
            # 'heat_of_combustion': None,
            'xlocs': np.array([1., 2., 1.]),
            'ylocs': np.array([1., 2., 1.]),
            'zlocs': np.array([0., 0., 1.]),
            'bst_flame_speed': 5.2,
            'tnt_factor': 0.03,
            'output_dir': self.output_dir,
            'verbose': VERBOSE,
        }

    def tearDown(self):
        pass

    def test_default(self):
        wrapped = c_api.unconfined_overpressure_analysis(**self.default_params)
        status = wrapped["status"]
        result_dict = wrapped["data"]
        message = wrapped["message"]
        warning = wrapped["warning"]

        overpressure = result_dict['overpressure']
        impulse = result_dict['impulse']

        self.assertTrue(status)
        self.assertTrue('figure_file_path' in result_dict)
        self.assertTrue(result_dict['figure_file_path'] is not None)

        np.testing.assert_almost_equal(overpressure, [28571., 32697., 28494], decimal=0)
        np.testing.assert_almost_equal(impulse, [38.2, 42.9, 38.0], decimal=1)
