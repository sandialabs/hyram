"""
This allows for all tests to be executed by simply running this file.

Alternately, tests can be run using the Python `unittest` framework. 
For test discovery, all tests will be located in this "tests" directory and will match the pattern: 
    `test_*.py`.
"""

import unittest
# Be careful about shadowing GUI tests pkg; added 'hyram.' for this
from tests import test_c_api_etk, test_c_api_phys, test_api_flux, test_c_api_jet_plume, test_c_api_qra
from tests import test_api_flux

def suite():
    """
    Compile automated tests for comparing with references, testing APIs, etc.

    format for adding whole class of tests:
    suite.addTest(unittest.makeSuite(filename.TestClassName))
    -------

    """
    suite = unittest.TestSuite()

    # TESTS USING REFERENCE/PAPER DATA
    # TODO: add validation tests for blowdown
    # TODO: add validation tests for jet plume
    # TODO: add validation tests for jet flame
    # TODO: add validation tests for overpressure/accumulation

    # C API TESTS
    suite.addTest(unittest.makeSuite(test_c_api_etk.ETKMassFlowTestCase))
    suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTankMassTestCase))
    suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTPDTestCase))
    suite.addTest(unittest.makeSuite(test_c_api_jet_plume.H2JetPlumeTestCase))
    suite.addTest(unittest.makeSuite(test_c_api_jet_plume.LH2JetPlumeTestCase))
    suite.addTest(unittest.makeSuite(test_c_api_phys.IndoorReleaseTestCase))
    suite.addTest(unittest.makeSuite(test_c_api_phys.TestFlameTempPlotGeneration))
    suite.addTest(unittest.makeSuite(test_c_api_phys.TestRadHeatAnalysis))
    suite.addTest(unittest.makeSuite(test_c_api_qra.TestQRA))

    # FLUX PHYS API TESTS
    suite.addTest(unittest.makeSuite(test_api_flux.TestDischargeRateCalculation))
    suite.addTest(unittest.makeSuite(test_api_flux.TestPositionalFlux))
    # TODO: add additional API tests

    return suite


if __name__ == '__main__':
    runner = unittest.TextTestRunner(failfast=False)
    runner.run(suite())
