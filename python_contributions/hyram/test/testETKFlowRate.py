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
import os
from hyram.phys.wrapper.mass_flow_rate import wrapper

class TestMassFlowRate(unittest.TestCase):

    def testMassFlowSteady(self):
        # Run test tank mass calculation
        TankTemperature = 298
        InitialHydrogenPressure = 90000000
        TankVolume = 0
        SteadyFlag = True
        OrificeDiameter = 0.03

        result = []

        result.append(wrapper(T=TankTemperature, P=InitialHydrogenPressure, TankVol=TankVolume, d_orifice=OrificeDiameter,Steady=SteadyFlag, Cd0=1))
        
        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test values

    def testMassFlowBlowdown(self):
        # Run test tank mass calculation
        TankTemperature = 298
        InitialHydrogenPressure = 90000000
        TankVolume = 0.005
        SteadyFlag = False
        OrificeDiameter = 0.03

        result = []

        result.append(wrapper(T=TankTemperature, P=InitialHydrogenPressure, TankVol=TankVolume, d_orifice=OrificeDiameter,Steady=SteadyFlag, Cd0=1))
        
        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test that image file exists
        directory = os.getcwd()
        filename = os.path.join(directory, "image.png")
        self.assertTrue(os.path.isfile(filename))

        # Test values


if __name__ == '__main__':
    unittest.main()
