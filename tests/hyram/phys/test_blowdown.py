"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest
import numpy as np

from hyram.phys import Fluid, Orifice, Source

VERBOSE = False

class TestBlowdown(unittest.TestCase):
    """
    Tests of tank blowdown
    """
    def setUp(self):
        release_fluid = Fluid('H2',
                              T=298,  # K
                              P=70e6)  # Pa
        self.orifice = Orifice(d=0.001)  # m
        self.tank = Source.fromMass(5, release_fluid)

    def test_isentropic_vs_heat_flow(self):
        '''tests that mass flow rate, temperature, and pressure in tank are similar (after 100 seconds)
        for an isentropic blowdown (where interpolating functions are used) and a blowdown with a very small heat leak
        (where interpolating functions are not used)'''
        (mdot_isen, fluid_list_isen, t_isen, solution_isen) = self.tank.empty(self.orifice)
        (mdot_not_isen, fluid_list_not_isen, t_not_isen, solution_not_isen) = self.tank.empty(self.orifice, heat_flux = 1e-10)

        self.assertAlmostEqual(np.interp(100, t_isen, mdot_isen), np.interp(100, t_not_isen, mdot_not_isen), delta = 1e-4)
        self.assertAlmostEqual(np.interp(100, t_isen, np.array([f.P for f in fluid_list_isen])),
                               np.interp(100, t_not_isen, np.array([f.P for f in fluid_list_not_isen])), delta = 1000)
        self.assertAlmostEqual(np.interp(100, t_isen, np.array([f.T for f in fluid_list_isen])),
                               np.interp(100, t_not_isen, np.array([f.T for f in fluid_list_not_isen])), delta = 1e-1)