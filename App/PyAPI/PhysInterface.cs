using System;
using QRAState;
using Python.Runtime;
using System.Diagnostics;


namespace PyAPI
{
    public class PhysInterface
    {
        /// <summary>
        /// Create plume plot via python
        /// </summary>
        /// <param name="ambientPressure"></param>
        /// <param name="ambientTemp"></param>
        /// <param name="h2Pressure"></param>
        /// <param name="h2Temp"></param>
        /// <param name="orificeDiam"></param>
        /// <param name="dischargeCoeff"></param>
        /// <param name="xMin"></param>
        /// <param name="xMax"></param>
        /// <param name="yMin"></param>
        /// <param name="yMax"></param>
        /// <param name="contours"></param>
        /// <param name="jetAngle"></param>
        /// <param name="plotTitle"></param>
        /// <returns></returns>
        public string CreatePlumePlot(
            double ambientPressure, double ambientTemp, double h2Pressure, double h2Temp, double orificeDiam,
            double dischargeCoeff, double xMin, double xMax, double yMin, double yMax, double contours, double jetAngle, string plotTitle)
        {
            bool isDebug = QraStateContainer.GetValue<bool>("debug");

            string dataDirLoc = QraStateContainer.UserDataDir;
            string plotFilepath;

            Trace.TraceInformation("Acquiring python lock and importing module...");

            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyHyramLib = Py.Import("hyram");

                try
                {
                    // Execute python function call. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python plume call...");
                    dynamic resultPyObj = pyHyramLib.phys.capi.create_plume_plot(
                        ambientPressure, ambientTemp, h2Pressure, h2Temp, orificeDiam, dischargeCoeff, xMin, xMax, yMin, yMax, contours,
                        jetAngle, plotTitle, dataDirLoc, isDebug
                        );
                    plotFilepath = (string) resultPyObj;
                    resultPyObj.Dispose();
                    Trace.TraceInformation("Python call complete. Created plot: " + plotFilepath);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw new InvalidOperationException("Error during plume plot generation. Check log for details.");
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyHyramLib.Dispose();
                }
            }
            return plotFilepath;
        }

        /// <summary>
        /// Compute results of overpressure and generate associated plots.
        /// </summary>
        /// <param name="ambPressure"></param>
        /// <param name="ambTemp"></param>
        /// <param name="h2Pressure"></param>
        /// <param name="h2Temp"></param>
        /// <param name="orificeDiam"></param>
        /// <param name="orificeDischargeCoeff"></param>
        /// <param name="tankVolume"></param>
        /// <param name="releaseDischargeCoeff"></param>
        /// <param name="releaseArea"></param>
        /// <param name="releaseHeight"></param>
        /// <param name="enclosureHeight"></param>
        /// <param name="floorCeilingArea"></param>
        /// <param name="distReleaseToWall"></param>
        /// <param name="ceilVentXArea"></param>
        /// <param name="ceilVentHeight"></param>
        /// <param name="floorVentXArea"></param>
        /// <param name="floorVentHeight"></param>
        /// <param name="flowRate"></param>
        /// <param name="releaseAngle"></param>
        /// <param name="timesToPlot"></param>
        /// <param name="dotMarkPressures"></param>
        /// <param name="dotMarkTimes"></param>
        /// <param name="limitLinePressures"></param>
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
        public int ExecuteOverpressureAnalysis(
                        double ambPressure, double ambTemp, double h2Pressure, double h2Temp, double orificeDiam, double orificeDischargeCoeff, double tankVolume,
                        double releaseDischargeCoeff, double releaseArea, double releaseHeight, double enclosureHeight, double floorCeilingArea, double distReleaseToWall,
                        double ceilVentXArea, double ceilVentHeight, double floorVentXArea, double floorVentHeight, double flowRate, double releaseAngle, double[] timesToPlot,
                        double[] dotMarkPressures, double[] dotMarkTimes, double[] limitLinePressures, double maxSimTime,
                        out double[] pressuresPerTime, out double[] depths, out double[] concentrations, out double overpressure, out double timeOfOverpressure,
                        out string pressurePlotFilepath, out string massPlotFilepath, out string layerPlotFilepath, out string trajectoryPlotFilepath
                        )
        {
            bool isDebug = QraStateContainer.GetValue<bool>("debug");
            string dataDirLoc = QraStateContainer.UserDataDir;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyHyramLib = Py.Import("hyram");

                try
                {
                    Trace.TraceInformation("Executing python overpressure call...");
                    // Execute python function call. Will return PyObject containing results.
                    dynamic resultPyObj = pyHyramLib.phys.capi.overpressure_indoor_release(
                        ambPressure, ambTemp, h2Pressure, h2Temp, tankVolume, orificeDiam, orificeDischargeCoeff,
                        releaseDischargeCoeff, releaseArea, releaseHeight, enclosureHeight, floorCeilingArea, distReleaseToWall,
                        ceilVentXArea, ceilVentHeight, floorVentXArea, floorVentHeight, flowRate, releaseAngle, timesToPlot,
                        dotMarkPressures, dotMarkTimes, limitLinePressures, maxSimTime, dataDirLoc, isDebug
                        );
                    Trace.TraceInformation("Python call complete. Processing results...");

                    pressuresPerTime = (double[]) resultPyObj["pressures_per_time"];
                    depths = (double[]) resultPyObj["depths"];
                    concentrations = (double[]) resultPyObj["concentrations"];
                    overpressure = (double) resultPyObj["overpressure"];
                    timeOfOverpressure = (double) resultPyObj["time_of_overp"];
                    pressurePlotFilepath = (string) resultPyObj["pres_plot_filepath"];
                    massPlotFilepath = (string) resultPyObj["mass_plot_filepath"];
                    layerPlotFilepath = (string) resultPyObj["layer_plot_filepath"];
                    trajectoryPlotFilepath = (string) resultPyObj["trajectory_plot_filepath"];
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw new InvalidOperationException("Error during overpressure calculation. Check log for details.");
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyHyramLib.Dispose();
                }
            }
            return 1;
        }

        public string CreateFlameTemperaturePlot(
            double ambTemp, double ambPressure, double h2Temp, double h2Pressure,
            double orificeDiam, double y0, double releaseAngle, string nozzleModelKey)
        {
            bool isDebug = QraStateContainer.GetValue<bool>("debug");

            // Derive path to data dir for temp and data files, e.g. pickling
            string dataDirLoc = QraStateContainer.UserDataDir;
            string plotFilepath = "";

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyHyramLib = Py.Import("hyram");

                try
                {
                    // Execute python function call. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python flame temp plot generation...");
                    dynamic resultPyObj = pyHyramLib.phys.capi.create_flame_temp_plot(
                        ambPressure, ambTemp, h2Pressure, h2Temp, orificeDiam, y0, releaseAngle, nozzleModelKey,
                        dataDirLoc, isDebug
                    );
                    Trace.TraceInformation("Python call complete. Processing results...");

                    plotFilepath = (string) resultPyObj;
                    Debug.WriteLine(" Created plot: " + plotFilepath);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw new InvalidOperationException(
                        "Error during flame temp plot generation. Check log for details.");
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyHyramLib.Dispose();
                }
            }
            return plotFilepath;
        }


        public void AnalyzeRadiativeHeatFlux(
            double ambTemp, double ambPressure, double h2Temp, double h2Pressure, double orificeDiam, double leakHeight, double releaseAngle,
                NozzleModel notionalNozzleModel, double[] radHeatFluxX, double[] radHeatFluxY, double[] radHeatFluxZ, double relativeHumidity,
                RadiativeSourceModels radiativeSourceModel,
                double[] contourLevels, out double[] fluxData, out string fluxPlotFilepath, out string tempPlotFilepath)
        {
            bool isDebug = QraStateContainer.GetValue<bool>("debug");
            string dataDirLoc = QraStateContainer.UserDataDir;
            string plotFilepath = "";

            Trace.TraceInformation("Acquiring python lock and importing module");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyHyramLib = Py.Import("hyram");

                try
                {
                    Trace.TraceInformation("Executing python radiative heat flux analysis...");
                    dynamic resultPyObj = pyHyramLib.phys.capi.analyze_radiative_heat_flux(
                        ambTemp, ambPressure, h2Temp, h2Pressure, orificeDiam, leakHeight, releaseAngle,
                        notionalNozzleModel.GetKey(), radHeatFluxX, radHeatFluxY, radHeatFluxZ, relativeHumidity,
                        radiativeSourceModel.ToString().ToLower(),
                        contourLevels, dataDirLoc, isDebug
                        );
                    Trace.TraceInformation("Python call complete. Processing results...");

                    fluxData = (double[]) resultPyObj["flux_data"];
                    fluxPlotFilepath = (string) resultPyObj["flux_plot_filepath"];
                    tempPlotFilepath = (string) resultPyObj["temp_plot_filepath"];
                    Debug.WriteLine(" Flux results: " + plotFilepath);
                    Debug.WriteLine(" Flux plots: " + fluxPlotFilepath + ", " + tempPlotFilepath);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw new InvalidOperationException(
                        "Error during rad heat flux analysis. Check log for details.");
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyHyramLib.Dispose();
                }
            }
        }

        /// <summary>
        /// Calculate mass flow rate (kg/m3) or time to empty tank.
        /// Currently called by ETK.
        /// </summary>
        /// <param name="temp">K</param>
        /// <param name="pressure">Pa</param>
        /// <param name="tankVolume">m3</param>
        /// <param name="orificeDiam">m</param>
        /// <param name="isSteady">bool; whether to compute flow rate or blowdown timing</param>
        /// <param name="dischargeCoeff">coefficient to account for non-plug flow; between 0 and 1</param>
        /// <param name="massFlowRate">Output parameter for flow roate (kg/m3)</param>
        /// 
        /// <param name="timeToEmpty">Output parameter for time-to-empty (s)</param>
        /// <param name="plotFileLoc">Output parameter string for plot location if computing time-to-empty</param>
        public void ComputeFlowRateOrTimeToEmpty(double temp, double pressure, double tankVolume, double orificeDiam, bool isSteady, double dischargeCoeff,
            out double? massFlowRate, out double? timeToEmpty, out string plotFileLoc)
        {
            // Derive path to data dir for temp and data files, e.g. pickling
            string dataDirLoc = QraStateContainer.UserDataDir;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyHyramLib = Py.Import("hyram");

                try
                {
                    // Execute python analysis. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python FRoTTE call...");
                    dynamic resultPyObj = pyHyramLib.phys.capi.compute_mass_flow_rate(
                        temp, pressure, tankVolume, orificeDiam, isSteady, dischargeCoeff, dataDirLoc);
                    Trace.TraceInformation("Python call complete. Processing results...");

                    if (isSteady)
                    {
                        massFlowRate = (double)resultPyObj["mass_flow_rate"];
                        timeToEmpty = null;
                        plotFileLoc = "";
                    } else
                    {
                        massFlowRate = null;
                        timeToEmpty = (double)resultPyObj["time_to_empty"];
                        plotFileLoc = (string)resultPyObj["plot"];
                    }
                    Trace.TraceInformation("Mass flow rate results - rate: " + massFlowRate + ", time: " + timeToEmpty);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw new System.InvalidOperationException("Error during mass flow calculation. Check log for details.");
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyHyramLib.Dispose();
                }
            }
        }

        /// <summary>
        /// Calculate tank mass (kg)
        /// Currently called by ETK.
        /// </summary>
        /// <param name="temp"></param>
        /// <param name="pressure"></param>
        /// <param name="tankVolume"></param>
        /// <returns></returns>
        public double? ComputeTankMass(double temp, double pressure, double tankVolume)
        {
            // Derive path to data dir for temp and data files, e.g. pickling
            string dataDirLoc = QraStateContainer.UserDataDir;
            double? tankMass;

            Trace.TraceInformation("Acquiring python lock and importing module...");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyHyramLib = Py.Import("hyram");

                try
                {
                    Trace.TraceInformation("Executing python tank mass call...");
                    dynamic resultPyObj = pyHyramLib.phys.capi.compute_tank_mass(temp, pressure, tankVolume, dataDirLoc);
                    tankMass = (double) resultPyObj;
                    Trace.TraceInformation("Tank mass query result: " + tankMass + " kg. Processing...");
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw new InvalidOperationException("Error during tank mass calculation. Check log for details.");
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyHyramLib.Dispose();
                }
            }
            return tankMass;
        }

        /// <summary>
        /// Calculate temp, pressure or density based on which parameters are provided.
        /// Currently called by ETK.
        /// </summary>
        /// <param name="temp">K</param>
        /// <param name="pressure">Pa</param>
        /// <param name="density">kg/m3</param>
        /// <returns></returns>
        public double? ComputeTpd(double? temp, double? pressure, double? density)
        {
            // Derive path to data dir for temp and data files, e.g. pickling
            string dataDirLoc = QraStateContainer.UserDataDir;
            double? result;

            Trace.TraceInformation("Executing python call");
            using (Py.GIL())
            {
                dynamic pyGC = Py.Import("gc");
                dynamic pyHyramLib = Py.Import("hyram");
                try
                {
                    // Execute python analysis. Will return PyObject containing results.
                    Trace.TraceInformation("Executing python TPD call...");
                    dynamic resultPyObj = pyHyramLib.phys.capi.access_thermo_calculations(temp, pressure, density, dataDirLoc);
                    Trace.TraceInformation("Python call complete. Processing results...");
                    result = (double) resultPyObj;
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                    throw new InvalidOperationException("Error during TPD calculation. Check log for details.");
                }
                finally
                {
                    pyGC.InvokeMethod("collect");
                    pyGC.Dispose();
                    pyHyramLib.Dispose();
                }
            }
            return result;
        }

    }
}
