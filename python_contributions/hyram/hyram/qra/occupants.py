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

import numpy as np

from .utilities import misc_utils as misc


def create_occupant_groups_from_list(occupants_list):
    """
    Create list of OccupantGroup objects representing groups from dicts.

    Parameters
    ----------
    occupants_list : list
        list of dicts defining each occupant group

    Returns
    -------
    occ_groups : list
        list of OccupantGroup objects

    """
    occ_groups = []
    for occ_group_dict in occupants_list:
        occ_group = create_occupant_group_from_dict(occ_group_dict)
        occ_groups.append(occ_group)

    return occ_groups


def create_occupant_group_from_dict(input_dict):
    """ Convenience function for creating set of occupants with parameters passed via dict.
    Dict must have keys matching parse function in analysis.py
    """
    num_occupants = misc.get_or_error(input_dict, 'count', 'Enter number of occupants in group')
    descrip = input_dict.get('descrip', 'Occupant group')
    x_distr = misc.get_or_error(input_dict, 'xdistr', 'Select distribution type for X coordinates')
    y_distr = misc.get_or_error(input_dict, 'ydistr', 'Select distribution type for Y coordinates')
    z_distr = misc.get_or_error(input_dict, 'zdistr', 'Select distribution type for Z coordinates')
    xa = misc.get_or_error(input_dict, 'xa', 'Enter parameter A for X positions')
    xb = misc.get_or_error(input_dict, 'xb', 'Enter parameter B for X positions')
    ya = misc.get_or_error(input_dict, 'ya', 'Enter parameter A for Y positions')
    yb = misc.get_or_error(input_dict, 'yb', 'Enter parameter B for Y positions')
    za = misc.get_or_error(input_dict, 'za', 'Enter parameter A for Z positions')
    zb = misc.get_or_error(input_dict, 'zb', 'Enter parameter B for Z positions')
    hours = misc.get_or_error(input_dict, 'hours', 'Specify number of exposed hours')

    occ_group = OccupantGroup(num_occupants, descrip,
                              x_distr, xa, xb,
                              y_distr, ya, yb,
                              z_distr, za, zb,
                              exposed_hours=hours)
    return occ_group


class OccupantGroup(object):
    """
    Group of occupants using system/in facility.
    Each occupant will be represented by instance of Occupant object.

    Parameters
    ----------
    description : str
        Label for the group
    num_occupants : int
        Number of occupants in this group only
    x_distr_type : str
        One of uniform, normal, deterministic
    xa : inf, float or array
        if distribution is deterministic, is array or positions.
    xb :
        if distr is deterministic, is unused.
    y_distr_type : str
        One of uniform, normal, deterministic
    ya :
        if distribution is deterministic, is array or positions.
    yb :
        if distr is deterministic, is unused.
    exposed_hours :

    """

    def __init__(self, num_occupants, description,
                 x_distr_type='norm', xa=-np.inf, xb=np.inf,
                 y_distr_type='norm', ya=-np.inf, yb=np.inf,
                 z_distr_type='norm', za=-np.inf, zb=np.inf,
                 exposed_hours=8760):

        self.description = description

        # Validate and set state
        if num_occupants > 0:
            self.num_occupants = self.count = int(num_occupants)
        else:
            raise ValueError('Must have at least one occupant')

        if exposed_hours > 0:
            self.hours = self.exposed_hours = int(exposed_hours)
        else:
            raise ValueError('Exposed hours must be positive integer')

        self.x_distr_type = x_distr_type
        self.xa = xa
        self.xb = xb

        self.y_distr_type = y_distr_type
        self.ya = ya
        self.yb = yb

        self.z_distr_type = z_distr_type
        self.za = za
        self.zb = zb

    def generate_physics_format(self):
        """
        Export this group in format for hyramphys position generator:
        loc_distributions : list
            List of location distributions.  Each location distribution should be a list like
                [n, (xdist_type, xparam_a, xparam_b),
                (ydist_type, yparam_a, yparam_b),
                (zdist_type, zparam_a, zparam_b)]
            where *dist_type is one of 'dete', 'unif', or 'normal' and param_a and param_b depend on the distr type:
                For deterministic, param_a = value, param_b = None.
                For uniform, param_a = minval, param_b = maxval.
                For normal, param_a = mu, param_b = sigma.

        """
        return [
            self.num_occupants,
            (self.x_distr_type, self.xa, self.xb),
            (self.y_distr_type, self.ya, self.yb),
            (self.z_distr_type, self.za, self.zb),
        ]
