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
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class MassFlowRateForm : UserControl
    {
        private DistanceUnit _mActiveOrificeDiamDistUnit = DistanceUnit.Meter;
        private PressureUnit _mActivePressureUnit = PressureUnit.Pa;
        private TempUnit _mActiveTempUnit = TempUnit.Kelvin;
        private VolumeUnit _mActiveVolumeUnit = VolumeUnit.Liter;
        private double _mOrificeDiameterValue = double.NaN;
        private double _mPressureValue = double.NaN;
        private double _mTemperatureValue = double.NaN;
        private double _mVolumeValue = double.NaN;

        private void MassFlowRateForm_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                fuelPhaseSelector.DataSource = StateContainer.Instance.FluidPhases;
                fuelPhaseSelector.SelectedItem = StateContainer.GetValue<FluidPhase>("ReleaseFluidPhase");

                temperatureUnitSelector.Converter = StockConverters.GetConverterByName("Temperature");
                _mActiveTempUnit = UnitParser.ParseTempUnit(Settings.Default.MFRTempUnit);
                temperatureUnitSelector.SelectedItem = _mActiveTempUnit;

                _mActivePressureUnit = UnitParser.ParsePressureUnit(Settings.Default.MFRPressureUnit);
                pressureUnitSelector.Converter = StockConverters.GetConverterByName("Pressure");
                pressureUnitSelector.SelectedItem = _mActivePressureUnit;

                _mActiveVolumeUnit = UnitParser.ParseVolumeUnit(Settings.Default.MFRVolumeUnit);
                tankVolumeUnitSelector.Converter = StockConverters.GetConverterByName("Volume");
                tankVolumeUnitSelector.SelectedItem = _mActiveVolumeUnit;

                _mActiveOrificeDiamDistUnit =
                    UnitParser.ParseDistanceUnit(Settings.Default.MFROrificeDiamDistUnit);
                orificeDiameterUnitSelector.Converter = StockConverters.GetConverterByName("Distance");
                orificeDiameterUnitSelector.SelectedItem = _mActiveOrificeDiamDistUnit;
                _mOrificeDiameterValue = 3.0e-2;
                orificeDiameterInput.Text = "" + _mOrificeDiameterValue;

                CheckFormValid();
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

        public MassFlowRateForm()
        {
            InitializeComponent();
        }

        private bool SteadyBlowdown
        {
            get => isSteadySelector.Checked;
            set => isSteadySelector.Checked = value;
        }

        /// <summary>
        /// Verify inputs and enable submission button.
        /// </summary>
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
                formReady = (ParseUtility.IsParseableNumber(volumeInput.Text) &&
                             ParseUtility.IsParseableNumber(pressureInput.Text) &&
                             ParseUtility.IsParseableNumber(orificeDiameterInput.Text));
            }
            else
            {
                formReady = (ParseUtility.IsParseableNumber(volumeInput.Text) &&
                             ParseUtility.IsParseableNumber(temperatureInput.Text) &&
                             ParseUtility.IsParseableNumber(pressureInput.Text) &&
                             ParseUtility.IsParseableNumber(orificeDiameterInput.Text));
            }
            calculateButton.Enabled = formReady;
        }


        private void ReleaseTypeChanged(object sender, EventArgs e)
        {
            if (SteadyBlowdown)
            {
                volumeInput.Text = "0.0";
                volumeInput.Enabled = false;
            }
            else
            {
                volumeInput.Enabled = true;
            }
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            var temp = temperatureUnitSelector.ConvertValue(
                UnitParser.ParseTempUnit((string) temperatureUnitSelector.SelectedItem), 
                TempUnit.Kelvin,
                _mTemperatureValue);

            var pressure = pressureUnitSelector.ConvertValue(
                UnitParser.ParsePressureUnit((string)pressureUnitSelector.SelectedItem),
                PressureUnit.Pa,
                _mPressureValue);

            var tankVolume = tankVolumeUnitSelector.ConvertValue(
                UnitParser.ParseVolumeUnit((string) tankVolumeUnitSelector.SelectedItem), 
                VolumeUnit.CubicMeter,
                _mVolumeValue);

            var orificeDiam = orificeDiameterUnitSelector.ConvertValue(
                UnitParser.ParseDistanceUnit((string) orificeDiameterUnitSelector.SelectedItem), 
                DistanceUnit.Meter,
                _mOrificeDiameterValue);

            var isSteady = SteadyBlowdown;
            var dischargeCoeff = 1.0;

            var physApi = new PhysicsInterface();

            bool status = physApi.ComputeFlowRateOrTimeToEmpty(
                orificeDiam, temp, pressure,
                tankVolume, isSteady, dischargeCoeff,
                out var statusMsg, out var massFlowRate, out var timeToEmpty, out var plotFileLoc
                );

            if (!status)
            {
                MessageBox.Show(statusMsg);
            }
            else
            {
                if (isSteady)
                {
                    resultLabel.Text = "Mass flow rate (kg/s)";
                    resultOutput.Text = massFlowRate != null ? ParseUtility.DoubleToString((double) massFlowRate) : "Error";
                }
                else
                {
                    resultLabel.Text = "Time to empty (s)";
                    resultImagePicture.Load(plotFileLoc);
                    resultImagePicture.Visible = true;
                    resultOutput.Text = timeToEmpty != null ? ParseUtility.DoubleToString((double) timeToEmpty) : "Error";
                }

                ParentForm.TopMost = true;
                mainTabControl.SelectedTab = outputTab;
            }
        }

        private void temperatureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(temperatureInput.Text, out _mTemperatureValue);
            CheckFormValid();
        }

        private void orificeDiameterUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (orificeDiameterUnitSelector.SelectedItem != null)
            {
                var newUnit =
                    UnitParser.ParseDistanceUnit((string) orificeDiameterUnitSelector.SelectedItem);
                _mOrificeDiameterValue = orificeDiameterUnitSelector.ConvertValue(_mActiveOrificeDiamDistUnit, newUnit,
                    _mOrificeDiameterValue);
                if (!double.IsNaN(_mOrificeDiameterValue)) orificeDiameterInput.Text = "" + _mOrificeDiameterValue;

                _mActiveOrificeDiamDistUnit = newUnit;
                Settings.Default.MFROrificeDiamDistUnit = newUnit.ToString();
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
                Settings.Default.MFRPressureUnit = newUnit.ToString();
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
                Settings.Default.MFRVolumeUnit = _mActiveVolumeUnit.ToString();
            }
        }

        private void temperatureUnitSelector_OnSelectedIndexChange(object sender, EventArgs e)
        {
            if (temperatureUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseTempUnit((string) temperatureUnitSelector.SelectedItem);
                _mTemperatureValue = temperatureUnitSelector.ConvertValue(_mActiveTempUnit, newUnit, _mTemperatureValue);
                if (!double.IsNaN(_mTemperatureValue)) temperatureInput.Text = ParseUtility.DoubleToString(_mTemperatureValue);

                _mActiveTempUnit = newUnit;
                Settings.Default.MFRTempUnit = _mActiveTempUnit.ToString();
            }
        }

        private void orificeDiameterInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(orificeDiameterInput.Text, out _mOrificeDiameterValue);
            CheckFormValid();
        }

        private void volumeInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(volumeInput.Text, out _mVolumeValue);
            CheckFormValid();
        }

        private void pressureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(pressureInput.Text, out _mPressureValue);
            CheckFormValid();
        }

        private void calculateButton_MouseMove(object sender, MouseEventArgs e)
        {
            // Image doesn't zoom correctly if set while invisible. Can hide later if needed
            if (!resultImagePicture.Visible) resultImagePicture.Visible = true;
        }

        private void inputTab_MouseMove(object sender, MouseEventArgs e)
        {
            if (!resultImagePicture.Visible) resultImagePicture.Visible = true;
        }

        private void outputTab_Enter(object sender, EventArgs e)
        {
            resultImagePicture.Visible = isBlowdownSelector.Checked;
        }

        private void fuelPhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetValue("ReleaseFluidPhase", fuelPhaseSelector.SelectedItem);
            // Deactivate temp input for saturated phases
            CheckFormValid();
        }
    }
}