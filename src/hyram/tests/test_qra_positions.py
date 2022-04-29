"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


# TODO: add tests for each of the specific position distribution types

import unittest

import hyram.qra.positions as hyram_pos


class TestPositionGenerator(unittest.TestCase):
    """
    Tests of generation of positions for QRA
    """
    def setUp(self):
        loc_distributions = [
            [1,  # number of occupants for this distribution
             ('deterministic', 1.0, None),  # distribution and parameters for x-direction
             ('deterministic', 2.0, None),  # distribution and parameters for y-direction
             ('deterministic', 3.0, None)],  # distribution and parameters for z-direction
            [2,  # number of occupants for this distribution
             ('uniform', 4.0, 5.0),  # distribution and parameters for x-direction
             ('deterministic', 6.0, None),  # distribution and parameters for y-direction
             ('deterministic', 7.0, None)]  # distribution and parameters for z-direction
        ]
        self.excl_radius = 0.01  # meters
        self.rand_seed = 3632850
        posgen = hyram_pos.PositionGenerator(loc_distributions, self.excl_radius, self.rand_seed)
        self.posgen = posgen

    def test_get_locations(self):
        num_positions = len(self.posgen.locs)
        self.assertEqual(num_positions, 3)

    def test_get_xlocs(self):
        xlocs = self.posgen.get_xlocs()
        self.assertEqual(len(xlocs), 3)

    def test_get_ylocs(self):
        ylocs = self.posgen.get_ylocs()
        self.assertListEqual(ylocs, [2, 6, 6])

    def test_get_zlocs(self):
        zlocs = self.posgen.get_zlocs()
        self.assertListEqual(zlocs, [3, 7, 7])

    def test_zero_occupants(self):
        loc_distributions = [
            [0,  # number of occupants for this distribution
             ('deterministic', 1.0, None),  # distribution and parameters for x-direction
             ('deterministic', 2.0, None),  # distribution and parameters for y-direction
             ('deterministic', 3.0, None)],  # distribution and parameters for z-direction
        ]
        posgen = hyram_pos.PositionGenerator(loc_distributions, self.excl_radius, self.rand_seed)
        locations = posgen.locs
        self.assertListEqual(locations, [])



if __name__ == "__main__":
    unittest.main()
