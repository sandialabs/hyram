"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
import os
import warnings

import gc
import numpy as np

from . import api
from ..utilities import c_utils, misc_utils, exceptions, custom_warnings


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
    misc_utils.setup_file_log(output_dir, verbose=verbose, logname=__name__)


def etk_compute_mass_flow_rate(species, temp, pres, phase, orif_diam,
                               is_steady, tank_vol, dis_coeff, output_dir):
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
    amb_pres = 101325.
    results = {"status": False, "data": None, "message": None}

    if pres is not None and pres < amb_pres:
        msg = "Error during calculation: fluid pressure is less than ambient pressure (1 atm)"
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


def etk_compute_tank_mass(species, temp, pres, phase, tank_vol):
    """
    Process GUI request for tank mass calculation.

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
    api.create_fluid
    api.compute_tank_mass

    """
    log.info("Initializing CAPI: ETK tank mass...")
    log.info(misc_utils.params_as_str(locals()))
    results = {"status": False, "data": None, "message": None}

    try:
        fluid = api.create_fluid(species, temp, pres, density=None, phase=phase)
        mass = api.compute_tank_mass(fluid, tank_vol)
        results["data"] = mass
        results["status"] = True

        log.info("RESULTS: {}".format(mass))

    except exceptions.InputError as exc:
        msg = "Calculation failed due to invalid inputs: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)

    except Exception as exc:
        msg = "Tank mass calculation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(exc)

    finally:
        gc.collect()
        return results


def etk_compute_thermo_param(species, temp, pres, density):
    """
    Process GUI request for various thermo calculations, e.g. pressure.

    Returns
    ----------
    result : dict
        status : bool
            True if call successful.
        message : str or None
            Contains error message if call fails.
        data : float
            Requested parameter, i.e. whichever was None. Temp (K), Pressure (Pa), density (kg/m3).

    See Also
    --------
    api.compute_thermo_param

    """
    log.info("Initializing CAPI: ETK TPD calculation...")
    params = locals()
    log.info(misc_utils.params_as_str(params))
    results = {"status": False, "data": None, "message": None}

    try:
        param_value = api.compute_thermo_param(species, temp, pres, density)
        log.info("Result: {}".format(param_value))
        results["data"] = param_value
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


def etk_compute_equivalent_tnt_mass(vapor_mass, percent_yield, fuel):
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
        mass = api.compute_equivalent_tnt_mass(vapor_mass, percent_yield, fuel)
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
                      orif_diam, rel_angle, dis_coeff, nozzle_model,
                      contour, xmin, xmax, ymin, ymax,
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

    try:
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = api.create_fluid('AIR', amb_temp, amb_pres)
            rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, density=None, phase=rel_phase)

            data_dict = api.analyze_jet_plume(amb_fluid, rel_fluid, orif_diam,
                                              rel_angle=rel_angle, dis_coeff=dis_coeff, nozzle_model=nozzle_model,
                                              create_plot=True, contour=contour,
                                              xmin=xmin, xmax=xmax, ymin=ymin, ymax=ymax, plot_title=plot_title,
                                              output_dir=output_dir, verbose=verbose)

            log.info("File path: {}".format(data_dict['plot']))
            results["data"] = data_dict
            results["status"] = True

            for wrng in warning_list:
                if wrng.category is custom_warnings.PhysicsWarning:
                    results["warning"] = str(wrng.message)

    except exceptions.InputError as exc:
        msg = "Plume plot generation failed due to InputError: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)

    except ValueError as exc:
        results["message"] = exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG
        log.error(exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG)

    except Exception as exc:
        msg = "Plume plot generation failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

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
        times = c_utils.convert_to_numpy_array(times)

    # Optional array params
    if pt_pressures is not None:
        pt_pressures = c_utils.convert_to_numpy_array(pt_pressures)

    if pt_times is not None:
        pt_times = c_utils.convert_to_numpy_array(pt_times)

    if pt_pressures is None or pt_times is None:
        temp_pres_points = None
    else:
        temp_pres_points = np.array([pt_times, pt_pressures]).T

    if pres_ticks is not None:
        pres_ticks = c_utils.convert_to_numpy_array(pres_ticks)

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
        results["message"] = exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG
        log.exception(exc)

    except Exception as exc:
        msg = "Accumulation analysis failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results


def jet_flame_analysis(amb_temp, amb_pres,
                       rel_species, rel_temp, rel_pres, rel_phase,
                       orif_diam,
                       rel_angle, rel_height,
                       nozzle_key, rad_src_key, rel_humid,
                       xpos, ypos, zpos,
                       contours,
                       analyze_flux=True,
                       output_dir=None, verbose=False):
    """
    Analyze heat flux and generate corresponding plots.
    See analyses.rad_heat_flux_analysis for parameter descriptions.
    """
    log.info("Initializing CAPI: jet flame analysis...")
    params = locals()
    log.info(misc_utils.params_as_str(params))

    results = {"status": False, "data": None, "message": None, "warning": ""}

    # Check if chem file found. If not found, pass None; will be initialized later.
    chem_filename = 'flux_chem.pkl'
    chem_filepath = os.path.join(output_dir, chem_filename)
    # Maker sure file actually exists, otherwise clear it
    if not os.path.isfile(chem_filepath):
        chem_filepath = None

    if chem_filepath:
        log.info("Chem file found: {}".format(chem_filepath))
    else:
        log.info("Chem file not found")

    if xpos is not None:
        xpos = c_utils.convert_to_numpy_array(xpos)

    if ypos is not None:
        ypos = c_utils.convert_to_numpy_array(ypos)

    if zpos is not None:
        zpos = c_utils.convert_to_numpy_array(zpos)

    if contours is not None:
        contours = c_utils.convert_to_numpy_array(contours)

    log.info("Flux X: {}".format(xpos))
    log.info("Flux Y: {}".format(ypos))
    log.info("Flux Z: {}".format(zpos))
    log.info("Contours: {}".format(contours))

    try:
        # Generate flux data and 2d slice plot
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = api.create_fluid('AIR', amb_temp, amb_pres)
            rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, density=None, phase=rel_phase)

            (temp_plot_filepath, _, flux2d_filepath, flux_data,
             mass_flow, srad) = api.jet_flame_analysis(
                    amb_fluid, rel_fluid, orif_diam,
                    rel_angle=rel_angle, rel_height=rel_height,
                    nozzle_key=nozzle_key, rad_src_key=rad_src_key, rel_humid=rel_humid, contours=contours,
                    create_temp_plot=True, analyze_flux=analyze_flux, create_3dplot=False,
                    xpos=xpos, ypos=ypos, zpos=zpos,
                    chem_filepath=chem_filepath,
                    output_dir=output_dir, verbose=verbose)

            output_dict = {
                'flux_data': flux_data,
                'flux_plot_filepath': flux2d_filepath,
                'temp_plot_filepath': temp_plot_filepath,
                'mass_flow_rate': mass_flow,
                'srad': srad
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
        results["message"] = exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG
        log.error(exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG)

    except Exception as exc:
        msg = "jet flame analysis failed: {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    finally:
        gc.collect()
        return results


def unconfined_overpressure_analysis(amb_temp, amb_pres,
                                     rel_species, rel_temp, rel_pres, rel_phase,
                                     orif_diam, rel_angle, discharge_coeff, nozzle_model, method,
                                     xlocs, ylocs, zlocs,
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

    See Also
    --------
    api.compute_overpressure

    """
    log.info("C API CALL: overpressure calculation ...")
    params = locals()
    log.info(misc_utils.params_as_str(params))
    results = {"status": False, "data": None, "message": None, "warning": ""}

    xlocs = c_utils.convert_to_numpy_array(xlocs)
    ylocs = c_utils.convert_to_numpy_array(ylocs)
    zlocs = c_utils.convert_to_numpy_array(zlocs)
    method = method.lower()
    
    try:
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = api.create_fluid('AIR', amb_temp, amb_pres)
            rel_fluid = api.create_fluid(rel_species, rel_temp, rel_pres, density=None, phase=rel_phase)
            locations = np.array([xlocs, ylocs, zlocs]).T

            data = api.compute_overpressure(method=method, locations=locations,
                                            ambient_fluid=amb_fluid, release_fluid=rel_fluid,
                                            orifice_diameter=orif_diam, release_angle=rel_angle,
                                            discharge_coefficient=discharge_coeff, nozzle_model=nozzle_model,
                                            heat_of_combustion=None,
                                            BST_mach_flame_speed=bst_flame_speed, TNT_equivalence_factor=tnt_factor,
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







