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


namespace SandiaNationalLaboratories.Hyram
{
    public partial class TankMassForm : UserControl
    {
        public TankMassForm()
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

        private void ProcessLoadEvent()
        {
            if (!DesignMode)
            {
                fuelPhaseSelector.DataSource = StateContainer.Instance.FluidPhases;
                fuelPhaseSelector.SelectedItem = StateContainer.GetValue<FluidPhase>("ReleaseFluidPhase");

                temperatureUnitSelector.Converter = StockConverters.GetConverterByName("Temperature");
                _mActiveTempUnit = UnitParser.ParseTempUnit(Settings.Default.TMTempUnit);
                temperatureUnitSelector.SelectedItem = _mActiveTempUnit;

                _mActivePressureUnit = UnitParser.ParsePressureUnit(Settings.Default.TMPressureUnit);
                pressureUnitSelector.Converter = StockConverters.GetConverterByName("Pressure");
                pressureUnitSelector.SelectedItem = _mActivePressureUnit;

                _mActiveVolumeUnit = UnitParser.ParseVolumeUnit(Settings.Default.TMVolumeUnit);
                tankVolumeUnitSelector.Converter = StockConverters.GetConverterByName("Volume");
                tankVolumeUnitSelector.SelectedItem = _mActiveVolumeUnit;

                massUnitSelector.Converter = StockConverters.GetConverterByName("Mass");
                massUnitSelector.SelectedItem = UnitParser.ParseMassUnit(Settings.Default.TMMassUnit);
            }
        }

        /// <summary>
        /// Convenience func when tab is entered (via parent EtkMainForm)
        /// </summary>
        public void EnteringForm()
        {
            fuelPhaseSelector.SelectedItem = StateContainer.GetValue<FluidPhase>("ReleaseFluidPhase");
            CheckFormValid();
        }

        private void temperatureUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (temperatureUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) temperatureUnitSelector.SelectedItem);
                _mTemperatureValue = temperatureUnitSelector.ConvertValue(_mActiveTempUnit, newUnit, _mTemperatureValue);
                if (!double.IsNaN(_mTemperatureValue)) temperatureInput.Text = ParseUtility.DoubleToString(_mTemperatureValue);

                _mActiveTempUnit = newUnit;
                Settings.Default.TMTempUnit = _mActiveTempUnit.ToString();
            }
        }

        private void pressureUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (pressureUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParsePressureUnit((string) pressureUnitSelector.SelectedItem);
                _mPressureValue = pressureUnitSelector.ConvertValue(_mActivePressureUnit, newUnit, _mPressureValue);
                if (!double.IsNaN(_mPressureValue)) pressureInput.Text = ParseUtility.DoubleToString(_mPressureValue);

                _mActivePressureUnit = newUnit;
                Settings.Default.TMPressureUnit = _mActivePressureUnit.ToString();
            }
        }

        private void ddVolume_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (tankVolumeUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseVolumeUnit((string) tankVolumeUnitSelector.SelectedItem);

                _mVolumeValue = tankVolumeUnitSelector.ConvertValue(_mActiveVolumeUnit, newUnit, _mVolumeValue);
                if (!double.IsNaN(_mVolumeValue)) volumeInput.Text = ParseUtility.DoubleToString(_mVolumeValue);

                _mActiveVolumeUnit = newUnit;
                Settings.Default.TMVolumeUnit = _mActiveVolumeUnit.ToString();
            }
        }

        private void cpEtkTankMass_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent();
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            var temp = GetTempInCorrectUnits();
            var pressure = GetPressureValueInCorrectUnits();
            var volume = GetVolumeValueInCorrectUnits();

            var physApi = new PhysicsInterface();
            bool status = physApi.ComputeTankMass(temp, pressure, volume, out string statusMsg, out var mass);

            if (!status || mass == null)
            {
                massInput.Text = "Error";
                MessageBox.Show(statusMsg);
            }
            else
            {
                var correctedMass = massUnitSelector.ConvertValue(MassUnit.Kilogram, _mActiveMassUnit, (double) mass);
                massInput.Text = correctedMass.ToString();
            }

        }

        private double PutResultIntoUserUnits()
        {
            Enum destEnum = _mActiveMassUnit;
            var result = massUnitSelector.ConvertValue(MassUnit.Kilogram, _mActiveMassUnit, _mMassValueInKg);
            return result;
        }

        private double GetVolumeValueInCorrectUnits()
        {
            var oldUnit = UnitParser.ParseVolumeUnit((string) tankVolumeUnitSelector.SelectedItem);
            var newUnit = VolumeUnit.CubicMeter;
            ;
            return tankVolumeUnitSelector.ConvertValue(oldUnit, newUnit, _mVolumeValue);
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

        private MassUnit _mActiveMassUnit = MassUnit.Kilogram;

        private void massUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (massUnitSelector.SelectedItem != null)
            {
                _mActiveMassUnit = UnitParser.ParseMassUnit((string) massUnitSelector.SelectedItem);
                var valueInUserUnits = PutResultIntoUserUnits();

                Settings.Default.TMMassUnit = _mActiveMassUnit.ToString();
                massInput.Text = "" + valueInUserUnits;
            }
        }

        private void volumeInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(volumeInput.Text, out _mVolumeValue);
            CheckFormValid();
        }

        public void CheckFormValid()
        {
            bool formReady;
            if (FluidPhase.DisplayTemperature())
            {
                temperatureInput.Enabled = true;
                temperatureUnitSelector.Enabled = true;
            }
            else
            {
                temperatureInput.Enabled = false;
                temperatureUnitSelector.Enabled = false;
                temperatureInput.Text = "";
            }
            if (StateContainer.FuelPhaseIsSaturated())
            {
                formReady = ParseUtility.IsParseableNumber(volumeInput.Text) &&
                             ParseUtility.IsParseableNumber(pressureInput.Text);
            }
            else
            {
                // gas phase requires all inputs
                formReady = ParseUtility.IsParseableNumber(volumeInput.Text) &&
                             ParseUtility.IsParseableNumber(temperatureInput.Text) &&
                             ParseUtility.IsParseableNumber(pressureInput.Text);
            }

            calculateButton.Enabled = formReady;
        }

        private void massInput_TextChanged(object sender, EventArgs e)
        {
            ShowOrHideMassControls();
        }

        private void ShowOrHideMassControls()
        {
            var showIt = massInput.Text.Length >= 0;
            massInput.Visible = showIt;
            massUnitSelector.Visible = showIt;
            massInputLabel.Visible = showIt;
        }

        private void temperatureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(temperatureInput.Text, out _mTemperatureValue);
            CheckFormValid();
        }

        private void pressureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(pressureInput.Text, out _mPressureValue);
            CheckFormValid();
        }

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var phase = fuelPhaseSelector.SelectedItem;
            StateContainer.SetValue("ReleaseFluidPhase", phase);
            // Deactivate temp input for saturated phases
            CheckFormValid();
        }
    }
}