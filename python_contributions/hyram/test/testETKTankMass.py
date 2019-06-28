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
from hyram.phys.wrapper.tank_mass import wrapper

class TestTankMass(unittest.TestCase):

    def testTankMassCalc(self):
        # Run test tank mass calculation
        T_H2 = 298
        P_H2 = 90000000
        TankVol = 0.01

        result = []

        result.append("Mass:" + str(wrapper(T=T_H2, P=P_H2, TankVol=TankVol)))
        
        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test values



if __name__ == '__main__':
    unittest.main()
