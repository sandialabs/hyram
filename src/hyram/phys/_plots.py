"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import copy
import os

import numpy as np
import matplotlib as mpl
import matplotlib.pyplot as plt
from matplotlib.ticker import MaxNLocator
from mpl_toolkits.axes_grid1 import ImageGrid

from ._fuel_props import FuelProperties
from ..utilities import misc_utils


def plot_sliced_contour(contours, xlims, ylims, zlims, nx, ny, nz,
                        distance_func, value_func, slice_xyz,
                        colorbar_label,
                        origin_lines=None,
                        title=None,
                        savefig=False,
                        directory=None,
                        filename='sliced_contour_plot.png',
                        *args, **kwargs):
    '''
    Get grid location values for use in sliced contour plot

    Parameters
    ----------
    contours : list-like
        contour levels to show on plot;
        only first value is used to determine furthest contour
    xlims : tuple/list or None
        x-limits for plot
        (if None, will be determined based on furthest contour)
    ylims : tuple/list or None
        y-limits for plot
        (if None, will be determined based on furthest contour)
    zlims : tuple/list or None
        z-limits for plot
        (if None, will be determined based on furthest contour)
    nx : int
        number of points to solve for in the x-direction
    ny : int
        number of points to solve for in the y-direction
    nz : int
        number of points to solve for in the z-direction
    distance_func : callable
        function/method that will calculate the distance to furthest contour;
        must accept an effect value as the first argument argument,
        a keyword argument of direction,
        followed by whatever other arguments are used
    value_func : callable
        function/method that will calculate values for contour plot;
        must accept list-likes of x-values, y-values, and z-values
        as the first three arguments,
        followed by whatever other arguments are used
    slice_xyz : list of 3 values
        The (x, y, z) coordinate of where the 3 slices are located
    colorbar_label : string
        label for colorbar
    origin_lines : list of 3 list-like (optional)
        if provided, this is a list of values to plot black lines
        [x-values, y-values, z-values]
    title : string (optional)
        if provided, title shown on plot
    savefig : Boolean (optional)
        determines if figure file is saved
    directory : string (optional)
        directory in which to save file
        defaults to current working directory
    filename : string (optional)
        file name to write
    *args : positional arguments (optional)
        if provided, passed to distance_func
    **kwargs : keyword arguments, optional
        if provided, passed to distance_func

    Returns
    -------
    fig_or_filepath: matplotlib.pyplot.Figure object or string
        If savefig is True, returns filename of the corresponding plot.
        If savefig is false, returns fig object.
    '''
    furthest_contour = contours[0] * 1000  # convert from kilo
    default_scaling = 1.1  # default scaling is 10% past furthest contour

    if xlims is None:
        pos_x_distance_to_contour = distance_func(furthest_contour,
                                                  direction='x',
                                                  *args, **kwargs)
        neg_x_distance_to_contour = distance_func(furthest_contour,
                                                  direction='x',
                                                  negative_direction=True,
                                                  *args, **kwargs)
        padded_pos_x_distance = default_scaling * pos_x_distance_to_contour
        padded_neg_x_distance = default_scaling * neg_x_distance_to_contour
        dx = (padded_pos_x_distance - padded_neg_x_distance) / nx
        x0 = slice(padded_neg_x_distance, padded_pos_x_distance, dx)
    else:
        dx = (xlims[1] - xlims[0]) / nx
        x0 = slice(xlims[0], xlims[1], dx)

    if ylims is None:
        y_distance_to_contour = distance_func(furthest_contour,
                                              direction='y',
                                              *args, **kwargs)
        padded_y_distance = default_scaling * y_distance_to_contour
        dy = padded_y_distance / ny
        y0 = slice(0, padded_y_distance, dy)
    else:
        dy = (ylims[1] - ylims[0]) / ny
        y0 = slice(ylims[0], ylims[1], dy)

    if zlims is None:
        z_distance_to_contour = distance_func(furthest_contour,
                                              direction='z',
                                              *args, **kwargs)
        padded_z_distance = default_scaling * z_distance_to_contour
        dz = 2 * padded_z_distance / nz
        z0 = slice(-padded_z_distance, padded_z_distance, dz)
    else:
        dz = (zlims[1] - zlims[0]) / nz
        z0 = slice(zlims[0], zlims[1], dz)

    x_z, y_z = np.mgrid[x0, y0]
    x_y, z_y = np.mgrid[x0, z0]
    y_x, z_x = np.mgrid[y0, z0]

    fxy = value_func(x_z, y_z, slice_xyz[2] * np.ones_like(x_z), *args, **kwargs)
    fxz = value_func(x_y, slice_xyz[1] * np.ones_like(x_y), z_y, *args, **kwargs)
    fzy = value_func(slice_xyz[0] * np.ones_like(z_x), y_x, z_x, *args, **kwargs)
    fxy = fxy / 1000  # convert to kilo
    fxz = fxz / 1000
    fzy = fzy / 1000

    fig = plt.figure(figsize=(8, 4.5))
    # Not sure if this will always be right; tight_layout incompatible with ImageGrid
    fig.subplots_adjust(top=0.967,
                        bottom=0.378)
    grid = ImageGrid(fig, 111,  # similar to subplot(111)
                     nrows_ncols=(2, 2),  # creates 2x2 grid of axes
                     axes_pad=0.1,  # pad between axes in inches.
                     label_mode="L",
                     cbar_mode='edge',
                     cbar_location='bottom',
                     cbar_size='10%',
                     cbar_pad=-1.5)
    ax_xy, ax_zy, ax_xz, ax_cb = grid[0], grid[1], grid[2], grid[3]

    for ax in [ax_xy, ax_zy, ax_xz]:
        ax.cax.set_visible(False)
        ax.minorticks_on()
        ax.grid(alpha=0.2, color='k')
        ax.grid(which='minor', alpha=0.1, color='k')
        ax.set_aspect(1)
    ax_zy.axis['bottom'].toggle(all=True)
    ax_cb.set_frame_on(False)
    ax_cb.set_axis_off()
    ax_cb.cax.set_visible(True)

    ClrMap = copy.copy(mpl.colormaps['RdYlGn_r'])
    ClrMap.set_under('white')

    ax_xy.contourf(x_z, y_z, fxy, cmap=ClrMap,
                    levels=contours, extend='both')
    if origin_lines is not None:
        ax_xy.plot(origin_lines[0], origin_lines[1],
                   color='k', linewidth=3, solid_capstyle='round')
    ax_xy.set_ylabel('Height (y) [m]')
    ax_xy.annotate(f'z = {slice_xyz[2]:.2f} m', xy=(0.02, 0.98),
                   xycoords='axes fraction', va='top', color='k')

    ax_xz.contourf(x_y, z_y, fxz, cmap=ClrMap,
                    levels=contours, extend='both')
    if origin_lines is not None:
        ax_xz.plot(origin_lines[0], origin_lines[2],
                   color='k', linewidth=3, solid_capstyle='round')
    ax_xz.set_xlabel('Horizontal Distance (x) [m]')
    ax_xz.set_ylabel('Perpendicular Distance (z) [m]')
    ax_xz.annotate(f'y = {slice_xyz[1]:.2f} m', xy=(0.02, 0.98),
                   xycoords='axes fraction', va='top', color='k')

    im = ax_zy.contourf(z_x, y_x, fzy, cmap=ClrMap,
                        levels=contours, extend='both')
    if origin_lines is not None:
        ax_zy.plot(origin_lines[2], origin_lines[1],
                   color='k', linewidth=3, solid_capstyle='round')
    ax_zy.set_xlabel('Perpendicular Distance (z) [m]')
    ax_zy.annotate(f'x = {slice_xyz[0]:.2f} m', xy=(0.02, 0.98),
                   xycoords='axes fraction', va='top', color='k')

    cb = plt.colorbar(im, cax=ax_cb.cax, orientation='horizontal', extendfrac='auto')
    cb.set_label(colorbar_label)

    if title is not None:
        fig.suptitle(title)

    if savefig:
        output_folder = misc_utils.get_output_folder(directory)
        filepath = os.path.join(output_folder, filename)
        fig.savefig(filepath, bbox_inches='tight')
        plt.close(fig)
        return filepath
    else:
        return fig


def plot_contour(data_type, jet_or_flame, mcolors='w',
                 xlims=None, ylims=None, vlims=None,
                 contour_levels=None, add_colorbar=True, plot_color=None,
                 plot_title=None, x_label=None, y_label=None,
                 fig_params=None, plot_params=None, subplot_params=None, fig=None, ax=None):
    '''
    Creates contour plot.
    Currently handles temperature, mole fraction, mass fraction, and velocity curves. Specify type using the 'data_type' parameter.

    Parameters
    ----------
    data_type: str
        Specifies the type of plot (controls labels and default colors). Options are 'temperature', 'mole', 'mass', and 'velocity'
    jet_or_flame: object
        Jet or Flame object
    mcolors: color or list of colors, optional
        Color(s) of marked contour levels
    xlims: tuple(float or None) or None, optional
        X-axis boundaries of (xmin, xmax) for contour plot
    ylims: tuple(float or None) or None, optional
        Y-axis boundaries of (ymin, ymax) for contour plot
    vlims: tuple(float or None) or None, optional
        Value (z-axis) boundaries of (vmin, vmax) for contour plot
    contour_levels: int or list, optional
        Contour levels to mark and label
    add_colorbar: boolean, optional
        Add a colorbar to the plot. Defaults to True
    plot_color: str, optional
        Matplotlib Colormap name to use for the contour plot. Default colors are set by the different types of plot
    plot_title: str, optional
        Title text for the plot
    x_label: str, optional
        X-axis label for the plot
    y_label: str, optional
        Y-axis label for the plot
    fig_params: dict or None, optional
        Dictionary of figure parameters (e.g. figsize)
    plot_params: dict or None, optional
        Dictionary of contour plot parameters (e.g. colors)
    subplot_params: dict or None, optional
        Dictionary of subplots_adjust parameters (e.g. top)
    fig: matplotlib Figure, optional
        Figure on which to make the plot, used if figure already exists before this function is called
    ax: matplotlib Axes, optional
        Axes on which to make the plot, used if figure already exists

    Returns
    -------
    fig: matplotlib Figure
        Figure containing the contour plot
    '''
    # Set params to dicts
    if fig_params is None:
        fig_params = {}
    if plot_params is None:
        plot_params = {}
    if subplot_params is None:
        subplot_params = {}

    # Make figure and axis if not specified
    if ax is None and fig is None:
        fig, ax = plt.subplots(**fig_params)
        plt.subplots_adjust(**subplot_params)
    elif ax is None and fig is not None:
        ax = fig.subplots(**fig_params)
        plt.subplots_adjust(**subplot_params)
    else:
        fig = ax.figure

    # Format plot
    ax.set_aspect('equal')
    if xlims is not None:
        ax.set_xlim(*xlims)
    if ylims is not None:
        ax.set_ylim(*ylims)
    if plot_title is not None:
        ax.set_title(plot_title)

    if x_label is not None:
        ax.set_xlabel(x_label)
    else:
        ax.set_xlabel('x (m)')

    if y_label is not None:
        ax.set_ylabel(y_label)
    else:
        ax.set_ylabel('y (m)')

    # Contour plot parameters based on type selection
    if data_type == "temperature":
        x, y, v = jet_or_flame._contourdata
        cb_label = 'Temperature (K)'
        if not plot_color:
            plot_color = 'plasma'

    elif data_type == "mole":
        x, y, v, __, __, __ = jet_or_flame._contourdata
        cb_label = 'Mole Fraction'
        if not plot_color:
            plot_color = 'viridis'

    elif data_type == "mass":
        x, y, __, v, __, __ = jet_or_flame._contourdata
        cb_label = 'Mass Fraction'
        if not plot_color:
            plot_color = 'viridis'

    elif data_type == "velocity":
        x, y, __, __, v, __ = jet_or_flame._contourdata
        cb_label = 'Velocity (m/s)'
        if not plot_color:
            plot_color = 'viridis'

    else:
        raise ValueError('Invalid datatype')

    # Adjust v-limits for filled contour colors
    if vlims is not None:
        vmin = vlims[0]
        vmax = vlims[1]
        if vmin is None and vmax is not None:
            if np.amax(v) > vmax:
                extend = 'max'
            else:
                extend = 'neither'
            vmin = np.amin(v)
        elif vmax is None and vmin is not None:
            if np.amin(v) < vmin:
                extend = 'min'
            else:
                extend = 'neither'
            vmax = np.amax(v)
        else:
            if np.amax(v) > vmax and np.amin(v) < vmin:
                extend = 'both'
            elif np.amax(v) > vmax and np.amin(v) >= vmin:
                extend = 'max'
            elif np.amax(v) <= vmax and np.amin(v) < vmin:
                extend = 'min'
            else:
                extend = 'neither'
    else:
        vmin = np.amin(v)
        vmax = np.amax(v)
        extend = 'neither'

    filled_levels = np.linspace(vmin, vmax, 101)

    # Filled contour and background
    plt.set_cmap(plot_color)
    if hasattr(mpl, 'colormaps'):
        ax.set_facecolor(mpl.colormaps[plot_color](0))
    else:
        ax.set_facecolor(mpl.cm.get_cmap(plot_color)(0))

    filled_contour = ax.contourf(x, y, v, levels=filled_levels, vmin=vmin, vmax=vmax, extend=extend, **plot_params)

    # Add specific line contours
    if contour_levels is not None:
        contour_levels = [contour_levels] if type(contour_levels) in [int, float] else contour_levels

        # Check for values outside of range
        if data_type == "mole":
            contour_levels = [FuelProperties(jet_or_flame.fluid.species).LFL if value == 'LFL' else value for value in contour_levels]
            contour_levels = [FuelProperties(jet_or_flame.fluid.species).UFL if value == 'UFL' else value for value in contour_levels]

            for contour in contour_levels:
                if contour <= 0 or contour >= 1:
                    error_msg = (f'Mole fraction contour values must be >0 and <1 (current value: {contour})')
                    raise ValueError(error_msg)

        # Draw contour lines
        CS = ax.contour(x, y, v, levels=np.sort(contour_levels), colors=mcolors, linewidths=1.5, **plot_params)

        # Draw contour line labels
        if xlims is not None and ylims is not None:
            manual = [np.interp(m, jet_or_flame.X_cl[::-1],
                                jet_or_flame.S[::-1]) * np.array([np.cos(jet_or_flame.theta[0]),
                                                                    np.sin(jet_or_flame.theta[0])])*.7 for m in contour_levels]
        else:
            manual = False

        ax.clabel(CS, levels=CS.levels, inline=True, manual=manual, inline_spacing=2)

    # Add color bar
    if add_colorbar:
        cb_kwargs = {}
        cb_label_kwargs = {}

        # Automatically set colorbar orientation
        if xlims:
            width = np.abs(xlims[1] - xlims[0])
        else:
            lims = ax.get_xlim()
            width = np.abs(lims[1] - lims[0])

        if ylims:
            height = np.abs(ylims[1] - ylims[0])
        else:
            lims = ax.get_ylim()
            height = np.abs(lims[1] - lims[0])

        if width <= height:
            cb_kwargs = {'orientation':'vertical'}
            cb_label_kwargs = {'rotation':-90, 'va':'bottom'}
        else:
            cb_kwargs = {'orientation':'horizontal'}

        if vmin and vmax:
            colorbar = plt.colorbar(filled_contour, ticks=MaxNLocator().tick_values(vmin, vmax), **cb_kwargs)
        else:
            colorbar = plt.colorbar(filled_contour, **cb_kwargs)

        colorbar.set_label(cb_label, **cb_label_kwargs)

    ax.set_aspect(1)
    fig.tight_layout()

    return fig
