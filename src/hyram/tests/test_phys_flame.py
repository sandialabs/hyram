"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

from hyram.phys import _flame
import hyram.phys.api as phys_api
import hyram.phys._comps as phys_comps


VERBOSE = False


class TestAtmosphericTransmissivity(unittest.TestCase):
    """
    Test calculation of atmospheric transmissivity
    """
    def test_against_values_from_paper(self):
        path_lengths = [
            10, 100, 1000,
            10, 100, 1000,
            10, 100, 1000,
            10, 100, 1000
        ]  # m
        temperatures = [
            253, 253, 253,
            273, 273, 273,
            293, 293, 293,
            303, 303, 303
        ]  # K
        relative_humidities = [
            0.5, 0.5, 0.5,
            0.5, 0.5, 0.5,
            0.5, 0.5, 0.5,
            0.5, 0.5, 0.5
        ]
        calc_taus = []
        for path_length, amb_temp, rel_humid in zip(path_lengths,
                                                    temperatures,
                                                    relative_humidities):
            tau = _flame.calc_transmissivity(path_length, amb_temp, rel_humid)
            calc_taus.append(tau)
        taus_from_paper = [
            0.96, 0.86, 0.72,
            0.91, 0.78, 0.61,
            0.86, 0.71, 0.51,
            0.84, 0.67, 0.47
        ]
        for calc_tau, paper_tau in zip(calc_taus, taus_from_paper):
            tau_diff_pct = (calc_tau - paper_tau) / paper_tau * 100
            self.assertLessEqual(tau_diff_pct, 0.6)


class TestFlameObject(unittest.TestCase):
    """
    Tests of Flame class
    """
    def setUp(self):
        release_fluid = phys_api.create_fluid('H2',
                                              temp=288,  # K
                                              pres=35e6,  # Pa
                                              phase='none')
        ambient_fluid = phys_api.create_fluid('AIR',
                                              temp=288,  # K
                                              pres=101325)  # Pa
        leak_diam = 0.003  # m
        orifice = phys_comps.Orifice(leak_diam)
        flame = _flame.Flame(release_fluid,
                             orifice,
                             ambient_fluid,
                             verbose=VERBOSE)
        self.flame = flame

    def test_zero_occupants_positional_flux(self):
        locations = []
        rel_humid = 0.5
        fluxes = self.flame.generate_positional_flux(locations, rel_humid)
        self.assertEqual(len(fluxes), 0)


if __name__ == "__main__":
    unittest.main()
