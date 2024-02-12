"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np

from . import distributions



class ComponentFailureSet:
    """

    Parameters
    ----------
    f_failure_override : float or None
        Manual frequency value for accidents/shutdown failure.
        Other parameters will be ignored if this is not None.

    num_vehicles : int
        Number of vehicles in use.

    daily_fuelings : int
        Number of fuelings per day for each vehicle

    vehicle_days : int
        Annual days of operations

    noz_po_dist : str
        ID of distribution for pop-off failure mode.

    noz_po_a : float
        Failure distribution parameter A

    noz_po_b : float or None
        Failure distribution parameter B. None if using Expected Value

    noz_ftc_dist : str
        ID of distribution for nozzle failure-to-close failure mode.

    noz_ftc_a : float
        Failure distribution parameter A

    noz_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    mvalve_ftc_dist : str
        ID of distribution for manual valve FTC failure mode.

    mvalve_ftc_a : float
        Failure distribution parameter A

    mvalve_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    svalve_ftc_dist : str
        ID of distribution for solenoid valve FTC failure mode.

    svalve_ftc_a : float
        Failure distribution parameter A

    svalve_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    svalve_ccf_dist : str
        ID of distribution for solenoid valve common-cause failure mode.

    svalve_ccf_a : float
        Failure distribution parameter A

    svalve_ccf_b : float or None
        Failure distribution parameter B. None if using Expected Value

    overp_dist : str
        ID of distribution for overpressure failure mode.

    overp_a : float
        Failure distribution parameter A

    overp_b : float or None
        Failure distribution parameter B. None if using Expected Value

    pvalve_fto_dist : str
        ID of distribution for pressure-relief valve failure-to-open failure mode.

    pvalve_fto_a : float
        Failure distribution parameter A

    pvalve_fto_b : float or None
        Failure distribution parameter B. None if using Expected Value

    driveoff_dist : str
        ID of distribution for driveoff failure mode.

    driveoff_a : float
        Failure distribution parameter A

    driveoff_b : float or None
        Failure distribution parameter B. None if using Expected Value

    coupling_ftc_dist : str
        ID of distribution for coupling failure-to-close failure mode.

    coupling_ftc_a : float
        Failure distribution parameter A

    coupling_ftc_b : float or None
        Failure distribution parameter B. None if using Expected Value

    """
    use_override = False
    f_failure = None
    noz_po = None
    noz_ftc = None
    mvalve_ftc = None
    svalve_ftc = None
    svalve_ccf = None
    overp = None
    pvalve_fto = None
    driveoff = None
    coupling_ftc = None

    p_overp_rupture = None
    f_overp_rupture = None
    p_driveoff = None
    f_driveoff = None
    p_nozzle_release = None
    f_nozzle_release = None
    p_sol_valves_ftc = None
    f_sol_valves_ftc = None
    p_mvalve_ftc = None
    f_mvalve_ftc = None

    def __init__(self, f_failure_override=None,
                 num_vehicles=20, daily_fuelings=2, vehicle_days=250,
                 noz_po_dist='beta', noz_po_a=0.5, noz_po_b=610415.5,
                 noz_ftc_dist='expv', noz_ftc_a=0.002, noz_ftc_b=None,
                 mvalve_ftc_dist='expv', mvalve_ftc_a=0.001, mvalve_ftc_b=None,
                 svalve_ftc_dist='expv', svalve_ftc_a=0.002, svalve_ftc_b=None,
                 svalve_ccf_dist='expv', svalve_ccf_a=0.000127659574468085, svalve_ccf_b=None,
                 overp_dist='beta', overp_a=3.5, overp_b=310289.5,
                 pvalve_fto_dist='logn', pvalve_fto_a=-11.7359368859313, pvalve_fto_b=0.667849415603714,
                 driveoff_dist='beta', driveoff_a=31.5, driveoff_b=610384.5,
                 coupling_ftc_dist='beta', coupling_ftc_a=0.5, coupling_ftc_b=5031, verbose=False):

        if f_failure_override is not None:
            # User provided vehicle fueling failure frequency directly so ignore individual events
            self.use_override = True
            self.f_failure = float(f_failure_override)

        else:
            # calculate events; will be applied to 100% release for now
            self.use_override = False
            self.noz_po = ComponentFailure('Nozzle', 'Pop-off', noz_po_dist, noz_po_a, noz_po_b)
            self.noz_ftc = ComponentFailure('Nozzle', 'Failure to close', noz_ftc_dist, noz_ftc_a, noz_ftc_b)
            self.mvalve_ftc = ComponentFailure('Manual valve', 'Failure to close',
                                               mvalve_ftc_dist, mvalve_ftc_a, mvalve_ftc_b)
            self.svalve_ftc = ComponentFailure('Solenoid valves', 'Failure to close',
                                               svalve_ftc_dist, svalve_ftc_a, svalve_ftc_b)
            self.svalve_ccf = ComponentFailure('Solenoid valves', 'Common-cause failure',
                                               svalve_ccf_dist, svalve_ccf_a, svalve_ccf_b)

            self.overp = ComponentFailure('Overpressure during fueling', 'Accident', overp_dist, overp_a, overp_b)
            self.pvalve_fto = ComponentFailure('Pressure-relief valve', 'Failure to open',
                                               pvalve_fto_dist, pvalve_fto_a, pvalve_fto_b)
            self.driveoff = ComponentFailure('Driveoff', 'Accident', driveoff_dist, driveoff_a, driveoff_b)
            self.coupling_ftc = ComponentFailure('Breakaway coupling', 'Failure to close',
                                                 coupling_ftc_dist, coupling_ftc_a, coupling_ftc_b)

            num_fuelings = num_vehicles * daily_fuelings * vehicle_days
            self.p_driveoff = np.around(self.driveoff.mean * self.coupling_ftc.mean, 20)
            self.f_driveoff = np.around(num_fuelings * self.p_driveoff, 20)

            self.p_overp_rupture = np.around(self.overp.mean * self.pvalve_fto.mean, 20)
            self.f_overp_rupture = np.around(num_fuelings * self.p_overp_rupture, 20)
            f_accidents = self.f_overp_rupture + self.f_driveoff

            self.p_nozzle_release = np.around(self.noz_po.mean + self.noz_ftc.mean, 20)
            self.f_nozzle_release = np.around(num_fuelings * self.p_nozzle_release, 20)

            self.p_sol_valves_ftc = np.around(self.svalve_ftc.mean ** 3. + self.svalve_ccf.mean, 20)
            self.f_sol_valves_ftc = np.around(num_fuelings * self.p_sol_valves_ftc, 20)

            self.p_mvalve_ftc = np.around(self.mvalve_ftc.mean, 20)
            self.f_mvalve_ftc = np.around(num_fuelings * self.p_mvalve_ftc, 20)

            # Combined
            p_shutdown_fail = self.p_sol_valves_ftc * self.mvalve_ftc.mean * self.p_nozzle_release
            f_shutdown_fail = num_fuelings * p_shutdown_fail
            self.f_failure = np.around(float(f_accidents + f_shutdown_fail), 20)

            if verbose:
                print("COMPONENT FAILURE DATA")
                print(f'{num_fuelings} fuelings ({num_vehicles} vehicles, {daily_fuelings} fuel/d, {vehicle_days} days)')
                print("Nozzle popoff {}, {}, {}".format(noz_po_dist, noz_po_a, noz_po_b))
                print("Nozzle FTC {}, {}, {}".format(noz_ftc_dist, noz_ftc_a, noz_ftc_b))
                print("MValve FTC {}, {}, {}".format(mvalve_ftc_dist, mvalve_ftc_a, mvalve_ftc_b))
                print("SValve FTC {}, {}, {}".format(svalve_ftc_dist, svalve_ftc_a, svalve_ftc_b))
                print("SValve CCF {}, {}, {}".format(svalve_ccf_dist, svalve_ccf_a, svalve_ccf_b))
                print("PR Valve FTO {}, {}, {}".format(pvalve_fto_dist, pvalve_fto_a, pvalve_fto_b))
                print("BreakCoup FTC {}, {}, {}".format(coupling_ftc_dist, coupling_ftc_a, coupling_ftc_b))
                print("Fueling overp {}, {}, {}".format(overp_dist, overp_a, overp_b))
                print("Driveoff {}, {}, {}".format(driveoff_dist, driveoff_a, driveoff_b))

                print("Driveoff P {:.3g}, F {:.3g}".format(self.p_driveoff, self.f_driveoff))
                print("Overpressure rupture P {:.3g}, F {:.3g}".format(self.p_overp_rupture, self.f_overp_rupture))
                print("Nozzle release P {:.3g}, F {:.3g}".format(self.p_nozzle_release, self.f_nozzle_release))
                print("Sol valve FTC P {:.3g}, F {:.3g}".format(self.p_sol_valves_ftc, self.f_sol_valves_ftc))
                print("Shutdown fail P {:.3g}, F {:.3g}".format(p_shutdown_fail, f_shutdown_fail))

        if verbose:
            print("Frequency of other failures: {:.3g}".format(self.f_failure))

    def __str__(self):
        if self.use_override:
            # User provided vehicle fueling failure frequency directly so ignore individual events
            return f"Fuel failure OVERRIDE: {self.f_failure}"

        else:
            return f"Fuel failure: {self.f_failure}"


class ComponentFailure:
    """

    Parameters
    ----------
    component : str
        Name of component

    mode : str
        Description of failure mode

    distr_type : str
        Name referencing distribution type. lognormal, beta, ev, normal, uniform

    a : float
        First parameter describing distribution. Depends on type.
        e.g. if lognormal, assume this is sigma

    b : float
        Second parameter describing distribution. Depends on type.
        e.g. if lognorm, assume this is mu
    """

    def __init__(self, component, mode, distr_type, a, b=None):
        if not distributions.has_distribution(distr_type):
            msg = "Component failure distribution key {} not recognized".format(distr_type)
            raise ValueError(msg)
        if a is None and b is None:
            msg = "Distribution parameters cannot both be None"
            raise ValueError(msg)

        self.component = component
        self.mode = self.failure_mode = mode
        self.distr_type = distr_type
        self.a = a
        self.b = b

        distr_class = distributions.get_distribution_class(self.distr_type)
        self.distr = distr_class(a, b)
        self.mean = self.p = self.distr.mean

    def __str__(self):
        return "Component failure: {} {} | {}, mean {:.3g}".format(self.component, self.mode, self.distr, self.mean)
