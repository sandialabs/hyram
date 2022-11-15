"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import numpy as np


class EventNode:
    """
    Event_Node class to create an event node to populate an event tree

    Parameters
    ----------
    probability : array
        probability that the event occurs
    name : str
        name of event node
    key : {'shut', 'noig', 'jetf', 'expl'}
        Short unambiguous identifier for event comparison
    preceding_event_probability_not_occur : bool
        if not initial node in event tree, probability of preceding/parent node not occurring
    consequence_type : str
        type of consequence of event end state
    num_leak_sizes : int
        number of leak sizes considered in overarching risk assessment

    """
    def __init__(self,
                 probability,
                 name,
                 key,
                 preceding_event_probability_not_occur,
                 consequence_type,
                 num_leak_sizes):
        self.name = name
        self.key = key
        self.consequence_type = consequence_type
        self.preceding_event_probability_not_occur = preceding_event_probability_not_occur
        self.event_probability = self.ensure_probability_is_array(probability, num_leak_sizes)
        self.calc_event_split()
        self.end_state_probability = self.probability_occur

        # TODO: what are reasonable initial values for these?
        # self.probability_occur = 0
        # self.probability_not_occur = 1

    def calc_event_split(self):
        self.probability_occur = self.event_probability * self.preceding_event_probability_not_occur
        self.probability_not_occur = self.not_probability(self.event_probability) * self.preceding_event_probability_not_occur

    @staticmethod
    def ensure_probability_is_array(probability, num_leak_sizes):
        if isinstance(probability, float) or isinstance(probability, int):
            return np.array([probability]*num_leak_sizes)
        else:
            return probability

    @staticmethod
    def not_probability(event_probability):
        return 1 - event_probability


def build_event_tree(events_in_tree, num_leak_sizes):
    '''
    Build an event tree out of event_node objects from a diction of event definitions

    Inputs:
        events_in_tree - dict
            dictionary of event definitions
        num_leak_sizes - int
            number of leak sizes considered in overarching risk assessment
    
    Outputs:
        event_tree - list
            list of event objects encompassing event tree
    '''
    event_tree = []
    for i, event_definition in enumerate(events_in_tree):
        if i == 0:
            preceding_event_probability_not_occur = 1
        else:
            preceding_event_probability_not_occur = event_tree[-1].probability_not_occur
        event_tree.append(EventNode(probability=event_definition['event_prob'],
                                    name=event_definition['name'],
                                    key=event_definition['key'],
                                    preceding_event_probability_not_occur=preceding_event_probability_not_occur,
                                    consequence_type=event_definition['consequence_type'],
                                    num_leak_sizes=num_leak_sizes))
    return event_tree
