namespace QRA_Frontend.ContentPanels {
	partial class CpHazMatScenario1 {
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
			this.lblNarrative = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// lblNarrative
			// 
			this.lblNarrative.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblNarrative.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblNarrative.Location = new System.Drawing.Point(0, 0);
			this.lblNarrative.Name = "lblNarrative";
			this.lblNarrative.Size = new System.Drawing.Size(604, 571);
			this.lblNarrative.TabIndex = 2;
			this.lblNarrative.Text = "This design scenario addresses the concern regarding the spread of hazardous cond" +
    "itions from the point of release.";
			this.lblNarrative.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// cpHazMatScenario1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblNarrative);
			this.Name = "CpHazMatScenario1";
			this.Size = new System.Drawing.Size(604, 571);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label lblNarrative;
	}
}
