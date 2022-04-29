# HyRAM+ Python Library

The HyRAM+ library contains Python modules for physics and QRA calculations.


# Installation

To install this Python module, navigate to this directory on your machine and type:

~~~~~~~~~
pip install .
~~~~~~~~~


# Usage

The HyRAM+ package can be utilized after installation using a standard Python import:
```python
import hyram
```

Specific models can be accessed within the HyRAM+ package. The main calculations for the physics models can be accessed using `hyram.phys.api`. This includes models such as:
* `create_fluid`
* `compute_mass_flow`
* `compute_tank_mass`
* `compute_thermo_param`
* `compute_equivalent_tnt_mass`
* `analyze_jet_plume`
* `analyze_accumulation`
* `jet_flame_analysis`
* `compute_overpressure`

The following are the main model objects that can be accessed using `hyram.phys`:
* `Fluid`
* `Orifice`
* `Jet`
* `Flame`
* `Source`
* `Enclosure`
* `Vent`
* `IndoorRelease`
* `BST_method`
* `TNT_method`
* `Bauwens_method`

The main risk calculations are located in `hyram.qra.analysis.conduct_analysis`.

More information about the specific models and the inputs/outputs for each one can be found in the specific modules themselves. 


# Development/Modifications

To make changes to the source code, either for a specific need or to contribute to HyRAM+, please follow the instructions in the [CONTRIBUTING](./CONTRIBUTING.md) file for the HyRAM+ Python library. 
