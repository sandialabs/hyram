"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np

from . import probits


def calc_thermal_fatality_probabilities(qrads, probit_thermal_id, exposure_time,
                                        num_leak_sizes, total_occupants):
    """
    Calculates thermal fatality probabilities for all positions and leak sizes

    Parameters
    ----------
    qrads : array
        Heat flux data (W/m2) for all positions and leaksizes
        1-D array, all leaksizes for location 1,
        then all leaksizes location 2, etc.

    probit_thermal_id : str
        String to identify thermal probit to use
        See probits.py for details

    exposure_time : float
        Exposure time (seconds) for use in thermal probit

    num_leak_sizes : int
        Number of leak sizes

    total_occupants : int
        Number of occupants under consideration

    Returns
    -------
    thermal_fatality_probs_per_leak : array
        Probability of fatality from thermal effects per leak size
        (considering all occupants for each leak size)
    """
    thermal_fatality_probs = []
    for qrad in qrads:
        p_therm_fatal = probits.compute_thermal_fatality_prob(probit_thermal_id,
                                                              qrad,
                                                              exposure_time)
        thermal_fatality_probs.append(p_therm_fatal)

    # Convert from 1d to 2d where rows are qrads for single leak size
    thermal_fatality_probs_matrix = np.reshape(thermal_fatality_probs,
                                               (num_leak_sizes, total_occupants))

    # Sum over positions so 1 val per leak size
    thermal_fatality_probs_per_leak = np.sum(thermal_fatality_probs_matrix,
                                             axis=1)

    return thermal_fatality_probs_per_leak


def calc_overpressure_fatality_probabilities(overpressures,
                                             impulses,
                                             overpressure_probit_id,
                                             num_leak_sizes, total_occupants):
    """
    Calculates overpressure fatality probabilities
    for all positions and leak sizes

    Parameters
    ----------
    overpressures : array
        Peak overpressure values (Pa) for all positions and leaksizes
        1-D array, all leaksizes for location 1,
        then all leaksizes location 2, etc.

    impulses : array
        Impulse values (Pa*s) for all positions and leaksizes
        1-D array, all leaksizes for location 1,
        then all leaksizes location 2, etc.

    overpressure_probit_id : str
        String to identify thermal probit to use
        See probits.py for details

    num_leak_sizes : int
        Number of leak sizes

    total_occupants : int
        Number of occupants under consideration

    Returns
    -------
    overp_fatality_probs_per_leak : array
        Probability of fatality from overpressure effects per leak size
        (considering all occupants for each leak size)
    """
    overp_fatality_probs = []
    for overpressure, impulse in zip(overpressures, impulses):
        p_overp_fatal = probits.compute_overpressure_fatality_prob(overpressure_probit_id,
                                                              overpressure,
                                                              impulse)
        overp_fatality_probs.append(p_overp_fatal)

    # Convert from 1d to 2d where rows are qrads for single leak size
    overp_fatality_probs_matrix = np.reshape(overp_fatality_probs,
                                             (num_leak_sizes, total_occupants))

    # Sum over positions so 1 val per leak size
    overp_fatality_probs_per_leak = np.sum(overp_fatality_probs_matrix,
                                           axis=1)

    return overp_fatality_probs_per_leak
