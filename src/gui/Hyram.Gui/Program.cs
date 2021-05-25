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
            Directory.CreateDirectory(StateContainer.UserDataDir);

            System.IO.DirectoryInfo userDataDirInfo = new DirectoryInfo(StateContainer.UserDataDir);
            foreach (FileInfo file in userDataDirInfo.EnumerateFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception e)
                {
                    //Console.WriteLine(e);
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
