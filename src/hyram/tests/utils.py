"""
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import datetime
import os
import csv
import statistics

import numpy as np
import matplotlib.lines
import matplotlib.pyplot as plt
from scipy import interpolate


def get_error(exp_vals, calc_vals, error_limits, units, msg, output_filename, create_output=True, verbose=True):
    """
    Get error statistics for unit tests and print output.

    Parameters
    ----------
    exp_vals : list
        Experimental values to check against

    calc_vals : list
        Hyram generated values

    error_limits : dict
        Error limit values, read from Limits.csv.
        Uses the same format as the return dict

    units : str
        Name of units

    msg : str
        Title string for the unit test

    output_filename : str
        Absolute path to the output text file

    create_output : bool (optional)
        Toggle creating an output text file, defaults to True

    verbose : bool (optional)
        Toggle printing output to the command line, defaults to True

    Returns
    -------
    output : dict
        Dictionary of calculated error values
    """
    # Get error metrics
    errors = []
    percent_errors = []
    for exp, calc in zip(exp_vals, calc_vals):
        errors.append(abs(exp - calc))

        if not (exp == 0 and calc == 0):
            percent_errors.append(100 * abs((exp - calc) / ((abs(exp) + abs(calc)) / 2)))

    max_abs_error = max(errors)
    avg_abs_error = statistics.mean(errors)

    max_percent_error = max(percent_errors)
    avg_percent_error = statistics.mean(percent_errors)
    rsq = np.corrcoef(calc_vals, exp_vals)[0][1]

    # Handle output
    if verbose or create_output:
        timestamp = datetime.datetime.now()
        text = f"--- {msg} [{timestamp}] ---\n"
        abs_text = f"Absolute Error: Max = {max_abs_error:.2f} {units}, Mean = {avg_abs_error:.2f} {units}"
        avg_text = f"Percent Error: Max = {max_percent_error:.2f}%, Mean = {avg_percent_error:.2f}%"
        stat_text = f"Statistics: R Squared = {rsq:.4f}"

        # Use the longest string length to place 'Criteria' text
        length = max([len(abs_text), len(avg_text), len(stat_text)])
        text = text + "{0: <{1}} {2}".format(abs_text, length, f"|Criteria: Max = {error_limits['Max Absolute Error']}, Mean = {error_limits['Avg Absolute Error']}\n")
        text = text + "{0: <{1}} {2}".format(avg_text, length, f"|Criteria: Max = {error_limits['Max Percent Error']}, Mean = {error_limits['Avg Percent Error']}\n")
        text = text + "{0: <{1}} {2}".format(stat_text, length, f"|Criteria: R Squared = {error_limits['R2']}\n\n")

        if verbose:
            print(text)

        if create_output:
            os.makedirs(os.path.dirname(output_filename), exist_ok=True)
            with open(output_filename, 'a') as file:
                file.write(text)
                file.close

    # Return all values as a dict
    output = dict()
    output['Max Absolute Error'] = max_abs_error
    output['Avg Absolute Error'] = avg_abs_error
    output['Max Percent Error'] = max_percent_error
    output['Avg Percent Error'] = avg_percent_error
    output['R2'] = rsq
    output['SMAPE'] = percent_errors

    return output

def create_basic_plot(x, exp_y, calc_y, filename, title, xlabel, ylabel, xlim, ylim, axis_scale, draw_line):
    """
    Creates a basic scatter/line plot.

    Parameters
    ----------
    x : list
        X-axis values, shared by both experimental and calculated data

    exp_y : list
        Experimental y values

    calc_y : list
        HyRAM-calculated y values

    filename : str
        Absolute path to the output pdf file

    title : str
        Title of the graph

    xlabel : str
        Graph's x-axis label

    ylabel : str
        Graph's y-axis label

    xlim : None or list
        2-element list of [lower bound, upper bound] that sets the x-axis bounds of the graph. Defaults to None, which uses automatic scaling

    ylim : None or list
        2-element list of [lower bound, upper bound] that sets the y-axis bounds of the graph. Defaults to None, which uses automatic scaling

    axis_scale : None or str
        Scale transformation of the graph, e.g. 'log', 'symlog', 'logit'. Defaults to None, which applies linear scaling

    draw_line : bool
        Toggles drawing a line between HyRAM data points. Defaults to True
    """
    fig, ax = plt.subplots(figsize=(6, 6))
    ax.plot(x, exp_y, 'go', markersize=4, label="Experimental Data")

    if draw_line:
        ax.plot(x, calc_y, 'bo-', markersize=4, lw=2, label="HyRAM Data")
    else:
        ax.plot(x, calc_y, 'bo', markersize=4, lw=2, label="HyRAM Data")

    ax.legend(fontsize=10)
    ax.minorticks_on()
    ax.grid(which='major', alpha=.5)
    ax.grid(which='minor', alpha=.2)
    ax.set_title(title)
    ax.set_xlabel(xlabel)
    ax.set_ylabel(ylabel)

    if xlim:
        ax.set_xlim(xlim[0], xlim[1])
    if ylim:
        ax.set_ylim(ylim[0], ylim[1])
    if axis_scale:
        ax.set_xscale(axis_scale)
        ax.set_yscale(axis_scale)

    os.makedirs(os.path.dirname(filename), exist_ok=True)
    fig.savefig(filename, bbox_inches='tight')

def create_error_plot(x, exp_y, calc_y, max_error, avg_error, filename, title, xlabel, ylabel, xlim, ylim, axis_scale, draw_line):
    """
    Creates the first error plot for validation tests.
    Uses absolute error values to compare data point by point.

    Parameters
    ----------
    x : list
        X-axis values, shared by both experimental and calculated data

    exp_y : list
        Experimental y values

    calc_y : list
        HyRAM-calculated y values

    max_error : float
        Maximum absolute error value

    avg_error : float
        Average absolute error value

    filename : str
        Absolute path to the output pdf file

    title : str
        Title of the graph

    xlabel : str
        Graph's x-axis label

    ylabel : str
        Graph's y-axis label

    xlim : None or list
        2-element list of [lower bound, upper bound] that sets the x-axis bounds of the graph. Defaults to None, which uses automatic scaling

    ylim : None or list
        2-element list of [lower bound, upper bound] that sets the y-axis bounds of the graph. Defaults to None, which uses automatic scaling

    axis_scale : None or str
        Scale transformation of the graph, e.g. 'log', 'symlog', 'logit'. Defaults to None, which applies linear scaling

    draw_line : bool
        Toggles drawing a line between HyRAM data points. Defaults to True

    Returns
    -------
    None
    """
    # Set up the plot, add experimental and calculated datapoints
    fig, ax = plt.subplots(figsize=(6, 6))
    ax.plot(x, exp_y, 'go', markersize=4, label="Experimental Data")

    if draw_line:
        ax.plot(x, calc_y, 'bo-', markersize=4, lw=2, label="HyRAM Data")
    else:
        ax.plot(x, calc_y, 'bo', markersize=4, lw=2, label="HyRAM Data")

    # Add error bars and add to legend
    ax.errorbar(x, exp_y, yerr=max_error, fmt='none', color='k', capsize=3, alpha=0.75)
    ax.errorbar(x, exp_y, yerr=avg_error, fmt='none', elinewidth=5, color='k', alpha=0.25)

    legend_elements, _ = ax.get_legend_handles_labels()
    legend_elements.append(matplotlib.lines.Line2D([], [], color = 'k', marker='|', linestyle='None', markersize=10, alpha=0.75, label='Max Error'))
    legend_elements.append(matplotlib.lines.Line2D([], [], color = 'k', marker='|', linestyle='None', markersize=10, markeredgewidth=5, alpha=0.25, label='Average Error'))

    # Finalize plot
    ax.legend(handles=legend_elements, fontsize=10)
    ax.minorticks_on()
    ax.grid(which='major', alpha=.5)
    ax.grid(which='minor', alpha=.2)
    ax.set_title(title)
    ax.set_xlabel(xlabel)
    ax.set_ylabel(ylabel)

    if xlim:
        ax.set_xlim(xlim[0], xlim[1])
    if ylim:
        ax.set_ylim(ylim[0], ylim[1])
    if axis_scale:
        ax.set_xscale(axis_scale)
        ax.set_yscale(axis_scale)

    os.makedirs(os.path.dirname(filename), exist_ok=True)
    fig.savefig(filename, bbox_inches='tight')


def create_residual_plot(x, exp_y, calc_y, filename, title, xlabel):
    """
    Creates the third error plot for validation tests.
    Displays a scatter plot of residual values.

    Parameters
    ----------
    x : list
        X-axis values, shared between experimental and calculated y's

    exp_y : list
        Experimental y values

    calc_y : list
        HyRAM-calculated y values

    filename : os.path
        Absolute path to the output pdf file

    title : str
        Title of the graph

    xlabel : str
        Graph's x-axis label

    Returns
    -------
    None
    """
    residuals = [y0 - y1 for y0, y1 in zip(exp_y, calc_y)]

    # Create plot
    fig, ax = plt.subplots(figsize=(6, 6))
    ax.plot(x, residuals, 'bo', markersize=4, label="Residual")
    ax.axhline(y=0, color="black", linestyle="--", alpha=0.5)

    ax.legend(fontsize=10)
    ax.minorticks_on()
    ax.grid(which='major', alpha=.5)
    ax.grid(which='minor', alpha=.2)
    ax.set_title(title)
    ax.set_xlabel(xlabel)
    ax.set_ylabel('Residual')

    os.makedirs(os.path.dirname(filename), exist_ok=True)
    fig.savefig(filename, bbox_inches='tight')


def create_smape_plot(x, smape, filename, title, xlabel):
    """
    Creates the fourth error plot for validation tests.
    Displays a scatter plot of residual values.

    Parameters
    ----------
    x : list
        X-axis values, shared between experimental and calculated y's

    exp_y : list
        Experimental y values

    calc_y : list
        HyRAM-calculated y values

    filename : os.path
        Absolute path to the output pdf file

    title : str
        Title of the graph

    xlabel : str
        Graph's x-axis label

    Returns
    -------
    None
    """
    # Create plot
    fig, ax = plt.subplots(figsize=(6, 6))
    ax.plot(x, smape, 'bo', markersize=4, label="SMAPE Value")
    ax.axhline(y=0, color="black", linestyle="--", alpha=0.5)

    ax.legend(fontsize=10)
    ax.minorticks_on()
    ax.grid(which='major', alpha=.5)
    ax.grid(which='minor', alpha=.2)
    ax.set_title(title)
    ax.set_xlabel(xlabel)
    ax.set_ylabel('SMAPE (%)')
    ax.set_ylim(0, 205)

    os.makedirs(os.path.dirname(filename), exist_ok=True)
    fig.savefig(filename, bbox_inches='tight')


def create_plots(output_dir, x, exp_y, calc_y, max_error, avg_error, smape, title, xlabel, ylabel,
                 xlim=None, ylim=None, axis_scale=None, draw_line_eb=True):
    """
    Creates the first error plot for validation tests.
    Uses absolute error values to compare data point by point.

    Parameters
    ----------
    output_dir : os.path
        Location of test's output directory

    x : list
        X-axis values, shared by both experimental and calculated data

    exp_y : list
        Experimental y values

    calc_y : list
        HyRAM-calculated y values

    max_error : float
        Maximum absolute error value

    avg_error : float
        Average absolute error value

    smape : list
        List of SMAPE values for each x value

    title : str
        Title of the graph, also used as the pdf file's name

    xlabel : str
        Graph's x-axis label

    ylabel : str
        Graph's y-axis label

    xlim : None or list
        2-element list of [lower bound, upper bound] that sets the x-axis bounds of the graph. Defaults to None, which uses automatic scaling

    ylim : None or list
        2-element list of [lower bound, upper bound] that sets the y-axis bounds of the graph. Defaults to None, which uses automatic scaling

    axis_scale : None or str
        Scale transformation of the graph, e.g. 'log', 'symlog', 'logit'. Defaults to None, which applies linear scaling

    draw_line_eb : bool
        Toggles drawing a line between HyRAM data points for the errorbar plot. Defaults to True

    Returns
    -------
    None
    """
    # Split title to create subfolders
    split_title = title.split(' - ')

    # Replace '_' characters with '/' for titles displayed on the graph
    title = title.replace('_', '/')

    filename = os.path.join(output_dir, 'Basic Plots', split_title[0], f'{split_title[1]}.pdf')
    create_basic_plot(x=x,
                      exp_y=exp_y,
                      calc_y=calc_y,
                      filename=filename,
                      title=title,
                      xlabel=xlabel,
                      ylabel=ylabel,
                      xlim=xlim,
                      ylim=ylim,
                      axis_scale=axis_scale,
                      draw_line=draw_line_eb)

    filename = os.path.join(output_dir, 'Errorbar Plots', split_title[0], f'{split_title[1]}.pdf')
    create_error_plot(x=x,
                      exp_y=exp_y,
                      calc_y=calc_y,
                      max_error=max_error,
                      avg_error=avg_error,
                      filename=filename,
                      title=title,
                      xlabel=xlabel,
                      ylabel=ylabel,
                      xlim=xlim,
                      ylim=ylim,
                      axis_scale=axis_scale,
                      draw_line=draw_line_eb)

    filename = os.path.join(output_dir, 'Residual Plots', split_title[0], f'{split_title[1]}.pdf')
    create_residual_plot(x=x,
                    exp_y=exp_y,
                    calc_y=calc_y,
                    filename=filename,
                    title=title,
                    xlabel=xlabel)

    filename = os.path.join(output_dir, 'SMAPE Plots', split_title[0], f'{split_title[1]}.pdf')
    create_smape_plot(x=x,
                      smape=smape,
                      filename=filename,
                      title=title,
                      xlabel=xlabel)

def interp_and_trim_data(calculated_x, calculated_y, experiment_x, experiment_y, lower_bound=None, upper_bound=None):
    """
    Cleans up data for error checking and plotting.
    Linearly interpolates experimental data to calculated data points and trims both to range specified by [lower_bound, upper_bound].
    If bounds aren't specified, they are set to the extent of the HyRAM calculated data.

    Parameters
    ----------
    calculated_x : list
        List of HyRAM-calculated x values
    calculated_y : list
        List of HyRAM-calculated y values
    experiment_x : list
        List of experimental x values
    experiment_y : list
        List of experimental y values
    lower_bound : float or None, optional
        Lower bound of the data
    upper_bound : float or None, optional
        Upper bound of the data

    Returns
    -------
    data_out : dict
        Dictionary of calculated and experimental data values. Keys are 'calc_x', 'calc_y', 'exp_x', and 'exp_y'
    """
    data_out = dict()
    sorted_data = sorted(zip(calculated_x, calculated_y))

    # Get bounds if not explicitly defined
    lower = lower_bound if lower_bound else max(min(calculated_x), min(experiment_x))
    upper = upper_bound if upper_bound else min(max(calculated_x), max(experiment_x))

    # Catch if there is no overlap in the data
    if lower > upper:
        upper = max(max(calculated_x), max(experiment_x))
        lower = min(min(calculated_x), min(experiment_x))

    sorted_data = [point for point in sorted_data if point[0] >= lower and point[0] <= upper]
    data_out['calc_x'], data_out['calc_y'] = zip(*sorted_data)

    interp = interpolate.interp1d(experiment_x, experiment_y, fill_value='extrapolate')
    data_out['exp_y'] = interp(data_out['calc_x'])

    return data_out


def read_csv(filename, header):
    """
    Reads in csv files using the Python Standard Lib csv functions.
    Emulates Pandas read_csv function by placing data in a dict keyed by columns.

    Parameters
    ----------
    filename : str
        Absolute path to the csv file

    header : int
        Index of the header row

    Returns
    -------
    data : dict
        Dictionary of the data. Keys are set to column names
    """
    data = dict()

    # Stores headers for index/key relationship
    headers = []

    with open(filename, newline='') as file:
        reader = csv.reader(file)

        for idx, row in enumerate(reader):
            # Skip rows before header
            if idx < header:
                continue

            # Set keys from the header row
            if idx == header:
                for key in row:
                    data[key] = []
                    headers.append(key)
            else:
                for i, val in enumerate(row):
                    # Catch "na" values and handle strings
                    if val != '':
                        try:
                            val = float(val)
                        except:
                            val = str(val)

                        data[headers[i]].append(val)

    return data