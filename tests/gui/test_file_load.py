"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import os
import unittest
from pathlib import Path

from hyramgui.models import models
from hyramgui.models.enums import FuelMixType
from hyramgui import app_settings


VERBOSE = False


class FileLoadTestCase(unittest.TestCase):
    def setUp(self):
        app_settings.BASE_DIR = Path(os.path.dirname(os.path.abspath(__file__)))
        app_settings.SESSION_DIR = app_settings.BASE_DIR.joinpath('out')
        app_settings.init()
        self.fixture_dir = app_settings.BASE_DIR.joinpath('fixtures')

    def test_initial_component_counts(self):
        state = models.State()
        self.assertEqual(state.n_compressors.value, 1)
        self.assertEqual(state.n_vessels.value, 2)
        self.assertEqual(state.n_valves.value, 7)
        self.assertEqual(state.n_extra1.value, 0)

    def test_load_custom_counts(self):
        state = models.State()
        state.load_data_from_file(self.fixture_dir.joinpath('qra_component_counts_all4.hrm'))
        self.assertEqual(state.n_compressors.value, 4)
        self.assertEqual(state.n_vessels.value, 4)
        self.assertEqual(state.n_valves.value, 4)
        self.assertEqual(state.n_extra1.value, 4)

    def test_load_defaults_after_custom(self):
        state = models.State()
        state.load_data_from_file(self.fixture_dir.joinpath('qra_component_counts_all4.hrm'))
        self.assertEqual(state.n_compressors.value, 4)
        self.assertEqual(state.n_vessels.value, 4)
        self.assertEqual(state.n_valves.value, 4)
        self.assertEqual(state.n_extra1.value, 4)

        state.load_data_from_file(self.fixture_dir.joinpath('qra_default_component_counts.hrm'))
        self.assertEqual(state.n_compressors.value, 1)
        self.assertEqual(state.n_vessels.value, 2)
        self.assertEqual(state.n_valves.value, 7)
        self.assertEqual(state.n_extra1.value, 0)
