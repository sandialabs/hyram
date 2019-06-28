// Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
// Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
// 
// This file is part of HyRAM (Hydrogen Risk Assessment Models).
// 
// HyRAM is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// HyRAM is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

using System;

namespace JrConversions
{
    [Serializable]
    public class NdConvertibleValue : object, IIValueDelegate
    {
        private UnitOfMeasurementConverters _mConverters;

        private double _mMaxValue = double.PositiveInfinity;

        private double _mMinValue = double.NegativeInfinity;

        public NdConvertibleValue(UnitOfMeasurementConverters conversionDelegate, Enum sourceValueUnit,
            double[] valueToSet,
            double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity)
        {
            string sourceValueUnitName = null;
            sourceValueUnitName = sourceValueUnit.ToString();
            Converters = conversionDelegate;
            _mMaxValue = maximum;
            _mMinValue = minimum;


            SetValue(sourceValueUnitName, valueToSet);
            InputUnit = sourceValueUnit;
        }

        public double[] BaseValue { get; private set; }

        public Enum InputUnit { get; }

        public double MinValue
        {
            get => _mMinValue;
            set => _mMinValue = value;
        }

        public double MaxValue
        {
            get => _mMaxValue;
            set => _mMaxValue = value;
        }

        public UnitOfMeasurementConverters Converters
        {
            get => _mConverters;
            set
            {
                foreach (var thisProvider in value.Values)
                    if (thisProvider.HasBadConversionFactor())
                        throw new Exception("A conversion provider has a bad conversion factor: " +
                                            thisProvider.ConversionDataObject.Name);

                _mConverters = value;
            }
        }


        #region iValueDelegate Members

        string IIValueDelegate.DisplayValue
        {
            get
            {
                var result = "";

                for (var index = 0; index < BaseValue.Length; index++)
                    if (index == 0)
                        result = BaseValue[index].ToString();
                    else
                        result += ", " + BaseValue[index];

                if (BaseValue.Length > 1) result = "(" + result + ")";

                return result;
            }
        }

        #endregion

        public double[] GetValue(Enum destinationUnit)
        {
            return GetValue(destinationUnit.ToString());
        }

        public double[] GetValue(string destinationUnitType)
        {
            var destinationUnitTypeChanged = false;

            if (destinationUnitType == "Celcius")
            {
                destinationUnitType = "Celsius";
                destinationUnitTypeChanged = true;
            }

            NdDualConversionProvider provider = null;

            if (destinationUnitTypeChanged)
                if (_mConverters.ContainsKey("Celcius"))
                    Converters.Remove("Celcius");

            if (!_mConverters.ContainsKey(destinationUnitType))
                if (destinationUnitType == "Celsius")
                {
                    provider = new NdDualConversionProvider();
                    provider.ConversionDelegate = null;
                    provider.ConversionDataObject = new NdUnitAgConversionData();
                    provider.ConversionDataObject.ConversionFactor = 1;
                    _mConverters.Add("Celsius", provider);
                }

            var converter = _mConverters[destinationUnitType];
            double[] result = null;

            if (converter.ConversionDataObject == null)
            {
                result = converter.ConversionDelegate.ConvertFrom(BaseValue);
            }
            else
            {
                result = new double[BaseValue.Length];
                for (var index = 0; index < BaseValue.Length; index++)
                    result[index] = BaseValue[index] / converter.ConversionDataObject.ConversionFactor;
            }

            return result;
        }

        public void SetValue(Enum sourceUnitType, double[] value)
        {
            SetValue(sourceUnitType.ToString(), value);
        }

        public void SetValue(string sourceUnitType, double[] value)
        {
            var converter = _mConverters[sourceUnitType];
            if (converter.ConversionDelegate != null)
            {
                var newValues = converter.ConversionDelegate.ConvertTo(value);
                for (var index = 0; index < value.Length; index++)
                {
                    if (newValues[index] < _mMinValue) newValues[index] = _mMinValue;

                    if (newValues[index] > _mMaxValue) newValues[index] = _mMaxValue;
                }

                BaseValue = newValues;
            }
            else
            {
                BaseValue = new double[value.Length];
                for (var index = 0; index < value.Length; index++)
                {
                    var newValue = converter.ConversionDataObject.ConversionFactor * value[index];
                    if (newValue < _mMinValue) newValue = _mMinValue;

                    if (newValue > _mMaxValue) newValue = _mMaxValue;

                    BaseValue[index] = newValue;
                }
            }
        }

        public void EnsureValueIsTruncatedInt()
        {
            throw new NotImplementedException();
        }

        // This routine is used to allow unchanging integer values to be 
        // stored in the base double[] type. Redundant, probably unneeded, 
        // but important for things like repeatable random number seeds.
        // Keep in mind that this is really only useful for unitless
        // values, because other values can be converted upon retrieval
        // to values with fractional parts.
        public void EnsureBaseValueIsTruncatedInt()
        {
            for (var index = 0; index < BaseValue.Length; index++) BaseValue[index] = Math.Truncate(BaseValue[index]);
        }
    }
}