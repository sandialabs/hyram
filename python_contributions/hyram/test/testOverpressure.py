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
from hyram.phys.wrapper.overpressure import execall_indoor_release

class TestOverpressure(unittest.TestCase):

    def testOverP(self):
        # Run test overpressure model
        ambient_pressure = 101325
        ambient_temperature = 288.15
        H2_pressure = 13420000
        H2_temperature = 287.8
        tank_volume = 0.0036
        orifice_diameter = 0.0036
        orifice_discharge_coefficient = 1
        release_discharge_coefficient = 1
        release_area = 0.0172
        release_height = 0.2495
        enclosure_height = 2.72
        floor_ceiling_area = 16.7222
        dist_release_to_wall = 2.1255
        ceiling_vent_cross_sectional_area = 0.0908
        ceiling_vent_height_from_floor = 2.42
        floor_vent_height_from_floor = 0.044
        floor_vent_cross_sectional_area = 0.0076
        volume_flow_rate = 0
        TimesToPlotInSeconds = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 
                                14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24,
                                25, 26, 27, 28, 29, 29.5]
        PlotDotsPressureAtTimes = []
        LimitLinePressures = []
        max_sim_time = 30
        AngleOfReleaseInRadians = 0
        result = []

        result.append(execall_indoor_release(ambient_pressure, 
                                             ambient_temperature,
                                             H2_pressure,
                                             H2_temperature,
                                             tank_volume,
                                             orifice_diameter,
                                             orifice_discharge_coefficient,
                                             release_discharge_coefficient,
                                             release_area,
                                             release_height,
                                             enclosure_height,
                                             floor_ceiling_area,
                                             dist_release_to_wall,
                                             ceiling_vent_cross_sectional_area,
                                             ceiling_vent_height_from_floor,
                                             floor_vent_height_from_floor,
                                             floor_vent_cross_sectional_area,
                                             volume_flow_rate,
                                             TimesToPlotInSeconds,
                                             PlotDotsPressureAtTimes,
                                             LimitLinePressures,
                                             max_sim_time,
                                             AngleOfReleaseInRadians,
                                             verbose=False))

        # Test that result is a non-empty list
        self.assertTrue(result)

        # Test that files exist
        directory = os.getcwd()
        fname = os.path.join(directory, 'pressure_plot.png')
        lh_fname = os.path.join(directory, 'layer_plot.png')
        traj_fname = os.path.join(directory, 'trajectory_plot.png')
        fm_fname = os.path.join(directory, 'flam_mass_plot.png')
        self.assertTrue(os.path.isfile(fname))
        self.assertTrue(os.path.isfile(lh_fname))
        self.assertTrue(os.path.isfile(traj_fname))
        self.assertTrue(os.path.isfile(fm_fname))

        # Test values


if __name__ == '__main__':
    unittest.main()
