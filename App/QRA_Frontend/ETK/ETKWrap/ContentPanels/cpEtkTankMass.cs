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

using System;
using System.Windows.Forms;
using DefaultParsing;
using EssStringLib;
using JrConversions;
using PyAPI;
using QRA_Frontend.Properties;

namespace QRA_Frontend.ETK.ETKWrap.ContentPanels
{
    public partial class CpEtkTankMass : UserControl
    {
        public CpEtkTankMass()
        {
            InitializeComponent();
        }

        private double _mVolumeValue = double.NaN;
        private double _mTemperatureValue = double.NaN;
        private double _mPressureValue = double.NaN;
        private readonly double _mMassValueInKg = double.NaN;

        private PressureUnit _mActivePressureUnit = PressureUnit.Pa;
        private TempUnit _mActiveTempUnit = TempUnit.Kelvin;
        private VolumeUnit _mActiveVolumeUnit = VolumeUnit.Liter;

        private void ProcessLoadEvent(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                ddTemperature.Converter = StockConverters.GetConverterByName("Temperature");
                _mActiveTempUnit = UnitParser.ParseTempUnit(Settings.Default.TMTempUnit);
                ddTemperature.SelectedItem = _mActiveTempUnit;

                _mActivePressureUnit = UnitParser.ParsePressureUnit(Settings.Default.TMPressureUnit);
                ddPressure.Converter = StockConverters.GetConverterByName("Pressure");
                ddPressure.SelectedItem = _mActivePressureUnit;

                _mActiveVolumeUnit = UnitParser.ParseVolumeUnit(Settings.Default.TMVolumeUnit);
                ddTankVolume.Converter = StockConverters.GetConverterByName("Volume");
                ddTankVolume.SelectedItem = _mActiveVolumeUnit;

                ddMass.Converter = StockConverters.GetConverterByName("Mass");
                ddMass.SelectedItem = UnitParser.ParseMassUnit(Settings.Default.TMMassUnit);
            }
        }

        private void ddTemperature_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (ddTemperature.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) ddTemperature.SelectedItem);
                _mTemperatureValue = ddTemperature.ConvertValue(_mActiveTempUnit, newUnit, _mTemperatureValue);
                if (!double.IsNaN(_mTemperatureValue)) tbTemperature.Text = Parsing.DoubleToString(_mTemperatureValue);

                _mActiveTempUnit = newUnit;
                Settings.Default.TMTempUnit = _mActiveTempUnit.ToString();
            }
        }

        private void ddPressure_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (ddPressure.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) ddPressure.SelectedItem);
                _mPressureValue = ddPressure.ConvertValue(_mActivePressureUnit, newUnit, _mPressureValue);
                if (!double.IsNaN(_mPressureValue)) tbPressure.Text = Parsing.DoubleToString(_mPressureValue);

                _mActivePressureUnit = newUnit;
                Settings.Default.TMPressureUnit = _mActivePressureUnit.ToString();
            }
        }

        private void ddVolume_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (ddTankVolume.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseVolumeUnit((string) ddTankVolume.SelectedItem);

                _mVolumeValue = ddTankVolume.ConvertValue(_mActiveVolumeUnit, newUnit, _mVolumeValue);
                if (!double.IsNaN(_mVolumeValue)) tbVolume.Text = Parsing.DoubleToString(_mVolumeValue);

                _mActiveVolumeUnit = newUnit;
                Settings.Default.TMVolumeUnit = _mActiveVolumeUnit.ToString();
            }
        }

        private void cpEtkTankMass_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent(sender, e);
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            try
            {
                var temp = GetTempInCorrectUnits();
                var pressure = GetPressureValueInCorrectUnits();
                var volume = GetVolumeValueInCorrectUnits();

                var physApi = new PhysInterface();
                var mass = physApi.ComputeTankMass(temp, pressure, volume);
                if (mass != null)
                {
                    var correctedMass = ddMass.ConvertValue(MassUnit.Kilogram, _mActiveMassUnit, (double) mass);
                    tbMass.Text = correctedMass.ToString();
                }
                else
                {
                    tbMass.Text = "Error";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred when attempting to perform the calculation: " + ex.Message);
            }
        }

        private double PutResultIntoUserUnits()
        {
            Enum destEnum = _mActiveMassUnit;
            var result = ddMass.ConvertValue(MassUnit.Kilogram, _mActiveMassUnit, _mMassValueInKg);
            return result;
        }

        private double GetVolumeValueInCorrectUnits()
        {
            var oldUnit = UnitParser.ParseVolumeUnit((string) ddTankVolume.SelectedItem);
            var newUnit = VolumeUnit.CubicMeter;
            ;
            return ddTankVolume.ConvertValue(oldUnit, newUnit, _mVolumeValue);
        }

        private double GetPressureValueInCorrectUnits()
        {
            var oldUnit = UnitParser.ParsePressureUnit((string) ddPressure.SelectedItem);
            var newUnit = PressureUnit.Pa;
            return ddPressure.ConvertValue(oldUnit, newUnit, _mPressureValue);
        }

        private double GetTempInCorrectUnits()
        {
            var oldUnit = UnitParser.ParseTempUnit((string) ddTemperature.SelectedItem);
            var newUnit = TempUnit.Kelvin;
            return ddTemperature.ConvertValue(oldUnit, newUnit, _mTemperatureValue);
        }

        private MassUnit _mActiveMassUnit = MassUnit.Kilogram;

        private void ddMass_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddMass.SelectedItem != null)
            {
                _mActiveMassUnit = UnitParser.ParseMassUnit((string) ddMass.SelectedItem);
                var valueInUserUnits = PutResultIntoUserUnits();

                Settings.Default.TMMassUnit = _mActiveMassUnit.ToString();
                tbMass.Text = "" + valueInUserUnits;
            }
        }

        private void tbVolume_TextChanged(object sender, EventArgs e)
        {
            Parsing.TryParseDouble(tbVolume.Text, out _mVolumeValue);
            SetButtonEnabled();
        }

        private void SetButtonEnabled()
        {
            btnCalculate.Enabled = AllInputsAreParseable();
        }

        private bool AllInputsAreParseable()
        {
            var result = MiscFunctions.IsParseableNumber(tbVolume.Text) &&
                         MiscFunctions.IsParseableNumber(tbTemperature.Text) &&
                         MiscFunctions.IsParseableNumber(tbPressure.Text);
            return result;
        }

        private void tbMass_TextChanged(object sender, EventArgs e)
        {
            ShowOrHideMassControls();
        }

        private void ShowOrHideMassControls()
        {
            var showIt = tbMass.Text.Length >= 0;
            tbMass.Visible = showIt;
            ddMass.Visible = showIt;
            lblMass.Visible = showIt;
        }

        private void tbTemperature_TextChanged(object sender, EventArgs e)
        {
            Parsing.TryParseDouble(tbTemperature.Text, out _mTemperatureValue);
            SetButtonEnabled();
        }

        private void tbPressure_TextChanged(object sender, EventArgs e)
        {
            Parsing.TryParseDouble(tbPressure.Text, out _mPressureValue);
            SetButtonEnabled();
        }
    }
}