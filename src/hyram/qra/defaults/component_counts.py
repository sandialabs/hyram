"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


h2_gas_component_counts = {
    'compressor': 1,
    'vessel': 2,
    'filter': 3,
    'hose': 1,
    'joint': 43,
    'pipe': 30,
    'valve': 7,
    'instrument': 5
}


h2_liquid_component_counts = {
    'vessel': 1,
    'flange': 8,
    'hose': 1,
    'pipe': 30,
    'valve': 44
}


ch4_gas_component_counts = {
    'compressor': 1,
    'vessel': 2,
    'filter': 3,
    'hose': 1,
    'joint': 43,
    'pipe': 30,
    'valve': 7,
    'instrument': 5,
    'exchanger': 1
}


ch4_liquid_component_counts = {
    'vessel': 1,
    'flange': 8,
    'hose': 1,
    'pipe': 30,
    'valve': 44,
    'exchanger': 1
}


c3h8_gas_component_counts = {
    'compressor': 1,
    'vessel': 1,
    'filter': 2,
    'flange': 8,
    'hose': 1,
    'pipe': 30,
    'valve': 44
}


c3h8_liquid_component_counts = c3h8_gas_component_counts


def get_component_counts_for_species(species, saturated_phase=None):
    """Returns a dictionary of the component categories and their respective
    quantities given a species and saturated phase.
    """
    species = species.lower()

    if species in ['h2', 'hydrogen']:
        counts = (
            h2_liquid_component_counts if saturated_phase in ['gas', 'liquid']
            else h2_gas_component_counts)

    elif species in ['ch4', 'methane']:
        counts = (
            ch4_liquid_component_counts if saturated_phase in ['gas', 'liquid']
            else ch4_gas_component_counts)

    elif species in ['c3h8', 'propane']:
        counts = (
            c3h8_liquid_component_counts if saturated_phase in ['gas', 'liquid']
            else c3h8_gas_component_counts)

    else:
        raise ValueError('Species not recognized. Valid entires are ' +
                         "'hydrogen', 'methane', or 'propane'.")

    return counts


def get_component_count(component, species, saturated_phase=None):
    component_counts = get_component_counts_for_species(
        species, saturated_phase)
    try:
        component_count = component_counts[component]
    except KeyError:
        raise KeyError(f'{component} is not included in default component' +
                       f"counts for '{species}' species and " +
                       f"'{saturated_phase}' saturated phase.")
    return component_count
