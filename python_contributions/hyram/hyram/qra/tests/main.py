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

import os
import unittest

from . import test_full


# def suite():
#     suite = unittest.TestSuite()
#     suite.addTest(test_full.TestDefaultInputs())
#     return suite


if __name__ == '__main__':
    # loader = unittest.TestLoader()
    # start_dir = os.path.curdir()
    # suite = loader.discover(start_dir)
    # suite1 = unittest.TestLoader().loadTestsFromTestCase(test_full.TestDefaultInputs)
    # suite2 = unittest.TestLoader().loadTestsFromTestCase(test_full.TestOneOfEachComponent)
    suite3 = unittest.TestLoader().loadTestsFromTestCase(test_full.TestOneOfEachComponent)
    alltests = unittest.TestSuite([
        # suite1,
        # suite2,
        suite3,
    ])

    runner = unittest.TextTestRunner(verbosity=2)
    runner.run(alltests)
