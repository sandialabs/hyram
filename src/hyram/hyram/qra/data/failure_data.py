"""
Known data for component failures
"""
#  Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
#  Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
#  .
#  This file is part of HyRAM (Hydrogen Risk Assessment Models).
#  .
#  HyRAM is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#  .
#  HyRAM is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#  .
#  You should have received a copy of the GNU General Public License
#  along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

import logging
from .. import distributions


class ComponentFailure(object):
    """

    Parameters
    ----------
    component : str
        Name of component

    mode : str
        Description of failure mode

    distr_type : str
        Name referencing distribution type. lognormal, beta, ev, normal, uniform

    a : float
        First parameter describing distribution. Depends on type.
        e.g. if lognormal, assume this is sigma

    b : float
        Second parameter describing distribution. Depends on type.
        e.g. if lognorm, assume this is mu
    """
    def __init__(self, component, mode, distr_type, a, b=None):
        valid, error_msg = self.validate_parameters(component, mode, distr_type, a, b)
        if not valid:
            raise ValueError(error_msg)

        self.component = component
        self.mode = self.failure_mode = mode
        self.distr_type = distr_type
        self.a = a
        self.b = b

        distr_class = distributions.get_distribution_class(self.distr_type)
        self.distr = distr_class(a, b)
        self.mean = self.p = self.distr.mean

        log = logging.getLogger('hyram.qra')
        log.info("Component failure: {} {} | {}, mean {:.3g}".format(
            self.component, self.mode, self.distr, self.mean))

    def validate_parameters(self, component, mode, distr_type, a, b):
        valid = True
        msg = ""
        if not distributions.has_distribution(distr_type):
            valid = False
            msg = "Component failure distribution key {} not recognized".format(distr_type)
        if a is None and b is None:
            valid = False
            msg = "Distribution parameters cannot both be None"

        return valid, msg
