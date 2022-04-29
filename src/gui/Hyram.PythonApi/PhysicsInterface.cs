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
    public class PhysicsInterface
    {
        private readonly string libName = "hyram";
//#if DEBUG
        private readonly bool isVerbose = true;
//#else
        //private readonly bool isVerbose = false;
//#endif

        // Calculates mass flow rate (kg/m3) or time to empty tank.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool ComputeFlowRateOrTimeToEmpty(double orifDiam, double? temp, double pressure,
            double tankVolume, bool isSteady, double dischargeCoeff, double ambientPressure,
            out string statusMsg, out double? massFlowRate, out double? timeToEmpty, out string plotFileLoc)
        {
            // Default return vals
            bool status = false;
            statusMsg = "";
            timeToEmpty = null;
            plotFileLoc = "";
            massFlowRate = null;

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;

            string species = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) temp = null;  // clear temp if not gas

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                // Activate python logging
                pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                // Generic try/catch with Exception handler due to python.net interfacing, which can yield ambiguous bugs
                try
                {
                    // Execute python analysis. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python call...");
                    dynamic resultPyObj;
                    resultPyObj = pyLib.phys.c_api.etk_compute_mass_flow_rate(
                        species, temp, pressure, phaseKey, orifDiam,
                        isSteady, tankVolume, dischargeCoeff, ambientPressure, outputDirPath);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    // unwrap result status before actual results
                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    // Verify python func completed without error
                    if (status)
                    {
                        dynamic resultDict = (dynamic)resultPyObj["data"];
                        // Extract correct results based on analysis type
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
                    pyLib.Dispose();
                }
                return status;
            }
        }

        // Calculates tank mass (kg)
        // See python module hyram.phys.api for input parameter descriptions.
        public bool ComputeTankMass(double? temp, double pressure, double tankVolume,
            out string statusMsg, out double? tankMass)
        {
            bool status = false;
            statusMsg = "";
            tankMass = null;

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;

            string species = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature())
            {
                temp = null;  // clear temp if not gas
            }

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                // Activate python logging
                pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                try
                {
                    Trace.TraceInformation("Executing python tank mass call...");
                    dynamic resultPyObj = pyLib.phys.c_api.etk_compute_tank_mass(
                        species, temp, pressure, phaseKey, tankVolume);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    // unwrap results
                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    // Verify python func completed without error
                    if (status)
                    {
                        tankMass = (double)resultPyObj["data"];
                        Trace.TraceInformation("Tank mass query result: " + tankMass + " kg. Processing...");
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
                    statusMsg = "Error during tank mass calculation. Check log for details.";
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyLib.Dispose();
                }
                return status;
            }
        }

        // Calculates temperature, pressure or density based on given parameters, including phase.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool ComputeTpd(double? temp, double? pressure, double? density, string phaseKey,
            out string statusMsg, out double? output1, out double? output2)
        {
            bool status = false;
            statusMsg = "";
            output1 = null;
            output2 = null;

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;
            string species = StateContainer.GetValue<FuelType>("FuelType").GetKey();

            Trace.TraceInformation("Executing python call");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                // Activate python logging
                pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                try
                {
                    // Execute python analysis. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python TPD call...");
                    dynamic resultPyObj = pyLib.phys.c_api.etk_compute_thermo_param(species, phaseKey, temp, pressure, density);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    // unwrap results
                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    // Verify python func completed without error
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
                    pyLib.Dispose();
                }

                return status;
            }
        }

        // Calculates TNT equivalence data.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool ComputeTntEquivalence(double vaporMass, double yield, out string statusMsg, out double? tntMass)
        {
            bool status = false;
            statusMsg = "";
            tntMass = null;

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;
            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                // Activate python logging
                pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                try
                {
                    Trace.TraceInformation("Executing python TNT mass call...");
                    dynamic resultPyObj = pyLib.phys.c_api.etk_compute_equivalent_tnt_mass(vaporMass, yield, relSpecies);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    // unwrap results
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
                    pyLib.Dispose();
                }
                return status;
            }
        }


        // Creates release plume plot.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool CreatePlumePlot(
            double ambPres, double ambTemp,
            double relPres, double? relTemp, double relAngle,
            double orifDiam, double dischargeCoeff,
            double xMin, double xMax, double yMin, double yMax,
            double contours, string nozzleModel, string plotTitle,
            out string statusMsg, out string warningMsg,
            out string plotFilepath, out float massFlowRate
            )
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            plotFilepath = "";
            massFlowRate = float.NaN;

            //bool ambSpecies = StateContainer.GetValue<bool>("FuelType");
            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) relTemp = null;  // clear temp if not gas

            string outputDirPath = StateContainer.UserDataDir;

            Trace.TraceInformation($"Output directory: {outputDirPath}, Verbose {isVerbose}");
            Trace.TraceInformation("Acquiring python lock and importing module...");

            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                try
                {
                    // Activate python logging
                    pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                    // Execute python function call. Will return PyObject containing wrapped results.
                    Trace.TraceInformation("Executing python plume call...");
                    dynamic resultPyObj;
                    resultPyObj = pyLib.phys.c_api.analyze_jet_plume(
                        ambTemp, ambPres,
                        relSpecies, relTemp, relPres, phaseKey,
                        orifDiam, relAngle, dischargeCoeff, nozzleModel,
                        contours, xMin, xMax, yMin, yMax,
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
                    pyLib.Dispose();
                }

                return status;
            }
        }

        // Evaluates accumulation and generates associated data and plots.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool AnalyzeAccumulation(
                        double ambPres, double ambTemp, double relPres, double? relTemp, double orifDiam, double orifDisCoeff, double tankVolume,
                        double relHeight, double enclosureHeight, double floorCeilingArea, double distReleaseToWall,
                        double ceilVentXArea, double ceilVentHeight, double floorVentXArea, double floorVentHeight, double flowRate, double relAngle, string nozzleModelKey,
                        double[] timesToPlot, double[] ptPressures, double[] ptTimes, double[] presTicks, double maxSimTime, bool isSteady,
                        out string statusMsg, out string warningMsg,
                        out double[] pressuresPerTime, out double[] depths, out double[] concentrations, out double[] massFlowRates, out double overpressure, out double timeOfOverpressure,
                        out string pressurePlotFilepath, out string massPlotFilepath, out string layerPlotFilepath, out string trajectoryPlotFilepath, out string massFlowPlotFilepath
                        )
        {
            bool status = false;
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

            // times must be converted to correct values in seconds, depending on selected time unit

            string outputDirPath = StateContainer.UserDataDir;

            //bool ambSpecies = StateContainer.GetValue<bool>("FuelType");
            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) relTemp = null;  // clear temp if not gas

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                try
                {
                    // Activate python logging
                    pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                    // Execute python function call. Will return PyObject containing wrapped results.
                    Trace.TraceInformation("Executing accumulation analysis...");
                    dynamic resultPyObj;

                    resultPyObj = pyLib.phys.c_api.analyze_accumulation(
                        ambTemp, ambPres,
                        relSpecies, relTemp, relPres, phaseKey,
                        tankVolume, orifDiam, relHeight,
                        enclosureHeight, floorCeilingArea,
                        ceilVentXArea, ceilVentHeight,
                        floorVentXArea, floorVentHeight,
                        timesToPlot,
                        orifDisCoeff,
                        flowRate, distReleaseToWall,
                        maxSimTime, relAngle, nozzleModelKey,
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
                    pyLib.Dispose();
                }
            }
            return status;
        }

        // Creates flame release temperature plot.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool CreateFlameTemperaturePlot(
            double ambTemp, double ambPres, double? relTemp, double relPres,
            double orifDiam, double dischargeCoeff, double relAngle, string nozzleModelKey,
            out string statusMsg, out string warningMsg, out string plotFilepath,
            out float massFlowRate, out float srad, out float flameLength)
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            plotFilepath = "";
            massFlowRate = float.NaN;
            srad = float.NaN;
            flameLength = float.NaN;

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;

            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) relTemp = null;  // clear temp if not gas

            var relHumid = StateContainer.GetNdValue("relativeHumidity");
            bool analyzeFlux = false;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                try
                {
                    // Activate python logging
                    pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                    // Execute python function call. Will return PyObject containing wrapped results.
                    Trace.TraceInformation("Generating flame temp plot...");
                    dynamic resultPyObj = pyLib.phys.c_api.jet_flame_analysis(
                        ambTemp, ambPres,
                        relSpecies, relTemp, relPres, phaseKey,
                        orifDiam, dischargeCoeff,
                        relAngle,
                        nozzleModelKey, relHumid,
                        null, null, null, null, analyzeFlux,
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
                    pyLib.Dispose();
                }
            }
            return status;
        }


        // Evaluates radiative heat flux of release.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool AnalyzeRadiativeHeatFlux(
            double ambTemp, double ambPres, double? relTemp, double relPres, double orifDiam, double dischargeCoeff,
            double relAngle,
            NozzleModel notionalNozzleModel, double[] radHeatFluxX, double[] radHeatFluxY, double[] radHeatFluxZ,
            double relativeHumidity, double[] contourLevels,
            out string statusMsg, out string warningMsg,
            out double[] fluxData, out string fluxPlotFilepath, out string tempPlotFilepath,
            out float massFlowRate, out float srad, out float flameLength)
        {
            bool status = false;
            fluxData = new double[radHeatFluxX.Length];
            statusMsg = "";
            warningMsg = "";
            fluxPlotFilepath = "";
            tempPlotFilepath = "";
            massFlowRate = float.NaN;
            srad = float.NaN;
            flameLength = float.NaN;

            string outputDirPath = StateContainer.UserDataDir;
            string plotFilepath = "";

            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) relTemp = null;  // clear temp if not gas
            bool analyzeFlux = true;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                try
                {
                    // Activate python logging
                    pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                    // Execute python function call. Will return PyObject containing wrapped results.
                    Trace.TraceInformation("Radiative heat flux analysis...");

                    dynamic resultPyObj = pyLib.phys.c_api.jet_flame_analysis(
                        ambTemp, ambPres,
                        relSpecies, relTemp, relPres, phaseKey,
                        orifDiam, dischargeCoeff,
                        relAngle,
                        notionalNozzleModel.GetKey(),
                        relativeHumidity,
                        radHeatFluxX, radHeatFluxY, radHeatFluxZ,
                        contourLevels,
                        analyzeFlux,
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
                    pyLib.Dispose();
                }

                return status;
            }
        }


        // Evaluates overpressure release.
        // See python module hyram.phys.api for input parameter descriptions.
        public bool AnalyzeUnconfinedOverpressure(
            double ambTemp, double ambPres,
            double? relTemp, double relPres, double orifDiam, double relAngle, double dischargeCoeff,
            NozzleModel notionalNozzleModel, UnconfinedOverpressureMethod method,
            double[] xLocs, double[] yLocs, double[] zLocs, double[] contours,
            double flameSpeed, double tntFactor,
            out string statusMsg, out string warningMsg,
            out double[] overpressures, out double[] impulses, out string plotFilepath, out float massFlowRate)
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            overpressures = new double[xLocs.Length];
            impulses = new double[xLocs.Length];
            massFlowRate = float.NaN;
            plotFilepath = "";

            string outputDirPath = StateContainer.UserDataDir;

            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature())
            {
                relTemp = null; // clear temp if not gas
            }

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyLib = Py.Import(libName);

                try
                {
                    // Activate python logging
                    pyLib.phys.c_api.setup(outputDirPath, isVerbose);

                    // Execute python function call. Will return PyObject containing wrapped results.
                    Trace.TraceInformation("Unconfined overpressure analysis...");

                    dynamic resultPyObj = pyLib.phys.c_api.unconfined_overpressure_analysis(
                        ambTemp, ambPres,
                        relSpecies, relTemp, relPres, phaseKey,
                        orifDiam, relAngle, dischargeCoeff,
                        notionalNozzleModel.GetKey(), method.GetKey(),
                        xLocs, yLocs, zLocs, contours,
                        flameSpeed, tntFactor,
                        outputDirPath, isVerbose);

                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];
                    dynamic resultData = resultPyObj["data"];

                    if (status)
                    {
                        overpressures = (double[])resultData["overpressure"];
                        impulses = (double[])resultData["impulse"];
                        plotFilepath = (string)resultData["figure_file_path"];
                        massFlowRate = (float)resultData["mass_flow_rate"];
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
                    pyLib.Dispose();
                }

                return status;
            }
        }

    }
}
