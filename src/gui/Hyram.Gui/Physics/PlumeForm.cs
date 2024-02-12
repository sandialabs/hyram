/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    public partial class PlumeForm : UserControl
    {
        private StateContainer _state = State.Data;

        private string _plotFilename;
        private bool _analysisStatus;
        private string _statusMsg;
        private string _warningMsg;
        private double _massFlow;
        private List<double> _orderedContours;
        private List<double> _streamlineDists;
        // one elem for each contour: {x: [2 doubles], y: [2 doubles]}
        private List<List<double>> _contourDists;

        public PlumeForm()
        {
            InitializeComponent();

            // right-click to save images
            var picMenu = new ContextMenuStrip();
            {
                var item = new ToolStripMenuItem {Text = "Save As..."};
                item.MouseUp += UiHelpers.SaveImageToolStripMenuItem_Click;
                picMenu.Items.Add(item);
            }
            OutputPlot.ContextMenuStrip = picMenu;
        }

        private void PlumeForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            CheckFormValid();

            var toolTip1 = new ToolTip
            {
                AutoPopDelay = 5000, InitialDelay = 1000, ReshowDelay = 500, ShowAlways = true
            };
            toolTip1.SetToolTip(ContourLabel, "Separate multiple contour values with spaces");
            toolTip1.SetToolTip(ContourInput, "Separate multiple contour values with spaces");

            RefreshForm();
        }

        private void RefreshForm()
        {
            if (!string.IsNullOrEmpty(_state.PlumePlotTitle))
            {
                PlotTitleInput.Text = _state.PlumePlotTitle;
            }

            // match fuel LFL if not blend or null before updating grid.
            var fuel = _state.GetActiveFuel();
            if (fuel.Lfl != null && _state.PlumeContourLevels.Length == 0)
            {
                _state.PlumeContourLevels = new[] {(double)fuel.Lfl};
            }
            ParseUtility.PutDoubleArrayIntoTextBox(ContourInput, _state.PlumeContourLevels);

            InputGrid.Rows.Clear();
            var formParams = ParameterInput.GetParameterInputList(new[]
            {
                _state.OrificeDiameter,
                _state.PlumeReleaseAngle,
            });

            AutoSetLimits.Checked = _state.PlumePlotAutoLimits;
            if (!_state.PlumePlotAutoLimits)
            {
                formParams.AddRange(ParameterInput.GetParameterInputList(new []
                {
                    _state.PlumeXMin, _state.PlumeXMax,
                    _state.PlumeYMin, _state.PlumeYMax,
                    _state.PlumeVMin, _state.PlumeVMax,
                }));
            }

            GridHelpers.InitParameterGrid(InputGrid, formParams, false);

            CheckFormValid();
        }

        private void CheckFormValid()
        {
            bool showWarning = false;
            string warningText = "";

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
            _analysisStatus = physInt.CreatePlumePlot(out _statusMsg, out _warningMsg, out _plotFilename, out _massFlow,
                out _orderedContours, out _streamlineDists, out _contourDists);
        }

        private void DisplayResults()
        {
            spinnerPictureBox.Hide();
            SubmitBtn.Enabled = true;
            resultTipLabel.Hide();

            if (!_analysisStatus)
            {
                MessageBox.Show(_statusMsg);
                return;
            }

            OutputPlot.Load(_plotFilename);
            outputMassFlowRate.Text = _massFlow.ToString("E3");

            contourGrid.Rows.Clear();
            if (_orderedContours.Count > 0)
            {
                contourGrid.Enabled = true;
            }
            else
            {
                contourGrid.Enabled = false;
            }

            for (int i = 0; i < _orderedContours.Count; i++)
            {
                string contour = _orderedContours[i].ToString();
                double streamline = _streamlineDists[i];
                List<double> xys = _contourDists[i];

                contourGrid.Rows.Add(contour, streamline, xys[0], xys[1], xys[2], xys[3]);
            }

            for (int i = 0; i < _contourDists.Count; i++)
            {
                var contourDict = _contourDists[i];
                double streamlineDist = _streamlineDists[i];

            }

            if (_warningMsg.Length != 0)
            {
                inputWarning.Text = _warningMsg;
                inputWarning.Show();
            }
        }

        private void InputGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridHelpers.ChangeParameterValue((DataGridView) sender, e);
            CheckFormValid();
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            spinnerPictureBox.Show();
            inputWarning.Hide();
            SubmitBtn.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void PlotTitleInput_TextChanged(object sender, EventArgs e)
        {
            _state.PlumePlotTitle = PlotTitleInput.Text ?? "";
        }

        private void MassFlowInput_TextChanged(object sender, EventArgs e)
        {
            FormHelpers.HandleNullableDoubleInputChange(MassFlowInput, sender, e, out _state.FluidMassFlow);
        }

        private void AutoSetLimits_CheckedChanged(object sender, EventArgs e)
        {
            _state.PlumePlotAutoLimits = AutoSetLimits.Checked;
            RefreshForm();
        }

        private void InputGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(InputGrid, sender, e);
        }

        private void ContourInput_TextChanged(object sender, EventArgs e)
        {
            var contours = new List<double>();

            string val = ContourInput.Text;
            // Regex.Replace(val, @"\s+", "");  // trim whitespace
            if (val != "")
            {
                contours = new List<double>(ParseUtility.GetArrayFromString(ContourInput.Text, ' '));
            }

            _state.PlumeContourLevels = contours.ToArray();
        }
    }
}