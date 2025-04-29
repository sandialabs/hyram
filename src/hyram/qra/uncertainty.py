"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from copy import deepcopy
from re import search
from typing import Optional, Iterable, Union
import warnings

import numpy as np

from .uq.sampling import RandomStudy, LHSStudy
from .uq.distributions import (
    specify_distribution, convert_distributions_to_deterministic)
from .uq.parallel_evaluations import parallel_function_evaluations
from .component_failure import ComponentFailureSet, ComponentFailure
from .component import create_component_set
from .analysis import conduct_analysis
from .defaults import (default_fueling_parameters,
                       create_default_component_parameters)


def generate_uncertain_component_parameters(
        name:str,
        leak_sizes:Iterable[float],
        quantity:int,
        distribution_types:Iterable[str],
        distribution_parameters:dict,
        mass_flow_rates:Optional[Iterable[float]]=None
        ):
    """Helper function to create input dictionary for
     uncertainty quantification for a specified component type.

    Parameters
    ----------
    name : str
        Name of the component category

    leak_sizes : list
        Leak sizes to include for the components, in percent
        of diameter.

    quantity : int
        Number of components

    distribution_types : list
        Type of probability distribution to use for each leak
        size in `leak_sizes`. Valid entires are `'deterministic'`,
        `'normal'`, `'log_normal'`, `'trunc_normal'`, `'beta'`,
        and `'uniform'`.
    """
    component_specifications = {name: dict(
        leak_sizes=leak_sizes,
        quantity=quantity,
        distribution_type=distribution_types,
        distribution_parameters=distribution_parameters
    )}
    if mass_flow_rates is not None:
        component_specifications['mass_flow_rates'] = mass_flow_rates
    return component_specifications


def substitute_defaults(defaults:dict, substitutions:dict):
    """Overwrites the dictionary values for keys given in
    `substitutions`.
    """
    output_dict = deepcopy(defaults)
    output_dict.update(substitutions)
    return output_dict


def add_dist_definitions(definitions:dict,
                        names:Iterable[str],
                        dist_types:Iterable[str],
                        dist_parameters:Iterable[dict],
                        uncertainty_type:str
                        ):
    """Adds distribution definitions in-place to the `definitions` argument,
    for use in the sampling analysis.
    """
    for i, name in enumerate(names):
        keys = ['name', 'distribution_type']
        keys.extend(dist_parameters[i].keys())
        vals = [name, dist_types[i],
                *dist_parameters[i].values()]
        definitions[name] = dict(zip(keys, vals))
        if dist_types[i] != 'deterministic':
            definitions[name]['uncertainty_type'] = uncertainty_type
    return definitions


def create_fueling_failure_dist_name(failure_type:str, failure_mode:str):
    """Automatically generates a fueling failure distribution name for
    the sampling analysis, to be parsed later in the QRA analysis.
    """
    return f'Fueling Failure: {failure_type}, {failure_mode}'


def set_fueling_failure_definitions(uncertain_parameters:dict,
                                    uncertainty_type:str):
    """Converts parameters for uncertain aspects of fueling failure into
    the sampling definitions needed for the UQ sampling procedure

    Parameters
    ----------
    uncertain_parameters : dict
        Uncertain parameters of fueling failure

    uncertainty_type : str
        Type of uncertainty in the specifications. Valid entries are
        `aleatory` or `epistemic`.

    Returns
    -------
    definitions : dict
        Sampling definitions of the uncertain fueling failure parameters
    """
    fueling_failure_parameters = substitute_defaults(
        default_fueling_parameters, uncertain_parameters)
    failure_type_names = []
    dist_types = []
    dist_params = []
    for failure_type, parameters in fueling_failure_parameters.items():
        if isinstance(parameters['mode'], list):
            for i, failure_mode in enumerate(parameters['mode']):
                failure_type_names.append(create_fueling_failure_dist_name(
                    failure_type=failure_type,
                    failure_mode=failure_mode
                ))
                dist_types.append(parameters['distribution_type'][i])
                dist_params.append(parameters['distribution_parameters'][i])
        else:
            failure_type_names.append(create_fueling_failure_dist_name(
                failure_type=failure_type,
                failure_mode=parameters['mode']
            ))
            dist_types.append(parameters['distribution_type'])
            dist_params.append(parameters['distribution_parameters'])
    definitions = add_dist_definitions(definitions=dict(),
                                       names=failure_type_names,
                                       dist_types=dist_types,
                                       dist_parameters=dist_params,
                                       uncertainty_type=uncertainty_type)
    return definitions


def create_leak_freq_dist_name(component:str,
                               num:int,
                               leak_size:float,
                               mass_flow_rate:Union[float, None]):
    """Generate the string of a particular leak for the
    leak distribution list.
    """
    if mass_flow_rate is None:
        return f'Component: {component} #{num}, {leak_size}% leak'
    else:
        return (f'Component: {component} #{num}, {leak_size}% leak, ' +
                f'MFR={mass_flow_rate} kg/s')


def set_leak_frequency_definitions(uncertain_parameters:dict,
                                   uncertainty_type:str,
                                   species:Union[str, dict],
                                   saturated_phase:Union[str, None],
                                   include_defaults:Optional[bool]=True):
    """Converts parameters for uncertain aspects of component leak
    frequencies into the sampling definitions needed for the
    uncertainty quantification sampling procedure.

    Parameters
    ----------
    uncertain_parameters : dict
        Uncertain parameters of component leak frequencies. If a particular
        component for the given species/phase is not provided, default values
        are used.

    uncertainty_type : str
        Type of uncertainty . Valid entries are
        `aleatory` or `epistemic`.

    species : str or dict
        Fuel species to be used. A string is provided for a pure fuel
        (e.g., 'hydrogen' or 'h2'), and a dictionary for a mixture
        (e.g., {'h2': 0.1, 'ch4': 0.9}).

    saturated_phase : str or None, optional
        State of the fuel. If `None`, gaseous fuel is assumed.
        Default is `None`.

    include_defaults : bool
        If `True`, components not specified in `uncertain_parameters` are
        set to their defaults for the given species. This argument is ignored
        if `species` is provided as a dictionary, as a fuel blend is assumed.
        Default is `True`.

    Returns
    -------
    definitions : dict
        Sampling definitions of the uncertain component leak frequency
        parameters
    """
    if include_defaults:
        if isinstance(species, str):
            default_leak_freq_params = create_default_component_parameters(
                species, saturated_phase)
            uncertain_parameters = substitute_defaults(
                default_leak_freq_params, uncertain_parameters)
        else:
            warnings.warn('Default components not provided for fuel blends. ' +
                          '`include_defaults=True` ignored.')
    definitions = dict()
    for component, parameters in uncertain_parameters.items():
        leak_sizes = parameters['leak_sizes']
        distribution_types = parameters['distribution_type']
        num_leak_sizes = len(leak_sizes)

        if 'mass_flow_rates' in parameters:
            mass_flow_rates = parameters['mass_flow_rates']
        else:
            mass_flow_rates = [None] * num_leak_sizes

        if np.size(distribution_types) != num_leak_sizes:
            if np.size(distribution_types) == 1:
                distribution_types = [distribution_types] * num_leak_sizes
        comp_names = []
        for component_num in range(1, parameters['quantity']+1):
            for i in range(num_leak_sizes):
                comp_names.append(create_leak_freq_dist_name(
                    component=component,
                    num=component_num,
                    leak_size=leak_sizes[i],
                    mass_flow_rate=mass_flow_rates[i]))
        comp_dist_types = (distribution_types
                         * parameters['quantity'])
        comp_dist_params = (parameters['distribution_parameters']
                          * parameters['quantity'])
        definitions = add_dist_definitions(definitions=definitions,
                                           names=comp_names,
                                           dist_types=comp_dist_types,
                                           dist_parameters=comp_dist_params,
                                           uncertainty_type=uncertainty_type)
    return definitions


def create_occupant_location_dist_name(group, direction, num):
    """Automatically generates an occupant location distribution name
    for the sampling analysis, to be parsed later in the QRA analysis.
    """
    return f'Occupant Location: {group} #{num}, {direction}-direction'


def set_occupant_location_definitions(uncertain_parameters:dict,
                                      uncertainty_type:str):
    """Converts parameters from uncertain occupant locations into the
    sampling definitions needed for the UQ sampling procedure

    Parameters
    ----------
    uncertain_parameters : dict
        Uncertain parameters of occupant location

    uncertainty_type : str
        Type of uncertainty in the specifications. Valid entries are
        `aleatory` or `epistemic`.

    Returns
    -------
    definitions : dict
        Sampling definitions of the uncertain occupant location
    """
    def enumerate_param_item(parameter, quantity):
        """Distributes an item in parameters list to the correct number of
        elements based on whether the parameter is defined uniformly for
        x/y/z, or separately by direction."""
        if np.size(parameter) == 1 or isinstance(parameter, dict):
            return [parameter] * quantity * 3
        elif np.size(parameter) == 3:
            return list(parameter) * quantity
        else:
            return ValueError('Uncertain occupant location parameters must ' +
                              'be defined by direction (length 3) or ' +
                              'uniformly for all directions.')

    definitions = dict()
    for group_name, parameters in uncertain_parameters.items():
        group_location_names = []
        for occupant_num in range(1, parameters['quantity']+1):
            for dir in ['x', 'y', 'z']:
                group_location_names.append(
                    create_occupant_location_dist_name(
                        group=group_name,
                        direction=dir,
                        num=occupant_num))
        group_dist_types = enumerate_param_item(
            parameters['distribution_type'], parameters['quantity'])
        group_dist_params = enumerate_param_item(
            parameters['distribution_parameters'], parameters['quantity'])
        definitions = add_dist_definitions(definitions=definitions,
                                           names=group_location_names,
                                           dist_types=group_dist_types,
                                           dist_parameters=group_dist_params,
                                           uncertainty_type=uncertainty_type)
    return definitions


def stack_locations_xyz(locations:dict):
    """Combines keys in the uncertain location parameters dictionary
    into the x/y/z directions of each occupant into a single key.
    """
    sorted_list = sorted(locations.items())
    sorted_locations = [entry[1] for entry in sorted_list]
    stacked_locations = [tuple(sorted_locations[i:i+3])
                         for i in range(0, len(sorted_locations), 3)]
    return stacked_locations


def generate_distributions(definitions:dict, fix_to_mean:bool):
    """Converts distribution definitions into distribution objects provided
    by the `qra.uq.uncertainty_definitions` module.
    """
    distributions = dict()
    for name, definition in definitions.items():
        distributions[name] = specify_distribution(definition)
    if fix_to_mean:
        distributions = convert_distributions_to_deterministic(distributions)
    return distributions


def get_inputs_per_sample(samples:dict, num_samples:int):
    """Creates a list of dictionaries, where each dictionary has keys
    corresponding to the uncertain parameters, and each list element
    corresponding to a given sample that will be applied to the QRA.
    """
    all_inputs = []
    names = list(samples.keys())
    parameters = list(samples.values())

    for sample_num in range(num_samples):
        all_inputs.append({name: sample_vals[sample_num]
                           for (name, sample_vals) in zip(names, parameters)})
    return all_inputs


def parse_uncertain_parameter_type(uncertain_parameters:dict, type):
    """Parses the names of an uncertain parameter type"""
    return {k: v for k, v in uncertain_parameters.items()
            if f'{type}:' in k}


def parse_fueling_failures(uncertain_parameters:dict):
    """Parses the names for each fueling failure uncertain parameter
    back into variables to be applied to the QRA.
    """
    fueling_failures = parse_uncertain_parameter_type(
        uncertain_parameters, 'Fueling Failure')
    failure_types = []
    failure_modes = []
    failure_probs = []
    for name, probability in fueling_failures.items():
        failure_type, mode = search(
            ': (.+), (.+)', name).group(1,2)
        failure_types.append(failure_type)
        failure_modes.append(mode)
        failure_probs.append(probability)
    return (failure_types, failure_modes, failure_probs)


def parse_components(uncertain_parameters):
    """Parses the names for each component uncertain parameter
    back into variables to be applied to the QRA.
    """
    components = parse_uncertain_parameter_type(
        uncertain_parameters, 'Component')
    total_num_components = len(components)
    component_names_unsorted = np.empty(total_num_components, dtype=object)
    leak_sizes_unsorted = np.empty(total_num_components, dtype=float)
    mass_flow_rates_unsorted = np.empty(total_num_components, dtype=float)
    frequencies_unsorted = np.empty(total_num_components, dtype=float)
    for i, (name, freq) in enumerate(components.items()):
        component_name, leak_size, mfr_clause = search(
            ': (.+) #.+, (.+)% leak(.*)', name).group(1, 2, 3)
        if len(mfr_clause):
            mass_flow_rate = search(', MFR=(.+) kg/s', mfr_clause).group(1)
        else:
            mass_flow_rate = np.nan
        component_names_unsorted[i] = component_name
        leak_sizes_unsorted[i] = leak_size
        mass_flow_rates_unsorted[i] = mass_flow_rate
        frequencies_unsorted[i] = freq
    component_categories, component_counts_all_sizes = np.unique(
        component_names_unsorted, return_counts=True)
    leak_sizes = np.empty(len(component_categories), dtype=object)
    mass_flow_rates = np.empty(len(component_categories), dtype=object)
    frequencies = {}
    for i, category in enumerate(component_categories):
        category_idxs = component_names_unsorted==category
        leak_sizes[i], leak_size_idxs = np.unique(
            leak_sizes_unsorted[category_idxs], return_inverse=True)
        category_mass_flow_rates = np.unique(
            mass_flow_rates_unsorted[category_idxs])
        if np.isnan(category_mass_flow_rates).all():
            mass_flow_rates[i] = None
        else:
            mass_flow_rates[i] = category_mass_flow_rates
        category_freqs = frequencies_unsorted[category_idxs]
        freqs_at_size = np.empty(len(leak_sizes[i]), dtype=object)
        for j, leak_size in enumerate(leak_sizes[i]):
            freqs_at_size[j] = np.sum(category_freqs[leak_size_idxs==j])
        frequencies[category] = freqs_at_size
    leak_sizes_by_category = np.array(
        [len(cat_leak_size) for cat_leak_size in leak_sizes])
    component_counts = np.array(component_counts_all_sizes / leak_sizes_by_category, dtype=int)
    return (component_categories, component_counts, leak_sizes,
            frequencies, mass_flow_rates)


def parse_occupant_locations(uncertain_parameters):
    """Parses the names for each occupant location parameter,
    still separated by direction.
    """
    return parse_uncertain_parameter_type(
        uncertain_parameters, 'Occupant Location')


def set_fueling_failures(uncertain_parameters:dict,
                         num_vehicles:int,
                         daily_fuelings:int,
                         vehicle_days:float):
    """Interprets the returns from a sampling analysis into a
     `ComponentFailureSet` object used for the QRA.

    Parameters
    ----------
    uncertain_parameters : dict
        Contains values for uncertain parameters

    num_vehicles : int
        Number of vehicles

    daily_fuelings : int
        Number of times a vehicle is filled per day

    vehicle_days : float
        Number of days in which the vehicles are filled

    Returns
    -------
    ComponentFailureSet
        Updated component failure set with sampled parameter failure
        probability values

    """
    failure_types, failure_modes, failure_probs = parse_fueling_failures(
        uncertain_parameters)
    failures = [ComponentFailure(comp, mode, value) for (comp, mode, value) in
                zip(failure_types, failure_modes, failure_probs)]
    return ComponentFailureSet(
        num_vehicles=num_vehicles,
        daily_fuelings=daily_fuelings,
        vehicle_days=vehicle_days,
        failures=failures)


def set_components(uncertain_parameters:dict):
    """Interprets the returns from the sampling analysis into a component set
    used for the QRA.

    Parameters
    ----------
    uncertain_parameters : dict
        Uncertainty parameters associated with component leak frequencies
        varied by sampling analysis.

    Returns
    -------
    components : list
        List of Component objects, with each object specifying the component
        category, quantity, and leak information.
    """
    (component_names, component_counts, leak_sizes,
     frequencies, mass_flow_rates) = parse_components(uncertain_parameters)
    components = create_component_set(
        categories=component_names,
        quantities=component_counts,
        frequencies=[frequencies[component] for component in component_names])
    return components


def set_occupant_locations(uncertain_parameters):
    """Interprets the returns from a sampling analysis into a list of
     locations to be used for the QRA.

    Parameters
    ----------
    uncertain_parameters : dict
        Contains values for uncertain parameters

    Returns
    -------
    locations : list
        List of tuples indicating the xyz locations of all occupants
    """
    locations = parse_occupant_locations(uncertain_parameters)
    stacked_locations = stack_locations_xyz(locations)
    return stacked_locations


def calc_qra_at_distance(uncertain_parameters:dict,
                         static_parameters:dict):
    """Takes a single set of uncertainty samples, maps the uncertain
     parameters to the QRA framework, and conducts the QRA.

    Parameters
    ----------
    uncertain_parameters : dict
        Values of uncertain parameters.

    static_parameters : dict
        Contains QRA analysis parameters not varied by sampling analysis

    Returns
    -------
    results : dict
        QRA analysis results
    """
    component_failure_set = set_fueling_failures(
        uncertain_parameters,
        static_parameters['num_vehicles'],
        static_parameters['daily_fuelings'],
        static_parameters['vehicle_days'])
    components = set_components(uncertain_parameters)
    locations = set_occupant_locations(uncertain_parameters)

    # update this as more static parameters are allowed to be uncertain
    # in future versions
    results = conduct_analysis(
        pipe_inner_diam=static_parameters['pipe_inner_diam'],
        amb_temp=static_parameters['amb_temp'],
        amb_pres=static_parameters['amb_pres'],
        rel_temp=static_parameters['rel_temp'],
        rel_pres=static_parameters['rel_pres'],
        rel_species=static_parameters['rel_species'],
        locations=locations,
        rel_phase=static_parameters['rel_phase'],
        component_set=components,
        failure_set=component_failure_set,
        occupant_hours=static_parameters['occupant_hours'],
        ft_overrides=static_parameters['ft_overrides'],
        ign_probs=static_parameters['ign_probs'],
        discharge_coeff=static_parameters['discharge_coeff'],
        mass_flow_rates=static_parameters['mass_flow_rates'],
        detection_credit=static_parameters['detection_credit'],
        overp_method=static_parameters['overp_method'],
        tnt_factor=static_parameters['tnt_factor'],
        bst_flame_speed=static_parameters['bst_flame_speed'],
        probit_thermal_id=static_parameters['probit_thermal_id'],
        exposure_time=static_parameters['exposure_time'],
        probit_overp_id=static_parameters['probit_overp_id'],
        nozzle_model=static_parameters['nozzle_model'],
        rel_angle=static_parameters['rel_angle'],
        rel_humid=static_parameters['rel_humid'],
        verbose=static_parameters['verbose'],
        output_dir=static_parameters['output_dir'],
        create_plots=static_parameters['create_plots'])

    return results


def create_study(study_type:str,
                 uncertainty_type:str,
                 num_samples:int,
                 random_seed:Optional[Union[int, None]]=None):
    """Generates a sampling study object specified in `qra.uq.sampling`
    module for either aleatoric or epistemic uncertainty

    Parameters
    ----------
    study_type : str
        Type of uncertainty study to be conducted. Valid inputs
        are `'random'` for a random sampling study, or
        `'lhs'` for a Latin hypercube sampling study.

    uncertainty_type : str
        Type of uncertainty to use in the uncertainty quantification.
        Valid inputs are `'aleatory'` or `'epistemic'`.

    num_samples : int
        Number of samples per parameter

    random_seed : int or None
        A seed number used to create a random number generator state to
        allow for reproducible results. If `None`, the state itself will
        be random. Default is `None`.

    Returns
    -------
    RandomStudy or LHSStudy object
    """
    # TODO: currently only set up for a single loop
    # (i.e. single uncertainty type), expand this in the future to allow
    # double loop operation
    if uncertainty_type == 'aleatory':
        num_aleatory_samples = num_samples
        num_epistemic_samples = 0
    elif uncertainty_type == 'epistemic':
        num_aleatory_samples = 0
        num_epistemic_samples = num_samples

    if study_type.lower() == 'random':
        return RandomStudy(num_aleatory_samples=num_aleatory_samples,
                    num_epistemic_samples=num_epistemic_samples,
                    random_seed=np.random.default_rng(random_seed))
    elif study_type.lower() == 'lhs':
        return LHSStudy(num_aleatory_samples=num_aleatory_samples,
                        num_epistemic_samples=num_epistemic_samples,
                        random_seed=np.random.default_rng(random_seed))


def generate_uq_samples(study:Union[LHSStudy, RandomStudy],
                        distributions:dict):
    """Performs the sampling analysis.

    Parameters
    ----------
    study : LHSStudy or RandomStudy
        Type of uncertainty study to be conducted.

    distributions : dict
        Distribution specifications for all the variables to be sampled in the
        uncertainty study.

    Returns
    -------
    dict
        A dictionary containing the samples for each variable specified
        in `distributions`.
    """
    study.add_variables(distributions)
    return study.create_variable_sample_sheet()


def evaluate_qra_uq(species:Union[str, dict],
                    study_type:str,
                    num_samples:int,
                    fueling_parameters:dict,
                    component_parameters:dict,
                    location_parameters:dict,
                    static_parameters:dict,
                    uncertainty_type:str,
                    saturated_phase:Optional[Union[str, None]]=None,
                    include_defaults:Optional[bool]=True,
                    fix_to_mean:Optional[bool]=False,
                    random_seed:Optional[Union[int, None]]=None):
    """Evaluates ensemble of parameter samples in QRA analysis

    Parameters
    ----------
    species : str or dict
        Fuel species to be used, which is a string for a pure fuel
        (e.g., 'hydrogen' or 'h2') or a dictionary for a mixture
        (e.g., {'h2': 0.1, 'ch4': 0.9})

    study_type : str
        Type of sampling analysis study to perform.
        Valid options are random sampling (`'random'`) or Latin hypercube
        sampling (`'lhs'`).

    num_samples : int
        Number of samples to include in the study

    fueling_parameters : dict
        Uncertainty parameters associated with fueling failures varied by
        sampling analysis

    component_parameters : dict
        Uncertainty parameters associated with component leak frequencies
        varied by sampling analysis

    location_parameters : dict
        Uncertainty parameters associated with occupant location varied by
            sampling analysis

    static_parameters : dict
        QRA analysis parameters not varied by sampling analysis

    locations : list
        List of Cartesian coordinates in the form [x, y, z] for each location

    saturated_phase : str or None, optional
        State of the fuel. If `None`, gaseous fuel is assumed.
        Default is `None`.

    include_defaults : bool, optional
        If `True`, components not specified in `component_parameters` are
        set to their defaults for the given species. This argument is ignored
        if `species` is provided as a dictionary, as a fuel blend is assumed.
        Default is `True`.

    fix_to_mean : bool, optional
        Flag for if only mean parameter values should be used instead of
        distributions. This will set all uncertain quantities to be
        determistic.

    random_seed : int or None
        A seed number used to create a random number generator state to
        allow for reproducible results. If `None`, the state itself will
        be random. Default is `None`.

    Returns
    -------
    results_list : list
        Result dictionaries for every parameter sample

    uq_samples : dict
        Sampled values of all uncertain parameters
    """
    leak_frequency_definitions = set_leak_frequency_definitions(
        uncertain_parameters=component_parameters,
        uncertainty_type=uncertainty_type,
        species=species,
        saturated_phase=saturated_phase,
        include_defaults=include_defaults)

    fueling_failure_definitions = set_fueling_failure_definitions(
        uncertain_parameters=fueling_parameters,
        uncertainty_type=uncertainty_type)

    occupant_location_definitions = set_occupant_location_definitions(
        uncertain_parameters=location_parameters,
        uncertainty_type=uncertainty_type)

    study = create_study(study_type,
                         uncertainty_type,
                         num_samples,
                         random_seed)
    study_distributions = generate_distributions(
        leak_frequency_definitions | fueling_failure_definitions | occupant_location_definitions,
        fix_to_mean)
    uq_samples = generate_uq_samples(study, study_distributions)
    inputs_per_sample = get_inputs_per_sample(uq_samples, num_samples)

    results = \
        parallel_function_evaluations(
            calc_qra_at_distance, inputs_per_sample, additional_inputs={
                'static_parameters': static_parameters})

    return results, uq_samples
