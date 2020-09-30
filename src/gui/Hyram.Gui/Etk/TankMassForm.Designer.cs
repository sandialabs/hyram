namespace SandiaNationalLaboratories.Hyram {
	partial class TankMassForm {

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.calculateButton = new System.Windows.Forms.Button();
            this.volumeInput = new System.Windows.Forms.TextBox();
            this.pressureInput = new System.Windows.Forms.TextBox();
            this.temperatureInput = new System.Windows.Forms.TextBox();
            this.tankVolumeUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.pressureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.temperatureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.volumeInputLabel = new System.Windows.Forms.Label();
            this.pressureInputLabel = new System.Windows.Forms.Label();
            this.temperatureInputLabel = new System.Windows.Forms.Label();
            this.massUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.massInputLabel = new System.Windows.Forms.Label();
            this.massInput = new System.Windows.Forms.TextBox();
            this.fuelPhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // calculateButton
            // 
            this.calculateButton.Enabled = false;
            this.calculateButton.Location = new System.Drawing.Point(315, 133);
            this.calculateButton.Name = "calculateButton";
            this.calculateButton.Size = new System.Drawing.Size(107, 23);
            this.calculateButton.TabIndex = 35;
            this.calculateButton.Text = "Calculate Mass";
            this.calculateButton.UseVisualStyleBackColor = true;
            this.calculateButton.Click += new System.EventHandler(this.calculateButton_Click);
            // 
            // volumeInput
            // 
            this.volumeInput.Location = new System.Drawing.Point(315, 107);
            this.volumeInput.Name = "volumeInput";
            this.volumeInput.Size = new System.Drawing.Size(107, 20);
            this.volumeInput.TabIndex = 34;
            this.volumeInput.TextChanged += new System.EventHandler(this.volumeInput_TextChanged);
            // 
            // pressureInput
            // 
            this.pressureInput.Location = new System.Drawing.Point(315, 77);
            this.pressureInput.Name = "pressureInput";
            this.pressureInput.Size = new System.Drawing.Size(107, 20);
            this.pressureInput.TabIndex = 33;
            this.pressureInput.TextChanged += new System.EventHandler(this.pressureInput_TextChanged);
            // 
            // temperatureInput
            // 
            this.temperatureInput.Location = new System.Drawing.Point(315, 47);
            this.temperatureInput.Name = "temperatureInput";
            this.temperatureInput.Size = new System.Drawing.Size(107, 20);
            this.temperatureInput.TabIndex = 32;
            this.temperatureInput.TextChanged += new System.EventHandler(this.temperatureInput_TextChanged);
            // 
            // tankVolumeUnitSelector
            // 
            this.tankVolumeUnitSelector.Converter = null;
            this.tankVolumeUnitSelector.Location = new System.Drawing.Point(113, 105);
            this.tankVolumeUnitSelector.Name = "tankVolumeUnitSelector";
            this.tankVolumeUnitSelector.SelectedItem = null;
            this.tankVolumeUnitSelector.Size = new System.Drawing.Size(189, 22);
            this.tankVolumeUnitSelector.StoredValue = new double[0];
            this.tankVolumeUnitSelector.TabIndex = 31;
            this.tankVolumeUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.ddVolume_OnSelectedIndexChange);
            // 
            // pressureUnitSelector
            // 
            this.pressureUnitSelector.Converter = null;
            this.pressureUnitSelector.Location = new System.Drawing.Point(113, 75);
            this.pressureUnitSelector.Name = "pressureUnitSelector";
            this.pressureUnitSelector.SelectedItem = null;
            this.pressureUnitSelector.Size = new System.Drawing.Size(189, 22);
            this.pressureUnitSelector.StoredValue = new double[0];
            this.pressureUnitSelector.TabIndex = 30;
            this.pressureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.pressureUnitSelector_OnSelectedIndexChange);
            // 
            // temperatureUnitSelector
            // 
            this.temperatureUnitSelector.Converter = null;
            this.temperatureUnitSelector.Location = new System.Drawing.Point(113, 45);
            this.temperatureUnitSelector.Name = "temperatureUnitSelector";
            this.temperatureUnitSelector.SelectedItem = null;
            this.temperatureUnitSelector.Size = new System.Drawing.Size(189, 22);
            this.temperatureUnitSelector.StoredValue = new double[0];
            this.temperatureUnitSelector.TabIndex = 29;
            this.temperatureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.temperatureUnitSelector_OnSelectedIndexChange);
            // 
            // volumeInputLabel
            // 
            this.volumeInputLabel.AutoSize = true;
            this.volumeInputLabel.Location = new System.Drawing.Point(23, 110);
            this.volumeInputLabel.Name = "volumeInputLabel";
            this.volumeInputLabel.Size = new System.Drawing.Size(42, 13);
            this.volumeInputLabel.TabIndex = 28;
            this.volumeInputLabel.Text = "Volume";
            // 
            // pressureInputLabel
            // 
            this.pressureInputLabel.AutoSize = true;
            this.pressureInputLabel.Location = new System.Drawing.Point(23, 80);
            this.pressureInputLabel.Name = "pressureInputLabel";
            this.pressureInputLabel.Size = new System.Drawing.Size(48, 13);
            this.pressureInputLabel.TabIndex = 27;
            this.pressureInputLabel.Text = "Pressure";
            // 
            // temperatureInputLabel
            // 
            this.temperatureInputLabel.AutoSize = true;
            this.temperatureInputLabel.Location = new System.Drawing.Point(23, 49);
            this.temperatureInputLabel.Name = "temperatureInputLabel";
            this.temperatureInputLabel.Size = new System.Drawing.Size(67, 13);
            this.temperatureInputLabel.TabIndex = 26;
            this.temperatureInputLabel.Text = "Temperature";
            // 
            // massUnitSelector
            // 
            this.massUnitSelector.Converter = null;
            this.massUnitSelector.Location = new System.Drawing.Point(113, 167);
            this.massUnitSelector.Name = "massUnitSelector";
            this.massUnitSelector.SelectedItem = null;
            this.massUnitSelector.Size = new System.Drawing.Size(189, 22);
            this.massUnitSelector.StoredValue = new double[0];
            this.massUnitSelector.TabIndex = 37;
            this.massUnitSelector.Visible = false;
            this.massUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.massUnitSelector_OnSelectedIndexChanged);
            // 
            // massInputLabel
            // 
            this.massInputLabel.AutoSize = true;
            this.massInputLabel.Location = new System.Drawing.Point(23, 172);
            this.massInputLabel.Name = "massInputLabel";
            this.massInputLabel.Size = new System.Drawing.Size(32, 13);
            this.massInputLabel.TabIndex = 36;
            this.massInputLabel.Text = "Mass";
            this.massInputLabel.Visible = false;
            // 
            // massInput
            // 
            this.massInput.Enabled = false;
            this.massInput.Location = new System.Drawing.Point(315, 169);
            this.massInput.Name = "massInput";
            this.massInput.Size = new System.Drawing.Size(107, 20);
            this.massInput.TabIndex = 38;
            this.massInput.Visible = false;
            this.massInput.TextChanged += new System.EventHandler(this.massInput_TextChanged);
            // 
            // fuelPhaseSelector
            // 
            this.fuelPhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fuelPhaseSelector.FormattingEnabled = true;
            this.fuelPhaseSelector.Location = new System.Drawing.Point(315, 16);
            this.fuelPhaseSelector.Name = "fuelPhaseSelector";
            this.fuelPhaseSelector.Size = new System.Drawing.Size(107, 21);
            this.fuelPhaseSelector.TabIndex = 54;
            this.fuelPhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.fuelPhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Location = new System.Drawing.Point(23, 19);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(61, 13);
            this.phaseLabel.TabIndex = 53;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // TankMassForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.fuelPhaseSelector);
            this.Controls.Add(this.phaseLabel);
            this.Controls.Add(this.massInput);
            this.Controls.Add(this.massUnitSelector);
            this.Controls.Add(this.massInputLabel);
            this.Controls.Add(this.calculateButton);
            this.Controls.Add(this.volumeInput);
            this.Controls.Add(this.pressureInput);
            this.Controls.Add(this.temperatureInput);
            this.Controls.Add(this.tankVolumeUnitSelector);
            this.Controls.Add(this.pressureUnitSelector);
            this.Controls.Add(this.temperatureUnitSelector);
            this.Controls.Add(this.volumeInputLabel);
            this.Controls.Add(this.pressureInputLabel);
            this.Controls.Add(this.temperatureInputLabel);
            this.Name = "TankMassForm";
            this.Size = new System.Drawing.Size(447, 216);
            this.Load += new System.EventHandler(this.cpEtkTankMass_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button calculateButton;
		private System.Windows.Forms.TextBox volumeInput;
		private System.Windows.Forms.TextBox pressureInput;
		private System.Windows.Forms.TextBox temperatureInput;
		private ValueConverterDropdown tankVolumeUnitSelector;
		private ValueConverterDropdown pressureUnitSelector;
		private ValueConverterDropdown temperatureUnitSelector;
		private System.Windows.Forms.Label volumeInputLabel;
		private System.Windows.Forms.Label pressureInputLabel;
		private System.Windows.Forms.Label temperatureInputLabel;
		private ValueConverterDropdown massUnitSelector;
		private System.Windows.Forms.Label massInputLabel;
		private System.Windows.Forms.TextBox massInput;
        private System.Windows.Forms.ComboBox fuelPhaseSelector;
        private System.Windows.Forms.Label phaseLabel;
    }
}
