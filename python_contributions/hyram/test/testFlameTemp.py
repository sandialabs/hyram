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
from hyram.phys.wrapper.flame import plotT

class TestFlameTemp(unittest.TestCase):

    def testFlameT(self):
        # Run test flame temperature model
        T_amb = 288.15
        P_amb = 101325
        T_H2 = 287.8
        P_H2 = 13420000
        d_orifice = 0.0036
        y0 = 1
        ReleaseAngle = 0
        nnmodel = "YuceilOtugen"

        directory = os.getcwd()
        result = []

        result.append(plotT(T_amb, P_amb, T_H2, P_H2, d_orifice, y0,
                            ReleaseAngle, nnmodel, directory, verbose=False))

        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test that files exist
        fname = os.path.join(directory, 'Tplot.png')
        self.assertTrue(os.path.isfile(fname))
        
        # Test values


if __name__ == '__main__':
    unittest.main()
