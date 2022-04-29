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
    [Serializable]
    public class DualConversionProvider
    {
        public ConversionData ConversionObject = null;
        public IISpecialConversionDelegate ConversionDelegate = null;

        public bool HasBadConversionFactor()
        {
            var result = false;
            if (ConversionObject != null) result = ConversionObject.HasBadConversionFactor();

            return result;
        }
    }

    public interface IISpecialConversionDelegate
    {
        double[] ConvertFrom(double[] value);
        double[] ConvertTo(double[] value);
        string GetName();
    }
}