/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class TemperaturePressureDensityForm : UserControl
    {
        private double _mTemperatureValue = double.NaN;
        private double _mPressureValue = double.NaN;
        private double _mDensityValue = double.NaN;
        private PressureUnit _mActivePressureUnit = PressureUnit.Pa;
        private TempUnit _mActiveTempUnit = TempUnit.Kelvin;
        private DensityUnit _mActiveDensityUnit = DensityUnit.KilogramPerCubicMeter;
        private bool _changeSilently = false;

        public TemperaturePressureDensityForm()
        {
            InitializeComponent();
        }

        private void cpEtkTempPressureDensity_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent(sender, e);
        }

        private void ProcessLoadEvent(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                SetRadiobuttonSavedSettings();

                fuelPhaseSelector.DataSource = StateContainer.Instance.FluidPhases;
                fuelPhaseSelector.SelectedItem = StateContainer.GetValue<FluidPhase>("ReleaseFluidPhase");

                temperatureUnitSelector.Converter = StockConverters.GetConverterByName("Temperature");
                _mActiveTempUnit = GetDefaultActiveTempUnit();
                temperatureUnitSelector.SelectedItem = _mActiveTempUnit;

                _mActivePressureUnit = GetDefaultActivePressureUnit();
                pressureUnitSelector.Converter = StockConverters.GetConverterByName("Pressure");
                pressureUnitSelector.SelectedItem = _mActivePressureUnit;

                _mActiveDensityUnit = GetDefaultActiveDensityUnit();

                densityUnitSelector.Converter = StockConverters.GetConverterByName("Density");
                densityUnitSelector.SelectedItem = _mActiveDensityUnit;

                RefreshInputs();
            }
        }

        public void EnteringForm()
        {
            fuelPhaseSelector.SelectedItem = StateContainer.GetValue<FluidPhase>("ReleaseFluidPhase");
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
            var phase = StateContainer.Instance.GetFluidPhase();
            bool isSaturated = (phase != FluidPhase.GasDefault);

            bool input1Valid = true;
            bool input2Valid = true;
            densityInput.Enabled = true;
            pressureInput.Enabled = true;
            temperatureInput.Enabled = true;

            if (isSaturated)
            {
                temperatureInput.Enabled = false;
                temperatureInput.Text = "";
            }

            if (densitySelector.Checked)
            {
                // If not saturated, verify that two other parameters are valid
                densityInput.Enabled = false;
                input1Valid = ParseUtility.IsParseableNumber(pressureInput.Text);
                if (!isSaturated)
                {
                    // If saturated, second parameter not required
                    input2Valid = ParseUtility.IsParseableNumber(temperatureInput.Text);
                }
            }
            else if (pressureSelector.Checked)
            {
                pressureInput.Enabled = false;
                input1Valid = ParseUtility.IsParseableNumber(densityInput.Text);
                if (!isSaturated)
                {
                    input2Valid = ParseUtility.IsParseableNumber(temperatureInput.Text);
                }
            }
            else
            {
                // compute temperature; check which other input is provided
                temperatureInput.Enabled = false;
                if (isSaturated)
                {
                    // only 1 other input needed
                    if (pressureInput.Text.Length > 0)
                    {
                        input1Valid = ParseUtility.IsParseableNumber(pressureInput.Text);
                        densityInput.Enabled = false;
                    }
                    else if (densityInput.Text.Length > 0)
                    {
                        input1Valid = ParseUtility.IsParseableNumber(densityInput.Text);
                        pressureInput.Enabled = false;
                    }
                    else
                    {
                        input1Valid = false;
                    }
                }
                else
                {
                    input1Valid = ParseUtility.IsParseableNumber(densityInput.Text);
                    input2Valid = ParseUtility.IsParseableNumber(pressureInput.Text);
                }
            }

            submitButton.Enabled = input1Valid && input2Valid;
        }

        private void temperatureUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (temperatureUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) temperatureUnitSelector.SelectedItem);
                _mTemperatureValue = temperatureUnitSelector.ConvertValue(_mActiveTempUnit, newUnit, _mTemperatureValue);
                if (!double.IsNaN(_mTemperatureValue))
                    temperatureInput.Text = ParseUtility.DoubleToString(_mTemperatureValue);

                _mActiveTempUnit = newUnit;
                Settings.Default.TPDTempUnit = _mActiveTempUnit.ToString();
            }
        }

        // Restores input states from last ETK form display
        private void SetRadiobuttonSavedSettings()
        {
            densitySelector.Checked = Settings.Default.TPDDensityControl;
            pressureSelector.Checked = Settings.Default.TPDPressureControl;
            temperatureSelector.Checked = Settings.Default.TPDTempControl;
            densitySelector.Refresh();
            pressureSelector.Refresh();
            temperatureSelector.Refresh();
        }

        private void pressureUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (pressureUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) pressureUnitSelector.SelectedItem);
                _mPressureValue = pressureUnitSelector.ConvertValue(_mActivePressureUnit, newUnit, _mPressureValue);
                if (!double.IsNaN(_mPressureValue)) pressureInput.Text = ParseUtility.DoubleToString(_mPressureValue);

                _mActivePressureUnit = newUnit;
                Settings.Default.TPDPressureUnit = newUnit.ToString();
            }
        }

        private void densityUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (densityUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseDensityUnit((string) densityUnitSelector.SelectedItem);
                _mDensityValue = densityUnitSelector.ConvertValue(_mActiveDensityUnit, newUnit, _mDensityValue);

                if (!double.IsNaN(_mDensityValue)) densityInput.Text = "" + _mDensityValue;

                _mActiveDensityUnit = newUnit;
                Settings.Default.TPDDensitiyUnit = newUnit.ToString();
            }
        }

        private DensityUnit GetDefaultActiveDensityUnit()
        {
            return UnitParser.ParseDensityUnit(Settings.Default.TPDDensitiyUnit);
        }

        private PressureUnit GetDefaultActivePressureUnit()
        {
            return UnitParser.ParsePressureUnit(Settings.Default.TPDPressureUnit);
        }

        private void tbDensity_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(densityInput.Text, out _mDensityValue);
            RefreshInputs();
        }

        private void temperatureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(temperatureInput.Text, out _mTemperatureValue);
            RefreshInputs();
        }

        private void pressureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(pressureInput.Text, out _mPressureValue);
            RefreshInputs();
        }

        private TempUnit GetDefaultActiveTempUnit()
        {
            return UnitParser.ParseTempUnit(Settings.Default.TPDTempUnit);
        }

        private void CalcOptionRbCheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.TPDDensityControl = densitySelector.Checked;
            Settings.Default.TPDPressureControl = pressureSelector.Checked;
            Settings.Default.TPDTempControl = temperatureSelector.Checked;
            RefreshInputs();
        }


        // Computes missing parameter(s). If saturated, second missing parameter will also be calculated.
        private void calculateButton_Click(object sender, EventArgs e)
        {
            double? temp = null;
            double? pressure = null;
            double? density = null;
            var phase = StateContainer.Instance.GetFluidPhase();
            bool isSaturated = (phase != FluidPhase.GasDefault);

            _changeSilently = true;

            if (densitySelector.Checked)
            {
                pressure = GetPressureValueInCorrectUnits();
                temp = !isSaturated ? (double?)GetTempInCorrectUnits() : null;
            }
            else if (pressureSelector.Checked)
            {
                density = GetDensityInCorrectUnits();
                temp = !isSaturated ? (double?)GetTempInCorrectUnits() : null;
            }
            else
            {
                // compute temperature; only 1 input needed if saturated
                if (isSaturated)
                {
                    if (ParseUtility.IsParseableNumber(pressureInput.Text))
                    {
                        pressure = GetPressureValueInCorrectUnits();
                    }
                    else
                    {
                        density = GetDensityInCorrectUnits();
                    }
                }
                else
                {
                    pressure = GetPressureValueInCorrectUnits();
                    density = GetDensityInCorrectUnits();
                }
            }

            var physApi = new PhysicsInterface();
            bool status = physApi.ComputeTpd(temp, pressure, density, phase.GetKey(),
                                             out string statusMsg, out double? param1, out double? param2);

            if (!status)
            {
                MessageBox.Show(statusMsg);
            }
            else
            {
                if (densitySelector.Checked)
                {
                    densityInput.Text = densityUnitSelector.ConvertValue(DensityUnit.KilogramPerCubicMeter,
                                                                  _mActiveDensityUnit, (double)param1).ToString();
                    if (isSaturated)
                    {
                        temperatureInput.Text = temperatureUnitSelector.ConvertValue(TempUnit.Kelvin,
                                                                      _mActiveTempUnit, (double)param2).ToString();
                    }

                }
                else if (pressureSelector.Checked)
                {
                    pressureInput.Text = pressureUnitSelector.ConvertValue(PressureUnit.Pa,
                                                                  _mActivePressureUnit, (double)param1).ToString();
                    if (isSaturated)
                    {
                        temperatureInput.Text = temperatureUnitSelector.ConvertValue(TempUnit.Kelvin,
                                                                      _mActiveTempUnit, (double)param2).ToString();
                    }
                }
                else
                {
                    // get temperature and missing param if saturated
                    if (isSaturated)
                    {
                        temperatureInput.Text = temperatureUnitSelector.ConvertValue(TempUnit.Kelvin,
                                                                      _mActiveTempUnit, (double)param2).ToString();
                        if (pressure == null)
                        {
                            pressureInput.Text = pressureUnitSelector.ConvertValue(PressureUnit.Pa,
                                                                          _mActivePressureUnit, (double)param1).ToString();
                        }
                        else
                        {
                            densityInput.Text = densityUnitSelector.ConvertValue(DensityUnit.KilogramPerCubicMeter,
                                                                          _mActiveDensityUnit, (double)param1).ToString();
                        }
                    }
                    else
                    {
                        temperatureInput.Text = temperatureUnitSelector.ConvertValue(TempUnit.Kelvin,
                                                                      _mActiveTempUnit, (double)param1).ToString();
                    }
                }
            }

            _changeSilently = false;
        }


        private double GetDensityInCorrectUnits()
        {
            var oldUnit = UnitParser.ParseDensityUnit((string) densityUnitSelector.SelectedItem);
            var newUnit = DensityUnit.KilogramPerCubicMeter;
            return densityUnitSelector.ConvertValue(oldUnit, newUnit, _mDensityValue);
        }

        private double GetPressureValueInCorrectUnits()
        {
            var oldUnit = UnitParser.ParsePressureUnit((string) pressureUnitSelector.SelectedItem);
            var newUnit = PressureUnit.Pa;
            return pressureUnitSelector.ConvertValue(oldUnit, newUnit, _mPressureValue);
        }

        private double GetTempInCorrectUnits()
        {
            var oldUnit = UnitParser.ParseTempUnit((string) temperatureUnitSelector.SelectedItem);
            var newUnit = TempUnit.Kelvin;
            return temperatureUnitSelector.ConvertValue(oldUnit, newUnit, _mTemperatureValue);
        }

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var phase = fuelPhaseSelector.SelectedItem;
            StateContainer.SetValue("ReleaseFluidPhase", phase);
            RefreshInputs();
        }
    }
}