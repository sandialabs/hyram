"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest

from scipy import constants as const

from hyram.phys import c_api
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
        c_api.setup(self.output_dir, VERBOSE)

    def tearDown(self):
        pass

    def test_default(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=13420000.,
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']
        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))
        self.assertTrue(not warning)

    def test_too_many_fluid_params_fails(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=13420000.,
                rel_phase='gas',
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_too_few_fluid_params_fails(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=None,
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_unchoked_flow_returns_warning(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=151325.,  # low
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']

        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))

        self.assertTrue(warning)

    def test_contour_invalid_0(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=13420000.,
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.0,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_contour_invalid_1(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=13420000.,
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=1.0,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))


class LH2JetPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis.
    """
    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        c_api.setup(self.output_dir, VERBOSE)

    def tearDown(self):
        pass

    def test_default_LH2(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 2.0 * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        data_dict = wrapped['data']

        # Ensure plot file exists
        filepath = data_dict["plot"]
        self.assertTrue(filepath is not None)
        self.assertTrue(os.path.isfile(filepath))

    def test_higher_pressure(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 10.0 * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        data_dict = wrapped['data']

        # Ensure plot file exists
        filepath = data_dict["plot"]
        self.assertTrue(filepath is not None)
        self.assertTrue(os.path.isfile(filepath))

    def test_equal_pressure_fails(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 1.0 * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_pressure_past_critical_fails(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 13. * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))


if __name__ == "__main__":
    unittest.main()
