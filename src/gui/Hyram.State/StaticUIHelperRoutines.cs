/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class StaticUiHelperRoutines
    {
        public static void UnitlessTextBoxValueChanged(TextBox sender, ref ConvertibleValue valueObj)
        {
            var testValue = double.NaN;

            if (ParseUtility.TryParseDouble(sender.Text, out testValue))
            {
                var newValue = new double[1];
                newValue[0] = testValue;
                valueObj.SetValue(UnitlessUnit.Unitless.ToString(), newValue);
                if (sender.ForeColor != Color.Black) sender.ForeColor = Color.Black;
            }
            else
            {
                if (sender.ForeColor != Color.Red) sender.ForeColor = Color.Red;
            }
        }
    }
}