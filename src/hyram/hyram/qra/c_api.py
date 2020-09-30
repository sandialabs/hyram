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
import logging
import os
from logging.config import dictConfig

from . import analysis
from ..utilities import c_utils, misc_utils, exceptions


def setup(output_dir, verbose):
    """ Set up module logging globally. Called by C# GUI.

    Parameters
    ----------
    output_dir : str
        Path to directory for e.g. logging

    debug : bool
        whether debug mode active
    """
    if not os.path.isdir(output_dir):
        os.mkdir(output_dir)

    logfile = os.path.join(output_dir, 'log_hyram.txt')

    logging_config = {
        'version': 1,
        'disable_existing_loggers': False,
        'formatters': {
            'f': {'format': '%(asctime)s %(name)-12s %(levelname)-8s %(message)s'}
        },
        'handlers': {
            'h': {'class': 'logging.StreamHandler',
                  'formatter': 'f',
                  'level': logging.INFO},
            'fh': {'class': 'logging.handlers.RotatingFileHandler',
                   'formatter': 'f',
                   'level': logging.INFO,
                   'filename': logfile,
                   'mode': 'a',
                   'maxBytes': 4096000,
                   'backupCount': 10,
                   },
        },
        'root': {
            'handlers': ['h', 'fh'],
            'level': logging.INFO,
        },
    }
    dictConfig(logging_config)
    global log  # todo: cleaner way to set this up.
    log = logging.getLogger('hyram.qra.c_api')

    # Do this in separate step to keep descendant loggers at their original level
    if verbose:
        log.setLevel(logging.DEBUG)  # open-source now so expand logging
    else:
        log.setLevel(logging.ERROR)
    logging.getLogger('matplotlib.font_manager').disabled = True
    log.info("PYTHON LOGGING ACTIVATED")


def qra_analysis(pipe_length, num_compressors, num_cylinders, num_valves,
                 num_instruments, num_joints, num_hoses,
                 num_filters, num_flanges, num_extra_comp1, num_extra_comp2,
                 facil_length, facil_width, facil_height,
                 pipe_outer_diam, pipe_thickness,

                 rel_temp, rel_pres, rel_phase, amb_temp, amb_pres, discharge_coeff,
                 num_vehicles, daily_fuelings, vehicle_days,

                 immed_ign_probs, delayed_ign_probs, ign_thresholds,

                 detect_gas_flame, detection_credit,
                 probit_thermal_id, exposure_time,
                 probit_rel_id, peak_overp_list, overp_impulse_list,
                 overp_frag_mass, overp_velocity, overp_total_mass,

                 rad_source_model, nozzle_model,
                 leak_height,
                 release_angle,
                 excl_radius, rand_seed, rel_humid,

                 occupant_dist_json,

                 compressor_leak_probs,
                 cylinder_leak_probs,
                 valve_leak_probs,
                 instrument_leak_probs,
                 pipe_leak_probs,
                 joint_leak_probs,
                 hose_leak_probs,
                 filter_leak_probs,
                 flange_leak_probs,
                 extra_comp1_leak_probs,
                 extra_comp2_leak_probs,

                 noz_po_dist, noz_po_a, noz_po_b,
                 noz_ftc_dist, noz_ftc_a, noz_ftc_b,
                 mvalve_ftc_dist, mvalve_ftc_a, mvalve_ftc_b,
                 svalve_ftc_dist, svalve_ftc_a, svalve_ftc_b,
                 svalve_ccf_dist, svalve_ccf_a, svalve_ccf_b,
                 overp_dist, overp_a, overp_b,
                 pvalve_fto_dist, pvalve_fto_a, pvalve_fto_b,
                 driveoff_dist, driveoff_a, driveoff_b,
                 coupling_ftc_dist, coupling_ftc_a, coupling_ftc_b,
                 release_freq_000d01,
                 release_freq_000d10,
                 release_freq_001d00,
                 release_freq_010d00,
                 release_freq_100d00,
                 fueling_fail_freq_override,

                 output_dir,
                 print_results=False,
                 debug=False,
                 ):
    """
    Process C# inputs and execute QRA analysis.
    Advanced types must be converted to Python equivalents, including lists.
    See analysis.py for parameter descriptions, including description of return dict.

    Returns dict
    -------

    """
    log.info("C API CALL: QRA Request...")
    log.info(misc_utils.params_as_str(locals()))

    # Convert ignition probability lists to list of dicts
    immed_ign_probs = c_utils.convert_to_numpy_array(immed_ign_probs)
    delayed_ign_probs = c_utils.convert_to_numpy_array(delayed_ign_probs)
    ign_thresholds = c_utils.convert_to_numpy_array(ign_thresholds)

    ign_prob_ranges = misc_utils.convert_ign_prob_lists_to_dicts(immed_ign_probs, delayed_ign_probs, ign_thresholds)

    # Convert component leak probability sets from C# 2D array to list of dicts
    compressor_leak_probs = c_utils.convert_2d_array_to_numpy_array(compressor_leak_probs)
    compr_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(compressor_leak_probs)

    cylinder_leak_probs = c_utils.convert_2d_array_to_numpy_array(cylinder_leak_probs)
    cyl_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(cylinder_leak_probs)

    filter_leak_probs = c_utils.convert_2d_array_to_numpy_array(filter_leak_probs)
    filter_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(filter_leak_probs)

    flange_leak_probs = c_utils.convert_2d_array_to_numpy_array(flange_leak_probs)
    flange_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(flange_leak_probs)

    hose_leak_probs = c_utils.convert_2d_array_to_numpy_array(hose_leak_probs)
    hose_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(hose_leak_probs)

    joint_leak_probs = c_utils.convert_2d_array_to_numpy_array(joint_leak_probs)
    joint_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(joint_leak_probs)

    pipe_leak_probs = c_utils.convert_2d_array_to_numpy_array(pipe_leak_probs)
    pipe_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(pipe_leak_probs)

    valve_leak_probs = c_utils.convert_2d_array_to_numpy_array(valve_leak_probs)
    valve_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(valve_leak_probs)

    instrument_leak_probs = c_utils.convert_2d_array_to_numpy_array(instrument_leak_probs)
    instr_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(instrument_leak_probs)

    extra_comp1_leak_probs = c_utils.convert_2d_array_to_numpy_array(extra_comp1_leak_probs)
    extra1_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(extra_comp1_leak_probs)

    extra_comp2_leak_probs = c_utils.convert_2d_array_to_numpy_array(extra_comp2_leak_probs)
    extra2_leak_dicts = misc_utils.convert_component_prob_lists_to_dicts(extra_comp2_leak_probs)

    peak_overp_list = c_utils.convert_to_numpy_array(peak_overp_list)
    overp_impulse_list = c_utils.convert_to_numpy_array(overp_impulse_list)

    occupant_group_dicts = c_utils.convert_occupant_json_to_dicts(occupant_dist_json)
    rand_seed = int(rand_seed)

    # Ensure lower-case, alphanumeric.
    nozzle_model = misc_utils.clean_name(nozzle_model)
    rad_source_model = misc_utils.clean_name(rad_source_model)
    probit_thermal_id = misc_utils.clean_name(probit_thermal_id)
    probit_rel_id = misc_utils.clean_name(probit_rel_id)

    if fueling_fail_freq_override in ["", None, " "]:
        failure_manual_override = -1

    # Log converted lists, ids
    log.info("")
    log.info("ARRAYS:")
    log.info("Ignition data:")
    for data in ign_prob_ranges:
        log.info(data)

    log.info("Leak data:")
    for data in compr_leak_dicts:
        log.info(data)

    for data in valve_leak_dicts:
        log.info(data)

    for data in extra1_leak_dicts:
        log.info(data)

    log.info("")
    log.info("Occupants:")
    for data in occupant_group_dicts:
        log.info(data)

    log.info("Peak overp {}".format(peak_overp_list))
    log.info("Impulse: {}".format(overp_impulse_list))
    log.info("")
    log.info("probit_thermal_id {}".format(probit_thermal_id))
    log.info("probit_rel_id {}".format(probit_rel_id))
    log.info("rad_source_model {}".format(rad_source_model))
    log.info("nozzle_model {}".format(nozzle_model))

    results = {"status": False, "data": None, "message": None}

    try:
        analysis_dict = analysis.conduct_analysis(
                pipe_outer_diam=pipe_outer_diam, pipe_thickness=pipe_thickness,
                amb_temp=amb_temp, amb_pres=amb_pres,
                rel_temp=rel_temp, rel_pres=rel_pres, rel_phase=rel_phase,
                facil_length=facil_length, facil_width=facil_width, facil_height=facil_height,
                num_vehicles=num_vehicles, daily_fuelings=daily_fuelings, vehicle_days=vehicle_days,
                ign_prob_ranges=ign_prob_ranges,
                occupant_input_list=occupant_group_dicts,
                discharge_coeff=discharge_coeff, detect_gas_flame=detect_gas_flame, detection_credit=detection_credit,
                probit_thermal_id=probit_thermal_id, exposure_time=exposure_time, probit_rel_id=probit_rel_id,
                rad_source_model=rad_source_model, nozzle_model=nozzle_model,
                leak_height=leak_height, rel_angle=release_angle, rel_humid=rel_humid,
                rand_seed=rand_seed, excl_radius=excl_radius,
                overp_frag_mass=overp_frag_mass, overp_velocity=overp_velocity, overp_total_mass=overp_total_mass,
                peak_overp_list=peak_overp_list, overp_impulse_list=overp_impulse_list,

                compressor_leak_probs=compr_leak_dicts,
                cylinder_leak_probs=cyl_leak_dicts,
                valve_leak_probs=valve_leak_dicts,
                instrument_leak_probs=instr_leak_dicts,
                pipe_leak_probs=pipe_leak_dicts,
                joint_leak_probs=joint_leak_dicts,
                hose_leak_probs=hose_leak_dicts,
                filter_leak_probs=filter_leak_dicts,
                flange_leak_probs=flange_leak_dicts,
                extra_comp1_leak_probs=extra1_leak_dicts,
                extra_comp2_leak_probs=extra2_leak_dicts,

                pipe_length=pipe_length,
                num_compressors=num_compressors, num_cylinders=num_cylinders, num_valves=num_valves,
                num_instruments=num_instruments, num_joints=num_joints, num_hoses=num_hoses,
                num_filters=num_filters, num_flanges=num_flanges,
                num_extra_comp1=num_extra_comp1,
                num_extra_comp2=num_extra_comp2,

                noz_po_dist=noz_po_dist, noz_po_a=noz_po_a, noz_po_b=noz_po_b,
                noz_ftc_dist=noz_ftc_dist, noz_ftc_a=noz_ftc_a, noz_ftc_b=noz_ftc_b,
                mvalve_ftc_dist=mvalve_ftc_dist, mvalve_ftc_a=mvalve_ftc_a,
                mvalve_ftc_b=mvalve_ftc_b,
                svalve_ftc_dist=svalve_ftc_dist, svalve_ftc_a=svalve_ftc_a,
                svalve_ftc_b=svalve_ftc_b,
                svalve_ccf_dist=svalve_ccf_dist, svalve_ccf_a=svalve_ccf_a,
                svalve_ccf_b=svalve_ccf_b,
                overp_dist=overp_dist, overp_a=overp_a, overp_b=overp_b,
                pvalve_fto_dist=pvalve_fto_dist, pvalve_fto_a=pvalve_fto_a,
                pvalve_fto_b=pvalve_fto_b,
                driveoff_dist=driveoff_dist, driveoff_a=driveoff_a, driveoff_b=driveoff_b,
                coupling_ftc_dist=coupling_ftc_dist, coupling_ftc_a=coupling_ftc_a,
                coupling_ftc_b=coupling_ftc_b,
                release_freq_000d01=release_freq_000d01,
                release_freq_000d10=release_freq_000d10,
                release_freq_001d00=release_freq_001d00,
                release_freq_010d00=release_freq_010d00,
                release_freq_100d00=release_freq_100d00,
                fueling_fail_freq_override=fueling_fail_freq_override,

                print_results=print_results, debug=debug, output_dir=output_dir, create_plots=True
        )

        for key, val in analysis_dict.items():
            print(key, val)

        # convert objects to dicts
        leak_results = analysis_dict['leak_results']
        leak_result_dicts = [vars(leak_result) for leak_result in leak_results]
        analysis_dict['leak_results'] = leak_result_dicts

        log.info("\n LEAK RESULT DICTS:")
        for leak_result_dict in leak_result_dicts:
            log.info(leak_result_dict)

        results['status'] = True
        results['data'] = analysis_dict

    except ValueError as exc:
        # results["message"] = exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG
        # log.error(exceptions.LIQUID_RELEASE_PRESSURE_INVALID_MSG)
        msg = "Invalid input. {}".format(str(exc))
        results["message"] = msg
        log.error(msg)
        log.exception(exc)

    except exceptions.InputError as exc:
        msg = "Invalid input: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)
        log.exception(exc)

    except Exception as exc:
        msg = "Analysis failed: {}".format(str(exc))
        results["message"] = msg
        log.exception(exc)

    finally:
        logging.shutdown()
        gc.collect()
        return results
