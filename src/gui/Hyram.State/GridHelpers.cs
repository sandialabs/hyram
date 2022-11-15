/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class GridHelpers
    {

        public static void InitGrid(DataGridView grid, string[] columnNames, string[] columnDisplayNames, bool canSort = true)
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

        // Populates DataGridView with parameter inputs as rows.
        public static void InitParameterGrid(DataGridView grid, List<ParameterInput> parameterInputs, bool canSort = true)
        {
            InitGrid(grid, new[] { "colVariable", "colValue", "colUnit" }, new[] { "Variable", "Value", "Unit" }, canSort);

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
        }

        // Adds new row, including unit selector, representing parameter input to data grid view
        public static void AddParameterInputToGrid(DataGridView view, ParameterInput input, int index = -1)
        {
            var newRow = new DataGridViewRow();
            var unitToUse = input.Parameter.DisplayUnit;
            string unitLabel = null;

            if (unitToUse is UnitlessUnit)
            {
                unitLabel = "...";
            }

            var value = input.Parameter.GetValue(unitToUse);
            if (unitToUse is UnitlessUnit || unitLabel != null)
            {
                newRow.CreateCells(view, input.Label, value.ToString(), unitLabel);
                newRow.Cells[0].ReadOnly = true;
//                    newRow.Cells[0].Tag = vdo.Key;
                newRow.Cells[2].ReadOnly = true;
            }
            else
            {
                // label cell
                newRow.Cells.Add(new DataGridViewTextBoxCell());
                newRow.Cells[0].Value = input.Label;
                newRow.Cells[0].ReadOnly = true;

                // value cell
                newRow.Cells.Add(new DataGridViewTextBoxCell());
                newRow.Cells[1].Value = value.ToString();

                // unit selector
                var unitType = unitToUse.GetType();
                var possibleValues = unitType.GetEnumValues();
                var cbCell = new DataGridViewComboBoxCell {DataSource = possibleValues, ValueType = unitType};
                newRow.Cells.Add(cbCell);
                newRow.Cells[2].Value = unitToUse;
            }

            input.OriginalValue = value;
//            input.Unit = unitToUse;

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
        public static void ChangeParameterValue(DataGridView view, DataGridViewCellEventArgs e, int valueColumn, int unitColumn)
        {
            var activeRow = view.Rows[e.RowIndex];
            // return if invalid row parameter or if modified column is neither value nor unit
            if (activeRow.Tag == null ||
                !(e.ColumnIndex == valueColumn || e.ColumnIndex == unitColumn))
            {
                return;
            }

            var input = (ParameterInput)activeRow.Tag;
            if (activeRow.Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
            {
                input.Parameter.DisplayUnit = (Enum)activeRow.Cells[e.ColumnIndex].Value;
            }
            else if (e.ColumnIndex == valueColumn)
            {
                var newValue = activeRow.Cells[e.ColumnIndex].Value; 
                if (newValue != null && double.TryParse(newValue.ToString(), out double result))
                {
                    input.Parameter.SetValue(input.Parameter.DisplayUnit, result);
                }
            }
            activeRow.Cells[valueColumn].Value = input.Parameter.GetValue(input.Parameter.DisplayUnit);
        }

        private static void RespondToEventAndResizeColumns(object sender, EventArgs e)
        {
            ((DataGridView) sender).AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

    }
}