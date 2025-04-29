"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import os
import unittest

import numpy as np
import matplotlib.pyplot as plt

from hyram.phys import Orifice, Fluid, Jet, Flame
from hyram.phys._plots import plot_contour
from hyram.utilities import misc_utils


VERBOSE = False
OUTPUT_DIR = 'out'


class TestContourPlots(unittest.TestCase):
    def setUp(self):
        t_amb = 300  # K
        orifice_diameter = 0.0254  # m
        dischage_coefficent = 1
        ambient_pressure = 101325  # Pa
        p_rel = 60e5  # Pa

        orifice = Orifice(orifice_diameter, dischage_coefficent)
        amb_fluid = Fluid(species='air', P=ambient_pressure, T=t_amb)
        rel_fluid = Fluid(species='hydrogen', T=t_amb, P=p_rel)

        nozzle_cons_m, nozzle_t_param = misc_utils.convert_nozzle_model_to_params('yuce', rel_fluid)

        self.jet_object_horizontal = Jet(rel_fluid, orifice, amb_fluid,
                                        nn_conserve_momentum=nozzle_cons_m,
                                        nn_T=nozzle_t_param,
                                        verbose=VERBOSE)

        self.jet_object_vertical = Jet(rel_fluid, orifice, amb_fluid,
                                        theta0=np.pi/2,
                                        nn_conserve_momentum=nozzle_cons_m,
                                        nn_T=nozzle_t_param,
                                        verbose=VERBOSE)

        self.flame_object = Flame(rel_fluid, orifice, amb_fluid)

    def test_auto_limits_horizontal(self):
        fig = plot_contour(data_type="mole", jet_or_flame=self.jet_object_horizontal, vlims=None)
        self.assertTrue(fig is not None)

        filename = os.path.join(OUTPUT_DIR, 'test_auto_limits_h.png')
        fig.savefig(filename)
        plt.close(fig)

    def test_auto_limits_vertical(self):
        fig = plot_contour(data_type="mole", jet_or_flame=self.jet_object_vertical, vlims=None)
        self.assertTrue(fig is not None)

        filename = os.path.join(OUTPUT_DIR, 'test_auto_limits_v.png')
        fig.savefig(filename)
        plt.close(fig)

    def test_set_limits_horizontal(self):
        fig = plot_contour(data_type="mole", jet_or_flame=self.jet_object_horizontal, xlims=(0, 20), ylims=(-2, 2), contour_levels=0.1)
        self.assertTrue(fig is not None)

        filename = os.path.join(OUTPUT_DIR, 'test_set_limits_h.png')
        fig.savefig(filename)
        plt.close(fig)

    def test_set_limits_vertical(self):
        fig = plot_contour(data_type="mole", jet_or_flame=self.jet_object_vertical, xlims=(-2, 2), ylims=(0, 20), contour_levels=0.1)
        self.assertTrue(fig is not None)

        filename = os.path.join(OUTPUT_DIR, 'test_set_limits_v.png')
        fig.savefig(filename)
        plt.close(fig)

    def test_temp_plot(self):
        fig = plot_contour(data_type="temperature", jet_or_flame=self.flame_object, xlims=(0, 20), ylims=(-2.5, 5))
        self.assertTrue(fig is not None)

        filename = os.path.join(OUTPUT_DIR, 'test_temp.png')
        fig.savefig(filename)
        plt.close(fig)

    def test_invalid_input(self):
        with self.assertRaises(ValueError):
            plot_contour(data_type="test", jet_or_flame=self.jet_object_horizontal)

    def test_invalid_contour_levels(self):
        with self.assertRaises(ValueError):
            plot_contour(data_type="mole", jet_or_flame=self.jet_object_horizontal, contour_levels=0)
        with self.assertRaises(ValueError):
            plot_contour(data_type="mole", jet_or_flame=self.jet_object_horizontal, contour_levels=1)
        with self.assertRaises(ValueError):
            plot_contour(data_type="mole", jet_or_flame=self.jet_object_horizontal, contour_levels=[0,1])
