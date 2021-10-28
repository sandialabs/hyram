/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class UiStateRoutines
    {
        private const string LongSinglePointRsm = "Single point radiation source";
        private const string LongMultipleSourceRsm = "Multiple radiation sources, integrated";

        public static Dictionary<string, Enum> GetRadiativeSourceModelDict()
        {
            var result = new Dictionary<string, Enum>
            {
                {LongSinglePointRsm, RadiativeSourceModels.Single},
                {LongMultipleSourceRsm, RadiativeSourceModels.Multi}
            };
            return result;
        }

        public static void FillUserParamsFromString(string theString, string key, UnitOfMeasurementConverters converter,
            Enum theUnit)
        {
            var tbValue = double.NaN;
            var ucKey = key.ToUpper();
            if (ParseUtility.TryParseDouble(theString, out tbValue))
            {
                double fieldMinValue = double.NegativeInfinity, fieldMaxValue = double.PositiveInfinity;
                if (StateContainer.Instance.IsItemInDatabase(ucKey))
                {
                    fieldMinValue = StateContainer.Instance.GetStateDefinedValueObject(ucKey).MinValue;
                    fieldMaxValue = StateContainer.Instance.GetStateDefinedValueObject(ucKey).MaxValue;
                }

                StateContainer.Instance.Parameters[ucKey] = new ConvertibleValue(converter, theUnit,
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