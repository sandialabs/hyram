/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandiaNationalLaboratories.Hyram
{

    [JsonConverter(typeof(ParameterJsonConverter))]
    public class Parameter : object
    {
        private UnitOfMeasurementConverters _mConverters;

        /// <summary>
        /// Represents generic numeric variable input.
        /// </summary>
        /// <param name="converter">Type of units of measurement, e.g. distance.</param>
        /// <param name="unit">Active unit, e.g. meters.</param>
        /// <param name="value">Current value of parameter.</param>
        /// <param name="label">Name describing value, for display</param>
        /// <param name="minimum">Minimum allowed value</param>
        /// <param name="maximum">Maximum allowed value</param>
        public Parameter(UnitOfMeasurementConverters converter, Enum unit,
                            double value, string label = "",
                            double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity)
        {
            Label = label;
            UnitConverters = converter;
            DisplayUnit = unit;

            MaxValue = maximum;
            MinValue = minimum;
            SetValue(unit, value);
        }

        /// <summary>
        /// Initializes parameter with double, default converter for given unit of measurement (e.g. meters for distance).
        /// </summary>
        public Parameter(UnitOfMeasurementConverters converter,
                            double value, string label = "",
                            double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity)
        {
            UnitConverters = converter;
            MaxValue = maximum;
            MinValue = minimum;
            Label = label;
            DisplayUnit = Converters.GetDefaultUnit(converter);
            SetValue(DisplayUnit, value);
        }

        /// <summary>
        /// Initializes unitless Parameter.
        /// </summary>
        public Parameter(double value, string label, double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity)
        {
            UnitConverters = Converters.Unitless;
            DisplayUnit = UnitlessUnit.Unitless;
            MaxValue = maximum;
            MinValue = minimum;
            Label = label;
            SetValue(UnitlessUnit.Unitless, value);
        }

        /// <summary>
        /// Initializes unitless, label-less Parameter.
        /// </summary>
        public Parameter(double value, double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity)
        {
            UnitConverters = Converters.Unitless;
            DisplayUnit = UnitlessUnit.Unitless;
            MaxValue = maximum;
            MinValue = minimum;
            Label = "";
            SetValue(UnitlessUnit.Unitless, value);
        }

        public string Label { get; }

        public double BaseValue { get; private set; }

        public double MinValue { get; }

        public double MaxValue { get; }

        public UnitOfMeasurementConverters UnitConverters
        {
            get => _mConverters;
            set
            {
                _mConverters = value;
            }
        }

        public Enum DisplayUnit { get; set; }

        public double GetValue(Enum unit = null)
        {
            if (unit == null)
            {
                unit = Converters.GetDefaultUnit(UnitConverters);
            }
            return GetValue(unit.ToString());
        }

        public double GetValue(string units)
        {
            var destinationUnitTypeChanged = false;

            if (units == "Celcius")
            {
                units = "Celsius";
                destinationUnitTypeChanged = true;
            }

            DualConversionProvider provider = null;

            if (destinationUnitTypeChanged)
                if (_mConverters.ContainsKey("Celcius"))
                    UnitConverters.Remove("Celcius");

            if (!_mConverters.ContainsKey(units))
                if (units == "Celsius")
                {
                    provider = new DualConversionProvider
                    {
                        ConversionDelegate = null, ConversionObject = new ConversionData {ConversionFactor = 1}
                    };
                    _mConverters.Add("Celsius", provider);
                }

            var converter = _mConverters[units];
            double result;

            if (converter.ConversionObject == null)
            {
                result = (converter.ConversionDelegate.ConvertFrom(new [] {BaseValue}))[0];
            }
            else
            {
                result = BaseValue / converter.ConversionObject.ConversionFactor;
            }

            return result;
        }

        /// <summary>
        /// Sets value of Parameter according to default unit.
        /// </summary>
        /// <param name="value">New value of Parameter.</param>
        public void SetValue(double value)
        {
            Enum unit = Converters.GetDefaultUnit(UnitConverters);
            SetValue(unit, value);
        }

        /// <summary>
        /// Sets Parameter value according to provided unit type.
        /// </summary>
        /// <param name="sourceUnitTypeEnum">Unit type.</param>
        /// <param name="value">New value of Parameter.</param>
        public void SetValue(Enum sourceUnitTypeEnum, double value)
        {
            string sourceUnitType = sourceUnitTypeEnum.ToString();
            var converter = _mConverters[sourceUnitType];
            double newValue;
            if (converter.ConversionDelegate != null)
            {
                newValue = (converter.ConversionDelegate.ConvertTo(new [] {value}))[0];
            }
            else
            {
                newValue = converter.ConversionObject.ConversionFactor * value;
            }
            if (newValue < MinValue)
            {
                newValue = MinValue;
            }
            if (newValue > MaxValue)
            {
                newValue = MaxValue;
            }

            BaseValue = newValue;
        }
    }

    /// <summary>
    /// Represents displayed parameter in grid view
    /// </summary>
    public class ParameterInput
    {
        public ParameterInput(Parameter param, string label = "")
        {
            Parameter = param;
            Converter = param.UnitConverters;
            Label = label == "" ? param.Label : label;
        }

        public Parameter Parameter { get; set; }
        public string Label { get; }
        public UnitOfMeasurementConverters Converter { get; set; }
        public double OriginalValue { get; set; }

        public static List<ParameterInput> GetParameterInputList(Parameter[] parameters)
        {
            List<ParameterInput> result = new List<ParameterInput>();

            for (int i = 0; i < parameters.Length; i++)
            {
                result.Add(new ParameterInput(parameters[i]));
            }
            return result;
        }
    }


    /// <summary>
    /// Serializes and Deserializes Parameter to JSON, with all properties.
    /// </summary>
    public class ParameterJsonConverter : JsonConverter<Parameter>
    {
        public override Parameter Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string propertyName;
            string displayUnit = "ERROR";
            string unitType = "";
            string label ="ERROR";
            double value = 0;
            double minValue = double.NegativeInfinity;
            double maxValue = double.PositiveInfinity;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    UnitOfMeasurementConverters converter = Converters.GetConverterByName(unitType);
                    var unit = Converters.GetUnitFromString(displayUnit, converter);
                    Parameter parameter = new Parameter(converter, unit, value, label, minValue, maxValue);
                    return parameter;
                }

                propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "Label":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            label = reader.GetString();
                        }
                        break;
                    }
                    case "BaseValue":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            value = reader.GetDouble();
                        }
                        break;
                    }
                    case "MinValue":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            minValue = reader.GetDouble();
                        }
                        break;
                    }
                    case "MaxValue":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            maxValue = reader.GetDouble();
                        }
                        break;
                    }
                    case "DisplayUnit":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            displayUnit = reader.GetString();
                        }
                        break;
                    }
                    case "UnitConverters":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            unitType = reader.GetString();
                        }
                        break;
                    }
                }
            }
            throw new JsonException();  // truncated file

        }

        public override void Write(Utf8JsonWriter writer, Parameter parameter, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(parameter.Label), parameter.Label);
//            writer.WriteNumber(nameof(parameter.BaseValue), parameter.GetValue());
            writer.WriteNumber(nameof(parameter.BaseValue), parameter.GetValue(parameter.DisplayUnit));

            writer.WriteString(nameof(parameter.UnitConverters), parameter.UnitConverters.Name);
            writer.WriteString(nameof(parameter.DisplayUnit), parameter.DisplayUnit.ToString());

            writer.WritePropertyName(nameof(parameter.MinValue));
            if (double.IsNegativeInfinity(parameter.MinValue))
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue(parameter.MinValue);
            }

            writer.WritePropertyName(nameof(parameter.MaxValue));
            if (double.IsPositiveInfinity(parameter.MaxValue))
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue(parameter.MaxValue);
            }

            writer.WriteEndObject();
        }
    }
}