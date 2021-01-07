namespace SandiaNationalLaboratories.Hyram {
	partial class QraOutputNavPanel {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.qraResultsButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // qraResultsButton
            // 
            this.qraResultsButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.qraResultsButton.Location = new System.Drawing.Point(6, 11);
            this.qraResultsButton.Name = "qraResultsButton";
            this.qraResultsButton.Size = new System.Drawing.Size(277, 23);
            this.qraResultsButton.TabIndex = 10;
            this.qraResultsButton.Text = "Output";
            this.qraResultsButton.UseVisualStyleBackColor = true;
            this.qraResultsButton.Click += new System.EventHandler(this.qraResultsButton_Click);
            // 
            // QraOutputNavPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.qraResultsButton);
            this.Name = "QraOutputNavPanel";
            this.Size = new System.Drawing.Size(289, 79);
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button qraResultsButton;

	}
}
