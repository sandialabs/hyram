/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using SandiaNationalLaboratories.Hyram.Resources;


namespace SandiaNationalLaboratories.Hyram
{
    public enum ApButtonClickOption
    {
        PerformClick,
        NoClick
    }

    /// <summary>
    /// Main GUI
    /// </summary>
    public partial class MainForm : Form
    {
        private static UserControl _currentControl;
        //private string genericNavErrorMsg = @"Action could not be completed due to error: ";
        public static MainForm ActiveScreen { get; private set; }
        public Color WarningBackColor = Color.Cornsilk;
        public Color WarningForeColor = Color.DarkGoldenrod;
        public Color ErrorBackColor = Color.MistyRose;
        public Color ErrorForeColor = Color.Maroon;
        //public Color InfoBackColor = Color.LightCyan;
        //public Color InfoForeColor = Color.PaleTurquoise;

        private static SystemDescriptionForm _systemDescriptionForm;
        private static ProbabilitiesForm _probabilitiesForm;
        private static ScenariosForm _scenariosForm;
        private static ConsequenceModelsForm _consequenceModelsForm;
        private static List<AnalysisForm> _qraForms;
        private static List<UserControl> _qraFormControls;
        private static int _mode = 0;  // 0 QRA, 1 physics
        private static string _topAlertMessage = "";
        private static int _topAlertType = 0;


        public MainForm()
        {
            _qraForms = new List<AnalysisForm>();
            _qraFormControls = new List<UserControl>();

            InitializeComponent();
            StateContainer.Instance.InitDatabase();
            GotoAppStartDefaultLocation();
            TheSplashscreen.FadeOut();
        }

        private static Splashscreen _mTheSplashscreen;

        public static Splashscreen TheSplashscreen
        {
            get
            {
                if (_mTheSplashscreen == null) _mTheSplashscreen = new Splashscreen();
                return _mTheSplashscreen;
            }
        }

        // Engineering Toolkit form
        private EtkMainForm _mEtkForm;

        private void MainForm_Load(object sender, EventArgs e)
        {
            ActiveScreen = this;
            modeTabs.SelectedIndex = 0;
            modeTabs.SelectedTab = null;
            modeTabs.SelectedTab = qraModeTab;

            // Populate fuel selection dropdowns.
            // One dropdown on phys UI, one on QRA. Both sync to same backend param.
            physicsFuelTypeSelector.DataSource = StateContainer.Instance.FuelTypes;
            qraFuelTypeSelector.DataSource = StateContainer.Instance.FuelTypes;
            // Hide until other fuels are used
            //fuelTypeLabel1.Visible = false;
            //fuelTypeLabel2.Visible = false;
            //physicsFuelTypeSelector.Visible = false;
            //qraFuelTypeSelector.Visible = false;

            RefreshTopState();
            _mEtkForm = new EtkMainForm();

            // refresh main form when ETK closed
            _mEtkForm.Closing += (o, args) => GotoAppStartDefaultLocation();
        }

        private void RefreshTopState()
        {
            physicsFuelTypeSelector.SelectedItem = StateContainer.GetValue<FuelType>("FuelType");
            qraFuelTypeSelector.SelectedItem = StateContainer.GetValue<FuelType>("FuelType");
        }

        /// <summary>
        /// Child forms call this to let MainForm know of a large change, i.e. to update alert messages.
        /// </summary>
        public void NotifyOfChildPublicStateChange()
        {
            ValidateTopParameters();
        }

        /// <summary>
        /// Update display of notifications and alerts.
        /// Will prioritize top-level messages and hoist child-level messages if current form has none but siblings do.
        /// </summary>
        private void RefreshAlertDisplays()
        {
            int alertType = 0;
            string alertMessage = "";

            if (_topAlertType > 0)
            {
                alertType = _topAlertType;
                alertMessage = _topAlertMessage;
            }
            else
            {
                // no top-level alerts so check child forms
                foreach (AnalysisForm form in _qraForms)
                {
                    alertMessage = form.AlertMessage;
                    alertType = form.AlertType;

                    if (alertType > 0)
                    {
                        break;
                    }
                }
            }

            if (_mode == 1 || alertType == 0)
            {
                mainMessage.Visible = false;
            }
            else
            {
                mainMessage.Visible = true;
                mainMessage.BackColor = (alertType == 1) ? WarningBackColor : ErrorBackColor;
                mainMessage.ForeColor = (alertType == 1) ? WarningForeColor : ErrorForeColor;
            }
            mainMessage.Text = alertMessage;

            // hide if active form already has msg up and planned alert is child-level
            // i.e. only show if it's a top-level alert or childform has no alert but sibling does.
            if ((_mode == 0) && 
                ((_currentControl != null) && ((AnalysisForm) _currentControl).AlertDisplayed && _topAlertType == 0))
            {
                mainMessage.Visible = false;
            }
        }


        private void scenariosFormButton_Click(object sender, EventArgs e)
        {
            if (_scenariosForm == null)
            {
                _scenariosForm = new ScenariosForm(this);
                _qraForms.Add(_scenariosForm);
                _qraFormControls.Add(_scenariosForm);
            }
            ChangePanel(sender, _scenariosForm, Narratives.QraScenariosDescrip);
        }

        private void probabilitiesFormButton_Click(object sender, EventArgs e)
        {
            if (_probabilitiesForm == null)
            {
                _probabilitiesForm = new ProbabilitiesForm(this);
                _qraForms.Add(_probabilitiesForm);
                _qraFormControls.Add(_probabilitiesForm);
            }
            ChangePanel(sender, _probabilitiesForm, Narratives.QraProbabilitiesDescrip);
        }

        private void consequenceModelsFormButton_Click(object sender, EventArgs e)
        {
            if (_consequenceModelsForm == null)
            {
                _consequenceModelsForm = new ConsequenceModelsForm(this);
                _qraForms.Add(_consequenceModelsForm);
            }
            ChangePanel(sender, _consequenceModelsForm, Narratives.QraConsequenceModelsDescrip);
        }

        private void systemDescriptionFormButton_Click(object sender, EventArgs e)
        {
            if (_systemDescriptionForm == null)
            {
                _systemDescriptionForm = new SystemDescriptionForm(this);
                _qraForms.Add(_systemDescriptionForm);
                _qraFormControls.Add(_systemDescriptionForm);
            }
            ChangePanel(sender, _systemDescriptionForm, Narratives.QraSystemDescriptionDescrip);
        }
        private void plumeDispersionFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new PlumeForm(), Narratives.PhysPlumeDescrip, new PlumeDispersionPanel());
        }

        private void accumulationFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new AccumulationForm(), Narratives.PhysAccumulationDescrip, new AccumulationPanel());
        }

        private void jetPlotTempFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new JetFlameTemperaturePlotForm(), Narratives.PhysJetFlameDescrip, new AccumulationPanel());
        }

        private void jetHeatAnalysisFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new JetFlameHeatAnalysisForm(), Narratives.PhysRadHeatDescrip, new AccumulationPanel());
        }

        private void overpressureFormBtn_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new UnconfinedOverpressureForm(), Narratives.PhysUnconfinedOverpressureDescrip,
                new AccumulationPanel());
        }

        /// <summary>
        /// Change inner panel (form) based on button click, e.g. selecting Data & Probabilities button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="nextControl"></param>
        /// <param name="descripFilepath"></param>
        /// <param name="panelToActivate"></param>
        private void ChangePanel(object sender, UserControl nextControl = null, string descripFilepath = null,
            UserControl panelToActivate = null, ApButtonClickOption clickOption = ApButtonClickOption.NoClick)
        {
            // Activate action panel
            SuspendLayout();
            try
            {
                //var modeNavPanel = (Panel) ((Button)sender).Parent;
                if (panelToActivate is null)
                {
                    panelToActivate = new QraOutputNavPanel();
                }

                mainOutputNavPanel.Controls.Clear();
                mainOutputNavPanel.Controls.Add(panelToActivate);
                panelToActivate.Dock = DockStyle.Fill;

                UiStateRoutines.UnselectButtons(qraNavPanel);
                UiStateRoutines.UnselectButtons(physicsNavPanel);

                if (clickOption == ApButtonClickOption.PerformClick)
                {
                    var firstButton = QuickFunctions.GetTopButton(panelToActivate, ChildNavOptions.NavigateChildren);
                    firstButton?.PerformClick();
                }
            }
            finally
            {
                ResumeLayout();
            }

            if (nextControl != null)
            {
                //_currentControl?.Dispose();
                _currentControl = nextControl;
                SetContentScreen((Button) sender, nextControl);
            }

            if (nextControl != null && descripFilepath != null)
            {
                ContentPanel.SetNarrative(nextControl, descripFilepath);
            }
            ValidateTopParameters();
        }

        public static void SetContentScreen(Button caller, UserControl contentControl)
        {
            UiStateRoutines.UnselectButtons(caller.Parent);

            contentControl.Dock = DockStyle.Fill;
            var cpParent = new ContentPanel();
            ActiveScreen.mainFormPanel.Controls.Clear();
            ActiveScreen.mainFormPanel.Controls.Add(cpParent);

            cpParent.Dock = DockStyle.Fill;
            cpParent.ChildControl = contentControl;

            // qra-specific form load function to e.g. refresh data since forms are persisted
            if ((_mode == 0) && _qraFormControls.Contains(contentControl))
            {
                ((AnalysisForm)contentControl).OnFormDisplay();
            }

            caller.ForeColor = Color.Green;
        }

        /// <summary>
        /// Disable QRA content panels and phys tab, e.g. during analysis.
        /// </summary>
        public void DisableNavigation()
        {
            mainOutputNavPanel.Enabled = false;
            navContainer.Panel1.Enabled = false;
            modeTabs.Enabled = false;
        }

        /// <summary>
        /// Enable QRA content panels and phys tab, e.g. after analysis completes
        /// </summary>
        public void EnableNavigation()
        {
            mainOutputNavPanel.Enabled = true;
            navContainer.Panel1.Enabled = true;
            modeTabs.Enabled = true;
        }


        private void ValidateTopParameters()
        {
            _topAlertType = 0;
            _topAlertMessage = "";
            if (_mode == 0)
            {
                // QRA mode
                if (StateContainer.GetValue<FuelType>("FuelType") != FuelType.Hydrogen)
                {
                    _topAlertType = 1;
                    _topAlertMessage = "QRA mode inputs are currently tailored to H2";
                }
            }
            else
            {

            }
            RefreshAlertDisplays();
        }

        /// <summary>
        /// Switch between main analysis modes (QRA and phys)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void modeTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            var activeTab = modeTabs.SelectedTab;
            Panel activePanel = null;

            if (activeTab == qraModeTab)
            {
                activePanel = qraNavPanel;
                _mode = 0;
            }
            else
            {
                activePanel = physicsNavPanel;
                _mode = 1;
            }

            mainOutputNavPanel.Controls.Clear();  // Clear nav panel
            QuickFunctions.GetTopButton(activePanel, ChildNavOptions.NavigateChildren).PerformClick();
            ValidateTopParameters();
        }

        /// <summary>
        /// Reset to System Description tab under QRA mode
        /// </summary>
        private void GotoAppStartDefaultLocation()
        {
            modeTabs.SelectedIndex = 0;
            RefreshTopState();
            systemDescriptionFormButton.PerformClick();
            ValidateTopParameters();

            foreach (AnalysisForm form in _qraForms)
            {
                form.CheckFormValid();
            }
        }


        private void aboutMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.Show();
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new FileSaveLoadForm
                {
                    IsSaveFileForm = true,
                    CurrentLoadedState = StateContainer.Instance
                };
                saveDialog.ShowDialog();
                saveDialog.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not save input file. Error: " + ex.Message);
            }
        }

        private void loadMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var loadDialog = new FileSaveLoadForm
                {
                    IsSaveFileForm = false
                };
                loadDialog.ShowDialog();
                if (loadDialog.CurrentLoadedState != null) StateContainer.Instance = loadDialog.CurrentLoadedState;

                loadDialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not open input file. Error: " + ex.Message);
            }

            StateContainer.Instance.UndoStateDamageCausedByLoad();

            GotoAppStartDefaultLocation();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void resetMenuItem_Click(object sender, EventArgs e)
        {
            var dlgResult = MessageBox.Show(this,
                "Resetting parameters. Restart to revert this action. " +
                "To continue, click \"OK.\" To abort, click \"Cancel.\"",
                "Confirm input reset action", MessageBoxButtons.OKCancel);
            if (dlgResult == DialogResult.OK)
            {
                StateContainer.Instance.ResetInputsAndDefaults();
                //if (ActiveContentPanel != null) ActiveContentPanel.Notify_LoadComplete();
            }
        }

        /// <summary>
        /// Display Engineering Toolkit window
        /// </summary>
        private void etkMenuItem_Click(object sender, EventArgs e)
        {
            _mEtkForm.Visible = true;
            _mEtkForm.TopMost = true;
            _mEtkForm.TopMost = false;
        }

        /// <summary>
        ///     Opens Windows Explorer view of User AppData folder
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataDirectoryMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(StateContainer.UserDataDir);
        }


        private void physicsFuelTypeSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetFuel((FuelType)physicsFuelTypeSelector.SelectedItem);
            GotoAppStartDefaultLocation();
        }

        private void qraFuelTypeSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            StateContainer.SetFuel((FuelType)qraFuelTypeSelector.SelectedItem);
            GotoAppStartDefaultLocation();
        }
    }
}