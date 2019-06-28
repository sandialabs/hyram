"""
Wrapper file for qra mode specifically.

Prevents needing to call out to separate wrappers for each 
leak size.  Additionally, allows reusing of single h2chem
object for each flame (creating h2chem is time-consuming).
"""

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

from __future__ import print_function, absolute_import, division

import os

import numpy as np

from .._therm import H2_Combustion
from .._positions import PositionGenerator
from .flame import fl, calcQ


def run_qrad(amb_temp, amb_pressure, h2_temp, h2_pressure, orifice_diameters,
             leak_height, release_angle, notional_nozzle_model,
             loc_distributions, exclusion_radius, random_seed,
             relative_humidity, radiative_source_model, facility_length, 
             facility_width, verbose=True, create_plots=True, chem_file=None, output_dir=None):

    # Generate the positions
    posgen = PositionGenerator(loc_distributions, exclusion_radius, random_seed)
    posgen.gen_positions()
    xlocs=posgen.get_xlocs()
    ylocs=posgen.get_ylocs()
    zlocs=posgen.get_zlocs()
    positions=posgen.locs
    
    # Make single h2chem that all of the flames will use
    h2chem = H2_Combustion(amb_temp, amb_pressure, verbose=verbose)

    # Make some lists to store results for all of the orifice diams
    all_qrads = np.zeros((len(orifice_diameters), posgen.totworkers))
    all_iso_fname = []
    all_T_fname = []
    all_pos_fname = []
    all_pos_filepaths = []

    if output_dir is None:
        output_dir = os.getcwd()
    
    for i in range(len(orifice_diameters)):
        iso_fname = 'isoPlot{}'.format(i)
        T_fname = 'Tplot{}'.format(i)
        
        # Make flame object
        flame = fl(T_amb=amb_temp, P_amb=amb_pressure, T_H2=h2_temp,
                   P_H2=h2_pressure, d_orifice=orifice_diameters[i],
                   theta0=release_angle, y0=leak_height,
                   h2chem=h2chem, nnmodel=notional_nozzle_model,
                   verbose=verbose, chem_file=chem_file)
        
        # Calculate heat flux at locations
        Q = calcQ(x=xlocs, y=ylocs, z=zlocs, RH=relative_humidity,
                  flame=flame, smodel=radiative_source_model)

        # Each row is the heatflux for all locs for specific leak size
        all_qrads[i,:] = Q
        all_iso_fname.append(iso_fname)
        all_T_fname.append(T_fname)

        # Plot the heatflux position plot
        orifice_diameter_mm = orifice_diameters[i]*1000
        if create_plots:
            pos_fname = 'positionPlot{}.png'.format(i)
            plot_filepath = os.path.join(output_dir, pos_fname)
            pos_title = '{} mm Leak Size'.format(round(orifice_diameter_mm, 3))
            posgen.plot_positions(plot_filepath, pos_title, Q, facility_length, facility_width)
            all_pos_fname.append(pos_fname)
            all_pos_filepaths.append(plot_filepath)

    # Flatten heatflux (all leaksize loc1, then all leaksize loc2, etc)
    # Corresponds to flattening by row which is C-style ordering
    qrad_flattened = all_qrads.flatten(order='C')
    
    # Multiply by 1000 to convert from kilowatts to watts
    # Previously this was done in C# code
    qrad_flattened *= 1000.0

    result_dict = {
        "qrad_flattened": qrad_flattened,
        "all_iso_fname": all_iso_fname,
        "all_t_fname": all_T_fname,
        "all_pos_fname": all_pos_fname,
        "all_pos_files": all_pos_filepaths,
        "xlocs": xlocs,
        "ylocs": ylocs,
        "zlocs": zlocs,
        "positions": positions,
    }

    return result_dict
