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
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using EssStringLib;
using JrConversions;
using PyAPI;
using QRA_Frontend.Resources;
using QRAState;
using UIHelpers;

namespace QRA_Frontend.ContentPanels
{
    public partial class CpFlQradSingleAndMulti : UserControl, IQraBaseNotify
    {
        // Results
        private double[] _fluxData;
        private string _fluxPlotFilepath;
        private bool _mIgnoreXyzChangeEvent = true;

        private double[] _radHeatFluxX;
        private double[] _radHeatFluxY;

        private double[] _radHeatFluxZ;
        private string _tempPlotFilepath;

        public CpFlQradSingleAndMulti()
        {
            InitializeComponent();
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
            SetNarrative();
            pbSpinner.Hide();

            // Initialize input grid
            StaticGridHelperRoutines.InitInteractiveGrid(dgInput, new ParameterWrapperCollection(
                new[]
                {
                    new ParameterWrapper("SysParam.ExternalTempC", "Ambient Temperature", TempUnit.Kelvin,
                        StockConverters.TemperatureConverter),
                    new ParameterWrapper("SysParam.ExternalPresMPA", "Ambient Pressure", PressureUnit.Pa,
                        StockConverters.PressureConverter),
                    new ParameterWrapper("FlameWrapper.T_H2", "Hydrogen Temperature", TempUnit.Kelvin,
                        StockConverters.TemperatureConverter),
                    new ParameterWrapper("FlameWrapper.P_H2", "Hydrogen Pressure", PressureUnit.Pa,
                        StockConverters.PressureConverter),
                    new ParameterWrapper("FlameWrapper.d_orifice", "Leak Diameter", DistanceUnit.Meter,
                        StockConverters.DistanceConverter),
                    new ParameterWrapper("FlameWrapper.RH", "Relative Humidity", UnitlessUnit.Unitless,
                        StockConverters.UnitlessConverter),
                    new ParameterWrapper("OpWrapper.ReleaseAngle", "Release Angle", AngleUnit.Radians,
                        StockConverters.AngleConverter),
                    new ParameterWrapper("FlameWrapper.ReleaseHeight", "Leak Height from Floor", DistanceUnit.Meter,
                        StockConverters.DistanceConverter)
                }
            ));
            dgInput.Columns[0].Width = 180;

            // Initialize flex input
            UiParsingRoutines.PutDoubleArrayIntoTextBox(tbRadiativeHeatFluxPointsX,
                QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.radiative_heat_flux_point:x")
                    .GetValue(DistanceUnit.Meter));
            UiParsingRoutines.PutDoubleArrayIntoTextBox(tbRadiativeHeatFluxPointsY,
                QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.radiative_heat_flux_point:y")
                    .GetValue(DistanceUnit.Meter));
            UiParsingRoutines.PutDoubleArrayIntoTextBox(tbRadiativeHeatFluxPointsZ,
                QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.radiative_heat_flux_point:z")
                    .GetValue(DistanceUnit.Meter));
            UiParsingRoutines.PutDoubleArrayIntoTextBox(tbContourLevels,
                QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.contour_levels")
                    .GetValue(UnitlessUnit.Unitless));

            _mIgnoreXyzChangeEvent = false;
            SetRadiativeSourceModel();
        }

        private void SetRadiativeSourceModel()
        {
            var current = QraStateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel");
            cbRadiativeSourceModel.Fill(UiStateRoutines.GetRadiativeSourceModelDict(), current);
        }

        private void SetNarrative()
        {
            ContentPanel.SetNarrative(this, Narratives.CP_CP_PlotT_TopNarrative);
        }

        private void dgInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
        }

        private void cbRadiativeSourceModel_OnValueChanged(object sender, EventArgs e)
        {
            var newValue =
                (RadiativeSourceModels) ((AnyEnumComboSelector) sender).SelectedItem;
            QraStateContainer.SetValue("RadiativeSourceModel", newValue);
        }

        private async void btnExecute_Click(object sender, EventArgs e)
        {
            ClearResults();
            pbSpinner.Show();
            btnExecute.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void ClearResults()
        {
            if (pbPlotIsoOutput.Image != null)
            {
                pbPlotIsoOutput.Image.Dispose();
                pbPlotIsoOutput.Image = null;
            }

            if (pbTPlot.Image != null)
            {
                pbTPlot.Image.Dispose();
                pbTPlot.Image = null;
            }

            dgResult.Rows.Clear();
        }

        private void Execute()
        {
            // Blanket try block to help catch deployed issue Brian encountered
            Trace.TraceInformation("Gathering parameters for radiative flux analysis...");
            try
            {
                var ambTemp = QraStateContainer.GetNdValue("SysParam.ExternalTempC", TempUnit.Kelvin);
                var ambPressure = QraStateContainer.GetNdValue("SysParam.ExternalPresMPA", PressureUnit.Pa);
                var h2Temp = QraStateContainer.GetNdValue("FlameWrapper.T_H2", TempUnit.Kelvin);
                var h2Pressure = QraStateContainer.GetNdValue("FlameWrapper.P_H2", PressureUnit.Pa);
                var orificeDiam = QraStateContainer.GetNdValue("FlameWrapper.d_orifice", DistanceUnit.Meter);
                var leakHeight = QraStateContainer.GetNdValue("FlameWrapper.ReleaseHeight", DistanceUnit.Meter);
                _radHeatFluxX =
                    QraStateContainer.GetNdValueList("FlameWrapper.radiative_heat_flux_point:x", DistanceUnit.Meter);
                _radHeatFluxY =
                    QraStateContainer.GetNdValueList("FlameWrapper.radiative_heat_flux_point:y", DistanceUnit.Meter);
                _radHeatFluxZ =
                    QraStateContainer.GetNdValueList("FlameWrapper.radiative_heat_flux_point:z", DistanceUnit.Meter);
                var contourLevels =
                    QraStateContainer.GetNdValueList("FlameWrapper.contour_levels", UnitlessUnit.Unitless);
                var relativeHumidity = QraStateContainer.GetNdValue("FlameWrapper.RH", UnitlessUnit.Unitless);
                var notionalNozzleModel = QraStateContainer.GetValue<NozzleModel>("NozzleModel");
                var releaseAngle = QraStateContainer.GetNdValue("OpWrapper.ReleaseAngle", AngleUnit.Radians);
                var radiativeSourceModel =
                    QraStateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel");

                Trace.TraceInformation("Creating PhysInterface for python call");
                var physInt = new PhysInterface();
                physInt.AnalyzeRadiativeHeatFlux(ambTemp, ambPressure, h2Temp, h2Pressure, orificeDiam, leakHeight,
                    releaseAngle,
                    notionalNozzleModel, _radHeatFluxX, _radHeatFluxY, _radHeatFluxZ, relativeHumidity,
                    radiativeSourceModel,
                    contourLevels, out _fluxData, out _fluxPlotFilepath, out _tempPlotFilepath);
                Trace.TraceInformation("PhysInterface call complete");
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                MessageBox.Show(@"Heat flux analysis failed, please try again. Check log for details.");
            }
        }

        private void DisplayResults()
        {
            var numDataPoints = _fluxData.Length;

            if (numDataPoints == 0)
            {
                MessageBox.Show("Analysis yielded no data.");
            }
            else
            {
                pbPlotIsoOutput.Load(_fluxPlotFilepath);
                pbTPlot.Load(_tempPlotFilepath);

                dgResult.Rows.Clear();
                for (var index = 0; index < numDataPoints; index++)
                {
                    var values = new object[4];
                    values[0] = _radHeatFluxX[index].ToString("F4");
                    values[1] = _radHeatFluxY[index].ToString("F4");
                    values[2] = _radHeatFluxZ[index].ToString("F4");
                    values[3] = _fluxData[index].ToString("F4");
                    dgResult.Rows.Add(values);
                }

                pbSpinner.Hide();
                btnExecute.Enabled = true;
                tcIO.SelectTab(tpOutput);
            }
        }

        private void tbRadiativeHeatFluxPointsX_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
            ConditionallyShowCalculateButton();
        }

        private void tbRadiativeHeatFluxPointsY_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
            ConditionallyShowCalculateButton();
        }

        private void tbRadiativeHeatFluxPointsZ_TextChanged(object sender, EventArgs e)
        {
            ExtractAndSaveXyzValues();
            ConditionallyShowCalculateButton();
        }

        private void ExtractAndSaveXyzValues()
        {
            if (!_mIgnoreXyzChangeEvent)
            {
                var xValues =
                    ArrayFunctions.ExtractFloatArrayFromDelimitedString(tbRadiativeHeatFluxPointsX.Text, ',');
                var yValues =
                    ArrayFunctions.ExtractFloatArrayFromDelimitedString(tbRadiativeHeatFluxPointsY.Text, ',');
                var zValues =
                    ArrayFunctions.ExtractFloatArrayFromDelimitedString(tbRadiativeHeatFluxPointsZ.Text, ',');
                if (xValues.Length == yValues.Length && yValues.Length == zValues.Length && zValues.Length > 0)
                {
                    QraStateContainer.SetValue("FlameWrapper.radiative_heat_flux_point:x",
                        new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, xValues));
                    QraStateContainer.SetValue("FlameWrapper.radiative_heat_flux_point:y",
                        new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, yValues));
                    QraStateContainer.SetValue("FlameWrapper.radiative_heat_flux_point:z",
                        new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, zValues));
                }
            }
        }

        private void ConditionallyShowCalculateButton()
        {
            var enableButton = false;
            var arrayCountWarningLabelVisible = false;

            if (tbRadiativeHeatFluxPointsY.Text.Trim().Length > 0)
            {
                var numXElems = CountElements(tbRadiativeHeatFluxPointsX.Text);
                var numYElems = CountElements(tbRadiativeHeatFluxPointsY.Text);
                var numZElems = CountElements(tbRadiativeHeatFluxPointsZ.Text);
                if (numZElems == numYElems && numZElems == numXElems)
                {
                    enableButton = true;
                    arrayCountWarningLabelVisible = false;
                }
                else
                {
                    arrayCountWarningLabelVisible = true;
                }

                lblXElemCount.Text = numXElems + " elements";
                lblYElemCount.Text = numYElems + " elements";
                lblZElementCount.Text = numZElems + " elements";
            }

            if (btnExecute.Enabled != enableButton) btnExecute.Enabled = enableButton;

            if (lblArrayWarning.Visible != arrayCountWarningLabelVisible)
                lblArrayWarning.Visible = arrayCountWarningLabelVisible;
        }

        private int CountElements(string textToParse)
        {
            var values = textToParse.Trim().Split(',');
            return values.Length;
        }

        private void btnCopyToClipboard_Click(object sender, EventArgs e)
        {
            try
            {
                CopyResultGridToClipboard(dgResult);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem copying to the clipboard: " + ex.Message);
            }
        }

        private void CopyResultGridToClipboard(DataGridView dgGridToCopy)
        {
            var sa = new ClsEditableStringArray();
            string thisLine = null;

            foreach (DataGridViewColumn thisColumn in dgGridToCopy.Columns)
                if (thisLine == null)
                    thisLine = thisColumn.HeaderText;
                else
                    thisLine += "\t" + thisColumn.HeaderText;

            sa.Append(thisLine);

            foreach (DataGridViewRow thisRow in dgGridToCopy.Rows)
            {
                thisLine = null;
                foreach (DataGridViewCell thisCell in thisRow.Cells)
                    if (thisCell.Value != null)
                    {
                        var thisValue = thisCell.Value.ToString() ?? "";
                        if (thisLine == null)
                            thisLine = thisValue;
                        else
                            thisLine += "\t" + thisValue;
                    }

                if (thisLine != null)
                {
                    sa.Append(thisLine);
                    thisLine = null;
                }
            }

            var clipboardText = sa.CombineToString(ArrayStringConversionOption.AppendCrlf);
            Clipboard.SetDataObject(clipboardText, true);
        }

        private void tbContourLevels_TextChanged(object sender, EventArgs e)
        {
            if (!_mIgnoreXyzChangeEvent)
            {
                var contourLevels = ArrayFunctions.ExtractFloatArrayFromDelimitedString(tbContourLevels.Text, ',');
                QraStateContainer.Instance.Parameters["FlameWrapper.contour_levels"] =
                    new NdConvertibleValue(StockConverters.UnitlessConverter, UnitlessUnit.Unitless, contourLevels,
                        0.0);
            }
        }
    }
}