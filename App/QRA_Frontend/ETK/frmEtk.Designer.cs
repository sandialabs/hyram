namespace QRA_Frontend.ETK {
	partial class FrmEtk {
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.tcEtkMain = new System.Windows.Forms.TabControl();
			this.tpTPDCalculator = new System.Windows.Forms.TabPage();
			this.gbTPDCalc = new System.Windows.Forms.GroupBox();
			this.tpTankMass = new System.Windows.Forms.TabPage();
			this.tpMassFlowRate = new System.Windows.Forms.TabPage();
			this.tpTNTEquivalencyModel = new System.Windows.Forms.TabPage();
			this.cpEtkTempPressureDensity1 = new QRA_Frontend.ETK.ETKWrap.ContentPanels.CpEtkTempPressureDensity();
			this.cpEtkTankMass1 = new QRA_Frontend.ETK.ETKWrap.ContentPanels.CpEtkTankMass();
			this.cpEtkMassFlowRate1 = new QRA_Frontend.ETK.ETKWrap.ContentPanels.CpEtkMassFlowRate();
			this.cpEtkTNTMassEquiv1 = new QRA_Frontend.ETK.ETKWrap.ContentPanels.CpEtkTntMassEquiv();
			this.tcEtkMain.SuspendLayout();
			this.tpTPDCalculator.SuspendLayout();
			this.gbTPDCalc.SuspendLayout();
			this.tpTankMass.SuspendLayout();
			this.tpMassFlowRate.SuspendLayout();
			this.tpTNTEquivalencyModel.SuspendLayout();
			this.SuspendLayout();
			// 
			// tcEtkMain
			// 
			this.tcEtkMain.Controls.Add(this.tpTPDCalculator);
			this.tcEtkMain.Controls.Add(this.tpTankMass);
			this.tcEtkMain.Controls.Add(this.tpMassFlowRate);
			this.tcEtkMain.Controls.Add(this.tpTNTEquivalencyModel);
			this.tcEtkMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tcEtkMain.Location = new System.Drawing.Point(0, 0);
			this.tcEtkMain.Name = "tcEtkMain";
			this.tcEtkMain.SelectedIndex = 0;
			this.tcEtkMain.Size = new System.Drawing.Size(552, 457);
			this.tcEtkMain.TabIndex = 0;
			// 
			// tpTPDCalculator
			// 
			this.tpTPDCalculator.Controls.Add(this.gbTPDCalc);
			this.tpTPDCalculator.Location = new System.Drawing.Point(4, 22);
			this.tpTPDCalculator.Name = "tpTPDCalculator";
			this.tpTPDCalculator.Padding = new System.Windows.Forms.Padding(3);
			this.tpTPDCalculator.Size = new System.Drawing.Size(544, 431);
			this.tpTPDCalculator.TabIndex = 1;
			this.tpTPDCalculator.Text = "Temperature, Pressure and Density";
			this.tpTPDCalculator.UseVisualStyleBackColor = true;
			// 
			// gbTPDCalc
			// 
			this.gbTPDCalc.Controls.Add(this.cpEtkTempPressureDensity1);
			this.gbTPDCalc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gbTPDCalc.Location = new System.Drawing.Point(3, 3);
			this.gbTPDCalc.Name = "gbTPDCalc";
			this.gbTPDCalc.Size = new System.Drawing.Size(538, 425);
			this.gbTPDCalc.TabIndex = 0;
			this.gbTPDCalc.TabStop = false;
			// 
			// tpTankMass
			// 
			this.tpTankMass.Controls.Add(this.cpEtkTankMass1);
			this.tpTankMass.Location = new System.Drawing.Point(4, 22);
			this.tpTankMass.Name = "tpTankMass";
			this.tpTankMass.Size = new System.Drawing.Size(544, 431);
			this.tpTankMass.TabIndex = 2;
			this.tpTankMass.Text = "Tank Mass";
			this.tpTankMass.UseVisualStyleBackColor = true;
			// 
			// tpMassFlowRate
			// 
			this.tpMassFlowRate.Controls.Add(this.cpEtkMassFlowRate1);
			this.tpMassFlowRate.Location = new System.Drawing.Point(4, 22);
			this.tpMassFlowRate.Name = "tpMassFlowRate";
			this.tpMassFlowRate.Size = new System.Drawing.Size(544, 431);
			this.tpMassFlowRate.TabIndex = 3;
			this.tpMassFlowRate.Text = "Mass Flow Rate";
			this.tpMassFlowRate.UseVisualStyleBackColor = true;
			// 
			// tpTNTEquivalencyModel
			// 
			this.tpTNTEquivalencyModel.Controls.Add(this.cpEtkTNTMassEquiv1);
			this.tpTNTEquivalencyModel.Location = new System.Drawing.Point(4, 22);
			this.tpTNTEquivalencyModel.Name = "tpTNTEquivalencyModel";
			this.tpTNTEquivalencyModel.Size = new System.Drawing.Size(544, 431);
			this.tpTNTEquivalencyModel.TabIndex = 4;
			this.tpTNTEquivalencyModel.Text = "TNT Mass Equivalence";
			this.tpTNTEquivalencyModel.UseVisualStyleBackColor = true;
			// 
			// cpEtkTempPressureDensity1
			// 
			this.cpEtkTempPressureDensity1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cpEtkTempPressureDensity1.Location = new System.Drawing.Point(3, 16);
			this.cpEtkTempPressureDensity1.Name = "cpEtkTempPressureDensity1";
			this.cpEtkTempPressureDensity1.Size = new System.Drawing.Size(532, 406);
			this.cpEtkTempPressureDensity1.TabIndex = 0;
			// 
			// cpEtkTankMass1
			// 
			this.cpEtkTankMass1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cpEtkTankMass1.Location = new System.Drawing.Point(0, 0);
			this.cpEtkTankMass1.Name = "cpEtkTankMass1";
			this.cpEtkTankMass1.Size = new System.Drawing.Size(544, 431);
			this.cpEtkTankMass1.TabIndex = 0;
			// 
			// cpEtkMassFlowRate1
			// 
			this.cpEtkMassFlowRate1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.cpEtkMassFlowRate1.Location = new System.Drawing.Point(0, 0);
			this.cpEtkMassFlowRate1.Name = "cpEtkMassFlowRate1";
			this.cpEtkMassFlowRate1.Size = new System.Drawing.Size(544, 431);
			this.cpEtkMassFlowRate1.TabIndex = 0;
			// 
			// cpEtkTNTMassEquiv1
			// 
			this.cpEtkTNTMassEquiv1.Location = new System.Drawing.Point(8, 20);
			this.cpEtkTNTMassEquiv1.Name = "cpEtkTNTMassEquiv1";
			this.cpEtkTNTMassEquiv1.Size = new System.Drawing.Size(520, 167);
			this.cpEtkTNTMassEquiv1.TabIndex = 0;
			// 
			// frmEtk
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(552, 457);
			this.Controls.Add(this.tcEtkMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "FrmEtk";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Engineering Toolkit";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmEtk_FormClosing);
			this.tcEtkMain.ResumeLayout(false);
			this.tpTPDCalculator.ResumeLayout(false);
			this.gbTPDCalc.ResumeLayout(false);
			this.tpTankMass.ResumeLayout(false);
			this.tpMassFlowRate.ResumeLayout(false);
			this.tpTNTEquivalencyModel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcEtkMain;
		private System.Windows.Forms.TabPage tpTPDCalculator;
		private System.Windows.Forms.GroupBox gbTPDCalc;
		private System.Windows.Forms.TabPage tpTankMass;
		private ETKWrap.ContentPanels.CpEtkTempPressureDensity cpEtkTempPressureDensity1;
		private ETKWrap.ContentPanels.CpEtkTankMass cpEtkTankMass1;
		private System.Windows.Forms.TabPage tpMassFlowRate;
		private ETKWrap.ContentPanels.CpEtkMassFlowRate cpEtkMassFlowRate1;
		private System.Windows.Forms.TabPage tpTNTEquivalencyModel;
		private ETKWrap.ContentPanels.CpEtkTntMassEquiv cpEtkTNTMassEquiv1;
	}
}