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
    public partial class CpEtkMassFlowRate : UserControl
    {
        private DistanceUnit _mActiveOrificeDiamDistUnit = DistanceUnit.Meter;

        private PressureUnit _mActivePressureUnit = PressureUnit.Pa;
        private TempUnit _mActiveTempUnit = TempUnit.Kelvin;
        private VolumeUnit _mActiveVolumeUnit = VolumeUnit.Liter;
        private double _mOrificeDiameterValue = double.NaN;
        private double _mPressureValue = double.NaN;
        private double _mTemperatureValue = double.NaN;
        private double _mVolumeValue = double.NaN;

        public CpEtkMassFlowRate()
        {
            InitializeComponent();
        }

        private bool SteadyBlowdown
        {
            get => rbRtSteady.Checked;
            set => rbRtSteady.Checked = value;
        }

        private void ddOrificeDiameter_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddOrificeDiameter.SelectedItem != null)
            {
                var newUnit =
                    UnitParser.ParseDistanceUnit((string) ddOrificeDiameter.SelectedItem);
                _mOrificeDiameterValue = ddOrificeDiameter.ConvertValue(_mActiveOrificeDiamDistUnit, newUnit,
                    _mOrificeDiameterValue);
                if (!double.IsNaN(_mOrificeDiameterValue)) tbOrificeDiameter.Text = "" + _mOrificeDiameterValue;

                _mActiveOrificeDiamDistUnit = newUnit;
                SetDefaultActiveOrificeDiameterDistanceUnit(newUnit);
            }
        }

        private void SetDefaultActiveOrificeDiameterDistanceUnit(DistanceUnit value)
        {
            Settings.Default.MFROrificeDiamDistUnit = value.ToString();
        }

        private void ddPressure_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (ddPressure.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) ddPressure.SelectedItem);
                _mPressureValue = ddPressure.ConvertValue(_mActivePressureUnit, newUnit, _mPressureValue);
                if (!double.IsNaN(_mPressureValue)) tbPressure.Text = Parsing.DoubleToString(_mPressureValue);

                _mActivePressureUnit = newUnit;
                SetDefaultActivePressureUnit(newUnit);
            }
        }

        private void SetDefaultActivePressureUnit(PressureUnit value)
        {
            Settings.Default.MFRPressureUnit = value.ToString();
        }

        private void ddVolume_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (ddTankVolume.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseVolumeUnit((string) ddTankVolume.SelectedItem);

                _mVolumeValue = ddTankVolume.ConvertValue(_mActiveVolumeUnit, newUnit, _mVolumeValue);
                if (!double.IsNaN(_mVolumeValue)) tbVolume.Text = Parsing.DoubleToString(_mVolumeValue);

                _mActiveVolumeUnit = newUnit;
                SetDefaultActiveVolumeUnit(_mActiveVolumeUnit);
            }
        }

        private void SetDefaultActiveVolumeUnit(VolumeUnit value)
        {
            Settings.Default.MFRVolumeUnit = value.ToString();
        }

        private void SetDefaultActiveTempUnit(TempUnit value)
        {
            Settings.Default.MFRTempUnit = value.ToString();
        }

        private void ddTemperature_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (ddTemperature.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) ddTemperature.SelectedItem);
                _mTemperatureValue = ddTemperature.ConvertValue(_mActiveTempUnit, newUnit, _mTemperatureValue);
                if (!double.IsNaN(_mTemperatureValue)) tbTemperature.Text = Parsing.DoubleToString(_mTemperatureValue);

                _mActiveTempUnit = newUnit;
                SetDefaultActiveTempUnit(_mActiveTempUnit);
            }
        }

        private void tbOrificeDiameter_TextChanged(object sender, EventArgs e)
        {
            Parsing.TryParseDouble(tbOrificeDiameter.Text, out _mOrificeDiameterValue);
            SetButtonEnabled();
        }

        private void tbVolume_TextChanged(object sender, EventArgs e)
        {
            Parsing.TryParseDouble(tbVolume.Text, out _mVolumeValue);
            SetButtonEnabled();
        }

        private bool AllInputsAreParseable()
        {
            var result = MiscFunctions.IsParseableNumber(tbVolume.Text) &&
                         MiscFunctions.IsParseableNumber(tbTemperature.Text) &&
                         MiscFunctions.IsParseableNumber(tbPressure.Text) &&
                         MiscFunctions.IsParseableNumber(tbOrificeDiameter.Text);
            return result;
        }

        private void SetButtonEnabled()
        {
            btnCalculate.Enabled = AllInputsAreParseable();
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

        private void ProcessLoadEvent(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                ddTemperature.Converter = StockConverters.GetConverterByName("Temperature");
                _mActiveTempUnit = UnitParser.ParseTempUnit(Settings.Default.MFRTempUnit);
                ddTemperature.SelectedItem = _mActiveTempUnit;

                _mActivePressureUnit = UnitParser.ParsePressureUnit(Settings.Default.MFRPressureUnit);
                ddPressure.Converter = StockConverters.GetConverterByName("Pressure");
                ddPressure.SelectedItem = _mActivePressureUnit;

                _mActiveVolumeUnit = UnitParser.ParseVolumeUnit(Settings.Default.MFRVolumeUnit);
                ddTankVolume.Converter = StockConverters.GetConverterByName("Volume");
                ddTankVolume.SelectedItem = _mActiveVolumeUnit;

                _mActiveOrificeDiamDistUnit =
                    UnitParser.ParseDistanceUnit(Settings.Default.MFROrificeDiamDistUnit);
                ddOrificeDiameter.Converter = StockConverters.GetConverterByName("Distance");
                ddOrificeDiameter.SelectedItem = _mActiveOrificeDiamDistUnit;
                _mOrificeDiameterValue = 3.0e-2;
                tbOrificeDiameter.Text = "" + _mOrificeDiameterValue;

            }
        }

        private void cpEtkMassFlowRate_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent(sender, e);
        }

        private void ReleaseTypeChanged(object sender, EventArgs e)
        {
            if (SteadyBlowdown)
            {
                tbVolume.Text = "0.0";
                tbVolume.Enabled = false;
            }
            else
            {
                tbVolume.Enabled = true;
            }
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            var temp = GetTempInWrapperUnits();
            var pressure = GetPressureInWrapperUnits();
            var tankVolume = GetTankVolumeInWrapperUnits();
            var isSteady = SteadyBlowdown;
            var orificeDiam = GetOrificeDiameterInWrapperUnits();
            var dischargeCoeff = 1.0;

            var physApi = new PhysInterface();

            physApi.ComputeFlowRateOrTimeToEmpty(temp, pressure, tankVolume, orificeDiam, isSteady, dischargeCoeff,
                out var massFlowRate, out var timeToEmpty, out var plotFileLoc);

            if (isSteady)
            {
                lblResult.Text = "Mass flow rate (kg/s)";
                tbResult.Text = massFlowRate != null ? Parsing.DoubleToString((double) massFlowRate) : "Error";
            }
            else
            {
                lblResult.Text = "Time to empty (s)";
                pbResultImage.Load(plotFileLoc);
                pbResultImage.Visible = true;
                tbResult.Text = timeToEmpty != null ? Parsing.DoubleToString((double) timeToEmpty) : "Error";
            }

            ParentForm.TopMost = true;
            tcMain.SelectedTab = tpOutput;
        }


        private double GetOrificeDiameterInWrapperUnits()
        {
            var userUnit = UnitParser.ParseDistanceUnit((string) ddOrificeDiameter.SelectedItem);
            var wrapperUnit = DistanceUnit.Meter;
            return ddOrificeDiameter.ConvertValue(userUnit, wrapperUnit, _mOrificeDiameterValue);
        }

        private double GetTankVolumeInWrapperUnits()
        {
            var userUnit = UnitParser.ParseVolumeUnit((string) ddTankVolume.SelectedItem);
            var wrapperUnit = VolumeUnit.CubicMeter;
            return ddTankVolume.ConvertValue(userUnit, wrapperUnit, _mVolumeValue);
        }

        private double GetPressureInWrapperUnits()
        {
            var oldUnit = UnitParser.ParsePressureUnit((string) ddPressure.SelectedItem);
            var newUnit = PressureUnit.Pa;
            return ddPressure.ConvertValue(oldUnit, newUnit, _mPressureValue);
        }

        private double GetTempInWrapperUnits()
        {
            var oldUnit = UnitParser.ParseTempUnit((string) ddTemperature.SelectedItem);
            var newUnit = TempUnit.Kelvin;
            return ddTemperature.ConvertValue(oldUnit, newUnit, _mTemperatureValue);
        }

        private void MakeSureOffscreenImageIsShowing()
        {
            // Image doesn't zoom correctly if it's set while invisible. This will make it visible and if we don't want to
            // show user we can hide it later.
            if (!pbResultImage.Visible) pbResultImage.Visible = true;
        }

        private void btnCalculate_MouseMove(object sender, MouseEventArgs e)
        {
            MakeSureOffscreenImageIsShowing();
        }

        private void tpInput_MouseMove(object sender, MouseEventArgs e)
        {
            MakeSureOffscreenImageIsShowing();
        }

        private void tpOutput_Enter(object sender, EventArgs e)
        {
            pbResultImage.Visible = RbRtBlowdown.Checked;
        }
    }
}