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

namespace SandiaNationalLaboratories.Hyram
{
    public enum EWorkerDistribution
    {
        // do not rearrange without updating python as well
        Normal,
        Uniform,
        Constant
    }

    [Serializable]
    public class OccupantDistributionInfo
    {
        private string _mDesc = "";

        private double _mExposureHours = 2000;

        //Private members and defaults
        private int _mNumTargets = 1;
        private DistanceUnit _mParamsUnitType = DistanceUnit.Meter;
        private Enum _mXLocDistribution = EWorkerDistribution.Uniform;
        private ConvertibleValue _mXLocParamA = CreateDefaultXLocParamA();
        private ConvertibleValue _mXLocParamB = CreateDefaultXLocParamB();
        private Enum _mYLocDistribution = EWorkerDistribution.Uniform;
        private ConvertibleValue _mYLocParamA = CreateDefaultYLocParamA();
        private ConvertibleValue _mYLocParamB = CreateDefaultYLocParamB();
        private Enum _mZLocDistribution = EWorkerDistribution.Uniform;
        private ConvertibleValue _mZLocParamA = CreateDefaultZLocParamA();
        private ConvertibleValue _mZLocParamB = CreateDefaultZLocParamB();

        public OccupantDistributionInfo(int numTargets, string description,
            EWorkerDistribution xDistribution, double xParamA, double xParamB,
            EWorkerDistribution yDistribution, double yParamA, double yParamB,
            EWorkerDistribution zDistribution, double zParamA, double zParamB,
            DistanceUnit paramUnitType, double exposureHours)
        {
            NumTargets = numTargets;
            Desc = description;
            XLocDistribution = xDistribution;
            YLocDistribution = yDistribution;
            ZLocDistribution = zDistribution;

            ParamUnitType = paramUnitType;
            XLocParamA = xParamA;
            YLocParamA = yParamA;
            ZLocParamA = zParamA;

            XLocParamB = xParamB;
            YLocParamB = yParamB;
            ZLocParamB = zParamB;

            ExposureHours = exposureHours;
        }

        public OccupantDistributionInfo()
        {
        }

        public int NumTargets
        {
            get => _mNumTargets;
            set
            {
                if (value >= 0) _mNumTargets = value;
            }
        }

        public string Desc
        {
            get => _mDesc;
            set => _mDesc = value;
        }

        public Enum ZLocDistribution
        {
            get
            {
                if (_mZLocDistribution == null) _mZLocDistribution = EWorkerDistribution.Normal;

                return _mZLocDistribution;
            }
            set => _mZLocDistribution = value;
        }

        public Enum XLocDistribution
        {
            get
            {
                if (_mXLocDistribution == null) _mXLocDistribution = EWorkerDistribution.Normal;

                return _mXLocDistribution;
            }
            set => _mXLocDistribution = value;
        }

        public double XLocParamA
        {
            get
            {
                if (_mXLocParamA == null) _mXLocParamA = CreateDefaultXLocParamA();

                if (_mXLocParamA.Converters.HasBadConversionFactor())
                {
                    _mXLocParamA.Converters = StockConverters.DistanceConverter;
                    if (_mXLocParamA.Converters.HasBadConversionFactor())
                        throw new Exception("Bad conversion factor after replacement");
                }

                if (_mXLocParamB.Converters.HasBadConversionFactor())
                    _mXLocParamB.Converters = StockConverters.DistanceConverter;

                if (_mYLocParamA.Converters.HasBadConversionFactor())
                    _mYLocParamA.Converters = StockConverters.DistanceConverter;

                if (_mYLocParamB.Converters.HasBadConversionFactor())
                    _mYLocParamB.Converters = StockConverters.DistanceConverter;

                if (_mZLocParamA.Converters.HasBadConversionFactor())
                    _mZLocParamA.Converters = StockConverters.DistanceConverter;

                if (_mZLocParamB.Converters.HasBadConversionFactor())
                    _mZLocParamB.Converters = StockConverters.DistanceConverter;

                return _mXLocParamA.GetValue(_mParamsUnitType)[0];
            }
            set
            {
                if (value >= 0.0)
                    _mXLocParamA.SetValue(_mParamsUnitType, new[] {value});
                else
                    _mXLocParamA.SetValue(_mParamsUnitType, new[] {0.0});
            }
        }

        public double XLocParamB
        {
            get
            {
                if (_mXLocParamB == null) _mXLocParamB = CreateDefaultXLocParamB();

                return _mXLocParamB.GetValue(_mParamsUnitType)[0];
            }
            set
            {
                if (value >= 0.0)
                    _mXLocParamB.SetValue(_mParamsUnitType, new[] {value});
                else
                    _mXLocParamB.SetValue(_mParamsUnitType, new[] {0.0});
            }
        }

        public Enum YLocDistribution
        {
            get
            {
                if (_mYLocDistribution == null) _mYLocDistribution = EWorkerDistribution.Normal;

                return _mYLocDistribution;
            }
            set => _mYLocDistribution = value;
        }

        public double YLocParamA
        {
            get
            {
                if (_mYLocParamA == null) _mYLocParamA = CreateDefaultYLocParamA();

                return _mYLocParamA.GetValue(_mParamsUnitType)[0];
            }
            set
            {
                if (value >= 0.0)
                    _mYLocParamA.SetValue(_mParamsUnitType, new[] {value});
                else
                    _mYLocParamA.SetValue(_mParamsUnitType, new[] {0.0});
            }
        }

        public double YLocParamB
        {
            get
            {
                if (_mYLocParamB == null) _mYLocParamB = CreateDefaultYLocParamB();

                return _mYLocParamB.GetValue(_mParamsUnitType)[0];
            }
            set
            {
                if (value >= 0.0)
                    _mYLocParamB.SetValue(_mParamsUnitType, new[] {value});
                else
                    _mYLocParamB.SetValue(_mParamsUnitType, new[] {0.0});
            }
        }

        public double ZLocParamA
        {
            get
            {
                if (_mZLocParamA == null) _mZLocParamA = CreateDefaultZLocParamA();

                return _mZLocParamA.GetValue(_mParamsUnitType)[0];
            }
            set
            {
                if (value >= 0.0)
                    _mZLocParamA.SetValue(_mParamsUnitType, new[] {value});
                else
                    _mZLocParamA.SetValue(_mParamsUnitType, new[] {0.0});
            }
        }

        public double ZLocParamB
        {
            get
            {
                if (_mZLocParamB == null) _mZLocParamB = CreateDefaultZLocParamB();

                return _mZLocParamB.GetValue(_mParamsUnitType)[0];
            }
            set
            {
                if (value >= 0.0)
                    _mZLocParamB.SetValue(_mParamsUnitType, new[] {value});
                else
                    _mZLocParamB.SetValue(_mParamsUnitType, new[] {0.0});
            }
        }

        public DistanceUnit ParamUnitType
        {
            get => _mParamsUnitType;
            set => _mParamsUnitType = value;
        }

        public double ExposureHours
        {
            get => _mExposureHours;
            set
            {
                if (value > 8760.0)
                    _mExposureHours = 8760.0;
                else if (value < 0.0)
                    _mExposureHours = 0.0;
                else
                    _mExposureHours = value;
            }
        }

        private static ConvertibleValue CreateDefaultXLocParamA()
        {
            return new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {1.0});
        }

        private static ConvertibleValue CreateDefaultXLocParamB()
        {
            return new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {20.0});
        }

        private static ConvertibleValue CreateDefaultYLocParamA()
        {
            return new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {1.0});
        }

        private static ConvertibleValue CreateDefaultYLocParamB()
        {
            return new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {20.0});
        }


        private static ConvertibleValue CreateDefaultZLocParamA()
        {
            return new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {1.0});
        }


        private static ConvertibleValue CreateDefaultZLocParamB()
        {
            return new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {20.0});
        }


        private static ConvertibleValue CreateDefaultParam()
        {
            return new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {0.0});
        }

        public string GetSimpleString()
        {
            string xDistr = GetDistributionString((EWorkerDistribution)_mXLocDistribution, "X", _mXLocParamA, _mXLocParamB);
            string yDistr = GetDistributionString((EWorkerDistribution)_mYLocDistribution, "Y", _mYLocParamA, _mYLocParamB);
            string zDistr = GetDistributionString((EWorkerDistribution)_mZLocDistribution, "Z", _mZLocParamA, _mZLocParamB);
            string combined = $"{{\"NumTargets\":{_mNumTargets},\"Desc\":\"{_mDesc}\",\"ExposureHours\":{_mExposureHours},{xDistr},{yDistr},{zDistr}}}";
            return combined;
        }

        private string GetDistributionString(EWorkerDistribution distribution, string letter, ConvertibleValue paramA, ConvertibleValue paramB)
        {
            string paramAString = paramA.GetValue(DistanceUnit.Meter)[0].ToString();
            string paramBString = paramB.GetValue(DistanceUnit.Meter)[0].ToString();
            return $"\"{letter}LocDistribution\":{(int)distribution},\"{letter}LocParamA\":{paramAString},\"{letter}LocParamB\":{paramBString}";
        }


        internal string GetPythonVariableDeclaration()
        {
            var xLocDistStr = GetPythonVariableDeclarationForParamXx(XLocDistribution, _mXLocParamA, _mXLocParamB);
            var yLocDistStr = GetPythonVariableDeclarationForParamXx(YLocDistribution, _mYLocParamA, _mYLocParamB);
            var zLocDistStr = GetPythonVariableDeclarationForParamXx(ZLocDistribution, _mZLocParamA, _mZLocParamB);


            var result = "[" + _mNumTargets + ",[" + xLocDistStr + ",[" + yLocDistStr + ",[" + zLocDistStr + "]";

            return result;
        }

        private string GetPythonVariableDeclarationForParamXx(Enum locDistribution, ConvertibleValue locParamA,
            ConvertibleValue locParamB)
        {
            var distributionSelected = (EWorkerDistribution) locDistribution;

            var result = "\"" + distributionSelected.ToString().ToLower() + "\"";
            string paramA = null;
            string paramB = null;

            switch (distributionSelected)
            {
                case EWorkerDistribution.Constant:
                    paramA = locParamA.GetValue(DistanceUnit.Meter)[0].ToString();
                    paramB = "`None`";
                    break;
                case EWorkerDistribution.Normal:
                    paramA = locParamA.GetValue(DistanceUnit.Meter)[0].ToString();
                    paramB = locParamB.GetValue(DistanceUnit.Meter)[0].ToString();
                    break;
                case EWorkerDistribution.Uniform:
                    paramA = locParamA.GetValue(DistanceUnit.Meter)[0].ToString();
                    paramB = locParamB.GetValue(DistanceUnit.Meter)[0].ToString();
                    break;
                default:
                    throw new Exception("Distribution type " + distributionSelected + " is unrecognized.");
            }


            result += "," + paramA;
            result += "," + paramB;
            result += "]";

            return result;
        }
    }

    [Serializable]
    public class OccupantDistributionInfoCollection : List<OccupantDistributionInfo>
    {
        //private string _mVariableName = "occupant_dist_info_cln";
        public OccupantDistributionInfoCollection(bool populateDefaults)
        {
            if (populateDefaults)
            {
                var workersDesc = "Station workers";
                var workers = new OccupantDistributionInfo(9, workersDesc,
                    EWorkerDistribution.Uniform, 1, 20,
                    EWorkerDistribution.Constant, 0, double.NaN, EWorkerDistribution.Uniform, 1, 12,
                    DistanceUnit.Meter, 2000);
                Add(workers);
            }
        }

        public new void Add(OccupantDistributionInfo item)
        {
            if (!Contains(item)) base.Add(item);
        }

        public string GetSimpleString()
        {
            string combined = "[";
            for (int i = 0; i < Count; i++)
            {
                OccupantDistributionInfo occupantSet = this[i];
                if (i > 0)
                {
                    combined += ",";
                }
                combined += $"{occupantSet.GetSimpleString()}";
            }

            combined += "]";
            return combined;
        }
    }
}