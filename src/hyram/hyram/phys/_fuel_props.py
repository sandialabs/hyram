"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""


class Fuel_Properties:
    """
    Fuel Properties

    Requires any fuel with CoolProp chemical formula or 'other_fluid' in the _data dictionary below

    Currently Available fluids: 
        H2, CH4, C3H8

    Required Fields:
        LFL: Lower Flammability Limit, mole/volume fraction
        UFL: Upper Flammability Limit, mole/volume fraction
        dHc: Heat of Combustion, J/kg
        other_name: Full Name of Fuel, string
    """
    _data = {'H2': {'LFL': 0.04,
                    'UFL': 0.75,
                    'dHc': 1.20E+08,
                    'other_name': 'hydrogen'
                   },
             'CH4': {'LFL': 0.05,
                     'UFL': 0.15,
                     'dHc': 5.00E+07,
                     'other_name': 'methane'
                    },
             'C3H8': {'LFL': 0.021,
                      'UFL': 0.095,
                      'dHc': 4.64E+07,
                      'other_name': 'propane'
                     }
            }

    def __init__(self, species):
        species = species.upper()
        if species not in list(self._data.keys()):
            if species.lower() not in [fuel['other_name'].lower() for fuel in self._data.values()]:
                raise ValueError('unknown properties for %s' % species)
            else:
                species = [k for k, v in self._data.items() if v['other_name'].lower() == species.lower()][0]
        self.species = species
        for k in ['LFL', 'UFL', 'dHc']:
            self.__dict__[k] = self._data[species][k]
        if 'C' in species:
            if species.split('C')[-1][0].isdecimal():
                self.nC = int(species.split('C')[-1][0])
            else:
                self.nC = 1
        else:
            self.nC = 0
