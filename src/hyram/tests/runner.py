"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
import unittest

import test_phys_api, test_phys_jet, test_phys_flame, test_phys_fluid, test_phys_overpressure, test_phys_plots, test_phys_therm, test_phys_utils
import test_qra_analysis, test_qra_effects, test_qra_consequence, test_qra_ignition_probs, test_qra_pipe_size, test_qra_positions, test_qra_probits, test_qra_risk, test_qra_leak, test_qra_blends
import test_validation_blowdown, test_validation_heatflux, test_validation_jetplume, test_validation_cryo_concentrations, test_validation_cryo, test_validation_overpressure_accumulation


def suite():
    """
    Compile automated tests for comparing with references, testing APIs, etc.
    Tests can also be executed using the Python `unittest` framework.

    format for adding whole class of tests:
        suite.addTest(unittest.makeSuite(filename.TestClassName))

    For test discovery, all tests will be located in this "tests" directory and will match the pattern:
        `test_*.py`.

    """
    suite = unittest.TestSuite()

    do_test_qra = True
    do_test_phys_api = True
    do_test_phys = True
    do_test_validation = True

    if do_test_qra:
        suite.addTest(unittest.makeSuite(test_qra_analysis.HydrogenTestCase))
        suite.addTest(unittest.makeSuite(test_qra_blends.QraBlendsTestCase))
        suite.addTest(unittest.makeSuite(test_qra_consequence.TestThermalFatalitiesCalc))
        suite.addTest(unittest.makeSuite(test_qra_consequence.TestOverpressureFatalitiesCalc))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestThermalEffects))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestOverpressureEffects))
        suite.addTest(unittest.makeSuite(test_qra_effects.TestEffectPlots))
        suite.addTest(unittest.makeSuite(test_qra_consequence.TestThermalFatalitiesCalc))
        suite.addTest(unittest.makeSuite(test_qra_consequence.TestOverpressureFatalitiesCalc))
        suite.addTest(unittest.makeSuite(test_qra_ignition_probs.TestIgnitionProbability))
        suite.addTest(unittest.makeSuite(test_qra_leak.LeakResultTestCase))
        suite.addTest(unittest.makeSuite(test_qra_pipe_size.TestPipeSizeCalcs))
        suite.addTest(unittest.makeSuite(test_qra_positions.TestPositionGenerator))
        suite.addTest(unittest.makeSuite(test_qra_probits.TestFatalityProbabilityCalc))
        suite.addTest(unittest.makeSuite(test_qra_probits.TestThermalProbits))
        suite.addTest(unittest.makeSuite(test_qra_probits.TestOverpressureProbits))
        suite.addTest(unittest.makeSuite(test_qra_risk.TestRiskMetricCalcs))
        suite.addTest(unittest.makeSuite(test_qra_risk.TestScenarioRiskCalcs))

    if do_test_phys_api:
        suite.addTest(unittest.makeSuite(test_phys_api.EtkTpdTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.TestETKTNTEquivalentMass))
        suite.addTest(unittest.makeSuite(test_phys_api.PlumeDispersionTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.JetFlameAnalysisTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.AccumulationTestCase))
        suite.addTest(unittest.makeSuite(test_phys_api.OverpressureTestCase))

    if do_test_phys:
        suite.addTest(unittest.makeSuite(test_phys_flame.TestAtmosphericTransmissivity))
        suite.addTest(unittest.makeSuite(test_phys_flame.TestFlameObject))
        suite.addTest(unittest.makeSuite(test_phys_fluid.PureFluidTestCase))
        suite.addTest(unittest.makeSuite(test_phys_fluid.BlendFluidTestCase))
        suite.addTest(unittest.makeSuite(test_phys_jet.TestJetObject))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.GenericMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.BstMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.TntMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.BauwensMethodTestCase))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.TestJallaisOverpressureH2))
        suite.addTest(unittest.makeSuite(test_phys_overpressure.OverpressureBlendInputTestCase))
        suite.addTest(unittest.makeSuite(test_phys_plots.TestContourPlots))
        suite.addTest(unittest.makeSuite(test_phys_therm.TestPropsSI))
        suite.addTest(unittest.makeSuite(test_phys_utils.TestGetDistanceToEffect))

    if do_test_validation:
        suite.addTest(unittest.makeSuite(test_validation_blowdown.Test_Ekoto_2012))
        suite.addTest(unittest.makeSuite(test_validation_blowdown.Test_Schefer_2007))
        suite.addTest(unittest.makeSuite(test_validation_blowdown.Test_Proust_2011))
        suite.addTest(unittest.makeSuite(test_validation_blowdown.Test_Schefer_2006))
        suite.addTest(unittest.makeSuite(test_validation_cryo_concentrations.Test_Xiao_2010))
        suite.addTest(unittest.makeSuite(test_validation_cryo_concentrations.Test_Friedrich_Dist1))
        suite.addTest(unittest.makeSuite(test_validation_cryo_concentrations.Test_Friedrich_Dist2))
        suite.addTest(unittest.makeSuite(test_validation_cryo.Test_LabCryoData))
        suite.addTest(unittest.makeSuite(test_validation_heatflux.Test_Ekoto_2014))
        suite.addTest(unittest.makeSuite(test_validation_heatflux.Test_Schefer_2006_2007))
        suite.addTest(unittest.makeSuite(test_validation_heatflux.Test_Imamura_2008))
        suite.addTest(unittest.makeSuite(test_validation_heatflux.Test_Mogi_2005))
        suite.addTest(unittest.makeSuite(test_validation_heatflux.Test_Proust_2011))
        suite.addTest(unittest.makeSuite(test_validation_jetplume.Test_HoufSchefer_2008_fig8))
        suite.addTest(unittest.makeSuite(test_validation_jetplume.Test_HoufSchefer_2008_fig9))
        suite.addTest(unittest.makeSuite(test_validation_jetplume.Test_RugglesEkoto_2012))
        suite.addTest(unittest.makeSuite(test_validation_jetplume.Test_Han_2013_fig3))
        suite.addTest(unittest.makeSuite(test_validation_jetplume.Test_Han_2013_fig6))
        suite.addTest(unittest.makeSuite(test_validation_jetplume.Test_Han_2013_fig7))
        suite.addTest(unittest.makeSuite(test_validation_jetplume.Test_Han_2013_fig8))
        suite.addTest(unittest.makeSuite(test_validation_overpressure_accumulation.Test_Ekoto_2012))
        suite.addTest(unittest.makeSuite(test_validation_overpressure_accumulation.Test_Giannissi_2015))
        suite.addTest(unittest.makeSuite(test_validation_overpressure_accumulation.Test_Merilo))
        suite.addTest(unittest.makeSuite(test_validation_overpressure_accumulation.Test_BMHA_Fig7))
        suite.addTest(unittest.makeSuite(test_validation_overpressure_accumulation.Test_BMHA_Fig9))

    return suite


if __name__ == '__main__':
    runner = unittest.TextTestRunner(failfast=True)
    runner.run(suite())
