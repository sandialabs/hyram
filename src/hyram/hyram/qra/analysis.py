"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging

import numpy as np

from . import effects, ignition_probs, leak_frequency, pipe_size, risk, event_tree, consequence
from . import component_set
from . import positions as qra_positions
from ..phys import api as phys_api
from ..phys import _comps
from ..utilities import misc_utils

log = logging.getLogger(__name__)


def conduct_analysis(pipe_outer_diam, pipe_thickness,
                     amb_temp, amb_pres,
                     rel_temp, rel_pres, rel_phase,
                     facil_length, facil_width,
                     immed_ign_probs,
                     delayed_ign_probs,
                     ign_thresholds,
                     occupant_input_list,
                     component_sets,
                     component_failure_set,
                     rel_species,
                     leak_sizes=None,
                     discharge_coeff=1, detection_credit=0.9,
                     overp_method='bst',
                     TNT_equivalence_factor=None,
                     BST_mach_flame_speed=None,
                     probit_thermal_id='eise', exposure_time=30, probit_overp_id='head',
                     nozzle_model='yuce',
                     rel_angle=0, rel_humid=0.89,
                     rand_seed=3632850, excl_radius=0.01,
                     release_freq_overrides=None,
                     event_tree_override=None,
                     verbose=False,
                     output_dir=None,
                     create_plots=True):
    """
    Quantitative risk assessment including scenario calculations and harm modeling

    Default values for optional overrides (to not use override) is -1 due to type restrictions from C# calls

    Parameters
    ----------
    pipe_outer_diam : float
        [m] Outer diameter of pipe

    pipe_thickness : float
        [m] Thickness of pipe wall (single side)

    amb_temp : float
        [K] Ambient temperature

    amb_pres : float
        [Pa] Ambient pressure (absolute)

    rel_temp : float
        [K] Fluid temperature

    rel_pres : float
        [Pa] Fluid pressure (absolute)

    rel_phase : {'gas', 'liquid', None}
        Fluid phase; gas implies saturated vapor, liquid implies saturated liquid.
        None corresponds to default 'gas' in GUI.

    facil_length : float
        [m] Length of facility

    facil_width : float
        [m] Width of facility

    immed_ign_probs : list of floats
        List of immediate ignition probabilities based on the mass flow rate thresholds 

    delayed_ign_probs : list of floats
        List of delayed ignition probabilities based on the mass flow rate thresholds

    ign_thresholds : list of floats
        Sorted list of mass flow rates [kg/s] that separate the list of ignition probabilities

    occupant_input_list : list of dicts
        Each dict defines group of occupants/workers near radiative source.
        Format: {count, descrip, xdistr, xa, xb, ydistr, ya, yb, zdistr, za, zb, hours}
        Where distributions can be uniform, deterministic or normal, {'unif', 'dete', 'norm'}.
        Example:
            {
                'count': 9,
                'descrip': 'workers',
                'xdistr': 'uniform', 'xa': 1, 'xb': 20,
                'ydistr': 'dete', 'ya': 1, 'yb': None,
                'zdistr': 'unif', 'za': 1, 'zb': 12,
                'hours': 2000,
            }

    component_sets : [ComponentSet]
        List of components (e.g. compressors)

    component_failure_set : ComponentFailureSet
        Object representing component failure properties and parameters.

    rel_species : string or dict
        Release fluid species (e.g., 'h2', 'ch4') or dict of species and concentrations

    leak_sizes : [floats] or None
        List of percentages representing % leak

    discharge_coeff : float
        [-] Discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow)

    detection_credit : float
        Probability of detecting flame/release, as fraction

    overp_method : {'tnt', 'bst', 'bauwens'}
        ID of unconfined overpressure model to use

    TNT_equivalence_factor : float
        TNT mass equivalence factor for use with TNT unconfined overpressure model
        (unused if TNT overpressure model is not used)

    BST_mach_flame_speed : {0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2}
        Mach flame speed for use with BST unconfined overpressure model
        (unused if BST overpressure model is not used)

    probit_thermal_id : {'eise', 'tsao', 'tno', 'lees'}
        ID of thermal probit model to use

    exposure_time : float
        [s] Duration of exposure to heat source

    probit_overp_id : {'leis', 'lhse', 'head', 'coll'}
        ID of overpressure probit model to use

    nozzle_model : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        4-char key referencing notional nozzle model to use for high-pressure release

    rel_angle : float
        [deg] Leak release angle for use in qrad flame calculation

    rel_humid : float
        Relative humidity between 0.0 and 1.0

    rand_seed : int
        Random seed value for random number position generator

    excl_radius : float
        [m] Exclusion radius, for use in qrad flame calculation

    release_freq_overrides : list of floats or None
        Manual override values for release frequency at each leak size.
        Not used if == -1
        If None, vals will be set to -1 (i.e. ignored).

    event_tree_override : list of event_node specifications with order dictating event_tree construction or None
        Manual override specification for event tree construction.
        If None, will use default event tree specification

    verbose : bool
        If True, extra output will be printed (default False)

    output_dir : str
        Path of directory for saving local files,
        e.g., log-file and plot images

    create_plots : bool
        Whether output plots should be created

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
            qrad_plot_files : list of strings
                File locations of QRAD plots for each leak size, in order
            overp_plot_files : list of strings
                File locations of overpressure plots for each leak size, in order
            impulse_plot_files : list of strings
                File locations of impulse plots for each leak size, in order
            leak_results : list of LeakResult objects
                Each contains PLL contribution, expected probabilities for scenarios,
                and component leak probabilities
            positions : 2d array
                (x,y,z) coordinates of occupants
            position_qrads : 2d array
                flux data [W/m2] per leak per position
                e.g. for 9 positions, 9x5 array
            position_overps : 2d array
                peak overpressure data [Pa] per leak per position
                e.g. for 9 positions, 9x5 array
            position_impulses : 2d array
                impulse data [Pa] per leak per position,
                e.g. for 9 positions, 9x5 array
    """
    if probit_overp_id in ['head', 'coll'] and overp_method == 'bauwens':
        impulse_probit_error_msg = ('Overpressure method "bauwens"'
                                    + ' does not produce impulse values,'
                                    + ' and so cannot be used with'
                                    + f' overpressure probit "{probit_overp_id}"')
        raise ValueError(impulse_probit_error_msg)

    params = locals()  # for logging
    sorted_params = sorted(params)

    if output_dir is None:
        output_dir = misc_utils.get_temp_folder()

    log.info("")
    log.info("=== BEGINNING ANALYSIS ===")
    log.info("")
    log.info("PARAMETERS")
    for param_name in sorted_params:
        param_val = params[param_name]
        if isinstance(param_val, list) and isinstance(param_val[0], component_set.ComponentSet):
            log.info("Component Sets:")
            for comp_set in param_val:
                log.info(f"{comp_set}")
        else:
            log.info("{}: {}".format(param_name, str(param_val)))

    amb_fluid = phys_api.create_fluid('AIR', amb_temp, amb_pres)
    rel_fluid = phys_api.create_fluid(rel_species, rel_temp, rel_pres, phase=rel_phase)
    if rel_temp is None:
        rel_temp = rel_fluid.T
    if amb_temp is None:
        amb_temp = amb_fluid.T

    # Each occupant row in GUI is represented as group and stored as dict inside list
    # Massage into required format for phys module [count, (xdistr, xa, xb), (ydistr...]
    loc_distributions = []
    total_occupants = 0
    total_occupant_hours = 0
    for group_dict in occupant_input_list:
        num_occupants = int(group_dict['count'])
        loc_distribution = [num_occupants,
                            (group_dict['xdistr'], group_dict['xa'], group_dict['xb']),
                            (group_dict['ydistr'], group_dict['ya'], group_dict['yb']),
                            (group_dict['zdistr'], group_dict['za'], group_dict['zb'])]
        loc_distributions.append(loc_distribution)
        total_occupants += num_occupants
        total_occupant_hours += int(group_dict['hours'] * num_occupants)
    if total_occupant_hours == 0:
        occupant_avg_hours = 0
    else:
        occupant_avg_hours = total_occupant_hours / total_occupants

    if verbose:
        component_set_log_msg = ""
        for compset in component_sets:
            if compset.num_components:
                component_set_log_msg += "{}\n".format(str(compset))
        log.info(f"Location distributions: {loc_distributions}")
        log.info(f"{total_occupants} Occupants for {occupant_avg_hours} average hours")
        log.info("")

    leak_results, leak_result100 = leak_frequency.compute_leak_frequencies(leak_sizes,
                                                                           release_freq_overrides,
                                                                           component_sets)
    num_leak_sizes = len(leak_results)

    # Account for non-leak fueling failure contributors in 100% release only. Use override value if provided
    leak_result100.set_failures(component_failure_set)
    total_leak_freqs = np.array([leak_res.total_release_freq for leak_res in leak_results])

    log.info("RELEASE FREQUENCIES:")
    for leak_result in leak_results[:-1]:
        log.info(f" {leak_result.leak_size:0.2f}% - {leak_result.total_release_freq:.3g}")
    log.info(f"  {leak_result100.leak_size:.2f}% - {leak_result100.total_release_freq:.3g}")

    # Compute leak diameters and discharge rates, one per leak size
    pipe_inner_diam = pipe_size.calc_pipe_inner_diameter(pipe_outer_diam, pipe_thickness)
    pipe_flow_area = pipe_size.calc_pipe_flow_area(pipe_inner_diam)
    log.info("System pipe inner diameter {:.3g} m, area {:.3g} m^2".format(pipe_inner_diam, pipe_flow_area))
    orifices = []
    discharge_rates = []
    for leak_result in leak_results:
        orifice_leak_diam = pipe_size.calc_orifice_diameter(pipe_flow_area, leak_result.leak_size/100)
        orifice = _comps.Orifice(orifice_leak_diam, discharge_coeff)
        orifices.append(orifice)
        discharge_rate = orifice.mdot(orifice.flow(rel_fluid))
        discharge_rates.append(discharge_rate)
        log.info("For {}% leak size: orifice leak diameter: {:.3g} m, discharge rate: {:.3g} kg/s".format(leak_result.leak_size, orifice_leak_diam, discharge_rate))

    # Determine ignition probabilities for each leak size based on discharge rates and thresholds
    immed_ign_probs_per_leak = []
    delay_ign_probs_per_leak = []
    for rate in discharge_rates:
        (immed_ign_prob, delayed_ign_prob) = ignition_probs.get_ignition_probability(rate,
                                                                                     ign_thresholds,
                                                                                     immed_ign_probs,
                                                                                     delayed_ign_probs)
        immed_ign_probs_per_leak.append(immed_ign_prob)
        delay_ign_probs_per_leak.append(delayed_ign_prob)
        log.info("Flow rate {:.3g} (kg/s) ignition probabilities: immed {}, delayed {}".format(rate, immed_ign_prob, delayed_ign_prob))
    immed_ign_probs_per_leak = np.array(immed_ign_probs_per_leak)
    delay_ign_probs_per_leak = np.array(delay_ign_probs_per_leak)

    # Evaluate event tree
    # Use override event tree specification if provided
    if event_tree_override is None:
        events_in_tree = []
        events_in_tree.append({'name': 'Shutdown',
                               'key': 'shut',
                               'event_prob': detection_credit,
                               'consequence_type': None})
        events_in_tree.append({'name': 'No Ignition',
                               'key': 'noig',
                               'event_prob': 1 - (immed_ign_probs_per_leak + delay_ign_probs_per_leak),
                               'consequence_type': None})                       
        events_in_tree.append({'name': 'Jetfire',
                               'key': 'jetf',
                               'event_prob': immed_ign_probs_per_leak / (immed_ign_probs_per_leak + delay_ign_probs_per_leak),
                               'consequence_type': 'thermal'})
        events_in_tree.append({'name': 'Explosion',
                               'key': 'expl',
                               'event_prob': 1,
                               'consequence_type': 'overp'})
        end_states = event_tree.build_event_tree(events_in_tree, num_leak_sizes)

    else:
        end_states = event_tree.build_event_tree(event_tree_override, num_leak_sizes)

    # Generate positions
    posgen = qra_positions.PositionGenerator(loc_distributions,
                                             excl_radius,
                                             rand_seed)
    locations = posgen.locs
    transposed_positions = np.array(locations).transpose()

    # Calculate harm/hazard effects at each position for each leak size
    # Calculate thermal effects
    log.info("Computing thermal effects...")
    rel_angle_rads = np.radians(rel_angle)
    try:
        flux_dict = effects.calc_thermal_effects(amb_fluid,
                                                 rel_fluid,
                                                 rel_angle=rel_angle_rads,
                                                 site_length=facil_length,
                                                 site_width=facil_width,
                                                 orifices=orifices,
                                                 rel_humid=rel_humid,
                                                 not_nozzle_model=nozzle_model,
                                                 locations=locations,
                                                 create_plots=create_plots,
                                                 output_dir=output_dir,
                                                 verbose=verbose)
    except ValueError as err:
        if type(rel_species) == dict:
            raise ValueError('Invalid blend provided')

    log.info("Thermal effects analysis complete")
    qrads = flux_dict['fluxes']
    qrad_plot_files = flux_dict['all_pos_files']
    log.info("Heat flux data:\n{}".format(qrads))

    # Calculate overpressure effects
    log.info("Computing overpressure effects...")
    overp_dict = effects.calc_overp_effects(orifices,
                                            nozzle_model,
                                            rel_fluid,
                                            amb_fluid,
                                            rel_angle,
                                            locations,
                                            facil_length,
                                            facil_width,
                                            overp_method,
                                            BST_mach_flame_speed,
                                            TNT_equivalence_factor,
                                            create_plots=create_plots,
                                            output_dir=output_dir,
                                            verbose=verbose)
    log.info("Overpressure effects analysis complete")
    overpressures = overp_dict['overpressures']
    impulses = overp_dict['impulses']
    overp_plot_files = overp_dict['all_pos_overp_files']
    impulse_plot_files = overp_dict['all_pos_impulse_files']
    log.info("Overpressure data:\n{}".format(overpressures))
    log.info("Impulse data:\n{}".format(impulses))

    # Estimate fatality probabilities
    event_consequences = []
    physical_responses = {'qrads': qrads,
                          'overpressures': overpressures,
                          'impulses': impulses}
    consequence_modeling_decisions = {'probit_thermal_id': probit_thermal_id,
                                      'exposure_time': exposure_time,
                                      'probit_overp_id': probit_overp_id}
    for end_state in end_states:
        event_consequences.append(consequence.calculate_event_consequence(end_state.consequence_type,
                                                                          num_leak_sizes,
                                                                          total_occupants,
                                                                          physical_responses,
                                                                          consequence_modeling_decisions))

    # Calculate risk metrics

    # Risk for each scenario
    log.info("Calculating frequencies and consequences for each scenario...")
    scenario_freqs = []
    scenario_fatalities = []
    for i in range(num_leak_sizes):
        # Compute expected events per year for this leak size
        for end_state, consequence_outcome in zip(end_states, event_consequences):
            scenario_freqs.append(end_state.end_state_probability[i] * total_leak_freqs[i])
            scenario_fatalities.append(consequence_outcome[i])

    # Overall risk metrics
    log.info("Calculating risk for each scenario...")
    plls = risk.calc_all_plls(scenario_freqs, scenario_fatalities)
    log.info("Calculating overall risk metrics and risk contributions for each scenario...")
    total_pll, pll_contributions = risk.calc_risk_contributions(plls)
    far = risk.calc_far(total_pll, total_occupants)
    air = risk.calc_air(far, occupant_avg_hours)
    

    # Output results

    # Collect results for each leak size
    log.info("Results for each leak size:")
    for i, leak_result in enumerate(leak_results):
        for j, end_state in enumerate(end_states):
            leak_result.list_event_names.append(end_state.name)
            leak_result.list_event_keys.append(end_state.key)
            leak_result.list_p_events.append(np.around(end_state.end_state_probability[i], 20))
            leak_result.list_avg_events.append(np.around(scenario_freqs[i*len(end_states)+j], 20))
            leak_result.list_pll_contrib.append(np.around(pll_contributions[i*len(end_states)+j], 20))
            # event data in dict format for easier GUI consumption
            leak_result.event_dicts = leak_result.get_result_dicts()

        leak_result.mass_flow_rate = discharge_rates[i]
        leak_result.leak_diam = orifices[i].d
        log.info(str(leak_result))

    # Re-shape harm values into position table
    position_qrads_reshape = (qrads.reshape((num_leak_sizes, total_occupants))).T
    position_overps_reshape = (overpressures.reshape((num_leak_sizes, total_occupants))).T
    position_impulses_reshape = (impulses.reshape((num_leak_sizes, total_occupants))).T

    results = {
        'total_pll': total_pll,
        'far': far,
        'air': air,
        'leak_results': leak_results,
        'qrad_plot_files': qrad_plot_files,
        'overp_plot_files': overp_plot_files,
        'impulse_plot_files': impulse_plot_files,
        'positions': transposed_positions,
        'position_qrads': position_qrads_reshape,
        'position_overps': position_overps_reshape,
        'position_impulses': position_impulses_reshape
    }

    if verbose:
        print("")
        for leak_res in leak_results:
            print(leak_res)
        print("PLL: {:.5E}".format(total_pll))
        print("FAR: {:.5E}".format(far))
        print("AIR: {:.5E}\n".format(air))

    # Print one result key/val pair per line
    log.info("\nANALYSIS RESULTS:\n{}".format("\n".join(["{}: {}".format(key, val) for key, val in results.items()])))
    log.info("=== ANALYSIS COMPLETE ===")

    return results
