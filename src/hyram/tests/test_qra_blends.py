"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import unittest

import scipy.constants as spc

from hyram.qra import analysis as qra_analysis
from hyram.qra import component_failure
from hyram.qra.component_set import ComponentSet
from hyram.qra.data import component_data

VERBOSE = False

class QraBlendsTestCase(unittest.TestCase):
    """
    Test QRA with blends.
    """

    def setUp(self):
        rel_species = {"ch4": 0.96, 'n2': 0.04}
        rel_phase = "None"
        leak_sizes = None

        num_compressors = 0
        num_vessels = 0
        num_valves = 5
        num_instruments = 3
        num_joints = 35
        num_hoses = 1
        num_pipes = 20  # m
        num_filters = 0
        num_flanges = 0
        num_heat_exchangers = 0
        num_vaporizers = 0
        num_transfer_arms = 0
        num_extra1s = 0
        num_extra2s = 0

        component_sets = [
            ComponentSet('compressor', num_compressors, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['compressor'],
                         leak_sizes=leak_sizes),
            ComponentSet('vessel', num_vessels, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['vessel'],
                         leak_sizes=leak_sizes),
            ComponentSet('valve', num_valves, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['valve'],
                         leak_sizes=leak_sizes),
            ComponentSet('instrument', num_instruments, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['instrument'],
                         leak_sizes=leak_sizes),
            ComponentSet('joint', num_joints, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['joint'],
                         leak_sizes=leak_sizes),
            ComponentSet('hose', num_hoses, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['hose'],
                         leak_sizes=leak_sizes),
            ComponentSet('pipe', num_pipes, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['pipe'],
                         leak_sizes=leak_sizes),
            ComponentSet('filter', num_filters, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['filter'],
                         leak_sizes=leak_sizes),
            ComponentSet('flange', num_flanges, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['flange'],
                         leak_sizes=leak_sizes),
            ComponentSet('exchanger', num_heat_exchangers, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['exchanger'],
                         leak_sizes=leak_sizes),
            ComponentSet('vaporizer', num_vaporizers, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['vaporizer'],
                         leak_sizes=leak_sizes),
            ComponentSet('arm', num_transfer_arms, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['arm'],
                         leak_sizes=leak_sizes),
            ComponentSet('extra1', num_extra1s, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['extra1'],
                         leak_sizes=leak_sizes),
            ComponentSet('extra2', num_extra2s, species=rel_species, saturated_phase=rel_phase,
                         leak_frequency_lists=component_data.h2_gas_params['extra2'],
                         leak_sizes=leak_sizes)
        ]

        component_failure_set = component_failure.ComponentFailureSet(
                f_failure_override=None, num_vehicles=20, daily_fuelings=2, vehicle_days=250,
                noz_po_dist='beta', noz_po_a=0.5, noz_po_b=610415.5,
                noz_ftc_dist='expv', noz_ftc_a=0.002, noz_ftc_b=None,
                mvalve_ftc_dist='expv', mvalve_ftc_a=0.001, mvalve_ftc_b=None,
                svalve_ftc_dist='expv', svalve_ftc_a=0.002, svalve_ftc_b=None,
                svalve_ccf_dist='expv', svalve_ccf_a=0.000128, svalve_ccf_b=None,
                overp_dist='beta', overp_a=3.5, overp_b=310289.5,
                pvalve_fto_dist='logn', pvalve_fto_a=-11.74, pvalve_fto_b=0.67,
                driveoff_dist='beta', driveoff_a=31.5, driveoff_b=610384.5,
                coupling_ftc_dist='beta', coupling_ftc_a=0.5, coupling_ftc_b=5031)

        self.inputs = {
            "pipe_outer_diam": 0.375 * spc.inch,
            "pipe_thickness": 0.065 * spc.inch,
            "amb_temp": 15 + spc.zero_Celsius,
            "amb_pres": 101325,
            "rel_temp": 15 + spc.zero_Celsius,
            "rel_pres": 35 * spc.mega,
            "rel_phase": None,
            "facil_length": 20,  # m
            "facil_width": 12,  # m
            "immed_ign_probs": [0.008, 0.053, 0.23],
            "delayed_ign_probs": [0.004, 0.027, 0.12],
            "ign_thresholds": [0.125, 6.25],

            "occupant_input_list": [{
                'count': 9,
                'descrip': 'workers',
                'xdistr': 'uniform', 'xa': 1, 'xb': 20,
                'ydistr': 'dete', 'ya': 1, 'yb': None,
                'zdistr': 'unif', 'za': 1, 'zb': 12,
                'hours': 2000,
            }],
            "component_sets": component_sets,
            "component_failure_set": component_failure_set,
            "rel_species": rel_species,
            "leak_sizes": leak_sizes,

            "discharge_coeff": 1,
            "detection_credit": 0.9,
            "overp_method": 'bst',
            "TNT_equivalence_factor": None,
            "BST_mach_flame_speed": 0.35,
            "probit_thermal_id": 'eise', "exposure_time": 60, "probit_overp_id": 'head',
            "nozzle_model": 'yuce',
            "rel_angle": 0, "rel_humid": 0.89,
            "rand_seed": 3632850, "excl_radius": 0.01,
            "release_freq_overrides": None,
            "event_tree_override": None,
            "verbose": VERBOSE, "output_dir": None, "create_plots": False,
        }

    # @unittest.skip
    def test_basic_ch4_n2(self):
        results = qra_analysis.conduct_analysis(**self.inputs)
        risk_value = results['total_pll']
        self.assertAlmostEqual(risk_value, 1.05e-5, places=2)


if __name__ == "__main__":
    unittest.main()
