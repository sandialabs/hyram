"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
from logging.config import dictConfig
import os
import datetime
import re

from CoolProp import CoolProp


def params_as_str(param_dict):
    """

    Parameters
    ----------
    param_dict : dict
        dict of parameter names and values

    Returns : str
    -------
    string of params and values, one per line

    """
    sorted_params = sorted(param_dict)
    msg = "PARAMETERS:"
    for param in sorted_params:
        msg += "\n{:<48s}{}: {}".format('', param, param_dict[param])
    return msg


def clean_name(name):
    """
    Convert string name to alphanumeric lower-case

    """
    parsed = re.sub(r'\W+', '', name.lower())
    return parsed


def parse_nozzle_model(name):
    """
    Determine correct nozzle model name by ensuring name string is lower-case alphanumeric (includes underscore).

    Parameters
    ----------
    name : str
        Name of notional nozzle model

    Returns
    -------

    """
    cleanstr = clean_name(name)
    if cleanstr in ['yuc', 'yuce', 'yuceil_otugen', 'yuceilotugen', 'yuceil-otugen']:
        ret_str = 'yuce'
    elif cleanstr in ['ewan', 'ewan_moodie', 'ewanmoodie', 'ewan-moodie']:
        ret_str = 'ewan'
    elif cleanstr in ['bir', 'birc', 'birch', 'birch1']:
        ret_str = 'birc'
    elif cleanstr in ['birch2' 'bir2', 'b2']:
        ret_str = 'bir2'
    elif cleanstr in ['molkov', 'molk', 'mol']:
        ret_str = 'molk'
    else:
        ret_str = cleanstr

    return ret_str


def convert_nozzle_model_to_params(nozzle_model, rel_fluid=None):
    """ Convert nozzle model to conservation and solve spec.
    Options are:

    Parameters
    ----------
    nozzle_model : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model ID

    rel_fluid: Fluid or None
        Required for birch models, provides temperature

    Returns
    -------
    con_mom : bool
        whether model conserves mass and momentum (True) or mass only (False)

    t_param : str

    """
    cleaned = parse_nozzle_model(nozzle_model)
    if cleaned == 'yuce':
        con_mom = True
        t_param = 'solve_energy'

    elif cleaned == 'ewan':
        con_mom = False
        t_param = 'Tthroat'

    elif cleaned == 'birc':
        con_mom = False
        t_param = rel_fluid.T

    elif cleaned == 'bir2':
        con_mom = True
        t_param = rel_fluid.T

    elif cleaned == 'molk':
        con_mom = False
        t_param = 'solve_energy'

    else:  # includes hars
        raise ValueError("Nozzle model not convertible to parameters")

    return con_mom, t_param


def get_now_str():
    """ Generates a string-formatted time, down to seconds """
    now = datetime.datetime.now()
    return now.strftime('%Y%m%d-%H%M')


def setup_file_log(output_dir, verbose=False, logfile='log_hyram.txt', logname='hyram'):
    """ Set up module logging.

    Parameters
    ----------
    output_dir : str
        Path to logfile directory

    """
    if not os.path.isdir(output_dir):
        os.makedirs(output_dir)

    logfile = os.path.join(output_dir, logfile)
    level = logging.INFO if verbose else logging.ERROR

    logging_config = {
        'version': 1,
        'disable_existing_loggers': False,
        'formatters': {
            'f': {'format': '%(asctime)s %(name)-12s %(levelname)-8s %(message)s'}
        },
        'handlers': {
            'h': {'class': 'logging.StreamHandler',
                  'formatter': 'f',
                  'level': level},
            'fh': {'class': 'logging.handlers.RotatingFileHandler',
                   'formatter': 'f',
                   'level': level,
                   'filename': logfile,
                   'mode': 'a',
                   'maxBytes': 4096000,
                   'backupCount': 10,
                   },
        },
        'root': {
            'handlers': ['h', 'fh'],
            'level': level,
        },
    }
    dictConfig(logging_config)
    log = logging.getLogger(logname)
    logging.getLogger('matplotlib.font_manager').disabled = True

    log.info("Log setup complete")


def parse_phase_key(key):
    """
    Convert phase string identifier into value for phys library.
    All values besides 'gas' and 'liquid' will be converted to None

    Parameters
    ----------
    key : str
        Phase identifier key

    Returns
    -------
    str or None

    """
    return key if key in ['gas', 'liquid'] else None


def is_valid_fluid_string(fluid_str):
    """ Validates fluid string based on CoolProp fluids. """
    valid = False
    for fluid in CoolProp.FluidsList():
        if fluid_str.lower() == str(fluid).lower():
            valid = True
            break

    return valid


def parse_blend_dict_into_coolprop_dict(blend_dict):
    """ Builds coolprop-ready dict with cleaned names from dict of fluids and concentrations.

    Parameters
    ----------
    blend_dict : dict
    fluids and concentrations, e.g. {'ch4': 0.96, 'h2': 0, 'n2': 0.04}

    """
    epsilon = 0.00001
    if abs(sum(blend_dict.values()) - 1) > epsilon:
        raise ValueError(f"Total blend mole fraction must equal 1.0. Provided fraction was {sum(blend_dict.values())}")
    species_dict = {}
    total_frac = 0
    for key, val in blend_dict.items():
        if val <= 0.:
            continue

        cleaned = parse_fluid_key(key)
        if is_valid_fluid_string(cleaned):
            val = round(val, 6)
            total_frac += val
            species_dict[cleaned] = val
        else:
            raise ValueError(f"'{cleaned}' is not a valid fluid name")

    return species_dict


def parse_coolprop_dict_into_string(cdict):
    """
    Converts dict of fluids and concentrations into concatenated string.

    Parameters
    ----------
    cdict : dict

    Returns
    -------

    """
    if len(list(cdict.values())) == 1:
        s = list(cdict.keys())[0]
    else:
        s = '&'.join(['%s[%f]' % (s, X) for s, X in zip(cdict.keys(), cdict.values())])
    return s


def is_parsed_blend_string(fluid_str):
    """
    True if string is combination of fuel names and concentrations.
    Examples: True for HYDROGEN[1.000] and [HYDROGEN[0.5]&METHANE[0.5]
    """
    return '[' in fluid_str or '&' in fluid_str


def parse_fluid_key(key: str) -> str:
    """
    Parses fluid name to match valid CoolProp fluid strings.
    Note that 'n-' fluid names must have a capitalized root when passed to CoolProp.
    i.e. n-propane will fail but n-Propane will work.

    """
    key = key.lower()
    if key in ['hydrogen', 'hy', 'h2']:
        result = 'Hydrogen'
    elif key in ['methane', 'ch4']:
        result = 'Methane'
    elif key in ['propane', 'c3h8', 'n-propane']:
        result = 'n-Propane'
    elif key in ['nitrogen', 'n2']:
        result = 'Nitrogen'
    elif key in ['carbondioxide', 'co2', 'carbon dioxide']:
        result = 'CarbonDioxide'
    elif key in ['ethane', 'c2h6']:
        result = 'Ethane'
    elif key in ['n-butane', 'nbutane', 'n-c4h10']:
        result = 'n-Butane'
    elif key in ['n-pentane', 'npentane', 'n-c5h12']:
        result = 'n-Pentane'
    elif key in ['n-hexane', 'nhexane', 'n-c6h14']:
        result = 'n-Hexane'

    else:
        matched_names = [fluid for fluid in CoolProp.FluidsList() if fluid.lower() == key]
        if matched_names:
            result = matched_names[0]
        else:
            raise ValueError(f'Name ({key}) not found in CoolProp')

    return result


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


def get_temp_folder(temp_dir_name='temp'):
    """
    Returns location of temporary folder
    and creates it if needed

    Parameters
    ----------
    temp_dir_name : str, optional
        Name of temporary folder (default is 'temp')

    Returns
    -------
    temp_dir_path : str
        absolute path to temporary folder
    """
    temp_dir_path = os.path.join(os.getcwd(), temp_dir_name)
    if not os.path.isdir(temp_dir_path):
        os.mkdir(temp_dir_path)
    return temp_dir_path
