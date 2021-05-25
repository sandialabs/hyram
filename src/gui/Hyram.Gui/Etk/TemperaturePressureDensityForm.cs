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
    public partial class TemperaturePressureDensityForm : UserControl
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

        public TemperaturePressureDensityForm()
        {
            InitializeComponent();
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

        private void SetRadiobuttonSavedSettings()
        {
            densitySelector.Checked = Settings.Default.TPDDensityControl;
            densitySelector.Refresh();
            pressureSelector.Checked = Settings.Default.TPDPressureControl;
            pressureSelector.Refresh();
            temperatureSelector.Checked = Settings.Default.TPDTempControl;
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
            SetCalcButtonProperties();
        }

        private void temperatureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(temperatureInput.Text, out _mTemperatureValue);
            SetCalcButtonProperties();
        }

        private void pressureInput_TextChanged(object sender, EventArgs e)
        {
            ParseUtility.TryParseDouble(pressureInput.Text, out _mPressureValue);
            SetCalcButtonProperties();
        }

        private TempUnit GetDefaultActiveTempUnit()
        {
            return UnitParser.ParseTempUnit(Settings.Default.TPDTempUnit);
        }

        private void CalcOptionRbCheckedChanged(object sender, EventArgs e)
        {
            SetTextboxEnabled();
            Settings.Default.TPDDensityControl = densitySelector.Checked;
            Settings.Default.TPDPressureControl = pressureSelector.Checked;
            Settings.Default.TPDTempControl = temperatureSelector.Checked;
        }

        private void SetTextboxEnabled()
        {
            var densEnabled = !densitySelector.Checked;
            var presEnabled = !pressureSelector.Checked;
            var tempEnabled = !temperatureSelector.Checked;

            if (tempEnabled && presEnabled)
                _mCalculationOption = CalculationOption.CalculateDensity;
            else if (tempEnabled && densEnabled)
                _mCalculationOption = CalculationOption.CalculatePressure;
            else if (presEnabled && densEnabled)
                _mCalculationOption = CalculationOption.CalculateTemperature;
            else
                MessageBox.Show(@"Unable to determine calculation option.");

            if (densityInput.Enabled != densEnabled) densityInput.Enabled = densEnabled;

            if (pressureInput.Enabled != presEnabled) pressureInput.Enabled = presEnabled;

            if (temperatureInput.Enabled != tempEnabled) temperatureInput.Enabled = tempEnabled;

            SetCalcButtonProperties();
        }

        private void SetCalcButtonProperties()
        {
            string leftValue, rightValue;

            switch (_mCalculationOption)
            {
                case CalculationOption.CalculateDensity:
                    leftValue = temperatureInput.Text;
                    rightValue = pressureInput.Text;
                    submitButton.Text = "Calculate Density";
                    break;
                case CalculationOption.CalculatePressure:
                    leftValue = temperatureInput.Text;
                    rightValue = densityInput.Text;
                    submitButton.Text = "Calculate Pressure";
                    break;
                case CalculationOption.CalculateTemperature:
                    leftValue = densityInput.Text;
                    rightValue = pressureInput.Text;
                    submitButton.Text = "Calculate Temperature";
                    break;
                default:
                    submitButton.Text = "Unknown Option Selected";
                    throw new Exception("Calculation option of " + _mCalculationOption + " unknown.");
            }

            var enableButton = false;
            if (ParseUtility.IsParseableNumber(leftValue) && ParseUtility.IsParseableNumber(rightValue))
                enableButton = true;

            submitButton.Enabled = enableButton;
        }

        /// <summary>
        ///     Compute missing parameter via python call
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calculateButton_Click(object sender, EventArgs e)
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

            var physApi = new PhysicsInterface();
            bool status = physApi.ComputeTpd(temp, pressure, density, out string statusMsg, out double? result);

            if (!status)
            {
                MessageBox.Show(statusMsg);
            }
            else
            {
                var valueToUse = double.NaN;
                switch (_mCalculationOption)
                {
                    case CalculationOption.CalculateDensity:
                        valueToUse =
                            densityUnitSelector.ConvertValue(DensityUnit.KilogramPerCubicMeter, _mActiveDensityUnit, (double)result);
                        break;
                    case CalculationOption.CalculatePressure:
                        valueToUse = pressureUnitSelector.ConvertValue(PressureUnit.Pa, _mActivePressureUnit, (double)result);
                        break;
                    case CalculationOption.CalculateTemperature:
                        valueToUse = temperatureUnitSelector.ConvertValue(TempUnit.Kelvin, _mActiveTempUnit, (double)result);
                        break;
                }

                var resultContainer = GetResultContainer();
                resultContainer.Text = ParseUtility.DoubleToString(valueToUse);
            }
        }

        private TextBox GetResultContainer()
        {
            TextBox[] candidates = {densityInput, pressureInput, temperatureInput};
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

        private void ProcessLoadEvent(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                SetRadiobuttonSavedSettings();

                temperatureUnitSelector.Converter = StockConverters.GetConverterByName("Temperature");
                _mActiveTempUnit = GetDefaultActiveTempUnit();
                temperatureUnitSelector.SelectedItem = _mActiveTempUnit;

                _mActivePressureUnit = GetDefaultActivePressureUnit();
                pressureUnitSelector.Converter = StockConverters.GetConverterByName("Pressure");
                pressureUnitSelector.SelectedItem = _mActivePressureUnit;

                _mActiveDensityUnit = GetDefaultActiveDensityUnit();

                densityUnitSelector.Converter = StockConverters.GetConverterByName("Density");
                densityUnitSelector.SelectedItem = _mActiveDensityUnit;

                SetTextboxEnabled();
            }
        }


        private PressureUnit _mActivePressureUnit = PressureUnit.Pa;
        private TempUnit _mActiveTempUnit = TempUnit.Kelvin;
        private DensityUnit _mActiveDensityUnit = DensityUnit.KilogramPerCubicMeter;

        private void cpEtkTempPressureDensity_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent(sender, e);
        }

    }
}