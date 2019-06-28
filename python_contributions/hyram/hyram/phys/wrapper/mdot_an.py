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

from .._comps import Orifice, Gas
from .._therm import AbelNoble


def compute_discharge_rate(temp, pressure, orifice_diam, discharge_coeff, verbose=True):
    """
    Returns the mass flow rate through an orifice of diameter d
    from gas at a temperature T and pressure P

    Parameters
    ----------
    temp : float
        gas temperature (K)

    pressure : float
        gas pressure (Pa)

    orifice_diam : float
        orifice diameter (m)

    discharge_coeff : float
        discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow)

    Returns
    -------
    rate : float
        mass flow rate (kg/s)
    """

    if verbose:
        print("computing discharge rate...")

    o = Orifice(orifice_diam, Cd=discharge_coeff)
    therm = AbelNoble()
    h2 = Gas(therm, T=temp, P=pressure)
    rhot = therm.rho_Iflow(h2.rho)  # throat density [kg/m**3]
    Tt = therm.T_IE(h2.T, h2.rho, rhot)  # temperature at throat [K]
    ct = therm.a(Tt, rhot)  # throat speed of sound [m/s]
    rate = o.mdot(rhot, ct)

    if verbose:
        print(rate)
        print("finished computing discharge rate...")

    return rate
