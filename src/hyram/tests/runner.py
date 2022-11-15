"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


import unittest

from tests import test_phys_api
from tests import test_phys_flame
from tests import test_phys_fluid
from tests import test_phys_overpressure
from tests import test_phys_utils
from tests import test_qra_analysis
from tests import test_qra_effects
from tests import test_qra_consequence
from tests import test_qra_ignition_probs
from tests import test_qra_pipe_size
from tests import test_qra_positions
from tests import test_qra_probits
from tests import test_qra_risk


def suite():
    """
    Compile automated tests for comparing with references, testing APIs, etc.

    format for adding whole class of tests:
        suite.addTest(unittest.makeSuite(filename.TestClassName))

    Alternately, tests can be run using the Python `unittest` framework
    For test discovery, all tests will be located in this "tests" directory and will match the pattern:
    `test_*.py`.
    """
    suite = unittest.TestSuite()

    do_test_qra = True
    do_test_phys_api = True
    do_test_phys = True

    if do_test_qra:
        suite.addTest(unittest.makeSuite(test_qra_analysis.TestAnalysis))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestThermalEffects))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestOverpressureEffects))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestEffectPlots))
        suite.addTest(unittest.makeSuite(test_qra_consequence.TestThermalFatalitiesCalc))
        suite.addTest(unittest.makeSuite(test_qra_consequence.TestOverpressureFatalitiesCalc))
        suite.addTest(unittest.makeSuite(test_qra_ignition_probs.TestIgnitionProbability))
        suite.addTest(unittest.makeSuite(test_qra_pipe_size.TestPipeSizeCalcs))
        suite.addTest(unittest.makeSuite(test_qra_positions.TestPositionGenerator))
        suite.addTest(unittest.makeSuite(test_qra_probits.TestFatalityProbabilityCalc))
        suite.addTest(unittest.makeSuite(test_qra_probits.TestThermalProbits))
        suite.addTest(unittest.makeSuite(test_qra_probits.TestOverpressureProbits))
        suite.addTest(unittest.makeSuite(test_qra_risk.TestRiskMetricCalcs))
        suite.addTest(unittest.makeSuite(test_qra_risk.TestScenarioRiskCalcs))

    # PHYSICS TESTS
    if do_test_phys_api:
        suite.addTest(unittest.makeSuite(test_phys_api.EtkTpdTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.TestETKTNTEquivalentMass))
        suite.addTest(unittest.makeSuite(test_phys_api.PlumeDispersionTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.JetFlameAnalysisTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.AccumulationTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.OverpressureTestCase))

    if do_test_phys:
        suite.addTest(unittest.makeSuite(test_phys_fluid.PureFluidTestCase))
        suite.addTest(unittest.makeSuite(test_phys_fluid.BlendFluidTestCase))
        suite.addTest(unittest.makeSuite(test_phys_flame.TestAtmosphericTransmissivity))
        suite.addTest(unittest.makeSuite(test_phys_flame.TestFlameObject))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.GenericMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.BstMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.TntMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.BauwensMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.TestJallaisOverpressureH2))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.OverpressureBlendInputTestCase))
        suite.addTest(unittest.makeSuite(test_phys_utils.TestGetDistanceToEffect))

    return suite



if __name__ == '__main__':
    runner = unittest.TextTestRunner(failfast=True)
    runner.run(suite())
