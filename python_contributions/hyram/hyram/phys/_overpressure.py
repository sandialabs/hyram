
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

# coding: utf-8

# ##Lean Limit
from __future__ import print_function, absolute_import, division

import numpy as np
from scipy import optimize, special


# ##Overpressure


def dP_expansion(enclosure, mass, gas):
    '''
    Pressure due to the expansion of gas from combustion in an enclosure
    
    ESH: Note: only valid for hydrogen at this point--need to rework products_hydrogen_stoich 
    
    Parameters
    ----------
    enclosure : object
       enclosure where expansion occurs
    mass : float
       mass of combustible gas in enclosure
    gas : object
       gas being combusted (at the temperature and pressure of the gas in the enclosure)
       
    Returns
    -------
    P : float
       pressure upon expansion
    '''
    Vol_total = enclosure.V
    Vol_gas   = mass/gas.rho

    prod = products_hydrogen_stoich(gas.T, 1)
    Xi_u, sigma = prod[0], prod[-1]    

    VolStoich = Vol_gas/Xi_u[2]

    deltaP  = gas.P*((((Vol_total+Vol_gas)/Vol_total)*((Vol_total+VolStoich*(sigma-1))/Vol_total))**gas.therm.gamma-1)
    return deltaP



#### Function computes the composition and select thermodymic variables for
#### both the products and reactants of complete combustion
# Inputs:
#   T0:     Ambient temperature of reactants(K)
#   Xi_f:   mole fraction of fuel
#   selector:Specifies the fuel type (1: hydrogen, 2: methane)
#
# Outputs:
#   MW_u, MW_b: Molecular weights for unburned and burned (g/mol)
#   Cp_r, Cp_p: Specific heats for reactants and products (J/(kg K))
#   gammar, gammap: Specific heat ratio for reactants and products 

def products_hydrogen_stoich(T0,selector):
    MW_O2   = 31.999
    MW_N2   = 28.013
    MW_CO2 = 44.01
    MW_H2O = 18.015
    Xi_O2_a = 0.2095
    if selector is 1:
        MW_f= 2.016
        DHc = 119.0e6  # heat of combustion [J/kg]
        cnum= 0.
        hnum= 2.
    elif selector is 2:
        MW_f= 16.04
        DHc = 50.0e6  # heat of combustion [J/kg]
        cnum= 1.
        hnum= 4.

    ratio   = 1./(hnum/4. + cnum)
    Xi_f    = Xi_O2_a*ratio/(1+Xi_O2_a*ratio)
    MW      = np.array([MW_O2,MW_N2,MW_f,MW_H2O,MW_CO2])
    Ru      = 8314.5

    Xi_u    = np.zeros(5)
    Xi_b    = np.zeros(5)

    Xi_u[0] = Xi_O2_a*(1-Xi_f)
    Xi_u[1] = (1 - Xi_O2_a)*(1-Xi_f)
    Xi_u[2] = Xi_f
    MW_u    = np.sum(Xi_u*MW)
    Y_u     = Xi_u*MW/MW_u

    Xi_b[0] = Xi_u[0] - 1./ratio*Xi_f
    Xi_b[1] = Xi_u[1]
    Xi_b[3] = Xi_f*hnum/2.
    Xi_b[4] = Xi_f*cnum
    Xi_b    = Xi_b/np.sum(Xi_b)
    MW_b    = np.sum(Xi_b*MW)
    Y_b     = Xi_b*MW/MW_b

    DHc     = DHc*Y_u[2]
    Cp_u    = computeCp(Xi_u,T0,MW,selector)
    # [J/(kg K)]
    R_u     = Ru/MW_u
    gamma_u = Cp_u/(Cp_u - R_u)
    H0      = Cp_u*T0

    MW_b    = np.sum(Xi_b*MW)
    H_b     = H0 + DHc

    Cpguess = computeCp(Xi_b,T0,MW,selector)  # [J/(kg K)]
    Tguess  = H_b/Cpguess 
    f0      = lambda T: computeT(T,H_b,Xi_b,MW,selector)
    T_b     = optimize.fsolve(f0,Tguess)[0]  # Saturation Temperature [K]
    Cp_b    = H_b/T_b
    R_b     = Ru/MW_b
    gamma_b = Cp_b/(Cp_b - R_b)

    sigma   = (MW_u/T0)/(MW_b/T_b)
    return [Xi_u,Xi_b,Y_u,Y_b,MW_b,MW_u,gamma_b,gamma_u,T_b,sigma]


def computeT(T,H_b,Xi_b,MW,selector):
    Cp  = computeCp(Xi_b,T,MW,selector)
    y   = Cp*T - H_b
    return y

# Compute specific heat for each species (data from CHEMKIN thermo file)
def computeCp(Xi,T,MW,selector): # [J/(kg K)]
    O2  = 0
    N2  = 1
    CH4 = 2
    H2O = 3
    CO2 = 4
    H2  = 5

    if selector is 1:
        fuel = CH4  # hydrogen
    elif selector is 2:
        fuel = H2  # methane
    
    ORDER = [O2, N2, fuel, H2O, CO2]

    TLIM = np.zeros([6,3])
    TLIM[O2,:] = [700, 2000, 6000]
    TLIM[N2,:] = [500, 2000, 6000]
    TLIM[CH4,:]= [1300, 6000, 0]
    TLIM[H2O,:] = [1700, 6000, 0]
    TLIM[CO2,:] = [1200, 6000, 0]
    TLIM[H2,:]  = [1000, 2500, 6000]

    A, B, C, D, E = np.zeros([6,3]), np.zeros([6,3]), np.zeros([6,3]), np.zeros([6,3]), np.zeros([6,3])
    F, G, H = np.zeros([6,3]), np.zeros([6,3]), np.zeros([6,3])

    A[O2,0] = 31.32234
    B[O2,0] = -20.23531
    C[O2,0] = 57.86644
    D[O2,0] = -36.50624
    E[O2,0] = -0.007374
    F[O2,0] = -8.903471
    G[O2,0] = 246.7945
    H[O2,0] = 0

    A[O2,1] = 30.03235
    B[O2,1] = 8.772972
    C[O2,1] = -3.988133
    D[O2,1] = 0.788313
    E[O2,1] = -0.741599
    F[O2,1] = -11.32468
    G[O2,1] = 236.1663
    H[O2,1] = 0

    A[O2,2] = 20.91111
    B[O2,2] = 10.72071
    C[O2,2] = -2.020498
    D[O2,2] = 0.146449
    E[O2,2] = 9.245722
    F[O2,2] = 5.337651
    G[O2,2] = 237.6185
    H[O2,2] = 0

    A[N2,0] = 28.98641
    B[N2,0] = 1.853978
    C[N2,0] = -9.647459
    D[N2,0] = 16.63537
    E[N2,0] = 0.000117
    F[N2,0] = -8.671914
    G[N2,0] = 226.4168
    H[N2,0] = 0

    A[N2,1] = 19.50583
    B[N2,1] = 19.88705
    C[N2,1] = -8.598535
    D[N2,1] = 1.369784
    E[N2,1] = 0.527601
    F[N2,1] = -4.935202
    G[N2,1] = 212.39
    H[N2,1] = 0

    A[N2,2] = 35.51872
    B[N2,2] = 1.128728
    C[N2,2] = -0.196103
    D[N2,2] = 0.014662
    E[N2,2] = -4.55376
    F[N2,2] = -18.97091
    G[N2,2] = 224.981
    H[N2,2] = 0

    A[CH4,0]= -0.703029
    B[CH4,0]= 108.4773
    C[CH4,0]= -42.52157
    D[CH4,0]= 5.862788
    E[CH4,0]= 0.678565
    F[CH4,0]= -76.84376
    G[CH4,0]= 158.7163
    H[CH4,0]= -74.8731

    A[CH4,1]= 85.81217
    B[CH4,1]= 11.26467
    C[CH4,1]= -2.114146
    D[CH4,1]= 0.13819
    E[CH4,1]= -26.42221
    F[CH4,1]= -153.5327
    G[CH4,1]= 224.4143
    H[CH4,1]= -74.8731

    A[H2O,0]= 30.092
    B[H2O,0]= 6.832514
    C[H2O,0]= 6.793435
    D[H2O,0]= -2.53448
    E[H2O,0]= 0.082139
    F[H2O,0]= -250.881
    G[H2O,0]= 223.3967
    H[H2O,0]= -241.8264

    A[H2O,1]= 41.96426
    B[H2O,1]= 8.622053
    C[H2O,1]= -1.49978
    D[H2O,1]= 0.098119
    E[H2O,1]= -11.15764
    F[H2O,1]= -272.1797
    G[H2O,1]= 219.7809
    H[H2O,1]= -241.8264

    A[CO2,0]= 24.99735
    B[CO2,0]= 55.18696
    C[CO2,0]= -33.69137
    D[CO2,0]= 7.948387
    E[CO2,0]= -0.136638
    F[CO2,0]= -403.6075
    G[CO2,0]= 228.2431
    H[CO2,0]= -393.5224

    A[CO2,1]= 58.16639
    B[CO2,1]= 2.720074
    C[CO2,1]= -0.492289
    D[CO2,1]= 0.038844
    E[CO2,1]= -6.447293
    F[CO2,1]= -425.9186
    G[CO2,1]= 263.6125
    H[CO2,1]= -393.5224

    A[H2,:] =  [33.066178,18.563083,43.413560]
    B[H2,:] = [-11.363417,12.257357,-4.293079]
    C[H2,:] = [11.432816,-2.859786,1.272428]
    D[H2,:] = [-2.772874,0.268238,-0.096876]
    E[H2,:] = [-0.158558,1.977990,-20.533862]
    F[H2,:] = [-9.980797,-1.147438,-38.515158]
    G[H2,:] = [172.707974,156.288133,162.081354]
    H[H2,:] = [0.0,0.0,0]

    Cp = np.zeros(5)
    for I in range(len(ORDER)):
        N = ORDER[I]
        if T < TLIM[N,0]:
            J = 0
        elif T < TLIM[N,1]:
            J = 1
        else:
            J = 2
        t = T/1000.
        Cp[I] = A[N,J] + B[N,J]*t + C[N,J]*t**2 + D[N,J]*t**3 + E[N,J]/t**2
    Cp_tot = np.sum(Cp*Xi)/np.sum(MW*Xi)*1000  # (J/(kg K))
    return Cp_tot

