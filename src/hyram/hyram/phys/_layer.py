# coding: utf-8

# ## Layer Model:
# 
# from Lowesmith et al. IJHE, Vol 34, 2009

from __future__ import print_function, absolute_import, division

import numpy as np
from scipy import integrate
import scipy.constants as const
import warnings

from ._comps import Fluid
from ._jet import Jet

########################################################################
#TODO: untested for alternate fuels
########################################################################

class LayeringJet(Jet):
    def layer_accumulation(self, t, Vol_conc0, enclosure,
                           tol = 1e-4, model = 'Lowesmith'):
        '''integrates the Lowesmith layer accumulation over time t
        Inputs:
        -------
        t : ndarray
            arrray of time values to integrate over
        Vol_conc: ndarray
            gas volume (m^3) and concentration (mol/m^3) at the first time in the t array (t[0])
        Returns:
        --------
        tuple of time, volume and concentration

        - note: just outputting final value - could change to output full array if desired
        '''
        if model == 'Lowesmith':
            r = integrate.ode(self._gov_eqns_Lowesmith).set_integrator('dopri5', atol = tol, rtol = tol)
        elif model == 'Ethan':
            r = integrate.ode(self._gov_eqns_Ethan).set_integrator('dopri5', atol = tol, rtol = tol)

        r.set_f_params(enclosure)
        r.set_initial_value(Vol_conc0, t[0])

        T, Vc = [], []
        def solout(t, y):
            T.append(t)
#            r.y[0] = max(0, min(r.y[0], enclosure.V))
#            r.y[1] = max(0, min(r.y[1], 1))
            y[0] = max(0, min(y[0], enclosure.V))
            y[1] = max(0, min(y[1], 1))
            Vc.append(np.array(y))
        r.set_solout(solout)

        for tval in t[1:]:
            r.integrate(tval)
            if not r.successful():
                #print(r.get_return_code())
                warnings.warn('break at %f sec'%r.t)
                break
        Vol, c = np.array(Vc).T
        t = np.array(T)
        return t[-1], Vol[-1], c[-1]

    def _gov_eqns_Lowesmith(self, t, Vol_conc,
                           enclosure):
        '''
        Returns the derivitive of the volume and concentration with respect to time
        for a buyoant gas from a released jet within a vented enclosure.  The
        model is directly from Lowesmith et al. IJHE, Vol 34, 2009.

        Parameters
        ----------
        t:  time
        Vol_conc:  array of [volume, concentration]
        enclosure: class describing enclosure

        Returns
        -------
        array of differential values [d(volume)/dt, d(concentration)/dt]
        '''
        Vol, c = Vol_conc  # Layer Volume, Layer Volume fraction (or mole fraction)

        H_layer = min(Vol/enclosure.A, enclosure.H)  # height of a uniform layer with volume Vol
        H_vent  = H_layer - (enclosure.H - enclosure.ceiling_vent.H) # amount of layer height being exhausted by ceiling vent
        y_layer = enclosure.H - H_layer  # y-coordinate of bottom of flammable layer

        B, v = np.interp(y_layer, self.y, self.B), np.interp(y_layer, self.y, self.V_cl)

        Qj      = const.pi * B**2 * v  # volumetric flow rate of jet into layer (m**3/s)
        Qs       = self.Q_jet # this is set in the IndoorRelease class - volumetric flow of jet into enclosure

        Qw      = enclosure.floor_vent.Qw  # volumetric flow rate into enclosure due to wind (m**3/s)
        if H_vent > 0: # layer is being exhausted
            # volumetric flowrate due to buoyancy
            QB = (enclosure.ceiling_vent.Cd * enclosure.ceiling_vent.A *
                  np.sqrt(const.g * c * (1 - (self.fluid.therm.MW / self.ambient.therm.MW)) * H_vent))
            Qin = np.sqrt(QB**2 + Qw**2)
            dVdt = Qj - (Qin + Qs)
            if H_layer == enclosure.H:
                # Layer fills entire enclosure volume
                dVdt = min(0, dVdt) # dVdt <= 0
        else: # Layer is at minimum value
            QB = 0
            Qin = np.sqrt(QB**2 + Qw**2)
            dVdt = max(0, Qj - (Qin + Qs)) # dVdt >= 0
    #        dVdt = Qj - (Qin + Qs)
        
        if c >= 1:
            dcdt = min(0, (Qs - c*Qj) / Vol)
        elif c <= 0:
            dcdt = max(0, (Qs - c*Qj) / Vol)
        else:
            dcdt = (Qs - c*Qj) / Vol
        return np.array([dVdt, dcdt])

    def _gov_eqns_Ethan(self, t, Vol_conc, enclosure):
        '''
        ESH: This returns the derivitive of the volume and concentration with respect to time
        for a gas from a released jet within a vented enclosure.
        Derived by ESH. In progress...

        Assumption: gas volume in enclosure remains fixed (Q_in = Q_out).

        Parameters
        ----------
        t:  time
        Vol_conc:  array of [volume, concentration]
        enclosure: class describing enclosure

        Returns
        -------
        array of differential values [d(volume)/dt, d(concentration)/dt]
        '''
        Vol, c = Vol_conc  # Layer Volume, Layer Concentration

        H_layer = Vol/enclosure.A  # height of a uniform layer with volume Vol
        y_layer = enclosure.H - H_layer  # y-coordinate of bottom of flammable layer

        H_vent  = H_layer - (enclosure.H - enclosure.ceiling_vent.H) # amount of layer height being exhausted by ceiling vent

        B, v = np.interp(y_layer, self.y, self.B), np.interp(y_layer, self.y, self.V_cl)
    #    i       = np.argmax(y_layer < self.y)
    #    if i < len(self.y) - 1:
    #        dy      = (y_layer - self.y[i])
    #        dB_dy   = (self.B[i+1] - self.B[i])/(self.y[i+1] - self.y[i])
    #        dv_dy   = (self.V_cl[i+1] - self.V_cl[i])/(self.y[i+1] - self.y[i])
    #        #dY_dy   = (self.Y_cl[i+1] - self.Y_cl[i])/(self.y[i+1] - self.y[i])
    #        B       = self.B[i] + (dB_dy)*dy  # jet radius heading into flammable layer(m)
    #        v       = self.V_cl[i] + (dv_dy)*dy  # jet velocity heading into flammable layer(m/s)
    #    else:
    #        B       = self.B[i]  # jet radius heading into flammable layer(m)
    #        v       = self.V_cl[i]  # jet velocity heading into flammable layer(m/s)
    #        #Y       = self.Y_cl[i]  # jet mass fraction heading into flammable layer

        Qj      = const.pi*B**2*v  # volumetric flow rate of jet into layer, (m**3/s)
        Q_H2_in = self.mdot0/self.rho0 #Need to redo this so it's not dependent on mdot0 and rho0 - should be able to use Y_cl! ESH
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
