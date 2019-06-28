namespace QRA_Frontend.ActionPanels {
	partial class ApSafety {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
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
			this.btnScenario2 = new System.Windows.Forms.Button();
			this.btnScenario1 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnScenario2
			// 
			this.btnScenario2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnScenario2.Location = new System.Drawing.Point(3, 41);
			this.btnScenario2.Name = "btnScenario2";
			this.btnScenario2.Size = new System.Drawing.Size(191, 23);
			this.btnScenario2.TabIndex = 1;
			this.btnScenario2.Text = "Scenario 2: Under Construction";
			this.btnScenario2.UseVisualStyleBackColor = true;
			this.btnScenario2.Click += new System.EventHandler(this.btnScenario2_Click);
			// 
			// btnScenario1
			// 
			this.btnScenario1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnScenario1.Location = new System.Drawing.Point(3, 12);
			this.btnScenario1.Name = "btnScenario1";
			this.btnScenario1.Size = new System.Drawing.Size(191, 23);
			this.btnScenario1.TabIndex = 2;
			this.btnScenario1.Text = "Scenario 1: Principal Exit Blocked";
			this.btnScenario1.UseVisualStyleBackColor = true;
			this.btnScenario1.Click += new System.EventHandler(this.btnScenario1_Click);
			// 
			// apSafety
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnScenario1);
			this.Controls.Add(this.btnScenario2);
			this.Name = "ApSafety";
			this.Size = new System.Drawing.Size(201, 79);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnScenario2;
		private System.Windows.Forms.Button btnScenario1;
	}
}
