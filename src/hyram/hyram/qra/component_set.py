"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np

from .data import component_data
from .leaks import Leak


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
    
    default_leak_sizes = [0.01, 0.10, 1, 10, 100]
    
    def __init__(self, category, num_components, species, saturated_phase=None,
                 leak_frequency_lists=None, leak_sizes=None):
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
