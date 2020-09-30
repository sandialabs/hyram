"""
Jet/Plume Validation Tests
"""

import os
import unittest

import matplotlib.pyplot as plt
import numpy as np
import pandas as pd
from scipy.constants import milli, kilo, liter, minute, pi, bar

import hyram
from tests.utils import HY_COLOR, compute_rss, style_plot


test_dir = os.path.dirname(os.path.realpath(__file__))
data_dir = os.path.join(test_dir, 'data')
output_dir = os.path.join(test_dir, 'temp')
if not os.path.isdir(output_dir):
    os.mkdir(output_dir)


# TODO: add Molkov book tests

class JetPlumeHoufSchefer2008(unittest.TestCase):
    """
    Houf and Schefer 2008
    Analytical and experimental investigation of small-scale unintended releases of hydrogen
    https://doi.org/10.1016/j.ijhydene.2007.11.031
    """
    def setUp(self):
        """
        Common values and objects
        """
        # Input values
        self.d = 1.905 * milli#meters
        self.rho = 0.0838 #kg/m^3
        self.Q = 22.9 * liter/minute
        self.T_amb = 294 #K
        self.P_amb = 100 * kilo#Pa

        # create common objects
        self.air = hyram.phys.Fluid(T = self.T_amb, P = self.P_amb, species = 'air')
        self.gas = hyram.phys.Fluid(T = self.T_amb, P = self.P_amb)
        self.orifice = hyram.phys.Orifice(d = self.d)
        self.jet = hyram.phys.Jet(self.gas, self.orifice, self.air, mdot = self.rho*self.Q, theta0 = pi/2)#, lam = 1.16)
        
    def testFig5(self):
        """
        Figure 5

        Measured time-averaged radial profiles of H2 mass fraction for a vertical hydrogen jet (Fr_den=268) with diameter of 1.905 mm

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig5_df = pd.read_csv(os.path.join(data_dir, 'houfschefer-2008-fig5.csv'), header=1)

        # Create plot
        fig, ax = plt.subplots()

        # For different axial distances...
        axial_dists = [10, 25, 50, 75, 100]
        for z_over_d in axial_dists:
            # Get reference values
            ref_x = fig5_df['z' + str(z_over_d) + 'x']
            ref_y = fig5_df['z' + str(z_over_d) + 'y']

            # Get plume profile at desired point
            profile = self.jet._radial_profile(z_over_d * self.orifice.d, ind_var='Y', nB=20)
            xvals = profile[0] * 1000.
            yvals = profile[1]
            
            # Interpolate calculated values at reference x-vals
            ycalc = np.interp(ref_x, xvals, yvals)

            # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
            rss = compute_rss(ref_y, ycalc)
            # TODO: need to define a better threshold, relative to value of interest
            rss_threshold = 1.0
            self.assertTrue(rss < rss_threshold)

            # Plot values
            ax.plot(ref_x, ref_y, lw=0, marker = 'o', ms = 3, mfc = 'none', label='Reference Z/D=%d'%z_over_d)
            c = ax.lines[np.argwhere(np.array([l.get_label() for l in ax.lines]) == "Reference Z/D={}".format(z_over_d))[0][0]].get_color()
            ax.plot(xvals, yvals, c = c, label="HyRAM Z/D={}".format(z_over_d))

        # Finalize plot
        style_plot(ax, title="Houf and Schefer 2008 Fig 5", xlabel='X (mm)', ylabel=r'H$_2$ Mass Fraction', left=-25, right=25, top=0.3, bottom=0.0)
        fig.savefig(os.path.join(output_dir, 'Validation_JetPlume_HoufSchefer2008_Fig5.png'), bbox_inches='tight')

    def testFig8(self):
        """
        Figure 8
        
        Comparison of simulation of hydrogen mole fraction concentration decay along jet centerline (in the momentum-dominated region) with data from the slow leak experiment for a vertical hydrogen leak (Fr_den = 268, D = 1.905 mm)

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig8_df = pd.read_csv(os.path.join(data_dir, 'houfschefer-2008-fig8.csv'), header=1)
        ref_x = fig8_df['dp_x']
        ref_y = fig8_df['dp_y']

        # Get values from calculated jet
        xvals = self.jet.S / self.orifice.d
        yvals = 1 / self.jet.X_cl

        # Interpolate calculated values at reference x-vals
        ycalc = np.interp(ref_x, xvals, yvals)

        # Make plot   
        fig, ax = plt.subplots()

        # ax.plot(fig8_df['slm_x'], fig8_df['slm_y'],lw=4, color='tab:blue', label="Slow Leak Model")
        ax.plot(ref_x, ref_y, marker='o', label='Reference')
        ax.plot(xvals, yvals, label="HyRAM")

        style_plot(ax, xlabel='Axial Distance Z/D', ylabel=r"$1/X_{cl}$", left=0, right=80, bottom=0., top=8.)
        fig.savefig(os.path.join(output_dir, 'Validation_JetPlume_HoufSchefer2008_Fig8.png'), bbox_inches='tight')

        # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
        rss = compute_rss(ref_y, ycalc)
        # TODO: need to define a better threshold, relative to value of interest
        rss_threshold = 10.0
        self.assertTrue(rss < rss_threshold)

    def testFig9(self):
        """
        Figure 9
        
        Comparison of simulations of the centerline concentration decay (XH2 = mole fraction) for vertical buoyant hydrogen jets
        with concentration data from the slow leak experiment (Fr_den=99, 152, 269)

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig9_df = pd.read_csv(os.path.join(data_dir, 'houfschefer-2008-fig9.csv'), header=1)

        # Input values
        Qvals = np.array([3.5, 8.497, 13.08, 22.9]) * liter/minute # volumetric flow rates for Fr = 41, 99, 152, 268

        # Calculate jet plume
        jets = []
        for Q in Qvals:
            jets.append(hyram.phys.Jet(self.gas, self.orifice, self.air, mdot = self.rho*Q, theta0 = pi/2))#, lam = 1.16))

        # Make plot   
        colors = {41:'tab:green', 99:'tab:blue', 152:'tab:red', 268:'black'}
        symbols = {41:'>', 99:'o', 152:'s', 268:'D'}
        fig, ax = plt.subplots()
        Frs = [41, 99, 152, 268]
        ref_keys = {99: 'blu', 152: 'red', 268: 'bla'}
        for Fr, j in zip(Frs, jets):
            # Calculate values
            xvals = j.S / self.orifice.d
            yvals = 1 / j.X_cl

            if Fr in ref_keys:
                # Get reference values
                ref_x = fig9_df[ref_keys[Fr] + '_x']
                ref_y = fig9_df[ref_keys[Fr] + '_y']
            
                # Interpolate for reference values
                ycalc = np.interp(ref_x, xvals, yvals)

                # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
                rss = compute_rss(ref_y, ycalc)
                # TODO: need to define a better threshold, relative to value of interest
                rss_threshold = 10000.0
                self.assertTrue(rss < rss_threshold)

                # Plot reference values
                ax.plot(ref_x, ref_y, marker=symbols[Fr], linestyle='None', color=colors[Fr], label=('Reference Fr=' + str(Fr)))
            
            # Plot values
            ax.plot(xvals, yvals, c=colors[Fr], linestyle='--', label = 'HyRAM Fr=%d'%Fr)
        style_plot(ax, title="Houf and Schefer 2008 Fig 9", xlabel='Z/D', ylabel=r"1/$X_{H_{2}}$", left=0, right=200, bottom=0, top=25)
        fig.savefig(os.path.join(output_dir, 'Validation_JetPlume_HoufSchefer2008_Fig9.png'), bbox_inches='tight')


class JetPlumeRugglesEkoto2012(unittest.TestCase):
    """
    Ruggles and Ekoto 2012
    Ignitability and mixing of underexpanded hydrogen jets
    https://doi.org/10.1016/j.ijhydene.2012.03.063
    """
    def testFig7(self):
        """
        Figure 7

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig7df = pd.read_csv(os.path.join(data_dir, 'rugglesekoto-2012-fig7.csv'), header=2)
        ref_x1 = fig7df['x1']
        ref_y1 = fig7df['y1']
        ref_x2 = fig7df['x3']
        ref_y2 = fig7df['y3']

        # Input values
        d = 0.75*2 *milli#meters
        T_H2 = 295.4 #K
        P_H2 = 983.2 *kilo#Pa
        C_d = 0.979
        T_amb = 296. #K
        P_amb = 98.37 *kilo#Pa

        # Calculate jet plume
        air = hyram.phys.Fluid(T = T_amb, P = P_amb, species = 'air')
        gas = hyram.phys.Fluid(T = T_H2, P = P_H2)
        orifice = hyram.phys.Orifice(d = d, Cd = C_d)
        jet = hyram.phys.Jet(gas, orifice, air, theta0 = pi/2)#, lam = 1.16)

        # Get calculated values
        xvals = jet.y / (orifice.d / 2)
        yvals1 = 1 / jet.Y_cl
        yvals2 = jet.B / milli

        # Interpolate for reference values
        ycalc1 = np.interp(ref_x1, xvals, yvals1)
        ycalc2 = np.interp(ref_x2, xvals, yvals2)

        # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
        rss1 = compute_rss(ref_y1, ycalc1)
        # TODO: need to define a better threshold, relative to value of interest
        rss1_threshold = 10000.0
        self.assertTrue(rss1 < rss1_threshold)
        rss2 = compute_rss(ref_y2, ycalc2)
        # TODO: need to define a better threshold, relative to value of interest
        rss2_threshold = 1000.0
        self.assertTrue(rss2 < rss2_threshold)

        # Make plot
        fig, axes = plt.subplots(nrows=1, ncols=2, figsize=(16, 6))
        ax1, ax2 = axes

        ax1.plot(xvals, yvals1, label = 'HyRAM')
        ax2.plot(xvals, yvals2, label = 'HyRAM')
        ax1.plot(ref_x1, ref_y1, marker='o', linestyle='None', label='Referece')
        ax2.plot(ref_x2, ref_y2, marker='o', linestyle='None', label='Referece')

        style_plot(ax1, xlabel='z/r', ylabel=r"$1/Y_{cl}$")
        style_plot(ax2, xlabel='z/r', ylabel="Jet Half Width (mm)")
        ax1.set_xlim(80, 350); ax1.set_ylim(10, 60)
        ax2.set_xlim(80, 350); ax2.set_ylim(5, 30)
        fig.savefig(os.path.join(output_dir, 'Validation_JetPlume_RugglesEkoto_Fig7.png'), bbox_inches='tight')


class JetPlumeHan2013(unittest.TestCase):
    """
    Han, Chang, and Kim 2013
    Release characteristics of highly pressurized hydrogen through a small hole
    https://doi.org/10.1016/j.ijhydene.2012.11.071
    """
    def setUp(self):
        """
        Common values and objects
        """
        # Input values
        T_H2 = 293. #K
        T_amb = 293. #K
        P_amb = 101325. #Pa
        self.diameters = np.array([0.5, 0.7, 1]) *milli#meters
        self.pressures_H2 = np.arange(100, 401, 100) * bar
        
        # create common objects
        air = hyram.phys.Fluid(T = T_amb, P = P_amb, species = 'air')
        jets = {}
        for P_H2 in self.pressures_H2:
            gas = hyram.phys.Fluid(T = T_H2, P = P_H2)
            jets[P_H2] = {}
            for d in self.diameters:
                orifice = hyram.phys.Orifice(d = d)
                jets[P_H2][d] = hyram.phys.Jet(gas, orifice, air, theta0 = 0, Ymin = 1e-4)#, lam = 1.16)
        self.jets = jets

    def testFig3(self):
        """
        Figure 3

        Concentration as a function of distance

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig3a_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig3a.csv'), header=2)
        fig3b_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig3b.csv'), header=2)
        fig3c_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig3c.csv'), header=2)
        fig3d_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig3d.csv'), header=2)

        # Make figure
        fig, axes = plt.subplots(nrows=2, ncols=2, figsize=(18, 10))
        ax1, ax2 = axes[0]
        ax3, ax4 = axes[1]
        ax_list = [ax1, ax2, ax3, ax4]
        fig.tight_layout(pad=4.)
        fig.suptitle('Han, Chang, and Kim 2013 - Figure 3', fontsize=16)

        d1_color = 'tab:green'
        d2_color = 'tab:blue'
        d3_color = 'tab:orange'
        colors = [d1_color, d2_color, d3_color]

        for ax, rel_pres, df in zip(ax_list, self.pressures_H2, [fig3a_df, fig3b_df, fig3c_df, fig3d_df]):
            ax.set_yscale('log')
            for d, color, dfidx in zip(self.diameters, colors, ['1', '3', '5']):
                # Get calculated values
                xvals = self.jets[rel_pres][d].x
                yvals = self.jets[rel_pres][d].X_cl

                # Add values to plot
                ax.plot(xvals, yvals, color=color, label = 'HyRAM d=%.1fmm'%(d/milli))
                
                # Check if data exists (fig 3b missing data points)
                if 'x6' in df or dfidx != '5':
                    # Get reference values
                    ref_x = df['x' + dfidx]
                    ref_y = df['y' + dfidx]

                    # Interpolate for reference values
                    ycalc = np.interp(ref_x, xvals, yvals)

                    # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
                    rss = compute_rss(ref_y, ycalc)
                    # TODO: need to define a better threshold, relative to value of interest
                    rss_threshold = 1.0
                    self.assertTrue(rss < rss_threshold)

                    # Add values to plot
                    ax.plot(ref_x, ref_y, marker='o', linestyle='None', color=color, label='Reference d=%.1fmm'%(d/milli))
            style_plot(ax, title=r"$P_0$=%d bar"%(rel_pres/bar), xlabel='Distance (m)', left=0, right=10, ylabel="Hydrogen Concentration")
        fig.savefig(os.path.join(output_dir, 'Validation_JetPlume_Han_Fig3.png'), bbox_inches='tight')

    def testFig6(self):
        """
        Figure 6

        Distance to percentage of LFL

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig6a_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig6a.csv'), header=2)
        fig6b_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig6b.csv'), header=2)
        fig6c_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig6c.csv'), header=2)

        # Make figure
        fig, axes = plt.subplots(nrows=1, ncols=3, figsize=(24, 8))
        fig.suptitle('Han, Chang, and Kim 2013 - Figure 3', fontsize=16)

        LFL = 0.04

        frac_labels = {1: '', 2: '1/2', 4: '1/4'}
        frac_markers = {1: 'o', 2: 's', 4: 'D'}
        refidx = {1: '2', 2: '4', 4: '6'}
        for d, ax, df in zip(self.diameters, axes, [fig6a_df, fig6b_df, fig6c_df]):
            for frac in [1, 2, 4]:
                # Get reference values
                # Note: the labels for Exp and Cal in the csv file appears to be incorrect
                # TODO: verify and fix csv file if so
                ref_x = df['x' + refidx[frac]]
                ref_y = df['y' + refidx[frac]]

                # Get calculated values
                xvals = []
                yvals = []
                for P in self.pressures_H2:
                    # Get calculated values
                    P_bar = P/bar
                    Xcl_vals = self.jets[P][d].X_cl[::-1]
                    dist_vals = self.jets[P][d].S[::-1]
                    dil_length = np.interp(LFL / frac, Xcl_vals, dist_vals)
                    xvals.append(P_bar)
                    yvals.append(dil_length)
                
                # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
                rss = compute_rss(ref_y, yvals)
                # TODO: need to define a better threshold, relative to value of interest
                rss_threshold = 100.0
                self.assertTrue(rss < rss_threshold)

                # Add values to plot
                ax.plot(xvals, yvals, marker=frac_markers[frac], linestyle='None', label = ('HyRAM ' + frac_labels[frac] + 'LFL'))
                ax.plot(ref_x, ref_y, marker=frac_markers[frac], linestyle='None', label=('Reference ' + frac_labels[frac] + 'LFL'))
            style_plot(ax, title=('d=' + str(d) + ' mm'), xlabel='Pressure (bar)', ylabel="Dilution length (m)", bottom=0, top=16.)
        plt.savefig(os.path.join(output_dir, 'Validation_JetPlume_Han_Fig6.png'), bbox_inches='tight')

    def testFig7(self):
        """
        Figure 7

        Inverse centerline mole fractions (300 bar)

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig7_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig7.csv'), header=2)

        # Make figure
        fig, axes = plt.subplots(nrows=1, ncols=3, figsize=(24, 8))

        # For each diameter
        for r, a, d, t in zip(['1', '4', '7'], axes, self.diameters, ['d=0.5mm', 'd=0.7mm', 'd=1.0mm']):
            # Get reference values
            ref_x = fig7_df['x' + r]
            ref_y = fig7_df['y' + r]

            # Get calculated values
            xvals = self.jets[300*bar][d].S / d
            yvals = 1 / self.jets[300*bar][d].X_cl

            # Interpolate for reference values
            ycalc = np.interp(ref_x, xvals, yvals)

            # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
            rss = compute_rss(ref_y, ycalc)
            # TODO: need to define a better threshold, relative to value of interest
            rss_threshold = 100.0
            self.assertTrue(rss < rss_threshold)

            # Add values to plot
            a.plot(xvals, yvals, label='HyRAM')
            a.plot(ref_x, ref_y, marker='s', linestyle='None', color='k', label='Data')

            style_plot(a, title=t, xlabel='z/d', ylabel=r"1/$X_{H2}$", bottom=0, top=80., left = 0, right = 15e3)
        fig.savefig(os.path.join(output_dir, 'Validation_JetPlume_Han_Fig7.png'), bbox_inches='tight')

    def testFig8(self):
        """
        Figure 8

        Inverse centerline mole fractions (d = 0.7mm)

        Note: test metric is based on sum of squared error, but a plot is also made for visual inspection
        """
        # Get reference data
        fig8a_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig8a.csv'), header=2)
        fig8b_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig8b.csv'), header=2)
        fig8c_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig8c.csv'), header=2)
        fig8d_df = pd.read_csv(os.path.join(data_dir, 'han-2013-fig8d.csv'), header=2)

        # Make figure
        fig, axes = plt.subplots(nrows=2, ncols=2, figsize=(12, 14))

        # For each pressure
        for rel_pres, ax, ref_df in zip(self.pressures_H2, axes.flatten(), [fig8a_df, fig8b_df, fig8c_df, fig8d_df]):
            # Get reference values
            ref_x = ref_df['x1']
            ref_y = ref_df['y1']

            # Get calculated values
            xvals = self.jets[rel_pres][0.7*milli].S / (0.7*milli)
            yvals = 1. / self.jets[rel_pres][0.7*milli].X_cl

            # Interpolate for reference values
            ycalc = np.interp(ref_x, xvals, yvals)

            # Compare by computing residual sum of squares (RSS/SSE) for model vals at same x-vals
            rss = compute_rss(ref_y, ycalc)
            # TODO: need to define a better threshold, relative to value of interest
            rss_threshold = 100.0
            self.assertTrue(rss < rss_threshold)

            # Add values to plot
            ax.plot(xvals, yvals, label='HyRAM')
            ax.plot(ref_x, ref_y, marker='s', linestyle='None', color='k', label="Reference")

            style_plot(ax, title=r"$P_{0}=%d$ bar"%(rel_pres/bar), xlabel='z/d', ylabel=r"1/$X_{H2}$", bottom=0, top=120., left=0, right=11000.)
        fig.savefig(os.path.join(output_dir, 'Validation_JetPlume_Han_Fig8.png'), bbox_inches='tight')



if __name__ == "__main__":
    unittest.main()
