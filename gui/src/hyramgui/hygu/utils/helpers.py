"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import enum
import os
import re
import sys

import numpy as np
from datetime import datetime
from pathlib import Path

""" Generic convenience functions. Note that this file should not import any app-specific code. """


class InputStatus(enum.IntEnum):
    ERROR = 0
    GOOD = 1
    INFO = 2
    WARN = 3


class ValidationResponse:
    status: InputStatus
    message: str = ""

    def __init__(self, stat=InputStatus.GOOD, msg=''):
        self.status = stat
        self.message = msg


def hround(x, p=5):
    """Returns rounded value:
        if decimal, round to 5 significant digits
        if non-decimal, round to 4 decimal places

    References:
        https://stackoverflow.com/a/59888924/875127

    """
    if np.abs(x) < 1:
        x = np.asarray(x)
        x_positive = np.where(np.isfinite(x) & (x != 0), np.abs(x), 10 ** (p - 1))
        mags = 10 ** (p - 1 - np.floor(np.log10(x_positive)))
        return np.round(x * mags) / mags

    else:
        return np.round(x, p)


def get_num_str(val) -> str:
    """Returns formatted string representation of converted value. """
    if val == -np.inf:
        result = '-infinity'
    elif val == np.inf:
        result = '+infinity'
    elif val is None:
        result = ""

    else:
        abs_val = abs(val)
        if abs_val > 1000:
            result = f"{val:.0e}"
        elif abs_val >= 1:
            result = f"{val:.1f}"
        elif abs_val >= 0.01:
            result = f"{val:.3f}"
        elif abs_val == 0:
            result = "0"
        else:
            result = f"{val:.3e}"
    return result


def init_session_dir(parent_dir) -> Path:
    """ Creates directory for logs and output files. """
    started_at = datetime.now()
    session_dirname = started_at.strftime('session_%Y%m%d_%H%M%S')
    session_dir = parent_dir.joinpath(session_dirname)
    session_dir.mkdir(parents=True, exist_ok=True)
    return session_dir


def init_app_data_dir(appname: str) -> Path:
    """ Creates application data directory. """
    parent_dir = get_app_data_dir(appname)
    Path.mkdir(parent_dir, parents=True, exist_ok=True)
    return parent_dir


def get_app_data_dir(appname: str) -> Path:
    """ Returns platform-specific path to application directory for persistent storage. """
    if sys.platform == "win32":
        parent_dir = Path(os.getenv('APPDATA'))  # roaming/
    else:
        # macOS
        parent_dir = Path('~/Library/Application Support/').expanduser()

    result = parent_dir.joinpath(appname)
    return result


def convert_string_to_filename(st):
    """Removes characters which aren't letters, numbers, underscores, dashes, periods. """
    return re.sub(r'(?u)[^-\w.]', '_', st)


def get_now_str():
    return datetime.now().strftime('%y%m%d%H%M%S')


def count_decimal_places(num):
    """ Calculates number of decimal places in a number. """
    num_str = str(num)
    decimal_point_index = num_str.find('.')
    if decimal_point_index == -1:
        return 0
    decimal_places = len(num_str) - decimal_point_index - 1
    return decimal_places


def convert_to_float_list(vals: str or list or float):
    """Converts str of float vals, or single float, to list of floats. """
    if isinstance(vals, str):
        val_strs = vals.split(' ')
        list_f = []
        for val_s in val_strs:
            if val_s.strip() == '':
                continue
            list_f.append(float(val_s))
        vals = list_f

    if isinstance(vals, float):
        vals = [vals]
    return vals


def slim_arr(arr, max_len=150, x_decimals=None, y_decimals=None):
    """Returns slimmed-down ndarray. """
    if isinstance(arr, list):
        arr = np.array(arr)
    if len(arr) > max_len:
        step = int(len(arr) / max_len)
        arr = arr[0::step]

    if x_decimals is not None:
        arr.T[0] = np.round(arr.T[0], x_decimals)
    if y_decimals is not None:
        arr.T[1] = np.round(arr.T[1], y_decimals)
    return arr

