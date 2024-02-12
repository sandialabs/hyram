"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import unittest

from hyram.phys._therm import CoolPropWrapper, Combustion
from hyram.phys import Fluid


VERBOSE = False


class TestPropsSI(unittest.TestCase):
    def setUp(self):
        self.pressure = 1286500
        self.entropy = 10761.12726
        self.therm = CoolPropWrapper('H2')

    def test_wrapper_single(self):
        t = self.therm.PropsSI('T', P=self.pressure, S=self.entropy)
        self.assertTrue

    def test_wrapper_multi(self):
        h, rho = self.therm.PropsSI(['H', 'D'], P=self.pressure, S=self.entropy)
        self.assertTrue

    def test_wrapper_blend(self):
        therm = CoolPropWrapper("METHANE[0.5]&HYDROGEN[0.5]")
        t = therm.PropsSI('T', P=self.pressure, S=self.entropy)
        self.assertTrue

class TestCombustion(unittest.TestCase):
    def setUp(self):
        H2 = Fluid('H2', P = 101325, T = 298)
        CH4 = Fluid('CH4', P = 101325, T = 298)
        self.comb_H2 = Combustion(H2)
        self.comb_CH4 = Combustion(CH4)

    def test_H2_absorption_coeff(self):
        self.assertAlmostEqual(self.comb_H2.absorption_coeff, 0.23, None, "H2 absorption coefficient not 0.23", 0.02)

    def test_CH4_absorption_coeff(self):
        self.assertAlmostEqual(self.comb_CH4.absorption_coeff, 0.5, None, "CH4 absorption coefficient not 0.5", 0.02)
