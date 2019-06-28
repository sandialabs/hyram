
# The Hydrogen Risk Assessment Model
This document describes the Hydrogen Risk Assessment Model ("HyRAM") application development.
The application comprises a frontend GUI written in C# and a "PyHyRAM" backend module written in Python.
Step-by-step instructions are included for setting up a C# development environment using MS Visual Studio 2017 ("MSVS").
Similar setup instructions are provided for backend python development.
In addition, basic usage of the backend as a standalone python module is is provided.

&nbsp;

# TABLE OF CONTENTS

[A. Repository Layout](#repo-lay)

[B. C# GUI Development](#c-gui)<br>
&nbsp;&nbsp;[B.1 Solution Layout](#c-lay)<br>
&nbsp;&nbsp;[B.2 Set Up Python](#set-py)<br>
&nbsp;&nbsp;[B.3 Set Up Solution in MSVS](#set-msvs)<br>
&nbsp;&nbsp;[B.4 Installer Project Setup](#set-ins)<br>
&nbsp;&nbsp;[B.5 Misc. Notes](#c-notes)<br>
            
[C. PyHyRAM Python Module Development](#py-dev)<br>
&nbsp;&nbsp;[C.1 Module Layout](#py-lay)<br>
        
[D. PyHyRAM Usage](#py-use)<br>
&nbsp;&nbsp;[D.1 As C# Backend](#py-cb)<br>
&nbsp;&nbsp;[D.2 As standalone Python module](#py-mod)<br>

[E. References](#refs)<br>
    

<a name="sec-repo-lay">&nbsp;</a>
# A. Repository Layout
Comprehensive development of HyRAM includes both the C# frontend GUI and the backend Python module.
Application code is organized in the git repository in the following way:

```
├───App
│   ├───HyRAMSetup
│   ├───PyAPI
│   ├───PythonDir
│   └───QRA_Frontend
│   └───QRA.sln
├───middleware
│   ├───general
│   │   ├───DefaultParsing
│   │   ├───IOUtils
│   │   ├───JrCollections
│   │   ├───JrConversions
│   │   ├───JrRegistry
│   │   ├───JrSorting
│   │   ├───JrString
│   │   ├───JrTemp
│   │   ├───JrWindowsAPI
│   │   ├───ProcessHelpers
│   │   └───UIHelpers
│   ├───QRAState
│   └───science
│       ├───high_level_IO_support
│       └───imported_matlab_models
└───python_contributions
    ├───ethan_qra
    ├───hyram
    │   ├───hyram
    │   │   ├───phys
    │   │   ├───qra
    │   │   └───utilities
    │   └───test
    ├───hyramphys
    ├───hyram_python
    ├───miscellaneous
    ├───nonhyram
    └───not_integrated
```

Currently the C# projects comprising the GUI are located in the following top-level directories:

    /App
    /middleware

The active backend python module is located in:

    /python_contributions/hyram

Other python code is stored alongside the `hyram`. 
Note that the backend code is separated into three sub-modules which contain the physics, QRA and utilities code. 

**Developer's Note**: Directories such as bin, obj, build, and debug, user-specific Visual Studio settings (e.g. .vs directories),
and the Python interpreter directory and its contents should **not** be added to version control.
See the `.gitignore` file for other exclusions.



<a name="c-gui">&nbsp;</a>
# B. C# GUI Development
This section describes how to set up a development environment for the HyRAM graphical user interface.
The following steps are written for MS Visual Studio 2017 and an x64 solution configuration.

The C# GUI follows a custom Model-View-Controller ("MVC") paradigm, separate from the python backend, wherein
content panel forms define the views and controllers and a custom state object acts as the data model.


<a name="c-lay">&nbsp;</a>
## B.1 Solution Layout
The GUI code resides in the `App` and `middleware` top-level directories:

```
├───App
│   ├───HyRAMSetup
│   ├───PyAPI
│   ├───PythonDir
│   └───QRA_Frontend
│   └───QRA.sln
├───middleware
│   ├───general
│   │   ├───DefaultParsing
│   │   ├───IOUtils
│   │   ├───JrCollections
│   │   ├───JrConversions
│   │   ├───JrRegistry
│   │   ├───JrSorting
│   │   ├───JrString
│   │   ├───JrTemp
│   │   ├───JrWindowsAPI
│   │   ├───ProcessHelpers
│   │   └───UIHelpers
│   ├───QRAState
│   └───science
│       ├───high_level_IO_support
│       └───imported_matlab_models
```

**Dev Note:** Work is ongoing to update the code organization into a more streamlined, intuitive layout.


&nbsp;
### B.1.1 Key C# Project Descriptions

`App/HYRAMSetup` - the user-facing installer which packages the GUI and python code

`App/PyAPI` - interface which communicates with the PyHyRAM module via Python.NET. Includes separate classes for physics and QRA.

`App/PythonDir` - empty project containing the Python interpreter. Contains build events which copy the python interpreter and modules during build.

`App/QRA_Frontend` - the content panels, forms and logic for user-interaction (views and controllers).
Also includes the engineering toolkit.

`App/QRA.sln` - Main C# solution file.

`middleware/general` - utility projects required by the GUI.

`middleware/QRAState` - database which handles the model data and state, including user inputs.

`middleware/science` - now-unused files including prior versions of the analysis code written for matlab.



<a name="set-py">&nbsp;</a>
## B.2 Set Up Python
HyRAM requires an embedded Python 3.6 installation.
Note that the HyRAM VCS repository does not include the Python interpreter;
it should be installed manually according to the following steps.


&nbsp;
### B.2.1 Install Python 3.6
The HyRAM build process requires the Python 3.6 directory to be located at:

    App\PythonDir\python

(Note that the Python installation location is up to the user if intending to use the pyhyram module without the GUI.)

To install Python, navigate to the [Python 3.6 webpage](https://www.python.org/downloads/release/python-368/)
and download the Python 3.6 x86-64 executable installer ([direct link](https://www.python.org/ftp/python/3.6.8/python-3.6.8-amd64.exe)).
Follow the installation instructions:

 * *Optional Features*: enable `pip` and `tcl/tk` options only. Disable all other options.
 * *Advanced Options*: check `precompile standard library` only. Uncheck all other options.
 * Set the custom install location to `\location\of\repo\App\PythonDir\python\`
 * After installation completes, verify that the `python` directory exists in `App/PythonDir`
 * Navigate to the `tcl` directory in `App/PythonDir/python/tcl` and copy the `tck8.6` and `tk8.6` directories to `/python/Lib/`


<a name="">&nbsp;</a>
### B.2.2 Install Required Python Modules
The python backend requires the following modules:

```
cloudpickle==0.8.1
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
```

Follow the provided steps to correctly install these modules.

#### 1. Obtain core .whl files
Many of these modules are dependencies of numpy, scipy, pandas and matplotlib,
which should be installed first using Windows-specific .whl files found on the [Gohlke webpage](https://www.lfd.uci.edu/~gohlke/pythonlibs).
First download each .whl file pertaining to Python 3.6, x64
(the names will have suffixes similar to `cp36-win_amd64.whl`):

* Numpy 1.15 MKL: [numpy‑1.15.4+mkl‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#numpy)
* scipy: [scipy‑1.2.1‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#scipy)
* pandas: [pandas‑0.24.2‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#pandas)
* matplotlib 3.0.0rc2: [matplotlib‑2.2.4‑cp36‑cp36m‑win_amd64.whl](https://www.lfd.uci.edu/~gohlke/pythonlibs/#matplotlib)

#### 2. Place .whl files
Copy the downloaded .whl files from your download directory to the python directory (`/App/PythonDir/python`)

#### 3. Verify pip
Open a command-line prompt and navigate to the `PythonDir/python` directory.
Verify that the python pip tool is accessible by executing the following command:

    python -m pip --version

This should print the location of the pip command.
The location should be within the `PythonDir`; for example: `PythonDir/python/lib/site-packages/pip`.
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

    python -m pip install Pillow pythonnet pywavelets scikit-image dill pyreadline toolz Cython cloudpickle

#### 6. Verify modules
Verify that all the modules have been installed:

    python -m pip freeze

#### 7. Delete .whl files
The four core module .whl files (numpy, scipy, pandas, matplotlib) are not needed after installation and should be deleted.
They should **not** be added to version control.



<a name="set-msvs">&nbsp;</a>
## B.3 Set up Solution in MSVS
Before loading the solution, open Visual Studio and install the official **Microsoft Visual Studio Installer Projects** extension:

    Tools -> Extensions and Updates -> Online -> [Search]

After installing the extension and restarting MSVS, open the QRA solution by selecting the following file:

    /App/QRA.sln 
   
This will load the solution and its various projects. 
The active config (top of IDE) should be set to `Debug|Any CPU` during development.


&nbsp;
### B.3.1 Solution Settings
Verify the following configuration settings and properties after loading the solution in MSVS.

**Solution Properties** - Right-click Solution 'QRA' in Solution Explorer:
* Common Properties -> Startup Project -> Simple Startup project -> QRA_Frontend
* Configuration Properties under `Debug|Any CPU` : all should be set to `Debug|Any CPU` with Build checked,
except HyRAMSetup (installer), which should be unchecked.

**QRA_Frontend project settings** - In solution explorer, right-click project -> properties:
* Application -> Startup object should be `QRA_Frontend.Program`
* Under Build, set `Platform target` to `x64`.
Output path should be any build location which is **not tracked** by git.

**PyAPI project settings** - Right-click project -> properties:
* Under Build, set `Platform target` to `x64`.

**Output paths**:
* All project output paths (project settings -> Build) should be set to suitable location which is **not tracked** by git.


&nbsp;
### B.3.2 Build Events
Two post-build events reside in the `PythonDir` project (`PythonDir properties -> Build Events`)
and are used to ensure the python files are available to the compiled application.

The first copies the Python interpreter into the build directory.

The second copies the pyhyram python module into a `pylibs` build sub-directory alongside the python directory.

These events only need to be executed during the first build and when the python code changes thereafter.
The python interpreter event only needs to be run when a new module is installed or updated.
The pyhyram event only needs to be executed when the code has been modified.
The events can be disabled when the python code has not been modified between builds
(i.e. the developer is working on the C# code).
To disable an event, modify the conditional statement to ensure it is false; for example:

    if "$(ConfigurationName)" == "AlwaysFalse"

To enable the event, make sure the conditional matches the Config: `== "DEBUG"` or `== "RELEASE"`.


&nbsp;
### B.3.3 Python.NET Runtime
The Python.NET runtime DLL must be added as a reference for any projects which rely on it,
including QRA_Frontend and PyAPI.
In the Solution Explorer right-click the project name and select `Add -> Reference`.
Under the `Browse` tab, `Python.Runtime.DLL` should be listed (and checked) with a location similar to:

    /App/PythonDir/python/Lib/site-packages/Python.Runtime.DLL
    
If it is not listed, click `Add` and browse to the DLL in the above location.
The Solution should now be Buildable in VS.



<a name="set-ins">&nbsp;</a>
## B.4 Installer Project Setup
This section describes the steps required to build an Application Installer which can be used to install HyRAM on end-user machines.
This process involves setting up the Installer Project in C# HyRAM, incorporating the `python` and `pythonlib` directories into it, and building the final installer.

(Note: This process requires the free *Visual Studio Installer Projects* tool for MSVS.
The 2017 version can be found [here](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2017InstallerProjects))


&nbsp;
### B.4.1 Add the Installer Project
This step assumes the `HyRAMSetup` installer project does not already exist in the QRA solution.
You can also start here if you plan on remaking the project.

First, navigate to the `Add New Project` menu by selecting it from the `File` menu, typing `Ctrl+Shift+N` or by
right-clicking on the `Solution 'QRA'` and selecting `Add -> New Project`.

Select `Installed -> Other Project Types -> Visual Studio Installer` to view the available Installer project options.
Select the `Setup Wizard` and enter the following parameters:
* Name: HyRAMSetup
* Location: `/repo/software/App`
* Solution: Add to solution

The Setup Wizard dialog will open. Navigate the steps with the following inputs:
* Step 2/5: select *Create setup for a Windows Application* 
* Step 3/5: select the Primary Output of each project and the Content Files when relevant. Do NOT select source files.
* Click Finish (skip step 4)


&nbsp;
### B.4.2 Set Installer Properties
Open the Installer Project Properties (`right-click -> Properties`) and set the following:
* Config Release - Output filename: `Release\HyRAM.msi` (if doing debug config, place in `Debug` dir instead)

Open the Deployment Project Properties (Highlight Installer in Solutions Explorer and push F4):
* Author/Manufacturer: Sandia National Laboratories
* Product Name: HyRAM
* RemovePreviousVersions: True
* TargetPlatform: x64
* Title: HyRAM
* Version: (depends)


&nbsp;
### B.4.3 Add Python Directory
The `python` and `pylibs` directories must be added to the Installer project manually.
If you are replacing the existing versions of either directory, make sure you delete the relevant directory from the
installer (`right-click -> View -> filesytem -> application`).

Open the Installer Filesystem view: `right-click installer -> View -> File system`.
Navigate to the python directory in a separate window (or in the Solutions Explorer).
IMPORTANT: the python directory you place here will be the exact directory that is installed with HyRAM.
Make sure you select the correct directory!

(**Dev Note**: before copying the python directories, ensure all `*.pyc` files are removed.
This can be done via command-line, after navigating to the pyhyram directory,
via `find . -name '*.pyc' -delete` in bash or `del /S *.pyc` in Windows cmd (untested).)

1. In the left window of the File System view, right-click `Application Folder` and select `Add Folder`.
2. Name the created directory `python`
3. Open the directory properties (F4) and set `AlwaysCreate` to true
4. In Windows Explorer, open the python directory, highlight all contents and copy them
5. Back in MSVS, click the recently-created `python` dir in the `File System on Target Machine` hierarchy
6. In the right window, right-click and select Paste.

The contents of the python dir will be copied into the installer's python directory.
You may see the following dialog appear multiple times:
* Asks if you want to add a merge module instead. Select No.


&nbsp;
### B.4.4 Add Python Libs
The above process will be repeated for the `pylibs` directory, which contains the pyhyram module.
* Add another folder and name it `pylibs`
* Copy and paste the contents of the actual `pylibs` directory into the installer `pylibs` dir. (It may be easiest to navigate to the `pylibs` dir in your build directory.)
Make sure any .pyc files or temp files are deleted prior to building.
* NOTE: if building with .pyd files, make sure only the .pyd files are copied!
* Set the `AlwaysCreate` property of the folder to True.


&nbsp;
### B.4.5 Add Shortcuts
Next, we'll ensure the installer adds a shortcut to the user's desktop and Programs Menu.
Navigate to the Installer Filesystem and select the Application Folder.
Right-click `QRA_Frontend Primary Output` and select `Create Shortcut`.
Drag the shortcut to the left window, into the `User's Desktop` folder.

Repeat this process and place the second shortcut in the `User's Programs Menu`.


&nbsp;
### B.4.6 Build the Installer
Once the above steps are complete, the Installer is ready to build.
`Right-click -> Build` the Installer in the Solution Explorer.
Note that this may take several minutes.
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
To work around this, set the form project to Any CPU temporarily. This should allow the designer to load the form.


<a name="py-dev">&nbsp;</a>
# C. PyHyRAM Python Module Development


<a name="py-lay">&nbsp;</a>
## C.1 Module Layout
PyHyRAM primarily consists of two sub-modules: physics and qra.
Each sub-module contains its own `api.py` access point.

    ├───hyram
    │   ├───phys
    │   ├───qra
    │   └───utilities

The quantitative risk analysis algorithm can be found in `qra/analysis.py`.


    
<a name="py-use">&nbsp;</a>
# D. PyHyRAM Usage
PyHyRAM can be utilized as a C# backend with Python.NET, or independently via Python as a standalone module.
The primary module access point for each sub-module is their `api.py` file.
C# calls via python.NET should utilize the `capi.py` files.



<a name="py-cb">&nbsp;</a>
## D.1 PyHyRAM as C# Backend
PyHyRAM is integrated into the C# HyRAM application via a Python.NET interface.
A separate API file, `capi.py`, is called from C# when conducting analyses.
This distinct API loads modules which are only available while the Python.runtime.dll is loaded.
It is not currently compatible with a normal Python session.


<a name="py-mod">&nbsp;</a>
## D.2 PyHyRAM as a Python module
The primary analysis function is contained in the `analysis.py` file.
Analyses can be conducted programmatically via the `api.conduct_analysis_from_dict` function,
which accepts a Python dict of analysis parameters and conducts a full analysis.

The analysis function is written in a procedural fashion.
It accepts all QRA parameters, declares few defaults, and returns a complete set of results for five leak sizes.
A few python objects are defined to aid in organization; the majority of the algorithm utilizes basic python and numpy types.
Logging is enabled by default and seeks to preserve each analysis step explicitly.



<a name="refs">&nbsp;</a>
# E. References

* Cython [Compiling C extension modules on Windows](https://github.com/cython/cython/wiki/CythonExtensionsOnWindows)

* MS 2017 Build Tools [downloads](https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2017)

* Docs on distributing python modules [(link)](https://docs.python.org/3/distutils/index.html#distutils-index)
