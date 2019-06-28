# -*- coding: utf-8 -*-

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

from __future__ import print_function, division


def TNT_mass_equivalent(mass_flammable_vapor_release, energy_yield, heat_of_combustion):
    '''
    Computes TNT mass equivalent
    TNT mass equivalent equation found in nureg-1805
    '''

    mass_equivalent = mass_flammable_vapor_release * energy_yield * heat_of_combustion / 4500
    
    return mass_equivalent


if __name__ == '__main__':
    #mString is mass of flammable vapor release
    mString = input("mass of flammable vapor release(kg): ")
    mass_flammable_vapor_release = float(mString)
    #yString is yield
    yString = input("yield(%): ")
    yield_percentage = float(yString)
    energy_yield = yield_percentage/100
    #heat of combustion for hydrogen unit: kJ/kg
    heat_of_combustion = 130800

    Calculated_Result = TNT_mass_equivalent(mass_flammable_vapor_release, energy_yield, heat_of_combustion)
    
    Result = []
    Result.append("TNT_Mass_Equivalent (kg)")
    Result.append(Calculated_Result)
    
    print(Result)
