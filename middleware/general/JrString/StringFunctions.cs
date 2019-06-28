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
using System.IO;
using System.Text;

namespace EssStringLib
{
    /// <summary>
    ///     Class containing useful string functions.
    /// </summary>
    public static class StringFunctions
    {
        /// <summary>
        ///     Returns string value, or "" if value is null.
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string StringValue(string source)
        {
            if (source == null)
                return "";
            return source;
        }

        public static bool SplitFileNameAndNum(string baseName, out string namePart, out int numPart)
        {
            var success = false;
            numPart = -1;
            namePart = null;

            var strNumPart = GetTrailingNumPartOfString(baseName);
            if (strNumPart != null)
            {
                numPart = int.Parse(strNumPart);
                namePart = LeftStr(baseName, strNumPart);
                success = true;
            }

            return success;
        }

        public static string GetStringAfterLastCharacter(string originalString, char characterToFind)
        {
            var searchArray = new char[1];
            searchArray[0] = characterToFind;

            var allParts = originalString.Split(searchArray);

            var result = "";
            if (allParts.Length > 1) result = allParts[allParts.Length - 1];

            return result;
        }

        /// <summary>
        ///     If StringToQuery is null or empty, returns true.  Otherwise, returns false.
        /// </summary>
        /// <param name="stringToQuery"></param>
        /// <returns></returns>
        public static bool IsEmptyString(string stringToQuery)
        {
            var result = true;

            if (stringToQuery != null)
                if (stringToQuery.Length > 0)
                    result = false;

            return result;
        }

        public static void CheckStringForNullOrEmpty(string stringToQuery, string functionName, string variableName)
        {
            if (stringToQuery == null)
                throw new NullReferenceException(functionName + " failed. Argument or Variable " + variableName +
                                                 " is null.");
            if (stringToQuery.Length == 0)
                throw new Exception(functionName + " failed. Argument or Variable " + variableName +
                                    " is an empty string.");
        }

        public static string GetTextAfter(string sourceString, string substringToFind)
        {
            string result = null; // If unsuccessful, result will be null.  If SubstringToFind
            // is at the end of the string, will return an empty string.

            var ucSourceString = sourceString.ToUpper();
            var ucSubstringToFind = substringToFind.ToUpper();

            var substringPos = ucSourceString.IndexOf(ucSubstringToFind);
            if (substringPos > -1)
            {
                var position = substringPos + substringToFind.Length;
                var numChars = sourceString.Length - position;
                if (numChars > 0)
                    result = sourceString.Substring(position, numChars);
                else
                    result = "";
            }

            return result;
        }

        public static string GetTextBefore(string sourceString, string substringToFind)
        {
            string result = null; // If unsuccessful, result will be null. If SubstringToFind
            // is at the first position, will return an empty string.

            var ucSourceString = sourceString.ToUpper();
            var ucSubstringToFind = substringToFind.ToUpper();

            var substringPos = ucSourceString.IndexOf(ucSubstringToFind);
            if (substringPos == 0)
                result = "";
            else if (substringPos > 0) result = sourceString.Substring(0, substringPos);


            return result;
        }

        public static string RemovePrecedingBackslash(string sourcePath)
        {
            var result = sourcePath;
            if (result == null) throw new Exception("RemovePrecedingBackslash failed.  SourcePath argument is null.");

            if (result.Length > 0)
                while (result.Substring(0, 1) == "\\")
                {
                    result = result.Substring(1, result.Length - 1);
                    if (result.Length == 0) break;
                }

            return result;
        }

        public static string RemoveTrailingBackslash(string sourcePath)
        {
            var result = sourcePath;
            if (result == null) throw new Exception("RemoveTrailingBackslash failed.  SourcePath argument is null.");

            if (result.Length > 0)
                if (result.Substring(result.Length - 1, 1) == "\\")
                    result = result.Substring(0, result.Length - 1);


            return result;
        }

        /// <summary>
        ///     Perform a character-by-character comparison of two strings.
        /// </summary>
        /// <param name="string1">First string to compare.</param>
        /// <param name="string2">Second string to compare.</param>
        /// <returns>true if the strings are identical, false otherwise.</returns>
        private static bool StringCompareChbyCh(string string1, string string2)
        {
            var result = true;

            for (var index = 0; index < string1.Length; index++)
                if (string1[index] != string2[index])
                {
                    result = false;
                    break;
                }

            return result;
        }

        public static string RemoveChars(this string source, char[] charsToRemove)
        {
            return RemoveCharsFromString(source, charsToRemove);
        }

        public static string RemoveCharsFromString(string sourceString, char[] charsToRemove)
        {
            var result = "";
            var sourceChars = sourceString.ToCharArray();
            foreach (var sourceChar in sourceChars)
                if (!CharArrayContainsChar(charsToRemove, sourceChar))
                    result += sourceChar;

            return result;
        }

        public static bool CharArrayContainsChar(char[] charArray, char charToFind)
        {
            var result = false;
            foreach (var thisChar in charArray)
                if (thisChar == charToFind)
                {
                    result = true;
                    break;
                }

            return result;
        }

        /// <summary>
        ///     Compare two strings, allowing the comparison of nulls as well.
        /// </summary>
        /// <param name="string1">First string to compare.</param>
        /// <param name="string2">Second string to compare.</param>
        /// <returns>true if strings are identical, false otherwise.</returns>
        public static bool StringsAreTheSame(string string1, string string2)
        {
            var result = true;
            if (string1 == null && string2 == null)
            {
                result = true;
            }
            else if (string1 == null || string2 == null)
            {
                result = false;
            }
            else
            {
                if (string1.Length == string2.Length)
                    result = StringCompareChbyCh(string1, string2);
                else
                    result = false;
            }

            return result;
        }


        /// <summary>
        ///     Compares a string with an array of possible values.
        /// </summary>
        /// <param name="stringToCheck">String to check.</param>
        /// <param name="stringsToFind">Possible values the string may contain.</param>
        /// <param name="caseSensitive">Whether to perform case-sensitive comparisons.</param>
        /// <param name="stringFound">Set to the value of the first string found in case further evaluation is needed.</param>
        /// <returns>True is the string starts with of the possible values, false otherwise.</returns>
        public static bool StringStartsWith(string stringToCheck, string[] stringsToFind, bool caseSensitive,
            ref string stringFound)
        {
            var result = false;
            string checkString;

            if (caseSensitive)
                checkString = stringToCheck;
            else
                checkString = stringToCheck.ToUpper();

            foreach (var stringToFind in stringsToFind)
            {
                string findString;
                if (caseSensitive)
                    findString = stringToFind;
                else
                    findString = stringToFind.ToUpper();

                if (checkString.IndexOf(findString) == 0)
                {
                    result = true;
                    stringFound = stringToFind;
                    break;
                }

                if (stringToFind.Length > 3 && checkString.Length > 3)
                    if (stringToFind.Substring(0, 3).ToUpper() == "\\W ")
                    {
                        var allStringWordsToCheck = GetWordsFromString(checkString);
                        if (allStringWordsToCheck.Length > 0)
                        {
                            var allStringWordsToFind = GetWordsFromString(stringToFind);
                            if (allStringWordsToFind.Length > 0)
                                if (allStringWordsToCheck[1].Trim().ToUpper() ==
                                    allStringWordsToFind[1].Trim().ToUpper())
                                {
                                    result = true;
                                    stringFound = allStringWordsToFind[1].Trim();
                                    break;
                                }
                        }
                    }
            }

            return result;
        }

        /// <summary>
        ///     Remove matching lines from an array.  Regular expression matching not performed.
        /// </summary>
        /// <param name="source">Array to crunch.</param>
        /// <param name="filter">Value to match.</param>
        /// <param name="compareType">Type of comparison to perform.</param>
        /// <param name="caseSensitive">Whether comparison is case-sensitive.</param>
        /// <param name="invert">Whether to invert the results of the comparison before determining whether to remove each element.</param>
        /// <returns>Modified array.</returns>
        public static string[] CrunchArray(string[] source, string filter, ECompareType compareType, bool caseSensitive,
            bool invert)
        {
            var destIndex = 0;

            string testLine;

            if (caseSensitive)
                testLine = filter;
            else
                testLine = filter.ToUpper();

            // Remove all lines matching Filter.
            var result = new string[source.Length];
            foreach (var sourceLine in source)
            {
                var dropThisOne = false;

                if (sourceLine.Length == 0)
                {
                    if (testLine.Length != 0) dropThisOne = true;
                }
                else if (testLine.Length > sourceLine.Length)
                {
                    dropThisOne = true;
                }
                else
                {
                    string modifiedSourceLine;


                    if (caseSensitive)
                        modifiedSourceLine = sourceLine;
                    else
                        modifiedSourceLine = sourceLine.ToUpper();

                    switch (compareType)
                    {
                        case ECompareType.CtInStr:
                            if (modifiedSourceLine.IndexOf(testLine) < 0) dropThisOne = true;

                            break;
                        case ECompareType.CtLeft:
                            if (modifiedSourceLine.Length >= testLine.Length)
                            {
                                if (modifiedSourceLine.Substring(0, testLine.Length) != testLine) dropThisOne = true;
                            }
                            else
                            {
                                dropThisOne = true;
                            }

                            break;

                        case ECompareType.CtWhole:
                            if (modifiedSourceLine != testLine) dropThisOne = true;

                            break;
                        default:
                            throw new Exception("ECompareType not recognized in CrunchArray function.");
                    }
                }


                if (invert) dropThisOne = !dropThisOne;

                if (!dropThisOne)
                {
                    result[destIndex] = sourceLine;
                    destIndex++;
                }
            }

            var numElements = destIndex;
            if (numElements != result.Length) result = CopySubArray(result, 0, numElements);

            return result;
        }

        /// <summary>
        ///     Checks whether any duplicate values exist in an array.
        /// </summary>
        /// <param name="arrayToTest">Array to search.</param>
        /// <returns>True if duplicate values exist, false otherwise.</returns>
        public static bool ArrayHasDuplicateValues(string[] arrayToTest)
        {
            var result = false;
            for (var outerIndex = 0; outerIndex < arrayToTest.Length; outerIndex++)
            {
                for (var innerIndex = 0; innerIndex < arrayToTest.Length; innerIndex++)
                    if (innerIndex != outerIndex)
                        if (arrayToTest[innerIndex] == arrayToTest[outerIndex])
                        {
                            result = true;
                            break;
                        }

                if (result) break;
            }

            return result;
        }

        /// <summary>
        ///     Copies a subarray from SourceArray.
        /// </summary>
        /// <param name="sourceArray">Array to copy from.</param>
        /// <param name="startPos">Index at which copying begins.</param>
        /// <param name="numLines">Number of lines to copy.</param>
        /// <returns>New array.</returns>
        public static string[] GetSubArray(string[] sourceArray, int startPos, int numLines)
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

            var result = new ClsEditableStringArray(priorLines);
            result.Append(dataToInsert);
            result.Append(afterLines);

            return result.Data;
        }

        /// <summary>
        ///     Converts a string into a value that can be pasted directly as C#
        ///     code.  See "@ string literal" in Visual Studio online help for more information.
        /// </summary>
        /// <param name="theString"></param>
        /// <returns></returns>
        public static string TurnStringIntoHereDocument(string theString)
        {
            var stringArray = BreakStringIntoShorterArrays(theString, 512);
            return TurnStringArrayIntoHereDocument(stringArray);
        }

        /// <summary>
        ///     Convert a character array to a string.
        /// </summary>
        /// <param name="chars">Characters to convert.</param>
        /// <returns>New string.</returns>
        public static string Chars2String(char[] chars)
        {
            var result = "";
            foreach (var ch in chars) result = result + ch;

            return result;
        }

        /// <summary>
        ///     Given a string containing carriage returns, or CR/LF combinations,
        ///     return an array of lines in that string.
        /// </summary>
        /// <param name="source">String containing 1 or more lines (Source may be zero length).</param>
        /// <returns>New array.</returns>
        public static string[] GetStringArrayFromMultiLinedString(string source)
        {
            var numLines = 1;
            var lastCh = '\0';

            foreach (var thisCh in source)
            {
                if (thisCh == (char) 10 || thisCh == (char) 13 && lastCh != (char) 10) numLines++;

                lastCh = thisCh;
            }

            var result = new string[numLines];
            var resultIndex = 0;
            lastCh = '\0';

            foreach (var thisCh in source)
            {
                if (thisCh == (char) 10 || thisCh == (char) 13 && lastCh != (char) 10)
                    resultIndex++;
                else if (thisCh != (char) 10 && thisCh != (char) 13) result[resultIndex] = result[resultIndex] + thisCh;

                lastCh = thisCh;
            }

            if (result.Length > 0)
                if (result[result.Length - 1] == null)
                {
                    var deleter = new ClsEditableStringArray();
                    deleter.Data = result;
                    deleter.Delete(result.Length - 1, 1);
                    result = deleter.Data;
                    deleter = null;
                }

            if (result.Length > 0)
                for (var clearNullIndex = 0; clearNullIndex < result.Length; clearNullIndex++)
                    if (result[clearNullIndex] == null)
                        result[clearNullIndex] = "";

            return result;
        }

        /// <summary>
        ///     Break a string into lines no longer than specified length.
        /// </summary>
        /// <param name="source">String to convert.</param>
        /// <param name="maxLineLen">Maximum line length.</param>
        /// <returns>Array of converted text.</returns>
        public static string[] BreakStringIntoShorterArrays(string source, int maxLineLen)
        {
            var numElements = source.Length / maxLineLen;
            if (numElements * maxLineLen < source.Length) numElements++;


            var result = new string[numElements];
            var inputStream = GetStringReader(source);

            var chars = new char[maxLineLen];
            var startingPos = 0;

            for (var index = 0; index < result.Length; index++)
            {
                var numChars = maxLineLen;
                if (numChars + startingPos > source.Length)
                {
                    numChars = source.Length - startingPos;
                    chars = new char[numChars];
                }

                inputStream.Read(chars, 0, numChars);
                result[index] = Chars2String(chars);
                startingPos += numChars;
            }

            return result;
        }

        /// <summary>
        ///     Turns an array of strings into a Here document (See "@ string literal" in Visual Studio .NET online help.
        /// </summary>
        /// <param name="stringArray">Data to convert.</param>
        /// <returns>Here document for pasting into C# code.</returns>
        public static string TurnStringArrayIntoHereDocument(string[] stringArray)
        {
            var result = "";

            for (var index = 0; index < stringArray.Length; index++)
            {
                result = result + "@\"" + stringArray[index] + "\"";
                if (index < stringArray.Length - 1)
                    result = result + "+\n";
                else
                    result = result + ";\n";
            }

            return result;
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
        ///     Determines whether word found at Pos in Line starts with a numeral.
        /// </summary>
        /// <param name="line">Line to search.</param>
        /// <param name="pos">Position in line to test.</param>
        /// <returns>True or false.</returns>
        public static bool PosFollowedByNumber(string line, int pos)
        {
            bool result;

            if (line.Length == pos + 1)
            {
                result = false;
            }
            else
            {
                var rs = line.Substring(pos + 1, line.Length - (pos + 1));
                result = StringStartsWithNumber(rs);
            }

            return result;
        }

        /// <summary>
        ///     Determine whether a string starts with a numeral (0-9).
        /// </summary>
        /// <param name="word">String to test.</param>
        /// <returns>True if string starts with a numeral, false otherwise.</returns>
        public static bool StringStartsWithNumber(string word)
        {
            bool result;
            if (word.Length == 0)
            {
                result = false;
            }
            else
            {
                var thisCh = word.Substring(0, 1);
                char[] charsToFind = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                result = StringContainsChars(charsToFind, thisCh);
            }

            return result;
        }

        /// <summary>
        ///     Determine whether a string starts with one of a range of specified values.
        /// </summary>
        /// <param name="stringToSearch">String to compare.</param>
        /// <param name="stringsToLookFor">An array of strings to look for.</param>
        /// <param name="stringFound">Which value was actually found.</param>
        /// <returns>True or false.</returns>
        public static bool StartsWithOneOf(string stringToSearch, string[] stringsToLookFor, ref string stringFound)
        {
            var result = false;


            foreach (var stringToLookFor in stringsToLookFor)
            {
                result = StartsWith(stringToSearch, stringToLookFor, ref stringFound);
                if (result) break;
            }

            return result;
        }

        /// <summary>
        ///     Split a string into an array of all words it contains.
        /// </summary>
        /// <param name="stringToSplit">String to split.</param>
        /// <returns>Array of words found.</returns>
        public static string[] GetWordsFromString(string stringToSplit)
        {
            var preprocessedString = "";

            foreach (var ch in stringToSplit)
                if (preprocessedString.Length == 0)
                {
                    if (ch != ' ') preprocessedString = ch.ToString();
                }
                else
                {
                    if (ch == ' ')
                    {
                        if (preprocessedString[preprocessedString.Length - 1] != ',') preprocessedString += ",";
                    }
                    else
                    {
                        preprocessedString += ch.ToString();
                    }
                }

            char[] delimiters = {','};

            var result = preprocessedString.Split(delimiters);

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

        public static string ReplaceSubstring(string source, string stringToReplace, string newValue)
        {
            var startPos = 0;

            while (startPos > -1)
            {
                startPos = source.ToUpper().IndexOf(stringToReplace.ToUpper(), startPos);
                if (startPos != -1)
                {
                    source = ReplaceSubstring(source, startPos, stringToReplace.Length, newValue);
                    startPos += newValue.Length;
                }
            }

            return source;
        }

        public static string ReplaceSubstring(string source, int startPos, int length, string replacementString)
        {
            var leftPart = source.Substring(0, startPos);
            var replLen = length;
            var endPos = startPos + replLen;
            var rightPart = "";
            if (endPos - 1 < source.Length) rightPart = source.Substring(endPos, source.Length - endPos);

            var result = leftPart + replacementString + rightPart;


            return result;
        }

        /// <summary>
        ///     Replace every string in array containing search string with new value.
        /// </summary>
        /// <param name="arrayToSearch">Array containing data.</param>
        /// <param name="stringToFind">String to locate.</param>
        /// <param name="newValue">Replacement value.</param>
        /// <returns></returns>
        public static string[] ReplaceStringContainingValue(string[] arrayToSearch, string stringToFind,
            string newValue)
        {
            var result = new string[arrayToSearch.Length];
            for (var index = 0; index < arrayToSearch.Length; index++)
            {
                var thisLine = arrayToSearch[index];

                if (StringContainsValue(thisLine, stringToFind, false)) thisLine = stringToFind;

                result[index] = thisLine;
            }

            return result;
        }


        /// <summary>
        ///     Determine whether a string contains one or more of specified characters.
        /// </summary>
        /// <param name="charsToFind">Characters to find.</param>
        /// <param name="stringToSearch">String to search.</param>
        /// <returns>True or false.</returns>
        public static bool StringContainsChars(char[] charsToFind, string stringToSearch)
        {
            var result = false;
            var charsToSearch = stringToSearch.ToCharArray();

            foreach (var charToFind in charsToFind)
            {
                foreach (var charToSearch in charsToSearch)
                    if (charToFind == charToSearch)
                    {
                        result = true;
                        break;
                    }

                if (result) break;
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
        ///     Chops off last line from an array of bytes unless an end-of-line exists
        /// </summary>
        /// <param name="source">Bytes to truncate.</param>
        /// <returns>Byte array with incomplete line part removed.</returns>
        private static byte[] RemovePartialLineFarEnd(byte[] source)
        {
            var index = 0;
            var numChars = 0;

            for (index = source.Length - 1; index > -1; index--)
            {
                numChars++;
                if (source[index] == 10 || source[index] == 13)
                {
                    while (source[index] == 10 || source[index] == 13)
                    {
                        numChars++;
                        index--;
                    }

                    break;
                }
            }

            var result = new byte[source.Length - numChars];
            for (var destIndex = 0; destIndex < result.Length; destIndex++) result[destIndex] = source[destIndex];

            return result;
        }

        /// <summary>
        ///     Chops off first line from an array of bytes unless an end-of-line precedes it.
        /// </summary>
        /// <param name="source">Bytes to truncate.</param>
        /// <returns>Byte array with incomplete line part removed.</returns>
        private static byte[] RemovePartialLineFrontEnd(byte[] source)
        {
            var index = 0;
            var numChars = 0;

            for (index = 0; index < source.Length; index++)
            {
                numChars++;
                if (source[index] == 10 || source[index] == 13)
                {
                    while (source[index] == 10 || source[index] == 13)
                    {
                        numChars++;
                        index++;
                    }

                    break;
                }
            }

            var result = new byte[source.Length - numChars];
            for (var destIndex = 0; destIndex < result.Length; destIndex++)
                result[destIndex] = source[destIndex + numChars];

            return result;
        }


        /// <summary>
        ///     Converts a bool to a string.
        /// </summary>
        /// <param name="boolValue">Value to convert.</param>
        /// <returns>"True" or "False"</returns>
        public static string Bool2Str(bool boolValue)
        {
            var result = "";

            if (boolValue)
                result = "True";
            else
                result = "False";

            return result;
        }

        /// <summary>
        ///     Converts a string to a boolean.  Not case-sensitive.
        /// </summary>
        /// <param name="boolValue">Value to convert.</param>
        /// <returns>True or false.</returns>
        public static bool Str2Bool(string boolValue)
        {
            bool result;
            var test = boolValue.ToUpper();
            if (test == "TRUE")
            {
                result = true;
            }
            else
            {
                if (test == "FALSE")
                    result = false;
                else
                    throw new Exception("Str2Bool failed:  Value \"" +
                                        boolValue + "\" is not a valid boolean string.");
            }

            return result;
        }

        /// <summary>
        ///     Chops a partial line off the specified end of an array of bytes.
        /// </summary>
        /// <param name="source">Data to modify.</param>
        /// <param name="whichEnd">End to chop.</param>
        /// <returns>Modified byte array.</returns>
        public static byte[] RemovePartialLine(byte[] source, EWhichEnd whichEnd)
        {
            byte[] result = null;

            if (whichEnd == EWhichEnd.WeFrontEnd || whichEnd == EWhichEnd.WeBothEnds)
                result = RemovePartialLineFrontEnd(source);
            else
                // Reduce the lines of code in the next if statement
                result = source;

            if (whichEnd == EWhichEnd.WeBackEnd || whichEnd == EWhichEnd.WeBothEnds)
                result = RemovePartialLineFarEnd(result);

            return result;
        }

        /// <summary>
        ///     Creates a StringReader object from a string.
        /// </summary>
        /// <param name="stringToRead">String to use.</param>
        /// <returns>StringReader created.</returns>
        public static StringReader GetStringReader(string stringToRead)
        {
            return new StringReader(stringToRead);
        }

        /// <summary>
        ///     Returns the first line found in a string containing multiple lines of text.
        /// </summary>
        /// <param name="multiLineString">String to search.</param>
        /// <returns>First line found or whole line.</returns>
        public static string GetFirstLineFromString(string multiLineString)
        {
            var sr = GetStringReader(multiLineString);

            var result = sr.ReadLine();
            sr.Close();
            sr = null;
            return result;
        }

        /// <summary>
        ///     Returns the last line found in a string containing multiple lines.
        /// </summary>
        /// <param name="multiLineString">String containing multiple lines.</param>
        /// <returns>Last line.</returns>
        public static string GetLastLineFromString(string multiLineString)
        {
            var sr = new StringReader(multiLineString);

            var result = "";
            var eof = false;
            while (!eof)
            {
                var thisString = sr.ReadLine();
                if (thisString != null)
                    result = thisString;
                else
                    eof = true;
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
        ///     Remove the rightmost n characters from a string, where n is the length of the second argument.
        /// </summary>
        /// <param name="sourceStr">String to shorten.</param>
        /// <param name="rightPartOfString">Part of string to remove.</param>
        /// <returns>Truncated left portion of string</returns>
        public static string LeftStr(string sourceStr, string rightPartOfString)
        {
            var srcStr = ReverseStr(sourceStr);
            var result = RightStr(srcStr, rightPartOfString);
            return ReverseStr(result);
        }

        public static string LeftStr(string sourceStr, int numChars)
        {
            return sourceStr.Substring(0, numChars);
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
        ///     Convert an array of bytes to a string.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <returns>New string.</returns>
        public static string BytesToString(byte[] bytes)
        {
            if (bytes.Length == 0) return "";

            var result = new StringBuilder(bytes.Length, bytes.Length);
            result.Length = bytes.Length;

            for (var index = 0; index < bytes.Length; index++) result[index] = (char) bytes[index];

            return result.ToString();
        }

        /// <summary>
        ///     Bounds Value with Token for inclusion in XML.
        /// </summary>
        /// <param name="token">Keyword to surround value with.</param>
        /// <param name="value">Value to surround.</param>
        /// <returns>Bounded value.</returns>
        public static string CreateXmlBoundedString(string token, object value)
        {
            return CreateXmlBs(token, value);
        }

        private static string CreateXmlBs(string token, object value)
        {
            var outValue = "";
            if (value == null)
                outValue = "null";
            else
                outValue = value.ToString();


            return "<" + token + ">" + outValue + "</" + token + ">";
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
        ///     Find out whether a string contains one of several values.
        /// </summary>
        /// <param name="strToCheck">String to search.</param>
        /// <param name="possibleValues">Array containing all values to search for.</param>
        /// <param name="caseSensitive">Whether to perform a case-sensitive comparison.</param>
        /// <returns>True or false.</returns>
        public static bool StringContainsValue(string strToCheck, string[] possibleValues, bool caseSensitive)
        {
            var result = false;

            foreach (var possibleValue in possibleValues)
                if (StringContainsValue(strToCheck, possibleValue, caseSensitive))
                {
                    result = true;
                    break;
                }

            return result;
        }

        /// <summary>
        ///     Find out whether a string contains one of several values.
        /// </summary>
        /// <param name="strToCheck">String to search.</param>
        /// <param name="possibleValues">Array containing all values to search for.</param>
        /// <param name="caseSensitive">Whether to perform a case-sensitive comparison.</param>
        /// <param name="valueFound">If the value is found, it's returned in this argument.  Otherwise, this argument is set to ""</param>
        /// <returns>True or false.</returns>
        public static bool StringContainsValue(string strToCheck, string[] possibleValues, bool caseSensitive,
            ref string valueFound)
        {
            var result = false;
            valueFound = "";

            foreach (var possibleValue in possibleValues)
            {
                string stringToFind = null;
                string stringToSearch = null;

                if (caseSensitive)
                {
                    stringToFind = possibleValue;
                    stringToSearch = strToCheck;
                }
                else
                {
                    stringToFind = possibleValue.ToUpper();
                    stringToSearch = strToCheck.ToUpper();
                }

                if (stringToSearch.Contains(stringToFind))
                {
                    valueFound = possibleValue;
                    result = true;
                    break;
                }
            }

            return result;
        }

#if false
        /// <summary>
        /// Converts a hexadecimal string into ASCII.
        /// </summary>
        /// <param name="HexedString">String to convert.</param>
        /// <returns>Converted string.</returns>
        public static string HexStringToString(string HexedString) {
            byte[] Bytes = HexString2Bytes(HexedString);

            return BytesToString(Bytes);

        }
        public static string HexFileToString(string Filename) {
            FileStream fs = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.None);
            BinaryReader InputFile = new BinaryReader(fs);

            StringBuilder Result = new StringBuilder();
            for (int Index = 0; Index < fs.Length; Index++) {
                byte OneByte = InputFile.ReadByte();
                Result.Append(OneByte);
            }

            fs = null;
            InputFile.Close();
            InputFile = null;

            System.GC.Collect();
            System.GC.GetTotalMemory(true);
            System.GC.Collect();

            return Result.ToString();
        }
#endif


        /// <summary>
        ///     Converts a string to an array of bytes.
        /// </summary>
        /// <param name="String">String to convert.</param>
        /// <returns>Converted array.</returns>
        public static byte[] StringToBytes(string String)
        {
            var sr = new StringReader(String);

            var result = new byte[String.Length];
            var chars = new char[1];
            for (var i = 0; i < String.Length; i++)
            {
                sr.Read(chars, 0, 1);
                result[i] = (byte) chars[0];
            }

            return result;
        }

        /// <summary>
        ///     Convert an array of bytes to an array of characters.
        /// </summary>
        /// <param name="bytes">Array of bytes to convert.</param>
        /// <returns>Character array.</returns>
        public static char[] BytesToCharArray(byte[] bytes)
        {
            var result = new char[bytes.Length];
            bytes.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        ///     Returns index to first row in array containing specified value.
        /// </summary>
        /// <param name="linesToSearch">Array to search.</param>
        /// <param name="valueToFind">Substring to find.</param>
        /// <param name="caseSensitive">Whether to perform a case-sensitive comparison.</param>
        /// <returns>Index if successful, -1 if no match found.</returns>
        public static int GetInstrRow(string[] linesToSearch, string valueToFind, bool caseSensitive)
        {
            var result = -1;

            var findValue = valueToFind;

            if (!caseSensitive) findValue = findValue.ToUpper();


            for (var index = 0; index < linesToSearch.Length; index++)
            {
                var lineToSearch = linesToSearch[index];

                if (!caseSensitive) lineToSearch = lineToSearch.ToUpper();


                if (lineToSearch.IndexOf(findValue) != -1)
                {
                    result = index;
                    break;
                }
            }

            return result;
        }

#if false
        public static ByteString BytesToByteString(ref byte[] Bytes) {
            return new ByteString(Bytes);

        }
#endif

        ///// <summary>
        ///// Converts and array of bytes to a string.
        ///// </summary>
        ///// <param name="Bytes">Array of bytes to convert.</param>
        ///// <returns>String.</returns>
        //public static string BytesToString(ref byte[] Bytes)
        //{

        //    System.IO.StringWriter SW=new StringWriter();
        //    foreach(byte ThisByte in Bytes){
        //        SW.Write(ThisByte);
        //    }

        //    return SW.ToString();

        //}

        /// <summary>
        ///     Converts each hexadecimal value in a string to its numeric
        ///     equivalent in an array of bytes.  Operation on non-hexadecimal
        ///     data undefined.
        /// </summary>
        /// <param name="hexedString">String to convert.</param>
        /// <returns>Byte values in string.</returns>
        public static byte[] HexString2Bytes(string hexedString)
        {
            var result = new byte[hexedString.Length / 2];
            var srcIndex = 0;

            for (var index = 0; index < result.Length; index++)
            {
                var hexData = hexedString.Substring(srcIndex, 2);
                result[index] = Convert.ToByte(hexData, 16);
                srcIndex += 2;
            }

            return result;
        }

#if false
        /// <summary>
        /// Converts an ASCII string to a string twice the size containing
        /// hexadecimal representations of each byte in that string.
        /// </summary>
        /// <param name="TheString">String to convert.</param>
        /// <returns>Converted string.</returns>
        public static string StringToHexString(ref string TheString, bool UseTempFile) {

            byte[] Bytes = StringToBytes(TheString);
            TheString = String.Empty;
            System.GC.Collect();
            System.GC.GetTotalMemory(true);
            TheString = null;
            System.GC.Collect();
            System.GC.GetTotalMemory(true);

            string TempFilename = BytesToHexFile(ref Bytes);
            System.GC.GetTotalMemory(true);

            Bytes = null;
            System.GC.Collect();
            System.GC.Collect();
            StreamReader InputFile = new StreamReader(TempFilename, Encoding.ASCII, false, 1024);
            string Result = InputFile.ReadToEnd();
            InputFile.Close();
            System.IO.File.Delete(TempFilename);


            TempFilename = "";
            System.GC.Collect();
            return Result;
        }
#endif


        /// <summary>
        ///     Enlarges a string array.
        /// </summary>
        /// <param name="sourceString">Original array.</param>
        /// <param name="numElementsToAdd">Number of elements to add to array.</param>
        /// <returns>Expanded array containing original data and NumElementsToAdd empty strings.</returns>
        public static string[] Expand(string[] sourceString, int numElementsToAdd)
        {
            var result = new string[sourceString.Length + numElementsToAdd];
            sourceString.CopyTo(result, 0);
            return result;
        }

        /// <summary>
        ///     Converts an integer to its hexadecimal equivalent.
        /// </summary>
        /// <param name="intValueToConvert">Value to convert.</param>
        /// <returns>Hexadecimal equivalent.</returns>
        public static string Int2Hex(int intValueToConvert)
        {
            var result = "";
            result = intValueToConvert.ToString("X");
            return result;
        }


#if false
        /// <summary>
        /// Converts an array of bytes to a string of containing its hexadecimal equivalents.
        /// </summary>
        /// <param name="Bytes">Array to convert.</param>
        /// <param name="UseTempFile">Whether use use a file instead of memory.</param>
        /// <returns>Hexadecimal string.</returns>
        public static string BytesToHexString(ref byte[] Bytes, bool UseTempFile) {

            string Result = "";
            string TempFilename = null;
            System.Text.StringBuilder sb = new StringBuilder();
            StreamWriter fs = null;


            if (UseTempFile) {
                TempFilename = System.IO.Path.GetTempFileName();
                fs = new StreamWriter(TempFilename, false);
            }

            for (int Index = 0; Index < Bytes.Length; Index++) {
                string HexStr = Bytes[Index].ToString("X");

                if (HexStr.Length == 1) {
                    HexStr = "0" + HexStr;
                }

                if (UseTempFile) {
                    fs.Write(HexStr);
                }
                else {
                    sb.Append(HexStr);
                }

            }
            Bytes = null;
            System.GC.Collect();

            if (UseTempFile) {
                fs.Flush();
                fs.Close();
                StreamReader InputFile = new StreamReader(TempFilename);
                Result = InputFile.ReadToEnd();
                InputFile.Close();
                InputFile = null;
                System.IO.File.Delete(TempFilename);

            }
            else {
                Result = sb.ToString();
            }

            return Result;
        }
#endif

        /// <summary>
        ///     Converts an array of bytes to a string of containing its hexadecimal equivalents.
        /// </summary>
        /// <param name="bytes">Array to convert.</param>
        /// <returns>Hexadecimal string.</returns>
        public static string BytesToHexFile(ref byte[] bytes)
        {
            string tempFilename = null;
            StreamWriter fs = null;


            tempFilename = Path.GetTempFileName();
            fs = new StreamWriter(tempFilename, false);


            for (var index = 0; index < bytes.Length; index++)
            {
                var hexStr = bytes[index].ToString("X");

                if (hexStr.Length == 1) hexStr = "0" + hexStr;

                fs.Write(hexStr);
            }

            bytes = null;
            //System.GC.Collect();


            fs.Flush();
            fs.Close();
            var inputFile = new StreamReader(tempFilename);

            inputFile.Close();
            inputFile = null;

            return tempFilename;
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
        ///     Extract elements from an array of strings.
        /// </summary>
        /// <param name="sourceArray">Array containing data to extract.</param>
        /// <param name="idxStart">Index to start extraction.</param>
        /// <param name="numElements">Number of elements to extract.</param>
        /// <returns>Subarray extracted.</returns>
        public static string[] CopySubArray(string[] sourceArray, int idxStart, int numElements)
        {
            var result = new string[numElements];
            var destIndex = 0;
            var srcIndex = idxStart;

            for (var index = 0; index < numElements; index++)
            {
                result[destIndex] = sourceArray[srcIndex];
                destIndex++;
                srcIndex++;
            }

            return result;
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


        public static string StripPunctuation(string thisWord)
        {
            char[] punctuationChars = {',', '.', '\'', '"', ':', '?', '{', '}', '(', ')', '-', '!'};
            var result = thisWord;
            foreach (var thisChar in punctuationChars)
            {
                var tmpResult = result.Replace(thisChar, ' ');

                if (tmpResult != result)
                {
                    result = tmpResult;
                    result = result.Replace(" ", "");
                }
            }


            return result;
        }


        public static int CountChars(string stringToSearch, char charToCount)
        {
            var result = 0;
            if (stringToSearch != null)
                foreach (var thisChar in stringToSearch)
                    if (thisChar == charToCount)
                        result++;

            return result;
        }

        public static string GetTextBetween(string source, string leftToken, string rightToken)
        {
            var result = GetTextAfter(source, leftToken);
            if (result != null) result = GetTextBefore(result, rightToken);

            return result;
        }

        /// <summary>
        ///     Strips information about everything but table data.
        /// </summary>
        /// <param name="xmlText"></param>
        /// <returns></returns>
        public static string[] StripExtraneousTableMetadata(string xmlText)
        {
            var result = new StringArrayList();
            var startString = xmlText;
            while (startString.Length > 0)
            {
                var thisCell = GetTextBefore(startString, "</TD>");
                var cutLength = thisCell.Length;
                thisCell = thisCell.Trim();
                if (thisCell.Length > 0)
                {
                    if (thisCell.Substring(0, 4) == "<TD>") thisCell = GetTextAfter(thisCell, "<TD>");

                    if (thisCell.Contains("</TD>")) thisCell = GetTextBefore(thisCell, "</TD>");

                    thisCell = RemoveTokens(thisCell);

                    result.Add(thisCell);
                    startString = startString.Substring(cutLength, startString.Length - cutLength).Trim();
                    if (startString.Length > 5)
                        if (startString.Substring(0, 5) == "</TD>")
                            startString = GetTextAfter(startString, "</TD>");

                    startString = startString.Trim();
                }
                else
                {
                    break;
                }
            }

            return result.ToStringArray();
        }

        private static string RemoveTokens(string thisCell)
        {
            var result = "";
            var bracketCount = 0;

            if (thisCell.Contains("<") && thisCell.Contains(">"))
                for (var index = 0; index < thisCell.Length; index++)
                {
                    var thisCh = thisCell[index];
                    if (thisCh == '<')
                    {
                        bracketCount++;
                    }
                    else if (thisCh == '>')
                    {
                        bracketCount--;
                    }
                    else
                    {
                        if (bracketCount == 0 && index > 0) result += thisCh;
                    }

                    if (bracketCount != 0 && result.Length > 0) break;
                }
            else
                result = thisCell;

            return result;
        }

        public static string[] GetTableRows(string bodyText)
        {
            var result = new StringArrayList();

            var diminishingString = bodyText.Trim();
            while (diminishingString.Contains("</TR>"))
            {
                var thisLine = GetTextBefore(diminishingString, "</TR>");

                thisLine = thisLine.Trim();
                if (thisLine != "<TR>")
                {
                    var values = StripExtraneousTableMetadata(thisLine);
                    result.Add(string.Join(",", values));
                }

                diminishingString = GetTextAfter(diminishingString, "</TR>");
                diminishingString = diminishingString.Trim();
            }

            return result.ToStringArray();
        }

        /// <summary>
        ///     PackString removes all spaces from string.
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <returns></returns>
        public static string PackString(string sourceValue)
        {
            var result = "";
            foreach (var thisChar in sourceValue)
                if (thisChar != ' ')
                    result += thisChar;

            return result;
        }

        public static string TruncateAt(string originalLine, params char[] terminatingCharacters)
        {
            var result = originalLine;
            if (result.Length > 0)
                foreach (var ch in terminatingCharacters)
                {
                    result = TruncateAt(result, ch);
                    if (result.Length == 0) break;
                }

            return result;
        }

        public static string TruncateAt(string originalLine, char terminatingCharacter)
        {
            string result = null;
            var lineText = originalLine ?? "";
            if (lineText.Length > 0)
            {
                if (!originalLine.Contains(terminatingCharacter.ToString()))
                {
                    result = originalLine;
                }
                else
                {
                    var posOfTerminator = originalLine.IndexOf(terminatingCharacter);
                    result = originalLine.Substring(0,
                        posOfTerminator); // PosOfTerminator+1 would also return the terminating character,
                    // something I don't want.
                }
            }
            else
            {
                result = originalLine;
            }

            return result;
        }

        public static string RemoveBoundingCharacter(string stringToModify, char boundingCharacter)
        {
            var result = stringToModify;

            if (stringToModify.Length > 1)
                if (stringToModify[0] == boundingCharacter &&
                    stringToModify[stringToModify.Length - 1] == boundingCharacter)
                    result = stringToModify.Substring(1, stringToModify.Length - 2);

            return result;
        }

        public static string GetEmailExtension(string email)
        {
            var pos = email.LastIndexOf('.');
            string result = null;

            if (pos == -1)
                throw new InvalidDataException("Cannot find extension in " + email + ".");
            result = email.Substring(pos + 1, email.Length - (pos + 1));

            return result;
        }

        public static string[] TrimStringArray(string[] arrayToTrim)
        {
            var result = new string[arrayToTrim.Length];
            for (var index = 0; index < result.Length; index++) result[index] = arrayToTrim[index].Trim();

            return result;
        }

        public static string[] ToUpper(string[] originalArray)
        {
            var result = new string[originalArray.Length];
            for (var index = 0; index < result.Length; index++) result[index] = originalArray[index].ToUpper();

            return result;
        }

        public static string RemoveSubstring(string binName, string substringToRemove)
        {
            var result = binName;

            if (binName.Contains(substringToRemove))
                result = GetTextBefore(binName, substringToRemove) + GetTextAfter(binName, substringToRemove);

            return result;
        }

        public static string RemoveVowels(string parameterName)
        {
            var result = "";
            foreach (var ch in parameterName)
                if (!"aeiou".Contains("" + ch))
                    result += ch;

            return result;
        }

        public static string RemoveRecurringCharacterInstance(string stringToSearch, char characterToFind)
        {
            var result = "";


            var charCount = 0;
            foreach (var character in stringToSearch)
                if (character == characterToFind)
                {
                    if (charCount == 0) result += character;

                    charCount++;
                }
                else
                {
                    result += character;
                    charCount = 0;
                }

            return result;
        }

        public static string RemoveLongChars(string fileText)
        {
            var result = "";
            var size = fileText.Length;
            var resultArray = new char[size];
            var destIndex = 0;
            for (var sourceIndex = 0; sourceIndex < size; sourceIndex++)
            {
                var thisCharacter = fileText[sourceIndex];

                var asciiValue = (int) thisCharacter;
                var keep = true;


                if (asciiValue < 0x002D)
                    keep = false;
                else if (asciiValue == 0x002F)
                    keep = false;
                else if (asciiValue >= 0x003A && asciiValue <= 0x003F)
                    keep = false;
                else if (asciiValue >= 0x005B && asciiValue <= 0x0060)
                    keep = false;
                else if (asciiValue > 0x007B) keep = false;

                if (keep)
                {
                    resultArray[destIndex] = thisCharacter;
                    destIndex++;
                }
                else
                {
                    if (thisCharacter != '\0')
                        if (destIndex > 2)
                            if (resultArray[destIndex - 1] != ' ')
                            {
                                resultArray[destIndex] = ' ';
                                destIndex++;
                            }
                }
            }

            var sb = new StringBuilder(null);

            var sw = new StringWriter(sb);

            result = "";

            sw.Write(resultArray, 0, destIndex + 1);
            result = sb.ToString();

            return result;
        }

        public static string ReplaceFirstOccurrence(string sourceString, char chToFind, char replacementCh)
        {
            var pos = sourceString.IndexOf(chToFind);
            string result = null;

            if (pos >= 0)
            {
                var tempCharArray = sourceString.ToCharArray();
                tempCharArray[pos] = replacementCh;

                foreach (var ch in tempCharArray) result += ch;
            }
            else
            {
                result = sourceString;
            }

            return result;
        }

        public static int FindFirstMatch(string[] arrayToSearch, string valueToFind)
        {
            var result = -1;
            for (var index = 0; index < arrayToSearch.Length; index++)
                if (arrayToSearch[index].Contains(valueToFind))
                {
                    result = index;
                    break;
                }

            return result;
        }

        private const NumberStyles MDefaultFloatParseStyles =
            NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent;

        public static string GetTrailingNumPartOfString(string strToExtractNumPartFrom,
            NumberStyles floatParseStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
            CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null) cultureInfo = new CultureInfo("en-US");

            string result = null;
            for (var index = strToExtractNumPartFrom.Length - 1; index > -1; index--)
            {
                var thisPosNumValue = 0;
                var ch = strToExtractNumPartFrom.Substring(index, 1);

                if (int.TryParse(ch, floatParseStyles, cultureInfo, out thisPosNumValue))
                {
                    if (result == null)
                        result = ch;
                    else
                        result = ch + result;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

#if false
        public static string ShuffleString(string ValueToShuffle) {
            
            char[] DestChars = ValueToShuffle.ToCharArray();
            if(ValueToShuffle.Length>1){
                int NumCycles = DestChars.Length;
                for(int Cycle = 0;Cycle<NumCycles;Cycle++){
                    for(int ValueIndex = 0;ValueIndex<ValueToShuffle.Length;ValueIndex++){
                        int SwapPos = SkeeterRandom.GetRandomNumber(0, DestChars.Length-1);
                        if(SwapPos!=ValueIndex){
                            char Temp = DestChars[ValueIndex];
                            DestChars[ValueIndex] = DestChars[SwapPos];
                            DestChars[SwapPos] = Temp;
                        }
                    }
                }
            }
         
            StringBuilder Result = new StringBuilder();
            Result.Append(DestChars);
            //return Result.ToString();   
            
            return new StringBuilder().Append(DestChars).ToString();
        }
#endif

        public static string TextBetween(this string value, string leftToken, string rightToken)
        {
            return GetTextBetween(value, leftToken, rightToken);
        }


        public static double[] Parse(this string[] value, bool nanForParseErrors = false,
            NumberStyles floatParseStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
            CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null) cultureInfo = new CultureInfo("en-US");

            var result = new double[value.Length];
            for (var index = 0; index < value.Length; index++)
            {
                var thisValue = double.NaN;
                try
                {
                    thisValue = double.Parse(value[index]);
                    result[index] = thisValue;
                }
                catch (Exception ex)
                {
                    if (nanForParseErrors)
                        result[index] = double.NaN;
                    else
                        throw new Exception("Could not parse " + value[index] + " to a floating point representation.",
                            ex);
                }
            }

            return result;
        }

        public static string CapitalizeFirstLetter(string wordToCapitalize)
        {
            if (wordToCapitalize.Length > 0)
                return wordToCapitalize.Substring(0, 1).ToUpper() +
                       RightStr(wordToCapitalize, wordToCapitalize.Length - 1);
            return wordToCapitalize;
        }

        // Zero in the NrTimes arguments causes an empty string ("") to be returned
        public static string RepeatString(string stringToRepeat, int nrTimes)
        {
            var result = "";
            for (var index = 0; index < nrTimes; index++) result += stringToRepeat;

            return result;
        }
    }


    /// <summary>
    ///     Enumeration allowing the specifying which end a string operation is performed on.
    /// </summary>
    public enum EWhichEnd
    {
        WeFrontEnd,
        WeBackEnd,
        WeBothEnds
    }

    /// <summary>
    ///     Enumeration specifying how a compare should be performed.
    /// </summary>
    public enum ECompareType
    {
        CtInStr,
        CtLeft,
        CtWhole
    }
}