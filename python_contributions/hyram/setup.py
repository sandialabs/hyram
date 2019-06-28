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

from setuptools import setup, find_packages
from hyram import __version__ as ver

setup(name='hyram', 
      packages = find_packages(),
      version = ver,
      author = 'Ethan Hecht',
      author_email = 'ehecht@sandia.gov',
      description = 'physics and QRA modules for HyRAM',
      url = '',
      license = '',
      classifiers = 'Development Status :: 3 - Alpha',
      install_requires = ['numpy', 'matplotlib', 'scipy', 'dill', 'pandas'],
      keywords = 'hydrogen, flame, plume, jet, overpressure, quantitiave risk assessment',
)
