"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import os
import unittest

from hyramgui.models import models
from hyramgui.models.enums import FuelMixType
from hyramgui import app_settings


VERBOSE = False


class StateTestCase(unittest.TestCase):
    def setUp(self):
        app_settings.init()

    def test_initial_values(self):
        state = models.State()
        self.assertEqual(state.fuel_mix.value, FuelMixType.h2)
        self.assertEqual(state.conc_h2.value, 100)
        self.assertEqual(state.conc_ch4.value, 0)
        self.assertEqual(state.conc_pro.value, 0)


class StateSpeciesTestCase(unittest.TestCase):
    def setUp(self):
        app_settings.init()

    def test_species_output_h2(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.h2)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 1)
        self.assertTrue('hydrogen' in species_dict)
        self.assertEqual(species_dict['hydrogen'], 100)

    def test_species_output_ch4(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.ch4)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 1)
        self.assertTrue('methane' in species_dict)
        self.assertEqual(species_dict['methane'], 100)

    def test_species_output_c3h8(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.c3h8)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 1)
        self.assertTrue('propane' in species_dict)
        self.assertEqual(species_dict['propane'], 100)

    def test_species_output_manual_mix(self):
        state = models.State()
        state.conc_h2.value = 25
        state.conc_ch4.value = 25
        state.conc_pro.value = 50
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 3)
        self.assertTrue('hydrogen' in species_dict)
        self.assertEqual(species_dict['hydrogen'], 25)
        self.assertTrue('methane' in species_dict)
        self.assertEqual(species_dict['methane'], 25)
        self.assertTrue('propane' in species_dict)
        self.assertEqual(species_dict['propane'], 50)

    def test_species_output_nist1(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.nist1)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 10)
        self.assertTrue('hydrogen' not in species_dict)
        self.assertTrue('propane' in species_dict)
        self.assertEqual(species_dict['methane'], 96.521)

    def test_species_output_nist2(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.nist2)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 10)
        self.assertTrue('hydrogen' not in species_dict)
        self.assertTrue('propane' in species_dict)
        self.assertEqual(species_dict['methane'], 90.673)

    def test_species_output_rg2(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.rg2)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 9)
        self.assertTrue('hydrogen' not in species_dict)
        self.assertTrue('propane' in species_dict)
        self.assertEqual(species_dict['methane'], 85.905)

    def test_species_output_gu1(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.gu1)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 7)
        self.assertTrue('hydrogen' not in species_dict)
        self.assertTrue('propane' in species_dict)
        self.assertEqual(species_dict['methane'], 81.441)

    def test_species_output_gu2(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.gu2)
        species_dict = state._get_conc_dict()

        self.assertEqual(len(species_dict), 7)
        self.assertTrue('hydrogen' not in species_dict)
        self.assertTrue('propane' in species_dict)
        self.assertEqual(species_dict['methane'], 81.212)

    def test_species_change_conc_to_100_h2(self):
        """Updates h2 concentration but doesn't clear others"""
        state = models.State()
        state.set_fuel_mix(FuelMixType.gu2)
        state.conc_h2.value = 100

        species_dict = state._get_conc_dict()
        self.assertEqual(len(species_dict), 8)
        self.assertTrue('hydrogen' in species_dict)
        self.assertEqual(species_dict['hydrogen'], 100)
        self.assertEqual(state.conc_h2.value, 100)
        self.assertEqual(state.conc_ch4.value, 81.212)

    def test_species_change_conc_to_50_ch4(self):
        state = models.State()
        state.set_fuel_mix(FuelMixType.nist1)
        state.conc_ch4.value = 50

        species_dict = state._get_conc_dict()
        self.assertEqual(len(species_dict), 10)
        self.assertTrue('hydrogen' not in species_dict)
        self.assertEqual(species_dict['methane'], 50)
        self.assertEqual(state.conc_ch4.value, 50)
        self.assertEqual(state.conc_h2.value, 0)
        self.assertEqual(state.conc_pro.value, 0.46)

