"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import math


def round_to_sig_figs(num, sig_figs):
    if num == 0:
        return 0
    # Calculate the number of decimal places to round to
    decimal_places = sig_figs - int(math.floor(math.log10(abs(num)))) - 1
    factor = 10 ** decimal_places
    return round(num * factor) / factor


def get_fluid_formula_from_name(name):
    """ Returns shortened formula from name or name-like. For example, 'Hydrogen' returns 'h2'. """
    name = name.lower()
    result = name
    if name in ['h2', 'hydrogen', 'hy']:
        result = 'h2'
    elif name in ['ch4', 'methane']:
        result = 'ch4'
    elif name in ['c3h8', 'n-propane', 'propane']:
        result = 'c3h8'
    elif name in ['nitrogen', 'n2']:
        result = 'n2'
    elif name in ['carbon dioxide', 'co2']:
        result = 'co2'
    elif name in ['carbon monoxide', 'co']:
        result = 'co'
    elif name in ['ethane', 'c2h6']:
        result = 'c2h6'
    elif name in ['n-butane', 'nbutane', 'n-c4h10']:
        result = 'n-butane'
    elif name in ['isobutane', 'iso-butane']:
        result = 'isobutane'
    elif name in ['n-pentane', 'npentane', 'n-c5h12']:
        result = 'n-pentane'
    elif name in ['isopentane', 'iso-pentane']:
        result = 'isopentane'
    elif name in ['n-hexane', 'nhexane', 'n-c6h14']:
        result = 'n-hexane'
    return result


def get_fluid_formula_from_blend_str(s):
    """
    Returns first formula from longer blend string. Examples:
    Hydrogen[1.00] -> H2
    Methane[0.5]&Propane[0.5] -> CH4

    """
    species_name = s
    if '[' in s:
        species_name = s.split('[')[0]
    result = get_fluid_formula_from_name(species_name)

    return result