namespace SandiaNationalLaboratories.Hyram {
	partial class MainForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainContainer = new System.Windows.Forms.SplitContainer();
            this.divider = new System.Windows.Forms.Label();
            this.navContainer = new System.Windows.Forms.SplitContainer();
            this.modeTabs = new System.Windows.Forms.TabControl();
            this.qraModeTab = new System.Windows.Forms.TabPage();
            this.qraNavPanel = new System.Windows.Forms.Panel();
            this.fuelTypeLabel2 = new System.Windows.Forms.Label();
            this.qraFuelTypeSelector = new System.Windows.Forms.ComboBox();
            this.scenariosFormButton = new System.Windows.Forms.Button();
            this.systemDescriptionFormButton = new System.Windows.Forms.Button();
            this.probabilitiesFormButton = new System.Windows.Forms.Button();
            this.consequenceModelsFormButton = new System.Windows.Forms.Button();
            this.physicsModeTab = new System.Windows.Forms.TabPage();
            this.physicsNavPanel = new System.Windows.Forms.Panel();
            this.jetHeatAnalysisFormButton = new System.Windows.Forms.Button();
            this.jetPlotTempFormButton = new System.Windows.Forms.Button();
            this.fuelTypeLabel1 = new System.Windows.Forms.Label();
            this.physicsFuelTypeSelector = new System.Windows.Forms.ComboBox();
            this.overpressureFormButton = new System.Windows.Forms.Button();
            this.plumeFormButton = new System.Windows.Forms.Button();
            this.mainOutputPanel = new System.Windows.Forms.Panel();
            this.mainOutputNavPanel = new System.Windows.Forms.Panel();
            this.mainOutputGroupBox = new System.Windows.Forms.GroupBox();
            this.mainOutputLabel = new System.Windows.Forms.Label();
            this.mainFormPanel = new System.Windows.Forms.Panel();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.fileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.etkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dataDirectoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.navContainer)).BeginInit();
            this.navContainer.Panel1.SuspendLayout();
            this.navContainer.Panel2.SuspendLayout();
            this.navContainer.SuspendLayout();
            this.modeTabs.SuspendLayout();
            this.qraModeTab.SuspendLayout();
            this.qraNavPanel.SuspendLayout();
            this.physicsModeTab.SuspendLayout();
            this.physicsNavPanel.SuspendLayout();
            this.mainOutputPanel.SuspendLayout();
            this.mainOutputGroupBox.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainContainer
            // 
            this.mainContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainContainer.Location = new System.Drawing.Point(0, 24);
            this.mainContainer.Name = "mainContainer";
            // 
            // mainContainer.Panel1
            // 
            this.mainContainer.Panel1.Controls.Add(this.divider);
            this.mainContainer.Panel1.Controls.Add(this.navContainer);
            this.mainContainer.Panel1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.mainContainer.Panel1MinSize = 150;
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.Controls.Add(this.mainFormPanel);
            this.mainContainer.Size = new System.Drawing.Size(1184, 705);
            this.mainContainer.SplitterDistance = 186;
            this.mainContainer.SplitterWidth = 6;
            this.mainContainer.TabIndex = 0;
            // 
            // divider
            // 
            this.divider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.divider.BackColor = System.Drawing.SystemColors.Control;
            this.divider.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.divider.ForeColor = System.Drawing.SystemColors.Control;
            this.divider.Location = new System.Drawing.Point(184, 20);
            this.divider.Margin = new System.Windows.Forms.Padding(3, 10, 3, 0);
            this.divider.Name = "divider";
            this.divider.Size = new System.Drawing.Size(2, 698);
            this.divider.TabIndex = 14;
            // 
            // navContainer
            // 
            this.navContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.navContainer.Location = new System.Drawing.Point(3, 0);
            this.navContainer.Name = "navContainer";
            this.navContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // navContainer.Panel1
            // 
            this.navContainer.Panel1.Controls.Add(this.modeTabs);
            this.navContainer.Panel1MinSize = 220;
            // 
            // navContainer.Panel2
            // 
            this.navContainer.Panel2.Controls.Add(this.mainOutputPanel);
            this.navContainer.Panel2MinSize = 150;
            this.navContainer.Size = new System.Drawing.Size(183, 705);
            this.navContainer.SplitterDistance = 263;
            this.navContainer.TabIndex = 0;
            // 
            // modeTabs
            // 
            this.modeTabs.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.modeTabs.Controls.Add(this.qraModeTab);
            this.modeTabs.Controls.Add(this.physicsModeTab);
            this.modeTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modeTabs.Location = new System.Drawing.Point(0, 0);
            this.modeTabs.Margin = new System.Windows.Forms.Padding(0);
            this.modeTabs.Multiline = true;
            this.modeTabs.Name = "modeTabs";
            this.modeTabs.Padding = new System.Drawing.Point(0, 0);
            this.modeTabs.SelectedIndex = 0;
            this.modeTabs.Size = new System.Drawing.Size(183, 263);
            this.modeTabs.TabIndex = 0;
            this.modeTabs.SelectedIndexChanged += new System.EventHandler(this.modeTabs_SelectedIndexChanged);
            // 
            // qraModeTab
            // 
            this.qraModeTab.Controls.Add(this.qraNavPanel);
            this.qraModeTab.Location = new System.Drawing.Point(4, 25);
            this.qraModeTab.Margin = new System.Windows.Forms.Padding(0);
            this.qraModeTab.Name = "qraModeTab";
            this.qraModeTab.Size = new System.Drawing.Size(175, 234);
            this.qraModeTab.TabIndex = 1;
            this.qraModeTab.Text = "QRA Mode";
            this.qraModeTab.UseVisualStyleBackColor = true;
            // 
            // qraNavPanel
            // 
            this.qraNavPanel.Controls.Add(this.fuelTypeLabel2);
            this.qraNavPanel.Controls.Add(this.qraFuelTypeSelector);
            this.qraNavPanel.Controls.Add(this.scenariosFormButton);
            this.qraNavPanel.Controls.Add(this.systemDescriptionFormButton);
            this.qraNavPanel.Controls.Add(this.probabilitiesFormButton);
            this.qraNavPanel.Controls.Add(this.consequenceModelsFormButton);
            this.qraNavPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.qraNavPanel.Location = new System.Drawing.Point(0, 0);
            this.qraNavPanel.Margin = new System.Windows.Forms.Padding(0);
            this.qraNavPanel.Name = "qraNavPanel";
            this.qraNavPanel.Size = new System.Drawing.Size(175, 234);
            this.qraNavPanel.TabIndex = 1;
            // 
            // fuelTypeLabel2
            // 
            this.fuelTypeLabel2.AutoSize = true;
            this.fuelTypeLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fuelTypeLabel2.Location = new System.Drawing.Point(-1, 181);
            this.fuelTypeLabel2.Name = "fuelTypeLabel2";
            this.fuelTypeLabel2.Size = new System.Drawing.Size(76, 16);
            this.fuelTypeLabel2.TabIndex = 13;
            this.fuelTypeLabel2.Text = "Fuel type:";
            // 
            // qraFuelTypeSelector
            // 
            this.qraFuelTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.qraFuelTypeSelector.FormattingEnabled = true;
            this.qraFuelTypeSelector.Location = new System.Drawing.Point(2, 200);
            this.qraFuelTypeSelector.Name = "qraFuelTypeSelector";
            this.qraFuelTypeSelector.Size = new System.Drawing.Size(114, 21);
            this.qraFuelTypeSelector.TabIndex = 12;
            this.qraFuelTypeSelector.SelectionChangeCommitted += new System.EventHandler(this.qraFuelTypeSelector_SelectionChangeCommitted);
            // 
            // scenariosFormButton
            // 
            this.scenariosFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scenariosFormButton.Location = new System.Drawing.Point(0, 32);
            this.scenariosFormButton.Name = "scenariosFormButton";
            this.scenariosFormButton.Size = new System.Drawing.Size(172, 23);
            this.scenariosFormButton.TabIndex = 2;
            this.scenariosFormButton.Text = "Scenarios";
            this.scenariosFormButton.UseVisualStyleBackColor = true;
            this.scenariosFormButton.Click += new System.EventHandler(this.scenariosFormButton_Click);
            // 
            // systemDescriptionFormButton
            // 
            this.systemDescriptionFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.systemDescriptionFormButton.Location = new System.Drawing.Point(0, 3);
            this.systemDescriptionFormButton.Name = "systemDescriptionFormButton";
            this.systemDescriptionFormButton.Size = new System.Drawing.Size(172, 23);
            this.systemDescriptionFormButton.TabIndex = 1;
            this.systemDescriptionFormButton.Text = "System Description";
            this.systemDescriptionFormButton.UseVisualStyleBackColor = true;
            this.systemDescriptionFormButton.Click += new System.EventHandler(this.systemDescriptionFormButton_Click);
            // 
            // probabilitiesFormButton
            // 
            this.probabilitiesFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.probabilitiesFormButton.Location = new System.Drawing.Point(0, 61);
            this.probabilitiesFormButton.Name = "probabilitiesFormButton";
            this.probabilitiesFormButton.Size = new System.Drawing.Size(172, 23);
            this.probabilitiesFormButton.TabIndex = 3;
            this.probabilitiesFormButton.Text = "Data / Probabilities";
            this.probabilitiesFormButton.UseVisualStyleBackColor = true;
            this.probabilitiesFormButton.Click += new System.EventHandler(this.probabilitiesFormButton_Click);
            // 
            // consequenceModelsFormButton
            // 
            this.consequenceModelsFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consequenceModelsFormButton.Location = new System.Drawing.Point(0, 90);
            this.consequenceModelsFormButton.Name = "consequenceModelsFormButton";
            this.consequenceModelsFormButton.Size = new System.Drawing.Size(172, 23);
            this.consequenceModelsFormButton.TabIndex = 4;
            this.consequenceModelsFormButton.Text = "Consequence Models";
            this.consequenceModelsFormButton.UseVisualStyleBackColor = true;
            this.consequenceModelsFormButton.Click += new System.EventHandler(this.consequenceModelsFormButton_Click);
            // 
            // physicsModeTab
            // 
            this.physicsModeTab.Controls.Add(this.physicsNavPanel);
            this.physicsModeTab.Location = new System.Drawing.Point(4, 25);
            this.physicsModeTab.Margin = new System.Windows.Forms.Padding(0);
            this.physicsModeTab.Name = "physicsModeTab";
            this.physicsModeTab.Size = new System.Drawing.Size(175, 234);
            this.physicsModeTab.TabIndex = 3;
            this.physicsModeTab.Text = "Physics";
            this.physicsModeTab.UseVisualStyleBackColor = true;
            // 
            // physicsNavPanel
            // 
            this.physicsNavPanel.Controls.Add(this.jetHeatAnalysisFormButton);
            this.physicsNavPanel.Controls.Add(this.jetPlotTempFormButton);
            this.physicsNavPanel.Controls.Add(this.fuelTypeLabel1);
            this.physicsNavPanel.Controls.Add(this.physicsFuelTypeSelector);
            this.physicsNavPanel.Controls.Add(this.overpressureFormButton);
            this.physicsNavPanel.Controls.Add(this.plumeFormButton);
            this.physicsNavPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicsNavPanel.Location = new System.Drawing.Point(0, 0);
            this.physicsNavPanel.Margin = new System.Windows.Forms.Padding(0);
            this.physicsNavPanel.Name = "physicsNavPanel";
            this.physicsNavPanel.Size = new System.Drawing.Size(175, 234);
            this.physicsNavPanel.TabIndex = 0;
            // 
            // jetHeatAnalysisFormButton
            // 
            this.jetHeatAnalysisFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jetHeatAnalysisFormButton.Location = new System.Drawing.Point(0, 99);
            this.jetHeatAnalysisFormButton.Name = "jetHeatAnalysisFormButton";
            this.jetHeatAnalysisFormButton.Size = new System.Drawing.Size(171, 26);
            this.jetHeatAnalysisFormButton.TabIndex = 13;
            this.jetHeatAnalysisFormButton.Text = "Radiative Heat Flux";
            this.jetHeatAnalysisFormButton.UseVisualStyleBackColor = true;
            this.jetHeatAnalysisFormButton.Click += new System.EventHandler(this.jetHeatAnalysisFormButton_Click);
            // 
            // jetPlotTempFormButton
            // 
            this.jetPlotTempFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.jetPlotTempFormButton.Location = new System.Drawing.Point(0, 67);
            this.jetPlotTempFormButton.Name = "jetPlotTempFormButton";
            this.jetPlotTempFormButton.Size = new System.Drawing.Size(171, 26);
            this.jetPlotTempFormButton.TabIndex = 12;
            this.jetPlotTempFormButton.Text = "Flame Temperature / Trajectory";
            this.jetPlotTempFormButton.UseVisualStyleBackColor = true;
            this.jetPlotTempFormButton.Click += new System.EventHandler(this.jetPlotTempFormButton_Click);
            // 
            // fuelTypeLabel1
            // 
            this.fuelTypeLabel1.AutoSize = true;
            this.fuelTypeLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fuelTypeLabel1.Location = new System.Drawing.Point(3, 190);
            this.fuelTypeLabel1.Name = "fuelTypeLabel1";
            this.fuelTypeLabel1.Size = new System.Drawing.Size(76, 16);
            this.fuelTypeLabel1.TabIndex = 11;
            this.fuelTypeLabel1.Text = "Fuel type:";
            // 
            // physicsFuelTypeSelector
            // 
            this.physicsFuelTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.physicsFuelTypeSelector.FormattingEnabled = true;
            this.physicsFuelTypeSelector.Location = new System.Drawing.Point(6, 209);
            this.physicsFuelTypeSelector.Name = "physicsFuelTypeSelector";
            this.physicsFuelTypeSelector.Size = new System.Drawing.Size(114, 21);
            this.physicsFuelTypeSelector.TabIndex = 10;
            this.physicsFuelTypeSelector.SelectionChangeCommitted += new System.EventHandler(this.physicsFuelTypeSelector_SelectionChangeCommitted);
            // 
            // overpressureFormButton
            // 
            this.overpressureFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.overpressureFormButton.Location = new System.Drawing.Point(0, 35);
            this.overpressureFormButton.Name = "overpressureFormButton";
            this.overpressureFormButton.Size = new System.Drawing.Size(171, 26);
            this.overpressureFormButton.TabIndex = 9;
            this.overpressureFormButton.Text = "Overpressure";
            this.overpressureFormButton.UseVisualStyleBackColor = true;
            this.overpressureFormButton.Click += new System.EventHandler(this.overpressureFormButton_Click);
            // 
            // plumeFormButton
            // 
            this.plumeFormButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.plumeFormButton.Location = new System.Drawing.Point(0, 3);
            this.plumeFormButton.Name = "plumeFormButton";
            this.plumeFormButton.Size = new System.Drawing.Size(171, 26);
            this.plumeFormButton.TabIndex = 6;
            this.plumeFormButton.Text = "Plume Dispersion";
            this.plumeFormButton.UseVisualStyleBackColor = true;
            this.plumeFormButton.Click += new System.EventHandler(this.plumeDispersionFormButton_Click);
            // 
            // mainOutputPanel
            // 
            this.mainOutputPanel.AutoScroll = true;
            this.mainOutputPanel.Controls.Add(this.mainOutputNavPanel);
            this.mainOutputPanel.Controls.Add(this.mainOutputGroupBox);
            this.mainOutputPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainOutputPanel.Location = new System.Drawing.Point(0, 0);
            this.mainOutputPanel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.mainOutputPanel.Name = "mainOutputPanel";
            this.mainOutputPanel.Size = new System.Drawing.Size(183, 438);
            this.mainOutputPanel.TabIndex = 0;
            // 
            // mainOutputNavPanel
            // 
            this.mainOutputNavPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainOutputNavPanel.Location = new System.Drawing.Point(0, 59);
            this.mainOutputNavPanel.Name = "mainOutputNavPanel";
            this.mainOutputNavPanel.Size = new System.Drawing.Size(183, 379);
            this.mainOutputNavPanel.TabIndex = 1;
            // 
            // mainOutputGroupBox
            // 
            this.mainOutputGroupBox.Controls.Add(this.mainOutputLabel);
            this.mainOutputGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.mainOutputGroupBox.Location = new System.Drawing.Point(0, 0);
            this.mainOutputGroupBox.Name = "mainOutputGroupBox";
            this.mainOutputGroupBox.Padding = new System.Windows.Forms.Padding(0);
            this.mainOutputGroupBox.Size = new System.Drawing.Size(183, 59);
            this.mainOutputGroupBox.TabIndex = 0;
            this.mainOutputGroupBox.TabStop = false;
            // 
            // mainOutputLabel
            // 
            this.mainOutputLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainOutputLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainOutputLabel.Location = new System.Drawing.Point(0, 13);
            this.mainOutputLabel.Margin = new System.Windows.Forms.Padding(0);
            this.mainOutputLabel.Name = "mainOutputLabel";
            this.mainOutputLabel.Size = new System.Drawing.Size(183, 46);
            this.mainOutputLabel.TabIndex = 0;
            this.mainOutputLabel.Text = "Output";
            this.mainOutputLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mainFormPanel
            // 
            this.mainFormPanel.AutoScroll = true;
            this.mainFormPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainFormPanel.Location = new System.Drawing.Point(0, 0);
            this.mainFormPanel.Name = "mainFormPanel";
            this.mainFormPanel.Padding = new System.Windows.Forms.Padding(0, 25, 0, 0);
            this.mainFormPanel.Size = new System.Drawing.Size(992, 705);
            this.mainFormPanel.TabIndex = 0;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenuItem,
            this.toolsMenuItem,
            this.helpMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1184, 24);
            this.mainMenuStrip.TabIndex = 1;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // fileMenuItem
            // 
            this.fileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveMenuItem,
            this.loadMenuItem,
            this.exitMenuItem});
            this.fileMenuItem.Name = "fileMenuItem";
            this.fileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileMenuItem.Text = "File";
            // 
            // saveMenuItem
            // 
            this.saveMenuItem.Name = "saveMenuItem";
            this.saveMenuItem.Size = new System.Drawing.Size(161, 22);
            this.saveMenuItem.Text = "Save Workspace";
            this.saveMenuItem.Click += new System.EventHandler(this.saveMenuItem_Click);
            // 
            // loadMenuItem
            // 
            this.loadMenuItem.Name = "loadMenuItem";
            this.loadMenuItem.Size = new System.Drawing.Size(161, 22);
            this.loadMenuItem.Text = "Load Workspace";
            this.loadMenuItem.Click += new System.EventHandler(this.loadMenuItem_Click);
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(161, 22);
            this.exitMenuItem.Text = "Exit";
            this.exitMenuItem.Click += new System.EventHandler(this.exitMenuItem_Click);
            // 
            // toolsMenuItem
            // 
            this.toolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.etkMenuItem,
            this.resetMenuItem,
            this.dataDirectoryMenuItem});
            this.toolsMenuItem.Name = "toolsMenuItem";
            this.toolsMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsMenuItem.Text = "Tools";
            // 
            // etkMenuItem
            // 
            this.etkMenuItem.Name = "etkMenuItem";
            this.etkMenuItem.Size = new System.Drawing.Size(260, 22);
            this.etkMenuItem.Text = "Engineering Toolkit";
            this.etkMenuItem.Click += new System.EventHandler(this.etkMenuItem_Click);
            // 
            // resetMenuItem
            // 
            this.resetMenuItem.Name = "resetMenuItem";
            this.resetMenuItem.Size = new System.Drawing.Size(260, 22);
            this.resetMenuItem.Text = "Reset all defaults and inputs to zero";
            this.resetMenuItem.Visible = false;
            this.resetMenuItem.Click += new System.EventHandler(this.resetMenuItem_Click);
            // 
            // dataDirectoryMenuItem
            // 
            this.dataDirectoryMenuItem.Name = "dataDirectoryMenuItem";
            this.dataDirectoryMenuItem.Size = new System.Drawing.Size(260, 22);
            this.dataDirectoryMenuItem.Text = "Open data directory in Explorer";
            this.dataDirectoryMenuItem.Click += new System.EventHandler(this.dataDirectoryMenuItem_Click);
            // 
            // helpMenuItem
            // 
            this.helpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutMenuItem});
            this.helpMenuItem.Name = "helpMenuItem";
            this.helpMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpMenuItem.Text = "Help";
            // 
            // aboutMenuItem
            // 
            this.aboutMenuItem.Name = "aboutMenuItem";
            this.aboutMenuItem.Size = new System.Drawing.Size(151, 22);
            this.aboutMenuItem.Text = "About HyRAM";
            this.aboutMenuItem.Click += new System.EventHandler(this.aboutMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1184, 729);
            this.Controls.Add(this.mainContainer);
            this.Controls.Add(this.mainMenuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mainMenuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HyRAM";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
            this.mainContainer.ResumeLayout(false);
            this.navContainer.Panel1.ResumeLayout(false);
            this.navContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.navContainer)).EndInit();
            this.navContainer.ResumeLayout(false);
            this.modeTabs.ResumeLayout(false);
            this.qraModeTab.ResumeLayout(false);
            this.qraNavPanel.ResumeLayout(false);
            this.qraNavPanel.PerformLayout();
            this.physicsModeTab.ResumeLayout(false);
            this.physicsNavPanel.ResumeLayout(false);
            this.physicsNavPanel.PerformLayout();
            this.mainOutputPanel.ResumeLayout(false);
            this.mainOutputGroupBox.ResumeLayout(false);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer mainContainer;
		private System.Windows.Forms.SplitContainer navContainer;
		private System.Windows.Forms.Panel mainOutputPanel;
        private System.Windows.Forms.GroupBox mainOutputGroupBox;
        private System.Windows.Forms.Panel mainFormPanel;
		private System.Windows.Forms.Label mainOutputLabel;
		private System.Windows.Forms.Panel mainOutputNavPanel;
		private System.Windows.Forms.TabControl modeTabs;
		private System.Windows.Forms.TabPage qraModeTab;
		private System.Windows.Forms.Panel qraNavPanel;
		private System.Windows.Forms.Button systemDescriptionFormButton;
		private System.Windows.Forms.Button probabilitiesFormButton;
		private System.Windows.Forms.Button consequenceModelsFormButton;
		private System.Windows.Forms.Button scenariosFormButton;
		private System.Windows.Forms.MenuStrip mainMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutMenuItem;
        private System.Windows.Forms.TabPage physicsModeTab;
        private System.Windows.Forms.Panel physicsNavPanel;
        private System.Windows.Forms.Button overpressureFormButton;
		private System.Windows.Forms.Button plumeFormButton;
        private System.Windows.Forms.ToolStripMenuItem saveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadMenuItem;
		private System.Windows.Forms.ToolStripMenuItem toolsMenuItem;
		private System.Windows.Forms.ToolStripMenuItem resetMenuItem;
		private System.Windows.Forms.ToolStripMenuItem etkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dataDirectoryMenuItem;
        private System.Windows.Forms.Label fuelTypeLabel1;
        private System.Windows.Forms.ComboBox physicsFuelTypeSelector;
        private System.Windows.Forms.Label fuelTypeLabel2;
        private System.Windows.Forms.ComboBox qraFuelTypeSelector;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Button jetHeatAnalysisFormButton;
        private System.Windows.Forms.Button jetPlotTempFormButton;
    }
}
