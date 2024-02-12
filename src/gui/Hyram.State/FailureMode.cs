/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;


namespace SandiaNationalLaboratories.Hyram
{
    public enum DistributionChoice
    {
        Beta,
        LogNormal,
        ExpectedValue
    }

    /// <summary>
    /// Distribution modeling specified failure mode (e.g. runaway)
    /// </summary>
    public class FailureMode
    {
        private double _a;
        private double _b;
        private DistributionChoice _dist;

        public FailureMode(string name, string mode, DistributionChoice dist, double paramA, double paramB)
        {
            Name = name;
            Mode = mode;
            Dist = dist;
            if ((Dist == DistributionChoice.Beta) && (paramA == 0.0f))
            {
                ParamA = 1.0f;
            }
            else
            {
                ParamA = paramA;
            }

            if (paramB == 0.0f &&
                (Dist == DistributionChoice.Beta || Dist == DistributionChoice.LogNormal)
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

        public DistributionChoice Dist
        {
            get => _dist;
            set
            {
                if (_a == 0.0f &&
                    (value == DistributionChoice.Beta))
                {
                    _a = 1.0f;
                }
                if (_b == 0.0f &&
                    (value == DistributionChoice.Beta || value == DistributionChoice.LogNormal))
                {
                    _b = 1.0f;
                }

                _dist = value;
            }
        }

        public double ParamA
        {
            get => _a;
            set
            {
                if (Dist == DistributionChoice.Beta && value == 0.0f)
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
                if (value == 0.0f &&
                    (Dist == DistributionChoice.Beta || Dist == DistributionChoice.LogNormal))
                {
                    return;
                }
                _b = value;
            }
        }
    }
}
