"""
Parameters for Lognormal distribution for component leak frequencies
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


h2_cng_leak_frequency_parameters = {
    'compressor': [
        {'mu': -1.7336, 'sigma': 0.2247},
        {'mu': -3.9463, 'sigma': 0.5027},
        {'mu': -5.1554, 'sigma': 0.8000},
        {'mu': -8.8441, 'sigma': 0.8402},
        {'mu': -11.3361, 'sigma': 1.3688},
    ],
    'cylinder': [
        {'mu': -13.9235, 'sigma': 0.6717},
        {'mu': -14.0646, 'sigma': 0.6476},
        {'mu': -14.4389, 'sigma': 0.6513},
        {'mu': -14.9903, 'sigma': 0.6510},
        {'mu': -15.6250, 'sigma': 0.6827},
    ],
    'filter': [
        {'mu': -5.2470, 'sigma': 1.9850},
        {'mu': -5.2884, 'sigma': 1.5181},
        {'mu': -5.3393, 'sigma': 1.4810},
        {'mu': -5.3758, 'sigma': 0.8887},
        {'mu': -5.4262, 'sigma': 0.9548},
    ],
    'flange': [
        {'mu': -3.9229, 'sigma': 1.6609},
        {'mu': -6.1215, 'sigma': 1.2537},
        {'mu': -8.3307, 'sigma': 2.2026},
        {'mu': -10.5387, 'sigma': 0.8326},
        {'mu': -12.7454, 'sigma': 1.8277},
    ],
    'hose': [
        {'mu': -6.8268, 'sigma': 0.2833},
        {'mu': -8.7346, 'sigma': 0.6140},
        {'mu': -8.8500, 'sigma': 0.5939},
        {'mu': -8.9568, 'sigma': 0.5897},
        {'mu': -9.9111, 'sigma': 0.8777},
    ],
    'joint': [
        {'mu': -9.5816, 'sigma': 0.1699},
        {'mu': -12.9214, 'sigma': 0.8141},
        {'mu': -11.9328, 'sigma': 0.5140},
        {'mu': -12.0872, 'sigma': 0.5772},
        {'mu': -12.2182, 'sigma': 0.6106},
    ],
    'pipe': [
        {'mu': -11.9126, 'sigma': 0.6917},
        {'mu': -12.5711, 'sigma': 0.7122},
        {'mu': -13.8834, 'sigma': 1.1383},
        {'mu': -14.5859, 'sigma': 1.1619},
        {'mu': -15.7292, 'sigma': 1.7161},
    ],
    'valve': [
        {'mu': -5.1881, 'sigma': 0.1794},
        {'mu': -7.3069, 'sigma': 0.4204},
        {'mu': -9.7139, 'sigma': 0.9817},
        {'mu': -10.3381, 'sigma': 0.6854},
        {'mu': -11.9999, 'sigma': 1.3330},
    ],
    'instrument': [
        {'mu': -7.3815, 'sigma': 0.7146},
        {'mu': -8.5400, 'sigma': 0.8179},
        {'mu': -9.1045, 'sigma': 0.9218},
        {'mu': -9.2086, 'sigma': 1.0906},
        {'mu': -10.2084, 'sigma': 1.4870},
    ],
    'extra1': [
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
    ],
    'extra2': [
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
    ],
}


# TODO: update these; they're just copies of H2
cng_leak_frequency_parameters = {
    'compressor': [
        {'mu': -1.7, 'sigma': 0.2},
        {'mu': -3.9, 'sigma': 0.5},
        {'mu': -5.1, 'sigma': 0.8},
        {'mu': -8.8, 'sigma': 0.8},
        {'mu': -11.3, 'sigma': 1.3},
    ],
    'cylinder': [
        {'mu': -13.9, 'sigma': 0.6},
        {'mu': -14.0, 'sigma': 0.6},
        {'mu': -14.4, 'sigma': 0.6},
        {'mu': -14.9, 'sigma': 0.6},
        {'mu': -15.6, 'sigma': 0.6},
    ],
    'filter': [
        {'mu': -5.2, 'sigma': 1.9},
        {'mu': -5.2, 'sigma': 1.5},
        {'mu': -5.3, 'sigma': 1.4},
        {'mu': -5.3, 'sigma': 0.8},
        {'mu': -5.4, 'sigma': 0.9},
    ],
    'flange': [
        {'mu': -3.9229, 'sigma': 1.6609},
        {'mu': -6.1215, 'sigma': 1.2537},
        {'mu': -8.3307, 'sigma': 2.2026},
        {'mu': -10.5387, 'sigma': 0.8326},
        {'mu': -12.7454, 'sigma': 1.8277},
    ],
    'hose': [
        {'mu': -6.8268, 'sigma': 0.2833},
        {'mu': -8.7346, 'sigma': 0.6140},
        {'mu': -8.8500, 'sigma': 0.5939},
        {'mu': -8.9568, 'sigma': 0.5897},
        {'mu': -9.9111, 'sigma': 0.8777},
    ],
    'joint': [
        {'mu': -9.5816, 'sigma': 0.1699},
        {'mu': -12.9214, 'sigma': 0.8141},
        {'mu': -11.9328, 'sigma': 0.5140},
        {'mu': -12.0872, 'sigma': 0.5772},
        {'mu': -12.2182, 'sigma': 0.6106},
    ],
    'pipe': [
        {'mu': -11.9126, 'sigma': 0.6917},
        {'mu': -12.5711, 'sigma': 0.7122},
        {'mu': -13.8834, 'sigma': 1.1383},
        {'mu': -14.5859, 'sigma': 1.1619},
        {'mu': -15.7292, 'sigma': 1.7161},
    ],
    'valve': [
        {'mu': -5.1881, 'sigma': 0.1794},
        {'mu': -7.3069, 'sigma': 0.4204},
        {'mu': -9.7139, 'sigma': 0.9817},
        {'mu': -10.3381, 'sigma': 0.6854},
        {'mu': -11.9999, 'sigma': 1.3330},
    ],
    'instrument': [
        {'mu': -7.3815, 'sigma': 0.7146},
        {'mu': -8.5400, 'sigma': 0.8179},
        {'mu': -9.1045, 'sigma': 0.9218},
        {'mu': -9.2086, 'sigma': 1.0906},
        {'mu': -10.2084, 'sigma': 1.4870},
    ],
    'extra1': [
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
    ],
    'extra2': [
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
        {'mu': 0., 'sigma': 0.},
    ],
}


def get_leak_frequency_set_for_species(species='H2'):
    if species == 'CNG':
        leak_params = cng_leak_frequency_parameters
    else:
        leak_params = h2_cng_leak_frequency_parameters
    return leak_params


def get_component_leak_frequency_parameters(component, species='H2'):
    leak_params = get_leak_frequency_set_for_species(species)
    return leak_params[component]