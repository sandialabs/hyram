"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""

import numpy as np

from hyramgui.hygu.utils.helpers import InputStatus, ValidationResponse, get_num_str
from hyramgui.hygu.models.fields import NumField
from .enums import FailureDistribution


class DistributionField(NumField):
    """Unitless parameter described by distribution type and up to two parameters.

    Attributes
    ----------
    distr : {'beta', 'ln', 'ev'}
    pa : float
        Parameter of distribution function.
    pb : float
        Parameter of distribution function.
    str_display : str
        String representation of field, for display.

    Notes
    -----

    """
    _dtype = float
    descrip: str
    _distr: str
    _pa: float
    _pb: float

    def __init__(self, label, descrip, slug='', distr=FailureDistribution.ev, pa=0, pb=1, tooltip=None, label_rtf=None):
        super().__init__(label=label, slug=slug, value=1, label_rtf=label_rtf, tooltip=tooltip)
        self.descrip = descrip
        self._distr = distr
        self._pa = pa
        self._pb = pb

    @property
    def distr(self):
        return self._distr

    @distr.setter
    def distr(self, val: str):
        if val in FailureDistribution.keys:
            self._distr = val
            self.notify_changed()

    @property
    def pa(self):
        return self._pa

    @pa.setter
    def pa(self, val):
        self._pa = float(val)
        self.notify_changed()

    @property
    def pa_str(self):
        return get_num_str(self.pa)

    @property
    def pb(self):
        return self._pb

    @pb.setter
    def pb(self, val):
        self._pb = float(val)
        self.notify_changed()

    @property
    def pb_str(self):
        return get_num_str(self.pb)

    @property
    def str_display(self):
        """ Returns string-representation of parameter, including label."""
        result = f"{self.label}: pa {self.pa} pb {self.pb}"
        return result

    def check_valid(self) -> ValidationResponse:
        resp = super().check_valid()
        if resp.status != InputStatus.GOOD:
            return resp

        msg = ""
        status = InputStatus.ERROR if msg else InputStatus.GOOD
        return ValidationResponse(status, msg)

    def get_prepped_value(self):
        return {'pa': self.pa, 'pb': self.pb, 'distr': self.distr}

    def to_dict(self) -> dict:
        """Returns data representation with values in standard (raw) format.

        Returns
        -------
        dict
            Parsed parameter data with values in standard units.

        """
        result = super().to_dict()
        extra = {
            'descrip': self.descrip,
            'distr': self._distr,
            'pa': self._pa,
            'pb': self._pb,
        }
        result |= extra
        return result

    def from_dict(self, data: dict, notify_from_model=True, silent=False):
        """
        Overwrites all parameter data from contents of incoming dict.

        Parameters
        ----------
        data : dict
            parameter data in standard units with keys matching field names.

        notify_from_model : bool, default=True
            flag indicating the call originated from backend model. Triggers corresponding event.

        Notes
        -----
        Assumes all required properties are present.
        Min and max values stored as strings to accommodate infinity.

        """
        super().from_dict(data=data, notify_from_model=notify_from_model, silent=True)
        # Verify all extra data present
        # expected_keys = self.to_dict().keys()
        # for key in expected_keys:
        #     if key not in data:
        #         raise ValueError(f'Required key {key} not found in data {data}')

        self.descrip = data['descrip']
        self._distr = data['distr']
        self._pa = data['pa']
        self._pb = data['pb']

        if not silent:
            self.changed.notify(self)

            if notify_from_model:
                self.changed_by_model.notify(self)


class LognormField(NumField):
    """Unitless parameter described by mu, sigma, and median.

    Attributes
    ----------
    mu : float
        Logarithmic average of the distribution function.
    sigma : float
        Scatter of the distribution function.
    median : float
        Geometric mean of the distribution function.
    str_display : str
        String representation of field, for display.

    Notes
    -----

    """
    _dtype = float
    _mu: float
    _sigma: float
    _median: float
    _mean: float
    _variance: float
    _p5: float
    _p95: float

    def __init__(self, label, slug='', mu=0, sigma=1, median=1, tooltip=None, label_rtf=None):
        super().__init__(label=label, slug=slug, value=1, label_rtf=label_rtf, tooltip=tooltip)
        self._rng = np.random.default_rng()
        self._mu = mu
        self._sigma = sigma
        self._median = median
        self.update_vals()

    def update_vals(self):
        self.sample = self._rng.lognormal(self._mu, self._sigma, 10000)
        if self._mu < 998:
            self._mean = np.average(self.sample)
            self._p5 = np.percentile(self.sample, 5)
            self._p95 = np.percentile(self.sample, 95)
        else:
            self._mean = 999
            self._p5 = 999
            self._p95 = 999
        self.notify_changed()

    @property
    def mu(self):
        return self._mu

    @mu.setter
    def mu(self, val):
        self._mu = float(val)
        self._median = np.exp(self._mu) if self._mu < 999 else 999
        self.update_vals()

    @property
    def mu_str(self):
        return get_num_str(self.mu)

    @property
    def sigma(self):
        return self._sigma

    @sigma.setter
    def sigma(self, val):
        self._sigma = float(val)
        self.update_vals()

    @property
    def sigma_str(self):
        return get_num_str(self.sigma)

    @property
    def median(self):
        return self._median

    @median.setter
    def median(self, val):
        self._median = float(val)
        self._mu = np.log(self._median)
        self.update_vals()

    @property
    def median_str(self):
        return get_num_str(self.median)

    @property
    def mean(self):
        return self._mean

    @property
    def p5(self):
        return self._p5

    @property
    def p95(self):
        return self._p95

    @property
    def str_display(self):
        """ Returns string-representation of parameter, including label."""
        result = f"{self.label}: mu {self.mu} sigma {self.sigma}"
        return result

    def check_valid(self) -> ValidationResponse:
        resp = super().check_valid()
        if resp.status != InputStatus.GOOD:
            return resp

        msg = ""
        status = InputStatus.ERROR if msg else InputStatus.GOOD
        return ValidationResponse(status, msg)

    def get_prepped_value(self):
        return {'mu': self.mu, 'sigma': self.sigma, 'median': self.median}

    def to_dict(self) -> dict:
        """Returns data representation with values in standard (raw) format.

        Returns
        -------
        dict
            Parsed parameter data with values in standard units.

        """
        result = super().to_dict()
        extra = {
            'mu': self._mu,
            'sigma': self._sigma,
            'median': self._median
        }
        result |= extra
        return result

    def from_dict(self, data: dict, notify_from_model=True, silent=False):
        """
        Overwrites all parameter data from contents of incoming dict.

        Parameters
        ----------
        data : dict
            parameter data in standard units with keys matching field names.

        notify_from_model : bool, default=True
            flag indicating the call originated from backend model. Triggers corresponding event.

        Notes
        -----
        Assumes all required properties are present.
        Min and max values stored as strings to accommodate infinity.

        """
        super().from_dict(data=data, notify_from_model=notify_from_model, silent=True)

        # Verify all extra data present
        # expected_keys = self.to_dict().keys()
        # for key in expected_keys:
        #     if key not in data:
        #         raise ValueError(f'Required key {key} not found in data {data}')

        self._mu = data['mu']
        self._sigma = data['sigma']
        self._median = data['median']

        if not silent:
            self.changed.notify(self)

            if notify_from_model:
                self.changed_by_model.notify(self)
