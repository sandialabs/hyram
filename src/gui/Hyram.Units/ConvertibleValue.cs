/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;

namespace SandiaNationalLaboratories.Hyram
{
    [Serializable]
    public class ConvertibleValue : object
                
    {
        private UnitOfMeasurementConverters _mConverters;

        public ConvertibleValue(UnitOfMeasurementConverters conversionDelegate, Enum sourceValueUnit,
            double[] valueToSet,
            double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity)
        {
            string sourceValueUnitName = null;
            sourceValueUnitName = sourceValueUnit.ToString();
            Converters = conversionDelegate;
            MaxValue = maximum;
            MinValue = minimum;


            SetValue(sourceValueUnitName, valueToSet);
            InputUnit = sourceValueUnit;
        }

        public double[] BaseValue { get; private set; }

        public Enum InputUnit { get; }

        public double MinValue { get; set; } = double.NegativeInfinity;

        public double MaxValue { get; set; } = double.PositiveInfinity;

        public UnitOfMeasurementConverters Converters
        {
            get => _mConverters;
            set
            {
                foreach (var thisProvider in value.Values)
                    if (thisProvider.HasBadConversionFactor())
                        throw new Exception("A conversion provider has a bad conversion factor: " +
                                            thisProvider.ConversionObject.Name);

                _mConverters = value;
            }
        }

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

            DualConversionProvider provider = null;

            if (destinationUnitTypeChanged)
                if (_mConverters.ContainsKey("Celcius"))
                    Converters.Remove("Celcius");

            if (!_mConverters.ContainsKey(destinationUnitType))
                if (destinationUnitType == "Celsius")
                {
                    provider = new DualConversionProvider
                    {
                        ConversionDelegate = null, ConversionObject = new ConversionData {ConversionFactor = 1}
                    };
                    _mConverters.Add("Celsius", provider);
                }

            var converter = _mConverters[destinationUnitType];
            double[] result = null;

            if (converter.ConversionObject == null)
            {
                result = converter.ConversionDelegate.ConvertFrom(BaseValue);
            }
            else
            {
                result = new double[BaseValue.Length];
                for (var index = 0; index < BaseValue.Length; index++)
                    result[index] = BaseValue[index] / converter.ConversionObject.ConversionFactor;
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
                    if (newValues[index] < MinValue) newValues[index] = MinValue;

                    if (newValues[index] > MaxValue) newValues[index] = MaxValue;
                }

                BaseValue = newValues;
            }
            else
            {
                BaseValue = new double[value.Length];
                for (var index = 0; index < value.Length; index++)
                {
                    var newValue = converter.ConversionObject.ConversionFactor * value[index];
                    if (newValue < MinValue) newValue = MinValue;

                    if (newValue > MaxValue) newValue = MaxValue;

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