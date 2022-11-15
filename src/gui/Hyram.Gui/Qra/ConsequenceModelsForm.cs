/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class ConsequenceModelsForm : AnalysisForm
    {
        private StateContainer _state = State.Data;

        public ConsequenceModelsForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
            RefreshForm();
        }

        public sealed override void RefreshForm()
        {
            _state = State.Data;
            var method = _state.SelectedOverpressureMethod;

            flameSpeedSelector.Enabled = method == _state.BstMethod;
            flameSpeedLabel.Enabled = method == _state.BstMethod;

            tntLabel.Enabled = method == _state.TntMethod;
            tntInput.Enabled = method == _state.TntMethod;
            tntInput.Text = _state.TntEquivalenceFactor.GetValue().ToString("F2");

            notionalNozzleSelector.DataSource = _state.NozzleModels;
            notionalNozzleSelector.SelectedItem = _state.Nozzle;

            overpMethodSelector.DataSource = _state.OverpressureMethods;
            overpMethodSelector.SelectedItem = _state.SelectedOverpressureMethod;

            flameSpeedSelector.DataSource = _state.MachFlameSpeeds;
            flameSpeedSelector.SelectedItem = _state.OverpressureFlameSpeed;

            thermalProbitSelector.DataSource = _state.ThermalProbitModels;
            thermalProbitSelector.SelectedItem = _state.ThermalProbit;

            overpressureProbitSelector.DataSource = _state.OverpressureProbitModels;
            overpressureProbitSelector.SelectedItem = _state.OverpressureProbit;

            // Fill exposure time unit selector
            var exposureTimeUnit = _state.ExposureTimeUnit;
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
            exposureTimeUnitSelector.Items.Clear();
            exposureTimeUnitSelector.Items.AddRange(timeUnitObjects);
            exposureTimeUnitSelector.SelectedIndex = defaultIndex;

            CheckFormValid();
        }

        public override void CheckFormValid()
        {
            Alert = AlertLevel.AlertNull;
            AlertMessage = "";

            // Disallow Bauwens overpressure model with TNO overpressure probits
            if (_state.SelectedOverpressureMethod == _state.BauwensMethod)
            {
                if (_state.OverpressureProbit == _state.ProbitCollapse || _state.OverpressureProbit == _state.ProbitHead)
                {
                    Alert = AlertLevel.AlertError;
                    AlertMessage =
                        "Overpressure method 'Bauwens' does not produce impulse values and cannot be used with " +
                        "overpressure probit models 'TNO - Head Impact' or 'TNO - Structural Collapse'";
                }

            }

            formWarning.Visible = Alert != AlertLevel.AlertNull;
            formWarning.Text = AlertMessage;
            formWarning.BackColor = _state.AlertBackColors[(int)Alert];
            formWarning.ForeColor = _state.AlertTextColors[(int)Alert];

            MainForm.NotifyOfChildPublicStateChange();
        }

        private void exposureTimeInput_TextChanged(object sender, EventArgs e)
        {
            var parsedValue = double.NaN;
            if (double.TryParse(exposureTimeInput.Text, out parsedValue))
            {
                _state.FlameExposureTime.SetValue(parsedValue);
            }
        }

        private void exposureTimeUnitSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            var iValue = GetExposureTimeUnitFromDropdown();

            if (iValue != null)
            {
                _state.ExposureTimeUnit = (TimeUnit) Enum.Parse(_state.ExposureTimeUnit.GetType(), iValue.ToString());
            }

            exposureTimeInput.Text = _state.FlameExposureTime.GetValue().ToString("F4");
        }

        private TimeUnit? GetExposureTimeUnitFromDropdown()
        {
            TimeUnit? result = null;
            var selectedItemName = exposureTimeUnitSelector.Items[exposureTimeUnitSelector.SelectedIndex].ToString();

            if (Enum.TryParse<TimeUnit>(selectedItemName, out var iResult)) result = iResult;

            return result;
        }

        private void notionalNozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Nozzle = (ModelPair)notionalNozzleSelector.SelectedItem;
        }

        private void thermalProbitSelector_SelectionChangeCommotted(object sender, EventArgs e)
        {
            _state.ThermalProbit = (ModelPair)thermalProbitSelector.SelectedItem;
            CheckFormValid();
        }

        private void overpressureProbitSelector_SelectionChangeCommotted(object sender, EventArgs e)
        {
            _state.OverpressureProbit = (ModelPair)overpressureProbitSelector.SelectedItem;
            CheckFormValid();
        }

        private void overpMethodSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.SelectedOverpressureMethod = (ModelPair) overpMethodSelector.SelectedItem;
            CheckFormValid();
            RefreshForm();
        }

        private void flameSpeedSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            double speed = (double)flameSpeedSelector.SelectedItem;
            _state.OverpressureFlameSpeed = speed;
        }

        private void tntInput_TextChanged(object sender, EventArgs e)
        {
            double val = double.Parse(tntInput.Text);
            _state.TntEquivalenceFactor.SetValue(val);
        }
    }
}