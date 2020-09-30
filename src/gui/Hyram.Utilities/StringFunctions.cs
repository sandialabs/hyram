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
using System.Text;

namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    ///     Class containing useful string functions.
    /// </summary>
    public static class StringFunctions
    {
        /// <summary>
        ///     Copies a subarray from SourceArray.
        /// </summary>
        /// <param name="sourceArray">Array to copy from.</param>
        /// <param name="startPos">Index at which copying begins.</param>
        /// <param name="numLines">Number of lines to copy.</param>
        /// <returns>New array.</returns>
        private static string[] GetSubArray(string[] sourceArray, int startPos, int numLines)
        {
            var result = new string[numLines];

            var destIndex = 0;
            var sourceIndex = startPos;
            for (var index = 0; index < numLines; index++)
            {
                result[destIndex] = sourceArray[sourceIndex];
                destIndex++;
                sourceIndex++;
            }

            return result;
        }

        /// <summary>
        ///     Inserts an array into another array.
        /// </summary>
        /// <param name="source">Original array.</param>
        /// <param name="dataToInsert">Array containing extra data to add.</param>
        /// <param name="position">Position at which to perform insertion.</param>
        /// <returns>Combined array.</returns>
        public static string[] InsertDataIntoArray(string[] source, string[] dataToInsert, int position)
        {
            var priorLines = GetSubArray(source, 0, position);
            var afterLines = new string[0];
            if (source.Length - position > 0) afterLines = GetSubArray(source, position, source.Length - position);

            if (priorLines.Length + afterLines.Length != source.Length)
                throw new Exception(
                    "InsertDataIntoArray function computed bad number of elements or position incorrectly. This is a software defect.  Please notify your Technical Supportperson.");

            var result = new EditableStringArray(priorLines);
            result.Append(dataToInsert);
            result.Append(afterLines);

            return result.Data;
        }

        /// <summary>
        ///     Returns whitespace-delimited word found in Line at Position.
        /// </summary>
        /// <param name="line">Line to extract word from.</param>
        /// <param name="position">Position at which to perform expansion.</param>
        /// <returns>Word found.</returns>
        public static string ExpandWordLeft(string line, int position)
        {
            var result = "";

            if (position == 0)
            {
                result = "";
            }
            else
            {
                var currentPos = position;

                while (currentPos >= 0)
                {
                    var thisChar = line.Substring(currentPos, 1).ToCharArray(0, 1)[0];
                    if (IsSpace(thisChar))
                        break;
                    result = thisChar + result;

                    currentPos--;
                }
            }

            return result;
        }

        /// <summary>
        ///     Determines whether character is a carriage-return or space.
        /// </summary>
        /// <param name="thisChar">Character to test.</param>
        /// <returns>True if character is a carriage return or space, false otherwise.</returns>
        public static bool IsSpace(char thisChar)
        {
            var asciiCode = (int) thisChar;
            bool result;

            if (asciiCode > 13 && asciiCode != 32)
                result = false;
            else
                result = true;

            return result;
        }

        /// <summary>
        ///     Returns whitespace-delimited word found in Line at Position.
        /// </summary>
        /// <param name="line">Line to extract word from.</param>
        /// <param name="position">Position at which to perform expansion.</param>
        /// <returns>Word found.</returns>
        public static string ExpandWordRight(string line, int position)
        {
            var result = "";
            var restOfLine = line.ToCharArray(position, line.Length - (position + 1));
            foreach (var thisChar in restOfLine)
                if (!IsSpace(thisChar))
                    result = result + thisChar;
                else
                    break;

            return result;
        }

        /// <summary>
        ///     Tests whether a string starts with a specific value.
        /// </summary>
        /// <param name="stringToSearch">String to search.</param>
        /// <param name="stringToLookFor">Substring to find.</param>
        /// <param name="stringFound">Value found.</param>
        /// <returns>True if match exists, false otherwise.</returns>
        public static bool StartsWith(string stringToSearch, string stringToLookFor, ref string stringFound)
        {
            var result = false;

            var substitution = stringToLookFor.IndexOf("%s");
            var searchLen = stringToSearch.Length;
            var lookForLen = stringToLookFor.Length;

            if (searchLen >= lookForLen)
            {
                if (substitution == 0)
                {
                    var thisStringFound = ExpandToWord(stringToSearch, 0);
                    stringToSearch = RightStr(stringToSearch, thisStringFound);

                    stringToLookFor = stringToLookFor.Substring(substitution + 2);
                    result = StartsWith(stringToSearch, stringToLookFor, ref stringFound);
                    if (result) stringFound = thisStringFound + stringFound;
                }
                else
                {
                    if (stringToSearch.IndexOf(stringToLookFor) == 0)
                    {
                        result = true;
                        stringFound = stringToLookFor;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Return word (succession of non-space characters) found in Line at Position.
        /// </summary>
        /// <param name="line">Line to extract word from.</param>
        /// <param name="position">Position in line containing word to extract.</param>
        /// <returns>Extracted word.</returns>
        public static string ExpandToWord(string line, int position)
        {
            var result = line.Substring(position, 1);

            if (line.Length > position + 1)
            {
                result = result + ExpandWordRight(line, position + 1);
                result = ExpandWordLeft(line, position - 1) + result;
            }

            return result;
        }

        /// <summary>
        ///     Reverses each character in a string.
        /// </summary>
        /// <param name="sourceStr">String to reverse.</param>
        /// <returns>String containing characters is reverse order.</returns>
        public static string ReverseStr(string sourceStr)
        {
            var index = sourceStr.Length - 1;
            var resultBuilder = new StringBuilder(sourceStr);
            var sourceBuilder = new StringBuilder(sourceStr);


            var destIndex = 0;
            while (index > -1)
            {
                resultBuilder[destIndex] = sourceBuilder[index];
                destIndex++;
                index--;
            }

            return resultBuilder.ToString();
        }

        /// <summary>
        ///     Remove the leftmost n characters from a string, where n is the length of the second argument.
        /// </summary>
        /// <param name="sourceStr">String to shorten.</param>
        /// <param name="leftPartOfString">Part of string to remove.</param>
        /// <returns>Truncated right portion of string</returns>
        public static string RightStr(string sourceStr, string leftPartOfString)
        {
            var result = RightStr(sourceStr, sourceStr.Length - leftPartOfString.Length);
            return result;
        }


        /// <summary>
        ///     Get the rightmost NumBytes characters in a string.
        /// </summary>
        /// <param name="sourceStr">String to truncate.</param>
        /// <param name="numBytes">Number of bytes to extract.</param>
        /// <returns>Right[NumBytes] of string.</returns>
        public static string RightStr(string sourceStr, int numBytes)
        {
            var source = ReverseStr(sourceStr);
            var result = ReverseStr(source.Substring(0, numBytes));
            return result;
        }

        /// <summary>
        ///     Find out whether string contains value.
        /// </summary>
        /// <param name="strToCheck">String to search.</param>
        /// <param name="valueToLookFor">Value to find.</param>
        /// <param name="caseSensitive">Whether to perform case-sensitive search.</param>
        /// <returns>True or false.</returns>
        public static bool StringContainsValue(string strToCheck, string valueToLookFor, bool caseSensitive)
        {
            var result = false;

            if (caseSensitive)
            {
                if (strToCheck.IndexOf(valueToLookFor) > -1) result = true;
            }
            else
            {
                result = StringContainsValue(strToCheck.ToUpper(), valueToLookFor.ToUpper(), true);
            }

            return result;
        }

        /// <summary>
        ///     Get a string containing specified number of spaces.
        /// </summary>
        /// <param name="numSpaces">Number of spaces desired.</param>
        /// <returns>1 or more spaces.</returns>
        public static string Spaces(int numSpaces)
        {
            return Chars(' ', numSpaces);
        }

        /// <summary>
        ///     Get a string containing the number of characters specified.
        /// </summary>
        /// <param name="character">Character wanted.</param>
        /// <param name="numChars">Number of that character.</param>
        /// <returns>A string containing the characters specified.</returns>
        public static string Chars(char character, int numChars)
        {
            return new string(character, numChars);
        }

        /// <summary>
        ///     Chop off last character from a string if its last value is the character specified.
        /// </summary>
        /// <param name="source">String to chop.</param>
        /// <param name="ifLastValueIs">Last character value.</param>
        /// <returns>Modified string.</returns>
        public static string ConditionalChop(string source, char ifLastValueIs)
        {
            string result = null;

            if (source.Length > 0)
            {
                var thisCh = source[source.Length - 1];
                if (thisCh == ifLastValueIs)
                    result = source.Substring(0, source.Length - 1);
                else
                    result = source;
            }

            if (result == null) result = source;

            return result;
        }
    }
}