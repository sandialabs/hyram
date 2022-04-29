/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MathNet.Numerics.Distributions;

namespace SandiaNationalLaboratories.Hyram
{

    /// <summary>
    /// Generic probability representation used by components, failure modes.
    /// </summary>
    [Serializable]
    public class ComponentProbability : INotifyPropertyChanged
    {
        private double _mu;
        private double _sigma;
        private double _mean;
        private double _fifth;
        private double _median;
        private double _ninetyFifth;
        private bool _updateFromMedian = true;

        public string LeakSize { get; set; }

        public double Mu
        {
            get => _mu;
            set
            {
                _mu = value;
                UpdateDistributionParameters();
                NotifyPropertyChanged();
            }
        }

        public double Sigma
        {
            get => _sigma;
            set
            {
                _sigma = value;
                UpdateDistributionParameters();
                NotifyPropertyChanged();
            }
        }

        public double Mean 
        {
            get => _mean;
            set
            {
                _mean = value;
                NotifyPropertyChanged();
            }
        }

        public double Fifth 
        {
            get => _fifth;
            set
            {
                _fifth = value;
                NotifyPropertyChanged();
            }
        }

        public double Median
        {
            get => _median;
            set
            {
                _median = value;
                if (_updateFromMedian) Mu = Math.Log(_median);
                NotifyPropertyChanged();
            }
        }

        public double NinetyFifth 
        {
            get => _ninetyFifth;
            set
            {
                _ninetyFifth = value;
                NotifyPropertyChanged();
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ComponentProbability(string leakSize, double mu, double sigma)
        {
            LeakSize = leakSize;
            Mu = mu;
            Sigma = sigma;
        }

        public void SetParameters(double mu, double sigma)
        {
            _mu = mu;
            _sigma = sigma;
            UpdateDistributionParameters();
            NotifyPropertyChanged();
        }

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
        public List<ComponentProbability> Vessel;
        public List<ComponentProbability> Filter;
        public List<ComponentProbability> Flange;
        public List<ComponentProbability> Hose;
        public List<ComponentProbability> Joint;
        public List<ComponentProbability> Pipe;
        public List<ComponentProbability> Valve;
        public List<ComponentProbability> Instrument;
        public List<ComponentProbability> HeatExchanger;
        public List<ComponentProbability> Vaporizer;
        public List<ComponentProbability> LoadingArm;
        public List<ComponentProbability> Extra1;
        public List<ComponentProbability> Extra2;
    }

    public class FuelData
    {

        public static readonly ComponentProbabilitySet H2GasLeaks = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -2.3053, 0.3018),
                new ComponentProbability("0.10%", -4.0761,0.5249),
                new ComponentProbability("1%", -5.3881, 0.8212),
                new ComponentProbability("10%",-8.7929, 0.7263),
                new ComponentProbability("100%",-11.1359, 1.2067),
            },

            Vessel = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -13.4770, 0.7347),
                new ComponentProbability("0.10%", -13.6410, 0.6387),
                new ComponentProbability("1%", -14.0512,0.6203),
                new ComponentProbability("10%", -14.6144,0.6041),
                new ComponentProbability("100%", -15.2732,0.6156),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.2348,1.6955),
                new ComponentProbability("0.10%", -5.2822,1.2885),
                new ComponentProbability("1%", -5.3303,1.2879),
                new ComponentProbability("10%", -5.3798,0.7448),
                new ComponentProbability("100%", -5.4288,0.8171)
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -3.9125,1.4920),
                new ComponentProbability("0.10%", -6.1191,1.1345),
                new ComponentProbability("1%", -8.3252,2.0541),
                new ComponentProbability("10%", -10.5327,0.7208),
                new ComponentProbability("100%", -12.7385,1.6925),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-7.4534,0.3863),
                new ComponentProbability("0.10%",-8.5011,0.6050),
                new ComponentProbability("1%",-8.7103,0.6106),
                new ComponentProbability("10%",-8.8001,0.5901),
                new ComponentProbability("100%",-9.6934,0.9836),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-10.2591,0.2423),
                new ComponentProbability("0.10%",-12.2703,0.8727),
                new ComponentProbability("1%",-11.7538,0.5333),
                new ComponentProbability("10%",-11.7961,0.6073),
                new ComponentProbability("100%",-11.9590,0.6600),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-11.7331,0.7162),
                new ComponentProbability("0.10%",-12.5079,0.7070),
                new ComponentProbability("1%",-13.8601,1.2515),
                new ComponentProbability("10%",-14.5893,1.1933),
                new ComponentProbability("100%",-15.7354,1.8486),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-5.8546,0.2500),
                new ComponentProbability("0.10%",-7.4425,0.4344),
                new ComponentProbability("1%",-9.8190,1.1434),
                new ComponentProbability("10%",-10.6079,0.6270),
                new ComponentProbability("100%",-12.2436,1.3690),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-7.3800,0.7100),
                new ComponentProbability("0.10%",-8.5400,0.8500),
                new ComponentProbability("1%",-9.1000,0.9200),
                new ComponentProbability("10%",-9.2100,1.0900),
                new ComponentProbability("100%",-10.2100,1.4900),
            },

            HeatExchanger = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Vaporizer = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            LoadingArm = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            }
        };

        public static readonly ComponentProbabilitySet H2LiquidLeaks = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Vessel = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -7.3426,1.7799),
                new ComponentProbability("0.10%", -8.8915,2.5535),
                new ComponentProbability("1%", -10.4746,2.052),
                new ComponentProbability("10%",-12.0829,2.7255),
                new ComponentProbability("100%",-13.6552,3.1272),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-3.9125,1.4920),
                new ComponentProbability("0.10%",-6.1191,1.1345),
                new ComponentProbability("1%",-8.3252,2.0541),
                new ComponentProbability("10%",-10.5327,0.7208),
                new ComponentProbability("100%",-12.7385,1.6925),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-7.4534,0.3863),
                new ComponentProbability("0.10%",-8.5011,0.6050),
                new ComponentProbability("1%",-8.7103,0.6106),
                new ComponentProbability("10%",-8.8001,0.5901),
                new ComponentProbability("100%",-9.6934,0.9836)
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-10.2591,0.2423),
                new ComponentProbability("0.10%",-12.2703,0.8727),
                new ComponentProbability("1%",-11.7538,0.5333),
                new ComponentProbability("10%",-11.7961,0.6073),
                new ComponentProbability("100%",-11.9590,0.6600),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-11.7331,0.7162),
                new ComponentProbability("0.10%",-12.5079,0.7070),
                new ComponentProbability("1%",-13.8601,1.2515),
                new ComponentProbability("10%",-14.5893,1.1933),
                new ComponentProbability("100%",-15.7354,1.8486),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-5.8546,0.2500),
                new ComponentProbability("0.10%",-7.4425,0.4344),
                new ComponentProbability("1%",-9.8190,1.1434),
                new ComponentProbability("10%",-10.6079,0.6270),
                new ComponentProbability("100%",-12.2436,1.3690),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            HeatExchanger = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Vaporizer = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            LoadingArm = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            }
        };

        public static readonly ComponentProbabilitySet MethaneGasLeaks = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",0.7785,1.3159),
                new ComponentProbability("0.10%",-2.2394,0.9997),
                new ComponentProbability("1%",-5.2560,1.0068),
                new ComponentProbability("10%",-8.2731,0.7015),
                new ComponentProbability("100%",-11.2905,1.2350),
            },

            Vessel = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-0.4139,1.3445),
                new ComponentProbability("0.10%",-3.8954,1.0446),
                new ComponentProbability("1%",-7.3613,0.8054),
                new ComponentProbability("10%",-10.8805,0.6776),
                new ComponentProbability("100%",-14.3160,0.6934),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-5.2348,1.6955),
                new ComponentProbability("0.10%",-5.2822,1.2885),
                new ComponentProbability("1%",-5.3303,1.2879),
                new ComponentProbability("10%",-5.3798,0.7448),
                new ComponentProbability("100%",-5.4288,0.8171
),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-3.9125,1.4920),
                new ComponentProbability("0.10%",-6.1191,1.1345),
                new ComponentProbability("1%",-8.3252,2.0541),
                new ComponentProbability("10%",-10.5327,0.7208),
                new ComponentProbability("100%",-12.7385,1.6925),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",2.5434,1.2507),
                new ComponentProbability("0.10%",0.3455,0.9447),
                new ComponentProbability("1%",-1.8439,0.7670),
                new ComponentProbability("10%",-4.0745,0.6712),
                new ComponentProbability("100%",-6.2263,1.3906),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-0.6255,1.2727),
                new ComponentProbability("0.10%",-2.3062,0.9835),
                new ComponentProbability("1%",-4.0101,0.9541),
                new ComponentProbability("10%",-5.6481,0.5745),
                new ComponentProbability("100%",-7.3739,0.6569),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-7.9272,0.9882),
                new ComponentProbability("0.10%",-9.6818,0.7884),
                new ComponentProbability("1%",-11.4317,1.5059),
                new ComponentProbability("10%",-13.2003,1.2938),
                new ComponentProbability("100%",-14.9400,2.1066),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-4.4577,1.0301),
                new ComponentProbability("0.10%",-6.2603,0.8332),
                new ComponentProbability("1%",-8.0664,1.5396),
                new ComponentProbability("10%",-9.8503,0.6458),
                new ComponentProbability("100%",-11.6738,1.4350),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            HeatExchanger = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Vaporizer = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            LoadingArm = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            }
        };

        public static readonly ComponentProbabilitySet MethaneLiquidLeaks = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Vessel = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-7.6443,1.1463),
                new ComponentProbability("0.10%",-8.8817,2.2191),
                new ComponentProbability("1%",-10.1486,1.9222),
                new ComponentProbability("10%",-11.4202,2.4219),
                new ComponentProbability("100%",-12.6988,3.1816),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-10.0826,0.7306),
                new ComponentProbability("0.10%",-10.6980,1.2105),
                new ComponentProbability("1%",-11.1787,2.3964),
                new ComponentProbability("10%",-11.6591,2.7634),
                new ComponentProbability("100%",-12.1601,2.9150),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-13.3971,0.7419),
                new ComponentProbability("0.10%",-11.7497,0.5645),
                new ComponentProbability("1%",-10.0943,4.1882),
                new ComponentProbability("10%",-8.4524,0.9343),
                new ComponentProbability("100%",-6.8075,3.6381),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",10.4669,2.1960),
                new ComponentProbability("0.10%",6.1666,1.6594),
                new ComponentProbability("1%",1.8652,1.1464),
                new ComponentProbability("10%",-2.4345,0.7068),
                new ComponentProbability("100%",-6.7363,0.6198),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-12.8352,1.3156),
                new ComponentProbability("0.10%",-13.4479,1.4332),
                new ComponentProbability("1%",-14.0571,1.1603),
                new ComponentProbability("10%",-14.6718,1.3553),
                new ComponentProbability("100%",-15.2867,1.8314),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-9.3821,0.7206),
                new ComponentProbability("0.10%",-10.0776,0.9821),
                new ComponentProbability("1%",-10.7378,1.1534),
                new ComponentProbability("10%",-11.3422,1.9367),
                new ComponentProbability("100%",-11.9495,1.9345),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            HeatExchanger = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-6.0563,0.9571),
                new ComponentProbability("0.10%",-7.0191,1.3035),
                new ComponentProbability("1%",-8.0347,1.4167),
                new ComponentProbability("10%",-9.0535,2.3068),
                new ComponentProbability("100%",-10.0834,1.6190),
            },

            Vaporizer = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-4.8184,2.5489),
                new ComponentProbability("0.10%",-3.6464,1.8695),
                new ComponentProbability("1%",-2.4760,1.2243),
                new ComponentProbability("10%",-1.3037,0.7074),
                new ComponentProbability("100%",-0.1328,0.7068),
            },

            LoadingArm = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-1.6169,3.0126),
                new ComponentProbability("0.10%",-4.3987,2.0841),
                new ComponentProbability("1%",-7.2019,1.8945),
                new ComponentProbability("10%",-10.3173,1.0339),
                new ComponentProbability("100%",-12.7037,3.3875),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            }
        };

        public static readonly ComponentProbabilitySet PropaneGasLeaks = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",0.7785,1.3159),
                new ComponentProbability("0.10%",-2.2394,0.9997),
                new ComponentProbability("1%",-5.2560,1.0068),
                new ComponentProbability("10%",-8.2731,0.7015),
                new ComponentProbability("100%",-11.2905,1.2350),
            },

            Vessel = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-0.4139,1.3445),
                new ComponentProbability("0.10%",-3.8954,1.0446),
                new ComponentProbability("1%",-7.3613,0.8054),
                new ComponentProbability("10%",-10.8805,0.6776),
                new ComponentProbability("100%",-14.3160,0.6934),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-5.2348,1.6955),
                new ComponentProbability("0.10%",-5.2822,1.2885),
                new ComponentProbability("1%",-5.3303,1.2879),
                new ComponentProbability("10%",-5.3798,0.7448),
                new ComponentProbability("100%",-5.4288,0.8171),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-3.9125,1.4920),
                new ComponentProbability("0.10%",-6.1191,1.1345),
                new ComponentProbability("1%",-8.3252,2.0541),
                new ComponentProbability("10%",-10.5327,0.7208),
                new ComponentProbability("100%",-12.7385,1.6925),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",2.5434,1.2507),
                new ComponentProbability("0.10%",0.3455,0.9447),
                new ComponentProbability("1%",-1.8439,0.7670),
                new ComponentProbability("10%",-4.0745,0.6712),
                new ComponentProbability("100%",-6.2263,1.3906),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-0.6255,1.2727),
                new ComponentProbability("0.10%",-2.3062,0.9835),
                new ComponentProbability("1%",-4.0101,0.9541),
                new ComponentProbability("10%",-5.6481,0.5745),
                new ComponentProbability("100%",-7.3739,0.6569),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-7.9272,0.9882),
                new ComponentProbability("0.10%",-9.6818,0.7884),
                new ComponentProbability("1%",-11.4317,1.5059),
                new ComponentProbability("10%",-13.2003,1.2938),
                new ComponentProbability("100%",-14.9400,2.1066),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%",-4.4577,1.0301),
                new ComponentProbability("0.10%",-6.2603,0.8332),
                new ComponentProbability("1%",-8.0664,1.5396),
                new ComponentProbability("10%",-9.8503,0.6458),
                new ComponentProbability("100%",-11.6738,1.4350),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            HeatExchanger = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Vaporizer = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            LoadingArm = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            }
        };

        public static readonly ComponentProbabilitySet PropaneLiquidLeaks = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0.7785, 1.3159),
                new ComponentProbability("0.10%", -2.2394, 0.9997),
                new ComponentProbability("1%", -5.2560, 1.0068),
                new ComponentProbability("10%", -8.2731, 0.7015),
                new ComponentProbability("100%", -11.2905, 1.2350),
            },

            Vessel = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -0.4139, 1.3445),
                new ComponentProbability("0.10%", -3.8954, 1.0446),
                new ComponentProbability("1%", -7.3613, 0.8054),
                new ComponentProbability("10%", -10.8805, 0.6776),
                new ComponentProbability("100%", -14.3160, 0.6934),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -5.2348, 1.6955),
                new ComponentProbability("0.10%", -5.2822, 1.2885),
                new ComponentProbability("1%", -5.3303, 1.2879),
                new ComponentProbability("10%", -5.3798, 0.7448),
                new ComponentProbability("100%", -5.4288, 0.8171),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -3.9125, 1.4920),
                new ComponentProbability("0.10%", -6.1191, 1.1345),
                new ComponentProbability("1%", -8.3252, 2.0541),
                new ComponentProbability("10%", -10.5327, 0.7208),
                new ComponentProbability("100%", -12.7385, 1.6925),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 2.5434, 1.2507),
                new ComponentProbability("0.10%", 0.3455, 0.9447),
                new ComponentProbability("1%", -1.8439, 0.7670),
                new ComponentProbability("10%", -4.0745, 0.6712),
                new ComponentProbability("100%", -6.2263, 1.3906),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -0.6255, 1.2727),
                new ComponentProbability("0.10%", -2.3062, 0.9835),
                new ComponentProbability("1%", -4.0101, 0.9541),
                new ComponentProbability("10%", -5.6481, 0.5745),
                new ComponentProbability("100%", -7.3739, 0.6569),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -7.9272, 0.9882),
                new ComponentProbability("0.10%", -9.6818, 0.7884),
                new ComponentProbability("1%", -11.4317, 1.5059),
                new ComponentProbability("10%", -13.2003, 1.2938),
                new ComponentProbability("100%", -14.9400, 2.1066),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", -4.4577, 1.0301),
                new ComponentProbability("0.10%", -6.2603, 0.8332),
                new ComponentProbability("1%", -8.0664, 1.5396),
                new ComponentProbability("10%", -9.8503, 0.6458),
                new ComponentProbability("100%", -11.6738, 1.4350),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            HeatExchanger = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Vaporizer = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            LoadingArm = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra1 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            },

            Extra2 = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 999, 999),
                new ComponentProbability("0.10%", 999, 999),
                new ComponentProbability("1%", 999, 999),
                new ComponentProbability("10%", 999, 999),
                new ComponentProbability("100%", 999, 999),
            }
        };

        public static readonly ComponentProbabilitySet ActiveLeakSet = new ComponentProbabilitySet
        {
            Compressor = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Vessel = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Filter = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Flange = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Hose = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Joint = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Pipe = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },
            Valve = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Instrument = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            HeatExchanger = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            Vaporizer = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
            },

            LoadingArm = new List<ComponentProbability>
            {
                new ComponentProbability("0.01%", 0, 0),
                new ComponentProbability("0.10%", 0, 0),
                new ComponentProbability("1%", 0, 0),
                new ComponentProbability("10%", 0, 0),
                new ComponentProbability("100%", 0, 0),
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
