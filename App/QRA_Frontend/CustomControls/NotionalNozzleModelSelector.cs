// Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
// Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
// 
// This file is part of HyRAM (Hydrogen Risk Assessment Models).
// 
// HyRAM is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// HyRAM is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;
using QRAState;

namespace QRA_Frontend.CustomControls
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
            for (var nmsIndex = 0; nmsIndex < cbNotionalNozzleModel.Items.Count - 1; nmsIndex++)
                if ((string) cbNotionalNozzleModel.Items[nmsIndex] == nozzleName)
                {
                    cbNotionalNozzleModel.SelectedIndex = nmsIndex;
                    break;
                }
        }

        public event EventHandler OnNotionalNozzleModelChanged;

        private void SpawnNotionalNozzleModelChangedEvent()
        {
            if (DesignMode) return;

            OnNotionalNozzleModelChanged?.Invoke(this, new EventArgs());
        }

        private void cbNotionalNozzleModel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DesignMode) return;

            var newModel = NozzleModel.ParseNozzleModelName(cbNotionalNozzleModel.SelectedItem.ToString());

            QraStateContainer.SetValue("NozzleModel", newModel);
            //QraStateContainer.Instance.SetNozzleModel((string)cbNotionalNozzleModel.SelectedItem);
            var oldValue = _mNozzleModelSelected;

            //string NozzleName = QraStateContainer.Instance.GetObject("NotionalNozzleModel").ToString();
            //mNozzleModelSelected = QraStateContainer.Instance.GetNozzleModel();
            var oldModel = QraStateContainer.GetValue<NozzleModel>("NozzleModel");

            if (_mNozzleModelSelected != oldValue) SpawnNotionalNozzleModelChangedEvent();
        }

        private void ReadFromGlobalDataCollectionAndSet()
        {
            if (DesignMode) return;
            //UIStateRoutines.SetSelectedDropdownValue(cbNotionalNozzleModel, (string)QraStateContainer.Instance.GlobalData["NozzleModel"]);
            UiStateRoutines.SetSelectedDropdownValue(cbNotionalNozzleModel,
                QraStateContainer.GetObject("NozzleModel").ToString());
        }
    }
}