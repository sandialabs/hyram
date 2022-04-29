"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

"""
Alternately, tests can be run using the Python `unittest` framework
For test discovery, all tests will be located in this "tests" directory and will match the pattern: 
    `test_*.py`.
"""

import unittest
# Be careful about shadowing GUI tests pkg; added 'hyram.' for this
from tests import (test_c_api_etk, test_c_api_phys, test_c_api_jet_plume,
                   test_c_api_qra, test_c_api_overpressure,
                   test_qra_analysis, test_qra_effects, test_qra_fatalities,
                   test_qra_ignition_probs, test_qra_pipe_size,
                   test_qra_positions, test_qra_probits, test_qra_risk,
                   test_phys_api, test_phys_flame, test_phys_overpressure)


def suite():
    """
    Compile automated tests for comparing with references, testing APIs, etc.

    format for adding whole class of tests:
        suite.addTest(unittest.makeSuite(filename.TestClassName))
    """
    suite = unittest.TestSuite()

    do_test_c_api_etk = True
    do_test_c_api_phys = True
    do_test_c_api_qra = True
    do_test_qra = True
    do_test_phys_api = True
    do_test_phys = True

    # C-API TESTS
    if do_test_c_api_etk:
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKMassFlowTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTankMassTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTPDTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTntMassTestCase))

    if do_test_c_api_phys:
        suite.addTest(unittest.makeSuite(test_c_api_jet_plume.H2JetPlumeTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_jet_plume.LH2JetPlumeTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_phys.IndoorReleaseTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_phys.TestFlameTempPlotGeneration))
        suite.addTest(unittest.makeSuite(test_c_api_phys.TestRadHeatAnalysis))
        suite.addTest(unittest.makeSuite(test_c_api_overpressure.OverpressureTestCase))

    if do_test_c_api_qra:
        suite.addTest(unittest.makeSuite(test_c_api_qra.TestHydrogen))
        suite.addTest(unittest.makeSuite(test_c_api_qra.TestMethane))
        suite.addTest(unittest.makeSuite(test_c_api_qra.TestPropane))

    # QRA TESTS
    if do_test_qra:
        suite.addTest(unittest.makeSuite(test_qra_analysis.TestAnalysis))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestThermalEffects))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestOverpressureEffects))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestEffectPlots))
        suite.addTest(unittest.makeSuite(test_qra_fatalities.TestThermalFatalitiesCalc))
        suite.addTest(unittest.makeSuite(test_qra_fatalities.TestOverpressureFatalitiesCalc))
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
        suite.addTest(unittest.makeSuite(test_phys_api.TestETKTemperaturePressureDensity))
        suite.addTest(unittest.makeSuite(test_phys_api.TestPlumeDispersion))
        suite.addTest(unittest.makeSuite(test_phys_api.TestJetFlameAnalysis))
        suite.addTest(unittest.makeSuite(test_phys_api.OverpressureTestCase))

    if do_test_phys:
        suite.addTest(unittest.makeSuite(test_phys_flame.TestAtmosphericTransmissivity))
        suite.addTest(unittest.makeSuite(test_phys_flame.TestFlameObject))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.GenericMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.BstMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.TntMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.BauwensMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.TestJallaisOverpressureH2))

    return suite


if __name__ == '__main__':
    runner = unittest.TextTestRunner(failfast=True)
    runner.run(suite())
