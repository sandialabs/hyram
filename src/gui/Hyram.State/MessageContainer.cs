/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

namespace SandiaNationalLaboratories.Hyram
{

    public static class MessageContainer
    {
        //public const string LiquidReleasePressureInvalid =
            //"Saturated release pressure must be between ambient pressure and critical pressure (1.29 MPa)";

        //public const string FuelFlowChoked = "Fuel flow is choked; release pressure is less than critical ratio * ambient pressure";

        public static string GetAlertMessageReleasePressureInvalid()
        {
            FuelType fuel = StateContainer.GetValue<FuelType>("FuelType");
            double criticalP = fuel.GetCriticalPressureMpa();
            string msg = $"Saturated release pressure must be between ambient pressure and critical pressure ({criticalP} MPa)";
            return msg;
        }
    }

}
