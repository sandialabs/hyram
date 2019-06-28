namespace QRA_Frontend.ActionPanels {
	partial class ApHazMat {
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
			this.btnScenario1 = new System.Windows.Forms.Button();
			this.btnScenario2 = new System.Windows.Forms.Button();
			this.btnScenario3 = new System.Windows.Forms.Button();
			this.btnScenario4 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnScenario1
			// 
			this.btnScenario1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnScenario1.Location = new System.Drawing.Point(3, 3);
			this.btnScenario1.Name = "btnScenario1";
			this.btnScenario1.Size = new System.Drawing.Size(304, 23);
			this.btnScenario1.TabIndex = 3;
			this.btnScenario1.Text = "Scenario 1: Unintended Release";
			this.btnScenario1.UseVisualStyleBackColor = true;
			this.btnScenario1.Click += new System.EventHandler(this.btnScenario1_Click);
			// 
			// btnScenario2
			// 
			this.btnScenario2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnScenario2.Location = new System.Drawing.Point(4, 30);
			this.btnScenario2.Name = "btnScenario2";
			this.btnScenario2.Size = new System.Drawing.Size(304, 23);
			this.btnScenario2.TabIndex = 4;
			this.btnScenario2.Text = "Scenario 2: Exposure Fire";
			this.btnScenario2.UseVisualStyleBackColor = true;
			this.btnScenario2.Click += new System.EventHandler(this.btnScenario2_Click);
			// 
			// btnScenario3
			// 
			this.btnScenario3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnScenario3.Location = new System.Drawing.Point(4, 58);
			this.btnScenario3.Name = "btnScenario3";
			this.btnScenario3.Size = new System.Drawing.Size(304, 23);
			this.btnScenario3.TabIndex = 5;
			this.btnScenario3.Text = "Scenario 3: External Factor";
			this.btnScenario3.UseVisualStyleBackColor = true;
			this.btnScenario3.Click += new System.EventHandler(this.btnScenario3_Click);
			// 
			// btnScenario4
			// 
			this.btnScenario4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnScenario4.Location = new System.Drawing.Point(4, 86);
			this.btnScenario4.Name = "btnScenario4";
			this.btnScenario4.Size = new System.Drawing.Size(304, 23);
			this.btnScenario4.TabIndex = 6;
			this.btnScenario4.Text = "Scenario 4: Discharge and Protection Systems Failure";
			this.btnScenario4.UseVisualStyleBackColor = true;
			this.btnScenario4.Click += new System.EventHandler(this.btnScenario4_Click);
			// 
			// apHazMat
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnScenario4);
			this.Controls.Add(this.btnScenario3);
			this.Controls.Add(this.btnScenario2);
			this.Controls.Add(this.btnScenario1);
			this.Name = "ApHazMat";
			this.Size = new System.Drawing.Size(312, 111);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button btnScenario1;
		private System.Windows.Forms.Button btnScenario2;
		private System.Windows.Forms.Button btnScenario3;
		private System.Windows.Forms.Button btnScenario4;
	}
}
