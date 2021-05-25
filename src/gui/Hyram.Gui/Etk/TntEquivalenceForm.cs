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
    public partial class TntEquivalenceForm : UserControl
    {
        private MassUnit _vaporMassDisplayUnit = MassUnit.Kilogram;
        private SpecificEnergyUnit _specificEnergyDisplayUnit = SpecificEnergyUnit.JoulePerKilogram;
        private MassUnit _tntMassDisplayUnit = MassUnit.Kilogram;
        // values stored with standard units at all times
        private double _heatOfCombustion = 120000;  // kJ/kg
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
                        ParseUtility.IsParseableNumber(vaporMassInput.Text) &&
                        ParseUtility.IsParseableNumber(netHeatInput.Text);

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
            var fail = double.IsNaN(_vaporMass) || double.IsNaN(_mYieldPercentage) ||
                       double.IsNaN(_heatOfCombustion);

            if (!fail)
            {
                var physApi = new PhysicsInterface();
                bool status = physApi.ComputeTntEquivalence(_vaporMass, _mYieldPercentage, _heatOfCombustion, out string statusMsg, out var mass);

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
                    equivalentMassOutput.Text = displayMass.ToString();
                }
            }
            else
            {
                equivalentMassOutput.Text = "NaN";
            }
        }
    }
}