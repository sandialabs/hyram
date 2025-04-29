# Contributing to HyRAM+
This document describes the Hydrogen Plus Other Alternative Fuesl Risk Assessment Models ("HyRAM+") application development.
The application comprises a Qt-based GUI and a backend module written in Python.
Step-by-step instructions are included for setting up a development environment for both the GUI and scientific library.

# Changelog
For any significant changes made to the source code, it is expected that the change will be summarized in the [CHANGELOG](./CHANGELOG.md) document. Guidance and suggestions for how to best enter these changes in the changelog are [here](https://keepachangelog.com/en/1.1.0/). New changes should be added to the `[Unreleased]` section at the top of the file; these will be removed to a release section during the next public release.


# Table of Contents
[A. Repository Layout](#repo-layout)<br>

[B. GUI Development](#gui)<br>

&nbsp;&nbsp; [B.1. Development Environment Setup](#gui-dev)

&nbsp;&nbsp; [B.2. Building the HyRAM+ Application for Distribution](#gui-distr)

[C. HyRAM+ Module Development](#py-dev)<br>

&nbsp;&nbsp;[C.1 Package Layout](#py-layout)<br>

&nbsp;&nbsp;[C.2 Installation](#py-install)<br>

[D. HyRAM+ Usage](#py-usage)<br>



<a name="repo-layout">&nbsp;</a>
# A. Repository Layout
Development of HyRAM+ includes both the Qt GUI and the backend scientific Python module.
Source code is organized as follows:

```
$
├─── gui
│   ├─── build
│   │   ├─── windows
│   │   └─── mac
│   ├─── src
|   │   ├─── hyramgui
|   │   │   ├─── assets
|   │   │   ├─── forms
|   │   │   ├─── models
|   │   │   ├─── ui
|   │   │   └─── hygu
├─── src
│   └─── hyram
│       ├─── phys
│       ├─── qra
│       └─── utilities
└───tests
    ├─── gui
    └─── hyram
        ├─── phys
        ├─── qra
        └─── validation
```

* `src` - HyRAM+ scientific library Python package.
* `src/hyram` - HyRAM+ scientific library source code, including physics and quantitative risk assessment tools.
* `gui` - Qt-based multi-platform GUI Python package.
* `gui/build` - Scripts for building GUI installers for Windows, intel-based macOS, and Apple Silicon macOS.
* `gui/src` - GUI source code.
* `tests` - automated tests for both the GUI and HyRAM+ (`hyram`) packages.


<a name="gui">&nbsp;</a>
# B. GUI Development
This section describes how to set up a cross-platform development environment for the HyRAM+ GUI.
It includes instructions for both developing and distributing the GUI application to users of Windows and macOS systems.

The GUI uses the Qt framework and PySide wrapper to implement the UI and to interface with the backend python HyRAM+ library.
This document assumes familiarity with Python 3.9, the Qt framework, and basic JavaScript, which is used in Qt UI .qml files.

**NOTE: The Qt framework has a complex licensing situation.
The HyRAM+ team must always verify that no incompatible modules are included in any release.**

### Nomenclature

The following terms and labels are used in this document:

    GUI         - The HyRAM+ Graphical User Interface
    backend     - analysis code found in the HyRAM+ module
    repo/       - path to the HyRAM+ repository on your machine; e.g. P:/projects/hyram/repo
    build/      - path to the GUI installers directory, (repo/gui/build/)

<a name="gui-dev"></a>
## B.1. Development Environment Setup

### Step 1. Clone the Repository
The GUI and backend code reside within the HyRAM+ repository.
Clone it via the gitlab instructions. Make sure to initialize any submodules as well.

### Step 2. Set Up a Python Development Environment
HyRAM+ and the GUI require a standard Python 3.9 virtual environment.
See Python documentation for installing Python 3.9 and creating a new virtual environment.

For development, the following requirements file contains all required python modules.
Activate your virtualenv before installing these:

    python pip install -r repo/gui/requirements-dev.txt

**Warning**: the dev modules must NOT be bundled into a HyRAM+ distribution.
Many of these modules have incompatible licenses for distribution and are used for development only.
For example, *jupyter* must not be included in the distribution.
The bundling scripts include an import check which halts the process if certain modules are found.

After installing the requirements, navigate to the `gui` directory and install it as an editable module:

    cd gui
    pip install -e .


### Step 3. Set Up a Separate Distribution Virtual Environment
A separate virtual environment must be created for building release-ready HyRAM+ distributions.
This env contains only those modules necessary for the distribution and excludes development-only modules like *jupyter*.

    python pip install -r repo/gui/requirements.txt

Next, install the GUI as an editable module into the release env as above.


### Step 4. Install Qt 6.6
*(Note: this step requires a free Qt account)*

Download the Qt online installer for Open Source Qt [here](https://www.qt.io/download-open-source).
Open the installer and select "Custom Installation". Modify the components as follows:

* Disable Qt Design studio
* Enable Qt 6.6
* Under Qt 6.6, enable only the following options:
    * (Windows) MSVC 64-bit
    * (Windows) MinGW 64-bit
    * (macOS) macOS option
* Under Qt 6.6 > Additional libraries, enable only the following:
    * Qt Image Formats
    * Qt WebEngine
    * Qt WebView

Wait for the installer to finish.

*...an eternity later...*


### Step 5. Set Up Project in QtCreator
The QtCreator IDE is recommended for developing the UI .QML files and running the application during development.
After opening the `gui` directory in QtCreator, modify the project settings as follows:
1. Select the Projects tab (on the left) > Run
2. Add a new Python interpreter
3. In the interpreter settings, navigate to the python exe or symlink in your env.
4. Make sure the specified virtualenv is now selected for the project

**Careful**: QtCreator on macOS will try to follow the symlinked python when it is selected during the above steps.
If this occurs, the path will be set to `env/bin/python3.9` instead of to your virtualenv.
Revise this path to make sure the interpreter location points to the symlink file in the virtualenv, and NOT the systemwide parent bin/python.

It should be something like:

    /Users/cianan/projects/hyram/envs/py3.9r/bin/python3.9

And not:

    /Library/Frameworks/Python.Framework/Versions/3.9/Python

Once the project is set up, click the "Run" button to launch the Qt application.
Open the .qml files in `gui/ui` to edit the interface, or use your favorite IDE with *.qml functionality.

<a name="gui-distr"></a>
## B.2. Building the HyRAM+ Application for Distribution

Building HyRAM+ is a two-step process. First, the GUI components and python files are bundled via pyInstaller.
Next, the bundled files are incorporated into a platform-specific installer package.
These steps must be conducted on the platform for which a distribution is being created.
For example, the Intel-based macOS distribution must be built on an Intel-based macOS system.

Note that the .sh scripts used below include filters for excluding license-incompatible modules according to
the [Qt Docs](https://doc.qt.io/qt-6/qtmodules.html#gpl-licensed-addons).

<a name="distr-win"></a>
### HyRAM+ for Windows

#### Requirements

The following tools are required to build HyRAM+ on Windows:
* Inno Setup Compiler (https://jrsoftware.org/isdl.php)

#### Step 0. Update Version and Configuration
Update the HyRAM+ versioning in the .spec file and within the application code.
Also check that the build configuration is set to `DEBUG=False` in the `app_settings.py` file.

#### Step 1. Create the HyRAM+ bundle
Update the version number in the .spec file before running pyinstaller.

    cd gui\build\windows\
    pyinstaller .\build_win.spec --noconfirm

To test the bundle executable:

    cd gui\build\windows\
    .\dist\hyram\HyRAM.exe

#### Step 2. Build the distribution setup file
1. Open Inno Setup compiler
2. Select the  `build_win_pkg.iss` script file
3. Click "compile"

This creates the HyRAM+ installer .exe file (named "mysetup.exe") in the `build/windows/installer` directory.
This file can be renamed and distributed to end-users.


<a name="distr-mac"></a>
### HyRAM+ for macOS
Mac-based distributions of HyRAM+ follow the same basic build process;
however, there are some slight differences if building for Intel-based Mac vs. Apple-silicon Macs.
These differences are described below.

#### Requirements
Before attempting to build HyRAM+, verify that the following prerequisites are present:
* Xcode (https://developer.apple.com)
* Xcode command-lines tools (see below)
* An active Apple Developer account
* create an app-specific password for code-signing [here](https://www.qt.io/download-open-source)

It is important to **never accidentally include credentials in a git commit**.
Use this command to store your credentials on your machine:

    xcrun notarytool store-credentials "<keychain profile>"
                   --apple-id "AC_USERNAME"
                   --team-id <WWDRTeamID>
                   --password <secret_2FA_password>

Xcode command-line tools can be installed via the Terminal:

    xcode-select --install

#### Step 0. Update Version and Configuration
Update the HyRAM+ version fields in the *.spec, distribution.xml, and *.sh files, and within the GUI.
Also check that the build configuration is set to `DEBUG=False` in the `app_settings.py` file.

#### Step 1. Create the HyRAM+ app bundle
Create the bundle via the .spec script in the `build/mac/dist` directory.

    cd build/mac/
    pyinstaller build_mac.spec --noconfirm

To verify the code-signed .app:

    codesign -v -vvv --deep --strict dist/HyRAM.app

To test the bundle:

    cd build/mac/
    dist/hyram/HyRAM


#### Step 2. Build Mac package installer
Execute the script to build a .pkg installer from the HyRAM.app file.
This script will also submit the pkg to Apple for notarization.

Note that this will copy the HyRAM.app to a child dist/app/ directory.
This is done so that the bundle process can correctly identify it as a component.

Pass your codesign identity as the first argument and credential profile as the second.
Remember to surround entries with quotes "" if there are spaces.

    ./build_mac_pkg.sh "<Installer ID>" "<Profile>"

Notarization may take a long time to complete. To check on the status or poll for updates:

    xcrun notarytool info HyRAM.app.zip --keychain-profile "<Profile>"
    xcrun notarytool wait HyRAM.app.zip --keychain-profile "<Profile>"


### References

Qt QML Documentation <br>
https://doc.qt.io/qt-6/qmltypes.html

PyInstaller on signing macOS bundles<br>
https://pyinstaller.org/en/stable/feature-notes.html?highlight=identity#app-bundles

The Apple notarization process<br>
https://developer.apple.com/documentation/security/notarizing_macos_software_before_distribution
https://developer.apple.com/documentation/security/notarizing_macos_software_before_distribution/customizing_the_notarization_workflow
https://www.unix.com/man-page/osx/1/productbuild/



<a name="py-dev">&nbsp;</a>
# C. Python HyRAM+ Module Development

This section describes the layout, usage, and front-end integration of the Python HyRAM+ module. 
The complete package source code can be cloned from the [HyRAM+ Github page](https://github.com/sandialabs/hyram).

The standalone Python package source code can also be downloaded from [PyPI](https://pypi.org/project/hyram/#files) or from [Conda-Forge](https://anaconda.org/conda-forge/hyram/files).

For more information about the installation, use, and developement of the Python HyRAM+ module specifically, see the [README](./src/hyram/README.md) file in the `src/hyram` directory.

<a name="py-layout">&nbsp;</a>
## C.1 Module Layout
HyRAM+ primarily consists of two sub-modules: physics and qra.

    ├───hyram
        ├───phys
        ├───qra
        └───utilities

The physics sub-module includes a `api.py` file for interacting with the main function calls programmatically.
The quantitative risk analysis algorithm can be found in `qra/analysis.py`.

<a name="py-install">&nbsp;</a>
## C.2 Installation

Install the HyRAM+ module via the `editable` flag (symbolic link) to enable modifications without re-installation.
Navigate to the repository root directory containing the `pyproject.toml` file, and run:
~~~~ 
pip install -e .
~~~~

This is especially useful with the `%load_ext autoreload` and `%autorelaod 2` commands in iPython/Jupyter Notebooks.





<a name="py-usage">&nbsp;</a>
# D. Python HyRAM+ Usage
QRA analysis can be executed via command-line through the `qra/analysis.py` file.
Physics analyses can be conducted via the `phys/api.py` file.
Docstrings are provided for details and guidance.
For more information about Python usage, see the [README](./src/hyram/README.md) file in the `src/hyram` directory.
