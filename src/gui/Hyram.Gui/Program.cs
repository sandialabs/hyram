/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/


using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
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
            using (Mutex mutex = new Mutex(false, appGuid))
            {
                if (!mutex.WaitOne(0, false))
                {
                    MessageBox.Show("HyRAM is already running");
                    return;
                }
                Directory.CreateDirectory(StateContainer.UserDataDir);

                System.IO.DirectoryInfo userDataDirInfo = new DirectoryInfo(StateContainer.UserDataDir);
                foreach (FileInfo file in userDataDirInfo.EnumerateFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch (Exception)
                    {
                        ;
                    }
                }

                AppDomain currentDomain = default(AppDomain);
                currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += GlobalUnhandledExceptionHandler;
                Application.ThreadException += GlobalThreadExceptionHandler;

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
                GC.KeepAlive(mutex);
            }
        }

        private static string appGuid = "A6256A13-D1FE-4D2E-9BB8-DEE9FF314047";

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
