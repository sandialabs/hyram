
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.inputTab = new System.Windows.Forms.TabPage();
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
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.NozzleSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
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
            this.outputTabControl = new System.Windows.Forms.TabControl();
            this.dataTab = new System.Windows.Forms.TabPage();
            this.OutputMassFlowRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.ResultGrid = new System.Windows.Forms.DataGridView();
            this.CopyBtn = new System.Windows.Forms.Button();
            this.overpPlotTab = new System.Windows.Forms.TabPage();
            this.OverpPlotBox = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.impulsePlotTab = new System.Windows.Forms.TabPage();
            this.ImpulsePlotBox = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.outputWarning = new System.Windows.Forms.Label();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.overpressureCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.impulseCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mainTabControl.SuspendLayout();
            this.inputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            this.outputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.outputTabControl.SuspendLayout();
            this.dataTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).BeginInit();
            this.overpPlotTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverpPlotBox)).BeginInit();
            this.impulsePlotTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ImpulsePlotBox)).BeginInit();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.inputTab);
            this.mainTabControl.Controls.Add(this.outputTab);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(992, 594);
            this.mainTabControl.TabIndex = 0;
            // 
            // inputTab
            // 
            this.inputTab.Controls.Add(this.massFlowLabel);
            this.inputTab.Controls.Add(this.MassFlowInput);
            this.inputTab.Controls.Add(this.AutoSetLimits);
            this.inputTab.Controls.Add(this.label4);
            this.inputTab.Controls.Add(this.ImpulseContourInput);
            this.inputTab.Controls.Add(this.lblContourLevels);
            this.inputTab.Controls.Add(this.OverpContourInput);
            this.inputTab.Controls.Add(this.FlameSpeedSelector);
            this.inputTab.Controls.Add(this.flameSpeedLabel);
            this.inputTab.Controls.Add(this.MethodSelector);
            this.inputTab.Controls.Add(this.label3);
            this.inputTab.Controls.Add(this.inputWarning);
            this.inputTab.Controls.Add(this.PhaseSelector);
            this.inputTab.Controls.Add(this.phaseLabel);
            this.inputTab.Controls.Add(this.NozzleSelector);
            this.inputTab.Controls.Add(this.label1);
            this.inputTab.Controls.Add(this.spinnerPictureBox);
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
            this.massFlowLabel.Location = new System.Drawing.Point(13, 117);
            this.massFlowLabel.Name = "massFlowLabel";
            this.massFlowLabel.Size = new System.Drawing.Size(184, 13);
            this.massFlowLabel.TabIndex = 77;
            this.massFlowLabel.Text = "Fluid mass flow rate (unchoked, kg/s)";
            // 
            // MassFlowInput
            // 
            this.MassFlowInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MassFlowInput.Location = new System.Drawing.Point(203, 114);
            this.MassFlowInput.Name = "MassFlowInput";
            this.MassFlowInput.Size = new System.Drawing.Size(247, 20);
            this.MassFlowInput.TabIndex = 76;
            this.MassFlowInput.TextChanged += new System.EventHandler(this.MassFlowInput_TextChanged);
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(203, 145);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(171, 17);
            this.AutoSetLimits.TabIndex = 73;
            this.AutoSetLimits.Text = "Automatically set plot axis limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 493);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 13);
            this.label4.TabIndex = 68;
            this.label4.Text = "Impulse contour levels (kPa*s):";
            // 
            // ImpulseContourInput
            // 
            this.ImpulseContourInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ImpulseContourInput.Location = new System.Drawing.Point(193, 490);
            this.ImpulseContourInput.Name = "ImpulseContourInput";
            this.ImpulseContourInput.Size = new System.Drawing.Size(257, 20);
            this.ImpulseContourInput.TabIndex = 67;
            this.ImpulseContourInput.TextChanged += new System.EventHandler(this.ImpulseContourInput_TextChanged);
            // 
            // lblContourLevels
            // 
            this.lblContourLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblContourLevels.AutoSize = true;
            this.lblContourLevels.Location = new System.Drawing.Point(13, 467);
            this.lblContourLevels.Name = "lblContourLevels";
            this.lblContourLevels.Size = new System.Drawing.Size(170, 13);
            this.lblContourLevels.TabIndex = 66;
            this.lblContourLevels.Text = "Overpressure contour levels (kPa):";
            // 
            // OverpContourInput
            // 
            this.OverpContourInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OverpContourInput.Location = new System.Drawing.Point(193, 464);
            this.OverpContourInput.Name = "OverpContourInput";
            this.OverpContourInput.Size = new System.Drawing.Size(257, 20);
            this.OverpContourInput.TabIndex = 65;
            this.OverpContourInput.TextChanged += new System.EventHandler(this.OverpContourInput_TextChanged);
            // 
            // FlameSpeedSelector
            // 
            this.FlameSpeedSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlameSpeedSelector.FormattingEnabled = true;
            this.FlameSpeedSelector.Location = new System.Drawing.Point(203, 87);
            this.FlameSpeedSelector.Name = "FlameSpeedSelector";
            this.FlameSpeedSelector.Size = new System.Drawing.Size(247, 21);
            this.FlameSpeedSelector.TabIndex = 64;
            this.FlameSpeedSelector.SelectionChangeCommitted += new System.EventHandler(this.FlameSpeedSelector_SelectionChangeCommitted);
            // 
            // flameSpeedLabel
            // 
            this.flameSpeedLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.flameSpeedLabel.Location = new System.Drawing.Point(13, 87);
            this.flameSpeedLabel.Name = "flameSpeedLabel";
            this.flameSpeedLabel.Size = new System.Drawing.Size(190, 18);
            this.flameSpeedLabel.TabIndex = 63;
            this.flameSpeedLabel.Text = "Mach flame speed (BST-only)";
            // 
            // MethodSelector
            // 
            this.MethodSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MethodSelector.FormattingEnabled = true;
            this.MethodSelector.Location = new System.Drawing.Point(203, 6);
            this.MethodSelector.Name = "MethodSelector";
            this.MethodSelector.Size = new System.Drawing.Size(247, 21);
            this.MethodSelector.TabIndex = 62;
            this.MethodSelector.SelectionChangeCommitted += new System.EventHandler(this.MethodSelector_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 6);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(156, 18);
            this.label3.TabIndex = 61;
            this.label3.Text = "Calculation method";
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
            this.PhaseSelector.Location = new System.Drawing.Point(203, 60);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(247, 21);
            this.PhaseSelector.TabIndex = 58;
            this.PhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.PhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phaseLabel.Location = new System.Drawing.Point(13, 60);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(71, 15);
            this.phaseLabel.TabIndex = 57;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // NozzleSelector
            // 
            this.NozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NozzleSelector.FormattingEnabled = true;
            this.NozzleSelector.Location = new System.Drawing.Point(203, 33);
            this.NozzleSelector.Name = "NozzleSelector";
            this.NozzleSelector.Size = new System.Drawing.Size(247, 21);
            this.NozzleSelector.TabIndex = 22;
            this.NozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.NozzleSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(156, 18);
            this.label1.TabIndex = 21;
            this.label1.Text = "Notional nozzle model";
            // 
            // spinnerPictureBox
            // 
            this.spinnerPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.spinnerPictureBox.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.spinnerPictureBox.Location = new System.Drawing.Point(350, 516);
            this.spinnerPictureBox.Margin = new System.Windows.Forms.Padding(2);
            this.spinnerPictureBox.MinimumSize = new System.Drawing.Size(15, 16);
            this.spinnerPictureBox.Name = "spinnerPictureBox";
            this.spinnerPictureBox.Size = new System.Drawing.Size(24, 23);
            this.spinnerPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.spinnerPictureBox.TabIndex = 20;
            this.spinnerPictureBox.TabStop = false;
            // 
            // lblZElementCount
            // 
            this.lblZElementCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblZElementCount.AutoSize = true;
            this.lblZElementCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblZElementCount.Location = new System.Drawing.Point(456, 440);
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
            this.lblYElemCount.Location = new System.Drawing.Point(456, 414);
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
            this.lblXElemCount.Location = new System.Drawing.Point(456, 390);
            this.lblXElemCount.Name = "lblXElemCount";
            this.lblXElemCount.Size = new System.Drawing.Size(103, 13);
            this.lblXElemCount.TabIndex = 11;
            this.lblXElemCount.Text = "Array Element Count";
            // 
            // LocZInput
            // 
            this.LocZInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LocZInput.Location = new System.Drawing.Point(193, 438);
            this.LocZInput.Name = "LocZInput";
            this.LocZInput.Size = new System.Drawing.Size(257, 20);
            this.LocZInput.TabIndex = 8;
            this.LocZInput.TextChanged += new System.EventHandler(this.Locations_TextChanged);
            // 
            // lblHeatFluxPointsZ
            // 
            this.lblHeatFluxPointsZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsZ.AutoSize = true;
            this.lblHeatFluxPointsZ.Location = new System.Drawing.Point(13, 441);
            this.lblHeatFluxPointsZ.Name = "lblHeatFluxPointsZ";
            this.lblHeatFluxPointsZ.Size = new System.Drawing.Size(78, 13);
            this.lblHeatFluxPointsZ.TabIndex = 7;
            this.lblHeatFluxPointsZ.Text = "Z positions (m):";
            // 
            // LocYInput
            // 
            this.LocYInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LocYInput.Location = new System.Drawing.Point(193, 412);
            this.LocYInput.Name = "LocYInput";
            this.LocYInput.Size = new System.Drawing.Size(257, 20);
            this.LocYInput.TabIndex = 6;
            this.LocYInput.TextChanged += new System.EventHandler(this.Locations_TextChanged);
            // 
            // lblHeadFluxPointsY
            // 
            this.lblHeadFluxPointsY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeadFluxPointsY.AutoSize = true;
            this.lblHeadFluxPointsY.Location = new System.Drawing.Point(13, 415);
            this.lblHeadFluxPointsY.Name = "lblHeadFluxPointsY";
            this.lblHeadFluxPointsY.Size = new System.Drawing.Size(78, 13);
            this.lblHeadFluxPointsY.TabIndex = 5;
            this.lblHeadFluxPointsY.Text = "Y positions (m):";
            // 
            // LocXInput
            // 
            this.LocXInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LocXInput.Location = new System.Drawing.Point(193, 386);
            this.LocXInput.Name = "LocXInput";
            this.LocXInput.Size = new System.Drawing.Size(257, 20);
            this.LocXInput.TabIndex = 4;
            this.LocXInput.TextChanged += new System.EventHandler(this.Locations_TextChanged);
            // 
            // lblHeatFluxPointsX
            // 
            this.lblHeatFluxPointsX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsX.AutoSize = true;
            this.lblHeatFluxPointsX.Location = new System.Drawing.Point(13, 389);
            this.lblHeatFluxPointsX.Name = "lblHeatFluxPointsX";
            this.lblHeatFluxPointsX.Size = new System.Drawing.Size(78, 13);
            this.lblHeatFluxPointsX.TabIndex = 3;
            this.lblHeatFluxPointsX.Text = "X positions (m):";
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SubmitBtn.Location = new System.Drawing.Point(379, 516);
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
            this.InputGrid.Location = new System.Drawing.Point(6, 168);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.Size = new System.Drawing.Size(444, 212);
            this.InputGrid.TabIndex = 0;
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.GridInput_CellValueChanged);
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
            this.splitContainer1.Panel1.Controls.Add(this.outputTabControl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputWarning);
            this.splitContainer1.Size = new System.Drawing.Size(978, 562);
            this.splitContainer1.SplitterDistance = 523;
            this.splitContainer1.TabIndex = 6;
            // 
            // outputTabControl
            // 
            this.outputTabControl.Controls.Add(this.dataTab);
            this.outputTabControl.Controls.Add(this.overpPlotTab);
            this.outputTabControl.Controls.Add(this.impulsePlotTab);
            this.outputTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputTabControl.Location = new System.Drawing.Point(0, 0);
            this.outputTabControl.Name = "outputTabControl";
            this.outputTabControl.SelectedIndex = 0;
            this.outputTabControl.Size = new System.Drawing.Size(978, 523);
            this.outputTabControl.TabIndex = 5;
            // 
            // dataTab
            // 
            this.dataTab.Controls.Add(this.OutputMassFlowRate);
            this.dataTab.Controls.Add(this.label2);
            this.dataTab.Controls.Add(this.ResultGrid);
            this.dataTab.Controls.Add(this.CopyBtn);
            this.dataTab.Location = new System.Drawing.Point(4, 22);
            this.dataTab.Name = "dataTab";
            this.dataTab.Padding = new System.Windows.Forms.Padding(3);
            this.dataTab.Size = new System.Drawing.Size(970, 497);
            this.dataTab.TabIndex = 0;
            this.dataTab.Text = "Values";
            this.dataTab.UseVisualStyleBackColor = true;
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
            this.ResultGrid.Location = new System.Drawing.Point(3, 35);
            this.ResultGrid.Name = "ResultGrid";
            this.ResultGrid.Size = new System.Drawing.Size(962, 426);
            this.ResultGrid.TabIndex = 3;
            // 
            // CopyBtn
            // 
            this.CopyBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CopyBtn.Location = new System.Drawing.Point(845, 467);
            this.CopyBtn.Name = "CopyBtn";
            this.CopyBtn.Size = new System.Drawing.Size(119, 23);
            this.CopyBtn.TabIndex = 2;
            this.CopyBtn.Text = "Copy to Clipboard";
            this.CopyBtn.UseVisualStyleBackColor = true;
            this.CopyBtn.Click += new System.EventHandler(this.CopyBtn_Click);
            // 
            // overpPlotTab
            // 
            this.overpPlotTab.Controls.Add(this.OverpPlotBox);
            this.overpPlotTab.Location = new System.Drawing.Point(4, 22);
            this.overpPlotTab.Name = "overpPlotTab";
            this.overpPlotTab.Padding = new System.Windows.Forms.Padding(3);
            this.overpPlotTab.Size = new System.Drawing.Size(970, 497);
            this.overpPlotTab.TabIndex = 1;
            this.overpPlotTab.Text = "Overpressure plot";
            this.overpPlotTab.UseVisualStyleBackColor = true;
            // 
            // OverpPlotBox
            // 
            this.OverpPlotBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OverpPlotBox.Location = new System.Drawing.Point(3, 3);
            this.OverpPlotBox.Name = "OverpPlotBox";
            this.OverpPlotBox.Size = new System.Drawing.Size(964, 491);
            this.OverpPlotBox.TabIndex = 4;
            this.OverpPlotBox.TabStop = false;
            // 
            // impulsePlotTab
            // 
            this.impulsePlotTab.Controls.Add(this.ImpulsePlotBox);
            this.impulsePlotTab.Location = new System.Drawing.Point(4, 22);
            this.impulsePlotTab.Name = "impulsePlotTab";
            this.impulsePlotTab.Padding = new System.Windows.Forms.Padding(3);
            this.impulsePlotTab.Size = new System.Drawing.Size(970, 497);
            this.impulsePlotTab.TabIndex = 2;
            this.impulsePlotTab.Text = "Impulse plot";
            this.impulsePlotTab.UseVisualStyleBackColor = true;
            // 
            // ImpulsePlotBox
            // 
            this.ImpulsePlotBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ImpulsePlotBox.Location = new System.Drawing.Point(3, 3);
            this.ImpulsePlotBox.Name = "ImpulsePlotBox";
            this.ImpulsePlotBox.Size = new System.Drawing.Size(964, 491);
            this.ImpulsePlotBox.TabIndex = 5;
            this.ImpulsePlotBox.TabStop = false;
            // 
            // outputWarning
            // 
            this.outputWarning.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.outputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWarning.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.outputWarning.Location = new System.Drawing.Point(0, 0);
            this.outputWarning.Name = "outputWarning";
            this.outputWarning.Size = new System.Drawing.Size(978, 35);
            this.outputWarning.TabIndex = 19;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
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
            // UnconfinedOverpressureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTabControl);
            this.Name = "UnconfinedOverpressureForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.UnconfinedOverpressureForm_Load);
            this.mainTabControl.ResumeLayout(false);
            this.inputTab.ResumeLayout(false);
            this.inputTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            this.outputTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.outputTabControl.ResumeLayout(false);
            this.dataTab.ResumeLayout(false);
            this.dataTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).EndInit();
            this.overpPlotTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.OverpPlotBox)).EndInit();
            this.impulsePlotTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ImpulsePlotBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl mainTabControl;
        private TabPage inputTab;
        private TabPage outputTab;
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
        private Label lblZElementCount;
        private Label lblYElemCount;
        private Label lblXElemCount;
        private PictureBoxWithSave OverpPlotBox;
        private TabControl outputTabControl;
        private TabPage dataTab;
        private TabPage overpPlotTab;
        private PictureBox spinnerPictureBox;
        private ComboBox NozzleSelector;
        private Label label1;
        private SplitContainer splitContainer1;
        private Label outputWarning;
        private ComboBox PhaseSelector;
        private Label phaseLabel;
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
        private PictureBoxWithSave ImpulsePlotBox;
        private CheckBox AutoSetLimits;
        private Label massFlowLabel;
        private TextBox MassFlowInput;
        private DataGridViewTextBoxColumn colX;
        private DataGridViewTextBoxColumn colY;
        private DataGridViewTextBoxColumn colZ;
        private DataGridViewTextBoxColumn overpressureCol;
        private DataGridViewTextBoxColumn impulseCol;
    }
}
