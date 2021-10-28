/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    public partial class JetFlameTemperaturePlotForm : UserControl
    {
        private string _statusMsg;
        private string _warningMsg;
        private bool _analysisStatus;
        private string _resultImageFilepath;
        private float _massFlow;
        private float _srad;

        public JetFlameTemperaturePlotForm()
        {
            InitializeComponent();
        }

        private void PhysJetTempPlotForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            outputWarning.Hide();

            notionalNozzleSelector.DataSource = StateContainer.Instance.NozzleModels;
            notionalNozzleSelector.SelectedItem = StateContainer.GetValue<NozzleModel>("NozzleModel");

            // Watch custom event for when fuel type selection changes, and updated displayed params to match
            StateContainer.Instance.FuelTypeChangedEvent += delegate{RefreshGridParameters();};
            fuelPhaseSelector.DataSource = StateContainer.Instance.FluidPhases;
            fuelPhaseSelector.SelectedItem = StateContainer.Instance.GetFluidPhase();

            RefreshGridParameters();
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
                new ParameterWrapper("releaseHeight", "Leak height from floor (y0)",
                    DistanceUnit.Meter, StockConverters.DistanceConverter),
                new ParameterWrapper("releaseAngle", "Release angle", AngleUnit.Degrees,
                    StockConverters.AngleConverter)
            });
            formParams.Add("fluidPressure",
                new ParameterWrapper("fluidPressure", "Fluid pressure (absolute)", PressureUnit.Pa,
                    StockConverters.PressureConverter));
            if (FluidPhase.DisplayTemperature())
            {
                formParams.Add("fluidTemperature",
                    new ParameterWrapper("fluidTemperature", "Fluid temperature", TempUnit.Kelvin,
                        StockConverters.TemperatureConverter));
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
                warningText = MessageContainer.GetAlertMessageReleasePressureInvalid();
                showWarning = true;
            }

            inputWarning.Text = warningText;
            inputWarning.Visible = showWarning;
            executeButton.Enabled = !showWarning;
        }

        private void Execute()
        {
            var ambTemp = StateContainer.Instance.GetStateDefinedValueObject("ambientTemperature")
                .GetValue(TempUnit.Kelvin)[0];
            var ambPres = StateContainer.Instance.GetStateDefinedValueObject("ambientPressure")
                .GetValue(PressureUnit.Pa)[0];
            var h2Temp = StateContainer.Instance.GetStateDefinedValueObject("fluidTemperature")
                .GetValue(TempUnit.Kelvin)[0];
            var h2Pres = StateContainer.Instance.GetStateDefinedValueObject("fluidPressure")
                .GetValue(PressureUnit.Pa)[0];
            var orificeDiam = StateContainer.Instance.GetStateDefinedValueObject("orificeDiameter")
                .GetValue(DistanceUnit.Meter)[0];
            var y0 = StateContainer.Instance.GetStateDefinedValueObject("releaseHeight")
                .GetValue(DistanceUnit.Meter)[0];
            var releaseAngle = StateContainer.Instance.GetStateDefinedValueObject("releaseAngle")
                .GetValue(AngleUnit.Radians)[0];
            var nozzleModel = StateContainer.GetValue<NozzleModel>("NozzleModel");

            var physInt = new PhysicsInterface();
            _analysisStatus = physInt.CreateFlameTemperaturePlot(ambTemp, ambPres, h2Temp, h2Pres, orificeDiam, y0,
                releaseAngle, nozzleModel.GetKey(),
                out _statusMsg, out _warningMsg, out _resultImageFilepath, out _massFlow, out _srad);
        }

        private void DisplayResults()
        {
            spinnerPictureBox.Hide();
            executeButton.Enabled = true;
            if (!_analysisStatus)
            {
                MessageBox.Show(_statusMsg);
            }
            else
            {
                outputPictureBox.Load(_resultImageFilepath);
                outputMassFlowRate.Text = _massFlow.ToString("E3");
                outputSrad.Text = _srad.ToString("E3");
                tcIO.SelectedTab = outputTab;

                if (_warningMsg.Length != 0)
                {
                    outputWarning.Text = _warningMsg;
                    outputWarning.Show();
                }
            }
        }

        private async void executeButton_Click(object sender, EventArgs e)
        {
            spinnerPictureBox.Show();
            outputWarning.Hide();
            executeButton.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void dgInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
            CheckFormValid();
        }

        private void notionalNozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var modelName = notionalNozzleSelector.SelectedItem.ToString();
            StateContainer.SetValue("NozzleModel", NozzleModel.ParseNozzleModelName(modelName));
        }

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetReleasePhase((FluidPhase)fuelPhaseSelector.SelectedItem);
            RefreshGridParameters();
        }
    }
}