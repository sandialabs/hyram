/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    /// <summary>
    /// Contains functions for interfacing with HyRAM+ python module QRA functions.
    /// Parameters are loaded from StateContainer.
    /// See python module cs_api.qra and hyram.qra.analysis for detailed function and parameter descriptions.
    /// </summary>
    public class QraInterface
    {
#if DEBUG
            readonly bool isVerbose = true;
#else
            readonly bool isVerbose = true;
#endif

        /// <summary>
        /// Initiates quantitative risk analysis of hydrogen release.
        /// </summary>
        public void Execute()
        {
            var _state = State.Data;

            var pipeLength = _state.PipeLength.GetValue();
            var numCompressors = (int) _state.NumCompressors.GetValue();
            var numVessels = (int) _state.NumVessels.GetValue();
            var numValves = (int) _state.NumValves.GetValue();
            var numInstruments = (int) _state.NumInstruments.GetValue();
            var numJoints = (int) _state.NumJoints.GetValue();
            var numHoses = (int) _state.NumHoses.GetValue();
            var numFilters = (int) _state.NumFilters.GetValue();
            var numFlanges = (int) _state.NumFlanges.GetValue();
            var numExchangers = (int) _state.NumExchangers.GetValue();
            var numVaporizers = (int) _state.NumVaporizers.GetValue();
            var numArms = (int) _state.NumArms.GetValue();
            var numExtraComp1 = (int) _state.NumExtraComponents1.GetValue();
            var numExtraComp2 = (int) _state.NumExtraComponents2.GetValue();

            var pipeDiameter = _state.PipeDiameter.GetValue();
            var pipeThickness = _state.PipeThickness.GetValue();

            double ambientP = _state.AmbientPressure.GetValue(PressureUnit.Pa);
            double ambientT = _state.AmbientTemperature.GetValue(TempUnit.Kelvin);
            double releaseP = _state.GetFluidPressure(PressureUnit.Pa);
            double? releaseT = _state.FluidTemperature.GetValue(TempUnit.Kelvin);
            if (!_state.DisplayTemperature()) releaseT = null;  // clear temp if not gas

            string phaseKey = _state.Phase.GetKey();

            var dischargeCoeff = _state.OrificeDischargeCoefficient.GetValue();
            var numVehicles = _state.VehicleCount.GetValue();
            var numFuelingPerDay = _state.VehicleFuelings.GetValue();
            var numVehicleOpDays = _state.VehicleDays.GetValue();
            var facilityLength = _state.FacilityLength.GetValue();
            var facilityWidth = _state.FacilityWidth.GetValue();

            double? massFlow = _state.QraMassFlow.GetValueMaybeNull();
            int massFlowLeakSize = _state.QraMassFlowLeakSize;

            var immediateIgnitionProbs = _state.ImmediateIgnitionProbs;
            var delayedIgnitionProbs = _state.DelayedIgnitionProbs;
            var ignitionThresholds = _state.IgnitionThresholds;

            double? h2Release000d01 = _state.Release000d01.GetValueMaybeNull();
            double? h2Release000d10 = _state.Release000d10.GetValueMaybeNull();
            double? h2Release001d00 = _state.Release001d00.GetValueMaybeNull();
            double? h2Release010d00 = _state.Release010d00.GetValueMaybeNull();
            double? h2Release100d00 = _state.Release100d00.GetValueMaybeNull();
            double? failureOverride = _state.FailureOverride.GetValueMaybeNull();
            double gasDetectCredit = _state.GasDetectionProb.GetValue();

            var overpMethod = _state.OverpressureMethod.GetKey();
            var tntFactor = _state.TntEquivalenceFactor.GetValue();
            double bstMachFlameSpeed = _state.OverpressureFlameSpeed;

            var thermalExposureTime = _state.FlameExposureTime.GetValue();

            var thermalProbitKey = _state.ThermalProbit.GetKey();
            var overpressureProbitKey = _state.OverpressureProbit.GetKey();

            var nozzle = _state.Nozzle.GetKey();
            var releaseAngle = _state.ReleaseAngle.GetValue(AngleUnit.Degrees);

            var exclusionRadius = _state.ExclusionRadius.GetValue();
            var randomSeed = (int) _state.RandomSeed.GetValue();
            var relativeHumid = _state.RelativeHumidity.GetValue();

            var occupantDistributions = _state.OccupantInfo;
            var occupantJson = occupantDistributions.GetSimpleString();

            var compressorProbs = _state.GetProbabilitiesAsArray(_state.ProbCompressor);
            var vesselProbs = _state.GetProbabilitiesAsArray(_state.ProbVessel);
            var filterProbs = _state.GetProbabilitiesAsArray(_state.ProbFilter);
            var flangeProbs = _state.GetProbabilitiesAsArray(_state.ProbFlange);
            var hoseProbs = _state.GetProbabilitiesAsArray(_state.ProbHose);
            var jointProbs = _state.GetProbabilitiesAsArray(_state.ProbJoint);
            var pipeProbs = _state.GetProbabilitiesAsArray(_state.ProbPipe);
            var valveProbs = _state.GetProbabilitiesAsArray(_state.ProbValve);
            var instrumentProbs = _state.GetProbabilitiesAsArray(_state.ProbInstrument);
            var exchangerProbs = _state.GetProbabilitiesAsArray(_state.ProbExchanger);
            var vaporizerProbs = _state.GetProbabilitiesAsArray(_state.ProbVaporizer);
            var armProbs = _state.GetProbabilitiesAsArray(_state.ProbArm);
            var extra1Probs = _state.GetProbabilitiesAsArray(_state.ProbExtra1);
            var extra2Probs = _state.GetProbabilitiesAsArray(_state.ProbExtra2);

            var nozPo = _state.FailureNozPo;
            var nozFtc = _state.FailureNozFtc;
            var mValveFtc = _state.FailureMValveFtc;
            var sValveFtc = _state.FailureSValveFtc;
            var sValveCcf = _state.FailureSValveCcf;

            var overpFail = _state.FailureOverp;
            var pValveFto = _state.FailurePValveFto;
            var driveoffFail = _state.FailureDriveoff;
            var couplingFtc = _state.FailureCouplingFtc;

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
                dynamic pyApi = Py.Import("cs_api");
                PyObject relSpecies = _state.GetFuelsPyDict();

                try
                {
                    // Activate python logging
                    pyApi.qra.setup(dataDirLoc, isVerbose);

                    Trace.TraceInformation("Executing QRA analysis...");
                    dynamic resultPyObj;

                    // Execute python analysis. Will return PyObject which is parsed in QRAResult.
                    resultPyObj = pyApi.qra.c_request_analysis(
                        pipeLength,
                        numCompressors, numVessels, numValves,
                        numInstruments, numJoints, numHoses,
                        numFilters, numFlanges,
                        numExchangers, numVaporizers, numArms,
                        numExtraComp1, numExtraComp2,
                        facilityLength, facilityWidth,
                        pipeDiameter, pipeThickness,
                        massFlow, massFlowLeakSize,
                        relSpecies, releaseT, releaseP, phaseKey, ambientT, ambientP, dischargeCoeff,
                        numVehicles, numFuelingPerDay, numVehicleOpDays,
                        immediateIgnitionProbs, delayedIgnitionProbs, ignitionThresholds,

                        gasDetectCredit,
                        overpMethod, tntFactor, bstMachFlameSpeed,
                        thermalProbitKey, thermalExposureTime,
                        overpressureProbitKey,

                        nozzle,
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
                        failureOverride,
                        dataDirLoc, isVerbose
                    );

                    bool status = (bool)resultPyObj["status"];
                    string statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        result = new QraResult(resultPyObj["data"]);
                        _state.QraResult = result;
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                        Console.Out.Flush();
                        Console.Error.Flush();
                        pyApi.Dispose();
                        throw new InvalidOperationException(statusMsg);
                    }
                }
                catch (PythonException ex)
                {
                    Trace.TraceError(ex.Message);
                    Debug.WriteLine(ex.Message);
                    Console.Out.Flush();
                    Console.Error.Flush();
                    pyApi.Dispose();
                    throw new InvalidOperationException(
                        "Something went wrong during QRA execution. Check log for details.");
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    throw ex;
                }
                finally
                {
                    // Force display of stderr, stdout. This includes print statements from python
                    Console.Out.Flush();
                    Console.Error.Flush();

                    // Unload imports
                    pyApi.Dispose();
                    GC.Collect();
                }
            }
        }
    }
}