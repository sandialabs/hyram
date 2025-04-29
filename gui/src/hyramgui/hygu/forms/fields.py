"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import numpy as np
from PySide6.QtCore import QObject, Slot, Signal, Property, QStringListModel
from PySide6.QtQml import QmlElement

from ..models.fields import ChoiceField, IntField, BoolField, FieldBase, StringField, NumField

QML_IMPORT_NAME = "hygu.classes"
QML_IMPORT_MAJOR_VERSION = 1


@QmlElement
class FormFieldBase(QObject):
    """Base class for all form field types. Form fields represent parameters for display and interaction.
    Each field instance corresponds to a single parameter input.

    Attributes
    ----------
    label
    label_rtf
    enabled
    str_display
    value_display
    value_tooltip
    labelChanged : Signal
        Event emitted when parameter label is changed.
    modelChanged : Signal
        Event emitted when state model changes value.

    Notes
    -----
    value getter/setter must be defined in derived classes because Qt requires type to be defined.

    """
    labelChanged = Signal(str)
    modelChanged = Signal()
    _param: type(FieldBase)
    _dtype: type

    def __init__(self, param=None):
        """Assigns parameter model. """
        super().__init__(parent=None)
        self._param = param
        self._param.changed_by_model += lambda x: self.modelChanged.emit()

    @Property(str, notify=labelChanged)
    def label(self):
        """Parameter label. """
        return self._param.label

    @Property(str, constant=True)
    def label_rtf(self):
        """Parameter label in rich-text formatting; e.g. Volume H<sub>2</sub>. """
        return self._param.label_rtf

    @Property(bool, constant=True)
    def enabled(self):
        """Whether parameter is currently enabled and ready for changes. """
        return bool(self._param.enabled)

    @Property(str, constant=True)
    def alert(self):
        """Whether parameter is currently enabled and ready for changes. """
        return str(self._param.get_alert())

    @Property(int, constant=True)
    def status(self):
        """Whether parameter is currently enabled and ready for changes. """
        return int(self._param.status.value)

    @Property(str)
    def str_display(self):
        """String representation of Parameter including label and value. """
        return self._param.str_display

    @Property(str)
    def value_display(self):
        """Display-ready value; e.g. 'deterministic'. """
        return self._param.get_value_display()

    @Property(str, constant=True)
    def value_tooltip(self):
        """Tooltip description of parameter. """
        return self._param.get_value_tooltip()

    def __repr__(self):
        return self.str_display

    def __str__(self):
        return self.str_display


@QmlElement
class StringFormField(FormFieldBase):
    """Manages simple string parameter model.

    Attributes
    ----------
    value : str
        Stored parameter value.
    valueChanged : Signal
        Event emitted when parameter value changes via UI.
    """
    valueChanged = Signal(str)
    _param: StringField
    _dtype = str

    def __init__(self, param):
        """Assigns parameter model. """
        super().__init__(param=param)

    @Property(str, notify=valueChanged)
    def value(self):
        val = self._param.value
        return val

    @value.setter
    def value(self, val):
        self._param.value = val
        self.valueChanged.emit(val)


@QmlElement
class ChoiceFormField(FormFieldBase):
    """Manages parameter model comprising a list of choices typically displayed with a selector combo-box.

    Attributes
    ----------
    choices
    value
    valueChanged : Signal
        Event emitted when parameter value changes via UI.

    """
    valueChanged = Signal(str)
    _param: ChoiceField
    _choices: QStringListModel

    def __init__(self, param=None):
        """Initializes controller with parameter model.

        Parameters
        ----------
        param : ChoiceField
            Parameter model object with which to bind.

        """
        super().__init__(param=param)
        self._choices = QStringListModel(self._param.get_choice_displays())

    @Property(QObject, constant=True)
    def choices(self):
        """List of parameter choices as QStringListModel. """
        return self._choices

    @Property(str, notify=valueChanged)
    def value(self):
        """Shortened key representation of stored value. """
        return self._param.value

    @value.setter
    def value(self, index: int):
        """Sets value from selected index into list of choices.

        Parameters
        ----------
        index : int
            Index into list of parameter choices.

        """
        self._param.set_value_from_index(int(index))
        self.valueChanged.emit(self._param.value)

    @Slot(result=int)
    def get_index(self):
        """Returns index of currently-selected value out of available choices. """
        result = self._param.get_value_index()
        return result


@QmlElement
class BoolFormField(FormFieldBase):
    """Manages boolean parameter model.

    Attributes
    ----------
    value : bool
        Parameter bool value.
    valueChanged : Signal
        Event emitted when parameter value changes via UI.

    """
    valueChanged = Signal(bool)
    _param: BoolField

    def __init__(self, param=None):
        """Assigns parameter model. """
        super().__init__(param=param)

    def get_value(self):
        val = bool(self._param.value)
        return val

    def set_value(self, val: bool):
        self._param.value = val
        self.valueChanged.emit(val)

    value = Property(bool, get_value, set_value, notify=valueChanged)


@QmlElement
class IntFormField(FormFieldBase):
    """Manages unit-less int-based parameter model object.

    Attributes
    ----------
    min_value
    max_value
    value : int
        Stored parameter value which is deterministic and unit-less.
    valueChanged : Signal
        Event emitted when parameter value changes via UI.

    """
    valueChanged = Signal(int)
    _param: IntField

    def __init__(self, param=None):
        """Assigns parameter model. """
        super().__init__(param=param)

    @Property(str, constant=True)
    def min_value_str(self):
        """Minimum value allowed, as string. """
        return str(self._param.min_value_str)

    @Property(str, constant=True)
    def max_value_str(self):
        """Maximum value allowed, as string. """
        return str(self._param.max_value_str)

    @Property(int, constant=True)
    def min_value(self):
        """Minimum value allowed. """
        return int(self._param.min_value)

    @Property(int, constant=True)
    def max_value(self):
        """Maximum value allowed. """
        return int(self._param.max_value)

    def get_value(self):
        if self._param.value is None:
            return None

        val = int(self._param.value)
        return val

    def set_value(self, val):
        if val is None or val == '':
            val = None
        self._param.value = val
        self.valueChanged.emit(val)

    @Property(bool, constant=True)
    def is_null(self):
        return self._param.is_null

    @Slot()
    def set_null(self):
        """Sets value to null because QML doesn't support passing different dtype value (null)."""
        self.set_value(None)

    value = Property(int or None, get_value, set_value, notify=valueChanged)


@QmlElement
class NumFormField(FormFieldBase):
    """Manages unit-ful int or float parameter model object.

    Attributes
    ----------
    unit_choices
    min_value
    max_value
    unit_type
    get_unit_disp
    unit : str
        Key defining active unit of measurement; e.g. 'm'.
    unit_type : UnitType
        Class of units of measurement, e.g. Distance.
    value : float
        Parameter value in selected units; e.g. 315 K.
    valueChanged : Signal
        Event emitted when parameter value changes via UI.
    labelChanged : Signal
        Event emitted when parameter label is changed.
    unitChanged : Signal
        Event emitted when parameter unit is changed.

    """
    valueChanged = Signal(float)
    unitChanged = Signal(str)

    _param: NumField
    _unit_choices: QStringListModel

    def __init__(self, param=None):
        """Assigns model parameter to bind and preps unit choice list. """
        super().__init__(param=param)
        self._unit_choices = QStringListModel(self._param.unit_choices_display)

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

    @Property(str, constant=True)
    def unit_type(self):
        """UnitType as string, e.g. 'Temperature'. """
        return str(self._param.unit_type.label)

    @Property(str)
    def get_unit_disp(self):
        """Display-ready representation of active unit. """
        result = self._param.unit_choices_display[self._param.get_unit_index()]
        if result == 'fraction':
            result = '\u2013'
        return result

    # ==========================
    # PROPERTY GETTERS & SETTERS

    def get_value(self):
        val = self._param.value
        if type(val) is float:
            val = round(val, 8)
        return val

    def set_value(self, val):
        if val is None or val == '':
            val = None
        self._param.value = val
        self.valueChanged.emit(val)

    def get_unit(self):
        return self._param.unit

    def set_unit(self, val: str):
        self._param.set_unit_from_display(val)
        self.unitChanged.emit(val)

    @Property(bool, constant=True)
    def is_null(self):
        return self._param.is_null

    @Slot()
    def set_null(self):
        """Sets value to null because QML doesn't support passing different dtype value (null)."""
        self.set_value(None)

    @Slot(str)
    def set_null(self, field):
        """Overload Sets value to null; accepts field specifier for compatibility with other field types. """
        self.set_value(None)

    unit = Property(str, get_unit, set_unit, notify=unitChanged)
    value = Property(float or None, get_value, set_value, notify=valueChanged)

    # =================
    # UTILITY FUNCTIONS
    @Slot(result=int)
    def get_unit_index(self):
        """Returns index of active unit. """
        result = self._param.get_unit_index()
        return result


@QmlElement
class NumListFormField(FormFieldBase):
    """Manages unit-ful parameter model representing list of floats.

    Attributes
    ----------
    unit_choices
    min_value
    max_value
    unit_type
    get_unit_disp
    unit : str
        Key defining active unit of measurement; e.g. 'm'.
    unit_type : UnitType
        Class of units of measurement, e.g. Distance.
    value : list
        List of values in selected units; e.g. 315 K.
    valueChanged : Signal
        Event emitted when parameter value changes via UI.
    labelChanged : Signal
        Event emitted when parameter label is changed.
    unitChanged : Signal
        Event emitted when parameter unit is changed.

    """
    valueChanged = Signal(float)
    unitChanged = Signal(str)

    _param: NumField
    _unit_choices: QStringListModel

    def __init__(self, param=None):
        """Assigns model parameter to bind and preps unit choice list. """
        super().__init__(param=param)
        self._unit_choices = QStringListModel(self._param.unit_choices_display)

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
        return self._param.unit_choices_display[self._param.get_unit_index()]

    # ==========================
    # PROPERTY GETTERS & SETTERS
    def get_value(self):
        vals = self._param.value
        vals_str = ""
        for val in vals:
            vals_str += f"{val} "
        return vals_str

    def set_value(self, val: list or str):
        self._param.value = val
        self.valueChanged.emit(val)

    def get_unit(self):
        return self._param.unit

    def set_unit(self, val: str):
        self._param.set_unit_from_display(val)
        self.unitChanged.emit(val)

    unit = Property(str, get_unit, set_unit, notify=unitChanged)
    value = Property(str, get_value, set_value, notify=valueChanged)

    # =================
    # UTILITY FUNCTIONS
    @Slot(result=int)
    def get_unit_index(self):
        """Returns index of active unit. """
        result = self._param.get_unit_index()
        return result
