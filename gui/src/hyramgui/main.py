"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import multiprocessing
import os
import sys

from pathlib import Path

from PySide6.QtGui import QGuiApplication, QIcon
from PySide6.QtQml import QQmlApplicationEngine

import logging
from hyramgui import app_settings
from hyramgui.hygu.models.fields import ChoiceField, IntField, StringField, NumField, NumListField, BoolField
from hyramgui.hygu.forms.fields import StringFormField, IntFormField, BoolFormField, NumFormField, NumListFormField, ChoiceFormField

from hyramgui.models.fields import LognormField, DistributionField
from hyramgui.forms.fields import LognormFormField, DistributionFormField

"""
This file initializes the HyRAM+ GUI and backing data model.

Notes
-----
HyRAM-specific modules are imported below after logging is setup so the logging config can be centralized.

"""


def main():
    app_settings.init()
    log = logging.getLogger(app_settings.APPNAME)
    log.info(f'Initializing {app_settings.APPNAME}...')
    log.info(f"working dir: {app_settings.CWD_DIR}")
    log.info(f"data dir: {app_settings.DATA_DIR}")
    log.info(f"session dir: {app_settings.SESSION_DIR}")
    log.info(f'User set session dir? {app_settings.USER_SET_SESSION_DIR}')

    from hyramgui.hygu.displays import QueueDisplay
    from hyramgui.forms.app import HyramAppForm

    app_form = HyramAppForm()
    queue = QueueDisplay()
    app_form.set_queue(queue)
    app_form.analysisStarted.connect(queue.handle_new_analysis)

    if app_settings.IS_WINDOWS:
        icon_file = 'icon.ico'
        # support high DPI scaling on windows
        os.environ["QT_AUTO_SCREEN_SCALE_FACTOR"] = "1"
        os.environ["QT_ENABLE_HIGHDPI_SCALING"] = "1"
        os.environ["QT_SCALE_FACTOR"] = "1"
    else:
        icon_file = 'icon.icns'

    app = QGuiApplication(sys.argv)
    icon_path = app_settings.CWD_DIR.joinpath('assets/logo/').joinpath(icon_file)
    app.setWindowIcon(QIcon(icon_path.as_posix()))

    engine = QQmlApplicationEngine()
    ctx = engine.rootContext()
    app_form.set_app(app)

    # Create references to fields so they're not GC'd
    session_dir = StringFormField(param=app_form.db.session_dir)

    param_refs = {}
    class_pairs = {ChoiceField: ChoiceFormField,
                   NumField: NumFormField,
                   LognormField: LognormFormField,
                   DistributionField: DistributionFormField,
                   StringField: StringFormField,
                   NumListField: NumListFormField,
                   IntField: IntFormField,
                   BoolField: BoolFormField}
    for attr, value in app_form.db.__dict__.items():
        fc = class_pairs.get(type(value), None)
        if fc is not None:
            form_param = fc(param=value)
            param_refs[attr] = form_param
            qml_name = attr + "_c"
            ctx.setContextProperty(qml_name, form_param)

    ctx.setContextProperty("app_form", app_form)
    ctx.setContextProperty("queue", queue)
    ctx.setContextProperty("session_dir_c", session_dir)

    qml_file = Path(__file__).resolve().parent / "ui/main.qml"
    engine.load(qml_file)

    if not engine.rootObjects():
        sys.exit(-1)

    sys.exit(app.exec())


if __name__ == "__main__":
    multiprocessing.freeze_support()
    main()
