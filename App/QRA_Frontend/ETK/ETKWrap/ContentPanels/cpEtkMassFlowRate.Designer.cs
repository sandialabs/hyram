namespace QRA_Frontend.ETK.ETKWrap.ContentPanels {
	partial class CpEtkMassFlowRate {
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
            this.tbVolume = new System.Windows.Forms.TextBox();
            this.tbPressure = new System.Windows.Forms.TextBox();
            this.tbTemperature = new System.Windows.Forms.TextBox();
            this.lblVolume = new System.Windows.Forms.Label();
            this.lblPressure = new System.Windows.Forms.Label();
            this.lblTemperature = new System.Windows.Forms.Label();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.tbOrificeDiameter = new System.Windows.Forms.TextBox();
            this.lblOrificeDiam = new System.Windows.Forms.Label();
            this.rbRtSteady = new System.Windows.Forms.RadioButton();
            this.RbRtBlowdown = new System.Windows.Forms.RadioButton();
            this.lblReleaseType = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpInput = new System.Windows.Forms.TabPage();
            this.ddPressure = new QRA_Frontend.ValueConverterDropdown();
            this.ddTemperature = new QRA_Frontend.ValueConverterDropdown();
            this.ddTankVolume = new QRA_Frontend.ValueConverterDropdown();
            this.ddOrificeDiameter = new QRA_Frontend.ValueConverterDropdown();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.pbResultImage = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.tcMain.SuspendLayout();
            this.tpInput.SuspendLayout();
            this.tpOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResultImage)).BeginInit();
            this.SuspendLayout();
            // 
            // tbVolume
            // 
            this.tbVolume.Location = new System.Drawing.Point(431, 97);
            this.tbVolume.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbVolume.Name = "tbVolume";
            this.tbVolume.Size = new System.Drawing.Size(141, 22);
            this.tbVolume.TabIndex = 43;
            this.tbVolume.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbVolume.TextChanged += new System.EventHandler(this.tbVolume_TextChanged);
            // 
            // tbPressure
            // 
            this.tbPressure.Location = new System.Drawing.Point(431, 57);
            this.tbPressure.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbPressure.Name = "tbPressure";
            this.tbPressure.Size = new System.Drawing.Size(141, 22);
            this.tbPressure.TabIndex = 42;
            this.tbPressure.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbPressure.TextChanged += new System.EventHandler(this.tbPressure_TextChanged);
            // 
            // tbTemperature
            // 
            this.tbTemperature.Location = new System.Drawing.Point(431, 23);
            this.tbTemperature.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbTemperature.Name = "tbTemperature";
            this.tbTemperature.Size = new System.Drawing.Size(141, 22);
            this.tbTemperature.TabIndex = 41;
            this.tbTemperature.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbTemperature.TextChanged += new System.EventHandler(this.tbTemperature_TextChanged);
            // 
            // lblVolume
            // 
            this.lblVolume.AutoSize = true;
            this.lblVolume.Location = new System.Drawing.Point(25, 102);
            this.lblVolume.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblVolume.Name = "lblVolume";
            this.lblVolume.Size = new System.Drawing.Size(55, 17);
            this.lblVolume.TabIndex = 37;
            this.lblVolume.Text = "Volume";
            // 
            // lblPressure
            // 
            this.lblPressure.AutoSize = true;
            this.lblPressure.Location = new System.Drawing.Point(25, 62);
            this.lblPressure.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPressure.Name = "lblPressure";
            this.lblPressure.Size = new System.Drawing.Size(65, 17);
            this.lblPressure.TabIndex = 36;
            this.lblPressure.Text = "Pressure";
            // 
            // lblTemperature
            // 
            this.lblTemperature.AutoSize = true;
            this.lblTemperature.Location = new System.Drawing.Point(25, 28);
            this.lblTemperature.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTemperature.Name = "lblTemperature";
            this.lblTemperature.Size = new System.Drawing.Size(90, 17);
            this.lblTemperature.TabIndex = 35;
            this.lblTemperature.Text = "Temperature";
            // 
            // btnCalculate
            // 
            this.btnCalculate.Enabled = false;
            this.btnCalculate.Location = new System.Drawing.Point(369, 182);
            this.btnCalculate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(204, 28);
            this.btnCalculate.TabIndex = 44;
            this.btnCalculate.Text = "Calculate Mass Flow Rate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            this.btnCalculate.MouseMove += new System.Windows.Forms.MouseEventHandler(this.btnCalculate_MouseMove);
            // 
            // tbOrificeDiameter
            // 
            this.tbOrificeDiameter.Location = new System.Drawing.Point(431, 133);
            this.tbOrificeDiameter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbOrificeDiameter.Name = "tbOrificeDiameter";
            this.tbOrificeDiameter.Size = new System.Drawing.Size(141, 22);
            this.tbOrificeDiameter.TabIndex = 47;
            this.tbOrificeDiameter.Text = "3.0e-2";
            this.tbOrificeDiameter.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tbOrificeDiameter.TextChanged += new System.EventHandler(this.tbOrificeDiameter_TextChanged);
            // 
            // lblOrificeDiam
            // 
            this.lblOrificeDiam.AutoSize = true;
            this.lblOrificeDiam.Location = new System.Drawing.Point(25, 138);
            this.lblOrificeDiam.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOrificeDiam.Name = "lblOrificeDiam";
            this.lblOrificeDiam.Size = new System.Drawing.Size(110, 17);
            this.lblOrificeDiam.TabIndex = 45;
            this.lblOrificeDiam.Text = "Orifice Diameter";
            // 
            // rbRtSteady
            // 
            this.rbRtSteady.AutoSize = true;
            this.rbRtSteady.Location = new System.Drawing.Point(139, 182);
            this.rbRtSteady.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbRtSteady.Name = "rbRtSteady";
            this.rbRtSteady.Size = new System.Drawing.Size(73, 21);
            this.rbRtSteady.TabIndex = 48;
            this.rbRtSteady.Text = "Steady";
            this.rbRtSteady.UseVisualStyleBackColor = true;
            this.rbRtSteady.CheckedChanged += new System.EventHandler(this.ReleaseTypeChanged);
            // 
            // RbRtBlowdown
            // 
            this.RbRtBlowdown.AutoSize = true;
            this.RbRtBlowdown.Checked = true;
            this.RbRtBlowdown.Location = new System.Drawing.Point(221, 182);
            this.RbRtBlowdown.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.RbRtBlowdown.Name = "RbRtBlowdown";
            this.RbRtBlowdown.Size = new System.Drawing.Size(91, 21);
            this.RbRtBlowdown.TabIndex = 49;
            this.RbRtBlowdown.TabStop = true;
            this.RbRtBlowdown.Text = "Blowdown";
            this.RbRtBlowdown.UseVisualStyleBackColor = true;
            this.RbRtBlowdown.CheckedChanged += new System.EventHandler(this.ReleaseTypeChanged);
            // 
            // lblReleaseType
            // 
            this.lblReleaseType.AutoSize = true;
            this.lblReleaseType.Location = new System.Drawing.Point(28, 185);
            this.lblReleaseType.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblReleaseType.Name = "lblReleaseType";
            this.lblReleaseType.Size = new System.Drawing.Size(96, 17);
            this.lblReleaseType.TabIndex = 50;
            this.lblReleaseType.Text = "Release Type";
            // 
            // tbResult
            // 
            this.tbResult.Enabled = false;
            this.tbResult.Location = new System.Drawing.Point(189, 12);
            this.tbResult.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbResult.Name = "tbResult";
            this.tbResult.Size = new System.Drawing.Size(141, 22);
            this.tbResult.TabIndex = 53;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(21, 16);
            this.lblResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(147, 17);
            this.lblResult.TabIndex = 51;
            this.lblResult.Text = "Mass Flow Rate (kg/s)";
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpInput);
            this.tcMain.Controls.Add(this.tpOutput);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(696, 511);
            this.tcMain.TabIndex = 54;
            // 
            // tpInput
            // 
            this.tpInput.Controls.Add(this.ddPressure);
            this.tpInput.Controls.Add(this.lblTemperature);
            this.tpInput.Controls.Add(this.lblPressure);
            this.tpInput.Controls.Add(this.lblReleaseType);
            this.tpInput.Controls.Add(this.lblVolume);
            this.tpInput.Controls.Add(this.rbRtSteady);
            this.tpInput.Controls.Add(this.ddTemperature);
            this.tpInput.Controls.Add(this.RbRtBlowdown);
            this.tpInput.Controls.Add(this.ddTankVolume);
            this.tpInput.Controls.Add(this.tbOrificeDiameter);
            this.tpInput.Controls.Add(this.tbTemperature);
            this.tpInput.Controls.Add(this.ddOrificeDiameter);
            this.tpInput.Controls.Add(this.tbPressure);
            this.tpInput.Controls.Add(this.lblOrificeDiam);
            this.tpInput.Controls.Add(this.tbVolume);
            this.tpInput.Controls.Add(this.btnCalculate);
            this.tpInput.Location = new System.Drawing.Point(4, 25);
            this.tpInput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tpInput.Name = "tpInput";
            this.tpInput.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tpInput.Size = new System.Drawing.Size(688, 482);
            this.tpInput.TabIndex = 0;
            this.tpInput.Text = "Input";
            this.tpInput.UseVisualStyleBackColor = true;
            this.tpInput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tpInput_MouseMove);
            // 
            // ddPressure
            // 
            this.ddPressure.Converter = null;
            this.ddPressure.Location = new System.Drawing.Point(161, 55);
            this.ddPressure.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ddPressure.Name = "ddPressure";
            this.ddPressure.SelectedItem = null;
            this.ddPressure.Size = new System.Drawing.Size(252, 27);
            this.ddPressure.StoredValue = new double[0];
            this.ddPressure.TabIndex = 39;
            this.ddPressure.OnSelectedIndexChanged += new System.EventHandler(this.ddPressure_OnSelectedIndexChange);
            // 
            // ddTemperature
            // 
            this.ddTemperature.Converter = null;
            this.ddTemperature.Location = new System.Drawing.Point(161, 22);
            this.ddTemperature.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ddTemperature.Name = "ddTemperature";
            this.ddTemperature.SelectedItem = null;
            this.ddTemperature.Size = new System.Drawing.Size(252, 27);
            this.ddTemperature.StoredValue = new double[0];
            this.ddTemperature.TabIndex = 38;
            this.ddTemperature.OnSelectedIndexChanged += new System.EventHandler(this.ddTemperature_OnSelectedIndexChange);
            // 
            // ddTankVolume
            // 
            this.ddTankVolume.Converter = null;
            this.ddTankVolume.Location = new System.Drawing.Point(161, 96);
            this.ddTankVolume.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ddTankVolume.Name = "ddTankVolume";
            this.ddTankVolume.SelectedItem = null;
            this.ddTankVolume.Size = new System.Drawing.Size(252, 27);
            this.ddTankVolume.StoredValue = new double[0];
            this.ddTankVolume.TabIndex = 40;
            this.ddTankVolume.OnSelectedIndexChanged += new System.EventHandler(this.ddVolume_OnSelectedIndexChange);
            // 
            // ddOrificeDiameter
            // 
            this.ddOrificeDiameter.Converter = null;
            this.ddOrificeDiameter.Location = new System.Drawing.Point(161, 132);
            this.ddOrificeDiameter.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.ddOrificeDiameter.Name = "ddOrificeDiameter";
            this.ddOrificeDiameter.SelectedItem = null;
            this.ddOrificeDiameter.Size = new System.Drawing.Size(252, 27);
            this.ddOrificeDiameter.StoredValue = new double[0];
            this.ddOrificeDiameter.TabIndex = 46;
            this.ddOrificeDiameter.OnSelectedIndexChanged += new System.EventHandler(this.ddOrificeDiameter_OnSelectedIndexChanged);
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.pbResultImage);
            this.tpOutput.Controls.Add(this.lblResult);
            this.tpOutput.Controls.Add(this.tbResult);
            this.tpOutput.Location = new System.Drawing.Point(4, 25);
            this.tpOutput.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tpOutput.Size = new System.Drawing.Size(688, 482);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            this.tpOutput.Enter += new System.EventHandler(this.tpOutput_Enter);
            // 
            // pbResultImage
            // 
            this.pbResultImage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbResultImage.Location = new System.Drawing.Point(9, 44);
            this.pbResultImage.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pbResultImage.Name = "pbResultImage";
            this.pbResultImage.Size = new System.Drawing.Size(668, 427);
            this.pbResultImage.TabIndex = 54;
            this.pbResultImage.TabStop = false;
            // 
            // cpEtkMassFlowRate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcMain);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "CpEtkMassFlowRate";
            this.Size = new System.Drawing.Size(696, 511);
            this.Load += new System.EventHandler(this.cpEtkMassFlowRate_Load);
            this.tcMain.ResumeLayout(false);
            this.tpInput.ResumeLayout(false);
            this.tpInput.PerformLayout();
            this.tpOutput.ResumeLayout(false);
            this.tpOutput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbResultImage)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox tbVolume;
		private System.Windows.Forms.TextBox tbPressure;
		private System.Windows.Forms.TextBox tbTemperature;
		private ValueConverterDropdown ddTankVolume;
		private ValueConverterDropdown ddPressure;
		private ValueConverterDropdown ddTemperature;
		private System.Windows.Forms.Label lblVolume;
		private System.Windows.Forms.Label lblPressure;
		private System.Windows.Forms.Label lblTemperature;
		private System.Windows.Forms.Button btnCalculate;
		private System.Windows.Forms.TextBox tbOrificeDiameter;
		private ValueConverterDropdown ddOrificeDiameter;
		private System.Windows.Forms.Label lblOrificeDiam;
		private System.Windows.Forms.RadioButton rbRtSteady;
		private System.Windows.Forms.RadioButton RbRtBlowdown;
		private System.Windows.Forms.Label lblReleaseType;
		private System.Windows.Forms.TextBox tbResult;
		private System.Windows.Forms.Label lblResult;
		private System.Windows.Forms.TabControl tcMain;
		private System.Windows.Forms.TabPage tpInput;
		private System.Windows.Forms.TabPage tpOutput;
		private CustomControls.PictureBoxWithSave pbResultImage;
	}
}
