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

namespace QRA_Frontend.ContentPanels
{
    public partial class CpPlotT : UserControl, IQraBaseNotify
    {
        private string _resultImageFilepath;

        public CpPlotT()
        {
            InitializeComponent();
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
            ContentPanel.SetNarrative(this, Narratives.CP_CP_PlotT_TopNarrative);
            InitializeInputGrid();
            pbSpinner.Hide();
        }

        private void InitializeInputGrid()
        {
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
                    new ParameterWrapper("FlameWrapper.ReleaseHeight", "Leak Height from Floor (y0)",
                        DistanceUnit.Meter, StockConverters.DistanceConverter),
                    new ParameterWrapper("OpWrapper.ReleaseAngle", "Release Angle", AngleUnit.Degrees,
                        StockConverters.AngleConverter)
                }
            ));

            dgInput.Columns[0].Width = 180;
        }

        private async void btnExecute_Click(object sender, EventArgs e)
        {
            pbSpinner.Show();
            btnExecute.Enabled = false;
            await Task.Run(() => Execute());
            DisplayResults();
        }

        private void Execute()
        {
            var ambTemp = QraStateContainer.Instance.GetStateDefinedValueObject("SysParam.ExternalTempC")
                .GetValue(TempUnit.Kelvin)[0];
            var ambPres = QraStateContainer.Instance.GetStateDefinedValueObject("SysParam.ExternalPresMPA")
                .GetValue(PressureUnit.Pa)[0];
            var h2Temp = QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.T_H2")
                .GetValue(TempUnit.Kelvin)[0];
            var h2Pres = QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.P_H2")
                .GetValue(PressureUnit.Pa)[0];
            var orificeDiam = QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.d_orifice")
                .GetValue(DistanceUnit.Meter)[0];
            var y0 = QraStateContainer.Instance.GetStateDefinedValueObject("FlameWrapper.ReleaseHeight")
                .GetValue(DistanceUnit.Meter)[0];
            var releaseAngle = QraStateContainer.Instance.GetStateDefinedValueObject("OpWrapper.ReleaseAngle")
                .GetValue(AngleUnit.Radians)[0];
            var nozzleModel = QraStateContainer.GetValue<NozzleModel>("NozzleModel");

            var physInt = new PhysInterface();
            _resultImageFilepath = physInt.CreateFlameTemperaturePlot(ambTemp, ambPres, h2Temp, h2Pres, orificeDiam, y0,
                releaseAngle, nozzleModel.GetKey());
        }

        private void DisplayResults()
        {
            pbSpinner.Hide();
            btnExecute.Enabled = true;
            if (string.IsNullOrEmpty(_resultImageFilepath))
                MessageBox.Show(@"Error generating flame temp plot. Please verify the inputs.");
            else
                pbOutput.Load(_resultImageFilepath);

            tcIO.SelectedTab = tpOutput;
        }

        private void dgInput_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StaticGridHelperRoutines.ProcessDataGridViewRowValueChangedEvent((DataGridView) sender, e, 1, 2, false);
        }
    }
}