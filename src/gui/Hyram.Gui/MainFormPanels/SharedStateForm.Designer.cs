namespace SandiaNationalLaboratories.Hyram
{
    partial class SharedStateForm
    {

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ParameterGrid = new System.Windows.Forms.DataGridView();
            this.NozzleSelector = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.FuelGrid = new System.Windows.Forms.DataGridView();
            this.Active = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LabelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.formula = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.percent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AllocationSelector = new System.Windows.Forms.ComboBox();
            this.FuelSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.FuelCompAlert = new System.Windows.Forms.Label();
            this.totalAmountLabel = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.AllocateBtn = new System.Windows.Forms.Button();
            this.fuelTypeLabel2 = new System.Windows.Forms.Label();
            this.StateAlert = new System.Windows.Forms.Label();
            this.pressureAbsoluteToggle = new System.Windows.Forms.CheckBox();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParameterGrid)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FuelGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // PhaseSelector
            // 
            this.PhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PhaseSelector.FormattingEnabled = true;
            this.PhaseSelector.Location = new System.Drawing.Point(171, 27);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(126, 21);
            this.PhaseSelector.TabIndex = 5;
            this.PhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.PhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phaseLabel.Location = new System.Drawing.Point(14, 28);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(71, 15);
            this.phaseLabel.TabIndex = 55;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.StateAlert);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(8);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(5);
            this.panel1.Size = new System.Drawing.Size(1154, 594);
            this.panel1.TabIndex = 57;
            // 
            // groupBox2
            // 
            this.groupBox2.AutoSize = true;
            this.groupBox2.Controls.Add(this.pressureAbsoluteToggle);
            this.groupBox2.Controls.Add(this.ParameterGrid);
            this.groupBox2.Controls.Add(this.NozzleSelector);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.phaseLabel);
            this.groupBox2.Controls.Add(this.PhaseSelector);
            this.groupBox2.Location = new System.Drawing.Point(436, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(464, 546);
            this.groupBox2.TabIndex = 79;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Common Inputs";
            // 
            // ParameterGrid
            // 
            this.ParameterGrid.AllowUserToAddRows = false;
            this.ParameterGrid.AllowUserToDeleteRows = false;
            this.ParameterGrid.AllowUserToResizeRows = false;
            this.ParameterGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ParameterGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ParameterGrid.Location = new System.Drawing.Point(17, 118);
            this.ParameterGrid.Margin = new System.Windows.Forms.Padding(2);
            this.ParameterGrid.Name = "ParameterGrid";
            this.ParameterGrid.RowHeadersVisible = false;
            this.ParameterGrid.RowTemplate.Height = 24;
            this.ParameterGrid.ShowEditingIcon = false;
            this.ParameterGrid.Size = new System.Drawing.Size(418, 281);
            this.ParameterGrid.TabIndex = 7;
            this.ParameterGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.ParameterGrid_CellValidating);
            this.ParameterGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.ParameterGrid_CellValueChanged);
            this.ParameterGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.ParameterGrid_DataError);
            // 
            // NozzleSelector
            // 
            this.NozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NozzleSelector.FormattingEnabled = true;
            this.NozzleSelector.Location = new System.Drawing.Point(171, 54);
            this.NozzleSelector.Name = "NozzleSelector";
            this.NozzleSelector.Size = new System.Drawing.Size(126, 21);
            this.NozzleSelector.TabIndex = 6;
            this.NozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.NozzleSelector_SelectionChangeCommitted);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(14, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(156, 18);
            this.label4.TabIndex = 78;
            this.label4.Text = "Notional nozzle model";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.FuelGrid);
            this.groupBox1.Controls.Add(this.AllocationSelector);
            this.groupBox1.Controls.Add(this.FuelSelector);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.FuelCompAlert);
            this.groupBox1.Controls.Add(this.totalAmountLabel);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.AllocateBtn);
            this.groupBox1.Controls.Add(this.fuelTypeLabel2);
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(387, 546);
            this.groupBox1.TabIndex = 78;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fuel Specification";
            // 
            // FuelGrid
            // 
            this.FuelGrid.AllowUserToAddRows = false;
            this.FuelGrid.AllowUserToDeleteRows = false;
            this.FuelGrid.AllowUserToResizeRows = false;
            this.FuelGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.FuelGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.FuelGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Active,
            this.LabelColumn,
            this.formula,
            this.percent});
            this.FuelGrid.Location = new System.Drawing.Point(9, 88);
            this.FuelGrid.MultiSelect = false;
            this.FuelGrid.Name = "FuelGrid";
            this.FuelGrid.RowHeadersVisible = false;
            this.FuelGrid.ShowEditingIcon = false;
            this.FuelGrid.Size = new System.Drawing.Size(361, 311);
            this.FuelGrid.TabIndex = 2;
            this.FuelGrid.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.FuelGrid_CellParsing);
            this.FuelGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.FuelGrid_CellValueChanged);
            this.FuelGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.FuelGrid_DataError);
            // 
            // Active
            // 
            this.Active.DataPropertyName = "Active";
            this.Active.FillWeight = 25F;
            this.Active.HeaderText = "Active";
            this.Active.MinimumWidth = 20;
            this.Active.Name = "Active";
            this.Active.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // LabelColumn
            // 
            this.LabelColumn.FillWeight = 45F;
            this.LabelColumn.HeaderText = "Fuel";
            this.LabelColumn.MinimumWidth = 25;
            this.LabelColumn.Name = "LabelColumn";
            this.LabelColumn.ReadOnly = true;
            // 
            // formula
            // 
            this.formula.FillWeight = 35F;
            this.formula.HeaderText = "Formula";
            this.formula.Name = "formula";
            this.formula.ReadOnly = true;
            // 
            // percent
            // 
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.percent.DefaultCellStyle = dataGridViewCellStyle4;
            this.percent.FillWeight = 50F;
            this.percent.HeaderText = "Percent (vol-%)";
            this.percent.Name = "percent";
            // 
            // AllocationSelector
            // 
            this.AllocationSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AllocationSelector.FormattingEnabled = true;
            this.AllocationSelector.Location = new System.Drawing.Point(181, 440);
            this.AllocationSelector.Name = "AllocationSelector";
            this.AllocationSelector.Size = new System.Drawing.Size(93, 21);
            this.AllocationSelector.TabIndex = 3;
            // 
            // FuelSelector
            // 
            this.FuelSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FuelSelector.FormattingEnabled = true;
            this.FuelSelector.Location = new System.Drawing.Point(149, 27);
            this.FuelSelector.Name = "FuelSelector";
            this.FuelSelector.Size = new System.Drawing.Size(107, 21);
            this.FuelSelector.TabIndex = 1;
            this.FuelSelector.SelectionChangeCommitted += new System.EventHandler(this.FuelSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 441);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 15);
            this.label1.TabIndex = 64;
            this.label1.Text = "Allocate remainder:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(228, 409);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 16);
            this.label2.TabIndex = 65;
            this.label2.Text = "Total";
            // 
            // FuelCompAlert
            // 
            this.FuelCompAlert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.FuelCompAlert.AutoSize = true;
            this.FuelCompAlert.BackColor = System.Drawing.Color.MistyRose;
            this.FuelCompAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FuelCompAlert.ForeColor = System.Drawing.Color.Maroon;
            this.FuelCompAlert.Location = new System.Drawing.Point(6, 487);
            this.FuelCompAlert.Name = "FuelCompAlert";
            this.FuelCompAlert.Padding = new System.Windows.Forms.Padding(4);
            this.FuelCompAlert.Size = new System.Drawing.Size(272, 23);
            this.FuelCompAlert.TabIndex = 70;
            this.FuelCompAlert.Text = "Total fuel composition must equal 100%";
            this.FuelCompAlert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.FuelCompAlert.Visible = false;
            // 
            // totalAmountLabel
            // 
            this.totalAmountLabel.AutoSize = true;
            this.totalAmountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalAmountLabel.Location = new System.Drawing.Point(297, 409);
            this.totalAmountLabel.Name = "totalAmountLabel";
            this.totalAmountLabel.Size = new System.Drawing.Size(73, 16);
            this.totalAmountLabel.TabIndex = 66;
            this.totalAmountLabel.Text = "100.000%";
            this.totalAmountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(330, 15);
            this.label3.TabIndex = 69;
            this.label3.Text = "Specify single fuel or fuel blend by adjusting concentrations.";
            // 
            // AllocateBtn
            // 
            this.AllocateBtn.Location = new System.Drawing.Point(277, 439);
            this.AllocateBtn.Name = "AllocateBtn";
            this.AllocateBtn.Size = new System.Drawing.Size(93, 23);
            this.AllocateBtn.TabIndex = 4;
            this.AllocateBtn.Text = "Allocate";
            this.AllocateBtn.UseVisualStyleBackColor = true;
            this.AllocateBtn.Click += new System.EventHandler(this.AllocateBtn_Click);
            // 
            // fuelTypeLabel2
            // 
            this.fuelTypeLabel2.AutoSize = true;
            this.fuelTypeLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fuelTypeLabel2.Location = new System.Drawing.Point(6, 28);
            this.fuelTypeLabel2.Name = "fuelTypeLabel2";
            this.fuelTypeLabel2.Size = new System.Drawing.Size(122, 15);
            this.fuelTypeLabel2.TabIndex = 68;
            this.fuelTypeLabel2.Text = "Fuel (overrides table)";
            // 
            // StateAlert
            // 
            this.StateAlert.AutoSize = true;
            this.StateAlert.BackColor = System.Drawing.Color.MistyRose;
            this.StateAlert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StateAlert.ForeColor = System.Drawing.Color.Maroon;
            this.StateAlert.Location = new System.Drawing.Point(8, 557);
            this.StateAlert.Name = "StateAlert";
            this.StateAlert.Padding = new System.Windows.Forms.Padding(4);
            this.StateAlert.Size = new System.Drawing.Size(198, 23);
            this.StateAlert.TabIndex = 61;
            this.StateAlert.Text = "Warning/error message here";
            this.StateAlert.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.StateAlert.Visible = false;
            // 
            // pressureAbsoluteToggle
            // 
            this.pressureAbsoluteToggle.AutoSize = true;
            this.pressureAbsoluteToggle.Location = new System.Drawing.Point(171, 82);
            this.pressureAbsoluteToggle.Name = "pressureAbsoluteToggle";
            this.pressureAbsoluteToggle.Size = new System.Drawing.Size(144, 17);
            this.pressureAbsoluteToggle.TabIndex = 79;
            this.pressureAbsoluteToggle.Text = "Fluid pressure is absolute";
            this.pressureAbsoluteToggle.UseVisualStyleBackColor = true;
            this.pressureAbsoluteToggle.CheckedChanged += new System.EventHandler(this.pressureAbsoluteToggle_CheckedChanged);
            // 
            // FuelSpecForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "FuelSpecForm";
            this.Size = new System.Drawing.Size(1154, 594);
            this.Load += new System.EventHandler(this.FuelSpecForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ParameterGrid)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FuelGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox PhaseSelector;
        private System.Windows.Forms.Label phaseLabel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label StateAlert;
        private System.Windows.Forms.Label FuelCompAlert;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label fuelTypeLabel2;
        private System.Windows.Forms.Button AllocateBtn;
        private System.Windows.Forms.Label totalAmountLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox AllocationSelector;
        private System.Windows.Forms.DataGridView FuelGrid;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Active;
        private System.Windows.Forms.DataGridViewTextBoxColumn LabelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn formula;
        private System.Windows.Forms.DataGridViewTextBoxColumn percent;
        private System.Windows.Forms.ComboBox FuelSelector;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox NozzleSelector;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DataGridView ParameterGrid;
        private System.Windows.Forms.CheckBox pressureAbsoluteToggle;
    }
}
