# Contributing to HyRAM+
This document describes the Hydrogen Plus Other Alternative Fuesl Risk Assessment Models ("HyRAM+") application development.
The application comprises a frontend GUI written in C# and a backend module written in Python.
Step-by-step instructions are included for setting up a C# development environment using MS Visual Studio 2017 ("MSVS").
Similar setup instructions are provided for backend python development.

# Changelog
For any significant changes made to the source code, it is expected that the change will be summarized in the [CHANGELOG](./CHANGELOG.md) document. Guidance and suggestions for how to best enter these changes in the changelog are [here](https://keepachangelog.com/en/1.0.0/). New changes should be added to the `[Unreleased]` section at the top of the file; these will be removed to a release section during the next public release.


&nbsp;
# Table of Contents
[A. Repository Layout](#repo-layout)<br>

[B. C# GUI Development](#c-gui)<br>
&nbsp;&nbsp;[B.1 C# Project Descriptions](#c-projects)<br>
&nbsp;&nbsp;[B.2 Setting Up Python Interpreter](#c-setup-py)<br>
&nbsp;&nbsp;[B.3 Setting Up Solution in MSVS](#c-setup-msvs)<br>
&nbsp;&nbsp;[B.4 Configuring the Hyram.Setup Projects](#c-setup-installer)<br>
&nbsp;&nbsp;[B.5 Misc. Notes](#c-notes)<br>

[C. Python HyRAM+ Module Development](#py-dev)<br>
&nbsp;&nbsp;[C.1 Module Layout](#py-layout)<br>

[D. Python HyRAM+ Usage](#py-usage)<br>
&nbsp;&nbsp;[D.1 As C# Backend](#py-usage-c)<br>
&nbsp;&nbsp;[D.2 As standalone Python module](#py-usage-py)<br>



<a name="repo-layout">&nbsp;</a>
# A. Repository Layout
Development of HyRAM+ includes both the C# frontend GUI and the backend Python module.
Source code is organized as follows:

```
$
├───build
└───src
    ├───gui
    │   ├───Hyram.gui
    │   ├───Hyram.PythonApi
    │   ├───Hyram.PythonDirectory
    │   │   └───python interpreter
    │   ├───Hyram.State
    │   ├───Hyram.Units
    │   ├───Hyram.Utilities
    │   ├───Hyram.Setup
    │   └───Hyram.SetupBootstrapper
    ├───cs_api
    └───hyram
        ├───tests
        └───hyram
            ├───phys
            ├───qra
            └───utilities
```

* `src` - Project source code under version control, including C# GUI and python package.
* `src/gui` - Front-end C# user interface, including MSVS Solution and C# projects.
    * The python interpreter is installed in `gui/Hyram.PythonDirectory` and should *not* be added to version control.
* `src/hyram` - Python module of HyRAM+ tools including physics, quantitative risk assessment, and miscellaneous utilities.
* `src/cs_api` - python functions providing C# access to HyRAM+ python code via the python.NET library.
* `build` - should be located alongside the source directory to contain C# build files excluded from version control.


<a name="c-gui">&nbsp;</a>
# B. C# GUI Development
This section describes how to set up a C# development environment for the HyRAM+ graphical user interface.
The following steps are written for MS Visual Studio 2017 and an x64 solution configuration.


&nbsp;
<a name="c-projects">&nbsp;</a>
## B.1 C# Project Descriptions

**`Hyram.Gui`**<br>
Content panels, forms, and logic for user-interaction.
Also includes the engineering toolkit.

**`Hyram.PythonApi`**<br>
Interface classes for requesting and consuming python function calls via Python.NET.
Includes separate classes for physics and QRA.

**`Hyram.PythonDirectory`**<br>
Dummy project containing the Python interpreter.
Conditional build events copy the python interpreter, modules, and python.net runtime during build.

**`Hyram.State`**<br>
Class handling internal state and storage of parameters during UI interaction.

**`Hyram.Units`**<br>
Classes, functions, and tests for handling units and their conversions for display.

**`Hyram.Utilities`**<br>
Misc. utility classes and functions.

**`Hyram.Setup`**<br>
WiX project to configure the HyRAM+ MSI installer.

**`Hyram.SetupBootstrapper`**<br>
WiX bootstrapper to configure the HyRAM+ .exe bundle installer, including the program MSI created by Hyram.Setup.



<a name="c-setup-py">&nbsp;</a>
## B.2 Setting Up Python Interpreter
Development requires an embedded Python 3.9 installation which is ignored by git and must be set up manually.

#### 1. Install Python 3.9
The HyRAM+ C# build process requires the Python 3.9 directory to be located at:

    repo/src/gui/Hyram.PythonDirectory/python/

To install Python, navigate to the [Python webpage](https://www.python.org/downloads/windows/)
and download the Python 3.9 x86-64 executable installer ([direct link](https://www.python.org/ftp/python/3.9.2/python-3.9.2-amd64.exe)).
Follow the installation instructions:

 * *Optional Features*: enable `pip` and `tcl/tk` options only. Disable all other options.
 * *Advanced Options*: check `precompile standard library` only. Uncheck all other options.
 * Set the custom install location to `<location_of_repo>\src\gui\Hyram.PythonDirectory\python\`
 * After installation completes, verify that the `python` directory exists.

#### 2. Verify pip
Open a command-line prompt and navigate to the `python` directory.
Verify that the python pip tool is accessible by executing the following command:

    python -m pip --version

This should print the location of the pip command.
The location should be within the `python` directory; for example:

    C:repositories/hyram/src/gui/Hyram.PythonDirectory/python/lib/site-packages/pip

If a different location is displayed, such as within `program files`, ensure that you correctly navigated to the `python` directory and that `python.exe` is present.

#### 3. Install Required Python Modules
The python backend requires the following modules:

```
scipy==1.6.1
numpy==1.24.1
coolprop==6.4.1

matplotlib==3.6.3
Pillow==9.4.0
python-dateutil==2.8.2
cycler==0.11.0
kiwisolver==1.3.1
pyparsing==3.0.9
six==1.16.0

// for GUI
pythonnet==2.5.2
pycparser==2.20
```

These modules can be installed with pip.
Note that CoolProp may fail to install with pip. If this occurs, it should be installed as a wheel:

```
cd <path/to/python/dir>
./python.exe -m pip install Cython
git clone https://github.com/CoolProp/CoolProp --recursive coolprop
cd coolprop
git checkout tags/v6.4.1
cd wrappers/Python
../../../python.exe setup.py install

cd <path/to/python/dir>
./python.exe -m pip uninstall Cython
```

After the wheel installs, remove the cloned coolprop repository from the python directory.

Finally, verify the installed modules with `python.exe -m pip freeze`


<a name="c-setup-msvs">&nbsp;</a>
## B.3 Setting Up Solution in MSVS
Before loading the solution, install the [Wix Installer Toolset v3.11.2](https://wixtoolset.org/releases/)

Open the HyRAM+ solution in MSVS by selecting the following file:

    /src/gui/Hyram.sln

This will load the solution and its various projects.
The GUI requires the MathNet.Numerics and NewtonSoft.Json packages, which should be installed via the NuGet manager (`Tools -> NuGet Package Manager`).


### 1. Solution Settings
Verify the following configuration settings and properties after loading the solution in MSVS.

**Solution Properties**
* Common Properties -> Startup Project -> Simple Startup project -> Hyram.Gui
* Configuration Properties: all projects except the Setup projects should be set to build.

**Hyram.Gui project settings**
* Application -> Startup object should be `Hyram.Gui.Program`
* Under Build, set `Platform target` to `x64`.

**Hyram.PythonApi project settings**
* Under Build, set `Platform target` to `x64`.

**Output paths**:
* Set project output paths (project settings -> Build) to `..\..\..\build\bin\<Configuration>\` (Release or Debug).


### 2. Build Events
Build events are located in the `Hyram.PythonDirectory` project (`properties -> Build Events`).
Pre- and Post-build events provide the following functionality:
   * ensure the python interpreter, Python.Net runtime, and HyRAM+ module are copied to the compiled application.
   * delete any test image files and .pyc files
   * delete development-specific directories (test, .idea) in the build dir

The python interpreter build event only needs to be executed during the first build or when the python interpreter
or required packages are changed.
The events can be disabled by modifying the conditional statement to ensure it is false; for example:

    if "$(ConfigurationName)" == "AlwaysFalse"

To enable the event, make sure the conditional matches the Configuration (DEBUG or RELEASE):

    if "$(ConfigurationName)" == "DEBUG"


### 3. Python.NET Runtime
The Python.NET runtime DLL must be added as a reference for any projects which rely on it (e.g. Hyram.PythonApi).
View the project references to verify that Python.NET is listed.

To add it to a new project, in the Solution Explorer right-click the project name and select `Add -> Reference`.
Under the `Browse` tab select the `Python.Runtime.DLL` at:

    src/gui/Hyram.PythonDirectory/python/Python.Runtime.DLL


<a name="c-setup-installer">&nbsp;</a>
## B.4 Configuring the Hyram.Setup projects
This section describes the steps required to build a debug or release-ready HyRAM+ installer package bundle.
It also describes the current setup configuration.
This process uses the WiX installer tools to bundle the python interpreter and files alongside the GUI components into a single installer.
Two C# projects comprise the installer setup:
* Hyram.Setup - builds the HyRAM+ application installer
* Hyram.SetupBootstrapper - builds the final installation bundle which will be delivered to users. This can later be augmented with additional installers as needed (e.g. the .NET runtime installer).

Note: installed files require a unique GUID. In some cases WiX can create this automatically.
When it cannot be auto-generated, a custom GUID can be generated in MSVS via the Tools -> Create GUID... feature.

### Hyram.Setup Project
The Hyram.Setup project builds the HyRAM+ installer which installs the GUI, python interpreter,
and python HyRAM+ module.

The application setup is described in the **`Product.wxs`** XML configuration file.
It defines the product properties, included C# projects and files,
and the integrated python interpreter.
Note that WiX uses a specific directory layout and file configuration method.
The WiX website has various guides for understanding this approach.

The integrated python interpreter and python module are configured in the **`Hyram.Setup.wixproj`** file.
Due to the large number of files in the python interpreter, the WiX Heat tool (included in the WiX MSVS toolset)
is used to automatically gather and include these files during project build.
The `wixproj` file should only be modified by first unloading the Hyram.Setup project,
then opening the `Hyram.Setup.wixproj` file in a separate file editor,
then reloading the project once editing is complete.

Note: The **`PythonInstallFiles.wxs`** and **`PythonLibsFiles.wxs`** files are automatically generated by Heat
and should not be manually modified.


**Important Configuration Properties:**
- ProductCode - specified by the Product ID.
- UpgradeCode - A6256A13-D1FE-4D2E-9BB8-DEE9FF314047
- Version: *Set as needed*

The UpgradeCode describes an application with multiple versions.
The ProductCode describes a release of one of those versions.
The HyRAM+ UpgradeCode should never change unless a different application is desired,
such as another fuel analysis program that should be installed separately from HyRAM+.
The ProductCode should be changed with each new release.

Windows uses the installation context (i.e. "all users" or "just me" options),
UpgradeCode and ProductCode when determining if a previous version of an application resides on a user's system.


### Hyram.SetupBootstrapper Project
The `Hyram.SetupBootstrapper` project builds the final installation bundle.
It can include multiple application installers (e.g. HyRAM+ and a .NET runtime installer).
It also describes the installation GUI used by the user when installing HyRAM+.

- **`Bundle.wxs`** - configuration file specifying the included package(s) and installer GUI theme files.
- **`theme.xml`** - configures the GUI of the installer.


### 7. Building the Installer
Building a HyRAM+ installer is now a straightforward process.
The setup projects no longer need to be recreated when the python interpreter files changes.
To build a new installer, simply build the Hyram.Setup and Hyram.SetupBootstrapper projects.
Make sure the platform and configuration properties are set correctly for each included project.

The resulting Hyram.Setup.exe file can be tested internally and distributed as needed.

Note that the SetupBootstrapper currently outputs a .exe file bundle.
The .msi file created by the Hyram.Setup project is included in this bundle.
The .msi file should not be distributed.



<a name="c-notes">&nbsp;</a>
## B.5 Miscellaneous Notes

#### Issue: form fails to display in MSVS designer
**Winforms Forms containing custom controls cannot be displayed by the MSVS GUI designer when the project is built in x64.**

This is a known issue with the designer, which can only handle 32-bit controls. If a form fails to load due to a custom control, rebuild the `Hyram.Gui` and `Hyram.Units` projects in `Any CPU` configuration mode.
MSVS may also need to be restarted.
Be sure to reset the configurations to `x64` after editing with the designer.

When adding custom controls to a form, ensure that the custom control is not set as a public property of the form.
Automated tools, such as various ReSharper tools, may also erroneously apply this property.
When this occurs, ensure that the control is set as a private property in the designer.cs file.

**Winforms forms deriving from a custom form class fail to display in the GUI designer.**

Similar to above, this is a known issue that apparently has not been fixed as of 2022.
To display these forms in the designer, you must set the platform target of the solution and of *every* project to x86. Clean and rebuild. You may need to restart MSVS as well.

This may cause compilation to fail due to a python.NET incompatibility.
If this occurs, edit the form in the designer as needed and then restore the platform target settings.
In some cases you can set the target to Any CPU after doing the above, and the designer will still load until the next clean.


<a name="py-dev">&nbsp;</a>
# C. Python HyRAM+ Module Development

This section describes the layout, usage, and front-end integration of the Python HyRAM+ module. For more information about the installation, use, and developement of the Python HyRAM+ module specifically, see the [README](./src/hyram/README.md) file in the `src/hyram` directory.

<a name="py-layout">&nbsp;</a>
## C.1 Module Layout
HyRAM+ primarily consists of two sub-modules: physics and qra.

    ├───hyram
    │   ├───phys
    │   ├───qra
    │   └───utilities

C# access to these modules is provided by the sibling `cs_api` directory and code.
The physics sub-module includes a `api.py` file for interacting with the main function calls programmatically.
The quantitative risk analysis algorithm can be found in `qra/analysis.py`.



<a name="py-usage">&nbsp;</a>
# D. Python HyRAM+ Usage
HyRAM+ can be utilized as a C# backend with Python.NET, or independently via Python as a standalone module.
C# calls via python.NET utilize the `phys` and `qra` wrapper code found in the `cs_api` directory.



<a name="py-usage-c">&nbsp;</a>
## D.1 HyRAM+ as C# Backend
The Python HyRAM+ package is integrated into the C# HyRAM+ application via a Python.NET interface.
C# caller functions in the `Hyram.PythonApi` project use the `cs_api` wrapper code to conduct HyRAM+ analyses.
The cs_api files load custom Python.NET modules which are only available while the Python.runtime.dll is loaded.
These functions are not currently compatible with a normal Python session.

**Adding a new API function call to the C# GUI**

Add a new function to the PhysicsInterface or QraInterface files in the `Hyram.PythonApi` project.
The function should gather the appropriate inputs in a non-nullable format and activate the Python.NET lock space
before calling the desired python function in the `qra.py` or `phys.py` wrapper files.
Refer to existing functions for reference.

**Adding a new API python function call to the python HyRAM+ module**

A corresponding python function must be created in the `cs_api/qra.py` or `cs_api/phys.py` files.
This function can be called by C# functions as needed.

The function should parse and convert incoming data; arrays can be handled with available utility functions.
Note that functions called by C# should not have keyword or optional arguments.
After parsing incoming data, the function can then execute HyRAM+ python code and then return a wrapped object to C#.
Refer to existing functions for reference.


<a name="py-usage-py">&nbsp;</a>
## D.2 Using HyRAM+ as a Python module
QRA analysis can be executed via command-line through the `qra/analysis.py` file.
Physics analyses can be conducted via the `phys/api.py` file.
See the analysis function docstring for details and guidance.

