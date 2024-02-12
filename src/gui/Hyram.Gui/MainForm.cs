/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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

        private CancellationTokenSource _token;
        private string _progressMessage;
        private int _progressStatus;
        private ProgressDisplay _progressDisplay;
        private delegate void Delegate();
        private static FormMode _mode = FormMode.Qra;


        public MainForm()
        {
            InitializeComponent();

            State.Data.InitializeState();
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

            ExitMenuItem.Click += (o, args) => Close();

            FuelFormBtn.Click += (o, args) => ChangeForm(o, new SharedStateForm(), Narratives.FuelFormDescrip);

            ScenariosFormBtn.Click += (o, args) => ChangeForm(o, new ScenariosForm(), Narratives.QraScenariosDescrip);
            ProbabilitiesFormBtn.Click += (o, args) => ChangeForm(o, new ProbabilitiesForm(this), Narratives.QraProbabilitiesDescrip);
            ConsequenceFormBtn.Click += (o, args) => ChangeForm(o, new ConsequenceModelsForm(this), Narratives.QraConsequenceModelsDescrip);

            SystemDescripFormBtn.Click += (o, args) => ChangeForm(o, new SystemDescriptionForm(), 
                                                                  Narratives.QraSystemDescriptionDescrip);

            PlumeFormBtn.Click += (o, args) => ChangeForm(o, new PlumeForm(), Narratives.PhysPlumeDescrip);
            AccumulationFormBtn.Click += (o, args) => ChangeForm(o, new AccumulationForm(), Narratives.PhysAccumulationDescrip);
            JetPlotFormBtn.Click += (o, args) => ChangeForm(o, new JetFlameTemperaturePlotForm(), Narratives.PhysJetFlameDescrip);
            JetFluxFormBtn.Click += (o, args) => ChangeForm(o, new JetFlameHeatAnalysisForm(), Narratives.PhysRadHeatDescrip);
            OverpressureFormBtn.Click += (o, args) => ChangeForm(o, new UnconfinedOverpressureForm(), Narratives.PhysUnconfinedOverpressureDescrip);
            PoolingFormBtn.Click += (o, args) => ChangeForm(o, new PoolingForm(), Narratives.PhysPoolingDescrip);

            TpdFormBtn.Click += (o, args) => ChangeForm(o, new TpdForm(), Narratives.PhysTpdDescrip);
            TankMassFormBtn.Click += (o, args) => ChangeForm(o, new TankMassForm(), Narratives.PhysTankMassDescrip);
            MassFlowFormBtn.Click += (o, args) => ChangeForm(o, new MassFlowForm(), Narratives.PhysMassFlowDescrip);
            TntFormBtn.Click += (o, args) => ChangeForm(o, new TntForm(), Narratives.PhysTntDescrip);

            DataDirectoryMenuItem.Click += (o, args) => Process.Start(StateContainer.UserDataDir);

            RefreshState();

            FuelFormBtn.PerformClick();
            TheSplashscreen.FadeOut();
        }


        private void RefreshState()
        {
            _state = State.Data;
            _state.FuelTypeChangedEvent -= HandleKeyParameterChanged;
            _state.FuelPhaseEvent -= HandleKeyParameterChanged;
            _state.FluidPressure.ParameterChangedEvent -= HandleKeyParameterChanged;

            _state.FuelTypeChangedEvent += HandleKeyParameterChanged;
            _state.FuelPhaseEvent += HandleKeyParameterChanged;
            _state.FluidPressure.ParameterChangedEvent += HandleKeyParameterChanged;
        }

        private void HandleKeyParameterChanged(object sender, EventArgs args)
        {
            RefreshNotifications();
        }


        /// <summary>
        /// Update display of notifications and alerts.
        /// Will prioritize top-level messages and hoist child-level messages if current form has none but siblings do.
        /// </summary>
        private void RefreshNotifications()
        {
            fuelLabel.Text = _state.GetSpeciesOptionString();

            AlertLevel level = AlertLevel.AlertNull;
            string message = "";

            QraSubmitBtn.Enabled = true;

            // if liquid, validate fuel pressure
            if (!_state.ReleasePressureIsValid())
            {
                message = Notifications.ReleasePressureInvalid();
                level = AlertLevel.AlertError;
            }

            if (_mode == FormMode.Qra)
            {
                if (_state.GetActiveFuel() == _state.FuelBlend)
                {
                    level = AlertLevel.AlertWarning;
                    message = Notifications.QraBlendWarning;
                }
                // warn about other pure fluids from QRA analysis
                else if (_state.GetActiveFuel() != _state.FuelH2 &&
                        _state.GetActiveFuel() != _state.FuelMethane &&
                        _state.GetActiveFuel() != _state.FuelPropane)
                {
                    level = AlertLevel.AlertWarning;
                    message = Notifications.QraNonDefaultPureFluidWarning;
                }
            }

            mainMessage.BackColor = _state.AlertBackColors[(int)level];
            mainMessage.ForeColor = _state.AlertTextColors[(int)level];
            mainMessage.Text = message;
//            mainMessage.Visible = _mode == FormMode.Qra && level != AlertLevel.AlertNull;
            mainMessage.Visible = level != AlertLevel.AlertNull;
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
            RefreshNotifications();
        }

        /// <summary>
        /// Loads form in right UI section
        /// </summary>
        public void ChangeForm(object caller, UserControl form, string rtfFilepath = null)
        {
            UiHelpers.UnselectButtons(qraNavPanel);
            UiHelpers.UnselectButtons(physicsNavPanel);
            UiHelpers.UnselectButton(QraSubmitBtn);
            UiHelpers.UnselectButton(FuelFormBtn);

            _currentControl = form;
            _rightFormPanel.ChildControl = form;
            _rightFormPanel.UpdateNarrative(rtfFilepath);

            Controls.Remove(_currentControl);

            UiHelpers.ShowButtonActive((Button)caller);
            RefreshNotifications();
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
            RefreshNotifications();
        }

        /// <summary>
        /// Reset to System Description tab under QRA mode
        /// </summary>
        private void GotoAppStartDefaultLocation()
        {
            ModeTabs.SelectedIndex = 0;

            FuelFormBtn.PerformClick();
            RefreshNotifications();
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
                    RefreshState();
                    RefreshNotifications();
                    GotoAppStartDefaultLocation();
                }

                loadDialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Could not open input file. Error: " + ex.Message);
            }

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