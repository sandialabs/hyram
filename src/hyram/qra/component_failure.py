"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
from dataclasses import dataclass
from typing import Iterable, Optional

import numpy as np


@dataclass(frozen=True)
class ComponentFailure:
    """

    Parameters
    ----------
    component : str
        Name of component

    mode : str
        Description of failure mode

    value : float
        probability of component failure
    """
    component:str
    mode:str
    value:float

    def __str__(self):
        return (f'Component failure: {self.component} {self.mode} ' +
                f'| {(self.value * 100):.3g}%')


class ComponentFailureSet:
    """
    Probabilities and failure type/mode information for
    fueling failures associated with the dispenser.

    Parameters
    ----------
    num_vehicles : int
        Number of vehicles at the facility.

    daily_fuelings : int
        Average number of fuelings per vehicle per day.

    vehicle_days : float
        Number of operating days in a year.

    failures : [ComponentFailure]
        List of failure types/modes for the dispenser.

    verbose : bool, optional
        Returns verbose logging information when `True`.
    """

    def __init__ (self,
                  num_vehicles:int,
                  daily_fuelings:int,
                  vehicle_days:float,
                  failures:Iterable[ComponentFailure],
                  verbose:Optional[bool]=False):

        self.failures = failures
        num_fuelings = num_vehicles * daily_fuelings * vehicle_days
        self.prob_nozzle_release = np.round(sum(
            f.value for f in failures if f.component == 'Nozzle'), 20)
        self.prob_mvalve_ftc = np.round(sum(
            f.value for f in failures if f.component == 'Manual valve'), 20)
        self.prob_svalves_ftc = self.calc_prob_svalves_ftc(failures)
        self.prob_driveoff = self.calc_prob_driveoff(failures)
        self.freq_overp_rupture = self.calc_freq_overp_rupture(
            failures, num_fuelings)

        self.freq_driveoff = np.round(
            num_fuelings * self.prob_driveoff, 20)
        freq_accidents = self.freq_overp_rupture + self.freq_driveoff

        self.freq_nozzle_release = np.round(
            num_fuelings * self.prob_nozzle_release, 20)

        self.freq_svalves_ftc = np.round(
            num_fuelings * self.prob_svalves_ftc, 20)

        self.freq_mvalve_ftc = np.round(
            num_fuelings * self.prob_mvalve_ftc, 20)

        # Combined
        prob_shutdown_fail = (self.prob_svalves_ftc
                            * self.prob_mvalve_ftc
                            * self.prob_nozzle_release)
        freq_shutdown_fail = num_fuelings * prob_shutdown_fail
        self.freq_failure = np.round(float(
            freq_accidents + freq_shutdown_fail), 20)

        if verbose:
            print('COMPONENT FAILURE DATA' +
                  '======================' +
                 f'{num_fuelings} fuelings ({num_vehicles} vehicles, ' +
                 f'{daily_fuelings} fuel/d, {vehicle_days} days)\n' +
                  '----------------------')
            print(*failures, sep='\n')
            print('----------------------\n' +
                 f'Driveoff P {self.prob_driveoff:.3g}, ' +
                          f'F {self.freq_driveoff:.3g}\n' +
                 f'Overpressure rupture F {self.freq_overp_rupture:.3g}\n' +
                 f'Nozzle release P {self.prob_nozzle_release:.3g}, ' +
                                f'F {self.freq_nozzle_release:.3g}\n' +
                 f'Sol valve FTC P {self.prob_svalves_ftc:.3g}, ' +
                               f'F {self.freq_svalves_ftc:.3g}\n' +
                 f'Shutdown fail P {prob_shutdown_fail:.3g}, ' +
                               f'F {freq_shutdown_fail:.3g}')

    def to_dict(self):
        return {k:v for k, v in self.__dict__.items()
                if not (k.startswith('__') or k.startswith('failures'))
                and not callable(v)}

    def calc_prob_svalves_ftc(self, failures):
        prob_svalves_ftc = 0
        for f in failures:
            if f.component == 'Solenoid valves':
                if f.mode == 'Failure to close':
                    prob_svalves_ftc += f.value**3
                elif f.mode == 'Common-cause failure':
                    prob_svalves_ftc += f.value
        return np.round(prob_svalves_ftc, 20)

    def calc_prob_driveoff(self, failures):
        prob_driveoff = 1
        for f in failures:
            if f.component == 'Breakaway coupling':
                prob_driveoff *= f.value
            elif f.mode == 'Driveoff':
                prob_driveoff *= f.value
        return np.round(prob_driveoff, 20)

    def calc_freq_overp_rupture(self, failures, num_fuelings):
        freq_overp_rupture = 1
        for f in failures:
            if f.component == 'Pressure-relief valve':
                freq_overp_rupture *= f.value
            elif f.mode == 'Overpressure during fueling':
                freq_overp_rupture *= f.value
        return num_fuelings * np.round(freq_overp_rupture, 20)

    def __str__(self):
        return f"Fuel failure: {self.freq_failure}"

    def __repr__(self):
        str1 = 'Component Failure Set - '
        str2 = ''.join(f'{failure} - ' for failure in self.failures)
        str3 = f'Fuel failure: {(self.freq_failure*100):.3g}%'
        return str1 + str2 + str3


def create_failure_set(num_vehicles:int,
                       daily_fuelings:int,
                       vehicle_days:float,
                       prob_noz_popoff:Optional[float]=8.191135225813216e-07,
                       prob_noz_ftc:Optional[float]=0.002,
                       prob_mvalve_ftc:Optional[float]=0.001,
                       prob_svalve_ftc:Optional[float]=0.002,
                       prob_svalve_ccf:Optional[float]=0.000127659574468085,
                       prob_prvalve_fto:Optional[float]=np.exp(-11.7359368859313),
                       prob_coupl_ftc:Optional[float]=9.937394415184339e-05,
                       prob_overp:Optional[float]=1.1279661481245145e-05,
                       prob_driveoff:Optional[float]=5.160415192262326e-05):
    """
    Returns the `ComponentFailureSet` object for the given dispenser failure
    probabilities.

    If any probabilities are not specified, they are assumed to be
    default values.

    Parameters
    ----------
    num_vehicles : int
        Number of vehicles at the facility.

    daily_fuelings : int
        Average number of fuelings per vehicle per day.

    vehicle_days : float
        Number of operating days in a year.

    prob_noz_popoff : float, optional
        Probability of dispenser failure by nozzle pop-off.

    prob_noz_ftc : float, optional
        Probability of dispenser failure by the nozzle failing to close.

    prob_mvalve_ftc : float, optional
        Probability of dispenser failure by the manual valve failing to close.

    prob_svalve_ftc : float, optional
        Probability of dispenser failure by a solenoid valve failing to close.

    prob_svalve_ccf : float, optional
        Probability of dispenser failure by a cause making all solenoid valves
        fail to close (e.g., loss of connection to sensors).

    prob_prvalve_fto : float, optional
        Probability of dispenser failure by the pressure relief valve failing
        to open on demand.

    prob_coupl_ftc : float, optional
        Probability of dispenser failure by the breakaway coupling
        failing to close.

    prob_overp : float, optional
        Probability of dispenser failure by an overpressure accidentally
        occurring during fueling.

    prob_driveoff : float, optional
        Probability of dispenser failure by a vehicle driving off while
        still attached to the dispenser.
    """
    return ComponentFailureSet(
        num_vehicles=num_vehicles,
        daily_fuelings=daily_fuelings,
        vehicle_days=vehicle_days,
        failures=[
            ComponentFailure('Nozzle', 'Pop-off', prob_noz_popoff),
            ComponentFailure('Nozzle', 'Failure to close', prob_noz_ftc),
            ComponentFailure('Manual valve', 'Failure to close', prob_mvalve_ftc),
            ComponentFailure('Solenoid valves', 'Failure to close', prob_svalve_ftc),
            ComponentFailure('Solenoid valves', 'Common-cause failure', prob_svalve_ccf),
            ComponentFailure('Pressure-relief valve', 'Failure to open', prob_prvalve_fto),
            ComponentFailure('Breakaway coupling', 'Failure to close', prob_coupl_ftc),
            ComponentFailure('Accident', 'Overpressure during fueling', prob_overp),
            ComponentFailure('Accident', 'Driveoff', prob_driveoff)
        ]
    )
