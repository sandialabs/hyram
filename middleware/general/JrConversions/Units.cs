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
using System.Collections.Generic;

namespace JrConversions
{
    public enum SelectedUnitEnum
    {
        SpeedUnit,
        AreaUnit,
        DistanceUnit,
        TempUnit,
        PressureUnit,
        UnitlessUnit,
        EnergyUnit,
        ManpowerTimeUnit,
        JulianTimeConversionUnit,
        ElapsingTimeConversionUnit,
        AngleUnit,
        VolumeUnit,
        DensityUnit,
        VolumetricFlowUnit,
        NotSet
    }

    public enum SpeedUnit
    {
        MetersPerSecond
    }

    public enum VolumetricFlowUnit
    {
        CubicMetersPerSecond
    }

    public enum AreaUnit
    {
        SqMeters,
        SqCm,
        SqMm,
        SqInch,
        SqFoot,
        SqYard
    }

    public enum DistanceUnit
    {
        Meter,
        Centimeter,
        Millimeter,
        Inch,
        Foot,
        Yard,
        Mile,
        Au
    }

    public enum TempUnit
    {
        Celsius,
        Fahrenheit,
        Kelvin
    }

    public enum PressureUnit
    {
        PsIg,
        MPa,
        KPa,
        Pa,
        Psi,
        Atm,
        Bar,
        Jcm
    }

    public enum DensityUnit
    {
        KilogramCubicMeter,
        GramCubicMeter,
        GramCubicCentimeter,
        MilligramLiter,
        GramLiter,
        OunceCubicFoot,
        OunceGallonUk,
        OunceGallonUs
    }

    public enum UnitlessUnit
    {
        Unitless
    }

    public enum EnergyUnit
    {
        Joule,
        Kwh,
        Botu
    }

    public enum SpecificEnergyUnit
    {
        JouleKg,
        JouleG,
        KjKg
    }

    // Manpower Time Unit isn't a general conversion enumeration.
    public enum ManpowerTimeUnit
    {
        Hour
    }

    public enum JulianTimeConversionUnit
    {
        Year,
        Day
    } // Year is 365.25

    public enum ElapsingTimeConversionUnit
    {
        Hour,
        Minute,
        Second,
        Millisecond
    }

    public enum AngleUnit
    {
        Radians,
        Degrees
    }

    public enum VolumeUnit
    {
        Liter,
        CubicCentimeter,
        CubicDecimeter,
        CubicDekameter,
        CubicFoot,
        CubicInch,
        CubicKilometer,
        CubicMeter,
        CubicMile,
        CubicMicrometer,
        CubicMillimeter,
        CubicYard,
        Deciliter,
        Dekaliter,
        Kiloliter,
        Megaliter,
        Microliter,
        Milliliter
    }

    public enum MassUnit
    {
        Gram,
        Milligram,
        Centigram,
        Decigram,
        Dekagram,
        Hectogram,
        Kilogram,
        Megagram,
        Pound
    }


    public static class UnitParser
    {
        private static readonly Dictionary<string, SpecificEnergyUnit> _mSpecificEnergyUnits =
            CreateSpecificEnergyUnitsParsingDictionary();

        private static readonly Dictionary<string, DistanceUnit> _mDistanceUnits =
            CreateDistanceUnitsParsingDictionary();

        private static readonly Dictionary<string, MassUnit> _mMassUnits = CreateMassUnitsParsingDictionary();

        private static readonly Dictionary<string, VolumeUnit> _mVolumeUnits = CreateVolumeUnitsParsingDictionary();

        private static readonly Dictionary<string, DensityUnit> _mDensityUnits = CreateDensityUnitsParsingDictionary();

        public static PressureUnit ParsePressureUnit(string unitName)
        {
            PressureUnit result;

            var unitNameUpper = unitName.Trim().ToUpper();
            switch (unitNameUpper)
            {
                case "PSIG":
                    result = PressureUnit.PsIg;
                    break;
                case "MPA":
                    result = PressureUnit.MPa;
                    break;
                case "KPA":
                    result = PressureUnit.KPa;
                    break;
                case "PA":
                    result = PressureUnit.Pa;
                    break;
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
                    result = PressureUnit.Jcm;
                    break;
                default:
                    throw new Exception("Unit of " + unitName + " is not a recognized Pressure unit.");
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
                    throw new Exception("Unit of " + unitName + " is not a recognized Temperature unit.");
            }

            return result;
        }

        private static Dictionary<string, SpecificEnergyUnit> CreateSpecificEnergyUnitsParsingDictionary()
        {
            var result = new Dictionary<string, SpecificEnergyUnit>();
            var possibleValues =
                (SpecificEnergyUnit[]) EnumUtil.GetEnumPossibleValues<SpecificEnergyUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, DistanceUnit> CreateDistanceUnitsParsingDictionary()
        {
            var result = new Dictionary<string, DistanceUnit>();
            var possibleValues =
                (DistanceUnit[]) EnumUtil.GetEnumPossibleValues<DistanceUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, MassUnit> CreateMassUnitsParsingDictionary()
        {
            var result = new Dictionary<string, MassUnit>();
            var possibleValues = (MassUnit[]) EnumUtil.GetEnumPossibleValues<MassUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, VolumeUnit> CreateVolumeUnitsParsingDictionary()
        {
            var result = new Dictionary<string, VolumeUnit>();
            var possibleValues = (VolumeUnit[]) EnumUtil.GetEnumPossibleValues<VolumeUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        private static Dictionary<string, DensityUnit> CreateDensityUnitsParsingDictionary()
        {
            var result = new Dictionary<string, DensityUnit>();

            var possibleValues = (DensityUnit[]) EnumUtil.GetEnumPossibleValues<DensityUnit>();
            foreach (var thisValue in possibleValues) result.Add(thisValue.ToString(), thisValue);

            return result;
        }

        public static DensityUnit ParseDensityUnit(string unitName)
        {
            return _mDensityUnits[unitName];
        }

        public static VolumeUnit ParseVolumeUnit(string unitName)
        {
            return _mVolumeUnits[unitName];
        }


        public static MassUnit ParseMassUnit(string unitName)
        {
            return _mMassUnits[unitName];
        }

        public static DistanceUnit ParseDistanceUnit(string unitName)
        {
            return _mDistanceUnits[unitName];
        }

        public static SpecificEnergyUnit ParseSpecificEnergyUnit(string unitName)
        {
            return _mSpecificEnergyUnits[unitName];
        }
    }


    public static class ConversionHelper
    {
        public static Enum GetConversionUnitByFullName(string conversionUnit, out SelectedUnitEnum selectedEnumType)
        {
            Enum result = SelectedUnitEnum.NotSet; // not a valid value or even in the set of valid values
            selectedEnumType = SelectedUnitEnum.NotSet;

            var parts = conversionUnit.Split('.');
            if (parts.Length != 2)
                throw new Exception("Cannot split ConversionUnit argument (\"" + conversionUnit +
                                    "\") into two parts for conversion.");

            var enumName = parts[0].ToUpper();
            var enumValue = parts[1];

            if (enumName == "SPEEDUNIT")
            {
                SpeedUnit suResult;
                if (Enum.TryParse(enumValue, out suResult))
                {
                    result = suResult;
                    selectedEnumType = SelectedUnitEnum.SpeedUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("SpeedUnit", enumValue);

                    throw new Exception("Cannot parse SpeedUnit." + enumValue);
                }
            }
            else if (enumName == "AREAUNIT")
            {
                AreaUnit auResult;
                if (Enum.TryParse(enumValue, out auResult))
                {
                    result = auResult;
                    selectedEnumType = SelectedUnitEnum.AreaUnit;
                }
                else
                {
                    throw new Exception("Cannot parse AreaUnit." + enumValue);
                }
            }
            else if (enumName == "DISTANCEUNIT")
            {
                DistanceUnit duResult;
                if (Enum.TryParse(enumValue, out duResult))
                {
                    result = duResult;
                    selectedEnumType = SelectedUnitEnum.DistanceUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("DistanceUnit", enumValue);
                }
            }
            else if (enumName == "TEMPUNIT")
            {
                TempUnit tuResult;
                if (Enum.TryParse(enumValue, out tuResult))
                {
                    result = tuResult;
                    selectedEnumType = SelectedUnitEnum.TempUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("TempUnit", enumValue);
                }
            }
            else if (enumName == "PRESSUREUNIT")
            {
                PressureUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.PressureUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("PressureUnit", enumValue);
                }
            }
            else if (enumName == "UNITLESSUNIT")
            {
                UnitlessUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.UnitlessUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("UnitlessUnit", enumValue);
                }
            }
            else if (enumName == "ENERGYUNIT")
            {
                EnergyUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.EnergyUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("EnergyUnit", enumValue);
                }
            }
            else if (enumName == "ANGLEUNIT")
            {
                AngleUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.AngleUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("AngleUnit", enumValue);
                }
            }
            else if (enumName == "VOLUMEUNIT")
            {
                VolumeUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.VolumeUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("VolumeUnit", enumValue);
                }
            }
            else if (enumName == "VOLUMETRICFLOWUNIT" || enumName == "VOLUMETRICFLOW")
            {
                VolumetricFlowUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.VolumetricFlowUnit;
                }
            }
            else if (enumName == "MANPOWERTIMEUNIT")
            {
                ManpowerTimeUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.ManpowerTimeUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("ManpowerTimeUnit", enumValue);
                }
            }
            else if (enumName == "JULIANTIMECONVERSIONUNIT")
            {
                JulianTimeConversionUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.JulianTimeConversionUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("JulianTimeConversionUnit", enumValue);
                }
            }
            else if (enumName == "ELAPSINGTIMECONVERSIONUNIT")
            {
                ElapsingTimeConversionUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.ElapsingTimeConversionUnit;
                }
                else
                {
                    ThrowUnitEnumFailure("ElapsingTimeConversionUnit", enumValue);
                }
            }
            else if (enumName == "DENSITYUNIT")
            {
                DensityUnit eResult;
                if (Enum.TryParse(enumValue, out eResult))
                {
                    result = eResult;
                    selectedEnumType = SelectedUnitEnum.DensityUnit;
                }
            }

            if ((SelectedUnitEnum) result == SelectedUnitEnum.NotSet)
                throw new Exception("ConversionUnit " + conversionUnit + " is unknown.");

            return result;
        }

        private static void ThrowUnitEnumFailure(string enumType, string enumValue)
        {
            throw new Exception("Cannot parse " + enumType + "." + enumValue);
        }
    }
}