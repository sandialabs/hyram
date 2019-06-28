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

using System.Drawing;
using System.Windows.Forms;
using DefaultParsing;
using JrConversions;

namespace QRAState
{
    public class StaticUiHelperRoutines
    {
        public static void SetDistanceTextBox(TextBox tb, ref NdConvertibleValue valueObj, string key)
        {
            valueObj = QraStateContainer.Instance.GetStateDefinedValueObject(key) ??
                       new NdConvertibleValue(StockConverters.DistanceConverter, DistanceUnit.Meter, new[] {0D});
            tb.Text = valueObj.GetValue(DistanceUnit.Meter.ToString())[0].ToString("F2");
        }

        public static void SetUnitlessTextBox(TextBox tb, ref NdConvertibleValue valueObj, string key)
        {
            valueObj = QraStateContainer.Instance.GetStateDefinedValueObject(key) ??
                       new NdConvertibleValue(StockConverters.UnitlessConverter, UnitlessUnit.Unitless,
                           new[] {0D});
            tb.Text = valueObj.GetValue(UnitlessUnit.Unitless.ToString())[0].ToString("F2");
        }


        public static void DistTextboxValueChanged(TextBox sender, ref NdConvertibleValue valueObj)
        {
            var testValue = double.NaN;

            if (Parsing.TryParseDouble(sender.Text, out testValue))
            {
                var newValue = new double[1];
                newValue[0] = testValue;
                valueObj.SetValue(DistanceUnit.Meter.ToString(), newValue);
                if (sender.ForeColor != Color.Black) sender.ForeColor = Color.Black;
            }
            else
            {
                if (sender.ForeColor != Color.Red) sender.ForeColor = Color.Red;
            }
        }


        public static void UnitlessTextBoxValueChanged(TextBox sender, ref NdConvertibleValue valueObj)
        {
            var testValue = double.NaN;

            if (Parsing.TryParseDouble(sender.Text, out testValue))
            {
                var newValue = new double[1];
                newValue[0] = testValue;
                valueObj.SetValue(UnitlessUnit.Unitless.ToString(), newValue);
                if (sender.ForeColor != Color.Black) sender.ForeColor = Color.Black;
            }
            else
            {
                if (sender.ForeColor != Color.Red) sender.ForeColor = Color.Red;
            }
        }
    }
}