"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest

from scipy import constants as const

from cs_api import phys, utils
from hyram.utilities import misc_utils


"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


class H2JetPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis.
    """
    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        utils.setup_file_log(self.output_dir, verbose=VERBOSE)
        self.params = {'rel_species': {'h2': 1, 'ch4': 0},
                       'amb_temp': 288.15,
                       'amb_pres': 101325.,
                       'rel_temp': 287.8,
                       'rel_pres': 13420000.,
                       'rel_phase': None,
                       'mass_flow': None,
                       'orif_diam': 0.00356,
                       'rel_angle': 1.5708,
                       'dis_coeff': 1.0,
                       'nozzle_model': 'yuce',
                       'contours': 0.04,
                       'xmin': -2.5,
                       'xmax': 2.5,
                       'ymin': 0,
                       'ymax': 10,
                       'vmin': 0,
                       'vmax': 0.1,
                       'plot_title': 'Plume test',
                       'output_dir': self.output_dir,
                       'verbose': VERBOSE}

    def tearDown(self):
        pass

    def test_default(self):
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']
        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))
        self.assertTrue(not warning)

    def test_too_many_fluid_params_fails(self):
        self.params['rel_phase'] = 'gas'
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_too_few_fluid_params_fails(self):
        self.params['rel_pres'] = None
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_unchoked_flow_returns_warning(self):
        self.params['rel_pres'] = 151325
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']

        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))
        self.assertTrue(warning)

    def test_contour_invalid_0(self):
        self.params['contours'] = 0.0
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_contour_invalid_1(self):
        self.params['contours'] = 1.0
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))


class LH2JetPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis.
    """
    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        utils.setup_file_log(self.output_dir, verbose=VERBOSE)

        self.params = {'rel_species': {'h2': 1},
                       'amb_temp': 288.15,
                       'amb_pres': 101325.,
                       'rel_temp': None,
                       'rel_pres': 2 * const.atm,
                       'mass_flow': None,
                       'rel_phase': 'liquid',
                       'orif_diam': 0.00356,
                       'rel_angle': 1.5708,
                       'dis_coeff': 1.0,
                       'nozzle_model': 'yuce',
                       'contours': 0.04,
                       'xmin': -2.5,
                       'xmax': 2.5,
                       'ymin': 0,
                       'ymax': 10,
                       'vmin': 0,
                       'vmax': 0.1,
                       'plot_title': 'Plume test',
                       'output_dir': self.output_dir,
                       'verbose': VERBOSE}

    def tearDown(self):
        pass

    def test_default_LH2(self):
        wrapped = phys.analyze_jet_plume(**self.params)
        data_dict = wrapped['data']

        # Ensure plot file exists
        filepath = data_dict["plot"]
        self.assertTrue(filepath is not None)
        self.assertTrue(os.path.isfile(filepath))

    def test_higher_pressure(self):
        self.params['rel_pres'] = 10 * const.atm
        wrapped = phys.analyze_jet_plume(**self.params)
        data_dict = wrapped['data']
        filepath = data_dict["plot"]
        self.assertTrue(filepath is not None)
        self.assertTrue(os.path.isfile(filepath))

    def test_equal_pressure_fails(self):
        self.params['rel_pres'] = 1 * const.atm
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_pressure_past_critical_fails(self):
        self.params['rel_pres'] = 13 * const.atm
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))


class BlendPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis with blend.
    """
    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        utils.setup_file_log(self.output_dir, verbose=VERBOSE)
        self.params = dict(rel_species={'h2': 1, 'ch4': 0},
                           amb_temp=288.15,
                           amb_pres=101325.,
                           rel_temp=287.8,
                           rel_pres=13420000.,
                           mass_flow=None,
                           rel_phase=None,
                           orif_diam=0.00356,
                           rel_angle=1.5708,
                           dis_coeff=1.0,
                           nozzle_model='yuce',
                           contours=0.04,
                           xmin=-2.5,
                           xmax=2.5,
                           ymin=0,
                           ymax=10,
                           vmin=0,
                           vmax=0.1,
                           plot_title='Plume test',
                           output_dir=self.output_dir,
                           verbose=VERBOSE)

    def tearDown(self):
        pass

    def test_n2_methane(self):
        blend = {'ch4': 0.965, 'n2': 0.035}
        self.params['rel_species'] = blend
        wrapped = phys.analyze_jet_plume(**self.params)
        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']
        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))
        self.assertTrue(not warning)

    def test_default_ch4(self):
        blend = {'h2': 0.0, 'ch4': 1.0}
        self.params['rel_species'] = blend
        wrapped = phys.analyze_jet_plume(**self.params)

        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']
        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))
        self.assertTrue(not warning)


if __name__ == "__main__":
    unittest.main()
