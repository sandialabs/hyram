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
using DefaultParsing;

namespace EssStringLib
{
    /// <summary>
    ///     Summary description for MiscFunctions.
    /// </summary>
    public static class MiscFunctions
    {
        /// <summary>
        ///     Create a string containing a GUID.
        /// </summary>
        /// <returns></returns>
        public static string CreateGuid()
        {
            return Guid.NewGuid().ToString();
        }

        public static string CreateStrippedGuid()
        {
            var result = CreateGuid();
            var charsToRemove = "{}-";

            result = StringFunctions.RemoveCharsFromString(result, charsToRemove.ToCharArray());

            return result;
        }

        /// <summary>
        ///     Sets background color for an HTML document.  Currently only works for documents containing
        ///     "background-color: rgb(".  If needed, will be updated in the future to support
        ///     "bkcolor", and/or to support documents for which no background color is specified.
        /// </summary>
        /// <param name="documentText"></param>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static string SetHtmlDocTextBackgroundColor(string documentText, int red, int green, int blue)
        {
            string result = null;
            var strToFind = "BACKGROUND-COLOR: RGB(";
            var ucDocText = documentText.ToUpper();


            if (ucDocText.Contains(strToFind))
            {
                var startPos = ucDocText.IndexOf(strToFind);
                if (startPos > 0)
                {
                    var leftPart = documentText.Substring(0, startPos + strToFind.Length);
                    var endPos = documentText.IndexOf(')', startPos + 1);
                    if (endPos > startPos)
                    {
                        var rightPart = documentText.Substring(endPos, documentText.Length - endPos);
                        if (rightPart.Substring(0, 1) != ")")
                            throw new Exception("Substring math is off in SetHtmlDocTextBackgroundColor.");

                        result = leftPart + red + ", " + green + ", " + blue + rightPart;
                    }
                }
            }

            return result;
        }

        public static bool IsParseableNumber(string textToParse)
        {
            double num;
            var result = false;
            textToParse = textToParse.Trim();

            if (textToParse.Length > 0) result = Parsing.TryParseDouble(textToParse, out num);

            return result;
        }
    }
}