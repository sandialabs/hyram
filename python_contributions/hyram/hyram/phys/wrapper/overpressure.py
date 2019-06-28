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

from .._comps import Enclosure, Gas, Orifice, Source, Vent
from .._indoor_release import IndoorRelease
from .._therm import AbelNoble


def build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure,
                         H2_temperature, tank_volume, orifice_diameter,
                         orifice_discharge_coefficient, 
                         release_discharge_coefficient, release_area,
                         release_height, enclosure_height,
                         floor_ceiling_area, dist_release_to_wall,
                         ceiling_vent_cross_sectional_area,
                         ceiling_vent_height_from_floor,
                         floor_vent_height_from_floor,
                         floor_vent_cross_sectional_area,
                         ceiling_vent_discharge_coefficient,
                         floor_vent_discharge_coefficient, vol_flow_rate,
                         theta0=0, x0=0, y0=0, tmax=30, verbose=True):

    # Make the objects that are needed:
    ambient = Gas(AbelNoble(b=0, MW=28.97), T=ambient_temperature, P=ambient_pressure)
    H2 = Gas(AbelNoble(), T=H2_temperature, P=H2_pressure)
    source = Source(tank_volume, H2)
    orifice = Orifice(orifice_diameter, orifice_discharge_coefficient)
    
    ceiling_vent = Vent(ceiling_vent_cross_sectional_area,
                        ceiling_vent_height_from_floor,
                        ceiling_vent_discharge_coefficient, vol_flow_rate)
    floor_vent = Vent(floor_vent_cross_sectional_area,
                      floor_vent_height_from_floor, 
                      floor_vent_discharge_coefficient, vol_flow_rate)
    enclosure = Enclosure(enclosure_height, floor_ceiling_area, release_height,
                          ceiling_vent, floor_vent, dist_release_to_wall)

    if release_area is not None:
        release_area = release_discharge_coefficient * release_area
    
    IR = IndoorRelease(source, orifice, ambient, enclosure, tmax,
                        release_area, theta0, x0, y0, release_height,
                        lam=1.16, Xi_lean=0.04, tol=1e-8, n_pts_plume=50,
                        n_pts_layer=50, numplume=200, verbose=verbose)
    return IR


def analyze_indoor_release(ambient_pressure, amb_temp, h2_pressure,
                           h2_temp, tank_volume, orifice_diameter,
                           ori_discharge_coeff,
                           rel_discharge_coeff, release_area,
                           release_height, enclosure_height,
                           floor_ceiling_area, dist_release_to_wall,
                           ceiling_vent_cross_sectional_area,
                           ceiling_vent_height_from_floor,
                           floor_vent_height_from_floor,
                           floor_vent_cross_sectional_area, vol_flow_rate,
                           t, data, limit, max_sim_time, theta0,
                           ceiling_vent_discharge_coeff=0.61,
                           floor_vent_discharge_coeff=0.61,
                           x0=0, y0=0,
                           output_dir=None,
                           verbose=True):
    """
    Used to simulate an indoor release.

    NOTE(Cianan): polished version of execall_indoor_release for C#

    Parameters
    ----------
    ambient_pressure: float, ambient pressure (Pa)
    amb_temp: float, ambient temperature (K)
    h2_pressure: float, hydrogen pressure (Pa)
    h2_temp: float, hydrogen temperature (K)
    tank_volume: float, tank volume (m^3)
    orifice_diameter: float, orifice diameter (m)
    ori_discharge_coeff: float, orifice discharge coeffecient [unitless]
    rel_discharge_coeff: float, release discharge coefficient [unitless]
    release_area: float, area of release (m^2)
    release_height: float, height of release (m)
    enclosure_height: float, height of enclosure (m)
    floor_ceiling_area: float, area of floor and ceiling (m^2)
    dist_release_to_wall: float, distance between release and wall (m)
    ceiling_vent_cross_sectional_area: float, cross-sectional area of ceiling vent (m^2)
    ceiling_vent_height_from_floor: float, height from floor to middle of ceiling vent (m)
    floor_vent_height_from_floor: float, height from floor to middle of floor vent (m)
    floor_vent_cross_sectional_area: float, cross-sectional area of floor vent (m^2)
    ceiling_vent_discharge_coeff: float, discharge coefficient for ceiling vent: hard coded to 0.61 in IR call
    floor_vent_discharge_coeff: float, discharge coefficient for floor vent: hard coded to 0.61 in IR call
    vol_flow_rate: volumetric flow rate for vents (m^3/s)
    t: list, times at which to return the overpressure (s)
    data: 2xn array, list of [t, P], pairs to plot data points where pairs are in units of [s, kPa]
    limit: array, list of pressures at which to draw horizontal lines
    max_sim_time: float, maximum simulation time, must be greater than maximum value in t list (s)
    theta0: float, angle of release (0 is horizontal) (radians)

    Returns
    ----------
    result_dict : dict
    Parameters as follows:
        status: 1 if successful
        errors: list of error messages
        pressures_per_time: ndarray of pressure floats
        depths: ndarray of floats
        concentrations: ndarray
        overpressure: float max overpressure value
        time_of_overp: float
        pres_plot_filepath: string file path to pressure plot
        mass_plot_filepath: string file path to mass plot
        layer_plot_filepath: string file path to layer plot
        trajectory_plot_filepath: string file path to trajectory plot
    }

    """
    if output_dir is None:
        output_dir = os.getcwd()

    # Create temporary variables (avoid recomputing sqrt and brevity)
    sqrt_ceilvent_area = ceiling_vent_cross_sectional_area ** 0.5
    sqrt_floorvent_area = floor_vent_cross_sectional_area ** 0.5

    # TODO (Cianan): this should be updated to use standard error handling with message & status code
    errors = []
    if (enclosure_height - ceiling_vent_height_from_floor) < 0.5 * sqrt_ceilvent_area:
        ceiling_vent_height_from_floor = enclosure_height - 0.5 * sqrt_ceilvent_area
        errors.append("Vent1 has been moved to {}".format(ceiling_vent_height_from_floor))

    elif ceiling_vent_height_from_floor < 0.5 * sqrt_ceilvent_area:
        ceiling_vent_height_from_floor = 0.5 * sqrt_ceilvent_area
        errors.append("Vent1 has been moved to {}".format(ceiling_vent_height_from_floor))

    if floor_vent_height_from_floor < 0.5 * sqrt_floorvent_area:
        floor_vent_height_from_floor = 0.5 * sqrt_floorvent_area
        errors.append("Vent2 height too low; changed to {}".format(floor_vent_height_from_floor))

    elif (enclosure_height - floor_vent_height_from_floor) < 0.5 * sqrt_floorvent_area:
        floor_vent_height_from_floor = enclosure_height - 0.5 * sqrt_floorvent_area
        errors.append("Vent2 moved to {}".format(floor_vent_height_from_floor))

    indoor_rel = build_indoor_release(ambient_pressure, amb_temp, h2_pressure, h2_temp, tank_volume,
                                      orifice_diameter,
                                      ori_discharge_coeff,
                                      rel_discharge_coeff, release_area,
                                      release_height, enclosure_height, floor_ceiling_area, dist_release_to_wall,
                                      ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                                      floor_vent_height_from_floor,
                                      floor_vent_cross_sectional_area, ceiling_vent_discharge_coeff,
                                      floor_vent_discharge_coeff,
                                      vol_flow_rate, int(theta0), x0, y0, int(max_sim_time), verbose=verbose)

    pressure_t = indoor_rel.pressure(t)
    pres_plot_fpath = os.path.join(output_dir, 'pressure_plot.png')

    op = indoor_rel.plot_overpressure(data, limit)
    op.savefig(pres_plot_fpath, bbox_inches='tight')

    layer_depth_t = indoor_rel.layer_depth(t)
    layer_concentration_t = indoor_rel.concentration(t)
    layer_plot_fpath = os.path.join(output_dir, 'layer_plot.png')
    lh = indoor_rel.plot_layer()
    lh.savefig(layer_plot_fpath, bbox_inches='tight')

    traj_plot_fpath = os.path.join(output_dir, 'trajectory_plot.png')
    traj = indoor_rel.plot_trajectories()
    traj.savefig(traj_plot_fpath, bbox_inches='tight')

    mass_plot_fpath = os.path.join(output_dir, 'flam_mass_plot.png')
    fm = indoor_rel.plot_mass()
    fm.savefig(mass_plot_fpath, bbox_inches='tight')

    max_p_t = indoor_rel.max_p_t()

    result_dict = {
        'status': 1,
        'errors': errors,
        'pressures_per_time': pressure_t,
        'depths': layer_depth_t,
        'concentrations': layer_concentration_t,
        'overpressure': max_p_t[0],
        'time_of_overp': max_p_t[1],
        'pres_plot_filepath': pres_plot_fpath,
        'mass_plot_filepath': mass_plot_fpath,
        'layer_plot_filepath': layer_plot_fpath,
        'trajectory_plot_filepath': traj_plot_fpath,
    }

    if errors:
        raise ValueError("Parameter errors found: {}".format(str(errors)))

    return result_dict


def execall_indoor_release(ambient_pressure, ambient_temperature, H2_pressure,
                           H2_temperature, tank_volume, orifice_diameter,
                           orifice_discharge_coefficient,
                           release_discharge_coefficient, release_area,
                           release_height, enclosure_height,
                           floor_ceiling_area, dist_release_to_wall,
                           ceiling_vent_cross_sectional_area,
                           ceiling_vent_height_from_floor,
                           floor_vent_height_from_floor,
                           floor_vent_cross_sectional_area, vol_flow_rate,
                           t, data, limit, max_sim_time, theta0,
                           ceiling_vent_discharge_coefficient=0.61,
                           floor_vent_discharge_coefficient=0.61, x0=0, y0=0,
                           verbose=True):
    '''
    Used to simulate an indoor release

    Parameters
    ----------
    ambient_pressure: float, ambient pressure (Pa)
    ambient_temperature: float, ambient temperature (K)
    H2_pressure: float, hydrogen pressure (Pa)
    H2_temperature: float, hydrogen temperature (K)
    tank_volume: float, tank volume (m^3)
    orifice_diameter: float, orifice diameter (m)
    orifice_discharge_coeffecient: float, orifice discharge coeffecient [unitless]
    release_discharge_coefficient: float, release discharge coefficient [unitless]
    release_area: float, area of release (m^2)
    release_height: float, height of release (m)
    enclosure_height: float, height of enclosure (m)
    floor_ceiling_area: float, area of floor and ceiling (m^2)
    dist_release_to_wall: float, distance between release and wall (m)
    ceiling_vent_cross_sectional_area: float, cross-sectional area of ceiling vent (m^2)
    ceiling_vent_height_from_floor: float, height from floor to middle of ceiling vent (m)
    floor_vent_height_from_floor: float, height from floor to middle of floor vent (m)
    floor_vent_cross_sectional_area: float, cross-sectional area of floor vent (m^2)
    ceiling_vent_discharge_coefficient: float, discharge coefficient for ceiling vent: hard coded to 0.61 in IR call
    floor_vent_discharge_coefficient: float, discharge coefficient for floor vent: hard coded to 0.61 in IR call
    wind_velocity: float, wind velocity (m/s) = now vol flow rate
    vol_flow_rate: volumetric flow rate for vents (m^3/s)
    t: list, times at which to return the overpressre (s)
    data: 2xn array, list of [t, P], pairs to plot data points where pairs are in units of [s, Pa]
    limit: array, list of pressures at which to draw horizontal lines
    max_sim_time: float, maximum simulation time, must be greater than maximum value in t list (s)
    theta0: float, angle of release (0 is horizontal) (radians)
    '''

    # Create temporary variables (avoid recomputing sqrt and brevity)
    sqrt_ceilvent_area = ceiling_vent_cross_sectional_area**0.5
    sqrt_floorvent_area = floor_vent_cross_sectional_area**0.5

    result = []
    if((enclosure_height - ceiling_vent_height_from_floor) < 0.5*sqrt_ceilvent_area):
        result.append([["error_vent1"],
                       ["Vent1 has been moved to %s" % str(enclosure_height - 0.5*sqrt_ceilvent_area)] ])
        ceiling_vent_height_from_floor = enclosure_height - 0.5*sqrt_ceilvent_area
    elif(ceiling_vent_height_from_floor < 0.5*sqrt_ceilvent_area ):
        result.append([["error_vent1"],
                       ["Vent1 has been moved to %s" % str(0.5*sqrt_ceilvent_area)]])
        ceiling_vent_height_from_floor =  0.5*sqrt_ceilvent_area
    else:
        result.append([["error_vent1"],["None"]])
    
    if(floor_vent_height_from_floor < 0.5*sqrt_floorvent_area):
        result.append([["error_vent2"],
                       ["Vent2 height value too low. It was changed to %s" % str(0.5*sqrt_floorvent_area)] ])
        floor_vent_height_from_floor =  0.5*sqrt_floorvent_area
    elif((enclosure_height - floor_vent_height_from_floor) < 0.5*sqrt_floorvent_area):
        result.append([["error_vent2"],
                       ["Vent2 has been moved to %s" % str(enclosure_height - 0.5*sqrt_floorvent_area)] ])
        floor_vent_height_from_floor = enclosure_height - 0.5*sqrt_floorvent_area
    else:
        result.append([["error_vent2"],["None"]])

    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient, 
                             release_discharge_coefficient, release_area, 
                             release_height, enclosure_height, floor_ceiling_area, dist_release_to_wall,
                             ceiling_vent_cross_sectional_area,ceiling_vent_height_from_floor, floor_vent_height_from_floor,
                             floor_vent_cross_sectional_area, ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient,
                             vol_flow_rate, theta0, x0, y0, max_sim_time, verbose=verbose)
    pressure_t = IR.pressure(t)
    directory = os.getcwd()
    fname = os.path.join(directory, 'pressure_plot.png')
    
    op = IR.plot_overpressure(data, limit)
    op.savefig(fname, bbox_inches='tight')

    layer_depth_t = IR.layer_depth(t)
    layer_concentration_t = IR.concentration(t)
    lh_fname = os.path.join(directory, 'layer_plot.png')
    lh = IR.plot_layer()
    lh.savefig(lh_fname, bbox_inches='tight')
    
    traj_fname = os.path.join(directory, 'trajectory_plot.png')
    traj = IR.plot_trajectories()
    traj.savefig(traj_fname, bbox_inches='tight')
    
    fm_fname = os.path.join(directory, 'flam_mass_plot.png')
    fm = IR.plot_mass()
    fm.savefig(fm_fname, bbox_inches='tight')

    
    max_p_t = IR.max_p_t()

    result.append([["time_steps"],[t]])
    result.append([["max_p_t"], [max_p_t]])
    result.append([["pressure_t"], [pressure_t]])
    result.append([["pressure_image_filename"], [fname]])
    result.append([["layer_depth_t"], [layer_depth_t]])
    result.append([["layer_concentration_t"], [layer_concentration_t]])
    result.append([["layer_image_filename"], [lh_fname]])
    result.append([["trajectory_image_filename"], [traj_fname]])   
    result.append([["flammable_mass_image_filename"], [fm_fname]])
    return result

def exec_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                        orifice_discharge_coefficient, 
                        release_discharge_coefficient, release_blockage_area, release_area,
                        release_height, enclosure_height,
                        floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                        floor_vent_height_from_floor, floor_vent_cross_sectional_area, ceiling_vent_discharge_coefficient,
                        floor_vent_discharge_coefficient, vol_flow_rate,
                        theta0 = 0, x0 = 0, y0 = 0, tmax = 30):
    '''
    returns the maximum overpressure (Pa) and time when overpressure occurred (seconds)
    '''
    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient, 
                             release_discharge_coefficient, release_blockage_area, release_area,
                             release_height, enclosure_height,
                             floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                             floor_vent_height_from_floor, floor_vent_cross_sectional_area, 
                             ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate, theta0, x0, y0, tmax)
    T = IR.max_p_t()
    return T
    
def indoor_release_P_time(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                          orifice_discharge_coefficient, 
                          release_discharge_coefficient, release_blockage_area, release_area,
                          release_height, enclosure_height,
                          floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                          floor_vent_height_from_floor, floor_vent_cross_sectional_area, ceiling_vent_discharge_coefficient,
                          floor_vent_discharge_coefficient, vol_flow_rate,
                          t, theta0 = 0, x0 = 0, y0 = 0, tmax = 30):
    '''
    returns a(n array) of pressure(s) at time(s) t (t in seconds).
    '''
    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient, 
                             release_discharge_coefficient, release_blockage_area, release_area,
                             release_height, enclosure_height,
                             floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                             floor_vent_height_from_floor, floor_vent_cross_sectional_area, 
                             ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate, theta0, x0, y0, tmax)
    return IR.pressure(t)

def indoor_release_P_time_plot(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                               orifice_discharge_coefficient, release_discharge_coefficient, release_blockage_area, release_area,
                               release_height, enclosure_height,
                               floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                               floor_vent_height_from_floor, floor_vent_cross_sectional_area, ceiling_vent_discharge_coefficient,
                               floor_vent_discharge_coefficient, vol_flow_rate,
                               directory = os.getcwd(), theta0 = 0, x0 = 0, y0 = 0, tmax = 30, data = None, limit = None):
    '''
    returns a plot of the overpressure as a function of ignition delay
    Parameters
    ----------
    data: list of [time, pressure] pairs to add to plot
    limit: list of pressures at which to to add horizontal line (overpressure limit) 
    '''
    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient, 
                             release_discharge_coefficient, release_blockage_area, release_area,
                             release_height, enclosure_height,
                             floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                             floor_vent_height_from_floor, floor_vent_cross_sectional_area, 
                             ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate, theta0, x0, y0, tmax)
    
    fname = os.path.join(directory, 'pressure_plot.png')
    fig = IR.plot_overpressure(data, limit)
    fig.savefig(fname, bbox_inches='tight')
    return fname
    
def indoor_release_flam_mass_plot(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                                  orifice_discharge_coefficient, 
                                  release_discharge_coefficient, release_blockage_area, release_area,
                                  release_height, enclosure_height,
                                  floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area,
                                  ceiling_vent_height_from_floor, floor_vent_height_from_floor, floor_vent_cross_sectional_area,
                                  ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate,
                                  directory = os.getcwd(), theta0 = 0, x0 = 0, y0 = 0, tmax = 30):
    '''
    returns a plot of the flammable hydrogen mass as a function of time
    '''
    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient, 
                             release_discharge_coefficient, release_blockage_area, release_area,
                             release_height, enclosure_height,
                             floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                             floor_vent_height_from_floor, floor_vent_cross_sectional_area, 
                             ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate, theta0, x0, y0, tmax)
    
    fname = os.path.join(directory, 'flam_mass.png')
    fig = IR.plot_mass()
    fig.savefig(fname, bbox_inches='tight')
    return fname

def indoor_release_mass_flow_plot(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                                  orifice_discharge_coefficient, 
                                  release_discharge_coefficient, release_blockage_area, release_area,
                                  release_height, enclosure_height,
                                  floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area,
                                  ceiling_vent_height_from_floor, floor_vent_height_from_floor, floor_vent_cross_sectional_area,
                                  ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate,
                                  directory = os.getcwd(), theta0 = 0, x0 = 0, y0 = 0, tmax = 30):
    '''
    returns a plot of the mass flow rate of hydrogen as a function of time
    '''
    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient, 
                             release_discharge_coefficient, release_blockage_area, release_area,
                             release_height, enclosure_height,
                             floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                             floor_vent_height_from_floor, floor_vent_cross_sectional_area, 
                             ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate, theta0, x0, y0, tmax)
    
    fname = os.path.join(directory, 'mass_flow.png')
    fig = IR.plot_mass_flows()
    fig.savefig(fname, bbox_inches='tight')
    return fname
    
def indoor_release_layer_plot(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                              orifice_discharge_coefficient, 
                              release_discharge_coefficient, release_blockage_area, release_area,
                              release_height, enclosure_height,
                              floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                              floor_vent_height_from_floor, floor_vent_cross_sectional_area, ceiling_vent_discharge_coefficient,
                              floor_vent_discharge_coefficient, vol_flow_rate,
                              directory = os.getcwd(), theta0 = 0, x0 = 0, y0 = 0, tmax = 30):
    '''
    returns a plot of the layer height and mole fraction in the layer as a function of time
    '''
    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient,
                             release_discharge_coefficient, release_blockage_area, release_area,
                             release_height, enclosure_height,
                             floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                             floor_vent_height_from_floor, floor_vent_cross_sectional_area,
                             ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate, theta0, x0, y0, tmax)

    fname = os.path.join(directory, 'layer_plot.png')
    fig = IR.plot_layer()
    fig.savefig(fname, bbox_inches='tight')
    return fname

def indoor_release_trajectory_plot(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                                   orifice_discharge_coefficient, 
                                   release_discharge_coefficient, release_blockage_area, release_area,
                                   release_height, enclosure_height,
                                   floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area,
                                   ceiling_vent_height_from_floor, floor_vent_height_from_floor, floor_vent_cross_sectional_area,
                                   ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate,
                                   directory = os.getcwd(), theta0 = 0, x0 = 0, y0 = 0, tmax = 30):
    '''
    returns a plot of the release trajectories over time
    '''
    IR = build_indoor_release(ambient_pressure, ambient_temperature, H2_pressure, H2_temperature, tank_volume, orifice_diameter,
                             orifice_discharge_coefficient, 
                             release_discharge_coefficient, release_blockage_area, release_area,
                             release_height, enclosure_height,
                             floor_ceiling_area, dist_release_to_wall, ceiling_vent_cross_sectional_area, ceiling_vent_height_from_floor,
                             floor_vent_height_from_floor, floor_vent_cross_sectional_area, 
                             ceiling_vent_discharge_coefficient, floor_vent_discharge_coefficient, vol_flow_rate, theta0, x0, y0, tmax)
    fname = os.path.join(directory, 'traj_plot.png')
    fig = IR.plot_trajectories()
    fig.savefig(fname, bbox_inches='tight')
    return fname
