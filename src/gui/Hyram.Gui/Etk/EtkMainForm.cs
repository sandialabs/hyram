/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class EtkMainForm : Form
    {
        public EtkMainForm()
        {
            InitializeComponent();
        }

        private void EtkMainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void tankMassTabPage_Enter(object sender, System.EventArgs e)
        {
            tankMassForm.EnteringForm();
        }

        private void massFlowRateTabPage_Enter(object sender, System.EventArgs e)
        {
            massFlowRateForm.EnteringForm();
        }
    }
}