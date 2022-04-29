/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class NotionalNozzleModelSelector : UserControl
    {
        private NozzleModel _mNozzleModelSelected = NozzleModel.Birch2;

        public NotionalNozzleModelSelector()
        {
            InitializeComponent();
            if (!DesignMode) ReadFromGlobalDataCollectionAndSet();
        }

        public bool CanChange { get; set; } = false;

        public NozzleModel GetValue()
        {
            return _mNozzleModelSelected;
        }

        public void SetValue(NozzleModel value)
        {
            if (DesignMode) return;
            _mNozzleModelSelected = value;
            var nozzleName = _mNozzleModelSelected.ToString();
            for (var nmsIndex = 0; nmsIndex < notionalNozzleSelector.Items.Count - 1; nmsIndex++)
                if ((string) notionalNozzleSelector.Items[nmsIndex] == nozzleName)
                {
                    notionalNozzleSelector.SelectedIndex = nmsIndex;
                    break;
                }
        }

        public event EventHandler OnNotionalNozzleModelChanged;

        private void SpawnNotionalNozzleModelChangedEvent()
        {
            if (DesignMode) return;

            OnNotionalNozzleModelChanged?.Invoke(this, new EventArgs());
        }

        private void notionalNozzleSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DesignMode) return;

            var newModel = NozzleModel.ParseNozzleModelName(notionalNozzleSelector.SelectedItem.ToString());

            StateContainer.SetValue("NozzleModel", newModel);
            //StateContainer.Instance.SetNozzleModel((string)notionalNozzleSelector.SelectedItem);
            var oldValue = _mNozzleModelSelected;

            //string NozzleName = StateContainer.Instance.GetObject("NotionalNozzleModel").ToString();
            //mNozzleModelSelected = StateContainer.Instance.GetNozzleModel();
            var oldModel = StateContainer.GetValue<NozzleModel>("NozzleModel");

            if (_mNozzleModelSelected != oldValue) SpawnNotionalNozzleModelChangedEvent();
        }

        private void ReadFromGlobalDataCollectionAndSet()
        {
            if (DesignMode) return;
            //UIStateRoutines.SetSelectedDropdownValue(notionalNozzleSelector, (string)StateContainer.Instance.GlobalData["NozzleModel"]);
            UiStateRoutines.SetSelectedDropdownValue(notionalNozzleSelector,
                StateContainer.GetObject("NozzleModel").ToString());
        }
    }
}