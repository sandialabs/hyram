"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from ..component_failure import ComponentFailureSet, ComponentFailure


default_fueling_parameters = {
    'Nozzle': {
        'mode': ['Pop-off', 'Failure to close'],
        'distribution_type': ['beta', 'deterministic'],
        'distribution_parameters': [
            {'a': 0.5, 'b': 610415.5},
            {'value': 0.002}
        ]
    },
    'Manual valve': {
        'mode': 'Failure to close',
        'distribution_type': 'deterministic',
        'distribution_parameters': {
            'value': 0.001
        }
    },
    'Solenoid valves': {
        'mode': ['Failure to close', 'Common-cause failure'],
        'distribution_type': ['deterministic', 'deterministic'],
        'distribution_parameters': [
            {'value': 0.002},
            {'value': 0.000127659574468085}
        ]
    },
    'Pressure-relief valve': {
        'mode': 'Failure to open',
        'distribution_type': 'log_normal',
        'distribution_parameters': {
            'mu': -11.7359368859313,
            'sigma': 0.667849415603714
        }
    },
    'Breakaway coupling': {
        'mode': 'Failure to close',
        'distribution_type': 'beta',
        'distribution_parameters': {
            'a': 0.5, 'b': 5031
        }
    },
    'Accident': {
        'mode': ['Overpressure during fueling', 'Driveoff'],
        'distribution_type': ['beta', 'beta'],
        'distribution_parameters': [
            {'a': 3.5, 'b': 310289.5},
            {'a': 31.5, 'b': 610384.5}
        ]
    }
}

default_failure_set = ComponentFailureSet(
    num_vehicles=20,
    daily_fuelings=2,
    vehicle_days=250,
    failures=[
        ComponentFailure('Nozzle', 'Pop-off', 8.191135225813216e-07),
        ComponentFailure('Nozzle', 'Failure to close', 0.002),
        ComponentFailure('Manual valve', 'Failure to close', 0.001),
        ComponentFailure('Solenoid valves', 'Failure to close', 0.002),
        ComponentFailure('Solenoid valves', 'Common-cause failure', 0.000127659574468085),
        ComponentFailure('Pressure-relief valve', 'Failure to open', 8.001057112661898e-06),
        ComponentFailure('Breakaway coupling', 'Failure to close', 9.937394415184339e-05),
        ComponentFailure('Accident', 'Overpressure during fueling', 1.1279661481245145e-05),
        ComponentFailure('Accident', 'Driveoff', 5.160415192262326e-05)
    ]
)
