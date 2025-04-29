"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import configparser
import time
import logging
from pathlib import Path

from PySide6.QtCore import (QObject, Slot, Signal, QUrl, Property)
from PySide6.QtGui import QClipboard, QImage
from PySide6.QtQml import QmlElement

from ..displays import QueueDisplay
from ..utils.helpers import InputStatus, ValidationResponse
from ..models.models import ModelBase
from ..threads import AnalysisThread

try:
    from ... import app_settings
except ImportError or ModuleNotFoundError:
    import app_settings


QML_IMPORT_NAME = "hygu.classes"
QML_IMPORT_MAJOR_VERSION = 1
log = logging.getLogger(app_settings.APPNAME)


@QmlElement
class AppForm(QObject):
    """Top-level application manager of GUI form, analysis thread, and analysis requests.

    Attributes
    ----------
    save_file_exists
    about_str
    copyright_str
    version_str
    can_undo
    can_redo
    db : type(State)
        Backing data store of all parameter values.
    q_app : QApplication or None
        Initialized Qt application
    queue_controller : type(QueueDisplay)
        Analysis queue manager
    result_forms : dict
        Dict of results form controllers corresponding to submitted or completed analysis; {analysis_id, controller}
    thread : AnalysisThread
        Long-running thread which handles analysis submissions and sub-process pool.
    analysisStarted : Signal
        Event emitted when a new analysis is submitted.
    historyChanged : Signal
        Event emitted when form history is modified.
    alertChanged : Signal
        Event emitted when alert state changes.
    newMessageEvent : Signal
        Event emitted when new messages is placed on UI message display queue.

    Notes
    -----
    Qt signals use camelCase for consistency with QML coding style.

    """
    db: type(ModelBase)
    queue_controller: type(QueueDisplay) = None
    result_forms: dict = {}
    thread: AnalysisThread

    analysisStarted = Signal(type(ModelBase))
    historyChanged = Signal()
    alertChanged = Signal(str, int)
    newMessageEvent = Signal(str)

    # parent QApplication
    q_app = None

    _about_str = ""
    _copyright_str = ""

    def __init__(self, model_class):
        """Initializes backend store and thread controller. """
        super().__init__(parent=None)
        self.db = model_class()
        self.db.history_changed += lambda x: self.historyChanged.emit()

        self.thread = AnalysisThread(self.db)
        self.thread.start()

        self._activate_validation()

    def set_app(self, app):
        """Sets Qt application once initialized. """
        self.q_app = app

    def set_queue(self, controller):
        """Sets queue controller. """
        self.queue_controller = controller

    def check_form_valid(self, *args, **kwargs) -> bool:
        """Checks whether form state is valid. Triggers form alert display and continuous validation when invalid.

        Notes
        -----
        May receive misc. inputs from other events.

        Returns
        -------
        bool
            True if form state is valid.

        """
        valid_resp = self.db.check_valid()  # 3 is error, 2 is warning, 1 is info, 0 is no issues
        self._toggle_form_alert(valid_resp)
        return valid_resp.status in [InputStatus.WARN, InputStatus.INFO, InputStatus.GOOD]

    @Slot(str)
    def set_session_dir(self, raw_dir):
        """ Attempts to set data output directory on model. """
        new_dir = QUrl(raw_dir).toLocalFile()
        success = self.db.set_session_dir(new_dir)
        if success:
            self.newMessageEvent.emit("Output directory set")

    @Slot()
    def reset_session_dir(self):
        """ Clears user-set data output directory and restores default functionality. """
        success = self.db.reset_session_dir()
        if success:
            self.newMessageEvent.emit("Output directory reset")

    @Slot()
    def request_analysis(self):
        """Handles analysis request by submitting valid data to thread and updating queue. """
        raise NotImplementedError()

    @Slot(int)
    def cancel_analysis(self, ac_id):
        """Begins process of canceling in-progress analysis. Note that GUI updates immediately by removing queue item.
        """
        if ac_id in self.result_forms:
            ac = self.result_forms[ac_id]
            ac.do_cancel()
        self.thread.cancel_analysis(ac_id)

    def analysis_started_callback(self, analysis_id: int):
        """Updates analysis status. Called via thread once analysis is prepped for pool processing. """
        ac = self.result_forms[analysis_id]
        ac.started = True

    def analysis_finished_callback(self, state_obj: type(ModelBase), results: dict):
        """Updates state of returned analysis with finalized data and sends it to its AnalysisController for final processing.
        Called via thread after processing pool finishes executing analysis.

        Parameters
        ----------
        state_obj : State
            state model object containing parameter and result data for specified analysis.

        results : dict
            analysis results including data and plots.

        """
        raise NotImplementedError()

    @Slot(str)
    def copy_image_to_clipboard(self, img_str):
        """Copies image filepath string to user's OS clipboard. """
        if app_settings.IS_WINDOWS:
            # remove prefix slash for Windows file paths
            img_str = img_str.removeprefix("file:///")
        else:
            img_str = img_str.removeprefix("file://")
        clip = QClipboard()
        img = QImage(img_str)
        clip.setImage(img, QClipboard.Mode.Clipboard)

    def handle_child_requests_form_overwrite(self, data: dict):
        """ Overwrites main state with parameter data from dict. """
        self.db.load_data_from_dict(data)
        self.newMessageEvent.emit("Data loaded successfully")

    @Slot()
    def shutdown(self):
        """Shuts down analysis thread (with timer) and exits app. """
        self._log("Beginning shutdown. Updating config file...")
        config_file = app_settings.DATA_DIR.joinpath('config.ini')
        config = configparser.ConfigParser()

        # try:
        # if config_file.exists():
        config['DEFAULT'] = {'session_dir': ""}
        if app_settings.USER_SET_SESSION_DIR:
            config['DEFAULT']['session_dir'] = str(app_settings.SESSION_DIR.as_posix())

        with open(config_file.as_posix(), 'w') as fl:
            config.write(fl)
        # except Exception as err:
        #     log.exception(err)

        self.thread.shutdown()
        # Wait while thread ends processes and shuts down.
        timer = app_settings.SHUTDOWN_TIMER
        while timer > 0:
            if self.thread.is_shutdown:
                break
            time.sleep(1.0)
            timer -= 1
            self._log(f"{timer}...")
        self._log("Pool shutdown complete")

        if self.q_app is not None:
            self._log("Quitting app. Goodbye!")
            self.q_app.quit()  # exit main loop; sys.exit called in main.py
            self.q_app.exit()

    @Slot()
    def print_state(self):
        self.db.print_state()

    @Slot()
    def print_history(self):
        self.db.print_history()

    @Slot()
    def open_data_directory(self):
        """Opens session directory in native file browser. """
        import webbrowser
        dirpath = Path(self.db.session_dir.value)
        if dirpath.is_dir():
            webbrowser.open("file:///" + dirpath.as_posix())

    def _toggle_form_alert(self, valid_resp: ValidationResponse):
        """Displays and populates form-wide alert.

        Parameters
        ----------
        valid_resp : ValidationResponse
        level : {0, 1, 2, 3}
            Tier of alert: 0 is no alert (hide), 1 is informational, 2 is warning, 3 is error.

        """
        self.alertChanged.emit(valid_resp.message, int(valid_resp.status.value))

    def _activate_validation(self):
        self.db.history_changed += self.check_form_valid

    def _deactivate_validation(self):
        self.db.history_changed -= self.check_form_valid

    # /////////////////////
    # SAVE / LOAD FUNCTIONS
    @Property(bool)
    def save_file_exists(self):
        """True if a filepath for save-file currently exists. TODO: check if file exists? """
        result = self.db.save_filepath is not None
        return result

    @Slot()
    def save_file(self):
        """Saves current state to existing savefile. """
        if self.save_file_exists:
            self.db.save_to_file(filepath=None)
            self.newMessageEvent.emit("File saved")

    @Slot(QUrl)
    def save_file_as(self, filepath: QUrl):
        """Saves current state to new savefile. """
        self.db.save_to_file(filepath.toLocalFile())
        self.newMessageEvent.emit("File saved")

    @Slot(QUrl)
    def load_save_file(self, filepath: QUrl):
        """Loads state from existing JSON save file. """
        filepath = filepath.toLocalFile()
        self.db.load_data_from_file(filepath)
        self.newMessageEvent.emit("Data loaded successfully")

    @Slot()
    def load_new_form(self):
        """Clears form and loads deterministic demo as new data. """
        self.db.clear_save_file()
        self.db.load_demo_file_data('det')
        self.newMessageEvent.emit("Form reset to default values")

    @Slot()
    def load_demo(self):
        """Loads demo data from repo file. """
        self.db.load_demo_file_data('')
        self.newMessageEvent.emit("Demo loaded")

    @Property(str, constant=True)
    def about_str(self):
        return self._about_str

    @Property(str, constant=True)
    def copyright_str(self):
        return self._copyright_str

    @Property(str, constant=True)
    def version_str(self):
        """Current version of app. """
        result = f"V{app_settings.VERSION}"
        return result

    @Property(bool, constant=True)
    def is_debug_mode(self):
        """Whether debug/development mode is active. """
        return app_settings.DEBUG

    # ///////////////////
    # UNDO / REDO HISTORY
    @Property(bool)
    def can_undo(self):
        """True if history is available to undo. """
        return self.db.can_undo()

    @Property(bool)
    def can_redo(self):
        """True if forward history is available to redo. """
        return self.db.can_redo()

    @Slot()
    def undo(self):
        """ Triggers rollback of single event in state history. """
        self.db.undo_state_change()

    @Slot()
    def redo(self):
        """ Triggers forward step of single event in state history. """
        self.db.redo_state_change()

    def _log(self, msg: str):
        log.info(f"State - {msg}")


