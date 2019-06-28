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

import datetime
import gc
import os
import logging
import numpy as np

from ..utilities import c_utils
from .wrapper import mass_flow_rate, tank_mass, thermo, plume, overpressure, flame


def compute_mass_flow_rate(temp, pressure, tank_volume, d_orifice,
                           is_steady=True, discharge_coeff=1., output_dir=None, debug=False):
    """

    Parameters
    ----------
    temp : float
        temperature (K)

    pressure : float
        pressure (Pa)

    tank_volume : float
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
        if is_steady, mass flow rate will be returned. If false, time_to_empty and plot are returned.
        mass_flow_rate : float
            mass flow rate (kg/s) of steady release
        time_to_empty : float
            (s) time it takes to blowdown the tank to empty
        plot : str
            path to plot of mass flow rate vs. time. Only created if Steady is false

    """

    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    now = datetime.datetime.now()

    # Set up logger to store log in separate file for each call. Null handler if error.
    try:
        log_name = now.strftime('log-etk-flow_%Y-%m-%d_%H-%M-%S.log')
        logfile = os.path.join(output_dir, log_name)
        log = logging.getLogger('hyram.phys.mass')
        file_handler_info = logging.FileHandler(logfile, mode='w')
        log.addHandler(file_handler_info)
    except Exception as exc:
        log = logging.getLogger('hyram.phys.mass').addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.ERROR)

    log.info("Mass flow analysis parameters:")
    log.info("temp: {}".format(temp))
    log.info("pressure: {}".format(pressure))
    log.info("tank_volume: {}".format(tank_volume))
    log.info("d_orifice: {}".format(d_orifice))
    log.info("is_steady: {}\n".format(is_steady))
    try:
        log.info("API call for mass flow calculation...")
        result = mass_flow_rate.analyze_flow(temp, pressure, tank_volume, d_orifice, is_steady=is_steady,
                                             discharge_coeff=discharge_coeff, output_dir=output_dir)
        log.info("Mass flow rate results: {}".format(result))
    except Exception as exc:
        log.info("Mass flow calculation failed with message {}".format(str(exc)))
        result = np.nan
        if debug:
            log.exception(exc)

    gc.collect()
    return result


def compute_tank_mass(temp, pressure, tank_volume, output_dir=None, debug=False):
    """
    Calculate tank mass

    Parameters
    ----------
    temp : float
        temperature (K)

    pressure : float
        pressure (Pa)

    tank_volume : float
        Volume of source (tank) (m^3)

    output_dir : str
        Path to directory in which to place output file(s)

    Returns
    ----------
    result : float
        Tank mass in kg

    """
    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    now = datetime.datetime.now()

    try:
        log_name = now.strftime('log-etk-tank_%Y-%m-%d_%H-%M-%S.log')
        logfile = os.path.join(output_dir, log_name)
        log = logging.getLogger('hyram.phys.tank')
        file_handler_info = logging.FileHandler(logfile, mode='w')
        log.addHandler(file_handler_info)
    except Exception as exc:
        log = logging.getLogger('hyram.phys.tank').addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.ERROR)

    log.info("Tank mass calculation parameters:")
    log.info("temp: {}".format(temp))
    log.info("pressure: {}".format(pressure))
    log.info("tank_volume: {}".format(tank_volume))

    try:
        log.info("API call for tank mass calculation...")
        mass = tank_mass.compute_tank_mass(temp=temp, pressure=pressure, tank_volume=tank_volume)
        log.info("Tank mass calculated: {}".format(mass))

    except Exception as exc:
        log.info("Tank mass calculation failed with message {}".format(str(exc)))
        if debug:
            log.exception(exc)
        mass = None

    gc.collect()
    return mass


def access_thermo_calculations(temp, pressure, density, output_dir=None, debug=False):
    """
    Calculate temperature, pressure or density

    Parameters
    ----------
    temp : float
        temperature (K)

    pressure : float
        pressure (Pa)

    density : float
        kg/m^3

    output_dir : str
        Path to directory in which to place output file(s)

    Returns
    ----------
    result : float
        Result depends on provided inputs. Third param will be calculated from other two.

    """
    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    try:
        now = datetime.datetime.now()
        log_name = now.strftime('log-etk-thermo_%Y-%m-%d_%H-%M-%S.log')
        logfile = os.path.join(output_dir, log_name)
        log = logging.getLogger('hyram.phys.thermo')
        file_handler_info = logging.FileHandler(logfile, mode='w')
        log.addHandler(file_handler_info)
    except Exception as exc:
        log = logging.getLogger('hyram.phys.thermo').addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.INFO)

    log.info("Tank mass calculation parameters:")
    log.info("temp: {} K".format(temp))
    log.info("pressure: {} Pa".format(pressure))
    log.info("density: {} kg/m3".format(density))

    try:
        log.info("API call for thermo calculation...")
        result = thermo.compute_tpr(temp=temp, pressure=pressure, rho=density)
        log.info("Result: {}".format(result))
    except Exception as exc:
        log.info("API call failed with message {}".format(str(exc)))
        if debug:
            log.exception(exc)
        result = np.nan

    gc.collect()
    return result


def create_plume_plot(amb_pressure, amb_temp, h2_pressure, h2_temp, orifice_diam, discharge_coeff,
                      xmin, xmax, ymin, ymax, contour, jet_angle, plot_title,
                      output_dir=None, debug=False, verbose=False, filename=None):
    """
    Create plume plot for leak. See called function for parameter descriptions.

    Returns
    -------
    string plot file path

    """
    params = locals()
    sorted_params = sorted(params)
    now = datetime.datetime.now()
    log_name = now.strftime('log-plume_%Y-%m-%d_%H-%M-%S.log')

    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    if filename is None:
        filename = now.strftime('plume-mole-plot-%Y-%m-%d_%H-%M-%S.png')

    try:
        logfile = os.path.join(output_dir, log_name)
        log = logging.getLogger('hyram.phys.plume')
        file_handler_info = logging.FileHandler(logfile, mode='w')
        log.addHandler(file_handler_info)
    except Exception as exc:
        log = logging.getLogger('hyram.phys.plume').addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.INFO)

    log.info("PLUME PLOT GENERATION")
    for param in sorted_params:
        log.info("{}: {}".format(param, params[param]))

    try:
        log.info("API call for plume plot...")
        result = plume.plot_plume(amb_pressure, amb_temp, h2_pressure, h2_temp, orifice_diam, discharge_coeff,
                                  xmin, xmax, ymin, ymax, contour, jet_angle=jet_angle,
                                  plot_title=plot_title, output_dir=output_dir, filename=filename, verbose=verbose)

        log.info("Result: {}".format(result))
    except Exception as exc:
        log.info("Plume plot generation failed with message {}".format(str(exc)))
        if debug:
            log.exception(exc)
        result = ""

    return result


def overpressure_indoor_release(amb_pres, amb_temp, h2_pres, h2_temp, tank_volume, orifice_diameter,
                                orifice_discharge_coeff, release_discharge_coeff, release_area,
                                release_height, enclosure_height,
                                floor_ceiling_area, dist_release_to_wall,
                                ceiling_vent_xarea, ceiling_vent_height_from_floor,
                                floor_vent_xarea, floor_vent_height_from_floor,
                                vol_flow_rate, release_angle, times, dot_mark_pressures, dot_mark_times,
                                limit_line_pressures,
                                max_sim_time, output_dir=None, debug=False):
    """ Conduct indoor release analysis. See overpressure.analyze_indoor_release for input descriptions. """
    # Sort dict of local variables for logging
    params = locals()
    sorted_params = sorted(params)

    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    try:
        now = datetime.datetime.now()
        log_name = now.strftime('log-overp_%Y-%m-%d_%H-%M-%S.log')
        logfile = os.path.join(output_dir, log_name)
        log = logging.getLogger('hyram.phys.overpressure')
        file_handler_info = logging.FileHandler(logfile, mode='w')
        log.addHandler(file_handler_info)
    except Exception as exc:
        log = logging.getLogger('hyram.phys.overpressure').addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.INFO)

    log.info("OVERPRESSURE ANALYSIS")

    log.info(params)
    for param in sorted_params:
        log.info("{}: {}".format(param, params[param]))

    limit_line_pressures = c_utils.convert_to_numpy_array(limit_line_pressures)
    times = c_utils.convert_to_numpy_array(times)
    dot_mark_pressures = c_utils.convert_to_numpy_array(dot_mark_pressures)
    dot_mark_times = c_utils.convert_to_numpy_array(dot_mark_times)
    dot_mark_array = np.array([dot_mark_times, dot_mark_pressures]).T

    log.info("limit line pressures: {}".format(limit_line_pressures))
    log.info("times: {}".format(times))
    log.info("dot mark pressures: {}".format(dot_mark_pressures))
    log.info("dot mark times: {}".format(dot_mark_times))

    try:
        result_dict = overpressure.analyze_indoor_release(amb_pres, amb_temp, h2_pres,
                                                          h2_temp, tank_volume, orifice_diameter,
                                                          orifice_discharge_coeff,
                                                          release_discharge_coeff, release_area,
                                                          release_height, enclosure_height,
                                                          floor_ceiling_area, dist_release_to_wall,
                                                          ceiling_vent_xarea,
                                                          ceiling_vent_height_from_floor,
                                                          floor_vent_height_from_floor,
                                                          floor_vent_xarea, vol_flow_rate,
                                                          t=times, data=dot_mark_array, limit=limit_line_pressures,
                                                          max_sim_time=max_sim_time,
                                                          theta0=release_angle, output_dir=output_dir, verbose=False)
    except Exception as exc:
        log.info("Overpressure indoor release analysis failed with message {}".format(str(exc)))
        if debug:
            log.exception(exc)
        result_dict = {}

    gc.collect()
    return result_dict


def create_flame_temp_plot(amb_pressure, amb_temp, h2_pressure, h2_temp, orifice_diam, y0,
                           release_angle, nozzle_model_key,
                           output_dir=None, debug=False, verbose=False, filename='flame_temp_plot.png'):
    """ Create flame temperature plot. See flame.plot_temp for parameter descriptions. """
    params = locals()
    sorted_params = sorted(params)

    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    try:
        now = datetime.datetime.now()
        log_name = now.strftime('log-flame_%Y-%m-%d_%H-%M-%S.log')
        logfile = os.path.join(output_dir, log_name)
        log = logging.getLogger('hyram.phys.flame')
        file_handler_info = logging.FileHandler(logfile, mode='w')
        log.addHandler(file_handler_info)
    except Exception as exc:
        log = logging.getLogger('hyram.phys.flame').addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.INFO)

    log.info("FLAME TEMP PLOT GENERATION")
    for param in sorted_params:
        log.info("{}: {}".format(param, params[param]))

    chem_file = os.path.join(output_dir, "h2chem.pkl")
    log.info("Chem save file: {}".format(chem_file))

    try:
        result = flame.plot_temp(T_amb=amb_temp, P_amb=amb_pressure, T_H2=h2_temp, P_H2=h2_pressure,
                                 d_orifice=orifice_diam,
                                 y0=y0, theta0=release_angle, nnmodel=nozzle_model_key, output_dir=output_dir,
                                 fname=filename,
                                 chem_file=chem_file, verbose=verbose)

        log.info("Result: {}".format(result))
    except Exception as exc:
        log.info("Flame temp plot generation failed with message {}".format(str(exc)))
        if debug:
            log.exception(exc)
        result = ""

    gc.collect()
    return result


def analyze_radiative_heat_flux(amb_temp, amb_pressure, h2_temp, h2_pressure, orifice_diam, leak_height,
                                release_angle, notional_nozzle_model,
                                flux_x, flux_y, flux_z, rel_humid,
                                rad_source_model, contours,
                                output_dir=None, debug=False, verbose=True, plot_title=None):
    """ Analyze heat flux and generate corresponding plots. See flame.plot_flux for parameter descriptions. """
    params = locals()
    sorted_params = sorted(params)

    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    try:
        now = datetime.datetime.now()
        log_name = now.strftime('log-heat-flux_%Y-%m-%d_%H-%M-%S.log')
        logfile = os.path.join(output_dir, log_name)
        log = logging.getLogger('hyram.phys.flux')
        file_handler_info = logging.FileHandler(logfile, mode='w')
        log.addHandler(file_handler_info)
    except Exception as exc:
        log = logging.getLogger('hyram.phys.flux').addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.INFO)

    log.info("RADIATIVE HEAT FLUX ANALYSIS")
    for param in sorted_params:
        log.info("{}: {}".format(param, params[param]))

    flux_x = c_utils.convert_to_numpy_array(flux_x)
    flux_y = c_utils.convert_to_numpy_array(flux_y)
    flux_z = c_utils.convert_to_numpy_array(flux_z)
    contours = c_utils.convert_to_numpy_array(contours)

    log.info("Flux X: {}".format(flux_x))
    log.info("Flux Y: {}".format(flux_y))
    log.info("Flux Z: {}".format(flux_z))
    log.info("Contours: {}".format(contours))

    chem_file = os.path.join(output_dir, "h2chem.pkl")
    log.info("Chem save file: {}".format(chem_file))

    try:
        log.info("API call for heat flux...")
        flux, flux2d_filepath, temp_plot_filepath = flame.plot_flux(amb_temp, amb_pressure, h2_temp, h2_pressure,
                                                                    orifice_diam, leak_height,
                                                                    release_angle, notional_nozzle_model,
                                                                    flux_x, flux_y, flux_z, rel_humid=rel_humid,
                                                                    rad_source_model=rad_source_model,
                                                                    plot_title=plot_title,
                                                                    contours=contours, chem_file=chem_file,
                                                                    output_dir=output_dir, verbose=False)

        results = {
            'flux_data': flux,
            'flux_plot_filepath': flux2d_filepath,
            'temp_plot_filepath': temp_plot_filepath,
            'status': 1,
        }
        log.info("Result: {}".format(results))

    except Exception as exc:
        log.info("Radiative heat flux analysis failed with message {}".format(str(exc)))
        if debug:
            log.exception(exc)
        results = {}

    gc.collect()
    return results
