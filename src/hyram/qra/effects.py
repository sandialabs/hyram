"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os

import numpy as np
import matplotlib.pyplot as plt
import matplotlib.patches as mplpatch

from ..phys import _flame, _jet, _unconfined_overpressure
from ..utilities import misc_utils


def calc_thermal_effects(amb_fluid, rel_fluid, rel_angle,
                         site_length, site_width,
                         orifices, rel_humid,
                         not_nozzle_model,
                         locations, developing_flows = None, chem = None,
                         create_plots=True, output_dir=None, verbose=False):
    """
    Calculates thermal effects for all positions in QRA

    Parameters
    ----------
    amb_fluid : Fluid object
        Ambient fluid

    rel_fluid : Fluid object
        Release fluid

    rel_angle : float
        angle of release (0 is horizontal) (radians)

    site_length : float
        Facility length (m)

    site_width : float
        Facility width (m)

    orifices : list of Orifice objects
        Leak orifices objects to perform calculations on
        One for each leak size

    rel_humid : float
        Relative humidity between 0 and 1

    not_nozzle_model : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    locations : list of tuples
        List of locations at which to determine thermal effects,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]
        
    developing_flows : DevelopingFlow objects
        Specified DevelopingFlows - same length as orifices
        
    chem : Combustion object
        Specified Combustion chemistry

    create_plots : bool
        Whether plot files should be created

    output_dir : str
        file path to directory in which to create files and plots

    verbose : bool
        If True, extra output will be printed (default False)

    Returns : dict
    -------
        fluxes : ndarray
            [W/m2] Heat flux data for all positions and leaksizes
            1-D array, all leaksizes for location 1,
            then all leaksizes location 2, etc.

        all_pos_files : list of str
            position plot file paths
    """
    num_sizes = len(orifices)
    num_positions = len(locations)
    x_locations = [location[0] for location in locations]
    z_locations = [location[2] for location in locations]
    cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(not_nozzle_model, rel_fluid)

    all_qrads = np.zeros((num_sizes, num_positions))
    all_pos_filepaths = []
    if developing_flows is None:
        developing_flows = len(orifices)*[None]
    for i, (orifice, developing_flow) in enumerate(zip(orifices, developing_flows)):
        if developing_flow is None:
            flame = _flame.Flame(rel_fluid, orifice, amb_fluid,
                                 theta0=rel_angle,
                                 nn_conserve_momentum=cons_momentum, nn_T=notional_noz_t,
                                 developing_flow=developing_flow, chem = chem,
                                 verbose=verbose)
        else:
            flame = _flame.Flame.from_developed_flow(developing_flow=developing_flow, chem = chem,
                                                     verbose=verbose)

        fluxes = flame.generate_positional_flux(locations, rel_humid)

        # Each row is the heatflux for all locs for specific leak size
        all_qrads[i, :] = fluxes

        if create_plots:
            fluxes_kWm2 = fluxes / 1000
            fluxes_str = 'Radiative Heat Flux (kW/m$^2$)'
            now_str = misc_utils.get_now_str()
            orif_diam_mm = orifice.d * 1000
            pos_fname = 'HeatFluxPositionPlot{}_{}.png'.format(i, now_str)
            if output_dir is None:
                output_dir = misc_utils.get_temp_folder()
            plot_filepath = os.path.join(output_dir, pos_fname)
            pos_title = '{} mm Leak Size'.format(round(orif_diam_mm, 3))
            plot_effect_positions(fluxes_kWm2, fluxes_str,
                                  plot_filepath, pos_title,
                                  x_locations, z_locations,
                                  site_length, site_width)
            all_pos_filepaths.append(plot_filepath)

    # Flatten heatflux (all leaksize loc1, then all leaksize loc2, etc)
    # Corresponds to flattening by row which is C-style ordering
    qrads_flat = all_qrads.flatten(order='C')

    result_dict = {
        "fluxes": qrads_flat,
        "all_pos_files": all_pos_filepaths
    }

    return result_dict


def calc_overp_effects(orifices, notional_nozzle_model,
                       release_fluid, ambient_fluid, release_angle,
                       locations, site_length, site_width,
                       overp_method,
                       BST_mach_flame_speed=None, TNT_equivalence_factor=None, 
                       developing_flows = None,
                       create_plots=True, output_dir=None,
                       verbose=False):
    """
    Calculates overpressure effects for all positions in QRA

    Parameters
    ----------
    orifices : list of Orifice objects
        Leak orifices objects to perform calculations on
        One for each leak size

    notional_nozzle_model : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    release_fluid : Fluid object
        Release fluid

    ambient_fluid : Fluid object
        Ambient fluid

    release_angle : float
        Angle of release (0 is horizontal) (radians)

    locations : list of locations
        List of locations at which to determine overpressure effects,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]

    site_length : float
        Facility length (m)

    site_width : float
        Facility width (m)
    
    overp_method : {'bst', 'tnt', 'bauwens'}
        Overpressure harm model identifier

    developing_flows : DevelopingFlow objects
        Specified DevelopingFlow - same length as orifices

    BST_mach_flame_speed : float
        If overp_method is 'bst', this must be specified
        Mach flame speed:
        0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2
        Detonation should use 5.2

    TNT_equivalence_factor : float
        If overp_method is 'tnt', this must be specified
        TNT equivalency factor

    create_plots : bool
        Whether plot files should be created
        Default is True

    output_dir : str
        File path to directory in which to create files and plots
        Default is None, which will use a temporary directory
        in the current working directory

    verbose : bool
        If True, extra output will be printed (default is False)

    Returns : dict
    -------
        overpressures : ndarray
            [Pa] Peak overpressure results for each position and leak size
            1-D array, all leaksizes for location 1,
            then all leaksizes location 2, etc.

        impulses : ndarray
            [Pa*s] Impulse results for each position and leak size
            1-D array, all leaksizes for location 1,
            then all leaksizes location 2, etc.
            Note: impulses are np.nan if impulse does not exist
            for specified model

        all_pos_overp_files : list of str
            position plot file paths for peak overpressure by position

        all_pos_impulse_files : list of str
            position plot file paths for impulse by position
    """
    num_sizes = len(orifices)
    num_positions = len(locations)
    x_locations = [location[0] for location in locations]
    z_locations = [location[2] for location in locations]

    all_overpressures = np.zeros((num_sizes, num_positions))
    all_impulses = np.zeros((num_sizes, num_positions))
    all_pos_overp_filepaths = []
    all_pos_impulse_filepaths = []
    if developing_flows is None:
        developing_flows = len(orifices)*[None]
    for i, (orifice, developing_flow) in enumerate(zip(orifices, developing_flows)):
        nozzle_cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(notional_nozzle_model, release_fluid)
        if developing_flow is None:
            jet = _jet.Jet(release_fluid, orifice, ambient_fluid,
                        theta0=release_angle,
                        nn_conserve_momentum=nozzle_cons_momentum, nn_T=notional_noz_t, developing_flow=developing_flow, 
                        verbose=verbose)
        else:
            jet = _jet.Jet.from_developed_flow(developing_flow=developing_flow, verbose=verbose)

        method = overp_method.lower()
        if method == 'bst':
            over_pressure_model = _unconfined_overpressure.BST_method(jet_object=jet,
                                                                      mach_flame_speed=BST_mach_flame_speed)
        elif method == 'tnt':
            over_pressure_model = _unconfined_overpressure.TNT_method(jet_object=jet,
                                                                      equivalence_factor=TNT_equivalence_factor)
        elif method == 'bauwens':
            over_pressure_model = _unconfined_overpressure.Bauwens_method(jet_object=jet)
        else:
            raise ValueError('Invalid overpressure method name')

        overpressures = over_pressure_model.calc_overpressure(locations)
        impulses = over_pressure_model.calc_impulse(locations)
        all_overpressures[i, :] = overpressures
        all_impulses[i, :] = impulses

        if create_plots:
            overpressures_kPa = overpressures / 1000
            overpressures_str = 'Peak Overpressure (kPa)'
            now_str = misc_utils.get_now_str()
            orif_diam_mm = orifice.d * 1000  # mm
            pos_fname = 'OverpressurePositionPlot{}_{}.png'.format(i, now_str)
            if output_dir is None:
                output_dir = misc_utils.get_temp_folder()
            plot_filepath = os.path.join(output_dir, pos_fname)
            pos_title = '{} mm Leak Size'.format(round(orif_diam_mm, 3))
            plot_effect_positions(overpressures_kPa, overpressures_str,
                                  plot_filepath, pos_title,
                                  x_locations, z_locations,
                                  site_length, site_width)
            all_pos_overp_filepaths.append(plot_filepath)

            if not np.isnan(impulses).any():
                impulses_kPas = impulses / 1000
                impulses_str = 'Impulse (kPa*s)'
                now_str = misc_utils.get_now_str()
                pos_fname = 'ImpulsePositionPlot{}_{}.png'.format(i, now_str)
                plot_filepath = os.path.join(output_dir, pos_fname)
                plot_effect_positions(impulses_kPas, impulses_str,
                                      plot_filepath, pos_title,
                                      x_locations, z_locations,
                                      site_length, site_width)
                all_pos_impulse_filepaths.append(plot_filepath)


    # Flatten overpressures and impulses
    # (all leaksize loc1, then all leaksize loc2, etc)
    # Corresponds to flattening by row which is C-style ordering
    all_overpressures_flat = all_overpressures.flatten(order='C')
    all_impulses_flat = all_impulses.flatten(order='C')

    result_dict = {
        'overpressures': all_overpressures_flat, 
        'impulses': all_impulses_flat,
        'all_pos_overp_files': all_pos_overp_filepaths,
        'all_pos_impulse_files': all_pos_impulse_filepaths
    }
    return result_dict


def plot_effect_positions(effects, effect_label, filename, title,
                          x_locations, z_locations, length, width):
    """
    Plots positions on facility grid colored by effect
    (e.g., heat flux, peak overpressure, or impulse)

    Parameters
    ----------
    effects : array
        Effect values at each position for coloring points
    
    effect_label : string
        Text label for the effect being plotted

    filename : string
        filename for saving plot

    title : string
        title for plot

    x_locations : array
        x-dimensions of the locations to plot (m)

    z_locations : array
        z-dimensions of the locations to plot (m)

    length : float
        Facility length (m)
    """
    fig, ax = plt.subplots()

    # Blue square represents leak/release source
    ax.plot(0, 0, 'bs', markersize=8, label='Leak Source')

    # Plot facility border
    ax.add_patch(mplpatch.Rectangle((-.2, -.2), length+.2, width+.2, fill=None))

    # Arrow representing flame direction, does not represent length/width
    ax.arrow(0, 0, length/4, 0, head_width=0.7, alpha=1, head_length=0.7, linewidth=0.3, fc='blue', ec='blue')

    # Plot occupant positions, colored by effect
    effect_ln = ax.scatter(x_locations, z_locations, s=36, c=effects, linewidths=0.5, edgecolors='black', cmap = plt.get_cmap('plasma'))

    # Plot formatting, colorbar, and saving
    ax.set_aspect('equal') 
    for spine in ['top', 'bottom', 'left', 'right']:
        ax.spines[spine].set_visible(False)
    ax.set_xlabel('Length (m)')
    ax.set_ylabel('Width (m)')
    ax.set_title(title)
    cb = fig.colorbar(effect_ln)
    cb.set_label(effect_label)
    fig.savefig(filename, bbox_inches='tight')
    plt.close(fig)
