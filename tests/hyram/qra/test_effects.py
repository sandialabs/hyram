"""
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os
import unittest

import numpy as np

from hyram.qra import effects
import hyram.phys.api as phys_api
from hyram.phys._jet import DevelopingFlow
from hyram.phys._therm import Combustion
from hyram.phys import Orifice
from hyram.utilities import misc_utils


class TestThermalEffects(unittest.TestCase):
    """
    Test calculation and plotting of thermal effects
    """
    def setUp(self):
        self.amb_fluid = phys_api.create_fluid('AIR',
                                               temp=288,  # K
                                               pres=101325)  # Pa
        self.rel_fluid = phys_api.create_fluid('H2',
                                               temp=288,  # K
                                               pres=35e6,  # Pa
                                               phase='none')
        self.rel_angle = 0  # radians
        self.site_length = 20  # m
        self.site_width = 12  # m
        leak_diams = [0.001, 0.003]  # m
        self.orifices = [Orifice(leak_diam) for leak_diam in leak_diams]
        self.rel_humid = 0.89
        self.not_nozzle_model = 'yuce'
        cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(self.not_nozzle_model, self.rel_fluid)
        self.developing_flows = [DevelopingFlow(self.rel_fluid, o, self.amb_fluid,
                                                theta0 = self.rel_angle,
                                                nn_conserve_momentum=cons_momentum, nn_T=notional_noz_t,
                                                verbose=False) for o in self.orifices]
        self.chem = Combustion(self.rel_fluid)
        self.locations = [(5, 0, 1), (6, 1, 2), (7, 0, 2)]  # m
        self.create_plots = True
        self.output_dir = misc_utils.get_temp_folder()
        self.verbose=False

    def test_calc_thermal_effects(self):
        flux_dict = effects.calc_thermal_effects(self.amb_fluid,
                                                 self.rel_fluid,
                                                 self.rel_angle,
                                                 self.site_length,
                                                 self.site_width,
                                                 self.orifices,
                                                 self.rel_humid,
                                                 self.not_nozzle_model,
                                                 self.locations,
                                                 self.developing_flows,
                                                 self.chem,
                                                 self.create_plots,
                                                 self.output_dir,
                                                 self.verbose)
        self.assertGreater(max(flux_dict['fluxes']), 0)
        self.assertGreater(len(flux_dict['all_pos_files']), 0)

        # ensure that specifying a developing flow performs the same as not
        flux_dict_no_develop_flow = \
            effects.calc_thermal_effects(amb_fluid=self.amb_fluid,
                                         rel_fluid=self.rel_fluid,
                                         rel_angle=self.rel_angle,
                                         site_length=self.site_length,
                                         site_width=self.site_width,
                                         orifices=self.orifices,
                                         rel_humid=self.rel_humid,
                                         not_nozzle_model=self.not_nozzle_model,
                                         locations=self.locations,
                                         developing_flows=None,
                                         chem=None,
                                         create_plots=self.create_plots,
                                         output_dir=self.output_dir,
                                         verbose=self.verbose)
        self.assertListEqual(flux_dict['fluxes'].tolist(), flux_dict_no_develop_flow['fluxes'].tolist())

    def test_zero_occupants(self):
        locations = []
        flux_dict = effects.calc_thermal_effects(self.amb_fluid,
                                                 self.rel_fluid,
                                                 self.rel_angle,
                                                 self.site_length,
                                                 self.site_width,
                                                 self.orifices,
                                                 self.rel_humid,
                                                 self.not_nozzle_model,
                                                 locations,
                                                 self.developing_flows,
                                                 self.chem,
                                                 self.create_plots,
                                                 self.output_dir,
                                                 self.verbose)
        self.assertEqual(len(flux_dict['fluxes']), 0)


class TestOverpressureEffects(unittest.TestCase):
    """
    Test calculation and plotting of overpressure effects
    """
    def setUp(self):
        leak_diams = [0.001, 0.003]  # m
        self.orifices = [Orifice(leak_diam) for leak_diam in leak_diams]
        self.notional_nozzle_model = 'yuce'
        self.release_fluid = phys_api.create_fluid('H2',
                                                   temp=288,  # K
                                                   pres=35e6,  # Pa
                                                   phase='none')
        self.ambient_fluid = phys_api.create_fluid('AIR',
                                                   temp=288,  # K
                                                   pres=101325)  # Pa
        self.release_angle = 0  # radians
        cons_momentum, notional_noz_t = misc_utils.convert_nozzle_model_to_params(self.notional_nozzle_model, self.release_fluid)
        self.developing_flows = [DevelopingFlow(self.release_fluid, o, self.ambient_fluid,
                                                theta0 = self.release_angle,
                                                nn_conserve_momentum=cons_momentum, nn_T=notional_noz_t,
                                                verbose=False) for o in self.orifices]
        self.overp_method = 'bst'
        self.locations = [(5, 0, 1), (6, 1, 2), (7, 0, 2)]  # m
        self.site_length = 20  # m
        self.site_width = 12  # m
        self.BST_mach_flame_speed = 0.35
        self.TNT_equivalence_factor = 0.03
        self.create_plots = True
        self.output_dir = None
        self.verbose = False

    def test_calc_overp_effects_TNT(self):
        overp_dict = effects.calc_overp_effects(self.orifices,
                                                self.notional_nozzle_model,
                                                self.release_fluid,
                                                self.ambient_fluid,
                                                self.release_angle,
                                                self.locations,
                                                self.site_length,
                                                self.site_width,
                                                'tnt',
                                                self.BST_mach_flame_speed,
                                                self.TNT_equivalence_factor,
                                                self.developing_flows,
                                                self.create_plots,
                                                self.output_dir,
                                                self.verbose)
        self.assertGreater(max(overp_dict['overpressures']), 0)
        self.assertGreater(max(overp_dict['impulses']), 0)
        self.assertGreater(len(overp_dict['all_pos_overp_files']), 0)
        self.assertGreater(len(overp_dict['all_pos_impulse_files']), 0)

        # ensure that specifying a developing flow performs the same as not
        overp_dict_no_develop_flow = \
            effects.calc_overp_effects(orifices=self.orifices,
                                       notional_nozzle_model=self.notional_nozzle_model,
                                       release_fluid=self.release_fluid,
                                       ambient_fluid=self.ambient_fluid,
                                       release_angle=self.release_angle,
                                       locations=self.locations,
                                       site_length=self.site_length,
                                       site_width=self.site_width,
                                       overp_method='tnt',
                                       BST_mach_flame_speed=self.BST_mach_flame_speed,
                                       TNT_equivalence_factor=self.TNT_equivalence_factor,
                                       developing_flows=None,
                                       create_plots=self.create_plots,
                                       output_dir=self.output_dir,
                                       verbose=self.verbose)
        self.assertListEqual(overp_dict['overpressures'].tolist(),
                             overp_dict_no_develop_flow['overpressures'].tolist())


    def test_calc_overp_effects_BST(self):
        overp_dict = effects.calc_overp_effects(self.orifices,
                                                self.notional_nozzle_model,
                                                self.release_fluid,
                                                self.ambient_fluid,
                                                self.release_angle,
                                                self.locations,
                                                self.site_length,
                                                self.site_width,
                                                'bst',
                                                self.BST_mach_flame_speed,
                                                self.TNT_equivalence_factor,
                                                self.developing_flows,
                                                self.create_plots,
                                                self.output_dir,
                                                self.verbose)
        self.assertGreater(max(overp_dict['overpressures']), 0)
        self.assertGreater(max(overp_dict['impulses']), 0)
        self.assertGreater(len(overp_dict['all_pos_overp_files']), 0)
        self.assertGreater(len(overp_dict['all_pos_impulse_files']), 0)

    def test_calc_overp_effects_Bauwens(self):
        overp_dict = effects.calc_overp_effects(self.orifices,
                                                self.notional_nozzle_model,
                                                self.release_fluid,
                                                self.ambient_fluid,
                                                self.release_angle,
                                                self.locations,
                                                self.site_length,
                                                self.site_width,
                                                'bauwens',
                                                self.BST_mach_flame_speed,
                                                self.TNT_equivalence_factor,
                                                self.developing_flows,
                                                self.create_plots,
                                                self.output_dir,
                                                self.verbose)
        self.assertGreater(max(overp_dict['overpressures']), 0)
        self.assertTrue(np.all(np.isnan(overp_dict['impulses'])))
        self.assertGreater(len(overp_dict['all_pos_overp_files']), 0)
        self.assertEqual(len(overp_dict['all_pos_impulse_files']), 0)

    def test_zero_occupants(self):
        locations = []
        overp_dict = effects.calc_overp_effects(self.orifices,
                                                self.notional_nozzle_model,
                                                self.release_fluid,
                                                self.ambient_fluid,
                                                self.release_angle,
                                                locations,
                                                self.site_length,
                                                self.site_width,
                                                self.overp_method,
                                                self.BST_mach_flame_speed,
                                                self.TNT_equivalence_factor,
                                                self.developing_flows,
                                                self.create_plots,
                                                self.output_dir,
                                                self.verbose)
        self.assertEqual(len(overp_dict['overpressures']), 0)
        self.assertEqual(len(overp_dict['impulses']), 0)


class TestEffectPlots(unittest.TestCase):
    """
    Test plotting of thermal and overpressure effects
    """
    def test_plot_effect_positions(self):
        test_effects = [0.06, 0.01]
        effect_label = 'Test Effect [units]'
        output_dir = misc_utils.get_temp_folder()
        filename = 'test_effect_positions.png'
        filepathname = os.path.join(output_dir, filename)
        title = 'Test Effect Positions Plot'
        x_locations = [1, 5]
        z_locations = [1, 2]
        length = 20
        width = 12
        effects.plot_effect_positions(test_effects,
                                      effect_label,
                                      filepathname,
                                      title,
                                      x_locations,
                                      z_locations,
                                      length,
                                      width)
        self.assertTrue(os.path.isfile(filepathname))
