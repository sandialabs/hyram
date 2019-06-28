"""
External access-point of QRA module.
Avoid importing/accessing non-api qra python files/functions externally.
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

import json
from .utilities import misc_utils as misc
from . import analysis


def test_inputs(*args, **kwargs):
    for arg in args:
        print("{} | {}".format(arg, type(arg)))

    for key, val in kwargs.items():
        print("{} {} | {}".format(key, val, type(val)))

    return True


def conduct_analysis_from_dict(input_data, print_results=False, is_json=False):
    """
    Procedural QRA analysis entry point.
    This computes the risk metrics and scenario stats. Each call requires all inputs.
    No default values are assumed.
    For advanced usage with sane defaults, see analysis_advanced.

    For a description of each parameter, see analysis.conduct_analysis().

    Returns
    -------

    """
    if is_json:
        input_data = json.loads(input_data)  # Convert json to dict

    # Retrieve all analysis parameters. See analysis function for parameter descriptions.
    pipe_length = misc.get_or_error(input_data, 'pipe_length', 'Pipe length required')
    pipe_outer_diam = misc.get_or_error(input_data, 'pipe_outer_diam', 'Pipe outer diameter required')
    pipe_thickness = misc.get_or_error(input_data, 'pipe_thickness', 'Pipe thickness required')

    h2_temp = misc.get_or_error(input_data, 'h2_temp', 'Hydrogen temperature parameter required')
    h2_pressure = misc.get_or_error(input_data, 'h2_pressure', 'Hydrogen pressure parameter required')
    discharge_coeff = misc.get_or_error(input_data, 'discharge_coeff', 'Discharge coefficient required')
    amb_temp = misc.get_or_error(input_data, 'amb_temp', 'Ambient temperature parameter required')
    amb_pressure = misc.get_or_error(input_data, 'amb_pressure', 'Ambient pressure parameter required')

    # Facility inputs
    facil_length = misc.get_or_error(input_data, 'facil_length', 'Facility length required')
    facil_width = misc.get_or_error(input_data, 'facil_width', 'Facility width required')
    facil_height = misc.get_or_error(input_data, 'facil_height', 'Facility height required')

    num_vehicles = misc.get_or_error(input_data, 'num_vehicles', 'Number of vehicles required')
    fuelings_per_day = misc.get_or_error(input_data, 'fuelings_per_day',
                                         'Number of fuelings per vehicle per day required')
    vehicle_operating_days = misc.get_or_error(input_data, 'vehicle_operating_days', 'Vehicle operating days required')

    ign_prob_ranges = misc.get_or_error(input_data, 'ign_prob_ranges', 'Ignition probability data required')

    detect_gas_and_flame = misc.get_or_error(input_data, 'detect_gas_and_flame', 'Specify whether gas detection used')
    if detect_gas_and_flame:
        gas_detection_credit = misc.get_or_error(input_data, 'gas_detection_credit', 'Enter a gas detection credit')
    else:
        gas_detection_credit = 0.

    num_compressors = int(misc.get_or_error(input_data, 'num_compressors', 'Enter number of compressors used'))
    num_cylinders = int(misc.get_or_error(input_data, 'num_cylinders', 'Enter number of cylinders used'))
    num_valves = int(misc.get_or_error(input_data, 'num_valves', 'Enter number of valves used'))
    num_instruments = int(misc.get_or_error(input_data, 'num_instruments', 'Enter number of instruments used'))
    num_joints = int(misc.get_or_error(input_data, 'num_joints', 'Enter number of joints used'))
    num_hoses = int(misc.get_or_error(input_data, 'num_hoses', 'Enter number of hoses used'))
    num_filters = int(misc.get_or_error(input_data, 'num_filters', 'Enter number of filters used'))
    num_flanges = int(misc.get_or_error(input_data, 'num_flanges', 'Enter number of flanges used'))
    num_extra_comp1 = int(misc.get_or_error(input_data, 'num_extra_comp1', 'Enter number of extra components 1'))
    num_extra_comp2 = int(misc.get_or_error(input_data, 'num_extra_comp2', 'Enter number of extra components 2'))

    # Harm model inputs
    probit_thermal_model_id = misc.get_or_error(input_data, 'probit_thermal_model_id', 'Probit thermal model required')
    exposure_time = misc.get_or_error(input_data, 'thermal_exposure_time', 'Thermal exposure time required')

    probit_overp_model_id = misc.get_or_error(input_data, 'probit_overp_model_id', 'Probit overpressure model required')
    peak_overp_list = input_data.get('peak_overp', [2.5e3, 2.5e3, 5e3, 16.e3, 30.e3])
    overp_impulse_list = input_data.get('overp_impulse', [0., 0, 0, 0, 0.])  # [250, 500, 1000, 2000, 4000]  Pa * s
    overp_frag_mass = input_data.get('overp_frag_mass', None)
    overp_velocity = input_data.get('overp_velocity', None)
    overp_total_mass = input_data.get('overp_total_mass', None)

    rad_source_model = misc.get_or_error(input_data, 'rad_source_model', 'Select a radiative source model')
    notional_nozzle_model = misc.get_or_error(input_data, 'notional_nozzle_model', 'Select a notional nozzle model')

    leak_height = input_data.get('leak_height', 1.)
    release_angle = input_data.get('release_angle', 0.)
    excl_radius = input_data.get('excl_radius', 0.01)
    rand_seed = int(input_data.get('rand_seed', 3632850))
    rel_humid = input_data.get('rel_humid', 0.89)

    release_freq_000d01 = input_data.get('release_freq_000d01')
    release_freq_000d10 = input_data.get('release_freq_000d10')
    release_freq_001d00 = input_data.get('release_freq_001d00')
    release_freq_010d00 = input_data.get('release_freq_010d00')
    release_freq_100d00 = input_data.get('release_freq_100d00')
    fueling_fail_freq_override = input_data.get('fueling_fail_freq_override')

    occupant_input_list = misc.get_or_error(input_data, 'occupants', 'Create one or more occupant groups')

    result_dict = analysis.conduct_analysis(pipe_length=pipe_length, pipe_outer_diam=pipe_outer_diam,
                                            pipe_thickness=pipe_thickness,
                                            h2_temp=h2_temp, h2_pressure=h2_pressure, amb_temp=amb_temp,
                                            amb_pressure=amb_pressure,
                                            discharge_coeff=discharge_coeff,
                                            facil_length=facil_length, facil_width=facil_width,
                                            facil_height=facil_height,
                                            num_vehicles=num_vehicles, fuelings_per_day=fuelings_per_day,
                                            vehicle_operating_days=vehicle_operating_days,
                                            ign_prob_ranges=ign_prob_ranges,
                                            detect_gas_and_flame=detect_gas_and_flame,
                                            gas_detection_credit=gas_detection_credit,
                                            num_compressors=num_compressors, num_cylinders=num_cylinders,
                                            num_valves=num_valves,
                                            num_instruments=num_instruments, num_joints=num_joints, num_hoses=num_hoses,
                                            num_filters=num_filters,
                                            num_flanges=num_flanges, num_extra_comp1=num_extra_comp1,
                                            num_extra_comp2=num_extra_comp2,
                                            probit_thermal_model_id=probit_thermal_model_id,
                                            thermal_exposure_time=exposure_time,
                                            probit_overp_model_id=probit_overp_model_id,
                                            overp_frag_mass=overp_frag_mass,
                                            overp_velocity=overp_velocity, overp_total_mass=overp_total_mass,
                                            peak_overp_list=peak_overp_list, overp_impulse_list=overp_impulse_list,
                                            rad_source_model=rad_source_model,
                                            notional_nozzle_model=notional_nozzle_model,
                                            leak_height=leak_height, release_angle=release_angle,
                                            excl_radius=excl_radius, rand_seed=rand_seed, rel_humid=rel_humid,
                                            occupant_input_list=occupant_input_list,
                                            print_results=print_results,
                                            release_freq_000d01=release_freq_000d01,
                                            release_freq_000d10=release_freq_000d10,
                                            release_freq_001d00=release_freq_001d00,
                                            release_freq_010d00=release_freq_010d00,
                                            release_freq_100d00=release_freq_100d00,
                                            fueling_fail_freq_override=fueling_fail_freq_override,
                                            )

    return result_dict
