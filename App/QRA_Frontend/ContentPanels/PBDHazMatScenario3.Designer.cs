namespace QRA_Frontend.ContentPanels {
	partial class CpHazMatScenario3 {
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
			this.tc5443 = new System.Windows.Forms.TabControl();
			this.tpPbInput = new System.Windows.Forms.TabPage();
			this.cbPipeDiams = new System.Windows.Forms.ComboBox();
			this.lblPipeInnerDiameter = new System.Windows.Forms.Label();
			this.tpOutput = new System.Windows.Forms.TabPage();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.tcScenarios = new System.Windows.Forms.TabControl();
			this.tpPipeBurst = new System.Windows.Forms.TabPage();
			this.tpOther1 = new System.Windows.Forms.TabPage();
			this.tpOther2 = new System.Windows.Forms.TabPage();
			this.tc5443.SuspendLayout();
			this.tpPbInput.SuspendLayout();
			this.tpOutput.SuspendLayout();
			this.tcScenarios.SuspendLayout();
			this.tpPipeBurst.SuspendLayout();
			this.SuspendLayout();
			// 
			// tc5443
			// 
			this.tc5443.Controls.Add(this.tpPbInput);
			this.tc5443.Controls.Add(this.tpOutput);
			this.tc5443.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tc5443.Location = new System.Drawing.Point(3, 3);
			this.tc5443.Name = "tc5443";
			this.tc5443.SelectedIndex = 0;
			this.tc5443.Size = new System.Drawing.Size(540, 465);
			this.tc5443.TabIndex = 4;
			// 
			// tpPbInput
			// 
			this.tpPbInput.Controls.Add(this.cbPipeDiams);
			this.tpPbInput.Controls.Add(this.lblPipeInnerDiameter);
			this.tpPbInput.Location = new System.Drawing.Point(4, 22);
			this.tpPbInput.Name = "tpPbInput";
			this.tpPbInput.Size = new System.Drawing.Size(532, 439);
			this.tpPbInput.TabIndex = 2;
			this.tpPbInput.Text = "Inputs";
			this.tpPbInput.UseVisualStyleBackColor = true;
			// 
			// cbPipeDiams
			// 
			this.cbPipeDiams.FormattingEnabled = true;
			this.cbPipeDiams.Items.AddRange(new object[] {
            "0.25",
            "0.31",
            "0.50",
            "1.00",
            "2.00"});
			this.cbPipeDiams.Location = new System.Drawing.Point(165, 27);
			this.cbPipeDiams.Name = "cbPipeDiams";
			this.cbPipeDiams.Size = new System.Drawing.Size(121, 21);
			this.cbPipeDiams.TabIndex = 1;
			// 
			// lblPipeInnerDiameter
			// 
			this.lblPipeInnerDiameter.AutoSize = true;
			this.lblPipeInnerDiameter.Location = new System.Drawing.Point(18, 30);
			this.lblPipeInnerDiameter.Name = "lblPipeInnerDiameter";
			this.lblPipeInnerDiameter.Size = new System.Drawing.Size(140, 13);
			this.lblPipeInnerDiameter.TabIndex = 0;
			this.lblPipeInnerDiameter.Text = "Pipe inner diameter (inches):";
			// 
			// tpOutput
			// 
			this.tpOutput.Controls.Add(this.label2);
			this.tpOutput.Controls.Add(this.label1);
			this.tpOutput.Location = new System.Drawing.Point(4, 22);
			this.tpOutput.Name = "tpOutput";
			this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
			this.tpOutput.Size = new System.Drawing.Size(532, 297);
			this.tpOutput.TabIndex = 1;
			this.tpOutput.Text = "Output";
			this.tpOutput.UseVisualStyleBackColor = true;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(160, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(34, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "100.0";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(18, 30);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(135, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Pipe area leak percentage:";
			// 
			// tcScenarios
			// 
			this.tcScenarios.Controls.Add(this.tpPipeBurst);
			this.tcScenarios.Controls.Add(this.tpOther1);
			this.tcScenarios.Controls.Add(this.tpOther2);
			this.tcScenarios.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcScenarios.Location = new System.Drawing.Point(0, 0);
			this.tcScenarios.Name = "tcScenarios";
			this.tcScenarios.SelectedIndex = 0;
			this.tcScenarios.Size = new System.Drawing.Size(554, 497);
			this.tcScenarios.TabIndex = 5;
			// 
			// tpPipeBurst
			// 
			this.tpPipeBurst.Controls.Add(this.tc5443);
			this.tpPipeBurst.Location = new System.Drawing.Point(4, 22);
			this.tpPipeBurst.Name = "tpPipeBurst";
			this.tpPipeBurst.Padding = new System.Windows.Forms.Padding(3);
			this.tpPipeBurst.Size = new System.Drawing.Size(546, 471);
			this.tpPipeBurst.TabIndex = 0;
			this.tpPipeBurst.Text = "Pipe Burst";
			this.tpPipeBurst.UseVisualStyleBackColor = true;
			// 
			// tpOther1
			// 
			this.tpOther1.Location = new System.Drawing.Point(4, 22);
			this.tpOther1.Name = "tpOther1";
			this.tpOther1.Padding = new System.Windows.Forms.Padding(3);
			this.tpOther1.Size = new System.Drawing.Size(546, 329);
			this.tpOther1.TabIndex = 1;
			this.tpOther1.Text = "Other 1";
			this.tpOther1.UseVisualStyleBackColor = true;
			// 
			// tpOther2
			// 
			this.tpOther2.Location = new System.Drawing.Point(4, 22);
			this.tpOther2.Name = "tpOther2";
			this.tpOther2.Size = new System.Drawing.Size(546, 329);
			this.tpOther2.TabIndex = 2;
			this.tpOther2.Text = "Other 2";
			this.tpOther2.UseVisualStyleBackColor = true;
			// 
			// cpHazMatScenario3
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tcScenarios);
			this.Name = "CpHazMatScenario3";
			this.Size = new System.Drawing.Size(554, 497);
			this.tc5443.ResumeLayout(false);
			this.tpPbInput.ResumeLayout(false);
			this.tpPbInput.PerformLayout();
			this.tpOutput.ResumeLayout(false);
			this.tpOutput.PerformLayout();
			this.tcScenarios.ResumeLayout(false);
			this.tpPipeBurst.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tc5443;
        private System.Windows.Forms.TabPage tpPbInput;
        private System.Windows.Forms.ComboBox cbPipeDiams;
        private System.Windows.Forms.Label lblPipeInnerDiameter;
        private System.Windows.Forms.TabPage tpOutput;
        private System.Windows.Forms.TabControl tcScenarios;
        private System.Windows.Forms.TabPage tpPipeBurst;
        private System.Windows.Forms.TabPage tpOther1;
        private System.Windows.Forms.TabPage tpOther2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
	}
}
