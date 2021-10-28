"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

LIQUID_RELEASE_PRESSURE_INVALID_MSG = "Invalid inputs - release pressure may be invalid."


class Error(Exception):
    """ Base class for HyRAM exceptions. """
    pass


class InputError(Error):
    """
    Exception raised when errors are present due to input parameters provided by user.

    Attributes
    ----------
    function : str
        Function location at which error detected

    message : str
        Error message describing issue
    """

    def __init__(self, function, message):
        self.function = function
        self.message = message


class FluidSpecificationError(InputError):
    """
    Error when fluid parameters not specified correctly, i.e. too few or too many.

    Attributes
    ----------
    function : str
        Function location at which error detected

    message : str
        Error message describing issue
    """

    def __init__(self, function='', message=''):
        self.function = function
        if not message:
            message = 'Fluid must be defined by exactly two of the following parameters: temperature, pressure, density, phase'
        self.message = message
