"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

import logging
from collections import OrderedDict

import numpy as np
import pandas as pd

from .distributions import LogNormDistribution

log = logging.getLogger('hyram.qra')


class LeakSizeResult:
    """ Convenience class to hold analysis results for specific leak size (e.g. 1%)
    """

    def __init__(self, leak_size):
        self.leak_size = leak_size
        self.descrip = "{:06.2f}% Release".format(leak_size)

        # User-provided frequency value for vehicle fueling failures. Used in 100% release only.
        # All other releases should be set to 'None'. If no override, should be -1.
        self.fueling_fail_freq_override = None

        # User-provided value for frequency of this release. -1 if not used
        self.release_freq_override = -1.

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

        self.p_jetfire = None
        self.p_explos = None
        self.p_no_ign = None
        self.p_shutdown = None

        self.shutdown_avg_events = None
        self.jetfire_avg_events = None
        self.explos_avg_events = None
        self.no_ign_avg_events = None

        self.jetfire_pll_contrib = None
        self.explos_pll_contrib = None

        self.fueling_fail_freq = None
        self.component_leak_freqs = {}
        self.total_release_freq = 0.

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
        if not failure_set.use_override:
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
        return self.p_shutdown + self.p_jetfire + self.p_explos + self.p_no_ign

    def sum_events(self):
        return self.shutdown_avg_events + self.jetfire_avg_events + self.explos_avg_events + self.no_ign_avg_events

    def sum_plls(self):
        return self.explos_pll_contrib + self.jetfire_pll_contrib

    def get_result_dicts(self):
        """ Get dict of event data (for pretty-printing in table) """
        return [
            {'label': 'Shutdown', 'prob': self.p_shutdown, 'events': self.shutdown_avg_events, 'pll': 0},
            {'label': 'Jetfire', 'prob': self.p_jetfire, 'events': self.jetfire_avg_events,
             'pll': self.jetfire_pll_contrib},
            {'label': 'Explosion', 'prob': self.p_explos, 'events': self.explos_avg_events,
             'pll': self.explos_pll_contrib},
            {'label': 'No ignition', 'prob': self.p_no_ign, 'events': self.no_ign_avg_events, 'pll': 0},
            {'label': 'TOTAL', 'prob': self.sum_probabilities(), 'events': self.sum_events(), 'pll': self.sum_plls()}
        ]

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
    Random Hydrogen release (leak) chance, defined by an orifice diameter and probability of occurrence.

    Parameters
    ----------
    description : str
        Optional description of leak instance.
    size : float
        Release size, as percentage of total orifice diameter. Current hard-coded options are 0.01, 0.1, 1, 10, 100
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
        if size < 0. or size > 100.:
            raise ValueError("Leak size {} is not valid. Size must be between 0 and 100%".format(size))
        if mu in [None, "", np.nan] or sigma in [None, "", np.nan]:
            msg = ("Leak probability parameters for size {}% invalid. Must include valid mu and sigma."
                   "Passed values for mu/sigma/mean/variance are {}, {}."
                   ).format(size, mu, sigma)
            raise ValueError(msg)

        self.description = str(description)

        # Log normal distribution object for this leak size if mean/variance not given
        self.size = size
        self.mu = mu
        self.sigma = sigma
        self.prob = self.p = self.probability = LogNormDistribution(mu=self.mu, sigma=self.sigma)
        self.mean = self.prob.mean
        self.variance = self.prob.variance

    def _repr_html_(self):
        """
        Output leak parameters as string in HTML format (via dataframe).
        """
        df = pd.DataFrame(columns=['description', 'size (%)', 'probability'])
        df.loc[0] = [self.description, self.size, self.prob]
        return df.to_html(index=False)

    def __str__(self):
        """
        Output leak as string structured by dataframe.
        """
        df = pd.DataFrame(columns=['description', 'size (%)', 'probability'])
        df.loc[0] = [self.description, self.size, self.prob]
        param_str = 'Leak {}: {} mu, {} sigma, {} mean, {} var'.format(self.size, self.mu, self.sigma,
                                                                       self.mean, self.variance)
        df_str = df.to_string(index=False)
        return "{}\n{}".format(param_str, df_str)
