"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest
from numpy import array, isnan
from numpy.testing import assert_almost_equal
from hyram.phys import TNT_method, Orifice, Jet, Fluid, BST_method, Bauwens_method
from hyram.utilities import misc_utils


class BstMethodTestCase(unittest.TestCase):
    """
    Tests of different aspects of unconfined overpressure using BST method
    """
    def setUp(self):
        ambient_pressure = 101325.  # Pa, ambient pressure
        ambient_temperature = 300.  # K, jet exit temperature
        jet_pressure = 60e5  # Pa, jet exit pressure
        jet_diameter = 0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(P=ambient_pressure, T=ambient_temperature, species='air')
        release_fluid = Fluid(T=ambient_temperature, P=jet_pressure, species='hydrogen')
        orifice_diameter = jet_diameter
        mach_flame_speed = 5.2
        origin_at_orifice = True
        nozzle_model='yuce'
        dischage_coefficent = 1.

        orifice = Orifice(orifice_diameter, dischage_coefficent)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param)

        self.BST_calc = BST_method(jet_object=jet_object, mach_flame_speed=mach_flame_speed, origin_at_orifice=origin_at_orifice)
        self.BST_calc.flammable_mass = 1.
        self.BST_calc.energy = self.BST_calc.calc_energy()

    def test_calc_energy(self):
        self.assertEqual(self.BST_calc.energy, 2*1*1.20E+08)

    def test_calc_scaled_distance(self):
        distance = 1.  #m
        scaled_distance = self.BST_calc.calc_scaled_distance(distance=distance)  # m/(J/Pa))^(1/3)
        self.assertEqual(scaled_distance, 1/(2*1*1.20E+08/101325)**(1/3))

    def test_get_scaled_overpressure(self):
        scaled_distance = 1.  # m/(J/Pa)^(1/3)
        scaled_overpressure = self.BST_calc.get_scaled_overpressure(scaled_distance=scaled_distance)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_overpressure, 0.4, places=1)

    def test_calc_real_overpressure(self):
        scaled_overpressure = 10
        unscaled_overpressure = self.BST_calc.calc_unscaled_overpressure(scaled_overpressure=scaled_overpressure)
        self.assertEqual(unscaled_overpressure, scaled_overpressure*101325)

    def test_calc_overpressure(self):
        locations = [array([5.0, 0., 0.])]  # m
        overpressure = self.BST_calc.calc_overpressure(locations=locations)[0]  # Pa
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(overpressure/202650, 202650/202650, places=0)
    
    def test_calc_overpressure_array(self):
        locations = [array([5.0, 0., 0.]), array([0., 5., 0.])]  # m
        overpressure = self.BST_calc.calc_overpressure(locations=locations)  # Pa
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        # Use numpy version of testing for vector
        assert_almost_equal(overpressure/202650, array([202650/202650, 202650/202650]), decimal=0)

    def test_get_scaled_impulse(self):
        scaled_distance = 1.  # m/kg^(1/3)
        scaled_impulse = self.BST_calc.get_scaled_impulse(scaled_distance=scaled_distance)  # Pa*s*(m/s)/J^(1/3)/Pa^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_impulse, 0.03, places=0)

    def test_calc_real_impulse(self):
        scaled_impulse = 0.1
        unscaled_impulse = self.BST_calc.calc_unscaled_impulse(scaled_impulse=scaled_impulse)
        self.assertEqual(unscaled_impulse, scaled_impulse*(2*1*1.20E+08)**(1/3)*101325**(2/3) / 340)

    def test_calc_impulse(self):
        locations = [array([5.0, 0., 0.])]  # m
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
                             self.BST_calc.scaled_peak_overpressure_data['scaled_overpressure_Mf' + str(Mf)].dropna().iloc[0])
            self.assertEqual(self.BST_calc.get_scaled_impulse(scaled_distance=0.005),
                             self.BST_calc.all_scaled_impulse_data['scaled_impulse_Mf' + str(Mf)].dropna().iloc[0])

    def test_value_right_of_figure_data_returns_final_value(self):
        for Mf in [0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2]:
            self.BST_calc.set_mach_flame_speed(mach_flame_speed=Mf)
            self.assertEqual(self.BST_calc.get_scaled_overpressure(scaled_distance=15),
                             self.BST_calc.scaled_peak_overpressure_data['scaled_overpressure_Mf' + str(Mf)].dropna().iloc[-1])
            self.assertEqual(self.BST_calc.get_scaled_impulse(scaled_distance=15),
                             self.BST_calc.all_scaled_impulse_data['scaled_impulse_Mf' + str(Mf)].dropna().iloc[-1])

    def test_calc_scaled_overpressure(self):
        overpressure = 13.  # Pa
        scaled_overpressure = self.BST_calc.calc_scaled_overpressure(overpressure=overpressure)  # unitless
        self.assertEqual(scaled_overpressure, overpressure/101325)

    def test_get_scaled_distance_from_scaled_overpressure(self):
        scaled_overpressure = 10.
        scaled_distance = self.BST_calc.get_scaled_distance_from_scaled_overpressure(scaled_overpressure=scaled_overpressure)  # m/(J/Pa)^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 0.2, places=1)

    def test_calc_unscaled_distance(self):
        scaled_distance = 1.  # m/(J/Pa)^(1/3)
        unscaled_distance = self.BST_calc.calc_unscaled_distance(scaled_distance=scaled_distance)  # m
        self.assertEqual(unscaled_distance, 1*(2*1*1.20E+08/101325)**(1/3))

    def test_calc_distance_from_overpressure(self):
        overpressure = 202650  # Pa
        distance = self.BST_calc.calc_distance_from_overpressure(overpressure=overpressure)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 5, places=0)

    def test_calc_scaled_impulse(self):
        impulse = 13.  # Pa*s
        scaled_impulse = self.BST_calc.calc_scaled_impulse(impulse=impulse)
        self.assertEqual(scaled_impulse, impulse*340/(2*1*1.20E+08)**(1/3)/101325**(2/3))

    def test_get_scaled_distance_from_scaled_impulse(self):
        scaled_impulse = 0.1
        scaled_distance = self.BST_calc.get_scaled_distance_from_scaled_impulse(scaled_impulse=scaled_impulse)  # m/(J/Pa)^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 0.27, places=0)

    def test_calc_distance_from_impulse(self):
        impulse = 284  # Pa*s
        distance = self.BST_calc.calc_distance_from_impulse(impulse=impulse)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 6, places=0)


class BauwensMethodTestCase(unittest.TestCase):
    """
    Tests of different aspects of unconfined overpressure using Bauwens method
    """
    def setUp(self):
        ambient_pressure = 101325.  # Pa, ambient pressure
        ambient_temperature = 300.  # K, jet exit temperature
        jet_pressure = 60e5  # Pa, jet exit pressure
        jet_diameter = 0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(P=ambient_pressure, T=ambient_temperature, species='air')
        release_fluid = Fluid(T=ambient_temperature, P=jet_pressure, species='hydrogen')
        orifice_diameter = jet_diameter
        origin_at_orifice = True
        nozzle_model='yuce'
        dischage_coefficent = 1.

        orifice = Orifice(orifice_diameter, dischage_coefficent)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param)

        self.Bauwens_calc = Bauwens_method(jet_object=jet_object, origin_at_orifice=origin_at_orifice)
        self.Bauwens_calc.detonable_mass= 1.
        self.Bauwens_calc.energy = self.Bauwens_calc.calc_energy()

    def test_calc_overpressure(self):
        # test to monitor if result changes
        locations = [array([5.0, 0., 0.])]
        overpressure = self.Bauwens_calc.calc_overpressure(locations)[0]
        self.assertAlmostEqual(overpressure/124884, 124884/124884, places=0)

    def test_calc_impulse(self):
        # ensure that method returns a nan value due to not computing impulse
        locations = [array([5.0, 0., 0.])]
        impulse = self.Bauwens_calc.calc_impulse(locations)[0]
        self.assertTrue(isnan(impulse))

    def test_calc_energy(self):
        self.assertEqual(self.Bauwens_calc.energy, 1*1.20E+08)

    def test_calc_dimenionless_distance(self):
        distance = 2.
        self.assertEqual(self.Bauwens_calc.calc_dimensionless_distance(distance), 2.*(101325/1.20E+08)**(1./3.))


class TntMethodTestCase(unittest.TestCase):
    """
    Tests of different aspects of unconfined overpressure using TNT equivalence method
    """
    def setUp(self):
        self.ambient_pressure = 101325.  # Pa, ambient pressure
        ambient_temperature = 300.  # K, jet exit temperature
        jet_pressure = 60e5  # Pa, jet exit pressure
        jet_diameter = 0.0254  #m, jet exit diameter
        ambient_fluid = Fluid(P=self.ambient_pressure, T=ambient_temperature, species='air')
        release_fluid = Fluid(T=ambient_temperature, P=jet_pressure, species='hydrogen')
        orifice_diameter = jet_diameter
        equivalence_factor = 0.03
        origin_at_orifice = True
        nozzle_model='yuce'
        dischage_coefficent = 1.

        orifice = Orifice(orifice_diameter, dischage_coefficent)
        nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
        jet_object = Jet(release_fluid, orifice, ambient_fluid,
                         nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param)

        self.TNT_method_calc = TNT_method(jet_object=jet_object, equivalence_factor=equivalence_factor, origin_at_orifice=origin_at_orifice)
        self.TNT_method_calc.flammable_mass = 1.
        self.TNT_method_calc.equiv_TNT_mass = self.TNT_method_calc.calc_TNT_equiv_mass()

    def test_calc_TNT_mass(self):
        # Hand calculation based on 1 kg H2 with TNT blast energy 4.68 MJ/kg and yield of 3%
        self.assertEqual(self.TNT_method_calc.equiv_TNT_mass, 0.03*1*1.20E+08/4.68e6)

    def test_calc_scaled_distance(self):
        distance = 1.0  # m
        scaled_distance = self.TNT_method_calc.calc_scaled_distance(distance=distance)  # m/kg^(1/3)
        self.assertEqual(scaled_distance, 1/((0.03*1*1.20E+08/4.68e6)**(1/3)))

    def test_get_scaled_overpressure(self):
        scaled_distance = 1.  # m/kg^(1/3)
        scaled_overpressure = self.TNT_method_calc.get_scaled_overpressure(scaled_distance=scaled_distance)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_overpressure, 1.3e1, places=0)

    def test_calc_real_overpressure(self):
        scaled_overpressure = 13.
        overpressure = self.TNT_method_calc.calc_unscaled_overpressure(scaled_overpressure=scaled_overpressure)  # Pa
        self.assertEqual(overpressure, scaled_overpressure*self.ambient_pressure)

    def test_calc_overpressure(self):
        locations = [array([1., 0., 0.])]  # m
        overpressure = self.TNT_method_calc.calc_overpressure(locations=locations)[0]  # Pa
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(overpressure/1013250, 1013250/1013250, places=0)

    def test_get_scaled_impulse(self):
        scaled_distance = 3.  # m/kg^(1/3)
        scaled_impulse = self.TNT_method_calc.get_scaled_impulse(scaled_distance=scaled_distance)  # Pa*s/kg^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_impulse, 9e-4, places=0)

    def test_calc_real_impulse(self):
        scaled_impulse = 13.  # Pa*s/kg^(1/3)
        unscaled_impulse = self.TNT_method_calc.calc_unscaled_impulse(scaled_impulse=scaled_impulse)  # Pa*s
        self.assertEqual(unscaled_impulse, scaled_impulse*(0.03*1*1.20E+08/4.68e6)**(1/3))

    def test_calc_impulse(self):
        locations = [array([1., 0., 0.])]  # m
        impulse = self.TNT_method_calc.calc_impulse(locations=locations)[0]
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(impulse/0.0021, 0.0021/0.0021, places=0)

    def test_value_left_of_figure_data_returns_initial_value(self):
        self.assertEqual(self.TNT_method_calc.get_scaled_overpressure(scaled_distance=0.05),
                         self.TNT_method_calc.scaled_peak_overP_data['scaled_overpressure'].iloc[0])
        self.assertEqual(self.TNT_method_calc.get_scaled_impulse(scaled_distance=0.05),
                         self.TNT_method_calc.scaled_impulse_data['scaled_impulse'].iloc[0])

    def test_value_right_of_figure_data_returns_final_value(self):
        self.assertEqual(self.TNT_method_calc.get_scaled_overpressure(scaled_distance=40),
                         self.TNT_method_calc.scaled_peak_overP_data['scaled_overpressure'].iloc[-1])
        self.assertEqual(self.TNT_method_calc.get_scaled_impulse(scaled_distance=40),
                         self.TNT_method_calc.scaled_impulse_data['scaled_impulse'].iloc[-1])

    def test_calc_scaled_overpressure(self):
        overpressure = 13.
        scaled_overpressure = self.TNT_method_calc.calc_scaled_overpressure(overpressure=overpressure)  # unitless
        self.assertEqual(scaled_overpressure, overpressure/self.ambient_pressure)

    def test_get_scaled_distance_from_scaled_overpressure(self):
        scaled_overpressure = 100.
        scaled_distance = self.TNT_method_calc.get_scaled_distance_from_scaled_overpressure(scaled_overpressure=scaled_overpressure)  # m/kg^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 0.3, places=1)

    def test_calc_unscaled_distance(self):
        scaled_distance = 1.  # m/kg^(1/3)
        unscaled_distance = self.TNT_method_calc.calc_unscaled_distance(scaled_distance=scaled_distance)  # m
        self.assertEqual(unscaled_distance, 1*(0.03*1*1.20E+08/4.68e6)**(1/3))

    def test_calc_distance_from_overpressure(self):
        overpressure = 1013250  # Pa
        distance = self.TNT_method_calc.calc_distance_from_overpressure(overpressure=overpressure)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 1, places=0)

    def test_calc_scaled_impulse(self):
        impulse = 13.  # Pa*s
        scaled_impulse = self.TNT_method_calc.calc_scaled_impulse(impulse=impulse)  # Pa*s/kg^(1/3)
        self.assertEqual(scaled_impulse, impulse/(0.03*1*1.20E+08/4.68e6)**(1/3))

    def test_get_scaled_distance_from_scaled_impulse(self):
        scaled_impulse = 1e-4  # Pa*s/kg^(1/3)
        scaled_distance = self.TNT_method_calc.get_scaled_distance_from_scaled_impulse(scaled_impulse=scaled_impulse)  # m/kg^(1/3)
        # Figure-estimation by hand
        self.assertAlmostEqual(scaled_distance, 31, places=0)

    def test_calc_distance_from_impulse(self):
        impulse = 0.0021  # Pa*s
        distance = self.TNT_method_calc.calc_distance_from_impulse(impulse=impulse)  # m
        # Hand calculation and figure estimation by hand
        # VERY rough approximate value
        self.assertAlmostEqual(distance, 0, places=0)


if __name__ == "__main__":
    unittest.main()
