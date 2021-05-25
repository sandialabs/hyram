namespace SandiaNationalLaboratories.Hyram {
	partial class TemperaturePressureDensityForm {

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.outputSelectorGroupBox = new System.Windows.Forms.GroupBox();
            this.temperatureSelector = new System.Windows.Forms.RadioButton();
            this.densitySelector = new System.Windows.Forms.RadioButton();
            this.pressureSelector = new System.Windows.Forms.RadioButton();
            this.submitButton = new System.Windows.Forms.Button();
            this.densityInput = new System.Windows.Forms.TextBox();
            this.pressureInput = new System.Windows.Forms.TextBox();
            this.temperatureInput = new System.Windows.Forms.TextBox();
            this.densityInputLabel = new System.Windows.Forms.Label();
            this.pressureInputLabel = new System.Windows.Forms.Label();
            this.temperatureInputLabel = new System.Windows.Forms.Label();
            this.densityUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.pressureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.temperatureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // outputSelectorGroupBox
            // 
            this.outputSelectorGroupBox.Location = new System.Drawing.Point(14, 215);
            this.outputSelectorGroupBox.Name = "outputSelectorGroupBox";
            this.outputSelectorGroupBox.Size = new System.Drawing.Size(98, 77);
            this.outputSelectorGroupBox.TabIndex = 24;
            this.outputSelectorGroupBox.TabStop = false;
            this.outputSelectorGroupBox.Text = "Calculate...";
            this.outputSelectorGroupBox.Visible = false;
            // 
            // temperatureSelector
            // 
            this.temperatureSelector.AutoSize = true;
            this.temperatureSelector.Checked = true;
            this.temperatureSelector.Location = new System.Drawing.Point(124, 15);
            this.temperatureSelector.Name = "temperatureSelector";
            this.temperatureSelector.Size = new System.Drawing.Size(85, 17);
            this.temperatureSelector.TabIndex = 10;
            this.temperatureSelector.TabStop = true;
            this.temperatureSelector.Text = "Temperature";
            this.temperatureSelector.UseVisualStyleBackColor = true;
            this.temperatureSelector.CheckedChanged += new System.EventHandler(this.CalcOptionRbCheckedChanged);
            // 
            // densitySelector
            // 
            this.densitySelector.AutoSize = true;
            this.densitySelector.Location = new System.Drawing.Point(124, 51);
            this.densitySelector.Name = "densitySelector";
            this.densitySelector.Size = new System.Drawing.Size(60, 17);
            this.densitySelector.TabIndex = 12;
            this.densitySelector.Text = "Density";
            this.densitySelector.UseVisualStyleBackColor = true;
            this.densitySelector.CheckedChanged += new System.EventHandler(this.CalcOptionRbCheckedChanged);
            // 
            // pressureSelector
            // 
            this.pressureSelector.AutoSize = true;
            this.pressureSelector.Location = new System.Drawing.Point(124, 33);
            this.pressureSelector.Name = "pressureSelector";
            this.pressureSelector.Size = new System.Drawing.Size(66, 17);
            this.pressureSelector.TabIndex = 11;
            this.pressureSelector.Text = "Pressure";
            this.pressureSelector.UseVisualStyleBackColor = true;
            this.pressureSelector.CheckedChanged += new System.EventHandler(this.CalcOptionRbCheckedChanged);
            // 
            // submitButton
            // 
            this.submitButton.Enabled = false;
            this.submitButton.Location = new System.Drawing.Point(287, 161);
            this.submitButton.Name = "submitButton";
            this.submitButton.Size = new System.Drawing.Size(137, 23);
            this.submitButton.TabIndex = 25;
            this.submitButton.UseVisualStyleBackColor = true;
            this.submitButton.Click += new System.EventHandler(this.calculateButton_Click);
            // 
            // densityInput
            // 
            this.densityInput.Enabled = false;
            this.densityInput.Location = new System.Drawing.Point(317, 132);
            this.densityInput.Name = "densityInput";
            this.densityInput.Size = new System.Drawing.Size(107, 20);
            this.densityInput.TabIndex = 23;
            this.densityInput.TextChanged += new System.EventHandler(this.tbDensity_TextChanged);
            // 
            // pressureInput
            // 
            this.pressureInput.Enabled = false;
            this.pressureInput.Location = new System.Drawing.Point(317, 103);
            this.pressureInput.Name = "pressureInput";
            this.pressureInput.Size = new System.Drawing.Size(107, 20);
            this.pressureInput.TabIndex = 22;
            this.pressureInput.TextChanged += new System.EventHandler(this.pressureInput_TextChanged);
            // 
            // temperatureInput
            // 
            this.temperatureInput.Enabled = false;
            this.temperatureInput.Location = new System.Drawing.Point(317, 75);
            this.temperatureInput.Name = "temperatureInput";
            this.temperatureInput.Size = new System.Drawing.Size(107, 20);
            this.temperatureInput.TabIndex = 21;
            this.temperatureInput.TextChanged += new System.EventHandler(this.temperatureInput_TextChanged);
            // 
            // densityInputLabel
            // 
            this.densityInputLabel.AutoSize = true;
            this.densityInputLabel.Location = new System.Drawing.Point(22, 135);
            this.densityInputLabel.Name = "densityInputLabel";
            this.densityInputLabel.Size = new System.Drawing.Size(42, 13);
            this.densityInputLabel.TabIndex = 17;
            this.densityInputLabel.Text = "Density";
            // 
            // pressureInputLabel
            // 
            this.pressureInputLabel.AutoSize = true;
            this.pressureInputLabel.Location = new System.Drawing.Point(22, 106);
            this.pressureInputLabel.Name = "pressureInputLabel";
            this.pressureInputLabel.Size = new System.Drawing.Size(97, 13);
            this.pressureInputLabel.TabIndex = 16;
            this.pressureInputLabel.Text = "Pressure (absolute)";
            // 
            // temperatureInputLabel
            // 
            this.temperatureInputLabel.AutoSize = true;
            this.temperatureInputLabel.Location = new System.Drawing.Point(22, 78);
            this.temperatureInputLabel.Name = "temperatureInputLabel";
            this.temperatureInputLabel.Size = new System.Drawing.Size(67, 13);
            this.temperatureInputLabel.TabIndex = 15;
            this.temperatureInputLabel.Text = "Temperature";
            // 
            // densityUnitSelector
            // 
            this.densityUnitSelector.Converter = null;
            this.densityUnitSelector.Location = new System.Drawing.Point(124, 130);
            this.densityUnitSelector.Name = "densityUnitSelector";
            this.densityUnitSelector.SelectedItem = null;
            this.densityUnitSelector.Size = new System.Drawing.Size(178, 22);
            this.densityUnitSelector.StoredValue = new double[0];
            this.densityUnitSelector.TabIndex = 20;
            this.densityUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.densityUnitSelector_OnSelectedIndexChange);
            // 
            // pressureUnitSelector
            // 
            this.pressureUnitSelector.Converter = null;
            this.pressureUnitSelector.Location = new System.Drawing.Point(124, 102);
            this.pressureUnitSelector.Name = "pressureUnitSelector";
            this.pressureUnitSelector.SelectedItem = null;
            this.pressureUnitSelector.Size = new System.Drawing.Size(178, 22);
            this.pressureUnitSelector.StoredValue = new double[0];
            this.pressureUnitSelector.TabIndex = 19;
            this.pressureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.pressureUnitSelector_OnSelectedIndexChange);
            // 
            // temperatureUnitSelector
            // 
            this.temperatureUnitSelector.Converter = null;
            this.temperatureUnitSelector.Location = new System.Drawing.Point(124, 74);
            this.temperatureUnitSelector.Name = "temperatureUnitSelector";
            this.temperatureUnitSelector.SelectedItem = null;
            this.temperatureUnitSelector.Size = new System.Drawing.Size(178, 22);
            this.temperatureUnitSelector.StoredValue = new double[0];
            this.temperatureUnitSelector.TabIndex = 18;
            this.temperatureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.temperatureUnitSelector_OnSelectedIndexChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 26;
            this.label1.Text = "Output";
            // 
            // TemperaturePressureDensityForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.densitySelector);
            this.Controls.Add(this.temperatureSelector);
            this.Controls.Add(this.pressureSelector);
            this.Controls.Add(this.outputSelectorGroupBox);
            this.Controls.Add(this.submitButton);
            this.Controls.Add(this.densityInput);
            this.Controls.Add(this.pressureInput);
            this.Controls.Add(this.temperatureInput);
            this.Controls.Add(this.densityUnitSelector);
            this.Controls.Add(this.pressureUnitSelector);
            this.Controls.Add(this.temperatureUnitSelector);
            this.Controls.Add(this.densityInputLabel);
            this.Controls.Add(this.pressureInputLabel);
            this.Controls.Add(this.temperatureInputLabel);
            this.Name = "TemperaturePressureDensityForm";
            this.Size = new System.Drawing.Size(525, 224);
            this.Load += new System.EventHandler(this.cpEtkTempPressureDensity_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox outputSelectorGroupBox;
		private System.Windows.Forms.RadioButton temperatureSelector;
		private System.Windows.Forms.RadioButton densitySelector;
		private System.Windows.Forms.RadioButton pressureSelector;
		private System.Windows.Forms.Button submitButton;
		private System.Windows.Forms.TextBox densityInput;
		private System.Windows.Forms.TextBox pressureInput;
		private System.Windows.Forms.TextBox temperatureInput;
		private ValueConverterDropdown densityUnitSelector;
		private ValueConverterDropdown pressureUnitSelector;
		private ValueConverterDropdown temperatureUnitSelector;
		private System.Windows.Forms.Label densityInputLabel;
		private System.Windows.Forms.Label pressureInputLabel;
		private System.Windows.Forms.Label temperatureInputLabel;
        private System.Windows.Forms.Label label1;
    }
}
