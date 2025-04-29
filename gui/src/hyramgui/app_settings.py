"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import configparser
import logging

from hyramgui.hygu.global_settings import *


"""
This module contains application-wide settings and parameters. Functionality and parameters here must be idempotent.

Note: convert this to lazy-loaded settings class that populates from global settings and this, for cleaner configuration.

"""

DEBUG = False
VERBOSE = True
USE_LOGFILE = True

APPNAME = "HyRAM+"
APPNAME_LOWER = APPNAME.lower()
VERSION = "6.0.0"

BASE_DIR = Path(os.path.dirname(__file__))
SESSION_DIR = None
USER_SET_SESSION_DIR = False

root = logging.getLogger(f"{APPNAME}")
root.setLevel(logging.DEBUG if DEBUG else logging.INFO)


def init():
    """ Finishes initializing app-specific and derived settings and loads config file. """
    if IS_WINDOWS:
        version_str = VERSION.replace('.', '_')  # must be of form 'V1_2_3'
        WINDOWS_APP_ID = f'com.SandiaNationalLaboratories.{APPNAME}.{version_str}'
        windll.shell32.SetCurrentProcessExplicitAppUserModelID(WINDOWS_APP_ID)

    global DATA_DIR
    DATA_DIR = helpers.init_app_data_dir(appname=APPNAME)

    global SESSION_DIR
    global USER_SET_SESSION_DIR

    # parse configuration file of persistent settings
    config_file = DATA_DIR.joinpath('config.ini')
    config = configparser.ConfigParser()

    if config_file.exists():
        config.read(config_file.as_posix())

        # Parse user-set session directory. Will create it if doesn't exist.
        ses_dir = config.get('DEFAULT', 'session_dir', fallback=None)
        if ses_dir not in ["", None]:
            try:
                SESSION_DIR = Path(ses_dir)
                if not SESSION_DIR.exists():
                    SESSION_DIR.mkdir(parents=True, exist_ok=True)
                USER_SET_SESSION_DIR = True
            except Exception:
                SESSION_DIR = None
                USER_SET_SESSION_DIR = False

    else:
        # if it doesn't exist (i.e. first run) create it
        config['DEFAULT'] = {'session_dir': ""}
        with open(config_file.as_posix(), 'w') as fl:
            config.write(fl)

    # Create/set defaults if config processing not available or failed
    if SESSION_DIR in [None, ""]:
        # Not user-set so create default directory
        SESSION_DIR = helpers.init_session_dir(parent_dir=DATA_DIR)

    log_dir = SESSION_DIR

    # set up logging
    if USE_LOGFILE and log_dir is not None and log_dir.exists():
        log_file = log_dir.joinpath(f'{APPNAME}.log')
        f_handler = logging.FileHandler(log_file.as_posix())
        f_handler.setLevel(logging.INFO)
        f_format = logging.Formatter(LOG_FORMAT, datefmt=LOG_DT_FORMAT)
        f_handler.setFormatter(f_format)
        root.addHandler(f_handler)

    # Output to console during dev
    if not APPLICATION_MODE:
        c_handler = logging.StreamHandler()
        c_handler.setLevel(logging.INFO)
        c_format = logging.Formatter(LOG_FORMAT, datefmt=LOG_DT_FORMAT)
        c_handler.setFormatter(c_format)
        root.addHandler(c_handler)
