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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace SandiaNationalLaboratories.Hyram
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
    public class StateContainer
    {
        // TODO wishlist: The following updates could improve this storage class
        // - All param values are known so store them as properties on a DBTable object 
        // - Add suitable wrapper for setting min/max, unit conversions to reinforce MVC
        // - Centralize all external parameter edits to use single function/db entry point

        // Main store of parameter values. Initialized in InitDatabase.
        // Analysis parameters stored in mDatabase["PARAMETERS"]. Default values stored in mDatabase["DEFAULTS"]

        public string[] Distributions = {"Beta", "LogNormal", "Normal", "ExpectedValue", "Uniform"};

        // Custom event to listen for fuel type changes
        //public delegate void FuelTypeChangedDelegate(object sender, FuelType fuelType);

        // Event for when fuel type is changed
        [field:NonSerialized]
        public event EventHandler FuelTypeChangedEvent;

        public List<NozzleModel> NozzleModels = new List<NozzleModel>
        {
            NozzleModel.Birch,
            NozzleModel.Birch2,
            NozzleModel.EwanMoodie,
            NozzleModel.Molkov,
            NozzleModel.YuceilOtugen
            //NozzleModel.HarstadBellan
        };

        // Note that fuel type has custom event which is controlled in SetValue method below
        public List<FuelType> FuelTypes = new List<FuelType>
        {
            FuelType.H2, FuelType.CNG, FuelType.LNG
        };

        public List<FluidPhase> FluidPhases = new List<FluidPhase>
        {
            FluidPhase.GasDefault, FluidPhase.SatGas, FluidPhase.SatLiquid
        };

        public List<ThermalProbitModel> ThermalProbitModels = new List<ThermalProbitModel>
        {
            ThermalProbitModel.Eisenberg, ThermalProbitModel.Tsao, ThermalProbitModel.Tno, ThermalProbitModel.Lees
        };

        public List<OverpressureProbitModel> OverpressureProbitModels = new List<OverpressureProbitModel>
        {
            OverpressureProbitModel.LungEis, OverpressureProbitModel.LungHse, OverpressureProbitModel.Head,
            OverpressureProbitModel.Collapse  //  OverpressureProbitModel.Debris
        };

        // AppData/HyRAM dir for non-user-specific output
        //public static string AppDataDir =
        //    Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
        //        "HyRAM");

        // User AppData/HyRAM dir for user-specific output like logs, plots, etc.
        public static string UserDataDir =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "HyRAM");

        private const string ParamTableName = "PARAMETERS";
        private const string DefaultsTableName = "DEFAULTS";

        // Do not delete
        private bool _mGasAndFlameDetectionOn = true;

        private static StateContainer _mInstance = new StateContainer();

        private const string OccupantDistributionsCollectionKey = "OccupantDistributions";

        public PressureUnit CfdPressureUnit = PressureUnit.Pa;
        private ElapsingTimeConversionUnit _mExposureTimeUnit = ElapsingTimeConversionUnit.Second;

        private ParameterDatabase Database { get; set; }

        /// <summary>
        /// Get parameter set stored in Database
        /// </summary>
        public ParameterDatabase Parameters
        {
            get
            {
                ParameterDatabase result = null;
                lock (Synchronize.LockId)
                {
                    result = (ParameterDatabase) Database[ParamTableName];
                }

                return result;
            }
        }

        /// <summary>
        ///     Get default values of parameters, stored in Database
        /// </summary>
        public ParameterDatabase Defaults
        {
            get
            {
                ParameterDatabase result = null;
                lock (Synchronize.LockId)
                {
                    result = (ParameterDatabase) Database[DefaultsTableName];
                }

                return result;
            }
        }

        public static StateContainer Instance
        {
            get
            {
                StateContainer result = null;
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
            {
                Instance.Parameters[uKey] = newVal;

                // Trigger events, if any. Currently using events for FuelType selection on change
                if (uKey == "FUELTYPE")
                {
                    EventHandler handler = Instance.FuelTypeChangedEvent;
                    if (handler != null)
                    {
                        var e = EventArgs.Empty;
                        handler(Instance, e);
                    }
                }
            }
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
        public ConvertibleValue GetStateDefinedValueObject(string key)
        {
            // Debug.WriteLine($"DVO {key}");
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
        public ConvertibleValue GetStateDefinedValueObject(string key, out UnitOfMeasurementConverters converter)
        {
            ConvertibleValue
                result = null; // Will be checked on all cases, even first, to prevent bugs when code reordered
            var ukey = key.ToUpper();

            if (Parameters.ContainsKey(ukey)) result = (ConvertibleValue) Parameters[ukey];

            if (result == null)
                if (Defaults.ContainsKey(ukey))
                    result = (ConvertibleValue) Defaults[ukey];

            converter = result?.Converters;
            return result;

        }

        public void RefreshLeakFrequencyData(FuelType fuel = null)
        {
            // update params on Instance based on selected fuel
            if (fuel is null)
            {
                fuel = GetValue<FuelType>("FuelType");
            }
            ComponentProbabilitySet probabilities = FuelData.H2LeakFrequencies;

            if (fuel == FuelType.H2)
            {
                probabilities = FuelData.H2LeakFrequencies;
            }
            else if (fuel == FuelType.CNG)
            {
                probabilities = FuelData.CngLeakFrequencies;
            }
            else if (fuel == FuelType.LNG)
            {
                probabilities = FuelData.H2LeakFrequencies;
            }

            SetValue("Prob.Compressor", probabilities.Compressor);
            SetValue("Prob.Cylinder", probabilities.Cylinder);
            SetValue("Prob.Filter", probabilities.Filter);
            SetValue("Prob.Flange", probabilities.Flange);
            SetValue("Prob.Hose", probabilities.Hose);
            SetValue("Prob.Joint", probabilities.Joint);
            SetValue("Prob.Pipe", probabilities.Pipe);
            SetValue("Prob.Valve", probabilities.Valve);
            SetValue("Prob.Instrument", probabilities.Instrument);
            SetValue("Prob.Extra1", probabilities.Extra1);
            SetValue("Prob.Extra2", probabilities.Extra2);
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
        public void InitDatabase()
        {
            Database = new ParameterDatabase();

            Database[ParamTableName] = new ParameterDatabase();
            Database[DefaultsTableName] = new ParameterDatabase();

            Database[ParamTableName] = InitDefaults();
            RefreshLeakFrequencyData();

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
        /// TODO: No reason to have two sets. Get rid of Defaults ParameterDatabase obj.
        /// </summary>
        /// <returns></returns>
        private ParameterDatabase InitDefaults()
        {
            var database = new ParameterDatabase();

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
            database["pipeDiameter"] = new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Inch,
                new[] {0.375D}, 0D);
            // Pipe wall thickness [inches]
            database["pipeThickness"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Inch, new[] {0.065D}, 0D);

            database["ambientTemperature"] = new ConvertibleValue(StockConverters.TemperatureConverter,
                TempUnit.Kelvin, new[] {288D}, -273.14D);
            database["ambientPressure"] = new ConvertibleValue(StockConverters.PressureConverter,
                PressureUnit.MPa, new[] {.101325}, 0D);

            //System temperature [C]
            database["fluidTemperature"] = new ConvertibleValue(StockConverters.TemperatureConverter,
                TempUnit.Kelvin, new[] {287.8}, -273.14D);
            database["fluidPressure"] = new ConvertibleValue(StockConverters.PressureConverter,
                PressureUnit.MPa, new[] {35D}, 0D);

            database["exclusionRadius"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {.01});

            database["ImmedIgnitionProbs"] = new[] {0.008D, 0.053D, 0.23D};
            database["DelayIgnitionProbs"] = new[] {0.004D, 0.027D, 0.12D};
            database["IgnitionThresholds"] = new[] {0.125, 6.25};
            ;

            database["FuelType"] = FuelType.H2;
            database["ReleaseFluidPhase"] = FluidPhase.GasDefault;
            database["ThermalProbit"] = ThermalProbitModel.Eisenberg;
            database["OverpressureProbit"] = OverpressureProbitModel.Collapse;

            database["RadiativeSourceModel"] = RadiativeSourceModels.Multi;  // unused as of 3.1

            // Overpressure consequences
            database["OverpressureConsequences"] = new ConvertibleValue(GetConverterByDatabaseKey("OverpressureConsequences"), PressureUnit.Pa,
                new[] {2.5e3, 2.5e3, 5e3, 16e3, 30e3});
            database["Impulses"] = new ConvertibleValue(GetConverterByDatabaseKey("Impulses"), PressureUnit.Pa,
                new double[] {250, 500, 1000, 2000, 4000});

            //Nozzle model for gas jets
            database["NozzleModel"] = NozzleModel.YuceilOtugen;

            //Stores the last/default inputted unit for all stored variables
            database["VarUnitDict"] = new Dictionary<string, Enum>();

            database["OpWrapper.PlotDotsPressureAtTimes"] = new[]
            {
                new NdPressureAtTime(1, 13.8D),
                new NdPressureAtTime(15, 15.0D),
                new NdPressureAtTime(20, 55.2D)
            };

            // Optional overrides whereby user can control release for each size. -1 to ignore it.
            database["H2Release.000d01"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.000d10"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.001d00"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.010d00"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["H2Release.100d00"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);
            database["Pdetectisolate"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {.9D}, 0D, 1D);
            database["Failure.ManualOverride"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {-1D}, -1D, 1D);

            database["numCompressors"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["numCylinders"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["numValves"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {5}, 0D);
            database["numInstruments"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {3}, 0D);
            database["numJoints"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {35}, 0D);
            database["numHoses"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {1}, 0D);
            database["pipeLength"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new double[] {20}, 0D);
            database["numFlanges"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["numFilters"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["numExtraComponent1"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);
            database["numExtraComponent2"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new double[] {0}, 0D);

            database["facilityLength"] = new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter,
                new double[] {20}, 0D);
            database["facilityWidth"] = new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter,
                new double[] {12}, 0D);
            database["facilityHeight"] = new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter,
                new double[] {5}, 0D);

            // Discharge coefficient of the hole; 1.0 is a conservative value, 0.6 is suggested value for screening
            database["DischargeCoefficient"] = new ConvertibleValue(
                    GetConverterByDatabaseKey("DischargeCoefficient"),
                    UnitlessUnit.Unitless, new[] {1D}, 0D);
            database["numVehicles"] = new ConvertibleValue(
                    GetConverterByDatabaseKey("numVehicles"),
                UnitlessUnit.Unitless, new double[] {20}, 0D);
            database["dailyFuelings"] = new ConvertibleValue(
                GetConverterByDatabaseKey("dailyFuelings"),
                UnitlessUnit.Unitless, new double[] {2}, 0D);
            database["annualDemand"] = new ConvertibleValue(
                    GetConverterByDatabaseKey("annualDemand"),
                    UnitlessUnit.Unitless, new[] {10000D}, 0D);
            database["vehicleOperatingDays"] = new ConvertibleValue(
                GetConverterByDatabaseKey("vehicleOperatingDays"),
                UnitlessUnit.Unitless, new[] {250D}, 0D, 366D);

            database["LeakHeight"] = new ConvertibleValue(
                    StockConverters.DistanceConverter,
                    DistanceUnit.Meter, new[] {0.0D}, 0D);  // lacks input field; used by QRA
            database["ReleaseAngle"] = new ConvertibleValue(
                    StockConverters.AngleConverter,
                    AngleUnit.Degrees, new[] {0.0D}, 0D, 180.0D);

            database["Failure.NozPO"] = new FailureMode("Nozzle", "Pop-off", FailureDistributionType.Beta, 0.5D, 610415.5D);
            //Database["Failure.NozFTC"] = new FailureMode("Nozzle", "Failure to close", FailureDistributionType.Beta, 31.5D, 610384.5D);
            database["Failure.NozFTC"] = new FailureMode("Nozzle", "Failure to close",
                FailureDistributionType.ExpectedValue, 0.002D, 0D);
            database["Failure.MValveFTC"] = new FailureMode("Manual valve", "Failure to close",
                FailureDistributionType.ExpectedValue, 0.001D, 0D);
            database["Failure.SValveFTC"] = new FailureMode("Solenoid valve", "Failure to close",
                FailureDistributionType.ExpectedValue, 0.002D, 0D);
            database["Failure.SValveCCF"] = new FailureMode("Solenoid valve", "Common-cause failure",
                FailureDistributionType.ExpectedValue, 0.000128D, 0D);

            database["Failure.Overp"] = new FailureMode("Overpressure during fueling", "Accident",
                FailureDistributionType.Beta, 3.5D, 310289.5D);
            database["Failure.PValveFTO"] = new FailureMode("Pressure relief valve", "Failure to open",
                FailureDistributionType.LogNormal, -11.74D, 0.67D);
            database["Failure.Driveoff"] =
                new FailureMode("Driveoff", "Accident", FailureDistributionType.Beta, 31.5D, 610384.5D);
            database["Failure.CouplingFTC"] = new FailureMode("Breakaway coupling", "Failure to close",
                FailureDistributionType.Beta, 0.5D, 5031.0D);

            // Component leak probabilities stored in lists of objects for easy data-binding.
            // Note that func above will refresh these depending on selected fuel. (They're refreshed after initialization.)
            // Array elements are leak size, Mu, Sigma, Mean, Variance, in order.
            database["Prob.Compressor"] = FuelData.H2LeakFrequencies.Compressor;
            database["Prob.Cylinder"] = FuelData.H2LeakFrequencies.Cylinder;
            database["Prob.Filter"] = FuelData.H2LeakFrequencies.Filter;
            database["Prob.Flange"] = FuelData.H2LeakFrequencies.Flange;
            database["Prob.Hose"] = FuelData.H2LeakFrequencies.Hose;
            database["Prob.Joint"] = FuelData.H2LeakFrequencies.Joint;
            database["Prob.Pipe"] = FuelData.H2LeakFrequencies.Pipe;
            database["Prob.Valve"] = FuelData.H2LeakFrequencies.Valve;
            database["Prob.Instrument"] = FuelData.H2LeakFrequencies.Instrument;
            database["Prob.Extra1"] = FuelData.H2LeakFrequencies.Extra1;
            database["Prob.Extra2"] = FuelData.H2LeakFrequencies.Extra2;

            database["tankVolume"] = new ConvertibleValue(
                    StockConverters.VolumeConverter,
                    VolumeUnit.CubicMeter, new[] {3.63 / 1000});
            database["orificeDischargeCoefficient"] = new ConvertibleValue(
                    StockConverters.UnitlessConverter,
                    UnitlessUnit.Unitless, new[] {1D});

            database["releaseDischargeCoefficient"] = new ConvertibleValue(
                    StockConverters.UnitlessConverter,
                    UnitlessUnit.Unitless, new[] {1D});
            database["releaseToWallDistance"] = new ConvertibleValue(
                    StockConverters.DistanceConverter,
                    DistanceUnit.Meter, new[] {2.1255D});
            database["ceilingVentArea"] = new ConvertibleValue(StockConverters.AreaConverter,
                AreaUnit.SqMeters, new[] {Math.Pow(0.34, 2) * Math.PI / 4});
            // Floor vent (area?)
            database["floorVentArea"] = new ConvertibleValue(StockConverters.AreaConverter,
                AreaUnit.SqMeters, new[] {0.12 * 0.0635});
            database["enclosureHeight"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {2.72});
            // Speed of wind
            database["ventVolumetricFlowRate"] = new ConvertibleValue(StockConverters.VolumetricFlowConverter,
                VolumetricFlowUnit.CubicMetersPerSecond,
                new[] {0D});
            database["releaseArea"] = new ConvertibleValue(StockConverters.AreaConverter, AreaUnit.SqMeters, new[] {0.01716});
            database["floorCeilingArea"] = new ConvertibleValue(StockConverters.AreaConverter, AreaUnit.SqMeters, new[] {16.72216D});
            database["ceilingVentHeight"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {2.42D});
            database["floorVentHeight"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {0.044});

            database["OpWrapper.SecondsToPlot"] = new ConvertibleValue(
                GetConverterByDatabaseKey("OpWrapper.SecondsToPlot"), ElapsingTimeConversionUnit.Second,
                new[]
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27,
                    28, 29, 29.5
                });

            database["OpWrapper.LimitLinePressures"] = new ConvertibleValue(
                GetConverterByDatabaseKey("OpWrapper.LimitLinePressures"), PressureUnit.kPa,
                new[] {13.8D, 15.0D, 55.2D});

            // Fill Plume wrapper defaults
            // co-volume constant (default is 7.6921e-3 m^3/kg, valid for H2)
            database["PlumeWrapper.PlotTitle"] = "Mole Fraction of Leak";
            database["PlumeWrapper.XMin"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {-2.5D});
            database["PlumeWrapper.XMax"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {2.5D});
            database["PlumeWrapper.YMin"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {0D});
            database["PlumeWrapper.YMax"] = new ConvertibleValue(StockConverters.DistanceConverter,
                DistanceUnit.Meter, new[] {10D});
            database["PlumeWrapper.Contours"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {0.04});

            database["PlumeWrapper.co_volume_constant"] = new ConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.co_volume_constant"), DensityUnit.KilogramPerCubicMeter,
                new[] {7.6921e-3});
            database["PlumeWrapper.distance_to_wall"] = new ConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.distance_to_wall"), DistanceUnit.Meter, new[] {100D});
            database["PlumeWrapper.jet_angle"] = new ConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.jet_angle"), AngleUnit.Radians, new[] {1.5708D});
            database["PlumeWrapper.FlameBoundary"] = new ConvertibleValue(
                GetConverterByDatabaseKey("PlumeWrapper.FlameBoundary"), UnitlessUnit.Unitless,
                new[] {0.04D, 0.08D});

            // Fill QRA enclosure upper and lower vent defaults
            database["Enclosure.Height"] = new ConvertibleValue(GetConverterByDatabaseKey("Enclosure.Height"),
                DistanceUnit.Meter, new[] {2D});
            database["Enclosure.AreaOfFloorAndCeiling"] = new ConvertibleValue(
                GetConverterByDatabaseKey("Enclosure.AreaOfFloorAndCeiling"), AreaUnit.SqMeters, new[] {10D});
            database["Enclosure.HeightOfRelease"] = new ConvertibleValue(
                GetConverterByDatabaseKey("Enclosure.HeightOfRelease"), DistanceUnit.Meter, new[] {0D});
            database["Enclosure.XWall"] = new ConvertibleValue(GetConverterByDatabaseKey("Enclosure.XWall"),
                DistanceUnit.Meter, new[] {3D});

#if false
            // Vent inputs
            foreach (WhichVent ventPurpose in Enum.GetValues(typeof(WhichVent)))
            {
                var crossSectionalAreaKey = ventPurpose + ".CrossSectionalArea";
                var heightFromFloorKey = ventPurpose + ".VentHeightFromFloor";
                var dischargeCoefficientKey = ventPurpose + ".DischargeCoefficient";
                var windVelocityKey = ventPurpose + ".WindVelocity";

                database[crossSectionalAreaKey] = new ConvertibleValue( StockConverters.AreaConverter, AreaUnit.SqMeters, new[] {.1D});
                database[dischargeCoefficientKey] = new ConvertibleValue( StockConverters.UnitlessConverter, UnitlessUnit.Unitless, new[] {1D});
                database[windVelocityKey] = new ConvertibleValue(StockConverters.SpeedConverter, SpeedUnit.MetersPerSecond, new[] {1D});

                var heightFromFloorDefault = (ventPurpose == WhichVent.Ceiling) ? 2.5D : 0D;
                database[heightFromFloorKey] = new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {heightFromFloorDefault});
            }
#endif

            database["maxSimTime"] = new ConvertibleValue(StockConverters.ElapsingTimeConverter, ElapsingTimeConversionUnit.Second, new[] { 30D });
            database["releaseAngle"] = new ConvertibleValue(StockConverters.AngleConverter, AngleUnit.Radians, new[] {0D});

            // Person's flame exposure time [s] 
            database["flameExposureTime"] = new ConvertibleValue(StockConverters.ElapsingTimeConverter,
                ElapsingTimeConversionUnit.Second, new[] {60D}, 0D);

            database["relativeHumidity"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                UnitlessUnit.Unitless, new[] {0.89D}, 0D);
            database["orificeDiameter"] = new ConvertibleValue(
                StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {0.00356}, 0D);

            database["releaseHeight"] = new ConvertibleValue( StockConverters.DistanceConverter,
                DistanceUnit.Meter, new double[] {0}, 0D);

            database["FlameWrapper.radiative_heat_flux_point:x"] = new ConvertibleValue( StockConverters.DistanceConverter, DistanceUnit.Meter,
                new[] {0.01D, 0.5D, 1D, 2D, 2.5D, 5D, 10D, 15D, 25D, 40D});
            database["FlameWrapper.radiative_heat_flux_point:y"] = new ConvertibleValue( StockConverters.DistanceConverter, DistanceUnit.Meter,
                new[] {1D, 1D, 1D, 1D, 1D, 2D, 2D, 2D, 2D, 2D});
            database["FlameWrapper.radiative_heat_flux_point:z"] = new ConvertibleValue( StockConverters.DistanceConverter, DistanceUnit.Meter,
                new[] {0.01D, 0.5D, 0.5D, 1D, 1D, 1D, 0.5D, 0.5D, 1D, 2D});
            database["FlameWrapper.contour_levels"] = new ConvertibleValue( StockConverters.UnitlessConverter, UnitlessUnit.Unitless,
                new[] {1.577, 4.732, 25.237}, 0.0);

            // Defaults to arbitrary value - in this case, the number of steps the Glenn Frey statue unveiled 9/23/2016 would have
            // take to walk from Standin' on a Corner Park in Winslow Arizona to the Rock And Roll Hall of Fame in Cincinnatti, OH.
            database["randomSeed"] = new ConvertibleValue(StockConverters.UnitlessConverter, UnitlessUnit.Unitless, new double[] {3632850});

            database["Dist.LeakScenarios"] = new ConvertibleValue(StockConverters.UnitlessConverter,
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

            // TODO: update db field names and this lookup to use consistent naming conventions.
            // Avoid forced uppercase so that search will find names.
            // Updating the db names will likely break saves.
            switch (ucKey)
            {
                case "T_EXPOSE_THERMAL":
                case "OPWRAPPER.SECONDSTOPLOT":
                case "OPWRAPPER.MAXSIMTIME":
                case "MAXSIMTIME":
                case "FLAMEEXPOSURETIME":
                    result = StockConverters.ElapsingTimeConverter;
                    break;
                case "FLAMEWRAPPER.T_H2":
                case "SYSPARAM.EXTERNALTEMPC":
                case "SYSPARAM.INTERNALTEMPC":
                case "AMBIENTTEMPERATURE":
                case "FLUIDTEMPERATURE":
                    result = StockConverters.TemperatureConverter;
                    break;
                case "FLAMEWRAPPER.P_H2":
                case "OVERPRESSURECONSEQUENCES":
                case "IMPULSES":
                case "OPWRAPPER.LIMITLINEPRESSURES":
                case "SYSPARAM.EXTERNALPRESMPA":
                case "SYSPARAM.INTERNALPRESMPA":
                case "AMBIENTPRESSURE":
                case "FLUIDPRESSURE":
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
                case "PLUMEWRAPPER.DISTANCE_TO_WALL":
                case "ORIFICEDIAMETER":
                case "PIPELENGTH":
                case "PIPEDIAMETER":
                case "PIPETHICKNESS":
                case "FACILITYLENGTH":
                case "FACILITYWIDTH":
                case "FACILITYHEIGHT":
                case "OPWRAPPER.S0":
                case "OPWRAPPER.XWALL":
                case "OPWRAPPER.HV":
                case "OPWRAPPER.H":
                case "OPWRAPPER.CVHF":
                case "OPWRAPPER.FVHF":
                case "SYSPARAM.PIPEOD":
                case "SYSPARAM.PIPEWALLTHICK":
                case "FLOOR.VENTHEIGHTFROMFLOOR":
                case "CEILING.VENTHEIGHTFROMFLOOR":
                case "RELEASETOWALLDISTANCE":
                case "RELEASEHEIGHT":
                case "ENCLOSUREHEIGHT":
                case "LEAKHEIGHT":
                case "CEILINGVENTHEIGHT":
                case "FLOORVENTHEIGHT":
                // used in QRA
                case "ENCLOSURE.HEIGHT":
                case "ENCLOSURE.HEIGHTOFRELEASE":
                case "ENCLOSURE.XWALL":
                    result = StockConverters.DistanceConverter;
                    break;
                case "OPWRAPPER.TANKVOLUME":
                case "TANKVOLUME":
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
                case "CEILINGVENTAREA":
                case "FLOORVENTAREA":
                case "RELEASEAREA":
                case "FLOORCEILINGAREA":
                    result = StockConverters.AreaConverter;
                    break;
                case "OPWRAPPER.VOLUMEFLOWRATE":
                case "VENTVOLUMETRICFLOWRATE":
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
        public static StateContainer Load(string filename)
        {
            SetValue("ResultsAreStale", true);
            SetValue<object>("Result", null);
            var result = Deserialize(filename);
            result.RefreshLeakFrequencyData();
            return result;
        }

        /// <summary>
        /// Write state to file
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="objectToSerialize"></param>
        public static void Serialize(string filename, StateContainer objectToSerialize)
        {
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
        public static StateContainer Deserialize(string filename)
        {
            StateContainer result = null;

            var fs =
                new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                var formatter = new BinaryFormatter();
                var oResult = formatter.Deserialize(fs);
                var typeName = oResult.GetType().ToString().ToUpper();
                result = (StateContainer) oResult;
                var savedParams = (ParameterDatabase) result.Database[ParamTableName];

                //Backward compatibility fix for changing the unit type on vehicleOperatingDays
                if (result.GetStateDefinedValueObject("vehicleOperatingDays").InputUnit is JulianTimeConversionUnit)
                    savedParams["vehicleOperatingDays"] = new ConvertibleValue(StockConverters.UnitlessConverter,
                        UnitlessUnit.Unitless,
                        result.GetStateDefinedValueObject("vehicleOperatingDays")
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
                    if (userValue is ConvertibleValue)
                    {
                        var value = userValue as ConvertibleValue;
                        if (value.MaxValue == 0 && value.MinValue == 0 && result.Defaults.ContainsKey(key))
                        {
                            value.MaxValue = ((ConvertibleValue) result.Defaults[key]).MaxValue;
                            value.MinValue = ((ConvertibleValue) result.Defaults[key]).MinValue;
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
                if (thisItem is ParameterDatabase propsItem)
                    foreach (var propsItemKey in propsItem.Keys)
                    {
                        var thisValue = propsItem[propsItemKey];
                        if (thisValue is ConvertibleValue cv)
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
            // Some things in the QRA StateContainer container are dependent upon the local system.
            // Make sure that these things are reset back to their correct and local values.
            ResetUnitConverters();
        }

        private void ResetUnitConverters()
        {
            // Note (Cianan): not sure when this is applicable or when a converter is broken, per John's comment below.
            //
            //Make sure the latest version of a converter is being used after load.
            //clsProperties[] Databases = new clsProperties[] { UserSessionValues, Defaults, Constants };
            ParameterDatabase[] databases = {Parameters, Defaults};
            foreach (var thisDatabase in databases)
            foreach (var itemKey in thisDatabase.Keys)
            {
                var oValueNode = thisDatabase[itemKey];
                if (oValueNode is ConvertibleValue valueNode)
                    valueNode.Converters = GetConverterByDatabaseKey(itemKey);
            }
        }

        public FluidPhase GetFluidPhase()
        {
            return GetValue<FluidPhase>("ReleaseFluidPhase");
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


        public static bool FuelTypeIsGaseous()
        {
            FuelType selectedFuel = GetValue<FuelType>("FuelType");
            bool isGaseous = (selectedFuel == FuelType.H2 || selectedFuel == FuelType.CNG);
            //Debug.WriteLine($"Fuel {selectedFuel} gaseous? {isGaseous}");
            return isGaseous;
        }


        public static bool FuelPhaseIsSaturated()
        {
            FluidPhase phase = Instance.GetFluidPhase();
            return (phase == FluidPhase.SatLiquid || phase == FluidPhase.SatGas);
        }


        /// <summary>
        /// Checks whether fuel release pressure is within valid range
        /// </summary>
        /// <returns></returns>
        public static bool ReleasePressureIsValid(string varName)
        {
            bool result = true;
            FluidPhase phase = Instance.GetFluidPhase();
            if (phase == FluidPhase.SatLiquid || phase == FluidPhase.SatGas)
            {
                double relPres = GetNdValue(varName, PressureUnit.Atm);
                double ambPres = GetNdValue("ambientPressure", PressureUnit.Atm);
                double critPres = 12.73131;
                if (relPres <= ambPres || relPres > critPres)
                {
                    result = false;
                }
            }

            return result;
        }
        public static bool ReleasePressureIsValid()
        {
            return ReleasePressureIsValid("fluidPressure");
        }

        public static bool FuelFlowUnchoked()
        {
            var ambPres= GetNdValue("ambientPressure", PressureUnit.Pa);
            var fuelPres = GetNdValue("fluidPressure", PressureUnit.Pa);
            var selectedFuel = GetValue<FuelType>("FuelType");
            return (fuelPres < (selectedFuel.GetRatio() * ambPres));
        }

    }

}
