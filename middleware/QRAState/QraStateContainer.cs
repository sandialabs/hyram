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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using JrCollections;
using JrConversions;

namespace QRAState
{
    /// <summary>
    /// Collection of param containers
    /// </summary>
    public class ParameterWrapperCollection : Dictionary<string, ParameterWrapper>
    {
        public ParameterWrapperCollection(ParameterWrapper[] initValues)
        {
            foreach (var thisObject in initValues) Add(thisObject.Key, thisObject);
        }
    }

    /// <summary>
    /// Convenience container which holds analysis parameters and references to their units of measurement and conversion helpers
    /// </summary>
    public class ParameterWrapper
    {
        /// <summary>
        /// Realistic parameter container
        /// </summary>
        /// <param name="key">Param key</param>
        /// <param name="variableDisplayName">Param label</param>
        /// <param name="unit">Actual parameter unit of measurement</param>
        /// <param name="converter">Which object to use to convert between units of measurement (e.g. feet to meters)</param>
        public ParameterWrapper(string key, string variableDisplayName, Enum unit = null,
            UnitOfMeasurementConverters converter = null)
        {
            if (unit == null) unit = UnitlessUnit.Unitless;
            if (converter == null) converter = StockConverters.UnitlessConverter;

            Key = key;
            Unit = unit;
            MConverter = converter;
            VariableDisplayName = variableDisplayName;
        }

        public string Key { get; set; }

        public UnitOfMeasurementConverters Converter
        {
            get => MConverter;
            set => MConverter = value;
        }

        public string VariableDisplayName { get; }

        public double[] OriginalValues { get; set; } = null;

        public Enum Unit { get; set; }

        public UnitOfMeasurementConverters MConverter
        {
            get => MConverter1;
            set => MConverter1 = value;
        }

        public UnitOfMeasurementConverters MConverter1 { get; set; }
    }

    /// <summary>
    /// Locking mechanism for modifying the db state container
    /// </summary>
    public class Synchronize
    {
        public static object LockId = new object();
    }

    /// <summary>
    /// Database containing all analysis parameters.
    /// Params are updated when user changes an input and are retrieved from DB when analysis is requested.
    /// </summary>
    [Serializable]
    public class QraStateContainer
    {
        // TODO (Cianan): The following updates could be performed to improve this class
        // - All param values are known so store them as properties on a DBTable object 
        // - Add suitable wrapper for setting min/max, unit conversions to reinforce MVC
        // - Centralize all external parameter edits to use single function/db entry point

        // Main store of parameter values. Initialized in InitDatabase.
        // Analysis parameters stored in mDatabase["PARAMETERS"]. Default values stored in mDatabase["DEFAULTS"]

        public string[] Distributions = {"Beta", "LogNormal", "Normal", "ExpectedValue", "Uniform"};

        public List<NozzleModel> NozzleModels = new List<NozzleModel>
        {
            NozzleModel.Birch, NozzleModel.Birch2, NozzleModel.EwanMoodie, NozzleModel.YuceilOtugen,
            NozzleModel.HarstadBellan
        };

        public List<DeflagrationModel> DeflagrationModels = new List<DeflagrationModel>
        {
            DeflagrationModel.Bauwens, DeflagrationModel.Cfd
        };

        public List<FuelType> FuelTypes = new List<FuelType>
        {
            FuelType.H2, FuelType.LH2, FuelType.CNG, FuelType.LNG
        };

        public List<ThermalProbitModel> ThermalProbitModels = new List<ThermalProbitModel>
        {
            ThermalProbitModel.Eisenberg, ThermalProbitModel.Tsao, ThermalProbitModel.Tno, ThermalProbitModel.Lees
        };

        public List<OverpressureProbitModel> OverpressureProbitModels = new List<OverpressureProbitModel>
        {
            OverpressureProbitModel.LungEis, OverpressureProbitModel.LungHse, OverpressureProbitModel.Head,
            OverpressureProbitModel.Collapse, OverpressureProbitModel.Debris
        };

        // AppData/HyRAM dir for non-user-specific output
        public static string AppDataDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                "HyRAM");

        // User AppData/HyRAM dir for user-specific output like logs, plots, etc.
        public static string UserDataDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HyRAM");

        private const string ParamTableName = "PARAMETERS";
        private const string DefaultsTableName = "DEFAULTS";

        // Do not delete
        private bool _mGasAndFlameDetectionOn = true;

        private static QraStateContainer _mInstance = new QraStateContainer();

        private const string OccupantDistributionsCollectionKey = "OccupantDistributions";

        public PressureUnit CfdPressureUnit = PressureUnit.Pa;
        private ElapsingTimeConversionUnit _mExposureTimeUnit = ElapsingTimeConversionUnit.Second;

        private ClsProperties Database { get; set; }

        /// <summary>
        /// Get parameter set stored in Database
        /// </summary>
        public ClsProperties Parameters
        {
            get
            {
                ClsProperties result = null;
                lock (Synchronize.LockId)
                {
                    if (Database == null) InitDatabase();

                    result = (ClsProperties) Database[ParamTableName];
                }

                return result;
            }
        }

        /// <summary>
        ///     Get default values of parameters, stored in Database
        /// </summary>
        public ClsProperties Defaults
        {
            get
            {
                ClsProperties result = null;
                lock (Synchronize.LockId)
                {
                    if (Database == null) InitDatabase();

                    result = (ClsProperties) Database[DefaultsTableName];
                }

                return result;
            }
        }

        public static QraStateContainer Instance
        {
            get
            {
                QraStateContainer result = null;
                lock (Synchronize.LockId)
                {
                    result = _mInstance;
                }

                return result;
            }
            set
            {
                lock (Synchronize.LockId)
                {
                    _mInstance = value;
                }
            }
        }

        /// <summary>
        /// Check whether results based on current parameters.
        /// If params have changed since analysis submitted, results will be stale.
        /// </summary>
        /// <returns>true if results based on current params.</returns>
        public static bool ResultsPristine()
        {
            var areStale = GetValue<bool>("ResultsAreStale");
            return !areStale;
        }

        // TODO (Cianan): Added these to simplify data binding. Propagate their use.
        /// <summary>
        /// Retrieve parameter value in specified unit of measurement
        /// </summary>
        /// <param name="key">string key of param in DB</param>
        /// <param name="unit">Unit of measurement </param>
        /// <returns></returns>
        public static double GetNdValue(string key, Enum unit = null)
        {
            if (unit == null) unit = UnitlessUnit.Unitless;

            return Instance.GetStateDefinedValueObject(key).GetValue(unit)[0];
        }

        /// <summary>
        /// Set parameter value in DB based on incoming value and unit of measurement
        /// </summary>
        /// <param name="key">DB string of param</param>
        /// <param name="unit">Unit of measurement</param>
        /// <param name="value">Value of param, in specified units</param>
        public static void SetNdValue(string key, Enum unit, double value)
        {
            var valueNode = Instance.GetStateDefinedValueObject(key);
            valueNode.SetValue(unit, new[] {value});
        }

        /// <summary>
        /// Retrieve parameter from DB (from Parameter collection, or default if former doesn't exist yet)
        /// </summary>
        /// <typeparam name="T">Type of parameter we're getting</typeparam>
        /// <param name="key">Reference key of parameter</param>
        /// <returns>Parameter value of type T</returns>
        public static T GetValue<T>(string key)
        {
            T result;
            var uKey = key.ToUpper();

            if (Instance.Parameters.ContainsKey(uKey))
                result = (T) Instance.Parameters[uKey];
            else if (Instance.Defaults.ContainsKey(uKey))
                result = (T) Instance.Defaults[uKey];
            else
                throw new Exception(key + " not present in state database");

            return result;
        }

        /// <summary>
        /// Retrieve param from DB as generic object
        /// </summary>
        /// <param name="key">Param string ref in DB</param>
        /// <returns></returns>
        public static object GetObject(string key)
        {
            var result = default(object);
            var uKey = key.ToUpper();

            if (Instance.Parameters.ContainsKey(uKey))
                result = Instance.Parameters[uKey];
            else if (Instance.Defaults.ContainsKey(uKey))
                result = Instance.Defaults[uKey];
            else
                throw new Exception(key + " not present in state database");

            return result;
        }

        /// <summary>
        ///     Set parameter value
        /// </summary>
        /// <typeparam name="T">Type of parameter we're setting</typeparam>
        /// <param name="key">Reference key of parameter</param>
        public static void SetValue<T>(string key, T newVal)
        {
            var uKey = key.ToUpper();
            if (Instance.Parameters.ContainsKey(uKey))
                Instance.Parameters[uKey] = newVal;
            else
                throw new Exception(key + " not present in parameters");
        }

        /// <summary>
        /// Retrieve param, which is list, from DB
        /// </summary>
        /// <param name="key"></param>
        /// <param name="unit"></param>
        /// <returns></returns>
        public static double[] GetNdValueList(string key, Enum unit = null)
        {
            if (unit == null) unit = UnitlessUnit.Unitless;

            return Instance.GetStateDefinedValueObject(key).GetValue(unit);
        }

        /// <summary>
        /// Retrieve comment param
        /// </summary>
        public string Comments
        {
            get
            {
                if (Parameters.Contains("Comments"))
                    return (string) Parameters["Comments"];
                return "";
            }
            set => Parameters["Comments"] = value;
        }

        public bool IsItemInDatabase(string key)
        {
            var convertibleValue = GetStateDefinedValueObject(key);
            return convertibleValue != null;
        }

        /// <summary>
        /// Retrieve param storage object from DB
        /// </summary>
        /// <param name="key">Param ref in DB</param>
        /// <returns>Storage object holding param value and converter</returns>
        public NdConvertibleValue GetStateDefinedValueObject(string key)
        {
            var result =
                GetStateDefinedValueObject(key, out var throwAwayConverter);
            if (result == null)
            {
                var errorMsg = "Cannot find " + key + " in state-defined values";
                Trace.TraceError(errorMsg);
                throw new Exception(errorMsg);
            }

            if (result.Converters.HasBadConversionFactor())
            {
                result.Converters = GetConverterByDatabaseKey(key);
            }
            return result;
        }

        // Retrieve value of key as ndConvertibleValue from Parameters or from Defaults, including converter.
        public NdConvertibleValue GetStateDefinedValueObject(string key, out UnitOfMeasurementConverters converter)
        {
            NdConvertibleValue
                result = null; // Will be checked on all cases, even first, to prevent bugs when code reordered
            var ukey = key.ToUpper();

            if (Parameters.ContainsKey(ukey)) result = (NdConvertibleValue) Parameters[ukey];

            if (result == null)
                if (Defaults.ContainsKey(ukey))
                    result = (NdConvertibleValue) Defaults[ukey];

            converter = result?.Converters;
            return result;

        }

        public ElapsingTimeConversionUnit ExposureTimeUnit
        {
            get => _mExposureTimeUnit;
            set => _mExposureTimeUnit = value;
        }

        /// <summary>
        /// Initialize database set objects which hold all analysis parameters (default and parameter sets)
        /// and ensure occupant distributions are initialized within DB.
        /// 
        /// </summary>
        private void InitDatabase()
        {
            Database = new ClsProperties();

            Database[ParamTableName] = new ClsProperties();
            Database[DefaultsTableName] = new ClsProperties();

            Database[ParamTableName] = InitDefaults();
            Database[DefaultsTableName] = InitDefaults();
            InitOccupantDistributions();
        }

        public OccupantDistributionInfoCollection OccupantDistributionInfoCollection =>
            (OccupantDistributionInfoCollection) Parameters[OccupantDistributionsCollectionKey];

        public void InitOccupantDistributions(bool force = false)
        {
            if (!Parameters.ContainsKey(OccupantDistributionsCollectionKey) || force)
                Parameters[OccupantDistributionsCollectionKey] = new OccupantDistributionInfoCollection(true);
        }

        /// <summary>
        /// Set up all parameter default values in DB set.
        /// Note that the full DB contains two of these sets: Parameters and Defaults.
        /// TODO: No reason to have two sets. Get rid of Defaults ClsProperties obj.
        /// </summary>
        /// <returns></returns>
        private ClsProperties InitDefaults()
        {
            var database = new ClsProperties();

#if DEBUG
            database["debug"] = true;
#else
            database["debug"] = false;
#endif

            // Track whether QRA must be run when next visiting results tabs.
            // Naive for now; set to true on load and whenever user visits an input tab. False after analysis executed.
            database["ResultsAreStale"] = true;
            database["Result"] = null;

            // Pipe outer diameter [inches]
            database["SysParam.PipeOD"] = new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Inch,
                new[] {0.375D}, 0D);
            // Pipe wall thickness [inches]
            database["SysParam.PipeWallThick"] = new NdConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Inch, new[] {0.065D}, 0D);
            // System pressure [MPa]
            database["SysParam.InternalPresMPA"] = new NdConvertibleValue(StockConverters.PressureConverter,
                PressureUnit.MPa, new[] {35D}, 0D);
            //System temperature [C]
            database["SysParam.InternalTempC"] = new NdConvertibleValue(StockConverters.TemperatureConverter,
                TempUnit.Celsius, new[] {15D}, -273D);
            database["SysParam.ExternalTempC"] = new NdConvertibleValue(StockConverters.TemperatureConverter,
                TempUnit.Celsius, new[] {15D}, -273D);
            // External pressure [MPa]
            database["SysParam.ExternalPresMPa"] = new NdConvertibleValue(StockConverters.PressureConverter,
                PressureUnit.MPa, new[] {.101325}, 0D);
            database["QRAD:EXCLUSIONRADIUS"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {.01});

            database["ImmedIgnitionProbs"] = new[] {0.008D, 0.053D, 0.23D};
            database["DelayIgnitionProbs"] = new[] {0.004D, 0.027D, 0.12D};
            database["IgnitionThresholds"] = new[] {0.125, 6.25};
            ;

            database["FuelType"] = FuelType.H2;
            database["ThermalProbit"] = ThermalProbitModel.Eisenberg;
            database["OverpressureProbit"] = OverpressureProbitModel.Collapse;

            database["RadiativeSourceModel"] = RadiativeSourceModels.Multi;

            //Nozzle model for gas jets
            // TODO (Cianan): add separate notional nozzle parameter for phys param?
            database["NozzleModel"] = NozzleModel.YuceilOtugen;

            //Model for deflagrations/detonations
            database["DeflagrationModel"] = DeflagrationModel.Bauwens;

            //Stores the last/default inputted unit for all stored variables
            database["VarUnitDict"] = new Dictionary<string, Enum>();

            database["OpWrapper.PlotDotsPressureAtTimes"] = new[]
            {
                new NdPressureAtTime(1, 13.8D),
                new NdPressureAtTime(15, 15.0D),
                new NdPressureAtTime(20, 55.2D)
            };

            // Optional overrides whereby user can control release for each size. -1 to ignore it.
            database["H2Release.000d01"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.000d10"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.001d00"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.010d00"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.100d00"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["Pdetectisolate"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {.9D}, 0D, 1D);
            database["Failure.ManualOverride"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);

            database["Components.NrCompressors"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["Components.NrCylinders"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["Components.NrValves"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {5}, 0D);
            database["Components.NrInstruments"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {3}, 0D);
            database["Components.NrJoints"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {35}, 0D);
            database["Components.NrHoses"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {1}, 0D);
            database["Components.PipeLength"] = new NdConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new double[] {20}, 0D);
            database["Components.NrFlanges"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["Components.NrFilters"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["Components.NrExtraComp1"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["Components.NrExtraComp2"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);

            database["Facility.Length"] = new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter,
                new double[] {20}, 0D);
            database["Facility.Width"] = new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter,
                new double[] {12}, 0D);
            database["Facility.Height"] = new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter,
                new double[] {5}, 0D);

            // Discharge coefficient of the hole; 1.0 is a conservative value, 0.6 is suggested value for screening
            database["DischargeCoefficient"] = new NdConvertibleValue(GetConverterByDatabaseKey("DischargeCoefficient"),
                UnitlessUnit.Unitless, new[] {1D}, 0D);
            database["nVehicles"] = new NdConvertibleValue(GetConverterByDatabaseKey("nVehicles"),
                UnitlessUnit.Unitless, new double[] {20}, 0D);
            database["nFuelingsPerVehicleDay"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("nFuelingsPerVehicleDay"), UnitlessUnit.Unitless, new double[] {2}, 0D);
            database["nDemands"] = new NdConvertibleValue(GetConverterByDatabaseKey("nDemands"), UnitlessUnit.Unitless,
                new[] {10000D}, 0D);
            database["nVehicleOperatingDays"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("nVehicleOperatingDays"), UnitlessUnit.Unitless, new[] {250D}, 0D,
                366D);

            database["LeakHeight"] = new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter,
                new[] {1.0D}, 0D);
            database["ReleaseAngle"] = new NdConvertibleValue(StockConverters.AngleConverter, AngleUnit.Degrees,
                new[] {0.0D}, 0D, 180.0D);

            database["Failure.NozPO"] =
                new FailureMode("Nozzle", "Pop-off", FailureDistributionType.Beta, 0.5D, 610415.5D);
            //Database["Failure.NozFTC"] = new FailureMode("Nozzle", "Failure to close", FailureDistributionType.Beta, 31.5D, 610384.5D);
            database["Failure.NozFTC"] = new FailureMode("Nozzle", "Failure to close",
                FailureDistributionType.ExpectedValue, 0.002D, 0D);
            database["Failure.MValveFTC"] = new FailureMode("Manual valve", "Failure to close",
                FailureDistributionType.ExpectedValue, 0.001D, 0D);
            database["Failure.SValveFTC"] = new FailureMode("Solenoid valve", "Failure to close",
                FailureDistributionType.ExpectedValue, 0.002D, 0D);
            database["Failure.SValveCCF"] = new FailureMode("Solenoid valve", "Common-cause failure",
                FailureDistributionType.ExpectedValue, 0.00012766D, 0D);

            database["Failure.Overp"] = new FailureMode("Overpressure during fueling", "Accident",
                FailureDistributionType.Beta, 3.5D, 310289.5D);
            database["Failure.PValveFTO"] = new FailureMode("Pressure relief valve", "Failure to open",
                FailureDistributionType.LogNormal, -11.7359368859313D, 0.667849415603714D);
            database["Failure.Driveoff"] =
                new FailureMode("Driveoff", "Accident", FailureDistributionType.Beta, 31.5D, 610384.5D);
            database["Failure.CouplingFTC"] = new FailureMode("Breakaway coupling", "Failure to close",
                FailureDistributionType.Beta, 0.5D, 5031.0D);

            // Component leak probabilities stored in lists of objects for easy data-binding
            // Current types are: Compressors, Cylinders, Filters, Flanges, Hoses, Joints, Pipes, Valves, Instruments, extra 1, extra 2
            // Array elements are leak size, Mu, Sigma, Mean, Variance, in order.
            database["Prob.Compressor"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -1.7198D, 0.2143D, 1.83E-001D, 1.58E-003D),
                new ComponentProbability("0.10%", -3.9185D, 0.4841D, 2.23e-002D, 1.32e-004D),
                new ComponentProbability("1%", -5.1394D, 0.7898D, 8.01e-003D, 5.55e-005D),
                new ComponentProbability("10%", -8.8408D, 0.8381D, 2.06e-004D, 4.31e-008D),
                new ComponentProbability("100%", -11.3365D, 1.3689D, 3.04e-005D, 5.11e-009D)
            };

            // cyl
            database["Prob.Cylinder"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -13.8364D, 0.6156D, 1.18e-006D, 6.46e-013D),
                new ComponentProbability("0.10%", -14.0010D, 0.6065D, 9.98e-007D, 4.43e-013D),
                new ComponentProbability("1%", -14.3953D, 0.6232D, 6.80e-007D, 2.19e-013D),
                new ComponentProbability("10%", -14.9562D, 0.6290D, 3.90e-007D, 7.36e-014D),
                new ComponentProbability("100%", -15.6047D, 0.6697D, 2.09e-007D, 2.47e-014D)
            };

            // filter
            database["Prob.Filter"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.2471D, 1.9849D, 3.77e-002D, 7.18e-002D),
                new ComponentProbability("0.10%", -5.2884D, 1.5180D, 1.60e-002D, 2.30e-003D),
                new ComponentProbability("1%", -5.3389D, 1.4806D, 1.44e-002D, 1.64e-003D),
                new ComponentProbability("10%", -5.3758D, 0.8886D, 6.87e-003D, 5.67e-005D),
                new ComponentProbability("100%", -5.4257D, 0.9544D, 6.94e-003D, 7.16e-005D)
            };

            // flang
            database["Prob.Flange"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -3.9236D, 1.6611D, 7.86e-002D, 9.13e-002D),
                new ComponentProbability("0.10%", -6.1211D, 1.2533D, 4.82e-003D, 8.84e-005D),
                new ComponentProbability("1%", -8.3307D, 2.2024D, 2.72e-003D, 9.41e-004D),
                new ComponentProbability("10%", -10.5399D, 0.8332D, 3.74e-005D, 1.41e-009D),
                new ComponentProbability("100%", -12.7453D, 1.8274D, 1.55e-005D, 6.53e-009D)
            };

            // hose
            database["Prob.Hose"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -6.8061D, 0.2682D, 1.15e-003D, 9.82e-008D),
                new ComponentProbability("0.10%", -8.6394D, 0.5520D, 2.06e-004D, 1.51e-008D),
                new ComponentProbability("1%", -8.7740D, 0.5442D, 1.79e-004D, 1.11e-008D),
                new ComponentProbability("10%", -8.8926D, 0.5477D, 1.60e-004D, 8.92e-009D),
                new ComponentProbability("100%", -9.8600D, 0.8457D, 7.47e-005D, 5.82e-009D)
            };

            // joint
            database["Prob.Joint"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -9.5738D, 0.1638D, 7.05e-005D, 1.35e-010D),
                new ComponentProbability("0.10%", -12.8316D, 0.7575D, 3.56e-006D, 9.84e-012D),
                new ComponentProbability("1%", -11.8743D, 0.4750D, 7.80e-006D, 1.54e-011D),
                new ComponentProbability("10%", -12.0156D, 0.5302D, 6.96e-006D, 1.57e-011D),
                new ComponentProbability("100%", -12.1486D, 0.5652D, 6.21e-006D, 1.45e-011D)
            };

            // pipe
            database["Prob.Pipe"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -11.8584D, 0.6570D, 8.78e-006D, 4.16e-011D),
                new ComponentProbability("0.10%", -12.5337D, 0.6884D, 4.57e-006D, 1.26e-011D),
                new ComponentProbability("1%", -13.8662D, 1.1276D, 1.80e-006D, 8.27e-012D),
                new ComponentProbability("10%", -14.5757D, 1.1555D, 9.12e-007D, 2.33e-012D),
                new ComponentProbability("100%", -15.7261D, 1.7140D, 6.43e-007D, 7.39e-012D)
            };

            // valve
            database["Prob.Valve"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.1796, 0.1728D, 5.71e-003D, 9.90e-007D),
                new ComponentProbability("0.10%", -7.2748D, 0.3983D, 7.50e-004D, 9.67e-008D),
                new ComponentProbability("1%", -9.6802D, 0.9607D, 9.92e-005D, 1.49e-008D),
                new ComponentProbability("10%", -10.3230D, 0.6756D, 4.13e-005D, 9.86e-010D),
                new ComponentProbability("100%", -11.9960D, 1.3304D, 1.49e-005D, 1.09e-009D)
            };

            // instr
            database["Prob.Instrument"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -7.3205D, 0.6756D, 8.31e-004D, 4.00e-007D),
                new ComponentProbability("0.10%", -8.5018D, 0.7938D, 2.78e-004D, 6.80e-008D),
                new ComponentProbability("1%", -9.0619D, 0.8952D, 1.73e-004D, 3.68e-008D),
                new ComponentProbability("10%", -9.1711D, 1.0674D, 1.84e-004D, 7.18e-008D),
                new ComponentProbability("100%", -10.1962D, 1.4795D, 1.11e-004D, 9.85e-008D)
            };

            // ex1
            database["Prob.Extra1"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0, 0, 0),
                new ComponentProbability("0.10%", 0, 0, 0, 0),
                new ComponentProbability("1%", 0, 0, 0, 0),
                new ComponentProbability("10%", 0, 0, 0, 0),
                new ComponentProbability("100%", 0, 0, 0, 0)
            };

            // ex2
            database["Prob.Extra2"] = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0, 0, 0),
                new ComponentProbability("0.10%", 0, 0, 0, 0),
                new ComponentProbability("1%", 0, 0, 0, 0),
                new ComponentProbability("10%", 0, 0, 0, 0),
                new ComponentProbability("100%", 0, 0, 0, 0)
            };

            // value taken from call_OverpressureWrapper.py, M**3
            database["OpWrapper.TankVolume"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.TankVolume"),
                VolumeUnit.CubicMeter, new[] {3.63 / 1000});
            database["OpWrapper.Cd0"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.Cd0"),
                UnitlessUnit.Unitless, new[] {1D});

            var cdR = 1D;
            var cdBl = 0.51D;
            database["OpWrapper.CdR"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.CdR"),
                UnitlessUnit.Unitless, new[] {cdR});
            database["OpWrapper.CdBl"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.CdBl"),
                UnitlessUnit.Unitless, new[] {cdBl});
            // Value from S0 in Exec_OpWrapper. Release Height
            database["OpWrapper.S0"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.S0"),
                DistanceUnit.Meter, new[] {0.2495D});
            // Value from Xwall in Exec_OpWrapper. Distance from release to wall
            database["OpWrapper.Xwall"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.Xwall"),
                DistanceUnit.Meter, new[] {2.1255D});
            database["OpWrapper.WindAngle"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.WindAngle"),
                AngleUnit.Radians, new[] {0 * Math.PI / 180});
            database["OpWrapper.Cdv"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.Cdv"),
                UnitlessUnit.Unitless, new[] {0.61});
            database["OpWrapper.Aceil"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.Aceil"),
                AreaUnit.SqMeters, new[] {4.594 * 3.64});
            // dist. between ceiling and vent
            database["OpWrapper.Hv"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.Hv"),
                DistanceUnit.Meter, new[] {0.3});
            database["OpWrapper.Av_ceil"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.Av_ceil"),
                AreaUnit.SqMeters, new[] {Math.Pow(0.34, 2) * Math.PI / 4});
            // Floor vent (area?)
            database["OpWrapper.Av_floor"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.Av_floor"),
                AreaUnit.SqMeters, new[] {0.12 * 0.0635});
            // Height of enclosure, meters
            database["OpWrapper.H"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.H"),
                DistanceUnit.Meter, new[] {2.72});
            // Speed of wind
            database["OpWrapper.VolumeFlowRate"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("OpWrapper.VolumeFlowRate"), VolumetricFlowUnit.CubicMetersPerSecond,
                new[] {0D});
            //double SecondaryArea = .131 * .131 * CdBl * CdR;
            database["OpWrapper.SecondaryArea"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("OpWrapper.SecondaryArea"), AreaUnit.SqMeters, new[] {0.01716});
            database["OpWrapper.FCA"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.FCA"),
                AreaUnit.SqMeters, new[] {16.72216D});
            database["OpWrapper.CVHF"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.CVHF"),
                DistanceUnit.Meter, new[] {2.42D});
            database["OpWrapper.FVHF"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.FVHF"),
                DistanceUnit.Meter, new[] {0.044});
            database["OpWrapper.CVDC"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.CVDC"),
                UnitlessUnit.Unitless, new[] {0.61});
            database["OpWrapper.FVDC"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.FVDC"),
                UnitlessUnit.Unitless, new[] {0.61});

            database["OpWrapper.SecondsToPlot"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("OpWrapper.SecondsToPlot"), ElapsingTimeConversionUnit.Second,
                new[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27,
                    28, 29, 29.5
                });

            database["OpWrapper.LimitLinePressures"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("OpWrapper.LimitLinePressures"), PressureUnit.KPa,
                new[] {13.8D, 15.0D, 55.2D});

            // Fill Plume wrapper defaults
            // co-volume constant (default is 7.6921e-3 m^3/kg, valid for H2)
            database["PlumeWrapper.PlotTitle"] = "Mole Fraction of Leak";
            database["PlumeWrapper.XMin"] = new NdConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {-2.5D});
            database["PlumeWrapper.XMax"] = new NdConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {2.5D});
            database["PlumeWrapper.YMin"] = new NdConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {0D});
            database["PlumeWrapper.YMax"] = new NdConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {10D});
            database["PlumeWrapper.Contours"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {0.04});

            database["PlumeWrapper.co_volume_constant"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.co_volume_constant"), DensityUnit.KilogramCubicMeter,
                new[] {7.6921e-3});
            database["PlumeWrapper.distance_to_wall"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.distance_to_wall"), DistanceUnit.Meter, new[] {100D});
            database["PlumeWrapper.jet_angle"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.jet_angle"), AngleUnit.Radians, new[] {1.5708D});
            database["PlumeWrapper.FlameBoundary"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.FlameBoundary"), UnitlessUnit.Unitless,
                new[] {0.04D, 0.08D});

            // Fill QRA enclosure upper and lower vent defaults
            database["Enclosure.Height"] = new NdConvertibleValue(GetConverterByDatabaseKey("Enclosure.Height"),
                DistanceUnit.Meter, new[] {2D});
            database["Enclosure.AreaOfFloorAndCeiling"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("Enclosure.AreaOfFloorAndCeiling"), AreaUnit.SqMeters, new[] {10D});
            database["Enclosure.HeightOfRelease"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("Enclosure.HeightOfRelease"), DistanceUnit.Meter, new[] {1D});
            database["Enclosure.XWall"] = new NdConvertibleValue(GetConverterByDatabaseKey("Enclosure.XWall"),
                DistanceUnit.Meter, new[] {3D});

            // Vent inputs
            foreach (WhichVent ventPurpose in Enum.GetValues(typeof(WhichVent)))
            {
                var crossSectionalAreaKey = ventPurpose + ".CrossSectionalArea";
                var heightFromFloorKey = ventPurpose + ".VentHeightFromFloor";
                var dischargeCoefficientKey = ventPurpose + ".DischargeCoefficient";
                var windVelocityKey = ventPurpose + ".WindVelocity";

                database[crossSectionalAreaKey] = new NdConvertibleValue(
                    GetConverterByDatabaseKey(crossSectionalAreaKey), AreaUnit.SqMeters, new[] {.1D});
                database[dischargeCoefficientKey] = new NdConvertibleValue(
                    GetConverterByDatabaseKey(dischargeCoefficientKey), UnitlessUnit.Unitless, new[] {1D});
                database[windVelocityKey] = new NdConvertibleValue(GetConverterByDatabaseKey(windVelocityKey),
                    SpeedUnit.MetersPerSecond, new[] {1D});

                var heightFromFloorDefault = double.NaN;
                if (ventPurpose == WhichVent.Ceiling)
                    heightFromFloorDefault = 2.5D;
                else if (ventPurpose == WhichVent.Floor) heightFromFloorDefault = 0.01D;

                database[heightFromFloorKey] = new NdConvertibleValue(GetConverterByDatabaseKey(heightFromFloorKey),
                    DistanceUnit.Meter, new[] {heightFromFloorDefault});
            }

            database["OpWrapper.MaxSimTime"] = new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.MaxSimTime"),
                ElapsingTimeConversionUnit.Second, new[] {30D});
            database["OpWrapper.ReleaseAngle"] =
                new NdConvertibleValue(GetConverterByDatabaseKey("OpWrapper.ReleaseAngle"), AngleUnit.Radians,
                    new[] {0D});

            // Person's flame exposure time [s] 
            database["t_expose_thermal"] = new NdConvertibleValue(GetConverterByDatabaseKey("t_expose_thermal"),
                ElapsingTimeConversionUnit.Second, new[] {60D}, 0D);

            database["FlameWrapper.T_H2"] = new NdConvertibleValue(GetConverterByDatabaseKey("FlameWrapper.T_H2"),
                TempUnit.Kelvin, new[] {287.8}, -273.14D);
            database["FlameWrapper.P_H2"] = new NdConvertibleValue(GetConverterByDatabaseKey("FlameWrapper.P_H2"),
                PressureUnit.MPa, new[] {13.42}, 0D);
            const double relHumidityDefaultFromFlamewrapperPy = 0.89d;
            database["FlameWrapper.RH"] = new NdConvertibleValue(GetConverterByDatabaseKey("FlameWrapper.RH"),
                UnitlessUnit.Unitless, new[] {relHumidityDefaultFromFlamewrapperPy}, 0D);
            database["FlameWrapper.d_orifice"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("FlameWrapper.d_orifice"), DistanceUnit.Meter, new[] {0.00356}, 0D);
            database["FlameWrapper.ReleaseHeight"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("FlameWrapper.ReleaseHeight"), DistanceUnit.Meter, new double[] {1}, 0.01D);
            database["FlameWrapper.radiative_heat_flux_point:x"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("FlameWrapper.radiative_heat_flux_point:x"), DistanceUnit.Meter,
                new[] {0.01D, 0.5D, 1D, 2D, 2.5D, 5D, 10D, 15D, 25D, 40D});
            database["FlameWrapper.radiative_heat_flux_point:y"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("FlameWrapper.radiative_heat_flux_point:y"), DistanceUnit.Meter,
                new[] {1D, 1D, 1D, 1D, 1D, 2D, 2D, 2D, 2D, 2D});
            database["FlameWrapper.radiative_heat_flux_point:z"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("FlameWrapper.radiative_heat_flux_point:z"), DistanceUnit.Meter,
                new[] {0.01D, 0.5D, 0.5D, 1D, 1D, 1D, 0.5D, 0.5D, 1D, 2D});
            database["FlameWrapper.contour_levels"] = new NdConvertibleValue(
                GetConverterByDatabaseKey("FlameWrapper.contour_levels"), UnitlessUnit.Unitless,
                new[] {1.577, 4.732, 25.237}, 0.0);

            //  // Overpressure consequences
            //	P_s= [2.5e3 2.5e3 5e3 16e3 30e3]; //Peak explosion overpressure [Pa]
            database["P_s"] = new NdConvertibleValue(GetConverterByDatabaseKey("P_s"), PressureUnit.Pa,
                new[] {2.5e3, 2.5e3, 5e3, 16e3, 30e3});
            //	impulse=[250 500 1000 2000 4000]; //Impulse of shock wave [Pa*s] 
            database["impulse"] = new NdConvertibleValue(GetConverterByDatabaseKey("impulse"), PressureUnit.Pa,
                new double[] {0, 0, 0, 0, 0});

            // Defaults to arbitrary value - in this case, the number of steps the Glenn Frey statue unveiled 9/23/2016 would have
            // take to walk from Standin' on a Corner Park in Winslow Arizona to the Rock And Roll Hall of Fame in Cincinnatti, OH.
            database["RANDOMSEED"] = new NdConvertibleValue(GetConverterByDatabaseKey("RANDOMSEED"),
                UnitlessUnit.Unitless, new double[] {3632850});

            database["Dist.LeakScenarios"] = new NdConvertibleValue(GetConverterByDatabaseKey("Dist.LeakScenarios"),
                UnitlessUnit.Unitless, new[] {0.0001, 0.001, 0.01, 0.1, 1.0});

            return database;
        }

        /// <summary>
        /// Retrieve unit converter object based on param key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static UnitOfMeasurementConverters GetConverterByDatabaseKey(string key)
        {
            UnitOfMeasurementConverters result = null;
            var ucKey = key.ToUpper();

            switch (ucKey)
            {
                case "T_EXPOSE_THERMAL":
                case "OPWRAPPER.SECONDSTOPLOT":
                case "OPWRAPPER.MAXSIMTIME":
                    result = StockConverters.ElapsingTimeConverter;
                    break;
                case "FLAMEWRAPPER.T_H2":
                case "SYSPARAM.EXTERNALTEMPC":
                case "SYSPARAM.INTERNALTEMPC":
                    result = StockConverters.TemperatureConverter;
                    break;
                case "FLAMEWRAPPER.P_H2":
                case "P_S":
                case "IMPULSE":
                case "OPWRAPPER.LIMITLINEPRESSURES":
                case "SYSPARAM.EXTERNALPRESMPA":
                case "SYSPARAM.INTERNALPRESMPA":
                    result = StockConverters.PressureConverter;
                    break;
                case "FLAMEWRAPPER.D_ORIFICE":
                case "FLAMEWRAPPER.RELEASEHEIGHT":
                case "FLAMEWRAPPER.RADIATIVE_HEAT_FLUX_POINT:X":
                case "FLAMEWRAPPER.RADIATIVE_HEAT_FLUX_POINT:Y":
                case "FLAMEWRAPPER.RADIATIVE_HEAT_FLUX_POINT:Z":
                case "PLUMEWRAPPER.XMIN":
                case "PLUMEWRAPPER.XMAX":
                case "PLUMEWRAPPER.YMIN":
                case "PLUMEWRAPPER.YMAX":
                case "COMPONENTS.PIPELENGTH":
                case "FACILITY.LENGTH":
                case "FACILITY.WIDTH":
                case "FACILITY.HEIGHT":
                case "OPWRAPPER.S0":
                case "OPWRAPPER.XWALL":
                case "OPWRAPPER.HV":
                case "OPWRAPPER.H":
                case "OPWRAPPER.CVHF":
                case "OPWRAPPER.FVHF":
                case "PLUMEWRAPPER.DISTANCE_TO_WALL":
                case "SYSPARAM.PIPEOD":
                case "SYSPARAM.PIPEWALLTHICK":
                case "FLOOR.VENTHEIGHTFROMFLOOR":
                case "CEILING.VENTHEIGHTFROMFLOOR":
                case "ENCLOSURE.HEIGHT":
                case "ENCLOSURE.HEIGHTOFRELEASE":
                case "ENCLOSURE.XWALL":
                case "LEAKHEIGHT":
                    result = StockConverters.DistanceConverter;
                    break;
                case "OPWRAPPER.TANKVOLUME":
                    result = StockConverters.VolumeConverter;
                    break;
                case "OPWRAPPER.WINDANGLE":
                case "OPWRAPPER.RELEASEANGLE":
                case "PLUMEWRAPPER.JET_ANGLE":
                case "RELEASEANGLE":
                    result = StockConverters.AngleConverter;
                    break;
                case "OPWRAPPER.ACEIL":
                case "OPWRAPPER.AV_CEIL":
                case "OPWRAPPER.AV_FLOOR":
                case "OPWRAPPER.SECONDARYAREA":
                case "OPWRAPPER.FCA":
                case "ENCLOSURE.AREAOFFLOORANDCEILING":
                case "FLOOR.CROSSSECTIONALAREA":
                case "CEILING.CROSSSECTIONALAREA":
                    result = StockConverters.AreaConverter;
                    break;
                case "OPWRAPPER.VOLUMEFLOWRATE":
                    result = StockConverters.VolumetricFlowConverter;
                    break;
                case "PLUMEWRAPPER.CO_VOLUME_CONSTANT":
                    result = StockConverters.DensityConverter;
                    break;
                case "FLOOR.WINDVELOCITY":
                case "CEILING.WINDVELOCITY":
                    result = StockConverters.SpeedConverter;
                    break;
                case "PLUMEWRAPPER.CONTOURS":
                default:
                    result = StockConverters.UnitlessConverter;
                    break;
            }

            return result;
        }

        /// <summary>
        /// Serialize container (DB) into file
        /// </summary>
        /// <param name="filename"></param>
        public void Save(string filename)
        {
            Serialize(filename, this);
        }

        /// <summary>
        /// Fill out param values based on saved data in file.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static QraStateContainer Load(string filename)
        {
            SetValue("ResultsAreStale", true);
            SetValue<object>("Result", null);
            var result = Deserialize(filename);
            return result;
        }

        /// <summary>
        /// Write state to file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="objectToSerialize"></param>
        public static void Serialize(string filename, QraStateContainer objectToSerialize)
        {
            if (filename == null)
                throw new NullReferenceException("Failed to open writable binary file. Filename argument is null.");
            var formatter = new BinaryFormatter();

            // Open writable binary file
            var fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);

            try
            {
                formatter.Serialize(fs, objectToSerialize);
            }
            finally
            {
                fs.Close();
            }
        }

        /// <summary>
        /// Load parameter data from file and populate complex parameters accordingly
        /// </summary>
        /// <param name="filename">Path to save file</param>
        /// <returns>Populated state container</returns>
        public static QraStateContainer Deserialize(string filename)
        {
            QraStateContainer result = null;

            var fs =
                new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                var formatter = new BinaryFormatter();
                var oResult = formatter.Deserialize(fs);
                var typeName = oResult.GetType().ToString().ToUpper();
                result = (QraStateContainer) oResult;
                var savedParams = (ClsProperties) result.Database[ParamTableName];

                //Backward compatibility fix for changing the unit type on nVehicleOperatingDays
                if (result.GetStateDefinedValueObject("nVehicleOperatingDays").InputUnit is JulianTimeConversionUnit)
                    savedParams["nVehicleOperatingDays"] = new NdConvertibleValue(StockConverters.UnitlessConverter,
                        UnitlessUnit.Unitless,
                        result.GetStateDefinedValueObject("nVehicleOperatingDays")
                            .GetValue(JulianTimeConversionUnit.Day), 0, 366);

                if (savedParams.Keys.Intersect(new[]
                        {"NWORKERS", "YEARLYWORKINGHOURS", "MINDIST", "MAXDIST", "WORKERDISTMEAN", "WORKERDISTSTDDEV"})
                    .Any())
                {
                    var distributions =
                        (OccupantDistributionInfoCollection) savedParams["OccupantDistributions"];
                    var nworkers = result.GetStateDefinedValueObject("nworkers");
                    var workinghours = result.GetStateDefinedValueObject("yearlyworkinghours");
                    var varUnitDict = (Dictionary<string, Enum>) savedParams["VarUnitDict"];

                    if (((string) savedParams["WorkerDistribution"]).ToUpper() == "UNIFORM")
                    {
                        var mindist = result.GetStateDefinedValueObject("mindist");
                        var maxdist = result.GetStateDefinedValueObject("maxdist");
                        var unit = DistanceUnit.Meter;
                        Enum passUnit =
                            DistanceUnit
                                .Meter; // Not sure why Greg was passing basic Enums, trying to fix that. May roll back.

                        varUnitDict.TryGetValue("UniDistUnit", out passUnit);
                        unit = (DistanceUnit) passUnit; // If this fails, explains previous comment.

                        distributions.Clear();
                        var dmindist = mindist.GetValue(unit)[0];
                        var dmaxdist = maxdist.GetValue(unit)[0];

                        var workersOdi = new OccupantDistributionInfo(
                            (int) nworkers.GetValue(UnitlessUnit.Unitless)[0], "<imported from old save file>",
                            EWorkerDistribution.Uniform, dmindist, dmaxdist, EWorkerDistribution.Uniform, dmindist,
                            dmaxdist, EWorkerDistribution.Uniform, dmindist, dmaxdist,
                            DistanceUnit.Meter, 2000);


                        distributions.Add(workersOdi);
                    }
                    else if (((string) savedParams["WorkerDistribution"]).ToUpper() == "NORMAL")
                    {
                        Enum unit = DistanceUnit.Meter;
                        varUnitDict.TryGetValue("NormDistUnit", out unit);

                        distributions.Clear();
                        var workersOdi = new OccupantDistributionInfo(
                            (int) nworkers.GetValue(UnitlessUnit.Unitless)[0], "<imported from old save file>",
                            EWorkerDistribution.Uniform,
                            20, 12, EWorkerDistribution.Uniform, 20, 12, EWorkerDistribution.Uniform, .2, 1,
                            DistanceUnit.Meter, 2000);

                        distributions.Add(workersOdi);
                    }
                }

                //Backward compatability fix for max/min value checking
                foreach (var key in savedParams.Keys)
                {
                    var userValue = savedParams[key];
                    if (userValue is NdConvertibleValue)
                    {
                        var value = userValue as NdConvertibleValue;
                        if (value.MaxValue == 0 && value.MinValue == 0 && result.Defaults.ContainsKey(key))
                        {
                            value.MaxValue = ((NdConvertibleValue) result.Defaults[key]).MaxValue;
                            value.MinValue = ((NdConvertibleValue) result.Defaults[key]).MinValue;
                        }
                    }
                }
            }
            finally
            {
                fs.Close();
            }

            return result;
        }

        public bool GasAndFlameDetectionOn
        {
            get => _mGasAndFlameDetectionOn;

            set => _mGasAndFlameDetectionOn = value;
        }

        /// <summary>
        /// Clear current DB parameter values and set to defaults
        /// </summary>
        public void ResetInputsAndDefaults()
        {
            ResetDatabases();
            //ResetGlobalData();
        }

        /// <summary>
        /// Zero out stored parameter values
        /// Note (Cianan): this may not be functional after 2.0
        /// </summary>
        private void ResetDatabases()
        {
            foreach (var dbKeyValue in Database.Keys)
            {
                var thisItem = Database[dbKeyValue];
                if (thisItem is ClsProperties propsItem)
                    foreach (var propsItemKey in propsItem.Keys)
                    {
                        var thisValue = propsItem[propsItemKey];
                        if (thisValue is NdConvertibleValue cv)
                        {
                            for (var baseValueIndex = 0; baseValueIndex < cv.BaseValue.Length; baseValueIndex++)
                                cv.BaseValue[baseValueIndex] = 0.0D;
                        }
                        else if (thisValue is OccupantDistributionInfoCollection)
                        {
                            ; // Do not zero out.
                        }
                        else
                        {
                            var typeName = thisValue.GetType().Name;

                            throw new Exception("Value " + propsItemKey + " in sub-element " + dbKeyValue +
                                                " is an invalid type of " + typeName +
                                                ". ndConvertibleValue was expected.");
                        }
                    }
                else
                    throw new Exception("Database element " + dbKeyValue +
                                        " is not a valid type.  It is unsupported type " + thisItem.GetType().Name +
                                        ".");
            }
        }

        public void UndoStateDamageCausedByLoad()
        {
            // Some things in the QRA State container are dependent upon the local system.
            // Make sure that these things are reset back to their correct and local values.
            ResetUnitConverters();
        }

        private void ResetUnitConverters()
        {
            // Note (Cianan): not sure when this is applicable or when a converter is broken, per John's comment below.
            //
            //Make sure the latest version of a converter is being used after load.
            //clsProperties[] Databases = new clsProperties[] { UserSessionValues, Defaults, Constants };
            ClsProperties[] databases = {Parameters, Defaults};
            foreach (var thisDatabase in databases)
            foreach (var itemKey in thisDatabase.Keys)
            {
                var oValueNode = thisDatabase[itemKey];
                if (oValueNode is NdConvertibleValue valueNode)
                    valueNode.Converters = GetConverterByDatabaseKey(itemKey);
            }
        }
    }

    // Provides names to prepend database key for floor and ceiling grid input variables.
    public enum WhichVent
    {
        Floor,
        Ceiling
    } 

    //public enum NozzleModels { Birch, Birch2, EwanMoodie, HarstadBellan, Molkov, YuceilOtugen };
    public enum FailureDistributionType
    {
        Beta,
        LogNormal,
        ExpectedValue
    }

    public enum RadiativeSourceModels
    {
        None,
        Single,
        Multi
    }

    /// <summary>
    /// Classes for storing model choice including descriptive string name.
    /// Used instead of enum for more flexibility.
    /// </summary>
    [Serializable]
    public class NozzleModel
    {
        public static readonly NozzleModel Birch = new NozzleModel(0, "Birch", "birc");
        public static readonly NozzleModel Birch2 = new NozzleModel(1, "Birch2", "bir2");
        public static readonly NozzleModel EwanMoodie = new NozzleModel(2, "Ewan/Moodie", "ewan");
        public static readonly NozzleModel YuceilOtugen = new NozzleModel(3, "Yuceil/Otugen", "yuce");
        public static readonly NozzleModel HarstadBellan = new NozzleModel(4, "Harstad/Bellan", "hars");
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;

        public NozzleModel(int value, string name, string key)
        {
            _name = name;
            _value = value;
            _key = key;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }

        public static NozzleModel ParseNozzleModelKey(string key)
        {
            switch (key)
            {
                case "birc":
                    return Birch;
                case "bir2":
                    return Birch2;
                case "ewan":
                    return EwanMoodie;
                case "hars":
                    return HarstadBellan;
                case "yuce":
                default:
                    return YuceilOtugen;
            }
        }

        public static NozzleModel ParseNozzleModelName(string name)
        {
            switch (name)
            {
                case "Birch":
                    return Birch;
                case "Birch2":
                    return Birch2;
                case "Ewan/Moodie":
                    return EwanMoodie;
                case "Harstad/Bellan":
                    return HarstadBellan;
                case "Yuceil/Otugen":
                default:
                    return YuceilOtugen;
            }
        }
    }

    /// <summary>
    /// Representation of fuel type, designated by string label which is consumed by python funcs.
    /// </summary>
    [Serializable]
    public sealed class FuelType
    {
        public static readonly FuelType H2 = new FuelType(0, "H2", "h2");
        public static readonly FuelType LH2 = new FuelType(1, "LH2", "lh2");
        public static readonly FuelType CNG = new FuelType(2, "CNG", "cng");
        public static readonly FuelType LNG = new FuelType(3, "LNG", "lng");
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;

        private FuelType(int value, string name, string key)
        {
            _name = name;
            _value = value;
            _key = key;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }
    }

    /// <summary>
    /// Representation of thermal probit model, designated by string label which is consumed by python funcs.
    /// Determines thermal harm calculations.
    /// </summary>
    [Serializable]
    public sealed class ThermalProbitModel
    {
        public static readonly ThermalProbitModel Eisenberg = new ThermalProbitModel(0, "Eisenberg", "eise");
        public static readonly ThermalProbitModel Tsao = new ThermalProbitModel(1, "Tsao", "tsao");
        public static readonly ThermalProbitModel Tno = new ThermalProbitModel(2, "TNO", "tno");
        public static readonly ThermalProbitModel Lees = new ThermalProbitModel(3, "Lees", "lees");
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;

        private ThermalProbitModel(int value, string name, string key)
        {
            _name = name;
            _value = value;
            _key = key;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }
    }

    /// <summary>
    /// Representation of overpressure probit model, designated by string label which is consumed by python funcs.
    /// Determines overpressure harm calculations.
    /// Collapse assumes that the probability of collapse is also the probability of worker death.
    /// </summary>
    [Serializable]
    public sealed class OverpressureProbitModel
    {
        public static readonly OverpressureProbitModel LungEis =
            new OverpressureProbitModel(0, "Lung (Eisenberg)", "leis");

        public static readonly OverpressureProbitModel LungHse = new OverpressureProbitModel(1, "Lung (HSE)", "lhse");
        public static readonly OverpressureProbitModel Head = new OverpressureProbitModel(2, "Head Impact", "head");
        public static readonly OverpressureProbitModel Collapse = new OverpressureProbitModel(3, "Collapse", "coll");
        public static readonly OverpressureProbitModel Debris = new OverpressureProbitModel(4, "Debris", "debr");
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;

        private OverpressureProbitModel(int value, string name, string key)
        {
            _name = name;
            _value = value;
            _key = key;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }
    }

    /// <summary>
    /// Specification of deflagration.
    /// </summary>
    [Serializable]
    public class DeflagrationModel
    {
        public static readonly DeflagrationModel Bauwens = new DeflagrationModel(0, "Bauwens/Ekoto", "bauw");
        public static readonly DeflagrationModel Cfd = new DeflagrationModel(1, "CFD", "cfd");
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;

        public DeflagrationModel(int value, string name, string key)
        {
            _name = name;
            _value = value;
            _key = key;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }
    }

    /// <summary>
    /// Distribution to use for specified failure mode (e.g. runaway)
    /// </summary>
    [Serializable]
    public class FailureMode
    {
        public FailureMode(string name, string mode, FailureDistributionType dist, double paramA, double paramB)
        {
            Name = name;
            Mode = mode;
            Dist = dist;
            ParamA = paramA;
            ParamB = paramB;
        }
        public string Name { get; set; }
        public string Mode { get; set; }
        public FailureDistributionType Dist { get; set; }
        public double ParamA { get; set; }
        public double ParamB { get; set; }
    }

    /// <summary>
    /// Generic probability representation used by components, failure modes.
    /// </summary>
    [Serializable]
    public class ComponentProbability
    {
        public ComponentProbability(string leakSize, double mu, double sigma, double mean, double variance)
        {
            LeakSize = leakSize;
            Mu = mu;
            Sigma = sigma;
            Mean = mean;
            Variance = variance;
        }

        public string LeakSize { get; set; }
        public double? Mu { get; set; }
        public double? Sigma { get; set; }
        public double? Mean { get; set; }
        public double? Variance { get; set; }

        /// <summary>
        /// Compile probability params and return as list
        /// </summary>
        /// <returns></returns>
        public double[] GetDataForPython()
        {
            double[] data;
            if (Mu == null || Sigma == null)
                data = new[] {-1000D, -1000D, (double) Mean, (double) Variance};
            else
                data = new[] {(double) Mu, (double) Sigma, -1000D, -1000D};

            return data;
        }
    }
}