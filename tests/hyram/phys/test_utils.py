"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

import hyram.phys._utils as hpu


class TestGetDistanceToEffect(unittest.TestCase):
    """
    Test of function to get distance to a physical effect
    """
    @staticmethod
    def effect_function(x_vals, y_vals, z_vals, effect_val=1):
        """
        Dummy effect function that returns values for
        a 1/r decaying sphere centered at (1, 1, 0)
        with a dummy optional keyword argument that scales the values
        """
        origin = (1, 1, 0)
        effects = []
        for x_val, y_val, z_val in zip(x_vals, y_vals, z_vals):
            distance = ((x_val - origin[0]) ** 2
                        + (y_val - origin[1]) ** 2
                        + (z_val - origin[2]) ** 2) ** (1/2)
            if distance == 0:
                effect = 1e99
            else:
                effect = 1 / distance * effect_val
            effects.append(effect)
        return effects

    def test_effect_function(self):
        # check that both (1, 0, 0) and (0, 1, 0) are equal to 1
        self.assertEqual(self.effect_function([1], [0], [0]),
                         self.effect_function([0], [1], [0]))
        # check that maximum occurs at (1, 1, 0)
        x_vals = [1, 1.1, 1,  1, 1, 1, 0.9]
        y_vals = [1, 1, 1.1,  1, 0.9, 1, 1]
        z_vals = [0, 0, 0, -0.1, 0, 0.1, 0]
        effects = self.effect_function(x_vals, y_vals, z_vals)
        for non_max_effect in effects[1:]:
            self.assertGreater(effects[0], non_max_effect)

    def test_get_distance_to_effect(self):
        effect_value = 1
        from_point = (0, 1, 0)
        direction = 'x'
        effect_func = self.effect_function
        distance = hpu.get_distance_to_effect(effect_value, from_point, direction, effect_func)
        self.assertAlmostEqual(distance, 2, places=4)

    def test_get_distance_to_effect_with_kwarg(self):
        effect_value = 0.5
        from_point = (0, 1, 0)
        direction = 'x'
        effect_func = self.effect_function
        effect_scaling = 0.5
        distance = hpu.get_distance_to_effect(effect_value, from_point, direction, effect_func, effect_val=effect_scaling)
        self.assertAlmostEqual(distance, 2, places=4)
