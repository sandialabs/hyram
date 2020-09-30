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
            this.radiativeSourceSelector = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.consequenceInputsGroupBox = new System.Windows.Forms.GroupBox();
            this.consequenceInputUnitSelector = new System.Windows.Forms.ComboBox();
            this.consequenceInputGrid = new System.Windows.Forms.DataGridView();
            this.colVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fatalityConsequenceTab = new System.Windows.Forms.TabPage();
            this.exposureTimeUnitSelector = new System.Windows.Forms.ComboBox();
            this.thermalProbitSelector = new System.Windows.Forms.ComboBox();
            this.exposureTimeInput = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.exposureTimeLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.overpressureProbitSelector = new System.Windows.Forms.ComboBox();
            this.modelSelectionTabControl.SuspendLayout();
            this.physicalConsequenceTab.SuspendLayout();
            this.consequenceInputsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.consequenceInputGrid)).BeginInit();
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
            this.modelSelectionTabControl.Controls.Add(this.physicalConsequenceTab);
            this.modelSelectionTabControl.Controls.Add(this.fatalityConsequenceTab);
            this.modelSelectionTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelSelectionTabControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.modelSelectionTabControl.Location = new System.Drawing.Point(0, 0);
            this.modelSelectionTabControl.Name = "modelSelectionTabControl";
            this.modelSelectionTabControl.SelectedIndex = 0;
            this.modelSelectionTabControl.Size = new System.Drawing.Size(699, 645);
            this.modelSelectionTabControl.TabIndex = 9;
            // 
            // physicalConsequenceTab
            // 
            this.physicalConsequenceTab.Controls.Add(this.radiativeSourceSelector);
            this.physicalConsequenceTab.Controls.Add(this.label2);
            this.physicalConsequenceTab.Controls.Add(this.consequenceInputsGroupBox);
            this.physicalConsequenceTab.Controls.Add(this.notionalNozzleSelector);
            this.physicalConsequenceTab.Controls.Add(this.notionalNozzleLabel);
            this.physicalConsequenceTab.Location = new System.Drawing.Point(4, 24);
            this.physicalConsequenceTab.Name = "physicalConsequenceTab";
            this.physicalConsequenceTab.Padding = new System.Windows.Forms.Padding(3);
            this.physicalConsequenceTab.Size = new System.Drawing.Size(691, 617);
            this.physicalConsequenceTab.TabIndex = 0;
            this.physicalConsequenceTab.Text = "Physical Consequence Models";
            this.physicalConsequenceTab.UseVisualStyleBackColor = true;
            // 
            // radiativeSourceSelector
            // 
            this.radiativeSourceSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.radiativeSourceSelector.FormattingEnabled = true;
            this.radiativeSourceSelector.Location = new System.Drawing.Point(199, 40);
            this.radiativeSourceSelector.Name = "radiativeSourceSelector";
            this.radiativeSourceSelector.Size = new System.Drawing.Size(248, 23);
            this.radiativeSourceSelector.TabIndex = 26;
            this.radiativeSourceSelector.SelectionChangeCommitted += new System.EventHandler(this.radiativeSourceSelector_SelectionChangeCommitted);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(11, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(173, 18);
            this.label2.TabIndex = 25;
            this.label2.Text = "Radiative source model:";
            // 
            // consequenceInputsGroupBox
            // 
            this.consequenceInputsGroupBox.Controls.Add(this.consequenceInputUnitSelector);
            this.consequenceInputsGroupBox.Controls.Add(this.consequenceInputGrid);
            this.consequenceInputsGroupBox.Location = new System.Drawing.Point(13, 108);
            this.consequenceInputsGroupBox.Name = "consequenceInputsGroupBox";
            this.consequenceInputsGroupBox.Size = new System.Drawing.Size(662, 229);
            this.consequenceInputsGroupBox.TabIndex = 9;
            this.consequenceInputsGroupBox.TabStop = false;
            this.consequenceInputsGroupBox.Text = "Overpressure consequence inputs";
            // 
            // consequenceInputUnitSelector
            // 
            this.consequenceInputUnitSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.consequenceInputUnitSelector.FormattingEnabled = true;
            this.consequenceInputUnitSelector.Location = new System.Drawing.Point(7, 20);
            this.consequenceInputUnitSelector.Name = "consequenceInputUnitSelector";
            this.consequenceInputUnitSelector.Size = new System.Drawing.Size(77, 23);
            this.consequenceInputUnitSelector.TabIndex = 2;
            this.consequenceInputUnitSelector.SelectedIndexChanged += new System.EventHandler(this.consequenceInputUnitSelector_SelectedIndexChanged);
            // 
            // consequenceInputGrid
            // 
            this.consequenceInputGrid.AllowUserToAddRows = false;
            this.consequenceInputGrid.AllowUserToDeleteRows = false;
            this.consequenceInputGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.consequenceInputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.consequenceInputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.consequenceInputGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVariable,
            this.col0,
            this.col1,
            this.col2,
            this.col3,
            this.col4});
            this.consequenceInputGrid.Location = new System.Drawing.Point(6, 59);
            this.consequenceInputGrid.Name = "consequenceInputGrid";
            this.consequenceInputGrid.Size = new System.Drawing.Size(650, 163);
            this.consequenceInputGrid.TabIndex = 0;
            this.consequenceInputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.consequenceInputGrid_CellValueChanged);
            // 
            // colVariable
            // 
            this.colVariable.HeaderText = "Variable";
            this.colVariable.Name = "colVariable";
            // 
            // col0
            // 
            this.col0.HeaderText = "0.01% Leak";
            this.col0.Name = "col0";
            // 
            // col1
            // 
            this.col1.HeaderText = "0.1% Leak";
            this.col1.Name = "col1";
            // 
            // col2
            // 
            this.col2.HeaderText = "1.0% Leak";
            this.col2.Name = "col2";
            // 
            // col3
            // 
            this.col3.HeaderText = "10% Leak";
            this.col3.Name = "col3";
            // 
            // col4
            // 
            this.col4.HeaderText = "100% Leak";
            this.col4.Name = "col4";
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
            this.fatalityConsequenceTab.Size = new System.Drawing.Size(691, 617);
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
            "Collapse",
            "Debris"});
            this.overpressureProbitSelector.Location = new System.Drawing.Point(199, 69);
            this.overpressureProbitSelector.Name = "overpressureProbitSelector";
            this.overpressureProbitSelector.Size = new System.Drawing.Size(248, 23);
            this.overpressureProbitSelector.TabIndex = 16;
            this.overpressureProbitSelector.SelectionChangeCommitted += new System.EventHandler(this.overpressureProbitSelector_SelectionChangeCommotted);
            // 
            // ConsequenceModelsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.modelSelectionTabControl);
            this.Name = "ConsequenceModelsForm";
            this.Size = new System.Drawing.Size(699, 645);
            this.modelSelectionTabControl.ResumeLayout(false);
            this.physicalConsequenceTab.ResumeLayout(false);
            this.physicalConsequenceTab.PerformLayout();
            this.consequenceInputsGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.consequenceInputGrid)).EndInit();
            this.fatalityConsequenceTab.ResumeLayout(false);
            this.fatalityConsequenceTab.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox notionalNozzleSelector;
		private System.Windows.Forms.Label notionalNozzleLabel;
		private System.Windows.Forms.TabControl modelSelectionTabControl;
		private System.Windows.Forms.TabPage physicalConsequenceTab;
		private System.Windows.Forms.TabPage fatalityConsequenceTab;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox overpressureProbitSelector;
		private System.Windows.Forms.GroupBox consequenceInputsGroupBox;
		private System.Windows.Forms.DataGridView consequenceInputGrid;
		private System.Windows.Forms.ComboBox consequenceInputUnitSelector;
		private System.Windows.Forms.DataGridViewTextBoxColumn colVariable;
		private System.Windows.Forms.DataGridViewTextBoxColumn col0;
		private System.Windows.Forms.DataGridViewTextBoxColumn col1;
		private System.Windows.Forms.DataGridViewTextBoxColumn col2;
		private System.Windows.Forms.DataGridViewTextBoxColumn col3;
		private System.Windows.Forms.DataGridViewTextBoxColumn col4;
        private System.Windows.Forms.ComboBox exposureTimeUnitSelector;
        private System.Windows.Forms.ComboBox thermalProbitSelector;
        private System.Windows.Forms.TextBox exposureTimeInput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label exposureTimeLabel;
        private System.Windows.Forms.ComboBox radiativeSourceSelector;
        private System.Windows.Forms.Label label2;
    }
}
