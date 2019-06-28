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

import numpy as np
from scipy import optimize

from . import utils
from ._comps import Gas, Orifice


class NotionalNozzle:
    def __init__(self, high_P_gas, orifice, low_P_gas):
        '''Notional nozzle class'''
        self.high_P_gas, self.orifice, self.low_P_gas = high_P_gas, orifice, low_P_gas
        
    def throat(self):
        '''Determine the conditions at the throat'''
        high_P_gas, orifice = self.high_P_gas, self.orifice
        rhot    = high_P_gas.therm.rho_Iflow(high_P_gas.rho) # throat density [kg/m**3]
        Tt      = high_P_gas.therm.T_IE(high_P_gas.T, high_P_gas.rho, rhot) # temperature at throat [K]
        ct      = high_P_gas.therm.a(Tt, rhot) # throat speed of sound [m/s]
        mdot    = orifice.mdot(rhot, ct)  # Mass flow rate [kg/s]
        pt      = high_P_gas.therm.P(Tt, rhot) # throat pressure [Pa]
        return (rhot, Tt, ct, mdot, pt)
    
    def calculate(self, model='yuce'):
        '''
        Calculates the properties after the notional nozzle using one of the following models:
        
        YuceilOtugen
        EwanMoodie
        Birch
        Birch2
        Molkov
        HarstadBellan
        
        Parameters
        ----------
        model - string of model name (default is YuceilOtugen)
        
        Returns
        -------
        tuple of (Gas object, Orifice object, velocity), all at exit of notional nozzle
        '''
        name = utils.parse_nozzle_model(model)  # lower-case alphanumeric
        if name == 'yuce':
            return self._YuceilOtugen()
        elif name == 'ewan':
            return self._EwanMoodie()
        elif name == 'birc':
            return self._Birch()
        elif name == 'bir2':
            return self._Birch2()
        elif name == 'molk':
            return self._Molkov()
        elif name == 'hars':
            return self._HarstadBellan()
        else:
            raise ValueError("Nozzle model with key {} not found".format(name))

    def _YuceilOtugen(self):
        '''
        #*************************Ma**********************************************
        # This function uses the relationships developed by YUCEIL and OTUGEN 
        # (Physics of Fluids Vol 14 (2002) 4206 - 4215) to compute the effective 
        # area, velocity, and temperature after the Mach disk for an underexpanded 
        # jet. The model is identical to the Birch2 model except that the flow 
        # downstream of the Mach disk is assuemed to be sonic, and the downstream 
        # temperature is assumed to be the throat temperture.
        #
        # An additional feature that has been added to the model is the
        # inclusion of the Abel-Noble equation of state so that non-ideal gas
        # characteristics for HYDROGEN can be captured.
        #
        # Unlike the first Birch and the Ewan and Moodie models, this model 
        # accounts for the conservation of MASS and MOMENTUM; however, the
        # model assumes the flow remains supersonic, and no attempt is made to 
        # account for the change in entropy across the Mach disk. 
        #
        # Created by Isaac Ekoto (March 10, 2011)
        #*************************************************************************
        '''

        # Determine the conditions at the throat
        (rhot, Tt, ct, mdot, pt) = self.throat()
        
        # Simultaneous solution of conservation of mass and momentum (see Birch paper):
        Veff = ct + (pt - self.low_P_gas.P)/(ct*rhot) # Effective Velocity [m/s]
        T2 = self.high_P_gas.therm.T_IflowV(Tt, ct, pt, Veff, self.low_P_gas.P) # Effective Temperature (K)
        rho2 = self.high_P_gas.therm.rho(T2, self.low_P_gas.P)
        gas_eff = Gas(self.high_P_gas.therm, T = T2, rho = rho2)
        
        # Conservation of mass yeilds orifice diameter:
        orifice_eff = Orifice(np.sqrt(mdot/(rho2*Veff)*4/np.pi))
        
        return (gas_eff, orifice_eff, Veff)

    def _EwanMoodie(self):
        '''
        #*************************************************************************
        # This function uses the relationships developed by Ewan and Moodie
        # (Combust Sci Tech Vol 45 (1986) 275 - 288) to compute the effective 
        # area, velocity, and temperature after the Mach disk for an underexpanded 
        # jet. 
        # 
        # An additional feature that has been added to the model is the
        # inclusion of the Abel-Noble equation of state so that non-ideal gas
        # characteristics for HYDROGEN can be captured.
        #
        # The model accounts for the conservation of MASS only. The model is 
        # identical to the Birch Model except that the exit temperature is assumed
        # to be the throat temperature instead of the ambient temperature. The 
        # model makes the following assumptions:
        #
        # 1) The effective temperature after the Mach disk is equal to the
        #    throat temperature
        # 2) The Mach number after the Mach disk is assumed to be 1
        # 3) The static pressure at the Mach disk is assumed to be ambient pressure
        # 
        # No attempt is made to account for the change in entropy across the shock
        #
        # Created by Isaac Ekoto (March 10, 2011)
        #*************************************************************************
        '''
        (rhot, Tt, ct, mdot, pt) = self.throat()
        
        # Calculate state at expansion conditions from assumptions 1 & 3:
        T2      = Tt
        p2      = self.low_P_gas.P
        rho2    = self.high_P_gas.therm.rho(T2, p2)
        gas_eff = Gas(self.high_P_gas.therm, T = T2, rho = rho2)

        # The Mach number after the Mach disk is assumed to be 1 (this clearly is 
        # a faulty assumption).
        Veff    = self.high_P_gas.therm.a(T2, rho2) # speed of sound at Mach disk [m/s]
        
        # Conservation of mass yeilds orifice diameter:
        orifice_eff = Orifice(np.sqrt(mdot/(rho2*Veff)*4/np.pi))
        
        return (gas_eff, orifice_eff, Veff)
    
    def _Birch(self):
        '''
        #*************************************************************************
        # This function uses the relationships developed by Birch et al. 
        # (Combust Sci Tech Vol 36 (1984) 249 - 261) to compute the effective 
        # area, velocity, and temperature after the Mach disk for an underexpanded 
        # jet. 
        # 
        # An additional feature that has been added to the model is the
        # inclusion of the Abel-Noble equation of state so that non-ideal gas
        # characteristics for HYDROGEN can be captured.
        #
        # The model accounts for the conservation of MASS only. The model makes the
        # following assumptions:
        #
        # 1) The effective temperature after the Mach disk is equal to the
        #    stagnation temperature
        # 2) The Mach number after the Mach disk is assumed to be 1
        # 3) The static pressure at the Mach disk is assumed to be ambient pressure
        # 
        # No attempt is made to account for the change in entropy across the shock
        #
        # Created by Isaac Ekoto (March 10, 2011), updated by ESH (May, 2014)
        #*************************************************************************
        '''
        (rhot, Tt, ct, mdot, pt) = self.throat()
        # Calculate state at expansion conditions from assumptions 1 & 3:
        T2      = self.high_P_gas.T
        p2      = self.low_P_gas.P
        rho2    = self.high_P_gas.therm.rho(T2, p2)
        gas_eff = Gas(self.high_P_gas.therm, T = T2, rho = rho2)

        # Calculate speed from assumption 2 (which is clearly faulty):
        Veff    = self.high_P_gas.therm.a(T2, rho2) # speed of sound at Mach disk [m/s]
        
        # Conservation of mass yeilds orifice diameter:
        orifice_eff = Orifice(np.sqrt(mdot/(rho2*Veff)*4/np.pi))
        
        return (gas_eff, orifice_eff, Veff)
    
    def _Birch2(self):
        '''
        #*************************************************************************
        # This function uses the second set of relationships developed by Birch et  
        # al. (Combust Sci Tech Vol 52 (1987) 161 - 171) to compute the effective 
        # area, velocity, and temperature after the Mach disk for an underexpanded 
        # jet. 
        # 
        # An additional feature that has been added to the model is the
        # inclusion of the Abel-Noble equation of state so that non-ideal gas
        # characteristics for HYDROGEN can be captured.
        #
        # The model is similar to the Birch1 model, except that it also accounts 
        # for the conservation of Momentum along with the conservation of MASS. 
        # The model makes the following assumptions:
        #
        # 1) The effective temperature after the Mach disk is equal to the
        #    stagnation temperature
        # 2) The static pressure at the Mach disk is assumed to be ambient pressure
        # 
        # No attempt is made to account for the change in entropy across the shock
        #
        # Created by Isaac Ekoto (March 10, 2011), updated by ESH (May, 2014)
        #*************************************************************************
        '''
        # throat conditions:
        (rhot, Tt, ct, mdot, pt) = self.throat()
        
        # Assumptions used to calculate state at expansion conditions:
        T2      = self.high_P_gas.T  # Effective Temperature [K]
        rho2    = self.high_P_gas.therm.rho(T2, self.low_P_gas.P)
        gas_eff = Gas(self.high_P_gas.therm, T = T2, rho = rho2)
        
        # Simultaneous solution of conservation of mass and momentum (see Birch paper):
        Veff = ct + (pt - self.low_P_gas.P)/(ct*rhot) # Effective Velocity [m/s]
        
        # Conservation of mass yeilds orifice diameter:
        orifice_eff = Orifice(np.sqrt(mdot/(rho2*Veff)*4/np.pi))
        
        return (gas_eff, orifice_eff, Veff)
    
    def _Molkov(self):
        '''
        #*************************************************************************
        # This function uses the second set of relationships developed by Molkov et  
        # al. "PHYSICS AND MODELLING OF UNDER-EXPANDED JETS AND HYDROGEN DISPERSION
        # IN ATMOSPHERE" to compute the effective area, velocity, and temperature 
        # after the Mach disk for an underexpanded jet. 
        # 
        # An model includes both Abel-Noble and ideal gas formulations for the EOS.
        #
        # The model is similar to the Birch (1987) model, except that assumes:
        #
        # 1) A sonic velocity at the Mach disk
        # 2) The static pressure at the Mach disk is assumed to be ambient pressure
        # 
        # No attempt is made to account for the change in entropy across the shock
        #
        # Created by Isaac Ekoto (March 10, 2011)
        #*************************************************************************
        '''
        (rhot, Tt, ct, mdot, pt) = self.throat()
        
        Peff  =  self.low_P_gas.P
        Teff  =  self.high_P_gas.therm.T_IflowSonic(Tt, pt, Peff)
        gas_eff = Gas(self.high_P_gas.therm, T = Teff, P = Peff)
        
        Veff    = self.high_P_gas.therm.a(Teff, gas_eff.rho)  # Effective Velocity [m/s]
        Aeff    = mdot/(gas_eff.rho*Veff)  # Conservation of mass to get effective area [m**2]
        Deff    = np.sqrt(Aeff*4/np.pi)  # Effective Diameter [m]
        orifice_eff = Orifice(Deff)
        
        return (gas_eff, orifice_eff, Veff)
    
    
    def _HarstadBellan(self):
        '''
        #*************************************************************************
        # This function uses the relationships developed by Harstad and Bellan 
        # (Combustion and Flame Vol 144 (2006) 89 - 102) to compute the effective 
        # area, velocity, and temperature after the Mach disk for an underexpanded 
        # jet. 
        #
        # An additional feature that has been added to the model is the
        # inclusion of the Abel-Noble equation of state so that non-ideal gas
        # characteristics for HYDROGEN can be captured.
        #
        # The model accounts for the conservation of MASS, MOMENTUM, and ENERGY,
        # and unlike other models, also accounts for the increase in ENTROPY that
        # occurs accross the Mach Disk. From application of this model, all
        # downstream velocities will be subsonic, and there will be no assumed
        # locally sonic regions after the Mach disk. This model neglects locally
        # supersonic regions within the jet boundary surrounding the Mach disk.
        #
        # Created by Isaac Ekoto (March 10, 2011)
        #*************************************************************************
        '''
        (rhot, Tt, ct, mdot, pt) = self.throat()                                        
        
        rhoamb, pamb = self.low_P_gas.rho, self.low_P_gas.P
        p0, rho0 = self.high_P_gas.P, self.high_P_gas.rho
        gamma, b = self.high_P_gas.therm.gamma, self.high_P_gas.therm.b
        
        def err_MBS(MBS):
            '''# Funcion iteratively computes the Mach number BEFORE the Mach disk'''

            # From the assumed Mach number, the density BEFORE the shock is determined
            rhoBS   = self.high_P_gas.therm.rho_Iflow(rho0, MBS)

            # From the assumed Mach number and density before the shock, the density 
            # AFTER the shock is determined
            rhoAS   = optimize.fsolve(DensityAS,rhoamb, args = (rhoBS,MBS))[0]

            MAS     = MachAS(MBS,rhoBS,rhoAS)  # Mach AFTER shock

            pBS     = 1+(gamma*MBS**2)/(1-rhoBS*b)  # total pressure before shock (P + rho V^2)
            pAS     = 1+(gamma*MAS**2)/(1-rhoAS*b)  # total pressure after shock (P + rho V^2)
            TBS     = 1+((gamma-1)/(2*(1-rhoBS*b)**2))*MBS**2  #T0/T for isentropic expansion (eq. 52)

            p1      = p0/(TBS**(gamma/(gamma-1)))  #P0/P for isentropic expansion (eq. 56)
            y       = pamb/p1 - pBS/pAS
            return y
        
        def DensityAS(rhoAS,rhoBS,MBS):
            '''# Error in density ratio.  Density ratio calculated thorugh AN-EOS from p and T ratio
            calculated using momentum conservation across shock'''
            MAS     = MachAS(MBS,rhoBS,rhoAS)
            pBS     = 1+(gamma*MBS**2)/(1-rhoBS*b)  # momentum/area before shock (P + rho V^2)
            TBS     = 1+((gamma-1)/(2*(1-rhoBS*b)**2))*MBS**2  #T0/T for isentropic expansion (eq. 52)
            pAS     = 1+(gamma*MAS**2)/(1-rhoAS*b)  # momentum/area after shock (P + rho V^2)
            TAS     = 1+((gamma-1)/(2*(1-rhoAS*b)**2))*MAS**2  #T0/T for isentropic expansion (eq. 52)
            pas_pbs = pBS/pAS
            Tas_Tbs = TBS/TAS
            y       = rhoAS/rhoBS - pas_pbs/Tas_Tbs*(1-b*rhoAS)/(1-b*rhoBS)
            return y
        
        def MachAS(MBS,rhoBS,rhoAS):
            '''# Function determines the Mach number AFTER the shock'''
            pBStemp = 1+(gamma*MBS**2)/(1-rhoBS*b)  # momentum/area (P + rho V^2)
            TBStemp = 1+((gamma-1)/(2*(1-rhoBS*b)**2))*MBS**2  #T0/T for isentropic expansion (eq. 52)
            B       = MBS**2*TBStemp/pBStemp**2
            Atemp   = (2*gamma*B-1)*(1-rhoAS*b)
            Btemp   = (1-rhoAS*b)*np.sqrt((1-rhoAS*b)**2-4*gamma*B*(1-rhoAS*b)+2*B*(gamma-1))
            Ctemp   = gamma-1-2*B*gamma**2
            MAS     = np.sqrt((Atemp+Btemp)/Ctemp)
            return MAS
        
        # Determine the Mach number just befor the Mach Disk
        MBS     = optimize.fsolve(err_MBS, 10.)[0]
        
        # Recompute the DENSITY just IN FRONT of the Mach disk using the correct 
        # Mach number determined from the previous function [kg/m**3]
        rhoBS   = self.high_P_gas.therm.rho_Iflow(rho0, MBS)
        TBS = self.high_P_gas.therm.T_IE(self.high_P_gas.T, self.high_P_gas.rho, rhoBS)
        vBS = self.high_P_gas.therm.a(TBS, rhoBS)*MBS
        #mdot = self.orifice.mdot(rhoBS, vBS)

        # Recompute the DENSITY just AFTER of the Mach disk using the correct 
        # Mach number and density in front of the Mach disk determined from the 
        # previous two functions [kg/m**3]

        rhoAS   = optimize.fsolve(DensityAS,rhoamb, args = (rhoBS,MBS))[0]
        cbs    = self.high_P_gas.therm.a(TBS, rhoBS)
        
        # Compute the DOWNSTREAM Mach number using the computed upstream and 
        # downstream density, along with the upstream Mach number shock
        MAS     = MachAS(MBS,rhoBS,rhoAS)

        # Compute the DOWNSTREAM temperature
        TAS = self.high_P_gas.therm.T_Iflow(self.high_P_gas.T, rhoAS, MAS)
        
        gas_eff = Gas(self.high_P_gas.therm, T = TAS, rho = rhoAS)
        # speed of sounds downstream of the Mach disk [m/s]
        ceff    = self.high_P_gas.therm.a(TAS, rhoAS)
        Veff    = ceff*MAS  # Effective velocity downstream of the Mach disk [m/s]
        Aeff    = mdot/(rhoAS*Veff)  # Effective Area of Mach disk [m**2]
        Deff    = np.sqrt(Aeff*4/np.pi)  # Effective diameter of Mach disk [m]
        
        print(self.high_P_gas.therm.P(TBS, rhoBS), gas_eff.P, rhoBS, rhoAS, TBS, TAS, cbs*MBS, Veff)
        
        orifice_eff = Orifice(Deff)
            
        return (gas_eff, orifice_eff, Veff)

