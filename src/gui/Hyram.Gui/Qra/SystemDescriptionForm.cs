/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
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

    public partial class SystemDescriptionForm : AnalysisForm
    {
        private StateContainer _state = State.Data;

        public SystemDescriptionForm(MainForm mainForm)
        {
            _ignoreChangeEvents = true;

            MainForm = mainForm;
            InitializeComponent();
            RefreshForm();

            _state.FuelTypeChangedEvent += delegate{RefreshForm();};

            VehiclesGridView.CellEndEdit += VehicleGridView_CellEndEdit;
            ComponentsGridView.CellValueChanged += GridValueChanged;
            PipingGridView.CellValueChanged += GridValueChanged;
            OverridesGridView.CellValueChanged += GridValueChanged;
            FacilityGridView.CellValueChanged += GridValueChanged;

            _ignoreChangeEvents = false;
        }

        // Refreshes state-related data
        public sealed override void RefreshForm()
        {
            var ignoreEvents = _ignoreChangeEvents;
            _ignoreChangeEvents = true;

            _state = State.Data;

            fuelPhaseSelector.DataSource = _state.Phases;
            fuelPhaseSelector.SelectedItem = _state.Phase;

            ComponentsGridView.Rows.Clear();
            var componentInputs = ParameterInput.GetParameterInputList(new [] {
                                                        _state.NumCompressors,
                                                        _state.NumVessels,
                                                        _state.NumValves,
                                                        _state.NumInstruments,
                                                        _state.NumJoints,
                                                        _state.NumHoses,
                                                        _state.PipeLength,
                                                        _state.NumFilters,
                                                        _state.NumFlanges,
                                                        _state.NumExchangers,
                                                        _state.NumVaporizers,
                                                        _state.NumArms,
                                                        _state.NumExtraComponents1,
                                                        _state.NumExtraComponents2 });
            GridHelpers.InitParameterGrid(ComponentsGridView, componentInputs, false);
            ComponentsGridView.Columns[0].Width = 220;
            ComponentsGridView.Columns[1].Width = 100;
            ComponentsGridView.Columns[2].Width = 100;

            // Vehicle grid
            VehiclesGridView.Rows.Clear();
            var vehicleInputs = ParameterInput.GetParameterInputList(new[] {
                                        _state.VehicleCount,
                                        _state.VehicleFuelings,
                                        _state.VehicleDays,
                                        _state.VehicleAnnualDemand });
            GridHelpers.InitParameterGrid(VehiclesGridView, vehicleInputs, false);
            VehiclesGridView.Columns[0].Width = 400;
            VehiclesGridView.Columns[1].Width = 200;
            VehiclesGridView.Columns[2].Width = 200;
            VehiclesGridView.Rows[3].Cells[1].ReadOnly = true;
            VehiclesGridView.Rows[3].Cells[1].Style.BackColor = Color.LightGray;

            // Set up facility grid
            FacilityGridView.Rows.Clear();
            var facilityInputs = ParameterInput.GetParameterInputList(new[] { _state.FacilityLength, _state.FacilityWidth, });
            GridHelpers.InitParameterGrid(FacilityGridView, facilityInputs, false);

            // Set up occupants grid
            OccupantGridView.Rows.Clear();

            foreach (var dist in _state.OccupantInfo)
            {
                var newRow = new DataGridViewRow();
                newRow.CreateCells(OccupantGridView, dist.NumTargets, dist.Desc,
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
                OccupantGridView.Rows.Add(newRow);
            }
            OccupantGridView.Columns[(int) OccupantCols.Description].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            var distXColumn = (DataGridViewComboBoxColumn) OccupantGridView.Columns[(int) OccupantCols.XDistType];
            distXColumn.DataSource = typeof(WorkerDist).GetEnumValues();
            distXColumn.ValueType = typeof(WorkerDist);
            var distYColumn = (DataGridViewComboBoxColumn) OccupantGridView.Columns[(int) OccupantCols.YDistType];
            distYColumn.DataSource = typeof(WorkerDist).GetEnumValues();
            distYColumn.ValueType = typeof(WorkerDist);
            var distZColumn = (DataGridViewComboBoxColumn) OccupantGridView.Columns[(int) OccupantCols.ZDistType];
            distZColumn.DataSource = typeof(WorkerDist).GetEnumValues();
            distZColumn.ValueType = typeof(WorkerDist);
            var unitClm = (DataGridViewComboBoxColumn) OccupantGridView.Columns[(int) OccupantCols.Unit];
            unitClm.DataSource = typeof(DistanceUnit).GetEnumValues();
            unitClm.ValueType = typeof(DistanceUnit);

            // Override grid
            OverridesGridView.Rows.Clear();
            var overrides = ParameterInput.GetParameterInputList(new[] {
                                _state.Release000d01,
                                _state.Release000d10,
                                _state.Release001d00,
                                _state.Release010d00,
                                _state.Release100d00,
                                _state.FailureOverride,
                                _state.GasDetectionProb});
            GridHelpers.InitParameterGrid(OverridesGridView, overrides, false);

            exclusionInput.Text = _state.ExclusionRadius.GetValue().ToString();
            seedInput.Text = (Math.Truncate(_state.RandomSeed.GetValue())).ToString();

            UpdateParameterCellVisibility();

            // refresh piping table
            PipingGridView.Rows.Clear();
            var pipeInputs = ParameterInput.GetParameterInputList(new[] {
                                    _state.PipeDiameter, _state.PipeThickness, _state.FluidPressure,
                                    _state.AmbientTemperature, _state.AmbientPressure,
                                    _state.OrificeDischargeCoefficient
            });
            if (_state.DisplayTemperature())
            {
                pipeInputs.Insert(2, new ParameterInput(_state.FluidTemperature));
            }
            GridHelpers.InitParameterGrid(PipingGridView, pipeInputs, false);
            PipingGridView.Columns[0].Width = 200;

            CheckFormValid();
            _ignoreChangeEvents = ignoreEvents;
        }


        public override void CheckFormValid()
        {
            Alert = AlertLevel.AlertNull;
            AlertMessage = "";

            if (!_state.ReleasePressureIsValid())
            {
                Alert = AlertLevel.AlertError;
                AlertMessage = MessageContainer.ReleasePressureInvalid();
            }

            formWarning.Visible = Alert != AlertLevel.AlertNull;
            formWarning.Text = AlertMessage;
            formWarning.BackColor = _state.AlertBackColors[(int)Alert];
            formWarning.ForeColor = _state.AlertTextColors[(int)Alert];

            MainForm.NotifyOfChildPublicStateChange();
        }

        private void GridValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_ignoreChangeEvents)
            {
                GridHelpers.ChangeParameterValue((DataGridView) sender, e, 1, 2);
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
                VehiclesGridView.Rows[3].Cells[1].Value = demands;
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
            for (var rowIndex = 0; rowIndex < OccupantGridView.RowCount - 1; rowIndex++)
            {
                var thisRow = OccupantGridView.Rows[rowIndex];
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
                var changedRow = OccupantGridView.Rows[e.RowIndex];
                UpdateOccupantSetandRowData(changedRow, e.ColumnIndex);
            }
        }

        private void OccupantGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!_ignoreChangeEvents)
            {
                _ignoreChangeEvents = true;
                var row = OccupantGridView.Rows[e.RowIndex - 1];
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
                    e.ColumnIndex + "(" + OccupantGridView.Columns[e.ColumnIndex].Name + "). Error details: " +
                    ex);
                MessageBox.Show("Program will be terminated.");
                Environment.Exit(0);
            }
        }

        private void tbRandomSeed_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(seedInput.Text, out int val))
            {
                _state.RandomSeed.SetValue(val);
            }
            else
            {
                seedInput.Text = _state.RandomSeed.GetValue().ToString();
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

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Phase = (ModelPair)fuelPhaseSelector.SelectedItem;
            RefreshForm();
        }

        private void tcSystemDescription_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshForm();
        }
    }
}