/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    public enum OccupantCols
    {
        NumberOfTargets,
        Description,
        Unit,
        XDistType,
        XDistParmA,
        XDistParmB,
        YDistType,
        YDistParmA,
        YDistParmB,
        ZDistType,
        ZDistParmA,
        ZDistParmB,
        ExposureHours
    }

    public partial class SystemDescriptionForm : UserControl
    {
        private StateContainer _state = State.Data;
        protected bool _ignoreChangeEvents;
        public string AlertMessage { get; set; } = "";
        public AlertLevel Alert { get; set; } = AlertLevel.AlertNull;

        public SystemDescriptionForm()
        {
            _ignoreChangeEvents = true;

            InitializeComponent();
            LoadForm();

            VehiclesGrid.CellEndEdit += VehicleGridView_CellEndEdit;
            ComponentGrid.CellValueChanged += GridValueChanged;
            PipingGrid.CellValueChanged += GridValueChanged;
            OverridesGrid.CellValueChanged += GridValueChanged;

            _ignoreChangeEvents = false;
        }

        // Refreshes state-related data
        public void LoadForm()
        {
            var ignoreEvents = _ignoreChangeEvents;
            _ignoreChangeEvents = true;

            _state = State.Data;

            ComponentGrid.Rows.Clear();
            var componentInputs = ParameterInput.GetParameterInputList(new [] {
                                                        _state.NumCompressors,
                                                        _state.NumVessels,
                                                        _state.NumValves,
                                                        _state.NumInstruments,
                                                        _state.NumJoints,
                                                        _state.NumHoses,
                                                        _state.NumFilters,
                                                        _state.NumFlanges,
                                                        _state.NumExchangers,
                                                        _state.NumVaporizers,
                                                        _state.NumArms,
                                                        _state.NumExtraComponents1,
                                                        _state.NumExtraComponents2 });
            GridHelpers.InitParameterGrid(ComponentGrid, componentInputs, false);
            ComponentGrid.DefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Regular);
            ComponentGrid.Columns[1].Width = 100;
            ComponentGrid.Columns[2].Width = 100;

            // Vehicle grid
            VehiclesGrid.Rows.Clear();
            var vehicleInputs = ParameterInput.GetParameterInputList(new[] {
                                        _state.VehicleCount,
                                        _state.VehicleFuelings,
                                        _state.VehicleDays,
                                        _state.VehicleAnnualDemand });
            GridHelpers.InitParameterGrid(VehiclesGrid, vehicleInputs, false);
            VehiclesGrid.Columns[0].Width = 240;
            VehiclesGrid.Columns[2].Width = 50;
            VehiclesGrid.Rows[3].Cells[1].ReadOnly = true;
            VehiclesGrid.Rows[3].Cells[1].Style.BackColor = Color.LightGray;
            VehiclesGrid.DefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Regular);

            LengthUnitSelector.DataSource = new List<DistanceUnit>
            {
                DistanceUnit.Meter, DistanceUnit.Foot,
                DistanceUnit.Yard, DistanceUnit.Mile
            };
            LengthUnitSelector.SelectedItem = _state.FacilityLength.DisplayUnit;
            LengthInput.Text = _state.FacilityLength.GetValue(_state.FacilityLength.DisplayUnit).ToString();

            WidthUnitSelector.DataSource = new List<DistanceUnit>
            {
                DistanceUnit.Meter, DistanceUnit.Foot,
                DistanceUnit.Yard, DistanceUnit.Mile
            };
            WidthUnitSelector.SelectedItem = _state.FacilityWidth.DisplayUnit;
            WidthInput.Text = _state.FacilityWidth.GetValue(_state.FacilityWidth.DisplayUnit).ToString();

            PipingGrid.Rows.Clear();
            var pipeInputs = ParameterInput.GetParameterInputList(new[] {
                                    _state.PipeLength,
                                    _state.PipeDiameter,
                                    _state.PipeThickness,
                                    _state.RelativeHumidity
            });
            GridHelpers.InitParameterGrid(PipingGrid, pipeInputs, false);
            PipingGrid.DefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Regular);
            PipingGrid.Columns[0].Width = 240;

            var leakSizeOptions = new Dictionary<string, int>
            {
                { "0.01% leak size", 1},
                { "0.10% leak size", 10},
                { "1% leak size", 100},
                { "10% leak size", 1000},
                { "100% leak size", 10000},
            };
            MassFlowLeakSizeSelector.DataSource = new BindingSource(leakSizeOptions, null);
            MassFlowLeakSizeSelector.DisplayMember = "Key";
            MassFlowLeakSizeSelector.ValueMember = "Value";

            MassFlowUnitSelector.DataSource = new List<MassFlowUnit> {MassFlowUnit.KgPerMin, MassFlowUnit.KgPerSecond};
            MassFlowInput.Text = _state.QraMassFlow.GetDisplayValueMaybeNull().ToString();
            MassFlowUnitSelector.SelectedItem = _state.QraMassFlow.DisplayUnit;

            ToggleMassFlowInputs(_state.FuelFlowUnchoked());

            // show seed input if UQ/SA is disabled
            if (!_state.AllowUncertainty)
            {
                SeedInput.Visible = true;
                SeedLabel.Visible = true;
                SeedDescrip.Visible = true;
                SeedInput.Text = (Math.Truncate(_state.RandomSeed.GetValue())).ToString();
            }
            else
            {
                SeedInput.Visible = false;
                SeedLabel.Visible = false;
                SeedDescrip.Visible = false;
            }

            // OCCUPANTS TAB
            OccupantGrid.Rows.Clear();

            foreach (var dist in _state.OccupantInfo)
            {
                var newRow = new DataGridViewRow();
                newRow.CreateCells(OccupantGrid, dist.NumTargets, dist.Desc,
                                    dist.ParamUnitType,
                                    dist.XLocDistribution, dist.XLocParamA, dist.XLocParamB,
                                    dist.YLocDistribution, dist.YLocParamA, dist.YLocParamB,
                                    dist.ZLocDistribution, dist.ZLocParamA, dist.ZLocParamB,
                                    dist.ExposureHours);
                newRow.Tag = dist;

                if ((WorkerDist) dist.XLocDistribution == WorkerDist.Constant)
                {
                    var thisCell = newRow.Cells[(int) OccupantCols.XDistParmB];
                    thisCell.Value = null;
                    thisCell.ReadOnly = true;
                    thisCell.Style.BackColor = Color.LightGray;
                    thisCell.Style.ForeColor = Color.DarkGray;
                }

                if ((WorkerDist) dist.ZLocDistribution == WorkerDist.Constant)
                {
                    var thisCell = newRow.Cells[(int) OccupantCols.ZDistParmB];
                    thisCell.Value = null;
                    thisCell.ReadOnly = true;
                    thisCell.Style.BackColor = Color.LightGray;
                    thisCell.Style.ForeColor = Color.DarkGray;
                }
                OccupantGrid.Rows.Add(newRow);
            }
            OccupantGrid.Columns[(int) OccupantCols.Description].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            var distXColumn = (DataGridViewComboBoxColumn) OccupantGrid.Columns[(int) OccupantCols.XDistType];
            distXColumn.DataSource = typeof(WorkerDist).GetEnumValues();
            distXColumn.ValueType = typeof(WorkerDist);
//            distXColumn.Width = 50;
            var distYColumn = (DataGridViewComboBoxColumn) OccupantGrid.Columns[(int) OccupantCols.YDistType];
            distYColumn.DataSource = typeof(WorkerDist).GetEnumValues();
            distYColumn.ValueType = typeof(WorkerDist);
            var distZColumn = (DataGridViewComboBoxColumn) OccupantGrid.Columns[(int) OccupantCols.ZDistType];
            distZColumn.DataSource = typeof(WorkerDist).GetEnumValues();
            distZColumn.ValueType = typeof(WorkerDist);
            var unitClm = (DataGridViewComboBoxColumn) OccupantGrid.Columns[(int) OccupantCols.Unit];
            unitClm.DataSource = typeof(DistanceUnit).GetEnumValues();
            unitClm.ValueType = typeof(DistanceUnit);
            OccupantGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Sans Serif", 8.0F, FontStyle.Bold);

            // Override grid
            OverridesGrid.Rows.Clear();
            var overrides = ParameterInput.GetParameterInputList(new[] {
                                _state.Release000d01,
                                _state.Release000d10,
                                _state.Release001d00,
                                _state.Release010d00,
                                _state.Release100d00,
                                _state.FailureOverride,
                                _state.GasDetectionProb});
            GridHelpers.InitParameterGrid(OverridesGrid, overrides, false);

            exclusionInput.Text = _state.ExclusionRadius.GetValue().ToString();

            UpdateParameterCellVisibility();

            CheckFormValid();
            _ignoreChangeEvents = ignoreEvents;
        }

        /// <summary>
        /// Enables or disables leak-based mass flow inputs for unchoked flow.
        /// </summary>
        /// <param name="enabled"></param>
        private void ToggleMassFlowInputs(bool enabled = true)
        {
            if (enabled)
            {
                MassFlowInput.Enabled = true;
                MassFlowLeakSizeSelector.Enabled = true;
                MassFlowUnitSelector.Enabled = true;
//                valueCell.Style.BackColor = Color.White;
//                valueCell.Value = ((ParameterInput)row.Tag).Parameter.GetDisplayValue();
            }
            else
            {
                MassFlowInput.Enabled = false;
                MassFlowLeakSizeSelector.Enabled = false;
                MassFlowUnitSelector.Enabled = false;
//                valueCell.Style.BackColor = Color.LightGray;
//                valueCell.Value = "-";
            }
        }


        public void CheckFormValid()
        {
            Alert = AlertLevel.AlertNull;
            AlertMessage = "";
//
            formWarning.Visible = Alert != AlertLevel.AlertNull;
//            formWarning.Text = AlertMessage;
//            formWarning.BackColor = _state.AlertBackColors[(int)Alert];
//            formWarning.ForeColor = _state.AlertTextColors[(int)Alert];
        }

        private void GridValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_ignoreChangeEvents)
            {
                GridHelpers.ChangeParameterValue((DataGridView) sender, e);
            }
            CheckFormValid();
        }

        private void VehicleGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);

            // Update annual demands
            var row = e.RowIndex;
            Debug.WriteLine("Changed row:" + row + ", col: " + e.ColumnIndex);
            if (row != 3)
            {
                var numVehicles = _state.VehicleCount.GetValue();
                var numDays = _state.VehicleDays.GetValue();
                var numFuelings = _state.VehicleFuelings.GetValue();
                var demands = numVehicles * numDays * numFuelings;

                _state.VehicleAnnualDemand.SetValue(demands);
                VehiclesGrid.Rows[3].Cells[1].Value = demands;
            }
        }

        /// <summary>
        /// Store row data in Occupant object while ensuring values are valid. If invalid, replace with existing.
        /// TODO: clean up data binding here
        /// </summary>
        /// <param name="changedRow"></param>
        private void UpdateOccupantSetandRowData(DataGridViewRow row, int changedColumn = -1)
        {
            var ignoreChange = _ignoreChangeEvents;
            _ignoreChangeEvents = true;

            DataGridViewCell numTargetsCell = row.Cells[(int) OccupantCols.NumberOfTargets];
            DataGridViewCell xDistributionCell = row.Cells[(int) OccupantCols.XDistType];
            DataGridViewCell xParamACell = row.Cells[(int) OccupantCols.XDistParmA];
            DataGridViewCell xParamBCell = row.Cells[(int) OccupantCols.XDistParmB];
            DataGridViewCell yDistributionCell = row.Cells[(int) OccupantCols.YDistType];
            DataGridViewCell yParamACell = row.Cells[(int) OccupantCols.YDistParmA];
            DataGridViewCell yParamBCell = row.Cells[(int) OccupantCols.YDistParmB];
            DataGridViewCell zDistributionCell = row.Cells[(int) OccupantCols.ZDistType];
            DataGridViewCell zParamACell = row.Cells[(int) OccupantCols.ZDistParmA];
            DataGridViewCell zParamBCell = row.Cells[(int) OccupantCols.ZDistParmB];
            DataGridViewCell descripCell = row.Cells[(int) OccupantCols.Description];
            DataGridViewCell distanceUnitCell = row.Cells[(int) OccupantCols.Unit];
            DataGridViewCell hoursCell = row.Cells[(int) OccupantCols.ExposureHours];

            var occupantSet = (OccupantDistributionInfo) row.Tag;
            if (occupantSet == null)
            {
                occupantSet = new OccupantDistributionInfo();
                row.Tag = occupantSet;
                _state.OccupantInfo.Add(occupantSet);
            }

            if (numTargetsCell.Value == null || !int.TryParse(numTargetsCell.Value.ToString(), out int numTargets))
            {
                numTargets = occupantSet.NumTargets;
                numTargetsCell.Value = numTargets;
            }

            if (changedColumn != (int) OccupantCols.Unit)  // skip updating values if user only changed units
            {
                // try to parse inputted value; otherwise use current value
                Enum xDistribution;
                if (xDistributionCell.Value == null)
                {
                    xDistribution = occupantSet.XLocDistribution;
                    xDistributionCell.Value = xDistribution;
                }
                if (xParamACell.Value == null || !double.TryParse(xParamACell.Value.ToString(), out double xParamA))
                {
                    xParamA = occupantSet.XLocParamA;
                    xParamACell.Value = xParamA;
                }

                if (xParamBCell.Value == null || !double.TryParse(xParamBCell.Value.ToString(), out double xParamB))
                {
                    xParamB = occupantSet.XLocParamB;
                    xParamBCell.Value = xParamB;
                }

                if (yParamACell.Value == null || !double.TryParse(yParamACell.Value.ToString(), out double yParamA))
                {
                    yParamA = occupantSet.YLocParamA;
                    yParamACell.Value = yParamA;
                }

                if (yParamBCell.Value == null || !double.TryParse(yParamBCell.Value.ToString(), out double yParamB))
                {
                    yParamB = occupantSet.YLocParamB;
                    yParamBCell.Value = yParamB;
                }

                if (zParamACell.Value == null || !double.TryParse(zParamACell.Value.ToString(), out double zParamA))
                {
                    zParamA = occupantSet.ZLocParamA;
                    zParamACell.Value = zParamA;
                }

                if (zParamBCell.Value == null || !double.TryParse(zParamBCell.Value.ToString(), out double zParamB))
                {
                    zParamB = occupantSet.ZLocParamB;
                    zParamBCell.Value = zParamB;
                }

                if (distanceUnitCell.Value == null)
                    distanceUnitCell.Value = occupantSet.ParamUnitType;

                if (hoursCell.Value == null || !double.TryParse(hoursCell.Value.ToString(), out double exposureHours))
                {
                    exposureHours = occupantSet.ExposureHours;
                    hoursCell.Value = occupantSet.ExposureHours;
                }

                // Update object values
                occupantSet.NumTargets = numTargets;
                if (descripCell.Value != null) occupantSet.Desc = descripCell.Value.ToString();
                occupantSet.XLocDistribution = (WorkerDist) xDistributionCell.Value;
                occupantSet.XLocParamA = xParamA;
                occupantSet.XLocParamB = xParamB;
                occupantSet.YLocDistribution = (WorkerDist) yDistributionCell.Value;
                occupantSet.YLocParamA = yParamA;
                occupantSet.YLocParamB = yParamB;
                occupantSet.ZLocDistribution = (WorkerDist) zDistributionCell.Value;
                occupantSet.ZLocParamA = zParamA;
                occupantSet.ZLocParamB = zParamB;
                occupantSet.ExposureHours = exposureHours;
            }
            else
            {
                // user changed units so update display
                occupantSet.ParamUnitType = (DistanceUnit) distanceUnitCell.Value;
                xParamACell.Value = occupantSet.XLocParamA;
                xParamBCell.Value = occupantSet.XLocParamB;
                yParamACell.Value = occupantSet.YLocParamA;
                yParamBCell.Value = occupantSet.YLocParamB;
                zParamACell.Value = occupantSet.ZLocParamA;
                zParamBCell.Value = occupantSet.ZLocParamB;
            }

            UpdateParameterCellVisibility();

            _ignoreChangeEvents = ignoreChange;
        }

        private void UpdateParameterCellVisibility()
        {
            int[] colStarts =
            {
                (int) OccupantCols.XDistType, (int) OccupantCols.YDistType,
                (int) OccupantCols.ZDistType
            };
            for (var rowIndex = 0; rowIndex < OccupantGrid.RowCount - 1; rowIndex++)
            {
                var thisRow = OccupantGrid.Rows[rowIndex];
                // ensure values match units


                foreach (var colStart in colStarts)
                {
                    var typeCell = thisRow.Cells[colStart];
                    var paramBCell = thisRow.Cells[colStart + 2];
                    // if deterministic, clear param B and make it readonly
                    if (typeCell.Value != null && typeCell.Value.Equals(WorkerDist.Constant))
                    {
                        var oldIgnore = _ignoreChangeEvents;
                        _ignoreChangeEvents = true;
                        paramBCell.Value = 0D;
                        _ignoreChangeEvents = oldIgnore;
                        paramBCell.ReadOnly = true;
                        paramBCell.Style.BackColor = Color.LightGray;
                        paramBCell.Style.ForeColor = Color.DarkGray;
                    }
                    else
                    {
                        paramBCell.Style.BackColor = thisRow.DataGridView.DefaultCellStyle.BackColor;
                        paramBCell.Style.ForeColor = thisRow.DataGridView.DefaultCellStyle.ForeColor;
                        paramBCell.ReadOnly = false;
                    }
                }
            }
        }

        private void OccupantGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_ignoreChangeEvents)
            {
                var changedRow = OccupantGrid.Rows[e.RowIndex];
                UpdateOccupantSetandRowData(changedRow, e.ColumnIndex);
            }
        }

        private void OccupantGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!_ignoreChangeEvents)
            {
                _ignoreChangeEvents = true;
                var row = OccupantGrid.Rows[e.RowIndex - 1];
                var occupantSet = (OccupantDistributionInfo) row.Tag;
                if (occupantSet == null)
                {
                    occupantSet = new OccupantDistributionInfo();
                    row.Tag = occupantSet;
                    _state.OccupantInfo.Add(occupantSet);
                }

                row.Cells[(int) OccupantCols.NumberOfTargets].Value = occupantSet.NumTargets;
                row.Cells[(int) OccupantCols.Description].Value = occupantSet.Desc;
                row.Cells[(int) OccupantCols.Unit].Value = occupantSet.ParamUnitType;
                row.Cells[(int) OccupantCols.XDistType].Value = occupantSet.XLocDistribution;
                row.Cells[(int) OccupantCols.XDistParmA].Value = occupantSet.XLocParamA;
                row.Cells[(int) OccupantCols.XDistParmB].Value = occupantSet.XLocParamB;
                row.Cells[(int) OccupantCols.YDistType].Value = occupantSet.YLocDistribution;
                row.Cells[(int) OccupantCols.YDistParmA].Value = occupantSet.YLocParamA;
                row.Cells[(int) OccupantCols.YDistParmB].Value = occupantSet.YLocParamB;
                row.Cells[(int) OccupantCols.ZDistType].Value = occupantSet.ZLocDistribution;
                row.Cells[(int) OccupantCols.ZDistParmA].Value = occupantSet.ZLocParamA;
                row.Cells[(int) OccupantCols.ZDistParmB].Value = occupantSet.ZLocParamB;
                row.Cells[(int) OccupantCols.ExposureHours].Value = occupantSet.ExposureHours;

                _ignoreChangeEvents = false;
            }
        }

        private void OccupantGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!_ignoreChangeEvents)
            {
                var thisDist = (OccupantDistributionInfo) e.Row.Tag;
                if (_state.OccupantInfo.Count > 1)
                {
                    _state.OccupantInfo.Remove(thisDist);
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show("Occupant distributions grid must contain at least one row. Add another row before deleting this one.");
                }
            }
        }

        private void OccupantGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            try
            {
                throw new Exception("Error detected", e.Exception);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Error during tab initialization. This has caused an unknown " +
                    "condition and you are likely to see more errors. Please contact the HyRAM development team. Column: " +
                    e.ColumnIndex + "(" + OccupantGrid.Columns[e.ColumnIndex].Name + "). Error details: " +
                    ex);
                MessageBox.Show("Program will be terminated.");
                Environment.Exit(0);
            }
        }

        private void tbExclusionRadius_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(exclusionInput.Text, out double val))
            {
                _state.ExclusionRadius.SetValue(val);
            }
            else
            {
                exclusionInput.Text = _state.ExclusionRadius.GetValue().ToString();
            }
        }

        private void LengthInput_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(LengthInput.Text, out double val))
            {
                _state.FacilityLength.SetValueFromDisplay(val);
            }
            else
            {
                LengthInput.Text = _state.FacilityLength.GetDisplayValue().ToString();
            }

        }

        private void WidthInput_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(WidthInput.Text, out double val))
            {
                _state.FacilityWidth.SetValueFromDisplay(val);
            }
            else
            {
                WidthInput.Text = _state.FacilityWidth.GetDisplayValue().ToString();
            }
        }

        private void LengthUnitSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.FacilityLength.DisplayUnit = (DistanceUnit)LengthUnitSelector.SelectedItem;
            LengthInput.Text = _state.FacilityLength.GetDisplayValue().ToString("F3");
        }

        private void WidthUnitSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.FacilityWidth.DisplayUnit = (DistanceUnit)WidthUnitSelector.SelectedItem;
            WidthInput.Text = _state.FacilityWidth.GetDisplayValue().ToString("F3");
        }

        private void ComponentGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(ComponentGrid, sender, e);
        }

        private void PipingGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(PipingGrid, sender, e);

        }

        private void VehiclesGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(VehiclesGrid, sender, e);
        }

        private void OverridesGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(OverridesGrid, sender, e);
        }

        private void MassFlowLeakSizeSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            int leakSize = ((dynamic)MassFlowLeakSizeSelector.SelectedItem).Value;
            _state.QraMassFlowLeakSize = leakSize;
        }

        private void MassFlowUnitSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.QraMassFlow.DisplayUnit = (MassFlowUnit) MassFlowUnitSelector.SelectedItem;
            MassFlowInput.Text = _state.QraMassFlow.GetDisplayValueMaybeNull().ToString();
        }

        private void MassFlowInput_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(MassFlowInput.Text, out double val))
            {
                _state.QraMassFlow.SetValueFromDisplay(val);
            }
            else
            {
                MassFlowInput.Text = _state.QraMassFlow.GetDisplayValueMaybeNull().ToString();
            }

        }

        private void SeedInput_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(SeedInput.Text, out int val))
            {
                _state.RandomSeed.SetValue(val);
            }
            else
            {
                SeedInput.Text = _state.RandomSeed.GetValue().ToString();
            }
        }
    }
}