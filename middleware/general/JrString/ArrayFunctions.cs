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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DefaultParsing;

namespace EssStringLib
{
    public static class ArrayFunctions
    {
        public static string[] IntsToStrings(int[] source)
        {
            var result = new string[source.Length];
            for (var index = 0; index < source.Length; index++) result[index] = source[index].ToString();

            return result;
        }

        public static string[] RemoveEmptyStrings(string[] originalArray)
        {
            var result = new StringArrayList();
            foreach (var thisString in originalArray)
            {
                var trimmedString = thisString.Trim();

                if (trimmedString.Length > 0) result.Add(thisString);
            }

            return result.ToStringArray();
        }

        public static ArrayList GetArrayList(string[] sourceStringArray)
        {
            var result = new ArrayList();
            foreach (var sourceString in sourceStringArray) result.Add(sourceString);

            return result;
        }


        /// <summary>
        ///     Find out whether an array contains a specific value.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="Value">Values to find.</param>
        /// <param name="caseSensitive">Whether to perform a case-sensitive comparison.</param>
        /// <param name="SubstringCheck">If true, perform a substring check.  If false, perform whole-string comparison.</param>
        /// <returns></returns>
        public static bool ArrayHasValue(string[] array, string[] values, bool caseSensitive,
            bool substringToCheck = false)
        {
            var result = false;

            foreach (var thisString in values)
            {
                result = ArrayHasValue(array, thisString, caseSensitive, substringToCheck);
                if (result) break;
            }

            return result;
        }

        /// <summary>
        ///     Find out whether an array contains a specific value.
        /// </summary>
        /// <param name="array">Array to search.</param>
        /// <param name="value">Value to find.</param>
        /// <param name="caseSensitive">Whether to perform a case-sensitive comparison.</param>
        /// <param name="substringCheck">If true, perform a substring check.  If false, perform whole-string comparison.</param>
        /// <returns></returns>
        public static bool ArrayHasValue(string[] array, string value, bool caseSensitive, bool substringCheck = false)
        {
            var result = false;
            var ucValue = "";

            if (!caseSensitive)
                ucValue = value.ToUpper();
            else
                ucValue = value;


            for (var index = 0; index < array.Length; index++)
            {
                string testValue = null;

                if (!caseSensitive)
                    testValue = array[index].ToUpper();
                else
                    testValue = array[index];

                var matchFound = false;

                if (substringCheck)
                    matchFound = testValue.IndexOf(ucValue) != -1;
                else
                    matchFound = testValue == ucValue;

                if (matchFound)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        /// <summary>
        ///     Go through an array, determining if TextValue contains any substrings in the array.
        ///     If it does, returns true, and updates the ValuesFound array.
        /// </summary>
        /// <param name="array"></param>
        /// <param name="textValue"></param>
        /// <param name="CaseSensitive"></param>
        /// <returns></returns>
        public static bool TextContainsArraySubstrings(string[] array, string textValue, ref string[] valuesFound)
        {
            valuesFound = new string[0];
            var esa = new ClsEditableStringArray();
            var textValueUpper = textValue.ToUpper();
            var result = false;

            foreach (var thisArrayValue in array)
            {
                var avUpper = thisArrayValue.ToUpper();
                if (textValueUpper.Contains(avUpper))
                {
                    result = true;
                    esa.Append(thisArrayValue);
                }
            }

            valuesFound = esa.Data;
            esa = null;
            return result;
        }


        /// <summary>
        ///     ShuffleStringArray randomly shuffles all of the strings in the array it's passed.
        /// </summary>
        /// <param name="arrayToShuffle">The array to shuffle.</param>
        public static void ShuffleStringArray(string[] arrayToShuffle)
        {
            if (arrayToShuffle.Length > 1)
            {
                var randomNumberGenerator = new Random(Environment.TickCount);

                for (var index = 0; index < arrayToShuffle.Length; index++)
                {
                    var destIndex = index;
                    while (destIndex == index) destIndex = randomNumberGenerator.Next(arrayToShuffle.Length);

                    var swapString = arrayToShuffle[destIndex];
                    arrayToShuffle[destIndex] = arrayToShuffle[index];
                    arrayToShuffle[index] = swapString;
                }
            }
        }

        /// <summary>
        ///     Returns the width of the widest string in the array.
        /// </summary>
        /// <param name="arrayToSearch">Count all strings in this array.</param>
        /// <returns>The width of the widest string.</returns>
        public static int GetWidestStringWidth(string[] arrayToSearch)
        {
            return GetWidestArrayString(arrayToSearch).Length;
        }

        public static int GetShortestStringWidth(string[] arrayToSearch)
        {
            var result = -1;
            var shortestString = GetShortestString(arrayToSearch);
            if (shortestString != null) result = shortestString.Length;

            return result;
        }

        public static string GetShortestString(string[] arrayToSearch)
        {
            string result = null;
            foreach (var thisString in arrayToSearch)
                if (result == null)
                {
                    result = thisString;
                }
                else
                {
                    if (result.Length > thisString.Length) result = thisString;
                }

            return result;
        }


        /// <summary>
        ///     Gets the widest string found in the array.
        /// </summary>
        /// <param name="arrayToSearch">Array to search through.</param>
        /// <returns>The widest string found.</returns>
        public static string GetWidestArrayString(string[] arrayToSearch)
        {
            var result = "";


            foreach (var thisString in arrayToSearch)
                if (thisString != null)
                    if (thisString.Length > result.Length)
                        result = thisString;

            return result;
        }

        /// <summary>
        ///     Insert text in front of each line in an array of strings.
        /// </summary>
        /// <param name="textToInsert">Value to insert.</param>
        /// <param name="sourceArray">Array to operate on.</param>
        /// <returns></returns>
        public static string[] InsertTextIntoArrayLines(string textToInsert, string[] sourceArray)
        {
            var result = new string[sourceArray.Length];
            for (var index = 0; index < sourceArray.Length; index++) result[index] = textToInsert + sourceArray[index];

            return result;
        }

        /// <summary>
        ///     Compare each string in two arrays.
        /// </summary>
        /// <param name="array1">First array to compare.</param>
        /// <param name="array2">Second array to compare.</param>
        /// <returns>true if all strings are identical, false otherwise.</returns>
        public static bool StringsAreTheSame(string[] array1, string[] array2)
        {
            var result = true;

            if (array1.Length == array2.Length)
                for (var index = 0; index < array1.Length; index++)
                {
                    var string1 = array1[index];
                    var string2 = array2[index];
                    if (!StringFunctions.StringsAreTheSame(string1, string2))
                    {
                        result = false;
                        break;
                    }
                }
            else
                result = false;

            return result;
        }

        public static string[] TrimStrings(string[] sourceArray)
        {
            var result = new string[sourceArray.Length];

            for (var index = 0; index < sourceArray.Length; index++) result[index] = sourceArray[index].Trim();

            return result;
        }

        /// <summary>
        ///     Remove all duplicates from an array of strings.
        /// </summary>
        /// <param name="arrayToFix">The array to process.</param>
        public static void RemoveArrayDupes(ref string[] arrayToFix)
        {
            var dupeFinder = new Hashtable();

            for (var index = 0; index < arrayToFix.Length; index++)
            {
                var thisString = arrayToFix[index];

                if (!dupeFinder.Contains(thisString)) dupeFinder[thisString] = null;
            }

            if (dupeFinder.Count < arrayToFix.Length)
            {
                arrayToFix = new string[dupeFinder.Count];
                var arrayIndex = 0;

                foreach (var thisKey in dupeFinder.Keys)
                {
                    arrayToFix[arrayIndex] = (string) thisKey;
                    arrayIndex++;
                }
            }
        }

        public static char[] MergeCharArraysUniquely(Array allArray)
        {
            var dupes = new Hashtable();

            var ms = new MemoryStream();
            foreach (Array thisArray in allArray)
            foreach (char thisCh in thisArray)
                if (!dupes.Contains(thisCh))
                {
                    dupes.Add(thisCh, thisCh);
                    ms.WriteByte((byte) thisCh);
                }

            ms.Seek(0, SeekOrigin.Begin);
            var resultBuffer = new byte[(int) ms.Length];
            ms.Read(resultBuffer, 0, (int) ms.Length);

            var result = new char[resultBuffer.Length];
            for (var index = 0; index < resultBuffer.Length; index++) result[index] = (char) resultBuffer[index];

            return result;
        }

        public static void ReverseStringArrayElementOrder(string[] arrayToReorder)
        {
            var topIndex = arrayToReorder.Length - 1;

            for (var bottomIndex = 0; bottomIndex < arrayToReorder.Length; bottomIndex++)
            {
                if (bottomIndex == topIndex)
                    break; // This breaks out if there is only one element; also
                // gives a second guarantee that indices don't cross the 
                // centerpoint of the array.

                var holder = arrayToReorder[bottomIndex];
                arrayToReorder[bottomIndex] = arrayToReorder[topIndex];
                arrayToReorder[topIndex] = holder;

                topIndex--;
                if (topIndex <= bottomIndex)
                    break; // Doing it this way avoids having to worry about where the middle value is.  Finished is finished.
            }
        }

        public static string FindFirstStringContainingValue(string[] arrayToSearch, string valueToFind,
            bool caseSensitive,
            int startingIndex, ref int positionFound)
        {
            positionFound = -1;

            string valueToLocate = null;
            if (!caseSensitive)
                valueToLocate = valueToFind.ToUpper();
            else
                valueToLocate = valueToFind;

            var result = "";

            for (var index = startingIndex; index < arrayToSearch.Length; index++)
            {
                string valueToCompare = null;
                if (!caseSensitive)
                    valueToCompare = arrayToSearch[index].ToUpper();
                else
                    valueToCompare = arrayToSearch[index];

                if (valueToCompare.Contains(valueToLocate))
                {
                    result = arrayToSearch[index];
                    positionFound = index;
                    break;
                }
            }

            return result;
        }

        public static bool StringArrayHasDupes(string[] arrayToCheck)
        {
            var result = false;
            var tester = new Dictionary<string, string>();
            foreach (var thisString in arrayToCheck)
                if (tester.ContainsKey(thisString))
                {
                    result = true;
                    break;
                }
                else
                {
                    tester.Add(thisString, thisString);
                }

            return result;
        }


        public static string[][] DivideArray(string[] arrayToDivide, int divideBy)
        {
            var lowerBoundLen = arrayToDivide.Length / divideBy;
            var result = new string[divideBy][];

            var sourceIndex = 0;

            for (var upperIndex = 0; upperIndex < divideBy; upperIndex++)
            {
                var destStr = new string[lowerBoundLen];

                for (var lowerIndex = 0; lowerIndex < lowerBoundLen; lowerIndex++)
                {
                    destStr[lowerIndex] = arrayToDivide[sourceIndex];
                    sourceIndex++;
                }

                result[upperIndex] = destStr;
                destStr = null;
            }

            return result;
        }

        public static void ToUpperCase(string[] mSearchTerms)
        {
            for (var index = 0; index < mSearchTerms.Length; index++)
                mSearchTerms[index] = mSearchTerms[index].ToUpper();
        }

        public static string[] PackArray(string[] sourceArray, bool trimFirst = false)
        {
            var result = new StringArrayList();
            foreach (var thisLine in sourceArray)
                if (thisLine != null)
                {
                    var lineToUse = thisLine;
                    if (trimFirst) lineToUse = lineToUse.Trim();

                    if (lineToUse.Length > 0) result.Add(lineToUse);
                }

            return result.ToStringArray();
        }

        public static string CreateConstStringArray(string[] arrayContainingValues)
        {
            string result = null;
            foreach (var thisLine in arrayContainingValues)
                if (result == null)
                    result = "\"" + thisLine + "\"";
                else
                    result += ",\"" + thisLine + "\"";


            result = "{" + result + "}";

            return result;
        }


        public static string[] CreateArrayFromString(string multilineString)
        {
            var sa = new ClsEditableStringArray(multilineString);
            return sa.Data;
        }

        public static string[] FilterArray(string[] arrayToFilter, string valueToFind, bool invert)
        {
            var result = new StringArrayList();
            var ucValueToFind = valueToFind.ToUpper();

            foreach (var valueToCheck in arrayToFilter)
            {
                var useString = valueToCheck.ToUpper().Contains(ucValueToFind);
                if (invert) useString = !useString;

                if (useString) result.Add(valueToCheck);
            }

            return result.ToStringArray();
        }

        public static void IncrementAllArrayElements(ref int[] arrayElements, bool ignoreNegativeOne)
        {
            for (var index = 0; index < arrayElements.Length; index++)
            {
                var value = arrayElements[index];
                value++;
                if (ignoreNegativeOne && value == 0) value--;

                arrayElements[index] = value;
            }
        }

        public static bool ArrayContainsSubstringOfString(string[] substrings, string stringToQuery)
        {
            var result = false;
            var stringToQueryUp = stringToQuery.ToUpper();
            foreach (var substring in substrings)
                if (stringToQueryUp.Contains(substring.ToUpper()))
                {
                    result = true;
                    break;
                }

            return result;
        }

        public static bool ArrayElementContainsSubstring(string[] elementsToCheck, string substring, bool caseSensitive)
        {
            var result = false;
            string ucSubstring = null;
            if (caseSensitive)
                ucSubstring = substring;
            else
                ucSubstring = substring.ToUpper();

            foreach (var element in elementsToCheck)
            {
                string ucElement = null;

                if (caseSensitive)
                    ucElement = element;
                else
                    ucElement = element.ToUpper();

                if (ucElement.Contains(ucSubstring))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }


        public static string[] ReplaceElementValue(string[] arrayToSearch, string elementValueToFind, string newValue)
        {
            var result = arrayToSearch;
            var upElementValueToFind = elementValueToFind.ToUpper();

            for (var index = 0; index < result.Length; index++)
                if (arrayToSearch[index].ToUpper() == upElementValueToFind)
                    result[index] = newValue;
                else
                    result[index] = arrayToSearch[index];

            return result;
        }

        public static string[] ReplaceElementValues(string[] originalArray, string[] valuesToFind,
            string[] replacementValues)
        {
            var result = originalArray;

            for (var index = 0; index < valuesToFind.Length; index++)
            {
                var valueToFind = valuesToFind[index];
                var replacementValue = replacementValues[index];

                result = ReplaceElementValue(result, valueToFind, replacementValue);
            }

            return result;
        }

        public static int GetIndexForMatchingArrayElement(string[] elements, string valueToFind)
        {
            if (elements == null)
                throw new NullReferenceException("Elements argument is null in GetIndexForMatchingArrayElement.");

            var result = -1;
            var ucValueToFind = valueToFind.ToUpper();

            for (var index = 0; index < elements.Length; index++)
                if (elements[index].ToUpper() == ucValueToFind)
                {
                    result = index;
                    break;
                }

            return result;
        }

        public static char[] AppendCharsToCharArray(char[] firstArray, char[] secondArray)
        {
            var result = new char[firstArray.Length + secondArray.Length];
            var destIndex = 0;
            foreach (var arrayChar in firstArray)
            {
                result[destIndex] = arrayChar;
                destIndex++;
            }

            foreach (var arrayChar in secondArray)
            {
                result[destIndex] = arrayChar;
                destIndex++;
            }

            return result;
        }

        public static string[] RemoveValues(string[] values, string[] badValues)
        {
            var result = new StringArrayList();
            foreach (var thisValue in values)
            {
                var newValue = thisValue;
                foreach (var badValue in badValues)
                {
                    newValue = StringFunctions.RemoveSubstring(newValue, badValue);
                    if (newValue.Length == 0) break;
                }

                if (newValue.Length > 0) result.Add(newValue);
            }

            return result.ToStringArray();
        }

        public static string GetStringWithSubstringMatch(string[] arrayToSearch, string substringToFind)
        {
            var upSubstringToFind = substringToFind.ToUpper();
            string result = null;

            foreach (var stringToSearch in arrayToSearch)
            {
                var upperCandidateString = stringToSearch.ToUpper();
                if (upperCandidateString.Contains(upSubstringToFind))
                {
                    result = stringToSearch;
                    break;
                }
            }

            return result;
        }

        public static string GetSuccessiveElementAfterMatch(string[] arrayToSearch, string keyToFind,
            bool caseSensitive)
        {
            var foundMatch = false;
            var ucKeyToFind = keyToFind;
            string result = null;

            if (!caseSensitive) ucKeyToFind = ucKeyToFind.ToUpper();

            foreach (var thisLine in arrayToSearch)
                if (!foundMatch)
                {
                    string lineToCompare = null;
                    if (caseSensitive)
                        lineToCompare = thisLine;
                    else
                        lineToCompare = thisLine.ToUpper();

                    if (lineToCompare == ucKeyToFind) foundMatch = true;
                }
                else
                {
                    result = thisLine;
                    break;
                }

            return result;
        }

        public static bool AllLinesAreTheSameLength(string[] sourceArray)
        {
            var result = true;
            var lineSize = -1;
            foreach (var sourceLine in sourceArray)
                if (lineSize == -1)
                {
                    lineSize = sourceLine.Length;
                }
                else
                {
                    if (lineSize != sourceLine.Length)
                    {
                        result = false;
                        break;
                    }
                }

            return result;
        }

        public static int GetLongestLineLength(string[] sourceArray)
        {
            var result = 0;
            foreach (var thisString in sourceArray)
                if (thisString != null)
                    if (thisString.Length > result)
                        result = thisString.Length;

            return result;
        }

        public static string[] GetColumn(string[] data, int columnIndex, string delimiters)
        {
            var delims = delimiters.ToCharArray();
            var result = new string[data.Length];

            for (var lineIndex = 0; lineIndex < data.Length; lineIndex++)
            {
                var thisLine = data[lineIndex];
                var columns = thisLine.Split(delims);
                columns = PackArray(columns, true);

                result[lineIndex] = columns[columnIndex];
            }

            return result;
        }

        public static string[] PrependSaElement(string newElement, string[] source)
        {
            var result = new string[source.Length + 1];
            result[0] = newElement;
            source.CopyTo(result, 1);
            return result;
        }


        public static string JoinDoublesToDelimitedString(double[] theArrayToJoin, string delimiter = ",",
            string format = "0.####", CultureInfo culture = null)
        {
            string result = null;
            foreach (var thisValue in theArrayToJoin)
            {
                string stringValue = null;
                if (culture == null) culture = new CultureInfo("en-US");

                stringValue = thisValue.ToString(format, culture);


                if (result == null)
                    result = stringValue;
                else
                    result += delimiter + stringValue;
            }

            return result;
        }

        public static double[] ExtractFloatArrayFromDelimitedString(string delimitedString, char delimiter)
        {
            char[] delimiters = {delimiter};
            var values = delimitedString.Split(delimiters);
            var result = new double[values.Length];
            for (var index = 0; index < result.Length; index++)
            {
                double parsedValue;
                result[index] = double.NaN;

                if (Parsing.TryParseDouble(values[index], out parsedValue)) result[index] = parsedValue;
            }

            return result;
        }
    }
}