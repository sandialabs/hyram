/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using Python.Runtime;

namespace SandiaNationalLaboratories.Hyram
{
    public static class PythonApiConstructor
    {
        private static IntPtr lck;

        public static void Setup()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string envPythonHome = exeDir + @"python";
            // Custom location of our python libs inside app dir
            string pythonLibs = exeDir + @"pylibs";

            string dirDLLs = envPythonHome + @"\DLLs";
            string dirLib = envPythonHome + @"\Lib";
            string dirSitePackages = dirLib + @"\site-packages";
            // TODO: remove this once coolprop has reliable 3.9 pypi package
            string coolpropDir = dirSitePackages + @"\CoolProp-6.4.1-py3.9-win-amd64.egg";

            var path = $"{envPythonHome};{dirSitePackages};{dirLib};{dirDLLs};{coolpropDir}";
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);

            // Ensure Python.Runtime is in PythonHome
            var pythonHome = $"{envPythonHome};{dirLib};";
            Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome, EnvironmentVariableTarget.Process);

            var pythonPath = $"{envPythonHome};{dirSitePackages};{dirLib};{dirDLLs};{pythonLibs};{coolpropDir}";
            Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath, EnvironmentVariableTarget.Process);

            // Override matplotlib backend to use renderer only. Avoid using TK to avoid threading issue.
            Environment.SetEnvironmentVariable("MPLBACKEND", "agg");

            // Cianan: Must initialize once instead of during each use to avoid re-import issues
            PythonEngine.Initialize();

            // Release GIL from main thread so other threads (i.e. analysis threads) can acquire it
            lck = PythonEngine.BeginAllowThreads();
        }
    }
}
