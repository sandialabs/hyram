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

using Python.Runtime;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SandiaNationalLaboratories.Hyram
{
    public class QraInterface
    {
        public void Execute()
        {
            var inst = StateContainer.Instance;

            var resultsAreStale = StateContainer.GetValue<bool>("ResultsAreStale");
            // Just return stored result if no inputs have changed
            if (!resultsAreStale) return;

            // Gather inputs
            var pipeLength = StateContainer.GetNdValue("pipeLength", DistanceUnit.Meter);
            var numCompressors = (int)StateContainer.GetNdValue("numCompressors");
            var numCylinders = (int)StateContainer.GetNdValue("numCylinders");
            var numValves = (int)StateContainer.GetNdValue("numValves");
            var numInstruments = (int)StateContainer.GetNdValue("numInstruments");
            var numJoints = (int)StateContainer.GetNdValue("numJoints");
            var numHoses = (int)StateContainer.GetNdValue("numHoses");
            var numFilters = (int)StateContainer.GetNdValue("numFilters");
            var numFlanges = (int)StateContainer.GetNdValue("numFlanges");
            var numExtraComp1 = (int)StateContainer.GetNdValue("numExtraComponent1");
            var numExtraComp2 = (int)StateContainer.GetNdValue("numExtraComponent2");

            var facilLength = StateContainer.GetNdValue("facilityLength", DistanceUnit.Meter);
            var facilWidth = StateContainer.GetNdValue("facilityWidth", DistanceUnit.Meter);
            var facilHeight = StateContainer.GetNdValue("facilityHeight", DistanceUnit.Meter);

            var pipeOuterD = StateContainer.GetNdValue("pipeDiameter", DistanceUnit.Meter);
            var pipeThickness = StateContainer.GetNdValue("pipeThickness", DistanceUnit.Meter);

            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            double? relTemp = StateContainer.GetNdValue("fluidTemperature", TempUnit.Kelvin);
            var relPres = StateContainer.GetNdValue("fluidPressure", PressureUnit.Pa);
            var ambTemp = StateContainer.GetNdValue("ambientTemperature", TempUnit.Kelvin);
            var ambPres = StateContainer.GetNdValue("ambientPressure", PressureUnit.Pa);

            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) relTemp = null;  // clear temp if not gas

            var dischargeCoeff = StateContainer.GetNdValue("DischargeCoefficient");
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

            var detectGasAndFlame = inst.GasAndFlameDetectionOn;
            var gasDetectCredit = StateContainer.GetNdValue("PdetectIsolate");

            var probitThermalModelId = StateContainer.GetValue<ThermalProbitModel>("ThermalProbit").GetKey();
            var thermalExposureTime =
                StateContainer.GetNdValue("flameExposureTime", ElapsingTimeConversionUnit.Second);

            var probitOverpModelId =
                StateContainer.GetValue<OverpressureProbitModel>("OverpressureProbit").GetKey();
            var overpressureConsequences = StateContainer.GetNdValueList("OverpressureConsequences", PressureUnit.Pa);
            var impulses = StateContainer.GetNdValueList("Impulses", PressureUnit.Pa);
            // NOTE (Cianan): These aren't used yet
            double overpFragMass = 0;
            double overpVelocity = 0;
            double overpTotalMass = 0;

            var notionalNozzleModel = StateContainer.GetValue<NozzleModel>("NozzleModel").GetKey();
            var leakHeight = StateContainer.GetNdValue("LeakHeight", DistanceUnit.Meter);
            var releaseAngle = StateContainer.GetNdValue("ReleaseAngle", AngleUnit.Degrees);

            var exclusionRadius = StateContainer.GetNdValue("exclusionRadius");
            var randomSeed = (int)StateContainer.GetNdValue("randomSeed");
            var relativeHumid = StateContainer.GetNdValue("relativeHumidity");

            var occupantDistributions =
                (OccupantDistributionInfoCollection)inst.Parameters["OccupantDistributions"];
            //var occupantJson2 = JsonConvert.SerializeObject(occupantDistributions);
            var occupantJson = occupantDistributions.GetSimpleString();

            // Massage component probabilities into double[][]
            var compList =
                StateContainer.GetValue<List<ComponentProbability>>("Prob.Compressor");
            var
                cylList = StateContainer.GetValue<List<ComponentProbability>>("Prob.Cylinder");
            var filterList =
                StateContainer.GetValue<List<ComponentProbability>>("Prob.Filter");
            var flangeList =
                StateContainer.GetValue<List<ComponentProbability>>("Prob.Flange");
            var hoseList = StateContainer.GetValue<List<ComponentProbability>>("Prob.Hose");
            var jointList = StateContainer.GetValue<List<ComponentProbability>>("Prob.Joint");
            var pipeList = StateContainer.GetValue<List<ComponentProbability>>("Prob.Pipe");
            var valveList = StateContainer.GetValue<List<ComponentProbability>>("Prob.Valve");
            var instrList =
                StateContainer.GetValue<List<ComponentProbability>>("Prob.Instrument");
            var ex1List = StateContainer.GetValue<List<ComponentProbability>>("Prob.Extra1");
            var ex2List = StateContainer.GetValue<List<ComponentProbability>>("Prob.Extra2");

            double[][] compressorProbs =
            {
                compList[0].GetParameters(), compList[1].GetParameters(), compList[2].GetParameters(), compList[3].GetParameters(), compList[4].GetParameters()
            };

            double[][] cylinderProbs =
            {
                cylList[0].GetParameters(), cylList[1].GetParameters(), cylList[2].GetParameters(), cylList[3].GetParameters(), cylList[4].GetParameters()
            };

            double[][] filterProbs =
            {
                filterList[0].GetParameters(), filterList[1].GetParameters(), filterList[2].GetParameters(), filterList[3].GetParameters(), filterList[4].GetParameters()
            };

            double[][] flangeProbs =
            {
                flangeList[0].GetParameters(), flangeList[1].GetParameters(), flangeList[2].GetParameters(), flangeList[3].GetParameters(), flangeList[4].GetParameters()
            };

            double[][] hoseProbs =
            {
                hoseList[0].GetParameters(), hoseList[1].GetParameters(), hoseList[2].GetParameters(), hoseList[3].GetParameters(), hoseList[4].GetParameters()
            };

            double[][] jointProbs =
            {
                jointList[0].GetParameters(), jointList[1].GetParameters(), jointList[2].GetParameters(), jointList[3].GetParameters(), jointList[4].GetParameters()
            };

            double[][] pipeProbs =
            {
                pipeList[0].GetParameters(), pipeList[1].GetParameters(), pipeList[2].GetParameters(), pipeList[3].GetParameters(), pipeList[4].GetParameters()
            };

            double[][] valveProbs =
            {
                valveList[0].GetParameters(), valveList[1].GetParameters(), valveList[2].GetParameters(), valveList[3].GetParameters(), valveList[4].GetParameters()
            };

            double[][] instrumentProbs =
            {
                instrList[0].GetParameters(), instrList[1].GetParameters(), instrList[2].GetParameters(), instrList[3].GetParameters(), instrList[4].GetParameters()
            };

            double[][] extraComp1Probs =
            {
                ex1List[0].GetParameters(), ex1List[1].GetParameters(), ex1List[2].GetParameters(), ex1List[3].GetParameters(), ex1List[4].GetParameters()
            };

            double[][] extraComp2Probs =
            {
                ex2List[0].GetParameters(), ex2List[1].GetParameters(), ex2List[2].GetParameters(), ex2List[3].GetParameters(), ex2List[4].GetParameters()
            };
#if false
            foreach (double[] j in compressorProbs)
            {
                Trace.TraceInformation("{0}, {1}, {2}, {3}; ", j[0], j[1], j[2], j[3]);
            }
#endif

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
            bool isVerbose = false;
            //bool isVerbose = StateContainer.GetValue<bool>("debug");

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
                    resultPyObj = pyLib.qra.c_api.qra_analysis(
                        pipeLength, numCompressors, numCylinders, numValves, numInstruments, numJoints, numHoses,
                        numFilters, numFlanges, numExtraComp1, numExtraComp2,
                        facilLength, facilWidth, facilHeight,
                        pipeOuterD, pipeThickness,
                        relSpecies, relTemp, relPres, phaseKey, ambTemp, ambPres, dischargeCoeff,
                        numVehicles, numFuelingPerDay, numVehicleOpDays,
                        immediateIgnitionProbs, delayedIgnitionProbs, ignitionThresholds,
                        detectGasAndFlame, gasDetectCredit,
                        probitThermalModelId, thermalExposureTime,
                        probitOverpModelId, overpressureConsequences, impulses, overpFragMass, overpVelocity,
                        overpTotalMass,
                        notionalNozzleModel,
                        leakHeight, releaseAngle,
                        exclusionRadius, randomSeed, relativeHumid,
                        occupantJson,
                        compressorProbs, cylinderProbs, valveProbs, instrumentProbs, pipeProbs, jointProbs, hoseProbs,
                        filterProbs, flangeProbs, extraComp1Probs, extraComp2Probs,
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