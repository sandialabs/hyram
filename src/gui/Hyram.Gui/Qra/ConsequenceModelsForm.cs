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
    public partial class ConsequenceModelsForm : UserControl
    {
        private const string PsVariableName = "Peak overpressure";
        private const string ImpulseVariableName = "Impulse";

        public ConsequenceModelsForm()
        {
            InitializeComponent();
            var db = StateContainer.Instance;
            notionalNozzleSelector.DataSource = db.NozzleModels;
            notionalNozzleSelector.SelectedItem = StateContainer.GetValue<NozzleModel>("NozzleModel");

            thermalProbitSelector.DataSource = db.ThermalProbitModels;
            thermalProbitSelector.SelectedItem = StateContainer.GetValue<ThermalProbitModel>("ThermalProbit");

            overpressureProbitSelector.DataSource = db.OverpressureProbitModels;
            overpressureProbitSelector.SelectedItem =
                StateContainer.GetValue<OverpressureProbitModel>("OverpressureProbit");

            radiativeSourceSelector.DataSource = Enum.GetValues(typeof(RadiativeSourceModels));
            radiativeSourceSelector.SelectedItem = StateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel");

            // fill Cfd unit selection
            var pressureUnits = PressureUnit.Pa.GetType().GetEnumValues();
            var pressureUnitObjects = new object[pressureUnits.GetLength(0)];
            for (var index = 0; index < pressureUnitObjects.Length; index++)
                pressureUnitObjects[index] = pressureUnits.GetValue(index);
            consequenceInputUnitSelector.Items.AddRange(pressureUnitObjects);

            // Set CFD unit
            for (var index = 0; index < consequenceInputUnitSelector.Items.Count; index++)
            {
                var unitFound = consequenceInputUnitSelector.Items[index].ToString();
                if (unitFound == StateContainer.Instance.CfdPressureUnit.ToString())
                {
                    consequenceInputUnitSelector.SelectedIndex = index;
                    break;
                }
            }

            // Fill exposure time unit selector
            var exposureTimeUnit = StateContainer.Instance.ExposureTimeUnit;
            var defaultIndex = 0;
            var timeUnits = exposureTimeUnit.GetType().GetEnumValues();
            var timeUnitObjects = new object[timeUnits.GetLength(0)];
            for (var index = 0; index < timeUnitObjects.Length; index++)
            {
                timeUnitObjects[index] = timeUnits.GetValue(index);
                if (timeUnitObjects[index].ToString() == exposureTimeUnit.ToString())
                    defaultIndex = index;
            }
            exposureTimeUnitSelector.Items.AddRange(timeUnitObjects);
            exposureTimeUnitSelector.SelectedIndex = defaultIndex;

            StateContainer.SetValue("ResultsAreStale", true);
        }

        private void AddCfdInputRow(string variableName, double[] variableValues, PressureUnit unit)
        {
            var input = new object[variableValues.Length + 1];
            input[0] = variableName;
            for (var index = 0; index < variableValues.Length; index++)
                input[index + 1] = variableValues[index].ToString("F4");

            consequenceInputGrid.Rows.Add(input);
        }

        private void consequenceInputUnitSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse<PressureUnit>(consequenceInputUnitSelector.SelectedItem.ToString(), out var parsedUnit))
            {
                StateContainer.Instance.CfdPressureUnit = parsedUnit;
                var ps = ReadOverpressureConsequences(StateContainer.Instance.CfdPressureUnit);
                var impulse = ReadImpulse(StateContainer.Instance.CfdPressureUnit);

                // Fill grid
                consequenceInputGrid.Rows.Clear();
                AddCfdInputRow(PsVariableName, ps, StateContainer.Instance.CfdPressureUnit);
                AddCfdInputRow(ImpulseVariableName, impulse, StateContainer.Instance.CfdPressureUnit);
            }
            else
            {
                MessageBox.Show("Could not parse selected unit.");
            }
        }

        private double[] ReadOverpressureConsequences(PressureUnit unit)
        {
            return StateContainer.Instance.GetStateDefinedValueObject("OverpressureConsequences").GetValue(unit);
        }
        private void WriteOverpressureConsequences(PressureUnit unit, double[] values)
        {
            var valueObj = new ConvertibleValue(StockConverters.PressureConverter, unit, values);
            StateContainer.SetValue("OverpressureConsequences", valueObj);
        }

        private double[] ReadImpulse(PressureUnit unit)
        {
            return StateContainer.Instance.GetStateDefinedValueObject("Impulses").GetValue(unit);
        }
        private void WriteImpulse(PressureUnit unit, double[] values)
        {
            var valueObj = new ConvertibleValue(StockConverters.PressureConverter, unit, values);
            StateContainer.SetValue("Impulses", valueObj);
        }

        private void consequenceInputGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;
            if (rowIndex > -1)
            {
                var currentRow = consequenceInputGrid.Rows[rowIndex];
                var variableNameCell = currentRow.Cells[0];
                var variableName = variableNameCell.Value.ToString();

                if (variableName != PsVariableName && variableName != ImpulseVariableName)
                {
                    MessageBox.Show("Unknown variable name of " + variableName);
                }
                else
                {
                    var rowValues = GetCdfInputValuesFromGrid(currentRow, out var canUse);
                    if (canUse)
                    {
                        if (variableName == PsVariableName)
                            WriteOverpressureConsequences(StateContainer.Instance.CfdPressureUnit, rowValues);
                        else if (variableName == ImpulseVariableName)
                            WriteImpulse(StateContainer.Instance.CfdPressureUnit, rowValues);
                        else
                            MessageBox.Show("Parameter type " + variableNameCell + " not recognized.");
                    }
                    else
                    {
                        MessageBox.Show("Some input values were invalid. Database not updated.");
                    }
                }
            }
        }

        private double[] GetCdfInputValuesFromGrid(DataGridViewRow currentRow, out bool allValuesGood)
        {
            allValuesGood = true;

            var result = new double[currentRow.Cells.Count - 1];
            for (var colIndex = 1; colIndex < currentRow.Cells.Count; colIndex++)
            {
                var thevalue = double.NaN;
                var strValue = currentRow.Cells[colIndex].Value.ToString();
                if (ParseUtility.TryParseDouble(strValue, out thevalue))
                    result[colIndex - 1] = thevalue;
                else
                    allValuesGood = false;
            }

            return result;
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

        private void radiativeSourceSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetValue("RadiativeSourceModel", radiativeSourceSelector.SelectedValue);
        }
    }
}