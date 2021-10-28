
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
            this.tcIO = new System.Windows.Forms.TabControl();
            this.inputTab = new System.Windows.Forms.TabPage();
            this.flameSpeedSelector = new System.Windows.Forms.ComboBox();
            this.flameSpeedLabel = new System.Windows.Forms.Label();
            this.methodSelector = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.inputWarning = new System.Windows.Forms.Label();
            this.fuelPhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.notionalNozzleSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.lblZElementCount = new System.Windows.Forms.Label();
            this.lblYElemCount = new System.Windows.Forms.Label();
            this.lblXElemCount = new System.Windows.Forms.Label();
            this.zLocsInput = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsZ = new System.Windows.Forms.Label();
            this.yLocsInput = new System.Windows.Forms.TextBox();
            this.lblHeadFluxPointsY = new System.Windows.Forms.Label();
            this.xLocsInput = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsX = new System.Windows.Forms.Label();
            this.executeButton = new System.Windows.Forms.Button();
            this.dgInput = new System.Windows.Forms.DataGridView();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tcOutputs = new System.Windows.Forms.TabControl();
            this.dataTab = new System.Windows.Forms.TabPage();
            this.dgResult = new System.Windows.Forms.DataGridView();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.overpressureCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.impulseCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnCopyToClipboard = new System.Windows.Forms.Button();
            this.plotTab = new System.Windows.Forms.TabPage();
            this.plotBox = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.outputWarning = new System.Windows.Forms.Label();
            this.tcIO.SuspendLayout();
            this.inputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).BeginInit();
            this.outputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tcOutputs.SuspendLayout();
            this.dataTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).BeginInit();
            this.plotTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.plotBox)).BeginInit();
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
            this.tcIO.Size = new System.Drawing.Size(762, 593);
            this.tcIO.TabIndex = 0;
            // 
            // inputTab
            // 
            this.inputTab.Controls.Add(this.flameSpeedSelector);
            this.inputTab.Controls.Add(this.flameSpeedLabel);
            this.inputTab.Controls.Add(this.methodSelector);
            this.inputTab.Controls.Add(this.label3);
            this.inputTab.Controls.Add(this.inputWarning);
            this.inputTab.Controls.Add(this.fuelPhaseSelector);
            this.inputTab.Controls.Add(this.phaseLabel);
            this.inputTab.Controls.Add(this.notionalNozzleSelector);
            this.inputTab.Controls.Add(this.label1);
            this.inputTab.Controls.Add(this.spinnerPictureBox);
            this.inputTab.Controls.Add(this.lblZElementCount);
            this.inputTab.Controls.Add(this.lblYElemCount);
            this.inputTab.Controls.Add(this.lblXElemCount);
            this.inputTab.Controls.Add(this.zLocsInput);
            this.inputTab.Controls.Add(this.lblHeatFluxPointsZ);
            this.inputTab.Controls.Add(this.yLocsInput);
            this.inputTab.Controls.Add(this.lblHeadFluxPointsY);
            this.inputTab.Controls.Add(this.xLocsInput);
            this.inputTab.Controls.Add(this.lblHeatFluxPointsX);
            this.inputTab.Controls.Add(this.executeButton);
            this.inputTab.Controls.Add(this.dgInput);
            this.inputTab.Location = new System.Drawing.Point(4, 22);
            this.inputTab.Name = "inputTab";
            this.inputTab.Padding = new System.Windows.Forms.Padding(3);
            this.inputTab.Size = new System.Drawing.Size(754, 567);
            this.inputTab.TabIndex = 0;
            this.inputTab.Text = "Input";
            this.inputTab.UseVisualStyleBackColor = true;
            // 
            // flameSpeedSelector
            // 
            this.flameSpeedSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.flameSpeedSelector.FormattingEnabled = true;
            this.flameSpeedSelector.Location = new System.Drawing.Point(193, 87);
            this.flameSpeedSelector.Name = "flameSpeedSelector";
            this.flameSpeedSelector.Size = new System.Drawing.Size(257, 21);
            this.flameSpeedSelector.TabIndex = 64;
            this.flameSpeedSelector.SelectionChangeCommitted += new System.EventHandler(this.flameSpeedSelector_SelectionChangeCommitted);
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
            // methodSelector
            // 
            this.methodSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.methodSelector.FormattingEnabled = true;
            this.methodSelector.Location = new System.Drawing.Point(193, 6);
            this.methodSelector.Name = "methodSelector";
            this.methodSelector.Size = new System.Drawing.Size(257, 21);
            this.methodSelector.TabIndex = 62;
            this.methodSelector.SelectionChangeCommitted += new System.EventHandler(this.methodSelector_SelectionChangeCommitted);
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
            this.inputWarning.Location = new System.Drawing.Point(13, 535);
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
            this.fuelPhaseSelector.Location = new System.Drawing.Point(193, 60);
            this.fuelPhaseSelector.Name = "fuelPhaseSelector";
            this.fuelPhaseSelector.Size = new System.Drawing.Size(257, 21);
            this.fuelPhaseSelector.TabIndex = 58;
            this.fuelPhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.fuelPhaseSelector_SelectionChangeCommitted);
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
            // notionalNozzleSelector
            // 
            this.notionalNozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.notionalNozzleSelector.FormattingEnabled = true;
            this.notionalNozzleSelector.Location = new System.Drawing.Point(193, 33);
            this.notionalNozzleSelector.Name = "notionalNozzleSelector";
            this.notionalNozzleSelector.Size = new System.Drawing.Size(257, 21);
            this.notionalNozzleSelector.TabIndex = 22;
            this.notionalNozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.notionalNozzleSelector_SelectionChangeCommitted);
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
            this.spinnerPictureBox.Location = new System.Drawing.Point(350, 509);
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
            this.lblZElementCount.Location = new System.Drawing.Point(456, 488);
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
            this.lblYElemCount.Location = new System.Drawing.Point(456, 462);
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
            this.lblXElemCount.Location = new System.Drawing.Point(456, 438);
            this.lblXElemCount.Name = "lblXElemCount";
            this.lblXElemCount.Size = new System.Drawing.Size(103, 13);
            this.lblXElemCount.TabIndex = 11;
            this.lblXElemCount.Text = "Array Element Count";
            // 
            // zLocsInput
            // 
            this.zLocsInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.zLocsInput.Location = new System.Drawing.Point(175, 486);
            this.zLocsInput.Name = "zLocsInput";
            this.zLocsInput.Size = new System.Drawing.Size(275, 20);
            this.zLocsInput.TabIndex = 8;
            this.zLocsInput.TextChanged += new System.EventHandler(this.zLocs_TextChanged);
            // 
            // lblHeatFluxPointsZ
            // 
            this.lblHeatFluxPointsZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsZ.AutoSize = true;
            this.lblHeatFluxPointsZ.Location = new System.Drawing.Point(13, 489);
            this.lblHeatFluxPointsZ.Name = "lblHeatFluxPointsZ";
            this.lblHeatFluxPointsZ.Size = new System.Drawing.Size(78, 13);
            this.lblHeatFluxPointsZ.TabIndex = 7;
            this.lblHeatFluxPointsZ.Text = "Z positions (m):";
            // 
            // yLocsInput
            // 
            this.yLocsInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.yLocsInput.Location = new System.Drawing.Point(175, 460);
            this.yLocsInput.Name = "yLocsInput";
            this.yLocsInput.Size = new System.Drawing.Size(275, 20);
            this.yLocsInput.TabIndex = 6;
            this.yLocsInput.TextChanged += new System.EventHandler(this.yLocs_TextChanged);
            // 
            // lblHeadFluxPointsY
            // 
            this.lblHeadFluxPointsY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeadFluxPointsY.AutoSize = true;
            this.lblHeadFluxPointsY.Location = new System.Drawing.Point(13, 463);
            this.lblHeadFluxPointsY.Name = "lblHeadFluxPointsY";
            this.lblHeadFluxPointsY.Size = new System.Drawing.Size(78, 13);
            this.lblHeadFluxPointsY.TabIndex = 5;
            this.lblHeadFluxPointsY.Text = "Y positions (m):";
            // 
            // xLocsInput
            // 
            this.xLocsInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.xLocsInput.Location = new System.Drawing.Point(175, 434);
            this.xLocsInput.Name = "xLocsInput";
            this.xLocsInput.Size = new System.Drawing.Size(275, 20);
            this.xLocsInput.TabIndex = 4;
            this.xLocsInput.TextChanged += new System.EventHandler(this.xLocs_TextChanged);
            // 
            // lblHeatFluxPointsX
            // 
            this.lblHeatFluxPointsX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsX.AutoSize = true;
            this.lblHeatFluxPointsX.Location = new System.Drawing.Point(13, 437);
            this.lblHeatFluxPointsX.Name = "lblHeatFluxPointsX";
            this.lblHeatFluxPointsX.Size = new System.Drawing.Size(78, 13);
            this.lblHeatFluxPointsX.TabIndex = 3;
            this.lblHeatFluxPointsX.Text = "X positions (m):";
            // 
            // executeButton
            // 
            this.executeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.executeButton.Location = new System.Drawing.Point(379, 509);
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
            this.dgInput.Location = new System.Drawing.Point(6, 114);
            this.dgInput.Name = "dgInput";
            this.dgInput.Size = new System.Drawing.Size(444, 314);
            this.dgInput.TabIndex = 0;
            this.dgInput.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgInput_CellValueChanged);
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.splitContainer1);
            this.outputTab.Location = new System.Drawing.Point(4, 22);
            this.outputTab.Name = "outputTab";
            this.outputTab.Padding = new System.Windows.Forms.Padding(3);
            this.outputTab.Size = new System.Drawing.Size(754, 567);
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
            this.splitContainer1.Size = new System.Drawing.Size(748, 561);
            this.splitContainer1.SplitterDistance = 523;
            this.splitContainer1.TabIndex = 6;
            // 
            // tcOutputs
            // 
            this.tcOutputs.Controls.Add(this.dataTab);
            this.tcOutputs.Controls.Add(this.plotTab);
            this.tcOutputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcOutputs.Location = new System.Drawing.Point(0, 0);
            this.tcOutputs.Name = "tcOutputs";
            this.tcOutputs.SelectedIndex = 0;
            this.tcOutputs.Size = new System.Drawing.Size(748, 523);
            this.tcOutputs.TabIndex = 5;
            // 
            // dataTab
            // 
            this.dataTab.Controls.Add(this.dgResult);
            this.dataTab.Controls.Add(this.btnCopyToClipboard);
            this.dataTab.Location = new System.Drawing.Point(4, 22);
            this.dataTab.Name = "dataTab";
            this.dataTab.Padding = new System.Windows.Forms.Padding(3);
            this.dataTab.Size = new System.Drawing.Size(740, 497);
            this.dataTab.TabIndex = 0;
            this.dataTab.Text = "Values";
            this.dataTab.UseVisualStyleBackColor = true;
            // 
            // dgResult
            // 
            this.dgResult.AllowUserToAddRows = false;
            this.dgResult.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgResult.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgResult.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgResult.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colX,
            this.colY,
            this.colZ,
            this.overpressureCol,
            this.impulseCol});
            this.dgResult.Location = new System.Drawing.Point(3, 3);
            this.dgResult.Name = "dgResult";
            this.dgResult.Size = new System.Drawing.Size(732, 458);
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
            // overpressureCol
            // 
            this.overpressureCol.HeaderText = "Overpressure (Pa)";
            this.overpressureCol.Name = "overpressureCol";
            this.overpressureCol.ReadOnly = true;
            // 
            // impulseCol
            // 
            this.impulseCol.HeaderText = "Impulse (Pa*s)";
            this.impulseCol.Name = "impulseCol";
            this.impulseCol.ReadOnly = true;
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyToClipboard.Location = new System.Drawing.Point(615, 467);
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(119, 23);
            this.btnCopyToClipboard.TabIndex = 2;
            this.btnCopyToClipboard.Text = "Copy to Clipboard";
            this.btnCopyToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // plotTab
            // 
            this.plotTab.Controls.Add(this.plotBox);
            this.plotTab.Location = new System.Drawing.Point(4, 22);
            this.plotTab.Name = "plotTab";
            this.plotTab.Padding = new System.Windows.Forms.Padding(3);
            this.plotTab.Size = new System.Drawing.Size(740, 497);
            this.plotTab.TabIndex = 1;
            this.plotTab.Text = "Overpressure plot";
            this.plotTab.UseVisualStyleBackColor = true;
            // 
            // plotBox
            // 
            this.plotBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.plotBox.Location = new System.Drawing.Point(3, 3);
            this.plotBox.Name = "plotBox";
            this.plotBox.Size = new System.Drawing.Size(734, 491);
            this.plotBox.TabIndex = 4;
            this.plotBox.TabStop = false;
            // 
            // outputWarning
            // 
            this.outputWarning.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.outputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWarning.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.outputWarning.Location = new System.Drawing.Point(0, 0);
            this.outputWarning.Name = "outputWarning";
            this.outputWarning.Size = new System.Drawing.Size(748, 34);
            this.outputWarning.TabIndex = 19;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
            // 
            // UnconfinedOverpressureForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcIO);
            this.Name = "UnconfinedOverpressureForm";
            this.Size = new System.Drawing.Size(762, 593);
            this.Load += new System.EventHandler(this.UnconfinedOverpressureForm_Load);
            this.tcIO.ResumeLayout(false);
            this.inputTab.ResumeLayout(false);
            this.inputTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.spinnerPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).EndInit();
            this.outputTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tcOutputs.ResumeLayout(false);
            this.dataTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
            this.plotTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.plotBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tcIO;
        private TabPage inputTab;
        private TabPage outputTab;
        private DataGridView dgInput;

        private Button executeButton;
        private Label lblHeatFluxPointsX;
        private TextBox zLocsInput;
        private Label lblHeatFluxPointsZ;
        private TextBox yLocsInput;
        private Label lblHeadFluxPointsY;
        private TextBox xLocsInput;
        private Button btnCopyToClipboard;
        private DataGridView dgResult;
        private Label lblZElementCount;
        private Label lblYElemCount;
        private Label lblXElemCount;
        private PictureBoxWithSave plotBox;
        private TabControl tcOutputs;
        private TabPage dataTab;
        private TabPage plotTab;
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
            char[] delimiters = { delimiter };
            var values = delimitedString.Split(delimiters);
            var result = new double[values.Length];
            for (var index = 0; index < result.Length; index++)
            {
                result[index] = double.NaN;

                if (ParseUtility.TryParseDouble(values[index], out double parsedValue)) result[index] = parsedValue;
            }

            return result;
        }
        private ComboBox methodSelector;
        private Label label3;
        private ComboBox flameSpeedSelector;
        private Label flameSpeedLabel;
        private DataGridViewTextBoxColumn colX;
        private DataGridViewTextBoxColumn colY;
        private DataGridViewTextBoxColumn colZ;
        private DataGridViewTextBoxColumn overpressureCol;
        private DataGridViewTextBoxColumn impulseCol;
    }
}
