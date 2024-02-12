/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class UiHelpers
    {
        public static void UnselectButtons(Control parentControl)
        {
            foreach (Control thisControl in parentControl.Controls)
            {
                if (thisControl.HasChildren)
                {
                    UnselectButtons(thisControl);
                }

                UnselectButton(thisControl);
            }
        }

        public static void UnselectButton(Button button)
        {
            button.ForeColor = Color.Black;
        }

        public static void UnselectButton(Control control)
        {
            if (control is Button)
            {
                UnselectButton((Button) control);
            }
        }

        public static void ShowButtonActive(Button button)
        {
            button.ForeColor = Color.Green;
        }


        /// <summary>
        /// Opens save dialog after verifying that context menu owner is PictureBox.
        /// </summary>
        /// <param name="sender">ToolStripMenuItem Save As button</param>
        /// <param name="e"></param>
        public static void SaveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem itm && itm.Owner is ContextMenuStrip strip)
            {
                var pb = (PictureBox)strip.SourceControl;
                if (pb.Image == null) return;
                ShowSaveDialog(pb);
            }
        }

        /// <summary>
        /// Displays save dialog for associated PictureBox.
        /// </summary>
        /// <param name="pb">PictureBox that owns associated context menu and Save As button.</param>
        public static void ShowSaveDialog(PictureBox pb)
        {
            var sfd = new SaveFileDialog {Filter = "Image|*.png"};
            if (sfd.ShowDialog() != DialogResult.OK) return;
            if (pb == null || sfd.FileName == null) return;
            SaveImage(pb.Image, sfd.FileName);
        }

        /// <summary>
        /// Saves image to file.
        /// </summary>
        /// <param name="im"></param>
        /// <param name="destPath"></param>
        private static void SaveImage(Image im, string destPath)
        {
            im.Save(destPath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}