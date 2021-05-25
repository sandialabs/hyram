"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

import pandas as pd
from .leaks import Leak
from .data import component_data


class ComponentSet:
    """
    Representation of 1+ components of a specific type or category.
    Has associated LeakSet which defines all leaks for this component type.

    Parameters
    ----------
    category : str
        Name of component type, e.g. cylinder

    num_components : int
        Number of components in this category/type

    leak_frequency_lists: list of lists
        Leak frequency parameters for each leak size of format [[mu, sigma]]

    leak_sizes : list
        List of floats representing % leakage

    Attributes
    -----------
    category : str
        name of component type, e.g. 'compressor'

    num_components : int
        number of components in analysis

    leaks : dict of {size: LeakSet}

    """

    def __init__(self, category, num_components, species='H2', leak_frequency_lists=None, leak_sizes=None):
        if type(category) != str:
            raise ValueError("Category must be string")
        if num_components < 0:
            raise ValueError("# of components must be 0 or greater")

        if leak_sizes is None:
            leak_sizes = [0.01, 0.10, 1.00, 10.00, 100.00]

        self.category = category
        self.num_components = num_components
        self.leaks = {}

        # Get or convert leak frequency data as [{mu, sigma}]
        if leak_frequency_lists is None:
            leak_frequency_dicts = component_data.get_component_leak_frequency_parameters(self.category, species)
        else:
            leak_frequency_dicts = [{'mu': x[0], 'sigma': x[1]} for x in leak_frequency_lists]

        # Create leak representations for each size
        for leak_size, leak_freq_dict in zip(leak_sizes, leak_frequency_dicts):
            leak = Leak(leak_size, leak_freq_dict['mu'], leak_freq_dict['sigma'])
            self.leaks[leak.size] = leak

    def __str__(self):
        # return '{} set containing {} components'.format(self.category, self.num_components)
        return '{} set of {} with leak data:\n{}\n'.format(self.category, self.num_components, self.get_leaks_as_str())

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
            freq = leak.mean * self.num_components
        return freq

    def get_leaks_as_str(self):
        df = pd.DataFrame(columns=['size (%)', 'probability'])
        leak_list = sorted(self.leaks.values(), key=lambda x: x.size)
        for i, leak in enumerate(leak_list):
            df.loc[i] = [leak.size, leak.prob]
        return df.to_string(index=False)
