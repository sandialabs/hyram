"""
Copyright 2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
from PySide6.QtCore import QObject, Slot, Signal, Property, QStringListModel
from PySide6.QtQml import QmlElement

from ..forms.fields import FormFieldBase
from ..models.fields_probabilistic import UncertainField
from ..utils.distributions import Uncertainties

QML_IMPORT_NAME = "hygu.classes"
QML_IMPORT_MAJOR_VERSION = 1


@QmlElement
class UncertainFormField(FormFieldBase):
    """Manages float parameter model object.

    Attributes
    ----------
    unit_choices
    min_value
    max_value
    unit_type
    get_unit_disp
    uncertainty_plot
    unit : str
        Key defining active unit of measurement; e.g. 'm'.
    unit_type : UnitType
        Class of units of measurement, e.g. Distance.
    value : float
        Parameter value in selected units; e.g. 315 K.
    input_type : {'det', 'tnor', 'tlog', 'nor', 'log', 'uni'}
        Key representation of input or distribution.
    uncertainty : {'ale', 'epi', None}
        Type of uncertainty, if any.
    a : float
        Non-deterministic distribution parameter (see model for details).
    b : float
        Non-deterministic distribution parameter.
    c : float
        Non-deterministic distribution parameter (see model for details).
    d : float
        Non-deterministic distribution parameter.
    valueChanged : Signal
        Event emitted when parameter value changes via UI.
    aChanged : Signal
        Event emitted when parameter `a` changes.
    bChanged : Signal
        Event emitted when parameter `b` changes.
    labelChanged : Signal
        Event emitted when parameter label is changed.
    unitChanged : Signal
        Event emitted when parameter unit is changed.
    inputTypeChanged : Signal
        Event emitted when parameter input type is changed.
    uncertaintyChanged : Signal
        Event emitted when parameter uncertainty is changed.

    """
    valueChanged = Signal(float)
    aChanged = Signal(float)
    bChanged = Signal(float)
    cChanged = Signal(float)
    dChanged = Signal(float or None)
    unitChanged = Signal(str)
    inputTypeChanged = Signal(str)
    uncertaintyChanged = Signal(str)

    _param: UncertainField
    _unit_choices: QStringListModel

    def __init__(self, param=None):
        """Assigns model parameter to bind and preps unit choice list. """
        super().__init__(param=param)
        self._unit_choices = QStringListModel(self._param.unit_choices_display)

        # listen for db update to distribution
        self._param.distr_changed += lambda x: self.inputTypeChanged.emit(self._param.distr)
        self._param.uncertainty_changed += lambda x: self.uncertaintyChanged.emit(self._param.uncertainty)

    @Property(QObject, constant=True)
    def unit_choices(self):
        """QObject representing list of unit choices available. """
        return self._unit_choices

    @Property(float, constant=True)
    def min_value(self):
        """Minimum value allowed as string, in selected units."""
        result = float(self._param.min_value)
        return result

    @Property(float, constant=True)
    def max_value(self):
        """Maximum value allowed as string, in selected units."""
        result = float(self._param.max_value)
        return result

    @Property(str)
    def unit_type(self):
        """UnitType as string, e.g. 'Temperature'. """
        return self._param.unit_type

    @Property(str)
    def get_unit_disp(self):
        """Display-ready representation of active unit. """
        result = self._param.unit_choices_display[self._param.get_unit_index()]
        if result == 'fraction':
            result = '\u2013'
        return result

    @Property(str)
    def uncertainty_disp(self):
        """Display-ready representation of uncertainty. """
        val = ""
        if self.uncertainty == Uncertainties.ale:
            val = "aleatory"
        elif self.uncertainty == Uncertainties.epi:
            val = "epistemic"
        return val

    @Property(str)
    def uncertainty_plot(self):
        """Filepath of uncertainty plot, if any. """
        return self._param.uncertainty_plot

    # ==========================
    # PROPERTY GETTERS & SETTERS

    @Slot(str)
    def set_null(self, field:str):
        """Explicit function sets value to null because QML doesn't support passing different dtype value (null)."""
        if field == 'value':
            self.set_value(None)
        elif field == 'a':
            self.a = None
        elif field == 'b':
            self.b = None
        elif field == 'c':
            self.c = None
        elif field == 'd':
            self.d = None

    def get_value(self):
        val = self._param.value
        if type(val) is float:
            val = round(val, 8)
        return val

    def set_value(self, val: float or None):
        self._param.value = val
        self.valueChanged.emit(val)

    def get_input_type(self):
        return self._param.distr

    def set_input_type(self, val):
        self._param.distr = str(val)
        self.inputTypeChanged.emit(val)

    def get_uncertainty(self):
        return self._param.uncertainty

    def set_uncertainty(self, val):
        self._param.uncertainty = str(val)
        self.uncertaintyChanged.emit(val)

    def get_unit(self):
        return self._param.unit

    def set_unit(self, val: str):
        self._param.set_unit_from_display(val)
        self.unitChanged.emit(val)

    def get_a(self):
        return self._param.a

    def set_a(self, val: float):
        self._param.a = val

    def get_b(self):
        return self._param.b

    def set_b(self, val: float):
        self._param.b = val

    def get_c(self):
        return self._param.c

    def set_c(self, val: float):
        self._param.c = val

    @Property(float or None, notify=dChanged)
    def d(self):
        return self._param.d

    @d.setter
    def d(self, val: float or str):
        self._param.d = val

    @Property(bool, constant=True)
    def d_is_null(self):
        return self._param.d is None

    unit = Property(str, get_unit, set_unit, notify=unitChanged)
    input_type = Property(str, get_input_type, set_input_type, notify=inputTypeChanged)
    uncertainty = Property(str, get_uncertainty, set_uncertainty, notify=uncertaintyChanged)
    value = Property(float, get_value, set_value, notify=valueChanged)
    a = Property(float, get_a, set_a, notify=aChanged)
    b = Property(float, get_b, set_b, notify=bChanged)
    c = Property(float, get_c, set_c, notify=cChanged)

    # =================
    # UTILITY FUNCTIONS
    @Slot(result=int)
    def get_unit_index(self):
        """Returns index of active unit. """
        # print(f"PC | {self.label} get_unit_index of {self.get_unit()} in {self._param.unit_choices_display}")
        result = self._param.get_unit_index()
        return result

    @Slot(result=int)
    def get_input_type_index(self):
        """Returns index of selected input type. """
        result = self._param.get_distr_index()
        return result

    @Slot(result=int)
    def get_uncertainty_index(self):
        """Returns index of selected uncertainty type. """
        result = self._param.get_uncertainty_index()
        return result
