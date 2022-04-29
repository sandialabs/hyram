"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import gc
import logging
import os

from . import analysis, component_failure
from .component_set import ComponentSet
from ..utilities import c_utils, misc_utils, exceptions

log = logging.getLogger(__name__)


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


def c_request_analysis(pipe_length,
                       num_compressors, num_vessels, num_valves,
                       num_instruments, num_joints, num_hoses,
                       num_filters, num_flanges,
                       num_exchangers, num_vaporizers, num_arms,
                       num_extra_comp1, num_extra_comp2,

                       facil_length, facil_width,
                       pipe_outer_diam, pipe_thickness,

                       rel_species, rel_temp, rel_pres, rel_phase,
                       amb_temp, amb_pres,
                       discharge_coeff,
                       num_vehicles, daily_fuelings, vehicle_days,

                       immed_ign_probs,
                       delayed_ign_probs,
                       ign_thresholds,

                       detection_credit,
                       overp_method,
                       TNT_equivalence_factor,
                       BST_mach_flame_speed,
                       probit_thermal_id, exposure_time,
                       probit_overp_id,

                       nozzle_model,
                       release_angle,
                       excl_radius,
                       rand_seed,
                       rel_humid,

                       occupant_dist_json,

                       compressor_leak_probs, vessel_leak_probs, valve_leak_probs,
                       instrument_leak_probs, pipe_leak_probs, joint_leak_probs,
                       hose_leak_probs, filter_leak_probs, flange_leak_probs,
                       exchanger_leak_probs, vaporizer_leak_probs, arm_leak_probs,
                       extra_comp1_leak_probs, extra_comp2_leak_probs,

                       noz_po_dist, noz_po_a, noz_po_b,
                       noz_ftc_dist, noz_ftc_a, noz_ftc_b,
                       mvalve_ftc_dist, mvalve_ftc_a, mvalve_ftc_b,
                       svalve_ftc_dist, svalve_ftc_a, svalve_ftc_b,
                       svalve_ccf_dist, svalve_ccf_a, svalve_ccf_b,
                       overp_dist, overp_a, overp_b,
                       pvalve_fto_dist, pvalve_fto_a, pvalve_fto_b,
                       driveoff_dist, driveoff_a, driveoff_b,
                       coupling_ftc_dist, coupling_ftc_a, coupling_ftc_b,
                       rel_freq_000d01,
                       rel_freq_000d10,
                       rel_freq_001d00,
                       rel_freq_010d00,
                       rel_freq_100d00,
                       fueling_fail_freq_override,

                       output_dir, verbose=False,
                       ):
    """
    Primary GUI access point for QRA analysis.
    Pre-process C# inputs before executing analysis.
    Advanced types must be converted to Python equivalents, including lists.
    See analysis.py for parameter descriptions, including description of return dict.

    Returns
    -------
    results : dict
        status : boolean
            boolean indicating whether the analysis was successfull
        message : string
            error message if status is False
        data : dict
            Compilation of QRA analysis results
            See analysis.py for details
    """
    log.info("Initializing CAPI: QRA Request...")

    immed_ign_probs = list(c_utils.convert_to_numpy_array(immed_ign_probs))
    delayed_ign_probs = list(c_utils.convert_to_numpy_array(delayed_ign_probs))
    ign_thresholds = list(c_utils.convert_to_numpy_array(ign_thresholds))

    occupant_group_dicts = c_utils.convert_occupant_json_to_dicts(occupant_dist_json)

    # Convert component leak probability sets from C# 2D array to np 2d arrays
    compressor_leak_probs = c_utils.convert_2d_array_to_numpy_array(compressor_leak_probs)
    vessel_leak_probs = c_utils.convert_2d_array_to_numpy_array(vessel_leak_probs)
    filter_leak_probs = c_utils.convert_2d_array_to_numpy_array(filter_leak_probs)
    flange_leak_probs = c_utils.convert_2d_array_to_numpy_array(flange_leak_probs)
    hose_leak_probs = c_utils.convert_2d_array_to_numpy_array(hose_leak_probs)
    joint_leak_probs = c_utils.convert_2d_array_to_numpy_array(joint_leak_probs)
    pipe_leak_probs = c_utils.convert_2d_array_to_numpy_array(pipe_leak_probs)
    valve_leak_probs = c_utils.convert_2d_array_to_numpy_array(valve_leak_probs)
    instrument_leak_probs = c_utils.convert_2d_array_to_numpy_array(instrument_leak_probs)
    exchanger_leak_probs = c_utils.convert_2d_array_to_numpy_array(exchanger_leak_probs)
    vaporizer_leak_probs = c_utils.convert_2d_array_to_numpy_array(vaporizer_leak_probs)
    arm_leak_probs = c_utils.convert_2d_array_to_numpy_array(arm_leak_probs)
    extra_comp1_leak_probs = c_utils.convert_2d_array_to_numpy_array(extra_comp1_leak_probs)
    extra_comp2_leak_probs = c_utils.convert_2d_array_to_numpy_array(extra_comp2_leak_probs)

    results = {"status": False, "data": None, "message": None}

    try:
        analysis_dict = request_analysis(pipe_length=pipe_length,
                                         num_compressors=num_compressors, num_vessels=num_vessels, num_valves=num_valves,
                                         num_instruments=num_instruments, num_joints=num_joints, num_hoses=num_hoses,
                                         num_filters=num_filters, num_flanges=num_flanges,
                                         num_exchangers=num_exchangers, num_vaporizers=num_vaporizers, num_arms=num_arms,
                                         num_extra_comp1=num_extra_comp1, num_extra_comp2=num_extra_comp2,

                                         facil_length=facil_length, facil_width=facil_width,
                                         pipe_outer_diam=pipe_outer_diam,
                                         pipe_thickness=pipe_thickness,

                                         rel_species=rel_species, rel_temp=rel_temp, rel_pres=rel_pres, rel_phase=rel_phase,
                                         amb_temp=amb_temp, amb_pres=amb_pres,
                                         discharge_coeff=discharge_coeff,
                                         num_vehicles=num_vehicles, daily_fuelings=daily_fuelings,
                                         vehicle_days=vehicle_days,

                                         immed_ign_probs=immed_ign_probs,
                                         delayed_ign_probs=delayed_ign_probs,
                                         ign_thresholds=ign_thresholds,

                                         detection_credit=detection_credit,
                                         overp_method=overp_method,
                                         TNT_equivalence_factor=TNT_equivalence_factor,
                                         BST_mach_flame_speed=BST_mach_flame_speed,
                                         probit_thermal_id=probit_thermal_id,
                                         exposure_time=exposure_time,
                                         probit_overp_id=probit_overp_id,

                                         nozzle_model=nozzle_model,
                                         release_angle=release_angle,
                                         excl_radius=excl_radius,
                                         rand_seed=rand_seed,
                                         rel_humid=rel_humid,

                                         occupant_group_dicts=occupant_group_dicts,

                                         compressor_leak_probs=compressor_leak_probs,
                                         vessel_leak_probs=vessel_leak_probs,
                                         valve_leak_probs=valve_leak_probs,
                                         instrument_leak_probs=instrument_leak_probs,
                                         pipe_leak_probs=pipe_leak_probs,
                                         joint_leak_probs=joint_leak_probs,
                                         hose_leak_probs=hose_leak_probs,
                                         filter_leak_probs=filter_leak_probs,
                                         flange_leak_probs=flange_leak_probs,
                                         exchanger_leak_probs=exchanger_leak_probs,
                                         vaporizer_leak_probs=vaporizer_leak_probs,
                                         arm_leak_probs=arm_leak_probs,
                                         extra_comp1_leak_probs=extra_comp1_leak_probs,
                                         extra_comp2_leak_probs=extra_comp2_leak_probs,

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
                                         rel_freq_000d01=rel_freq_000d01,
                                         rel_freq_000d10=rel_freq_000d10,
                                         rel_freq_001d00=rel_freq_001d00,
                                         rel_freq_010d00=rel_freq_010d00,
                                         rel_freq_100d00=rel_freq_100d00,
                                         fueling_fail_freq_override=fueling_fail_freq_override,

                                         output_dir=output_dir, verbose=verbose, create_plots=True,
                                         )

        # convert objects to dicts for C# consumption
        leak_results = analysis_dict['leak_results']
        leak_result_dicts = []
        for leak_result in leak_results:
            if leak_result.fueling_fail_freq_override is None:
                leak_result.fueling_fail_freq_override = -1.
            newdict = vars(leak_result)
            leak_result_dicts.append(newdict)

        analysis_dict['leak_results'] = leak_result_dicts

        log.info("\n LEAK RESULT DICTS:")
        for leak_result_dict in leak_result_dicts:
            log.info(leak_result_dict)

        # Convert heat flux and overpressure to expected units in GUI
        analysis_dict['position_qrads'] /= 1000.  # kW/m2 from W/m2
        analysis_dict['position_overps'] /= 1000.  # kPa from Pa

        results['status'] = True
        results['data'] = analysis_dict

    except ValueError as exc:
        msg = "Invalid input. {}".format(str(exc))
        results["message"] = msg
        log.error(msg)

    except exceptions.InputError as exc:
        msg = "Invalid input: {}".format(exc.message)
        results["message"] = msg
        log.error(msg)

    except Exception as exc:
        msg = "Analysis failed: {}".format(str(exc))
        results["message"] = msg
        log.error(exc)

    finally:
        logging.shutdown()
        gc.collect()
        return results


def request_analysis(pipe_length=0,
                     num_compressors=0, num_vessels=0, num_valves=0,
                     num_instruments=0, num_joints=0, num_hoses=0,
                     num_filters=0, num_flanges=0,
                     num_exchangers=0, num_vaporizers=0, num_arms=0,
                     num_extra_comp1=0, num_extra_comp2=0,

                     facil_length=10, facil_width=10,
                     pipe_outer_diam=0.00952501905,
                     pipe_thickness=.001650033,

                     rel_species='h2', rel_temp=288.15, rel_pres=35000000, rel_phase=None,
                     amb_temp=288.15, amb_pres=101325,
                     discharge_coeff=1.,
                     num_vehicles=20, daily_fuelings=2, vehicle_days=250,

                     immed_ign_probs=None,
                     delayed_ign_probs=None,
                     ign_thresholds=None,

                     detection_credit=0.9,
                     overp_method='bst',
                     TNT_equivalence_factor=None,
                     BST_mach_flame_speed=None,
                     probit_thermal_id='eis', exposure_time=60.,
                     probit_overp_id='col',

                     nozzle_model='yuce',
                     release_angle=0.,
                     excl_radius=0.01,
                     rand_seed=3632850,
                     rel_humid=0.89,

                     occupant_group_dicts=None,

                     compressor_leak_probs=None, vessel_leak_probs=None, valve_leak_probs=None,
                     instrument_leak_probs=None, pipe_leak_probs=None, joint_leak_probs=None,
                     hose_leak_probs=None, filter_leak_probs=None, flange_leak_probs=None,
                     exchanger_leak_probs=None, vaporizer_leak_probs=None, arm_leak_probs=None,
                     extra_comp1_leak_probs=None, extra_comp2_leak_probs=None,

                     noz_po_dist='Beta', noz_po_a=.5, noz_po_b=610415.5,
                     noz_ftc_dist='ExpectedValue', noz_ftc_a=.002, noz_ftc_b=0.,
                     mvalve_ftc_dist='ExpectedValue', mvalve_ftc_a=.001, mvalve_ftc_b=0.,
                     svalve_ftc_dist='ExpectedValue', svalve_ftc_a=.002, svalve_ftc_b=0.,
                     svalve_ccf_dist='ExpectedValue', svalve_ccf_a=0.00012766, svalve_ccf_b=0.,
                     overp_dist='Beta', overp_a=3.5, overp_b=310289.5,
                     pvalve_fto_dist='LogNormal', pvalve_fto_a=-11.7359368859313, pvalve_fto_b=0.667849415603714,
                     driveoff_dist='Beta', driveoff_a=31.5, driveoff_b=610384.5,
                     coupling_ftc_dist='Beta', coupling_ftc_a=.5, coupling_ftc_b=5031.,
                     rel_freq_000d01=-1.,
                     rel_freq_000d10=-1.,
                     rel_freq_001d00=-1.,
                     rel_freq_010d00=-1.,
                     rel_freq_100d00=-1.,
                     fueling_fail_freq_override=-1.,

                     output_dir=None, verbose=False, create_plots=True,
                     ):
    """
    QRA analysis with default inputs for easier, non-GUI access.
    See analysis.py for parameter descriptions, including description of return dict.
    TODO: move this to different file since it's not specific to c_api functionality.

    Returns
    -------
    results : dict
        Compilation of QRA analysis results
        See analysis.py for details
    """
    log.info("Initializing API: QRA Analysis...")
    rel_species = rel_species.upper()

    if output_dir is None:
        output_dir = os.getcwd()

    if occupant_group_dicts is None:
        occupant_group_dicts = [
            {"NumTargets": 9, "Desc": "Group 1", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 1.0,
             "XLocParamB": 20.0, "YLocDistribution": 2, "YLocParamA": 1.0, "YLocParamB": 0.0, "ZLocParamA": 1.0,
             "ZLocParamB": 12.0, "ParamUnitType": 0, "ExposureHours": 2000.0}]

    component_sets = [
        ComponentSet('compressor', num_compressors,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=compressor_leak_probs),
        ComponentSet('vessel', num_vessels,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=vessel_leak_probs),
        ComponentSet('valve', num_valves,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=valve_leak_probs),
        ComponentSet('instrument', num_instruments,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=instrument_leak_probs),
        ComponentSet('joint', num_joints,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=joint_leak_probs),
        ComponentSet('hose', num_hoses,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=hose_leak_probs),
        ComponentSet('pipe', int(pipe_length),
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=pipe_leak_probs),
        ComponentSet('filter', num_filters,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=filter_leak_probs),
        ComponentSet('flange', num_flanges,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=flange_leak_probs),
        ComponentSet('exchanger', num_exchangers,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=exchanger_leak_probs),
        ComponentSet('vaporizer', num_vaporizers,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=vaporizer_leak_probs),
        ComponentSet('arm', num_arms,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=arm_leak_probs),
        ComponentSet('extra1', num_extra_comp1,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=extra_comp1_leak_probs),
        ComponentSet('extra2', num_extra_comp2,
                     species=rel_species, saturated_phase=rel_phase, leak_frequency_lists=extra_comp2_leak_probs),
    ]

    if fueling_fail_freq_override in ["", None, " ", -1, -1.]:
        fueling_fail_freq_override = None

    component_failure_set = component_failure.ComponentFailureSet(
            f_failure_override=fueling_fail_freq_override,
            num_vehicles=num_vehicles, daily_fuelings=daily_fuelings, vehicle_days=vehicle_days,
            noz_po_dist=noz_po_dist, noz_po_a=noz_po_a, noz_po_b=noz_po_b,
            noz_ftc_dist=noz_ftc_dist, noz_ftc_a=noz_ftc_a, noz_ftc_b=noz_ftc_b,
            mvalve_ftc_dist=mvalve_ftc_dist, mvalve_ftc_a=mvalve_ftc_a, mvalve_ftc_b=mvalve_ftc_b,
            svalve_ftc_dist=svalve_ftc_dist, svalve_ftc_a=svalve_ftc_a, svalve_ftc_b=svalve_ftc_b,
            svalve_ccf_dist=svalve_ccf_dist, svalve_ccf_a=svalve_ccf_a, svalve_ccf_b=svalve_ccf_b,
            overp_dist=overp_dist, overp_a=overp_a, overp_b=overp_b,
            pvalve_fto_dist=pvalve_fto_dist, pvalve_fto_a=pvalve_fto_a, pvalve_fto_b=pvalve_fto_b,
            driveoff_dist=driveoff_dist, driveoff_a=driveoff_a, driveoff_b=driveoff_b,
            coupling_ftc_dist=coupling_ftc_dist, coupling_ftc_a=coupling_ftc_a, coupling_ftc_b=coupling_ftc_b,
            verbose=verbose)

    rel_freq_overrides = [rel_freq_000d01, rel_freq_000d10, rel_freq_001d00, rel_freq_010d00, rel_freq_100d00]
    rand_seed = int(rand_seed)

    # Ensure lower-case, alphanumeric
    nozzle_model = misc_utils.clean_name(nozzle_model)
    probit_thermal_id = misc_utils.clean_name(probit_thermal_id)
    probit_overp_id = misc_utils.clean_name(probit_overp_id)

    if verbose:
        log.info("")
        log.info("\n== C API ARRAYS ==")
        log.info("Ignition data:")
        log.info("immed_ign_probs {}".format(immed_ign_probs))
        log.info("delayed_ign_probs {}".format(delayed_ign_probs))
        log.info("ign_thresholds {}".format(ign_thresholds))
        log.info("")
        log.info("Occupants:")
        for data in occupant_group_dicts:
            log.info(data)
        log.info("\nprobit_thermal_id {}".format(probit_thermal_id))
        log.info("probit_overp_id {}".format(probit_overp_id))
        log.info("nozzle_model {}".format(nozzle_model))

    analysis_dict = analysis.conduct_analysis(
            pipe_outer_diam=pipe_outer_diam, pipe_thickness=pipe_thickness,
            amb_temp=amb_temp, amb_pres=amb_pres,
            rel_species=rel_species, rel_temp=rel_temp, rel_pres=rel_pres, rel_phase=rel_phase,
            facil_length=facil_length, facil_width=facil_width,
            immed_ign_probs=immed_ign_probs,
            delayed_ign_probs=delayed_ign_probs,
            ign_thresholds=ign_thresholds,
            occupant_input_list=occupant_group_dicts,
            component_sets=component_sets,
            component_failure_set=component_failure_set,
            discharge_coeff=discharge_coeff, detection_credit=detection_credit,
            overp_method=overp_method,
            TNT_equivalence_factor=TNT_equivalence_factor,
            BST_mach_flame_speed=BST_mach_flame_speed,
            probit_thermal_id=probit_thermal_id, exposure_time=exposure_time, probit_overp_id=probit_overp_id,
            nozzle_model=nozzle_model,
            rel_angle=release_angle, rel_humid=rel_humid,
            rand_seed=rand_seed, excl_radius=excl_radius,

            release_freq_overrides=rel_freq_overrides,
            verbose=verbose, output_dir=output_dir, create_plots=create_plots)

    return analysis_dict
