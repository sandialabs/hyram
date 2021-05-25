"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

import gc
import logging
import pdb

from . import analysis
from .component_set import ComponentSet
from .component_failure import ComponentFailureSet
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


def qra_analysis(pipe_length, num_compressors, num_cylinders, num_valves,
                 num_instruments, num_joints, num_hoses,
                 num_filters, num_flanges, num_extra_comp1, num_extra_comp2,
                 facil_length, facil_width, facil_height,
                 pipe_outer_diam, pipe_thickness,

                 rel_species, rel_temp, rel_pres, rel_phase, amb_temp, amb_pres, discharge_coeff,
                 num_vehicles, daily_fuelings, vehicle_days,

                 immed_ign_probs, delayed_ign_probs, ign_thresholds,

                 detect_gas_flame, detection_credit,
                 probit_thermal_id, exposure_time,
                 probit_rel_id, peak_overp_list, overp_impulse_list,
                 overp_frag_mass, overp_velocity, overp_total_mass,

                 nozzle_model,
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
                 rel_freq_000d01,
                 rel_freq_000d10,
                 rel_freq_001d00,
                 rel_freq_010d00,
                 rel_freq_100d00,
                 fueling_fail_freq_override,

                 output_dir, verbose=False,
                 ):
    """
    Process C# inputs and execute QRA analysis.
    Advanced types must be converted to Python equivalents, including lists.
    See analysis.py for parameter descriptions, including description of return dict.

    Returns dict
    -------

    """
    log.info("Initializing CAPI: QRA Request...")
    log.info(misc_utils.params_as_str(locals()))
    rel_species = rel_species.upper()

    # Convert ignition probability lists to list of dicts
    immed_ign_probs = c_utils.convert_to_numpy_array(immed_ign_probs)
    delayed_ign_probs = c_utils.convert_to_numpy_array(delayed_ign_probs)
    ign_thresholds = c_utils.convert_to_numpy_array(ign_thresholds)

    ign_prob_ranges = misc_utils.convert_ign_prob_lists_to_dicts(immed_ign_probs, delayed_ign_probs, ign_thresholds)

    # Convert component leak probability sets from C# 2D array to np 2d arrays
    compressor_leak_probs = c_utils.convert_2d_array_to_numpy_array(compressor_leak_probs)
    cylinder_leak_probs = c_utils.convert_2d_array_to_numpy_array(cylinder_leak_probs)
    filter_leak_probs = c_utils.convert_2d_array_to_numpy_array(filter_leak_probs)
    flange_leak_probs = c_utils.convert_2d_array_to_numpy_array(flange_leak_probs)
    hose_leak_probs = c_utils.convert_2d_array_to_numpy_array(hose_leak_probs)
    joint_leak_probs = c_utils.convert_2d_array_to_numpy_array(joint_leak_probs)
    pipe_leak_probs = c_utils.convert_2d_array_to_numpy_array(pipe_leak_probs)
    valve_leak_probs = c_utils.convert_2d_array_to_numpy_array(valve_leak_probs)
    instrument_leak_probs = c_utils.convert_2d_array_to_numpy_array(instrument_leak_probs)
    extra_comp1_leak_probs = c_utils.convert_2d_array_to_numpy_array(extra_comp1_leak_probs)
    extra_comp2_leak_probs = c_utils.convert_2d_array_to_numpy_array(extra_comp2_leak_probs)

    component_sets = [
        ComponentSet('compressor', num_compressors, species=rel_species, leak_frequency_lists=compressor_leak_probs),
        ComponentSet('cylinder', num_cylinders, species=rel_species, leak_frequency_lists=cylinder_leak_probs),
        ComponentSet('valve', num_valves, species=rel_species, leak_frequency_lists=valve_leak_probs),
        ComponentSet('instrument', num_instruments, species=rel_species, leak_frequency_lists=instrument_leak_probs),
        ComponentSet('joint', num_joints, species=rel_species, leak_frequency_lists=joint_leak_probs),
        ComponentSet('hose', num_hoses, species=rel_species, leak_frequency_lists=hose_leak_probs),
        ComponentSet('pipe', int(pipe_length), species=rel_species, leak_frequency_lists=pipe_leak_probs),
        ComponentSet('filter', num_filters, species=rel_species, leak_frequency_lists=filter_leak_probs),
        ComponentSet('flange', num_flanges, species=rel_species, leak_frequency_lists=flange_leak_probs),
        ComponentSet('extra1', num_extra_comp1, species=rel_species, leak_frequency_lists=extra_comp1_leak_probs),
        ComponentSet('extra2', num_extra_comp2, species=rel_species, leak_frequency_lists=extra_comp2_leak_probs),
    ]

    if fueling_fail_freq_override in ["", None, " ", -1, -1.]:
        fueling_fail_freq_override = None

    component_failure_set = ComponentFailureSet(
            f_failure_override=fueling_fail_freq_override,
            num_vehicles=num_vehicles, daily_fuelings=daily_fuelings, vehicle_days=vehicle_days,
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
            coupling_ftc_b=coupling_ftc_b)

    rel_freq_overrides = [rel_freq_000d01, rel_freq_000d10, rel_freq_001d00, rel_freq_010d00, rel_freq_100d00]

    peak_overp_list = c_utils.convert_to_numpy_array(peak_overp_list)
    overp_impulse_list = c_utils.convert_to_numpy_array(overp_impulse_list)

    occupant_group_dicts = c_utils.convert_occupant_json_to_dicts(occupant_dist_json)
    rand_seed = int(rand_seed)

    # Ensure lower-case, alphanumeric.
    nozzle_model = misc_utils.clean_name(nozzle_model)
    probit_thermal_id = misc_utils.clean_name(probit_thermal_id)
    probit_rel_id = misc_utils.clean_name(probit_rel_id)

    # Log converted lists, ids
    log.info("")
    log.info("ARRAYS:")
    log.info("Ignition data:")
    for data in ign_prob_ranges:
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
    log.info("nozzle_model {}".format(nozzle_model))

    results = {"status": False, "data": None, "message": None}

    try:
        analysis_dict = analysis.conduct_analysis(
                pipe_outer_diam=pipe_outer_diam, pipe_thickness=pipe_thickness,
                amb_temp=amb_temp, amb_pres=amb_pres,
                rel_species=rel_species, rel_temp=rel_temp, rel_pres=rel_pres, rel_phase=rel_phase,
                facil_length=facil_length, facil_width=facil_width, facil_height=facil_height,
                ign_prob_ranges=ign_prob_ranges,
                occupant_input_list=occupant_group_dicts,
                component_sets=component_sets,
                component_failure_set=component_failure_set,
                discharge_coeff=discharge_coeff, detect_gas_flame=detect_gas_flame, detection_credit=detection_credit,
                probit_thermal_id=probit_thermal_id, exposure_time=exposure_time, probit_rel_id=probit_rel_id,
                nozzle_model=nozzle_model,
                leak_height=leak_height, rel_angle=release_angle, rel_humid=rel_humid,
                rand_seed=rand_seed, excl_radius=excl_radius,
                overp_frag_mass=overp_frag_mass, overp_velocity=overp_velocity, overp_total_mass=overp_total_mass,
                peak_overp_list=peak_overp_list, overp_impulse_list=overp_impulse_list,

                release_freq_overrides=rel_freq_overrides,
                verbose=verbose, output_dir=output_dir, create_plots=True)

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
