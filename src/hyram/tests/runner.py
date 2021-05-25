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

-----
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
