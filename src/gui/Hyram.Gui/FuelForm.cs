/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{

    public partial class FuelForm : Form
    {
        private readonly StateContainer _state = State.Data;
        private SortedDictionary<int, string> _allocationOptions;
        private bool _ignoreStateChange = false;

        BindingSource bs = new BindingSource();

        /// <summary>
        ///
        /// Note on handling fuel change and fuelchange event, which triggers updates in other forms:
        /// The fuelchange event is fired when fuel is changed via simple fuel dropdowns.
        /// It is also fired when this fuelform is closed.
        /// Also note that user changes in this form are stored immediately, not at form close.
        /// </summary>
        public FuelForm()
        {
            InitializeComponent();
        }


        private void FuelForm_Load(object sender, EventArgs e)
        {
            // Use species enum for preset blends.
            var selectorFuels = new Dictionary<string, SpeciesOptions>
            {
                {_state.FuelH2.ToString(), SpeciesOptions.EFuelH2},
                {_state.FuelMethane.ToString(), SpeciesOptions.EFuelMethane},
                {_state.FuelPropane.ToString(), SpeciesOptions.EFuelPropane},
                {"NIST1 Blend", SpeciesOptions.EFuelBlendNist1},
                {"NIST2 Blend", SpeciesOptions.EFuelBlendNist2},
                {"RG2 Blend", SpeciesOptions.EFuelBlendRg2},
                {"GU1 Blend", SpeciesOptions.EFuelBlendGu1},
                {"GU2 Blend", SpeciesOptions.EFuelBlendGu2},
                {"Blend (manual)", SpeciesOptions.EFuelBlendEmpty},
            };
            FuelSelector.DataSource = new BindingSource(selectorFuels, null);
            FuelSelector.DisplayMember = "Key";
            FuelSelector.ValueMember = "Value";
            RefreshFuelSelectorSelection();

            // use int instead of fuel to allow for customized options
            _allocationOptions = new SortedDictionary<int, string>
            {
                {_state.FuelBlend.Id, "Normalize" },
                {_state.FuelH2.Id, _state.FuelH2.ToString() },
                {_state.FuelMethane.Id, _state.FuelMethane.ToString() },
                {_state.FuelPropane.Id, _state.FuelPropane.ToString() },

                {_state.FuelN2.Id, _state.FuelN2.ToString() },
                {_state.FuelCo2.Id, _state.FuelCo2.ToString() },
                {_state.FuelEthane.Id, _state.FuelEthane.ToString() },
                {_state.FuelNButane.Id, _state.FuelNButane.ToString() },
                {_state.FuelIsoButane.Id, _state.FuelIsoButane.ToString() },
                {_state.FuelNPentane.Id, _state.FuelNPentane.ToString() },
                {_state.FuelIsoPentane.Id, _state.FuelIsoPentane.ToString() },
                {_state.FuelNHexane.Id, _state.FuelNHexane.ToString() },
            };
            AllocationSelector.DataSource = new BindingSource(_allocationOptions, null);
            AllocationSelector.DisplayMember = "Value";
            AllocationSelector.ValueMember = "Key";
            AllocationSelector.SelectedIndex = 1;

            var gridFuels = new BindingList<FuelType>
            {
                _state.FuelH2, _state.FuelMethane, _state.FuelPropane,
                _state.FuelN2, _state.FuelCo2, _state.FuelEthane, _state.FuelNButane, _state.FuelIsoButane,
                _state.FuelNPentane, _state.FuelIsoPentane, _state.FuelNHexane,
            };
            bs.DataSource = gridFuels;
            FuelDataGrid.AutoGenerateColumns = false;
            // NOTE: to make this sortable, either convert to datatable or extend with BindingList and IBindingListView
            FuelDataGrid.DataSource = bs;
            FuelDataGrid.Columns[0].DataPropertyName = "Active";
            FuelDataGrid.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;

            FuelDataGrid.Columns[1].DataPropertyName = "Name";
            FuelDataGrid.Columns[2].DataPropertyName = "Formula";
            FuelDataGrid.Columns[3].DataPropertyName = "Amount";
            FuelDataGrid.Columns[3].DefaultCellStyle.Format = "P3";
            FuelDataGrid.Columns[3].ValueType = typeof(Double);

            FuelDataGrid.Columns[0].Width = 55;
            FuelDataGrid.Columns[1].Width = 110;
            FuelDataGrid.Columns[2].Width = 100;
            FuelDataGrid.Columns[3].Width = 120;

            _state.FuelTypeChangedEvent += (o, args) => RefreshState();

            UpdateTotal();
        }

        private void UpdateTotal()
        {
            double val = _state.SumFuelAmounts();
            totalAmountLabel.Text = val.ToString("P3");

            if (Math.Abs(val - 1.0f) > _state.FuelPrecision)
            {
                alertLabel.Show();
                closeBtn.Enabled = false;
            }
            else
            {
                alertLabel.Hide();
                closeBtn.Enabled = true;
            }
        }

        private void RefreshState()
        {
            if (!_ignoreStateChange)
            {
                RefreshFuelSelectorSelection();
                FuelDataGrid.Update();
                FuelDataGrid.Refresh();
            }
            UpdateTotal();
        }

        private void RefreshFuelSelectorSelection()
        {
            if (FuelSelector.Items.Count == 0)
            {
                // form still initializing
                return;
            }

            var option = _state.SpeciesOption;
            if (option == SpeciesOptions.EFuelH2) FuelSelector.SelectedIndex = 0;
            else if (option == SpeciesOptions.EFuelMethane) FuelSelector.SelectedIndex = 1;
            else if (option == SpeciesOptions.EFuelPropane) FuelSelector.SelectedIndex = 2;
            else if (option == SpeciesOptions.EFuelBlendNist1) FuelSelector.SelectedIndex = 3;
            else if (option == SpeciesOptions.EFuelBlendNist2) FuelSelector.SelectedIndex = 4;
            else if (option == SpeciesOptions.EFuelBlendRg2) FuelSelector.SelectedIndex = 5;
            else if (option == SpeciesOptions.EFuelBlendGu1) FuelSelector.SelectedIndex = 6;
            else if (option == SpeciesOptions.EFuelBlendGu2) FuelSelector.SelectedIndex = 7;
            else if (option == SpeciesOptions.EFuelBlendEmpty) FuelSelector.SelectedIndex = 8;
        }


        private void CloseBtn_Click(object sender, System.EventArgs e)
        {
            if (Math.Abs(_state.SumFuelAmounts() - 1.0f) > _state.FuelPrecision)
            {
                UpdateTotal();
            }
            else
            {
                Close();
            }
        }


        private void FuelForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _state.NotifyFuelChange();
        }

        private void AllocateBtn_Click(object sender, System.EventArgs e)
        {
            int fuelId = ((KeyValuePair<int, string>)AllocationSelector.SelectedItem).Key;
            FuelType selectedFuel = _state.GetFuel(fuelId);
            _state.AllocateFuelRemainder(selectedFuel);
            RefreshState();
        }

        private void FuelSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _ignoreStateChange = true;

            SpeciesOptions species = ((dynamic)FuelSelector.SelectedItem).Value;
            _state.SetFuel(species);
            FuelDataGrid.Update();
            FuelDataGrid.Refresh();
            UpdateTotal();

            _ignoreStateChange = false;
        }

        private void FuelDataGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RefreshState();
        }

        private void FuelDataGrid_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                if (e != null)
                {
                    if (e.Value != null)
                    {
                        try
                        {
                            // Handle '%' sign, if any
                            string input = e.Value.ToString().TrimEnd(new char[] {'%', ' '});
                            if (double.TryParse(input, out double val))
                            {
                                val /= 100D;
                                e.Value = val;
                                e.ParsingApplied = true;
                            }
                            else
                            {
                                e.ParsingApplied = false;
                            }
                        }
                        catch (FormatException)
                        {
                            e.ParsingApplied = false;
                        }
                    }
                }
            }
        }

        private void FuelDataGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Input format invalid");
        }
    }
}