"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
import os

import matplotlib.pyplot as plt
import numpy as np

from . import Fluid, Orifice, Source, Vent, Enclosure, Flame, Jet, NozzleFlow
from . import IndoorRelease, FuelProperties, Bauwens_method, TNT_method, BST_method
from ._plots import plot_contour
from ..utilities import misc_utils

"""
The API file provides external access to common analysis functions within the physics module.
API functions prep logging, clean up parameters, and return results or error notifications.

Notes:
    * Logging should be set up by the user, not the app. The only exception to this is the GUI, which sets up logging
    for the GUI requests.
"""

log = logging.getLogger(__name__)


def create_fluid(species, temp=None, pres=None, density=None, phase=None):
    """
    Create fluid from given parameters. Exactly two of temperature, pressure, density, phase must be provided.

    Parameters
    ----------
    species : str or dict
        Name of fluid or dict of blend mixture of format {fluid-name: mole-frac...}

    pres : float or None
        Fluid pressure (Pa).

    temp : float or None
        Fluid temperature (K).

    density : float or None
        Fluid density (kg/m3)

    phase : str or None
        Fluid phase; values from CoolProp spec. If none, Coolprop determines it.
        GUI passes in {'none', 'gas', 'liquid'} where:
            * 'gas' - saturated vapor.
            * 'liquid' - saturated liquid.
            * 'none' or any other string represents gas phase and will be converted to None.

    Returns
    -------
    Fluid object described by provided parameters

    """
    # phase parameter determines whether temperature is used, but temp may still be passed so must clear it.
    phase = misc_utils.parse_phase_key(phase)
    if phase is not None:
        temp = None

    # User should pass only two specifiers for each fluid, after parsing
    num_params = len([x for x in [temp, pres, density, phase] if x is not None])
    if not num_params == 2:
        message = ('Fluid must be defined by exactly two of the following parameters:'
                   ' temperature, pressure, density, phase')
        raise ValueError(message)

    fluid = Fluid(species=species, T=temp, P=pres, rho=density, phase=phase)
    return fluid

def is_choked(fluid, amb_pres=101325):
    """
    Determines whether a flow is choked as a fluid is released into a lower-pressure environment.
    
    Parameters
    ----------
    fluid : Fluid
        Release fluid object

    amb_pres : float, optional
        Ambient fluid pressure (Pa).
        
    Returns
    -------
    is_choked : boolean
        Whether flow is choked.
    """
    if fluid.P == amb_pres:
        return False
    else:
        return NozzleFlow(fluid, Orifice(0.001), amb_pres).choked

def compute_mass_flow(fluid, orif_diam, amb_pres=101325,
                      is_steady=True, tank_vol=None, dis_coeff=1, output_dir=None, create_plot=True):
    """
    Calculate mass flow rate based on given conditions.

    Parameters
    ----------
    fluid : Fluid
        Release fluid object

    orif_diam : float
        Orifice diameter (m).

    amb_pres : float, optional
        Ambient fluid pressure (Pa).

    is_steady : bool
        Whether system is at steady-state.

    tank_vol : float
        Volume of source (tank) (m^3).

    dis_coeff : float
        Discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow).

    output_dir : str or None
        Path to directory in which to place output file(s).
        If unspecified, output will be saved to the current working directory.

    create_plot : bool
        Whether mass flow vs. time plot should be created

    Returns
    ----------
    result : dict
        If is_steady, only mass flow rate will be returned as single value.
        If is_steady is false, all parameters will be returned and mass_flow_rate will be list of values.
        rates : list of floats
            Mass flow rate(s) (kg/s)
        time_to_empty : float
            Time (s) it takes to blowdown the tank to empty.
        plot : str
            Path to plot of mass flow rate vs. time. Only created if Steady is false.
        times : array of floats
            Times at which mass flow rates occur during blowdown.

    """
    log.info("Mass Flow analysis requested")

    result = {"time_to_empty": None, "plot": "", "times": None, "rates": None}

    orif = Orifice(orif_diam, Cd=dis_coeff)

    if is_steady:
        result['rates'] = [NozzleFlow(fluid, orif, amb_pres).mdot]

    else:
        source = Source(tank_vol, fluid)
        mdots, fluid_list, t, sol = source.empty(orif, amb_pres)

        if create_plot:
            output_folder = misc_utils.get_output_folder(output_dir)
            filename = "time-to-empty-{}.png".format(misc_utils.get_now_str())
            filepath = os.path.join(output_folder, filename)
            fig = source.plot_time_to_empty()
            fig.savefig(filepath, bbox_inches='tight')
            plt.close(fig)
            result['plot'] = filepath

        result["time_to_empty"] = t[-1]
        result["times"] = t
        result["rates"] = mdots

    log.info("Mass Flow analysis complete")
    return result


def compute_tank_mass_param(species, phase=None, temp=None, pres=None, vol=None, mass=None):
    """
    Tank mass calculation.
    Three of temp (or phase), pressure, volume, or mass are required.

    species : str
        Fluid species formula or name
        (see CoolProp documentation)

    phase : None or str
        CoolProp specifier for phase
        ('liquid' for saturated liquid,
        'gas' for saturated vapor,
        or None to specify temperature and pressure)

    temp : float or None
        Fluid temperature (K)

    pres : float or None
        Fluid absolute pressure (Pa)

    vol : float or None
        Fluid volume (m^3)

    mass : float or None
        Fluid mass (kg)

    Returns
    -------
    result1 : float
        Fluid temperature, pressure, volume, or mass, depending on provided input parameters.
        If saturated phase, first parameter is pressure, volume, or mass.

    result2 : float or None
        If unsaturated, None returned.
        If saturated phase, this will be temperature.
    """
    log.info("Tank Mass calculation requested")

    phase = misc_utils.parse_phase_key(phase)
    if (temp is not None or phase is not None) and pres is not None and vol is not None and mass is not None:
        msg = 'Too many inputs provided - three of [temperature (or phase), pressure, volume, mass] required'
        raise ValueError(msg)

    if vol is not None and mass is not None:
        density = mass / vol  # kg/m3
    else:
        density = None

    fluid = create_fluid(species, temp, pres, density, phase)

    if density is None:
        if vol is None:
            result1 = mass / fluid.rho
        else:
            result1 = vol * fluid.rho
    elif pres is None:
        result1 = fluid.P
    else:
        result1 = fluid.T

    if phase is not None:  # saturated phase
        result2 = fluid.T
    else:
        result2 = None

    log.info("Tank Mass calculation complete")
    return result1, result2


def compute_thermo_param(species, phase=None, temp=None, pres=None, density=None):
    """
    Calculates temperature, pressure or density of species.
    Requires two of [pressure, density, and (temperature or phase)]
    If unsaturated phase, this returns whichever parameter was not provided.
    If saturated phase, this returns temperature and whichever other parameter was not provided.

    Parameters
    ----------
    species : str
        Fluid species formula or name
        (see CoolProp documentation)

    phase : None or str
        CoolProp specifier for phase
        ('liquid' for saturated liquid,
        'gas' for saturated vapor,
        or None to specify temperature and pressure)

    temp : float or None
        Fluid temperature (K)

    pres : float or None
        Fluid pressure (Pa)

    density : float or None
        Fluid density (kg/m^3)

    Returns
    -------
    float
        Fluid temperature, pressure, or density, depending on provided input parameters.
        If saturated phase, first parameter is either density or pressure.

    float or None
        If unsaturated, None returned.
        If saturated phase, this will be temperature.
    """
    log.info("TPD Parameter calculation requested")

    phase = misc_utils.parse_phase_key(phase)
    if pres is not None and density is not None and (temp is not None or phase is not None):
        msg = 'Too many inputs provided - two of [temperature (or phase), pressure, density] are required'
        raise ValueError(msg)

    fluid = create_fluid(species, temp, pres, density, phase)

    result1 = None
    result2 = None

    if phase is not None:
        # saturated phase
        result1 = fluid.rho if density is None else fluid.P
        result2 = fluid.T

    else:
        if density is None:
            result1 = fluid.rho
        elif pres is None:
            result1 = fluid.P
        else:
            result1 = fluid.T

    log.info("TPD Parameter calculation complete")
    return result1, result2


def compute_equivalent_tnt_mass(vapor_mass, percent_yield, species):
    """
    Calculate equivalent mass of TNT.

    Parameters
    ----------
    vapor_mass : float
        Mass of flammable vapor released (kg)

    percent_yield : float
        Explosive energy yield (0 to 100%)

    species : str or dict
        Name of fluid or dict of blend mixture of format {fluid-name: mole-frac...}

    Returns
    ----------
    equiv_TNT_mass : float
        Equivalent mass of TNT (kg)
    """
    if type(species) == dict:
        parsed_dict = misc_utils.parse_blend_dict_into_coolprop_dict(species)
        species = misc_utils.parse_coolprop_dict_into_string(parsed_dict)

    heat_of_combustion = FuelProperties(species).dHc  # J/kg
    log.info(f"TNT mass calculation: vapor mass {vapor_mass:,.4f} kg, yield {percent_yield:,.1f}%, heat of combustion {heat_of_combustion:,.5f} J/kg")
    yield_fraction = percent_yield / 100
    result = TNT_method.calc_TNT_equiv_mass(equivalence_factor=yield_fraction,
                                            flammable_mass=vapor_mass,
                                            heat_of_combustion=heat_of_combustion)
    log.info("TNT mass calculation complete")
    return result


def analyze_jet_plume(amb_fluid, rel_fluid, orif_diam, mass_flow=None,
                      rel_angle=(np.pi/2), dis_coeff=1, nozzle_model='yuce',
                      create_plot=True, contours=None,
                      xmin=None, xmax=None, ymin=None, ymax=None, vmin=0, vmax=0.1,
                      plot_title="Mole Fraction of Leak",
                      filename=None, output_dir=None, verbose=False):
    """
    Simulate jet plume for leak and generate plume positional data, including mass and mole fractions, plume plot.

    Parameters
    ----------
    amb_fluid : Fluid
        Ambient fluid object

    rel_fluid : Fluid
        Release fluid object

    orif_diam : float
        Diameter of orifice (m).

    mass_flow : float or None
        fluid flow rate when unchoked [kg/s]

    rel_angle : float
        Angle of release (radian). 0 is horizontal, pi/2 is vertical.

    dis_coeff : float
        Release discharge coefficient (unitless).

    nozzle_model: {'yuce', 'hars', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model id. Will be parsed to ensure str matches available options.

    create_plot : bool
        Whether mole fraction contour plot should be created

    contours : list or int or float
        Define contour lines
        Default is None: will use default LFL for selected fuel

    xmin : float, optional
        Plot x-axis minimum.

    xmax : float, optional
        Plot x-axis maximum.

    ymin : float, optional
        Plot y-axis minimum.

    ymax : float, optional
        Plot y-axis maximum.

    vmin : float, optional
        Molar fraction (color bar) scale minimum

    vmax : float, optional
        Molar fraction (color bar) scale maximum

    plot_title : str
        Title displayed in output plot.

    filename : str or None
        Plot filename, excluding path.

    output_dir : str or None
        Directory in which to place plot file.
        If unspecified, output will be saved to the current working directory.

    verbose : bool, False

    Returns
    -------
    result_dict : dict
        plot : str
            plot file path
        xs : 2D ndarray
            horizontal positions (m) along jet plume
        ys : 2D ndarray
            vertical positions (m) along jet plume
        mole_fracs : 2D ndarray
            mole fractions along jet plume
        mass_fracs : 2D ndarray
            mass fractions along jet plume
        vs : 2D ndarray
            velocities along jet plume
        temps : 2D ndarray
            temperatures (K) along jet plume
        mass_flow_rate : float
            Mass flow rate (kg/s) of steady release.
        streamline_dists: [float]
            Distance(s) of streamline for each contour
        mole_frac_dists: dict
            x and y min-max values for each contour. {contour (float): [(x1, x2), (y1, y2) }
    """
    log.info("Plume plot requested")
    if contours is None:
        fuel_props = FuelProperties(rel_fluid.species)
        contours = [fuel_props.LFL]

    contours = [contours] if type(contours) in [int, float] else contours
    contours = np.sort(contours)

    log.info('Creating components')
    orifice = Orifice(orif_diam, dis_coeff)

    # Jet requires parameters defining notional nozzle model, rather than model itself.
    nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, rel_fluid)

    log.info('Creating jet')
    jet_obj = Jet(rel_fluid, orifice, amb_fluid, theta0=rel_angle, mdot=mass_flow,
                  nn_conserve_momentum=nozzle_cons_momentum,
                  nn_T=nozzle_t_param, verbose=verbose)

    xs, ys, mole_fracs, mass_fracs, vs, temps = jet_obj.get_contour_data()
    mass_flow_rate = jet_obj.mass_flow_rate
    streamline_dists = jet_obj.get_streamline_distances_to_mole_fractions(contours)
    mole_frac_dists = jet_obj.get_xy_distances_to_mole_fractions(contours)

    if create_plot:
        log.info("Creating mole fraction contour plot")
        output_folder = misc_utils.get_output_folder(output_dir)

        if filename is None:
            filename = "plume-mole-plot-{}.png".format(misc_utils.get_now_str())

        if xmin is not None and xmax is not None:
            if xmin >= xmax:
                raise ValueError('x minimum must be less than x maximum')
            xlims = (xmin, xmax)
        else:
            xlims = None

        if ymin is not None and ymax is not None:
            if ymin >= ymax:
                raise ValueError('y minimum must be less than y maximum')
            ylims = (ymin, ymax)
        else:
            ylims = None

        if vmin is None:
            vmin = 0
        if vmax is None:
            vmax = 0.1

        plot_filepath = os.path.join(output_folder, filename)
        plot_fig = plot_contour(data_type="mole", jet_or_flame=jet_obj,
                                xlims=xlims, ylims=ylims, vlims=(vmin, vmax),
                                plot_title=plot_title, contour_levels=contours,)
        plot_fig.savefig(plot_filepath, bbox_inches='tight')
        plt.close(plot_fig)
        log.info("Plume plot complete")
    else:
        plot_filepath = ''

    result_dict = {'xs': xs,
                   'ys': ys,
                   'mole_fracs': mole_fracs,
                   'mass_fracs': mass_fracs,
                   'vs': vs,
                   'temps': temps,
                   'plot': plot_filepath,
                   'mass_flow_rate': mass_flow_rate,
                   'streamline_dists': streamline_dists,
                   'mole_frac_dists': mole_frac_dists}
    return result_dict


def analyze_accumulation(amb_fluid, rel_fluid,
                         tank_volume, orif_diam, rel_height,
                         enclos_height, floor_ceil_area,
                         ceil_vent_xarea, ceil_vent_height,
                         floor_vent_xarea, floor_vent_height,
                         times, orif_dis_coeff=1, ceil_vent_coeff=1, floor_vent_coeff=1,
                         vol_flow_rate=0, dist_rel_to_wall=np.inf,
                         tmax=None, rel_area=None, rel_angle=0, nozzle_key='yuce',
                         x0=0, y0=0, nmax=1000,
                         temp_pres_points=None, pres_ticks=None, is_steady=False,
                         create_plots=True, output_dir=None, verbose=False):
    """
    Simulate an indoor release of designated fluid.

    Parameters
    ----------
    amb_fluid : Fluid
        Ambient fluid object

    rel_fluid : Fluid
        Release fluid object

    tank_volume : float
        Tank volume (m^3).

    orif_diam : float
        Orifice diameter (m).

    rel_height : float
        Height of release (m) above floor at 0.

    enclos_height : float
        Height of enclosure (m).

    floor_ceil_area : float
        Area of floor and ceiling (m^2).

    ceil_vent_xarea : float
        Cross-sectional area of ceiling vent (m^2).

    ceil_vent_height: float
        Height from floor to middle of ceiling vent (m).

    floor_vent_xarea : float
        Cross-sectional area of floor vent (m^2).

    floor_vent_height: float
        Height from floor to middle of floor vent (m).

    times : list or ndarray
        Times at which to return the overpressure (s).

    orif_dis_coeff : float
        Orifice discharge coefficient [unitless].

    ceil_vent_coeff : float
        Discharge coefficient for ceiling vent.

    floor_vent_coeff : float
        Discharge coefficient for floor vent.

    vol_flow_rate : float
        Volumetric flow rate for vents (m^3/s).

    dist_rel_to_wall : float
        Distance between release and wall (m).

    tmax : float, optional
        Maximum simulation time, must be greater than maximum value in t list (s).

    rel_area : float, optional
        Area of release (m^2).

    rel_angle : float
        Angle of release (0 is horizontal) (radians).

    nozzle_key : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    x0 : float (optional)
        x-coordinate of release (m), default is 0

    y0 : float (optional)
        y-coordinate of release (m), default is 0

    nmax: int, optional
        maximum number of iterations for blowdown integration

    temp_pres_points : 2xn array, optional
        List of [t, P], pairs to plot data points where pairs are in units of [s, kPa].

    pres_ticks : ndarray, optional
        List of pressures at which to draw horizontal lines.

    create_plots : bool
        Whether plots should be generated. If false, plot path params will contain ''.

    output_dir : str or None
        Filepath of output directory in which to place plots.
        If unspecified, output will be saved to the current working directory.

    verbose : bool
        Verbosity of logging and print statements.

    Returns
    ----------
    result_dict : dict
        status : bool
            Set to 1 if successful
        pressures_per_time : ndarray of floats
            Pressure values (Pa) per time step.
        depths : ndarray of floats
             Depth locations (m) at which concentrations determined.
        concentrations: ndarray
            Fluid concentrations (%) at specified depths.
        overpressure: float
            Max overpressure (Pa) value.
        time_of_overp: float
            Time (s) at which max over-pressure occurred.
        pres_plot_filepath: str
            file path to pressure plot
        mass_plot_filepath: str
            file path to mass plot
        layer_plot_filepath: str
            file path to layer plot
        trajectory_plot_filepath: str
            file path to trajectory plot
        mass_flow_plot_filepath
            file path to mass flow rate plot

    """
    log.info("Accumulation analysis requested")
    if isinstance(times, list):
        times = np.array(times)

    log.info('Creating components')
    rel_source = Source(tank_volume, rel_fluid)
    orifice = Orifice(orif_diam, orif_dis_coeff)

    conserve_momentum, notional_nozzle_t = misc_utils.convert_nozzle_model_to_params(nozzle_key, rel_fluid)
    ceil_vent = Vent(ceil_vent_xarea, ceil_vent_height, ceil_vent_coeff, vol_flow_rate)
    floor_vent = Vent(floor_vent_xarea, floor_vent_height, floor_vent_coeff, vol_flow_rate)
    enclosure = Enclosure(enclos_height, floor_ceil_area, rel_height, ceil_vent, floor_vent,
                          Xwall=dist_rel_to_wall)

    release_obj = IndoorRelease(rel_source, orifice, amb_fluid, enclosure,
                                tmax=tmax, release_area=rel_area, steady=is_steady,
                                nn_conserve_momentum=conserve_momentum, nn_T=notional_nozzle_t,
                                theta0=rel_angle, x0=x0, y0=y0, nmax=nmax, verbose=verbose)

    # interpolate mass flow rates for given times
    rates = np.interp(times, release_obj.ts, release_obj.mdots)

    # Generate plots
    if create_plots:
        output_folder = misc_utils.get_output_folder(output_dir)

        now_str = misc_utils.get_now_str()
        pres_plot_fpath = os.path.join(output_folder, 'pressure_plot_{}.png'.format(now_str))
        layer_plot_fpath = os.path.join(output_folder, 'layer_plot_{}.png'.format(now_str))
        traj_plot_fpath = os.path.join(output_folder, 'trajectory_plot_{}.png'.format(now_str))
        mass_plot_fpath = os.path.join(output_folder, 'flam_mass_plot_{}.png'.format(now_str))
        mass_flow_plot_fpath = os.path.join(output_folder, 'time-to-empty_{}.png'.format(now_str))

        pfig = release_obj.plot_overpressure(temp_pres_points, pres_ticks)
        pfig.savefig(pres_plot_fpath, bbox_inches='tight')
        plt.close(pfig)

        lfig = release_obj.plot_layer()
        lfig.savefig(layer_plot_fpath, bbox_inches='tight')
        plt.close(lfig)

        tfig = release_obj.plot_trajectories()
        tfig.savefig(traj_plot_fpath, bbox_inches='tight')
        plt.close(tfig)

        mfig = release_obj.plot_mass()
        mfig.savefig(mass_plot_fpath, bbox_inches='tight')
        plt.close(mfig)

        if is_steady:
            mass_flow_plot_fpath = ''
        else:
            mdotfig = rel_source.plot_time_to_empty()
            mdotfig.savefig(mass_flow_plot_fpath, bbox_inches = 'tight')
            plt.close(mdotfig)

    else:
        pres_plot_fpath = ''
        layer_plot_fpath = ''
        traj_plot_fpath = ''
        mass_plot_fpath = ''
        mass_flow_plot_fpath = ''

    max_pres, max_time = release_obj.max_p_t()

    result_dict = {
        'status': 1,
        'pressures_per_time': release_obj.pressure(times),
        'depths': release_obj.layer_depth(times),
        'concentrations': release_obj.concentration(times),
        'overpressure': max_pres,
        'time_of_overp': max_time,
        'mass_flow_rates': rates,
        'pres_plot_filepath': pres_plot_fpath,
        'mass_plot_filepath': mass_plot_fpath,
        'layer_plot_filepath': layer_plot_fpath,
        'trajectory_plot_filepath': traj_plot_fpath,
        'mass_flow_plot_filepath': mass_flow_plot_fpath
    }

    log.info("Accumulation analysis complete")
    return result_dict


def jet_flame_analysis(amb_fluid, rel_fluid, orif_diam, mass_flow=None, dis_coeff=1,
                       rel_angle=0,
                       nozzle_key='yuce', rel_humid=0.89,
                       # temperature plot
                       create_temp_plot=True, temp_plot_filename=None, temp_plot_title="", temp_contours=None,
                       temp_xlims=None, temp_ylims=None,
                       # heat flux plot
                       analyze_flux=True, create_flux_plot=True, flux_plot_filename=None,
                       flux_coordinates=None, flux_contours=None,
                       flux_xlims=None, flux_ylims=None, flux_zlims=None,
                       output_dir=None, verbose=False):
    """
    Assess jet flame behavior and flux data and create corresponding plots.

    Parameters
    ----------
    amb_fluid : Fluid
        Ambient fluid object

    rel_fluid : Fluid
        Release fluid object

    orif_diam : float
        Orifice diameter (m)

    mass_flow : float or None
        fluid flow rate when unchoked [kg/s]

    dis_coeff : float
        Orifice discharge coeffecient [unitless]

    rel_angle : float
        Angle of release (0 is horizontal) (radians)

    nozzle_key : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    rel_humid : float
        Relative humidity between 0 and 1

    create_temp_plot : bool, True
        Whether temperature plot should be created

    temp_plot_filename : str or None
        Desired filename of output temp plot file

    temp_plot_title : str
        Title to display in plot

    temp_contours : array-like or None
        temperatures at which to draw contour lines

    temp_xlims : tuple/list or None, optional
        Temperature plot x limits: (x_min, x_max). Default is None to use automatic limits.

    temp_ylims : tuple/list or None, optional
        Temperature plot y limits: (y_min, y_max). Default is None to use automatic limits.

    analyze_flux : bool, True
        Whether radiative heat flux analysis should be performed,
        including creating heat flux plots and generating flux data

    create_flux_plot : bool, True
        Whether heat flux plot should be created

    flux_plot_filename : str or None
        Filename of heat flux plot file

    flux_coordinates : list of locations
        List of locations at which to determine flux,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]

    flux_contours : array-like or None
        Values at which to plot heat flux contours
        (default: 1.577, 4.732, and 25.237 kW/m2)

    flux_xlims : tuple/list or None, optional
        Heat flux plot x limits: (x_min, x_max). Default is None to use automatic limits.

    flux_ylims : tuple/list or None, optional
        Heat flux plot y limits: (y_min, y_max). Default is None to use automatic limits.

    flux_zlims : tuple/list or None, optional
        Heat flux plot z limits: (z_min, z_max). Default is None to use automatic limits.

    output_dir : str or None
        Filepath of output directory in which to place plots.
        If unspecified, output will be saved to the current working directory.

    verbose : bool
        Verbosity of logging and print statements

    Returns
    -------
    temp_plot_filepath : str or None
        absolute path to temperature plot file

    heatflux_filepath : str or None
        absolute path to temperature plot file

    pos_flux : ndarray or None
        positional flux data (W/m2)

    mass_flow_rate : float
        mass flow rate (kg/s) of the jet flame

    srad : float
        total emitted radiative power of the jet flame (W)

    visible_length : float
        length of visible flame (m)

    radiant_frac : float
        Radiant fraction of flame

    """
    log.info("Jet flame analysis requested")
    now_str = misc_utils.get_now_str()

    if flux_contours is None:
        flux_contours = [1.577, 4.732, 25.237]

    log.info('Creating components')
    orifice = Orifice(orif_diam, Cd=dis_coeff)

    conserve_momentum, notional_nozzle_t = misc_utils.convert_nozzle_model_to_params(nozzle_key, rel_fluid)
    flame_obj = Flame(rel_fluid, orifice, amb_fluid,
                      theta0=rel_angle, y0=0, mdot=mass_flow,
                      nn_conserve_momentum=conserve_momentum, nn_T=notional_nozzle_t,
                      verbose=verbose)

    mass_flow = flame_obj.mass_flow_rate
    srad = flame_obj.get_srad()
    radiant_frac = flame_obj.Xrad
    visible_length = flame_obj.get_visible_length()

    if create_temp_plot:
        output_folder = misc_utils.get_output_folder(output_dir)

        log.info("Creating temp plot")
        if temp_plot_filename is None:
            temp_plot_filename = 'flame_temp_plot_{}.png'.format(now_str)
        temp_plot_filepath = os.path.join(output_folder, temp_plot_filename)

        fig = plot_contour(data_type="temperature", jet_or_flame=flame_obj, xlims=temp_xlims, ylims=temp_ylims, plot_title=temp_plot_title,
                           contour_levels=temp_contours)
        fig.savefig(temp_plot_filepath, bbox_inches='tight')
        plt.close(fig)
    else:
        log.info("Skipping temp plot")
        temp_plot_filepath = None

    if analyze_flux:
        log.info("Assessing flux")
        pos_flux = flame_obj.generate_positional_flux(flux_coordinates, rel_humid)
        if create_flux_plot:
            if flux_plot_filename is None:
                flux_plot_filename = 'flame_heatflux_{}.png'.format(now_str)
            heatflux_filepath = flame_obj.plot_heat_flux_sliced(filename=flux_plot_filename,
                                                                directory=output_dir,
                                                                RH=rel_humid, contours=flux_contours,
                                                                xlims=flux_xlims, ylims=flux_ylims, zlims=flux_zlims,
                                                                savefig=True)
        else:
            log.info("Skipping flux plot")
            heatflux_filepath = None
    else:
        log.info("Skipping flux analysis")
        heatflux_filepath = None
        pos_flux = None

    log.info("Jet flame analysis complete")
    return temp_plot_filepath, heatflux_filepath, pos_flux, mass_flow, srad, visible_length, radiant_frac


def compute_overpressure(method: str, locations,
                         ambient_fluid, release_fluid,
                         orifice_diameter: float, mass_flow=None,
                         release_angle: float = 0, discharge_coefficient: float = 1,
                         nozzle_model: str = 'yuce',
                         bst_flame_speed: float = 0.35, tnt_factor: float = 0.03,
                         flammability_limits=None,
                         origin_at_orifice=False,
                         create_overpressure_plot: bool = True,
                         create_impulse_plot: bool = True,
                         overpressure_plot_filename=None,
                         impulse_plot_filename=None,
                         output_dir=None, verbose=False,
                         overp_contours=None,
                         overp_xlims=None, overp_ylims=None, overp_zlims=None,
                         impulse_contours=None,
                         impulse_xlims=None, impulse_ylims=None, impulse_zlims=None,
                         ):
    """
    Calculate the overpressure and impulse at a specified locations

    Parameters
    ----------
    method : {'bst', 'tnt', 'bauwens'}
        Unconfined overpressure calculation method

    locations : list
        List of locations at which to determine flux,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]

    ambient_fluid : Fluid
        Ambient fluid object

    release_fluid : Fluid
        Release fluid object

    orifice_diameter : float
        Diameter of orifice (m)

    mass_flow : float or None
        fluid flow rate when unchoked [kg/s]

    release_angle : float
        Angle of release (radian). 0 is horizontal, pi/2 is vertical.

    discharge_coefficient : float
        Release discharge coefficient (unitless)

    nozzle_model: {'yuce', 'hars', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model id. Will be parsed to ensure str matches available options.

    bst_flame_speed : float, optional, only needed for BST model
        available mach flame speeds 0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2
        use 5.2 for detonation

    tnt_factor : float, optional, only needed for TNT model
        equivalence factor, float
        TNT equivalency, unitless
        based on HSE guidance in CCPS book https://ebookcentral.proquest.com/lib/sandia/detail.action?docID=624492
        equivalence_factor = 0.03

    flammability_limits : tuple of (lower_flammability_limit, upper_flammability_limit), optional
        Default is None: use default LFL, UFL for selected fuel

    origin_at_orifice : boolean, optional, default to False
        specify if the origin should be at the orifice or calculated

    create_overpressure_plot : bool, True
        Whether overpressure plot should be created

    create_impulse_plot : bool, True
        Whether impulse plot should be created

    overpressure_plot_filename : str or None
        name of overpressure plot file

    impulse_plot_filename : str or None
        name of impulse plot file

    output_dir : str or None
        Filepath of output directory in which to place plots.
        If unspecified, output will be saved to the current working directory.

    overp_contours : array-like or None
        Overpressure values at which to plot contours

    overp_xlims : tuple/list, None, optional
        Overpressure plot x limits: (x_min, x_max). Default is None to use automatic limits.

    overp_ylims : tuple/list, None, optional
        Overpressure plot y limits: (y_min, y_max). Default is None to use automatic limits.

    overp_zlims : tuple/list, None, optional
        Overpressure plot z limits: (z_min, z_max). Default is None to use automatic limits.

    impulse_contours : array-like or None
        Impulse values at which to plot contours

    impulse_xlims : tuple/list, None, optional
        Impulse plot x limits: (x_min, x_max). Default is None to use automatic limits.

    impulse_ylims : tuple/list, None, optional
        Impulse plot y limits: (y_min, y_max). Default is None to use automatic limits.

    impulse_zlims : tuple/list, None, optional
        Impulse plot z limits: (z_min, z_max). Default is None to use automatic limits.

    verbose : bool, False

    Returns
    -------
    dict
        overpressures : ndarray of floats
            overpressure in Pa
        impulses : ndarray of floats
            impulse in Pa*s
        overp_plot_filepath : str or None
            path to overpressure figure
        impulse_plot_filepath : str or None
            path to impulse figure
        mass_flow_rate : float
            Mass flow rate (kg/s) of release
        flam_or_det_mass : float
            Mass of release related to overpressure calculation (kg).
            Outputs flammable mass for BST and TNT methods, detonable mass for Bauwens
        tnt_mass : float or None
            Equivalent mass of TNT [kg]; only generated for TNT method

    """
    log.info("Unconfined overpressure calculation requested")
    params = locals()
    log.info(misc_utils.params_as_str(params))

    orifice = Orifice(orifice_diameter, discharge_coefficient)
    nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
    log.info('Creating jet')
    jet_object = Jet(release_fluid, orifice, ambient_fluid, mdot=mass_flow, theta0=release_angle,
                     nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param, verbose=verbose)

    tnt_mass = None

    method = method.lower()
    if method == 'bst':
        log.info("Calculating overpressure with BST method")
        over_pressure_model = BST_method(jet_object=jet_object,
                                         mach_flame_speed=bst_flame_speed,
                                         flammability_limits=flammability_limits,
                                         origin_at_orifice=origin_at_orifice)
        flam_or_det_mass = over_pressure_model.flammable_mass
    elif method == 'tnt':
        log.info("Calculating overpressure with TNT method")
        over_pressure_model = TNT_method(jet_object=jet_object,
                                         equivalence_factor=tnt_factor,
                                         flammability_limits=flammability_limits,
                                         origin_at_orifice=origin_at_orifice)
        flam_or_det_mass = over_pressure_model.flammable_mass
        tnt_mass = over_pressure_model.equiv_TNT_mass
    elif method == 'bauwens':
        log.info("Calculating overpressure with Bauwens method")
        over_pressure_model = Bauwens_method(jet_object=jet_object,
                                             origin_at_orifice=origin_at_orifice)
        flam_or_det_mass = over_pressure_model.detonable_mass
    else:
        raise ValueError('Invalid method name in unconfined overpressure analysis')

    overpressure = over_pressure_model.calc_overpressure(locations)
    impulse = over_pressure_model.calc_impulse(locations)

    if create_overpressure_plot:
        log.info("Creating overpressure plot")
        if overpressure_plot_filename is None:
            now_str = misc_utils.get_now_str()
            overpressure_plot_filename = 'overpressure_plot_{}.png'.format(now_str)
        overpressure_plot_filepath = over_pressure_model.plot_overpressure_sliced(
                                                            plot_filename=overpressure_plot_filename,
                                                            directory=output_dir,
                                                            contours=overp_contours,
                                                            xlims=overp_xlims,
                                                            ylims=overp_ylims,
                                                            zlims=overp_zlims,
                                                            savefig=True)
    else:
        log.info("Skipping overpressure plot")
        overpressure_plot_filepath = None

    if create_impulse_plot and not np.isnan(impulse).any():
        log.info("Creating impulse plot")
        if impulse_plot_filename is None:
            now_str = misc_utils.get_now_str()
            impulse_plot_filename = 'impulse_plot_{}.png'.format(now_str)
        impulse_plot_filepath = over_pressure_model.plot_impulse_sliced(
                                                            plot_filename=impulse_plot_filename,
                                                            directory=output_dir,
                                                            contours=impulse_contours,
                                                            xlims=impulse_xlims,
                                                            ylims=impulse_ylims,
                                                            zlims=impulse_zlims,
                                                            savefig=True)
    else:
        log.info("Skipping impulse plot")
        impulse_plot_filepath = None

    log.info("Unconfined overpressure calculation complete")
    results = {
        'overpressures': overpressure,
        'impulses': impulse,
        'overp_plot_filepath': overpressure_plot_filepath,
        'impulse_plot_filepath': impulse_plot_filepath,
        'mass_flow_rate': jet_object.mass_flow_rate,
        'flam_or_det_mass': flam_or_det_mass,
        'tnt_mass': tnt_mass
    }
    return results
