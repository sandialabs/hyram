"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest
import json

import numpy as np

from hyram.phys import Fluid, Orifice, Jet

from . import utils


# Flags to enable command line output, pyplots, and text file output
VERBOSE = False
CREATE_PLOTS = False
CREATE_OUTPUT = False

# Absolute paths to input/output data
DATA_LOC = os.path.join(os.path.dirname(__file__), 'data')
LIMITS_FILE = os.path.join(DATA_LOC, 'Limits-cryo_concentrations.json')
OUTPUT_LOC = os.path.join('out', 'validation-cryo_concentrations')


class Test_Xiao_2010(unittest.TestCase):
    """
    Tests for table 5-3 from Molkov
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'xiao-data.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        case1 = {'P': 1.7e6, 'd': 2.0e-3, 'T': 298}
        case2 = {'P': 6.85e6, 'd': 1.0e-3, 'T': 298}
        case3 = {'P': 0.825e6, 'd': 2.0e-3, 'T': 80}
        case4 = {'P': 3.2e6, 'd': 1.0e-3,'T': 80}

        self.output_data = []
        for idx, case in enumerate([case1, case2, case3, case4]):
            fluid = Fluid(T=case['T'], P=case['P'], species='h2')
            orifice = Orifice(case['d'])
            air = Fluid(T=295, P=101325, species='air')
            jet = Jet(fluid=fluid, orifice=orifice, ambient=air, Smax=6)

            calc_distance = jet.S / jet.developing_flow.d0
            calc_concentration = 1 / jet.X_cl

            case_num = str(idx + 1)
            exp_distance = data['case' + case_num + '_x']
            exp_concentration = data['case' + case_num + '_y']

            trimmed_data = utils.interp_and_trim_data(calculated_x=calc_distance, calculated_y=calc_concentration,
                                                      experiment_x=exp_distance, experiment_y=exp_concentration)

            self.output_data.append(trimmed_data)

    def test_concentration_1(self):
        case = self.output_data[0]
        title = "Xiao et al. 2010 - Figure 3, Inverse Mass Fraction over Normalized Distance, Case 1"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                            x=case['calc_x'],
                            exp_y=case['exp_y'],
                            calc_y=case['calc_y'],
                            max_error=error['Max Absolute Error'],
                            avg_error=error['Avg Absolute Error'],
                            smape=error['SMAPE'],
                            title=title,
                            xlabel=r'Normalized Distance ($S/D_0$)',
                            ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_2(self):
        case = self.output_data[1]
        title = "Xiao et al. 2010 - Figure 3, Inverse Mass Fraction over Normalized Distance, Case 2"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                            x=case['calc_x'],
                            exp_y=case['exp_y'],
                            calc_y=case['calc_y'],
                            max_error=error['Max Absolute Error'],
                            avg_error=error['Avg Absolute Error'],
                            smape=error['SMAPE'],
                            title=title,
                            xlabel=r'Normalized Distance ($S/D_0$)',
                            ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_3(self):
        case = self.output_data[2]
        title = "Xiao et al. 2010 - Figure 3, Inverse Mass Fraction over Normalized Distance, Case 3"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                            x=case['calc_x'],
                            exp_y=case['exp_y'],
                            calc_y=case['calc_y'],
                            max_error=error['Max Absolute Error'],
                            avg_error=error['Avg Absolute Error'],
                            smape=error['SMAPE'],
                            title=title,
                            xlabel=r'Normalized Distance ($S/D_0$)',
                            ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_4(self):
        case = self.output_data[3]
        title = "Xiao et al. 2010 - Figure 3, Inverse Mass Fraction over Normalized Distance, Case 4"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                            x=case['calc_x'],
                            exp_y=case['exp_y'],
                            calc_y=case['calc_y'],
                            max_error=error['Max Absolute Error'],
                            avg_error=error['Avg Absolute Error'],
                            smape=error['SMAPE'],
                            title=title,
                            xlabel=r'Normalized Distance ($S/D_0$)',
                            ylabel='Inverse Centerline Mole Fraction')


class Test_Friedrich_Dist1(unittest.TestCase):
    """
    Tests for Inverse Centerline Mole Fraction vs. Normalized Distance
    'Normalized Distance' here is defined as z/d * sqrt(rho(ambient)/rho(initial))
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'friedrich-fig5.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        experiments = ['3000', '3002', '3004', '3005', '5001', '5002']
        parameters = [{'P':19, 'T':37, 'd':1},
                       {'P':15, 'T':38, 'd':1},
                       {'P':29, 'T':36, 'd':1},
                       {'P':18, 'T':36, 'd':1},
                       {'P':29.85, 'T':43.59, 'd':0.5},
                       {'P':29.85, 'T':43.59, 'd':0.5}]

        self.output_data = []
        for idx, params in enumerate(parameters):
            fluid = Fluid(T=params['T'], P=params['P']*1e5, species='h2')
            orifice = Orifice(params['d']*1e-3)
            air = Fluid(T=295, P=101325, species='air')
            jet = Jet(fluid=fluid, orifice=orifice, ambient=air)

            calc_distance = jet.S / jet.developing_flow.d0 * (np.sqrt(jet.ambient.rho / jet.fluid.rho))
            calc_concentration = 1 / jet.X_cl

            case_num = experiments[idx]
            exp_distance = data['x_' + case_num]
            exp_concentration = np.array(data['y_' + case_num]) * 100

            trimmed_data = utils.interp_and_trim_data(calculated_x=calc_distance, calculated_y=calc_concentration,
                                                      experiment_x=exp_distance, experiment_y=exp_concentration)

            self.output_data.append(trimmed_data)

    def test_concentration_exp1(self):
        case = self.output_data[0]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 1, Experiment #3000"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp2(self):
        case = self.output_data[1]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 1, Experiment #3002"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp3(self):
        case = self.output_data[2]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 1, Experiment #3004"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp4(self):
        case = self.output_data[3]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 1, Experiment #3005"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp5(self):
        case = self.output_data[4]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 1, Experiment #5001"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp6(self):
        case = self.output_data[5]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 1, Experiment #5002"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

class Test_Friedrich_Dist2(unittest.TestCase):
    """
    Tests for Inverse Centerline Mole Fraction vs. Normalized Distance
    'Normalized Distance' here is defined as z/d * sqrt(rho(ambient)/rho(nozzle))
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'friedrich-fig5.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        experiments = ['3000', '3002', '3004', '3005', '5001', '5002']
        parameters = [{'P':19, 'T':37, 'd':1},
                       {'P':15, 'T':38, 'd':1},
                       {'P':29, 'T':36, 'd':1},
                       {'P':18, 'T':36, 'd':1},
                       {'P':29.85, 'T':43.59, 'd':0.5},
                       {'P':29.85, 'T':43.59, 'd':0.5}]

        self.output_data = []
        for idx, params in enumerate(parameters):
            fluid = Fluid(T=params['T'], P=params['P']*1e5, species='H2')
            orifice = Orifice(params['d']*1e-3)
            air = Fluid(T=295, P=101325, species='air')
            jet = Jet(fluid=fluid, orifice=orifice, ambient=air)

            calc_distance = jet.S / jet.developing_flow.d0 * (np.sqrt(jet.ambient.rho / jet.developing_flow.fluid_orifice.rho))
            calc_concentration = 1 / jet.X_cl

            case_num = experiments[idx]
            exp_distance = data['x_' + case_num]
            exp_concentration = np.array(data['y_' + case_num]) * 100

            trimmed_data = utils.interp_and_trim_data(calculated_x=calc_distance, calculated_y=calc_concentration,
                                                      experiment_x=exp_distance, experiment_y=exp_concentration)

            self.output_data.append(trimmed_data)

    def test_concentration_exp1(self):
        case = self.output_data[0]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 2, Experiment #3000"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp2(self):
        case = self.output_data[1]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 2, Experiment #3002"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp3(self):
        case = self.output_data[2]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 2, Experiment #3004"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp4(self):
        case = self.output_data[3]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 2, Experiment #3005"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp5(self):
        case = self.output_data[4]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 2, Experiment #5001"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')

    def test_concentration_exp6(self):
        case = self.output_data[5]
        title = "Friedrich - Figure 5, Inverse Centerline Mole Fraction over Normalized Distance 2, Experiment #5002"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=case['exp_y'],
                                calc_vals=case['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Maximum Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Absolute Error'], error_limits['Avg Absolute Error'], f"{title}: Average Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Maximum Percent Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Avg Percent Error'], error_limits['Avg Percent Error'], f"{title}: Average Percent Error out of range")
        with self.subTest():
            self.assertGreaterEqual(error['R2'], error_limits['R2'], f"{title}: R Squared Value out of range")

        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=case['calc_x'],
                                exp_y=case['exp_y'],
                                calc_y=case['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance $\left(\frac{z}{d}\sqrt{\frac{\rho_{\rm amb}}{\rho_{0}}}\right)$',
                                ylabel='Inverse Centerline Mole Fraction')
