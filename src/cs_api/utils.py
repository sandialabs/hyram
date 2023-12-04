"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import os
import logging
from logging.config import dictConfig

import json
import ctypes
import numpy as np
# NOTE (Cianan): these imports may be unused here but are required by python.net
import clr
import System
from System.Runtime.InteropServices import GCHandle, GCHandleType

_MAP_NET_NP = {
    'Single': np.dtype('float32'),
    'Double': np.dtype('float64'),
    'SByte': np.dtype('int8'),
    'Int16': np.dtype('int16'),
    'Int32': np.dtype('int32'),
    'Int64': np.dtype('int64'),
    'Byte': np.dtype('uint8'),
    'UInt16': np.dtype('uint16'),
    'UInt32': np.dtype('uint32'),
    'UInt64': np.dtype('uint64'),
    'Boolean': np.dtype('bool'),
}


def convert_cnet_dictionary_to_dict(cnet_dict):
    """ Converts C# Dictionary<string, string> into python dict of {string: float or None} """
    result = {}
    for entry in cnet_dict:
        key = entry.Key
        val = str(entry.Value).strip().lower()
        val = None if val in ['none', 'null', '', None] else float(val)
        result[key] = val

    return result


def convert_2d_array_to_numpy_array(cnet_2darray):
    """
    Convert [][] Array to numpy equivalent.

    Parameters
    ----------
    cnet_2darray : System.Array
    2D array from C#.

    Returns
    -------
    2D numpy array

    """
    ndarr = np.array([convert_to_numpy_array(cn_arr) for cn_arr in cnet_2darray])
    return ndarr


def convert_to_numpy_array(cnet_array):
    """
    Converts CLR System.Array into numpy.ndarray.
    See _MAP_NET_NP for the mapping of CLR types to Numpy dtypes.
    Reference: https://github.com/pythonnet/pythonnet/issues/514

    Parameters
    ----------
    cnet_array : System.Array
        Array from C#

    Returns
    -------
    ndarray

    """
    if type(cnet_array) == list:
        return np.array(cnet_array)
    elif cnet_array is None:
        raise ValueError('cnet_array is None')
    elif type(cnet_array) == np.ndarray:
        return cnet_array

    dims = np.empty(cnet_array.Rank, dtype=int)
    for I in range(cnet_array.Rank):
        dims[I] = cnet_array.GetLength(I)
    cnet_type = cnet_array.GetType().GetElementType().Name

    try:
        np_array = np.empty(dims, order='C', dtype=_MAP_NET_NP[cnet_type])
    except KeyError:
        raise NotImplementedError("asNumpyArray does not yet support System type {}".format(cnet_type))

    try:  # Memmove
        src_handle = GCHandle.Alloc(cnet_array, GCHandleType.Pinned)
        src_ptr = src_handle.AddrOfPinnedObject().ToInt64()
        dest_ptr = np_array.__array_interface__['data'][0]
        ctypes.memmove(dest_ptr, src_ptr, np_array.nbytes)
    finally:
        if src_handle.IsAllocated:
            src_handle.Free()

    return np_array


def convert_occupant_json_to_dicts(occ_json):
    """
    Convert C# JSON input into list of dicts in correct format.
    Incoming dict format is:
        {NumTargets, Desc, XLocDistribution (int), XLocParamA, XLocParamB, ParamUnitType, ExposureHours}

    Outgoing format is:
        {count, descrip, xdistr, xa, xb, ydistr, ya, yb, zdistr, za, zb, hours}

    Note that incoming distances are always in meters.

    Parameters
    ----------
    occ_json : JSON

    Returns
    -------

    """
    occ_cnet_list = json.loads(occ_json)
    occ_groups = []

    for cnet_group in occ_cnet_list:
        # distributions from enum so convert to string representing normal, uniform, deterministic (constant)
        distr_labels = ['norm', 'unif', 'dete']
        x_distr = distr_labels[cnet_group['XLocDistribution']]
        y_distr = distr_labels[cnet_group['YLocDistribution']]
        z_distr = distr_labels[cnet_group['ZLocDistribution']]

        group = {
            'count': cnet_group['NumTargets'],
            'descrip': cnet_group['Desc'],
            'hours': cnet_group['ExposureHours'],
            'xdistr': x_distr,
            'xa': cnet_group['XLocParamA'],
            'xb': cnet_group['XLocParamB'],
            'ydistr': y_distr,
            'ya': cnet_group['YLocParamA'],
            'yb': cnet_group['YLocParamB'],
            'zdistr': z_distr,
            'za': cnet_group['ZLocParamA'],
            'zb': cnet_group['ZLocParamB'],
        }
        occ_groups.append(group)

    return occ_groups


def setup_file_log(output_dir, verbose=False, logfile='log_hyram.txt', logname='hyram') -> str:
    """ Set up module logging.

    Parameters
    ----------
    output_dir : str
        Path to logfile directory
    verbose : bool, optional
        Toggles level of logging
    logfile : str, optional
        Logfile name, defaults to 'log_hyram.txt'
    logname : str, optional
        Base level log name, defaults to 'hyram'

    Returns
    -------
    filepath : str
        path to file log
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
    return logfile


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