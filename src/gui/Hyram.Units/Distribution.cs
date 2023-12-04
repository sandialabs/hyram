using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandiaNationalLaboratories.Hyram

{
    public enum EDistributionType
    {
        Constant,
        Beta,
        LogNormal,
        ExpectedValue
    }

    public class Distribution
    {
        private double _a;
        private double _b;
        private EDistributionType _dist;

        private const double _tol = 1E-8;

        public EDistributionType Dist
        {
            get => _dist;
            set
            {
                if (Math.Abs(_a) < _tol && (value == EDistributionType.Beta))
                {
                    _a = 1;
                }
                if (Math.Abs(_b) < _tol && (value == EDistributionType.Beta || value == EDistributionType.LogNormal))
                {
                    _b = 1;
                }

                _dist = value;
            }
        }

        public double ParamA
        {
            get => _a;
            set
            {
                if (Dist == EDistributionType.Beta && Math.Abs(value) < _tol)
                {
                    return;
                }

                _a = value;
            }
        }
        public double ParamB
        {
            get => _b;
            set
            {
                if (Dist == EDistributionType.Beta && Math.Abs(value) < _tol)
                {
                    return;
                }

                _b = value;
            }
        }

        public Distribution(EDistributionType dist, double paramA, double paramB)
        {
            Dist = dist;
            ParamA = paramA;
            ParamB = paramB;
        }
    }
}
