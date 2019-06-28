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
from hyram.phys.wrapper.tnt_mass_equivalent import TNT_mass_equivalent

class TestTNTcalc(unittest.TestCase):

    def testTNTMassEquiv(self):
        # Run test TNT calculation
        mass_flammable_vapor_release = 1.0
        energy_yield = 1.0
        heat_of_combustion = 130800

        result = []

        result.append("TNT_Mass_Equivalent (kg)")
        result.append(TNT_mass_equivalent(mass_flammable_vapor_release, energy_yield, heat_of_combustion))

        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test values


if __name__ == '__main__':
    unittest.main()
