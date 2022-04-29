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
    public partial class ConsequenceModelsForm : AnalysisForm
    {
        public ConsequenceModelsForm(MainForm mainForm)
        {
            MainForm = mainForm;

            InitializeComponent();
            var db = StateContainer.Instance;
            notionalNozzleSelector.DataSource = db.NozzleModels;
            notionalNozzleSelector.SelectedItem = StateContainer.GetValue<NozzleModel>("NozzleModel");

            overpMethodSelector.DataSource = StateContainer.Instance.OverpressureMethods;
            overpMethodSelector.SelectedItem =
                StateContainer.GetValue<UnconfinedOverpressureMethod>("unconfinedOverpressureMethod");

            flameSpeedSelector.DataSource = StateContainer.Instance.MachFlameSpeeds;
            flameSpeedSelector.SelectedItem =
                StateContainer.GetNdValue("overpressureFlameSpeed");

            thermalProbitSelector.DataSource = db.ThermalProbitModels;
            thermalProbitSelector.SelectedItem = StateContainer.GetValue<ThermalProbitModel>("ThermalProbit");

            overpressureProbitSelector.DataSource = db.OverpressureProbitModels;
            overpressureProbitSelector.SelectedItem =
                StateContainer.GetValue<OverpressureProbitModel>("OverpressureProbit");

            // Fill exposure time unit selector
            var exposureTimeUnit = StateContainer.Instance.ExposureTimeUnit;
            var defaultIndex = 0;
            var timeUnits = exposureTimeUnit.GetType().GetEnumValues();
            var timeUnitObjects = new object[timeUnits.GetLength(0)];
            for (var index = 0; index < timeUnitObjects.Length; index++)
            {
                timeUnitObjects[index] = timeUnits.GetValue(index);
                if (timeUnitObjects[index].ToString() == exposureTimeUnit.ToString())
                {
                    defaultIndex = index;
                }
            }
            exposureTimeUnitSelector.Items.AddRange(timeUnitObjects);
            exposureTimeUnitSelector.SelectedIndex = defaultIndex;

            StateContainer.SetValue("ResultsAreStale", true);
            CheckFormValid();
            RefreshParameterDisplay();
        }

        private void RefreshParameterDisplay()
        {
            var method = StateContainer.GetOverpressureMethod();

            flameSpeedSelector.Enabled = method == UnconfinedOverpressureMethod.BstMethod;
            flameSpeedLabel.Enabled = method == UnconfinedOverpressureMethod.BstMethod;

            tntLabel.Enabled = method == UnconfinedOverpressureMethod.TntMethod;
            tntInput.Enabled = method == UnconfinedOverpressureMethod.TntMethod;
            tntInput.Text = StateContainer.GetNdValue("tntEquivalenceFactor").ToString("F2");
        }

        public override void CheckFormValid()
        {
            AlertType = 0;
            AlertMessage = "";
            AlertDisplayed = false;

            FuelType fuel = StateContainer.GetValue<FuelType>("FuelType");

            if (fuel != FuelType.Hydrogen)
            {
                AlertDisplayed = true;
                AlertType = 1;
                AlertMessage = "Default data generated for " +
                               "high pressure gaseous hydrogen systems and may not be appropriate for the selected fuel";
            }

            // Disallow Bauwens overpressure model with TNO overpressure probits
            var unconfinedOverpMethod = StateContainer.GetValue<UnconfinedOverpressureMethod>("unconfinedOverpressureMethod");
            if (unconfinedOverpMethod == UnconfinedOverpressureMethod.BauwensMethod)
            {
                var overpProbitMethod = StateContainer.GetValue<OverpressureProbitModel>("OverpressureProbit");
                if (overpProbitMethod == OverpressureProbitModel.Collapse ||
                    overpProbitMethod == OverpressureProbitModel.Head)
                {
                    AlertDisplayed = true;
                    AlertType = 2;
                    AlertMessage =
                        "Overpressure method 'Bauwens' does not produce impulse values and cannot be used with " +
                        "overpressure probit models 'TNO - Head Impact' or 'TNO - Structural Collapse'";
                }

            }

            formWarning.Visible = AlertDisplayed;
            formWarning.Text = AlertMessage;
            formWarning.BackColor = AlertType == 1 ? MainForm.WarningBackColor : MainForm.ErrorBackColor;
            formWarning.ForeColor = AlertType == 1 ? MainForm.WarningForeColor : MainForm.ErrorForeColor;

            MainForm.NotifyOfChildPublicStateChange();
        }

        private double GetThermalExposureTime(ElapsingTimeConversionUnit unit)
        {
            // ElapsingTimeConversionUnit.Second
            return StateContainer.Instance.GetStateDefinedValueObject("flameExposureTime").GetValue(unit)[0];
        }

        private void SetThermalExposureTime(ElapsingTimeConversionUnit unit, double value)
        {
            var values = new double[1];
            values[0] = value;
            var convertibleValue =
                StateContainer.Instance.GetStateDefinedValueObject("flameExposureTime");
            convertibleValue.SetValue(unit, values);
        }

        private void notionalNozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetValue("NozzleModel", (NozzleModel) notionalNozzleSelector.SelectedItem);
        }

        private void thermalProbitSelector_SelectionChangeCommotted(object sender, EventArgs e)
        {
            StateContainer.SetValue("ThermalProbit", thermalProbitSelector.SelectedItem);
        }

        private void overpressureProbitSelector_SelectionChangeCommotted(object sender, EventArgs e)
        {
            StateContainer.SetValue("OverpressureProbit", overpressureProbitSelector.SelectedItem);
            CheckFormValid();
        }

        private void exposureTimeInput_TextChanged(object sender, EventArgs e)
        {
            var parsedValue = double.NaN;
            if (ParseUtility.TryParseDouble(exposureTimeInput.Text, out parsedValue))
                SetThermalExposureTime(StateContainer.Instance.ExposureTimeUnit, parsedValue);
        }

        private void exposureTimeUnitSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var iValue = GetExposureTimeUnitFromDropdown();

            if (iValue != null)
                StateContainer.Instance.ExposureTimeUnit =
                    (ElapsingTimeConversionUnit) Enum.Parse(StateContainer.Instance.ExposureTimeUnit.GetType(),
                        iValue.ToString());

            exposureTimeInput.Text =
                GetThermalExposureTime(StateContainer.Instance.ExposureTimeUnit).ToString("F4");
        }

        private ElapsingTimeConversionUnit? GetExposureTimeUnitFromDropdown()
        {
            ElapsingTimeConversionUnit? result = null;
            var selectedItemName =
                exposureTimeUnitSelector.Items[exposureTimeUnitSelector.SelectedIndex].ToString();

            if (Enum.TryParse<ElapsingTimeConversionUnit>(selectedItemName, out var iResult)) result = iResult;

            return result;
        }

        private void overpMethodSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string methodName = overpMethodSelector.SelectedItem.ToString();
            UnconfinedOverpressureMethod method = UnconfinedOverpressureMethod.ParseName(methodName);
            StateContainer.SetValue("UnconfinedOverpressureMethod", method);
            CheckFormValid();
            RefreshParameterDisplay();
        }

        private void flameSpeedSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            double speed = (double)flameSpeedSelector.SelectedItem;
            StateContainer.SetNdValue("overpressureFlameSpeed", UnitlessUnit.Unitless, speed);
        }

        private void tntInput_TextChanged(object sender, EventArgs e)
        {
            double val = double.Parse(tntInput.Text);
            StateContainer.SetNdValue("tntEquivalenceFactor", UnitlessUnit.Unitless, val);
        }
    }
}