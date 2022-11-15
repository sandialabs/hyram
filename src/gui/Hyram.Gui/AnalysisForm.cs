/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    /// Generic persisted form for QRA. Form is kept alive after user navigates away.
    /// Constructor should call RefreshForm.
    /// </summary>
    public class AnalysisForm : UserControl
    {
        public string AlertMessage { get; set; } = "";
        public AlertLevel Alert { get; set; } = AlertLevel.AlertNull;

        protected bool _ignoreChangeEvents;

        // Keeps reference to parent to notify of state change; could change this to event.
        protected MainForm MainForm;

        // Updates state-based data, such as data sources. Should call CheckFormValid at end.
        public virtual void RefreshForm()
        {
        }

        public virtual void CheckFormValid()
        {
        }

    }
}
