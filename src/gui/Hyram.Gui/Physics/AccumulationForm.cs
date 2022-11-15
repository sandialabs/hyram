/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class AccumulationForm : UserControl
    {
        private StateContainer _state = State.Data;
        private bool _ignoreLineChange = true;
        private bool _analysisStatus;
        private string _warningMsg;
        private double[] _concentrations;
        private double[] _massFlowRates;
        private double[] _depths;
        private double[] _markedPressures;
        private double[] _markedTimes;
        private string _layerPlotFilepath = "";
        private string _massPlotFilepath = "";
        private bool _mReactToDdTimePressureOptionsEdit;
        private double _overpressure = Double.NaN;
        private string _pressurePlotFilepath = "";
        private double[] _pressuresPerTime;
        private double _timeOfOverpressure = Double.NaN;
        private string _massFlowPlotFilepath = "";

        // Parameters for analysis; filled once Execute is clicked
        private double[] _timesToPlot;
        private string _trajectoryPlotFilepath = "";

        public AccumulationForm()
        {
            InitializeComponent();
        }

        ~AccumulationForm()
        {
            GeometryPicture.Dispose();
            _state.FuelTypeChangedEvent -= OnFuelChange;
        }

        private void IndoorReleaseForm_Load(object sender, EventArgs e)
        {
            _state = State.Data;
            PressureLinesCheckbox.Checked = Settings.Default.OPHorizLines;
//            PressureLinesCheckbox.Refresh();
            PressuresPerTimeCheckbox.Checked = Settings.Default.OPMarkDots;
//            PressuresPerTimeCheckbox.Refresh();

            OverpressureSpinner.Hide();
            InputWarning.Hide();
            outputWarning.Hide();

            NozzleSelector.DataSource = _state.NozzleModels;
            NozzleSelector.SelectedItem = _state.Nozzle;
            PhaseSelection.DataSource = _state.Phases;

            _state.FuelTypeChangedEvent += OnFuelChange;
            RefreshForm();
        }

        private void OnFuelChange(object o, EventArgs e)
        {
            RefreshForm();
        }

        /// <summary>
        /// Change which parameters are displayed based on fuel selection
        /// </summary>
        private void RefreshForm()
        {
            PressureLinesCheckbox.Refresh();
            PressuresPerTimeCheckbox.Refresh();

            // Initialize input options tab
            // NOTE (Cianan): for simplicity, all accum. time values are stored unitless and converted to seconds,
            // corresponding to selected time unit, prior to analysis in execute() function.
            _mReactToDdTimePressureOptionsEdit = false;
            var pressuresAtTimes = _state.OpTimePressures;
            foreach (var ptNode in pressuresAtTimes)
            {
                var values = new string[2];
                values[0] = ptNode.Item1.ToString();
                values[1] = ptNode.Item2.ToString();
                PressuresPerTimeGrid.Rows.Add(values);
            }
            _mReactToDdTimePressureOptionsEdit = true;

            // Initialize plot times
            ParseUtility.PutDoubleArrayIntoTextBox(PlotTimesInput, _state.AccumulationPlotTimes);
            MaxTimeInput.Text = _state.MaxSimTime.GetValue().ToString();

            // initialize lines grid
            _ignoreLineChange = true;
            try
            {
                PressureLinesGrid.Rows.Clear();
                foreach (var pres in _state.AccumulationPlotPressures)
                {
                    PressureLinesGrid.Rows.Add(pres.ToString());
                }
            }
            finally
            {
                _ignoreLineChange = false;
            }

            NozzleSelector.DataSource = _state.NozzleModels;
            NozzleSelector.SelectedItem = _state.Nozzle;
            PhaseSelection.SelectedItem = _state.Phase;

            // Fill time unit selector
            var timeUnit = _state.AccumulationTimeUnit;
            var defaultIndex = 0;
            var timeUnits = timeUnit.GetType().GetEnumValues();
            var timeUnitObjects = new object[timeUnits.GetLength(0)];
            for (var index = 0; index < timeUnitObjects.Length; index++)
            {
                timeUnitObjects[index] = timeUnits.GetValue(index);
                if (timeUnitObjects[index].ToString() == timeUnit.ToString())
                {
                    defaultIndex = index;
                }
            }
            TimeUnitSelector.Items.AddRange(timeUnitObjects);
            TimeUnitSelector.SelectedIndex = defaultIndex;

            // refresh grid params
            InputGrid.Rows.Clear();
            var formParams = ParameterInput.GetParameterInputList(new[]
            {
                _state.AmbientTemperature,
                _state.AmbientPressure,
                _state.OrificeDiameter,
                _state.OrificeDischargeCoefficient,
                _state.ReleaseHeight,
                _state.EnclosureHeight,
                _state.FloorCeilingArea,
                _state.ReleaseToWallDistance,
                _state.CeilingVentArea,
                _state.CeilingVentHeight,
                _state.FloorVentArea,
                _state.FloorVentHeight,
                _state.ReleaseAngle,
                _state.FluidPressure,
            });
            if (_state.DisplayTemperature())
            {
                formParams.Add(new ParameterInput(_state.FluidTemperature));
            }
            formParams.Add(new ParameterInput(_state.TankVolume));
            formParams.Add(new ParameterInput(_state.VentFlowRate));

            GridHelpers.InitParameterGrid(InputGrid, formParams, false);
            InputGrid.Columns[0].Width = 200;

            CheckFormValid();
        }

        private void CheckFormValid()
        {
            bool showWarning = false;
            string warningText = "";

            // if liquid, validate fuel pressure
            if (!_state.ReleasePressureIsValid())
            {
                warningText = MessageContainer.ReleasePressureInvalid();
                showWarning = true;
            }

            // verify times
            var timesToPlot = ExtractArrayFromTextbox(PlotTimesInput);

            // Decide whether to allow the user to continue
            if (timesToPlot == null)
            {
                showWarning = true;
                warningText = "Select plot times on other tab.";
            }
            else
            {
                var maxSelectedTime = Enumerable.Max((IEnumerable<double>) timesToPlot);
                var maxTimeEnteredByUser = Double.NaN;
                double.TryParse(MaxTimeInput.Text, out maxTimeEnteredByUser);

                if (Double.IsNaN(maxSelectedTime))
                {
                    showWarning = true;
                    warningText = "Final input time is invalid or not set.";
                }
                else
                {
                    if (Double.IsNaN(maxTimeEnteredByUser))
                    {
                        showWarning = true;
                        warningText = "Maximum time is invalid or not set.";
                    }
                    else if (maxTimeEnteredByUser < maxSelectedTime)
                    {
                        showWarning = true;
                        warningText = "Times to plot must all be less than maximum time entered.";
                    }
                }
            }

            InputWarning.Text = warningText;
            InputWarning.Visible = showWarning;
            SubmitBtn.Enabled = !showWarning;
        }

        private void Execute()
        {
            var maxSimTime = _state.MaxSimTime.GetValue();
            _timesToPlot = _state.AccumulationPlotTimes;

            // Whether to plot marked pressures at times
            int timePressurePairCount = 0;
            _markedTimes = new double[0];
            _markedPressures = new double[0];

            if (PressuresPerTimeCheckbox.Checked)
            {
                timePressurePairCount = _state.OpTimePressures.Count;
                _markedTimes = new double[timePressurePairCount];
                _markedPressures = new double[timePressurePairCount];

                for (var i = 0; i < timePressurePairCount; i++)
                {
                    _markedTimes[i] = _state.OpTimePressures[i].Item1;
                    _markedPressures[i] = _state.OpTimePressures[i].Item2;
                }
            }

            // prep vars to hold results
            _pressuresPerTime = new double[timePressurePairCount];
            _depths = new double[timePressurePairCount];
            _concentrations = new double[timePressurePairCount];

            // Whether to plot line pressures
            double[] limitLinePressures = { };
            if (PressureLinesCheckbox.Checked)
            {
                limitLinePressures = _state.AccumulationPlotPressures;
            }

            // convert stored time values to corresponding units.
            var timesToPlotConv = new double[_timesToPlot.Length];
            var pressureTimesConv = _markedTimes;
            TimeUnit timeUnit = _state.AccumulationTimeUnit;
            double timeConversion = 1;
            switch (timeUnit)
            {
                case TimeUnit.Hour:
                    timeConversion = 3600;
                    break;
                case TimeUnit.Minute:
                    timeConversion = 60;
                    break;
                case TimeUnit.Millisecond:
                    timeConversion = 0.001;
                    break;
            }
            maxSimTime *= timeConversion;

            for (var i = 0; i < _timesToPlot.Length; i++)
            {
                timesToPlotConv[i] = _timesToPlot[i] * timeConversion;
            }

            if (PressuresPerTimeCheckbox.Checked)
            {
                for (var i = 0; i < timePressurePairCount; i++)
                {
                    pressureTimesConv[i] = _markedTimes[i] * timeConversion;
                }
            }

            Trace.TraceInformation("Initializing PhysicsInterface...");
            var physInt = new PhysicsInterface();

            bool isSteady = !ReleaseBlowdownBtn.Checked;

            _analysisStatus = physInt.AnalyzeAccumulation(timesToPlotConv, _markedPressures, pressureTimesConv, limitLinePressures, maxSimTime, isSteady,
                                                        out string statusMsg, out _warningMsg,
                                                        out _pressuresPerTime, out _depths, out _concentrations, out _massFlowRates, out _overpressure, out _timeOfOverpressure,
                                                        out _pressurePlotFilepath, out _massPlotFilepath, out _layerPlotFilepath,
                                                        out _trajectoryPlotFilepath, out _massFlowPlotFilepath);
            Trace.TraceInformation("PhysicsInterface call complete. Displaying results..");

            if (!_analysisStatus)
            {
                Trace.TraceError(statusMsg);
                MessageBox.Show(statusMsg);
            }
        }

        private void DisplayResults()
        {
            OverpressureSpinner.Hide();
            SubmitBtn.Enabled = true;

            if (_analysisStatus)
            {
                // Display result data and plots
                double overp = _overpressure / 1000;
                tbMaxPressure.Text = overp.ToString("N2");
                tbTime.Text = _timeOfOverpressure.ToString("G4");

                overpressureResultGrid.SuspendLayout();
                overpressureResultGrid.Rows.Clear();

                try
                {
                    for (var i = 0; i < _timesToPlot.Length; i++)
                    {
                        double pres = _pressuresPerTime[i] / 1000;
                        overpressureResultGrid.Rows.Add(
                            _timesToPlot[i].ToString(),
                            pres.ToString("E3"),
                            _depths[i].ToString("N3"),
                            _concentrations[i].ToString("N3"),
                            _massFlowRates[i].ToString("E3")
                            );
                    }
                }
                finally
                {
                    overpressureResultGrid.ResumeLayout();
                }

                pbPressure.Unload();
                pbPressure.Load(_pressurePlotFilepath);
                pbLayer.Unload();
                pbLayer.Load(_layerPlotFilepath);
                pbFlammableMass.Unload();
                pbFlammableMass.Load(_massPlotFilepath);
                pbTrajectory.Unload();
                pbTrajectory.Load(_trajectoryPlotFilepath);

                pbMassFlowPlot.Unload();
                // steady release won't provide mass flow plot
                if (!string.IsNullOrEmpty(_massFlowPlotFilepath))
                {
                    pbMassFlowPlot.Load(_massFlowPlotFilepath);
                }

                IOTabs.SelectedTab = outputTab;

                if (_warningMsg.Length != 0)
                {
                    outputWarning.Text = _warningMsg;
                    outputWarning.Show();
                }
            }
            else
            {
                outputWarning.Hide();
            }
        }


        private void TimePressureOptionsDataChanged()
        {
            if (_mReactToDdTimePressureOptionsEdit)
            {
                _mReactToDdTimePressureOptionsEdit = false;
                try
                {
                    var fail = false;
                    var numRows = PressuresPerTimeGrid.Rows.Count;
                    var cell0 = PressuresPerTimeGrid.Rows[numRows - 1].Cells[0];
                    var cell1 = PressuresPerTimeGrid.Rows[numRows - 1].Cells[1];
                    if (cell0.Value == null && cell1.Value == null) numRows--;

                    var pressureAtTimes = new List<(double, double)>(numRows);
                    var pressureIndex = 0;
                    for (var rowIndex = 0; rowIndex < numRows; rowIndex++)
                    {
                        var thisTime = Double.NaN;
                        if (PressuresPerTimeGrid.Rows[rowIndex].Cells.Count < 2)
                        {
                            fail = true;
                            break;
                        }

                        var value0 = PressuresPerTimeGrid.Rows[rowIndex].Cells[0].Value;
                        var value1 = PressuresPerTimeGrid.Rows[rowIndex].Cells[1].Value;
                        if (value0 != null && value1 != null)
                        {
                            if (double.TryParse(value0.ToString(), out thisTime))
                            {
                                var thisPressure = Double.NaN;
                                if (double.TryParse(value1.ToString(), out thisPressure))
                                {
                                    pressureAtTimes.Insert(pressureIndex, (thisTime, thisPressure));
//                                    pressureAtTimes[pressureIndex] = (thisTime, thisPressure);
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
                        _state.OpTimePressures = pressureAtTimes;
                        SubmitBtn.Enabled = true;
                    }
                    else
                    {
                        SubmitBtn.Enabled = false;
                    }
                }
                finally
                {
                    _mReactToDdTimePressureOptionsEdit = true;
                }
            }
        }

        private void HorizontalLinesDisplayDataChanged()
        {
            if (!_ignoreLineChange)
            {
                _ignoreLineChange = true;
                var values = new List<double>();

                foreach (DataGridViewRow thisRow in PressureLinesGrid.Rows)
                {
                    var thisValue = thisRow.Cells[0].Value;
                    if (thisValue != null)
                    {
                        if (double.TryParse((string) thisValue, out var doubleValue))
                        {
                            values.Add(doubleValue);
                        }
                    }
                }
                _state.AccumulationPlotPressures = values.ToArray();
                _ignoreLineChange = false;
            }
        }
        private static double[] ExtractArrayFromTextbox(TextBox tb)
        {
            var sResult = tb.Text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var result = new double[sResult.Length];
            for (var index = 0; index < result.Length; index++)
            {
                var successfullyParsed = double.TryParse(sResult[index], out double parsedValue);
                result[index] = successfullyParsed ? parsedValue : double.NaN;
            }

            return result;
        }

        private void InputGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
//            GridHelpers.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
            GridHelpers.ChangeParameterValue((DataGridView) sender, e, 1, 2);
            CheckFormValid();
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            OverpressureSpinner.Show();
            outputWarning.Hide();
            SubmitBtn.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void PlotTimesInput_TextChanged(object sender, EventArgs e)
        {
            var timesToPlot = ExtractArrayFromTextbox(PlotTimesInput);
            var maxSimTime = _state.MaxSimTime.GetValue();

            if (Enumerable.Max(timesToPlot) <= maxSimTime)
            {
                _state.AccumulationPlotTimes = timesToPlot;
                PlotTimesInput.ForeColor = Color.Black;
            }
            else
            {
                PlotTimesInput.ForeColor = Color.Red;
            }

            CheckFormValid();
        }

        private void MaxTimeInput_TextChanged(object sender, EventArgs e)
        {
            var maximumTimes = ExtractArrayFromTextbox(MaxTimeInput);
            if (maximumTimes.Length == 1)
            {
                _state.MaxSimTime.SetValue(maximumTimes[0]);
            }

            CheckFormValid();
        }

        private void PressureLinesCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.OPHorizLines = PressureLinesCheckbox.Checked;
            PressureLinesGroupBox.Visible = PressureLinesCheckbox.Checked;
        }

        private void PressuresPerTimeCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.OPMarkDots = PressuresPerTimeCheckbox.Checked;
            PressuresPerTimeGroupBox.Visible = PressuresPerTimeCheckbox.Checked;
        }

        private void PressureLinesGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            HorizontalLinesDisplayDataChanged();
        }
        private void PressureLinesGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HorizontalLinesDisplayDataChanged();
        }

        private void PressuresPerTimeGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            TimePressureOptionsDataChanged();
        }
        private void PressuresPerTimeGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            TimePressureOptionsDataChanged();
        }

        private void PressuresPerTimeGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            QuickFunctions.PerformNumericSortOnGrid(sender, e);
        }

        private void PressureLinesGrid_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            QuickFunctions.PerformNumericSortOnGrid(sender, e);
        }

        private void PhaseSelection_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Phase = (ModelPair)PhaseSelection.SelectedItem;
            RefreshForm();
        }

        private void NozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Nozzle = (ModelPair)NozzleSelector.SelectedItem;
        }

        private void TimeUnitSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var iValue = GetTimeUnitFromDropdown();

            if (iValue != null)
            {
                _state.AccumulationTimeUnit =
                    (TimeUnit) Enum.Parse(_state.AccumulationTimeUnit.GetType(),
                                                            iValue.ToString());
            }
        }

        private TimeUnit? GetTimeUnitFromDropdown()
        {
            TimeUnit? result = null;
            var selectedItemName =
                TimeUnitSelector.Items[TimeUnitSelector.SelectedIndex].ToString();

            if (Enum.TryParse<TimeUnit>(selectedItemName, out var iResult)) result = iResult;

            return result;
        }
    }
}