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

from hyram.qra import api
from hyram.utilities.constants import IN_TO_M


def run_default():
    ignition_prob_data = [
        {'threshold_min': None, 'threshold_max': 0.125, 'immed_prob': 0.008, 'delay_prob': 0.004},
        {'threshold_min': 0.125, 'threshold_max': 6.25, 'immed_prob': 0.053, 'delay_prob': 0.027},
        {'threshold_min': 6.25, 'threshold_max': None, 'immed_prob': 0.230, 'delay_prob': 0.120},
    ]

    input_dict = {
        'pipe_outer_diam': 0.375 * IN_TO_M,
        'pipe_thickness': 0.065 * IN_TO_M,
        'h2_temp': 288.15,
        'h2_pressure': 35.e6,
        'discharge_coeff': 1.,
        'amb_temp': 288.15,
        'amb_pressure': 101325.,

        'facil_length': 20,
        'facil_width': 12,
        'facil_height': 5,

        'num_vehicles': 20,
        'fuelings_per_day': 2,
        'vehicle_operating_days': 250,
        'annual_demands': 10000,

        'ign_prob_ranges': ignition_prob_data,
        'detect_gas_and_flame': True,
        'gas_detection_credit': 0.9,

        'occupants': [
            {
                'count': 9,
                'descrip': 'workers',
                'xdistr': 'uniform',
                'xa': 1,
                'xb': 20,
                'ydistr': 'deterministic',
                'ya': 1,
                'yb': None,
                'zdistr': 'uniform',
                'za': 1,
                'zb': 12,
                'hours': 2000,
            }
        ],
        'num_compressors': 0,
        'num_cylinders': 0,
        'num_valves': 5,
        'num_instruments': 3,
        'num_joints': 35,
        'num_hoses': 1,
        'pipe_length': 20,
        'num_filters': 0,
        'num_flanges': 0,
        'num_extra_comp1': 0,
        'num_extra_comp2': 0,

        'probit_thermal_model_id': 'eis',
        'thermal_exposure_time': 60,
        'probit_overp_model_id': 'col',  # elh, lhs, hea, col, deb
        'rad_source_model': 'multi',
        'rel_humid': 0.89,
        'notional_nozzle_model': 'bir2',
        'rand_seed': 3632850,
        'excl_radius': 0.01,
        'release_freq_000d01': -1,
        'release_freq_000d10': -1,
        'release_freq_001d00': -1,
        'release_freq_010d00': -1,
        'release_freq_100d00': -1,
        'fueling_fail_freq_override': -1,
    }
    results = api.conduct_analysis_from_dict(input_dict, print_results=True)
    return results


def run_harm_model():
    ignition_prob_data = [
        {'threshold_min': None, 'threshold_max': 0.125, 'immed_prob': 0.008, 'delay_prob': 0.004},
        {'threshold_min': 0.125, 'threshold_max': 6.25, 'immed_prob': 0.053, 'delay_prob': 0.027},
        {'threshold_min': 6.25, 'threshold_max': None, 'immed_prob': 0.230, 'delay_prob': 0.120},
    ]

    input_dict = {
        'pipe_outer_diam': 0.375 * IN_TO_M,
        'pipe_thickness': 0.065 * IN_TO_M,
        'h2_temp': 288,
        'h2_pressure': 35.e6,
        'discharge_coeff': 1.,
        'amb_temp': 288,
        'amb_pressure': 101325.,

        'facil_length': 20,
        'facil_width': 12,
        'facil_height': 5,

        'num_vehicles': 20,
        'fuelings_per_day': 2,
        'vehicle_operating_days': 250,
        'annual_demands': 10000,

        'ign_prob_ranges': ignition_prob_data,
        'detect_gas_and_flame': True,
        'gas_detection_credit': 0.9,

        'occupants': [
            {
                'count': 9,
                'descrip': 'workers',
                'xdistr': 'uniform',
                'xa': 1,
                'xb': 20,
                'ydistr': 'deterministic',
                'ya': 1,
                'yb': None,
                'zdistr': 'uniform',
                'za': 1,
                'zb': 12,
                'hours': 2000,
            }
        ],
        'num_compressors': 0,
        'num_cylinders': 0,
        'num_valves': 5,
        'num_instruments': 3,
        'num_joints': 35,
        'num_hoses': 1,
        'pipe_length': 20,
        'num_filters': 0,
        'num_flanges': 0,
        'num_extra_comp1': 0,
        'num_extra_comp2': 0,

        'probit_thermal_model_id': 'eis',  # eis, tsa,
        'thermal_exposure_time': 60,
        'probit_overp_model_id': 'elh',  # elh, lhs, hea, col, deb
        'rad_source_model': 'multi',
        'rel_humid': 0.89,
        'notional_nozzle_model': 'YuceilOtugen',
        'rand_seed': 3632850,
        'excl_radius': 0.01,
    }
    results = api.conduct_analysis_from_dict(input_dict, print_results=True)
    return results


if __name__ == "__main__":
    results = run_default()
