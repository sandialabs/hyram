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
using System.Globalization;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class ParseUtility
    {
        public static string DoubleToString(double value, string fmt = null)
        {
            string result = null;
            if (fmt == null)
                result = value.ToString(new CultureInfo("en-US"));
            else
                result = value.ToString(fmt, new CultureInfo("en-US"));

            return result;
        }

        public static bool TryParseDouble(string value, out double result,
            NumberStyles floatParseStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent |
                                            NumberStyles.AllowLeadingSign, CultureInfo cultureInfo = null)
        {
            value = value.Trim();

            result = Double.NaN;
            var bResult = false;

            if (cultureInfo == null) cultureInfo = new CultureInfo("en-US");

            bResult = Double.TryParse(value, floatParseStyles, cultureInfo, out result);
            if (!bResult) bResult = Double.TryParse(value, out result);

            return bResult;
        }

        public static bool IsParseableNumber(string textToParse)
        {
            var result = false;
            textToParse = textToParse.Trim();

            if (textToParse.Length > 0) result = TryParseDouble(textToParse, out _);

            return result;
        }

        public static void PutDoubleArrayIntoTextBox(TextBox tb, double[] arrayWithData)
        {
            string result = null;
            string format = "0.####";
            string delimiter = ",";
            foreach (var thisValue in arrayWithData)
            {
                string stringValue = null;
                stringValue = thisValue.ToString(format, new CultureInfo("en-US"));

                if (result == null)
                    result = stringValue;
                else
                    result += delimiter + stringValue;
            }

            tb.Text = result.Replace(",", ", ");
        }
    }
}