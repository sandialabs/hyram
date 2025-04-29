"""
Copyright 2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the BSD License along with HELPR.

"""
import unittest

import numpy as np
from ..models.fields import IntField, MAX_INT


class TestIntField(unittest.TestCase):

    def setUp(self):
        self.int_field = IntField(label='Test', value=10)

    def test_init(self):
        self.assertEqual(self.int_field.label, 'Test')
        self.assertEqual(self.int_field.value, 10)
        self.assertEqual(self.int_field.max_value, MAX_INT)
        self.assertEqual(self.int_field.min_value, 0)

    def test_str_display(self):
        self.assertEqual(self.int_field.str_display, 'Test: 10')

    def test_from_dict(self):
        data = {
            'label': 'New Test',
            'slug': 'new_test',
            'unit': 'm',
            'value': 20,
            'min_value': '0',
            'max_value': '100'
        }
        self.int_field.from_dict(data)
        self.assertEqual(self.int_field.label, 'New Test')
        self.assertEqual(self.int_field.slug, 'new_test')
        self.assertEqual(self.int_field.unit, 'm')
        self.assertEqual(self.int_field.value, 20)
        self.assertEqual(self.int_field._min_value, 0)
        self.assertEqual(self.int_field._max_value, 100)

    def test_to_dict_when_empty(self):
        self.int_field.value = None
        dct = self.int_field.to_dict()
        self.assertEqual(dct['label'], 'Test')
        self.assertEqual(dct['slug'], 'test')
        self.assertEqual(dct['unit'], None)
        self.assertEqual(dct['value'], None)
        self.assertEqual(dct['min_value'], '0')
        self.assertEqual(dct['max_value'], '2e+09')

    def test_from_dict_when_null(self):
        data = {
            'label': 'New Test',
            'slug': 'new_test',
            'unit': 'm',
            'value': None,
            'min_value': '0',
            'max_value': '100'
        }
        self.int_field.from_dict(data)
        self.assertEqual(self.int_field.label, 'New Test')
        self.assertEqual(self.int_field.slug, 'new_test')
        self.assertEqual(self.int_field.unit, 'm')
        self.assertEqual(self.int_field.value, None)
        self.assertTrue(self.int_field.is_null)
        self.assertEqual(self.int_field._min_value, 0)
        self.assertEqual(self.int_field._max_value, 100)

    def test_from_dict_with_inf_max(self):
        data = {
            'label': 'New Test',
            'slug': 'new_test',
            'unit': 'm',
            'value': 20,
            'min_value': '0',
            'max_value': 'infinity'
        }
        self.int_field.from_dict(data)
        self.assertEqual(self.int_field.label, 'New Test')
        self.assertEqual(self.int_field.slug, 'new_test')
        self.assertEqual(self.int_field.unit, 'm')
        self.assertEqual(self.int_field.value, 20)
        self.assertEqual(self.int_field._min_value, 0)
        self.assertEqual(self.int_field._max_value, MAX_INT)
