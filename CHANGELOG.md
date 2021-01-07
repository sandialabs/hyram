# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
- Orifice flow speed of sound calculations changed to use minor perterbation on pressure rather than CoolProp library (works along saturation curve)
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
- Flame model now uses same developing flow regions as jet/plume model, to allow for zone of initial entrianment and heating and remove duplication in both Flame and Jet models
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
- Modified QRA event sequence diagram and equations so that ignition probabilites are used correctly


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
