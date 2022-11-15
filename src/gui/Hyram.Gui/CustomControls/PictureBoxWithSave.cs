/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class PictureBoxWithSave : PictureBox
    {
        public PictureBoxWithSave()
        {
            InitializeComponent();
        }

        public new void Load(string imageFilename)
        {
            var imageFile = new FileStream(imageFilename, FileMode.Open);
            try
            {
                pbPicture.Image = Image.FromStream(imageFile);
            }
            finally
            {
                imageFile.Close();
            }
        }

        public void Unload()
        {
            if (pbPicture.Image != null)
            {
                var existingFile = pbPicture.Image;
                pbPicture.Image = null;
                existingFile?.Dispose();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string plotSavePath = Settings.Default.plotSavePath;
                var outputFilename = QuickFunctions.SelectSaveAsFilename("save image", ref plotSavePath, "png", "PNG files (*.png)|*.png");

                if (outputFilename.Length > 0)
                {
                    pbPicture.Image.Save(outputFilename);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Action could not be performed due to error: " + ex.Message);
            }
        }
    }
}