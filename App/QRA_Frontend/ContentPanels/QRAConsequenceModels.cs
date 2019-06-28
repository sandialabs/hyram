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
using JrConversions;
using QRA_Frontend.Resources;
using QRAState;

namespace QRA_Frontend.ContentPanels
{
    public partial class CpConsequenceModels : UserControl, IQraBaseNotify
    {
        // TODO (Cianan): Clean up logic which uses these, then delete them
        private const string PsVariableName = "Peak Overpressure(P_s)";
        private const string ImpulseVariableName = "Impulse";

        public CpConsequenceModels()
        {
            InitializeComponent();
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
            SetupUi();
            QraStateContainer.SetValue("ResultsAreStale", true);
        }

        private void SetupUi()
        {
            ContentPanel.SetNarrative(this, Narratives.CM__ConsequenceModels);
            SetupCfdInputGrid();
            FillExposureTimeUnitsSelector();
        }

        /// <summary>
        ///     Set up data sources and load stored models as selected choices
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpConsequenceModels_Load(object sender, EventArgs e)
        {
            var qraInst = QraStateContainer.Instance;

            cbNotionalNozzleModel.DataSource = qraInst.NozzleModels;
            cbNotionalNozzleModel.SelectedItem = QraStateContainer.GetValue<NozzleModel>("NozzleModel");

            cbDeflagrationModel.DataSource = qraInst.DeflagrationModels;
            var model = QraStateContainer.GetValue<DeflagrationModel>("DeflagrationModel");
            cbDeflagrationModel.SelectedItem = model;
            UpdateCfdInput(model);

            cbThermalProbitModel.DataSource = qraInst.ThermalProbitModels;
            cbThermalProbitModel.SelectedItem = QraStateContainer.GetValue<ThermalProbitModel>("ThermalProbit");

            cbOverpressureProbitModel.DataSource = qraInst.OverpressureProbitModels;
            cbOverpressureProbitModel.SelectedItem =
                QraStateContainer.GetValue<OverpressureProbitModel>("OverpressureProbit");

            var radModel = QraStateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel");
            cbRadiativeSourceModel.Fill(UiStateRoutines.GetRadiativeSourceModelDict(), radModel);
            cbRadiativeSourceModel.SelectedItem = radModel;
        }

        /// <summary>
        ///     Parse selected string into Notional Nozzle Model and store it.
        /// </summary>
        private void cbNotionalNozzleModel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var model = (NozzleModel) cbNotionalNozzleModel.SelectedItem;
            QraStateContainer.SetValue("NozzleModel", model);
        }

        /// <summary>
        ///     Parse selected string into Deflagration Model, store it, and update CFD input display.
        /// </summary>
        private void cbDeflagrationModel_SelectionChangeCommitted(object sender, EventArgs e)
        {
            var model = (DeflagrationModel) cbDeflagrationModel.SelectedItem;
            QraStateContainer.SetValue("DeflagrationModel", model);
            UpdateCfdInput(model);
        }

        /// <summary>
        ///     Only display CFD inputs when model is CFD
        /// </summary>
        /// <param name="model"></param>
        private void UpdateCfdInput(DeflagrationModel model)
        {
            var key = model.GetKey();
            gbCFDInput.Visible = key == "cfd";
        }

        private void cbThermalProbitModel_SelectionChangeCommotted(object sender, EventArgs e)
        {
            QraStateContainer.SetValue("ThermalProbit", cbThermalProbitModel.SelectedItem);
        }

        private void cbOverpressureProbitModel_SelectionChangeCommotted(object sender, EventArgs e)
        {
            QraStateContainer.SetValue("OverpressureProbit", cbOverpressureProbitModel.SelectedItem);
        }

        private void cbRadiativeSourceModel_OnValueChanged(object sender, EventArgs e)
        {
            QraStateContainer.SetValue("RadiativeSourceModel", cbRadiativeSourceModel.SelectedItem);
        }

        private void SetupCfdInputGrid()
        {
            FillCfdUnitsSelector();
            SetCfdUnit(QraStateContainer.Instance.CfdPressureUnit);
        }

        private void SetCfdUnit(PressureUnit unitToSet)
        {
            var unitToFind = unitToSet.ToString();

            for (var index = 0; index < cbCFDUnits.Items.Count; index++)
            {
                var unitFound = cbCFDUnits.Items[index].ToString();
                if (unitFound == unitToFind)
                {
                    cbCFDUnits.SelectedIndex = index;
                    break;
                }
            }
        }

        private void FillUnitsSelector(ComboBox dropDown, Enum theUnit, bool setDefault = true)
        {
            var defaultIndex = 0;

            var theItems = theUnit.GetType().GetEnumValues();
            var items = new object[theItems.GetLength(0)];
            for (var index = 0; index < items.Length; index++)
            {
                items[index] = theItems.GetValue(index);
                if (setDefault)
                    if (items[index].ToString() == theUnit.ToString())
                        defaultIndex = index;
            }

            dropDown.Items.AddRange(items);
            dropDown.SelectedIndex = defaultIndex;
        }

        private void FillExposureTimeUnitsSelector()
        {
            var exposureTimeUnit = QraStateContainer.Instance.ExposureTimeUnit;
            FillUnitsSelector(cbThermalExposureTimeUnits, exposureTimeUnit);
        }

        private void FillCfdUnitsSelector()
        {
            var unit = PressureUnit.Pa;
            var theItems = unit.GetType().GetEnumValues();
            var items = new object[theItems.GetLength(0)];
            for (var index = 0; index < items.Length; index++) items[index] = theItems.GetValue(index);

            cbCFDUnits.Items.AddRange(items);
        }

        private void AddCfdInputRow(string variableName, double[] variableValues, PressureUnit unit)
        {
            var input = new object[variableValues.Length + 1];
            input[0] = variableName;
            for (var index = 0; index < variableValues.Length; index++)
                input[index + 1] = variableValues[index].ToString("F4");

            dgCFDInput.Rows.Add(input);
        }

        private void cbCFDUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Enum.TryParse<PressureUnit>(cbCFDUnits.SelectedItem.ToString(), out var parsedUnit))
            {
                QraStateContainer.Instance.CfdPressureUnit = parsedUnit;
                FillCfdGrid();
            }
            else
            {
                MessageBox.Show("Could not parse selected unit.");
            }
        }

        private double[] ReadPs(PressureUnit unit)
        {
            // PressureUnit.Pa
            return QraStateContainer.Instance.GetStateDefinedValueObject("P_s").GetValue(unit);
        }

        private void WritePs(PressureUnit unit, double[] values)
        {
            var valueObj = new NdConvertibleValue(StockConverters.PressureConverter, unit, values);
            QraStateContainer.SetValue("P_s", valueObj);
        }

        private void WriteImpulse(PressureUnit unit, double[] values)
        {
            var valueObj = new NdConvertibleValue(StockConverters.PressureConverter, unit, values);
            QraStateContainer.SetValue("impulse", valueObj);
            //QraStateContainer.Instance.Parameters["impulse"] = ValueObj;
        }

        private double[] ReadImpulse(PressureUnit unit)
        {
            // PressureUnit.Pa
            return QraStateContainer.Instance.GetStateDefinedValueObject("impulse").GetValue(unit);
        }


#if false
        private void SetDropdownValues() {
			UIStateRoutines.SetSelectedDropdownValue(cbNotionalNozzleModel, 
                                                (string)QraStateContainer.Instance.GetValueSimple<string>("NozzleModel"));
                                                //(string)QraStateContainer.Instance.GlobalData["NozzleModel"]);
            UIStateRoutines.SetSelectedDropdownValue(cbDeflagrationModel,
                                                (string)QraStateContainer.Instance.GetValueSimple<string>("DeflagrationModel"));
                                                //(string)QraStateContainer.Instance.GlobalData["DeflagrationModel"]);
            UIStateRoutines.SetSelectedDropdownValue(cbThermalProbitModel,
                                                (string)QraStateContainer.Instance.GetValueSimple<string>("ThermalProbit"));
                                                //(string)QraStateContainer.Instance.GlobalData["ThermalProbit"]);
            UIStateRoutines.SetSelectedDropdownValue(cbOverpressureProbitModel,
                                                (string)QraStateContainer.Instance.GetValueSimple<string>("OPPROBIT"));
                                                //(string)QraStateContainer.Instance.GlobalData["OPPROBIT"]);
			

		}
#endif

        private double GetThermalExposureTime(ElapsingTimeConversionUnit unit)
        {
            // ElapsingTimeConversionUnit.Second
            return QraStateContainer.Instance.GetStateDefinedValueObject("t_expose_thermal").GetValue(unit)[0];
        }

        private void SetThermalExposureTime(ElapsingTimeConversionUnit unit, double value)
        {
            var values = new double[1];
            values[0] = value;
            var convertibleValue =
                QraStateContainer.Instance.GetStateDefinedValueObject("t_expose_thermal");
            convertibleValue.SetValue(unit, values);
        }

        private void FillCfdGrid()
        {
            var ps = ReadPs(QraStateContainer.Instance.CfdPressureUnit);
            var impulse = ReadImpulse(QraStateContainer.Instance.CfdPressureUnit);

            dgCFDInput.Rows.Clear();
            AddCfdInputRow(PsVariableName, ps, QraStateContainer.Instance.CfdPressureUnit);
            AddCfdInputRow(ImpulseVariableName, impulse, QraStateContainer.Instance.CfdPressureUnit);
        }

        private void dgCFDInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            var rowIndex = e.RowIndex;
            if (rowIndex > -1)
            {
                var currentRow = dgCFDInput.Rows[rowIndex];
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
                            WritePs(QraStateContainer.Instance.CfdPressureUnit, rowValues);
                        else if (variableName == ImpulseVariableName)
                            WriteImpulse(QraStateContainer.Instance.CfdPressureUnit, rowValues);
                        else
                            MessageBox.Show("Cannot set " + variableNameCell + ". Don't know what it is.");
                    }
                    else
                    {
                        MessageBox.Show("Some CFD Input row values were invalid values. Database not updated.");
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
                if (Parsing.TryParseDouble(strValue, out thevalue))
                    result[colIndex - 1] = thevalue;
                else
                    allValuesGood = false;
            }

            return result;
        }

        private void tbThermalExposureTime_TextChanged(object sender, EventArgs e)
        {
            var parsedValue = double.NaN;
            if (Parsing.TryParseDouble(tbThermalExposureTime.Text, out parsedValue))
                SetThermalExposureTime(QraStateContainer.Instance.ExposureTimeUnit, parsedValue);
        }

        private void cbThermalExposureTimeUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            var iValue = GetExposureTimeUnitFromDropdown();

            if (iValue != null)
                QraStateContainer.Instance.ExposureTimeUnit =
                    (ElapsingTimeConversionUnit) Enum.Parse(QraStateContainer.Instance.ExposureTimeUnit.GetType(),
                        iValue.ToString());

            tbThermalExposureTime.Text =
                GetThermalExposureTime(QraStateContainer.Instance.ExposureTimeUnit).ToString("F4");
        }

        private ElapsingTimeConversionUnit? GetExposureTimeUnitFromDropdown()
        {
            ElapsingTimeConversionUnit? result = null;
            var selectedItemName =
                cbThermalExposureTimeUnits.Items[cbThermalExposureTimeUnits.SelectedIndex].ToString();

            if (Enum.TryParse<ElapsingTimeConversionUnit>(selectedItemName, out var iResult)) result = iResult;

            return result;
        }
    }
}