/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;

namespace SandiaNationalLaboratories.Hyram
{
    public class GridColumnTemplate
    {
        private Dictionary<int, string> _mColumnIdxToNameReference;

        private Dictionary<string, int> _mColumnNameToIdxReference;

        public GridColumnTemplate(string[] headers)
        {
            Headers = new string[headers.Length];
            ReadOnlyColumns = new bool[headers.Length];
            ColumnFormatStrings = new string[headers.Length];
            for (var i = 0; i < headers.Length; ++i) ColumnFormatStrings[i] = "G";

            headers.CopyTo(Headers, 0);
            CreateGridColumnIndexes();
        }

        public string[] Headers { get; }

        public string[] ColumnFormatStrings { get; set; }

        public bool[] ReadOnlyColumns { get; set; }


        public string[] FirstColumnValues { get; set; } = null;

        public string GetColumnName(int columnIndex)
        {
            if (_mColumnIdxToNameReference.ContainsKey(columnIndex))
                return _mColumnIdxToNameReference[columnIndex];
            throw new Exception("Column #" + columnIndex + " does not exist in referenced grid column template.");
        }

        public int GetColumnIndex(string columnName)
        {
            if (_mColumnNameToIdxReference.ContainsKey(columnName))
                return _mColumnNameToIdxReference[columnName];
            throw new Exception("Column " + columnName + " does not exist in referenced grid column template.");
        }

        private void CreateGridColumnIndexes()
        {
            _mColumnIdxToNameReference = new Dictionary<int, string>();
            _mColumnNameToIdxReference = new Dictionary<string, int>();
            for (var index = 0; index < Headers.Length; index++)
            {
                _mColumnNameToIdxReference.Add(Headers[index], index);
                _mColumnIdxToNameReference.Add(index, Headers[index]);
            }
        }
    }
}