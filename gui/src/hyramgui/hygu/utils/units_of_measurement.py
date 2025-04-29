"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""


from .helpers import hround
import scipy.constants as scp


def c_to_k(val):
    """Converts Celsius temperate value to kelvin.

    """
    return val + scp.zero_Celsius


def k_to_c(val):
    """Converts kelvin temperature value to Celsius.

    """
    return val - scp.zero_Celsius


def f_to_k(val):
    """Converts Fahrenheit temperature value to kelvin.

    """
    result = (val - 32) * (5/9) + scp.zero_Celsius
    return result


def k_to_f(val):
    """Converts kelvin temperature value to Fahrenheit.

    """
    result = (val - scp.zero_Celsius) * (9/5) + 32
    return result


def get_unit_type(unit_type: str):
    """Returns UnitType class corresponding to string representation.

    Parameters
    ----------
    unit_type : str
        Unit type, e.g. 'unitless' or 'temperature'.

    Returns
    -------
    UnitType
        UnitType class corresponding to string key.

    """
    unit_type = unit_type.lower().strip()
    result = UNIT_TYPES_DICT.get(unit_type, None)
    return result


def get_unit_type_from_unit_key(key: str):
    """
    Returns UnitType containing the given unit represented by string key. Returns Unitless if key not found.
    For example, 'm' returns Distance.

    Parameters
    ----------
    key : str
        string representation of unit, e.g. 'm' or 'psi'

    Returns
    -------
    UnitType
        UnitType containing the specified unit.

    """
    key = key.lower().strip()

    result = Unitless
    for g in UNIT_TYPES_LIST:
        if key in g.units():
            result = g
            break
    return result


class UnitType:
    """
    Base class which describes a set of units of measurement, e.g. distance. These classes are primarily used for unit conversions.

    Attributes
    ----------
    label : str
        Short descriptor of the class of units of measurement; e.g. 'Distance'.
    unit_data : dict
        {string: float or function} Ordered pairs indicating the unit and conversion factor or function that converts from the standard unit
        to that unit; e.g. {'mm': 0.001}.
    display_units: list
        String descriptors of units which are ready for display. These can include HTML and must be ordered according to unit_data pairs.
    std_unit: str or None
        key of standard unit; e.g. 'm' for Distance.

    Notes
    -----
    Unit keys should be unique across UnitType, except for SmallDistance.

    """
    label: str
    unit_data: dict
    display_units: list
    std_unit: str or None  # must match value of std unit property, not it's name. e.g. '%' for percentage, not 'p'
    scale_base: float or None = None  # if scale unit (e.g. temperature), this is the base value in std units

    @classmethod
    def convert(cls, val, old=None, new=None, do_round=False):
        """
        Converts value from old to new units, with optional rounding.

        Notes
        -----
        It is usually better to store an un-rounded parameter value and only round for display.

        Parameters
        ----------
        val : float
            Value to convert.

        old : str or None
            Units of input value. If None, standard unit (e.g. meters) will be used.

        new : str or None
            Units of output. If None, standard unit (e.g. meters) will be used.

        do_round : bool, default=False
            Whether output should be rounded.

        Returns
        -------
        float or None
            Value converted from old units, into new units.

        """
        if val is None:
            return None
        old_c = cls.unit_data[cls.std_unit] if old is None else cls.unit_data[old]
        new_c = cls.unit_data[cls.std_unit] if new is None else cls.unit_data[new]
        result = old_c * val / new_c
        if do_round:
            result = hround(result)
        return result

    @classmethod
    def units(cls) -> list:
        """Returns list of units available in UnitType class, as string keys.

        """
        return list(cls.unit_data.keys())


class Unitless(UnitType):
    """Unit type representing lack of units.

    """
    label = 'unitless'
    unit_data = {}
    display_units = ['--']
    std_unit = None

    @classmethod
    def convert(cls, val, do_round=False, **kwargs):
        if do_round:
            val = hround(val)
        return val


class Fractional(UnitType):
    """Unit type for fractional and percent-based values. Stored as decimal out of 1; i.e. 25% is stored as 0.25."""
    label = 'frac'
    unit_data = {'fr': 1, '%': 0.01}
    display_units = ['fraction', '%']
    fr = 'fr'
    p = '%'
    std_unit = 'fr'


class BasicPercent(UnitType):
    """Unit type for displaying percent-based values, 0 to 100. This is a convenience option with less complexity than Fractional."""
    label = 'perc'
    unit_data = {'p': 1}
    display_units = ['%']
    p = 'p'
    std_unit = 'p'


class Distance(UnitType):
    """Default unit type indicating distance.

    """
    label = 'dist'
    unit_data = {'m': 1, 'mm': scp.milli, 'km': scp.kilo, 'in': scp.inch, 'ft': scp.foot, 'mi': scp.mile}
    display_units = ['m', 'mm', 'Km', 'in', 'ft', 'mi']
    m = 'm'
    mm = 'mm'
    km = 'km'
    inch = 'in'
    ft = 'ft'
    mi = 'mi'
    std_unit = 'm'


class SmallDistance(UnitType):
    """Specialized distance unit type which only includes units equal to or smaller than meters.
    This class is not assigned automatically and must be set by user.

    """
    label = 'dist_sm'
    unit_data = {'m': 1, 'mm': scp.milli, 'in': scp.inch, 'ft': scp.foot}
    display_units = ['m', 'mm', 'in', 'ft']
    m = 'm'
    mm = 'mm'
    inch = 'in'
    ft = 'ft'
    std_unit = 'm'


class Pressure(UnitType):
    """Pressure unit type.

    """
    label = 'pres'
    unit_data = {'mpa': 1, 'psi': scp.psi * scp.micro, 'bar': scp.bar * scp.micro}
    display_units = ['MPa', 'psi', 'bar']
    mpa = 'mpa'
    psi = 'psi'
    bar = 'bar'
    std_unit = 'mpa'


class Temperature(UnitType):
    """Temperature unit type.

    """
    label = 'temp'
    unit_data = {'k': 1, 'c': c_to_k, 'f': f_to_k}  # converting FROM unit to std
    unit_data_out = {'k': 1, 'c': k_to_c, 'f': k_to_f}  # converting TO unit from std
    display_units = ['K', '&deg;C', '&deg;F']
    k = 'k'
    c = 'c'
    f = 'f'
    std_unit = 'k'
    scale_base = scp.zero_Celsius

    @classmethod
    def convert(cls, val, old=None, new=None, do_round=False):
        if val is None:
            return None
        old_c = 1 if old is None else cls.unit_data[old]
        new_c = 1 if new is None else cls.unit_data_out[new]
        result = old_c(val) if callable(old_c) else old_c * val
        result = new_c(result) if callable(new_c) else new_c * result
        if do_round:
            result = hround(result)
        return result


class Fracture(UnitType):
    """Fracture unit type.

    """
    label = 'fracture'
    unit_data = {'mpm': 1}
    display_units = ['MPa-m<sup>1/2</sup>']
    mpm = 'mpm'
    std_unit = 'mpm'


class MassFlow(UnitType):
    """Mass flow rate unit type.
    """
    label = 'massflow'
    unit_data = {'kgs': 1}
    display_units = ['kg/s']
    kgs = 'kgs'
    std_unit = 'kgs'


class Angle(UnitType):
    """Angle units
    """
    label = 'angle'
    unit_data = {'rad': 1, 'deg': scp.degree}
    display_units = ['rad', 'deg']
    rad = 'rad'
    deg = 'deg'
    std_unit = 'rad'


UNIT_TYPES_DICT = {
    'unitless': Unitless,
    'dist': Distance,
    'dist_sm': SmallDistance,
    'pres': Pressure,
    'temp': Temperature,
    'fracture': Fracture,
    'frac': Fractional,
    'perc': BasicPercent,
    'massflow': MassFlow,
    'angle': Angle,
}

UNIT_TYPES_LIST = [SmallDistance, Distance, Pressure, Temperature, Fracture, Fractional, BasicPercent, MassFlow, Angle]

