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
using System.Threading.Tasks;
using System.Windows.Forms;
using JrConversions;
using PyAPI;
using QRA_Frontend.Resources;
using QRAState;
using UIHelpers;

namespace QRA_Frontend.ContentPanels
{
    public partial class CpGasPlumeDispersion : UserControl, IQraBaseNotify
    {
        private string _plotFilename; // result

        public CpGasPlumeDispersion()
        {
            InitializeComponent();
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
            pbSpinner.Hide();
            var cpType = new ContentPanel().GetType();
            var cp = (ContentPanel) QuickFunctions.GetFirstParentOfSpecifiedType(this, cpType);
            cp.SetNarrative(Narratives.GPD_GasPlumeDispersion);

            // Set up data-bind grids
            var vdColl = new ParameterWrapperCollection(new[]
            {
                new ParameterWrapper("PlumeWrapper.XMin", "X lower limit", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("PlumeWrapper.XMax", "X upper limit", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("PlumeWrapper.YMin", "Y lower limit", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("PlumeWrapper.YMax", "Y upper limit", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("PlumeWrapper.Contours", "Contours (mole fraction)", UnitlessUnit.Unitless,
                    StockConverters.UnitlessConverter),

                new ParameterWrapper("SysParam.ExternalPresMPA", "Ambient pressure", PressureUnit.Pa,
                    StockConverters.PressureConverter),
                new ParameterWrapper("SysParam.ExternalTempC", "Ambient temperature", TempUnit.Kelvin,
                    StockConverters.TemperatureConverter),
                new ParameterWrapper("FlameWrapper.d_orifice", "Orifice diameter", DistanceUnit.Meter,
                    StockConverters.DistanceConverter),
                new ParameterWrapper("OpWrapper.Cd0", "Orifice discharge coefficient", UnitlessUnit.Unitless,
                    StockConverters.UnitlessConverter),
                new ParameterWrapper("FlameWrapper.P_H2", "Hydrogen pressure", PressureUnit.Pa,
                    StockConverters.PressureConverter),
                new ParameterWrapper("FlameWrapper.T_H2", "Hydrogen temperature", TempUnit.Kelvin,
                    StockConverters.TemperatureConverter),
                new ParameterWrapper("PlumeWrapper.jet_angle", "Angle of jet", AngleUnit.Radians,
                    StockConverters.AngleConverter)
            });
            StaticGridHelperRoutines.InitInteractiveGrid(PlumeParams, vdColl, false);

            PlumeParams.Columns[0].Width = 200;
            PlumeParams.Columns[1].Width = 200;
            PlumeParams.Columns[2].Width = 200;

            //tbPlotTitle_TextChanged(tbPlotTitle,new System.EventArgs());
        }

        private void cpGasPlumeDispersion_Load(object sender, EventArgs e)
        {
            tbPlotTitle.TextChanged += tbPlotTitle_TextChanged;
        }

        private void GridValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
        }

        private void PlumeParams_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridValueChanged(sender, e);
        }

        private async void btnCalculate_Click(object sender, EventArgs e)
        {
            pbSpinner.Show();
            btnCalculate.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void Execute()
        {
            var ambPressure = QraStateContainer.GetNdValue("SysParam.ExternalPresMPA", PressureUnit.Pa);
            var ambTemp = QraStateContainer.GetNdValue("SysParam.ExternalTempC", TempUnit.Kelvin);
            var h2Pressure = QraStateContainer.GetNdValue("FlameWrapper.P_H2", PressureUnit.Pa);
            var h2Temp = QraStateContainer.GetNdValue("FlameWrapper.T_H2", TempUnit.Kelvin);
            var orificeDiam = QraStateContainer.GetNdValue("FlameWrapper.d_orifice", DistanceUnit.Meter);
            var dischargeCoeff = QraStateContainer.GetNdValue("OpWrapper.Cd0", UnitlessUnit.Unitless);
            var xMin = QraStateContainer.GetNdValue("PlumeWrapper.XMin", DistanceUnit.Meter);
            var xMax = QraStateContainer.GetNdValue("PlumeWrapper.XMax", DistanceUnit.Meter);
            var yMin = QraStateContainer.GetNdValue("PlumeWrapper.YMin", DistanceUnit.Meter);
            var yMax = QraStateContainer.GetNdValue("PlumeWrapper.YMax", DistanceUnit.Meter);
            var contours = QraStateContainer.GetNdValue("PlumeWrapper.Contours", UnitlessUnit.Unitless);
            var jetAngle = QraStateContainer.GetNdValue("PlumeWrapper.jet_angle", AngleUnit.Radians);
            var plotTitle = QraStateContainer.GetValue<string>("PlumeWrapper.PlotTitle");

            var physInt = new PhysInterface();
            _plotFilename = physInt.CreatePlumePlot(
                ambPressure, ambTemp, h2Pressure, h2Temp, orificeDiam, dischargeCoeff, xMin, xMax, yMin, yMax, contours,
                jetAngle, plotTitle);
        }

        private void DisplayResults()
        {
            pbSpinner.Hide();
            btnCalculate.Enabled = true;

            if (string.IsNullOrEmpty(_plotFilename))
            {
                MessageBox.Show(@"Plot generation failed, please try again. Check log for details.");
            }
            else
            {
                pbOutput.Load(_plotFilename);
                tcMain.SelectedTab = tpOutput;
            }
        }

        private void tbPlotTitle_TextChanged(object sender, EventArgs e)
        {
            if (tbPlotTitle.Text != null)
                QraStateContainer.SetValue("PlumeWrapper.PlotTitle", tbPlotTitle.Text);
            else
                QraStateContainer.SetValue("PlumeWrapper.PlotTitle", "");
        }

    }
}