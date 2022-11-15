/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System.IO;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    ///     Summary description for QuickFunctions.
    /// </summary>
    public class QuickFunctions
    {
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
            if (result.Length > 0 && cancelOrNot != DialogResult.Cancel)
            {
                startingPath = GetFileDirectory(result);
            }

            return result;
        }

        public static string SelectFilename(string title, ref string startingPath, string filter,
                                            bool checkFileExists = true)
        {
            var result = "";
//            var resultArray = SelectFilenames(title, ref startingPath, filter, false, checkFileExists);

            // Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
            if (filter.Length == 5 || filter.Length == 3)
            {
                filter = filter + " Files (" + filter + ")|" + filter;
            }

            var fd = new OpenFileDialog();

            if (!string.IsNullOrEmpty(startingPath) && !Directory.Exists(startingPath))
            {
                startingPath = DropBottomDirName(startingPath);
            }

            fd.CheckFileExists = checkFileExists;
            fd.Title = title;
            fd.InitialDirectory = startingPath;
            fd.Filter = filter;
            fd.Multiselect = false;
            var dlgResult = fd.ShowDialog();

            string[] resultArray = null;
            if (dlgResult == DialogResult.OK)
            {
                resultArray = fd.FileNames;
                if (resultArray != null && resultArray.Length > 0 && resultArray[0].Length > 0)
                {
                    startingPath = Path.GetDirectoryName(resultArray[0]);
                }
            }

            if (resultArray != null && resultArray.Length == 1)
            {
                result = resultArray[0];
            }

            return result;
        }


        private static double GetNumericPartOfCellValueString(string cellValue)
        {
            // If successful full parse, return the value. Otherwise, try to find
            // numeric first part of string
            if (!double.TryParse(cellValue, out var result))
                if (cellValue.Length > 0)
                {
                    string numericPart = "";
                    for (var index = 0; index < cellValue.Length; index++)
                    {
                        var thisCh = cellValue.Substring(index, 1);
                        if (ParseUtility.IsParseableNumber(result + thisCh))
                        {
                            numericPart += thisCh;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (numericPart.Length > 0) double.TryParse(numericPart, out result);
                }

            return result;
        }

        private static string ConditionalChop(string source, char ifLastValueIs)
        {
            string result = null;

            if (source.Length > 0)
            {
                var thisCh = source[source.Length - 1];
                result = thisCh == ifLastValueIs ? source.Substring(0, source.Length - 1) : source;
            }
            if (result == null)
            {
                result = source;
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
                result = ConditionalChop(result, '\\');
                result = ConditionalChop(result, '/');
            }

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

//        private static string[] SelectFilenames(string title, ref string startingPath, string filter, bool multiselect,
//                                                bool checkFileExists = true)
//        {
//            // Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*" ;
//            if (filter.Length == 5 || filter.Length == 3) filter = filter + " Files (" + filter + ")|" + filter;
//
//            var fd = new OpenFileDialog();
//
//            if (!string.IsNullOrEmpty(startingPath) && !Directory.Exists(startingPath))
//            {
//                startingPath = DropBottomDirName(startingPath);
//            }
//
//            fd.CheckFileExists = checkFileExists;
//
//            fd.Title = title;
//            fd.InitialDirectory = startingPath;
//            fd.Filter = filter;
//            fd.Multiselect = multiselect;
//            var dlgResult = fd.ShowDialog();
//
//            string[] result = null;
//
//            if (dlgResult == DialogResult.OK)
//            {
//                result = fd.FileNames;
//                if (result != null && result.Length > 0 && result[0].Length > 0)
//                {
//                    startingPath = Path.GetDirectoryName(result[0]);
//                }
//            }
//            return result;
//        }

    }
}