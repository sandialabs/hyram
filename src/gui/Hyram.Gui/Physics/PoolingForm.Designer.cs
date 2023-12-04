namespace SandiaNationalLaboratories.Hyram
{
    partial class PoolingForm
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.spinner = new System.Windows.Forms.PictureBox();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.inputWarning = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.resultTipLabel = new System.Windows.Forms.Label();
            this.AutoSetLimits = new System.Windows.Forms.CheckBox();
            this.OutputPlot = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.spinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPlot)).BeginInit();
            this.SuspendLayout();
            // 
            // spinner
            // 
            this.spinner.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinner.Location = new System.Drawing.Point(259, 465);
            this.spinner.Margin = new System.Windows.Forms.Padding(2);
            this.spinner.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinner.Name = "spinner";
            this.spinner.Size = new System.Drawing.Size(24, 23);
            this.spinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinner.TabIndex = 63;
            this.spinner.TabStop = false;
            // 
            // InputGrid
            // 
            this.InputGrid.AllowUserToAddRows = false;
            this.InputGrid.AllowUserToDeleteRows = false;
            this.InputGrid.AllowUserToResizeRows = false;
            this.InputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.InputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGrid.Location = new System.Drawing.Point(3, 41);
            this.InputGrid.Margin = new System.Windows.Forms.Padding(2);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.RowTemplate.Height = 24;
            this.InputGrid.ShowEditingIcon = false;
            this.InputGrid.Size = new System.Drawing.Size(360, 401);
            this.InputGrid.TabIndex = 1;
            this.InputGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.InputGrid_CellValidating);
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputGrid_CellValueChanged);
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Location = new System.Drawing.Point(288, 465);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 23);
            this.SubmitBtn.TabIndex = 3;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // inputWarning
            // 
            this.inputWarning.AutoSize = true;
            this.inputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.inputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.inputWarning.Location = new System.Drawing.Point(3, 582);
            this.inputWarning.Name = "inputWarning";
            this.inputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.inputWarning.Size = new System.Drawing.Size(384, 23);
            this.inputWarning.TabIndex = 64;
            this.inputWarning.Text = "Test warning notification area with long warning message";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.resultTipLabel);
            this.groupBox1.Controls.Add(this.OutputPlot);
            this.groupBox1.Location = new System.Drawing.Point(384, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(763, 576);
            this.groupBox1.TabIndex = 67;
            this.groupBox1.TabStop = false;
            // 
            // resultTipLabel
            // 
            this.resultTipLabel.AutoSize = true;
            this.resultTipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultTipLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.resultTipLabel.Location = new System.Drawing.Point(275, 266);
            this.resultTipLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.resultTipLabel.Name = "resultTipLabel";
            this.resultTipLabel.Size = new System.Drawing.Size(222, 20);
            this.resultTipLabel.TabIndex = 67;
            this.resultTipLabel.Text = "Submit analysis to view results";
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(224, 447);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(139, 17);
            this.AutoSetLimits.TabIndex = 2;
            this.AutoSetLimits.Text = "Automatic plot axis limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.Visible = false;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // OutputPlot
            // 
            this.OutputPlot.Location = new System.Drawing.Point(6, 38);
            this.OutputPlot.Name = "OutputPlot";
            this.OutputPlot.Size = new System.Drawing.Size(757, 532);
            this.OutputPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.OutputPlot.TabIndex = 68;
            this.OutputPlot.TabStop = false;
            // 
            // PoolingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.AutoSetLimits);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.InputGrid);
            this.Controls.Add(this.SubmitBtn);
            this.Controls.Add(this.spinner);
            this.Controls.Add(this.inputWarning);
            this.Name = "PoolingForm";
            this.Size = new System.Drawing.Size(1150, 613);
            this.Load += new System.EventHandler(this.PoolingForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.spinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPlot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox spinner;
        private System.Windows.Forms.DataGridView InputGrid;
        private System.Windows.Forms.Button SubmitBtn;
        private System.Windows.Forms.Label inputWarning;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox AutoSetLimits;
        private System.Windows.Forms.Label resultTipLabel;
        private System.Windows.Forms.PictureBox OutputPlot;
    }
}
