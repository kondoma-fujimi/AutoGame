# Configuration file for the Sphinx documentation builder.

project = 'AutoPlayCraft 設計資料'
copyright = '2026, Masahiro Kondo'
author = 'Masahiro Kondo'
release = '1.0.0'

extensions = [
    'sphinx.ext.autodoc',
    'sphinx.ext.napoleon',
    'myst_parser',
]

templates_path = []
exclude_patterns = ['_build', 'Thumbs.db', '.DS_Store']

html_theme = 'sphinx_rtd_theme'
html_static_path = []
html_copy_source = False
html_show_sourcelink = False
html_show_sphinx = False
