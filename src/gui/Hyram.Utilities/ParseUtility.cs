/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System.Globalization;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class ParseUtility
    {
        public static bool IsParseableNumber(string textToParse)
        {
            var result = false;
            textToParse = textToParse.Trim();

            if (textToParse.Length > 0) result = double.TryParse(textToParse, out _);

            return result;
        }

        public static void PutDoubleArrayIntoTextBox(TextBox tb, double[] arrayWithData)
        {
            if (arrayWithData.Length == 0)
            {
                return;
            }
            string result = null;
            // string format = "0.####";
            string delimiter = " ";
            foreach (var thisValue in arrayWithData)
            {
                string stringValue = null;
                // stringValue = thisValue.ToString(new CultureInfo("es-ES"));
                stringValue = thisValue.ToString();

                if (result == null)
                    result = stringValue;
                else
                    result += delimiter + stringValue;
            }

            tb.Text = result;
        }

        public static double[] GetArrayFromString(string delimitedString, char delimiter)
        {
            char[] delimiters = {delimiter};
            var values = delimitedString.Split(delimiters);
            var result = new double[values.Length];
            for (var index = 0; index < result.Length; index++)
            {
                result[index] = double.NaN;

                if (double.TryParse(values[index], out double parsedValue))
                {
                    result[index] = parsedValue;
                }
            }

            return result;
        }


    }
}