"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging

from . import leaks

log = logging.getLogger(__name__)


def compute_leak_frequencies(leak_sizes,
                             release_freq_overrides,
                             component_sets):
    '''
    compute_leak_frequencies calculates leak frequency for each leak size

    Inputs:
        leak_sizes, list(floats) or None
            leak sizes (% area of pipe)
        release_freq_overrides, list(floats) or None
            release frequencies if specified to override those calculated by HyRAM
        component_sets, list(component_set)
            list of component set objects

    Outputs:
        leak_results, list(LeakSizeResult)
            list of leak size result objects
        leak_result100, LeakSizeResult
            leak size result object for 100% area leak
    '''
    leak_sizes = set_leak_size_defaults(leak_sizes)
    release_freq_overrides = set_release_freq_override_defaults(release_freq_overrides, leak_sizes)

    # For each leak size, sum release frequencies for all components at that size
    leak_results = [leaks.LeakSizeResult(leak_size) for leak_size in leak_sizes]
    leak_result100 = leak_results[-1]

    # Compute leak frequencies for each leak size
    for leak_result, release_freq_override in zip(leak_results, release_freq_overrides):
        set_component_leak_freq(leak_result, release_freq_override, component_sets)
        
    return leak_results, leak_result100


def set_leak_size_defaults(leak_sizes):
    '''
    If leak_sizes are not specified, assumed 0.01, 0.10, 1, 10, 100
    otherwise ensure 100 included in specified list
    '''
    if leak_sizes is None:
        leak_sizes = [0.01, 0.10, 1, 10, 100]
    
    leak_sizes.sort()
    if leak_sizes[-1] != 100:
        leak_sizes.append(100)

    return leak_sizes


def set_release_freq_override_defaults(release_freq_overrides, leak_sizes):
    '''
    If release_freq_overrides are not specified, set as list of -1
    '''
    if release_freq_overrides is None:
        return [-1]*len(leak_sizes)
    else:
        return release_freq_overrides


def set_component_leak_freq(leak_result, release_freq_override, component_sets):
    '''
    Determine if leak frequency is calculated or specified
    '''
    if release_freq_override != -1:
        override_release_freq(leak_result, release_freq_override, component_sets)
    else:
        calc_component_release_based_leak_freq(leak_result, component_sets)

    log.info("Total release freq for size {}: {:.3g}\n".format(leak_result.leak_size, leak_result.total_release_freq))


def override_release_freq(leak_result, release_freq_override, component_sets):
    '''
    Assign leak frequency as specified value
    Use override value if provided (i.e. other than -1) and zero out component leak frequencies
    '''
    leak_result.release_freq_override = release_freq_override
    leak_result.total_release_freq = release_freq_override
    for comp_set in component_sets:
        leak_result.component_leak_freqs[comp_set.category] = 0


def calc_component_release_based_leak_freq(leak_result, component_sets):
    '''
    Calculate leak frequency based on component releases
    '''
    total_leak_freq = 0
    for comp_set in component_sets:
        component_leak_frequency = comp_set.get_leak_frequency(leak_result.leak_size)
        leak_result.component_leak_freqs[comp_set.category] = component_leak_frequency
        total_leak_freq += component_leak_frequency
        log.info("Leak {} for {}: {:.3g}".format(leak_result.leak_size, comp_set.category, component_leak_frequency))

    leak_result.total_release_freq = total_leak_freq