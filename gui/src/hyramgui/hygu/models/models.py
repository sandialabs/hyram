"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import json
import logging
import os
from datetime import datetime
from pathlib import Path

try:
    from ... import app_settings
except ImportError or ModuleNotFoundError:
    import app_settings

from ..models.fields import StringField
from ..utils import helpers
from ..utils.events import Event
from ..utils.helpers import hround, InputStatus, ValidationResponse


log = logging.getLogger(app_settings.APPNAME)


class ModelBase:
    """Base representation of analysis parameter data.

    Attributes
    ----------
    session_dir : StringField
        Filepath to directory containing files for this application session.
    save_filepath : Path or None
        Last-used filepath to save file for easy re-saving

    analysis_name : StringField
        Optional name of next analysis
    analysis_id : int or None
        Unique identifier for analysis associated with this state, if it represents a completed analysis.
    started_at : datetime or None
        Time at which analysis began.
    finished_at : datetime or None
        Time at which analysis finished.
    is_finished : bool
        True if associated analysis finished.
    has_error : bool
        True if analysis encountered error.
    error_message : str
        Error message describing error encountered during analysis.
    error : Exception
        Error describing error encountered during analysis.
    has_warning : bool
        True if analysis encountered warning.
    warning_message : str
        Warning message describing error encountered during analysis.
    history_changed : Event
        Emitted when state history is modified.

    Notes
    -----
    A single primary model instance is created and bound to the form UI.
    This instance is then cloned for each analysis to preserve the submitted state, resulting in child instances for each completed analysis.

    """

    _output_dir: Path or None
    _cwd_dir: Path
    _demo_dir: Path
    fields: list

    # last-used save-file to enable easy resaving
    save_filepath: Path or None

    # internal configuration file to persist settings between sessions
    config_filepath: Path or None

    # Properties used during analysis, not by main state object.
    analysis_id: int or None = None
    started_at: datetime or None = None
    finished_at: datetime or None = None
    is_finished: bool = False

    has_error: bool = False
    error_message: str = ""
    error: Exception = None
    has_warning: bool = False
    warning_message: str = ""

    was_canceled: bool = False

    analysis_name: StringField
    session_dir: StringField

    # Track changes to data over time. Each entry is dict (or JSON?) describing data state before change.
    _history: list
    _redo_history: list
    _record_changes = True
    history_changed: Event

    def __init__(self):
        """Initializes parameter values and history tracking. """
        self._log('Initializing data-store')

        # self.app_settings = app_settings
        self.history_changed = Event()  # any change occurs; instance-only

        self._cwd_dir = app_settings.CWD_DIR
        self._demo_dir = self._cwd_dir.joinpath('assets').joinpath('demo')
        self._output_dir = None
        self._history = []
        self._redo_history = []
        self.clear_save_file()

        self.analysis_name = StringField('Analysis name', value="")
        self.session_dir = StringField('Output directory', value=app_settings.SESSION_DIR)

        self.fields = []

    def post_init(self):
        """ Final setup after model fields are created. """
        # do not trigger change events with intermediate params
        for field in self.fields:
            field.changed += lambda x: self._handle_param_change()

        # record initial state as first entry
        self._handle_param_change()

    def check_valid(self) -> ValidationResponse:
        """Validates parameter values in form-wide manner. For example, validation of a parameter which depends on another parameter
        is done here.

        Notes
        -----
        Checks for errors first, then warnings, so most serious is shown first.

        Returns : ValidationResponse
        """
        for field in self.fields:
            field_resp = field.check_valid()
            if field_resp.status != InputStatus.GOOD:
                return field_resp

        return ValidationResponse(InputStatus.GOOD, '')

    def set_id(self, val):
        """Updates analysis ID to match submitted analysis. """
        self.analysis_id = val

    def set_output_dir(self, val):
        """Assigns filepath object to directory containing analysis file outputs. """
        self._output_dir = val

    def get_output_dir(self):
        """Filepath to directory containing files for analysis output. Child directory of `session_dir`. """
        if self._output_dir is not None and type(self._output_dir) != str and self._output_dir.is_dir():
            return self._output_dir
        else:
            return None

    # ===========================================
    # ========= DATA CONVERSION & PARSING =======
    def load_data_from_file(self, filepath):
        """Loads state parameter data from specified filepath to JSON file.

        Parameters
        ----------
        filepath : Path or str
            Path to file containing complete parameter JSON data to load.

        Notes
        -----
        Custom filetype is alias for JSON filetype.

        """
        if type(filepath) == str:
            filepath = Path(filepath)

        recording = self._record_changes
        self._record_changes = False  # Only do 1 record at end

        if filepath.exists():
            with open(filepath, 'r', encoding='utf-8') as f:
                state_dict = json.load(f)
                self._from_dict(state_dict)
            self._record_changes = True
            self._record_state_change()
        else:
            self._log(f"Can't load data - file not found {filepath.as_posix()}")
        self._record_changes = recording

    def load_data_from_dict(self, data: dict):
        """Overwrites state parameter data from complete dict as a single history state change.

        Parameters
        ----------
        data : dict
            Dictionary containing data for all parameters, including value, selected unit, etc.

        """
        recording = self._record_changes
        self._record_changes = False  # Only do 1 record at end
        self._from_dict(data)
        # update history
        self._record_changes = True
        self._record_state_change()
        # restore tracking state
        self._record_changes = recording

    def save_to_file(self, filepath=None):
        """Saves current state to file. Overwrites current save-file path if different.

        Parameters
        ----------
        filepath : str or None
            Full filepath of possibly non-existent .hpr (JSON) file in which to output data.

        """
        if filepath is None or filepath == self.save_filepath:
            filepath = self.save_filepath

        else:  # new filepath, i.e. "Save As"
            filepath = Path(filepath)
            self.save_filepath = filepath

        data = self.to_dict()
        with open(filepath, 'w', encoding='utf-8') as f:
            json.dump(data, f, ensure_ascii=False, indent=4)

    def clear_save_file(self):
        """Empties current save filepath. """
        self.save_filepath = None

    def get_prepped_param_dict(self):
        """Returns dict of parameters prepared for analysis submission. """
        data = {
            'session_dir': Path(self.session_dir.value),  # sub-process must not try to
            'output_dir': None,
        }

        for field in self.fields:
            data[field.slug] = field.get_prepped_value()

        return data

    def to_dict(self) -> dict:
        """Returns representation of this state as dict of parameter dicts. """
        result = {}
        for field in self.fields:
            result[field.slug] = field.to_dict()
        return result

    def _from_dict(self, data: dict):
        """Overwrites state analysis parameter data from dict containing all parameters.

        Parameters
        ----------
        data : dict
            Analysis parameter data with keys matching internal snake_case naming.

        """
        expected_keys = self.to_dict().keys()
        for key in expected_keys:
            if key not in data:
                raise ValueError(f'Required key {key} not found in state data {data}')

        for field in self.fields:
            field.from_dict(data[field.slug])

    def _handle_param_change(self):
        self._record_state_change()

    # ======================================
    # ========= HISTORY RECORDING ==========
    def _record_state_change(self):
        """Records full state to history as dict of params (also dicts) """
        if not self._record_changes:
            return

        current = self.to_dict()
        # ensure state has actually changed
        if self._history:
            prev = self._history[-1]
            if current == prev:
                return

        self._redo_history = []
        self._history.append(current)
        self.history_changed.notify(self)

    def undo_state_change(self):
        """Restores previous state from history and stores the change to redo_history list. """
        recording = self._record_changes
        self._record_changes = False

        if self.can_undo():
            current = self._history.pop(-1)
            self._redo_history.append(current)
            new_current = self._history[-1]
            self._from_dict(new_current)

        self._record_changes = recording
        self.history_changed.notify(self)

    def redo_state_change(self):
        """Undoes previous undo call. """
        recording = self._record_changes
        self._record_changes = False

        if self.can_redo():
            dct = self._redo_history.pop(-1)
            self._history.append(dct)
            self._from_dict(dct)

        self._record_changes = recording
        self.history_changed.notify(self)

    def can_undo(self) -> bool:
        """Flag indicating whether there is history to return to. """
        return len(self._history) > 1

    def can_redo(self) -> bool:
        """Flag indicating whether there is forward history available for restoration. """
        return bool(self._redo_history)

    # ======================
    # ==== LOGGING & DEV ===
    def _log(self, msg):
        log.info(f'State - {msg}')

    def print_state(self):
        pass

    def print_history(self):
        pass
        # for i, item in enumerate(self._history):
        #     print(f"HISTORY {i}")
        #     PPRINT.pprint(item)

    def load_demo_file_data(self):
        """Loads sample data from specified demo file. """
        demo_file = self._demo_dir.joinpath('demo.data')
        self.load_data_from_file(demo_file)

    def set_session_dir(self, new_dir: str):
        """ Updates the output directory if the new directory is permissible. """
        if os.access(new_dir, os.W_OK):
            self.session_dir.set_from_model(new_dir)
            app_settings.SESSION_DIR = Path(new_dir)
            app_settings.USER_SET_SESSION_DIR = True
            return True
        else:
            self.session_dir.set_alert("Selected directory was not accessible", stat=InputStatus.WARN)
            return False

    def reset_session_dir(self):
        """ Clears the manually-set session directory and restores default behavior via config setting. """
        try:
            default_dir = helpers.init_session_dir(parent_dir=app_settings.DATA_DIR)
            if os.access(default_dir, os.W_OK):
                self.session_dir.set_from_model(default_dir)
                app_settings.SESSION_DIR = Path(default_dir)
                app_settings.USER_SET_SESSION_DIR = False
                return True
            else:
                self.session_dir.set_alert("Unable to restore default session directory", stat=InputStatus.ERROR)
                return False
        except Exception as e:
            self.session_dir.set_alert("Could not create default session directory", stat=InputStatus.ERROR)
            return False
