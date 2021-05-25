/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
*/

using System;
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

        public MainForm()
        {
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
        }

        private void RefreshTopState()
        {
            physicsFuelTypeSelector.SelectedItem = StateContainer.GetValue<FuelType>("FuelType");
            qraFuelTypeSelector.SelectedItem = StateContainer.GetValue<FuelType>("FuelType");
        }

        private void scenariosFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new ScenariosForm(), Narratives.QraScenariosDescrip);
        }

        private void probabilitiesFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new ProbabilitiesForm(), Narratives.QraProbabilitiesDescrip);
        }

        private void consequenceModelsFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new ConsequenceModelsForm(), Narratives.QraConsequenceModelsDescrip);
        }

        private void systemDescriptionFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new SystemDescriptionForm(), Narratives.QraSystemDescriptionDescrip);
        }
        private void plumeDispersionFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new PlumeForm(), Narratives.PhysPlumeDescrip, new PlumeDispersionPanel());
        }

        private void overpressureFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new IndoorReleaseForm(), Narratives.PhysOverpressureDescrip, new OverpressurePanel());
        }

        private void jetPlotTempFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new JetFlameTemperaturePlotForm(), Narratives.PhysJetFlameDescrip, new OverpressurePanel());
        }

        private void jetHeatAnalysisFormButton_Click(object sender, EventArgs e)
        {
            ChangePanel(sender, new JetFlameHeatAnalysisForm(), Narratives.PhysRadHeatDescrip, new OverpressurePanel());
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
                    panelToActivate = new QraOutputNavPanel();

                mainOutputNavPanel.Controls.Clear();
                mainOutputNavPanel.Controls.Add(panelToActivate);
                panelToActivate.Dock = DockStyle.Fill;

                UiStateRoutines.UnselectButtons(qraNavPanel);
                UiStateRoutines.UnselectButtons(physicsNavPanel);
                //if (modeNavPanel != null) UiStateRoutines.UnselectButtons(modeNavPanel);

#if false
                //UiStateRoutines.UnselectButtons(mainOutputNavPanel);
                //clickedButton.ForeColor = Color.Green;

// remove nav buttons (will add later if needed)
                foreach (Control thisControl in mainOutputNavPanel.Controls)
                    if (thisControl is Button)
                        mainOutputNavPanel.Controls.Remove(thisControl);
#endif


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
                _currentControl?.Dispose();
                _currentControl = nextControl;
                SetContentScreen((Button) sender, nextControl);
            }
            if (nextControl != null && descripFilepath != null)
                ContentPanel.SetNarrative(nextControl, descripFilepath);
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
            }
            else
            {
                activePanel = physicsNavPanel;
            }

            mainOutputNavPanel.Controls.Clear();  // Clear nav panel
            QuickFunctions.GetTopButton(activePanel, ChildNavOptions.NavigateChildren).PerformClick();
        }

        /// <summary>
        /// Reset to System Description tab under QRA mode
        /// </summary>
        private void GotoAppStartDefaultLocation()
        {
            modeTabs.SelectedIndex = 0;
            RefreshTopState();
            systemDescriptionFormButton.PerformClick();
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


        private void ChangeSelectedFuel(FuelType newFuel)
        {
            StateContainer.SetValue("FuelType", newFuel);
            StateContainer.Instance.RefreshLeakFrequencyData(newFuel);
            GotoAppStartDefaultLocation();
        }


        private void physicsFuelTypeSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ChangeSelectedFuel((FuelType)physicsFuelTypeSelector.SelectedItem);
        }

        private void qraFuelTypeSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ChangeSelectedFuel((FuelType)qraFuelTypeSelector.SelectedItem);
        }
    }
}