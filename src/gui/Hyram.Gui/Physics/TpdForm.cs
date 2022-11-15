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
    public partial class TpdForm : UserControl
    {
        private readonly StateContainer _state = State.Data;
        private string _statusMsg;
        private bool _status;
        private double? _param1;
        private double? _param2;
        private double _t = double.NaN;
        private double _p = double.NaN;
        private double _d = double.NaN;
        private PressureUnit _pUnit = PressureUnit.Pa;
        private TempUnit _tUnit = TempUnit.Kelvin;
        private DensityUnit _dUnit = DensityUnit.KilogramPerCubicMeter;
        private bool _changeSilently = false;

        public TpdForm()
        {
            InitializeComponent();
        }

        private void TpdForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            warningLabel.Hide();

            PhaseSelector.DataSource = _state.Phases;
            PhaseSelector.SelectedItem = _state.Phase;

            TempUnitSelector.Converter = Converters.GetConverterByName("Temperature");
            TempUnitSelector.SelectedItem = _tUnit;

            PresUnitSelector.Converter = Converters.GetConverterByName("Pressure");
            PresUnitSelector.SelectedItem = _pUnit;

            DenUnitSelector.Converter = Converters.GetConverterByName("Density");
            DenUnitSelector.SelectedItem = _dUnit;

            RefreshInputs();
        }

        // Updates state of input fields based on parameter values and phase.
        public void RefreshInputs()
        {
            if (_changeSilently)
            {
                return;
            }

            // if saturated, required inputs are either pressure or density, not both
            var phase = _state.Phase;
            bool isSaturated = (phase != _state.GasDefault);

            bool input1Valid = true;
            bool input2Valid = true;
            DenInput.Enabled = true;
            PresInput.Enabled = true;
            TempInput.Enabled = true;

            if (isSaturated)
            {
                TempInput.Enabled = false;
                TempInput.Text = "";
            }

            if (DenSelector.Checked)
            {
                // If not saturated, verify that two other parameters are valid
                DenInput.Enabled = false;
                input1Valid = ParseUtility.IsParseableNumber(PresInput.Text);
                input2Valid = isSaturated || ParseUtility.IsParseableNumber(TempInput.Text);
            }
            else if (PresSelector.Checked)
            {
                PresInput.Enabled = false;
                input1Valid = ParseUtility.IsParseableNumber(DenInput.Text);
                input2Valid = isSaturated || ParseUtility.IsParseableNumber(TempInput.Text);
            }
            else
            {
                // compute temperature; check which other input is provided
                TempInput.Enabled = false;
                if (isSaturated)
                {
                    // only 1 other input needed
                    if (PresInput.Text.Length > 0)
                    {
                        input1Valid = ParseUtility.IsParseableNumber(PresInput.Text);
                        DenInput.Enabled = false;
                    }
                    else if (DenInput.Text.Length > 0)
                    {
                        input1Valid = ParseUtility.IsParseableNumber(DenInput.Text);
                        PresInput.Enabled = false;
                    }
                    else
                    {
                        input1Valid = false;
                    }
                }
                else
                {
                    input1Valid = ParseUtility.IsParseableNumber(DenInput.Text);
                    input2Valid = ParseUtility.IsParseableNumber(PresInput.Text);
                }
            }

            SubmitBtn.Enabled = input1Valid && input2Valid;
        }

        private void TempUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (TempUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) TempUnitSelector.SelectedItem);
                _t = TempUnitSelector.ConvertValue(_tUnit, newUnit, _t);

                if (!double.IsNaN(_t))
                {
                    TempInput.Text = _t.ToString();
                }

                _tUnit = newUnit;
            }
        }

        private void PresUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (PresUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) PresUnitSelector.SelectedItem);
                _p = PresUnitSelector.ConvertValue(_pUnit, newUnit, _p);

                if (!double.IsNaN(_p))
                {
                    PresInput.Text = _p.ToString();
                }

                _pUnit = newUnit;
            }
        }

        private void DenUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (DenUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseDensityUnit((string) DenUnitSelector.SelectedItem);
                _d = DenUnitSelector.ConvertValue(_dUnit, newUnit, _d);

                if (!double.IsNaN(_d))
                {
                    DenInput.Text = "" + _d;
                }

                _dUnit = newUnit;
            }
        }

        private void DenInput_TextChanged(object sender, EventArgs e)
        {
            double.TryParse(DenInput.Text, out _d);
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

        private void OutputCheckedChanged(object sender, EventArgs e)
        {
            RefreshInputs();
        }


        // Computes missing parameter(s). If saturated, second missing parameter will also be calculated.
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
            double? temp = null;
            double? pressure = null;
            double? density = null;
            var phase = _state.Phase;
            bool isSaturated = (phase != _state.GasDefault);

            _changeSilently = true;

            if (DenSelector.Checked)
            {
                pressure = GetPressure(_pUnit);
                temp = !isSaturated ? (double?)GetTemp(_tUnit) : null;
            }
            else if (PresSelector.Checked)
            {
                density = GetDensity(_dUnit);
                temp = !isSaturated ? (double?)GetTemp(_tUnit) : null;
            }
            else
            {
                // compute temperature; only 1 input needed if saturated
                if (isSaturated)
                {
                    if (ParseUtility.IsParseableNumber(PresInput.Text))
                    {
                        pressure = GetPressure(_pUnit);
                    }
                    else
                    {
                        density = GetDensity(_dUnit);
                    }
                }
                else
                {
                    pressure = GetPressure(_pUnit);
                    density = GetDensity(_dUnit);
                }
            }

            var physApi = new PhysicsInterface();
            _status = physApi.ComputeTpd(temp, pressure, density, phase.GetKey(),
                                             out _statusMsg, out _param1, out _param2);
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
                var phase = _state.Phase;
                bool isSaturated = (phase != _state.GasDefault);

                if (!string.IsNullOrEmpty(_statusMsg))
                {
                    warningLabel.Text = _statusMsg;
                    warningLabel.Show();
                }

                if (DenSelector.Checked)
                {
                    _d = DenUnitSelector.ConvertValue(DensityUnit.KilogramPerCubicMeter, _dUnit, (double) _param1);
                    DenInput.Text = _d.ToString();
                    if (isSaturated)
                    {
                        _t = TempUnitSelector.ConvertValue(TempUnit.Kelvin, _tUnit, (double)_param2);
                        TempInput.Text = _t.ToString();
                    }

                }
                else if (PresSelector.Checked)
                {
                    _p = PresUnitSelector.ConvertValue(PressureUnit.Pa, _pUnit, (double)_param1);
                    PresInput.Text = _p.ToString();
                    if (isSaturated)
                    {
                        _t = TempUnitSelector.ConvertValue(TempUnit.Kelvin, _tUnit, (double)_param2);
                        TempInput.Text = _t.ToString();
                    }
                }
                else
                {
                    // get temperature and missing param if saturated
                    if (isSaturated)
                    {
                        _t = TempUnitSelector.ConvertValue(TempUnit.Kelvin, _tUnit, (double)_param2);
                        TempInput.Text = _t.ToString();
                        if (!ParseUtility.IsParseableNumber(PresInput.Text))
                        {
                            // if (pressure == null)
                            _p = PresUnitSelector.ConvertValue(PressureUnit.Pa, _pUnit, (double)_param1);
                            PresInput.Text = _p.ToString();
                        }
                        else
                        {
                            _d = DenUnitSelector.ConvertValue(DensityUnit.KilogramPerCubicMeter, _dUnit, (double)_param1);
                            DenInput.Text = _d.ToString();
                        }
                    }
                    else
                    {
                        _t = TempUnitSelector.ConvertValue(TempUnit.Kelvin, _tUnit, (double)_param1);
                        TempInput.Text = _t.ToString();
                    }
                }
            }

            _changeSilently = false;
        }


        private double GetDensity(DensityUnit old = DensityUnit.KilogramPerCubicMeter)
        {
            return DenUnitSelector.ConvertValue(old, DensityUnit.KilogramPerCubicMeter, _d);
        }

        private double GetPressure(PressureUnit old = PressureUnit.Pa)
        {
            return PresUnitSelector.ConvertValue(old, PressureUnit.Pa, _p);
        }

        private double GetTemp(TempUnit old = TempUnit.Kelvin)
        {
            return TempUnitSelector.ConvertValue(old, TempUnit.Kelvin, _t);
        }

        private void PhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Phase = (ModelPair)PhaseSelector.SelectedItem;
            RefreshInputs();
        }
    }
}