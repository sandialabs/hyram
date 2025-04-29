"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os

import numpy as np
import matplotlib.pyplot as plt

from ..phys import _flame, _jet, _unconfined_overpressure
from ..utilities import misc_utils


def calc_thermal_effects(ambient_fluid, release_fluid, release_angle,
                         orifice, rel_humid,
                         notional_nozzle_model,
                         locations, leak_idx,
                         developing_flow=None, chem=None,
                         create_plots=True, output_dir=None, verbose=False):
    """
    Calculates thermal effects for all positions in QRA

    Parameters
    ----------
    ambient_fluid : Fluid object
        Ambient fluid specification

    release_fluid : Fluid object
        Release fluid specification

    release_angle: float
        angle of release (0 is horizontal) (radians)

    orifice : Orifice object
        Single leak orifice object to perform calculations on

    rel_humid : float
        Relative humidity between 0 and 1

    notional_nozzle_model : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    locations : list of tuples
        List of locations at which to determine thermal effects,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]

    leak_idx : int
        Index number of leak size

    developing_flow : DevelopingFlow object
        Specified DevelopingFlow for Leak

    chem : Combustion object
        Specified Combustion chemistry

    create_plots : bool
        Whether plot files should be created

    output_dir : str
        File path to directory in which to create files and plots

    verbose : bool
        If True, extra output will be printed (default False)

    Returns
    -------
    fluxes : ndarray
        [W/m2] Heat flux for all positions for the given leak size

    plot_filepath : str
        Position plot file path
    """
    cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(
        notional_nozzle_model, release_fluid)

    if developing_flow is None:
        flame = _flame.Flame(release_fluid, orifice, ambient_fluid,
                             theta0=release_angle,
                             nn_conserve_momentum=cons_momentum,
                             nn_T=notional_noz_t,
                             chem=chem,
                             verbose=verbose)
    else:
        flame = _flame.Flame.from_developed_flow(
            developing_flow=developing_flow,
            chem = chem,
            verbose=verbose)

    fluxes = flame.generate_positional_flux(locations, rel_humid)

    plot_filepath = ""
    if create_plots:
        x_locations = [location[0] for location in locations]
        z_locations = [location[2] for location in locations]
        fluxes_kWm2 = fluxes / 1000
        fluxes_str = 'Radiative Heat Flux (kW/m$^2$)'
        orif_diam_mm = orifice.d * 1000
        output_folder = misc_utils.get_output_folder(output_dir)
        now_str = misc_utils.get_now_str()
        pos_fname = 'HeatFluxPositionPlot{}_{}.png'.format(leak_idx, now_str)
        plot_filepath = os.path.join(output_folder, pos_fname)
        pos_title = '{} mm Leak Size'.format(round(orif_diam_mm, 3))
        plot_effect_positions(fluxes_kWm2, fluxes_str,
                            plot_filepath, pos_title,
                            x_locations, z_locations)

    return fluxes, plot_filepath


def calc_overp_effects(orifice, notional_nozzle_model,
                       release_fluid, ambient_fluid, release_angle,
                       locations, overp_method, leak_idx,
                       BST_mach_flame_speed=None, TNT_equivalence_factor=None,
                       developing_flow=None,
                       create_plots=True, output_dir=None,
                       verbose=False):
    """
    Calculates overpressure effects for all positions in QRA

    Parameters
    ----------
    orifice : Orifice object
        Single leak orifice object to perform calculations on

    notional_nozzle_model : {'yuce', 'ewan', 'birc', 'bir2', 'molk'}
        Notional nozzle model identifier (i.e. for under-expanded jet zone)

    release_fluid : Fluid object
        Release fluid specification

    ambient_fluid : Fluid object
        Ambient fluid specification

    release_angle : float
        Angle of release (0 is horizontal) (radians)

    locations : list of locations
        List of locations at which to determine overpressure effects,
        each location is a tuple of 3 coordinates (m):
        [(x1, y1, z1), (x2, y2, z2), ...]

    overp_method : {'bst', 'tnt', 'bauwens'}
        Overpressure harm model identifier

    leak_idx : int
        Index number of leak size

    developing_flow : DevelopingFlow object
        Specified DevelopingFlow for leak

    BST_mach_flame_speed : float
        If overp_method is 'bst', this must be specified
        Mach flame speed:
        0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2
        Detonation should use 5.2

    TNT_equivalence_factor : float
        If overp_method is 'tnt', this must be specified
        TNT equivalency factor

    create_plots : bool
        Whether plot file should be created
        Default is True

    output_dir : str
        File path to directory in which to create files and plots
        Default is None, which will use current working directory

    verbose : bool
        If True, extra output will be printed (default is False)

    Returns
    -------
    overpressures : ndarray
        [Pa] Peak overpressure results for each position for the given leak

    impulses : ndarray
        [Pa*s] Impulse results for each position for the given leak
        Note: impulses are np.nan if impulse does not exist for specified model

    overpressure_plot_filepath : str
        Position plot file path for peak overpressure by position

    impulse_plot_filepath : str
        Position plot file paths for impulse by position
    """
    nozzle_cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(notional_nozzle_model, release_fluid)

    if developing_flow is None:
        jet = _jet.Jet(release_fluid, orifice, ambient_fluid,
                    theta0=release_angle,
                    nn_conserve_momentum=nozzle_cons_momentum, nn_T=notional_noz_t,
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

    overpressure_plot_filepath = ""
    impulse_plot_filepath = ""
    if create_plots:
        x_locations = [location[0] for location in locations]
        z_locations = [location[2] for location in locations]
        output_folder = misc_utils.get_output_folder(output_dir)
        overpressures_kPa = overpressures / 1000
        overpressures_str = 'Peak Overpressure (kPa)'
        now_str = misc_utils.get_now_str()
        orif_diam_mm = orifice.d * 1000  # mm
        pos_fname = 'OverpressurePositionPlot{}_{}.png'.format(leak_idx, now_str)
        overpressure_plot_filepath = os.path.join(output_folder, pos_fname)
        pos_title = '{} mm Leak Size'.format(round(orif_diam_mm, 3))
        plot_effect_positions(overpressures_kPa, overpressures_str,
                              overpressure_plot_filepath, pos_title,
                              x_locations, z_locations)

        if not np.isnan(impulses).any():
            impulses_kPas = impulses / 1000
            impulses_str = 'Impulse (kPa*s)'
            now_str = misc_utils.get_now_str()
            pos_fname = 'ImpulsePositionPlot{}_{}.png'.format(leak_idx, now_str)
            impulse_plot_filepath = os.path.join(output_folder, pos_fname)
            plot_effect_positions(impulses_kPas, impulses_str,
                                  impulse_plot_filepath, pos_title,
                                  x_locations, z_locations)

    return overpressures, impulses, overpressure_plot_filepath, impulse_plot_filepath


def plot_effect_positions(effects, effect_label, filename, title,
                          x_locations, z_locations):
    """
    Plots positions colored by effect
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
    """
    fig, ax = plt.subplots()

    # Blue square represents leak/release source
    ax.plot(0, 0, 'bs', markersize=8, label='Leak Source')

    # Arrow representing flame direction, does not represent length/width of leak
    if len(x_locations) == 0:
        max_x_distance = 4
    else:
        max_x_distance = max(x_locations)
    ax.arrow(0, 0, max_x_distance/4, 0, head_width=0.7, alpha=1, head_length=0.7, linewidth=0.3, fc='blue', ec='blue')

    # Plot occupant positions, colored by effect
    effect_ln = ax.scatter(x_locations, z_locations, s=36, c=effects, linewidths=0.5, edgecolors='black', cmap = plt.get_cmap('plasma'))

    # Plot formatting, colorbar, and saving
    ax.set_aspect('equal')
    ax.set_xlabel('X (m)')
    ax.set_ylabel('Z (m)')
    ax.set_title(title)
    cb = fig.colorbar(effect_ln)
    cb.set_label(effect_label)
    fig.savefig(filename, bbox_inches='tight')
    plt.close(fig)
