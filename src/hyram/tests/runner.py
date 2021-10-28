"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""

"""
-----
Alternately, tests can be run using the Python `unittest` framework. 
For test discovery, all tests will be located in this "tests" directory and will match the pattern: 
    `test_*.py`.
"""

import unittest
# Be careful about shadowing GUI tests pkg; added 'hyram.' for this
from tests import (test_c_api_etk, test_c_api_phys, test_c_api_jet_plume, test_c_api_qra, test_c_api_overpressure,
                   test_api_flux,  test_api_overpressure,
                   test_overpressure)


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

    do_test_etk = True
    do_test_phys = True
    do_test_qra = True
    do_test_overpressure = True

    # C API TESTS
    if do_test_etk:
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKMassFlowTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTankMassTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTPDTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_etk.ETKTntMassTestCase))

    if do_test_phys:
        suite.addTest(unittest.makeSuite(test_c_api_jet_plume.H2JetPlumeTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_jet_plume.LH2JetPlumeTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_phys.IndoorReleaseTestCase))
        suite.addTest(unittest.makeSuite(test_c_api_phys.TestFlameTempPlotGeneration))
        suite.addTest(unittest.makeSuite(test_c_api_phys.TestRadHeatAnalysis))
        suite.addTest(unittest.makeSuite(test_api_flux.TestPositionalFlux))

    # QRA Tests
    if do_test_qra:
        # NOTE: these are regression tests only and do not contain experimentally-validated comparisons.
        suite.addTest(unittest.makeSuite(test_c_api_qra.TestHydrogen))
        suite.addTest(unittest.makeSuite(test_c_api_qra.TestMethane))
        suite.addTest(unittest.makeSuite(test_c_api_qra.TestPropane))

    # OVERPRESSURE METHODS API AND FUNCTION TESTS
    if do_test_overpressure:
        suite.addTest(unittest.makeSuite(test_c_api_overpressure.OverpressureTestCase))
        suite.addTest(unittest.makeSuite(test_api_overpressure.OverpressureTestCase))
        suite.addTest(unittest.makeSuite(test_overpressure.BstMethodTestCase))
        suite.addTest(unittest.makeSuite(test_overpressure.TntMethodTestCase))
        suite.addTest(unittest.makeSuite(test_overpressure.BauwensMethodTestCase))

    return suite


if __name__ == '__main__':
    runner = unittest.TextTestRunner(failfast=False)
    runner.run(suite())
