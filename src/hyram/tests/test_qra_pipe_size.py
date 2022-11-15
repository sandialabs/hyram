"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.qra import pipe_size


class TestPipeSizeCalcs(unittest.TestCase):
    """
    Test basic calculation of pipe sizes
    """
    def test_calc_pipe_inner_diameter(self):
        pipe_outer_diameter = 0.17  # m
        pipe_wall_thickness = 0.01  # m
        pipe_inner_diameter = pipe_size.calc_pipe_inner_diameter(pipe_outer_diameter,
                                                                 pipe_wall_thickness)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(pipe_inner_diameter, 0.15)

    def test_calc_pipe_flow_area(self):
        pipe_inner_diameter = 0.1  # m
        pipe_flow_area = pipe_size.calc_pipe_flow_area(pipe_inner_diameter)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(pipe_flow_area, 0.007853982)

    def test_calc_orifice_diameter(self):
        pipe_flow_area = 0.08  # m^2
        leak_size_fraction = 0.1
        leak_diameter = pipe_size.calc_orifice_diameter(pipe_flow_area,
                                                        leak_size_fraction)
        # Hand-calculation of above numbers
        self.assertAlmostEqual(leak_diameter, 0.100925301)


if __name__ == "__main__":
    unittest.main()
