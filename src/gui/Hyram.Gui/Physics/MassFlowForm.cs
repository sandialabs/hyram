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
    public partial class MassFlowForm : UserControl
    {
        private readonly StateContainer _state = State.Data;
        private DistanceUnit _diamUnit = DistanceUnit.Meter;
        private PressureUnit _pUnit = PressureUnit.Pa;
        private PressureUnit _pAmbUnit = PressureUnit.Pa;
        private TempUnit _tUnit = TempUnit.Kelvin;
        private VolumeUnit _vUnit = VolumeUnit.Liter;
        private double _diam = double.NaN;
        private double _p = double.NaN;
        private double _pAmb = double.NaN;
        private double _t = double.NaN;
        private double _v = double.NaN;
        private double _dc = double.NaN;

        private string _statusMsg;
        private bool _status;
        private double? _outFlow;
        private double? _outTime;
        private string _outFilepath;

        public MassFlowForm()
        {
            InitializeComponent();
        }

        private void EtkMassFlowRateForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            warningMsg.Hide();
            PlotBox.Hide();
            warningMsg.BackColor = _state.AlertBackColors[(int)AlertLevel.AlertError];
            warningMsg.ForeColor = _state.AlertTextColors[(int)AlertLevel.AlertError];

            PhaseSelector.DataSource = _state.Phases;
            PhaseSelector.SelectedItem = _state.Phase;

            TemperatureUnitSelector.Converter = Converters.GetConverterByName("Temperature");
            TemperatureUnitSelector.SelectedItem = _tUnit;

            PressureUnitSelector.Converter = Converters.GetConverterByName("Pressure");
            PressureUnitSelector.SelectedItem = _pUnit;

            AmbPresUnitSelector.Converter = Converters.GetConverterByName("Pressure");
            AmbPresUnitSelector.SelectedItem = _pAmbUnit;

            TankVolumeUnitSelector.Converter = Converters.GetConverterByName("Volume");
            TankVolumeUnitSelector.SelectedItem = _vUnit;

            OrificeDiameterUnitSelector.Converter = Converters.GetConverterByName("Distance");
            OrificeDiameterUnitSelector.SelectedItem = _diamUnit;

            CheckFormValid();
        }

        private bool SteadyBlowdown
        {
            get => SteadySelector.Checked;
            set => SteadySelector.Checked = value;
        }

        /// <summary>
        /// Verify inputs and enable submission button.
        /// </summary>
        public void CheckFormValid()
        {
            bool formReady = true;
            warningMsg.Hide();
            string msg = "";

            if (_state.DisplayTemperature())
            {
                TemperatureInput.Enabled = true;
                TemperatureUnitSelector.Enabled = true;
            }
            else
            {
                TemperatureInput.Enabled = false;
                TemperatureUnitSelector.Enabled = false;
                TemperatureInput.Text = "";
            }

            if (_state.PhaseIsSaturated())
            {
                if (!(ParseUtility.IsParseableNumber(VolumeInput.Text) &&
                      ParseUtility.IsParseableNumber(PressureInput.Text) &&
                      ParseUtility.IsParseableNumber(AmbPresInput.Text) &&
                      ParseUtility.IsParseableNumber(OrificeDiameterInput.Text)))
                {
                    formReady = false;
                    msg = "Verify inputs are valid";
                }
            }
            else
            {
                if (!(ParseUtility.IsParseableNumber(VolumeInput.Text) &&
                      ParseUtility.IsParseableNumber(TemperatureInput.Text) &&
                      ParseUtility.IsParseableNumber(PressureInput.Text) &&
                      ParseUtility.IsParseableNumber(AmbPresInput.Text) &&
                      ParseUtility.IsParseableNumber(OrificeDiameterInput.Text)))
                {
                    msg = "Verify inputs are valid";
                    formReady = false;
                }
            }

            if (!SteadyBlowdown)
            {
                if (!(double.TryParse(VolumeInput.Text, out double v) && v > 0))
                {
                    formReady = false;
                    msg = "Volume must be > 0";
                }
            }

            if (double.TryParse(DcInput.Text, out double dcVal))
            {
                if (dcVal < 0.0 || dcVal > 1.0)
                {
                    formReady = false;
                    msg = "Discharge coefficient must be between 0 and 1";
                }
            }
            else
            {
                msg = "Discharge coefficient is invalid";
                formReady = false;
            }

            if (!string.IsNullOrEmpty(msg))
            {
                warningMsg.Text = msg;
                warningMsg.Show();
            }
            submitBtn.Enabled = formReady;
        }


        private void ReleaseTypeChanged(object sender, EventArgs e)
        {
            if (SteadyBlowdown)
            {
                VolumeInput.Text = "0.0";
                VolumeInput.Enabled = false;
            }
            else
            {
                VolumeInput.Enabled = true;
            }
            CheckFormValid();
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            spinnerPictureBox.Show();
            warningMsg.Hide();
            submitBtn.Enabled = false;
            PlotBox.Visible = false;
            PlotBox.Hide();
            MassFlowTextbox.Text = "";

            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void Execute()
        {
            var temp = TemperatureUnitSelector.ConvertValue(_tUnit, TempUnit.Kelvin, _t);
            var pressure = PressureUnitSelector.ConvertValue(_pUnit, PressureUnit.Pa, _p);
            var ambientP = AmbPresUnitSelector.ConvertValue(_pAmbUnit, PressureUnit.Pa, _pAmb);
            var tankVolume = TankVolumeUnitSelector.ConvertValue(_vUnit, VolumeUnit.CubicMeter, _v);
            var orificeDiam = OrificeDiameterUnitSelector.ConvertValue(_diamUnit, DistanceUnit.Meter, _diam);
//            double ambientP = _state.AmbientPressure.GetValue();
            var isSteady = SteadyBlowdown;
            var dischargeCoeff = (float)_dc;

            var physApi = new PhysicsInterface();
            _status = physApi.ComputeFlowRateOrTimeToEmpty(orificeDiam, temp, pressure,
                                                                tankVolume, isSteady, dischargeCoeff, ambientP,
                                                                out _statusMsg, out _outFlow,
                                                                out _outTime, out _outFilepath);
        }

        private void DisplayResults()
        {
            spinnerPictureBox.Hide();
            submitBtn.Enabled = true;

            if (!_status)
            {
                MessageBox.Show(_statusMsg);
            }
            else
            {
                if (SteadyBlowdown)
                {
                    outputLabel.Text = "Mass flow rate (kg/s)";
                    MassFlowTextbox.Text = _outFlow != null ? ((double)_outFlow).ToString("E3") : "Error";
                }
                else
                {
                    PlotBox.Show();
                    outputLabel.Text = "Time to empty (s)";
                    PlotBox.Load(_outFilepath);
                    PlotBox.Visible = true;
                    MassFlowTextbox.Text = _outTime != null ? ((double)_outTime).ToString("E3") : "Error";
                }
            }
        }

        private void TemperatureInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(TemperatureInput.Text, out _t);
            CheckFormValid();
        }

        private void OrificeDiameterUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (OrificeDiameterUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseDistanceUnit((string) OrificeDiameterUnitSelector.SelectedItem);
                _diam = OrificeDiameterUnitSelector.ConvertValue(_diamUnit, newUnit, _diam);
                if (!double.IsNaN(_diam)) OrificeDiameterInput.Text = "" + _diam;

                _diamUnit = newUnit;
                Settings.Default.MFROrificeDiamDistUnit = newUnit.ToString();
            }
        }

        private void PressureUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (PressureUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) PressureUnitSelector.SelectedItem);
                _p = PressureUnitSelector.ConvertValue(_pUnit, newUnit, _p);
                if (!double.IsNaN(_p))
                {
                    PressureInput.Text = _p.ToString();
                }

                _pUnit = newUnit;
                Settings.Default.MFRPressureUnit = newUnit.ToString();
            }
        }

        private void AmbPresUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (AmbPresUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) AmbPresUnitSelector.SelectedItem);
                _pAmb = AmbPresUnitSelector.ConvertValue(_pAmbUnit, newUnit, _pAmb);
                if (!double.IsNaN(_pAmb))
                {
                    AmbPresInput.Text = _pAmb.ToString();
                }

                _pAmbUnit = newUnit;
//                Settings.Default.MFRPressureUnit = newUnit.ToString();
            }

        }

        private void TankVolumeUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (TankVolumeUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseVolumeUnit((string) TankVolumeUnitSelector.SelectedItem);

                _v = TankVolumeUnitSelector.ConvertValue(_vUnit, newUnit, _v);
                if (!double.IsNaN(_v))
                {
                    VolumeInput.Text = _v.ToString();
                }

                _vUnit = newUnit;
                Settings.Default.MFRVolumeUnit = _vUnit.ToString();
            }
        }

        private void TemperatureUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (TemperatureUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) TemperatureUnitSelector.SelectedItem);
                _t = TemperatureUnitSelector.ConvertValue(_tUnit, newUnit, _t);
                if (!double.IsNaN(_t))
                {
                    TemperatureInput.Text = _t.ToString();
                }

                _tUnit = newUnit;
                Settings.Default.MFRTempUnit = _tUnit.ToString();
            }
        }

        private void OrificeDiameterInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(OrificeDiameterInput.Text, out _diam);
            CheckFormValid();
        }

        private void VolumeInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(VolumeInput.Text, out _v);
            CheckFormValid();
        }

        private void PressureInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(PressureInput.Text, out _p);
            CheckFormValid();
        }

        private void AmbPresInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(AmbPresInput.Text, out _pAmb);
            CheckFormValid();
        }

        private void DcInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(DcInput.Text, out _dc);
            CheckFormValid();
        }

        private void PhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Phase = (ModelPair) PhaseSelector.SelectedItem;
            // Deactivate temp input for saturated phases
            CheckFormValid();
        }

    }
}