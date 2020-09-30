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
using System.IO;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    ///     Summary description for QuickFunctions.
    /// </summary>
    public class QuickFunctions
    {
        public static void PreventGridColumnSorting(DataGridView grid)
        {
            foreach (DataGridViewColumn thisColumn in grid.Columns)
                thisColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
        }

        public static void PerformNumericSortOnGrid(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.CellValue1 != null && e.CellValue2 != null)
            {
                var cellValue1 = GetNumericPartOfCellValueString(e.CellValue1.ToString());
                var cellValue2 = GetNumericPartOfCellValueString(e.CellValue2.ToString());

                if (!double.IsNaN(cellValue1) && !double.IsNaN(cellValue2))
                {
                    if (cellValue1 == cellValue2)
                        e.SortResult = 0;
                    else if (cellValue1 < cellValue2)
                        e.SortResult = -1;
                    else
                        e.SortResult = 1;

                    e.Handled = true;
                }
            }
        }

        private static double GetNumericPartOfCellValueString(string cellValue)
        {
            var result = double.NaN;
            // If successful full parse, return the value. Otherwise, try to find
            // numeric first part of string
            if (!ParseUtility.TryParseDouble(cellValue, out result))
                if (cellValue.Length > 0)
                {
                    var numericPart = GetPrependedNumericPartOfString(cellValue);
                    if (numericPart.Length > 0) ParseUtility.TryParseDouble(numericPart, out result);
                }

            return result;
        }

        private static string GetPrependedNumericPartOfString(string cellValue)
        {
            var result = "";
            for (var index = 0; index < cellValue.Length; index++)
            {
                var thisCh = cellValue.Substring(index, 1);
                if (ParseUtility.IsParseableNumber(result + thisCh))
                    result += thisCh;
                else
                    break;
            }

            return result;
        }

        // Find the first parent of specified type, from control.  An example would be to find the Form that owns the current
        // control. Another example might be a TabControl.
        public static Control GetFirstParentOfSpecifiedType(Control childToChaseParentFrom, Type specifiedParentType)
        {
            Control result = null;
            var parentVar = childToChaseParentFrom.Parent;
            var done = false;

            while (!done)
            {
                var parentControlType = parentVar.GetType();
                if (parentControlType == specifiedParentType)
                {
                    result = parentVar;
                    done = true;
                }
                else
                {
                    parentVar = parentVar.Parent;
                }

                if (parentVar == null) done = true;
            }

            return result;
        }

        public static Button GetTopButton(Control buttonParent, ChildNavOptions navOptions)
        {
            Button result = null;

            foreach (Control thisControl in buttonParent.Controls)
                if (thisControl is Button)
                {
                    if (result == null)
                        result = (Button) thisControl;
                    else if (result.Top > ((Button) thisControl).Top) result = (Button) thisControl;
                }
                else if (thisControl.HasChildren && navOptions == ChildNavOptions.NavigateChildren)
                {
                    result = GetTopButton(thisControl, ChildNavOptions.NavigateChildren);
                    if (result != null) break;
                }

            return result;
        }

        private static string GetFileDirectory(string path)
        {
            var filename = Path.GetFileName(path);
            string result = null;

            if (filename.Length > 0)
            {
                result = path.Substring(0, path.Length - filename.Length);
                result = StringFunctions.ConditionalChop(result, '\\');
                result = StringFunctions.ConditionalChop(result, '/');
            }

            return result;
        }

        public static string SelectSaveAsFilename(string title, ref string startingPath, string extension, string filter)
        {
            var fd = new SaveFileDialog
            {
                DefaultExt = extension,
                Filter = filter,
                InitialDirectory = startingPath,
                OverwritePrompt = true,
                Title = title,
                ValidateNames = true
            };

            var cancelOrNot = fd.ShowDialog();

            var result = fd.FileName;
            fd.Dispose();
            fd = null;
            if (result.Length > 0 && cancelOrNot != DialogResult.Cancel)
                startingPath = GetFileDirectory(result);

            return result;
        }

        private static string DropBottomDirName(string directoryName)
        {
            // Get bottom dir name
            var bottomDirName = "";
            var lastPos = directoryName.LastIndexOf("\\");
            if (lastPos != -1)
            {
                lastPos++;
                bottomDirName = directoryName.Substring(lastPos, directoryName.Length - lastPos);
            }

            var result = directoryName.Substring(0, directoryName.Length - bottomDirName.Length);

            // Remove trailing backslash
            if (result.Length > 0)
                while (result[result.Length - 1] == '\\' || result[result.Length - 1] == '/')
                {
                    result = result.Substring(0, result.Length - 1);
                    if (result.Length == 0) break;
                }

            return result;
        }

        public static string[] SelectFilenames(string title, ref string startingPath, string filter, bool multiselect,
            bool checkFileExists = true)
        {
            // Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
            if (filter.Length == 5 || filter.Length == 3) filter = filter + " Files (" + filter + ")|" + filter;

            var fd = new OpenFileDialog();

            if (startingPath != null)
                if (startingPath.Length > 0)
                    if (!Directory.Exists(startingPath))
                        startingPath = DropBottomDirName(startingPath);

            fd.CheckFileExists = checkFileExists;

            fd.Title = title;
            fd.InitialDirectory = startingPath;
            fd.Filter = filter;
            fd.Multiselect = multiselect;
            var dlgResult = fd.ShowDialog();

            string[] result = null;

            if (dlgResult == DialogResult.OK)
            {
                result = fd.FileNames;
                if (result != null)
                    if (result.Length > 0)
                        if (result[0].Length > 0)
                            startingPath = Path.GetDirectoryName(result[0]);
            }
            return result;
        }

        public static string SelectFilename(string title, ref string startingPath, string filter,
            bool checkFileExists = true)
        {
            string[] resultArray = null;
            var result = "";

            resultArray = SelectFilenames(title, ref startingPath, filter, false, checkFileExists);
            if (resultArray != null)
                if (resultArray.Length == 1)
                    result = resultArray[0];

            return result;
        }
    }

    public enum ChildNavOptions
    {
        NavigateChildren,
        TopLevelOnly
    }
}