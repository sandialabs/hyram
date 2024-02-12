
namespace SandiaNationalLaboratories.Hyram {
	partial class PlumeForm {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.outputMassFlowRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.massFlowLabel = new System.Windows.Forms.Label();
            this.MassFlowInput = new System.Windows.Forms.TextBox();
            this.inputWarning = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.PlotTitleInput = new System.Windows.Forms.TextBox();
            this.lblPlotTitle = new System.Windows.Forms.Label();
            this.AutoSetLimits = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.resultTipLabel = new System.Windows.Forms.Label();
            this.OutputPlot = new System.Windows.Forms.PictureBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.contourGridLabel = new System.Windows.Forms.Label();
            this.contourGrid = new System.Windows.Forms.DataGridView();
            this.contour = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.streamline = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hDist1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hDist2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vDist1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vDist2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ContourLabel = new System.Windows.Forms.Label();
            this.ContourInput = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPlot)).BeginInit();
            this.tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contourGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // outputMassFlowRate
            // 
            this.outputMassFlowRate.Location = new System.Drawing.Point(119, 12);
            this.outputMassFlowRate.Name = "outputMassFlowRate";
            this.outputMassFlowRate.ReadOnly = true;
            this.outputMassFlowRate.Size = new System.Drawing.Size(129, 20);
            this.outputMassFlowRate.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Mass flow rate (kg/s)";
            // 
            // massFlowLabel
            // 
            this.massFlowLabel.AutoSize = true;
            this.massFlowLabel.Location = new System.Drawing.Point(5, 62);
            this.massFlowLabel.Name = "massFlowLabel";
            this.massFlowLabel.Size = new System.Drawing.Size(184, 13);
            this.massFlowLabel.TabIndex = 58;
            this.massFlowLabel.Text = "Fluid mass flow rate (unchoked, kg/s)";
            // 
            // MassFlowInput
            // 
            this.MassFlowInput.Location = new System.Drawing.Point(195, 58);
            this.MassFlowInput.Name = "MassFlowInput";
            this.MassFlowInput.Size = new System.Drawing.Size(169, 20);
            this.MassFlowInput.TabIndex = 3;
            this.MassFlowInput.TextChanged += new System.EventHandler(this.MassFlowInput_TextChanged);
            // 
            // inputWarning
            // 
            this.inputWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.inputWarning.AutoSize = true;
            this.inputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.inputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.inputWarning.Location = new System.Drawing.Point(14, 560);
            this.inputWarning.Name = "inputWarning";
            this.inputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.inputWarning.Size = new System.Drawing.Size(384, 23);
            this.inputWarning.TabIndex = 22;
            this.inputWarning.Text = "Test warning notification area with long warning message";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(260, 453);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 21;
            this.spinnerPictureBox.TabStop = false;
            // 
            // InputGrid
            // 
            this.InputGrid.AllowUserToAddRows = false;
            this.InputGrid.AllowUserToDeleteRows = false;
            this.InputGrid.AllowUserToResizeRows = false;
            this.InputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.InputGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.InputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.InputGrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.InputGrid.Location = new System.Drawing.Point(5, 105);
            this.InputGrid.Margin = new System.Windows.Forms.Padding(2);
            this.InputGrid.Name = "InputGrid";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.InputGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.RowTemplate.Height = 24;
            this.InputGrid.ShowEditingIcon = false;
            this.InputGrid.Size = new System.Drawing.Size(358, 343);
            this.InputGrid.TabIndex = 5;
            this.InputGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.InputGrid_CellValidating);
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputGrid_CellValueChanged);
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Location = new System.Drawing.Point(288, 453);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 23);
            this.SubmitBtn.TabIndex = 6;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // PlotTitleInput
            // 
            this.PlotTitleInput.Location = new System.Drawing.Point(196, 10);
            this.PlotTitleInput.Name = "PlotTitleInput";
            this.PlotTitleInput.Size = new System.Drawing.Size(168, 20);
            this.PlotTitleInput.TabIndex = 1;
            this.PlotTitleInput.Text = "Mole Fraction of Leak";
            this.PlotTitleInput.TextChanged += new System.EventHandler(this.PlotTitleInput_TextChanged);
            // 
            // lblPlotTitle
            // 
            this.lblPlotTitle.AutoSize = true;
            this.lblPlotTitle.Location = new System.Drawing.Point(6, 13);
            this.lblPlotTitle.Name = "lblPlotTitle";
            this.lblPlotTitle.Size = new System.Drawing.Size(48, 13);
            this.lblPlotTitle.TabIndex = 0;
            this.lblPlotTitle.Text = "Plot Title";
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(195, 83);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(150, 17);
            this.AutoSetLimits.TabIndex = 4;
            this.AutoSetLimits.Text = "Automatically set plot limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(380, 2);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(767, 543);
            this.tabControl1.TabIndex = 93;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.resultTipLabel);
            this.tabPage1.Controls.Add(this.OutputPlot);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage1.Size = new System.Drawing.Size(759, 517);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Plot";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // resultTipLabel
            // 
            this.resultTipLabel.AutoSize = true;
            this.resultTipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultTipLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.resultTipLabel.Location = new System.Drawing.Point(260, 208);
            this.resultTipLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.resultTipLabel.Name = "resultTipLabel";
            this.resultTipLabel.Size = new System.Drawing.Size(222, 20);
            this.resultTipLabel.TabIndex = 1;
            this.resultTipLabel.Text = "Submit analysis to view results";
            // 
            // OutputPlot
            // 
            this.OutputPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputPlot.Location = new System.Drawing.Point(2, 2);
            this.OutputPlot.Name = "OutputPlot";
            this.OutputPlot.Size = new System.Drawing.Size(755, 513);
            this.OutputPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.OutputPlot.TabIndex = 2;
            this.OutputPlot.TabStop = false;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.contourGridLabel);
            this.tabPage2.Controls.Add(this.contourGrid);
            this.tabPage2.Controls.Add(this.outputMassFlowRate);
            this.tabPage2.Controls.Add(this.label2);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(2);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(2);
            this.tabPage2.Size = new System.Drawing.Size(759, 517);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Data";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // contourGridLabel
            // 
            this.contourGridLabel.AutoSize = true;
            this.contourGridLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.2F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contourGridLabel.Location = new System.Drawing.Point(9, 54);
            this.contourGridLabel.Name = "contourGridLabel";
            this.contourGridLabel.Size = new System.Drawing.Size(127, 17);
            this.contourGridLabel.TabIndex = 16;
            this.contourGridLabel.Text = "Distance to hazard";
            // 
            // contourGrid
            // 
            this.contourGrid.AllowUserToAddRows = false;
            this.contourGrid.AllowUserToDeleteRows = false;
            this.contourGrid.AllowUserToOrderColumns = true;
            this.contourGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.contourGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.contourGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.contourGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.contour,
            this.streamline,
            this.hDist1,
            this.hDist2,
            this.vDist1,
            this.vDist2});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.Format = "F2";
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.contourGrid.DefaultCellStyle = dataGridViewCellStyle5;
            this.contourGrid.Location = new System.Drawing.Point(10, 72);
            this.contourGrid.Margin = new System.Windows.Forms.Padding(2);
            this.contourGrid.Name = "contourGrid";
            this.contourGrid.ReadOnly = true;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.contourGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.contourGrid.RowHeadersVisible = false;
            this.contourGrid.RowTemplate.Height = 24;
            this.contourGrid.Size = new System.Drawing.Size(733, 250);
            this.contourGrid.TabIndex = 15;
            // 
            // contour
            // 
            this.contour.HeaderText = "Contour";
            this.contour.Name = "contour";
            this.contour.ReadOnly = true;
            // 
            // streamline
            // 
            this.streamline.HeaderText = "Streamline Distance (m)";
            this.streamline.Name = "streamline";
            this.streamline.ReadOnly = true;
            // 
            // hDist1
            // 
            this.hDist1.HeaderText = "Min Horizontal Distance (m)";
            this.hDist1.Name = "hDist1";
            this.hDist1.ReadOnly = true;
            // 
            // hDist2
            // 
            this.hDist2.HeaderText = "Max Horizontal Distance (m)";
            this.hDist2.Name = "hDist2";
            this.hDist2.ReadOnly = true;
            // 
            // vDist1
            // 
            this.vDist1.HeaderText = "Min Vertical Distance (m)";
            this.vDist1.Name = "vDist1";
            this.vDist1.ReadOnly = true;
            // 
            // vDist2
            // 
            this.vDist2.HeaderText = "Max Vertical Distance (m)";
            this.vDist2.Name = "vDist2";
            this.vDist2.ReadOnly = true;
            // 
            // ContourLabel
            // 
            this.ContourLabel.AutoSize = true;
            this.ContourLabel.Location = new System.Drawing.Point(6, 37);
            this.ContourLabel.Name = "ContourLabel";
            this.ContourLabel.Size = new System.Drawing.Size(164, 13);
            this.ContourLabel.TabIndex = 95;
            this.ContourLabel.Text = "Contours (mole fraction, optional):";
            // 
            // ContourInput
            // 
            this.ContourInput.Location = new System.Drawing.Point(196, 34);
            this.ContourInput.Name = "ContourInput";
            this.ContourInput.Size = new System.Drawing.Size(168, 20);
            this.ContourInput.TabIndex = 2;
            this.ContourInput.TextChanged += new System.EventHandler(this.ContourInput_TextChanged);
            // 
            // PlumeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.ContourLabel);
            this.Controls.Add(this.ContourInput);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.AutoSetLimits);
            this.Controls.Add(this.massFlowLabel);
            this.Controls.Add(this.lblPlotTitle);
            this.Controls.Add(this.PlotTitleInput);
            this.Controls.Add(this.InputGrid);
            this.Controls.Add(this.inputWarning);
            this.Controls.Add(this.spinnerPictureBox);
            this.Controls.Add(this.MassFlowInput);
            this.Controls.Add(this.SubmitBtn);
            this.Name = "PlumeForm";
            this.Size = new System.Drawing.Size(1150, 594);
            this.Load += new System.EventHandler(this.PlumeForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPlot)).EndInit();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.contourGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.Label lblPlotTitle;
		private System.Windows.Forms.TextBox PlotTitleInput;
		private System.Windows.Forms.Button SubmitBtn;
        private System.Windows.Forms.DataGridView InputGrid;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
        private System.Windows.Forms.Label inputWarning;
        private System.Windows.Forms.Label massFlowLabel;
        private System.Windows.Forms.TextBox MassFlowInput;
        private System.Windows.Forms.TextBox outputMassFlowRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox AutoSetLimits;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Label resultTipLabel;
        private System.Windows.Forms.Label contourGridLabel;
        private System.Windows.Forms.DataGridView contourGrid;
        private System.Windows.Forms.Label ContourLabel;
        private System.Windows.Forms.TextBox ContourInput;
        private System.Windows.Forms.DataGridViewTextBoxColumn contour;
        private System.Windows.Forms.DataGridViewTextBoxColumn streamline;
        private System.Windows.Forms.DataGridViewTextBoxColumn hDist1;
        private System.Windows.Forms.DataGridViewTextBoxColumn hDist2;
        private System.Windows.Forms.DataGridViewTextBoxColumn vDist1;
        private System.Windows.Forms.DataGridViewTextBoxColumn vDist2;
        private System.Windows.Forms.PictureBox OutputPlot;
    }
}
