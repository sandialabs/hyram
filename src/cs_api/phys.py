"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
import warnings
import gc

import numpy as np
from . import utils, exceptions

try:
    from hyram.phys import api
    from hyram.utilities import misc_utils, custom_warnings
except ModuleNotFoundError as err:
    from hyram.hyram.phys import api
    from hyram.hyram.utilities import misc_utils, custom_warnings


log = logging.getLogger(__name__)

"""
The C API provides access to the physics module with suitable pre-processing for the C# GUI inputs.
This file should call the api file and should not be called by any other functions except external GUIs.

Note that, by design, these functions take no optional parameters. Parameters and return values are hard-coded to match
the GUI. Detailed docstrings can be found in the API file or in respective analysis functions.

To ensure errors are handled in GUI, all return parameters are wrapped in dict:
    status : bool
        True if call succeeded without error; False otherwise
    data : float, dict, or None
        actual analysis output/result(s) or None if error occurred
    message : str or None
        Error message, if any, or None if successful
    
"""


def setup(output_dir, verbose):
    """ Set up module logging globally.

    Parameters
    ----------
    output_dir : str
        Path to log directory

    verbose : bool
        Determine level of logging
    """
    utils.setup_file_log(output_dir, verbose=verbose, logname=__name__)


def etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                               is_steady, tank_vol, dis_coeff, amb_pres, output_dir):
    """
    Process GUI request for mass flow calculation.

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : dict
            mass_flow_rate : float
                Mass flow rate (kg/s) of steady release. Only present if is_steady is true.
            time_to_empty : float
                (s) time it takes to blowdown the tank to empty. Present if is_steady is false.
            plot : str
                path to plot of mass flow rate vs. time. Only created if Steady is false. Present if is_steady is false.
            times : array of floats
                Times at which mass flow rates occur during blowdown. Present if is_steady is false.
            rates : array of floats
                Mass flow rates during blowdown. Present if is_steady is false.

    See Also
    --------
    api.compute_mass_flow_rate

    """
    log.info("Initializing CAPI: ETK mass flow analysis...")
    params = locals()
    log.info(misc_utils.params_as_str(params))
    results = {"status": False, "data": None, "message": None}

    if pres is not None and pres < amb_pres:
        msg = "Error during calculation: fluid pressure is less than ambient pressure"
        results["message"] = msg
        log.error(msg)
        return results

    try:
        fluid = api.create_fluid(species, temp, pres, None, phase)
        result_dict = api.compute_mass_flow(fluid, orif_diam, amb_pres,
                                            is_steady, tank_vol, dis_coeff, output_dir, create_plot=True)
        results["data"] = result_dict
        results["status"] = True
        log.info("RESULTS: {}".format(result_dict))

    except exceptions.InputError as exc:
        msg = "calculation failed due to invalid inputs: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)

    except Exception as exc:
        msg = "Mass flow calculation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results


def etk_compute_tank_mass_param(species, temp, pres, phase, vol, mass):
    """
    Process GUI request for tank mass parameter calculation.

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : float
            Mass of tank (kg)

    See Also
    --------
    api.compute_tank_mass_param

    """
    log.info("Initializing CAPI: ETK tank mass parameter calculation...")
    log.info(misc_utils.params_as_str(locals()))
    results = {"status": False, "data": None, "message": None}

    try:
        param1, param2 = api.compute_tank_mass_param(species, phase, temp, pres, vol, mass)
        log.info(f"Results: {param1}, {param2}")
        results["data"] = {'param1': param1, 'param2': param2}
        results["status"] = True

    except exceptions.InputError as exc:
        msg = "Calculation failed due to invalid inputs: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)

    except Exception as exc:
        msg = "Tank mass parameter calculation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(exc)

    finally:
        gc.collect()
        return results


def etk_compute_thermo_param(species, phase, temp, pres, density):
    """
    Process GUI request for various thermo calculations, e.g. pressure.

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : dict of floats or float and None
            {param1, param2}
            Requested parameter, i.e. whichever was None. Temp (K), Pressure (Pa), density (kg/m3).
            Second param is temp if phase is saturated. If unsaturated, first is param and second is None.

    See Also
    --------
    api.compute_thermo_param

    """
    log.info("Initializing CAPI: ETK TPD calculation...")
    params = locals()
    log.info(misc_utils.params_as_str(params))
    results = {"status": False, "data": None, "message": None}

    try:
        param1, param2 = api.compute_thermo_param(species, phase, temp, pres, density)
        log.info(f"Results: {param1}, {param2}")
        results["data"] = {'param1': param1, 'param2': param2}
        results["status"] = True

    except exceptions.InputError as exc:
        msg = "TPD calculation failed due to InputError: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    except Exception as exc:
        msg = "TPD calculation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results


def etk_compute_equivalent_tnt_mass(vapor_mass, percent_yield, species):
    """
    Process GUI request for computing equivalent mass of TNT.

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : float
            Equivalent mass (kg)

    See Also
    --------
    api.compute_equivalent_tnt_mass

    """
    log.info("Initializing CAPI: ETK TNT mass...")
    log.info(misc_utils.params_as_str(locals()))
    results = {"status": False, "data": None, "message": None}

    try:
        mass = api.compute_equivalent_tnt_mass(vapor_mass, percent_yield, species)
        results["data"] = mass
        results["status"] = True

        log.info("RESULT: {}".format(mass))

    except exceptions.InputError as exc:
        msg = "Calculation failed due to invalid inputs: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)

    except Exception as exc:
        msg = "TNT mass calculation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results


def analyze_jet_plume(amb_temp, amb_pres,
                      rel_species, rel_temp, rel_pres, rel_phase,
                      orif_diam, mass_flow, rel_angle, dis_coeff, nozzle_model,
                      contours, xmin, xmax, ymin, ymax, vmin, vmax,
                      plot_title, output_dir, verbose):
    """
    Create plume plot for leak.

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : dict
            jet plume data and arrays, including 'plot' file path

    See Also
    --------
    api.create_fluid
    api.analyze_jet_plume

    """
    log.info("Initializing CAPI: plume plot generation...")
    params = locals()
    log.info(misc_utils.params_as_str(params))
    results = {"status": False, "data": None, "message": None, "warning": ""}

    nozzle_model = misc_utils.parse_nozzle_model(nozzle_model)

    if contours is not None:
        if type(contours) == float:
            contours = [contours]
        contours = utils.convert_to_numpy_array(contours)
        if len(contours) == 0:
            contours = None

    try:
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = api.create_fluid('AIR', amb_temp, amb_pres)
            rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, density=None, phase=rel_phase)

            data_dict = api.analyze_jet_plume(amb_fluid, rel_fluid, orif_diam, mass_flow=mass_flow,
                                              rel_angle=rel_angle, dis_coeff=dis_coeff, nozzle_model=nozzle_model,
                                              create_plot=True, contours=contours,
                                              xmin=xmin, xmax=xmax, ymin=ymin, ymax=ymax,
                                              vmin=vmin, vmax=vmax,
                                              plot_title=plot_title,
                                              output_dir=output_dir, verbose=verbose)

            # Parse contour data into lists of floats
            # ordered_contours is sorted list of contour values
            # dists is x and y min-max values for each contour. Each sub-list is [x1, x2, y1, y2]
            contour_dicts = data_dict['mole_frac_dists']
            if contour_dicts:
                ordered_contours = []
                contour_dists = []
                for key, xys in contour_dicts.items():
                    ordered_contours.append(key)
                    contour_dists.append([xys[0][0], xys[0][1], xys[1][0], xys[1][1]])
                data_dict['ordered_contours'] = ordered_contours
                data_dict['mole_frac_dists'] = contour_dists

            log.info("results: {}".format(data_dict))
            results["data"] = data_dict
            results["status"] = True

            for wrng in warning_list:
                if wrng.category is custom_warnings.PhysicsWarning:
                    results["warning"] = str(wrng.message)

    except exceptions.InputError as exc:
        msg = "Plume plot generation failed due to InputError: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)
        log.exception(exc)

    except ValueError as exc:
        err_msg = str(exc)
        if len(err_msg) > 5:
            results["message"] = err_msg
        else:
            results["message"] = exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG
        log.error(results["message"])

    except Exception as exc:
        msg = "Plume plot generation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)
        log.exception(exc)

    finally:
        gc.collect()
        return results


def analyze_accumulation(amb_temp, amb_pres,
                         rel_species, rel_temp, rel_pres, rel_phase,
                         tank_volume, orif_diam, rel_height,
                         enclos_height, floor_ceil_area,
                         ceil_vent_xarea, ceil_vent_height,
                         floor_vent_xarea, floor_vent_height,
                         times,
                         orif_dis_coeff,
                         vol_flow_rate, dist_rel_to_wall,
                         tmax, rel_angle, nozzle_key,
                         pt_pressures, pt_times, pres_ticks,
                         is_steady,
                         output_dir=None, verbose=False):
    """ Conduct indoor release analysis. See indoor_release for input descriptions.

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : dict
            Indoor release data, including temporal data and plot file paths.

    See Also
    --------
    api.create_fluid
    api.analyze_indoor_release
    """
    log.info("Initializing CAPI: Accumulation analysis...")
    params = locals()
    log.info(misc_utils.params_as_str(params))

    results = {"status": False, "data": None, "message": None, "warning": ""}

    if type(times) != np.ndarray:
        times = utils.convert_to_numpy_array(times)

    # Optional array params
    if pt_pressures is not None:
        pt_pressures = utils.convert_to_numpy_array(pt_pressures)

    if pt_times is not None:
        pt_times = utils.convert_to_numpy_array(pt_times)

    if pt_pressures is None or pt_times is None:
        temp_pres_points = None
    else:
        temp_pres_points = np.array([pt_times, pt_pressures]).T

    if pres_ticks is not None:
        pres_ticks = utils.convert_to_numpy_array(pres_ticks)

    log.info("limit line pressures: {}".format(pres_ticks))
    log.info("times: {}".format(times))
    log.info("dot mark pressures: {}".format(pt_pressures))
    log.info("dot mark times: {}".format(pt_times))

    try:
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = api.create_fluid('AIR', amb_temp, amb_pres)
            rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, density=None, phase=rel_phase)

            result_dict = api.analyze_accumulation(amb_fluid, rel_fluid,
                                                   tank_volume, orif_diam, rel_height,
                                                   enclos_height, floor_ceil_area,
                                                   ceil_vent_xarea, ceil_vent_height,
                                                   floor_vent_xarea, floor_vent_height,
                                                   times, orif_dis_coeff=orif_dis_coeff,
                                                   vol_flow_rate=vol_flow_rate, dist_rel_to_wall=dist_rel_to_wall,
                                                   tmax=tmax, rel_area=None, rel_angle=rel_angle,
                                                   nozzle_key=nozzle_key,
                                                   temp_pres_points=temp_pres_points, pres_ticks=pres_ticks,
                                                   is_steady=is_steady,
                                                   create_plots=True, output_dir=output_dir, verbose=verbose)
            results["data"] = result_dict
            results["status"] = True

            log.info("RESULTS:".format(results))
            for key, val in result_dict.items():
                log.info("{}: {}".format(key, val))

            for wrng in warning_list:
                if wrng.category is custom_warnings.PhysicsWarning:
                    results["warning"] = str(wrng.message)

    except exceptions.InputError as exc:
        msg = "Accumulation analysis failed due to InputError: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    except ValueError as exc:
        err_msg = str(exc)
        if len(err_msg) > 5:
            results["message"] = err_msg
        else:
            results["message"] = exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG
        log.error(results["message"])

    except Exception as exc:
        msg = "Accumulation analysis failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results


def jet_flame_analysis(amb_temp, amb_pres,
                       rel_species, rel_temp, rel_pres, rel_phase,
                       orif_diam, mass_flow, discharge_coeff,
                       rel_angle, nozzle_key, rel_humid,

                       temp_plot_filename, temp_plot_title, temp_contours,
                       txmin, txmax, tymin, tymax,

                       analyze_flux, xpos, ypos, zpos, flux_contours,
                       fxmin, fxmax, fymin, fymax, fzmin, fzmax,

                       output_dir=None, verbose=False):
    """
    Analyze heat flux and generate corresponding plots.
    See analyses.rad_heat_flux_analysis for parameter descriptions.
    """
    log.info("Initializing CAPI: jet flame analysis...")
    params = locals()
    log.info(misc_utils.params_as_str(params))

    results = {"status": False, "data": None, "message": None, "warning": ""}

    if xpos is not None:
        xpos = utils.convert_to_numpy_array(xpos)
    if ypos is not None:
        ypos = utils.convert_to_numpy_array(ypos)
    if zpos is not None:
        zpos = utils.convert_to_numpy_array(zpos)

    if temp_contours is not None:
        temp_contours = utils.convert_to_numpy_array(temp_contours)
        if len(temp_contours) == 0:
            temp_contours = None

    if flux_contours is not None:
        flux_contours = utils.convert_to_numpy_array(flux_contours)
        if len(flux_contours) == 0:
            flux_contours = None

    temp_xlims = None if txmin is None and txmax is None else (txmin, txmax)
    temp_ylims = None if tymin is None and tymax is None else (tymin, tymax)

    # flux plot requires both to define limit
    flux_xlims = None if fxmin is None or fxmax is None or fxmin >= fxmax else (fxmin, fxmax)
    flux_ylims = None if fymin is None or fymax is None or fymin >= fymax else (fymin, fymax)
    flux_zlims = None if fzmin is None or fzmax is None or fzmin >= fzmax else (fzmin, fzmax)

    log.info("Temperature contours: {}".format(temp_contours))
    log.info("Flux X: {}".format(xpos))
    log.info("Flux Y: {}".format(ypos))
    log.info("Flux Z: {}".format(zpos))
    log.info("Flux contours: {}".format(flux_contours))

    try:
        # Generate flux data and 2d slice plot
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = api.create_fluid('AIR', amb_temp, amb_pres)
            rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, density=None, phase=rel_phase)
            flux_coordinates = np.array([xpos, ypos, zpos]).T if xpos is not None else None

            (temp_plot_filepath,
             heatflux_filepath,
             flux_data,
             mass_flow,
             srad,
             visible_length,
             radiant_frac
             ) = api.jet_flame_analysis(amb_fluid, rel_fluid, orif_diam, mass_flow=mass_flow,
                                        dis_coeff=discharge_coeff,
                                        rel_angle=rel_angle,
                                        nozzle_key=nozzle_key, rel_humid=rel_humid,

                                        create_temp_plot=True, temp_plot_filename=None,
                                        temp_plot_title=temp_plot_title, temp_contours=temp_contours,
                                        temp_xlims=temp_xlims, temp_ylims=temp_ylims,

                                        analyze_flux=analyze_flux, flux_plot_filename=None,
                                        flux_coordinates=flux_coordinates, flux_contours=flux_contours,
                                        flux_xlims=flux_xlims, flux_ylims=flux_ylims, flux_zlims=flux_zlims,

                                        output_dir=output_dir, verbose=verbose)

            flux_data_kwm2 = [flux / 1000 for flux in flux_data] if flux_data is not None else None

            output_dict = {
                'flux_data': flux_data_kwm2,
                'flux_plot_filepath': heatflux_filepath,
                'temp_plot_filepath': temp_plot_filepath,
                'mass_flow_rate': mass_flow,
                'srad': srad,
                'visible_length': visible_length,
                'radiant_frac': radiant_frac,
            }

            log.info("Result: {}".format(output_dict))
            results['data'] = output_dict
            results["status"] = True

            for wrng in warning_list:
                if wrng.category is custom_warnings.PhysicsWarning:
                    results["warning"] = str(wrng.message)

    except exceptions.InputError as exc:
        msg = "jet flame analysis failed due to InputError: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    except ValueError as exc:
        err_msg = str(exc)
        if len(err_msg) > 5:
            results["message"] = err_msg
        else:
            results["message"] = exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG
        log.error(results["message"])

    except Exception as exc:
        msg = "jet flame analysis failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results


def unconfined_overpressure_analysis(amb_temp, amb_pres,
                                     rel_species, rel_temp, rel_pres, rel_phase,
                                     orif_diam, mass_flow, rel_angle, discharge_coeff, nozzle_model, method,
                                     xlocs, ylocs, zlocs,
                                     overp_contours,
                                     oxmin, oxmax, oymin, oymax, ozmin, ozmax,
                                     impulse_contours,
                                     ixmin, ixmax, iymin, iymax, izmin, izmax,
                                     bst_flame_speed, tnt_factor,
                                     output_dir=None, verbose=False):
    """
    Calculate the overpressure and impulse at specified x,y,z locations

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : dict
            overpressure value(s) [Pa]
            impulse value(s) [Pa*s]
            plot filepath
            mass flow rate [kg/s]

    See Also
    --------
    api.compute_overpressure

    """
    log.info("C API CALL: overpressure calculation ...")
    params = locals()
    log.info(misc_utils.params_as_str(params))
    results = {"status": False, "data": None, "message": None, "warning": ""}

    xlocs = utils.convert_to_numpy_array(xlocs)
    ylocs = utils.convert_to_numpy_array(ylocs)
    zlocs = utils.convert_to_numpy_array(zlocs)
    method = method.lower()

    overp_xlims = None if oxmin is None or oxmax is None or oxmin >= oxmax else (oxmin, oxmax)
    overp_ylims = None if oymin is None or oymax is None or oymin >= oymax else (oymin, oymax)
    overp_zlims = None if ozmin is None or ozmax is None or ozmin >= ozmax else (ozmin, ozmax)

    impulse_xlims = None if ixmin is None or ixmax is None or ixmin >= ixmax else (ixmin, ixmax)
    impulse_ylims = None if iymin is None or iymax is None or iymin >= iymax else (iymin, iymax)
    impulse_zlims = None if izmin is None or izmax is None or izmin >= izmax else (izmin, izmax)

    if overp_contours is not None:
        overp_contours = utils.convert_to_numpy_array(overp_contours)
        if len(overp_contours) == 0:
            overp_contours = None

    if impulse_contours is not None:
        impulse_contours = utils.convert_to_numpy_array(impulse_contours)
        if len(impulse_contours) == 0:
            impulse_contours = None

    try:
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = api.create_fluid('AIR', amb_temp, amb_pres)
            rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, density=None, phase=rel_phase)
            locations = np.array([xlocs, ylocs, zlocs]).T

            data = api.compute_overpressure(method=method, locations=locations,
                                            ambient_fluid=amb_fluid, release_fluid=rel_fluid,
                                            orifice_diameter=orif_diam, mass_flow=mass_flow, release_angle=rel_angle,
                                            discharge_coefficient=discharge_coeff, nozzle_model=nozzle_model,
                                            bst_flame_speed=bst_flame_speed, tnt_factor=tnt_factor,
                                            overp_contours=overp_contours,
                                            overp_xlims=overp_xlims, overp_ylims=overp_ylims, overp_zlims=overp_zlims,
                                            impulse_contours=impulse_contours,
                                            impulse_xlims=impulse_xlims, impulse_ylims=impulse_ylims,
                                            impulse_zlims=impulse_zlims,
                                            create_overpressure_plot=True, output_dir=output_dir, verbose=verbose
                                            )

            log.info("API CALL COMPLETED SUCCESSFULLY")
            log.info("Result: {}".format(data))
            results["data"] = data
            results["status"] = True

            for wrng in warning_list:
                if wrng.category is custom_warnings.PhysicsWarning:
                    results["warning"] = str(wrng.message)

    except exceptions.InputError as exc:
        msg = "Overpressure calculation failed due to InputError: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    except Exception as exc:
        msg = "Overpressure calculation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results
