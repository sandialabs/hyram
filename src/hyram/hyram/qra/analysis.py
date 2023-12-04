"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np

from . import effects, ignition_probs, pipe_size, risk, event_tree, consequence, leak_size_results
from . import component_set
from . import positions as qra_positions
from ..phys import api as phys_api
from ..phys import _comps, _jet, _therm
from ..utilities import misc_utils


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
                     tnt_factor=None,
                     bst_flame_speed=None,
                     probit_thermal_id='eise', exposure_time=30, probit_overp_id='head',
                     nozzle_model='yuce',
                     rel_angle=0, rel_humid=0.89,
                     rand_seed=None, excl_radius=0.01,
                     mass_flow=None, mass_flow_leak_size=None,
                     f_release_overrides=None,
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

    tnt_factor : float
        TNT mass equivalence factor for use with TNT unconfined overpressure model
        (unused if TNT overpressure model is not used)

    bst_flame_speed : float
        One of {0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2}
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

    rand_seed : int or None
        Random seed value for random number position generator.
        If None, will generate new random seed each call.

    mass_flow : float, optional
        [kg/s] Mass flow rate for specified leak size, if unchoked

    mass_flow_leak_size : {1, 10, 100, 1000, 10000}, optional
        Represents leak size (as hundredth of percent) of specified mass flow rate, assuming unchoked flow.

    excl_radius : float
        [m] Exclusion radius, for use in qrad flame calculation

    f_release_overrides : list of floats or None
        Manual override values for release frequency at each leak size.
        Not used if == -1 or None
        If None, vals will be set to -1 (i.e. ignored).

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
                e.g. for 9 positions with 5 leak sizes, 9x5 array
            position_overps : 2d array
                peak overpressure data [Pa] per leak per position
                e.g. for 9 positions with 5 leak sizes, 9x5 array
            position_impulses : 2d array
                impulse data [Pa s] per leak per position,
                e.g. for 9 positions with 5 leak sizes, 9x5 array
    """
    if probit_overp_id in ['head', 'coll'] and overp_method == 'bauwens':
        impulse_probit_error_msg = ('Overpressure method "bauwens"'
                                    + ' does not produce impulse values,'
                                    + ' and so cannot be used with'
                                    + f' overpressure probit "{probit_overp_id}"')
        raise ValueError(impulse_probit_error_msg)

    params = locals()
    sorted_params = sorted(params)

    if output_dir is None:
        output_dir = misc_utils.get_temp_folder()

    if verbose:
        print("=== BEGINNING ANALYSIS ===")
        print("Parameters:")
        for param_name in sorted_params:
            param_val = params[param_name]
            if isinstance(param_val, list) and isinstance(param_val[0], component_set.ComponentSet):
                print("Component Sets:")
                for comp_set in param_val:
                    print(f"{comp_set}")
            else:
                print("{}: {}".format(param_name, str(param_val)))
        print("\n")

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
        print(component_set_log_msg)
        print(f"Location distributions: {loc_distributions}")
        print(f"{total_occupants} Occupants for {occupant_avg_hours} average hours\n")

    leak_results = leak_size_results.init_leak_results(leak_sizes, f_release_overrides, component_sets,
                                                       mass_flow_leak_size=mass_flow_leak_size, mass_flow=mass_flow)
    leak_result100 = leak_results[-1]
    num_leak_sizes = len(leak_results)

    # Account for non-leak fueling failure contributors in 100% release only. Will use override value if provided.
    leak_result100.set_failure_set(component_failure_set)
    total_leak_freqs = np.array([leak_res.f_release for leak_res in leak_results])

    # Compute leak diameters and discharge rates, one per leak size
    pipe_inner_diam = pipe_size.calc_pipe_inner_diameter(pipe_outer_diam, pipe_thickness)
    pipe_flow_area = pipe_size.calc_pipe_flow_area(pipe_inner_diam)

    if verbose:
        print("RELEASE FREQUENCIES:")
        for leak_result in leak_results:
            print(f" {leak_result.leak_size:0.2f}% - {leak_result.f_release:.3g}")
        print(f"System pipe inner diameter {pipe_inner_diam:.3g} m, area {pipe_flow_area:.3g} m^2")

    orifices = []
    discharge_rates = []
    developing_flows = []
    cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(nozzle_model, rel_fluid)
    # Calculate mass flows for leak sizes if given unchoked mass flow for single leak size

    for leak_result in leak_results:
        orifice_leak_diam = pipe_size.calc_orifice_diameter(pipe_flow_area, leak_result.leak_size/100)
        orifice = _comps.Orifice(orifice_leak_diam, discharge_coeff)
        orifices.append(orifice)
        developing_flow = _jet.DevelopingFlow(rel_fluid, orifice, amb_fluid, theta0=rel_angle,
                                              nn_conserve_momentum=cons_momentum, nn_T=notional_noz_t, verbose=verbose)
        developing_flows.append(developing_flow)
        # discharge_rate = orifice.mdot(developing_flow.fluid_orifice)
        discharge_rate = orifice.mdot(orifice.flow(rel_fluid, mdot=leak_result.mass_flow_override))
        discharge_rates.append(discharge_rate)
        if verbose:
            print(f"For {leak_result.leak_size}% leak size: orifice leak diameter: {orifice_leak_diam:.3g} m, "
                     f"discharge rate: {discharge_rate:.3g} kg/s")

    # Evaluate event tree
    # Determine ignition probabilities for each leak size based on discharge rates and thresholds
    end_state_probabilities_per_leak = []
    end_state_consequence_types = {'Shutdown': None,
                                   'No Ignition': None,
                                   'Jet Fire': 'thermal',
                                   'Explosion': 'overp'}
    for rate in discharge_rates:
        (immed_ign_prob, delayed_ign_prob) = ignition_probs.get_ignition_probability(rate,
                                                                                     ign_thresholds,
                                                                                     immed_ign_probs,
                                                                                     delayed_ign_probs)
        if verbose:
            print("Flow rate {:.3g} (kg/s) ignition probabilities: immed {}, delayed {}".format(rate, immed_ign_prob, delayed_ign_prob))

        # Calculate event tree end-state probabilities
        total_ign_prob = immed_ign_prob + delayed_ign_prob
        no_ign_prob = event_tree.calc_probability_not_occur(total_ign_prob)
        conditional_immed_ign_prob = immed_ign_prob / total_ign_prob
        event_probabilities = [detection_credit, no_ign_prob, conditional_immed_ign_prob]
        end_state_probabilities = event_tree.calc_end_state_probabilities(event_probabilities)
        end_state_probabilities_per_leak.append(end_state_probabilities)

    # Generate positions
    posgen = qra_positions.PositionGenerator(loc_distributions, excl_radius, rand_seed)
    locations = posgen.locs

    # Note - these are the same conditions for initializing the Combustion object as is in phys.Flame
    chem = _therm.Combustion(_comps.Fluid(species = rel_fluid.species, T = amb_temp, P = amb_pres))

    # Calculate harm/hazard effects at each position for each leak size
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
                                                 developing_flows=developing_flows,
                                                 chem = chem,
                                                 create_plots=create_plots,
                                                 output_dir=output_dir,
                                                 verbose=verbose)
    except ValueError as err:
        if type(rel_species) == dict:
            raise ValueError('Invalid blend provided')

    qrads = flux_dict['fluxes']
    qrad_plot_files = flux_dict['all_pos_files']
    overp_dict = effects.calc_overp_effects(orifices,
                                            nozzle_model,
                                            rel_fluid,
                                            amb_fluid,
                                            rel_angle,
                                            locations,
                                            facil_length,
                                            facil_width,
                                            overp_method,
                                            bst_flame_speed,
                                            tnt_factor,
                                            developing_flows,
                                            create_plots=create_plots,
                                            output_dir=output_dir,
                                            verbose=verbose)
    overpressures = overp_dict['overpressures']
    impulses = overp_dict['impulses']
    overp_plot_files = overp_dict['all_pos_overp_files']
    impulse_plot_files = overp_dict['all_pos_impulse_files']

    if verbose:
        print(f"Heat flux data:\n{qrads}\n Overpressure data:\n{overpressures}\n Impulse data:\n{impulses}")

    # Estimate fatality probabilities
    physical_responses = {'qrads': qrads,
                          'overpressures': overpressures,
                          'impulses': impulses}
    consequence_modeling_decisions = {'probit_thermal_id': probit_thermal_id,
                                      'exposure_time': exposure_time,
                                      'probit_overp_id': probit_overp_id}
    event_consequences = []
    for end_state_name in end_state_consequence_types:
        event_consequence = consequence.calculate_event_consequence(end_state_consequence_types[end_state_name],
                                                                    num_leak_sizes,
                                                                    total_occupants,
                                                                    physical_responses,
                                                                    consequence_modeling_decisions,
                                                                    verbose)
        event_consequences.append(event_consequence)

    # //////////////////////
    # CALCULATE RISK METRICS

    # Risk for each scenario
    if verbose:
        print("Calculating frequencies and consequences for each scenario...")
    scenario_freqs = []
    scenario_fatalities = []
    for i in range(num_leak_sizes):
        # Compute expected events per year for this leak size
        for end_state_probability, consequence_outcome in zip(end_state_probabilities_per_leak[i], event_consequences):
            scenario_freqs.append(end_state_probability * total_leak_freqs[i])
            scenario_fatalities.append(consequence_outcome[i])

    # Overall risk metrics
    if verbose:
        print("Calculating risk for each scenario...")
    plls = risk.calc_all_plls(scenario_freqs, scenario_fatalities)
    if verbose:
        print("Calculating overall risk metrics and risk contributions for each scenario...")
    total_pll, pll_contributions = risk.calc_risk_contributions(plls)
    far = risk.calc_far(total_pll, total_occupants)
    air = risk.calc_air(far, occupant_avg_hours)

    # Collect results for each leak size
    if verbose:
        print("Results for each leak size:")
    end_state_keys = {'Shutdown': 'shut',
                      'No Ignition': 'noig',
                      'Jet Fire': 'jetf',
                      'Explosion': 'expl'}
    for i, leak_result in enumerate(leak_results):
        for j, end_state_name in enumerate(end_state_consequence_types):
            leak_result.list_event_names.append(end_state_name)
            leak_result.list_event_keys.append(end_state_keys[end_state_name])
            leak_result.list_p_events.append(np.around(end_state_probabilities_per_leak[i][j], 20))
            leak_result.list_avg_events.append(np.around(scenario_freqs[i*len(end_state_consequence_types)+j], 20))
            leak_result.list_pll_contrib.append(np.around(pll_contributions[i*len(end_state_consequence_types)+j], 20))
            # event data in dict format for easier GUI consumption
            leak_result.event_dicts = leak_result.get_result_dicts()

        leak_result.mass_flow_rate = discharge_rates[i]
        leak_result.leak_diam = orifices[i].d
        if verbose:
            print(str(leak_result))

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
        'position_impulses': position_impulses_reshape
    }

    if verbose:
        print(f"PLL: {total_pll:.5E}\nFAR: {far:.5E}\nAIR: {air:.5E}")

        # Print one result key/val pair per line
        print("\nANALYSIS RESULTS:\n{}".format("\n".join(["{}: {}".format(key, val) for key, val in results.items()])))
        print("=== ANALYSIS COMPLETE ===")

    return results
