/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

namespace SandiaNationalLaboratories.Hyram
{

    /// <summary>
    /// Convenience class holding notification strings.
    /// </summary>
    public static class Notifications
    {

        public static string ReleasePressureInvalid()
        {
            FuelType fuel = State.Data.GetActiveFuel();
            double criticalP = fuel.GetCriticalPressureMpa();
            string msg = $"Saturated release pressure must be between ambient pressure and critical pressure ({criticalP} MPa)";
            return msg;
        }

        public const string QraNonDefaultPureFluidWarning = "QRA defaults developed for hydrogen, methane, and propane and may not be suitable for other fuels";

        public const string QraBlendWarning = "QRA defaults developed for hydrogen, methane,and propane are applied in a conservative manner for blends";

        public const string DispenserNonDefaultFluidWarning = "Default data for dispenser failures were generated for high pressure gaseous hydrogen systems and may not be appropriate for the selected fuel";

    }

}
