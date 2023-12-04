# Hydrogen Plus Other Alternative Fuels Risk Assessment Models (HyRAM+)
The Hydrogen Plus Other Alternative Fuels Risk Assessment Models (HyRAM+) toolkit integrates deterministic and probabilistic models for quantifying accident scenarios, predicting physical effects, and characterizing the impact on people from hydrogen and other alternative fuels.

Additional descriptions and documentation, as well as a Windows installer, can be found at https://hyram.sandia.gov/.

&nbsp;
## Copyright and License
The copyright language is available in the [COPYRIGHT.txt](./COPYRIGHT.txt) file.
The license, as well as terms and conditions, are available in the [COPYING.txt](./COPYING.txt) file.

&nbsp;
## Contributing
The application comprises a frontend GUI written in C# and a backend module written in Python.
Anyone who wants to contribute to the development of the open-source HyRAM+ project should refer to the details in the [CONTRIBUTING](./CONTRIBUTING.md) document.

&nbsp;
## Documentation
The [HyRAM+ Technical Reference Manual](https://hyram.sandia.gov/) contains descriptions of the models and calculations used within HyRAM+. It also contains references to the original works that these models and calculations are based on.

The [HyRAM 2.0 User Guide](https://energy.sandia.gov/download/44669/) contains details and examples on how to use the HyRAM+ software through the graphical user interface (GUI), with example calculations updated with changes to the interface and improved calculation options. This document more references how to use the software interface, rather than specifics on the models and calculations themselves. While there have been many changes to the current HyRAM+ version of the code, many of the examples are still applicable even though the User Guide is based on the previous version; a new version of the User Guide will be published in the future.

&nbsp;
## Repository Layout
The HyRAM+ repository includes both the C# frontend GUI and the backend Python module.
Application code is organized in directories in the git repository in the following way:

```
$
└───src
    ├───gui
    │   ├───Hyram.gui
    │   ├───Hyram.PythonApi
    │   ├───Hyram.PythonDirectory
    │   ├───Hyram.Setup
    │   ├───Hyram.SetupBootstrapper
    │   ├───Hyram.State
    │   ├───Hyram.Units
    │   └───Hyram.Utilities
    ├───cs_api
    └───hyram
        ├───tests
        └───hyram
            ├───phys
            ├───qra
            └───utilities
```

* `src` - Project source code, including C# GUI and python modules
* `src/gui` - Front-end C# interface providing convenient access to HyRAM+ tools
* `src/cs_api` - Python functions providing C# access to HyRAM+ python code via the python.NET library.
* `src/hyram` - Python module of HyRAM+ tools including physics, quantitative risk assessment, and miscellaneous utilities
    * Additional information on the usage and development of the HyRAM+ Python module can be found in the [README](./src/hyram/README.md) of that directory
* `src/hyram/hyram` - Python source code for physics and risk models
    * This directory contains the code for the risk and physics model calculations that are accessible through the front-end GUI
