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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SandiaNationalLaboratories.Hyram.Resources;


namespace SandiaNationalLaboratories.Hyram
{
    enum FormMode
    {
        Qra,
        Physics
    }

    /// <summary>
    /// Primary container form and UI
    /// </summary>
    public partial class MainForm : Form
    {
        private StateContainer _state = State.Data;
        private static UserControl _currentControl;
        public static MainForm TheMainForm { get; private set; }
        private ContentPanel _rightFormPanel;
        private FuelForm _fuelForm;

        private CancellationTokenSource _token;
        private string _progressMessage;
        private int _progressStatus;
        private ProgressDisplay _progressDisplay;
        private delegate void Delegate();

        private bool _ignoreStateChange;

        private static SystemDescriptionForm _systemDescriptionForm;
        private static ProbabilitiesForm _probabilitiesForm;
        private static ScenariosForm _scenariosForm;
        private static ConsequenceModelsForm _consequencesForm;
        private static List<AnalysisForm> _qraForms = new List<AnalysisForm>();
        private static FormMode _mode = FormMode.Qra;


        public MainForm()
        {
            InitializeComponent();

            State.Data.InitializeState();
            RefreshState();

            _ignoreStateChange = true;
            _scenariosForm = new ScenariosForm(this);
            _probabilitiesForm = new ProbabilitiesForm(this);
            _consequencesForm = new ConsequenceModelsForm(this);
            _systemDescriptionForm = new SystemDescriptionForm(this);
            _qraForms = new List<AnalysisForm> { _scenariosForm, _probabilitiesForm, _consequencesForm, _systemDescriptionForm };
            _ignoreStateChange = false;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            TheMainForm = this;
            ModeTabs.SelectedIndex = 0;
            ModeTabs.SelectedTab = null;
            ModeTabs.SelectedTab = qraModeTab;

            _rightFormPanel = new ContentPanel { Dock = DockStyle.Fill };
            TheMainForm.formArea.Controls.Clear();
            TheMainForm.formArea.Controls.Add(_rightFormPanel);

            PhysicsFuelBtn.Click += (o, args) => DisplayFuelForm();
            QraFuelBtn.Click += (o, args) => DisplayFuelForm();
            ExitMenuItem.Click += (o, args) => Close();

            ScenariosFormBtn.Click += (o, args) => ChangeForm(o, _scenariosForm, Narratives.QraScenariosDescrip);
            ProbabilitiesFormBtn.Click += (o, args) => ChangeForm(o, _probabilitiesForm, Narratives.QraProbabilitiesDescrip);
            ConsequenceFormBtn.Click += (o, args) => ChangeForm(o, _consequencesForm, Narratives.QraConsequenceModelsDescrip);
            SystemDescripFormBtn.Click += (o, args) => ChangeForm(o, _systemDescriptionForm, Narratives.QraSystemDescriptionDescrip);

            PlumeFormBtn.Click += (o, args) => ChangeForm(o, new PlumeForm(), Narratives.PhysPlumeDescrip);
            AccumulationFormBtn.Click += (o, args) => ChangeForm(o, new AccumulationForm(), Narratives.PhysAccumulationDescrip);
            JetPlotFormBtn.Click += (o, args) => ChangeForm(o, new JetFlameTemperaturePlotForm(), Narratives.PhysJetFlameDescrip);
            JetFluxFormBtn.Click += (o, args) => ChangeForm(o, new JetFlameHeatAnalysisForm(), Narratives.PhysRadHeatDescrip);
            OverpressureFormBtn.Click += (o, args) => ChangeForm(o, new UnconfinedOverpressureForm(), Narratives.PhysUnconfinedOverpressureDescrip);

            TpdFormBtn.Click += (o, args) => ChangeForm(o, new TpdForm(), Narratives.PhysTpdDescrip);
            TankMassFormBtn.Click += (o, args) => ChangeForm(o, new TankMassForm(), Narratives.PhysTankMassDescrip);
            MassFlowFormBtn.Click += (o, args) => ChangeForm(o, new MassFlowForm(), Narratives.PhysMassFlowDescrip);
            TntFormBtn.Click += (o, args) => ChangeForm(o, new TntForm(), Narratives.PhysTntDescrip);

            DataDirectoryMenuItem.Click += (o, args) => Process.Start(StateContainer.UserDataDir);

            FuelType.FuelChangedEvent += (o, args) => RefreshState();

            TheSplashscreen.FadeOut();
            SystemDescripFormBtn.PerformClick();
        }


        private void RefreshState()
        {
            _state = State.Data;

            if (!_ignoreStateChange)
            {
                FuelType fuel = _state.GetActiveFuel();
                List<FuelType> displayFuels = new List<FuelType> { _state.FuelH2, _state.FuelMethane, _state.FuelPropane, _state.FuelBlend };
                QraFuelTypeSelector.DataSource = null;
                QraFuelTypeSelector.DataSource = displayFuels;
                QraFuelTypeSelector.SelectedItem = fuel;
                PhysicsFuelTypeSelector.DataSource = null;
                PhysicsFuelTypeSelector.DataSource = displayFuels;
                PhysicsFuelTypeSelector.SelectedItem = fuel;
            }
            RefreshQraForms();
            RefreshAlerts();
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


        /// <summary>
        /// Child forms call this to let MainForm know of a large change, i.e. to update alert messages.
        /// </summary>
        public void NotifyOfChildPublicStateChange()
        {
            if (!_ignoreStateChange)
            {
                RefreshAlerts();
            }
        }

        /// <summary>
        /// Loads form in right UI section
        /// </summary>
        public void ChangeForm(object caller, UserControl form, string rtfFilepath = null)
        {
            UiHelpers.UnselectButtons(qraNavPanel);
            UiHelpers.UnselectButtons(physicsNavPanel);
            UiHelpers.UnselectButton(QraSubmitBtn);

            _currentControl = form;
            _rightFormPanel.ChildControl = form;
            _rightFormPanel.UpdateNarrative(rtfFilepath);

            // refresh persisted QRA forms
            if (_mode == FormMode.Qra && form is AnalysisForm)
            {
                ((AnalysisForm)form).RefreshForm();
            }

            UiHelpers.ShowButtonActive((Button)caller);
            RefreshAlerts();
        }

        /// <summary>
        /// Toggle usability of navigation buttons and tabs
        /// </summary>
        public void ToggleNavigability(bool enabled)
        {
            ModeTabs.Enabled = enabled;
        }
        // container function for delegate
        public void ToggleNavigability() { ToggleNavigability(true);}


        /// <summary>
        /// Update display of notifications and alerts.
        /// Will prioritize top-level messages and hoist child-level messages if current form has none but siblings do.
        /// </summary>
        private void RefreshAlerts()
        {
            AlertLevel alert = AlertLevel.AlertNull;
            string alertMessage = "";

            // top-level alerts
            if (_mode == FormMode.Qra && _state.GetActiveFuel() == _state.FuelBlend)
            {
                alert = AlertLevel.AlertError;
                alertMessage = "QRA cannot assess blends at this time.";
                QraSubmitBtn.Enabled = false;
            }
            else
            {
                QraSubmitBtn.Enabled = true;
                // display sibling child alert from QRA form if no top alert
                foreach (AnalysisForm form in _qraForms)
                {
                    if (form.Alert != AlertLevel.AlertNull && form != _currentControl)
                    {
                        alertMessage = form.AlertMessage;
                        alert = form.Alert;
                        break;
                    }
                }
            }

            mainMessage.BackColor = _state.AlertBackColors[(int)alert];
            mainMessage.ForeColor = _state.AlertTextColors[(int)alert];
            mainMessage.Text = alertMessage;
            mainMessage.Visible = _mode == FormMode.Qra && alert != AlertLevel.AlertNull;
        }

        /// <summary>
        /// Switch between main analysis modes (QRA and phys)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ModeTabs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ModeTabs.SelectedTab == qraModeTab || ModeTabs.SelectedTab == null)
            {
                _mode = FormMode.Qra;
                SystemDescripFormBtn.PerformClick();
            }
            else
            {
                _mode = FormMode.Physics;
                PlumeFormBtn.PerformClick();
            }
            RefreshAlerts();
        }

        private void RefreshQraForms()
        {
            foreach (AnalysisForm form in _qraForms)
            {
                form.RefreshForm();
            }
        }

        /// <summary>
        /// Reset to System Description tab under QRA mode
        /// </summary>
        private void GotoAppStartDefaultLocation()
        {
            ModeTabs.SelectedIndex = 0;
            RefreshState();

            SystemDescripFormBtn.PerformClick();
            RefreshQraForms();
            RefreshAlerts();
        }


        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new AboutForm();
            aboutForm.Show();
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new FileSaveLoadForm
                {
                    IsSaveFileForm = true,
                    CurrentLoadedState = State.Data,
                };
                saveDialog.ShowDialog();
                saveDialog.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not save input file. Error: " + ex.Message);
            }
        }

        private void LoadMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                var loadDialog = new FileSaveLoadForm
                {
                    IsSaveFileForm = false
                };
                loadDialog.ShowDialog();
                if (loadDialog.CurrentLoadedState != null)
                {
                    State.Data = loadDialog.CurrentLoadedState;
                }

                loadDialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not open input file. Error: " + ex.Message);
            }

            RefreshState();
            GotoAppStartDefaultLocation();
        }

        private void PhysicsFuelTypeSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _ignoreStateChange = true;

            FuelType fuel = (FuelType)PhysicsFuelTypeSelector.SelectedItem;
            _state.SetFuel(fuel);

            _ignoreStateChange = false;

            if (fuel == _state.FuelBlend)
            {
                PhysicsFuelBtn.PerformClick();
            }
        }

        private void QraFuelTypeSelector_SelectionChangeCommitted(object sender, EventArgs e)
        {
            _ignoreStateChange = true;

            FuelType fuel = (FuelType)QraFuelTypeSelector.SelectedItem;
            _state.SetFuel(fuel);

            _ignoreStateChange = false;
            RefreshAlerts();

            if (fuel == _state.FuelBlend)
            {
                QraFuelBtn.PerformClick();
            }
        }

        private void DisplayFuelForm()
        {
            _fuelForm = new FuelForm();
            _fuelForm.ShowDialog();
        }


        // /////////////////////
        // QRA ANALYSIS

        private void QraSubmitBtn_Click(object sender, EventArgs e)
        {
            PrepareQraAnalysisTask((Button) sender);
        }

        private void PrepareQraAnalysisTask(Button sender)
        {
            try
            {
                UiHelpers.UnselectButtons(FindForm());
                _progressDisplay = new ProgressDisplay();
                ChangeForm(sender, _progressDisplay);

                TheMainForm.ToggleNavigability(false);
                _token = new CancellationTokenSource();
                var task = Task.Factory.StartNew(ConductAnalysis, _token.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show("Action failed: " + ex.Message);
            }
        }

        // Async updates progress for QRA analysis.
        private void AnalysisTaskUpdate(int prog, string msg)
        {
            _progressStatus = prog;
            _progressMessage = msg;
            if (_progressDisplay.InvokeRequired)
            {
                var myDelegate = new Delegate(UpdateProgress);
                _progressDisplay.Invoke(myDelegate);
            }
            else
            {
                UpdateProgress();
            }
        }

        // Container function for delegate to update progress display
        private void UpdateProgress()
        {
            _progressDisplay.UpdateProgress(_progressStatus, _progressMessage);
        }

        /// <summary>
        ///     Execute QRA analysis while updating progress bar via delegate.
        ///     Assume this runs in separate thread. When complete, will trigger callback.
        /// </summary>
        private void ConductAnalysis()
        {
            AnalysisTaskUpdate(10, "Gathering parameters...");
            var qra = new QraInterface();
            AnalysisTaskUpdate(30, "Conducting analysis... this may take several minutes.");

            try
            {
                qra.Execute();
            }
            catch (Exception ex)
            {
                // If execution fails, display error on progress bar and re-enable navigation
                AnalysisTaskUpdate(-1, ex.Message);
                if (TheMainForm.InvokeRequired)
                {
                    Invoke(new Action(ToggleNavigability));
                }
                else
                {
                    ToggleNavigability();
                }

                return;
            }

            AnalysisTaskUpdate(100, "Analysis complete");
            Thread.Sleep(2000);

            // trigger callback to load results panel.
            if (InvokeRequired)
            {
                var myDelegate = new Delegate(ShowQraResults);
                Invoke(myDelegate);
            }
        }

        private void ShowQraResults()
        {
            TheMainForm.ToggleNavigability(true);
            ChangeForm(QraSubmitBtn, new QraResultsPanel(), Narratives.QraOutputDescrip);
            _progressDisplay?.Dispose();
        }
    }
}