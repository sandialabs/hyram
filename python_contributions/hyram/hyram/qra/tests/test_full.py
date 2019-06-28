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

import unittest
import numpy as np

from ...qra import api
from python_contributions.qra.utilities.constants import IN_TO_M


reltol = 0.03


class TestDefaultInputs(unittest.TestCase):

    def setUp(self):
        ignition_prob_data = [
            {'threshold_min': None, 'threshold_max': 0.125, 'immed_prob': 0.008, 'delay_prob': 0.004},
            {'threshold_min': 0.125, 'threshold_max': 6.25, 'immed_prob': 0.053, 'delay_prob': 0.027},
            {'threshold_min': 6.25, 'threshold_max': None, 'immed_prob': 0.230, 'delay_prob': 0.120},
        ]

        self.input_dict = {
            'pipe_length': 20,
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
            'num_filters': 0,
            'num_flanges': 0,
            'num_extra_comp1': 0,
            'num_extra_comp2': 0,
            'probit_thermal_model_id': 'eis',
            'thermal_exposure_time': 60,

            'probit_overp_model_id': 'elh',  # elh, lhs, hea, col, deb

            'rad_source_model': 'multi',
            'rel_humid': 0.89,

            'notional_nozzle_model': 'YuceilOtugen',

            'rand_seed': 3632850,
            'excl_radius': 0.01,
        }

    def tearDown(self):
        self.input_dict = None

    def test_all_results(self):
        """ Test full analysis with current C# defaults """
        known_pll = 1.649e-5
        known_far = 2.09e-02
        known_air = 4.1184e-07
        results = api.analysis_verbose(self.input_dict)
        np.testing.assert_allclose(results['total_pll'], known_pll, rtol=reltol)
        np.testing.assert_allclose(results['far'], known_far, rtol=reltol)
        np.testing.assert_allclose(results['air'], known_air, rtol=reltol)
        # Check cut set data
        scen_stats = results['scenario_stats']
        atol = 0.0003
        np.testing.assert_allclose(scen_stats[0.01]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['p_no_ignition'], 0.0348, atol=atol)

        np.testing.assert_allclose(scen_stats[0.1]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['p_no_ignition'], 0.005, atol=atol)

        np.testing.assert_allclose(scen_stats[1.]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['p_no_ignition'], 0.0015, atol=atol)

        np.testing.assert_allclose(scen_stats[10.]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['p_no_ignition'], 0.0012, atol=atol)

        # np.testing.assert_allclose(scen_stats[100.]['jetf_exp_fatals'], 0., atol=atol)  # TODO: check why
        np.testing.assert_allclose(scen_stats[100.]['jetf_pll_contrib'], 1., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['p_no_ignition'], 0.0008, atol=atol)


class TestOneOfEachComponent(unittest.TestCase):

    def setUp(self):
        ignition_prob_data = [
            {'threshold_min': None, 'threshold_max': 0.125, 'immed_prob': 0.008, 'delay_prob': 0.004},
            {'threshold_min': 0.125, 'threshold_max': 6.25, 'immed_prob': 0.053, 'delay_prob': 0.027},
            {'threshold_min': 6.25, 'threshold_max': None, 'immed_prob': 0.230, 'delay_prob': 0.120},
        ]

        self.input_dict = {
            'pipe_length': 20,
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
            'num_compressors': 1,
            'num_cylinders': 1,
            'num_valves': 1,
            'num_instruments': 1,
            'num_joints': 1,
            'num_hoses': 1,
            'num_filters': 1,
            'num_flanges': 1,
            'num_extra_comp1': 0,
            'num_extra_comp2': 0,

            'probit_thermal_model_id': 'eis',
            'thermal_exposure_time': 60,

            'probit_overp_model_id': 'elh',  # elh, lhs, hea, col, deb

            'rad_source_model': 'multi',
            'rel_humid': 0.89,

            'notional_nozzle_model': 'YuceilOtugen',

            'rand_seed': 3632850,
            'excl_radius': 0.01,
        }

    def tearDown(self):
        self.input_dict = None

    def test_all_results_for_single_components(self):
        """ Test full analysis with current C# defaults """
        known_pll = 1.558e-04
        known_far = 0.1976
        known_air = 3.951e-06
        results = api.analysis_verbose(self.input_dict)
        np.testing.assert_allclose(results['total_pll'], known_pll, rtol=reltol)
        np.testing.assert_allclose(results['far'], known_far, rtol=reltol)
        np.testing.assert_allclose(results['air'], known_air, rtol=reltol)
        # Check cut set data
        scen_stats = results['scenario_stats']
        atol = 0.0003
        np.testing.assert_allclose(scen_stats[0.01]['jetf_exp_fatals'], 0.0002, atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['expl_exp_fatals'], 0.0001, atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['p_no_ignition'], 0.3071, atol=atol)

        np.testing.assert_allclose(scen_stats[0.1]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['p_no_ignition'], 0.0444, atol=atol)

        np.testing.assert_allclose(scen_stats[1.]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['p_no_ignition'], 0.0256, atol=atol)

        np.testing.assert_allclose(scen_stats[10.]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['p_no_ignition'], 0.0075, atol=atol)

        # np.testing.assert_allclose(scen_stats[100.]['jetf_exp_fatals'], 0., atol=atol)  # TODO: check why
        np.testing.assert_allclose(scen_stats[100.]['jetf_pll_contrib'], 1., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['p_no_ignition'], 0.0072, atol=atol)


class TestHarmModelsWithOtherDefaults(unittest.TestCase):

    def setUp(self):
        ignition_prob_data = [
            {'threshold_min': None, 'threshold_max': 0.125, 'immed_prob': 0.008, 'delay_prob': 0.004},
            {'threshold_min': 0.125, 'threshold_max': 6.25, 'immed_prob': 0.053, 'delay_prob': 0.027},
            {'threshold_min': 6.25, 'threshold_max': None, 'immed_prob': 0.230, 'delay_prob': 0.120},
        ]

        self.input_dict = {
            'pipe_length': 20,
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
            'num_filters': 0,
            'num_flanges': 0,
            'num_extra_comp1': 0,
            'num_extra_comp2': 0,
            'probit_thermal_model_id': 'eis',
            'thermal_exposure_time': 60,

            'probit_overp_model_id': 'elh',  # elh, lhs, hea, col, deb

            'rad_source_model': 'multi',
            'rel_humid': 0.89,

            'notional_nozzle_model': 'YuceilOtugen',

            'rand_seed': 3632850,
            'excl_radius': 0.01,
        }

    def tearDown(self):
        self.input_dict = None

    def test_thermal_tsao_overp_eis(self):
        """ Test full analysis with current C# defaults """
        self.input_dict['probit_thermal_model_id'] = 'tsa'
        self.input_dict['probit_overp_model_id'] = 'elh'

        known_pll = 1.956-4
        known_far = 0.2481
        known_air = 4.962e-6
        results = api.analysis_verbose(self.input_dict)

        np.testing.assert_allclose(results['total_pll'], known_pll, rtol=reltol)
        np.testing.assert_allclose(results['far'], known_far, rtol=reltol)
        np.testing.assert_allclose(results['air'], known_air, rtol=reltol)
        # Check cut set data
        scen_stats = results['scenario_stats']
        atol = 0.0003
        np.testing.assert_allclose(scen_stats[0.01]['jetf_exp_fatals'], 0.0002, atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['expl_exp_fatals'], 0.0001, atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.01]['p_no_ignition'], 0.3071, atol=atol)

        np.testing.assert_allclose(scen_stats[0.1]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[0.1]['p_no_ignition'], 0.0444, atol=atol)

        np.testing.assert_allclose(scen_stats[1.]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[1.]['p_no_ignition'], 0.0256, atol=atol)

        np.testing.assert_allclose(scen_stats[10.]['jetf_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['jetf_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[10.]['p_no_ignition'], 0.0075, atol=atol)

        # np.testing.assert_allclose(scen_stats[100.]['jetf_exp_fatals'], 0., atol=atol)  # TODO: check why
        np.testing.assert_allclose(scen_stats[100.]['jetf_pll_contrib'], 1., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['expl_exp_fatals'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['expl_pll_contrib'], 0., atol=atol)
        np.testing.assert_allclose(scen_stats[100.]['p_no_ignition'], 0.0072, atol=atol)

