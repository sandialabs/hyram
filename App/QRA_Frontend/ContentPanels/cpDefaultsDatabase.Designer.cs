namespace QRA_Frontend.ContentPanels {
	partial class CpDefaultsDatabase {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.wbConstants = new System.Windows.Forms.WebBrowser();
            this.dgConstants = new System.Windows.Forms.DataGridView();
            this.tcGrids = new System.Windows.Forms.TabControl();
            this.tpConstants = new System.Windows.Forms.TabPage();
            this.tpVariable = new System.Windows.Forms.TabPage();
            this.dgVariable = new System.Windows.Forms.DataGridView();
            this.coldgDefaultsVariableName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.coldgDefaultsValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDefault = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgConstants)).BeginInit();
            this.tcGrids.SuspendLayout();
            this.tpConstants.SuspendLayout();
            this.tpVariable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgVariable)).BeginInit();
            this.SuspendLayout();
            // 
            // wbConstants
            // 
            this.wbConstants.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.wbConstants.Location = new System.Drawing.Point(0, 3);
            this.wbConstants.MinimumSize = new System.Drawing.Size(20, 20);
            this.wbConstants.Name = "wbConstants";
            this.wbConstants.Size = new System.Drawing.Size(645, 269);
            this.wbConstants.TabIndex = 0;
            this.wbConstants.Visible = false;
            // 
            // dgConstants
            // 
            this.dgConstants.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgConstants.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.coldgDefaultsVariableName,
            this.coldgDefaultsValue});
            this.dgConstants.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgConstants.Location = new System.Drawing.Point(3, 3);
            this.dgConstants.Name = "dgConstants";
            this.dgConstants.Size = new System.Drawing.Size(666, 506);
            this.dgConstants.TabIndex = 1;
            // 
            // tcGrids
            // 
            this.tcGrids.Controls.Add(this.tpConstants);
            this.tcGrids.Controls.Add(this.tpVariable);
            this.tcGrids.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcGrids.Location = new System.Drawing.Point(0, 0);
            this.tcGrids.Name = "tcGrids";
            this.tcGrids.SelectedIndex = 0;
            this.tcGrids.Size = new System.Drawing.Size(680, 538);
            this.tcGrids.TabIndex = 2;
            // 
            // tpConstants
            // 
            this.tpConstants.Controls.Add(this.dgConstants);
            this.tpConstants.Location = new System.Drawing.Point(4, 22);
            this.tpConstants.Name = "tpConstants";
            this.tpConstants.Padding = new System.Windows.Forms.Padding(3);
            this.tpConstants.Size = new System.Drawing.Size(672, 512);
            this.tpConstants.TabIndex = 0;
            this.tpConstants.Text = "Constants";
            this.tpConstants.UseVisualStyleBackColor = true;
            // 
            // tpVariable
            // 
            this.tpVariable.Controls.Add(this.dgVariable);
            this.tpVariable.Location = new System.Drawing.Point(4, 22);
            this.tpVariable.Name = "tpVariable";
            this.tpVariable.Padding = new System.Windows.Forms.Padding(3);
            this.tpVariable.Size = new System.Drawing.Size(672, 512);
            this.tpVariable.TabIndex = 1;
            this.tpVariable.Text = "Variable";
            this.tpVariable.UseVisualStyleBackColor = true;
            // 
            // dgVariable
            // 
            this.dgVariable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgVariable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.colDefault,
            this.colUser});
            this.dgVariable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgVariable.Location = new System.Drawing.Point(3, 3);
            this.dgVariable.Name = "dgVariable";
            this.dgVariable.Size = new System.Drawing.Size(666, 506);
            this.dgVariable.TabIndex = 2;
            // 
            // coldgDefaultsVariableName
            // 
            this.coldgDefaultsVariableName.HeaderText = "Name";
            this.coldgDefaultsVariableName.Name = "coldgDefaultsVariableName";
            this.coldgDefaultsVariableName.ReadOnly = true;
            // 
            // coldgDefaultsValue
            // 
            this.coldgDefaultsValue.HeaderText = "Value";
            this.coldgDefaultsValue.Name = "coldgDefaultsValue";
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Name";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // colDefault
            // 
            this.colDefault.HeaderText = "Default";
            this.colDefault.Name = "colDefault";
            this.colDefault.ReadOnly = true;
            // 
            // colUser
            // 
            this.colUser.HeaderText = "Session (user)";
            this.colUser.Name = "colUser";
            // 
            // cpDefaultsDatabase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcGrids);
            this.Controls.Add(this.wbConstants);
            this.Name = "CpDefaultsDatabase";
            this.Size = new System.Drawing.Size(680, 538);
            ((System.ComponentModel.ISupportInitialize)(this.dgConstants)).EndInit();
            this.tcGrids.ResumeLayout(false);
            this.tpConstants.ResumeLayout(false);
            this.tpVariable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgVariable)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser wbConstants;
        private System.Windows.Forms.DataGridView dgConstants;
        private System.Windows.Forms.TabControl tcGrids;
        private System.Windows.Forms.TabPage tpConstants;
        private System.Windows.Forms.TabPage tpVariable;
        private System.Windows.Forms.DataGridView dgVariable;
        private System.Windows.Forms.DataGridViewTextBoxColumn coldgDefaultsVariableName;
        private System.Windows.Forms.DataGridViewTextBoxColumn coldgDefaultsValue;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDefault;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUser;

	}
}
