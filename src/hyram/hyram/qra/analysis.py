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

import datetime
import logging
import os

import numpy as np

from . import probit
from . import occupants as occ_lib
from .flame_data import FlameData
from ..utilities import constants, misc_utils, exceptions
from .components import ComponentSet, LeakSizeResult
from .data import failure_data

from ..phys import api as phys_api


log = logging.getLogger(__name__)


def conduct_analysis(pipe_outer_diam, pipe_thickness,
                     amb_temp, amb_pres,
                     rel_temp, rel_pres, rel_phase,
                     facil_length, facil_width, facil_height,
                     num_vehicles, daily_fuelings, vehicle_days,
                     ign_prob_ranges,
                     occupant_input_list,
                     discharge_coeff=1., detect_gas_flame=True, detection_credit=0.9,
                     probit_thermal_id='eise', exposure_time=60, probit_rel_id='coll',
                     rad_source_model='multi', nozzle_model='yuce',
                     leak_height=0., rel_angle=0., rel_humid=0.89, rand_seed=3632850, excl_radius=0.01,
                     overp_frag_mass=None, overp_velocity=None, overp_total_mass=None,
                     peak_overp_list=None, overp_impulse_list=None,

                     # Components
                     compressor_leak_probs=None,
                     cylinder_leak_probs=None,
                     valve_leak_probs=None,
                     instrument_leak_probs=None,
                     pipe_leak_probs=None,
                     joint_leak_probs=None,
                     hose_leak_probs=None,
                     filter_leak_probs=None,
                     flange_leak_probs=None,
                     extra_comp1_leak_probs=None,
                     extra_comp2_leak_probs=None,
                     pipe_length=0,
                     num_compressors=0, num_cylinders=0, num_valves=0, num_instruments=0, num_joints=0, num_hoses=0,
                     num_filters=0, num_flanges=0, num_extra_comp1=0, num_extra_comp2=0,

                     noz_po_dist='beta', noz_po_a=0.5, noz_po_b=610415.5,
                     # noz_ftc_dist='beta', noz_ftc_a=31.5, noz_ftc_b=610384.5,
                     noz_ftc_dist='expv', noz_ftc_a=0.002, noz_ftc_b=None,
                     mvalve_ftc_dist='expv', mvalve_ftc_a=0.001, mvalve_ftc_b=None,
                     svalve_ftc_dist='expv', svalve_ftc_a=0.002, svalve_ftc_b=None,
                     svalve_ccf_dist='expv', svalve_ccf_a=0.000127659574468085, svalve_ccf_b=None,
                     overp_dist='beta', overp_a=3.5, overp_b=310289.5,
                     pvalve_fto_dist='logn', pvalve_fto_a=-11.7359368859313, pvalve_fto_b=0.667849415603714,
                     driveoff_dist='beta', driveoff_a=31.5, driveoff_b=610384.5,
                     coupling_ftc_dist='beta', coupling_ftc_a=0.5, coupling_ftc_b=5031.,
                     release_freq_000d01=-1.,
                     release_freq_000d10=-1.,
                     release_freq_001d00=-1.,
                     release_freq_010d00=-1.,
                     release_freq_100d00=-1.,
                     fueling_fail_freq_override=-1.,

                     print_results=False,
                     debug=True,
                     output_dir=None,
                     create_plots=True,
                     ):
    """
    Quantitative risk assessment including scenario calculations and harm modeling.
    Note: facility height currently unused.

    Default units of measurement:
        Pressure [Pa]
        distance [m]
        time [s]


    Parameters
    ----------
    compressor_leak_probs : list of dicts
        List of component probability data for compressors for each leak size. Each entry is {mu, sigma, mean, variance}.
        If mu and sigma provided, mean and variance are not used. If mu or sigma missing, mean and variance are used.

    cylinder_leak_probs : list of dicts
        For cylinders per above.

    valve_leak_probs : list of dicts
        For valves per above.

    instrument_leak_probs : list of dicts
        For instruments per above.

    pipe_leak_probs : list of dicts
        For pipes per above.

    joint_leak_probs : list of dicts
        For joints per above.

    hose_leak_probs : list of dicts
        For hoses per above.

    filter_leak_probs : list of dicts
        For filters per above.

    flange_leak_probs : list of dicts
        For flanges per above.

    extra_comp1_leak_probs : list of dicts
        For extra component type 1 per above.

    extra_comp2_leak_probs : list of dicts
        For extra component type 2 per above.

    noz_po_dist : str
        ID of distribution for pop-off failure mode.

    noz_po_a : float
        Failure distribution parameter A

    noz_po_b : float or None
        Failure distribution parameter B. None if using Expected Value

    noz_ftc_dist : str
        ID of distribution for nozzle failure-to-close failure mode.

    noz_ftc_a : float
        Failure distribution parameter A

    noz_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    mvalve_ftc_dist : str
        ID of distribution for manual valve FTC failure mode.

    mvalve_ftc_a : float
        Failure distribution parameter A

    mvalve_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    svalve_ftc_dist : str
        ID of distribution for solenoid valve FTC failure mode.

    svalve_ftc_a : float
        Failure distribution parameter A

    svalve_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    svalve_ccf_dist : str
        ID of distribution for solenoid valve common-cause failure mode.

    svalve_ccf_a : float
        Failure distribution parameter A

    svalve_ccf_b : float or None
        Failure distribution parameter B. None if using Expected Value

    overp_dist : str
        ID of distribution for overpressure failure mode.

    overp_a : float
        Failure distribution parameter A

    overp_b : float or None
        Failure distribution parameter B. None if using Expected Value

    pvalve_fto_dist : str
        ID of distribution for pressure-relief valve failure-to-open failure mode.

    pvalve_fto_a : float
        Failure distribution parameter A

    pvalve_fto_b : float or None
        Failure distribution parameter B. None if using Expected Value

    driveoff_dist : str
        ID of distribution for driveoff failure mode.

    driveoff_a : float
        Failure distribution parameter A

    driveoff_b : float or None
        Failure distribution parameter B. None if using Expected Value

    coupling_ftc_dist : str
        ID of distribution for coupling failure-to-close failure mode.

    coupling_ftc_a : float
        Failure distribution parameter A

    coupling_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    release_freq_000d01 : float
        Manual override value for H2 release frequency at specified release size. Ignored if == -1.0

    release_freq_000d10 : float
        Manual override value for H2 release frequency at specified release size. Ignored if == -1.0

    release_freq_001d00 : float
        Manual override value for H2 release frequency at specified release size. Ignored if == -1.0

    release_freq_010d00 : float
        Manual override value for H2 release frequency at specified release size. Ignored if == -1.0

    release_freq_100d00 : float
        Manual override value for H2 release frequency at specified release size. Ignored if == -1.0

    fueling_fail_freq_override : float or None
        If not None, user has provided a manual value for probability of accidents/shutdown failure

    pipe_length : float
        [m] total lengths of component pipe(s)

    pipe_outer_diam : float
        [m] Outer diameter of pipe

    pipe_thickness : float
        [m] Thickness of pipe wall (single side)

    rel_temp : float
        [K] Hydrogen temperature

    rel_pres : float
        [Pa] Hydrogen pressure

    amb_temp : float
        [K] Ambient temperature

    amb_pres : float
        [Pa] Ambient pressure

    discharge_coeff : float
        [-] Discharge coefficient to account for non-plug flow (always <=1, assumed to be 1 for plug flow)

    facil_length : float
        [m] Length of facility

    facil_width : float
        [m] Width of facility

    facil_height : float
        [m] Height of facility

    num_vehicles : int
        Number of vehicles in use.

    daily_fuelings : int
        Number of fuelings per day for each vehicle

    vehicle_days : int
        Annual days of operations

    ign_prob_ranges : list of dicts
        Minimum and maximum thresholds for rates of immediate and delayed ignition.
        Min or max can be None (null) to represent +/- infinity
        Format: {threshold_min, threshold_max, immed_prob, delay_prob}

    detect_gas_flame : bool
        Whether credit should be applied.

    detection_credit : float

    num_compressors : int
        Number of compressors in system

    num_cylinders : int
        Number of cylinders in system

    num_valves : int
        Number of valves in system

    num_instruments : int
        Number of instruments in system

    num_joints : int
        Number of joints in system

    num_hoses : int
        Number of hoses in system

    num_filters : int
        Number of filters in system

    num_flanges : int
        Number of flanges in system

    num_extra_comp1 : int
        Number of extra components of type 1 in system

    num_extra_comp2 : int
        Number of extra components of type 2 in system

    probit_thermal_id : {'eise', 'tsao', 'tno', 'lees'}
        4-char ID of thermal harm model to use.

    exposure_time : float
        [s] Duration of exposure to heat source

    probit_rel_id : {'leis', 'lhse', 'head', 'coll', 'debr'}
        4-char ID of overpressure harm model to use.
        See probit.py for current options.

    overp_frag_mass : float
        [kg] For debris overpressure method; mass of (individual) fragments

    overp_velocity : float
        [m/s] Debris velocity for use in debris overpressure model

    overp_total_mass : float
        [kg] total mass of all debris, for use in debris overpressure model

    peak_overp_list : list
        [Pa] Peak overpressure for each leak size

    overp_impulse_list : list
        [Pa*s] Impulse of shock wave for each leak size

    rad_source_model : {'single', 'multi'}
        Radiative source description.

    nozzle_model : str
        4-char key referencing notional nozzle model to use for high-pressure release. See phys h2_nn for options.

    leak_height : float
        [m] Vertical height of leak, for use in qrad flame calculation

    rel_angle : float
        [deg] Leak release angle for use in qrad flame calculation

    excl_radius : float
        [m] Exclusion radius, for use in qrad flame calculation

    rand_seed : int
        Random seeding for flame calculation

    rel_humid : float
        Relative humidity between 0.0 and 1.0

    occupant_input_list : list of dicts
        Each dict defines group of occupants/workers near radiative source.
        Format: {count, descrip, xdistr, xa, xb, ydistr, ya, yb, zdistr, za, zb, hours}
        Where xdistr can be uniform, deterministic or normal.
        Example:
            {
                'count': 9,
                'descrip': 'workers',
                'xdistr': 'uniform',
                'xa': 1,
                'xb': 20,
                'ydistr': 'dete',
                'ya': 1,
                'yb': None,
                'zdistr': 'unif',
                'za': 1,
                'zb': 12,
                'hours': 2000,
            }

    output_dir : str
        Path of directory for saving temp data, e.g. pickled flame data

    Returns
    -------
    results : dict
        Compilation of analysis results containing:
            air : float
                Average Individual Risk is expected # of fatalities per exposed individual
            far : float
                Fatal Accident Rate is expected # of fatalities per 100 million exposed hours
            total_pll : float
                Potential Loss of Life is expected # of fatalities per system year
            plot_files : list of strings
                File locations of QRAD plots for each leak size, in order

            leak_results : list of LeakResult objects
                Each contains PLL contribution, expected probabilities for scenarios, and component leak probabilities

    """
    now = datetime.datetime.now()
    params = locals()  # for logging
    sorted_params = sorted(params)

    if peak_overp_list is None:
        peak_overp_list = [2.5e3, 2.5e3, 5e3, 16.e3, 30.e3]

    if overp_impulse_list is None:
        overp_impulse_list = [0., 0, 0, 0, 0.]

    if not detect_gas_flame:
        detection_credit = 0.

    calc_shutdown = True  # set to False to match prev version

    if output_dir is None:
        dir_path = os.path.dirname(os.path.realpath(__file__))
        output_dir = os.path.join(dir_path, 'temp')

    # Record all parameters in log file
    log.info("=== NEW ANALYSIS... {} ===\nSorted Parameters:".format(now))
    for param in sorted_params:
        log.info("{}: {}".format(param, params[param]))
    log.info("output_dir: {}".format(output_dir))

    amb_fluid = phys_api.create_fluid('AIR', amb_temp, amb_pres)
    rel_fluid = phys_api.create_fluid('H2', rel_temp, rel_pres, phase=rel_phase)
    if rel_temp is None:
        rel_temp = rel_fluid.T
    if amb_temp is None:
        amb_temp = amb_fluid.T

    # Each occupant row in GUI is represented as group and stored as dict inside list
    # Massage into required format for phys module (xdistr, xa, xb, etc.)
    # NOTE (Cianan): This can be cleaned up; don't need objects here.
    occupant_groups = occ_lib.create_occupant_groups_from_list(occupant_input_list)

    loc_distributions = []
    for group in occupant_groups:
        loc_distributions.append(group.generate_physics_format())

    total_occupants = np.sum([group.num_occupants for group in occupant_groups])
    occupant_avg_hours = np.sum([group.hours * group.num_occupants for group in occupant_groups]) / total_occupants

    log.info("Location distributions: {}".format(loc_distributions))
    log.info("{} Occupants for {} average hours".format(total_occupants, occupant_avg_hours))

    compressor_set = ComponentSet('compressor', num_compressors, leak_probs=compressor_leak_probs)
    cylinder_set = ComponentSet('cylinder', num_cylinders, leak_probs=cylinder_leak_probs)
    valve_set = ComponentSet('valve', num_valves, leak_probs=valve_leak_probs)
    instrument_set = ComponentSet('instrument', num_instruments, leak_probs=instrument_leak_probs)
    joint_set = ComponentSet('joint', num_joints, leak_probs=joint_leak_probs)
    hose_set = ComponentSet('hose', num_hoses, leak_probs=hose_leak_probs)
    pipe_set = ComponentSet('pipe', int(pipe_length), leak_probs=pipe_leak_probs)
    filter_set = ComponentSet('filter', num_filters, leak_probs=filter_leak_probs)
    flange_set = ComponentSet('flange', num_flanges, leak_probs=flange_leak_probs)
    extra_comp1_set = ComponentSet('extra1', num_extra_comp1, leak_probs=extra_comp1_leak_probs)
    extra_comp2_set = ComponentSet('extra2', num_extra_comp2, leak_probs=extra_comp2_leak_probs)

    component_sets = [compressor_set, cylinder_set, valve_set, instrument_set, joint_set, hose_set, pipe_set,
                      filter_set, flange_set,
                      extra_comp1_set, extra_comp2_set]

    comp_fail_noz_po = failure_data.ComponentFailure('Nozzle', 'Pop-off',
                                                     noz_po_dist, noz_po_a, noz_po_b)
    comp_fail_noz_ftc = failure_data.ComponentFailure('Nozzle', 'Failure to close',
                                                      noz_ftc_dist, noz_ftc_a, noz_ftc_b)
    comp_fail_mvalve_ftc = failure_data.ComponentFailure('Manual valve', 'Failure to close',
                                                         mvalve_ftc_dist, mvalve_ftc_a, mvalve_ftc_b)
    comp_fail_svalve_ftc = failure_data.ComponentFailure('Solenoid valves', 'Failure to close',
                                                         svalve_ftc_dist, svalve_ftc_a, svalve_ftc_b)
    comp_fail_svalve_ccf = failure_data.ComponentFailure('Solenoid valves', 'Common-cause failure',
                                                         svalve_ccf_dist, svalve_ccf_a, svalve_ccf_b)

    comp_fail_overp = failure_data.ComponentFailure('Overpressure during fueling', 'Accident',
                                                    overp_dist, overp_a, overp_b)
    comp_fail_pvalve_fto = failure_data.ComponentFailure('Pressure-relief valve', 'Failure to open',
                                                         pvalve_fto_dist, pvalve_fto_a, pvalve_fto_b)
    comp_fail_driveoff = failure_data.ComponentFailure('Driveoff', 'Accident',
                                                       driveoff_dist, driveoff_a, driveoff_b)
    comp_fail_coupling_ftc = failure_data.ComponentFailure('Breakaway coupling', 'Failure to close',
                                                           coupling_ftc_dist, coupling_ftc_a, coupling_ftc_b)

    log.info("Component Sets:")
    for compset in component_sets:
        if compset.num_components:
            log.info(compset.get_description())

    # For each leak size, sum release freqs for all components at that size
    leak_result_000d01 = LeakSizeResult(constants.LEAK_SIZES[0])
    leak_result_000d10 = LeakSizeResult(constants.LEAK_SIZES[1])
    leak_result_001d00 = LeakSizeResult(constants.LEAK_SIZES[2])
    leak_result_010d00 = LeakSizeResult(constants.LEAK_SIZES[3])
    leak_result_100d00 = LeakSizeResult(constants.LEAK_SIZES[4])
    num_leak_sizes = len(constants.LEAK_SIZES)

    leak_results = [leak_result_000d01, leak_result_000d10, leak_result_001d00, leak_result_010d00, leak_result_100d00]
    release_freq_overrides = [release_freq_000d01, release_freq_000d10, release_freq_001d00, release_freq_010d00,
                              release_freq_100d00]

    # Compute leak frequencies for each leak size
    for i, leak_size in enumerate(constants.LEAK_SIZES):
        leak_result = leak_results[i]
        release_freq_override = release_freq_overrides[i]

        # Use override value if provided (i.e. other than -1.0)
        if release_freq_override != -1.:
            leak_result.release_freq_override = release_freq_override
            leak_result.total_release_freq = release_freq_override
            # Zero out component leak freqs since unused
            for comp_set in component_sets:
                leak_result.component_leak_freqs[comp_set.category] = 0.

        # If no override, calculate based on component releases
        else:
            total_leak_freq = 0.
            for comp_set in component_sets:
                comp_freq = comp_set.get_freq_for_size(leak_size)
                leak_result.component_leak_freqs[comp_set.category] = comp_freq
                total_leak_freq += comp_freq
                log.debug("Leak {} for {}: {:.3g}".format(leak_size, comp_set.category, comp_freq))
            leak_result.total_release_freq = total_leak_freq

        log.info("Total release freq for size {}: {:.3g}\n".format(leak_size, leak_result.total_release_freq))

    # Account for non-leak fueling failure contributors in 100% release only. Use override value if provided
    log.info("Manual value for 100% leak vehicle fueling failure? {}".format(fueling_fail_freq_override))
    leak_result_100d00.fueling_fail_freq_override = fueling_fail_freq_override

    # User provided vehicle fueling failure frequency directly so ignore individual events and set it manually
    if fueling_fail_freq_override != -1.:
        leak_result_100d00.fueling_fail_freq = float(fueling_fail_freq_override)
        leak_result_100d00.total_release_freq += leak_result_100d00.fueling_fail_freq

    # fueling freq not provided; calculate event probabilities and frequencies and augment 100% release freq
    else:
        num_fuelings = num_vehicles * daily_fuelings * vehicle_days
        prob_driveoff_release = comp_fail_driveoff.mean * comp_fail_coupling_ftc.mean
        freq_driveoff_release = num_fuelings * prob_driveoff_release
        prob_overp_rupture = comp_fail_overp.mean * comp_fail_pvalve_fto.mean
        freq_overp_rupture = num_fuelings * prob_overp_rupture
        freq_accidents = freq_overp_rupture + freq_driveoff_release

        prob_nozzle_release = comp_fail_noz_po.mean + comp_fail_noz_ftc.mean
        freq_nozzle_release = num_fuelings * prob_nozzle_release

        prob_sol_valves_ftc = comp_fail_svalve_ftc.mean ** 3. + comp_fail_svalve_ccf.mean
        freq_sol_valves_ftc = num_fuelings * prob_sol_valves_ftc

        # Combined
        prob_shutdown_fail = prob_sol_valves_ftc * comp_fail_mvalve_ftc.mean * prob_nozzle_release
        freq_shutdown_fail = num_fuelings * prob_shutdown_fail

        leak_result_100d00.p_overp_rupture = np.around(prob_overp_rupture, 20)
        leak_result_100d00.f_overp_rupture = np.around(freq_overp_rupture, 20)

        leak_result_100d00.p_driveoff = np.around(prob_driveoff_release, 20)
        leak_result_100d00.f_driveoff = np.around(freq_driveoff_release, 20)

        leak_result_100d00.p_nozzle_release = np.around(prob_nozzle_release, 20)
        leak_result_100d00.f_nozzle_release = np.around(freq_nozzle_release, 20)

        leak_result_100d00.p_sol_valves_ftc = np.around(prob_sol_valves_ftc, 20)
        leak_result_100d00.f_sol_valves_ftc = np.around(freq_sol_valves_ftc, 20)

        leak_result_100d00.p_mvalve_ftc = np.around(comp_fail_mvalve_ftc.mean, 20)
        leak_result_100d00.f_mvalve_ftc = np.around(comp_fail_mvalve_ftc.mean * num_fuelings, 20)

        fueling_fail_freq = float(freq_accidents + freq_shutdown_fail)
        leak_result_100d00.fueling_fail_freq = np.around(fueling_fail_freq, 20)
        leak_result_100d00.total_release_freq += fueling_fail_freq

        log.info("Driveoff p, f: {:.3g}, {:.3g}".format(prob_driveoff_release, freq_driveoff_release))
        log.info("Overpressure rupture p, f: {:.3g}, {:.3g}".format(prob_overp_rupture, freq_overp_rupture))
        log.info("Nozzle release p, f: {:.3g}, {:.3g}".format(prob_nozzle_release, freq_nozzle_release))
        log.info("Sol valve FTC p, f: {:.3g}, {:.3g}".format(prob_sol_valves_ftc, freq_sol_valves_ftc))
        log.info("Shutdown fail p, f: {:.3g}, {:.3g}".format(prob_shutdown_fail, freq_shutdown_fail))
        log.info("Total freq of other failures: {:.3g}".format(fueling_fail_freq))

    total_leak_freqs = np.array([leak_res.total_release_freq for leak_res in leak_results])

    log.info("RELEASE FREQUENCIES:")
    log.info("    0.01% - {:.3g}".format(leak_result_000d01.total_release_freq))
    log.info("    0.10% - {:.3g}".format(leak_result_000d10.total_release_freq))
    log.info("    1.00% - {:.3g}".format(leak_result_001d00.total_release_freq))
    log.info("   10.00% - {:.3g}".format(leak_result_010d00.total_release_freq))
    log.info("  100.00% - {:.3g}".format(leak_result_100d00.total_release_freq))

    # IGNITION PROBABILITIES
    # Compute effective diameter of leak inside pipe, taking into account pipe thickness
    # NOTE: C# HyRAM incorrectly subtracted 1 * pipe_thickness.
    # Encapsulating error in pipe_thickness_factor to reproduce it during testing.
    pipe_inner_diam = pipe_outer_diam - 2. * pipe_thickness
    pipe_area = np.pi * (pipe_inner_diam / 2.) ** 2.

    # Convert sizes from percent to fraction when using in calculation
    leak_areas = (np.array(constants.LEAK_SIZES) / 100.) * pipe_area
    orifice_leak_diams = np.sqrt(4. * (leak_areas / np.pi))

    log.info("Pipe inner diam {:.3g} m, area {:.3g} m^2\n".format(pipe_inner_diam, pipe_area))

    # Compute discharge rates, one per leak size
    hy_discharge_rates = []
    for leak_diam in orifice_leak_diams:
        discharge_rate = phys_api.compute_discharge_rate(rel_fluid, leak_diam, dis_coeff=discharge_coeff, verbose=False)
        hy_discharge_rates.append(discharge_rate)
        log.info("Orifice leak diam: {:.3g}, discharge rate: {:.3g}".format(leak_diam, discharge_rate))

    hy_discharge_rates = np.array(hy_discharge_rates)

    # Determine ignition probabilities for each leak size based on discharge rates and thresholds
    ign_immed_probs = []
    ign_delay_probs = []
    for rate in hy_discharge_rates:
        for ign_range in ign_prob_ranges:
            thres_min = ign_range['threshold_min']
            thres_max = ign_range['threshold_max']
            # Ranges are in order so just check against max. If we get to end (i.e. max is null), then use its range.
            if rate < thres_max or (thres_max is None and rate >= thres_min):
                immed_prob = ign_range['immed_prob']
                delayed_prob = ign_range['delay_prob']
                ign_immed_probs.append(immed_prob)
                ign_delay_probs.append(delayed_prob)
                log.info("Flow rate {:.3g} ign probs: immed {}, delayed {}".format(rate, immed_prob, delayed_prob))
                break

    ign_immed_probs = np.array(ign_immed_probs)
    ign_delay_probs = np.array(ign_delay_probs)

    # Compute probabilities of outcomes: shutdown, jetfire, explosion, or no ignition
    # release_freqs = np.array([res.total_release_freq for res in leak_results], dtype=np.float64)

    # Probabilities within each leak size should sum to 1
    if detect_gas_flame:
        prob_shutdown_per_leak = detection_credit
        prob_jetfire_per_leak = ign_immed_probs * (1. - detection_credit)
        prob_explos_per_leak = ign_delay_probs * (1. - detection_credit)
        prob_no_ign_per_leak = (1. - ign_immed_probs - ign_delay_probs) * (1. - detection_credit)

    else:  # no gas detection credit
        prob_shutdown_per_leak = 0.
        prob_jetfire_per_leak = ign_immed_probs
        prob_explos_per_leak = ign_delay_probs
        prob_no_ign_per_leak = 1. - ign_immed_probs - ign_delay_probs

    # phys wrapper currently returns data in a list where each entry is tuple of (label, data)
    flame_data = FlameData(amb_temp, amb_pres, rel_temp, rel_pres, rel_phase, orifice_leak_diams, leak_height,
                                        rel_angle,
                                        nozzle_model, loc_distributions, excl_radius, rand_seed, rel_humid,
                                        rad_source_model, facil_length, facil_width)

    flame_file = os.path.join(output_dir, 'flame.pkl')

    # Attempt to load previously-pickled flame calc data.
    # Re-run calc if it's params are different or if load fails
    try:
        prev_flame_saved = False
        loaded_flame_data = misc_utils.load_object(flame_file)
        loaded_flame_data.__class__ = FlameData
        log.info("FLAME file loc: {}".format(flame_file))
        log.info("Analysis flame parameters: {}".format(vars(flame_data)))
        log.info("Loaded flame parameters: {}".format(vars(loaded_flame_data)))
        # Check whether all pickled flame inputs are identical to this run; if so, don't need to run flame calc
        if flame_data == loaded_flame_data:
            log.info("Flame data identical")
            prev_flame_saved = True
    except Exception as err:
        log.info("Flame load error. Will re-compute.")
        prev_flame_saved = False
        loaded_flame_data = None

    if prev_flame_saved:
        log.info("Loading flame data...")
        qrads = loaded_flame_data.qrads
        qrad_plot_files = loaded_flame_data.plot_files

    else:
        log.info("Re-computing flame data...")
        chem_file = os.path.join(output_dir, 'chem.pkl')

        flux_dict = phys_api.flux_analysis(amb_fluid, rel_fluid, rel_height=leak_height,
                                           rel_angle=np.radians(rel_angle),
                                           site_length=facil_length, site_width=facil_width,
                                           orif_diams=orifice_leak_diams, rel_humid=rel_humid,
                                           dis_coeff=discharge_coeff,
                                           rad_src_key=rad_source_model, not_nozzle_key=nozzle_model,
                                           loc_distributions=loc_distributions,
                                           excl_radius=excl_radius, rand_seed=rand_seed,
                                           verbose=False, create_plots=create_plots, chem_file=chem_file,
                                           output_dir=output_dir)

        qrads = flux_dict['fluxes'] * 1000.  # want W from kW
        qrad_plot_files = flux_dict['all_pos_files']
        # Pickle it for later
        flame_data.qrads = qrads
        flame_data.plot_files = qrad_plot_files
        misc_utils.save_object(flame_file, flame_data)

    log.info("QRAD data:\n{}".format(qrads))

    # HARM MODELS
    thermal_fatality_probs = []
    for i, qrad in enumerate(qrads):
        p_therm_fatal = probit.compute_thermal_fatality_prob(probit_thermal_id, qrad, exposure_time)
        thermal_fatality_probs.append(p_therm_fatal)

    thermal_fatality_probs = np.array(thermal_fatality_probs)

    # Convert from 1d to 2d where rows are qrads for single leak size
    thermal_fatality_probs = thermal_fatality_probs.reshape((num_leak_sizes, total_occupants))
    # Probability of jetfire fatality for each leak size
    thermal_fatality_probs_per_leak = np.sum(thermal_fatality_probs, axis=1)  # sum over positions so 1 val / leak size
    log.info("Probit thermal data:\n{}\n".format(thermal_fatality_probs_per_leak))

    # Probability of fatality from overpressure for each leak size
    overp_fatality_probs = []
    for i in range(len(peak_overp_list)):
        p_overp_fatal = probit.compute_overpressure_fatality_prob(probit_rel_id, peak_overp_list[i],
                                                                  impulse=overp_impulse_list[i],
                                                                  fragment_mass=overp_frag_mass,
                                                                  velocity=overp_velocity, total_mass=overp_total_mass)
        overp_fatality_probs.append(p_overp_fatal)
    overp_fatality_probs_per_leak = np.array(overp_fatality_probs)
    log.info("Probit overpressure data:\n{}\n".format(overp_fatality_probs_per_leak))

    if total_occupants > 0:
        # potential loss of life is expected # of fatalities per system year, first computed per leak size
        pll = total_leak_freqs * (
                (prob_jetfire_per_leak * thermal_fatality_probs_per_leak) +
                (total_occupants * prob_explos_per_leak * overp_fatality_probs_per_leak))
        total_pll = np.around(np.nansum(pll), 30)

        # fatal accident rate is expected # of fatalities per 100 million exposed hours
        exposed_hours = 1.e8
        far = total_pll * exposed_hours / (total_occupants * constants.YEARS_TO_HOURS)
        # average individual risk is expected # of fatalities per exposed individual
        air = occupant_avg_hours * far * 1.e-8
    else:
        total_pll = 0.
        far = 0.
        air = 0.

    results = {
        'total_pll': total_pll,
        'far': far,
        'air': air,
        'leak_results': None,  # Will calc these next
        'plot_files': qrad_plot_files,
    }

    # Scenario ranking
    # cpScenarioStats.cs
    for i, leak_size in enumerate(constants.LEAK_SIZES):
        leak_result = leak_results[i]
        leak_release_freq = leak_result.total_release_freq

        # Retrieve event probabilities for this leak size
        prob_jetfire = prob_jetfire_per_leak[i]
        prob_explos = prob_explos_per_leak[i]

        if calc_shutdown:
            prob_shutdown = prob_shutdown_per_leak
            prob_no_ign = prob_no_ign_per_leak[i]
        else:
            prob_shutdown = 0.
            prob_no_ign = 1. - prob_jetfire - prob_explos

        # Compute average events per year for this leak size
        shutdown_avg_events = prob_shutdown * leak_release_freq
        jetfire_avg_events = prob_jetfire * leak_release_freq
        explos_avg_events = prob_explos * leak_release_freq
        no_ign_avg_events = prob_no_ign * leak_release_freq

        if total_pll > 0.:
            # Calculate PLL contributions as % of total PLL
            jetfire_pll_contrib = thermal_fatality_probs_per_leak[i] * jetfire_avg_events / total_pll
            explos_pll_contrib = total_occupants * overp_fatality_probs_per_leak[i] * explos_avg_events / total_pll
        else:
            jetfire_pll_contrib = 0.
            explos_pll_contrib = 0.

        # Store calculation results for display
        leak_result.p_shutdown = np.around(prob_shutdown, 20)
        leak_result.p_jetfire = np.around(prob_jetfire, 20)
        leak_result.p_explos = np.around(prob_explos, 20)
        leak_result.p_no_ign = np.around(prob_no_ign, 20)

        leak_result.shutdown_avg_events = np.around(shutdown_avg_events, 20)
        leak_result.jetfire_avg_events = np.around(jetfire_avg_events, 20)
        leak_result.explos_avg_events = np.around(explos_avg_events, 20)
        leak_result.no_ign_avg_events = np.around(no_ign_avg_events, 20)

        leak_result.jetfire_pll_contrib = np.around(jetfire_pll_contrib, 20)
        leak_result.explos_pll_contrib = np.around(explos_pll_contrib, 20)

        log.info(str(leak_result))

    if print_results:
        print("")
        for leak_res in leak_results:
            print(leak_res)
        print("PLL: {:.5E}".format(total_pll))
        print("FAR: {:.5E}".format(far))
        print("AIR: {:.5E}\n".format(air))

    results['leak_results'] = leak_results

    # Print one result key/val pair per line
    log.info("\nANALYSIS RESULTS:\n{}".format("\n".join(["{}: {}".format(key, val) for key, val in results.items()])))
    log.info("=== ANALYSIS COMPLETE ===")

    return results
