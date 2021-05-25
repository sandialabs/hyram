"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

import numpy as np


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
    num_occupants = input_dict['count']
    descrip = input_dict['descrip']
    x_distr = input_dict['xdistr']
    y_distr = input_dict['ydistr']
    z_distr = input_dict['zdistr']
    xa = input_dict['xa']
    xb = input_dict['xb']
    ya = input_dict['ya']
    yb = input_dict['yb']
    za = input_dict['za']
    zb = input_dict['zb']
    hours = input_dict['hours']

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
        self.num_occupants = self.count = int(num_occupants)

        if exposed_hours >= 0:
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
