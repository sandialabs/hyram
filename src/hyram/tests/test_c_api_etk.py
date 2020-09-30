import os
import numpy as np
import unittest
from hyram.phys import c_api


""" NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.

TODO: determine appropriate sig figs, relative error or absolute error guidelines for tests.
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

        desired_times = np.array([0.0, 0.027727759451148137, 0.17358174468387583, 0.42435699663780974, 
            0.7388127089252341, 1.1088919666821084, 1.5149267545447858, 1.8007525940667102, 
            2.2327490428006587, 2.5778932859468826, 2.8951012115330577, 3.1441033247788597, 
            3.3419170790933443, 3.499415598346434, 3.6015051881334204, 3.703041984181422, 
            3.774879483137331, 3.781580447540091, 3.799587973480758, 3.817595499421425, 
            3.835603025362092, 3.8536105513027588, 3.8716180772434257, 3.8896256031840926, 
            3.8914263557781594, 3.8932271083722263, 3.895027860966293, 3.89682861356036, 
            3.898629366154427, 3.9004301187484938, 3.9022308713425606, 3.9040316239366275])

        desired_rates = np.array([0.4306160064474597, 0.4227260845734627, 0.38391195960289254, 
            0.32651042990964935, 0.2681392239727423, 0.21438768009748607, 0.16927922623399946, 
            0.1441128821955624, 0.11387196959865735, 0.09346865599671554, 0.07335021480562373, 
            0.056524730226872176, 0.04258374077007213, 0.031183274624415485, 0.023681117800952535, 
            0.016153555821764108, 0.010798912515667537, 0.010298527384590748, 0.008953220504200849, 
            0.007607104713508785, 0.006260300689884663, 0.004912929743308802, 0.0035651139197227, 
            0.002216979024370546, 0.0020821519012491786, 0.0019473228874911349, 0.0018124921054906932, 
            0.0016776596776204467, 0.0015428257263310828, 0.0014079903740879542, 0.0012731537434090617, 
            0.0011383159569150225])

        self.assertTrue(plot is not None)
        np.testing.assert_approx_equal(empty_time, 3.90403, significant=4)
        np.testing.assert_allclose(times, desired_times, atol=0.005)
        np.testing.assert_allclose(rates, desired_rates, atol=0.005)

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

        desired_times = np.array([0.0, 0.044477947582451736, 0.4671917011173726, 2.438508282916699, 
            5.662308781372499, 9.46582884257959, 13.566931322904349, 17.55127065081946, 21.585992229661215, 
            24.40302170105298, 28.08270470060588, 30.884010658642534, 33.212212826383166, 35.10254130163892, 
            36.454815463473636, 36.857668366720866, 37.85679081565053, 38.54452267280182, 38.61279543180281, 
            38.788308138311, 38.963820844819196, 39.13933355132739, 39.31484625783558, 39.49035896434378, 
            39.66587167085197, 39.841384377360164, 40.01689708386836, 40.19240979037655, 40.20996106102737, 
            40.22751233167819, 40.24506360232901])

        desired_rates = np.array([2.8220450695170753, 2.8165175694740205, 2.764127724849026, 2.524250682143973, 
            2.153712260339367, 1.760137740988422, 1.3941932385420555, 1.096950771099861, 0.8508688956470504, 
            0.7086871974071859, 0.5551608741780298, 0.45594174082043654, 0.3630822246587767, 0.2772537795678414, 
            0.2104192652843857, 0.1897274939738959, 0.13709249272432994, 0.09995726679921516, 0.09623835609671016, 
            0.08665483821300077, 0.07704059827891326, 0.06739888612295662, 0.0577330164056871, 0.048046361498307294, 
            0.038342344166787286, 0.02862443066818914, 0.01889612813949851, 0.009166462173869183, 0.008192700419408044, 
            0.007218908813270732, 0.006245090908423705])

        self.assertTrue(plot is not None)
        np.testing.assert_approx_equal(empty_time, 40.24506, significant=4)
        np.testing.assert_allclose(times, desired_times, atol=0.005)
        np.testing.assert_allclose(rates, desired_rates, atol=0.005)


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
        np.testing.assert_approx_equal(mass, 0.7795, significant=4)

    def test_LH2(self):
        # TODO: verify this result is accurate
        species = "H2"
        temp = None
        pres = 201325.
        tank_vol = 10.
        phase = 'liquid'
        results = c_api.etk_compute_tank_mass(species, temp, pres, phase, tank_vol, self.debug)
        mass = results["data"]
        np.testing.assert_approx_equal(mass, 676., significant=3)


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
        np.testing.assert_approx_equal(result["data"], 273.15, significant=5)

    def test_calc_pressure_stp(self):
        species = 'H2'
        temp = 273.15
        pres = None
        density = .08988  # kg/m^3
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        np.testing.assert_approx_equal(result['data'], 101325., significant=5)

    def test_calc_density_stp(self):
        species = 'H2'
        temp = 273.15
        pres = 101325.
        density = None
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        np.testing.assert_approx_equal(result['data'], .089882, significant=5)

    def test_calc_pressure(self):
        species = 'H2'
        temp = 315.
        pres = None
        density = 1.
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        np.testing.assert_approx_equal(result["data"], 1308875.3, significant=7)

    def test_calc_density(self):
        species = 'H2'
        temp = 315.
        pres = 101325.
        density = None
        debug = False
        result = c_api.etk_compute_thermo_param(species, temp, pres, density, debug)
        np.testing.assert_approx_equal(result["data"], 0.0779475, significant=4)


if __name__ == "__main__":
    unittest.main()
