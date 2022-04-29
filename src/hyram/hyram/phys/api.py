"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
import os

import matplotlib.pyplot as plt
import numpy as np

from . import _comps, _flame, _fuel_props, _indoor_release, _jet, _unconfined_overpressure
from ._fuel_props import Fuel_Properties
from ..utilities import misc_utils, exceptions

"""
The API file provides external access to common analysis functions within the physics module.
API functions prep logging, clean up parameters, and return results or error notifications.

Notes:
    * Queries from the C# GUI should call the c_api and not this file to ensure adequate pre-processing occurs
    * Logging should be set up by the user, not the app. The only exception to this is the C API, which sets up logging
    for the GUI requests.
"""

log = logging.getLogger(__name__)


def create_fluid(species, temp=None, pres=None, density=None, phase=None):
    """
    Create fluid from given parameters. Exactly two of temperature, pressure, density, phase must be provided.

    Parameters
    ----------
    species : str
        Name of fluid, e.g. 'H2', 'AIR'.

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
    if not misc_utils.is_fluid_specified(temp, pres, density, phase):
        raise exceptions.FluidSpecificationError(function='API')

    fluid = _comps.Fluid(species=species.upper(), T=temp, P=pres, rho=density, phase=phase)
    return fluid


def compute_mass_flow(fluid, orif_diam, amb_pres=101325.,
                      is_steady=True, tank_vol=None, dis_coeff=1., output_dir=None, create_plot=True):
    """
    Calculate mass flow rate based on given conditions.

    Parameters
    ----------
    fluid : _comps.Fluid
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

    output_dir : str
        Path to directory in which to place output file(s).

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

    orif = _comps.Orifice(orif_diam, Cd=dis_coeff)

    if is_steady:
        result['rates'] = [orif.mdot(orif.flow(fluid, amb_pres))]

    else:
        source = _comps.Source(tank_vol, fluid)
        mdots, fluid_list, t, sol = source.empty(orif, amb_pres)

        if create_plot:
            if output_dir is None:
                output_dir = os.getcwd()
            filename = "time-to-empty-{}.png".format(misc_utils.get_now_str())
            filepath = os.path.join(output_dir, filename)
            fig, axs = plt.subplots(4, 1, sharex=True, squeeze=True, figsize=(4, 7))
            axs[0].plot(t, sol[0])
            axs[0].set_ylabel('Mass [kg]')
            axs[1].plot(t, np.array([f.P for f in fluid_list]) * 1e-5)
            axs[1].set_ylabel('Pressure [bar]')
            axs[2].plot(t, mdots)
            axs[2].set_ylabel('Flow Rate [kg/s]')
            axs[3].plot(t, [f.T for f in fluid_list])
            axs[3].set_ylabel('Temperature [K]')
            axs[3].set_xlabel('Time [s]')
            [a.minorticks_on() for a in axs]
            [a.grid(which='major', color='k', dashes=(2, 2), alpha=.5) for a in axs]
            [a.grid(which='minor', color='k', alpha=.1) for a in axs]
            fig.savefig(filepath, bbox_inches='tight')
            plt.close(fig)
            result['plot'] = filepath

        result["time_to_empty"] = t[-1]
        result["times"] = t
        result["rates"] = mdots

    log.info("Mass Flow analysis complete")
    return result


def compute_tank_mass(fluid, tank_vol):
    """
    Tank mass calculation.
    Two of temp, pressure, phase are required.

    Parameters
    ----------
    fluid : _comps.Fluid
        Release fluid object

    tank_vol : float
        Volume of source in tank (m^3)

    Returns
    ----------
    float
        Tank mass (kg)
    """
    log.info("Tank Mass calculation requested")
    source = _comps.Source(tank_vol, fluid)
    mass = source.mass
    log.info("Tank Mass calculation complete")
    return mass


def compute_thermo_param(species='H2', phase=None, temp=None, pres=None, density=None):
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
        msg = ('Wrong number of inputs provided'
               ' - two of [temperature (or phase), pressure, density] are required')
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


def compute_equivalent_tnt_mass(vapor_mass, percent_yield, fuel):
    """
    Calculate equivalent mass of TNT.

    Parameters
    ----------
    vapor_mass : float
        Mass of flammable vapor released (kg)

    percent_yield : float
        Explosive energy yield (0 to 100)
    
    fuel : string
        fuel being used (H2, CH4, C3H8, etc.)

    Returns
    ----------
    result : float
        Equivalent mass of TNT (kg)
    """

    heat_of_combustion = _fuel_props.Fuel_Properties(fuel).dHc # heat of combustion, J/kg
    log.info("TNT mass calculation: vapor mass {:,.4f} kg, yield {:,.1f}%, heat of combustion {:,.5f} kj/kg".format(
             vapor_mass, percent_yield, heat_of_combustion/1000))
    result = vapor_mass * (percent_yield / 100.) * heat_of_combustion / 1000 / 4500.

    log.info("TNT mass calculation complete")
    return result


def analyze_jet_plume(amb_fluid, rel_fluid, orif_diam,
                      rel_angle=0., dis_coeff=1., nozzle_model='yuce',
                      create_plot=True, contour=None, contour_min=0., contour_max=0.1,
                      xmin=-2.5, xmax=2.5, ymin=0., ymax=10., plot_title="Mole Fraction of Leak",
                      filename=None, output_dir=None, verbose=False):
    """
    Simulate jet plume for leak and generate plume positional data, including mass and mole fractions, plume plot.

    Parameters
    ----------
    amb_fluid : _comps.Fluid
        Ambient fluid object

    rel_fluid : _comps.Fluid
        Release fluid object

    orif_diam : float
        Diameter of orifice (m).

    rel_angle : float
        Angle of release (radian). 0 is horizontal, pi/2 is vertical.

    dis_coeff : float
        Release discharge coefficient (unitless).

    nozzle_model: {'yuce', 'hars', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model id. Will be parsed to ensure str matches available options.

    create_plot : bool
        Whether mole fraction contour plot should be created

    contour : list or int or float
        define contour lines
        Default is None: will use default LFL for selected fuel

    contour_min : float
        minimum value of contour line

    contour_max : float
        maximum value of contour line

    xmin : float
        Plot x minimum.

    xmax : float
        Plot x maximum.

    ymin : float
        Plot y minimum.

    ymax : float
        Plot y maximum.

    plot_title : str
        Title displayed in output plot.

    filename : str or None
        Plot filename, excluding path.

    output_dir : str or None
        Directory in which to place plot file.

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
    """
    log.info("Plume plot requested")
    if contour is None:
        fuel_props = Fuel_Properties(rel_fluid.species)
        contour = fuel_props.LFL

    log.info('Creating components')
    orifice = _comps.Orifice(orif_diam, dis_coeff)

    # Jet requires parameters defining notional nozzle model, rather than model itself.
    nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, rel_fluid)

    log.info('Creating jet')
    jet_obj = _jet.Jet(rel_fluid, orifice, amb_fluid, theta0=rel_angle,
                       nn_conserve_momentum=nozzle_cons_momentum,
                       nn_T=nozzle_t_param, verbose=verbose)

    xs, ys, mole_fracs, mass_fracs, vs, temps = jet_obj.get_contour_data()
    mass_flow_rate = jet_obj.mass_flow_rate

    if create_plot:
        log.info("Creating mole fraction contour plot")
        if output_dir is None:
            output_dir = misc_utils.get_temp_folder()

        if filename is None:
            filename = "plume-mole-plot-{}.png".format(misc_utils.get_now_str())

        if xmax >= xmin:
            xlims = (xmin, xmax)
        else:
            raise ValueError(f'xmin ({xmin}) is greater than xmax ({xmax})')
        if ymax >= ymin:
            ylims = (ymin, ymax)
        else:
            raise ValueError(f'ymin ({ymin}) is greater than ymax ({ymax})')

        contours = [contour] if type(contour) in [int, float] else contour
        for contour in contours:
            if contour <= 0 or contour >=1:
                error_msg = ('Mole fraction contour values must be >0 and <1'
                             + ' (current value: {})'.format(contour))
                raise ValueError(error_msg)
        plot_filepath = os.path.join(output_dir, filename)
        plot_fig = jet_obj.plot_moleFrac_Contour(xlims=xlims, ylims=ylims,
                                                 plot_title=plot_title,
                                                 mark=contours,
                                                 vmin=contour_min,
                                                 vmax=contour_max)
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
                   'mass_flow_rate': mass_flow_rate}
    return result_dict


def analyze_accumulation(amb_fluid, rel_fluid,
                         tank_volume, orif_diam, rel_height,
                         enclos_height, floor_ceil_area,
                         ceil_vent_xarea, ceil_vent_height,
                         floor_vent_xarea, floor_vent_height,
                         times, orif_dis_coeff=1., ceil_vent_coeff=1., floor_vent_coeff=1.,
                         vol_flow_rate=0., dist_rel_to_wall=np.inf,
                         tmax=None, rel_area=None, rel_angle=0., nozzle_key='yuce',
                         x0=0., y0=0., nmax=1000,
                         temp_pres_points=None, pres_ticks=None, is_steady=False,
                         create_plots=True, output_dir=None, verbose=False):
    """
    Simulate an indoor release of designated fluid.

    Parameters
    ----------
    amb_fluid : _comps.Fluid
        Ambient fluid object

    rel_fluid : _comps.Fluid
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
        Orifice discharge coeffecient [unitless].

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

    output_dir : str, optional
        Filepath of output directory in which to place plots. Will use cwd if not provided.

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
    rel_source = _comps.Source(tank_volume, rel_fluid)
    orifice = _comps.Orifice(orif_diam, orif_dis_coeff)

    conserve_momentum, notional_nozzle_t = misc_utils.convert_nozzle_model_to_params(nozzle_key, rel_fluid)
    ceil_vent = _comps.Vent(ceil_vent_xarea, ceil_vent_height, ceil_vent_coeff, vol_flow_rate)
    floor_vent = _comps.Vent(floor_vent_xarea, floor_vent_height, floor_vent_coeff, vol_flow_rate)
    enclosure = _comps.Enclosure(enclos_height, floor_ceil_area, rel_height, ceil_vent, floor_vent,
                                 Xwall=dist_rel_to_wall)

    release_obj = _indoor_release.IndoorRelease(rel_source, orifice, amb_fluid, enclosure,
                                                tmax=tmax, release_area=rel_area, steady=is_steady,
                                                nn_conserve_momentum=conserve_momentum, nn_T=notional_nozzle_t,
                                                theta0=rel_angle, x0=x0, y0=y0, nmax=nmax, verbose=verbose)

    mass_flow_result = compute_mass_flow(rel_fluid, orif_diam, amb_fluid.P, is_steady=is_steady, tank_vol=tank_volume,
                                         dis_coeff=orif_dis_coeff, output_dir=output_dir, create_plot=create_plots)
    # interpolate mass flow rates for given times
    if is_steady:
        rates = np.full_like(times, mass_flow_result['rates'])
    else:
        rates = np.interp(times, mass_flow_result['times'], mass_flow_result['rates'])

    # Generate plots
    if create_plots:
        if output_dir is None:
            output_dir = os.getcwd()

        now_str = misc_utils.get_now_str()
        pres_plot_fpath = os.path.join(output_dir, 'pressure_plot_{}.png'.format(now_str))
        layer_plot_fpath = os.path.join(output_dir, 'layer_plot_{}.png'.format(now_str))
        traj_plot_fpath = os.path.join(output_dir, 'trajectory_plot_{}.png'.format(now_str))
        mass_plot_fpath = os.path.join(output_dir, 'flam_mass_plot_{}.png'.format(now_str))

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

        mass_flow_plot_fpath = mass_flow_result['plot']

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


def jet_flame_analysis(amb_fluid, rel_fluid, orif_diam,
                       dis_coeff=1., rel_angle=0.,
                       nozzle_key='yuce', rel_humid=0.89, contours=None,
                       create_temp_plot=True, analyze_flux=True,
                       temp_plot_filename=None,
                       heatflux_plot_filename=None,
                       flux_coordinates=None,
                       output_dir=None, verbose=False):
    """
    Assess jet flame behavior and flux data and create corresponding plots.

    Parameters
    ----------
    amb_fluid : _comps.Fluid
        Ambient fluid object

    rel_fluid : _comps.Fluid
        Release fluid object

    orif_diam : float
        Orifice diameter (m)

    dis_coeff : float
        Orifice discharge coeffecient [unitless]

    rel_angle : float
        Angle of release (0 is horizontal) (radians)

    nozzle_key : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    rel_humid : float
        Relative humidity between 0 and 1

    contours : array-like
        Values at which to plot heat flux contours
        (default: 1.577, 4.732, and 25.237 kW/m2)

    create_temp_plot : bool, True
        Whether temperature plot should be created

    analyze_flux : bool, True
        Whether radiative heat flux analysis should be performed,
        including creating heat flux plots and generating flux data

    temp_plot_filename : str or None
        Desired filename of output temp plot file

    heatflux_plot_filename : str or None
        Filename of heat flux plot file

    flux_coordinates : list of locations
        List of locations at which to determine flux,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]

    output_dir : str
        Filepath of output directory in which to place plots,
        will use cwd if not provided

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
    """
    log.info("Jet flame analysis requested")
    now_str = misc_utils.get_now_str()

    if output_dir is None:
        output_dir = os.getcwd()
    if contours is None:
        contours = [1.577, 4.732, 25.237]

    log.info('Creating components')
    orifice = _comps.Orifice(orif_diam, Cd=dis_coeff)

    conserve_momentum, notional_nozzle_t = misc_utils.convert_nozzle_model_to_params(nozzle_key, rel_fluid)
    flame_obj = _flame.Flame(rel_fluid, orifice, amb_fluid,
                             theta0=rel_angle, y0=0,
                             nn_conserve_momentum=conserve_momentum, nn_T=notional_nozzle_t,
                             verbose=verbose)

    mass_flow = flame_obj.mass_flow_rate
    srad = flame_obj.get_srad()
    visible_length = flame_obj.get_visible_length()

    if create_temp_plot:
        log.info("Creating temp plot")
        if temp_plot_filename is None:
            temp_plot_filename = 'flame_temp_plot_{}.png'.format(now_str)
        temp_plot_filepath = os.path.join(output_dir, temp_plot_filename)
        fig, _ = flame_obj.plot_Ts()
        fig.savefig(temp_plot_filepath, bbox_inches='tight')
        plt.close(fig)
    else:
        log.info("Skipping temp plot")
        temp_plot_filepath = None

    if analyze_flux:
        log.info("Assessing flux and creating plots")
        if heatflux_plot_filename is None:
            heatflux_plot_filename = 'flame_heatflux_{}.png'.format(now_str)
        heatflux_filepath = flame_obj.plot_heat_flux_sliced(filename=heatflux_plot_filename,
                                                            directory=output_dir,
                                                            RH=rel_humid, contours=contours,
                                                            savefig=True)
        pos_flux = flame_obj.generate_positional_flux(flux_coordinates, rel_humid)
    else:
        log.info("Skipping flux analysis")
        heatflux_filepath = None
        pos_flux = None

    log.info("Jet flame analysis complete")
    return temp_plot_filepath, heatflux_filepath, pos_flux, mass_flow, srad, visible_length


def compute_overpressure(method: str, locations,
                         ambient_fluid, release_fluid,
                         orifice_diameter: float, release_angle: float = 0., discharge_coefficient: float = 1.,
                         nozzle_model: str = 'yuce', heat_of_combustion=None,
                         BST_mach_flame_speed: float = 5.2, TNT_equivalence_factor: float = 0.03,
                         origin_at_orifice=False,
                         create_overpressure_plot: bool = True,
                         overpressure_plot_filename=None,
                         output_dir=None, contours=None, verbose=False,
                         xmin=None, xmax=None, ymin=None, ymax=None, zmin=None, zmax=None):
    """
    Calculate the overpressure and impulse at a specified locations

    Parameters
    ----------
    method : {'bst', 'tnt', 'bauwens'}
        Unconfined overpressure calculation method
    
    locations : list of locations
        List of locations at which to determine flux,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]

    ambient_fluid : _comps.Fluid
        Ambient fluid object

    release_fluid : _comps.Fluid
        Release fluid object

    orifice_diameter : float
        Diameter of orifice (m)

    release_angle : float
        Angle of release (radian). 0 is horizontal, pi/2 is vertical.

    discharge_coefficient : float
        Release discharge coefficient (unitless)

    nozzle_model: {'yuce', 'hars', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model id. Will be parsed to ensure str matches available options.

    heat_of_combustion : float, optional
        heat of combustion of fuel in J/kg

    BST_mach_flame_speed : float, optional, only needed for BST model
        available mach flame speeds 0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2
        use 5.2 for detonation 

    TNT_equivalence_factor : float, optional, only needed for TNT model
        equivalence factor, float
        TNT equivalency, unitless
        # based on HSE guidance in CCPS book https://ebookcentral.proquest.com/lib/sandia/detail.action?docID=624492
        equivalence_factor = 0.03
    
    origin_at_orifice : boolean, optional, default to False
        specify if the origin should be at the orifice or calculated

    create_overpressure_plot : bool, True
        Whether overpressure plot should be created

    overpressure_plot_filepath : str or None
        absolute path to overpressure plot file

    output_dir : str
        Filepath of output directory in which to place plots. Will use cwd if not provided.

    contours : array-like or None
        Overpressure values at which to plot contours

    verbose : bool, False

    xmin : float, None
        Plot x minimum.

    xmax : float, None
        Plot x maximum.

    ymin : float, None
        Plot y minimum.

    ymax : float, None
        Plot y maximum.

    zmin : float, None
        Plot z minimum.

    zmax : float, None
        Plot z maximum.

    Returns
    -------
    dict
        overpressure : ndarray of floats
            overpressure in Pa
        impulse : ndarray of floats
            impulse in Pa*s
        figure_file_path : str or None
            path to overpressure figure
        mass_flow_rate : float
            Mass flow rate (kg/s) of release

    """
    log.info("Unconfined overpressure calculation requested")
    params = locals()
    log.info(misc_utils.params_as_str(params))

    orifice = _comps.Orifice(orifice_diameter, discharge_coefficient)
    nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, release_fluid)
    log.info('Creating jet')
    jet_object = _jet.Jet(release_fluid, orifice, ambient_fluid, theta0=release_angle,
                          nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param, verbose=verbose)

    method = method.lower()
    if method == 'bst':
        log.info("Calculating overpressure with BST method")
        over_pressure_model = _unconfined_overpressure.BST_method(jet_object=jet_object,
                                                                  mach_flame_speed=BST_mach_flame_speed,
                                                                  origin_at_orifice=origin_at_orifice)
    elif method == 'tnt':
        log.info("Calculating overpressure with TNT method")
        over_pressure_model = _unconfined_overpressure.TNT_method(jet_object=jet_object,
                                                                  equivalence_factor=TNT_equivalence_factor,
                                                                  origin_at_orifice=origin_at_orifice)
    elif method == 'bauwens':
        log.info("Calculating overpressure with Bauwens method")
        over_pressure_model = _unconfined_overpressure.Bauwens_method(jet_object=jet_object,
                                                                      origin_at_orifice=origin_at_orifice)
    else:
        raise exceptions.InputError(function="Overpressure analysis", message='Invalid method name')
    overpressure = over_pressure_model.calc_overpressure(locations)
    impulse = over_pressure_model.calc_impulse(locations)

    if create_overpressure_plot:
        log.info("Creating overpressure plot")
        now_str = misc_utils.get_now_str()
        if output_dir is None:
            output_dir = os.getcwd()
        if overpressure_plot_filename is None:
            overpressure_plot_filename = 'overpressure_plot_{}.png'.format(now_str)
        overpressure_plot_filepath = over_pressure_model.plot_overpressure_sliced(
                                                            plot_filename=overpressure_plot_filename,
                                                            directory=output_dir,
                                                            contours=contours,
                                                            xlims=[xmin, xmax],
                                                            ylims=[ymin, ymax],
                                                            zlims=[zmin, zmax],
                                                            savefig=True)
    else:
        log.info("Skipping overpressure plot")
        overpressure_plot_filepath = None

    log.info("Unconfined overpressure calculation complete")
    results = {
        'overpressure': overpressure,
        'impulse': impulse,
        'figure_file_path': overpressure_plot_filepath,
        'mass_flow_rate': jet_object.mass_flow_rate,
    }
    return results
