import logging
import os
import unittest

import numpy as np

from hyram.phys import api
from hyram.utilities import misc_utils, constants

""" NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.

TODO: determine appropriate sig figs, relative error or absolute error guidelines for tests.
"""

log = None


# @unittest.skip
class TestDischargeRateCalculation(unittest.TestCase):
    """
    Test plume analysis.
    """

    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        self.debug = False
        self.verbose = False

        logname = 'hyram'
        misc_utils.setup_file_log(self.output_dir, self.debug, logfile='hyram-test.log', logname=logname)
        self.log = logging.getLogger(logname)

    def tearDown(self):
        pass

    def test_default(self):
        rel_species = "H2"
        rel_temp = 288.15
        rel_pres = 35.e6
        rel_density = None
        rel_phase = None
        dis_coeff = 1.0

        pipe_outer_diam = .375 * constants.IN_TO_M
        pipe_thickness = .065 * constants.IN_TO_M

        # Replicate orifice leak diam calc
        pipe_inner_diam = pipe_outer_diam - 2. * pipe_thickness
        pipe_area = np.pi * (pipe_inner_diam / 2.) ** 2.
        leak_sizes = constants.LEAK_SIZES
        leak_areas = (np.array(leak_sizes) / 100.) * pipe_area
        orif_leak_diams = np.sqrt(4. * (leak_areas / np.pi))

        rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, rel_density, rel_phase)

        discharge_rates = []
        for orif_diam in orif_leak_diams:
            rate = api.compute_discharge_rate(rel_fluid, orif_diam, dis_coeff, verbose=False)
            discharge_rates.append(rate)

        discharge_rates = np.array(discharge_rates)

        hyram_rates = np.array([5.8496676e-05, 5.8496676e-04, 5.8496676e-03, 5.8496676e-02, 5.8496676e-01])
        np.testing.assert_allclose(hyram_rates, discharge_rates, rtol=0.1)


# @unittest.skip
class TestPositionalFlux(unittest.TestCase):
    """
    Test plume analysis.
    """

    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        self.debug = False
        self.verbose = False

        logname = 'hyram'
        misc_utils.setup_file_log(self.output_dir, self.debug, logfile='hyram-test.log', logname=logname)
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

        pipe_outer_diam = .375 * constants.IN_TO_M
        pipe_thickness = .065 * constants.IN_TO_M

        # Replicate orifice leak diam calc
        pipe_inner_diam = pipe_outer_diam - 2. * pipe_thickness
        pipe_area = np.pi * (pipe_inner_diam / 2.) ** 2.
        leak_sizes = constants.LEAK_SIZES
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
                                        output_dir=self.output_dir, verbose=False)

        fluxes = result_dict['fluxes']
        fluxes *= 1000.
        xlocs = result_dict['xlocs']
        ylocs = result_dict['ylocs']
        zlocs = result_dict['zlocs']
        positions = result_dict['positions']

        hyram_default_fluxes = np.array(
                [3.10842878e-02, 2.79365350e-02, 3.39250180e-02, 1.84520533e-01,
                 1.78740909e-02, 2.64547287e-02, 9.97008053e-02, 4.30454862e-02,
                 6.13941444e-02, 5.38988974e-01, 4.88521447e-01, 6.01404837e-01,
                 3.22863804e+00, 3.14224822e-01, 4.63442990e-01, 1.80337032e+00,
                 7.41661619e-01, 1.09981886e+00, 9.51006181e+00, 8.87206958e+00,
                 1.14634529e+01, 5.77947251e+01, 5.82064337e+00, 8.47079759e+00,
                 3.69077410e+01, 1.28037635e+01, 2.17506671e+01, 1.72331926e+02,
                 1.79606338e+02, 2.90044049e+02, 9.42726741e+02, 1.30606978e+02,
                 1.76486798e+02, 1.30911636e+03, 2.13913155e+02, 6.55811644e+02,
                 2.56126288e+03, 3.75313315e+03, 6.73722165e+04, 7.12382896e+03,
                 9.82888665e+03, 4.24734759e+03, 5.48242272e+04, 2.55610105e+03,
                 1.21931462e+05]
        )
        print(fluxes)

        self.assertTrue(result_dict is not None)
        np.testing.assert_allclose(fluxes, hyram_default_fluxes, rtol=0.3)

    # @unittest.skip
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

        pipe_outer_diam = .375 * constants.IN_TO_M
        pipe_thickness = .065 * constants.IN_TO_M

        # Replicate orifice leak diam calc
        pipe_inner_diam = pipe_outer_diam - 2. * pipe_thickness
        pipe_area = np.pi * (pipe_inner_diam / 2.) ** 2.
        leak_sizes = constants.LEAK_SIZES
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
                                        output_dir=self.output_dir, verbose=False)

        fluxes = result_dict['fluxes']
        fluxes *= 1000.
        xlocs = result_dict['xlocs']
        ylocs = result_dict['ylocs']
        zlocs = result_dict['zlocs']
        positions = result_dict['positions']

        hyram_default_fluxes = np.array(
                [3.51612936e-02, 3.16239979e-02, 3.83589357e-02, 2.02434008e-01,
                 2.02777998e-02, 2.99564676e-02, 1.11245915e-01, 4.85669691e-02,
                 6.90305172e-02, 5.21170139e-02, 7.29302636e-02, 7.66503214e-01,
                 1.73505896e-02, 1.03112184e-02, 2.11293327e-01, 2.11293327e-01,
                 2.11293327e-01, 2.11293327e-01, 6.09751379e-01, 5.53132719e-01,
                 6.80268080e-01, 3.54087688e+00, 3.56609218e-01, 5.24926552e-01,
                 2.01208214e+00, 8.36815363e-01, 1.23696110e+00, 9.13329163e-01,
                 1.30616767e+00, 1.46399183e+01, 3.03117070e-01, 1.79694414e-01,
                 3.67140852e+00, 3.67140852e+00, 3.67140852e+00, 3.67140852e+00,
                 1.07618564e+01, 1.00528903e+01, 1.29841545e+01, 6.33232445e+01,
                 6.61384822e+00, 9.60280672e+00, 4.11655064e+01, 1.44464037e+01,
                 2.44869109e+01, 1.66761863e+01, 2.57926926e+01, 3.54093798e+02,
                 5.49435327e+00, 3.23209945e+00, 6.42438845e+01, 6.42438845e+01,
                 6.42438845e+01, 6.42438845e+01, 1.95066081e+02, 2.03934608e+02,
                 3.30571587e+02, 1.03166745e+03, 1.49159436e+02, 2.00613634e+02,
                 1.45142289e+03, 2.41169253e+02, 7.41784547e+02, 3.35959301e+02,
                 7.58974736e+02, 7.80271285e+03, 1.12291350e+02, 6.45929216e+01,
                 9.89763433e+02, 9.89763433e+02, 9.89763433e+02, 9.89763433e+02,
                 2.88526260e+03, 4.24908354e+03, 8.47499626e+04, 7.80999257e+03,
                 1.19867868e+04, 4.82250187e+03, 5.46479442e+04, 2.86719064e+03,
                 1.17697630e+05, 5.49327225e+03, 4.31898755e+04, 2.79048764e+04,
                 3.11730690e+03, 1.85260176e+03, 7.29953330e+03, 7.29953330e+03,
                 7.29953330e+03, 7.29953330e+03]
        )
        print(fluxes)

        self.assertTrue(result_dict is not None)
        np.testing.assert_allclose(fluxes, hyram_default_fluxes, rtol=0.2)


if __name__ == "__main__":
    unittest.main()
