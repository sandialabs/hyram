/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class QraOutputNavPanel : UserControl
    {
        private CancellationTokenSource _analysisToken;
        private Action _callback; // which function to call when task (analysis) is complete
        private string _msg;
        private int _progress;

        private ProgressDisplay _progressCp;

        public QraOutputNavPanel()
        {
            InitializeComponent();
        }

        private void qraResultsButton_Click(object sender, EventArgs e)
        {
            // For now, to be overly conservative, always re-run analysis
            //if (StateContainer.ResultsReady())
            _callback = ActivateScenarioStatsCp;
            PrepForAnalysis((Button) sender);
        }


        /// <summary>
        ///     Set up progress screen and execute analysis task.
        ///     Callback will be called once analysis is complete.
        /// </summary>
        /// <param name="sender"></param>
        private void PrepForAnalysis(Button sender)
        {
            try
            {
                UiStateRoutines.UnselectButtons(FindForm());
                _progressCp = new ProgressDisplay();
                MainForm.SetContentScreen(sender, _progressCp);

                MainForm.ActiveScreen.DisableNavigation(); // ugly way to ensure user can't navigate away
                _analysisToken = new CancellationTokenSource();
                //var StartTime = DateTime.Now;
                var task = Task.Factory.StartNew(ConductAnalysis, _analysisToken.Token);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
                MessageBox.Show("Action failed with this error: " + ex.Message);
            }
        }

        private void TaskHelperUpdate(int prog, string msg)
        {
            _progress = prog;
            _msg = msg;
            if (_progressCp.InvokeRequired)
            {
                var myDelegate = new Delegate(UpdateProgress);
                _progressCp.Invoke(myDelegate);
            }
            else
            {
                UpdateProgress();
            }
        }

        /// <summary>
        ///     Execute QRA analysis while updating progress bar via delegate.
        ///     Assume this runs in separate thread. When complete, will trigger callback.
        /// </summary>
        private void ConductAnalysis()
        {
            TaskHelperUpdate(10, "Gathering parameters...");
            var qra = new QraInterface();
            TaskHelperUpdate(30, "Conducting analysis... this may take several minutes");

            try
            {
                qra.Execute();
            }
            catch (Exception ex)
            {
                // If execution fails, display error on progress bar and re-enable navigation
                TaskHelperUpdate(-1, ex.Message);
                if (MainForm.ActiveScreen.InvokeRequired)
                {
                    var myDelegate = new Delegate(MainForm.ActiveScreen.EnableNavigation);
                    Invoke(myDelegate);
                }
                else
                {
                    MainForm.ActiveScreen.EnableNavigation();
                }

                return;
            }

            TaskHelperUpdate(100, "Analysis complete");
            Thread.Sleep(2000);

            // All done so trigger callback to load actual results panel.
            if (InvokeRequired)
            {
                var myDelegate = new Delegate(_callback);
                Invoke(myDelegate);
            }
        }

        private void UpdateProgress()
        {
            _progressCp.UpdateProgress(_progress, _msg);
        }

        private void ActivateScenarioStatsCp()
        {
            MainForm.ActiveScreen.EnableNavigation();
            MainForm.SetContentScreen(qraResultsButton, new QraResultsPanel());
            if (_progressCp != null) _progressCp.Dispose();
        }

        private delegate void Delegate();
    }
}