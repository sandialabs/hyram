
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    partial class UnconfinedOverpressureForm
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

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
            this.massFlowLabel = new System.Windows.Forms.Label();
            this.MassFlowInput = new System.Windows.Forms.TextBox();
            this.AutoSetLimits = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.ImpulseContourInput = new System.Windows.Forms.TextBox();
            this.lblContourLevels = new System.Windows.Forms.Label();
            this.OverpContourInput = new System.Windows.Forms.TextBox();
            this.FlameSpeedSelector = new System.Windows.Forms.ComboBox();
            this.flameSpeedLabel = new System.Windows.Forms.Label();
            this.MethodSelector = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.inputWarning = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.ZCountLabel = new System.Windows.Forms.Label();
            this.YCountLabel = new System.Windows.Forms.Label();
            this.XCountLabel = new System.Windows.Forms.Label();
            this.LocZInput = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsZ = new System.Windows.Forms.Label();
            this.LocYInput = new System.Windows.Forms.TextBox();
            this.lblHeadFluxPointsY = new System.Windows.Forms.Label();
            this.LocXInput = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsX = new System.Windows.Forms.Label();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.outputTabControl = new System.Windows.Forms.TabControl();
            this.overpPlotTab = new System.Windows.Forms.TabPage();
            this.resultTipLabel = new System.Windows.Forms.Label();
            this.OverpPlot = new System.Windows.Forms.PictureBox();
            this.impulsePlotTab = new System.Windows.Forms.TabPage();
            this.ImpulsePlot = new System.Windows.Forms.PictureBox();
            this.dataTab = new System.Windows.Forms.TabPage();
            this.FlamDetMassOutput = new System.Windows.Forms.TextBox();
            this.FlamDetMassLabel = new System.Windows.Forms.Label();
            this.OutputMassFlowRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ResultGrid = new System.Windows.Forms.DataGridView();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.overpressureCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.impulseCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CopyBtn = new System.Windows.Forms.Button();
            this.outputWarning = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.outputTabControl.SuspendLayout();
            this.overpPlotTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverpPlot)).BeginInit();
            this.impulsePlotTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImpulsePlot)).BeginInit();
            this.dataTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // massFlowLabel
            // 
            this.massFlowLabel.AutoSize = true;
            this.massFlowLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.massFlowLabel.Location = new System.Drawing.Point(3, 200);
            this.massFlowLabel.Name = "massFlowLabel";
            this.massFlowLabel.Size = new System.Drawing.Size(116, 30);
            this.massFlowLabel.TabIndex = 77;
            this.massFlowLabel.Text = "Fluid mass flow rate\r\n(unchoked, kg/s)";
            // 
            // MassFlowInput
            // 
            this.MassFlowInput.Location = new System.Drawing.Point(205, 197);
            this.MassFlowInput.Name = "MassFlowInput";
            this.MassFlowInput.Size = new System.Drawing.Size(182, 20);
            this.MassFlowInput.TabIndex = 8;
            this.MassFlowInput.TextChanged += new System.EventHandler(this.MassFlowInput_TextChanged);
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(205, 224);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(139, 17);
            this.AutoSetLimits.TabIndex = 9;
            this.AutoSetLimits.Text = "Automatic plot axis limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 174);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(172, 15);
            this.label4.TabIndex = 68;
            this.label4.Text = "Impulse contour levels (kPa*s)";
            // 
            // ImpulseContourInput
            // 
            this.ImpulseContourInput.Location = new System.Drawing.Point(205, 170);
            this.ImpulseContourInput.Name = "ImpulseContourInput";
            this.ImpulseContourInput.Size = new System.Drawing.Size(182, 20);
            this.ImpulseContourInput.TabIndex = 7;
            this.ImpulseContourInput.TextChanged += new System.EventHandler(this.ImpulseContourInput_TextChanged);
            // 
            // lblContourLevels
            // 
            this.lblContourLevels.AutoSize = true;
            this.lblContourLevels.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContourLevels.Location = new System.Drawing.Point(6, 148);
            this.lblContourLevels.Name = "lblContourLevels";
            this.lblContourLevels.Size = new System.Drawing.Size(190, 15);
            this.lblContourLevels.TabIndex = 66;
            this.lblContourLevels.Text = "Overpressure contour levels (kPa)";
            // 
            // OverpContourInput
            // 
            this.OverpContourInput.Location = new System.Drawing.Point(205, 144);
            this.OverpContourInput.Name = "OverpContourInput";
            this.OverpContourInput.Size = new System.Drawing.Size(182, 20);
            this.OverpContourInput.TabIndex = 6;
            this.OverpContourInput.TextChanged += new System.EventHandler(this.OverpContourInput_TextChanged);
            // 
            // FlameSpeedSelector
            // 
            this.FlameSpeedSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlameSpeedSelector.FormattingEnabled = true;
            this.FlameSpeedSelector.Location = new System.Drawing.Point(205, 37);
            this.FlameSpeedSelector.Name = "FlameSpeedSelector";
            this.FlameSpeedSelector.Size = new System.Drawing.Size(182, 21);
            this.FlameSpeedSelector.TabIndex = 2;
            this.FlameSpeedSelector.SelectionChangeCommitted += new System.EventHandler(this.FlameSpeedSelector_SelectionChangeCommitted);
            // 
            // flameSpeedLabel
            // 
            this.flameSpeedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flameSpeedLabel.Location = new System.Drawing.Point(6, 40);
            this.flameSpeedLabel.Name = "flameSpeedLabel";
            this.flameSpeedLabel.Size = new System.Drawing.Size(190, 18);
            this.flameSpeedLabel.TabIndex = 63;
            this.flameSpeedLabel.Text = "Mach flame speed (BST-only)";
            // 
            // MethodSelector
            // 
            this.MethodSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MethodSelector.FormattingEnabled = true;
            this.MethodSelector.Location = new System.Drawing.Point(205, 10);
            this.MethodSelector.Name = "MethodSelector";
            this.MethodSelector.Size = new System.Drawing.Size(182, 21);
            this.MethodSelector.TabIndex = 1;
            this.MethodSelector.SelectionChangeCommitted += new System.EventHandler(this.MethodSelector_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 18);
            this.label3.TabIndex = 61;
            this.label3.Text = "Calculation method";
            // 
            // inputWarning
            // 
            this.inputWarning.AutoSize = true;
            this.inputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.inputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.inputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.inputWarning.Location = new System.Drawing.Point(3, 581);
            this.inputWarning.Name = "inputWarning";
            this.inputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.inputWarning.Size = new System.Drawing.Size(384, 23);
            this.inputWarning.TabIndex = 60;
            this.inputWarning.Text = "Test warning notification area with long warning message";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(287, 552);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 20;
            this.spinnerPictureBox.TabStop = false;
            // 
            // ZCountLabel
            // 
            this.ZCountLabel.AutoSize = true;
            this.ZCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ZCountLabel.Location = new System.Drawing.Point(127, 124);
            this.ZCountLabel.Name = "ZCountLabel";
            this.ZCountLabel.Size = new System.Drawing.Size(66, 13);
            this.ZCountLabel.TabIndex = 13;
            this.ZCountLabel.Text = "## elements";
            // 
            // YCountLabel
            // 
            this.YCountLabel.AutoSize = true;
            this.YCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YCountLabel.Location = new System.Drawing.Point(127, 98);
            this.YCountLabel.Name = "YCountLabel";
            this.YCountLabel.Size = new System.Drawing.Size(66, 13);
            this.YCountLabel.TabIndex = 12;
            this.YCountLabel.Text = "## elements";
            // 
            // XCountLabel
            // 
            this.XCountLabel.AutoSize = true;
            this.XCountLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.XCountLabel.Location = new System.Drawing.Point(127, 72);
            this.XCountLabel.Name = "XCountLabel";
            this.XCountLabel.Size = new System.Drawing.Size(66, 13);
            this.XCountLabel.TabIndex = 11;
            this.XCountLabel.Text = "## elements";
            // 
            // LocZInput
            // 
            this.LocZInput.Location = new System.Drawing.Point(205, 118);
            this.LocZInput.Name = "LocZInput";
            this.LocZInput.Size = new System.Drawing.Size(182, 20);
            this.LocZInput.TabIndex = 5;
            this.LocZInput.TextChanged += new System.EventHandler(this.Locations_TextChanged);
            // 
            // lblHeatFluxPointsZ
            // 
            this.lblHeatFluxPointsZ.AutoSize = true;
            this.lblHeatFluxPointsZ.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeatFluxPointsZ.Location = new System.Drawing.Point(6, 121);
            this.lblHeatFluxPointsZ.Name = "lblHeatFluxPointsZ";
            this.lblHeatFluxPointsZ.Size = new System.Drawing.Size(88, 15);
            this.lblHeatFluxPointsZ.TabIndex = 7;
            this.lblHeatFluxPointsZ.Text = "Z positions (m)";
            // 
            // LocYInput
            // 
            this.LocYInput.Location = new System.Drawing.Point(205, 92);
            this.LocYInput.Name = "LocYInput";
            this.LocYInput.Size = new System.Drawing.Size(182, 20);
            this.LocYInput.TabIndex = 4;
            this.LocYInput.TextChanged += new System.EventHandler(this.Locations_TextChanged);
            // 
            // lblHeadFluxPointsY
            // 
            this.lblHeadFluxPointsY.AutoSize = true;
            this.lblHeadFluxPointsY.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeadFluxPointsY.Location = new System.Drawing.Point(6, 95);
            this.lblHeadFluxPointsY.Name = "lblHeadFluxPointsY";
            this.lblHeadFluxPointsY.Size = new System.Drawing.Size(88, 15);
            this.lblHeadFluxPointsY.TabIndex = 5;
            this.lblHeadFluxPointsY.Text = "Y positions (m)";
            // 
            // LocXInput
            // 
            this.LocXInput.Location = new System.Drawing.Point(205, 66);
            this.LocXInput.Name = "LocXInput";
            this.LocXInput.Size = new System.Drawing.Size(182, 20);
            this.LocXInput.TabIndex = 3;
            this.LocXInput.TextChanged += new System.EventHandler(this.Locations_TextChanged);
            // 
            // lblHeatFluxPointsX
            // 
            this.lblHeatFluxPointsX.AutoSize = true;
            this.lblHeatFluxPointsX.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeatFluxPointsX.Location = new System.Drawing.Point(6, 69);
            this.lblHeatFluxPointsX.Name = "lblHeatFluxPointsX";
            this.lblHeatFluxPointsX.Size = new System.Drawing.Size(89, 15);
            this.lblHeatFluxPointsX.TabIndex = 3;
            this.lblHeatFluxPointsX.Text = "X positions (m)";
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Location = new System.Drawing.Point(316, 552);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(71, 23);
            this.SubmitBtn.TabIndex = 11;
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
            this.InputGrid.Location = new System.Drawing.Point(6, 249);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.Size = new System.Drawing.Size(381, 297);
            this.InputGrid.TabIndex = 10;
            this.InputGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.InputGrid_CellValidating);
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridInput_CellValueChanged);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(393, 10);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.outputTabControl);
            this.splitContainer1.Panel1MinSize = 400;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputWarning);
            this.splitContainer1.Size = new System.Drawing.Size(754, 600);
            this.splitContainer1.SplitterDistance = 569;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 6;
            // 
            // outputTabControl
            // 
            this.outputTabControl.Controls.Add(this.overpPlotTab);
            this.outputTabControl.Controls.Add(this.impulsePlotTab);
            this.outputTabControl.Controls.Add(this.dataTab);
            this.outputTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTabControl.Location = new System.Drawing.Point(0, 0);
            this.outputTabControl.Name = "outputTabControl";
            this.outputTabControl.SelectedIndex = 0;
            this.outputTabControl.Size = new System.Drawing.Size(754, 569);
            this.outputTabControl.TabIndex = 2;
            // 
            // overpPlotTab
            // 
            this.overpPlotTab.Controls.Add(this.resultTipLabel);
            this.overpPlotTab.Controls.Add(this.OverpPlot);
            this.overpPlotTab.Location = new System.Drawing.Point(4, 22);
            this.overpPlotTab.Name = "overpPlotTab";
            this.overpPlotTab.Padding = new System.Windows.Forms.Padding(3);
            this.overpPlotTab.Size = new System.Drawing.Size(746, 543);
            this.overpPlotTab.TabIndex = 1;
            this.overpPlotTab.Text = "Overpressure plot";
            this.overpPlotTab.UseVisualStyleBackColor = true;
            // 
            // resultTipLabel
            // 
            this.resultTipLabel.AutoSize = true;
            this.resultTipLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.resultTipLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.resultTipLabel.Location = new System.Drawing.Point(262, 261);
            this.resultTipLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.resultTipLabel.Name = "resultTipLabel";
            this.resultTipLabel.Size = new System.Drawing.Size(222, 20);
            this.resultTipLabel.TabIndex = 5;
            this.resultTipLabel.Text = "Submit analysis to view results";
            // 
            // OverpPlot
            // 
            this.OverpPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OverpPlot.Location = new System.Drawing.Point(3, 3);
            this.OverpPlot.Name = "OverpPlot";
            this.OverpPlot.Size = new System.Drawing.Size(740, 537);
            this.OverpPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.OverpPlot.TabIndex = 6;
            this.OverpPlot.TabStop = false;
            // 
            // impulsePlotTab
            // 
            this.impulsePlotTab.Controls.Add(this.ImpulsePlot);
            this.impulsePlotTab.Location = new System.Drawing.Point(4, 22);
            this.impulsePlotTab.Name = "impulsePlotTab";
            this.impulsePlotTab.Padding = new System.Windows.Forms.Padding(3);
            this.impulsePlotTab.Size = new System.Drawing.Size(746, 543);
            this.impulsePlotTab.TabIndex = 2;
            this.impulsePlotTab.Text = "Impulse plot";
            this.impulsePlotTab.UseVisualStyleBackColor = true;
            // 
            // ImpulsePlot
            // 
            this.ImpulsePlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImpulsePlot.Location = new System.Drawing.Point(3, 3);
            this.ImpulsePlot.Name = "ImpulsePlot";
            this.ImpulsePlot.Size = new System.Drawing.Size(740, 537);
            this.ImpulsePlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.ImpulsePlot.TabIndex = 6;
            this.ImpulsePlot.TabStop = false;
            // 
            // dataTab
            // 
            this.dataTab.Controls.Add(this.FlamDetMassOutput);
            this.dataTab.Controls.Add(this.FlamDetMassLabel);
            this.dataTab.Controls.Add(this.OutputMassFlowRate);
            this.dataTab.Controls.Add(this.label2);
            this.dataTab.Controls.Add(this.ResultGrid);
            this.dataTab.Controls.Add(this.CopyBtn);
            this.dataTab.Location = new System.Drawing.Point(4, 22);
            this.dataTab.Name = "dataTab";
            this.dataTab.Padding = new System.Windows.Forms.Padding(3);
            this.dataTab.Size = new System.Drawing.Size(746, 543);
            this.dataTab.TabIndex = 0;
            this.dataTab.Text = "Values";
            this.dataTab.UseVisualStyleBackColor = true;
            // 
            // FlamDetMassOutput
            // 
            this.FlamDetMassOutput.Location = new System.Drawing.Point(400, 9);
            this.FlamDetMassOutput.Name = "FlamDetMassOutput";
            this.FlamDetMassOutput.ReadOnly = true;
            this.FlamDetMassOutput.Size = new System.Drawing.Size(114, 20);
            this.FlamDetMassOutput.TabIndex = 18;
            // 
            // FlamDetMassLabel
            // 
            this.FlamDetMassLabel.AutoSize = true;
            this.FlamDetMassLabel.Location = new System.Drawing.Point(288, 12);
            this.FlamDetMassLabel.Name = "FlamDetMassLabel";
            this.FlamDetMassLabel.Size = new System.Drawing.Size(105, 13);
            this.FlamDetMassLabel.TabIndex = 17;
            this.FlamDetMassLabel.Text = "Flammable mass (kg)";
            // 
            // OutputMassFlowRate
            // 
            this.OutputMassFlowRate.Location = new System.Drawing.Point(118, 9);
            this.OutputMassFlowRate.Name = "OutputMassFlowRate";
            this.OutputMassFlowRate.ReadOnly = true;
            this.OutputMassFlowRate.Size = new System.Drawing.Size(114, 20);
            this.OutputMassFlowRate.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 15;
            this.label2.Text = "Mass flow rate (kg/s)";
            // 
            // ResultGrid
            // 
            this.ResultGrid.AllowUserToAddRows = false;
            this.ResultGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.ResultGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colX,
            this.colY,
            this.colZ,
            this.overpressureCol,
            this.impulseCol});
            this.ResultGrid.Location = new System.Drawing.Point(3, 36);
            this.ResultGrid.Name = "ResultGrid";
            this.ResultGrid.RowHeadersVisible = false;
            this.ResultGrid.Size = new System.Drawing.Size(738, 471);
            this.ResultGrid.TabIndex = 3;
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
            // overpressureCol
            // 
            this.overpressureCol.HeaderText = "Overpressure (kPa)";
            this.overpressureCol.Name = "overpressureCol";
            this.overpressureCol.ReadOnly = true;
            // 
            // impulseCol
            // 
            this.impulseCol.HeaderText = "Impulse (kPa*s)";
            this.impulseCol.Name = "impulseCol";
            this.impulseCol.ReadOnly = true;
            // 
            // CopyBtn
            // 
            this.CopyBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CopyBtn.Location = new System.Drawing.Point(621, 513);
            this.CopyBtn.Name = "CopyBtn";
            this.CopyBtn.Size = new System.Drawing.Size(119, 23);
            this.CopyBtn.TabIndex = 2;
            this.CopyBtn.Text = "Copy to Clipboard";
            this.CopyBtn.UseVisualStyleBackColor = true;
            this.CopyBtn.Click += new System.EventHandler(this.CopyBtn_Click);
            // 
            // outputWarning
            // 
            this.outputWarning.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.outputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWarning.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.outputWarning.Location = new System.Drawing.Point(0, 0);
            this.outputWarning.Name = "outputWarning";
            this.outputWarning.Size = new System.Drawing.Size(754, 29);
            this.outputWarning.TabIndex = 19;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
            // 
            // UnconfinedOverpressureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.Controls.Add(this.massFlowLabel);
            this.Controls.Add(this.MassFlowInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.AutoSetLimits);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.InputGrid);
            this.Controls.Add(this.ImpulseContourInput);
            this.Controls.Add(this.SubmitBtn);
            this.Controls.Add(this.lblContourLevels);
            this.Controls.Add(this.lblHeatFluxPointsX);
            this.Controls.Add(this.OverpContourInput);
            this.Controls.Add(this.LocXInput);
            this.Controls.Add(this.FlameSpeedSelector);
            this.Controls.Add(this.lblHeadFluxPointsY);
            this.Controls.Add(this.flameSpeedLabel);
            this.Controls.Add(this.LocYInput);
            this.Controls.Add(this.MethodSelector);
            this.Controls.Add(this.lblHeatFluxPointsZ);
            this.Controls.Add(this.LocZInput);
            this.Controls.Add(this.inputWarning);
            this.Controls.Add(this.XCountLabel);
            this.Controls.Add(this.spinnerPictureBox);
            this.Controls.Add(this.YCountLabel);
            this.Controls.Add(this.ZCountLabel);
            this.Name = "UnconfinedOverpressureForm";
            this.Size = new System.Drawing.Size(1150, 613);
            this.Load += new System.EventHandler(this.UnconfinedOverpressureForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.outputTabControl.ResumeLayout(false);
            this.overpPlotTab.ResumeLayout(false);
            this.overpPlotTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverpPlot)).EndInit();
            this.impulsePlotTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImpulsePlot)).EndInit();
            this.dataTab.ResumeLayout(false);
            this.dataTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private DataGridView InputGrid;

        private Button SubmitBtn;
        private Label lblHeatFluxPointsX;
        private TextBox LocZInput;
        private Label lblHeatFluxPointsZ;
        private TextBox LocYInput;
        private Label lblHeadFluxPointsY;
        private TextBox LocXInput;
        private Button CopyBtn;
        private DataGridView ResultGrid;
        private Label ZCountLabel;
        private Label YCountLabel;
        private Label XCountLabel;
        private TabControl outputTabControl;
        private TabPage dataTab;
        private TabPage overpPlotTab;
        private PictureBox spinnerPictureBox;
        private SplitContainer splitContainer1;
        private Label outputWarning;
        private Label inputWarning;

        private ComboBox MethodSelector;
        private Label label3;
        private ComboBox FlameSpeedSelector;
        private Label flameSpeedLabel;
        private Label lblContourLevels;
        private TextBox OverpContourInput;
        private TextBox OutputMassFlowRate;
        private Label label2;
        private Label label4;
        private TextBox ImpulseContourInput;
        private TabPage impulsePlotTab;
        private CheckBox AutoSetLimits;
        private Label massFlowLabel;
        private TextBox MassFlowInput;
        private DataGridViewTextBoxColumn colX;
        private DataGridViewTextBoxColumn colY;
        private DataGridViewTextBoxColumn colZ;
        private DataGridViewTextBoxColumn overpressureCol;
        private DataGridViewTextBoxColumn impulseCol;
        private Label resultTipLabel;
        private TextBox FlamDetMassOutput;
        private Label FlamDetMassLabel;
        private PictureBox OverpPlot;
        private PictureBox ImpulsePlot;
    }
}
