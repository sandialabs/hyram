"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import unittest

from ..utils.distributions import BaseChoiceList, Distributions
from ..utils.helpers import InputStatus
from ..models.fields import StringField, ChoiceField, BoolField


class StringFieldTestCase(unittest.TestCase):
    def setUp(self):
        self.string_field = StringField("Field 1")

    def test_is_instance_of_fieldBase(self):
        self.assertIsInstance(self.string_field, StringField)

    def test_slug_generation(self):
        self.assertEqual(self.string_field.slug, "field_1")

    def test_default_value(self):
        self.assertEqual(self.string_field.value, None)

    def test_value_assignment(self):
        self.string_field.value = "Sample Value"
        self.assertEqual(self.string_field.value, "Sample Value")

    def test_label_assignment(self):
        self.assertEqual(self.string_field.label, "Field 1")

    def test_dtype_assignment(self):
        self.assertEqual(self.string_field._dtype, str)

class BoolFieldTestCase(unittest.TestCase):

    def setUp(self):
        self.label = 'field_label'
        self.value = True
        self.slug = 'field_slug'
        self.bool_field = BoolField(self.label, self.value, self.slug)

    def test_init(self):
        self.assertEqual(self.bool_field._dtype, bool)
        self.assertEqual(self.bool_field._value, True)

    def test_value_update(self):
        self.bool_field._value = False
        self.assertEqual(self.bool_field._value, False)

    def test_slug_update(self):
        new_slug = 'new_slug'
        self.bool_field._slug = new_slug
        self.assertEqual(self.bool_field._slug, new_slug)

    def test_label_update(self):
        new_label = 'new_label'
        self.bool_field._label = new_label
        self.assertEqual(self.bool_field._label, new_label)

class ChoiceFieldTestCase(unittest.TestCase):

    def setUp(self):
        self.field = ChoiceField(label="Test Label", choices=Distributions)

    def test_initial_value(self):
        self.assertEqual(self.field._value, Distributions.keys[0])

    def test_str_display(self):
        self.assertEqual(self.field.str_display, f"Test Label: {Distributions.labels[0]}")

    def test_set_value_from_key(self):
        self.field.set_value_from_key('tnor')
        self.assertEqual(self.field._value, 'tnor')

    def test_set_value_from_invalid_key(self):
        with self.assertRaises(ValueError):
            self.field.set_value_from_key('invalid')

    def test_set_value_from_index(self):
        self.field.set_value_from_index(1)
        self.assertEqual(self.field._value, Distributions.keys[1])

    def test_set_value_from_invalid_index(self):
        with self.assertRaises(ValueError):
            self.field.set_value_from_index(10)

    def test_get_value_index(self):
        self.assertEqual(self.field.get_value_index(), 0)

    def test_get_value_display(self):
        self.assertEqual(self.field.get_value_display(), Distributions.labels[0])

    def test_get_choice_keys(self):
        self.assertEqual(self.field.get_choice_keys(), Distributions.keys)

    def test_get_choice_displays(self):
        self.assertEqual(self.field.get_choice_displays(), Distributions.labels)

    def test_from_dict(self):
        data = {'label': 'Changed', 'value': 'det'}
        self.field.from_dict(data)
        self.assertEqual(self.field.label, 'Changed')
        self.assertEqual(self.field._value, 'det')

    def test_from_dict_with_slug(self):
        data = {'label': 'Changed', 'value': 'det', 'slug': 'test_slug'}
        self.field.from_dict(data)
        self.assertEqual(self.field.label, 'Changed')
        self.assertEqual(self.field._value, 'det')
        self.assertEqual(self.field.slug, 'test_slug')
