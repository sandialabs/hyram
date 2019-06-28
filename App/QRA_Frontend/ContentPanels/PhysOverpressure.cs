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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DefaultParsing;
using JrConversions;
using PyAPI;
using QRA_Frontend.Properties;
using QRA_Frontend.Resources;
using QRAState;
using UIHelpers;

namespace QRA_Frontend.ContentPanels
{
    public partial class CpOverpressure : UserControl, IQraBaseNotify
    {
        private double[] _concentrations;
        private double[] _depths;
        private double[] _dotMarkPressures;
        private double[] _dotMarkTimes;
        private string _layerPlotFilepath = "";
        private string _massPlotFilepath = "";
        private bool _mIgnoreHorzLinesSpecifierChangeEvent = true;
        private bool _mReactToDdTimePressureOptionsEdit;
        private double _overpressure = double.NaN;
        private string _pressurePlotFilepath = "";
        private double[] _pressuresPerTime;
        private double _timeOfOverpressure = double.NaN;

        // Parameters for analysis; filled once Execute is clicked
        private double[] _timesToPlot;
        private string _trajectoryPlotFilepath = "";

        public CpOverpressure()
        {
            InitializeComponent();
        }


        void IQraBaseNotify.Notify_LoadComplete()
        {
            var cpType = new ContentPanel().GetType();
            var cp = (ContentPanel) QuickFunctions.GetFirstParentOfSpecifiedType(this, cpType);
            cp.SetNarrative(Narratives.CPO_Overpressure);

            pbOverpSpinner.Hide();
            InitializeInputGrid();
            InitializeInputOptionsTab();
        }

        private void InitializeInputGrid()
        {
            StaticGridHelperRoutines.InitInteractiveGrid(dgInput,
                new ParameterWrapperCollection(
                    new[]
                    {
                        new ParameterWrapper("SysParam.ExternalPresMPA", "Ambient Pressure", PressureUnit.Pa,
                            StockConverters.PressureConverter),
                        new ParameterWrapper("SysParam.ExternalTempC", "Ambient Temperature", TempUnit.Kelvin,
                            StockConverters.TemperatureConverter),
                        new ParameterWrapper("FlameWrapper.P_H2", "H2 Tank Pressure", PressureUnit.Pa,
                            StockConverters.PressureConverter),
                        new ParameterWrapper("FlameWrapper.T_H2", "H2 Tank Temperature", TempUnit.Kelvin,
                            StockConverters.TemperatureConverter),
                        new ParameterWrapper("OpWrapper.TankVolume", "H2 Tank Volume", VolumeUnit.CubicMeter,
                            StockConverters.VolumeConverter),
                        new ParameterWrapper("FlameWrapper.d_orifice", "Leak Diameter", DistanceUnit.Meter,
                            StockConverters.DistanceConverter),
                        new ParameterWrapper("OpWrapper.Cd0", "Discharge Coefficient-Orifice", UnitlessUnit.Unitless,
                            StockConverters.UnitlessConverter),
                        new ParameterWrapper("OpWrapper.CdR", "Discharge Coefficient-Release", UnitlessUnit.Unitless,
                            StockConverters.UnitlessConverter),
                        new ParameterWrapper("OpWrapper.SecondaryArea", "Release Area", AreaUnit.SqMeters,
                            StockConverters.AreaConverter),
                        new ParameterWrapper("OpWrapper.S0", "Release Height", DistanceUnit.Meter,
                            StockConverters.DistanceConverter),
                        new ParameterWrapper("OpWrapper.H", "Enclosure Height", DistanceUnit.Meter,
                            StockConverters.DistanceConverter),
                        new ParameterWrapper("OpWrapper.FCA", "Floor/Ceiling Area", AreaUnit.SqMeters,
                            StockConverters.AreaConverter),
                        new ParameterWrapper("OpWrapper.Xwall", "Distance from Release to Wall", DistanceUnit.Meter,
                            StockConverters.DistanceConverter),
                        new ParameterWrapper("OpWrapper.Av_ceil", "Vent 1 (Ceiling Vent) Cross-Sectional Area",
                            AreaUnit.SqMeters, StockConverters.AreaConverter),
                        new ParameterWrapper("OpWrapper.CVHF", "Vent 1 (Ceiling Vent) Height from Floor",
                            DistanceUnit.Meter, StockConverters.DistanceConverter),
                        new ParameterWrapper("OpWrapper.Av_floor", "Vent 2 (Floor Vent) Cross-Sectional Area",
                            AreaUnit.SqMeters, StockConverters.AreaConverter),
                        new ParameterWrapper("OpWrapper.FVHF", "Vent 2 (Floor Vent) Height from Floor",
                            DistanceUnit.Meter, StockConverters.DistanceConverter),
                        new ParameterWrapper("OpWrapper.VolumeFlowRate", "Vent Volumetric Flow Rate",
                            VolumetricFlowUnit.CubicMetersPerSecond, StockConverters.VolumetricFlowConverter),
                        new ParameterWrapper("OpWrapper.ReleaseAngle", "Angle of Release (0=Horz.)", AngleUnit.Degrees,
                            StockConverters.AngleConverter)
                    }
                ));

            dgInput.Columns[0].Width = 235;
        }

        private void InitializeInputOptionsTab()
        {
            InitPressuresAtTimesGrid();

            // Initialize plot times
            var timesToPlot = QraStateContainer.Instance.GetStateDefinedValueObject("OpWrapper.SecondsToPlot")
                .GetValue(ElapsingTimeConversionUnit.Second);
            UiParsingRoutines.PutDoubleArrayIntoTextBox(tbPlotTimes, timesToPlot);
            var maxTimes = QraStateContainer.Instance.GetStateDefinedValueObject("OpWrapper.MaxSimTime")
                .GetValue(ElapsingTimeConversionUnit.Second);
            if (maxTimes.Length > 0) tbMaxTime.Text = "" + maxTimes[0];

            // initialize lines grid
            _mIgnoreHorzLinesSpecifierChangeEvent = true;
            try
            {
                var llp =
                    QraStateContainer.Instance.GetStateDefinedValueObject("OPWRAPPER.LIMITLINEPRESSURES");
                var limitLinePressures = llp.GetValue(PressureUnit.KPa);
                dgHorzLinesSpecifier.Rows.Clear();
                foreach (var limitLinePressure in limitLinePressures)
                {
                    var newValue = limitLinePressure.ToString();
                    dgHorzLinesSpecifier.Rows.Add(newValue);
                }
            }
            finally
            {
                _mIgnoreHorzLinesSpecifierChangeEvent = false;
            }
        }

        private void InitPressuresAtTimesGrid()
        {
            _mReactToDdTimePressureOptionsEdit = false;
            var pressuresAtTimes =
                (NdPressureAtTime[]) QraStateContainer.Instance.Parameters["OpWrapper.PlotDotsPressureAtTimes"];
            dgTimePressureOptions.Rows.Clear();
            foreach (var thisPressureTimeNode in pressuresAtTimes)
            {
                var values = new string[2];
                values[0] = thisPressureTimeNode.Time.ToString();
                values[1] = thisPressureTimeNode.Pressure.ToString();

                dgTimePressureOptions.Rows.Add(values);
            }

            _mReactToDdTimePressureOptionsEdit = true;
        }

        private void dgInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
        }

        private async void btnExecute_Click(object sender, EventArgs e)
        {
            pbOverpSpinner.Show();
            btnExecute.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void Execute()
        {
            var ambPressure = QraStateContainer.GetNdValue("SysParam.ExternalPresMPA", PressureUnit.Pa);
            var ambTemp = QraStateContainer.GetNdValue("SysParam.ExternalTempC", TempUnit.Kelvin);
            var h2Pressure = QraStateContainer.GetNdValue("FlameWrapper.P_H2", PressureUnit.Pa);
            var h2Temp = QraStateContainer.GetNdValue("FlameWrapper.T_H2", TempUnit.Kelvin);
            var orificeDiam = QraStateContainer.GetNdValue("FlameWrapper.d_orifice", DistanceUnit.Meter);
            var orificeDischargeCoeff = QraStateContainer.GetNdValue("OpWrapper.Cd0", UnitlessUnit.Unitless);
            var tankVolume = QraStateContainer.GetNdValue("OpWrapper.TankVolume", VolumeUnit.CubicMeter);
            var releaseDischargeCoeff = QraStateContainer.GetNdValue("OpWrapper.CdR", UnitlessUnit.Unitless);
            var releaseArea = QraStateContainer.GetNdValue("OpWrapper.SecondaryArea", AreaUnit.SqMeters);
            var releaseHeight = QraStateContainer.GetNdValue("OpWrapper.S0", DistanceUnit.Meter);
            var enclosureHeight = QraStateContainer.GetNdValue("OpWrapper.H", DistanceUnit.Meter);
            var floorCeilingArea = QraStateContainer.GetNdValue("OpWrapper.FCA", AreaUnit.SqMeters);
            var distReleaseToWall = QraStateContainer.GetNdValue("OpWrapper.Xwall", DistanceUnit.Meter);
            var ceilVentXArea = QraStateContainer.GetNdValue("OpWrapper.Av_ceil", AreaUnit.SqMeters);
            var ceilVentHeight = QraStateContainer.GetNdValue("OpWrapper.CVHF", DistanceUnit.Meter);
            var floorVentXArea = QraStateContainer.GetNdValue("OpWrapper.Av_floor", AreaUnit.SqMeters);
            var floorVentHeight = QraStateContainer.GetNdValue("OpWrapper.FVHF", DistanceUnit.Meter);
            var flowRate =
                QraStateContainer.GetNdValue("OpWrapper.VolumeFlowRate", VolumetricFlowUnit.CubicMetersPerSecond);
            var releaseAngle = QraStateContainer.GetNdValue("OpWrapper.ReleaseAngle", AngleUnit.Radians);

            // Blanket try block to catch odd Win8 VM issue
            try
            {
                Trace.TraceInformation("Primitive overpressure parameters gathered. Extracting advanced...");
                _timesToPlot =
                    QraStateContainer.GetNdValueList("OpWrapper.SecondsToPlot", ElapsingTimeConversionUnit.Second);

                // Whether to mark pressures on chart. Gets custom time-pressure objects
                NdPressureAtTime[] pressuresAtTimes = { };

                if (cbMarkChartWithPTDots.Checked)
                {
                    pressuresAtTimes =
                        QraStateContainer.GetValue<NdPressureAtTime[]>("OpWrapper.PlotDotsPressureAtTimes");
                    var numPressures = pressuresAtTimes.Length;
                    _dotMarkPressures = new double[numPressures];
                    _dotMarkTimes = new double[numPressures];
                    for (var i = 0; i < numPressures; i++)
                    {
                        _dotMarkPressures[i] = pressuresAtTimes[i].Pressure;
                        _dotMarkTimes[i] = pressuresAtTimes[i].Time;
                    }
                }
                else
                {
                    _dotMarkPressures = new double[0];
                    _dotMarkTimes = new double[0];
                }

                // WHether to plot line pressures
                var llp = QraStateContainer.GetNdValueList("OPWRAPPER.LIMITLINEPRESSURES", PressureUnit.KPa);
                double[] limitLinePressures = { };
                if (cbHorizontalLines.Checked) limitLinePressures = llp;

                var maxSimTime =
                    QraStateContainer.GetNdValue("OpWrapper.MaxSimTime", ElapsingTimeConversionUnit.Second);

                // prep vars to hold results
                var numTimes = pressuresAtTimes.Length;
                _pressuresPerTime = new double[numTimes];
                _depths = new double[numTimes];
                _concentrations = new double[numTimes];

                Trace.TraceInformation("Initializing PhysInterface...");
                var physInt = new PhysInterface();
                var status = physInt.ExecuteOverpressureAnalysis(
                    ambPressure, ambTemp, h2Pressure, h2Temp, orificeDiam, orificeDischargeCoeff, tankVolume,
                    releaseDischargeCoeff, releaseArea, releaseHeight, enclosureHeight, floorCeilingArea,
                    distReleaseToWall,
                    ceilVentXArea, ceilVentHeight, floorVentXArea, floorVentHeight, flowRate, releaseAngle,
                    _timesToPlot,
                    _dotMarkPressures, _dotMarkTimes, limitLinePressures, maxSimTime,
                    out _pressuresPerTime, out _depths, out _concentrations, out _overpressure, out _timeOfOverpressure,
                    out _pressurePlotFilepath, out _massPlotFilepath, out _layerPlotFilepath,
                    out _trajectoryPlotFilepath
                );
                Trace.TraceInformation("PhysInterface call complete. Displaying results..");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
            }
        }

        private void DisplayResults()
        {
            // Display result data and plots
            tbMaxPressure.Text = Parsing.DoubleToString(_overpressure);
            tbTime.Text = Parsing.DoubleToString(_timeOfOverpressure);

            FillDoubleColumnDataGrid(dgPressures, _timesToPlot, _pressuresPerTime);
            FillDoubleColumnDataGrid(dgDepthAndConcentration, _depths, _concentrations);

            pbPressure.Load(_pressurePlotFilepath);
            pbLayer.Load(_layerPlotFilepath);
            pbFlammableMass.Load(_massPlotFilepath);
            pbTrajectory.Load(_trajectoryPlotFilepath);

            pbOverpSpinner.Hide();
            btnExecute.Enabled = true;
            tcMain.SelectedTab = tpOutput;
        }


        private void FillDoubleColumnDataGrid(DataGridView gridView, double[] leftColumnValues,
            double[] rightColumnValues)
        {
            gridView.SuspendLayout();
            gridView.Rows.Clear();
            try
            {
                for (var i = 0; i < leftColumnValues.Length; i++)
                {
                    var leftStr = leftColumnValues[i].ToString();
                    var rightStr = rightColumnValues[i].ToString("E3");
                    gridView.Rows.Add(leftStr, rightStr);
                }
            }
            finally
            {
                gridView.ResumeLayout();
            }
        }

        private void dgTimePressureOptions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            TimePressureOptionsDataChanged();
        }

        private void TimePressureOptionsDataChanged()
        {
            if (_mReactToDdTimePressureOptionsEdit)
            {
                _mReactToDdTimePressureOptionsEdit = false;
                try
                {
                    var fail = false;
                    var numRows = dgTimePressureOptions.Rows.Count;
                    var cell0 = dgTimePressureOptions.Rows[numRows - 1].Cells[0];
                    var cell1 = dgTimePressureOptions.Rows[numRows - 1].Cells[1];
                    if (cell0.Value == null && cell1.Value == null) numRows--;


                    var pressureAtTimes = new NdPressureAtTime[numRows];
                    var pressureIndex = 0;
                    for (var rowIndex = 0; rowIndex < numRows; rowIndex++)
                    {
                        var thisTime = double.NaN;
                        if (dgTimePressureOptions.Rows[rowIndex].Cells.Count < 2)
                        {
                            fail = true;
                            break;
                        }

                        var value0 = dgTimePressureOptions.Rows[rowIndex].Cells[0].Value;
                        var value1 = dgTimePressureOptions.Rows[rowIndex].Cells[1].Value;
                        if (value0 != null && value1 != null)
                        {
                            if (Parsing.TryParseDouble(value0.ToString(), out thisTime))
                            {
                                var thisPressure = double.NaN;
                                if (Parsing.TryParseDouble(value1.ToString(), out thisPressure))
                                {
                                    pressureAtTimes[pressureIndex] = new NdPressureAtTime(thisTime, thisPressure);
                                }
                                else
                                {
                                    fail = true;
                                    break;
                                }
                            }
                            else
                            {
                                fail = true;
                                break;
                            }
                        }
                        else
                        {
                            fail = true;
                            break;
                        }

                        pressureIndex++;
                    }

                    if (!fail)
                    {
                        QraStateContainer.Instance.Parameters["OpWrapper.PlotDotsPressureAtTimes"] = pressureAtTimes;
                        btnExecute.Enabled = true;
                    }
                    else
                    {
                        btnExecute.Enabled = false;
                    }
                }
                finally
                {
                    _mReactToDdTimePressureOptionsEdit = true;
                }
            }
        }

        private void dgHorzLinesSpecifier_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HorizontalLinesDisplayDataChanged();
        }

        private void HorizontalLinesDisplayDataChanged()
        {
            if (!_mIgnoreHorzLinesSpecifierChangeEvent)
            {
                _mIgnoreHorzLinesSpecifierChangeEvent = true;

                var values = new List<double>();

                foreach (DataGridViewRow thisRow in dgHorzLinesSpecifier.Rows)
                {
                    var thisValue = thisRow.Cells[0].Value;
                    if (thisValue != null)
                    {
                        var doubleValue = double.NaN;
                        if (Parsing.TryParseDouble((string) thisValue, out doubleValue)) values.Add(doubleValue);
                    }
                }

                var llp =
                    QraStateContainer.Instance.GetStateDefinedValueObject("OPWRAPPER.LIMITLINEPRESSURES");

                var limitLinePressures = values.ToArray();
                llp.SetValue(PressureUnit.KPa, limitLinePressures);

                _mIgnoreHorzLinesSpecifierChangeEvent = false;
            }
        }

        private void tbPlotTimes_TextChanged(object sender, EventArgs e)
        {
            var timesToPlot = UiParsingRoutines.ExtractArrayFromTextbox(tbPlotTimes);

            var values =
                QraStateContainer.Instance.GetStateDefinedValueObject("OpWrapper.SecondsToPlot");
            if (PlotTimesAreUsable(timesToPlot))
            {
                values.SetValue(ElapsingTimeConversionUnit.Second, timesToPlot);
                tbPlotTimes.ForeColor = Color.Black;
            }
            else
            {
                tbPlotTimes.ForeColor = Color.Red;
            }

            CheckTimesVersusMaxTimes();
        }

        private void tbMaxTime_TextChanged(object sender, EventArgs e)
        {
            var maximumTimes = UiParsingRoutines.ExtractArrayFromTextbox(tbMaxTime);
            if (maximumTimes.Length == 1)
            {
                var values =
                    QraStateContainer.Instance.GetStateDefinedValueObject("OpWrapper.MaxSimTime");
                values.SetValue(ElapsingTimeConversionUnit.Second, maximumTimes);
            }

            CheckTimesVersusMaxTimes();
        }

        private void CheckTimesVersusMaxTimes()
        {
            var timesToPlot = UiParsingRoutines.ExtractArrayFromTextbox(tbPlotTimes);
            var showErrorLabel = false;
            var errorLabelText = "";

            // Decide whether to allow the user to continue
            if (timesToPlot != null)
            {
                var maxMaxTime = timesToPlot.Max();
                if (!double.IsNaN(maxMaxTime))
                {
                    var selectedMaxTime = double.NaN;
                    Parsing.TryParseDouble(tbMaxTime.Text, out selectedMaxTime);
                    if (!double.IsNaN(selectedMaxTime))
                    {
                        var maxTimeEnteredByUser = double.NaN;
                        Parsing.TryParseDouble(tbMaxTime.Text, out maxTimeEnteredByUser);
                        if (!double.IsNaN(maxTimeEnteredByUser))
                        {
                            if (maxTimeEnteredByUser >= maxMaxTime)
                            {
                                ; // Show button and hide error label
                                lblError.Text = "No error, and this caption is hidden.";
                            }
                            else
                            {
                                showErrorLabel = true;
                                errorLabelText = "Times to plot must all be less than maximum time selected.";
                            }
                        }
                        else
                        {
                            showErrorLabel = true;
                            errorLabelText = "Maximum Time is NaN or not set.";
                        }
                    }
                }
                else
                {
                    showErrorLabel = true;
                    errorLabelText = "SelectedMaxTime is NaN or unparseable.";
                }
            }
            else
            {
                showErrorLabel = true;
                errorLabelText = "Select plot times on other tab.";
            }

            btnExecute.Enabled = !showErrorLabel;
            lblError.Text = errorLabelText;
            lblError.Visible = showErrorLabel;
        }

        private bool PlotTimesAreUsable(double[] times)
        {
            var result = true;
            var maxValue = times.Max();
            if (maxValue > 30) result = false;

            return result;
        }

        private void cbHorizontalLines_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.OPHorizLines = cbHorizontalLines.Checked;
            gbHorizontalLines.Visible = cbHorizontalLines.Checked;
        }

        private void cpOverpressure_Load(object sender, EventArgs e)
        {
            cbHorizontalLines.Checked = Settings.Default.OPHorizLines;
            cbHorizontalLines.Refresh();
            cbMarkChartWithPTDots.Checked = Settings.Default.OPMarkDots;
            cbMarkChartWithPTDots.Refresh();
        }

        private void cbMarkChartWithPTDots_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.OPMarkDots = cbMarkChartWithPTDots.Checked;
            gbTimePressureOptions.Visible = cbMarkChartWithPTDots.Checked;
        }

        private void dgHorzLinesSpecifier_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            HorizontalLinesDisplayDataChanged();
        }

        private void dgTimePressureOptions_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            TimePressureOptionsDataChanged();
        }

        private void dgTimePressureOptions_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            QuickFunctions.PerformNumericSortOnGrid(sender, e);
        }

        private void dgHorzLinesSpecifier_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            QuickFunctions.PerformNumericSortOnGrid(sender, e);
        }
    }
}