"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

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
    def layer_accumulation(self, t, Vol_conc0, enclosure, tol = 1e-4):
        '''
        Integrates the Lowesmith layer accumulation over time t

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
        
        r = integrate.ode(self._gov_eqns).set_integrator('dopri5', atol = tol, rtol = tol)
        
        r.set_f_params(enclosure)
        r.set_initial_value(Vol_conc0, t[0])

        T, Vc = [], []
        def solout(t, y):
            T.append(t)
            y[0] = max(0, min(y[0], enclosure.V))
            y[1] = max(0, min(y[1], 1))
            Vc.append(np.array(y))
        r.set_solout(solout)

        for tval in t[1:]:
            r.integrate(tval)
            if not r.successful():
                warnings.warn('break at %f sec'%r.t)
                break
        Vol, c = np.array(Vc).T
        t = np.array(T)
        return t, Vol, c

    def _gov_eqns(self, t, Vol_conc, enclosure):
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
