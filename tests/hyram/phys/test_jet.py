"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.phys import Fluid, Jet, Orifice


VERBOSE = False


class TestJetObject(unittest.TestCase):
    """
    Tests of Jet class
    """
    def setUp(self):
        release_fluid = Fluid('H2',
                              T=298,  # K
                              P=35e6)  # Pa
        ambient_fluid = Fluid('AIR',
                              T=298,  # K
                              P=101325)  # Pa
        orifice = Orifice(d=0.003)  # m
        jet = Jet(release_fluid,
                  orifice,
                  ambient_fluid,
                  verbose=VERBOSE)
        self.jet_obj = jet

    def test_get_xy_distances_to_molefrac_horizontal(self):
        #  Default jet object input for theta0 is 0, which is horizontal
        mole_fraction = 0.04
        distances = self.jet_obj.get_xy_distances_to_mole_fractions(mole_fraction)
        self.assertEqual(distances[mole_fraction][0][0], 0)
        self.assertGreater(distances[mole_fraction][0][1], 0)
        self.assertLess(distances[mole_fraction][1][0], 0)
        self.assertGreater(distances[mole_fraction][1][1], 0)

    def test_get_xy_distances_to_molefrac_vertical(self):
        vertical_jet = Jet(self.jet_obj.fluid,
                           self.jet_obj.developing_flow.orifice,
                           self.jet_obj.ambient,
                           theta0=1.570796)  # 90 deg
        mole_fraction = 0.04
        distances = vertical_jet.get_xy_distances_to_mole_fractions(mole_fraction)
        self.assertLess(distances[mole_fraction][0][0], 0)
        self.assertGreater(distances[mole_fraction][0][1], 0)
        self.assertEqual(distances[mole_fraction][1][0], 0)
        self.assertGreater(distances[mole_fraction][1][1], 0)

    def test_get_xy_distances_to_multiple_molefracs(self):
        mole_fractions = [0.04, 0.1]
        distances = self.jet_obj.get_xy_distances_to_mole_fractions(mole_fractions)
        for mole_frac in mole_fractions:
            self.assertIn(mole_frac, distances)
        sorted_mole_fracs = sorted(mole_fractions)
        self.assertLessEqual(distances[sorted_mole_fracs[0]][0][0],
                             distances[sorted_mole_fracs[1]][0][0])
        self.assertGreater(distances[sorted_mole_fracs[0]][0][1],
                           distances[sorted_mole_fracs[1]][0][1])
        self.assertLess(distances[sorted_mole_fracs[0]][1][0],
                        distances[sorted_mole_fracs[1]][1][0])
        self.assertGreater(distances[sorted_mole_fracs[0]][1][1],
                           distances[sorted_mole_fracs[1]][1][1])
