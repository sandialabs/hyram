namespace QRA_Frontend.ContentPanels {
	partial class CpConsequenceModels {
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
            this.cbNotionalNozzleModel = new System.Windows.Forms.ComboBox();
            this.cbDeflagrationModel = new System.Windows.Forms.ComboBox();
            this.lblNotionalNozzleModel = new System.Windows.Forms.Label();
            this.lblOPModel = new System.Windows.Forms.Label();
            this.tcModelSelection = new System.Windows.Forms.TabControl();
            this.tpPhysicalConsequence = new System.Windows.Forms.TabPage();
            this.cbRadiativeSourceModel = new UIHelpers.AnyEnumComboSelector();
            this.gbCFDInput = new System.Windows.Forms.GroupBox();
            this.cbCFDUnits = new System.Windows.Forms.ComboBox();
            this.lblUnits = new System.Windows.Forms.Label();
            this.dgCFDInput = new System.Windows.Forms.DataGridView();
            this.colVariable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col0 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tpFatality = new System.Windows.Forms.TabPage();
            this.cbThermalExposureTimeUnits = new System.Windows.Forms.ComboBox();
            this.cbThermalProbitModel = new System.Windows.Forms.ComboBox();
            this.tbThermalExposureTime = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblThermalExposureTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cbOverpressureProbitModel = new System.Windows.Forms.ComboBox();
            this.tcModelSelection.SuspendLayout();
            this.tpPhysicalConsequence.SuspendLayout();
            this.gbCFDInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCFDInput)).BeginInit();
            this.tpFatality.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbNotionalNozzleModel
            // 
            this.cbNotionalNozzleModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNotionalNozzleModel.FormattingEnabled = true;
            this.cbNotionalNozzleModel.IntegralHeight = false;
            this.cbNotionalNozzleModel.ItemHeight = 18;
            this.cbNotionalNozzleModel.Location = new System.Drawing.Point(265, 15);
            this.cbNotionalNozzleModel.Margin = new System.Windows.Forms.Padding(4);
            this.cbNotionalNozzleModel.Name = "cbNotionalNozzleModel";
            this.cbNotionalNozzleModel.Size = new System.Drawing.Size(330, 26);
            this.cbNotionalNozzleModel.TabIndex = 3;
            this.cbNotionalNozzleModel.SelectionChangeCommitted += new System.EventHandler(this.cbNotionalNozzleModel_SelectionChangeCommitted);
            // 
            // cbDeflagrationModel
            // 
            this.cbDeflagrationModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbDeflagrationModel.FormattingEnabled = true;
            this.cbDeflagrationModel.Items.AddRange(new object[] {
            "Bauwens/Ekoto",
            "CFD"});
            this.cbDeflagrationModel.Location = new System.Drawing.Point(265, 85);
            this.cbDeflagrationModel.Margin = new System.Windows.Forms.Padding(4);
            this.cbDeflagrationModel.Name = "cbDeflagrationModel";
            this.cbDeflagrationModel.Size = new System.Drawing.Size(330, 26);
            this.cbDeflagrationModel.TabIndex = 5;
            this.cbDeflagrationModel.SelectionChangeCommitted += new System.EventHandler(this.cbDeflagrationModel_SelectionChangeCommitted);
            // 
            // lblNotionalNozzleModel
            // 
            this.lblNotionalNozzleModel.AutoSize = true;
            this.lblNotionalNozzleModel.Location = new System.Drawing.Point(13, 18);
            this.lblNotionalNozzleModel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNotionalNozzleModel.Name = "lblNotionalNozzleModel";
            this.lblNotionalNozzleModel.Size = new System.Drawing.Size(163, 18);
            this.lblNotionalNozzleModel.TabIndex = 6;
            this.lblNotionalNozzleModel.Text = "Notional Nozzle Model:";
            // 
            // lblOPModel
            // 
            this.lblOPModel.AutoSize = true;
            this.lblOPModel.Location = new System.Drawing.Point(14, 90);
            this.lblOPModel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOPModel.Name = "lblOPModel";
            this.lblOPModel.Size = new System.Drawing.Size(136, 18);
            this.lblOPModel.TabIndex = 8;
            this.lblOPModel.Text = "Deflagration Model:";
            // 
            // tcModelSelection
            // 
            this.tcModelSelection.Controls.Add(this.tpPhysicalConsequence);
            this.tcModelSelection.Controls.Add(this.tpFatality);
            this.tcModelSelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcModelSelection.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tcModelSelection.Location = new System.Drawing.Point(0, 0);
            this.tcModelSelection.Margin = new System.Windows.Forms.Padding(4);
            this.tcModelSelection.Name = "tcModelSelection";
            this.tcModelSelection.SelectedIndex = 0;
            this.tcModelSelection.Size = new System.Drawing.Size(932, 794);
            this.tcModelSelection.TabIndex = 9;
            // 
            // tpPhysicalConsequence
            // 
            this.tpPhysicalConsequence.Controls.Add(this.cbRadiativeSourceModel);
            this.tpPhysicalConsequence.Controls.Add(this.gbCFDInput);
            this.tpPhysicalConsequence.Controls.Add(this.lblOPModel);
            this.tpPhysicalConsequence.Controls.Add(this.cbNotionalNozzleModel);
            this.tpPhysicalConsequence.Controls.Add(this.cbDeflagrationModel);
            this.tpPhysicalConsequence.Controls.Add(this.lblNotionalNozzleModel);
            this.tpPhysicalConsequence.Location = new System.Drawing.Point(4, 27);
            this.tpPhysicalConsequence.Margin = new System.Windows.Forms.Padding(4);
            this.tpPhysicalConsequence.Name = "tpPhysicalConsequence";
            this.tpPhysicalConsequence.Padding = new System.Windows.Forms.Padding(4);
            this.tpPhysicalConsequence.Size = new System.Drawing.Size(924, 763);
            this.tpPhysicalConsequence.TabIndex = 0;
            this.tpPhysicalConsequence.Text = "Physical Consequence Models";
            this.tpPhysicalConsequence.UseVisualStyleBackColor = true;
            // 
            // cbRadiativeSourceModel
            // 
            this.cbRadiativeSourceModel.Caption = "Radiative Source Model:";
            this.cbRadiativeSourceModel.Location = new System.Drawing.Point(12, 50);
            this.cbRadiativeSourceModel.Margin = new System.Windows.Forms.Padding(5);
            this.cbRadiativeSourceModel.Name = "cbRadiativeSourceModel";
            this.cbRadiativeSourceModel.SelectedItem = null;
            this.cbRadiativeSourceModel.Size = new System.Drawing.Size(584, 28);
            this.cbRadiativeSourceModel.TabIndex = 20;
            this.cbRadiativeSourceModel.OnValueChanged += new System.EventHandler(this.cbRadiativeSourceModel_OnValueChanged);
            // 
            // gbCFDInput
            // 
            this.gbCFDInput.Controls.Add(this.cbCFDUnits);
            this.gbCFDInput.Controls.Add(this.lblUnits);
            this.gbCFDInput.Controls.Add(this.dgCFDInput);
            this.gbCFDInput.Location = new System.Drawing.Point(17, 133);
            this.gbCFDInput.Margin = new System.Windows.Forms.Padding(4);
            this.gbCFDInput.Name = "gbCFDInput";
            this.gbCFDInput.Padding = new System.Windows.Forms.Padding(4);
            this.gbCFDInput.Size = new System.Drawing.Size(883, 282);
            this.gbCFDInput.TabIndex = 9;
            this.gbCFDInput.TabStop = false;
            this.gbCFDInput.Text = "CFD-Specific Input";
            // 
            // cbCFDUnits
            // 
            this.cbCFDUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbCFDUnits.FormattingEnabled = true;
            this.cbCFDUnits.Location = new System.Drawing.Point(80, 28);
            this.cbCFDUnits.Margin = new System.Windows.Forms.Padding(4);
            this.cbCFDUnits.Name = "cbCFDUnits";
            this.cbCFDUnits.Size = new System.Drawing.Size(218, 26);
            this.cbCFDUnits.TabIndex = 2;
            this.cbCFDUnits.SelectedIndexChanged += new System.EventHandler(this.cbCFDUnits_SelectedIndexChanged);
            // 
            // lblUnits
            // 
            this.lblUnits.AutoSize = true;
            this.lblUnits.Location = new System.Drawing.Point(19, 30);
            this.lblUnits.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUnits.Name = "lblUnits";
            this.lblUnits.Size = new System.Drawing.Size(46, 18);
            this.lblUnits.TabIndex = 1;
            this.lblUnits.Text = "Units:";
            // 
            // dgCFDInput
            // 
            this.dgCFDInput.AllowUserToAddRows = false;
            this.dgCFDInput.AllowUserToDeleteRows = false;
            this.dgCFDInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgCFDInput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgCFDInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgCFDInput.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colVariable,
            this.col0,
            this.col1,
            this.col2,
            this.col3,
            this.col4});
            this.dgCFDInput.Location = new System.Drawing.Point(8, 73);
            this.dgCFDInput.Margin = new System.Windows.Forms.Padding(4);
            this.dgCFDInput.Name = "dgCFDInput";
            this.dgCFDInput.Size = new System.Drawing.Size(867, 201);
            this.dgCFDInput.TabIndex = 0;
            this.dgCFDInput.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgCFDInput_CellValueChanged);
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
            // tpFatality
            // 
            this.tpFatality.Controls.Add(this.cbThermalExposureTimeUnits);
            this.tpFatality.Controls.Add(this.cbThermalProbitModel);
            this.tpFatality.Controls.Add(this.tbThermalExposureTime);
            this.tpFatality.Controls.Add(this.label4);
            this.tpFatality.Controls.Add(this.lblThermalExposureTime);
            this.tpFatality.Controls.Add(this.label3);
            this.tpFatality.Controls.Add(this.cbOverpressureProbitModel);
            this.tpFatality.Location = new System.Drawing.Point(4, 27);
            this.tpFatality.Margin = new System.Windows.Forms.Padding(4);
            this.tpFatality.Name = "tpFatality";
            this.tpFatality.Padding = new System.Windows.Forms.Padding(4);
            this.tpFatality.Size = new System.Drawing.Size(924, 763);
            this.tpFatality.TabIndex = 1;
            this.tpFatality.Text = "Harm Models";
            this.tpFatality.UseVisualStyleBackColor = true;
            // 
            // cbThermalExposureTimeUnits
            // 
            this.cbThermalExposureTimeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbThermalExposureTimeUnits.FormattingEnabled = true;
            this.cbThermalExposureTimeUnits.Location = new System.Drawing.Point(427, 50);
            this.cbThermalExposureTimeUnits.Margin = new System.Windows.Forms.Padding(4);
            this.cbThermalExposureTimeUnits.Name = "cbThermalExposureTimeUnits";
            this.cbThermalExposureTimeUnits.Size = new System.Drawing.Size(168, 26);
            this.cbThermalExposureTimeUnits.TabIndex = 21;
            this.cbThermalExposureTimeUnits.SelectedIndexChanged += new System.EventHandler(this.cbThermalExposureTimeUnits_SelectedIndexChanged);
            // 
            // cbThermalProbitModel
            // 
            this.cbThermalProbitModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbThermalProbitModel.FormattingEnabled = true;
            this.cbThermalProbitModel.Items.AddRange(new object[] {
            "Eisenberg",
            "Tsao",
            "TNO",
            "Lees"});
            this.cbThermalProbitModel.Location = new System.Drawing.Point(265, 15);
            this.cbThermalProbitModel.Margin = new System.Windows.Forms.Padding(4);
            this.cbThermalProbitModel.Name = "cbThermalProbitModel";
            this.cbThermalProbitModel.Size = new System.Drawing.Size(330, 26);
            this.cbThermalProbitModel.TabIndex = 15;
            this.cbThermalProbitModel.SelectionChangeCommitted += new System.EventHandler(this.cbThermalProbitModel_SelectionChangeCommotted);
            // 
            // tbThermalExposureTime
            // 
            this.tbThermalExposureTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbThermalExposureTime.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tbThermalExposureTime.Location = new System.Drawing.Point(265, 50);
            this.tbThermalExposureTime.Margin = new System.Windows.Forms.Padding(4);
            this.tbThermalExposureTime.MaximumSize = new System.Drawing.Size(155, 26);
            this.tbThermalExposureTime.MinimumSize = new System.Drawing.Size(2, 26);
            this.tbThermalExposureTime.Name = "tbThermalExposureTime";
            this.tbThermalExposureTime.Size = new System.Drawing.Size(154, 26);
            this.tbThermalExposureTime.TabIndex = 20;
            this.tbThermalExposureTime.TextChanged += new System.EventHandler(this.tbThermalExposureTime_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 18);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(154, 18);
            this.label4.TabIndex = 17;
            this.label4.Text = "Thermal Probit Model:";
            // 
            // lblThermalExposureTime
            // 
            this.lblThermalExposureTime.AutoSize = true;
            this.lblThermalExposureTime.Location = new System.Drawing.Point(13, 54);
            this.lblThermalExposureTime.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblThermalExposureTime.Name = "lblThermalExposureTime";
            this.lblThermalExposureTime.Size = new System.Drawing.Size(170, 18);
            this.lblThermalExposureTime.TabIndex = 19;
            this.lblThermalExposureTime.Text = "Thermal Exposure Time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 89);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(190, 18);
            this.label3.TabIndex = 18;
            this.label3.Text = "Overpressure Probit Model:";
            // 
            // cbOverpressureProbitModel
            // 
            this.cbOverpressureProbitModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbOverpressureProbitModel.FormattingEnabled = true;
            this.cbOverpressureProbitModel.Items.AddRange(new object[] {
            "Lung Eisenberg",
            "Lung HSE",
            "Head impact",
            "Collapse",
            "Debris"});
            this.cbOverpressureProbitModel.Location = new System.Drawing.Point(265, 85);
            this.cbOverpressureProbitModel.Margin = new System.Windows.Forms.Padding(4);
            this.cbOverpressureProbitModel.Name = "cbOverpressureProbitModel";
            this.cbOverpressureProbitModel.Size = new System.Drawing.Size(330, 26);
            this.cbOverpressureProbitModel.TabIndex = 16;
            this.cbOverpressureProbitModel.SelectionChangeCommitted += new System.EventHandler(this.cbOverpressureProbitModel_SelectionChangeCommotted);
            // 
            // cpConsequenceModels
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcModelSelection);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CpConsequenceModels";
            this.Size = new System.Drawing.Size(932, 794);
            this.Load += new System.EventHandler(this.cpConsequenceModels_Load);
            this.tcModelSelection.ResumeLayout(false);
            this.tpPhysicalConsequence.ResumeLayout(false);
            this.tpPhysicalConsequence.PerformLayout();
            this.gbCFDInput.ResumeLayout(false);
            this.gbCFDInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgCFDInput)).EndInit();
            this.tpFatality.ResumeLayout(false);
            this.tpFatality.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cbNotionalNozzleModel;
		private System.Windows.Forms.ComboBox cbDeflagrationModel;
		private System.Windows.Forms.Label lblNotionalNozzleModel;
		private System.Windows.Forms.Label lblOPModel;
		private System.Windows.Forms.TabControl tcModelSelection;
		private System.Windows.Forms.TabPage tpPhysicalConsequence;
		private System.Windows.Forms.TabPage tpFatality;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cbOverpressureProbitModel;
		private System.Windows.Forms.GroupBox gbCFDInput;
		private System.Windows.Forms.DataGridView dgCFDInput;
		private System.Windows.Forms.ComboBox cbCFDUnits;
        private System.Windows.Forms.Label lblUnits;
		private System.Windows.Forms.DataGridViewTextBoxColumn colVariable;
		private System.Windows.Forms.DataGridViewTextBoxColumn col0;
		private System.Windows.Forms.DataGridViewTextBoxColumn col1;
		private System.Windows.Forms.DataGridViewTextBoxColumn col2;
		private System.Windows.Forms.DataGridViewTextBoxColumn col3;
		private System.Windows.Forms.DataGridViewTextBoxColumn col4;
		private UIHelpers.AnyEnumComboSelector cbRadiativeSourceModel;
        private System.Windows.Forms.ComboBox cbThermalExposureTimeUnits;
        private System.Windows.Forms.ComboBox cbThermalProbitModel;
        private System.Windows.Forms.TextBox tbThermalExposureTime;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblThermalExposureTime;
    }
}
