/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
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