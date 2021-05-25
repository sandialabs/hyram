namespace SandiaNationalLaboratories.Hyram {
	partial class MassFlowRateForm {
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
            this.volumeInput = new System.Windows.Forms.TextBox();
            this.pressureInput = new System.Windows.Forms.TextBox();
            this.temperatureInput = new System.Windows.Forms.TextBox();
            this.volumeInputLabel = new System.Windows.Forms.Label();
            this.pressureInputLabel = new System.Windows.Forms.Label();
            this.temperatureInputLabel = new System.Windows.Forms.Label();
            this.calculateButton = new System.Windows.Forms.Button();
            this.orificeDiameterInput = new System.Windows.Forms.TextBox();
            this.orificeDiameterLabel = new System.Windows.Forms.Label();
            this.isSteadySelector = new System.Windows.Forms.RadioButton();
            this.isBlowdownSelector = new System.Windows.Forms.RadioButton();
            this.releaseTypeLabel = new System.Windows.Forms.Label();
            this.resultOutput = new System.Windows.Forms.TextBox();
            this.resultLabel = new System.Windows.Forms.Label();
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.inputTab = new System.Windows.Forms.TabPage();
            this.fuelPhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.pressureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.temperatureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.tankVolumeUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.orificeDiameterUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.resultImagePicture = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.mainTabControl.SuspendLayout();
            this.inputTab.SuspendLayout();
            this.outputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultImagePicture)).BeginInit();
            this.SuspendLayout();
            // 
            // volumeInput
            // 
            this.volumeInput.Location = new System.Drawing.Point(314, 100);
            this.volumeInput.Name = "volumeInput";
            this.volumeInput.Size = new System.Drawing.Size(107, 20);
            this.volumeInput.TabIndex = 43;
            this.volumeInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.volumeInput.TextChanged += new System.EventHandler(this.volumeInput_TextChanged);
            // 
            // pressureInput
            // 
            this.pressureInput.Location = new System.Drawing.Point(314, 70);
            this.pressureInput.Name = "pressureInput";
            this.pressureInput.Size = new System.Drawing.Size(107, 20);
            this.pressureInput.TabIndex = 42;
            this.pressureInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.pressureInput.TextChanged += new System.EventHandler(this.pressureInput_TextChanged);
            // 
            // temperatureInput
            // 
            this.temperatureInput.Location = new System.Drawing.Point(314, 41);
            this.temperatureInput.Name = "temperatureInput";
            this.temperatureInput.Size = new System.Drawing.Size(107, 20);
            this.temperatureInput.TabIndex = 41;
            this.temperatureInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.temperatureInput.TextChanged += new System.EventHandler(this.temperatureInput_TextChanged);
            // 
            // volumeInputLabel
            // 
            this.volumeInputLabel.AutoSize = true;
            this.volumeInputLabel.Location = new System.Drawing.Point(19, 104);
            this.volumeInputLabel.Name = "volumeInputLabel";
            this.volumeInputLabel.Size = new System.Drawing.Size(42, 13);
            this.volumeInputLabel.TabIndex = 37;
            this.volumeInputLabel.Text = "Volume";
            // 
            // pressureInputLabel
            // 
            this.pressureInputLabel.AutoSize = true;
            this.pressureInputLabel.Location = new System.Drawing.Point(19, 74);
            this.pressureInputLabel.Name = "pressureInputLabel";
            this.pressureInputLabel.Size = new System.Drawing.Size(97, 13);
            this.pressureInputLabel.TabIndex = 36;
            this.pressureInputLabel.Text = "Pressure (absolute)";
            // 
            // temperatureInputLabel
            // 
            this.temperatureInputLabel.AutoSize = true;
            this.temperatureInputLabel.Location = new System.Drawing.Point(19, 45);
            this.temperatureInputLabel.Name = "temperatureInputLabel";
            this.temperatureInputLabel.Size = new System.Drawing.Size(67, 13);
            this.temperatureInputLabel.TabIndex = 35;
            this.temperatureInputLabel.Text = "Temperature";
            // 
            // calculateButton
            // 
            this.calculateButton.Enabled = false;
            this.calculateButton.Location = new System.Drawing.Point(314, 166);
            this.calculateButton.Name = "calculateButton";
            this.calculateButton.Size = new System.Drawing.Size(107, 23);
            this.calculateButton.TabIndex = 44;
            this.calculateButton.Text = "Calculate Mass Flow Rate";
            this.calculateButton.UseVisualStyleBackColor = true;
            this.calculateButton.Click += new System.EventHandler(this.calculateButton_Click);
            this.calculateButton.MouseMove += new System.Windows.Forms.MouseEventHandler(this.calculateButton_MouseMove);
            // 
            // orificeDiameterInput
            // 
            this.orificeDiameterInput.Location = new System.Drawing.Point(314, 129);
            this.orificeDiameterInput.Name = "orificeDiameterInput";
            this.orificeDiameterInput.Size = new System.Drawing.Size(107, 20);
            this.orificeDiameterInput.TabIndex = 47;
            this.orificeDiameterInput.Text = "3.0e-2";
            this.orificeDiameterInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.orificeDiameterInput.TextChanged += new System.EventHandler(this.orificeDiameterInput_TextChanged);
            // 
            // orificeDiameterLabel
            // 
            this.orificeDiameterLabel.AutoSize = true;
            this.orificeDiameterLabel.Location = new System.Drawing.Point(19, 133);
            this.orificeDiameterLabel.Name = "orificeDiameterLabel";
            this.orificeDiameterLabel.Size = new System.Drawing.Size(82, 13);
            this.orificeDiameterLabel.TabIndex = 45;
            this.orificeDiameterLabel.Text = "Orifice Diameter";
            // 
            // isSteadySelector
            // 
            this.isSteadySelector.AutoSize = true;
            this.isSteadySelector.Location = new System.Drawing.Point(123, 169);
            this.isSteadySelector.Name = "isSteadySelector";
            this.isSteadySelector.Size = new System.Drawing.Size(58, 17);
            this.isSteadySelector.TabIndex = 48;
            this.isSteadySelector.Text = "Steady";
            this.isSteadySelector.UseVisualStyleBackColor = true;
            this.isSteadySelector.CheckedChanged += new System.EventHandler(this.ReleaseTypeChanged);
            // 
            // isBlowdownSelector
            // 
            this.isBlowdownSelector.AutoSize = true;
            this.isBlowdownSelector.Checked = true;
            this.isBlowdownSelector.Location = new System.Drawing.Point(185, 169);
            this.isBlowdownSelector.Name = "isBlowdownSelector";
            this.isBlowdownSelector.Size = new System.Drawing.Size(74, 17);
            this.isBlowdownSelector.TabIndex = 49;
            this.isBlowdownSelector.TabStop = true;
            this.isBlowdownSelector.Text = "Blowdown";
            this.isBlowdownSelector.UseVisualStyleBackColor = true;
            this.isBlowdownSelector.CheckedChanged += new System.EventHandler(this.ReleaseTypeChanged);
            // 
            // releaseTypeLabel
            // 
            this.releaseTypeLabel.AutoSize = true;
            this.releaseTypeLabel.Location = new System.Drawing.Point(19, 171);
            this.releaseTypeLabel.Name = "releaseTypeLabel";
            this.releaseTypeLabel.Size = new System.Drawing.Size(73, 13);
            this.releaseTypeLabel.TabIndex = 50;
            this.releaseTypeLabel.Text = "Release Type";
            // 
            // resultOutput
            // 
            this.resultOutput.Enabled = false;
            this.resultOutput.Location = new System.Drawing.Point(142, 10);
            this.resultOutput.Name = "resultOutput";
            this.resultOutput.Size = new System.Drawing.Size(107, 20);
            this.resultOutput.TabIndex = 53;
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Location = new System.Drawing.Point(16, 13);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(114, 13);
            this.resultLabel.TabIndex = 51;
            this.resultLabel.Text = "Mass Flow Rate (kg/s)";
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.inputTab);
            this.mainTabControl.Controls.Add(this.outputTab);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(522, 415);
            this.mainTabControl.TabIndex = 54;
            // 
            // inputTab
            // 
            this.inputTab.Controls.Add(this.fuelPhaseSelector);
            this.inputTab.Controls.Add(this.phaseLabel);
            this.inputTab.Controls.Add(this.pressureUnitSelector);
            this.inputTab.Controls.Add(this.temperatureInputLabel);
            this.inputTab.Controls.Add(this.pressureInputLabel);
            this.inputTab.Controls.Add(this.releaseTypeLabel);
            this.inputTab.Controls.Add(this.volumeInputLabel);
            this.inputTab.Controls.Add(this.isSteadySelector);
            this.inputTab.Controls.Add(this.temperatureUnitSelector);
            this.inputTab.Controls.Add(this.isBlowdownSelector);
            this.inputTab.Controls.Add(this.tankVolumeUnitSelector);
            this.inputTab.Controls.Add(this.orificeDiameterInput);
            this.inputTab.Controls.Add(this.temperatureInput);
            this.inputTab.Controls.Add(this.orificeDiameterUnitSelector);
            this.inputTab.Controls.Add(this.pressureInput);
            this.inputTab.Controls.Add(this.orificeDiameterLabel);
            this.inputTab.Controls.Add(this.volumeInput);
            this.inputTab.Controls.Add(this.calculateButton);
            this.inputTab.Location = new System.Drawing.Point(4, 22);
            this.inputTab.Name = "inputTab";
            this.inputTab.Padding = new System.Windows.Forms.Padding(3);
            this.inputTab.Size = new System.Drawing.Size(514, 389);
            this.inputTab.TabIndex = 0;
            this.inputTab.Text = "Input";
            this.inputTab.UseVisualStyleBackColor = true;
            this.inputTab.MouseMove += new System.Windows.Forms.MouseEventHandler(this.inputTab_MouseMove);
            // 
            // fuelPhaseSelector
            // 
            this.fuelPhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fuelPhaseSelector.FormattingEnabled = true;
            this.fuelPhaseSelector.Location = new System.Drawing.Point(314, 11);
            this.fuelPhaseSelector.Name = "fuelPhaseSelector";
            this.fuelPhaseSelector.Size = new System.Drawing.Size(107, 21);
            this.fuelPhaseSelector.TabIndex = 52;
            this.fuelPhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.fuelPhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Location = new System.Drawing.Point(19, 14);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(61, 13);
            this.phaseLabel.TabIndex = 51;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // pressureUnitSelector
            // 
            this.pressureUnitSelector.Converter = null;
            this.pressureUnitSelector.Location = new System.Drawing.Point(123, 69);
            this.pressureUnitSelector.Margin = new System.Windows.Forms.Padding(4);
            this.pressureUnitSelector.Name = "pressureUnitSelector";
            this.pressureUnitSelector.SelectedItem = null;
            this.pressureUnitSelector.Size = new System.Drawing.Size(178, 22);
            this.pressureUnitSelector.StoredValue = new double[0];
            this.pressureUnitSelector.TabIndex = 39;
            this.pressureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.pressureUnitSelector_OnSelectedIndexChange);
            // 
            // temperatureUnitSelector
            // 
            this.temperatureUnitSelector.Converter = null;
            this.temperatureUnitSelector.Location = new System.Drawing.Point(123, 40);
            this.temperatureUnitSelector.Margin = new System.Windows.Forms.Padding(4);
            this.temperatureUnitSelector.Name = "temperatureUnitSelector";
            this.temperatureUnitSelector.SelectedItem = null;
            this.temperatureUnitSelector.Size = new System.Drawing.Size(178, 22);
            this.temperatureUnitSelector.StoredValue = new double[0];
            this.temperatureUnitSelector.TabIndex = 38;
            this.temperatureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.temperatureUnitSelector_OnSelectedIndexChange);
            // 
            // tankVolumeUnitSelector
            // 
            this.tankVolumeUnitSelector.Converter = null;
            this.tankVolumeUnitSelector.Location = new System.Drawing.Point(123, 99);
            this.tankVolumeUnitSelector.Margin = new System.Windows.Forms.Padding(4);
            this.tankVolumeUnitSelector.Name = "tankVolumeUnitSelector";
            this.tankVolumeUnitSelector.SelectedItem = null;
            this.tankVolumeUnitSelector.Size = new System.Drawing.Size(178, 22);
            this.tankVolumeUnitSelector.StoredValue = new double[0];
            this.tankVolumeUnitSelector.TabIndex = 40;
            this.tankVolumeUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.ddVolume_OnSelectedIndexChange);
            // 
            // orificeDiameterUnitSelector
            // 
            this.orificeDiameterUnitSelector.Converter = null;
            this.orificeDiameterUnitSelector.Location = new System.Drawing.Point(123, 128);
            this.orificeDiameterUnitSelector.Margin = new System.Windows.Forms.Padding(4);
            this.orificeDiameterUnitSelector.Name = "orificeDiameterUnitSelector";
            this.orificeDiameterUnitSelector.SelectedItem = null;
            this.orificeDiameterUnitSelector.Size = new System.Drawing.Size(178, 22);
            this.orificeDiameterUnitSelector.StoredValue = new double[0];
            this.orificeDiameterUnitSelector.TabIndex = 46;
            this.orificeDiameterUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.orificeDiameterUnitSelector_OnSelectedIndexChanged);
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.resultImagePicture);
            this.outputTab.Controls.Add(this.resultLabel);
            this.outputTab.Controls.Add(this.resultOutput);
            this.outputTab.Location = new System.Drawing.Point(4, 22);
            this.outputTab.Name = "outputTab";
            this.outputTab.Padding = new System.Windows.Forms.Padding(3);
            this.outputTab.Size = new System.Drawing.Size(514, 389);
            this.outputTab.TabIndex = 1;
            this.outputTab.Text = "Output";
            this.outputTab.UseVisualStyleBackColor = true;
            this.outputTab.Enter += new System.EventHandler(this.outputTab_Enter);
            // 
            // resultImagePicture
            // 
            this.resultImagePicture.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.resultImagePicture.Location = new System.Drawing.Point(7, 36);
            this.resultImagePicture.Name = "resultImagePicture";
            this.resultImagePicture.Size = new System.Drawing.Size(501, 347);
            this.resultImagePicture.TabIndex = 54;
            this.resultImagePicture.TabStop = false;
            // 
            // MassFlowRateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTabControl);
            this.Name = "MassFlowRateForm";
            this.Size = new System.Drawing.Size(522, 415);
            this.Load += new System.EventHandler(this.MassFlowRateForm_Load);
            this.mainTabControl.ResumeLayout(false);
            this.inputTab.ResumeLayout(false);
            this.inputTab.PerformLayout();
            this.outputTab.ResumeLayout(false);
            this.outputTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.resultImagePicture)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox volumeInput;
		private System.Windows.Forms.TextBox pressureInput;
		private System.Windows.Forms.TextBox temperatureInput;
		private ValueConverterDropdown tankVolumeUnitSelector;
		private ValueConverterDropdown pressureUnitSelector;
		private ValueConverterDropdown temperatureUnitSelector;
		private System.Windows.Forms.Label volumeInputLabel;
		private System.Windows.Forms.Label pressureInputLabel;
		private System.Windows.Forms.Label temperatureInputLabel;
		private System.Windows.Forms.Button calculateButton;
		private System.Windows.Forms.TextBox orificeDiameterInput;
		private ValueConverterDropdown orificeDiameterUnitSelector;
		private System.Windows.Forms.Label orificeDiameterLabel;
		private System.Windows.Forms.RadioButton isSteadySelector;
		private System.Windows.Forms.RadioButton isBlowdownSelector;
		private System.Windows.Forms.Label releaseTypeLabel;
		private System.Windows.Forms.TextBox resultOutput;
		private System.Windows.Forms.Label resultLabel;
		private System.Windows.Forms.TabControl mainTabControl;
		private System.Windows.Forms.TabPage inputTab;
		private System.Windows.Forms.TabPage outputTab;
		private Hyram.PictureBoxWithSave resultImagePicture;
        private System.Windows.Forms.Label phaseLabel;
        private System.Windows.Forms.ComboBox fuelPhaseSelector;
    }
}
