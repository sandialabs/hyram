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
using System.Runtime.Serialization;
using System.Windows.Forms;
using QRAState;
using UIHelpers;

namespace QRA_Frontend
{
    public partial class FrmLoadSaveFile : Form
    {
        private string _filename = "";

        private QraStateContainer _mCurrentLoadedState;

        private bool _mIsSaveFileForm;

        public FrmLoadSaveFile()
        {
            InitializeComponent();
        }

        public QraStateContainer CurrentLoadedState
        {
            get => _mCurrentLoadedState;
            set
            {
                _mCurrentLoadedState = value;
                if (_mCurrentLoadedState != null)
                    rtbComments.Text = _mCurrentLoadedState.Comments;
                else
                    rtbComments.Text = "";
            }
        }

        public bool IsSaveFileForm
        {
            get => _mIsSaveFileForm;
            set
            {
                _mIsSaveFileForm = value;
                if (_mIsSaveFileForm)
                {
                    Text = "Save Workspace...";
                    btnOK.Text = "Save";
                    rtbComments.Enabled = true;
                    tmrCheckFile.Enabled = false;
                }
                else
                {
                    Text = "Load Workspace...";
                    btnOK.Text = "Load";
                    rtbComments.Enabled = false;
                    tmrCheckFile.Enabled = true;
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _mCurrentLoadedState = null;
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_filename.Length == 0)
            {
                if (_mIsSaveFileForm)
                    MessageBox.Show("You must select a save location.");
                else
                    MessageBox.Show("You must select a file to load.");
            }
            else if (_mIsSaveFileForm)
            {
                try
                {
                    _mCurrentLoadedState.Comments = rtbComments.Text;
                    _mCurrentLoadedState.Save(_filename);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The workspace could not be saved due to the following error: " + ex.Message);
                }
            }
            else if (_mCurrentLoadedState == null)
            {
                try
                {
                    CurrentLoadedState = QraStateContainer.Load(_filename);
                    QraStateContainer.Instance = CurrentLoadedState;
                    Close();
                }
                catch (SerializationException)
                {
                    MessageBox.Show(
                        "The workspace could not be loaded. The file selected is not a valid HyRAM workspace file.");
                    CurrentLoadedState = null;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("The workspace could not be loaded due to the following error: " + ex.Message);
                    CurrentLoadedState = null;
                }
            }

            else
            {
                Close();
            }
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string workspaceSavePath = Properties.Settings.Default.workspaceSavePath;
            if (_mIsSaveFileForm)
            {
                _filename = QuickFunctions.SelectSaveAsFilename("Save workspace.", ref workspaceSavePath, 
                    "HyRAM", "HyRAM State Files|*.HyRAM|All files (*.*)|*.*");
            }
            else
            {
                _filename = QuickFunctions.SelectFilename("Load workspace", ref workspaceSavePath,
                    "HyRAM State Files|*.HyRAM|All files (*.*)|*.*");
            }

            tbFile.Text = _filename;
        }

        private void tmrCheckFile_Tick(object sender, EventArgs e)
        {
            if (tbFile.Text.Length > 0)
            {
                try
                {
                    CurrentLoadedState = QraStateContainer.Deserialize(tbFile.Text);
                    _filename = tbFile.Text;
                }
                catch (Exception)
                {
                    lblWarning.Visible = true;
                    CurrentLoadedState = null;
                }
            }
            else
            {
                lblWarning.Visible = false;
                CurrentLoadedState = null;
            }
        }
    }
}