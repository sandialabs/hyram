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
from scipy.constants import milli
from scipy.optimize import fsolve

from hyram.phys import Fluid, Orifice, Source, Vent, Enclosure, IndoorRelease, NozzleFlow

from . import utils


# Flags to enable command line output, pyplots, and text file output
VERBOSE = False
CREATE_PLOTS = False
CREATE_OUTPUT = False

# Absolute paths to input/output data
DATA_LOC = os.path.join(os.path.dirname(__file__), 'data')
LIMITS_FILE = os.path.join(DATA_LOC, 'Limits-overpressure_accumulation.json')
OUTPUT_LOC = os.path.join('out', 'validation-overpressure_accumulation')


class Test_Ekoto_2012(unittest.TestCase):
    """
    Test mole fraction and peak pressure calculations against Ekoto et al. (2012).
    """

    def setUp(self):
        self.amb_species = "air"
        self.rel_species = "H2"
        self.amb_temp = 297
        self.amb_pressure = 101325
        self.rel_pressure = 13450000
        self.rel_temp = 297
        self.tank_volume = .00363  # m3
        self.orifice_diameter = .00356  # m
        self.orifice_dis_coeff = 0.75
        self.rel_dis_coeff = 1
        self.rel_area = 0.0171  # m2
        self.rel_height = 0.2495  # m
        self.enclosure_height = 2.72  # m
        self.floor_ceil_area = 16.722  # m2
        self.dist_rel_to_wall = 2.1255  # m
        self.rel_angle = 0
        self.is_steady = False

        fluid = Fluid(species=self.rel_species, P=self.rel_pressure, T=self.rel_temp)
        self.ambient = Fluid(species=self.amb_species, T=self.amb_temp, P=self.amb_pressure)
        self.orifice = Orifice(self.orifice_diameter, Cd=self.orifice_dis_coeff)
        self.source = Source(self.tank_volume, fluid)

        # Create IndoorRelease object with no vent
        ceil_vent_xarea = 0.00865  # m2
        ceil_vent_height = 2.42
        floor_vent_xarea = 0.0
        floor_vent_height = 0.0
        vol_flow_rate = 0.0

        ceil_vent = Vent(ceil_vent_xarea, ceil_vent_height, self.rel_dis_coeff, vol_flow_rate)
        floor_vent = Vent(floor_vent_xarea, floor_vent_height, self.rel_dis_coeff, vol_flow_rate)
        enclosure = Enclosure(self.enclosure_height, self.floor_ceil_area, self.rel_height, ceil_vent, floor_vent,
                              self.dist_rel_to_wall)

        self.novent_release = IndoorRelease(self.source, self.orifice, self.ambient, enclosure, tmax=20.5,release_area=self.rel_area,
                                            theta0=self.rel_angle, verbose=False)

        # Create IndoorRelease object with vent
        ceil_vent_xarea = 0.1  # m2
        vol_flow_rate = 6.5 / 60  # m3/s

        ceil_vent = Vent(ceil_vent_xarea, ceil_vent_height, self.rel_dis_coeff, vol_flow_rate)
        floor_vent = Vent(floor_vent_xarea, floor_vent_height, self.rel_dis_coeff, vol_flow_rate)
        enclosure = Enclosure(self.enclosure_height, self.floor_ceil_area, self.rel_height, ceil_vent, floor_vent,
                                            self.dist_rel_to_wall)

        self.vent_release = IndoorRelease(self.source, self.orifice, self.ambient, enclosure, tmax=20.5,
                                    release_area=self.rel_area, theta0=self.rel_angle, verbose=False)


        # Import experimental data
        self.data = utils.read_csv(os.path.join(DATA_LOC, 'ekoto-2012-fig9.csv'), header=2)

        # Import error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    # Fig 9, Mole Fraction vs. Time
    def test_fig9_noVent(self):
        sensor_list = ['S04 No Vent', 'S08 No Vent', 'S11 No Vent', 'S04 No Vent 2', 'S08 No Vent 2', 'S11 No Vent 2']
        for i, sensor in enumerate(sensor_list):
            title = f'Ekoto et al. 2012 - Figure 9 Concentration vs. Time, {sensor}'
            error_limits = self.limits[title]

            # Get data from csv
            time = self.data[f'x{i+1}']
            exp_concentration = self.data[f'y{i+1}']

            # Calculate concentration
            calc_concentration = self.novent_release.concentration(time) / 100

            error = utils.get_error(exp_vals=exp_concentration,
                                    calc_vals=calc_concentration,
                                    error_limits=error_limits,
                                    units='Mole Fraction',
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

            # Create plots
            if CREATE_PLOTS:
                utils.create_plots(output_dir=OUTPUT_LOC,
                                   x=time,
                                   exp_y=exp_concentration,
                                   calc_y=calc_concentration,
                                   max_error=error['Max Absolute Error'],
                                   avg_error=error['Avg Absolute Error'],
                                   smape=error['SMAPE'],
                                   title=title,
                                   xlabel='Time (s)',
                                   ylabel='Mole Fraction')

    # Fig 9, w/ vent
    def test_fig9_vent(self):
        sensor_list = ['S04 Vent', 'S08 Vent', 'S11 Vent']
        for i, sensor in enumerate(sensor_list):
            title = f'Ekoto et al. 2012 - Figure 9 Concentration vs. Time, {sensor}'
            error_limits = self.limits[title]

            # Get data from csv
            time = self.data[f'x{7+i}']
            exp_concentration = self.data[f'y{7+i}']

            # Calculate concentration
            calc_concentration = self.vent_release.concentration(time) / 100

            error = utils.get_error(exp_vals=exp_concentration,
                                    calc_vals=calc_concentration,
                                    error_limits=error_limits,
                                    units='Mole Fraction',
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

            # Create plots
            if CREATE_PLOTS:
                utils.create_plots(output_dir=OUTPUT_LOC,
                                   x=time,
                                   exp_y=exp_concentration,
                                   calc_y=calc_concentration,
                                   max_error=error['Max Absolute Error'],
                                   avg_error=error['Avg Absolute Error'],
                                   smape=error['SMAPE'],
                                   title=title,
                                   xlabel='Time (s)',
                                   ylabel='Mole Fraction')

    # Fig 14, Test peak pressure. As point-to-point comparisons, this test uses its own error checking
    def test_peak_pressure(self):
        amb_species = "air"
        rel_species = "H2"
        amb_temp = 297
        amb_pressure = 101325
        rel_pressure = 13450000
        rel_temp = 297
        tank_volume = .00363  # m3
        orifice_diameter = .00356  # m
        orifice_dis_coeff = 0.75
        rel_dis_coeff = 1
        rel_area = 0.0171  # m2
        rel_height = 0.2495  # m
        enclosure_height = 2.72  # m
        floor_ceil_area = 16.722  # m2
        dist_to_wall = 2.1255  # m
        rel_angle = 0
        xloc = 0
        yloc = 0

        fluid = Fluid(species=rel_species, P=rel_pressure, T=rel_temp)
        ambient = Fluid(species=amb_species, T=amb_temp, P=amb_pressure)
        orifice = Orifice(orifice_diameter, Cd=orifice_dis_coeff)
        source = Source(tank_volume, fluid)

        # From the notebook
        t = 3

        # Lumping these 3 into this one test since they're all just point-to-point comparisons
        # Test 12 - Small vent, passive ventilation
        title = "Ekoto et al. 2012 - Figure 14, Peak Pressure Test 12, Small vent w/ passive ventilation"
        error_limits = self.limits[title]

        ceil_vent = Vent(0.00368, 2.42, rel_dis_coeff, 0)
        floor_vent = Vent(0, 0, rel_dis_coeff, 0)
        enclosure = Enclosure(enclosure_height, floor_ceil_area, rel_height, ceil_vent, floor_vent,
                              dist_to_wall)
        release = IndoorRelease(source, orifice, ambient, enclosure, tmax=8.5,
                                release_area=rel_area, theta0=rel_angle,
                                x0=xloc, y0=yloc, verbose=False)
	
        exp_pressure = 24.48451643
        calc_pressure = release.pressure(t) / 1000
        error = utils.get_error(exp_vals=[exp_pressure],
                                calc_vals=[calc_pressure],
                                error_limits=error_limits,
                                units='kPa',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        # Only test absolute error and percent error values
        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Percent Error out of range")

        # Test 11 - Big vent, passive ventilation
        title = "Ekoto et al. 2012 - Figure 14, Peak Pressure Test 11, Big vent w/ passive ventilation"
        error_limits = self.limits[title]

        ceil_vent = Vent(0.09716, 2.42, rel_dis_coeff, 0)
        floor_vent = Vent(0, 0, rel_dis_coeff, 0)
        enclosure = Enclosure(enclosure_height, floor_ceil_area, rel_height, ceil_vent, floor_vent,
                              dist_to_wall)
        release = IndoorRelease(source, orifice, ambient, enclosure, tmax=8.5,
                                release_area=rel_area, theta0=rel_angle,
                                x0=xloc, y0=yloc, verbose=False)

        exp_pressure = 4.131355932
        calc_pressure = release.pressure(t) / 1000
        error = utils.get_error(exp_vals=[exp_pressure],
                                calc_vals=[calc_pressure],
                                error_limits=error_limits,
                                units='kPa',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        # Only test absolute error and percent error values
        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Percent Error out of range")


        # Test 10 - Big vent, active ventilation
        title = "Ekoto et al. 2012 - Figure 14, Peak Pressure Test 10, Big vent w/ active ventilation"
        error_limits = self.limits[title]

        ceil_vent = Vent(0.09716, 2.42, rel_dis_coeff, 6.3/60)
        floor_vent = Vent(0, 0, rel_dis_coeff, 6.3/60)
        enclosure = Enclosure(enclosure_height, floor_ceil_area, rel_height, ceil_vent, floor_vent,
                                    dist_to_wall)
        release = IndoorRelease(source, orifice, ambient, enclosure, tmax=8.5,
                                release_area=rel_area, theta0=rel_angle,
                                x0=xloc, y0=yloc, verbose=False)

        exp_pressure = 3.019067797
        calc_pressure = release.pressure(t) / 1000
        error = utils.get_error(exp_vals=[exp_pressure],
                                calc_vals=[calc_pressure],
                                error_limits=error_limits,
                                units='kPa',
                                msg=title,
                                output_dir=OUTPUT_LOC,
                                create_output=CREATE_OUTPUT,
                                verbose=VERBOSE)

        # Only test absolute error and percent error values
        with self.subTest():
            self.assertLessEqual(error['Max Absolute Error'], error_limits['Max Absolute Error'], f"{title}: Absolute Error out of range")
        with self.subTest():
            self.assertLessEqual(error['Max Percent Error'], error_limits['Max Percent Error'], f"{title}: Percent Error out of range")


class Test_Giannissi_2015(unittest.TestCase):
    """
    Test concentration calculation against Giannissi al. (2015)
    """

    def setUp(self):
        amb_species = "air"
        rel_species = "H2"
        amb_temp = 287
        amb_pressure = 101325
        rel_pressure = 1.70e06
        rel_temp = 285
        tank_volume = 3943 / 1000 / 60 # std liters/min to m3/s
        orifice_diameter = .00055  # m
        orifice_dis_coeff = 1
        rel_dis_coeff = 1
        rel_area = np.pi * (orifice_diameter / 2) ** 2  # m2
        rel_height = 0.5  # m
        enclosure_height = 2.5  # m
        floor_ceil_area = 12.5  # m2
        dist_to_wall = 2.5  # m
        rel_angle = np.pi / 2
        is_steady = True

        # Set max time to pressure peak
        max_time = 1600

        ceil_vent_xarea = 0.2241  # m2
        ceil_vent_height = 2.13
        floor_vent_xarea = 0.0
        floor_vent_height = 0.0
        vol_flow_rate = 0.0

        fluid = Fluid(species=rel_species, P=rel_pressure, T=rel_temp)
        ambient = Fluid(species=amb_species, T=amb_temp, P=amb_pressure)
        orifice = Orifice(orifice_diameter, Cd=orifice_dis_coeff)
        source = Source(tank_volume, fluid)
        ceil_vent = Vent(ceil_vent_xarea, ceil_vent_height, rel_dis_coeff, vol_flow_rate)
        floor_vent = Vent(floor_vent_xarea, floor_vent_height, rel_dis_coeff, vol_flow_rate)
        enclosure = Enclosure(enclosure_height, floor_ceil_area, rel_height, ceil_vent, floor_vent,
                                            dist_to_wall)
        self.release = IndoorRelease(source, orifice, ambient, enclosure, tmax=max_time,
                                        release_area=rel_area, theta0=rel_angle, steady=is_steady, nsteady = 20,
                                        x0=0, y0=rel_height, verbose=False)

        self.data = utils.read_csv(os.path.join(DATA_LOC, 'giannissi-2015-fig7.csv'), header=2)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_concentration_time_s18(self):
        idx = 0
        title = "Giannissi et al. 2015 - Figure 7, Concentration v. Time, Sensor 18"
        error_limits = self.limits[title]

        # Cull data past 1600s
        time = self.data[f'x{idx+1}'][:16]
        exp_concentration = self.data[f'y{idx+1}'][:16]

        calc_concentration = self.release.concentration(time)

        error = utils.get_error(exp_vals=exp_concentration,
                                calc_vals=calc_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=time,
                                exp_y=exp_concentration,
                                calc_y=calc_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Time (s)',
                                ylabel='Concentration (vol %)')

    def test_concentration_time_s21(self):
        idx = 1
        title = "Giannissi et al. 2015 - Figure 7, Concentration v. Time, Sensor 21"
        error_limits = self.limits[title]

        # Cull data past 1600s
        time = self.data[f'x{idx+1}'][:16]
        exp_concentration = self.data[f'y{idx+1}'][:16]

        calc_concentration = self.release.concentration(time)

        error = utils.get_error(exp_vals=exp_concentration,
                                calc_vals=calc_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=time,
                                exp_y=exp_concentration,
                                calc_y=calc_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Time (s)',
                                ylabel='Concentration (vol %)')

    def test_concentration_time_s27(self):
        idx = 0
        title = "Giannissi et al. 2015 - Figure 7, Concentration v. Time, Sensor 27"
        error_limits = self.limits[title]

        # Cull data past 1600s
        time = self.data[f'x{idx+1}'][:16]
        exp_concentration = self.data[f'y{idx+1}'][:16]

        calc_concentration = self.release.concentration(time)

        error = utils.get_error(exp_vals=exp_concentration,
                                calc_vals=calc_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=time,
                                exp_y=exp_concentration,
                                calc_y=calc_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Time (s)',
                                ylabel='Concentration (vol %)')


class Test_Merilo(unittest.TestCase):
    """
    Test concentration calculation against Merilo al. (2011) Figure 2
    """

    def setUp(self):
        self.amb_species = "air"
        self.rel_species = "H2"
        self.amb_temp = 285
        self.amb_pressure = 101325
        self.rel_temp = 285
        self.tank_volume = 1e6 # unnaturally large so that flow is nominally steady
        release_diameter = 7.75*milli  # m
        self.orifice_dis_coeff = 1
        self.rel_dis_coeff = 1
        self.rel_area = np.pi * (release_diameter / 2) ** 2  # m2
        self.enclosure_height = 2.72
        self.floor_ceil_area = 22.204
        self.rel_angle = np.pi / 2
        self.is_steady = True # ESH
        self.floor_vent_flow = 0.0

        self.ceil_vent_xarea = 0.11  # m2
        self.ceil_vent_height = 2.42
        self.floor_vent_xarea = .11
        self.floor_vent_height = .17

        tests = [
            {'name': 'Test 1', 'rel_rate': 9.22, 'mass_rel': 3.07,  'vel': 668, 'dur': 20, 'rel_ht': 0.25, 'dist_to_wall': 3.04, 'flow': 0, 'd_throat':1.7*milli, 'tmax':20*60},
            {'name': 'Test 2', 'rel_rate': 9.04, 'mass_rel': 3.01,  'vel': 653, 'dur': 20, 'rel_ht': 1, 'dist_to_wall': 4.85, 'flow': 0, 'd_throat':1.7*milli, 'tmax':20*60},
            {'name': 'Test 3', 'rel_rate': 0.88, 'mass_rel': .44,  'vel': 63, 'dur': 30, 'rel_ht': 1, 'dist_to_wall': 4.85, 'flow': 0, 'd_throat':1.2*milli, 'tmax':30*60},
            {'name': 'Test 7', 'rel_rate': 6.7, 'mass_rel': 4.47,  'vel': 502, 'dur': 30, 'rel_ht': .25, 'dist_to_wall': 3.04, 'flow': 0.1, 'd_throat':1.7*milli, 'tmax':45*60},
        ]

        self.releases = []
        for test in tests:
            # Set up release
            ambient = Fluid(species=self.amb_species, T=self.amb_temp, P=self.amb_pressure)
            orifice = Orifice(test['d_throat'])

            def err(P):
                g = Fluid(species=self.rel_species, P=P, T=self.rel_temp)
                return (test['rel_rate'] / 3600) - NozzleFlow(g, orifice, self.amb_pressure).mdot
            pressure = fsolve(err, 10*self.amb_pressure)[0]

            fluid = Fluid(species=self.rel_species, P=pressure, T=self.rel_temp)
            orifice = Orifice(test['d_throat'], Cd=self.orifice_dis_coeff)
            source = Source(self.tank_volume, fluid)
            ceil_vent = Vent(self.ceil_vent_xarea, self.ceil_vent_height, self.rel_dis_coeff, test['flow'])
            floor_vent = Vent(self.floor_vent_xarea, self.floor_vent_height, self.rel_dis_coeff, self.floor_vent_flow)
            enclosure = Enclosure(self.enclosure_height, self.floor_ceil_area, test['rel_ht'], ceil_vent, floor_vent, test['dist_to_wall'])
            release = IndoorRelease(source, orifice, ambient, enclosure, tmax=test['tmax'],
                                    release_area=self.rel_area, theta0=self.rel_angle, steady=self.is_steady,
                                    x0=0, y0=enclosure.H_release, verbose=False, nsteady=30)
            self.releases.append(release)

        self.data = utils.read_csv(os.path.join(DATA_LOC, 'merilo-2011-fig2.csv'), header=2)

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_concentration_time_test1(self):
        idx = 0
        title = f"Merilo et al. 2011 - Figure 2, Concentration v. Time, Test 1"
        error_limits = self.limits[title]

        # Using best fit line from experimental data
        time = self.data[f'x{(idx+1)*4}']
        exp_concentration = self.data[f'y{(idx+1)*4}']

        # Use linear interpolation on the calculated values
        calc_concentration = np.interp(time, self.releases[idx].t_layer/60, self.releases[idx].x_layer*100)

        error = utils.get_error(exp_vals=exp_concentration,
                                calc_vals=calc_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=time,
                                exp_y=exp_concentration,
                                calc_y=calc_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Time (s)',
                                ylabel='Concentration (vol %)')

    def test_concentration_time_test2(self):
        idx = 1
        title = f"Merilo et al. 2011 - Figure 2, Concentration v. Time, Test 2"
        error_limits = self.limits[title]

        time = self.data[f'x{(idx+1)*4}']
        exp_concentration = self.data[f'y{(idx+1)*4}']
        calc_concentration = np.interp(time, self.releases[idx].t_layer/60, self.releases[idx].x_layer*100)

        error = utils.get_error(exp_vals=exp_concentration,
                                calc_vals=calc_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=time,
                                exp_y=exp_concentration,
                                calc_y=calc_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Time (s)',
                                ylabel='Concentration (vol %)')

    def test_concentration_time_test3(self):
        idx = 2
        title = f"Merilo et al. 2011 - Figure 2, Concentration v. Time, Test 3"
        error_limits = self.limits[title]

        time = self.data[f'x{(idx+1)*4}']
        exp_concentration = self.data[f'y{(idx+1)*4}']
        calc_concentration = np.interp(time, self.releases[idx].t_layer/60, self.releases[idx].x_layer*100)

        error = utils.get_error(exp_vals=exp_concentration,
                                calc_vals=calc_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=time,
                                exp_y=exp_concentration,
                                calc_y=calc_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Time (s)',
                                ylabel='Concentration (vol %)')

    def test_concentration_time_test7(self):
        idx = 3
        title = f"Merilo et al. 2011 - Figure 2, Concentration v. Time, Test 7"
        error_limits = self.limits[title]

        time = self.data[f'x{(idx+1)*4}']
        exp_concentration = self.data[f'y{(idx+1)*4}']
        calc_concentration = np.interp(time, self.releases[idx].t_layer/60, self.releases[idx].x_layer*100)

        error = utils.get_error(exp_vals=exp_concentration,
                                calc_vals=calc_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=time,
                                exp_y=exp_concentration,
                                calc_y=calc_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Time (s)',
                                ylabel='Concentration (vol %)')


class Test_BMHA_Fig7(unittest.TestCase):
    """
    Test concentration vs flowrate calculation against Bernard-Michel and Houssin-Agbomson (2017), Figure 7 -- release in a 2m^3 enclosure
    """

    def setUp(self):
        amb_species = "air"
        rel_species = "H2"
        amb_temp = 286
        amb_pressure = 101325
        rel_temp = 286
        tank_volume = 1e6 # unnaturally large so that flow is nominally steady
        rel_dis_coeff = 1

        diameter_1 = .0272
        diameter_2 = .004
        area_1 = np.pi * (diameter_1 / 2) ** 2  # m2
        area_2 = np.pi * (diameter_2 / 2) ** 2  # m2
        Qs2m3 = np.array([5.2, 21, 73, 218])/1000/60
        rel_angle = np.pi / 2
        x0 = 0
        y0 = 0

        # Figure 7
        rel_height = 0.27  # m
        enclosure_height = 2.1  # m
        floor_ceil_area = .9216
        dist_to_wall = 0.48  # m
        ceil_vent_xarea = 0.1862  # m2
        ceil_vent_height = 1.7
        floor_vent_xarea = .1862
        floor_vent_height = 0.02
        is_steady = True

        d_imaginary = 2e-4
        ambient = Fluid(species=amb_species, T=amb_temp, P=amb_pressure)
        orifice = Orifice(d_imaginary)
        rho_H2_STP = Fluid(species='hydrogen', T=295, P=101325).rho

        self.flowrate = Qs2m3 * 60 * 1000

        # Calculate pressure data
        pressures = []
        for Q in Qs2m3:
            def err_hy3(P):
                g = Fluid(species=rel_species, P=P, T=rel_temp)
                throat = NozzleFlow(g, orifice, amb_pressure)
                return Q - throat.mdot/rho_H2_STP

            pressures.append(fsolve(err_hy3, 10*amb_pressure)[0])

        data = utils.read_csv(os.path.join(DATA_LOC, 'bernardmichelhoussinagbomson-2017-fig7.csv'), header=2)

        # Create concentration data for both release areas, d=4mm and d=27mm
        self.calc_4mm_concentration = []
        self.calc_27mm_concentration = []
        for pressure in pressures:
            fluid = Fluid(species=rel_species, P=pressure, T=rel_temp)
            source = Source(tank_volume, fluid)

            ceil_vent = Vent(ceil_vent_xarea, ceil_vent_height, rel_dis_coeff, 0)
            floor_vent = Vent(floor_vent_xarea, floor_vent_height, rel_dis_coeff, 0)
            enclosure = Enclosure(enclosure_height, floor_ceil_area, rel_height, ceil_vent, floor_vent,
                                        dist_to_wall)

            release_4mm = IndoorRelease(source, orifice, ambient, enclosure,
                                        release_area=area_1, theta0=rel_angle, steady=is_steady,
                                        x0=x0, y0=y0, verbose=False)
            release_27mm = IndoorRelease(source, orifice, ambient, enclosure,
                                         release_area=area_2, theta0=rel_angle, steady=is_steady,
                                         x0=x0, y0=y0, verbose=False)

            self.calc_4mm_concentration.append(release_4mm.x_layer[-1] * 100)
            self.calc_27mm_concentration.append(release_27mm.x_layer[-1] * 100)

        self.exp_4mm_concentration = data['y1']
        self.exp_27mm_concentration = data['y2']

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_flowrate_4mm(self):
        title = "Bernard et al. 2017 - Figure 7, Concentration v. Flow Rate, d = 4mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=self.exp_4mm_concentration,
                                calc_vals=self.calc_4mm_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=self.flowrate,
                                exp_y=self.exp_4mm_concentration,
                                calc_y=self.calc_4mm_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Flow Rate (NL/min)',
                                ylabel='Concentration (vol %)')

    def test_flowrate_27mm(self):
        title = "Bernard et al. 2017 - Figure 7, Concentration v. Flow Rate, d = 27mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=self.exp_27mm_concentration,
                                calc_vals=self.calc_27mm_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=self.flowrate,
                                exp_y=self.exp_27mm_concentration,
                                calc_y=self.calc_27mm_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Flow Rate (NL/min)',
                                ylabel='Concentration (vol %)')


class Test_BMHA_Fig9(unittest.TestCase):
    """
    Test concentration vs flowrate calculation against Bernard-Michel and Houssin-Agbomson (2017), Figure 9 -- release in a 1m^3 enclosure
    """

    def setUp(self):
        amb_species = "air"
        rel_species = "H2"
        amb_temp = 286
        amb_pressure = 101325
        rel_temp = 286
        tank_volume = 1e6 # unnaturally large so that flow is nominally steady
        rel_dis_coeff = 1

        diameter_1 = .0272
        diameter_2 = .004
        area_1 = np.pi * (diameter_1 / 2) ** 2  # m2
        area_2 = np.pi * (diameter_2 / 2) ** 2  # m2
        Qs1m3 = np.array([10, 21, 62, 104, 218])/1000/60
        rel_height = .08
        enclosure_height = 1
        floor_ceil_area = .990025
        dist_to_wall = .4975
        ceil_vent_xarea = .1728
        ceil_vent_height = .75
        floor_vent_xarea = .1728
        floor_vent_height = .02
        rel_angle = np.pi/2
        x0 = 0
        y0 = 0
        is_steady = True

        d_imaginary = 2e-4
        ambient = Fluid(species=amb_species, T=amb_temp, P=amb_pressure)
        orifice = Orifice(d_imaginary)
        rho_H2_STP = Fluid(species=rel_species, T = 295, P = 101325).rho

        self.flowrate = Qs1m3 * 60 * 1000

        pressures = []
        for Q in Qs1m3:
            def err_hy3(P):
                g = Fluid(species=rel_species, P=P, T=rel_temp)
                throat = NozzleFlow(g, orifice, amb_pressure)
                return Q - throat.mdot/rho_H2_STP

            pressures.append(fsolve(err_hy3, 10*amb_pressure)[0])

        data = utils.read_csv(os.path.join(DATA_LOC, 'bernardmichelhoussinagbomson-2017-fig9.csv'), header=2)

        # Create concentration data for both release areas, d=4mm and d=27mm
        self.calc_4mm_concentration = []
        self.calc_27mm_concentration = []
        for pressure in pressures:
            fluid = Fluid(species=rel_species, P=pressure, T=rel_temp)
            source = Source(tank_volume, fluid)

            ceil_vent = Vent(ceil_vent_xarea, ceil_vent_height, rel_dis_coeff, 0)
            floor_vent = Vent(floor_vent_xarea, floor_vent_height, rel_dis_coeff, 0)
            enclosure = Enclosure(enclosure_height, floor_ceil_area, rel_height, ceil_vent, floor_vent, dist_to_wall)

            release_4mm = IndoorRelease(source, orifice, ambient, enclosure,
                                        release_area=area_1, theta0=rel_angle, steady=is_steady,
                                        x0=x0, y0=y0, verbose=False)
            release_27mm = IndoorRelease(source, orifice, ambient, enclosure,
                                         release_area=area_2, theta0=rel_angle, steady=is_steady,
                                         x0=x0, y0=y0, verbose=False)

            self.calc_4mm_concentration.append(release_4mm.x_layer[-1] * 100)
            self.calc_27mm_concentration.append(release_27mm.x_layer[-1] * 100)

        self.exp_4mm_concentration = data['y1']
        self.exp_27mm_concentration = data['y2']

        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_flowrate_4mm(self):
        title = "Bernard et al. 2017 - Figure 9, Concentration v. Flow Rate, d = 4mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=self.exp_4mm_concentration,
                                calc_vals=self.calc_4mm_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=self.flowrate,
                                exp_y=self.exp_4mm_concentration,
                                calc_y=self.calc_4mm_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Flow Rate (NL/min)',
                                ylabel='Concentration (vol %)')

    def test_flowrate_27mm(self):
        title = "Bernard et al. 2017 - Figure 9, Concentration v. Flow Rate, d = 27mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=self.exp_27mm_concentration,
                                calc_vals=self.calc_27mm_concentration,
                                error_limits=error_limits,
                                units='vol %',
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

        # Create plots
        if CREATE_PLOTS:
            utils.create_plots(output_dir=OUTPUT_LOC,
                                x=self.flowrate,
                                exp_y=self.exp_27mm_concentration,
                                calc_y=self.calc_27mm_concentration,
                                max_error=error['Max Absolute Error'],
                                avg_error=error['Avg Absolute Error'],
                                smape=error['SMAPE'],
                                title=title,
                                xlabel='Flow Rate (NL/min)',
                                ylabel='Concentration (vol %)')
