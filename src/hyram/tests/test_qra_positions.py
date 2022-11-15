"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

# TODO: add tests for each of the specific position distribution types

import unittest

from hyram.qra import positions


class TestPositionGenerator(unittest.TestCase):
    """
    Tests of generation of positions for QRA
    """
    def setUp(self):
        loc_distributions = [
            [1,  # number of occupants for this distribution
             ('deterministic', 1, None),  # distribution and parameters for x-direction
             ('deterministic', 2, None),  # distribution and parameters for y-direction
             ('deterministic', 3, None)],  # distribution and parameters for z-direction
            [2,  # number of occupants for this distribution
             ('uniform', 4, 5),  # distribution and parameters for x-direction
             ('deterministic', 6, None),  # distribution and parameters for y-direction
             ('deterministic', 7, None)]  # distribution and parameters for z-direction
        ]
        self.excl_radius = 0.01  # meters
        self.rand_seed = 3632850
        posgen = positions.PositionGenerator(loc_distributions, self.excl_radius, self.rand_seed)
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
             ('deterministic', 1, None),  # distribution and parameters for x-direction
             ('deterministic', 2, None),  # distribution and parameters for y-direction
             ('deterministic', 3, None)],  # distribution and parameters for z-direction
        ]
        posgen = positions.PositionGenerator(loc_distributions, self.excl_radius, self.rand_seed)
        locations = posgen.locs
        self.assertListEqual(locations, [])

    def test_reject_too_many_dists(self):
        loc_distributions = [
            [1,  # number of occupants for this distribution
             ('deterministic', 1, None),  # distribution and parameters for x-direction
                                            # y-direction missing
             ('deterministic', 3, None)],  # distribution and parameters for z-direction
        ]
        self.assertRaises(ValueError, positions.PositionGenerator, loc_distributions)

    def test_reject_too_few_dists(self):
        loc_distributions = [
            [1,  # number of occupants for this distribution
             ('deterministic', 1, None),  # distribution and parameters for x-direction
             ('deterministic', 2, None),  # distribution and parameters for y-direction
             ('deterministic', 2, None),  # extra distribution
             ('deterministic', 3, None)],  # distribution and parameters for z-direction
        ]
        self.assertRaises(ValueError, positions.PositionGenerator, loc_distributions)

    def test_reject_negative_number_occupants(self):
        loc_distributions = [
            [-1,  # negative number of occupants for this distribution
             ('deterministic', 1, None),  # distribution and parameters for x-direction
             ('deterministic', 2, None),  # distribution and parameters for y-direction
             ('deterministic', 3, None)],  # distribution and parameters for z-direction
        ]
        self.assertRaises(ValueError, positions.PositionGenerator, loc_distributions)

    def test_reject_non_integer_number_occupants(self):
        loc_distributions = [
            [1.1,  # float number of occupants for this distribution
             ('deterministic', 1, None),  # distribution and parameters for x-direction
             ('deterministic', 2, None),  # distribution and parameters for y-direction
             ('deterministic', 3, None)],  # distribution and parameters for z-direction
        ]
        self.assertRaises(ValueError, positions.PositionGenerator, loc_distributions)

    def test_reject_location_inside_exclusion_radius(self):
        loc_distributions = [
            [1,  # float number of occupants for this distribution
             ('deterministic', 0.005, None),  # distribution and parameters for x-direction
             ('deterministic', 0, None),  # distribution and parameters for y-direction
             ('deterministic', 0, None)],  # distribution and parameters for z-direction
        ]
        self.assertRaises(ValueError, positions.PositionGenerator, loc_distributions, self.excl_radius)



if __name__ == "__main__":
    unittest.main()
