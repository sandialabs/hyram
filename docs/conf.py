# Configuration file for the Sphinx documentation builder.
#
# For the full list of built-in configuration values, see the documentation:
# https://www.sphinx-doc.org/en/master/usage/configuration.html

import os
import sys
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__),"..","src")))

import hyram

# -- Project information -----------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#project-information

project = 'hyram'

copyright_path = os.path.abspath(
    os.path.join(os.path.dirname(__file__),"..","COPYRIGHT.txt")
)
with open(copyright_path, "r") as f:
    copyright_txt = f.read()
copyright = copyright_txt.strip()[:-1]  # removing trailing spaces and final period to avoid double

author_path = os.path.abspath(
    os.path.join(os.path.dirname(__file__),"trm","AUTHORS.rst")
)
with open(author_path, "r") as f:
    author_txt = f.read()
author = author_txt.strip()
          
release = hyram.__version__

# -- General configuration ---------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#general-configuration

extensions = [
    'sphinx.ext.autodoc',
    'sphinx.ext.autosummary',
    'sphinx_design',
    'sphinx.ext.napoleon',
    'sphinx.ext.doctest',
    'sphinxcontrib.bibtex'
    ]

autoclass_content = 'both'

bibtex_bibfiles = ['./trm/refs.bib']
bibtex_default_style = 'unsrt'
        
autosummary_generate = True

templates_path = ['_templates']

exclude_patterns = ['_build', 'Thumbs.db', '.DS_Store']

suppress_warnings = ["config.cache"]



# -- Options for HTML output -------------------------------------------------
# https://www.sphinx-doc.org/en/master/usage/configuration.html#options-for-html-output

import sphinx_rtd_theme

html_theme = 'sphinx_rtd_theme'

html_static_path = ['_static']
html_logo = "_static/banner_slim_lg.jpg"
html_theme_options = {'logo_only': True}
html_css_files = [
    'custom.css',
]

highlight_language = 'python3'
rst_prolog = """
.. role:: python(code)
   :language: python
"""

autodoc_preserve_defaults = True

# Enable numbering for figures and math
numfig = True

