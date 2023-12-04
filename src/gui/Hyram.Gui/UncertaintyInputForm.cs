/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class UncertaintyInputForm : Form
    {
        private readonly StateContainer _state = State.Data;
        private Parameter _p;
        private int _x;
        private int _y;

        public UncertaintyInputForm(Parameter param, int x, int y)
        {
            InitializeComponent();

            _p = param;
            _x = x;
            _y = y;

            if (_p.Distr is null)
            {
                _p.Distr = SensitivityDistribution.Deterministic;
            }

            paramLabel.Text = _p.Label;

            DistrSelector.DataSource = _state.SensitivityDistributions;
            DistrSelector.SelectedItem = _p.Distr;

            UncertaintySelector.DataSource = _state.UncertaintyTypes;
            DistrSelector.SelectedItem = _p.Uncertainty;

            RefreshForm();

            Load += Form_Load;
//            Deactivate += delegate { ValidateAndClose(); };
        }

        private void RefreshForm()
        {
            if (_p.Distr == SensitivityDistribution.Deterministic)
            {
                UncertaintySelector.Enabled = false;
                ParamAInput.Enabled = false;
                ParamBInput.Enabled = false;
            }
            else
            {
                UncertaintySelector.Enabled = true;
                ParamAInput.Enabled = true;
                ParamBInput.Enabled = true;
            }

            ValueInput.Text = _p.GetDisplayValue().ToString();
            ParamAInput.Text = _p.DisplayParamA().ToString();
            ParamBInput.Text = _p.DisplayParamB().ToString();

            if (_p.Distr == SensitivityDistribution.Uniform)
            {
                DistrSelector.SelectedItem = SensitivityDistribution.Uniform;
                ParamALabel.Text = "Lower bound";
                ParamBLabel.Text = "Upper bound";
            }
            else
            {
                ParamALabel.Text = "Mean";
                ParamBLabel.Text = "Std dev";
                
            }
        }

        private bool IsValid()
        {
            var msg = "";
            var valid = true;

            if (_p.Distr != SensitivityDistribution.Deterministic)
            {
                if (_p.Uncertainty == UncertaintyType.None)
                {
                    valid = false;
                    msg = "Select an uncertainty type";
                }
            }

            if (_p.Distr == SensitivityDistribution.Uniform)
            {
                if (_p.ParamA == null)
                {
                    valid = false;
                    msg = "Enter a lower bound";
                }
                else if (_p.ParamA >= _p.ParamB)
                {
                    valid = false;
                    msg = "Enter a lower bound that is less than the upper bound";
                }
                else if (_p.ParamB == null)
                {
                    valid = false;
                    msg = "Enter an upper bound";
                }
                else if (_p.ParamB <= _p.ParamA)
                {
                    valid = false;
                    msg = "Enter an upper bound that is greater than the lower bound";
                }
            }
            if (_p.Distr == SensitivityDistribution.LogNormal || _p.Distr == SensitivityDistribution.Normal)
            {
                if (_p.ParamA == null)
                {
                    valid = false;
                    msg = "Enter a mean";
                }
                if (_p.ParamB == null)
                {
                    valid = false;
                    msg = "Enter a standard deviation";
                }
            }

            if (string.IsNullOrEmpty(_p.GetValue().ToString()))
            {
                valid = false;
                msg = "Enter a nominal value";
            }

            alertLabel.Text = msg;
            alertLabel.Visible = !valid;

            return valid;
        }

        private void ClearAlert()
        {
            alertLabel.Visible = false;
        }

        private void Form_Load(object sender, EventArgs e)
        {
            SetDesktopLocation(_x - 600, _y - 100);
        }

        private void ValidateAndClose()
        {
            Debug.WriteLine("Validating " + IsValid());
            if (IsValid())
            {
                Close();
            }
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
            ValidateAndClose();
        }

        private void DistrSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _p.Distr = (ModelPair)DistrSelector.SelectedItem;

            if (_p.Distr == SensitivityDistribution.Deterministic)
            {
                _p.Uncertainty = UncertaintyType.None;
            }

            ClearAlert();
            RefreshForm();
        }

        private void ValueInput_TextChanged(object sender, EventArgs e)
        {
            FormHelpers.HandleParameterValueChange(ValueInput, sender, e, _p);
            if (alertLabel.Visible) IsValid();
        }

        private void ParamAInput_TextChanged(object sender, EventArgs e)
        {
            FormHelpers.HandleParameterParamAChange(ParamAInput, sender, e, _p);
            if (alertLabel.Visible) IsValid();
        }

        private void ParamBInput_TextChanged(object sender, EventArgs e)
        {
            FormHelpers.HandleParameterParamBChange(ParamBInput, sender, e, _p);
            if (alertLabel.Visible) IsValid();
        }

        private void UncertaintySelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _p.Uncertainty = (ModelPair)UncertaintySelector.SelectedItem;
            RefreshForm();
            if (alertLabel.Visible) IsValid();
        }
    }
}
