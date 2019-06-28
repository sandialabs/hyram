namespace QRA_Frontend {
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
            this.tmrCloseSpashscreen = new System.Windows.Forms.Timer(this.components);
            this.tmrBringMeBack = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // tmrCloseSpashscreen
            // 
            this.tmrCloseSpashscreen.Interval = 2250;
            this.tmrCloseSpashscreen.Tick += new System.EventHandler(this.tmrCloseSpashscreen_Tick);
            // 
            // tmrBringMeBack
            // 
            this.tmrBringMeBack.Interval = 25;
            //this.tmrBringMeBack.Tick += new System.EventHandler(this.tmrBringMeBack_Tick);
            // 
            // Splashscreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::QRA_Frontend.Properties.Resources.splash_screen;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ClientSize = new System.Drawing.Size(725, 363);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Splashscreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Splashscreen";
            this.Activated += new System.EventHandler(this.Splashscreen_Activated);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer tmrCloseSpashscreen;
		private System.Windows.Forms.Timer tmrBringMeBack;
	}
}