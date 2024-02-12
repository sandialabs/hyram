/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SandiaNationalLaboratories.Hyram.Resources;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class QraResultsPanel : UserControl
    {
        private bool _hasImpulseResults = true;
        private readonly StateContainer _state = State.Data;

        public QraResultsPanel()
        {
            InitializeComponent();
            Load += cpScenarioStats_Load;

            // right-click to save images
            var picMenu = new ContextMenuStrip();
            {
                var item = new ToolStripMenuItem {Text = "Save As..."};
                item.MouseUp += UiHelpers.SaveImageToolStripMenuItem_Click;
                picMenu.Items.Add(item);
            }
            riskPlot1.ContextMenuStrip = picMenu;
            riskPlot2.ContextMenuStrip = picMenu;
            riskPlot3.ContextMenuStrip = picMenu;
            OverpressurePic1.ContextMenuStrip = picMenu;
            OverpressurePic2.ContextMenuStrip = picMenu;
            OverpressurePic3.ContextMenuStrip = picMenu;
            OverpressurePic4.ContextMenuStrip = picMenu;
            OverpressurePic5.ContextMenuStrip = picMenu;
            ImpulsePic1.ContextMenuStrip = picMenu;
            ImpulsePic2.ContextMenuStrip = picMenu;
            ImpulsePic3.ContextMenuStrip = picMenu;
            ImpulsePic4.ContextMenuStrip = picMenu;
            ImpulsePic5.ContextMenuStrip = picMenu;
            ThermalPic1.ContextMenuStrip = picMenu;
            ThermalPic2.ContextMenuStrip = picMenu;
            ThermalPic3.ContextMenuStrip = picMenu;
            ThermalPic4.ContextMenuStrip = picMenu;
            ThermalPic5.ContextMenuStrip = picMenu;
            ScenPlot.ContextMenuStrip = picMenu;
        }

        private void cpScenarioStats_Load(object sender, EventArgs e)
        {
            ScenColRanking.ValueType = typeof(int);
            ScenColScenario.ValueType = typeof(string);
            ScenColScenario.Width = 220;
            ScenColEndStateType.ValueType = typeof(string);
            ScenColAvgEvents.ValueType = typeof(double);
            ScenColAvgEvents.DefaultCellStyle.Format = "N10";
            AvgEventsMax.ValueType = typeof(double);
            AvgEventsMax.DefaultCellStyle.Format = "N10";

            ScenColBranchLineProb.ValueType = typeof(double);
            ScenColBranchLineProb.DefaultCellStyle.Format = "P8";
            ScenColPLL.ValueType = typeof(double);
            ScenColPLL.DefaultCellStyle.Format = "P8";

            EventPlotCol.HeaderText = "";
            //EventPlotCol.Name = "";
            EventPlotCol.Text = "plot";
            EventPlotCol.UseColumnTextForButtonValue = true;
            EventPlotCol.Width = 30;

            dgRiskMetrics.Columns[0].Width = 250;
            dgRiskMetrics.Columns[1].Width = 80;
            dgRiskMetrics.Columns[2].Width = 250;
            dgRiskMetrics.Rows.Add("Potential Loss of Life (PLL)", "...", "Fatalities/system-year");
            dgRiskMetrics.Rows.Add("Fatal Accident Rate (FAR)", "...", "Fatalities in 10^8 person-hours");
            dgRiskMetrics.Rows.Add("Average Individual Risk (AIR)", "...", "Fatalities/year");

            dgRiskMetrics.Rows[0].Cells[1].Value = "Calculating...";
            dgRiskMetrics.Rows[1].Cells[1].Value = "Calculating...";
            dgRiskMetrics.Rows[2].Cells[1].Value = "Calculating...";

            detailsLeakGrid.Columns[1].DefaultCellStyle.Format = "E3";
            detailsLeakGrid.Columns[2].DefaultCellStyle.Format = "E3";

            detailsOutcomeGrid.Columns[1].DefaultCellStyle.Format = "P3";
            detailsOutcomeGrid.Columns[2].DefaultCellStyle.Format = "P3";
            detailsOutcomeGrid.Columns[3].DefaultCellStyle.Format = "P3";
            detailsOutcomeGrid.Columns[4].DefaultCellStyle.Format = "P3";
            detailsOutcomeGrid.Columns[5].DefaultCellStyle.Format = "P3";

            // x,y,z coordinates
            thermalDataGrid.Columns[1].DefaultCellStyle.Format = "N1";
            thermalDataGrid.Columns[2].DefaultCellStyle.Format = "N1";
            thermalDataGrid.Columns[3].DefaultCellStyle.Format = "N1";

            overpressureDataGrid.Columns[1].DefaultCellStyle.Format = "N1";
            overpressureDataGrid.Columns[2].DefaultCellStyle.Format = "N1";
            overpressureDataGrid.Columns[3].DefaultCellStyle.Format = "N1";

            impulseDataGrid.Columns[1].DefaultCellStyle.Format = "N1";
            impulseDataGrid.Columns[2].DefaultCellStyle.Format = "N1";
            impulseDataGrid.Columns[3].DefaultCellStyle.Format = "N1";

            // flux values
            thermalDataGrid.Columns[4].DefaultCellStyle.Format = "E3";
            thermalDataGrid.Columns[5].DefaultCellStyle.Format = "E3";
            thermalDataGrid.Columns[6].DefaultCellStyle.Format = "E3";
            thermalDataGrid.Columns[7].DefaultCellStyle.Format = "E3";
            thermalDataGrid.Columns[8].DefaultCellStyle.Format = "E3";

            overpressureDataGrid.Columns[4].DefaultCellStyle.Format = "E3";
            overpressureDataGrid.Columns[5].DefaultCellStyle.Format = "E3";
            overpressureDataGrid.Columns[6].DefaultCellStyle.Format = "E3";
            overpressureDataGrid.Columns[7].DefaultCellStyle.Format = "E3";
            overpressureDataGrid.Columns[8].DefaultCellStyle.Format = "E3";

            impulseDataGrid.Columns[4].DefaultCellStyle.Format = "E3";
            impulseDataGrid.Columns[5].DefaultCellStyle.Format = "E3";
            impulseDataGrid.Columns[6].DefaultCellStyle.Format = "E3";
            impulseDataGrid.Columns[7].DefaultCellStyle.Format = "E3";
            impulseDataGrid.Columns[8].DefaultCellStyle.Format = "E3";

            GenerateResults();

            if (_state.IsUncertain())
            {
                ScenPlot.Visible = true;
                EventPlotCol.Visible = true;
                AvgEventsMax.Visible = true;
                riskMetricsTab.AutoScroll = true;
            }
            else
            {
                ScenPlot.Visible = false;
                EventPlotCol.Visible = false;
                AvgEventsMax.Visible = false;
                riskMetricsTab.AutoScroll = false;
            }
            ScenRankingGrid.ScrollBars = ScrollBars.Both;

            ScenColEndStateType.SortMode = DataGridViewColumnSortMode.Automatic;

            ScenRankingGrid.SortCompare += dgRanking_CustomSort;

            // handle clicks in the Scenario Ranking uncertainty plot button column.
            ScenRankingGrid.CellClick += ScenRankingGrid_CellClick;
        }

        // Calls the Employee.RequestStatus method.
        void ScenRankingGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex != 5) return;

            // plot filepath stored as tag
            var row = ScenRankingGrid.Rows[e.RowIndex];
            var plot = (string)row.Tag;
            if (!string.IsNullOrEmpty(plot))
            {
                ScenPlot.Load(plot);
                ScenPlot.Show();
            }
        }

        private void GenerateResults()
        {
            var result = (QraResult) State.Data.QraResult;
            var isUncertain = _state.IsUncertain();

            _hasImpulseResults = result.ImpulsePlotFiles.Length > 0;
            ScenPlot.Image = null;
            ScenPlot.Hide();

            // ===============
            // RISK METRICS
            dgRiskMetrics.Rows[0].Cells[1].Value = result.TotalPll;
            dgRiskMetrics.Rows[1].Cells[1].Value = result.Far;
            dgRiskMetrics.Rows[2].Cells[1].Value = result.Air;

            //if (true)
            if (isUncertain)
            {
                riskPlot1.Load(result.QradPlotFiles[0]);
                riskPlot2.Load(result.QradPlotFiles[1]);
                riskPlot3.Load(result.QradPlotFiles[2]);

                ScenColAvgEvents.HeaderText = "Average Events / Year (Minimum)";
                AvgEventsMax.Visible = true;
            }
            else
            {
                ScenColAvgEvents.HeaderText = "Average Events / Year";
                AvgEventsMax.Visible = false;
                riskPlot1.Hide();
                riskPlot2.Hide();
                riskPlot3.Hide();
            }

            detailsOutcomeGrid.Rows.Add("Shutdown", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            detailsOutcomeGrid.Rows.Add("Jetfire", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            detailsOutcomeGrid.Rows.Add("Explosion", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            detailsOutcomeGrid.Rows.Add("No ignition", double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);

            var curRow = 1;
            for (var i = 0; i < result.LeakResults.Count; i++)
            {
                var leakRes = result.LeakResults[i];
                ScenRankingGrid.Rows.Add(curRow++,
                                        leakRes.GetLeakSizeString(), "Shutdown",
                                        leakRes.ShutdownAvgEvents,
                                        leakRes.ShutdownAvgEventsMax,
                                        null,
                                        leakRes.ProbShutdown,
                                        0);
                ScenRankingGrid.Rows[ScenRankingGrid.Rows.Count - 1].Tag = result.QradPlotFiles[0];

                ScenRankingGrid.Rows.Add(curRow++,
                                        leakRes.GetLeakSizeString(), "Jet fire",
                                        leakRes.JetfireAvgEvents,
                                        leakRes.JetfireAvgEventsMax,
                                        null,
                                        leakRes.ProbJetfire,
                                        leakRes.JetfirePllContrib);
                ScenRankingGrid.Rows[ScenRankingGrid.Rows.Count - 1].Tag = result.QradPlotFiles[1];

                ScenRankingGrid.Rows.Add(curRow++,
                                        leakRes.GetLeakSizeString(), "Explosion",
                                        leakRes.ExplosAvgEvents,
                                        leakRes.ExplosAvgEventsMax,
                                        null,
                                        leakRes.ProbExplosion,
                                        leakRes.ExplosionPllContrib);
                ScenRankingGrid.Rows[ScenRankingGrid.Rows.Count - 1].Tag = result.QradPlotFiles[2];

                ScenRankingGrid.Rows.Add(curRow++,
                                        leakRes.GetLeakSizeString(), "No ignition",
                                        leakRes.NoIgnAvgEvents,
                                        leakRes.NoIgnAvgEventsMax,
                                        null,
                                        leakRes.ProbNoIgnition,
                                        0);
                ScenRankingGrid.Rows[ScenRankingGrid.Rows.Count - 1].Tag = result.QradPlotFiles[0];

                detailsLeakGrid.Rows.Add(leakRes.GetLeakSizeString(), leakRes.MassFlowRate, leakRes.LeakDiam);

                detailsOutcomeGrid.Rows[0].Cells[i+1].Value = leakRes.ProbShutdown;
                detailsOutcomeGrid.Rows[1].Cells[i+1].Value = leakRes.ProbJetfire;
                detailsOutcomeGrid.Rows[2].Cells[i+1].Value = leakRes.ProbExplosion;
                detailsOutcomeGrid.Rows[3].Cells[i+1].Value = leakRes.ProbNoIgnition;
            }

            // Load thermal effects data
            ThermalPic1.Load(result.QradPlotFiles[0]);
            ThermalPic2.Load(result.QradPlotFiles[1]);
            ThermalPic3.Load(result.QradPlotFiles[2]);
            ThermalPic4.Load(result.QradPlotFiles[3]);
            ThermalPic5.Load(result.QradPlotFiles[4]);

            double[] posXs = result.Positions[0];
            double[] posYs = result.Positions[1];
            double[] posZs = result.Positions[2];
            var numPositions = posXs.Length;

            for (var i = 0; i < numPositions; i++)
            {
                double[] fluxes = result.PositionQrads[i];
                double[] overps = result.PositionOverpressures[i];
                double[] impulses = result.PositionImpulses[i];

                thermalDataGrid.Rows.Add(i + 1,
                    posXs[i], posYs[i], posZs[i],
                    fluxes[0], fluxes[1], fluxes[2], fluxes[3], fluxes[4]);

                overpressureDataGrid.Rows.Add(i + 1,
                    posXs[i], posYs[i], posZs[i],
                    overps[0],
                    overps[1],
                    overps[2],
                    overps[3],
                    overps[4]);

                if (_hasImpulseResults)
                {
                    impulseDataGrid.Rows.Add(i + 1,
                        posXs[i], posYs[i], posZs[i],
                        impulses[0] / 1000,
                        impulses[1] / 1000,
                        impulses[2] / 1000,
                        impulses[3] / 1000,
                        impulses[4] / 1000);
                }
            }

            // Load overpressure data
            OverpressurePic1.Load(result.OverpressurePlotFiles[0]);
            OverpressurePic2.Load(result.OverpressurePlotFiles[1]);
            OverpressurePic3.Load(result.OverpressurePlotFiles[2]);
            OverpressurePic4.Load(result.OverpressurePlotFiles[3]);
            OverpressurePic5.Load(result.OverpressurePlotFiles[4]);

            // Load impulse data
            if (_hasImpulseResults)
            {
                impulsePlotTabs.Visible = true;
                impulseDataTab.Enabled = true;
                ImpulsePic1.Load(result.ImpulsePlotFiles[0]);
                ImpulsePic2.Load(result.ImpulsePlotFiles[1]);
                ImpulsePic3.Load(result.ImpulsePlotFiles[2]);
                ImpulsePic4.Load(result.ImpulsePlotFiles[3]);
                ImpulsePic5.Load(result.ImpulsePlotFiles[4]);
            }
            else
            {
                impulsePlotTabs.Visible = false;
                impulseDataTab.Enabled = false;
            }

            // ===================
            // Non-UQ Cut Set data
            //CutSetGrid.Columns[2].ValueType = typeof(double);
            //CutSetGrid.Columns[2].DefaultCellStyle.Format = "N10";
            if (!_state.IsUncertain())
            {
                var grid = CutSetGrid;
                cutSetsUqTab.Hide();
                qraResultTabs.TabPages.Remove(cutSetsUqTab);

                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.RowHeadersVisible = false;

                grid.Rows.Clear();
                // if given, 100% release override replaces ALL values
                grid.Rows.Add(0, "Fluid Release (Override)");
                grid.Rows.Add(1);
                grid.Rows.Add(2, "Compressor leak");
                grid.Rows.Add(3, "Vessel leak");
                grid.Rows.Add(4, "Valve leak");
                grid.Rows.Add(5, "Instrument leak");
                grid.Rows.Add(6, "Joint leak");
                grid.Rows.Add(7, "Hose leak");
                grid.Rows.Add(8, "Pipe leak");
                grid.Rows.Add(9, "Filter leak");
                grid.Rows.Add(10, "Flange leak");
                grid.Rows.Add(11, "Heat exchanger leak");
                grid.Rows.Add(12, "Vaporizer leak");
                grid.Rows.Add(13, "Loading arm leak");
                grid.Rows.Add(14, "Extra component #1 leak");
                grid.Rows.Add(15, "Extra component #2 leak");
                grid.Rows.Add(1);
                grid.Rows.Add(17, "100% Fluid release from accidents and shutdown failures");
                grid.Rows.Add(18, "Overpressure during fueling induces rupture");
                grid.Rows.Add(19, "Release due to drive-offs");
                grid.Rows.Add(20, "Nozzle release");
                grid.Rows.Add(21, "Manual valve fails to close");
                grid.Rows.Add(22, "Solenoid valves fail to close");
                grid.Rows[1].Height = 20; // so all rows fit in GUI height
                grid.Rows[16].Height = 20;

                PopulateCutSetColumn(2, result.LeakResults[0], _state.Release000d01.GetValueMaybeNull());
                PopulateCutSetColumn(3, result.LeakResults[1], _state.Release000d10.GetValueMaybeNull());
                PopulateCutSetColumn(4, result.LeakResults[2], _state.Release001d00.GetValueMaybeNull());
                PopulateCutSetColumn(5, result.LeakResults[3], _state.Release010d00.GetValueMaybeNull());
                PopulateCutSetColumn(6, result.LeakResults[4], _state.Release100d00.GetValueMaybeNull());

            }
            else
            {
                var grid = UqCutSetGrid;
                cutSetsTab.Hide();
                qraResultTabs.TabPages.Remove(cutSetsTab);

                // ===============
                // UQ Cut Set Data
                grid.RowHeadersVisible = false;
                grid.ColumnHeadersDefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                // TODO: compress row label creation for both grids
                grid.Rows.Clear();
                // if given, 100% release override replaces ALL values
                grid.Rows.Add(0, "Fluid Release (Override)");
                grid.Rows.Add(1);
                grid.Rows.Add(2, "Compressor leak");
                grid.Rows.Add(3, "Vessel leak");
                grid.Rows.Add(4, "Valve leak");
                grid.Rows.Add(5, "Instrument leak");
                grid.Rows.Add(6, "Joint leak");
                grid.Rows.Add(7, "Hose leak");
                grid.Rows.Add(8, "Pipe leak");
                grid.Rows.Add(9, "Filter leak");
                grid.Rows.Add(10, "Flange leak");
                grid.Rows.Add(11, "Heat exchanger leak");
                grid.Rows.Add(12, "Vaporizer leak");
                grid.Rows.Add(13, "Loading arm leak");
                grid.Rows.Add(14, "Extra component #1 leak");
                grid.Rows.Add(15, "Extra component #2 leak");
                grid.Rows.Add(1);
                grid.Rows.Add(17, "100% release from accidents, shutdown failures");
                grid.Rows.Add(18, "Overpressure during fueling induces rupture");
                grid.Rows.Add(19, "Release due to drive-offs");
                grid.Rows.Add(20, "Nozzle release");
                grid.Rows.Add(21, "Manual valve fails to close");
                grid.Rows.Add(22, "Solenoid valves fail to close");
                grid.Rows[1].Height = 20; // so all rows fit in GUI height
                grid.Rows[16].Height = 20;

                PopulateUncertainCutSetColumn(2, result.LeakResults[0], _state.Release000d01.GetValueMaybeNull());
                PopulateUncertainCutSetColumn(4, result.LeakResults[1], _state.Release000d10.GetValueMaybeNull());
                PopulateUncertainCutSetColumn(6, result.LeakResults[2], _state.Release001d00.GetValueMaybeNull());
                PopulateUncertainCutSetColumn(8, result.LeakResults[3], _state.Release010d00.GetValueMaybeNull());
                PopulateUncertainCutSetColumn(10, result.LeakResults[4], _state.Release100d00.GetValueMaybeNull());

            }
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
                        ScenRankingGrid.Rows[e.RowIndex1].Cells["ScenColScenario"].Value.ToString(),
                        ScenRankingGrid.Rows[e.RowIndex2].Cells["ScenColScenario"].Value.ToString());

                e.Handled = true;
            }
        }

        private void PopulateCutSetColumn(int col, LeakResult res, double? overrideVal)
        {
            var grid = CutSetGrid;
            var finalCol = 6;
            if (overrideVal != null)
            {
                grid.Rows[0].Cells[col].Value = overrideVal;
                return;
            }
            grid.Rows[0].Cells[col].Value = "-";
            grid.Rows[2].Cells[col].Value = res.CompressorLeakFreq;
            grid.Rows[3].Cells[col].Value = res.VesselLeakFreq;
            grid.Rows[4].Cells[col].Value = res.ValveLeakFreq;
            grid.Rows[5].Cells[col].Value = res.InstrumentLeakFreq;
            grid.Rows[6].Cells[col].Value = res.JointLeakFreq;
            grid.Rows[7].Cells[col].Value = res.HoseLeakFreq;
            grid.Rows[8].Cells[col].Value = res.PipeLeakFreq;
            grid.Rows[9].Cells[col].Value = res.FilterLeakFreq;
            grid.Rows[10].Cells[col].Value = res.FlangeLeakFreq;
            grid.Rows[11].Cells[col].Value = res.ExchangerLeakFreq;
            grid.Rows[12].Cells[col].Value = res.VaporizerLeakFreq;
            grid.Rows[13].Cells[col].Value = res.ArmLeakFreq;
            grid.Rows[14].Cells[col].Value = res.ExtraComp1LeakFreq;
            grid.Rows[15].Cells[col].Value = res.ExtraComp2LeakFreq;
            if (col != finalCol)
            {
                return;
            }

            if (!_state.FailureOverride.IsNull)
            {
                grid.Rows[17].Cells[col].Value = _state.FailureOverride.GetValue();
                grid.Rows[18].Cells[1].Value = "(Note: provided by user)";
                grid.Rows[19].Cells[1].Value = "";
                grid.Rows[20].Cells[1].Value = "";
                grid.Rows[21].Cells[1].Value = "";
                grid.Rows[22].Cells[1].Value = "";
            }
            else
            {
                grid.Rows[17].Cells[col].Value = res.TotalFreqOtherFailures;
                grid.Rows[18].Cells[col].Value = res.FreqOverpressureRupture;
                grid.Rows[19].Cells[col].Value = res.FreqDriveoffs;
                grid.Rows[20].Cells[col].Value = res.FreqNozzleRelease;
                grid.Rows[21].Cells[col].Value = res.FreqMValveFtc;
                grid.Rows[22].Cells[col].Value = res.FreqSolValvesFtc;
            }
        }

        private void PopulateUncertainCutSetColumn(int col50, LeakResult res, double? overrideVal)
        {
            int col95 = col50 + 1;
            var grid = UqCutSetGrid;
            var finalCol = 10;
            if (overrideVal != null)
            {
                grid.Rows[0].Cells[col50].Value = overrideVal;
                return;
            }
            grid.Rows[0].Cells[col50].Value = "-";
            grid.Rows[2].Cells[col50].Value = res.CompressorLeakFreq;
            grid.Rows[3].Cells[col50].Value = res.VesselLeakFreq;
            grid.Rows[4].Cells[col50].Value = res.ValveLeakFreq;
            grid.Rows[5].Cells[col50].Value = res.InstrumentLeakFreq;
            grid.Rows[6].Cells[col50].Value = res.JointLeakFreq;
            grid.Rows[7].Cells[col50].Value = res.HoseLeakFreq;
            grid.Rows[8].Cells[col50].Value = res.PipeLeakFreq;
            grid.Rows[9].Cells[col50].Value = res.FilterLeakFreq;
            grid.Rows[10].Cells[col50].Value = res.FlangeLeakFreq;
            grid.Rows[11].Cells[col50].Value = res.ExchangerLeakFreq;
            grid.Rows[12].Cells[col50].Value = res.VaporizerLeakFreq;
            grid.Rows[13].Cells[col50].Value = res.ArmLeakFreq;
            grid.Rows[14].Cells[col50].Value = res.ExtraComp1LeakFreq;
            grid.Rows[15].Cells[col50].Value = res.ExtraComp2LeakFreq;

            grid.Rows[0].Cells[col95].Value = "-";
            grid.Rows[2].Cells[col95].Value = res.CompressorLeakFreq95;
            grid.Rows[3].Cells[col95].Value = res.VesselLeakFreq95;
            grid.Rows[4].Cells[col95].Value = res.ValveLeakFreq95;
            grid.Rows[5].Cells[col95].Value = res.InstrumentLeakFreq95;
            grid.Rows[6].Cells[col95].Value = res.JointLeakFreq95;
            grid.Rows[7].Cells[col95].Value = res.HoseLeakFreq95;
            grid.Rows[8].Cells[col95].Value = res.PipeLeakFreq95;
            grid.Rows[9].Cells[col95].Value = res.FilterLeakFreq95;
            grid.Rows[10].Cells[col95].Value = res.FlangeLeakFreq95;
            grid.Rows[11].Cells[col95].Value = res.ExchangerLeakFreq95;
            grid.Rows[12].Cells[col95].Value = res.VaporizerLeakFreq95;
            grid.Rows[13].Cells[col95].Value = res.ArmLeakFreq95;
            grid.Rows[14].Cells[col95].Value = res.ExtraComp1LeakFreq95;
            grid.Rows[15].Cells[col95].Value = res.ExtraComp2LeakFreq95;

            if (col50 != finalCol)
            {
                return;
            }

            if (!_state.FailureOverride.IsNull)
            {
                grid.Rows[17].Cells[col50].Value = _state.FailureOverride.GetValue();
                grid.Rows[18].Cells[1].Value = "(Note: provided by user)";
                grid.Rows[19].Cells[1].Value = "";
                grid.Rows[20].Cells[1].Value = "";
                grid.Rows[21].Cells[1].Value = "";
                grid.Rows[22].Cells[1].Value = "";
            }
            else
            {
                grid.Rows[17].Cells[col50].Value = res.TotalFreqOtherFailures;
                grid.Rows[18].Cells[col50].Value = res.FreqOverpressureRupture;
                grid.Rows[19].Cells[col50].Value = res.FreqDriveoffs;
                grid.Rows[20].Cells[col50].Value = res.FreqNozzleRelease;
                grid.Rows[21].Cells[col50].Value = res.FreqMValveFtc;
                grid.Rows[22].Cells[col50].Value = res.FreqSolValvesFtc;
                grid.Rows[17].Cells[col95].Value = res.TotalFreqOtherFailures95;
                grid.Rows[18].Cells[col95].Value = res.FreqOverpressureRupture95;
                grid.Rows[19].Cells[col95].Value = res.FreqDriveoffs95;
                grid.Rows[20].Cells[col95].Value = res.FreqNozzleRelease95;
                grid.Rows[21].Cells[col95].Value = res.FreqMValveFtc95;
                grid.Rows[22].Cells[col95].Value = res.FreqSolValvesFtc95;
            }
        }

        //private void PopulateCutSetTable(DataGridView grid, LeakResult leakRes, double? overrideVal, bool is100Leak = false)
        //{
        //    grid.Rows.Clear();

        //    if (overrideVal != null)
        //    {
        //        // if given, 100% release override replaces ALL values
        //        grid.Rows.Add(1, "Fluid Release (Override)", overrideVal);
        //        return;
        //    }

        //    grid.Rows.Add(1, "Compressor leak", leakRes.CompressorLeakFreq);
        //    grid.Rows.Add(2, "Vessel leak", leakRes.VesselLeakFreq);
        //    grid.Rows.Add(3, "Valve leak", leakRes.ValveLeakFreq);
        //    grid.Rows.Add(4, "Instrument leak", leakRes.InstrumentLeakFreq);
        //    grid.Rows.Add(5, "Joint leak", leakRes.JointLeakFreq);
        //    grid.Rows.Add(6, "Hose leak", leakRes.HoseLeakFreq);
        //    grid.Rows.Add(7, "Pipe leak", leakRes.PipeLeakFreq);
        //    grid.Rows.Add(8, "Filter leak", leakRes.FilterLeakFreq);
        //    grid.Rows.Add(9, "Flange leak", leakRes.FlangeLeakFreq);
        //    grid.Rows.Add(10, "Heat exchanger leak", leakRes.ExchangerLeakFreq);
        //    grid.Rows.Add(11, "Vaporizer leak", leakRes.VaporizerLeakFreq);
        //    grid.Rows.Add(12, "Loading arm leak", leakRes.ArmLeakFreq);
        //    grid.Rows.Add(13, "Extra component #1 leak", leakRes.ExtraComp1LeakFreq);
        //    grid.Rows.Add(14, "Extra component #2 leak", leakRes.ExtraComp2LeakFreq);

        //    // Display failure/accident data only for 100% leak
        //    if (is100Leak)
        //    {
        //        grid.Rows.Add();
        //        var rowIndex = 16;
        //        if (!_state.FailureOverride.IsNull)
        //        {
        //            grid.Rows.Add(rowIndex, "100% Fluid Release from Accidents and Shutdown Failures",
        //                          _state.FailureOverride.GetValue());
        //            grid.Rows.Add(rowIndex + 1, "(Note: provided by user)");
        //        }
        //        else
        //        {
        //            grid.Rows.Add(rowIndex, "100% Fluid Release from Accidents and Shutdown Failures",
        //                leakRes.TotalFreqOtherFailures);
        //            grid.Rows.Add(rowIndex + 1, "Overpressure during fueling induces rupture",
        //                leakRes.FreqOverpressureRupture);
        //            grid.Rows.Add(rowIndex + 2, "Release due to drive-offs", leakRes.FreqDriveoffs);
        //            grid.Rows.Add(rowIndex + 3, "Nozzle release", leakRes.FreqNozzleRelease);
        //            grid.Rows.Add(rowIndex + 4, "Manual valve fails to close", leakRes.FreqMValveFtc);
        //            grid.Rows.Add(rowIndex + 5, "Solenoid valves fail to close", leakRes.FreqSolValvesFtc);
        //        }
        //    }
        //} 

        private void dgRanking_SortCompare(object sender, DataGridViewSortCompareEventArgs e)
        {
            QuickFunctions.PerformNumericSortOnGrid(sender, e);
        }

        private void qraResultTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_hasImpulseResults && qraResultTabs.SelectedTab == impulseTab)
            {
                MessageBox.Show("Overpressure method 'Bauwens' does not produce impulse values");
                qraResultTabs.SelectedTab = riskMetricsTab;
            }

        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            var picBox = (PictureBox) sender;
            var pbName = picBox.Name;

            switch (e.Button)
            {
                case MouseButtons.Right:
                {
                }
                break;
            }
        }
    }
}
