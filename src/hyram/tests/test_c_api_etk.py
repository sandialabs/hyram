import os
import numpy as np
import unittest
from hyram.phys import c_api


"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""


# @unittest.skip
class ETKMassFlowTestCase(unittest.TestCase):
    """
    Test mass flow rate calculation, including steady and non-steady.
    Units are: Kelvin, Pa, m3, m
    """
    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        self.debug = False
        c_api.setup(self.output_dir, self.debug)

    def tearDown(self):
        pass

    # @unittest.skip
    def test_basic_blowdown(self):
        species = "H2"
        temp = 315.
        pres = 1013250.
        phase = 'default'
        tank_vol = 1.  # Note that GUI units default to liters; backend is m^3
        orif_diam = 0.03
        is_steady = False
        dis_coeff = 1.0

        wrapped_results = c_api.etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                                                           is_steady, tank_vol, dis_coeff, self.output_dir, self.debug)
        data = wrapped_results["data"]

        empty_time = data["time_to_empty"]
        plot = data["plot"]
        times = data["times"]
        rates = data["rates"]

        self.assertTrue(plot is not None)
        self.assertGreaterEqual(empty_time, 0.0)
        self.assertTrue(t >= 0.0 for t in times)
        self.assertTrue(m >= 0.0 for m in rates)

    def test_invalid_low_pressure(self):
        species = "H2"
        temp = 288.
        pres = 100000.
        phase = 'default'
        tank_vol = 1.
        orif_diam = 0.003
        is_steady = False
        dis_coeff = 1.0

        wrapped_results = c_api.etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                                                           is_steady, tank_vol, dis_coeff, self.output_dir, self.debug)
        data = wrapped_results["data"]

        self.assertFalse(wrapped_results['status'])
        self.assertEqual(data, None)

    # @unittest.skip
    def test_LH2_blowdown(self):
        species = 'H2'
        temp = None
        pres = 1013250.
        phase = 'liquid'

        tank_vol = 1.  # Note that GUI units default to liters; backend is m^3
        orif_diam = 0.03
        is_steady = False
        dis_coeff = 1.0

        wrapped_results = c_api.etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                                                           is_steady, tank_vol, dis_coeff, self.output_dir, self.debug)
        data = wrapped_results["data"]

        empty_time = data["time_to_empty"]
        plot = data["plot"]
        times = data["times"]
        rates = data["rates"]

        self.assertTrue(plot is not None)
        self.assertGreaterEqual(empty_time, 0.0)
        self.assertTrue(t >= 0.0 for t in times)
        self.assertTrue(m >= 0.0 for m in rates)


# @unittest.skip
class ETKTankMassTestCase(unittest.TestCase):
    """
    Test calculation of tank mass.

    """

    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        self.debug = False
        c_api.setup(self.output_dir, self.debug)

    def tearDown(self):
        pass

    def test_default(self):
        species = "H2"
        temp = 315.
        pres = 101325.
        tank_vol = 10.
        phase = None
        results = c_api.etk_compute_tank_mass(species, temp, pres, phase, tank_vol, self.debug)
        mass = results["data"]
        self.assertGreaterEqual(mass, 0.0)

    def test_LH2(self):
        species = "H2"
        temp = None
        pres = 201325.
        tank_vol = 10.
        phase = 'liquid'
        results = c_api.etk_compute_tank_mass(species, temp, pres, phase, tank_vol, self.debug)
        mass = results["data"]
        self.assertGreaterEqual(mass, 0.0)


# @unittest.skip
class ETKTPDTestCase(unittest.TestCase):
    """
    Test calculations of thermo properties.

    """

    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        self.debug = False
        c_api.setup(self.output_dir, self.debug)

    def tearDown(self):
        pass

    def test_calc_temp_stp(self):
        species = 'H2'
        temp = None
        pres = 101325.
        density = .08988  # kg/m^3
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_pressure_stp(self):
        species = 'H2'
        temp = 273.15
        pres = None
        density = .08988  # kg/m^3
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_density_stp(self):
        species = 'H2'
        temp = 273.15
        pres = 101325.
        density = None
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_pressure(self):
        species = 'H2'
        temp = 315.
        pres = None
        density = 1.
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_density(self):
        species = 'H2'
        temp = 315.
        pres = 101325.
        density = None
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        self.assertGreaterEqual(result["data"], 0.0)


if __name__ == "__main__":
    unittest.main()
