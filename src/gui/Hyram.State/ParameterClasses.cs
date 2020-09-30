using System;
using MathNet.Numerics.Distributions;

namespace SandiaNationalLaboratories.Hyram
{
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
        public static readonly NozzleModel Molkov = new NozzleModel(3, "Molkov", "molk");
        public static readonly NozzleModel YuceilOtugen = new NozzleModel(4, "Yuceil/Otugen", "yuce");
        //public static readonly NozzleModel HarstadBellan = new NozzleModel(5, "Harstad/Bellan", "hars");
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
                case "molk":
                    return Molkov;
                //case "hars":
                    //return HarstadBellan;
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
                case "Molkov":
                    return Molkov;
                //case "Harstad/Bellan":
                    //return HarstadBellan;
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
        // TODO: update critical ratios
        public static readonly FuelType H2 = new FuelType(0, "H2", "H2", crit_ratio:1.893);
        public static readonly FuelType CNG = new FuelType(1, "CNG", "CNG", crit_ratio:0.0);
        public static readonly FuelType LNG = new FuelType(2, "LNG", "LNG", crit_ratio:0.0);
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;
        private readonly double _crit_ratio;

        private FuelType(int value, string name, string key, double crit_ratio)
        {
            _name = name;
            _value = value;
            _key = key;
            _crit_ratio = crit_ratio;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }

        public double GetRatio()
        {
            return _crit_ratio;
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
        //public static readonly OverpressureProbitModel Debris = new OverpressureProbitModel(4, "Debris", "debr");
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
    /// Representation of available fluid phases.
    /// </summary>
    [Serializable]
    public sealed class FluidPhase
    {
        public static readonly FluidPhase GasDefault = new FluidPhase("Gas", "default", index: 0);
        public static readonly FluidPhase SatGas = new FluidPhase("Saturated vapor", "gas", index: 1);
        public static readonly FluidPhase SatLiquid = new FluidPhase("Saturated liquid", "liquid", index: 2);
        private readonly string _key;
        private readonly string _name;
        private readonly int _index;

        private FluidPhase(string name, string key, int index)
        {
            _name = name;
            _key = key;
            _index = index;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }

        public int GetIndex()
        {
            return _index;
        }

        /// <summary>
        /// Whether the temperature should be displayed. Only true when phase is gas and not saturated.
        /// </summary>
        /// <returns></returns>
        public static bool DisplayTemperature()
        {
            FluidPhase selectedPhase = StateContainer.GetValue<FluidPhase>("ReleaseFluidPhase");
            return (selectedPhase.GetIndex() == 0);
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
        private double _median;
        private bool _updateFromMedian = true;
        private double _mu;
        private double _sigma;

        public ComponentProbability(string leakSize, double mu, double sigma)
        {
            LeakSize = leakSize;
            Mu = mu;
            Sigma = sigma;
        }

        public string LeakSize { get; set; }

        public double Mu
        {
            get => _mu;
            set
            {
                _mu = value;
                UpdateDistributionParameters();
            }
        }

        public double Sigma
        {
            get => _sigma;
            set
            {
                _sigma = value;
                UpdateDistributionParameters();
            }
        }

        public double Mean { get; set; }
        public double Fifth { get; set; }
        public double Median
        {
            get => _median;
            set
            {
                _median = value;
                if (_updateFromMedian) Mu = Math.Log(_median);
            }
        }
        public double NinetyFifth { get; set; }

        /// <summary>
        /// Compile probability params and return as list
        /// </summary>
        /// <returns></returns>
        public double[] GetParameters()
        {
            double[] data = {Mu, Sigma};
            return data;
        }

        public void UpdateDistributionParameters()
        {
            // disable auto-compute of Mu from Median
            bool storedUpdateFlag = _updateFromMedian;
            _updateFromMedian = false;

            var logNormalDist = new LogNormal(Mu, Sigma);
            Mean = logNormalDist.Mean;
            Median = logNormalDist.Median;

            // get z-score via inverse CD
            var curve = new Normal();
            var zValue = curve.InverseCumulativeDistribution(0.95);
            NinetyFifth = Math.Exp(Mu + zValue * Sigma);

            zValue = curve.InverseCumulativeDistribution(0.05);
            Fifth = Math.Exp(Mu + zValue * Sigma);

            _updateFromMedian = storedUpdateFlag;
        }
    }

    // Provides names to prepend database key for floor and ceiling grid input variables.
    public enum WhichVent
    {
        Floor,
        Ceiling
    } 

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
}
