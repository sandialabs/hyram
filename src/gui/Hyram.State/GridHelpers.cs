/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    enum EColumn
    {
        ColLabel,
        ColValue,
        ColUnits,
//        ColCertainty,
//        ColParamA,
//        ColParamB
    }

    public class GridHelpers
    {

        public static void InitGrid(DataGridView grid, string[] columnNames, string[] columnDisplayNames, bool canSort = true, bool clearCols = true,
                                    bool allowUncertain = false)
        {
            if (clearCols)
            {
                grid.Columns.Clear();
            }
            grid.AllowUserToAddRows = false;
            if (columnNames.Length == columnDisplayNames.Length)
            {
                for (var colIndex = 0; colIndex < columnNames.Length; colIndex++)
                {
                    grid.Columns.Add(columnNames[colIndex], columnDisplayNames[colIndex]);
                    if (!canSort) grid.Columns[colIndex].SortMode = DataGridViewColumnSortMode.NotSortable;
                }
            }
            else
            {
                throw new ArgumentException("Column name and column display name arrays must be the same length.");
            }

            // prep uncertainty column and have button text be identical
            if (allowUncertain)
            {
                DataGridViewButtonColumn uncertaintyCol = new DataGridViewButtonColumn();
                uncertaintyCol.HeaderText = "";
                uncertaintyCol.Name = "uncertainty";
                uncertaintyCol.Text = "...";
                uncertaintyCol.UseColumnTextForButtonValue = true;
                grid.Columns.Add(uncertaintyCol);
            }
        }

        // Populates DataGridView with parameter inputs as rows.
        public static void InitParameterGrid(DataGridView grid, List<ParameterInput> parameterInputs, bool canSort = true)
        {
            InitGrid(grid, new[] { "colVariable", "colValue", "colUnit" }, new[] { "Parameter", "Value", "Unit" }, canSort);

            foreach (var input in parameterInputs)
            {
                AddParameterInputToGrid(grid, input);
            }

            foreach (DataGridViewColumn thisColumn in grid.Columns)
            {
                thisColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            grid.HandleCreated += RespondToEventAndResizeColumns;

            // reduce # of clicks required to modify cells and open dropdowns
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Bold);
            grid.Columns[0].Width = 180;
        }

        public static void InitUncertainParameterGrid(DataGridView grid, List<ParameterInput> parameterInputs, bool canSort = true)
        {
            InitGrid(grid, 
                new[] {"colVariable", "colValue", "colUnit" }, 
                new[] {"Parameter", "Value", "Unit"}, canSort, allowUncertain:true);

            foreach (var input in parameterInputs)
            {
                AddParameterInputToGrid(grid, input, allowUncertain:true);
            }

            foreach (DataGridViewColumn thisColumn in grid.Columns)
            {
                thisColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            grid.HandleCreated += RespondToEventAndResizeColumns;

            // reduce # of clicks required to modify cells and open dropdowns
            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Bold);
            grid.Columns[3].Width = 40;
        }


        // Adds new row, including unit selector, representing parameter input to data grid view
        public static void AddParameterInputToGrid(DataGridView view, ParameterInput input, int index = -1, bool allowUncertain = false)
        {
            var newRow = new DataGridViewRow();
            var param = input.Parameter;
            var selectedUnit = param.DisplayUnit;
            string unitLabel = null;

            if (selectedUnit is UnitlessUnit)
            {
                unitLabel = "...";
            }

            var value = param.GetValue(selectedUnit);
            DataGridViewCell valueCell;

            if (selectedUnit is UnitlessUnit || unitLabel != null)
            {
                newRow.CreateCells(view, input.Label, value.ToString(), unitLabel);
                newRow.Cells[0].ReadOnly = true;
                newRow.Cells[2].ReadOnly = true;
                valueCell = newRow.Cells[1];
            }
            else
            {
                // label cell
                newRow.Cells.Add(new DataGridViewTextBoxCell());
                newRow.Cells[0].Value = input.Label;
                newRow.Cells[0].ReadOnly = true;

                // value cell
                valueCell = new DataGridViewTextBoxCell();
                valueCell.Value = value.ToString();
                newRow.Cells.Add(valueCell);

                // unit selector
                var unitType = selectedUnit.GetType();
                var possibleValues = unitType.GetEnumValues();
                var unitCell = new DataGridViewComboBoxCell {DataSource = possibleValues, ValueType = unitType};
                newRow.Cells.Add(unitCell);
                newRow.Cells[2].Value = selectedUnit;
            }

            // overwrite value if nullable and is null
            if (param.MaybeNull && param.IsNull)
            {
                newRow.Cells[1].Value = "";
            }

            if (allowUncertain && param.CanBeUncertain)
            {
                var btn = new DataGridViewButtonCell();
                btn.UseColumnTextForButtonValue = true;
                newRow.Cells.Add(btn);
            }

            if (param.IsUncertain())
            {
                valueCell.ReadOnly = true;
                valueCell.Style.BackColor = Color.LightGray;
            }

            newRow.Tag = input;
            if (index == -1)
            {
                view.Rows.Add(newRow);
            }
            else
            {
                view.Rows.Insert(index, newRow);
            }
        }

        // Updates stored parameter value or display units based on event (user input).
        public static void ChangeParameterValue(DataGridView view, DataGridViewCellEventArgs e)
        {
            if (view.Rows.Count == 0 || e.RowIndex == -1)
            {
                // form still initializing
                return;
            }

            var activeRow = view.Rows[e.RowIndex];
            if (activeRow.Tag == null || e.ColumnIndex == (int)EColumn.ColLabel)
            {
                return;
            }

            var input = (ParameterInput)activeRow.Tag;
            var cell = activeRow.Cells[e.ColumnIndex];
            var param = input.Parameter;
            var val = cell.Value;
            double result;

            switch (e.ColumnIndex)
            {
                case (int)EColumn.ColValue:
                    if (val == null && param.MaybeNull)
                    {
                        param.SetValueToNull();
                        Debug.WriteLine("Setting " + param.Label + " to null");
                    }
                    if (val != null && double.TryParse(val.ToString(), out result))
                    {
                        param.SetValue(input.Parameter.DisplayUnit, result);
                    }
                    break;

                case (int)EColumn.ColUnits:
                    param.DisplayUnit = (Enum)val;
                    activeRow.Cells[(int)EColumn.ColValue].Value = param.GetValue(param.DisplayUnit);
                    break;

                default:
                    break;
            }

            if (e.ColumnIndex == (int)EColumn.ColUnits)
//            if (activeRow.Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
            {
            }

            else if (e.ColumnIndex == (int)EColumn.ColValue)
            {
            }

//            activeRow.Cells[(int)EColumn.ColValue].Value = input.Parameter.GetValue(input.Parameter.DisplayUnit);
//            cell.Value = input.Parameter.GetValue(input.Parameter.DisplayUnit);
        }

        private static void RespondToEventAndResizeColumns(object sender, EventArgs e)
        {
            ((DataGridView) sender).AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        /// <summary>
        /// Checks if inputted value in second column is acceptable parameter value (double or null when allowed).
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void CellValidating_CheckDoubleOrNullable(DataGridView grid, object sender,
                                                                DataGridViewCellValidatingEventArgs e)
        {
            var activeRow = grid.Rows[e.RowIndex];
            if (activeRow.Tag == null || e.ColumnIndex != 1)
            {
                return;
            }

            var input = (ParameterInput)activeRow.Tag;
            var strVal = e.FormattedValue.ToString();
            if (!double.TryParse(strVal, out var newVal))
            {
                // not a double; if param is nullable, check for null
                if (input.Parameter.MaybeNull)
                {
                    if (!string.IsNullOrWhiteSpace(strVal))
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

    }
}