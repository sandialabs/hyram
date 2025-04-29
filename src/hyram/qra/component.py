"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
from typing import Iterable, Optional, Union

from .defaults import (default_leak_sizes,
                       get_component_counts_for_species,
                       get_default_leak_frequencies)


_valid_component_types = ['compressor', 'vessel', 'filter', 'flange', 'hose',
                          'joint', 'pipe', 'valve', 'instrument', 'exchanger',
                          'vaporizer', 'arm', 'extra1', 'extra2']


class Component:
    """
    Representation of components of a specific type or category.

    Parameters
    ----------
    category : str
        Name of component type, e.g. vessel

    quantity : int
        Number of components of this type

    leak_freqs : list of floats or None
        Annual leak frequencies for given component,
        for leak sizes of [0.01%, 0.1%, 1%, 10%, 100%].
        If None, then default leak frequencies will be used.
        Default is None.
    """
    def __init__(self, category, quantity, leak_freqs=None):
        category = category.lower()
        if category not in _valid_component_types:
            raise ValueError(f"Category must be one of: {_valid_component_types}")
        if not isinstance(quantity, int) or quantity < 0:
            raise ValueError(f"quantity ({quantity}) must be non-negative integer")
        if leak_freqs is not None:
            if len(leak_freqs) != len(default_leak_sizes):
                raise ValueError("Leak frequencies must be specified for all leak sizes")
        self.category = category
        self.quantity = int(quantity)
        self.leak_freqs = leak_freqs

    def __str__(self):
        return (f'Component {self.category} ({self.quantity}) freqs: ' +
                f'{self.leak_freqs}')

    def get_random_leak_frequencies_at_size(self, leak_size):
        """Retrieve random leak frequencies for a specified leak size."""
        leak_idx = default_leak_sizes.index(leak_size)
        return [self.leak_freqs[leak_idx]] * self.quantity


def get_leak_frequencies_at_size_for_set(component_set:Iterable[Component],
                                         leak_size:float):
    """Returns a dictionary of random leak frequencies for a given leak size,
    where the dictionary keys are the category name of the component.
    """
    leak_freqs = {}
    for component in component_set:
        leak_freqs[component.category] = sum(
            component.get_random_leak_frequencies_at_size(leak_size))
    return leak_freqs


def create_component_set(categories:Iterable[str],
                         quantities:Iterable[int],
                         species:Optional[Union[str, dict, None]]=None,
                         saturated_phase:Optional[Union[str, None]]=None,
                         frequencies:Optional[Union[Iterable[float], None]]=None):
    """
    Generates a component set for use in `qra.analysis.conduct_analysis`.

    Parameters
    ----------
    categories : List of str
        A list of component categories to include in the set.
        Valid options include:

            * 'compressor'
            * 'vessel'
            * 'filter'
            * 'flange'
            * 'hose'
            * 'joint'
            * 'pipe'
            * 'valve'
            * 'instrument'
            * 'exchanger'
            * 'vaporizer'
            * 'arm'
            * 'extra1'
            * 'extra2'

    quantities : Iterable of int
        A list of how many of each component in `categories` should be
        included in the set. Each element corresponds to the index given in
        `categories`. Must be the same length as `categories`.

    species : str or dict or None, optional
        Species in question, which is a string for a pure fuel
        (e.g., 'hydrogen' or 'h2') or a dictionary for a mixture
        (e.g., {'h2': 0.1, 'ch4': 0.9}).
        Default is `'hydrogen'`.

    saturated_phase : str or None, optional
        State of the fuel. If `None`, gaseous fuel is assumed.
        Default is `None`.

    frequencies : List of lists or None, optional
        Random leak frequencies for each leak size for each component.
        If specified, this is a list of lists where the index of the 
        top-level lists corresponds to the component category at that
        index given in `categories`, and the index of the second-level
        corresponds to leak sizes.
    """
    categories = [category.lower() for category in categories]
    if frequencies is None:
        if species is None:
            raise ValueError('Fuel species must be specified when ' +
                            'default leak frequencies used.')
        def_freqs = get_default_leak_frequencies(
            species=species,
            saturated_phase=saturated_phase,
            categories=categories)
        frequencies = [def_freqs[category] for category in categories]

    component_set = []
    for category, quantity, comp_freqs in zip(categories, quantities, frequencies):
        if category not in _valid_component_types:
            raise ValueError(f'Unrecognized component category: {category}')
        component_set.append(Component(category, int(quantity), comp_freqs))

    return component_set


def create_default_component_set(species:Union[str, dict],
                                 saturated_phase:Optional[Union[str, None]]=None):
    """
    Generates the default list of Component objects to
    `qra.analysis.conduct_analysis` using the default parameters

    Parameters
    ----------
    species : str or dict
        Species in question, which is a string for a pure fuel
        (e.g., 'hydrogen' or 'h2') or a dictionary for a mixture
        (e.g., {'h2': 0.1, 'ch4': 0.9})

    saturated_phase : str or None, optional
        State of the fuel. If `None`, gaseous fuel is assumed.
        Default is `None`.
    """
    categories_w_counts = get_component_counts_for_species(
        species, saturated_phase)
    categories = list(categories_w_counts.keys())
    counts = list(categories_w_counts.values())
    component_set = create_component_set(
        species=species,
        categories=categories,
        quantities=counts,
        saturated_phase=saturated_phase
    )
    return component_set
