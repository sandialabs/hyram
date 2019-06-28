

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

# Convert between meters and feet
M_TO_FT = 3.28084
FT_TO_M = 1. / M_TO_FT
IN_TO_M = 0.0254  # inches to meters
YEARS_TO_HOURS = 8760.

# Release size as percentage of total orifice diameter
LEAK_SIZES = [0.01, 0.100, 1.00, 10.00, 100.00]


# MODEL REFERENCE IDs
# All are alphanumeric, lower-case. Aim for a few identifying characters.
# Note: nozzle models set in hyramphys

# Probit references are matched to their functions in the probit file (end of file)
# Probit thermal model ids. Referenced in probit file
PROBIT_THERMAL_IDS = ['eis', 'tsa', 'tno', 'lee']

# Probit overpressure model ids. Referenced in probit file
PROBIT_OVERP_IDS = ['elh', 'lhs', 'hea', 'col', 'deb']
