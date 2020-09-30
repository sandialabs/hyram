# Contributing to the Hydrogen Risk Assessment Models
This document describes the Hydrogen Risk Assessment Models ("HyRAM") application development.
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
&nbsp;&nbsp;[B.4 Setting Up Installer Project](#c-setup-installer)<br>
&nbsp;&nbsp;[B.5 Misc. Notes](#c-notes)<br>
            
[C. Python HyRAM Module Development](#py-dev)<br>
&nbsp;&nbsp;[C.1 Module Layout](#py-layout)<br>
        
[D. Python HyRAM Usage](#py-usage)<br>
&nbsp;&nbsp;[D.1 As C# Backend](#py-usage-c)<br>
&nbsp;&nbsp;[D.2 As standalone Python module](#py-usage-py)<br>

[E. References](#refs)<br>
    

<a name="repo-layout">&nbsp;</a>
# A. Repository Layout
Development of HyRAM includes both the C# frontend GUI and the backend Python module.
Application code is organized in the git repository in the following way:

```
$
├───build
└───src
    ├───gui (C#)
    │   ├───Hyram.gui
    │   ├───Hyram.PythonApi
    │   ├───Hyram.PythonDirectory
    │   │   └───python
    │   ├───Hyram.State
    │   ├───Hyram.Units
    │   ├───Hyram.Utilities
    │   └───Hyram.Setup
    └───hyram
        ├───tests
        └───hyram
            ├───phys
            ├───qra
            └───utilities

```
* `build` - contains C# build files and other transitory contents excluded from version control.
* `src` - Project source code, including C# GUI and python module(s).
* `src/gui` - Front-end C# interface providing convenient access to HyRAM tools. The python interpreter should be installed in `src/gui/Hyram.PythonDirectory` and should *not* be added to version control.
* `src/hyram` - Python module of HyRAM tools including physics, quantitative risk assessment, and miscellaneous utilities.


<a name="c-gui">&nbsp;</a>
# B. C# GUI Development
This section describes how to set up a development environment for the HyRAM graphical user interface.
The following steps are written for MS Visual Studio 2017 and an x64 solution configuration.
(**Note**: the MSVS graphical form editor may require a project platform of x86 when displaying forms with custom controls.)


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
Dummy project containing the Python interpreter. Custom build events copy the python interpreter, modules, and python.net runtime during build.

**`Hyram.State`**<br>
Database class handling state and storage of parameters.

**`Hyram.Units`**<br>
Custom classes and functions for handling units and their conversions for display.

**`Hyram.Utilities`**<br>
Misc. utility classes and functions.

**`Hyram.Setup`**<br>
Builds the user-facing program installer.



<a name="c-setup-py">&nbsp;</a>
## B.2 Setting Up Python Interpreter
Development requires an embedded Python 3.6 installation which is excluded from the repository.
It must be installed manually according to the following steps.

#### 1. Install Python 3.6
The HyRAM C# build process requires the Python 3.6 directory to be located at:

    repo/src/gui/Hyram.PythonDirectory/python/

To install Python, navigate to the [Python 3.6 webpage](https://www.python.org/downloads/release/python-368/)
and download the Python 3.6 x86-64 executable installer ([direct link](https://www.python.org/ftp/python/3.6.8/python-3.6.8-amd64.exe)).
Follow the installation instructions:

 * *Optional Features*: enable `pip` and `tcl/tk` options only. Disable all other options.
 * *Advanced Options*: check `precompile standard library` only. Uncheck all other options.
 * Set the custom install location to `<location_of_repo>\src\gui\Hyram.PythonDirectory\python\`
 * After installation completes, verify that the `python` directory exists.
 * Navigate to the `tcl` directory in `python/tcl` and copy the `tck8.6` and `tk8.6` directories to `python/Lib/`


<a name="">&nbsp;</a>
#### 2. Install Required Python Modules
The python backend requires the following modules:

```
cloudpickle==0.8.1
CoolProp
cycler==0.10.0
Cython==0.29.7
decorator==4.4.0
dill==0.2.9
imageio==2.5.0
kiwisolver==1.1.0
matplotlib==2.2.4
networkx==2.3
numpy==1.15.4+mkl
pandas==0.24.2
Pillow==6.0.0
pyparsing==2.4.0
pyreadline==2.1
python-dateutil==2.8.0
pythonnet==2.3.0
pytz==2019.1
PyWavelets==1.0.3
scikit-image==0.15.0
scipy==1.2.1
six==1.12.0
toolz==0.9.0
xlrd
```

Many of these modules are dependencies of numpy, scipy, pandas and matplotlib,
which should be installed first using Windows-specific .whl files found on the [Gohlke webpage](https://www.lfd.uci.edu/~gohlke/pythonlibs).
First download each .whl file pertaining to Python 3.6, x64
(the names will have suffixes similar to `cp36-win_amd64.whl`):

* Numpy 1.15 MKL: [numpy‑1.15.4+mkl‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#numpy)
* scipy: [scipy‑1.2.1‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#scipy)
* pandas: [pandas‑0.24.2‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#pandas)
* matplotlib 3.0.0rc2: [matplotlib‑2.2.4‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#matplotlib)

Copy the downloaded .whl files from your download directory to the python directory.

#### 3. Verify pip
Open a command-line prompt and navigate to the `python` directory. 
Verify that the python pip tool is accessible by executing the following command:

    python -m pip --version

This should print the location of the pip command.
The location should be within the `python` directory; for example:

    C:repositories/hyram/src/Hyram.PythonDirectory/python/lib/site-packages/pip

If a different location is displayed, such as within `program files`, ensure that you correctly navigated to the `python` directory and that `python.exe` is present.

#### 4. Install core modules

Install each module via the pip command:

    python -m pip install <module_name>

For example:

    python -m pip install "numpy‑1.15.4+mkl‑cp36‑cp36m‑win_amd64.whl"

**Warning**: if the command exits with an error of "not a supported wheel",
you will need to re-download the correct module .whl file.
This error can also occur if the .whl has been downloaded multiple times and has an appended `(1)` number on its name.

#### 5. Install remaining modules
Remaining packages should be installed via pip from the command-line (in python dir):

    python -m pip install <module_name>
    
For convenience, use the following to batch install the modules:

    python -m pip install Pillow pythonnet pywavelets scikit-image dill pyreadline toolz Cython cloudpickle xlrd CoolProp

#### 6. Verify modules
Verify that all the modules have been installed:

    python -m pip freeze

#### 7. Delete .whl files
The four core module .whl files (numpy, scipy, pandas, matplotlib) are not needed after installation and should be deleted.
They should **not** be added to version control.



<a name="c-setup-msvs">&nbsp;</a>
## B.3 Setting Up Solution in MSVS
Before loading the solution, open Visual Studio and install the official **Microsoft Visual Studio Installer Projects** extension:

    Tools -> Extensions and Updates -> Online -> [Search]

After installing the extension and restarting MSVS, open the HyRAM solution by selecting the following file:

    /src/gui/Hyram.sln 
   
This will load the solution and its various projects. 
The GUI requires the MathNet.Numerics and NewtonSoft.Json packages, which should be installed via the NuGet manager (`Tools -> NuGet Package Manager`).


&nbsp;
### 1. Solution Settings
Verify the following configuration settings and properties after loading the solution in MSVS.

**Solution Properties**
* Common Properties -> Startup Project -> Simple Startup project -> Hyram.Gui
* Configuration Properties: all projects except `Hyram.Setup` should be set to build.

**Hyram.Gui project settings**
* Application -> Startup object should be `Hyram.Gui.Program`
* Under Build, set `Platform target` to `x64`.

**Hyram.PythonApi project settings**
* Under Build, set `Platform target` to `x64`.

**Output paths**:
* Set project output paths (project settings -> Build) to `..\..\..\build\bin\<Configuration>\` (Release or Debug).


### 2. Build Events
Post-build events are used by the `Hyram.PythonDirectory` project (`properties -> Build Events`)
to ensure the python interpreter, Python.Net runtime, and HyRAM module are available to the compiled application.

These events only need to be executed during the first build and when the python code is changed.
The events can be disabled by modifying the conditional statement to ensure it is false; for example:

    if "$(ConfigurationName)" == "AlwaysFalse"

To enable the event, make sure the conditional matches the Configuration (DEBUG or RELEASE):

    if "$(ConfigurationName)" == "DEBUG"


### 3. Python.NET Runtime
The Python.NET runtime DLL must be added as a reference for any projects which rely on it (Hyram.PythonApi).
View the project references to verify that Python.NET is listed.

To add it to a new project, in the Solution Explorer right-click the project name and select `Add -> Reference`.
Under the `Browse` tab select the `Python.Runtime.DLL` at:

    src/gui/Hyram.PythonDirectory/python/Python.Runtime.DLL


<a name="c-setup-installer">&nbsp;</a>
## B.4 Installer Project Setup
This section describes the steps required to build the HyRAM installer project, `Hyram.Setup`, which can be used to install HyRAM on end-user machines.
This process involves creating the project in MSVS, incorporating the python code into it, and building the final installer.

This process requires the free *Visual Studio Installer Projects* (VSIP) tool for MSVS.
The 2017 version can be found [here](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2017InstallerProjects).

**Note**: due to current VSIP limitations, the installer project must be remade from scratch whenever the python interpreter directory or its contents changes substantially.
This is because custom directories (i.e. the python directory) cannot be removed from VSIP if they are not empty, so directory changes cannot be accommodated in an existing Project.


### 1. Add the Installer Project
In MSVS, do the following:
- Select `Add -> New Project`.
- Select `Installed -> Other Project Types -> Visual Studio Installer` to view the available Installer project options.
- Select the `Setup Wizard` 

In the Setup Wizard form, enter the following parameters:
* Name: Hyram.Setup
* Location: `repo/src/gui/`
* Solution: Add to solution

The Setup Wizard dialog will open. Navigate the steps with the following inputs:
* Step 2/5: select *Create setup for a Windows Application* 
* Step 3/5: select the Primary Output of each project and the Content Files from `Hyram.Gui`.
* Click Finish (skip step 4)


&nbsp;
### 2. Set Installer Properties
Open the Installer Project Properties (`right-click -> Properties`) and set the following:
* Output filename: `<Configuration>\HyRAM.Setup.msi` (Configuration should be either Release or Debug)

Open the Deployment Project Properties (Highlight Installer in Solutions Explorer and push F4) and set the following:
- Author - Sandia National Laboratories
- DetectNewerInstalledVersion - True
- InstallAllUsers - False
- Manufacturer - Sandia National Laboratories
- ProductCode - *generate a new one for each new release*
- Product Name - HyRAM
- RemovePreviousVersions - True
- RunPostBuildEvent - "on successful build"
- TargetPlatform - x64
- Title - HyRAM
- UpgradeCode - A6256A13-D1FE-4D2E-9BB8-DEE9FF314047
- Version: *Set as needed*

**Note**: the Upgrade Code describes an application with multiple versions. 
The Product Code describes a release of one of those versions.
The HyRAM Upgrade Code should never change unless a different application is desired,
such as another fuel analysis program that should be installed separately from HyRAM.
The Product Code should be changed with each new release.

Windows uses the installation context (i.e. "all users" or "just me" options), UpgradeCode and ProductCode when determining if a previous version of an application resides on a user's system.

### 3. Installer UI
Navigate to the UI options, Hyram.Setup -> View -> User Interface and set the following for each step:
- copyright - "Copyright 2015-2020 Sandia Corporation. WARNING: This computer program is protected by copyright law and international treaties. Unauthorized duplication or distribution of this program, or any portion of it, may result in severe civil or criminal penalties, and will be prosecuted to the maximum extent possible under the law."
- Finished -> UpdateText: "Thank you for using HyRAM"


### 4. Add Python Directory
The `python` and `pylibs` directories must be added to the Installer project manually via a custom directory.
Note that VSIP does not allow you to remove a custom directory if it is not empty, so a modified python interpreter directory usually necessitates remaking the Hyram.Setup project entirely.

Open the Installer Filesystem view: right-click `Hyram.Setup -> View -> File system`.
Navigate to the python directory in a separate window (or in the Solutions Explorer).
IMPORTANT: the python directory you place here will be the exact directory that is installed with HyRAM.
Make sure you select the correct directory!

(**Note**: before copying the python directories, ensure all `*.pyc` files are removed.
This can be done via command-line by navigating to the parent directory and executing the following command:
`find . -name '*.pyc' -delete` in bash or `del /S *.pyc` in Windows cmd (untested).)

1. In the left window of the File System view, right-click `Application Folder` and select `Add Folder`.
2. Name the created directory `python`
3. Open the directory properties (F4) and set `AlwaysCreate` to true
4. In Windows Explorer, open the python directory, highlight all contents and copy them
5. Back in MSVS, click the recently-created `python` dir in the `File System on Target Machine` hierarchy
6. In the right window, right-click and select Paste. (This may take awhile.)

The contents of the python dir will be copied into the installer's python directory.
If the system asks about adding a merge module, select No.


### 5. Add Python HyRAM module
The above process will be repeated for the `pylibs` directory, which contains the hyram module.
- Make sure any .pyc files or temp files are deleted prior to building via the above instructions.
- Add another Application folder to the File System and name it `pylibs`.
- Set the `AlwaysCreate` property of the folder to True.
- Copy and paste the `src/hyram/hyram` module directory into the `pylibs` folder.


### 6. Add Shortcuts and additional files
Next, add a shortcut to the user's desktop and Programs Menu:
- Add the shortcut icon: in the Hyram.Setup File System view, right-click the Application Folder, select Add New Item... and navigate to the Hyram.Gui directory on the filesystem and select the .ico file.
- Add the banner image via the same method, selecting `BannerLogo.jpg`
- Add the Python.runtime.dll file via the same method, selecting `python.runtime.dll`
- Back in MSVS in the Hyram.Setup File System view, select the Application Folder to display its current contents.
- Right-click `Hyram.Gui Primary Output` and select `Create Shortcut`.
- Select the shortcut file that was created and rename it to "HyRAM".
- Open the shortcut file properties (right-click -> Properties or F4) and change the icon file to the file added in step (1).
- Drag the shortcut to the left window, into the `User's Desktop` folder.
- Repeat this process and place the second shortcut in the `User's Programs Menu`.


### 7. Build the Installer
Once the above steps are complete, the Installer is ready to build.
`Right-click -> Build` the Installer in the Solution Explorer.
Note that this may take several minutes.
The final .msi installer should be over 250 Mb in size and will be located in the Hyram.Setup/build directory.
The output will also include a setup.exe file which checks that the end-user's system has the necessary C# redistributable installed.

If the build fails due to insufficient memory, try closing and reopening MSVS and repeating the build.


<a name="c-notes">&nbsp;</a>
## B.5 Miscellaneous Notes

#### Issue: form fails to display in MSVS designer
Forms can occasionally fail to load in the MSVS designer.
This can usually be solved in one of two ways. In both cases the issue seems to occur when custom controls are used.

First ensure that the custom control is not set as a public property of the form.
This can be done, for example, by automated cleanup tools like ReSharper.
When this occurs, ensure that the control is set as a private property in the designer.cs file.

Second, this issue can be caused by the project being built as 64-bit.
To work around this, set the form project platform to Any CPU temporarily. This should allow the designer to load the form.
You may need to restart MSVS after rebuilding for the change to propagate to the designer.


<a name="py-dev">&nbsp;</a>
# C. Python HyRAM Module Development

This section describes the layout, usage, and front-end integration of the Python HyRAM module. For more information about the installation, use, and developement of the Python HyRAM module specifically, see the [README](./src/hyram/README.md) file in the `src/hyram` directory. 

<a name="py-layout">&nbsp;</a>
## C.1 Module Layout
HyRAM primarily consists of two sub-modules: physics and qra.

    ├───hyram
    │   ├───phys
    │   ├───qra
    │   └───utilities

Each sub-module contains its own `c_api.py` access point, which is used by the C# GUI to interface with the toolset.
The physics sub-module also includes a `api.py` file for interacting with the main function calls programmatically.
The quantitative risk analysis algorithm can be found in `qra/analysis.py`.


    
<a name="py-usage">&nbsp;</a>
# D. Python HyRAM Usage
HyRAM can be utilized as a C# backend with Python.NET, or independently via Python as a standalone module.
C# calls via python.NET should utilize the `c_api.py` files.



<a name="py-usage-c">&nbsp;</a>
## D.1 HyRAM as C# Backend
The Python HyRAM package is integrated into the C# HyRAM application via a Python.NET interface.
A separate API file, `c_api.py`, is called from C# when conducting analyses via the interface files in the `Hyram.PythonApi` project.
The c_api.py files load custom Python.NET modules which are only available while the Python.runtime.dll is loaded.
It is not currently compatible with a normal Python session.

**Adding a new API function call to the C# GUI**

Add a new function to the PhysicsInterface or QraInterface files in the `Hyram.PythonApi` project.
The function should gather the appropriate inputs in a non-nullable format and activate the Python.NET lock space before calling the corresponding function in the `c_api.py` file.
Refer to existing functions for reference.

**Adding a new API python function call to the python HyRAM module**

A corresponding function call should be created in the suitable `c_api.py` file that the C# GUI will access.
The function should parse and convert incoming data; arrays can be handled with available utility functions.
Note that functions called by C# should not have keyword or optional arguments.
After parsing incoming data, the function should execute its API calls and then return a wrapped data set to C#.
Refer to existing functions for reference.


<a name="py-usage-py">&nbsp;</a>
## D.2 Using HyRAM as a Python module
QRA analysis can be executed via command-line through the `analysis.py` file.
See the analysis function docstring for details and guidance.

The analysis function is written in a procedural fashion.
It accepts all QRA parameters, declares few defaults, and returns a complete set of results for five leak sizes.
A few python objects are defined to aid in organization; the majority of the algorithm utilizes basic python and numpy types.
Logging is enabled by default and seeks to preserve each analysis step explicitly.



<a name="refs">&nbsp;</a>
# E. References

* Cython [Compiling C extension modules on Windows](https://github.com/cython/cython/wiki/CythonExtensionsOnWindows)

* MS 2017 Build Tools [downloads](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2017)

* Docs on distributing python modules [(link)](https://docs.python.org/3/distutils/index.html#distutils-index)
