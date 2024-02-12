"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import gc

import json
import logging
import os
import sys

from datetime import datetime
from . import utils, exceptions

try:
    from hyram.qra import analysis, component_failure
    from hyram.qra.component_set import ComponentSet, init_component_sets
    from hyram.utilities import misc_utils
except ModuleNotFoundError as err:
    from hyram.hyram.qra import analysis, component_failure
    from hyram.hyram.qra.component_set import ComponentSet, init_component_sets
    from hyram.hyram.utilities import misc_utils

log = logging.getLogger(__name__)


class PrintToLogger(object):
    def __init__(self, logfile):
        self.terminal = sys.stdout
        self.writer = open(logfile, "a")

    def write(self, msg):
        msg = str.strip(msg)
        if msg == '':
            return
        msg = f'{datetime.now().strftime("%Y-%m-%d %H:%M:%S")}  {msg}\n'
        if self.terminal is not None:
            self.terminal.write(msg)
        self.writer.write(msg)


def setup(output_dir, verbose):
    """ Set up module logging globally.

    Parameters
    ----------
    output_dir : str
        Path to log directory

    verbose : bool
        Determine level of logging
    """
    logfilepath = utils.setup_file_log(output_dir, verbose=verbose, logname=__name__)
    # send print statement output to logfile
    if verbose:
        sys.stdout = PrintToLogger(logfilepath)


def c_request_analysis(n_pipes, n_compressors, n_vessels, n_valves, n_instruments, n_joints, n_hoses, n_filters, n_flanges,
                       n_exchangers, n_vaporizers, n_arms, n_extra_comp1, n_extra_comp2,

                       facil_length, facil_width,
                       pipe_outer_diam, pipe_thickness,
                       mass_flow, mass_flow_leak_size,

                       rel_species, rel_temp, rel_pres, rel_phase,
                       amb_temp, amb_pres,
                       discharge_coeff,
                       n_vehicles, daily_fuelings, vehicle_days,

                       immed_ign_probs,
                       delayed_ign_probs,
                       ign_thresholds,

                       detection_credit,
                       overp_method,
                       tnt_factor,
                       bst_flame_speed,
                       probit_thermal_id, exposure_time,
                       probit_overp_id,

                       nozzle_model,
                       release_angle,
                       excl_radius,
                       rand_seed,
                       rel_humid,

                       occupant_dist_json,

                       compressor_ps, vessel_ps, valve_ps, instrument_ps, pipe_ps, joint_ps, hose_ps, filter_ps, flange_ps,
                       exchanger_ps, vaporizer_ps, arm_ps, extra_comp1_ps, extra_comp2_ps,

                       noz_po_dist, noz_po_a, noz_po_b,
                       noz_ftc_dist, noz_ftc_a, noz_ftc_b,
                       mvalve_ftc_dist, mvalve_ftc_a, mvalve_ftc_b,
                       svalve_ftc_dist, svalve_ftc_a, svalve_ftc_b,
                       svalve_ccf_dist, svalve_ccf_a, svalve_ccf_b,
                       overp_dist, overp_a, overp_b,
                       pvalve_fto_dist, pvalve_fto_a, pvalve_fto_b,
                       driveoff_dist, driveoff_a, driveoff_b,
                       coupling_ftc_dist, coupling_ftc_a, coupling_ftc_b,
                       # overrides
                       f_release_000d01,
                       f_release_000d10,
                       f_release_001d00,
                       f_release_010d00,
                       f_release_100d00,
                       f_failure,

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
        status : bool
            indicates whether the analysis was successful
        message : string
            error message if status is False
        data : dict
            Compilation of QRA analysis results
            See analysis.py for details
    """
    log.info("Initializing C API: QRA Request...")

    immed_ign_probs = list(utils.convert_to_numpy_array(immed_ign_probs))
    delayed_ign_probs = list(utils.convert_to_numpy_array(delayed_ign_probs))
    ign_thresholds = list(utils.convert_to_numpy_array(ign_thresholds))

    occupant_group_dicts = utils.convert_occupant_json_to_dicts(occupant_dist_json)

    # Convert component leak probability sets from C# 2D array to np 2d arrays
    compressor_ps = utils.convert_2d_array_to_numpy_array(compressor_ps)
    vessel_ps = utils.convert_2d_array_to_numpy_array(vessel_ps)
    filter_ps = utils.convert_2d_array_to_numpy_array(filter_ps)
    flange_ps = utils.convert_2d_array_to_numpy_array(flange_ps)
    hose_ps = utils.convert_2d_array_to_numpy_array(hose_ps)
    joint_ps = utils.convert_2d_array_to_numpy_array(joint_ps)
    pipe_ps = utils.convert_2d_array_to_numpy_array(pipe_ps)
    valve_ps = utils.convert_2d_array_to_numpy_array(valve_ps)
    instrument_ps = utils.convert_2d_array_to_numpy_array(instrument_ps)
    exchanger_ps = utils.convert_2d_array_to_numpy_array(exchanger_ps)
    vaporizer_ps = utils.convert_2d_array_to_numpy_array(vaporizer_ps)
    arm_ps = utils.convert_2d_array_to_numpy_array(arm_ps)
    extra_comp1_ps = utils.convert_2d_array_to_numpy_array(extra_comp1_ps)
    extra_comp2_ps = utils.convert_2d_array_to_numpy_array(extra_comp2_ps)

    results = {"status": False, "data": None, "message": None}

    if type(rel_species) == dict:
        rel_species_cp_str = '&'.join(['%s[%f]' % (s, X) for s, X in zip(rel_species.keys(), rel_species.values())])
        major_species = utils.get_fluid_formula_from_blend_str(rel_species_cp_str)
    else:  # species is string
        rel_species = rel_species.upper()
        major_species = rel_species

    if output_dir is None:
        output_dir = os.getcwd()

    if occupant_group_dicts is None:
        occupant_group_dicts = [
            {"NumTargets": 9, "Desc": "Group 1", "ZLocDistribution": 1, "XLocDistribution": 1, "XLocParamA": 1.0,
             "XLocParamB": 20.0, "YLocDistribution": 2, "YLocParamA": 1.0, "YLocParamB": 0.0, "ZLocParamA": 1.0,
             "ZLocParamB": 12.0, "ParamUnitType": 0, "ExposureHours": 2000.0}]

    component_sets = init_component_sets(species=major_species, phase=rel_phase,
                                         n_compressors=n_compressors, compressor_ps=compressor_ps,
                                         n_vessels=n_vessels, vessel_ps=vessel_ps,
                                         n_valves=n_valves, valve_ps=valve_ps,
                                         n_instruments=n_instruments, instrument_ps=instrument_ps,
                                         n_joints=n_joints, joint_ps=joint_ps,
                                         n_hoses=n_hoses, hose_ps=hose_ps,
                                         n_pipes=n_pipes, pipe_ps=pipe_ps,
                                         n_filters=n_filters, filter_ps=filter_ps,
                                         n_flanges=n_flanges, flange_ps=flange_ps,
                                         n_exchangers=n_exchangers, exchanger_ps=exchanger_ps,
                                         n_vaporizers=n_vaporizers, vaporizer_ps=vaporizer_ps,
                                         n_arms=n_arms, arm_ps=arm_ps,
                                         n_extra1s=n_extra_comp1, extra1_ps=extra_comp1_ps,
                                         n_extra2s=n_extra_comp2, extra2_ps=extra_comp2_ps)

    component_failure_set = component_failure.ComponentFailureSet(
            f_failure_override=f_failure,
            num_vehicles=n_vehicles, daily_fuelings=daily_fuelings, vehicle_days=vehicle_days,
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

    f_release_overrides = [f_release_000d01, f_release_000d10, f_release_001d00, f_release_010d00, f_release_100d00]
    rand_seed = int(rand_seed)

    # Ensure lower-case, alphanumeric
    nozzle_model = misc_utils.clean_name(nozzle_model)
    probit_thermal_id = misc_utils.clean_name(probit_thermal_id)
    probit_overp_id = misc_utils.clean_name(probit_overp_id)

    if verbose:
        log.info("")
        log.info("== C API ARRAYS ==")
        log.info("Ignition data:")
        log.info("immed_ign_probs {}".format(immed_ign_probs))
        log.info("delayed_ign_probs {}".format(delayed_ign_probs))
        log.info("ign_thresholds {}".format(ign_thresholds))
        log.info("")
        log.info("Occupants:")
        for data in occupant_group_dicts:
            log.info(data)

    try:
        analysis_dict = analysis.conduct_analysis(pipe_outer_diam=pipe_outer_diam,
                                                  pipe_thickness=pipe_thickness,
                                                  amb_temp=amb_temp,
                                                  amb_pres=amb_pres,
                                                  rel_species=rel_species,
                                                  rel_temp=rel_temp,
                                                  rel_pres=rel_pres,
                                                  rel_phase=rel_phase,
                                                  facil_length=facil_length,
                                                  facil_width=facil_width,
                                                  immed_ign_probs=immed_ign_probs,
                                                  delayed_ign_probs=delayed_ign_probs,
                                                  ign_thresholds=ign_thresholds,
                                                  occupant_input_list=occupant_group_dicts,
                                                  component_sets=component_sets,
                                                  component_failure_set=component_failure_set,
                                                  discharge_coeff=discharge_coeff,
                                                  detection_credit=detection_credit,
                                                  overp_method=overp_method,
                                                  tnt_factor=tnt_factor,
                                                  bst_flame_speed=bst_flame_speed,
                                                  probit_thermal_id=probit_thermal_id,
                                                  exposure_time=exposure_time, probit_overp_id=probit_overp_id,
                                                  nozzle_model=nozzle_model,
                                                  rel_angle=release_angle,
                                                  rel_humid=rel_humid,
                                                  rand_seed=rand_seed,
                                                  mass_flow=mass_flow, mass_flow_leak_size=mass_flow_leak_size,
                                                  excl_radius=excl_radius,
                                                  f_release_overrides=f_release_overrides,
                                                  verbose=verbose,
                                                  output_dir=output_dir,
                                                  create_plots=True,
                                                  )

        # convert objects to dicts for C# consumption.
        leak_result_dicts = [res.to_dict() for res in analysis_dict['leak_results']]
        analysis_dict['leak_results'] = leak_result_dicts

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
