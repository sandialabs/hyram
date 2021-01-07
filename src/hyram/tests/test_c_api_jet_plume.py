import os
import pdb

import numpy as np
import unittest
from hyram.phys import c_api, Jet, Orifice, Fluid
from hyram.utilities import misc_utils, exceptions, constants

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""


# @unittest.skip
class H2JetPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis.
    """
    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        self.debug = False
        self.verbose = False
        c_api.setup(self.output_dir, self.debug)

    def tearDown(self):
        pass

    # @unittest.skip
    def test_default(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_pres = 13420000.
        rel_temp = 287.8
        rel_phase = None

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
                                          plot_title, self.output_dir, self.verbose, self.debug)

        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        filepath = data_dict["plot"]
        self.assertTrue(filepath is not None)
        self.assertTrue(os.path.isfile(filepath))

    # @unittest.skip
    def test_too_many_fluid_params_fails(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_pres = 13420000.
        rel_temp = 287.8
        rel_phase = 'gas'

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
                                          plot_title, self.output_dir, self.verbose, self.debug)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    # @unittest.skip
    def test_too_few_fluid_params_fails(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_pres = None
        rel_temp = 287.8
        rel_phase = None

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
                                          plot_title, self.output_dir, self.verbose, self.debug)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))



class LH2JetPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis.
    """
    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        self.debug = True
        self.verbose = True
        c_api.setup(self.output_dir, self.debug)

    def tearDown(self):
        pass

    # @unittest.skip
    def test_default_LH2(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 2.0 * constants.ATM_TO_PA
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
                                          plot_title, self.output_dir, self.verbose, self.debug)

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
        rel_pres = 10.0 * constants.ATM_TO_PA
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
                                          plot_title, self.output_dir, self.verbose, self.debug)

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
        rel_pres = 1.0 * constants.ATM_TO_PA
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
                                          plot_title, self.output_dir, self.verbose, self.debug)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_pressure_past_critical_fails(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 13. * constants.ATM_TO_PA
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
                                          plot_title, self.output_dir, self.verbose, self.debug)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))


if __name__ == "__main__":
    unittest.main()
