#  Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
#  Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
#  .
#  This file is part of HyRAM (Hydrogen Risk Assessment Models).
#  .
#  HyRAM is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#  .
#  HyRAM is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#  .
#  You should have received a copy of the GNU General Public License
#  along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

import unittest
from hyram.phys.wrapper.mdot_an import compute_discharge_rate

class TestMDot(unittest.TestCase):

    def testMdot(self):
        # Run test mdot calculation
        T = 288.15
        P = 35000000
        d = [7.874015748E-05, 0.000248998240957144, 0.0007874015748,
             0.00248998240957144, 0.007874015748]
        Cd = 1
        result = []

        for element_d in d:
            result.append(compute_discharge_rate(T, P, element_d, Cd, verbose=False))

        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test values


if __name__ == '__main__':
    unittest.main()
