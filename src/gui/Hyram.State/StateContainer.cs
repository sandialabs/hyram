/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SandiaNationalLaboratories.Hyram
{
    public enum AlertLevel
    {
        AlertNull,
        AlertWarning,
        AlertError,
        AlertInfo,
    }

    /// <summary>
    /// Represents all selectable fuel options, including blend presets. Used in FuelForm in advanced fuel selector.
    /// Actual molar concentrations are stored in Fuel objects within StateContainer.
    /// </summary>
    public enum SpeciesOptions
    {
        EFuelH2,
        EFuelMethane,
        EFuelPropane,
        EFuelBlendEmpty,
        EFuelBlendNist1,
        EFuelBlendNist2,
        EFuelBlendRg2,
        EFuelBlendGu1,
        EFuelBlendGu2,
    }

    public class State
    {
        public static StateContainer Data = new StateContainer();
    }

    /// <summary>
    /// Database-like object stores parameter values used in all analyses.
    /// The backbone data model of the GUI.
    /// </summary>
    public class StateContainer
    {
        public string Comments = "";
        [JsonIgnore]
        public object QraResult;

        private bool _changeConcentrationSilently = false;

        public OccupantDistributionInfoCollection OccupantInfo = new OccupantDistributionInfoCollection(true);

        // Fuel objects store concentrations and species-specific properties. Rename to SpeciesType?
        public FuelType FuelH2 = new FuelType(0, "Hydrogen", "h2", 1.899, 1296400, 1.0, 0.04, true),
                        FuelMethane = new FuelType(1, "Methane", "ch4", 1.839, 4599200, 0, 0.05),
                        FuelPropane = new FuelType(2, "Propane", "c3h8", 1.719, 4251200, 0, 0.021),
                        FuelN2 = new FuelType(3, "Nitrogen", "n2", 1.889, 0, 0),
                        FuelCo2 = new FuelType(4, "Carbon Dioxide", "co2", 1.829, 0, 0),
                        FuelEthane = new FuelType(5, "Ethane", "c2h6", 1.769, 0, 0, 0.03),
                        FuelNButane = new FuelType(6, "n-Butane", "n-c4h10", 1.689, 0, 0, 0.018),
                        FuelIsoButane = new FuelType(7, "Isobutane", "ISOBUTANE", 1.689, 0, 0, 0.018),
                        FuelNPentane = new FuelType(8, "n-Pentane", "n-c5h12", 0, 0, 0, 0.014),
                        FuelIsoPentane = new FuelType(9, "Isopentane", "ISOPENTANE", 0, 0, 0, 0.014),
                        FuelNHexane = new FuelType(10, "n-Hexane", "n-c6h14", 0, 0, 0, 0.012),

                        FuelBlend = new FuelType(99, "Blend", "blend", -1.0, -1.0, 0);
        [JsonIgnore]
        public List<FuelType> Fuels = new List<FuelType>();

        // Track dropdown selection in fuel form, which includes all possible species.
        public SpeciesOptions SpeciesOption = SpeciesOptions.EFuelH2;

        // Fuel phases
        [JsonIgnore]
        public ModelPair GasDefault = new ModelPair("Fluid", "none"),
                         SatGas = new ModelPair("Saturated vapor", "gas"),
                         SatLiquid = new ModelPair("Saturated liquid", "liquid"),

                         // Notional nozzle options
                         NozzleBirch = new ModelPair("Birch", "birc"),
                         NozzleBirch2 = new ModelPair("Birch2", "bir2"),
                         NozzleEwanMoodie = new ModelPair("Ewan/Moodie", "ewan"),
                         NozzleMolkov = new ModelPair("Molkov", "molk"),
                         NozzleYuceilOtugen = new ModelPair("Yuceil/Otugen", "yuce"),
                         BstMethod = new ModelPair("BST", "bst"),
                         TntMethod = new ModelPair("TNT", "tnt"),
                         BauwensMethod = new ModelPair("Bauwens", "bauwens"),

                         // Overpressure probit options
                         ProbitLungEis = new ModelPair("Lung (Eisenberg)", "leis"),
                         ProbitLungHse = new ModelPair("Lung (HSE)", "lhse"),
                         ProbitHead = new ModelPair("Head Impact", "head"),
                         ProbitCollapse = new ModelPair("Collapse", "coll"),

                         // Thermal probit options
                         ThermalEisenberg = new ModelPair("Eisenberg", "eise"),
                         ThermalTsao = new ModelPair("Tsao", "tsao"),
                         ThermalTno = new ModelPair("TNO", "tno"),
                         ThermalLees = new ModelPair("Lees", "lees")
                         ;

        [JsonIgnore]
        public List<ModelPair> Phases = new List<ModelPair>(),
                               NozzleModels = new List<ModelPair>(),
                               OverpressureMethods = new List<ModelPair>(),
                               OverpressureProbitModels = new List<ModelPair>(),
                               ThermalProbitModels = new List<ModelPair>(),
                               SensitivityDistributions = new List<ModelPair>(),
                               UncertaintyTypes = new List<ModelPair>();

        private ModelPair _phase;
        public ModelPair Phase
        {
            get => _phase;
            set
            {
                _phase = value;
                RefreshLeakFrequencyData();
                RefreshComponentQuantities();
                FuelPhaseEvent?.Invoke(this, EventArgs.Empty);
            }
        }


        [field:NonSerialized]
        // Unlike fuel concentration event, this only fires when entire fuel is changed (e.g. methane -> hydrogen)
        public event EventHandler FuelTypeChangedEvent;

        public event EventHandler FuelPhaseEvent;

        [JsonIgnore]
        public double FuelPrecision = 1E-5;

        // Index into these with Alert enum
        [field: NonSerialized]
        [JsonIgnore]
        public List<Color> AlertBackColors = new List<Color>{Color.LightCyan, Color.Cornsilk, Color.MistyRose};
        [field: NonSerialized]
        [JsonIgnore]
        public List<Color> AlertTextColors = new List<Color>{Color.PaleTurquoise, Color.DarkGoldenrod, Color.Maroon};

        // User AppData/HyRAM dir for user-specific output like logs, plots, etc.
        public static string UserDataDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                                                        "HyRAM");


        // // // // // // // // // //
        // SHARED QRA & PHYSICS PARAMETERS
        public ModelPair Nozzle;
        public ModelPair OverpressureMethod;

        public Parameter AmbientTemperature = new Parameter(Converters.Temperature, 288, 
                                                            "Ambient temperature", -273.14);
        public Parameter AmbientPressure = new Parameter(Converters.Pressure, PressureUnit.MPa, .101325, 
                                                        "Ambient pressure", 0);

        public Parameter FluidTemperature = new Parameter(Converters.Temperature, 287.8, 
                                                            "Tank fluid temperature", -273.14);

        public Parameter FluidPressure = new Parameter(Converters.Pressure, PressureUnit.MPa, 35, 
                                                    "Tank fluid pressure", minimum:0,
                                                    canBeUncertain:true);

        public bool PressureIsAbsolute = true;
        public Parameter OrificeDischargeCoefficient = new Parameter(1, "Discharge coefficient", 0, 1);

        [field: NonSerialized]
        [JsonIgnore]
        public List<double> MachFlameSpeeds = new List<double> { 0.2D, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2D };

        public double OverpressureFlameSpeed = 0.35;
        public Parameter TntEquivalenceFactor = new Parameter(0.03D, "TNT equivalence factor");


        // // // // // // // // // //
        // SHARED PHYSICS-ONLY
        public Parameter ReleaseAngle = new Parameter(Converters.Angle, AngleUnit.Degrees, 0, 
                                                      "Release angle", 0, 180.0);
        public Parameter OrificeDiameter = new Parameter(Converters.Distance, 0.00356, "Leak diameter", 0);
        public double? FluidMassFlow = null;


        // // // // // // // // // //
        // PLUME FORM
        public string PlumePlotTitle = "Mole Fraction of Leak";
        public bool PlumePlotAutoLimits = true;
        public Parameter PlumeXMin = new Parameter(Converters.Distance, -2.5, label:"X min", maybeNull: true);
        public Parameter PlumeXMax = new Parameter(Converters.Distance, 2.5, "X max", maybeNull: true);
        public Parameter PlumeYMin = new Parameter(Converters.Distance, 0, "Y min", maybeNull: true);
        public Parameter PlumeYMax = new Parameter(Converters.Distance, 10, "Y max", maybeNull: true);
//        public Parameter PlumeContours = new Parameter(0.04, "Contours (mole fraction)", 0.00001, 0.99);
        public double[] PlumeContourLevels = {};
        public Parameter PlumeReleaseAngle = new Parameter(Converters.Angle, AngleUnit.Radians, 1.5708D, "Angle of jet");

        public Parameter PlumeVMin = new Parameter(0, "Mole fraction scale minimum", 0, 1);
        public Parameter PlumeVMax = new Parameter(0.1, "Mole fraction scale maximum", 0, 1);


        // // // // // // // // // //
        // ACCUMULATION FORM
        public Parameter TankVolume = new Parameter(Converters.Volume, 3.63 / 1000, "Tank volume");
        public Parameter EnclosureHeight = new Parameter(Converters.Distance, 2.72, "Enclosure height");
        public Parameter FloorCeilingArea = new Parameter(Converters.Area, 16.72216, "Floor/ceiling area");
        public Parameter ReleaseToWallDistance = new Parameter(Converters.Distance,  2.1255, 
                                                               "Distance from release to wall");
        public Parameter CeilingVentArea = new Parameter(Converters.Area, Math.Round(Math.Pow(0.34, 2) * Math.PI / 4, 6), 
                                                    "Vent 1 (ceiling vent) cross-sectional area");
        public Parameter CeilingVentHeight = new Parameter(Converters.Distance, 2.42, 
                                                          "Vent 1 (ceiling vent) height from floor");

        public Parameter FloorVentArea = new Parameter(Converters.Area, 0.12 * 0.0635, 
                                                       "Vent 2 (floor vent) cross-sectional area");
        public Parameter FloorVentHeight = new Parameter(Converters.Distance, 0.044, 
                                                        "Vent 2 (floor vent) height from floor");
        public Parameter ReleaseHeight = new Parameter(Converters.Distance, 0, "Release height", 0);
        // Speed of wind
        public Parameter VentFlowRate = new Parameter(Converters.VolumetricFlow, 0, "Vent volumetric flow rate");

        // Time values are stored unitless and converted to seconds prior to analysis, based on selected time unit.
        public List<(double, double)> OpTimePressures = new List<(double, double)>{(1, 13.8), (15, 15), (20, 55.2)};
        public TimeUnit AccumulationTimeUnit { get; set; } = TimeUnit.Second;
        public Parameter MaxSimTime = new Parameter(30 );
        // units of these times vary based on selected input. Converted to seconds at execution time.
        public double[] AccumulationPlotTimes = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10,
                                                    11, 12, 13, 14, 15, 16, 17, 18, 19, 20,
                                                    21, 22, 23, 24, 25, 26, 27, 28, 29, 29.5 };

        public double[] AccumulationPlotPressures = {13.8D, 15.0D, 55.2D};  // always kPa

        public bool AccumPlotAutoLimits = true;  // unused - backend does not accept limits
        public Parameter AccumPresXMin = new Parameter(Converters.ElapsingTime, -2, "Pressure plot X min (time)", maybeNull:true);
        public Parameter AccumPresXMax = new Parameter(Converters.ElapsingTime, 32, "Pressure plot X max (time)", maybeNull:true);
        public Parameter AccumPresYMin = new Parameter(Converters.Pressure, PressureUnit.kPa, -3, "Pressure plot Y min", maybeNull:true);
        public Parameter AccumPresYMax = new Parameter(Converters.Pressure, PressureUnit.kPa, 75, "Pressure plot Y max", maybeNull:true);


        // // // // // // // // // //
        // FLAME TEMP FORM
        public string FlamePlotTitle = "";
        public bool FlameTempAutoLimits = true;
        public Parameter FlameXMin = new Parameter(Converters.Distance, 0, "Temperature plot X min", maybeNull:true);
        public Parameter FlameXMax = new Parameter(Converters.Distance, 9, "Temperature plot X max", maybeNull:true);
        public Parameter FlameYMin = new Parameter(Converters.Distance, -3.5, "Temperature plot Y min", maybeNull:true);
        public Parameter FlameYMax = new Parameter(Converters.Distance, 3.5, "Temperature plot Y max", maybeNull:true);
        public double[] TempContourLevels = {};


        // // // // // // // // // //
        // FLAME HEAT FLUX FORM
        public bool FlameFluxAutoLimits = true;
        public Parameter FluxXMin = new Parameter(Converters.Distance, -5, "Heat flux plot X min", maybeNull:true);
        public Parameter FluxXMax = new Parameter(Converters.Distance, 15, "Heat flux plot X max", maybeNull:true);
        public Parameter FluxYMin = new Parameter(Converters.Distance, -10, "Heat flux plot Y min", maybeNull:true);
        public Parameter FluxYMax = new Parameter(Converters.Distance, 10, "Heat flux plot Y max", maybeNull:true);
        public Parameter FluxZMin = new Parameter(Converters.Distance, -10, "Heat flux plot Z min", maybeNull:true);
        public Parameter FluxZMax = new Parameter(Converters.Distance, 10, "Heat flux plot Z max", maybeNull:true);

        public double[] RadiativeFluxX = { 0.01D, 0.5D, 1D, 2D, 2.5D, 5D, 10D, 15D, 25D, 40D};
        public double[] RadiativeFluxY = { 1D, 1D, 1D, 1D, 1D, 2D, 2D, 2D, 2D, 2D};
        public double[] RadiativeFluxZ = { 0.01D, 0.5D, 0.5D, 1D, 1D, 1D, 0.5D, 0.5D, 1D, 2D};
        public double[] FlameContourLevels = { 1.577, 4.732, 25.237};
        public Parameter RelativeHumidity = new Parameter(0.89D, "Relative humidity", 0D);


        // // // // // // // // // //
        // UNCONFINED OVERPRESSURE
        public bool OverpAutoLimits = true;
        public double[] OverpressureX = { 1, 2 };
        public double[] OverpressureY = { 0, 0 };
        public double[] OverpressureZ = { 1, 2};
        public double[] OverpressureContours = { 5, 16, 70 };  // always kPa
        public double[] ImpulseContours = { 0.13, 0.18, 0.27 };  // always kPa*s
        public Parameter OverpXMin = new Parameter(Converters.Distance, -35, "Overpressure plot X min", maybeNull:true);
        public Parameter OverpXMax = new Parameter(Converters.Distance, 35, "Overpressure plot X max", maybeNull:true);
        public Parameter OverpYMin = new Parameter(Converters.Distance, 0, "Overpressure plot Y min", maybeNull:true);
        public Parameter OverpYMax = new Parameter(Converters.Distance, 35, "Overpressure plot Y max", maybeNull:true);
        public Parameter OverpZMin = new Parameter(Converters.Distance, -35, "Overpressure plot Z min", maybeNull:true);
        public Parameter OverpZMax = new Parameter(Converters.Distance, 35, "Overpressure plot Z max", maybeNull:true);
        public Parameter ImpulseXMin = new Parameter(Converters.Distance, -2, "Impulse plot X min", maybeNull:true);
        public Parameter ImpulseXMax = new Parameter(Converters.Distance, 4.5, "Impulse plot X max", maybeNull:true);
        public Parameter ImpulseYMin = new Parameter(Converters.Distance, 0, "Impulse plot Y min", maybeNull:true);
        public Parameter ImpulseYMax = new Parameter(Converters.Distance, 3, "Impulse plot Y max", maybeNull:true);
        public Parameter ImpulseZMin = new Parameter(Converters.Distance, -3, "Impulse plot Z min", maybeNull:true);
        public Parameter ImpulseZMax = new Parameter(Converters.Distance, 3, "Impulse plot Z max", maybeNull:true);


        // // // // // // // // // //
        // CRYOGENIC POOLING FORM
        public Parameter SurfaceThermCond = new Parameter(1.03, "Surface thermal conductivity (W/m-K)", 0D);
        public Parameter SurfaceThermDiff = new Parameter(5e-6, "Surface thermal diffusivity (m^2/s)", 0D);
        public Parameter InflowRadius = new Parameter(Converters.Distance, 0.0015, "Inflow radius");
        public Parameter SurfaceTemp = new Parameter(Converters.Temperature, 295, "Surface temperature");
        public Parameter SimTime = new Parameter(Converters.ElapsingTime, 1000, "Simulation time");
        public Parameter PoolMassFlow = new Parameter(Converters.MassFlow, 0.0707, "Mass flow");
        public bool CryoAutoLimits = true;  // unused - backend does not accept limits
        public Parameter CryoXMin = new Parameter(Converters.ElapsingTime, -50, "Plot X min", maybeNull:true);
        public Parameter CryoXMax = new Parameter(Converters.ElapsingTime, 1100, "Plot X max", maybeNull:true);
        public Parameter CryoYMin = new Parameter(Converters.Distance, -0.5, "Plot Y min", maybeNull:true);
        public Parameter CryoYMax = new Parameter(Converters.Distance, 7.5, "Plot Y max", maybeNull:true);


        // // // // // // // // // //
        // QRA SYSTEM DESCRIPTION FORM
        public Parameter NumCompressors = new Parameter(0, "# Compressors", 0D);
        public Parameter NumVessels = new Parameter(0, "# Vessels (cylinders, tanks)", 0D);
        public Parameter NumValves = new Parameter(5, "# Valves", 0D);
        public Parameter NumInstruments = new Parameter(3, "# Instruments", 0D);
        public Parameter NumJoints = new Parameter(35, "# Joints", 0D);
        public Parameter NumHoses = new Parameter(1, "# Hoses", 0D);
        public Parameter PipeLength = new Parameter(Converters.Distance, DistanceUnit.Meter, 20, "Pipes (length)", 0D);
        public Parameter NumFlanges = new Parameter(0, "# Flanges", 0D);
        public Parameter NumFilters = new Parameter(0, "# Filters", 0D);
        public Parameter NumExchangers = new Parameter(0, "# Heat exchangers", 0D);
        public Parameter NumVaporizers = new Parameter(0, "# Vaporizers", 0D);
        public Parameter NumArms = new Parameter(0, "# Loading arms", 0D);
        public Parameter NumExtraComponents1 = new Parameter(0, "# Extra component 1", 0D);
        public Parameter NumExtraComponents2 = new Parameter(0, "# Extra component 2", 0D);


        public Parameter VehicleCount = new Parameter(20,"Number of Vehicles", 0);
        public Parameter VehicleFuelings = new Parameter(2,"Number of Fuelings Per Vehicle Day", 0);
        public Parameter VehicleDays = new Parameter(250, "Number of Vehicle Operating Days per Year", 0, 366);
        public Parameter VehicleAnnualDemand = new Parameter(10000,"Annual Demands (calculated)", 0);
        public Parameter FacilityLength = new Parameter(Converters.Distance, 20, "Length (x-direction)", 0);
        public Parameter FacilityWidth = new Parameter(Converters.Distance,12, "Width (z-direction)", 0);
        public Parameter PipeDiameter = new Parameter(Converters.Distance, DistanceUnit.Inch, 0.375, "Pipe outer diameter", 0);
        public Parameter PipeThickness = new Parameter(Converters.Distance, DistanceUnit.Inch, 0.065, "Pipe wall thickness", 0);

        public int QraMassFlowLeakSize = 1;  // divide by 100 for leak size as %; choices are {1, 10, 100, 1000, 10000}
        public Parameter QraMassFlow = new Parameter(Converters.MassFlow, 0, "Release mass flow (unchoked)", 0, 
                                                     maybeNull: true, isNull: true);

        // Optional overrides whereby user can control release for each size. Use -1 to disable override.
        public Parameter Release000d01 = new Parameter(Converters.Frequency, 0, "0.01% release annual frequency", 0,
                                                        maybeNull:true, isNull: true);
        public Parameter Release000d10 = new Parameter(Converters.Frequency,0, "0.10% release annual frequency", 0,
                                                        maybeNull:true, isNull: true);
        public Parameter Release001d00 = new Parameter(Converters.Frequency,0, "1% release annual frequency", 0,
                                                        maybeNull:true, isNull: true);
        public Parameter Release010d00 = new Parameter(Converters.Frequency,0, "10% release annual frequency", 0,
                                                        maybeNull:true, isNull: true);
        public Parameter Release100d00 = new Parameter(Converters.Frequency,0, "100% release annual frequency", 0,
                                                        maybeNull:true, isNull: true);

        public Parameter GasDetectionProb = new Parameter(0.9, "Gas detection credit probability", 0, 1);
        public Parameter ExclusionRadius = new Parameter(0.01, 0);
        public Parameter FailureOverride = new Parameter(Converters.Frequency, 0, 
                                                    "100% release annual frequency (accidents and shutdown failures)", 0,
                                                        maybeNull: true, isNull: true);


        // // // // // // // // // //
        // QRA DATA & PROBABILITIES
        public bool AllowUncertainty = false;  // if false, hides UQ inputs including Probabilities form tab-page
        public bool SampleOccupants = false;
        public bool SampleLeaks = false;
        public bool SampleFailures = false;
        public Parameter NumSamples = new Parameter(10);
        public Parameter RandomSeed = null;
        public List<ComponentProbability> ProbCompressor = FuelData.ActiveLeakSet.Compressor;
        public List<ComponentProbability> ProbVessel = FuelData.ActiveLeakSet.Vessel;
        public List<ComponentProbability> ProbFilter = FuelData.ActiveLeakSet.Filter;
        public List<ComponentProbability> ProbFlange = FuelData.ActiveLeakSet.Flange;
        public List<ComponentProbability> ProbHose = FuelData.ActiveLeakSet.Hose;
        public List<ComponentProbability> ProbJoint = FuelData.ActiveLeakSet.Joint;
        public List<ComponentProbability> ProbPipe = FuelData.ActiveLeakSet.Pipe;
        public List<ComponentProbability> ProbValve = FuelData.ActiveLeakSet.Valve;
        public List<ComponentProbability> ProbInstrument = FuelData.ActiveLeakSet.Instrument;
        public List<ComponentProbability> ProbExchanger = FuelData.ActiveLeakSet.HeatExchanger;
        public List<ComponentProbability> ProbVaporizer = FuelData.ActiveLeakSet.Vaporizer;
        public List<ComponentProbability> ProbArm = FuelData.ActiveLeakSet.LoadingArm;
        public List<ComponentProbability> ProbExtra1 = FuelData.ActiveLeakSet.Extra1;
        public List<ComponentProbability> ProbExtra2 = FuelData.ActiveLeakSet.Extra2;

        public FailureMode FailureNozPo = new FailureMode("Nozzle", "Pop-off", 
                                                          DistributionChoice.Beta, 0.5, 610415.5);
        public FailureMode FailureNozFtc = new FailureMode("Nozzle", "Failure to close", 
                                                           DistributionChoice.ExpectedValue, 0.002D, 0D);
        public FailureMode FailureMValveFtc = new FailureMode("Manual valve", "Failure to close", 
                                                              DistributionChoice.ExpectedValue, 0.001D, 0D);
        public FailureMode FailureSValveFtc = new FailureMode("Solenoid valve", "Failure to close", 
                                                              DistributionChoice.ExpectedValue, 0.002D, 0D);
        public FailureMode FailureSValveCcf = new FailureMode("Solenoid valve", "Common-cause failure", 
                                                              DistributionChoice.ExpectedValue, 0.000128D, 0D);
        public FailureMode FailureOverp = new FailureMode("Overpressure during fueling", "Accident", 
                                                      DistributionChoice.Beta, 3.5, 310289.5);
        public FailureMode FailurePValveFto = new FailureMode("Pressure relief valve", "Failure to open", 
                                                              DistributionChoice.LogNormal, -11.74D, 0.67D);
        public FailureMode FailureDriveoff = new FailureMode("Driveoff", "Accident", DistributionChoice.Beta, 31.5D, 610384.5D);
        public FailureMode FailureCouplingFtc = new FailureMode("Breakaway coupling", "Failure to close", 
                                                                DistributionChoice.Beta, 0.5D, 5031.0D);

        public double[] ImmediateIgnitionProbs = { 0.008D, 0.053D, 0.23D};
        public double[] DelayedIgnitionProbs = { 0.004, 0.027, 0.12 };
        public double[] IgnitionThresholds = { 0.125, 6.25 };


        // // // // // // // // // //
        // QRA CONSEQUENCE MODELS
        public ModelPair ThermalProbit;
        public ModelPair OverpressureProbit;
        public Parameter FlameExposureTime = new Parameter(Converters.ElapsingTime, 30, "", 0);  // s
        public TimeUnit ExposureTimeUnit { get; set; } = TimeUnit.Second;


        /// <summary>
        /// Returns component probability data as 2d array with outer dimension representing leak sizes.
        /// </summary>
        /// <param name="probabilitiesList"></param>
        /// <returns>Array of probabilities for each leak size and component.</returns>
        public double[][] GetProbabilitiesAsArray(List<ComponentProbability> probabilitiesList)
        {
            double[][] probabilities =
            {
                probabilitiesList[0].GetParameters(), probabilitiesList[1].GetParameters(), probabilitiesList[2].GetParameters(),
                probabilitiesList[3].GetParameters(), probabilitiesList[4].GetParameters()
            };
            return probabilities;
        }


        /// <summary>
        /// Initializes dependent parameters and lists used throughout the GUI.
        /// </summary>
        public void InitializeState(bool fromFile = false)
        {
            Fuels.Clear();
            Fuels.AddRange(new List<FuelType>
            {
                FuelH2, FuelMethane, FuelPropane,
                FuelN2, FuelCo2, FuelEthane, FuelNButane, FuelIsoButane,
                FuelNPentane, FuelIsoPentane, FuelNHexane,
            });

            FuelType.FuelConcentrationChangedEvent += (o, args) => HandleFuelConcentrationChanged();
            // fuel changed via dropdown, not via concentration editing.
            FuelTypeChangedEvent += (o, args) => HandleFuelTypeChanged();

            Phases.Clear();
            Phases.AddRange(new List<ModelPair> {GasDefault, SatGas, SatLiquid});
            if (Phase == null)
            {
                Phase = GasDefault;
            }

            NozzleModels.Clear();
            NozzleModels.AddRange(new List<ModelPair>{ NozzleBirch, NozzleBirch2, NozzleEwanMoodie, NozzleMolkov, NozzleYuceilOtugen });
            if (Nozzle == null)
            {
                Nozzle = NozzleYuceilOtugen;
            }

            // this is refreshed when fuel changes since Bauwens only allowed for H2
            OverpressureMethods.Clear();
            OverpressureMethods.AddRange(new List<ModelPair> { BstMethod, TntMethod, BauwensMethod });
            if (OverpressureMethod == null)
            {
                OverpressureMethod = BstMethod;
            }

            OverpressureProbitModels.Clear();
            OverpressureProbitModels.AddRange(new List<ModelPair> {ProbitLungEis, ProbitLungHse, ProbitHead, ProbitCollapse});
            if (OverpressureProbit == null)
            {
                OverpressureProbit = ProbitHead;
            }

            ThermalProbitModels.Clear();
            ThermalProbitModels.AddRange(new List<ModelPair> {ThermalEisenberg, ThermalTsao, ThermalTno, ThermalLees});
            if (ThermalProbit == null)
            {
                ThermalProbit = ThermalEisenberg;
            }

            SensitivityDistributions.Clear();
            SensitivityDistributions.AddRange(new List<ModelPair>
            {
                SensitivityDistribution.Deterministic,
                SensitivityDistribution.Normal,
                SensitivityDistribution.LogNormal,
                SensitivityDistribution.Uniform

            });

            UncertaintyTypes.Clear();
            UncertaintyTypes.AddRange(new List<ModelPair>
            {
                UncertaintyType.None,
                UncertaintyType.Aleatory,
                UncertaintyType.Epistemic
            });

            // don't overwrite possibly-customized data
            if (!fromFile)
            {
                RefreshLeakFrequencyData();
            }

            var rand = new Random();
            if (RandomSeed == null)
            {
                var seed = rand.Next(1000000, 10000000);
                RandomSeed = new Parameter(seed);
            }

            AlertBackColors = new List<Color>{Color.LightCyan, Color.Cornsilk, Color.MistyRose};
            AlertTextColors = new List<Color>{Color.PaleTurquoise, Color.DarkGoldenrod, Color.Maroon};
        }

        private void HandleFuelConcentrationChanged()
        {
            if (_changeConcentrationSilently) return;

            RefreshFuelProperties();
            FuelTypeChangedEvent?.Invoke(this, EventArgs.Empty);
        }


        /// <summary>
        /// Ensures fuel-related data is in sync with selected fuel and concentrations.
        /// Updates displayed species (i.e. dropdown), leak frequency data, component counts, ignition probabilities, and probit models.
        /// </summary>
        private void HandleFuelTypeChanged()
        {
            RefreshSpeciesOption();
            RefreshLeakFrequencyData();
            RefreshComponentQuantities();
            RefreshIgnitionProbabilities();
            RefreshProbits();
        }

        public bool IsUncertain()
        {
            return AllowUncertainty && (SampleFailures | SampleLeaks | SampleOccupants);
        }


        /// <summary>
        /// Sets leak frequency state to default values for fuel and phase.
        /// </summary>
        private void RefreshLeakFrequencyData()
        {
            FuelType fuel = GetActiveFuel();
            ComponentProbabilitySet newP;

            if (fuel == FuelH2)
            {
                newP = Phase == GasDefault ? FuelData.H2GasLeaks : FuelData.H2LiquidLeaks;
            }
            else if (fuel == FuelPropane)
            {
                newP = Phase == GasDefault ? FuelData.PropaneGasLeaks : FuelData.PropaneLiquidLeaks;
            }
            else
            {
                // blends and Methane rely on Methane data
                newP = Phase == GasDefault ? FuelData.MethaneGasLeaks : FuelData.MethaneLiquidLeaks;
            }

            for (int i = 0; i < 5; i++)
            {
                ProbCompressor[i].SetParameters(newP.Compressor[i].Mu, newP.Compressor[i].Sigma);
                ProbVessel[i].SetParameters(newP.Vessel[i].Mu, newP.Vessel[i].Sigma);
                ProbFilter[i].SetParameters(newP.Filter[i].Mu, newP.Filter[i].Sigma);
                ProbFlange[i].SetParameters(newP.Flange[i].Mu, newP.Flange[i].Sigma);
                ProbHose[i].SetParameters(newP.Hose[i].Mu, newP.Hose[i].Sigma);
                ProbJoint[i].SetParameters(newP.Joint[i].Mu, newP.Joint[i].Sigma);
                ProbPipe[i].SetParameters(newP.Pipe[i].Mu, newP.Pipe[i].Sigma);
                ProbValve[i].SetParameters(newP.Valve[i].Mu, newP.Valve[i].Sigma);
                ProbInstrument[i].SetParameters(newP.Instrument[i].Mu, newP.Instrument[i].Sigma);
                ProbExchanger[i].SetParameters(newP.HeatExchanger[i].Mu, newP.HeatExchanger[i].Sigma);
                ProbVaporizer[i].SetParameters(newP.Vaporizer[i].Mu, newP.Vaporizer[i].Sigma);
                ProbArm[i].SetParameters(newP.LoadingArm[i].Mu, newP.LoadingArm[i].Sigma);
                ProbExtra1[i].SetParameters(newP.Extra1[i].Mu, newP.Extra1[i].Sigma);
                ProbExtra2[i].SetParameters(newP.Extra2[i].Mu, newP.Extra2[i].Sigma);
            }
        }

        /// <summary>
        /// Updates component quantities to default values for selected fuel and phase.
        /// </summary>
        private void RefreshComponentQuantities()
        {
            var fuel = GetActiveFuel();
            double[] counts;
            double[] gh2Counts =     {1d, 2, 3, 0, 1, 43, 30,  7, 5, 0, 0, 0, 0, 0};
            double[] lh2Counts =     {0d, 1, 0, 8, 1,  0, 30, 44, 0, 0, 0, 0, 0, 0};
            double[] gasCh4Counts =  {1d, 2, 3, 0, 1, 43, 30,  7, 5, 1, 0, 0, 0, 0};
            double[] liqCh4Counts =  {0d, 1, 0, 8, 1, 0,  30, 44, 0, 1, 0, 0, 0, 0};
            double[] propaneCounts = {1d, 1, 2, 8, 1, 0,  30, 44, 0, 0, 0, 0, 0, 0};
            double[] blendsCounts = gh2Counts;

            if (fuel == FuelH2)
            {
                counts = Phase == GasDefault ? gh2Counts : lh2Counts;
            }
            else if (fuel == FuelMethane)
            {
                counts = Phase == GasDefault ? gasCh4Counts : liqCh4Counts;

            }
            else if (fuel == FuelPropane)
            {
                counts = propaneCounts;
            }
            else
            {
                counts = blendsCounts;
            }

            NumCompressors.SetValue(counts[0]);
            NumVessels.SetValue(counts[1]);
            NumFilters.SetValue(counts[2]);
            NumFlanges.SetValue(counts[3]);
            NumHoses.SetValue(counts[4]);
            NumJoints.SetValue(counts[5]);
            PipeLength.SetValue(DistanceUnit.Meter, counts[6]);
            NumValves.SetValue(counts[7]);
            NumInstruments.SetValue(counts[8]);
            NumExchangers.SetValue(counts[9]);
            NumVaporizers.SetValue(counts[10]);
            NumArms.SetValue(counts[11]);
            NumExtraComponents1.SetValue(counts[12]);
            NumExtraComponents2.SetValue(counts[13]);
        }

        /// <summary>
        /// Updates ignition probability data based on selected fuel.
        /// </summary>
        private void RefreshIgnitionProbabilities()
        {
            var h2Thresholds = new[] { 0.125, 6.25 };
            var h2Immediate = new[] { 0.008D, 0.053D, 0.23D };
            var h2Delayed = new[] { 0.004D, 0.027D, 0.12D };
            // hydrocarbon data
            var hcThresholds = new[] { 1D, 50D };
            var hcImmediate = new[] { 0.007D, 0.047D, 0.20D };
            var hcDelayed = new[] { 0.003D, 0.023D, 0.10D };

            // if using H2 or blend with H2, use H2 data. Otherwise use Methane data.
            if (FuelH2.Amount > 0)
            {
                ImmediateIgnitionProbs = h2Immediate;
                DelayedIgnitionProbs = h2Delayed;
                IgnitionThresholds = h2Thresholds;
            }
            else
            {
                ImmediateIgnitionProbs = hcImmediate;
                DelayedIgnitionProbs = hcDelayed;
                IgnitionThresholds = hcThresholds;
            }
        }

        private void RefreshProbits()
        {
            var fuel = GetActiveFuel();
            if (fuel == FuelH2)
            {
                ThermalProbit = ThermalEisenberg;
                OverpressureMethods.Clear();
                OverpressureMethods.AddRange(new List<ModelPair> { BstMethod, TntMethod, BauwensMethod });
            }
            else
            {
                ThermalProbit = ThermalTsao;
                OverpressureMethods.Clear();
                OverpressureMethods.AddRange(new List<ModelPair> { BstMethod, TntMethod });
                if (OverpressureMethod == BauwensMethod)
                {
                    OverpressureMethod = BstMethod;
                }
            }
        }

        /// <summary>
        /// Computes choked flow ratio as weighted average and minimum critical pressure for blend compositions
        /// </summary>
        private void RefreshFuelProperties()
        {
            FuelType fuel = GetActiveFuel();
            if (fuel == FuelBlend)
            {
                double ratio = 0;
                double p = 0;

                foreach (FuelType next in Fuels)
                {
                    if (next != FuelBlend && next.Active && next.Amount > 0)
                    {
                        ratio += next.ChokedFlowRatio * next.Amount;
                        if (next.CriticalP < p)
                        {
                            p = next.CriticalP;
                        }
                    }
                }

                FuelBlend.ChokedFlowRatio = ratio;
                FuelBlend.CriticalP = p;
            }
        }


        /// <summary>
        /// Updates dropdown-bound species indicator.
        /// </summary>
        private void RefreshSpeciesOption()
        {
            // NOTE: does nothing if species set to a blend preset.
            var fuel = GetActiveFuel();
            if (fuel == FuelH2)
            {
                SpeciesOption = SpeciesOptions.EFuelH2;
            }
            else if (fuel == FuelMethane)
            {
                SpeciesOption = SpeciesOptions.EFuelMethane;
            }
            else if (fuel == FuelPropane)
            {
                SpeciesOption = SpeciesOptions.EFuelPropane;
            }
            else
            {
                if (SpeciesOption == SpeciesOptions.EFuelH2 ||
                    SpeciesOption == SpeciesOptions.EFuelMethane ||
                    SpeciesOption == SpeciesOptions.EFuelPropane)
                {
                    SpeciesOption = SpeciesOptions.EFuelBlendEmpty;
                }
            }
        }

        /// <summary>
        /// Sets all fuel concentrations to 0 and deactivates all fuels.
        /// </summary>
        private void ClearFuels()
        {
            foreach (var fuel in Fuels)
            {
                if (fuel.Amount > 0)
                {
                    fuel.Amount = 0;
                    fuel.Active = false;
                }
            }
        }

        ///////////////////////////////
        /// PUBLIC

        /// <summary>
        /// Writes this StateContainer parameter data to JSON file.
        /// </summary>
        /// <param name="filepath">Path to JSON file</param>
        public void Save(string filepath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(filepath, json);
        }

        /// <summary>
        /// Loads parameter data from JSON file into StateContainer object and refreshes it's dependent parameters.
        /// </summary>
        /// <param name="filepath">Path to JSON file</param>
        /// <returns>StateContainer</returns>
        public static StateContainer Load(string filepath)
        {
            var result = Deserialize(filepath);
            result.InitializeState(true);
            return result;
        }

        /// <summary>
        /// Loads JSON data into state object and returns it.
        /// </summary>
        /// <param name="filepath">Path to JSON file</param>
        /// <returns></returns>
        public static StateContainer Deserialize(string filepath)
        {
            StateContainer result;

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                IncludeFields = true,
                NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault
            };
            string json = File.ReadAllText(filepath);
            result = JsonSerializer.Deserialize<StateContainer>(json, options);

            return result;
        }

        /// <summary>
        /// Checks whether active phase is saturated.
        /// </summary>
        /// <returns>bool</returns>
        public bool PhaseIsSaturated()
        {
            return (Phase == SatLiquid || Phase == SatGas);
        }

        /// <summary>
        /// Checks if temperature input should be used according to selected phase.
        /// </summary>
        /// <returns></returns>
        public bool DisplayTemperature()
        {
            return !PhaseIsSaturated();
        }

        /// <summary>
        /// Calculates total concentration summed from all fuels.
        /// </summary>
        /// <returns></returns>
        public double SumFuelAmounts()
        {
            double result = 0;
            foreach (var fuelType in Fuels)
            {
                result += fuelType.Amount;
            }

            result = Math.Round(result, 5);
            return result;
        }

        /// <summary>
        /// Returns active fuel object.
        /// </summary>
        /// <returns>FuelType</returns>
        public FuelType GetActiveFuel()
        {
            FuelType result = FuelH2;
            int numActiveFuels = 0;
            foreach (var fuel in Fuels)
            {
                if (fuel.Amount > 0)
                {
                    numActiveFuels++;
                    result = fuel;
                }
            }

            result = (numActiveFuels > 1 || numActiveFuels == 0) ? FuelBlend : result;
            return result;
        }

        /// <summary>
        /// Sets active status of all fuels to provided value.
        /// </summary>
        /// <param name="enable">boolean</param>
        public void ToggleFuelStatus(bool enable = true)
        {
            foreach (var fuel in Fuels)
            {
                fuel.Active = enable;
            }
        }

        /// <summary>
        /// Returns the fuel object matches the provided id.
        /// </summary>
        /// <param name="id">Fuel ID</param>
        /// <returns>FuelType</returns>
        public FuelType GetFuel(int id)
        {
            FuelType result = null;
            foreach (var fuel in Fuels)
            {
                if (id == fuel.Id)
                {
                    result = fuel;
                    break;
                }
            }
            if (result == null)
            {
                result = FuelBlend;
            }

            return result;
        }

        /// <summary>
        /// Adjusts fuel concentrations to activate the selected fuel, and refreshes related parameters and displayed data.
        /// </summary>
        /// <param name="fuel"></param>
        public void SetFuel(FuelType fuel)
        {
//            _changeConcentrationSilently = true;

            ClearFuels();

            if (fuel == FuelH2)
            {
                FuelH2.Amount = 1;
            }
            else if (fuel == FuelMethane)
            {
                FuelMethane.Amount = 1;
            }
            else if (fuel == FuelPropane)
            {
                FuelPropane.Amount = 1;
            }
//            _changeConcentrationSilently = false;

            HandleFuelTypeChanged();
        }


        /// <summary>
        /// Updates selected fuel amounts from preset blend (e.g. NIST1) or from single species.
        /// </summary>
        /// <param name="species"></param>
        public void SetFuel(SpeciesOptions species)
        {
            SpeciesOption = species;

            // Trigger single event after everything updated
            _changeConcentrationSilently = true;

            switch (species)
            {
                case SpeciesOptions.EFuelH2:
                    SetFuel(FuelH2);
                    break;
                case SpeciesOptions.EFuelMethane:
                    SetFuel(FuelMethane);
                    break;
                case SpeciesOptions.EFuelPropane:
                    SetFuel(FuelPropane);
                    break;
                default:
                    // blend, possibly a preset
                    SetFuel(FuelBlend);

                    if (species == SpeciesOptions.EFuelBlendEmpty)
                    {
                        ;
                    }
                    else if (species == SpeciesOptions.EFuelBlendNist1)
                    {
                        FuelMethane.Amount = 0.965210;
                        FuelN2.Amount = 0.00260;
                        FuelCo2.Amount = 0.00596;
                        FuelEthane.Amount = 0.018190;
                        FuelPropane.Amount = 0.00460;
                        FuelNButane.Amount = 0.00101;
                        FuelIsoButane.Amount = 0.00098;
                        FuelNPentane.Amount = 0.00032;
                        FuelIsoPentane.Amount = 0.00047;
                        FuelNHexane.Amount = 0.00066;
                    }
                    else if (species == SpeciesOptions.EFuelBlendNist2)
                    {
                        FuelMethane.Amount = 0.906729;
                        FuelN2.Amount = 0.03128;
                        FuelCo2.Amount = 0.00468;
                        FuelEthane.Amount = 0.045280;
                        FuelPropane.Amount = 0.00828;
                        FuelNButane.Amount = 0.00156;
                        FuelIsoButane.Amount = 0.00104;
                        FuelNPentane.Amount = 0.00044;
                        FuelIsoPentane.Amount = 0.00032;
                        FuelNHexane.Amount = 0.00039;
                    }
                    else if (species == SpeciesOptions.EFuelBlendRg2)
                    {
                        FuelMethane.Amount = 0.859051;
                        FuelN2.Amount = 0.01007;
                        FuelCo2.Amount = 0.01495;
                        FuelEthane.Amount = 0.084919;
                        FuelPropane.Amount = 0.02302;
                        FuelNButane.Amount = 0.00351;
                        FuelIsoButane.Amount = 0.00349;
                        FuelNPentane.Amount = 0.00048;
                        FuelIsoPentane.Amount = 0.00051;
                        FuelNHexane.Amount = 0;
                    }
                    else if (species == SpeciesOptions.EFuelBlendGu1)
                    {
                        FuelMethane.Amount = 0.814410;
                        FuelN2.Amount = 0.13465;
                        FuelCo2.Amount = 0.00985;
                        FuelEthane.Amount = 0.033000;
                        FuelPropane.Amount = 0.00605;
                        FuelNButane.Amount = 0.00104;
                        FuelIsoButane.Amount = 0.00100;
                        FuelNPentane.Amount = 0;
                        FuelIsoPentane.Amount = 0;
                        FuelNHexane.Amount = 0;
                    }
                    else if (species == SpeciesOptions.EFuelBlendGu2)
                    {
                        FuelMethane.Amount = 0.812120;
                        FuelN2.Amount = 0.05702;
                        FuelCo2.Amount = 0.07585;
                        FuelEthane.Amount = 0.043030;
                        FuelPropane.Amount = 0.00895;
                        FuelNButane.Amount = 0.00152;
                        FuelIsoButane.Amount = 0.00151;
                        FuelNPentane.Amount = 0;
                        FuelIsoPentane.Amount = 0;
                        FuelNHexane.Amount = 0;
                    }
                    break;
            }

            _changeConcentrationSilently = false;
            HandleFuelConcentrationChanged();
        }


        /// <summary>
        /// Adjusts fuel amounts to ensure 100% capacity, based on fuel type.
        /// If fuel type is a single fuel, remainder is set to that fuel.
        /// If fuel type is blend, remainder is proportionally applied to all fuels with amount > 0.
        /// </summary>
        public void AllocateFuelRemainder(FuelType fuel)
        {
            double epsilon = 0.00005;
            var fuelSum = SumFuelAmounts();
            double remainder = (1 - fuelSum);

            if (Math.Abs(fuelSum - 1.0f) <= epsilon)
            {
                return;
            }

            if (fuel != FuelBlend)
            {
                fuel.Amount += remainder;
            }
            else
            {
                // allocate remainder across active fuels
                int numActiveFuels = 0;
                // remember first used to give it any final remainder
                FuelType firstActive = null;

                // check how many fuels are being used
                foreach (var next in Fuels)
                {
                    if (next.Active)
                    {
                        if (numActiveFuels == 0)
                        {
                            firstActive = next;
                        }
                        numActiveFuels++;
                    }
                }

                double amountPerFuel = remainder / numActiveFuels;

                // apply remainder to those active fuels
                foreach (var next in Fuels)
                {
                    if (next.Active)
                    {
                        next.Amount += amountPerFuel;
                    }

                }

                // handle any final remainder due to float calcs
                if (firstActive != null)
                {
                    firstActive.Amount += (1.0 - SumFuelAmounts());
                }
            }
        }

        /// <summary>
        /// Returns list of fuels as python dictionary(key, amount)
        /// NOTE: must be called from within python GIL (i.e. inside Py.GIL() clause).
        /// </summary>
        /// <returns>PyObject</returns>
        public PyObject GetFuelsPyDict()
        {
            {
                PyDict dict = new PyDict();
                foreach (var fuel in Fuels)
                {
                    var next = new PyFloat(fuel.Amount);
                    dict[fuel.Key] = next;
                }
                return dict;
            }
        }

        public double GetFluidPressure(PressureUnit unit = PressureUnit.MPa)
        {
            var result = FluidPressure.GetValue(unit);
            if (!PressureIsAbsolute)
            {
                var ambientP = AmbientPressure.GetValue(unit);
                result += ambientP;
            }
            return result;
        }


        public string GetSpeciesOptionString()
        {
            string result = "";
            switch (SpeciesOption)
            {
                case SpeciesOptions.EFuelH2:
                    result = "Hydrogen";
                    break;
                case SpeciesOptions.EFuelMethane:
                    result = "Methane";
                    break;
                case SpeciesOptions.EFuelPropane:
                    result = "Propane";
                    break;
                case SpeciesOptions.EFuelBlendGu1:
                    result = "GU1 Blend";
                    break;
                case SpeciesOptions.EFuelBlendGu2:
                    result = "GU2 Blend";
                    break;
                case SpeciesOptions.EFuelBlendNist1:
                    result = "NIST1 Blend";
                    break;
                case SpeciesOptions.EFuelBlendNist2:
                    result = "NIST2 Blend";
                    break;
                case SpeciesOptions.EFuelBlendRg2:
                    result = "RG2 Blend";
                    break;
                case SpeciesOptions.EFuelBlendEmpty:
                    result = "Blend";
                    break;
            }

            return result;
        }


        /// <summary>
        /// Checks whether fuel release pressure is within valid range
        /// </summary>
        /// <returns>true if pressure is valid for fuel and phase.</returns>
        public bool ReleasePressureIsValid()
        {
            bool result = true;
            if (PhaseIsSaturated())
            {
                var fluidPressure = GetFluidPressure();
                var ambientP = AmbientPressure.GetValue(PressureUnit.MPa);
                if (!PressureIsAbsolute)
                {
                    fluidPressure += ambientP;
                }
                var criticalP = GetActiveFuel().GetCriticalPressureMpa();

                if (fluidPressure <= ambientP || fluidPressure > criticalP)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// Checks whether active fuel is unchoked.
        /// </summary>
        /// <returns>bool</returns>
        public bool FuelFlowUnchoked()
        {
            double fluidP = GetFluidPressure(PressureUnit.Pa);
            double ambientP = AmbientPressure.GetValue(PressureUnit.Pa);
            var fuel = GetActiveFuel();

            if ((Math.Abs(fuel.ChokedFlowRatio) > FuelPrecision) &&
                fluidP < fuel.ChokedFlowRatio * ambientP)
            {
                return true;
            }
            else
            {
                FluidMassFlow = null;
                return false;
            }
        }
    }

}
