"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import numpy as np
from .component_failure import ComponentFailureSet
from .component_set import ComponentSet


def init_leak_results(leak_sizes, f_release_overrides, component_sets, mass_flow_leak_size=None, mass_flow=None):
    """
    Creates LeakSizeResult objects for each leak size and obtains leak frequency data for each.
    Release frequencies are sum of frequencies for all components at that leak size.

    Parameters
    ----------
    leak_sizes : list(floats) or None
        leak sizes (% area of pipe)

    f_release_overrides : list(floats, None) or None
        Optional release frequency values which override those calculated by HyRAM

    component_sets : list(component_set)
        list of component set objects for each leak size.

    mass_flow_leak_size : {1, 10, 100, 1000, 10000}, optional
        Leak size (as hundredth of percent) of specified mass flow rate, assuming unchoked flow.

    mass_flow : float, optional
        [kg/s] Mass flow rate for specified leak size, if unchoked

    Returns
    -------
    leak_results: list(LeakSizeResult)
        list of leak size result objects ordered by size

    """
    if leak_sizes is None:
        leak_sizes = [0.01, 0.10, 1, 10, 100]

    # always included 100% size
    leak_sizes.sort()
    if leak_sizes[-1] != 100:
        leak_sizes.append(100)

    if f_release_overrides is None:
        f_release_overrides = [None] * len(leak_sizes)

    if mass_flow is not None:
        perc_leak_sizes = np.array([1, 10, 100, 1000, 10000])
        if mass_flow_leak_size not in perc_leak_sizes:
            raise ValueError(f'{mass_flow_leak_size} Leak size for unchoked mass flow not recognized')
        base_mass_flow = mass_flow / float(mass_flow_leak_size)
        mass_flows = base_mass_flow * perc_leak_sizes
    else:
        mass_flows = [None] * len(leak_sizes)

    leak_results = []
    for i, size in enumerate(leak_sizes):
        leak_result = LeakSizeResult(size, f_release_overrides[i], component_sets, mass_flow_override=mass_flows[i])
        leak_results.append(leak_result)

    return leak_results


class LeakSizeResult:
    """
    Represents leak release, including all components, at specified size.

    Parameters
    ----------
    leak_size : float
        Leak size as fraction of total area.

    f_release_override : float, optional
        Override indicating total frequency of release.

    component_sets : [ComponentSet]
        Component leak probability data for this leak size.

    mass_flow_override : float, optional
        Override indicating unchoked mass flow [kg/s].

    """
    leak_size: float
    mass_flow_rate: float or None = None
    mass_flow_override: float or None = None  # override for unchoked flow
    leak_diam: float or None = None
    f_release: float = 0  # Total release frequency for this leak size
    use_override: bool = False  # True if release frequency is set by user

    list_event_names: list
    list_event_keys: list
    list_p_events: list
    list_avg_events: list
    list_pll_contrib: list
    event_dicts: list  # list of dicts e.g. {id, label, prob, events, pll}
    f_component_leaks = {}

    # 100% Leak Size properties (None for all other sizes)
    f_failure: float or None = None  # Total vehicle & shutdown failure frequency
    use_failure_override: bool = False  # True if failure frequency is set by user
    failure_set: ComponentFailureSet or None = None

    def __init__(self, leak_size: float, f_release_override: float or None, component_sets: [ComponentSet], mass_flow_override=None):
        self.list_event_names = []
        self.list_event_keys = []
        self.list_p_events = []
        self.list_avg_events = []
        self.list_pll_contrib = []
        self.event_dicts = []
        self.f_component_leaks = {}

        self.leak_size = leak_size
        self.mass_flow_override = mass_flow_override

        # Calculate leak frequency from component releases or from override, if given.
        if f_release_override is not None:
            self.use_override = True
            self.f_release = f_release_override
            for comp_set in component_sets:
                self.f_component_leaks[comp_set.category] = 0

        else:
            for comp_set in component_sets:
                component_leak_frequency = comp_set.get_leak_frequency(self.leak_size)
                self.f_component_leaks[comp_set.category] = component_leak_frequency
                self.f_release += component_leak_frequency

    def _set_rounded_attr(self, attr_name, val):
        """ Helper func to assign probability parameters """
        if val is not None:
            setattr(self, attr_name, np.around(val, 20))

    def set_failure_set(self, failure_set):
        """
        Updates failure frequency, release frequency, and related flags from failure set.
        NOTE: only used for 100% leak.

        Parameters
        ----------
        failure_set : ComponentFailureSet

        """
        self.failure_set = failure_set
        self.use_failure_override = failure_set.use_override

        self.f_failure = failure_set.f_failure  # equals override, if provided
        if not self.use_override:
            self.f_release += failure_set.f_failure

    def to_dict(self):
        """ Creates shallow copy of parameters without failure set class, for easy JSON serialization. """
        result = vars(self).copy()
        result['list_p_events'] = list(self.list_p_events)
        result['f_component_leaks'] = self.f_component_leaks

        # Pass through probabilities and frequencies of component failures, if an override isn't used.
        if self.failure_set is not None:
            if not self.use_failure_override:
                result |= {
                    'p_overp_rupture': self.failure_set.p_overp_rupture,
                    'f_overp_rupture': self.failure_set.f_overp_rupture,
                    'p_driveoff': self.failure_set.p_driveoff,
                    'f_driveoff': self.failure_set.f_driveoff,
                    'p_nozzle_release': self.failure_set.p_nozzle_release,
                    'f_nozzle_release': self.failure_set.f_nozzle_release,
                    'p_sol_valves_ftc': self.failure_set.p_sol_valves_ftc,
                    'f_sol_valves_ftc': self.failure_set.f_sol_valves_ftc,
                    'p_mvalve_ftc': self.failure_set.p_mvalve_ftc,
                    'f_mvalve_ftc': self.failure_set.f_mvalve_ftc
                }
            result.pop('failure_set', None)

        return result

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

    def __repr__(self):
        return f"{self.leak_size:06.2f}% Leak Release"

    def __str__(self):
        # TODO (Cianan): does anyone use the table string generators below?
        override_str = "(override)" if self.use_override else ""
        failure_str = f", vehicle failure probabilities: {self.f_failure:.6}" if self.leak_size == 100 else ""
        msg = f"{self.leak_size:06.2f}% Leak release - total leak frequency {override_str} {self.f_release:.3g}{failure_str}"
        return msg
