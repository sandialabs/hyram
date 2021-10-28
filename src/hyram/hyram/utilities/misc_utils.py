"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging
from logging.config import dictConfig
import os
import datetime
import re

import numpy as np

from .exceptions import InputError

try:
    import cPickle as pickle  # C module
except ModuleNotFoundError:
    import pickle


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


def get_num_carbon_atoms_from_species(species):
    """ Returns int # carbon atoms based on species input """
    species = str(species).lower()
    if species == "h2":
        num_atoms = 0
    elif species == "ch4":
        num_atoms = 1
    # elif species == "c2h6":
    #     num_atoms = 2
    elif species == "c3h8":
        num_atoms = 3
    else:
        raise InputError("Species {} not recognized".format(species))

    return num_atoms


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


def convert_ign_prob_lists_to_dicts(immed_ign_probs, delayed_ign_probs, thresholds):
    """
    Convert lists of ignition data into list of dicts with one entry per rate group.

    Parameters
    ----------
    immed_ign_probs : list
        Immediate ignition probabilities for each group
    delayed_ign_probs : list
        Delayed ignition probabilities for each group
    thresholds : list
        Ignition release rate thresholds for each group (kg/s). floats.

    Returns
    -------
    ign_dict : list of dicts
        Each entry is: {threshold_min, threshold_max, immed_prob, delay_prob}

    """
    num_groups = len(immed_ign_probs)
    ign_dicts = []

    for i in range(num_groups):
        if i == 0:
            thres_min = -np.inf
            thres_max = thresholds[i]

        elif i == (num_groups - 1):
            thres_min = thresholds[-1]
            thres_max = np.inf

        else:
            thres_min = thresholds[i-1]
            thres_max = thresholds[i]

        immed_prob = immed_ign_probs[i]
        delay_prob = delayed_ign_probs[i]

        ign_dicts.append({
            'threshold_min': thres_min,
            'threshold_max': thres_max,
            'immed_prob': immed_prob,
            'delay_prob': delay_prob,
        })

    return ign_dicts


def save_object(filepath, obj):
    """
    Save object to file via pickling

    Parameters
    ----------
    filepath : str
        Location of file in which to store object, including its path.

    obj : object
        Object to store in file

    """
    with open(filepath, 'wb') as outfile:
        pickle.dump(obj, outfile, pickle.HIGHEST_PROTOCOL)


def load_object(filepath):
    """
    Load existing object from file via pickling

    Parameters
    ----------
    filepath : str
        Location of file in which to store object, including its path.

    Returns
    ----------
    obj : object
        Retrieved object
    """
    with open(filepath, 'rb') as infile:
        obj = pickle.load(infile)
        return obj


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
