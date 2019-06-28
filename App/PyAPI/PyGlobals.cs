using System;
using Python.Runtime;

namespace PyAPI
{
    public static class PyGlobals
    {
        private static IntPtr lck;

        public static void Setup()
        {
            string exeDir = AppDomain.CurrentDomain.BaseDirectory;
            string envPythonHome = exeDir + @"python";
            // Custom location of our python libs inside app dir
            string hyramPyLibs = exeDir + @"pylibs";

            string dirDLLs = envPythonHome + @"\DLLs";
            string dirLib = envPythonHome + @"\Lib";
            string dirSitePackages = dirLib + @"\site-packages";

            var path = $"{envPythonHome};{dirLib};{dirDLLs};{dirSitePackages};";
            Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);

            // Ensure Python.Runtime is in PythonHome
            var pythonHome = $"{envPythonHome};{dirLib};";
            Environment.SetEnvironmentVariable("PYTHONHOME", pythonHome, EnvironmentVariableTarget.Process);

            var pythonPath = $"{envPythonHome};{dirLib};{dirDLLs};{dirSitePackages};{hyramPyLibs};";
            Environment.SetEnvironmentVariable("PYTHONPATH", pythonPath, EnvironmentVariableTarget.Process);

            // Override matplotlib backend to use renderer only. Avoid using TK to avoid threading issue.
            Environment.SetEnvironmentVariable("MPLBACKEND", "agg");

            // Cianan: Must initialize once instead of during each use to avoid re-import issues
            PythonEngine.Initialize();

            // Release GIL from main thread so other threads (i.e. analysis threads) can acquire it
            lck = PythonEngine.BeginAllowThreads();
        }

#if false
        public static void Teardown()
        {
            PythonEngine.EndAllowThreads(lck);
            PythonEngine.Shutdown();
            GC.Collect();
        }
#endif
    }
}
