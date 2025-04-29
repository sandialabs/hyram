"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import unittest
from unittest.mock import MagicMock

from ..utils.units_of_measurement import Distance, Temperature
from ..utils.helpers import InputStatus
from ..models.fields import NumField


class NumFieldTestCase(unittest.TestCase):

    def setUp(self):
        """Set up test fixtures"""
        self.num_field = NumField(label='TestLabel', value=10.0, unit_type=Distance, unit='m', min_value=0,
                                  max_value=100)

    def test_initial_value(self):
        """Test that the initial value is set correctly"""
        self.assertEqual(self.num_field.value, 10.0)

    def test_set_value(self):
        """Test setting the value"""
        self.num_field.value = 20.0
        self.assertEqual(self.num_field.value, 20.0)

    def test_set_value_out_of_range_ignored_new_value(self):
        """Test setting a value outside the allowed range"""
        old_val = self.num_field.value
        self.assertFalse(self.num_field.in_range(200.0))
        self.num_field.value = 200.0
        self.assertEqual(self.num_field.value, old_val)

    def test_min_value(self):
        """Test getting the minimum value"""
        self.assertEqual(self.num_field.min_value, 0.0)

    def test_max_value(self):
        """Test getting the maximum value"""
        self.assertEqual(self.num_field.max_value, 100.0)

    def test_in_range(self):
        """Test value is within the range"""
        self.assertTrue(self.num_field.in_range(50.0))
        self.assertFalse(self.num_field.in_range(200.0))

    def test_check_valid(self):
        """Test value validation"""
        # Initial check, assuming default value is within the allowed range
        resp = self.num_field.check_valid()
        self.assertEqual(resp.status, InputStatus.GOOD)

        # Set a value beyond the default maximum limit and check
        self.num_field._value = self.num_field.max_value + 1
        resp = self.num_field.check_valid()
        self.assertEqual(resp.status, InputStatus.ERROR)

        # Assuming the minimum value is 0, set a value that's lower and check
        self.num_field._value = self.num_field.min_value - 1
        resp = self.num_field.check_valid()
        self.assertEqual(resp.status, InputStatus.ERROR)

        # Set the value to a valid range and check again
        self.num_field.value = (self.num_field.min_value + self.num_field.max_value) / 2
        resp = self.num_field.check_valid()
        self.assertEqual(resp.status, InputStatus.GOOD)

    def test_unit_conversion(self):
        """Test unit conversion"""
        self.num_field.value = 1.0
        self.assertEqual(self.num_field.value, 1)

        self.num_field.set_unit_from_display('mm')
        self.assertEqual(self.num_field.unit, 'mm')  # Assuming 1 meter = 1000 millimeters
        self.assertEqual(self.num_field.value, 1)
        self.assertEqual(self.num_field.value_raw, 0.001)

    def test_notify_changed(self):
        """Test that notify_changed is called when value is changed"""
        self.num_field.changed = MagicMock()
        self.num_field.value = 30.0
        self.num_field.changed.notify.assert_called_once()

    def test_to_dict(self):
        """Test converting the object to a dictionary"""
        dict_repr = self.num_field.to_dict()
        self.assertEqual(dict_repr['label'], 'TestLabel')
        self.assertEqual(dict_repr['value'], 10.0)
        self.assertEqual(dict_repr['unit_type'], 'dist')

    def test_set_value_none(self):
        """Test setting the value to None"""
        self.num_field.value = None
        self.assertIsNone(self.num_field.value)
        self.assertTrue(self.num_field.is_null)
        self.assertEqual(self.num_field.value_raw, None)
        self.assertEqual(self.num_field.value_str, "")

    def test_get_value_none(self):
        """Test getting the value when set to None"""
        self.num_field.value = None
        self.assertEqual(self.num_field.value, None)


class NumFieldTemperatureTestCase(unittest.TestCase):
    def setUp(self):
        self.num_field = NumField(
            label="Temperature",
            value=25.0,  # Default value in Celsius
            unit_type=Temperature,
            min_value=-273.15,  # Absolute zero in Celsius
            max_value=100.0,  # Boiling point of water in Celsius
            unit=Temperature.c,
        )

    def test_initial_value(self):
        self.assertEqual(self.num_field.value, 25.0)
        self.assertEqual(self.num_field.unit, Temperature.c)

    def test_unit_conversion(self):
        # Set value in Celsius and check value in Kelvin
        self.num_field.value = 0.0
        self.assertAlmostEqual(self.num_field._value, 273.15, places=2)

        self.num_field.set_unit_from_display("K")
        self.assertAlmostEqual(self.num_field.value, 0, places=2)
        self.assertAlmostEqual(self.num_field._value, 0, places=2)

        # Set value in Kelvin and check value in Fahrenheit
        self.num_field.value = 373.15
        self.assertAlmostEqual(self.num_field._value, 373.15, places=2)

        self.num_field.set_unit_from_display("&deg;F")
        self.assertAlmostEqual(self.num_field.value, 373.15, places=2)
        self.assertAlmostEqual(self.num_field._value, 462.68, places=2)

    def test_set_value_in_range(self):
        self.num_field.value = 50.0
        self.assertTrue(self.num_field.in_range(self.num_field.value))
        self.assertEqual(self.num_field.check_valid().status, InputStatus.GOOD)

    def test_set_value_out_of_range_high_is_ignored(self):
        self.assertFalse(self.num_field.in_range(150))
        self.num_field.value = 150.0
        self.assertEqual(self.num_field.check_valid().status, InputStatus.GOOD)

    def test_set_value_out_of_range_low_is_ignored(self):
        self.assertFalse(self.num_field.in_range(-300))
        self.num_field.value = -300.0
        self.assertEqual(self.num_field.check_valid().status, InputStatus.GOOD)

    def test_min_value(self):
        self.num_field.value = self.num_field.min_value
        self.assertEqual(self.num_field.value, -273.15)
        self.assertTrue(self.num_field.in_range(self.num_field.value))

    def test_max_value(self):
        self.num_field.value = self.num_field.max_value
        self.assertEqual(self.num_field.value, 100.0)
        self.assertTrue(self.num_field.in_range(self.num_field.value))

    def test_notify_changed(self):
        def mock_notify():
            self.notified = True

        self.num_field.notify_changed = mock_notify
        self.notified = False
        self.num_field.value = 80.0
        self.assertTrue(self.notified)

    def test_to_dict(self):
        expected_dict = {
            'label': 'Temperature',
            'value': 298.15,
            'unit': 'c',
            'min_value': '0',
            'max_value': '373.1',
            'slug': 'temperature',
            'unit_type': 'temp'
        }
        self.assertEqual(self.num_field.to_dict(), expected_dict)

if __name__ == '__main__':
    unittest.main()
