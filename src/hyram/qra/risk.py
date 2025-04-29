"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import numpy as np


def calc_far(pll, total_occupants):
    """
    Calculation of fatal accident rate (FAR)

    Parameters
    ----------
    pll : float
        Potential loss of life (fatalities per year)

    total_occupants : float
        Number of exposed people (people)

    Returns
    -------
    far : float
        Fatal accident rate (fatalities per 10^8 person*hours)
    """
    hours_per_year = 8760  # based on 24 hours per day, 365 days per year
    if pll == 0 or total_occupants == 0:
        far = 0
    else:
        far = (pll * 1e8) / (total_occupants * hours_per_year)
    return far


def calc_air(far, exposed_hours_per_year):
    """
    Calculation of average individual risk (AIR)

    Parameters
    ----------
    far : float
        Fatal accident rate (fatalities per 10^8 person*hours)

    exposed_hours_per_year : float
        Number of hours the individual spends in the facility,
        e.g., 2,000 hours for a full time worker
        (40 hours per week, 50 weeks per year)

    Returns
    -------
    air : float
        Average individual risk (fatalities per year)
    """
    air = far * 1e-8 * exposed_hours_per_year
    return air


def calc_all_plls(frequencies, consequences):
    """
    Calculation of multiple PLL metrics for multiple
    frequency and fatality values

    Parameters
    ----------
    frequencies : list
        List of frequency values (units of per year)

    consequences : list
        List of fatality values (units of fatalities)

    Returns
    -------
    plls : list
        List of PLL values (units of fatalities per year)
    """
    return frequencies * consequences


def calc_risk_contributions(risk_values:np.ndarray):
    """
    Calculation of contribution to total risk from multiple risk values
    e.g., PLL

    Parameters
    ----------
    risk_values : list
        List of risk values

    Returns
    -------
    total_risk : float
        Sum total of risk from all inputs

    risk_contributions : list
        List of fractional contributions for each risk value
        in same order as input list
    """

    total_risk = risk_values.sum()
    if total_risk > 0:
        risk_contributions = risk_values / total_risk
    else:
        risk_contributions = np.zeros_like(risk_values)
    return total_risk, risk_contributions
