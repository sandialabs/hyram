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
            // Add gas or liquid params
            if (StateContainer.FuelTypeIsGaseous())
            {
                formParams.Add("fluidPressure",
                    new ParameterWrapper("fluidPressure", "Fluid pressure (absolute)", PressureUnit.Pa,
                        StockConverters.PressureConverter));
                if (FluidPhase.DisplayTemperature())
                {
                    formParams.Add("fluidTemperature",
                        new ParameterWrapper("fluidTemperature", "Fluid temperature", TempUnit.Kelvin,
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
                out _statusMsg, out _warningMsg, out _resultImageFilepath);
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
            var phase = fuelPhaseSelector.SelectedItem;
            StateContainer.SetValue("ReleaseFluidPhase", phase);
            RefreshGridParameters();
        }
    }
}