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

import numpy as np
from scipy.stats import norm

from ..utilities import misc_utils


def thermal_dose(heat_flux, exposure_time):
    """
    Compute thermal dose for given time.

    Parameters
    -------------
    heat_flux : float
        heat flux intensity (W/m^2)
    exposure_time : float
        duration of exposure (s)

    Returns
    ---------
    float
        Thermal dose in (W/m^2)^4/3 s
    """
    return exposure_time * heat_flux ** (4. / 3)


"""
THERMAL Fatality probit models
TRM p. 20
Ethan: Equations copied from OverpressureFatality.m and ThermalFataility.m- may need some work.
I found and corrected at least one error (lung_HSE---I think, although the HSE paper I was looking
at seems to have a lot of errors.).

other notes from OverpressureFatality.m:
TNO Lung hemorrhage requires the person's mass, so we don't calculate it
TNO whole body impact model produces lower probabilities than head
impact model, which means head impact fatalities will dominate whole
body impact fatalities. So we don't include TNO whole body fatalities.
"""


def compute_thermal_fatality_prob(model_ref, heat_flux, exposure_time, mean=5):
    """
    Calculate probability of fatality from thermal source using user-specified model.

    Parameters
    ----------
    model_ref : str
        reference to internal thermal probit model function to apply (see below)
    heat_flux : float
        heat flux intensity (W/m^2)
    exposure_time : float
        duration of exposure (s)
    mean : int
        Default value of 5 (TRM p. 19 footnote) avoids negative values, consistent with published models.

    Returns
    -------
    prob : float
        Probability of fatality

    """
    cleaned_id = parse_thermal_model(model_ref)
    model = PROBIT_THERMAL_CHOICES[cleaned_id]

    val = model(heat_flux, exposure_time)

    return norm.cdf(val, loc=mean)


def thermal_eisenberg(heat_flux, exposure_time):
    """
    Eisenberg - thermal exposure

    Parameters
    -------------
    heat_flux : float
        heat flux intensity (W/m^2)
    exposure_time : float
        duration of exposure (s)

    Returns
    ---------
    probability of fatality
    """
    return -38.48 + 2.56 * np.log(thermal_dose(heat_flux, exposure_time))


def thermal_tsao(heat_flux, exposure_time):
    """
    Tsao & Perry - thermal exposure

    Parameters
    -------------
    heat_flux : float
        heat flux intensity (W/m^2)
    exposure_time : float
        duration of exposure (s)

    Returns
    ---------
    probability of fatality
    """
    return -36.38 + 2.56 * np.log(thermal_dose(heat_flux, exposure_time))


def thermal_tno(heat_flux, exposure_time):
    """
    TNO - thermal exposure

    Parameters
    -------------
    heat_flux : float
        heat flux intensity (W/m^2)
    exposure_time : float
        duration of exposure (s)

    Returns
    ---------
    probability of fatality
    """
    return -37.23 + 2.56 * np.log(thermal_dose(heat_flux, exposure_time))


def thermal_lees(heat_flux, exposure_time):
    """
    Lees - thermal exposure

    Parameters
    -------------
    heat_flux : float
        heat flux intensity (W/m^2)
    exposure_time : float
        duration of exposure (s)

    Returns
    ---------
    probability of fatality
    """
    return -29.02 + 1.99 * np.log(0.5 * thermal_dose(heat_flux, exposure_time))


"""
OVERPRESSURE MODELS
TRM p. 21 
"""


def compute_overpressure_fatality_prob(model_ref, overp, impulse=None, mean=5,
                                       fragment_mass=None, velocity=None, total_mass=None):
    """
    Calculate probability of fatality from overpressure using user-specified model.

    Parameters
    ----------
    model_ref : str
        reference to internal probit model function to use (from available functions)
    overp : float
        Peak overpressure (Pa)
    impulse : float
        Impulse of shock wave (Pa*s)
    mean : int
        Default value of 5 (TRM p. 19 footnote) avoids negative values, consistent with published models.
    fragment_mass : float
        For debris method; mass of (individual) fragments (kg)
    velocity : float
        For debris method; debris velocity (m/s)
    total_mass : float
        For debris method; total mass of all debris [kg]

    Returns
    -------
    prob : float
        Probability of fatality

    """
    cleaned_id = parse_overp_model(model_ref)
    model = PROBIT_OVERP_CHOICES[cleaned_id]
    val = model(overp=overp, impulse=impulse, fragment_mass=fragment_mass, velocity=velocity, total_mass=total_mass)
    prob = norm.cdf(val, loc=mean)
    return prob


def overp_eisenberg(overp, **kwargs):
    """
    Eisenberg - Lung hemorrhage

    Parameters
    -------------
    overp : float
        Peak overpressure (Pa)

    Returns
    ---------
    probability of fatality
    """
    return -77.1 + 6.91 * np.log(overp)


def overp_hse(overp, **kwargs):
    """
    HSE - Lung hemorrhage
    Ethan: changed from OverpressureFatality.m.  Coefficients in report were for units of psig.
    value changes from 1.47 -> 5.13 for units of barg, so I did this and then converted P
    to bar within the logarithm

    Parameters
    -------------
    overp : float
        Peak overpressure (Pa)

    Returns
    ---------
    probability of fatality
    """
    return 5.13 + 1.37 * np.log(overp * 1e-5)


def overp_tno_head(overp, impulse, **kwargs):
    """
    TNO - Head impact

    Parameters
    -------------
    overp : float
        Peak overpressure (Pa)
    impulse : float
        Impulse of shock wave (Pa*s)

    Returns
    ---------
    probability of fatality
    """
    if impulse == 0. or overp == 0.:
        val = -np.inf
    else:
        val = 5. - 8.49 * np.log((2430. / overp) + 4.e8 / (overp * impulse))
    return val


def overp_tno_struct_collapse(overp, impulse, **kwargs):
    """
    TNO - Structural collapse

    Parameters
    -------------
    overp : float
        Peak overpressure (Pa)
    impulse: impulse of shock wave (Pa*s)

    Returns
    ---------
    probability of fatality
    """

    if impulse == 0. or overp == 0.:
        val = -np.inf
    else:
        val = 5. - 0.22 * np.log((40000. / overp) ** 7.4 + (460. / impulse) ** 11.3)
    return val


def debris(fragment_mass, velocity, total_mass, **kwargs):
    """
    TNO - Debris impact

    Parameters
    -------------
    fragment_mass : float
        Enter mass of (individual) fragments (kg)
    velocity : float
        Debris velocity (m/s)
    total_mass : float
        Total mass of all debris [kg]

    Returns
    ---------
    probability of fatality
    """
    val = ((fragment_mass >= 4.5) * (-13.19 + 10.54 * np.log(velocity)) +
           (fragment_mass >= 0.1) * (-17.56 + 5.3 * np.log(0.5 * total_mass * velocity ** 2)) +
           (fragment_mass >= 0.001) * (-29.15 + 2.1 * np.log(total_mass * velocity ** 5.115)))
    return val


# Reference to fatality models.
#   key: 3-char internal reference
#   val: function reference
PROBIT_THERMAL_CHOICES = {
    'eise': thermal_eisenberg,
    'tsao': thermal_tsao,
    'tno': thermal_tno,
    'lees': thermal_lees,
}

# Note: keys are unique with above dict to ensure no accidental overlap
# Note2: these are referenced in generic compute func above. If adding new method, update the model call as well.
PROBIT_OVERP_CHOICES = {
    'leis': overp_eisenberg,
    'lhse': overp_hse,
    'head': overp_tno_head,
    'coll': overp_tno_struct_collapse,
    'debr': debris,
}


def parse_thermal_model(name):
    """ Determine model ID from string name """
    cleaned = misc_utils.clean_name(name)  # alphanumeric lower-case

    if cleaned in ['eise', 'eisenberg', 'eis', 'eisen']:
        model_id = 'eise'
    elif cleaned in ['tsao', 'tsa']:
        model_id = 'tsao'
    elif cleaned in ['tno', 'tn']:
        model_id = 'tno'
    elif cleaned in ['lees', 'lee', 'le']:
        model_id = 'lees'
    else:
        raise ValueError("Thermal model name {} not recognized".format(cleaned))

    return model_id


def parse_overp_model(name):
    """ Determine model ID from string name """
    cleaned = misc_utils.clean_name(name)  # alphanumeric lower-case

    if cleaned in ['leis', 'lung_eisenberg', 'lungeisenberg', 'lunge', 'lung_eis', 'elh']:
        model_id = 'leis'
    elif cleaned in ['lhse', 'lung_hse', 'lunghse', 'lungh', 'lhs']:
        model_id = 'lhse'
    elif cleaned in ['head_impact', 'head', 'headimpact', 'hea']:
        model_id = 'head'
    elif cleaned in ['col', 'collapse', 'coll']:
        model_id = 'coll'
    elif cleaned in ['debris', 'deb', 'debr', 'debri']:
        model_id = 'debr'
    else:
        raise ValueError("Probit overpressure model name {} not recognized".format(cleaned))

    return model_id
