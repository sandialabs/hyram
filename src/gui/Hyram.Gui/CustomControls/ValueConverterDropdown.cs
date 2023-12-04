/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
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
            var cv = new Parameter(_mConverter, oldUnit, value);
            return cv.GetValue(newUnit);
        }
    }
}