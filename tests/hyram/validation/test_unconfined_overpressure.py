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

from hyram.phys import Fluid, Orifice, Jet, BST_method, TNT_method, Bauwens_method
from hyram.phys._unconfined_overpressure import JallaisOverpressureH2

from .data import unconfined_overpressure_data
from . import utils


# Flags to enable command line output, pyplots, and text file output
VERBOSE = False
CREATE_PLOTS = False
CREATE_OUTPUT = False

# Absolute paths to input/output data
DATA_LOC = os.path.join(os.path.dirname(__file__), 'data')
LIMITS_FILE = os.path.join(DATA_LOC, 'Limits-unconfined_overpressure.json')
OUTPUT_LOC = os.path.join('out', 'validation-unconfined_overpressure')


amb_species = "air"
rel_species = "H2"
amb_temp = 297
amb_pressure = 101325
ambient = Fluid(species=amb_species, T=amb_temp, P=amb_pressure)
method_names = ['BST', 'TNT', 'Bauwens', 'Jallais']


def define_release_state(rel_pressure, rel_temp, nozzle_dia):
    release_fluid = Fluid(species=rel_species, P=rel_pressure, T=rel_temp)
    orifice = Orifice(nozzle_dia)

    jet = Jet(release_fluid,
            orifice,
            ambient,
            )

    return jet


def calc_overpressure(jet_object, distances, equivalence_factor=0.03):
    mass_flow = jet_object.developing_flow.mass_flow_rate
    if mass_flow > 1:
        mach_flame_speed=0.7
    elif 0.5 <= mass_flow <= 1:
        mach_flame_speed=0.35
    else:
        mach_flame_speed=0.2

    # Define methods
    bst_method = BST_method(jet_object=jet_object,
                            mach_flame_speed=mach_flame_speed,
                            origin_at_orifice=True)
    tnt_method = TNT_method(jet_object=jet_object,
                            equivalence_factor=equivalence_factor,
                            origin_at_orifice=True)
    bauwens_method = Bauwens_method(jet_object=jet_object,
                                    origin_at_orifice=True)
    jallais_method = JallaisOverpressureH2(jet_object=jet_object,
                                            origin_at_orifice=True)

    # Calculate overpressure
    locs = list(zip(distances, np.zeros_like(distances), np.zeros_like(distances)))
    op_bst = bst_method.calc_overpressure(locs)
    op_tnt = tnt_method.calc_overpressure(locs)
    op_bauwens = bauwens_method.calc_overpressure(locs)
    op_jallais = jallais_method.calc_overpressure(locs)

    return op_bst, op_tnt, op_bauwens, op_jallais


def plot_errors(errors, exp_op, calc_ops, distances, title):
    for i in range (len(method_names)):
        utils.create_plots(output_dir=OUTPUT_LOC,
                            x=distances,
                            exp_y=exp_op,
                            calc_y=calc_ops[i],
                            max_error=errors[i]['Max Absolute Error'],
                            avg_error=errors[i]['Avg Absolute Error'],
                            smape=errors[i]['SMAPE'],
                            title=f'{title}, {method_names[i]} method',
                            xlabel='Distance from Nozzle (m)',
                            ylabel='Overpressure (Pa)')


def check_overpressure_kobayashi_tbl1(self, nozzle, title, error_limits):
    calc_ops = []
    nozzle_dia = nozzle["Nozzle diameter"]
    exp_op = np.array(nozzle['Overpressure']) * 1000 # kPa -> Pa
    for i in range(len(nozzle['Overpressure'])):
        jet = self.define_release_state(
            nozzle['Pressure'][i] * 1000 + self.amb_pressure, # kPa -> Pa
            nozzle['Temperature'][i],
            nozzle_dia)
        calc_ops.append(list(
            np.concatenate(self.calc_overpressure(
                jet_object=jet,
                distances=[nozzle['Distance'][i]]
                ))))
    calc_ops = list(map(list, zip(*calc_ops)))
    errors = self.check_errors(error_limits=error_limits,
                                exp_op=exp_op,
                                calc_ops=calc_ops,
                                title=title)
    if CREATE_PLOTS:
        self.plot_errors(errors=errors,
                            exp_op=exp_op,
                            calc_ops=calc_ops,
                            distances=nozzle['Distance'],
                            title=title)


class Test_Bauwens_2019(unittest.TestCase):
    """
    Test overpressure calculations against distances from origin for the
    BST, TNT, Bauwens, and Jallais overpressure calculation methods. Data from
    Bauwens and Dorofeev (2019), Figure 9.
    """

    def setUp(self):
        self.rel_species = rel_species
        self.amb_pressure = amb_pressure
        self.ambient = ambient
        self.method_names = method_names
        self.rel_pressure = 6e6
        self.rel_temp = 300
        self.title = 'Bauwens and Dorofeev 2019 - Figure 9'
        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_overpressure(self):
        for nozzle_name, nozzle in unconfined_overpressure_data.bauwens_2019_data.items():
            nozzle_dia = nozzle["Nozzle diameter"]
            exp_op = np.array(nozzle["Overpressure"]) * 1000 # kPa -> Pa
            jet = define_release_state(rel_pressure=self.rel_pressure,
                                       rel_temp=self.rel_temp,
                                       nozzle_dia=nozzle_dia
                                            )
            calc_ops = calc_overpressure(jet_object=jet,
                                         distances=nozzle["Distance"])
            errors = []
            for i in range(len(method_names)):
                test_title = f'{self.title}, {nozzle_name}, {method_names[i]} method'
                error_limits = self.limits[test_title]
                errors.append(utils.get_error(
                    exp_vals=exp_op,
                    calc_vals=calc_ops[i],
                    error_limits=error_limits,
                    units='Pa',
                    msg=test_title,
                    output_dir=OUTPUT_LOC,
                    create_output=CREATE_OUTPUT,
                    verbose=VERBOSE))

                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Absolute Error'], 2),
                                         round(error_limits['Max Absolute Error'], 2),
                                         f"{test_title}: Maximum Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Absolute Error'], 2),
                                         round(error_limits['Avg Absolute Error'], 2),
                                         f"{test_title}: Average Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Percent Error'], 2),
                                         round(error_limits['Max Percent Error'], 2),
                                         f"{test_title}: Maximum Percent Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Percent Error'], 2),
                                         round(error_limits['Avg Percent Error'], 2),
                                         f"{test_title}: Average Percent Error out of range")
                with self.subTest():
                    self.assertGreaterEqual(round(errors[i]['R2'], 2),
                                            round(error_limits['R2'], 2),
                                            f"{test_title}: R Squared Value out of range")


class Test_Jallais_2018(unittest.TestCase):
    """
    Test overpressure calculations against distances from origin for the
    BST, TNT, Bauwens, and Jallais overpressure calculation methods. Data from
    Jallais et al. (2018), Figure 22.
    """

    def setUp(self):
        self.rel_species = rel_species
        self.amb_pressure = amb_pressure
        self.ambient = ambient
        self.method_names = method_names
        self.rel_pressure = 6e6 + amb_pressure
        self.rel_temp = 279.95
        self.title = 'Jallais et al. 2018 - Figure 22'
        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_overpressure(self):
        for nozzle_name, nozzle in unconfined_overpressure_data.jallais_2018_data.items():
            nozzle_dia = nozzle["Nozzle diameter"]
            exp_op = np.array(nozzle["Overpressure"]) * 1000 # kPa -> Pa
            jet = define_release_state(rel_pressure=self.rel_pressure,
                                       rel_temp=self.rel_temp,
                                       nozzle_dia=nozzle_dia
                                            )
            calc_ops = calc_overpressure(jet_object=jet,
                                         distances=nozzle["Distance"])
            errors = []
            for i in range(len(method_names)):
                test_title = f'{self.title}, {nozzle_name}, {method_names[i]} method'
                error_limits = self.limits[test_title]
                errors.append(utils.get_error(
                    exp_vals=exp_op,
                    calc_vals=calc_ops[i],
                    error_limits=error_limits,
                    units='Pa',
                    msg=test_title,
                    output_dir=OUTPUT_LOC,
                    create_output=CREATE_OUTPUT,
                    verbose=VERBOSE))

                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Absolute Error'], 2),
                                         round(error_limits['Max Absolute Error'], 2),
                                         f"{test_title}: Maximum Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Absolute Error'], 2),
                                         round(error_limits['Avg Absolute Error'], 2),
                                         f"{test_title}: Average Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Percent Error'], 2),
                                         round(error_limits['Max Percent Error'], 2),
                                         f"{test_title}: Maximum Percent Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Percent Error'], 2),
                                         round(error_limits['Avg Percent Error'], 2),
                                         f"{test_title}: Average Percent Error out of range")
                with self.subTest():
                    self.assertGreaterEqual(round(errors[i]['R2'], 2),
                                            round(error_limits['R2'], 2),
                                            f"{test_title}: R Squared Value out of range")


class Test_Kobayashi_2020(unittest.TestCase):
    """
    Test overpressure calculations against distances from origin for the
    BST, TNT, Bauwens, and Jallais overpressure calculation methods. Data from
    Kobayashi et al. (2020), Figure 8 and Table 1.
    """

    def setUp(self):
        self.rel_species = rel_species
        self.amb_pressure = amb_pressure
        self.ambient = ambient
        self.method_names = method_names
        self.title = 'Kobayashi et al. 2020'
        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_overpressure_fig8(self):
        rel_pressure = 5.90e7 + amb_pressure
        rel_temp = 65
        title = f'{self.title} - Figure 8'
        for nozzle_name, nozzle in unconfined_overpressure_data.kobayashi_2020_fig8_data.items():
            nozzle_dia = nozzle["Nozzle diameter"]
            exp_op = np.array(nozzle["Overpressure"]) * 1000 # kPa -> Pa
            jet = define_release_state(rel_pressure=rel_pressure,
                                       rel_temp=rel_temp,
                                       nozzle_dia=nozzle_dia
                                            )
            calc_ops = calc_overpressure(jet_object=jet,
                                         distances=nozzle["Distance"])
            errors = []
            for i in range(len(method_names)):
                test_title = f'{title}, {nozzle_name}, {method_names[i]} method'
                error_limits = self.limits[test_title]
                errors.append(utils.get_error(
                    exp_vals=exp_op,
                    calc_vals=calc_ops[i],
                    error_limits=error_limits,
                    units='Pa',
                    msg=test_title,
                    output_dir=OUTPUT_LOC,
                    create_output=CREATE_OUTPUT,
                    verbose=VERBOSE))

                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Absolute Error'], 2),
                                         round(error_limits['Max Absolute Error'], 2),
                                         f"{test_title}: Maximum Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Absolute Error'], 2),
                                         round(error_limits['Avg Absolute Error'], 2),
                                         f"{test_title}: Average Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Percent Error'], 2),
                                         round(error_limits['Max Percent Error'], 2),
                                         f"{test_title}: Maximum Percent Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Percent Error'], 2),
                                         round(error_limits['Avg Percent Error'], 2),
                                         f"{test_title}: Average Percent Error out of range")
                with self.subTest():
                    self.assertGreaterEqual(round(errors[i]['R2'], 2),
                                            round(error_limits['R2'], 2),
                                            f"{test_title}: R Squared Value out of range")

    def test_kobayashi_2020_tbl1(self):
        title = f'{self.title} - Table 1'
        for nozzle_name, nozzle in unconfined_overpressure_data.kobayashi_2020_tbl1_data.items():
            calc_ops = []
            nozzle_dia = nozzle["Nozzle diameter"]
            exp_op = np.array(nozzle["Overpressure"]) * 1000 # kPa -> Pa
            for i in range(len(nozzle['Overpressure'])):
                jet = define_release_state(
                    nozzle['Pressure'][i] * 1000 + self.amb_pressure, # kPa -> Pa
                    nozzle['Temperature'][i],
                    nozzle_dia)
                calc_ops.append(list(
                    np.concatenate(calc_overpressure(
                        jet_object=jet,
                        distances=[nozzle['Distance'][i]]
                        ))))
            calc_ops = list(map(list, zip(*calc_ops)))
            errors = []
            for i in range(len(method_names)):
                test_title = f'{title}, {nozzle_name}, {method_names[i]} method'
                error_limits = self.limits[test_title]
                errors.append(utils.get_error(
                    exp_vals=exp_op,
                    calc_vals=calc_ops[i],
                    error_limits=error_limits,
                    units='Pa',
                    msg=test_title,
                    output_dir=OUTPUT_LOC,
                    create_output=CREATE_OUTPUT,
                    verbose=VERBOSE))

                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Absolute Error'], 2),
                                         round(error_limits['Max Absolute Error'], 2),
                                         f"{test_title}: Maximum Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Absolute Error'], 2),
                                         round(error_limits['Avg Absolute Error'], 2),
                                         f"{test_title}: Average Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Percent Error'], 2),
                                         round(error_limits['Max Percent Error'], 2),
                                         f"{test_title}: Maximum Percent Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Percent Error'], 2),
                                         round(error_limits['Avg Percent Error'], 2),
                                         f"{test_title}: Average Percent Error out of range")
                with self.subTest():
                    self.assertGreaterEqual(round(errors[i]['R2'], 2),
                                            round(error_limits['R2'], 2),
                                            f"{test_title}: R Squared Value out of range")


class Test_Royle_2011(unittest.TestCase):
    """
    Test overpressure calculations against distances from origin for the
    BST, TNT, Bauwens, and Jallais overpressure calculation methods. Data from
    Royle and Willoughby (2011), Table 1.
    """

    def setUp(self):
        self.rel_species = rel_species
        self.amb_pressure = amb_pressure
        self.ambient = ambient
        self.method_names = method_names
        self.rel_pressure = 2.05e7
        self.rel_temp = 293.15
        self.title = 'Royle and Willoughby 2011 - Table 1'
        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_overpressure(self):
        for nozzle_name, nozzle in unconfined_overpressure_data.royle_2011_data.items():
            nozzle_dia = nozzle["Nozzle diameter"]
            exp_op = np.array(nozzle["Overpressure"]) * 1000 # kPa -> Pa
            jet = define_release_state(rel_pressure=self.rel_pressure,
                                       rel_temp=self.rel_temp,
                                       nozzle_dia=nozzle_dia
                                            )
            calc_ops = calc_overpressure(jet_object=jet,
                                         distances=nozzle["Distance"])
            errors = []
            for i in range(len(method_names)):
                test_title = f'{self.title}, {nozzle_name}, {method_names[i]} method'
                error_limits = self.limits[test_title]
                errors.append(utils.get_error(
                    exp_vals=exp_op,
                    calc_vals=calc_ops[i],
                    error_limits=error_limits,
                    units='Pa',
                    msg=test_title,
                    output_dir=OUTPUT_LOC,
                    create_output=CREATE_OUTPUT,
                    verbose=VERBOSE))

                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Absolute Error'], 2),
                                         round(error_limits['Max Absolute Error'], 2),
                                         f"{test_title}: Maximum Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Absolute Error'], 2),
                                         round(error_limits['Avg Absolute Error'], 2),
                                         f"{test_title}: Average Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Percent Error'], 2),
                                         round(error_limits['Max Percent Error'], 2),
                                         f"{test_title}: Maximum Percent Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Percent Error'], 2),
                                         round(error_limits['Avg Percent Error'], 2),
                                         f"{test_title}: Average Percent Error out of range")
                with self.subTest():
                    self.assertGreaterEqual(round(errors[i]['R2'], 2),
                                            round(error_limits['R2'], 2),
                                            f"{test_title}: R Squared Value out of range")


class Test_Takeno_2007(unittest.TestCase):
    """
    Test overpressure calculations against distances from origin for the
    BST, TNT, Bauwens, and Jallais overpressure calculation methods. Data from
    Takeno et al. (2007), Figures 13 and 16.
    """

    def setUp(self):
        self.rel_species = rel_species
        self.amb_pressure = amb_pressure
        self.ambient = ambient
        self.method_names = method_names
        self.rel_pressure = 4e7
        self.rel_temp = 293.15
        self.title = 'Takeno et al. 2007 - Figures 13 & 16'
        # Get error limits
        with open(LIMITS_FILE) as f:
            self.limits = json.load(f)[__class__.__name__]

    def test_overpressure(self):
        for nozzle_name, nozzle in unconfined_overpressure_data.takeno_2007_data.items():
            nozzle_dia = nozzle["Nozzle diameter"]
            exp_op = np.array(nozzle["Overpressure"]) * 1000 # kPa -> Pa
            jet = define_release_state(rel_pressure=self.rel_pressure,
                                       rel_temp=self.rel_temp,
                                       nozzle_dia=nozzle_dia
                                            )
            calc_ops = calc_overpressure(jet_object=jet,
                                         distances=nozzle["Distance"])
            errors = []
            for i in range(len(method_names)):
                test_title = f'{self.title}, {nozzle_name}, {method_names[i]} method'
                error_limits = self.limits[test_title]
                errors.append(utils.get_error(
                    exp_vals=exp_op,
                    calc_vals=calc_ops[i],
                    error_limits=error_limits,
                    units='Pa',
                    msg=test_title,
                    output_dir=OUTPUT_LOC,
                    create_output=CREATE_OUTPUT,
                    verbose=VERBOSE))

                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Absolute Error'], 2),
                                         round(error_limits['Max Absolute Error'], 2),
                                         f"{test_title}: Maximum Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Absolute Error'], 2),
                                         round(error_limits['Avg Absolute Error'], 2),
                                         f"{test_title}: Average Absolute Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Max Percent Error'], 2),
                                         round(error_limits['Max Percent Error'], 2),
                                         f"{test_title}: Maximum Percent Error out of range")
                with self.subTest():
                    self.assertLessEqual(round(errors[i]['Avg Percent Error'], 2),
                                         round(error_limits['Avg Percent Error'], 2),
                                         f"{test_title}: Average Percent Error out of range")
                with self.subTest():
                    self.assertGreaterEqual(round(errors[i]['R2'], 2),
                                            round(error_limits['R2'], 2),
                                            f"{test_title}: R Squared Value out of range")
