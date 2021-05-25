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
using System.Diagnostics;


namespace SandiaNationalLaboratories.Hyram
{
    public class PhysicsInterface
    {
        private readonly string libName = "hyram";
        private readonly bool isVerbose = true;

        /// <summary>
        /// Calculate mass flow rate (kg/m3) or time to empty tank.
        /// Currently called by ETK.
        /// </summary>
        /// <param name="temp">K</param>
        /// <param name="pressure">Pa</param>
        /// <param name="tankVolume">m3</param>
        /// <param name="orifDiam">m</param>
        /// <param name="isSteady">bool; whether to compute flow rate or blowdown timing</param>
        /// <param name="dischargeCoeff">coefficient to account for non-plug flow; between 0 and 1</param>
        /// <param name="massFlowRate">Output parameter for flow roate (kg/m3)</param>
        /// 
        /// <param name="timeToEmpty">Output parameter for time-to-empty (s)</param>
        /// <param name="plotFileLoc">Output parameter string for plot location if computing time-to-empty</param>
        public bool ComputeFlowRateOrTimeToEmpty(double orifDiam, double? temp, double pressure,
            double tankVolume, bool isSteady, double dischargeCoeff,
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
                        isSteady, tankVolume, dischargeCoeff, outputDirPath);
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
                            massFlowRate = (double)resultDict["mass_flow_rate"];
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

        /// <summary>
        /// Calculate tank mass (kg)
        /// Currently called by ETK.
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="pressure"></param>
        /// <param name="phase">Fluid phase</param>
        /// <param name="tankVolume"></param>
        /// <returns></returns>
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
            if (!FluidPhase.DisplayTemperature()) temp = null;  // clear temp if not gas

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

        /// <summary>
        /// Calculate temp, pressure or density based on which parameters are provided.
        /// Currently called by ETK.
        /// </summary>
        /// <param name="temp">K</param>
        /// <param name="pressure">Pa</param>
        /// <param name="density">kg/m3</param>
        /// <returns></returns>
        public bool ComputeTpd(double? temp, double? pressure, double? density,
            out string statusMsg, out double? tpdResult)
        {
            bool status = false;
            statusMsg = "";
            tpdResult = null;

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;

            string species = StateContainer.GetValue<FuelType>("FuelType").GetKey();

            double? result;

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
                    dynamic resultPyObj = pyLib.phys.c_api.etk_compute_thermo_param(species, temp, pressure, density);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    // unwrap results
                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    // Verify python func completed without error
                    if (status)
                    {
                        result = (double)resultPyObj["data"];
                        resultPyObj.Dispose();
                        Trace.TraceInformation("TPD result: " + result);
                        tpdResult = result;
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
                    tpdResult = null;
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

        /// <summary>
        /// Get TNT equivalence data
        /// Currently called by ETK.
        /// </summary>
        public bool ComputeTntEquivalence(double vaporMass, double yield, double heatOfCombustion,
            out string statusMsg, out double? tntMass)
        {
            bool status = false;
            statusMsg = "";
            tntMass = null;

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;

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
                    dynamic resultPyObj = pyLib.phys.c_api.etk_compute_equivalent_tnt_mass(
                        vaporMass, yield, heatOfCombustion);
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


        /// <summary>
        /// Create plume plot via python
        /// </summary>
        /// <param name="ambPres"></param>
        /// <param name="ambTemp"></param>
        /// <param name="relPres"></param>
        /// <param name="relTemp"></param>
        /// <param name="orifDiam"></param>
        /// <param name="dischargeCoeff"></param>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <param name="contours"></param>
        /// <param name="jetAngle"></param>
        /// <param name="plotTitle"></param>
        /// <returns></returns>
        public bool CreatePlumePlot(
            double ambPres, double ambTemp,
            double relPres, double? relTemp, double relAngle,
            double orifDiam, double dischargeCoeff,
            double xMin, double xMax, double yMin, double yMax,
            double contours, string nozzleModel, string plotTitle,
            out string statusMsg, out string warningMsg,
            out string plotFilepath
            )
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            plotFilepath = "";

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

        /// <summary>
        /// Compute results of overpressure and generate associated plots.
        /// </summary>
        /// <param name="ambPres"></param>
        /// <param name="ambTemp"></param>
        /// <param name="relPres"></param>
        /// <param name="relTemp"></param>
        /// <param name="orifDiam"></param>
        /// <param name="orifDisCoeff"></param>
        /// <param name="tankVolume"></param>
        /// <param name="relDisCoeff"></param>
        /// <param name="relArea"></param>
        /// <param name="relHeight"></param>
        /// <param name="enclosureHeight"></param>
        /// <param name="floorCeilingArea"></param>
        /// <param name="distReleaseToWall"></param>
        /// <param name="ceilVentXArea"></param>
        /// <param name="ceilVentHeight"></param>
        /// <param name="floorVentXArea"></param>
        /// <param name="floorVentHeight"></param>
        /// <param name="flowRate"></param>
        /// <param name="relAngle"></param>
        /// <param name="timesToPlot"></param>
        /// <param name="ptPressures"></param>
        /// <param name="ptTimes"></param>
        /// <param name="presTicks"></param>
        /// <param name="maxSimTime"></param>
        /// <param name="pressuresPerTime"></param>
        /// <param name="depths"></param>
        /// <param name="concentrations"></param>
        /// <param name="overpressure"></param>
        /// <param name="timeOfOverpressure"></param>
        /// <param name="pressurePlotFilepath"></param>
        /// <param name="massPlotFilepath"></param>
        /// <param name="layerPlotFilepath"></param>
        /// <param name="trajectoryPlotFilepath"></param>
        /// <returns></returns>
        public bool ExecuteOverpressureAnalysis(
                        double ambPres, double ambTemp, double relPres, double? relTemp, double orifDiam, double orifDisCoeff, double tankVolume,
                        double relDisCoeff, double relArea, double relHeight, double enclosureHeight, double floorCeilingArea, double distReleaseToWall,
                        double ceilVentXArea, double ceilVentHeight, double floorVentXArea, double floorVentHeight, double flowRate, double relAngle, string nozzleModelKey,
                        double[] timesToPlot, double[] ptPressures, double[] ptTimes, double[] presTicks, double maxSimTime,
                        out string statusMsg, out string warningMsg,
                        out double[] pressuresPerTime, out double[] depths, out double[] concentrations, out double overpressure, out double timeOfOverpressure,
                        out string pressurePlotFilepath, out string massPlotFilepath, out string layerPlotFilepath, out string trajectoryPlotFilepath
                        )
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            pressuresPerTime = new double[0];
            depths = new double[0];
            concentrations = new double[0];
            overpressure = -1;
            timeOfOverpressure = -1;
            pressurePlotFilepath = "";
            massPlotFilepath = "";
            layerPlotFilepath = "";
            trajectoryPlotFilepath = "";

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
                    Trace.TraceInformation("Executing overpressure call...");
                    dynamic resultPyObj;

                    resultPyObj = pyLib.phys.c_api.overpressure_indoor_release(
                        ambTemp, ambPres,
                        relSpecies, relTemp, relPres, phaseKey,
                        tankVolume, orifDiam, relHeight,
                        enclosureHeight, floorCeilingArea,
                        ceilVentXArea, ceilVentHeight,
                        floorVentXArea, floorVentHeight,
                        timesToPlot,
                        orifDisCoeff, relDisCoeff,
                        flowRate, distReleaseToWall,
                        maxSimTime, relArea, relAngle, nozzleModelKey,
                        ptPressures, ptTimes, presTicks,
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
                        overpressure = (double)resultData["overpressure"];
                        timeOfOverpressure = (double)resultData["time_of_overp"];
                        pressurePlotFilepath = (string)resultData["pres_plot_filepath"];
                        massPlotFilepath = (string)resultData["mass_plot_filepath"];
                        layerPlotFilepath = (string)resultData["layer_plot_filepath"];
                        trajectoryPlotFilepath = (string)resultData["trajectory_plot_filepath"];

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
                    statusMsg = "Error during overpressure calculation. Check log for details.";
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

        public bool CreateFlameTemperaturePlot(
            double ambTemp, double ambPres, double? relTemp, double relPres,
            double orifDiam, double relHeight, double relAngle, string nozzleModelKey,
            out string statusMsg, out string warningMsg, out string plotFilepath)
        {
            bool status = false;
            statusMsg = "";
            warningMsg = "";
            plotFilepath = "";

            // Derive path to data dir for temp and data files, e.g. pickling
            string outputDirPath = StateContainer.UserDataDir;

            string relSpecies = StateContainer.GetValue<FuelType>("FuelType").GetKey();
            string phaseKey = StateContainer.Instance.GetFluidPhase().GetKey();
            if (!FluidPhase.DisplayTemperature()) relTemp = null;  // clear temp if not gas

            // unused params required by python
            var radSourceModel =
                StateContainer.GetValue<RadiativeSourceModels>("RadiativeSourceModel").ToString();
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
                        orifDiam,
                        relAngle, relHeight,
                        nozzleModelKey, radSourceModel, relHumid,
                        null, null, null, null, analyzeFlux,
                        outputDirPath, isVerbose);

                    Trace.TraceInformation("Python call complete. Processing results...");

                    status = (bool)resultPyObj["status"];
                    statusMsg = (string)resultPyObj["message"];

                    if (status)
                    {
                        dynamic resultData = (dynamic)resultPyObj["data"];
                        plotFilepath = (string)resultData["temp_plot_filepath"];
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


        public bool AnalyzeRadiativeHeatFlux(
            double ambTemp, double ambPres, double? relTemp, double relPres, double orifDiam, double relHeight, double relAngle,
            NozzleModel notionalNozzleModel, double[] radHeatFluxX, double[] radHeatFluxY, double[] radHeatFluxZ, double relativeHumidity,
            RadiativeSourceModels radiativeSourceModel, double[] contourLevels,
            out string statusMsg, out string warningMsg,
            out double[] fluxData, out string fluxPlotFilepath, out string tempPlotFilepath)
        {
            bool status = false;
            fluxData = new double[radHeatFluxX.Length];
            statusMsg = "";
            warningMsg = "";
            fluxPlotFilepath = "";
            tempPlotFilepath = "";

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
                        orifDiam,
                        relAngle, relHeight,
                        notionalNozzleModel.GetKey(), radiativeSourceModel.ToString().ToLower(),
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

    }
}
