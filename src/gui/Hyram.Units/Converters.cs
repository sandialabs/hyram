/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;

namespace SandiaNationalLaboratories.Hyram
{
    [Serializable]
    public enum ConverterName
    {
        Distance,
        Area,
        Pressure,
        Temperature,
        Unitless,
        Time,
        JulianTime,
        ElapsingTime,
        Frequency,
        Energy,
        Volume,
        Angle,
        Speed,
        Density,
        Mass,
        VolumetricFlow,
        SpecificEnergy,
        MassFlow
    }

    [Serializable]
    public class Converters
    {
        private const double VolLiterConversionFactor = 1D;
        private const double VolCubicCentimeterConversionFactor = .001D;
        private const double VolCubicDecimeterConversionFactor = VolLiterConversionFactor;
        private const double VolCubicDekameterConversionFactor = 1000000D;
        private const double VolCubicFootConversionFactor = 28.316846592D;
        private const double VolCubicInchConversionFactor = 0.016387064D;
        private const double VolCubicKilometerConversionFactor = 1000000000000D;
        private const double VolCubicMeterConversionFactor = 1000D;
        private const double VolCubicMileConversionFactor = 4168181825400D;
        private const double VolCubicMicrometerConversionFactor = 1.0e-15D;
        private const double VolCubicMillimeterConversionFactor = 0.000001D;
        private const double VolCubicYardConversionFactor = 764.55485798D;
        private const double VolDeciliterConversionFactor = 0.1D;
        private const double VolDekaliterConversionFactor = 10D;
        private const double VolKiloliterConversionFactor = 1000D;
        private const double VolMegaliterConversionFactor = 1000000D;
        private const double VolMicroliterConversionFactor = 0.000001D;
        private const double VolMilliliterConversionFactor = 0.001D;

        private const double MpaConversionFactor = 1D;
        private const double BarConversionFactor = .1;
        private const double AtmosphereInPsi = 14.7;
        private const double AtmConversionFactor = 0.101325;
        private const double PsiConversionFactor = 0.00689475728;

        private const double JouleConversionUnit = 1D;
        private const double KwhConversionUnit = 3600000;

        // To convert TO meters, the value is multiplied by a conversion factor to convert to the stated unit.
        // To go the other direction, the value is divided by its corresponding conversion factor.
        private const double MeterConversionFactor = 1D;
        private const double MileConversionFactor = 1609.347;
        private const double FootConversionFactor = 0.30480060960D;
        private const double AstronomicalUnitConversionFactor = 149597870700D;
        private const double CentimeterConversionFactor = 0.01D;
        private const double MillimeterConversionFactor = 0.001D;
        private static readonly UnitsOfMeasurement _mUnits = new UnitsOfMeasurement();
        private static readonly double _paConversionFactor = MpaConversionFactor / 1000000;
        private static readonly double _kpaConversionFactor = _paConversionFactor * 1000;
        private static readonly double _jpCmConversionFactor = _paConversionFactor; // joules/m**3
        private static readonly double _botuConversionUnit = KwhConversionUnit;
        private static readonly double _inchConversionFactor = FootConversionFactor / 12D;
        private static readonly double _yardConversionFactor = FootConversionFactor * 3D;

        public static Enum GetDefaultUnit(UnitOfMeasurementConverters converter)
        {
            Enum result = null;
            if (converter == Distance)
            {
                result = DistanceUnit.Meter;
            }
            else if (converter == Area)
            {
                result = AreaUnit.SqMeters;
            }
            else if (converter == Pressure)
            {
                result = PressureUnit.MPa;
            }
            else if (converter == Temperature)
            {
                result = TempUnit.Kelvin;
            }
            else if (converter == JulianTime)
            {
                result = JulianTimeConversionUnit.Day;
            }
            else if (converter == ElapsingTime)
            {
                result = TimeUnit.Second;
            }
            else if (converter == Frequency)
            {
                result = FreqUnit.PerYear;
            }
            else if (converter == Energy)
            {
                result = EnergyUnit.Joule;
            }
            else if (converter == Volume)
            {
                result = VolumeUnit.CubicMeter;
            }
            else if (converter == Speed)
            {
                result = SpeedUnit.MetersPerSecond;
            }
            else if (converter == Density)
            {
                result = DensityUnit.KilogramPerCubicMeter;
            }
            else if (converter == Mass)
            {
                result = MassUnit.Kilogram;
            }
            else if (converter == VolumetricFlow)
            {
                result = VolumetricFlowUnit.CubicMetersPerSecond;
            }
            else if (converter == SpecificEnergy)
            {
                result = SpecificEnergyUnit.JoulePerKilogram;
            }
            else if (converter == Angle)
            {
                result = AngleUnit.Radians;
            }
            else if (converter == ManpowerTime)
            {
                result = ManpowerTimeUnit.Hour;
            }
            else if (converter == MassFlow)
            {
                result = MassFlowUnit.KgPerSecond;
            }
            else
            {
                result = UnitlessUnit.Unitless;
            }

            return result;
        }

//        public static void GetUnitFromString(UnitOfMeasurementConverters converter, string unitName, Enum unit)
//        {
//            if (converter == Distance)
//            {
//                unit = UnitParser.ParseDistanceUnit(unitName);
//            }
//
//        }


        public static UnitOfMeasurementConverters Speed
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Speed.ToString())) InitSpeedConverters();

                return _mUnits.GetUnitConverter(ConverterName.Speed.ToString());
            }
        }

        public static UnitOfMeasurementConverters VolumetricFlow
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.VolumetricFlow.ToString())) InitVolumetricFlowConverters();

                return _mUnits.GetUnitConverter(ConverterName.VolumetricFlow.ToString());
            }
        }


        public static UnitOfMeasurementConverters Area
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Area.ToString())) InitAreaConverters();

                return _mUnits.GetUnitConverter(ConverterName.Area.ToString());
            }
        }


        public static UnitOfMeasurementConverters Energy
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Energy.ToString())) InitEnergyConverters();

                return _mUnits.GetUnitConverter(ConverterName.Energy.ToString());
            }
        }

        public static UnitOfMeasurementConverters SpecificEnergy
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.SpecificEnergy.ToString())) InitSpecificEnergyConverters();

                return _mUnits.GetUnitConverter(ConverterName.SpecificEnergy.ToString());
            }
        }

        public static UnitOfMeasurementConverters Mass
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Mass.ToString())) InitMassConverters();

                return _mUnits.GetUnitConverter(ConverterName.Mass.ToString());
            }
        }


        public static UnitOfMeasurementConverters MassFlow
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.MassFlow.ToString())) InitMassFlowConverters();

                return _mUnits.GetUnitConverter(ConverterName.MassFlow.ToString());
            }
        }


        public static UnitOfMeasurementConverters Angle
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Angle.ToString())) InitAngleConverters();

                return _mUnits.GetUnitConverter(ConverterName.Angle.ToString());
            }
        }


        public static UnitOfMeasurementConverters Distance
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Distance.ToString())) InitDistanceConverters();

                return _mUnits.GetUnitConverter(ConverterName.Distance.ToString());
            }

        }

        public static UnitOfMeasurementConverters ElapsingTime
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.ElapsingTime.ToString())) InitElapsingTimeConverters();

                return _mUnits.GetUnitConverter(ConverterName.ElapsingTime.ToString());
            }
        }

        public static UnitOfMeasurementConverters Frequency
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Frequency.ToString())) InitFrequencyConverters();

                return _mUnits.GetUnitConverter(ConverterName.Frequency.ToString());
            }
        }

        public static UnitOfMeasurementConverters Unitless
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Unitless.ToString())) InitUnitlessConverters();

                return _mUnits.GetUnitConverter(ConverterName.Unitless.ToString());
            }
        }


        public static UnitOfMeasurementConverters Volume
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Volume.ToString())) InitVolumeConverters();

                return _mUnits.GetUnitConverter(ConverterName.Volume.ToString());
            }
        }

        public static UnitOfMeasurementConverters Density
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Density.ToString())) InitDensityConverters();

                return _mUnits.GetUnitConverter(ConverterName.Density.ToString());
            }
        }

        public static UnitOfMeasurementConverters Pressure
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Pressure.ToString())) InitPressureConverters();

                return _mUnits.GetUnitConverter(ConverterName.Pressure.ToString());
            }
        }

        public static UnitOfMeasurementConverters ManpowerTime
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Time.ToString())) InitManhourConverters();

                return _mUnits.GetUnitConverter(ConverterName.Time.ToString());
            }
        }

        public static UnitOfMeasurementConverters JulianTime
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.JulianTime.ToString())) InitJulianTimeConverters();

                return _mUnits.GetUnitConverter(ConverterName.JulianTime.ToString());
            }
        }

        public static UnitOfMeasurementConverters Temperature
        {
            get
            {
                if (!_mUnits.ContainsKey(ConverterName.Temperature.ToString())) InitTemperatureConverters();

                return _mUnits.GetUnitConverter(ConverterName.Temperature.ToString());
            }
        }

        public static UnitOfMeasurementConverters GetConverterByName(string name)
        {
            UnitOfMeasurementConverters result = null;

            switch (name)
            {
                case "Distance":
                    result = Distance;
                    break;
                case "Pressure":
                    result = Pressure;
                    break;
                case "Temperature":
                    result = Temperature;
                    break;
                case "Unitless":
                    result = Unitless;
                    break;
                case "Time":
                    result = ManpowerTime;
                    break;
                case "JulianTime":
                    result = JulianTime;
                    break;
                case "ElapsingTime":
                    result = ElapsingTime;
                    break;
                case "Frequency":
                    result = Frequency;
                    break;
                case "Energy":
                    result = Energy;
                    break;
                case "Volume":
                    result = Volume;
                    break;
                case "Density":
                    result = Density;
                    break;
                case "Angle":
                    result = Angle;
                    break;
                case "Mass":
                    result = Mass;
                    break;
                case "SpecificEnergy":
                    result = SpecificEnergy;
                    break;
                case "Area":
                    result = Area;
                    break;
                case "VolumetricFlow":
                    result = VolumetricFlow;
                    break;
                case "MassFlow":
                    result = MassFlow;
                    break;
                default:
                    throw new Exception("Converter name " + name + " unrecognized.");
            }

            return result;
        }

        public static Enum GetUnitFromString(string name, UnitOfMeasurementConverters converter)
        {
            if (converter == Distance && Enum.TryParse(name, out DistanceUnit ud)) return ud;
            if (converter == Pressure && Enum.TryParse(name, out PressureUnit up)) return up;
            if (converter == Temperature && Enum.TryParse(name, out TempUnit ut)) return ut;
            if (converter == ManpowerTime && Enum.TryParse(name, out ManpowerTimeUnit umt)) return umt;
            if (converter == JulianTime && Enum.TryParse(name, out JulianTimeConversionUnit ujt)) return ujt;
            if (converter == ElapsingTime && Enum.TryParse(name, out TimeUnit uet)) return uet;
            if (converter == Frequency && Enum.TryParse(name, out FreqUnit uft)) return uft;
            if (converter == Energy && Enum.TryParse(name, out EnergyUnit ue)) return ue;
            if (converter == Volume && Enum.TryParse(name, out VolumeUnit uv)) return uv;
            if (converter == Density && Enum.TryParse(name, out DensityUnit uden)) return uden;
            if (converter == Angle && Enum.TryParse(name, out AngleUnit uan)) return uan;
            if (converter == Mass && Enum.TryParse(name, out MassUnit uma)) return uma;
            if (converter == SpecificEnergy && Enum.TryParse(name, out SpecificEnergyUnit use)) return use;
            if (converter == Area && Enum.TryParse(name, out AreaUnit uar)) return uar;
            if (converter == MassFlow && Enum.TryParse(name, out MassFlowUnit mfu)) return mfu;
            if (converter == VolumetricFlow && Enum.TryParse(name, out VolumetricFlowUnit uvf)) return uvf;
            if (converter == Unitless) return UnitlessUnit.Unitless;

            throw new Exception("Unit name " + name + " unrecognized.");
        }

        private static void InitVolumeConverters()
        {
            _mUnits.Populate(ConverterName.Volume.ToString(),
                new[]
                {
                    VolumeUnit.Liter.ToString(), VolumeUnit.CubicCentimeter.ToString(), VolumeUnit.CubicDecimeter.ToString(), VolumeUnit.CubicDekameter.ToString(),
                    VolumeUnit.CubicFoot.ToString(), VolumeUnit.CubicInch.ToString(), VolumeUnit.CubicKilometer.ToString(), VolumeUnit.CubicMeter.ToString(),
                    VolumeUnit.CubicMicrometer.ToString(), VolumeUnit.CubicMile.ToString(), VolumeUnit.CubicMillimeter.ToString(),
                    VolumeUnit.CubicYard.ToString(), VolumeUnit.Deciliter.ToString(), VolumeUnit.Dekaliter.ToString(),
                    VolumeUnit.Kiloliter.ToString(), VolumeUnit.Megaliter.ToString(),
                    VolumeUnit.Microliter.ToString(), VolumeUnit.Milliliter.ToString()
                },
                new[]
                {
                    VolLiterConversionFactor, VolCubicCentimeterConversionFactor, VolCubicDecimeterConversionFactor, VolCubicDekameterConversionFactor,
                    VolCubicFootConversionFactor, VolCubicInchConversionFactor, VolCubicKilometerConversionFactor, VolCubicMeterConversionFactor,
                    VolCubicMicrometerConversionFactor, VolCubicMileConversionFactor, VolCubicMillimeterConversionFactor,
                    VolCubicYardConversionFactor, VolDeciliterConversionFactor, VolDekaliterConversionFactor,
                    VolKiloliterConversionFactor, VolMegaliterConversionFactor,
                    VolMicroliterConversionFactor, VolMilliliterConversionFactor
                }
            );
        }

        private static void InitDensityConverters()
        {
            // 	Milligram_CubicMeter, Milligram_Liter}

            _mUnits.Populate(ConverterName.Density.ToString(),
                new[]
                {
                    DensityUnit.KilogramPerCubicMeter.ToString(),
                    DensityUnit.GramPerCubicMeter.ToString(),
                    DensityUnit.GramPerCubicCentimeter.ToString(),
                    DensityUnit.MilligramPerLiter.ToString(),
                    DensityUnit.OuncePerCubicFoot.ToString(),
                    DensityUnit.OuncePerGallonUK.ToString(),
                    DensityUnit.OuncePerGallonUS.ToString()
                },
                new[] {1D, .001, 1000, .001, 1.001153956601D, 6.236023291419D, 7.489151675466}
            );
        }

        private static void InitPressureConverters()
        {
            _mUnits.Populate(ConverterName.Pressure.ToString(),
                new[]
                {
                    PressureUnit.MPa.ToString(), PressureUnit.kPa.ToString(), PressureUnit.Pa.ToString(),
                    PressureUnit.Psi.ToString(),
                    PressureUnit.Atm.ToString(), PressureUnit.Bar.ToString(), PressureUnit.JoulePerCubicMeter.ToString()
                },
                new[]
                {
                    MpaConversionFactor, _kpaConversionFactor, _paConversionFactor,
                    PsiConversionFactor, AtmConversionFactor, BarConversionFactor, _jpCmConversionFactor
                }
            );
        }

        private static void InitMassConverters()
        {
            _mUnits.Populate(ConverterName.Mass.ToString(),
                new[]
                {
                    MassUnit.Gram.ToString(), MassUnit.Milligram.ToString(), MassUnit.Centigram.ToString(),
                    MassUnit.Decigram.ToString(), MassUnit.Dekagram.ToString(), MassUnit.Hectogram.ToString(),
                    MassUnit.Kilogram.ToString(), MassUnit.Megagram.ToString(), MassUnit.Pound.ToString()
                },
                new[]
                {
                    1D, 1 / 1000D, 1 / 100D, 1 / 10D, 1 / 0.1D, 1 / 0.01D, 1 / 0.001D, 1 / 0.000001D, 1 / 0.00220462D
                });
        }


        private static void InitMassFlowConverters()
        {
            _mUnits.Populate(ConverterName.MassFlow.ToString(),
                new[]
                {
                    MassFlowUnit.KgPerSecond.ToString(), MassFlowUnit.KgPerMin.ToString()
                },
                new[]
                {
                    1D, 1/ 60D
                });
        }


        private static void InitEnergyConverters()
        {
            _mUnits.Populate(ConverterName.Energy.ToString(),
                new[] {EnergyUnit.Joule.ToString(), EnergyUnit.Kwh.ToString(), EnergyUnit.Botu.ToString()},
                new[] {JouleConversionUnit, KwhConversionUnit, _botuConversionUnit}
            );
        }

        private static void InitJulianTimeConverters()
        {
            _mUnits.Populate(ConverterName.JulianTime.ToString(),
                new[] {JulianTimeConversionUnit.Year.ToString(), JulianTimeConversionUnit.Day.ToString()},
                new[] {1D, 1 / 365.25D}
            );
        }

        private static void InitElapsingTimeConverters()
        {
            var firstValue = 1D;
            var secondValue = 1D / 60D;
            var thirdValue = 1D / 60D / 60D;
            var fourthValue = 1D / 60D / 60D / 1000D;
            double[] conversionFactors = {firstValue, secondValue, thirdValue, fourthValue};

            _mUnits.Populate(ConverterName.ElapsingTime.ToString(),
                new[]
                {
                    TimeUnit.Hour.ToString(), TimeUnit.Minute.ToString(),
                    TimeUnit.Second.ToString(), TimeUnit.Millisecond.ToString()
                },
                conversionFactors);
        }

        private static void InitFrequencyConverters()
        {
            var firstValue = 1D;
            double[] conversionFactors = {firstValue};

            _mUnits.Populate(ConverterName.Frequency.ToString(),
                new[]
                {
                    FreqUnit.PerYear.ToString(),
                },
                conversionFactors);
        }

        private static void InitAreaConverters()
        {
            _mUnits.Populate(ConverterName.Area.ToString(),
                new[]
                {
                    AreaUnit.SqMeters.ToString(), AreaUnit.SqCm.ToString(), AreaUnit.SqFoot.ToString(),
                    AreaUnit.SqInch.ToString(), AreaUnit.SqMm.ToString(), AreaUnit.SqYard.ToString()
                },
                new[] {1D, 1D / 10000D, 1D / 10.76391042D, 1D / 1550.00310001D, 1D / 1000000D, 1D / 1.19599005D}
            );
        }

        private static void InitDistanceConverters()
        {
            _mUnits.Populate(ConverterName.Distance.ToString(),
                new[]
                {
                    DistanceUnit.Meter.ToString(), DistanceUnit.Centimeter.ToString(),
                    DistanceUnit.Millimeter.ToString(), DistanceUnit.Inch.ToString(), DistanceUnit.Foot.ToString(),
                    DistanceUnit.Yard.ToString(), DistanceUnit.Mile.ToString(), DistanceUnit.Au.ToString()
                },
                new[]
                {
                    MeterConversionFactor, CentimeterConversionFactor, MillimeterConversionFactor,
                    _inchConversionFactor,
                    FootConversionFactor, _yardConversionFactor, MileConversionFactor, AstronomicalUnitConversionFactor
                }
            );
        }

        private static void InitSpeedConverters()
        {
            const double metersPerSecondConversionFactor = 1D;
            _mUnits.Populate(ConverterName.Speed.ToString(),
                // Currently only one speed unit, but will add more as requested.
                new[] {SpeedUnit.MetersPerSecond.ToString()}, new[] {metersPerSecondConversionFactor}
            );
        }

        private static void InitTemperatureConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(ConverterName.Temperature.ToString());

            var conversionProvider = new DualConversionProvider {ConversionDelegate = new FhtToCelcConversion()};
            converters.Add("Fahrenheit", conversionProvider);

            conversionProvider = new DualConversionProvider {ConversionDelegate = new AdditionBasedConversion()};
            ((AdditionBasedConversion) conversionProvider.ConversionDelegate).AdditionMember = 273.15;
            converters.Add("Kelvin", conversionProvider);

            conversionProvider = new DualConversionProvider
            {
                ConversionDelegate = null, ConversionObject = new ConversionData {ConversionFactor = 1}
            };
            converters.Add("Celsius", conversionProvider);
        }

        private static void InitVolumetricFlowConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(ConverterName.VolumetricFlow.ToString());
            var conversionProvider = new DualConversionProvider
            {
                ConversionObject = new ConversionData {ConversionFactor = 1D}
            };
            converters.Add(VolumetricFlowUnit.CubicMetersPerSecond.ToString(), conversionProvider);
        }

        private static void InitSpecificEnergyConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(ConverterName.SpecificEnergy.ToString());
            var conversionProvider = new DualConversionProvider
            {
                ConversionObject = new ConversionData {ConversionFactor = 1D}
            };
            // Base unit
            converters.Add(SpecificEnergyUnit.JoulePerKilogram.ToString(), conversionProvider);

            conversionProvider = new DualConversionProvider
            {
                ConversionObject = new ConversionData {ConversionFactor = 1000D}
            };
            converters.Add(SpecificEnergyUnit.JoulePerGram.ToString(), conversionProvider);

            conversionProvider = new DualConversionProvider
            {
                ConversionObject = new ConversionData {ConversionFactor = 1000000D}
            };
            converters.Add(SpecificEnergyUnit.KiloJoulePerKilogram.ToString(), conversionProvider);
        }

        private static void InitAngleConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(ConverterName.Angle.ToString());

            var conversionProvider = new DualConversionProvider();
            converters.Add("Radians", conversionProvider);
            conversionProvider.ConversionDelegate = new DummyRadiansConversionDelegate();

            conversionProvider = new DualConversionProvider();
            converters.Add("Degrees", conversionProvider);
            conversionProvider.ConversionDelegate = new DegreesRadiansConversionDelegate();
        }

        private static void InitManhourConverters()
        {
            _mUnits.Populate(ConverterName.Time.ToString(),
                new[] {ManpowerTimeUnit.Hour.ToString()},
                new[] {1D}
            );
        }

        private static void InitUnitlessConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(ConverterName.Unitless.ToString());
            var conversionProvider = new DualConversionProvider
            {
                ConversionObject = new ConversionData {ConversionFactor = 1D}
            };
            converters.Add("Unitless", conversionProvider);
        }


        [Serializable]
        private class AdditionBasedConversion : IISpecialConversionDelegate
        {
            #region iSpecialConversionDelegate Members

            private double _mAdditionMember = double.NaN;

            public double AdditionMember
            {
                get => _mAdditionMember;
                set => _mAdditionMember = value;
            }

            double[] IISpecialConversionDelegate.ConvertFrom(double[] value)
            {
                var result = new double[value.Length];
                for (var index = 0; index < value.Length; index++) result[index] = value[index] + _mAdditionMember;

                return result;
            }

            double[] IISpecialConversionDelegate.ConvertTo(double[] value)
            {
                var result = new double[value.Length];
                for (var index = 0; index < value.Length; index++) result[index] = value[index] - _mAdditionMember;

                return result;
            }

            string IISpecialConversionDelegate.GetName()
            {
                throw new NotImplementedException();
            }

            #endregion
        }

        [Serializable]
        // Delegate to convert Radians and Degrees
        private class DummyRadiansConversionDelegate : IISpecialConversionDelegate
        {
            // Radians is the default

            #region iSpecialConversionDelegate Members

            double[] IISpecialConversionDelegate.ConvertFrom(double[] value)
            {
                return value;
            }

            double[] IISpecialConversionDelegate.ConvertTo(double[] value)
            {
                return value;
            }

            string IISpecialConversionDelegate.GetName()
            {
                return AngleUnit.Radians.ToString();
            }

            #endregion
        }

        [Serializable]
        private class DegreesRadiansConversionDelegate : IISpecialConversionDelegate
        {
            // Radians is the default
            private static double ConvertDegreesToRadians(double angle)
            {
                var result = Math.PI * angle / 180.0;
                return result;
            }

            private static double ConvertRadiansToDegrees(double angle)
            {
                return angle * (180.0 / Math.PI);
            }

            #region iSpecialConversionDelegate Members

            double[] IISpecialConversionDelegate.ConvertFrom(double[] value)
            {
                // Convert from Radians to Degrees
                var result = new double[value.Length];
                for (var index = 0; index < value.Length; index++)
                    result[index] = ConvertRadiansToDegrees(value[index]);

                return result;
            }

            double[] IISpecialConversionDelegate.ConvertTo(double[] value)
            {
                // Convert from Degrees to Radians
                var result = new double[value.Length];
                for (var index = 0; index < value.Length; index++)
                    result[index] = ConvertDegreesToRadians(value[index]);

                return result;
            }

            string IISpecialConversionDelegate.GetName()
            {
                return AngleUnit.Degrees.ToString();
            }

            #endregion
        }

        [Serializable]
        // Delegate to convert from Fahrenheit to Celsius
        private class FhtToCelcConversion : IISpecialConversionDelegate
        {
            #region iSpecialConversionDelegate Members

            // Convert FROM Celsius TO Fahrenheit
            double[] IISpecialConversionDelegate.ConvertFrom(double[] value)
            {
                var result = new double[value.Length];
                for (var index = 0; index < value.Length; index++) result[index] = value[index] * 9 / 5 + 32;

                return result;
            }

            // Convert TO Celsius FROM Fahrenheit
            double[] IISpecialConversionDelegate.ConvertTo(double[] value)
            {
                var result = new double[value.Length];
                for (var index = 0; index < value.Length; index++) result[index] = (value[index] - 32) * 5 / 9;

                return result;
            }

            string IISpecialConversionDelegate.GetName()
            {
                return TempUnit.Fahrenheit.ToString();
            }

            #endregion
        }
    }
}