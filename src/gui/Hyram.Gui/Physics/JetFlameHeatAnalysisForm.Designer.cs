
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram {
	partial class JetFlameHeatAnalysisForm {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private IContainer components = null;

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
            this.inputWarning = new System.Windows.Forms.Label();
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.NozzleSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.lblContourLevels = new System.Windows.Forms.Label();
            this.ContourInput = new System.Windows.Forms.TextBox();
            this.pbFlameGeometry = new System.Windows.Forms.PictureBox();
            this.lblZElementCount = new System.Windows.Forms.Label();
            this.lblYElemCount = new System.Windows.Forms.Label();
            this.lblXElemCount = new System.Windows.Forms.Label();
            this.LocZInput = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsZ = new System.Windows.Forms.Label();
            this.LocYInput = new System.Windows.Forms.TextBox();
            this.lblHeadFluxPointsY = new System.Windows.Forms.Label();
            this.LocXInput = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsX = new System.Windows.Forms.Label();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tcOutputs = new System.Windows.Forms.TabControl();
            this.outputTabData = new System.Windows.Forms.TabPage();
            this.outputRadiantFrac = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.outputFlameLength = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.outputSrad = new System.Windows.Forms.TextBox();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.outputMassFlowRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dgResult = new System.Windows.Forms.DataGridView();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFlux = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultLabel = new System.Windows.Forms.Label();
            this.CopyBtn = new System.Windows.Forms.Button();
            this.tpPlotIsoPlot = new System.Windows.Forms.TabPage();
            this.pbPlotIsoOutput = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.tpt_fname = new System.Windows.Forms.TabPage();
            this.pbTPlot = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.outputWarning = new System.Windows.Forms.Label();
            this.tcIO.SuspendLayout();
            this.inputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlameGeometry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            this.outputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tcOutputs.SuspendLayout();
            this.outputTabData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).BeginInit();
            this.tpPlotIsoPlot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlotIsoOutput)).BeginInit();
            this.tpt_fname.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTPlot)).BeginInit();
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
            this.inputTab.Controls.Add(this.inputWarning);
            this.inputTab.Controls.Add(this.PhaseSelector);
            this.inputTab.Controls.Add(this.phaseLabel);
            this.inputTab.Controls.Add(this.NozzleSelector);
            this.inputTab.Controls.Add(this.label1);
            this.inputTab.Controls.Add(this.spinnerPictureBox);
            this.inputTab.Controls.Add(this.lblContourLevels);
            this.inputTab.Controls.Add(this.ContourInput);
            this.inputTab.Controls.Add(this.pbFlameGeometry);
            this.inputTab.Controls.Add(this.lblZElementCount);
            this.inputTab.Controls.Add(this.lblYElemCount);
            this.inputTab.Controls.Add(this.lblXElemCount);
            this.inputTab.Controls.Add(this.LocZInput);
            this.inputTab.Controls.Add(this.lblHeatFluxPointsZ);
            this.inputTab.Controls.Add(this.LocYInput);
            this.inputTab.Controls.Add(this.lblHeadFluxPointsY);
            this.inputTab.Controls.Add(this.LocXInput);
            this.inputTab.Controls.Add(this.lblHeatFluxPointsX);
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
            this.massFlowLabel.Location = new System.Drawing.Point(9, 64);
            this.massFlowLabel.Name = "massFlowLabel";
            this.massFlowLabel.Size = new System.Drawing.Size(184, 13);
            this.massFlowLabel.TabIndex = 75;
            this.massFlowLabel.Text = "Fluid mass flow rate (unchoked, kg/s)";
            // 
            // MassFlowInput
            // 
            this.MassFlowInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MassFlowInput.Location = new System.Drawing.Point(203, 61);
            this.MassFlowInput.Name = "MassFlowInput";
            this.MassFlowInput.Size = new System.Drawing.Size(247, 20);
            this.MassFlowInput.TabIndex = 74;
            this.MassFlowInput.TextChanged += new System.EventHandler(this.MassFlowInput_TextChanged);
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(201, 87);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(171, 17);
            this.AutoSetLimits.TabIndex = 72;
            this.AutoSetLimits.Text = "Automatically set plot axis limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // inputWarning
            // 
            this.inputWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.inputWarning.AutoSize = true;
            this.inputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.inputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.inputWarning.Location = new System.Drawing.Point(3, 542);
            this.inputWarning.Name = "inputWarning";
            this.inputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.inputWarning.Size = new System.Drawing.Size(384, 23);
            this.inputWarning.TabIndex = 60;
            this.inputWarning.Text = "Test warning notification area with long warning message";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
            // 
            // PhaseSelector
            // 
            this.PhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PhaseSelector.FormattingEnabled = true;
            this.PhaseSelector.Location = new System.Drawing.Point(203, 34);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(247, 21);
            this.PhaseSelector.TabIndex = 58;
            this.PhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.PhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phaseLabel.Location = new System.Drawing.Point(9, 35);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(71, 15);
            this.phaseLabel.TabIndex = 57;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // NozzleSelector
            // 
            this.NozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NozzleSelector.FormattingEnabled = true;
            this.NozzleSelector.Location = new System.Drawing.Point(203, 7);
            this.NozzleSelector.Name = "NozzleSelector";
            this.NozzleSelector.Size = new System.Drawing.Size(247, 21);
            this.NozzleSelector.TabIndex = 22;
            this.NozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.NozzleSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 18);
            this.label1.TabIndex = 21;
            this.label1.Text = "Notional nozzle model";
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(348, 518);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 20;
            this.spinnerPictureBox.TabStop = false;
            // 
            // lblContourLevels
            // 
            this.lblContourLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblContourLevels.AutoSize = true;
            this.lblContourLevels.Location = new System.Drawing.Point(9, 523);
            this.lblContourLevels.Name = "lblContourLevels";
            this.lblContourLevels.Size = new System.Drawing.Size(132, 13);
            this.lblContourLevels.TabIndex = 18;
            this.lblContourLevels.Text = "Contour Levels (kW/m^2):";
            // 
            // ContourInput
            // 
            this.ContourInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ContourInput.Location = new System.Drawing.Point(175, 520);
            this.ContourInput.Name = "ContourInput";
            this.ContourInput.Size = new System.Drawing.Size(170, 20);
            this.ContourInput.TabIndex = 17;
            this.ContourInput.TextChanged += new System.EventHandler(this.ContourInput_TextChanged);
            // 
            // pbFlameGeometry
            // 
            this.pbFlameGeometry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFlameGeometry.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.geometry_of_flame;
            this.pbFlameGeometry.Location = new System.Drawing.Point(456, 31);
            this.pbFlameGeometry.Name = "pbFlameGeometry";
            this.pbFlameGeometry.Size = new System.Drawing.Size(522, 407);
            this.pbFlameGeometry.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFlameGeometry.TabIndex = 1;
            this.pbFlameGeometry.TabStop = false;
            // 
            // lblZElementCount
            // 
            this.lblZElementCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblZElementCount.AutoSize = true;
            this.lblZElementCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZElementCount.Location = new System.Drawing.Point(453, 499);
            this.lblZElementCount.Name = "lblZElementCount";
            this.lblZElementCount.Size = new System.Drawing.Size(76, 13);
            this.lblZElementCount.TabIndex = 13;
            this.lblZElementCount.Text = "Element Count";
            // 
            // lblYElemCount
            // 
            this.lblYElemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblYElemCount.AutoSize = true;
            this.lblYElemCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblYElemCount.Location = new System.Drawing.Point(453, 473);
            this.lblYElemCount.Name = "lblYElemCount";
            this.lblYElemCount.Size = new System.Drawing.Size(76, 13);
            this.lblYElemCount.TabIndex = 12;
            this.lblYElemCount.Text = "Element Count";
            // 
            // lblXElemCount
            // 
            this.lblXElemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblXElemCount.AutoSize = true;
            this.lblXElemCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblXElemCount.Location = new System.Drawing.Point(453, 447);
            this.lblXElemCount.Name = "lblXElemCount";
            this.lblXElemCount.Size = new System.Drawing.Size(103, 13);
            this.lblXElemCount.TabIndex = 11;
            this.lblXElemCount.Text = "Array Element Count";
            // 
            // LocZInput
            // 
            this.LocZInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LocZInput.Location = new System.Drawing.Point(175, 496);
            this.LocZInput.Name = "LocZInput";
            this.LocZInput.Size = new System.Drawing.Size(273, 20);
            this.LocZInput.TabIndex = 8;
            this.LocZInput.TextChanged += new System.EventHandler(this.LocZInput_TextChanged);
            // 
            // lblHeatFluxPointsZ
            // 
            this.lblHeatFluxPointsZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsZ.AutoSize = true;
            this.lblHeatFluxPointsZ.Location = new System.Drawing.Point(9, 499);
            this.lblHeatFluxPointsZ.Name = "lblHeatFluxPointsZ";
            this.lblHeatFluxPointsZ.Size = new System.Drawing.Size(162, 13);
            this.lblHeatFluxPointsZ.TabIndex = 7;
            this.lblHeatFluxPointsZ.Text = "Z Radiative Heat Flux Points (m):";
            // 
            // LocYInput
            // 
            this.LocYInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LocYInput.Location = new System.Drawing.Point(175, 470);
            this.LocYInput.Name = "LocYInput";
            this.LocYInput.Size = new System.Drawing.Size(273, 20);
            this.LocYInput.TabIndex = 6;
            this.LocYInput.TextChanged += new System.EventHandler(this.LocYInput_TextChanged);
            // 
            // lblHeadFluxPointsY
            // 
            this.lblHeadFluxPointsY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeadFluxPointsY.AutoSize = true;
            this.lblHeadFluxPointsY.Location = new System.Drawing.Point(9, 473);
            this.lblHeadFluxPointsY.Name = "lblHeadFluxPointsY";
            this.lblHeadFluxPointsY.Size = new System.Drawing.Size(162, 13);
            this.lblHeadFluxPointsY.TabIndex = 5;
            this.lblHeadFluxPointsY.Text = "Y Radiative Heat Flux Points (m):";
            // 
            // LocXInput
            // 
            this.LocXInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LocXInput.Location = new System.Drawing.Point(175, 444);
            this.LocXInput.Name = "LocXInput";
            this.LocXInput.Size = new System.Drawing.Size(273, 20);
            this.LocXInput.TabIndex = 4;
            this.LocXInput.TextChanged += new System.EventHandler(this.LocXInput_TextChanged);
            // 
            // lblHeatFluxPointsX
            // 
            this.lblHeatFluxPointsX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsX.AutoSize = true;
            this.lblHeatFluxPointsX.Location = new System.Drawing.Point(9, 447);
            this.lblHeatFluxPointsX.Name = "lblHeatFluxPointsX";
            this.lblHeatFluxPointsX.Size = new System.Drawing.Size(162, 13);
            this.lblHeatFluxPointsX.TabIndex = 3;
            this.lblHeatFluxPointsX.Text = "X Radiative Heat Flux Points (m):";
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SubmitBtn.Location = new System.Drawing.Point(377, 518);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(71, 23);
            this.SubmitBtn.TabIndex = 2;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // InputGrid
            // 
            this.InputGrid.AllowUserToAddRows = false;
            this.InputGrid.AllowUserToDeleteRows = false;
            this.InputGrid.AllowUserToResizeRows = false;
            this.InputGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.InputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.InputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGrid.Location = new System.Drawing.Point(3, 110);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.Size = new System.Drawing.Size(445, 328);
            this.InputGrid.TabIndex = 0;
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
            this.splitContainer1.Panel1.Controls.Add(this.tcOutputs);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputWarning);
            this.splitContainer1.Size = new System.Drawing.Size(978, 562);
            this.splitContainer1.SplitterDistance = 524;
            this.splitContainer1.TabIndex = 6;
            // 
            // tcOutputs
            // 
            this.tcOutputs.Controls.Add(this.outputTabData);
            this.tcOutputs.Controls.Add(this.tpPlotIsoPlot);
            this.tcOutputs.Controls.Add(this.tpt_fname);
            this.tcOutputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcOutputs.Location = new System.Drawing.Point(0, 0);
            this.tcOutputs.Name = "tcOutputs";
            this.tcOutputs.SelectedIndex = 0;
            this.tcOutputs.Size = new System.Drawing.Size(978, 524);
            this.tcOutputs.TabIndex = 5;
            // 
            // outputTabData
            // 
            this.outputTabData.Controls.Add(this.outputRadiantFrac);
            this.outputTabData.Controls.Add(this.label4);
            this.outputTabData.Controls.Add(this.outputFlameLength);
            this.outputTabData.Controls.Add(this.label3);
            this.outputTabData.Controls.Add(this.outputSrad);
            this.outputTabData.Controls.Add(this.lblSeconds);
            this.outputTabData.Controls.Add(this.outputMassFlowRate);
            this.outputTabData.Controls.Add(this.label2);
            this.outputTabData.Controls.Add(this.dgResult);
            this.outputTabData.Controls.Add(this.resultLabel);
            this.outputTabData.Controls.Add(this.CopyBtn);
            this.outputTabData.Location = new System.Drawing.Point(4, 22);
            this.outputTabData.Name = "outputTabData";
            this.outputTabData.Padding = new System.Windows.Forms.Padding(3);
            this.outputTabData.Size = new System.Drawing.Size(970, 498);
            this.outputTabData.TabIndex = 0;
            this.outputTabData.Text = "Values";
            this.outputTabData.UseVisualStyleBackColor = true;
            // 
            // outputRadiantFrac
            // 
            this.outputRadiantFrac.Location = new System.Drawing.Point(443, 32);
            this.outputRadiantFrac.Name = "outputRadiantFrac";
            this.outputRadiantFrac.ReadOnly = true;
            this.outputRadiantFrac.Size = new System.Drawing.Size(114, 20);
            this.outputRadiantFrac.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(323, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Radiant fraction";
            // 
            // outputFlameLength
            // 
            this.outputFlameLength.Location = new System.Drawing.Point(443, 6);
            this.outputFlameLength.Name = "outputFlameLength";
            this.outputFlameLength.ReadOnly = true;
            this.outputFlameLength.Size = new System.Drawing.Size(114, 20);
            this.outputFlameLength.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(323, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Visible flame length (m)";
            // 
            // outputSrad
            // 
            this.outputSrad.Location = new System.Drawing.Point(170, 32);
            this.outputSrad.Name = "outputSrad";
            this.outputSrad.ReadOnly = true;
            this.outputSrad.Size = new System.Drawing.Size(114, 20);
            this.outputSrad.TabIndex = 14;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(6, 35);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(163, 13);
            this.lblSeconds.TabIndex = 13;
            this.lblSeconds.Text = "Total emitted radiative power (W)";
            // 
            // outputMassFlowRate
            // 
            this.outputMassFlowRate.Location = new System.Drawing.Point(170, 6);
            this.outputMassFlowRate.Name = "outputMassFlowRate";
            this.outputMassFlowRate.ReadOnly = true;
            this.outputMassFlowRate.Size = new System.Drawing.Size(114, 20);
            this.outputMassFlowRate.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Mass flow rate (kg/s)";
            // 
            // dgResult
            // 
            this.dgResult.AllowUserToAddRows = false;
            this.dgResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colX,
            this.colY,
            this.colZ,
            this.colFlux});
            this.dgResult.Location = new System.Drawing.Point(3, 82);
            this.dgResult.Name = "dgResult";
            this.dgResult.Size = new System.Drawing.Size(962, 380);
            this.dgResult.TabIndex = 3;
            // 
            // colX
            // 
            this.colX.HeaderText = "X (m)";
            this.colX.Name = "colX";
            this.colX.ReadOnly = true;
            // 
            // colY
            // 
            this.colY.HeaderText = "Y (m)";
            this.colY.Name = "colY";
            this.colY.ReadOnly = true;
            // 
            // colZ
            // 
            this.colZ.HeaderText = "Z (m)";
            this.colZ.Name = "colZ";
            this.colZ.ReadOnly = true;
            // 
            // colFlux
            // 
            this.colFlux.HeaderText = "Flux (kW/m^2)";
            this.colFlux.Name = "colFlux";
            this.colFlux.ReadOnly = true;
            this.colFlux.Width = 110;
            // 
            // resultLabel
            // 
            this.resultLabel.AutoSize = true;
            this.resultLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultLabel.Location = new System.Drawing.Point(6, 66);
            this.resultLabel.Name = "resultLabel";
            this.resultLabel.Size = new System.Drawing.Size(306, 13);
            this.resultLabel.TabIndex = 0;
            this.resultLabel.Text = "Radiative heat flux calculated at specified locations:";
            // 
            // CopyBtn
            // 
            this.CopyBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CopyBtn.Location = new System.Drawing.Point(845, 468);
            this.CopyBtn.Name = "CopyBtn";
            this.CopyBtn.Size = new System.Drawing.Size(119, 23);
            this.CopyBtn.TabIndex = 2;
            this.CopyBtn.Text = "Copy to Clipboard";
            this.CopyBtn.UseVisualStyleBackColor = true;
            this.CopyBtn.Click += new System.EventHandler(this.CopyBtn_Click);
            // 
            // tpPlotIsoPlot
            // 
            this.tpPlotIsoPlot.Controls.Add(this.pbPlotIsoOutput);
            this.tpPlotIsoPlot.Location = new System.Drawing.Point(4, 22);
            this.tpPlotIsoPlot.Name = "tpPlotIsoPlot";
            this.tpPlotIsoPlot.Padding = new System.Windows.Forms.Padding(3);
            this.tpPlotIsoPlot.Size = new System.Drawing.Size(970, 498);
            this.tpPlotIsoPlot.TabIndex = 1;
            this.tpPlotIsoPlot.Text = "Heat Flux Plot";
            this.tpPlotIsoPlot.UseVisualStyleBackColor = true;
            // 
            // pbPlotIsoOutput
            // 
            this.pbPlotIsoOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPlotIsoOutput.Location = new System.Drawing.Point(3, 3);
            this.pbPlotIsoOutput.Name = "pbPlotIsoOutput";
            this.pbPlotIsoOutput.Size = new System.Drawing.Size(964, 492);
            this.pbPlotIsoOutput.TabIndex = 4;
            this.pbPlotIsoOutput.TabStop = false;
            // 
            // tpt_fname
            // 
            this.tpt_fname.Controls.Add(this.pbTPlot);
            this.tpt_fname.Location = new System.Drawing.Point(4, 22);
            this.tpt_fname.Name = "tpt_fname";
            this.tpt_fname.Size = new System.Drawing.Size(970, 498);
            this.tpt_fname.TabIndex = 2;
            this.tpt_fname.Text = "Temperature Plot";
            this.tpt_fname.UseVisualStyleBackColor = true;
            // 
            // pbTPlot
            // 
            this.pbTPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbTPlot.Location = new System.Drawing.Point(0, 0);
            this.pbTPlot.Name = "pbTPlot";
            this.pbTPlot.Size = new System.Drawing.Size(970, 498);
            this.pbTPlot.TabIndex = 0;
            this.pbTPlot.TabStop = false;
            // 
            // outputWarning
            // 
            this.outputWarning.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.outputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWarning.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.outputWarning.Location = new System.Drawing.Point(0, 0);
            this.outputWarning.Name = "outputWarning";
            this.outputWarning.Size = new System.Drawing.Size(978, 34);
            this.outputWarning.TabIndex = 19;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
            // 
            // JetFlameHeatAnalysisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcIO);
            this.Name = "JetFlameHeatAnalysisForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.JetFlameHeatAnalysisForm_Load);
            this.tcIO.ResumeLayout(false);
            this.inputTab.ResumeLayout(false);
            this.inputTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlameGeometry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            this.outputTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tcOutputs.ResumeLayout(false);
            this.outputTabData.ResumeLayout(false);
            this.outputTabData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
            this.tpPlotIsoPlot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPlotIsoOutput)).EndInit();
            this.tpt_fname.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTPlot)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private TabControl tcIO;
		private TabPage inputTab;
		private TabPage outputTab;
		private DataGridView InputGrid;

		private Button SubmitBtn;
		private Label resultLabel;
		private Label lblHeatFluxPointsX;
		private TextBox LocZInput;
		private Label lblHeatFluxPointsZ;
		private TextBox LocYInput;
		private Label lblHeadFluxPointsY;
		private TextBox LocXInput;
		private Button CopyBtn;
        private DataGridView dgResult;
		private Label lblZElementCount;
		private Label lblYElemCount;
		private Label lblXElemCount;
        private DataGridViewTextBoxColumn colX;
        private DataGridViewTextBoxColumn colY;
        private DataGridViewTextBoxColumn colZ;
        private DataGridViewTextBoxColumn colFlux;
		private PictureBox pbFlameGeometry;
        private TextBox ContourInput;
        private Label lblContourLevels;
		private PictureBoxWithSave pbPlotIsoOutput;
		private TabControl tcOutputs;
		private TabPage outputTabData;
		private TabPage tpPlotIsoPlot;
		private TabPage tpt_fname;
		private PictureBoxWithSave pbTPlot;
        private PictureBox spinnerPictureBox;
        private ComboBox NozzleSelector;
        private Label label1;
        private SplitContainer splitContainer1;
        private Label outputWarning;
        private ComboBox PhaseSelector;
        private Label phaseLabel;
        private Label inputWarning;

        private TextBox outputSrad;
        private Label lblSeconds;
        private TextBox outputMassFlowRate;
        private Label label2;
        private TextBox outputFlameLength;
        private Label label3;
        private CheckBox AutoSetLimits;
        private TextBox outputRadiantFrac;
        private Label label4;
        private Label massFlowLabel;
        private TextBox MassFlowInput;
    }
}
