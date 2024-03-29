﻿namespace SandiaNationalLaboratories.Hyram {
	partial class Splashscreen {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Splashscreen));
            this.FadeOutTime = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // FadeOutTime
            // 
            this.FadeOutTime.Interval = 25;
            this.FadeOutTime.Tick += new System.EventHandler(this.FadeOutTime_Tick);
            // 
            // Splashscreen
            // 
            resources.ApplyResources(this, "$this");
            this.BackgroundImage = global::SandiaNationalLaboratories.Hyram.Properties.Resources.BannerLogov2;
            this.ControlBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Splashscreen";
            this.ShowIcon = false;
            this.TopMost = true;
            this.ResumeLayout(false);

		}

		#endregion
        private System.Windows.Forms.Timer FadeOutTime;
    }
}