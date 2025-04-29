"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import unittest
import os
from datetime import datetime
from pathlib import Path
from ..models import models


class TestModelBase(unittest.TestCase):

    def setUp(self):
        self.model = models.ModelBase()

    def test_initial_values(self):
        """Tests initial values of ModelBase instance"""
        self.assertIsInstance(self.model._cwd_dir, Path)
        self.assertIsNotNone(self.model._demo_dir)
        self.assertIsNone(self.model._output_dir)
        self.assertEqual(len(self.model._history), 0)
        self.assertEqual(len(self.model._redo_history), 0)
        self.assertEqual(self.model.analysis_name.value, "")
        self.assertEqual(self.model.session_dir.value, None)
        self.assertEqual(len(self.model.fields), 0)

    def test_is_finished(self):
        """Tests the is_finished attribute"""
        self.assertEqual(self.model.is_finished, False)

    def test_has_error(self):
        """Tests the has_error attribute"""
        self.assertEqual(self.model.has_error, False)

    def test_error_message(self):
        """Tests the error_message attribute"""
        self.assertEqual(self.model.error_message, "")

    def test_has_warning(self):
        """Tests the has_warning attribute"""
        self.assertEqual(self.model.has_warning, False)

    def test_warning_message(self):
        """Tests the warning_message attribute"""
        self.assertEqual(self.model.warning_message, "")

    def test_error(self):
        """Tests the error attribute"""
        self.assertIsNone(self.model.error)

    def test_set_id_function(self):
        _id = 5
        self.model.set_id(_id)
        self.assertEqual(self.model.analysis_id, _id)

    def test_set_output_dir_function(self):
        val = Path(os.getcwd())
        self.model.set_output_dir(val)
        self.assertEqual(self.model._output_dir, val)

    def test_get_output_dir_function(self):
        val = Path(os.getcwd())
        self.model._output_dir = val
        self.assertEqual(self.model.get_output_dir(), val)

    def test_set_session_dir(self):
        val = os.getcwd()
        self.model.set_session_dir(val)
        self.assertEqual(self.model.session_dir.value, val)

    def test_can_undo(self):
        self.model._history = []
        self.assertFalse(self.model.can_undo())

        self.model._history = ["change", "change 2"]
        self.assertTrue(self.model.can_undo())

    def test_can_redo(self):
        self.model._redo_history = []
        self.assertFalse(self.model.can_redo())

        self.model._redo_history = ["change", "change 2"]
        self.assertTrue(self.model.can_redo())

