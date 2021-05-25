"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
"""

import os
import unittest

from scipy import constants as const

from hyram.phys import c_api, Jet, Fluid

"""
NOTE: if running from IDE like pycharm, make sure cwd is hyram/ and not hyram/tests.
"""

VERBOSE = False


# @unittest.skip
class H2JetPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis.
    """
    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        c_api.setup(self.output_dir, VERBOSE)

    def tearDown(self):
        pass

    # @unittest.skip
    def test_default(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=13420000.,
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']
        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))
        self.assertTrue(not warning)

    # @unittest.skip
    def test_too_many_fluid_params_fails(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=13420000.,
                rel_phase='gas',
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    # @unittest.skip
    def test_too_few_fluid_params_fails(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=None,
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    # @unittest.skip
    def test_unchoked_flow_returns_warning(self):
        wrapped = c_api.analyze_jet_plume(
                amb_temp=288.15,
                amb_pres=101325.,
                rel_species='H2',
                rel_temp=287.8,
                rel_pres=151325.,  # low
                rel_phase=None,
                orif_diam=0.00356,
                rel_angle=1.5708,
                dis_coeff=1.0,
                nozzle_model='yuce',
                contour=0.04,
                xmin=-2.5,
                xmax=2.5,
                ymin=0,
                ymax=10,
                plot_title='Plume test',
                output_dir=self.output_dir,
                verbose=VERBOSE)

        self.assertTrue(wrapped['status'])
        data_dict = wrapped['data']
        warning = wrapped['warning']

        filepath = data_dict["plot"]
        self.assertTrue(filepath)
        self.assertTrue(os.path.isfile(filepath))

        self.assertTrue(warning)


class LH2JetPlumeTestCase(unittest.TestCase):
    """
    Test plume analysis.
    """
    def setUp(self):
        self.dir_path = os.path.dirname(os.path.realpath(__file__))
        self.output_dir = os.path.join(self.dir_path, 'temp')
        c_api.setup(self.output_dir, VERBOSE)

    def tearDown(self):
        pass

    # @unittest.skip
    def test_default_LH2(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 2.0 * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        data_dict = wrapped['data']

        # Ensure plot file exists
        filepath = data_dict["plot"]
        self.assertTrue(filepath is not None)
        self.assertTrue(os.path.isfile(filepath))

    def test_higher_pressure(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 10.0 * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        data_dict = wrapped['data']

        # Ensure plot file exists
        filepath = data_dict["plot"]
        self.assertTrue(filepath is not None)
        self.assertTrue(os.path.isfile(filepath))

    def test_equal_pressure_fails(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 1.0 * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))

    def test_pressure_past_critical_fails(self):
        amb_temp = 288.15
        amb_pres = 101325.

        rel_species = "H2"
        rel_temp = None
        rel_pres = 13. * const.atm
        rel_phase = 'liquid'

        orif_diam = 0.00356
        dis_coeff = 1.0
        rel_angle = 1.5708

        xmin = -2.5
        xmax = 2.5
        ymin = 0.
        ymax = 10.
        nozzle_model = 'yuce'
        contour = 0.04
        plot_title = "Plume test"

        wrapped = c_api.analyze_jet_plume(amb_temp, amb_pres,
                                          rel_species, rel_temp, rel_pres, rel_phase,
                                          orif_diam, rel_angle, dis_coeff, nozzle_model,
                                          contour, xmin, xmax, ymin, ymax,
                                          plot_title, self.output_dir, VERBOSE)

        self.assertFalse(wrapped['status'])
        self.assertTrue(len(wrapped['message']))


if __name__ == "__main__":
    unittest.main()
