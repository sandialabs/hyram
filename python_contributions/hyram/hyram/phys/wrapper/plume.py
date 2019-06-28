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
import sys
import logging

import numpy as np

from .._comps import Gas, Orifice
from .._jet import Jet
from .._therm import AbelNoble
from .. import utils


def plot_plume(amb_pressure, amb_temp, h2_pressure, h2_temp, orifice_diam, discharge_coeff,
               xmin, xmax, ymin, ymax, contour, contourmin=0.0, contourmax=0.1,
               notional_nozzle='yuce', jet_angle=0,
               plot_title="Mole Fraction of Leak", output_dir=None, filename='PlumeMole_Plot.png',
               verbose=True):
    """
    Generate plot of gas plume.
    (Replaces PlumeWrap and PlumeWrapper funcs below.)

    Parameters
    ----------
    amb_pressure
    amb_temp
    h2_pressure
    h2_temp
    orifice_diam
    discharge_coeff
    xmin
    xmax
    ymin
    ymax
    contour
    notional_nozzle
    jet_angle
    output_dir : str
        output directory in which to place file
    plot_title : str
        filename of plot
    contourmin
    contourmax
    verbose

    Returns
    -------

    """
    log = logging.getLogger('hyram.phys')
    flow = None

    if output_dir is None:
        output_dir = os.getcwd()

    try:
        log.info("Creating ambient noble gas object...")
        ambient = Gas(AbelNoble(b=0, MW=28.97), T=amb_temp, P=amb_pressure)
        log.info("Creating source noble gas object...")
        source_gas = Gas(AbelNoble(), T=h2_temp, P=h2_pressure)
        log.info("Creating orifice...")
        orifice = Orifice(orifice_diam, discharge_coeff)

        log.info("Creating jet...")
        # Clean data
        notional_nozzle = utils.parse_nozzle_model(notional_nozzle)

        pl = Jet(source_gas, orifice, ambient, theta0=jet_angle, Q=flow, nnmodel=notional_nozzle, verbose=verbose)

    except Exception as err:  # TODO (Cianan): specify expected error types
        log.error("Failed to create plume objects:")
        log.exception(err)
        raise err

    # Make mole fraction contour plot
    log.info("Creating mole fraction contour plot...")
    plot_filepath = os.path.join(output_dir, filename)
    xlims = np.array([xmin, xmax])
    ylims = np.array([ymin, ymax])
    if type(contour) == int or type(contour) == float:
        contour = [contour]
    plot_fig = pl.plot_moleFrac_Contour(xlims=xlims, ylims=ylims, plot_title=plot_title, mark=contour,
                                        vmin=contourmin, vmax=contourmax)

    try:
        log.info("Attempting to save plot...")
        plot_fig.savefig(plot_filepath, bbox_inches='tight')
    except Exception as err:  # TODO (Cianan): specify expected error types
        log.error("Failed to save figure:")
        log.exception(err)
        raise err

    return plot_filepath


def PlumeWrap(amb_pressure, amb_temp, h2_pressure,
              h2_temp, orifice_diameter, discharge_coeff,
              jet_angle, directory, xlims, ylims, plot_title, contours,
              nnmodel = 'YuceilOtugen', contourmin = 0.0, contourmax = 0.1,
              verbose = True):
    if xlims is not None:
        if len(xlims) != 2:
            xlims = None
    if ylims is not None:
        if len(ylims) != 2:
            ylims = None
            
    return PlumeWrapper(amb_pressure, amb_temp, h2_pressure,
                        h2_temp, orifice_diameter,
                        discharge_coeff, theta0=jet_angle,
                        directory=directory, xlims=xlims, ylims=ylims, 
                        plot_title=plot_title, mark=contours, nnmodel=nnmodel, 
                        contourmin = contourmin, contourmax = contourmax,
                        verbose = verbose)

                        
def PlumeWrapper(P_amb, T_amb, P_H2, T_H2, d_orifice, Cd0, Q = None, 
                 n_pts_plume = 200, tol = 1e-8, theta0 = 0, x0 = 0, y0 = 0, 
                 S0 = 0, eng_model='no_energy', directory = os.getcwd(), 
                 xlims = None, ylims = None, plot_title = None, mark = None, 
                 source_gas_name = 'H2', nnmodel = 'YuceilOtugen', 
                 contourmin = 0.0, contourmax = 0.1, verbose = True):
    # Make the objects that are needed
    ambient = Gas(AbelNoble(b = 0, MW = 28.97), T = T_amb, P = P_amb)
    if source_gas_name != 'H2':
        sys.exit()
    else:
        source_gas = Gas(AbelNoble(), T = T_H2, P = P_H2)
    orifice = Orifice(d_orifice, Cd0)
    pl = Jet(source_gas, orifice, ambient, theta0=theta0, Q = Q,
             nnmodel=nnmodel, verbose=verbose)
    
    # Make mole fraction contour plot
    pc=pl.plot_moleFrac_Contour(xlims = xlims, ylims = ylims, 
                                plot_title = plot_title, mark = mark, 
                                vmin = contourmin, vmax = contourmax)
    
    # Save figure in desired directory
    fnameMole = os.path.join(directory, 'PlumeMole_Plot.png')
    pc.savefig(fnameMole, bbox_inches = 'tight')
    
    mole_key='Plume Mole Plot='+ fnameMole

    return mole_key
