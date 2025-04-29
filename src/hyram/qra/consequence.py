"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np

from . import probits


def thermal_consequence(physical_responses,
                        consequence_modeling_decisions):
    """
    Calculates thermal fatality probabilities for all positions and leak sizes

    Parameters
    ----------
    physical_responses : dict
        Dictionary holding all physical responses associated with event

        qrads : array (item in physical_responses dict)
            Heat flux (W/m2) for all positions and leak sizes
            1-D array, all leak sizes for location 1,
            then all leak sizes location 2, etc.

    consequence_modeling_decisions : dict
        Dictionary holding modeling decisions/specifications used to enacted the desired consequence model

        probit_thermal_id : str (item in consequence_modeling_decisions dict)
            String to identify thermal probit to use
            See probits.py for details

        exposure_time : float (item in consequence_modeling_decisions dict)
            Exposure time (seconds) for use in thermal probit

    Returns
    -------
    thermal_fatality_probs : array
        Probability of fatality from thermal effects
    """
    thermal_fatality_probs = []
    for qrad in physical_responses['qrads']:
        p_therm_fatal = probits.compute_thermal_fatality_prob(consequence_modeling_decisions['probit_thermal_id'],
                                                              qrad,
                                                              consequence_modeling_decisions['exposure_time'])
        thermal_fatality_probs.append(p_therm_fatal)

    return thermal_fatality_probs

def overpressure_consequence(physical_responses,
                             consequence_modeling_decisions):
    """
    Calculates overpressure fatality probabilities
    for all positions and leak sizes

    Parameters
    ----------
    physical_responses : dict
        Dictionary holding all physical responses associated with event

        overpressures : array (item in physical_responses dict)
            Peak overpressure values (Pa) for all positions and leak sizes
            1-D array, all leak sizes for location 1,
            then all leak sizes location 2, etc.

        impulses : array (item in physical_responses dict)
            Impulse values (Pa*s) for all positions and leak sizes
            1-D array, all leak sizes for location 1,
            then all leak sizes location 2, etc.

    consequence_modeling_decisions : dict
        Dictionary holding modeling decisions/specifications used to enacted the desired consequence model

        overpressure_probit_id : str (item in consequence_modeling_decisions dict)
            String to identify thermal probit to use
            See probits.py for details

    Returns
    -------
    overp_fatality_probs : array
        Probability of fatality from overpressure effects
    """
    overp_fatality_probs = []
    for overpressure, impulse in zip(physical_responses['overpressures'], physical_responses['impulses']):
        p_overp_fatal = probits.compute_overpressure_fatality_prob(consequence_modeling_decisions['probit_overp_id'],
                                                                   overpressure,
                                                                   impulse)
        overp_fatality_probs.append(p_overp_fatal)

    return overp_fatality_probs

def convert_consequence_to_per_leak_basis(consequences,
                                          num_leak_sizes,
                                          total_occupants):
    """
    Convert consequences to a leak basis, summing over all positions

    Parameters
    ----------
    num_leak_sizes : int
        Number of leak sizes

    total_occupants : int
        Number of occupants under consideration

    Returns
    -------
    consequence_probs_per_leak : array
        Probability of consequence per leak size
        (considering all occupants for each leak size)
    """
    # Convert from 1d to 2d where rows are consequences for single leak size
    consequence_probs_matrix = np.reshape(consequences, (num_leak_sizes, total_occupants))
    # Sum over positions so 1 val per leak size
    return np.sum(consequence_probs_matrix, axis=1)

def calculate_event_consequence(consequence_type,
                                num_leak_sizes,
                                total_occupants,
                                physical_responses,
                                consequence_modeling_decisions,
                                verbose=False):
    """
    Calculate consequence for different consequence types for all leak sizes and locations

    Parameters
    ----------
    consequence_type : str
        specifies what type of consequence model to use ('thermal', 'overp', or None)

    num_leak_sizes : int
        Number of leak sizes

    total_occupants : int
        Number of occupants under consideration

    physical_responses : dict
        Dictionary holding all physical responses associated with event

    consequence_modeling_decisions : dict
        Dictionary holding modeling decisions/specifications used to enacted the desired consequence model

    verbose : bool, optional
        Toogle extra output to command line

    Returns
    -------
    consequence_probs_per_leak : array
        Probability of specified consequence type per leak size
        (considering all occupants for each leak size)
    """
    if consequence_type == None or total_occupants == 0:
        return np.array([0.0]*num_leak_sizes)
    elif consequence_type == 'thermal':
        consequence_model =  thermal_consequence
    elif consequence_type == 'overp':
        consequence_model = overpressure_consequence
    else:
        consequence_type_error_msg = ('Non valid consequence_type passed.'
                                    + ' Valid consequence_type inputs are'
                                    + ' "None", "thermal", and "overp".')
        raise ValueError(consequence_type_error_msg)

    if verbose:
        print(f"Calculating {consequence_type} fatality probabilities...")
    consequence_probs = consequence_model(physical_responses,
                                          consequence_modeling_decisions)
    consequence_probs_per_leak = convert_consequence_to_per_leak_basis(consequence_probs,
                                                                       num_leak_sizes,
                                                                       total_occupants)
    if verbose:
        print(f"Probit {consequence_type} results:")
        print(consequence_probs_per_leak)
    return consequence_probs_per_leak


def generate_event_results(event_names: dict,
                           event_keys: dict,
                           prob_end_states: list,
                           prob_event_occurrence: list,
                           pll_contrib: list):

    """
    Creates a list of dictionaries for output purposes, with each
    dictionary detailing a particular consequence, its likelihood, and
    its PLL contributions.

    Parameters
    ----------
    event_names : dict
        Names of the potential events
    event_keys : dict
        Dictionary mapping the event names to the key
        for different consequences
    prob_end_states : list
        Probability of each end state in `event_names`.
        Should add up to 1.
    prob_event_occurrence : list
        Overall probability of each consequence in `event_keys`
    pll_contrib : list
        Contributions of each consequence to the overall potential
        loss of life (PLL).
    """

    result_dicts = []
    for i, name in enumerate(event_names):
        rdict = {'label': name,
                    'key': event_keys[name],
                    'prob': prob_end_states[i],
                    'events': prob_event_occurrence[i],
                    'pll': pll_contrib[i]}
        result_dicts.append(rdict)

    total_dict = {'label': 'TOTAL', 'key': 'tot',
                    'prob': sum(prob_end_states),
                    'events': sum(prob_event_occurrence),
                    'pll': sum(pll_contrib)}
    result_dicts.append(total_dict)
    return result_dicts