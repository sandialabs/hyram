"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import numpy as np

from ..utils.distributions import BaseChoiceList
from ..utils.helpers import InputStatus, ValidationResponse, get_num_str, count_decimal_places, convert_to_float_list
from ..utils.units_of_measurement import (UnitType, Unitless, get_unit_type_from_unit_key, get_unit_type)
from ..utils.events import Event


MAX_INT = int(2e9)


def _get_slug(label: str, slug=None) -> str:
    """returns string in snake_case, derived from label if slug not already found. """
    if slug is None:
        slug = ""
    result = slug.strip()
    if result == "":
        result = label.lower().strip().replace(" ", "_")
    return result


class FieldBase:
    """Analysis parameter described by string. Parameter types must derive from this base class.

    Attributes
    ----------
    str_display
    label : str
        Descriptive label.
    label_rtf : str
        Parameter label in rich-text formatting; e.g. Volume H<sub>2</sub>.
    slug : str
        Internal representation of parameter label. Must match backend API.
    tooltip : str or None
        Descriptive representation of parameter for populating tooltip.

    changed : Event
        Event emitted when user changes value via GUI.
    changed_by_model : Event
        Event emitted when value is changed by state.

    """
    label: str
    label_rtf: str = ""
    _value: int or str or float or bool or None
    enabled: bool = True
    tooltip: None or str
    changed: Event
    changed_by_model: Event
    _status = InputStatus.GOOD  # 0 error, 1 good, 2 info, 3 warning
    _alert: str = ""
    _dtype: type

    def __init__(self, label, slug=None, value=None, label_rtf=None, tooltip=None):
        self.label = label
        self.label_rtf = label_rtf if label_rtf is not None else label

        self._value = None if value is None else self._dtype(value)
        self.slug = _get_slug(label, slug)
        self.tooltip = tooltip

        self.changed = Event()  # any change occurs; instance-only
        self.changed_by_model = Event()

    @property
    def value(self):
        """Parameter value. """
        return self._value

    @value.setter
    def value(self, val):
        """Sets value and emits changed event. """
        self._value = self._dtype(val)
        self.changed.notify(self)

    @property
    def str_display(self):
        """Returns string-representation of parameter, including label. """
        result = f"{self.label}: {self.value}"
        return result

    def __repr__(self):
        return f"{type(self).__name__} {self.str_display}"

    @property
    def has_alert(self):
        return self._status != InputStatus.GOOD

    @property
    def status(self) -> InputStatus:
        return self._status

    @status.setter
    def status(self, val: InputStatus):
        self._status = val

    # TODO: convert to properties
    def get_alert(self) -> str:
        return self._alert

    def set_alert(self, val, stat):
        self._alert = val
        self._status = stat
        self.changed_by_model.notify(self)

    def clear_alert(self):
        self._alert = ""
        self._status = InputStatus.GOOD
        self.changed_by_model.notify(self)

    def toggle_enabled(self, val: bool, silent=True):
        """Sets enabled status and optionally emits model change event. """
        self.enabled = val
        if not silent:
            self.changed_by_model.notify(self)

    def get_prepped_value(self):
        """Returns value. """
        return self._value

    def set_from_model(self, raw):
        """Sets raw value via backend state. """
        self.clear_alert()
        self._value = self._dtype(raw)
        self.changed_by_model.notify(self)

    def check_valid(self) -> ValidationResponse:
        """Validates parameter data.

        Returns : ValidationResponse
        """
        return ValidationResponse(InputStatus.GOOD, '')

    def to_dict(self) -> dict:
        """Returns data representation of parameter as dict. """
        result = {
            'label': self.label,
            'slug': self.slug,
            'value': self._value,
        }
        return result

    def from_dict(self, data: dict, notify_from_model=True):
        """Updates parameter properties from dict.

        Parameters
        ----------
        data : dict
            property value pairs as {'property': value}.
        notify_from_model : bool
            True if this function called by model.

        Notes
        -----
        Assumes all properties are present.

        """
        self.label = data['label']
        self.slug = data['slug'] if 'slug' in data else _get_slug(self.label)
        self._value = self._dtype(data['value'])

        self.changed.notify(self)

        if notify_from_model:
            self.changed_by_model.notify(self)


class StringField(FieldBase):
    """Analysis parameter described by string.
    """
    _value: str
    _dtype = str

    def __init__(self, label, slug=None, value=None):
        super().__init__(label=label, slug=slug, value=value)


class BoolField(FieldBase):
    """Analysis parameter described by boolean value; e.g. a flag.
    """
    _value: bool = True
    _dtype = bool

    def __init__(self, label, value, slug=None):
        super().__init__(label=label, value=value, slug=slug)


class ChoiceField(FieldBase):
    """Analysis parameter described by set of selectable options.

    Attributes
    ----------
    str_display
    choices : BaseChoiceList

    """
    choices: type(BaseChoiceList)
    _value: str  # 3-letter key identifying selected choice
    _dtype = str

    def __init__(self, label, choices, value=None, slug=None):
        super().__init__(label=label, value=value, slug=slug)
        self.choices = choices

        if value is not None:
            self._value = value.lower()
        else:
            self._value = self.choices[0]

    @property
    def str_display(self):
        """Returns string-representation of parameter, including label. """
        result = f"{self.label}: {self.get_value_display()}"
        return result

    def set_value_from_key(self, val: str):
        """ Sets active unit from key. e.g. 'MPa' updates unit type to 'mpa'. """
        if val not in self.choices:
            raise ValueError('Choice not found')
        self._value = val.lower()
        self.changed.notify(self)

    def set_value_from_index(self, i: int):
        """Sets value via index into choice list. """
        if i >= len(self.choices):
            raise ValueError("Index for selected value not found")
        self._value = self.choices[i]
        self.changed.notify(self)

    def get_value_index(self) -> int:
        """Returns index of selected value into choices options. """
        return self.choices.index(self._value)

    def get_value_display(self) -> str:
        """Returns display-ready form of value; e.g. Deterministic. """
        idx = self.get_value_index()
        result = self.choices.labels[idx]
        return result

    def get_choice_keys(self) -> [str]:
        """Returns key-only list of all choices. """
        results = self.choices.keys
        return results

    def get_choice_displays(self) -> [str]:
        """Returns display-ready list of all choices. """
        results = self.choices.labels
        return results

    def from_dict(self, data: dict, notify_from_model=True):
        """Updates parameter properties from dict.

        Parameters
        ----------
        data : dict
            property value pairs as {'property': value}.
        notify_from_model : bool
            True if this function called by model.

        Notes
        -----
        Assumes all properties are present.

        """
        self.label = data['label']
        self.slug = data['slug'] if 'slug' in data else _get_slug(self.label)
        self.set_value_from_key(data['value'])

        self.changed.notify(self)

        if notify_from_model:
            self.changed_by_model.notify(self)


class NumField(FieldBase):
    """Analysis parameter described by int or float.

    Attributes
    ----------
    value
    min_value_str
    max_value_str
    value
    str_display
    unit_choices_display
    min_value : float
        Minimum allowed value.
    max_value : float
        Maximum allowed value.
    unit_type : UnitType
        Class of units of measurement, e.g. Distance.
    unit : str or None
        Key defining active unit of measurement; e.g. 'm'. Value of None indicates parameter is unit-less.

    """
    _min_value: float
    _max_value: float
    unit_type: type(UnitType)
    unit: str or None
    _unit_choices: list
    _display_units: list
    _dtype = float

    _track_changes = True

    def __init__(self, label, value, slug='', unit_type=None, unit=None, min_value=0, max_value=np.inf, label_rtf=None, tooltip=None,
                 unit_choices=None):
        super().__init__(label=label, slug=slug, value=value, label_rtf=label_rtf, tooltip=tooltip)

        # unit type is given or determined from unit (e.g. 'm' unit yields Distance)
        if unit_type is not None:
            self.unit_type = unit_type
        elif unit_type is None and unit is None:
            self.unit_type = Unitless
        elif unit_type is None:
            self.unit_type = get_unit_type_from_unit_key(unit)  # NOTE: finds SmallDistance before Distance

        if unit is None:
            unit = self.unit_type.std_unit
        self.unit = unit

        # build up lists of available unit ids and display names
        if unit_choices is None:
            self._unit_choices = self.unit_type.units()
            self._display_units = self.unit_type.display_units
        else:
            self._unit_choices = []
            self._display_units = []
            for un_id, un_disp in zip(self.unit_type.units(), self.unit_type.display_units):
                if un_id in unit_choices:
                    self._unit_choices.append(un_id)
                    self._display_units.append(un_disp)

        self._value = self.unit_type.convert(value, old=self.unit)
        self._min_value = self.unit_type.convert(min_value, old=self.unit)
        self._max_value = self.unit_type.convert(max_value, old=self.unit)

    @property
    def value(self):
        """ Returns value in selected units. If probabilistic, returns value as distribution. """
        result = None if self._value is None else self._dtype(self.unit_type.convert(self._value, new=self.unit, do_round=True))
        return result

    @value.setter
    def value(self, val: float):
        """ Sets stored standard value according to active unit. """
        if val is None or val == "":
            self._value = None
            self.notify_changed()
        else:
            new_value = self.unit_type.convert(val, old=self.unit)
            if self.in_range_raw(new_value):
                self._value = self._dtype(new_value)
                self.notify_changed()

    @property
    def value_raw(self):
        return self._value

    def set_value_raw(self, val):
        """ Sets value assuming standard units. """
        if val is None or val == "":
            self._value = None
            self.notify_changed()
        elif self.in_range_raw(val):
            self._value = self._dtype(val)
            self.notify_changed()

    @property
    def min_value(self):
        if self._min_value == -np.inf:
            result = -np.inf
        else:
            result = self.unit_type.convert(self._min_value, new=self.unit, do_round=True)
        return result

    @min_value.setter
    def min_value(self, val: float):
        new_value = self.unit_type.convert(val, old=self.unit)
        self._min_value = self._dtype(new_value)
        self.notify_changed()

    @property
    def max_value(self):
        if self._max_value == np.inf:
            result = np.inf
        else:
            result = self.unit_type.convert(self._max_value, new=self.unit, do_round=True)
        return result

    @max_value.setter
    def max_value(self, val: float):
        new_value = self.unit_type.convert(val, old=self.unit)
        self._max_value = self._dtype(new_value)
        self.notify_changed()

    @property
    def is_null(self) -> bool:
        return self.value is None or self.value == ''

    def in_range(self, val) -> bool:
        return val is None or self.min_value <= val <= self.max_value

    def in_range_raw(self, val) -> bool:
        return val is None or self._min_value <= val <= self._max_value

    def set_from_model(self, raw):
        """Sets raw value via backend model. """
        raw = self._dtype(raw)
        if self.in_range(raw):
            super().set_from_model(raw)

    def get_unit_index(self):
        """ Returns int index of selected unit within its unit choices. e.g. bar in [MPa, bar, psi] would return 1. """
        if self.unit is None or self.unit_type in [Unitless]:
            return 0

        if self.unit not in self._unit_choices:
            raise ValueError(f"Unit {self.unit} not found in unit choices")
        result = self._unit_choices.index(self.unit)
        return result

    def set_unit_from_display(self, val: str):
        """ Sets active unit from a selected display value, not key. e.g. 'MPa' updates unit type to 'mpa' """
        disp_units = self._display_units
        if len(disp_units) <= 1:
            return
        if val not in disp_units:
            raise ValueError('Display unit not found')

        old_param_val = self.value
        i = disp_units.index(val)
        self.unit = self._unit_choices[i]

        # displayed value now in new units so update raw values in batch to yield single change event
        tracking = self._track_changes
        self._track_changes = False
        if old_param_val is None:
            self._value = None
        else:
            new_value = self.unit_type.convert(old_param_val, old=self.unit)
            self._value = self._dtype(new_value)
        self._track_changes = tracking

        self.changed.notify(self)

    @property
    def unit_choices_display(self):
        return self._display_units

    @property
    def min_value_str(self):
        """Minimum value formatted for display. """
        return get_num_str(self.min_value)

    @property
    def max_value_str(self):
        """Maximum value formatted for display. """
        return get_num_str(self.max_value)

    @property
    def value_str(self):
        """ Returns formatted string representation of converted value. """
        return get_num_str(self.value)

    def get_value_tooltip(self):
        """Returns tooltip for parameter. """
        if self.tooltip is None:
            if self.min_value == 0 and self.max_value == np.inf:
                result = "Enter a non-negative value"
            elif self.min_value == -np.inf and self.max_value == np.inf:
                result = "Enter a value"
            elif self.max_value == np.inf:
                result = f"Enter a value >= {self.min_value_str}"
            else:
                result = f"Enter a value between {self.min_value_str} and {self.max_value_str}"
            return result
        else:
            return self.tooltip

    def check_valid(self) -> ValidationResponse:
        resp = super().check_valid()
        if resp.status != InputStatus.GOOD:
            return resp

        msg = ""
        if self.value is not None:
            if self.value < self.min_value:
                msg = f'{self.label} value below minimum ({self.value_str} < {self.min_value_str})'
            elif self.value > self.max_value:
                msg = f'{self.label} value above maximum ({self.value_str} > {self.max_value_str})'

        status = InputStatus.ERROR if msg else InputStatus.GOOD
        return ValidationResponse(status, msg)

    def notify_changed(self):
        if self._track_changes:
            self.changed.notify(self)

    def to_dict(self) -> dict:
        """Provide data representation in dict format. All values are stored in standard (raw) format. """
        result = super().to_dict()
        # store as str to handle np.inf
        extra = {
            'min_value': get_num_str(self._min_value),
            'max_value': get_num_str(self._max_value),
            'unit_type': self.unit_type.label,
            'unit': self.unit,
        }
        result |= extra
        return result

    def from_dict(self, data: dict, notify_from_model=True, silent=False):
        """Set all values from dict. Assume all properties are present. """
        self.label = data['label']
        self.slug = data['slug']

        if 'unit_type' in data:
            unit_type_key = data['unit_type']
            unit_type = get_unit_type(unit_type_key)
            self.unit_type = unit_type
        if 'unit' in data:
            self.unit = data['unit']

        self.set_value_raw((data['value']))

        # limit may be stored as string
        min_str = data.get('min_value', -np.inf)
        max_str = data.get('max_value', np.inf)
        self._min_value = -np.inf if type(min_str) is str and 'infinity' in min_str else float(min_str)
        self._max_value = np.inf if type(max_str) is str and 'infinity' in max_str else float(max_str)

        if not silent:
            self.changed.notify(self)

            if notify_from_model:
                self.changed_by_model.notify(self)

    @property
    def str_display(self):
        """ Returns string-representation of parameter, including label.
        """
        if self.unit is None:
            unit_str = ""
        else:
            disp_unit_index = self.get_unit_index()
            unit_str = self._display_units[disp_unit_index]
        val = self.value_str
        result = f"{self.label}: {val} {unit_str}"
        return result


class IntField(NumField):
    """Analysis parameter described by unit-less, deterministic integer; e.g. random seed.

    Attributes
    ----------
    value
    str_display

    """
    _value: int
    _dtype = int

    def __init__(self, label, value, **kwargs):
        super().__init__(label=label, value=value, **kwargs)
        if self.max_value in [np.inf, None]:
            self.max_value = int(MAX_INT)
        if self.min_value in [-np.inf, None]:
            self.min_value = -int(MAX_INT)

    @property
    def str_display(self):
        """ Returns string-representation of parameter, including label. """
        return f"{self.label}: {self.value:0d}"

    def get_value_tooltip(self):
        """Returns tooltip for parameter. """
        if self.tooltip is None:
            if self.min_value == 0 and self.max_value == int(MAX_INT):
                result = "Enter a non-negative value"
            elif self.min_value == -np.inf and self.max_value == int(MAX_INT):
                result = "Enter a value"
            elif self.max_value == int(MAX_INT):
                result = f"Enter a value >= {self.min_value_str}"
            else:
                result = f"Enter a value between {self.min_value_str} and {self.max_value_str}"
            return result
        else:
            return self.tooltip


    def from_dict(self, data: dict, notify_from_model=True, silent=False):
        """Set all values from dict. Assume all properties are present. """
        self.label = data['label']
        self.slug = data['slug']

        if 'unit_type' in data:
            unit_type_key = data['unit_type']
            unit_type = get_unit_type(unit_type_key)
            self.unit_type = unit_type
        if 'unit' in data:
            self.unit = data['unit']

        self.set_value_raw((data['value']))

        # limit may be stored as string
        min_str = data.get('min_value', 'infinity')
        if type(min_str) is str and 'infinity' in min_str or min_str == str(-MAX_INT):
            self._min_value = -int(MAX_INT)
        else:
            self._min_value = int(float(min_str))

        max_str = data.get('max_value', 'infinity')
        if type(max_str) is str and 'infinity' in max_str or max_str == str(MAX_INT):
            self._max_value = int(MAX_INT)
        else:
            self._max_value = int(float(max_str))

        if not silent:
            self.changed.notify(self)

            if notify_from_model:
                self.changed_by_model.notify(self)


class NumListField(FieldBase):
    """Analysis parameter described by list of floats.
    Raw values stored in list as standard units.

    Attributes
    ----------
    value
    min_value_str
    max_value_str
    value
    str_display
    unit_choices_display
    min_value : float
        Minimum allowed value.
    max_value : float
        Maximum allowed value.
    unit_type : UnitType
        Class of units of measurement, e.g. Distance.
    unit : str or None
        Key defining active unit of measurement; e.g. 'm'. Value of None indicates parameter is unit-less.

    """
    _min_value: float
    _max_value: float
    unit_type: type(UnitType)
    unit: str or None
    _unit_choices: list
    _display_units: list
    _dtype = list
    _num_decimals = 2

    _track_changes = True

    def __init__(self, label, value, slug='', unit_type=None, unit=None, min_value=0, max_value=np.inf, label_rtf=None, tooltip=None,
                 unit_choices=None):
        super().__init__(label=label, slug=slug, value=value, label_rtf=label_rtf, tooltip=tooltip)

        # unit type is given or determined from unit (e.g. 'm' unit yields Distance)
        if unit_type is not None:
            self.unit_type = unit_type
        elif unit_type is None and unit is None:
            self.unit_type = Unitless
        elif unit_type is None:
            self.unit_type = get_unit_type_from_unit_key(unit)

        if unit is None:
            unit = self.unit_type.std_unit
        self.unit = unit

        # build up lists of available unit ids and display names
        if unit_choices is None:
            self._unit_choices = self.unit_type.units()
            self._display_units = self.unit_type.display_units
        else:
            self._unit_choices = []
            self._display_units = []
            for un_id, un_disp in zip((self.unit_type.units(), self.unit_type.display_units)):
                if un_id in unit_choices:
                    self._unit_choices.append(un_id)
                    self._display_units.append(un_disp)

        self._value = self._clean_list(value, check_lims=False, do_round=False)  # value is list of floats in standard units
        self._min_value = self.unit_type.convert(min_value, old=self.unit)
        self._max_value = self.unit_type.convert(max_value, old=self.unit)

    def _clean_list(self, vals: list, convert_to_std=True, check_lims=True, do_round=True):
        """Converts list of floats to, optionally, standard units while checking limits. """
        new_vals = []
        if convert_to_std:
            for val in vals:  # value is list of floats in designated unit
                nv = self.unit_type.convert(val, old=self.unit)
                if do_round:
                    nv = np.format_float_positional(nv, precision=self._num_decimals, trim='-')
                new_vals.append(nv)
        else:
            new_vals = vals

        results = []
        if check_lims:
            for val in new_vals:
                if self._min_value <= val <= self._max_value:
                    results.append(val)
        else:
            results = new_vals
        return results

    def _update_decimals(self, arr: [float]):
        """Updates max used decimals within array to ensure displayed/converted values match user inputs. """
        max_decimals = 0
        for val in arr:
            dec = count_decimal_places(val)
            if dec > max_decimals:
                max_decimals = dec
        self._num_decimals = max_decimals

    @property
    def value(self):
        """ Returns value in selected units. If probabilistic, returns value as distribution. """
        disp_vals = []
        for val in self._value:
            nv = self.unit_type.convert(val, new=self.unit)
            nv = np.format_float_positional(nv, precision=self._num_decimals, trim='-')
            disp_vals.append(nv)
        return disp_vals

    @value.setter
    def value(self, vals: str or list):
        """ Sets stored standard value list according to active unit. """
        vals = convert_to_float_list(vals)
        # track # of decimals user inputted
        self._update_decimals(vals)
        new_vals = self._clean_list(vals, convert_to_std=True, check_lims=True, do_round=False)
        self._value = new_vals

        if self._track_changes:
            self.changed.notify(self)

    @property
    def value_raw(self):
        return self._value

    def set_value_raw(self, vals: str or list):
        """ Sets value assuming standard units. """
        vals = convert_to_float_list(vals)
        self._value = self._clean_list(vals, convert_to_std=False, check_lims=True)

        if self._track_changes:
            self.changed.notify(self)

    @property
    def min_value(self):
        if self._min_value == -np.inf:
            result = -np.inf
        else:
            result = self.unit_type.convert(self._min_value, new=self.unit, do_round=True)
        return result

    @min_value.setter
    def min_value(self, val: float):
        nv = self.unit_type.convert(val, old=self.unit)
        self._min_value = nv
        if self._track_changes:
            self.changed.notify(self)

    @property
    def max_value(self):
        if self._max_value == np.inf:
            result = np.inf
        else:
            result = self.unit_type.convert(self._max_value, new=self.unit, do_round=True)
        return result

    @max_value.setter
    def max_value(self, val: float):
        nv = self.unit_type.convert(val, old=self.unit)
        self._max_value = nv
        if self._track_changes:
            self.changed.notify(self)

    def set_from_model(self, vals: str or list):
        """Sets value via backend model. NOTE: assumes in std units. """
        vals = convert_to_float_list(vals)
        new_vals = self._clean_list(vals, convert_to_std=False, check_lims=True)
        super().set_from_model(new_vals)

    def get_unit_index(self):
        """ Returns int index of selected unit within its unit choices. e.g. bar in [MPa, bar, psi] would return 1. """
        if self.unit is None or self.unit_type in [Unitless]:
            return 0

        if self.unit not in self._unit_choices:
            raise ValueError(f"Unit {self.unit} not found in unit choices")
        result = self._unit_choices.index(self.unit)
        return result

    def set_unit_from_display(self, val: str):
        """ Sets active unit from a selected display value, not key. e.g. 'MPa' updates unit type to 'mpa'. """
        disp_units = self._display_units
        if len(disp_units) <= 1:
            return
        if val not in disp_units:
            raise ValueError('Display unit not found')

        old_vals = self.value  # in old units
        i = disp_units.index(val)
        self.unit = self._unit_choices[i]

        # displayed value now in new units so update raw values in batch to yield single change event
        tracking = self._track_changes
        self._track_changes = False
        self._value = self._clean_list(old_vals, convert_to_std=True, check_lims=False)
        self._track_changes = tracking
        self.changed.notify(self)

    @property
    def unit_choices_display(self):
        return self._display_units

    @property
    def min_value_str(self):
        """Minimum value formatted for display. """
        return get_num_str(self.min_value)

    @property
    def max_value_str(self):
        """Maximum value formatted for display. """
        return get_num_str(self.max_value)

    @property
    def value_str(self):
        """ Returns formatted string representation of converted value. """
        val_list_str = ""
        for val in self.value:
            val_str = get_num_str(val)
            val_list_str += f"{val_str}, "
        return val_list_str

    def get_value_tooltip(self):
        """Returns tooltip for parameter. """
        if self.tooltip is None:
            if self.min_value == 0 and self.max_value == np.inf:
                result = "Enter list of positive values separated by space (' ')"
            elif self.max_value == np.inf:
                result = f"Enter list of values >= {self.min_value_str}. Separate values with a space (' ')."
            else:
                result = f"Enter list of values between {self.min_value_str} and {self.max_value_str}. Separate values with a space (' ')."
            return result
        else:
            return self.tooltip

    def check_valid(self) -> ValidationResponse:
        resp = super().check_valid()
        if resp.status != InputStatus.GOOD:
            return resp

        msg = ""
        for val in self.value:
            if val < self.min_value:
                msg = f'{self.label} value {get_num_str(val)} below minimum ({self.min_value_str})'
            elif val > self.max_value:
                msg = f'{self.label} value {get_num_str(val)} above maximum ({self.max_value_str})'

        status = InputStatus.ERROR if msg else InputStatus.GOOD
        return ValidationResponse(status, msg)

    def to_dict(self) -> dict:
        """Provide data representation in dict format. All values are stored in standard (raw) format. """
        result = super().to_dict()
        # store as str to handle np.inf
        extra = {
            'min_value': get_num_str(self._min_value),
            'max_value': get_num_str(self._max_value),
            'unit_type': self.unit_type.label,
            'unit': self.unit,
        }
        result |= extra
        return result

    def from_dict(self, data: dict, notify_from_model=True, silent=False):
        """Set all values from dict. Assume all properties are present. """
        self.label = data['label']
        self.slug = data['slug']

        if 'unit_type' in data:
            unit_type_key = data['unit_type']
            unit_type = get_unit_type(unit_type_key)
            self.unit_type = unit_type
        if 'unit' in data:
            self.unit = data['unit']

        self.set_value_raw((data['value']))

        # limit may be stored as string
        min_str = data.get('min_value', -np.inf)
        max_str = data.get('max_value', np.inf)
        self._min_value = -np.inf if type(min_str) is str and 'infinity' in min_str else float(min_str)
        self._max_value = np.inf if type(max_str) is str and 'infinity' in max_str else float(max_str)

        if not silent:
            self.changed.notify(self)

            if notify_from_model:
                self.changed_by_model.notify(self)

    @property
    def str_display(self):
        """ Returns string-representation of parameter, including label.
        """
        if self.unit is None:
            unit_str = ""
        else:
            disp_unit_index = self.get_unit_index()
            unit_str = self._display_units[disp_unit_index]
        val = self.value_str
        result = f"{self.label} {unit_str}: {val}"
        return result
