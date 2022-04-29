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

from .exceptions import InputError


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
        raise InputError("Nozzle model conversion", "Nozzle model not convertible")

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
        os.mkdir(output_dir)

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

    # Can set level in separate step to keep descendant loggers at their original level
    # log.setLevel(level)
    # for handler in log.handlers:
    #     handler.setLevel(level)

    log.info("Log setup complete")


def is_fluid_specified(temp=None, pres=None, density=None, phase=None):
    """ Verify that fluid is defined by exactly two parameters """
    num_params = len([x for x in [temp, pres, density, phase] if x is not None])
    return num_params == 2


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
