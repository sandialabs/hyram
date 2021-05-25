/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
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
    public partial class IndoorReleaseForm : UserControl
    {
        private bool _analysisStatus;
        private string _warningMsg;
        private double[] _concentrations;
        private double[] _depths;
        private double[] _dotMarkPressures;
        private double[] _dotMarkTimes;
        private string _layerPlotFilepath = "";
        private string _massPlotFilepath = "";
        private bool _mIgnoreHorzLinesSpecifierChangeEvent = true;
        private bool _mReactToDdTimePressureOptionsEdit;
        private double _overpressure = Double.NaN;
        private string _pressurePlotFilepath = "";
        private double[] _pressuresPerTime;
        private double _timeOfOverpressure = Double.NaN;

        // Parameters for analysis; filled once Execute is clicked
        private double[] _timesToPlot;
        private string _trajectoryPlotFilepath = "";

        public IndoorReleaseForm()
        {
            InitializeComponent();
        }

        private void IndoorReleaseForm_Load(object sender, EventArgs e)
        {
            PressureLinesCheckbox.Checked = Settings.Default.OPHorizLines;
            PressureLinesCheckbox.Refresh();
            PressuresPerTimeCheckbox.Checked = Settings.Default.OPMarkDots;
            PressuresPerTimeCheckbox.Refresh();

            OverpressureSpinner.Hide();
            InputWarning.Hide();
            outputWarning.Hide();

            // Initialize input options tab
            _mReactToDdTimePressureOptionsEdit = false;
            var pressuresAtTimes =
                (NdPressureAtTime[]) StateContainer.Instance.Parameters["OpWrapper.PlotDotsPressureAtTimes"];
            PressuresPerTimeGrid.Rows.Clear();
            foreach (var thisPressureTimeNode in pressuresAtTimes)
            {
                var values = new string[2];
                values[0] = thisPressureTimeNode.Time.ToString();
                values[1] = thisPressureTimeNode.Pressure.ToString();
                PressuresPerTimeGrid.Rows.Add(values);
            }
            _mReactToDdTimePressureOptionsEdit = true;

            // Initialize plot times
            var timesToPlot = StateContainer.Instance.GetStateDefinedValueObject("OpWrapper.SecondsToPlot")
                .GetValue(ElapsingTimeConversionUnit.Second);
            ParseUtility.PutDoubleArrayIntoTextBox(PlotTimesInput, timesToPlot);
            var maxTimes = StateContainer.Instance.GetStateDefinedValueObject("maxSimTime")
                .GetValue(ElapsingTimeConversionUnit.Second);
            if (maxTimes.Length > 0) MaxTimeInput.Text = "" + maxTimes[0];

            // initialize lines grid
            _mIgnoreHorzLinesSpecifierChangeEvent = true;
            try
            {
                var llp =
                    StateContainer.Instance.GetStateDefinedValueObject("OPWRAPPER.LIMITLINEPRESSURES");
                var limitLinePressures = llp.GetValue(PressureUnit.kPa);
                PressureLinesGrid.Rows.Clear();
                foreach (var limitLinePressure in limitLinePressures)
                {
                    var newValue = limitLinePressure.ToString();
                    PressureLinesGrid.Rows.Add(newValue);
                }
            }
            finally
            {
                _mIgnoreHorzLinesSpecifierChangeEvent = false;
            }

            // Populate notional nozzle model combo box. Manual to exclude Harstad
            //notionalNozzleSelector.Items.Add(NozzleModel.Birch.ToString());
            notionalNozzleSelector.DataSource = StateContainer.Instance.NozzleModels;
            notionalNozzleSelector.SelectedItem = StateContainer.GetValue<NozzleModel>("NozzleModel");

            StateContainer.Instance.FuelTypeChangedEvent += delegate{RefreshGridParameters();};
            PhaseSelection.DataSource = StateContainer.Instance.FluidPhases;
            PhaseSelection.SelectedItem = StateContainer.Instance.GetFluidPhase();

            RefreshGridParameters();
        }

        /// <summary>
        /// Change which parameters are displayed based on fuel selection
        /// </summary>
        private void RefreshGridParameters()
        {
            InputGrid.Rows.Clear();

            ParameterWrapperCollection formParams = new ParameterWrapperCollection(new[]
            {
                new ParameterWrapper("ambientPressure", "Ambient pressure", PressureUnit.Pa,
                    StockConverters.PressureConverter),
                new ParameterWrapper("ambientTemperature", "Ambient temperature", TempUnit.Kelvin,
                    StockConverters.TemperatureConverter),
                new ParameterWrapper("orificeDiameter", "Leak diameter", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("orificeDischargeCoefficient", "Discharge coefficient-orifice", UnitlessUnit.Unitless,
                    StockConverters.UnitlessConverter),
                new ParameterWrapper("releaseDischargeCoefficient", "Discharge coefficient-release", UnitlessUnit.Unitless,
                    StockConverters.UnitlessConverter),
                new ParameterWrapper("releaseArea", "Release area", AreaUnit.SqMeters,
                    StockConverters.AreaConverter),
                new ParameterWrapper("releaseHeight", "Release height", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("enclosureHeight", "Enclosure height", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("floorCeilingArea", "Floor/ceiling area", AreaUnit.SqMeters,
                    StockConverters.AreaConverter),
                new ParameterWrapper("releaseToWallDistance", "Distance from release to wall", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("ceilingVentArea", "Vent 1 (ceiling vent) cross-sectional area",
                    AreaUnit.SqMeters, StockConverters.AreaConverter),
                new ParameterWrapper("ceilingVentHeight", "Vent 1 (ceiling vent) height from floor",
                    DistanceUnit.Meter, StockConverters.DistanceConverter),
                new ParameterWrapper("floorVentArea", "Vent 2 (floor vent) cross-sectional area",
                    AreaUnit.SqMeters, StockConverters.AreaConverter),
                new ParameterWrapper("floorVentHeight", "Vent 2 (floor vent) height from floor",
                    DistanceUnit.Meter, StockConverters.DistanceConverter),
                new ParameterWrapper("releaseAngle", "Angle of release (0=horz.)", AngleUnit.Degrees,
                    StockConverters.AngleConverter)
            });

            // Add gas or liquid params
            if (StateContainer.FuelTypeIsGaseous())
            {
                formParams.Add("fluidPressure",
                    new ParameterWrapper("fluidPressure", "Tank fluid pressure (absolute)",
                        PressureUnit.Pa,
                        StockConverters.PressureConverter));

                if (FluidPhase.DisplayTemperature())
                {
                    formParams.Add("fluidTemperature",
                        new ParameterWrapper("fluidTemperature", "Tank fluid temperature", TempUnit.Kelvin,
                            StockConverters.TemperatureConverter));
                }

                formParams.Add("tankVolume",
                    new ParameterWrapper("tankVolume", "Tank volume", VolumeUnit.CubicMeter,
                        StockConverters.VolumeConverter));
                formParams.Add("ventVolumetricFlowRate",
                    new ParameterWrapper("ventVolumetricFlowRate", "Vent volumetric flow rate",
                        VolumetricFlowUnit.CubicMetersPerSecond, StockConverters.VolumetricFlowConverter));
            }

            StaticGridHelperRoutines.InitInteractiveGrid(InputGrid, formParams, false);
            InputGrid.Columns[0].Width = 235;

            CheckFormValid();
        }

        private void CheckFormValid()
        {
            bool showWarning = false;
            string warningText = "";

            // if liquid, validate fuel pressure
            if (!StateContainer.ReleasePressureIsValid())
            {
                warningText = MessageContainer.LiquidReleasePressureInvalid;
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
                ParseUtility.TryParseDouble(MaxTimeInput.Text, out maxTimeEnteredByUser);

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
            ExecuteBtn.Enabled = !showWarning;
        }

        private void Execute()
        {
            var ambPressure = StateContainer.GetNdValue("ambientPressure", PressureUnit.Pa);
            var ambTemp = StateContainer.GetNdValue("ambientTemperature", TempUnit.Kelvin);
            var h2Pressure = StateContainer.GetNdValue("fluidPressure", PressureUnit.Pa);
            var h2Temp = StateContainer.GetNdValue("fluidTemperature", TempUnit.Kelvin);
            var orificeDiam = StateContainer.GetNdValue("orificeDiameter", DistanceUnit.Meter);
            var orificeDischargeCoeff = StateContainer.GetNdValue("orificeDischargeCoefficient", UnitlessUnit.Unitless);
            var tankVolume = StateContainer.GetNdValue("tankVolume", VolumeUnit.CubicMeter);
            var releaseDischargeCoeff = StateContainer.GetNdValue("releaseDischargeCoefficient", UnitlessUnit.Unitless);
            var releaseArea = StateContainer.GetNdValue("releaseArea", AreaUnit.SqMeters);
            var releaseHeight = StateContainer.GetNdValue("releaseHeight", DistanceUnit.Meter);
            var enclosureHeight = StateContainer.GetNdValue("enclosureHeight", DistanceUnit.Meter);
            var floorCeilingArea = StateContainer.GetNdValue("floorCeilingArea", AreaUnit.SqMeters);
            var distReleaseToWall = StateContainer.GetNdValue("releaseToWallDistance", DistanceUnit.Meter);
            var ceilVentXArea = StateContainer.GetNdValue("ceilingVentArea", AreaUnit.SqMeters);
            var ceilVentHeight = StateContainer.GetNdValue("ceilingVentHeight", DistanceUnit.Meter);
            var floorVentXArea = StateContainer.GetNdValue("floorVentArea", AreaUnit.SqMeters);
            var floorVentHeight = StateContainer.GetNdValue("floorVentHeight", DistanceUnit.Meter);
            var flowRate =
                StateContainer.GetNdValue("ventVolumetricFlowRate", VolumetricFlowUnit.CubicMetersPerSecond);
            var releaseAngle = StateContainer.GetNdValue("releaseAngle", AngleUnit.Radians);
            var nozzleModel = StateContainer.GetValue<NozzleModel>("NozzleModel");

            // Blanket try block to catch odd Win8 VM issue
            //try
            //{
            Trace.TraceInformation("Primitive overpressure parameters gathered. Extracting advanced...");
            _timesToPlot =
                StateContainer.GetNdValueList("OpWrapper.SecondsToPlot", ElapsingTimeConversionUnit.Second);

            // Whether to mark pressures on chart. Gets custom time-pressure objects
            NdPressureAtTime[] pressuresAtTimes = { };

            if (PressuresPerTimeCheckbox.Checked)
            {
                pressuresAtTimes =
                    StateContainer.GetValue<NdPressureAtTime[]>("OpWrapper.PlotDotsPressureAtTimes");
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

            // Whether to plot line pressures
            var llp = StateContainer.GetNdValueList("OPWRAPPER.LIMITLINEPRESSURES", PressureUnit.kPa);
            double[] limitLinePressures = { };
            if (PressureLinesCheckbox.Checked) limitLinePressures = llp;

            var maxSimTime =
                StateContainer.GetNdValue("maxSimTime", ElapsingTimeConversionUnit.Second);

            // prep vars to hold results
            var numTimes = pressuresAtTimes.Length;
            _pressuresPerTime = new double[numTimes];
            _depths = new double[numTimes];
            _concentrations = new double[numTimes];

            Trace.TraceInformation("Initializing PhysicsInterface...");
            var physInt = new PhysicsInterface();

            _analysisStatus = physInt.ExecuteOverpressureAnalysis(
                ambPressure, ambTemp, h2Pressure, h2Temp, orificeDiam, orificeDischargeCoeff, tankVolume,
                releaseDischargeCoeff, releaseArea, releaseHeight, enclosureHeight, floorCeilingArea,
                distReleaseToWall,
                ceilVentXArea, ceilVentHeight, floorVentXArea, floorVentHeight, flowRate, releaseAngle, nozzleModel.GetKey(),
                _timesToPlot,
                _dotMarkPressures, _dotMarkTimes, limitLinePressures, maxSimTime,
                out string statusMsg, out _warningMsg,
                out _pressuresPerTime, out _depths, out _concentrations, out _overpressure, out _timeOfOverpressure,
                out _pressurePlotFilepath, out _massPlotFilepath, out _layerPlotFilepath,
                out _trajectoryPlotFilepath
            );
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
            ExecuteBtn.Enabled = true;

            if (_analysisStatus)
            {
                // Display result data and plots
                tbMaxPressure.Text = ParseUtility.DoubleToString(_overpressure, "N2");
                tbTime.Text = ParseUtility.DoubleToString(_timeOfOverpressure, "G4");

                overpressureResultGrid.SuspendLayout();
                overpressureResultGrid.Rows.Clear();

                try
                {
                    for (var i = 0; i < _timesToPlot.Length; i++)
                    {
                        overpressureResultGrid.Rows.Add(
                            _timesToPlot[i].ToString(),
                            _pressuresPerTime[i].ToString("E3"),
                            _depths[i].ToString("N3"),
                            _concentrations[i].ToString("N3")
                            );
                    }
                }
                finally
                {
                    overpressureResultGrid.ResumeLayout();
                }

                pbPressure.Load(_pressurePlotFilepath);
                pbLayer.Load(_layerPlotFilepath);
                pbFlammableMass.Load(_massPlotFilepath);
                pbTrajectory.Load(_trajectoryPlotFilepath);

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


                    var pressureAtTimes = new NdPressureAtTime[numRows];
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
                            if (ParseUtility.TryParseDouble(value0.ToString(), out thisTime))
                            {
                                var thisPressure = Double.NaN;
                                if (ParseUtility.TryParseDouble(value1.ToString(), out thisPressure))
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
                        StateContainer.Instance.Parameters["OpWrapper.PlotDotsPressureAtTimes"] = pressureAtTimes;
                        ExecuteBtn.Enabled = true;
                    }
                    else
                    {
                        ExecuteBtn.Enabled = false;
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
            if (!_mIgnoreHorzLinesSpecifierChangeEvent)
            {
                _mIgnoreHorzLinesSpecifierChangeEvent = true;
                var values = new List<double>();

                foreach (DataGridViewRow thisRow in PressureLinesGrid.Rows)
                {
                    var thisValue = thisRow.Cells[0].Value;
                    if (thisValue != null)
                    {
                        var doubleValue = Double.NaN;
                        if (ParseUtility.TryParseDouble((string) thisValue, out doubleValue)) values.Add(doubleValue);
                    }
                }

                var llp = StateContainer.Instance.GetStateDefinedValueObject("OPWRAPPER.LIMITLINEPRESSURES");
                llp.SetValue(PressureUnit.kPa, values.ToArray());
                _mIgnoreHorzLinesSpecifierChangeEvent = false;
            }
        }
        private static double[] ExtractArrayFromTextbox(TextBox tb)
        {
            var sResult = tb.Text.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            var result = new double[sResult.Length];
            for (var index = 0; index < result.Length; index++)
            {
                var parsedValue = Double.NaN;
                var successfullyParsed = ParseUtility.TryParseDouble(sResult[index], out parsedValue);

                if (successfullyParsed)
                    result[index] = parsedValue;
                else
                    result[index] = Double.NaN;
            }

            return result;
        }

        private void InputGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
            CheckFormValid();
        }

        private async void ExecuteBtn_Click(object sender, EventArgs e)
        {
            OverpressureSpinner.Show();
            outputWarning.Hide();
            ExecuteBtn.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void PlotTimesInput_TextChanged(object sender, EventArgs e)
        {
            var timesToPlot = ExtractArrayFromTextbox(PlotTimesInput);
            var values = StateContainer.Instance.GetStateDefinedValueObject("OpWrapper.SecondsToPlot");
            var maxSimTime =
                StateContainer.GetNdValue("maxSimTime", ElapsingTimeConversionUnit.Second);

            if (Enumerable.Max((IEnumerable<double>) timesToPlot) <= maxSimTime)
            {
                values.SetValue(ElapsingTimeConversionUnit.Second, timesToPlot);
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
                var values =
                    StateContainer.Instance.GetStateDefinedValueObject("maxSimTime");
                values.SetValue(ElapsingTimeConversionUnit.Second, maximumTimes);
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
            StateContainer.SetValue("ReleaseFluidPhase", PhaseSelection.SelectedItem);
            RefreshGridParameters();
        }

        private void notionalNozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var modelName = notionalNozzleSelector.SelectedItem.ToString();
            StateContainer.SetValue("NozzleModel", NozzleModel.ParseNozzleModelName(modelName));
        }
    }
}