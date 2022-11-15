"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


def calc_pll(frequency, consequence):
    """
    Calculation of potential loss of life (PLL)

    Parameters
    ----------
    frequency : float
        Frequency of occurrence (events per year)
    
    consequence : float
        Fatal consequences of occurrence (fatalities)
    
    Returns
    -------
    pll : float
        Potential loss of life (fatalities per year)
    """
    pll = frequency * consequence
    return pll


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
    if pll == 0:
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
    plls = []
    for frequency, consequence in zip(frequencies, consequences):
        pll = calc_pll(frequency, consequence)
        plls.append(pll)
    return plls


def calc_risk_contributions(risk_values):
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
    total_risk = sum(risk_values)
    risk_contributions = []
    for risk_value in risk_values:
        if total_risk > 0:
            risk_contribution = risk_value / total_risk
        else:
            risk_contribution = 0
        risk_contributions.append(risk_contribution)
    return total_risk, risk_contributions
