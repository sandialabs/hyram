# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [6.0] - 2025-04-29

### Added
- Added new cross-platform GUI and build scripts for Windows and macOS
- Added unconfined overpressure tests to the validation test suite
- Added ability to skip heatflux plot in Physics API module
- Added uncertainty quantification framework, currently assesses leak frequency, dispenser failure, and occupant location distributions
- Added default component counts and helper default creation functions from the HyRAM+ user manual
- Added testing suites for the `qra/component` and `qra/uncertainty` modules
- Added CONTRIBUTORS file

### Changed
- Fixed deprecation warnings for some SciPy and Matplotlib calls
- Changed Python minimum version requirement to 3.9, removed maximum version requirement
- Changed QRA function call in standalone Python package to use pipe ID rather than OD and pipe thickness
- Changed default save location for files generated from Python package from sub-directory to current working directory
- Changed ignition probabilities to a combined dictionary input rather than three seperate list inputs for the Python QRA analysis function
- Changed ignition probabilities to an optional input to the Python QRA analysis function; if not specified, default values are used based on whether or not hydrogen is part of the fluid
- Optimized code structure for QRA analysis and effects calculation
- Renamed `ComponentSet` class to `Component` in QRA
- Revamped QRA `Leak`, `Component`, and `ComponentFailure` classes to fit into UQ framework more cleanly
- Renamed QRA event result keys for improved clarity
- Changed event probability calculations in `conduct_analysis` to use NumPy arrays to improve clarity and efficiency
- QRA `ComponentFailureSet` now uses `failures` as an input, containing a list of `ComponentFailure` objects, rather than the individual distribution parameters
- QRA `ComponentFailure` is now a frozen dataclass for easier repr reading (and will save substantial memory once HyRAM+ is Python >=3.10)
- Sped up QRA analysis by skipping unneeded consequence calculations for 0 risk conditions
- Modified isentropic blowdown calculation to use interpolating functions, which improves calculation speed by limiting calls to CoolProp
- Simplified QRA analysis function in backend (python package) to use list of locations rather than occupant distributions
- Changed QRA calculation input for release angle to be radians instead of degrees for consistancy
- Updated default filter leak frequencies for gaseous hydrogen, gaseous methane, and propane to match SAND2024-06491
- Updated default overpressure contour levels to match SAND2023-12548
- Updated integrator for blowdown function from scipy.integrate.ode to scipy.integrate.solve_ivp to improve time-step handling and improve calculation speed
- Updated blowdown validation test limits for new integrator; some error metric worsened slightly due to different time-steps, but overall fit metrics generally improved
- Changed fluid phase (`rel_phase`) input to QRA `conduct_analysis` function to be optional; by default, the value is `None` and a user must specify the fluid temperature and pressure
- Added default values of 0.03 (3%) for the TNT equivalence factor and 0.35 for the BST Mach flame speed for the QRA `conduct_analysis` function to better align with the GUI and physics mode
- Updated calculation of x, y distance to mole fraction contour to use contourpy instead of generating a contour plot
- Changed physics backend from Orifice.flow function to a NozzleFlow class to make Pythonic syntax and calcuation of flows more intuitive
- Updated calculation of zone of flow establishment length based on the Froude number so that low Froude number releases have length of 0
- Changed output dictionary keys for x-/y-distances to mole fractions to unignited plume, no longer NumPy objects
- Renamed QRA `data` sub-directory to `defaults` and moved many of the default values there
- Moved calculation of total and conditional ignition probability to `ignition_probs.py` for clarity
- The fault tree override (`f_release_overrides`) input to the QRA `conduct_analysis.py` function has been renamed `ft_overrides` and now uses values of `None` rather than -1 to show when an override should not be used.
- Updated Python version dependency to exclude 3.13

### Removed
- Removed C#-based Windows GUI and `cs_api` integration library
- Removed facility length and width for QRA plots (was previously unused in calcualtions)
- Removed `LeakSizeResult` and `ComponentSet`, with functionality moved to Component and Leak classes/modules and `generate_event_results` function
- Removed `positions.py` module and associated `PositionGenerator` class, position generation now in `uncertainty.py`
- Removed `get_component_leak_parameters` function in `component_leak_frequencies.py`

### Fixed
- Corrected call to QRA overpressure effects to pass a release angle in radians rather than degrees
- Corrected flammable mass calculation in Jet class, was previously being undercalculated by missing some flammable mass near fuel-rich region
- Updated call to scipy.integrate.ode that was causing errors on MacOS (see changed)


## [5.1.1] - 2024-02-08

### Changed
- Re-organized source code to simplify repository structure
- Changed delimiter of list-based inputs in the GUI to a space (' ') instead of a comma (',') to improve compatibility for international users
- Fixed bug with empty temp folder being created when not needed
- Renamed 'temp' folder to 'out'
- Fixed typo in package keywords
- Fixed R-squared calculation in validation tests
- Updated validation test criteria for 5.1
- Fixed a bug with QRA immediate and delayed ignition probabilities processing values of 0 and 1 in the GUI
- Ambient temperature input now retains its label when a saturated phase is selected
- Updated copyright year in source files


## [5.1.0] - 2023-12-04

### Added
- Added ability to output streamline and x-/y-distances to mole fractions to unignited plume
- Added carbon monoxide as a potential fuel in the Python backend

### Changed
- Moved python package metadata handling from setup.py/setup.cfg to pyproject.toml
- Updated fault tree images in GUI to specify "Extra Component 1" rather than "Component 1"
- Updated contour plotting functionality and moved code to phys/_plots.py
- Added validation test suite for hydrogen physics based on the SAND2021-5811 report
- Fixed a bug that was not allowing heat flux contours to be changed using the GUI
- Added error handling to CoolPropWrapper's PropsSI that estimates solutions for edge cases where CoolProp cannot find one directly
- Bauwens/Dorofeev unconfined overpressure model now calculates constant overpressure calculations very close to overpressure-origin to avoid division-by-zero
- Updated Jet and Flame to utilize premade DevelopingFlow objects to speed up calculation
- Updated QRA Analysis to use new Jet and Flame parameters
- Added test in test_qra_effects for premade DevelopingFlow usage
- Clarified back-end event tree calculations to simplify calculations, added corresponding tests
- Allow blank (null) entries for QRA leak frequency overrides
- Increase GUI size to 1366x768px
- Simplify GUI form layouts
- Add Shared State form encompassing shared parameters and fuel specifications
- Improve form parameter grid cell validation
- Add plot axis limit inputs for most Physics functionality
- Added flammability limits as optional input to the Physics API overpressure calculation
- Added flammable mass (detonable mass for Bauwens model) as an output to the Physics API overpressure calculation
- Removed logging from physics and QRA modules and updated output to command line
- Refine CoolPropWrapper PropsSI calls to general get_property function in phys/_therm.py
- Calculation of Planck mean absorption coefficient (used in radiant fraction calculation) is now automatic based on the fuel
- Changed default propane leak frequency distribution values based on results from SAND2023-05818 report
- Changed default component counts for all fuels, previously had zero (0) for all component types for all non-hydrogen fuels

### Removed
- Removed default random seed value for QRA in Python, so that a new value will be used for each run
- Removed optional Python input to override event tree specification in QRA, was previously limited in what could actually be specified


## [5.0.0] - 2022-11-11

### Added
- Added blends functionality to Physics analyses and GUI to support evaluation of fuel blends, including presets
- Added event_tree.py to provide flexible capability for building event trees; fault tree construction is now based on the order in which events are specified and only allows for two outcomes per event (accessible through Python backend)
- Added consequence.py to provide an abstraction for how event consequences were calculated and enable implementation of future consequences, which also absorbed material that was previously held in fatalities.py, making fatalities.py no longer needed (accessible through Python backend)
- Added test_qra_consequences.py to replace test_qra_fatalities.py due to incorporation of the fatality.py capabilities into consequence.py
- Added mass flow input for choked flows
- Added various plot formatting inputs to Physics analyses
- Added leak_frequency.py to place leak frequency calculation for the QRA analysis within its own module
- Added tests for support of leak frequencies beyond default values
- Added thermodynamic calculation subroutines so that blends are supported
- Added additional thermodynamic property values so that natural gas components are included
- Added support for keywords 'LFL' and 'UFL' within Jet.plot_moleFrac_Contour method
- Added keyword support for time constrained blowdown within Source.empty method
- Added ability to calculate steady-state releases for accumulation in GUI

### Changed
- Moved Engineering Toolkit analysis forms into Physics mode
- Revised GUI save/load functionality to utilize JSON format instead of binary; save files can now be modified directly via text editor
- Changed display units from Pa to kPa for peak overpressure contour plot in Physics Unconfined Overpressure analysis
- Changed tabular display units from Pa to kPa for impulse data in QRA results
- Changed Physics TNT Equivalency calculation to use Unconfined Overpressure TNT method calculation, rather than separate similar calculation
- Changed jet flame and unconfined overpressure methods to calculate distance to effect (heat flux, overpressure, impulse) in any direction (x, y, or z) rather than only x
- Changed construction of event tree within QRA Mode's analysis.py to now utilize event_tree.py instead of having the event tree hard coded; the default event tree implemented within analysis.py
- Changed calculation of consequences within QRA Mode's analysis.py to now utilize consequence.py, instead of having that functionality hard coded
- Changed risk calculations within QRA Mode's analysis.py to loop over the potential events and associated consequences in a generic fashion to enable use of alternative event trees in the future
- Changed leak size implementation to support any leak size. Interpolates between default leak sizes that were used during calibration
- Changed Physics Tank Mass calculation to calculate temperature, pressure, volume, or mass if given the other three inputs
- Moved overpressure/impulse data to _overpressure_data.py
- Simplified package data import in setup.cfg
- Updated Jet.plot_moleFrac_Contour method aesthetics (changed labeling of contours and colorbar orientation is dependent on plot aspect ratio)
- Updated Flame.plot_Ts method aesthetics (colorbar orientation is dependent on plot aspect ratio)
- Fixed bug that was causing notional nozzle calculations to be made for unchoked flows
- Changed thermodynamic Combustion calculations so that blends are supported
- Moved plotting subroutine of blowdown into Source object
- Fixed bugs in various modules to resolve errors with certain versions of Numpy and CoolProp
- Revised random seed for occupant locations to now generate a new value each time GUI is launched; this value is still editable by user
- Updated CNG component leak frequency estimates based on recent statistical effort using an updated data set
- Changed default BST Mach Flame Speed to 0.35
- Changed default Thermal Exposure Time in QRA to 30 seconds

### Removed
- Removed instances in which hydrogen was the default "species" argument for multiple functions/methods; the "species" is now a required input without a default
- Removed fatalities.py from QRA Mode as these capabilities were integrated into the new consequence.py
- Removed test_qra_fatalities.py due to being replaced by test_qra_consequences.py
- Removed overpressure and impulse data csv files, phys/data folder deleted


## [4.1.1] - 2022-08-24

### Added
- Added a setup.cfg file

### Changed
- Moved metadata handling from setup.py to setup.cfg
- Fixed a typo in CONTRIBUTING.md
- Updated metadata for standalone python package


## [4.1.0] - 2022-04-29

### Added
- Added basic automatic Python tests for QRA probits, positions, effects, and fatalities
- Added basic utility to create temporary folder in current working directory if one doesn't already exist
- Added functions to calculate and plot overpressure effects for positions in QRA
- Added function to calculate fatality probability based on overpressure effects for QRA
- Added visible flame length as output to the Physics mode flame analysis
- Added mass flow rate as output to Physics mode Plume Dispersion and Unconfined Overpressure analyses
- Added automatic tests to verify zero-risk cases for QRA calculations
- Added check for Physics mode Plume Dispersion mole fraction contour values that reject 0.0 or 1.0 as possible inputs
- Added ambient pressure to Python engineering toolkit mass flow rate inputs
- Added wind to Python flame analyses
- Added Python calculation of hydrogen peak overpressures using modified BST method from Jallais et al. (2018) https://doi.org/10.1002/prs.11965
- Added impulse and overpressure data to QRA results
- Added discharge coefficient input to flame and QRA analyses
- Added phase input to Engineering Toolkit temperature, pressure, density calculations
- ETK temperature-pressure-density tool now calculates second missing parameter when phase is saturated

### Changed
- Changed calculation of flow through an orifice to find maximum flux rather than relying on calculated speeds of sound
- Moved positions and heat flux effects from hyram.phys to hyram.qra and removed flux-analysis call from physics api
- Moved position generation to main QRA analysis instead of inside heat flux effects
- Moved calculation of thermal fatalities loop to separate fatalities module
- Changed QRA main analysis to calculate overpressure harm using unconfined overpressure model rather than overpressure/impulse direct inputs
- Changed QRA main analysis to utilize refactored code in new effects, fatalities, ignition probabilities, pipe size, and risk modules
- Changed internal calculations to use W/m2 for heat flux and Pa for peak overpressure for consistency; GUI and plots still report kW/m2 and kPa
- Figures are explicitly closed after saving to file to avoid taking up memory
- Changed the origin of unconfined overpressure blast wave to be the point at which the jet concentration is midway between the upper and lower flammability limits, instead of half the distance to the lower flammability limit
- Changed Bauwens/Dorofeev unconfined overpressure models to utilize fitted curves for detonation cell size rather than a saved spline-fit object in a pickle file
- Some Python optional arguments had default values that were specific to hydrogen (e.g., LFL and UFL), changed the default to select values specific to the fuel selected rather than hydrogen
- Fixed overpressure origin calculation, y- and z-coordinates were flipped
- Changed default overpressure probit model to TNO - Head Impact
- Changed ode integrator within Flame objects to use implicit Adams/BDF method rather than explicit Runge-Kutta method
- Improved accuracy of calculation of flammable mass within a jet/plume by interpolating the endpoints near the rich and lean regions
- Updated fault tree images in GUI to reflect added components in version 4.0

### Removed
- Removed unused debris fatality probit
- Removed numerical comparison tests for QRA C-API, the tests now just check that the interface is working and that calculations run without error
- Removed saving and loading of flame data object file in QRA calculations since old results could stay in there despite model changes
- Removed unused save and load object functions from physics thermo module and utilities
- Removed value-specific checks from Python C-API tests, these may be incorporated in separate future validation tests
- Removed single-point radiation model from jet flame entirely from the Python code
- Removed unused 3D ISO heat flux plot, along with associated scikit-image dependency
- Removed unused unconfined overpressure contour plot
- Removed package dill as a dependency of the hyram Python package
- Removed package pandas as a dependency of the hyram Python package
- Removed overpressure impulse table inputs from GUI
- Removed unused facility height input in QRA
- Removed unused leak height input in QRA calculations
- Removed Python 2-phase speed of sound calculations


## [4.0.0] - 2021-10-23

### Added
- Ability to select hydrogen, methane, or propane as the fuel
- Additional components in QRA mode
- Unconfined overpressure model
- Default leak frequencies for liquid hydrogen, methane (liquid and gas), and propane (liquid and gas)
- Fuel specific critical pressure check
- Mass flow rate graph for indoor accumulation model
- Python tests for QRA mode and unconfined overpressure models
- Scenario details in QRA results
- Positional heat flux table in QRA results
- Flame temperature/trajectory model output displays mass flow rate and emitted power

### Changed
- Name change to HyRAM+, along with new logo and icons
- Default leak frequencies for gaseous hydrogen updated based on more recent Bayesian analysis
- Overpressure physics model renamed to accumulation to differentiate from unconfined overpressure model
- Occupant location calculation in QRA mode
- TNT equivalent model in ETK looks up heat of combustion
- Fuel properties data table converted to pythonic dictionary
- Plot labels specific to hydrogen were changed to 'fuel'
- Sketch of accumulation scenario (without secondary area)
- Improved warning and error notifications
- Data directory now cleared on program launch
- Updated copyright

### Removed
- Release area input from accumulation model

### Fixed
- Suppressed interpolation bounds errors in indoor accumulation model


## [3.1.0] - 2021-05-11

### Added
- Jet _radial_profile method outputs temperatures
- Input label clarification that pressure units are absolute, not gauge
- Input checks in both GUI and Python to ensure valid probability distribution parameter values

### Changed
- New Windows installer, uses WiX Toolset
- Distributed Python updated to 3.9
- All distributed Python packages now installed as-is from PyPI except for CoolProp (now setup as a wheel)
- Distributed NumPy built with OpenBLAS (previously the Gohlke wheel used MKL)
- AppData HyRAM folder now cleared on application launch
- Separated overpressure layer plot onto stacked axes
- Isentropic expansion factor and other properties for overpressure calculations come from CoolProp instead of hard-coded heat capacity tables
- Made heat of combustion values consistent throughout code and with sources cited in Technical Reference Manual
- Clarified use of verbosity parameter for logging
- Unchoked flows can be simulated, with a warning in the results that the flowrate should be verified as realistic

### Removed
- Removed unused image files in GUI
- Removed unused layer model, unused functions in _jet.py, and commented code
- Radiative source model choice was removed (hidden) from GUI, will now always use multi-radiation source (not single)
- Removed "PSIG" unit as an option for pressure, this was previously no different than "PSI" and all pressure units are absolute
- Removed unused fuel properties from data file
- Removed unused constants module after changing usage to local variables or scipy.constants

### Fixed
- 2-phase speed of sound calculation corrected to use volume fraction instead of quality (which is mass fraction)
- Super-script in position plot colorbar label
- Position plot point coloring was not incorporating previous change
- Improved time resolution in overpressure plots
- Indoor Release plot now correctly handles max times beyond 30 seconds


## [3.0.1] - 2021-01-05

### Changed
- Removed value-specific checks from Python tests, these will be incorporated in separate validation tests
- Added black edge around markers and changed colormap to make heat flux position plots easier to read
- Added check so that GUI does not accept beta distribution parameters equal to 0

### Removed
- Removed currently unused validation tests, can add these back in once better numerical criteria are developed

### Fixed
- Fixed ability to save and load input parameters in .hyram files
- Corrected text labels for QRA mode output screen and column headers
- Updated two-phase flow speed of sound calculation to use method described by Chung, Park, Lee (2004) (doi:10.1016/j.jsv.2003.07.003)
- Updated orifice flow to use bounded pressure search algorithm to improve choked flow calculation


## [3.0] - 2020-09-30

### Added
- Added CHANGELOG
- Added CONTRIBUTING files for development instructions for main repository and Python-specific
- Added CoolProp as dependency for property calculations over wider range of conditions
- Added default impulse values to QRA overpressure consequence input table from Groth et al. (SAND2012-10150)
- Initial entrainment and heating zone added to jet/plume model; not needed before since hydrogen was never at low temperature
- Added user-selectable fluid phase option to most forms

### Changed
- Reorganized repository for clarity
- Moved development-specific instructions from README files to CONTRIBUTING files
- Now using geometric mean (median) rather than arithmetic mean for QRA calculations with lognormal distribution
- Updated QRA component leak frequency mu and sigma based on numerical fit of LaChance et al. (SAND2009-0874) data
- Made QRA overpressure consequence input table user-editable
- Abel-Noble equation of state has been replaced with CoolProp for property calculations
- Combustion code was modified to use CoolProp and expanded to prepare for future use of hydrocarbons
- Orifice flow calculations changed to use CoolProp for properties
- Orifice flow speed of sound calculations changed to use minor perturbation on pressure rather than CoolProp library (works along saturation curve)
- Notional Nozzle model inputs changed to specific temperature and momentum conservation options, rather than explicit model selection
- Notional Nozzle model calculations changed to use CoolProp for calculation of entropy
- Jet/plume flow establishment method now allows for <100% hydrogen due to initial entrainment and heating zone
- Jet/plume flow establishment method no longer uses [Li et al. (2016)](https://doi.org/10.1016/j.ijhydene.2015.10.071) model
- Jet/plume models modified to use fluid temperature rather than ambient temperature for centerline density
- Momentum-driven entrainment based on modified plug flow model rather than conditions at orifice
- Jet/plume model energy equation now solved; previously model assumed everything was at ambient temperature
- Tank blowdown model changed to use CoolProp properties
- Tank blowdown model changed to use numerical solver rather than equal time steps
- Physics models with numerical solvers (jet, flame, blowdown, layer) all use adaptive spatial- and time-steps
- Cleanup/streamlining of indoor release model code
- Flame model now uses same developing flow regions as jet/plume model, to allow for zone of initial entrainment and heating and remove duplication in both Flame and Jet models
- Modified Python tests for C-API and some validation tests
- Moved physics "wrapper" scripts to API and C-API modules directly
- Leak release height in QRA now occurs at (0,0,0) rather than (0,1,0), so that all (x,y,z) dimensions are relative to leak point
- Default Occupant y-coordinate positions are now constant value of 0, so on same horizontal plane as leak point (previously were at 1 m to match leak point height)

### Removed
- Removed unused `_indoor_release2.py` module
- Removed Harstad-Bellan as a notional nozzle model option, since this was not fully implemented yet
- Removed Jupyter notebooks used for development
- Removed QRA-specific Python test directory; now all tests are in main tests directory
- Removed unused C# projects and library code

### Fixed
- Added version number to `setup.py` file so that Python package installation runs without error
- Modified QRA event sequence diagram and equations so that ignition probabilities are used correctly


## [2.0.0] - 2019-06-28
Initial open-source release

### Added
- Added open source licensing files
- Added Extra Component #1 and #2 to QRA mode calculations
- Added ability to override fault tree leak frequency results to allow for more flexible QRA calculations
- Added python API for GUI and programmatic access to HyRAM toolkit
- Added automated python tests and unit tests

### Changed
- Made QRA dispenser failures user-editable
- Corrected QRA HSE - Lung Hemorrhage probit values for input units
- Converted QRA calculations to Python from C#
- Used [Li et al. (2016)](https://doi.org/10.1016/j.ijhydene.2015.10.071) model for zone of flow establishment

### Removed
- Removed licensing code since now open source

### Fixed
- Pipe flow area correctly calculated from outer diameter and pipe thickness
