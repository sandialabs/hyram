"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import unittest

import numpy as np

from hyram.phys import TNT_method, Orifice, Jet, Fluid, BST_method, Bauwens_method
import hyram.phys._unconfined_overpressure as hyram_overp
from hyram.utilities import misc_utils

VERBOSE = False


class OverpressureBlendInputTestCase(unittest.TestCase):
    """ Test unconfined overpressure with blend-like fuel inputs. """
    def setUp(self):
        self.t_amb = 300  # K
        orifice_diameter = 0.0254  # m
        dischage_coefficent = 1
        ambient_pressure = 101325  # Pa

        self.orifice = Orifice(orifice_diameter, dischage_coefficent)
        self.fluid_amb = Fluid(species='air', P=ambient_pressure, T=self.t_amb)

    def test_no_blend_origin_at_leak_orifice(self):
        p_rel = 60e5  # Pa
        release_fluid = Fluid(species='hydrogen', T=self.t_amb, P=p_rel)
        nozzle_cons_m, nozzle_t_param = misc_utils.convert_nozzle_model_to_params('yuce', release_fluid)
        jet_object = Jet(release_fluid, self.orifice, self.fluid_amb,
                         nn_conserve_momentum=nozzle_cons_m,
                         nn_T=nozzle_t_param,
                         verbose=VERBOSE)
        generic_calc = hyram_overp.Generic_overpressure_method(jet_object, origin_at_orifice=True)
        for coord in generic_calc.origin:
            self.assertAlmostEqual(coord, 0)

    def test_h2(self):
        p_rel = 60e5  # Pa
        release_fluid = Fluid(species={'h2': 1}, T=self.t_amb, P=p_rel)
        nozzle_cons_m, nozzle_t_param = misc_utils.convert_nozzle_model_to_params('yuce', release_fluid)
        jet_object = Jet(release_fluid, self.orifice, self.fluid_amb,
                         nn_conserve_momentum=nozzle_cons_m,
                         nn_T=nozzle_t_param,
                         verbose=VERBOSE)
        generic_calc = hyram_overp.Generic_overpressure_method(jet_object, origin_at_orifice=True)
        for coord in generic_calc.origin:
            self.assertAlmostEqual(coord, 0)

    def test_bauwens_overp(self):
        p_rel = 60e5  # Pa
        release_fluid = Fluid(species={'h2': 1}, T=self.t_amb, P=p_rel)
        nozzle_cons_m, nozzle_t_param = misc_utils.convert_nozzle_model_to_params('yuce', release_fluid)
        jet_object = Jet(release_fluid, self.orifice, self.fluid_amb,
                         nn_conserve_momentum=nozzle_cons_m,
                         nn_T=nozzle_t_param,
                         verbose=VERBOSE)
        bauwens_calc = Bauwens_method(jet_object)
        locations = [(5, 0, 0)]
        overpressure = bauwens_calc.calc_overpressure(locations)[0]
        self.assertGreater(overpressure, 0)

    def test_bst_overp(self):
        p_rel = 60e5  # Pa
        release_fluid = Fluid(species={'h2': 1}, T=self.t_amb, P=p_rel)
        nozzle_cons_m, nozzle_t_param = misc_utils.convert_nozzle_model_to_params('yuce', release_fluid)
        jet_object = Jet(release_fluid, self.orifice, self.fluid_amb,
                         nn_conserve_momentum=nozzle_cons_m,
                         nn_T=nozzle_t_param,
                         verbose=VERBOSE)

        mach_flame_speed = 5.2
        origin_at_orifice = True
        bst = BST_method(jet_object=jet_object, mach_flame_speed=mach_flame_speed, origin_at_orifice=origin_at_orifice)
        bst.flammable_mass = 1
        bst.energy = bst.calc_energy()
        self.assertEqual(bst.energy, 2*1*1.20E+08)

    def test_tnt_overp(self):
        p_rel = 60e5  # Pa
        release_fluid = Fluid(species={'h2': 1}, T=self.t_amb, P=p_rel)
        nozzle_cons_m, nozzle_t_param = misc_utils.convert_nozzle_model_to_params('yuce', release_fluid)
        jet_object = Jet(release_fluid, self.orifice, self.fluid_amb,
                         nn_conserve_momentum=nozzle_cons_m,
                         nn_T=nozzle_t_param,
                         verbose=VERBOSE)
        tnt_factor = 0.03
        origin_at_orifice = True
        tnt = TNT_method(jet_object=jet_object, equivalence_factor=tnt_factor, origin_at_orifice=origin_at_orifice)

        tnt.flammable_mass = 1
        equiv_TNT_mass = tnt.calc_TNT_equiv_mass(tnt_factor, tnt.flammable_mass, tnt.heat_of_combustion)
        self.assertEqual(equiv_TNT_mass, 0.03*1*1.20E+08/4.68e6)


class GenericMethodTestCase(unittest.TestCase):
    """
    Tests of generic method functions for unconfined overpressure
    """
    def setUp(self):
        ambient_temperature = 300  # K
        jet_pressure = 60e5  # Pa
        orifice_diameter = 0.0254  # m
        dischage_coefficent = 1
        ambient_pressure = 101325  # Pa
        nozzle_model = 'yuce'
        release_fluid = Fluid(species='hydrogen',
                              T=ambient_temperature,
                              P=jet_pressure)
        orifice = Orifice(orifice_diameter, dischage_coefficent)
        ambient_fluid = Fluid(species='air',
                              P=ambient_pressure,
                              T=ambient_temperature)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
        self.jet_object = Jet(release_fluid, orifice, ambient_fluid,
                              nn_conserve_momentum=nozzle_cons_momentum,
                              nn_T=nozzle_t_param,
                              verbose=VERBOSE)

    def test_origin_at_leak_orifice(self):
        generic_calc = hyram_overp.Generic_overpressure_method(self.jet_object,
                                                               origin_at_orifice=True)
        for coord in generic_calc.origin:
            self.assertAlmostEqual(coord, 0)

    def test_origin_not_at_leak_orifice(self):
        generic_calc = hyram_overp.Generic_overpressure_method(self.jet_object)
        self.assertGreater(generic_calc.origin[0], 0)
        self.assertGreater(generic_calc.origin[1], 0)
        self.assertEqual(generic_calc.origin[2], 0)

    def test_distance_no_locations(self):
        locations = []
        origin = (0, 0, 0)
        distances = hyram_overp.Generic_overpressure_method.calc_distance(locations,
                                                                          origin)
        self.assertEqual(len(distances), 0)

    def test_flammability_limits(self):
        lower_flammability_limit = 0.1
        upper_flammability_limit = 0.75
        flammability_limits = (lower_flammability_limit, upper_flammability_limit)
        generic_smaller_flam = hyram_overp.Generic_overpressure_method(self.jet_object,
                                                                       flammability_limits=flammability_limits)
        generic_larger_flam = hyram_overp.Generic_overpressure_method(self.jet_object)
        self.assertGreater(generic_larger_flam.flammable_mass, generic_smaller_flam.flammable_mass)


class BstMethodTestCase(unittest.TestCase):
    """
    Tests of different aspects of unconfined overpressure using BST method
    """
    def setUp(self):
        ambient_pressure = 101325  # Pa, ambient pressure
        ambient_temperature = 300  # K, jet exit temperature
        jet_pressure = 60e5  # Pa, jet exit pressure
        jet_diameter = 0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(species='air', P=ambient_pressure, T=ambient_temperature)
        release_fluid = Fluid(species='hydrogen', T=ambient_temperature, P=jet_pressure)
        orifice_diameter = jet_diameter
        mach_flame_speed = 5.2
        origin_at_orifice = True
        nozzle_model = 'yuce'
        dischage_coefficent = 1

        orifice = Orifice(orifice_diameter, dischage_coefficent)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param,
                         verbose=VERBOSE)

        self.BST_calc = BST_method(jet_object=jet_object, mach_flame_speed=mach_flame_speed, origin_at_orifice=origin_at_orifice)
        self.BST_calc.flammable_mass = 1
        self.BST_calc.energy = self.BST_calc.calc_energy()

    def test_calc_energy(self):
        self.assertEqual(self.BST_calc.energy, 2*1*1.20E+08)

    def test_calc_scaled_distance(self):
        distance = 1  #m
        scaled_distance = self.BST_calc.calc_scaled_distance(distance=distance)  # m/(J/Pa))^(1/3)
        self.assertEqual(scaled_distance, 1/(2*1*1.20E+08/101325)**(1/3))

    def test_get_scaled_overpressure(self):
        scaled_distance = 1  # m/(J/Pa)^(1/3)
        scaled_overpressure = self.BST_calc.get_scaled_overpressure(scaled_distance=scaled_distance)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_overpressure, 0.4, places=1)

    def test_calc_real_overpressure(self):
        scaled_overpressure = 10
        unscaled_overpressure = self.BST_calc.calc_unscaled_overpressure(scaled_overpressure=scaled_overpressure)
        self.assertEqual(unscaled_overpressure, scaled_overpressure*101325)

    def test_calc_overpressure(self):
        locations = [np.array([5, 0, 0])]  # m
        overpressure = self.BST_calc.calc_overpressure(locations=locations)[0]  # Pa
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(overpressure/202650, 202650/202650, places=0)

    def test_calc_overpressure_array(self):
        locations = [np.array([5, 0, 0]), np.array([0, 5, 0])]  # m
        overpressure = self.BST_calc.calc_overpressure(locations=locations)  # Pa
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        calculated_values = overpressure/202650
        test_values = np.array([202650/202650, 202650/202650])
        for calc_val, test_val in zip(calculated_values, test_values):
            self.assertAlmostEqual(calc_val, test_val, places=0)

    def test_get_scaled_impulse(self):
        scaled_distance = 1  # m/kg^(1/3)
        scaled_impulse = self.BST_calc.get_scaled_impulse(scaled_distance=scaled_distance)  # Pa*s*(m/s)/J^(1/3)/Pa^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_impulse, 0.03, places=0)

    def test_calc_real_impulse(self):
        scaled_impulse = 0.1
        unscaled_impulse = self.BST_calc.calc_unscaled_impulse(scaled_impulse=scaled_impulse)
        self.assertEqual(unscaled_impulse, scaled_impulse*(2*1*1.20E+08)**(1/3)*101325**(2/3) / 340)

    def test_calc_impulse(self):
        locations = [np.array([5, 0, 0])]  # m
        impulse = self.BST_calc.calc_impulse(locations=locations)[0]
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(impulse/283, 283/283, places=0)

    def test_changing_mach_flame_speed(self):
        mach_flame_speed = 0.7
        self.BST_calc.set_mach_flame_speed(mach_flame_speed=mach_flame_speed)
        self.assertEqual(self.BST_calc.mach_flame_speed, mach_flame_speed)
        mach_flame_speed = 1.4
        self.BST_calc.set_mach_flame_speed(mach_flame_speed=mach_flame_speed)
        self.assertEqual(self.BST_calc.mach_flame_speed, mach_flame_speed)

    def test_value_left_of_figure_data_returns_initial_value(self):
        for Mf in [0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2]:
            self.BST_calc.set_mach_flame_speed(mach_flame_speed=Mf)
            self.assertEqual(self.BST_calc.get_scaled_overpressure(scaled_distance=0.05),
                             self.BST_calc.scaled_peak_overpressure_data['scaled_overpressure_Mf' + str(Mf)][0])
            self.assertEqual(self.BST_calc.get_scaled_impulse(scaled_distance=0.005),
                             self.BST_calc.all_scaled_impulse_data['scaled_impulse_Mf' + str(Mf)][0])

    def test_value_right_of_figure_data_returns_final_value(self):
        for Mf in [0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2]:
            self.BST_calc.set_mach_flame_speed(mach_flame_speed=Mf)
            self.assertEqual(self.BST_calc.get_scaled_overpressure(scaled_distance=15),
                             self.BST_calc.scaled_peak_overpressure_data['scaled_overpressure_Mf' + str(Mf)][-1])
            self.assertEqual(self.BST_calc.get_scaled_impulse(scaled_distance=15),
                             self.BST_calc.all_scaled_impulse_data['scaled_impulse_Mf' + str(Mf)][-1])

    def test_calc_scaled_overpressure(self):
        overpressure = 13  # Pa
        scaled_overpressure = self.BST_calc.calc_scaled_overpressure(overpressure=overpressure)  # unitless
        self.assertEqual(scaled_overpressure, overpressure/101325)

    def test_get_scaled_distance_from_scaled_overpressure(self):
        scaled_overpressure = 10
        scaled_distance = self.BST_calc.get_scaled_distance_from_scaled_overpressure(scaled_overpressure=scaled_overpressure)  # m/(J/Pa)^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 0.2, places=1)

    def test_calc_unscaled_distance(self):
        scaled_distance = 1  # m/(J/Pa)^(1/3)
        unscaled_distance = self.BST_calc.calc_unscaled_distance(scaled_distance=scaled_distance)  # m
        self.assertEqual(unscaled_distance, 1*(2*1*1.20E+08/101325)**(1/3))

    def test_calc_distance_to_overpressure(self):
        overpressure = 202650  # Pa
        distance = self.BST_calc.calc_distance_to_overpressure(overpressure=overpressure)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 5, places=0)

    def test_calc_scaled_impulse(self):
        impulse = 13  # Pa*s
        scaled_impulse = self.BST_calc.calc_scaled_impulse(impulse=impulse)
        self.assertEqual(scaled_impulse, impulse*340/(2*1*1.20E+08)**(1/3)/101325**(2/3))

    def test_get_scaled_distance_from_scaled_impulse(self):
        scaled_impulse = 0.1
        scaled_distance = self.BST_calc.get_scaled_distance_from_scaled_impulse(scaled_impulse=scaled_impulse)  # m/(J/Pa)^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 0.27, places=0)

    def test_calc_distance_to_impulse(self):
        impulse = 284  # Pa*s
        distance = self.BST_calc.calc_distance_to_impulse(impulse=impulse)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 6, places=0)


class BauwensMethodTestCase(unittest.TestCase):
    """
    Tests of different aspects of unconfined overpressure using Bauwens method
    """
    def setUp(self):
        ambient_pressure = 101325  # Pa, ambient pressure
        ambient_temperature = 300  # K, jet exit temperature
        jet_pressure = 60e5  # Pa, jet exit pressure
        orifice_diameter = 0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(species='air', P=ambient_pressure, T=ambient_temperature)
        release_fluid = Fluid(species='hydrogen', T=ambient_temperature, P=jet_pressure)
        nozzle_model='yuce'
        dischage_coefficent = 1

        orifice = Orifice(orifice_diameter, dischage_coefficent)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param,
                         verbose=VERBOSE)

        self.Bauwens_calc = Bauwens_method(jet_object)

    def test_calc_overpressure(self):
        # test to check that result is non-zero
        locations = [(5, 0, 0)]
        overpressure = self.Bauwens_calc.calc_overpressure(locations)[0]
        self.assertGreater(overpressure, 0)

    def test_calc_impulse(self):
        # ensure that method returns a nan value due to not computing impulse
        locations = [(5, 0, 0)]
        impulse = self.Bauwens_calc.calc_impulse(locations)[0]
        self.assertTrue(np.isnan(impulse))

    def test_calc_energy(self):
        # test to check that result is non-zero
        self.assertGreater(self.Bauwens_calc.energy, 0)

    def test_calc_scaled_overpressure(self):
        scaled_distance = 3  # not 0, not 1
        scaled_overpressure = Bauwens_method.calc_scaled_overpressure_from_dist(scaled_distance)
        hand_calc_value = 0.0855
        self.assertAlmostEqual(scaled_overpressure, hand_calc_value, places=3)

    def test_calc_scaled_overpressure_zero_distance_error(self):
        self.assertRaises(ZeroDivisionError, Bauwens_method.calc_scaled_overpressure_from_dist, 0)

    def test_get_scaled_overpressure(self):
        scaled_distances = np.array([0, 1, 2])  # including zero and non-zero distances
        scaled_overpressures = self.Bauwens_calc.get_scaled_overpressure(scaled_distances)
        for scaled_overpressure in scaled_overpressures:
            self.assertGreater(scaled_overpressure, 0)


class TntMethodTestCase(unittest.TestCase):
    """
    Tests of different aspects of unconfined overpressure using TNT equivalence method
    """
    def setUp(self):
        self.ambient_pressure = 101325  # Pa, ambient pressure
        ambient_temperature = 300  # K, jet exit temperature
        jet_pressure = 60e5  # Pa, jet exit pressure
        jet_diameter = 0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(species='air', P=self.ambient_pressure, T=ambient_temperature)
        release_fluid = Fluid(species='hydrogen', T=ambient_temperature, P=jet_pressure)
        orifice_diameter = jet_diameter
        equivalence_factor = 0.03
        origin_at_orifice = True
        nozzle_model='yuce'
        dischage_coefficent = 1

        orifice = Orifice(orifice_diameter, dischage_coefficent)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param,
                         verbose=VERBOSE)

        self.TNT_method_calc = TNT_method(jet_object=jet_object,
                                          equivalence_factor=equivalence_factor,
                                          origin_at_orifice=origin_at_orifice)

        self.TNT_method_calc.flammable_mass = 1
        self.TNT_method_calc.equiv_TNT_mass = self.TNT_method_calc.calc_TNT_equiv_mass(equivalence_factor,
                                                                                       self.TNT_method_calc.flammable_mass,
                                                                                       self.TNT_method_calc.heat_of_combustion)

    def test_calc_TNT_mass(self):
        # Hand calculation based on 1 kg H2 with TNT blast energy 4.68 MJ/kg and yield of 3%
        equiv_TNT_mass = TNT_method.calc_TNT_equiv_mass(equivalence_factor=0.03,
                                                        flammable_mass=1,
                                                        heat_of_combustion=1.20E+08)
        self.assertEqual(equiv_TNT_mass, 0.03*1*1.20E+08/4.68e6)

    def test_calc_scaled_distance(self):
        distance = 1  # m
        scaled_distance = self.TNT_method_calc.calc_scaled_distance(distance=distance)  # m/kg^(1/3)
        self.assertEqual(scaled_distance, 1/((0.03*1*1.20E+08/4.68e6)**(1/3)))

    def test_get_scaled_overpressure(self):
        scaled_distance = 1  # m/kg^(1/3)
        scaled_overpressure = self.TNT_method_calc.get_scaled_overpressure(scaled_distance=scaled_distance)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_overpressure, 1.3e1, places=0)

    def test_calc_real_overpressure(self):
        scaled_overpressure = 13
        overpressure = self.TNT_method_calc.calc_unscaled_overpressure(scaled_overpressure=scaled_overpressure)  # Pa
        self.assertEqual(overpressure, scaled_overpressure*self.ambient_pressure)

    def test_calc_overpressure(self):
        locations = [np.array([1, 0, 0])]  # m
        overpressure = self.TNT_method_calc.calc_overpressure(locations=locations)[0]  # Pa
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(overpressure/1013250, 1013250/1013250, places=0)

    def test_get_scaled_impulse(self):
        scaled_distance = 3  # m/kg^(1/3)
        scaled_impulse = self.TNT_method_calc.get_scaled_impulse(scaled_distance=scaled_distance)  # Pa*s/kg^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_impulse, 9e-4, places=0)

    def test_calc_real_impulse(self):
        scaled_impulse = 13  # Pa*s/kg^(1/3)
        unscaled_impulse = self.TNT_method_calc.calc_unscaled_impulse(scaled_impulse=scaled_impulse)  # Pa*s
        self.assertEqual(unscaled_impulse, scaled_impulse*(0.03*1*1.20E+08/4.68e6)**(1/3))

    def test_calc_impulse(self):
        locations = [np.array([1, 0, 0])]  # m
        impulse = self.TNT_method_calc.calc_impulse(locations=locations)[0]
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(impulse/0.0021, 0.0021/0.0021, places=0)

    def test_value_left_of_figure_data_returns_initial_value(self):
        self.assertEqual(self.TNT_method_calc.get_scaled_overpressure(scaled_distance=0.05),
                         self.TNT_method_calc.scaled_peak_overP_data['scaled_overpressure'][0])
        self.assertEqual(self.TNT_method_calc.get_scaled_impulse(scaled_distance=0.05),
                         self.TNT_method_calc.scaled_impulse_data['scaled_impulse'][0])

    def test_value_right_of_figure_data_returns_final_value(self):
        self.assertEqual(self.TNT_method_calc.get_scaled_overpressure(scaled_distance=40),
                         self.TNT_method_calc.scaled_peak_overP_data['scaled_overpressure'][-1])
        self.assertEqual(self.TNT_method_calc.get_scaled_impulse(scaled_distance=40),
                         self.TNT_method_calc.scaled_impulse_data['scaled_impulse'][-1])

    def test_calc_scaled_overpressure(self):
        overpressure = 13
        scaled_overpressure = self.TNT_method_calc.calc_scaled_overpressure(overpressure=overpressure)  # unitless
        self.assertEqual(scaled_overpressure, overpressure/self.ambient_pressure)

    def test_get_scaled_distance_from_scaled_overpressure(self):
        scaled_overpressure = 100
        scaled_distance = self.TNT_method_calc.get_scaled_distance_from_scaled_overpressure(scaled_overpressure=scaled_overpressure)  # m/kg^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 0.3, places=1)

    def test_calc_unscaled_distance(self):
        scaled_distance = 1  # m/kg^(1/3)
        unscaled_distance = self.TNT_method_calc.calc_unscaled_distance(scaled_distance=scaled_distance)  # m
        self.assertEqual(unscaled_distance, 1*(0.03*1*1.20E+08/4.68e6)**(1/3))

    def test_calc_distance_to_overpressure(self):
        overpressure = 1013250  # Pa
        distance = self.TNT_method_calc.calc_distance_to_overpressure(overpressure=overpressure)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 1, places=0)

    def test_calc_scaled_impulse(self):
        impulse = 13  # Pa*s
        scaled_impulse = self.TNT_method_calc.calc_scaled_impulse(impulse=impulse)  # Pa*s/kg^(1/3)
        self.assertEqual(scaled_impulse, impulse/(0.03*1*1.20E+08/4.68e6)**(1/3))

    def test_get_scaled_distance_from_scaled_impulse(self):
        scaled_impulse = 1e-4  # Pa*s/kg^(1/3)
        scaled_distance = self.TNT_method_calc.get_scaled_distance_from_scaled_impulse(scaled_impulse=scaled_impulse)  # m/kg^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 31, places=0)

    def test_calc_distance_to_impulse(self):
        impulse = 0.0021  # Pa*s
        distance = self.TNT_method_calc.calc_distance_to_impulse(impulse=impulse)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 0, places=0)


class TestJallaisOverpressureH2(unittest.TestCase):
    """
    Tests of hydrogen overpressure calculation from Jallais et al.
    """
    def setUp(self):
        ambient_pressure = 101325  # Pa
        ambient_temperature = 300  # K
        fluid_pressure = 60e5  # Pa
        orifice_diameter = 0.0254  # m
        ambient_fluid = Fluid(species='air', P=ambient_pressure, T=ambient_temperature)
        release_fluid = Fluid(species='hydrogen', T=ambient_temperature, P=fluid_pressure)
        nozzle_model='yuce'
        dischage_coefficent = 1
        orifice = Orifice(orifice_diameter, dischage_coefficent)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param,
                         verbose=VERBOSE)
        self.jallais_calc = hyram_overp.JallaisOverpressureH2(jet_object)

    def test_reject_nonhydrogen_inputs(self):
        ambient_fluid = Fluid(species='air', P=101325, T=300)
        release_fluid = Fluid(species='methane', T=300, P=1e6)
        nozzle_model='yuce'
        orifice = Orifice(0.0254)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param,
                         verbose=VERBOSE)
        self.assertRaises(ValueError, hyram_overp.JallaisOverpressureH2, jet_object)

    def test_calc_flame_speed(self):
        mass_flow_rate = 0.7  # kg/s
        flame_speed = hyram_overp.JallaisOverpressureH2.calc_flame_speed(mass_flow_rate)  # m/s
        self.assertEqual(flame_speed, 140)

    def test_calc_mach_flame_speed(self):
        flame_speed = 100  # m/s
        mach_flame_speed = self.jallais_calc.calc_mach_flame_speed(flame_speed)
        self.assertAlmostEqual(mach_flame_speed, 100/340)

    def test_get_mach_flame_speed_curve(self):
        mach_flame_speed = 0.29
        selected_mach_flame_speed = hyram_overp.JallaisOverpressureH2.get_mach_flame_speed_curve(mach_flame_speed)
        self.assertEqual(selected_mach_flame_speed, 0.35)

    def test_calc_overpressure(self):
        locations = [(5, 0, 0)]
        overpressures = self.jallais_calc.calc_overpressure(locations)
        for overpressure in overpressures:
            self.assertGreater(overpressure, 0)
