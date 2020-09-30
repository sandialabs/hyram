import logging
from logging.config import dictConfig
import os
import datetime
import re

import numpy as np

from .exceptions import InputError
from . import constants

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
    elif species == "c2h6":
        num_atoms = 2
    elif species == "c3h8":
        num_atoms = 3
    else:
        raise InputError("Species {} not recognized".format(species))

    return num_atoms


def get_now_str():
    """ Generates a string-formatted time, down to seconds """
    now = datetime.datetime.now()
    return now.strftime('%Y%m%d-%H%M')


def setup_file_log(output_dir, debug, logfile='hyram.log', logname='hyram'):
    """
    Set up module logging. Called by C# GUI.

    Parameters
    ----------
    output_dir : str
        Path to directory for e.g. logging

    """
    if not os.path.isdir(output_dir):
        os.mkdir(output_dir)

    logfile = os.path.join(output_dir, logfile)
    # formatter = logging.Formatter('%(asctime)s - %(name)s - %(levelname)s - %(message)s')
    logging_config = {
        'version': 1,
        'disable_existing_loggers': False,
        'formatters': {
            'f': {'format': '%(asctime)s %(name)-12s %(levelname)-8s %(message)s'}
        },
        'handlers': {
            'h': {'class': 'logging.StreamHandler',
                  'formatter': 'f',
                  'level': logging.DEBUG},
            'fh': {'class': 'logging.handlers.RotatingFileHandler',
                   'formatter': 'f',
                   'level': logging.DEBUG,
                   'filename': logfile,
                   'mode': 'a',
                   'maxBytes': 4096000,
                   'backupCount': 10,
                   },

        },
        'loggers': {
            logname: {
                'level': logging.DEBUG,
                'handlers': ['h', 'fh'],
            }
        },
        'root': {
            'handlers': ['h', 'fh'],
            'level': logging.DEBUG,
        },
    }
    dictConfig(logging_config)
    log = logging.getLogger(logname)
    # log = logging.getLogger().addHandler(logging.NullHandler())

    if debug:
        # Do this in separate step to keep descendant loggers at their original level
        log.setLevel(logging.DEBUG)
    else:
        log.setLevel(logging.ERROR)

    log.info("PYTHON LOGGING ACTIVATED")


def convert_component_prob_lists_to_dicts(leak_prob_sets):
    """
    Convert probability sets for single component to lists of dicts. Inner lists become {mu, sigma, mean, var}.
    Note that Python.NET converter func can't currently convert nullable list (e.g. double?) to numpy array.
    If mu/sigma or mean/variance are null in C#, they're set to -1000D.

    Parameters
    ----------
    leak_prob_sets : list
    List of lists of probability data. Inner list is ordered: mu, sigma, mean, variance.

    Returns
    -------
    prob_dicts : list
    List of dicts where inner list is dict. {mu, sigma, mean, variance}

    """
    prob_dicts = []
    if type(leak_prob_sets) == np.ndarray:
        leak_prob_sets = list(leak_prob_sets)

    for prob_set in leak_prob_sets:
        mu, sigma = prob_set

        prob_dict = {
            'mu': mu,
            'sigma': sigma,
        }
        prob_dicts.append(prob_dict)

    return prob_dicts


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
    return key if key in constants.PHASE_IDS else None
