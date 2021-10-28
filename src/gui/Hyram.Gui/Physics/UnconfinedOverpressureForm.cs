/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class UnconfinedOverpressureForm : UserControl
    {
        // Results
        private bool _analysisStatus;
        private string _warningMsg;
        private string _statusMsg;
        private bool _mIgnoreXyzChangeEvent = true;

        private double[] _xLocs;
        private double[] _yLocs;
        private double[] _zLocs;

        private double[] _overpressures;
        private double[] _impulses;
        private string _plotFilepath;

        public UnconfinedOverpressureForm()
        {
            InitializeComponent();
        }

        private void UnconfinedOverpressureForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            outputWarning.Hide();

            methodSelector.DataSource = StateContainer.Instance.OverpressureMethods;
            methodSelector.SelectedItem =
                StateContainer.GetValue<UnconfinedOverpressureMethod>("unconfinedOverpressureMethod");

            flameSpeedSelector.DataSource = StateContainer.Instance.MachFlameSpeeds;
            flameSpeedSelector.SelectedItem =
                StateContainer.GetNdValue("overpressureFlameSpeed");

            notionalNozzleSelector.DataSource = StateContainer.Instance.NozzleModels;
            notionalNozzleSelector.SelectedItem = StateContainer.GetValue<NozzleModel>("NozzleModel");

            fuelPhaseSelector.DataSource = StateContainer.Instance.FluidPhases;
            fuelPhaseSelector.SelectedItem = StateContainer.Instance.GetFluidPhase();

            // Watch custom event for when fuel type selection changes, and updated displayed params to match
            StateContainer.Instance.FuelTypeChangedEvent += delegate{RefreshParameterDisplay();};
            RefreshParameterDisplay();

            // Initialize flex input
            ParseUtility.PutDoubleArrayIntoTextBox(xLocsInput,
                StateContainer.Instance.GetStateDefinedValueObject("overpressure.x")
                    .GetValue(DistanceUnit.Meter));
            ParseUtility.PutDoubleArrayIntoTextBox(yLocsInput,
                StateContainer.Instance.GetStateDefinedValueObject("overpressure.y")
                    .GetValue(DistanceUnit.Meter));
            ParseUtility.PutDoubleArrayIntoTextBox(zLocsInput,
                StateContainer.Instance.GetStateDefinedValueObject("overpressure.z")
                    .GetValue(DistanceUnit.Meter));

            _mIgnoreXyzChangeEvent = false;
        }

        /// <summary>
        /// Change which parameters are displayed based on fuel selection
        /// </summary>
        private void RefreshParameterDisplay()
        {
            var method = StateContainer.GetOverpressureMethod();
            flameSpeedSelector.Visible = method == UnconfinedOverpressureMethod.BstMethod;
            flameSpeedLabel.Visible = method == UnconfinedOverpressureMethod.BstMethod;

            dgInput.Rows.Clear();

            // Create collection initially containing common params
            ParameterWrapperCollection formParams = new ParameterWrapperCollection(new[]
            {
                new ParameterWrapper("ambientTemperature", "Ambient temperature", TempUnit.Kelvin,
                    StockConverters.TemperatureConverter),
                new ParameterWrapper("ambientPressure", "Ambient pressure", PressureUnit.Pa,
                    StockConverters.PressureConverter),
                new ParameterWrapper("orificeDiameter", "Leak diameter", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("releaseAngle", "Release angle", AngleUnit.Radians,
                    StockConverters.AngleConverter),
                new ParameterWrapper("DischargeCoefficient", "Discharge coefficient", UnitlessUnit.Unitless,
                    StockConverters.UnitlessConverter),
                //new ParameterWrapper("relativeHumidity", "Relative humidity", UnitlessUnit.Unitless,
                //    StockConverters.UnitlessConverter),
                //new ParameterWrapper("releaseHeight", "Leak height from floor", DistanceUnit.Meter,
                //    StockConverters.DistanceConverter)
            });

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

            if (method == UnconfinedOverpressureMethod.TntMethod)
            {
                formParams.Add("tntEquivalenceFactor",
                    new ParameterWrapper("tntEquivalenceFactor", "TNT equivalence factor", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter));
            }

            //formParams.Add("overpressure.xmin",
            //    new ParameterWrapper("overpressure.xmin", "Plot x minimum (meters)", UnitlessUnit.Unitless,
            //        StockConverters.UnitlessConverter));

            StaticGridHelperRoutines.InitInteractiveGrid(dgInput, formParams, false);
            dgInput.Columns[0].Width = 180;
            CheckFormValid();
        }

        private void CheckFormValid()
        {
            int alertType = 0;
            string alertText = "";

            if (StateContainer.GetFuel() != FuelType.Hydrogen ||
                StateContainer.Instance.GetFluidPhase() != FluidPhase.GasDefault)
            {
                alertText = "Unconfined overpressure analysis currently tailored to gaseous H2";
                alertType = 1;
            }

            if (!StateContainer.ReleasePressureIsValid())
            {
                // if liquid, validate fuel pressure
                alertText = MessageContainer.GetAlertMessageReleasePressureInvalid();
                alertType = 2;
            }

            // Verify x,y,z inputs
            if (yLocsInput.Text.Trim().Length > 0)
            {
                var numXElems = CountElements(xLocsInput.Text);
                var numYElems = CountElements(yLocsInput.Text);
                var numZElems = CountElements(zLocsInput.Text);
                if (!(numZElems == numYElems && numZElems == numXElems))
                {
                    alertText = "X, Y, Z location arrays must be the same size";
                    alertType = 2;
                }
                lblXElemCount.Text = numXElems + " elements";
                lblYElemCount.Text = numYElems + " elements";
                lblZElementCount.Text = numZElems + " elements";
            }

            //inputWarning.BackColor = alertType == 1 ? MainForm.WarningBackColor : MainForm.ErrorBackColor;
            //inputWarning.ForeColor = alertType == 1 ? MainForm.WarningForeColor : MainForm.ErrorForeColor;
            inputWarning.Text = alertText;
            inputWarning.Visible = (alertType != 0);
            executeButton.Enabled = (alertType != 2);
        }

        private void Execute()
        {
            Trace.TraceInformation("Gathering parameters for overpressure analysis...");
            var ambTemp = StateContainer.GetNdValue("ambientTemperature", TempUnit.Kelvin);
            var ambPressure = StateContainer.GetNdValue("ambientPressure", PressureUnit.Pa);
            var relTemp = StateContainer.GetNdValue("fluidTemperature", TempUnit.Kelvin);
            var relPres = StateContainer.GetNdValue("fluidPressure", PressureUnit.Pa);
            var orificeDiam = StateContainer.GetNdValue("orificeDiameter", DistanceUnit.Meter);
            var dischargeCoeff = StateContainer.GetNdValue("DischargeCoefficient", UnitlessUnit.Unitless);
            var flameSpeed = StateContainer.GetNdValue("overpressureFlameSpeed", UnitlessUnit.Unitless);
            var tntFactor = StateContainer.GetNdValue("tntEquivalenceFactor", UnitlessUnit.Unitless);

            _xLocs = StateContainer.GetNdValueList("overpressure.x", DistanceUnit.Meter);
            _yLocs = StateContainer.GetNdValueList("overpressure.y", DistanceUnit.Meter);
            _zLocs = StateContainer.GetNdValueList("overpressure.z", DistanceUnit.Meter);
            var notionalNozzleModel = StateContainer.GetValue<NozzleModel>("NozzleModel");
            var method = StateContainer.GetOverpressureMethod();
            var relAngle = StateContainer.GetNdValue("releaseAngle", AngleUnit.Radians);

            Trace.TraceInformation("Creating PhysicsInterface for python call");
            var physInt = new PhysicsInterface();

            _analysisStatus = physInt.AnalyzeUnconfinedOverpressure(
                ambTemp, ambPressure, relTemp, relPres, orificeDiam, relAngle, dischargeCoeff,
                notionalNozzleModel, method, _xLocs, _yLocs, _zLocs,
                flameSpeed, tntFactor,
                out _statusMsg, out _warningMsg, out _overpressures, out _impulses, out _plotFilepath);
            Trace.TraceInformation("PhysicsInterface call complete");
        }

        private void DisplayResults()
        {
            if (!_analysisStatus)
            {
                MessageBox.Show(_statusMsg);
                spinnerPictureBox.Hide();
                executeButton.Enabled = true;
            }
            else if (_overpressures.Length == 0)
            {
                MessageBox.Show("Analysis yielded no data.");
                spinnerPictureBox.Hide();
                executeButton.Enabled = true;
            }
            else
            {
                plotBox.Load(_plotFilepath);

                dgResult.Rows.Clear();
                for (var index = 0; index < _overpressures.Length; index++)
                {
                    var values = new object[5];
                    values[0] = _xLocs[index].ToString("F4");
                    values[1] = _yLocs[index].ToString("F4");
                    values[2] = _zLocs[index].ToString("F4");
                    values[3] = _overpressures[index].ToString("E4");
                    values[4] = _impulses[index].ToString("E4");
                    dgResult.Rows.Add(values);
                }

                if (_warningMsg.Length != 0)
                {
                    outputWarning.Text = _warningMsg;
                    outputWarning.Show();
                }

                spinnerPictureBox.Hide();
                executeButton.Enabled = true;
                tcIO.SelectTab(outputTab);

            }
        }

        private void ExtractAndSaveXyzValues()
        {
            if (!_mIgnoreXyzChangeEvent)
            {
                var xValues =
                    ExtractFloatArrayFromDelimitedString(xLocsInput.Text, ',');
                var yValues =
                    ExtractFloatArrayFromDelimitedString(yLocsInput.Text, ',');
                var zValues =
                    ExtractFloatArrayFromDelimitedString(zLocsInput.Text, ',');
                if (xValues.Length == yValues.Length && yValues.Length == zValues.Length && zValues.Length > 0)
                {
                    StateContainer.SetValue("overpressure.x",
                        new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, xValues));
                    StateContainer.SetValue("overpressure.y",
                        new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, yValues));
                    StateContainer.SetValue("overpressure.z",
                        new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, zValues));
                }
            }
            CheckFormValid();
        }

        private int CountElements(string textToParse)
        {
            var values = textToParse.Trim().Split(',');
            return values.Length;
        }

        private async void executeButton_Click(object sender, EventArgs e)
        {
            if (plotBox.Image != null)
            {
                plotBox.Image.Dispose();
                plotBox.Image = null;
            }
            dgResult.Rows.Clear();
            outputWarning.Hide();
            spinnerPictureBox.Show();
            executeButton.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void dgInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
            CheckFormValid();
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                var sa = new EditableStringArray();
                string thisLine = null;

                foreach (DataGridViewColumn thisColumn in dgResult.Columns)
                    if (thisLine == null)
                        thisLine = thisColumn.HeaderText;
                    else
                        thisLine += "\t" + thisColumn.HeaderText;

                sa.Append(thisLine);

                foreach (DataGridViewRow thisRow in dgResult.Rows)
                {
                    thisLine = null;
                    foreach (DataGridViewCell thisCell in thisRow.Cells)
                        if (thisCell.Value != null)
                        {
                            var thisValue = thisCell.Value.ToString() ?? "";
                            if (thisLine == null)
                                thisLine = thisValue;
                            else
                                thisLine += "\t" + thisValue;
                        }

                    if (thisLine != null)
                    {
                        sa.Append(thisLine);
                        thisLine = null;
                    }
                }
                var clipboardText = sa.CombineToString(ArrayStringConversionOption.AppendCrlf);
                Clipboard.SetDataObject(clipboardText, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem copying to the clipboard: " + ex.Message);
            }
        }

        private void notionalNozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string modelName = notionalNozzleSelector.SelectedItem.ToString();
            StateContainer.SetValue("NozzleModel", NozzleModel.ParseNozzleModelName(modelName));
        }

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetReleasePhase((FluidPhase)fuelPhaseSelector.SelectedItem);
            RefreshParameterDisplay();
        }

        private void methodSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string methodName = methodSelector.SelectedItem.ToString();
            UnconfinedOverpressureMethod method = UnconfinedOverpressureMethod.ParseName(methodName);
            StateContainer.SetValue("UnconfinedOverpressureMethod", method);

            RefreshParameterDisplay();
        }

        private void xLocs_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }
        private void yLocs_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }
        private void zLocs_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }

        private void flameSpeedSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            double speed = (double)flameSpeedSelector.SelectedItem;
            StateContainer.SetNdValue("overpressureFlameSpeed", UnitlessUnit.Unitless, speed);
        }
    }
}