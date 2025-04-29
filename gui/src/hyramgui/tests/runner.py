"""
Copyright 2015-2025 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.

"""
import os
import subprocess
from pathlib import Path


def run_tests():
    """Execute tests in specified directories.

    Can also do this via cl from hyram/repo directory:
    $ pytest gui tests/gui tests/hyram/phys tests/hyram/qra

    """
    directories = [
        "gui",
        "tests/gui",
        "tests/hyram/phys",
        "tests/hyram/qra",
        # "tests/hyram/validation",
    ]

    repo_dir = Path(os.getcwd()).parents[3]
    os.chdir(repo_dir)
    print(f'Initializing Test Runner')
    print(f'Dir: {os.getcwd()}')

    venv_directory = "..\\envs\\py3.9d1"
    activate_script_path = os.path.join(venv_directory, "Scripts", "activate")

    # Activate the virtual environment
    activate_env_cmd = f"{activate_script_path} &&"

    for directory in directories:
        print(f"\nRunning tests in {directory} directory\n{'-' * 30}")

        # Build the pytest command
        pytest_cmd = f"{activate_env_cmd} pytest {directory}"
        print(f"Cmd: {pytest_cmd}")

        # Execute the command
        result = subprocess.run(pytest_cmd, shell=True, capture_output=True, text=True)
        print(result.stdout)


# Execute run_tests function
run_tests()
