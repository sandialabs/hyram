
namespace SandiaNationalLaboratories.Hyram {
	partial class EtkMainForm {


		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.etkTabControl = new System.Windows.Forms.TabControl();
            this.temperaturePressureDensityTabPage = new System.Windows.Forms.TabPage();
            this.temperaturePressureDensityForm = new SandiaNationalLaboratories.Hyram.TemperaturePressureDensityForm();
            this.tankMassTabPage = new System.Windows.Forms.TabPage();
            this.tankMassForm = new SandiaNationalLaboratories.Hyram.TankMassForm();
            this.massFlowRateTabPage = new System.Windows.Forms.TabPage();
            this.massFlowRateForm = new SandiaNationalLaboratories.Hyram.MassFlowRateForm();
            this.tntEquivalenceTabPage = new System.Windows.Forms.TabPage();
            this.tntEquivalenceForm = new SandiaNationalLaboratories.Hyram.TntEquivalenceForm();
            this.etkTabControl.SuspendLayout();
            this.temperaturePressureDensityTabPage.SuspendLayout();
            this.tankMassTabPage.SuspendLayout();
            this.massFlowRateTabPage.SuspendLayout();
            this.tntEquivalenceTabPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // etkTabControl
            // 
            this.etkTabControl.Controls.Add(this.temperaturePressureDensityTabPage);
            this.etkTabControl.Controls.Add(this.tankMassTabPage);
            this.etkTabControl.Controls.Add(this.massFlowRateTabPage);
            this.etkTabControl.Controls.Add(this.tntEquivalenceTabPage);
            this.etkTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.etkTabControl.Location = new System.Drawing.Point(0, 0);
            this.etkTabControl.Name = "etkTabControl";
            this.etkTabControl.SelectedIndex = 0;
            this.etkTabControl.Size = new System.Drawing.Size(535, 457);
            this.etkTabControl.TabIndex = 0;
            // 
            // temperaturePressureDensityTabPage
            // 
            this.temperaturePressureDensityTabPage.Controls.Add(this.temperaturePressureDensityForm);
            this.temperaturePressureDensityTabPage.Location = new System.Drawing.Point(4, 22);
            this.temperaturePressureDensityTabPage.Name = "temperaturePressureDensityTabPage";
            this.temperaturePressureDensityTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.temperaturePressureDensityTabPage.Size = new System.Drawing.Size(527, 431);
            this.temperaturePressureDensityTabPage.TabIndex = 1;
            this.temperaturePressureDensityTabPage.Text = "Temperature, Pressure and Density";
            this.temperaturePressureDensityTabPage.UseVisualStyleBackColor = true;
            // 
            // temperaturePressureDensityForm
            // 
            this.temperaturePressureDensityForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.temperaturePressureDensityForm.Location = new System.Drawing.Point(3, 3);
            this.temperaturePressureDensityForm.Name = "temperaturePressureDensityForm";
            this.temperaturePressureDensityForm.Size = new System.Drawing.Size(521, 425);
            this.temperaturePressureDensityForm.TabIndex = 0;
            // 
            // tankMassTabPage
            // 
            this.tankMassTabPage.Controls.Add(this.tankMassForm);
            this.tankMassTabPage.Location = new System.Drawing.Point(4, 22);
            this.tankMassTabPage.Name = "tankMassTabPage";
            this.tankMassTabPage.Size = new System.Drawing.Size(527, 431);
            this.tankMassTabPage.TabIndex = 2;
            this.tankMassTabPage.Text = "Tank Mass";
            this.tankMassTabPage.UseVisualStyleBackColor = true;
            this.tankMassTabPage.Enter += new System.EventHandler(this.tankMassTabPage_Enter);
            // 
            // tankMassForm
            // 
            this.tankMassForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tankMassForm.Location = new System.Drawing.Point(0, 0);
            this.tankMassForm.Name = "tankMassForm";
            this.tankMassForm.Size = new System.Drawing.Size(527, 431);
            this.tankMassForm.TabIndex = 0;
            // 
            // massFlowRateTabPage
            // 
            this.massFlowRateTabPage.Controls.Add(this.massFlowRateForm);
            this.massFlowRateTabPage.Location = new System.Drawing.Point(4, 22);
            this.massFlowRateTabPage.Name = "massFlowRateTabPage";
            this.massFlowRateTabPage.Size = new System.Drawing.Size(527, 431);
            this.massFlowRateTabPage.TabIndex = 3;
            this.massFlowRateTabPage.Text = "Mass Flow Rate";
            this.massFlowRateTabPage.UseVisualStyleBackColor = true;
            this.massFlowRateTabPage.Enter += new System.EventHandler(this.massFlowRateTabPage_Enter);
            // 
            // massFlowRateForm
            // 
            this.massFlowRateForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.massFlowRateForm.Location = new System.Drawing.Point(0, 0);
            this.massFlowRateForm.Name = "massFlowRateForm";
            this.massFlowRateForm.Size = new System.Drawing.Size(527, 431);
            this.massFlowRateForm.TabIndex = 0;
            // 
            // tntEquivalenceTabPage
            // 
            this.tntEquivalenceTabPage.Controls.Add(this.tntEquivalenceForm);
            this.tntEquivalenceTabPage.Location = new System.Drawing.Point(4, 22);
            this.tntEquivalenceTabPage.Name = "tntEquivalenceTabPage";
            this.tntEquivalenceTabPage.Size = new System.Drawing.Size(527, 431);
            this.tntEquivalenceTabPage.TabIndex = 4;
            this.tntEquivalenceTabPage.Text = "TNT Mass Equivalence";
            this.tntEquivalenceTabPage.UseVisualStyleBackColor = true;
            // 
            // tntEquivalenceForm
            // 
            this.tntEquivalenceForm.Location = new System.Drawing.Point(8, 20);
            this.tntEquivalenceForm.Name = "tntEquivalenceForm";
            this.tntEquivalenceForm.Size = new System.Drawing.Size(520, 167);
            this.tntEquivalenceForm.TabIndex = 0;
            // 
            // EtkMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(535, 457);
            this.Controls.Add(this.etkTabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "EtkMainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Engineering Toolkit";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EtkMainForm_FormClosing);
            this.etkTabControl.ResumeLayout(false);
            this.temperaturePressureDensityTabPage.ResumeLayout(false);
            this.tankMassTabPage.ResumeLayout(false);
            this.massFlowRateTabPage.ResumeLayout(false);
            this.tntEquivalenceTabPage.ResumeLayout(false);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl etkTabControl;
		private System.Windows.Forms.TabPage temperaturePressureDensityTabPage;
		private System.Windows.Forms.TabPage tankMassTabPage;
		private System.Windows.Forms.TabPage massFlowRateTabPage;
		private System.Windows.Forms.TabPage tntEquivalenceTabPage;
		private TemperaturePressureDensityForm temperaturePressureDensityForm;
		private TankMassForm tankMassForm;
		private MassFlowRateForm massFlowRateForm;
		private TntEquivalenceForm tntEquivalenceForm;
	}
}