# HyRAM+ Python Package

The Hydrogen Plus Other Alternative Fuels Risk Assessment Models (HyRAM+) Python package contains modules for the calculation of physical release and risk for alternative fuels.


## Installation

To install [HyRAM+ from the Python Package Index](https://pypi.org/project/hyram/), use the command:

```
pip install hyram
```

More instructions on installing Python packages with `pip` can be found [here](https://packaging.python.org/en/latest/tutorials/installing-packages/).

To install [HyRAM+ from Conda-Forge](https://anaconda.org/conda-forge/hyram), use the command:

```
conda install -c conda-forge hyram
```

More instructions on installing Python packages with `conda` can be found [here](https://docs.anaconda.com/anacondaorg/user-guide/howto/#use-packages).


## Usage

The HyRAM+ package can be utilized after installation using a standard Python import:
```python
import hyram
```

Specific models can be accessed within the HyRAM+ package.
The main calculations for the physics models can be accessed using `hyram.phys.api`.
This includes models such as:
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
* `FuelProperties`

The main risk calculations are located in `hyram.qra.analysis.conduct_analysis`.

More information about the specific models and the inputs/outputs for each one can be found in the specific modules themselves. 


## Documentation

The models and equations used in the HyRAM+ Python package are described in the Technical Reference Manual, located on the [HyRAM+ website](https://hyram.sandia.gov).


## Development/Modifications

To make changes to the source code, either for a specific need or to contribute to HyRAM+, please follow the instructions in the [CONTRIBUTING](https://github.com/sandialabs/hyram/blob/master/src/hyram/CONTRIBUTING.md) file for the HyRAM+ Python package. 
