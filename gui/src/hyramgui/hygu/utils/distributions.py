"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""


class MetaChoiceList(type):
    """Metaclass defining enum-like list of choices described by short strings.

    """
    keys: list

    def __contains__(cls, item):
        item = item.lower().strip()
        return item in cls.keys

    def __len__(cls):
        return len(cls.keys)

    def __index__(cls, i: int):
        return cls.keys[i]

    def __getitem__(cls, item):
        return cls.keys[item]


class BaseChoiceList(metaclass=MetaChoiceList):
    """Represents a list of paired keys and labels. """
    keys: list
    labels: list

    @classmethod
    def index(cls, item: str):
        item = item.lower().strip()
        return cls.keys.index(item)


class Distributions(BaseChoiceList):
    """Distribution options for input parameter sampling. """
    keys = ['det', 'tnor', 'tlog', 'uni', 'nor', 'log']
    labels = ['Deterministic', 'Normal (truncated)', 'Lognormal (truncated)', 'Uniform', 'Normal', 'Lognormal']
    det = 'det'
    tnor = 'tnor'
    tlog = 'tlog'
    uni = 'uni'
    nor = 'nor'
    log = 'log'


class Uncertainties(BaseChoiceList):
    """Uncertainty options for input parameter sampling. """
    keys = ['ale', 'epi', None]
    labels = ['Aleatory', 'Epistemic', 'None']
    ale = 'ale'
    epi = 'epi'


