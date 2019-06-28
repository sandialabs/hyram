from h2_comps import *
from h2_therm import *

def MassFlowRate_wrapper(T=None, P=None,TankVol=None, d_orifice=None, Steady = True , Cd0=1):
    release_gas  = Gas(AbelNoble(), T = T, P = P)
    orifice      = Orifice(d_orifice, Cd0)

    if Steady == True:
        rho_throat = release_gas.therm.rho_Iflow(release_gas.therm.rho(release_gas.T, release_gas.P))
        T_throat = release_gas.therm.T_Iflow(release_gas.T, rho_throat)
        mdot = orifice.mdot(rho_throat, release_gas.therm.a(T_throat, rho_throat)*1.)
        result = mdot
    else:
        source       = Source(TankVol, release_gas)
        tmax = 30
        numplume = 200
        ts = append(0, logspace(log10(tmax/1e5),log10(tmax), numplume - 1));
        #mdots, gas_list = source.blowdown(ts, orifice)
        mdots, gas_list, t =source.empty(orifice)
        result = t[-1]

        print("Calling figure(1)")
        figure(1)
        print("Calling plot")
        plot(t, mdots)
        print ("Calling SaveFig")
        mdots.savefig(getcwd() + "\\" + image.png")
        xlabel('time [s]');ylabel('Mass Flow Rate [kg/s]')
        print ("Calling Show()")
        show()
        print("Called Show()")

    return result

