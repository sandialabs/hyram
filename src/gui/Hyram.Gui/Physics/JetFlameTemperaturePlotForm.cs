/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class JetFlameTemperaturePlotForm : UserControl
    {
        private StateContainer _state = State.Data;

        private string _statusMsg;
        private string _warningMsg;
        private bool _analysisStatus;
        private string _resultImageFilepath;
        private float _massFlow;
        private float _srad;
        private float _flameLength;
        private float _radiantFrac;
        private bool _mIgnoreXyzChangeEvent = true;

        public JetFlameTemperaturePlotForm()
        {
            InitializeComponent();
        }

        private void PhysJetTempPlotForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();

            var picMenu = new ContextMenuStrip();
            {
                var item = new ToolStripMenuItem {Text = "Save As..."};
                item.MouseUp += UiHelpers.SaveImageToolStripMenuItem_Click;
                picMenu.Items.Add(item);
            }
            OutputPlot.ContextMenuStrip = picMenu;

            RefreshForm();
            ParseUtility.PutDoubleArrayIntoTextBox(ContourInput, _state.TempContourLevels);

            _mIgnoreXyzChangeEvent = false;
        }

        /// <summary>
        /// Change which parameters are displayed based on fuel selection
        /// </summary>
        private void RefreshForm()
        {
            AutoSetLimits.Checked = _state.FlameTempAutoLimits;

            if (!string.IsNullOrEmpty(_state.FlamePlotTitle))
            {
                PlotTitleInput.Text = _state.FlamePlotTitle;
            }

            InputGrid.Rows.Clear();
            var formParams = ParameterInput.GetParameterInputList(new[] {
                _state.OrificeDiameter,
                _state.ReleaseAngle,
            });

            if (!_state.FlameTempAutoLimits)
            {
                formParams.AddRange(ParameterInput.GetParameterInputList(new[]
                    {
                        _state.FlameXMin, _state.FlameXMax,
                        _state.FlameYMin, _state.FlameYMax,
                    }
                ));
            }

            GridHelpers.InitParameterGrid(InputGrid, formParams, false);
            InputGrid.Columns[0].Width = 200;
            CheckFormValid();
        }

        private void CheckFormValid()
        {
            bool showWarning = false;
            string warningText = "";

            if (!_state.ReleasePressureIsValid())
            {
                // if liquid, validate fuel pressure
                warningText = Notifications.ReleasePressureInvalid();
                showWarning = true;
            }

            if (_state.FuelFlowUnchoked())
            {
                MassFlowInput.Enabled = true;
                MassFlowInput.Text = _state.FluidMassFlow.ToString();
            }
            else
            {
                MassFlowInput.Enabled = false;
                MassFlowInput.Text = "";
            }


            inputWarning.Text = warningText;
            inputWarning.Visible = showWarning;
            SubmitBtn.Enabled = !showWarning;
        }

        private void Execute()
        {
            var physInt = new PhysicsInterface();
            _analysisStatus = physInt.CreateFlameTemperaturePlot(out _statusMsg, out _warningMsg, out _resultImageFilepath,
                                                                 out _massFlow, out _srad, out _flameLength, out _radiantFrac);
        }

        private void DisplayResults()
        {
            spinnerPictureBox.Hide();
            SubmitBtn.Enabled = true;
            if (!_analysisStatus)
            {
                MessageBox.Show(_statusMsg);
            }
            else
            {
                resultTipLabel.Hide();
                OutputPlot.Load(_resultImageFilepath);
                outputMassFlowRate.Text = _massFlow.ToString("E3");
                outputSrad.Text = _srad.ToString("E3");
                outputFlameLength.Text = _flameLength.ToString("F3");
                outputRadiantFrac.Text = _radiantFrac.ToString("F3");

                if (_warningMsg.Length != 0)
                {
                    inputWarning.Text = _warningMsg;
                    inputWarning.Show();
                }
            }
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            spinnerPictureBox.Show();
            OutputPlot.Image?.Dispose();
            SubmitBtn.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void InputGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridHelpers.ChangeParameterValue((DataGridView) sender, e);
            CheckFormValid();
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

                _state.TempContourLevels = contours.ToArray();
            }

        }

        private void PlotTitleInput_TextChanged(object sender, EventArgs e)
        {
            _state.FlamePlotTitle = PlotTitleInput.Text ?? "";
        }

        private void AutoSetLimits_CheckedChanged(object sender, EventArgs e)
        {
            _state.FlameTempAutoLimits = AutoSetLimits.Checked;
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