/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;

namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    /// Classes for storing model choice including descriptive string name.
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
        public override bool Equals(object obj)
        {
            return Equals(obj as NozzleModel);
        }

        public bool Equals(NozzleModel f)
        {
            // If parameter is null, return false.
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (_value == f._value);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(NozzleModel lhs, NozzleModel rhs)
        {
            // Check for null on left side.
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(NozzleModel lhs, NozzleModel rhs)
        {
            return !(lhs == rhs);
        }
    }

    [Serializable]
    public class UnconfinedOverpressureMethod
    {
        public static readonly UnconfinedOverpressureMethod BstMethod = new UnconfinedOverpressureMethod(0, "BST", "bst");
        public static readonly UnconfinedOverpressureMethod TntMethod = new UnconfinedOverpressureMethod(1, "TNT", "tnt");
        public static readonly UnconfinedOverpressureMethod BauwensMethod = new UnconfinedOverpressureMethod(2, "Bauwens", "bauwens");
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;

        public UnconfinedOverpressureMethod(int value, string name, string key)
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

        public static UnconfinedOverpressureMethod ParseName(string name)
        {
            switch (name.ToLower())
            {
                case "tnt":
                    return TntMethod;
                case "bauwens":
                    return BauwensMethod;
                case "bst":
                default:
                    return BstMethod;
            }
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as UnconfinedOverpressureMethod);
        }

        public bool Equals(UnconfinedOverpressureMethod f)
        {
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            if (GetType() != f.GetType())
            {
                return false;
            }
            return (_value == f._value);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(UnconfinedOverpressureMethod lhs, UnconfinedOverpressureMethod rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    return true;
                }
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(UnconfinedOverpressureMethod lhs, UnconfinedOverpressureMethod rhs)
        {
            return !(lhs == rhs);
        }
    }


    /// <summary>
    /// Representation of fuel type, designated by string label which is consumed by python funcs.
    /// </summary>
    [Serializable]
    public sealed class FuelType
    {
        // TODO: update critical ratios
        public static readonly FuelType Hydrogen = new FuelType(0, "Hydrogen", "h2", chokedFlowPRatio:1.904, criticalP: 1296400);
        public static readonly FuelType Methane = new FuelType(1, "Methane", "ch4", chokedFlowPRatio:1.844, criticalP: 4599200);
        public static readonly FuelType Propane = new FuelType(2, "Propane", "c3h8", chokedFlowPRatio:1.725, criticalP: 4251200);
        private readonly string _key;
        private readonly string _name;
        private readonly int _value;
        private readonly double _chokedFlowPRatio;  // [-]
        private readonly double _liquidCriticalP;  // [Pa]

        private FuelType(int value, string name, string key, double chokedFlowPRatio, double criticalP)
        {
            _name = name;
            _value = value;
            _key = key;
            _chokedFlowPRatio = chokedFlowPRatio;
            _liquidCriticalP = criticalP;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }

        public double GetCriticalRatio()
        {
            return _chokedFlowPRatio;
        }

        public double GetCriticalPressureMpa()
        {
            return (_liquidCriticalP / 1000000.0);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FuelType);
        }

        public bool Equals(FuelType f)
        {
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (_value == f._value);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(FuelType lhs, FuelType rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FuelType lhs, FuelType rhs)
        {
            return !(lhs == rhs);
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
        public override bool Equals(object obj)
        {
            return Equals(obj as ThermalProbitModel);
        }

        public bool Equals(ThermalProbitModel f)
        {
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (_value == f._value);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(ThermalProbitModel lhs, ThermalProbitModel rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ThermalProbitModel lhs, ThermalProbitModel rhs)
        {
            return !(lhs == rhs);
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
        public override bool Equals(object obj)
        {
            return Equals(obj as OverpressureProbitModel);
        }

        public bool Equals(OverpressureProbitModel f)
        {
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (_value == f._value);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(OverpressureProbitModel lhs, OverpressureProbitModel rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(OverpressureProbitModel lhs, OverpressureProbitModel rhs)
        {
            return !(lhs == rhs);
        }
    }


    /// <summary>
    /// Representation of available fluid phases.
    /// </summary>
    [Serializable]
    public sealed class FluidPhase
    {
        public static readonly FluidPhase GasDefault = new FluidPhase("Gas", "none", index: 0);
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
        public override bool Equals(object obj)
        {
            return Equals(obj as FluidPhase);
        }

        public bool Equals(FluidPhase f)
        {
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (_index == f._index);
        }

        public override int GetHashCode()
        {
            return _index;
        }

        public static bool operator ==(FluidPhase lhs, FluidPhase rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(FluidPhase lhs, FluidPhase rhs)
        {
            return !(lhs == rhs);
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
        public override bool Equals(object obj)
        {
            return Equals(obj as DeflagrationModel);
        }

        public bool Equals(DeflagrationModel f)
        {
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (_value == f._value);
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public static bool operator ==(DeflagrationModel lhs, DeflagrationModel rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(DeflagrationModel lhs, DeflagrationModel rhs)
        {
            return !(lhs == rhs);
        }
    }

    /// <summary>
    /// Distribution to use for specified failure mode (e.g. runaway)
    /// </summary>
    [Serializable]
    public class FailureMode
    {
        private double _paramA;
        private double _paramB;
        private FailureDistributionType _dist;

        public FailureMode(string name, string mode, FailureDistributionType dist, double paramA, double paramB)
        {
            Name = name;
            Mode = mode;
            Dist = dist;
            if ((Dist == FailureDistributionType.Beta) && (paramA == 0.0f))
            {
                ParamA = 1.0f;
            }
            else
            {
                ParamA = paramA;
            }

            if (paramB == 0.0f &&
                (Dist == FailureDistributionType.Beta || Dist == FailureDistributionType.LogNormal)
                )
            {
                ParamB = 1.0f;
            }
            else
            {
                ParamB = paramB;
            }
        }
        public string Name { get; set; }
        public string Mode { get; set; }

        public FailureDistributionType Dist
        {
            get => _dist;
            set
            {
                if (_paramA == 0.0f &&
                    (value == FailureDistributionType.Beta))
                {
                    _paramA = 1.0f;
                }
                if (_paramB == 0.0f &&
                    (value == FailureDistributionType.Beta || value == FailureDistributionType.LogNormal))
                {
                    _paramB = 1.0f;
                }

                _dist = value;
            }
        }

        public double ParamA
        {
            get => _paramA;
            set
            {
                if (Dist == FailureDistributionType.Beta && value == 0.0f)
                {
                    return;
                }
                _paramA = value;
            }
        }

        public double ParamB
        {
            get => _paramB;
            set
            {
                if (value == 0.0f &&
                    (Dist == FailureDistributionType.Beta || Dist == FailureDistributionType.LogNormal))
                {
                    return;
                }
                _paramB = value;
            }
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
