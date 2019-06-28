namespace QRA_Frontend.ActionPanels {
	partial class ApOutputNavigator {
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
            this.btnScenarioStats = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnScenarioStats
            // 
            this.btnScenarioStats.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScenarioStats.Location = new System.Drawing.Point(8, 14);
            this.btnScenarioStats.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnScenarioStats.Name = "btnScenarioStats";
            this.btnScenarioStats.Size = new System.Drawing.Size(369, 28);
            this.btnScenarioStats.TabIndex = 10;
            this.btnScenarioStats.Text = "Scenario Stats";
            this.btnScenarioStats.UseVisualStyleBackColor = true;
            this.btnScenarioStats.Click += new System.EventHandler(this.btnScenarioStats_Click);
            // 
            // apOutputNavigator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnScenarioStats);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "ApOutputNavigator";
            this.Size = new System.Drawing.Size(385, 97);
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button btnScenarioStats;

	}
}
