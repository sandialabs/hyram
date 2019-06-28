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
using DefaultParsing;
using JrCollections;
using JrConversions;
using QRAState;

namespace QRA_Frontend.ContentPanels
{
    public partial class CpDefaultsDatabase : UserControl, IQraBaseNotify
    {
        public CpDefaultsDatabase()
        {
            InitializeComponent();
        }

        private void FillCombinedDataGrid(DataGridView dgVariable, ClsProperties defaults,
            ClsProperties userSessionValues)
        {
            var keys = defaults.Keys;
            Array.Sort(keys);

            foreach (var thisKey in keys)
            {
                var theName = thisKey;
                var oDefaultValue = QraStateContainer.Instance.Defaults[thisKey];
                var defaultValue = GetValueFromObject(oDefaultValue);
                object oUserValue = null;
                var userValue = "Not set this session";
                if (userSessionValues.ContainsKey(thisKey))
                {
                    oUserValue = QraStateContainer.Instance.Parameters[thisKey];
                    userValue = GetValueFromObject(oUserValue);
                }

                string[] newRow = {theName, defaultValue, userValue};
                dgVariable.Rows.Add(newRow);
            }
        }

        private string GetValueFromObject(object theValue)
        {
            string result = null;

            if (theValue is NdConvertibleValue)
            {
                var cv = (NdConvertibleValue) theValue;
                result = CombineArrayToString(cv.BaseValue);
            }
            else
            {
                if (result != null)
                    result = theValue.ToString();
                else
                    result = "EMPTY";
            }

            return result;
        }

        private string CombineArrayToString(double[] sourceArray)
        {
            string result = null;

            if (sourceArray.Length == 0)
            {
                result = "empty";
            }
            else if (sourceArray.Length == 1)
            {
                result = Parsing.DoubleToString(sourceArray[0]);
            }
            else
            {
                foreach (var thisValue in sourceArray)
                    if (result == null)
                        result = "{" + Parsing.DoubleToString(thisValue);
                    else
                        result += "," + Parsing.DoubleToString(thisValue);

                result += "}";
            }

            return result;
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
            InitScreenPlugin();
        }

        private void InitScreenPlugin()
        {
            var defaults = QraStateContainer.Instance.Defaults;
            var userSessionValues = QraStateContainer.Instance.Parameters;

            dgVariable.Columns[0].Width = 200;
            dgVariable.Columns[1].Width = 280;
            dgVariable.Columns[2].Width = 280;

            FillCombinedDataGrid(dgVariable, defaults, userSessionValues);
        }
    }
}