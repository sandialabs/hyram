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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using PyAPI;
using QRA_Frontend.ContentPanels;
using QRAState;

namespace QRA_Frontend.ActionPanels
{
    public partial class ApOutputNavigator : UserControl
    {
        private CancellationTokenSource _analysisToken;
        private Action _callback; // which function to call when task (analysis) is complete
        private string _msg;
        private int _progress;

        private ProgressDisplay _progressCp;

        public ApOutputNavigator()
        {
            InitializeComponent();
        }

        private void btnScenarioStats_Click(object sender, EventArgs e)
        {
            // For now, to be overly conservative, always re-run analysis
            //if (QraStateContainer.ResultsReady())
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
                ActionUtils.SetContentScreen(sender, _progressCp);

                FrmQreMain.ActiveScreen.DisableNavigation(); // ugly way to ensure user can't navigate away
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
            var qra = new PyQrAnalysis();
            TaskHelperUpdate(30, "Conducting analysis... (this may take several minutes)");

            try
            {
                qra.Execute();
            }
            catch (Exception ex)
            {
                // If execution fails, display error on progress bar and re-enable navigation
                var msg = "Error during analysis: " + ex.Message;
                TaskHelperUpdate(-1, msg);
                if (FrmQreMain.ActiveScreen.InvokeRequired)
                {
                    var myDelegate = new Delegate(FrmQreMain.ActiveScreen.EnableNavigation);
                    Invoke(myDelegate);
                }
                else
                {
                    FrmQreMain.ActiveScreen.EnableNavigation();
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
            FrmQreMain.ActiveScreen.EnableNavigation();
            ActionUtils.SetContentScreen(btnScenarioStats, new CpScenarioStats());
            if (_progressCp != null) _progressCp.Dispose();
        }

        private delegate void Delegate();
    }
}