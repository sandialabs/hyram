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
from hyram.qra.wrapper import run_qrad

class TestQradCalc(unittest.TestCase):

    def testQrad(self):
        # Run test qrad calculation
        T_amb = 288.15
        P_amb = 101325
        T_H2 = 288.15
        P_H2 = 35000000
        orifice_diameters = [0.000079, 0.000249, 0.000787, 0.00249, 0.007874]
        leak_height = 1
        angle_of_release = 0
        notional_nozzle_model = "Birch2"
        loc_distributions = [[9,
                              ["uniform", 1, 20],
                              ["deterministic", 1, None],
                              ["uniform", 1, 12]]]
        exclusion_radius = 0.01
        random_seed = 3632850
        RH = 0.89
        radiative_source_model = "multi"
        facility_length = 20
        facility_width = 12

        result = []

        result.append(run_qrad(T_amb, P_amb, T_H2, P_H2, orifice_diameters,
                               leak_height, angle_of_release, 
                               notional_nozzle_model, loc_distributions, 
                               exclusion_radius, random_seed, RH, 
                               radiative_source_model, facility_length, 
                               facility_width, verbose=False))

        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test values


if __name__ == '__main__':
    unittest.main()
