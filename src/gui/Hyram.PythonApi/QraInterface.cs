/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using Python.Runtime;

using System;
using System.Diagnostics;

namespace SandiaNationalLaboratories.Hyram
{
    public class QraInterface
    {
#if DEBUG
            bool isVerbose = true;
#else
            bool isVerbose = true;
#endif

        // Quantitative risk analysis of hydrogen release.
        // See python module hyram.qra.analysis for descriptions of parameters and analysis.
        public void Execute()
        {
            var inst = StateContainer.Instance;

            var resultsAreStale = StateContainer.GetValue<bool>("ResultsAreStale");
            // Just return stored result if no inputs have changed
            //if (!resultsAreStale) return;

            // Gather inputs
            var pipeLength = StateContainer.GetNdValue("pipeLength", DistanceUnit.Meter);
            var numCompressors = (int)StateContainer.GetNdValue("numCompressors");
            var numVessels = (int)StateContainer.GetNdValue("numVessels");
            var numValves = (int)StateContainer.GetNdValue("numValves");
            var numInstruments = (int)StateContainer.GetNdValue("numInstruments");
            var numJoints = (int)StateContainer.GetNdValue("numJoints");
            var numHoses = (int)StateContainer.GetNdValue("numHoses");
            var numFilters = (int)StateContainer.GetNdValue("numFilters");
            var numFlanges = (int)StateContainer.GetNdValue("numFlanges");
            var numExchangers = (int)StateContainer.GetNdValue("numExchangers");
            var numVaporizers = (int)StateContainer.GetNdValue("numVaporizers");
            var numArms = (int)StateContainer.GetNdValue("numArms");
            var numExtraComp1 = (int)StateContainer.GetNdValue("numExtraComponent1");
            var numExtraComp2 = (int)StateContainer.GetNdValue("numExtraComponent2");

            var facilLength = StateContainer.GetNdValue("facilityLength", DistanceUnit.Meter);
            var facilWidth = StateContainer.GetNdValue("facilityWidth", DistanceUnit.Meter);

            var pipeOuterD = StateContainer.GetNdValue("pipeDiameter", DistanceUnit.Meter);
            var pipeThickness = StateContainer.GetNdValue("pipeThickness", DistanceUnit.Meter);

            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            double? relTemp = StateContainer.GetNdValue("fluidTemperature", TempUnit.Kelvin);
            double relPres = StateContainer.GetNdValue("fluidPressure", PressureUnit.Pa);
            double ambTemp = StateContainer.GetNdValue("ambientTemperature", TempUnit.Kelvin);
            double ambPres = StateContainer.GetNdValue("ambientPressure", PressureUnit.Pa);

            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) relTemp = null;  // clear temp if not gas

            var dischargeCoeff = StateContainer.GetNdValue("orificeDischargeCoefficient");
            var numVehicles = StateContainer.GetNdValue("numVehicles");
            var numFuelingPerDay = StateContainer.GetNdValue("dailyFuelings");
            var numVehicleOpDays = StateContainer.GetNdValue("vehicleOperatingDays");

            var immediateIgnitionProbs = (double[])inst.Parameters["ImmedIgnitionProbs"];
            var delayedIgnitionProbs = (double[])inst.Parameters["DelayIgnitionProbs"];
            var ignitionThresholds = (double[])inst.Parameters["IgnitionThresholds"];

            var h2Release000d01 = StateContainer.GetNdValue("H2Release.000d01");
            var h2Release000d10 = StateContainer.GetNdValue("H2Release.000d10");
            var h2Release001d00 = StateContainer.GetNdValue("H2Release.001d00");
            var h2Release010d00 = StateContainer.GetNdValue("H2Release.010d00");
            var h2Release100d00 = StateContainer.GetNdValue("H2Release.100d00");
            var failureManualOverride = StateContainer.GetNdValue("Failure.ManualOverride");

            var gasDetectCredit = StateContainer.GetNdValue("PdetectIsolate");

            var overpMethod =
                StateContainer.GetValue<UnconfinedOverpressureMethod>("unconfinedOverpressureMethod").GetKey();
            var tntFactor = StateContainer.GetNdValue("tntEquivalenceFactor");
            var bstMachFlameSpeed = StateContainer.GetNdValue("overpressureFlameSpeed");

            var probitThermalId = StateContainer.GetValue<ThermalProbitModel>("ThermalProbit").GetKey();
            var thermalExposureTime =
                StateContainer.GetNdValue("flameExposureTime", ElapsingTimeConversionUnit.Second);

            var probitOverpId =
                StateContainer.GetValue<OverpressureProbitModel>("OverpressureProbit").GetKey();

            var notionalNozzleModel = StateContainer.GetValue<NozzleModel>("NozzleModel").GetKey();
            var releaseAngle = StateContainer.GetNdValue("ReleaseAngle", AngleUnit.Degrees);

            var exclusionRadius = StateContainer.GetNdValue("exclusionRadius");
            var randomSeed = (int)StateContainer.GetNdValue("randomSeed");
            var relativeHumid = StateContainer.GetNdValue("relativeHumidity");

            var occupantDistributions =
                (OccupantDistributionInfoCollection)inst.Parameters["OccupantDistributions"];
            //var occupantJson2 = JsonConvert.SerializeObject(occupantDistributions);
            var occupantJson = occupantDistributions.GetSimpleString();

            double[][] compressorProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Compressor");
            double[][] vesselProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Vessel");
            double[][] filterProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Filter");
            double[][] flangeProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Flange");
            double[][] hoseProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Hose");
            double[][] jointProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Joint");
            double[][] pipeProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Pipe");
            double[][] valveProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Valve");
            double[][] instrumentProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Instrument");
            double[][] exchangerProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Exchanger");
            double[][] vaporizerProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Vaporizer");
            double[][] armProbs = StateContainer.Instance.GetComponentProbabilities("Prob.Arm");
            double[][] extra1Probs = StateContainer.Instance.GetComponentProbabilities("Prob.Extra1");
            double[][] extra2Probs = StateContainer.Instance.GetComponentProbabilities("Prob.Extra2");

            var nozPo = StateContainer.GetValue<FailureMode>("Failure.NozPO");
            var nozFtc = StateContainer.GetValue<FailureMode>("Failure.NozFTC");
            var mValveFtc = StateContainer.GetValue<FailureMode>("Failure.MValveFTC");
            var sValveFtc = StateContainer.GetValue<FailureMode>("Failure.SValveFTC");
            var sValveCcf = StateContainer.GetValue<FailureMode>("Failure.SValveCCF");

            var overpFail = StateContainer.GetValue<FailureMode>("Failure.Overp");
            var pValveFto = StateContainer.GetValue<FailureMode>("Failure.PValveFTO");
            var driveoffFail = StateContainer.GetValue<FailureMode>("Failure.Driveoff");
            var couplingFtc = StateContainer.GetValue<FailureMode>("Failure.CouplingFTC");

            var nozPoDist = nozPo.Dist.ToString();
            var nozPoParamA = nozPo.ParamA;
            var nozPoParamB = nozPo.ParamB;

            var nozFtcDist = nozFtc.Dist.ToString();
            var nozFtcParamA = nozFtc.ParamA;
            var nozFtcParamB = nozFtc.ParamB;

            var mValveFtcDist = mValveFtc.Dist.ToString();
            var mValveFtcParamA = mValveFtc.ParamA;
            var mValveFtcParamB = mValveFtc.ParamB;

            var sValveFtcDist = sValveFtc.Dist.ToString();
            var sValveFtcParamA = sValveFtc.ParamA;
            var sValveFtcParamB = sValveFtc.ParamB;

            var sValveCcfDist = sValveCcf.Dist.ToString();
            var sValveCcfParamA = sValveCcf.ParamA;
            var sValveCcfParamB = sValveCcf.ParamB;

            var overpDist = overpFail.Dist.ToString();
            var overpParamA = overpFail.ParamA;
            var overpParamB = overpFail.ParamB;

            var pValveFtoDist = pValveFto.Dist.ToString();
            var pValveFtoParamA = pValveFto.ParamA;
            var pValveFtoParamB = pValveFto.ParamB;

            var driveoffDist = driveoffFail.Dist.ToString();
            var driveoffParamA = driveoffFail.ParamA;
            var driveoffParamB = driveoffFail.ParamB;

            var couplingFtcDist = couplingFtc.Dist.ToString();
            var couplingFtcParamA = couplingFtc.ParamA;
            var couplingFtcParamB = couplingFtc.ParamB;

            // Derive path to data dir for temp and data files, e.g. pickling
            var dataDirLoc = StateContainer.UserDataDir;

            QraResult result;

            using (Py.GIL())
            {
                dynamic pyLib = Py.Import("hyram");

                try
                {
                    // Activate python logging
                    pyLib.qra.c_api.setup(dataDirLoc, isVerbose);

                    Trace.TraceInformation("Executing QRA analysis...");
                    dynamic resultPyObj;

                    // Execute python analysis. Will return PyObject which is parsed in QRAResult.
                    resultPyObj = pyLib.qra.c_api.c_request_analysis(
                        pipeLength,
                        numCompressors, numVessels, numValves,
                        numInstruments, numJoints, numHoses,
                        numFilters, numFlanges,
                        numExchangers, numVaporizers, numArms,
                        numExtraComp1, numExtraComp2,
                        facilLength, facilWidth,
                        pipeOuterD, pipeThickness,
                        relSpecies, relTemp, relPres, phaseKey, ambTemp, ambPres, dischargeCoeff,
                        numVehicles, numFuelingPerDay, numVehicleOpDays,
                        immediateIgnitionProbs, delayedIgnitionProbs, ignitionThresholds,

                        gasDetectCredit,
                        overpMethod, tntFactor, bstMachFlameSpeed,
                        probitThermalId, thermalExposureTime,
                        probitOverpId,

                        notionalNozzleModel,
                        releaseAngle,
                        exclusionRadius, randomSeed, relativeHumid,
                        occupantJson,
                        compressorProbs, vesselProbs, valveProbs, instrumentProbs, pipeProbs, jointProbs, hoseProbs,
                        filterProbs, flangeProbs, exchangerProbs, vaporizerProbs, armProbs, extra1Probs, extra2Probs,
                        nozPoDist, nozPoParamA, nozPoParamB,
                        nozFtcDist, nozFtcParamA, nozFtcParamB,
                        mValveFtcDist, mValveFtcParamA, mValveFtcParamB,
                        sValveFtcDist, sValveFtcParamA, sValveFtcParamB,
                        sValveCcfDist, sValveCcfParamA, sValveCcfParamB,
                        overpDist, overpParamA, overpParamB,
                        pValveFtoDist, pValveFtoParamA, pValveFtoParamB,
                        driveoffDist, driveoffParamA, driveoffParamB,
                        couplingFtcDist, couplingFtcParamA, couplingFtcParamB,
                        h2Release000d01, h2Release000d10, h2Release001d00, h2Release010d00, h2Release100d00,
                        failureManualOverride,
                        dataDirLoc, isVerbose
                    );

                    bool status = (bool)resultPyObj["status"];
                    string statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        result = new QraResult(resultPyObj["data"]);
                        StateContainer.SetValue("ResultsAreStale", false);
                        StateContainer.SetValue("Result", result);
                    }
                    else
                    {
                        StateContainer.SetValue("ResultsAreStale", true);
                        Trace.TraceError(statusMsg);
                        Console.Out.Flush();
                        Console.Error.Flush();
                        pyLib.Dispose();
                        throw new InvalidOperationException(statusMsg);
                    }
                }
                catch (PythonException ex)
                {
                    StateContainer.SetValue("ResultsAreStale", true);
                    Trace.TraceError(ex.Message);
                    Debug.WriteLine(ex.Message);
                    Console.Out.Flush();
                    Console.Error.Flush();
                    pyLib.Dispose();
                    throw new InvalidOperationException(
                        "Something went wrong during QRA execution. Check log for details.");
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    StateContainer.SetValue("ResultsAreStale", true);
                    throw ex;
                }
                finally
                {
                    // Force display of stderr, stdout. This includes print statements from python
                    Console.Out.Flush();
                    Console.Error.Flush();

                    // Unload imports
                    pyLib.Dispose();
                    //PythonEngine.ReleaseLock(lck);
                    GC.Collect();
                }
            }
        }
    }
}