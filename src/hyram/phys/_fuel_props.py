"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

from CoolProp.CoolProp import AbstractState
import numpy as np
from ..utilities import misc_utils


class FuelProperties:
    """
    Fuel Properties

    Requires any fuel with CoolProp chemical formula or 'other_fluid' in the _data dictionary below

    Currently Available fluids:
        H2, CH4, C3H8, N2, CO2, CO, C2H6, C4H10, C5H12, C6H16

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
                      'other_name': 'n-Propane'
                     },
             'N2': {'LFL': np.nan,
                    'UFL': np.nan,
                    'dHc': 0,
                    'other_name': 'nitrogen'
                    },
             'CarbonDioxide': {'LFL': np.nan,
                     'UFL': np.nan,
                     'dHc': 0,
                     'other_name': 'CO2'
                     },
             'CarbonMonoxide': {'LFL': 0.125,
                     'UFL': 0.74,
                     'dHc': 1.01e7,
                     'other_name': 'CO'
                     },
             'C2H6': {'LFL': 0.03,
                      'UFL': .124,
                      'dHc': 4.75e7,
                      'other_name': 'ethane'
                      },
             'n-Butane': {'LFL': 0.018,
                          'UFL': .084,
                          'dHc': 4.57e7,
                          'other_name': 'n-butane'
                          },
             'IsoButane': {'LFL': 0.018,
                           'UFL': .084,
                           'dHc': 4.57e7,
                           'other_name': 'isobutane'
                           },
             'n-Pentane': {'LFL': 0.014,
                           'UFL': .078,
                           'dHc': 4.51e7,
                           'other_name': 'n-pentane'
                           },
             'IsoPentane': {'LFL': 0.014,
                            'UFL': .078,
                            'dHc': 4.51e7,
                            'other_name': 'isopentane'
                            },
             'n-Hexane': {'LFL': 0.012,
                           'UFL': .074,
                           'dHc': 4.48e7,
                           'other_name': 'n-hexane'
                           },
            }

    def __init__(self, species):
        if not misc_utils.is_parsed_blend_string(species):
            species = species.upper()
            if species not in list(self._data.keys()):
                if species.lower() not in [fuel['other_name'].lower() for fuel in self._data.values()]:
                    raise ValueError('unknown properties for %s' % species)
                else:
                    species = [k for k, v in self._data.items() if v['other_name'].lower() == species.lower()][0]
            self.species = species
            for k in ['LFL', 'UFL', 'dHc']:
                self.__dict__[k] = self._data[species][k]
            self.nC, self.nH, self.nO = self._count_atoms(species)
        else:
            self.species = species
            specnames = [s.split('[')[0] for s in species.split('&')]
            molefracs = [float(s.split('[')[1][:-1]) for s in species.split('&')]
            eos = AbstractState('HEOS', '&'.join(specnames))
            eos.set_mole_fractions(molefracs)
            massfracs = eos.get_mass_fractions()
            # heat of combustion and nC weighted by mass/mole fraction
            # LFL and UFL found using Le Chatelier's formula (https://doi.org/10.1016/j.jhazmat.2007.11.085)
            self.dHc = 0; self.nC, self.nH, self.nO = 0, 0, 0; invLFL = 0; invUFL = 0; XtotLFL = 0; XtotUFL = 0;
            for species, X, Y in zip(specnames, molefracs, massfracs):
                if species.upper() not in [fuel.upper() for fuel in self._data.keys()]:
                    if species.lower() not in [fuel['other_name'].lower() for fuel in self._data.values()]:
                        raise ValueError('unknown properties for %s' % species)
                    else:
                        species = [k for k, v in self._data.items() if v['other_name'].lower() == species.lower()][0]
                else:
                    species = [k for k in self._data.keys() if k.lower() == species.lower()][0]
                self.dHc += self._data[species]['dHc']*Y
                count = FuelProperties._count_atoms(species)
                self.nC += count[0]*X; self.nH += count[1]*X; self.nO += count[2]*X
                if not np.isnan(self._data[species]['LFL']):
                    invLFL += X/self._data[species]['LFL']
                    XtotLFL += X
                if not np.isnan(self._data[species]['UFL']):
                    invUFL += X/self._data[species]['UFL']
                    XtotUFL += X
            self.LFL, self.UFL = XtotLFL/invLFL, XtotUFL/invUFL

    @staticmethod
    def _count_atoms(species):
        if   'hydrogen' in species.lower() or species == 'H2': nC, nH, nO = 0, 2, 0
        elif 'methane'  in species.lower() or species == 'CH4': nC, nH, nO = 1, 4, 0
        elif 'ethane'   in species.lower() or species == 'C2H6': nC, nH, nO = 2, 6, 0
        elif 'propane'  in species.lower() or species == 'C3H8': nC, nH, nO = 3, 8, 0
        elif 'butane'   in species.lower(): nC, nH, nO = 4, 10, 0
        elif 'pentane'  in species.lower(): nC, nH, nO = 5, 12, 0
        elif 'hexane'   in species.lower(): nC, nH, nO = 6, 14, 0
        elif 'carbondioxide' in species.lower(): nC, nH, nO = 1, 0, 2
        elif 'carbonmonoxide' in species.lower(): nC, nH, nO = 1, 0, 1
        elif 'nitrogen' in species.lower() or species == 'N2': nC, nH, nO = 0, 0, 0
        return nC, nH, nO

    def __str__(self):
        return f'{self.species}\n------------\nLFL: {self.LFL}\nUFL: {self.UFL}\nheat of combustion: {self.dHc*1e-6:.1f} MJ/kg'

    def __repr__(self):
        return self.__str__()
