using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SandiaNationalLaboratories.Hyram;

namespace Hyram.Tests
{
    [TestClass]
    public class UnitConversionTestCase
    {
        [TestMethod]
        public void TestDistanceConversions()
        {
            var distM = new Parameter(Converters.Distance, DistanceUnit.Meter, 1D);
            var delta = 0.01D;
            Assert.AreEqual(distM.GetValue(DistanceUnit.Meter), 1D, delta);
            Assert.AreEqual(distM.GetValue(DistanceUnit.Centimeter), 100D, delta);
            Assert.AreEqual(distM.GetValue(DistanceUnit.Millimeter), 1000D, delta);
            Assert.AreEqual(distM.GetValue(DistanceUnit.Foot), 3.2808D, 3.2 / 1000D);
            Assert.AreEqual(distM.GetValue(DistanceUnit.Inch), 39.37D, delta);
            Assert.AreEqual(distM.GetValue(DistanceUnit.Mile), 6.214E-4D, 6.2E-4 / 1000);
            Assert.AreEqual(distM.GetValue(DistanceUnit.Yard), 1.09361D, 1 / 100D);
        }

        [TestMethod]
        public void TestPressureConversions()
        {
            var val = new Parameter(Converters.Pressure, PressureUnit.Pa, 101325D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(PressureUnit.Pa), 101325D, delta);
            Assert.AreEqual(val.GetValue(PressureUnit.MPa), 0.101325D, 0.1 / 1000);
            Assert.AreEqual(val.GetValue(PressureUnit.kPa), 101.325D, delta);
            Assert.AreEqual(val.GetValue(PressureUnit.Atm), 1D, delta);
            Assert.AreEqual(val.GetValue(PressureUnit.Psi), 14.69595D, delta);
            Assert.AreEqual(val.GetValue(PressureUnit.Bar), 1.01325D, delta);
            Assert.AreEqual(val.GetValue(PressureUnit.JoulePerCubicMeter), 101325D, delta);
        }

        [TestMethod]
        public void TestTemperatureConversions()
        {
            var val = new Parameter(Converters.Temperature, TempUnit.Kelvin, 300D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(TempUnit.Kelvin), 300D, delta);
            Assert.AreEqual(val.GetValue(TempUnit.Celsius), 26.85D, delta);
            Assert.AreEqual(val.GetValue(TempUnit.Fahrenheit), 80.33D, delta);
        }

        [TestMethod]
        public void TestAreaConversions()
        {
            var val = new Parameter(Converters.Area, AreaUnit.SqMeters, 1D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(AreaUnit.SqMeters), 1, delta);
            Assert.AreEqual(val.GetValue(AreaUnit.SqCm), 10000, delta);
            Assert.AreEqual(val.GetValue(AreaUnit.SqMm), 1000000, delta);
            Assert.AreEqual(val.GetValue(AreaUnit.SqYard), 1.19599, delta);
            Assert.AreEqual(val.GetValue(AreaUnit.SqFoot), 10.7639, delta);
            Assert.AreEqual(val.GetValue(AreaUnit.SqInch), 1549.9944, delta);
        }

        [TestMethod]
        public void TestDensityConversions()
        {
            var val = new Parameter(Converters.Density, DensityUnit.KilogramPerCubicMeter, 1D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(DensityUnit.KilogramPerCubicMeter), 1, delta);
            Assert.AreEqual(val.GetValue(DensityUnit.GramPerCubicMeter), 1000, delta);
            Assert.AreEqual(val.GetValue(DensityUnit.GramPerCubicCentimeter), 0.001, 0.001 / 1000);
            Assert.AreEqual(val.GetValue(DensityUnit.MilligramPerLiter), 1000, delta);
//            Assert.AreEqual(val.GetValue(DensityUnit.GramPerLiter), 1, delta);
            Assert.AreEqual(val.GetValue(DensityUnit.OuncePerCubicFoot), 0.99885D, delta);
            Assert.AreEqual(val.GetValue(DensityUnit.OuncePerGallonUK), 0.160359D, 0.16 / 1000);
            Assert.AreEqual(val.GetValue(DensityUnit.OuncePerGallonUS), 0.13353D, 0.13 / 1000);
        }

        [TestMethod]
        public void TestEnergyConversions()
        {
            var val = new Parameter(Converters.Energy, EnergyUnit.Joule, 100D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(EnergyUnit.Joule), 100D, delta);
            Assert.AreEqual(val.GetValue(EnergyUnit.Botu), 2.7778E-5, 2.8E-5 / 1000);
            Assert.AreEqual(val.GetValue(EnergyUnit.Kwh), 2.7778E-5, 2.8E-5 / 1000);

            val = new Parameter(Converters.Energy, EnergyUnit.Kwh, 100D);
            Assert.AreEqual(val.GetValue(EnergyUnit.Kwh), 100D, delta);
            Assert.AreEqual(val.GetValue(EnergyUnit.Joule), 3.6E8, 100);
            Assert.AreEqual(val.GetValue(EnergyUnit.Botu), 100D, delta);
        }

        [TestMethod]
        public void TestSpecificEnergyConversions()
        {
            var val = new Parameter(Converters.SpecificEnergy, SpecificEnergyUnit.JoulePerGram, 
                100D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(SpecificEnergyUnit.JoulePerGram), 100, delta);
            Assert.AreEqual(val.GetValue(SpecificEnergyUnit.JoulePerKilogram), 1E5, delta);
            Assert.AreEqual(val.GetValue(SpecificEnergyUnit.KiloJoulePerKilogram), 0.1D, delta / 1000);
        }

        [TestMethod]
        public void TestTimeConversions()
        {
            var val = new Parameter(Converters.JulianTime, JulianTimeConversionUnit.Year, 1D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(JulianTimeConversionUnit.Year), 1, delta);
            Assert.AreEqual(val.GetValue(JulianTimeConversionUnit.Day), 365.25, delta);
        }

        [TestMethod]
        public void TestElapsingTimeConversions()
        {
            var val = new Parameter(Converters.ElapsingTime, TimeUnit.Minute, 1D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(TimeUnit.Minute), 1, delta);
            Assert.AreEqual(val.GetValue(TimeUnit.Second), 60D, delta);
            Assert.AreEqual(val.GetValue(TimeUnit.Hour), 0.0166667D, delta / 100000);
            Assert.AreEqual(val.GetValue(TimeUnit.Millisecond), 60000D, delta);
        }

        [TestMethod]
        public void TestAngleConversions()
        {
            var val = new Parameter(Converters.Angle, AngleUnit.Degrees, 180D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(AngleUnit.Degrees), 180D, delta);
            Assert.AreEqual(val.GetValue(AngleUnit.Radians), 3.14159D, delta);
        }

        [TestMethod]
        public void TestVolumeConversions()
        {
            var val = new Parameter(Converters.Volume, VolumeUnit.CubicMeter, 1D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicMeter), 1, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.Liter), 1000, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicCentimeter), 1E6, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicDecimeter), 1E3, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicDekameter), 0.001, 0.01 / 1000);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicFoot), 35.314667D, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicInch), 61023.744D, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicKilometer), 1E-9D, 1E-4D / 1000);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicMile), 2.3991E-10, 2.4E-10 / 1000);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicYard), 1.30795, 1.3 / 1000);
            Assert.AreEqual(val.GetValue(VolumeUnit.Deciliter), 10000, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.Dekaliter), 100, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.Kiloliter), 1, 0.001);
            Assert.AreEqual(val.GetValue(VolumeUnit.Megaliter), 0.001, 0.001 / 1000);
            Assert.AreEqual(val.GetValue(VolumeUnit.Microliter), 1E9, delta);
            Assert.AreEqual(val.GetValue(VolumeUnit.Milliliter), 1E6, delta);

            val = new Parameter(Converters.Volume, VolumeUnit.Microliter, 1D);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicMicrometer), 1E9, 10);
            Assert.AreEqual(val.GetValue(VolumeUnit.CubicMillimeter), 1, 0.01);
        }

        [TestMethod]
        public void TestMassConversions()
        {
            var val = new Parameter(Converters.Mass, MassUnit.Gram, 1000D);
            var delta = 0.01D;
            Assert.AreEqual(val.GetValue(MassUnit.Gram), 1000D, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Milligram), 1E6, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Centigram), 1E5, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Decigram), 1E4, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Dekagram), 100D, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Hectogram), 10D, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Kilogram), 1D, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Megagram), 0.001D, delta);
            Assert.AreEqual(val.GetValue(MassUnit.Pound), 2.20462D, delta);
        }
    }
}
