/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    public partial class ProbabilitiesForm : AnalysisForm
    {
        private const int ImmedIgnitionColumn = 1;
        private const int DelayIgnitionColumn = 2;

        private List<ComponentProbability> _compressorProbs;
        private List<ComponentProbability> _vesselProbs;
        private List<ComponentProbability> _filterProbs;
        private List<ComponentProbability> _flangeProbs;
        private List<ComponentProbability> _hoseProbs;
        private List<ComponentProbability> _instrProbs;
        private List<ComponentProbability> _jointProbs;
        private List<ComponentProbability> _pipeProbs;
        private List<ComponentProbability> _valveProbs;
        private List<ComponentProbability> _exchangerProbs;
        private List<ComponentProbability> _vaporizerProbs;
        private List<ComponentProbability> _armProbs;
        private List<ComponentProbability> _ex1Probs;
        private List<ComponentProbability> _ex2Probs;

        private bool _mIgnoringChangeEvents;

        public ProbabilitiesForm(MainForm mainForm)
        {
            MainForm = mainForm;
            InitializeComponent();
            CheckFormValid();
        }

        /// <summary>
        ///     Set up data-binding between DB and displayed tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpDataProbabilities_Load(object sender, EventArgs e)
        {
            clmCFComponent.DefaultCellStyle.BackColor = Color.LightGray;
            clmCFFailureMode.DefaultCellStyle.BackColor = Color.LightGray;
            clmCFDistributionType.DataSource = Enum.GetValues(typeof(FailureDistributionType));

            clmAPCompName.DefaultCellStyle.BackColor = Color.LightGray;
            clmAPCompName.ReadOnly = true;
            clmAPFailMode.DefaultCellStyle.BackColor = Color.LightGray;
            clmAPDistributionType.DataSource = Enum.GetValues(typeof(FailureDistributionType));

            OnFormDisplay();

            // Toggle ParamB input based on distribution selection
            componentFailuresGrid.CellFormatting += ComponentFailures_CellFormatting;
            accidentProbabilitiesGrid.CellFormatting += AccidentFailures_CellFormatting;

            // Format cells
            DataGridView[] componentProbTabs =
            {
                compressorDistributionsGrid, cylinderDistributionsGrid, filterDistributionsGrid, flangeDistributionsGrid, hoseDistributionsGrid, jointDistributionsGrid, pipeDistributionsGrid,
                valveDistributionsGrid, instrumentDistributionsGrid,
                exchangerGrid, vaporizerGrid, armGrid,
                extraComponent1DistributionsGrid, extraComponent2DistributionsGrid
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

            ignitionProbabilitiesGrid.Columns[ImmedIgnitionColumn].DefaultCellStyle.Format = "N4";
            ignitionProbabilitiesGrid.Columns[DelayIgnitionColumn].DefaultCellStyle.NullValue = 0;

            //StateContainer.Instance.FuelTypeChangedEvent += delegate{OnFormDisplay();};

            StateContainer.SetValue("ResultsAreStale", true);
            _mIgnoringChangeEvents = false;
        }

        public override void OnFormDisplay()
        {
            var ignoreEvents = _mIgnoringChangeEvents;
            _mIgnoringChangeEvents = true;
            _compressorProbs = StateContainer.Instance.GetLeakData("Prob.Compressor");
            _vesselProbs = StateContainer.Instance.GetLeakData("Prob.Vessel");
            _filterProbs = StateContainer.Instance.GetLeakData("Prob.Filter");
            _flangeProbs = StateContainer.Instance.GetLeakData("Prob.Flange");
            _hoseProbs = StateContainer.Instance.GetLeakData("Prob.Hose");
            _instrProbs = StateContainer.Instance.GetLeakData("Prob.Instrument");
            _jointProbs = StateContainer.Instance.GetLeakData("Prob.Joint");
            _pipeProbs = StateContainer.Instance.GetLeakData("Prob.Pipe");
            _valveProbs = StateContainer.Instance.GetLeakData("Prob.Valve");
            _exchangerProbs = StateContainer.Instance.GetLeakData("Prob.Exchanger");
            _vaporizerProbs = StateContainer.Instance.GetLeakData("Prob.Vaporizer");
            _armProbs = StateContainer.Instance.GetLeakData("Prob.Arm");
            _ex1Probs = StateContainer.Instance.GetLeakData("Prob.Extra1");
            _ex2Probs = StateContainer.Instance.GetLeakData("Prob.Extra2");

            compressorDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_compressorProbs), null);
            cylinderDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_vesselProbs), null);
            filterDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_filterProbs), null);
            flangeDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_flangeProbs), null);
            hoseDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_hoseProbs), null);
            jointDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_jointProbs), null);
            pipeDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_pipeProbs), null);
            valveDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_valveProbs), null);
            instrumentDistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_instrProbs), null);
            exchangerGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_exchangerProbs), null);
            vaporizerGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_vaporizerProbs), null);
            armGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_armProbs), null);
            extraComponent1DistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_ex1Probs), null);
            extraComponent2DistributionsGrid.DataSource = new BindingSource(new BindingList<ComponentProbability>(_ex2Probs), null);

            var compFailures = new List<FailureMode>
            {
                StateContainer.GetValue<FailureMode>("Failure.NozPO"),
                StateContainer.GetValue<FailureMode>("Failure.NozFTC"),
                StateContainer.GetValue<FailureMode>("Failure.MValveFTC"),
                StateContainer.GetValue<FailureMode>("Failure.SValveFTC"),
                StateContainer.GetValue<FailureMode>("Failure.SValveCCF"),
                StateContainer.GetValue<FailureMode>("Failure.PValveFTO"),
                StateContainer.GetValue<FailureMode>("Failure.CouplingFTC")
            };
            var accidentFailures = new List<FailureMode>
            {
                StateContainer.GetValue<FailureMode>("Failure.Overp"),
                StateContainer.GetValue<FailureMode>("Failure.Driveoff")
            };

            componentFailuresGrid.DataSource = new BindingSource(new BindingList<FailureMode>(compFailures), null);
            accidentProbabilitiesGrid.DataSource = new BindingSource(new BindingList<FailureMode>(accidentFailures), null);

            PopulateFormIgnitionData();

            _mIgnoringChangeEvents = ignoreEvents;
        }

        public override void CheckFormValid()
        {
            AlertType = 0;
            AlertMessage = "";
            AlertDisplayed = false;

            FuelType fuel = StateContainer.GetValue<FuelType>("FuelType");
            FluidPhase phase = StateContainer.GetValue<FluidPhase>("ReleaseFluidPhase");

            if (fuel != FuelType.Hydrogen)
            {
                AlertDisplayed = true;
                AlertType = 1;
                AlertMessage = "Default data for failures were generated for " +
                               "high pressure gaseous hydrogen systems and may not be appropriate for the selected fuel";
            }

            formWarning.Visible = AlertDisplayed;
            formWarning.Text = AlertMessage;
            formWarning.BackColor = AlertType == 1 ? MainForm.WarningBackColor : MainForm.ErrorBackColor;
            formWarning.ForeColor = AlertType == 1 ? MainForm.WarningForeColor : MainForm.ErrorForeColor;

            MainForm.NotifyOfChildPublicStateChange();
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
            List<ComponentProbability> probabilitiesList = _compressorProbs;
            if (grid == compressorDistributionsGrid)
            {
                probabilitiesList = _compressorProbs;
            }
            else if (grid == cylinderDistributionsGrid)
            {
                probabilitiesList = _vesselProbs;
            }
            else if (grid == filterDistributionsGrid)
            {
                probabilitiesList = _filterProbs;
            }
            else if (grid == flangeDistributionsGrid)
            {
                probabilitiesList = _flangeProbs;
            }
            else if (grid == hoseDistributionsGrid)
            {
                probabilitiesList = _hoseProbs;
            }
            else if (grid == jointDistributionsGrid)
            {
                probabilitiesList = _jointProbs;
            }
            else if (grid == pipeDistributionsGrid)
            {
                probabilitiesList = _pipeProbs;
            }
            else if (grid == valveDistributionsGrid)
            {
                probabilitiesList = _valveProbs;
            }
            else if (grid == instrumentDistributionsGrid)
            {
                probabilitiesList = _instrProbs;
            }
            else if (grid == exchangerGrid)
            {
                probabilitiesList = _exchangerProbs;
            }
            else if (grid == vaporizerGrid)
            {
                probabilitiesList = _vaporizerProbs;
            }
            else if (grid == armGrid)
            {
                probabilitiesList = _armProbs;
            }
            else if (grid == extraComponent1DistributionsGrid)
            {
                probabilitiesList = _ex1Probs;
            }
            else if (grid == extraComponent2DistributionsGrid)
            {
                probabilitiesList = _ex2Probs;
            }

            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, probabilitiesList[e.RowIndex], grid);
        }


        private void LeakFrequency_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Cell value must be numeric");
            }
        }

        /// <summary>
        ///     If failure mode uses Expected Value distribution, disable the Parameter B input
        /// </summary>
        private void ComponentFailures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var col = e.ColumnIndex;
            if (col == 4)
            {
                var row = e.RowIndex;
                var cell = componentFailuresGrid.Rows[row].Cells[col];
                // Get the distribution type of that row
                var dist = (FailureDistributionType) componentFailuresGrid.Rows[row].Cells[2].Value;

                if (dist == FailureDistributionType.ExpectedValue)
                {
                    cell.ReadOnly = true;
                    e.CellStyle.BackColor = Color.LightGray;
                    e.Value = "";
                }
                else
                {
                    cell.ReadOnly = false;
                    e.CellStyle.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        ///     If failure mode uses Expected Value distribution, disable the Parameter B input
        /// </summary>
        private void AccidentFailures_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var col = e.ColumnIndex;
            if (col == 4)
            {
                var row = e.RowIndex;
                var cell = accidentProbabilitiesGrid.Rows[row].Cells[col];
                // Get the distribution type of that row
                var dist =
                    (FailureDistributionType) accidentProbabilitiesGrid.Rows[row].Cells[2].Value;

                if (dist == FailureDistributionType.ExpectedValue)
                {
                    cell.ReadOnly = true;
                    e.CellStyle.BackColor = Color.LightGray;
                    e.Value = "";
                }
                else
                {
                    cell.ReadOnly = false;
                    e.CellStyle.BackColor = Color.White;
                }
            }
        }

        /// <summary>
        ///     Silently fills ignition form, including rate thresholds and probabilities, with state data.
        /// </summary>
        private void PopulateFormIgnitionData()
        {
            _mIgnoringChangeEvents = true;

            ignitionRatesListBox.Items.Clear();
            var ignitionThresholds = StateContainer.GetValue<double[]>("IgnitionThresholds");
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
            _mIgnoringChangeEvents = false;
        }

         ///
         /// <summary>
         ///    Fills data columns for immediate and delayed ignition probabilities from state.
         /// </summary>
        private void PopulateIgnitionProbabilities()
        {
            var immedIgnitionProbs = StateContainer.GetValue<double[]>("ImmedIgnitionProbs");
            var delayIgnitionProbs = StateContainer.GetValue<double[]>("DelayIgnitionProbs");

            // fill by column
            for (var rowIndex = 0; rowIndex < immedIgnitionProbs.Length; rowIndex++)
            {
                var valueIndex = rowIndex;
                var thisRow = ignitionProbabilitiesGrid.Rows[rowIndex];
                //thisRow.Cells[ImmedIgnitionColumn].Value = immedIgnitionProbs[valueIndex].ToString("F4");
                thisRow.Cells[ImmedIgnitionColumn].Value = immedIgnitionProbs[valueIndex];
            }
            for (var rowIndex = 0; rowIndex < delayIgnitionProbs.Length; rowIndex++)
            {
                var valueIndex = rowIndex;
                var thisRow = ignitionProbabilitiesGrid.Rows[rowIndex];
                thisRow.Cells[DelayIgnitionColumn].Value = delayIgnitionProbs[valueIndex].ToString("F4");
            }
        }

        private void StoreIgnitionProbabilities()
        {
            if (!_mIgnoringChangeEvents)
            {
                var immedIgnitionProbs = GetColumnData(ignitionProbabilitiesGrid, ImmedIgnitionColumn);
                var delayIgnitionProbs = GetColumnData(ignitionProbabilitiesGrid, DelayIgnitionColumn);
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
                    StateContainer.SetValue("ImmedIgnitionProbs", immedIgnitionProbs);
                    StateContainer.SetValue("DelayIgnitionProbs", delayIgnitionProbs);
                    StateContainer.SetValue("IgnitionThresholds", ignitionThresholds);
                }
            }
        }


        private double[] GetColumnData(DataGridView dgvControl, int colIndex)
        {
            var result = new double[dgvControl.Rows.Count];
            for (var rowIndex = 0; rowIndex < result.Length; rowIndex++)
            {
                var thisRow = dgvControl.Rows[rowIndex];
                var resultCellValue = double.NaN;
                string cellValue = thisRow.Cells[colIndex].Value.ToString();
                if (ParseUtility.TryParseDouble(cellValue, out resultCellValue))
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
            ParseUtility.TryParseDouble(tbIgnitionProbabilitiesAdd.Text, out newThreshold);
            if (newThreshold > 0.0)
            {
                var ignitionThresholds = StateContainer.GetValue<double[]>("IgnitionThresholds");
                var immedIgnitionProbs = StateContainer.GetValue<double[]>("ImmedIgnitionProbs");
                var delayIgnitionProbs = StateContainer.GetValue<double[]>("DelayIgnitionProbs");
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
                    StateContainer.SetValue("IgnitionThresholds", newIgnitionThresholds);
                    StateContainer.SetValue("ImmedIgnitionProbs", newImmediate);
                    StateContainer.SetValue("DelayIgnitionProbs", newDelayed);

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
                var ignitionThresholds = StateContainer.GetValue<double[]>("IgnitionThresholds");
                var immedIgnitionProbs = StateContainer.GetValue<double[]>("ImmedIgnitionProbs");
                var delayIgnitionProbs = StateContainer.GetValue<double[]>("DelayIgnitionProbs");

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

                StateContainer.SetValue("IgnitionThresholds", newIgnitionThresholds);
                StateContainer.SetValue("ImmedIgnitionProbs", newImmediate);
                StateContainer.SetValue("DelayIgnitionProbs", newDelayed);
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

        private void ignitionProbabilitiesGrid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if ((e.ColumnIndex == DelayIgnitionColumn || e.ColumnIndex == ImmedIgnitionColumn) &&
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
            if (e.ColumnIndex == DelayIgnitionColumn || e.ColumnIndex == ImmedIgnitionColumn)
            {
                double newVal;
                if (!double.TryParse(e.FormattedValue.ToString(), out newVal) || newVal < 0)
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
    }
}