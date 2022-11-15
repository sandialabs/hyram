/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    public partial class PlumeForm : UserControl
    {
        private StateContainer _state = State.Data;

        private string _plotFilename;
        private bool _analysisStatus;
        private string _statusMsg;
        private string _warningMsg;
        private double _massFlow;

        public PlumeForm()
        {
            InitializeComponent();
        }

        ~PlumeForm()
        {
            _state.FuelTypeChangedEvent -= OnFuelChange;
        }

        private void PlumeForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            CheckFormValid();

            // Populate notional nozzle model combo box. Manual to exclude Harstad
            NozzleSelector.DataSource = _state.NozzleModels;
            NozzleSelector.SelectedItem = _state.Nozzle;

            PhaseSelector.DataSource = _state.Phases;
            PhaseSelector.SelectedItem = _state.Phase;

            _state.FuelTypeChangedEvent += OnFuelChange;

            RefreshForm();
        }

        private void OnFuelChange(object o, EventArgs e)
        {
            RefreshForm();
        }

        private void RefreshForm()
        {
            NozzleSelector.DataSource = _state.NozzleModels;
            NozzleSelector.SelectedItem = _state.Nozzle;
            PhaseSelector.SelectedItem = _state.Phase;

            if (!string.IsNullOrEmpty(_state.PlumePlotTitle))
            {
                PlotTitleInput.Text = _state.PlumePlotTitle;
            }

            // match fuel LFL if not blend or null before updating grid.
            var fuel = _state.GetActiveFuel();
            if (fuel.Lfl != null)
            {
                _state.PlumeContours.SetValue((double)fuel.Lfl);
            }

            InputGrid.Rows.Clear();
            var formParams = ParameterInput.GetParameterInputList(new[]
            {
                _state.AmbientPressure, _state.AmbientTemperature,
                _state.OrificeDiameter, _state.OrificeDischargeCoefficient,
                _state.PlumeReleaseAngle, _state.FluidPressure,

                _state.PlumeXMin, _state.PlumeXMax,
                _state.PlumeYMin, _state.PlumeYMax,
                _state.PlumeContours,
                _state.PlumeVMin, _state.PlumeVMax,
            });

            if (_state.DisplayTemperature())
            {
                formParams.Insert(6, new ParameterInput(_state.FluidTemperature));
            }

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

            if (_state.FuelFlowUnchoked())
            {
                MassFlowInput.Visible = true;
                massFlowLabel.Visible = true;
                MassFlowInput.Text = _state.FluidMassFlow.ToString();
            }
            else
            {
                MassFlowInput.Visible = false;
                massFlowLabel.Visible = false;
                MassFlowInput.Text = "";
            }

            inputWarning.Text = warningText;
            inputWarning.Visible = showWarning;
            SubmitBtn.Enabled = !showWarning;
        }

        private void Execute()
        {
            var physInt = new PhysicsInterface();
            _analysisStatus = physInt.CreatePlumePlot(out _statusMsg, out _warningMsg, out _plotFilename, out _massFlow);
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
                OutputPictureBox.Load(_plotFilename);
                mainTabControl.SelectedTab = outputTab;
                outputMassFlowRate.Text = _massFlow.ToString("E3");

                if (_warningMsg.Length != 0)
                {
                    outputWarning.Text = _warningMsg;
                    outputWarning.Show();
                }
            }
        }

        private void InputGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridHelpers.ChangeParameterValue((DataGridView) sender, e, 1, 2);
            CheckFormValid();
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            spinnerPictureBox.Show();
            outputWarning.Hide();
            SubmitBtn.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void PlotTitleInput_TextChanged(object sender, EventArgs e)
        {
            _state.PlumePlotTitle = PlotTitleInput.Text ?? "";
        }

        private void NozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Nozzle = (ModelPair)NozzleSelector.SelectedItem;
        }

        private void PhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Phase = (ModelPair)PhaseSelector.SelectedItem;
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
    }
}