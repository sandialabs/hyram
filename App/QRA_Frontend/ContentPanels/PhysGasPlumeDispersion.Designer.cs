namespace QRA_Frontend.ContentPanels {
	partial class CpGasPlumeDispersion {
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
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpInput = new System.Windows.Forms.TabPage();
            this.PlumeParams = new System.Windows.Forms.DataGridView();
            this.btnCalculate = new System.Windows.Forms.Button();
            this.tbPlotTitle = new System.Windows.Forms.TextBox();
            this.lblPlotTitle = new System.Windows.Forms.Label();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.pbOutput = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.pbSpinner = new System.Windows.Forms.PictureBox();
            this.tcMain.SuspendLayout();
            this.tpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PlumeParams)).BeginInit();
            this.tpOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpInput);
            this.tcMain.Controls.Add(this.tpOutput);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Margin = new System.Windows.Forms.Padding(4);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(779, 450);
            this.tcMain.TabIndex = 0;
            // 
            // tpInput
            // 
            this.tpInput.Controls.Add(this.pbSpinner);
            this.tpInput.Controls.Add(this.PlumeParams);
            this.tpInput.Controls.Add(this.btnCalculate);
            this.tpInput.Controls.Add(this.tbPlotTitle);
            this.tpInput.Controls.Add(this.lblPlotTitle);
            this.tpInput.Location = new System.Drawing.Point(4, 25);
            this.tpInput.Margin = new System.Windows.Forms.Padding(4);
            this.tpInput.Name = "tpInput";
            this.tpInput.Padding = new System.Windows.Forms.Padding(4);
            this.tpInput.Size = new System.Drawing.Size(771, 421);
            this.tpInput.TabIndex = 0;
            this.tpInput.Text = "Input";
            this.tpInput.UseVisualStyleBackColor = true;
            // 
            // PlumeParams
            // 
            this.PlumeParams.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlumeParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PlumeParams.Location = new System.Drawing.Point(9, 39);
            this.PlumeParams.Name = "PlumeParams";
            this.PlumeParams.RowTemplate.Height = 24;
            this.PlumeParams.Size = new System.Drawing.Size(755, 331);
            this.PlumeParams.TabIndex = 0;
            this.PlumeParams.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.PlumeParams_CellValueChanged);
            // 
            // btnCalculate
            // 
            this.btnCalculate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCalculate.Location = new System.Drawing.Point(667, 377);
            this.btnCalculate.Margin = new System.Windows.Forms.Padding(4);
            this.btnCalculate.Name = "btnCalculate";
            this.btnCalculate.Size = new System.Drawing.Size(100, 28);
            this.btnCalculate.TabIndex = 1;
            this.btnCalculate.Text = "Calculate";
            this.btnCalculate.UseVisualStyleBackColor = true;
            this.btnCalculate.Click += new System.EventHandler(this.btnCalculate_Click);
            // 
            // tbPlotTitle
            // 
            this.tbPlotTitle.Location = new System.Drawing.Point(94, 10);
            this.tbPlotTitle.Margin = new System.Windows.Forms.Padding(4);
            this.tbPlotTitle.Name = "tbPlotTitle";
            this.tbPlotTitle.Size = new System.Drawing.Size(152, 22);
            this.tbPlotTitle.TabIndex = 1;
            this.tbPlotTitle.Text = "Mole Fraction of Leak";
            this.tbPlotTitle.TextChanged += new System.EventHandler(this.tbPlotTitle_TextChanged);
            // 
            // lblPlotTitle
            // 
            this.lblPlotTitle.AutoSize = true;
            this.lblPlotTitle.Location = new System.Drawing.Point(6, 13);
            this.lblPlotTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPlotTitle.Name = "lblPlotTitle";
            this.lblPlotTitle.Size = new System.Drawing.Size(63, 17);
            this.lblPlotTitle.TabIndex = 0;
            this.lblPlotTitle.Text = "Plot Title";
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.pbOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 25);
            this.tpOutput.Margin = new System.Windows.Forms.Padding(4);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(4);
            this.tpOutput.Size = new System.Drawing.Size(771, 421);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // pbOutput
            // 
            this.pbOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbOutput.Location = new System.Drawing.Point(4, 4);
            this.pbOutput.Margin = new System.Windows.Forms.Padding(4);
            this.pbOutput.Name = "pbOutput";
            this.pbOutput.Size = new System.Drawing.Size(763, 413);
            this.pbOutput.TabIndex = 0;
            this.pbOutput.TabStop = false;
            // 
            // pbSpinner
            // 
            this.pbSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSpinner.Image = global::QRA_Frontend.Properties.Resources.AjaxSpinner;
            this.pbSpinner.Location = new System.Drawing.Point(628, 376);
            this.pbSpinner.MinimumSize = new System.Drawing.Size(20, 20);
            this.pbSpinner.Name = "pbSpinner";
            this.pbSpinner.Size = new System.Drawing.Size(32, 28);
            this.pbSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSpinner.TabIndex = 21;
            this.pbSpinner.TabStop = false;
            // 
            // cpGasPlumeDispersion
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcMain);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CpGasPlumeDispersion";
            this.Size = new System.Drawing.Size(779, 450);
            this.Load += new System.EventHandler(this.cpGasPlumeDispersion_Load);
            this.tcMain.ResumeLayout(false);
            this.tpInput.ResumeLayout(false);
            this.tpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PlumeParams)).EndInit();
            this.tpOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcMain;
		private System.Windows.Forms.TabPage tpInput;
		private System.Windows.Forms.TabPage tpOutput;
		private System.Windows.Forms.Label lblPlotTitle;
		private System.Windows.Forms.TextBox tbPlotTitle;
		private System.Windows.Forms.Button btnCalculate;
		private CustomControls.PictureBoxWithSave pbOutput;
        private System.Windows.Forms.DataGridView PlumeParams;
        private System.Windows.Forms.PictureBox pbSpinner;
    }
}
