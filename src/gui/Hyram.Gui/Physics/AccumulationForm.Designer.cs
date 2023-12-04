using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram {
	partial class AccumulationForm {

		#region Component Designer generated code
        private void InitializeComponent() {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.IOTabs = new System.Windows.Forms.TabControl();
            this.InputTab = new System.Windows.Forms.TabPage();
            this.OverpressureSpinner = new System.Windows.Forms.PictureBox();
            this.InputWarning = new System.Windows.Forms.Label();
            this.InputTabs = new System.Windows.Forms.TabControl();
            this.Inputs = new System.Windows.Forms.TabPage();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.ReleaseSteadyBtn = new System.Windows.Forms.RadioButton();
            this.ReleaseBlowdownBtn = new System.Windows.Forms.RadioButton();
            this.AutoSetLimits = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.GeometryPicture = new System.Windows.Forms.PictureBox();
            this.Outputs = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.TimeUnitSelector = new System.Windows.Forms.ComboBox();
            this.MaxTimeInput = new System.Windows.Forms.TextBox();
            this.MaxTimeLabel = new System.Windows.Forms.Label();
            this.PlottingOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.PressuresPerTimeCheckbox = new System.Windows.Forms.CheckBox();
            this.PressuresPerTimeGroupBox = new System.Windows.Forms.GroupBox();
            this.PressuresPerTimeGrid = new System.Windows.Forms.DataGridView();
            this.MarkTimeGridCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MarkPressureGridCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PressureLinesGroupBox = new System.Windows.Forms.GroupBox();
            this.PressureLinesGrid = new System.Windows.Forms.DataGridView();
            this.PressureLineCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PressureLinesCheckbox = new System.Windows.Forms.CheckBox();
            this.PlotTimesInput = new System.Windows.Forms.TextBox();
            this.PlotTimesLabel = new System.Windows.Forms.Label();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tcOutput = new System.Windows.Forms.TabControl();
            this.tpPressure = new System.Windows.Forms.TabPage();
            this.PressurePlot = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tpFlammableMass = new System.Windows.Forms.TabPage();
            this.MassPlot = new System.Windows.Forms.PictureBox();
            this.tpLayer = new System.Windows.Forms.TabPage();
            this.LayerPlot = new System.Windows.Forms.PictureBox();
            this.tpTrajectory = new System.Windows.Forms.TabPage();
            this.TrajectoryPlot = new System.Windows.Forms.PictureBox();
            this.tpMassFlow = new System.Windows.Forms.TabPage();
            this.MassFlowPlot = new System.Windows.Forms.PictureBox();
            this.tpData = new System.Windows.Forms.TabPage();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.tbMaxPressure = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.overpressureResultGrid = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDepth2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConcentration2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.massFlowRate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.outputWarning = new System.Windows.Forms.Label();
            this.IOTabs.SuspendLayout();
            this.InputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverpressureSpinner)).BeginInit();
            this.InputTabs.SuspendLayout();
            this.Inputs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeometryPicture)).BeginInit();
            this.Outputs.SuspendLayout();
            this.PlottingOptionsGroupBox.SuspendLayout();
            this.PressuresPerTimeGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PressuresPerTimeGrid)).BeginInit();
            this.PressureLinesGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PressureLinesGrid)).BeginInit();
            this.outputTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tcOutput.SuspendLayout();
            this.tpPressure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PressurePlot)).BeginInit();
            this.tpFlammableMass.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MassPlot)).BeginInit();
            this.tpLayer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.LayerPlot)).BeginInit();
            this.tpTrajectory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrajectoryPlot)).BeginInit();
            this.tpMassFlow.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MassFlowPlot)).BeginInit();
            this.tpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overpressureResultGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // IOTabs
            // 
            this.IOTabs.Controls.Add(this.InputTab);
            this.IOTabs.Controls.Add(this.outputTab);
            this.IOTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IOTabs.Location = new System.Drawing.Point(0, 0);
            this.IOTabs.Multiline = true;
            this.IOTabs.Name = "IOTabs";
            this.IOTabs.SelectedIndex = 0;
            this.IOTabs.Size = new System.Drawing.Size(1151, 594);
            this.IOTabs.TabIndex = 0;
            // 
            // InputTab
            // 
            this.InputTab.Controls.Add(this.OverpressureSpinner);
            this.InputTab.Controls.Add(this.InputWarning);
            this.InputTab.Controls.Add(this.InputTabs);
            this.InputTab.Controls.Add(this.SubmitBtn);
            this.InputTab.Location = new System.Drawing.Point(4, 22);
            this.InputTab.Name = "InputTab";
            this.InputTab.Padding = new System.Windows.Forms.Padding(3);
            this.InputTab.Size = new System.Drawing.Size(1143, 568);
            this.InputTab.TabIndex = 0;
            this.InputTab.Text = "Input";
            this.InputTab.UseVisualStyleBackColor = true;
            // 
            // OverpressureSpinner
            // 
            this.OverpressureSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OverpressureSpinner.Image = global::SandiaNationalLaboratories.Hyram.Properties.Resources.AjaxSpinner;
            this.OverpressureSpinner.Location = new System.Drawing.Point(302, 518);
            this.OverpressureSpinner.MinimumSize = new System.Drawing.Size(20, 20);
            this.OverpressureSpinner.Name = "OverpressureSpinner";
            this.OverpressureSpinner.Size = new System.Drawing.Size(32, 23);
            this.OverpressureSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.OverpressureSpinner.TabIndex = 18;
            this.OverpressureSpinner.TabStop = false;
            // 
            // InputWarning
            // 
            this.InputWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.InputWarning.AutoSize = true;
            this.InputWarning.BackColor = System.Drawing.Color.MistyRose;
            this.InputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InputWarning.ForeColor = System.Drawing.Color.Maroon;
            this.InputWarning.Location = new System.Drawing.Point(3, 542);
            this.InputWarning.Name = "InputWarning";
            this.InputWarning.Padding = new System.Windows.Forms.Padding(4);
            this.InputWarning.Size = new System.Drawing.Size(384, 23);
            this.InputWarning.TabIndex = 17;
            this.InputWarning.Text = "Test warning notification area with long warning message";
            this.InputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.InputWarning.Visible = false;
            // 
            // InputTabs
            // 
            this.InputTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputTabs.Controls.Add(this.Inputs);
            this.InputTabs.Controls.Add(this.Outputs);
            this.InputTabs.Location = new System.Drawing.Point(3, 6);
            this.InputTabs.Name = "InputTabs";
            this.InputTabs.SelectedIndex = 0;
            this.InputTabs.Size = new System.Drawing.Size(1137, 506);
            this.InputTabs.TabIndex = 1;
            // 
            // Inputs
            // 
            this.Inputs.AutoScroll = true;
            this.Inputs.Controls.Add(this.splitContainer2);
            this.Inputs.Location = new System.Drawing.Point(4, 22);
            this.Inputs.Name = "Inputs";
            this.Inputs.Padding = new System.Windows.Forms.Padding(3);
            this.Inputs.Size = new System.Drawing.Size(1129, 480);
            this.Inputs.TabIndex = 0;
            this.Inputs.Text = "Parameters";
            this.Inputs.UseVisualStyleBackColor = true;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(3, 3);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.ReleaseSteadyBtn);
            this.splitContainer2.Panel1.Controls.Add(this.ReleaseBlowdownBtn);
            this.splitContainer2.Panel1.Controls.Add(this.AutoSetLimits);
            this.splitContainer2.Panel1.Controls.Add(this.label4);
            this.splitContainer2.Panel1.Controls.Add(this.InputGrid);
            this.splitContainer2.Panel1MinSize = 250;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.GeometryPicture);
            this.splitContainer2.Size = new System.Drawing.Size(1123, 474);
            this.splitContainer2.SplitterDistance = 405;
            this.splitContainer2.TabIndex = 2;
            // 
            // ReleaseSteadyBtn
            // 
            this.ReleaseSteadyBtn.AutoSize = true;
            this.ReleaseSteadyBtn.Location = new System.Drawing.Point(173, 13);
            this.ReleaseSteadyBtn.Name = "ReleaseSteadyBtn";
            this.ReleaseSteadyBtn.Size = new System.Drawing.Size(58, 17);
            this.ReleaseSteadyBtn.TabIndex = 2;
            this.ReleaseSteadyBtn.TabStop = true;
            this.ReleaseSteadyBtn.Text = "Steady";
            this.ReleaseSteadyBtn.UseVisualStyleBackColor = true;
            // 
            // ReleaseBlowdownBtn
            // 
            this.ReleaseBlowdownBtn.AutoSize = true;
            this.ReleaseBlowdownBtn.Checked = true;
            this.ReleaseBlowdownBtn.Location = new System.Drawing.Point(93, 13);
            this.ReleaseBlowdownBtn.Name = "ReleaseBlowdownBtn";
            this.ReleaseBlowdownBtn.Size = new System.Drawing.Size(74, 17);
            this.ReleaseBlowdownBtn.TabIndex = 1;
            this.ReleaseBlowdownBtn.TabStop = true;
            this.ReleaseBlowdownBtn.Text = "Blowdown";
            this.ReleaseBlowdownBtn.UseVisualStyleBackColor = true;
            // 
            // AutoSetLimits
            // 
            this.AutoSetLimits.AutoSize = true;
            this.AutoSetLimits.Location = new System.Drawing.Point(237, 17);
            this.AutoSetLimits.Name = "AutoSetLimits";
            this.AutoSetLimits.Size = new System.Drawing.Size(171, 17);
            this.AutoSetLimits.TabIndex = 73;
            this.AutoSetLimits.Text = "Automatically set plot axis limits";
            this.AutoSetLimits.UseVisualStyleBackColor = true;
            this.AutoSetLimits.Visible = false;
            this.AutoSetLimits.CheckedChanged += new System.EventHandler(this.AutoSetLimits_CheckedChanged);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(4, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(100, 15);
            this.label4.TabIndex = 61;
            this.label4.Text = "Release type";
            // 
            // InputGrid
            // 
            this.InputGrid.AllowUserToAddRows = false;
            this.InputGrid.AllowUserToDeleteRows = false;
            this.InputGrid.AllowUserToResizeRows = false;
            this.InputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.InputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGrid.Location = new System.Drawing.Point(0, 53);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.Size = new System.Drawing.Size(405, 418);
            this.InputGrid.TabIndex = 3;
            this.InputGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.InputGrid_CellValidating);
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputGrid_CellValueChanged);
            // 
            // GeometryPicture
            // 
            this.GeometryPicture.BackgroundImage = global::SandiaNationalLaboratories.Hyram.Properties.Resources.geometry_of_indoor_release;
            this.GeometryPicture.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.GeometryPicture.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GeometryPicture.InitialImage = global::SandiaNationalLaboratories.Hyram.Properties.Resources.geometry_of_indoor_release;
            this.GeometryPicture.Location = new System.Drawing.Point(0, 0);
            this.GeometryPicture.Name = "GeometryPicture";
            this.GeometryPicture.Size = new System.Drawing.Size(714, 474);
            this.GeometryPicture.TabIndex = 2;
            this.GeometryPicture.TabStop = false;
            // 
            // Outputs
            // 
            this.Outputs.Controls.Add(this.label5);
            this.Outputs.Controls.Add(this.TimeUnitSelector);
            this.Outputs.Controls.Add(this.MaxTimeInput);
            this.Outputs.Controls.Add(this.MaxTimeLabel);
            this.Outputs.Controls.Add(this.PlottingOptionsGroupBox);
            this.Outputs.Controls.Add(this.PlotTimesInput);
            this.Outputs.Controls.Add(this.PlotTimesLabel);
            this.Outputs.Location = new System.Drawing.Point(4, 22);
            this.Outputs.Name = "Outputs";
            this.Outputs.Padding = new System.Windows.Forms.Padding(3);
            this.Outputs.Size = new System.Drawing.Size(1129, 480);
            this.Outputs.TabIndex = 1;
            this.Outputs.Text = "Output Options";
            this.Outputs.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Units of time";
            // 
            // TimeUnitSelector
            // 
            this.TimeUnitSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.TimeUnitSelector.FormattingEnabled = true;
            this.TimeUnitSelector.Location = new System.Drawing.Point(175, 9);
            this.TimeUnitSelector.Name = "TimeUnitSelector";
            this.TimeUnitSelector.Size = new System.Drawing.Size(100, 21);
            this.TimeUnitSelector.TabIndex = 1;
            this.TimeUnitSelector.SelectionChangeCommitted += new System.EventHandler(this.TimeUnitSelector_SelectionChangeCommitted);
            // 
            // MaxTimeInput
            // 
            this.MaxTimeInput.Location = new System.Drawing.Point(175, 62);
            this.MaxTimeInput.Name = "MaxTimeInput";
            this.MaxTimeInput.Size = new System.Drawing.Size(100, 20);
            this.MaxTimeInput.TabIndex = 3;
            this.MaxTimeInput.TextChanged += new System.EventHandler(this.MaxTimeInput_TextChanged);
            // 
            // MaxTimeLabel
            // 
            this.MaxTimeLabel.AutoSize = true;
            this.MaxTimeLabel.Location = new System.Drawing.Point(7, 65);
            this.MaxTimeLabel.Name = "MaxTimeLabel";
            this.MaxTimeLabel.Size = new System.Drawing.Size(76, 13);
            this.MaxTimeLabel.TabIndex = 5;
            this.MaxTimeLabel.Text = "Maximum time:";
            // 
            // PlottingOptionsGroupBox
            // 
            this.PlottingOptionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlottingOptionsGroupBox.Controls.Add(this.PressuresPerTimeCheckbox);
            this.PlottingOptionsGroupBox.Controls.Add(this.PressuresPerTimeGroupBox);
            this.PlottingOptionsGroupBox.Controls.Add(this.PressureLinesGroupBox);
            this.PlottingOptionsGroupBox.Controls.Add(this.PressureLinesCheckbox);
            this.PlottingOptionsGroupBox.Location = new System.Drawing.Point(6, 104);
            this.PlottingOptionsGroupBox.Name = "PlottingOptionsGroupBox";
            this.PlottingOptionsGroupBox.Size = new System.Drawing.Size(1117, 373);
            this.PlottingOptionsGroupBox.TabIndex = 4;
            this.PlottingOptionsGroupBox.TabStop = false;
            this.PlottingOptionsGroupBox.Text = "Plotting options";
            // 
            // PressuresPerTimeCheckbox
            // 
            this.PressuresPerTimeCheckbox.AutoSize = true;
            this.PressuresPerTimeCheckbox.Checked = true;
            this.PressuresPerTimeCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PressuresPerTimeCheckbox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.PressuresPerTimeCheckbox.Location = new System.Drawing.Point(302, 16);
            this.PressuresPerTimeCheckbox.Name = "PressuresPerTimeCheckbox";
            this.PressuresPerTimeCheckbox.Size = new System.Drawing.Size(186, 17);
            this.PressuresPerTimeCheckbox.TabIndex = 6;
            this.PressuresPerTimeCheckbox.Text = "Mark chart with pressures at times";
            this.PressuresPerTimeCheckbox.UseVisualStyleBackColor = true;
            this.PressuresPerTimeCheckbox.CheckedChanged += new System.EventHandler(this.PressuresPerTimeCheckbox_CheckedChanged);
            // 
            // PressuresPerTimeGroupBox
            // 
            this.PressuresPerTimeGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.PressuresPerTimeGroupBox.Controls.Add(this.PressuresPerTimeGrid);
            this.PressuresPerTimeGroupBox.Location = new System.Drawing.Point(299, 40);
            this.PressuresPerTimeGroupBox.Name = "PressuresPerTimeGroupBox";
            this.PressuresPerTimeGroupBox.Size = new System.Drawing.Size(316, 220);
            this.PressuresPerTimeGroupBox.TabIndex = 9;
            this.PressuresPerTimeGroupBox.TabStop = false;
            this.PressuresPerTimeGroupBox.Text = "Place dots where pressure/time intersect";
            // 
            // PressuresPerTimeGrid
            // 
            this.PressuresPerTimeGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.PressuresPerTimeGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.PressuresPerTimeGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PressuresPerTimeGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.MarkTimeGridCol,
            this.MarkPressureGridCol});
            this.PressuresPerTimeGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PressuresPerTimeGrid.Location = new System.Drawing.Point(3, 16);
            this.PressuresPerTimeGrid.Name = "PressuresPerTimeGrid";
            this.PressuresPerTimeGrid.RowHeadersVisible = false;
            this.PressuresPerTimeGrid.Size = new System.Drawing.Size(310, 201);
            this.PressuresPerTimeGrid.TabIndex = 7;
            this.PressuresPerTimeGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.PressuresPerTimeGrid_CellValueChanged);
            this.PressuresPerTimeGrid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.PressuresPerTimeGrid_RowsRemoved);
            this.PressuresPerTimeGrid.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.PressuresPerTimeGrid_SortCompare);
            // 
            // MarkTimeGridCol
            // 
            this.MarkTimeGridCol.HeaderText = "Time";
            this.MarkTimeGridCol.Name = "MarkTimeGridCol";
            // 
            // MarkPressureGridCol
            // 
            this.MarkPressureGridCol.HeaderText = "Pressure (kPa)";
            this.MarkPressureGridCol.Name = "MarkPressureGridCol";
            // 
            // PressureLinesGroupBox
            // 
            this.PressureLinesGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.PressureLinesGroupBox.AutoSize = true;
            this.PressureLinesGroupBox.Controls.Add(this.PressureLinesGrid);
            this.PressureLinesGroupBox.Location = new System.Drawing.Point(9, 37);
            this.PressureLinesGroupBox.Name = "PressureLinesGroupBox";
            this.PressureLinesGroupBox.Size = new System.Drawing.Size(225, 223);
            this.PressureLinesGroupBox.TabIndex = 8;
            this.PressureLinesGroupBox.TabStop = false;
            this.PressureLinesGroupBox.Text = "Specify pressures in kPa";
            // 
            // PressureLinesGrid
            // 
            this.PressureLinesGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.PressureLinesGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.PressureLinesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.PressureLinesGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PressureLineCol});
            this.PressureLinesGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PressureLinesGrid.Location = new System.Drawing.Point(3, 16);
            this.PressureLinesGrid.Name = "PressureLinesGrid";
            this.PressureLinesGrid.RowHeadersVisible = false;
            this.PressureLinesGrid.Size = new System.Drawing.Size(219, 204);
            this.PressureLinesGrid.TabIndex = 5;
            this.PressureLinesGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.PressureLinesGrid_CellValueChanged);
            this.PressureLinesGrid.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.PressureLinesGrid_RowsRemoved);
            this.PressureLinesGrid.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.PressureLinesGrid_SortCompare);
            // 
            // PressureLineCol
            // 
            this.PressureLineCol.HeaderText = "Pressure (kPa)";
            this.PressureLineCol.Name = "PressureLineCol";
            // 
            // PressureLinesCheckbox
            // 
            this.PressureLinesCheckbox.AutoSize = true;
            this.PressureLinesCheckbox.Checked = true;
            this.PressureLinesCheckbox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.PressureLinesCheckbox.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.PressureLinesCheckbox.Location = new System.Drawing.Point(10, 16);
            this.PressureLinesCheckbox.Name = "PressureLinesCheckbox";
            this.PressureLinesCheckbox.Size = new System.Drawing.Size(270, 17);
            this.PressureLinesCheckbox.TabIndex = 4;
            this.PressureLinesCheckbox.Text = "Draw horizontal lines on chart at specified pressures";
            this.PressureLinesCheckbox.UseVisualStyleBackColor = true;
            this.PressureLinesCheckbox.CheckedChanged += new System.EventHandler(this.PressureLinesCheckbox_CheckedChanged);
            // 
            // PlotTimesInput
            // 
            this.PlotTimesInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PlotTimesInput.Location = new System.Drawing.Point(175, 36);
            this.PlotTimesInput.Name = "PlotTimesInput";
            this.PlotTimesInput.Size = new System.Drawing.Size(743, 20);
            this.PlotTimesInput.TabIndex = 2;
            this.PlotTimesInput.TextChanged += new System.EventHandler(this.PlotTimesInput_TextChanged);
            // 
            // PlotTimesLabel
            // 
            this.PlotTimesLabel.AutoSize = true;
            this.PlotTimesLabel.Location = new System.Drawing.Point(7, 39);
            this.PlotTimesLabel.Name = "PlotTimesLabel";
            this.PlotTimesLabel.Size = new System.Drawing.Size(158, 13);
            this.PlotTimesLabel.TabIndex = 0;
            this.PlotTimesLabel.Text = "Output pressures at these times:";
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.SubmitBtn.Location = new System.Drawing.Point(340, 518);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 23);
            this.SubmitBtn.TabIndex = 4;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.splitContainer1);
            this.outputTab.Location = new System.Drawing.Point(4, 22);
            this.outputTab.Name = "outputTab";
            this.outputTab.Padding = new System.Windows.Forms.Padding(3);
            this.outputTab.Size = new System.Drawing.Size(1143, 568);
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
            this.splitContainer1.Panel1.Controls.Add(this.tcOutput);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputWarning);
            this.splitContainer1.Size = new System.Drawing.Size(1137, 562);
            this.splitContainer1.SplitterDistance = 532;
            this.splitContainer1.TabIndex = 19;
            // 
            // tcOutput
            // 
            this.tcOutput.Controls.Add(this.tpPressure);
            this.tcOutput.Controls.Add(this.tpFlammableMass);
            this.tcOutput.Controls.Add(this.tpLayer);
            this.tcOutput.Controls.Add(this.tpTrajectory);
            this.tcOutput.Controls.Add(this.tpMassFlow);
            this.tcOutput.Controls.Add(this.tpData);
            this.tcOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcOutput.Location = new System.Drawing.Point(0, 0);
            this.tcOutput.Name = "tcOutput";
            this.tcOutput.SelectedIndex = 0;
            this.tcOutput.Size = new System.Drawing.Size(1137, 532);
            this.tcOutput.TabIndex = 3;
            // 
            // tpPressure
            // 
            this.tpPressure.Controls.Add(this.PressurePlot);
            this.tpPressure.Controls.Add(this.label2);
            this.tpPressure.Location = new System.Drawing.Point(4, 22);
            this.tpPressure.Name = "tpPressure";
            this.tpPressure.Padding = new System.Windows.Forms.Padding(3);
            this.tpPressure.Size = new System.Drawing.Size(1129, 506);
            this.tpPressure.TabIndex = 0;
            this.tpPressure.Text = "Pressure plot";
            this.tpPressure.UseVisualStyleBackColor = true;
            // 
            // PressurePlot
            // 
            this.PressurePlot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PressurePlot.Location = new System.Drawing.Point(3, 30);
            this.PressurePlot.Name = "PressurePlot";
            this.PressurePlot.Size = new System.Drawing.Size(1126, 476);
            this.PressurePlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PressurePlot.TabIndex = 2;
            this.PressurePlot.TabStop = false;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(3, 3);
            this.label2.Name = "label2";
            this.label2.Padding = new System.Windows.Forms.Padding(3);
            this.label2.Size = new System.Drawing.Size(1123, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "Time-history of the overpressure that would develop if the accumulated fuel were " +
    "to be ignited after some delay after the leak started";
            // 
            // tpFlammableMass
            // 
            this.tpFlammableMass.AutoScroll = true;
            this.tpFlammableMass.Controls.Add(this.MassPlot);
            this.tpFlammableMass.Location = new System.Drawing.Point(4, 22);
            this.tpFlammableMass.Name = "tpFlammableMass";
            this.tpFlammableMass.Size = new System.Drawing.Size(1129, 506);
            this.tpFlammableMass.TabIndex = 4;
            this.tpFlammableMass.Text = "Flammable mass plot";
            this.tpFlammableMass.UseVisualStyleBackColor = true;
            // 
            // MassPlot
            // 
            this.MassPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MassPlot.Location = new System.Drawing.Point(0, 0);
            this.MassPlot.Name = "MassPlot";
            this.MassPlot.Size = new System.Drawing.Size(1129, 506);
            this.MassPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MassPlot.TabIndex = 2;
            this.MassPlot.TabStop = false;
            // 
            // tpLayer
            // 
            this.tpLayer.AutoScroll = true;
            this.tpLayer.Controls.Add(this.LayerPlot);
            this.tpLayer.Location = new System.Drawing.Point(4, 22);
            this.tpLayer.Name = "tpLayer";
            this.tpLayer.Padding = new System.Windows.Forms.Padding(3);
            this.tpLayer.Size = new System.Drawing.Size(1129, 506);
            this.tpLayer.TabIndex = 1;
            this.tpLayer.Text = "Layer plot";
            this.tpLayer.UseVisualStyleBackColor = true;
            // 
            // LayerPlot
            // 
            this.LayerPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LayerPlot.Location = new System.Drawing.Point(3, 3);
            this.LayerPlot.Name = "LayerPlot";
            this.LayerPlot.Size = new System.Drawing.Size(1123, 500);
            this.LayerPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.LayerPlot.TabIndex = 3;
            this.LayerPlot.TabStop = false;
            // 
            // tpTrajectory
            // 
            this.tpTrajectory.AutoScroll = true;
            this.tpTrajectory.Controls.Add(this.TrajectoryPlot);
            this.tpTrajectory.Location = new System.Drawing.Point(4, 22);
            this.tpTrajectory.Name = "tpTrajectory";
            this.tpTrajectory.Size = new System.Drawing.Size(1129, 506);
            this.tpTrajectory.TabIndex = 3;
            this.tpTrajectory.Text = "Trajectory plot";
            this.tpTrajectory.UseVisualStyleBackColor = true;
            // 
            // TrajectoryPlot
            // 
            this.TrajectoryPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrajectoryPlot.Location = new System.Drawing.Point(0, 0);
            this.TrajectoryPlot.Name = "TrajectoryPlot";
            this.TrajectoryPlot.Size = new System.Drawing.Size(1129, 506);
            this.TrajectoryPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.TrajectoryPlot.TabIndex = 3;
            this.TrajectoryPlot.TabStop = false;
            // 
            // tpMassFlow
            // 
            this.tpMassFlow.AutoScroll = true;
            this.tpMassFlow.Controls.Add(this.MassFlowPlot);
            this.tpMassFlow.Location = new System.Drawing.Point(4, 22);
            this.tpMassFlow.Name = "tpMassFlow";
            this.tpMassFlow.Size = new System.Drawing.Size(1129, 506);
            this.tpMassFlow.TabIndex = 5;
            this.tpMassFlow.Text = "Mass flow plot";
            this.tpMassFlow.UseVisualStyleBackColor = true;
            // 
            // MassFlowPlot
            // 
            this.MassFlowPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MassFlowPlot.Location = new System.Drawing.Point(0, 0);
            this.MassFlowPlot.Name = "MassFlowPlot";
            this.MassFlowPlot.Size = new System.Drawing.Size(1129, 506);
            this.MassFlowPlot.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.MassFlowPlot.TabIndex = 3;
            this.MassFlowPlot.TabStop = false;
            // 
            // tpData
            // 
            this.tpData.Controls.Add(this.tbTime);
            this.tpData.Controls.Add(this.lblSeconds);
            this.tpData.Controls.Add(this.tbMaxPressure);
            this.tpData.Controls.Add(this.label1);
            this.tpData.Controls.Add(this.overpressureResultGrid);
            this.tpData.Location = new System.Drawing.Point(4, 22);
            this.tpData.Name = "tpData";
            this.tpData.Size = new System.Drawing.Size(1129, 506);
            this.tpData.TabIndex = 2;
            this.tpData.Text = "Data";
            this.tpData.UseVisualStyleBackColor = true;
            // 
            // tbTime
            // 
            this.tbTime.Location = new System.Drawing.Point(172, 38);
            this.tbTime.Name = "tbTime";
            this.tbTime.ReadOnly = true;
            this.tbTime.Size = new System.Drawing.Size(184, 20);
            this.tbTime.TabIndex = 6;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(16, 41);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(113, 13);
            this.lblSeconds.TabIndex = 5;
            this.lblSeconds.Text = "Time this Occurred (s):";
            // 
            // tbMaxPressure
            // 
            this.tbMaxPressure.Location = new System.Drawing.Point(172, 10);
            this.tbMaxPressure.Name = "tbMaxPressure";
            this.tbMaxPressure.ReadOnly = true;
            this.tbMaxPressure.Size = new System.Drawing.Size(184, 20);
            this.tbMaxPressure.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Maximum Overpressure (kPa):";
            // 
            // overpressureResultGrid
            // 
            this.overpressureResultGrid.AllowUserToAddRows = false;
            this.overpressureResultGrid.AllowUserToDeleteRows = false;
            this.overpressureResultGrid.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.overpressureResultGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.overpressureResultGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.overpressureResultGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.overpressureResultGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.colPressure,
            this.colDepth2,
            this.colConcentration2,
            this.massFlowRate});
            this.overpressureResultGrid.Location = new System.Drawing.Point(19, 73);
            this.overpressureResultGrid.Name = "overpressureResultGrid";
            this.overpressureResultGrid.RowHeadersVisible = false;
            this.overpressureResultGrid.Size = new System.Drawing.Size(678, 409);
            this.overpressureResultGrid.TabIndex = 2;
            // 
            // colTime
            // 
            this.colTime.HeaderText = "Time (Seconds)";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            // 
            // colPressure
            // 
            this.colPressure.HeaderText = "Combined Pressure (kPa)";
            this.colPressure.Name = "colPressure";
            this.colPressure.ReadOnly = true;
            // 
            // colDepth2
            // 
            this.colDepth2.HeaderText = "Depth (m)";
            this.colDepth2.Name = "colDepth2";
            this.colDepth2.ReadOnly = true;
            // 
            // colConcentration2
            // 
            this.colConcentration2.HeaderText = "Concentration (%)";
            this.colConcentration2.Name = "colConcentration2";
            this.colConcentration2.ReadOnly = true;
            // 
            // massFlowRate
            // 
            this.massFlowRate.HeaderText = "Mass Flow Rate (kg/s)";
            this.massFlowRate.Name = "massFlowRate";
            this.massFlowRate.ReadOnly = true;
            // 
            // outputWarning
            // 
            this.outputWarning.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.outputWarning.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputWarning.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.outputWarning.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.outputWarning.Location = new System.Drawing.Point(0, 0);
            this.outputWarning.Name = "outputWarning";
            this.outputWarning.Size = new System.Drawing.Size(1137, 26);
            this.outputWarning.TabIndex = 18;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
            // 
            // AccumulationForm
            // 
            this.Controls.Add(this.IOTabs);
            this.Name = "AccumulationForm";
            this.Size = new System.Drawing.Size(1151, 594);
            this.Load += new System.EventHandler(this.IndoorReleaseForm_Load);
            this.IOTabs.ResumeLayout(false);
            this.InputTab.ResumeLayout(false);
            this.InputTab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.OverpressureSpinner)).EndInit();
            this.InputTabs.ResumeLayout(false);
            this.Inputs.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.InputGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.GeometryPicture)).EndInit();
            this.Outputs.ResumeLayout(false);
            this.Outputs.PerformLayout();
            this.PlottingOptionsGroupBox.ResumeLayout(false);
            this.PlottingOptionsGroupBox.PerformLayout();
            this.PressuresPerTimeGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PressuresPerTimeGrid)).EndInit();
            this.PressureLinesGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PressureLinesGrid)).EndInit();
            this.outputTab.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tcOutput.ResumeLayout(false);
            this.tpPressure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PressurePlot)).EndInit();
            this.tpFlammableMass.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MassPlot)).EndInit();
            this.tpLayer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.LayerPlot)).EndInit();
            this.tpTrajectory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrajectoryPlot)).EndInit();
            this.tpMassFlow.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MassFlowPlot)).EndInit();
            this.tpData.ResumeLayout(false);
            this.tpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.overpressureResultGrid)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private TabControl IOTabs;
		private TabPage InputTab;
		private DataGridView InputGrid;
		private TabPage outputTab;
		private Button SubmitBtn;
		private TabControl InputTabs;
		private TabPage Inputs;
		private TabPage Outputs;
		private TextBox PlotTimesInput;
		private Label PlotTimesLabel;
		private GroupBox PlottingOptionsGroupBox;
		private DataGridView PressuresPerTimeGrid;
		private DataGridView PressureLinesGrid;
		private DataGridViewTextBoxColumn PressureLineCol;
		private DataGridView overpressureResultGrid;
		private CheckBox PressureLinesCheckbox;
		private GroupBox PressureLinesGroupBox;
		private GroupBox PressuresPerTimeGroupBox;
		private CheckBox PressuresPerTimeCheckbox;
		private TabControl tcOutput;
		private TabPage tpPressure;
		private TabPage tpLayer;
		private TabPage tpData;
		private TextBox tbTime;
		private Label lblSeconds;
		private TextBox tbMaxPressure;
		private Label label1;
		private Label InputWarning;
		private TextBox MaxTimeInput;
		private Label MaxTimeLabel;
		private TabPage tpTrajectory;
		private TabPage tpFlammableMass;
        private PictureBox OverpressureSpinner;
        private Label outputWarning;
        private SplitContainer splitContainer1;
        private Label label2;
        private TabPage tpMassFlow;
        private SplitContainer splitContainer2;
        private PictureBox GeometryPicture;
        private RadioButton ReleaseSteadyBtn;
        private RadioButton ReleaseBlowdownBtn;
        private Label label4;
        private ComboBox TimeUnitSelector;
        private Label label5;
        private DataGridViewTextBoxColumn MarkTimeGridCol;
        private DataGridViewTextBoxColumn MarkPressureGridCol;
        private DataGridViewTextBoxColumn colTime;
        private DataGridViewTextBoxColumn colPressure;
        private DataGridViewTextBoxColumn colDepth2;
        private DataGridViewTextBoxColumn colConcentration2;
        private DataGridViewTextBoxColumn massFlowRate;
        private CheckBox AutoSetLimits;
        private PictureBox PressurePlot;
        private PictureBox MassPlot;
        private PictureBox LayerPlot;
        private PictureBox TrajectoryPlot;
        private PictureBox MassFlowPlot;
    }
}
