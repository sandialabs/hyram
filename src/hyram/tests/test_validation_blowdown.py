"""
Blowdown Validation Tests
"""

import os
import unittest

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd

import hyram
from hyram.utilities import constants
from tests.utils import HY_COLOR, compute_rss, style_plot


test_dir = os.path.dirname(os.path.realpath(__file__))
data_dir = os.path.join(test_dir, 'data')
output_dir = os.path.join(test_dir, 'temp')
if not os.path.isdir(output_dir):
    os.mkdir(output_dir)


class BlowdownEkoto2012(unittest.TestCase):
    """
    Ekoto et al. 2012
    Experimental investigation of hydrogen release and ignition from fuel cell powered forklifts in enclosed spaces
    https://doi.org/10.1016/j.ijhydene.2012.03.161
    """
    def testFig3(self):
        """
        Figure 3

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        df = pd.read_csv(os.path.join(data_dir, 'ekoto-2012-fig3.csv'), header=2)
        x3 = np.ravel(df['x3'])
        y3 = np.ravel(df['y3'])
        xvals = x3[x3 > 0]
        yvals = y3[y3 > 0]

        # Input values
        temp = 297  # K...assumed initial tank temp is ambient air temp. Table 1
        pressure = 13450000 # Pa...Table 1 of above source
        d_orifice = 0.00356  # m...Table 1 of above source
        discharge_coeff = 0.75 #...Table 1 of above source
        tank_vol = 0.00363  # m^3...Table 1 of above source
        rel_species = 'H2'

        # Calculate blowdown
        fluid = hyram.phys.Fluid(species=rel_species, P=pressure, T=temp)
        orifice = hyram.phys.Orifice(d_orifice, discharge_coeff)
        source = hyram.phys.Source(tank_vol, fluid)
        mdots, _, times, _ = source.empty(orifice)

        # Interpolate calculated values at reference x-vals
        ycalc = np.interp(xvals, times, mdots)
        ycalc[27:39] = np.nan

        # Make plot
        fig, ax = plt.subplots(nrows=1, ncols=1, figsize=(7, 6))
        ax.plot(times, mdots, label = "HyRAM", color=HY_COLOR)
        ax.scatter(xvals, yvals, label = "SRI Scale, Meas")

        ax.legend(loc='upper right')
        style_plot(ax, title='Ekoto 2012 Fig. 3 - Blowdown', xlabel='Time (sec)', ylabel='Flow Rate (kg/sec)',
                    bottom=0.00042, top=0.065, left=0, right=5)
        fig.savefig(os.path.join(output_dir, 'Validation_Blowdown_Ekoto2012_Fig3.png'), bbox_inches='tight')

        # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
        rss = compute_rss(yvals, ycalc)
        # TODO: need to define a better threshold, relative to value of interest
        rss_threshold = 1.0
        self.assertTrue(rss < rss_threshold)


class BlowdownSchefer2007(unittest.TestCase):
    """
    Schefer et al. 2007
    Characterization of high-pressure, underexpanded hydrogen-jet flames
    https://doi.org/10.1016/j.ijhydene.2006.08.037
    """
    def testFig4(self):
        """
        Figure 4

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        df = pd.read_csv(os.path.join(data_dir, 'schefer-2007-fig4.csv'), header=2)
        xvals = df['x1']
        yvals = df['y1']
        init_xvals = df['x2']
        init_yvals = df['y2']

        # Input values
        temp = 290.  # K
        pressure = 4.31e7 # Pa
        d_orifice = 0.00508  # m
        discharge_coeff = 1.
        tank_vol = 1234. * constants.L_TO_M3  # m^3
        rel_species = 'H2'

        # Calculate blowdown
        fluid = hyram.phys.Fluid(species=rel_species, P=pressure, T=temp)
        orifice = hyram.phys.Orifice(d_orifice, discharge_coeff)
        source = hyram.phys.Source(tank_vol, fluid)
        _, fluid_list, times, _ = source.empty(orifice)
        times = np.array(times)
        pressures_psi = np.array([fluid.P for fluid in fluid_list]) * constants.PA_TO_PSI

        # Interpolate calculated values at reference x-vals
        ycalc = np.interp(xvals, times, pressures_psi)

        # Grab data for initial 10s
        init_pressures = pressures_psi[times <= 10.]
        init_times = times[times <= 10.]

        # Make plot
        fig, axes = plt.subplots(nrows=1, ncols=2, figsize=(12, 6))
        ax1, ax2 = axes

        ax1.plot(times, pressures_psi, label = "HyRAM", color=HY_COLOR)
        ax2.plot(init_times, init_pressures, label = "HyRAM", color=HY_COLOR)

        ax1.scatter(xvals, yvals, label='Reference')
        ax2.scatter(init_xvals, init_yvals, label='Reference')

        fig.suptitle('Schefer 2007 Fig. 4 - Blowdown', fontsize=16)
        style_plot(ax1, title='4A', xlabel='Time (s)', ylabel='Pstag (psig)')
        style_plot(ax2, title='4B - Initial 10 seconds', xlabel='Time (s)', ylabel='Pstag (psig)')
        fig.savefig(os.path.join(output_dir, 'Validation_Blowdown_Schefer2007_Fig4.png'), bbox_inches='tight')

        # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
        rss = compute_rss(yvals, ycalc)
        # TODO: need to define a better threshold, relative to value of interest
        rss_threshold = 100000000.0
        self.assertTrue(rss < rss_threshold)


class BlowdownProust2011(unittest.TestCase):
    """
    Proust et al. 2011
    High pressure hydrogen fires
    https://doi.org/10.1016/j.ijhydene.2010.04.055
    """
    def testFig4(self):
        """
        Figure 4

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        df = pd.read_csv(os.path.join(data_dir, 'proust-2011-fig4.csv'), header=2)
        # Note that the temp values in data file appear to be off. Using the temp data for the pressure curve.
        # TODO: verify and fix csv file
        ref_temp_x = df['x1']
        ref_temp_y = df['y1']
        ref_pres_x = df['x3']
        ref_pres_y = df['y3']

        # Input values
        temp = 315.15  # K
        pressure = 90 * 1000000. # Pa
        d_orifice = 0.002  # m
        tank_vol = 25. * constants.L_TO_M3  # m^3
        rel_species = 'H2'

        # Calculate blowdown
        fluid = hyram.phys.Fluid(species=rel_species, P=pressure, T=temp)
        orifice = hyram.phys.Orifice(d_orifice)
        source = hyram.phys.Source(tank_vol, fluid)
        _, fluid_list, times, _ = source.empty(orifice)

        # Extract pressures and temps
        times = np.array(times)
        pressures_bar = np.array([fluid.P for fluid in fluid_list]) / constants.BAR_TO_PA
        temps_degC = np.array([fluid.T for fluid in fluid_list]) - 273.15

        # Interpolate calculated values at reference x-vals
        ycalc_temp = np.interp(ref_temp_x, times, pressures_bar)
        ycalc_pres = np.interp(ref_pres_x, times, temps_degC)

        # Make plot
        fig, ax = plt.subplots(nrows=1, ncols=1, figsize=(8, 6))
        axy2 = ax.twinx()

        l1 = ax.plot(times, pressures_bar, label='HyRAM Pressure', color=HY_COLOR)
        l2 = axy2.plot(times, temps_degC, label='HyRAM Temperature', ls='--', color=HY_COLOR)

        s1 = ax.scatter(ref_temp_x, ref_temp_y, label='Reference Temperature', color='C3')
        s2 = ax.scatter(ref_pres_x, ref_pres_y, label='Reference Pressure', color='C0')

        style_plot(ax, title='Proust 2011 Figure 4', xlabel='Time (s)', ylabel='Overpressure (bar)',
                         top=1000, bottom=0, left=0, right=70)
        style_plot(axy2, ylabel=r'Temperature ($\degree$C)', legend=False)
        ls = [l1[0], l2[0], s1, s2]
        labels = [l.get_label() for l in ls]
        ax.legend(ls, labels)
        fig.savefig(os.path.join(output_dir, 'Validation_Blowdown_Proust2011_Fig4.png'), bbox_inches='tight')

        # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
        rss_temp = compute_rss(ref_temp_y, ycalc_temp)
        # TODO: need to define a better threshold, relative to value of interest
        rss_temp_threshold = 10000000.0
        self.assertTrue(rss_temp < rss_temp_threshold)
        rss_pres = compute_rss(ref_pres_y, ycalc_pres)
        # TODO: need to define a better threshold, relative to value of interest
        rss_pres_threshold = 100000000.0
        self.assertTrue(rss_pres < rss_pres_threshold)


class BlowdownSchefer2006(unittest.TestCase):
    """
    Schefer et al. 2006
    Spatial and radiative properties of an open-flame hydrogen plume
    https://doi.org/10.1016/j.ijhydene.2005.11.020
    """
    def testFig3b(self):
        """
        Figure 3b

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        df = pd.read_csv(os.path.join(data_dir, 'schefer-2006-fig3b.csv'), header=2)
        xvals = df['x1']
        yvals = df['y1']

        # Input values
        temp = 315.15  # K
        pressure = 1.5513e7 # Pa
        d_orifice = 0.003175  # m
        tank_vol = 0.098  # m^3
        rel_species = 'H2'

        # Calculate blowdown
        fluid = hyram.phys.Fluid(species=rel_species, P=pressure, T=temp)
        orifice = hyram.phys.Orifice(d_orifice)
        source = hyram.phys.Source(tank_vol, fluid)
        mdots, _, times, _ = source.empty(orifice)
        
        # Interpolate calculated values at reference x-vals
        mdots_gps = np.array(mdots) * 1000.
        ycalc = np.interp(xvals, times, mdots_gps)

        # Make plot
        fig, ax = plt.subplots(nrows=1, ncols=1, figsize=(8, 6))

        ax.plot(times, mdots_gps, label='HyRAM', color=HY_COLOR)
        ax.scatter(xvals, yvals, label='Reference', color='C0')

        style_plot(ax, title='Schefer 2006 Figure 3b', xlabel='Time (s)', ylabel='mass flow (gm/sec)',
                         top=80, bottom=0, left=0, right=100)
        fig.savefig(os.path.join(output_dir, 'Validation_Blowdown_Schefer2006_Fig3b.png'), bbox_inches='tight')

        # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
        rss = compute_rss(yvals, ycalc)
        # TODO: need to define a better threshold, relative to value of interest
        rss_threshold = 1000.0
        self.assertTrue(rss < rss_threshold)



if __name__ == "__main__":
    unittest.main()
