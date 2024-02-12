"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""

import os
import unittest
import json

import numpy as np
from scipy.constants import liter, bar, psi
from scipy import interpolate

from hyram.phys import Source, Fluid, Orifice
from hyram.utilities import misc_utils

from . import utils

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

# Flags to enable command line output, pyplots, and text file output
VERBOSE = False
CREATE_PLOTS = False
CREATE_OUTPUT = False

# Absolute paths to input/output data
DATA_LOC = os.path.join(os.path.dirname(__file__), 'data')
OUTPUT_LOC = os.path.join(misc_utils.get_temp_folder(), 'validation-blowdown')
OUTPUT_FILE = os.path.join(OUTPUT_LOC, 'Validation.txt')
LIMITS_FILE = os.path.join(DATA_LOC, 'Limits-blowdown.json')


class Test_Ekoto_2012(unittest.TestCase):
    """
    Single test for flow rate over time, data from Ekoto et al. 2012 Figure 3
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'ekoto-2012-fig3.csv'), header=2)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set up parameters
        temp = 297  # K...assumed initial tank temp is ambient air temp. Table 1 of 'Exp. investigation of hydrogen release' by Ekoto
        pressure = 13450000 # Pa...Table 1 of above source
        orifice_diameter = 0.00356  # m...Table 1 of above source
        discharge_coeff = 0.75 #...Table 1 of above source
        tank_volume = 0.00363  # m^3...Table 1 of above source
        rel_species = 'H2'

        fluid = Fluid(species=rel_species, P=pressure, T=temp)
        orifice = Orifice(orifice_diameter, discharge_coeff)
        source = Source(tank_volume, fluid)

        # Calculated values
        self.calc_flowrate, _, self.calc_time, _ = source.empty(orifice)

        # Experimental values
        exp_time = np.ravel(data['x3'])
        self.exp_flowrate = np.ravel(data['y3'])

        # Interpolate to calculated data
        interp = interpolate.interp1d(exp_time, self.exp_flowrate, fill_value='extrapolate')
        self.exp_flowrate = interp(self.calc_time)

    def test_flowrate(self):
        title = "Ekoto et al. 2012 - Figure 3, Flow Rate v. Time"
        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.exp_flowrate,
                                calc_vals=self.calc_flowrate,
                                error_limits=error_limits,
                                units='kg/s',
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
                               x=self.calc_time,
                               exp_y=self.exp_flowrate,
                               calc_y=self.calc_flowrate,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Flow Rate (kg/s)')


class Test_Schefer_2007(unittest.TestCase):
    """
    Single test for pressure over time, data from Schefer et al. 2007 Figure 4
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'schefer-2007-fig4.csv'), header=2)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set up variables
        temp = 290  # K
        pressure = 4.31e7 # Pa
        orifice_diameter = 0.00508  # m
        discharge_coeff = 1
        tank_volume = 1234 * liter  # m^3
        rel_species = 'H2'

        fluid = Fluid(species=rel_species, P=pressure, T=temp)
        orifice = Orifice(orifice_diameter, discharge_coeff)
        source = Source(tank_volume, fluid)
        _, fluid_list, times, _ = source.empty(orifice)

        # Calculated values
        self.calc_time = np.array(times)
        self.calc_pressure = np.array([fluid.P for fluid in fluid_list]) / psi

        x_data = data['x1'] + data['x2']
        y_data = data['y1'] + data['y2']
        sorted_data = sorted(zip(x_data, y_data))

        # Experimental values
        exp_time = []
        self.exp_pressure = []

        def add_data(x, y):
            exp_time.append(x)
            self.exp_pressure.append(y)

        [add_data(x,y) for x, y in sorted_data if x not in exp_time]

        interp = interpolate.interp1d(exp_time, self.exp_pressure, fill_value='extrapolate')
        self.exp_pressure = interp(self.calc_time)

    def test_pressure(self):
        title = "Schefer et al. 2007 - Figure 4, Pressure v. Time"
        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.exp_pressure,
                                calc_vals=self.calc_pressure,
                                error_limits=error_limits,
                                units='PSIG',
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
                               x=self.calc_time,
                               exp_y=self.exp_pressure,
                               calc_y=self.calc_pressure,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Pressure (PSIG)')


class Test_Proust_2011(unittest.TestCase):
    """
    Tests for pressure and temperature over time, data from Proust et al. 2011 Figure 4
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'proust-2011-fig4.csv'), header=1)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set up variables
        temp = 315.15  # K
        pressure = 90 * 1000000 # Pa
        orifice_diameter = 0.002  # m
        tank_volume = 25 * liter  # m^3
        rel_species = 'H2'

        fluid = Fluid(species=rel_species, P=pressure, T=temp)
        orifice = Orifice(orifice_diameter)
        source = Source(tank_volume, fluid)
        _, fluid_list, times, _ = source.empty(orifice, heat_flux = 10)

        # Only use first 60 seconds
        times = [t for t in times if t < 60]
        fluid_list = fluid_list[:len(times)]

        # Calculated values
        self.calc_time = np.array(times)
        self.calc_pressure = np.array([fluid.P for fluid in fluid_list]) / bar
        self.calc_temp = np.array([fluid.T for fluid in fluid_list]) - 273.15

        # Experimental values
        time_pressure = data['x3']
        time_temp = data['x4']
        self.exp_pressure = data['y3']
        self.exp_temp = data['y4']

        # Interpolate values
        interp = interpolate.interp1d(time_pressure, self.exp_pressure, fill_value='extrapolate')
        self.exp_pressure = interp(self.calc_time)
        interp = interpolate.interp1d(time_temp, self.exp_temp, fill_value='extrapolate')
        self.exp_temp = interp(self.calc_time)

    def test_pressure(self):
        title = "Proust et al. 2011 - Figure 4, Pressure v. Time"
        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.exp_pressure,
                                calc_vals=self.calc_pressure,
                                error_limits=error_limits,
                                units='bar',
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
                               x=self.calc_time,
                               exp_y=self.exp_pressure,
                               calc_y=self.calc_pressure,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Pressure (bar)')

    def test_temperature(self):
        title = "Proust et al. 2011 - Figure 4, Temperature v. Time"
        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.exp_temp,
                                calc_vals=self.calc_temp,
                                error_limits=error_limits,
                                units='C',
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
                               x=self.calc_time,
                               exp_y=self.exp_temp,
                               calc_y=self.calc_temp,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Temperature (C)')


class Test_Schefer_2006(unittest.TestCase):
    """
    Single test for flow rate over time, data from Schefer et al. 2006 Figure 3b
    """
    def setUp(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'schefer-2006-fig3b.csv'), header=2)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        # Set up variables
        temp = 315.15  # K
        pressure = 1.5513e7 # Pa
        d_orifice = 0.003175  # m
        tank_vol = 0.098  # m^3
        rel_species = 'H2'

        fluid = Fluid(species=rel_species, P=pressure, T=temp)
        orifice = Orifice(d_orifice)
        source = Source(tank_vol, fluid)

        # Calculated values
        flowrate, _, self.calc_time, _ = source.empty(orifice)
        self.calc_flowrate = np.array(flowrate) * 1000

        # Experimental values
        exp_time = data['x1']
        self.exp_flowrate = data['y1']

        interp = interpolate.interp1d(exp_time, self.exp_flowrate, fill_value='extrapolate')
        self.exp_flowrate = interp(self.calc_time)

    def test_flowrate(self):
        title = "Schefer et al. 2006 - Figure 3b, Flow Rate v. Time"
        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=self.exp_flowrate,
                                calc_vals=self.calc_flowrate,
                                error_limits=error_limits,
                                units='g/s',
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
                               x=self.calc_time,
                               exp_y=self.exp_flowrate,
                               calc_y=self.calc_flowrate,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Mass Flow Rate (g/s)')
