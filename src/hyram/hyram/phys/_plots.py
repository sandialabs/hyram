"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import copy
import os

import matplotlib.pyplot as plt
from mpl_toolkits.axes_grid1 import ImageGrid
import numpy as np


def plot_sliced_contour(contours, xlims, ylims, zlims, nx, ny, nz,
                        distance_func, value_func, slice_xyz,
                        colorbar_label,
                        origin_lines=None,
                        title=None,
                        savefig=False,
                        directory=os.getcwd(),
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

    ClrMap = copy.copy(plt.cm.get_cmap('RdYlGn_r'))
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
        filepath = os.path.join(directory, filename)
        fig.savefig(filepath, bbox_inches='tight')
        plt.close(fig)
        return filepath
    else:
        return fig
