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

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string plotSavePath = Settings.Default.plotSavePath;
                //var outputFilename = QuickFunctions.SelectSaveAsFilename("Save image", ref plotSavePath,
                    //"tiff", "Tiff Files(*.tiff)|*.tiff");
                    var outputFilename = QuickFunctions.SelectSaveAsFilename("save image", ref plotSavePath, "png", "PNG files (*.png)|*.png");

                if (outputFilename.Length > 0) pbPicture.Image.Save(outputFilename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Action could not be performed due to error: " + ex.Message);
            }
        }
    }
}