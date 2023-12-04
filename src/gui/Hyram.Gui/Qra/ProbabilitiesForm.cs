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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class ProbabilitiesForm : UserControl
    {
        private StateContainer _state = State.Data;
        private const int _immedIgnitionCol = 1;
        private const int _delayIgnitionCol = 2;
        private readonly MainForm _mainForm;
        private bool _ignoreChangeEvents;

        public string AlertMessage { get; set; } = "";
        public AlertLevel Alert { get; set; } = AlertLevel.AlertNull;

        public ProbabilitiesForm(MainForm mainForm)
        {
            _ignoreChangeEvents = true;

            _mainForm = mainForm;
            InitializeComponent();
            RefreshForm();

            Load += delegate { RefreshForm(); };

            clmCFComponent.DefaultCellStyle.BackColor = Color.LightGray;
            clmCFFailureMode.DefaultCellStyle.BackColor = Color.LightGray;

            clmAPCompName.DefaultCellStyle.BackColor = Color.LightGray;
            clmAPCompName.ReadOnly = true;
            clmAPFailMode.DefaultCellStyle.BackColor = Color.LightGray;

            // Toggle ParamB input based on distribution selection
            failuresGrid.CellFormatting += ComponentFailures_CellFormatting;
            accidentProbabilitiesGrid.CellFormatting += AccidentFailures_CellFormatting;

            // Format cells
            DataGridView[] componentProbTabs =
            {
                compressorGrid, vesselGrid, filterGrid, flangeGrid, hoseGrid, jointGrid, pipeGrid,
                valveGrid, instrumentGrid,
                exchangerGrid, vaporizerGrid, armGrid,
                extraComponent1Grid, extraComponent2Grid
            };

            foreach (var probTab in componentProbTabs)
            {
                probTab.Columns[(int) Column.LeakSize].ReadOnly = true;
                probTab.Columns[(int) Column.LeakSize].DefaultCellStyle.BackColor = Color.LightGray;

                probTab.Columns[(int) Column.Mu].DefaultCellStyle.Format = "N4";
                probTab.Columns[(int) Column.Mu].DefaultCellStyle.NullValue = "N/A";

                probTab.Columns[(int) Column.Sigma].DefaultCellStyle.Format = "N4";
                probTab.Columns[(int) Column.Sigma].DefaultCellStyle.NullValue = "N/A";

                probTab.Columns[(int) Column.Mean].DefaultCellStyle.Format = "E1";
                probTab.Columns[(int) Column.Mean].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.Mean].DefaultCellStyle.BackColor = Color.LightGray;
                probTab.Columns[(int) Column.Mean].ReadOnly = true;

                probTab.Columns[(int) Column.Fifth].DefaultCellStyle.Format = "E1";
                probTab.Columns[(int) Column.Fifth].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.Fifth].DefaultCellStyle.BackColor = Color.LightGray;
                probTab.Columns[(int)Column.Fifth].HeaderText = "5th";
                probTab.Columns[(int) Column.Fifth].ReadOnly = true;

                probTab.Columns[(int) Column.Median].DefaultCellStyle.Format = "E1";
                probTab.Columns[(int) Column.Median].DefaultCellStyle.NullValue = "N/A";

                probTab.Columns[(int) Column.NinetyFifth].DefaultCellStyle.Format = "E1";
                probTab.Columns[(int) Column.NinetyFifth].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.NinetyFifth].DefaultCellStyle.BackColor = Color.LightGray;
                probTab.Columns[(int)Column.NinetyFifth].HeaderText = "95th";
                probTab.Columns[(int) Column.NinetyFifth].ReadOnly = true;
            }

            ignitionProbabilitiesGrid.Columns[_immedIgnitionCol].DefaultCellStyle.Format = "N4";
            ignitionProbabilitiesGrid.Columns[_immedIgnitionCol].DefaultCellStyle.NullValue = 0;

            ignitionProbabilitiesGrid.Columns[_delayIgnitionCol].DefaultCellStyle.NullValue = 0;

            _ignoreChangeEvents = false;
        }


        public void RefreshForm()
        {
            var ignoreEvents = _ignoreChangeEvents;
            _ignoreChangeEvents = true;

            _state = State.Data;
            if (!_state.AllowUncertainty)
            {
                uncertaintyTab.Visible = false;
                uncertaintyTab.Hide();
                dataProbabilitiesTabControl.TabPages.Remove(uncertaintyTab);
            }

            SampleOccupantsCheck.Checked = _state.SampleOccupants;
            SampleLeaksCheck.Checked = _state.SampleLeaks;
            SampleFailureCheck.Checked = _state.SampleFailures;

            clmCFDistributionType.DataSource = Enum.GetValues(typeof(DistributionChoice));
            clmAPDistributionType.DataSource = Enum.GetValues(typeof(DistributionChoice));

            compressorGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbCompressor), null);
            vesselGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbVessel), null);
            filterGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbFilter), null);
            flangeGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbFlange), null);
            hoseGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbHose), null);
            jointGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbJoint), null);
            pipeGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbPipe), null);
            valveGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbValve), null);
            instrumentGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbInstrument), null);
            exchangerGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbExchanger), null);
            vaporizerGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbVaporizer), null);
            armGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbArm), null);
            extraComponent1Grid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbExtra1), null);
            extraComponent2Grid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_state.ProbExtra2), null);

            compressorGrid.Tag = _state.ProbCompressor;
            vesselGrid.Tag = _state.ProbVessel;
            filterGrid.Tag = _state.ProbFilter;
            flangeGrid.Tag = _state.ProbFlange;
            hoseGrid.Tag = _state.ProbHose;
            jointGrid.Tag = _state.ProbJoint;
            pipeGrid.Tag = _state.ProbPipe;
            valveGrid.Tag = _state.ProbValve;
            instrumentGrid.Tag = _state.ProbInstrument;
            exchangerGrid.Tag = _state.ProbExchanger;
            vaporizerGrid.Tag = _state.ProbVaporizer;
            armGrid.Tag = _state.ProbArm;
            extraComponent1Grid.Tag = _state.ProbExtra1;
            extraComponent2Grid.Tag = _state.ProbExtra2;

            var compFailures = new List<FailureMode>
            {
                _state.FailureNozPo,
                _state.FailureNozFtc,
                _state.FailureMValveFtc,
                _state.FailureSValveFtc,
                _state.FailureSValveCcf,
                _state.FailurePValveFto,
                _state.FailureCouplingFtc,
            };
            var accidentFailures = new List<FailureMode>
            {
                _state.FailureOverp,
                _state.FailureDriveoff
            };

            failuresGrid.DataSource = new BindingSource(new BindingList<FailureMode>(compFailures), null);
            accidentProbabilitiesGrid.DataSource = new BindingSource(new BindingList<FailureMode>(accidentFailures), null);

            SeedInput.Text = (Math.Truncate(_state.RandomSeed.GetValue())).ToString();
            NumSamplesInput.Text = (Math.Truncate(_state.NumSamples.GetValue())).ToString();

            PopulateFormIgnitionData();

            CheckFormValid();
            _ignoreChangeEvents = ignoreEvents;
        }

        public void CheckFormValid()
        {
            Alert = AlertLevel.AlertNull;
            AlertMessage = "";

            FuelType fuel = _state.GetActiveFuel();

            if (fuel != _state.FuelH2)
            {
                Alert = AlertLevel.AlertWarning;
                AlertMessage = Notifications.DispenserNonDefaultFluidWarning;
            }

            formWarning.Visible = Alert != AlertLevel.AlertNull;
            formWarning.Text = AlertMessage;
            formWarning.BackColor = _state.AlertBackColors[(int)Alert];
            formWarning.ForeColor = _state.AlertTextColors[(int)Alert];

            _mainForm.NotifyOfChildPublicStateChange();
        }

        /// <summary>
        ///     Update component probability for given component.
        ///     Re-computes mean and variance if mu/sigma changed. Clears mu/sigma if mean or variance changed.
        /// </summary>
        private void UpdateLeakProbability(int row, Column col, ComponentProbability leakData, DataGridView grid)
        {
            if (col == Column.Mu) leakData.Mu = (double)grid.Rows[row].Cells[(int) Column.Mu].Value;
            if (col == Column.Sigma) leakData.Sigma = (double)grid.Rows[row].Cells[(int) Column.Sigma].Value;
            // if user changes median, re-calculate mu and then other parameters from it
            if (col == Column.Median) leakData.Median = (double)grid.Rows[row].Cells[(int) Column.Median].Value;
        }

        private void LeakDataGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var grid = (DataGridView) sender;
            List<ComponentProbability> probabilitiesList = (List<ComponentProbability>)grid.Tag;

            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, probabilitiesList[e.RowIndex], grid);
        }


        private void LeakFrequency_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Cell value must be numeric");
            }
        }

        // Disables the Parameter B input if failure mode uses Expected Value distribution, 
        private void ComponentFailures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == (int)FailuresColumn.ParamB)
            {
                var row = failuresGrid.Rows[e.RowIndex];
                var paramBCell = row.Cells[e.ColumnIndex];
                var dist = (DistributionChoice) row.Cells[(int)FailuresColumn.Distribution].Value;

                if (dist == DistributionChoice.ExpectedValue)
                {
                    paramBCell.ReadOnly = true;
                    e.CellStyle.BackColor = Color.LightGray;
                    e.Value = "";
                }
                else
                {
                    paramBCell.ReadOnly = false;
                    e.CellStyle.BackColor = Color.White;
                }
            }
        }

        // Disables the Parameter B input if failure mode uses Expected Value distribution, 
        private void AccidentFailures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == (int)FailuresColumn.ParamB)
            {
                var row = accidentProbabilitiesGrid.Rows[e.RowIndex];
                var paramBCell = row.Cells[e.ColumnIndex];
                var dist = (DistributionChoice) row.Cells[(int)FailuresColumn.Distribution].Value;

                if (dist == DistributionChoice.ExpectedValue)
                {
                    paramBCell.ReadOnly = true;
                    e.CellStyle.BackColor = Color.LightGray;
                    e.Value = "";
                }
                else
                {
                    paramBCell.ReadOnly = false;
                    e.CellStyle.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        ///     Silently fills ignition form, including rate thresholds and probabilities, with state data.
        /// </summary>
        private void PopulateFormIgnitionData()
        {
            _ignoreChangeEvents = true;

            ignitionRatesListBox.Items.Clear();
            var ignitionThresholds = _state.IgnitionThresholds;
            for (var i = 0; i < ignitionThresholds.Length; ++i)
            {
                ignitionRatesListBox.Items.Add(ignitionThresholds[i]);
            }

            ignitionProbabilitiesGrid.Rows.Clear();
            if (ignitionThresholds.Length > 0)
            {
                ignitionProbabilitiesGrid.Rows.Add("<" + ignitionThresholds[0], "", "");
                for (var i = 1; i < ignitionThresholds.Length; ++i)
                {
                    ignitionProbabilitiesGrid.Rows.Add(ignitionThresholds[i - 1] + "-" + ignitionThresholds[i], "", "");
                }

                ignitionProbabilitiesGrid.Rows.Add("\u2265" + ignitionThresholds[ignitionThresholds.Length - 1], "", "");

                PopulateIgnitionProbabilities();
            }
            ignitionRatesListBox.Update();
            ignitionProbabilitiesGrid.Update();
            _ignoreChangeEvents = false;
        }

         ///
         /// <summary>
         ///    Fills data columns for immediate and delayed ignition probabilities from state.
         /// </summary>
        private void PopulateIgnitionProbabilities()
        {
            var immedIgnitionProbs = _state.ImmediateIgnitionProbs;
            var delayIgnitionProbs = _state.DelayedIgnitionProbs;

            // fill by column
            for (var rowIndex = 0; rowIndex < immedIgnitionProbs.Length; rowIndex++)
            {
                var valueIndex = rowIndex;
                var thisRow = ignitionProbabilitiesGrid.Rows[rowIndex];
                thisRow.Cells[_immedIgnitionCol].Value = immedIgnitionProbs[valueIndex];
            }
            for (var rowIndex = 0; rowIndex < delayIgnitionProbs.Length; rowIndex++)
            {
                var valueIndex = rowIndex;
                var thisRow = ignitionProbabilitiesGrid.Rows[rowIndex];
                thisRow.Cells[_delayIgnitionCol].Value = delayIgnitionProbs[valueIndex].ToString("F4");
            }
        }

        private void StoreIgnitionProbabilities()
        {
            if (!_ignoreChangeEvents)
            {
                var immedIgnitionProbs = GetColumnData(ignitionProbabilitiesGrid, _immedIgnitionCol);
                var delayIgnitionProbs = GetColumnData(ignitionProbabilitiesGrid, _delayIgnitionCol);
                var ignitionThresholds = new double[ignitionRatesListBox.Items.Count];
                var lbItems = new object[ignitionRatesListBox.Items.Count];
                ignitionRatesListBox.Items.CopyTo(lbItems, 0);

                for (var i = 0; i < ignitionThresholds.Length; ++i)
                {
                    ignitionThresholds[i] = (double) lbItems[i];
                }

                for (var i = 0; i < delayIgnitionProbs.Length; ++i)
                {
                    if (delayIgnitionProbs[i] < 0) delayIgnitionProbs[i] = 0;

                    if (delayIgnitionProbs[i] > 1) delayIgnitionProbs[i] = 1;
                }

                for (var i = 0; i < immedIgnitionProbs.Length; ++i)
                {
                    if (immedIgnitionProbs[i] < 0) immedIgnitionProbs[i] = 0;

                    if (immedIgnitionProbs[i] > 1) immedIgnitionProbs[i] = 1;
                }

                if (delayIgnitionProbs.Length != 0 && immedIgnitionProbs.Length != 0)
                {
                    _state.ImmediateIgnitionProbs = immedIgnitionProbs;
                    _state.DelayedIgnitionProbs = delayIgnitionProbs;
                    _state.IgnitionThresholds = ignitionThresholds;
                }
            }
        }


        private double[] GetColumnData(DataGridView dgvControl, int colIndex)
        {
            var result = new double[dgvControl.Rows.Count];
            for (var rowIndex = 0; rowIndex < result.Length; rowIndex++)
            {
                var cell = dgvControl.Rows[rowIndex].Cells[colIndex];
                string cellValue = (cell.Value == null) ? "0" : cell.Value.ToString();

                if (double.TryParse(cellValue, out var resultCellValue))
                {
                    result[rowIndex] = resultCellValue;
                }
            }

            return result;
        }

        /// <summary>
        ///     Stores new ignition threshold in db and expands ignition probability lists to match.
        /// </summary>
        private void btnIgnitionProbabilitiesAdd_Click(object sender, EventArgs e)
        {
            double newThreshold = -1;
            double.TryParse(tbIgnitionProbabilitiesAdd.Text, out newThreshold);
            if (newThreshold > 0.0)
            {
                var immedIgnitionProbs = _state.ImmediateIgnitionProbs;
                var delayIgnitionProbs = _state.DelayedIgnitionProbs;
                var ignitionThresholds = _state.IgnitionThresholds;

                if (!ignitionThresholds.Contains(newThreshold))
                {
                    var newIgnitionThresholds = new double[ignitionThresholds.Length + 1];
                    var newImmediate = new double[immedIgnitionProbs.Length + 1];
                    var newDelayed = new double[delayIgnitionProbs.Length + 1];
                    int offset = 0, index = 0;

                    //Insert new ignition threshold in order
                    for (var i = 0; i < newIgnitionThresholds.Length; ++i)
                    {
                        if (offset == 0 &&
                            (i == newIgnitionThresholds.Length - 1 || ignitionThresholds[i] > newThreshold))
                        {
                            newIgnitionThresholds[i] = newThreshold;
                            index = i;
                            ++offset;
                        }
                        else
                        {
                            newIgnitionThresholds[i] = ignitionThresholds[i - offset];
                        }
                    }

                    offset = 0;
                    for (var i = 0; i < newImmediate.Length; ++i)
                    {
                        if (i == index)
                        {
                            newImmediate[i] = 0.0;
                            newDelayed[i] = 0.0;
                            ++offset;
                        }
                        else
                        {
                            newDelayed[i] = delayIgnitionProbs[i - offset];
                            newImmediate[i] = immedIgnitionProbs[i - offset];
                        }
                    }
                    _state.ImmediateIgnitionProbs = newImmediate;
                    _state.DelayedIgnitionProbs = newDelayed;
                    _state.IgnitionThresholds = newIgnitionThresholds;

                    PopulateFormIgnitionData();
                }
                else
                {
                    MessageBox.Show(newThreshold + " is already in the list of ignition thresholds.");
                }
            }
            else
            {
                MessageBox.Show("Error: ignition threshold must be greater than zero");
            }
        }

        private void btnIgnitionProbabilitiesDelete_Click(object sender, EventArgs e)
        {
            var thresholdToRemove = Convert.ToDouble(ignitionRatesListBox.SelectedItem);
            if (ignitionRatesListBox.Items.Count > 1)
            {
                var immedIgnitionProbs = _state.ImmediateIgnitionProbs;
                var delayIgnitionProbs = _state.DelayedIgnitionProbs;
                var ignitionThresholds = _state.IgnitionThresholds;

                var newIgnitionThresholds = new double[ignitionThresholds.Length - 1];
                var newImmediate = new double[immedIgnitionProbs.Length - 1];
                var newDelayed = new double[delayIgnitionProbs.Length - 1];
                int offset = 0, index = 0;
                for (var i = 0; i < ignitionThresholds.Length; ++i) //Insert new ignition threshold in order
                    if (ignitionThresholds[i] == thresholdToRemove)
                    {
                        index = i;
                        ++offset;
                    }
                    else
                    {
                        newIgnitionThresholds[i - offset] = ignitionThresholds[i];
                    }

                offset = 0;
                for (var i = 0; i < immedIgnitionProbs.Length; ++i)
                    if (i == index)
                    {
                        ++offset;
                    }
                    else
                    {
                        newDelayed[i - offset] = delayIgnitionProbs[i];
                        newImmediate[i - offset] = immedIgnitionProbs[i];
                    }

                _state.ImmediateIgnitionProbs = newImmediate;
                _state.DelayedIgnitionProbs = newDelayed;
                _state.IgnitionThresholds = newIgnitionThresholds;
                PopulateFormIgnitionData();
            }
            else
            {
                MessageBox.Show("Error: At least one ignition threshold is required.");
            }
        }

        private enum Column
        {
            LeakSize,
            Mu,
            Sigma,
            Mean,
            Fifth,
            Median,
            NinetyFifth,
        }

        private enum FailuresColumn
        {
//            Index,
            ComponentName,
            Mode,
            Distribution,
            ParamA,
            ParamB
        }

        private void ignitionProbabilitiesGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == _delayIgnitionCol || e.ColumnIndex == _immedIgnitionCol) &&
                e.Value != null)
            {
                if (double.TryParse(e.Value.ToString(), out double val))
                {
                    e.Value = val.ToString("F4");
                }
            }
        }

        private void ignitionProbabilitiesGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == _delayIgnitionCol || e.ColumnIndex == _immedIgnitionCol)
            {
                if (!double.TryParse(e.FormattedValue.ToString(), out var newVal) || newVal < 0)
                {
                    e.Cancel = true;
                    ignitionProbabilitiesGrid.Rows[e.RowIndex].ErrorText = "Value must be non-negative number";
                }
                else
                {
                    ignitionProbabilitiesGrid.Rows[e.RowIndex].ErrorText = null;
                }
            }
        }

        private void ignitionProbabilitiesGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            StoreIgnitionProbabilities();
        }

        private void SeedInput_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(SeedInput.Text, out int val))
            {
                _state.RandomSeed.SetValue(val);
            }
            else
            {
                SeedInput.Text = _state.RandomSeed.GetValue().ToString();
            }

        }

        private void SampleOccupantsCheck_CheckedChanged(object sender, EventArgs e)
        {
            _state.SampleOccupants = SampleOccupantsCheck.Checked;

        }

        private void SampleLeaksCheck_CheckedChanged(object sender, EventArgs e)
        {
            _state.SampleLeaks = SampleLeaksCheck.Checked;

        }

        private void SampleFailureCheck_CheckedChanged(object sender, EventArgs e)
        {
            _state.SampleFailures = SampleFailureCheck.Checked;
        }

        private void NumSamplesInput_TextChanged(object sender, EventArgs e)
        {
            if (int.TryParse(NumSamplesInput.Text, out int val))
            {
                _state.NumSamples.SetValue(val);
            }
            else
            {
                NumSamplesInput.Text = _state.NumSamples.GetValue().ToString();
            }

        }
    }
}