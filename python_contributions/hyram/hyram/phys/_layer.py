# coding: utf-8

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

# ## Layer Model:
# 
# from Lowesmith et al. IJHE, Vol 34, 2009

from __future__ import print_function, absolute_import, division

import numpy as np
from scipy import integrate
import scipy.constants as const

from ._comps import Gas


def runLayerModel(t, Vol_conc0,
                  plume, enclosure,
                  tol = 1e-8, model = 'Lowesmith'):
    '''integrates the Lowesmith layer accumulation over time t
    Inputs:
    -------
    t : ndarray
        arrray of time values to integrate over
    Vol_conc: ndarray
        gas volume (m^3) and concentration (mol/m^3) at the first time in the t array (t[0])
    '''
    if model == 'Lowesmith':
        r = integrate.ode(gov_eqns_Lowesmith).set_integrator('dopri5', atol = tol, rtol = tol)
    elif model == 'Ethan':
        r = integrate.ode(gov_eqns).set_integrator('dopri5', atol = tol, rtol = tol)
    r.set_f_params(plume, enclosure)
    r.set_initial_value(Vol_conc0, t[0])

    T, Vc = [r.t], [r.y]
    for tval in t[1:]:
        r.integrate(tval)
        if not r.successful():
            print('break at', tval)
            break
        T.append(r.t)
        Vc.append(r.y) 
    
    Vol, c = np.array(Vc).T

    return Vol, c


def gov_eqns(t, Vol_conc,
             plume, enclosure):
    '''
    ESH: This returns the derivitive of the volume and concentration with respect to time
    for a gas from a released jet within a vented enclosure.  
    Derived by ESH. 
    
    Assumption: gas volume in enclosure remains fixed (Q_in = Q_out).
    
    Parameters
    ----------
    t:  time
    Vol_conc:  array of [volume, concentration]
    plume: class describing release
    enclosure: class describing enclosure

    Returns
    -------
    array of differential values [d(volume)/dt, d(concentration)/dt]
    '''
    Vol, c = Vol_conc  # Layer Volume, Layer Concentration

    H_layer = Vol/enclosure.A  # height of a uniform layer with volume Vol
    y_layer = enclosure.H - H_layer  # y-coordinate of bottom of flammable layer
    
    H_vent  = H_layer - (enclosure.H - enclosure.ceiling_vent.H) # amount of layer height being exhausted by ceiling vent

    B, v = np.interp(y_layer, plume.y, plume.B), np.interp(y_layer, plume.y, plume.V_cl)
#    i       = np.argmax(y_layer < plume.y)
#    if i < len(plume.y) - 1:
#        dy      = (y_layer - plume.y[i])
#        dB_dy   = (plume.B[i+1] - plume.B[i])/(plume.y[i+1] - plume.y[i])
#        dv_dy   = (plume.V_cl[i+1] - plume.V_cl[i])/(plume.y[i+1] - plume.y[i])
#        #dY_dy   = (plume.Y_cl[i+1] - plume.Y_cl[i])/(plume.y[i+1] - plume.y[i])
#        B       = plume.B[i] + (dB_dy)*dy  # plume radius heading into flammable layer(m)
#        v       = plume.V_cl[i] + (dv_dy)*dy  # plume velocity heading into flammable layer(m/s)
#    else:
#        B       = plume.B[i]  # plume radius heading into flammable layer(m)
#        v       = plume.V_cl[i]  # plume velocity heading into flammable layer(m/s)
#        #Y       = plume.Y_cl[i]  # plume mass fraction heading into flammable layer

    Qj      = const.pi*B**2*v  # volumetric flow rate of plume into layer, (m**3/s)
    Q_H2_in = plume.mdot0/plume.rho0 #Need to redo this so it's not dependent on mdot0 and rho0 - should be able to use Y_cl! ESH
    Qw_B    = enclosure.floor_vent.Qw  #volumetric flowrate into enclosure from bottom vent due to wind (m**3/s)
    Qw_T    = enclosure.ceiling_vent.Qw  #volumetric flowrate into enclosure from top vent due to wind (m**3/s)
    Qout    = Q_H2_in + Qw_B + Qw_T # total flow out -- equal to total flow in
    A_T, A_B = enclosure.ceiling_vent.A*enclosure.ceiling_vent.Cd, enclosure.floor_vent.A*enclosure.floor_vent.Cd
    Qout_T, Qout_B = Qout*A_T/(A_T + A_B), Qout*A_B/(A_T + A_B)# split flow out based on areas
    Q_T     = Qw_T - Qout_T #Total flow IN top vent
    Q_B     = Qw_B - Qout_B #Total flow IN top vent
    
    if y_layer <= enclosure.ceiling_vent.H: # mixture is flowing out of top vent
        if y_layer <= enclosure.floor_vent.H: # mixture is flowing out of both vents
            dVdt = Qj + Q_T + Q_B
            QH2l = Q_H2_in + c*Q_T*(Q_T < 0) + c*Q_B*(Q_B < 0)
        else: # mixture is flowing out of top vent only
            dVdt = Qj + Q_T
            QH2l = Q_H2_in + c*Q_T*(Q_T < 0)
    else: # mixture is not flowing out either vent
        dVdt = Qj
        QH2l = Q_H2_in
    dcdt = QH2l/Vol

    return np.array([dVdt, dcdt])


def gov_eqns_Lowesmith(t, Vol_conc,
                       plume, enclosure):
    '''
    ESH: note: I would like to get rid of plume.mdot0 and plume.rho0 and calculate the mass flow rate
    another way--however, with the way the establishment region works right now, I don't get 
    the same answer...needs some work.
    
    This returns the derivitive of the volume and concentration with respect to time
    for a buyoant gas from a released jet within a vented enclosure.  The
    model is directly from Lowesmith et al. IJHE, Vol 34, 2009.
    
    Parameters
    ----------
    t:  time
    Vol_conc:  array of [volume, concentration]
    plume: class describing release
    enclosure: class describing enclosure

    Returns
    -------
    array of differential values [d(volume)/dt, d(concentration)/dt]
    '''
    Vol, c = Vol_conc  # Layer Volume, Layer Volume fraction (or mole fraction)

    H_layer = min(Vol/enclosure.A, enclosure.H)  # height of a uniform layer with volume Vol
    H_vent  = H_layer - (enclosure.H - enclosure.ceiling_vent.H) # amount of layer height being exhausted by ceiling vent
    y_layer = enclosure.H - H_layer  # y-coordinate of bottom of flammable layer

    B, v = np.interp(y_layer, plume.y, plume.B), np.interp(y_layer, plume.y, plume.V_cl)

    Qj      = const.pi * B**2 * v  # volumetric flow rate of plume into layer (m**3/s)
    gas_rho = Gas(plume.gas.therm, T=plume.ambient.T, P=plume.ambient.P).rho
    Qs      = plume.mdot0 / gas_rho  # volumetric flow rate of plume into enclosure (m**3/s)

    Qw      = enclosure.floor_vent.Qw  # volumetric flow rate into enclosure due to wind (m**3/s)
    
    if H_vent > 0: # layer is below ceiling vent - layer is being exhausted
        # volumetric flowrate due to buoyancy
        QB = (enclosure.ceiling_vent.Cd * enclosure.ceiling_vent.A *
              np.sqrt(const.g * c * (1 - (plume.gas.therm.MW / plume.ambient.therm.MW)) * H_vent))
        Qin = np.sqrt(QB**2 + Qw**2)
        dVdt = Qj - (Qin + Qs)
        if H_layer == enclosure.H:
            # Layer fills entire enclosure volume
            dVdt = min(0, dVdt) # dVdt <= 0
    else: # Layer is at or above upper vent
        QB = 0
        Qin = np.sqrt(QB**2 + Qw**2)
        dVdt = max(0, Qj - (Qin + Qs)) # dVdt >= 0
#        dVdt = Qj - (Qin + Qs)
    
    dcdt = (Qs - c*Qj) / Vol
    
    return np.array([dVdt, dcdt])
