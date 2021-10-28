
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
            this.inputWarning = new System.Windows.Forms.Label();
            this.fuelPhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.notionalNozzleSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.lblContourLevels = new System.Windows.Forms.Label();
            this.tbContourLevels = new System.Windows.Forms.TextBox();
            this.pbFlameGeometry = new System.Windows.Forms.PictureBox();
            this.lblZElementCount = new System.Windows.Forms.Label();
            this.lblYElemCount = new System.Windows.Forms.Label();
            this.lblXElemCount = new System.Windows.Forms.Label();
            this.tbRadiativeHeatFluxPointsZ = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsZ = new System.Windows.Forms.Label();
            this.tbRadiativeHeatFluxPointsY = new System.Windows.Forms.TextBox();
            this.lblHeadFluxPointsY = new System.Windows.Forms.Label();
            this.tbRadiativeHeatFluxPointsX = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsX = new System.Windows.Forms.Label();
            this.executeButton = new System.Windows.Forms.Button();
            this.dgInput = new System.Windows.Forms.DataGridView();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tcOutputs = new System.Windows.Forms.TabControl();
            this.outputTabData = new System.Windows.Forms.TabPage();
            this.dgResult = new System.Windows.Forms.DataGridView();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFlux = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.resultLabel = new System.Windows.Forms.Label();
            this.btnCopyToClipboard = new System.Windows.Forms.Button();
            this.tpPlotIsoPlot = new System.Windows.Forms.TabPage();
            this.tpt_fname = new System.Windows.Forms.TabPage();
            this.outputWarning = new System.Windows.Forms.Label();
            this.pbPlotIsoOutput = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.pbTPlot = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.outputSrad = new System.Windows.Forms.TextBox();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.outputMassFlowRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tcIO.SuspendLayout();
            this.inputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlameGeometry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).BeginInit();
            this.outputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tcOutputs.SuspendLayout();
            this.outputTabData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).BeginInit();
            this.tpPlotIsoPlot.SuspendLayout();
            this.tpt_fname.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlotIsoOutput)).BeginInit();
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
            this.tcIO.Size = new System.Drawing.Size(762, 461);
            this.tcIO.TabIndex = 0;
            // 
            // inputTab
            // 
            this.inputTab.Controls.Add(this.inputWarning);
            this.inputTab.Controls.Add(this.fuelPhaseSelector);
            this.inputTab.Controls.Add(this.phaseLabel);
            this.inputTab.Controls.Add(this.notionalNozzleSelector);
            this.inputTab.Controls.Add(this.label1);
            this.inputTab.Controls.Add(this.spinnerPictureBox);
            this.inputTab.Controls.Add(this.lblContourLevels);
            this.inputTab.Controls.Add(this.tbContourLevels);
            this.inputTab.Controls.Add(this.pbFlameGeometry);
            this.inputTab.Controls.Add(this.lblZElementCount);
            this.inputTab.Controls.Add(this.lblYElemCount);
            this.inputTab.Controls.Add(this.lblXElemCount);
            this.inputTab.Controls.Add(this.tbRadiativeHeatFluxPointsZ);
            this.inputTab.Controls.Add(this.lblHeatFluxPointsZ);
            this.inputTab.Controls.Add(this.tbRadiativeHeatFluxPointsY);
            this.inputTab.Controls.Add(this.lblHeadFluxPointsY);
            this.inputTab.Controls.Add(this.tbRadiativeHeatFluxPointsX);
            this.inputTab.Controls.Add(this.lblHeatFluxPointsX);
            this.inputTab.Controls.Add(this.executeButton);
            this.inputTab.Controls.Add(this.dgInput);
            this.inputTab.Location = new System.Drawing.Point(4, 22);
            this.inputTab.Name = "inputTab";
            this.inputTab.Padding = new System.Windows.Forms.Padding(3);
            this.inputTab.Size = new System.Drawing.Size(754, 435);
            this.inputTab.TabIndex = 0;
            this.inputTab.Text = "Input";
            this.inputTab.UseVisualStyleBackColor = true;
            // 
            // inputWarning
            // 
            this.inputWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.inputWarning.AutoSize = true;
            this.inputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.inputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.inputWarning.Location = new System.Drawing.Point(13, 403);
            this.inputWarning.Name = "inputWarning";
            this.inputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.inputWarning.Size = new System.Drawing.Size(198, 23);
            this.inputWarning.TabIndex = 60;
            this.inputWarning.Text = "Warning/error message here";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
            // 
            // fuelPhaseSelector
            // 
            this.fuelPhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.fuelPhaseSelector.FormattingEnabled = true;
            this.fuelPhaseSelector.Location = new System.Drawing.Point(175, 34);
            this.fuelPhaseSelector.Name = "fuelPhaseSelector";
            this.fuelPhaseSelector.Size = new System.Drawing.Size(275, 21);
            this.fuelPhaseSelector.TabIndex = 58;
            this.fuelPhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.fuelPhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phaseLabel.Location = new System.Drawing.Point(13, 34);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(71, 15);
            this.phaseLabel.TabIndex = 57;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // notionalNozzleSelector
            // 
            this.notionalNozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.notionalNozzleSelector.FormattingEnabled = true;
            this.notionalNozzleSelector.Location = new System.Drawing.Point(175, 7);
            this.notionalNozzleSelector.Name = "notionalNozzleSelector";
            this.notionalNozzleSelector.Size = new System.Drawing.Size(275, 21);
            this.notionalNozzleSelector.TabIndex = 22;
            this.notionalNozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.notionalNozzleSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 18);
            this.label1.TabIndex = 21;
            this.label1.Text = "Notional nozzle model";
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(350, 377);
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
            this.lblContourLevels.Location = new System.Drawing.Point(13, 383);
            this.lblContourLevels.Name = "lblContourLevels";
            this.lblContourLevels.Size = new System.Drawing.Size(132, 13);
            this.lblContourLevels.TabIndex = 18;
            this.lblContourLevels.Text = "Contour Levels (kW/m^2):";
            // 
            // tbContourLevels
            // 
            this.tbContourLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbContourLevels.Location = new System.Drawing.Point(175, 380);
            this.tbContourLevels.Name = "tbContourLevels";
            this.tbContourLevels.Size = new System.Drawing.Size(170, 20);
            this.tbContourLevels.TabIndex = 17;
            this.tbContourLevels.TextChanged += new System.EventHandler(this.tbContourLevels_TextChanged);
            // 
            // pbFlameGeometry
            // 
            this.pbFlameGeometry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFlameGeometry.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.geometry_of_flame;
            this.pbFlameGeometry.Location = new System.Drawing.Point(456, 31);
            this.pbFlameGeometry.Name = "pbFlameGeometry";
            this.pbFlameGeometry.Size = new System.Drawing.Size(292, 265);
            this.pbFlameGeometry.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFlameGeometry.TabIndex = 1;
            this.pbFlameGeometry.TabStop = false;
            // 
            // lblZElementCount
            // 
            this.lblZElementCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblZElementCount.AutoSize = true;
            this.lblZElementCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZElementCount.Location = new System.Drawing.Point(456, 356);
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
            this.lblYElemCount.Location = new System.Drawing.Point(456, 330);
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
            this.lblXElemCount.Location = new System.Drawing.Point(456, 306);
            this.lblXElemCount.Name = "lblXElemCount";
            this.lblXElemCount.Size = new System.Drawing.Size(103, 13);
            this.lblXElemCount.TabIndex = 11;
            this.lblXElemCount.Text = "Array Element Count";
            // 
            // tbRadiativeHeatFluxPointsZ
            // 
            this.tbRadiativeHeatFluxPointsZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbRadiativeHeatFluxPointsZ.Location = new System.Drawing.Point(175, 354);
            this.tbRadiativeHeatFluxPointsZ.Name = "tbRadiativeHeatFluxPointsZ";
            this.tbRadiativeHeatFluxPointsZ.Size = new System.Drawing.Size(275, 20);
            this.tbRadiativeHeatFluxPointsZ.TabIndex = 8;
            this.tbRadiativeHeatFluxPointsZ.TextChanged += new System.EventHandler(this.tbRadiativeHeatFluxPointsZ_TextChanged);
            // 
            // lblHeatFluxPointsZ
            // 
            this.lblHeatFluxPointsZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsZ.AutoSize = true;
            this.lblHeatFluxPointsZ.Location = new System.Drawing.Point(13, 357);
            this.lblHeatFluxPointsZ.Name = "lblHeatFluxPointsZ";
            this.lblHeatFluxPointsZ.Size = new System.Drawing.Size(162, 13);
            this.lblHeatFluxPointsZ.TabIndex = 7;
            this.lblHeatFluxPointsZ.Text = "Z Radiative Heat Flux Points (m):";
            // 
            // tbRadiativeHeatFluxPointsY
            // 
            this.tbRadiativeHeatFluxPointsY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbRadiativeHeatFluxPointsY.Location = new System.Drawing.Point(175, 328);
            this.tbRadiativeHeatFluxPointsY.Name = "tbRadiativeHeatFluxPointsY";
            this.tbRadiativeHeatFluxPointsY.Size = new System.Drawing.Size(275, 20);
            this.tbRadiativeHeatFluxPointsY.TabIndex = 6;
            this.tbRadiativeHeatFluxPointsY.TextChanged += new System.EventHandler(this.tbRadiativeHeatFluxPointsY_TextChanged);
            // 
            // lblHeadFluxPointsY
            // 
            this.lblHeadFluxPointsY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeadFluxPointsY.AutoSize = true;
            this.lblHeadFluxPointsY.Location = new System.Drawing.Point(13, 331);
            this.lblHeadFluxPointsY.Name = "lblHeadFluxPointsY";
            this.lblHeadFluxPointsY.Size = new System.Drawing.Size(162, 13);
            this.lblHeadFluxPointsY.TabIndex = 5;
            this.lblHeadFluxPointsY.Text = "Y Radiative Heat Flux Points (m):";
            // 
            // tbRadiativeHeatFluxPointsX
            // 
            this.tbRadiativeHeatFluxPointsX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tbRadiativeHeatFluxPointsX.Location = new System.Drawing.Point(175, 302);
            this.tbRadiativeHeatFluxPointsX.Name = "tbRadiativeHeatFluxPointsX";
            this.tbRadiativeHeatFluxPointsX.Size = new System.Drawing.Size(275, 20);
            this.tbRadiativeHeatFluxPointsX.TabIndex = 4;
            this.tbRadiativeHeatFluxPointsX.TextChanged += new System.EventHandler(this.tbRadiativeHeatFluxPointsX_TextChanged);
            // 
            // lblHeatFluxPointsX
            // 
            this.lblHeatFluxPointsX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsX.AutoSize = true;
            this.lblHeatFluxPointsX.Location = new System.Drawing.Point(13, 305);
            this.lblHeatFluxPointsX.Name = "lblHeatFluxPointsX";
            this.lblHeatFluxPointsX.Size = new System.Drawing.Size(162, 13);
            this.lblHeatFluxPointsX.TabIndex = 3;
            this.lblHeatFluxPointsX.Text = "X Radiative Heat Flux Points (m):";
            // 
            // executeButton
            // 
            this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.executeButton.Location = new System.Drawing.Point(379, 377);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(71, 23);
            this.executeButton.TabIndex = 2;
            this.executeButton.Text = "Calculate";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // dgInput
            // 
            this.dgInput.AllowUserToAddRows = false;
            this.dgInput.AllowUserToDeleteRows = false;
            this.dgInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgInput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgInput.Location = new System.Drawing.Point(6, 61);
            this.dgInput.Name = "dgInput";
            this.dgInput.Size = new System.Drawing.Size(444, 235);
            this.dgInput.TabIndex = 0;
            this.dgInput.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgInput_CellValueChanged);
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.splitContainer1);
            this.outputTab.Location = new System.Drawing.Point(4, 22);
            this.outputTab.Name = "outputTab";
            this.outputTab.Padding = new System.Windows.Forms.Padding(3);
            this.outputTab.Size = new System.Drawing.Size(754, 435);
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
            this.splitContainer1.Size = new System.Drawing.Size(748, 429);
            this.splitContainer1.SplitterDistance = 400;
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
            this.tcOutputs.Size = new System.Drawing.Size(748, 400);
            this.tcOutputs.TabIndex = 5;
            // 
            // outputTabData
            // 
            this.outputTabData.Controls.Add(this.outputSrad);
            this.outputTabData.Controls.Add(this.lblSeconds);
            this.outputTabData.Controls.Add(this.outputMassFlowRate);
            this.outputTabData.Controls.Add(this.label2);
            this.outputTabData.Controls.Add(this.dgResult);
            this.outputTabData.Controls.Add(this.resultLabel);
            this.outputTabData.Controls.Add(this.btnCopyToClipboard);
            this.outputTabData.Location = new System.Drawing.Point(4, 22);
            this.outputTabData.Name = "outputTabData";
            this.outputTabData.Padding = new System.Windows.Forms.Padding(3);
            this.outputTabData.Size = new System.Drawing.Size(740, 374);
            this.outputTabData.TabIndex = 0;
            this.outputTabData.Text = "Values";
            this.outputTabData.UseVisualStyleBackColor = true;
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
            this.dgResult.Size = new System.Drawing.Size(732, 256);
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
            this.resultLabel.Size = new System.Drawing.Size(241, 13);
            this.resultLabel.TabIndex = 0;
            this.resultLabel.Text = "Radiative heat flux calculated (kW/m^2):";
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyToClipboard.Location = new System.Drawing.Point(615, 344);
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(119, 23);
            this.btnCopyToClipboard.TabIndex = 2;
            this.btnCopyToClipboard.Text = "Copy to Clipboard";
            this.btnCopyToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // tpPlotIsoPlot
            // 
            this.tpPlotIsoPlot.Controls.Add(this.pbPlotIsoOutput);
            this.tpPlotIsoPlot.Location = new System.Drawing.Point(4, 22);
            this.tpPlotIsoPlot.Name = "tpPlotIsoPlot";
            this.tpPlotIsoPlot.Padding = new System.Windows.Forms.Padding(3);
            this.tpPlotIsoPlot.Size = new System.Drawing.Size(740, 374);
            this.tpPlotIsoPlot.TabIndex = 1;
            this.tpPlotIsoPlot.Text = "Heat Flux Plot";
            this.tpPlotIsoPlot.UseVisualStyleBackColor = true;
            // 
            // tpt_fname
            // 
            this.tpt_fname.Controls.Add(this.pbTPlot);
            this.tpt_fname.Location = new System.Drawing.Point(4, 22);
            this.tpt_fname.Name = "tpt_fname";
            this.tpt_fname.Size = new System.Drawing.Size(740, 374);
            this.tpt_fname.TabIndex = 2;
            this.tpt_fname.Text = "Temperature Plot";
            this.tpt_fname.UseVisualStyleBackColor = true;
            // 
            // outputWarning
            // 
            this.outputWarning.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.outputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWarning.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.outputWarning.Location = new System.Drawing.Point(0, 0);
            this.outputWarning.Name = "outputWarning";
            this.outputWarning.Size = new System.Drawing.Size(748, 25);
            this.outputWarning.TabIndex = 19;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
            // 
            // pbPlotIsoOutput
            // 
            this.pbPlotIsoOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPlotIsoOutput.Location = new System.Drawing.Point(3, 3);
            this.pbPlotIsoOutput.Name = "pbPlotIsoOutput";
            this.pbPlotIsoOutput.Size = new System.Drawing.Size(734, 368);
            this.pbPlotIsoOutput.TabIndex = 4;
            this.pbPlotIsoOutput.TabStop = false;
            // 
            // pbTPlot
            // 
            this.pbTPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbTPlot.Location = new System.Drawing.Point(0, 0);
            this.pbTPlot.Name = "pbTPlot";
            this.pbTPlot.Size = new System.Drawing.Size(740, 374);
            this.pbTPlot.TabIndex = 0;
            this.pbTPlot.TabStop = false;
            // 
            // outputSrad
            // 
            this.outputSrad.Location = new System.Drawing.Point(168, 32);
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
            this.lblSeconds.Size = new System.Drawing.Size(155, 13);
            this.lblSeconds.TabIndex = 13;
            this.lblSeconds.Text = "Total emitted radiate power (W)";
            // 
            // outputMassFlowRate
            // 
            this.outputMassFlowRate.Location = new System.Drawing.Point(168, 6);
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
            // JetFlameHeatAnalysisForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcIO);
            this.Name = "JetFlameHeatAnalysisForm";
            this.Size = new System.Drawing.Size(762, 461);
            this.Load += new System.EventHandler(this.JetFlameHeatAnalysisForm_Load);
            this.tcIO.ResumeLayout(false);
            this.inputTab.ResumeLayout(false);
            this.inputTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlameGeometry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).EndInit();
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
            this.tpt_fname.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPlotIsoOutput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbTPlot)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private TabControl tcIO;
		private TabPage inputTab;
		private TabPage outputTab;
		private DataGridView dgInput;

		private Button executeButton;
		private Label resultLabel;
		private Label lblHeatFluxPointsX;
		private TextBox tbRadiativeHeatFluxPointsZ;
		private Label lblHeatFluxPointsZ;
		private TextBox tbRadiativeHeatFluxPointsY;
		private Label lblHeadFluxPointsY;
		private TextBox tbRadiativeHeatFluxPointsX;
		private Button btnCopyToClipboard;
        private DataGridView dgResult;
		private Label lblZElementCount;
		private Label lblYElemCount;
		private Label lblXElemCount;
        private DataGridViewTextBoxColumn colX;
        private DataGridViewTextBoxColumn colY;
        private DataGridViewTextBoxColumn colZ;
        private DataGridViewTextBoxColumn colFlux;
		private PictureBox pbFlameGeometry;
        private TextBox tbContourLevels;
        private Label lblContourLevels;
		private PictureBoxWithSave pbPlotIsoOutput;
		private TabControl tcOutputs;
		private TabPage outputTabData;
		private TabPage tpPlotIsoPlot;
		private TabPage tpt_fname;
		private PictureBoxWithSave pbTPlot;
        private PictureBox spinnerPictureBox;
        private ComboBox notionalNozzleSelector;
        private Label label1;
        private SplitContainer splitContainer1;
        private Label outputWarning;
        private ComboBox fuelPhaseSelector;
        private Label phaseLabel;
        private Label inputWarning;

        public static double[] ExtractFloatArrayFromDelimitedString(string delimitedString, char delimiter)
        {
            char[] delimiters = {delimiter};
            var values = delimitedString.Split(delimiters);
            var result = new double[values.Length];
            for (var index = 0; index < result.Length; index++)
            {
                result[index] = double.NaN;

                if (ParseUtility.TryParseDouble(values[index], out double parsedValue)) result[index] = parsedValue;
            }

            return result;
        }

        private TextBox outputSrad;
        private Label lblSeconds;
        private TextBox outputMassFlowRate;
        private Label label2;
    }
}
