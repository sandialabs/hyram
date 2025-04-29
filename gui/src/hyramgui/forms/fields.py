"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""

import numpy as np
from PySide6.QtCore import Signal, Property
from PySide6.QtQml import QmlElement

from hyramgui.models.fields import LognormField, DistributionField
from hyramgui.hygu.forms.fields import FormFieldBase

QML_IMPORT_NAME = "hyram.classes"
QML_IMPORT_MAJOR_VERSION = 1


@QmlElement
class DistributionFormField(FormFieldBase):
    """Manages parameter described by distribution with two parameters.

    Attributes
    ----------
    distr : {'beta', 'ev', 'ln'}
        Distribution key
    pa : float
        Distribution parameter A
    pb : float
        Distribution parameter B
    distrChanged : Signal
        Event emitted when distribution choice changes.
    pChanged : Signal
        Event emitted when distribution parameter changes.

    """
    distrChanged = Signal(str)
    pChanged = Signal(float)

    _param: DistributionField

    def __init__(self, param=None):
        super().__init__(param=param)

    @Property(str, notify=distrChanged)
    def distr(self):
        return self._param.distr

    @distr.setter
    def distr(self, val: str):
        self._param.distr = val
        self.distrChanged.emit(val)

    @Property(float, notify=pChanged)
    def pa(self):
        return self._param.pa

    @pa.setter
    def pa(self, val: float):
        self._param.pa = val
        self.pChanged.emit(val)

    @Property(float, notify=pChanged)
    def pb(self):
        return self._param.pb

    @pb.setter
    def pb(self, val: float):
        self._param.pb = val
        self.pChanged.emit(val)


@QmlElement
class LognormFormField(FormFieldBase):
    """Manages parameter described by Lognormal distribution.

    Attributes
    ----------
    mu : float
        Logarithmic average of the distribution function.
    sigma : float
        Scatter of the distribution function.
    median : float
        Geometric mean of the distribution function.
    muChanged : Signal
        Event emitted when mu changes.
    sigmaChanged : Signal
        Event emitted when sigma changes.
    medianChanged : Signal
        Event emitted when median changes.

    """
    muChanged = Signal(float)
    sigmaChanged = Signal(float)
    medianChanged = Signal(float)
    pChanged = Signal(float)

    _param: LognormField

    def __init__(self, param=None):
        super().__init__(param=param)

    @Property(float, notify=muChanged)
    def mu(self):
        return self._param.mu

    @mu.setter
    def mu(self, val: float):
        self._param.mu = val
        self.muChanged.emit(val)

    @Property(float, notify=sigmaChanged)
    def sigma(self):
        return self._param.sigma

    @sigma.setter
    def sigma(self, val: float):
        self._param.sigma = val
        self.sigmaChanged.emit(val)

    @Property(float, notify=medianChanged)
    def median(self):
        return self._param.median

    @median.setter
    def median(self, val: float):
        self._param.median = val
        self.medianChanged.emit(val)

    @Property(float, notify=pChanged)
    def mean(self):
        return self._param.mean

    @Property(float, notify=pChanged)
    def p5(self):
        return self._param.p5

    @Property(float, notify=pChanged)
    def p95(self):
        return self._param.p95

