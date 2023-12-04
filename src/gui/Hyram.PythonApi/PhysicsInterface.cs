/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;


namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    /// Contains functions for interfacing with HyRAM+ python module physics functions.
    /// ETK functions pass "pure" values from forms to python to generate results.
    /// Non-ETK physics functions load parameters from StateContainer.
    /// See python module cs_api.phys and hyram.phys.api for detailed function and parameter descriptions.
    /// </summary>
    public class PhysicsInterface
    {
        private readonly StateContainer _state = State.Data;
        private readonly string pyApiName = "cs_api";
        readonly string outputDirPath = StateContainer.UserDataDir;

        //#if DEBUG
        private readonly bool isVerbose = true;
//#else
        //private readonly bool isVerbose = false;
//#endif

        /// <summary>
        /// Calculates mass flow rate (kg/m3) or time to empty tank.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool ComputeFlowRateOrTimeToEmpty(double orificeDiam, double? temp, double pressure,
            double tankVolume, bool isSteady, double dischargeCoeff, double ambientPressure,
            out string statusMsg, out double? massFlowRate, out double? timeToEmpty, out string plotFileLoc)
        {
            statusMsg = "";
            timeToEmpty = null;
            plotFileLoc = "";
            massFlowRate = null;

            string phaseKey = _state.Phase.GetKey();

            if (!_state.DisplayTemperature()) temp = null;  // clear temp if not gas

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);

                // Activate python logging
                pyApi.phys.setup(outputDirPath, isVerbose);

                PyObject species = _state.GetFuelsPyDict();

                // Generic try/catch with Exception handler due to python.net interfacing, which can yield ambiguous bugs
                bool status;
                try
                {
                    // Execute python analysis. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python call...");
                    dynamic resultPyObj;
                    resultPyObj = pyApi.phys.etk_compute_mass_flow_rate(species, temp, pressure, phaseKey, orificeDiam,
                                                                        isSteady, tankVolume, dischargeCoeff,
                                                                        ambientPressure, outputDirPath);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    // unwrap result status before actual results
                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        dynamic resultDict = (dynamic)resultPyObj["data"];
                        if (isSteady)
                        {
                            massFlowRate = ((double[])resultDict["rates"])[0];
                            Trace.TraceInformation("Mass flow rate results - rate: " + massFlowRate);
                        }
                        else
                        {
                            timeToEmpty = (double)resultDict["time_to_empty"];
                            plotFileLoc = (string)resultDict["plot"];
                            Trace.TraceInformation("Time-to-empty: " + timeToEmpty + ", plot: " + plotFileLoc);
                        }
                        resultPyObj.Dispose();
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during mass flow calculation. Check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }
                return status;
            }
        }

        /// <summary>
        /// Calculates tank parameter from those provided.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool ComputeTankParameter(double? temp, double? pressure, double? volume, double? mass, string phaseKey,
                                        out string statusMsg, out double? output1, out double? output2)
        {
            statusMsg = "";
            output1 = null;
            output2 = null;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);

                PyObject species = _state.GetFuelsPyDict();

                pyApi.phys.setup(outputDirPath, isVerbose);

                bool status;
                try
                {
                    Trace.TraceInformation("Executing python tank mass call...");
                    dynamic resultPyObj = pyApi.phys.etk_compute_tank_mass_param(species, temp, pressure, phaseKey, volume, mass);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];
                    if (status)
                    {
                        dynamic resultData = resultPyObj["data"];
                        output1 = (double?)resultData["param1"];
                        output2 = (double?)resultData["param2"];
                        Trace.TraceInformation("Tank mass parameter results: " + output1 + ", " + output2);
                        resultPyObj.Dispose();
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during tank parameter calculation. Check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }
                return status;
            }
        }

        /// <summary>
        /// Calculates temperature, pressure or density based on given parameters, including phase.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool ComputeTpd(double? temp, double? pressure, double? density, string phaseKey,
                               out string statusMsg, out double? output1, out double? output2)
        {
            statusMsg = "";
            output1 = null;
            output2 = null;

            Trace.TraceInformation("Executing python call");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);
                pyApi.phys.setup(outputDirPath, isVerbose);

                PyObject species = _state.GetFuelsPyDict();

                bool status;
                try
                {
                    // Execute python analysis. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python TPD call...");
                    dynamic resultPyObj = pyApi.phys.etk_compute_thermo_param(species, phaseKey, temp, pressure, density);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];
                    if (status)
                    {
                        dynamic resultData = resultPyObj["data"];
                        output1 = (double?)resultData["param1"];
                        output2 = (double?)resultData["param2"];
                        resultPyObj.Dispose();
                        Trace.TraceInformation("TPD results: " + output1 + ", " + output2);
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during TPD calculation. Check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }

                return status;
            }
        }

        /// <summary>
        /// Calculates TNT equivalence data.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool ComputeTntEquivalence(double vaporMass, double yield, out string statusMsg, out double? tntMass)
        {
            statusMsg = "";
            tntMass = null;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);

                pyApi.phys.setup(outputDirPath, isVerbose);
                PyObject species = _state.GetFuelsPyDict();

                var status = false;
                try
                {
                    Trace.TraceInformation("Executing python TNT mass call...");
                    dynamic resultPyObj = pyApi.phys.etk_compute_equivalent_tnt_mass(vaporMass, yield, species);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    // Verify python func completed without error
                    if (status)
                    {
                        tntMass = (double)resultPyObj["data"];
                        Trace.TraceInformation("TNT mass query result: " + tntMass + ". Processing...");
                        resultPyObj.Dispose();
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during TNT mass calculation. Check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }
                return status;
            }
        }


        /// <summary>
        /// Creates release plume plot.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool CreatePlumePlot(out string statusMsg, out string warningMsg, out string plotFilepath, out double massFlowRate,
            out List<double> orderedContours, out List<double> streamlineDists,
            out List<List<double>> contourDistSets)
        {
            statusMsg = "";
            warningMsg = "";
            plotFilepath = "";
            massFlowRate = float.NaN;
            orderedContours = new List<double>();
            streamlineDists = new List<double>();
            contourDistSets = new List<List<double>>();

            string phaseKey = _state.Phase.GetKey();
            double ambientP = _state.AmbientPressure.GetValue(PressureUnit.Pa);
            double ambientT = _state.AmbientTemperature.GetValue(TempUnit.Kelvin);
            double releaseP = _state.GetFluidPressure(PressureUnit.Pa);
            double releaseT = _state.FluidTemperature.GetValue(TempUnit.Kelvin);
            double releaseAngle = _state.PlumeReleaseAngle.GetValue(AngleUnit.Radians);
            double? inputMassFlow = _state.FluidMassFlow;
            double orificeDiam = _state.OrificeDiameter.GetValue();
            double dischargeCoeff = _state.OrificeDischargeCoefficient.GetValue();
            double[] contours = _state.PlumeContourLevels;
            string nozzleKey = _state.Nozzle.GetKey();
            string plotTitle = _state.PlumePlotTitle;

            double? xmin = _state.PlumeXMin.GetValueMaybeNull();
            double? xmax = _state.PlumeXMax.GetValueMaybeNull();
            double? ymin = _state.PlumeYMin.GetValueMaybeNull();
            double? ymax = _state.PlumeYMax.GetValueMaybeNull();
            double? vmin = _state.PlumeVMin.GetValueMaybeNull();
            double? vmax = _state.PlumeVMax.GetValueMaybeNull();
            if (_state.PlumePlotAutoLimits)
            {
                xmin = null; xmax = null;
                ymin = null; ymax = null;
                vmin = null; vmax = null;
            }

            Trace.TraceInformation($"Output directory: {outputDirPath}, Verbose {isVerbose}");
            Trace.TraceInformation("Acquiring python lock and importing module...");

            //var testDict = new Dictionary<string, string>
            //{
            //    {"test", "123.554454"},
            //    { "two", "-43435.3433"}
            //};

            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);
                PyObject relSpecies = _state.GetFuelsPyDict();

                bool status;
                try
                {
                    pyApi.phys.setup(outputDirPath, isVerbose);

                    Trace.TraceInformation("Executing python plume call...");
                    dynamic resultPyObj;
                    resultPyObj = pyApi.phys.analyze_jet_plume(ambientT, ambientP,
                                                               relSpecies, releaseT, releaseP, phaseKey,
                                                               orificeDiam, inputMassFlow, releaseAngle, dischargeCoeff, nozzleKey,
                                                               contours, xmin, xmax, ymin, ymax, vmin, vmax,
                                                               plotTitle, outputDirPath, isVerbose);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        dynamic resultData = resultPyObj["data"];
                        plotFilepath = (string)resultData["plot"];
                        warningMsg = (string)resultPyObj["warning"];
                        massFlowRate = (float)resultData["mass_flow_rate"];
                        var streamlineDoubles = (double[])resultData["streamline_dists"];
                        if (streamlineDoubles.Length > 0)
                        {
                            streamlineDists = new List<double>(streamlineDoubles);
                            // parse xy contour distance data. Contours in ordered list.
                            orderedContours = new List<double>(
                                                (double[])resultData["ordered_contours"]);
                            // distance values are in sets of 4 (x1, x2, y1, y2)
                            var orderedDistSets = (double[][])resultData["mole_frac_dists"];
                            foreach (var distSet in orderedDistSets)
                            {
                                contourDistSets.Add(new List<double>(distSet));
                            }

                        }

                        resultPyObj.Dispose();
                        Trace.TraceInformation("Plume plot: " + plotFilepath);
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during plume plot generation. Check log for details.";
                    plotFilepath = "";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }

                return status;
            }
        }

        /// <summary>
        /// Evaluates accumulation and generates associated data and plots.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool AnalyzeAccumulation(double[] timesToPlot, double[] ptPressures, double[] ptTimes, double[] presTicks, double maxSimTime, bool isSteady,
                                        out string statusMsg, out string warningMsg,
                                        out double[] pressuresPerTime, out double[] depths, out double[] concentrations, out double[] massFlowRates,
                                        out double overpressure, out double timeOfOverpressure,
                                        out string pressurePlotFilepath, out string massPlotFilepath, out string layerPlotFilepath,
                                        out string trajectoryPlotFilepath, out string massFlowPlotFilepath
                        )
        {
            statusMsg = "";
            warningMsg = "";
            pressuresPerTime = new double[0];
            depths = new double[0];
            concentrations = new double[0];
            massFlowRates = new double[0];
            overpressure = -1;
            timeOfOverpressure = -1;
            pressurePlotFilepath = "";
            massPlotFilepath = "";
            layerPlotFilepath = "";
            trajectoryPlotFilepath = "";
            massFlowPlotFilepath = "";

            bool status;
            string phaseKey = _state.Phase.GetKey();
            double ambientP = _state.AmbientPressure.GetValue(PressureUnit.Pa);
            double ambientT = _state.AmbientTemperature.GetValue(TempUnit.Kelvin);
            double releaseP = _state.GetFluidPressure(PressureUnit.Pa);
            double releaseT = _state.FluidTemperature.GetValue(TempUnit.Kelvin);
            double releaseAngle = _state.ReleaseAngle.GetValue(AngleUnit.Radians);
            double orificeDiam = _state.OrificeDiameter.GetValue();
            double dischargeCoeff = _state.OrificeDischargeCoefficient.GetValue();
            string nozzleKey = _state.Nozzle.GetKey();
            double tankVolume = _state.TankVolume.GetValue(VolumeUnit.CubicMeter);
            double releaseHeight = _state.ReleaseHeight.GetValue();
            double enclosureHeight = _state.EnclosureHeight.GetValue();
            double floorCeilingArea = _state.FloorCeilingArea.GetValue();
            double distReleaseToWall = _state.ReleaseToWallDistance.GetValue();
            double ceilVentXArea = _state.CeilingVentArea.GetValue();
            double ceilVentHeight = _state.CeilingVentHeight.GetValue();
            double floorVentXArea = _state.FloorVentArea.GetValue();
            double floorVentHeight = _state.FloorVentHeight.GetValue();
            double flowRate = _state.VentFlowRate.GetValue(VolumetricFlowUnit.CubicMetersPerSecond);

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);
                PyObject relSpecies = _state.GetFuelsPyDict();

                try
                {
                    pyApi.phys.setup(outputDirPath, isVerbose);

                    Trace.TraceInformation("Executing accumulation analysis...");
                    dynamic resultPyObj;

                    resultPyObj = pyApi.phys.analyze_accumulation(ambientT, ambientP,
                                                                  relSpecies, releaseT, releaseP, phaseKey,
                                                                  tankVolume, orificeDiam, releaseHeight,
                                                                  enclosureHeight, floorCeilingArea,
                                                                  ceilVentXArea, ceilVentHeight,
                                                                  floorVentXArea, floorVentHeight,
                                                                  timesToPlot,
                                                                  dischargeCoeff,
                                                                  flowRate, distReleaseToWall,
                                                                  maxSimTime, releaseAngle, nozzleKey,
                                                                  ptPressures, ptTimes, presTicks, isSteady,
                                                                  outputDirPath, isVerbose);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        dynamic resultData = resultPyObj["data"];
                        pressuresPerTime = (double[])resultData["pressures_per_time"];
                        depths = (double[])resultData["depths"];
                        concentrations = (double[])resultData["concentrations"];
                        massFlowRates = (double[])resultData["mass_flow_rates"];
                        overpressure = (double)resultData["overpressure"];
                        timeOfOverpressure = (double)resultData["time_of_overp"];
                        pressurePlotFilepath = (string)resultData["pres_plot_filepath"];
                        massPlotFilepath = (string)resultData["mass_plot_filepath"];
                        layerPlotFilepath = (string)resultData["layer_plot_filepath"];
                        trajectoryPlotFilepath = (string)resultData["trajectory_plot_filepath"];
                        massFlowPlotFilepath = (string)resultData["mass_flow_plot_filepath"];

                        warningMsg = (string)resultPyObj["warning"];
                        resultPyObj.Dispose();
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    statusMsg = "Error during accumulation analysis. Check log for details.";
                    status = false;
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }
            }
            return status;
        }

        /// <summary>
        /// Creates flame release temperature plot.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool CreateFlameTemperaturePlot(out string statusMsg, out string warningMsg, out string plotFilepath,
                                                out float massFlowRate, out float srad, out float flameLength, out float radiantFrac)
        {
            bool status;
            statusMsg = "";
            warningMsg = "";
            plotFilepath = "";
            massFlowRate = float.NaN;
            srad = float.NaN;
            flameLength = float.NaN;
            radiantFrac = float.NaN;

            double? xmin = _state.FlameXMin.GetValueMaybeNull();
            double? xmax = _state.FlameXMax.GetValueMaybeNull();
            double? ymin = _state.FlameYMin.GetValueMaybeNull();
            double? ymax = _state.FlameYMax.GetValueMaybeNull();
            if (_state.FlameTempAutoLimits)
            {
                xmin = null; xmax = null;
                ymin = null; ymax = null;
            }

            string phaseKey = _state.Phase.GetKey();
            double ambientP = _state.AmbientPressure.GetValue(PressureUnit.Pa);
            double ambientT = _state.AmbientTemperature.GetValue(TempUnit.Kelvin);
            double releaseP = _state.GetFluidPressure(PressureUnit.Pa);
            double releaseT = _state.FluidTemperature.GetValue(TempUnit.Kelvin);
            double releaseAngle = _state.ReleaseAngle.GetValue(AngleUnit.Radians);
            double orificeDiam = _state.OrificeDiameter.GetValue();
            double? inputMassFlow = _state.FluidMassFlow;
            double dischargeCoeff = _state.OrificeDischargeCoefficient.GetValue();
            string nozzleKey = _state.Nozzle.GetKey();
            var relHumid = _state.RelativeHumidity.GetValue();
            double[] contours = _state.TempContourLevels;
            string plotTitle = _state.FlamePlotTitle;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);
                PyObject relSpecies = _state.GetFuelsPyDict();

                try
                {
                    pyApi.phys.setup(outputDirPath, isVerbose);

                    Trace.TraceInformation("Generating flame temp plot...");
                    dynamic resultPyObj = pyApi.phys.jet_flame_analysis(ambientT, ambientP,
                                                                        relSpecies, releaseT, releaseP, phaseKey,
                                                                        orificeDiam, inputMassFlow, dischargeCoeff,
                                                                        releaseAngle, nozzleKey, relHumid,
                                                                        null, plotTitle, contours,
                                                                        xmin, xmax, ymin, ymax,

                                                                        false, null, null, null, null,
                                                                        null, null, null, null, null, null,  // plot limits
                                                                        outputDirPath, isVerbose);

                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        dynamic resultData = (dynamic)resultPyObj["data"];
                        plotFilepath = (string)resultData["temp_plot_filepath"];
                        massFlowRate = (float)resultData["mass_flow_rate"];
                        srad = (float)resultData["srad"];
                        flameLength = (float)resultData["visible_length"];
                        radiantFrac = (float)resultData["radiant_frac"];
                        Debug.WriteLine(" Created plot: " + plotFilepath);
                        warningMsg = (string)resultPyObj["warning"];
                        resultPyObj.Dispose();
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during flame temp plot generation. Check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }
            }
            return status;
        }


        /// <summary>
        /// Evaluates radiative heat flux of release.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool AnalyzeRadiativeHeatFlux(out string statusMsg, out string warningMsg,
                                            out double[] fluxData, out string fluxPlotFilepath, out string tempPlotFilepath,
                                            out float massFlowRate, out float srad, out float flameLength, out float radiantFrac)
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            fluxPlotFilepath = "";
            tempPlotFilepath = "";
            massFlowRate = float.NaN;
            srad = float.NaN;
            flameLength = float.NaN;
            radiantFrac = float.NaN;

            string plotFilepath = "";
            string tempPlotTitle = "";
            double? tempContours = null;

            string phaseKey = _state.Phase.GetKey();
            double ambientP = _state.AmbientPressure.GetValue(PressureUnit.Pa);
            double ambientT = _state.AmbientTemperature.GetValue(TempUnit.Kelvin);
            double releaseP = _state.GetFluidPressure(PressureUnit.Pa);
            double releaseT = _state.FluidTemperature.GetValue(TempUnit.Kelvin);
            double releaseAngle = _state.ReleaseAngle.GetValue(AngleUnit.Radians);
            double orificeDiam = _state.OrificeDiameter.GetValue();
            double? inputMassFlow = _state.FluidMassFlow;
            double dischargeCoeff = _state.OrificeDischargeCoefficient.GetValue();
            string nozzleKey = _state.Nozzle.GetKey();
            var relHumid = _state.RelativeHumidity.GetValue();
            double[] radHeatFluxX = _state.RadiativeFluxX;
            double[] radHeatFluxY = _state.RadiativeFluxY;
            double[] radHeatFluxZ = _state.RadiativeFluxZ;
            double[] fluxContours = _state.FlameContourLevels;

            bool analyzeFlux = true;
            fluxData = new double[radHeatFluxX.Length];

            double? txmin = _state.FlameXMin.GetValueMaybeNull();
            double? txmax = _state.FlameXMax.GetValueMaybeNull();
            double? tymin = _state.FlameYMin.GetValueMaybeNull();
            double? tymax = _state.FlameYMax.GetValueMaybeNull();

            double? fxmin = _state.FluxXMin.GetValueMaybeNull();
            double? fxmax = _state.FluxXMax.GetValueMaybeNull();
            double? fymin = _state.FluxYMin.GetValueMaybeNull();
            double? fymax = _state.FluxYMax.GetValueMaybeNull();
            double? fzmin = _state.FluxZMin.GetValueMaybeNull();
            double? fzmax = _state.FluxZMax.GetValueMaybeNull();

            if (_state.FlameFluxAutoLimits)
            {
                txmin = null; txmax = null;
                tymin = null; tymax = null;
                fxmin = null; fxmax = null;
                fymin = null; fymax = null;
                fzmin = null; fzmax = null;
            }

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);
                PyObject relSpecies = _state.GetFuelsPyDict();

                try
                {
                    pyApi.phys.setup(outputDirPath, isVerbose);
                    Trace.TraceInformation("Radiative heat flux analysis...");
                    dynamic resultPyObj = pyApi.phys.jet_flame_analysis(ambientT, ambientP,
                                                                relSpecies, releaseT, releaseP, phaseKey,
                                                                orificeDiam, inputMassFlow, dischargeCoeff,

                                                                releaseAngle, nozzleKey, relHumid,
                                                                null, tempPlotTitle, tempContours,
                                                                txmin, txmax, tymin, tymax,

                                                                analyzeFlux, radHeatFluxX, radHeatFluxY, radHeatFluxZ, fluxContours,
                                                                fxmin, fxmax, fymin, fymax, fzmin, fzmax,

                                                                outputDirPath, isVerbose);

                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];
                    dynamic resultData = resultPyObj["data"];

                    if (status)
                    {
                        fluxData = (double[])resultData["flux_data"];
                        fluxPlotFilepath = (string)resultData["flux_plot_filepath"];
                        tempPlotFilepath = (string)resultData["temp_plot_filepath"];
                        massFlowRate = (float)resultData["mass_flow_rate"];
                        srad = (float)resultData["srad"];
                        flameLength = (float)resultData["visible_length"];
                        radiantFrac = (float)resultData["radiant_frac"];
                        Debug.WriteLine(" Flux results: " + plotFilepath);
                        Debug.WriteLine(" Flux plots: " + fluxPlotFilepath + ", " + tempPlotFilepath);
                        Debug.WriteLine(" Created plot: " + plotFilepath);
                        warningMsg = (string)resultPyObj["warning"];

                        resultPyObj.Dispose();
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during radiative heat flux analysis. Check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }

                return status;
            }
        }


        // Evaluates overpressure release.
        // See python module hyram.phys.api for input parameter descriptions.
        /// <summary>
        /// Calculates tank parameter from those provided.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool AnalyzeUnconfinedOverpressure(out string statusMsg, out string warningMsg, out double[] overpressures, out double[] impulses,
                                                    out string overpPlot, out string impulsePlot, out float massFlowRate, out float flamOrDetMass)
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            massFlowRate = float.NaN;
            flamOrDetMass = float.NaN;
            overpPlot = "";
            impulsePlot = "";

            var xLocs = _state.OverpressureX;
            var yLocs = _state.OverpressureY;
            var zLocs = _state.OverpressureZ;
            overpressures = new double[xLocs.Length];
            impulses = new double[xLocs.Length];

            string phaseKey = _state.Phase.GetKey();
            double ambientP = _state.AmbientPressure.GetValue(PressureUnit.Pa);
            double ambientT = _state.AmbientTemperature.GetValue(TempUnit.Kelvin);
            double releaseP = _state.GetFluidPressure(PressureUnit.Pa);
            double releaseT = _state.FluidTemperature.GetValue(TempUnit.Kelvin);
            double releaseAngle = _state.ReleaseAngle.GetValue(AngleUnit.Radians);
            double orificeDiam = _state.OrificeDiameter.GetValue();
            double? inputMassFlow = _state.FluidMassFlow;
            double dischargeCoeff = _state.OrificeDischargeCoefficient.GetValue();
            string nozzleKey = _state.Nozzle.GetKey();

            double flameSpeed = _state.OverpressureFlameSpeed;
            double tntFactor = _state.TntEquivalenceFactor.GetValue();
            string overpMethod = _state.OverpressureMethod.GetKey();
            double[] overpContours = _state.OverpressureContours;  // kPa
            double[] impulseContours = _state.ImpulseContours;  // kPa*s

            double? oxmin = _state.OverpXMin.GetValueMaybeNull();
            double? oxmax = _state.OverpXMax.GetValueMaybeNull();
            double? oymin = _state.OverpYMin.GetValueMaybeNull();
            double? oymax = _state.OverpYMax.GetValueMaybeNull();
            double? ozmin = _state.OverpZMin.GetValueMaybeNull();
            double? ozmax = _state.OverpZMax.GetValueMaybeNull();

            double? ixmin = _state.ImpulseXMin.GetValueMaybeNull();
            double? ixmax = _state.ImpulseXMax.GetValueMaybeNull();
            double? iymin = _state.ImpulseYMin.GetValueMaybeNull();
            double? iymax = _state.ImpulseYMax.GetValueMaybeNull();
            double? izmin = _state.ImpulseZMin.GetValueMaybeNull();
            double? izmax = _state.ImpulseZMax.GetValueMaybeNull();

            if (_state.OverpAutoLimits)
            {
                oxmin = null; oxmax = null;
                oymin = null; oymax = null;
                ozmin = null; ozmax = null;
                ixmin = null; ixmax = null;
                iymin = null; iymax = null;
                izmin = null; izmax = null;
            }

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);
                PyObject relSpecies = _state.GetFuelsPyDict();

                try
                {
                    pyApi.phys.setup(outputDirPath, isVerbose);
                    Trace.TraceInformation("Unconfined overpressure analysis...");
                    dynamic resultPyObj = pyApi.phys.unconfined_overpressure_analysis(
                                                        ambientT, ambientP, relSpecies, releaseT, releaseP, phaseKey,
                                                        orificeDiam, inputMassFlow, releaseAngle, dischargeCoeff,
                                                        nozzleKey, overpMethod,
                                                        xLocs, yLocs, zLocs,
                                                        overpContours, oxmin, oxmax, oymin, oymax, ozmin, ozmax,
                                                        impulseContours, ixmin, ixmax, iymin, iymax, izmin, izmax,
                                                        flameSpeed, tntFactor, outputDirPath, isVerbose);

                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];
                    dynamic resultData = resultPyObj["data"];

                    if (status)
                    {
                        overpressures = (double[])resultData["overpressures"];
                        impulses = (double[])resultData["impulses"];
                        overpPlot = (string)resultData["overp_plot_filepath"];
                        impulsePlot = (string)resultData["impulse_plot_filepath"];
                        massFlowRate = (float)resultData["mass_flow_rate"];
                        flamOrDetMass = (float)resultData["flam_or_det_mass"];
                        warningMsg = (string)resultPyObj["warning"];

                        resultPyObj.Dispose();
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }

                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during analysis - check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }

                return status;
            }
        }


        /// <summary>
        /// Creates cryogenic pooling plot.
        /// </summary>
        /// <returns>True if successful</returns>
        public bool CreatePoolingPlot(out string statusMsg, out string warningMsg, out string plotFilepath)
        {
            statusMsg = "";
            warningMsg = "";
            plotFilepath = "";

            string relSpeciesKey = _state.GetActiveFuel().Key;
            double thermC = _state.SurfaceThermCond.GetValue();
            double thermD = _state.SurfaceThermDiff.GetValue();
            double temp = _state.SurfaceTemp.GetValue(TempUnit.Kelvin);
            double massFlow = _state.PoolMassFlow.GetValue(MassFlowUnit.KgPerMin);
            double inflowR = _state.InflowRadius.GetValue(DistanceUnit.Meter);
            double simTime = _state.SimTime.GetValue(TimeUnit.Second);

            double? xmin = _state.CryoXMin.GetValueMaybeNull();
            double? xmax = _state.CryoXMax.GetValueMaybeNull();
            double? ymin = _state.CryoYMin.GetValueMaybeNull();
            double? ymax = _state.CryoYMax.GetValueMaybeNull();

            if (_state.CryoAutoLimits)
            {
                xmin = null; xmax = null;
                ymin = null; ymax = null;
            }

            Trace.TraceInformation($"Output directory: {outputDirPath}, Verbose {isVerbose}");
            Trace.TraceInformation("Acquiring python lock and importing module...");

            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyApi = Py.Import(pyApiName);

                bool status;
                try
                {
                    pyApi.phys.setup(outputDirPath, isVerbose);

                    Trace.TraceInformation("Executing python pooling call...");
                    dynamic resultPyObj;
                    resultPyObj = pyApi.phys.analyze_pooling(relSpeciesKey, thermC, thermD, temp, 
                                                             massFlow, inflowR, simTime,
                                                               outputDirPath, isVerbose);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        dynamic resultData = resultPyObj["data"];
                        plotFilepath = (string)resultData["plot"];
                        warningMsg = (string)resultPyObj["warning"];
                        resultPyObj.Dispose();
                        Trace.TraceInformation("Pooling plot: " + plotFilepath);
                    }
                    else
                    {
                        Trace.TraceError(statusMsg);
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    status = false;
                    statusMsg = "Error during pooling plot generation. Check log for details.";
                    plotFilepath = "";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyApi.Dispose();
                }

                return status;
            }
        }


    }
}
