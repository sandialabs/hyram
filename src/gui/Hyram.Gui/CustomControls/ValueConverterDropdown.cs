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
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class ValueConverterDropdown : UserControl
    {
        private UnitOfMeasurementConverters _mConverter;

        public ValueConverterDropdown()
        {
            InitializeComponent();
        }

        public double[] StoredValue { get; set; } = new double[0];

        public object SelectedItem
        {
            get => cbMain.SelectedItem;
            set
            {
                if (value != null)
                {
                    var selItem = (Enum) value;
                    var sSelItem = selItem.ToString();
                    for (var index = 0; index < cbMain.Items.Count; index++)
                        if ((string) cbMain.Items[index] == sSelItem)
                        {
                            cbMain.SelectedIndex = index;
                            break;
                        }
                }
            }
        }

        public UnitOfMeasurementConverters Converter
        {
            get => _mConverter;
            set
            {
                _mConverter = value;
                if (_mConverter != null) Fill();
            }
        }

        private void Fill()
        {
            cbMain.Items.Clear();
            foreach (var thisKey in _mConverter.Keys) cbMain.Items.Add(thisKey);
        }

        public event EventHandler OnSelectedIndexChanged;


        private void cbMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            OnSelectedIndexChanged?.Invoke(sender, e);
        }

        public double ConvertValue(Enum oldUnit, Enum newUnit, double value)
        {
            var valArray = new double[1] {value};

            var cv = new ConvertibleValue(_mConverter, oldUnit, valArray);
            return cv.GetValue(newUnit)[0];
        }
    }
}