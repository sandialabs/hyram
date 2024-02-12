/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Windows.Forms;

//using JrWindowsAPI;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class Splashscreen : Form
    {
        public Splashscreen()
        {
            InitializeComponent();
        }

        public void FadeOut()
        {
            FadeOutTime.Enabled = true;
        }

        private void FadeOutTime_Tick(object sender, EventArgs e)
        {
            if (Opacity > 0.05)
            {
                Opacity -= .05;
            }
            else
            {
                FadeOutTime.Enabled = false;
                Hide();
                Enabled = false;
            }
        }
    }
}