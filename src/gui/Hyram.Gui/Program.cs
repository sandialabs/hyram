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
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{

    static class Program
    {
        //static string logFilename = "gui-" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
        private static readonly string logFilename = "log_gui.txt";
        private static readonly string logFilepath = Path.Combine(StateContainer.UserDataDir, logFilename);

        //public static Serilog.Core.Logger log;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Ensure AppData dirs exist
            Directory.CreateDirectory(StateContainer.AppDataDir);
            Directory.CreateDirectory(StateContainer.UserDataDir);

            AppDomain currentDomain = default(AppDomain);
            currentDomain = AppDomain.CurrentDomain;
            // Handler for unhandled exceptions.
            currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
            // Handler for exceptions in threads behind forms.
            Application.ThreadException += GlobalThreadExceptionHandler;

#if false
            log = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.File(logFilepath, rollingInterval: RollingInterval.Day).CreateLogger();
            log.Information("");
            log.Information("STARTING HyRAM...");
#endif

            Trace.AutoFlush = true;
            TextWriterTraceListener textLog = new TextWriterTraceListener(logFilepath);
            Trace.Listeners.Add(textLog);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm.TheSplashscreen.Show();

            // Load python, env variables for dir locations, and initialize PythonEngine
            //log.Information("Setting up python...");
            Trace.TraceInformation("Setting up python...");

            PythonApiConstructor.Setup();
            //log.Information("Python loaded. Loading GUI.");
            Trace.TraceInformation("Python loaded. Loading GUI.");

            Application.Run(new MainForm());
        }

        private static void GlobalUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = (Exception)e.ExceptionObject;
            //log.Error(ex.Message + "\n" + ex.StackTrace);
            Trace.TraceInformation(ex.Message + "\n" + ex.StackTrace);
        }

        private static void GlobalThreadExceptionHandler(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            Exception ex = default(Exception);
            ex = e.Exception;
            //log.Error(ex.Message + "\n" + ex.StackTrace);
            Trace.TraceInformation(ex.Message + "\n" + ex.StackTrace);
        }
    }
}
