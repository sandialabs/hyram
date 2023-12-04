/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class SharedStateForm : UserControl
    {
        private readonly StateContainer _state = State.Data;

        private SortedDictionary<int, string> _allocationOptions;
//        private bool _ignoreStateChange = false;

        BindingSource bs = new BindingSource();


        public SharedStateForm()
        {
            InitializeComponent();
        }

        private void FuelSpecForm_Load(object sender, EventArgs e)
        {
            StateAlert.Hide();
            // TODO: hoist warnings to mainform if user leaves

            // Initialize fuel blend specification table
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

            // use id instead of fuel to allow for customized options
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
            FuelGrid.AutoGenerateColumns = false;

            // to make this sortable, either convert to datatable or extend with BindingList and IBindingListView
            FuelGrid.DataSource = bs;
            FuelGrid.Columns[0].DataPropertyName = "Active";
            FuelGrid.Columns[0].SortMode = DataGridViewColumnSortMode.Automatic;

            FuelGrid.Columns[1].DataPropertyName = "Name";
            FuelGrid.Columns[2].DataPropertyName = "Formula";
            FuelGrid.Columns[3].DataPropertyName = "Amount";
            FuelGrid.Columns[3].DefaultCellStyle.Format = "P3";
            FuelGrid.Columns[3].ValueType = typeof(Double);

            FuelGrid.Columns[0].Width = 55;
            FuelGrid.Columns[1].Width = 110;
            FuelGrid.Columns[2].Width = 100;
            FuelGrid.Columns[3].Width = 120;

            PhaseSelector.DataSource = _state.Phases;
            PhaseSelector.SelectedItem = _state.Phase;

            NozzleSelector.DataSource = _state.NozzleModels;
            NozzleSelector.SelectedItem = _state.Nozzle;

            RefreshFuelSelectorSelection();

            RefreshFuelGrid();
            RefreshParameterGrid();
        }

        private void RefreshParameterGrid()
        {
            ParameterGrid.Rows.Clear();

            List<ParameterInput> formParams;
            formParams = ParameterInput.GetParameterInputList(new[] {
                _state.FluidPressure,
                _state.AmbientTemperature,
                _state.AmbientPressure,
                _state.OrificeDischargeCoefficient,
            });

            if (_state.DisplayTemperature())
            {
                formParams.Insert(0, new ParameterInput(_state.FluidTemperature));
            }

            pressureAbsoluteToggle.Checked = _state.PressureIsAbsolute;
            GridHelpers.InitParameterGrid(ParameterGrid, formParams, false);
            ParameterGrid.ColumnHeadersDefaultCellStyle.Font = new Font("Sans Serif", 9.0F, FontStyle.Bold);
            ParameterGrid.Columns[0].Width = 180;

            var pressureDescripCell = (DataGridViewTextBoxCell)ParameterGrid.Rows[1].Cells[0];
            pressureDescripCell.Value = _state.PressureIsAbsolute ? "Tank fluid pressure (absolute)" : "Tank fluid pressure (gauge)";


            CheckFormValid();

        }


        private void CheckFormValid()
        {
            bool showAlert = false;
            string alertText = "";

            if (showAlert)
            {
                StateAlert.Text = alertText;
                StateAlert.Show();
            }
            else
            {
                StateAlert.Hide();
            }
        }


        private void RefreshFuelGrid()
        {
            FuelGrid.Update();
            FuelGrid.Refresh();

            // update displayed total
            double val = _state.SumFuelAmounts();
            totalAmountLabel.Text = val.ToString("P3");

            if (Math.Abs(val - 1.0f) > _state.FuelPrecision)
            {
                FuelCompAlert.Show();
                AllocateBtn.Enabled = true;
            }
            else
            {
                FuelCompAlert.Hide();
                AllocateBtn.Enabled = false;
            }

            RefreshFuelSelectorSelection();
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


        private void AllocateBtn_Click(object sender, EventArgs e)
        {
            int fuelId = ((KeyValuePair<int, string>)AllocationSelector.SelectedItem).Key;
            FuelType selectedFuel = _state.GetFuel(fuelId);
            _state.AllocateFuelRemainder(selectedFuel);
            RefreshFuelGrid();
        }

        private void FuelSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SpeciesOptions species = ((dynamic)FuelSelector.SelectedItem).Value;
            _state.SetFuel(species);
            RefreshFuelGrid();
        }

        private void FuelGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RefreshFuelGrid();
        }

        private void FuelGrid_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
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

        private void FuelGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Fuel concentration format invalid");
        }

        private void ParameterGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            GridHelpers.ChangeParameterValue((DataGridView) sender, e);
            CheckFormValid();

        }

        private void ParameterGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Input format invalid");
        }

        private void PhaseSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Phase = (ModelPair)PhaseSelector.SelectedItem;
            RefreshParameterGrid();
        }

        private void NozzleSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _state.Nozzle = (ModelPair)NozzleSelector.SelectedItem;
        }

        private void ParameterGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            GridHelpers.CellValidating_CheckDoubleOrNullable(ParameterGrid, sender, e);
        }

        private void pressureAbsoluteToggle_CheckedChanged(object sender, EventArgs e)
        {
            _state.PressureIsAbsolute = pressureAbsoluteToggle.Checked;
            RefreshParameterGrid();
        }
    }
}