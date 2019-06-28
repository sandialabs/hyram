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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using QRA_Frontend.ActionPanels;
using QRA_Frontend.ContentPanels;
using QRA_Frontend.ETK;
using QRA_Frontend.Resources;
using QRAState;
using UIHelpers;

namespace QRA_Frontend
{
    public enum ApButtonClickOption
    {
        PerformClick,
        NoClick
    }

    /// <summary>
    /// Main GUI
    /// </summary>
    public partial class FrmQreMain : Form
    {
        public ApOutputNavigator ResultsActionPanel;

        private string genericNavErrorMsg = @"Action could not be completed due to error: ";

        public FrmQreMain()
        {
            // Force initialization of databases since they're on-demand
            //var Params = QraStateContainer.Instance.Parameters;
            //var defaults = QraStateContainer.Instance.Defaults;
            InitializeComponent();

            GotoAppStartDefaultLocation();
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

        public static FrmQreMain ActiveScreen { get; private set; }


        private FrmInputEditor _inputEditor;

        // Engineering Toolkit form
        private readonly FrmEtk _mEtkForm = new FrmEtk();

        public static void UpdateControls(ContentPanel newPanel)
        {
            ActiveScreen.ContentPanel.Controls.Clear();
            ActiveScreen.ContentPanel.Controls.Add(newPanel);
        }

        private void frmQFEMain_Load(object sender, EventArgs e)
        {
            // Delete old temp files from prev runs
            QuickFunctions.ClearOldTemporaryFiles();

            ActiveScreen = this;

            tcNav.TabPages.Remove(tpNfpa2Mode);
            tcNav.TabPages.Remove(tpTests);
            tcNav.SelectedIndex = 0;
            tcNav.SelectedTab = null;
            tcNav.SelectedTab = tpQraMode;

            // Populate fuel selection dropdowns.
            // One dropdown on phys UI, one on QRA. Both sync to same backend param.
            fuelTypePhys.DataSource = QraStateContainer.Instance.FuelTypes;
            fuelTypePhys.SelectedItem = QraStateContainer.GetValue<FuelType>("FuelType");
            fuelTypeQra.DataSource = QraStateContainer.Instance.FuelTypes;
            fuelTypeQra.SelectedItem = QraStateContainer.GetValue<FuelType>("FuelType");
        }

        /// <summary>
        /// Handle selection of panels to display, including updating buttons.
        /// </summary>
        /// <param name="mainForm"></param>
        /// <param name="sender"></param>
        /// <param name="containerPanel"></param>
        /// <param name="panelToActivate">New panel</param>
        /// <param name="panelNarrativeText">Text pulled from RTF to display as header</param>
        /// <param name="clickOption"></param>
        public static void ActivateActionPanel(FrmQreMain mainForm, Button sender, Panel containerPanel,
            UserControl panelToActivate, string panelNarrativeText,
            ApButtonClickOption clickOption = ApButtonClickOption.PerformClick)
        {
            mainForm.SuspendLayout();
            try
            {
                var panelThatOwnsCaller = (Panel) sender.Parent;
                containerPanel.Controls.Clear();
                if (panelThatOwnsCaller != null) UiStateRoutines.UnselectButtons(panelThatOwnsCaller);

                UiStateRoutines.UnselectButtons(containerPanel);
                sender.ForeColor = Color.Green;

                // Clear unneeded controls
                foreach (Control thisControl in containerPanel.Controls)
                    if (thisControl is Button)
                        containerPanel.Controls.Remove(thisControl);

                panelToActivate.Dock = DockStyle.Fill;
                containerPanel.Controls.Add(panelToActivate);
                ActiveScreen.lblActionNarrative.Text = panelNarrativeText;
                if (clickOption == ApButtonClickOption.PerformClick)
                {
                    var firstButton = QuickFunctions.GetTopButton(panelToActivate, ChildNavOptions.NavigateChildren);
                    if (firstButton != null)
                        firstButton.PerformClick();
                    else
                        MessageBox.Show(@"Cannot perform click for the " + panelToActivate.GetType().Name +
                                        @" action panel because it contains no buttons.");
                }
            }
            finally
            {
                mainForm.ResumeLayout();
            }
        }

        /// <summary>
        /// Disable QRA content panels and phys tab, e.g. during analysis.
        /// </summary>
        public void DisableNavigation()
        {
            pnlActionButtons.Enabled = false;
            scNavigation.Panel1.Enabled = false;
            tcNav.Enabled = false;
        }

        /// <summary>
        /// Enable QRA content panels and phys tab, e.g. after analysis completes
        /// </summary>
        public void EnableNavigation()
        {
            pnlActionButtons.Enabled = true;
            scNavigation.Panel1.Enabled = true;
            tcNav.Enabled = true;
        }

        /// <summary>
        /// Switch between main analysis modes (QRA and phys)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tcNav_SelectedIndexChanged(object sender, EventArgs e)
        {
            var activeTab = tcNav.SelectedTab;
            Panel activePanel = null;
            var topLevelTab = true;

            if (activeTab == tpNfpa2Mode)
            {
                activePanel = pnlNfpaNav;
            }
            else if (activeTab == tpQraMode)
            {
                activePanel = pnlQraNav;
            }

            else if (activeTab == tpPhysics)
            {
                activePanel = pnlPhysicsNav;
            }
            else
            {
                topLevelTab = false;
            }

            if (!topLevelTab) return;

            pnlActionButtons.Controls.Clear();  // Clear nav panel
            var defaultButton = QuickFunctions.GetTopButton(activePanel, ChildNavOptions.NavigateChildren);

            defaultButton.PerformClick();

            if (activeTab == tpNfpa2Mode)
            {
                var actionsNarrative = "";
                ActivateActionPanel(this, btnFire, pnlActionButtons, new ApBlank(), actionsNarrative,
                    ApButtonClickOption.NoClick);
                var blankPanel = new CpBlank();
                ActionUtils.SetContentScreen(btnFire, blankPanel);
                ContentPanels.ContentPanel.SetNarrative(blankPanel, Narratives.PBDMode);
            }
        }

        /// <summary>
        /// Reset to System Description tab under QRA mode
        /// </summary>
        private void GotoAppStartDefaultLocation()
        {
            tcNav.SelectedIndex = 0;
            btnSystemDescription.PerformClick();
        }

        private static UserControl _currentControl;

        private void ChangePanel(string actionsNarrative, object sender, UserControl nextControl)
        {
            try
            {
                // Track displayed control and free up resources when it changes
                if (_currentControl != null) _currentControl.Dispose();
                _currentControl = nextControl;

                UiStateRoutines.UnselectButtons(this);
                ResultsActionPanel = new ApOutputNavigator();
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, ResultsActionPanel, actionsNarrative,
                    ApButtonClickOption.NoClick);
                ActionUtils.SetContentScreen((Button) sender, nextControl);
            }
            catch (Exception ex)
            {
                Trace.TraceError(Environment.StackTrace);
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        /// <summary>
        /// Display QRA scenario input tabs
        /// </summary>
        private void btnScenarios_Click(object sender, EventArgs e)
        {
            ChangePanel("Output", sender, new CpScenarios());
        }

        /// <summary>
        /// Activate QRA data probabilities panel
        /// </summary>
        private void btnQraData_Probabilities_Click(object sender, EventArgs e)
        {
            ChangePanel("Output", sender, new CpDataProbabilities());
        }

        private void btnConsequenceModels_Click(object sender, EventArgs e)
        {
            try
            {
                UiStateRoutines.UnselectButtons(this);
                var actionsNarrative = "Output";
                ResultsActionPanel = new ApOutputNavigator();
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, ResultsActionPanel, actionsNarrative,
                    ApButtonClickOption.NoClick);

                ActionUtils.SetContentScreen((Button) sender, new CpConsequenceModels());
            }
            catch (Exception ex)
            {
                Trace.TraceError(Environment.StackTrace);
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        /// <summary>
        /// Display result panels
        /// </summary>
        private void btnSystemDescription_Click(object sender, EventArgs e)
        {
            try
            {
                UiStateRoutines.UnselectButtons(this);

                var actionsNarrative = "Output";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApOutputNavigator(),
                    actionsNarrative, ApButtonClickOption.NoClick);

                UserControl newControl = new CpSystemDescription();
                ActionUtils.SetContentScreen((Button) sender, newControl);
            }
            catch (Exception ex)
            {
                Trace.TraceError(Environment.StackTrace);
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        private void About_Click(object sender, EventArgs e)
        {
            var aboutClass = new ClsAbout
            {
                DialogCaption = "HyRAM",

                AuthorEmail = "amuna@sandia.gov",
                AuthorName = "Alice Muna",
                CopyrightStatement = GetRequiredCopyrightStatement(),
                Narrative =
                    "Developed for the United States Department of Energy by Sandia National Laboratories. Please contact SNL for attribution details.",
                WebsiteLinkText = "Visit HyRAM.sandia.gov",
                WebsiteUrl = "http://hyram.sandia.gov"
            };

            aboutClass.Show();
        }

        private string GetRequiredCopyrightStatement()
        {
            return @"Copyright 2015-2019 Sandia Corporation.  

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license for use of this work by or on behalf of the U.S. Government. Export of this data may require a license from the United States Government. For five (5) years from 2/16/2016, the United States Government is granted for itself and others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide license in this data to reproduce, prepare derivative works, and perform publicly and display publicly, by or on behalf of the Government. There is provision for the possible extension of the term of this license. Subsequent to that period or any extension granted, the United States Government is granted for itself and others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide license in this data to reproduce, prepare derivative works, distribute copies to the public, perform publicly and display publicly, and to permit others to do so. The specific term of the license can be identified by inquiry made to Sandia Corporation or DOE.
 
NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF ENERGY, NOR SANDIA CORPORATION, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS, OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.
 
Any licensee of " + "\"" + @"HyRAM (Hydrogen Risk Assessment Models) v. 1.0" + "\"" +
                   @" has the obligation and responsibility to abide by the applicable export control laws, regulations, and general prohibitions relating to the export of technical data. Failure to obtain an export control license or other authority from the Government may result in criminal liability under U.S. laws.
";
        }

        private void btnGasPlumeDispersion_Click(object sender, EventArgs e)
        {
            try
            {
                var actionsNarrative = "";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApGasPlumeDispersion(),
                    actionsNarrative, ApButtonClickOption.NoClick);
                ActionUtils.SetContentScreen((Button) sender, new CpGasPlumeDispersion());
            }
            catch (Exception ex)
            {
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        private void btnOverpressure_Click(object sender, EventArgs e)
        {
            try
            {
                var actionsNarrative = "";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApOverpressure(),
                    actionsNarrative, ApButtonClickOption.NoClick);
                ActionUtils.SetContentScreen((Button) sender, new CpOverpressure());
            }
            catch (Exception ex)
            {
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        private void btnJetFlame_Click(object sender, EventArgs e)
        {
            try
            {
                var actionsNarrative = "";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApJetFlame(),
                    actionsNarrative);
                // ActionUtils.SetContentScreen((Button)sender, new cpBlank());
            }
            catch (Exception ex)
            {
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        private void mnuSave_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new FrmLoadSaveFile
                {
                    IsSaveFileForm = true,
                    CurrentLoadedState = QraStateContainer.Instance
                };
                saveDialog.ShowDialog();
                saveDialog.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not save input file. Error: " + ex.Message);
            }
        }

        private void mnuLoad_Click(object sender, EventArgs e)
        {
            try
            {
                var loadDialog = new FrmLoadSaveFile
                {
                    IsSaveFileForm = false
                };
                loadDialog.ShowDialog();
                if (loadDialog.CurrentLoadedState != null) QraStateContainer.Instance = loadDialog.CurrentLoadedState;

                loadDialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not open input file. Error: " + ex.Message);
            }

            QraStateContainer.Instance.UndoStateDamageCausedByLoad();

            GotoAppStartDefaultLocation();
        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void mnuResetDefaults_Click(object sender, EventArgs e)
        {
            var dlgResult = MessageBox.Show(this,
                "Resetting parameters. Restart to revert this action. " +
                "To continue, click \"OK.\" To abort, click \"Cancel.\"",
                "Confirm input reset action", MessageBoxButtons.OKCancel);
            if (dlgResult == DialogResult.OK)
            {
                QraStateContainer.Instance.ResetInputsAndDefaults();
                if (ActionUtils.ActiveContentPanel != null) ActionUtils.ActiveContentPanel.Notify_LoadComplete();
            }
        }


        private void openMasterInputEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_inputEditor == null)
            {
                _inputEditor = new FrmInputEditor();
                _inputEditor.Show();
            }
            else
            {
                _inputEditor.Activate();
            }

            _inputEditor.FormClosed += (sender2, args) => { _inputEditor = null; };
        }


        /// <summary>
        /// Display Engineering Toolkit window
        /// </summary>
        private void mnuETK_Click(object sender, EventArgs e)
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
        private void mnuOpenUserDataDir_Click(object sender, EventArgs e)
        {
            Process.Start(QraStateContainer.UserDataDir);
        }


        /// <summary>
        /// PBD Safety panel, currently unused
        /// </summary>
        private void btnSafety_Click(object sender, EventArgs e)
        {
            try
            {
                var safetyActionsNarrative = "Required Design Scenario-Safety";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApSafety(),
                    safetyActionsNarrative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        /// <summary>
        /// PBD Settings panel, currently unused
        /// </summary>
        private void btnSettings_Click(object sender, EventArgs e)
        {
            try
            {
                var settingsNarrative = "Settings and Defaults";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApSettings(),
                    settingsNarrative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        /// <summary>
        /// PBD Fire panel, currently unused
        /// </summary>
        private void btnFire_Click(object sender, EventArgs e)
        {
            btnHazMat.PerformClick();
        }

        /// <summary>
        /// PBD Explosion panel, currently unused
        /// </summary>
        private void btnExplosions_Click(object sender, EventArgs e)
        {
            try
            {
                var explosionActionsNarrative = "Required Design Scenario-Explosion";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApExplosion(),
                    explosionActionsNarrative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        /// <summary>
        /// PBD hazmat mode, currently unused
        /// </summary>
        private void btnHazMat_Click(object sender, EventArgs e)
        {
            try
            {
                var hazMatActionsNarrative = "Required Design Scenario-Hazardous Materials";
                ActivateActionPanel(this, (Button) sender, pnlActionButtons, new ApHazMat(),
                    hazMatActionsNarrative);
            }
            catch (Exception ex)
            {
                MessageBox.Show(genericNavErrorMsg + ex.Message);
            }
        }

        private void fuelTypePhys_SelectionChangeCommitted(object sender, EventArgs e)
        {
            QraStateContainer.SetValue("FuelType", fuelTypePhys.SelectedItem);
        }

        private void fuelTypeQra_SelectionChangeCommitted(object sender, EventArgs e)
        {
            QraStateContainer.SetValue("FuelType", fuelTypePhys.SelectedItem);
        }
    }
}