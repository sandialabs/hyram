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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DefaultParsing;
using JrConversions;
using MathNet.Numerics.Distributions;
using QRA_Frontend.Resources;
using QRAState;

namespace QRA_Frontend.ContentPanels
{
    public partial class CpDataProbabilities : UserControl, IQraBaseNotify
    {
        private const int ColImmed = 1;
        private const int ColDelayed = 2;

        private readonly List<ComponentProbability> _compressorList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Compressor");

        private readonly FailureMode _couplingFtc = QraStateContainer.GetValue<FailureMode>("Failure.CouplingFTC");

        private readonly List<ComponentProbability> _cylList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Cylinder");

        private readonly FailureMode _driveoffFail = QraStateContainer.GetValue<FailureMode>("Failure.Driveoff");

        private readonly List<ComponentProbability> _ex1List =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Extra1");

        private readonly List<ComponentProbability> _ex2List =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Extra2");

        private readonly List<ComponentProbability> _filterList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Filter");

        private readonly List<ComponentProbability> _flangeList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Flange");

        private readonly List<ComponentProbability> _hoseList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Hose");

        private readonly List<ComponentProbability>
            _instrList = QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Instrument");

        private readonly List<ComponentProbability> _jointList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Joint");

        private bool _mIgnoringChangeEvents;
        private readonly FailureMode _mValveFtc = QraStateContainer.GetValue<FailureMode>("Failure.MValveFTC");
        private readonly FailureMode _nozFtc = QraStateContainer.GetValue<FailureMode>("Failure.NozFTC");
        private readonly FailureMode _nozPo = QraStateContainer.GetValue<FailureMode>("Failure.NozPO");

        private readonly FailureMode _overpFail = QraStateContainer.GetValue<FailureMode>("Failure.Overp");

        private readonly List<ComponentProbability> _pipeList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Pipe");

        private readonly FailureMode _pValveFto = QraStateContainer.GetValue<FailureMode>("Failure.PValveFTO");
        private readonly FailureMode _sValveCcf = QraStateContainer.GetValue<FailureMode>("Failure.SValveCCF");
        private readonly FailureMode _sValveFtc = QraStateContainer.GetValue<FailureMode>("Failure.SValveFTC");

        private readonly List<ComponentProbability> _valveList =
            QraStateContainer.GetValue<List<ComponentProbability>>("Prob.Valve");


        public CpDataProbabilities()
        {
            InitializeComponent();
        }

        void IQraBaseNotify.Notify_LoadComplete()
        {
            //SetNarrative();
            ContentPanel.SetNarrative(this, Narratives.DP__Data_Probabilities);
            SetDischargeCoefficient();

            UpdateIgnitionProbablitiesList();
            FillIgnitionProbabilitiesTable();

            QraStateContainer.SetValue("ResultsAreStale", true);
            _mIgnoringChangeEvents = false;
        }

        /// <summary>
        ///     Set up data-binding between DB and displayed tables
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cpDataProbabilities_Load(object sender, EventArgs e)
        {
            var qraInst = QraStateContainer.Instance;
            // Data-binding for component distributions
            dgCompressors.DataSource = new BindingSource(new BindingList<ComponentProbability>(_compressorList), null);
            dgCylinders.DataSource = new BindingSource(new BindingList<ComponentProbability>(_cylList), null);
            dgFilters.DataSource = new BindingSource(new BindingList<ComponentProbability>(_filterList), null);
            dgFlanges.DataSource = new BindingSource(new BindingList<ComponentProbability>(_flangeList), null);
            dgHoses.DataSource = new BindingSource(new BindingList<ComponentProbability>(_hoseList), null);
            dgJoints.DataSource = new BindingSource(new BindingList<ComponentProbability>(_jointList), null);
            dgPipes.DataSource = new BindingSource(new BindingList<ComponentProbability>(_pipeList), null);
            dgValves.DataSource = new BindingSource(new BindingList<ComponentProbability>(_valveList), null);
            dgInstruments.DataSource = new BindingSource(new BindingList<ComponentProbability>(_instrList), null);
            dgExtra1.DataSource = new BindingSource(new BindingList<ComponentProbability>(_ex1List), null);
            dgExtra2.DataSource = new BindingSource(new BindingList<ComponentProbability>(_ex2List), null);

            // Data-binding for failure modes
            // First two columns should be greyed out
            clmCFComponent.DefaultCellStyle.BackColor = Color.LightGray;
            clmCFFailureMode.DefaultCellStyle.BackColor = Color.LightGray;
            clmCFDistributionType.DataSource = Enum.GetValues(typeof(FailureDistributionType));

            clmAPCompName.DefaultCellStyle.BackColor = Color.LightGray;
            clmAPFailMode.DefaultCellStyle.BackColor = Color.LightGray;
            clmAPDistributionType.DataSource = Enum.GetValues(typeof(FailureDistributionType));

            var compFailures = new List<FailureMode> {_nozPo, _nozFtc, _mValveFtc, _sValveFtc, _sValveCcf};
            var accidentFailures = new List<FailureMode>
                {_overpFail, _pValveFto, _driveoffFail, _couplingFtc};

            dgComponentFailures.DataSource = new BindingSource(new BindingList<FailureMode>(compFailures), null);
            dgAccidentProbabilities.DataSource =
                new BindingSource(new BindingList<FailureMode>(accidentFailures), null);

            // Toggle ParamB input based on distribution selection
            dgComponentFailures.CellFormatting += ComponentFailures_CellFormatting;
            dgAccidentProbabilities.CellFormatting += AccidentFailures_CellFormatting;

            dgCompressors.CellEndEdit += dgCompressors_CellEndEdit;
            dgCylinders.CellEndEdit += dgCylinders_CellEndEdit;
            dgFilters.CellEndEdit += dgFilters_CellEndEdit;
            dgFlanges.CellEndEdit += dgFlanges_CellEndEdit;
            dgHoses.CellEndEdit += dgHoses_CellEndEdit;
            dgJoints.CellEndEdit += dgJoints_CellEndEdit;
            dgPipes.CellEndEdit += dgPipes_CellEndEdit;
            dgValves.CellEndEdit += dgValves_CellEndEdit;
            dgInstruments.CellEndEdit += dgInstruments_CellEndEdit;
            dgExtra1.CellEndEdit += dgExtra1_CellEndEdit;
            dgExtra2.CellEndEdit += dgExtra2_CellEndEdit;

            dgCompressors.DataError += dgCompressors_DataError;
            dgCylinders.DataError += dgCylinders_DataError;
            dgFilters.DataError += dgFilters_DataError;
            dgFlanges.DataError += dgFlanges_DataError;
            dgHoses.DataError += dgHoses_DataError;
            dgJoints.DataError += dgJoints_DataError;
            dgPipes.DataError += dgPipes_DataError;
            dgValves.DataError += dgValves_DataError;
            dgInstruments.DataError += dgInstruments_DataError;
            dgExtra1.DataError += dgExtra1_DataError;
            dgExtra2.DataError += dgExtra2_DataError;

            // Format cells
            DataGridView[] componentProbTabs =
            {
                dgCompressors, dgCylinders, dgFilters, dgFlanges, dgHoses, dgJoints, dgPipes,
                dgValves, dgInstruments, dgExtra1, dgExtra2
            };

            foreach (var probTab in componentProbTabs)
            {
                probTab.Columns[(int) Column.Mu].DefaultCellStyle.Format = "N4";
                probTab.Columns[(int) Column.Mu].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.Sigma].DefaultCellStyle.Format = "N4";
                probTab.Columns[(int) Column.Sigma].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.Mean].DefaultCellStyle.Format = "E2";
                probTab.Columns[(int) Column.Mean].DefaultCellStyle.NullValue = "N/A";
                probTab.Columns[(int) Column.Variance].DefaultCellStyle.Format = "E2";
                probTab.Columns[(int) Column.Variance].DefaultCellStyle.NullValue = "N/A";
            }
        }

        /// <summary>
        ///     Update component probability for given component.
        ///     Re-computes mean and variance if mu/sigma changed. Clears mu/sigma if mean or variance changed.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="compProb"></param>
        private void UpdateComponentProbabilityData(int row, Column col, ComponentProbability compProb,
            DataGridView probTable)
        {
            // If user changes mu/sigma, re-compute mean and var. If mean/var changed, clear mu/sigma.
            if (col == Column.Mu || col == Column.Sigma)
            {
                // mu or sigma changed so re-compute mean, variance. If one is null (i.e. user hasn't provided both yet), just clear mean and var
                compProb.Mean = null;
                compProb.Variance = null;
                var mu = (double?) probTable.Rows[row].Cells[(int) Column.Mu].Value;
                var sigma = (double?) probTable.Rows[row].Cells[(int) Column.Sigma].Value;

                if (mu == null || sigma == null)
                {
                    // User hasn't provided both yet so exit
                }
                else
                {
                    UpdateMeanAndVariance((double) mu, (double) sigma, out var sigmaCorrectedToZero, out var mean,
                        out var variance);
                    compProb.Mu = mu;
                    compProb.Sigma = sigma;
                    compProb.Mean = mean;
                    compProb.Variance = variance;
                }
            }
            else if (col == Column.Mean || col == Column.Variance)
            {
                // mean or variance changed so clear mu/sigma
                compProb.Mu = null;
                compProb.Sigma = null;
            }
        }

        private void dgCompressors_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _compressorList[e.RowIndex],
                dgCompressors);
        }

        private void dgCylinders_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _cylList[e.RowIndex], dgCylinders);
        }

        private void dgFilters_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _filterList[e.RowIndex], dgFilters);
        }

        private void dgFlanges_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _flangeList[e.RowIndex], dgFlanges);
        }

        private void dgHoses_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _hoseList[e.RowIndex], dgHoses);
        }

        private void dgJoints_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _jointList[e.RowIndex], dgJoints);
        }

        private void dgPipes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _pipeList[e.RowIndex], dgPipes);
        }

        private void dgValves_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _valveList[e.RowIndex], dgValves);
        }

        private void dgInstruments_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _instrList[e.RowIndex], dgInstruments);
        }

        private void dgExtra1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _ex1List[e.RowIndex], dgExtra1);
        }

        private void dgExtra2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            UpdateComponentProbabilityData(e.RowIndex, (Column) e.ColumnIndex, _ex2List[e.RowIndex], dgExtra2);
        }

        // Catch invalid data and show message
        // TODO: Fix msg display
        private void dgCompressors_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // If the data source raises an exception when a cell value is 
            // commited, display an error message.
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgCylinders_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgFilters_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgFlanges_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgHoses_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgJoints_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgPipes_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgValves_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgInstruments_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgExtra1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
                MessageBox.Show("Cell value must be numeric");
        }

        private void dgExtra2_DataError(object sender, DataGridViewDataErrorEventArgs e)
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
                var cell = dgComponentFailures.Rows[row].Cells[col];
                // Get the distribution type of that row
                var dist = (FailureDistributionType) dgComponentFailures.Rows[row].Cells[2].Value;

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
                var cell = dgAccidentProbabilities.Rows[row].Cells[col];
                // Get the distribution type of that row
                var dist =
                    (FailureDistributionType) dgAccidentProbabilities.Rows[row].Cells[2].Value;

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

        private void tbDischargeCoefficient_TextChanged(object sender, EventArgs e)
        {
            var cD = double.NaN;
            if (Parsing.TryParseDouble(tbDischargeCoefficient.Text, out cD))
                //ndConvertibleValue ValueNode = new ndConvertibleValue(StockConverters.UnitlessConverter, UnitlessUnit.Unitless, new double[] { C_D });
                QraStateContainer.SetNdValue("DischargeCoefficient", UnitlessUnit.Unitless, cD);
        }

        private void SetDischargeCoefficient()
        {
            var cD = QraStateContainer.Instance.GetStateDefinedValueObject("DischargeCoefficient")
                .GetValue(UnitlessUnit.Unitless)[0];
            tbDischargeCoefficient.Text = Parsing.DoubleToString(cD);
        }


        // TODO (Cianan): implement logic paths for when user changes mu and sigma vs. mean and variance
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

            var ignitionThresholds = QraStateContainer.GetValue<double[]>("IgnitionThresholds");

            if (ignitionThresholds.Length > 0)
            {
                dgIgnitionProbabilities.Rows.Add("<" + ignitionThresholds[0], "", "");
                for (var i = 1; i < ignitionThresholds.Length; ++i)
                    dgIgnitionProbabilities.Rows.Add(ignitionThresholds[i - 1] + "-" + ignitionThresholds[i], "", "");

                dgIgnitionProbabilities.Rows.Add("\u2265" + ignitionThresholds[ignitionThresholds.Length - 1], "", "");
                FillIgnitionProbabilitiesTableFromMemory();
            }

            _mIgnoringChangeEvents = false;
        }

        private void UpdateIgnitionProbablitiesList()
        {
            var ignitionThresholds = QraStateContainer.GetValue<double[]>("IgnitionThresholds");
            for (var i = 0; i < ignitionThresholds.Length; ++i)
                lbIgnitionProbabilities.Items.Add(ignitionThresholds[i]);
        }

        private void FillIgnitionProbabilitiesTableFromMemory()
        {
            var immedIgnitionProbs = QraStateContainer.GetValue<double[]>("ImmedIgnitionProbs");
            var delayIgnitionProbs = QraStateContainer.GetValue<double[]>("DelayIgnitionProbs");

            SetDatagridColumn(dgIgnitionProbabilities, ColImmed, immedIgnitionProbs);
            SetDatagridColumn(dgIgnitionProbabilities, ColDelayed, delayIgnitionProbs);
        }

        private void HarvestIgnitionProbabilitiesTable()
        {
            if (!_mIgnoringChangeEvents)
            {
                var immedIgnitionProbs = HarvestDatagridColumn(dgIgnitionProbabilities, ColImmed);
                var delayIgnitionProbs = HarvestDatagridColumn(dgIgnitionProbabilities, ColDelayed);
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
                    QraStateContainer.SetValue("ImmedIgnitionProbs", immedIgnitionProbs);
                    QraStateContainer.SetValue("DelayIgnitionProbs", delayIgnitionProbs);
                    QraStateContainer.SetValue("IgnitionThresholds", ignitionThresholds);
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
                if (Parsing.TryParseDouble(cellValue, out resultCellValue)) result[rowIndex] = resultCellValue;
            }

            return result;
        }

        private void dgIgnitionProbabilities_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            HarvestIgnitionProbabilitiesTable();
        }

        private void btnIgnitionProbabilitiesAdd_Click(object sender, EventArgs e)
        {
            double newThreshold = -1;
            Parsing.TryParseDouble(tbIgnitionProbabilitiesAdd.Text, out newThreshold);
            if (newThreshold > 0.0)
            {
                var ignitionThresholds = QraStateContainer.GetValue<double[]>("IgnitionThresholds");
                var immedIgnitionProbs = QraStateContainer.GetValue<double[]>("ImmedIgnitionProbs");
                var delayIgnitionProbs = QraStateContainer.GetValue<double[]>("DelayIgnitionProbs");
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

                    QraStateContainer.SetValue("IgnitionThresholds", newIgnitionThresholds);
                    QraStateContainer.SetValue("ImmedIgnitionProbs", newImmediate);
                    QraStateContainer.SetValue("DelayIgnitionProbs", newDelayed);
                    lbIgnitionProbabilities.Items.Clear();
                    dgIgnitionProbabilities.Rows.Clear();
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
                var ignitionThresholds = QraStateContainer.GetValue<double[]>("IgnitionThresholds");
                var immedIgnitionProbs = QraStateContainer.GetValue<double[]>("ImmedIgnitionProbs");
                var delayIgnitionProbs = QraStateContainer.GetValue<double[]>("DelayIgnitionProbs");

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

                QraStateContainer.SetValue("IgnitionThresholds", newIgnitionThresholds);
                QraStateContainer.SetValue("ImmedIgnitionProbs", newImmediate);
                QraStateContainer.SetValue("DelayIgnitionProbs", newDelayed);
                lbIgnitionProbabilities.Items.Clear();
                dgIgnitionProbabilities.Rows.Clear();
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
            LeakSize = 0,
            Mu,
            Sigma,
            Mean,
            Variance
        }
    }
}