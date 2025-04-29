"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import unittest
import scipy.constants as scp

from ..utils.units_of_measurement import (
    Distance, Pressure, Angle, Temperature, SmallDistance, Fracture, Unitless, Fractional)


DELTA = 1e-5


class DistanceTestCase(unittest.TestCase):

    def setUp(self):
        self.distance = Distance()

    def test_label(self):
        self.assertEqual(self.distance.label, 'dist')

    def test_unit_data_values(self):
        self.assertEqual(self.distance.unit_data.get('m'), 1)
        self.assertEqual(self.distance.unit_data.get('mm'), 0.001)
        self.assertEqual(self.distance.unit_data.get('km'), 1000)
        self.assertEqual(self.distance.unit_data.get('in'), scp.inch)
        self.assertEqual(self.distance.unit_data.get('ft'), scp.foot)
        self.assertEqual(self.distance.unit_data.get('mi'), scp.mile)

    def test_display_units(self):
        self.assertIn('m', self.distance.display_units)
        self.assertIn('mm', self.distance.display_units)
        self.assertIn('Km', self.distance.display_units)
        self.assertIn('in', self.distance.display_units)
        self.assertIn('ft', self.distance.display_units)
        self.assertIn('mi', self.distance.display_units)

    def test_units(self):
        self.assertEqual(self.distance.m, 'm')
        self.assertEqual(self.distance.mm, 'mm')
        self.assertEqual(self.distance.km, 'km')
        self.assertEqual(self.distance.inch, 'in')
        self.assertEqual(self.distance.ft, 'ft')
        self.assertEqual(self.distance.mi, 'mi')

    def test_std_unit(self):
        self.assertEqual(self.distance.std_unit, 'm')

    def test_m(self):
        """ Tests conversions from m to other distance units. """
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.m, new=Distance.mm), 50, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.m, new=Distance.mm), 1000, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.m, new=Distance.m), 0.05, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.m, new=Distance.m), 1, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.m, new=Distance.km), 5e-5, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.m, new=Distance.km), 1e-3, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.m, new=Distance.inch), 1.9685, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.m, new=Distance.inch), 39.37008, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.m, new=Distance.ft), 0.16404, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.m, new=Distance.ft), 3.28084, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.m, new=Distance.mi), 3.11e-5, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.m, new=Distance.mi), 6.21e-4, delta=DELTA)

    def test_mm(self):
        """ Tests conversions from mm. """
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mm, new=Distance.mm), 0.05, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mm, new=Distance.mm), 1, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mm, new=Distance.m), 5e-5, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mm, new=Distance.m), 1e-3, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mm, new=Distance.km), 5e-8, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mm, new=Distance.km), 1e-6, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mm, new=Distance.inch), 1.9685e-3, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mm, new=Distance.inch), 3.937e-2, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mm, new=Distance.ft), 1.6404e-4, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mm, new=Distance.ft), 3.28084e-3, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mm, new=Distance.mi), 3.11e-8, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mm, new=Distance.mi), 6.21e-7, delta=DELTA)

    def test_km(self):
        """ Tests conversions from km. """
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.km, new=Distance.mm), 5e4, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.km, new=Distance.mm), 1e6, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.km, new=Distance.m), 50, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.km, new=Distance.m), 1e3, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.km, new=Distance.km), 0.05, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.km, new=Distance.km), 1.0, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.km, new=Distance.inch), 1968.5, places=1)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.km, new=Distance.inch), 39370.1, places=1)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.km, new=Distance.ft), 164.04, places=2)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.km, new=Distance.ft), 3280.84, places=2)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.km, new=Distance.mi), 3.11e-2, places=0)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.km, new=Distance.mi), 0.62137, delta=DELTA)

    def test_inch(self):
        """ Tests conversions from inches. """
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.inch, new=Distance.mm), 1.27, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.inch, new=Distance.mm), 25.4, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.inch, new=Distance.m), 1.27e-3, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.inch, new=Distance.m), 2.54e-2, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.inch, new=Distance.km), 1.27e-6, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.inch, new=Distance.km), 2.54e-5, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.inch, new=Distance.inch), 0.05, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.inch, new=Distance.inch), 1, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.inch, new=Distance.ft), 4.1667e-3, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.inch, new=Distance.ft), 8.333e-2, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.inch, new=Distance.mi), 7.89e-7, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.inch, new=Distance.mi), 1.58e-5, delta=DELTA)

    def test_ft(self):
        """ Tests conversions from ft. """
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.ft, new=Distance.mm), 15.24, places=0)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.ft, new=Distance.mm), 304.8, places=0)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.ft, new=Distance.m), 1.524e-2, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.ft, new=Distance.m), 0.3048, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.ft, new=Distance.km), 1.524e-5, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.ft, new=Distance.km), 3.048e-4, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.ft, new=Distance.inch), 0.6, places=2)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.ft, new=Distance.inch), 12, places=2)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.ft, new=Distance.ft), 0.05, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.ft, new=Distance.ft), 1, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.ft, new=Distance.mi), 9.47e-6, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.ft, new=Distance.mi), 1.89e-4, delta=DELTA)

    def test_mi(self):
        """ Tests conversions from mi. """
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mi, new=Distance.mm), 80467, places=0)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mi, new=Distance.mm), 1609344.0, places=1)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mi, new=Distance.m), 80.47, places=1)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mi, new=Distance.m), 1609.34, places=1)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mi, new=Distance.km), 8.0467e-2, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mi, new=Distance.km), 1.60934, delta=DELTA)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mi, new=Distance.inch), 3168.0, places=1)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mi, new=Distance.inch), 63360, places=1)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mi, new=Distance.ft), 264, places=1)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mi, new=Distance.ft), 5280, places=1)
        self.assertAlmostEqual(Distance.convert(val=0.05,   old=Distance.mi, new=Distance.mi), 0.05, places=2)
        self.assertAlmostEqual(Distance.convert(val=1.0,    old=Distance.mi, new=Distance.mi), 1.00, places=2)

    def test_m_to_mm(self):
        self.assertAlmostEqual(Distance.convert(val=1.0, old=Distance.m, new=Distance.mm), 1000, delta=1e-5)
        self.assertAlmostEqual(Distance.convert(val=0.5, old=Distance.m, new=Distance.mm), 500, delta=1e-5)

    def test_mm_to_m(self):
        self.assertAlmostEqual(Distance.convert(val=1000.0, old=Distance.mm, new=Distance.m), 1, delta=1e-5)
        self.assertAlmostEqual(Distance.convert(val=500.0, old=Distance.mm, new=Distance.m), 0.5, delta=1e-5)

    def test_km_to_m(self):
        self.assertAlmostEqual(Distance.convert(val=1.0, old=Distance.km, new=Distance.m), 1000, delta=1e-5)
        self.assertAlmostEqual(Distance.convert(val=0.5, old=Distance.km, new=Distance.m), 500, delta=1e-5)

    def test_m_to_in(self):
        self.assertAlmostEqual(Distance.convert(val=1.0, old=Distance.m, new=Distance.inch), 39.37008, delta=1e-5)
        self.assertAlmostEqual(Distance.convert(val=0.5, old=Distance.m, new=Distance.inch), 19.68504, delta=1e-5)

    def test_in_to_m(self):
        self.assertAlmostEqual(Distance.convert(val=39.37008, old=Distance.inch, new=Distance.m), 1.0, delta=1e-5)
        self.assertAlmostEqual(Distance.convert(val=19.68504, old=Distance.inch, new=Distance.m), 0.5, delta=1e-5)

    def test_ft_to_m(self):
        self.assertAlmostEqual(Distance.convert(val=1.0, old=Distance.ft, new=Distance.m), 0.3048, delta=1e-5)
        self.assertAlmostEqual(Distance.convert(val=0.5, old=Distance.ft, new=Distance.m), 0.1524, delta=1e-5)

    def test_mi_to_m(self):
        self.assertAlmostEqual(Distance.convert(val=1.0, old=Distance.mi, new=Distance.m), 1609.34, places=1)
        self.assertAlmostEqual(Distance.convert(val=0.5, old=Distance.mi, new=Distance.m), 804.67, places=1)


class SmallDistanceConversionTestCase(unittest.TestCase):
    def test_m(self):
        """ Tests conversions from m to other units. """
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.m, new=SmallDistance.mm), 50, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.m, new=SmallDistance.mm), 1000, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.m, new=SmallDistance.m), 0.05, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.m, new=SmallDistance.m), 1, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.m, new=SmallDistance.inch), 1.9685, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.m, new=SmallDistance.inch), 39.37008, delta=DELTA)

    def test_mm(self):
        """ Tests conversions from mm. """
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.mm, new=SmallDistance.mm), 0.05, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.mm, new=SmallDistance.mm), 1, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.mm, new=SmallDistance.m), 5e-5, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.mm, new=SmallDistance.m), 1e-3, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.mm, new=SmallDistance.inch), 1.9685e-3, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.mm, new=SmallDistance.inch), 3.937e-2, delta=DELTA)

    def test_inch(self):
        """ Tests conversions from inches. """
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.inch, new=SmallDistance.mm), 1.27, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.inch, new=SmallDistance.mm), 25.4, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.inch, new=SmallDistance.m), 1.27e-3, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.inch, new=SmallDistance.m), 2.54e-2, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=0.05,   old=SmallDistance.inch, new=SmallDistance.inch), 0.05, delta=DELTA)
        self.assertAlmostEqual(SmallDistance.convert(val=1.0,    old=SmallDistance.inch, new=SmallDistance.inch), 1, delta=DELTA)



class PressureTestCase(unittest.TestCase):
    def test_mpa_to_psi(self):
        self.assertAlmostEqual(Pressure.convert(val=1.0, old=Pressure.mpa, new=Pressure.psi), 145.037, places=2)
        self.assertAlmostEqual(Pressure.convert(val=0.5, old=Pressure.mpa, new=Pressure.psi), 72.519, places=2)

    def test_psi_to_mpa(self):
        self.assertAlmostEqual(Pressure.convert(val=145.038, old=Pressure.psi, new=Pressure.mpa), 1.0, delta=1e-5)
        self.assertAlmostEqual(Pressure.convert(val=72.519, old=Pressure.psi, new=Pressure.mpa), 0.5, delta=1e-5)

    def test_mpa_to_bar(self):
        self.assertAlmostEqual(Pressure.convert(val=1.0, old=Pressure.mpa, new=Pressure.bar), 10, delta=1e-5)
        self.assertAlmostEqual(Pressure.convert(val=0.5, old=Pressure.mpa, new=Pressure.bar), 5, delta=1e-5)

    def test_bar_to_mpa(self):
        self.assertAlmostEqual(Pressure.convert(val=10, old=Pressure.bar, new=Pressure.mpa), 1.0, delta=1e-5)
        self.assertAlmostEqual(Pressure.convert(val=5, old=Pressure.bar, new=Pressure.mpa), 0.5, delta=1e-5)

    def test_mpa(self):
        """ Tests conversions from mpa. """
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.mpa, new=Pressure.mpa), 0.05, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.mpa, new=Pressure.mpa), 1, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.mpa, new=Pressure.psi), 7.25189, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.mpa, new=Pressure.psi), 145.03, places=1)
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.mpa, new=Pressure.bar), 0.5, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.mpa, new=Pressure.bar), 10, delta=DELTA)

    def test_psi(self):
        """ Tests conversions from psi. """
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.psi, new=Pressure.mpa), 3.45e-4, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.psi, new=Pressure.mpa), 6.895e-3, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.psi, new=Pressure.psi), 0.05, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.psi, new=Pressure.psi), 1.00, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.psi, new=Pressure.bar), 3.45e-3, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.psi, new=Pressure.bar), 6.895e-2, delta=DELTA)

    def test_bar(self):
        """ Tests conversions from bar. """
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.bar, new=Pressure.mpa), 0.005, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.bar, new=Pressure.mpa), 0.1, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.bar, new=Pressure.psi), 0.725189, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.bar, new=Pressure.psi), 14.503768, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=0.05,   old=Pressure.bar, new=Pressure.bar), 0.05, delta=DELTA)
        self.assertAlmostEqual(Pressure.convert(val=1.00,   old=Pressure.bar, new=Pressure.bar), 1.00, delta=DELTA)


class TemperatureTestCase(unittest.TestCase):
    def test_c_to_k(self):
        self.assertAlmostEqual(Temperature.convert(val=0.0, old=Temperature.c, new=Temperature.k), 273.15, delta=1e-5)
        self.assertAlmostEqual(Temperature.convert(val=100.0, old=Temperature.c, new=Temperature.k), 373.15, delta=1e-5)

    def test_k_to_c(self):
        self.assertAlmostEqual(Temperature.convert(val=273.15, old=Temperature.k, new=Temperature.c), 0.0, delta=1e-5)
        self.assertAlmostEqual(Temperature.convert(val=373.15, old=Temperature.k, new=Temperature.c), 100.0, delta=1e-5)

    def test_f_to_k(self):
        self.assertAlmostEqual(Temperature.convert(val=32.0, old=Temperature.f, new=Temperature.k), 273.15, delta=1e-5)
        self.assertAlmostEqual(Temperature.convert(val=212.0, old=Temperature.f, new=Temperature.k), 373.15, delta=1e-5)

    def test_k_to_f(self):
        self.assertAlmostEqual(Temperature.convert(val=273.15, old=Temperature.k, new=Temperature.f), 32.0, delta=1e-5)
        self.assertAlmostEqual(Temperature.convert(val=373.15, old=Temperature.k, new=Temperature.f), 212.0, delta=1e-5)

    def test_k(self):
        """ Tests conversions from k. """
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.k, new=Temperature.k), 0.05, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.k, new=Temperature.k), 1, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=273,     old=Temperature.k, new=Temperature.k), 273, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=315,     old=Temperature.k, new=Temperature.k), 315, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.k, new=Temperature.c), -273.10, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.k, new=Temperature.c), -272.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=273,     old=Temperature.k, new=Temperature.c), -0.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=315,     old=Temperature.k, new=Temperature.c), 41.85, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.k, new=Temperature.f), -459.58, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.k, new=Temperature.f), -457.87, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=273,     old=Temperature.k, new=Temperature.f), 31.73, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=315,     old=Temperature.k, new=Temperature.f), 107.33, delta=DELTA)

    def test_c(self):
        """ Tests conversions from c. """
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.c, new=Temperature.k), 273.2, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.c, new=Temperature.k), 274.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=-273,    old=Temperature.c, new=Temperature.k), 0.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=41,      old=Temperature.c, new=Temperature.k), 314.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=100,     old=Temperature.c, new=Temperature.k), 373.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.c, new=Temperature.c), 0.05, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.c, new=Temperature.c), 1.00, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=-273,    old=Temperature.c, new=Temperature.c), -273, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=41,      old=Temperature.c, new=Temperature.c), 41.0, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=100,     old=Temperature.c, new=Temperature.c), 100, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.c, new=Temperature.f), 32.09, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.c, new=Temperature.f), 33.8, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=-273,    old=Temperature.c, new=Temperature.f), -459.4, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=41,      old=Temperature.c, new=Temperature.f), 105.8, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=100,     old=Temperature.c, new=Temperature.f), 212, delta=DELTA)

    def test_f(self):
        """ Tests conversions from f. """
        self.assertAlmostEqual(Temperature.convert(val=-460,    old=Temperature.f, new=Temperature.k), -0.183333, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.f, new=Temperature.k), 255.4, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.f, new=Temperature.k), 255.92777, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=41,      old=Temperature.f, new=Temperature.k), 278.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=100,     old=Temperature.f, new=Temperature.k), 310.92777, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=212,     old=Temperature.f, new=Temperature.k), 373.15, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=-460,    old=Temperature.f, new=Temperature.c), -273.33333, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.f, new=Temperature.c), -17.75, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.f, new=Temperature.c), -17.22222, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=41,      old=Temperature.f, new=Temperature.c), 5, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=100,     old=Temperature.f, new=Temperature.c), 37.77777, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=212,     old=Temperature.f, new=Temperature.c), 100, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=-460,    old=Temperature.f, new=Temperature.f), -460, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=0.05,    old=Temperature.f, new=Temperature.f), 0.05, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=1,       old=Temperature.f, new=Temperature.f), 1, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=41,      old=Temperature.f, new=Temperature.f), 41, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=100,     old=Temperature.f, new=Temperature.f), 100, delta=DELTA)
        self.assertAlmostEqual(Temperature.convert(val=212,     old=Temperature.f, new=Temperature.f), 212, delta=DELTA)


class AngleTestCase(unittest.TestCase):
    def test_deg_to_rad(self):
        self.assertAlmostEqual(Angle.convert(val=180.0, old=Angle.deg, new=Angle.rad), 3.14159, delta=1e-3)
        self.assertAlmostEqual(Angle.convert(val=90.0, old=Angle.deg, new=Angle.rad), 1.5708, delta=1e-3)

    def test_rad_to_deg(self):
        self.assertAlmostEqual(Angle.convert(val=3.14159, old=Angle.rad, new=Angle.deg), 180.0, delta=1e-3)
        self.assertAlmostEqual(Angle.convert(val=1.5708, old=Angle.rad, new=Angle.deg), 90.0, delta=1e-3)


class FractureConversionTestCase(unittest.TestCase):
    def test_mpm(self):
        """ Tests conversions from mpm. """
        self.assertAlmostEqual(Fracture.convert(val=0.05,   old=Fracture.mpm, new=Fracture.mpm), 0.05, delta=DELTA)
        self.assertAlmostEqual(Fracture.convert(val=1.00,   old=Fracture.mpm, new=Fracture.mpm), 1.00, delta=DELTA)
        self.assertAlmostEqual(Fracture.convert(val=100,   old=Fracture.mpm, new=Fracture.mpm), 100, delta=DELTA)


class UnitlessConversionTestCase(unittest.TestCase):
    def test_unitless(self):
        """ Tests conversion attempts with unitless. """
        self.assertAlmostEqual(Unitless.convert(val=0.05), 0.05, delta=DELTA)
        self.assertAlmostEqual(Unitless.convert(val=1.00), 1, delta=DELTA)
        self.assertAlmostEqual(Unitless.convert(val=100), 100, delta=DELTA)
        self.assertAlmostEqual(Unitless.convert(val=0.05, old=Unitless.std_unit), 0.05, delta=DELTA)
        self.assertAlmostEqual(Unitless.convert(val=1.00, new=Unitless.std_unit), 1, delta=DELTA)
        self.assertAlmostEqual(Unitless.convert(val=100, old=Unitless.std_unit, new=Unitless.std_unit), 100, delta=DELTA)


class FractionalConversionTestCase(unittest.TestCase):
    def test_percent_to_percent(self):
        self.assertAlmostEqual(Fractional.convert(val=1, old=Fractional.p, new=Fractional.p), 1, delta=DELTA)
        self.assertAlmostEqual(Fractional.convert(val=10, old=Fractional.p, new=Fractional.p), 10, delta=DELTA)

    def test_percent_to_fractional(self):
        """ Tests conversions attempts with percents. """
        self.assertAlmostEqual(Fractional.convert(val=0.05), 0.05, delta=DELTA)
        self.assertAlmostEqual(Fractional.convert(val=1), 1, delta=DELTA)
        self.assertAlmostEqual(Fractional.convert(val=0.05, old=Fractional.p), 0.0005, delta=DELTA)

    def test_fractional_to_percent(self):
        self.assertAlmostEqual(Fractional.convert(val=1, new=Fractional.p), 100, delta=DELTA)
        self.assertAlmostEqual(Fractional.convert(val=0.01, new=Fractional.p), 1, delta=DELTA)
        self.assertAlmostEqual(Fractional.convert(val=100, new=Fractional.p), 10000, delta=DELTA)
