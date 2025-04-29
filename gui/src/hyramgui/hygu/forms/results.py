"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import logging
from datetime import datetime
import webbrowser

from PySide6.QtCore import (QObject, Slot, Signal, QUrl, Property)
from PySide6.QtQml import QmlElement

from ..utils.events import Event
from ..forms.fields import StringFormField
from ..models.models import ModelBase


QML_IMPORT_NAME = "hygu.classes"
QML_IMPORT_MAJOR_VERSION = 1


@QmlElement
class ResultsForm(QObject):
    """Controller form class which manages data binding and views during and after execution of single analysis.

    Notes
    -----
    These controllers reside in main thread and can't access in-progress analyses in child processes.
    Completed analysis results are returned from thread loop.
    Note that event names correspond to usage location; e.g. camelCase events are used in QML.
    Many attributes are implemented as properties to allow access via QML. This includes the analysis parameter controllers.

    Attributes
    ----------
    started
    finished
    has_error
    error_message
    has_warning
    warning_message
    state
    analysis_id
    plots
    analysis_name
    started_at_str

    startedChanged : Signal
        Event indicating analysis has started processing on backend.

    finishedChanged : Signal
        Event indicating analysis has finished.

    request_overwrite_event : Event
        Event indicating form data is overwritten by another set of data (e.g. user loading data from result pane.)

    """
    startedChanged: Signal = Signal(bool)
    finishedChanged: Signal = Signal(bool)

    request_overwrite_event: Event

    # cloned model obj before analysis; should equal later one
    _prelim_state: type(ModelBase) or None = None

    # instance of state that undergoes analysis.
    _analysis_id: int
    _state: type(ModelBase) or None = None
    _started: bool = False
    _finished: bool = False
    _canceled: bool = False

    # Parameter controllers used to process data in form and to display it in results pane for completed analyses.
    _analysis_name: str or StringFormField

    def __init__(self, analysis_id: int, prelim_state: type(ModelBase), *args, **kwargs):
        """Initializes form with data for in-progress (queued) analysis, including unique id.

        Parameters
        ----------
        analysis_id : int
            Increasing id indicating the analysis managed by this.
        prelim_state : type(ModelBase)
            Copy of state object prior to analysis

        Notes
        -----
        Cloned state object obtained after analysis is complete.

        """
        super().__init__(parent=None)
        self._analysis = None
        self._analysis_id = analysis_id
        self._started_at = datetime.now()
        self._prelim_state = prelim_state
        self._analysis_name = self._prelim_state.analysis_name.value  # will be replaced by controller once state is processed

        self.request_overwrite_event = Event()

    def update_from_state_object(self):
        """Updates parameters and state data from cloned state object after analysis complete.
        Derived classes must use this to update this with their specific parameters, e.g.:

            self._analysis_name = StringFormField(param=self._state.analysis_name)

        Notes
        -----
        State object returned from analysis processing thread after analysis completed.
        Emits finishedChanged event.

        """
        raise NotImplementedError

    def finish_state_update(self):
        """Sets status and triggers event indicating update complete. Derived classes must call this after finishing their update.

        Notes
        -----
        Emits finishedChanged event.

        """
        self._finished = True
        self.finishedChanged.emit(True)

    @Property(bool, notify=startedChanged)
    def started(self):
        """Flag indicating analysis has begun processing. """
        return self._started

    @started.setter
    def started(self, val: bool):
        self._started = val
        self.startedChanged.emit(val)

    def do_cancel(self):
        self._canceled = True
        self._finished = True

    @Property(bool, constant=True)
    def has_error(self):
        """Flag indicating analysis encountered an error. """
        return self._state and self._state.has_error

    @Property(str, constant=True)
    def error_message(self):
        """Message describes error encountered during analysis. """
        if self._state is None:
            return ""
        else:
            return self._state.error_message

    finished = Property(bool, fget=lambda self: self._finished, notify=finishedChanged)
    canceled = Property(bool, fget=lambda self: self._canceled)

    has_warning = Property(bool, fget=lambda self: self._state.has_warning, constant=True)
    warning_message = Property(str, fget=lambda self: self._state.warning_message, constant=True)

    state = Property(type(ModelBase), fget=lambda self: self._state)
    analysis_id = Property(int, fget=lambda self: self._analysis_id, constant=True)

    @Property(list)
    def plots(self):
        """List of plots as prefixed filepaths for use in QML. """
        results = []
        if self.state is not None:
            pass  # Derived class should provide this
        return results

    # =============
    # Result interaction
    @Slot()
    def overwrite_form_data(self):
        """Replaces current parameter in form state with data from this analysis. """
        new_data = self.state.to_dict()
        self.request_overwrite_event.notify(new_data)

    @Slot()
    def export_pdf(self):
        pass

    @Slot()
    def open_output_directory(self):
        """Opens data output directory in OS window. """
        output_dir = self.state.get_output_dir()
        if output_dir is not None:
            webbrowser.open("file:///" + output_dir.as_posix())

    # =====================
    # IN-PROGRESS TEMP DATA
    @Property(str, constant=True)
    def started_at_str(self):
        """String describing time at which analysis begin, (H:M:S). """
        return self._started_at.strftime('%H:%M:%S')

    @Property(str, constant=True)
    def name_str(self):
        """Returns default name or param. Latter will not be available immediately, until state is set. """
        if type(self._analysis_name) == str:
            return self._analysis_name
        else:
            return self._analysis_name.value

    # =====================
    # PARAMETERS
    analysis_name = Property(StringFormField, fget=lambda self: self._analysis_name)
