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

namespace EssStringLib
{
    /// <summary>
    ///     Specifies the search direction: Forward or Reverse.
    /// </summary>
    public enum SearchDirection
    {
        SdForward = 0,
        SdReverse = 1
    }

    /// <summary>
    ///     The TCompareType enum specifies how a string comparison should be made.
    /// </summary>
    public enum CompareType
    {
        CtInStr,
        CtLeft,
        CtRight
    }

    [Flags]
    /// <summary>
    /// Options for searching strings.
    /// </summary>
    public enum StringSearchOptions
    {
        NoneSpecified = 0x00,
        TrimFirst = 0x01,
        CaseSensitive = 0x02,
        PackFirst = 0x04
    }

    public enum ArrayStringConversionOption
    {
        AppendCarriageReturn,
        NoModifications,
        AppendCrlf
    }

    /// <summary>
    ///     Encapsulates a string array that can be resized, searched and manipulated without
    ///     having to call external functions or assign the result of a method call on one array
    ///     to a new one.  The Data property contains the array.
    /// </summary>
    public class ClsEditableStringArray
    {
        private string[] _mData = new string[0];

        /// <summary>
        ///     Create new instance by copying data from another instance.
        /// </summary>
        /// <param name="saClassToCopyFrom"></param>
        public ClsEditableStringArray(ClsEditableStringArray saClassToCopyFrom)
        {
            _mData = saClassToCopyFrom.Copy(0, saClassToCopyFrom.Data.Length);
        }

        /// <summary>
        ///     Create a new instance by referencing a preexisting string array.
        /// </summary>
        /// <param name="startingData">Data to reference.</param>
        public ClsEditableStringArray(string[] startingData)
        {
            _mData = startingData;
        }

        public ClsEditableStringArray(string[] startingData, bool copy)
        {
            if (!copy)
                _mData = startingData;
            else
                _mData = CopyArray(startingData);
        }

        /// <summary>
        ///     Create a new instance by parsing a string containing multiple lines.
        /// </summary>
        /// <param name="multipleLinesInString">A string containing multiple lines separated by carriage-return or CR/LF.</param>
        public ClsEditableStringArray(string multipleLinesInString)
        {
            _mData = StringFunctions.GetStringArrayFromMultiLinedString(multipleLinesInString);
        }

        /// <summary>
        ///     Default constructor.  A new array with zero elements is created.
        /// </summary>
        public ClsEditableStringArray()
        {
        }


        public string this[int index]
        {
            get => _mData[index];
            set => _mData[index] = value;
        }


        /// <summary>
        ///     Returns the widest string in the Data array.
        /// </summary>
        public string WidestString => ArrayFunctions.GetWidestArrayString(_mData);

        /// <summary>
        ///     Returns the length of the widest string in the Data array.
        /// </summary>
        public int MaxStringLength => ArrayFunctions.GetWidestStringWidth(_mData);

        /// <summary>
        ///     The string array that this class operates upon.
        /// </summary>
        public string[] Data
        {
            get => _mData;
            set => _mData = value;
        }

        public bool ContainsSubstring(string stringToFind, bool caseSensitive)
        {
            string stf = null;
            var result = false;

            if (caseSensitive)
                stf = stringToFind;
            else
                stf = stringToFind.ToUpper();


            foreach (var thisLine in Data)
            {
                string lineToTest = null;


                if (caseSensitive)
                    lineToTest = thisLine;
                else
                    lineToTest = thisLine.ToUpper();

                if (lineToTest.Contains(stringToFind))
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public ClsEditableStringArray GetLinesBetween_ToESA(string startLine, string endLine, CompareType compareType,
            bool trimFirst, bool caseSensitive)
        {
            var resultDataProp = GetLinesBetween(startLine, endLine, compareType, trimFirst, caseSensitive);
            var result = new ClsEditableStringArray(resultDataProp);
            return result;
        }

        public string[] GetLinesBetween(string startLine, string endLine, CompareType compareType, bool trimFirst,
            bool caseSensitive)
        {
            var result = new string[0];

            var startIndex = SeekForValue(0, startLine, compareType, trimFirst, caseSensitive);
            if (startIndex > -1)
            {
                var endIndex = SeekForValue(startIndex + 1, endLine, compareType, trimFirst, caseSensitive);
                if (endIndex > startIndex) result = ExtractSubArray(startIndex, endIndex);
            }

            return result;
        }

        /// <summary>
        ///     Removes all blank lines in the Data array.
        /// </summary>
        public void RemoveBlankLines()
        {
            RemoveBlankLines(false);
        }

        public void RemoveBlankLines(bool trimFirst)
        {
            for (var lineIndex = 0; lineIndex < _mData.Length; lineIndex++)
            {
                var thisLine = _mData[lineIndex];
                if (trimFirst) thisLine = thisLine.Trim();

                if (thisLine.Length == 0)
                {
                    Delete(lineIndex, 1);
                    lineIndex--;
                }
            }
        }


        /// <summary>
        ///     Find a value in the Data array.  Can specify search direction.
        /// </summary>
        /// <param name="startingPos">Index into array at which search begins.</param>
        /// <param name="value">Value to find.</param>
        /// <param name="compareType">Type of comparison to perform.</param>
        /// <param name="TrimFirst">Whether or not to trim before performing comparison.</param>
        /// <param name="CaseSensitive">Whether or not search is case-sensitive.</param>
        /// <param name="searchDirection">Direction of search (forward or reverse).</param>
        /// <returns>Index of found string if successful; -1 if no match was found.</returns>
        public int SeekForValue(int startingPos, string value, CompareType compareType,
            StringSearchOptions stringSearchOptions, SearchDirection searchDirection)
        {
            var result = -1;
            string valueToFind = null;

            var trimFirst = StringSearchOptionsContains(stringSearchOptions, StringSearchOptions.TrimFirst);
            var caseSensitive = StringSearchOptionsContains(stringSearchOptions, StringSearchOptions.CaseSensitive);
            var packFirst = StringSearchOptionsContains(stringSearchOptions, StringSearchOptions.PackFirst);


            if (trimFirst)
                valueToFind = value.Trim();
            else
                valueToFind = value;

            if (packFirst) valueToFind = StringFunctions.PackString(valueToFind);

            if (!caseSensitive) valueToFind = valueToFind.ToUpper();

            if (compareType == CompareType.CtRight) valueToFind = StringFunctions.ReverseStr(valueToFind);

            var done = false;
            var currentRow = startingPos;

            while (!done)
            {
                if (currentRow >= _mData.Length || currentRow < 0)
                {
                    done = true;
                }
                else
                {
                    var preparedLineValue = _mData[currentRow];
                    if (preparedLineValue != null)
                    {
                        if (trimFirst) preparedLineValue = preparedLineValue.Trim();

                        if (packFirst) preparedLineValue = StringFunctions.PackString(preparedLineValue);


                        if (compareType == CompareType.CtRight)
                            preparedLineValue = StringFunctions.ReverseStr(preparedLineValue);

                        if (!caseSensitive) preparedLineValue = preparedLineValue.ToUpper();

                        var itemFound = false;

                        if (compareType == CompareType.CtInStr)
                        {
                            if (preparedLineValue.IndexOf(valueToFind) != -1) itemFound = true;
                        }
                        else if (compareType == CompareType.CtLeft || compareType == CompareType.CtRight)
                        {
                            if (preparedLineValue.Length >= valueToFind.Length)
                            {
                                var truncatedPart = preparedLineValue.Substring(0, valueToFind.Length);

                                if (truncatedPart == valueToFind) itemFound = true;
                            }
                        }

                        if (itemFound)
                        {
                            result = currentRow;
                            done = true;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (searchDirection == SearchDirection.SdForward)
                    currentRow++;
                else
                    currentRow--;
            }

            return result;
        }

        private bool StringSearchOptionsContains(StringSearchOptions stringSearchOptions,
            StringSearchOptions stringSearchOptionsToFind)
        {
            var comparator = stringSearchOptions | stringSearchOptionsToFind;

            var result = false;

            if (comparator == stringSearchOptions) result = true;

            return result;
        }


        /// <summary>
        ///     Returns the index into the Data Array, of the matching value.
        /// </summary>
        /// <param name="startingPos">Index at which to start the search</param>
        /// <param name="value">Value to compare.</param>
        /// <param name="compareType">Type of comparison to perform.</param>
        /// <param name="trimFirst">Whether or not to trim each string before performing comparison (i.e., to ignore spaces).</param>
        /// <param name="caseSensitive">Whether or not the search is case sensitive.</param>
        /// <returns>The index of the first match.  -1 if no match could be found.</returns>
        public int SeekForValue(int startingPos, string value, CompareType compareType,
            bool trimFirst, bool caseSensitive)
        {
            var searchOptions = StringSearchOptions.NoneSpecified;
            if (trimFirst) searchOptions = StringSearchOptions.TrimFirst;

            if (caseSensitive) searchOptions = searchOptions | StringSearchOptions.CaseSensitive;


            return SeekForValue(startingPos, value, compareType, searchOptions, SearchDirection.SdForward);
        }

        public int FindSubstringMatch(int startingPos, string value)
        {
            var result = SeekForValue(startingPos, value, CompareType.CtInStr, false, false);
            return result;
        }

        public int[] SeekForValues(int[] startingPos, string[] values, CompareType compareType,
            bool trimFirst, bool caseSensitive)
        {
            var result = new int[values.Length];
            for (var index = 0; index < values.Length; index++) result[index] = -1;

            for (var index = 0; index < values.Length; index++)
                if (startingPos[index] == -1)
                    result[index] = -1;
                else
                    result[index] = SeekForValue(startingPos[index], values[index], compareType, trimFirst,
                        caseSensitive);

            return result;
        }


        public string CombineToString(ArrayStringConversionOption conversionOption)
        {
            var result = "";
            var appendValue = "";

            if (conversionOption == ArrayStringConversionOption.AppendCarriageReturn)
                appendValue = "\r";
            else if (conversionOption == ArrayStringConversionOption.AppendCrlf) appendValue = "\r\n";

            foreach (var thisLine in Data)
                if (result != "")
                    result += appendValue + thisLine;
                else
                    result += thisLine;

            return result;
        }


        /// <summary>
        ///     Copy lines from the Data Array to a new string array.
        /// </summary>
        /// <param name="startingIndex">Index into Data Array where copying begins.</param>
        /// <param name="numLines">Number of lines to copy.</param>
        /// <returns>A new string array containing the lines copied.</returns>
        public string[] Copy(int startingIndex, int numLines)
        {
            var result = new string[numLines];
            var destIndex = 0;
            var lastLineIndex = startingIndex + numLines;
            for (var index = startingIndex; index < lastLineIndex; index++)
            {
                result[destIndex] = _mData[index];
                destIndex++;
            }

            return result;
        }


        /// <summary>
        ///     Insert an array into the Data Array.
        /// </summary>
        /// <param name="linesToInsert">An array containing the data to insert.</param>
        /// <param name="beforeIndex">Point at which to make insertion.</param>
        public void Insert(string[] linesToInsert, int beforeIndex)
        {
            _mData = StringFunctions.InsertDataIntoArray(_mData, linesToInsert, beforeIndex);
        }

        /// <summary>
        ///     Extract lines from StartIndex to EndIndex from Data Array.
        /// </summary>
        /// <param name="startIndex">Index of first line.</param>
        /// <param name="endIndex">Index of last line.</param>
        /// <returns>The extracted array.</returns>
        public string[] ExtractSubArray(int startIndex, int endIndex)
        {
            var result = new string[endIndex - startIndex + 1];
            var destIndex = 0;
            for (var sourceIndex = startIndex; sourceIndex < endIndex + 1; sourceIndex++)
            {
                result[destIndex] = _mData[sourceIndex];
                destIndex++;
            }

            return result;
        }

        /// <summary>
        ///     Inserts one line into Data Array.
        /// </summary>
        /// <param name="lineToInsert">Line of data to insert.</param>
        /// <param name="beforeIndex">Index to insert lines before.</param>
        public void Insert(string lineToInsert, int beforeIndex)
        {
            var insertLines = new string[1];
            insertLines[0] = lineToInsert;
            Insert(insertLines, beforeIndex);
        }

        /// <summary>
        ///     Find the index of a line that matches LineToMatch.  Search is case-
        ///     sensitive and compares each complete string.  No substring comparisons
        ///     are made.
        /// </summary>
        /// <param name="lineToMatch">The string to search for.</param>
        /// <param name="posToStartAt">Position at which to begin search.</param>
        /// <returns>Index of matched string.  -1 if no match is found.</returns>
        public int FindFirstMatchFromPos(string lineToMatch, int posToStartAt)
        {
            var result = -1;

            for (var index = posToStartAt; index < _mData.Length; index++)
            {
                var thisLine = _mData[index];
                if (thisLine == lineToMatch)
                {
                    result = index;
                    break;
                }
            }

            return result;
        }

        public void AppendLine()
        {
            Append("");
        }

        /// <summary>
        ///     Add a new line to the end of the Data Array.
        /// </summary>
        /// <param name="elementToAppend">New line to append.</param>
        public void Append(string elementToAppend)
        {
            var elem = new string[1];
            elem[0] = elementToAppend;
            Append(elem);
        }

        /// <summary>
        ///     Insert a new line at the beginning of the Data Array.
        /// </summary>
        /// <param name="elementToPrepend">Line to prepend.</param>
        public void Prepend(string elementToPrepend)
        {
            var elem = new string[1];
            elem[0] = elementToPrepend;
            Prepend(elem);
        }

        public void Prepend(ClsEditableStringArray objToPrepend)
        {
            Prepend(objToPrepend.Data);
        }


        /// <summary>
        ///     Insert an array of lines to the beginning of the Data Array.
        /// </summary>
        /// <param name="dataToPrepend">The array of lines to prepend.</param>
        public void Prepend(string[] dataToPrepend)
        {
            var newData = new string[dataToPrepend.Length + _mData.Length];
            dataToPrepend.CopyTo(newData, 0);
            _mData.CopyTo(newData, dataToPrepend.Length);
            _mData = newData;
        }

        /// <summary>
        ///     Add an array of lines to the end of the Data Array.
        /// </summary>
        /// <param name="dataToAppend">The array of lines to append.</param>
        public void Append(string[] dataToAppend)
        {
            var newData = new string[dataToAppend.Length + _mData.Length];
            _mData.CopyTo(newData, 0);
            dataToAppend.CopyTo(newData, _mData.Length);
            _mData = newData;
        }

        /// <summary>
        ///     Remove all duplicate lines from the array.
        /// </summary>
        public void RemoveDuplicates()
        {
            ArrayFunctions.RemoveArrayDupes(ref _mData);
        }

        /// <summary>
        ///     Remove all lines from Data Array that meet specified criteria.  Matching is done based on the CompareType
        ///     argument: regular expressions are not supported.
        /// </summary>
        /// <param name="filter">The string containing the data to delete.</param>
        /// <param name="compareType">The type of search to perform.</param>
        /// <param name="caseSensitive">Whether or not to perform a case-sensitive search.</param>
        /// <param name="deleteLinesThatMatch">If true, delete matching lines.  If false, delete lines not matching.</param>
        public void CrunchArray(string filter, ECompareType compareType, bool caseSensitive, bool deleteLinesThatMatch)
        {
            _mData = StringFunctions.CrunchArray(_mData, filter, compareType, caseSensitive, deleteLinesThatMatch);
        }

        private string[] CopyArray(string[] source)
        {
            var result = new string[source.Length];
            for (var index = 0; index < source.Length; index++) result[index] = source[index];

            return result;
        }

        public bool ContainsLine(string lineToFind)
        {
            return ArrayFunctions.ArrayHasValue(_mData, lineToFind, true);
        }

        /// <summary>
        ///     Delete elements in Data Array.
        /// </summary>
        /// <param name="index">Index to first element deleted.</param>
        /// <param name="numItems">Number of lines deleted.</param>
        public void Delete(int index, int numItems)
        {
            var newData = new string[_mData.Length - numItems];

            if (numItems <= 0)
                throw new Exception("clnStringFile.Delete failed.  You have to specify at least one item to delete.");

            var sourceIndex = 0;
            var destIndex = 0;

            while (sourceIndex < index)
            {
                newData[destIndex] = _mData[sourceIndex];
                sourceIndex++;
                destIndex++;
            }

            sourceIndex += numItems;

            while (sourceIndex < _mData.Length)
            {
                newData[destIndex] = _mData[sourceIndex];
                sourceIndex++;
                destIndex++;
            }

            _mData = newData;

            if (destIndex != _mData.Length)
                throw new Exception("Destination index is invalid in clsStringFile.Delete.");
        }


        public void RemoveTextBetween(char leftValue, char rightValue)
        {
            for (var index = _mData.Length - 1; index > -1; index--)
            {
                var originalValue = _mData[index];

                var newValue = originalValue;

                var Continue = true;
                while (Continue)
                {
                    Continue = false;
                    var leftPos = newValue.IndexOf(leftValue, 0);
                    var rightPos = -1;

                    if (leftPos > -1)
                    {
                        rightPos = newValue.IndexOf(rightValue, leftPos + 1);


                        if (rightPos != -1 && rightPos > leftPos)
                        {
                            var leftSideStringPart = newValue.Substring(0, leftPos);
                            var numRightChars = newValue.Length - rightPos - 1;

                            var rightSideStringPart = newValue.Substring(rightPos + 1, numRightChars);
                            newValue = leftSideStringPart + rightSideStringPart;
                            Continue = true;
                        }
                    }
                }

                if (originalValue != newValue) _mData[index] = newValue;
            }
        }

        public void TruncateLinesAtChars(char[] charsToTreatAsEndline)
        {
            for (var index = 0; index < _mData.Length; index++)
            {
                var thisLine = _mData[index];
                foreach (var thisChar in charsToTreatAsEndline)
                {
                    var pos = thisLine.IndexOf(thisChar);
                    if (pos > -1)
                    {
                        var newLine = thisLine.Substring(0, pos);
                        _mData[index] = newLine;
                        break;
                    }
                }
            }
        }

        public void DeleteEmptyLines()
        {
            for (var index = _mData.Length - 1; index > -1; index--)
            {
                var dataLine = _mData[index].Trim();
                if (dataLine.Length == 0) Delete(index, 1);
            }
        }

        public string ExtractCombinedLine(int startPos, int endPos, bool trimFirst)
        {
            if (Data.Length > 0)
            {
                var result = new ClsEditableStringArray(Data);
                result.CombineLines(startPos, endPos, trimFirst);
                return result.Data[0];
            }

            return "";
        }

        public void CombineLines(int startPos, int endPos, bool trimFirst)
        {
            var combinedLine = "";
            for (var index = startPos; index <= endPos; index++)
                if (!trimFirst)
                    combinedLine += this[index];
                else
                    combinedLine += this[index].Trim();

            this[startPos] = combinedLine;
            var numLinesToDelete = endPos - startPos;

            Delete(startPos + 1, numLinesToDelete);
        }

        public void ReplaceLine(int index, string newValue)
        {
            _mData[index] = newValue;
        }

        public int ReplaceText(string textToFind, string replacementText, int startIndex, int endIndex)
        {
            var numberOfReplacements = 0;
            for (var index = startIndex; index < _mData.Length; index++)
            {
                if (startIndex < 0) break;

                if (index > endIndex) break;

                if (_mData[index].Contains(textToFind))
                {
                    _mData[index] = _mData[index].Replace(textToFind, replacementText);
                    numberOfReplacements++;
                }
            }


            return numberOfReplacements;
        }

        public void TrimAll()
        {
            for (var index = 0; index < _mData.Length; index++) _mData[index] = _mData[index].Trim();
        }

        public void Clear()
        {
            _mData = new string[0];
        }

        public string[] GetLinesToEmptyLine(int startPos)
        {
            var sal = new StringArrayList();
            string[] result = null;

            for (var lineIndex = startPos; lineIndex < _mData.Length; lineIndex++)
            {
                var thisLine = _mData[lineIndex];
                if (thisLine.Trim().Length == 0) break;

                sal.Add(thisLine);
            }


            if (sal.Count > 0) result = sal.ToStringArray();

            return result;
        }


        public void RemoveWhitespace()
        {
            for (var index = 0; index < _mData.Length; index++)
                _mData[index] = StringFunctions.RemoveCharsFromString(_mData[index], "\r\t\n ".ToCharArray());
        }

        public int GetMaxDelimitedColCount(string delimiter = "\t")
        {
            var result = 0;

            foreach (var thisLine in Data)
            {
                var lineToCheck = thisLine ?? "";
                if (lineToCheck.Length == 0)
                {
                    if (result == 0) result = 1;
                }
                else
                {
                    var numDelimiter = lineToCheck.Split(delimiter.ToCharArray()).Length + 1;
                    if (numDelimiter > result) result = numDelimiter;
                }
            }

            return result;
        }
    }
}