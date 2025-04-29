"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import numpy as np


def get_distance_to_effect(value, from_point, direction, effect_func, *args,
                           max_distance=500, interpolation_points=10000,
                           negative_direction=False, **kwargs):
    """
    Calculates the distance from some starting point to a physical effect value of interest

    Parameters
    ----------
    value: float or array-like
        effect value of interest
    from_point: array-like of length 3
        (x, y, z) location of reference point
        from which distance will be measured
    direction: 'x', 'y', or 'z'
        axis along which distance will be calculated
    effect_func: callable
        function/method that will calculate the effect values
        must accept (x_values, y_values, z_values) as arguments
        followed by whatever other arguments are used
    *args: positional arguments (optional)
        if provided, passed to effect_func
    max_distance: float (optional)
        maximum distance from which to calculate distance to effect
        (default value is 500)
    interpolation_points: int (optional)
        number of distance-points at which to calculate effects
        and use for interpolation
        (default value is 10,000)
    negative_direction : Boolean (optional)
        whether or not to look in the negative direction instead of positive
        (default is False)
    **kwargs: keyword arguments, optional
        if provided, passed to effect_func

    Returns
    -------
    distance: float or array-like
        distance to effect value of interest
        calculated from the from_point
        along the direction axis specified
    """
    # TODO: Should this be changed to a solver instead of interpolation? Does 10,000 calculations
    if negative_direction:
        max_distance = -1 * max_distance
    distances = np.linspace(max_distance, 0, interpolation_points)
    if direction == 'x':
        x_values = from_point[0] + distances
        y_values = from_point[1] * np.ones_like(distances)
        z_values = from_point[2] * np.ones_like(distances)
    elif direction == 'y':
        x_values = from_point[0] * np.ones_like(distances)
        y_values = from_point[1] + distances
        z_values = from_point[2] * np.ones_like(distances)
    elif direction == 'z':
        x_values = from_point[0] * np.ones_like(distances)
        y_values = from_point[1] * np.ones_like(distances)
        z_values = from_point[2] + distances
    else:
        raise ValueError(f"Direction ('{direction}') must be 'x', 'y', or 'z'")
    effects = effect_func(x_values, y_values, z_values, *args, **kwargs)
    # effect values must be monotonically increasing
    index_max_value = np.argmax(effects)
    distance = np.interp(value, effects[:index_max_value], distances[:index_max_value])
    return distance
