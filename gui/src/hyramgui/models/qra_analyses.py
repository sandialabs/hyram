"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import logging
import math

import numpy as np
import scipy.constants as constants

from hyramgui import app_settings
from hyramgui.hygu.utils import helpers
from pathlib import Path

from hyram.qra import analysis as qra
from hyram.qra.uncertainty import (generate_distributions,
                                   set_components,
                                   set_fueling_failures,
                                   set_occupant_locations, set_occupant_location_definitions,
                                   set_leak_frequency_definitions)
from hyram.qra.uq.distributions import (DeterministicCharacterization, BetaDistribution,
                                        LognormalDistribution, UniformDistribution, NormalDistribution)


def _get_phase(gui_val):
    if gui_val in ['gas', 'liquid']:
        return gui_val
    else:
        return None


def _convert_failure_dict(name, old):
    """Formats dispenser failure data for backend. """
    distr = old['distr']
    if distr == 'beta':
        converted = {'name': name, 'uncertainty_type': 'aleatory',
                     'distribution_type': 'beta', 'a': old['pa'], 'b': old['pb']}
    elif distr == 'ln':
        converted = {'name': name,'uncertainty_type': 'aleatory',
                     'distribution_type': 'log_normal', 'mu': old['pa'], 'sigma': old['pb']}
    else:  # 'ev'
        converted = {'name': name, 'distribution_type': 'deterministic', 'value': old['pa']}
    return converted


def _get_rv_nominals(definitions, seed):
    """
    Gets random value for distributions.

    Parameters
    ----------
    definitions : [dict]
        Describes distribution type and distribution parameters for each element.
    seed : int
        Random seed for RNG.

    Returns : dict
    -------
    Random values for each distribution.

    """
    nominals = dict()
    distributions = generate_distributions(definitions, fix_to_mean=False)

    # pass generator instead of seed so each position is unique while full set is replicable
    rng = np.random.default_rng(seed=seed)

    for name, distribution in distributions.items():
        if isinstance(distribution, DeterministicCharacterization):
            val = distribution.value
        else:
            val = distribution.distribution.rvs(1, random_state=rng)[0]
        nominals[name] = val
    return nominals


def _get_nominals(definitions):
    """
    Gets default value for selected distribution type.

    Parameters
    ----------
    definitions : [dict]
        Describes distribution type and distribution parameters for each element.

    Returns : dict
    -------
    Values for provided distribution elements.

    """
    nominals = dict()
    distributions = generate_distributions(definitions, fix_to_mean=False)
    for name, distribution in distributions.items():
        if isinstance(distribution, DeterministicCharacterization):
            val = distribution.value
        elif isinstance(distribution, BetaDistribution):
            val = distribution.distribution.mean()
        elif isinstance(distribution, LognormalDistribution):
            val = distribution.distribution.median()
        elif isinstance(distribution, NormalDistribution):
            val = distribution.distribution.mean()
        elif isinstance(distribution, UniformDistribution):
            val = distribution.distribution.mean()
        else:
            raise ValueError("Distribution not recognized")
        nominals[name] = val
    return nominals


def _format_component_params(params, key, n_components):
    """Formats component leak frequency data for backend. """
    params_d01 = params[f'{key}_d01']
    params_d1 = params[f'{key}_d1']
    params_1 = params[f'{key}_1']
    params_10 = params[f'{key}_10']
    params_100 = params[f'{key}_100']
    result = {
        key: {
            'leak_sizes': [0.01, 0.1, 1, 10, 100],
            'quantity': n_components,
            'distribution_type': ['log_normal'] * 5,
            'distribution_parameters': [
                {'mu': params_d01['mu'], 'sigma': params_d01['sigma']},
                {'mu': params_d1['mu'], 'sigma': params_d1['sigma']},
                {'mu': params_1['mu'], 'sigma': params_10['sigma']},
                {'mu': params_10['mu'], 'sigma': params_1['sigma']},
                {'mu': params_100['mu'], 'sigma': params_100['sigma']},
            ]
        }
    }
    return result


def _format_occupant_data(distr_index, a, b, unit_conv):
    """Formats occupant data for backend."""
    # input distribution vals are indexes from DistrComboBox
    choices = ['normal', 'uniform', 'deterministic']
    distr = choices[distr_index]
    if distr == 'normal':
        data = {'mean': a * unit_conv, 'std_deviation': b * unit_conv}
    elif distr == 'uniform':
        data = {'lower_bound': a * unit_conv, 'upper_bound': b * unit_conv}
    else:
        data = {'value': a * unit_conv}
    return distr, data


def do_qra_analysis(analysis_id, params: dict, global_status_dict: dict):
    """
    Executes QRA analysis with parameters formatted for backend, and parses relevant results once complete.
    Also updates sub-process copy of settings.

    Parameters
    ----------
    analysis_id : int
        Unique analysis id
    params : dict
        Map of prepped parameters ready for analysis {slug: Characterization}

    Notes
    -----
    Called within new process via cloned state object.
    Dict is used to avoid sub-processing issues with cloning state object.

    """
    # Update this process' version of GUI settings
    app_settings.SESSION_DIR = params['session_dir']

    # multiprocessing does not support logging to same file. Must implement queue handler if this functionality is desired.
    proc_log = logging.getLogger(__name__)
    proc_log.setLevel(logging.INFO)

    # create output dir for this analysis
    als_name = helpers.convert_string_to_filename(params['analysis_name'])
    now_str = helpers.get_now_str()
    output_dirname = f'{now_str}_A{analysis_id:03d}_{als_name[0:10]}'
    output_dir = app_settings.SESSION_DIR.joinpath(output_dirname)
    Path.mkdir(output_dir, parents=True, exist_ok=True)

    results = dict(status=1, output_dir=output_dir, analysis_type='qra')

    rel_species = params['fuel']
    rel_phase = _get_phase(params['fluid_phase'])
    rel_t = params['rel_t']if rel_phase is None else None

    verbose = True

    # Prep component leak frequency, occupant group, and dispenser failure parameters into strongly-defined dicts.
    component_keys = ['compressor', 'vessel', 'valve', 'instrument', 'joint', 'hose', 'pipe', 'filter', 'flange',
                      'exchanger', 'vaporizer', 'arm', 'extra1', 'extra2']
    quantity_keys = ['n_compressors', 'n_vessels', 'n_valves', 'n_instruments', 'n_joints', 'n_hoses', 'pipe_l',
                     'n_filters', 'n_flanges', 'n_exchangers', 'n_vaporizers', 'n_arms', 'n_extra1', 'n_extra2']
    component_params = {}
    for comp_key, quant_key in zip(component_keys, quantity_keys):
        n_components = math.ceil(params[quant_key])
        comp_params = _format_component_params(params, comp_key, n_components)
        component_params.update(comp_params)

    leak_freq_defs = set_leak_frequency_definitions(uncertain_parameters=component_params,
                                                    uncertainty_type='aleatory',
                                                    species=rel_species,
                                                    saturated_phase=rel_phase,
                                                    include_defaults=False)
    leak_freq_nominals = _get_nominals(leak_freq_defs)
    component_set = set_components(leak_freq_nominals)

    # Note the key/name formatting is required by backend, including case.
    failure_defs = {
        'Fueling Failure: Nozzle, Pop-off': _convert_failure_dict('Fueling Failure: Nozzle, Pop-off', params['nozzle_po']),
        'Fueling Failure: Nozzle, Failure to close': _convert_failure_dict('Fueling Failure: Nozzle, Failure to close', params['nozzle_ftc']),
        'Fueling Failure: Manual valve, Failure to close': _convert_failure_dict('Fueling Failure: Manual valve, Failure to close', params['mvalve_ftc']),
        'Fueling Failure: Solenoid valves, Failure to close': _convert_failure_dict('Fueling Failure: Solenoid valves, Failure to close', params['svalve_ftc']),
        'Fueling Failure: Solenoid valves, Common-cause failure': _convert_failure_dict('Fueling Failure: Solenoid valves, Common-cause failure', params['svalve_ccf']),
        'Fueling Failure: Pressure-relief valve, Failure to open': _convert_failure_dict('Fueling Failure: Pressure-relief valve, Failure to open', params['pvalve_fto']),
        'Fueling Failure: Breakaway coupling, Failure to close': _convert_failure_dict('Fueling Failure: Breakaway coupling, Failure to close', params['coupling_ftc']),
        'Fueling Failure: Accident, Overpressure during fueling': _convert_failure_dict('Fueling Failure: Accident, Overpressure during fueling', params['acc_fuel_overp']),
        'Fueling Failure: Accident, Driveoff': _convert_failure_dict('Fueling Failure: Accident, Driveoff', params['acc_driveoff']),
    }
    failure_means = _get_nominals(failure_defs)

    failure_set = set_fueling_failures(failure_means,
                                       params['n_vehicles'],
                                       params['n_fuelings'],
                                       params['n_vehicle_days'])

    f_release_overrides = [params['override_d01'],
                           params['override_d1'],
                           params['override_1'],
                           params['override_10'],
                           params['override_100']]

    ign_data = params['ignition_data']
    immed_ign_probs = [elem['immed'] for elem in ign_data]
    delay_ign_probs = [elem['delay'] for elem in ign_data]
    ign_thresholds = [elem['max'] for elem in ign_data if elem['max'] is not None]
    ign_probs = {
        'flow_thresholds': ign_thresholds,
        'immed_ign_probs': immed_ign_probs,
        'delayed_ign_probs': delay_ign_probs,
    }

    occupant_sets = params['occupant_data']
    distance_conversions = [1, constants.inch, constants.foot, constants.yard]

    occupant_hrs = []
    loc_defs = {}
    for i, elem in enumerate(occupant_sets):
        conv = distance_conversions[elem['units']]
        n_occupants = int(elem['occupants'])
        descrip = f"{i}- {elem['descrip']}" if elem['descrip'].strip() != "" else f"Group {i}"
        xdistr, xparams = _format_occupant_data(elem['xd'], elem['xa'], elem['xb'], conv)
        ydistr, yparams = _format_occupant_data(elem['yd'], elem['ya'], elem['yb'], conv)
        zdistr, zparams = _format_occupant_data(elem['zd'], elem['za'], elem['zb'], conv)
        occupant_hrs += [elem['hours']] * n_occupants

        occ_group = {f"{descrip}": {
            "quantity": n_occupants,
            "distribution_type": (xdistr, ydistr, zdistr),
            "distribution_parameters": (xparams, yparams, zparams)
        }}
        loc_defs |= occ_group

    loc_sample_defs = set_occupant_location_definitions(loc_defs, uncertainty_type='aleatory')
    seed = params['seed']
    loc_noms = _get_rv_nominals(loc_sample_defs, seed=seed)
    locs = set_occupant_locations(loc_noms)

    # Set up mass flow as list (ignored if choked flow)
    leak_sizes_to_idx = ['ld01', 'ld1', 'l1', 'l10', 'l100']
    leak_id = params['mass_flow_leak']
    mass_flow_rates = None
    if leak_id in leak_sizes_to_idx:
        mass_leak_size_idx = leak_sizes_to_idx.index(leak_id)
        mass_flow_rates = [None] * 5
        mass_flow_rates[mass_leak_size_idx] = params['mass_flow']

    mach_convs = {'s0.2': 0.2, 's0.35': 0.35, 's0.7': 0.7, 's1': 1, 's1.4': 1.4, 's2': 2, 's3': 3, 's4': 4, 's5.2': 5.2}
    mach_speed = mach_convs[params['mach_speed']]

    pipe_d = params.get('pipe_d', None)
    if pipe_d is None:
        pipe_d = params['pipe_od'] - 2 * params['pipe_thick']

    try:
        # analysis_dict = {'status': 1}
        analysis_dict = qra.conduct_analysis(pipe_inner_diam=pipe_d,
                                             amb_temp=params['amb_t'],
                                             amb_pres=params['amb_p'],
                                             rel_species=rel_species,
                                             rel_temp=rel_t,
                                             rel_pres=params['rel_p'],
                                             rel_phase=rel_phase,
                                             ign_probs=ign_probs,
                                             locations=locs,
                                             occupant_hours=occupant_hrs,
                                             component_set=component_set,
                                             failure_set=failure_set,
                                             discharge_coeff=params['discharge'],
                                             detection_credit=params['detection'],
                                             overp_method=params['overp_method'],
                                             tnt_factor=params['tnt_factor'],
                                             bst_flame_speed=mach_speed,
                                             probit_thermal_id=params['thermal_probit'],
                                             exposure_time=params['exposure_t'],
                                             probit_overp_id=params['overp_probit'],
                                             nozzle_model=params['nozzle'],
                                             rel_angle=params['rel_angle'],
                                             rel_humid=params['humid'],
                                             mass_flow_rates=mass_flow_rates,
                                             ft_overrides=f_release_overrides,
                                             verbose=verbose,
                                             output_dir=output_dir,
                                             create_plots=True)

        results['status'] = 1

        analysis_dict['leak_results'] = [res for res in analysis_dict['leak_results']]
        analysis_dict['position_qrads'] /= 1000.  # W/m2 -> kW/m2
        analysis_dict['position_overps'] /= 1000.  # Pa -> kPa
        results['data'] = analysis_dict

    except ValueError as exc:
        msg = "Invalid input. {}".format(str(exc))
        results["message"] = msg
        results['status'] = -1
        proc_log.error(msg)

    except Exception as exc:
        msg = "Analysis failed: {}".format(str(exc))
        results["message"] = msg
        results['status'] = -1
        proc_log.error(exc)

    return results
