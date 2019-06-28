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

using QRAState;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using PyAPI;

namespace QRA_Frontend
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Ensure AppData dirs exist
            Directory.CreateDirectory(QraStateContainer.AppDataDir);
            Directory.CreateDirectory(QraStateContainer.UserDataDir);

            // Load python, env variables for dir locations, and initialize PythonEngine
            PyGlobals.Setup();

            string logFilename = "GUI_" + DateTime.Now.ToFileTime() + ".txt";
            string logFileLoc = Path.Combine(QraStateContainer.UserDataDir, logFilename);

            Trace.AutoFlush = true;
            TextWriterTraceListener textLog = new TextWriterTraceListener(logFileLoc);
            Trace.Listeners.Add(textLog);
            Trace.TraceInformation("Starting HyRAM...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FrmQreMain.TheSplashscreen.Show();
            Application.Run(new FrmQreMain());
        }
    }
}
