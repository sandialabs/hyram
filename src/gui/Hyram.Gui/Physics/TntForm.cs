/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class TntForm : UserControl
    {
        private readonly StateContainer _state = State.Data;
        private string _statusMsg;
        private bool _status;
        private double? _result;

        private MassUnit _vaporMassDisplayUnit = MassUnit.Kilogram;
        private MassUnit _tntMassDisplayUnit = MassUnit.Kilogram;
        // values stored with standard units at all times
        private double _vaporMass = double.NaN;  // kg
        private double _tntMass = double.NaN;  // kg
        private double _mYieldPercentage = double.NaN; // [0 to 100] Needs to be converted to fraction before call

        public TntForm()
        {
            InitializeComponent();
        }

        private void TntForm_Load(object sender, EventArgs e)
        {
            spinnerPictureBox.Hide();
            warningMsg.Hide();

            warningMsg.BackColor = _state.AlertBackColors[(int)AlertLevel.AlertError];
            warningMsg.ForeColor = _state.AlertTextColors[(int)AlertLevel.AlertError];

            VaporMassUnitSelector.Converter = Converters.GetConverterByName("Mass");
            VaporMassUnitSelector.SelectedItem = MassUnit.Kilogram;

            EqMassUnitSelector.Converter = Converters.GetConverterByName("Mass");
            EqMassUnitSelector.SelectedItem = MassUnit.Kilogram;

            CheckFormValid();
        }

        public void CheckFormValid()
        {
            warningMsg.Hide();
            string msg = "";

            bool formReady = true;

            if (string.IsNullOrEmpty(YieldInput.Text) || string.IsNullOrEmpty(VaporMassInput.Text))
            {
                formReady = false;
                msg = "";
            }
            else if (!ParseUtility.IsParseableNumber(YieldInput.Text) || !ParseUtility.IsParseableNumber(VaporMassInput.Text))
            {
                formReady = false;
                msg = "Enter a valid number for mass and yield";
            }
            else if (double.TryParse(YieldInput.Text, out var yield) && (yield <= 0 || yield > 100))
            {
                formReady = false;
                msg = "Enter yield as a percentage between 0 and 100";
            }

            warningMsg.Text = msg;
            SubmitBtn.Enabled = formReady;

            if (!string.IsNullOrEmpty(msg))
            {
                warningMsg.Show();
            }
        }

        private void EqMassUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (EqMassUnitSelector.SelectedItem != null)
            {
                _tntMassDisplayUnit = UnitParser.ParseMassUnit((string) EqMassUnitSelector.SelectedItem);
                double displayValue = EqMassUnitSelector.ConvertValue(MassUnit.Kilogram, _tntMassDisplayUnit, _tntMass);
                if (!double.IsNaN(displayValue))
                {
                    EqMassOutput.Text = displayValue.ToString("E4");
                }
            }
        }

        private void VaporMassUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (VaporMassUnitSelector.SelectedItem != null)
            {
                _vaporMassDisplayUnit = UnitParser.ParseMassUnit((string) VaporMassUnitSelector.SelectedItem);
                double displayValue = VaporMassUnitSelector.ConvertValue(MassUnit.Kilogram, _vaporMassDisplayUnit, _vaporMass);
                if (!double.IsNaN(displayValue)) VaporMassInput.Text = displayValue.ToString();
            }
        }

        private void VaporMassInput_TextChanged(object sender, EventArgs e)
        {
            _vaporMass = double.NaN;
            double newValue = double.NaN;
            double.TryParse(VaporMassInput.Text, out newValue);
            _vaporMass = VaporMassUnitSelector.ConvertValue(_vaporMassDisplayUnit, MassUnit.Kilogram, newValue);
            CheckFormValid();
        }

        private void YieldInput_TextChanged(object sender, EventArgs e)
        {
            _mYieldPercentage = double.NaN;
            double.TryParse(YieldInput.Text, out _mYieldPercentage);
            CheckFormValid();
        }

        private async void SubmitBtn_Click(object sender, EventArgs e)
        {
            spinnerPictureBox.Show();
            warningMsg.Hide();
            SubmitBtn.Enabled = false;

            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void Execute()
        {
            if (double.IsNaN(_vaporMass) || double.IsNaN(_mYieldPercentage))
            {
                EqMassOutput.Text = "NaN";
                return;
            }

            var physApi = new PhysicsInterface();
            _status = physApi.ComputeTntEquivalence(_vaporMass, _mYieldPercentage, out _statusMsg, out _result);
        }

        private void DisplayResults()
        {
            spinnerPictureBox.Hide();
            SubmitBtn.Enabled = true;
            if (!_status || _result == null)
            {
                EqMassOutput.Text = "Error";
                MessageBox.Show(_statusMsg);
            }
            else
            {
                _tntMass = (double)_result;
                var displayMass = EqMassUnitSelector.ConvertValue(MassUnit.Kilogram, _tntMassDisplayUnit, _tntMass);
                EqMassOutput.Text = displayMass.ToString("N2");
            }
        }
    }
}