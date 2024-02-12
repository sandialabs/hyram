"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest
import json

from scipy.constants import pi

from hyram.phys import Fluid, Orifice, Flame
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
OUTPUT_LOC = os.path.join(misc_utils.get_temp_folder(), 'validation-cryo')
OUTPUT_FILE = os.path.join(OUTPUT_LOC, 'Validation.txt')
LIMITS_FILE = os.path.join(DATA_LOC, 'Limits-cryo.json')


class Test_LabCryoData(unittest.TestCase):
    """
    Tests for flame length and heatflux, measured vs. calculated
    """
    def setUp(self):
        self.csv_data = utils.read_csv(os.path.join(DATA_LOC, 'cryo-data.csv'), header=0)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

        air = Fluid(T=295, P=101325, species='air')

        self.test_ids = self.csv_data['TEST ID']
        data_length = len(self.test_ids)

        self.axial_dists = [.064, .22, .32, .42, .65]

        # Create lists for flamelength and heatflux
        self.flamelengths = []
        self.heatflux_qs = [[], [], [], [], []]
        for idx in range(data_length):
            diameter = self.csv_data['D (mm)'][idx] * 1e-3
            orifice = Orifice(diameter)

            temperature = self.csv_data['T (K)'][idx]
            pressure = self.csv_data['P (bar_g)'][idx] * 1e5 + 1e5
            gas = Fluid(T=temperature, P=pressure , species='h2')
            flame = Flame(gas, orifice, air, theta0=pi/2)

            self.flamelengths.append(flame.length())

            for idx, dist in enumerate(self.axial_dists):
                heatflux = flame.Qrad_multi(.2, dist, 0, RH=.8)
                self.heatflux_qs[idx].append(heatflux)

    def test_flamelength(self):
        title = "Lab Cryo Data - Flamelength"

        calc_flamelength = self.flamelengths
        exp_flamelength = self.csv_data['Fl']

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=exp_flamelength,
                                calc_vals=calc_flamelength,
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

    def test_heatflux_dist1(self):
        idx = 0
        dist = self.axial_dists[idx]
        title = "Lab Cryo Data - Heatflux, Axial Distance = " + str(dist) + " m"

        calc_flamelength = self.heatflux_qs[idx]
        exp_flamelength = self.csv_data[f'Q{idx+1}']

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=exp_flamelength,
                                calc_vals=calc_flamelength,
                                error_limits=error_limits,
                                units='kW/m^2',
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

    def test_heatflux_dist2(self):
        idx = 1
        dist = self.axial_dists[idx]
        title = "Lab Cryo Data - Heatflux, Axial Distance = " + str(dist) + " m"

        calc_flamelength = self.heatflux_qs[idx]
        exp_flamelength = self.csv_data[f'Q{idx+1}']

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=exp_flamelength,
                                calc_vals=calc_flamelength,
                                error_limits=error_limits,
                                units='kW/m^2',
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

    def test_heatflux_dist3(self):
        idx = 2
        dist = self.axial_dists[idx]
        title = "Lab Cryo Data - Heatflux, Axial Distance = " + str(dist) + " m"

        calc_flamelength = self.heatflux_qs[idx]
        exp_flamelength = self.csv_data[f'Q{idx+1}']

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=exp_flamelength,
                                calc_vals=calc_flamelength,
                                error_limits=error_limits,
                                units='kW/m^2',
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

    def test_heatflux_dist4(self):
        idx = 3
        dist = self.axial_dists[idx]
        title = "Lab Cryo Data - Heatflux, Axial Distance = " + str(dist) + " m"

        calc_flamelength = self.heatflux_qs[idx]
        exp_flamelength = self.csv_data[f'Q{idx+1}']

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=exp_flamelength,
                                calc_vals=calc_flamelength,
                                error_limits=error_limits,
                                units='kW/m^2',
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

    def test_heatflux_dist5(self):
        idx = 4
        dist = self.axial_dists[idx]
        title = "Lab Cryo Data - Heatflux, Axial Distance = " + str(dist) + " m"

        calc_flamelength = self.heatflux_qs[idx]
        exp_flamelength = self.csv_data[f'Q{idx+1}']

        error_limits = self.limits[title]
        error = utils.get_error(exp_vals=exp_flamelength,
                                calc_vals=calc_flamelength,
                                error_limits=error_limits,
                                units='kW/m^2',
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
