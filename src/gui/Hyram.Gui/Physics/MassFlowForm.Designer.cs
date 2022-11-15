namespace SandiaNationalLaboratories.Hyram {
	partial class MassFlowForm {
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
            this.VolumeInput = new System.Windows.Forms.TextBox();
            this.PressureInput = new System.Windows.Forms.TextBox();
            this.TemperatureInput = new System.Windows.Forms.TextBox();
            this.volumeInputLabel = new System.Windows.Forms.Label();
            this.pressureInputLabel = new System.Windows.Forms.Label();
            this.temperatureInputLabel = new System.Windows.Forms.Label();
            this.submitBtn = new System.Windows.Forms.Button();
            this.OrificeDiameterInput = new System.Windows.Forms.TextBox();
            this.orificeDiameterLabel = new System.Windows.Forms.Label();
            this.SteadySelector = new System.Windows.Forms.RadioButton();
            this.BlowdownSelector = new System.Windows.Forms.RadioButton();
            this.releaseTypeLabel = new System.Windows.Forms.Label();
            this.DcInput = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.warningMsg = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.outputLabel = new System.Windows.Forms.Label();
            this.MassFlowTextbox = new System.Windows.Forms.TextBox();
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AmbPresInput = new System.Windows.Forms.TextBox();
            this.AmbPresUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.label2 = new System.Windows.Forms.Label();
            this.PlotBox = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.OrificeDiameterUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.TankVolumeUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.PressureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.TemperatureUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PlotBox)).BeginInit();
            this.SuspendLayout();
            // 
            // VolumeInput
            // 
            this.VolumeInput.Location = new System.Drawing.Point(273, 127);
            this.VolumeInput.Name = "VolumeInput";
            this.VolumeInput.Size = new System.Drawing.Size(107, 20);
            this.VolumeInput.TabIndex = 43;
            this.VolumeInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.VolumeInput.TextChanged += new System.EventHandler(this.VolumeInput_TextChanged);
            // 
            // PressureInput
            // 
            this.PressureInput.Location = new System.Drawing.Point(273, 67);
            this.PressureInput.Name = "PressureInput";
            this.PressureInput.Size = new System.Drawing.Size(107, 20);
            this.PressureInput.TabIndex = 42;
            this.PressureInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.PressureInput.TextChanged += new System.EventHandler(this.PressureInput_TextChanged);
            // 
            // TemperatureInput
            // 
            this.TemperatureInput.Location = new System.Drawing.Point(273, 38);
            this.TemperatureInput.Name = "TemperatureInput";
            this.TemperatureInput.Size = new System.Drawing.Size(107, 20);
            this.TemperatureInput.TabIndex = 41;
            this.TemperatureInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.TemperatureInput.TextChanged += new System.EventHandler(this.TemperatureInput_TextChanged);
            // 
            // volumeInputLabel
            // 
            this.volumeInputLabel.AutoSize = true;
            this.volumeInputLabel.Location = new System.Drawing.Point(8, 131);
            this.volumeInputLabel.Name = "volumeInputLabel";
            this.volumeInputLabel.Size = new System.Drawing.Size(42, 13);
            this.volumeInputLabel.TabIndex = 37;
            this.volumeInputLabel.Text = "Volume";
            // 
            // pressureInputLabel
            // 
            this.pressureInputLabel.AutoSize = true;
            this.pressureInputLabel.Location = new System.Drawing.Point(8, 71);
            this.pressureInputLabel.Name = "pressureInputLabel";
            this.pressureInputLabel.Size = new System.Drawing.Size(97, 13);
            this.pressureInputLabel.TabIndex = 36;
            this.pressureInputLabel.Text = "Pressure (absolute)";
            // 
            // temperatureInputLabel
            // 
            this.temperatureInputLabel.AutoSize = true;
            this.temperatureInputLabel.Location = new System.Drawing.Point(8, 42);
            this.temperatureInputLabel.Name = "temperatureInputLabel";
            this.temperatureInputLabel.Size = new System.Drawing.Size(67, 13);
            this.temperatureInputLabel.TabIndex = 35;
            this.temperatureInputLabel.Text = "Temperature";
            // 
            // submitBtn
            // 
            this.submitBtn.Enabled = false;
            this.submitBtn.Location = new System.Drawing.Point(273, 211);
            this.submitBtn.Name = "submitBtn";
            this.submitBtn.Size = new System.Drawing.Size(107, 23);
            this.submitBtn.TabIndex = 44;
            this.submitBtn.Text = "Calculate Mass Flow Rate";
            this.submitBtn.UseVisualStyleBackColor = true;
            this.submitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // OrificeDiameterInput
            // 
            this.OrificeDiameterInput.Location = new System.Drawing.Point(273, 156);
            this.OrificeDiameterInput.Name = "OrificeDiameterInput";
            this.OrificeDiameterInput.Size = new System.Drawing.Size(107, 20);
            this.OrificeDiameterInput.TabIndex = 47;
            this.OrificeDiameterInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.OrificeDiameterInput.TextChanged += new System.EventHandler(this.OrificeDiameterInput_TextChanged);
            // 
            // orificeDiameterLabel
            // 
            this.orificeDiameterLabel.AutoSize = true;
            this.orificeDiameterLabel.Location = new System.Drawing.Point(8, 160);
            this.orificeDiameterLabel.Name = "orificeDiameterLabel";
            this.orificeDiameterLabel.Size = new System.Drawing.Size(80, 13);
            this.orificeDiameterLabel.TabIndex = 45;
            this.orificeDiameterLabel.Text = "Orifice diameter";
            // 
            // SteadySelector
            // 
            this.SteadySelector.AutoSize = true;
            this.SteadySelector.Location = new System.Drawing.Point(113, 214);
            this.SteadySelector.Name = "SteadySelector";
            this.SteadySelector.Size = new System.Drawing.Size(58, 17);
            this.SteadySelector.TabIndex = 48;
            this.SteadySelector.Text = "Steady";
            this.SteadySelector.UseVisualStyleBackColor = true;
            this.SteadySelector.CheckedChanged += new System.EventHandler(this.ReleaseTypeChanged);
            // 
            // BlowdownSelector
            // 
            this.BlowdownSelector.AutoSize = true;
            this.BlowdownSelector.Checked = true;
            this.BlowdownSelector.Location = new System.Drawing.Point(175, 214);
            this.BlowdownSelector.Name = "BlowdownSelector";
            this.BlowdownSelector.Size = new System.Drawing.Size(74, 17);
            this.BlowdownSelector.TabIndex = 49;
            this.BlowdownSelector.TabStop = true;
            this.BlowdownSelector.Text = "Blowdown";
            this.BlowdownSelector.UseVisualStyleBackColor = true;
            this.BlowdownSelector.CheckedChanged += new System.EventHandler(this.ReleaseTypeChanged);
            // 
            // releaseTypeLabel
            // 
            this.releaseTypeLabel.AutoSize = true;
            this.releaseTypeLabel.Location = new System.Drawing.Point(8, 216);
            this.releaseTypeLabel.Name = "releaseTypeLabel";
            this.releaseTypeLabel.Size = new System.Drawing.Size(73, 13);
            this.releaseTypeLabel.TabIndex = 50;
            this.releaseTypeLabel.Text = "Release Type";
            // 
            // DcInput
            // 
            this.DcInput.Location = new System.Drawing.Point(273, 182);
            this.DcInput.Name = "DcInput";
            this.DcInput.Size = new System.Drawing.Size(107, 20);
            this.DcInput.TabIndex = 66;
            this.DcInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.DcInput.TextChanged += new System.EventHandler(this.DcInput_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 65;
            this.label1.Text = "Discharge coefficient";
            // 
            // warningMsg
            // 
            this.warningMsg.AutoSize = true;
            this.warningMsg.BackColor = System.Drawing.Color.MistyRose;
            this.warningMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningMsg.ForeColor = System.Drawing.Color.Maroon;
            this.warningMsg.Location = new System.Drawing.Point(8, 277);
            this.warningMsg.Name = "warningMsg";
            this.warningMsg.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.warningMsg.Size = new System.Drawing.Size(198, 23);
            this.warningMsg.TabIndex = 64;
            this.warningMsg.Text = "Warning/error message here";
            this.warningMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.warningMsg.Visible = false;
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(241, 246);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 63;
            this.spinnerPictureBox.TabStop = false;
            // 
            // outputLabel
            // 
            this.outputLabel.AutoSize = true;
            this.outputLabel.Location = new System.Drawing.Point(8, 252);
            this.outputLabel.Name = "outputLabel";
            this.outputLabel.Size = new System.Drawing.Size(114, 13);
            this.outputLabel.TabIndex = 55;
            this.outputLabel.Text = "Mass Flow Rate (kg/s)";
            // 
            // MassFlowTextbox
            // 
            this.MassFlowTextbox.Enabled = false;
            this.MassFlowTextbox.Location = new System.Drawing.Point(273, 249);
            this.MassFlowTextbox.Name = "MassFlowTextbox";
            this.MassFlowTextbox.Size = new System.Drawing.Size(107, 20);
            this.MassFlowTextbox.TabIndex = 56;
            // 
            // PhaseSelector
            // 
            this.PhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PhaseSelector.FormattingEnabled = true;
            this.PhaseSelector.Location = new System.Drawing.Point(273, 8);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(107, 21);
            this.PhaseSelector.TabIndex = 52;
            this.PhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.PhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Location = new System.Drawing.Point(8, 11);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(61, 13);
            this.phaseLabel.TabIndex = 51;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.AmbPresInput);
            this.panel1.Controls.Add(this.AmbPresUnitSelector);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.phaseLabel);
            this.panel1.Controls.Add(this.DcInput);
            this.panel1.Controls.Add(this.submitBtn);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.VolumeInput);
            this.panel1.Controls.Add(this.warningMsg);
            this.panel1.Controls.Add(this.orificeDiameterLabel);
            this.panel1.Controls.Add(this.spinnerPictureBox);
            this.panel1.Controls.Add(this.PressureInput);
            this.panel1.Controls.Add(this.PlotBox);
            this.panel1.Controls.Add(this.OrificeDiameterUnitSelector);
            this.panel1.Controls.Add(this.outputLabel);
            this.panel1.Controls.Add(this.TemperatureInput);
            this.panel1.Controls.Add(this.MassFlowTextbox);
            this.panel1.Controls.Add(this.OrificeDiameterInput);
            this.panel1.Controls.Add(this.PhaseSelector);
            this.panel1.Controls.Add(this.TankVolumeUnitSelector);
            this.panel1.Controls.Add(this.BlowdownSelector);
            this.panel1.Controls.Add(this.PressureUnitSelector);
            this.panel1.Controls.Add(this.TemperatureUnitSelector);
            this.panel1.Controls.Add(this.temperatureInputLabel);
            this.panel1.Controls.Add(this.SteadySelector);
            this.panel1.Controls.Add(this.pressureInputLabel);
            this.panel1.Controls.Add(this.volumeInputLabel);
            this.panel1.Controls.Add(this.releaseTypeLabel);
            this.panel1.Location = new System.Drawing.Point(0, 8);
            this.panel1.Margin = new System.Windows.Forms.Padding(8, 8, 8, 8);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel1.Size = new System.Drawing.Size(984, 578);
            this.panel1.TabIndex = 55;
            // 
            // AmbPresInput
            // 
            this.AmbPresInput.Location = new System.Drawing.Point(273, 97);
            this.AmbPresInput.Name = "AmbPresInput";
            this.AmbPresInput.Size = new System.Drawing.Size(107, 20);
            this.AmbPresInput.TabIndex = 69;
            this.AmbPresInput.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.AmbPresInput.TextChanged += new System.EventHandler(this.AmbPresInput_TextChanged);
            // 
            // AmbPresUnitSelector
            // 
            this.AmbPresUnitSelector.Converter = null;
            this.AmbPresUnitSelector.Location = new System.Drawing.Point(113, 96);
            this.AmbPresUnitSelector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.AmbPresUnitSelector.Name = "AmbPresUnitSelector";
            this.AmbPresUnitSelector.SelectedItem = null;
            this.AmbPresUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.AmbPresUnitSelector.StoredValue = new double[0];
            this.AmbPresUnitSelector.TabIndex = 68;
            this.AmbPresUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.AmbPresUnitSelector_OnSelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 67;
            this.label2.Text = "Ambient pressure";
            // 
            // PlotBox
            // 
            this.PlotBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlotBox.Location = new System.Drawing.Point(386, 2);
            this.PlotBox.Name = "PlotBox";
            this.PlotBox.Size = new System.Drawing.Size(598, 576);
            this.PlotBox.TabIndex = 57;
            this.PlotBox.TabStop = false;
            // 
            // OrificeDiameterUnitSelector
            // 
            this.OrificeDiameterUnitSelector.Converter = null;
            this.OrificeDiameterUnitSelector.Location = new System.Drawing.Point(113, 155);
            this.OrificeDiameterUnitSelector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OrificeDiameterUnitSelector.Name = "OrificeDiameterUnitSelector";
            this.OrificeDiameterUnitSelector.SelectedItem = null;
            this.OrificeDiameterUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.OrificeDiameterUnitSelector.StoredValue = new double[0];
            this.OrificeDiameterUnitSelector.TabIndex = 46;
            this.OrificeDiameterUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.OrificeDiameterUnitSelector_OnSelectedIndexChanged);
            // 
            // TankVolumeUnitSelector
            // 
            this.TankVolumeUnitSelector.Converter = null;
            this.TankVolumeUnitSelector.Location = new System.Drawing.Point(113, 126);
            this.TankVolumeUnitSelector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TankVolumeUnitSelector.Name = "TankVolumeUnitSelector";
            this.TankVolumeUnitSelector.SelectedItem = null;
            this.TankVolumeUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.TankVolumeUnitSelector.StoredValue = new double[0];
            this.TankVolumeUnitSelector.TabIndex = 40;
            this.TankVolumeUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.TankVolumeUnitSelector_OnSelectedIndexChange);
            // 
            // PressureUnitSelector
            // 
            this.PressureUnitSelector.Converter = null;
            this.PressureUnitSelector.Location = new System.Drawing.Point(113, 66);
            this.PressureUnitSelector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.PressureUnitSelector.Name = "PressureUnitSelector";
            this.PressureUnitSelector.SelectedItem = null;
            this.PressureUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.PressureUnitSelector.StoredValue = new double[0];
            this.PressureUnitSelector.TabIndex = 39;
            this.PressureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.PressureUnitSelector_OnSelectedIndexChange);
            // 
            // TemperatureUnitSelector
            // 
            this.TemperatureUnitSelector.Converter = null;
            this.TemperatureUnitSelector.Location = new System.Drawing.Point(113, 37);
            this.TemperatureUnitSelector.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TemperatureUnitSelector.Name = "TemperatureUnitSelector";
            this.TemperatureUnitSelector.SelectedItem = null;
            this.TemperatureUnitSelector.Size = new System.Drawing.Size(152, 22);
            this.TemperatureUnitSelector.StoredValue = new double[0];
            this.TemperatureUnitSelector.TabIndex = 38;
            this.TemperatureUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.TemperatureUnitSelector_OnSelectedIndexChange);
            // 
            // MassFlowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "MassFlowForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.EtkMassFlowRateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PlotBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox VolumeInput;
		private System.Windows.Forms.TextBox PressureInput;
		private System.Windows.Forms.TextBox TemperatureInput;
		private ValueConverterDropdown TankVolumeUnitSelector;
		private ValueConverterDropdown PressureUnitSelector;
		private ValueConverterDropdown TemperatureUnitSelector;
		private System.Windows.Forms.Label volumeInputLabel;
		private System.Windows.Forms.Label pressureInputLabel;
		private System.Windows.Forms.Label temperatureInputLabel;
		private System.Windows.Forms.Button submitBtn;
		private System.Windows.Forms.TextBox OrificeDiameterInput;
		private ValueConverterDropdown OrificeDiameterUnitSelector;
		private System.Windows.Forms.Label orificeDiameterLabel;
		private System.Windows.Forms.RadioButton SteadySelector;
		private System.Windows.Forms.RadioButton BlowdownSelector;
		private System.Windows.Forms.Label releaseTypeLabel;
        private System.Windows.Forms.Label phaseLabel;
        private System.Windows.Forms.ComboBox PhaseSelector;
        private PictureBoxWithSave PlotBox;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.TextBox MassFlowTextbox;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
        private System.Windows.Forms.Label warningMsg;
        private System.Windows.Forms.TextBox DcInput;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox AmbPresInput;
        private ValueConverterDropdown AmbPresUnitSelector;
        private System.Windows.Forms.Label label2;
    }
}
