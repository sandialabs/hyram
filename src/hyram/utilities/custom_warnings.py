"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


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


