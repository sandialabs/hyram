"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
from typing import Optional

import numpy as np

import logging
import warnings

from hyramgui import app_settings
from hyramgui.hygu.utils import helpers
from pathlib import Path

from hyram.phys import api as phys
from hyram.utilities.custom_warnings import PhysicsWarning


def _get_phase(gui_val):
    if gui_val in ['gas', 'liquid']:
        return gui_val
    else:
        return None


def do_physics_analysis(analysis_id, params: dict, global_status_dict: dict):
    """
    Executes new analysis with dict of parameter values prepped for API consumption and queries for relevant results once complete.
    Also updates sub-process copy of settings.

    Parameters
    ----------
    analysis_id : int
        Unique analysis id
    params : dict
        Map of prepped parameters ready for analysis {slug: Characterization}

    Notes
    -----
    Called within new process via cloned state object.
    Dict is used to avoid sub-processing issues with cloning state object.

    """
    # Update this process' version of GUI settings
    app_settings.SESSION_DIR = params['session_dir']

    # multiprocessing does not support logging to same file. Must implement queue handler if this functionality is desired.
    proc_log = logging.getLogger(__name__)
    proc_log.setLevel(logging.INFO)

    # create output dir for this analysis
    als_name = helpers.convert_string_to_filename(params['analysis_name'])
    now_str = helpers.get_now_str()
    output_dirname = f'{now_str}_A{analysis_id:03d}_{als_name[0:10]}'
    output_dir = app_settings.SESSION_DIR.joinpath(output_dirname)
    Path.mkdir(output_dir, parents=True, exist_ok=True)

    # Update instance of api settings
    # pass manually since dir name based on time and may not be present in settings or idempotent
    # api_settings.OUTPUT_DIR = output_dir
    # api_settings.USING_GUI = True
    # api_settings.SESSION_DIR = app_settings.SESSION_DIR
    # api_settings.GUI_STATUS_DICT = global_status_dict
    # api_settings.ANALYSIS_ID = analysis_id

    results = {'status': 1}

    als_type = params['analysis_type']
    amb_p = params['amb_p']
    amb_t = params['amb_t']
    rel_p = params['rel_p']
    rel_t = params['rel_t']
    rel_phase = _get_phase(params['fluid_phase'])
    discharge = params['discharge']
    leak_d = params['leak_d']
    verbose = True

    rel_species = params['fuel']
    if rel_species not in ['h2', 'ch4', 'c3h8']:
        rel_species = params['species_dict']

    # for each analysis type, pass through data and return results for post-processing
    try:
        with warnings.catch_warnings(record=True) as warning_list:
            amb_fluid = phys.create_fluid('AIR', amb_t, amb_p)
            rel_fluid = phys.create_fluid(rel_species, rel_t, rel_p, density=None, phase=rel_phase)

            # param dict keys are slugs and must match api kwarg naming
            if als_type == 'plume':
                auto = params['plume_auto_limits']

                results = phys.analyze_jet_plume(amb_fluid, rel_fluid,
                                                 orif_diam=leak_d,
                                                 mass_flow=params['plume_mass_flow'],
                                                 rel_angle=params['rel_angle'],
                                                 dis_coeff=discharge,
                                                 nozzle_model=params['nozzle'],
                                                 create_plot=True,
                                                 contours=params['mole_contours'],
                                                 xmin=None if auto else params['plume_xmin'],
                                                 xmax=None if auto else params['plume_xmax'],
                                                 ymin=None if auto else params['plume_ymin'],
                                                 ymax=None if auto else params['plume_ymax'],
                                                 vmin=None if auto else params['plume_mole_min'],
                                                 vmax=None if auto else params['plume_mole_max'],
                                                 plot_title=params['plume_plot_title'],
                                                 output_dir=output_dir,
                                                 verbose=verbose)
                results['status'] = 1
                results['output_dir'] = output_dir

            elif als_type == 'accum':
                t_p_pts = np.array([params['pair_ts'], params['pair_ps']]).T if params['do_p_ts'] else None
                line_ps = np.array(params['line_ps']) if params['do_p_lines'] else None

                results = phys.analyze_accumulation(amb_fluid, rel_fluid,
                                                    tank_volume=params['tank_v'],
                                                    orif_diam=leak_d,
                                                    rel_height=params['rel_h'],
                                                    enclos_height=params['enclosure_h'],
                                                    floor_ceil_area=params['floor_ceil_a'],
                                                    ceil_vent_xarea=params['ceil_xarea'],
                                                    ceil_vent_height=params['ceil_h'],
                                                    floor_vent_xarea=params['floor_xarea'],
                                                    floor_vent_height=params['floor_h'],
                                                    orif_dis_coeff=params['discharge'],
                                                    vol_flow_rate=params['vent_flow'],
                                                    dist_rel_to_wall=params['rel_wall_dist'],
                                                    times=params['out_ts'],
                                                    tmax=params['t_max'],
                                                    rel_area=None,
                                                    rel_angle=params['rel_angle'],
                                                    nozzle_key=params['nozzle'],
                                                    temp_pres_points=t_p_pts,
                                                    pres_ticks=line_ps,
                                                    is_steady=params['is_blowdown'] == False,
                                                    create_plots=True, output_dir=output_dir, verbose=verbose)
                results['status'] = 1
                results['output_dir'] = output_dir

            elif als_type == 'flame':
                auto = params['flame_auto_limits']

                flame_contours = params['flame_contours'] if len(params['flame_contours']) > 0 else None
                (temp_plot_filepath,
                 heatflux_filepath,
                 flux_data,
                 mass_flow,
                 srad,
                 visible_length,
                 radiant_frac) = phys.jet_flame_analysis(amb_fluid, rel_fluid,
                                                         orif_diam=params['leak_d'],
                                                         mass_flow=params['plume_mass_flow'],
                                                         dis_coeff=params['discharge'],
                                                         rel_angle=params['rel_angle'],
                                                         nozzle_key=params['nozzle'],
                                                         rel_humid=params['humid'],

                                                         analyze_flux=True,
                                                         flux_plot_filename=None,
                                                         flux_coordinates=params['jet_flame_points'],
                                                         flux_contours=flame_contours,
                                                         flux_xlims=None if auto else [params['heat_xmin'], params['heat_xmax']],
                                                         flux_ylims=None if auto else [params['heat_ymin'], params['heat_ymax']],
                                                         flux_zlims=None if auto else [params['heat_zmin'], params['heat_zmax']],

                                                         create_temp_plot=True,
                                                         temp_plot_filename=None,
                                                         temp_plot_title="",
                                                         temp_contours=None,
                                                         temp_xlims=None if auto else [params['temp_xmin'], params['temp_xmax']],
                                                         temp_ylims=None if auto else [params['temp_ymin'], params['temp_ymax']],
                                                         output_dir=output_dir,
                                                         verbose=verbose)

                flux_data_kwm2 = [flux / 1000 for flux in flux_data] if flux_data is not None else None

                results = {
                    'status': 1,
                    'flux_data': flux_data_kwm2,
                    'flux_plot_fpath': heatflux_filepath,
                    'temp_plot_fpath': temp_plot_filepath,
                    'mass_flow': mass_flow,
                    'srad': srad,
                    'visible_len': visible_length,
                    'radiant_frac': radiant_frac,
                    'output_dir': output_dir,
                }

            elif als_type == 'uo':
                auto = params['uo_auto_limits']
                overp_contours = params['uo_overp_contours'] if len(params['uo_overp_contours']) > 0 else None
                impulse_contours = params['uo_impulse_contours'] if len(params['uo_impulse_contours']) > 0 else None

                mach_convs = {'s0.2': 0.2, 's0.35': 0.35, 's0.7': 0.7, 's1': 1, 's1.4': 1.4, 's2': 2, 's3': 3, 's4': 4, 's5.2': 5.2}
                mach_speed = mach_convs[params['mach_speed']]

                oxmin = params['uo_overp_xmin']
                oxmax = params['uo_overp_xmax']
                oymin = params['uo_overp_ymin']
                oymax = params['uo_overp_ymax']
                ozmin = params['uo_overp_zmin']
                ozmax = params['uo_overp_zmax']
                overp_xlims = None if auto or oxmin is None or oxmax is None or oxmin >= oxmax else (oxmin, oxmax)
                overp_ylims = None if auto or oymin is None or oymax is None or oymin >= oymax else (oymin, oymax)
                overp_zlims = None if auto or ozmin is None or ozmax is None or ozmin >= ozmax else (ozmin, ozmax)

                ixmin = params['uo_impulse_xmin']
                ixmax = params['uo_impulse_xmax']
                iymin = params['uo_impulse_ymin']
                iymax = params['uo_impulse_ymax']
                izmin = params['uo_impulse_zmin']
                izmax = params['uo_impulse_zmax']
                impulse_xlims = None if auto or ixmin is None or ixmax is None or ixmin >= ixmax else (ixmin, ixmax)
                impulse_ylims = None if auto or iymin is None or iymax is None or iymin >= iymax else (iymin, iymax)
                impulse_zlims = None if auto or izmin is None or izmax is None or izmin >= izmax else (izmin, izmax)

                data = phys.compute_overpressure(method=params['overp_method'],
                                                 locations=params['uo_points'],
                                                 ambient_fluid=amb_fluid,
                                                 release_fluid=rel_fluid,
                                                 orifice_diameter=params['leak_d'],
                                                 mass_flow=params['plume_mass_flow'],
                                                 release_angle=params['rel_angle'],
                                                 discharge_coefficient=params['discharge'],
                                                 nozzle_model=params['nozzle'],
                                                 bst_flame_speed=mach_speed,
                                                 tnt_factor=params['tnt_factor'],
                                                 overp_contours=overp_contours,
                                                 overp_xlims=overp_xlims,
                                                 overp_ylims=overp_ylims,
                                                 overp_zlims=overp_zlims,
                                                 impulse_contours=impulse_contours,
                                                 impulse_xlims=impulse_xlims,
                                                 impulse_ylims=impulse_ylims,
                                                 impulse_zlims=impulse_zlims,
                                                 create_overpressure_plot=True, output_dir=output_dir, verbose=verbose
                                                 )
                results['status'] = 1
                results['output_dir'] = output_dir
                results['data'] = data

        for wrng in warning_list:
            if wrng.category is PhysicsWarning:
                results["warning"] = str(wrng.message)

    except TypeError as err:
        proc_log.exception("TypeError occurred during analysis")
        results = dict(status=-1, analysis_id=analysis_id, error=err, message=str(err))
    except ValueError as err:
        proc_log.exception("ValueError occurred during analysis")
        results = dict(status=-1, analysis_id=analysis_id, error=err, message=str(err))
    except Exception as err:
        proc_log.exception("Exception occurred during analysis")
        results = dict(status=-1, analysis_id=analysis_id, error=err, message="Error during analysis - check log for details")

    if results['status'] == -1:
        # analysis failed
        try:
            output_dir.rmdir()  # only removes empty dir
        except Exception:
            pass
        return results

    # check if analysis was canceled
    # if api_settings.RUN_STATUS == api_settings.Status.STOPPED:
    #     results = {
    #         'status': 2,
    #         'analysis_id': analysis_id,
    #         'message': "Analysis canceled successfully"
    #     }
    #     try:
    #         output_dir.rmdir()
    #     except Exception:
    #         pass
    #     return results

    # analysis.save_results(output_dir=output_dir)

    return results


def do_plume_analysis(analysis_id, params: dict, status_dict: dict):
    params['analysis_type'] = 'plume'
    als_data = do_physics_analysis(analysis_id, params, status_dict)

    # postprocess
    status = als_data['status']
    if status == 1:
        # convert to list of contour dicts {contour, stream dist, horiz min dist, horiz max, vert min, vert max}
        mole_frac_dists = als_data['mole_frac_dists']
        streamline_dists = als_data['streamline_dists']

        contour_dicts = []
        ci = 0
        for contour, data in mole_frac_dists.items():
            # keys must match QML
            contour_dicts.append({
                'contour': contour,
                'stream': round(streamline_dists[ci], 2),
                'hmin': round(data[0][0], 2),
                'hmax': round(data[0][1], 2),
                'vmin': round(data[1][0], 2),
                'vmax': round(data[1][1], 2),
            })
            ci += 1

        results = {
            'status': 1,
            'analysis_type': 'plume',
            'analysis_id': analysis_id,
            'plot_fpath': als_data['plot'],
            'mass_flow': als_data['mass_flow_rate'],
            'output_dir': als_data['output_dir'],
            'contour_dicts': contour_dicts
        }

    else:
        results = als_data  # pass through error messages

    return results


def do_accum_analysis(analysis_id, params: dict, status_dict: dict):
    params['analysis_type'] = 'accum'
    als_data = do_physics_analysis(analysis_id, params, status_dict)

    # postprocess
    status = als_data['status']
    if status == 1:
        arrs = np.array([
            params['out_ts'],
            als_data['pressures_per_time'],
            als_data['depths'],
            als_data['concentrations'],
            als_data['mass_flow_rates'],
        ]).T
        data_dicts = []
        for elem in arrs:
            data_dicts.append({
                'time': elem[0],
                'pres': np.format_float_scientific(elem[1], 3),
                'depth': np.format_float_scientific(elem[2], 3),
                'conc': np.around(elem[3], 2),
                'flow': np.format_float_scientific(elem[4], 3),
            })

        results = {
            'status': 1,
            'analysis_type': 'accum',
            'analysis_id': analysis_id,
            'acc_pressure_plot': als_data['pres_plot_filepath'],
            'acc_flam_plot': als_data['mass_plot_filepath'],
            'acc_layer_plot': als_data['layer_plot_filepath'],
            'acc_traj_plot': als_data['trajectory_plot_filepath'],
            'acc_flow_plot': als_data['mass_flow_plot_filepath'],
            'max_overp': als_data['overpressure'] / 1000,
            'overp_t': als_data['time_of_overp'],
            'output_dir': als_data['output_dir'],
            'data_dicts': data_dicts
        }

    else:
        results = als_data  # pass through error messages

    return results


def do_jet_flame_analysis(analysis_id, params: dict, status_dict: dict):
    params['analysis_type'] = 'flame'
    results = do_physics_analysis(analysis_id, params, status_dict)

    # postprocess
    status = results['status']
    if status == 1:
        flux_dicts = []
        flux_results = results['flux_data']
        for i, point in enumerate(params['jet_flame_points']):
            flux_dicts.append(dict(x=point[0], y=point[1], z=point[2], flux=flux_results[i]))

        results |= {
            'analysis_type': 'flame',
            'analysis_id': analysis_id,
            'flux_data': flux_dicts,
        }
    return results


def do_unconf_overp_analysis(analysis_id, params: dict, status_dict: dict):
    params['analysis_type'] = 'uo'
    results = do_physics_analysis(analysis_id, params, status_dict)

    # postprocess
    status = results['status']
    if status == 1:
        results |= {
            'analysis_type': 'uo',
            'analysis_id': analysis_id,
        }
    return results
