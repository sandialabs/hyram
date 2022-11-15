/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class UiHelpers
    {
        public static void UnselectButtons(Control parentControl)
        {
            foreach (Control thisControl in parentControl.Controls)
            {
                if (thisControl.HasChildren)
                {
                    UnselectButtons(thisControl);
                }

                UnselectButton(thisControl);
            }
        }

        public static void UnselectButton(Button button)
        {
            button.ForeColor = Color.Black;
        }

        public static void UnselectButton(Control control)
        {
            if (control is Button)
            {
                UnselectButton((Button) control);
            }
        }

        public static void ShowButtonActive(Button button)
        {
            button.ForeColor = Color.Green;
        }
    }
}