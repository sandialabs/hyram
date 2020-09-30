"""
Utilities for python tests
"""

import numpy as np
import matplotlib.pyplot as plt

# C0 blue
# C1 orange
# C2 green
# C3 red
# C4 purple
# C5 brown
HY_COLOR = 'C1'

lw = 2


def compute_rss(model_data, real_data):
    if len(model_data) != len(real_data):
        print('Lengths not equal')
        return None
    else:
        return np.nansum((real_data - model_data) ** 2)


def style_plot(ax, title=None, xlabel=None, ylabel=None, bottom=None, top=None, left=None, right=None, y_pos='left', legend=True):
    plt.rcParams["figure.facecolor"] = "w"  # for dark-theme editors
    ax.minorticks_on()
    ax.grid(which='major', alpha=.5)
    ax.grid(which='minor', alpha=.2)
    if xlabel:
        ax.set_xlabel(xlabel)
    if ylabel:
        ax.set_ylabel(ylabel)
    if title:
        ax.set_title(title)

    # Position of y-axis
    if y_pos == 'right':
        ax.yaxis.set_label_position("right")
        ax.yaxis.tick_right()

    ax.set_xlim(left, right)
    ax.set_ylim(bottom, top)

    if legend:
        ax.legend()
