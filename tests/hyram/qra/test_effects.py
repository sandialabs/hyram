"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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

OUTPUT_DIR = 'out'


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
        self.output_dir = OUTPUT_DIR
        self.verbose=False

    def test_calc_thermal_effects(self):
        for idx in range(len(self.orifices)):
            fluxes, plot_filepath = effects.calc_thermal_effects(self.amb_fluid,
                                                                 self.rel_fluid,
                                                                 self.rel_angle,
                                                                 self.orifices[idx],
                                                                 self.rel_humid,
                                                                 self.not_nozzle_model,
                                                                 self.locations,
                                                                 idx,
                                                                 self.developing_flows[idx],
                                                                 self.chem,
                                                                 self.create_plots,
                                                                 self.output_dir,
                                                                 self.verbose)
            self.assertGreater(max(fluxes), 0)
            self.assertGreater(len(plot_filepath), 0)

            # ensure that specifying a developing flow performs the same as not
            no_developing_flow_fluxes, _ = \
                effects.calc_thermal_effects(ambient_fluid=self.amb_fluid,
                                             release_fluid=self.rel_fluid,
                                             release_angle=self.rel_angle,
                                             orifice=self.orifices[idx],
                                             rel_humid=self.rel_humid,
                                             notional_nozzle_model=self.not_nozzle_model,
                                             locations=self.locations,
                                             leak_idx=idx,
                                             developing_flow=None,
                                             chem=None,
                                             create_plots=self.create_plots,
                                             output_dir=self.output_dir,
                                             verbose=self.verbose)
            self.assertListEqual(fluxes.tolist(), no_developing_flow_fluxes.tolist())

    def test_zero_occupants(self):
        locations = []
        fluxes, _ = effects.calc_thermal_effects(self.amb_fluid,
                                                 self.rel_fluid,
                                                 self.rel_angle,
                                                 self.orifices[0],
                                                 self.rel_humid,
                                                 self.not_nozzle_model,
                                                 locations,
                                                 0,
                                                 self.developing_flows[0],
                                                 self.chem,
                                                 self.create_plots,
                                                 self.output_dir,
                                                 self.verbose)
        self.assertEqual(len(fluxes), 0)


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
        self.BST_mach_flame_speed = 0.35
        self.TNT_equivalence_factor = 0.03
        self.create_plots = True
        self.output_dir = OUTPUT_DIR
        self.verbose = False

    def test_calc_overp_effects_TNT(self):
        for idx in range(len(self.orifices)):
            (overpressures,
             impulses,
             overpressure_plot_filepath,
             impulse_plot_filepath) = effects.calc_overp_effects(self.orifices[idx],
                                                                 self.notional_nozzle_model,
                                                                 self.release_fluid,
                                                                 self.ambient_fluid,
                                                                 self.release_angle,
                                                                 self.locations,
                                                                 'tnt',
                                                                 idx,
                                                                 self.BST_mach_flame_speed,
                                                                 self.TNT_equivalence_factor,
                                                                 self.developing_flows[idx],
                                                                 self.create_plots,
                                                                 self.output_dir,
                                                                 self.verbose)
            self.assertGreater(max(overpressures), 0)
            self.assertGreater(max(impulses), 0)
            self.assertGreater(len(overpressure_plot_filepath), 0)
            self.assertGreater(len(impulse_plot_filepath), 0)

            # ensure that specifying a developing flow performs the same as not
            no_developing_flow_overpressures, _, _, _ = \
                effects.calc_overp_effects(orifice=self.orifices[idx],
                                           notional_nozzle_model=self.notional_nozzle_model,
                                           release_fluid=self.release_fluid,
                                           ambient_fluid=self.ambient_fluid,
                                           release_angle=self.release_angle,
                                           locations=self.locations,
                                           overp_method='tnt',
                                           leak_idx=idx,
                                           BST_mach_flame_speed=self.BST_mach_flame_speed,
                                           TNT_equivalence_factor=self.TNT_equivalence_factor,
                                           developing_flow=None,
                                           create_plots=self.create_plots,
                                           output_dir=self.output_dir,
                                           verbose=self.verbose)

            self.assertEqual(overpressures.tolist(), no_developing_flow_overpressures.tolist())


    def test_calc_overp_effects_BST(self):
        for idx in range(len(self.orifices)):
            (overpressures,
             impulses,
             overpressure_plot_filepath,
             impulse_plot_filepath) = effects.calc_overp_effects(self.orifices[idx],
                                                                 self.notional_nozzle_model,
                                                                 self.release_fluid,
                                                                 self.ambient_fluid,
                                                                 self.release_angle,
                                                                 self.locations,
                                                                 'bst',
                                                                 idx,
                                                                 self.BST_mach_flame_speed,
                                                                 self.TNT_equivalence_factor,
                                                                 self.developing_flows[idx],
                                                                 self.create_plots,
                                                                 self.output_dir,
                                                                 self.verbose)
            self.assertGreater(max(overpressures), 0)
            self.assertGreater(max(impulses), 0)
            self.assertGreater(len(overpressure_plot_filepath), 0)
            self.assertGreater(len(impulse_plot_filepath), 0)

    def test_calc_overp_effects_Bauwens(self):
        all_overpressures = []
        all_impulses = []
        for idx in range(len(self.orifices)):
            (overpressures,
             impulses,
             overpressure_plot_filepath,
             impulse_plot_filepath) = effects.calc_overp_effects(self.orifices[idx],
                                                                 self.notional_nozzle_model,
                                                                 self.release_fluid,
                                                                 self.ambient_fluid,
                                                                 self.release_angle,
                                                                 self.locations,
                                                                 'bauwens',
                                                                 idx,
                                                                 self.BST_mach_flame_speed,
                                                                 self.TNT_equivalence_factor,
                                                                 self.developing_flows[idx],
                                                                 self.create_plots,
                                                                 self.output_dir,
                                                                 self.verbose)
            all_overpressures.extend(overpressures)
            all_impulses.extend(impulses)
            self.assertGreater(len(overpressure_plot_filepath), 0)
            self.assertEqual(len(impulse_plot_filepath), 0)

        self.assertGreater(max(all_overpressures), 0)
        self.assertTrue(np.all(np.isnan(all_impulses)))

    def test_zero_occupants(self):
        locations = []
        overpressures, impulses, _, _ = effects.calc_overp_effects(self.orifices[0],
                                                                   self.notional_nozzle_model,
                                                                   self.release_fluid,
                                                                   self.ambient_fluid,
                                                                   self.release_angle,
                                                                   locations,
                                                                   self.overp_method,
                                                                   0,
                                                                   self.BST_mach_flame_speed,
                                                                   self.TNT_equivalence_factor,
                                                                   self.developing_flows[0],
                                                                   self.create_plots,
                                                                   self.output_dir,
                                                                   self.verbose)
        self.assertEqual(len(overpressures), 0)
        self.assertEqual(len(impulses), 0)


class TestEffectPlots(unittest.TestCase):
    """
    Test plotting of thermal and overpressure effects
    """
    def test_plot_effect_positions(self):
        test_effects = [0.06, 0.01]
        effect_label = 'Test Effect [units]'
        output_dir = OUTPUT_DIR
        filename = 'test_effect_positions.png'
        filepathname = os.path.join(output_dir, filename)
        title = 'Test Effect Positions Plot'
        x_locations = [1, 5]
        z_locations = [1, 2]
        effects.plot_effect_positions(test_effects,
                                      effect_label,
                                      filepathname,
                                      title,
                                      x_locations,
                                      z_locations)
        self.assertTrue(os.path.isfile(filepathname))
