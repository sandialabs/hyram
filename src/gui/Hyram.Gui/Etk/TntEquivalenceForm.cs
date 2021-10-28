/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class TntEquivalenceForm : UserControl
    {
        private MassUnit _vaporMassDisplayUnit = MassUnit.Kilogram;
        private SpecificEnergyUnit _specificEnergyDisplayUnit = SpecificEnergyUnit.JoulePerKilogram;
        private MassUnit _tntMassDisplayUnit = MassUnit.Kilogram;
        // values stored with standard units at all times
        private double _heatOfCombustion = 120000;  // kJ/kg. NOTE: no longer used in 4.0
        private double _vaporMass = double.NaN;  // kg
        private double _tntMass = double.NaN;  // kg
        private double _mYieldPercentage = double.NaN; // [0 to 100] Needs to be converted to fraction before call

        public TntEquivalenceForm()
        {
            InitializeComponent();
        }

        private void cpEtkTNTMassEquiv_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent(sender, e);
            netHeatInput.Visible = false;
            netHeatLabel.Visible = false;
            netHeatUnitSelector.Visible = false;
        }

        private MassUnit GetDefaultActiveMassUnit()
        {
            return MassUnit.Kilogram;
        }


        private void ProcessLoadEvent(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                netHeatUnitSelector.Converter = StockConverters.GetConverterByName("SpecificEnergy");
                //_specificEnergyDisplayUnit = GetDefaultActiveSpecificEnergyUnit();
                _specificEnergyDisplayUnit = SpecificEnergyUnit.KiloJoulePerKilogram;
                netHeatUnitSelector.SelectedItem = _specificEnergyDisplayUnit;

                vaporMassUnitSelector.Converter = StockConverters.GetConverterByName("Mass");
                vaporMassUnitSelector.SelectedItem = GetDefaultActiveMassUnit();

                equivalentMassUnitSelector.Converter = StockConverters.GetConverterByName("Mass");
                equivalentMassUnitSelector.SelectedItem = GetDefaultActiveMassUnit();
            }
        }

        public void CheckFormValid()
        {
            bool formReady;
            formReady = ParseUtility.IsParseableNumber(yieldInput.Text) &&
                        ParseUtility.IsParseableNumber(vaporMassInput.Text);

            calculateButton.Enabled = formReady;
        }


        private void equivalentMassUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (equivalentMassUnitSelector.SelectedItem != null)
            {
                _tntMassDisplayUnit = UnitParser.ParseMassUnit((string) equivalentMassUnitSelector.SelectedItem);
                double displayValue = equivalentMassUnitSelector.ConvertValue(MassUnit.Kilogram, _tntMassDisplayUnit, _tntMass);
                if (!double.IsNaN(displayValue))
                    equivalentMassOutput.Text = ParseUtility.DoubleToString(displayValue, "E4");
            }
        }

        private void vaporMassUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (vaporMassUnitSelector.SelectedItem != null)
            {
                _vaporMassDisplayUnit = UnitParser.ParseMassUnit((string) vaporMassUnitSelector.SelectedItem);
                double displayValue = vaporMassUnitSelector.ConvertValue(MassUnit.Kilogram, _vaporMassDisplayUnit, _vaporMass);
                if (!double.IsNaN(displayValue))
                    vaporMassInput.Text = ParseUtility.DoubleToString(displayValue);
            }
        }


        private void netHeatUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (netHeatUnitSelector.SelectedItem != null)
            {
                _specificEnergyDisplayUnit =
                    UnitParser.ParseSpecificEnergyUnit((string) netHeatUnitSelector.SelectedItem);

                double displayValue = netHeatUnitSelector.ConvertValue(SpecificEnergyUnit.KiloJoulePerKilogram,
                        _specificEnergyDisplayUnit, _heatOfCombustion);
                if (!double.IsNaN(displayValue)) netHeatInput.Text = ParseUtility.DoubleToString(displayValue);
            }
        }

        private void vaporMassInput_TextChanged(object sender, EventArgs e)
        {
            _vaporMass = double.NaN;
            double newValue = double.NaN;
            ParseUtility.TryParseDouble(vaporMassInput.Text, out newValue);
            _vaporMass = vaporMassUnitSelector.ConvertValue(_vaporMassDisplayUnit, MassUnit.Kilogram, newValue);
            CheckFormValid();
        }

        private void temperatureInput_TextChanged(object sender, EventArgs e)
        {
            _heatOfCombustion = double.NaN;
            double newValue = double.NaN;
            ParseUtility.TryParseDouble(netHeatInput.Text, out newValue);
            _heatOfCombustion = netHeatUnitSelector.ConvertValue(
                _specificEnergyDisplayUnit, SpecificEnergyUnit.KiloJoulePerKilogram, newValue);
            CheckFormValid();
        }


        private void tbYieldPercentage_TextChanged(object sender, EventArgs e)
        {
            _mYieldPercentage = double.NaN;
            ParseUtility.TryParseDouble(yieldInput.Text, out _mYieldPercentage);
            CheckFormValid();
        }

        private void calculateButton_Click(object sender, EventArgs e)
        {
            var fail = double.IsNaN(_vaporMass) || double.IsNaN(_mYieldPercentage);

            if (!fail)
            {
                var physApi = new PhysicsInterface();
                bool status = physApi.ComputeTntEquivalence(_vaporMass, _mYieldPercentage, out string statusMsg, out var mass);

                if (!status || mass == null)
                {
                    equivalentMassOutput.Text = "Error";
                    MessageBox.Show(statusMsg);
                }
                else
                {
                    _tntMass = (double)mass;
                    var displayMass = equivalentMassUnitSelector.ConvertValue(
                        MassUnit.Kilogram, _tntMassDisplayUnit, _tntMass);
                    equivalentMassOutput.Text = displayMass.ToString("N2");
                }
            }
            else
            {
                equivalentMassOutput.Text = "NaN";
            }
        }
    }
}