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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace UIHelpers
{
    /// <summary>
    ///     Summary description for frmMSCAbout.
    /// </summary>
    public class FrmMscAbout : Form
    {
        /// <summary>
        ///     Required designer variable.
        /// </summary>
        private readonly Container _components = null;

        private Button _btnOk;

        private Label _lblBuiltOn;
        private Label _lblNarrative;
        private LinkLabel _lblSendAuthorEmail;
        private Label _lblVersion;
        private LinkLabel _llVisitWebsite;


        private string _mAuthorEmail = "";

        private string _mWebsiteUrl;
        private PictureBox _pbLogo;
        private TextBox _tbCopyright;

        public FrmMscAbout()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            Text = "About " + Application.ProductName;
            _lblVersion.Text = "Version " + CreateVersionString();

            var buildDateTime = File.GetLastWriteTime(Application.ExecutablePath);
            _lblBuiltOn.Text = "Built on " + buildDateTime.ToLongDateString() + " at " +
                               buildDateTime.ToLongTimeString();
        }

        public string Narrative
        {
            get => _lblNarrative.Text;
            set => _lblNarrative.Text = value;
        }

        public string CopyrightStatement
        {
            get => _tbCopyright.Text;
            set => _tbCopyright.Text = value;
        }

        public string AuthorEmail
        {
            get => _mAuthorEmail;
            set
            {
                _mAuthorEmail = value;
                var showLabel = false;
                if (_mAuthorEmail != null)
                    if (_mAuthorEmail.Length > 0)
                        showLabel = true;

                _lblSendAuthorEmail.Visible = showLabel;
            }
        }

        public string AuthorName { get; set; } = "";

        public string WebsiteLinkText
        {
            get => _llVisitWebsite.Text;
            set => _llVisitWebsite.Text = value;
        }

        public string WebsiteUrl
        {
            get => _mWebsiteUrl;
            set
            {
                _mWebsiteUrl = value;
                if (_mWebsiteUrl == null)
                    _llVisitWebsite.Visible = false;
                else
                    _llVisitWebsite.Visible = true;
            }
        }

        private string CreateVersionString()
        {
            var components = Application.ProductVersion.Split('.');
            string result = null;

            for (var index = 0; index < components.Length; index++)
                if (index == components.Length - 1)
                {
                    //result += ", build " + components[index];
                }
                else
                {
                    if (result == null)
                        result = components[index];
                    else
                        result += "." + components[index];
                }

            return result;
        }

        /// <summary>
        ///     Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                if (_components != null)
                    _components.Dispose();

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._lblVersion = new System.Windows.Forms.Label();
            this._lblNarrative = new System.Windows.Forms.Label();
            this._btnOk = new System.Windows.Forms.Button();
            this._lblSendAuthorEmail = new System.Windows.Forms.LinkLabel();
            this._lblBuiltOn = new System.Windows.Forms.Label();
            this._llVisitWebsite = new System.Windows.Forms.LinkLabel();
            this._pbLogo = new System.Windows.Forms.PictureBox();
            this._tbCopyright = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this._pbLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // _lblVersion
            // 
            this._lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblVersion.Location = new System.Drawing.Point(15, 93);
            this._lblVersion.Name = "_lblVersion";
            this._lblVersion.Size = new System.Drawing.Size(372, 26);
            this._lblVersion.TabIndex = 1;
            this._lblVersion.Text = "Version XX.XXX.XXX";
            this._lblVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _lblNarrative
            // 
            this._lblNarrative.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblNarrative.Location = new System.Drawing.Point(15, 129);
            this._lblNarrative.Name = "_lblNarrative";
            this._lblNarrative.Size = new System.Drawing.Size(372, 41);
            this._lblNarrative.TabIndex = 2;
            this._lblNarrative.Text = "Developed by Sandia National Laboratories";
            this._lblNarrative.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _btnOk
            // 
            this._btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._btnOk.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._btnOk.Location = new System.Drawing.Point(337, 528);
            this._btnOk.Name = "_btnOk";
            this._btnOk.Size = new System.Drawing.Size(67, 21);
            this._btnOk.TabIndex = 5;
            this._btnOk.Text = "OK";
            this._btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // _lblSendAuthorEmail
            // 
            this._lblSendAuthorEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._lblSendAuthorEmail.Location = new System.Drawing.Point(5, 530);
            this._lblSendAuthorEmail.Name = "_lblSendAuthorEmail";
            this._lblSendAuthorEmail.Size = new System.Drawing.Size(146, 19);
            this._lblSendAuthorEmail.TabIndex = 6;
            this._lblSendAuthorEmail.TabStop = true;
            this._lblSendAuthorEmail.Text = "Contact us";
            this._lblSendAuthorEmail.Visible = false;
            this._lblSendAuthorEmail.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblSendAuthorEmail_LinkClicked);
            // 
            // _lblBuiltOn
            // 
            this._lblBuiltOn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblBuiltOn.Location = new System.Drawing.Point(15, 113);
            this._lblBuiltOn.Name = "_lblBuiltOn";
            this._lblBuiltOn.Size = new System.Drawing.Size(372, 23);
            this._lblBuiltOn.TabIndex = 7;
            this._lblBuiltOn.Text = "Built on";
            this._lblBuiltOn.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _llVisitWebsite
            // 
            this._llVisitWebsite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this._llVisitWebsite.AutoSize = true;
            this._llVisitWebsite.Location = new System.Drawing.Point(76, 530);
            this._llVisitWebsite.Name = "_llVisitWebsite";
            this._llVisitWebsite.Size = new System.Drawing.Size(83, 13);
            this._llVisitWebsite.TabIndex = 8;
            this._llVisitWebsite.TabStop = true;
            this._llVisitWebsite.Text = "HyRAM website";
            this._llVisitWebsite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.llVisitWebsite_LinkClicked);
            // 
            // _pbLogo
            // 
            this._pbLogo.Image = global::UIHelpers.Properties.Resources.hyram_bigtitle;
            this._pbLogo.Location = new System.Drawing.Point(-2, 0);
            this._pbLogo.Name = "_pbLogo";
            this._pbLogo.Size = new System.Drawing.Size(408, 90);
            this._pbLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._pbLogo.TabIndex = 9;
            this._pbLogo.TabStop = false;
            // 
            // _tbCopyright
            // 
            this._tbCopyright.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._tbCopyright.Location = new System.Drawing.Point(17, 172);
            this._tbCopyright.Multiline = true;
            this._tbCopyright.Name = "_tbCopyright";
            this._tbCopyright.ReadOnly = true;
            this._tbCopyright.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this._tbCopyright.Size = new System.Drawing.Size(389, 268);
            this._tbCopyright.TabIndex = 10;
            // 
            // FrmMscAbout
            // 
            this.AcceptButton = this._btnOk;
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.CancelButton = this._btnOk;
            this.ClientSize = new System.Drawing.Size(408, 556);
            this.ControlBox = false;
            this.Controls.Add(this._tbCopyright);
            this.Controls.Add(this._pbLogo);
            this.Controls.Add(this._llVisitWebsite);
            this.Controls.Add(this._lblBuiltOn);
            this.Controls.Add(this._lblSendAuthorEmail);
            this.Controls.Add(this._btnOk);
            this.Controls.Add(this._lblNarrative);
            this.Controls.Add(this._lblVersion);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmMscAbout";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.Activated += new System.EventHandler(this.frmMSCAbout_Activated);
            this.Load += new System.EventHandler(this.frmMSCAbout_Load);
            ((System.ComponentModel.ISupportInitialize)(this._pbLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void frmMSCAbout_Load(object sender, EventArgs e)
        {
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblSendAuthorEmail_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            var useAuthorName = false;

            if (AuthorName != null)
                if (AuthorName.Length > 0)
                    useAuthorName = true;

            var outText = "mailto:" + _mAuthorEmail;
            if (useAuthorName) outText += "(" + AuthorName + ")";

            var processObj = new Process();
            processObj.StartInfo.FileName = outText;

            processObj.Start();
        }

        private void llVisitWebsite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_mWebsiteUrl == null)
                MessageBox.Show("Website URL has not been set.");
            else
                try
                {
                    Process.Start(_mWebsiteUrl);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Could not access website due to this error: " + ex);
                }
        }

        private void frmMSCAbout_Activated(object sender, EventArgs e)
        {
            MakeLabelsTransparent();
        }

        private void MakeLabelsTransparent()
        {
            foreach (Control thisChild in Controls) MakeLabelsTransparent(thisChild);
        }

        private void MakeLabelsTransparent(Control controlToMakeTransparent)
        {
            foreach (Control thisChild in controlToMakeTransparent.Controls)
            {
                if (thisChild.HasChildren) MakeLabelsTransparent(thisChild);

                if (thisChild is Label)
                    if (thisChild.BackColor != Color.Transparent)
                        thisChild.BackColor = Color.Transparent;
            }

            if (controlToMakeTransparent is Label)
                if (controlToMakeTransparent.BackColor != Color.Transparent)
                    controlToMakeTransparent.BackColor = Color.Transparent;
        }
    }
}