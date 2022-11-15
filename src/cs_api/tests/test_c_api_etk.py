"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from cs_api import phys
from hyram.utilities import misc_utils

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


class ETKMassFlowTestCase(unittest.TestCase):
    """
    Test mass flow rate calculation, including steady and non-steady.
    Units are: Kelvin, Pa, m3, m
    """
    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        phys.setup(self.output_dir, verbose=VERBOSE)

    def tearDown(self):
        pass

    def test_basic_blowdown(self):
        species = "H2"
        temp = 315
        pres = 1013250
        phase = 'default'
        tank_vol = 1  # Note that GUI units default to liters; backend is m^3
        orif_diam = 0.03
        is_steady = False
        dis_coeff = 1
        amb_pres = 101325

        wrapped_results = phys.etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                                                           is_steady, tank_vol, dis_coeff, amb_pres,
                                                           self.output_dir)
        data = wrapped_results["data"]

        empty_time = data["time_to_empty"]
        plot = data["plot"]
        times = data["times"]
        rates = data["rates"]

        self.assertTrue(plot is not None)
        self.assertGreaterEqual(empty_time, 0)
        self.assertTrue(t >= 0 for t in times)
        self.assertTrue(m >= 0 for m in rates)

    def test_invalid_low_pressure(self):
        species = "H2"
        temp = 288
        pres = 100000
        phase = 'default'
        tank_vol = 1
        orif_diam = 0.003
        is_steady = False
        dis_coeff = 1
        amb_pres = 101325

        wrapped_results = phys.etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                                                           is_steady, tank_vol, dis_coeff, amb_pres,
                                                           self.output_dir)
        data = wrapped_results["data"]

        self.assertFalse(wrapped_results['status'])
        self.assertEqual(data, None)

    def test_LH2_blowdown(self):
        species = 'H2'
        temp = None
        pres = 1013250
        phase = 'liquid'
        tank_vol = 1  # Note that GUI units default to liters; backend is m^3
        orif_diam = 0.03
        is_steady = False
        dis_coeff = 1
        amb_pres = 101325

        wrapped_results = phys.etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                                                          is_steady, tank_vol, dis_coeff, amb_pres,
                                                          self.output_dir)
        data = wrapped_results["data"]

        empty_time = data["time_to_empty"]
        plot = data["plot"]
        times = data["times"]
        rates = data["rates"]

        self.assertTrue(plot is not None)
        self.assertGreaterEqual(empty_time, 0)
        self.assertTrue(t >= 0 for t in times)
        self.assertTrue(m >= 0 for m in rates)

    @unittest.skip  # skipping due to duration of blend calc
    def test_ch4_n2_blowdown(self):
        species = {"ch4": 0.96, "n2": 0.04}
        temp = 315
        pres = 3e7
        phase = 'default'
        tank_vol = 1  # Note that GUI units default to liters; backend is m^3
        orif_diam = 0.03
        is_steady = False
        dis_coeff = 1
        amb_pres = 101325

        wrapped_results = phys.etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                                                          is_steady, tank_vol, dis_coeff, amb_pres,
                                                          self.output_dir)
        data = wrapped_results["data"]

        empty_time = data["time_to_empty"]
        plot = data["plot"]
        times = data["times"]
        rates = data["rates"]

        self.assertTrue(plot is not None)
        self.assertGreaterEqual(empty_time, 0)
        self.assertTrue(t >= 0 for t in times)
        self.assertTrue(m >= 0 for m in rates)


class ETKTankMassTestCase(unittest.TestCase):
    """
    Test calculation of tank mass.

    """

    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        phys.setup(self.output_dir, verbose=VERBOSE)

    def tearDown(self):
        pass

    def test_default(self):
        species = "H2"
        temp = 315
        pres = 101325
        vol = 10
        phase = None
        results = phys.etk_compute_tank_mass_param(species, temp, pres, phase, vol, mass=None)
        mass = results["data"]["param1"]
        self.assertGreaterEqual(mass, 0)

    def test_LH2(self):
        species = "H2"
        temp = None
        pres = 201325
        vol = 10
        phase = 'liquid'
        results = phys.etk_compute_tank_mass_param(species, temp, pres, phase, vol, mass=None)
        mass = results["data"]["param1"]
        self.assertGreaterEqual(mass, 0)

    def test_p_t_from_sat(self):
        species = "H2"
        phase = 'gas'
        temp = None
        pres = None
        vol = 10
        mass = 55.
        results = phys.etk_compute_tank_mass_param(species, temp, pres, phase, vol, mass)
        output1 = results["data"]["param1"]
        output2 = results["data"]["param2"]
        self.assertGreaterEqual(output1, 0)
        self.assertGreaterEqual(output2, 0)

    def test_m_t_from_sat(self):
        species = "H2"
        phase = 'gas'
        temp = None
        pres = 120000
        vol = 10
        mass = None
        results = phys.etk_compute_tank_mass_param(species, temp, pres, phase, vol, mass)
        output1 = results["data"]["param1"]
        output2 = results["data"]["param2"]
        self.assertGreaterEqual(output1, 0)
        self.assertGreaterEqual(output2, 0)


class ETKTPDTestCase(unittest.TestCase):
    """
    Test calculations of thermo properties.

    """

    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        phys.setup(self.output_dir, verbose=VERBOSE)

    def tearDown(self):
        pass

    def test_calc_temp_stp(self):
        species = 'H2'
        phase = None
        temp = None
        pres = 101325
        density = .08988  # kg/m^3
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertIsNone(param2)

    def test_calc_pressure_stp(self):
        species = 'H2'
        phase = None
        temp = 273.15
        pres = None
        density = .08988  # kg/m^3
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertIsNone(param2)

    def test_calc_density_stp(self):
        species = 'H2'
        phase = None
        temp = 273.15
        pres = 101325
        density = None
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertIsNone(param2)

    def test_calc_pressure(self):
        species = 'H2'
        phase = None
        temp = 315
        pres = None
        density = 1
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertIsNone(param2)

    def test_calc_density(self):
        species = 'H2'
        phase = None
        temp = 315
        pres = 101325
        density = None
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertIsNone(param2)

    def test_calc_density_saturated(self):
        species = 'H2'
        phase = 'gas'
        temp = None
        pres = 101325
        density = None
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertGreaterEqual(param2, 0)

    def test_calc_pressure_saturated(self):
        species = 'H2'
        phase = 'gas'
        temp = None
        pres = None
        density = 1
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertGreaterEqual(param2, 0)

    def test_calc_temp_saturated_and_density(self):
        species = 'H2'
        phase = 'liquid'
        temp = None
        pres = 101325
        density = None
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertGreaterEqual(param2, 0)

    def test_calc_temp_saturated_and_pressure(self):
        species = 'H2'
        phase = 'liquid'
        temp = None
        pres = None
        density = 71
        result = phys.etk_compute_thermo_param(species, phase, temp, pres, density)
        param1 = result['data']['param1']
        param2 = result['data']['param2']
        self.assertGreaterEqual(param1, 0)
        self.assertGreaterEqual(param2, 0)


class ETKTntMassTestCase(unittest.TestCase):

    def setUp(self):
        self.output_dir = misc_utils.get_temp_folder()
        phys.setup(self.output_dir, verbose=VERBOSE)

    def tearDown(self):
        pass

    def test_hydrogen(self):
        vapor_mass = 55
        percent_yield = 55
        fuel = 'h2'
        result = phys.etk_compute_equivalent_tnt_mass(vapor_mass, percent_yield, fuel)
        self.assertTrue(result["status"])
        self.assertGreater(result["data"], 0)
        self.assertIsNone(result["message"])

    def test_methane(self):
        vapor_mass = 55
        percent_yield = 55
        fuel = 'ch4'
        result = phys.etk_compute_equivalent_tnt_mass(vapor_mass, percent_yield, fuel)
        self.assertTrue(result["status"])
        self.assertGreater(result["data"], 0)
        self.assertIsNone(result["message"])

    def test_propane(self):
        vapor_mass = 55
        percent_yield = 55
        fuel = 'c3h8'
        result = phys.etk_compute_equivalent_tnt_mass(vapor_mass, percent_yield, fuel)
        self.assertTrue(result["status"])
        self.assertGreater(result["data"], 0)
        self.assertIsNone(result["message"])

    def test_ch4_n2_blend(self):
        vapor_mass = 55
        percent_yield = 55
        fuel = {'ch4': 0.96, 'n2': 0.04}
        result = phys.etk_compute_equivalent_tnt_mass(vapor_mass, percent_yield, fuel)
        self.assertTrue(result["status"])
        self.assertGreater(result["data"], 0)
        self.assertIsNone(result["message"])


if __name__ == "__main__":
    unittest.main()
