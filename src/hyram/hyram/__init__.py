from __future__ import absolute_import
__version__ = '3.0'
from . import phys, qra

import subprocess, os
cur_dir = os.path.abspath('.')
os.chdir(os.path.dirname(__file__))
try:
    process = subprocess.Popen('git describe --tags --dirty',
                         stdout=subprocess.PIPE,
                         stderr=subprocess.PIPE)
    _git_commit, _ = process.communicate()
    _git_commit = _git_commit.split('\n')[0]
    del process
except:
    pass
os.chdir(cur_dir)
del subprocess, os, cur_dir