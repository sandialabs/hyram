/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
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