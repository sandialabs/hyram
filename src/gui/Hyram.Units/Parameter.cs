/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandiaNationalLaboratories.Hyram
{
    public class SensitivityDistribution
    {
        // sensitivity distribution options
        public static ModelPair Deterministic = new ModelPair("Deterministic", "det");
        public static ModelPair Normal = new ModelPair("Normal", "nor");
        public static ModelPair LogNormal = new ModelPair("LogNormal", "log");
        public static ModelPair Uniform = new ModelPair("Uniform", "uni");

    }
    public class UncertaintyType
    {
        public static ModelPair None = new ModelPair("None", "none");
        public static ModelPair Aleatory = new ModelPair("Aleatory", "ale");
        public static ModelPair Epistemic = new ModelPair("Epistemic", "epi");
    }

    [JsonConverter(typeof(ParameterJsonConverter))]
    public class Parameter : object
    {
        private UnitOfMeasurementConverters _mConverters;
        public event EventHandler ParameterChangedEvent;

        public string Label { get; }
        public double BaseValue { get; private set; }
        public double MinValue { get; }
        public double MaxValue { get; }

        // uncertainty inputs, ignored if distr is null or deterministic
        public bool CanBeUncertain { get; set; }
        public ModelPair Distr = SensitivityDistribution.Deterministic;
        public ModelPair Uncertainty = UncertaintyType.None;
        public double? ParamA { get; set; }
        public double? ParamB { get; set; }

        // workaround to allow null values without requiring double? type everywhere.
        public bool MaybeNull = false;
        public bool IsNull = false;


        public UnitOfMeasurementConverters UnitConverters
        {
            get => _mConverters;
            set
            {
                _mConverters = value;
            }
        }
        public Enum DisplayUnit { get; set; }


        /// <summary>
        /// Represents generic numeric variable input.
        /// Parameter typically holds float value; however, it can be made nullable via MaybeNull flag.
        /// If nullable, use specialized getter and setter functions to obtain/set null value.
        /// </summary>
        /// <param name="converter">Type of units of measurement, e.g. distance.</param>
        /// <param name="unit">Active unit, e.g. meters.</param>
        /// <param name="value">Current value of parameter.</param>
        /// <param name="label">Name describing value, for display</param>
        /// <param name="minimum">Minimum allowed value</param>
        /// <param name="maximum">Maximum allowed value</param>
        /// <param name="distr">Distribution, if parameter value is uncertain</param>
        public Parameter(UnitOfMeasurementConverters converter, Enum unit,
                            double value, string label = "",
                            double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity,
                            bool maybeNull = false, bool isNull = false,
                            bool canBeUncertain = false,
                            double?  paramA = null, double? paramB = null)
        {
            Label = label;
            UnitConverters = converter;
            DisplayUnit = unit;

            MaxValue = maximum;
            MinValue = minimum;

            MaybeNull = maybeNull;
            SetValue(unit, value);
            // set this after SetValue, otherwise SetValue will reset it.
            IsNull = isNull;

            CanBeUncertain = canBeUncertain;
            ParamA = paramA;
            ParamB = paramB;
        }

        /// <summary>
        /// Initializes parameter with double, default converter for given unit of measurement (e.g. meters for distance).
        /// </summary>
        public Parameter(UnitOfMeasurementConverters converter,
                            double value, string label = "",
                            double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity,
                            bool maybeNull = false, bool isNull = false,
                            bool canBeUncertain = false, ModelPair distr = null)
        {
            UnitConverters = converter;
            MaxValue = maximum;
            MinValue = minimum;
            Label = label;
            MaybeNull = maybeNull;

            DisplayUnit = Converters.GetDefaultUnit(converter);
            SetValue(DisplayUnit, value);
            IsNull = isNull;

            CanBeUncertain = canBeUncertain;
            Distr = distr ?? SensitivityDistribution.Deterministic;
        }

        /// <summary>
        /// Initializes unitless Parameter.
        /// </summary>
        public Parameter(double value, string label, double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity,
                         bool canBeUncertain = false)
        {
            UnitConverters = Converters.Unitless;
            DisplayUnit = UnitlessUnit.Unitless;
            MaxValue = maximum;
            MinValue = minimum;
            Label = label;
            SetValue(UnitlessUnit.Unitless, value);

            CanBeUncertain = canBeUncertain;
        }

        /// <summary>
        /// Initializes unitless, label-less Parameter.
        /// </summary>
        public Parameter(double value, double minimum = double.NegativeInfinity, double maximum = double.PositiveInfinity,
                         bool canBeUncertain = false)
        {
            UnitConverters = Converters.Unitless;
            DisplayUnit = UnitlessUnit.Unitless;
            MaxValue = maximum;
            MinValue = minimum;
            Label = "";
            SetValue(UnitlessUnit.Unitless, value);

            CanBeUncertain = canBeUncertain;
        }

        /// <summary>
        /// Initializes Parameter with uncertainty.
        /// </summary>
        public Parameter(string label, UnitOfMeasurementConverters converter)
        {
            UnitConverters = converter;
            DisplayUnit = Converters.GetDefaultUnit(converter);
            Label = label;
            CanBeUncertain = true;

            MinValue = double.NegativeInfinity;
            MaxValue = double.PositiveInfinity;
            SetValue(DisplayUnit, 0);
        }


        public double GetValue(Enum unit = null)
        {
            if (unit == null)
            {
                unit = Converters.GetDefaultUnit(UnitConverters);
            }
            return GetValue(unit.ToString());
        }

        public double? GetValueMaybeNull(Enum unit = null)
        {
            if (MaybeNull && IsNull)
            {
                return null;
            }
            else
            {
                return GetValue(unit);
            }
        }

        public double GetDisplayValue()
        {
            return GetValue(DisplayUnit);
        }

        public double? GetDisplayValueMaybeNull()
        {
            if (MaybeNull && IsNull)
            {
                return null;
            }
            else
            {
                return GetValue(DisplayUnit);
            }
        }

        public double GetValue(string units)
        {
            var destinationUnitTypeChanged = false;

            // TODO (Cianan): not sure what this was for. Bad spelling in previous version?
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
        /// Sets value to null, if allowed, by flag. Note that stored value does not change.
        /// </summary>
        public void SetValueToNull()
        {
            if (MaybeNull)
            {
                IsNull = true;
            }
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
            var converter = _mConverters[sourceUnitTypeEnum.ToString()];
            double newValue;
            if (converter.ConversionDelegate != null)
            {
                newValue = (converter.ConversionDelegate.ConvertTo(new [] {value}))[0];
            }
            else
            {
                newValue = converter.ConversionObject.ConversionFactor * value;
            }
            if (newValue < MinValue) newValue = MinValue;
            if (newValue > MaxValue) newValue = MaxValue;

            BaseValue = newValue;
            if (IsNull) IsNull = false;

            ParameterChangedEvent?.Invoke(this, EventArgs.Empty);

        }

        public void SetValueFromDisplay(double value)
        {
            SetValue(DisplayUnit, value);
        }

        ////////////////////////////////////////////
        /// Getters & Setters for uncertainty fields
        
        /// <summary>
        /// Sets param A value according to provided unit type.
        /// </summary>
        /// <param name="sourceUnitTypeEnum">Unit type.</param>
        /// <param name="value">New value of param A property.</param>
        public void SetParamA(Enum sourceUnitTypeEnum, double? value)
        {
            if (value == null)
            {
                ParamA = null;
                return;
            }

            double val = (double)value;
            var converter = _mConverters[sourceUnitTypeEnum.ToString()];
            double newValue;
            if (converter.ConversionDelegate != null)
            {
                newValue = (converter.ConversionDelegate.ConvertTo(new [] {val}))[0];
            }
            else
            {
                newValue = converter.ConversionObject.ConversionFactor * val;
            }
            if (newValue < MinValue) newValue = MinValue;
            if (newValue > MaxValue) newValue = MaxValue;

            ParamA = newValue;
//            ParameterChangedEvent?.Invoke(this, EventArgs.Empty);
        }

        public void SetParamAFromDisplay(double? value)
        {
            SetParamA(DisplayUnit, value);
        }

        public double? DisplayParamA(string units)
        {
            if (ParamA == null) return null;

            if (units == null) units = DisplayUnit.ToString();

//            if (!_mConverters.ContainsKey(units))
//                if (units == "Celsius")
//                {
//                    var provider = new DualConversionProvider
//                    {
//                        ConversionDelegate = null, ConversionObject = new ConversionData {ConversionFactor = 1}
//                    };
//                    _mConverters.Add("Celsius", provider);
//                }

            var converter = _mConverters[units];
            double result;

            if (converter.ConversionObject == null)
            {
                result = (converter.ConversionDelegate.ConvertFrom(new [] {(double)ParamA}))[0];
            }
            else
            {
                result = (double)ParamA / converter.ConversionObject.ConversionFactor;
            }

            return result;
        }

        public double? DisplayParamA()
        {
            return DisplayParamA(DisplayUnit.ToString());
        }


        /// <summary>
        /// Sets param B value according to provided unit type.
        /// </summary>
        /// <param name="sourceUnitTypeEnum">Unit type.</param>
        /// <param name="value">New value of param A property.</param>
        public void SetParamB(Enum sourceUnitTypeEnum, double? value)
        {
            if (value == null)
            {
                ParamB = null;
                return;
            }

            double val = (double)value;
            var converter = _mConverters[sourceUnitTypeEnum.ToString()];
            double newValue;
            if (converter.ConversionDelegate != null)
            {
                newValue = (converter.ConversionDelegate.ConvertTo(new [] {val}))[0];
            }
            else
            {
                newValue = converter.ConversionObject.ConversionFactor * val;
            }
            if (newValue < MinValue) newValue = MinValue;
            if (newValue > MaxValue) newValue = MaxValue;

            ParamB = newValue;
        }

        public void SetParamBFromDisplay(double? value)
        {
            SetParamB(DisplayUnit, value);
        }

        public double? DisplayParamB(string units)
        {
            if (ParamB == null) return null;

            if (units == null) units = DisplayUnit.ToString();

            var converter = _mConverters[units];
            double result;
            if (converter.ConversionObject == null)
            {
                result = (converter.ConversionDelegate.ConvertFrom(new [] {(double)ParamB}))[0];
            }
            else
            {
                result = (double)ParamB / converter.ConversionObject.ConversionFactor;
            }

            return result;
        }

        public double? DisplayParamB()
        {
            return DisplayParamB(DisplayUnit.ToString());
        }

        public bool IsUncertain()
        {
            return CanBeUncertain && !IsNull && Distr != SensitivityDistribution.Deterministic;
        }

        public Dictionary<string, string> GetDictionary()
        {
            var result = new Dictionary<string, string>
            {
                {"value", GetValueMaybeNull().ToString()},
                {"distr", Distr.GetKey() },
                {"uncertainty", Uncertainty.GetKey() },
                {"param_a", ParamA.ToString() },
                {"param_b", ParamB.ToString() },
            };
            return result;
        }
        
        public override string ToString()
        {
            return $"Param: {Label} {GetValue()} [{DisplayUnit.ToString()}] | nullable? {MaybeNull}, IsNull {IsNull}";
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
//        public double OriginalValue { get; set; }

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
            bool maybeNull = false;
            bool isNull = false;
            // sensitivity inputs
            bool canBeUncertain = false;
            var distr = SensitivityDistribution.Deterministic;
            var uncertainty = UncertaintyType.None;
            double? paramA = null;
            double? paramB = null;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    // Massage old values of -1 (e.g. release overrides) into null
                    if (Math.Abs(minValue) < 1E-5 && value < 0)
                    {
                        value = 0;
                        if (maybeNull)
                        {
                            isNull = true;
                        }
                    }
                    UnitOfMeasurementConverters converter = Converters.GetConverterByName(unitType);
                    var unit = Converters.GetUnitFromString(displayUnit, converter);
//                    Debug.WriteLine($"{label} nullable {maybeNull} isnull {isNull}");
                    Parameter param = new Parameter(converter, unit, value, label,
                                                        minimum: minValue, maximum: maxValue,
                                                        maybeNull: maybeNull, isNull: isNull,
                                                        canBeUncertain:canBeUncertain, paramA:paramA, paramB:paramB);
                    param.Distr = distr;
                    param.Uncertainty = uncertainty;
//                    Debug.WriteLine($"{param.Label} nullable {param.MaybeNull} isnull {param.IsNull}");
                    return param;
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
                    case "MaybeNull":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            maybeNull = reader.GetBoolean();
                        }
                        break;
                    }
                    case "IsNull":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            isNull = reader.GetBoolean();
                        }
                        break;
                    }

                    case "CanBeUncertain":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            canBeUncertain = reader.GetBoolean();
                        }
                        break;
                    }

                    case "Distr":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            var key = reader.GetString();
                            if (key == "uni") distr = SensitivityDistribution.Uniform;
                            else if (key == "nor") distr = SensitivityDistribution.Normal;
                            else if (key == "log") distr = SensitivityDistribution.LogNormal;
                            else distr = SensitivityDistribution.Deterministic;
                        }
                        break;
                    }

                    case "Uncertainty":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            var key = reader.GetString();
                            if (key == "ale") uncertainty = UncertaintyType.Aleatory;
                            else if (key == "epi") uncertainty = UncertaintyType.Epistemic;
                            else uncertainty = UncertaintyType.None;
                        }
                        break;
                    }

                    case "ParamA":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            paramA = reader.GetDouble();
                        }
                        break;
                    }
                    case "ParamB":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            paramB = reader.GetDouble();
                        }
                        break;
                    }
                }
            }
            throw new JsonException();  // truncated file
        }

        public override void Write(Utf8JsonWriter writer, Parameter parameter, JsonSerializerOptions options)
        {
//            Debug.WriteLine($"Saving {parameter.Label}");
            writer.WriteStartObject();

            writer.WriteString(nameof(parameter.Label), parameter.Label);
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

            writer.WriteBoolean(nameof(parameter.MaybeNull), parameter.MaybeNull);
            writer.WriteBoolean(nameof(parameter.IsNull), parameter.IsNull);

            writer.WriteBoolean(nameof(parameter.CanBeUncertain), parameter.CanBeUncertain);

            writer.WriteString(nameof(parameter.Distr), parameter.Distr.GetKey());
            writer.WriteString(nameof(parameter.Uncertainty), parameter.Uncertainty.GetKey());

            writer.WritePropertyName(nameof(parameter.ParamA));
            if (parameter.ParamA == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue((double)parameter.ParamA);
            }

            writer.WritePropertyName(nameof(parameter.ParamB));
            if (parameter.ParamB == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                writer.WriteNumberValue((double)parameter.ParamB);
            }

            writer.WriteEndObject();
        }
    }
}