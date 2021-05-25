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
using System.Diagnostics;
using System.Windows.Forms;
using SandiaNationalLaboratories.Hyram.Resources;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class QraResultsPanel : UserControl
    {
        public QraResultsPanel()
        {
            InitializeComponent();
            Load += cpScenarioStats_Load;
        }

        private void cpScenarioStats_Load(object sender, EventArgs e)
        {
            //ScenColRanking.Width = 70;
            ScenColRanking.ValueType = typeof(int);

            ScenColScenario.ValueType = typeof(string);
            ScenColScenario.Width = 220;

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
            dgRiskMetrics.Rows.Add("Average Individual Risk (AIR)", "...", "Fatalities/year");

            dgRiskMetrics.Rows[0].Cells[1].Value = "Calculating...";
            dgRiskMetrics.Rows[1].Cells[1].Value = "Calculating...";
            dgRiskMetrics.Rows[2].Cells[1].Value = "Calculating...";

            GenerateResults();

            dgRanking.ScrollBars = ScrollBars.Both;
            //dgRanking.Columns[3].HeaderText = "";

            ScenColEndStateType.SortMode = DataGridViewColumnSortMode.Automatic;

            dgRanking.SortCompare += dgRanking_CustomSort;
        }

        private void GenerateResults()
        {
            ContentPanel.SetNarrative(this, Narratives.QraOutputDescrip);
            var result = StateContainer.GetValue<QraResult>("Result");

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
                //double ManualOverride = StateContainer.GetNDValue("Failure.ManualOverride");
                if (leakRes.VehicleFailureOverride != -1)
                {
                    dgv.Rows.Add(nextRow, "100% H2 Release from Accidents and Shutdown Failures",
                        leakRes.TotalFreqOtherFailures);
                    dgv.Rows.Add(nextRow + 1, "(Note: provided by user)");
                }
                else
                {
                    dgv.Rows.Add(nextRow, "100% H2 Release from Accidents and Shutdown Failures",
                        leakRes.TotalFreqOtherFailures);
                    dgv.Rows.Add(nextRow + 1, "Overpressure during fueling induces rupture",
                        leakRes.FreqOverpressureRupture);
                    dgv.Rows.Add(nextRow + 2, "Release due to drive-offs", leakRes.FreqDriveoffs);
                    dgv.Rows.Add(nextRow + 3, "Nozzle release", leakRes.FreqNozzleRelease);
                    dgv.Rows.Add(nextRow + 4, "Manual valve fails to close", leakRes.FreqMValveFtc);
                    dgv.Rows.Add(nextRow + 5, "Solenoid valves fail to close", leakRes.FreqSolValvesFtc);
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