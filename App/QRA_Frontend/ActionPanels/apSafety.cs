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
using QRA_Frontend.ContentPanels;

namespace QRA_Frontend.ActionPanels
{
    public partial class ApSafety : UserControl, IQraBaseNotify
    {
        public ApSafety()
        {
            InitializeComponent();
        }


        void IQraBaseNotify.Notify_LoadComplete()
        {
        }


        private void btnScenario1_Click(object sender, EventArgs e)
        {
            try
            {
                ActionUtils.SetContentScreen((Button) sender, new CpSafetyScenario1());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Action failed with this error: " + ex.Message);
            }
        }


        private void btnScenario2_Click(object sender, EventArgs e)
        {
            try
            {
                ActionUtils.SetContentScreen((Button) sender, new CpSafetyScenario2());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Action failed with this error: " + ex.Message);
            }
        }
    }
}