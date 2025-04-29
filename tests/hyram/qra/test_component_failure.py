"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

from hyram.qra import component_failure

class ComponentFailureTestCase(unittest.TestCase):
    """
    Evaluate creation of ComponentFailure and ComponentFailureSet objects
    """

    def setUp(self):
        self.num_vehicles = 3
        self.daily_fuelings = 2
        self.vehicle_days = 10
        self.prob_noz_popoff = 1e-8
        self.prob_noz_ftc = 2e-3
        self.prob_mvalve_ftc = 1e-3
        self.prob_svalve_ftc = 2e-3
        self.prob_svalve_ccf = 1e-4
        self.prob_prvalve_fto = 1e-7
        self.prob_coupl_ftc = 1e-4
        self.prob_overp = 1e-5
        self.prob_driveoff = 5e-5

        self.failures = [
            component_failure.ComponentFailure(
                'Nozzle', 'Pop-off', self.prob_noz_popoff),
            component_failure.ComponentFailure(
                'Nozzle', 'Failure to close', self.prob_noz_ftc),
            component_failure.ComponentFailure(
                'Manual valve', 'Failure to close', self.prob_mvalve_ftc),
            component_failure.ComponentFailure(
                'Solenoid valves', 'Failure to close', self.prob_svalve_ftc),
            component_failure.ComponentFailure(
                'Solenoid valves', 'Common-cause failure', self.prob_svalve_ccf),
            component_failure.ComponentFailure(
                'Pressure-relief valve', 'Failure to open', self.prob_prvalve_fto),
            component_failure.ComponentFailure(
                'Breakaway coupling', 'Failure to close', self.prob_coupl_ftc),
            component_failure.ComponentFailure(
                'Accident', 'Overpressure during fueling', self.prob_overp),
            component_failure.ComponentFailure(
                'Accident', 'Driveoff', self.prob_driveoff)
        ]

    def test_componentfailure_creation(self):
        component = 'Nozzle'
        mode = 'Failure to close'
        probability = 0.01

        test_cf = component_failure.ComponentFailure(
            component=component,
            mode=mode,
            value=probability)

        self.assertEqual(test_cf.component, component)
        self.assertEqual(test_cf.mode, mode)
        self.assertEqual(test_cf.value, probability)

    def test_componentfailure_str(self):
        component = 'Nozzle'
        mode = 'Failure to close'
        probability = 0.01

        test_cf = component_failure.ComponentFailure(
            component=component,
            mode=mode,
            value=probability)
        exp_str = 'Component failure: Nozzle Failure to close | 1%'

        self.assertEqual(str(test_cf), exp_str)

    def test_componentfailureset_creation(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)

        self.assertEqual(test_cfs.failures, self.failures)
        self.assertTrue(hasattr(test_cfs, 'freq_failure'))

    def test_componentfailureset_freq_failure(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)
        # regression test value based on existing develop branch
        exp_freq_failure = 3.120610200048e-07

        self.assertAlmostEqual(test_cfs.freq_failure, exp_freq_failure)

    def test_componentfailureset_to_dict(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)
        exp_keys = ['prob_nozzle_release', 'prob_mvalve_ftc',
                    'prob_svalves_ftc', 'prob_driveoff', 'freq_overp_rupture',
                    'freq_driveoff', 'freq_nozzle_release', 'freq_svalves_ftc',
                    'freq_mvalve_ftc', 'freq_failure']

        self.assertEqual(list(test_cfs.to_dict().keys()), exp_keys)

    def test_componentfailureset_calc_prob_svalves_ftc(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)
        test_prob_svalves_ftc = test_cfs.calc_prob_svalves_ftc(self.failures)
        exp_prob_svalves_ftc = (self.prob_svalve_ftc**3
                                   + self.prob_svalve_ccf)

        self.assertAlmostEqual(test_prob_svalves_ftc, exp_prob_svalves_ftc)

    def test_componentfailureset_calc_prob_driveoff(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)
        test_prob_driveoff = test_cfs.calc_prob_driveoff(self.failures)
        exp_prob_driveoff = self.prob_driveoff * self.prob_coupl_ftc

        self.assertEqual(test_prob_driveoff, exp_prob_driveoff)

    def test_componentfailureset_calc_freq_overp_rupture(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)
        num_fuelings = (self.num_vehicles
                      * self.daily_fuelings
                      * self.vehicle_days)
        test_freq_overp_rupture = test_cfs.calc_freq_overp_rupture(
            self.failures, num_fuelings)
        exp_freq_overp_rupture = (num_fuelings
                                * self.prob_overp
                                * self.prob_prvalve_fto)

        self.assertEqual(test_freq_overp_rupture, exp_freq_overp_rupture)

    def test_componentfailureset_str(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)
        exp_str = 'Fuel failure: 3.120610200048e-07'

        self.assertEqual(str(test_cfs), exp_str)

    def test_componentfailureset_repr(self):
        test_cfs = component_failure.ComponentFailureSet(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            failures=self.failures)
        exp_repr = ('Component Failure Set - ' +
                    'Component failure: Nozzle Pop-off | 1e-06% - ' +
                    'Component failure: Nozzle Failure to close | 0.2% - ' +
                    'Component failure: Manual valve Failure to close | 0.1% - ' +
                    'Component failure: Solenoid valves Failure to close | 0.2% - ' +
                    'Component failure: Solenoid valves Common-cause failure | 0.01% - ' +
                    'Component failure: Pressure-relief valve Failure to open | 1e-05% - ' +
                    'Component failure: Breakaway coupling Failure to close | 0.01% - ' +
                    'Component failure: Accident Overpressure during fueling | 0.001% - ' +
                    'Component failure: Accident Driveoff | 0.005% - ' +
                    'Fuel failure: 3.12e-05%')

        self.assertEqual(repr(test_cfs), exp_repr)

    def test_create_failure_set(self):
        test_cfs = component_failure.create_failure_set(
            num_vehicles=self.num_vehicles,
            daily_fuelings=self.daily_fuelings,
            vehicle_days=self.vehicle_days,
            prob_noz_popoff=self.prob_noz_popoff,
            prob_noz_ftc=self.prob_noz_ftc,
            prob_mvalve_ftc=self.prob_mvalve_ftc,
            prob_svalve_ftc=self.prob_svalve_ftc,
            prob_svalve_ccf=self.prob_svalve_ccf,
            prob_prvalve_fto=self.prob_prvalve_fto,
            prob_coupl_ftc=self.prob_coupl_ftc,
            prob_overp=self.prob_overp,
            prob_driveoff=self.prob_driveoff)
        exp_str = 'Fuel failure: 3.120610200048e-07'

        self.assertTrue(isinstance(
            test_cfs, component_failure.ComponentFailureSet))
        self.assertEqual(str(test_cfs), exp_str)