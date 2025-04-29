"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from typing import Union, Optional, Iterable

import numpy as np

from .component_leak_frequencies import get_leak_frequency_set_for_species
from .component_counts import get_component_counts_for_species
from .dispenser_failures import default_fueling_parameters, default_failure_set


default_leak_sizes = [0.01, 0.10, 1, 10, 100]

default_occupant_hours = 2000  # hours per year


def get_default_ignition_probs(species):
    """
    Returns the default ignition probabilites
    based on whether or not the species in question contains hydrogen

    Input
    -----
    species : str or dict
        Species in question,
        which is a string for a pure fuel
        (e.g., 'hydrogen' or 'h2')
        or a dictionary for a mixture
        (e.g., {'h2': 0.1, 'ch4': 0.9})

    Output
    ------
    ignition_probs : dict
        Dictonary with default ignition probabilities
        for both immediate and delayed ignition
        based on mass flow rate thresholds.
        The dictionary will be of the form:
        {'flow_thresholds': flow_thresholds,
        'immed_ign_probs': immed_ign_probs,
        'delayed_ign_probs': delayed_ign_probs}
    """
    h2_strs = ['h2', 'hydrogen']
    if isinstance(species, str):
        species_contains_h2 = any([h2_str in species.lower() for h2_str in h2_strs])
    elif isinstance(species, dict):
        mix_species = [specie.lower() for specie in species]
        species_contains_h2 = any([h2_str in mix_species for h2_str in h2_strs])
    else:
        raise TypeError(f'species input must be str or dict ({type(species)})')

    if species_contains_h2:
        flow_thresholds = [0.125, 6.25]  # kg/s
        immed_ign_probs = [0.008, 0.053, 0.23]
        delayed_ign_probs = [0.004, 0.027, 0.12]
    else:  # generic ignition probabilities
        flow_thresholds = [1, 50]  # kg/s
        immed_ign_probs = [0.007, 0.047, 0.2]
        delayed_ign_probs = [0.003, 0.023, 0.1]
    ignition_probs = {
        'flow_thresholds': flow_thresholds,
        'immed_ign_probs': immed_ign_probs,
        'delayed_ign_probs': delayed_ign_probs
    }
    return ignition_probs


def get_default_leak_frequencies(
        species:str,
        saturated_phase:Optional[Union[str, None]]=None,
        categories:Optional[Union[Iterable[str], None]]=None):
    """
    Generates the default leak frequencies based on lognormal distributions
    provided in `component_leak_frequencies.py`.

    Parameters
    ----------
    species : str
        Species in question. Default frequencies are only provided
        for pure fuel compositions (i.e. no blends).

    saturated_phase : str or None, optional
        State of the fuel. If `None`, gaseous fuel is assumed.
        Default is `None`.

    categories : list, optional
        List of component categories to include, if different from the
        defaults for the given species and phase. If `None`, defaults to
        the default components. Default is `None`.

    Returns
    -------
    default_freqs : dict
        Default frequencies for each leak size sorted by component category
    """
    if isinstance(species, dict):
        raise TypeError('Default leak frequencies are only provided for ' +
                        'pure fuel compositions (no blends)')

    params = get_leak_frequency_set_for_species(species, saturated_phase)
    if categories is None:
        counts = get_component_counts_for_species(species, saturated_phase)
        categories = counts.keys()
    categories = [category.lower() for category in categories]
    default_freqs = {}
    for component in categories:
        mus = params[component][:, 0]
        default_freqs[component] = np.exp(mus)
    return default_freqs


def create_default_component_parameters(species:Union[str, dict],
                                     saturated_phase:Optional[Union[str, None]]=None):
    """Generates the default `component_parameters` inputs to
    `qra.uncertainty.evaluate_qra_uq` using the default leak frequency values
    and component counts.

    Parameters
    ----------
    species : str or dict
        Species in question, which is a string for a pure fuel
        (e.g., 'hydrogen' or 'h2') or a dictionary for a mixture
        (e.g., {'h2': 0.1, 'ch4': 0.9})

    saturated_phase : str or None, optional
        State of the fuel. If `None`, gaseous fuel is assumed.
        Default is `None`.

    Returns
    -------
    all_component_details : dict
        All the uncertain parameter details for the default component
        list, ready for input to `qra.uncertainty.evaluate_qra_uq`
    """
    params = get_leak_frequency_set_for_species(species, saturated_phase)
    counts = get_component_counts_for_species(species, saturated_phase)
    all_component_details = dict.fromkeys(counts)
    for component in all_component_details.keys():
        component_params = params[component]
        all_component_details[component] = {
            'leak_sizes': default_leak_sizes,
            'quantity': counts[component],
            'distribution_type': ['log_normal']*len(default_leak_sizes),
            'distribution_parameters': [
                dict(zip(['mu', 'sigma'], comp_params))
                for comp_params in component_params
            ],
        }

    return all_component_details
