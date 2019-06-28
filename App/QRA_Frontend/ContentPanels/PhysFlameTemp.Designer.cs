namespace QRA_Frontend.ContentPanels {
	partial class CpPlotT {
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
            this.tcIO = new System.Windows.Forms.TabControl();
            this.tpInput = new System.Windows.Forms.TabPage();
            this.pbSpinner = new System.Windows.Forms.PictureBox();
            this.btnExecute = new System.Windows.Forms.Button();
            this.dgInput = new System.Windows.Forms.DataGridView();
            this.nnmsModelSelector = new QRA_Frontend.CustomControls.NotionalNozzleModelSelector();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.pbOutput = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.tcIO.SuspendLayout();
            this.tpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).BeginInit();
            this.tpOutput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).BeginInit();
            this.SuspendLayout();
            // 
            // tcIO
            // 
            this.tcIO.Controls.Add(this.tpInput);
            this.tcIO.Controls.Add(this.tpOutput);
            this.tcIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcIO.Location = new System.Drawing.Point(0, 0);
            this.tcIO.Margin = new System.Windows.Forms.Padding(4);
            this.tcIO.Name = "tcIO";
            this.tcIO.SelectedIndex = 0;
            this.tcIO.Size = new System.Drawing.Size(863, 759);
            this.tcIO.TabIndex = 0;
            // 
            // tpInput
            // 
            this.tpInput.Controls.Add(this.pbSpinner);
            this.tpInput.Controls.Add(this.btnExecute);
            this.tpInput.Controls.Add(this.dgInput);
            this.tpInput.Controls.Add(this.nnmsModelSelector);
            this.tpInput.Location = new System.Drawing.Point(4, 25);
            this.tpInput.Margin = new System.Windows.Forms.Padding(4);
            this.tpInput.Name = "tpInput";
            this.tpInput.Padding = new System.Windows.Forms.Padding(4);
            this.tpInput.Size = new System.Drawing.Size(855, 730);
            this.tpInput.TabIndex = 0;
            this.tpInput.Text = "Input";
            this.tpInput.UseVisualStyleBackColor = true;
            // 
            // pbSpinner
            // 
            this.pbSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSpinner.Image = global::QRA_Frontend.Properties.Resources.AjaxSpinner;
            this.pbSpinner.Location = new System.Drawing.Point(705, 692);
            this.pbSpinner.MinimumSize = new System.Drawing.Size(20, 20);
            this.pbSpinner.Name = "pbSpinner";
            this.pbSpinner.Size = new System.Drawing.Size(32, 28);
            this.pbSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSpinner.TabIndex = 19;
            this.pbSpinner.TabStop = false;
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(744, 692);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(4);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(100, 28);
            this.btnExecute.TabIndex = 15;
            this.btnExecute.Text = "Calculate";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // dgInput
            // 
            this.dgInput.AllowUserToAddRows = false;
            this.dgInput.AllowUserToDeleteRows = false;
            this.dgInput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgInput.Location = new System.Drawing.Point(5, 45);
            this.dgInput.Margin = new System.Windows.Forms.Padding(4);
            this.dgInput.Name = "dgInput";
            this.dgInput.Size = new System.Drawing.Size(841, 639);
            this.dgInput.TabIndex = 3;
            this.dgInput.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgInput_CellValueChanged);
            // 
            // nnmsModelSelector
            // 
            this.nnmsModelSelector.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.nnmsModelSelector.CanChange = true;
            this.nnmsModelSelector.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nnmsModelSelector.Location = new System.Drawing.Point(8, 5);
            this.nnmsModelSelector.Margin = new System.Windows.Forms.Padding(5);
            this.nnmsModelSelector.Name = "nnmsModelSelector";
            this.nnmsModelSelector.Size = new System.Drawing.Size(562, 31);
            this.nnmsModelSelector.TabIndex = 2;
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.pbOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 25);
            this.tpOutput.Margin = new System.Windows.Forms.Padding(4);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(4);
            this.tpOutput.Size = new System.Drawing.Size(855, 730);
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
            this.pbOutput.Size = new System.Drawing.Size(847, 722);
            this.pbOutput.TabIndex = 2;
            this.pbOutput.TabStop = false;
            // 
            // cpPlotT
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcIO);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CpPlotT";
            this.Size = new System.Drawing.Size(863, 759);
            this.tcIO.ResumeLayout(false);
            this.tpInput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).EndInit();
            this.tpOutput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbOutput)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcIO;
		private System.Windows.Forms.TabPage tpInput;
		private CustomControls.NotionalNozzleModelSelector nnmsModelSelector;
		private System.Windows.Forms.TabPage tpOutput;
		private System.Windows.Forms.DataGridView dgInput;
		private System.Windows.Forms.Button btnExecute;
		private CustomControls.PictureBoxWithSave pbOutput;
        private System.Windows.Forms.PictureBox pbSpinner;
    }
}
