/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
*/

using System;

namespace SandiaNationalLaboratories.Hyram
{
    [Serializable]
    public enum StockConverterName
    {
        Distance,
        Area,
        Pressure,
        Temperature,
        Unitless,
        Time,
        JulianTime,
        ElapsingTime,
        Energy,
        Volume,
        Direction,
        Speed,
        Density,
        Mass,
        VolumetricFlow,
        SpecificEnergy
    }

    [Serializable]
    public class StockConverters
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


        // TODO:  Add test class to take a known value, store, compare and unstore.
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

        public static UnitOfMeasurementConverters SpeedConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Speed.ToString())) InitSpeedConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Speed.ToString());
            }
        }

        public static UnitOfMeasurementConverters VolumetricFlowConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.VolumetricFlow.ToString())) InitVolumetricFlowConverters();

                return _mUnits.GetUnitConverter(StockConverterName.VolumetricFlow.ToString());
            }
        }


        public static UnitOfMeasurementConverters AreaConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Area.ToString())) InitAreaConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Area.ToString());
            }
        }


        public static UnitOfMeasurementConverters EnergyConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Energy.ToString())) InitEnergyConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Energy.ToString());
            }
        }

        public static UnitOfMeasurementConverters SpecificEnergyConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.SpecificEnergy.ToString())) InitSpecificEnergyConverters();

                return _mUnits.GetUnitConverter(StockConverterName.SpecificEnergy.ToString());
            }
        }

        public static UnitOfMeasurementConverters MassConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Mass.ToString())) InitMassConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Mass.ToString());
            }
        }


        public static UnitOfMeasurementConverters AngleConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Direction.ToString())) InitAngleConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Direction.ToString());
            }
        }


        public static UnitOfMeasurementConverters DistanceConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Distance.ToString())) InitDistanceConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Distance.ToString());
            }
        }

        public static UnitOfMeasurementConverters ElapsingTimeConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.ElapsingTime.ToString())) InitElapsingTimeConverters();

                return _mUnits.GetUnitConverter(StockConverterName.ElapsingTime.ToString());
            }
        }

        public static UnitOfMeasurementConverters UnitlessConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Unitless.ToString())) InitUnitlessConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Unitless.ToString());
            }
        }


        public static UnitOfMeasurementConverters VolumeConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Volume.ToString())) InitVolumeConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Volume.ToString());
            }
        }

        public static UnitOfMeasurementConverters DensityConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Density.ToString())) InitDensityConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Density.ToString());
            }
        }

        public static UnitOfMeasurementConverters PressureConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Pressure.ToString())) InitPressureConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Pressure.ToString());
            }
        }

        public static UnitOfMeasurementConverters ManpowerTimeConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Time.ToString())) InitManhourConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Time.ToString());
            }
        }

        public static UnitOfMeasurementConverters JulianTimeConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.JulianTime.ToString())) InitJulianTimeConverters();

                return _mUnits.GetUnitConverter(StockConverterName.JulianTime.ToString());
            }
        }

        public static UnitOfMeasurementConverters TemperatureConverter
        {
            get
            {
                if (!_mUnits.ContainsKey(StockConverterName.Temperature.ToString())) InitTemperatureConverters();

                return _mUnits.GetUnitConverter(StockConverterName.Temperature.ToString());
            }
        }

        public static UnitOfMeasurementConverters GetConverterByName(string name)
        {
            UnitOfMeasurementConverters result = null;

            switch (name)
            {
                case "Distance":
                case "DistanceUnit":
                    result = DistanceConverter;
                    break;
                case "Pressure":
                case "PressureUnit":
                    result = PressureConverter;
                    break;
                case "Temperature":
                case "TempUnit":
                    result = TemperatureConverter;
                    break;
                case "Unitless":
                case "UnitlessUnit":
                    result = UnitlessConverter;
                    break;
                case "Time":
                case "TimeUnit":
                    result = ManpowerTimeConverter;
                    break;
                case "JulianTime":
                case "JulianTimeUnit":
                    result = JulianTimeConverter;
                    break;
                case "ElapsingTime":
                case "ElapsingTimeUnit":
                    result = ElapsingTimeConverter;
                    break;
                case "Energy":
                case "EnergyUnit":
                    result = EnergyConverter;
                    break;
                case "Volume":
                case "VolumeUnit":
                    result = VolumeConverter;
                    break;
                case "Density":
                case "DensityUnit":
                    result = DensityConverter;
                    break;
                case "AngleUnit":
                    result = AngleConverter;
                    break;
                case "Mass":
                    result = MassConverter;
                    break;
                case "SpecificEnergy":
                    result = SpecificEnergyConverter;
                    break;
                case "AreaUnit":
                    result = AreaConverter;
                    break;
                case "VolumetricFlow":
                    result = VolumetricFlowConverter;
                    break;
                default:
                    throw new Exception("Converter name " + name + " unrecognized.");
            }

            return result;
        }

        private static void InitVolumeConverters()
        {
            _mUnits.Populate(StockConverterName.Volume.ToString(),
                new[]
                {
                    VolumeUnit.Liter.ToString(), VolumeUnit.CubicCentimeter.ToString(),
                    VolumeUnit.CubicDecimeter.ToString(), VolumeUnit.CubicDekameter.ToString(),
                    VolumeUnit.CubicFoot.ToString(), VolumeUnit.CubicInch.ToString(),
                    VolumeUnit.CubicKilometer.ToString(), VolumeUnit.CubicMeter.ToString(),
                    VolumeUnit.CubicMicrometer.ToString(),
                    VolumeUnit.CubicMile.ToString(), VolumeUnit.CubicMillimeter.ToString(),
                    VolumeUnit.CubicYard.ToString(), VolumeUnit.Deciliter.ToString(), VolumeUnit.Dekaliter.ToString(),
                    VolumeUnit.Kiloliter.ToString(), VolumeUnit.Megaliter.ToString(), VolumeUnit.Microliter.ToString(),
                    VolumeUnit.Milliliter.ToString()
                },
                new[]
                {
                    VolLiterConversionFactor, VolCubicCentimeterConversionFactor, VolCubicDecimeterConversionFactor,
                    VolCubicDekameterConversionFactor,
                    VolCubicFootConversionFactor, VolCubicInchConversionFactor, VolCubicKilometerConversionFactor,
                    VolCubicMeterConversionFactor,
                    VolCubicMicrometerConversionFactor, VolCubicMileConversionFactor,
                    VolCubicMillimeterConversionFactor, VolCubicYardConversionFactor,
                    VolDeciliterConversionFactor, VolDekaliterConversionFactor, VolKiloliterConversionFactor,
                    VolMegaliterConversionFactor,
                    VolMicroliterConversionFactor, VolMilliliterConversionFactor
                }
            );
        }

        private static void InitDensityConverters()
        {
            // 	Milligram_CubicMeter, Milligram_Liter}

            _mUnits.Populate(StockConverterName.Density.ToString(),
                new[]
                {
                    DensityUnit.KilogramPerCubicMeter.ToString(), DensityUnit.GramPerCubicMeter.ToString(),
                    DensityUnit.GramPerCubicCentimeter.ToString(),
                    DensityUnit.MilligramPerLiter.ToString(), DensityUnit.OuncePerCubicFoot.ToString(),
                    DensityUnit.OuncePerGallonUK.ToString(), DensityUnit.OuncePerGallonUS.ToString()
                },
                new[] {1D, .001, 1000, .001, 1.001153956601D, 6.236023291419D, 7.489151675466}
            );
        }

        private static void InitPressureConverters()
        {
            _mUnits.Populate(StockConverterName.Pressure.ToString(),
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
            _mUnits.Populate(StockConverterName.Mass.ToString(),
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


        private static void InitEnergyConverters()
        {
            _mUnits.Populate(StockConverterName.Energy.ToString(),
                new[] {EnergyUnit.Joule.ToString(), EnergyUnit.Kwh.ToString(), EnergyUnit.Botu.ToString()},
                new[] {JouleConversionUnit, KwhConversionUnit, _botuConversionUnit}
            );
        }

        private static void InitJulianTimeConverters()
        {
            _mUnits.Populate(StockConverterName.JulianTime.ToString(),
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

            _mUnits.Populate(StockConverterName.ElapsingTime.ToString(),
                new[]
                {
                    ElapsingTimeConversionUnit.Hour.ToString(), ElapsingTimeConversionUnit.Minute.ToString(),
                    ElapsingTimeConversionUnit.Second.ToString(), ElapsingTimeConversionUnit.Millisecond.ToString()
                },
                conversionFactors);
        }

        private static void InitAreaConverters()
        {
            _mUnits.Populate(StockConverterName.Area.ToString(),
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
            _mUnits.Populate(StockConverterName.Distance.ToString(),
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
            _mUnits.Populate(StockConverterName.Speed.ToString(),
                // Currently only one speed unit, but will add more as requested.
                new[] {SpeedUnit.MetersPerSecond.ToString()}, new[] {metersPerSecondConversionFactor}
            );
        }

        private static void InitTemperatureConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(StockConverterName.Temperature.ToString());

            var conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionDelegate = new FhtToCelcConversion();
            converters.Add("Fahrenheit", conversionProvider);

            conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionDelegate = new AdditionBasedConversion();
            ((AdditionBasedConversion) conversionProvider.ConversionDelegate).AdditionMember = 273.15;
            converters.Add("Kelvin", conversionProvider);

            conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionDelegate = null;
            conversionProvider.ConversionObject = new ConversionData();
            conversionProvider.ConversionObject.ConversionFactor = 1;
            converters.Add("Celsius", conversionProvider);
        }

        private static void InitVolumetricFlowConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(StockConverterName.VolumetricFlow.ToString());
            var conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionObject = new ConversionData();
            conversionProvider.ConversionObject.ConversionFactor = 1D;
            converters.Add(VolumetricFlowUnit.CubicMetersPerSecond.ToString(), conversionProvider);
        }

        private static void InitSpecificEnergyConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(StockConverterName.SpecificEnergy.ToString());
            var conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionObject = new ConversionData();
            conversionProvider.ConversionObject.ConversionFactor = 1D; // Base unit
            converters.Add(SpecificEnergyUnit.JoulePerKilogram.ToString(), conversionProvider);

            conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionObject = new ConversionData();
            conversionProvider.ConversionObject.ConversionFactor = 1000D;
            converters.Add(SpecificEnergyUnit.JoulePerGram.ToString(), conversionProvider);

            conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionObject = new ConversionData();
            conversionProvider.ConversionObject.ConversionFactor = 1000000D;
            converters.Add(SpecificEnergyUnit.KiloJoulePerKilogram.ToString(), conversionProvider);
        }

        private static void InitAngleConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(StockConverterName.Direction.ToString());

            var conversionProvider = new DualConversionProvider();
            converters.Add("Radians", conversionProvider);
            conversionProvider.ConversionDelegate = new DummyRadiansConversionDelegate();

            conversionProvider = new DualConversionProvider();
            converters.Add("Degrees", conversionProvider);
            conversionProvider.ConversionDelegate = new DegreesRadiansConversionDelegate();
        }

        private static void InitManhourConverters()
        {
            _mUnits.Populate(StockConverterName.Time.ToString(),
                new[] {ManpowerTimeUnit.Hour.ToString()},
                new[] {1D}
            );
        }

        private static void InitUnitlessConverters()
        {
            var converters =
                _mUnits.CreateOrGetUnitConverter(StockConverterName.Unitless.ToString());
            var conversionProvider = new DualConversionProvider();
            conversionProvider.ConversionObject = new ConversionData();
            conversionProvider.ConversionObject.ConversionFactor = 1D;
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