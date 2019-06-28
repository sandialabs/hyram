from h2_comps import *
from h2_therm import *
import sys

def TankMass_wrapper(T=None, P=None,TankVol=None):
    release_gas  = Gas(AbelNoble(), T = T, P = P)
    source       = Source(TankVol, release_gas)

    result = source.m

    return result

