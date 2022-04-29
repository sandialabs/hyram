"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest
import logging
from math import isnan

from hyram.phys import api, Fluid
from hyram.utilities import misc_utils, exceptions

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


class TestETKTemperaturePressureDensity(unittest.TestCase):
    """
    Test engineering toolkit temperature, pressure, density API interface
    """
    def setUp(self):
        self.species = 'h2'
        self.phase = None
        self.phase_sat_vap = 'gas'  # saturated vapor
        self.phase_sat_liq = 'liquid'  # saturated vapor
        self.temperature = 298  # K
        self.pressure = 350e5  # Pa
        self.pressure_sat_vap = 1e6  # Pa, pressure for saturated phase
        self.density = 23  # kg/m3
        self.density_lh2 = 71  # kg/m3

    def test_compute_density_from_temperature_and_pressure(self):
        density, _ = api.compute_thermo_param(species=self.species,
                                              temp=self.temperature,
                                              pres=self.pressure)
        self.assertGreater(density, 0)

    def test_compute_density_from_phase_and_pressure(self):
        density, temp = api.compute_thermo_param(species=self.species,
                                                 phase=self.phase_sat_vap,
                                                 pres=self.pressure_sat_vap)
        self.assertGreater(density, 0)

    def test_compute_pressure_from_temperature_and_density(self):
        pressure, _ = api.compute_thermo_param(species=self.species,
                                               temp=self.temperature,
                                               density=self.density)
        self.assertGreater(pressure, 0)

    def test_compute_pressure_from_phase_and_density(self):
        pressure, temp = api.compute_thermo_param(species=self.species,
                                                  phase=self.phase_sat_vap,
                                                  density=self.density)
        self.assertGreater(pressure, 0)

    def test_compute_pressure_from_liq_phase_and_density(self):
        pressure, temp = api.compute_thermo_param(species=self.species,
                                                  phase=self.phase_sat_liq,
                                                  density=self.density_lh2)
        self.assertGreater(pressure, 0)

    def test_compute_temperature_from_pressure_and_density(self):
        temperature, _ = api.compute_thermo_param(species=self.species,
                                                  pres=self.pressure,
                                                  density=self.density)
        self.assertGreater(temperature, 0)

    def test_compute_temperature_from_pressure_and_phase(self):
        temperature, _ = api.compute_thermo_param(species=self.species,
                                                  phase=self.phase_sat_vap,
                                                  pres=self.pressure_sat_vap)
        self.assertGreater(temperature, 0)

    def test_reject_too_many_params(self):
        self.assertRaises(ValueError,
                          api.compute_thermo_param,
                          species=self.species,
                          phase=self.phase,
                          temp=self.temperature,
                          pres=self.pressure,
                          density=self.density)

    def test_reject_too_few_params(self):
        self.assertRaises(exceptions.FluidSpecificationError,
                          api.compute_thermo_param,
                          species=None,
                          phase=None,
                          temp=None,
                          pres=None,
                          density=None)


class TestPlumeDispersion(unittest.TestCase):
    """
    Test plue dispersion physics API interface
    """
    def setUp(self):
        # Default inputs
        self.amb_fluid = Fluid(T=298, P=101325, species='air')
        self.rel_fluid = Fluid(T=298, P=35e6, species='hydrogen')
        self.orif_diam = 3.56 / 1000  # m
        self.rel_angle = 0.0  # radians, 0=horizontal
        self.dis_coeff = 1.0
        self.nozzle_model = 'yuce'
        self.create_plot = True
        self.contour = 0.04  # mole fraction
        self.contour_min = 0.0  # mole fraction
        self.contour_max = 0.1  # mole fraction
        self.xmin = -2.5  # m
        self.xmax = 2.5  # m
        self.ymin = 0.0  # m
        self.ymax = 10.0  # m
        self.plot_title = "Mole Fraction of Leak"
        self.filename = None  # use default
        self.output_dir = None  # use default
        self.verbose = False

    def test_reject_mole_frac_0(self):
        contour = 0.0  # mole fraction
        self.assertRaises(ValueError, api.analyze_jet_plume,
                          self.amb_fluid, self.rel_fluid, self.orif_diam,
                          self.rel_angle, self.dis_coeff, self.nozzle_model,
                          self.create_plot, contour,
                          self.contour_min, self.contour_max,
                          self.xmin, self.xmax, self.ymin, self.ymax,
                          self.plot_title, self.filename, self.output_dir,
                          self.verbose)

    def test_reject_mole_frac_1(self):
        contour = 1.0  # mole fraction
        self.assertRaises(ValueError, api.analyze_jet_plume,
                          self.amb_fluid, self.rel_fluid, self.orif_diam,
                          self.rel_angle, self.dis_coeff, self.nozzle_model,
                          self.create_plot, contour,
                          self.contour_min, self.contour_max,
                          self.xmin, self.xmax, self.ymin, self.ymax,
                          self.plot_title, self.filename, self.output_dir,
                          self.verbose)

    def test_reject_xmin_greater_than_xmax(self):
        xmin = 2
        xmax = 0
        self.assertRaises(ValueError, api.analyze_jet_plume,
                          self.amb_fluid, self.rel_fluid, self.orif_diam,
                          self.rel_angle, self.dis_coeff, self.nozzle_model,
                          self.create_plot, self.contour,
                          self.contour_min, self.contour_max,
                          xmin, xmax, self.ymin, self.ymax,
                          self.plot_title, self.filename, self.output_dir,
                          self.verbose)

    def test_reject_ymin_greater_than_ymax(self):
        ymin = 2
        ymax = 0
        self.assertRaises(ValueError, api.analyze_jet_plume,
                          self.amb_fluid, self.rel_fluid, self.orif_diam,
                          self.rel_angle, self.dis_coeff, self.nozzle_model,
                          self.create_plot, self.contour,
                          self.contour_min, self.contour_max,
                          self.xmin, self.xmax, ymin, ymax,
                          self.plot_title, self.filename, self.output_dir,
                          self.verbose)


class TestJetFlameAnalysis(unittest.TestCase):
    """
    Test jet flame analysis physics API interface
    """
    def test_default_jet_flame_analysis(self):
        amb_fluid = api.create_fluid(species='air', temp=298, pres=101325)
        rel_fluid = api.create_fluid(species='h2', temp=298, pres=35e6)
        orif_diam = 3.56 / 1000  # m
        dis_coeff = 1.0
        rel_angle = 0.0
        nozzle_key = 'yuce'
        rel_humid = 0.89
        contours = None
        create_temp_plot = True
        analyze_flux = True
        temp_plot_filename = None
        heatflux_plot_filename = None
        flux_coordinates = [(0.01, 1, 0.01),
                            (0.5, 1, 0.5),
                            (1, 1, 0.5),
                            (2, 1, 1),
                            (2.5, 1, 1),
                            (5, 2, 1),
                            (10, 2, 0.5),
                            (15, 2, 0.5),
                            (25, 2, 1),
                            (40, 2, 2)]
        output_dir = misc_utils.get_temp_folder()
        verbose = False
        (temp_plot_filepath, heatflux_filepath,
         pos_flux, mass_flow, srad, visible_length
         ) = api.jet_flame_analysis(amb_fluid, rel_fluid, orif_diam, dis_coeff,
                                    rel_angle, nozzle_key, rel_humid,
                                    contours, create_temp_plot, analyze_flux,
                                    temp_plot_filename,
                                    heatflux_plot_filename,
                                    flux_coordinates,
                                    output_dir, verbose)
        self.assertTrue(os.path.exists(temp_plot_filepath))
        self.assertTrue(os.path.exists(heatflux_filepath))
        for flux_value in pos_flux:
            self.assertGreater(flux_value, 0)
        self.assertGreater(mass_flow, 0)
        self.assertGreater(srad, 0)
        self.assertGreater(visible_length, 0)


class OverpressureTestCase(unittest.TestCase):
    """
    Test overpressure calculation
    """
    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        # Set up logging
        logname = __name__
        misc_utils.setup_file_log(self.output_dir, verbose=VERBOSE, logfile='hyram-test.log', logname=logname)
        self.log = logging.getLogger(logname)
        # Default values
        self.locations = [(1, 1, 0)]  # distance is sqrt(2) m
        P_inf = 101325.  # Pa, ambient pressure
        jet_T = 300.  # K, jet exit temperature
        jet_P = 60e5  # Pa, jet exit pressure
        self.ambient_fluid = Fluid(P = P_inf, T = jet_T, species='air')
        self.release_fluid = Fluid(T = jet_T, P = jet_P, species = 'hydrogen')
        self.orifice_diameter = 2*0.0254  # m, jet exit diameter

    def test_basic_bst_overpressure(self):
        self.log.info("TESTING BST overpressure calculation")
        method = 'BST'
        BST_mach_flame_speed = 0.35
        result_dict = api.compute_overpressure(method, self.locations,
                                               ambient_fluid=self.ambient_fluid,
                                               release_fluid=self.release_fluid,
                                               orifice_diameter=self.orifice_diameter,
                                               BST_mach_flame_speed=BST_mach_flame_speed,
                                               output_dir=self.output_dir)
        overpressure = result_dict["overpressure"]
        impulse = result_dict["impulse"]
        figure_path = result_dict["figure_file_path"]
        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0.0)
        self.assertGreaterEqual(impulse, 0.0)
        self.assertTrue(os.path.exists(figure_path))
    
    def test_basic_tnt_overpressure(self):
        self.log.info("TESTING TNT overpressure calculation")
        method = 'TNT'
        TNT_equivalence_factor = 0.03
        result_dict = api.compute_overpressure(method, self.locations,
                                               ambient_fluid=self.ambient_fluid,
                                               release_fluid=self.release_fluid,
                                               orifice_diameter=self.orifice_diameter,
                                               TNT_equivalence_factor=TNT_equivalence_factor,
                                               output_dir=self.output_dir)
        overpressure = result_dict["overpressure"]
        impulse = result_dict["impulse"]
        figure_path = result_dict["figure_file_path"]
        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0.0)
        self.assertGreaterEqual(impulse, 0.0)
        self.assertTrue(os.path.exists(figure_path))

    def test_basic_bauwens_overpressure(self):
        self.log.info("TESTING Bauwens overpressure calculation")
        method = 'Bauwens'
        result_dict = api.compute_overpressure(method, self.locations,
                                               ambient_fluid=self.ambient_fluid,
                                               release_fluid=self.release_fluid,
                                               orifice_diameter=self.orifice_diameter,
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
