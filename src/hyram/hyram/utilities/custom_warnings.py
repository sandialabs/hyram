import warnings


class PhysicsWarning(UserWarning):
    """
    Generic warning class to bubble up pertinent warnings to GUI

    Attributes
    ----------
    message : str
        message describing reason for the warning
    """

    def __init__(self, message):
        self.message = message


