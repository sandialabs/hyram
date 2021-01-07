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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public enum OccupantColumns
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
        private bool _mCurrentlyIgnoringGridChangeEvents = true;
        private bool _ignoreRandomSeedChangeEvent;

        public SystemDescriptionForm()
        {
            InitializeComponent();
            dgSystemParameters_Vehicles.CellEndEdit += dgSystemParameters_Vehicles_CellEndEdit;

            _mCurrentlyIgnoringGridChangeEvents = true;
            // Set up component grid
            var vdColl = new ParameterWrapperCollection(new[]
            {
                new ParameterWrapper("numCompressors", "# Compressors"),
                new ParameterWrapper("numCylinders", "# Cylinders"),
                new ParameterWrapper("numValves", "# Valves"),
                new ParameterWrapper("numInstruments", "# Instruments"),
                new ParameterWrapper("numJoints", "# Joints"),
                new ParameterWrapper("numHoses", "# Hoses"),
                new ParameterWrapper("pipeLength", "Pipes (length)", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("numFilters", "# Filters"),
                new ParameterWrapper("numFlanges", "# Flanges"),
                new ParameterWrapper("numExtraComponent1", "# Extra Component 1"),
                new ParameterWrapper("numExtraComponent2", "# Extra Component 2")
            });
            StaticGridHelperRoutines.InitInteractiveGrid(dgComponents, vdColl, false);

            fuelPhaseSelector.DataSource = StateContainer.Instance.FluidPhases;
            fuelPhaseSelector.SelectedItem = StateContainer.Instance.GetFluidPhase();
            // Set up system params grid
            PopulateSystemParametersPiping();

            // Vehicle grid
            StaticGridHelperRoutines.InitInteractiveGrid(dgSystemParameters_Vehicles, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("numVehicles", "Number of Vehicles"),
                    new ParameterWrapper("dailyFuelings", "Number of Fuelings Per Vehicle Day"),
                    new ParameterWrapper("vehicleOperatingDays", "Number of Vehicle Operating Days per Year"),
                    new ParameterWrapper("annualDemand", "Annual Demands (calculated)", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter)
                }
            ), false);

            // Make annual demands read-only. Hacky...
            dgSystemParameters_Vehicles.Rows[3].Cells[1].ReadOnly = true;
            dgSystemParameters_Vehicles.Rows[3].Cells[1].Style.BackColor = Color.LightGray;

            dgSystemParameters_Vehicles.Columns[0].Width = 400;
            dgSystemParameters_Vehicles.Columns[1].Width = 200;
            dgSystemParameters_Vehicles.Columns[2].Width = 200;

            // Set up facility grid
            StaticGridHelperRoutines.InitInteractiveGrid(dgFacilityParameters, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("facilityLength", "Length (x-direction)", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("facilityWidth", "Width (z-direction)", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("facilityHeight", "Height (y-direction)", DistanceUnit.Meter,
                        StockConverters.DistanceConverter)
                }
            ), false);

            // Set up occupants input grid
            var distXColumn =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantColumns.XDistType];
            distXColumn.DataSource = typeof(EWorkerDistribution).GetEnumValues();
            distXColumn.ValueType = typeof(EWorkerDistribution);

            var distYColumn =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantColumns.YDistType];
            distYColumn.DataSource = typeof(EWorkerDistribution).GetEnumValues();
            distYColumn.ValueType = typeof(EWorkerDistribution);

            var distZColumn =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantColumns.ZDistType];
            distZColumn.DataSource = typeof(EWorkerDistribution).GetEnumValues();
            distZColumn.ValueType = typeof(EWorkerDistribution);

            dgOccupantInputDetails.Columns[(int) OccupantColumns.Description].DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            var unitClm =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantColumns.Unit];
            unitClm.DataSource = typeof(DistanceUnit).GetEnumValues();
            unitClm.ValueType = typeof(DistanceUnit);
            OccupantDistributionInfoCollection distributions = null;

            try
            {
                distributions = StateContainer.Instance.OccupantDistributionInfoCollection;
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show(
                    "You loaded an occupant distribution collection from a file saved using a previous version of HyRAM. That collection " +
                    "is not compatible with the extended type HyRAM now uses. A new default distribution will be created for you, but your old settings " +
                    "were lost.");
                StateContainer.Instance.InitOccupantDistributions(true);
                distributions = StateContainer.Instance.OccupantDistributionInfoCollection;
                Debug.Write(ex.Message);
            }

            foreach (var dist in distributions)
            {
                var newRow = new DataGridViewRow();
                newRow.CreateCells(dgOccupantInputDetails, dist.NumTargets, dist.Desc,
                    dist.ParamUnitType,
                    dist.XLocDistribution, dist.XLocParamA, dist.XLocParamB,
                    dist.YLocDistribution, dist.YLocParamA, dist.YLocParamB,
                    dist.ZLocDistribution, dist.ZLocParamA, dist.ZLocParamB,
                    dist.ExposureHours);
                newRow.Tag = dist;

                if ((EWorkerDistribution) dist.XLocDistribution == EWorkerDistribution.Constant)
                {
                    var thisCell = newRow.Cells[(int) OccupantColumns.XDistParmB];
                    thisCell.Value = null;
                    thisCell.ReadOnly = true;
                    thisCell.Style.BackColor = Color.LightGray;
                    thisCell.Style.ForeColor = Color.DarkGray;
                }

                if ((EWorkerDistribution) dist.ZLocDistribution == EWorkerDistribution.Constant)
                {
                    var thisCell = newRow.Cells[(int) OccupantColumns.ZDistParmB];
                    thisCell.Value = null;
                    thisCell.ReadOnly = true;
                    thisCell.Style.BackColor = Color.LightGray;
                    thisCell.Style.ForeColor = Color.DarkGray;
                }
                dgOccupantInputDetails.Rows.Add(newRow);
            }

            // Override grid
            var overrideColl = new ParameterWrapperCollection(new[]
            {
                new ParameterWrapper("H2Release.000d01", "0.01% H2 Release"),
                new ParameterWrapper("H2Release.000d10", "0.10% H2 Release"),
                new ParameterWrapper("H2Release.001d00", "1% H2 Release"),
                new ParameterWrapper("H2Release.010d00", "10% H2 Release"),
                new ParameterWrapper("H2Release.100d00", "100% H2 Release"),
                new ParameterWrapper("Failure.ManualOverride", "100% H2 Release (accidents and shutdown failures)"),
                new ParameterWrapper("Pdetectisolate", "Gas detection credit")
            });
            StaticGridHelperRoutines.InitInteractiveGrid(dgManualOverrides, overrideColl, false);


            // Set up random seed textbox
            var saveIgnoreValue = _ignoreRandomSeedChangeEvent;
            _ignoreRandomSeedChangeEvent = true;
            try
            {
                var cv = GetRandomSeedFromDatabase();
                cv.EnsureBaseValueIsTruncatedInt();
                var newValue = Math.Truncate(cv.GetValue(UnitlessUnit.Unitless)[0]);
                tbRandomSeed.Text = ParseUtility.DoubleToString(Math.Truncate(newValue));
            }
            finally
            {
                _ignoreRandomSeedChangeEvent = saveIgnoreValue;
            }

            // Set up exclusion textbox
            var exclusionRadius = StateContainer.GetNdValue("exclusionRadius");
            tbExclusionRadius.Text = ParseUtility.DoubleToString(exclusionRadius);

            UpdateParameterCellVisibility();

            _mCurrentlyIgnoringGridChangeEvents = false;

            // If check is enabled in analysis, this ensures analysis will be re-run
            StateContainer.SetValue("ResultsAreStale", true);
        }

        /// <summary>
        /// Add interactive rows to system parameters table.
        /// Fluid temp contingent on fluid phase selection.
        /// </summary>
        private void PopulateSystemParametersPiping()
        {
            dgSystemParameters_Piping.Rows.Clear();
            var vdColl2 = new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("pipeDiameter", "Pipe Outer Diameter", DistanceUnit.Inch,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("pipeThickness", "Pipe Wall Thickness", DistanceUnit.Inch,
                        StockConverters.DistanceConverter),
                }
            );
            if (FluidPhase.DisplayTemperature())
            {
                vdColl2.Add("fluidTemperature",
                            new ParameterWrapper("fluidTemperature", "Fluid Temperature", TempUnit.Celsius, StockConverters.TemperatureConverter) );
            }
            vdColl2.Add("fluidPressure",
                new ParameterWrapper("fluidPressure", "Fluid Pressure", PressureUnit.MPa, StockConverters.PressureConverter));
            vdColl2.Add("ambientTemperature",
                new ParameterWrapper("ambientTemperature", "Ambient Temperature", TempUnit.Celsius, StockConverters.TemperatureConverter));
            vdColl2.Add("ambientPressure",
                new ParameterWrapper("ambientPressure", "Ambient Pressure", PressureUnit.MPa, StockConverters.PressureConverter));

            StaticGridHelperRoutines.InitInteractiveGrid(dgSystemParameters_Piping, vdColl2, false);
            dgSystemParameters_Piping.Columns[0].Width = 200;
            CheckPipeFormValid();
        }

        private void CheckPipeFormValid()
        {
            bool showWarning = false;
            bool showError = false;
            string messageText = "";

            if (StateContainer.FuelPhaseIsSaturated())
            {
                showError = false;
                showWarning = true;
                messageText =
                    "Default data for leaks, failures, and ignition were generated for high pressure gaseous hydrogen systems and may not be appropriate for the selected phase";
            }
            if (!StateContainer.ReleasePressureIsValid())
            {
                // if liquid, validate fuel pressure
                messageText = MessageContainer.LiquidReleasePressureInvalid;
                showError = true;
                showWarning = false;
            }


            if (showWarning)
            {
                inputWarning.Visible = true;
                inputWarning.BackColor = Color.PaleGoldenrod;
                inputWarning.ForeColor = Color.DarkGoldenrod;
            } else if (showError)
            {
                inputWarning.Visible = true;
                inputWarning.BackColor = Color.MistyRose;
                inputWarning.ForeColor = Color.Maroon;
            }
            else
            {
                inputWarning.Visible = false;
                messageText = "";
            }

            inputWarning.Text = messageText;
        }


        private void GridValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
                StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2,
                    _mCurrentlyIgnoringGridChangeEvents);
        }

        private void dgComponents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private void dgSystemParameters_Piping_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
            CheckPipeFormValid();
        }

        private void dgManualOverrides_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private void dgSystemParameters_Vehicles_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
            // Update annual demands
            var row = e.RowIndex;
            Debug.WriteLine("Changed row:" + row + ", col: " + e.ColumnIndex);
            if (row != 3)
            {
                var numVehicles = StateContainer.GetNdValue("numVehicles");
                var numDays = StateContainer.GetNdValue("vehicleOperatingDays");
                var numFuelings = StateContainer.GetNdValue("dailyFuelings");
                var demands = numVehicles * numDays * numFuelings;
                StateContainer.SetNdValue("annualDemand", UnitlessUnit.Unitless, demands);
                dgSystemParameters_Vehicles.Rows[3].Cells[1].Value = demands;
            }
        }

        private void dgFacilityParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        /// <summary>
        /// Store row data in Occupant object while ensuring values are valid. If invalid, replace with existing.
        /// TODO: clean up data binding here
        /// </summary>
        /// <param name="changedRow"></param>
        private void UpdateOccupantSetandRowData(DataGridViewRow row, int changedColumn = -1)
        {
            _mCurrentlyIgnoringGridChangeEvents = true;
            DataGridViewCell numTargetsCell = row.Cells[(int) OccupantColumns.NumberOfTargets];
            DataGridViewCell xDistributionCell = row.Cells[(int) OccupantColumns.XDistType];
            DataGridViewCell xParamACell = row.Cells[(int) OccupantColumns.XDistParmA];
            DataGridViewCell xParamBCell = row.Cells[(int) OccupantColumns.XDistParmB];
            DataGridViewCell yDistributionCell = row.Cells[(int) OccupantColumns.YDistType];
            DataGridViewCell yParamACell = row.Cells[(int) OccupantColumns.YDistParmA];
            DataGridViewCell yParamBCell = row.Cells[(int) OccupantColumns.YDistParmB];
            DataGridViewCell zDistributionCell = row.Cells[(int) OccupantColumns.ZDistType];
            DataGridViewCell zParamACell = row.Cells[(int) OccupantColumns.ZDistParmA];
            DataGridViewCell zParamBCell = row.Cells[(int) OccupantColumns.ZDistParmB];
            DataGridViewCell descripCell = row.Cells[(int) OccupantColumns.Description];
            DataGridViewCell distanceUnitCell = row.Cells[(int) OccupantColumns.Unit];
            DataGridViewCell hoursCell = row.Cells[(int) OccupantColumns.ExposureHours];

            var occupantSet = (OccupantDistributionInfo) row.Tag;
            if (occupantSet == null)
            {
                occupantSet = new OccupantDistributionInfo();
                row.Tag = occupantSet;
                StateContainer.Instance.OccupantDistributionInfoCollection.Add(occupantSet);
            }

            if (numTargetsCell.Value == null || !int.TryParse(numTargetsCell.Value.ToString(), out int numTargets))
            {
                numTargets = occupantSet.NumTargets;
                numTargetsCell.Value = numTargets;
            }

            if (changedColumn != (int) OccupantColumns.Unit)  // skip updating values if user only changed units
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
                occupantSet.XLocDistribution = (EWorkerDistribution) xDistributionCell.Value;
                occupantSet.XLocParamA = xParamA;
                occupantSet.XLocParamB = xParamB;
                occupantSet.YLocDistribution = (EWorkerDistribution) yDistributionCell.Value;
                occupantSet.YLocParamA = yParamA;
                occupantSet.YLocParamB = yParamB;
                occupantSet.ZLocDistribution = (EWorkerDistribution) zDistributionCell.Value;
                occupantSet.ZLocParamA = zParamA;
                occupantSet.ZLocParamB = zParamB;
                occupantSet.ExposureHours = exposureHours;
            } else {
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
            _mCurrentlyIgnoringGridChangeEvents = false;
        }

        private void UpdateParameterCellVisibility()
        {
            int[] colStarts =
            {
                (int) OccupantColumns.XDistType, (int) OccupantColumns.YDistType,
                (int) OccupantColumns.ZDistType
            };
            for (var rowIndex = 0; rowIndex < dgOccupantInputDetails.RowCount - 1; rowIndex++)
            {
                var thisRow = dgOccupantInputDetails.Rows[rowIndex];
                // ensure values match units


                foreach (var colStart in colStarts)
                {
                    var typeCell = thisRow.Cells[colStart];
                    var paramBCell = thisRow.Cells[colStart + 2];
                    // if deterministic, clear param B and make it readonly
                    if (typeCell.Value != null && typeCell.Value.Equals(EWorkerDistribution.Constant))
                    {
                        var oldIgnore = _mCurrentlyIgnoringGridChangeEvents;
                        _mCurrentlyIgnoringGridChangeEvents = true;
                        paramBCell.Value = 0D;
                        _mCurrentlyIgnoringGridChangeEvents = oldIgnore;
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

        private void dgOccupantInputDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
            {
                var changedRow = dgOccupantInputDetails.Rows[e.RowIndex];
                UpdateOccupantSetandRowData(changedRow, e.ColumnIndex);
            }
        }

        private void dgOccupantInputDetails_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
            {
                bool ignoreState = _mCurrentlyIgnoringGridChangeEvents;
                _mCurrentlyIgnoringGridChangeEvents = true;

                var row = dgOccupantInputDetails.Rows[e.RowIndex - 1];
                var occupantSet = (OccupantDistributionInfo) row.Tag;
                if (occupantSet == null)
                {
                    occupantSet = new OccupantDistributionInfo();
                    row.Tag = occupantSet;
                    StateContainer.Instance.OccupantDistributionInfoCollection.Add(occupantSet);
                }

                row.Cells[(int) OccupantColumns.NumberOfTargets].Value = occupantSet.NumTargets;
                row.Cells[(int) OccupantColumns.Description].Value = occupantSet.Desc;
                row.Cells[(int) OccupantColumns.Unit].Value = occupantSet.ParamUnitType;
                row.Cells[(int) OccupantColumns.XDistType].Value = occupantSet.XLocDistribution;
                row.Cells[(int) OccupantColumns.XDistParmA].Value = occupantSet.XLocParamA;
                row.Cells[(int) OccupantColumns.XDistParmB].Value = occupantSet.XLocParamB;
                row.Cells[(int) OccupantColumns.YDistType].Value = occupantSet.YLocDistribution;
                row.Cells[(int) OccupantColumns.YDistParmA].Value = occupantSet.YLocParamA;
                row.Cells[(int) OccupantColumns.YDistParmB].Value = occupantSet.YLocParamB;
                row.Cells[(int) OccupantColumns.ZDistType].Value = occupantSet.ZLocDistribution;
                row.Cells[(int) OccupantColumns.ZDistParmA].Value = occupantSet.ZLocParamA;
                row.Cells[(int) OccupantColumns.ZDistParmB].Value = occupantSet.ZLocParamB;
                row.Cells[(int) OccupantColumns.ExposureHours].Value = occupantSet.ExposureHours;

                _mCurrentlyIgnoringGridChangeEvents = ignoreState;
            }
        }

        private void dgOccupantInputDetails_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
            {
                var thisDist = (OccupantDistributionInfo) e.Row.Tag;
                var distributions =
                    (OccupantDistributionInfoCollection) StateContainer.Instance.Parameters["OccupantDistributions"];
                if (distributions.Count > 1)
                {
                    distributions.Remove(thisDist);
                }
                else
                {
                    e.Cancel = true;
                    MessageBox.Show(
                        "Occupant distributions grid must contain at least one row. Add another row before deleting this one.");
                }
            }
        }

        private void dgOccupantInputDetails_DataError(object sender, DataGridViewDataErrorEventArgs e)
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
                    e.ColumnIndex + "(" + dgOccupantInputDetails.Columns[e.ColumnIndex].Name + "). Error details: " +
                    ex);
                MessageBox.Show("Program will be terminated.");
                Environment.Exit(0);
            }
        }

        private ConvertibleValue GetRandomSeedFromDatabase()
        {
            var result = StateContainer.Instance.GetStateDefinedValueObject("randomSeed");
            return result;
        }

        private void tbRandomSeed_TextChanged(object sender, EventArgs e)
        {
            if (!_ignoreRandomSeedChangeEvent)
            {
                _ignoreRandomSeedChangeEvent = true;
                try
                {
                    var randomSeedValueObj = GetRandomSeedFromDatabase();
                    StaticUiHelperRoutines.UnitlessTextBoxValueChanged(tbRandomSeed, ref randomSeedValueObj);
                }
                finally
                {
                    _ignoreRandomSeedChangeEvent = false;
                }
            }
        }

        private void tbExclusionRadius_TextChanged(object sender, EventArgs e)
        {
            var er = new double[1];
            ParseUtility.TryParseDouble(tbExclusionRadius.Text, out er[0]);
            var exclusionRadius = StateContainer.GetValue<ConvertibleValue>("exclusionRadius");
            exclusionRadius.SetValue(UnitlessUnit.Unitless, er);
        }

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetValue("ReleaseFluidPhase", fuelPhaseSelector.SelectedItem);
            PopulateSystemParametersPiping();
        }
    }
}