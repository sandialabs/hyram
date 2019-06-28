namespace QRA_Frontend.ETK.ETKWrap.ContentPanels {
	partial class CpEtkTempPressureDensity {
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
			this.gbCalculateOpts = new System.Windows.Forms.GroupBox();
			this.rb_Temperature = new System.Windows.Forms.RadioButton();
			this.rb_Density = new System.Windows.Forms.RadioButton();
			this.rb_Pressure = new System.Windows.Forms.RadioButton();
			this.btn_Calculate = new System.Windows.Forms.Button();
			this.tb_tpd_Density = new System.Windows.Forms.TextBox();
			this.tb_tpd_Pressure = new System.Windows.Forms.TextBox();
			this.tb_tpd_Temperature = new System.Windows.Forms.TextBox();
			this.lblDensity = new System.Windows.Forms.Label();
			this.lblPressure = new System.Windows.Forms.Label();
			this.lblTemperature = new System.Windows.Forms.Label();
            this.dd_Density = new QRA_Frontend.ValueConverterDropdown();
            this.dd_Pressure = new QRA_Frontend.ValueConverterDropdown();
            this.dd_Temperature = new QRA_Frontend.ValueConverterDropdown();
            this.gbCalculateOpts.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbCalculateOpts
			// 
			this.gbCalculateOpts.Controls.Add(this.rb_Temperature);
			this.gbCalculateOpts.Controls.Add(this.rb_Density);
			this.gbCalculateOpts.Controls.Add(this.rb_Pressure);
			this.gbCalculateOpts.Location = new System.Drawing.Point(-4, 12);
			this.gbCalculateOpts.Name = "gbCalculateOpts";
			this.gbCalculateOpts.Size = new System.Drawing.Size(94, 77);
			this.gbCalculateOpts.TabIndex = 24;
			this.gbCalculateOpts.TabStop = false;
			this.gbCalculateOpts.Text = "Calculate...";
			// 
			// rb_Temperature
			// 
			this.rb_Temperature.AutoSize = true;
			this.rb_Temperature.Checked = true;
			this.rb_Temperature.Location = new System.Drawing.Point(9, 19);
			this.rb_Temperature.Name = "rb_Temperature";
			this.rb_Temperature.Size = new System.Drawing.Size(85, 17);
			this.rb_Temperature.TabIndex = 10;
			this.rb_Temperature.TabStop = true;
			this.rb_Temperature.Text = "Temperature";
			this.rb_Temperature.UseVisualStyleBackColor = true;
			this.rb_Temperature.CheckedChanged += new System.EventHandler(this.CalcOptionRbCheckedChanged);
			// 
			// rb_Density
			// 
			this.rb_Density.AutoSize = true;
			this.rb_Density.Location = new System.Drawing.Point(9, 53);
			this.rb_Density.Name = "rb_Density";
			this.rb_Density.Size = new System.Drawing.Size(60, 17);
			this.rb_Density.TabIndex = 12;
			this.rb_Density.Text = "Density";
			this.rb_Density.UseVisualStyleBackColor = true;
			this.rb_Density.CheckedChanged += new System.EventHandler(this.CalcOptionRbCheckedChanged);
			// 
			// rb_Pressure
			// 
			this.rb_Pressure.AutoSize = true;
			this.rb_Pressure.Location = new System.Drawing.Point(9, 36);
			this.rb_Pressure.Name = "rb_Pressure";
			this.rb_Pressure.Size = new System.Drawing.Size(66, 17);
			this.rb_Pressure.TabIndex = 11;
			this.rb_Pressure.Text = "Pressure";
			this.rb_Pressure.UseVisualStyleBackColor = true;
			this.rb_Pressure.CheckedChanged += new System.EventHandler(this.CalcOptionRbCheckedChanged);
			// 
			// btn_Calculate
			// 
			this.btn_Calculate.Enabled = false;
			this.btn_Calculate.Location = new System.Drawing.Point(374, 99);
			this.btn_Calculate.Name = "btn_Calculate";
			this.btn_Calculate.Size = new System.Drawing.Size(142, 23);
			this.btn_Calculate.TabIndex = 25;
			this.btn_Calculate.UseVisualStyleBackColor = true;
			this.btn_Calculate.Click += new System.EventHandler(this.btnCalculate_Click);
			// 
			// tb_tpd_Density
			// 
			this.tb_tpd_Density.Enabled = false;
			this.tb_tpd_Density.Location = new System.Drawing.Point(399, 73);
			this.tb_tpd_Density.Name = "tb_tpd_Density";
			this.tb_tpd_Density.Size = new System.Drawing.Size(107, 20);
			this.tb_tpd_Density.TabIndex = 23;
			this.tb_tpd_Density.TextChanged += new System.EventHandler(this.tbDensity_TextChanged);
			// 
			// tb_tpd_Pressure
			// 
			this.tb_tpd_Pressure.Enabled = false;
			this.tb_tpd_Pressure.Location = new System.Drawing.Point(399, 43);
			this.tb_tpd_Pressure.Name = "tb_tpd_Pressure";
			this.tb_tpd_Pressure.Size = new System.Drawing.Size(107, 20);
			this.tb_tpd_Pressure.TabIndex = 22;
			this.tb_tpd_Pressure.TextChanged += new System.EventHandler(this.tbPressure_TextChanged);
			// 
			// tb_tpd_Temperature
			// 
			this.tb_tpd_Temperature.Enabled = false;
			this.tb_tpd_Temperature.Location = new System.Drawing.Point(399, 13);
			this.tb_tpd_Temperature.Name = "tb_tpd_Temperature";
			this.tb_tpd_Temperature.Size = new System.Drawing.Size(107, 20);
			this.tb_tpd_Temperature.TabIndex = 21;
			this.tb_tpd_Temperature.TextChanged += new System.EventHandler(this.tbTemperature_TextChanged);
			// 
			// lblDensity
			// 
			this.lblDensity.AutoSize = true;
			this.lblDensity.Location = new System.Drawing.Point(116, 76);
			this.lblDensity.Name = "lblDensity";
			this.lblDensity.Size = new System.Drawing.Size(42, 13);
			this.lblDensity.TabIndex = 17;
			this.lblDensity.Text = "Density";
			// 
			// lblPressure
			// 
			this.lblPressure.AutoSize = true;
			this.lblPressure.Location = new System.Drawing.Point(116, 43);
			this.lblPressure.Name = "lblPressure";
			this.lblPressure.Size = new System.Drawing.Size(48, 13);
			this.lblPressure.TabIndex = 16;
			this.lblPressure.Text = "Pressure";
			// 
			// lblTemperature
			// 
			this.lblTemperature.AutoSize = true;
			this.lblTemperature.Location = new System.Drawing.Point(116, 15);
			this.lblTemperature.Name = "lblTemperature";
			this.lblTemperature.Size = new System.Drawing.Size(67, 13);
			this.lblTemperature.TabIndex = 15;
			this.lblTemperature.Text = "Temperature";
			// 
			// dd_Density
			// 
			this.dd_Density.Converter = null;
			this.dd_Density.Location = new System.Drawing.Point(197, 71);
			this.dd_Density.Name = "dd_Density";
			this.dd_Density.SelectedItem = null;
			this.dd_Density.Size = new System.Drawing.Size(189, 22);
			this.dd_Density.StoredValue = new double[0];
			this.dd_Density.TabIndex = 20;
			this.dd_Density.OnSelectedIndexChanged += new System.EventHandler(this.ddDensity_OnSelectedIndexChange);
			// 
			// dd_Pressure
			// 
			this.dd_Pressure.Converter = null;
			this.dd_Pressure.Location = new System.Drawing.Point(197, 41);
			this.dd_Pressure.Name = "dd_Pressure";
			this.dd_Pressure.SelectedItem = null;
			this.dd_Pressure.Size = new System.Drawing.Size(189, 22);
			this.dd_Pressure.StoredValue = new double[0];
			this.dd_Pressure.TabIndex = 19;
			this.dd_Pressure.OnSelectedIndexChanged += new System.EventHandler(this.ddPressure_OnSelectedIndexChange);
			// 
			// dd_Temperature
			// 
			this.dd_Temperature.Converter = null;
			this.dd_Temperature.Location = new System.Drawing.Point(197, 11);
			this.dd_Temperature.Name = "dd_Temperature";
			this.dd_Temperature.SelectedItem = null;
			this.dd_Temperature.Size = new System.Drawing.Size(189, 22);
			this.dd_Temperature.StoredValue = new double[0];
			this.dd_Temperature.TabIndex = 18;
			this.dd_Temperature.OnSelectedIndexChanged += new System.EventHandler(this.ddTemperature_OnSelectedIndexChange);
			// 
			// cpEtkTempPressureDensity
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gbCalculateOpts);
			this.Controls.Add(this.btn_Calculate);
			this.Controls.Add(this.tb_tpd_Density);
			this.Controls.Add(this.tb_tpd_Pressure);
			this.Controls.Add(this.tb_tpd_Temperature);
			this.Controls.Add(this.dd_Density);
			this.Controls.Add(this.dd_Pressure);
			this.Controls.Add(this.dd_Temperature);
			this.Controls.Add(this.lblDensity);
			this.Controls.Add(this.lblPressure);
			this.Controls.Add(this.lblTemperature);
			this.Name = "CpEtkTempPressureDensity";
			this.Size = new System.Drawing.Size(525, 136);
			this.Load += new System.EventHandler(this.cpEtkTempPressureDensity_Load);
			this.gbCalculateOpts.ResumeLayout(false);
			this.gbCalculateOpts.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.GroupBox gbCalculateOpts;
		private System.Windows.Forms.RadioButton rb_Temperature;
		private System.Windows.Forms.RadioButton rb_Density;
		private System.Windows.Forms.RadioButton rb_Pressure;
		private System.Windows.Forms.Button btn_Calculate;
		private System.Windows.Forms.TextBox tb_tpd_Density;
		private System.Windows.Forms.TextBox tb_tpd_Pressure;
		private System.Windows.Forms.TextBox tb_tpd_Temperature;
		private ValueConverterDropdown dd_Density;
		private ValueConverterDropdown dd_Pressure;
		private ValueConverterDropdown dd_Temperature;
		private System.Windows.Forms.Label lblDensity;
		private System.Windows.Forms.Label lblPressure;
		private System.Windows.Forms.Label lblTemperature;
	}
}
