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

import os

import matplotlib.pyplot as plt

from .._comps import Gas, Orifice, Source
from .._therm import AbelNoble


def analyze_flow(temp=None, pressure=None, tank_vol=None, d_orifice=None, is_steady=True, discharge_coeff=1,
                 output_dir=None):
    """

    Parameters
    ----------
    temp : float
        temperature (K)

    pressure : float
        pressure (Pa)

    tank_vol : float
        Volume of source (tank) (m^3)

    d_orifice : float
        orifice diameter (m)

    is_steady : bool

    discharge_coeff : float
        discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow)

    output_dir : str
        Path to directory in which to place output file(s)

    Returns
    ----------
    result : dict
        mass_flow_rate : float
            mass flow rate (kg/s)
        time_to_empty : float
            (s)
        plot : str
            path to plot of mass flow rate vs. time. Only created if is_steady is false

    """
    if output_dir is None:
        output_dir = os.getcwd()

    release_gas = Gas(AbelNoble(), T=temp, P=pressure)
    orifice = Orifice(d_orifice, discharge_coeff)
    result = {"mass_flow_rate": None, "time_to_empty": None, "plot": ''}

    if is_steady:
        rho_throat = release_gas.therm.rho_Iflow(release_gas.therm.rho(release_gas.T, release_gas.P))
        T_throat = release_gas.therm.T_Iflow(release_gas.T, rho_throat)
        mdot = orifice.mdot(rho_throat, release_gas.therm.a(T_throat, rho_throat)*1.)
        result["mass_flow_rate"] = mdot

    else:
        source = Source(tank_vol, release_gas)
        mdots, gas_list, t = source.empty(orifice)

        filename = os.path.join(output_dir, "time-to-empty.png")
        plt.figure()
        plt.plot(t, mdots)
        plt.xlabel('Time [s]')
        plt.ylabel('Mass Flow Rate [kg/s]')
        plt.savefig(filename, bbox_inches='tight')

        result["time_to_empty"] = t[-1]
        result['plot'] = filename

    return result
