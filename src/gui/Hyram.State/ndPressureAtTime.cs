/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;

namespace SandiaNationalLaboratories.Hyram
{
    // Used to enable correct wrapper generation for time/pressure nodes for sending
    // to Overpressure plotting routines.
    [Serializable]
    public class NdPressureAtTime
    {
        private double _mPressure = double.NaN;
        private double _mTime = double.NaN;


        public NdPressureAtTime(double time, double pressure)
        {
            _mTime = time;
            _mPressure = pressure;
        }

        public double Time
        {
            get => _mTime;
            set => _mTime = value;
        }

        public double Pressure
        {
            get => _mPressure;
            set => _mPressure = value;
        }
    }
}