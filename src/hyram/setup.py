"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""
from setuptools import setup
from setuptools.command.egg_info import egg_info

class egg_info_ex(egg_info):
    """Includes license file into `.egg-info` folder."""

    def run(self):
        # don't duplicate license into `.egg-info` when building a distribution
        if not self.distribution.have_run.get('sdist', True):
            # `sdist` command is in progress, copy license
            self.mkpath(self.egg_info)
            self.copy_file('../../COPYING.txt', self.egg_info)

        egg_info.run(self)

if __name__ == "__main__":
    setup(cmdclass={'egg_info': egg_info_ex})