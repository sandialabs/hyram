"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import copy

import numpy as np
from scipy import optimize

from ._comps import Orifice


class NotionalNozzle:
    def __init__(self, nozzle, low_P_fluid):
        '''Notional nozzle class'''
        self.nozzle, self.low_P_fluid = nozzle, low_P_fluid

    def calculate(self, T = 'solve_energy', conserve_momentum = True):
        '''
        Calculates the properties after the notional nozzle using one of the following models:

        Parameters
        ----------
        T: string ('solve_energy', or 'Tthroat' - if conserve_momentum is false) or value
            how to calculate or specify temperature at end of notional nozzle
        conserve_momentum: boolean
            whether to use conservation of momentum equation
        Literature references for models of different combinations are:
            YuceilOtugen - conserve_momentum = True, T = solve_energy
            EwanMoodie - conserve_momentum = False, T = Tthroat
            Birch - conserve_momentum = False, T = T0
            Birch2 - conserve_momentum = True, T = T0
            Molkov - conserve_momentum = False, T = solve_energy

        Returns
        -------
        tuple of (fluid object, orifice object), all at exit of notional nozzle
        '''
        throat = self.nozzle.fluid
        if conserve_momentum:
            #YuceilOtugen, Birch2
            v = throat.v + (throat.P - self.low_P_fluid.P)/(throat.v*throat.rho*self.nozzle.orifice.Cd)
            if T == 'solve_energy':
                #YuceilOtugen
                throat_enthalpy = throat.therm.get_property('H0', P=throat.P, D=throat.rho, v=throat.v)
                rho = optimize.brentq(lambda rho: throat_enthalpy - throat.therm.get_property('H0', P=self.low_P_fluid.P, D=rho, v=v),
                                      throat.rho, throat.therm.get_property('D', T=self.low_P_fluid.T, P=self.low_P_fluid.P))
            elif np.isreal(T):
                #Birch2 - T specified as T0
                rho = throat.therm.get_property('D', T=T, P=self.low_P_fluid.P)
            else:
                raise NotImplementedError('Notional nozzle model not defined properly, ' +
                                          "nn_T must be specified temperature or 'solve_energy'.")
        else:
            #EwanMoodie, Birch, Molkov -- assume sonic at notional nozzle location
            if np.isreal(T):
                # Birch: T specified as T0
                v = throat.therm.get_property('A', T=T, P=self.low_P_fluid.P)
                rho = throat.therm.get_property('D', T=T, P=self.low_P_fluid.P)
            elif T == 'Tthroat':
                #EwanMoodie - T is Throat temperature
                v = throat.therm.get_property('A', T=throat.T, P=self.low_P_fluid.P)
                rho = throat.therm.get_property('D', T=throat.T, P=self.low_P_fluid.P)
            elif T == 'solve_energy':
                #Molkov
                def err(rho):
                    T = throat.therm.get_property('T', P=self.low_P_fluid.P, D=rho)
                    v = throat.therm.get_property('A', T=T, P=self.low_P_fluid.P)
                    return (throat.therm.get_property('H0', P=throat.P, D=throat.rho, v=throat.v) -
                            throat.therm.get_property('H0', P=self.low_P_fluid.P, D=rho, v=v))
                rho = optimize.newton(err, throat.therm.get_property('D', T=self.low_P_fluid.T, P=self.low_P_fluid.P))
                T = throat.therm.get_property('T', P=self.low_P_fluid.P, D=rho)
                v = throat.therm.get_property('A', T=T, P=self.low_P_fluid.P)
            else:
                raise NotImplementedError('Notional nozzle model not defined properly, ' +
                                          "nn_T must be specified temperature or 'solve_energy'")
        fluid = copy.copy(throat)
        fluid.update(rho=rho, P=self.low_P_fluid.P, v=v)
        # conserve mass to solve for effective diameter:
        orifice = Orifice(np.sqrt(self.nozzle.mdot/(fluid.rho*fluid.v)*4/np.pi))
        return fluid, orifice
