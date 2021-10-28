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
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MathNet.Numerics.Distributions;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class ProbabilitiesForm : AnalysisForm
    {
        private const int ColImmed = 1;
        private const int ColDelayed = 2;

        private readonly List<ComponentProbability> _compressorProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Compressor");

        private readonly FailureMode _breakawayCouplingFailureToClose = StateContainer.GetValue<FailureMode>("Failure.CouplingFTC");

        private readonly FailureMode _driveoffFail = StateContainer.GetValue<FailureMode>("Failure.Driveoff");

        private readonly List<ComponentProbability> _vesselProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Vessel");

        private readonly List<ComponentProbability> _filterProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Filter");

        private readonly List<ComponentProbability> _flangeProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Flange");

        private readonly List<ComponentProbability> _hoseProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Hose");

        private readonly List<ComponentProbability>
            _instrProbs = StateContainer.GetValue<List<ComponentProbability>>("Prob.Instrument");

        private readonly List<ComponentProbability> _jointProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Joint");

        private readonly List<ComponentProbability> _pipeProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Pipe");

        private readonly List<ComponentProbability> _valveProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Valve");

        private readonly List<ComponentProbability> _exchangerProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Exchanger");

        private readonly List<ComponentProbability> _vaporizerProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Vaporizer");

        private readonly List<ComponentProbability> _armProbs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Arm");

        private readonly List<ComponentProbability> _ex1Probs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Extra1");

        private readonly List<ComponentProbability> _ex2Probs =
            StateContainer.GetValue<List<ComponentProbability>>("Prob.Extra2");

        private bool _mIgnoringChangeEvents;
        private readonly FailureMode _manualValveFailureToClose = StateContainer.GetValue<FailureMode>("Failure.MValveFTC");
        private readonly FailureMode _nozzleFailureToClose = StateContainer.GetValue<FailureMode>("Failure.NozFTC");
        private readonly FailureMode _nozzlePopoffFailure = StateContainer.GetValue<FailureMode>("Failure.NozPO");
        private readonly FailureMode _overpressureFailure = StateContainer.GetValue<FailureMode>("Failure.Overp");
        private readonly FailureMode _pressureValveFailureToOpen = StateContainer.GetValue<FailureMode>("Failure.PValveFTO");
        private readonly FailureMode _solenoidValveCommonFailure = StateContainer.GetValue<FailureMode>("Failure.SValveCCF");
        private readonly FailureMode _solenoidValveFailureToClose = StateContainer.GetValue<FailureMode>("Failure.SValveFTC");

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
            // Data-binding for component distributions
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

            // Data-binding for failure modes
            // First two columns should be greyed out
            clmCFComponent.DefaultCellStyle.BackColor = Color.LightGray;
            clmCFFailureMode.DefaultCellStyle.BackColor = Color.LightGray;
            clmCFDistributionType.DataSource = Enum.GetValues(typeof(FailureDistributionType));

            clmAPCompName.DefaultCellStyle.BackColor = Color.LightGray;
            clmAPCompName.ReadOnly = true;
            clmAPFailMode.DefaultCellStyle.BackColor = Color.LightGray;
            clmAPDistributionType.DataSource = Enum.GetValues(typeof(FailureDistributionType));

            var compFailures = new List<FailureMode>
            {
                _nozzlePopoffFailure,
                _nozzleFailureToClose,
                _manualValveFailureToClose,
                _solenoidValveFailureToClose,
                _solenoidValveCommonFailure,
                _pressureValveFailureToOpen,
                _breakawayCouplingFailureToClose
            };
            var accidentFailures = new List<FailureMode>
            {
                _overpressureFailure,
                _driveoffFail
            };

            componentFailuresGrid.DataSource = new BindingSource(new BindingList<FailureMode>(compFailures), null);
            accidentProbabilitiesGrid.DataSource =
                new BindingSource(new BindingList<FailureMode>(accidentFailures), null);

            // Toggle ParamB input based on distribution selection
            componentFailuresGrid.CellFormatting += ComponentFailures_CellFormatting;
            accidentProbabilitiesGrid.CellFormatting += AccidentFailures_CellFormatting;

            compressorDistributionsGrid.CellEndEdit += compressorDistributionsGrid_CellEndEdit;
            cylinderDistributionsGrid.CellEndEdit += cylinderDistributionsGrid_CellEndEdit;
            filterDistributionsGrid.CellEndEdit += filterDistributionsGrid_CellEndEdit;
            flangeDistributionsGrid.CellEndEdit += flangeDistributionsGrid_CellEndEdit;
            hoseDistributionsGrid.CellEndEdit += hoseDistributionsGrid_CellEndEdit;
            jointDistributionsGrid.CellEndEdit += jointDistributionsGrid_CellEndEdit;
            pipeDistributionsGrid.CellEndEdit += pipeDistributionsGrid_CellEndEdit;
            valveDistributionsGrid.CellEndEdit += valveDistributionsGrid_CellEndEdit;
            instrumentDistributionsGrid.CellEndEdit += instrumentDistributionsGrid_CellEndEdit;
            extraComponent1DistributionsGrid.CellEndEdit += extraComponent1DistributionsGrid_CellEndEdit;
            extraComponent2DistributionsGrid.CellEndEdit += extraComponent2DistributionsGrid_CellEndEdit;

            compressorDistributionsGrid.DataError += compressorDistributionsGrid_DataError;
            cylinderDistributionsGrid.DataError += cylinderDistributionsGrid_DataError;
            filterDistributionsGrid.DataError += filterDistributionsGrid_DataError;
            flangeDistributionsGrid.DataError += flangeDistributionsGrid_DataError;
            hoseDistributionsGrid.DataError += hoseDistributionsGrid_DataError;
            jointDistributionsGrid.DataError += jointDistributionsGrid_DataError;
            pipeDistributionsGrid.DataError += pipeDistributionsGrid_DataError;
            valveDistributionsGrid.DataError += valveDistributionsGrid_DataError;
            instrumentDistributionsGrid.DataError += instrumentDistributionsGrid_DataError;
            extraComponent1DistributionsGrid.DataError += extraComponent1DistributionsGrid_DataError;
            extraComponent2DistributionsGrid.DataError += extraComponent2DistributionsGrid_DataError;

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

                //probTab.Columns[(int) Column.Fifth].HeaderText = "5th";
                probTab.Columns[(int) Column.Fifth].DefaultCellStyle.Format = "E1";
                probTab.Columns[(int) Column.Fifth].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.Fifth].DefaultCellStyle.BackColor = Color.LightGray;
                probTab.Columns[(int) Column.Fifth].ReadOnly = true;

                probTab.Columns[(int) Column.Median].DefaultCellStyle.Format = "E1";
                probTab.Columns[(int) Column.Median].DefaultCellStyle.NullValue = "N/A";

                //probTab.Columns[(int) Column.NinetyFifth].HeaderText = "95th";
                probTab.Columns[(int) Column.NinetyFifth].DefaultCellStyle.Format = "E1";
                probTab.Columns[(int) Column.NinetyFifth].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.NinetyFifth].DefaultCellStyle.BackColor = Color.LightGray;
                probTab.Columns[(int) Column.NinetyFifth].ReadOnly = true;

            }

            SetDischargeCoefficient();

            UpdateIgnitionProbablitiesList();
            FillIgnitionProbabilitiesTable();

            StateContainer.SetValue("ResultsAreStale", true);
            _mIgnoringChangeEvents = false;
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
                AlertMessage = "Default data for failures and ignition thresholds were generated for " +
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
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="compProb"></param>
        private void UpdateLeakProbability(int row, Column col, ComponentProbability leakData, DataGridView grid)
        {
            if (col == Column.Mu) leakData.Mu = (double)grid.Rows[row].Cells[(int) Column.Mu].Value;
            if (col == Column.Sigma) leakData.Sigma = (double)grid.Rows[row].Cells[(int) Column.Sigma].Value;
            // if user changes median, re-calculate mu and then other parameters from it
            if (col == Column.Median) leakData.Median = (double)grid.Rows[row].Cells[(int) Column.Median].Value;
        }

        private void compressorDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _compressorProbs[e.RowIndex],
                compressorDistributionsGrid);
        }

        private void cylinderDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _vesselProbs[e.RowIndex], cylinderDistributionsGrid);
        }

        private void filterDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _filterProbs[e.RowIndex], filterDistributionsGrid);
        }

        private void flangeDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _flangeProbs[e.RowIndex], flangeDistributionsGrid);
        }

        private void hoseDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _hoseProbs[e.RowIndex], hoseDistributionsGrid);
        }

        private void jointDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _jointProbs[e.RowIndex], jointDistributionsGrid);
        }

        private void pipeDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _pipeProbs[e.RowIndex], pipeDistributionsGrid);
        }

        private void valveDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _valveProbs[e.RowIndex], valveDistributionsGrid);
        }

        private void instrumentDistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _instrProbs[e.RowIndex], instrumentDistributionsGrid);
        }

        private void exchangerGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _exchangerProbs[e.RowIndex], exchangerGrid);
        }

        private void vaporizerGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _vaporizerProbs[e.RowIndex], vaporizerGrid);
        }

        private void armGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _armProbs[e.RowIndex], armGrid);
        }

        private void extraComponent1DistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _ex1Probs[e.RowIndex], extraComponent1DistributionsGrid);
        }

        private void extraComponent2DistributionsGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateLeakProbability(e.RowIndex, (Column) e.ColumnIndex, _ex2Probs[e.RowIndex], extraComponent2DistributionsGrid);
        }

        // Catch invalid data and show message
        // TODO: Fix msg display
        private void compressorDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // If the data source raises an exception when a cell value is 
            // commited, display an error message.
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void cylinderDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void filterDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void flangeDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void hoseDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void jointDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void pipeDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void valveDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void instrumentDistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void exchangerGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Cell value must be numeric");
            }
        }

        private void vaporizerGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Cell value must be numeric");
            }
        }

        private void armGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
            {
                MessageBox.Show("Cell value must be numeric");
            }
        }

        private void extraComponent1DistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void extraComponent2DistributionsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        /// <summary>
        ///     If failure mode uses Expected Value distribution, disable the Parameter B input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        private void dischargeCoefficientInput_TextChanged(object sender, EventArgs e)
        {
            var cD = double.NaN;
            if (ParseUtility.TryParseDouble(dischargeCoefficientInput.Text, out cD))
                //ndConvertibleValue ValueNode = new ndConvertibleValue(StockConverters.UnitlessConverter, UnitlessUnit.Unitless, new double[] { C_D });
                StateContainer.SetNdValue("DischargeCoefficient", UnitlessUnit.Unitless, cD);
        }

        private void SetDischargeCoefficient()
        {
            var cD = StateContainer.Instance.GetStateDefinedValueObject("DischargeCoefficient")
                .GetValue(UnitlessUnit.Unitless)[0];
            dischargeCoefficientInput.Text = ParseUtility.DoubleToString(cD);
        }


        private void UpdateMeanAndVariance(double mu, double sigma, out bool sigmaCorrectedToZero, out double mean,
            out double variance)
        {
            sigmaCorrectedToZero = false;
            if (sigma < 0)
            {
                sigma = 0;
                sigmaCorrectedToZero = true;
            }

            var logNormalDist = new LogNormal(mu, sigma);
            variance = logNormalDist.Variance;
            mean = logNormalDist.Mean;
        }

        private void FillIgnitionProbabilitiesTable()
        {
            _mIgnoringChangeEvents = true;

            var ignitionThresholds = StateContainer.GetValue<double[]>("IgnitionThresholds");

            if (ignitionThresholds.Length > 0)
            {
                ignitionProbabilitiesGrid.Rows.Add("<" + ignitionThresholds[0], "", "");
                for (var i = 1; i < ignitionThresholds.Length; ++i)
                    ignitionProbabilitiesGrid.Rows.Add(ignitionThresholds[i - 1] + "-" + ignitionThresholds[i], "", "");

                ignitionProbabilitiesGrid.Rows.Add("\u2265" + ignitionThresholds[ignitionThresholds.Length - 1], "", "");
                FillIgnitionProbabilitiesTableFromMemory();
            }

            _mIgnoringChangeEvents = false;
        }

        private void UpdateIgnitionProbablitiesList()
        {
            var ignitionThresholds = StateContainer.GetValue<double[]>("IgnitionThresholds");
            for (var i = 0; i < ignitionThresholds.Length; ++i)
                lbIgnitionProbabilities.Items.Add(ignitionThresholds[i]);
        }

        private void FillIgnitionProbabilitiesTableFromMemory()
        {
            var immedIgnitionProbs = StateContainer.GetValue<double[]>("ImmedIgnitionProbs");
            var delayIgnitionProbs = StateContainer.GetValue<double[]>("DelayIgnitionProbs");

            SetDatagridColumn(ignitionProbabilitiesGrid, ColImmed, immedIgnitionProbs);
            SetDatagridColumn(ignitionProbabilitiesGrid, ColDelayed, delayIgnitionProbs);
        }

        private void HarvestIgnitionProbabilitiesTable()
        {
            if (!_mIgnoringChangeEvents)
            {
                var immedIgnitionProbs = HarvestDatagridColumn(ignitionProbabilitiesGrid, ColImmed);
                var delayIgnitionProbs = HarvestDatagridColumn(ignitionProbabilitiesGrid, ColDelayed);
                var ignitionThresholds = new double[lbIgnitionProbabilities.Items.Count];
                var lbItems = new object[lbIgnitionProbabilities.Items.Count];
                lbIgnitionProbabilities.Items.CopyTo(lbItems, 0);

                for (var i = 0; i < ignitionThresholds.Length; ++i) ignitionThresholds[i] = (double) lbItems[i];

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
                    //CalculateIgnitionProbabilities.Execute(ImmedIgnitionProbs, DelayIgnitionProbs, IgnitionThresholds);
                    _mIgnoringChangeEvents = true;
                    FillIgnitionProbabilitiesTableFromMemory();
                    _mIgnoringChangeEvents = false;
                }
            }
        }


        private void SetDatagridColumn(DataGridView dgvControl, int colIndex, double[] values)
        {
            for (var rowIndex = 0; rowIndex < values.Length; rowIndex++)
            {
                var valueIndex = rowIndex;
                var thisRow = dgvControl.Rows[rowIndex];
                thisRow.Cells[colIndex].Value = values[valueIndex].ToString("F4");
            }
        }

        private double[] HarvestDatagridColumn(DataGridView dgvControl, int colIndex)
        {
            var result = new double[dgvControl.Rows.Count];
            for (var rowIndex = 0; rowIndex < result.Length; rowIndex++)
            {
                var thisRow = dgvControl.Rows[rowIndex];
                var resultCellValue = double.NaN;
                var cellValue = (string) thisRow.Cells[colIndex].Value;
                if (ParseUtility.TryParseDouble(cellValue, out resultCellValue)) result[rowIndex] = resultCellValue;
            }

            return result;
        }

        private void ignitionProbabilitiesGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HarvestIgnitionProbabilitiesTable();
        }

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
                    for (var i = 0; i < newIgnitionThresholds.Length; ++i) //Insert new ignition threshold in order
                        if (offset == 0 && (i == newIgnitionThresholds.Length - 1 ||
                                            ignitionThresholds[i] > newThreshold))
                        {
                            newIgnitionThresholds[i] = newThreshold;
                            index = i;
                            ++offset;
                        }
                        else
                        {
                            newIgnitionThresholds[i] = ignitionThresholds[i - offset];
                        }

                    offset = 0;
                    for (var i = 0; i < newImmediate.Length; ++i)
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

                    StateContainer.SetValue("IgnitionThresholds", newIgnitionThresholds);
                    StateContainer.SetValue("ImmedIgnitionProbs", newImmediate);
                    StateContainer.SetValue("DelayIgnitionProbs", newDelayed);
                    lbIgnitionProbabilities.Items.Clear();
                    ignitionProbabilitiesGrid.Rows.Clear();
                    UpdateIgnitionProbablitiesList();
                    FillIgnitionProbabilitiesTable();
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
            var thresholdToRemove = Convert.ToDouble(lbIgnitionProbabilities.SelectedItem);
            if (lbIgnitionProbabilities.Items.Count > 1)
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
                lbIgnitionProbabilities.Items.Clear();
                ignitionProbabilitiesGrid.Rows.Clear();
                UpdateIgnitionProbablitiesList();
                FillIgnitionProbabilitiesTable();
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
    }
}