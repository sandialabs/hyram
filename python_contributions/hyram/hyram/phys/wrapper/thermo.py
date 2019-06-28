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

from .._therm import AbelNoble


def compute_tpr(temp=None, pressure=None, rho=None):
    # using defaults so it will be h2
    # set b = 0 to get ideal gas
    therm = AbelNoble()

    if temp != None and pressure != None:
        result = therm.rho(temp, pressure)
        # temp = therm.T
        # pressure = therm.P
    elif temp != None and rho != None:
        result = therm.P(temp, rho)
        # temp = T
        # self.rho = therm.rho
    elif pressure != None and rho != None:
        result = therm.T(pressure, rho)
        # self.rho = rho
        # self.pressure = P

    return result
