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
                ChildPane.Controls.Add(_mChildControl);
                _mChildControl.Dock = DockStyle.Fill;
            }
        }

        public static void SetNarrative(Control childControl, string resourcePath = null)
        {
            if (resourcePath != null)
            {
                var cp = GetContentPanel(childControl);
                cp.SetNarrative(resourcePath);
            }
        }

        private static ContentPanel GetContentPanel(Control childControl)
        {
            ContentPanel result = null;

            if (childControl is ContentPanel)
                result = (ContentPanel) childControl;
            else
                result = (ContentPanel) childControl.Parent.Parent;

            return result;
        }


        public void SetNarrative(string narrString)
        {
            tbNarrative.BackColor = BackColor;
            tbNarrative.Rtf = narrString;
            var narrativeVisible = tbNarrative.Text.Length > 0;
            if (tbNarrative.Visible != narrativeVisible) tbNarrative.Visible = narrativeVisible;
        }


        private void _ContentPanel_Load(object sender, EventArgs e)
        {
            SetNarrative(this);
        }
    }
}