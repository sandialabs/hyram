/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
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