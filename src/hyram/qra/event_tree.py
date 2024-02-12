"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


def check_probability_value(probability):
    """
    Checks for valid probability value between 0.0 and 1.0
    """
    if probability < 0:
        raise ValueError(f"Probability value is <0 (value: {probability})")
    if probability > 1:
        raise ValueError(f"Probability value is >1 (value: {probability})")


def calc_probability_not_occur(probability):
    """
    Calculates complementary probability of a given event probability
    """
    probability_not_occur = 1 - probability
    return probability_not_occur


def calc_end_state_probabilities(event_probabilities):
    """
    Calculates the end-state probabilities for an event tree
    given a list of probabilities of "success" of the intermediate events.
    This formulation assumes a "cascading" event tree
    in which the "success" of an intermediate event will lead to an end-state,
    and that "failure" of an intermediate event will lead to the next intermediate event.
    The "failure" of the final intermediate event will lead to the final end-state.
    """
    end_state_probabilities = []
    previous_event_probability = 1.0
    for event_probability in event_probabilities:
        check_probability_value(event_probability)
        end_state_probability = event_probability * previous_event_probability
        end_state_probabilities.append(end_state_probability)
        previous_event_probability *= calc_probability_not_occur(event_probability)
    final_end_state_probability = previous_event_probability
    end_state_probabilities.append(final_end_state_probability)
    return end_state_probabilities


def calc_end_state_frequencies(initiating_event_frequency, end_state_probabilities):
    """
    Calculates the frequency of event tree end-states,
    given the frequency of the initiating event and the end-state probabilities
    """
    end_state_frequencies = []
    for end_state_probability in end_state_probabilities:
        end_state_frequency = initiating_event_frequency * end_state_probability
        end_state_frequencies.append(end_state_frequency)
    return end_state_frequencies
