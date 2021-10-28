"""
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""

import codecs
import os

from setuptools import setup, find_packages


def read(rel_path):
    here = os.path.abspath(os.path.dirname(__file__))
    with codecs.open(os.path.join(here, rel_path), 'r') as fp:
        return fp.read()


def get_version(rel_path):
    for line in read(rel_path).splitlines():
        if line.startswith('__version__'):
            # __version__ = "0.9"
            delim = '"' if '"' in line else "'"
            return line.split(delim)[1]
    raise RuntimeError("Unable to find version string.")


setup(name='hyram',
      packages = find_packages(),
      version = get_version(os.path.join('hyram', '__init__.py')),
      author = 'Ethan Hecht',
      author_email = 'ehecht@sandia.gov',
      description = 'Hydrogen Risk Assessment Models - physics and QRA modules',
      url = 'https://hyram.sandia.gov/',
      license = 'GNU General Public License v3 (GPLv3)',
      classifiers = 'Development Status :: 3 - Alpha',
      install_requires = ['numpy', 'matplotlib', 'scipy', 'dill', 'pandas', 'scikit-image', 'coolprop>=6.3', 'pythonnet'],
      keywords = 'hydrogen, flame, plume, jet, overpressure, quantitiave risk assessment',
)
