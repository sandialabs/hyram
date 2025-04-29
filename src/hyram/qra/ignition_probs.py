"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from .event_tree import check_probability_value


def get_ignition_probability(mass_flow_rate, ign_probs):
    """
    Get the ignition probabilities based on mass flow rate
    from immediate and delayed ignition probabilities
    that are separated into mass flow rate thresholds

    For the ignition probabilities,
    the first value will be used if the input mass flow rate
    is less than the first (smallest) value of the mass flow thresholds,
    the last value will be used if the input mass flow rate
    is greater than the last (largest) value of the mass flow thresholds,
    and intermediate values will be used if the input mass flow rate
    falls within the values of the mass_flow_thresholds

    Parameters
    ----------
    mass_flow_rate : float
        Mass flow rate [kg/s] of interest

    ign_probs : dict
        Dictionary of ignition probaiblities of the form:

        ign_probs = {
            'flow_thresholds': flow_thresholds,
            'immed_ign_probs': immed_ign_probs,
            'delayed_ign_probs': delayed_ign_probs
        }

        The values of the ditionary are:
        flow_thresholds : list of floats
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
    mass_flow_thresholds = ign_probs['flow_thresholds']
    immed_ign_probs = ign_probs['immed_ign_probs']
    delayed_ign_probs = ign_probs['delayed_ign_probs']

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


def calc_total_ign_prob(immed_ign_prob, delayed_ign_prob):
    """
    Immediate and delayed ignition probabilities
    are currently assumed to be combined into a
    total ignition probability.

    Inputs
    ------
    immed_ign_prob, float
        Immediate ignition probability
    delayed_ign_prob, float
        Delayed ignition probability

    Returns
    -------
    total_ign_prob, float
        Total ignition probability
    """
    check_probability_value(immed_ign_prob)
    check_probability_value(delayed_ign_prob)

    total_ign_prob = immed_ign_prob + delayed_ign_prob
    return total_ign_prob


def calc_cond_immed_ign_prob(abs_immed_ign_prob, total_ign_prob):
    """
    Immediate ignition probability input is assumed to be an
    absolute probability; that is, the probability of
    immediate ignition given that a leak has occured.
    However, the event tree framework needs a conditional
    probability; that is, the probability of immediate
    ignition given that ignition has occured.

    Inputs
    ------
    abs_immed_ign_prob, float
        Absolute immediate ignition probability,
        given that a leak has occured.
    total_ign_prob, float
        Total ignition probability, given that a leak has occured.

    Returns
    -------
    conditional_immed_ign_prob, float
        Conditional immediate ignition probability,
        based on the probability that ignition has occured.
    """
    check_probability_value(abs_immed_ign_prob)
    check_probability_value(total_ign_prob)

    if total_ign_prob == 0:
        conditional_immed_ign_prob = 0
    else:
        conditional_immed_ign_prob = abs_immed_ign_prob / total_ign_prob
    return conditional_immed_ign_prob
