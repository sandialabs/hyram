"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""


class Event:
    """Generic event class with basic listener and emitter functionality. """
    def __init__(self):
        self.listeners = []

    def __iadd__(self, listener):
        self.listeners.append(listener)
        return self

    def __isub__(self, other):
        if other in self.listeners:
            self.listeners.remove(other)
        return self

    def notify(self, *args, **kwargs):
        for listener in self.listeners:
            listener(*args, **kwargs)


