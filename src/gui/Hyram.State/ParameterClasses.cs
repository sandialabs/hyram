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
