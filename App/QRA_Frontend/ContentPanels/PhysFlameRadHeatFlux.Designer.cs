
namespace QRA_Frontend.ContentPanels {
	partial class CpFlQradSingleAndMulti {
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
            this.tpInput = new System.Windows.Forms.TabPage();
            this.pbSpinner = new System.Windows.Forms.PictureBox();
            this.cbRadiativeSourceModel = new UIHelpers.AnyEnumComboSelector();
            this.lblContourLevels = new System.Windows.Forms.Label();
            this.tbContourLevels = new System.Windows.Forms.TextBox();
            this.pbFlameGeometry = new System.Windows.Forms.PictureBox();
            this.lblArrayWarning = new System.Windows.Forms.Label();
            this.lblZElementCount = new System.Windows.Forms.Label();
            this.lblYElemCount = new System.Windows.Forms.Label();
            this.lblXElemCount = new System.Windows.Forms.Label();
            this.tbRadiativeHeatFluxPointsZ = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsZ = new System.Windows.Forms.Label();
            this.tbRadiativeHeatFluxPointsY = new System.Windows.Forms.TextBox();
            this.lblHeadFluxPointsY = new System.Windows.Forms.Label();
            this.tbRadiativeHeatFluxPointsX = new System.Windows.Forms.TextBox();
            this.lblHeatFluxPointsX = new System.Windows.Forms.Label();
            this.btnExecute = new System.Windows.Forms.Button();
            this.dgInput = new System.Windows.Forms.DataGridView();
            this.tpOutput = new System.Windows.Forms.TabPage();
            this.tcOutputs = new System.Windows.Forms.TabControl();
            this.tpOutputData = new System.Windows.Forms.TabPage();
            this.dgResult = new System.Windows.Forms.DataGridView();
            this.colX = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colZ = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFlux = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblResult = new System.Windows.Forms.Label();
            this.btnCopyToClipboard = new System.Windows.Forms.Button();
            this.tpPlotIsoPlot = new System.Windows.Forms.TabPage();
            this.tpt_fname = new System.Windows.Forms.TabPage();
            this.nnms = new QRA_Frontend.CustomControls.NotionalNozzleModelSelector();
            this.pbPlotIsoOutput = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.pbTPlot = new QRA_Frontend.CustomControls.PictureBoxWithSave();
            this.tcIO.SuspendLayout();
            this.tpInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlameGeometry)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).BeginInit();
            this.tpOutput.SuspendLayout();
            this.tcOutputs.SuspendLayout();
            this.tpOutputData.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).BeginInit();
            this.tpPlotIsoPlot.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbPlotIsoOutput)).BeginInit();
            this.tpt_fname.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbTPlot)).BeginInit();
            this.SuspendLayout();
            // 
            // tcIO
            // 
            this.tcIO.Controls.Add(this.tpInput);
            this.tcIO.Controls.Add(this.tpOutput);
            this.tcIO.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcIO.Location = new System.Drawing.Point(0, 0);
            this.tcIO.Margin = new System.Windows.Forms.Padding(4);
            this.tcIO.Name = "tcIO";
            this.tcIO.SelectedIndex = 0;
            this.tcIO.Size = new System.Drawing.Size(1016, 567);
            this.tcIO.TabIndex = 0;
            // 
            // tpInput
            // 
            this.tpInput.Controls.Add(this.pbSpinner);
            this.tpInput.Controls.Add(this.cbRadiativeSourceModel);
            this.tpInput.Controls.Add(this.lblContourLevels);
            this.tpInput.Controls.Add(this.tbContourLevels);
            this.tpInput.Controls.Add(this.pbFlameGeometry);
            this.tpInput.Controls.Add(this.nnms);
            this.tpInput.Controls.Add(this.lblArrayWarning);
            this.tpInput.Controls.Add(this.lblZElementCount);
            this.tpInput.Controls.Add(this.lblYElemCount);
            this.tpInput.Controls.Add(this.lblXElemCount);
            this.tpInput.Controls.Add(this.tbRadiativeHeatFluxPointsZ);
            this.tpInput.Controls.Add(this.lblHeatFluxPointsZ);
            this.tpInput.Controls.Add(this.tbRadiativeHeatFluxPointsY);
            this.tpInput.Controls.Add(this.lblHeadFluxPointsY);
            this.tpInput.Controls.Add(this.tbRadiativeHeatFluxPointsX);
            this.tpInput.Controls.Add(this.lblHeatFluxPointsX);
            this.tpInput.Controls.Add(this.btnExecute);
            this.tpInput.Controls.Add(this.dgInput);
            this.tpInput.Location = new System.Drawing.Point(4, 25);
            this.tpInput.Margin = new System.Windows.Forms.Padding(4);
            this.tpInput.Name = "tpInput";
            this.tpInput.Padding = new System.Windows.Forms.Padding(4);
            this.tpInput.Size = new System.Drawing.Size(1008, 538);
            this.tpInput.TabIndex = 0;
            this.tpInput.Text = "Input";
            this.tpInput.UseVisualStyleBackColor = true;
            // 
            // pbSpinner
            // 
            this.pbSpinner.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pbSpinner.Image = global::QRA_Frontend.Properties.Resources.AjaxSpinner;
            this.pbSpinner.Location = new System.Drawing.Point(593, 434);
            this.pbSpinner.MinimumSize = new System.Drawing.Size(20, 20);
            this.pbSpinner.Name = "pbSpinner";
            this.pbSpinner.Size = new System.Drawing.Size(32, 28);
            this.pbSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbSpinner.TabIndex = 20;
            this.pbSpinner.TabStop = false;
            // 
            // cbRadiativeSourceModel
            // 
            this.cbRadiativeSourceModel.Caption = "Radiative Source Model:";
            this.cbRadiativeSourceModel.Location = new System.Drawing.Point(15, 49);
            this.cbRadiativeSourceModel.Margin = new System.Windows.Forms.Padding(5);
            this.cbRadiativeSourceModel.Name = "cbRadiativeSourceModel";
            this.cbRadiativeSourceModel.SelectedItem = null;
            this.cbRadiativeSourceModel.Size = new System.Drawing.Size(480, 28);
            this.cbRadiativeSourceModel.TabIndex = 19;
            this.cbRadiativeSourceModel.OnValueChanged += new System.EventHandler(this.cbRadiativeSourceModel_OnValueChanged);
            // 
            // lblContourLevels
            // 
            this.lblContourLevels.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblContourLevels.AutoSize = true;
            this.lblContourLevels.Location = new System.Drawing.Point(57, 441);
            this.lblContourLevels.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblContourLevels.Name = "lblContourLevels";
            this.lblContourLevels.Size = new System.Drawing.Size(171, 17);
            this.lblContourLevels.TabIndex = 18;
            this.lblContourLevels.Text = "Contour Levels (kW/m^2):";
            // 
            // tbContourLevels
            // 
            this.tbContourLevels.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbContourLevels.Location = new System.Drawing.Point(233, 437);
            this.tbContourLevels.Margin = new System.Windows.Forms.Padding(4);
            this.tbContourLevels.Name = "tbContourLevels";
            this.tbContourLevels.Size = new System.Drawing.Size(353, 22);
            this.tbContourLevels.TabIndex = 17;
            this.tbContourLevels.TextChanged += new System.EventHandler(this.tbContourLevels_TextChanged);
            // 
            // pbFlameGeometry
            // 
            this.pbFlameGeometry.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbFlameGeometry.Image = global::QRA_Frontend.Properties.Resources.geometry_of_flame;
            this.pbFlameGeometry.Location = new System.Drawing.Point(608, 7);
            this.pbFlameGeometry.Margin = new System.Windows.Forms.Padding(4);
            this.pbFlameGeometry.Name = "pbFlameGeometry";
            this.pbFlameGeometry.Size = new System.Drawing.Size(389, 326);
            this.pbFlameGeometry.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbFlameGeometry.TabIndex = 1;
            this.pbFlameGeometry.TabStop = false;
            // 
            // nnms
            // 
            this.nnms.CanChange = false;
            this.nnms.Location = new System.Drawing.Point(13, 10);
            this.nnms.Margin = new System.Windows.Forms.Padding(5);
            this.nnms.Name = "nnms";
            this.nnms.Size = new System.Drawing.Size(499, 31);
            this.nnms.TabIndex = 15;
            // 
            // lblArrayWarning
            // 
            this.lblArrayWarning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblArrayWarning.AutoSize = true;
            this.lblArrayWarning.ForeColor = System.Drawing.Color.Red;
            this.lblArrayWarning.Location = new System.Drawing.Point(759, 446);
            this.lblArrayWarning.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblArrayWarning.Name = "lblArrayWarning";
            this.lblArrayWarning.Size = new System.Drawing.Size(241, 17);
            this.lblArrayWarning.TabIndex = 14;
            this.lblArrayWarning.Text = "X.Y.Z. Arrays must be the same size.";
            this.lblArrayWarning.Visible = false;
            // 
            // lblZElementCount
            // 
            this.lblZElementCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblZElementCount.AutoSize = true;
            this.lblZElementCount.Location = new System.Drawing.Point(735, 402);
            this.lblZElementCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblZElementCount.Name = "lblZElementCount";
            this.lblZElementCount.Size = new System.Drawing.Size(100, 17);
            this.lblZElementCount.TabIndex = 13;
            this.lblZElementCount.Text = "Element Count";
            // 
            // lblYElemCount
            // 
            this.lblYElemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblYElemCount.AutoSize = true;
            this.lblYElemCount.Location = new System.Drawing.Point(735, 370);
            this.lblYElemCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblYElemCount.Name = "lblYElemCount";
            this.lblYElemCount.Size = new System.Drawing.Size(100, 17);
            this.lblYElemCount.TabIndex = 12;
            this.lblYElemCount.Text = "Element Count";
            // 
            // lblXElemCount
            // 
            this.lblXElemCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblXElemCount.AutoSize = true;
            this.lblXElemCount.Location = new System.Drawing.Point(735, 341);
            this.lblXElemCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblXElemCount.Name = "lblXElemCount";
            this.lblXElemCount.Size = new System.Drawing.Size(138, 17);
            this.lblXElemCount.TabIndex = 11;
            this.lblXElemCount.Text = "Array Element Count";
            // 
            // tbRadiativeHeatFluxPointsZ
            // 
            this.tbRadiativeHeatFluxPointsZ.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRadiativeHeatFluxPointsZ.Location = new System.Drawing.Point(233, 402);
            this.tbRadiativeHeatFluxPointsZ.Margin = new System.Windows.Forms.Padding(4);
            this.tbRadiativeHeatFluxPointsZ.Name = "tbRadiativeHeatFluxPointsZ";
            this.tbRadiativeHeatFluxPointsZ.Size = new System.Drawing.Size(492, 22);
            this.tbRadiativeHeatFluxPointsZ.TabIndex = 8;
            this.tbRadiativeHeatFluxPointsZ.TextChanged += new System.EventHandler(this.tbRadiativeHeatFluxPointsZ_TextChanged);
            // 
            // lblHeatFluxPointsZ
            // 
            this.lblHeatFluxPointsZ.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsZ.AutoSize = true;
            this.lblHeatFluxPointsZ.Location = new System.Drawing.Point(17, 407);
            this.lblHeatFluxPointsZ.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeatFluxPointsZ.Name = "lblHeatFluxPointsZ";
            this.lblHeatFluxPointsZ.Size = new System.Drawing.Size(215, 17);
            this.lblHeatFluxPointsZ.TabIndex = 7;
            this.lblHeatFluxPointsZ.Text = "Z Radiative Heat Flux Points (m):";
            // 
            // tbRadiativeHeatFluxPointsY
            // 
            this.tbRadiativeHeatFluxPointsY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRadiativeHeatFluxPointsY.Location = new System.Drawing.Point(233, 370);
            this.tbRadiativeHeatFluxPointsY.Margin = new System.Windows.Forms.Padding(4);
            this.tbRadiativeHeatFluxPointsY.Name = "tbRadiativeHeatFluxPointsY";
            this.tbRadiativeHeatFluxPointsY.Size = new System.Drawing.Size(492, 22);
            this.tbRadiativeHeatFluxPointsY.TabIndex = 6;
            this.tbRadiativeHeatFluxPointsY.TextChanged += new System.EventHandler(this.tbRadiativeHeatFluxPointsY_TextChanged);
            // 
            // lblHeadFluxPointsY
            // 
            this.lblHeadFluxPointsY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeadFluxPointsY.AutoSize = true;
            this.lblHeadFluxPointsY.Location = new System.Drawing.Point(17, 375);
            this.lblHeadFluxPointsY.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeadFluxPointsY.Name = "lblHeadFluxPointsY";
            this.lblHeadFluxPointsY.Size = new System.Drawing.Size(215, 17);
            this.lblHeadFluxPointsY.TabIndex = 5;
            this.lblHeadFluxPointsY.Text = "Y Radiative Heat Flux Points (m):";
            // 
            // tbRadiativeHeatFluxPointsX
            // 
            this.tbRadiativeHeatFluxPointsX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tbRadiativeHeatFluxPointsX.Location = new System.Drawing.Point(233, 341);
            this.tbRadiativeHeatFluxPointsX.Margin = new System.Windows.Forms.Padding(4);
            this.tbRadiativeHeatFluxPointsX.Name = "tbRadiativeHeatFluxPointsX";
            this.tbRadiativeHeatFluxPointsX.Size = new System.Drawing.Size(492, 22);
            this.tbRadiativeHeatFluxPointsX.TabIndex = 4;
            this.tbRadiativeHeatFluxPointsX.TextChanged += new System.EventHandler(this.tbRadiativeHeatFluxPointsX_TextChanged);
            // 
            // lblHeatFluxPointsX
            // 
            this.lblHeatFluxPointsX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblHeatFluxPointsX.AutoSize = true;
            this.lblHeatFluxPointsX.Location = new System.Drawing.Point(17, 346);
            this.lblHeatFluxPointsX.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblHeatFluxPointsX.Name = "lblHeatFluxPointsX";
            this.lblHeatFluxPointsX.Size = new System.Drawing.Size(215, 17);
            this.lblHeatFluxPointsX.TabIndex = 3;
            this.lblHeatFluxPointsX.Text = "X Radiative Heat Flux Points (m):";
            // 
            // btnExecute
            // 
            this.btnExecute.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExecute.Location = new System.Drawing.Point(632, 434);
            this.btnExecute.Margin = new System.Windows.Forms.Padding(4);
            this.btnExecute.Name = "btnExecute";
            this.btnExecute.Size = new System.Drawing.Size(95, 28);
            this.btnExecute.TabIndex = 2;
            this.btnExecute.Text = "Calculate";
            this.btnExecute.UseVisualStyleBackColor = true;
            this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
            // 
            // dgInput
            // 
            this.dgInput.AllowUserToAddRows = false;
            this.dgInput.AllowUserToDeleteRows = false;
            this.dgInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgInput.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgInput.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgInput.Location = new System.Drawing.Point(8, 85);
            this.dgInput.Margin = new System.Windows.Forms.Padding(4);
            this.dgInput.Name = "dgInput";
            this.dgInput.Size = new System.Drawing.Size(592, 249);
            this.dgInput.TabIndex = 0;
            this.dgInput.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgInput_CellValueChanged);
            // 
            // tpOutput
            // 
            this.tpOutput.Controls.Add(this.tcOutputs);
            this.tpOutput.Location = new System.Drawing.Point(4, 25);
            this.tpOutput.Margin = new System.Windows.Forms.Padding(4);
            this.tpOutput.Name = "tpOutput";
            this.tpOutput.Padding = new System.Windows.Forms.Padding(4);
            this.tpOutput.Size = new System.Drawing.Size(1008, 538);
            this.tpOutput.TabIndex = 1;
            this.tpOutput.Text = "Output";
            this.tpOutput.UseVisualStyleBackColor = true;
            // 
            // tcOutputs
            // 
            this.tcOutputs.Controls.Add(this.tpOutputData);
            this.tcOutputs.Controls.Add(this.tpPlotIsoPlot);
            this.tcOutputs.Controls.Add(this.tpt_fname);
            this.tcOutputs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcOutputs.Location = new System.Drawing.Point(4, 4);
            this.tcOutputs.Margin = new System.Windows.Forms.Padding(4);
            this.tcOutputs.Name = "tcOutputs";
            this.tcOutputs.SelectedIndex = 0;
            this.tcOutputs.Size = new System.Drawing.Size(1000, 530);
            this.tcOutputs.TabIndex = 5;
            // 
            // tpOutputData
            // 
            this.tpOutputData.Controls.Add(this.dgResult);
            this.tpOutputData.Controls.Add(this.lblResult);
            this.tpOutputData.Controls.Add(this.btnCopyToClipboard);
            this.tpOutputData.Location = new System.Drawing.Point(4, 25);
            this.tpOutputData.Margin = new System.Windows.Forms.Padding(4);
            this.tpOutputData.Name = "tpOutputData";
            this.tpOutputData.Padding = new System.Windows.Forms.Padding(4);
            this.tpOutputData.Size = new System.Drawing.Size(992, 501);
            this.tpOutputData.TabIndex = 0;
            this.tpOutputData.Text = "Values";
            this.tpOutputData.UseVisualStyleBackColor = true;
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
            this.dgResult.Location = new System.Drawing.Point(4, 32);
            this.dgResult.Margin = new System.Windows.Forms.Padding(4);
            this.dgResult.Name = "dgResult";
            this.dgResult.Size = new System.Drawing.Size(978, 419);
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
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(7, 6);
            this.lblResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(260, 17);
            this.lblResult.TabIndex = 0;
            this.lblResult.Text = "Radiative heat flux calculated (kW/m^2):";
            // 
            // btnCopyToClipboard
            // 
            this.btnCopyToClipboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyToClipboard.Location = new System.Drawing.Point(823, 459);
            this.btnCopyToClipboard.Margin = new System.Windows.Forms.Padding(4);
            this.btnCopyToClipboard.Name = "btnCopyToClipboard";
            this.btnCopyToClipboard.Size = new System.Drawing.Size(159, 28);
            this.btnCopyToClipboard.TabIndex = 2;
            this.btnCopyToClipboard.Text = "Copy to Clipboard";
            this.btnCopyToClipboard.UseVisualStyleBackColor = true;
            this.btnCopyToClipboard.Click += new System.EventHandler(this.btnCopyToClipboard_Click);
            // 
            // tpPlotIsoPlot
            // 
            this.tpPlotIsoPlot.Controls.Add(this.pbPlotIsoOutput);
            this.tpPlotIsoPlot.Location = new System.Drawing.Point(4, 25);
            this.tpPlotIsoPlot.Margin = new System.Windows.Forms.Padding(4);
            this.tpPlotIsoPlot.Name = "tpPlotIsoPlot";
            this.tpPlotIsoPlot.Padding = new System.Windows.Forms.Padding(4);
            this.tpPlotIsoPlot.Size = new System.Drawing.Size(992, 501);
            this.tpPlotIsoPlot.TabIndex = 1;
            this.tpPlotIsoPlot.Text = "Heat Flux Plot";
            this.tpPlotIsoPlot.UseVisualStyleBackColor = true;
            // 
            // pbPlotIsoOutput
            // 
            this.pbPlotIsoOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbPlotIsoOutput.Location = new System.Drawing.Point(4, 4);
            this.pbPlotIsoOutput.Margin = new System.Windows.Forms.Padding(4);
            this.pbPlotIsoOutput.Name = "pbPlotIsoOutput";
            this.pbPlotIsoOutput.Size = new System.Drawing.Size(984, 493);
            this.pbPlotIsoOutput.TabIndex = 4;
            this.pbPlotIsoOutput.TabStop = false;
            // 
            // tpt_fname
            // 
            this.tpt_fname.Controls.Add(this.pbTPlot);
            this.tpt_fname.Location = new System.Drawing.Point(4, 25);
            this.tpt_fname.Margin = new System.Windows.Forms.Padding(4);
            this.tpt_fname.Name = "tpt_fname";
            this.tpt_fname.Size = new System.Drawing.Size(992, 501);
            this.tpt_fname.TabIndex = 2;
            this.tpt_fname.Text = "Temperature Plot";
            this.tpt_fname.UseVisualStyleBackColor = true;
            // 
            // pbTPlot
            // 
            this.pbTPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbTPlot.Location = new System.Drawing.Point(0, 0);
            this.pbTPlot.Margin = new System.Windows.Forms.Padding(4);
            this.pbTPlot.Name = "pbTPlot";
            this.pbTPlot.Size = new System.Drawing.Size(992, 501);
            this.pbTPlot.TabIndex = 0;
            this.pbTPlot.TabStop = false;
            // 
            // cpFl_QradSingleAndMulti
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tcIO);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "CpFlQradSingleAndMulti";
            this.Size = new System.Drawing.Size(1016, 567);
            this.tcIO.ResumeLayout(false);
            this.tpInput.ResumeLayout(false);
            this.tpInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbFlameGeometry)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgInput)).EndInit();
            this.tpOutput.ResumeLayout(false);
            this.tcOutputs.ResumeLayout(false);
            this.tpOutputData.ResumeLayout(false);
            this.tpOutputData.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResult)).EndInit();
            this.tpPlotIsoPlot.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbPlotIsoOutput)).EndInit();
            this.tpt_fname.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbTPlot)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TabControl tcIO;
		private System.Windows.Forms.TabPage tpInput;
		private System.Windows.Forms.TabPage tpOutput;
		private System.Windows.Forms.DataGridView dgInput;

		private System.Windows.Forms.Button btnExecute;
		private System.Windows.Forms.Label lblResult;
		private System.Windows.Forms.Label lblHeatFluxPointsX;
		private System.Windows.Forms.TextBox tbRadiativeHeatFluxPointsZ;
		private System.Windows.Forms.Label lblHeatFluxPointsZ;
		private System.Windows.Forms.TextBox tbRadiativeHeatFluxPointsY;
		private System.Windows.Forms.Label lblHeadFluxPointsY;
		private System.Windows.Forms.TextBox tbRadiativeHeatFluxPointsX;
		private System.Windows.Forms.Button btnCopyToClipboard;
        private System.Windows.Forms.DataGridView dgResult;
		private System.Windows.Forms.Label lblArrayWarning;
		private System.Windows.Forms.Label lblZElementCount;
		private System.Windows.Forms.Label lblYElemCount;
		private System.Windows.Forms.Label lblXElemCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colX;
        private System.Windows.Forms.DataGridViewTextBoxColumn colY;
        private System.Windows.Forms.DataGridViewTextBoxColumn colZ;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFlux;
		private System.Windows.Forms.PictureBox pbFlameGeometry;
        private System.Windows.Forms.TextBox tbContourLevels;
        private System.Windows.Forms.Label lblContourLevels;
		private QRA_Frontend.CustomControls.PictureBoxWithSave pbPlotIsoOutput;
		private System.Windows.Forms.TabControl tcOutputs;
		private System.Windows.Forms.TabPage tpOutputData;
		private System.Windows.Forms.TabPage tpPlotIsoPlot;
		private System.Windows.Forms.TabPage tpt_fname;
		private QRA_Frontend.CustomControls.PictureBoxWithSave pbTPlot;
		private QRA_Frontend.CustomControls.NotionalNozzleModelSelector nnms;
		private UIHelpers.AnyEnumComboSelector cbRadiativeSourceModel;
        private System.Windows.Forms.PictureBox pbSpinner;
    }
}
