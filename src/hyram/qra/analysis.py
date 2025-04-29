"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np

from . import consequence, defaults, effects, event_tree, ignition_probs
from . import pipe_size, risk
from .component import (get_leak_frequencies_at_size_for_set,
                        create_default_component_set)
from ..phys import _comps, _jet, _therm
from ..utilities import misc_utils


def get_total_leak_frequency_at_size(random_leak_frequency_set,
                                     failure_set,
                                     leak_size):
    """
    Returns the total leak frequency, including 100% leaks from fueling
    failures, for the given set of components and fueling failures.
    """
    # TODO: this function is related to the fault tree math
    #       so could be moved to another module
    total_random_leak_freq = sum(random_leak_frequency_set.values())
    if (leak_size == 100) and (failure_set is not None):
        other_leak_freqs = failure_set.freq_failure
    else:
        other_leak_freqs = 0
    return total_random_leak_freq + other_leak_freqs


def conduct_analysis(pipe_inner_diam,
                     amb_temp, amb_pres,
                     rel_temp, rel_pres, rel_species,
                     locations,
                     rel_phase=None,
                     component_set=None,
                     failure_set=None,
                     occupant_hours=None,
                     ft_overrides=None,
                     ign_probs=None,
                     discharge_coeff=1,
                     mass_flow_rates=None,
                     detection_credit=0.9,
                     overp_method='bst',
                     tnt_factor=0.03,
                     bst_flame_speed=0.35,
                     probit_thermal_id='eise', exposure_time=30, probit_overp_id='head',
                     nozzle_model='yuce',
                     rel_angle=0, rel_humid=0.89,
                     verbose=False,
                     output_dir=None,
                     create_plots=True):
    """
    Quantitative risk assessment including scenario calculations and harm modeling

    Parameters
    ----------
    pipe_inner_diam : float
        [m] Inner diameter of system pipe

    amb_temp : float
        [K] Ambient temperature

    amb_pres : float
        [Pa] Ambient pressure (absolute)

    rel_temp : float
        [K] Fluid temperature

    rel_pres : float
        [Pa] Fluid pressure (absolute)

    rel_species : string or dict
        Release fluid species (e.g., 'h2', 'ch4') or dict of species and concentrations

    locations : list of lists
        List of Cartesian coordinates in the form [x, y, z] for each location

    rel_phase : {'gas', 'liquid', None}
        Fluid phase; "gas" is saturated vapor, "liquid" is saturated liquid.
        For fluid phase of `None`, the fluid temperature (`rel_temp`)
        and pressure (`rel_pres`) must be specified.
        Default is `None`.
        Note: None corresponds to default 'Fluid' in GUI.

    component_set : [Component] or None
        List of components (e.g. compressors). If `None`, the default component
        set for the given state is used. Default is `None`.

    failure_set : ComponentFailureSet or None
        Object representing dispenser component failure properties
        and parameters. If `None`, the default failure set is used.
        Default is `None`.

    occupant_hours : [floats] or None
        List of hours present for each occupant,
        length of list should be equal to length of locations input.
        If not given, then default of 2000 hours per occupant is used.

    ft_overrides : [floats] or None
        Intended as a way to override the fault tree calculations.
        If specified, should be list of annual leak frequencies [leaks/year],
        one for each of the different leak sizes.
        If None, then annual leak frequencies will be calculated
        using the implemented fault trees.
        Default is None.

    ign_probs : dict or None
        If None, then default values will be used.
        Otherwise, dictionary of ignition probaiblities of the form:
        ```
        self.ignition_probs = {
            # Sorted list of flow rates [kg/s] that separate the ignition probabilities
            'flow_thresholds': flow_thresholds,
            # List of immediate ignition probabilities based on the mass flow rate thresholds
            'immed_ign_probs': immed_ign_probs,
            # List of delayed ignition probabilities based on the mass flow rate thresholds
            'delayed_ign_probs': delayed_ign_probs
        }
        ```

    discharge_coeff : float
        [-] Discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow)

    mass_flow_rates: [floats] or None
        Intended for use only for unchoked flow.
        If specified, should be list of mass flow rates [kg/s],
        one for each of the different leak sizes.
        If None, then mass flow rate will be calculated for choked flow.
        Default is None.

    detection_credit : float
        Probability of detecting flame/release, as fraction

    overp_method : {'tnt', 'bst', 'bauwens'}
        ID of unconfined overpressure model to use

    tnt_factor : float
        TNT mass equivalence factor for use with TNT unconfined overpressure model
        (unused if TNT overpressure model is not used).
        Default is 0.03 (3%).

    bst_flame_speed : float
        One of {0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2}.
        Mach flame speed for use with BST unconfined overpressure model
        (unused if BST overpressure model is not used).
        Default is 0.35.

    probit_thermal_id : {'eise', 'tsao', 'tno', 'lees'}
        ID of thermal probit model to use

    exposure_time : float
        [s] Duration of exposure to heat source

    probit_overp_id : {'leis', 'lhse', 'head', 'coll'}
        ID of overpressure probit model to use

    nozzle_model : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        4-char key referencing notional nozzle model to use for high-pressure release

    rel_angle : float
        [rad] Leak release angle, 0 is horizontal, pi/2 is vertical

    rel_humid : float
        Relative humidity between 0.0 and 1.0

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

            leak_results : list of dicts
                Each contains PLL contribution, expected probabilities for scenarios,
                and component leak probabilities

            positions : 2d array
                (x,y,z) coordinates of occupants

            position_qrads : 2d array
                heat flux [W/m2] per leak per position
                e.g. for 9 positions with 5 leak sizes, 9x5 array

            position_overps : 2d array
                peak overpressure [Pa] per leak per position
                e.g. for 9 positions with 5 leak sizes, 9x5 array

            position_impulses : 2d array
                impulse [Pa s] per leak per position,
                e.g. for 9 positions with 5 leak sizes, 9x5 array
    """
    if component_set is None:
        component_set = create_default_component_set(rel_species, rel_phase)

    if failure_set is None:
        failure_set = defaults.default_failure_set

    if probit_overp_id in ['head', 'coll'] and overp_method == 'bauwens':
        impulse_probit_error_msg = ('Overpressure method "Bauwens"'
                                    + ' does not produce impulse values,'
                                    + ' and so cannot be used with'
                                    + f' overpressure probit "{probit_overp_id}"')
        raise ValueError(impulse_probit_error_msg)

    params = locals()
    sorted_params = sorted(params)

    if verbose:
        print("=== BEGINNING ANALYSIS ===")
        print("Parameters:")
        for param_name in sorted_params:
            if param_name == 'component_set':
                print('component_set:')
                for comp in component_set:
                    print(comp)
            else:
                param_val = params[param_name]
                print(f'{param_name}: {str(param_val)}')
        print('')

    amb_fluid = _comps.Fluid(species='AIR', T=amb_temp, P=amb_pres)
    rel_fluid = _comps.Fluid(species=rel_species, T=rel_temp, P=rel_pres, phase=rel_phase)
    if rel_temp is None:
        rel_temp = rel_fluid.T
    if amb_temp is None:
        amb_temp = amb_fluid.T

    if ign_probs is None:
        ign_probs = defaults.get_default_ignition_probs(rel_species)

    leak_sizes = defaults.default_leak_sizes
    num_leak_sizes = len(leak_sizes)

    if mass_flow_rates is None:
        mass_flow_rates = [None] * num_leak_sizes

    if ft_overrides is None:
        ft_overrides = [None] * num_leak_sizes

    pipe_flow_area = pipe_size.calc_pipe_flow_area(pipe_inner_diam)

    if verbose:
        print(f"System pipe inner diameter {pipe_inner_diam:.3g} m, area {pipe_flow_area:.3g} m^2")

    orifices = []
    discharge_rates = []

    end_state_consequence_types = {'Shutdown': None,
                                   'No Ignition': None,
                                   'Jet Fire': 'thermal',
                                   'Explosion': 'overp'}
    event_consequences = np.zeros([num_leak_sizes, len(end_state_consequence_types)], dtype=float)
    scenario_freqs = np.zeros_like(event_consequences)
    end_state_probabilities = np.zeros_like(event_consequences)

    total_occupants = len(locations)
    zero_occupants = (total_occupants == 0)

    cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(nozzle_model, rel_fluid)

    chem = _therm.Combustion(_comps.Fluid(species=rel_fluid.species, T=amb_temp, P=amb_pres))

    leak_freqs_by_component = {}
    total_leak_freqs = {}
    all_qrads = np.zeros((num_leak_sizes, total_occupants))
    all_overpressures = np.zeros((num_leak_sizes, total_occupants))
    all_impulses = np.zeros((num_leak_sizes, total_occupants))
    qrad_plot_files = []
    overp_plot_files = []
    impulse_plot_files = []

    for idx, leak_size in enumerate(leak_sizes):
        if verbose:
            print(f'{leak_size}% LEAK SIZE')
            print('----------------------------')
            print("RELEASE FREQUENCIES:")
        if ft_overrides[idx] is None:
            leak_freqs_by_component[leak_size] = get_leak_frequencies_at_size_for_set(
                component_set, leak_size)
            total_leak_freqs[leak_size] = get_total_leak_frequency_at_size(
                leak_freqs_by_component[leak_size], failure_set, leak_size)
            if verbose:
                print("component leak frequencies")
                for component in leak_freqs_by_component[leak_size]:
                    print(f"{component}: {leak_freqs_by_component[leak_size][component]}")
        else:
            if verbose:
                print(f"fault tree override used: {ft_overrides[idx]}")
            leak_freqs_by_component[leak_size] = None
            total_leak_freqs[leak_size] = ft_overrides[idx]
        if verbose:
            print(f"total leak frequency: {total_leak_freqs[leak_size]}")
            print('----------------------------')

        orifice_leak_diam = pipe_size.calc_orifice_diameter(pipe_flow_area, leak_size/100)
        orifice = _comps.Orifice(orifice_leak_diam, discharge_coeff)
        orifices.append(orifice)
        developing_flow = _jet.DevelopingFlow(rel_fluid, orifice, amb_fluid,
                                              mdot=mass_flow_rates[idx], theta0=rel_angle,
                                              nn_conserve_momentum=cons_momentum, nn_T=notional_noz_t, verbose=verbose)

        discharge_rate = _comps.NozzleFlow(rel_fluid, orifice, amb_fluid.P, mdot=mass_flow_rates[idx]).mdot
        discharge_rates.append(discharge_rate)
        if verbose:
            print(f'({orifice_leak_diam:.3g} m leak diameter)')
            print(f'discharge rate: {discharge_rate:.3g} kg/s')
            print('----------------------------')

        (immed_ign_prob, delayed_ign_prob) = (
            ignition_probs.get_ignition_probability(discharge_rate, ign_probs))
        total_ign_prob = ignition_probs.calc_total_ign_prob(immed_ign_prob, delayed_ign_prob)
        no_ign_prob = event_tree.calc_probability_not_occur(total_ign_prob)
        cond_immed_ign_prob = ignition_probs.calc_cond_immed_ign_prob(immed_ign_prob, total_ign_prob)

        event_probabilities = [detection_credit, no_ign_prob, cond_immed_ign_prob]
        end_state_probs_for_size = event_tree.calc_end_state_probabilities(event_probabilities)
        end_state_probabilities[idx, :] = end_state_probs_for_size
        end_state_freqs = event_tree.calc_end_state_frequencies(total_leak_freqs[leak_size],
                                                                end_state_probs_for_size)
        scenario_freqs[idx, :] = end_state_freqs

        if verbose:
            print('OUTCOME PROBABILITIES:')
            print(f'detection/isolation: {detection_credit}')
            print(f'no ignition: {no_ign_prob}')
            print(f'immediate ignition: {immed_ign_prob}')
            print(f'delayed ignition: {delayed_ign_prob}')
            print('----------------------------')

        jetflame_freq = end_state_freqs[2]
        if jetflame_freq == 0 or zero_occupants:
            calculate_thermal_effects = False
        else:
            calculate_thermal_effects = True

        if calculate_thermal_effects:
            qrads, qrad_plot_file = effects.calc_thermal_effects(
                ambient_fluid=amb_fluid,
                release_fluid=rel_fluid,
                release_angle=rel_angle,
                orifice=orifice,
                rel_humid=rel_humid,
                notional_nozzle_model=nozzle_model,
                locations=locations,
                leak_idx=idx,
                developing_flow=developing_flow,
                chem=chem,
                create_plots=create_plots,
                output_dir=output_dir,
                verbose=verbose)
            all_qrads[idx, :] = qrads
            qrad_plot_files.append(qrad_plot_file)
        else:
            all_qrads[idx, :] = np.zeros(total_occupants)
            qrad_plot_files.append("")

        overp_freq = end_state_freqs[3]
        if overp_freq == 0 or zero_occupants:
            calculate_overpressure_effects = False
        else:
            calculate_overpressure_effects = True

        if calculate_overpressure_effects:
            (overpressures, impulses,
            overpressure_plot_filepath,
            impulse_plot_filepath) = effects.calc_overp_effects(
                orifice=orifice,
                notional_nozzle_model=nozzle_model,
                release_fluid=rel_fluid,
                ambient_fluid=amb_fluid,
                release_angle=rel_angle,
                locations=locations,
                overp_method=overp_method,
                leak_idx=idx,
                BST_mach_flame_speed=bst_flame_speed,
                TNT_equivalence_factor=tnt_factor,
                developing_flow=developing_flow,
                create_plots=create_plots,
                output_dir=output_dir,
                verbose=verbose)
            all_overpressures[idx, :] = overpressures
            all_impulses[idx, :] = impulses
            overp_plot_files.append(overpressure_plot_filepath)
            impulse_plot_files.append(impulse_plot_filepath)
        else:
            all_overpressures[idx, :] = np.zeros(total_occupants)
            all_impulses[idx, :] = np.zeros(total_occupants)
            overp_plot_files.append("")
            impulse_plot_files.append("")

        if verbose:
            print('============================')

    # Flatten results (all leaksize loc1, then all leaksize loc2, etc)
    # Corresponds to flattening by row which is C-style ordering
    qrads = all_qrads.flatten(order='C')
    overpressures = all_overpressures.flatten(order='C')
    impulses = all_impulses.flatten(order='C')

    if verbose:
        print("Heat flux results:")
        print(qrads)
        print("Overpressure results:")
        print(overpressures)
        print("Impulse results:")
        print(impulses)
        print('')

    if verbose:
        print("Calculating end state for each scenario...")

    # Calculate end state event consequences and fatalities
    physical_responses = {'qrads': qrads,
                          'overpressures': overpressures,
                          'impulses': impulses}
    consequence_modeling_decisions = {'probit_thermal_id': probit_thermal_id,
                                      'exposure_time': exposure_time,
                                      'probit_overp_id': probit_overp_id}
    for j, end_state_name in enumerate(end_state_consequence_types):
        event_consequence = consequence.calculate_event_consequence(end_state_consequence_types[end_state_name],
                                                                    num_leak_sizes,
                                                                    total_occupants,
                                                                    physical_responses,
                                                                    consequence_modeling_decisions,
                                                                    verbose)
        event_consequences[:, j] = event_consequence

    scenario_fatalities = np.zeros_like(event_consequences)
    for i in range(num_leak_sizes):
        scenario_fatalities[i, :] = event_consequences[i]

    if verbose:
        print('Calculating risk for each scenario...')

    if total_occupants == 0:
        occupant_avg_hours = 0
    else:
        if occupant_hours is None:
            occupant_hours = [defaults.default_occupant_hours] * total_occupants
        total_occupant_hours = np.sum(occupant_hours)
        occupant_avg_hours = total_occupant_hours / total_occupants
        if verbose:
            print(f"Locations: {locations}")
            print(f'{total_occupants} Occupants for ' +
                  f'{occupant_avg_hours} average hours')
            print('')

    # Calculate risk statistics
    if verbose:
        print('Calculating overall risk metrics and risk contributions ' +
              'for each scenario...')
    plls = risk.calc_all_plls(scenario_freqs, scenario_fatalities)
    total_pll, pll_contributions = risk.calc_risk_contributions(plls)
    far = risk.calc_far(total_pll, total_occupants)
    air = risk.calc_air(far, occupant_avg_hours)

    # Collect results for each leak size
    end_state_keys = {'Shutdown': 'shut',
                      'No Ignition': 'noig',
                      'Jet Fire': 'jetf',
                      'Explosion': 'expl'}
    leak_results = []
    for i in range(num_leak_sizes):
        event_results = consequence.generate_event_results(
            event_names=end_state_consequence_types,
            event_keys=end_state_keys,
            prob_end_states=end_state_probabilities[i, :],
            prob_event_occurrence=scenario_freqs[i, :],
            pll_contrib=pll_contributions[i, :]
        )
        if (leak_sizes[i] == 100) and (ft_overrides[i] is None):
            leak_result = dict(
                leak_size=leak_sizes[i],
                discharge_rates=discharge_rates[i],
                leak_diam=orifices[i].d,
                frequency=total_leak_freqs[leak_sizes[i]],
                result_dicts=event_results,
                component_leaks=leak_freqs_by_component[leak_sizes[i]],
                dispenser_failures=failure_set.to_dict()
            )
        else:
            leak_result = dict(
                leak_size=leak_sizes[i],
                discharge_rates=discharge_rates[i],
                leak_diam=orifices[i].d,
                frequency=total_leak_freqs[leak_sizes[i]],
                result_dicts=event_results,
                component_leaks=leak_freqs_by_component[leak_sizes[i]]
            )
        leak_results.append(leak_result)

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
        'positions': np.array(locations).transpose(),
        'position_qrads': position_qrads_reshape,
        'position_overps': position_overps_reshape,
        'position_impulses': position_impulses_reshape}

    if verbose:
        print(f"PLL: {total_pll:.5E}")
        print(f"FAR: {far:.5E}")
        print(f"AIR: {air:.5E}")
        print("ANALYSIS RESULTS:")
        for key, val in results.items():
            print(f"{key}: {val}")
        print("=== ANALYSIS COMPLETE ===")

    return results
