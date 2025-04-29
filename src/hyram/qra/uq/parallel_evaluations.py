"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import multiprocessing as mp
import functools
from typing import Callable, Union, Optional

"""Script to holder parallelization functions"""


def parallel_function_evaluations(input_function:Callable,
                                  input_dictionary_list:list,
                                  num_cpus:Optional[Union[int, None]]=None,
                                  additional_inputs:Optional[Union[dict, None]]=None
                                  ) -> list:
    """Function to evaluate a function with a dictionary input in a parallel fashion
        Parameters
    ----------
    input_function: Callable
    input_dictionary_list: list
    num_cpus: int
        Defaults to None
    additional_input: dict
        Additional arguments to pass to function
    """
    if num_cpus == None:
        num_cpus = mp.cpu_count()

    if additional_inputs == None:
        additional_inputs = {}

    pool_size = min(num_cpus, len(input_dictionary_list))
    f_partial = functools.partial(input_function, **additional_inputs)
    pool = mp.Pool(pool_size)
    with mp.Pool(pool_size) as pool:
        results = pool.map(f_partial, input_dictionary_list)
    return results
