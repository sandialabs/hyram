// Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
// Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
// 
// This file is part of HyRAM (Hydrogen Risk Assessment Models).
// 
// HyRAM is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// HyRAM is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

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