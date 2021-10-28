"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
import os
import unittest

import numpy as np
from scipy import constants as const

from hyram.phys import api
from hyram.utilities import misc_utils

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


class TestPositionalFlux(unittest.TestCase):
    """
    Test plume analysis.
    """

    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')

        logname = __name__
        misc_utils.setup_file_log(self.output_dir, verbose=VERBOSE, logfile='hyram-test.log', logname=logname)
        self.log = logging.getLogger(logname)

    def tearDown(self):
        pass

    def test_default(self):
        amb_species = "air"
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_pres = 35.e6
        rel_temp = 288.15
        rel_density = None
        rel_phase = None

        rel_angle = 0.
        rel_height = 0.

        site_length = 20.
        site_width = 12.

        pipe_outer_diam = .375 * const.inch
        pipe_thickness = .065 * const.inch

        # Replicate orifice leak diam calc
        pipe_inner_diam = pipe_outer_diam - 2. * pipe_thickness
        pipe_area = np.pi * (pipe_inner_diam / 2.) ** 2.
        leak_sizes = [0.01, 0.10, 1.00, 10.00, 100.00]
        leak_areas = (np.array(leak_sizes) / 100.) * pipe_area
        orif_leak_diams = np.sqrt(4. * (leak_areas / np.pi))

        rel_humid = 0.89
        dis_coeff = 1.0
        rad_src_model = 'multi'

        not_nozzle_model = "yuce"

        loc_distributions = [[9, ('unif', 1.0, 20.0), ('dete', 1.0, 0.0), ('unif', 1.0, 12.0)]]

        excl_radius = .01
        rand_seed = 3632850

        self.log.info("TESTING QRA FLUX")
        amb_fluid = api.create_fluid(amb_species, amb_temp, amb_pres, None, None)
        rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, rel_density, rel_phase)

        result_dict = api.flux_analysis(amb_fluid, rel_fluid,
                                        rel_height, rel_angle,
                                        site_length, site_width,
                                        orif_leak_diams, rel_humid, dis_coeff,
                                        rad_src_model, not_nozzle_model,
                                        loc_distributions,
                                        excl_radius, rand_seed,
                                        output_dir=self.output_dir, verbose=VERBOSE)

        fluxes = result_dict['fluxes']
        fluxes *= 1000.
        xlocs = result_dict['xlocs']
        ylocs = result_dict['ylocs']
        zlocs = result_dict['zlocs']
        positions = result_dict['positions']

        self.assertTrue(result_dict is not None)
        self.assertTrue(q >= 0.0 for q in fluxes)

    def test_three_occupant_sets(self):
        amb_species = "air"
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_pres = 35.e6
        rel_temp = 288.15
        rel_density = None
        rel_phase = None

        rel_angle = 0.
        rel_height = 0.

        site_length = 20.
        site_width = 12.

        pipe_outer_diam = .375 * const.inch
        pipe_thickness = .065 * const.inch

        # Replicate orifice leak diam calc
        pipe_inner_diam = pipe_outer_diam - 2. * pipe_thickness
        pipe_area = np.pi * (pipe_inner_diam / 2.) ** 2.
        leak_sizes = [0.01, 0.10, 1.00, 10.00, 100.00]
        leak_areas = (np.array(leak_sizes) / 100.) * pipe_area
        orif_leak_diams = np.sqrt(4. * (leak_areas / np.pi))

        rel_humid = 0.89
        dis_coeff = 1.0
        rad_src_model = 'multi'

        not_nozzle_model = "yuce"

        loc_distributions = [
            [9, ('unif', 1.0, 20.0), ('dete', 1.0, 0.0), ('unif', 1.0, 12.0)],
            [5, ('unif', 1.0, 20.0), ('dete', 1.0, 0.0), ('unif', 1.0, 12.0)],
            [4, ('dete', 1.0, 0), ('dete', 5.0, 0.0), ('dete', 1.0, 0)]
        ]

        excl_radius = .01
        rand_seed = 3632850

        self.log.info("TESTING QRA FLUX")
        amb_fluid = api.create_fluid(amb_species, amb_temp, amb_pres, None, None)
        rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, rel_density, rel_phase)

        result_dict = api.flux_analysis(amb_fluid, rel_fluid,
                                        rel_height, rel_angle,
                                        site_length, site_width,
                                        orif_leak_diams, rel_humid, dis_coeff,
                                        rad_src_model, not_nozzle_model,
                                        loc_distributions,
                                        excl_radius, rand_seed,
                                        output_dir=self.output_dir, verbose=VERBOSE)

        fluxes = result_dict['fluxes']
        fluxes *= 1000.
        xlocs = result_dict['xlocs']
        ylocs = result_dict['ylocs']
        zlocs = result_dict['zlocs']
        positions = result_dict['positions']

        self.assertTrue(result_dict is not None)
        self.assertTrue(q >= 0.0 for q in fluxes)


if __name__ == "__main__":
    unittest.main()
