/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace SandiaNationalLaboratories.Hyram
{
    public enum UnitlessUnit { Unitless }

    public enum SpeedUnit { MetersPerSecond }

    public enum VolumetricFlowUnit { CubicMetersPerSecond }

    public enum DistanceUnit { Meter, Centimeter, Millimeter, Inch, Foot, Yard, Mile, Au }

    public enum TempUnit { Celsius, Fahrenheit, Kelvin }

    public enum PressureUnit { MPa, kPa, Pa, Psi, Atm, Bar, JoulePerCubicMeter }

    public enum AreaUnit { SqMeters, SqCm, SqMm, SqInch, SqFoot, SqYard }

    public enum DensityUnit { KilogramPerCubicMeter, GramPerCubicMeter, GramPerCubicCentimeter, MilligramPerLiter, OuncePerCubicFoot, OuncePerGallonUK, OuncePerGallonUS }

    public enum EnergyUnit { Joule, Kwh, Botu }

    public enum AngleUnit { Radians, Degrees }

    public enum MassUnit { Gram, Milligram, Centigram, Decigram, Dekagram, Hectogram, Kilogram, Megagram, Pound }
    
    public enum VolumeUnit
    {
        Liter,
        CubicCentimeter, CubicDecimeter, CubicDekameter, CubicFoot, CubicInch, CubicKilometer,
        CubicMeter, CubicMile, CubicMicrometer, CubicMillimeter, CubicYard, Deciliter,
        Dekaliter, Kiloliter, Megaliter, Microliter, Milliliter
    }

    public enum SpecificEnergyUnit { JoulePerKilogram, JoulePerGram, KiloJoulePerKilogram }

    // Manpower Time Unit isn't a general conversion enumeration.
    public enum ManpowerTimeUnit { Hour }

    public enum JulianTimeConversionUnit { Year, Day } // Year is 365.25

    public enum TimeUnit { Hour, Minute, Second, Millisecond }

    public enum FreqUnit { PerYear }

    public enum MassFlowUnit { KgPerSecond }


    public static class UnitParser
    {
        private static readonly Dictionary<string, SpecificEnergyUnit> MSpecificEnergyUnits = CreateUnitDictionary<SpecificEnergyUnit>();
        private static readonly Dictionary<string, DistanceUnit> MDistanceUnits = CreateUnitDictionary<DistanceUnit>();
        private static readonly Dictionary<string, MassUnit> MMassUnits = CreateUnitDictionary<MassUnit>();
        private static readonly Dictionary<string, MassFlowUnit> MMassFlowUnits = CreateUnitDictionary<MassFlowUnit>();
        private static readonly Dictionary<string, VolumeUnit> MVolumeUnits = CreateUnitDictionary<VolumeUnit>();
        private static readonly Dictionary<string, DensityUnit> MDensityUnits = CreateUnitDictionary<DensityUnit>();

        public static PressureUnit ParsePressureUnit(string unitName)
        {
            PressureUnit result;

            var unitNameUpper = unitName.Trim().ToUpper();
            switch (unitNameUpper)
            {
                case "MPA":
                    result = PressureUnit.MPa;
                    break;
                case "KPA":
                    result = PressureUnit.kPa;
                    break;
                case "PA":
                    result = PressureUnit.Pa;
                    break;
                case "PSIG":
                case "PSI":
                    result = PressureUnit.Psi;
                    break;
                case "ATM":
                    result = PressureUnit.Atm;
                    break;
                case "BAR":
                    result = PressureUnit.Bar;
                    break;
                case "JCM":
                    result = PressureUnit.JoulePerCubicMeter;
                    break;
                default:
                    throw new ArgumentException("Unit of " + unitName + " is not a recognized Pressure unit.");
            }

            return result;
        }

        public static TempUnit ParseTempUnit(string unitName)
        {
            TempUnit result;
            var unitNameUpper = unitName.Trim().ToUpper();
            switch (unitNameUpper)
            {
                case "CELSIUS":
                    result = TempUnit.Celsius;
                    break;
                case "FAHRENHEIT":
                    result = TempUnit.Fahrenheit;
                    break;
                case "KELVIN":
                    result = TempUnit.Kelvin;
                    break;
                default:
                    throw new ArgumentException("Unit of " + unitName + " is not a recognized Temperature unit.");
            }

            return result;
        }

        public static DensityUnit ParseDensityUnit(string unitName)
        {
            if (MDensityUnits.ContainsKey(unitName))
            {
                return MDensityUnits[unitName];
            }
            throw new ArgumentException("Unit of " + unitName + " is not a recognized density unit.");
        }

        public static VolumeUnit ParseVolumeUnit(string unitName)
        {
            if (MVolumeUnits.ContainsKey(unitName))
            {
                return MVolumeUnits[unitName];
            }
            throw new ArgumentException("Unit of " + unitName + " is not a recognized volume unit.");
        }


        public static MassUnit ParseMassUnit(string unitName)
        {
            if (MMassUnits.ContainsKey(unitName))
            {
                return MMassUnits[unitName];
            }
            throw new ArgumentException("Unit of " + unitName + " is not a recognized mass unit.");
        }

        public static MassFlowUnit ParseMassFlowUnit(string unitName)
        {
            if (MMassFlowUnits.ContainsKey(unitName))
            {
                return MMassFlowUnits[unitName];
            }
            throw new ArgumentException("Unit of " + unitName + " is not a recognized mass unit.");
        }


        public static DistanceUnit ParseDistanceUnit(string unitName)
        {
            if (MDistanceUnits.ContainsKey(unitName))
            {
                return MDistanceUnits[unitName];
            }
            throw new ArgumentException("Unit of " + unitName + " is not a recognized distance unit.");
        }

        public static SpecificEnergyUnit ParseSpecificEnergyUnit(string unitName)
        {
            if (MSpecificEnergyUnits.ContainsKey(unitName))
            {
                return MSpecificEnergyUnits[unitName];
            }
            throw new ArgumentException("Unit of " + unitName + " is not a recognized specific energy unit.");
        }



        private static IEnumerable<T> GetEnumPossibleValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        private static Dictionary<string, SpecificEnergyUnit> CreateSpecificEnergyDictionary()
        {
            var result = new Dictionary<string, SpecificEnergyUnit>();
            var possibleValues =
                (SpecificEnergyUnit[]) GetEnumPossibleValues<SpecificEnergyUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, DistanceUnit> CreateDistanceUnitsDictionary()
        {
            var result = new Dictionary<string, DistanceUnit>();
            var possibleValues =
                (DistanceUnit[]) GetEnumPossibleValues<DistanceUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, MassUnit> CreateMassUnitsParsingDictionary()
        {
            var result = new Dictionary<string, MassUnit>();
            var possibleValues = (MassUnit[]) GetEnumPossibleValues<MassUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, VolumeUnit> CreateVolumeUnitsParsingDictionary()
        {
            var result = new Dictionary<string, VolumeUnit>();
            var possibleValues = (VolumeUnit[]) GetEnumPossibleValues<VolumeUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, DensityUnit> CreateDensityUnitsParsingDictionary()
        {
            var result = new Dictionary<string, DensityUnit>();
            var possibleValues = (DensityUnit[]) GetEnumPossibleValues<DensityUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);
            return result;
        }

        private static Dictionary<string, TUnit> CreateUnitDictionary<TUnit>()
        {
            var result = new Dictionary<string, TUnit>();
            var possibleValues = (TUnit[]) GetEnumPossibleValues<TUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);
            return result;
        }

    }

}