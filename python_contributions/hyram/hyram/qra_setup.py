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
from distutils.core import setup
from distutils.extension import Extension
from Cython.Distutils import build_ext

# Search from dir of setup.py, not python exe
package_dir = os.path.dirname(os.path.realpath(__file__))
source_dir = os.path.join(package_dir, 'qra')

# Parameters for build_ext:
# build-lib - location in which to place compiled extensions
# inplace - ignore build-lib and put compiled extensions into the source
# build-temp - directory for temporary files (build by-products)
# force - ignore timestamps and re-compile everything
# https://github.com/python/cpython/blob/master/Lib/distutils/command/build_ext.py

ext_modules = [
    Extension("distributions", [os.path.join(source_dir, 'distributions.py')]),

    Extension("utilities.constants", [os.path.join(source_dir, 'utilities/constants.py')]),
    Extension("utilities.math_utils", [os.path.join(source_dir, 'utilities/math_utils.py')]),
    Extension("utilities.misc_utils", [os.path.join(source_dir, 'utilities/misc_utils.py')]),
    Extension("utilities.c_utils", [os.path.join(source_dir, 'utilities/c_utils.py')]),
    Extension("utilities.object_utils", [os.path.join(source_dir, 'utilities/object_utils.py')]),

    Extension("data_sources.component_data", [os.path.join(source_dir, 'data_sources/component_data.py')]),
    Extension("data_sources.failure_data", [os.path.join(source_dir, 'data_sources/failure_data.py')]),

    Extension("components", [os.path.join(source_dir, 'components.py')]),
    Extension("leaks", [os.path.join(source_dir, 'leaks.py')]),
    Extension("occupants", [os.path.join(source_dir, 'occupants.py')]),
    Extension("probit", [os.path.join(source_dir, 'probit.py')]),

    Extension("analysis", [os.path.join(source_dir, 'analysis.py')]),
    Extension("api", [os.path.join(source_dir, 'api.py')]),
    Extension("capi", [os.path.join(source_dir, 'capi.py')]),
]
setup(
    name='qra',
    version='2.0',
    description='Python Quantitative Risk Analysis for HyRAM',
    author='Cianan Sims',
    author_email='cianan@simsindustries.com',
    cmdclass={'build_ext': build_ext},
    ext_modules=ext_modules
)
