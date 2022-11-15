
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
            this.tcIO = new System.Windows.Forms.TabControl();
            this.inputTab = new System.Windows.Forms.TabPage();
            this.massFlowLabel = new System.Windows.Forms.Label();
            this.MassFlowInput = new System.Windows.Forms.TextBox();
            this.AutoSetLimits = new System.Windows.Forms.CheckBox();
            this.lblContourLevels = new System.Windows.Forms.Label();
            this.ContourInput = new System.Windows.Forms.TextBox();
            this.PlotTitleInput = new System.Windows.Forms.TextBox();
            this.lblPlotTitle = new System.Windows.Forms.Label();
            this.inputWarning = new System.Windows.Forms.Label();
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.NozzleSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.outputRadiantFrac = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.outputFlameLength = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.outputSrad = new System.Windows.Forms.TextBox();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.outputMassFlowRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.OutputPictureBox = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.outputWarning = new System.Windows.Forms.Label();
            this.tcIO.SuspendLayout();
            this.inputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            this.outputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OutputPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // tcIO
            // 
            this.tcIO.Controls.Add(this.inputTab);
            this.tcIO.Controls.Add(this.outputTab);
            this.tcIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcIO.Location = new System.Drawing.Point(0, 0);
            this.tcIO.Name = "tcIO";
            this.tcIO.SelectedIndex = 0;
            this.tcIO.Size = new System.Drawing.Size(992, 594);
            this.tcIO.TabIndex = 0;
            // 
            // inputTab
            // 
            this.inputTab.Controls.Add(this.massFlowLabel);
            this.inputTab.Controls.Add(this.MassFlowInput);
            this.inputTab.Controls.Add(this.AutoSetLimits);
            this.inputTab.Controls.Add(this.lblContourLevels);
            this.inputTab.Controls.Add(this.ContourInput);
            this.inputTab.Controls.Add(this.PlotTitleInput);
            this.inputTab.Controls.Add(this.lblPlotTitle);
            this.inputTab.Controls.Add(this.inputWarning);
            this.inputTab.Controls.Add(this.PhaseSelector);
            this.inputTab.Controls.Add(this.phaseLabel);
            this.inputTab.Controls.Add(this.NozzleSelector);
            this.inputTab.Controls.Add(this.label1);
            this.inputTab.Controls.Add(this.spinnerPictureBox);
            this.inputTab.Controls.Add(this.SubmitBtn);
            this.inputTab.Controls.Add(this.InputGrid);
            this.inputTab.Location = new System.Drawing.Point(4, 22);
            this.inputTab.Name = "inputTab";
            this.inputTab.Padding = new System.Windows.Forms.Padding(3);
            this.inputTab.Size = new System.Drawing.Size(984, 568);
            this.inputTab.TabIndex = 0;
            this.inputTab.Text = "Input";
            this.inputTab.UseVisualStyleBackColor = true;
            // 
            // massFlowLabel
            // 
            this.massFlowLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.massFlowLabel.AutoSize = true;
            this.massFlowLabel.Location = new System.Drawing.Point(9, 115);
            this.massFlowLabel.Name = "massFlowLabel";
            this.massFlowLabel.Size = new System.Drawing.Size(184, 13);
            this.massFlowLabel.TabIndex = 73;
            this.massFlowLabel.Text = "Fluid mass flow rate (unchoked, kg/s)";
            // 
            // MassFlowInput
            // 
            this.MassFlowInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MassFlowInput.Location = new System.Drawing.Point(205, 112);
            this.MassFlowInput.Name = "MassFlowInput";
            this.MassFlowInput.Size = new System.Drawing.Size(247, 20);
            this.MassFlowInput.TabIndex = 72;
            this.MassFlowInput.TextChanged += new System.EventHandler(this.MassFlowInput_TextChanged);
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(205, 138);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(171, 17);
            this.AutoSetLimits.TabIndex = 71;
            this.AutoSetLimits.Text = "Automatically set plot axis limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // lblContourLevels
            // 
            this.lblContourLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblContourLevels.AutoSize = true;
            this.lblContourLevels.Location = new System.Drawing.Point(9, 89);
            this.lblContourLevels.Name = "lblContourLevels";
            this.lblContourLevels.Size = new System.Drawing.Size(140, 13);
            this.lblContourLevels.TabIndex = 68;
            this.lblContourLevels.Text = "Contour Levels (K, optional):";
            // 
            // ContourInput
            // 
            this.ContourInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ContourInput.Location = new System.Drawing.Point(205, 86);
            this.ContourInput.Name = "ContourInput";
            this.ContourInput.Size = new System.Drawing.Size(247, 20);
            this.ContourInput.TabIndex = 67;
            this.ContourInput.TextChanged += new System.EventHandler(this.ContourInput_TextChanged);
            // 
            // PlotTitleInput
            // 
            this.PlotTitleInput.Location = new System.Drawing.Point(205, 6);
            this.PlotTitleInput.Name = "PlotTitleInput";
            this.PlotTitleInput.Size = new System.Drawing.Size(247, 20);
            this.PlotTitleInput.TabIndex = 61;
            this.PlotTitleInput.TextChanged += new System.EventHandler(this.PlotTitleInput_TextChanged);
            // 
            // lblPlotTitle
            // 
            this.lblPlotTitle.AutoSize = true;
            this.lblPlotTitle.Location = new System.Drawing.Point(9, 9);
            this.lblPlotTitle.Name = "lblPlotTitle";
            this.lblPlotTitle.Size = new System.Drawing.Size(94, 13);
            this.lblPlotTitle.TabIndex = 60;
            this.lblPlotTitle.Text = "Plot Title (optional)";
            // 
            // inputWarning
            // 
            this.inputWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.inputWarning.AutoSize = true;
            this.inputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.inputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.inputWarning.Location = new System.Drawing.Point(3, 543);
            this.inputWarning.Name = "inputWarning";
            this.inputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.inputWarning.Size = new System.Drawing.Size(384, 23);
            this.inputWarning.TabIndex = 59;
            this.inputWarning.Text = "Test warning notification area with long warning message";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
            // 
            // PhaseSelector
            // 
            this.PhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PhaseSelector.DropDownWidth = 170;
            this.PhaseSelector.FormattingEnabled = true;
            this.PhaseSelector.Location = new System.Drawing.Point(205, 59);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(247, 21);
            this.PhaseSelector.TabIndex = 58;
            this.PhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.PhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phaseLabel.Location = new System.Drawing.Point(9, 60);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(71, 15);
            this.phaseLabel.TabIndex = 57;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // NozzleSelector
            // 
            this.NozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NozzleSelector.DropDownWidth = 170;
            this.NozzleSelector.FormattingEnabled = true;
            this.NozzleSelector.Location = new System.Drawing.Point(205, 32);
            this.NozzleSelector.Name = "NozzleSelector";
            this.NozzleSelector.Size = new System.Drawing.Size(247, 21);
            this.NozzleSelector.TabIndex = 24;
            this.NozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.NozzleSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 15);
            this.label1.TabIndex = 23;
            this.label1.Text = "Notional nozzle model";
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(348, 518);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 19;
            this.spinnerPictureBox.TabStop = false;
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SubmitBtn.Location = new System.Drawing.Point(377, 518);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 23);
            this.SubmitBtn.TabIndex = 15;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // InputGrid
            // 
            this.InputGrid.AllowUserToAddRows = false;
            this.InputGrid.AllowUserToDeleteRows = false;
            this.InputGrid.AllowUserToResizeRows = false;
            this.InputGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.InputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGrid.Location = new System.Drawing.Point(7, 160);
            this.InputGrid.Margin = new System.Windows.Forms.Padding(2);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.Size = new System.Drawing.Size(445, 353);
            this.InputGrid.TabIndex = 3;
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputGrid_CellValueChanged);
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.splitContainer1);
            this.outputTab.Location = new System.Drawing.Point(4, 22);
            this.outputTab.Name = "outputTab";
            this.outputTab.Padding = new System.Windows.Forms.Padding(3);
            this.outputTab.Size = new System.Drawing.Size(984, 568);
            this.outputTab.TabIndex = 1;
            this.outputTab.Text = "Output";
            this.outputTab.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(3, 3);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.outputRadiantFrac);
            this.splitContainer1.Panel1.Controls.Add(this.label4);
            this.splitContainer1.Panel1.Controls.Add(this.outputFlameLength);
            this.splitContainer1.Panel1.Controls.Add(this.label3);
            this.splitContainer1.Panel1.Controls.Add(this.outputSrad);
            this.splitContainer1.Panel1.Controls.Add(this.lblSeconds);
            this.splitContainer1.Panel1.Controls.Add(this.outputMassFlowRate);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.OutputPictureBox);
            this.splitContainer1.Panel1MinSize = 35;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputWarning);
            this.splitContainer1.Size = new System.Drawing.Size(978, 562);
            this.splitContainer1.SplitterDistance = 533;
            this.splitContainer1.TabIndex = 3;
            // 
            // outputRadiantFrac
            // 
            this.outputRadiantFrac.Location = new System.Drawing.Point(452, 29);
            this.outputRadiantFrac.Name = "outputRadiantFrac";
            this.outputRadiantFrac.ReadOnly = true;
            this.outputRadiantFrac.Size = new System.Drawing.Size(114, 20);
            this.outputRadiantFrac.TabIndex = 20;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(335, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 19;
            this.label4.Text = "Radiant fraction";
            // 
            // outputFlameLength
            // 
            this.outputFlameLength.Location = new System.Drawing.Point(452, 3);
            this.outputFlameLength.Name = "outputFlameLength";
            this.outputFlameLength.ReadOnly = true;
            this.outputFlameLength.Size = new System.Drawing.Size(114, 20);
            this.outputFlameLength.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(335, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Visible flame length (m)";
            // 
            // outputSrad
            // 
            this.outputSrad.Location = new System.Drawing.Point(170, 29);
            this.outputSrad.Name = "outputSrad";
            this.outputSrad.ReadOnly = true;
            this.outputSrad.Size = new System.Drawing.Size(114, 20);
            this.outputSrad.TabIndex = 10;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(4, 32);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(163, 13);
            this.lblSeconds.TabIndex = 9;
            this.lblSeconds.Text = "Total emitted radiative power (W)";
            // 
            // outputMassFlowRate
            // 
            this.outputMassFlowRate.Location = new System.Drawing.Point(170, 3);
            this.outputMassFlowRate.Name = "outputMassFlowRate";
            this.outputMassFlowRate.ReadOnly = true;
            this.outputMassFlowRate.Size = new System.Drawing.Size(114, 20);
            this.outputMassFlowRate.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Mass flow rate (kg/s)";
            // 
            // OutputPictureBox
            // 
            this.OutputPictureBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.OutputPictureBox.Location = new System.Drawing.Point(0, 72);
            this.OutputPictureBox.Name = "OutputPictureBox";
            this.OutputPictureBox.Size = new System.Drawing.Size(978, 461);
            this.OutputPictureBox.TabIndex = 2;
            this.OutputPictureBox.TabStop = false;
            // 
            // outputWarning
            // 
            this.outputWarning.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.outputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWarning.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.outputWarning.Location = new System.Drawing.Point(0, 0);
            this.outputWarning.Name = "outputWarning";
            this.outputWarning.Size = new System.Drawing.Size(978, 25);
            this.outputWarning.TabIndex = 20;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
            // 
            // JetFlameTemperaturePlotForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcIO);
            this.Name = "JetFlameTemperaturePlotForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.PhysJetTempPlotForm_Load);
            this.tcIO.ResumeLayout(false);
            this.inputTab.ResumeLayout(false);
            this.inputTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            this.outputTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OutputPictureBox)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcIO;
		private System.Windows.Forms.TabPage inputTab;
		private System.Windows.Forms.TabPage outputTab;
		private System.Windows.Forms.DataGridView InputGrid;
		private System.Windows.Forms.Button SubmitBtn;
		private Hyram.PictureBoxWithSave OutputPictureBox;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
        private System.Windows.Forms.ComboBox NozzleSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label outputWarning;
        private System.Windows.Forms.ComboBox PhaseSelector;
        private System.Windows.Forms.Label phaseLabel;
        private System.Windows.Forms.Label inputWarning;
        private System.Windows.Forms.TextBox outputSrad;
        private System.Windows.Forms.Label lblSeconds;
        private System.Windows.Forms.TextBox outputMassFlowRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox outputFlameLength;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox PlotTitleInput;
        private System.Windows.Forms.Label lblPlotTitle;
        private System.Windows.Forms.Label lblContourLevels;
        private System.Windows.Forms.TextBox ContourInput;
        private System.Windows.Forms.CheckBox AutoSetLimits;
        private System.Windows.Forms.TextBox outputRadiantFrac;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label massFlowLabel;
        private System.Windows.Forms.TextBox MassFlowInput;
    }
}
