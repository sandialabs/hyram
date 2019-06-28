"""
Known data for component leak frequencies
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

COMPRESSOR_PROB = [
    {'mu': -1.7198, 'sigma': 0.2143},
    {'mu': -3.9185, 'sigma': 0.4841},
    {'mu': -5.1394, 'sigma': 0.7898},
    {'mu': -8.8408, 'sigma': 0.8381},
    {'mu': -11.3365, 'sigma': 1.3689},
]

CYLINDER_PROB = [
    {'mu': -13.8364, 'sigma': 0.6156},
    {'mu': -14.001, 'sigma': 0.6065},
    {'mu': -14.3953, 'sigma': 0.6232},
    {'mu': -14.9562, 'sigma': 0.6290},
    {'mu': -15.6047, 'sigma': 0.6697},
]

FILTER_PROB = [
    {'mu': -5.2471, 'sigma': 1.9849},
    {'mu': -5.2884, 'sigma': 1.5180},
    {'mu': -5.3389, 'sigma': 1.4806},
    {'mu': -5.3758, 'sigma': 0.8886},
    {'mu': -5.4257, 'sigma': 0.9544},
]

FLANGE_PROB = [
    {'mu': -3.9236, 'sigma': 1.6611},
    {'mu': -6.1211, 'sigma': 1.2533},
    {'mu': -8.3307, 'sigma': 2.2024},
    {'mu': -10.5399, 'sigma': 0.8332},
    {'mu': -12.7453, 'sigma': 1.8274},
]

HOSE_PROB = [
    {'mu': -6.8061, 'sigma': 0.2682},
    {'mu': -8.6394, 'sigma': 0.5520},
    {'mu': -8.7740, 'sigma': 0.5442},
    {'mu': -8.8926, 'sigma': 0.5477},
    {'mu': -9.8600, 'sigma': 0.8457},
]

JOINT_PROB = [
    {'mu': -9.5738, 'sigma': 0.1638},
    {'mu': -12.8316, 'sigma': 0.7575},
    {'mu': -11.8743, 'sigma': 0.4750},
    {'mu': -12.0156, 'sigma': 0.5302},
    {'mu': -12.1486, 'sigma': 0.5652},
]

PIPE_PROB = [
    {'mu': -11.8584, 'sigma': 0.6570},
    {'mu': -12.5337, 'sigma': 0.6884},
    {'mu': -13.8662, 'sigma': 1.1276},
    {'mu': -14.5757, 'sigma': 1.1555},
    {'mu': -15.7261, 'sigma': 1.7140},
]

VALVE_PROB = [
    {'mu': -5.1796, 'sigma': 0.1728},
    {'mu': -7.2748, 'sigma': 0.3983},
    {'mu': -9.6802, 'sigma': 0.9607},
    {'mu': -10.3230, 'sigma': 0.6756},
    {'mu': -11.996, 'sigma': 1.3304},
]

INSTRUMENT_PROB = [
    {'mu': -7.3205, 'sigma': 0.6756},
    {'mu': -8.5018, 'sigma': 0.7938},
    {'mu': -9.0619, 'sigma': 0.8952},
    {'mu': -9.1711, 'sigma': 1.0674},
    {'mu': -10.1962, 'sigma': 1.4795},
]

EXTRA_COMP1_PROB = [
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
]

EXTRA_COMP2_PROB = [
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
    {'mu': 0., 'sigma': 0.},
]

LEAK_PROBABILITIES = {
    'compressor': COMPRESSOR_PROB,
    'cylinder': CYLINDER_PROB,
    'filter': FILTER_PROB,
    'flange': FLANGE_PROB,
    'hose': HOSE_PROB,
    'joint': JOINT_PROB,
    'pipe': PIPE_PROB,
    'valve': VALVE_PROB,
    'instrument': INSTRUMENT_PROB,
    'extra1': EXTRA_COMP1_PROB,
    'extra2': EXTRA_COMP2_PROB,
}
