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
#
#  This file is part of HyRAM (Hydrogen Risk Assessment Models).
#
#  HyRAM is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#
#  HyRAM is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#
#  You should have received a copy of the GNU General Public License
#  along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

from copy import deepcopy

import numpy as np


class FlameData(object):
    """ Convenience object for holding state of flame calc. For pickling as needed. """

    def __init__(self, amb_temp, amb_pressure, h2_temp, h2_pressure, h2_phase, orifice_leak_diams, leak_height, release_angle,
                 notional_nozzle_model, loc_distributions, excl_radius, rand_seed, rel_humid, rad_source_model,
                 facil_length, facil_width, qrads=None, plot_files=None):
        self.amb_temp = np.around(amb_temp, 3)
        self.amb_pressure = np.around(amb_pressure, 3)
        self.h2_temp = np.around(h2_temp, 3)
        self.h2_pressure = np.around(h2_pressure, 3)
        self.h2_phase = h2_phase
        self.orifice_leak_diams = list(np.around(orifice_leak_diams, 5))
        self.leak_height = np.around(leak_height, 3)
        self.release_angle = np.around(release_angle, 3)
        self.notional_nozzle_model = notional_nozzle_model
        self.loc_distributions = loc_distributions
        self.excl_radius = np.around(excl_radius, 3)
        self.rand_seed = rand_seed
        self.rel_humid = np.around(rel_humid, 3)
        self.rad_source_model = rad_source_model
        self.facil_length = np.around(facil_length, 3)
        self.facil_width = np.around(facil_width, 3)
        self.qrads = qrads
        self.plot_files = plot_files

    def __eq__(self, other_inst):
        """ Verify that another FlameData instance has identical attributes to this one """
        this_attrs = deepcopy(vars(self))
        other_attrs = deepcopy(vars(other_inst))
        # Don't compare values of values of generated data
        this_attrs.pop('qrads', None)
        other_attrs.pop('qrads', None)
        this_attrs.pop('plot_files', None)
        other_attrs.pop('plot_files', None)
        return this_attrs == other_attrs


