namespace SandiaNationalLaboratories.Hyram {
	partial class TntForm {
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
            this.VaporMassInput = new System.Windows.Forms.TextBox();
            this.massInputLabel = new System.Windows.Forms.Label();
            this.yieldInputLabel = new System.Windows.Forms.Label();
            this.YieldInput = new System.Windows.Forms.TextBox();
            this.EqMassOutput = new System.Windows.Forms.TextBox();
            this.equivalentMassLabel = new System.Windows.Forms.Label();
            this.EqMassUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.VaporMassUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.warningMsg = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // VaporMassInput
            // 
            this.VaporMassInput.Location = new System.Drawing.Point(332, 16);
            this.VaporMassInput.Name = "VaporMassInput";
            this.VaporMassInput.Size = new System.Drawing.Size(107, 20);
            this.VaporMassInput.TabIndex = 2;
            this.VaporMassInput.TextChanged += new System.EventHandler(this.VaporMassInput_TextChanged);
            // 
            // massInputLabel
            // 
            this.massInputLabel.AutoSize = true;
            this.massInputLabel.Location = new System.Drawing.Point(8, 24);
            this.massInputLabel.Name = "massInputLabel";
            this.massInputLabel.Size = new System.Drawing.Size(161, 13);
            this.massInputLabel.TabIndex = 39;
            this.massInputLabel.Text = "Flammable Vapor Release Mass:";
            // 
            // yieldInputLabel
            // 
            this.yieldInputLabel.AutoSize = true;
            this.yieldInputLabel.Location = new System.Drawing.Point(8, 51);
            this.yieldInputLabel.Name = "yieldInputLabel";
            this.yieldInputLabel.Size = new System.Drawing.Size(167, 13);
            this.yieldInputLabel.TabIndex = 42;
            this.yieldInputLabel.Text = "Explosive Energy Yield (%, 0-100):";
            // 
            // YieldInput
            // 
            this.YieldInput.Location = new System.Drawing.Point(332, 44);
            this.YieldInput.Name = "YieldInput";
            this.YieldInput.Size = new System.Drawing.Size(107, 20);
            this.YieldInput.TabIndex = 3;
            this.YieldInput.TextChanged += new System.EventHandler(this.YieldInput_TextChanged);
            // 
            // EqMassOutput
            // 
            this.EqMassOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EqMassOutput.Location = new System.Drawing.Point(332, 125);
            this.EqMassOutput.Name = "EqMassOutput";
            this.EqMassOutput.ReadOnly = true;
            this.EqMassOutput.Size = new System.Drawing.Size(107, 20);
            this.EqMassOutput.TabIndex = 6;
            // 
            // equivalentMassLabel
            // 
            this.equivalentMassLabel.AutoSize = true;
            this.equivalentMassLabel.Location = new System.Drawing.Point(8, 135);
            this.equivalentMassLabel.Name = "equivalentMassLabel";
            this.equivalentMassLabel.Size = new System.Drawing.Size(113, 13);
            this.equivalentMassLabel.TabIndex = 47;
            this.equivalentMassLabel.Text = "Equivalent TNT Mass:";
            // 
            // EqMassUnitSelector
            // 
            this.EqMassUnitSelector.Converter = null;
            this.EqMassUnitSelector.Location = new System.Drawing.Point(184, 125);
            this.EqMassUnitSelector.Name = "EqMassUnitSelector";
            this.EqMassUnitSelector.SelectedItem = null;
            this.EqMassUnitSelector.Size = new System.Drawing.Size(142, 22);
            this.EqMassUnitSelector.StoredValue = new double[0];
            this.EqMassUnitSelector.TabIndex = 5;
            this.EqMassUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.EqMassUnitSelector_OnSelectedIndexChanged);
            // 
            // VaporMassUnitSelector
            // 
            this.VaporMassUnitSelector.Converter = null;
            this.VaporMassUnitSelector.Location = new System.Drawing.Point(184, 16);
            this.VaporMassUnitSelector.Name = "VaporMassUnitSelector";
            this.VaporMassUnitSelector.SelectedItem = null;
            this.VaporMassUnitSelector.Size = new System.Drawing.Size(142, 22);
            this.VaporMassUnitSelector.StoredValue = new double[0];
            this.VaporMassUnitSelector.TabIndex = 1;
            this.VaporMassUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.VaporMassUnitSelector_OnSelectedIndexChanged);
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Enabled = false;
            this.SubmitBtn.Location = new System.Drawing.Point(332, 98);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(107, 23);
            this.SubmitBtn.TabIndex = 4;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.warningMsg);
            this.panel1.Controls.Add(this.spinnerPictureBox);
            this.panel1.Controls.Add(this.massInputLabel);
            this.panel1.Controls.Add(this.SubmitBtn);
            this.panel1.Controls.Add(this.VaporMassUnitSelector);
            this.panel1.Controls.Add(this.EqMassOutput);
            this.panel1.Controls.Add(this.VaporMassInput);
            this.panel1.Controls.Add(this.EqMassUnitSelector);
            this.panel1.Controls.Add(this.yieldInputLabel);
            this.panel1.Controls.Add(this.equivalentMassLabel);
            this.panel1.Controls.Add(this.YieldInput);
            this.panel1.Location = new System.Drawing.Point(0, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(8);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(984, 578);
            this.panel1.TabIndex = 51;
            // 
            // warningMsg
            // 
            this.warningMsg.AutoSize = true;
            this.warningMsg.BackColor = System.Drawing.Color.MistyRose;
            this.warningMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningMsg.ForeColor = System.Drawing.Color.Maroon;
            this.warningMsg.Location = new System.Drawing.Point(8, 159);
            this.warningMsg.Name = "warningMsg";
            this.warningMsg.Padding = new System.Windows.Forms.Padding(4);
            this.warningMsg.Size = new System.Drawing.Size(198, 23);
            this.warningMsg.TabIndex = 64;
            this.warningMsg.Text = "Warning/error message here";
            this.warningMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.warningMsg.Visible = false;
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(303, 98);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 63;
            this.spinnerPictureBox.TabStop = false;
            // 
            // TntForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "TntForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.TntForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox VaporMassInput;
		private ValueConverterDropdown VaporMassUnitSelector;
		private System.Windows.Forms.Label massInputLabel;
		private System.Windows.Forms.Label yieldInputLabel;
		private System.Windows.Forms.TextBox YieldInput;
		private System.Windows.Forms.TextBox EqMassOutput;
		private ValueConverterDropdown EqMassUnitSelector;
		private System.Windows.Forms.Label equivalentMassLabel;
        private System.Windows.Forms.Button SubmitBtn;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
        private System.Windows.Forms.Label warningMsg;
    }
}
