"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import logging

import numpy as np

from .distributions import LogNormDistribution

log = logging.getLogger('hyram.qra')


class LeakSizeResult:
    """ Convenience class to hold analysis results for specific leak size (e.g. 1%)
    """

    def __init__(self, leak_size):
        self.leak_size = leak_size
        self.descrip = "{:06.2f}% Release".format(leak_size)

        # User-provided frequency value for vehicle fueling failures. Used in 100% release only.
        # All other releases should be set to 'None'. If no override, should be -1
        self.fueling_fail_freq_override = None

        # User-provided value for frequency of this release. -1 if not used
        self.release_freq_override = -1

        # Other failures. Currently used in 100% release only
        self.p_overp_rupture = None
        self.f_overp_rupture = None

        self.p_driveoff = None
        self.f_driveoff = None

        self.p_nozzle_release = None
        self.f_nozzle_release = None

        self.p_sol_valves_ftc = None
        self.f_sol_valves_ftc = None

        self.p_mvalve_ftc = None
        self.f_mvalve_ftc = None

        self.mass_flow_rate = None
        self.leak_diam = None

        self.list_event_names = []
        self.list_event_keys = []
        self.list_p_events = []
        self.list_avg_events = []
        self.list_pll_contrib = []

        # Store list data in dict format for easier GUI consumption, e.g. {id, label, prob, events, pll}
        self.event_dicts = []

        self.fueling_fail_freq = None
        self.component_leak_freqs = {}
        self.total_release_freq = 0

    def _set_rounded_attr(self, attr_name, val):
        """ Helper func to assign probability parameters """
        if val is not None:
            setattr(self, attr_name, np.around(val, 20))

    def set_failures(self, failure_set):
        """
        Parameters
        ----------
        failure_set : ComponentFailureSet

        """
        if failure_set.use_override:
            self.fueling_fail_freq_override = failure_set.f_fueling_fail
        else:
            self.fueling_fail_freq_override = None
            self.p_overp_rupture = failure_set.p_overp_rupture
            self.f_overp_rupture = failure_set.f_overp_rupture
            self.p_driveoff = failure_set.p_driveoff
            self.f_driveoff = failure_set.f_driveoff
            self.p_nozzle_release = failure_set.p_nozzle_release
            self.f_nozzle_release = failure_set.f_nozzle_release
            self.p_sol_valves_ftc = failure_set.p_sol_valves_ftc
            self.f_sol_valves_ftc = failure_set.f_sol_valves_ftc
            self.p_mvalve_ftc = failure_set.p_mvalve_ftc
            self.f_mvalve_ftc = failure_set.f_mvalve_ftc

        self.fueling_fail_freq = failure_set.f_fueling_fail
        self.total_release_freq += failure_set.f_fueling_fail

    def get_component_freqs_str(self):
        return "\n".join(["{}: {:.3g}".format(key, val) for key, val in self.component_leak_freqs.items()])

    def sum_probabilities(self):
        return sum(self.list_p_events)

    def sum_events(self):
        return sum(self.list_avg_events)

    def sum_plls(self):
        return sum(self.list_pll_contrib)

    def get_result_dicts(self):
        """ Get dict of event data """
        result_dicts = []
        for i, name in enumerate(self.list_event_names):
            rdict = {'label': name,
                     'key': self.list_event_keys[i],
                     'prob': self.list_p_events[i],
                     'events': self.list_avg_events[i],
                     'pll': self.list_pll_contrib[i]}
            result_dicts.append(rdict)

        total_dict = {'label': 'TOTAL', 'key': 'tot',
                      'prob': self.sum_probabilities(),
                      'events': self.sum_events(), 'pll': self.sum_plls()}
        result_dicts.append(total_dict)
        return result_dicts

    def get_vehicle_fail_probabilities(self):
        return [
            {'label': 'Overpressure rupture', 'prob': self.p_overp_rupture},
            {'label': 'Driveoff', 'prob': self.p_driveoff},
            {'label': 'Nozzle release', 'prob': self.p_nozzle_release},
            {'label': 'Solenoid valve FTC', 'prob': self.p_sol_valves_ftc},
            {'label': 'Manual valve FTC', 'prob': self.p_mvalve_ftc},
        ]

    def get_vehicle_failure_prob_table_string(self):
        """ Generate vehicle failure probability string in tabular format """
        tmpl_hdr = "{label:<20} | {prob:>15}"
        str = '{}\n'.format(tmpl_hdr.format(label="Failure", prob="Probability"))

        template = "{label:<20} | {prob:>15.10f}\n"
        for item in self.get_vehicle_fail_probabilities():
            str += template.format(**item)
        return str

    def get_result_table_string(self):
        """ Generate leak result data string in tabular format """
        tmpl_hdr = "{label:<12} | {events:>15} | {prob:>15} | {pll:>15}"
        str = '{}\n'.format(tmpl_hdr.format(label="Type", events="Avg Events", prob="Branch line P", pll="PLL Contrib"))

        template = "{label:<12} | {events:>15.10f} | {prob:>15.10f} | {pll:>15.10f}\n"
        for entry in self.get_result_dicts():
            str += template.format(**entry)
        return str

    def get_comp_freq_table_string(self):
        """ Generate component leak freq data string in tabular format """
        tmpl_hdr = "{label:<12} | {freq:>15}"
        str = '{}\n'.format(tmpl_hdr.format(label="Component", freq="Leak Freq"))

        template = "{label:<12} | {freq:>15.10f}\n"
        for key, val in self.component_leak_freqs.items():
            str += template.format(label=key, freq=val)
        return str

    def __repr__(self):
        return "{:06.2f}% Leak Release".format(self.leak_size)

    def __str__(self):
        result_table = self.get_result_table_string()

        if int(self.release_freq_override) == -1:
            msg = ("{:06.2f}% LEAK RELEASE\n" + "{}" +
                   "Total Leak Frequency: {:.3g}\n\n" +
                   "{}\n"
                   ).format(self.leak_size, self.get_comp_freq_table_string(), self.total_release_freq, result_table)
        else:
            msg = ("{:06.2f}% LEAK RELEASE\n" +
                   "H2 release frequency (override): {}\n" +
                   "Total leak frequency: {:.3g}\n" +
                   "{}\n"
                   ).format(self.leak_size, self.release_freq_override, self.total_release_freq, result_table)

        if self.fueling_fail_freq_override is not None:
            if int(self.fueling_fail_freq_override) == -1:
                # Display calculated vehicle failure frequencies
                msg += "Vehicle failure probabilities:\n{}\n".format(self.get_vehicle_failure_prob_table_string())
            else:
                # Display user-provided override
                msg += "\nVehicle failure parameter (override): {:.10f}".format(self.fueling_fail_freq_override)

        return msg


class Leak:
    """
    Random release (leak) chance, defined by an orifice diameter and probability of occurrence.

    Parameters
    ----------
    description : str
        Optional description of leak instance.
    size : float
        Release size, as percentage of total orifice diameter
    mu : float
        Mean of associated normal distribution
    sigma : float
        Standard deviation of associated normal distribution

    Attributes
    ----------
    size : float
    description : str
    probability : scipy rv_continuous object
        Object representation of lognormal distribution
    mean : float
        mean parameter of lognormal distribution of this leak probability
    variance : float
        variance parameter of lognormal distribution of this leak probability

    Methods
    ----------
    _distribution(mu, sigma, mean, variance)
        Compute lognormal distribution for chance of leak occurring.
        Also available as property.

    """

    def __init__(self, size, mu, sigma, description=''):
        if size < 0 or size > 100:
            raise ValueError("Leak size {} is not valid. Size must be between 0 and 100%".format(size))
        if mu in [None, "", np.nan] or sigma in [None, "", np.nan]:
            msg = ("Leak probability parameters for size {}% invalid. Must include valid mu and sigma."
                   "Passed values for mu/sigma/mean/variance are {}, {}."
                   ).format(size, mu, sigma)
            raise ValueError(msg)

        self.description = str(description)

        self.size = size
        self.mu = mu
        self.sigma = sigma

    def get_leak_freq_mean(self):
        leak_freq_dist = LogNormDistribution(mu=self.mu, sigma=self.sigma)
        return leak_freq_dist.mean

    def __str__(self):
        param_str = ('Leak: size: {}%'.format(self.size)
                     + ', mu: {}'.format(self.mu)
                     + ', sigma: {}'.format(self.sigma)
                     + ', description: {}'.format(self.description))
        return param_str
