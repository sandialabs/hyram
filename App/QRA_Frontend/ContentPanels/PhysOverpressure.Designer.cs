namespace QRA_Frontend.ContentPanels {
	partial class CpOverpressure {

		#region Component Designer generated code
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CpOverpressure));
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tpInput = new System.Windows.Forms.TabPage();
            this.pbOverpSpinner = new System.Windows.Forms.PictureBox();
            this.lblError = new System.Windows.Forms.Label();
            this.tcInputOptions = new System.Windows.Forms.TabControl();
            this.tpIndoorRelease = new System.Windows.Forms.TabPage();
            this.pbGeometry = new System.Windows.Forms.PictureBox();
            this.dgInput = new System.Windows.Forms.DataGridView();
            this.tpOutputOptions = new System.Windows.Forms.TabPage();
            this.tbMaxTime = new System.Windows.Forms.TextBox();
            this.lblMaxTime = new System.Windows.Forms.Label();
            this.gbPlottingOptions = new System.Windows.Forms.GroupBox();
            this.cbMarkChartWithPTDots = new System.Windows.Forms.CheckBox();
            this.gbTimePressureOptions = new System.Windows.Forms.GroupBox();
            this.dgTimePressureOptions = new System.Windows.Forms.DataGridView();
            this.colMarkTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMarkTimePressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbHorizontalLines = new System.Windows.Forms.GroupBox();
            this.dgHorzLinesSpecifier = new System.Windows.Forms.DataGridView();
            this.colHorzPressure = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cbHorizontalLines = new System.Windows.Forms.CheckBox();
            this.tbPlotTimes = new System.Windows.Forms.TextBox();
            this.lblPlotPressures = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.tcOutput = new System.Windows.Forms.TabControl();
            this.tpPressure = new System.Windows.Forms.TabPage();
            this.pbPressure = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.tpFlammableMass = new System.Windows.Forms.TabPage();
            this.pbFlammableMass = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.tpLayer = new System.Windows.Forms.TabPage();
            this.pbLayer = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.tpTrajectory = new System.Windows.Forms.TabPage();
            this.pbTrajectory = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.tpData = new System.Windows.Forms.TabPage();
            this.dgDepthAndConcentration = new System.Windows.Forms.DataGridView();
            this.colDepth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConcentration = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.lblSeconds = new System.Windows.Forms.Label();
            this.tbMaxPressure = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgPressures = new System.Windows.Forms.DataGridView();
            this.colTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tcMain.SuspendLayout();
            this.tpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOverpSpinner)).BeginInit();
            this.tcInputOptions.SuspendLayout();
            this.tpIndoorRelease.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbGeometry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).BeginInit();
            this.tpOutputOptions.SuspendLayout();
            this.gbPlottingOptions.SuspendLayout();
            this.gbTimePressureOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgTimePressureOptions)).BeginInit();
            this.gbHorizontalLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgHorzLinesSpecifier)).BeginInit();
            this.tpOutput.SuspendLayout();
            this.tcOutput.SuspendLayout();
            this.tpPressure.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPressure)).BeginInit();
            this.tpFlammableMass.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlammableMass)).BeginInit();
            this.tpLayer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLayer)).BeginInit();
            this.tpTrajectory.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTrajectory)).BeginInit();
            this.tpData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDepthAndConcentration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgPressures)).BeginInit();
            this.SuspendLayout();
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tpInput);
            this.tcMain.Controls.Add(this.tpOutput);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(0, 0);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(938, 583);
            this.tcMain.TabIndex = 0;
            // 
            // tpInput
            // 
            this.tpInput.Controls.Add(this.pbOverpSpinner);
            this.tpInput.Controls.Add(this.lblError);
            this.tpInput.Controls.Add(this.tcInputOptions);
            this.tpInput.Controls.Add(this.btnExecute);
            this.tpInput.Location = new System.Drawing.Point(4, 25);
            this.tpInput.Name = "tpInput";
            this.tpInput.Padding = new System.Windows.Forms.Padding(3);
            this.tpInput.Size = new System.Drawing.Size(930, 554);
            this.tpInput.TabIndex = 0;
            this.tpInput.Text = "Input";
            this.tpInput.UseVisualStyleBackColor = true;
            // 
            // pbOverpSpinner
            // 
            this.pbOverpSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbOverpSpinner.Image = global::QRA_Frontend.Properties.Resources.AjaxSpinner;
            this.pbOverpSpinner.Location = new System.Drawing.Point(810, 525);
            this.pbOverpSpinner.MinimumSize = new System.Drawing.Size(20, 20);
            this.pbOverpSpinner.Name = "pbOverpSpinner";
            this.pbOverpSpinner.Size = new System.Drawing.Size(32, 23);
            this.pbOverpSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbOverpSpinner.TabIndex = 18;
            this.pbOverpSpinner.TabStop = false;
            // 
            // lblError
            // 
            this.lblError.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(3, 525);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(801, 23);
            this.lblError.TabIndex = 17;
            this.lblError.Text = "No errors noted and this label is invisible";
            this.lblError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblError.Visible = false;
            // 
            // tcInputOptions
            // 
            this.tcInputOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tcInputOptions.Controls.Add(this.tpIndoorRelease);
            this.tcInputOptions.Controls.Add(this.tpOutputOptions);
            this.tcInputOptions.Location = new System.Drawing.Point(3, 3);
            this.tcInputOptions.Name = "tcInputOptions";
            this.tcInputOptions.SelectedIndex = 0;
            this.tcInputOptions.Size = new System.Drawing.Size(924, 516);
            this.tcInputOptions.TabIndex = 1;
            // 
            // tpIndoorRelease
            // 
            this.tpIndoorRelease.AutoScroll = true;
            this.tpIndoorRelease.Controls.Add(this.pbGeometry);
            this.tpIndoorRelease.Controls.Add(this.dgInput);
            this.tpIndoorRelease.Location = new System.Drawing.Point(4, 25);
            this.tpIndoorRelease.Name = "tpIndoorRelease";
            this.tpIndoorRelease.Padding = new System.Windows.Forms.Padding(3);
            this.tpIndoorRelease.Size = new System.Drawing.Size(916, 487);
            this.tpIndoorRelease.TabIndex = 0;
            this.tpIndoorRelease.Text = "Indoor Release Parameters";
            this.tpIndoorRelease.UseVisualStyleBackColor = true;
            // 
            // pbGeometry
            // 
            this.pbGeometry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbGeometry.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pbGeometry.BackgroundImage")));
            this.pbGeometry.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.pbGeometry.Location = new System.Drawing.Point(539, 6);
            this.pbGeometry.Name = "pbGeometry";
            this.pbGeometry.Size = new System.Drawing.Size(374, 475);
            this.pbGeometry.TabIndex = 1;
            this.pbGeometry.TabStop = false;
            // 
            // dgInput
            // 
            this.dgInput.AllowUserToAddRows = false;
            this.dgInput.AllowUserToDeleteRows = false;
            this.dgInput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgInput.Location = new System.Drawing.Point(3, 3);
            this.dgInput.Name = "dgInput";
            this.dgInput.Size = new System.Drawing.Size(530, 484);
            this.dgInput.TabIndex = 0;
            this.dgInput.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgInput_CellValueChanged);
            // 
            // tpOutputOptions
            // 
            this.tpOutputOptions.Controls.Add(this.tbMaxTime);
            this.tpOutputOptions.Controls.Add(this.lblMaxTime);
            this.tpOutputOptions.Controls.Add(this.gbPlottingOptions);
            this.tpOutputOptions.Controls.Add(this.tbPlotTimes);
            this.tpOutputOptions.Controls.Add(this.lblPlotPressures);
            this.tpOutputOptions.Location = new System.Drawing.Point(4, 25);
            this.tpOutputOptions.Name = "tpOutputOptions";
            this.tpOutputOptions.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutputOptions.Size = new System.Drawing.Size(916, 487);
            this.tpOutputOptions.TabIndex = 1;
            this.tpOutputOptions.Text = "Output Options";
            this.tpOutputOptions.UseVisualStyleBackColor = true;
            // 
            // tbMaxTime
            // 
            this.tbMaxTime.Location = new System.Drawing.Point(108, 40);
            this.tbMaxTime.Name = "tbMaxTime";
            this.tbMaxTime.Size = new System.Drawing.Size(100, 22);
            this.tbMaxTime.TabIndex = 6;
            this.tbMaxTime.TextChanged += new System.EventHandler(this.tbMaxTime_TextChanged);
            // 
            // lblMaxTime
            // 
            this.lblMaxTime.AutoSize = true;
            this.lblMaxTime.Location = new System.Drawing.Point(16, 42);
            this.lblMaxTime.Name = "lblMaxTime";
            this.lblMaxTime.Size = new System.Drawing.Size(100, 17);
            this.lblMaxTime.TabIndex = 5;
            this.lblMaxTime.Text = "Maximum time:";
            // 
            // gbPlottingOptions
            // 
            this.gbPlottingOptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPlottingOptions.Controls.Add(this.cbMarkChartWithPTDots);
            this.gbPlottingOptions.Controls.Add(this.gbTimePressureOptions);
            this.gbPlottingOptions.Controls.Add(this.gbHorizontalLines);
            this.gbPlottingOptions.Controls.Add(this.cbHorizontalLines);
            this.gbPlottingOptions.Location = new System.Drawing.Point(6, 66);
            this.gbPlottingOptions.Name = "gbPlottingOptions";
            this.gbPlottingOptions.Size = new System.Drawing.Size(904, 418);
            this.gbPlottingOptions.TabIndex = 4;
            this.gbPlottingOptions.TabStop = false;
            this.gbPlottingOptions.Text = "Plotting options";
            // 
            // cbMarkChartWithPTDots
            // 
            this.cbMarkChartWithPTDots.AutoSize = true;
            this.cbMarkChartWithPTDots.Checked = true;
            this.cbMarkChartWithPTDots.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbMarkChartWithPTDots.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.cbMarkChartWithPTDots.Location = new System.Drawing.Point(287, 16);
            this.cbMarkChartWithPTDots.Name = "cbMarkChartWithPTDots";
            this.cbMarkChartWithPTDots.Size = new System.Drawing.Size(245, 21);
            this.cbMarkChartWithPTDots.TabIndex = 10;
            this.cbMarkChartWithPTDots.Text = "Mark chart with pressures at times";
            this.cbMarkChartWithPTDots.UseVisualStyleBackColor = true;
            this.cbMarkChartWithPTDots.CheckedChanged += new System.EventHandler(this.cbMarkChartWithPTDots_CheckedChanged);
            // 
            // gbTimePressureOptions
            // 
            this.gbTimePressureOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbTimePressureOptions.Controls.Add(this.dgTimePressureOptions);
            this.gbTimePressureOptions.Location = new System.Drawing.Point(284, 40);
            this.gbTimePressureOptions.Name = "gbTimePressureOptions";
            this.gbTimePressureOptions.Size = new System.Drawing.Size(316, 369);
            this.gbTimePressureOptions.TabIndex = 9;
            this.gbTimePressureOptions.TabStop = false;
            this.gbTimePressureOptions.Text = "Place dots where pressure/time intersect";
            // 
            // dgTimePressureOptions
            // 
            this.dgTimePressureOptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgTimePressureOptions.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colMarkTime,
            this.colMarkTimePressure});
            this.dgTimePressureOptions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgTimePressureOptions.Location = new System.Drawing.Point(3, 18);
            this.dgTimePressureOptions.Name = "dgTimePressureOptions";
            this.dgTimePressureOptions.Size = new System.Drawing.Size(310, 348);
            this.dgTimePressureOptions.TabIndex = 4;
            this.dgTimePressureOptions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgTimePressureOptions_CellValueChanged);
            this.dgTimePressureOptions.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgTimePressureOptions_RowsRemoved);
            this.dgTimePressureOptions.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgTimePressureOptions_SortCompare);
            // 
            // colMarkTime
            // 
            this.colMarkTime.HeaderText = "Time (Seconds)";
            this.colMarkTime.Name = "colMarkTime";
            // 
            // colMarkTimePressure
            // 
            this.colMarkTimePressure.HeaderText = "Pressure (kPa)";
            this.colMarkTimePressure.Name = "colMarkTimePressure";
            // 
            // gbHorizontalLines
            // 
            this.gbHorizontalLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbHorizontalLines.Controls.Add(this.dgHorzLinesSpecifier);
            this.gbHorizontalLines.Location = new System.Drawing.Point(9, 37);
            this.gbHorizontalLines.Name = "gbHorizontalLines";
            this.gbHorizontalLines.Size = new System.Drawing.Size(225, 375);
            this.gbHorizontalLines.TabIndex = 8;
            this.gbHorizontalLines.TabStop = false;
            this.gbHorizontalLines.Text = "Specify pressures in kPa";
            // 
            // dgHorzLinesSpecifier
            // 
            this.dgHorzLinesSpecifier.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgHorzLinesSpecifier.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colHorzPressure});
            this.dgHorzLinesSpecifier.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgHorzLinesSpecifier.Location = new System.Drawing.Point(3, 18);
            this.dgHorzLinesSpecifier.Name = "dgHorzLinesSpecifier";
            this.dgHorzLinesSpecifier.Size = new System.Drawing.Size(219, 354);
            this.dgHorzLinesSpecifier.TabIndex = 6;
            this.dgHorzLinesSpecifier.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgHorzLinesSpecifier_CellValueChanged);
            this.dgHorzLinesSpecifier.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgHorzLinesSpecifier_RowsRemoved);
            this.dgHorzLinesSpecifier.SortCompare += new System.Windows.Forms.DataGridViewSortCompareEventHandler(this.dgHorzLinesSpecifier_SortCompare);
            // 
            // colHorzPressure
            // 
            this.colHorzPressure.HeaderText = "Pressure (kPa)";
            this.colHorzPressure.Name = "colHorzPressure";
            // 
            // cbHorizontalLines
            // 
            this.cbHorizontalLines.AutoSize = true;
            this.cbHorizontalLines.Checked = true;
            this.cbHorizontalLines.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbHorizontalLines.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.cbHorizontalLines.Location = new System.Drawing.Point(10, 16);
            this.cbHorizontalLines.Name = "cbHorizontalLines";
            this.cbHorizontalLines.Size = new System.Drawing.Size(360, 21);
            this.cbHorizontalLines.TabIndex = 7;
            this.cbHorizontalLines.Text = "Draw horizontal lines on chart at specified pressures";
            this.cbHorizontalLines.UseVisualStyleBackColor = true;
            this.cbHorizontalLines.CheckedChanged += new System.EventHandler(this.cbHorizontalLines_CheckedChanged);
            // 
            // tbPlotTimes
            // 
            this.tbPlotTimes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbPlotTimes.Location = new System.Drawing.Point(230, 10);
            this.tbPlotTimes.Name = "tbPlotTimes";
            this.tbPlotTimes.Size = new System.Drawing.Size(668, 22);
            this.tbPlotTimes.TabIndex = 1;
            this.tbPlotTimes.TextChanged += new System.EventHandler(this.tbPlotTimes_TextChanged);
            // 
            // lblPlotPressures
            // 
            this.lblPlotPressures.AutoSize = true;
            this.lblPlotPressures.Location = new System.Drawing.Point(6, 12);
            this.lblPlotPressures.Name = "lblPlotPressures";
            this.lblPlotPressures.Size = new System.Drawing.Size(296, 17);
            this.lblPlotPressures.TabIndex = 0;
            this.lblPlotPressures.Text = "Output pressures at these times (in seconds):";
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(848, 525);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(75, 23);
            this.btnExecute.TabIndex = 16;
            this.btnExecute.Text = "Calculate";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.tcOutput);
            this.tpOutput.Location = new System.Drawing.Point(4, 25);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(3);
            this.tpOutput.Size = new System.Drawing.Size(930, 554);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // tcOutput
            // 
            this.tcOutput.Controls.Add(this.tpPressure);
            this.tcOutput.Controls.Add(this.tpFlammableMass);
            this.tcOutput.Controls.Add(this.tpLayer);
            this.tcOutput.Controls.Add(this.tpTrajectory);
            this.tcOutput.Controls.Add(this.tpData);
            this.tcOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcOutput.Location = new System.Drawing.Point(3, 3);
            this.tcOutput.Name = "tcOutput";
            this.tcOutput.SelectedIndex = 0;
            this.tcOutput.Size = new System.Drawing.Size(924, 548);
            this.tcOutput.TabIndex = 3;
            // 
            // tpPressure
            // 
            this.tpPressure.Controls.Add(this.pbPressure);
            this.tpPressure.Location = new System.Drawing.Point(4, 25);
            this.tpPressure.Name = "tpPressure";
            this.tpPressure.Padding = new System.Windows.Forms.Padding(3);
            this.tpPressure.Size = new System.Drawing.Size(916, 519);
            this.tpPressure.TabIndex = 0;
            this.tpPressure.Text = "Pressure plot";
            this.tpPressure.UseVisualStyleBackColor = true;
            // 
            // pbPressure
            // 
            this.pbPressure.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPressure.Location = new System.Drawing.Point(3, 3);
            this.pbPressure.Name = "pbPressure";
            this.pbPressure.Size = new System.Drawing.Size(910, 513);
            this.pbPressure.TabIndex = 0;
            this.pbPressure.TabStop = false;
            // 
            // tpFlammableMass
            // 
            this.tpFlammableMass.Controls.Add(this.pbFlammableMass);
            this.tpFlammableMass.Location = new System.Drawing.Point(4, 25);
            this.tpFlammableMass.Name = "tpFlammableMass";
            this.tpFlammableMass.Size = new System.Drawing.Size(916, 519);
            this.tpFlammableMass.TabIndex = 4;
            this.tpFlammableMass.Text = "Flammable mass plot";
            this.tpFlammableMass.UseVisualStyleBackColor = true;
            // 
            // pbFlammableMass
            // 
            this.pbFlammableMass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbFlammableMass.Location = new System.Drawing.Point(0, 0);
            this.pbFlammableMass.Name = "pbFlammableMass";
            this.pbFlammableMass.Size = new System.Drawing.Size(916, 519);
            this.pbFlammableMass.TabIndex = 1;
            this.pbFlammableMass.TabStop = false;
            // 
            // tpLayer
            // 
            this.tpLayer.Controls.Add(this.pbLayer);
            this.tpLayer.Location = new System.Drawing.Point(4, 25);
            this.tpLayer.Name = "tpLayer";
            this.tpLayer.Padding = new System.Windows.Forms.Padding(3);
            this.tpLayer.Size = new System.Drawing.Size(916, 519);
            this.tpLayer.TabIndex = 1;
            this.tpLayer.Text = "Layer plot";
            this.tpLayer.UseVisualStyleBackColor = true;
            // 
            // pbLayer
            // 
            this.pbLayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbLayer.Location = new System.Drawing.Point(3, 3);
            this.pbLayer.Name = "pbLayer";
            this.pbLayer.Size = new System.Drawing.Size(910, 513);
            this.pbLayer.TabIndex = 0;
            this.pbLayer.TabStop = false;
            // 
            // tpTrajectory
            // 
            this.tpTrajectory.Controls.Add(this.pbTrajectory);
            this.tpTrajectory.Location = new System.Drawing.Point(4, 25);
            this.tpTrajectory.Name = "tpTrajectory";
            this.tpTrajectory.Size = new System.Drawing.Size(916, 519);
            this.tpTrajectory.TabIndex = 3;
            this.tpTrajectory.Text = "Trajectory plot";
            this.tpTrajectory.UseVisualStyleBackColor = true;
            // 
            // pbTrajectory
            // 
            this.pbTrajectory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbTrajectory.Location = new System.Drawing.Point(0, 0);
            this.pbTrajectory.Name = "pbTrajectory";
            this.pbTrajectory.Size = new System.Drawing.Size(916, 519);
            this.pbTrajectory.TabIndex = 1;
            this.pbTrajectory.TabStop = false;
            // 
            // tpData
            // 
            this.tpData.Controls.Add(this.dgDepthAndConcentration);
            this.tpData.Controls.Add(this.tbTime);
            this.tpData.Controls.Add(this.lblSeconds);
            this.tpData.Controls.Add(this.tbMaxPressure);
            this.tpData.Controls.Add(this.label1);
            this.tpData.Controls.Add(this.dgPressures);
            this.tpData.Location = new System.Drawing.Point(4, 25);
            this.tpData.Name = "tpData";
            this.tpData.Size = new System.Drawing.Size(916, 519);
            this.tpData.TabIndex = 2;
            this.tpData.Text = "Data";
            this.tpData.UseVisualStyleBackColor = true;
            // 
            // dgDepthAndConcentration
            // 
            this.dgDepthAndConcentration.AllowUserToAddRows = false;
            this.dgDepthAndConcentration.AllowUserToDeleteRows = false;
            this.dgDepthAndConcentration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgDepthAndConcentration.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgDepthAndConcentration.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDepth,
            this.colConcentration});
            this.dgDepthAndConcentration.Location = new System.Drawing.Point(261, 73);
            this.dgDepthAndConcentration.Name = "dgDepthAndConcentration";
            this.dgDepthAndConcentration.RowHeadersVisible = false;
            this.dgDepthAndConcentration.Size = new System.Drawing.Size(207, 422);
            this.dgDepthAndConcentration.TabIndex = 7;
            // 
            // colDepth
            // 
            this.colDepth.HeaderText = "Depth (m)";
            this.colDepth.Name = "colDepth";
            this.colDepth.ReadOnly = true;
            // 
            // colConcentration
            // 
            this.colConcentration.HeaderText = "Concentration (%)";
            this.colConcentration.Name = "colConcentration";
            this.colConcentration.ReadOnly = true;
            // 
            // tbTime
            // 
            this.tbTime.Enabled = false;
            this.tbTime.Location = new System.Drawing.Point(172, 34);
            this.tbTime.Name = "tbTime";
            this.tbTime.Size = new System.Drawing.Size(184, 22);
            this.tbTime.TabIndex = 6;
            // 
            // lblSeconds
            // 
            this.lblSeconds.AutoSize = true;
            this.lblSeconds.Location = new System.Drawing.Point(16, 37);
            this.lblSeconds.Name = "lblSeconds";
            this.lblSeconds.Size = new System.Drawing.Size(201, 17);
            this.lblSeconds.TabIndex = 5;
            this.lblSeconds.Text = "Time this Occurred (Seconds):";
            // 
            // tbMaxPressure
            // 
            this.tbMaxPressure.Enabled = false;
            this.tbMaxPressure.Location = new System.Drawing.Point(172, 6);
            this.tbMaxPressure.Name = "tbMaxPressure";
            this.tbMaxPressure.Size = new System.Drawing.Size(184, 22);
            this.tbMaxPressure.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(192, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "Maximum Overpressure (Pa):";
            // 
            // dgPressures
            // 
            this.dgPressures.AllowUserToAddRows = false;
            this.dgPressures.AllowUserToDeleteRows = false;
            this.dgPressures.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgPressures.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgPressures.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colTime,
            this.dataGridViewTextBoxColumn1});
            this.dgPressures.Location = new System.Drawing.Point(19, 73);
            this.dgPressures.Name = "dgPressures";
            this.dgPressures.RowHeadersVisible = false;
            this.dgPressures.Size = new System.Drawing.Size(207, 422);
            this.dgPressures.TabIndex = 2;
            // 
            // colTime
            // 
            this.colTime.HeaderText = "Time (Seconds)";
            this.colTime.Name = "colTime";
            this.colTime.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Combined Pressure (Pa)";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // cpOverpressure
            // 
            this.Controls.Add(this.tcMain);
            this.Name = "CpOverpressure";
            this.Size = new System.Drawing.Size(938, 583);
            this.Load += new System.EventHandler(this.cpOverpressure_Load);
            this.tcMain.ResumeLayout(false);
            this.tpInput.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbOverpSpinner)).EndInit();
            this.tcInputOptions.ResumeLayout(false);
            this.tpIndoorRelease.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbGeometry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).EndInit();
            this.tpOutputOptions.ResumeLayout(false);
            this.tpOutputOptions.PerformLayout();
            this.gbPlottingOptions.ResumeLayout(false);
            this.gbPlottingOptions.PerformLayout();
            this.gbTimePressureOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgTimePressureOptions)).EndInit();
            this.gbHorizontalLines.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgHorzLinesSpecifier)).EndInit();
            this.tpOutput.ResumeLayout(false);
            this.tcOutput.ResumeLayout(false);
            this.tpPressure.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPressure)).EndInit();
            this.tpFlammableMass.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbFlammableMass)).EndInit();
            this.tpLayer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbLayer)).EndInit();
            this.tpTrajectory.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTrajectory)).EndInit();
            this.tpData.ResumeLayout(false);
            this.tpData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgDepthAndConcentration)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgPressures)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcMain;
		private System.Windows.Forms.TabPage tpInput;
		private System.Windows.Forms.DataGridView dgInput;
		private System.Windows.Forms.TabPage tpOutput;
		private System.Windows.Forms.Button btnExecute;
		private System.Windows.Forms.TabControl tcInputOptions;
		private System.Windows.Forms.TabPage tpIndoorRelease;
		private System.Windows.Forms.TabPage tpOutputOptions;
		private System.Windows.Forms.TextBox tbPlotTimes;
		private System.Windows.Forms.Label lblPlotPressures;
		private System.Windows.Forms.GroupBox gbPlottingOptions;
		private System.Windows.Forms.DataGridView dgTimePressureOptions;
		private System.Windows.Forms.DataGridView dgHorzLinesSpecifier;
		private System.Windows.Forms.DataGridViewTextBoxColumn colHorzPressure;
		private System.Windows.Forms.DataGridViewTextBoxColumn colMarkTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn colMarkTimePressure;
		private System.Windows.Forms.DataGridView dgPressures;
		private System.Windows.Forms.CheckBox cbHorizontalLines;
		private System.Windows.Forms.GroupBox gbHorizontalLines;
		private System.Windows.Forms.GroupBox gbTimePressureOptions;
		private System.Windows.Forms.CheckBox cbMarkChartWithPTDots;
		private System.Windows.Forms.PictureBox pbGeometry;
		private System.Windows.Forms.TabControl tcOutput;
		private System.Windows.Forms.TabPage tpPressure;
		private CustomControls.PictureBoxWithSave pbPressure;
		private System.Windows.Forms.TabPage tpLayer;
		private CustomControls.PictureBoxWithSave pbLayer;
		private System.Windows.Forms.TabPage tpData;
		private System.Windows.Forms.TextBox tbTime;
		private System.Windows.Forms.Label lblSeconds;
		private System.Windows.Forms.TextBox tbMaxPressure;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lblError;
		private System.Windows.Forms.TextBox tbMaxTime;
		private System.Windows.Forms.Label lblMaxTime;
		private System.Windows.Forms.TabPage tpTrajectory;
		private CustomControls.PictureBoxWithSave pbTrajectory;
		private System.Windows.Forms.TabPage tpFlammableMass;
		private CustomControls.PictureBoxWithSave pbFlammableMass;
		private System.Windows.Forms.DataGridView dgDepthAndConcentration;
		private System.Windows.Forms.DataGridViewTextBoxColumn colDepth;
		private System.Windows.Forms.DataGridViewTextBoxColumn colConcentration;
		private System.Windows.Forms.DataGridViewTextBoxColumn colTime;
		private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.PictureBox pbOverpSpinner;
    }
}
