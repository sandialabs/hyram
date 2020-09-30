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

import json

import numpy as np
import ctypes
import clr
import System
from System import Array, Int32
from System.Runtime.InteropServices import GCHandle, GCHandleType

_MAP_NP_NET = {
    np.dtype('float32'): System.Single,
    np.dtype('float64'): System.Double,
    np.dtype('int8'): System.SByte,
    np.dtype('int16'): System.Int16,
    np.dtype('int32'): System.Int32,
    np.dtype('int64'): System.Int64,
    np.dtype('uint8'): System.Byte,
    np.dtype('uint16'): System.UInt16,
    np.dtype('uint32'): System.UInt32,
    np.dtype('uint64'): System.UInt64,
    np.dtype('bool'): System.Boolean,
}
_MAP_NET_NP = {
    'Single': np.dtype('float32'),
    'Double': np.dtype('float64'),
    'SByte': np.dtype('int8'),
    'Int16': np.dtype('int16'),
    'Int32': np.dtype('int32'),
    'Int64': np.dtype('int64'),
    'Byte': np.dtype('uint8'),
    'UInt16': np.dtype('uint16'),
    'UInt32': np.dtype('uint32'),
    'UInt64': np.dtype('uint64'),
    'Boolean': np.dtype('bool'),
}


def convert_2d_array_to_numpy_array(cnet_2darray):
    """
    Convert [][] Array to numpy equivalent.

    Parameters
    ----------
    cnet_2darray : System.Array
    2D array from C#.

    Returns
    -------
    2D numpy array

    """
    ndarr = np.array([convert_to_numpy_array(cn_arr) for cn_arr in cnet_2darray])
    return ndarr


def convert_to_numpy_array(cnet_array):
    """
    Converts CLR System.Array into numpy.ndarray.
    See _MAP_NET_NP for the mapping of CLR types to Numpy dtypes.
    Reference: https://github.com/pythonnet/pythonnet/issues/514

    Parameters
    ----------
    cnet_array : System.Array
        Array from C#

    Returns
    -------
    ndarray

    """
    if type(cnet_array) == list:
        return np.array(cnet_array)
    elif cnet_array is None:
        raise ValueError('cnet_array is None')
    elif type(cnet_array) == np.ndarray:
        return cnet_array

    dims = np.empty(cnet_array.Rank, dtype=int)
    for I in range(cnet_array.Rank):
        dims[I] = cnet_array.GetLength(I)
    cnet_type = cnet_array.GetType().GetElementType().Name

    try:
        np_array = np.empty(dims, order='C', dtype=_MAP_NET_NP[cnet_type])
    except KeyError:
        raise NotImplementedError("asNumpyArray does not yet support System type {}".format(cnet_type))

    try:  # Memmove
        src_handle = GCHandle.Alloc(cnet_array, GCHandleType.Pinned)
        src_ptr = src_handle.AddrOfPinnedObject().ToInt64()
        dest_ptr = np_array.__array_interface__['data'][0]
        ctypes.memmove(dest_ptr, src_ptr, np_array.nbytes)
    finally:
        if src_handle.IsAllocated:
            src_handle.Free()

    return np_array


def convert_numpy_array_to_cnet(nparray):
    '''
    Given a `numpy.ndarray` returns a CLR `System.Array`.  See _MAP_NP_NET for
    the mapping of Numpy dtypes to CLR types.

    Note: `complex64` and `complex128` arrays are converted to `float32`
    and `float64` arrays respectively with shape [m,n,...] -> [m,n,...,2]
    Reference: https://github.com/pythonnet/pythonnet/issues/514
    '''
    dims = nparray.shape
    dtype = nparray.dtype
    # For complex arrays, we must make a view of the array as its corresponding
    # float type.
    if dtype == np.complex64:
        dtype = np.dtype('float32')
        dims.append(2)
        nparray = nparray.view(np.float32).reshape(dims)
    elif dtype == np.complex128:
        dtype = np.dtype('float64')
        dims.append(2)
        nparray = nparray.view(np.float64).reshape(dims)

    cnet_dims = Array.CreateInstance(Int32, nparray.ndim)
    for I in range(nparray.ndim):
        cnet_dims[I] = Int32(dims[I])

    if not nparray.flags.c_contiguous:
        nparray = nparray.copy(order='C')
    assert nparray.flags.c_contiguous

    try:
        cnet_array = Array.CreateInstance(_MAP_NP_NET[dtype], cnet_dims)
    except KeyError:
        raise NotImplementedError("asNetArray does not yet support dtype {}".format(dtype))

    try:  # Memmove
        dest_handle = GCHandle.Alloc(cnet_array, GCHandleType.Pinned)
        src_ptr = nparray.__array_interface__['data'][0]
        dest_ptr = dest_handle.AddrOfPinnedObject().ToInt64()
        ctypes.memmove(dest_ptr, src_ptr, nparray.nbytes)
    finally:
        if dest_handle.IsAllocated: dest_handle.Free()
    return cnet_array


def convert_occupant_json_to_dicts(occ_json):
    """
    Convert C# JSON input into list of dicts in correct format.
    Incoming dict format is:
        {NumTargets, Desc, XLocDistribution (int), XLocParamA, XLocParamB, ParamUnitType, ExposureHours}

    Outgoing format is:
        {count, descrip, xdistr, xa, xb, ydistr, ya, yb, zdistr, za, zb, hours}

    Note that incoming distances are always in meters.

    Parameters
    ----------
    occ_json : JSON

    Returns
    -------

    """
    occ_cnet_list = json.loads(occ_json)
    occ_groups = []

    for cnet_group in occ_cnet_list:
        # distributions from enum so convert to string representing normal, uniform, deterministic (constant)
        distr_labels = ['norm', 'unif', 'dete']
        x_distr = distr_labels[cnet_group['XLocDistribution']]
        y_distr = distr_labels[cnet_group['YLocDistribution']]
        z_distr = distr_labels[cnet_group['ZLocDistribution']]

        group = {
            'count': cnet_group['NumTargets'],
            'descrip': cnet_group['Desc'],
            'hours': cnet_group['ExposureHours'],
            'xdistr': x_distr,
            'xa': cnet_group['XLocParamA'],
            'xb': cnet_group['XLocParamB'],
            'ydistr': y_distr,
            'ya': cnet_group['YLocParamA'],
            'yb': cnet_group['YLocParamB'],
            'zdistr': z_distr,
            'za': cnet_group['ZLocParamA'],
            'zb': cnet_group['ZLocParamB'],
        }
        occ_groups.append(group)

    return occ_groups


def parse_nozzle_param(noz_str):
    ustr = noz_str.upper()
    return ustr
