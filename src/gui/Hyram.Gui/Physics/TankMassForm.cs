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
    public partial class TankMassForm : UserControl
    {
        private readonly StateContainer _state = State.Data;
        private string _statusMsg;
        private bool _status;
        private double _v = double.NaN;
        private double _t = double.NaN;
        private double _p = double.NaN;
        private double _m = double.NaN;
        private double? _output1;
        private double? _output2;
        private bool _changeSilently = false;

        private PressureUnit _pUnit = PressureUnit.Pa;
        private TempUnit _tUnit = TempUnit.Kelvin;
        private VolumeUnit _vUnit = VolumeUnit.Liter;
        private MassUnit _mUnit = MassUnit.Kilogram;

        public TankMassForm()
        {
            InitializeComponent();
        }

        private void TankMassForm_Load(object sender, EventArgs e)
        {
            _changeSilently = true;
            spinnerPictureBox.Hide();
            warningLabel.Hide();

            PhaseSelector.DataSource = _state.Phases;
            PhaseSelector.SelectedItem = _state.Phase;

            TempUnitSelector.Converter = Converters.GetConverterByName("Temperature");
            TempUnitSelector.SelectedItem = _tUnit;

            PresUnitSelector.Converter = Converters.GetConverterByName("Pressure");
            PresUnitSelector.SelectedItem = _pUnit;

            VolUnitSelector.Converter = Converters.GetConverterByName("Volume");
            VolUnitSelector.SelectedItem = _vUnit;

            MassUnitSelector.Converter = Converters.GetConverterByName("Mass");
            MassUnitSelector.SelectedItem = _mUnit;

            _changeSilently = false;
            RefreshInputs();
        }

        public void RefreshInputs()
        {
            if (_changeSilently)
            {
                return;
            }

            bool isSaturated = _state.PhaseIsSaturated();

            bool input1Valid = true;
            bool input2Valid = true;
            bool input3Valid = true;
            MassInput.Enabled = true;
            PresInput.Enabled = true;
            TempInput.Enabled = true;
            VolInput.Enabled = true;

            TempSelector.Enabled = true;
            if (isSaturated)
            {
                // can't select temp for output if saturated
                TempInput.Enabled = false;
                TempInput.Text = "";
                TempSelector.Checked = false;
                TempSelector.Enabled = false;

                if (TempSelector.Checked)
                {
                    PresSelector.Checked = true;
                }
            }
            else
            {
                TempInput.Enabled = true;
            }

            if (MassSelector.Checked)
            {
                MassInput.Enabled = false;
                MassInput.Text = "";
                input1Valid = isSaturated || ParseUtility.IsParseableNumber(TempInput.Text);
                input2Valid = ParseUtility.IsParseableNumber(PresInput.Text);
                input3Valid = ParseUtility.IsParseableNumber(VolInput.Text);

            }
            else if (PresSelector.Checked)
            {
                PresInput.Enabled = false;
                PresInput.Text = "";
                input1Valid = isSaturated || ParseUtility.IsParseableNumber(TempInput.Text);
                input2Valid = ParseUtility.IsParseableNumber(VolInput.Text);
                input3Valid = ParseUtility.IsParseableNumber(MassInput.Text);

            }
            else if (VolSelector.Checked)
            {
                VolInput.Enabled = false;
                VolInput.Text = "";
                input1Valid = isSaturated || ParseUtility.IsParseableNumber(TempInput.Text);
                input2Valid = ParseUtility.IsParseableNumber(PresInput.Text);
                input3Valid = ParseUtility.IsParseableNumber(MassInput.Text);

            }
            else  // temperature
            {
                TempInput.Enabled = false;
                TempInput.Text = "";
                input1Valid = ParseUtility.IsParseableNumber(PresInput.Text);
                input2Valid = ParseUtility.IsParseableNumber(VolInput.Text);
                input3Valid = ParseUtility.IsParseableNumber(MassInput.Text);
            }

            SubmitBtn.Enabled = input1Valid && input2Valid && input3Valid;
        }

        private void MassInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(MassInput.Text, out _m);
            RefreshInputs();
        }

        private void VolInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(VolInput.Text, out _v);
            RefreshInputs();
        }

        private void TempInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(TempInput.Text, out _t);
            RefreshInputs();
        }

        private void PresInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(PresInput.Text, out _p);
            RefreshInputs();
        }

        private void PhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Phase = (ModelPair)PhaseSelector.SelectedItem;
            RefreshInputs();
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            spinnerPictureBox.Show();
            warningLabel.Hide();
            SubmitBtn.Enabled = false;

            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void Execute()
        {
            double? temp = TempUnitSelector.ConvertValue(_tUnit, TempUnit.Kelvin, _t);
            double? pressure = PresUnitSelector.ConvertValue(_pUnit, PressureUnit.Pa, _p);
            double? volume = VolUnitSelector.ConvertValue(_vUnit, VolumeUnit.CubicMeter, _v);
            double? mass = MassUnitSelector.ConvertValue(_mUnit, MassUnit.Kilogram, _m);
            string phaseKey = _state.Phase.GetKey();

            if (MassSelector.Checked)
            {
                mass = null;
            }
            else if (PresSelector.Checked)
            {
                pressure = null;
            }
            else if (VolSelector.Checked)
            {
                volume = null;
            }
            else
            {
                temp = null;
            }

            if (_state.PhaseIsSaturated())
            {
                temp = null;
            }

            var physApi = new PhysicsInterface();
            _status = physApi.ComputeTankParameter(temp, pressure, volume, mass, phaseKey, out _statusMsg, out _output1, out _output2);
        }

        private void DisplayResults()
        {
            spinnerPictureBox.Hide();
            SubmitBtn.Enabled = true;

            if (!_status)
            {
                MessageBox.Show(_statusMsg);
            }
            else
            {
                if (!string.IsNullOrEmpty(_statusMsg))
                {
                    warningLabel.Text = _statusMsg;
                    warningLabel.Show();
                }

                _changeSilently = true;

                if (MassSelector.Checked)
                {
                    _m = MassUnitSelector.ConvertValue(MassUnit.Kilogram, _mUnit, (double) _output1);
                    MassInput.Text = _m.ToString("E3");
                }
                else if (PresSelector.Checked)
                {
                    _p = PresUnitSelector.ConvertValue(PressureUnit.Pa, _pUnit, (double) _output1);
                    PresInput.Text = _p.ToString("E3");
                }
                else if (VolSelector.Checked)
                {
                    _v = VolUnitSelector.ConvertValue(VolumeUnit.CubicMeter, _vUnit, (double) _output1);
                    VolInput.Text = _v.ToString("E3");
                }
                else
                {
                    _t = TempUnitSelector.ConvertValue(TempUnit.Kelvin, _tUnit, (double) _output1);
                    TempInput.Text = _t.ToString("F3");
                }

                if (_state.PhaseIsSaturated())
                {
                    _t = TempUnitSelector.ConvertValue(TempUnit.Kelvin, _tUnit, (double) _output2);
                    TempInput.Text = _t.ToString("F3");
                }

                _changeSilently = false;
            }

        }

        private void MassUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var newUnit = UnitParser.ParseMassUnit((string)MassUnitSelector.SelectedItem);
            _m = MassUnitSelector.ConvertValue(_mUnit, newUnit, _m);
            if (!double.IsNaN(_m))
            {
                MassInput.Text = _m.ToString();
            }
//            var valueInUserUnits = mUnitSelector.ConvertValue(MassUnit.Kilogram, _mUnit, _m);
//            mInput.Text = valueInUserUnits.ToString("E3");
            _mUnit = newUnit;
        }

        private void TempUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            var newUnit = UnitParser.ParseTempUnit((string) TempUnitSelector.SelectedItem);
            _t = TempUnitSelector.ConvertValue(_tUnit, newUnit, _t);
            if (!double.IsNaN(_t))
            {
                TempInput.Text = _t.ToString();
            }

            _tUnit = newUnit;
        }

        private void PresUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            var newUnit = UnitParser.ParsePressureUnit((string) PresUnitSelector.SelectedItem);
            _p = PresUnitSelector.ConvertValue(_pUnit, newUnit, _p);
            if (!double.IsNaN(_p))
            {
                PresInput.Text = _p.ToString();
            }

            _pUnit = newUnit;
        }

        private void VolUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (VolUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseVolumeUnit((string) VolUnitSelector.SelectedItem);

                _v = VolUnitSelector.ConvertValue(_vUnit, newUnit, _v);
                if (!double.IsNaN(_v))
                {
                    VolInput.Text = _v.ToString();
                }

                _vUnit = newUnit;
            }
        }

        private void Output_CheckedChanged(object sender, EventArgs e)
        {
            RefreshInputs();
        }
    }
}