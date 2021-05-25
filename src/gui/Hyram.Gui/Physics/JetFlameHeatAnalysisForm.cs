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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class JetFlameHeatAnalysisForm : UserControl
    {
        // Results
        private bool _analysisStatus;
        private string _warningMsg;
        private string _statusMsg;
        private double[] _fluxData;
        private string _fluxPlotFilepath;
        private bool _mIgnoreXyzChangeEvent = true;

        private double[] _radHeatFluxX;
        private double[] _radHeatFluxY;

        private double[] _radHeatFluxZ;
        private string _tempPlotFilepath;

        public JetFlameHeatAnalysisForm()
        {
            InitializeComponent();
        }

        private void JetFlameHeatAnalysisForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            outputWarning.Hide();

            notionalNozzleSelector.DataSource = StateContainer.Instance.NozzleModels;
            notionalNozzleSelector.SelectedItem = StateContainer.GetValue<NozzleModel>("NozzleModel");

            fuelPhaseSelector.DataSource = StateContainer.Instance.FluidPhases;
            fuelPhaseSelector.SelectedItem = StateContainer.Instance.GetFluidPhase();

            // If None option is selected (e.g. in QRA), reset it to Multi
            var selectedRadSrc = StateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel");
            if (selectedRadSrc == RadiativeSourceModels.None)
            {
                selectedRadSrc = RadiativeSourceModels.Multi;
                StateContainer.SetValue("RadiativeSourceModel", selectedRadSrc);
            }

            // Watch custom event for when fuel type selection changes, and updated displayed params to match
            StateContainer.Instance.FuelTypeChangedEvent += delegate{RefreshGridParameters();};

            // Set up databinds of parameters in rows
            RefreshGridParameters();

            // Initialize flex input
            ParseUtility.PutDoubleArrayIntoTextBox(tbRadiativeHeatFluxPointsX,
                StateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.radiative_heat_flux_point:x")
                    .GetValue(DistanceUnit.Meter));
            ParseUtility.PutDoubleArrayIntoTextBox(tbRadiativeHeatFluxPointsY,
                StateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.radiative_heat_flux_point:y")
                    .GetValue(DistanceUnit.Meter));
            ParseUtility.PutDoubleArrayIntoTextBox(tbRadiativeHeatFluxPointsZ,
                StateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.radiative_heat_flux_point:z")
                    .GetValue(DistanceUnit.Meter));
            ParseUtility.PutDoubleArrayIntoTextBox(tbContourLevels,
                StateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.contour_levels")
                    .GetValue(UnitlessUnit.Unitless));

            _mIgnoreXyzChangeEvent = false;
        }

        /// <summary>
        /// Change which parameters are displayed based on fuel selection
        /// </summary>
        private void RefreshGridParameters()
        {
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
                new ParameterWrapper("relativeHumidity", "Relative humidity", UnitlessUnit.Unitless,
                    StockConverters.UnitlessConverter),
                new ParameterWrapper("releaseAngle", "Release angle", AngleUnit.Radians,
                    StockConverters.AngleConverter),
                new ParameterWrapper("releaseHeight", "Leak height from floor", DistanceUnit.Meter,
                    StockConverters.DistanceConverter)
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
            }

            StaticGridHelperRoutines.InitInteractiveGrid(dgInput, formParams, false);
            dgInput.Columns[0].Width = 180;
            CheckFormValid();
        }

        private void CheckFormValid()
        {
            bool showWarning = false;
            string warningText = "";

            if (!StateContainer.ReleasePressureIsValid())
            {
                // if liquid, validate fuel pressure
                warningText = MessageContainer.LiquidReleasePressureInvalid;
                showWarning = true;
            }

            // Verify x,y,z inputs
            if (tbRadiativeHeatFluxPointsY.Text.Trim().Length > 0)
            {
                var numXElems = CountElements(tbRadiativeHeatFluxPointsX.Text);
                var numYElems = CountElements(tbRadiativeHeatFluxPointsY.Text);
                var numZElems = CountElements(tbRadiativeHeatFluxPointsZ.Text);
                if (!(numZElems == numYElems && numZElems == numXElems))
                {
                    warningText = "X, Y, Z flux arrays must be the same size";
                    showWarning = true;
                }
                lblXElemCount.Text = numXElems + " elements";
                lblYElemCount.Text = numYElems + " elements";
                lblZElementCount.Text = numZElems + " elements";
            }

            inputWarning.Text = warningText;
            inputWarning.Visible = showWarning;
            executeButton.Enabled = !showWarning;
        }

        private void Execute()
        {
            // Blanket try block to help catch deployed issue Brian encountered
            Trace.TraceInformation("Gathering parameters for radiative flux analysis...");
            var ambTemp = StateContainer.GetNdValue("ambientTemperature", TempUnit.Kelvin);
            var ambPressure = StateContainer.GetNdValue("ambientPressure", PressureUnit.Pa);
            var h2Temp = StateContainer.GetNdValue("fluidTemperature", TempUnit.Kelvin);
            var h2Pressure = StateContainer.GetNdValue("fluidPressure", PressureUnit.Pa);
            var orificeDiam = StateContainer.GetNdValue("orificeDiameter", DistanceUnit.Meter);
            var leakHeight = StateContainer.GetNdValue("releaseHeight", DistanceUnit.Meter);

            _radHeatFluxX =
                StateContainer.GetNdValueList("FlameWrapper.radiative_heat_flux_point:x", DistanceUnit.Meter);
            _radHeatFluxY =
                StateContainer.GetNdValueList("FlameWrapper.radiative_heat_flux_point:y", DistanceUnit.Meter);
            _radHeatFluxZ =
                StateContainer.GetNdValueList("FlameWrapper.radiative_heat_flux_point:z", DistanceUnit.Meter);
            var contourLevels =
                StateContainer.GetNdValueList("FlameWrapper.contour_levels", UnitlessUnit.Unitless);
            var relativeHumidity = StateContainer.GetNdValue("relativeHumidity", UnitlessUnit.Unitless);
            var notionalNozzleModel = StateContainer.GetValue<NozzleModel>("NozzleModel");
            var releaseAngle = StateContainer.GetNdValue("releaseAngle", AngleUnit.Radians);
            var radiativeSourceModel =
                StateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel");

            Trace.TraceInformation("Creating PhysicsInterface for python call");
            var physInt = new PhysicsInterface();

            _analysisStatus = physInt.AnalyzeRadiativeHeatFlux(
                ambTemp, ambPressure, h2Temp, h2Pressure, orificeDiam, leakHeight, releaseAngle,
                notionalNozzleModel, _radHeatFluxX, _radHeatFluxY, _radHeatFluxZ, relativeHumidity,
                radiativeSourceModel, contourLevels,
                out _statusMsg, out _warningMsg, out _fluxData, out _fluxPlotFilepath, out _tempPlotFilepath);
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
            else if (_fluxData.Length == 0)
            {
                MessageBox.Show("Analysis yielded no data.");
                spinnerPictureBox.Hide();
                executeButton.Enabled = true;
            }
            else
            {
                pbPlotIsoOutput.Load(_fluxPlotFilepath);
                pbTPlot.Load(_tempPlotFilepath);

                dgResult.Rows.Clear();
                for (var index = 0; index < _fluxData.Length; index++)
                {
                    var values = new object[4];
                    values[0] = _radHeatFluxX[index].ToString("F4");
                    values[1] = _radHeatFluxY[index].ToString("F4");
                    values[2] = _radHeatFluxZ[index].ToString("F4");
                    values[3] = _fluxData[index].ToString("F4");
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
                    ExtractFloatArrayFromDelimitedString(tbRadiativeHeatFluxPointsX.Text, ',');
                var yValues =
                    ExtractFloatArrayFromDelimitedString(tbRadiativeHeatFluxPointsY.Text, ',');
                var zValues =
                    ExtractFloatArrayFromDelimitedString(tbRadiativeHeatFluxPointsZ.Text, ',');
                if (xValues.Length == yValues.Length && yValues.Length == zValues.Length && zValues.Length > 0)
                {
                    StateContainer.SetValue("FlameWrapper.radiative_heat_flux_point:x",
                        new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, xValues));
                    StateContainer.SetValue("FlameWrapper.radiative_heat_flux_point:y",
                        new ConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, yValues));
                    StateContainer.SetValue("FlameWrapper.radiative_heat_flux_point:z",
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
            if (pbPlotIsoOutput.Image != null)
            {
                pbPlotIsoOutput.Image.Dispose();
                pbPlotIsoOutput.Image = null;
            }

            if (pbTPlot.Image != null)
            {
                pbTPlot.Image.Dispose();
                pbTPlot.Image = null;
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

        private void tbRadiativeHeatFluxPointsX_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }
        private void tbRadiativeHeatFluxPointsY_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }
        private void tbRadiativeHeatFluxPointsZ_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
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


        private void tbContourLevels_TextChanged(object sender, EventArgs e)
        {
            if (!_mIgnoreXyzChangeEvent)
            {
                var contourLevels = ExtractFloatArrayFromDelimitedString(tbContourLevels.Text, ',');
                StateContainer.Instance.Parameters["FlameWrapper.contour_levels"] =
                    new ConvertibleValue(StockConverters.UnitlessConverter, UnitlessUnit.Unitless, contourLevels,
                        0.0);
            }
        }

        private void notionalNozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string modelName = notionalNozzleSelector.SelectedItem.ToString();
            StateContainer.SetValue("NozzleModel", NozzleModel.ParseNozzleModelName(modelName));
        }

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var phase = fuelPhaseSelector.SelectedItem;
            StateContainer.SetValue("ReleaseFluidPhase", phase);
            RefreshGridParameters();
        }

    }
}