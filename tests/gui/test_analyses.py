"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import os
import unittest
from pathlib import Path
from unittest import mock
import time

from hyramgui.models import models
from hyramgui import app_settings
from hyramgui.forms import app
from hyramgui.hygu.displays import QueueDisplay


VERBOSE = False


def wait_for_analysis(testobj, n_complete=1, t_max=10):
    """ Guarded while-loop to sleep while pooled analysis completes. """
    while testobj.appform.thread._num_complete < n_complete:
        time.sleep(testobj.sleep_t)

        testobj.guard_t += 1
        if testobj.guard_t >= t_max:
            testobj.assertTrue(False)  # analysis is hanging, quit out of test

    time.sleep(testobj.delay_t)


class DefaultAnalysisTestCase(unittest.TestCase):
    """Tests interactions between datastore and backend analysis calls. """

    def setUp(self) -> None:
        # Set directories manually to ensure they reside in test dir, otherwise they'll be created based on user's terminal cwd.
        app_settings.BASE_DIR = Path(os.path.dirname(os.path.abspath(__file__)))
        app_settings.SESSION_DIR = app_settings.BASE_DIR.joinpath('out')
        app_settings.init()

        self.appform = app.HyramAppForm()
        self.queue = QueueDisplay()
        self.appform.set_queue(self.queue)

        self.state = self.appform.db
        self.sleep_t = 1
        self.delay_t = 1
        self.guard_t = 0

    def tearDown(self) -> None:
        self.appform.shutdown()
        self.appform = None
        self.queue = None
        self.state = None
        self.guard_t = 0

    def test_default_plume_analysis_request_succeeds(self):
        """Multiprocessed analysis with default inputs completes successfully. """
        real_func = self.appform.analysis_finished_callback
        self.appform.analysis_finished_callback = mock.Mock(name='analysis_finished_callback')
        self.appform.request_analysis(analysis_type='plume')
        wait_for_analysis(self)

        self.appform.analysis_finished_callback.assert_called_once()

        state_obj, result_dict = self.appform.analysis_finished_callback.call_args[0]
        self.assertIsInstance(state_obj, models.State)

        self.assertIsInstance(result_dict, dict)
        self.assertTrue('status' in result_dict)
        self.assertTrue(result_dict['status'] == 1)
        self.appform.analysis_finished_callback = real_func

    # @unittest.skip
    def test_default_accumulation_analysis_request_succeeds(self):
        """Accumulation analysis with default inputs completes successfully. """
        real_func = self.appform.analysis_finished_callback
        self.appform.analysis_finished_callback = mock.Mock(name='analysis_finished_callback')
        self.appform.request_analysis(analysis_type='accum')
        wait_for_analysis(self)

        self.appform.analysis_finished_callback.assert_called_once()

        state_obj, result_dict = self.appform.analysis_finished_callback.call_args[0]
        self.assertIsInstance(state_obj, models.State)

        self.assertIsInstance(result_dict, dict)
        self.assertTrue('status' in result_dict)
        self.assertTrue(result_dict['status'] == 1)
        self.appform.analysis_finished_callback = real_func

    # @unittest.skip
    def test_default_overpressure_analysis_request_succeeds(self):
        """Unconfined overpressure analysis with default inputs completes successfully. """
        real_func = self.appform.analysis_finished_callback
        self.appform.analysis_finished_callback = mock.Mock(name='analysis_finished_callback')
        self.appform.request_analysis(analysis_type='uo')
        wait_for_analysis(self)

        self.appform.analysis_finished_callback.assert_called_once()

        state_obj, result_dict = self.appform.analysis_finished_callback.call_args[0]
        self.assertIsInstance(state_obj, models.State)

        self.assertIsInstance(result_dict, dict)
        self.assertTrue('status' in result_dict)
        self.assertTrue(result_dict['status'] == 1)
        self.appform.analysis_finished_callback = real_func

    # @unittest.skip
    def test_default_flame_analysis_request_succeeds(self):
        """flame analysis with default inputs completes successfully. """
        real_func = self.appform.analysis_finished_callback
        self.appform.analysis_finished_callback = mock.Mock(name='analysis_finished_callback')
        self.appform.request_analysis(analysis_type='flame')
        wait_for_analysis(self)

        self.appform.analysis_finished_callback.assert_called_once()

        state_obj, result_dict = self.appform.analysis_finished_callback.call_args[0]
        self.assertIsInstance(state_obj, models.State)

        self.assertIsInstance(result_dict, dict)
        self.assertTrue('status' in result_dict)
        self.assertTrue(result_dict['status'] == 1)
        self.appform.analysis_finished_callback = real_func

    # @unittest.skip
    def test_default_qra_analysis_request_succeeds(self):
        """QRA analysis with default inputs completes successfully. """
        real_func = self.appform.analysis_finished_callback
        self.appform.analysis_finished_callback = mock.Mock(name='analysis_finished_callback')
        self.appform.request_analysis(analysis_type='qra')
        wait_for_analysis(self)

        self.appform.analysis_finished_callback.assert_called_once()

        state_obj, result_dict = self.appform.analysis_finished_callback.call_args[0]
        self.assertIsInstance(state_obj, models.State)

        self.assertIsInstance(result_dict, dict)
        self.assertTrue('status' in result_dict)
        self.assertTrue(result_dict['status'] == 1)
        self.appform.analysis_finished_callback = real_func


