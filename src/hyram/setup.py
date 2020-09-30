from setuptools import setup, find_packages

setup(name='hyram',
      packages = find_packages(),
      version = 3.0,
      author = 'Ethan Hecht',
      author_email = 'ehecht@sandia.gov',
      description = 'Hydrogen Risk Assessment Models - physics and QRA modules',
      url = 'https://hyram.sandia.gov/',
      license = 'GNU General Public License v3 (GPLv3)',
      classifiers = 'Development Status :: 3 - Alpha',
      install_requires = ['numpy', 'matplotlib', 'scipy', 'dill', 'pandas', 'scikit-image', 'coolprop>=6.3', 'pythonnet'],
      keywords = 'hydrogen, flame, plume, jet, overpressure, quantitiave risk assessment',
)
