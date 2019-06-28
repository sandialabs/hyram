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
using System.Windows.Forms;
using PyAPI;
using QRA_Frontend.Resources;
using QRAState;
using UIHelpers;

namespace QRA_Frontend.ContentPanels
{
    public partial class CpScenarioStats : UserControl, IQraBaseNotify
    {
        public CpScenarioStats()
        {
            InitializeComponent();
            Load += cpScenarioStats_Load;
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
        }

        private void cpScenarioStats_Load(object sender, EventArgs e)
        {
            //ScenColRanking.Width = 70;
            ScenColRanking.ValueType = typeof(int);

            ScenColScenario.ValueType = typeof(string);

            ScenColEndStateType.ValueType = typeof(string);

            ScenColAvgEvents.ValueType = typeof(double);
            ScenColAvgEvents.DefaultCellStyle.Format = "N10";

            ScenColBranchLineProb.ValueType = typeof(double);
            ScenColBranchLineProb.DefaultCellStyle.Format = "P8";

            ScenColPLL.ValueType = typeof(double);
            ScenColPLL.DefaultCellStyle.Format = "P8";

            CutSetDGV000d01.Columns[2].ValueType = typeof(double);
            CutSetDGV000d01.Columns[2].DefaultCellStyle.Format = "N10";
            CutSetDGV000d10.Columns[2].ValueType = typeof(double);
            CutSetDGV000d10.Columns[2].DefaultCellStyle.Format = "N10";
            CutSetDGV001d00.Columns[2].ValueType = typeof(double);
            CutSetDGV001d00.Columns[2].DefaultCellStyle.Format = "N10";
            CutSetDGV010d00.Columns[2].ValueType = typeof(double);
            CutSetDGV010d00.Columns[2].DefaultCellStyle.Format = "N10";
            CutSetDGV100d00.Columns[2].ValueType = typeof(double);
            CutSetDGV100d00.Columns[2].DefaultCellStyle.Format = "N10";

            dgRiskMetrics.Columns[0].Width = 250;
            dgRiskMetrics.Columns[1].Width = 80;
            dgRiskMetrics.Columns[2].Width = 250;
            dgRiskMetrics.Rows.Add("Potential Loss of Life (PLL)", "...", "Fatalities/system-year");
            dgRiskMetrics.Rows.Add("Fatal Accident Rate (FAR)", "...", "Fatalities in 10^8 person-hours");
            dgRiskMetrics.Rows.Add("Average individual risk (AIR)", "...", "Fatalities/year");

            dgRiskMetrics.Rows[0].Cells[1].Value = "Calculating...";
            dgRiskMetrics.Rows[1].Cells[1].Value = "Calculating...";
            dgRiskMetrics.Rows[2].Cells[1].Value = "Calculating...";

            GenerateResults();

            dgRanking.ScrollBars = ScrollBars.Both;

            ScenColEndStateType.SortMode = DataGridViewColumnSortMode.Automatic;

            dgRanking.SortCompare += dgRanking_CustomSort;
        }

        private void GenerateResults()
        {
            ContentPanel.SetNarrative(this, Narratives.SS__ScenarioStats);
            var result = QraStateContainer.GetValue<QraResult>("Result");

            // Set risk metrics
            dgRiskMetrics.Rows[0].Cells[1].Value = result.TotalPll;
            dgRiskMetrics.Rows[1].Cells[1].Value = result.Far;
            dgRiskMetrics.Rows[2].Cells[1].Value = result.Air;

            var curRow = 1;
            for (var i = 0; i < result.LeakResults.Count; i++)
            {
                var leakRes = result.LeakResults[i];
                dgRanking.Rows.Add(curRow++, leakRes.GetLeakSizeString(), "Shutdown", leakRes.ShutdownAvgEvents,
                    leakRes.ProbShutdown, 0);
                dgRanking.Rows.Add(curRow++, leakRes.GetLeakSizeString(), "Jet fire", leakRes.JetfireAvgEvents,
                    leakRes.ProbJetfire, leakRes.JetfirePllContrib);
                dgRanking.Rows.Add(curRow++, leakRes.GetLeakSizeString(), "Explosion", leakRes.ExplosAvgEvents,
                    leakRes.ProbExplosion, leakRes.ExplosionPllContrib);
                dgRanking.Rows.Add(curRow++, leakRes.GetLeakSizeString(), "No ignition", leakRes.NoIgnAvgEvents,
                    leakRes.ProbNoIgnition, 0);
            }

            // Load position plots
            pbPositionPlot000d01.SizeMode = PictureBoxSizeMode.Zoom;
            pbPositionPlot000d01.Load(result.PositionPlotFilenames[0]);
            pbPositionPlot000d10.Load(result.PositionPlotFilenames[1]);
            pbPositionPlot001d00.Load(result.PositionPlotFilenames[2]);
            pbPositionPlot010d00.Load(result.PositionPlotFilenames[3]);
            pbPositionPlot100d00.Load(result.PositionPlotFilenames[4]);

            // Add cut set data
            PopulateCutSetTable(CutSetDGV000d01, result.LeakResults[0], false);
            PopulateCutSetTable(CutSetDGV000d10, result.LeakResults[1], false);
            PopulateCutSetTable(CutSetDGV001d00, result.LeakResults[2], false);
            PopulateCutSetTable(CutSetDGV010d00, result.LeakResults[3], false);
            PopulateCutSetTable(CutSetDGV100d00, result.LeakResults[4], true);
        }

        private void dgRanking_CustomSort(object sender, DataGridViewSortCompareEventArgs e)
        {
            if (e.Column.Index == 1 || e.Column.Index == 2)
            {
                e.SortResult = string.Compare(e.CellValue1.ToString(), e.CellValue2.ToString());
                Debug.WriteLine(e.CellValue1 + ", " + e.CellValue2 + " | " +
                                e.SortResult);
                // If equal, sort by scenario
                if (e.SortResult == 0)
                    e.SortResult = string.Compare(
                        dgRanking.Rows[e.RowIndex1].Cells["ScenColScenario"].Value.ToString(),
                        dgRanking.Rows[e.RowIndex2].Cells["ScenColScenario"].Value.ToString());

                e.Handled = true;
            }
        }

        private void PopulateCutSetTable(DataGridView dgv, LeakResult leakRes, bool is100Leak)
        {
            var nextRow = 0;
            dgv.Rows.Clear();
            if (leakRes.H2ReleaseOverride != -1)
            {
                dgv.Rows.Add(1, "H2 Release (Override)", leakRes.H2ReleaseOverride);
                nextRow = 1;
            }
            else
            {
                dgv.Rows.Add(1, "Compressor leak", leakRes.CompressorLeakFreq);
                dgv.Rows.Add(2, "Cylinder leak", leakRes.CylinderLeakFreq);
                dgv.Rows.Add(3, "Valve leak", leakRes.ValveLeakFreq);
                dgv.Rows.Add(4, "Instrument leak", leakRes.InstrumentLeakFreq);
                dgv.Rows.Add(5, "Joint leak", leakRes.JointLeakFreq);
                dgv.Rows.Add(6, "Hose leak", leakRes.HoseLeakFreq);
                dgv.Rows.Add(7, "Pipe leak", leakRes.PipeLeakFreq);
                dgv.Rows.Add(8, "Filter leak", leakRes.FilterLeakFreq);
                dgv.Rows.Add(9, "Flange leak", leakRes.FlangeLeakFreq);
                dgv.Rows.Add(10, "Extra component #1 leak", leakRes.ExtraComp1LeakFreq);
                dgv.Rows.Add(11, "Extra component #2 leak", leakRes.ExtraComp2LeakFreq);
                nextRow = 12;
            }

            nextRow++;

            if (is100Leak && leakRes.H2ReleaseOverride == -1)
                // Display failure/accident data only for 100% leak (assuming user didn't override H2 release value)
            {
                dgv.Rows.Add();
                //double ManualOverride = QraStateContainer.GetNDValue("Failure.ManualOverride");
                if (leakRes.VehicleFailureProbOverride != -1)
                {
                    dgv.Rows.Add(nextRow, "100% H2 Release from Accidents and Shutdown Failures",
                        leakRes.TotalProbOtherFailures);
                    dgv.Rows.Add(nextRow + 1, "(Note: provided by user)");
                }
                else
                {
                    dgv.Rows.Add(nextRow, "100% H2 Release from Accidents and Shutdown Failures",
                        leakRes.TotalProbOtherFailures);
                    dgv.Rows.Add(nextRow + 1, "Overpressure during fueling induces rupture",
                        leakRes.ProbOverpressureRupture);
                    dgv.Rows.Add(nextRow + 2, "Release due to drive-offs", leakRes.ProbDriveoffs);
                    dgv.Rows.Add(nextRow + 3, "Nozzle release", leakRes.ProbNozzleRelease);
                    dgv.Rows.Add(nextRow + 4, "Manual valve fails to close", leakRes.ProbMValveFtc);
                    dgv.Rows.Add(nextRow + 5, "Solenoid valves fail to close", leakRes.ProbSolValvesFtc);
                }
            }
        }


        private void dgRanking_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            QuickFunctions.PerformNumericSortOnGrid(sender, e);
        }

        private enum ScenColumns
        {
        }
    }
}