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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class JetFlameHeatAnalysisForm : UserControl
    {
        private StateContainer _state = State.Data;
        // Results
        private bool _analysisStatus;
        private string _warningMsg;
        private string _statusMsg;
        private double[] _fluxData;
        private string _fluxPlotFilepath;
        private float _massFlow;
        private float _srad;
        private float _flameLength;
        private float _radiantFrac;
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

            // right-click to save images
            var picMenu = new ContextMenuStrip();
            {
                var item = new ToolStripMenuItem {Text = "Save As..."};
                item.MouseUp += UiHelpers.SaveImageToolStripMenuItem_Click;
                picMenu.Items.Add(item);
            }
            FluxPlot.ContextMenuStrip = picMenu;
            TempPlot.ContextMenuStrip = picMenu;

            RefreshForm();

            ParseUtility.PutDoubleArrayIntoTextBox(LocXInput, _state.RadiativeFluxX);
            ParseUtility.PutDoubleArrayIntoTextBox(LocYInput, _state.RadiativeFluxY);
            ParseUtility.PutDoubleArrayIntoTextBox(LocZInput, _state.RadiativeFluxZ);
            ParseUtility.PutDoubleArrayIntoTextBox(ContourInput, _state.FlameContourLevels);

            _mIgnoreXyzChangeEvent = false;
        }

        /// <summary>
        /// Change which parameters are displayed based on fuel selection
        /// </summary>
        private void RefreshForm()
        {
            AutoSetLimits.Checked = _state.FlameFluxAutoLimits;

            InputGrid.Rows.Clear();
            var formParams = ParameterInput.GetParameterInputList(new[]
            {
                _state.OrificeDiameter,
                _state.RelativeHumidity,
                _state.ReleaseAngle,
            });
            if (!_state.FlameFluxAutoLimits)
            {
                formParams.AddRange(ParameterInput.GetParameterInputList(new[]
                {
                    _state.FluxXMin, _state.FluxXMax,
                    _state.FluxYMin, _state.FluxYMax,
                    _state.FluxZMin, _state.FluxZMax,

                    _state.FlameXMin, _state.FlameXMax,
                    _state.FlameYMin, _state.FlameYMax,
                }));
            }

            GridHelpers.InitParameterGrid(InputGrid, formParams, false);
            InputGrid.Columns[0].Width = 180;

            CheckFormValid();
        }

        private void CheckFormValid()
        {
            bool showWarning = false;
            string warningText = "";

            if (_state.FuelFlowUnchoked())
            {
                MassFlowInput.Enabled = true;
//                massFlowLabel.Visible = true;
                MassFlowInput.Text = _state.FluidMassFlow.ToString();
            }
            else
            {
                MassFlowInput.Enabled = false;
//                massFlowLabel.Visible = false;
                MassFlowInput.Text = "";
            }

            // Verify x,y,z inputs
            if (LocYInput.Text.Trim().Length > 0)
            {
                var numXElems = CountElements(LocXInput.Text);
                var numYElems = CountElements(LocYInput.Text);
                var numZElems = CountElements(LocZInput.Text);
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
            SubmitBtn.Enabled = !showWarning;
        }

        private void Execute()
        {
            _radHeatFluxX = _state.RadiativeFluxX;
            _radHeatFluxY = _state.RadiativeFluxY;
            _radHeatFluxZ = _state.RadiativeFluxZ;
            var physInt = new PhysicsInterface();

            _analysisStatus = physInt.AnalyzeRadiativeHeatFlux(out _statusMsg, out _warningMsg, out _fluxData,
                                                                out _fluxPlotFilepath, out _tempPlotFilepath,
                                                                out _massFlow, out _srad, out _flameLength,
                                                                out _radiantFrac);
        }

        private void DisplayResults()
        {
            if (!_analysisStatus)
            {
                MessageBox.Show(_statusMsg);
                spinnerPictureBox.Hide();
                SubmitBtn.Enabled = true;
            }
            else if (_fluxData.Length == 0)
            {
                MessageBox.Show("Analysis yielded no data.");
                spinnerPictureBox.Hide();
                SubmitBtn.Enabled = true;
            }
            else
            {

                FluxPlot.Load(_fluxPlotFilepath);
                TempPlot.Load(_tempPlotFilepath);

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

                outputMassFlowRate.Text = _massFlow.ToString("E3");
                outputSrad.Text = _srad.ToString("E3");
                outputFlameLength.Text = _flameLength.ToString("F3");
                outputRadiantFrac.Text = _radiantFrac.ToString("F3");

                spinnerPictureBox.Hide();
                SubmitBtn.Enabled = true;
                tcIO.SelectTab(outputTab);

            }
        }

        // Saves x,y,z position data from textbox inputs
        private void ExtractAndSaveXyzValues()
        {
            if (!_mIgnoreXyzChangeEvent)
            {
                var xValues = ParseUtility.GetArrayFromString(LocXInput.Text, ',');
                var yValues = ParseUtility.GetArrayFromString(LocYInput.Text, ',');
                var zValues = ParseUtility.GetArrayFromString(LocZInput.Text, ',');
                if (xValues.Length == yValues.Length && yValues.Length == zValues.Length && zValues.Length > 0)
                {
                    _state.RadiativeFluxX = xValues;
                    _state.RadiativeFluxY = yValues;
                    _state.RadiativeFluxZ = zValues;
                }
            }
            CheckFormValid();
        }

        private int CountElements(string textToParse)
        {
            var values = textToParse.Trim().Split(',');
            return values.Length;
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            FluxPlot.Image?.Dispose();
            TempPlot.Image?.Dispose();

            dgResult.Rows.Clear();
            outputWarning.Hide();
            spinnerPictureBox.Show();
            SubmitBtn.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void InputGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridHelpers.ChangeParameterValue((DataGridView) sender, e);
            CheckFormValid();
        }

        private void LocXInput_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }
        private void LocYInput_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }
        private void LocZInput_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
        }

        private void CopyBtn_Click(object sender, EventArgs e)
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

        private void ContourInput_TextChanged(object sender, EventArgs e)
        {
            if (!_mIgnoreXyzChangeEvent)
            {
                var contours = new List<double>();

                string contourText = ContourInput.Text;
                Regex.Replace(contourText, @"\s+", "");  // trim whitespace
                if (contourText != "")
                {
                    contours = new List<double>(ParseUtility.GetArrayFromString(ContourInput.Text, ','));
                }

                _state.FlameContourLevels = contours.ToArray();
            }
        }

        private void AutoSetLimits_CheckedChanged(object sender, EventArgs e)
        {
            _state.FlameFluxAutoLimits = AutoSetLimits.Checked;
            RefreshForm();
        }

        private void MassFlowInput_TextChanged(object sender, EventArgs e)
        {
            if (double.TryParse(MassFlowInput.Text, out double result))
            {
                _state.FluidMassFlow = result;
            }
            else
            {
                MassFlowInput.Text = _state.FluidMassFlow.ToString();
            }
        }

        private void InputGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(InputGrid, sender, e);
        }
    }
}