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

import datetime
import logging
import os

import numpy as np

from . import occupants as occ_lib
from . import probit
from .leaks import LeakSizeResult
from .flame_data import FlameData
from ..phys import api as phys_api
from ..utilities import misc_utils

log = logging.getLogger(__name__)


def conduct_analysis(pipe_outer_diam, pipe_thickness,
                     amb_temp, amb_pres,
                     rel_temp, rel_pres, rel_phase,
                     facil_length, facil_width, facil_height,
                     ign_prob_ranges,
                     occupant_input_list,
                     component_sets,
                     component_failure_set,
                     leak_sizes=None,
                     rel_species='h2',
                     discharge_coeff=1., detect_gas_flame=True, detection_credit=0.9,
                     probit_thermal_id='eise', exposure_time=60, probit_rel_id='coll',
                     nozzle_model='yuce',
                     leak_height=0., rel_angle=0., rel_humid=0.89, rand_seed=3632850, excl_radius=0.01,
                     overp_frag_mass=None, overp_velocity=None, overp_total_mass=None,
                     peak_overp_list=None, overp_impulse_list=None,

                     release_freq_overrides=None,

                     verbose=False,
                     output_dir=None,
                     create_plots=True,
                     ):
    """
    Quantitative risk assessment including scenario calculations and harm modeling.
    Note: facility height currently unused.
    Default values for optional overrides (to not use override) is -1 due to type restrictions from C# calls.

    Default units of measurement:
        Pressure [Pa]
        distance [m]
        time [s]

    Parameters
    ----------
    pipe_outer_diam : float
        [m] Outer diameter of pipe

    pipe_thickness : float
        [m] Thickness of pipe wall (single side)

    amb_temp : float
        [K] Ambient temperature

    amb_pres : float
        [Pa] Ambient pressure

    rel_temp : float
        [K] Hydrogen temperature

    rel_pres : float
        [Pa] Hydrogen pressure

    rel_phase : {'gas', 'liquid', None}
        Fluid phase; gas implies saturated vapor, liquid implies saturated liquid.
        None corresponds to default 'gas' in GUI. Note that QRA currently designed for 'None' option.

    facil_length : float
        [m] Length of facility

    facil_width : float
        [m] Width of facility

    facil_height : float
        [m] Height of facility

    ign_prob_ranges : list of dicts
        Minimum and maximum thresholds for rates of immediate and delayed ignition.
        Min or max can be None (null) to represent +/- infinity
        Format: {threshold_min, threshold_max, immed_prob, delay_prob}

    occupant_input_list : list of dicts
        Each dict defines group of occupants/workers near radiative source.
        Format: {count, descrip, xdistr, xa, xb, ydistr, ya, yb, zdistr, za, zb, hours}
        Where xdistr can be uniform, deterministic or normal.
        Example:
            {
                'count': 9,
                'descrip': 'workers',
                'xdistr': 'uniform',
                'xa': 1,
                'xb': 20,
                'ydistr': 'dete',
                'ya': 1,
                'yb': None,
                'zdistr': 'unif',
                'za': 1,
                'zb': 12,
                'hours': 2000,
            }

    component_sets : [ComponentSet]
        List of components (e.g. compressors).

    component_failure_set : ComponentFailureSet
        Object representing component failure properties and parameters.

    leak_sizes : [floats] or None
        List of percentages representing % leak.

    rel_species : {'h2', 'cng'}
        Release fluid species

    discharge_coeff : float
        [-] Discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow)

    detect_gas_flame : bool
        Whether credit should be applied.

    detection_credit : float
        Chance of detecting flame/release, as decimal.

    probit_thermal_id : {'eise', 'tsao', 'tno', 'lees'}
        4-char ID of thermal harm model to use.

    exposure_time : float
        [s] Duration of exposure to heat source

    probit_rel_id : {'leis', 'lhse', 'head', 'coll', 'debr'}
        4-char ID of overpressure harm model to use.
        See probit.py for current options.

    nozzle_model : str
        4-char key referencing notional nozzle model to use for high-pressure release. See phys h2_nn for options.

    leak_height : float
        [m] Vertical height of leak, for use in qrad flame calculation

    rel_angle : float
        [deg] Leak release angle for use in qrad flame calculation

    rel_humid : float
        Relative humidity between 0.0 and 1.0

    rand_seed : int
        Random seeding for flame calculation

    excl_radius : float
        [m] Exclusion radius, for use in qrad flame calculation

    overp_frag_mass : float
        [kg] For debris overpressure method; mass of (individual) fragments

    overp_velocity : float
        [m/s] Debris velocity for use in debris overpressure model

    overp_total_mass : float
        [kg] total mass of all debris, for use in debris overpressure model

    peak_overp_list : list
        [Pa] Peak overpressure for each leak size

    overp_impulse_list : list
        [Pa*s] Impulse of shock wave for each leak size

    release_freq_overrides : list of float or None
        Manual override values for H2 release frequency at each release. Not used if == -1.
        If None, vals will be set to -1 (i.e. ignored).

    verbose : bool
        Level of logging.

    output_dir : str
        Path of directory for saving temp data, e.g. pickled flame data.

    create_plots : bool
        Whether output plots should be created.

    Returns
    -------
    results : dict
        Compilation of analysis results containing:
            air : float
                Average Individual Risk is expected # of fatalities per exposed individual
            far : float
                Fatal Accident Rate is expected # of fatalities per 100 million exposed hours
            total_pll : float
                Potential Loss of Life is expected # of fatalities per system year
            plot_files : list of strings
                File locations of QRAD plots for each leak size, in order

            leak_results : list of LeakResult objects
                Each contains PLL contribution, expected probabilities for scenarios, and component leak probabilities

    """
    rad_source_model = 'multi'
    now = datetime.datetime.now()
    params = locals()  # for logging
    sorted_params = sorted(params)
    rel_species = rel_species.upper()

    if leak_sizes is None:
        leak_sizes = [0.01, 0.10, 1.00, 10.00, 100.00]

    if peak_overp_list is None:
        peak_overp_list = [2.5e3, 2.5e3, 5e3, 16.e3, 30.e3]

    if overp_impulse_list is None:
        overp_impulse_list = [0., 0, 0, 0, 0.]

    if not detect_gas_flame:
        detection_credit = 0.

    if release_freq_overrides is None:
        release_freq_overrides = [-1., -1., -1., -1., -1.]

    calc_shutdown = True  # set to False to match prev version

    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    # Record all parameters in log file
    log.info("=== NEW ANALYSIS... {} ===\nSorted Parameters:".format(now))
    for param in sorted_params:
        log.info("{}: {}".format(param, params[param]))
    log.info("output_dir: {}".format(output_dir))

    amb_fluid = phys_api.create_fluid('AIR', amb_temp, amb_pres)
    rel_fluid = phys_api.create_fluid(rel_species, rel_temp, rel_pres, phase=rel_phase)
    if rel_temp is None:
        rel_temp = rel_fluid.T
    if amb_temp is None:
        amb_temp = amb_fluid.T

    # Each occupant row in GUI is represented as group and stored as dict inside list
    # Massage into required format for phys module (xdistr, xa, xb, etc.)
    # NOTE (Cianan): This can be cleaned up; don't need objects here.
    occupant_groups = occ_lib.create_occupant_groups_from_list(occupant_input_list)

    loc_distributions = []
    for group in occupant_groups:
        loc_distributions.append(group.generate_physics_format())

    total_occupants = np.sum([group.num_occupants for group in occupant_groups])
    occupant_avg_hours = np.sum([group.hours * group.num_occupants for group in occupant_groups]) / total_occupants

    log.info("Location distributions: {}".format(loc_distributions))
    log.info("{} Occupants for {} average hours".format(total_occupants, occupant_avg_hours))

    log.info("Component Sets:")
    for compset in component_sets:
        if compset.num_components:
            log.info(str(compset))

    # For each leak size, sum release freqs for all components at that size
    leak_results = [
        LeakSizeResult(leak_sizes[0]),
        LeakSizeResult(leak_sizes[1]),
        LeakSizeResult(leak_sizes[2]),
        LeakSizeResult(leak_sizes[3]),
        LeakSizeResult(leak_sizes[4]),
    ]
    leak_result100 = leak_results[-1]
    num_leak_sizes = len(leak_sizes)

    # Compute leak frequencies for each leak size
    for i, leak_size in enumerate(leak_sizes):
        leak_result = leak_results[i]
        release_freq_override = release_freq_overrides[i]

        # Use override value if provided (i.e. other than -1.0)
        if release_freq_override != -1.:
            leak_result.release_freq_override = release_freq_override
            leak_result.total_release_freq = release_freq_override
            # Zero out component leak freqs since unused
            for comp_set in component_sets:
                leak_result.component_leak_freqs[comp_set.category] = 0.

        # If no override, calculate based on component releases
        else:
            total_leak_freq = 0.
            for comp_set in component_sets:
                component_leak_frequency = comp_set.get_leak_frequency(leak_size)
                leak_result.component_leak_freqs[comp_set.category] = component_leak_frequency
                total_leak_freq += component_leak_frequency
                log.info("Leak {} for {}: {:.3g}".format(leak_size, comp_set.category, component_leak_frequency))
            leak_result.total_release_freq = total_leak_freq

        log.info("Total release freq for size {}: {:.3g}\n".format(leak_size, leak_result.total_release_freq))

    # Account for non-leak fueling failure contributors in 100% release only. Use override value if provided
    # log.info("Manual value for 100% leak vehicle fueling failure? {}".format(component_failure_set.f_fueling_fail))

    leak_result100.set_failures(component_failure_set)

    total_leak_freqs = np.array([leak_res.total_release_freq for leak_res in leak_results])

    log.info("RELEASE FREQUENCIES:")
    log.info("    0.01% - {:.3g}".format(leak_results[0].total_release_freq))
    log.info("    0.10% - {:.3g}".format(leak_results[1].total_release_freq))
    log.info("    1.00% - {:.3g}".format(leak_results[2].total_release_freq))
    log.info("   10.00% - {:.3g}".format(leak_results[3].total_release_freq))
    log.info("  100.00% - {:.3g}".format(leak_result100.total_release_freq))

    # IGNITION PROBABILITIES
    # Compute effective diameter of leak inside pipe, taking into account pipe thickness
    # NOTE: C# HyRAM incorrectly subtracted 1 * pipe_thickness.
    # Encapsulating error in pipe_thickness_factor to reproduce it during testing.
    pipe_inner_diam = pipe_outer_diam - 2. * pipe_thickness
    pipe_area = np.pi * (pipe_inner_diam / 2.) ** 2.

    # Convert sizes from percent to fraction when using in calculation
    leak_areas = (np.array(leak_sizes) / 100.) * pipe_area
    orifice_leak_diams = np.sqrt(4. * (leak_areas / np.pi))

    log.info("Pipe inner diam {:.3g} m, area {:.3g} m^2\n".format(pipe_inner_diam, pipe_area))

    # Compute discharge rates, one per leak size
    hy_discharge_rates = []
    for leak_diam in orifice_leak_diams:
        discharge_rate = phys_api.compute_discharge_rate(rel_fluid, leak_diam, dis_coeff=discharge_coeff)
        hy_discharge_rates.append(discharge_rate)
        log.info("Orifice leak diam: {:.3g}, discharge rate: {:.3g}".format(leak_diam, discharge_rate))

    hy_discharge_rates = np.array(hy_discharge_rates)

    # Determine ignition probabilities for each leak size based on discharge rates and thresholds
    ign_immed_probs = []
    ign_delay_probs = []
    for rate in hy_discharge_rates:
        for ign_range in ign_prob_ranges:
            thres_min = ign_range['threshold_min']
            thres_max = ign_range['threshold_max']
            # Ranges are in order so just check against max. If we get to end (i.e. max is null), then use its range.
            if rate < thres_max or (thres_max is None and rate >= thres_min):
                immed_prob = ign_range['immed_prob']
                delayed_prob = ign_range['delay_prob']
                ign_immed_probs.append(immed_prob)
                ign_delay_probs.append(delayed_prob)
                log.info("Flow rate {:.3g} ign probs: immed {}, delayed {}".format(rate, immed_prob, delayed_prob))
                break

    ign_immed_probs = np.array(ign_immed_probs)
    ign_delay_probs = np.array(ign_delay_probs)

    # Compute probabilities of outcomes: shutdown, jetfire, explosion, or no ignition
    # release_freqs = np.array([res.total_release_freq for res in leak_results], dtype=np.float64)

    # Probabilities within each leak size should sum to 1
    if detect_gas_flame:
        prob_shutdown_per_leak = detection_credit
        prob_jetfire_per_leak = ign_immed_probs * (1. - detection_credit)
        prob_explos_per_leak = ign_delay_probs * (1. - detection_credit)
        prob_no_ign_per_leak = (1. - ign_immed_probs - ign_delay_probs) * (1. - detection_credit)

    else:  # no gas detection credit
        prob_shutdown_per_leak = 0.
        prob_jetfire_per_leak = ign_immed_probs
        prob_explos_per_leak = ign_delay_probs
        prob_no_ign_per_leak = 1. - ign_immed_probs - ign_delay_probs

    # phys wrapper currently returns data in a list where each entry is tuple of (label, data)
    flame_data = FlameData(amb_temp, amb_pres, rel_temp, rel_pres, rel_phase, orifice_leak_diams, leak_height,
                           rel_angle,
                           nozzle_model, loc_distributions, excl_radius, rand_seed, rel_humid,
                           rad_source_model, facil_length, facil_width)

    flame_file = os.path.join(output_dir, 'flame.pkl')

    # Attempt to load previously-pickled flame calc data.
    # Re-run calc if it's params are different or if load fails
    try:
        prev_flame_saved = False
        loaded_flame_data = misc_utils.load_object(flame_file)
        loaded_flame_data.__class__ = FlameData
        log.info("FLAME file loc: {}".format(flame_file))
        log.info("Analysis flame parameters: {}".format(vars(flame_data)))
        log.info("Loaded flame parameters: {}".format(vars(loaded_flame_data)))
        # Check whether all pickled flame inputs are identical to this run; if so, don't need to run flame calc
        if flame_data == loaded_flame_data:
            log.info("Flame data identical")
            prev_flame_saved = True
    except Exception as err:
        log.info("Flame load error. Will re-compute.")
        prev_flame_saved = False
        loaded_flame_data = None

    if prev_flame_saved:
        log.info("Loading flame data...")
        qrads = loaded_flame_data.qrads
        qrad_plot_files = loaded_flame_data.plot_files

    else:
        log.info("Re-computing flame data...")
        # chem_file = os.path.join(output_dir, 'chem.pkl')

        flux_dict = phys_api.flux_analysis(amb_fluid, rel_fluid, rel_height=leak_height,
                                           rel_angle=np.radians(rel_angle),
                                           site_length=facil_length, site_width=facil_width,
                                           orif_diams=orifice_leak_diams, rel_humid=rel_humid,
                                           dis_coeff=discharge_coeff,
                                           rad_src_key=rad_source_model, not_nozzle_key=nozzle_model,
                                           loc_distributions=loc_distributions,
                                           excl_radius=excl_radius, rand_seed=rand_seed,
                                           create_plots=create_plots,
                                           output_dir=output_dir, verbose=verbose)

        qrads = flux_dict['fluxes'] * 1000.  # want W from kW
        qrad_plot_files = flux_dict['all_pos_files']
        # Pickle it for later
        flame_data.qrads = qrads
        flame_data.plot_files = qrad_plot_files
        misc_utils.save_object(flame_file, flame_data)

    log.info("QRAD data:\n{}".format(qrads))

    # HARM MODELS
    thermal_fatality_probs = []
    for i, qrad in enumerate(qrads):
        p_therm_fatal = probit.compute_thermal_fatality_prob(probit_thermal_id, qrad, exposure_time)
        thermal_fatality_probs.append(p_therm_fatal)

    thermal_fatality_probs = np.array(thermal_fatality_probs)

    # Convert from 1d to 2d where rows are qrads for single leak size
    thermal_fatality_probs = thermal_fatality_probs.reshape((num_leak_sizes, total_occupants))
    # Probability of jetfire fatality for each leak size
    thermal_fatality_probs_per_leak = np.sum(thermal_fatality_probs, axis=1)  # sum over positions so 1 val / leak size
    log.info("Probit thermal data:\n{}\n".format(thermal_fatality_probs_per_leak))

    # Probability of fatality from overpressure for each leak size
    overp_fatality_probs = []
    for i in range(len(peak_overp_list)):
        p_overp_fatal = probit.compute_overpressure_fatality_prob(probit_rel_id, peak_overp_list[i],
                                                                  impulse=overp_impulse_list[i],
                                                                  fragment_mass=overp_frag_mass,
                                                                  velocity=overp_velocity, total_mass=overp_total_mass)
        overp_fatality_probs.append(p_overp_fatal)
    overp_fatality_probs_per_leak = np.array(overp_fatality_probs)
    log.info("Probit overpressure data:\n{}\n".format(overp_fatality_probs_per_leak))

    if total_occupants > 0:
        # potential loss of life is expected # of fatalities per system year, first computed per leak size
        pll = total_leak_freqs * (
                (prob_jetfire_per_leak * thermal_fatality_probs_per_leak) +
                (total_occupants * prob_explos_per_leak * overp_fatality_probs_per_leak))
        total_pll = np.around(np.nansum(pll), 30)

        # fatal accident rate is expected # of fatalities per 100 million exposed hours
        exposed_hours = 1.e8
        hours_per_year = 8760.
        far = total_pll * exposed_hours / (total_occupants * hours_per_year)
        # average individual risk is expected # of fatalities per exposed individual
        air = occupant_avg_hours * far * 1.e-8
    else:
        total_pll = 0.
        far = 0.
        air = 0.

    results = {
        'total_pll': total_pll,
        'far': far,
        'air': air,
        'leak_results': None,  # Will calc these next
        'plot_files': qrad_plot_files,
    }

    # Scenario ranking
    # cpScenarioStats.cs
    for i, leak_size in enumerate(leak_sizes):
        leak_result = leak_results[i]
        leak_release_freq = leak_result.total_release_freq

        # Retrieve event probabilities for this leak size
        prob_jetfire = prob_jetfire_per_leak[i]
        prob_explos = prob_explos_per_leak[i]

        if calc_shutdown:
            prob_shutdown = prob_shutdown_per_leak
            prob_no_ign = prob_no_ign_per_leak[i]
        else:
            prob_shutdown = 0.
            prob_no_ign = 1. - prob_jetfire - prob_explos

        # Compute average events per year for this leak size
        shutdown_avg_events = prob_shutdown * leak_release_freq
        jetfire_avg_events = prob_jetfire * leak_release_freq
        explos_avg_events = prob_explos * leak_release_freq
        no_ign_avg_events = prob_no_ign * leak_release_freq

        if total_pll > 0.:
            # Calculate PLL contributions as % of total PLL
            jetfire_pll_contrib = thermal_fatality_probs_per_leak[i] * jetfire_avg_events / total_pll
            explos_pll_contrib = total_occupants * overp_fatality_probs_per_leak[i] * explos_avg_events / total_pll
        else:
            jetfire_pll_contrib = 0.
            explos_pll_contrib = 0.

        # Store calculation results for display
        leak_result.p_shutdown = np.around(prob_shutdown, 20)
        leak_result.p_jetfire = np.around(prob_jetfire, 20)
        leak_result.p_explos = np.around(prob_explos, 20)
        leak_result.p_no_ign = np.around(prob_no_ign, 20)

        leak_result.shutdown_avg_events = np.around(shutdown_avg_events, 20)
        leak_result.jetfire_avg_events = np.around(jetfire_avg_events, 20)
        leak_result.explos_avg_events = np.around(explos_avg_events, 20)
        leak_result.no_ign_avg_events = np.around(no_ign_avg_events, 20)

        leak_result.jetfire_pll_contrib = np.around(jetfire_pll_contrib, 20)
        leak_result.explos_pll_contrib = np.around(explos_pll_contrib, 20)

        log.info(str(leak_result))

    if verbose:
        print("")
        for leak_res in leak_results:
            print(leak_res)
        print("PLL: {:.5E}".format(total_pll))
        print("FAR: {:.5E}".format(far))
        print("AIR: {:.5E}\n".format(air))

    results['leak_results'] = leak_results

    # Print one result key/val pair per line
    log.info("\nANALYSIS RESULTS:\n{}".format("\n".join(["{}: {}".format(key, val) for key, val in results.items()])))
    log.info("=== ANALYSIS COMPLETE ===")

    return results
