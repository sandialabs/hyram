/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class PoolingForm : UserControl
    {
        private StateContainer _state = State.Data;

        private string _plotFilename;
        private bool _analysisStatus;
        private string _statusMsg;
        private string _warningMsg;

        public PoolingForm()
        {
            InitializeComponent();
        }

        private void PoolingForm_Load(object sender, EventArgs e)
        {
            spinner.Hide();
            // right-click to save images
            var picMenu = new ContextMenuStrip();
            {
                var item = new ToolStripMenuItem {Text = "Save As..."};
                item.MouseUp += UiHelpers.SaveImageToolStripMenuItem_Click;
                picMenu.Items.Add(item);
            }
            OutputPlot.ContextMenuStrip = picMenu;

            CheckFormValid();
            RefreshForm();

        }
        private void RefreshForm()
        {
            AutoSetLimits.Checked = _state.CryoAutoLimits;

            InputGrid.Rows.Clear();
            var formParams = ParameterInput.GetParameterInputList(new[]
            {
                _state.SurfaceThermCond, _state.SurfaceThermDiff, _state.SurfaceTemp,
                _state.PoolMassFlow, _state.InflowRadius, _state.SimTime
            });

            if (!_state.CryoAutoLimits)
            {
                formParams.AddRange(ParameterInput.GetParameterInputList(new[]
                {
                    _state.CryoXMin, _state.CryoXMax, _state.CryoYMin, _state.CryoYMax
                }));
            }

            GridHelpers.InitParameterGrid(InputGrid, formParams, false);
            InputGrid.Columns[0].Width = 210;

            CheckFormValid();
        }

        private void CheckFormValid()
        {
            bool showWarning = false;
            string warningText = "";

            inputWarning.Text = warningText;
            inputWarning.Visible = showWarning;
            SubmitBtn.Enabled = !showWarning;
        }

        private void Execute()
        {
            var physInt = new PhysicsInterface();
            _analysisStatus = physInt.CreatePoolingPlot(out _statusMsg, out _warningMsg, out _plotFilename);
        }

        private void DisplayResults()
        {
            spinner.Hide();
            SubmitBtn.Enabled = true;

            if (!_analysisStatus)
            {
                MessageBox.Show(_statusMsg);
                return;
            }
            resultTipLabel.Hide();
            OutputPlot.Load(_plotFilename);

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
            spinner.Show();
            SubmitBtn.Enabled = false;
            OutputPlot.Image?.Dispose();
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void AutoSetLimits_CheckedChanged(object sender, EventArgs e)
        {
            _state.CryoAutoLimits = AutoSetLimits.Checked;
            RefreshForm();
        }

        private void InputGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(InputGrid, sender, e);
        }
    }
}
