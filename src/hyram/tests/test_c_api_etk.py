"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

import os
import numpy as np
import unittest
from hyram.phys import c_api


"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


# @unittest.skip
class ETKMassFlowTestCase(unittest.TestCase):
    """
    Test mass flow rate calculation, including steady and non-steady.
    Units are: Kelvin, Pa, m3, m
    """
    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        c_api.setup(self.output_dir, verbose=VERBOSE)

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
                                                           is_steady, tank_vol, dis_coeff,
                                                           self.output_dir)
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
                                                           is_steady, tank_vol, dis_coeff, self.output_dir)
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
                                                           is_steady, tank_vol, dis_coeff, self.output_dir)
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
        c_api.setup(self.output_dir, verbose=VERBOSE)

    def tearDown(self):
        pass

    def test_default(self):
        species = "H2"
        temp = 315.
        pres = 101325.
        tank_vol = 10.
        phase = None
        results = c_api.etk_compute_tank_mass(species, temp, pres, phase, tank_vol)
        mass = results["data"]
        self.assertGreaterEqual(mass, 0.0)

    def test_LH2(self):
        species = "H2"
        temp = None
        pres = 201325.
        tank_vol = 10.
        phase = 'liquid'
        results = c_api.etk_compute_tank_mass(species, temp, pres, phase, tank_vol)
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
        c_api.setup(self.output_dir, verbose=VERBOSE)

    def tearDown(self):
        pass

    def test_calc_temp_stp(self):
        species = 'H2'
        temp = None
        pres = 101325.
        density = .08988  # kg/m^3
        result = c_api.etk_compute_thermo_param(species, temp, pres, density)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_pressure_stp(self):
        species = 'H2'
        temp = 273.15
        pres = None
        density = .08988  # kg/m^3
        result = c_api.etk_compute_thermo_param(species, temp, pres, density)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_density_stp(self):
        species = 'H2'
        temp = 273.15
        pres = 101325.
        density = None
        result = c_api.etk_compute_thermo_param(species, temp, pres, density)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_pressure(self):
        species = 'H2'
        temp = 315.
        pres = None
        density = 1.
        result = c_api.etk_compute_thermo_param(species, temp, pres, density)
        self.assertGreaterEqual(result["data"], 0.0)

    def test_calc_density(self):
        species = 'H2'
        temp = 315.
        pres = 101325.
        density = None
        result = c_api.etk_compute_thermo_param(species, temp, pres, density)
        self.assertGreaterEqual(result["data"], 0.0)


if __name__ == "__main__":
    unittest.main()
