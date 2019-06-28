namespace QRA_Frontend.ETK.ETKWrap.ContentPanels {
	partial class CpEtkTankMass {
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
			this.btnCalculate = new System.Windows.Forms.Button();
			this.tbVolume = new System.Windows.Forms.TextBox();
			this.tbPressure = new System.Windows.Forms.TextBox();
			this.tbTemperature = new System.Windows.Forms.TextBox();
			this.ddTankVolume = new QRA_Frontend.ValueConverterDropdown();
			this.ddPressure = new QRA_Frontend.ValueConverterDropdown();
			this.ddTemperature = new QRA_Frontend.ValueConverterDropdown();
			this.lblVolume = new System.Windows.Forms.Label();
			this.lblPressure = new System.Windows.Forms.Label();
			this.lblTemperature = new System.Windows.Forms.Label();
			this.ddMass = new QRA_Frontend.ValueConverterDropdown();
			this.lblMass = new System.Windows.Forms.Label();
			this.tbMass = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// btnCalculate
			// 
			this.btnCalculate.Enabled = false;
			this.btnCalculate.Location = new System.Drawing.Point(296, 99);
			this.btnCalculate.Name = "btnCalculate";
			this.btnCalculate.Size = new System.Drawing.Size(107, 23);
			this.btnCalculate.TabIndex = 35;
			this.btnCalculate.Text = "Calculate Mass";
			this.btnCalculate.UseVisualStyleBackColor = true;
			this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
			// 
			// tbVolume
			// 
			this.tbVolume.Location = new System.Drawing.Point(296, 73);
			this.tbVolume.Name = "tbVolume";
			this.tbVolume.Size = new System.Drawing.Size(107, 20);
			this.tbVolume.TabIndex = 34;
			this.tbVolume.TextChanged += new System.EventHandler(this.tbVolume_TextChanged);
			// 
			// tbPressure
			// 
			this.tbPressure.Location = new System.Drawing.Point(296, 43);
			this.tbPressure.Name = "tbPressure";
			this.tbPressure.Size = new System.Drawing.Size(107, 20);
			this.tbPressure.TabIndex = 33;
			this.tbPressure.TextChanged += new System.EventHandler(this.tbPressure_TextChanged);
			// 
			// tbTemperature
			// 
			this.tbTemperature.Location = new System.Drawing.Point(296, 13);
			this.tbTemperature.Name = "tbTemperature";
			this.tbTemperature.Size = new System.Drawing.Size(107, 20);
			this.tbTemperature.TabIndex = 32;
			this.tbTemperature.TextChanged += new System.EventHandler(this.tbTemperature_TextChanged);
			// 
			// ddTankVolume
			// 
			this.ddTankVolume.Converter = null;
			this.ddTankVolume.Location = new System.Drawing.Point(94, 71);
			this.ddTankVolume.Name = "ddTankVolume";
			this.ddTankVolume.SelectedItem = null;
			this.ddTankVolume.Size = new System.Drawing.Size(189, 22);
			this.ddTankVolume.StoredValue = new double[0];
			this.ddTankVolume.TabIndex = 31;
			this.ddTankVolume.OnSelectedIndexChanged += new System.EventHandler(this.ddVolume_OnSelectedIndexChange);
			// 
			// ddPressure
			// 
			this.ddPressure.Converter = null;
			this.ddPressure.Location = new System.Drawing.Point(94, 41);
			this.ddPressure.Name = "ddPressure";
			this.ddPressure.SelectedItem = null;
			this.ddPressure.Size = new System.Drawing.Size(189, 22);
			this.ddPressure.StoredValue = new double[0];
			this.ddPressure.TabIndex = 30;
			this.ddPressure.OnSelectedIndexChanged += new System.EventHandler(this.ddPressure_OnSelectedIndexChange);
			// 
			// ddTemperature
			// 
			this.ddTemperature.Converter = null;
			this.ddTemperature.Location = new System.Drawing.Point(94, 11);
			this.ddTemperature.Name = "ddTemperature";
			this.ddTemperature.SelectedItem = null;
			this.ddTemperature.Size = new System.Drawing.Size(189, 22);
			this.ddTemperature.StoredValue = new double[0];
			this.ddTemperature.TabIndex = 29;
			this.ddTemperature.OnSelectedIndexChanged += new System.EventHandler(this.ddTemperature_OnSelectedIndexChange);
			// 
			// lblVolume
			// 
			this.lblVolume.AutoSize = true;
			this.lblVolume.Location = new System.Drawing.Point(13, 76);
			this.lblVolume.Name = "lblVolume";
			this.lblVolume.Size = new System.Drawing.Size(42, 13);
			this.lblVolume.TabIndex = 28;
			this.lblVolume.Text = "Volume";
			// 
			// lblPressure
			// 
			this.lblPressure.AutoSize = true;
			this.lblPressure.Location = new System.Drawing.Point(13, 43);
			this.lblPressure.Name = "lblPressure";
			this.lblPressure.Size = new System.Drawing.Size(48, 13);
			this.lblPressure.TabIndex = 27;
			this.lblPressure.Text = "Pressure";
			// 
			// lblTemperature
			// 
			this.lblTemperature.AutoSize = true;
			this.lblTemperature.Location = new System.Drawing.Point(13, 15);
			this.lblTemperature.Name = "lblTemperature";
			this.lblTemperature.Size = new System.Drawing.Size(67, 13);
			this.lblTemperature.TabIndex = 26;
			this.lblTemperature.Text = "Temperature";
			// 
			// ddMass
			// 
			this.ddMass.Converter = null;
			this.ddMass.Location = new System.Drawing.Point(94, 133);
			this.ddMass.Name = "ddMass";
			this.ddMass.SelectedItem = null;
			this.ddMass.Size = new System.Drawing.Size(189, 22);
			this.ddMass.StoredValue = new double[0];
			this.ddMass.TabIndex = 37;
			this.ddMass.Visible = false;
			this.ddMass.OnSelectedIndexChanged += new System.EventHandler(this.ddMass_OnSelectedIndexChanged);
			// 
			// lblMass
			// 
			this.lblMass.AutoSize = true;
			this.lblMass.Location = new System.Drawing.Point(13, 138);
			this.lblMass.Name = "lblMass";
			this.lblMass.Size = new System.Drawing.Size(32, 13);
			this.lblMass.TabIndex = 36;
			this.lblMass.Text = "Mass";
			this.lblMass.Visible = false;
			// 
			// tbMass
			// 
			this.tbMass.Enabled = false;
			this.tbMass.Location = new System.Drawing.Point(296, 135);
			this.tbMass.Name = "tbMass";
			this.tbMass.Size = new System.Drawing.Size(107, 20);
			this.tbMass.TabIndex = 38;
			this.tbMass.Visible = false;
			this.tbMass.TextChanged += new System.EventHandler(this.tbMass_TextChanged);
			// 
			// cpEtkTankMass
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbMass);
			this.Controls.Add(this.ddMass);
			this.Controls.Add(this.lblMass);
			this.Controls.Add(this.btnCalculate);
			this.Controls.Add(this.tbVolume);
			this.Controls.Add(this.tbPressure);
			this.Controls.Add(this.tbTemperature);
			this.Controls.Add(this.ddTankVolume);
			this.Controls.Add(this.ddPressure);
			this.Controls.Add(this.ddTemperature);
			this.Controls.Add(this.lblVolume);
			this.Controls.Add(this.lblPressure);
			this.Controls.Add(this.lblTemperature);
			this.Name = "CpEtkTankMass";
			this.Size = new System.Drawing.Size(415, 167);
			this.Load += new System.EventHandler(this.cpEtkTankMass_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnCalculate;
		private System.Windows.Forms.TextBox tbVolume;
		private System.Windows.Forms.TextBox tbPressure;
		private System.Windows.Forms.TextBox tbTemperature;
		private ValueConverterDropdown ddTankVolume;
		private ValueConverterDropdown ddPressure;
		private ValueConverterDropdown ddTemperature;
		private System.Windows.Forms.Label lblVolume;
		private System.Windows.Forms.Label lblPressure;
		private System.Windows.Forms.Label lblTemperature;
		private ValueConverterDropdown ddMass;
		private System.Windows.Forms.Label lblMass;
		private System.Windows.Forms.TextBox tbMass;
	}
}
