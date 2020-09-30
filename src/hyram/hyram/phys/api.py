import datetime
import logging
import os
import warnings

import numpy as np
import matplotlib.pyplot as plt

from . import _comps, _therm, _flame, _flux, _jet
from ._indoor_release import IndoorRelease
from ..utilities import misc_utils, custom_warnings, exceptions

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

    phase : {'gas', 'liquid', None}
        Fluid phase; gas implies saturated vapor, liquid implies saturated liquid. If none, Coolprop determines it.
        None corresponds to default 'gas' in GUI.

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

    print(temp, pres, density, phase)
    fluid = _comps.Fluid(species=species.upper(), T=temp, P=pres, rho=density, phase=phase)
    return fluid


def compute_mass_flow(fluid, orif_diam, amb_pres=101325.,
                      is_steady=True, tank_vol=None, dis_coeff=1., output_dir=None, debug=False):
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

    Returns
    ----------
    result : dict
        If is_steady, mass flow rate will be returned. If false, all other params are present.
        mass_flow_rate : float
            Mass flow rate (kg/s) of steady release.
        time_to_empty : float
            Time (s) it takes to blowdown the tank to empty.
        plot : str
            Path to plot of mass flow rate vs. time. Only created if Steady is false.
        times : array of floats
            Times at which mass flow rates occur during blowdown.
        rates : array of floats
            Mass flow rates during blowdown.

    """
    log.info("Mass Flow analysis requested")

    if output_dir is None:
        output_dir = os.getcwd()

    result = {"mass_flow_rate": None, "time_to_empty": None, "plot": "", "times": None, "rates": None}

    orif = _comps.Orifice(orif_diam, Cd=dis_coeff)

    if is_steady:
        result['mass_flow_rate'] = orif.mdot(orif.flow(fluid, amb_pres))

    else:
        # create MFR vs time plot
        source = _comps.Source(tank_vol, fluid)

        mdots, fluid_list, t, sol = source.empty(orif, amb_pres)

        filename = os.path.join(output_dir, "time-to-empty.png")
        fig, ax = plt.subplots(4, 1, sharex=True, squeeze=True, figsize=(4, 7))
        ax[0].plot(t, sol[0])
        ax[0].set_ylabel('mass [kg]')
        ax[1].plot(t, np.array([f.P for f in fluid_list]) * 1e-5)
        ax[1].set_ylabel('pressure [bar]')
        ax[2].plot(t, mdots);
        ax[2].set_ylabel('$\dot{m}$ (kg/s)')
        ax[3].plot(t, [f.T for f in fluid_list])
        ax[3].set_ylabel('temperature [K]')
        ax[3].set_xlabel('time [s]')
        [a.minorticks_on() for a in ax]
        [a.grid(which='major', color='k', dashes=(2, 2), alpha=.5) for a in ax]
        [a.grid(which='minor', color='k', alpha=.1) for a in ax]
        plt.savefig(filename, bbox_inches='tight')
        plt.close()

        result["time_to_empty"] = t[-1]
        result['plot'] = filename
        # also return data for easier testing
        result["times"] = t
        result["rates"] = mdots

    log.info("Mass Flow analysis complete")
    return result


def compute_tank_mass(fluid, tank_vol, debug=False):
    """
    Tank mass calculation.
    Two of temp, pressure, phase are required.

    Parameters
    ----------
    fluid : _comps.Fluid
        Release fluid object

    tank_vol : float
        Volume of source in tank (m^3).

    debug : bool
        Whether debug mode is active

    Returns
    ----------
    float
        Tank mass (kg).

    """
    log.info("Tank Mass calculation requested")
    source = _comps.Source(tank_vol, fluid)
    mass = source.mass
    log.info("Tank Mass calculation complete")
    return mass


def compute_thermo_param(species='H2', temp=None, pres=None, density=None, debug=False):
    """
    Calculate temperature, pressure or density of species.

    Parameters
    ----------
    species : str
        Fluid species formula or name (see CoolProp documentation).

    temp : float
        Fluid temperature (K).

    pres : float
        Fluid pressure (Pa).

    density : float
        Fluid density (kg/m^3).

    debug : bool
        Whether debug mode is active

    Returns
    ----------
    result : float
        Temp, pressure, or density, depending on provided parameters.

    """
    log.info("TPD Parameter calculation requested")

    fluid = create_fluid(species, temp, pres, density, phase=None)

    if temp != None and pres != None:
        result = fluid.rho
    elif temp != None and density != None:
        result = fluid.P
    elif pres != None and density != None:
        result = fluid.T
    else:
        # TODO: best return val here?
        result = None
    log.info("TPD Parameter calculation complete")
    return result


def analyze_jet_plume(amb_fluid, rel_fluid, orif_diam,
                      rel_angle=0., dis_coeff=1., nozzle_model='yuce',
                      create_plot=True, contour=0.04, contour_min=0., contour_max=0.1,
                      xmin=-2.5, xmax=2.5, ymin=0., ymax=10., plot_title="Mole Fraction of Leak",
                      filename=None, output_dir=None, verbose=False, debug=False):
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

    debug : bool, False

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

    """
    log.info("Plume plot requested")
    log.info('Creating components')
    orifice = _comps.Orifice(orif_diam, dis_coeff)

    # Jet requires parameters defining notional nozzle model, rather than model itself.
    nozzle_cons_momentum, nozzle_t_param = misc_utils.convert_nozzle_model_to_params(nozzle_model, rel_fluid)

    log.info('Creating jet')
    jet_obj = _jet.Jet(rel_fluid, orifice, amb_fluid, theta0=rel_angle,
                       nn_conserve_momentum=nozzle_cons_momentum, nn_T=nozzle_t_param, verbose=verbose)

    xs, ys, mole_fracs, mass_fracs, vs, temps = jet_obj.get_contour_data()

    if create_plot:
        log.info("Creating mole fraction contour plot")
        if output_dir is None:
            output_dir = os.path.join(os.path.dirname(os.path.realpath(__file__)), 'temp')

        if filename is None:
            filename = "plume-mole-plot-{}.png".format(misc_utils.get_now_str())

        xlims = np.array([xmin, xmax])
        ylims = np.array([ymin, ymax])
        contours = [contour] if type(contour) in [int, float] else contour
        plot_filepath = os.path.join(output_dir, filename)
        plot_fig = jet_obj.plot_moleFrac_Contour(xlims=xlims, ylims=ylims, plot_title=plot_title, mark=contours,
                                                 vmin=contour_min, vmax=contour_max)
        plot_fig.savefig(plot_filepath, bbox_inches='tight')
        log.info("Plume plot complete")
    else:
        plot_filepath = ''

    result_dict = {'xs': xs, 'ys': ys, 'mole_fracs': mole_fracs, 'mass_fracs': mass_fracs, 'vs': vs,
                   'temps': temps, 'plot': plot_filepath}
    return result_dict


def analyze_indoor_release(amb_fluid, rel_fluid,
                           tank_volume, orif_diam, rel_height,
                           enclos_height, floor_ceil_area,
                           ceil_vent_xarea, ceil_vent_height,
                           floor_vent_xarea, floor_vent_height,
                           times, orif_dis_coeff=1., ceil_vent_coeff=1., floor_vent_coeff=1.,
                           vol_flow_rate=0., dist_rel_to_wall=np.inf,
                           tmax=None, rel_area=None, rel_angle=0., nozzle_key='yuce',
                           x0=0., y0=0., nmax=1000,
                           temp_pres_points=None, pres_ticks=None,
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

    """
    log.info("Indoor Release analysis requested")
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

    release_obj = IndoorRelease(rel_source, orifice, amb_fluid, enclosure,
                                tmax=tmax, release_area=rel_area,
                                nn_conserve_momentum=conserve_momentum, nn_T=notional_nozzle_t,
                                theta0=rel_angle, x0=x0, y0=y0, nmax=nmax, verbose=verbose)

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

        lfig = release_obj.plot_layer()
        lfig.savefig(layer_plot_fpath, bbox_inches='tight')

        tfig = release_obj.plot_trajectories()
        tfig.savefig(traj_plot_fpath, bbox_inches='tight')

        mfig = release_obj.plot_mass()
        mfig.savefig(mass_plot_fpath, bbox_inches='tight')

    else:
        pres_plot_fpath = ''
        layer_plot_fpath = ''
        traj_plot_fpath = ''
        mass_plot_fpath = ''

    max_pres, max_time = release_obj.max_p_t()

    result_dict = {
        'status': 1,
        'pressures_per_time': release_obj.pressure(times),
        'depths': release_obj.layer_depth(times),
        'concentrations': release_obj.concentration(times),
        'overpressure': max_pres,
        'time_of_overp': max_time,
        'pres_plot_filepath': pres_plot_fpath,
        'mass_plot_filepath': mass_plot_fpath,
        'layer_plot_filepath': layer_plot_fpath,
        'trajectory_plot_filepath': traj_plot_fpath,
    }

    log.info("Indoor release analysis complete")
    return result_dict


def jet_flame_analysis(amb_fluid, rel_fluid, orif_diam,
                       dis_coeff=1., rel_angle=0., rel_height=0.,
                       nozzle_key='yuce', rad_src_key='multi', rel_humid=0.89, contours=None,
                       create_temp_plot=True, analyze_flux=True, create_3dplot=False,
                       temp_plot_filename=None,
                       plot3d_filename=None,
                       plot2d_filename=None,
                       xpos=None, ypos=None, zpos=None,
                       chem_filepath=None,
                       output_dir=None, debug=False, verbose=False):
    """
    Assess jet flame behavior and flux data and create corresponding plots.

    Parameters
    ----------
    amb_fluid : _comps.Fluid
        Ambient fluid object

    rel_fluid : _comps.Fluid
        Release fluid object

    orif_diam : float
        Orifice diameter (m).

    dis_coeff : float
        Orifice discharge coeffecient [unitless].

    rel_angle : float
        Angle of release (0 is horizontal) (radians).

    rel_height : float, optional
        Height of release (m) above floor at 0.

    nozzle_key : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    rad_src_key : {'single', 'multi'}
        Radiative source model.

    rel_humid : float
        Relative humidity between 0 and 1.

    contours : array-like

    create_temp_plot : bool, True
        Whether temperature plot should be created

    analyze_flux : bool, True
        whether radiative heat flux analysis should be performed.
        Includes creating heat flux plots and generating flux data.

    create_3dplot : bool, False
        Whether 3D flux plot should be created

    temp_plot_filename : str or None
        Desired filename of output temp plot file.

    plot3d_filename : str or None
        Filename of 3D plot output.

    plot2d_filename : str or None
        Filename of 2D plot output.

    xpos : ndarray
        Array of x-coordinates, following flame center-line, of positions (m) at which to determine flux.

    ypos : ndarray
        Array of y-coordinates (vertical) of positions (m) at which to determine flux.

    zpos : ndarray
        Array of z-coordinates of positions (m) at which to determine flux.

    chem_filepath : str
        Path to .pkl file storing Combustion chemistry class data.

    output_dir : str
        Filepath of output directory in which to place plots. Will use cwd if not provided.

    verbose : bool
        Verbosity of logging and print statements.

    Returns
    -------
    temp_plot_filepath : str or None
        absolute path to temperature plot file

    flux3d_filepath : str or None
        absolute path to temperature plot file

    slice_filepath : str or None
        absolute path to temperature plot file

    pos_flux : ndarray or None
        positional flux data

    """
    log.info("Jet flame analysis requested")
    now_str = misc_utils.get_now_str()

    if output_dir is None:
        output_dir = os.getcwd()
    if contours is None:
        contours = [1.577, 4.732, 25.237]

    log.info('Creating components')
    orifice = _comps.Orifice(orif_diam, Cd=dis_coeff)
    num_c_atoms = misc_utils.get_num_carbon_atoms_from_species(rel_fluid.species)

    # Load chemistry class, if given
    chem_obj = _therm.Combustion(Treac=amb_fluid.T, nC=num_c_atoms, P=amb_fluid.P) if chem_filepath else None

    conserve_momentum, notional_nozzle_t = misc_utils.convert_nozzle_model_to_params(nozzle_key, rel_fluid)
    flame_obj = _flame.Flame(rel_fluid, orifice, amb_fluid,
                             nC=num_c_atoms, theta0=rel_angle, y0=rel_height,
                             nn_conserve_momentum=conserve_momentum, nn_T=notional_nozzle_t, chem=chem_obj,
                             verbose=verbose)

    if create_temp_plot:
        log.info("Creating flux plot")
        if temp_plot_filename is None:
            temp_plot_filename = 'flame_temp_plot_{}.png'.format(now_str)
        temp_plot_filepath = os.path.join(output_dir, temp_plot_filename)
        fig, colorbar = flame_obj.plot_Ts()
        fig.savefig(temp_plot_filepath, bbox_inches='tight')
    else:
        log.info("Skipping temp plot")
        temp_plot_filepath = None

    if analyze_flux:
        log.info("Assessing flux and creating plots")
        if plot3d_filename is None:
            plot3d_filename = 'flame_flux3d_{}.png'.format(now_str)
        if plot2d_filename is None:
            plot2d_filename = 'flame_flux2d_{}.png'.format(now_str)
        flux3d_filepath, slice_filepath = flame_obj.iso_heat_flux_plot_sliced(plot3d_filename=plot3d_filename,
                                                                              plot2d_filename=plot2d_filename,
                                                                              directory=output_dir,
                                                                              smodel=rad_src_key,
                                                                              RH=rel_humid, contours=contours,
                                                                              plot3d=create_3dplot, plot_sliced=True,
                                                                              savefigs=True)
        pos_flux = flame_obj.generate_positional_flux(xpos, ypos, zpos, rel_humid, rad_src_key)
    else:
        log.info("skipping flux analysis")
        flux3d_filepath = None
        slice_filepath = None
        pos_flux = None

    log.info("Jet flame analysis complete")
    return temp_plot_filepath, flux3d_filepath, slice_filepath, pos_flux


def compute_discharge_rate(fluid, orif_diam, dis_coeff=1., verbose=False):
    """
    Returns the mass flow rate through an orifice of diameter d from gas at a temperature T and pressure P.

    Parameters
    ----------
    Fluid : _comps.Fluid
        Release fluid

    orif_diam : float
        Orifice diameter (m).

    dis_coeff : float, optional
        Discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow).

    Returns
    -------
    float
        Mass flow rate (kg/s).

    """
    log.info("Discharge rate calculation requested")
    orif = _comps.Orifice(orif_diam, Cd=dis_coeff)
    discharge_rate = orif.mdot(orif.flow(fluid))
    log.info("Discharge rate request complete")
    return discharge_rate


def flux_analysis(amb_fluid, rel_fluid,
                  rel_height, rel_angle,
                  site_length, site_width,
                  orif_diams, rel_humid, dis_coeff,
                  rad_src_key, not_nozzle_key,
                  loc_distributions,
                  excl_radius, rand_seed,
                  verbose=False, create_plots=True, chem_file=None, output_dir=None):
    """
    QRA flux analysis for designated positions.

    Parameters
    ----------
    rel_height : float
        Height of release (m) above floor at 0.

    rel_angle : float
        Angle of release (0 is horizontal) (radians).

    site_length : float
        Facility length (m).

    site_width : float
        Facility width (m).

    orif_diams : ndarray of floats
        Orifice leak diameters (m), one per leak size.

    rel_humid : float
        Relative humidity between 0 and 1.

    dis_coeff : float
        Leak discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow).

    rad_src_key : {'single', 'multi'}
        Radiative source model.

    not_nozzle_key : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone).

    loc_distributions : list of lists
        Parameters describing positions of workers. See _positions for more information.

    excl_radius : float
        Exclusion radius describing area around source to ignore.

    rand_seed : int
        Seed for random generation during flame calculation.

    verbose : bool
        Verbosity of logging and print statements.

    create_plots : bool
        Whether plot files should be created.

    chem_file : file
        Chem object as file, currently unused.

    output_dir : str
        File path to directory in which to create files and plots.

    Returns
    -------
    dict
        fluxes : ndarray
            [kW/m2] Heat flux data for each position, flattened into 1d array

        fluxes_by_pos : ndarray
            [kW/m2] Heat flux data for each position, p x n where p is # positions, n is # leak sizes

        all_iso_fname : list of str
            iso plot filenames for each leak size

        all_t_fname : list of str
            temp plot filenames for each leak size

        all_pos_fname : list of str
            position plot filenames for each leak size

        all_pos_files : list of str
            position plot file paths

        xlocs : ndarray
            x location of positions

        ylocs : ndarray
            y location of positions

        zlocs : ndarray
            z location of positions

        positions : ndarray
            3d positions

    """
    log.info("Flux Analysis requested")
    params = locals()
    log.info(misc_utils.params_as_str(params))

    if output_dir is None:
        output_dir = os.getcwd()

    num_sizes = len(orif_diams)
    result_dict = _flux.positional_flux_analysis(amb_fluid, rel_fluid, rel_angle, rel_height,
                                                 site_length, site_width,
                                                 orif_diams, rel_humid, dis_coeff,
                                                 rad_src_key, not_nozzle_key,
                                                 loc_distributions, excl_radius, rand_seed,
                                                 create_plots=create_plots, chem_file=chem_file,
                                                 output_dir=output_dir, verbose=False)

    log.info("Flux Analysis complete")
    return result_dict
