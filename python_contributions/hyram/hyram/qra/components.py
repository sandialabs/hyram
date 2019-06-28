#  Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
#  Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
#  .
#  This file is part of HyRAM (Hydrogen Risk Assessment Models).
#  .
#  HyRAM is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#  .
#  HyRAM is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#  .
#  You should have received a copy of the GNU General Public License
#  along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

from .leaks import LeakSet
from .data_sources import component_data


def create_components_from_dict(input_dict):
    """
    Convenience function for initiating creation of components from dict containing component counts.

    Parameters
    ----------
    input_dict : dict

    Returns
    -------
    dict of component sets

    """
    # Get optional component param counts.
    # If missing from the dict, assume 0.
    num_compressors = int(input_dict.get('num_compressors', 0))
    num_cylinders = int(input_dict.get('num_cylinders', 0))
    num_valves = int(input_dict.get('num_valves', 0))
    num_instruments = int(input_dict.get('num_instruments', 0))
    num_joints = int(input_dict.get('num_joints', 0))
    num_hoses = int(input_dict.get('num_hoses', 0))
    pipe_length = int(input_dict.get('pipe_length', 0))
    num_filters = int(input_dict.get('num_filters', 0))
    num_flanges = int(input_dict.get('num_flanges', 0))
    num_extra_comp1 = int(input_dict.get('num_extra_comp1', 0))
    num_extra_comp2 = int(input_dict.get('num_extra_comp2', 0))

    components_dict = create_components(num_compressors=num_compressors, num_cylinders=num_cylinders,
                                        num_valves=num_valves, num_instruments=num_instruments,
                                        num_joints=num_joints, num_hoses=num_hoses, pipe_length=pipe_length,
                                        num_filters=num_filters, num_flanges=num_flanges,
                                        num_extra_comp1=num_extra_comp1, num_extra_comp2=num_extra_comp2)

    return components_dict


def create_components(
        num_compressors, num_cylinders, num_valves, num_instruments,
        num_joints, num_hoses, pipe_length, num_filters, num_flanges,
        num_extra_comp1, num_extra_comp2
):
    """
    Create component sets (container objects of components) for each component type, based on specified number.

    Parameters
    ----------
    num_compressors : int
    num_cylinders : int
    num_valves : int
    num_instruments : int
    num_joints : int
    num_hoses : int
    num_filters : int
    num_flanges : int
    num_extra_comp1 : int
    num_extra_comp2 : int

    Returns
    -------
    List of ComponentSet objects for all 10 categories

    """
    # Create sets of components based on # specified. If no number, or # is empty, create none
    compressor_set = ComponentSet('compressor', num_compressors)
    cylinder_set = ComponentSet('cylinder', num_cylinders)
    valve_set = ComponentSet('valve', num_valves)
    instrument_set = ComponentSet('instrument', num_instruments)
    joint_set = ComponentSet('joint', num_joints)
    hose_set = ComponentSet('hose', num_hoses)
    pipe_set = ComponentSet('pipe', pipe_length)
    filter_set = ComponentSet('filter', num_filters)
    flange_set = ComponentSet('flange', num_flanges)
    extra_comp1_set = ComponentSet('extra comp 1', num_extra_comp1)
    extra_comp2_set = ComponentSet('extra comp 2', num_extra_comp2)

    return [compressor_set, cylinder_set, valve_set,
            instrument_set, joint_set, hose_set, pipe_set, filter_set, flange_set,
            extra_comp1_set, extra_comp2_set]


class ComponentSet(object):
    """
    Representation of 1+ components of a specific type or category.
    Has associated LeakSet which defines all leaks for this component type.

    Parameters
    ----------
    category : str
        Name of component type, e.g. cylinder

    num_components : int
        Number of components in this category/type

    leak_probs : list
        List of dicts containing probability data for this component in order of increasing leak size
        [{mu, sigma}, ...]

    Attributes
    -----------
    leaks : LeakSet

    """

    def __init__(self, category, num_components, leak_probs=None):
        valid, error_msg = self.validate_parameters(category, num_components, leak_probs)
        if not valid:
            raise ValueError(error_msg)

        self.category = category
        self.num_components = num_components

        # if self.num_components:
        # Retrieve known leak frequency data. Formatted as {mu[], sigma[]}
        if leak_probs is None:
            self.leak_probs = component_data.LEAK_PROBABILITIES[self.category]
        else:
            self.leak_probs = leak_probs
        self.leak_set = LeakSet(parent=self, component_leak_probs=self.leak_probs)

    def validate_parameters(self, category, num_components, leak_probs):
        valid = True
        msg = ""
        if type(category) != str:
            valid = False
            msg = "Category must be string"
        elif num_components < 0:
            valid = False
            msg = "# of components must be 0 or greater"
        return valid, msg

    def __str__(self):
        return '{} set containing {} components'.format(self.category, self.num_components)

    def get_description(self):
        return '{} set of {} with leak data:\n{}\n'.format(self.category, self.num_components, self.leak_set)

    @property
    def has_components(self):
        """ Whether any components of this type are present. False if none found i.e. num_components==0. """
        return bool(self.num_components)

    def get_freq_for_size(self, leak_size):
        freq = 0
        if self.has_components:
            leak = self.get_leak(leak_size)
            freq = leak.mean * self.num_components
        return freq

    def get_leak(self, leak_size):
        """ Retrieve leak object at specified size for this component type. """
        if self.has_components:
            return self.leak_set.get_leak(leak_size)
        else:
            return None

    def leak_mean(self, leak_size):
        """ Retrieve probability mean for given leak size. """
        if self.has_components:
            leak = self.get_leak(leak_size)
            return leak.mean
        else:
            return 0.
