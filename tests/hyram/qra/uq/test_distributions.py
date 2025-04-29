"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest
import numpy as np

from hyram.qra.uq import distributions


class DistributionTestCase(unittest.TestCase):
    """class for unit tests of distributions module"""
    def setUp(self) -> None:
        """function to specify common inputs for distribution module"""

    def test_specify_distribution(self):
        dist1 = distributions.specify_distribution({
            'distribution_type': 'normal',
            'name': 'dist1',
            'uncertainty_type': 'aleatory',
            'mean': .5,
            'std_deviation': .3})
        dist2 = distributions.NormalDistribution(name='dist1',
                                                 uncertainty_type='aleatory',
                                                 mean=.5,
                                                 std_deviation=.3)

        self.assertTrue(isinstance(dist1, distributions.NormalDistribution))
        self.assertEqual(repr(dist1), repr(dist2))

    def test_convert_distribution_to_deterministic(self):
        """Check that distribution distributions are changed to deterministic
        with the mean value
        """

        uncertain_dist = distributions.NormalDistribution(
            name='dist1',
            uncertainty_type='aleatory',
            mean=0.5,
            std_deviation=.3)
        deterministic_dist = distributions.DeterministicCharacterization(
            name='dist2',
            value=4)
        input_distributions = {'dist1': uncertain_dist,
                               'dist2': deterministic_dist}
        test_distributions = (
            distributions.convert_distributions_to_deterministic(
                input_distributions))
        exp_values = [0.5, 4]

        self.assertTrue(all(
            [isinstance(dist, distributions.DeterministicCharacterization)
             for dist in test_distributions.values()]))
        self.assertListEqual(
            [dist.value for dist in test_distributions.values()],
            exp_values)

    def test_distribution_str(self):
        """unit test of printing out distribution information"""
        uncertain_var1 = distributions.NormalDistribution(
            name='test var1',
            uncertainty_type='aleatory',
            mean=0.5,
            std_deviation=.3)
        deterministic_var2 = distributions.DeterministicCharacterization(
            name='test var2',
            value=4)
        exp_uncertain_str = ("test var1 is a probabilistic variable " +
                                "represented with a norm distribution and " +
                                "parameters {'loc': 0.5, 'scale': 0.3}")
        exp_deterministic_str = ('test var2 is a deterministic variable ' +
                                    'with value 4')

        self.assertEqual(str(uncertain_var1), exp_uncertain_str)
        self.assertEqual(str(deterministic_var2), exp_deterministic_str)

    def test_distribution_repr(self):
        uncertain_var1 = distributions.NormalDistribution(
            name='test var1',
            uncertainty_type='aleatory',
            mean=0.5,
            std_deviation=.3)
        deterministic_var2 = distributions.DeterministicCharacterization(
            name='test var2',
            value=4)
        exp_uncertain_repr = ('test var1, norm, mean = 0.5, ' +
                              'aleatory uncertainty, parameters: ' +
                              'loc=0.5, scale=0.3')
        exp_deterministic_repr = 'test var2, deterministic, value=4'

        self.assertEqual(repr(uncertain_var1), exp_uncertain_repr)
        self.assertEqual(repr(deterministic_var2), exp_deterministic_repr)

    def test_generate_values(self):
        deterministic_var = distributions.DeterministicCharacterization(
            name='test var1',
            value=4)
        num_samples = 5
        test_values = deterministic_var.generate_values(num_samples)
        exp_values = np.array([4.0, 4.0, 4.0, 4.0, 4.0])

        self.assertTrue(np.array_equal(test_values, exp_values))

    def test_plot_distribution_deterministic(self):
        alt_name = 'alt name'
        deterministic_var = distributions.DeterministicCharacterization(
            name='test var1',
            value=4)
        deterministic_var.plot_distribution()
        deterministic_var.plot_distribution(alt_name)

    def test_generate_samples_normal(self):
        num_samples = 10
        uncertain_var = distributions.NormalDistribution(
            name='test var',
            uncertainty_type='aleatory',
            mean=.5,
            std_deviation=.3)
        exp_mean = 0.5
        exp_std = 0.3

        self.assertEqual(len(uncertain_var.generate_samples(num_samples)),
                         num_samples)
        self.assertEqual(uncertain_var.distribution.mean(), exp_mean)
        self.assertEqual(uncertain_var.distribution.std(), exp_std)

    def test_generate_samples_lognormal(self):
        num_samples = 7
        uncertain_var = distributions.LognormalDistribution(
            name='test var',
            uncertainty_type='epistemic',
            mu=.5,
            sigma=.3)
        exp_median = np.exp(0.5)
        exp_variance = (np.exp(.3**2) - 1)*(np.exp(2*.5 + .3**2))

        self.assertEqual(len(uncertain_var.generate_samples(num_samples)),
                         num_samples)
        self.assertEqual(uncertain_var.distribution.median(), exp_median)
        self.assertAlmostEqual(uncertain_var.distribution.var(),
                               exp_variance)

    def test_generate_samples_trunc_normal(self):
        num_samples = 100
        uncertain_var = distributions.TruncatedNormalDistribution(
            name='test var',
            uncertainty_type='aleatory',
            mean=.5,
            std_deviation=.3,
            lower_bound=.3,
            upper_bound=.7)
        random_samples = uncertain_var.generate_samples(num_samples)
        self.assertEqual(len(random_samples), num_samples)
        self.assertTrue((random_samples > .3).all())
        self.assertTrue((random_samples < .7).all())

    def test_generate_samples_trunc_lognormal(self):
        num_samples = 120
        uncertain_var = \
            distributions.TruncatedLognormalDistribution(
                name='test var4',
                uncertainty_type='aleatory',
                mu=-.3,
                sigma=.3,
                lower_bound=.4,
                upper_bound=1.2)
        random_samples = uncertain_var.generate_samples(num_samples)
        self.assertEqual(len(random_samples), num_samples)
        self.assertTrue((random_samples > .4).all())
        self.assertTrue((random_samples < 1.2).all())

    def test_generate_samples_uniform(self):
        num_samples = 13
        uncertain_var = distributions.UniformDistribution(
            name='test var',
            lower_bound=-1,
            upper_bound=3,
            uncertainty_type='epistemic')
        exp_mean = 1
        exp_std = np.sqrt(4/3)

        self.assertEqual(len(uncertain_var.generate_samples(num_samples)),
                         num_samples)
        self.assertEqual(uncertain_var.distribution.mean(), exp_mean)
        self.assertEqual(uncertain_var.distribution.std(), exp_std)

    def test_plot_distribution_uncertain(self):
        alt_name = 'alt_name'
        plot_limits = [-2, 2]
        uncertain_var = distributions.NormalDistribution(
            name='test var',
            uncertainty_type='aleatory',
            mean=.5,
            std_deviation=.3)
        uncertain_var.plot_distribution()
        uncertain_var.plot_distribution(alternative_name=alt_name,
                                        plot_limits=plot_limits)
