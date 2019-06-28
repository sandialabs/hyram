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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace UIHelpers
{
    public partial class AnyEnumComboSelector : UserControl
    {
        private Dictionary<Enum, int> _mIndexLookup;

        private Dictionary<string, Enum> _mSelectedEnumValueLookup;

        public AnyEnumComboSelector()
        {
            InitializeComponent();
        }

        [Browsable(true)]
        public string Caption
        {
            get => lblCaption.Text;
            set => lblCaption.Text = value;
        }

        public Enum SelectedItem
        {
            get
            {
                if (DesignMode)
                    return null;
                return _mSelectedEnumValueLookup[(string) cbEnums.SelectedItem];
            }
            set
            {
                if (!DesignMode && value != null) cbEnums.SelectedIndex = _mIndexLookup[value];
            }
        }

        [Browsable(true)] public event EventHandler OnValueChanged;

        // To specify display values different from the values that can
        // be extracted from an enumerated type, use the longer Fill() routine.
        public void Fill(Enum defaultValue)
        {
            var theItems = defaultValue.GetType().GetEnumValues();
            var displayNames = GetEnumTerseDisplayNames(theItems);
            var enumsAndDisplayTextTable = new Dictionary<string, Enum>();

            for (var itemIndex = 0; itemIndex < theItems.Length; itemIndex++)
                enumsAndDisplayTextTable.Add(displayNames[itemIndex], (Enum) theItems.GetValue(itemIndex));

            Fill(enumsAndDisplayTextTable, defaultValue);
        }

        public void Fill(Dictionary<string, Enum> enumsAndDisplayTextTable, Enum defaultValue)
        {
            _mSelectedEnumValueLookup = enumsAndDisplayTextTable;


            var displayValues = _mSelectedEnumValueLookup.Keys.ToArray();
            var enumValues = _mSelectedEnumValueLookup.Values.ToArray();

            FillIndexLookupCollection(enumValues);

            cbEnums.Items.Clear();
            cbEnums.Items.AddRange(displayValues);
            SelectedItem = defaultValue;
        }

        private string[] GetEnumTerseDisplayNames(Array theItems)
        {
            var result = new string[theItems.Length];
            for (var index = 0; index < theItems.Length; index++) result[index] = theItems.GetValue(index).ToString();

            return result;
        }


        private void FillIndexLookupCollection(Enum[] newItems)
        {
            _mIndexLookup = new Dictionary<Enum, int>();
            for (var index = 0; index < newItems.Length; index++) _mIndexLookup.Add(newItems[index], index);
        }

        private void cbEnums_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DesignMode) return;

            var enumKey = (string) cbEnums.SelectedItem;
            var enumValue = _mSelectedEnumValueLookup[enumKey];

            if (OnValueChanged != null) OnValueChanged(this, new EventArgs());
        }
    }
}