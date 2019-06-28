namespace QRA_Frontend.ContentPanels {
	partial class cp0Dot1Dot0Test {
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
			this.gbSysParam = new System.Windows.Forms.GroupBox();
			this.tbExternalTempC = new System.Windows.Forms.TextBox();
			this.tbExternalPresMPa = new System.Windows.Forms.TextBox();
			this.tbInternalTempC = new System.Windows.Forms.TextBox();
			this.tbInternalPresMPa = new System.Windows.Forms.TextBox();
			this.tbPipeWallThick = new System.Windows.Forms.TextBox();
			this.tbPipeOD = new System.Windows.Forms.TextBox();
			this.lblPipeWallThickness = new System.Windows.Forms.Label();
			this.lblInternalPresMPa = new System.Windows.Forms.Label();
			this.lblInternalTempC = new System.Windows.Forms.Label();
			this.lblExternalPresMPa = new System.Windows.Forms.Label();
			this.lblExternalTempC = new System.Windows.Forms.Label();
			this.lblPipeOD = new System.Windows.Forms.Label();
			this.rbUseDefaults = new System.Windows.Forms.RadioButton();
			this.rbUseMostCurrent = new System.Windows.Forms.RadioButton();
			this.tcBoundaryConditions = new System.Windows.Forms.TabControl();
			this.tpVehicles = new System.Windows.Forms.TabPage();
			this.tbNVehicleOperatingDays = new System.Windows.Forms.TextBox();
			this.lblNrVehicleOperatingDays = new System.Windows.Forms.Label();
			this.tbNrFuelingsPerVehicleDay = new System.Windows.Forms.TextBox();
			this.lblnfuelingspervehicleday = new System.Windows.Forms.Label();
			this.tbNvehicles = new System.Windows.Forms.TextBox();
			this.lblNVehicles = new System.Windows.Forms.Label();
			this.tpSysParam = new System.Windows.Forms.TabPage();
			this.btnRefreshShared = new System.Windows.Forms.Button();
			this.gbSharedData = new System.Windows.Forms.GroupBox();
			this.btnExecute = new System.Windows.Forms.Button();
			this.gbSysParam.SuspendLayout();
			this.tcBoundaryConditions.SuspendLayout();
			this.tpVehicles.SuspendLayout();
			this.tpSysParam.SuspendLayout();
			this.gbSharedData.SuspendLayout();
			this.SuspendLayout();
			// 
			// gbSysParam
			// 
			this.gbSysParam.Controls.Add(this.tbExternalTempC);
			this.gbSysParam.Controls.Add(this.tbExternalPresMPa);
			this.gbSysParam.Controls.Add(this.tbInternalTempC);
			this.gbSysParam.Controls.Add(this.tbInternalPresMPa);
			this.gbSysParam.Controls.Add(this.tbPipeWallThick);
			this.gbSysParam.Controls.Add(this.tbPipeOD);
			this.gbSysParam.Controls.Add(this.lblPipeWallThickness);
			this.gbSysParam.Controls.Add(this.lblInternalPresMPa);
			this.gbSysParam.Controls.Add(this.lblInternalTempC);
			this.gbSysParam.Controls.Add(this.lblExternalPresMPa);
			this.gbSysParam.Controls.Add(this.lblExternalTempC);
			this.gbSysParam.Controls.Add(this.lblPipeOD);
			this.gbSysParam.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbSysParam.Location = new System.Drawing.Point(3, 3);
			this.gbSysParam.Name = "gbSysParam";
			this.gbSysParam.Size = new System.Drawing.Size(274, 207);
			this.gbSysParam.TabIndex = 0;
			this.gbSysParam.TabStop = false;
			this.gbSysParam.Text = "SysParam";
			// 
			// tbExternalTempC
			// 
			this.tbExternalTempC.Location = new System.Drawing.Point(126, 149);
			this.tbExternalTempC.Name = "tbExternalTempC";
			this.tbExternalTempC.Size = new System.Drawing.Size(71, 20);
			this.tbExternalTempC.TabIndex = 14;
			this.tbExternalTempC.Tag = "SysParam.ExternalTempC";
			this.tbExternalTempC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbExternalTempC.TextChanged += new System.EventHandler(this.tbExternalTempC_TextChanged);
			// 
			// tbExternalPresMPa
			// 
			this.tbExternalPresMPa.Location = new System.Drawing.Point(126, 120);
			this.tbExternalPresMPa.Name = "tbExternalPresMPa";
			this.tbExternalPresMPa.Size = new System.Drawing.Size(71, 20);
			this.tbExternalPresMPa.TabIndex = 13;
			this.tbExternalPresMPa.Tag = "SysParam.ExternalPresMPa";
			this.tbExternalPresMPa.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbExternalPresMPa.TextChanged += new System.EventHandler(this.tbExternalPresMPa_TextChanged);
			// 
			// tbInternalTempC
			// 
			this.tbInternalTempC.Location = new System.Drawing.Point(126, 92);
			this.tbInternalTempC.Name = "tbInternalTempC";
			this.tbInternalTempC.Size = new System.Drawing.Size(71, 20);
			this.tbInternalTempC.TabIndex = 12;
			this.tbInternalTempC.Tag = "SysParam.InternalTempC";
			this.tbInternalTempC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbInternalTempC.TextChanged += new System.EventHandler(this.tbInternalTempC_TextChanged);
			// 
			// tbInternalPresMPa
			// 
			this.tbInternalPresMPa.Location = new System.Drawing.Point(126, 66);
			this.tbInternalPresMPa.Name = "tbInternalPresMPa";
			this.tbInternalPresMPa.Size = new System.Drawing.Size(71, 20);
			this.tbInternalPresMPa.TabIndex = 11;
			this.tbInternalPresMPa.Tag = "SysParam.InternalPresMPa";
			this.tbInternalPresMPa.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbInternalPresMPa.TextChanged += new System.EventHandler(this.tbInternalPresMPa_TextChanged);
			// 
			// tbPipeWallThick
			// 
			this.tbPipeWallThick.Location = new System.Drawing.Point(126, 40);
			this.tbPipeWallThick.Name = "tbPipeWallThick";
			this.tbPipeWallThick.Size = new System.Drawing.Size(71, 20);
			this.tbPipeWallThick.TabIndex = 10;
			this.tbPipeWallThick.Tag = "SysParam.PipeWallThick";
			this.tbPipeWallThick.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbPipeWallThick.TextChanged += new System.EventHandler(this.tbPipeWallThick_TextChanged);
			// 
			// tbPipeOD
			// 
			this.tbPipeOD.Location = new System.Drawing.Point(126, 14);
			this.tbPipeOD.Name = "tbPipeOD";
			this.tbPipeOD.Size = new System.Drawing.Size(71, 20);
			this.tbPipeOD.TabIndex = 9;
			this.tbPipeOD.Tag = "SysParam.PipeOD";
			this.tbPipeOD.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.tbPipeOD.TextChanged += new System.EventHandler(this.tbPipeOD_TextChanged);
			// 
			// lblPipeWallThickness
			// 
			this.lblPipeWallThickness.AutoSize = true;
			this.lblPipeWallThickness.Location = new System.Drawing.Point(18, 47);
			this.lblPipeWallThickness.Name = "lblPipeWallThickness";
			this.lblPipeWallThickness.Size = new System.Drawing.Size(79, 13);
			this.lblPipeWallThickness.TabIndex = 8;
			this.lblPipeWallThickness.Text = "PipeWallThick:";
			// 
			// lblInternalPresMPa
			// 
			this.lblInternalPresMPa.AutoSize = true;
			this.lblInternalPresMPa.Location = new System.Drawing.Point(18, 73);
			this.lblInternalPresMPa.Name = "lblInternalPresMPa";
			this.lblInternalPresMPa.Size = new System.Drawing.Size(88, 13);
			this.lblInternalPresMPa.TabIndex = 7;
			this.lblInternalPresMPa.Text = "InternalPresMPa:";
			// 
			// lblInternalTempC
			// 
			this.lblInternalTempC.AutoSize = true;
			this.lblInternalTempC.Location = new System.Drawing.Point(18, 99);
			this.lblInternalTempC.Name = "lblInternalTempC";
			this.lblInternalTempC.Size = new System.Drawing.Size(79, 13);
			this.lblInternalTempC.TabIndex = 6;
			this.lblInternalTempC.Text = "InternalTempC:";
			// 
			// lblExternalPresMPa
			// 
			this.lblExternalPresMPa.AutoSize = true;
			this.lblExternalPresMPa.Location = new System.Drawing.Point(18, 127);
			this.lblExternalPresMPa.Name = "lblExternalPresMPa";
			this.lblExternalPresMPa.Size = new System.Drawing.Size(91, 13);
			this.lblExternalPresMPa.TabIndex = 5;
			this.lblExternalPresMPa.Text = "ExternalPresMPa:";
			// 
			// lblExternalTempC
			// 
			this.lblExternalTempC.AutoSize = true;
			this.lblExternalTempC.Location = new System.Drawing.Point(18, 149);
			this.lblExternalTempC.Name = "lblExternalTempC";
			this.lblExternalTempC.Size = new System.Drawing.Size(82, 13);
			this.lblExternalTempC.TabIndex = 4;
			this.lblExternalTempC.Text = "ExternalTempC:";
			// 
			// lblPipeOD
			// 
			this.lblPipeOD.AutoSize = true;
			this.lblPipeOD.Location = new System.Drawing.Point(18, 21);
			this.lblPipeOD.Name = "lblPipeOD";
			this.lblPipeOD.Size = new System.Drawing.Size(47, 13);
			this.lblPipeOD.TabIndex = 2;
			this.lblPipeOD.Text = "PipeOD:";
			// 
			// rbUseDefaults
			// 
			this.rbUseDefaults.AutoSize = true;
			this.rbUseDefaults.Location = new System.Drawing.Point(6, 31);
			this.rbUseDefaults.Name = "rbUseDefaults";
			this.rbUseDefaults.Size = new System.Drawing.Size(86, 17);
			this.rbUseDefaults.TabIndex = 0;
			this.rbUseDefaults.Text = "Use Defaults";
			this.rbUseDefaults.UseVisualStyleBackColor = true;
			this.rbUseDefaults.CheckedChanged += new System.EventHandler(this.rbUseDefaults_CheckedChanged);
			// 
			// rbUseMostCurrent
			// 
			this.rbUseMostCurrent.AutoSize = true;
			this.rbUseMostCurrent.Checked = true;
			this.rbUseMostCurrent.Location = new System.Drawing.Point(97, 30);
			this.rbUseMostCurrent.Name = "rbUseMostCurrent";
			this.rbUseMostCurrent.Size = new System.Drawing.Size(107, 17);
			this.rbUseMostCurrent.TabIndex = 1;
			this.rbUseMostCurrent.TabStop = true;
			this.rbUseMostCurrent.Text = "Use Most Current";
			this.rbUseMostCurrent.UseVisualStyleBackColor = true;
			this.rbUseMostCurrent.CheckedChanged += new System.EventHandler(this.rbUseMostCurrent_CheckedChanged);
			// 
			// tcBoundaryConditions
			// 
			this.tcBoundaryConditions.Controls.Add(this.tpVehicles);
			this.tcBoundaryConditions.Controls.Add(this.tpSysParam);
			this.tcBoundaryConditions.Location = new System.Drawing.Point(7, 101);
			this.tcBoundaryConditions.Name = "tcBoundaryConditions";
			this.tcBoundaryConditions.SelectedIndex = 0;
			this.tcBoundaryConditions.Size = new System.Drawing.Size(288, 239);
			this.tcBoundaryConditions.TabIndex = 1;
			// 
			// tpVehicles
			// 
			this.tpVehicles.Controls.Add(this.tbNVehicleOperatingDays);
			this.tpVehicles.Controls.Add(this.lblNrVehicleOperatingDays);
			this.tpVehicles.Controls.Add(this.tbNrFuelingsPerVehicleDay);
			this.tpVehicles.Controls.Add(this.lblnfuelingspervehicleday);
			this.tpVehicles.Controls.Add(this.tbNvehicles);
			this.tpVehicles.Controls.Add(this.lblNVehicles);
			this.tpVehicles.Location = new System.Drawing.Point(4, 22);
			this.tpVehicles.Name = "tpVehicles";
			this.tpVehicles.Padding = new System.Windows.Forms.Padding(3);
			this.tpVehicles.Size = new System.Drawing.Size(280, 213);
			this.tpVehicles.TabIndex = 0;
			this.tpVehicles.Text = "Vehicles";
			this.tpVehicles.UseVisualStyleBackColor = true;
			// 
			// tbNVehicleOperatingDays
			// 
			this.tbNVehicleOperatingDays.Location = new System.Drawing.Point(130, 69);
			this.tbNVehicleOperatingDays.Name = "tbNVehicleOperatingDays";
			this.tbNVehicleOperatingDays.Size = new System.Drawing.Size(100, 20);
			this.tbNVehicleOperatingDays.TabIndex = 5;
			this.tbNVehicleOperatingDays.Tag = "nvehicleoperatingdays";
			this.tbNVehicleOperatingDays.TextChanged += new System.EventHandler(this.tbNVehicleOperatingDays_TextChanged);
			// 
			// lblNrVehicleOperatingDays
			// 
			this.lblNrVehicleOperatingDays.AutoSize = true;
			this.lblNrVehicleOperatingDays.Location = new System.Drawing.Point(7, 72);
			this.lblNrVehicleOperatingDays.Name = "lblNrVehicleOperatingDays";
			this.lblNrVehicleOperatingDays.Size = new System.Drawing.Size(118, 13);
			this.lblNrVehicleOperatingDays.TabIndex = 4;
			this.lblNrVehicleOperatingDays.Text = "# Veh. Operating Days:";
			// 
			// tbNrFuelingsPerVehicleDay
			// 
			this.tbNrFuelingsPerVehicleDay.Location = new System.Drawing.Point(131, 43);
			this.tbNrFuelingsPerVehicleDay.Name = "tbNrFuelingsPerVehicleDay";
			this.tbNrFuelingsPerVehicleDay.Size = new System.Drawing.Size(100, 20);
			this.tbNrFuelingsPerVehicleDay.TabIndex = 3;
			this.tbNrFuelingsPerVehicleDay.Tag = "nfuelingspervehicleday";
			this.tbNrFuelingsPerVehicleDay.TextChanged += new System.EventHandler(this.tbNrFuelingsPerVehicleDay_TextChanged);
			// 
			// lblnfuelingspervehicleday
			// 
			this.lblnfuelingspervehicleday.AutoSize = true;
			this.lblnfuelingspervehicleday.Location = new System.Drawing.Point(7, 46);
			this.lblnfuelingspervehicleday.Name = "lblnfuelingspervehicleday";
			this.lblnfuelingspervehicleday.Size = new System.Drawing.Size(108, 13);
			this.lblnfuelingspervehicleday.TabIndex = 2;
			this.lblnfuelingspervehicleday.Text = "# Fuelings/Veh. Day:";
			// 
			// tbNvehicles
			// 
			this.tbNvehicles.Location = new System.Drawing.Point(132, 17);
			this.tbNvehicles.Name = "tbNvehicles";
			this.tbNvehicles.Size = new System.Drawing.Size(100, 20);
			this.tbNvehicles.TabIndex = 1;
			this.tbNvehicles.Tag = "nvehicles";
			this.tbNvehicles.TextChanged += new System.EventHandler(this.tbNvehicles_TextChanged);
			// 
			// lblNVehicles
			// 
			this.lblNVehicles.AutoSize = true;
			this.lblNVehicles.Location = new System.Drawing.Point(7, 20);
			this.lblNVehicles.Name = "lblNVehicles";
			this.lblNVehicles.Size = new System.Drawing.Size(57, 13);
			this.lblNVehicles.TabIndex = 0;
			this.lblNVehicles.Text = "# Vehicles";
			// 
			// tpSysParam
			// 
			this.tpSysParam.Controls.Add(this.gbSysParam);
			this.tpSysParam.Location = new System.Drawing.Point(4, 22);
			this.tpSysParam.Name = "tpSysParam";
			this.tpSysParam.Padding = new System.Windows.Forms.Padding(3);
			this.tpSysParam.Size = new System.Drawing.Size(280, 213);
			this.tpSysParam.TabIndex = 1;
			this.tpSysParam.Text = "System Parameters (SysParam)";
			this.tpSysParam.UseVisualStyleBackColor = true;
			// 
			// btnRefreshShared
			// 
			this.btnRefreshShared.Location = new System.Drawing.Point(6, 54);
			this.btnRefreshShared.Name = "btnRefreshShared";
			this.btnRefreshShared.Size = new System.Drawing.Size(183, 23);
			this.btnRefreshShared.TabIndex = 3;
			this.btnRefreshShared.Text = "Refresh inputs from database";
			this.btnRefreshShared.UseVisualStyleBackColor = true;
			this.btnRefreshShared.Click += new System.EventHandler(this.btnRefreshShared_Click);
			// 
			// gbSharedData
			// 
			this.gbSharedData.Controls.Add(this.btnRefreshShared);
			this.gbSharedData.Controls.Add(this.rbUseDefaults);
			this.gbSharedData.Controls.Add(this.rbUseMostCurrent);
			this.gbSharedData.Location = new System.Drawing.Point(7, 3);
			this.gbSharedData.Name = "gbSharedData";
			this.gbSharedData.Size = new System.Drawing.Size(284, 92);
			this.gbSharedData.TabIndex = 4;
			this.gbSharedData.TabStop = false;
			this.gbSharedData.Text = "Shared/External Data";
			// 
			// btnExecute
			// 
			this.btnExecute.Location = new System.Drawing.Point(13, 377);
			this.btnExecute.Name = "btnExecute";
			this.btnExecute.Size = new System.Drawing.Size(75, 23);
			this.btnExecute.TabIndex = 5;
			this.btnExecute.Text = "Test";
			this.btnExecute.UseVisualStyleBackColor = true;
			this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
			// 
			// cp0Dot1Dot0Test
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnExecute);
			this.Controls.Add(this.gbSharedData);
			this.Controls.Add(this.tcBoundaryConditions);
			this.Name = "cp0Dot1Dot0Test";
			this.Size = new System.Drawing.Size(308, 457);
			this.gbSysParam.ResumeLayout(false);
			this.gbSysParam.PerformLayout();
			this.tcBoundaryConditions.ResumeLayout(false);
			this.tpVehicles.ResumeLayout(false);
			this.tpVehicles.PerformLayout();
			this.tpSysParam.ResumeLayout(false);
			this.gbSharedData.ResumeLayout(false);
			this.gbSharedData.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox gbSysParam;
		private System.Windows.Forms.TextBox tbExternalTempC;
		private System.Windows.Forms.TextBox tbExternalPresMPa;
		private System.Windows.Forms.TextBox tbInternalTempC;
		private System.Windows.Forms.TextBox tbInternalPresMPa;
		private System.Windows.Forms.TextBox tbPipeWallThick;
		private System.Windows.Forms.TextBox tbPipeOD;
		private System.Windows.Forms.Label lblPipeWallThickness;
		private System.Windows.Forms.Label lblInternalPresMPa;
		private System.Windows.Forms.Label lblInternalTempC;
		private System.Windows.Forms.Label lblExternalPresMPa;
		private System.Windows.Forms.Label lblExternalTempC;
		private System.Windows.Forms.Label lblPipeOD;
		private System.Windows.Forms.RadioButton rbUseMostCurrent;
		private System.Windows.Forms.RadioButton rbUseDefaults;
		private System.Windows.Forms.TabControl tcBoundaryConditions;
		private System.Windows.Forms.TabPage tpVehicles;
		private System.Windows.Forms.TabPage tpSysParam;
		private System.Windows.Forms.Button btnRefreshShared;
		private System.Windows.Forms.GroupBox gbSharedData;
		private System.Windows.Forms.Label lblNVehicles;
		private System.Windows.Forms.TextBox tbNVehicleOperatingDays;
		private System.Windows.Forms.Label lblNrVehicleOperatingDays;
		private System.Windows.Forms.TextBox tbNrFuelingsPerVehicleDay;
		private System.Windows.Forms.Label lblnfuelingspervehicleday;
		private System.Windows.Forms.TextBox tbNvehicles;
		private System.Windows.Forms.Button btnExecute;

	}
}
