"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import unittest
from hyramgui.models.units import Pressure, Time, Area, Volume, Density, Mass
from scipy import constants as spc


class PressureConversionTestCase(unittest.TestCase):
    '''
    potential units: 'pa', 'kpa', 'mpa', 'psi', 'atm', 'bar'
    '''
    def test_pa_to_kpa(self):
        result = Pressure.convert(1000, 'pa', 'kpa')
        self.assertAlmostEqual(result, 1)
        result = Pressure.convert(2000, 'pa', 'kpa')
        self.assertAlmostEqual(result, 2)

    def test_kpa_to_mpa(self):
        result = Pressure.convert(1, 'kpa', 'mpa')
        self.assertAlmostEqual(result, 1e-3)
        result = Pressure.convert(2.5, 'kpa', 'mpa')
        self.assertAlmostEqual(result, 2.5e-3)

    def test_pa_to_psi(self):
        result = Pressure.convert(101325, 'pa', 'psi')
        self.assertAlmostEqual(result, 14.7, places=1)
        result = Pressure.convert(14.696, 'psi', 'pa')
        self.assertAlmostEqual(result, 101325, places=0)

    def test_psi_to_atm(self):
        result = Pressure.convert(14.7, 'psi', 'atm')
        self.assertAlmostEqual(result, 1, places=0)
        result = Pressure.convert(1, 'atm', 'psi')
        self.assertAlmostEqual(result, 14.7, places=1)

    def test_atm_to_bar(self):
        result = Pressure.convert(1.01325, 'atm', 'bar')
        self.assertAlmostEqual(result, 1.026676, places=4)
        result = Pressure.convert(2.0265, 'atm', 'bar')
        self.assertAlmostEqual(result, 2.05335, places=4)
    
    def test_atm_to_pa(self):
        result = Pressure.convert(1, 'atm', 'pa')
        self.assertAlmostEqual(result, 101325, places=4)
        result = Pressure.convert(101325, 'pa', 'atm')
        self.assertAlmostEqual(result, 1, places=4)


class TimeConversionTestCase(unittest.TestCase):
    def test_sec_to_min(self):
        result = Time.convert(60, Time.sec, Time.min)
        self.assertAlmostEqual(result, 1)

    def test_min_to_hr(self):
        result = Time.convert(60, Time.min, Time.hr)
        self.assertAlmostEqual(result, 1)

    def test_hr_to_min(self):
        result = Time.convert(1, Time.hr, Time.min)
        self.assertAlmostEqual(result, 60)

    def test_min_to_sec(self):
        result = Time.convert(1, Time.min, Time.sec)
        self.assertAlmostEqual(result, 60)

    def test_sec_to_hr(self):
        result = Time.convert(3600, Time.sec, Time.hr)
        self.assertAlmostEqual(result, 1)

    def test_hr_to_sec(self):
        result = Time.convert(1, Time.hr, Time.sec)
        self.assertAlmostEqual(result, 3600)

    def test_sec_to_ms(self):
        result = Time.convert(1, Time.sec, Time.ms)
        self.assertAlmostEqual(result, 1000)

    def test_ms_to_sec(self):
        result = Time.convert(1000, Time.ms, Time.sec)
        self.assertAlmostEqual(result, 1)

    def test_min_to_ms(self):
        result = Time.convert(1, Time.min, Time.ms)
        self.assertAlmostEqual(result, 60000)

    def test_ms_to_min(self):
        result = Time.convert(60000, Time.ms, Time.min)
        self.assertAlmostEqual(result, 1)


class AreaConversionTestCase(unittest.TestCase):
    def test_m2_to_cm2(self):
        self.assertEqual(Area.convert(1, Area.m2, Area.cm2), 1e4)

    def test_m2_to_mm2(self):
        self.assertEqual(Area.convert(1, Area.m2, Area.mm2), 1e6)

    def test_m2_to_in2(self):
        self.assertAlmostEqual(Area.convert(1, Area.m2, Area.in2), 1 / (0.0254 ** 2))

    def test_m2_to_ft2(self):
        self.assertAlmostEqual(Area.convert(1, Area.m2, Area.ft2), 1 / (0.3048 ** 2))

    def test_m2_to_yd2(self):
        self.assertAlmostEqual(Area.convert(1, Area.m2, Area.yd2), 1 / (0.9144 ** 2))

    def test_cm2_to_m2(self):
        self.assertEqual(Area.convert(1, Area.cm2, Area.m2), 1e-4)

    def test_mm2_to_m2(self):
        self.assertEqual(Area.convert(1, Area.mm2, Area.m2), 1e-6)

    def test_in2_to_m2(self):
        self.assertAlmostEqual(Area.convert(1, Area.in2, Area.m2), (0.0254 ** 2))

    def test_ft2_to_m2(self):
        self.assertAlmostEqual(Area.convert(1, Area.ft2, Area.m2), (0.3048 ** 2))

    def test_yd2_to_m2(self):
        self.assertAlmostEqual(Area.convert(1, Area.yd2, Area.m2), (0.9144 ** 2))


class VolumeConversionTestCase(unittest.TestCase):
    '''
    Units Available: 'l', 'm3', 'mm3', 'cm3', 'ml', 'in3', 'ft3'
    '''
    def test_m3_to_l(self):
        self.assertEqual(Volume.convert(1, Volume.m3, Volume.l), 1e3)

    def test_m3_to_mm3(self):
        self.assertAlmostEqual(Volume.convert(1, Volume.m3, Volume.mm3), 1e9, places = 6)

    def test_m3_to_cm3(self):
        self.assertEqual(Volume.convert(1, Volume.m3, Volume.cm3), 1e6)

    def test_l_to_m3(self):
        self.assertEqual(Volume.convert(1, Volume.l, Volume.m3), 1e-3)

    def test_mm3_to_m3(self):
        self.assertEqual(Volume.convert(1, Volume.mm3, Volume.m3), 1e-9)

    def test_cm3_to_m3(self):
        self.assertEqual(Volume.convert(1, Volume.cm3, Volume.m3), 1e-6)

    def test_m3_to_ft3(self):
        self.assertAlmostEqual(Volume.convert(1, Volume.m3, Volume.ft3), 35.3, places = 1)

    def test_m3_to_in3(self):
        self.assertAlmostEqual(Volume.convert(1, Volume.m3, Volume.in3), 61023.7, places = 1)


class DensityConversionTestCase(unittest.TestCase):
    '''kgpm3', 'gpm3', 'gpcm3', 'kgpl', 'gpl', 'lbft3'
    '''
    def test_gpm3_to_kgpm3(self):
        self.assertEqual(Density.convert(1, Density.gpm3, Density.kgpm3), 1e-3)

    def test_gpm3_to_gpcm3(self):
        self.assertAlmostEqual(Density.convert(1, Density.gpm3, Density.gpcm3), 1e-6)

    def test_gpm3_to_kgpl(self):
        self.assertEqual(Density.convert(1, Density.gpm3, Density.kgpl), 1e-6)

    def test_kgpm3_to_gpm3(self):
        self.assertEqual(Density.convert(1, Density.kgpm3, Density.gpm3), 1e3)

    def test_kgpm3_to_gpcm3(self):
        self.assertEqual(Density.convert(1, Density.kgpm3, Density.gpcm3), 1e-3)

    def test_kgpm3_to_gpl(self):
        self.assertEqual(Density.convert(1, Density.kgpm3, Density.gpl), 1)
    
    def test_kgpm3_to_lbft3(self):
        self.assertAlmostEqual(Density.convert(1, Density.kgpm3, Density.lbpft3), 0.062428, places = 5)



class MassConversionTestCase(unittest.TestCase):
    '''
    Available units: 'mg', 'g', 'kg', 'lb'
    '''
    def test_g_to_mg(self):
        self.assertAlmostEqual(Mass.convert(1, Mass.g, Mass.mg), 1e3)

    def test_g_to_kg(self):
        self.assertEqual(Mass.convert(1, Mass.g, Mass.kg), 1e-3)

    def test_kg_to_lb(self):
        self.assertAlmostEqual(Mass.convert(1, Mass.kg, Mass.lb), 2.20462, places = 4)


    def test_mg_to_g(self):
        self.assertEqual(Mass.convert(1, Mass.mg, Mass.g), 1e-3)

    def test_kg_to_g(self):
        self.assertEqual(Mass.convert(1, Mass.kg, Mass.g), 1e3)

    def test_lb_to_kg(self):
        self.assertAlmostEqual(Mass.convert(1, Mass.lb, Mass.kg), 0.453592, places = 4)
