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
using System.Globalization;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class StaticGridHelperRoutines
    {

        public static void InitInteractiveGrid(DataGridView grid, ParameterWrapperCollection valueDefinitionObjects,
            bool canSort = true)
        {
            var initGridCalled = false;

            foreach (var thisVdo in valueDefinitionObjects.Values)
            {
                if (!initGridCalled)
                {
                    InitGrid(grid, new[] {"colVariable", "colValue", "colUnit"},
                        new[] {"Variable", "Value", "Unit"}, canSort);
                    initGridCalled = true;
                }

                AddLinkedValueRowToDataGrid(grid, null, thisVdo);
            }

            QuickFunctions.PreventGridColumnSorting(grid);
            grid.HandleCreated += RespondToEventAndResizeColumns;
        }

        private static void RespondToEventAndResizeColumns(object sender, EventArgs e)
        {
            ((DataGridView) sender).AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        public static void ProcessDataGridViewRowValueChangedEvent(DataGridView dg, DataGridViewCellEventArgs e,
            int dataColumnindex, int unitColumnindex, bool ignoringGridChangeEvents)
        {
            if (!ignoringGridChangeEvents)
                if (e.ColumnIndex == dataColumnindex || e.ColumnIndex == unitColumnindex)
                {
                    var activeRow = dg.Rows[e.RowIndex];
                    if (activeRow.Tag != null)
                    {
                        var vdo = (ParameterWrapper) activeRow.Tag;
                        var value = new double[1];
                        var converter = vdo.Converter;
                        if (activeRow.Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                        {
                            var unitType = (Enum) activeRow.Cells[e.ColumnIndex].Value;
                            var newValue = StateContainer.Instance.GetStateDefinedValueObject(vdo.Key.ToUpper())
                                .GetValue(unitType)[0];

                            UiStateRoutines.FillUserParamsFromString(newValue.ToString(), vdo.Key, converter, unitType);

                            vdo.Unit = unitType;
                            ((Dictionary<string, Enum>) StateContainer.Instance.Parameters["VarUnitDict"])[vdo.Key] =
                                unitType;
                        }
                        else if (e.ColumnIndex == dataColumnindex)
                        {
                            if (activeRow.Cells[e.ColumnIndex].Value != null)
                            {
                                var sValue = activeRow.Cells[e.ColumnIndex].Value.ToString();

                                UiStateRoutines.FillUserParamsFromString(sValue, vdo.Key, converter, vdo.Unit);
                            }
                        }

                        activeRow.Cells[dataColumnindex].Value = StateContainer.Instance
                            .GetStateDefinedValueObject(vdo.Key.ToUpper()).GetValue(vdo.Unit)[0];
                    }
                }
        }


        private static void AddLinkedValueRowToDataGrid(DataGridView dgObj, GridColumnTemplate gct,
            ParameterWrapper vdo)
        {
            var newRow = new DataGridViewRow();
            var varUnitDict =
                (Dictionary<string, Enum>) StateContainer.Instance.Parameters["VarUnitDict"];
            varUnitDict.TryGetValue(vdo.Key, out var unitToUse);
            unitToUse = unitToUse ?? vdo.Unit;
            string unitDisplayName = null;

            if (unitToUse is UnitlessUnit)
            {
                var uu = (UnitlessUnit) unitToUse;
                if (uu == UnitlessUnit.Unitless) unitDisplayName = "...";
            }

            var valueObj = StateContainer.Instance.GetStateDefinedValueObject(vdo.Key);

            var values = valueObj.GetValue(unitToUse);
            if (gct == null && values.Length == 1)
            {
                if (unitToUse is UnitlessUnit || unitDisplayName != null)
                {
                    newRow.CreateCells(dgObj, vdo.VariableDisplayName, ParseUtility.DoubleToString(values[0]),
                        unitDisplayName);
                    newRow.Cells[0].ReadOnly = true;
                    newRow.Cells[0].Tag = vdo.Key;
                    newRow.Cells[2].ReadOnly = true;
                }
                else
                {
                    var unitType = unitToUse.GetType();
                    var possibleValues = unitType.GetEnumValues();
                    var cbCell = new DataGridViewComboBoxCell {DataSource = possibleValues, ValueType = unitType};

                    newRow.Cells.Add(new DataGridViewTextBoxCell());
                    newRow.Cells.Add(new DataGridViewTextBoxCell());
                    newRow.Cells.Add(cbCell);
                    newRow.Cells[0].Value = vdo.VariableDisplayName;
                    newRow.Cells[0].Tag = vdo.Key;
                    newRow.Cells[1].Value = ParseUtility.DoubleToString(values[0]);
                    newRow.Cells[2].Value = unitToUse;
                    newRow.Cells[0].ReadOnly = true;
                }
            }
            else
            {
                if (gct == null && values.Length > 1)
                    throw new Exception("GridColumnTemplate is required when multiple values are specified.");

                var cellValues = new string[gct.Headers.Length];
                var indexIncrementor = 0;

                if (gct.FirstColumnValues != null)
                {
                    indexIncrementor += 1;

                    cellValues = new string[cellValues.Length + 1];
                    var optionalFirstColumnValueRowIndex = dgObj.Rows.Count - 1;
                    cellValues[0] = gct.FirstColumnValues[optionalFirstColumnValueRowIndex];
                }

                for (var cellIndex = 0; cellIndex < gct.Headers.Length; cellIndex++)
                    if (values.Length > cellIndex)
                    {
                        var svalue = values[cellIndex].ToString("F2", new CultureInfo("en-US"));
                        cellValues[cellIndex + indexIncrementor] = svalue;
                    }


                newRow.CreateCells(dgObj, cellValues);
            }

            vdo.OriginalValues = values;

            vdo.Unit = unitToUse;
            newRow.Tag = vdo;

            dgObj.Rows.Add(newRow);
        }

        public static void InitGrid(DataGridView grid, string[] columnNames, string[] columnDisplayNames,
            bool canSort = true)
        {
            grid.Columns.Clear();
            grid.AllowUserToAddRows = false;
            if (columnNames.Length == columnDisplayNames.Length)
                for (var colIndex = 0; colIndex < columnNames.Length; colIndex++)
                {
                    grid.Columns.Add(columnNames[colIndex], columnDisplayNames[colIndex]);
                    if (!canSort) grid.Columns[colIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            else
                throw new ArgumentException("Column name and column display name arrays must be the same length.");
        }
    }
}