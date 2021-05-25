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


