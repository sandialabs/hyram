
namespace SandiaNationalLaboratories.Hyram {
	partial class JetFlameTemperaturePlotForm {
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.resultTipLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.outputMassFlowRate = new System.Windows.Forms.TextBox();
            this.outputRadiantFrac = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.outputFlameLength = new System.Windows.Forms.TextBox();
            this.outputSrad = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.massFlowLabel = new System.Windows.Forms.Label();
            this.MassFlowInput = new System.Windows.Forms.TextBox();
            this.AutoSetLimits = new System.Windows.Forms.CheckBox();
            this.lblContourLevels = new System.Windows.Forms.Label();
            this.ContourInput = new System.Windows.Forms.TextBox();
            this.PlotTitleInput = new System.Windows.Forms.TextBox();
            this.lblPlotTitle = new System.Windows.Forms.Label();
            this.inputWarning = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.OutputPlot = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPlot)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Controls.Add(this.massFlowLabel);
            this.panel1.Controls.Add(this.MassFlowInput);
            this.panel1.Controls.Add(this.AutoSetLimits);
            this.panel1.Controls.Add(this.lblContourLevels);
            this.panel1.Controls.Add(this.ContourInput);
            this.panel1.Controls.Add(this.PlotTitleInput);
            this.panel1.Controls.Add(this.lblPlotTitle);
            this.panel1.Controls.Add(this.inputWarning);
            this.panel1.Controls.Add(this.spinnerPictureBox);
            this.panel1.Controls.Add(this.SubmitBtn);
            this.panel1.Controls.Add(this.InputGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1151, 594);
            this.panel1.TabIndex = 1;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.resultTipLabel);
            this.groupBox1.Controls.Add(this.OutputPlot);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.outputMassFlowRate);
            this.groupBox1.Controls.Add(this.outputRadiantFrac);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.lblSeconds);
            this.groupBox1.Controls.Add(this.outputFlameLength);
            this.groupBox1.Controls.Add(this.outputSrad);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(382, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(766, 557);
            this.groupBox1.TabIndex = 103;
            this.groupBox1.TabStop = false;
            // 
            // resultTipLabel
            // 
            this.resultTipLabel.AutoSize = true;
            this.resultTipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultTipLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.resultTipLabel.Location = new System.Drawing.Point(276, 275);
            this.resultTipLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.resultTipLabel.Name = "resultTipLabel";
            this.resultTipLabel.Size = new System.Drawing.Size(222, 20);
            this.resultTipLabel.TabIndex = 103;
            this.resultTipLabel.Text = "Submit analysis to view results";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(275, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 94;
            this.label2.Text = "Mass flow rate (kg/s)";
            // 
            // outputMassFlowRate
            // 
            this.outputMassFlowRate.Location = new System.Drawing.Point(384, 19);
            this.outputMassFlowRate.Name = "outputMassFlowRate";
            this.outputMassFlowRate.ReadOnly = true;
            this.outputMassFlowRate.Size = new System.Drawing.Size(114, 20);
            this.outputMassFlowRate.TabIndex = 95;
            // 
            // outputRadiantFrac
            // 
            this.outputRadiantFrac.Location = new System.Drawing.Point(646, 45);
            this.outputRadiantFrac.Name = "outputRadiantFrac";
            this.outputRadiantFrac.ReadOnly = true;
            this.outputRadiantFrac.Size = new System.Drawing.Size(114, 20);
            this.outputRadiantFrac.TabIndex = 101;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(558, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 100;
            this.label4.Text = "Radiant fraction";
            this.label4.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(218, 48);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(163, 13);
            this.lblSeconds.TabIndex = 96;
            this.lblSeconds.Text = "Total emitted radiative power (W)";
            // 
            // outputFlameLength
            // 
            this.outputFlameLength.Location = new System.Drawing.Point(646, 19);
            this.outputFlameLength.Name = "outputFlameLength";
            this.outputFlameLength.ReadOnly = true;
            this.outputFlameLength.Size = new System.Drawing.Size(114, 20);
            this.outputFlameLength.TabIndex = 99;
            // 
            // outputSrad
            // 
            this.outputSrad.Location = new System.Drawing.Point(384, 45);
            this.outputSrad.Name = "outputSrad";
            this.outputSrad.ReadOnly = true;
            this.outputSrad.Size = new System.Drawing.Size(114, 20);
            this.outputSrad.TabIndex = 97;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(529, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 98;
            this.label3.Text = "Visible flame length (m)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // massFlowLabel
            // 
            this.massFlowLabel.AutoSize = true;
            this.massFlowLabel.Location = new System.Drawing.Point(3, 63);
            this.massFlowLabel.Name = "massFlowLabel";
            this.massFlowLabel.Size = new System.Drawing.Size(184, 13);
            this.massFlowLabel.TabIndex = 93;
            this.massFlowLabel.Text = "Fluid mass flow rate (unchoked, kg/s)";
            // 
            // MassFlowInput
            // 
            this.MassFlowInput.Location = new System.Drawing.Point(193, 60);
            this.MassFlowInput.Name = "MassFlowInput";
            this.MassFlowInput.Size = new System.Drawing.Size(171, 20);
            this.MassFlowInput.TabIndex = 3;
            this.MassFlowInput.TextChanged += new System.EventHandler(this.MassFlowInput_TextChanged);
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(193, 86);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(171, 17);
            this.AutoSetLimits.TabIndex = 4;
            this.AutoSetLimits.Text = "Automatically set plot axis limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // lblContourLevels
            // 
            this.lblContourLevels.AutoSize = true;
            this.lblContourLevels.Location = new System.Drawing.Point(3, 37);
            this.lblContourLevels.Name = "lblContourLevels";
            this.lblContourLevels.Size = new System.Drawing.Size(136, 13);
            this.lblContourLevels.TabIndex = 90;
            this.lblContourLevels.Text = "Contour levels (K, optional):";
            // 
            // ContourInput
            // 
            this.ContourInput.Location = new System.Drawing.Point(193, 34);
            this.ContourInput.Name = "ContourInput";
            this.ContourInput.Size = new System.Drawing.Size(171, 20);
            this.ContourInput.TabIndex = 2;
            this.ContourInput.TextChanged += new System.EventHandler(this.ContourInput_TextChanged);
            // 
            // PlotTitleInput
            // 
            this.PlotTitleInput.Location = new System.Drawing.Point(193, 8);
            this.PlotTitleInput.Name = "PlotTitleInput";
            this.PlotTitleInput.Size = new System.Drawing.Size(171, 20);
            this.PlotTitleInput.TabIndex = 1;
            this.PlotTitleInput.TextChanged += new System.EventHandler(this.PlotTitleInput_TextChanged);
            // 
            // lblPlotTitle
            // 
            this.lblPlotTitle.AutoSize = true;
            this.lblPlotTitle.Location = new System.Drawing.Point(3, 11);
            this.lblPlotTitle.Name = "lblPlotTitle";
            this.lblPlotTitle.Size = new System.Drawing.Size(94, 13);
            this.lblPlotTitle.TabIndex = 87;
            this.lblPlotTitle.Text = "Plot Title (optional)";
            // 
            // inputWarning
            // 
            this.inputWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.inputWarning.AutoSize = true;
            this.inputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.inputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.inputWarning.Location = new System.Drawing.Point(3, 566);
            this.inputWarning.Name = "inputWarning";
            this.inputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.inputWarning.Size = new System.Drawing.Size(384, 23);
            this.inputWarning.TabIndex = 86;
            this.inputWarning.Text = "Test warning notification area with long warning message";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(260, 431);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 85;
            this.spinnerPictureBox.TabStop = false;
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Location = new System.Drawing.Point(289, 432);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 23);
            this.SubmitBtn.TabIndex = 6;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // InputGrid
            // 
            this.InputGrid.AllowUserToAddRows = false;
            this.InputGrid.AllowUserToDeleteRows = false;
            this.InputGrid.AllowUserToResizeRows = false;
            this.InputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.InputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGrid.Location = new System.Drawing.Point(6, 108);
            this.InputGrid.Margin = new System.Windows.Forms.Padding(2);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.Size = new System.Drawing.Size(358, 319);
            this.InputGrid.TabIndex = 5;
            this.InputGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.InputGrid_CellValidating);
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputGrid_CellValueChanged);
            // 
            // OutputPlot
            // 
            this.OutputPlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutputPlot.Location = new System.Drawing.Point(6, 71);
            this.OutputPlot.Name = "OutputPlot";
            this.OutputPlot.Size = new System.Drawing.Size(754, 480);
            this.OutputPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.OutputPlot.TabIndex = 104;
            this.OutputPlot.TabStop = false;
            // 
            // JetFlameTemperaturePlotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Name = "JetFlameTemperaturePlotForm";
            this.Size = new System.Drawing.Size(1151, 594);
            this.Load += new System.EventHandler(this.PhysJetTempPlotForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPlot)).EndInit();
            this.ResumeLayout(false);

		}

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox outputRadiantFrac;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox outputFlameLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox outputSrad;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.TextBox outputMassFlowRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label massFlowLabel;
        private System.Windows.Forms.TextBox MassFlowInput;
        private System.Windows.Forms.CheckBox AutoSetLimits;
        private System.Windows.Forms.Label lblContourLevels;
        private System.Windows.Forms.TextBox ContourInput;
        private System.Windows.Forms.TextBox PlotTitleInput;
        private System.Windows.Forms.Label lblPlotTitle;
        private System.Windows.Forms.Label inputWarning;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
        private System.Windows.Forms.Button SubmitBtn;
        private System.Windows.Forms.DataGridView InputGrid;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label resultTipLabel;
        private System.Windows.Forms.PictureBox OutputPlot;
    }
}
