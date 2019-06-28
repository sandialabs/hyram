namespace QRA_Frontend {
	partial class FrmQreMain {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmQreMain));
            this.scMainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.scNavigation = new System.Windows.Forms.SplitContainer();
            this.tcNav = new System.Windows.Forms.TabControl();
            this.tpQraMode = new System.Windows.Forms.TabPage();
            this.pnlQraNav = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.fuelTypeQra = new System.Windows.Forms.ComboBox();
            this.btnScenarios = new System.Windows.Forms.Button();
            this.btnSystemDescription = new System.Windows.Forms.Button();
            this.btnQraData_Probabilities = new System.Windows.Forms.Button();
            this.btnConsequenceModels = new System.Windows.Forms.Button();
            this.tpPhysics = new System.Windows.Forms.TabPage();
            this.pnlPhysicsNav = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.fuelTypePhys = new System.Windows.Forms.ComboBox();
            this.btnOverpressure = new System.Windows.Forms.Button();
            this.btnJetFlame = new System.Windows.Forms.Button();
            this.btnGasPlumeDispersion = new System.Windows.Forms.Button();
            this.tpNfpa2Mode = new System.Windows.Forms.TabPage();
            this.pnlNfpaNav = new System.Windows.Forms.Panel();
            this.btnAdvanced = new System.Windows.Forms.Button();
            this.btnFire = new System.Windows.Forms.Button();
            this.btnExplosions = new System.Windows.Forms.Button();
            this.btnHazMat = new System.Windows.Forms.Button();
            this.btnSafety = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.tpTests = new System.Windows.Forms.TabPage();
            this.pnlTestNav = new System.Windows.Forms.Panel();
            this.btnRoutines = new System.Windows.Forms.Button();
            this.SafetyActionsNarrative = new System.Windows.Forms.Panel();
            this.pnlActionButtons = new System.Windows.Forms.Panel();
            this.gbActionNarrative = new System.Windows.Forms.GroupBox();
            this.lblActionNarrative = new System.Windows.Forms.Label();
            this.ContentPanel = new System.Windows.Forms.Panel();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLoad = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuETK = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuResetDefaults = new System.Windows.Forms.ToolStripMenuItem();
            this.openMasterInputEditorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpenUserDataDir = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.About = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.scMainSplitContainer)).BeginInit();
            this.scMainSplitContainer.Panel1.SuspendLayout();
            this.scMainSplitContainer.Panel2.SuspendLayout();
            this.scMainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.scNavigation)).BeginInit();
            this.scNavigation.Panel1.SuspendLayout();
            this.scNavigation.Panel2.SuspendLayout();
            this.scNavigation.SuspendLayout();
            this.tcNav.SuspendLayout();
            this.tpQraMode.SuspendLayout();
            this.pnlQraNav.SuspendLayout();
            this.tpPhysics.SuspendLayout();
            this.pnlPhysicsNav.SuspendLayout();
            this.tpNfpa2Mode.SuspendLayout();
            this.pnlNfpaNav.SuspendLayout();
            this.tpTests.SuspendLayout();
            this.pnlTestNav.SuspendLayout();
            this.SafetyActionsNarrative.SuspendLayout();
            this.gbActionNarrative.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // scMainSplitContainer
            // 
            this.scMainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMainSplitContainer.Location = new System.Drawing.Point(0, 24);
            this.scMainSplitContainer.Name = "scMainSplitContainer";
            // 
            // scMainSplitContainer.Panel1
            // 
            this.scMainSplitContainer.Panel1.Controls.Add(this.scNavigation);
            this.scMainSplitContainer.Panel1MinSize = 225;
            // 
            // scMainSplitContainer.Panel2
            // 
            this.scMainSplitContainer.Panel2.Controls.Add(this.ContentPanel);
            this.scMainSplitContainer.Size = new System.Drawing.Size(1126, 586);
            this.scMainSplitContainer.SplitterDistance = 225;
            this.scMainSplitContainer.SplitterWidth = 6;
            this.scMainSplitContainer.TabIndex = 0;
            // 
            // scNavigation
            // 
            this.scNavigation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scNavigation.Location = new System.Drawing.Point(0, 0);
            this.scNavigation.Name = "scNavigation";
            this.scNavigation.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // scNavigation.Panel1
            // 
            this.scNavigation.Panel1.Controls.Add(this.tcNav);
            this.scNavigation.Panel1MinSize = 220;
            // 
            // scNavigation.Panel2
            // 
            this.scNavigation.Panel2.Controls.Add(this.SafetyActionsNarrative);
            this.scNavigation.Panel2MinSize = 150;
            this.scNavigation.Size = new System.Drawing.Size(225, 586);
            this.scNavigation.SplitterDistance = 220;
            this.scNavigation.TabIndex = 0;
            // 
            // tcNav
            // 
            this.tcNav.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tcNav.Controls.Add(this.tpQraMode);
            this.tcNav.Controls.Add(this.tpPhysics);
            this.tcNav.Controls.Add(this.tpNfpa2Mode);
            this.tcNav.Controls.Add(this.tpTests);
            this.tcNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcNav.Location = new System.Drawing.Point(0, 0);
            this.tcNav.Name = "tcNav";
            this.tcNav.SelectedIndex = 0;
            this.tcNav.Size = new System.Drawing.Size(225, 220);
            this.tcNav.TabIndex = 0;
            this.tcNav.SelectedIndexChanged += new System.EventHandler(this.tcNav_SelectedIndexChanged);
            // 
            // tpQraMode
            // 
            this.tpQraMode.Controls.Add(this.pnlQraNav);
            this.tpQraMode.Location = new System.Drawing.Point(4, 25);
            this.tpQraMode.Name = "tpQraMode";
            this.tpQraMode.Padding = new System.Windows.Forms.Padding(3);
            this.tpQraMode.Size = new System.Drawing.Size(217, 191);
            this.tpQraMode.TabIndex = 1;
            this.tpQraMode.Text = "QRA Mode";
            this.tpQraMode.UseVisualStyleBackColor = true;
            // 
            // pnlQraNav
            // 
            this.pnlQraNav.Controls.Add(this.label2);
            this.pnlQraNav.Controls.Add(this.fuelTypeQra);
            this.pnlQraNav.Controls.Add(this.btnScenarios);
            this.pnlQraNav.Controls.Add(this.btnSystemDescription);
            this.pnlQraNav.Controls.Add(this.btnQraData_Probabilities);
            this.pnlQraNav.Controls.Add(this.btnConsequenceModels);
            this.pnlQraNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlQraNav.Location = new System.Drawing.Point(3, 3);
            this.pnlQraNav.Name = "pnlQraNav";
            this.pnlQraNav.Size = new System.Drawing.Size(211, 185);
            this.pnlQraNav.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(7, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 16);
            this.label2.TabIndex = 13;
            this.label2.Text = "Fuel type:";
            this.label2.Visible = false;
            // 
            // fuelTypeQra
            // 
            this.fuelTypeQra.FormattingEnabled = true;
            this.fuelTypeQra.Location = new System.Drawing.Point(89, 13);
            this.fuelTypeQra.Name = "fuelTypeQra";
            this.fuelTypeQra.Size = new System.Drawing.Size(114, 21);
            this.fuelTypeQra.TabIndex = 12;
            this.fuelTypeQra.Visible = false;
            this.fuelTypeQra.SelectionChangeCommitted += new System.EventHandler(this.fuelTypeQra_SelectionChangeCommitted);
            // 
            // btnScenarios
            // 
            this.btnScenarios.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnScenarios.Location = new System.Drawing.Point(10, 66);
            this.btnScenarios.Name = "btnScenarios";
            this.btnScenarios.Size = new System.Drawing.Size(193, 23);
            this.btnScenarios.TabIndex = 2;
            this.btnScenarios.Text = "Scenarios";
            this.btnScenarios.UseVisualStyleBackColor = true;
            this.btnScenarios.Click += new System.EventHandler(this.btnScenarios_Click);
            // 
            // btnSystemDescription
            // 
            this.btnSystemDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSystemDescription.Location = new System.Drawing.Point(10, 40);
            this.btnSystemDescription.Name = "btnSystemDescription";
            this.btnSystemDescription.Size = new System.Drawing.Size(193, 23);
            this.btnSystemDescription.TabIndex = 1;
            this.btnSystemDescription.Text = "System Description";
            this.btnSystemDescription.UseVisualStyleBackColor = true;
            this.btnSystemDescription.Click += new System.EventHandler(this.btnSystemDescription_Click);
            // 
            // btnQraData_Probabilities
            // 
            this.btnQraData_Probabilities.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnQraData_Probabilities.Location = new System.Drawing.Point(10, 93);
            this.btnQraData_Probabilities.Name = "btnQraData_Probabilities";
            this.btnQraData_Probabilities.Size = new System.Drawing.Size(193, 23);
            this.btnQraData_Probabilities.TabIndex = 3;
            this.btnQraData_Probabilities.Text = "Data / Probabilities";
            this.btnQraData_Probabilities.UseVisualStyleBackColor = true;
            this.btnQraData_Probabilities.Click += new System.EventHandler(this.btnQraData_Probabilities_Click);
            // 
            // btnConsequenceModels
            // 
            this.btnConsequenceModels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConsequenceModels.Location = new System.Drawing.Point(10, 122);
            this.btnConsequenceModels.Name = "btnConsequenceModels";
            this.btnConsequenceModels.Size = new System.Drawing.Size(193, 23);
            this.btnConsequenceModels.TabIndex = 4;
            this.btnConsequenceModels.Text = "Consequence Models";
            this.btnConsequenceModels.UseVisualStyleBackColor = true;
            this.btnConsequenceModels.Click += new System.EventHandler(this.btnConsequenceModels_Click);
            // 
            // tpPhysics
            // 
            this.tpPhysics.Controls.Add(this.pnlPhysicsNav);
            this.tpPhysics.Location = new System.Drawing.Point(4, 25);
            this.tpPhysics.Margin = new System.Windows.Forms.Padding(2);
            this.tpPhysics.Name = "tpPhysics";
            this.tpPhysics.Padding = new System.Windows.Forms.Padding(2);
            this.tpPhysics.Size = new System.Drawing.Size(217, 191);
            this.tpPhysics.TabIndex = 3;
            this.tpPhysics.Text = "Physics";
            this.tpPhysics.UseVisualStyleBackColor = true;
            // 
            // pnlPhysicsNav
            // 
            this.pnlPhysicsNav.Controls.Add(this.label1);
            this.pnlPhysicsNav.Controls.Add(this.fuelTypePhys);
            this.pnlPhysicsNav.Controls.Add(this.btnOverpressure);
            this.pnlPhysicsNav.Controls.Add(this.btnJetFlame);
            this.pnlPhysicsNav.Controls.Add(this.btnGasPlumeDispersion);
            this.pnlPhysicsNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPhysicsNav.Location = new System.Drawing.Point(2, 2);
            this.pnlPhysicsNav.Margin = new System.Windows.Forms.Padding(2);
            this.pnlPhysicsNav.Name = "pnlPhysicsNav";
            this.pnlPhysicsNav.Size = new System.Drawing.Size(213, 187);
            this.pnlPhysicsNav.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 11;
            this.label1.Text = "Fuel type:";
            this.label1.Visible = false;
            // 
            // fuelTypePhys
            // 
            this.fuelTypePhys.FormattingEnabled = true;
            this.fuelTypePhys.Location = new System.Drawing.Point(89, 12);
            this.fuelTypePhys.Name = "fuelTypePhys";
            this.fuelTypePhys.Size = new System.Drawing.Size(114, 21);
            this.fuelTypePhys.TabIndex = 10;
            this.fuelTypePhys.Visible = false;
            this.fuelTypePhys.SelectionChangeCommitted += new System.EventHandler(this.fuelTypePhys_SelectionChangeCommitted);
            // 
            // btnOverpressure
            // 
            this.btnOverpressure.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOverpressure.Location = new System.Drawing.Point(9, 71);
            this.btnOverpressure.Name = "btnOverpressure";
            this.btnOverpressure.Size = new System.Drawing.Size(194, 26);
            this.btnOverpressure.TabIndex = 9;
            this.btnOverpressure.Text = "Overpressure";
            this.btnOverpressure.UseVisualStyleBackColor = true;
            this.btnOverpressure.Click += new System.EventHandler(this.btnOverpressure_Click);
            // 
            // btnJetFlame
            // 
            this.btnJetFlame.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnJetFlame.Location = new System.Drawing.Point(9, 104);
            this.btnJetFlame.Name = "btnJetFlame";
            this.btnJetFlame.Size = new System.Drawing.Size(194, 26);
            this.btnJetFlame.TabIndex = 7;
            this.btnJetFlame.Text = "Jet Flame";
            this.btnJetFlame.UseVisualStyleBackColor = true;
            this.btnJetFlame.Click += new System.EventHandler(this.btnJetFlame_Click);
            // 
            // btnGasPlumeDispersion
            // 
            this.btnGasPlumeDispersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGasPlumeDispersion.Location = new System.Drawing.Point(9, 39);
            this.btnGasPlumeDispersion.Name = "btnGasPlumeDispersion";
            this.btnGasPlumeDispersion.Size = new System.Drawing.Size(194, 26);
            this.btnGasPlumeDispersion.TabIndex = 6;
            this.btnGasPlumeDispersion.Text = "Gas Plume Dispersion";
            this.btnGasPlumeDispersion.UseVisualStyleBackColor = true;
            this.btnGasPlumeDispersion.Click += new System.EventHandler(this.btnGasPlumeDispersion_Click);
            // 
            // tpNfpa2Mode
            // 
            this.tpNfpa2Mode.Controls.Add(this.pnlNfpaNav);
            this.tpNfpa2Mode.Location = new System.Drawing.Point(4, 25);
            this.tpNfpa2Mode.Name = "tpNfpa2Mode";
            this.tpNfpa2Mode.Padding = new System.Windows.Forms.Padding(3);
            this.tpNfpa2Mode.Size = new System.Drawing.Size(217, 191);
            this.tpNfpa2Mode.TabIndex = 0;
            this.tpNfpa2Mode.Text = "PBD Mode";
            this.tpNfpa2Mode.UseVisualStyleBackColor = true;
            // 
            // pnlNfpaNav
            // 
            this.pnlNfpaNav.Controls.Add(this.btnAdvanced);
            this.pnlNfpaNav.Controls.Add(this.btnFire);
            this.pnlNfpaNav.Controls.Add(this.btnExplosions);
            this.pnlNfpaNav.Controls.Add(this.btnHazMat);
            this.pnlNfpaNav.Controls.Add(this.btnSafety);
            this.pnlNfpaNav.Controls.Add(this.btnSettings);
            this.pnlNfpaNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNfpaNav.Location = new System.Drawing.Point(3, 3);
            this.pnlNfpaNav.Name = "pnlNfpaNav";
            this.pnlNfpaNav.Size = new System.Drawing.Size(211, 185);
            this.pnlNfpaNav.TabIndex = 0;
            // 
            // btnAdvanced
            // 
            this.btnAdvanced.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdvanced.Enabled = false;
            this.btnAdvanced.Location = new System.Drawing.Point(9, 121);
            this.btnAdvanced.Name = "btnAdvanced";
            this.btnAdvanced.Size = new System.Drawing.Size(301, 23);
            this.btnAdvanced.TabIndex = 6;
            this.btnAdvanced.Text = "Advanced (per 5.4.1.3)";
            this.btnAdvanced.UseVisualStyleBackColor = true;
            // 
            // btnFire
            // 
            this.btnFire.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFire.Enabled = false;
            this.btnFire.Location = new System.Drawing.Point(9, 7);
            this.btnFire.Name = "btnFire";
            this.btnFire.Size = new System.Drawing.Size(301, 23);
            this.btnFire.TabIndex = 5;
            this.btnFire.Text = "5.4.2. Fire";
            this.btnFire.UseVisualStyleBackColor = true;
            this.btnFire.Click += new System.EventHandler(this.btnFire_Click);
            // 
            // btnExplosions
            // 
            this.btnExplosions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExplosions.Enabled = false;
            this.btnExplosions.Location = new System.Drawing.Point(10, 34);
            this.btnExplosions.Name = "btnExplosions";
            this.btnExplosions.Size = new System.Drawing.Size(301, 23);
            this.btnExplosions.TabIndex = 4;
            this.btnExplosions.Text = "5.4.3. Explosions";
            this.btnExplosions.UseVisualStyleBackColor = true;
            this.btnExplosions.Click += new System.EventHandler(this.btnExplosions_Click);
            // 
            // btnHazMat
            // 
            this.btnHazMat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnHazMat.Enabled = false;
            this.btnHazMat.Location = new System.Drawing.Point(10, 63);
            this.btnHazMat.Name = "btnHazMat";
            this.btnHazMat.Size = new System.Drawing.Size(301, 23);
            this.btnHazMat.TabIndex = 3;
            this.btnHazMat.Text = "5.4.4. Hazardous Materials";
            this.btnHazMat.UseVisualStyleBackColor = true;
            this.btnHazMat.Click += new System.EventHandler(this.btnHazMat_Click);
            // 
            // btnSafety
            // 
            this.btnSafety.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSafety.Enabled = false;
            this.btnSafety.Location = new System.Drawing.Point(10, 92);
            this.btnSafety.Name = "btnSafety";
            this.btnSafety.Size = new System.Drawing.Size(301, 23);
            this.btnSafety.TabIndex = 1;
            this.btnSafety.Text = "5.4.5. Safety (During Building Use)";
            this.btnSafety.UseVisualStyleBackColor = true;
            this.btnSafety.Click += new System.EventHandler(this.btnSafety_Click);
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.Enabled = false;
            this.btnSettings.Location = new System.Drawing.Point(10, 156);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(301, 23);
            this.btnSettings.TabIndex = 0;
            this.btnSettings.Text = "Settings/Defaults";
            this.btnSettings.UseVisualStyleBackColor = true;
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);
            // 
            // tpTests
            // 
            this.tpTests.Controls.Add(this.pnlTestNav);
            this.tpTests.Location = new System.Drawing.Point(4, 25);
            this.tpTests.Name = "tpTests";
            this.tpTests.Size = new System.Drawing.Size(217, 191);
            this.tpTests.TabIndex = 2;
            this.tpTests.Text = "Tests";
            this.tpTests.UseVisualStyleBackColor = true;
            // 
            // pnlTestNav
            // 
            this.pnlTestNav.Controls.Add(this.btnRoutines);
            this.pnlTestNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTestNav.Location = new System.Drawing.Point(0, 0);
            this.pnlTestNav.Name = "pnlTestNav";
            this.pnlTestNav.Size = new System.Drawing.Size(217, 191);
            this.pnlTestNav.TabIndex = 2;
            // 
            // btnRoutines
            // 
            this.btnRoutines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRoutines.Location = new System.Drawing.Point(10, 10);
            this.btnRoutines.Name = "btnRoutines";
            this.btnRoutines.Size = new System.Drawing.Size(306, 23);
            this.btnRoutines.TabIndex = 4;
            this.btnRoutines.Text = "Routines";
            this.btnRoutines.UseVisualStyleBackColor = true;
            // 
            // SafetyActionsNarrative
            // 
            this.SafetyActionsNarrative.AutoScroll = true;
            this.SafetyActionsNarrative.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SafetyActionsNarrative.Controls.Add(this.pnlActionButtons);
            this.SafetyActionsNarrative.Controls.Add(this.gbActionNarrative);
            this.SafetyActionsNarrative.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SafetyActionsNarrative.Location = new System.Drawing.Point(0, 0);
            this.SafetyActionsNarrative.Name = "SafetyActionsNarrative";
            this.SafetyActionsNarrative.Size = new System.Drawing.Size(225, 362);
            this.SafetyActionsNarrative.TabIndex = 0;
            // 
            // pnlActionButtons
            // 
            this.pnlActionButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlActionButtons.Location = new System.Drawing.Point(0, 59);
            this.pnlActionButtons.Name = "pnlActionButtons";
            this.pnlActionButtons.Size = new System.Drawing.Size(221, 299);
            this.pnlActionButtons.TabIndex = 1;
            // 
            // gbActionNarrative
            // 
            this.gbActionNarrative.Controls.Add(this.lblActionNarrative);
            this.gbActionNarrative.Dock = System.Windows.Forms.DockStyle.Top;
            this.gbActionNarrative.Location = new System.Drawing.Point(0, 0);
            this.gbActionNarrative.Name = "gbActionNarrative";
            this.gbActionNarrative.Size = new System.Drawing.Size(221, 59);
            this.gbActionNarrative.TabIndex = 0;
            this.gbActionNarrative.TabStop = false;
            // 
            // lblActionNarrative
            // 
            this.lblActionNarrative.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblActionNarrative.Location = new System.Drawing.Point(3, 16);
            this.lblActionNarrative.Name = "lblActionNarrative";
            this.lblActionNarrative.Size = new System.Drawing.Size(215, 40);
            this.lblActionNarrative.TabIndex = 0;
            this.lblActionNarrative.Text = "Output";
            this.lblActionNarrative.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ContentPanel
            // 
            this.ContentPanel.AutoScroll = true;
            this.ContentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentPanel.Location = new System.Drawing.Point(0, 0);
            this.ContentPanel.Name = "ContentPanel";
            this.ContentPanel.Size = new System.Drawing.Size(895, 586);
            this.ContentPanel.TabIndex = 0;
            // 
            // mnuMain
            // 
            this.mnuMain.BackColor = System.Drawing.SystemColors.MenuBar;
            this.mnuMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.toolsToolStripMenuItem,
            this.mnuHelp});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(1126, 24);
            this.mnuMain.TabIndex = 1;
            this.mnuMain.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSave,
            this.mnuLoad,
            this.mnuExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 20);
            this.mnuFile.Text = "File";
            // 
            // mnuSave
            // 
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.Size = new System.Drawing.Size(161, 22);
            this.mnuSave.Text = "Save Workspace";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuLoad
            // 
            this.mnuLoad.Name = "mnuLoad";
            this.mnuLoad.Size = new System.Drawing.Size(161, 22);
            this.mnuLoad.Text = "Load Workspace";
            this.mnuLoad.Click += new System.EventHandler(this.mnuLoad_Click);
            // 
            // mnuExit
            // 
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(161, 22);
            this.mnuExit.Text = "Exit";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuETK,
            this.mnuResetDefaults,
            this.openMasterInputEditorToolStripMenuItem,
            this.mnuOpenUserDataDir});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // mnuETK
            // 
            this.mnuETK.Name = "mnuETK";
            this.mnuETK.Size = new System.Drawing.Size(260, 22);
            this.mnuETK.Text = "Engineering Toolkit";
            this.mnuETK.Click += new System.EventHandler(this.mnuETK_Click);
            // 
            // mnuResetDefaults
            // 
            this.mnuResetDefaults.Name = "mnuResetDefaults";
            this.mnuResetDefaults.Size = new System.Drawing.Size(260, 22);
            this.mnuResetDefaults.Text = "Reset all defaults and inputs to zero";
            this.mnuResetDefaults.Visible = false;
            this.mnuResetDefaults.Click += new System.EventHandler(this.mnuResetDefaults_Click);
            // 
            // openMasterInputEditorToolStripMenuItem
            // 
            this.openMasterInputEditorToolStripMenuItem.Name = "openMasterInputEditorToolStripMenuItem";
            this.openMasterInputEditorToolStripMenuItem.Size = new System.Drawing.Size(260, 22);
            this.openMasterInputEditorToolStripMenuItem.Text = "QRA Master Input Editor";
            this.openMasterInputEditorToolStripMenuItem.Visible = false;
            this.openMasterInputEditorToolStripMenuItem.Click += new System.EventHandler(this.openMasterInputEditorToolStripMenuItem_Click);
            // 
            // mnuOpenUserDataDir
            // 
            this.mnuOpenUserDataDir.Name = "mnuOpenUserDataDir";
            this.mnuOpenUserDataDir.Size = new System.Drawing.Size(260, 22);
            this.mnuOpenUserDataDir.Text = "Open data directory in Explorer";
            this.mnuOpenUserDataDir.Click += new System.EventHandler(this.mnuOpenUserDataDir_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.About});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 20);
            this.mnuHelp.Text = "Help";
            // 
            // About
            // 
            this.About.Name = "About";
            this.About.Size = new System.Drawing.Size(151, 22);
            this.About.Text = "About HyRAM";
            this.About.Click += new System.EventHandler(this.About_Click);
            // 
            // FrmQreMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1126, 610);
            this.Controls.Add(this.scMainSplitContainer);
            this.Controls.Add(this.mnuMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.mnuMain;
            this.Name = "FrmQreMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HyRAM";
            this.Load += new System.EventHandler(this.frmQFEMain_Load);
            this.scMainSplitContainer.Panel1.ResumeLayout(false);
            this.scMainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scMainSplitContainer)).EndInit();
            this.scMainSplitContainer.ResumeLayout(false);
            this.scNavigation.Panel1.ResumeLayout(false);
            this.scNavigation.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.scNavigation)).EndInit();
            this.scNavigation.ResumeLayout(false);
            this.tcNav.ResumeLayout(false);
            this.tpQraMode.ResumeLayout(false);
            this.pnlQraNav.ResumeLayout(false);
            this.pnlQraNav.PerformLayout();
            this.tpPhysics.ResumeLayout(false);
            this.pnlPhysicsNav.ResumeLayout(false);
            this.pnlPhysicsNav.PerformLayout();
            this.tpNfpa2Mode.ResumeLayout(false);
            this.pnlNfpaNav.ResumeLayout(false);
            this.tpTests.ResumeLayout(false);
            this.pnlTestNav.ResumeLayout(false);
            this.SafetyActionsNarrative.ResumeLayout(false);
            this.gbActionNarrative.ResumeLayout(false);
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SplitContainer scMainSplitContainer;
		private System.Windows.Forms.SplitContainer scNavigation;
		private System.Windows.Forms.Panel SafetyActionsNarrative;
        private System.Windows.Forms.GroupBox gbActionNarrative;
        private System.Windows.Forms.Panel ContentPanel;
		private System.Windows.Forms.Label lblActionNarrative;
		private System.Windows.Forms.Panel pnlActionButtons;
		private System.Windows.Forms.Panel pnlNfpaNav;
		private System.Windows.Forms.Button btnExplosions;
		private System.Windows.Forms.Button btnHazMat;
		private System.Windows.Forms.Button btnSafety;
		private System.Windows.Forms.Button btnSettings;
		private System.Windows.Forms.TabControl tcNav;
		private System.Windows.Forms.TabPage tpNfpa2Mode;
		private System.Windows.Forms.TabPage tpQraMode;
		private System.Windows.Forms.Panel pnlQraNav;
		private System.Windows.Forms.Button btnSystemDescription;
		private System.Windows.Forms.Button btnQraData_Probabilities;
		private System.Windows.Forms.Button btnConsequenceModels;
		private System.Windows.Forms.TabPage tpTests;
		private System.Windows.Forms.Panel pnlTestNav;
		private System.Windows.Forms.Button btnRoutines;
		private System.Windows.Forms.Button btnScenarios;
        private System.Windows.Forms.Button btnAdvanced;
        private System.Windows.Forms.Button btnFire;
		private System.Windows.Forms.MenuStrip mnuMain;
		private System.Windows.Forms.ToolStripMenuItem mnuFile;
		private System.Windows.Forms.ToolStripMenuItem mnuExit;
		private System.Windows.Forms.ToolStripMenuItem mnuHelp;
		private System.Windows.Forms.ToolStripMenuItem About;
        private System.Windows.Forms.TabPage tpPhysics;
        private System.Windows.Forms.Panel pnlPhysicsNav;
        private System.Windows.Forms.Button btnOverpressure;
        private System.Windows.Forms.Button btnJetFlame;
		private System.Windows.Forms.Button btnGasPlumeDispersion;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripMenuItem mnuLoad;
		private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuResetDefaults;
        private System.Windows.Forms.ToolStripMenuItem openMasterInputEditorToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem mnuETK;
        private System.Windows.Forms.ToolStripMenuItem mnuOpenUserDataDir;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox fuelTypePhys;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox fuelTypeQra;
    }
}
