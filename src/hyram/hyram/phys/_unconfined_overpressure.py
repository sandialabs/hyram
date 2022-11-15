"""
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.

You should have received a copy of the GNU General Public License along with HyRAM+.
If not, see https://www.gnu.org/licenses/.
"""

import os

import numpy as np
import scipy as sp

from ._fuel_props import FuelProperties
from ._plots import plot_sliced_contour
from ._utils import get_distance_to_effect
from . import _overpressure_data as opdata

class Generic_overpressure_method:
    """
    Generic unconfined overpressure class

    Contains attributes and methods common to other
    unconfined overpressure sub-classes
    """
    data_dir = os.path.join(os.path.dirname(__file__), 'data')

    def __init__(self, jet_object, heat_of_combustion=None, flammability_limits=None, origin_at_orifice=False):
        self.jet_object = jet_object
        self.heat_of_combustion = heat_of_combustion
        self.set_ambient_pressure()
        self.set_fuel_properties()
        self.set_flammable_mass(flammability_limits)
        self.set_overpressure_origin(origin_at_orifice)

    def set_ambient_pressure(self) -> None:
        self.ambient_pressure = self.jet_object.ambient.P  # Pa

    def set_fuel_properties(self) -> None:
        fuel_props = FuelProperties(self.jet_object.fluid.species)
        self.species = fuel_props.species
        nC = fuel_props.nC  # number of carbons (nC)
        # Mole fraction of fuel to mole fraction of air at stoichiometric condition
        self.fuel_to_air_stoich_ratio = 1/(4.76*(1+3*nC)/2)
        self.set_flammability_limits(fuel_props)
        self.set_heat_of_combustion(fuel_props)

    def set_flammability_limits(self, fuel_props) -> None:
        self.molar_lower_flammability_limit = fuel_props.LFL
        self.molar_upper_flammability_limit = fuel_props.UFL

    def set_flammable_mass(self, flammability_limits=None) -> None:
        if flammability_limits is not None:
            lower_flammability_limit = flammability_limits[0]
            upper_flammability_limit = flammability_limits[1]
        else:
            lower_flammability_limit = self.molar_lower_flammability_limit
            upper_flammability_limit = self.molar_upper_flammability_limit
        self.flammable_mass = self.jet_object.m_flammable(lower_flammability_limit,
                                                          upper_flammability_limit) # kg

    def set_heat_of_combustion(self, fuel_props) -> None:
        if self.heat_of_combustion == None:
            self.heat_of_combustion = fuel_props.dHc  # J/kg

    def set_overpressure_origin(self, origin_at_orifice:bool) -> None:
        if origin_at_orifice:
            self.origin = (0, 0, 0)
        else:
            # Overpressure-origin is the point at which the concentration
            # is mid-way between the lower and upper flamability limits
            LFL = self.molar_lower_flammability_limit
            UFL = self.molar_upper_flammability_limit
            mid_flammability = LFL + (UFL - LFL) / 2

            # Get jet streamline coordinate based on centerline concentration
            s_coord = np.interp(mid_flammability,
                                self.jet_object.X_cl[::-1],
                                self.jet_object.S[::-1])

            # Get x and y coordinates from jet based on streamline coordinate
            jet_x = np.interp(s_coord, self.jet_object.S, self.jet_object.x)
            jet_y = np.interp(s_coord, self.jet_object.S, self.jet_object.y)

            self.origin = (jet_x, jet_y, 0)

    @staticmethod
    def calc_distance(locations:list, origin) -> list:
        '''
        locations : list of locations
            List of locations of interest,
            each location is a tuple of 3 coordinates (in meters):
            [(x1, y1, z1), (x2, y2, z2), ...]
        origin : tuple or list
            origin location (x, y, z) of the overpressure event, in meters
        '''
        locations = np.array(locations)
        origin = np.array(origin)
        if len(locations) > 0:
            distance = np.linalg.norm(locations - origin, axis=1)
        else:
            distance = []
        return distance

    def calc_overpressure(self, locations:list) -> list:
        """
        Calculate overpressure

        Parameters
        ----------
        locations : list of locations
            List of locations at which to determine overpressure,
            each location is a tuple of 3 coordinates (in meters):
            [(x1, y1, z1), (x2, y2, z2), ...]

        Returns
        -------
        overpressure, float
            Overpressure in Pa
        """
        distance = self.calc_distance(locations=locations, origin=self.origin)
        scaled_distance = self.calc_scaled_distance(distance=distance)
        scaled_overpressure = self.get_scaled_overpressure(scaled_distance=scaled_distance)
        overpressure = self.calc_unscaled_overpressure(scaled_overpressure=scaled_overpressure)  # Pa
        return overpressure

    def calc_scaled_distance(self, distance):
        # Placeholder method; this will be over-written by each sub-class below
        scaled_distance = np.empty(len(distance))
        scaled_distance.fill(np.nan)
        return scaled_distance

    def get_scaled_overpressure(self, scaled_distance):
        # Placeholder method; this will be over-written by each sub-class below
        scaled_overpressure = np.empty(len(scaled_distance))
        scaled_overpressure.fill(np.nan)
        return scaled_overpressure

    def calc_unscaled_overpressure(self, scaled_overpressure):
        unscaled_overpressure = scaled_overpressure * self.ambient_pressure  # Pa
        return unscaled_overpressure

    def calc_impulse(self, locations:list) -> list:
        """
        Calculate impulse

        Parameters
        ----------
        locations : list of locations
            List of locations at which to determine impulse,
            each location is a tuple of 3 coordinates (in meters):
            [(x1, y1, z1), (x2, y2, z2), ...]

        Returns
        -------
        impulse, float
            Impulse in Pa*s
        """
        distance = self.calc_distance(locations=locations, origin=self.origin)
        scaled_distance = self.calc_scaled_distance(distance=distance)
        scaled_impulse = self.get_scaled_impulse(scaled_distance=scaled_distance)
        impulse = self.calc_unscaled_impulse(scaled_impulse=scaled_impulse)
        return impulse

    def get_scaled_impulse(self, scaled_distance):
        # Placeholder method; this will be over-written by each sub-class below
        scaled_impulse = np.empty(len(scaled_distance))
        scaled_impulse.fill(np.nan)
        return scaled_impulse

    def calc_unscaled_impulse(self, scaled_impulse):
        # Placeholder method; this will be over-written by each sub-class below
        unscaled_impulse = np.empty(len(scaled_impulse))
        unscaled_impulse.fill(np.nan)
        return unscaled_impulse

    def calculate_overpressure_for_list_of_locations(self, x:np.array, y:np.array, z:np.array) -> np.array:
        evaluation_locations = []
        x_y_shape = x.shape
        for x_value, y_value, z_location in zip(x.reshape(-1), y.reshape(-1), z.reshape(-1)):
            evaluation_locations += [np.array([x_value, y_value, z_location])]
        overpressures = self.calc_overpressure(evaluation_locations)
        overpressures = overpressures.reshape(x_y_shape)
        return overpressures

    def calculate_impulse_for_list_of_locations(self, x:np.array, y:np.array, z:np.array) -> np.array:
        evaluation_locations = []
        x_y_shape = x.shape
        for x_value, y_value, z_location in zip(x.reshape(-1), y.reshape(-1), z.reshape(-1)):
            evaluation_locations += [np.array([x_value, y_value, z_location])]
        impulses = self.calc_impulse(evaluation_locations)
        impulses = impulses.reshape(x_y_shape)
        return impulses

    def calc_distance_to_overpressure(self, overpressure, direction='x', negative_direction=False):
        """
        Calculate distance from leak point to a given overpressure in the direction specified

        Parameters
        ----------
        overpressure : float
            Overpressure in Pa
        direction : 'x', 'y', or 'z' (optional)
            Direction for which to calculate the distance
            (default is 'x')
        negative_direction : Boolean (optional)
            Whether or not to look in the negative direction instead of positive
            (default is False)

        Returns
        -------
        distance : float
            Real distance (m) to overpressure from leak-point
        """
        scaled_overpressure = self.calc_scaled_overpressure(overpressure=overpressure)  # unitless
        scaled_distance = self.get_scaled_distance_from_scaled_overpressure(scaled_overpressure=scaled_overpressure)
        distance_from_overpressure_origin = self.calc_unscaled_distance(scaled_distance=scaled_distance)  # m
        if direction == 'x':
            distance_from_leakpoint_to_overpressure_origin = self.origin[0]
        elif direction == 'y':
            distance_from_leakpoint_to_overpressure_origin = self.origin[1]
        elif direction == 'z':
            distance_from_leakpoint_to_overpressure_origin = self.origin[2]
        else:
            raise ValueError(f"Direction ('{direction}') must be 'x', 'y', or 'z'")
        if negative_direction:
            distance = distance_from_leakpoint_to_overpressure_origin - distance_from_overpressure_origin
        else:
            distance = distance_from_leakpoint_to_overpressure_origin + distance_from_overpressure_origin
        return distance

    def calc_scaled_overpressure(self, overpressure):
        scaled_overpressure = overpressure / self.ambient_pressure  # unitless
        return scaled_overpressure

    def get_scaled_distance_from_scaled_overpressure(self, scaled_overpressure, max_distance=500):
        # May be re-written by sub-classes below to use single interpolation rather than solver; 
        # currently calculates distance and scales it in order to match syntax of re-written methods below
        overpressure = self.calc_unscaled_overpressure(scaled_overpressure)
        distance = get_distance_to_effect(value=overpressure,
                                          from_point=self.origin,
                                          direction='x',  # does not matter, since overpressure is omni-directional
                                          effect_func=self.calculate_overpressure_for_list_of_locations,
                                          max_distance=max_distance)
        scaled_distance = self.calc_scaled_distance(distance)
        return scaled_distance

    def calc_unscaled_distance(self, scaled_distance):
        # Placeholder method; this may be over-written by each sub-class below
        distance = scaled_distance
        return distance

    def calc_distance_to_impulse(self, impulse, direction='x', negative_direction=False):
        """
        Calculate distance from leak point to a given impulse in the direction specified

        Parameters
        ----------
        impulse : float
            Impulse in Pa*s
        direction : 'x', 'y', or 'z', optional
            Direction for which to calculate the distance
            (default is 'x')
        negative_direction : Boolean (optional)
            Whether or not to look in the negative direction instead of positive
            (default is False)

        Returns
        -------
        distance : float
            Real distance (m) to impulse from leak-point
        """
        scaled_impulse = self.calc_scaled_impulse(impulse=impulse)
        scaled_distance = self.get_scaled_distance_from_scaled_impulse(scaled_impulse=scaled_impulse)
        distance_from_overpressure_origin = self.calc_unscaled_distance(scaled_distance=scaled_distance)  # m
        if direction == 'x':
            distance_from_leakpoint_to_overpressure_origin = self.origin[0]
        elif direction == 'y':
            distance_from_leakpoint_to_overpressure_origin = self.origin[1]
        elif direction == 'z':
            distance_from_leakpoint_to_overpressure_origin = self.origin[2]
        else:
            raise ValueError(f"Direction ('{direction}') must be 'x', 'y', or 'z'")
        if negative_direction:
            distance = distance_from_leakpoint_to_overpressure_origin - distance_from_overpressure_origin
        else:
            distance = distance_from_leakpoint_to_overpressure_origin + distance_from_overpressure_origin
        return distance

    def calc_scaled_impulse(self, impulse):
        # Placeholder method; this will be over-written by sub-classes below
        scaled_impulse = np.full_like(impulse, np.nan)
        return scaled_impulse

    def get_scaled_distance_from_scaled_impulse(self, scaled_impulse):
        # Placeholder method; this will be over-written by sub-classes below
        scaled_distance = np.full_like(scaled_impulse, np.nan)
        return scaled_distance

    def plot_overpressure_sliced(self, title=None,
                                 plot_filename='overpressure_sliced_contour.png',
                                 directory=os.getcwd(),
                                 contours=None,
                                 nx=50, ny=50, nz=50,
                                 xlims=None, ylims=None, zlims=None,
                                 savefig=False):
        '''
        Plots contour slices of overpressure levels

        Parameters
        ----------
        title: string (optional)
            title shown on plot
        plot_filename: string, optional
            file name to write
        directory: string, optional
            directory in which to save file
        contours: ndarray or list (optional)
            contour levels shown on plot (default values are from ICHS paper, safe setback distances)
            default levels correspond to glass breakage (5 kPa), people knocked over with some structural damage (16 kPa),
            and then significant fatalities and structural damage (70 kPa)
        nx, ny, nz: float (optional)
            number of points to solve for the overpressure in the x, y, and z directions
        xlims, ylims, zlims: tuples (optional)
            limits for x, y, and z axes
        savefig: Boolean (optional)
            determines if figure file is saved

        Returns
        -------
        If savefig is True, returns filename corresponding plot.
        If savefig is false, returns fig object.
        '''
        if contours is None:
            contours = [5, 16, 70]  # kPa

        colorbar_label = 'Overpressure [kPa]'

        fig_or_filepath = plot_sliced_contour(contours, xlims, ylims, zlims, nx, ny, nz,
                                              self.calc_distance_to_overpressure,
                                              self.calculate_overpressure_for_list_of_locations,
                                              self.origin, colorbar_label,
                                              title=title, savefig=savefig,
                                              directory=directory, filename=plot_filename)
        return fig_or_filepath

    def plot_impulse_sliced(self, title=None,
                            plot_filename='impulse_sliced_contour.png',
                            directory=os.getcwd(),
                            contours=None,
                            nx=50, ny=50, nz=50,
                            xlims=None, ylims=None, zlims=None,
                            savefig=False):
        '''
        Plots contour slices of impulse levels

        Parameters
        ----------
        title: string (optional)
            title shown on plot
        plot_filename: string, optional
            file name to write
        directory: string, optional
            directory in which to save file
        contours: ndarray or list (optional)
            contour levels to be shown on plot
            (default values are mean to be examples only;
            they are obtained by calculating the impulse value
            that would yield a 1% probability of fatality
            using the TNO Head Impact probit
            for the default peak overpressure values used:
            70, 16, and 5 kPa)
        nx, ny, nz: float (optional)
            number of points to solve for the impulse in the x, y, and z directions
        xlims, ylims, zlims: tuples (optional)
            limits for x, y, and z axes
        savefig: Boolean (optional)
            determines if figure file is saved

        Returns
        -------
        If savefig is True, returns filename corresponding plot.
        If savefig is false, returns fig object.
        '''
        if contours is None:
            contours = [0.13, 0.18, 0.27]  # kPa*s

        colorbar_label = 'Impulse [kPa*s]'

        fig_or_filepath = plot_sliced_contour(contours, xlims, ylims, zlims, nx, ny, nz,
                                              self.calc_distance_to_impulse,
                                              self.calculate_impulse_for_list_of_locations,
                                              self.origin, colorbar_label,
                                              title=title, savefig=savefig,
                                              directory=directory, filename=plot_filename)
        return fig_or_filepath


class BST_method(Generic_overpressure_method):
    """
    Vapor Cloud Explosions using BST Method
    based on flammable mass of fuel

    Parameters
    ----------
    jet_object : hyram Jet object
        Jet object representing fuel leak
    mach_flame_speed : float
        Mach flame speed: 0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2
        Detonation should use 5.2
    heat_of_combustion : float or None
        Heat of combustion of fuel (J/kg)
        Default is None: use default value for selected fuel
    flammability_limits : tuple or None
        Tuple of (lower_flammability_limit, upper_flammability_limit)
        Default is None: use default LFL, UFL for selected fuel
    origin_at_orifice : Boolean
        If True, the origin of the overpressure will be at the orifice
        Default is False: will calculate location of origin,
        the point at which the concentration in the unignited jet
        is mid-way between the default lower and upper flammability limits

    From: CCPS, "Guidelines for Vapor Cloud Explosion, Pressure Vessel Burst, BLEVE, and Flash Fire Hazards",
        Second Edition, Center for Chemical Process Safety, American Institute of Chemical Engineers,
        John Wiley & Sons, Inc.,2010
    """
    speed_of_sound = 340  # m/s

    def __init__(self, jet_object, mach_flame_speed,
                 heat_of_combustion=None, flammability_limits=None, origin_at_orifice=False):
        Generic_overpressure_method.__init__(self, jet_object, heat_of_combustion,
                                             flammability_limits, origin_at_orifice)
        self.set_mach_flame_speed(mach_flame_speed)
        self.scaled_peak_overpressure_data = opdata.scaled_peak_overpressure_data
        self.all_scaled_impulse_data = opdata.all_scaled_impulse_data
        self.energy = self.calc_energy()

    def set_mach_flame_speed(self, mach_flame_speed):
        """
        Change the mach flame speed

        Parameters
        ----------
        mach_flame_speed, float
            Mach flame speed: 0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2
            Detonation should use 5.2
        """
        self.mach_flame_speed = mach_flame_speed

    def calc_energy(self):
        ground_reflection_factor = 2
        energy = ground_reflection_factor * self.flammable_mass * self.heat_of_combustion  # J
        return energy

    def calc_scaled_distance(self, distance):
        scaled_distance = distance / (self.energy / self.ambient_pressure) ** (1/3)  # m/(J/Pa)^(1/3)
        return scaled_distance

    def get_scaled_overpressure(self, scaled_distance):
        scaled_distance_data = self.scaled_peak_overpressure_data['scaled_distance_Mf' + str(self.mach_flame_speed)]
        scaled_overpressure_data = self.scaled_peak_overpressure_data['scaled_overpressure_Mf' + str(self.mach_flame_speed)]
        scaled_overpressure = np.interp(x=scaled_distance,  # m/(J/Pa)^(1/3)
                                        xp=scaled_distance_data,  # m/(J/Pa)^(1/3)
                                        fp=scaled_overpressure_data)  # unitless
        return scaled_overpressure

    def get_scaled_impulse(self, scaled_distance):
        scaled_distance_data = self.all_scaled_impulse_data['scaled_distance_Mf' + str(self.mach_flame_speed)]
        scaled_impulse_data = self.all_scaled_impulse_data['scaled_impulse_Mf' + str(self.mach_flame_speed)]
        scaled_impulse = np.interp(x=scaled_distance,  # m/(J/Pa)^(1/3)
                                   xp=scaled_distance_data,  # m/(J/Pa)^(1/3)
                                   fp=scaled_impulse_data)  # Pa*s*(m/s)/J^(1/3)/Pa^(1/3)
        return scaled_impulse

    def calc_unscaled_impulse(self, scaled_impulse):
        unscaled_impulse = scaled_impulse * self.energy ** (1/3) * self.ambient_pressure ** (2/3) / self.speed_of_sound # Pa*s
        return unscaled_impulse

    def get_scaled_distance_from_scaled_overpressure(self, scaled_overpressure):
        scaled_distance_data = self.scaled_peak_overpressure_data['scaled_distance_Mf' + str(self.mach_flame_speed)]
        scaled_overpressure_data = self.scaled_peak_overpressure_data['scaled_overpressure_Mf' + str(self.mach_flame_speed)]
        scaled_distance = np.interp(x=scaled_overpressure,
                                    xp=scaled_overpressure_data[::-1],
                                    fp=scaled_distance_data[::-1])
        return scaled_distance

    def calc_unscaled_distance(self, scaled_distance):
        unscaled_distance = scaled_distance * (self.energy / self.ambient_pressure) ** (1/3)  # m
        distance = unscaled_distance + self.origin[0]  # account for x-location of origin
        return distance

    def calc_scaled_impulse(self, impulse):
        scaled_impulse = impulse * self.speed_of_sound / self.energy ** (1/3) / self.ambient_pressure ** (2/3)
        return scaled_impulse

    def get_scaled_distance_from_scaled_impulse(self, scaled_impulse):
        scaled_distance_data = self.all_scaled_impulse_data['scaled_distance_Mf' + str(self.mach_flame_speed)]
        scaled_impulse_data = self.all_scaled_impulse_data['scaled_impulse_Mf' + str(self.mach_flame_speed)]
        scaled_distance = np.interp(x=scaled_impulse,
                                    xp=scaled_impulse_data[::-1],
                                    fp=scaled_distance_data[::-1])
        return scaled_distance


class TNT_method(Generic_overpressure_method):
    """
    Vapor Cloud Explosions using TNT Equivalency Method
    based on flammable mass of fuel

    Assumes TNT blast energy of 4.68 MJ/kg

    Parameters
    ----------
    jet_object : HyRAM Jet object
        Jet object representing fuel leak
    equivalence_factor : float
        TNT mass equivalency factor, unitless
        CCPS source recommends a default value of 0.03
    heat_of_combustion : float or None
        Heat of combustion of fuel (J/kg)
        Default is None: use default value for selected fuel
    flammability_limits : tuple or None
        Tuple of (lower_flammability_limit, upper_flammability_limit)
        Default is None: use default LFL, UFL for selected fuel
    origin_at_orifice : Boolean
        If True, the origin of the overpressure will be at the orifice
        Default is False: will calculate location of origin,
        the point at which the concentration in the unignited jet
        is mid-way between the default lower and upper flammability limits

    From: CCPS, "Guidelines for Vapor Cloud Explosion, Pressure Vessel Burst, BLEVE, and Flash Fire Hazards",
        Second Edition, Center for Chemical Process Safety, American Institute of Chemical Engineers,
        John Wiley & Sons, Inc.,2010
    """
    def __init__(self, jet_object, equivalence_factor,
                 heat_of_combustion=None, flammability_limits=None, origin_at_orifice=False):
        Generic_overpressure_method.__init__(self, jet_object, heat_of_combustion,
                                             flammability_limits, origin_at_orifice)
        self.equivalence_factor = equivalence_factor  # unitless
        self.equiv_TNT_mass = self.calc_TNT_equiv_mass(self.equivalence_factor,
                                                       self.flammable_mass,
                                                       self.heat_of_combustion)  # kg
        self.scaled_peak_overP_data = opdata.scaled_peak_overP_data
        self.scaled_impulse_data = opdata.scaled_impulse_data

    @staticmethod
    def calc_TNT_equiv_mass(equivalence_factor, flammable_mass, heat_of_combustion):
        blast_energy_TNT = 4.68e6  # J/kg
        TNT_equiv_mass = equivalence_factor * flammable_mass * heat_of_combustion / blast_energy_TNT  # kg
        return TNT_equiv_mass

    def calc_scaled_distance(self, distance):
        scaled_distance = distance / self.equiv_TNT_mass ** (1/3)  # m/kg^(1/3)
        return scaled_distance

    def get_scaled_overpressure(self, scaled_distance):
        scaled_peak_overP_data = self.scaled_peak_overP_data
        scaled_overpressure = np.interp(x=scaled_distance,  # m/kg^(1/3)
                                        xp=scaled_peak_overP_data['scaled_distance'],  # m/kg^(1/3)
                                        fp=scaled_peak_overP_data['scaled_overpressure'])  # unitless
        return scaled_overpressure

    def get_scaled_impulse(self, scaled_distance):
        scaled_impulse_data = self.scaled_impulse_data
        scaled_impulse = np.interp(x=scaled_distance,  # m/kg^(1/3)
                                   xp=scaled_impulse_data['scaled_distance'],  # m/kg^(1/3)
                                   fp=scaled_impulse_data['scaled_impulse'])  # Pa*s/kg^(1/3)
        return scaled_impulse

    def calc_unscaled_impulse(self, scaled_impulse):
        unscaled_impulse = scaled_impulse * self.equiv_TNT_mass ** (1/3)  # Pa*s
        return unscaled_impulse

    def get_scaled_distance_from_scaled_overpressure(self, scaled_overpressure):
        scaled_peak_overP_data = self.scaled_peak_overP_data
        scaled_distance = np.interp(x=scaled_overpressure,  # unitless
                                    xp=scaled_peak_overP_data['scaled_overpressure'][::-1],
                                    fp=scaled_peak_overP_data['scaled_distance'][::-1])
        return scaled_distance

    def calc_unscaled_distance(self, scaled_distance):
        unscaled_distance = scaled_distance * self.equiv_TNT_mass ** (1/3)  # m
        distance = unscaled_distance + self.origin[0]  # account for x-location of origin
        return distance

    def calc_scaled_impulse(self, impulse):
        scaled_impulse = impulse / self.equiv_TNT_mass ** (1/3)  # Pa*s
        return scaled_impulse

    def get_scaled_distance_from_scaled_impulse(self, scaled_impulse):
        scaled_impulse_data = self.scaled_impulse_data
        scaled_distance = np.interp(x=scaled_impulse,
                                    xp=scaled_impulse_data['scaled_impulse'][::-1],
                                    fp=scaled_impulse_data['scaled_distance'][::-1])
        return scaled_distance


class Bauwens_method(Generic_overpressure_method):
    """
    Performs overpressure calculations - based on work of
    Bauwens and Dorofeev, ICHS 2019 paper ID 279:
    https://hysafe.info/uploads/2019_papers/279.pdf
    Bauwens and Dorofeev, CNF 2020:
    https://doi.org/10.1016/j.combustflame.2020.08.003

    Parameters
    ----------
    jet_object : hyram Jet object
        Jet object representing fuel leak
    heat_of_combustion : float or None
        Heat of combustion of fuel (J/kg)
        Default is None: use default value for selected fuel
    min_streamline_divisions : int
        Minimum number of divisions along streamline
         - increase for increased accuracy -
         normally just uses calculated S values
         (default=50)
    number_radial_divisions : int
        Number of divisions along radius
         - increase for increased accuracy
         (default=50)
    max_cell_gradient : float
        Maximum gradient in cell size across which
        detonations can propagate
        (default=0.1)
    minimum_number_detonable_cell : float
        Minimum number of cells for detonation propagation
        (default=5)
    origin_at_orifice : Boolean
        If True, the origin of the overpressure will be at the orifice
        Default is False: will calculate location of origin,
        the point at which the concentration in the unignited jet
        is mid-way between the default lower and upper flammability limits
    """
    def __init__(self, jet_object, heat_of_combustion=None, min_streamline_divisions=50, number_radial_divisions=50,
                 max_cell_gradient=0.1, minimum_number_detonable_cell=5, origin_at_orifice=False):
        self.jet_object = jet_object
        self.heat_of_combustion = heat_of_combustion  # J/kg
        self.set_ambient_pressure()  # Pa
        self.set_fuel_properties()
        self.set_overpressure_origin(origin_at_orifice)

        streamline_points, streamline_point_indices, streamline_point_interpolated_indices = self.calc_streamline_discretization(min_streamline_divisions)
        radial_coordinate_values, streamline_indice_values = self.calc_radial_and_streamline_meshgrid(number_radial_divisions, streamline_point_interpolated_indices)
        x_coordinate_values, y_coordinate_values = self.calc_spatial_discretization(radial_coordinate_values, streamline_indice_values, streamline_point_indices)
        moleFractionField, massFractionField, densityField = self.get_plume_mixture_properties(streamline_point_indices, streamline_indice_values, radial_coordinate_values)
        detonable_cell_size = self.calc_detonable_cell_size(moleFractionField)
        gradient_cell_size = self.calc_cell_size_gradient(x_coordinate_values, y_coordinate_values, detonable_cell_size)
        number_detonable_cells = self.calc_number_detonable_cells(moleFractionField, radial_coordinate_values, gradient_cell_size, detonable_cell_size, max_cell_gradient)
        self.detonable_mass = self.calc_detonable_mass(massFractionField, number_detonable_cells, minimum_number_detonable_cell, densityField, radial_coordinate_values, streamline_points)  # UNITS ? kg ?
        self.energy = self.calc_energy()

    def calc_streamline_discretization(self, min_streamline_divisions):
        if min_streamline_divisions > len(np.unique(self.jet_object.S)):
            streamline_points = np.logspace(np.log10(self.jet_object.S.min()), np.log10(self.jet_object.S.max()), min_streamline_divisions)
        else:
            streamline_points = np.unique(self.jet_object.S)
        streamline_point_indices = np.arange(len(self.jet_object.S))
        streamline_point_interpolated_indices = np.interp(streamline_points, self.jet_object.S, streamline_point_indices)
        return streamline_points, streamline_point_indices, streamline_point_interpolated_indices

    def calc_spatial_discretization(self, radial_coordinate_values, streamline_indice_values, streamline_point_indices):
        x_coordinate_values = np.interp(streamline_indice_values, streamline_point_indices, self.jet_object.x) + \
            radial_coordinate_values*np.sin(np.interp(streamline_indice_values, streamline_point_indices, self.jet_object.theta))
        y_coordinate_values = np.interp(streamline_indice_values, streamline_point_indices, self.jet_object.y) - \
            radial_coordinate_values*np.cos(np.interp(streamline_indice_values, streamline_point_indices, self.jet_object.theta))
        return x_coordinate_values, y_coordinate_values

    def calc_radial_and_streamline_meshgrid(self, number_radial_divisions, streamline_point_interpolated_indices):
        # Calculates logspaced points around 0 out to np.log10(3*np.max(self.B))
        # poshalf[::-1] just notation for reversing a numpy array
        poshalf = np.logspace(-5, np.log10(3*np.max(self.jet_object.B)), number_radial_divisions)
        radial_values = np.concatenate((-1 * poshalf[::-1], [0], poshalf))
        radial_coordinate_values, streamline_indice_values = np.meshgrid(radial_values, streamline_point_interpolated_indices)
        return radial_coordinate_values, streamline_indice_values

    def get_plume_mixture_properties(self, streamline_point_indices, streamline_coordinate_values, radial_coordinate_values):
        plumeHalfWidth = self.get_plume_halfwidth(streamline_coordinate_values, streamline_point_indices)
        centerlineDensity = self.get_centerline_density(streamline_coordinate_values, streamline_point_indices)
        centerlineMassFraction = self.get_centerline_massfraction(streamline_coordinate_values, streamline_point_indices)

        densityField = self.jet_object.ambient.rho + (centerlineDensity - self.jet_object.ambient.rho)*np.exp(-radial_coordinate_values**2/self.jet_object.lam**2/plumeHalfWidth**2)
        massFractionField = centerlineMassFraction*centerlineDensity*np.exp(-(radial_coordinate_values**2)/((self.jet_object.lam*plumeHalfWidth)**2))/densityField
        molecularWeightField = self.jet_object.ambient.therm.MW*self.jet_object.fluid.therm.MW/(massFractionField*(self.jet_object.ambient.therm.MW - self.jet_object.fluid.therm.MW) + self.jet_object.fluid.therm.MW)
        moleFractionField = massFractionField*molecularWeightField/self.jet_object.fluid.therm.MW

        return moleFractionField, massFractionField, densityField

    def get_plume_halfwidth(self, streamline_coordinate_values, streamline_point_indices):
        return np.interp(streamline_coordinate_values, streamline_point_indices, self.jet_object.B)

    def get_centerline_density(self, streamline_coordinate_values, streamline_point_indices):
        return np.interp(streamline_coordinate_values, streamline_point_indices, self.jet_object.rho_cl)

    def get_centerline_massfraction(self, streamline_coordinate_values, streamline_point_indices):
        return np.interp(streamline_coordinate_values, streamline_point_indices, self.jet_object.Y_cl)

    def calc_number_detonable_cells(self, moleFractionField, radial_coordinate_values, grad_cell_size,
                                    detonable_cell_size, max_cell_gradient):
        number_detonable_cells_1D = sp.integrate.simpson(1/detonable_cell_size, radial_coordinate_values, axis = 1)
        number_detonable_cells = np.zeros_like(detonable_cell_size)
        number_detonable_cells[:] = number_detonable_cells_1D[:, None]
        # check if detonable cells have sufficient mole fraction for detonation and that gradient isn't too large
        criteria1 = moleFractionField <= self.molar_lower_flammability_limit
        criteria2 = moleFractionField >= self.molar_upper_flammability_limit
        criteria3 = grad_cell_size > max_cell_gradient
        number_detonable_cells[criteria1 | criteria2| criteria3] = 0
        return number_detonable_cells

    def calc_cell_size_gradient(self, x_coordinate_values, y_coordinate_values, detonable_cell_size):
        return np.linalg.norm(np.gradient(detonable_cell_size, x_coordinate_values.T[0], y_coordinate_values[0]), axis = 0)

    def calc_detonable_mass(self, massFractionField, number_detonable_cells, minimum_number_detonable_cell, densityField,
                            radial_coordinate_values, streamline_points):
        massFractionField_det = np.copy(massFractionField)
        # determine plume locations where sufficient fuel for detonation or negative (integration only needs radius)
        massFractionField_det[(number_detonable_cells <= minimum_number_detonable_cell) | (radial_coordinate_values < 0)] = 0
        detonableFuelField = densityField*massFractionField_det*2*np.pi*radial_coordinate_values
        detonable_mass = sp.integrate.simpson(sp.integrate.simpson(detonableFuelField, radial_coordinate_values), streamline_points)
        return detonable_mass

    def calc_scaled_distance(self, distance):
        """
        Dimensionless standoff distance

        Parameters
        ----------
        distance, float
            Distance in meters

        Returns
        -------
        dimensionless_distance, float
            dimensionless standoff distance
        """
        # check if detonable mass exists
        condition_list = self.create_dimensionless_distance_condition_list(distance)
        # evaluate piecewise function for dimensionless distance (if conditionlist is true use function, else use inf)
        return np.piecewise(distance, condition_list, [self.dimensionless_distance, np.inf])

    def create_dimensionless_distance_condition_list(self, distance):
        condition1 = self.detonable_mass > np.zeros_like(distance)
        condition2 = self.detonable_mass <= np.zeros_like(distance)
        return [condition1, condition2]

    def dimensionless_distance(self, distance):
        return distance*(self.ambient_pressure/self.energy)**(1/3)

    def calc_energy(self):
        return self.detonable_mass*self.heat_of_combustion

    def get_scaled_overpressure(self, scaled_distance):
        scaled_overpressure = (0.34 / scaled_distance ** (4/3)
                               + 0.062 / scaled_distance ** 2
                               + 0.0033 / scaled_distance ** 3)
        return scaled_overpressure

    def calc_detonable_cell_size(self, moleFractionField):
        equivalence_ratio = moleFractionField/(1 - moleFractionField)/self.fuel_to_air_stoich_ratio
        molar_flammability_limits = np.array([self.molar_lower_flammability_limit, self.molar_upper_flammability_limit])
        (equivalence_ratio_lean_limit,
         equivalence_ratio_rich_limit) = (molar_flammability_limits /
                                         (1-molar_flammability_limits)/self.fuel_to_air_stoich_ratio)
        # check if equiv_ratio is outside of ER region or within it
        condition_list = self.create_cell_size_condition_list(equivalence_ratio, equivalence_ratio_lean_limit, equivalence_ratio_rich_limit)
        # evaluate piecewise function for equiv_ratio where if it is outside of ER range it is set to 1e99 otherwise it
        # Note: if np.inf is used rather than 1e99 - leads to RuntimeWarning in gradient
        detonable_cell_size = np.piecewise(equivalence_ratio,
                                           condition_list,
                                           [1e99, lambda equiv_ratio: self.detonation_cell_size(equiv_ratio)])
        return detonable_cell_size

    @staticmethod
    def create_cell_size_condition_list(equivalence_ratio, equivalence_ratio_lean_limit,
                                        equivalence_ratio_rich_limit):
        mixture_too_lean = equivalence_ratio < equivalence_ratio_lean_limit
        mixture_too_rich = equivalence_ratio > equivalence_ratio_rich_limit
        nonflamable_mixture = mixture_too_lean | mixture_too_rich

        above_lower_flamable_limit = equivalence_ratio >= equivalence_ratio_lean_limit
        below_lower_flamable_limit = equivalence_ratio <= equivalence_ratio_rich_limit
        flamable_mixture = above_lower_flamable_limit & below_lower_flamable_limit
        return [nonflamable_mixture, flamable_mixture]

    def detonation_cell_size(self, equiv_ratio):
        """
        Estimate detonation cell size from the equivalence ratio

        These estimations are done using fitted curves,
        see the Technical Reference Manual for details

        Parameters
        ----------
        equiv_ratio
            Equivalence ratio (unitless)

        Returns
        -------
        cell_size
            Detonation cell size (m)
        """
        dcl_fitted_params = {
            'H2': [2.94771698, -0.16536739, 2.2608031, -1.18064551, 0.45823461],
            'CH4': [5.768321, 1.13938677, 113.36802963, 0, 0],
            'C3H8': [4.44856885, -0.73108257, 5.50526263, 0, 0],
        }
        a, b, c, d, e = dcl_fitted_params[self.species]
        log_equiv_ratio = np.log(equiv_ratio)
        log_cell_size = (a
                         + b * log_equiv_ratio
                         + c * log_equiv_ratio ** 2
                         + d * log_equiv_ratio ** 3
                         + e * log_equiv_ratio ** 4)
        cell_size_mm = np.exp(log_cell_size)
        cell_size = cell_size_mm * sp.constants.milli
        return cell_size


class JallaisOverpressureH2(BST_method):
    """
    Calculate peak overpressure for hydrogen release
    using flammability limits and flame speed
    suggested by Jallais et al.
    https://doi.org/10.1002/prs.11965

    Parameters
    ----------
    jet_object : hyram Jet object
        Jet object representing fuel leak
    origin_at_orifice : Boolean
        If True, the origin of the overpressure will be at the orifice
        Default is False: will calculate location of origin,
        the point at which the concentration in the unignited jet
        is mid-way between the default lower and upper flamability limits
    """
    def __init__(self, jet_object, origin_at_orifice=False):
        if jet_object.fluid.species.lower() not in ['h2', 'hydrogen']:
            raise ValueError('JallaisOverpressureH2 is for hydrogen only')
        heat_of_combustion = None  # will use default heat of combustion value
        flammability_limits = (0.1, 0.75)  # from Jallais et al.
        mass_flow_rate = jet_object.mass_flow_rate
        self.flame_speed = self.calc_flame_speed(mass_flow_rate)
        self.calculated_mach_flame_speed = self.calc_mach_flame_speed(self.flame_speed)
        self.mach_flame_speed_curve = self.get_mach_flame_speed_curve(self.calculated_mach_flame_speed)
        BST_method.__init__(self, jet_object, self.mach_flame_speed_curve,
                            heat_of_combustion, flammability_limits, origin_at_orifice)

    @staticmethod
    def calc_flame_speed(mass_flow_rate):
        # From Jallais et al., for ignition at 30%:
        if mass_flow_rate < 0.5:  # kg/s
            flame_speed = 100  # m/s
        elif mass_flow_rate > 0.5 and mass_flow_rate < 1:  # kg/s
            flame_speed = 140  # m/s
        elif mass_flow_rate > 1 and mass_flow_rate < 10:  # kg/s
            flame_speed = 240  # m/s
        else:
            raise ValueError(f'Invalid jet mass flow rate: {mass_flow_rate}')
        return flame_speed

    def calc_mach_flame_speed(self, flame_speed):
        mach_flame_speed = flame_speed / self.speed_of_sound
        return mach_flame_speed

    @staticmethod
    def get_mach_flame_speed_curve(mach_flame_speed):
        valid_mach_flame_speeds = [0.2, 0.35, 0.7, 1.0, 1.4, 2.0, 3.0, 4.0, 5.2]
        # Get Mach flame speed from valid list that is closest to calculated value
        mach_flame_speed_curve = min(valid_mach_flame_speeds, key=lambda x: abs(x - mach_flame_speed))
        return mach_flame_speed_curve
