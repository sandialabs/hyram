"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import logging
import os
import sys
from pathlib import Path
import tempfile

# Default interactive backend causes bouncing dock icons in macOS.
import matplotlib
from .utils import helpers


matplotlib.use("Agg")


"""
This module contains default application-wide settings and parameters. Functionality and parameters here must be idempotent.
Do not import these settings directly. Import these into an app-specific settings file and then import that file.

"""

DEBUG = False
VERBOSE = True
USE_LOGFILE = True

APPNAME = ""
APPNAME_LOWER = ""
VERSION = ""

# Ascertain platform (Windows or Mac)
IS_WINDOWS = True
WINDOWS_APP_ID = ""
try:
    from ctypes import windll  # Only exists on Windows.
    IS_WINDOWS = True
except ImportError:
    IS_WINDOWS = False
except ModuleNotFoundError:
    IS_WINDOWS = False

# Disable to see exceptions in dev terminal
USE_SUBPROCESS = True
MP_POOL = None

# Max delay time, in seconds, in which to gracefully shut down thread and pool after user quits app.
SHUTDOWN_TIMER = 30

BASE_DIR = Path(os.path.dirname(__file__))

# Persistent application data directory for e.g. configuration files
DATA_DIR = None

# session_dir name includes time at app startup. Sub-processes must be passed its value directly.
SESSION_DIR = None
# Whether user manually set session dir
USER_SET_SESSION_DIR = False

# global app configuration
CONF_FILEPATH = None

# Check if running from pyinstaller bundle, i.e. installed as app
if getattr(sys, 'frozen', False) and hasattr(sys, '_MEIPASS'):
    APPLICATION_MODE = True
    CWD_DIR = Path(sys._MEIPASS)
else:
    APPLICATION_MODE = False
    CWD_DIR = BASE_DIR.parent

# Dev-only access to in-development module code without reloading/reinstalling
# if not APPLICATION_MODE:
#     pkg_path = Path.cwd().parents[1].joinpath('src')
#     sys.path.insert(0, pkg_path.as_posix())

LOG_FORMAT = "%(asctime)s - %(process)d - %(levelname)s - %(message)s"
LOG_DT_FORMAT = "%d-%b-%y %H:%M:%S"
