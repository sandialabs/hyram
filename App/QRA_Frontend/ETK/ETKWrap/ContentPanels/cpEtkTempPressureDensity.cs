// Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
// Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
// 
// This file is part of HyRAM (Hydrogen Risk Assessment Models).
// 
// HyRAM is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// HyRAM is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

// Note that GUI editor may not be available in x64 mode. Have to re-compile in 32 to real-time edit due to MSVS limitation with custom controls (ValueConverterDropdown).

using System;
using System.Windows.Forms;
using DefaultParsing;
using EssStringLib;
using JrConversions;
using PyAPI;
using QRA_Frontend.Properties;

namespace QRA_Frontend.ETK.ETKWrap.ContentPanels
{
    public partial class CpEtkTempPressureDensity : UserControl
    {
        // TODO (Cianan): Update only changed the _click func to use python.
        // In future, this class could be simplified.
        private double _mTemperatureValue = double.NaN;
        private double _mPressureValue = double.NaN;
        private double _mDensityValue = double.NaN;

        private enum CalculationOption
        {
            CalculatePressure,
            CalculateTemperature,
            CalculateDensity
        }

        private CalculationOption _mCalculationOption;

        public CpEtkTempPressureDensity()
        {
            InitializeComponent();
        }

        private void ddTemperature_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (dd_Temperature.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) dd_Temperature.SelectedItem);
                _mTemperatureValue = dd_Temperature.ConvertValue(_mActiveTempUnit, newUnit, _mTemperatureValue);
                if (!double.IsNaN(_mTemperatureValue))
                    tb_tpd_Temperature.Text = Parsing.DoubleToString(_mTemperatureValue);

                _mActiveTempUnit = newUnit;
                Settings.Default.TPDTempUnit = _mActiveTempUnit.ToString();
            }
        }

        private void SetRadiobuttonSavedSettings()
        {
            rb_Density.Checked = Settings.Default.TPDDensityControl;
            rb_Density.Refresh();
            rb_Pressure.Checked = Settings.Default.TPDPressureControl;
            rb_Pressure.Refresh();
            rb_Temperature.Checked = Settings.Default.TPDTempControl;
            rb_Temperature.Refresh();
        }

        private void ddPressure_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (dd_Pressure.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) dd_Pressure.SelectedItem);
                _mPressureValue = dd_Pressure.ConvertValue(_mActivePressureUnit, newUnit, _mPressureValue);
                if (!double.IsNaN(_mPressureValue)) tb_tpd_Pressure.Text = Parsing.DoubleToString(_mPressureValue);

                _mActivePressureUnit = newUnit;
                Settings.Default.TPDPressureUnit = newUnit.ToString();
            }
        }

        private void ddDensity_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (dd_Density.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseDensityUnit((string) dd_Density.SelectedItem);
                _mDensityValue = dd_Density.ConvertValue(_mActiveDensityUnit, newUnit, _mDensityValue);

                if (!double.IsNaN(_mDensityValue)) tb_tpd_Density.Text = "" + _mDensityValue;

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
            Parsing.TryParseDouble(tb_tpd_Density.Text, out _mDensityValue);
            SetCalcButtonProperties();
        }

        private void tbTemperature_TextChanged(object sender, EventArgs e)
        {
            Parsing.TryParseDouble(tb_tpd_Temperature.Text, out _mTemperatureValue);
            SetCalcButtonProperties();
        }

        private void tbPressure_TextChanged(object sender, EventArgs e)
        {
            Parsing.TryParseDouble(tb_tpd_Pressure.Text, out _mPressureValue);
            SetCalcButtonProperties();
        }

        private TempUnit GetDefaultActiveTempUnit()
        {
            return UnitParser.ParseTempUnit(Settings.Default.TPDTempUnit);
        }

        private void CalcOptionRbCheckedChanged(object sender, EventArgs e)
        {
            SetTextboxEnabled();
            Settings.Default.TPDDensityControl = rb_Density.Checked;
            Settings.Default.TPDPressureControl = rb_Pressure.Checked;
            Settings.Default.TPDTempControl = rb_Temperature.Checked;
        }

        private void SetTextboxEnabled()
        {
            var densEnabled = !rb_Density.Checked;
            var presEnabled = !rb_Pressure.Checked;
            var tempEnabled = !rb_Temperature.Checked;

            if (tempEnabled && presEnabled)
                _mCalculationOption = CalculationOption.CalculateDensity;
            else if (tempEnabled && densEnabled)
                _mCalculationOption = CalculationOption.CalculatePressure;
            else if (presEnabled && densEnabled)
                _mCalculationOption = CalculationOption.CalculateTemperature;
            else
                MessageBox.Show(@"Unable to determine calculation option.");

            if (tb_tpd_Density.Enabled != densEnabled) tb_tpd_Density.Enabled = densEnabled;

            if (tb_tpd_Pressure.Enabled != presEnabled) tb_tpd_Pressure.Enabled = presEnabled;

            if (tb_tpd_Temperature.Enabled != tempEnabled) tb_tpd_Temperature.Enabled = tempEnabled;

            SetCalcButtonProperties();
        }

        private void SetCalcButtonProperties()
        {
            string leftValue, rightValue;

            switch (_mCalculationOption)
            {
                case CalculationOption.CalculateDensity:
                    leftValue = tb_tpd_Temperature.Text;
                    rightValue = tb_tpd_Pressure.Text;
                    btn_Calculate.Text = "Calculate Density";
                    break;
                case CalculationOption.CalculatePressure:
                    leftValue = tb_tpd_Temperature.Text;
                    rightValue = tb_tpd_Density.Text;
                    btn_Calculate.Text = "Calculate Pressure";
                    break;
                case CalculationOption.CalculateTemperature:
                    leftValue = tb_tpd_Density.Text;
                    rightValue = tb_tpd_Pressure.Text;
                    btn_Calculate.Text = "Calculate Temperature";
                    break;
                default:
                    btn_Calculate.Text = "Unknown Option Selected";
                    throw new Exception("Calculation option of " + _mCalculationOption + " unknown.");
            }

            var enableButton = false;
            if (MiscFunctions.IsParseableNumber(leftValue) && MiscFunctions.IsParseableNumber(rightValue))
                enableButton = true;

            btn_Calculate.Enabled = enableButton;
        }

        /// <summary>
        ///     Compute missing parameter via python call
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCalculate_Click(object sender, EventArgs e)
        {
            double? temp = null;
            double? pressure = null;
            double? density = null;

            switch (_mCalculationOption)
            {
                case CalculationOption.CalculateDensity:
                    temp = GetTempInCorrectUnits();
                    pressure = GetPressureValueInCorrectUnits();
                    break;
                case CalculationOption.CalculatePressure:
                    density = GetDensityInCorrectUnits();
                    temp = GetTempInCorrectUnits();
                    break;
                case CalculationOption.CalculateTemperature:
                    density = GetDensityInCorrectUnits();
                    pressure = GetPressureValueInCorrectUnits();
                    break;
                default:
                    throw new Exception("Calculation option of " + _mCalculationOption + " unknown.");
            }

            try
            {
                var physApi = new PhysInterface();
                var result = (double) physApi.ComputeTpd(temp, pressure, density);

                var valueToUse = double.NaN;
                switch (_mCalculationOption)
                {
                    case CalculationOption.CalculateDensity:
                        valueToUse =
                            dd_Density.ConvertValue(DensityUnit.KilogramCubicMeter, _mActiveDensityUnit, result);
                        break;
                    case CalculationOption.CalculatePressure:
                        valueToUse = dd_Pressure.ConvertValue(PressureUnit.Pa, _mActivePressureUnit, result);
                        break;
                    case CalculationOption.CalculateTemperature:
                        valueToUse = dd_Temperature.ConvertValue(TempUnit.Kelvin, _mActiveTempUnit, result);
                        break;
                }

                var resultContainer = GetResultContainer();
                resultContainer.Text = Parsing.DoubleToString(valueToUse);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when attempting to perform the calculation: " + ex.Message);
            }
        }

        private TextBox GetResultContainer()
        {
            TextBox[] candidates = {tb_tpd_Density, tb_tpd_Pressure, tb_tpd_Temperature};
            var numberOfWritable = 0; // Will throw exception if this isn't set to 1.
            TextBox result = null;

            foreach (var thisCandidate in candidates)
                if (!thisCandidate.Enabled)
                {
                    numberOfWritable++;
                    result = thisCandidate;
                }

            if (numberOfWritable != 1) throw new Exception("Output textbox candidate could not be determined");

            return result;
        }

#if false
        private ETKWrap.Wrappers.ArgName[] GetArguments() {
			ETKWrap.Wrappers.ArgName[] Result = new ETKWrap.Wrappers.ArgName[2];
			double TempValue = double.NaN,PressureValue = double.NaN,DensityValue = double.NaN;

			switch(mCalculationOption) {
				case CalculationOption.CalculateDensity:
					TempValue = GetTempInCorrectUnits();
					PressureValue = GetPressureValueInCorrectUnits();
					Result[0] = new ETKWrap.Wrappers.ArgName("T_H2",TempValue,"Tank temperature [K]");
					Result[1] = new ETKWrap.Wrappers.ArgName("P_H2",PressureValue,"initial hydrogen pressure [Pa]");
					break;
				case CalculationOption.CalculatePressure:
					DensityValue = GetDensityInCorrectUnits();
					TempValue = GetTempInCorrectUnits();
					Result[0] = new ETKWrap.Wrappers.ArgName("rho_H2",DensityValue,"Density [kg/m^3]");
					Result[1] = new ETKWrap.Wrappers.ArgName("T_H2",TempValue,"Tank temperature [K]");
					break;
				case CalculationOption.CalculateTemperature:
					DensityValue = GetDensityInCorrectUnits();
					PressureValue = GetPressureValueInCorrectUnits();
					Result[0] = new ETKWrap.Wrappers.ArgName("rho_H2",DensityValue,"Density in unknown units");				
					Result[1] = new ETKWrap.Wrappers.ArgName("P_H2",PressureValue,"initial hydrogen pressure [Pa]");
					break;
				default:
					throw new Exception("Calculation option of " + mCalculationOption.ToString() + " unknown.");
			}
			return Result;
		}
#endif

        private double GetDensityInCorrectUnits()
        {
            var oldUnit = UnitParser.ParseDensityUnit((string) dd_Density.SelectedItem);
            var newUnit = DensityUnit.KilogramCubicMeter;
            return dd_Density.ConvertValue(oldUnit, newUnit, _mDensityValue);
        }

        private double GetPressureValueInCorrectUnits()
        {
            var oldUnit = UnitParser.ParsePressureUnit((string) dd_Pressure.SelectedItem);
            var newUnit = PressureUnit.Pa;
            return dd_Pressure.ConvertValue(oldUnit, newUnit, _mPressureValue);
        }

        private double GetTempInCorrectUnits()
        {
            var oldUnit = UnitParser.ParseTempUnit((string) dd_Temperature.SelectedItem);
            var newUnit = TempUnit.Kelvin;
            return dd_Temperature.ConvertValue(oldUnit, newUnit, _mTemperatureValue);
        }

        private void ProcessLoadEvent(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                SetRadiobuttonSavedSettings();

                dd_Temperature.Converter = StockConverters.GetConverterByName("Temperature");
                _mActiveTempUnit = GetDefaultActiveTempUnit();
                dd_Temperature.SelectedItem = _mActiveTempUnit;

                _mActivePressureUnit = GetDefaultActivePressureUnit();
                dd_Pressure.Converter = StockConverters.GetConverterByName("Pressure");
                dd_Pressure.SelectedItem = _mActivePressureUnit;

                _mActiveDensityUnit = GetDefaultActiveDensityUnit();

                dd_Density.Converter = StockConverters.GetConverterByName("Density");
                dd_Density.SelectedItem = _mActiveDensityUnit;

                SetTextboxEnabled();
            }
        }


        private PressureUnit _mActivePressureUnit = PressureUnit.Pa;
        private TempUnit _mActiveTempUnit = TempUnit.Kelvin;
        private DensityUnit _mActiveDensityUnit = DensityUnit.KilogramCubicMeter;

        private void cpEtkTempPressureDensity_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent(sender, e);
        }
    }
}