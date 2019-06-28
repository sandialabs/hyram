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
using DefaultParsing;
using JrConversions;
using QRA_Frontend.Resources;
using QRAState;

namespace QRA_Frontend.ContentPanels
{
    public enum OccupantGridColumnIdx
    {
        NumberOfTargets = 0,
        Description = 1,
        XLocDistType = 2,
        XLocDistParmA = 3,
        XLocDistParmB = 4,
        YLocDistType = 5,
        YLocDistParmA = 6,
        YLocDistParmB = 7,
        ZLocDistType = 8,
        ZLocDistParmA = 9,
        ZLocDistParmB = 10,
        Unit = 11,
        ExposureHours = 12
    }

    public partial class CpSystemDescription : UserControl, IQraBaseNotify
    {
        private bool _mCurrentlyIgnoringGridChangeEvents = true;

        public CpSystemDescription()
        {
            InitializeComponent();
            Load += cpSystemDescription_Load;
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
            StartIgnoringGridChangeEvents();

            // TODO (Cianan): Can simplify all this setup by using one-way binding to objects
            ContentPanel.SetNarrative(this, Narratives.SD__System_Description);
            // Set up component grid
            var vdColl = new ParameterWrapperCollection(new[]
            {
                new ParameterWrapper("Components.NrCompressors", "# Compressors"),
                new ParameterWrapper("Components.NrCylinders", "# Cylinders"),
                new ParameterWrapper("Components.NrValves", "# Valves"),
                new ParameterWrapper("Components.NrInstruments", "# Instruments"),
                new ParameterWrapper("Components.NrJoints", "# Joints"),
                new ParameterWrapper("Components.NrHoses", "# Hoses"),
                new ParameterWrapper("Components.PipeLength", "Pipes (length)", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("Components.NrFilters", "# Filters"),
                new ParameterWrapper("Components.NrFlanges", "# Flanges"),
                new ParameterWrapper("Components.NrExtraComp1", "# Extra Component 1"),
                new ParameterWrapper("Components.NrExtraComp2", "# Extra Component 2")
            });
            StaticGridHelperRoutines.InitInteractiveGrid(dgComponents, vdColl, false);

            // Set up system params grid
            var vdColl2 = new ParameterWrapperCollection(
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
            );
            StaticGridHelperRoutines.InitInteractiveGrid(dgSystemParameters_Piping, vdColl2, false);
            dgSystemParameters_Piping.Columns[0].Width = 200;

            // Vehicle grid
            StaticGridHelperRoutines.InitInteractiveGrid(dgSystemParameters_Vehicles, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("nVehicles", "Number of Vehicles"),
                    new ParameterWrapper("nFuelingsPerVehicleDay", "Number of Fuelings Per Vehicle Day"),
                    new ParameterWrapper("nVehicleOperatingDays", "Number of Vehicle Operating Days per Year"),
                    new ParameterWrapper("nDemands", "Annual Demands (calculated)", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter)
                }
            ), false);

            dgSystemParameters_Vehicles.Columns[0].Width = 300;
            dgSystemParameters_Vehicles.Columns[1].Width = 100;

            // Set up facility grid
            StaticGridHelperRoutines.InitInteractiveGrid(dgFacilityParameters, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("Facility.Length", "Length (x-direction)", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("Facility.Width", "Width (z-direction)", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("Facility.Height", "Height (y-direction)", DistanceUnit.Meter,
                        StockConverters.DistanceConverter)
                }
            ), false);

            SetOccupantInputsGrid();

            // Set up enclosure grid
            StaticGridHelperRoutines.InitInteractiveGrid(dgEnclosure, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("Enclosure.Height", "Height", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("Enclosure.AreaOfFloorAndCeiling", "Area (floor and ceiling)",
                        AreaUnit.SqMeters, StockConverters.AreaConverter),
                    new ParameterWrapper("Enclosure.HeightOfRelease", "Height of release", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("Enclosure.XWall", "Distance from release to wall (perpendicular)",
                        DistanceUnit.Meter, StockConverters.DistanceConverter)
                }
            ));

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


            var ceilingVentDgv = dgCeilingVent;
            SetVentGrid(ceilingVentDgv, WhichVent.Ceiling);
            ceilingVentDgv.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);

            var floorVentDgv = dgFloorVent;
            SetVentGrid(floorVentDgv, WhichVent.Floor);

            // Set up random seed textbox
            var saveIgnoreValue = _ignoreRandomSeedChangeEvent;
            _ignoreRandomSeedChangeEvent = true;
            try
            {
                var cv = GetRandomSeedFromDatabase();
                cv.EnsureBaseValueIsTruncatedInt();
                var newValue = Math.Truncate(cv.GetValue(UnitlessUnit.Unitless)[0]);
                tbRandomSeed.Text = Parsing.DoubleToString(Math.Truncate(newValue));
            }
            finally
            {
                _ignoreRandomSeedChangeEvent = saveIgnoreValue;
            }

            // Set up exclusion textbox
            var exclusionRadius = QraStateContainer.GetNdValue("QRAD:EXCLUSIONRADIUS");
            tbExclusionRadius.Text = Parsing.DoubleToString(exclusionRadius);

            SetXLocParamAndModifyCellVisibilityInOccupantsGrid();

            StopIgnoringGridChangeEvents();
            // If check is enabled in analysis, this ensures analysis will be re-run
            QraStateContainer.SetValue("ResultsAreStale", true);

            // Remove a page that's not yet used. 
            tcFacilityParameters.TabPages.Remove(tpEnclosure);
        }

        private void cpSystemDescription_Load(object sender, EventArgs e)
        {
            dgSystemParameters_Vehicles.CellEndEdit += dgSystemParameters_Vehicles_CellEndEdit;
        }

        private void StartIgnoringGridChangeEvents()
        {
            _mCurrentlyIgnoringGridChangeEvents = true;
        }

        private void StopIgnoringGridChangeEvents()
        {
            _mCurrentlyIgnoringGridChangeEvents = false;
        }

        private bool _ignoreRandomSeedChangeEvent;

        private void SetVentGrid(DataGridView dg, WhichVent gridPurpose)
        {
            var crossSectionalAreaKey = gridPurpose + ".CrossSectionalArea";
            var heightFromFloorKey = gridPurpose + ".VentHeightFromFloor";
            var dischargeCoefficientKey = gridPurpose + ".DischargeCoefficient";
            var windVelocityKey = gridPurpose + ".WindVelocity";
            StaticGridHelperRoutines.InitInteractiveGrid(dg, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper(crossSectionalAreaKey, "Cross-sectional area", AreaUnit.SqMeters,
                        StockConverters.AreaConverter),
                    new ParameterWrapper(heightFromFloorKey, "Vent height from floor", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper(dischargeCoefficientKey, "Discharge coefficient", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper(windVelocityKey, "Wind velocity", SpeedUnit.MetersPerSecond,
                        StockConverters.SpeedConverter)
                }));
        }

        private void SetOccupantInputsGrid()
        {
            var distXColumn =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantGridColumnIdx.XLocDistType];
            distXColumn.DataSource = typeof(EWorkerDistribution).GetEnumValues();
            distXColumn.ValueType = typeof(EWorkerDistribution);

            var distYColumn =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantGridColumnIdx.YLocDistType];
            distYColumn.DataSource = typeof(EWorkerDistribution).GetEnumValues();
            distYColumn.ValueType = typeof(EWorkerDistribution);

            var distZColumn =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantGridColumnIdx.ZLocDistType];
            distZColumn.DataSource = typeof(EWorkerDistribution).GetEnumValues();
            distZColumn.ValueType = typeof(EWorkerDistribution);

            dgOccupantInputDetails.Columns[(int) OccupantGridColumnIdx.Description].DefaultCellStyle.WrapMode =
                DataGridViewTriState.True;

            var unitClm =
                (DataGridViewComboBoxColumn) dgOccupantInputDetails.Columns[(int) OccupantGridColumnIdx.Unit];
            unitClm.DataSource = typeof(DistanceUnit).GetEnumValues();
            unitClm.ValueType = typeof(DistanceUnit);
            OccupantDistributionInfoCollection distributions = null;

            try
            {
                distributions = QraStateContainer.Instance.OccupantDistributionInfoCollection;
            }
            catch (InvalidCastException ex)
            {
                MessageBox.Show(
                    "You loaded an occupant distribution collection from a file saved using a previous version of HyRAM. That collection " +
                    "is not compatible with the extended type HyRAM now uses. A new default distribution will be created for you, but your old settings " +
                    "were lost.");
                QraStateContainer.Instance.InitOccupantDistributions(true);
                distributions = QraStateContainer.Instance.OccupantDistributionInfoCollection;
                Debug.Write(ex.Message);
            }

            foreach (var dist in distributions)
            {
                var newRow = new DataGridViewRow();

                newRow.CreateCells(dgOccupantInputDetails, dist.NumTargets, dist.Desc,
                    dist.XLocDistribution, dist.XLocParamA, dist.XLocParamB,
                    dist.YLocDistribution, dist.YLocParamA, dist.YLocParamB,
                    dist.ZLocDistribution, dist.ZLocParamA, dist.ZLocParamB,
                    dist.ParamUnitType, dist.ExposureHours);
                newRow.Tag = dist;

                if ((EWorkerDistribution) dist.XLocDistribution == EWorkerDistribution.Deterministic)
                {
                    var thisCell = newRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB];
                    thisCell.Value = null;
                    thisCell.ReadOnly = true;
                    thisCell.Style.BackColor = Color.LightGray;
                    thisCell.Style.ForeColor = Color.DarkGray;
                }

                if ((EWorkerDistribution) dist.ZLocDistribution == EWorkerDistribution.Deterministic)
                {
                    var thisCell = newRow.Cells[(int) OccupantGridColumnIdx.ZLocDistParmB];

                    thisCell.Value = null;
                    thisCell.ReadOnly = true;
                    thisCell.Style.BackColor = Color.LightGray;
                    thisCell.Style.ForeColor = Color.DarkGray;
                }

                dgOccupantInputDetails.Rows.Add(newRow);
            }
        }

        private void GridValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
                StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2,
                    _mCurrentlyIgnoringGridChangeEvents);
            //CalculateAnnualDemands();
        }

        private void dgComponents_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private void dgSystemParameters_Piping_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
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
                var numVehicles = QraStateContainer.GetNdValue("nVehicles");
                var numDays = QraStateContainer.GetNdValue("nVehicleOperatingDays");
                var numFuelings = QraStateContainer.GetNdValue("nFuelingsPerVehicleDay");
                var demands = numVehicles * numDays * numFuelings;
                QraStateContainer.SetNdValue("nDemands", UnitlessUnit.Unitless, demands);
                dgSystemParameters_Vehicles.Rows[3].Cells[1].Value = demands; // TODO: binding
            }
        }

        private void dgFacilityParameters_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private void HarvestOccupantDatagridRow(DataGridViewRow changedRow, int columnIndex)
        {
            var distributionNode = (OccupantDistributionInfo) changedRow.Tag;

            var descriptionNodeCell = changedRow.Cells[(int) OccupantGridColumnIdx.Description];
            var nrTargetsCell =
                (DataGridViewTextBoxCell) changedRow.Cells[(int) OccupantGridColumnIdx.NumberOfTargets];

            var xLocDistTypeCell = changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistType];
            var yLocDistTypeCell = changedRow.Cells[(int) OccupantGridColumnIdx.YLocDistType];
            var zLocDistTypeCell = changedRow.Cells[(int) OccupantGridColumnIdx.ZLocDistType];

            var xLocDistParmACell = changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmA];
            var yLocDistParmACell = changedRow.Cells[(int) OccupantGridColumnIdx.YLocDistParmA];
            var zLocDistParmACell = changedRow.Cells[(int) OccupantGridColumnIdx.ZLocDistParmA];

            var xLocDistParmBCell = changedRow.Cells[(int) OccupantGridColumnIdx.XLocDistParmB];
            var yLocDistParmBCell = changedRow.Cells[(int) OccupantGridColumnIdx.YLocDistParmB];
            var zLocDistParmBCell = changedRow.Cells[(int) OccupantGridColumnIdx.ZLocDistParmB];

            if (distributionNode == null)
            {
                distributionNode = new OccupantDistributionInfo();
                changedRow.Tag = distributionNode;
                QraStateContainer.Instance.OccupantDistributionInfoCollection.Add(distributionNode);
            }

            var numTargets = 0;

            if (nrTargetsCell.Value != null)
            {
                var changedRowNrTargetsValueString = nrTargetsCell.Value.ToString();
                if (changedRowNrTargetsValueString != distributionNode.NumTargets.ToString())
                {
                    numTargets = int.MinValue;
                    if (int.TryParse(changedRowNrTargetsValueString, out numTargets))
                        distributionNode.NumTargets = numTargets;
                    else
                        nrTargetsCell.Value = distributionNode.NumTargets;
                }

                if (descriptionNodeCell.Value != null) distributionNode.Desc = descriptionNodeCell.Value.ToString();

                EnsureDistTypeValueValid(xLocDistTypeCell);
                EnsureDistTypeValueValid(yLocDistTypeCell);
                EnsureDistTypeValueValid(zLocDistTypeCell);

                distributionNode.XLocDistribution = (EWorkerDistribution) xLocDistTypeCell.Value;
                distributionNode.YLocDistribution = (EWorkerDistribution) yLocDistTypeCell.Value;
                distributionNode.ZLocDistribution = (EWorkerDistribution) zLocDistTypeCell.Value;

                distributionNode.XLocParamA = HarvestParamAFromCells(xLocDistParmACell, distributionNode.XLocParamA);
                distributionNode.YLocParamA = HarvestParamAFromCells(yLocDistParmACell, distributionNode.YLocParamA);
                distributionNode.ZLocParamA = HarvestParamAFromCells(zLocDistParmACell, distributionNode.ZLocParamA);

                distributionNode.XLocParamB = HarvestParamBFromCells(xLocDistParmBCell, distributionNode.XLocParamA,
                    distributionNode.XLocParamB);
                distributionNode.YLocParamB = HarvestParamBFromCells(yLocDistParmBCell, distributionNode.YLocParamA,
                    distributionNode.YLocParamB);
                distributionNode.ZLocParamB = HarvestParamBFromCells(zLocDistParmBCell, distributionNode.ZLocParamA,
                    distributionNode.ZLocParamB);

                SetXLocParamAndModifyCellVisibilityInOccupantsGrid(changedRow, distributionNode,
                    (int) OccupantGridColumnIdx.XLocDistType, (int) OccupantGridColumnIdx.XLocDistParmA,
                    (int) OccupantGridColumnIdx.XLocDistParmB);
                SetXLocParamAndModifyCellVisibilityInOccupantsGrid(changedRow, distributionNode,
                    (int) OccupantGridColumnIdx.YLocDistType, (int) OccupantGridColumnIdx.YLocDistParmA,
                    (int) OccupantGridColumnIdx.YLocDistParmB);
                SetXLocParamAndModifyCellVisibilityInOccupantsGrid(changedRow, distributionNode,
                    (int) OccupantGridColumnIdx.ZLocDistType, (int) OccupantGridColumnIdx.ZLocDistParmA,
                    (int) OccupantGridColumnIdx.ZLocDistParmB);
                var distanceUnitCell = changedRow.Cells[(int) OccupantGridColumnIdx.Unit];
                if (distanceUnitCell.Value == null) distanceUnitCell.Value = DistanceUnit.Meter;

                distributionNode.ParamUnitType = (DistanceUnit) distanceUnitCell.Value;

                var exposureHours = double.NaN;
                var exposureHoursCell = changedRow.Cells[(int) OccupantGridColumnIdx.ExposureHours];

                if (Parsing.TryParseDouble((exposureHoursCell.Value ?? string.Empty).ToString(), out exposureHours))
                    distributionNode.ExposureHours = exposureHours;
                else
                    exposureHoursCell.Value = distributionNode.ExposureHours;
            }
        }

        private double HarvestParamBFromCells(DataGridViewCell xLocDistParmBCell, double paramA, double paramB)
        {
            var resultParamB = paramB;

            if (xLocDistParmBCell.Value != null)
            {
                if (!Parsing.TryParseDouble((xLocDistParmBCell.Value ?? string.Empty).ToString(), out resultParamB))
                    if (xLocDistParmBCell.Value.ToString() != paramB.ToString())
                    {
                        var oldIgnore = _mCurrentlyIgnoringGridChangeEvents;

                        _mCurrentlyIgnoringGridChangeEvents = true;
                        try
                        {
                            xLocDistParmBCell.Value = paramB.ToString();
                        }
                        finally
                        {
                            _mCurrentlyIgnoringGridChangeEvents = oldIgnore;
                        }
                    }
            }
            else
            {
                resultParamB = 0D;
            }

            return resultParamB;
        }

        private double HarvestParamAFromCells(DataGridViewCell paramACell, double previousParamAValue)
        {
            var resultParamA = previousParamAValue;

            if (!Parsing.TryParseDouble((paramACell.Value ?? string.Empty).ToString(), out resultParamA))
            {
                paramACell.Value = previousParamAValue.ToString();
                resultParamA = previousParamAValue;
            }

            return resultParamA;
        }

        private void EnsureDistTypeValueValid(DataGridViewCell distTypeCell)
        {
            if (distTypeCell.Value == null) distTypeCell.Value = EWorkerDistribution.Uniform;
        }

        private void SetXLocParamAndModifyCellVisibilityInOccupantsGrid()
        {
            int[] colStarts =
            {
                (int) OccupantGridColumnIdx.XLocDistType, (int) OccupantGridColumnIdx.YLocDistType,
                (int) OccupantGridColumnIdx.ZLocDistType
            };
            for (var rowIndex = 0; rowIndex < dgOccupantInputDetails.RowCount - 1; rowIndex++)
            {
                var thisRow = dgOccupantInputDetails.Rows[rowIndex];
                foreach (var colStart in colStarts)
                {
                    var colDistType = colStart;
                    var colDistParmA = colStart + 1;
                    var colDistParmB = colStart + 2;

                    SetXLocParamAndModifyCellVisibilityInOccupantsGrid(thisRow, (OccupantDistributionInfo) thisRow.Tag,
                        colDistType, colDistParmA, colDistParmB);
                }
            }
        }

        private double GetDoubleCellValue(object value)
        {
            var result = double.NaN;
            if (value == null)
            {
                result = 0D;
            }
            else
            {
                var valueType = value.GetType();
                var doubleType = 0D.GetType();
                var stringType = "".GetType();

                if (valueType == stringType)
                {
                    var strValue = (string) value;
                    if (strValue == null)
                        result = 0D;
                    else
                        result = Parsing.ParseDouble(strValue);
                }
                else if (valueType == doubleType)
                {
                    result = (double) value;
                }
                else
                {
                    throw new Exception("Type of " + value.GetType().Name + " is unrecognized.");
                }
            }

            return result;
        }

        private void SetXLocParamAndModifyCellVisibilityInOccupantsGrid(DataGridViewRow changedRow,
            OccupantDistributionInfo dist, int colDistType, int colParamA, int colParamB)
        {
            var paramBCell = changedRow.Cells[colParamB];

            if (changedRow.Cells[colDistType].Value.Equals(EWorkerDistribution.Deterministic))
            {
                //Don't use the second parameter value for deterministic
                var paramBCellValue = GetDoubleCellValue(paramBCell.Value);

                if (paramBCellValue != 0D)
                {
                    var oldIgnore = _mCurrentlyIgnoringGridChangeEvents;
                    try
                    {
                        paramBCell.Value = 0D;
                    }
                    finally
                    {
                        _mCurrentlyIgnoringGridChangeEvents = oldIgnore;
                    }
                }

                MakeParamBCellReadOnly(paramBCell);
            }
            else
            {
                MakeParamBCellReadWrite(changedRow, paramBCell);
            }
        }

        private static void MakeParamBCellReadWrite(DataGridViewRow changedRow, DataGridViewCell paramBCell)
        {
            if (paramBCell.Style.BackColor != changedRow.DataGridView.DefaultCellStyle.BackColor)
                paramBCell.Style.BackColor = changedRow.DataGridView.DefaultCellStyle.BackColor;

            if (paramBCell.Style.ForeColor != changedRow.DataGridView.DefaultCellStyle.ForeColor)
                paramBCell.Style.ForeColor = changedRow.DataGridView.DefaultCellStyle.ForeColor;

            if (paramBCell.ReadOnly) paramBCell.ReadOnly = false;
        }

        private static void MakeParamBCellReadOnly(DataGridViewCell paramBCell)
        {
            if (!paramBCell.ReadOnly) paramBCell.ReadOnly = true;

            if (paramBCell.Style.BackColor != Color.LightGray) paramBCell.Style.BackColor = Color.LightGray;

            if (paramBCell.Style.ForeColor != Color.DarkGray) paramBCell.Style.ForeColor = Color.DarkGray;
        }

        private void dgOccupantInputDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
            {
                var changedRow = dgOccupantInputDetails.Rows[e.RowIndex];
                HarvestOccupantDatagridRow(changedRow, e.ColumnIndex);
            }
        }

        private void dgOccupantInputDetails_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
            {
                var changedRow = dgOccupantInputDetails.Rows[e.RowIndex - 1];
                HarvestOccupantDatagridRow(changedRow, -1);

                var distributions =
                    QraStateContainer.Instance.OccupantDistributionInfoCollection;
                distributions.Add((OccupantDistributionInfo) changedRow.Tag);
            }
        }

        private void dgOccupantInputDetails_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            if (!_mCurrentlyIgnoringGridChangeEvents)
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

                SetXLocParamAndModifyCellVisibilityInOccupantsGrid();
            }
        }

        private void cbIncludeCeilingVent_CheckedChanged(object sender, EventArgs e)
        {
            dgCeilingVent.Enabled = cbIncludeCeilingVent.Checked;
        }

        private void cbIncludeFloorVent_CheckedChanged(object sender, EventArgs e)
        {
            dgFloorVent.Enabled = cbIncludeFloorVent.Checked;
        }

        private void dgEnclosure_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private void dgCeilingVent_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private void dgFloorVent_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private void cbDefineAnEnclosure_CheckedChanged(object sender, EventArgs e)
        {
            gbEnclosure.Enabled = cbDefineAnEnclosure.Checked;
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

        private NdConvertibleValue GetRandomSeedFromDatabase()
        {
            var result = QraStateContainer.Instance.GetStateDefinedValueObject("RANDOMSEED");
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
            SetDatabaseExclusionRadius(Parsing.ParseDouble(tbExclusionRadius.Text));
        }

        private void SetDatabaseExclusionRadius(double value)
        {
            var er = new double[1];
            er[0] = value;
            var exclusionRadius = QraStateContainer.GetValue<NdConvertibleValue>("QRAD:EXCLUSIONRADIUS");
            exclusionRadius.SetValue(UnitlessUnit.Unitless, er);
        }
    }
}