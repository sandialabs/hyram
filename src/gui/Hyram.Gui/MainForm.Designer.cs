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
            this.ModeTabs = new System.Windows.Forms.TabControl();
            this.qraModeTab = new System.Windows.Forms.TabPage();
            this.qraNavPanel = new System.Windows.Forms.Panel();
            this.QraSubmitBtn = new System.Windows.Forms.Button();
            this.QraFuelBtn = new System.Windows.Forms.Button();
            this.fuelTypeLabel2 = new System.Windows.Forms.Label();
            this.QraFuelTypeSelector = new System.Windows.Forms.ComboBox();
            this.ScenariosFormBtn = new System.Windows.Forms.Button();
            this.SystemDescripFormBtn = new System.Windows.Forms.Button();
            this.ProbabilitiesFormBtn = new System.Windows.Forms.Button();
            this.ConsequenceFormBtn = new System.Windows.Forms.Button();
            this.physicsModeTab = new System.Windows.Forms.TabPage();
            this.physicsNavPanel = new System.Windows.Forms.Panel();
            this.TntFormBtn = new System.Windows.Forms.Button();
            this.MassFlowFormBtn = new System.Windows.Forms.Button();
            this.TankMassFormBtn = new System.Windows.Forms.Button();
            this.TpdFormBtn = new System.Windows.Forms.Button();
            this.PhysicsFuelBtn = new System.Windows.Forms.Button();
            this.OverpressureFormBtn = new System.Windows.Forms.Button();
            this.JetFluxFormBtn = new System.Windows.Forms.Button();
            this.JetPlotFormBtn = new System.Windows.Forms.Button();
            this.fuelTypeLabel1 = new System.Windows.Forms.Label();
            this.PhysicsFuelTypeSelector = new System.Windows.Forms.ComboBox();
            this.AccumulationFormBtn = new System.Windows.Forms.Button();
            this.PlumeFormBtn = new System.Windows.Forms.Button();
            this.mainMessage = new System.Windows.Forms.Label();
            this.formArea = new System.Windows.Forms.Panel();
            this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
            this.FileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DataDirectoryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).BeginInit();
            this.mainContainer.Panel1.SuspendLayout();
            this.mainContainer.Panel2.SuspendLayout();
            this.mainContainer.SuspendLayout();
            this.ModeTabs.SuspendLayout();
            this.qraModeTab.SuspendLayout();
            this.qraNavPanel.SuspendLayout();
            this.physicsModeTab.SuspendLayout();
            this.physicsNavPanel.SuspendLayout();
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
            this.mainContainer.Panel1.Controls.Add(this.ModeTabs);
            this.mainContainer.Panel1.Padding = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.mainContainer.Panel1MinSize = 150;
            // 
            // mainContainer.Panel2
            // 
            this.mainContainer.Panel2.Controls.Add(this.mainMessage);
            this.mainContainer.Panel2.Controls.Add(this.formArea);
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
            // ModeTabs
            // 
            this.ModeTabs.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.ModeTabs.Controls.Add(this.qraModeTab);
            this.ModeTabs.Controls.Add(this.physicsModeTab);
            this.ModeTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ModeTabs.Location = new System.Drawing.Point(3, 0);
            this.ModeTabs.Margin = new System.Windows.Forms.Padding(0);
            this.ModeTabs.Multiline = true;
            this.ModeTabs.Name = "ModeTabs";
            this.ModeTabs.Padding = new System.Drawing.Point(0, 0);
            this.ModeTabs.SelectedIndex = 0;
            this.ModeTabs.Size = new System.Drawing.Size(183, 705);
            this.ModeTabs.TabIndex = 0;
            this.ModeTabs.SelectedIndexChanged += new System.EventHandler(this.ModeTabs_SelectedIndexChanged);
            // 
            // qraModeTab
            // 
            this.qraModeTab.Controls.Add(this.qraNavPanel);
            this.qraModeTab.Location = new System.Drawing.Point(4, 25);
            this.qraModeTab.Margin = new System.Windows.Forms.Padding(0);
            this.qraModeTab.Name = "qraModeTab";
            this.qraModeTab.Size = new System.Drawing.Size(175, 676);
            this.qraModeTab.TabIndex = 1;
            this.qraModeTab.Text = "QRA Mode";
            this.qraModeTab.UseVisualStyleBackColor = true;
            // 
            // qraNavPanel
            // 
            this.qraNavPanel.Controls.Add(this.QraSubmitBtn);
            this.qraNavPanel.Controls.Add(this.QraFuelBtn);
            this.qraNavPanel.Controls.Add(this.fuelTypeLabel2);
            this.qraNavPanel.Controls.Add(this.QraFuelTypeSelector);
            this.qraNavPanel.Controls.Add(this.ScenariosFormBtn);
            this.qraNavPanel.Controls.Add(this.SystemDescripFormBtn);
            this.qraNavPanel.Controls.Add(this.ProbabilitiesFormBtn);
            this.qraNavPanel.Controls.Add(this.ConsequenceFormBtn);
            this.qraNavPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.qraNavPanel.Location = new System.Drawing.Point(0, 0);
            this.qraNavPanel.Margin = new System.Windows.Forms.Padding(0);
            this.qraNavPanel.Name = "qraNavPanel";
            this.qraNavPanel.Size = new System.Drawing.Size(175, 676);
            this.qraNavPanel.TabIndex = 1;
            // 
            // QraSubmitBtn
            // 
            this.QraSubmitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QraSubmitBtn.Location = new System.Drawing.Point(1, 266);
            this.QraSubmitBtn.Name = "QraSubmitBtn";
            this.QraSubmitBtn.Size = new System.Drawing.Size(171, 23);
            this.QraSubmitBtn.TabIndex = 15;
            this.QraSubmitBtn.Text = "Output";
            this.QraSubmitBtn.UseVisualStyleBackColor = true;
            this.QraSubmitBtn.Click += new System.EventHandler(this.QraSubmitBtn_Click);
            // 
            // QraFuelBtn
            // 
            this.QraFuelBtn.Location = new System.Drawing.Point(137, 8);
            this.QraFuelBtn.Name = "QraFuelBtn";
            this.QraFuelBtn.Size = new System.Drawing.Size(33, 23);
            this.QraFuelBtn.TabIndex = 16;
            this.QraFuelBtn.Text = "...";
            this.QraFuelBtn.UseVisualStyleBackColor = true;
            // 
            // fuelTypeLabel2
            // 
            this.fuelTypeLabel2.AutoSize = true;
            this.fuelTypeLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fuelTypeLabel2.Location = new System.Drawing.Point(1, 11);
            this.fuelTypeLabel2.Name = "fuelTypeLabel2";
            this.fuelTypeLabel2.Size = new System.Drawing.Size(42, 16);
            this.fuelTypeLabel2.TabIndex = 13;
            this.fuelTypeLabel2.Text = "Fuel:";
            // 
            // QraFuelTypeSelector
            // 
            this.QraFuelTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QraFuelTypeSelector.FormattingEnabled = true;
            this.QraFuelTypeSelector.Location = new System.Drawing.Point(48, 9);
            this.QraFuelTypeSelector.Name = "QraFuelTypeSelector";
            this.QraFuelTypeSelector.Size = new System.Drawing.Size(89, 21);
            this.QraFuelTypeSelector.TabIndex = 12;
            this.QraFuelTypeSelector.SelectionChangeCommitted += new System.EventHandler(this.QraFuelTypeSelector_SelectionChangeCommitted);
            // 
            // ScenariosFormBtn
            // 
            this.ScenariosFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ScenariosFormBtn.Location = new System.Drawing.Point(0, 68);
            this.ScenariosFormBtn.Name = "ScenariosFormBtn";
            this.ScenariosFormBtn.Size = new System.Drawing.Size(172, 26);
            this.ScenariosFormBtn.TabIndex = 2;
            this.ScenariosFormBtn.Text = "Scenarios";
            this.ScenariosFormBtn.UseVisualStyleBackColor = true;
            // 
            // SystemDescripFormBtn
            // 
            this.SystemDescripFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SystemDescripFormBtn.Location = new System.Drawing.Point(0, 36);
            this.SystemDescripFormBtn.Name = "SystemDescripFormBtn";
            this.SystemDescripFormBtn.Size = new System.Drawing.Size(172, 26);
            this.SystemDescripFormBtn.TabIndex = 1;
            this.SystemDescripFormBtn.Text = "System Description";
            this.SystemDescripFormBtn.UseVisualStyleBackColor = true;
            // 
            // ProbabilitiesFormBtn
            // 
            this.ProbabilitiesFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProbabilitiesFormBtn.Location = new System.Drawing.Point(0, 100);
            this.ProbabilitiesFormBtn.Name = "ProbabilitiesFormBtn";
            this.ProbabilitiesFormBtn.Size = new System.Drawing.Size(172, 26);
            this.ProbabilitiesFormBtn.TabIndex = 3;
            this.ProbabilitiesFormBtn.Text = "Data / Probabilities";
            this.ProbabilitiesFormBtn.UseVisualStyleBackColor = true;
            // 
            // ConsequenceFormBtn
            // 
            this.ConsequenceFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConsequenceFormBtn.Location = new System.Drawing.Point(0, 132);
            this.ConsequenceFormBtn.Name = "ConsequenceFormBtn";
            this.ConsequenceFormBtn.Size = new System.Drawing.Size(172, 26);
            this.ConsequenceFormBtn.TabIndex = 4;
            this.ConsequenceFormBtn.Text = "Consequence Models";
            this.ConsequenceFormBtn.UseVisualStyleBackColor = true;
            // 
            // physicsModeTab
            // 
            this.physicsModeTab.Controls.Add(this.physicsNavPanel);
            this.physicsModeTab.Location = new System.Drawing.Point(4, 25);
            this.physicsModeTab.Margin = new System.Windows.Forms.Padding(0);
            this.physicsModeTab.Name = "physicsModeTab";
            this.physicsModeTab.Size = new System.Drawing.Size(175, 676);
            this.physicsModeTab.TabIndex = 3;
            this.physicsModeTab.Text = "Physics";
            this.physicsModeTab.UseVisualStyleBackColor = true;
            // 
            // physicsNavPanel
            // 
            this.physicsNavPanel.Controls.Add(this.TntFormBtn);
            this.physicsNavPanel.Controls.Add(this.MassFlowFormBtn);
            this.physicsNavPanel.Controls.Add(this.TankMassFormBtn);
            this.physicsNavPanel.Controls.Add(this.TpdFormBtn);
            this.physicsNavPanel.Controls.Add(this.PhysicsFuelBtn);
            this.physicsNavPanel.Controls.Add(this.OverpressureFormBtn);
            this.physicsNavPanel.Controls.Add(this.JetFluxFormBtn);
            this.physicsNavPanel.Controls.Add(this.JetPlotFormBtn);
            this.physicsNavPanel.Controls.Add(this.fuelTypeLabel1);
            this.physicsNavPanel.Controls.Add(this.PhysicsFuelTypeSelector);
            this.physicsNavPanel.Controls.Add(this.AccumulationFormBtn);
            this.physicsNavPanel.Controls.Add(this.PlumeFormBtn);
            this.physicsNavPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.physicsNavPanel.Location = new System.Drawing.Point(0, 0);
            this.physicsNavPanel.Margin = new System.Windows.Forms.Padding(0);
            this.physicsNavPanel.Name = "physicsNavPanel";
            this.physicsNavPanel.Size = new System.Drawing.Size(175, 676);
            this.physicsNavPanel.TabIndex = 0;
            // 
            // TntFormBtn
            // 
            this.TntFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TntFormBtn.Location = new System.Drawing.Point(0, 308);
            this.TntFormBtn.Name = "TntFormBtn";
            this.TntFormBtn.Size = new System.Drawing.Size(171, 26);
            this.TntFormBtn.TabIndex = 19;
            this.TntFormBtn.Text = "TNT Mass Equivalence";
            this.TntFormBtn.UseVisualStyleBackColor = true;
            // 
            // MassFlowFormBtn
            // 
            this.MassFlowFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MassFlowFormBtn.Location = new System.Drawing.Point(0, 278);
            this.MassFlowFormBtn.Name = "MassFlowFormBtn";
            this.MassFlowFormBtn.Size = new System.Drawing.Size(171, 26);
            this.MassFlowFormBtn.TabIndex = 18;
            this.MassFlowFormBtn.Text = "Mass Flow Rate";
            this.MassFlowFormBtn.UseVisualStyleBackColor = true;
            // 
            // TankMassFormBtn
            // 
            this.TankMassFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TankMassFormBtn.Location = new System.Drawing.Point(0, 248);
            this.TankMassFormBtn.Name = "TankMassFormBtn";
            this.TankMassFormBtn.Size = new System.Drawing.Size(171, 26);
            this.TankMassFormBtn.TabIndex = 17;
            this.TankMassFormBtn.Text = "Tank Mass Parameter";
            this.TankMassFormBtn.UseVisualStyleBackColor = true;
            // 
            // TpdFormBtn
            // 
            this.TpdFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TpdFormBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TpdFormBtn.Location = new System.Drawing.Point(0, 218);
            this.TpdFormBtn.Name = "TpdFormBtn";
            this.TpdFormBtn.Size = new System.Drawing.Size(171, 26);
            this.TpdFormBtn.TabIndex = 16;
            this.TpdFormBtn.Text = "Temperature, Pressure, Density";
            this.TpdFormBtn.UseVisualStyleBackColor = true;
            // 
            // PhysicsFuelBtn
            // 
            this.PhysicsFuelBtn.Location = new System.Drawing.Point(137, 8);
            this.PhysicsFuelBtn.Name = "PhysicsFuelBtn";
            this.PhysicsFuelBtn.Size = new System.Drawing.Size(33, 23);
            this.PhysicsFuelBtn.TabIndex = 15;
            this.PhysicsFuelBtn.Text = "...";
            this.PhysicsFuelBtn.UseVisualStyleBackColor = true;
            // 
            // OverpressureFormBtn
            // 
            this.OverpressureFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OverpressureFormBtn.Location = new System.Drawing.Point(0, 160);
            this.OverpressureFormBtn.Name = "OverpressureFormBtn";
            this.OverpressureFormBtn.Size = new System.Drawing.Size(171, 26);
            this.OverpressureFormBtn.TabIndex = 14;
            this.OverpressureFormBtn.Text = "Unconfined Overpressure";
            this.OverpressureFormBtn.UseVisualStyleBackColor = true;
            // 
            // JetFluxFormBtn
            // 
            this.JetFluxFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JetFluxFormBtn.Location = new System.Drawing.Point(0, 130);
            this.JetFluxFormBtn.Name = "JetFluxFormBtn";
            this.JetFluxFormBtn.Size = new System.Drawing.Size(171, 26);
            this.JetFluxFormBtn.TabIndex = 13;
            this.JetFluxFormBtn.Text = "Radiative Heat Flux";
            this.JetFluxFormBtn.UseVisualStyleBackColor = true;
            // 
            // JetPlotFormBtn
            // 
            this.JetPlotFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.JetPlotFormBtn.Location = new System.Drawing.Point(0, 100);
            this.JetPlotFormBtn.Name = "JetPlotFormBtn";
            this.JetPlotFormBtn.Size = new System.Drawing.Size(171, 26);
            this.JetPlotFormBtn.TabIndex = 12;
            this.JetPlotFormBtn.Text = "Flame Temperature / Trajectory";
            this.JetPlotFormBtn.UseVisualStyleBackColor = true;
            // 
            // fuelTypeLabel1
            // 
            this.fuelTypeLabel1.AutoSize = true;
            this.fuelTypeLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fuelTypeLabel1.Location = new System.Drawing.Point(1, 11);
            this.fuelTypeLabel1.Name = "fuelTypeLabel1";
            this.fuelTypeLabel1.Size = new System.Drawing.Size(42, 16);
            this.fuelTypeLabel1.TabIndex = 11;
            this.fuelTypeLabel1.Text = "Fuel:";
            // 
            // PhysicsFuelTypeSelector
            // 
            this.PhysicsFuelTypeSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PhysicsFuelTypeSelector.FormattingEnabled = true;
            this.PhysicsFuelTypeSelector.Location = new System.Drawing.Point(48, 9);
            this.PhysicsFuelTypeSelector.Name = "PhysicsFuelTypeSelector";
            this.PhysicsFuelTypeSelector.Size = new System.Drawing.Size(89, 21);
            this.PhysicsFuelTypeSelector.TabIndex = 10;
            this.PhysicsFuelTypeSelector.SelectionChangeCommitted += new System.EventHandler(this.PhysicsFuelTypeSelector_SelectionChangeCommitted);
            // 
            // AccumulationFormBtn
            // 
            this.AccumulationFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AccumulationFormBtn.Location = new System.Drawing.Point(0, 70);
            this.AccumulationFormBtn.Name = "AccumulationFormBtn";
            this.AccumulationFormBtn.Size = new System.Drawing.Size(171, 26);
            this.AccumulationFormBtn.TabIndex = 9;
            this.AccumulationFormBtn.Text = "Accumulation";
            this.AccumulationFormBtn.UseVisualStyleBackColor = true;
            // 
            // PlumeFormBtn
            // 
            this.PlumeFormBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlumeFormBtn.Location = new System.Drawing.Point(0, 40);
            this.PlumeFormBtn.Name = "PlumeFormBtn";
            this.PlumeFormBtn.Size = new System.Drawing.Size(171, 26);
            this.PlumeFormBtn.TabIndex = 6;
            this.PlumeFormBtn.Text = "Plume Dispersion";
            this.PlumeFormBtn.UseVisualStyleBackColor = true;
            // 
            // mainMessage
            // 
            this.mainMessage.AutoSize = true;
            this.mainMessage.BackColor = System.Drawing.Color.MistyRose;
            this.mainMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mainMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMessage.ForeColor = System.Drawing.Color.Maroon;
            this.mainMessage.Location = new System.Drawing.Point(0, 674);
            this.mainMessage.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.mainMessage.MaximumSize = new System.Drawing.Size(800, 0);
            this.mainMessage.Name = "mainMessage";
            this.mainMessage.Padding = new System.Windows.Forms.Padding(8);
            this.mainMessage.Size = new System.Drawing.Size(206, 31);
            this.mainMessage.TabIndex = 61;
            this.mainMessage.Text = "Warning/error message here";
            this.mainMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // formArea
            // 
            this.formArea.AutoScroll = true;
            this.formArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.formArea.Location = new System.Drawing.Point(0, 0);
            this.formArea.Name = "formArea";
            this.formArea.Padding = new System.Windows.Forms.Padding(0, 25, 0, 0);
            this.formArea.Size = new System.Drawing.Size(992, 705);
            this.formArea.TabIndex = 0;
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.BackColor = System.Drawing.SystemColors.MenuBar;
            this.mainMenuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileMenuItem,
            this.ToolsMenuItem,
            this.HelpMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Size = new System.Drawing.Size(1184, 24);
            this.mainMenuStrip.TabIndex = 1;
            this.mainMenuStrip.Text = "menuStrip1";
            // 
            // FileMenuItem
            // 
            this.FileMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SaveMenuItem,
            this.LoadMenuItem,
            this.ExitMenuItem});
            this.FileMenuItem.Name = "FileMenuItem";
            this.FileMenuItem.Size = new System.Drawing.Size(37, 20);
            this.FileMenuItem.Text = "File";
            // 
            // SaveMenuItem
            // 
            this.SaveMenuItem.Name = "SaveMenuItem";
            this.SaveMenuItem.Size = new System.Drawing.Size(161, 22);
            this.SaveMenuItem.Text = "Save Workspace";
            this.SaveMenuItem.Click += new System.EventHandler(this.SaveMenuItem_Click);
            // 
            // LoadMenuItem
            // 
            this.LoadMenuItem.Name = "LoadMenuItem";
            this.LoadMenuItem.Size = new System.Drawing.Size(161, 22);
            this.LoadMenuItem.Text = "Load Workspace";
            this.LoadMenuItem.Click += new System.EventHandler(this.LoadMenuItem_Click);
            // 
            // ExitMenuItem
            // 
            this.ExitMenuItem.Name = "ExitMenuItem";
            this.ExitMenuItem.Size = new System.Drawing.Size(161, 22);
            this.ExitMenuItem.Text = "Exit";
            // 
            // ToolsMenuItem
            // 
            this.ToolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DataDirectoryMenuItem});
            this.ToolsMenuItem.Name = "ToolsMenuItem";
            this.ToolsMenuItem.Size = new System.Drawing.Size(46, 20);
            this.ToolsMenuItem.Text = "Tools";
            // 
            // DataDirectoryMenuItem
            // 
            this.DataDirectoryMenuItem.Name = "DataDirectoryMenuItem";
            this.DataDirectoryMenuItem.Size = new System.Drawing.Size(238, 22);
            this.DataDirectoryMenuItem.Text = "Open data directory in Explorer";
            // 
            // HelpMenuItem
            // 
            this.HelpMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AboutMenuItem});
            this.HelpMenuItem.Name = "HelpMenuItem";
            this.HelpMenuItem.Size = new System.Drawing.Size(44, 20);
            this.HelpMenuItem.Text = "Help";
            // 
            // AboutMenuItem
            // 
            this.AboutMenuItem.Name = "AboutMenuItem";
            this.AboutMenuItem.Size = new System.Drawing.Size(159, 22);
            this.AboutMenuItem.Text = "About HyRAM+";
            this.AboutMenuItem.Click += new System.EventHandler(this.AboutMenuItem_Click);
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
            this.Text = "HyRAM+";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainContainer.Panel1.ResumeLayout(false);
            this.mainContainer.Panel2.ResumeLayout(false);
            this.mainContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainContainer)).EndInit();
            this.mainContainer.ResumeLayout(false);
            this.ModeTabs.ResumeLayout(false);
            this.qraModeTab.ResumeLayout(false);
            this.qraNavPanel.ResumeLayout(false);
            this.qraNavPanel.PerformLayout();
            this.physicsModeTab.ResumeLayout(false);
            this.physicsNavPanel.ResumeLayout(false);
            this.physicsNavPanel.PerformLayout();
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer mainContainer;
        private System.Windows.Forms.Panel formArea;
		private System.Windows.Forms.TabControl ModeTabs;
		private System.Windows.Forms.TabPage qraModeTab;
		private System.Windows.Forms.Panel qraNavPanel;
		private System.Windows.Forms.Button SystemDescripFormBtn;
		private System.Windows.Forms.Button ProbabilitiesFormBtn;
		private System.Windows.Forms.Button ConsequenceFormBtn;
		private System.Windows.Forms.Button ScenariosFormBtn;
		private System.Windows.Forms.MenuStrip mainMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem FileMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ExitMenuItem;
		private System.Windows.Forms.ToolStripMenuItem HelpMenuItem;
		private System.Windows.Forms.ToolStripMenuItem AboutMenuItem;
        private System.Windows.Forms.TabPage physicsModeTab;
        private System.Windows.Forms.Panel physicsNavPanel;
        private System.Windows.Forms.Button AccumulationFormBtn;
		private System.Windows.Forms.Button PlumeFormBtn;
        private System.Windows.Forms.ToolStripMenuItem SaveMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadMenuItem;
		private System.Windows.Forms.ToolStripMenuItem ToolsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DataDirectoryMenuItem;
        private System.Windows.Forms.Label fuelTypeLabel1;
        private System.Windows.Forms.ComboBox PhysicsFuelTypeSelector;
        private System.Windows.Forms.Label fuelTypeLabel2;
        private System.Windows.Forms.ComboBox QraFuelTypeSelector;
        private System.Windows.Forms.Label divider;
        private System.Windows.Forms.Button JetFluxFormBtn;
        private System.Windows.Forms.Button JetPlotFormBtn;
        private System.Windows.Forms.Button OverpressureFormBtn;
        private System.Windows.Forms.Label mainMessage;
        private System.Windows.Forms.Button PhysicsFuelBtn;
        private System.Windows.Forms.Button QraFuelBtn;
        private System.Windows.Forms.Button QraSubmitBtn;
        private System.Windows.Forms.Button MassFlowFormBtn;
        private System.Windows.Forms.Button TankMassFormBtn;
        private System.Windows.Forms.Button TpdFormBtn;
        private System.Windows.Forms.Button TntFormBtn;
    }
}
