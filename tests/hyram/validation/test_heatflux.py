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
from scipy.constants import milli, kilo, liter, bar, pi
from scipy import optimize
from scipy import interpolate

from hyram.phys import Source, Fluid, Orifice, Flame, NozzleFlow

from . import utils


# Flags to enable command line output, pyplots, and text file output
VERBOSE = False
CREATE_PLOTS = False
CREATE_OUTPUT = False

# Absolute paths to input/output data
DATA_LOC = os.path.join(os.path.dirname(__file__), 'data')
LIMITS_FILE = os.path.join(DATA_LOC, 'Limits-heatflux.json')
OUTPUT_LOC = os.path.join('out', 'validation-heatflux')


class Test_Ekoto_2014(unittest.TestCase):
    """
    Test suite for jet flame length and heat flux, data from Ekoto, Ruggles, Creitz, Li (2014)
    """

    def setUp(self):
        ambient_pressures = np.array([1.022, 1.011])*bar
        ambient_temperatures = np.array([280, 280])#K
        diameters = np.array([20.9, 52.5])*milli#m
        pressures = np.array([59.8, 62.1])*bar + ambient_pressures
        temperatures = np.array([308.7, 287.8]) #K
        self.humidities = [0.943, 0.945]
        y0 = 3.25
        self.rad_x = [26, 48]
        self.rad_y = 1.75
        self.rad_z = 0

        self.flames = []
        for amb_pressure, amb_temp, diameter, pressure, temp in zip(ambient_pressures, ambient_temperatures, diameters, pressures, temperatures):
            air = Fluid(species='air', T=amb_temp, P=amb_pressure)
            gas = Fluid(species='H2', T=temp, P=pressure)
            orifice = Orifice(d=diameter)

            self.flames.append(Flame(gas, orifice, air, y0=y0))

        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    # Flame length
    def test_flame_length(self):
        title = "Ekoto et al. 2014 - Table 2, Flame Length"
        exp_lengths = [17.4, 45.9]
        calc_lengths = [flame.length() for flame in self.flames]
        error_limits = self.limits[title]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_lengths,
                                calc_vals=calc_lengths,
                                error_limits=error_limits,
                                units='m',
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


    # Heat flux
    def test_flux(self):
        title = "Ekoto et al. 2014 - Table 2, Heat Flux"
        exp_flux = [4.7, 23.9]
        calc_flux = [flame.Qrad_multi(x, self.rad_y, self.rad_z, RH=RH) for flame, x, RH in zip(self.flames, self.rad_x, self.humidities)]
        calc_flux_adj = [1e-3 * x for x in calc_flux] # convert to kW/m^2
        error_limits = self.limits[title]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux_adj,
                                error_limits=error_limits,
                                units='W/m^2',
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


class Test_Schefer_2006_2007(unittest.TestCase):
    """
    Test suite for Heat flux vs. Distance and Flame length vs. Time.
    Data from  Schefer, Houf, Bourne, Colton (2006) and Houf and Schefer (2007)
    """

    def setUp(self):
        diameter = 7.94*milli#m
        temp = 270 #K - assumed
        amb_pressure = 101325 # assumed
        amb_temp = 293 #K - assumed
        air = Fluid(T=amb_temp, P=amb_pressure, species='air')
        orifice = Orifice(d=diameter)

        # Keys are time t=5s, 20, 40, 60, 70
        init_flowrates = {5:57.3, 20:23.17, 40:6.92, 60:2.07, 70:1.13} #g/s
        velocities = {5:1233, 20:1231, 40:1078, 60:644, 70:446} #m/s
        densities = dict([[t, init_flowrates[t]/kilo/(velocities[t]*pi/4*diameter**2)] for t in init_flowrates.keys()]) #g/m^3
        gases, flowrates = {}, {}
        for t in densities.keys():
            gases[t] = Fluid(species='H2', T=temp, rho=densities[t])

            if gases[t].P <= 2 * amb_pressure: # unchoked
                gases[t] = Fluid(species='H2', T=temp, P=amb_pressure)
                flowrates[t] = init_flowrates[t] / kilo
            else: # choked - solve for stagnation density
                def err_mdot(rho0):
                    gases[t] = Fluid(species='H2', T=temp, rho=rho0[0])
                    return NozzleFlow(gases[t], orifice, amb_pressure).mdot - init_flowrates[t] / kilo
                rho0 = optimize.root(err_mdot, densities[t])['x'][0]
                gases[t] = Fluid(species='H2', T=temp, rho=rho0)
                flowrates[t] = None

        self.flames = []
        for t in gases.keys():
            self.flames.append(Flame(gases[t], orifice, air, mdot=flowrates[t], theta0=pi/2))

        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    # Heat Flux / Normalized Distance
    def test_flux_distance_fig8(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'schefer-2006-fig8.csv'), header=2)

        times = [5, 20, 40, 60, 70]
        for i in range(len(times)):
            title = f"Schefer 2006 - Figure 8, Flux v. Normalized Distance at t={times[i]}"
            error_limits = self.limits[title]

            # Data is under x1/y1, x3/y3, etc
            idx = (2 * i) + 1
            distance = data[f'x{idx}']
            exp_flux = [d * 1e3 for d in data[f'y{idx}']] # Convert kW to W

            flame_length = self.flames[i].length()
            y = [d * flame_length for d in distance]
            x = 1.82 * np.ones_like(y)
            z = np.zeros_like(y)
            calc_flux = self.flames[i].Qrad_multi(x, y, z, RH = .9)

            # Calculate error statistics and test
            error = utils.get_error(exp_vals=exp_flux,
                                    calc_vals=calc_flux,
                                    error_limits=error_limits,
                                    units='W/m^2',
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
                                   x=distance,
                                   exp_y=exp_flux,
                                   calc_y=calc_flux,
                                   max_error=error['Max Absolute Error'],
                                   avg_error=error['Avg Absolute Error'],
                                   smape=error['SMAPE'],
                                   title=title,
                                   xlabel=r'Normalized Distance ($x/L_{\rm vis}$)',
                                   ylabel='Heat Flux (W/m$^2$)')

    # Test figure 6
    def test_flux_distance_fig6(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'houfschefer-2007-fig6.csv'), header=2)
        title = "Houf & Shefer 2007 - Figure 6, Flux v. Normalized Distance"
        error_limits = self.limits[title]

        distance = data['x1']
        exp_flux = [d * 1e3 for d in data['y1']] # Convert kW to W

        flame_length = self.flames[0].length()
        y = [d * flame_length for d in distance]
        x = 1.82*np.ones_like(y)
        z = np.zeros_like(y)
        calc_flux = self.flames[0].Qrad_multi(x, y, z, .9)

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=distance,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel=r'Normalized Distance ($x/L_{\rm vis}$)',
                               ylabel='Heat Flux (W/m$^2$)')

    # Flame length / time
    def test_length_time(self):
        data = utils.read_csv(os.path.join(DATA_LOC, 'houfschefer-2007-fig5-ADJUSTED.csv'), header=2)
        title = "Houf & Shefer 2007 - Figure 5, Flame Length v. Time"
        error_limits = self.limits[title]

        exp_time = data['x1']
        exp_length = data['y1']

        calc_time = [5, 20, 40, 60, 70]
        calc_length = [flame.length() for flame in self.flames]

        # Interpolate calculated pressure
        interp = interpolate.interp1d(exp_time, exp_length, fill_value='extrapolate')
        exp_length = interp(calc_time)

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=calc_time,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Flame Length (m)')


class Test_Imamura_2008(unittest.TestCase):
    """
    Test suite for Mass Flow Rate and Flame Rate vs. Pressure.
    Data from Imamura et al. (2008)
    """

    def setUp(self):
        amb_pressure = 101325 # assumed
        amb_temp = 293 #K - assumed
        air = Fluid(T = amb_temp, P = amb_pressure, species = 'air')

        self.data = dict({'d_mm':[1,1,1,2,2,2,2,2,2,3,3,3,3,3,3,4,4,4,4,4,4],
                          'P_MPa':[1.09,2.17,3.26,0.47,0.93,1.4,1.87,2.34,2.8,0.36,0.73,1.09,1.46,1.82,2.19,0.23,0.47,0.7,0.94,1.17,1.41],
                          'Flow':[0.000536094, 0.001031305, 0.001567413, 0.000902573, 0.001805217, 0.00272147, 0.003678693,
                                0.004581309, 0.005470301, 0.001516577, 0.003142413, 0.004713612, 0.006298463, 0.007910646,
                                0.009468193, 0.00181646, 0.003673918, 0.005449494, 0.007334199, 0.00910976, 0.010967247],
                          'Lf_m':[0.35,0.6,0.7,0.55,0.72,0.99,1.04,1.29,1.29,0.78,1.08,1.31,1.45,1.62,1.67,0.95,1.21,1.49,1.61,1.77,1.82]})

        flames =[]
        for d in range(len(self.data['d_mm'])):
            orifice = Orifice(d=self.data['d_mm'][d]*milli)
            pressure = amb_pressure + self.data['P_MPa'][d] * 1e6
            gas = Fluid(species='H2', T=amb_temp, P=pressure)
            flames.append(Flame(gas, orifice, air))

        self.data['Flames'] = flames

        # Create subsets of the data for each d value
        self.data_1mm = dict()
        self.data_2mm = dict()
        self.data_3mm = dict()
        self.data_4mm = dict()

        for key in self.data:
            self.data_1mm[key] = self.data[key][:3]
            self.data_2mm[key] = self.data[key][3:9]
            self.data_3mm[key] = self.data[key][9:15]
            self.data_4mm[key] = self.data[key][15:21]

        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    # Test Mass flowrate vs. Pressure, d = 1mm
    def test_flowrate_1mm(self):
        title = "Imamura et al. 2008 - Figure 3, Mass Flow Rate v. Pressure, d=1mm"
        error_limits = self.limits[title]

        pressure = self.data_1mm['P_MPa']
        exp_flowrate = self.data_1mm['Flow']
        calc_flowrate = [f.mass_flow_rate for f in self.data_1mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_flowrate,
                                calc_vals=calc_flowrate,
                                error_limits=error_limits,
                                units='kg/s',
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
                               x=pressure,
                               exp_y=exp_flowrate,
                               calc_y=calc_flowrate,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Mass Flow Rate (kg/s)')

    # Test Mass flowrate vs. Pressure, d = 2mm
    def test_flowrate_2mm(self):
        title = "Imamura et al. 2008 - Figure 3, Mass Flow Rate v. Pressure, d=2mm"
        error_limits = self.limits[title]

        pressure = self.data_2mm['P_MPa']
        exp_flowrate = self.data_2mm['Flow']
        calc_flowrate = [f.mass_flow_rate for f in self.data_2mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_flowrate,
                                calc_vals=calc_flowrate,
                                error_limits=error_limits,
                                units='kg/s',
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
                               x=pressure,
                               exp_y=exp_flowrate,
                               calc_y=calc_flowrate,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Mass Flow Rate (kg/s)')

    # Test Mass flowrate vs. Pressure, d = 3mm
    def test_flowrate_3mm(self):
        title = "Imamura et al. 2008 - Figure 3, Mass Flow Rate v. Pressure, d=3mm"
        error_limits = self.limits[title]

        pressure = self.data_3mm['P_MPa']
        exp_flowrate = self.data_3mm['Flow']
        calc_flowrate = [f.mass_flow_rate for f in self.data_3mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_flowrate,
                                calc_vals=calc_flowrate,
                                error_limits=error_limits,
                                units='kg/s',
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
                               x=pressure,
                               exp_y=exp_flowrate,
                               calc_y=calc_flowrate,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Mass Flow Rate (kg/s)')

    # Test Mass flowrate vs. Pressure, d = 4mm
    def test_flowrate_4mm(self):
        title = "Imamura et al. 2008 - Figure 3, Mass Flow Rate v. Pressure, d=4mm"
        error_limits = self.limits[title]

        pressure = self.data_4mm['P_MPa']
        exp_flowrate = self.data_4mm['Flow']
        calc_flowrate = [f.mass_flow_rate for f in self.data_4mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_flowrate,
                                calc_vals=calc_flowrate,
                                error_limits=error_limits,
                                units='kg/s',
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
                               x=pressure,
                               exp_y=exp_flowrate,
                               calc_y=calc_flowrate,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Mass Flow Rate (kg/s)')


    # Test Flame Length vs. Pressure, d = 1mm
    def test_flamelength_1mm(self):
        title = "Imamura et al. 2008 - Figure 3, Flame Length v. Pressure, d=1mm"
        error_limits = self.limits[title]

        pressure = self.data_1mm['P_MPa']
        exp_length = self.data_1mm['Lf_m']
        calc_length = [f.length() for f in self.data_1mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=pressure,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Flame Length (m)')

    # Test Flame Length vs. Pressure, d = 2mm
    def test_flamelength_2mm(self):
        title = "Imamura et al. 2008 - Figure 3, Flame Length v. Pressure, d=2mm"
        error_limits = self.limits[title]

        pressure = self.data_2mm['P_MPa']
        exp_length = self.data_2mm['Lf_m']
        calc_length = [f.length() for f in self.data_2mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=pressure,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Flame Length (m)')

    # Test Flame Length vs. Pressure, d = 3mm
    def test_flamelength_3mm(self):
        title = "Imamura et al. 2008 - Figure 3, Flame Length v. Pressure, d=3mm"
        error_limits = self.limits[title]

        pressure = self.data_3mm['P_MPa']
        exp_length = self.data_3mm['Lf_m']
        calc_length = [f.length() for f in self.data_3mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=pressure,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Flame Length (m)')

    # Test Flame Length vs. Pressure, d = 4mm
    def test_flamelength_4mm(self):
        title = "Imamura et al. 2008 - Figure 3, Flame Length v. Pressure, d=4mm"
        error_limits = self.limits[title]

        pressure = self.data_4mm['P_MPa']
        exp_length = self.data_4mm['Lf_m']
        calc_length = [f.length() for f in self.data_4mm['Flames']]

        # Calculate error statistics and test
        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=pressure,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (MPa)',
                               ylabel='Flame Length (m)')


class Test_Mogi_2005(unittest.TestCase):
    """
    Test suite for Heat Flux vs. Mass Flow Rate.
    Data from Mogi, Nishida, Horiguci (2005)
    """

    def setUp(self):
        amb_pressure = 101325 # assumed
        amb_temp = 293 #K - assumed
        air = Fluid(T=amb_temp, P=amb_pressure, species='air')
        self.mogi_flames = {'d':np.array([.4, .8, 2])*milli,
                            'P':np.array([35, 25, 15, 5, 1])*1e6,
                            'L':[1.5, 2.5, 3.5]}

        # Calculate flames and add to mogi_flames
        flames = []
        for diameter in self.mogi_flames['d']:
            for pressure in self.mogi_flames['P']:
                orifice = Orifice(diameter)
                gas = Fluid(species='H2', T=amb_temp, P=(amb_pressure + pressure))
                flames.append(Flame(gas, orifice, air))

        self.mogi_flames['Flames'] = flames

        self.data = utils.read_csv(os.path.join(DATA_LOC, 'mogi-2005-fig9.csv'), header=3)

        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    # Mass Flow Rate vs. Heat Flux, L = 1.5m (distance from detector to flame axis)
    def test_flowrate_flux_L1(self):
        title = "Mogi et al. 2005 - Figure 9, Heat Flux v. Mass Flow Rate, L=1.5m"
        error_limits = self.limits[title]

        L = 1.5
        calc_flowrate = []
        calc_flux = []
        for i in range(len(self.mogi_flames['Flames'])):
            flame = self.mogi_flames['Flames'][i]

            flame_length = np.linspace(0, flame.length())
            calc_flowrate.append(flame.developing_flow.orifice_flow.mdot)
            calc_flux.append(max(flame.Qrad_multi(flame_length, np.ones_like(flame_length)*1, np.ones_like(flame_length)*L, .9)))

        # Sort by flowrate (X)
        calc_flux = [flux for _, flux in sorted(zip(calc_flowrate, calc_flux))]
        calc_flowrate = sorted(calc_flowrate)

        # Get experimental data
        exp_flowrate = self.data['x1']
        exp_flowrate.extend(self.data['x2'])
        exp_flowrate.extend(self.data['x3'])

        exp_flux = self.data['y1']
        exp_flux.extend(self.data['y2'])
        exp_flux.extend(self.data['y3'])

        # Sort data
        exp_flux = [flux for _, flux in sorted(zip(exp_flowrate, exp_flux))]
        exp_flowrate = sorted(exp_flowrate)

        # Interpolate calculated pressure
        interp = interpolate.interp1d(exp_flowrate, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_flowrate)

        # Get error values and test
        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_flowrate,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Mass Flow Rate (kg/s)',
                               ylabel='Heat Flux (W/m$^2$)')

    # Mass Flow Rate vs. Heat Flux, L = 2.5m (distance from detector to flame axis)
    def test_flowrate_flux_L2(self):
        title = "Mogi et al. 2005 - Figure 9, Heat Flux v. Mass Flow Rate, L=2.5m"
        error_limits = self.limits[title]

        L = 2.5
        calc_flowrate = []
        calc_flux = []
        for i in range(len(self.mogi_flames['Flames'])):
            flame = self.mogi_flames['Flames'][i]

            l = np.linspace(0, flame.length())
            calc_flowrate.append(flame.developing_flow.orifice_flow.mdot)
            calc_flux.append(max(flame.Qrad_multi(l, np.ones_like(l)*1, np.ones_like(l)*L, .9)))

        # Sort by flowrate (X)
        calc_flux = [flux for _, flux in sorted(zip(calc_flowrate, calc_flux))]
        calc_flowrate = sorted(calc_flowrate)

        # Get experimental data
        exp_flowrate = self.data['x5']
        exp_flowrate.extend(self.data['x6'])
        exp_flowrate.extend(self.data['x7'])

        exp_flux = self.data['y5']
        exp_flux.extend(self.data['y6'])
        exp_flux.extend(self.data['y7'])

        # Sort data
        exp_flux = [flux for _, flux in sorted(zip(exp_flowrate, exp_flux))]
        exp_flowrate = sorted(exp_flowrate)

        interp = interpolate.interp1d(exp_flowrate, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_flowrate)

        # Get error values and test
        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_flowrate,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Mass Flow Rate (kg/s)',
                               ylabel='Heat Flux (W/m$^2$)')


    # Mass Flow Rate vs. Heat Flux, L = 3.5m (distance from detector to flame axis)
    def test_flowrate_flux_L3(self):
        title = "Mogi et al. 2005 - Figure 9, Heat Flux v. Mass Flow Rate, L=3.5m"
        error_limits = self.limits[title]

        L = 3.5
        calc_flowrate = []
        calc_flux = []
        for i in range(len(self.mogi_flames['Flames'])):
            flame = self.mogi_flames['Flames'][i]

            l = np.linspace(0, flame.length())
            calc_flowrate.append(flame.developing_flow.orifice_flow.mdot)
            calc_flux.append(max(flame.Qrad_multi(l, np.ones_like(l)*1, np.ones_like(l)*L, .9)))

        # Sort by flowrate (X)
        calc_flux = [flux for _, flux in sorted(zip(calc_flowrate, calc_flux))]
        calc_flowrate = sorted(calc_flowrate)

        # Get experimental data
        exp_flowrate = self.data['x9']
        exp_flowrate.extend(self.data['x10'])
        exp_flowrate.extend(self.data['x11'])

        exp_flux = self.data['y9']
        exp_flux.extend(self.data['y10'])
        exp_flux.extend(self.data['y11'])

        # Sort data
        exp_flux = [flux for _, flux in sorted(zip(exp_flowrate, exp_flux))]
        exp_flowrate = sorted(exp_flowrate)

        interp = interpolate.interp1d(exp_flowrate, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_flowrate)

        # Get error values and test
        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_flowrate,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Mass Flow Rate (kg/s)',
                               ylabel='Heat Flux (W/m$^2$)')


class Test_Proust_2011(unittest.TestCase):
    """
    Variety of tests, including Pressure and Tank measurements.
    Data from Proust, Jamois, Studer (2011)
    """

    def setUp(self):
        self.y0 = 1.5 #m
        amb_pressure = 101325 # assumed
        amb_temp = 293 #K - assumed
        tank_pressure = 90*1e6 #Pa
        tank_volume = 25*liter
        diameters = np.array([1,2,3])*milli #m

        self.pressures = np.array([900,800,700,600,500,400,300,250,200,175,150,125,100,80,60,40,30,20]) #bar
        self.pressures_2 = np.append(self.pressures, [15, 10, 5, 4, 3, 2])
        self.temp_1mm = np.array([42,40,36,27,15,-2,-18,-25,-31,-34,-37,-38,-39,-39,-37,-34,-32,-28]) #C
        self.density_1mm = np.array([42,40,37,34,31,27,23,20,17,15,13,11,9,8,6,4,3,2])
        self.temp_2mm = np.array([42,40,38,34,27,15,-4,-20,-39,-47,-54,-58,-62,-64,-64,-62,-60,-56])
        self.density_2mm = np.array([41,39,37,34,31,27,23,20,17,15,13,11,9,7,5,3,3,2])
        self.temp_3mm = np.array([46,44,42,39,35,28,15,4,-11,-20,-30,-47,-61,-70,-77,-80,-78,-76])
        self.density_3mm = np.array([42,39,37,34,31,28,23,21,17,15,14,11,9,7,5,3,3,2])

        gas = Fluid(species='H2', T = self.temp_2mm[0]+273, P=(amb_pressure+tank_pressure))
        source = Source(tank_volume, gas)
        self.air = Fluid(T = amb_temp, P = amb_pressure, species = 'air')

        self.blowdown_data = []
        for d in diameters:
            orifice = Orifice(d)
            self.blowdown_data.append(source.empty(orifice))

        # Importing figure data
        self.fig4data = utils.read_csv(os.path.join(DATA_LOC, 'proust-2011-fig4.csv'), header=1)
        self.fig8data = utils.read_csv(os.path.join(DATA_LOC, 'proust-2011-fig8.csv'), header=2)
        self.fig10data = utils.read_csv(os.path.join(DATA_LOC, 'proust-2011-fig10.csv'), header=2)

        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    # Pressure vs. Time, fig. 4
    def test_pressure_time(self):
        title = "Proust et al. 2011 - Pressure v. Time, d=2mm"
        error_limits = self.limits[title]

        # Comparing 2mm data
        exp_time = self.fig4data['x3']
        exp_pressure = self.fig4data['y3']

        calc_time = self.blowdown_data[1][2]
        calc_pressure = [g.P*1e-5 for g in self.blowdown_data[1][1]]
        interp = interpolate.interp1d(exp_time, exp_pressure, fill_value='extrapolate')
        exp_pressure = interp(calc_time)

        # Get error values and test
        error = utils.get_error(exp_vals=exp_pressure,
                                calc_vals=calc_pressure,
                                error_limits=error_limits,
                                units='bar',
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
                               x=calc_time,
                               exp_y=exp_pressure,
                               calc_y=calc_pressure,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Pressure (bar)')


    # Tank Pressure vs. Temperature, 1mm, fig. 4a
    def test_temp_pressure_1mm(self):
        exp_pressure = self.pressures
        exp_temp = self.temp_1mm

        calc_pressure = [g.P*1e-5 for g in self.blowdown_data[0][1]]
        calc_temp = [g.T-273 for g in self.blowdown_data[0][1]]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_temp), fill_value='extrapolate')
        exp_temp = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 4a, Temperature v. Pressure, d=1mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_temp,
                                calc_vals=calc_temp,
                                error_limits=error_limits,
                                units='C',
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
                               x=calc_pressure,
                               exp_y=exp_temp,
                               calc_y=calc_temp,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Tank Pressure (bar)',
                               ylabel='Tank Temperature (C)')

    # Tank Pressure vs. Tank Temperature, 2mm, fig. 4a
    def test_temp_pressure_2mm(self):
        exp_pressure = self.pressures
        exp_temp = self.temp_2mm

        calc_pressure = [g.P*1e-5 for g in self.blowdown_data[1][1]]
        calc_temp = [g.T-273 for g in self.blowdown_data[1][1]]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_temp), fill_value='extrapolate')
        exp_temp = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 4a, Temperature v. Pressure, d=2mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_temp,
                                calc_vals=calc_temp,
                                error_limits=error_limits,
                                units='C',
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
                               x=calc_pressure,
                               exp_y=exp_temp,
                               calc_y=calc_temp,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Tank Pressure (bar)',
                               ylabel='Tank Temperature (C)')

    # Tank Pressure vs. Tank Temperature, 3mm, fig. 4a
    def test_temp_pressure_3mm(self):
        exp_pressure = self.pressures
        exp_temp = self.temp_3mm

        calc_pressure = [g.P*1e-5 for g in self.blowdown_data[2][1]]
        calc_temp = [g.T-273 for g in self.blowdown_data[2][1]]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_temp), fill_value='extrapolate')
        exp_temp = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 4a, Temperature v. Pressure, d=3mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_temp,
                                calc_vals=calc_temp,
                                error_limits=error_limits,
                                units='C',
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
                               x=calc_pressure,
                               exp_y=exp_temp,
                               calc_y=calc_temp,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Tank Pressure (bar)',
                               ylabel='Tank Temperature (C)')


    # Tank Pressure vs. Density, 1mm, fig. 5
    def test_density_pressure_1mm(self):
        exp_pressure = self.pressures
        exp_density = self.density_1mm

        calc_pressure = [g.P*1e-5 for g in self.blowdown_data[0][1]]
        calc_density = [g.rho for g in self.blowdown_data[0][1]]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_density), fill_value='extrapolate')
        exp_density = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 5, Density v. Pressure, d=1mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_density,
                                calc_vals=calc_density,
                                error_limits=error_limits,
                                units='kg/m^2',
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
                               x=calc_pressure,
                               exp_y=exp_density,
                               calc_y=calc_density,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Tank Pressure (bar)',
                               ylabel='Density (kg/$m^2$)')

    # Tank Pressure vs. Density, 2mm, fig. 5
    def test_density_pressure_2mm(self):
        exp_pressure = self.pressures
        exp_density = self.density_1mm

        calc_pressure = [g.P*1e-5 for g in self.blowdown_data[1][1]]
        calc_density = [g.rho for g in self.blowdown_data[1][1]]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_density), fill_value='extrapolate')
        exp_density = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 5, Density v. Pressure, d=2mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_density,
                                calc_vals=calc_density,
                                error_limits=error_limits,
                                units='kg/m^2',
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
                               x=calc_pressure,
                               exp_y=exp_density,
                               calc_y=calc_density,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Tank Pressure (bar)',
                               ylabel='Density (kg/$m^2$)')


    # Tank Pressure vs. Density, 3mm, fig. 5
    def test_density_pressure_3mm(self):
        exp_pressure = self.pressures
        exp_density = self.density_1mm

        calc_pressure = [g.P*1e-5 for g in self.blowdown_data[2][1]]
        calc_density = [g.rho for g in self.blowdown_data[2][1]]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_density), fill_value='extrapolate')
        exp_density = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 5, Density v. Pressure, d=3mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_density,
                                calc_vals=calc_density,
                                error_limits=error_limits,
                                units='kg/m^2',
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
                               x=calc_pressure,
                               exp_y=exp_density,
                               calc_y=calc_density,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Tank Pressure (bar)',
                               ylabel='Density (kg/$m^2$)')


    # Pressure vs. Flame Length, 1mm, fig. 8
    def test_length_pressure_1mm(self):
        # Build flames
        temps = self.temp_1mm + 273
        orifice = Orifice(1*milli)

        flames = []
        for i, p in enumerate(self.pressures):
            gas = Fluid(species='H2', T = temps[i], P = p*1e5)
            flames.append(Flame(gas, orifice, self.air, y0=self.y0))

        exp_pressure = self.fig8data['x3']
        exp_length = self.fig8data['y3']

        calc_pressure = self.pressures
        calc_length = [f.length() for f in flames]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_length), fill_value='extrapolate')
        exp_length = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 8, Flame Length v. Pressure, d=1mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=calc_pressure,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (bar)',
                               ylabel='Flame Length (m)')

    # Pressure vs. Flame Length, 2mm, fig. 8
    def test_length_pressure_2mm(self):
        # Build flames
        temps = self.temp_2mm + 273
        orifice = Orifice(2*milli)

        flames = []
        for i, p in enumerate(self.pressures):
            gas = Fluid(species='H2', T = temps[i], P = p*1e5)
            flames.append(Flame(gas, orifice, self.air, y0=self.y0))

        exp_pressure = self.fig8data['x2']
        exp_length = self.fig8data['y2']

        calc_pressure = self.pressures
        calc_length = [f.length() for f in flames]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_length), fill_value='extrapolate')
        exp_length = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 8, Flame Length v. Pressure, d=2mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=calc_pressure,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (bar)',
                               ylabel='Flame Length (m)')

    # Pressure vs. Flame Length, 3mm, fig. 8
    def test_length_pressure_3mm(self):
        # Build flames
        temps = self.temp_3mm + 273
        orifice = Orifice(3*milli)

        flames = []
        for i, p in enumerate(self.pressures):
            gas = Fluid(species='H2', T = temps[i], P = p*1e5)
            flames.append(Flame(gas, orifice, self.air, y0=self.y0))

        exp_pressure = self.fig8data['x1']
        exp_length = self.fig8data['y1']

        calc_pressure = self.pressures
        calc_length = [f.length() for f in flames]

        interp = interpolate.interp1d(np.flip(exp_pressure), np.flip(exp_length), fill_value='extrapolate')
        exp_length = interp(calc_pressure)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 8, Flame Length v. Pressure, d=3mm"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_length,
                                calc_vals=calc_length,
                                error_limits=error_limits,
                                units='m',
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
                               x=calc_pressure,
                               exp_y=exp_length,
                               calc_y=calc_length,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Pressure (bar)',
                               ylabel='Flame Length (m)')


    # Heat Flux over time #1, fig. 10
    def test_flux_1(self):
        # Get experimental values
        exp_time = self.fig10data['x1']
        exp_flux = self.fig10data['y1']

        # Get Hyram calculated values
        calc_time = [np.interp(P*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], self.blowdown_data[1][2][::-1]) for P in self.pressures_2]
        orifice = Orifice(2*milli) #d = 2mm
        rel_humidity = 0.9

        # Generate flames and calculate heat flux
        rad_loc = 1
        x = rad_loc * 1.2
        y = self.y0
        z = rad_loc
        calc_flux = []
        for i, p in enumerate(self.pressures_2):
            gas = Fluid(species='H2', T = np.interp(self.pressures_2[i]*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], [g.T for g in self.blowdown_data[1][1]][::-1]), P=p*1e5)
            flame = Flame(gas, orifice, self.air, y0=self.y0)
            calc_flux.append(flame.Qrad_multi(x, y, z, rel_humidity))

        interp = interpolate.interp1d(exp_time, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_time)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 10, Heat Flux v. Time, Fluxmeter 1"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_time,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Heat Flux (W/$m^2$)')

    # Heat Flux over time #2, fig. 10
    def test_flux_2(self):
        # Get experimental values
        exp_time = self.fig10data['x2']
        exp_flux = self.fig10data['y2']

        # Get Hyram calculated values
        calc_time = [np.interp(P*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], self.blowdown_data[1][2][::-1]) for P in self.pressures_2]
        orifice = Orifice(2*milli) #d = 2mm
        rel_humidity = 0.9

        # Generate flames and calculate heat flux
        rad_loc = 1.5
        x = rad_loc * 1.2
        y = self.y0
        z = rad_loc
        calc_flux = []
        for i, p in enumerate(self.pressures_2):
            gas = Fluid(species='H2', T = np.interp(self.pressures_2[i]*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], [g.T for g in self.blowdown_data[1][1]][::-1]), P=p*1e5)
            flame = Flame(gas, orifice, self.air, y0=self.y0)
            calc_flux.append(flame.Qrad_multi(x, y, z, rel_humidity))

        interp = interpolate.interp1d(exp_time, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_time)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 10, Heat Flux v. Time, Fluxmeter 2"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_time,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Heat Flux (W/$m^2$)')

    # Heat Flux over time #3, fig. 10
    def test_flux_3(self):
        # Get experimental values
        exp_time = self.fig10data['x3']
        exp_flux = self.fig10data['y3']

        # Get Hyram calculated values
        calc_time = [np.interp(P*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], self.blowdown_data[1][2][::-1]) for P in self.pressures_2]
        orifice = Orifice(2*milli) #d = 2mm
        rel_humidity = 0.9

        # Generate flames and calculate heat flux
        rad_loc = 2
        x = rad_loc * 1.2
        y = self.y0
        z = rad_loc
        calc_flux = []
        for i, p in enumerate(self.pressures_2):
            gas = Fluid(species='H2', T = np.interp(self.pressures_2[i]*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], [g.T for g in self.blowdown_data[1][1]][::-1]), P=p*1e5)
            flame = Flame(gas, orifice, self.air, y0=self.y0)
            calc_flux.append(flame.Qrad_multi(x, y, z, rel_humidity))

        interp = interpolate.interp1d(exp_time, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_time)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 10, Heat Flux v. Time, Fluxmeter 3"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_time,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Heat Flux (W/$m^2$)')


    # Heat Flux over time #4, fig. 10
    def test_flux_4(self):
        # Get experimental values
        exp_time = self.fig10data['x4']
        exp_flux = self.fig10data['y4']

        # Get Hyram calculated values
        calc_time = [np.interp(P*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], self.blowdown_data[1][2][::-1]) for P in self.pressures_2]
        orifice = Orifice(2*milli) #d = 2mm
        rel_humidity = 0.9

        # Generate flames and calculate heat flux
        rad_loc = 3
        x = rad_loc * 1.2
        y = self.y0
        z = rad_loc
        calc_flux = []
        for i, p in enumerate(self.pressures_2):
            gas = Fluid(species='H2', T = np.interp(self.pressures_2[i]*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], [g.T for g in self.blowdown_data[1][1]][::-1]), P=p*1e5)
            flame = Flame(gas, orifice, self.air, y0=self.y0)
            calc_flux.append(flame.Qrad_multi(x, y, z, rel_humidity))

        interp = interpolate.interp1d(exp_time, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_time)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 10, Heat Flux v. Time, Fluxmeter 4"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_time,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Heat Flux (W/$m^2$)')


    # Heat Flux over time #5, fig. 10
    def test_flux_5(self):
        # Get experimental values
        exp_time = self.fig10data['x5']
        exp_flux = self.fig10data['y5']

        # Get Hyram calculated values
        calc_time = [np.interp(P*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], self.blowdown_data[1][2][::-1]) for P in self.pressures_2]
        orifice = Orifice(2*milli) #d = 2mm
        rel_humidity = 0.9

        # Generate flames and calculate heat flux
        rad_loc = 4
        x = rad_loc * 1.2
        y = self.y0
        z = rad_loc
        calc_flux = []
        for i, p in enumerate(self.pressures_2):
            gas = Fluid(species='H2', T = np.interp(self.pressures_2[i]*1e5, [g.P for g in self.blowdown_data[1][1]][::-1], [g.T for g in self.blowdown_data[1][1]][::-1]), P=p*1e5)
            flame = Flame(gas, orifice, self.air, y0=self.y0)
            calc_flux.append(flame.Qrad_multi(x, y, z, rel_humidity))

        interp = interpolate.interp1d(exp_time, exp_flux, fill_value='extrapolate')
        exp_flux = interp(calc_time)

        # Get error values and test
        title = "Proust et al. 2011 - Figure 10, Heat Flux v. Time, Fluxmeter 5"
        error_limits = self.limits[title]

        error = utils.get_error(exp_vals=exp_flux,
                                calc_vals=calc_flux,
                                error_limits=error_limits,
                                units='W/m^2',
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
                               x=calc_time,
                               exp_y=exp_flux,
                               calc_y=calc_flux,
                               max_error=error['Max Absolute Error'],
                               avg_error=error['Avg Absolute Error'],
                               smape=error['SMAPE'],
                               title=title,
                               xlabel='Time (s)',
                               ylabel='Heat Flux (W/$m^2$)')
