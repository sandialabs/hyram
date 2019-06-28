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
using JrConversions;
using QRA_Frontend.Resources;
using QRAState;

namespace QRA_Frontend.ContentPanels
{
    // See cpcExplosionScenario2.cs for screen code.
    public partial class CpExplosionScenario2 : UserControl, IQraBaseNotify
    {
        public CpExplosionScenario2()
        {
            InitializeComponent();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            tcIO.SelectedTab = tpOutput;
        }


        private void tbLength_TextChanged(object sender, EventArgs e)
        {
            StaticUiHelperRoutines.DistTextboxValueChanged(tbLength, ref _mLength);
        }

        private void tbHeight_TextChanged(object sender, EventArgs e)
        {
            StaticUiHelperRoutines.DistTextboxValueChanged(tbHeight, ref _mHeight);
        }

        private void tbWidth_TextChanged(object sender, EventArgs e)
        {
            StaticUiHelperRoutines.DistTextboxValueChanged(tbWidth, ref _mWidth);
        }

        #region IQraBaseNotify Members

        void IQraBaseNotify.Notify_LoadComplete()
        {
            tcIO.SelectedTab = tpInput;
            SetupUi();
            InitializeFromDefaults();
        }

        private NdConvertibleValue _mLength;
        private NdConvertibleValue _mWidth;
        private NdConvertibleValue _mHeight;
        private NdConvertibleValue _mExhaustVentilationRate;
        private NdConvertibleValue _mMakeupAirRate;

        private void SetupUi()
        {
            ContentPanel.SetNarrative(this, Narratives.EX__HydrogenDeflagration);
        }


        private void InitializeFromDefaults()
        {
            StaticUiHelperRoutines.SetDistanceTextBox(tbLength, ref _mLength, "Length");
            StaticUiHelperRoutines.SetDistanceTextBox(tbWidth, ref _mWidth, "Width");
            StaticUiHelperRoutines.SetDistanceTextBox(tbHeight, ref _mHeight, "Height");
            StaticUiHelperRoutines.SetUnitlessTextBox(tbExhaustVentilationRate, ref _mExhaustVentilationRate,
                "ExhaustVentilationRate");
            StaticUiHelperRoutines.SetUnitlessTextBox(tbMakeupAirRate, ref _mMakeupAirRate, "MakeupAirRate");
        }

        #endregion
    }
}