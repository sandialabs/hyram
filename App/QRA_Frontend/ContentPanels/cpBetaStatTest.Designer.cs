namespace QRA_Frontend.ContentPanels {
	partial class cpBetaStatTest {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(cpBetaStatTest));
			this.gbInput = new System.Windows.Forms.GroupBox();
			this.TbB = new System.Windows.Forms.TextBox();
			this.lblB = new System.Windows.Forms.Label();
			this.tbA = new System.Windows.Forms.TextBox();
			this.lblA = new System.Windows.Forms.Label();
			this.tbNarrative = new System.Windows.Forms.TextBox();
			tbNarrative.Text = @"test 1:
 mu = 0:0.2:1;
 sigma = 0.2:0.2:1.2;
 [m, v] = lognstat (mu, sigma);
 expected_m = [1.0202, 1.3231, 1.7860, 2.5093,  3.6693,   5.5845];
 expected_v = [0.0425, 0.3038, 1.3823, 5.6447, 23.1345, 100.4437];
 assert (m, expected_m, 0.001);
 assert (v, expected_v, 0.001);

test 2:
 sigma = 0.2:0.2:1.2;
 [m, v] = lognstat (0, sigma);
 expected_m = [1.0202, 1.0833, 1.1972, 1.3771, 1.6487,  2.0544];
 expected_v = [0.0425, 0.2036, 0.6211, 1.7002, 4.6708, 13.5936];
 assert (m, expected_m, 0.001);
 assert (v, expected_v, 0.001);";

			this.btnTest = new System.Windows.Forms.Button();
			this.gbOutput = new System.Windows.Forms.GroupBox();
			this.tbV = new System.Windows.Forms.TextBox();
			this.lblV = new System.Windows.Forms.Label();
			this.tbM = new System.Windows.Forms.TextBox();
			this.lblM = new System.Windows.Forms.Label();
			this.gbInput.SuspendLayout();
			this.gbOutput.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbInput
			// 
			this.gbInput.Controls.Add(this.TbB);
			this.gbInput.Controls.Add(this.lblB);
			this.gbInput.Controls.Add(this.tbA);
			this.gbInput.Controls.Add(this.lblA);
			this.gbInput.Location = new System.Drawing.Point(3, 261);
			this.gbInput.Name = "gbInput";
			this.gbInput.Size = new System.Drawing.Size(155, 75);
			this.gbInput.TabIndex = 0;
			this.gbInput.TabStop = false;
			this.gbInput.Text = "Input";
			// 
			// TbB
			// 
			this.TbB.Location = new System.Drawing.Point(30, 45);
			this.TbB.Name = "TbB";
			this.TbB.Size = new System.Drawing.Size(104, 20);
			this.TbB.TabIndex = 3;
			this.TbB.Text = "1:0.2:2";
			// 
			// lblB
			// 
			this.lblB.AutoSize = true;
			this.lblB.Location = new System.Drawing.Point(7, 48);
			this.lblB.Name = "lblB";
			this.lblB.Size = new System.Drawing.Size(17, 13);
			this.lblB.TabIndex = 2;
			this.lblB.Text = "B:";
			// 
			// tbA
			// 
			this.tbA.Location = new System.Drawing.Point(30, 17);
			this.tbA.Name = "tbA";
			this.tbA.Size = new System.Drawing.Size(104, 20);
			this.tbA.TabIndex = 1;
			this.tbA.Text = "1:6";
			// 
			// lblA
			// 
			this.lblA.AutoSize = true;
			this.lblA.Location = new System.Drawing.Point(7, 20);
			this.lblA.Name = "lblA";
			this.lblA.Size = new System.Drawing.Size(17, 13);
			this.lblA.TabIndex = 0;
			this.lblA.Text = "A:";
			// 
			// tbNarrative
			// 
			this.tbNarrative.Dock = System.Windows.Forms.DockStyle.Top;
			this.tbNarrative.Enabled = false;
			this.tbNarrative.Location = new System.Drawing.Point(0, 0);
			this.tbNarrative.Multiline = true;
			this.tbNarrative.Name = "tbNarrative";
			this.tbNarrative.Size = new System.Drawing.Size(400, 245);
			this.tbNarrative.TabIndex = 1;
			this.tbNarrative.Text = resources.GetString("tbNarrative.Text");
			// 
			// btnTest
			// 
			this.btnTest.Location = new System.Drawing.Point(163, 271);
			this.btnTest.Name = "btnTest";
			this.btnTest.Size = new System.Drawing.Size(93, 23);
			this.btnTest.TabIndex = 2;
			this.btnTest.Text = "--> Execute -->";
			this.btnTest.UseVisualStyleBackColor = true;
			this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
			// 
			// gbOutput
			// 
			this.gbOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gbOutput.Controls.Add(this.tbV);
			this.gbOutput.Controls.Add(this.lblV);
			this.gbOutput.Controls.Add(this.tbM);
			this.gbOutput.Controls.Add(this.lblM);
			this.gbOutput.Location = new System.Drawing.Point(262, 261);
			this.gbOutput.Name = "gbOutput";
			this.gbOutput.Size = new System.Drawing.Size(130, 75);
			this.gbOutput.TabIndex = 3;
			this.gbOutput.TabStop = false;
			this.gbOutput.Text = "Output";
			// 
			// tbV
			// 
			this.tbV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbV.Location = new System.Drawing.Point(30, 45);
			this.tbV.Name = "tbV";
			this.tbV.Size = new System.Drawing.Size(94, 20);
			this.tbV.TabIndex = 3;
			// 
			// lblV
			// 
			this.lblV.AutoSize = true;
			this.lblV.Location = new System.Drawing.Point(7, 48);
			this.lblV.Name = "lblV";
			this.lblV.Size = new System.Drawing.Size(17, 13);
			this.lblV.TabIndex = 2;
			this.lblV.Text = "V:";
			// 
			// tbM
			// 
			this.tbM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tbM.Location = new System.Drawing.Point(30, 17);
			this.tbM.Name = "tbM";
			this.tbM.Size = new System.Drawing.Size(94, 20);
			this.tbM.TabIndex = 1;
			// 
			// lblM
			// 
			this.lblM.AutoSize = true;
			this.lblM.Location = new System.Drawing.Point(7, 20);
			this.lblM.Name = "lblM";
			this.lblM.Size = new System.Drawing.Size(19, 13);
			this.lblM.TabIndex = 0;
			this.lblM.Text = "M:";
			// 
			// cpBetaStatTest
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbOutput);
			this.Controls.Add(this.btnTest);
			this.Controls.Add(this.tbNarrative);
			this.Controls.Add(this.gbInput);
			this.Name = "cpBetaStatTest";
			this.Size = new System.Drawing.Size(400, 344);
			this.Load += new System.EventHandler(this.cpBetaStatTest_Load);
			this.gbInput.ResumeLayout(false);
			this.gbInput.PerformLayout();
			this.gbOutput.ResumeLayout(false);
			this.gbOutput.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox gbInput;
		private System.Windows.Forms.TextBox tbA;
		private System.Windows.Forms.Label lblA;
		private System.Windows.Forms.TextBox tbNarrative;
		private System.Windows.Forms.TextBox TbB;
		private System.Windows.Forms.Label lblB;
		private System.Windows.Forms.Button btnTest;
		private System.Windows.Forms.GroupBox gbOutput;
		private System.Windows.Forms.TextBox tbV;
		private System.Windows.Forms.Label lblV;
		private System.Windows.Forms.TextBox tbM;
		private System.Windows.Forms.Label lblM;
	}
}
