"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
from hyramgui.hygu.utils.units_of_measurement import UnitType
import scipy.constants as scp


class Pressure(UnitType):
    """Pressure unit options. """
    label = 'pres'
    unit_data = {'pa': 1, 'kpa': scp.kilo, 'mpa': scp.mega, 'psi': scp.psi, 'atm': scp.atm, 'bar': scp.bar}
    display_units = ['Pa', 'kPa', 'MPa', 'psi', 'atm', 'bar']
    pa = 'pa'
    kpa = 'kpa'
    mpa = 'mpa'
    psi = 'psi'
    atm = 'atm'
    bar = 'bar'
    std_unit = 'pa'


class Time(UnitType):
    label = 'time'
    unit_data = {'hr': scp.hour, 'min': scp.minute, 'sec': 1, 'ms': scp.milli}
    display_units = ['hr', 'min', 's', 'ms']
    hr = 'hr'
    min = 'min'
    sec = 'sec'
    ms = 'ms'
    std_unit = 'sec'


class Area(UnitType):
    label = 'area'
    unit_data = {'m2': 1, 'cm2': scp.centi**2, 'mm2': scp.milli**2, 'in2': scp.inch**2, 'ft2': scp.foot**2, 'yd2': scp.yard**2}
    display_units = ['m<sup>2</sup>', 'cm<sup>2</sup>', 'mm<sup>2</sup>', 'in<sup>2</sup>', 'ft<sup>2</sup>', 'yd<sup>2</sup>']
    m2 = 'm2'
    cm2 = 'cm2'
    mm2 = 'mm2'
    in2 = 'in2'
    ft2 = 'ft2'
    yd2 = 'yd2'
    std_unit = 'm2'


class Volume(UnitType):
    label = 'volume'
    unit_data = {'l': scp.liter, 'm3': 1, 'mm3': scp.milli**3, 'cm3': scp.milli**2, 'ml': scp.milli**2,
                 'in3': scp.inch**3, 'ft3': scp.foot**3}
    display_units = ['L', 'm<sup>3</sup>', 'mm<sup>3</sup>', 'cm<sup>3</sup>', 'mL', 'in<sup>3</sup>', 'ft<sup>3</sup>']
    l = 'l'
    m3 = 'm3'
    mm3 = 'mm3'
    cm3 = 'cm3'
    ml = 'ml'
    in3 = 'in3'
    ft3 = 'ft3'
    std_unit = 'm3'


class Density(UnitType):
    label = 'density'
    unit_data = {'kgpm3': 1, 'gpm3': 1/scp.kilo, 'gpcm3': 1/scp.centi**3/scp.kilo, 'kgpl': 1/scp.liter, 'gpl': 1/scp.liter/scp.kilo, 'lbpft3': scp.pound/scp.foot**3}
    display_units = ['kg/m<sup>3</sup>', 'g/m<sup>3</sup>', 'g/cm<sup>3</sup>', 'kg/L', 'g/l', 'lb/ft<sup>3</sup>']
    std_unit = 'kgpm3'
    kgpm3 = 'kgpm3'
    gpm3 = 'gpm3'
    gpcm3 = 'gpcm3'
    kgpl = 'kgpl'
    gpl = 'gpl'
    lbpft3 = 'lbpft3'


class Mass(UnitType):
    label = 'mass'
    unit_data = {'mg': scp.milli/scp.kilo, 'g': 1/scp.kilo, 'kg': 1, 'lb': scp.pound}
    display_units = ['mg', 'g', 'kg', 'lb']
    std_unit = 'kg'
    mg = 'mg'
    g = 'g'
    kg = 'kg'
    lb = 'lb'
