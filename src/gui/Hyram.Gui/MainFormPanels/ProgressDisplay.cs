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
using System.Diagnostics;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class ProgressDisplay : UserControl
    {
        public ProgressDisplay()
        {
            ContentPanel.SetNarrative(this);
            InitializeComponent();
            Load += ProgressDisplay_Load;
        }

        private void ProgressDisplay_Load(object sender, EventArgs e)
        {
            MainProgressBar.Visible = true;
            //MainProgressBar.Style = ProgressBarStyle.Continuous;
            //MainProgressBar.Value = 10;
        }

        public void UpdateProgress(int value, string msg)
        {
            Debug.WriteLine("Update: " + value + ", " + msg);
            MainProgressLabel.Text = msg;

            if (value == 100)
            {
                MainProgressBar.Style = ProgressBarStyle.Continuous;
                MainProgressBar.Value = 100;
            }
            else if (value == -1)
            {
                MainProgressBar.Style = ProgressBarStyle.Continuous;
                MainProgressBar.Value = 0;
            }
        }

        public int GetStatus()
        {
            return MainProgressBar.Value;
        }
    }
}