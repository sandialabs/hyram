# The Hydrogen Risk Assessment Models
The Hydrogen Risk Assessment Models ("HyRAM") toolkit integrates deterministic and probabilistic models for quantifying accident scenarios, predicting physical effects, and characterizing hydrogen hazards’ impact on people and structures. 
HyRAM incorporates generic probabilities for equipment failures and probabilistic models for heat-flux impact on humans and structures, with computationally and experimentally validated models of hydrogen release and flame physics.

Additional descriptions and documentation, as well as a Windows installer, can be found at https://hyram.sandia.gov/.

&nbsp;
## License
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
This license, as well as terms and conditions, are available in the [COPYING.txt](./COPYING.txt) file. 

&nbsp;
## Contributing
The application comprises a frontend GUI written in C# and a "HyRAM" backend module written in Python.
Anyone who wants to contribute to the development of the open-source HyRAM project should refer to the details in the [CONTRIBUTING](./CONTRIBUTING.md) document. 

&nbsp;
## Documentation
The [HyRAM 3.1 Technical Reference Manual](https://hyram.sandia.gov/) contains descriptions of the models and calculations used within HyRAM. It also contains references to the original works that these models and calculations are based on.

The [HyRAM 2.0 User Guide](https://energy.sandia.gov/download/44669/) contains details and examples on how to use the HyRAM software through the graphical user interface (GUI), with example calculations updated with changes to the interface and improved calculation options. This document more references how to use the software interface, rather than specifics on the models and calculations themselves. While there have been many changes to the current HyRAM version of the code, many of the examples are still applicable even though the User Guide is based on the previous version; a new version of the User Guide will be published in the future. 

&nbsp;
## Repository Layout
The HyRAM repository includes both the C# frontend GUI and the backend Python module.
Application code is organized in directories in the git repository in the following way:

```
$
└───src
    ├───gui
    │   ├───Hyram.gui
    │   ├───Hyram.PythonApi
    │   ├───Hyram.PythonDirectory
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

* `src` - Project source code, including C# GUI and python module(s)
* `src/gui` - Front-end C# interface providing convenient access to HyRAM tools
* `src/hyram` - Python module of HyRAM tools including physics, quantitative risk assessment, and miscellaneous utilities
    * Additional information on the usage and development of the HyRAM Python module can be found in the [README](./src/hyram/README.md) of that directory
* `src/hyram/hyram` - Python source code for physics and risk models
    * This directory contains the code for the risk and physics model calculations that are accessible through the front-end GUI
