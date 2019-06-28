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
from hyram.phys.wrapper.plume import PlumeWrap

class TestPhysPlume(unittest.TestCase):

    def testPlume(self):
        # Run test plume model
        ambient_pressure = 101325
        ambient_temperature = 288.15
        H2_pressure = 13420000
        H2_temperature = 287.8
        orifice_diameter = 0.0036
        orifice_discharge_coefficient = 1
        angle_of_jet = 1.5708
        Xlims = [ -2.5, 2.5 ]
        Ylims = [ 0, 10 ]
        plot_title = "Mole Fraction of Leak"
        contours = [ 0.04 ]
        
        directory = os.getcwd()
        result = []

        result.append(PlumeWrap(ambient_pressure, ambient_temperature,
                                H2_pressure, H2_temperature, orifice_diameter,
                                orifice_discharge_coefficient, angle_of_jet,
                                directory, Xlims, Ylims, plot_title,
                                contours, verbose=False))

        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test that file exists
        fnameMole = os.path.join(directory, 'PlumeMole_Plot.png')
        self.assertTrue(os.path.isfile(fnameMole))

        # Test values


if __name__ == '__main__':
    unittest.main()
