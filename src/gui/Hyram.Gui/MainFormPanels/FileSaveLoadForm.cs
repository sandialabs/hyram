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
using System.Runtime.Serialization;
using System.Windows.Forms;


namespace SandiaNationalLaboratories.Hyram
{
    public partial class FileSaveLoadForm : Form
    {
        private string _filename = "";

        private StateContainer _mCurrentLoadedState;

        private bool _mIsSaveFileForm;

        public FileSaveLoadForm()
        {
            InitializeComponent();
        }

        public StateContainer CurrentLoadedState
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
                    CurrentLoadedState = StateContainer.Load(_filename);
                    StateContainer.Instance = CurrentLoadedState;
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
            string workspaceSavePath = Hyram.Settings.Default.workspaceSavePath;
            if (_mIsSaveFileForm)
            {
                _filename = QuickFunctions.SelectSaveAsFilename("Save workspace.", ref workspaceSavePath, 
                    "HyRAM", "HyRAM StateContainer Files|*.HyRAM|All files (*.*)|*.*");
            }
            else
            {
                _filename = QuickFunctions.SelectFilename("Load workspace", ref workspaceSavePath,
                    "HyRAM StateContainer Files|*.HyRAM|All files (*.*)|*.*");
            }

            tbFile.Text = _filename;
        }

        private void tmrCheckFile_Tick(object sender, EventArgs e)
        {
            if (tbFile.Text.Length > 0)
            {
                try
                {
                    CurrentLoadedState = StateContainer.Deserialize(tbFile.Text);
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