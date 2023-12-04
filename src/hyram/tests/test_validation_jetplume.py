"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest
import json

import numpy as np
from scipy.constants import milli, kilo, liter, bar, pi, minute

from hyram.phys import Fluid, Orifice, Jet
from hyram.utilities import misc_utils

import utils

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

# Flags to enable command line output, pyplots, and text file output
VERBOSE = False
CREATE_PLOTS = False
CREATE_OUTPUT = False

# Absolute paths to input/output data
DATA_LOC = os.path.join(os.path.dirname(__file__), 'data')
OUTPUT_LOC = os.path.join(misc_utils.get_temp_folder(), 'validation-jetplume')
OUTPUT_FILE = os.path.join(OUTPUT_LOC, 'Validation.txt')
LIMITS_FILE = os.path.join(DATA_LOC, 'Limits-jetplume.json')


class Test_Molkov_2012(unittest.TestCase):
    """
    Tests for table 5-3 from Molkov
    """
    def setUp(self):
        csv_data = utils.read_csv(os.path.join(DATA_LOC, 'Molkov2012BookData.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters from data
        self.jets = []
        self.data = dict()
        air = Fluid(T = 298, P = 101325, species = 'air')
        for idx in range(0, len(csv_data['T']), 2):
            try:
                gas = Fluid(species='h2', T=csv_data['T'][idx], P=csv_data['P'][idx]*1e6)
                orifice = Orifice(d=csv_data['D'][idx]*1e-3)
                self.jets.append(Jet(gas, orifice, air, lam=1.16, theta0=pi/2))

                # Datapoints come in pairs, add to self.data dict
                for key in csv_data:
                    if key in self.data:
                        self.data[key].append(csv_data[key][idx])
                        self.data[key].append(csv_data[key][idx+1])

                    else:
                        self.data[key] = list()
                        self.data[key].append(csv_data[key][idx])
                        self.data[key].append(csv_data[key][idx+1])
            except:
                pass

    def test_concentration(self):
        trial_num = 0
        prev_title = ''
        for idx in range(0, len(self.data['T']), 2):
            jet_idx = int(idx/2)
            x1 = self.data['x'][idx] / (self.data['D'][idx] * np.sqrt(self.data['rho_n'][idx] / self.jets[jet_idx].ambient.rho * 1e-6))
            x2 = self.data['x'][idx+1] / (self.data['D'][idx+1] * np.sqrt(self.data['rho_n'][idx+1] / self.jets[jet_idx].ambient.rho * 1e-6))
            exp_x = [x1, x2]

            c1 = self.data['Y'][idx]
            c2 = self.data['Y'][idx+1]
            exp_concentration = [c1, c2]

            calc_x = self.jets[jet_idx].S / (self.data['D'][idx] * 1e-3 * np.sqrt(self.jets[jet_idx].developing_flow.fluid_orifice.rho / self.jets[jet_idx].ambient.rho))
            calc_concentration = self.jets[jet_idx].Y_cl

            final_data = utils.interp_and_trim_data(calculated_x=calc_x, calculated_y=calc_concentration,
                                                    experiment_x=exp_x, experiment_y=exp_concentration)

            title = self.data['Author-Year'][idx]
            if title != prev_title:
                prev_title = title
                trial_num = 1
            else:
                trial_num += 1

            title = "Molkov 2012, Table 5.3 - " + str(title) + ', Sample ' + str(trial_num)
            error_limits = self.limits[title]
            error = utils.get_error(exp_vals=final_data['exp_y'],
                                    calc_vals=final_data['calc_y'],
                                    error_limits=error_limits,
                                    units='',
                                    msg=title,
                                    output_filename=OUTPUT_FILE,
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
                                x=final_data['calc_x'],
                                exp_y=final_data['exp_y'],
                                calc_y=final_data['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel=r'Normalized Distance ($\frac{x}{d\sqrt{\rho_{\rm throat}/\rho_{\rm amb}}}$)',
                                ylabel='Centerline Mass Fraction')


class Test_HoufSchefer_2008_fig5(unittest.TestCase):
    """
    Tests for mass fraction curve at various distances, data from Houf and Schefer 2008 Figure 5
    """

    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'houfschefer-2008-fig5.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        diameter = 1.905 * milli #m
        density = 0.0838 #kg/m^3
        flowrate = 22.9 * liter/minute
        amb_temp = 294 #K
        amb_pressure = 100 * kilo #Pa
        axial_dists = [10, 25, 50, 75, 100] #Z/D, axial distances from the flame

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')
        gas = Fluid(T=amb_temp, P=amb_pressure, species='H2')
        orifice = Orifice(d=diameter)
        jet = Jet(gas, orifice, air, mdot=density*flowrate, theta0=pi/2)

        # Build dicts of calculated and experimental values
        self.output_data = []
        for zdist in axial_dists:
            profile = jet._radial_profile(zdist * orifice.d, ind_var='Y', nB=20)

            distance = profile[0] * 1000
            mass_fraction = profile[1]

            exp_distance = data[f'z{zdist}x']
            exp_mass_fraction = data[f'z{zdist}y']

            # Trim to [-20, 20]
            final_data = utils.interp_and_trim_data(calculated_x=distance, calculated_y=mass_fraction,
                                                    experiment_x=exp_distance, experiment_y=exp_mass_fraction,
                                                    lower_bound=-20, upper_bound=20)

            entry = [final_data['calc_x'], final_data['calc_y'], final_data['exp_y']]
            self.output_data.append(entry)

    def test_zdists(self):
        # Note: 'zdist' is the normalized axial distance Z/D, no units
        zdists = ['10', '25', '50', '75', '100']

        for idx, data in enumerate(self.output_data):
            zdist = zdists[idx]
            title = "Houf and Schefer 2008 - Figure 5, Mass Fraction over Radial Distance, zdist = " + zdist
            calc_distance = data[0]
            calc_mass_fraction = data[1]
            exp_mass_fraction = data[2]

            error_limits = self.limits[title]
            error = utils.get_error(exp_vals=exp_mass_fraction,
                                    calc_vals=calc_mass_fraction,
                                    error_limits=error_limits,
                                    units='',
                                    msg=title,
                                    output_filename=OUTPUT_FILE,
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
                                x=calc_distance,
                                exp_y=exp_mass_fraction,
                                calc_y=calc_mass_fraction,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Radial Distance (mm)',
                                ylabel='Mass Fraction')


class Test_HoufSchefer_2008_fig8(unittest.TestCase):
    """
    Single test for inverse centerline mole fraction curve over axial distance, data from Houf and Schefer 2008 Figure 8
    """

    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'houfschefer-2008-fig8.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        diameter = 1.905 * milli #m
        density = 0.0838 #kg/m^3
        flowrate = 22.9 * liter/minute
        amb_temp = 294 #K
        amb_pressure = 100 * kilo #Pa

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')
        gas = Fluid(T=amb_temp, P=amb_pressure, species='H2')
        orifice = Orifice(d=diameter)
        jet = Jet(gas, orifice, air, mdot=density*flowrate, theta0=pi/2)

        # Calculated values
        calc_axial_distance = jet.S / orifice.d
        calc_inverse_mole_fraction = 1 / jet.X_cl

        # Experimental values
        exp_axial_distance = data['dp_x']
        exp_inverse_mole_fraction = data['dp_y']

        # Trim to [0, 80]
        self.final_data = utils.interp_and_trim_data(calculated_x=calc_axial_distance, calculated_y=calc_inverse_mole_fraction,
                                                     experiment_x=exp_axial_distance, experiment_y=exp_inverse_mole_fraction,
                                                     lower_bound=0, upper_bound=80)

    def test_inverse_mole_fraction(self):
        title = "Houf and Schefer 2008 - Figure 8, Inverse Mole Fraction over Axial Distance"
        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.final_data['exp_y'],
                                calc_vals=self.final_data['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_filename=OUTPUT_FILE,
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
                               x=self.final_data['calc_x'],
                               exp_y=self.final_data['exp_y'],
                               calc_y=self.final_data['calc_y'],
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Radial Distance (mm)',
                               ylabel='Mass Fraction')


class Test_HoufSchefer_2008_fig9(unittest.TestCase):
    """
    Tests for inverse mass fraction curve at various axial distances, data from Houf and Schefer 2008 Figure 9
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'houfschefer-2008-fig9.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        diameter = 1.905 * milli #m
        density = 0.0838 #kg/m^3
        flowrate = 22.9 * liter/minute
        amb_temp = 294 #K
        amb_pressure = 100 * kilo #Pa

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')
        gas = Fluid(T=amb_temp, P=amb_pressure, species='H2')
        orifice = Orifice(d=diameter)

        flowrates = np.array([8.497, 13.08, 22.9]) * liter/minute # volumetric flow rates for Fr = 41, 99, 152, 268
        colors = ['blu', 'red', 'bla']

        self.output_data = []
        for idx, flowrate in enumerate(flowrates):
            jet = Jet(gas, orifice, air, mdot=density*flowrate, theta0=pi/2)
            calc_axial_distance = jet.S / orifice.d
            calc_mole_fraction = 1 / jet.X_cl

            exp_axial_distance = data[f'{colors[idx]}_x']
            exp_mole_fraction = data[f'{colors[idx]}_y']

            trimmed_data = utils.interp_and_trim_data(calculated_x=calc_axial_distance, calculated_y=calc_mole_fraction,
                                                    experiment_x=exp_axial_distance, experiment_y=exp_mole_fraction, upper_bound=100)

            self.output_data.append(trimmed_data)

    def test_inverse_mole_fraction(self):
        froud_nums = ['99', '152', '268']
        for idx, data in enumerate(self.output_data):
            title = "Houf and Schefer 2008 - Figure 9, Inverse Mass Fraction over Axial Distance, Fr = " + froud_nums[idx]

            error_limits = self.limits[title]
            error = utils.get_error(exp_vals=data['exp_y'],
                                    calc_vals=data['calc_y'],
                                    error_limits=error_limits,
                                    units='',
                                    msg=title,
                                    output_filename=OUTPUT_FILE,
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
                                   x=data['calc_x'],
                                   exp_y=data['exp_y'],
                                   calc_y=data['calc_y'],
                                   max_error=error['Max Absolute Error'],
                                   avg_error=error['Avg Absolute Error'],
                                   smape=error['SMAPE'],
                                   title=title,
                                   xlabel='Normalized Axial Distance (z/D)',
                                   ylabel='Inverse Centerline Mole Fraction')


class Test_RugglesEkoto_2012(unittest.TestCase):
    """
    Tests for inverse mass fraction curve and jet (half) width at various axial distances, data from Ruggles and Ekoto 2012
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'rugglesekoto-2012-fig7.csv'), header=2)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        diameter = 0.75*2 *milli#meters
        temp = 295.4 #K
        pressure = 983.2 *kilo#Pa
        discharge_coeff = 0.979
        amb_temp = 296 #K
        amb_pressure = 98.37 *kilo#Pa

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')
        gas = Fluid(T=temp, P=pressure, species='h2')
        orifice = Orifice(d=diameter, Cd=discharge_coeff)
        jet = Jet(gas, orifice, air, theta0=pi/2)

        calc_axial_distance = jet.y / (orifice.d / 2)
        calc_mass_fraction = 1 / jet.Y_cl
        calc_jet_width = jet.B * jet.lam * np.sqrt(-np.log(.5)) / milli

        exp_axial_distance_1 = data['x1']
        exp_mass_fraction = data['y1']

        exp_axial_distance_2 = data['x3']
        exp_jet_width = data['y3']

        self.mass_fraction_data = utils.interp_and_trim_data(calculated_x=calc_axial_distance, calculated_y=calc_mass_fraction,
                                                            experiment_x=exp_axial_distance_1, experiment_y=exp_mass_fraction,
                                                            upper_bound=300)

        self.jet_width_data = utils.interp_and_trim_data(calculated_x=calc_axial_distance, calculated_y=calc_jet_width,
                                                            experiment_x=exp_axial_distance_2, experiment_y=exp_jet_width,
                                                            upper_bound=300)

    def test_inverse_mass_fraction(self):
        title = "Ruggles and Ekoto 2012 - Figure 7, Inverse Mass Fraction over Axial Distance"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.mass_fraction_data['exp_y'],
                                calc_vals=self.mass_fraction_data['calc_y'],
                                error_limits=error_limits,
                                units='',
                                msg=title,
                                output_filename=OUTPUT_FILE,
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
                            x=self.mass_fraction_data['calc_x'],
                            exp_y=self.mass_fraction_data['exp_y'],
                            calc_y=self.mass_fraction_data['calc_y'],
                            max_error=error['Max Absolute Error'],
                            avg_error=error['Avg Absolute Error'],
                            smape=error['SMAPE'],
                            title=title,
                            xlabel='Normalized Axial Distance (z/r)',
                            ylabel='Inverse Centerline Mass Fraction')

    def test_jet_width(self):
        title = "Ruggles and Ekoto 2012 - Figure 7, Jet Width over Axial Distance"

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.jet_width_data['exp_y'],
                                calc_vals=self.jet_width_data['calc_y'],
                                error_limits=error_limits,
                                units='mm',
                                msg=title,
                                output_filename=OUTPUT_FILE,
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
                            x=self.jet_width_data['calc_x'],
                            exp_y=self.jet_width_data['exp_y'],
                            calc_y=self.jet_width_data['calc_y'],
                            max_error=error['Max Absolute Error'],
                            avg_error=error['Avg Absolute Error'],
                            smape=error['SMAPE'],
                            title=title,
                            xlabel='Normalized Axial Distance (z/r)',
                            ylabel='Jet Half Width (mm)')


class Test_Han_2013_fig3(unittest.TestCase):
    """
    Tests for centerline concentration over distance at various pressures, data from Han et al. 2013
    """
    def setUp(self):
        data_a = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig3a.csv'), header=2)
        data_b = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig3b.csv'), header=2)
        data_c = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig3c.csv'), header=2)
        data_d = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig3d.csv'), header=2)

        csv_data = [data_a, data_b, data_c, data_d]

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        temp = 293 #K
        amb_temp = 293 #K
        amb_pressure = 101325 #Pa

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')

        self.diameters = np.array([0.5, 0.7, 1]) * milli #m
        self.pressures = np.arange(100, 401, 100) * bar

        self.output_data = dict()
        for file, pressure in enumerate(self.pressures):
            gas = Fluid(T=temp, P=pressure, species='h2')
            self.output_data[pressure] = dict()
            for idx, diameter in enumerate(self.diameters):
                orifice = Orifice(d=diameter)
                jet = Jet(gas, orifice, air, theta0=0, Ymin=1e-4)

                calc_distance = jet.x
                calc_concentration = jet.X_cl

                # Data is in x1/y1, x3/y3, x5/y5
                exp_distance = csv_data[file][f'x{idx*2+1}']
                exp_concentration = csv_data[file][f'y{idx*2+1}']

                self.output_data[pressure][diameter] = utils.interp_and_trim_data(calculated_x=calc_distance, calculated_y=calc_concentration,
                                                            experiment_x=exp_distance, experiment_y=exp_concentration,
                                                            lower_bound=0.5, upper_bound=10)

    def test_centerline_concentration(self):
        pressure_list = ['100', '200', '300', '400']
        diameter_list = ['0.5', '0.7', '1.0']
        for p, pressure in enumerate(self.pressures):
            for d, diameter in enumerate(self.diameters):
                title = "Han et al. 2013 - Figure 3, Centerline Mass Fraction vs. Distance, P0 = " + pressure_list[p] + " bar, d = " + diameter_list[d] + "mm"

                data = self.output_data[pressure][diameter]
                error_limits = self.limits[title]
                error = utils.get_error(exp_vals=data['exp_y'],
                                        calc_vals=data['calc_y'],
                                        error_limits=error_limits,
                                        units='',
                                        msg=title,
                                        output_filename=OUTPUT_FILE,
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
                                    x=data['calc_x'],
                                    exp_y=data['exp_y'],
                                    calc_y=data['calc_y'],
                                    max_error=error['Max Absolute Error'],
                                    avg_error=error['Avg Absolute Error'],
                                    smape=error['SMAPE'],
                                    title=title,
                                    xlabel='Normalized Axial Distance (z/r)',
                                    ylabel='Inverse Centerline Mass Fraction')


class Test_Han_2013_fig6(unittest.TestCase):
    """
    Tests for dilution length over pressure with various orifice diameters, data from Han et al. 2013
    """
    def setUp(self):
        data_a = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig6a.csv'), header=2)
        data_b = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig6b.csv'), header=2)
        data_c = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig6c.csv'), header=2)

        csv_data = [data_a, data_b, data_c]

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        temp = 293 #K
        amb_temp = 293 #K
        amb_pressure = 101325 #Pa

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')

        self.diameters = np.array([0.5, 0.7, 1]) * milli #m
        pressures = np.arange(100, 401, 100) * bar

        # Build 2-level dict of jets based on pressure and orifice diameter
        jets = dict()
        for file, pressure in enumerate(pressures):
            gas = Fluid(T=temp, P=pressure, species='h2')
            jets[pressure] = dict()
            for idx, diameter in enumerate(self.diameters):
                orifice = Orifice(d=diameter)
                jets[pressure][diameter] = Jet(gas, orifice, air, theta0=0, Ymin=1e-4)

        lean_flamm_limit = 0.04
        self.lfl_fractions = [lean_flamm_limit, lean_flamm_limit/2, lean_flamm_limit/4]

        self.output_data = dict()
        for file, diameter in enumerate(self.diameters):
            self.output_data[diameter] = dict()
            for idx, fraction in enumerate(self.lfl_fractions):
                calc_pressure = pressures / bar
                calc_dilution_length = [np.interp(fraction, jets[pressure][diameter].X_cl[::-1], jets[pressure][diameter].S[::-1]) for pressure in pressures]

                # Data is in x2/y2, x4/y4, x6/y6
                exp_pressure = csv_data[file][f'x{(idx+1)*2}']
                exp_dilution_length = csv_data[file][f'y{(idx+1)*2}']

                self.output_data[diameter][fraction] = utils.interp_and_trim_data(calculated_x=calc_pressure, calculated_y=calc_dilution_length,
                                                                                  experiment_x=exp_pressure, experiment_y=exp_dilution_length)

    def test_dilution_length(self):
        fractions = ['', '_2', '_4']
        for diameter in self.diameters:
            for f, fraction in enumerate(self.lfl_fractions):
                title = "Han et al. 2013 - Figure 6, Dilution Length vs. Pressure, d = " + str(diameter / milli) + "mm, LFL" + fractions[f]

                data = self.output_data[diameter][fraction]
                error_limits = self.limits[title]
                error = utils.get_error(exp_vals=data['exp_y'],
                                        calc_vals=data['calc_y'],
                                        error_limits=error_limits,
                                        units='m',
                                        msg=title,
                                        output_filename=OUTPUT_FILE,
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
                                    x=data['calc_x'],
                                    exp_y=data['exp_y'],
                                    calc_y=data['calc_y'],
                                    max_error=error['Max Absolute Error'],
                                    avg_error=error['Avg Absolute Error'],
                                    smape=error['SMAPE'],
                                    title=title,
                                    xlabel='Pressure (bar)',
                                    ylabel='Dilution Length (m)')


class Test_Han_2013_fig7(unittest.TestCase):
    """
    Tests for concentration over normalized axial distance with various orifice diameters and constant pressure, data from Han et al. 2013
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig7.csv'), header=2)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        temp = 293 #K
        pressure = 300 * bar
        amb_temp = 293 #K
        amb_pressure = 101325 #Pa

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')

        self.diameters = np.array([0.5, 0.7, 1]) * milli #m

        self.output_data = dict()
        gas = Fluid(T=temp, P=pressure, species='h2')
        for idx, diameter in enumerate(self.diameters):
            orifice = Orifice(d=diameter)
            jet = Jet(gas, orifice, air, theta0=0, Ymin=1e-4)

            calc_distance = jet.S / diameter
            calc_concentration = 1 / jet.X_cl

            # Data is in x1/y1, x4/y4, x7/y7
            exp_distance = data[f'x{idx*3+1}']
            exp_concentration = data[f'y{idx*3+1}']

            self.output_data[diameter] = utils.interp_and_trim_data(calculated_x=calc_distance, calculated_y=calc_concentration,
                                                                    experiment_x=exp_distance, experiment_y=exp_concentration,
                                                                    upper_bound=15000)

    def test_centerline_concentration(self):
        for diameter in self.diameters:
            title = 'Han et al. 2013 - Figure 7, Inverse Mole Fraction vs. Axial Distance, d = ' + str(diameter / milli) + 'mm'

            data = self.output_data[diameter]
            error_limits = self.limits[title]
            error = utils.get_error(exp_vals=data['exp_y'],
                                    calc_vals=data['calc_y'],
                                    error_limits=error_limits,
                                    units='',
                                    msg=title,
                                    output_filename=OUTPUT_FILE,
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
                                x=data['calc_x'],
                                exp_y=data['exp_y'],
                                calc_y=data['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Normalized Axial Distance (z/D)',
                                ylabel='Inverse Mole Fraction')


class Test_Han_2013_fig8(unittest.TestCase):
    """
    Tests for concentration over normalized axial distance with constant orifice diameter and various pressures, data from Han et al. 2013
    """
    def setUp(self):
        data_a = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig8a.csv'), header=2)
        data_b = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig8b.csv'), header=2)
        data_c = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig8c.csv'), header=2)
        data_d = utils.read_csv(os.path.join(DATA_LOC, 'han-2013-fig8d.csv'), header=2)

        csv_data = [data_a, data_b, data_c, data_d]

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set parameters
        temp = 293 #K
        pressure = 300 * bar
        amb_temp = 293 #K
        amb_pressure = 101325 #Pa

        air = Fluid(T=amb_temp, P=amb_pressure, species='air')

        diameter = 0.7 * milli
        self.pressures = np.arange(100, 401, 100) * bar

        self.output_data = dict()
        for idx, pressure in enumerate(self.pressures):
            gas = Fluid(T=temp, P=pressure, species='h2')
            orifice = Orifice(d=diameter)
            jet = Jet(gas, orifice, air, theta0=0, Ymin=1e-4)

            calc_distance = jet.S / diameter
            calc_concentration = 1 / jet.X_cl

            exp_distance = csv_data[idx]['x1']
            exp_concentration = csv_data[idx]['y1']

            self.output_data[pressure] = utils.interp_and_trim_data(calculated_x=calc_distance, calculated_y=calc_concentration,
                                                                    experiment_x=exp_distance, experiment_y=exp_concentration)

    def test_centerline_concentration(self):
        for pressure in self.pressures:
            title = 'Han et al. 2013 - Figure 8, Inverse Mole Fraction vs. Axial Distance, P0 = ' + str(pressure / bar) + ' bar'

            data = self.output_data[pressure]
            error_limits = self.limits[title]
            error = utils.get_error(exp_vals=data['exp_y'],
                                    calc_vals=data['calc_y'],
                                    error_limits=error_limits,
                                    units='',
                                    msg=title,
                                    output_filename=OUTPUT_FILE,
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
                                x=data['calc_x'],
                                exp_y=data['exp_y'],
                                calc_y=data['calc_y'],
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Normalized Axial Distance (z/D)',
                                ylabel='Inverse Mole Fraction')

if __name__ == "__main__":
    if os.path.exists(OUTPUT_FILE):
        os.remove(OUTPUT_FILE)
    unittest.main()
