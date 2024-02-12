# HyRAM+ Tests

This folder includes a suite of automated tests using the Python `unittest` framework. These include standard unit tests for the physics and qra modules as well as validation tests.

These tests are executed using the Python `unittest` framework. All tests are located under this directory and are sub-foldered under the categories `phys`, `qra`, and `validation`.
All tests will match the default pattern of `test_*.py`.

When running the `unittest` module on the command line, you will have to run the command at the top-level `hyram` directory or specify it with the `-t` flag. 

To run all tests: 

```python -m unittest```

Run single test files by specifying either the path or module path, e.g.:

```python -m unittest /tests/hyram/phys/test_api.py```

```python -m unittest tests.hyram.phys.test_api```

You can also run single test suites using the module path, e.g.:

```python -m unittest tests.hyram.phys.test_api.EtkTpdTestCase```

Running batches of tests can be done using unittest discovery, e.g.:

```python -m unittest discover -s ./tests/hyram/phys```

Contributors are encouraged to create or modify tests based on the changes being made to the source code.
Any new or modified tests should be discoverable by `unittest` (see [unittest](https://docs.python.org/3/library/unittest.html)), and should follow the directory and naming conventions 

The expectation is that code committed to the repository should pass all tests in this directory.
