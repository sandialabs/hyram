"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.phys import Fluid

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


class PureFluidTestCase(unittest.TestCase):
    """
    Test engineering toolkit temperature, pressure, density API interface
    """
    def setUp(self):
        pass

    def test_air(self):
        fluid = Fluid(species='air', T=298, P=101325)
        self.assertAlmostEqual(fluid.T, 298.)
        self.assertAlmostEqual(fluid.P, 101325.)

        fluid = Fluid(species='air', T=290, P=103000)
        self.assertAlmostEqual(fluid.T, 290.)
        self.assertAlmostEqual(fluid.P, 103000)

    def test_bad_air(self):
        bad_name = 'ari'
        with self.assertRaises(ValueError):
            _ = Fluid(species=bad_name, T=290, P=103000)

    def test_hydrogen(self):
        fluid = Fluid(species='hydrogen', T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

        fluid = Fluid(species='h2', T=315, P=200000)
        self.assertAlmostEqual(fluid.T, 315.)
        self.assertAlmostEqual(fluid.P, 200000.)

    def test_hydrogen_dict(self):
        fluid = Fluid(species={'hydrogen': 1.0}, T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

        fluid = Fluid(species={'h2': 1.0}, T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

    def test_methane(self):
        fluid = Fluid(species='methane', T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

        fluid = Fluid(species='ch4', T=315, P=200000)
        self.assertAlmostEqual(fluid.T, 315.)
        self.assertAlmostEqual(fluid.P, 200000.)

    def test_methane_dict(self):
        fluid = Fluid(species={'methane': 1.0}, T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

        fluid = Fluid(species={'ch4': 1.0}, T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

    def test_propane(self):
        fluid = Fluid(species='propane', T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

        fluid = Fluid(species='c3h8', T=315, P=200000)
        self.assertAlmostEqual(fluid.T, 315.)
        self.assertAlmostEqual(fluid.P, 200000.)

    def test_propane_dict(self):
        fluid = Fluid(species={'propane': 1.0}, T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

        fluid = Fluid(species={'c3h8': 1.0}, T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)


class BlendFluidTestCase(unittest.TestCase):
    """
    Test engineering toolkit temperature, pressure, density API interface.

    """
    def setUp(self):
        pass

    def test_methane_blend(self):
        fluid = Fluid(species={'ch4': 0.965, 'n2': 0.035}, T=300, P=110000)
        self.assertAlmostEqual(fluid.T, 300.)
        self.assertAlmostEqual(fluid.P, 110000.)

    def test_insufficient_blend_raises_error(self):
        with self.assertRaises(ValueError):
            _ = Fluid(species={'methane': 0.5, 'h2': 0.4}, T=300, P=110000)

    def test_h2_methane_blend_with_default_t_p(self):
        fluid = Fluid(species={'ch4': 0.5, 'n2': 0.5}, T=287, P=35e6)
        self.assertAlmostEqual(fluid.T, 287.)
        self.assertAlmostEqual(fluid.P, 35e6)

