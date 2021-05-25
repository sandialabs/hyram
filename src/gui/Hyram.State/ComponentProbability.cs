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
using MathNet.Numerics.Distributions;

namespace SandiaNationalLaboratories.Hyram
{

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

    [Serializable]
    public class ComponentProbabilitySet
    {
        public List<ComponentProbability> Compressor;
        public List<ComponentProbability> Cylinder;
        public List<ComponentProbability> Filter;
        public List<ComponentProbability> Flange;
        public List<ComponentProbability> Hose;
        public List<ComponentProbability> Joint;
        public List<ComponentProbability> Pipe;
        public List<ComponentProbability> Valve;
        public List<ComponentProbability> Instrument;
        public List<ComponentProbability> Extra1;
        public List<ComponentProbability> Extra2;
    }

    public static class FuelData
    {
        public static ComponentProbabilitySet H2LeakFrequencies = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -1.73, 0.22),
                new ComponentProbability("0.10%", -3.95, 0.50),
                new ComponentProbability("1%", -5.16, 0.8),
                new ComponentProbability("10%", -8.84, 0.84),
                new ComponentProbability("100%", -11.34, 1.37),
            },

            Cylinder = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -13.92, 0.67),
                new ComponentProbability("0.10%", -14.06, 0.65),
                new ComponentProbability("1%", -14.44, 0.65),
                new ComponentProbability("10%", -14.99, 0.65),
                new ComponentProbability("100%", -15.62, 0.68),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.25, 1.99),
                new ComponentProbability("0.10%", -5.29, 1.52),
                new ComponentProbability("1%", -5.34, 1.48),
                new ComponentProbability("10%", -5.38, 0.89),
                new ComponentProbability("100%", -5.43, 0.95),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -3.92, 1.66),
                new ComponentProbability("0.10%", -6.12, 1.25),
                new ComponentProbability("1%", -8.33, 2.20),
                new ComponentProbability("10%", -10.54, 0.83),
                new ComponentProbability("100%", -12.75, 1.83),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -6.83, 0.28),
                new ComponentProbability("0.10%", -8.73, 0.61),
                new ComponentProbability("1%", -8.85, 0.59),
                new ComponentProbability("10%", -8.96, 0.59),
                new ComponentProbability("100%", -9.91, 0.88),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -9.58, 0.17),
                new ComponentProbability("0.10%", -12.92, 0.81),
                new ComponentProbability("1%", -11.93, 0.51),
                new ComponentProbability("10%", -12.09, 0.58),
                new ComponentProbability("100%", -12.22, 0.61),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -11.91, 0.69),
                new ComponentProbability("0.10%", -12.57, 0.71),
                new ComponentProbability("1%", -13.88, 1.14),
                new ComponentProbability("10%", -14.59, 1.16),
                new ComponentProbability("100%", -15.73, 1.72),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.19, 0.18),
                new ComponentProbability("0.10%", -7.31, 0.42),
                new ComponentProbability("1%", -9.71, 0.98),
                new ComponentProbability("10%", -10.34, 0.69),
                new ComponentProbability("100%", -12.0, 1.33),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -7.38, 0.71),
                new ComponentProbability("0.10%", -8.54, 0.82),
                new ComponentProbability("1%", -9.10, 0.92),
                new ComponentProbability("10%", -9.21, 1.09),
                new ComponentProbability("100%", -10.21, 1.49),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            }
        };

        // TODO: update these
        public static ComponentProbabilitySet CngLeakFrequencies = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -1.7, 0.2),
                new ComponentProbability("0.10%", -3.9, 0.50),
                new ComponentProbability("1%", -5.16, 0.8),
                new ComponentProbability("10%", -8.84, 0.84),
                new ComponentProbability("100%", -11.34, 1.37),
            },

            Cylinder = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -13.9, 0.67),
                new ComponentProbability("0.10%", -14.0, 0.65),
                new ComponentProbability("1%", -14.4, 0.65),
                new ComponentProbability("10%", -14.9, 0.65),
                new ComponentProbability("100%", -15.6, 0.68),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.25, 1.99),
                new ComponentProbability("0.10%", -5.29, 1.52),
                new ComponentProbability("1%", -5.34, 1.48),
                new ComponentProbability("10%", -5.38, 0.89),
                new ComponentProbability("100%", -5.43, 0.95),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -3.92, 1.66),
                new ComponentProbability("0.10%", -6.12, 1.25),
                new ComponentProbability("1%", -8.33, 2.20),
                new ComponentProbability("10%", -10.54, 0.83),
                new ComponentProbability("100%", -12.75, 1.83),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -6.83, 0.28),
                new ComponentProbability("0.10%", -8.73, 0.61),
                new ComponentProbability("1%", -8.85, 0.59),
                new ComponentProbability("10%", -8.96, 0.59),
                new ComponentProbability("100%", -9.91, 0.88),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -9.58, 0.17),
                new ComponentProbability("0.10%", -12.92, 0.81),
                new ComponentProbability("1%", -11.93, 0.51),
                new ComponentProbability("10%", -12.09, 0.58),
                new ComponentProbability("100%", -12.22, 0.61),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -11.91, 0.69),
                new ComponentProbability("0.10%", -12.57, 0.71),
                new ComponentProbability("1%", -13.88, 1.14),
                new ComponentProbability("10%", -14.59, 1.16),
                new ComponentProbability("100%", -15.73, 1.72),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.19, 0.18),
                new ComponentProbability("0.10%", -7.31, 0.42),
                new ComponentProbability("1%", -9.71, 0.98),
                new ComponentProbability("10%", -10.34, 0.69),
                new ComponentProbability("100%", -12.0, 1.33),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -7.38, 0.71),
                new ComponentProbability("0.10%", -8.54, 0.82),
                new ComponentProbability("1%", -9.10, 0.92),
                new ComponentProbability("10%", -9.21, 1.09),
                new ComponentProbability("100%", -10.21, 1.49),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            }
        };
    }

}
