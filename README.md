# Hydrogen Plus Other Alternative Fuels Risk Assessment Models (HyRAM+)
The Hydrogen Plus Other Alternative Fuels Risk Assessment Models (HyRAM+) toolkit integrates deterministic and probabilistic models for quantifying accident scenarios, predicting physical effects, and characterizing the impact on people from hydrogen and other alternative fuels.

Additional descriptions, documentation, and installation files can be found at https://hyram.sandia.gov/.

## Copyright and License
The copyright language is available in the [COPYRIGHT](./COPYRIGHT.txt) file.
The license, as well as terms and conditions, are available in the [COPYING](./COPYING.txt) file.

## Contributing
The application comprises a frontend GUI and a backend package, both written in Python.
Anyone who wants to contribute to the development of the open-source HyRAM+ project should refer to the details in the [CONTRIBUTING](./CONTRIBUTING.md) document.

## Documentation
The HyRAM+ Technical Reference Manual (available at https://hyram.sandia.gov) contains descriptions of the models and calculations used within HyRAM+. It also contains references to the original works that these models and calculations are based on.

The HyRAM+ User Guide (available at https://hyram.sandia.gov) contains details and examples on how to use the HyRAM+ software through an older graphical user interface (GUI). This document explains how to use the software interface, rather than specifics on the models and calculations themselves. While there have been many changes to the current HyRAM+ version of the code, many of the examples are still applicable even though the User Guide is based on the previous version; a new version of the User Guide will be published in the future.

## Repository Layout
The HyRAM+ repository includes both the frontend GUI and the backend package.
Source code is organized in directories in the git repository in the following directory structure:
```
├───gui
├───src
|   └───hyram
|       ├───phys
|       ├───qra
|       └───utilities
└───tests
    ├───gui
    └───hyram
        ├───phys
        ├───qra
        └───validation
```

* `gui` - Code and resources for developing the Qt-based graphical user interface and building installers for Windows and macOS.
* `src/hyram` - Python package of HyRAM+ tools including physics, quantitative risk assessment, and miscellaneous utilities.
    * Additional information on the usage and development of the HyRAM+ Python module can be found in the [README](./src/hyram/README.md) of that directory.
* `tests` - Tests of the project source code to verify calculations and functionality.
* `tests/gui` - Tests for the GUI models and state functionality.
* `tests/hyram` - Tests for the HyRAM+ Python package including physics, risk assessment, and physics validation.
