#  Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
#  Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
#  .
#  This file is part of HyRAM (Hydrogen Risk Assessment Models).
#  .
#  HyRAM is free software: you can redistribute it and/or modify
#  it under the terms of the GNU General Public License as published by
#  the Free Software Foundation, either version 3 of the License, or
#  (at your option) any later version.
#  .
#  HyRAM is distributed in the hope that it will be useful,
#  but WITHOUT ANY WARRANTY; without even the implied warranty of
#  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
#  GNU General Public License for more details.
#  .
#  You should have received a copy of the GNU General Public License
#  along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

import unittest
import os
from hyram.phys.wrapper.flame import plotFlux

class TestFlameFlux(unittest.TestCase):

    def testFlameFlux(self):
        # Run test flame flux model
        T_amb = 288.15
        P_amb = 101325
        T_H2 = 287.8
        P_H2 = 13420000
        d_orifice = 0.0036
        leak_height = 1
        angle_of_release = 0
        notional_nozzle_model = "YuceilOtugen"

        rhf_x = [0.01, 0.5, 1, 2, 2.5, 5, 10, 15, 25, 40]
        rhf_y = [1, 1, 1, 1, 1, 2, 2, 2, 2, 2]
        rhf_z = [0.01, 0.5, 0.5, 1, 1, 1, 0.5, 0.5, 1, 2]

        RH = 0.89
        radiative_source_model = "multi"
        plot_title = "ISO Plot"
        plot_contours = [1.577, 4.732, 25.237]

        result = []

        result.append(plotFlux(T_amb, P_amb,  T_H2, P_H2, d_orifice, leak_height,
                               angle_of_release, notional_nozzle_model, rhf_x, rhf_y, rhf_z,
                               smodel = radiative_source_model, RH = RH,
                               plot_title = plot_title, contours = plot_contours, 
                               verbose = False))

        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test that files exist
        directory = os.getcwd()
        Iso3D_fname = os.path.join(directory, '3DisoPlot.png')
        self.assertTrue(os.path.isfile(Iso3D_fname))
        Iso2D_fname = os.path.join(directory, '2DcutsIsoPlot.png')
        self.assertTrue(os.path.isfile(Iso2D_fname))
        T_fname = os.path.join(directory, 'Tplot.png')
        self.assertTrue(os.path.isfile(T_fname))

        # Test values
        #   Based on current HyRAM output


if __name__ == '__main__':
    unittest.main()
