/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class ContentPanel : UserControl
    {
        private static UserControl _mChildControl;

        public ContentPanel()
        {
            InitializeComponent();
        }

        public UserControl ChildControl
        {
            get => _mChildControl;
            set
            {
                _mChildControl = value;
                ChildPane.Controls.Clear();
                ChildPane.Controls.Add(_mChildControl);
                _mChildControl.Dock = DockStyle.Fill;
            }
        }

        public void UpdateNarrative(string narrString = null)
        {
            if (narrString != null)
            {
                tbNarrative.BackColor = BackColor;
                tbNarrative.Rtf = narrString;
                tbNarrative.Visible = tbNarrative.Text.Length > 0;
            }
            else
            {
                tbNarrative.Text = "";
            }
        }


        private void _ContentPanel_Load(object sender, EventArgs e)
        {
        }

        private void tbNarrative_Enter(object sender, EventArgs e)
        {
            // Disable interaction with narrative area to get rid of blinking cursor
            ActiveControl = ChildControl;
        }
    }
}