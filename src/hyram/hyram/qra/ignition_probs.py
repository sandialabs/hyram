"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


def get_ignition_probability(mass_flow_rate, mass_flow_thresholds, immed_ign_probs, delayed_ign_probs):
    """
    Get the ignition probabilities based on mass flow rate
    from a list immediate and delayed ignition probabilities
    that are separated into mass flow rate thresholds

    For the lists of ignition probabilities,
    the first value will be used if the input mass_flow_rate
    is less than the first (smallest) value of the mass_flow_thresholds,
    the last value will be used if the input mass_flow_rate
    is greater than the last (largest) value of the mass_flow_thresholds,
    and intermediate values will be used if the input mass_flow_rate
    falls within the values of the mass_flow_thresholds

    Parameters
    ----------
    mass_flow_rate : float
        Mass flow rate [kg/s] of interest

    mass_flow_thresholds : list of floats
        Sorted list of mass flow rates [kg/s] that separate the list of ignition probabilities

    immed_ign_probs : list of floats
        List of immediate ignition probabilities based on the mass flow rate thresholds 

    delayed_ign_probs : list of floats
        List of delayed ignition probabilities based on the mass flow rate thresholds

    Returns
    -------
    immediate_ignition_probability : float
        Probability of immediate ignition
    
    delayed_ignition_probability : float
        Probability of delayed ignition
    """
    if mass_flow_thresholds != sorted(mass_flow_thresholds):
        error_str = ('List of ignition thresholds ({})'.format(mass_flow_thresholds)
                     + ' must be ascending values for ignition probabilities.')
        raise ValueError(error_str)
    
    for prob_list in [immed_ign_probs, delayed_ign_probs]:
        if len(mass_flow_thresholds) != (len(prob_list) - 1):
            error_str = (
                'Number of ignition thresholds ({})'.format(len(mass_flow_thresholds))
                + ' must be exactly one value less than the'
                + ' number of ignition probabilities ({}).'.format(len(prob_list))
            )
            raise ValueError(error_str)

    if mass_flow_rate >= mass_flow_thresholds[-1]:
        threshold_index = len(mass_flow_thresholds)
    else:
        threshold_index = next(idx
                               for idx, max_threshold
                               in enumerate(mass_flow_thresholds)
                               if max_threshold > mass_flow_rate)
    immediate_ignition_probability = immed_ign_probs[threshold_index]
    delayed_ignition_probability = delayed_ign_probs[threshold_index]
    return immediate_ignition_probability, delayed_ignition_probability
