"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import numpy as np
import unittest
import logging
from hyram.phys import api, Fluid
from hyram.utilities import misc_utils
from math import isnan

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


# @unittest.skip
class OverpressureTestCase(unittest.TestCase):
    """
    Test overpressure calculation.
    Units are: kg, Pa, J, m
    """
    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        logname = __name__
        misc_utils.setup_file_log(self.output_dir, verbose=VERBOSE, logfile='hyram-test.log', logname=logname)
        self.log = logging.getLogger(logname)

    def tearDown(self):
        pass

    def test_basic_bst_overpressure(self):
        method = 'BST'
        locations = [np.array([1., 1., 0.]),] # distance is sqrt(2) m
        P_inf = 101325.  # Pa, ambient pressure
        jet_T = 300.  # K, jet exit temperature
        jet_P = 60e5  # Pa, jet exit pressure
        jet_d = 2*0.0254  #m, jet exit diameter
        # TODO : should a Fluid object be created here directly or should api.create_fluid be used?
        ambient_fluid = Fluid(P = P_inf, T = jet_T, species='air')
        release_fluid = Fluid(T = jet_T, P = jet_P, species = 'hydrogen')
        orifice_diameter = jet_d

        self.log.info("TESTING BST overpressure calculation")
        result_dict = api.compute_overpressure(method, locations, ambient_fluid=ambient_fluid,
                                               release_fluid=release_fluid, orifice_diameter=orifice_diameter,
                                               output_dir=self.output_dir)

        overpressure = result_dict["overpressure"]
        impulse = result_dict["impulse"]
        figure_path = result_dict["figure_file_path"]

        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0.0)
        self.assertGreaterEqual(impulse, 0.0)
        self.assertTrue(os.path.exists(figure_path))
    
    def test_basic_tnt_overpressure(self):
        method = 'TNT'
        locations = [np.array([1., 1., 0.]),] # distance is sqrt(2) m
        P_inf = 101325.  # Pa, ambient pressure
        jet_T = 300.  # K, jet exit temperature
        jet_P = 60e5  # Pa, jet exit pressure
        jet_d = 2*0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(P = P_inf, T = jet_T, species='air')
        release_fluid = Fluid(T = jet_T, P = jet_P, species = 'hydrogen')
        orifice_diameter = jet_d

        self.log.info("TESTING TNT overpressure calculation")
        result_dict = api.compute_overpressure(method, locations, ambient_fluid=ambient_fluid,
                                               release_fluid=release_fluid, orifice_diameter=orifice_diameter,
                                               output_dir=self.output_dir)

        overpressure = result_dict["overpressure"]
        impulse = result_dict["impulse"]
        figure_path = result_dict["figure_file_path"]

        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0.0)
        self.assertGreaterEqual(impulse, 0.0)
        self.assertTrue(os.path.exists(figure_path))

    def test_basic_bauwens_overpressure(self):
        method = 'Bauwens'
        locations = [np.array([1., 1., 0.]),] # distance is sqrt(2) m
        P_inf = 101325.  # Pa, ambient pressure
        jet_T = 300.  # K, jet exit temperature
        jet_P = 60e5  # Pa, jet exit pressure
        jet_d = 2*0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(P = P_inf, T = jet_T, species='air')
        release_fluid = Fluid(T = jet_T, P = jet_P, species = 'hydrogen')
        orifice_diameter = jet_d

        self.log.info("TESTING Bauwens overpressure calculation")
        result_dict = api.compute_overpressure(method, locations, ambient_fluid=ambient_fluid,
                                               release_fluid=release_fluid, orifice_diameter=orifice_diameter,
                                               output_dir=self.output_dir)

        overpressure = result_dict["overpressure"]
        impulse = result_dict["impulse"]
        figure_path = result_dict["figure_file_path"]

        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0.0)
        self.assertTrue(isnan(impulse[0]))
        self.assertTrue(os.path.exists(figure_path))


if __name__ == "__main__":
    unittest.main()
