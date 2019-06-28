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

from __future__ import print_function, absolute_import, division

import os

import numpy as np

from .._therm import AbelNoble, IdealGas, H2_Combustion, load_object
from .._comps import Gas, Orifice
from .._flame import Flame


def fl(T_amb, P_amb, T_H2, P_H2, d_orifice, Q=None,
       theta0=0., x0=0, y0=0,
       h2chem=None,
       lamf=1.24, lamv=1.24, am=3.42e-2, ab=0.000575, af=0.23,
       nnmodel='YuceilOtugen', numpts=100, numB=6, numBpts=10000,
       betaS=8.6, betaB=0.8, establish='old', verbose=True, chem_file='h2chem.pkl'):
    """
    Generates and solves for the flame.

    Parameters
    ----------
    T_amb: float
        temperature of the ambient air (K)
    P_amb: float
        pressure of the ambient air (Pa)
    T_H2: float
        temperature of the gas (H2) (K)
    P_H2: float
        pressure of the gas (H2) (K)
    d_orifice: float
        diameter of the leak (m)
    Q: float, optional if P_H2 >= 1.9*P_amb
        volumetric flow rate of subsonic leak (m^3/s)
    theta0 : float, optional
        angle of release (rad) default value of 0 is horizontal
    x0 : float, optional
        horizontal starting point (m)
    y0 : float, optional
        vertical starting point (m)
    h2chem : hydrogen chemistry class (see h2_therm for usage), optional
        if none given, will initialize new chemisty class
    lamf : float
        spreading ratio for mixture fraction Gaussian profiles
    lamv : float
        spreading ratio for velocity Gaussian profiles
    am : float
        momentum entrainment coefficient
    ab : float
        buoyancy entrainment coefficient
    af : float
        Plank's mean absorption coefficient for H2O
    nnmodel : string
        notional nozzle model to use for high-pressure release (see h2_nn for usage)
    numpts: float, optional
        number of points at which to solve for the flame
    numB : float, optional
        number of halfwidths to integrate to
    numBpts : float, optional
        number of points to use in integral model at each step
    betaS : float, optional
        proportionality constant for start of integral model relative to d* (see Li, et al. IJHE 2015)
    betaB : float, optional
        proportionality constant for width at start of integral model relative to d* (see Li, et al. IJHE 2015)

    chemfile : str, optional
        full filepath of output pickle file

    Returns
    -------
    flame: Flame object (see h2_flame)
    """
    try:
        if h2chem.Treac != T_amb or abs(h2chem.P / P_amb - 1) > 1e-10:
            h2chem.reinitilize(T_amb, P_amb)

    except Exception as err:
        try:
            h2chem = load_object(chem_file)
            if h2chem.Treac != T_amb or abs(h2chem.P / P_amb - 1) > 1e-10:
                h2chem.reinitilize(T_amb, P_amb)
        except Exception as err:
            h2chem = H2_Combustion(T_amb, P_amb, verbose=verbose)

    h2chem.save(chem_file)
    MWair = h2chem._MWmix(h2chem._Yreac(0))
    ambient = Gas(IdealGas(MW=MWair), T_amb, P=P_amb)
    h2 = Gas(AbelNoble(), T=T_H2, P=P_H2)
    o = Orifice(d_orifice)
    flame = Flame(h2, o, ambient, Q=Q,
                  theta0=theta0, x0=x0, y0=y0,
                  h2chem=h2chem,
                  lamf=lamf, lamv=lamv, am=am, ab=ab, af=af,
                  nnmodel=nnmodel, numpts=numpts, numB=numB, numBpts=numBpts,
                  betaS=betaS, betaB=betaB, verbose=verbose)
    flame.solve(establish=establish)
    return flame


def plot_temp(T_amb, P_amb, T_H2, P_H2, d_orifice, y0, theta0, nnmodel,
              output_dir=None,
              Q=None, x0=0, h2chem=None,
              lamf=1.24, lamv=1.24, am=3.42e-2, ab=0.000575, af=0.23,
              numpts=100, numB=6, numBpts=10000,
              betaS=8.6, betaB=0.8, establish='old',
              fname='Tplot.png', xlims=None, ylims=None,
              plot_title=None, mark=None, verbose=True, chem_file='h2chem.pkl'):
    """
    returns filename for plot of flame temperature and trajectory as a function of x, y, position

    Parameters
    ----------
    T_amb - float, ambient temperature (K)
    P_amb - float, ambient pressure (Pa)
    T_H2 - float, hydrogen temperature (K)
    P_H2 - float, hydrogen pressure (Pa)
    d_orifice - float, diameter of leak (m)
    y0 - float, height of leak (m)
    nnmodel - string, notional nozzle model (options are YuceilOtugen, EwanMoodie, Birch,
                                             Birch2, Molkov,or HarstadBellan)
    output_dir - string, directory in which to save picture

    Returns
    -------
    plot_file - plot filepath

    """
    if output_dir is None:
        output_dir = os.getcwd()
    plot_file = os.path.join(output_dir, fname)

    flame = fl(T_amb, P_amb, T_H2, P_H2, d_orifice, Q=Q,
               theta0=theta0, x0=x0, y0=y0,
               h2chem=h2chem,
               lamf=lamf, lamv=lamv, am=am, ab=ab, af=af,
               nnmodel=nnmodel, numpts=numpts, numB=numB,
               numBpts=numBpts, betaS=betaS, betaB=betaB,
               establish=establish, verbose=verbose, chem_file=chem_file)
    fig = flame.plot_Ts(xlims=xlims, ylims=ylims, plot_title=plot_title, mark=mark)
    fig.savefig(plot_file, dpi=300, bbox_inches='tight')
    return plot_file


def calcQ(x, y, z, RH, flame, smodel):
    '''
    TODO Add docstring to calcQ
    '''

    if smodel == 'multi':
        Q = flame.Qrad_multi(x, y, z, RH)
    elif smodel == 'single':
        Lvis = flame.length()
        flameCen = np.array([flame.x[np.argmin(np.abs(flame.S - Lvis / 2))],
                             flame.y[np.argmin(np.abs(flame.S - Lvis / 2))],
                             0])
        Q = flame.Qrad_single(x, y, z, flameCen, RH)
    else:
        raise ValueError('Radiative source model ("smodel") parameter in calcQ must be multi or single')

    return Q


def plot_flux(amb_temp, amb_pres, h2_temp, h2_pres, d_orifice, leak_height, release_angle, nozzle_model,
              rad_flux_x, rad_flux_y, rad_flux_z,
              output_dir=None,
              flux=None, x0=0,
              h2chem=None,
              lamf=1.24, lamv=1.24, am=3.42e-2, ab=0.000575, af=0.23,
              numpts=100, numB=6, numBpts=10000,
              betaS=8.6, betaB=0.8, establish='old',
              plot3d_name='3DisoPlot.png', plot2d_name='2DcutsIsoPlot.png',
              Tfname='Tplot.png',
              rad_source_model='multi', rel_humid=0.89,
              plot_title=None, contours=None, chem_file='h2chem.pkl',
              verbose=True):
    """
    TODO Add docstring

    Returns
    ----------
    flux : ndarray
        flux values

    flux2d_filepath : str
        filepath of 2D flux plot

    flame_temp_plot_filepath : str
        File path of flame temperature plot
    """
    if output_dir is None:
        output_dir = os.getcwd()

    # Make flame object and do calculations
    flame = fl(amb_temp, amb_pres, h2_temp, h2_pres, d_orifice, Q=flux,
               theta0=release_angle, x0=x0, y0=leak_height,
               h2chem=h2chem,
               lamf=lamf, lamv=lamv, am=am, ab=ab, af=af,
               nnmodel=nozzle_model, numpts=numpts, numB=numB,
               numBpts=numBpts, betaS=betaS, betaB=betaB,
               establish=establish, verbose=verbose, chem_file=chem_file)

    # Get desired heat flux values
    flux = calcQ(rad_flux_x, rad_flux_y, rad_flux_z, rel_humid, flame, rad_source_model)

    flux3d_filepath = os.path.join(output_dir, plot3d_name)
    flux2d_filepath = os.path.join(output_dir, plot2d_name)
    flame_temp_plot_filepath = os.path.join(output_dir, Tfname)

    # Make heat flux plot
    #   Currently only 2D slices are shown, 3D image not used
    flame.iso_heat_flux_plot_sliced(title=plot_title, flux3d_filepath=flux3d_filepath, flux2d_filepath=flux2d_filepath,
                                    smodel=rad_source_model, RH=rel_humid, contours=contours)

    fig = flame.plot_Ts()
    fig.savefig(flame_temp_plot_filepath, dpi=300, bbox_inches='tight')

    return flux, flux2d_filepath, flame_temp_plot_filepath
