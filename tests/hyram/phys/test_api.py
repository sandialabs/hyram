"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest
from math import isnan

import numpy as np

from hyram.phys import api, Fluid
from hyram.utilities import misc_utils

VERBOSE = False


class EtkTpdTestCase(unittest.TestCase):
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
        self.assertRaises(ValueError,
                          api.compute_thermo_param,
                          species=None,
                          phase=None,
                          temp=None,
                          pres=None,
                          density=None)


class TestETKTankMass(unittest.TestCase):
    """
    Test engineering toolkit tank mass API interface
    """
    def setUp(self):
        self.species = 'h2'
        self.phase = None
        self.phase_sat_vap = 'gas'  # saturated vapor
        self.phase_sat_liq = 'liquid'  # saturated vapor
        self.temperature = 298  # K
        self.pressure = 350e5  # Pa
        self.pressure_sat_vap = 1e6  # Pa, pressure for saturated phase
        self.volume = 1  # m3
        self.mass = 1  # kg
        self.mass_sat_liq = 70.85  # kg

    def test_calc_temp_from_pres_vol_mass(self):
        temp, _ = api.compute_tank_mass_param(species=self.species,
                                              pres=self.pressure,
                                              vol=self.volume,
                                              mass=self.mass)
        self.assertGreater(temp, 0)

    def test_calc_pres_from_temp_vol_mass(self):
        pres, _ = api.compute_tank_mass_param(species=self.species,
                                              temp=self.temperature,
                                              vol=self.volume,
                                              mass=self.mass)
        self.assertGreater(pres, 0)

    def test_calc_pres_temp_from_phase_vol_mass(self):
        pres, temp = api.compute_tank_mass_param(species=self.species,
                                                 phase=self.phase_sat_liq,
                                                 vol=self.volume,
                                                 mass=self.mass_sat_liq)
        self.assertGreater(pres, 0)
        self.assertGreater(temp, 0)

    def test_calc_vol_from_temp_pres_mass(self):
        vol, _ = api.compute_tank_mass_param(species=self.species,
                                             temp=self.temperature,
                                             pres=self.pressure,
                                             mass=self.mass)
        self.assertGreater(vol, 0)

    def test_calc_vol_temp_from_phase_pres_mass(self):
        vol, temp = api.compute_tank_mass_param(species=self.species,
                                                phase=self.phase_sat_liq,
                                                pres=self.pressure_sat_vap,
                                                mass=self.mass_sat_liq)
        self.assertGreater(vol, 0)
        self.assertGreater(temp, 0)

    def test_calc_mass_from_temp_pres_vol(self):
        mass, _ = api.compute_tank_mass_param(species=self.species,
                                              temp=self.temperature,
                                              pres=self.pressure,
                                              vol=self.volume)
        self.assertGreater(mass, 0)

    def test_calc_mass_temp_from_phase_pres_vol(self):
        mass, temp = api.compute_tank_mass_param(species=self.species,
                                                 phase=self.phase_sat_liq,
                                                 pres=self.pressure_sat_vap,
                                                 vol=self.volume)
        self.assertGreater(mass, 0)
        self.assertGreater(temp, 0)

    def test_reject_too_many_params(self):
        self.assertRaises(ValueError,
                          api.compute_tank_mass_param,
                          species=self.species,
                          phase=self.phase,
                          temp=self.temperature,
                          pres=self.pressure,
                          vol=self.volume,
                          mass=self.mass)

    def test_reject_too_few_params(self):
        self.assertRaises(ValueError,
                          api.compute_tank_mass_param,
                          species=None,
                          phase=None,
                          temp=None,
                          pres=None,
                          vol=None,
                          mass=None)


class TestETKTNTEquivalentMass(unittest.TestCase):
    """
    Test engineering toolkit TNT equivalence mass API interface
    """
    def test_calc_equiv_TNT_mass(self):
        flammable_mass = 1  # kg
        percent_yield = 3
        species = 'H2'
        equiv_TNT_mass = api.compute_equivalent_tnt_mass(vapor_mass=flammable_mass,
                                                         percent_yield=percent_yield,
                                                         species=species)
        # Hand calculation based on 1 kg H2 with TNT blast energy 4.68 MJ/kg and yield of 3%
        self.assertAlmostEqual(equiv_TNT_mass, 0.769, places=3)


class PlumeDispersionTestCase(unittest.TestCase):
    """
    Test plume dispersion physics API interface
    """
    def setUp(self):
        # Default inputs
        self.amb_fluid = Fluid(species='air', T=298, P=101325)
        self.rel_fluid = Fluid(species='hydrogen', T=298, P=35e6)
        self.mass_flow = None
        self.orif_diam = 3.56 / 1000  # m
        self.rel_angle = 0  # radians, 0=horizontal
        self.dis_coeff = 1
        self.nozzle_model = 'yuce'
        self.create_plot = True
        self.contour = 0.04  # mole fraction
        self.contour_min = 0  # mole fraction
        self.contour_max = 0.1  # mole fraction
        self.xmin = -2.5  # m
        self.xmax = 2.5  # m
        self.ymin = 0  # m
        self.ymax = 10  # m
        self.plot_title = "Mole Fraction of Leak"
        self.filename = None  # use default
        self.output_dir = None  # use default
        self.verbose = False

    def test_valid_input(self):
        result = api.analyze_jet_plume(self.amb_fluid, self.rel_fluid, self.orif_diam)
        self.assertTrue(result is not None)

    def test_blend(self):
        self.rel_fluid = Fluid({'h2': 0.05, 'ch4': 0.95}, T=298, P=1e6)
        result = api.analyze_jet_plume(self.amb_fluid, self.rel_fluid, self.orif_diam)
        self.assertTrue(result is not None)

    def test_reject_mole_frac_0(self):
        contour = 0  # mole fraction
        self.assertRaises(ValueError, api.analyze_jet_plume,
                          self.amb_fluid, self.rel_fluid, self.orif_diam, self.mass_flow,
                          self.rel_angle, self.dis_coeff, self.nozzle_model,
                          self.create_plot, contour,
                          self.contour_min, self.contour_max,
                          self.xmin, self.xmax, self.ymin, self.ymax,
                          self.plot_title, self.filename, self.output_dir,
                          self.verbose)

    def test_reject_mole_frac_1(self):
        contour = 1  # mole fraction
        self.assertRaises(ValueError, api.analyze_jet_plume,
                          self.amb_fluid, self.rel_fluid, self.orif_diam, self.mass_flow,
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
                          self.amb_fluid, self.rel_fluid, self.orif_diam, self.mass_flow,
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
                          self.amb_fluid, self.rel_fluid, self.orif_diam, self.mass_flow,
                          self.rel_angle, self.dis_coeff, self.nozzle_model,
                          self.create_plot, self.contour,
                          self.contour_min, self.contour_max,
                          self.xmin, self.xmax, ymin, ymax,
                          self.plot_title, self.filename, self.output_dir,
                          self.verbose)

    def test_default_inputs(self):
        results = api.analyze_jet_plume(self.amb_fluid,
                                        self.rel_fluid,
                                        self.orif_diam,
                                        self.mass_flow,
                                        self.rel_angle,
                                        self.dis_coeff,
                                        self.nozzle_model,
                                        self.create_plot,
                                        self.contour,
                                        self.contour_min,
                                        self.contour_max,
                                        self.xmin,
                                        self.xmax,
                                        self.ymin,
                                        self.ymax,
                                        self.plot_title,
                                        self.filename,
                                        self.output_dir,
                                        self.verbose)
        self.assertTrue(os.path.isfile(results['plot']))
        self.assertGreater(results['mass_flow_rate'], 0)
        self.assertGreater(results['streamline_dists'], 0)
        self.assertGreaterEqual(results['mole_frac_dists'][self.contour][0][1], 0)


class AccumulationTestCase(unittest.TestCase):
    """
    Test phys API accumulation (indoor release).
    """
    def setUp(self):
        # Default inputs
        output_dir = misc_utils.get_temp_folder()
        amb_fluid = Fluid(species='air', T=288, P=101325)
        rel_fluid = Fluid(species='hydrogen', T=287.8, P=35e6)

        self.times = np.array([1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
                               20, 21, 22, 23, 24, 25,
                               26, 27, 28, 29, 29.5])

        self.params = {
            'amb_fluid': amb_fluid,
            'rel_fluid': rel_fluid,
            'ceil_vent_xarea': 0.09079,
            'ceil_vent_height': 2.42,
            'dist_rel_to_wall': 2.1255,
            'enclos_height': 2.72,
            'floor_ceil_area': 16.72216,
            'floor_vent_height': 0.044,
            'floor_vent_xarea': 0.00762,
            'is_steady': False,
            'nozzle_key': 'yuce',
            'orif_diam': 3.56 / 1000,
            'orif_dis_coeff': 1,

            'rel_angle': 0,
            'rel_height': 0,
            'tank_volume': 0.00363,
            'vol_flow_rate': 0,
            'times': self.times,
            'tmax': 30,
            'create_plots': True,

            'output_dir': output_dir,
            'verbose': VERBOSE
        }

    def test_defaults(self):
        num_times = len(self.times)
        rdict = api.analyze_accumulation(**self.params)

        self.assertTrue(rdict['status'])

        self.assertTrue('pressures_per_time' in rdict)
        pressures = rdict['pressures_per_time']
        self.assertEqual(len(pressures), num_times)

        self.assertTrue('depths' in rdict)
        depths = rdict['depths']
        self.assertEqual(len(depths), num_times)

        self.assertTrue('concentrations' in rdict)
        concs = rdict['concentrations']
        self.assertEqual(len(concs), num_times)

        self.assertTrue('overpressure' in rdict)
        overp = rdict['overpressure']
        self.assertEqual(type(overp), np.float64)

        self.assertTrue('time_of_overp' in rdict)
        t_overp = rdict['time_of_overp']
        self.assertEqual(type(t_overp), np.float64)

        self.assertTrue('pres_plot_filepath' in rdict)
        plot = rdict['pres_plot_filepath']
        self.assertEqual(type(plot), str)
        self.assertTrue(os.path.isfile(plot))

        self.assertTrue('mass_plot_filepath' in rdict)
        plot = rdict['mass_plot_filepath']
        self.assertEqual(type(plot), str)
        self.assertTrue(os.path.isfile(plot))

        self.assertTrue('layer_plot_filepath' in rdict)
        plot = rdict['layer_plot_filepath']
        self.assertEqual(type(plot), str)
        self.assertTrue(os.path.isfile(plot))

        self.assertTrue('trajectory_plot_filepath' in rdict)
        plot = rdict['trajectory_plot_filepath']
        self.assertEqual(type(plot), str)
        self.assertTrue(os.path.isfile(plot))

        self.assertTrue('mass_flow_plot_filepath' in rdict)
        plot = rdict['mass_plot_filepath']
        self.assertEqual(type(plot), str)
        self.assertTrue(os.path.isfile(plot))


class JetFlameAnalysisTestCase(unittest.TestCase):
    """
    Test jet flame analysis physics API interface
    """
    def test_default_jet_flame_analysis(self):
        amb_fluid = api.create_fluid(species='air', temp=298, pres=101325)
        rel_fluid = api.create_fluid(species='h2', temp=298, pres=35e6)
        orif_diam = 3.56 / 1000  # m
        dis_coeff = 1
        rel_angle = 0
        mass_flow = None
        nozzle_key = 'yuce'
        rel_humid = 0.89
        create_temp_plot = True
        temp_plot_filename = None
        txmin = 0
        txmax = 10
        tymin = -3.5
        tymax = 3.5

        analyze_flux = True
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
        fxmin = -5
        fxmax = 15
        fymin = -10
        fymax = 10
        fzmin = -10
        fzmax = 10

        output_dir = misc_utils.get_temp_folder()
        verbose = False

        (temp_plot_filepath, heatflux_filepath,
         pos_flux, mass_flow, srad, visible_length, radiant_frac
         ) = api.jet_flame_analysis(amb_fluid, rel_fluid, orif_diam, mass_flow=None, dis_coeff=dis_coeff,
                                    rel_angle=rel_angle, nozzle_key=nozzle_key, rel_humid=rel_humid,
                                    create_temp_plot=create_temp_plot, temp_plot_filename=temp_plot_filename,
                                    temp_contours=None,
                                    temp_xlims=(txmin, txmax), temp_ylims=(tymin, tymax),

                                    analyze_flux=analyze_flux, flux_plot_filename=heatflux_plot_filename,
                                    flux_coordinates=flux_coordinates, flux_contours=None,
                                    flux_xlims=(fxmin, fxmax), flux_ylims=(fymin, fymax), flux_zlims=(fzmin, fzmax),

                                    output_dir=output_dir, verbose=verbose)
        self.assertTrue(os.path.exists(temp_plot_filepath))
        self.assertTrue(os.path.exists(heatflux_filepath))
        for flux_value in pos_flux:
            self.assertGreater(flux_value, 0)
        self.assertGreater(mass_flow, 0)
        self.assertGreater(srad, 0)
        self.assertGreater(visible_length, 0)
        self.assertGreater(radiant_frac, 0)


class OverpressureTestCase(unittest.TestCase):
    """
    Test overpressure calculation
    """
    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()

        # Default values
        self.locations = [(1, 1, 0)]  # distance is sqrt(2) m
        P_inf = 101325.  # Pa, ambient pressure
        jet_T = 300  # K, jet exit temperature
        jet_P = 60e5  # Pa, jet exit pressure
        self.ambient_fluid = Fluid(species='air', P = P_inf, T = jet_T)
        self.release_fluid = Fluid(species = 'hydrogen', T = jet_T, P = jet_P)
        self.orifice_diameter = 2*0.0254  # m, jet exit diameter

    def test_basic_bst_overpressure(self):
        method = 'BST'
        BST_mach_flame_speed = 0.35
        result_dict = api.compute_overpressure(method, self.locations,
                                               ambient_fluid=self.ambient_fluid,
                                               release_fluid=self.release_fluid,
                                               orifice_diameter=self.orifice_diameter,
                                               bst_flame_speed=BST_mach_flame_speed,
                                               output_dir=self.output_dir)
        overpressure = result_dict["overpressures"]
        impulse = result_dict["impulses"]
        overpressure_figure_path = result_dict["overp_plot_filepath"]
        impulse_figure_path = result_dict["impulse_plot_filepath"]
        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0)
        self.assertGreaterEqual(impulse, 0)
        self.assertTrue(os.path.exists(overpressure_figure_path))
        self.assertTrue(os.path.exists(impulse_figure_path))

    def test_basic_tnt_overpressure(self):
        method = 'TNT'
        tnt_factor = 0.03
        result_dict = api.compute_overpressure(method, self.locations,
                                               ambient_fluid=self.ambient_fluid,
                                               release_fluid=self.release_fluid,
                                               orifice_diameter=self.orifice_diameter,
                                               tnt_factor=tnt_factor,
                                               output_dir=self.output_dir)
        overpressure = result_dict["overpressures"]
        impulse = result_dict["impulses"]
        overpressure_figure_path = result_dict["overp_plot_filepath"]
        impulse_figure_path = result_dict["impulse_plot_filepath"]
        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0)
        self.assertGreaterEqual(impulse, 0)
        self.assertTrue(os.path.exists(overpressure_figure_path))
        self.assertTrue(os.path.exists(impulse_figure_path))

    def test_basic_bauwens_overpressure(self):
        method = 'Bauwens'
        result_dict = api.compute_overpressure(method, self.locations,
                                               ambient_fluid=self.ambient_fluid,
                                               release_fluid=self.release_fluid,
                                               orifice_diameter=self.orifice_diameter,
                                               output_dir=self.output_dir)
        overpressure = result_dict["overpressures"]
        impulse = result_dict["impulses"]
        overpressure_figure_path = result_dict["overp_plot_filepath"]
        impulse_figure_path = result_dict["impulse_plot_filepath"]
        self.assertTrue(result_dict is not None)
        self.assertGreaterEqual(overpressure, 0)
        self.assertTrue(isnan(impulse[0]))
        self.assertTrue(os.path.exists(overpressure_figure_path))
        self.assertIsNone(impulse_figure_path)
