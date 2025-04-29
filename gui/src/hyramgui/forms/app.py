"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import copy
import json
import logging

from hyramgui.hygu.models.models import ModelBase
from hyramgui.hygu.forms import forms

from PySide6.QtCore import Slot, QObject, Property, Signal
from PySide6.QtQml import QmlElement

from hyramgui import app_settings
from hyramgui.hygu.utils.helpers import InputStatus, ValidationResponse
from hyramgui.models.enums import FuelMixType
from hyramgui.models.models import State
from hyramgui.models.analyses import do_plume_analysis, do_accum_analysis, do_jet_flame_analysis, do_unconf_overp_analysis
from hyramgui.models.qra_analyses import do_qra_analysis
from hyramgui.forms.results import AnalysisResultsForm

QML_IMPORT_NAME = "hyram.classes"
QML_IMPORT_MAJOR_VERSION = 1

log = logging.getLogger(app_settings.APPNAME)


@QmlElement
class HyramAppForm(forms.AppForm):
    """Top-level manager of GUI form, analysis thread, and analysis requests.

    Notes
    -----
    Qt signals use camelCase for consistency with QML coding style.

    """
    historyChanged = Signal()

    def __init__(self):
        """Initializes backend store and thread controller. """
        super().__init__(model_class=State)
        self.db.history_changed += lambda x: self.historyChanged.emit()

        self._about_str = (
            "Maintainers (alphabetical): "
            "Michael C. Devin, "
            "Brian D. Ehrhart, "
            "Ethan S. Hecht, "
            "Benjamin R. Liu, "
            "Cianan Sims."
            "\n\n"
            "Authors (alphabetical): "
            "Erin E. Carrier, "
            "Michael C. Devin, "
            "Brian D. Ehrhart, "
            "Isaac W. Ekoto, "
            "Katrina M. Groth, "
            "Ethan S. Hecht, "
            "Benjamin R. Liu, "
            "Alice Muna, "
            "John T. Reynolds, "
            "Benjamin B. Schroeder, "
            "Cianan Sims, "
            "Gregory W. Walkup."
            "\n\n"
            "HyRAM+ development was supported by the U.S. Department of Energy (DOE) Office of Energy Efficiency "
            "(EERE), Hydrogen and Fuel Cell Technologies Office (HFTO), the DOE EERE Vehicles Technologies Office (VTO), "
            "and the U.S. Department of Transportation (DOT) Pipeline and Hazardous Materials Safety Administration (PHMSA)"
            "."
        )

        self._copyright_str = ("Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS). "
                               "Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain "
                               "rights in this software.\n\n"
                               "You should have received a copy of the GNU General Public License along with HyRAM+. "
                               "If not, see https://www.gnu.org/licenses/.")

    def check_form_valid(self, *args, **kwargs) -> bool:
        """Checks whether form state is valid. Triggers form alert display and continuous validation when invalid.

        Notes
        -----
        Default calls this on each history change.
        May receive misc. inputs from other events.

        Returns
        -------
        bool
            True if form state is valid.

        """
        valid_resp = self.db.check_valid()  # 3 is error, 2 is warning, 1 is info, 0 is no issues
        super()._toggle_form_alert(valid_resp)
        return valid_resp.status in [InputStatus.WARN, InputStatus.INFO, InputStatus.GOOD]

    @Slot(str)
    def do_log(self, msg):
        """Logs input message. Note: used by QML for logging. """
        self._log(msg)

    @Slot(result="QVariantList")
    def check_valid_shared_state(self):
        """Updates displayed state alert notifications based on stored status (i.e. after notification triggered). """
        resp = self.db.check_valid_shared_state()
        return [int(resp.status), resp.message]

    @Slot(result="QVariantList")
    def check_valid_qra(self):
        """Updates displayed QRA alert notifications based on stored status (i.e. after notification triggered). """
        resp = self.db.check_valid_qra()
        return [int(resp.status), resp.message]

    @Slot(result="QVariantList")
    def check_valid_accum(self):
        """Updates displayed QRA alert notifications based on stored status (i.e. after notification triggered). """
        resp = self.db.check_valid_accum()
        return [int(resp.status), resp.message]

    @Slot(result=str)
    def compute_tpd(self):
        result = self.db.compute_tpd()
        if result['status'] != 1:
            return result['message']
        else:
            return ''

    @Slot(result=str)
    def calc_tank_mass(self):
        result = self.db.compute_tank_mass()
        if result['status'] != 1:
            return result['message']
        else:
            return ''

    @Slot(result=str)
    def calc_mass_flow_blowdown(self):
        result = self.db.compute_mass_flow_blowdown()
        if result['status'] != 1:
            return result['message']
        else:
            return ''

    @Property(str)
    def etk_blowdown_plot(self):
        result = self.db.etk_blowdown_plot
        return result

    @Slot(int)
    def set_fuel_mix(self, mix_index):
        """Sets fuel mix from selected option. """
        mix_str = FuelMixType.keys[mix_index]
        self.db.set_fuel_mix(mix_str)

    @Slot(str)
    def allocate_remaining_conc(self, label):
        """Allocates remaining concentration to fuel specified by index. """
        self.db.allocate_remaining_conc(label)

    # Jet Flame funcs
    @Slot(int, str)
    def set_jet_flame_point_data(self, idx, json_str):
        """Sets data from JSON input. """
        try:
            lst = json.loads(json_str)
        except json.decoder.JSONDecodeError as err:
            resp = ValidationResponse(InputStatus.ERROR, 'Flame point invalid')
            self._toggle_form_alert(self, resp)
            return
        self.db.set_jet_flame_point_data(idx, lst)

    @Slot(int)
    def remove_jet_flame_point(self, index):
        self.db.remove_jet_flame_point(index)

    @Slot()
    def add_jet_flame_point(self):
        self.db.add_jet_flame_point()

    @Property(str, constant=True)
    def jet_flame_point_data(self):
        lst = self.db.jet_flame_points
        return json.dumps(lst)

    # Unconfined Overpressure funcs
    @Slot(int, str)
    def set_uo_point_data(self, idx, json_str):
        """Sets data from JSON input. """
        try:
            lst = json.loads(json_str)
        except json.decoder.JSONDecodeError as err:
            pass
        self.db.set_uo_point_data(idx, lst)

    @Slot(int)
    def remove_uo_point(self, index):
        self.db.remove_uo_point(index)

    @Slot()
    def add_uo_point(self):
        self.db.add_uo_point()

    @Property(str, constant=True)
    def uo_point_data(self):
        lst = self.db.uo_points
        return json.dumps(lst)

    # =====================
    # === QRA FUNCTIONS ===
    @Slot(str)
    def set_occupant_data(self, json_str):
        """Sets occupant data from JSON input. """
        try:
            data = json.loads(json_str)
        except json.decoder.JSONDecodeError as err:
            resp = ValidationResponse(InputStatus.ERROR, 'Occupant group invalid')
            self._toggle_form_alert(self, resp)
            return
        self.db.set_occupant_data(data)

    @Property(str, constant=True)
    def occupant_data(self):
        data = self.db.occupant_data
        return json.dumps(data)

    @Slot(int, str)
    def set_ignition_data(self, idx, json_str):
        """Sets ignition data from JSON input. """
        try:
            data = json.loads(json_str)
        except json.decoder.JSONDecodeError as err:
            resp = ValidationResponse(InputStatus.ERROR, 'Ignition data invalid')
            self._toggle_form_alert(self, resp)
            return
        self.db.set_ignition_data(idx, data)

    @Slot(int)
    def remove_ignition(self, index):
        self.db.remove_ignition(index)

    @Slot(float)
    def add_ignition(self, threshold):
        self.db.add_ignition(threshold)

    @Property(str, constant=True)
    def ignition_data(self):
        data = self.db.ignition_data
        return json.dumps(data)

    @Property(list, constant=True)
    def get_diff_keys(self):
        lst = self.db.get_diff()
        return lst

    @Property(bool, constant=True)
    def is_unchoked(self):
        return self.db.is_unchoked()

    @Slot(str)
    def request_analysis(self, analysis_type: str):
        """Handles analysis request by submitting valid data to thread and updating queue.

        Note: to accommodate additional forms, just pass an id that determines the form type and callbacks.

        """
        status = self.check_form_valid()
        if not status:
            return

        if analysis_type == 'plume':
            analysis = do_plume_analysis
            tp_name = 'Plume'
        elif analysis_type == 'accum':
            analysis = do_accum_analysis
            tp_name = 'Accum.'
        elif analysis_type == 'flame':
            analysis = do_jet_flame_analysis
            tp_name = 'Flame'
        elif analysis_type == 'uo':
            analysis = do_unconf_overp_analysis
            tp_name = 'Overpressure'
        elif analysis_type == 'qra':
            analysis = do_qra_analysis
            tp_name = 'QRA'
        else:
            return

        # must remove listeners on state, otherwise it will attempt (and fail) to deepcopy this object as well
        self._deactivate_validation()
        db_copy = copy.deepcopy(self.db)

        if db_copy.analysis_name.value in ["", None]:
            db_copy.analysis_name.value = f"Analysis {self.thread.get_curr_id() + 1} - {tp_name}"

        analysis_id = self.thread.request_new_analysis(db_copy,
                                                       analysis,
                                                       self.analysis_started_callback,
                                                       self.analysis_finished_callback)

        if analysis_id is not None:
            # Prep for results display with necessary data; won't have full access until analysis is complete.
            ac = AnalysisResultsForm(analysis_id, analysis_type, db_copy)
            ac.request_overwrite_event += self.handle_child_requests_form_overwrite
            self.result_forms[analysis_id] = ac
            # add view to visible queue
            self.queue_controller.handle_new_analysis(ac)

        self._activate_validation()

    def analysis_finished_callback(self, state_obj: type(ModelBase), results: dict):
        """Updates state of returned analysis with finalized data and sends it to its AnalysisController for final processing.
        Called via thread after processing pool finishes executing analysis.

        Parameters
        ----------
        state_obj : State
            state model object containing parameter and result data for specified analysis.

        results : dict
            analysis results including data and plots.

        """
        result_form = self.result_forms[state_obj.analysis_id]

        status = results['status']
        if status == -1:
            self._log('Skipping hydration - analysis encountered error.')
            state_obj.has_error = True
            state_obj.error = results.pop('error', None)
            state_obj.error_message = results.pop('message', '')

        elif status == 2:
            # Analysis canceled by user
            state_obj.has_error = False
            state_obj.was_canceled = True

        else:
            # Success so parse results
            state_obj.has_error = False
            state_obj.set_output_dir(results['output_dir'])

            analysis_type = results['analysis_type']
            if analysis_type == 'plume':
                state_obj.plume_plot = results.get('plot_fpath', None)
                state_obj.plume_out_flow = results.get('mass_flow', None)
                state_obj.plume_contour_dicts = results.get('contour_dicts', [])

            elif analysis_type == 'accum':
                state_obj.acc_pressure_plot = results.get('acc_pressure_plot', None)
                state_obj.acc_flam_plot = results.get('acc_flam_plot', None)
                state_obj.acc_layer_plot = results.get('acc_layer_plot', None)
                state_obj.acc_traj_plot = results.get('acc_traj_plot', None)
                state_obj.acc_flow_plot = results.get('acc_flow_plot', None)
                state_obj.acc_max_overp = results.get('max_overp', None)
                state_obj.acc_overp_t = results.get('overp_t', None)
                state_obj.acc_data_dicts = results.get('data_dicts', [])

            elif analysis_type == 'flame':
                state_obj.jet_flux_plot = results.get('flux_plot_fpath', None)
                state_obj.jet_temp_plot = results.get('temp_plot_fpath', None)
                state_obj.jet_mass_flow = results.get('mass_flow', None)
                state_obj.jet_srad = results.get('srad', None)
                state_obj.jet_visible_len = results.get('visible_len', None)
                state_obj.jet_radiant_frac = results.get('radiant_frac', None)
                state_obj.jet_flux_data = results.get('flux_data', [])

            elif analysis_type == 'uo':
                state_obj.uo_results = results['data']
                # state_obj.uo_impulse_plot = results.get('impulse_plot_fpath', None)
                # state_obj.uo_data = results.get('data', [])
                # state_obj.uo_tnt_mass = results.get('tnt_mass', None)
                # state_obj.uo_mass_flow = results.get('mass_flow', None)
                # state_obj.uo_flam_mass = results.get('flam_mass', None)

            elif analysis_type == 'qra':
                state_obj.qra_results = results['data']

        del results
        result_form.update_from_state_object(state_obj)

    @Slot()
    def load_new_form(self):
        """Clears form and loads deterministic demo as new data. """
        self.db.clear_save_file()
        self.db.load_demo_file_data()
        self.newMessageEvent.emit("Form reset to default values")

    @Slot()
    def load_demo1(self):
        """Loads deterministic analysis data from repo file. """
        self.db.load_demo_file_data('demo1')
        self.newMessageEvent.emit("Demo 1 loaded")

    @Slot()
    def load_demo2(self):
        """Loads Probabilistic analysis data from repo file. """
        self.db.load_demo_file_data('demo2')
        self.newMessageEvent.emit("Demo 2 loaded")
