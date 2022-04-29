namespace SandiaNationalLaboratories.Hyram {
	partial class ConsequenceModelsForm {
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
            this.notionalNozzleSelector = new System.Windows.Forms.ComboBox();
            this.notionalNozzleLabel = new System.Windows.Forms.Label();
            this.modelSelectionTabControl = new System.Windows.Forms.TabControl();
            this.physicalConsequenceTab = new System.Windows.Forms.TabPage();
            this.tntInput = new System.Windows.Forms.TextBox();
            this.tntLabel = new System.Windows.Forms.Label();
            this.flameSpeedSelector = new System.Windows.Forms.ComboBox();
            this.flameSpeedLabel = new System.Windows.Forms.Label();
            this.overpMethodSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.fatalityConsequenceTab = new System.Windows.Forms.TabPage();
            this.exposureTimeUnitSelector = new System.Windows.Forms.ComboBox();
            this.thermalProbitSelector = new System.Windows.Forms.ComboBox();
            this.exposureTimeInput = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.exposureTimeLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.overpressureProbitSelector = new System.Windows.Forms.ComboBox();
            this.formWarning = new System.Windows.Forms.Label();
            this.modelSelectionTabControl.SuspendLayout();
            this.physicalConsequenceTab.SuspendLayout();
            this.fatalityConsequenceTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // notionalNozzleSelector
            // 
            this.notionalNozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.notionalNozzleSelector.FormattingEnabled = true;
            this.notionalNozzleSelector.IntegralHeight = false;
            this.notionalNozzleSelector.ItemHeight = 15;
            this.notionalNozzleSelector.Location = new System.Drawing.Point(199, 12);
            this.notionalNozzleSelector.Name = "notionalNozzleSelector";
            this.notionalNozzleSelector.Size = new System.Drawing.Size(248, 23);
            this.notionalNozzleSelector.TabIndex = 3;
            this.notionalNozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.notionalNozzleSelector_SelectionChangeCommitted);
            // 
            // notionalNozzleLabel
            // 
            this.notionalNozzleLabel.AutoSize = true;
            this.notionalNozzleLabel.Location = new System.Drawing.Point(10, 15);
            this.notionalNozzleLabel.Name = "notionalNozzleLabel";
            this.notionalNozzleLabel.Size = new System.Drawing.Size(133, 15);
            this.notionalNozzleLabel.TabIndex = 6;
            this.notionalNozzleLabel.Text = "Notional nozzle model:";
            // 
            // modelSelectionTabControl
            // 
            this.modelSelectionTabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.modelSelectionTabControl.Controls.Add(this.physicalConsequenceTab);
            this.modelSelectionTabControl.Controls.Add(this.fatalityConsequenceTab);
            this.modelSelectionTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelSelectionTabControl.Location = new System.Drawing.Point(0, 0);
            this.modelSelectionTabControl.Name = "modelSelectionTabControl";
            this.modelSelectionTabControl.SelectedIndex = 0;
            this.modelSelectionTabControl.Size = new System.Drawing.Size(990, 517);
            this.modelSelectionTabControl.TabIndex = 9;
            // 
            // physicalConsequenceTab
            // 
            this.physicalConsequenceTab.Controls.Add(this.tntInput);
            this.physicalConsequenceTab.Controls.Add(this.tntLabel);
            this.physicalConsequenceTab.Controls.Add(this.flameSpeedSelector);
            this.physicalConsequenceTab.Controls.Add(this.flameSpeedLabel);
            this.physicalConsequenceTab.Controls.Add(this.overpMethodSelector);
            this.physicalConsequenceTab.Controls.Add(this.label1);
            this.physicalConsequenceTab.Controls.Add(this.notionalNozzleSelector);
            this.physicalConsequenceTab.Controls.Add(this.notionalNozzleLabel);
            this.physicalConsequenceTab.Location = new System.Drawing.Point(4, 24);
            this.physicalConsequenceTab.Name = "physicalConsequenceTab";
            this.physicalConsequenceTab.Padding = new System.Windows.Forms.Padding(3);
            this.physicalConsequenceTab.Size = new System.Drawing.Size(982, 489);
            this.physicalConsequenceTab.TabIndex = 0;
            this.physicalConsequenceTab.Text = "Physical Consequence Models";
            this.physicalConsequenceTab.UseVisualStyleBackColor = true;
            // 
            // tntInput
            // 
            this.tntInput.Location = new System.Drawing.Point(199, 99);
            this.tntInput.Name = "tntInput";
            this.tntInput.Size = new System.Drawing.Size(248, 21);
            this.tntInput.TabIndex = 68;
            this.tntInput.TextChanged += new System.EventHandler(this.tntInput_TextChanged);
            // 
            // tntLabel
            // 
            this.tntLabel.AutoSize = true;
            this.tntLabel.Location = new System.Drawing.Point(11, 102);
            this.tntLabel.Name = "tntLabel";
            this.tntLabel.Size = new System.Drawing.Size(132, 15);
            this.tntLabel.TabIndex = 67;
            this.tntLabel.Text = "TNT equivalence factor";
            // 
            // flameSpeedSelector
            // 
            this.flameSpeedSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flameSpeedSelector.FormattingEnabled = true;
            this.flameSpeedSelector.Location = new System.Drawing.Point(199, 70);
            this.flameSpeedSelector.Name = "flameSpeedSelector";
            this.flameSpeedSelector.Size = new System.Drawing.Size(248, 23);
            this.flameSpeedSelector.TabIndex = 66;
            this.flameSpeedSelector.SelectionChangeCommitted += new System.EventHandler(this.flameSpeedSelector_SelectionChangeCommitted);
            // 
            // flameSpeedLabel
            // 
            this.flameSpeedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flameSpeedLabel.Location = new System.Drawing.Point(10, 73);
            this.flameSpeedLabel.Name = "flameSpeedLabel";
            this.flameSpeedLabel.Size = new System.Drawing.Size(190, 18);
            this.flameSpeedLabel.TabIndex = 65;
            this.flameSpeedLabel.Text = "Mach flame speed";
            // 
            // overpMethodSelector
            // 
            this.overpMethodSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.overpMethodSelector.FormattingEnabled = true;
            this.overpMethodSelector.Location = new System.Drawing.Point(199, 41);
            this.overpMethodSelector.Name = "overpMethodSelector";
            this.overpMethodSelector.Size = new System.Drawing.Size(248, 23);
            this.overpMethodSelector.TabIndex = 64;
            this.overpMethodSelector.SelectionChangeCommitted += new System.EventHandler(this.overpMethodSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(147, 18);
            this.label1.TabIndex = 63;
            this.label1.Text = "Overpressure method";
            // 
            // fatalityConsequenceTab
            // 
            this.fatalityConsequenceTab.Controls.Add(this.exposureTimeUnitSelector);
            this.fatalityConsequenceTab.Controls.Add(this.thermalProbitSelector);
            this.fatalityConsequenceTab.Controls.Add(this.exposureTimeInput);
            this.fatalityConsequenceTab.Controls.Add(this.label4);
            this.fatalityConsequenceTab.Controls.Add(this.exposureTimeLabel);
            this.fatalityConsequenceTab.Controls.Add(this.label3);
            this.fatalityConsequenceTab.Controls.Add(this.overpressureProbitSelector);
            this.fatalityConsequenceTab.Location = new System.Drawing.Point(4, 24);
            this.fatalityConsequenceTab.Name = "fatalityConsequenceTab";
            this.fatalityConsequenceTab.Padding = new System.Windows.Forms.Padding(3);
            this.fatalityConsequenceTab.Size = new System.Drawing.Size(982, 489);
            this.fatalityConsequenceTab.TabIndex = 1;
            this.fatalityConsequenceTab.Text = "Harm Models";
            this.fatalityConsequenceTab.UseVisualStyleBackColor = true;
            // 
            // exposureTimeUnitSelector
            // 
            this.exposureTimeUnitSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.exposureTimeUnitSelector.FormattingEnabled = true;
            this.exposureTimeUnitSelector.Location = new System.Drawing.Point(320, 41);
            this.exposureTimeUnitSelector.Name = "exposureTimeUnitSelector";
            this.exposureTimeUnitSelector.Size = new System.Drawing.Size(127, 23);
            this.exposureTimeUnitSelector.TabIndex = 21;
            this.exposureTimeUnitSelector.SelectedIndexChanged += new System.EventHandler(this.exposureTimeUnitSelector_SelectedIndexChanged);
            // 
            // thermalProbitSelector
            // 
            this.thermalProbitSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.thermalProbitSelector.FormattingEnabled = true;
            this.thermalProbitSelector.Items.AddRange(new object[] {
            "Eisenberg",
            "Tsao",
            "TNO",
            "Lees"});
            this.thermalProbitSelector.Location = new System.Drawing.Point(199, 12);
            this.thermalProbitSelector.Name = "thermalProbitSelector";
            this.thermalProbitSelector.Size = new System.Drawing.Size(248, 23);
            this.thermalProbitSelector.TabIndex = 15;
            this.thermalProbitSelector.SelectionChangeCommitted += new System.EventHandler(this.thermalProbitSelector_SelectionChangeCommotted);
            // 
            // exposureTimeInput
            // 
            this.exposureTimeInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.exposureTimeInput.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.exposureTimeInput.Location = new System.Drawing.Point(199, 41);
            this.exposureTimeInput.MaximumSize = new System.Drawing.Size(117, 26);
            this.exposureTimeInput.MinimumSize = new System.Drawing.Size(2, 26);
            this.exposureTimeInput.Name = "exposureTimeInput";
            this.exposureTimeInput.Size = new System.Drawing.Size(116, 21);
            this.exposureTimeInput.TabIndex = 20;
            this.exposureTimeInput.TextChanged += new System.EventHandler(this.exposureTimeInput_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 15);
            this.label4.TabIndex = 17;
            this.label4.Text = "Thermal Probit Model:";
            // 
            // exposureTimeLabel
            // 
            this.exposureTimeLabel.AutoSize = true;
            this.exposureTimeLabel.Location = new System.Drawing.Point(10, 44);
            this.exposureTimeLabel.Name = "exposureTimeLabel";
            this.exposureTimeLabel.Size = new System.Drawing.Size(142, 15);
            this.exposureTimeLabel.TabIndex = 19;
            this.exposureTimeLabel.Text = "Thermal Exposure Time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 15);
            this.label3.TabIndex = 18;
            this.label3.Text = "Overpressure Probit Model:";
            // 
            // overpressureProbitSelector
            // 
            this.overpressureProbitSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.overpressureProbitSelector.FormattingEnabled = true;
            this.overpressureProbitSelector.Items.AddRange(new object[] {
            "Lung Eisenberg",
            "Lung HSE",
            "Head impact",
            "Collapse"});
            this.overpressureProbitSelector.Location = new System.Drawing.Point(199, 69);
            this.overpressureProbitSelector.Name = "overpressureProbitSelector";
            this.overpressureProbitSelector.Size = new System.Drawing.Size(248, 23);
            this.overpressureProbitSelector.TabIndex = 16;
            this.overpressureProbitSelector.SelectionChangeCommitted += new System.EventHandler(this.overpressureProbitSelector_SelectionChangeCommotted);
            // 
            // formWarning
            // 
            this.formWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.formWarning.AutoSize = true;
            this.formWarning.BackColor = System.Drawing.Color.MistyRose;
            this.formWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formWarning.ForeColor = System.Drawing.Color.Maroon;
            this.formWarning.Location = new System.Drawing.Point(3, 520);
            this.formWarning.MaximumSize = new System.Drawing.Size(700, 0);
            this.formWarning.Name = "formWarning";
            this.formWarning.Padding = new System.Windows.Forms.Padding(4);
            this.formWarning.Size = new System.Drawing.Size(177, 23);
            this.formWarning.TabIndex = 19;
            this.formWarning.Text = "Errors and warnings here";
            this.formWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConsequenceModelsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.formWarning);
            this.Controls.Add(this.modelSelectionTabControl);
            this.Name = "ConsequenceModelsForm";
            this.Size = new System.Drawing.Size(990, 595);
            this.modelSelectionTabControl.ResumeLayout(false);
            this.physicalConsequenceTab.ResumeLayout(false);
            this.physicalConsequenceTab.PerformLayout();
            this.fatalityConsequenceTab.ResumeLayout(false);
            this.fatalityConsequenceTab.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox notionalNozzleSelector;
		private System.Windows.Forms.Label notionalNozzleLabel;
		private System.Windows.Forms.TabControl modelSelectionTabControl;
		private System.Windows.Forms.TabPage physicalConsequenceTab;
		private System.Windows.Forms.TabPage fatalityConsequenceTab;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox overpressureProbitSelector;
        private System.Windows.Forms.ComboBox exposureTimeUnitSelector;
        private System.Windows.Forms.ComboBox thermalProbitSelector;
        private System.Windows.Forms.TextBox exposureTimeInput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label exposureTimeLabel;
        private System.Windows.Forms.Label formWarning;
        private System.Windows.Forms.ComboBox overpMethodSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox flameSpeedSelector;
        private System.Windows.Forms.Label flameSpeedLabel;
        private System.Windows.Forms.TextBox tntInput;
        private System.Windows.Forms.Label tntLabel;
    }
}
