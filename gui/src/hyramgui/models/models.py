"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import logging
import warnings
from copy import deepcopy
import numpy as np

from hyram.qra.defaults import get_leak_frequency_set_for_species
from hyramgui.hygu.models.fields import ChoiceField, BoolField, IntField, StringField, NumField, NumListField, FieldBase

from hyramgui import app_settings
from hyramgui.hygu.models.models import ModelBase
from hyramgui.hygu.utils.helpers import hround, InputStatus, ValidationResponse
from hyramgui.hygu.utils.units_of_measurement import (Distance, SmallDistance, Temperature, Fractional, MassFlow, Angle,
                                                 UNIT_TYPES_DICT, UNIT_TYPES_LIST)
from hyram.phys import api as phys
from hyram.utilities.custom_warnings import PhysicsWarning

from .enums import (FuelMixType, NozzleType, PhaseType, LeakSizeType, OverpressureType, MachSpeed,
                    ThermalProbitModel, OverpressureProbitModel, FailureDistribution)
from .fields import LognormField, DistributionField
from .units import Pressure, Time, Area, Volume, Density, Mass
from .species import species_dict, hydrogen, methane, propane, nitrogen, co2, ethane, nbutane, isobutane, npentane, isopentane, nhexane, blend

log = logging.getLogger(app_settings.APPNAME)

# Add custom unit options before defaults
global UNIT_TYPES_DICT
global UNIT_TYPES_LIST
for elem in [Pressure, Time, Area, Volume, Density, Mass]:
    UNIT_TYPES_DICT[elem.label] = elem
    UNIT_TYPES_LIST.insert(0, elem)


class State(ModelBase):
    """Representation of analysis parameter data.

    Attributes
    ----------
    pipe_diam : IntField
        Pipe outer diameter.
    seed : IntField
        Random seed.

    Notes
    -----
    Same state object can back multiple analysis form types.

    """
    pipe_diam: IntField
    seed: IntField

    # intermediate calculations
    _intermed_in_progress: bool = False

    # signal controller for conc_change update -> leak freq updates on field change
    _sync_conc_and_freqs: bool = True

    etk_blowdown_plot: str = ""

    # plume results
    plume_plot: str
    plume_out_flow: float
    plume_contour_dicts: list

    # jet flame data/results
    jet_flame_points: list
    jet_flux_plot: str
    jet_temp_plot: str
    jet_mass_flow: float = None
    jet_srad: float = None
    jet_visible_len: float = None
    jet_radiant_frac: float = None
    jet_flux_data: [dict] = []

    # UO data/results
    uo_points: list
    uo_results: dict

    # Accumulation results
    acc_pressure_plot: str = ""
    acc_flam_plot: str = ""
    acc_layer_plot: str = ""
    acc_traj_plot: str = ""
    acc_flow_plot: str = ""
    acc_data_dicts: list = []
    acc_max_overp: float = -1
    acc_overp_t: float = -1

    occupant_data: list
    ignition_data: list

    # QRA results
    qra_results: dict = {}

    # _is_silent = False  # whether to trigger change events

    def __init__(self):
        """Initializes parameter values and history tracking. """
        super().__init__()

        rint = np.random.randint(10**(7-1), 10**7-1 + 1)
        self.seed = IntField('Random seed', slug='seed', value=rint)

        # Neither concentrations nor fuel_mix take precedence. e.g. user might select fuel mix directly, or might adjust conc in table.
        # Their values should correspond to one another and only be set by shared_state interaction or save/load.
        self.fuel_mix = ChoiceField('Fuel', choices=FuelMixType, value=FuelMixType.h2)
        self.conc_h2 = NumField(hydrogen.name, slug=hydrogen.key, unit=Fractional.p, value=100, min_value=0, max_value=100)
        self.conc_ch4 = NumField(methane.name, slug=methane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_pro = NumField(propane.name, slug=propane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_n2 = NumField(nitrogen.name, slug=nitrogen.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_co2 = NumField(co2.name,slug=co2.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_eth = NumField(ethane.name, slug=ethane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_nbu = NumField(nbutane.name, slug=nbutane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_isb = NumField(isobutane.name, slug=isobutane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_npe = NumField(npentane.name, slug=npentane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_isp = NumField(isopentane.name, slug=isopentane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)
        self.conc_nhx = NumField(nhexane.name, slug=nhexane.key, unit=Fractional.p, value=0, min_value=0, max_value=100)

        self.fuels = [self.conc_h2, self.conc_ch4, self.conc_pro, self.conc_n2, self.conc_co2, self.conc_eth,
                      self.conc_nbu, self.conc_isb, self.conc_npe, self.conc_isp, self.conc_nhx]

        self.nozzle = ChoiceField('Notional nozzle model', slug='nozzle', choices=NozzleType, value=NozzleType.yuc)
        self.rel_phase = ChoiceField('Fluid phase', choices=PhaseType, value=PhaseType.fluid)
        # TODO: change var to p_rel, t_amb, etc. for consistency
        self.rel_p_abs = BoolField('Fluid pressure is absolute', value=True)
        self.rel_t = NumField('Tank fluid temperature', slug='rel_t', unit=Temperature.k, value=288, min_value=-np.inf)
        self.rel_p = NumField('Tank fluid pressure', slug='rel_p', unit=Pressure.mpa, value=35)

        self.amb_p = NumField('Ambient pressure', slug='amb_p', unit=Pressure.mpa, value=0.101325)
        self.amb_t = NumField('Ambient temperature', slug='amb_t', unit=Temperature.k, value=288, min_value=-np.inf)
        self.discharge = NumField('Discharge coefficient', slug='discharge', value=1, min_value=0, max_value=1)

        self.leak_d = NumField('Leak diameter', slug='leak_d', unit=SmallDistance.mm, value=3.56)
        self.rel_angle = NumField('Release angle', slug='rel_angle', unit=Angle.deg, value=0)

        self.plume_plot_title = StringField('Plot title', slug='plume_plot_title', value="Mole Fraction of Leak")
        self.mole_contours = NumListField('Mole fraction contours', slug='mole_contours', value=[0.04])
        self.plume_mass_flow = NumField('Fluid mass flow rate (unchoked)', slug='plume_mass_flow', unit=MassFlow.kgs,
                                        value=None)
        self.plume_auto_limits = BoolField('Automatically set plot limits', slug='plume_auto_limits', value=True)
        self.plume_xmin = NumField('Minimum X value', slug='plume_xmin', unit=Distance.m, value=-2.5, min_value=-np.inf)
        self.plume_xmax = NumField('Maximum X value', slug='plume_xmax', unit=Distance.m, value=2.5, min_value=-np.inf)
        self.plume_ymin = NumField('Minimum Y value', slug='plume_ymin', unit=Distance.m, value=0, min_value=-np.inf)
        self.plume_ymax = NumField('Maximum Y value', slug='plume_ymax', unit=Distance.m, value=10, min_value=-np.inf)
        self.plume_mole_min = NumField('Mole fraction minimum', slug='plume_mole_min', value=0, unit=Fractional.fr)
        self.plume_mole_max = NumField('Mole fraction maximum', slug='plume_mole_max', value=0.1, unit=Fractional.fr)

        # Accumulation
        self.is_blowdown = BoolField('Release type', slug='is_blowdown', value=True)
        self.rel_h = NumField('Release height', slug='rel_h', unit=Distance.m, value=0)
        self.enclosure_h = NumField('Enclosure height', slug='enclosure_h', unit=Distance.m, value=2.72)
        self.floor_ceil_a = NumField('Floor/ceiling area', slug='floor_ceil_a', unit=Area.m2, value=16.72216)
        self.rel_wall_dist = NumField('Distance from release to wall', slug='rel_wall_dist', unit=Distance.m, value=2.1255)
        self.ceil_xarea = NumField('Vent 1 (ceiling ) cross-sectional area', slug='ceil_xarea', unit=Area.m2, value=0.090792)
        self.ceil_h = NumField('Vent 1 (ceiling) height from floor', slug='ceil_h', unit=Distance.m, value=2.42)
        self.floor_xarea = NumField('Vent 2 (floor) cross-sectional area', slug='floor_xarea', unit=Area.m2, value=0.00762)
        self.floor_h = NumField('Vent 2 (floor) height from floor', slug='floor_h', unit=Distance.m, value=0.044)
        self.tank_v = NumField('Tank volume', slug='tank_v', unit=Volume.m3, value=0.00363)
        self.vent_flow = NumField('Vent volumetric flow rate', slug='vent_flow', value=0)
        self.out_ts = NumListField('Output pressures at these times', slug='out_ts', unit=Time.sec,
                                   value=[1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26,
                                          27, 28, 29, 29.5])
        self.t_max = NumField('Maximum time', slug='t_max', unit=Time.sec, value=30.0)
        self.do_p_lines = BoolField('Draw horizontal lines at specified pressures', slug='do_p_lines', value=False)
        self.line_ps = NumListField('Pressures to mark', slug='line_ps', unit=Pressure.kpa, value=[13.8, 15, 55.2])

        self.do_p_ts = BoolField('Mark specified pressure/time intersections', slug='do_p_ts', value=False)
        self.pair_ts = NumListField('Times', slug='pair_ts', unit=Time.sec, value=[1, 15, 20])
        self.pair_ps = NumListField('Pressures', slug='pair_ps', unit=Pressure.kpa, value=[13.8, 15, 55.2])

        # === FLAME Inputs ===
        self.flame_contours = NumListField('Contour levels (kW/m<sup>2</sup>)', slug='flame_contours', value=[1.577, 4.732, 25.237])
        self.flame_auto_limits = BoolField('Automatically set plot limits', slug='flame_auto_limits', value=True)
        self.heat_xmin = NumField('Heat flux X min', slug='heat_xmin', unit=Distance.m, value=-5, min_value=-np.inf)
        self.heat_xmax = NumField('Heat flux X max', slug='heat_xmax', unit=Distance.m, value=15, min_value=-np.inf)
        self.heat_ymin = NumField('Heat flux Y min', slug='heat_ymin', unit=Distance.m, value=-10, min_value=-np.inf)
        self.heat_ymax = NumField('Heat flux Y max', slug='heat_ymax', unit=Distance.m, value=10, min_value=-np.inf)
        self.heat_zmin = NumField('Heat flux Z min', slug='heat_zmin', unit=Distance.m, value=-10, min_value=-np.inf)
        self.heat_zmax = NumField('Heat flux Z max', slug='heat_zmax', unit=Distance.m, value=10, min_value=-np.inf)
        self.temp_xmin = NumField('Temperature X min', slug='temp_xmin', unit=Distance.m, value=-5, min_value=-np.inf)
        self.temp_xmax = NumField('Temperature X max', slug='temp_xmax', unit=Distance.m, value=15, min_value=-np.inf)
        self.temp_ymin = NumField('Temperature Y min', slug='temp_ymin', unit=Distance.m, value=-10, min_value=-np.inf)
        self.temp_ymax = NumField('Temperature Y max', slug='temp_ymax', unit=Distance.m, value=10, min_value=-np.inf)

        self.jet_flame_points = [
            (0.01, 1, 0.01),
            (0.5, 1, 0.5),
            (1, 1, 0.5),
            (2, 2, 1),
            (2.5, 1, 1),
            (5, 2, 1),
            (10, 2, 0.5),
            (15, 2, 0.5),
            (25, 2, 1),
            (40, 2, 2),
        ]

        # === Unconfined Overpressure Inputs ===
        self.uo_overp_contours = NumListField('Overpressure contours', slug='uo_overp_contours', value=[7, 13.7, 20.7])
        self.uo_impulse_contours = NumListField('Impulse contours', slug='uo_impulse_contours', value=[0.13, 0.18, 0.27])
        self.uo_auto_limits = BoolField('Automatically set plot limits', slug='uo_auto_limits', value=True)
        self.uo_overp_xmin = NumField('Overpressure X min', slug='uo_overp_xmin', unit=Distance.m, value=-35, min_value=-np.inf)
        self.uo_overp_xmax = NumField('Overpressure X max', slug='uo_overp_xmax', unit=Distance.m, value=35, min_value=-np.inf)
        self.uo_overp_ymin = NumField('Overpressure Y min', slug='uo_overp_ymin', unit=Distance.m, value=0, min_value=-np.inf)
        self.uo_overp_ymax = NumField('Overpressure Y max', slug='uo_overp_ymax', unit=Distance.m, value=35, min_value=-np.inf)
        self.uo_overp_zmin = NumField('Overpressure Z min', slug='uo_overp_zmin', unit=Distance.m, value=-35, min_value=-np.inf)
        self.uo_overp_zmax = NumField('Overpressure Z max', slug='uo_overp_zmax', unit=Distance.m, value=35, min_value=-np.inf)
        self.uo_impulse_xmin = NumField('Impulse X min', slug='uo_impulse_xmin', unit=Distance.m, value=-2, min_value=-np.inf)
        self.uo_impulse_xmax = NumField('Impulse X max', slug='uo_impulse_xmax', unit=Distance.m, value=4.5, min_value=-np.inf)
        self.uo_impulse_ymin = NumField('Impulse Y min', slug='uo_impulse_ymin', unit=Distance.m, value=0, min_value=-np.inf)
        self.uo_impulse_ymax = NumField('Impulse Y max', slug='uo_impulse_ymax', unit=Distance.m, value=3, min_value=-np.inf)
        self.uo_impulse_zmin = NumField('Impulse Z min', slug='uo_impulse_zmin', unit=Distance.m, value=-3, min_value=-np.inf)
        self.uo_impulse_zmax = NumField('Impulse Z max', slug='uo_impulse_zmax', unit=Distance.m, value=3, min_value=-np.inf)

        self.uo_points = [
            (1, 0, 1),
            (2, 0, 2),
        ]

        # === ETK Inputs ===
        self.etk_t = NumField('Temperature', slug='etk_t', unit=Temperature.k, value=298)
        self.etk_p = NumField('Pressure (absolute)', slug='etk_p', unit=Pressure.pa, value=110_000)
        self.etk_d = NumField('Density', slug='etk_d', unit=Density.kgpm3, value=None)
        self.etk_v = NumField('Volume', slug='etk_v', unit=Volume.l, value=None)
        self.etk_m = NumField('Mass', slug='etk_m', unit=Mass.kg, value=None)
        self.etk_p_amb = NumField('Ambient pressure', slug='etk_p_amb', unit=Pressure.pa, value=101_325)
        self.etk_orif_d = NumField('Orifice diameter', slug='etk_orif_d', unit=Distance.mm, value=10)
        self.etk_discharge = NumField('Discharge coefficient', slug='etk_discharge', value=1)
        self.etk_is_blowdown = BoolField('Release type', slug='etk_is_blowdown', value=True)
        self.etk_flow = NumField('Mass flow rate (kg/s)', slug='etk_flow', unit=MassFlow.kgs, value=None)  # returned if flow calc is steady
        self.etk_t_empty = NumField('Time to empty (s)', slug='etk_t_empty', unit=Time.sec, value=None)  # if flow calc is blowdown
        self.etk_blowdown_plot = ""


        # === QRA parameters ===
        self.n_compressors = IntField('# Compressors', slug='n_compressors', value=1)
        self.n_vessels = IntField('# Vessels', slug='n_vessels', value=2)
        self.n_valves = IntField('# Valves', slug='n_valves', value=7)
        self.n_instruments = IntField('# Instruments', slug='n_instruments', value=5)
        self.n_joints = IntField('# Joints', slug='n_joints', value=43)
        self.n_hoses = IntField('# Hoses', slug='n_hoses', value=1)
        self.n_filters = IntField('# Filters', slug='n_filters', value=3)
        self.n_flanges = IntField('# Flanges', slug='n_flanges', value=0)
        self.n_exchangers = IntField('# Heat exchangers', slug='n_exchangers', value=0)
        self.n_vaporizers = IntField('# Vaporizers', slug='n_vaporizers', value=0)
        self.n_arms = IntField('# Loading arms', slug='n_arms', value=0)
        self.n_extra1 = IntField('# Extra component 1', slug='n_extra1', value=0)
        self.n_extra2 = IntField('# Extra component 2', slug='n_extra2', value=0)

        self.pipe_l = NumField('Pipes (length)', slug='pipe_l', unit=Distance.m, value=30)
        self.pipe_d = NumField('Pipe inner diameter', slug='pipe_d', unit=SmallDistance.inch, value=0.245)
        self.pipe_od = NumField('Pipe outer diameter', slug='pipe_od', unit=SmallDistance.inch, value=None)   # value=0.375)
        self.pipe_thick = NumField('Pipe wall thickness', slug='pipe_thick', unit=SmallDistance.inch, value=None)  # value=0.065)
        self.pipe_d.changed += self.refresh_pipe_diams

        self.humidity = NumField('Relative humidity', slug='humid', value=0.89, min_value=0, max_value=1.0)

        self.n_vehicles = IntField('Number of vehicles', slug='n_vehicles', value=20)
        self.n_fuelings = NumField('Number of fuelings / vehicle / day', slug='n_fuelings', value=2)
        self.n_vehicle_days = NumField('Number of vehicle operating days', slug='n_vehicle_days', value=250)

        self.mass_flow_leak = ChoiceField('Leak size', slug='mass_flow_leak', choices=LeakSizeType, value=LeakSizeType.ld01)
        self.mass_flow = NumField('Release mass flow rate', slug='mass_flow', unit=MassFlow.kgs, value=None)  # if unchoked

        self.facility_l = NumField('Facility length (x)', slug='facility_l', unit=Distance.m, value=20)
        self.facility_w = NumField('Facility width (z)', slug='facility_w', unit=Distance.m, value=12)
        self.exclusion = NumField('Exclusion radius', slug='exclusion', unit=Distance.m, value=0.01)

        self.override_d01 = NumField('0.01% release annual frequency', slug='override_d01', value=None)
        self.override_d1 = NumField('0.10% release annual frequency', slug='override_d1', value=None)
        self.override_1 = NumField('1% release annual frequency', slug='override_1', value=None)
        self.override_10 = NumField('10% release annual frequency', slug='override_10', value=None)
        self.override_100 = NumField('100% release annual frequency', slug='override_100', value=None)
        self.override_fail = NumField('100% release annual frequency (accidents and shutdown failures)',
                                      slug='override_fail', value=None)
        self.detection = NumField('Gas detection credit', slug='detection', value=0.9,
                                  min_value = 0, max_value = 1)

        self.compressor_d01 = LognormField(label="0.01%", slug='compressor_d01')
        self.compressor_d1 = LognormField(label="0.1%", slug='compressor_d1')
        self.compressor_1 = LognormField(label="1%", slug='compressor_1')
        self.compressor_10 = LognormField(label="10%", slug='compressor_10')
        self.compressor_100 = LognormField(label="100%", slug='compressor_100')

        self.vessel_d01 = LognormField(label="0.01%", slug='vessel_d01')
        self.vessel_d1 = LognormField(label="0.1%", slug='vessel_d1')
        self.vessel_1 = LognormField(label="1%", slug='vessel_1')
        self.vessel_10 = LognormField(label="10%", slug='vessel_10')
        self.vessel_100 = LognormField(label="100%", slug='vessel_100')

        self.filter_d01 = LognormField(label="0.01%", slug='filter_d01')
        self.filter_d1 = LognormField(label="0.1%", slug='filter_d1')
        self.filter_1 = LognormField(label="1%", slug='filter_1')
        self.filter_10 = LognormField(label="10%", slug='filter_10')
        self.filter_100 = LognormField(label="100%", slug='filter_100')

        self.flange_d01 = LognormField(label="0.01%", slug='flange_d01')
        self.flange_d1 = LognormField(label="0.1%", slug='flange_d1')
        self.flange_1 = LognormField(label="1%", slug='flange_1')
        self.flange_10 = LognormField(label="10%", slug='flange_10')
        self.flange_100 = LognormField(label="100%", slug='flange_100')

        self.hose_d01 = LognormField(label="0.01%", slug='hose_d01')
        self.hose_d1 = LognormField(label="0.1%", slug='hose_d1')
        self.hose_1 = LognormField(label="1%", slug='hose_1')
        self.hose_10 = LognormField(label="10%", slug='hose_10')
        self.hose_100 = LognormField(label="100%", slug='hose_100')

        self.joint_d01 = LognormField(label="0.01%", slug='joint_d01')
        self.joint_d1 = LognormField(label="0.1%", slug='joint_d1')
        self.joint_1 = LognormField(label="1%", slug='joint_1')
        self.joint_10 = LognormField(label="10%", slug='joint_10')
        self.joint_100 = LognormField(label="100%", slug='joint_100')

        self.pipe_d01 = LognormField(label="0.01%", slug='pipe_d01')
        self.pipe_d1 = LognormField(label="0.1%", slug='pipe_d1')
        self.pipe_1 = LognormField(label="1%", slug='pipe_1')
        self.pipe_10 = LognormField(label="10%", slug='pipe_10')
        self.pipe_100 = LognormField(label="100%", slug='pipe_100')

        self.valve_d01 = LognormField(label="0.01%", slug='valve_d01')
        self.valve_d1 = LognormField(label="0.1%", slug='valve_d1')
        self.valve_1 = LognormField(label="1%", slug='valve_1')
        self.valve_10 = LognormField(label="10%", slug='valve_10')
        self.valve_100 = LognormField(label="100%", slug='valve_100')

        self.instrument_d01 = LognormField(label="0.01%", slug='instrument_d01')
        self.instrument_d1 = LognormField(label="0.1%", slug='instrument_d1')
        self.instrument_1 = LognormField(label="1%", slug='instrument_1')
        self.instrument_10 = LognormField(label="10%", slug='instrument_10')
        self.instrument_100 = LognormField(label="100%", slug='instrument_100')

        self.exchanger_d01 = LognormField(label="0.01%", slug='exchanger_d01')
        self.exchanger_d1 = LognormField(label="0.1%", slug='exchanger_d1')
        self.exchanger_1 = LognormField(label="1%", slug='exchanger_1')
        self.exchanger_10 = LognormField(label="10%", slug='exchanger_10')
        self.exchanger_100 = LognormField(label="100%", slug='exchanger_100')

        self.vaporizer_d01 = LognormField(label="0.01%", slug='vaporizer_d01')
        self.vaporizer_d1 = LognormField(label="0.1%", slug='vaporizer_d1')
        self.vaporizer_1 = LognormField(label="1%", slug='vaporizer_1')
        self.vaporizer_10 = LognormField(label="10%", slug='vaporizer_10')
        self.vaporizer_100 = LognormField(label="100%", slug='vaporizer_100')

        self.arm_d01 = LognormField(label="0.01%", slug='arm_d01')
        self.arm_d1 = LognormField(label="0.1%", slug='arm_d1')
        self.arm_1 = LognormField(label="1%", slug='arm_1')
        self.arm_10 = LognormField(label="10%", slug='arm_10')
        self.arm_100 = LognormField(label="100%", slug='arm_100')

        self.extra1_d01 = LognormField(label="0.01%", slug='extra1_d01')
        self.extra1_d1 = LognormField(label="0.1%", slug='extra1_d1')
        self.extra1_1 = LognormField(label="1%", slug='extra1_1')
        self.extra1_10 = LognormField(label="10%", slug='extra1_10')
        self.extra1_100 = LognormField(label="100%", slug='extra1_100')

        self.extra2_d01 = LognormField(label="0.01%", slug='extra2_d01')
        self.extra2_d1 = LognormField(label="0.1%", slug='extra2_d1')
        self.extra2_1 = LognormField(label="1%", slug='extra2_1')
        self.extra2_10 = LognormField(label="10%", slug='extra2_10')
        self.extra2_100 = LognormField(label="100%", slug='extra2_100')

        # Order must match component_data.py dicts in QRA backend
        self.compressor_leak_freqs = [self.compressor_d01, self.compressor_d1, self.compressor_1, self.compressor_10, self.compressor_100]
        self.vessel_leak_freqs = [self.vessel_d01, self.vessel_d1, self.vessel_1, self.vessel_10, self.vessel_100]
        self.filter_leak_freqs = [self.filter_d01, self.filter_d1, self.filter_1, self.filter_10, self.filter_100]
        self.flange_leak_freqs = [self.flange_d01, self.flange_d1, self.flange_1, self.flange_10, self.flange_100]
        self.hose_leak_freqs = [self.hose_d01, self.hose_d1, self.hose_1, self.hose_10, self.hose_100]
        self.joint_leak_freqs = [self.joint_d01, self.joint_d1, self.joint_1, self.joint_10, self.joint_100]
        self.pipe_leak_freqs = [self.pipe_d01, self.pipe_d1, self.pipe_1, self.pipe_10, self.pipe_100]
        self.valve_leak_freqs = [self.valve_d01, self.valve_d1, self.valve_1, self.valve_10, self.valve_100]
        self.instrument_leak_freqs = [self.instrument_d01, self.instrument_d1, self.instrument_1, self.instrument_10, self.instrument_100]
        self.exchanger_leak_freqs = [self.exchanger_d01, self.exchanger_d1, self.exchanger_1, self.exchanger_10, self.exchanger_100]
        self.vaporizer_leak_freqs = [self.vaporizer_d01, self.vaporizer_d1, self.vaporizer_1, self.vaporizer_10, self.vaporizer_100]
        self.arm_leak_freqs = [self.arm_d01, self.arm_d1, self.arm_1, self.arm_10, self.arm_100]
        self.extra1_leak_freqs = [self.extra1_d01, self.extra1_d1, self.extra1_1, self.extra1_10, self.extra1_100]
        self.extra2_leak_freqs = [self.extra2_d01, self.extra2_d1, self.extra2_1, self.extra2_10, self.extra2_100]
        self.component_leak_sets = [self.compressor_leak_freqs,
                                    self.vessel_leak_freqs,
                                    self.filter_leak_freqs,
                                    self.flange_leak_freqs,
                                    self.hose_leak_freqs,
                                    self.joint_leak_freqs,
                                    self.pipe_leak_freqs,
                                    self.valve_leak_freqs,
                                    self.instrument_leak_freqs,
                                    self.exchanger_leak_freqs,
                                    self.vaporizer_leak_freqs,
                                    self.arm_leak_freqs,
                                    self.extra1_leak_freqs,
                                    self.extra2_leak_freqs]

        self.fail_nozzle_po = DistributionField("Nozzle (pop-off)", "Pop-off", slug="nozzle_po",
                                                distr=FailureDistribution.beta, pa=0.5, pb=610415.5)
        self.fail_nozzle_ftc = DistributionField("Nozzle (failure to close)", "Failure to close", slug="nozzle_ftc",
                                                 distr=FailureDistribution.ev, pa=0.002, pb=0)
        self.fail_mvalve_ftc = DistributionField("Manual valve (failure to close)", "Failure to close", slug="mvalve_ftc",
                                                 distr=FailureDistribution.ev, pa=0.001, pb=0)
        self.fail_svalve_ftc = DistributionField("Solenoid valve (failure to close)", "Failure to close", slug="svalve_ftc",
                                                 distr=FailureDistribution.ev, pa=0.002, pb=0)
        self.fail_svalve_ccf = DistributionField("Solenoid valve (common-cause failure)", "Common-cause failure", slug="svalve_ccf",
                                                 distr=FailureDistribution.ev, pa=0.000128, pb=0)
        self.fail_pvalve_fto = DistributionField("Pressure relief valve (failure to open)", "Failure to open", slug="pvalve_fto",
                                                 distr=FailureDistribution.ln, pa=-11.74, pb=0.67)
        self.fail_coupling_ftc = DistributionField("Breakaway coupling (failure to close)", "Failure to close", slug="coupling_ftc",
                                                   distr=FailureDistribution.beta, pa=0.5, pb=5031)
        self.acc_fuel_overp = DistributionField("Overpressure during fueling", "", slug="acc_fuel_overp",
                                                distr=FailureDistribution.beta, pa=3.5, pb=310289.5)
        self.acc_driveoff = DistributionField("Driveoff", "", slug="acc_driveoff",
                                              distr=FailureDistribution.beta, pa=31.5, pb=610384.5)

        self.overp_method = ChoiceField('Overpressure method', slug='overp_method', choices=OverpressureType,
                                               value=OverpressureType.bst)
        self.mach_speed = ChoiceField('Mach flame speed', slug='mach_speed', choices=MachSpeed, value=MachSpeed.s0d35)
        self.tnt_factor = NumField('TNT equivalence factor', slug='tnt_factor', value=0.03)
        self.thermal_probit = ChoiceField('Thermal probit model', slug='thermal_probit', choices=ThermalProbitModel,
                                          value=ThermalProbitModel.eis)
        self.exposure_time = NumField('Thermal exposure time', slug='exposure_t', unit=Time.sec, value=30)
        self.overp_probit = ChoiceField('Overpressure probit model', slug='overp_probit', choices=OverpressureProbitModel,
                                        value=OverpressureProbitModel.hea)

        # keys must match UI (see OccupantField.qml)
        self.occupant_data = [
            {
                'occupants': 9,
                'descrip': 'Station workers',
                'hours': 2000,
                'units': 0,  # index corresponding to [meter, inch, ft, yard]. Convert to meters in analysis.
                'xd': 1,
                'xa': 1,
                'xb': 20,
                'yd': 2,
                'ya': 0,
                'yb': 0,
                'zd': 1,
                'za': 1,
                'zb': 12,
            }
        ]

        self.ignition_data = [
            {
                'min': None,
                'max': 0.125,
                'immed': 0.008,
                'delay': 0.004,
            },
            {
                'min': 0.125,
                'max': 6.25,
                'immed': 0.053,
                'delay': 0.027,
            },
            {
                'min': 6.25,
                'max': None,
                'immed': 0.23,
                'delay': 0.12,
            },
        ]

        # store list of parameters for easy iteration; manually skip excluded params
        self.fields = []
        for attr, value in self.__dict__.items():
            if issubclass(type(value), FieldBase):
                if value.slug == "output_directory":
                    continue
                self.fields.append(value)

        # Update fuelmix when user changes a concentration
        for field in self.fuels:
            field.changed += lambda fld: self.handle_conc_change(fld)

        # Update component data when phase changed (fuel changes handle this elsewhere)
        self.rel_phase.changed += lambda x: self.set_fuel_specific_parameters()

        self.set_fuel_specific_parameters()
        super().post_init()

    # ==============================
    # ==== PARAMETER VALIDATION ====
    def check_valid(self) -> ValidationResponse:
        """Validates parameter values in form-wide manner. For example, validation of a parameter which depends on another parameter
        is done here.

        Notes
        -----
        QRA validity is checked by QRA form and not here.
        Checks for errors first, then warnings, so most serious is shown first.

        Returns : ValidationResponse
        """
        shared_state_resp = self.check_valid_shared_state()
        if shared_state_resp.status != InputStatus.GOOD:
            return shared_state_resp

        return ValidationResponse(InputStatus.GOOD, '')

    def check_valid_shared_state(self) -> ValidationResponse:
        """Checks validity of state parameters.

        Notes
        -----
        Use HTML-style or markdown for text formatting and not RichText, as it will break dynamic width calculation in QML.

        """
        conc_sum = self._get_species_conc_sum()
        if conc_sum < 100 or conc_sum > 100:
            return ValidationResponse(InputStatus.WARN, 'Fuel concentrations must sum to 100%')

        # verify release pressure is valid
        if self.is_saturated:
            rel_p = self.rel_p.value_raw / 1e6
            amb_p = self.amb_p.value_raw / 1e6
            rel_is_abs = self.rel_p_abs.value
            if not rel_is_abs:
                rel_p += amb_p
            if self.fuel_mix.value in [FuelMixType.h2, FuelMixType.c3h8, FuelMixType.ch4]:
                p_crit = species_dict[self.fuel_mix.value].p_crit
            else:
                p_crit = 0

            if rel_p <= amb_p or rel_p > p_crit:
                return ValidationResponse(InputStatus.ERROR, f'Saturated release pressure must be between ambient pressure '
                                                             f'and critical pressure ({p_crit} MPa)')

        return ValidationResponse(InputStatus.GOOD, '')

    def check_valid_qra(self) -> ValidationResponse:
        """Checks validity of state parameters. """
        # can't use bauwens overpressure method with head impact or structural collapse
        if (self.overp_method.value == OverpressureType.bau and self.overp_probit.value in [OverpressureProbitModel.hea,
                                                                                            OverpressureProbitModel.col]):
            return ValidationResponse(InputStatus.ERROR,
                                      '<i>Bauwens</i> overpressure method does not produce impulse values and cannot be used '
                                      'with <i>head impact</i> or <i>structural collapse</i> overpressure probit models')

        if self.fuel_mix.value != FuelMixType.h2:
            return ValidationResponse(InputStatus.WARN,
                                      'Default data for dispenser failures were generated for high-pressure gaseous hydrogen '
                                      'systems and may not be appropriate for the selected fuel')

        if self.pipe_d.is_null and (self.pipe_od.is_null or self.pipe_thick.is_null):
            return ValidationResponse(InputStatus.ERROR,
                                      'Enter pipe inner diameter or outer diameter and thickness')

        return ValidationResponse(InputStatus.GOOD, '')

    def check_valid_accum(self) -> ValidationResponse:
        last_t = float(self.out_ts.value[-1])
        if len(self.out_ts.value) > 0 and self.t_max.value < last_t:
            return ValidationResponse(InputStatus.ERROR,
                                      'Enter maximum time greater than output times')
        return ValidationResponse(InputStatus.GOOD, '')

    def refresh_pipe_diams(self, _):
        if not self.pipe_d.is_null:
            self.pipe_od.value = None
            self.pipe_thick.value = None

    def get_diff(self) -> list:
        """Returns fields which changed in last history tracking. """
        curr = self.to_dict()
        if self._redo_history:
            # was an undo
            prev = self._redo_history[-1]
        elif len(self._history) > 1:
            prev = self._history[-2]
        else:
            return []

        diff_keys = []
        for key in curr.keys():
            c1 = curr.get(key)
            p1 = prev.get(key)
            if c1 != p1:
                diff_keys.append(key)
        return diff_keys

    def _handle_full_refresh(self, study_type: ChoiceField):
        """Refresh form and available parameters when analysis sample type changes. """
        self._refresh_intermediate_params()
        self._record_state_change()

    def _refresh_intermediate_params(self) -> None:
        """ Updates intermediate parameters via pool. """
        if self._intermed_in_progress:
            return
        self._intermed_in_progress = True
        params = self.get_prepped_param_dict()

        # results = _calc_intermediate_params(params, study_type)
        # fill in values

        self._intermed_in_progress = False

    # ===================
    # ==== JET FLAME ====
    def set_jet_flame_point_data(self, idx: int, data: list) -> None:
        self.jet_flame_points[idx] = deepcopy(data)
        self._handle_param_change()

    def add_jet_flame_point(self):
        self.jet_flame_points.append((0,0,0))
        self._handle_param_change()

    def remove_jet_flame_point(self, index: int):
        """Removes jet_flame flow rate threshold specified by index into jet_flame list.
        """
        removed_data = self.jet_flame_points.pop(index)
        self._handle_param_change()

    # =======================
    # ==== UNCONF. OVERP ====
    def set_uo_point_data(self, idx: int, data: list) -> None:
        self.uo_points[idx] = deepcopy(data)
        self._handle_param_change()

    def add_uo_point(self):
        self.uo_points.append((0, 0, 0))
        self._handle_param_change()

    def remove_uo_point(self, index: int):
        """Removes uo flow rate threshold specified by index into uo list.
        """
        removed_data = self.uo_points.pop(index)
        self._handle_param_change()

    # =======================
    # ==== QRA FUNCTIONS ====
    def set_occupant_data(self, data: dict):
        self.occupant_data = data
        self._handle_param_change()

    def set_ignition_data(self, idx: int, data: dict) -> None:
        self.ignition_data[idx] = deepcopy(data)
        self._handle_param_change()

    def get_ignition_thresholds(self):
        result = [elem['max'] for elem in self.ignition_data if elem['max'] is not None]
        return result

    def add_ignition(self, threshold: float):
        thresholds = self.get_ignition_thresholds()
        if threshold in thresholds:
            return
        first = self.ignition_data[0]
        last = self.ignition_data[-1]

        if threshold < first['max']:
            first['min'] = threshold
            self.ignition_data.insert(0, {'min': None, 'max': threshold, 'immed': 0, 'delay': 0})

        elif threshold > last['min']:
            # insert at end of list
            last['max'] = threshold
            self.ignition_data.append({'min': threshold, 'max': None, 'immed': 0, 'delay': 0})

        else:
            # creates new row and updates old one. New row inserted before modified one.
            idx = 1
            new_min = 0
            for i in range(1, len(self.ignition_data) - 1):  # skip first and last
                elem = self.ignition_data[i]
                if elem['min'] < threshold < elem['max']:
                    idx = i
                    new_min = elem['min']
                    elem['min'] = threshold
                    break
            self.ignition_data.insert(idx, {'min': new_min, 'max': threshold, 'immed': 0, 'delay': 0})

        self._handle_param_change()

    def remove_ignition(self, index: int):
        """Removes ignition flow rate threshold specified by index into ignition list. Always refers to max rate of that entry.
        Last row is never removed.
        """
        removed_data = self.ignition_data.pop(index)
        # removed_rate = removed_data['max']
        next_row = self.ignition_data[index]
        if index > 0:
            # not the first row
            prev_row = self.ignition_data[index - 1]
            next_row['min'] = prev_row['max']
        else:
            next_row['min'] = None
        self._handle_param_change()

    # ==============
    # FUEL FUNCTIONS
    def is_unchoked(self):
        choked_flow_ratio = 0
        eps = 1e-5
        for fuel in self.fuels:
            if fuel.value > eps:
                species_data = species_dict[fuel.slug]
                choked_flow_ratio += (fuel.value / 100.) * species_data.choked_flow  # convert from percentage

        amb_p = self.amb_p.value_raw
        rel_p = self.rel_p.value_raw
        return np.abs(choked_flow_ratio) > eps and rel_p < choked_flow_ratio * amb_p

    def handle_conc_change(self, _: NumField):
        """Adjusts concentration and changes fuel_mix param to match.

        Notes
        -----
        Expensive! Called on each field change.
        """
        if not self._sync_conc_and_freqs:
            return

        conc_dict = self._get_conc_dict()
        keys = list(conc_dict.keys())
        # changes still processing
        if len(keys) == 0:
            return

        key = keys[0]

        # TODO: add func to match named blends
        if len(conc_dict) > 1:
            self.fuel_mix.set_from_model(FuelMixType.man)
        elif key == self.conc_h2.label.lower():
            self.fuel_mix.set_from_model(FuelMixType.h2)
        elif key == self.conc_ch4.label.lower():
            self.fuel_mix.set_from_model(FuelMixType.ch4)
        elif key == self.conc_pro.label.lower():
            self.fuel_mix.set_from_model(FuelMixType.c3h8)
        else:
            self.fuel_mix.set_from_model(FuelMixType.man)

        self.set_fuel_specific_parameters()

    def set_fuel_specific_parameters(self):
        """Updates component counts, leak frequencies, ignition data, probit to match selected fuel and phase.
        Refer to Component_Count_Defaults.pdf (from issue #209).

        """
        recording_state = self._record_changes
        # Do one state save at end, otherwise will fill up with leak freq calculation adjustments
        self._record_changes = False

        species = self.fuel_mix.value
        phase = self.rel_phase.value

        def _set_counts(n_compressors=0, n_vessels=0, n_valves=0, n_instruments=0, n_joints=0, n_hoses=0, n_filters=0,
                        pipe_l=0, n_flanges=0, n_exchangers=0, n_vaporizers=0, n_arms=0, n_extra1=0, n_extra2=0):
            self.pipe_l.value = pipe_l  # assume same unit
            self.n_compressors.value = n_compressors
            self.n_vessels.value = n_vessels
            self.n_valves.value = n_valves
            self.n_instruments.value = n_instruments
            self.n_joints.value = n_joints
            self.n_hoses.value = n_hoses
            self.n_filters.value = n_filters
            self.n_flanges.value = n_flanges
            self.n_exchangers.value = n_exchangers
            self.n_vaporizers.value = n_vaporizers
            self.n_arms.value = n_arms
            self.n_extra1.value = n_extra1
            self.n_extra2.value = n_extra2

        if species == FuelMixType.h2 and phase in [PhaseType.sliq, PhaseType.svap]:
            _set_counts(n_compressors=0, n_vessels=1, n_valves=44, n_instruments=0, n_joints=0, n_hoses=1, n_filters=0,
                        pipe_l=30, n_flanges=8, n_exchangers=0, n_vaporizers=0, n_arms=0, n_extra1=0, n_extra2=0)

        elif species == FuelMixType.ch4:
            if phase == PhaseType.fluid:
                _set_counts(n_compressors=1, n_vessels=2, n_valves=7, n_instruments=5, n_joints=43, n_hoses=1, n_filters=3,
                            pipe_l=30, n_flanges=0, n_exchangers=1, n_vaporizers=0, n_arms=0, n_extra1=0, n_extra2=0)
            else:
                _set_counts(n_compressors=0, n_vessels=1, n_valves=44, n_instruments=0, n_joints=0, n_hoses=1, n_filters=0,
                            pipe_l=30, n_flanges=8, n_exchangers=1, n_vaporizers=0, n_arms=0, n_extra1=0, n_extra2=0)

        elif species == FuelMixType.c3h8:
            _set_counts(n_compressors=1, n_vessels=1, n_valves=44, n_instruments=0, n_joints=0, n_hoses=1, n_filters=2,
                        pipe_l=30, n_flanges=8, n_exchangers=0, n_vaporizers=0, n_arms=0, n_extra1=0, n_extra2=0)

        else:
            # GH2, blends
            _set_counts(n_compressors=1, n_vessels=2, n_valves=7, n_instruments=5, n_joints=43, n_hoses=1, n_filters=3,
                        pipe_l=30, n_flanges=0, n_exchangers=0, n_vaporizers=0, n_arms=0, n_extra1=0, n_extra2=0)

        # update thermal harm probit
        if species == FuelMixType.h2:
            self.thermal_probit.value = ThermalProbitModel.eis
        else:
            self.thermal_probit.value = ThermalProbitModel.tsa


        if species in [FuelMixType.ch4, FuelMixType.c3h8]:
            # Per issue 106
            self.ignition_data = [{'min': None, 'max': 1, 'immed': 0.007, 'delay': 0.003,},
                                  {'min': 1, 'max': 50, 'immed': 0.047, 'delay': 0.023,},
                                  {'min': 50, 'max': None, 'immed': 0.20, 'delay': 0.10,}]

        else:
            self.ignition_data = [{'min': None, 'max': 0.125, 'immed': 0.008, 'delay': 0.004,},
                                  {'min': 0.125, 'max': 6.25, 'immed': 0.053, 'delay': 0.027,},
                                  {'min': 6.25, 'max': None, 'immed': 0.23, 'delay': 0.12,}]


        # TODO: how to handle blends?
        if species in [FuelMixType.h2, FuelMixType.ch4, FuelMixType.c3h8]:
            leak_data = get_leak_frequency_set_for_species(species, phase)
            for data_list, field_list in zip(leak_data.values(), self.component_leak_sets):
                for leak_data, field in zip(data_list, field_list):
                    field.mu = leak_data[0]
                    field.sigma = leak_data[1]

        self._record_changes = recording_state
        self._handle_param_change()

    def get_fuel_param(self, label):
        for fuel in self.fuels:
            if fuel.label == label:
                return fuel

    def _clear_concs(self):
        for fuel in self.fuels:
            if fuel.value > 0:
                fuel.set_from_model(0)

    def allocate_remaining_conc(self, fuel_label):
        """Allocates remaining amount to specified fuel parameter, if total is below 100%. """
        conc_sum = self._get_species_conc_sum()
        rem = (100 - conc_sum) / 100.  # convert to raw [0,1]
        if rem > 0:
            fuel_param = self.get_fuel_param(fuel_label)
            rem += fuel_param.value_raw
            fuel_param.set_from_model(rem)
            fuel_param.changed.notify(fuel_param)

    def set_fuel_mix(self, mix: str):
        """Sets species concentrations according to user-selected mix option. Note that blend (manual) just clears all. """
        self._clear_concs()

        if mix == FuelMixType.h2:
            self.conc_h2.set_from_model(1)
        elif mix == FuelMixType.ch4:
            self.conc_ch4.set_from_model(1)
        elif mix == FuelMixType.c3h8:
            self.conc_pro.set_from_model(1)

        elif mix == FuelMixType.nist1:
            self.conc_ch4.set_from_model(0.96521)
            self.conc_pro.set_from_model(0.0046)
            self.conc_n2.set_from_model(0.0026)
            self.conc_co2.set_from_model(0.00596)
            self.conc_eth.set_from_model(0.01819)
            self.conc_nbu.set_from_model(0.00101)
            self.conc_isb.set_from_model(0.00098)
            self.conc_npe.set_from_model(0.00032)
            self.conc_isp.set_from_model(0.00047)
            self.conc_nhx.set_from_model(0.00066)

        elif mix == FuelMixType.nist2:
            self.conc_ch4.set_from_model(0.90673)
            self.conc_pro.set_from_model(0.00828)
            self.conc_n2.set_from_model(0.03128)
            self.conc_co2.set_from_model(0.00468)
            self.conc_eth.set_from_model(0.04528)
            self.conc_nbu.set_from_model(0.00156)
            self.conc_isb.set_from_model(0.00104)
            self.conc_npe.set_from_model(0.00044)
            self.conc_isp.set_from_model(0.00032)
            self.conc_nhx.set_from_model(0.00039)

        elif mix == FuelMixType.rg2:
            self.conc_ch4.set_from_model(0.85905)
            self.conc_pro.set_from_model(0.02302)
            self.conc_n2.set_from_model(0.01007)
            self.conc_co2.set_from_model(0.01495)
            self.conc_eth.set_from_model(0.08492)
            self.conc_nbu.set_from_model(0.00351)
            self.conc_isb.set_from_model(0.00349)
            self.conc_npe.set_from_model(0.00048)
            self.conc_isp.set_from_model(0.00051)

        elif mix == FuelMixType.gu1:
            self.conc_ch4.set_from_model(0.81441)
            self.conc_pro.set_from_model(0.00605)
            self.conc_n2.set_from_model(0.13465)
            self.conc_co2.set_from_model(0.00985)
            self.conc_eth.set_from_model(0.033)
            self.conc_nbu.set_from_model(0.00104)
            self.conc_isb.set_from_model(0.001)

        elif mix == FuelMixType.gu2:
            self.conc_ch4.set_from_model(0.81212)
            self.conc_pro.set_from_model(0.00895)
            self.conc_n2.set_from_model(0.05702)
            self.conc_co2.set_from_model(0.07585)
            self.conc_eth.set_from_model(0.04303)
            self.conc_nbu.set_from_model(0.00152)
            self.conc_isb.set_from_model(0.00151)

        self.set_fuel_specific_parameters()
        self.fuel_mix.changed.notify(self.conc_h2)

    # ================
    # Instant analyses
    def compute_tpd(self):
        species = self.fuel_mix.value
        phase = self.rel_phase.value
        temp = None if self.etk_t.is_null else self.etk_t.value_raw
        pres = None if self.etk_p.is_null else self.etk_p.value_raw
        dens = None if self.etk_d.is_null else self.etk_d.value_raw

        try:
            v1, v2 = phys.compute_thermo_param(species, phase, temp, pres, dens)
        except ValueError as err:
            results = dict(status=-1, error=err, message='Error during analysis - verify inputs are within acceptable range')
            return results
        except Exception as err:
            results = dict(status=-1, error=err, message="Error during analysis - check log for details")
            return results

        if self.is_saturated:
            if pres is None:
                self.etk_p.set_from_model(v1)
            else:
                self.etk_d.set_from_model(v1)
            self.etk_t.set_from_model(v2)

        else:
            if temp is None:
                self.etk_t.set_from_model(v1)
            elif pres is None:
                self.etk_p.set_from_model(v1)
            else:
                self.etk_d.set_from_model(v1)
        return {'status': 1}

    def compute_tank_mass(self):
        species = self.fuel_mix.value
        phase = self.rel_phase.value
        temp = None if self.etk_t.is_null else self.etk_t.value_raw
        pres = None if self.etk_p.is_null else self.etk_p.value_raw
        vol = None if self.etk_v.is_null else self.etk_v.value_raw
        mass = None if self.etk_m.is_null else self.etk_m.value_raw

        try:
            v1, v2 = phys.compute_tank_mass_param(species, phase=phase, temp=temp, pres=pres, vol=vol, mass=mass)
        except ValueError as err:
            results = dict(status=-1, error=err, message='Error during analysis - verify inputs are within acceptable range')
            return results
        except Exception as err:
            results = dict(status=-1, error=err, message="Error during analysis - check log for details")
            return results

        if self.is_saturated:
            if pres is None:
                self.etk_p.set_from_model(v1)
            elif vol is None:
                self.etk_v.set_from_model(v1)
            else:
                self.etk_m.set_from_model(v1)
            self.etk_t.set_from_model(v2)

        else:
            if temp is None:
                self.etk_t.set_from_model(v1)
            elif pres is None:
                self.etk_p.set_from_model(v1)
            elif vol is None:
                self.etk_v.set_from_model(v1)
            else:
                self.etk_m.set_from_model(v1)
        return {'status': 1}

    def compute_mass_flow_blowdown(self):
        species = self.fuel_mix.value
        phase = self.rel_phase.value
        rel_t = self.etk_t.value_raw
        rel_p = self.etk_p.value_raw
        amb_p = self.etk_p_amb.value_raw
        orif_d = self.etk_orif_d.value_raw
        discharge = self.etk_discharge.value_raw
        is_blowdown = self.etk_is_blowdown.value
        vol = self.etk_v.value_raw if is_blowdown else None
        output_dir = self.session_dir.value

        if phase == 'fluid':
            phase = None
        else:
            rel_t = None

        rel_fluid = phys.create_fluid(species, rel_t, rel_p, density=None, phase=phase)

        try:
            results = phys.compute_mass_flow(rel_fluid,
                                             orif_diam=orif_d,
                                             amb_pres=amb_p,
                                             is_steady=not is_blowdown,
                                             tank_vol=vol,
                                             dis_coeff=discharge,
                                             output_dir=output_dir,
                                             create_plot=is_blowdown)
            results['status'] = 1
        except Exception as err:
            results = dict(status=-1, error=err, message="Error during analysis - check log for details")
            return results

        if not is_blowdown:
            flow = results['rates'][0]
            self.etk_flow.value = flow
        else:
            t_empty = results['time_to_empty']
            self.etk_blowdown_plot = results['plot']
            self.etk_t_empty.value = t_empty

        return results

    def get_prepped_param_dict(self):
        """Returns dict of parameters prepared for analysis submission. """
        dct = super().get_prepped_param_dict()
        dct |= {
            'jet_flame_points': deepcopy(self.jet_flame_points),
            'uo_points': deepcopy(self.uo_points),
            'occupant_data': deepcopy(self.occupant_data),
            'ignition_data': deepcopy(self.ignition_data),
            'species_dict': self._get_prepped_conc_dict_for_backend(),
        }

        return dct

    def to_dict(self) -> dict:
        """Returns representation of this state as dict of parameter dicts. """
        result = super().to_dict()
        result |= {
            'jet_flame_points': deepcopy(self.jet_flame_points),
            'uo_points': deepcopy(self.uo_points),
            'occupant_data': deepcopy(self.occupant_data),
            'ignition_data': deepcopy(self.ignition_data),
        }
        return result

    def _from_dict(self, data: dict):
        """Overwrites state analysis parameter data from dict containing all parameters.

        Parameters
        ----------
        data : dict
            Analysis parameter data with keys matching internal snake_case naming.

        """
        # Verify all parameters present. Must account for new/revised params.
        old_sync_val = self._sync_conc_and_freqs
        self._sync_conc_and_freqs = False

        if 'analysis_name' not in data:
            data['analysis_name'] = self.analysis_name.to_dict()

        self.jet_flame_points = deepcopy(data['jet_flame_points'])
        self.uo_points = deepcopy(data['uo_points'])
        self.occupant_data = deepcopy(data['occupant_data'])
        self.ignition_data = deepcopy(data['ignition_data'])

        super()._from_dict(data)

        # do the update manually
        self._sync_conc_and_freqs = old_sync_val
        self.handle_conc_change(self.override_d01)

    def _get_conc_dict(self) -> dict:
        """Dict of fluid full names with concentrations summing to 100."""
        eps = 0.00001
        result = {}  # only include species with >0 concentration

        for fuel in self.fuels:
            if fuel.value > eps:
                result[fuel.label.lower()] = fuel.value

        return result

    def _get_prepped_conc_dict_for_backend(self) -> dict:
        """Dict of fluid slug names with concentrations summing to 1."""
        eps = 0.00001
        result = {}  # only include species with >0 concentration

        for fuel in self.fuels:
            if fuel.value > eps:
                result[fuel.slug.lower()] = fuel.value_raw  # as decimal
        return result

    def _get_species_conc_sum(self) -> float:
        eps = 0.00001
        sum = 0
        for fuel in self.fuels:
            if fuel.value > eps:
                sum += fuel.value
        return round(sum, 3)

    def _handle_param_change(self):
        self._refresh_intermediate_params()
        super()._handle_param_change()

    def load_demo_file_data(self, label=''):
        """Loads sample data from specified demo file. """
        if label == '':
            demo_file = self._demo_dir.joinpath('h2_defaults.hrm')
        elif label == 'demo1':
            demo_file = self._demo_dir.joinpath('demo1.hrm')
        else:
            demo_file = self._demo_dir.joinpath('demo2.hrm')
        self.load_data_from_file(demo_file)

    @property
    def is_saturated(self):
        return self.rel_phase.value != 'fluid'

