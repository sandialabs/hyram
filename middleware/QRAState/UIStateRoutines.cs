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
using System.Drawing;
using System.Windows.Forms;
using DefaultParsing;
using JrConversions;

namespace QRAState
{
    public class UiStateRoutines
    {
        private const string LongSinglePointRsm = "Single point radiation source";
        private const string LongMultipleSourceRsm = "Multiple radiation sources, integrated";

        public static Dictionary<string, Enum> GetRadiativeSourceModelDict()
        {
            var result = new Dictionary<string, Enum>();
            result.Add(LongSinglePointRsm, RadiativeSourceModels.Single);
            result.Add(LongMultipleSourceRsm, RadiativeSourceModels.Multi);
            return result;
        }

        public static void FillUserParamsFromString(string theString, string key, UnitOfMeasurementConverters converter,
            Enum theUnit)
        {
            var tbValue = double.NaN;
            var ucKey = key.ToUpper();
            if (Parsing.TryParseDouble(theString, out tbValue))
            {
                double fieldMinValue = double.NegativeInfinity, fieldMaxValue = double.PositiveInfinity;
                if (QraStateContainer.Instance.IsItemInDatabase(ucKey))
                {
                    fieldMinValue = QraStateContainer.Instance.GetStateDefinedValueObject(ucKey).MinValue;
                    fieldMaxValue = QraStateContainer.Instance.GetStateDefinedValueObject(ucKey).MaxValue;
                }

                QraStateContainer.Instance.Parameters[ucKey] = new NdConvertibleValue(converter, theUnit,
                    new double[1] {tbValue}, fieldMinValue, fieldMaxValue);
            }
        }

        public static void UnselectButtons(Control parentControl)
        {
            foreach (Control thisControl in parentControl.Controls)
            {
                if (thisControl.HasChildren) UnselectButtons(thisControl);

                if (thisControl is Button)
                {
                    if (thisControl.Text == "Harm Models") thisControl.Text = thisControl.Text;

                    var thisButton = (Button) thisControl;
                    if (thisButton.ForeColor != Color.Black) thisButton.ForeColor = Color.Black;
                }
            }
        }

        public static void SetSelectedDropdownValue(ComboBox cbToSet, string name)
        {
            var ucName = name.ToUpper().Trim();

            for (var index = 0; index < cbToSet.Items.Count; index++)
            {
                var ucItemName = cbToSet.Items[index].ToString().ToUpper().Trim();
                if (ucItemName == ucName)
                {
                    cbToSet.SelectedIndex = index;
                    break;
                }
            }
        }
    }
}