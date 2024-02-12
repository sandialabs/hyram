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
using Python.Runtime;

namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    /// Results for single leak scenario of QRA analysis
    /// </summary>
    public class LeakResult
    {
        public string LeakSize;

        // these are 50% values for UQ analysis
        public double CompressorLeakFreq;
        public double VesselLeakFreq;
        public double ValveLeakFreq;
        public double InstrumentLeakFreq;
        public double JointLeakFreq;
        public double HoseLeakFreq;
        public double PipeLeakFreq;
        public double FilterLeakFreq;
        public double FlangeLeakFreq;
        public double ExchangerLeakFreq;
        public double VaporizerLeakFreq;
        public double ArmLeakFreq;
        public double ExtraComp1LeakFreq;
        public double ExtraComp2LeakFreq;

        public double CompressorLeakFreq95;
        public double VesselLeakFreq95;
        public double ValveLeakFreq95;
        public double InstrumentLeakFreq95;
        public double JointLeakFreq95;
        public double HoseLeakFreq95;
        public double PipeLeakFreq95;
        public double FilterLeakFreq95;
        public double FlangeLeakFreq95;
        public double ExchangerLeakFreq95;
        public double VaporizerLeakFreq95;
        public double ArmLeakFreq95;
        public double ExtraComp1LeakFreq95;
        public double ExtraComp2LeakFreq95;

        public double MassFlowRate;
        public double LeakDiam;

        // Accident and shutdown failures for 100% leak size
        public double ProbOverpressureRupture;
        public double FreqOverpressureRupture;
        public double ProbDriveoffs;
        public double FreqDriveoffs;
        public double ProbNozzleRelease;
        public double FreqNozzleRelease;
        public double ProbMValveFtc;
        public double FreqMValveFtc;
        public double ProbSolValvesFtc;
        public double FreqSolValvesFtc;
        public double TotalFreqOtherFailures;

        public double ProbShutdown;
        public double ShutdownAvgEvents;
        public double ShutdownAvgEventsMax;

        public double ProbJetfire;
        public double JetfireAvgEvents;
        public double JetfireAvgEventsMax;
        public double JetfirePllContrib;

        public double ProbNoIgnition;
        public double NoIgnAvgEvents;
        public double NoIgnAvgEventsMax;

        public double ProbExplosion;
        public double ExplosAvgEvents;
        public double ExplosAvgEventsMax;
        public double ExplosionPllContrib;

        // UQ 95% data
        public double TotalFreqOtherFailures95;
        public double FreqOverpressureRupture95;
        public double FreqDriveoffs95;
        public double FreqNozzleRelease95;
        public double FreqMValveFtc95;
        public double FreqSolValvesFtc95;


        public string GetLeakSizeString()
        {
            return LeakSize + "% Release";
        }

        public override string ToString()
        {
            var summary = $@"Leak result {LeakSize}:
                P jetfire: {ProbJetfire}
                P explos: {ProbExplosion}
                P no ign: {ProbNoIgnition}
                P shutd: {ProbShutdown}
                ";
            return summary;
        }
    }

    /// <summary>
    /// Container for QRA analysis results returned by python module analysis.
    /// </summary>
    public class QraResult
    {
        private readonly StateContainer _state = State.Data;
        public double Air;
        public double Far;
        public List<LeakResult> LeakResults;
        public double TotalPll;
        public double[][] Positions;
        public string[] QradPlotFiles;
        public double[][] PositionQrads;  // kW
        public string[] OverpressurePlotFiles;
        public double[][] PositionOverpressures;  // Pa
        public string[] ImpulsePlotFiles;
        public double[][] PositionImpulses;  // Pa*s

        public QraResult(PyObject pyResult)
        {
            // Convert PyObject attrs to double with double-cast
            Air = (double) (dynamic) pyResult["air"];
            TotalPll = (double) (dynamic) pyResult["total_pll"];
            Far = (double) (dynamic) pyResult["far"];
            Positions = (double[][])(dynamic)pyResult["positions"];

            PositionQrads = (double[][])(dynamic)pyResult["position_qrads"];
            QradPlotFiles = (string[])(dynamic)pyResult["qrad_plot_files"];

            PositionOverpressures = (double[][])(dynamic)pyResult["position_overps"];
            OverpressurePlotFiles = (string[])(dynamic)pyResult["overp_plot_files"];

            PositionImpulses = (double[][])(dynamic)pyResult["position_impulses"];
            ImpulsePlotFiles = (string[])(dynamic)pyResult["impulse_plot_files"];

            // Parse scenario data for each leak size into objects
            dynamic leakResultData = pyResult["leak_results"];
            LeakResults = new List<LeakResult>();

            for (var i = 0; i < 5; i++)
            {
                var res = leakResultData[i];
                dynamic eventDicts = res["event_dicts"];
                int numEvents = ((double[]) res["list_p_events"]).Length;

                double probShutdown = 0;
                double shutdownAvgEvents = 0;
                double shutdownAvgEventsMax = 0;

                double probJetfire = 0;
                double jetfireAvgEvents = 0;
                double jetfireAvgEventsMax = 0;
                double jetfirePllContrib = 0;

                double probNoIgnition = 0;
                double noIgnAvgEvents = 0;
                double noIgnAvgEventsMax = 0;

                double probExplosion = 0;
                double explosAvgEvents = 0;
                double explosAvgEventsMax = 0;
                double explosionPllContrib = 0;

                // TODO: extract min, max values when uncertain
                // Fill parameters from possibly-unordered list of dicts via key comparison.
                for (var j = 0; j < numEvents; j++)
                {
                    var eventDict = eventDicts[j];
                    string key = (string) eventDict["key"];
                    if (key == "shut")
                    {
                        probShutdown = (double) eventDict["prob"];
                        shutdownAvgEvents = (double) eventDict["events"];
                    }
                    else if (key == "noig")
                    {
                        probNoIgnition = (double) eventDict["prob"];
                        noIgnAvgEvents = (double) eventDict["events"];
                    }
                    else if (key == "jetf")
                    {
                        probJetfire = (double) eventDict["prob"];
                        jetfireAvgEvents = (double) eventDict["events"];
                        jetfirePllContrib = (double) eventDict["pll"];
                    }
                    else if (key == "expl")
                    {
                        probExplosion = (double) eventDict["prob"];
                        explosAvgEvents = (double) eventDict["events"];
                        explosionPllContrib = (double) eventDict["pll"];
                    }
                    else if (key == "tot")
                    {
                        ;
                    }
                    else
                    {
                        throw new InvalidOperationException("Event type not recognized");
                    }

                }

                var leakSize = ((double) res["leak_size"]).ToString("000.00");
                var massFlowRate = (double) res["mass_flow_rate"];
                var leakDiam = (double) res["leak_diam"];

                var compressorLeakFreq = (double) res["f_component_leaks"]["compressor"];
                var vesselLeakFreq = (double) res["f_component_leaks"]["vessel"];
                var valveLeakFreq = (double) res["f_component_leaks"]["valve"];
                var instrumentLeakFreq = (double) res["f_component_leaks"]["instrument"];
                var jointLeakFreq = (double) res["f_component_leaks"]["joint"];
                var hoseLeakFreq = (double) res["f_component_leaks"]["hose"];
                var pipeLeakFreq = (double) res["f_component_leaks"]["pipe"];
                var filterLeakFreq = (double) res["f_component_leaks"]["filter"];
                var flangeLeakFreq = (double) res["f_component_leaks"]["flange"];
                var exchangerLeakFreq = (double) res["f_component_leaks"]["exchanger"];
                var vaporizerLeakFreq = (double) res["f_component_leaks"]["vaporizer"];
                var armLeakFreq = (double) res["f_component_leaks"]["arm"];
                var extraComp1LeakFreq = (double) res["f_component_leaks"]["extra1"];
                var extraComp2LeakFreq = (double) res["f_component_leaks"]["extra2"];

                var result = new LeakResult
                {
                    LeakSize = leakSize,
                    ProbJetfire = probJetfire,
                    JetfireAvgEvents = jetfireAvgEvents,
                    JetfireAvgEventsMax = jetfireAvgEventsMax,
                    JetfirePllContrib = jetfirePllContrib,

                    ProbExplosion = probExplosion,
                    ExplosAvgEvents = explosAvgEvents,
                    ExplosAvgEventsMax = explosAvgEventsMax,
                    ExplosionPllContrib = explosionPllContrib,

                    ProbShutdown = probShutdown,
                    ShutdownAvgEvents = shutdownAvgEvents,
                    ShutdownAvgEventsMax = shutdownAvgEventsMax,

                    ProbNoIgnition = probNoIgnition,
                    NoIgnAvgEvents = noIgnAvgEvents,
                    NoIgnAvgEventsMax = noIgnAvgEventsMax,

                    MassFlowRate = massFlowRate,
                    LeakDiam = leakDiam,

                    CompressorLeakFreq = compressorLeakFreq,
                    VesselLeakFreq = vesselLeakFreq,
                    ValveLeakFreq = valveLeakFreq,
                    InstrumentLeakFreq = instrumentLeakFreq,
                    JointLeakFreq = jointLeakFreq,
                    HoseLeakFreq = hoseLeakFreq,
                    PipeLeakFreq = pipeLeakFreq,
                    FilterLeakFreq = filterLeakFreq,
                    FlangeLeakFreq = flangeLeakFreq,
                    ExchangerLeakFreq = exchangerLeakFreq,
                    VaporizerLeakFreq = vaporizerLeakFreq,
                    ArmLeakFreq = armLeakFreq,
                    ExtraComp1LeakFreq = extraComp1LeakFreq,
                    ExtraComp2LeakFreq = extraComp2LeakFreq
                };

                if (_state.IsUncertain())
                {
                    // TODO: enable once available
                    //result.CompressorLeakFreq95 = (double)res["f_component_leaks"]["compressor95"];
                    //result.VesselLeakFreq95 = (double)res["f_component_leaks"]["vessel95"];
                    //result.ValveLeakFreq95 = (double)res["f_component_leaks"]["valve95"];
                    //result.InstrumentLeakFreq95 = (double)res["f_component_leaks"]["instrument95"];
                    //result.JointLeakFreq95 = (double)res["f_component_leaks"]["joint95"];
                    //result.HoseLeakFreq95 = (double)res["f_component_leaks"]["hose95"];
                    //result.PipeLeakFreq95 = (double)res["f_component_leaks"]["pipe95"];
                    //result.FilterLeakFreq95 = (double)res["f_component_leaks"]["filter95"];
                    //result.FlangeLeakFreq95 = (double)res["f_component_leaks"]["flange95"];
                    //result.ExchangerLeakFreq95 = (double)res["f_component_leaks"]["exchanger95"];
                    //result.VaporizerLeakFreq95 = (double)res["f_component_leaks"]["vaporizer95"];
                    //result.ArmLeakFreq95 = (double)res["f_component_leaks"]["arm95"];
                    //result.ExtraComp1LeakFreq95 = (double)res["f_component_leaks"]["extra195"];
                    //result.ExtraComp2LeakFreq95 = (double)res["f_component_leaks"]["extra295"];
                }

                if (i == 4)
                {
                    // Load shutdown/accident failure data for 100% leak size
                    if (_state.FailureOverride.IsNull)
                    {
                        result.ProbOverpressureRupture = (double) res["p_overp_rupture"];
                        result.FreqOverpressureRupture = (double) res["f_overp_rupture"];

                        result.ProbDriveoffs = (double) res["p_driveoff"];
                        result.FreqDriveoffs = (double) res["f_driveoff"];

                        result.ProbSolValvesFtc = (double) res["p_sol_valves_ftc"];
                        result.FreqSolValvesFtc = (double) res["f_sol_valves_ftc"];

                        result.ProbMValveFtc = (double) res["p_mvalve_ftc"];
                        result.FreqMValveFtc = (double) res["f_mvalve_ftc"];

                        result.ProbNozzleRelease = (double) res["p_nozzle_release"];
                        result.FreqNozzleRelease = (double) res["f_nozzle_release"];

                        result.TotalFreqOtherFailures = (double) res["f_failure"];

                        if (_state.IsUncertain())
                        {
                            //result.TotalFreqOtherFailures95 = (double) res["f_failure95"];
                            //result.FreqOverpressureRupture = (double) res["f_overp_rupture95"];
                            //result.FreqDriveoffs95 = (double) res["f_driveoff95"];
                            //result.FreqSolValvesFtc95 = (double) res["f_sol_valves_ftc95"];
                            //result.FreqMValveFtc95 = (double) res["f_mvalve_ftc95"];
                            //result.FreqNozzleRelease95 = (double) res["f_nozzle_release95"];
                        }
                    }
                    else
                    {
                        result.TotalFreqOtherFailures = _state.FailureOverride.GetValue();
                    }
                }
                LeakResults.Add(result);
            }
        }
    }
}