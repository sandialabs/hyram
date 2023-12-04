"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np
from . import _utils

from .data import component_data
from .leaks import Leak


def init_component_sets(species, phase, leak_sizes=None,
                        n_compressors=0, n_vessels=0, n_valves=5, n_instruments=3, n_joints=35, n_hoses=1, n_pipes=20, n_filters=0,
                        n_flanges=0, n_exchangers=0, n_vaporizers=0, n_arms=0, n_extra1s=0, n_extra2s=0,
                        compressor_ps=None, vessel_ps=None, valve_ps=None, instrument_ps=None, joint_ps=None,
                        hose_ps=None, pipe_ps=None, filter_ps=None, flange_ps=None, exchanger_ps=None,
                        vaporizer_ps=None, arm_ps=None, extra1_ps=None, extra2_ps=None,
                        fuel_component_data=None):
    if fuel_component_data is None:
        fuel_component_data = component_data.h2_gas_params

    if leak_sizes is None:
        leak_sizes = _utils.get_default_leak_sizes()

    if compressor_ps is None:
        compressor_ps = fuel_component_data['compressor']
    if vessel_ps is None:
        vessel_ps = fuel_component_data['vessel']
    if valve_ps is None:
        valve_ps = fuel_component_data['valve']
    if instrument_ps is None:
        instrument_ps = fuel_component_data['instrument']
    if joint_ps is None:
        joint_ps = fuel_component_data['joint']
    if hose_ps is None:
        hose_ps = fuel_component_data['hose']
    if pipe_ps is None:
        pipe_ps = fuel_component_data['pipe']
    if filter_ps is None:
        filter_ps = fuel_component_data['filter']
    if flange_ps is None:
        flange_ps = fuel_component_data['flange']
    if exchanger_ps is None:
        exchanger_ps = fuel_component_data['exchanger']
    if vaporizer_ps is None:
        vaporizer_ps = fuel_component_data['vaporizer']
    if arm_ps is None:
        arm_ps = fuel_component_data['arm']
    if extra1_ps is None:
        extra1_ps = fuel_component_data['extra1']
    if extra2_ps is None:
        extra2_ps = fuel_component_data['extra2']

    component_sets = [
        ComponentSet(category='compressor', num_components=n_compressors, species=species, saturated_phase=phase,
                     leak_frequency_lists=compressor_ps, leak_sizes=leak_sizes),
        ComponentSet(category='vessel', num_components=n_vessels, species=species, saturated_phase=phase,
                     leak_frequency_lists=vessel_ps, leak_sizes=leak_sizes),
        ComponentSet(category='valve', num_components=n_valves, species=species, saturated_phase=phase,
                     leak_frequency_lists=valve_ps, leak_sizes=leak_sizes),
        ComponentSet(category='instrument', num_components=n_instruments, species=species, saturated_phase=phase,
                     leak_frequency_lists=instrument_ps, leak_sizes=leak_sizes),
        ComponentSet(category='joint', num_components=n_joints, species=species, saturated_phase=phase,
                     leak_frequency_lists=joint_ps, leak_sizes=leak_sizes),
        ComponentSet(category='hose', num_components=n_hoses, species=species, saturated_phase=phase,
                     leak_frequency_lists=hose_ps, leak_sizes=leak_sizes),
        ComponentSet(category='pipe', num_components=n_pipes, species=species, saturated_phase=phase,
                     leak_frequency_lists=pipe_ps, leak_sizes=leak_sizes),
        ComponentSet(category='filter', num_components=n_filters, species=species, saturated_phase=phase,
                     leak_frequency_lists=filter_ps, leak_sizes=leak_sizes),
        ComponentSet(category='flange', num_components=n_flanges, species=species, saturated_phase=phase,
                     leak_frequency_lists=flange_ps, leak_sizes=leak_sizes),
        ComponentSet(category='exchanger', num_components=n_exchangers, species=species, saturated_phase=phase,
                     leak_frequency_lists=exchanger_ps, leak_sizes=leak_sizes),
        ComponentSet(category='vaporizer', num_components=n_vaporizers, species=species, saturated_phase=phase,
                     leak_frequency_lists=vaporizer_ps, leak_sizes=leak_sizes),
        ComponentSet(category='arm', num_components=n_arms, species=species, saturated_phase=phase,
                     leak_frequency_lists=arm_ps, leak_sizes=leak_sizes),
        ComponentSet(category='extra1', num_components=n_extra1s, species=species, saturated_phase=phase,
                     leak_frequency_lists=extra1_ps, leak_sizes=leak_sizes),
        ComponentSet(category='extra2', num_components=n_extra2s, species=species, saturated_phase=phase,
                     leak_frequency_lists=extra2_ps, leak_sizes=leak_sizes)
    ]
    return component_sets


class ComponentSet:
    """
    Representation of 1+ components of a specific type or category.
    Has associated LeakSet which defines all leaks for this component type.

    Parameters
    ----------
    category : str
        Name of component type, e.g. vessel

    num_components : int
        Number of components in this category/type

    species : str or dict
        If string, expect one of {'h2', 'ch4', 'c3h8'}.
        If dict, fluid is blend with key of species, value of concentration.


    saturated_phase : None or str
        CoolProp parameter for fuel phase.
        Note that these values may be unintuitive. 'gas' and 'liquid' indicate saturated state and will
        use the liquid leak frequency data.
        Typical h2 gas should have parameter value of None.

    leak_frequency_lists: list or None
        List of lists; Leak frequency parameters for each leak size of format [[mu, sigma]]

    leak_sizes : list or None
        List of floats representing % leakage

    Attributes
    -----------
    category : str
        name of component type, e.g. 'compressor'

    num_components : int
        number of components in analysis

    leaks : dict of {size: LeakSet}

    """
    
    default_leak_sizes = _utils.get_default_leak_sizes()
    
    def __init__(self, category, num_components, species, saturated_phase=None, leak_frequency_lists=None, leak_sizes=None):
        if type(category) != str:
            raise ValueError("Category must be string")
        if num_components < 0:
            raise ValueError("# of components must be 0 or greater")

        if leak_sizes is None:
            leak_sizes = self.default_leak_sizes
        
        leak_sizes.sort()
        if leak_sizes[-1] != 100:
            leak_sizes.append(100)

        self.category = category
        self.num_components = num_components
        self.leaks = {}

        # Get or convert leak frequency data as [{mu, sigma}]
        if leak_frequency_lists is None:
            leak_frequency_lists = component_data.get_component_leak_parameters(self.category,
                                                                                species=species,
                                                                                saturated_phase=saturated_phase)

        # Create leak representations for each size
        for leak_size in leak_sizes:
            leak_freq_pair = self.interpolate_leak_size_dist(leak_size, leak_frequency_lists)
            leak = Leak(leak_size, leak_freq_pair[0], leak_freq_pair[1])
            self.leaks[leak.size] = leak

    def __repr__(self):
        return str(self)

    def __str__(self):
        return "{} ({}) freqs: {}".format(self.category, self.num_components, self.get_leaks_str_simple())
    
    def interpolate_leak_size_dist(self, leak_size, leak_frequency_lists):
        alpha_1 = 1
        alpha_2 = 1
        #predicted_mean = alpha_1 + alpha_2*np.log(leak_size)
        # TODO: should this refer to default leak sizes or provided leak sizes?
        predicted_mean = np.interp(np.log(leak_size), np.log(self.default_leak_sizes), np.array(leak_frequency_lists)[:, 0])
        predicted_sigma = np.interp(np.log(leak_size), np.log(self.default_leak_sizes), np.array(leak_frequency_lists)[:, 1])
        return predicted_mean, predicted_sigma

    def get_leak(self, leak_size):
        """ Retrieve leak object at specified size for this component type. """
        if self.num_components:
            if leak_size in self.leaks.keys():
                return self.leaks[leak_size]
            else:
                raise KeyError('Leak of specified size not found')
        else:
            return None

    def get_leak_frequency(self, leak_size):
        freq = 0
        leak = self.get_leak(leak_size)
        if leak:
            freq = leak.get_leak_freq_mean() * self.num_components
        return freq

    def get_leaks_str_simple(self):
        """ simplified version of above as '[x, y], [x2, y2]... ' """
        leak_list = sorted(self.leaks.values(), key=lambda x: x.size)
        full_str = ''
        for i, leak in enumerate(leak_list):
            full_str += '[{}, {}], '.format(leak.mu, leak.sigma)
        full_str = full_str[:-2]  # remove final ", " and end of list
        return full_str
