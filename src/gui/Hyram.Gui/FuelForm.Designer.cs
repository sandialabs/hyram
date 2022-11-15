
namespace SandiaNationalLaboratories.Hyram
{
    partial class FuelForm
    {


        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.FuelDataGrid = new System.Windows.Forms.DataGridView();
            this.Active = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LabelColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.formula = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.percent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.closeBtn = new System.Windows.Forms.Button();
            this.AllocationSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.totalAmountLabel = new System.Windows.Forms.Label();
            this.AllocateBtn = new System.Windows.Forms.Button();
            this.fuelTypeLabel2 = new System.Windows.Forms.Label();
            this.FuelSelector = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.alertLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.FuelDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // FuelDataGrid
            // 
            this.FuelDataGrid.AllowUserToAddRows = false;
            this.FuelDataGrid.AllowUserToDeleteRows = false;
            this.FuelDataGrid.AllowUserToResizeRows = false;
            this.FuelDataGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.FuelDataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.FuelDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Active,
            this.LabelColumn,
            this.formula,
            this.percent});
            this.FuelDataGrid.Location = new System.Drawing.Point(12, 92);
            this.FuelDataGrid.MultiSelect = false;
            this.FuelDataGrid.Name = "FuelDataGrid";
            this.FuelDataGrid.RowHeadersVisible = false;
            this.FuelDataGrid.ShowEditingIcon = false;
            this.FuelDataGrid.Size = new System.Drawing.Size(328, 322);
            this.FuelDataGrid.TabIndex = 0;
            this.FuelDataGrid.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.FuelDataGrid_CellParsing);
            this.FuelDataGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.FuelDataGrid_CellValueChanged);
            this.FuelDataGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.FuelDataGrid_DataError);
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
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.percent.DefaultCellStyle = dataGridViewCellStyle2;
            this.percent.FillWeight = 50F;
            this.percent.HeaderText = "Percent (vol-%)";
            this.percent.Name = "percent";
            // 
            // closeBtn
            // 
            this.closeBtn.Location = new System.Drawing.Point(247, 561);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(93, 23);
            this.closeBtn.TabIndex = 1;
            this.closeBtn.Text = "Close";
            this.closeBtn.UseVisualStyleBackColor = true;
            this.closeBtn.Click += new System.EventHandler(this.CloseBtn_Click);
            // 
            // AllocationSelector
            // 
            this.AllocationSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AllocationSelector.FormattingEnabled = true;
            this.AllocationSelector.Location = new System.Drawing.Point(151, 462);
            this.AllocationSelector.Name = "AllocationSelector";
            this.AllocationSelector.Size = new System.Drawing.Size(93, 21);
            this.AllocationSelector.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 462);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Allocate remainder:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(200, 426);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 16);
            this.label2.TabIndex = 5;
            this.label2.Text = "Total";
            // 
            // totalAmountLabel
            // 
            this.totalAmountLabel.AutoSize = true;
            this.totalAmountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.totalAmountLabel.Location = new System.Drawing.Point(269, 426);
            this.totalAmountLabel.Name = "totalAmountLabel";
            this.totalAmountLabel.Size = new System.Drawing.Size(73, 16);
            this.totalAmountLabel.TabIndex = 6;
            this.totalAmountLabel.Text = "100.000%";
            this.totalAmountLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // AllocateBtn
            // 
            this.AllocateBtn.Location = new System.Drawing.Point(247, 461);
            this.AllocateBtn.Name = "AllocateBtn";
            this.AllocateBtn.Size = new System.Drawing.Size(93, 23);
            this.AllocateBtn.TabIndex = 7;
            this.AllocateBtn.Text = "Allocate";
            this.AllocateBtn.UseVisualStyleBackColor = true;
            this.AllocateBtn.Click += new System.EventHandler(this.AllocateBtn_Click);
            // 
            // fuelTypeLabel2
            // 
            this.fuelTypeLabel2.AutoSize = true;
            this.fuelTypeLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fuelTypeLabel2.Location = new System.Drawing.Point(122, 66);
            this.fuelTypeLabel2.Name = "fuelTypeLabel2";
            this.fuelTypeLabel2.Size = new System.Drawing.Size(122, 15);
            this.fuelTypeLabel2.TabIndex = 15;
            this.fuelTypeLabel2.Text = "Fuel (overrides table)";
            // 
            // FuelSelector
            // 
            this.FuelSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FuelSelector.FormattingEnabled = true;
            this.FuelSelector.Location = new System.Drawing.Point(247, 65);
            this.FuelSelector.Name = "FuelSelector";
            this.FuelSelector.Size = new System.Drawing.Size(93, 21);
            this.FuelSelector.TabIndex = 14;
            this.FuelSelector.SelectionChangeCommitted += new System.EventHandler(this.FuelSelector_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(9, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(267, 32);
            this.label3.TabIndex = 16;
            this.label3.Text = "Specify single fuel or fuel blend by adjusting\r\nconcentrations.";
            // 
            // alertLabel
            // 
            this.alertLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.alertLabel.AutoSize = true;
            this.alertLabel.BackColor = System.Drawing.Color.MistyRose;
            this.alertLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertLabel.ForeColor = System.Drawing.Color.Maroon;
            this.alertLabel.Location = new System.Drawing.Point(9, 562);
            this.alertLabel.Name = "alertLabel";
            this.alertLabel.Padding = new System.Windows.Forms.Padding(4);
            this.alertLabel.Size = new System.Drawing.Size(239, 21);
            this.alertLabel.TabIndex = 18;
            this.alertLabel.Text = "Total fuel composition must equal 100%";
            this.alertLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.alertLabel.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(11, 509);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(318, 45);
            this.label4.TabIndex = 19;
            this.label4.Text = "Note: blends capabilities have not been validated due to \r\nlimited availability o" +
    "f blends data. Analyses of blends\r\nmay fail to solve or may require additional t" +
    "ime (>10 min).";
            // 
            // FuelForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(352, 596);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.alertLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.fuelTypeLabel2);
            this.Controls.Add(this.FuelSelector);
            this.Controls.Add(this.AllocateBtn);
            this.Controls.Add(this.totalAmountLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.AllocationSelector);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.FuelDataGrid);
            this.DoubleBuffered = true;
            this.Name = "FuelForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Fuel Specification";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FuelForm_FormClosing);
            this.Load += new System.EventHandler(this.FuelForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.FuelDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView FuelDataGrid;
        private System.Windows.Forms.Button closeBtn;
        private System.Windows.Forms.ComboBox AllocationSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label totalAmountLabel;
        private System.Windows.Forms.Button AllocateBtn;
        private System.Windows.Forms.Label fuelTypeLabel2;
        private System.Windows.Forms.ComboBox FuelSelector;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label alertLabel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Active;
        private System.Windows.Forms.DataGridViewTextBoxColumn LabelColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn formula;
        private System.Windows.Forms.DataGridViewTextBoxColumn percent;
        private System.Windows.Forms.Label label4;
    }
}