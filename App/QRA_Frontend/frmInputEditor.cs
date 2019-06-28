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
using System.Drawing;
using System.Windows.Forms;
using DefaultParsing;
using JrConversions;
using QRA_Frontend.ContentPanels;
using QRAState;

namespace QRA_Frontend
{
    public partial class FrmInputEditor : Form
    {
        //This is some workarounds to avoid the annoying behavior of winforms controls scrolling to the top 
        //when they get resized.
        private Point _scrollPosition = new Point(0, 0);

        public FrmInputEditor()
        {
            InitializeComponent();
            AddGrids();
            DoubleBuffered = true;
        }

        private void AddGrids()
        {
            AddSystemDescriptionGrids();
        }

        private void AddSystemDescriptionGrids()
        {
            AddComponentsGrid();
            AddPipingGrid();
            AddVehiclesGrid();
            AddFacilitiesParametersGrid();
            AddOccupantsGrid();
        }

        //Much of this code is currently just copied from cpSystemDecription. 
        //A more elegant and extensible system might be nice.

        private void AddComponentsGrid()
        {
            var newDataGrid = new DataGridView();

            StaticGridHelperRoutines.InitInteractiveGrid(newDataGrid, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("Components.NrCompressors", "# Compressors", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("Components.NrCylinders", "# Cylinders", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("Components.NrValves", "# Valves", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("Components.NrInstruments", "# Instruments", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("Components.NrJoints", "# Joints", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("Components.NrHoses", "# Hoses", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("Components.PipeLength", "Pipes (length)", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("Components.NrFilters", "# Filters", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("Components.NrFlanges", "# Flanges", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter)
                }
            ), false);
            AddGridToLayout(newDataGrid, "Components");
        }

        private void AddPipingGrid()
        {
            var newDataGrid = new DataGridView();

            StaticGridHelperRoutines.InitInteractiveGrid(newDataGrid, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("SysParam.PipeOD", "Pipe Outer Diameter", DistanceUnit.Inch,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("SysParam.PipeWallThick", "Pipe Wall Thickness", DistanceUnit.Inch,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("SysParam.InternalTempC", "Hydrogen Temperature", TempUnit.Celsius,
                        StockConverters.TemperatureConverter),
                    new ParameterWrapper("SysParam.InternalPresMPA", "Hydrogen Pressure", PressureUnit.MPa,
                        StockConverters.PressureConverter),
                    new ParameterWrapper("SysParam.ExternalTempC", "Ambient Temperature", TempUnit.Celsius,
                        StockConverters.TemperatureConverter),
                    new ParameterWrapper("SysParam.ExternalPresMPA", "Ambient Pressure", PressureUnit.MPa,
                        StockConverters.PressureConverter)
                }
            ), false);

            newDataGrid.Columns[0].Width = 200;
            AddGridToLayout(newDataGrid, "System Parameters");
        }

        private void AddVehiclesGrid()
        {
            var newDataGrid = new DataGridView();

            StaticGridHelperRoutines.InitInteractiveGrid(newDataGrid, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("nVehicles", "# Vehicles", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("nFuelingsPerVehicleDay", "nFuelingsPerVehicleDay", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("nVehicleOperatingDays", "nVehicleOperatingDays", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("nDemands", "Annual demands (calculated)", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter)
                }
            ), false);

            newDataGrid.Columns[0].Width = 200;
            AddGridToLayout(newDataGrid, "System Demand");
        }

        private void AddFacilitiesParametersGrid()
        {
            var newDataGrid = new DataGridView();

            StaticGridHelperRoutines.InitInteractiveGrid(newDataGrid, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("Facility.Length", "Length", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("Facility.Width", "Width", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("Facility.Height", "Height", DistanceUnit.Meter,
                        StockConverters.DistanceConverter)
                }
            ), false);

            AddGridToLayout(newDataGrid, "Facility");
        }

        private void AddOccupantsGrid()
        {
            var newDataGrid = new DataGridView();
            var clmNumTargets = new DataGridViewTextBoxColumn();
            var clmDescription = new DataGridViewTextBoxColumn();
            var clmDistributionType = new DataGridViewComboBoxColumn();
            var clmParamA = new DataGridViewTextBoxColumn();
            var clmParamB = new DataGridViewTextBoxColumn();
            var clmParamUnit = new DataGridViewComboBoxColumn();
            var clmExposedHours = new DataGridViewTextBoxColumn();

            clmNumTargets.HeaderText = "Number of Targets";

            clmDescription.HeaderText = "Description";
            clmDescription.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            clmDescription.Width = 180;

            clmDistributionType.HeaderText = "Location Distribution Type";
            clmDistributionType.DataSource = typeof(EWorkerDistribution).GetEnumValues();
            clmDistributionType.ValueType = typeof(EWorkerDistribution);

            clmParamA.HeaderText = "Location Distribution Parameter A";

            clmParamB.HeaderText = "Location Distribution Parameter B";

            clmParamUnit.HeaderText = "Location Parameter Unit";
            clmParamUnit.DataSource = typeof(DistanceUnit).GetEnumValues();
            clmParamUnit.ValueType = typeof(DistanceUnit);

            clmExposedHours.HeaderText = "Exposed Hours Per Year";

            newDataGrid.Columns.AddRange(clmNumTargets, clmDescription, clmDistributionType, clmParamA, clmParamB,
                clmParamUnit, clmExposedHours);

            var distributions =
                (OccupantDistributionInfoCollection) QraStateContainer.Instance.Parameters["OccupantDistributions"];
            foreach (var dist in distributions)
            {
                var newRow = new DataGridViewRow();

                newRow.CreateCells(newDataGrid, dist.NumTargets, dist.Desc, dist.XLocDistribution, dist.XLocParamA,
                    dist.XLocParamB, dist.ParamUnitType, dist.ExposureHours);
                newRow.Tag = dist;

                if (dist.XLocDistribution.Equals(EWorkerDistribution.Deterministic))
                {
                    newRow.Cells[4].Value = null;
                    newRow.Cells[4].ReadOnly = true;
                    newRow.Cells[4].Style.BackColor = Color.LightGray;
                    newRow.Cells[4].Style.ForeColor = Color.DarkGray;
                }

                newDataGrid.Rows.Add(newRow);
            }

            AddGridToLayout(newDataGrid, "Targets");
            newDataGrid.AllowUserToAddRows = true;
            newDataGrid.AllowUserToDeleteRows = true;
            newDataGrid.CellValueChanged -= GridValueChanged;
            newDataGrid.CellValueChanged += OccupantGrid_CellValueChanged;
            newDataGrid.RowsAdded += OccupantGrid_RowsAdded;
            newDataGrid.UserDeletingRow += OccupantGrid_UserDeletingRow;
            newDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            newDataGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        }

        private void GridValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
        }

        private void AddGridToLayout(DataGridView newDataGrid, string title)
        {
            var newDataGridBox = new GroupBox();

            newDataGrid.CellValueChanged += GridValueChanged;
            newDataGrid.Dock = DockStyle.Fill;
            newDataGrid.AllowUserToAddRows = false;
            newDataGrid.AllowUserToDeleteRows = false;
            newDataGrid.AllowUserToOrderColumns = false;
            newDataGrid.AutoSize = true;

            newDataGridBox.Text = title;
            newDataGridBox.Anchor = AnchorStyles.Right | AnchorStyles.Left;
            newDataGridBox.AutoSize = true;

            newDataGridBox.Controls.Add(newDataGrid);
            tlpInputGrids.Controls.Add(newDataGridBox);
        }

        private void HarvestOccupantDatagridRow(DataGridViewRow changedRow)
        {
            var dist = changedRow.Tag as OccupantDistributionInfo;
            if (dist == null)
            {
                dist = new OccupantDistributionInfo();
                changedRow.Tag = dist;
            }

            var intOutput = 0;
            double doubleOutput = 0;

            if (int.TryParse((changedRow.Cells[0].Value ?? string.Empty).ToString(), out intOutput))
                dist.NumTargets = intOutput;

            changedRow.Cells[(int) OccupantGridColumnIdx.NumberOfTargets].Value = dist.NumTargets;

            if (changedRow.Cells[(int) OccupantGridColumnIdx.Description].Value != null)
                dist.Desc = changedRow.Cells[(int) OccupantGridColumnIdx.Description].Value.ToString();

            changedRow.Cells[(int) OccupantGridColumnIdx.Description].Value = dist.Desc;

            if (changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistType].Value == null)
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistType].Value = EWorkerDistribution.Uniform;

            dist.XLocDistribution =
                (EWorkerDistribution) changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistType].Value;

            if (Parsing.TryParseDouble(
                (changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmA].Value ?? string.Empty).ToString(),
                out doubleOutput))
                dist.XLocParamA = doubleOutput;

            if (!changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistType].Value
                .Equals(EWorkerDistribution.Deterministic))
            {
                if (Parsing.TryParseDouble(
                    (changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].Value ?? string.Empty).ToString(),
                    out doubleOutput))
                    dist.XLocParamB = doubleOutput;

                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].Style.BackColor =
                    changedRow.DataGridView.DefaultCellStyle.BackColor;
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].Style.ForeColor =
                    changedRow.DataGridView.DefaultCellStyle.ForeColor;
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].ReadOnly = false;
            }
            else
            {
                //Don't use the second parameter value for deterministic
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].Value = null;
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].ReadOnly = true;
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].Style.BackColor = Color.LightGray;
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].Style.ForeColor = Color.DarkGray;
            }

            if (changedRow.Cells[(int) OccupantGridColumnIdx.Unit].Value == null)
                changedRow.Cells[(int) OccupantGridColumnIdx.Unit].Value = DistanceUnit.Meter;

            dist.ParamUnitType = (DistanceUnit) changedRow.Cells[(int) OccupantGridColumnIdx.Unit].Value;
            changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmA].Value = dist.XLocParamA;
            if (!changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistType].Value
                .Equals(EWorkerDistribution.Deterministic))
                changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB].Value = dist.XLocParamB;

            if (int.TryParse(
                (changedRow.Cells[(int) OccupantGridColumnIdx.ExposureHours].Value ?? string.Empty).ToString(),
                out intOutput))
                dist.ExposureHours = intOutput;

            changedRow.Cells[(int) OccupantGridColumnIdx.ExposureHours].Value = dist.ExposureHours;
        }

        private void OccupantGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var changedRow = ((DataGridView) sender).Rows[e.RowIndex];
            HarvestOccupantDatagridRow(changedRow);
        }

        private void OccupantGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            var changedRow = ((DataGridView) sender).Rows[e.RowIndex - 1];
            HarvestOccupantDatagridRow(changedRow);

            var distributions =
                QraStateContainer.Instance.OccupantDistributionInfoCollection;
            distributions.Add((OccupantDistributionInfo) changedRow.Tag);
        }

        private void OccupantGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var thisDist = (OccupantDistributionInfo) e.Row.Tag;
            var distributions =
                (OccupantDistributionInfoCollection) QraStateContainer.Instance.Parameters["OccupantDistributions"];
            if (distributions.Count > 1)
            {
                distributions.Remove(thisDist);
            }
            else
            {
                e.Cancel = true;
                MessageBox.Show(
                    "The occupant distributions grid must contain at least one row. Please add another row before deleting this one.");
            }
        }

        //Un-minimize ourselves if we get activated
        private void frmInputEditor_Activated(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }

        private void tlpInputGrids_Scroll(object sender, ScrollEventArgs e)
        {
            _scrollPosition = tlpInputGrids.AutoScrollPosition;
        }

        private void tlpInputGrids_Resize(object sender, EventArgs e)
        {
            tlpInputGrids.AutoScrollPosition = new Point(-_scrollPosition.X, -_scrollPosition.Y);
        }

        //To avoid flicker from constant scrolling while resizing, suspend the layout while a resize is in process.
        private void frmInputEditor_ResizeBegin(object sender, EventArgs e)
        {
            SuspendLayout();
        }

        private void frmInputEditor_ResizeEnd(object sender, EventArgs e)
        {
            ResumeLayout();
        }
    }
}