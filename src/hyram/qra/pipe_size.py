"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import math


def calc_pipe_inner_diameter(pipe_outer_diameter, pipe_wall_thickness):
    """
    Calculate inner diameter of pipe

    Parameters
    ----------
    pipe_outer_diameter : float
        Outer diameter of pipe

    pipe_wall_thickness : float
        Wall thickness of pipe

    Returns
    -------
    pipe_inner_diameter : float
        Inner diameter of pipe
    """
    pipe_inner_diam = pipe_outer_diameter - 2 * pipe_wall_thickness
    return pipe_inner_diam


def calc_pipe_flow_area(pipe_inner_diameter):
    """
    Calculate flow area of pipe

    Parameters
    ----------
    pipe_inner_diameter : float
        Inner diameter of pipe

    Returns
    -------
    pipe_flow_area : float
        Inner cross-sectional area of pipe
    """
    pipe_flow_area = math.pi * (pipe_inner_diameter / 2) ** 2
    return pipe_flow_area


def calc_orifice_diameter(pipe_flow_area, leak_size_fraction):
    """
    Calculate diameter of circular orifice
    based on a fractional leak size of a pipe flow area

    Parameters
    ----------
    pipe_flow_area : float
        Inner cross-sectional area of pipe

    leak_size_fraction : float
        Leak size in terms of fraction of pipe flow area

    Returns
    -------
    leak_diameter : float
        Diameter of circular leak orifice
    """
    leak_area = pipe_flow_area * leak_size_fraction
    leak_diameter = math.sqrt(4 * leak_area / math.pi)
    return leak_diameter
