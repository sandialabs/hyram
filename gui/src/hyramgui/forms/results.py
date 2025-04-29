"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import json
import logging

import numpy as np
from PySide6.QtCore import Property, Slot
from PySide6.QtQml import QmlElement

from hyramgui.hygu.forms.results import ResultsForm
from hyramgui.hygu.forms.fields import ChoiceFormField, IntFormField, StringFormField, NumFormField, BoolFormField, NumListFormField
from hyramgui.hygu.models.fields import ChoiceField, IntField, StringField, NumField, NumListField, BoolField

from hyramgui.models.fields import LognormField, DistributionField
from hyramgui.forms.fields import LognormFormField, DistributionFormField

from hyramgui.models.models import State
from hyramgui import app_settings
from hyramgui.utils import round_to_sig_figs

QML_IMPORT_NAME = f"{ app_settings.APPNAME_LOWER}.classes"
QML_IMPORT_MAJOR_VERSION = 1
log = logging.getLogger(app_settings.APPNAME)


class_pairs = {ChoiceField: ChoiceFormField,
               NumField: NumFormField,
               LognormField: LognormFormField,
               DistributionField: DistributionFormField,
               StringField: StringFormField,
               NumListField: NumListFormField,
               IntField: IntFormField,
               BoolField: BoolFormField}


@QmlElement
class AnalysisResultsForm(ResultsForm):
    """Controller class which manages analysis data binding and views during and after execution of single analysis.

    Notes
    -----
    Can't access in-progress analyses in child processes.
    Completed analysis results are returned from thread loop.
    Note that event names correspond to usage location; e.g. camelCase events are used in QML.
    Many attributes are implemented as properties to allow access via QML. This includes the analysis parameter controllers.

    Internal parameter controllers (e.g. _conc_h2) are dynamically created for each field parameter.
    They process input data and display it in results pane for completed analyses.

    Attributes
    ----------
    pipe_diam
    seed

    """
    _analysis_type = ""

    def __init__(self, analysis_id: int, analysis_type: str, prelim_state, *args, **kwargs):
        """Initializes controller with key data for analysis being submitted, including unique id.

        Parameters
        ----------
        analysis_id : int
            Increasing id indicating the analysis managed by this controller.

        """
        self._analysis_type = analysis_type
        super().__init__(analysis_id=analysis_id, prelim_state=prelim_state)

    def update_from_state_object(self, state: State):
        """Updates parameters and state data from state object.

        Parameters
        ----------
        state : State
            State model containing parameter and result data for this completed analysis.

        Notes
        -----
        State object likely returned from analysis processing thread after analysis completed.

        """
        self._state = state

        # create internal attributes and corresponding property of all analysis parameters
        for attr, value in self._state.__dict__.items():
            form_class = class_pairs.get(type(value), None)
            if form_class is not None:
                setattr(self, "_" + attr, form_class(param=value))

        super().finish_state_update()

    # TODO: find way to dynamically create these? Can't do via property or setattr bc not stored in QtMetaObject
    seed = Property(IntFormField, fget=lambda self: self._seed)
    fuel_mix = Property(ChoiceFormField, fget=lambda self: self._fuel_mix)

    nozzle = Property(ChoiceFormField, fget=lambda self: self._nozzle)
    rel_phase = Property(ChoiceFormField, fget=lambda self: self._rel_phase)
    rel_p = Property(NumFormField, fget=lambda self: self._rel_p)
    rel_t = Property(NumFormField, fget=lambda self: self._rel_t)
    amb_p = Property(NumFormField, fget=lambda self: self._amb_p)
    amb_t = Property(NumFormField, fget=lambda self: self._amb_t)
    discharge = Property(NumFormField, fget=lambda self: self._discharge)
    humid = Property(NumFormField, fget=lambda self: self._humidity)

    leak_d = Property(NumFormField, fget=lambda self: self._leak_d)
    rel_angle = Property(NumFormField, fget=lambda self: self._rel_angle)

    # Plume params
    mole_contours = Property(NumFormField, fget=lambda self: self._mole_contours)
    plume_mass_flow = Property(NumFormField, fget=lambda self: self._plume_mass_flow)

    # Accum params
    is_blowdown = Property(BoolFormField, fget=lambda self: self._is_blowdown)
    rel_h = Property(NumFormField, fget=lambda self: self._rel_h)
    enclosure_h = Property(NumFormField, fget=lambda self: self._enclosure_h)
    floor_ceil_a = Property(NumFormField, fget=lambda self: self._floor_ceil_a)
    rel_wall_dist = Property(NumFormField, fget=lambda self: self._rel_wall_dist)
    ceil_xarea = Property(NumFormField, fget=lambda self: self._ceil_xarea)
    ceil_h = Property(NumFormField, fget=lambda self: self._ceil_h)
    floor_xarea = Property(NumFormField, fget=lambda self: self._floor_xarea)
    floor_h = Property(NumFormField, fget=lambda self: self._floor_h)
    tank_v = Property(NumFormField, fget=lambda self: self._tank_v)
    vent_flow = Property(NumFormField, fget=lambda self: self._vent_flow)
    t_max = Property(NumFormField, fget=lambda self: self._t_max)
    do_p_lines = Property(BoolFormField, fget=lambda self: self._do_p_lines)
    do_p_ts = Property(BoolFormField, fget=lambda self: self._do_p_ts)
    out_ts = Property(NumListFormField, fget=lambda self: self._out_ts)
    pair_ts = Property(NumListFormField, fget=lambda self: self._pair_ts)
    pair_ps = Property(NumListFormField, fget=lambda self: self._pair_ps)

    # QRA Params
    override_d01 = Property(NumFormField, fget=lambda self: self._override_d01)
    override_d1 = Property(NumFormField, fget=lambda self: self._override_d1)
    override_1 = Property(NumFormField, fget=lambda self: self._override_1)
    override_10 = Property(NumFormField, fget=lambda self: self._override_10)
    override_100 = Property(NumFormField, fget=lambda self: self._override_100)
    override_fail = Property(NumFormField, fget=lambda self: self._override_fail)

    pipe_l = Property(NumFormField, fget=lambda self: self._pipe_l)
    pipe_d = Property(NumFormField, fget=lambda self: self._pipe_d)
    pipe_od = Property(NumFormField, fget=lambda self: self._pipe_od)
    pipe_thick = Property(NumFormField, fget=lambda self: self._pipe_thick)

    overp_method = Property(ChoiceFormField, fget=lambda self: self._overp_method)
    overp_probit = Property(ChoiceFormField, fget=lambda self: self._overp_probit)
    tnt_factor = Property(NumFormField, fget=lambda self: self._tnt_factor)
    mach_speed = Property(ChoiceFormField, fget=lambda self: self._mach_speed)
    thermal_probit = Property(ChoiceFormField, fget=lambda self: self._thermal_probit)
    exposure_t = Property(NumFormField, fget=lambda self: self._exposure_time)

    detection = Property(NumFormField, fget=lambda self: self._detection)
    exclusion = Property(NumFormField, fget=lambda self: self._exclusion)
    mass_flow_leak = Property(ChoiceFormField, fget=lambda self: self._mass_flow_leak)
    mass_flow = Property(NumFormField, fget=lambda self: self._mass_flow)

    n_vehicles = Property(IntFormField, fget=lambda self: self._n_vehicles)
    n_fuelings = Property(NumFormField, fget=lambda self: self._n_fuelings)
    n_vehicle_days = Property(NumFormField, fget=lambda self: self._n_vehicle_days)

    n_compressors = Property(IntFormField, fget=lambda self: self._n_compressors)
    n_vessels = Property(IntFormField, fget=lambda self: self._n_vessels)
    n_valves = Property(IntFormField, fget=lambda self: self._n_valves)
    n_instruments = Property(IntFormField, fget=lambda self: self._n_instruments)
    n_joints = Property(IntFormField, fget=lambda self: self._n_joints)
    n_hoses = Property(IntFormField, fget=lambda self: self._n_hoses)
    n_filters = Property(IntFormField, fget=lambda self: self._n_filters)
    n_flanges = Property(IntFormField, fget=lambda self: self._n_flanges)
    n_exchangers = Property(IntFormField, fget=lambda self: self._n_exchangers)
    n_vaporizers = Property(IntFormField, fget=lambda self: self._n_vaporizers)
    n_arms = Property(IntFormField, fget=lambda self: self._n_arms)
    n_extra1 = Property(IntFormField, fget=lambda self: self._n_extra1)
    n_extra2 = Property(IntFormField, fget=lambda self: self._n_extra2)

    @Property(bool, constant=True)
    def is_unchoked(self):
        return self.state is not None and self.state.is_unchoked()

    # =================
    # ==== RESULTS ====
    @Property(str, constant=True)
    def analysis_type(self):
        """String indicator for type of analysis. """
        return self._analysis_type

    # def _get_state_property(self, prop, default_val=""):
    #     result = getattr(self.state, prop) if self.state is not None and hasattr(self.state, prop) else default_val
    #     return result

    @Property(str, constant=True)
    def plume_plot(self):
        """String filepath of pipe lifetime plot. """
        result = self.state.plume_plot if self.state is not None and self.state.plume_plot else ""
        return result

    @Property(str, constant=True)
    def plume_out_flow(self):
        """Plume mass flow rate output (kg/s). """
        result = self.state.plume_out_flow if self.state is not None and self.state.plume_out_flow else ""
        if result is not None:
            result = f"{result:.3e}"
        return result

    @Property(str, constant=True)
    def plume_contour_dicts(self):
        """Plume contour data. """
        lst = self.state.plume_contour_dicts if self.state is not None and self.state.plume_contour_dicts else []
        lst_str = json.dumps(lst)
        return lst_str

    # ACCUMULATION
    @Property(str, constant=True)
    def acc_pressure_plot(self):
        """String filepath to accumulation result plot. """
        result = self.state.acc_pressure_plot if self.state is not None and self.state.acc_pressure_plot else ""
        return result

    @Property(str, constant=True)
    def acc_flam_plot(self):
        """String filepath to accumulation result plot. """
        result = self.state.acc_flam_plot if self.state is not None and self.state.acc_flam_plot else ""
        return result

    @Property(str, constant=True)
    def acc_layer_plot(self):
        """String filepath to accumulation result plot. """
        result = self.state.acc_layer_plot if self.state is not None and self.state.acc_layer_plot else ""
        return result

    @Property(str, constant=True)
    def acc_traj_plot(self):
        """String filepath to accumulation result plot. """
        result = self.state.acc_traj_plot if self.state is not None and self.state.acc_traj_plot else ""
        return result

    @Property(str, constant=True)
    def acc_flow_plot(self):
        """String filepath to accumulation result plot. """
        result = self.state.acc_flow_plot if self.state is not None and self.state.acc_flow_plot else ""
        return result

    @Property(str, constant=True)
    def acc_data_dicts(self):
        """Accumulation data. """
        lst = self.state.acc_data_dicts if self.state is not None and self.state.acc_data_dicts else []
        for elem in lst:
            try:
                if 'conc' in elem and np.isnan(elem['conc']):
                    elem['conc'] = "NaN"
                if 'depth' in elem and np.isnan(elem['depth']):
                    elem['depth'] = "NaN"
            except Exception:
                pass
        lst_str = json.dumps(lst)
        return lst_str

    @Property(str, constant=True)
    def acc_max_overp(self):
        """Accumulation maximum overpressure (kPa). """
        result = self.state.acc_max_overp if self.state is not None and self.state.acc_max_overp else ""
        if result is not None:
            result = f"{result:.2f}"
        return result

    @Property(str, constant=True)
    def acc_overp_t(self):
        """Accumulation time at which overpressure occurred (s). """
        result = self.state.acc_overp_t if self.state is not None and self.state.acc_overp_t else ""
        if result is not None:
            result = f"{result:.2f}"
        return result

    @Property(str, constant=True)
    def jet_flux_plot(self):
        """String filepath to jet flame heat flux plot. """
        return self.state.jet_flux_plot if self.state is not None and self.state.jet_flux_plot else ""

    @Property(str, constant=True)
    def jet_temp_plot(self):
        """String filepath to jet flame heat flux plot. """
        return self.state.jet_temp_plot if self.state is not None and self.state.jet_temp_plot else ""

    @Property(str, constant=True)
    def jet_mass_flow(self):
        """Jet flame mass flow rate (kg/s). """
        return f"{self.state.jet_mass_flow:.3e}" if self.state is not None and self.state.jet_mass_flow else ""

    @Property(str, constant=True)
    def jet_srad(self):
        """Jet flame total emitted radiative power (W). """
        return f"{self.state.jet_srad:.3e}" if self.state is not None and self.state.jet_srad else ""

    @Property(str, constant=True)
    def jet_visible_len(self):
        """Jet flame visible flame length (m). """
        return f"{self.state.jet_visible_len:.3f}" if self.state is not None and self.state.jet_visible_len else ""

    @Property(str, constant=True)
    def jet_radiant_frac(self):
        """Jet flame radiant fraction. """
        return f"{self.state.jet_radiant_frac:.3f}" if self.state is not None and self.state.jet_radiant_frac else ""

    @Property(str)
    def jet_flux_data(self):
        """Jet flame flux data dicts. """
        arr = self.state.jet_flux_data if self.state is not None and self.state.jet_flux_data else []
        for elem in arr:
            elem['flux'] = f"{float(elem['flux']):.4f}"
        result = json.dumps(arr)
        return result

    # === UNCONF OVERP RESULTS
    def _uo_result(self):
        return self.state.uo_results if self.state is not None and self.state.uo_results else {}

    @Property(str, constant=True)
    def uo_overp_plot(self):
        """String filepath to jet flame heat flux plot. """
        res = self._uo_result()
        return res.get('overp_plot_filepath', '')

    @Property(str, constant=True)
    def uo_impulse_plot(self):
        """String filepath to jet flame heat flux plot. """
        res = self._uo_result()
        return res.get('impulse_plot_filepath', '')

    @Property(str, constant=True)
    def uo_mass_flow(self):
        """Mass flow rate (kg/s). """
        res = self._uo_result()
        val = res.get('mass_flow_rate', None)
        return f"{val:.3e}" if val is not None else ""

    @Property(str, constant=True)
    def uo_flam_mass(self):
        """Flammable mass. """
        res = self._uo_result()
        val = res.get('flam_or_det_mass', None)
        return f"{val:.3e}" if val is not None else ""

    @Property(str, constant=True)
    def uo_tnt_mass(self):
        """Flammable mass. """
        res = self._uo_result()
        val = res.get('tnt_mass', None)
        return f"{val:.3e}" if val is not None else ""

    @Property(str, constant=True)
    def uo_pos_data(self):
        """Unconf overp position data. """
        res = self._uo_result()
        impulses = res.get('impulses', [])
        overps = res.get('overpressures', [])
        pts = self.state.uo_points
        data = []
        for i in range(len(pts)):
            pt = pts[i]
            dc = dict(x=pt[0], y=pt[1], z=pt[2], impulse=f"{impulses[i]:.3e}", overp=f"{overps[i]:.3e}")
            data.append(dc)
        return json.dumps(data)

    # ===================
    # === QRA RESULTS ===
    def _qra_result(self):
        return self.state.qra_results if self.state is not None and self.state.qra_results else {}

    @Property(str, constant=True)
    def qra_air(self):
        """QRA avg individual risk. """
        res = self._qra_result()
        return f"{res['air']:.3e}" if 'air' in res else ''

    @Property(str, constant=True)
    def qra_pll(self):
        """QRA PLL value. """
        res = self._qra_result()
        return f"{res['total_pll']:.3e}" if 'total_pll' in res else ''

    @Property(str, constant=True)
    def qra_far(self):
        """QRA FAR value. """
        res = self._qra_result()
        return f"{res['far']:.3e}" if 'far' in res else ''

    @Property(str)
    def qra_scenario_ranking_data(self):
        """Retrieve and format QRA scenario ranking data."""
        res = self._qra_result()
        if 'leak_results' not in res:
            return ''

        data = []
        for res in res['leak_results']:
            leakname = f"{res['leak_size']/100:.2%} release"
            for evt in res['result_dicts']:
                if evt['key'] == 'tot':
                    continue

                pll = evt['pll']
                if pll > 0.01:
                    pll = f"{evt['pll']:.0%}"
                elif pll > 1e-4:
                    pll = f"{evt['pll']:.2%}"
                elif pll > 1e-6:
                    pll = f"{evt['pll']:.4%}"
                else:
                    pll = "0.00%"

                n_events = evt['events']
                if n_events > 1:
                    n_events = f"{n_events:.0f}"
                elif n_events > 0.01:
                    n_events = f"{n_events:.2f}"
                else:
                    n_events = f"{n_events:.2e}"

                dc = {
                    'scenario': leakname,
                    'outcome': evt['label'],
                    'events': n_events,
                    'pll': pll,
                }
                data.append(dc)
        result = json.dumps(data)
        return result

    @Property(str)
    def qra_scenario_details_data(self):
        """Retrieve and format QRA scenario details data."""
        res = self._qra_result()
        if 'leak_results' not in res:
            return ''

        data = []
        for res in res['leak_results']:
            leakname = f"{res['leak_size']/100:.2%} release"
            dc = {
                'leak': leakname,
                'flow': f"{res['discharge_rates']:.3e}",
                'leak_d': f"{res['leak_diam']:.3e}",
                'f_release': f"{res['frequency']:.3e}",
            }
            data.append(dc)

        result = json.dumps(data)
        return result

    @Property(str)
    def qra_scenario_outcome_data(self):
        """Retrieve and format QRA scenario outcome data."""
        res = self._qra_result()
        if 'leak_results' not in res:
            return ''

        events = ['Shutdown', 'No ignition', 'Jetfire', 'Explosion']
        leak_keys = ['r0d01', 'r0d1', 'r1', 'r10', 'r100']
        data = []
        # array of data per leak size but we need per event
        for i, evt in enumerate(events):
            dc = dict(outcome=evt, r0d01='', r0d1='', r1='', r10='', r100='')

            for j, leak in enumerate(res['leak_results']):
                result_dict = leak['result_dicts'][i]
                p_evt = result_dict['prob']
                dc[leak_keys[j]] = f'{p_evt:.3%}'
            data.append(dc)

        result = json.dumps(data)
        return result

    @Property(str)
    def qra_cut_set_data(self):
        """Retrieve and format QRA cut set data."""
        res = self._qra_result()
        if 'leak_results' not in res:
            return ''

        def rdnum(num):
            sig_figs = 6
            return f"{round_to_sig_figs(num, sig_figs):.3e}"

        def make_leak100_dict(label, val):
            return dict(cutSet=label, r0d01='', r0d1='', r1='', r10='', r100=rdnum(val))

        data = []

        overrides = [
            self.state.override_d01.is_null,
            self.state.override_d1.is_null,
            self.state.override_1.is_null,
            self.state.override_10.is_null,
            self.state.override_100.is_null,
        ]
        override_row = {
            'cutSet': 'Fluid release (override)',
            'r0d01': '-' if overrides[0] else res['leak_results'][0]['frequency'],
            'r0d1': '-' if overrides[1] else res['leak_results'][1]['frequency'],
            'r1': '-' if overrides[2] else res['leak_results'][2]['frequency'],
            'r10': '-' if overrides[3] else res['leak_results'][3]['frequency'],
            'r100': '-' if overrides[4] else res['leak_results'][4]['frequency'],
        }
        data.append(override_row)
        data.append(dict(cutSet='', r0d01='', r0d1='', r1='', r10='', r100=''))  # empty spacer row

        comps = ['compressor', 'vessel', 'valve', 'instrument', 'joint', 'hose', 'pipe', 'filter', 'flange',
                 'exchanger', 'vaporizer', 'arm', 'extra1', 'extra2',]
        comp_labels = ['Compressor', 'Vessel', 'Valve', 'Instrument', 'Joint', 'Hose', 'Pipe', 'Filter', 'Flange',
                       'Heat exchanger', 'Vaporizer', 'Loading arm', 'Extra component #1', 'Extra component #2']
        leak_keys = ['r0d01', 'r0d1', 'r1', 'r10', 'r100']

        for i, comp in enumerate(comps):
            name = f"{comp_labels[i]} leak"
            component_row = dict(cutSet=name, r0d01='', r0d1='', r1='', r10='', r100='')
            for j, leak in enumerate(res['leak_results']):
                if overrides[j] and comp in leak['component_leaks']:
                    leak_f = rdnum(leak['component_leaks'][comp])
                else:
                    # blank if override provided or comp is 0
                    leak_f = ''
                component_row[leak_keys[j]] = leak_f
            data.append(component_row)

        data.append(dict(cutSet='', r0d01='', r0d1='', r1='', r10='', r100=''))  # empty spacer row

        if overrides[4]:  # skip all these if 100% leak override provided
            leak100 = res['leak_results'][-1]
            disp_failures = leak100['dispenser_failures']
            use_fail_override = not self.state.override_fail.is_null
            if use_fail_override:
                dc = make_leak100_dict('100% fluid release from accidents and shutdown failures (set by user)',
                                       disp_failures['freq_failure'])
                data.append(dc)
            else:
                data.append(make_leak100_dict('100% fluid release from accidents and shutdown failures',disp_failures['freq_failure']))
                data.append(make_leak100_dict('Overpressure during fueling induces rupture', disp_failures['freq_overp_rupture']))
                data.append(make_leak100_dict('Release due to drive-offs', disp_failures['freq_driveoff']))
                data.append(make_leak100_dict('Nozzle release', disp_failures['freq_nozzle_release']))
                data.append(make_leak100_dict('Manual valve fails to close', disp_failures['freq_mvalve_ftc']))
                data.append(make_leak100_dict('Solenoid valves fail to close', disp_failures['freq_svalves_ftc']))

        result = json.dumps(data)
        return result

    @Property(str)
    def qra_thermal_plots(self):
        """Retrieve QRA thermal plot filepaths."""
        res = self._qra_result()
        result = res.get('qrad_plot_files', [])
        return json.dumps(result)

    @Property(str)
    def qra_overp_plots(self):
        """Retrieve QRA overpressure plot filepaths."""
        res = self._qra_result()
        result = res.get('overp_plot_files', [])
        return json.dumps(result)

    @Property(str)
    def qra_impulse_plots(self):
        """Retrieve QRA impulse plot filepaths."""
        res = self._qra_result()
        result = res.get('impulse_plot_files', [])
        return json.dumps(result)

    @Property(str)
    def qra_thermal_data(self):
        """Retrieve and format QRA thermal data."""
        res = self._qra_result()
        if 'positions' not in res:
            return ''

        data = []
        locs = res['positions'].T
        qrads = res['position_qrads']
        for i in range(len(locs)):
            dataset = qrads[i]
            loc = locs[i]
            dc = {
                'idx': int(i+1),
                'x': f"{loc[0]:.1f}",
                'y': f"{loc[1]:.1f}",
                'z': f"{loc[2]:.1f}",
                'r0d01': f"{dataset[0]:.3e}",
                'r0d1': f"{dataset[1]:.3e}",
                'r1': f"{dataset[2]:.3e}",
                'r10': f"{dataset[3]:.3e}",
                'r100': f"{dataset[4]:.3e}",
            }
            data.append(dc)
        result = json.dumps(data)
        return result

    @Property(str)
    def qra_overp_data(self):
        """Retrieve and format QRA overpressure data."""
        res = self._qra_result()
        if 'positions' not in res:
            return ''

        data = []
        locs = res['positions'].T
        overps = res['position_overps']
        for i in range(len(locs)):
            dataset = overps[i]
            loc = locs[i]
            dc = {
                'idx': int(i+1),
                'x': f"{loc[0]:.1f}",
                'y': f"{loc[1]:.1f}",
                'z': f"{loc[2]:.1f}",
                'r0d01': f"{dataset[0]:.3e}",
                'r0d1': f"{dataset[1]:.3e}",
                'r1': f"{dataset[2]:.3e}",
                'r10': f"{dataset[3]:.3e}",
                'r100': f"{dataset[4]:.3e}",
            }
            data.append(dc)
        result = json.dumps(data)
        return result

    @Property(str)
    def qra_impulse_data(self):
        """Retrieve and format QRA impulse data."""
        res = self._qra_result()
        if 'positions' not in res:
            return ''

        data = []
        locs = res['positions'].T
        impulses = res['position_impulses']
        for i in range(len(locs)):
            dataset = impulses[i]
            loc = locs[i]
            dc = {
                'idx': int(i+1),
                'x': f"{loc[0]:.1f}",
                'y': f"{loc[1]:.1f}",
                'z': f"{loc[2]:.1f}",
                'r0d01': f"{dataset[0]:.3e}",
                'r0d1': f"{dataset[1]:.3e}",
                'r1': f"{dataset[2]:.3e}",
                'r10': f"{dataset[3]:.3e}",
                'r100': f"{dataset[4]:.3e}",
            }
            data.append(dc)
        result = json.dumps(data)
        return result

    @Property(str, constant=True)
    def occupant_data(self):
        """Retrieve QRA occupant data sets as JSON."""
        data = self.state.occupant_data if self.state is not None else ''
        return json.dumps(data)

    @Property(str, constant=True)
    def ignition_data(self):
        """Retrieve QRA ignition data sets as JSON."""
        data = self.state.ignition_data if self.state is not None else ''
        return json.dumps(data)

