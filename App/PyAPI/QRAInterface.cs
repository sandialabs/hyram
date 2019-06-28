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
using System.Collections.Generic;
using System.Diagnostics;
using JrConversions;
using Newtonsoft.Json;
using Python.Runtime;
using QRAState;

namespace PyAPI
{
    public class PyQrAnalysis
    {
        public double[] Pll { get; set; }
        public double[] LeakProbs { get; set; }
        public double[] PJetFire { get; set; }
        public double[] PExplosion { get; set; }
        public double[] FatalJetFire { get; set; }
        public double[] FatalExplosion { get; set; }
        public double TotalPll { get; set; }
        public double Far { get; set; }
        public double Air { get; set; }
        public string[] PositionPlotFilenames { get; set; }


        public void Execute()
        {
            var inst = QraStateContainer.Instance;

            var resultsAreStale = QraStateContainer.GetValue<bool>("ResultsAreStale");
            // Just return stored result if no inputs have changed
            if (!resultsAreStale) return;

            // Gather inputs
            var pipeLength = QraStateContainer.GetNdValue("Components.PipeLength", DistanceUnit.Meter);
            var numCompressors = (int) QraStateContainer.GetNdValue("Components.NrCompressors");
            var numCylinders = (int) QraStateContainer.GetNdValue("Components.NrCylinders");
            var numValves = (int) QraStateContainer.GetNdValue("Components.NrValves");
            var numInstruments = (int) QraStateContainer.GetNdValue("Components.NrInstruments");
            var numJoints = (int) QraStateContainer.GetNdValue("Components.NrJoints");
            var numHoses = (int) QraStateContainer.GetNdValue("Components.NrHoses");
            var numFilters = (int) QraStateContainer.GetNdValue("Components.NrFilters");
            var numFlanges = (int) QraStateContainer.GetNdValue("Components.NrFlanges");
            var numExtraComp1 = (int) QraStateContainer.GetNdValue("Components.NrExtraComp1");
            var numExtraComp2 = (int) QraStateContainer.GetNdValue("Components.NrExtraComp2");

            var facilLength = QraStateContainer.GetNdValue("Facility.Length", DistanceUnit.Meter);
            var facilWidth = QraStateContainer.GetNdValue("Facility.Width", DistanceUnit.Meter);
            var facilHeight = QraStateContainer.GetNdValue("Facility.Height", DistanceUnit.Meter);

            var pipeOuterD = QraStateContainer.GetNdValue("SysParam.PipeOD", DistanceUnit.Meter);
            var pipeThickness = QraStateContainer.GetNdValue("SysParam.PipeWallThick", DistanceUnit.Meter);
            var h2Temp = QraStateContainer.GetNdValue("SysParam.InternalTempC", TempUnit.Kelvin);
            var h2Pres = QraStateContainer.GetNdValue("SysParam.InternalPresMPA", PressureUnit.Pa);
            var ambTemp = QraStateContainer.GetNdValue("SysParam.ExternalTempC", TempUnit.Kelvin);
            var ambPres = QraStateContainer.GetNdValue("SysParam.ExternalPresMPa", PressureUnit.Pa);

            var dischargeCoeff = QraStateContainer.GetNdValue("DischargeCoefficient");
            var numVehicles = QraStateContainer.GetNdValue("nVehicles");
            var numFuelingPerDay = QraStateContainer.GetNdValue("nFuelingsPerVehicleDay");
            var numVehicleOpDays = QraStateContainer.GetNdValue("nVehicleOperatingDays");

            var immediateIgnitionProbs = (double[]) inst.Parameters["ImmedIgnitionProbs"];
            var delayedIgnitionProbs = (double[]) inst.Parameters["DelayIgnitionProbs"];
            var ignitionThresholds = (double[]) inst.Parameters["IgnitionThresholds"];

            var h2Release000d01 = QraStateContainer.GetNdValue("H2Release.000d01");
            var h2Release000d10 = QraStateContainer.GetNdValue("H2Release.000d10");
            var h2Release001d00 = QraStateContainer.GetNdValue("H2Release.001d00");
            var h2Release010d00 = QraStateContainer.GetNdValue("H2Release.010d00");
            var h2Release100d00 = QraStateContainer.GetNdValue("H2Release.100d00");
            var failureManualOverride = QraStateContainer.GetNdValue("Failure.ManualOverride");
            //double? FailureManualOverride = QraStateContainer.GetValue<double?>("Failure.ManualOverride");

            // Note (Cianan): this is currently always true
            var detectGasAndFlame = inst.GasAndFlameDetectionOn;
            var gasDetectCredit = QraStateContainer.GetNdValue("PdetectIsolate");

            var probitThermalModelId = QraStateContainer.GetValue<ThermalProbitModel>("ThermalProbit").GetKey();
            var thermalExposureTime =
                QraStateContainer.GetNdValue("t_expose_thermal", ElapsingTimeConversionUnit.Second);

            var probitOverpModelId =
                QraStateContainer.GetValue<OverpressureProbitModel>("OverpressureProbit").GetKey();
            var peakOverpressures = QraStateContainer.GetNdValueList("P_s", PressureUnit.Pa);
            var overpImpulses = QraStateContainer.GetNdValueList("impulse", PressureUnit.Pa);
            // NOTE (Cianan): These aren't used yet
            double overpFragMass = 0;
            double overpVelocity = 0;
            double overpTotalMass = 0;

            var radSourceModel =
                QraStateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel").ToString();
            var notionalNozzleModel = QraStateContainer.GetValue<NozzleModel>("NozzleModel").GetKey();
            var leakHeight = QraStateContainer.GetNdValue("LeakHeight", DistanceUnit.Meter);
            var releaseAngle = QraStateContainer.GetNdValue("ReleaseAngle", AngleUnit.Degrees);

            var exclusionRadius = QraStateContainer.GetNdValue("QRAD:EXCLUSIONRADIUS");
            var randomSeed = (int) QraStateContainer.GetNdValue("RANDOMSEED");
            var relativeHumid = QraStateContainer.GetNdValue("FlameWrapper.RH");

            var occupantDistributions =
                (OccupantDistributionInfoCollection) inst.Parameters["OccupantDistributions"];
            var occupantJson = JsonConvert.SerializeObject(occupantDistributions);

            // Massage component probabilities into double[][]
            var compList =
                QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Compressor");
            var
                cylList = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Cylinder");
            var filterList =
                QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Filter");
            var flangeList =
                QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Flange");
            var hoseList = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Hose");
            var jointList = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Joint");
            var pipeList = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Pipe");
            var valveList = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Valve");
            var instrList =
                QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Instrument");
            var ex1List = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Extra1");
            var ex2List = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Extra2");

            // TODO (Cianan): Clean this up
            // NOTE (Cianan): Python.NET can't convert nullable list (e.g. double?) into numpy arr so  mu/sigma or mean/variance are set to -1000D if null
            double[][] compressorProbs =
            {
                compList[0].GetDataForPython(), compList[1].GetDataForPython(),
                compList[2].GetDataForPython(), compList[3].GetDataForPython(),
                compList[4].GetDataForPython()
            };

            double[][] cylinderProbs =
            {
                cylList[0].GetDataForPython(), cylList[1].GetDataForPython(),
                cylList[2].GetDataForPython(), cylList[3].GetDataForPython(),
                cylList[4].GetDataForPython()
            };

            double[][] filterProbs =
            {
                filterList[0].GetDataForPython(), filterList[1].GetDataForPython(),
                filterList[2].GetDataForPython(), filterList[3].GetDataForPython(),
                filterList[4].GetDataForPython()
            };

            double[][] flangeProbs =
            {
                flangeList[0].GetDataForPython(), flangeList[1].GetDataForPython(),
                flangeList[2].GetDataForPython(), flangeList[3].GetDataForPython(),
                flangeList[4].GetDataForPython()
            };

            double[][] hoseProbs =
            {
                hoseList[0].GetDataForPython(), hoseList[1].GetDataForPython(),
                hoseList[2].GetDataForPython(), hoseList[3].GetDataForPython(),
                hoseList[4].GetDataForPython()
            };

            double[][] jointProbs =
            {
                jointList[0].GetDataForPython(), jointList[1].GetDataForPython(),
                jointList[2].GetDataForPython(), jointList[3].GetDataForPython(),
                jointList[4].GetDataForPython()
            };

            double[][] pipeProbs =
            {
                pipeList[0].GetDataForPython(), pipeList[1].GetDataForPython(),
                pipeList[2].GetDataForPython(), pipeList[3].GetDataForPython(),
                pipeList[4].GetDataForPython()
            };

            double[][] valveProbs =
            {
                valveList[0].GetDataForPython(), valveList[1].GetDataForPython(),
                valveList[2].GetDataForPython(), valveList[3].GetDataForPython(),
                valveList[4].GetDataForPython()
            };

            double[][] instrumentProbs =
            {
                instrList[0].GetDataForPython(), instrList[1].GetDataForPython(),
                instrList[2].GetDataForPython(), instrList[3].GetDataForPython(),
                instrList[4].GetDataForPython()
            };

            double[][] extraComp1Probs =
            {
                ex1List[0].GetDataForPython(), ex1List[1].GetDataForPython(),
                ex1List[2].GetDataForPython(), ex1List[3].GetDataForPython(),
                ex1List[4].GetDataForPython()
            };

            double[][] extraComp2Probs =
            {
                ex2List[0].GetDataForPython(), ex2List[1].GetDataForPython(),
                ex2List[2].GetDataForPython(), ex2List[3].GetDataForPython(),
                ex2List[4].GetDataForPython()
            };

            var nozPo = QraStateContainer.GetValue<FailureMode>("Failure.NozPO");
            var nozFtc = QraStateContainer.GetValue<FailureMode>("Failure.NozFTC");
            var mValveFtc = QraStateContainer.GetValue<FailureMode>("Failure.MValveFTC");
            var sValveFtc = QraStateContainer.GetValue<FailureMode>("Failure.SValveFTC");
            var sValveCcf = QraStateContainer.GetValue<FailureMode>("Failure.SValveCCF");

            var overpFail = QraStateContainer.GetValue<FailureMode>("Failure.Overp");
            var pValveFto = QraStateContainer.GetValue<FailureMode>("Failure.PValveFTO");
            var driveoffFail = QraStateContainer.GetValue<FailureMode>("Failure.Driveoff");
            var couplingFtc = QraStateContainer.GetValue<FailureMode>("Failure.CouplingFTC");

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
            var dataDirLoc = QraStateContainer.UserDataDir;

            QraResult result;

            using (Py.GIL())
            {
                //var lck = PythonEngine.AcquireLock();
                dynamic pyQraLib = Py.Import("hyram");

                try
                {
                    // Execute python analysis. Will return PyObject which is parsed in QRAResult.
                    var resultPyObj = pyQraLib.qra.capi.conduct_analysis(
                        pipeLength, numCompressors, numCylinders, numValves, numInstruments, numJoints, numHoses,
                        numFilters, numFlanges, numExtraComp1, numExtraComp2,
                        facilLength, facilWidth, facilHeight,
                        pipeOuterD, pipeThickness,
                        h2Temp, h2Pres, ambTemp, ambPres, dischargeCoeff,
                        numVehicles, numFuelingPerDay, numVehicleOpDays,
                        immediateIgnitionProbs, delayedIgnitionProbs, ignitionThresholds,
                        detectGasAndFlame, gasDetectCredit,
                        probitThermalModelId, thermalExposureTime,
                        probitOverpModelId, peakOverpressures, overpImpulses, overpFragMass, overpVelocity,
                        overpTotalMass,
                        radSourceModel, notionalNozzleModel,
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
                        dataDirLoc
                    );

                    result = new QraResult(resultPyObj);
                    QraStateContainer.SetValue("ResultsAreStale", false);
                    QraStateContainer.SetValue("Result", result);
                }
                catch (PythonException ex)
                {
                    QraStateContainer.SetValue("ResultsAreStale", true);
                    Trace.TraceError(ex.Message);
                    Debug.WriteLine(ex.Message);
                    throw new InvalidOperationException(
                        "Something went wrong during PyQRA execution. Check log for details.");
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    QraStateContainer.SetValue("ResultsAreStale", true);
                    throw ex;
                }
                finally
                {
                    // Force display of stderr, stdout. This includes print statements from python
                    Console.Out.Flush();
                    Console.Error.Flush();

                    // Unload imports
                    pyQraLib.Dispose();
                    //PythonEngine.ReleaseLock(lck);
                }
            }
        }
    }
}