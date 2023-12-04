namespace SandiaNationalLaboratories.Hyram {
	partial class TpdForm {

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.TempSelector = new System.Windows.Forms.RadioButton();
            this.DenSelector = new System.Windows.Forms.RadioButton();
            this.PresSelector = new System.Windows.Forms.RadioButton();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.DenInput = new System.Windows.Forms.TextBox();
            this.PresInput = new System.Windows.Forms.TextBox();
            this.TempInput = new System.Windows.Forms.TextBox();
            this.densityInputLabel = new System.Windows.Forms.Label();
            this.pressureInputLabel = new System.Windows.Forms.Label();
            this.temperatureInputLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DenUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.PresUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.TempUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.warningLabel = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // TempSelector
            // 
            this.TempSelector.AutoSize = true;
            this.TempSelector.Checked = true;
            this.TempSelector.Location = new System.Drawing.Point(268, 15);
            this.TempSelector.Name = "TempSelector";
            this.TempSelector.Size = new System.Drawing.Size(85, 17);
            this.TempSelector.TabIndex = 1;
            this.TempSelector.TabStop = true;
            this.TempSelector.Text = "Temperature";
            this.TempSelector.UseVisualStyleBackColor = true;
            this.TempSelector.CheckedChanged += new System.EventHandler(this.OutputCheckedChanged);
            // 
            // DenSelector
            // 
            this.DenSelector.AutoSize = true;
            this.DenSelector.Location = new System.Drawing.Point(268, 51);
            this.DenSelector.Name = "DenSelector";
            this.DenSelector.Size = new System.Drawing.Size(60, 17);
            this.DenSelector.TabIndex = 3;
            this.DenSelector.Text = "Density";
            this.DenSelector.UseVisualStyleBackColor = true;
            this.DenSelector.CheckedChanged += new System.EventHandler(this.OutputCheckedChanged);
            // 
            // PresSelector
            // 
            this.PresSelector.AutoSize = true;
            this.PresSelector.Location = new System.Drawing.Point(268, 33);
            this.PresSelector.Name = "PresSelector";
            this.PresSelector.Size = new System.Drawing.Size(66, 17);
            this.PresSelector.TabIndex = 2;
            this.PresSelector.Text = "Pressure";
            this.PresSelector.UseVisualStyleBackColor = true;
            this.PresSelector.CheckedChanged += new System.EventHandler(this.OutputCheckedChanged);
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Enabled = false;
            this.SubmitBtn.Location = new System.Drawing.Point(268, 170);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(107, 23);
            this.SubmitBtn.TabIndex = 11;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // DenInput
            // 
            this.DenInput.Enabled = false;
            this.DenInput.Location = new System.Drawing.Point(268, 141);
            this.DenInput.Name = "DenInput";
            this.DenInput.Size = new System.Drawing.Size(107, 20);
            this.DenInput.TabIndex = 10;
            this.DenInput.TextChanged += new System.EventHandler(this.DenInput_TextChanged);
            // 
            // PresInput
            // 
            this.PresInput.Enabled = false;
            this.PresInput.Location = new System.Drawing.Point(268, 112);
            this.PresInput.Name = "PresInput";
            this.PresInput.Size = new System.Drawing.Size(107, 20);
            this.PresInput.TabIndex = 8;
            this.PresInput.TextChanged += new System.EventHandler(this.PresInput_TextChanged);
            // 
            // TempInput
            // 
            this.TempInput.Enabled = false;
            this.TempInput.Location = new System.Drawing.Point(268, 84);
            this.TempInput.Name = "TempInput";
            this.TempInput.Size = new System.Drawing.Size(107, 20);
            this.TempInput.TabIndex = 6;
            this.TempInput.TextChanged += new System.EventHandler(this.TempInput_TextChanged);
            // 
            // densityInputLabel
            // 
            this.densityInputLabel.AutoSize = true;
            this.densityInputLabel.Location = new System.Drawing.Point(8, 144);
            this.densityInputLabel.Name = "densityInputLabel";
            this.densityInputLabel.Size = new System.Drawing.Size(42, 13);
            this.densityInputLabel.TabIndex = 17;
            this.densityInputLabel.Text = "Density";
            // 
            // pressureInputLabel
            // 
            this.pressureInputLabel.AutoSize = true;
            this.pressureInputLabel.Location = new System.Drawing.Point(8, 115);
            this.pressureInputLabel.Name = "pressureInputLabel";
            this.pressureInputLabel.Size = new System.Drawing.Size(97, 13);
            this.pressureInputLabel.TabIndex = 16;
            this.pressureInputLabel.Text = "Pressure (absolute)";
            // 
            // temperatureInputLabel
            // 
            this.temperatureInputLabel.AutoSize = true;
            this.temperatureInputLabel.Location = new System.Drawing.Point(8, 87);
            this.temperatureInputLabel.Name = "temperatureInputLabel";
            this.temperatureInputLabel.Size = new System.Drawing.Size(67, 13);
            this.temperatureInputLabel.TabIndex = 15;
            this.temperatureInputLabel.Text = "Temperature";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Select output:";
            // 
            // DenUnitSelector
            // 
            this.DenUnitSelector.Converter = null;
            this.DenUnitSelector.Location = new System.Drawing.Point(110, 140);
            this.DenUnitSelector.Name = "DenUnitSelector";
            this.DenUnitSelector.SelectedItem = null;
            this.DenUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.DenUnitSelector.StoredValue = new double[0];
            this.DenUnitSelector.TabIndex = 9;
            this.DenUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.DenUnitSelector_OnSelectedIndexChange);
            // 
            // PresUnitSelector
            // 
            this.PresUnitSelector.Converter = null;
            this.PresUnitSelector.Location = new System.Drawing.Point(110, 112);
            this.PresUnitSelector.Name = "PresUnitSelector";
            this.PresUnitSelector.SelectedItem = null;
            this.PresUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.PresUnitSelector.StoredValue = new double[0];
            this.PresUnitSelector.TabIndex = 7;
            this.PresUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.PresUnitSelector_OnSelectedIndexChange);
            // 
            // TempUnitSelector
            // 
            this.TempUnitSelector.Converter = null;
            this.TempUnitSelector.Location = new System.Drawing.Point(110, 84);
            this.TempUnitSelector.Name = "TempUnitSelector";
            this.TempUnitSelector.SelectedItem = null;
            this.TempUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.TempUnitSelector.StoredValue = new double[0];
            this.TempUnitSelector.TabIndex = 5;
            this.TempUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.TempUnitSelector_OnSelectedIndexChange);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.warningLabel);
            this.panel1.Controls.Add(this.spinnerPictureBox);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.temperatureInputLabel);
            this.panel1.Controls.Add(this.pressureInputLabel);
            this.panel1.Controls.Add(this.densityInputLabel);
            this.panel1.Controls.Add(this.DenSelector);
            this.panel1.Controls.Add(this.TempUnitSelector);
            this.panel1.Controls.Add(this.TempSelector);
            this.panel1.Controls.Add(this.PresUnitSelector);
            this.panel1.Controls.Add(this.PresSelector);
            this.panel1.Controls.Add(this.DenUnitSelector);
            this.panel1.Controls.Add(this.SubmitBtn);
            this.panel1.Controls.Add(this.TempInput);
            this.panel1.Controls.Add(this.DenInput);
            this.panel1.Controls.Add(this.PresInput);
            this.panel1.Location = new System.Drawing.Point(0, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(8);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(984, 578);
            this.panel1.TabIndex = 57;
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.BackColor = System.Drawing.Color.MistyRose;
            this.warningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningLabel.ForeColor = System.Drawing.Color.Maroon;
            this.warningLabel.Location = new System.Drawing.Point(8, 206);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Padding = new System.Windows.Forms.Padding(4);
            this.warningLabel.Size = new System.Drawing.Size(198, 23);
            this.warningLabel.TabIndex = 61;
            this.warningLabel.Text = "Warning/error message here";
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.warningLabel.Visible = false;
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(238, 170);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 60;
            this.spinnerPictureBox.TabStop = false;
            // 
            // TpdForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "TpdForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.TpdForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.RadioButton TempSelector;
		private System.Windows.Forms.RadioButton DenSelector;
		private System.Windows.Forms.RadioButton PresSelector;
		private System.Windows.Forms.Button SubmitBtn;
		private System.Windows.Forms.TextBox DenInput;
		private System.Windows.Forms.TextBox PresInput;
		private System.Windows.Forms.TextBox TempInput;
		private ValueConverterDropdown DenUnitSelector;
		private ValueConverterDropdown PresUnitSelector;
		private ValueConverterDropdown TempUnitSelector;
		private System.Windows.Forms.Label densityInputLabel;
		private System.Windows.Forms.Label pressureInputLabel;
		private System.Windows.Forms.Label temperatureInputLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
    }
}
