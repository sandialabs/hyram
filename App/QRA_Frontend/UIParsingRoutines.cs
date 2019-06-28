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
using System.Windows.Forms;
using DefaultParsing;
using EssStringLib;

namespace QRA_Frontend
{
    public class UiParsingRoutines
    {
        public static double[] ExtractArrayFromTextbox(TextBox tb)
        {
            var sResult = tb.Text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var result = new double[sResult.Length];
            for (var index = 0; index < result.Length; index++)
            {
                var parsedValue = double.NaN;
                var successfullyParsed = Parsing.TryParseDouble(sResult[index], out parsedValue);

                if (successfullyParsed)
                    result[index] = parsedValue;
                else
                    result[index] = double.NaN;
            }

            return result;
        }


        public static string GetDisplayableCommaDelimitedArrayString(double[] arrayWithData)
        {
            return ArrayFunctions.JoinDoublesToDelimitedString(arrayWithData).Replace(",", ", ");
        }


        public static void PutDoubleArrayIntoTextBox(TextBox tb, double[] arrayWithData)
        {
            tb.Text = GetDisplayableCommaDelimitedArrayString(arrayWithData);
        }
    }
}