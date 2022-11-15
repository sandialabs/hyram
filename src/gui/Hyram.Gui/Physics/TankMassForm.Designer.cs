namespace SandiaNationalLaboratories.Hyram {
	partial class TankMassForm {

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.VolInput = new System.Windows.Forms.TextBox();
            this.PresInput = new System.Windows.Forms.TextBox();
            this.TempInput = new System.Windows.Forms.TextBox();
            this.volumeInputLabel = new System.Windows.Forms.Label();
            this.pressureInputLabel = new System.Windows.Forms.Label();
            this.temperatureInputLabel = new System.Windows.Forms.Label();
            this.massInputLabel = new System.Windows.Forms.Label();
            this.MassInput = new System.Windows.Forms.TextBox();
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.MassSelector = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.VolSelector = new System.Windows.Forms.RadioButton();
            this.TempSelector = new System.Windows.Forms.RadioButton();
            this.PresSelector = new System.Windows.Forms.RadioButton();
            this.warningLabel = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.MassUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.TempUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.PresUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.VolUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Enabled = false;
            this.SubmitBtn.Location = new System.Drawing.Point(273, 239);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(107, 23);
            this.SubmitBtn.TabIndex = 35;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // VolInput
            // 
            this.VolInput.Location = new System.Drawing.Point(273, 187);
            this.VolInput.Name = "VolInput";
            this.VolInput.Size = new System.Drawing.Size(107, 20);
            this.VolInput.TabIndex = 34;
            this.VolInput.TextChanged += new System.EventHandler(this.VolInput_TextChanged);
            // 
            // PresInput
            // 
            this.PresInput.Location = new System.Drawing.Point(273, 157);
            this.PresInput.Name = "PresInput";
            this.PresInput.Size = new System.Drawing.Size(107, 20);
            this.PresInput.TabIndex = 33;
            this.PresInput.TextChanged += new System.EventHandler(this.PresInput_TextChanged);
            // 
            // TempInput
            // 
            this.TempInput.Location = new System.Drawing.Point(273, 127);
            this.TempInput.Name = "TempInput";
            this.TempInput.Size = new System.Drawing.Size(107, 20);
            this.TempInput.TabIndex = 32;
            this.TempInput.TextChanged += new System.EventHandler(this.TempInput_TextChanged);
            // 
            // volumeInputLabel
            // 
            this.volumeInputLabel.AutoSize = true;
            this.volumeInputLabel.Location = new System.Drawing.Point(12, 190);
            this.volumeInputLabel.Name = "volumeInputLabel";
            this.volumeInputLabel.Size = new System.Drawing.Size(42, 13);
            this.volumeInputLabel.TabIndex = 28;
            this.volumeInputLabel.Text = "Volume";
            // 
            // pressureInputLabel
            // 
            this.pressureInputLabel.AutoSize = true;
            this.pressureInputLabel.Location = new System.Drawing.Point(12, 160);
            this.pressureInputLabel.Name = "pressureInputLabel";
            this.pressureInputLabel.Size = new System.Drawing.Size(97, 13);
            this.pressureInputLabel.TabIndex = 27;
            this.pressureInputLabel.Text = "Pressure (absolute)";
            // 
            // temperatureInputLabel
            // 
            this.temperatureInputLabel.AutoSize = true;
            this.temperatureInputLabel.Location = new System.Drawing.Point(12, 130);
            this.temperatureInputLabel.Name = "temperatureInputLabel";
            this.temperatureInputLabel.Size = new System.Drawing.Size(67, 13);
            this.temperatureInputLabel.TabIndex = 26;
            this.temperatureInputLabel.Text = "Temperature";
            // 
            // massInputLabel
            // 
            this.massInputLabel.AutoSize = true;
            this.massInputLabel.Location = new System.Drawing.Point(12, 216);
            this.massInputLabel.Name = "massInputLabel";
            this.massInputLabel.Size = new System.Drawing.Size(32, 13);
            this.massInputLabel.TabIndex = 36;
            this.massInputLabel.Text = "Mass";
            // 
            // MassInput
            // 
            this.MassInput.Location = new System.Drawing.Point(273, 213);
            this.MassInput.Name = "MassInput";
            this.MassInput.Size = new System.Drawing.Size(107, 20);
            this.MassInput.TabIndex = 38;
            this.MassInput.TextChanged += new System.EventHandler(this.MassInput_TextChanged);
            // 
            // PhaseSelector
            // 
            this.PhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PhaseSelector.FormattingEnabled = true;
            this.PhaseSelector.Location = new System.Drawing.Point(273, 96);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(107, 21);
            this.PhaseSelector.TabIndex = 54;
            this.PhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.PhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Location = new System.Drawing.Point(12, 99);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(61, 13);
            this.phaseLabel.TabIndex = 53;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.MassSelector);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.VolSelector);
            this.panel1.Controls.Add(this.TempSelector);
            this.panel1.Controls.Add(this.PresSelector);
            this.panel1.Controls.Add(this.warningLabel);
            this.panel1.Controls.Add(this.spinnerPictureBox);
            this.panel1.Controls.Add(this.phaseLabel);
            this.panel1.Controls.Add(this.PhaseSelector);
            this.panel1.Controls.Add(this.temperatureInputLabel);
            this.panel1.Controls.Add(this.pressureInputLabel);
            this.panel1.Controls.Add(this.MassInput);
            this.panel1.Controls.Add(this.volumeInputLabel);
            this.panel1.Controls.Add(this.MassUnitSelector);
            this.panel1.Controls.Add(this.TempUnitSelector);
            this.panel1.Controls.Add(this.massInputLabel);
            this.panel1.Controls.Add(this.PresUnitSelector);
            this.panel1.Controls.Add(this.SubmitBtn);
            this.panel1.Controls.Add(this.VolUnitSelector);
            this.panel1.Controls.Add(this.VolInput);
            this.panel1.Controls.Add(this.TempInput);
            this.panel1.Controls.Add(this.PresInput);
            this.panel1.Location = new System.Drawing.Point(0, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(8);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(984, 578);
            this.panel1.TabIndex = 55;
            // 
            // MassSelector
            // 
            this.MassSelector.AutoSize = true;
            this.MassSelector.Checked = true;
            this.MassSelector.Location = new System.Drawing.Point(273, 73);
            this.MassSelector.Name = "MassSelector";
            this.MassSelector.Size = new System.Drawing.Size(50, 17);
            this.MassSelector.TabIndex = 68;
            this.MassSelector.TabStop = true;
            this.MassSelector.Text = "Mass";
            this.MassSelector.UseVisualStyleBackColor = true;
            this.MassSelector.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(73, 13);
            this.label1.TabIndex = 67;
            this.label1.Text = "Select output:";
            // 
            // VolSelector
            // 
            this.VolSelector.AutoSize = true;
            this.VolSelector.Location = new System.Drawing.Point(273, 53);
            this.VolSelector.Name = "VolSelector";
            this.VolSelector.Size = new System.Drawing.Size(60, 17);
            this.VolSelector.TabIndex = 66;
            this.VolSelector.Text = "Volume";
            this.VolSelector.UseVisualStyleBackColor = true;
            this.VolSelector.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
            // 
            // TempSelector
            // 
            this.TempSelector.AutoSize = true;
            this.TempSelector.Location = new System.Drawing.Point(273, 17);
            this.TempSelector.Name = "TempSelector";
            this.TempSelector.Size = new System.Drawing.Size(85, 17);
            this.TempSelector.TabIndex = 64;
            this.TempSelector.Text = "Temperature";
            this.TempSelector.UseVisualStyleBackColor = true;
            this.TempSelector.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
            // 
            // PresSelector
            // 
            this.PresSelector.AutoSize = true;
            this.PresSelector.Location = new System.Drawing.Point(273, 35);
            this.PresSelector.Name = "PresSelector";
            this.PresSelector.Size = new System.Drawing.Size(66, 17);
            this.PresSelector.TabIndex = 65;
            this.PresSelector.Text = "Pressure";
            this.PresSelector.UseVisualStyleBackColor = true;
            this.PresSelector.CheckedChanged += new System.EventHandler(this.Output_CheckedChanged);
            // 
            // warningLabel
            // 
            this.warningLabel.AutoSize = true;
            this.warningLabel.BackColor = System.Drawing.Color.MistyRose;
            this.warningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningLabel.ForeColor = System.Drawing.Color.Maroon;
            this.warningLabel.Location = new System.Drawing.Point(12, 270);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Padding = new System.Windows.Forms.Padding(4);
            this.warningLabel.Size = new System.Drawing.Size(198, 23);
            this.warningLabel.TabIndex = 63;
            this.warningLabel.Text = "Warning/error message here";
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.warningLabel.Visible = false;
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(243, 239);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 62;
            this.spinnerPictureBox.TabStop = false;
            // 
            // MassUnitSelector
            // 
            this.MassUnitSelector.Converter = null;
            this.MassUnitSelector.Location = new System.Drawing.Point(115, 212);
            this.MassUnitSelector.Name = "MassUnitSelector";
            this.MassUnitSelector.SelectedItem = null;
            this.MassUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.MassUnitSelector.StoredValue = new double[0];
            this.MassUnitSelector.TabIndex = 37;
            this.MassUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.MassUnitSelector_OnSelectedIndexChanged);
            // 
            // TempUnitSelector
            // 
            this.TempUnitSelector.Converter = null;
            this.TempUnitSelector.Location = new System.Drawing.Point(115, 126);
            this.TempUnitSelector.Name = "TempUnitSelector";
            this.TempUnitSelector.SelectedItem = null;
            this.TempUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.TempUnitSelector.StoredValue = new double[0];
            this.TempUnitSelector.TabIndex = 29;
            this.TempUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.TempUnitSelector_OnSelectedIndexChange);
            // 
            // PresUnitSelector
            // 
            this.PresUnitSelector.Converter = null;
            this.PresUnitSelector.Location = new System.Drawing.Point(115, 156);
            this.PresUnitSelector.Name = "PresUnitSelector";
            this.PresUnitSelector.SelectedItem = null;
            this.PresUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.PresUnitSelector.StoredValue = new double[0];
            this.PresUnitSelector.TabIndex = 30;
            this.PresUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.PresUnitSelector_OnSelectedIndexChange);
            // 
            // VolUnitSelector
            // 
            this.VolUnitSelector.Converter = null;
            this.VolUnitSelector.Location = new System.Drawing.Point(115, 186);
            this.VolUnitSelector.Name = "VolUnitSelector";
            this.VolUnitSelector.SelectedItem = null;
            this.VolUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.VolUnitSelector.StoredValue = new double[0];
            this.VolUnitSelector.TabIndex = 31;
            this.VolUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.VolUnitSelector_OnSelectedIndexChange);
            // 
            // TankMassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "TankMassForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.TankMassForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button SubmitBtn;
		private System.Windows.Forms.TextBox VolInput;
		private System.Windows.Forms.TextBox PresInput;
		private System.Windows.Forms.TextBox TempInput;
		private ValueConverterDropdown VolUnitSelector;
		private ValueConverterDropdown PresUnitSelector;
		private ValueConverterDropdown TempUnitSelector;
		private System.Windows.Forms.Label volumeInputLabel;
		private System.Windows.Forms.Label pressureInputLabel;
		private System.Windows.Forms.Label temperatureInputLabel;
		private ValueConverterDropdown MassUnitSelector;
		private System.Windows.Forms.Label massInputLabel;
		private System.Windows.Forms.TextBox MassInput;
        private System.Windows.Forms.ComboBox PhaseSelector;
        private System.Windows.Forms.Label phaseLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
        private System.Windows.Forms.RadioButton MassSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton VolSelector;
        private System.Windows.Forms.RadioButton TempSelector;
        private System.Windows.Forms.RadioButton PresSelector;
    }
}
