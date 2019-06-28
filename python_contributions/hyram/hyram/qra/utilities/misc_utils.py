#  Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
#  Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
#  .
#  This file is part of HyRAM (Hydrogen Risk Assessment Models).
#  .
#  HyRAM is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#  .
#  HyRAM is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#  .
#  You should have received a copy of the GNU General Public License
#  along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

import re

import numpy as np

def get_or_error(data_dict, key, msg):
    """ Retrieve element from dict or raise error if not found. """
    if key in data_dict:
        return data_dict[key]
    else:
        raise AttributeError(msg)


def convert_ign_prob_lists_to_dicts(immed_ign_probs, delayed_ign_probs, thresholds):
    """
    Convert lists of ignition data into list of dicts with one entry per rate group.

    Parameters
    ----------
    immed_ign_probs : list
        Immediate ignition probabilities for each group
    delayed_ign_probs : list
        Delayed ignition probabilities for each group
    thresholds : list
        Ignition release rate thresholds for each group (kg/s). floats.

    Returns
    -------
    ign_dict : list of dicts
        Each entry is: {threshold_min, threshold_max, immed_prob, delay_prob}

    """
    num_groups = len(immed_ign_probs)
    ign_dicts = []

    for i in range(num_groups):
        if i == 0:
            thres_min = -np.inf
            thres_max = thresholds[i]

        elif i == (num_groups - 1):
            thres_min = thresholds[-1]
            thres_max = np.inf

        else:
            thres_min = thresholds[i-1]
            thres_max = thresholds[i]

        immed_prob = immed_ign_probs[i]
        delay_prob = delayed_ign_probs[i]

        ign_dicts.append({
            'threshold_min': thres_min,
            'threshold_max': thres_max,
            'immed_prob': immed_prob,
            'delay_prob': delay_prob,
        })

    return ign_dicts


def convert_component_prob_lists_to_dicts(leak_prob_sets):
    """
    Convert probability sets for single component to lists of dicts. Inner lists become {mu, sigma, mean, var}.
    Note that Python.NET converter func can't currently convert nullable list (e.g. double?) to numpy array.
    If mu/sigma or mean/variance are null in C#, they're set to -1000D.

    Parameters
    ----------
    leak_prob_sets : list
    List of lists of probability data. Inner list is ordered: mu, sigma, mean, variance.

    Returns
    -------
    prob_dicts : list
    List of dicts where inner list is dict. {mu, sigma, mean, variance}

    """
    prob_dicts = []
    if type(leak_prob_sets) == np.ndarray:
        leak_prob_sets = list(leak_prob_sets)

    for prob_set in leak_prob_sets:
        mu, sigma, mean, variance = prob_set
        if mu == -1000. or sigma == -1000.:
            mu = None
            sigma = None
        else:
            mean = None
            variance = None

        prob_dict = {
            'mu': mu,
            'sigma': sigma,
            'mean': mean,
            'variance': variance,
        }
        prob_dicts.append(prob_dict)

    return prob_dicts


def clean_name(name):
    """
    Convert string name to alphanumeric lower-case

    """
    parsed = re.sub(r'\W+', '', name.lower())
    return parsed
