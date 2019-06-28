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

using System.Drawing;

namespace UIHelpers
{
    public class ClsAbout
    {
        private FrmMscAbout _mAboutForm;

        public ClsAbout()
        {
            _mAboutForm = new FrmMscAbout();
        }

        public int Width
        {
            get => _mAboutForm.Width;
            set => _mAboutForm.Width = value;
        }

        public int Height
        {
            get => _mAboutForm.Height;
            set => _mAboutForm.Height = value;
        }

        public string WebsiteLinkText
        {
            get => _mAboutForm.WebsiteLinkText;
            set => _mAboutForm.WebsiteLinkText = value;
        }

        public string WebsiteUrl
        {
            get => _mAboutForm.WebsiteUrl;
            set => _mAboutForm.WebsiteUrl = value;
        }

        public string AuthorEmail
        {
            get => _mAboutForm.AuthorEmail;
            set => _mAboutForm.AuthorEmail = value;
        }

        public string AuthorName
        {
            get => _mAboutForm.AuthorName;
            set => _mAboutForm.AuthorName = value;
        }


        public string CopyrightStatement
        {
            get => _mAboutForm.CopyrightStatement;
            set => _mAboutForm.CopyrightStatement = value;
        }

        public string Narrative
        {
            get => _mAboutForm.Narrative;
            set => _mAboutForm.Narrative = value;
        }

        public string DialogCaption { get; set; } = null;

        public Image Background
        {
            get => _mAboutForm.BackgroundImage;
            set => _mAboutForm.BackgroundImage = value;
        }

        ~ClsAbout()
        {
            _mAboutForm.Dispose();
            _mAboutForm = null;
            //System.GC.Collect();
        }

        public void Show()
        {
            if (DialogCaption != null) _mAboutForm.Text = DialogCaption;

            _mAboutForm.ShowDialog();
        }
    }
}