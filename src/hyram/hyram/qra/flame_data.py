"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from copy import deepcopy

import numpy as np


class FlameData(object):
    """ Convenience object for holding state of flame calc. For pickling as needed. """

    def __init__(self, amb_temp, amb_pressure,
                 rel_species, rel_temp, rel_pressure, rel_phase,
                 orifice_leak_diams, leak_height, release_angle,
                 notional_nozzle_model, loc_distributions, excl_radius, rand_seed, rel_humid, rad_source_model,
                 facil_length, facil_width, qrads=None, plot_files=None, positions=None):
        self.amb_temp = np.around(amb_temp, 3)
        self.amb_pressure = np.around(amb_pressure, 3)
        self.rel_species = rel_species
        self.rel_temp = np.around(rel_temp, 3)
        self.rel_pressure = np.around(rel_pressure, 3)
        self.rel_phase = rel_phase
        self.orifice_leak_diams = list(np.around(orifice_leak_diams, 5))
        self.leak_height = np.around(leak_height, 3)
        self.release_angle = np.around(release_angle, 3)
        self.notional_nozzle_model = notional_nozzle_model
        self.loc_distributions = loc_distributions
        self.excl_radius = np.around(excl_radius, 4)
        self.rand_seed = rand_seed
        self.rel_humid = np.around(rel_humid, 4)
        self.rad_source_model = rad_source_model
        self.facil_length = np.around(facil_length, 3)
        self.facil_width = np.around(facil_width, 3)
        self.qrads = qrads
        self.plot_files = plot_files
        self.positions = positions  # 2d array of x,y,z

    def __eq__(self, other_inst):
        """ Verify that another FlameData instance has identical attributes to this one """
        this_attrs = deepcopy(vars(self))
        other_attrs = deepcopy(vars(other_inst))
        # Don't compare values of values of generated data
        this_attrs.pop('qrads', None)
        other_attrs.pop('qrads', None)
        this_attrs.pop('plot_files', None)
        other_attrs.pop('plot_files', None)
        this_attrs.pop('positions', None)
        other_attrs.pop('positions', None)
        return this_attrs == other_attrs


