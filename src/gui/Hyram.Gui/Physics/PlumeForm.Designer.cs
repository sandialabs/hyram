
namespace SandiaNationalLaboratories.Hyram {
	partial class PlumeForm {
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
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.inputTab = new System.Windows.Forms.TabPage();
            this.massFlowLabel = new System.Windows.Forms.Label();
            this.MassFlowInput = new System.Windows.Forms.TextBox();
            this.PhaseSelector = new System.Windows.Forms.ComboBox();
            this.phaseLabel = new System.Windows.Forms.Label();
            this.NozzleSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.inputWarning = new System.Windows.Forms.Label();
            this.spinnerPictureBox = new System.Windows.Forms.PictureBox();
            this.InputGrid = new System.Windows.Forms.DataGridView();
            this.SubmitBtn = new System.Windows.Forms.Button();
            this.PlotTitleInput = new System.Windows.Forms.TextBox();
            this.lblPlotTitle = new System.Windows.Forms.Label();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.outputMassFlowRate = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.OutputPictureBox = new SandiaNationalLaboratories.Hyram.PictureBoxWithSave();
            this.outputWarning = new System.Windows.Forms.Label();
            this.mainTabControl.SuspendLayout();
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
            this.inputTab.Controls.Add(this.PhaseSelector);
            this.inputTab.Controls.Add(this.phaseLabel);
            this.inputTab.Controls.Add(this.NozzleSelector);
            this.inputTab.Controls.Add(this.label1);
            this.inputTab.Controls.Add(this.inputWarning);
            this.inputTab.Controls.Add(this.spinnerPictureBox);
            this.inputTab.Controls.Add(this.InputGrid);
            this.inputTab.Controls.Add(this.SubmitBtn);
            this.inputTab.Controls.Add(this.PlotTitleInput);
            this.inputTab.Controls.Add(this.lblPlotTitle);
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
            this.massFlowLabel.Location = new System.Drawing.Point(9, 90);
            this.massFlowLabel.Name = "massFlowLabel";
            this.massFlowLabel.Size = new System.Drawing.Size(184, 13);
            this.massFlowLabel.TabIndex = 58;
            this.massFlowLabel.Text = "Fluid mass flow rate (unchoked, kg/s)";
            // 
            // MassFlowInput
            // 
            this.MassFlowInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.MassFlowInput.Location = new System.Drawing.Point(205, 87);
            this.MassFlowInput.Name = "MassFlowInput";
            this.MassFlowInput.Size = new System.Drawing.Size(247, 20);
            this.MassFlowInput.TabIndex = 57;
            this.MassFlowInput.TextChanged += new System.EventHandler(this.MassFlowInput_TextChanged);
            // 
            // PhaseSelector
            // 
            this.PhaseSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PhaseSelector.FormattingEnabled = true;
            this.PhaseSelector.Location = new System.Drawing.Point(205, 60);
            this.PhaseSelector.Name = "PhaseSelector";
            this.PhaseSelector.Size = new System.Drawing.Size(247, 21);
            this.PhaseSelector.TabIndex = 56;
            this.PhaseSelector.SelectionChangeCommitted += new System.EventHandler(this.PhaseSelector_SelectionChangeCommitted);
            // 
            // phaseLabel
            // 
            this.phaseLabel.AutoSize = true;
            this.phaseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.phaseLabel.Location = new System.Drawing.Point(9, 61);
            this.phaseLabel.Name = "phaseLabel";
            this.phaseLabel.Size = new System.Drawing.Size(71, 15);
            this.phaseLabel.TabIndex = 55;
            this.phaseLabel.Text = "Fluid phase";
            // 
            // NozzleSelector
            // 
            this.NozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.NozzleSelector.FormattingEnabled = true;
            this.NozzleSelector.Location = new System.Drawing.Point(205, 32);
            this.NozzleSelector.Name = "NozzleSelector";
            this.NozzleSelector.Size = new System.Drawing.Size(247, 21);
            this.NozzleSelector.TabIndex = 26;
            this.NozzleSelector.SelectionChangeCommitted += new System.EventHandler(this.NozzleSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(9, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 15);
            this.label1.TabIndex = 25;
            this.label1.Text = "Notional nozzle model";
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
            this.inputWarning.TabIndex = 22;
            this.inputWarning.Text = "Test warning notification area with long warning message";
            this.inputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.inputWarning.Visible = false;
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
            this.spinnerPictureBox.TabIndex = 21;
            this.spinnerPictureBox.TabStop = false;
            // 
            // InputGrid
            // 
            this.InputGrid.AllowUserToResizeRows = false;
            this.InputGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InputGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.InputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InputGrid.Location = new System.Drawing.Point(5, 112);
            this.InputGrid.Margin = new System.Windows.Forms.Padding(2);
            this.InputGrid.Name = "InputGrid";
            this.InputGrid.RowHeadersVisible = false;
            this.InputGrid.RowTemplate.Height = 24;
            this.InputGrid.ShowEditingIcon = false;
            this.InputGrid.Size = new System.Drawing.Size(445, 401);
            this.InputGrid.TabIndex = 0;
            this.InputGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.InputGrid_CellValueChanged);
            // 
            // SubmitBtn
            // 
            this.SubmitBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SubmitBtn.Location = new System.Drawing.Point(377, 518);
            this.SubmitBtn.Name = "SubmitBtn";
            this.SubmitBtn.Size = new System.Drawing.Size(75, 23);
            this.SubmitBtn.TabIndex = 1;
            this.SubmitBtn.Text = "Calculate";
            this.SubmitBtn.UseVisualStyleBackColor = true;
            this.SubmitBtn.Click += new System.EventHandler(this.SubmitBtn_Click);
            // 
            // PlotTitleInput
            // 
            this.PlotTitleInput.Location = new System.Drawing.Point(205, 6);
            this.PlotTitleInput.Name = "PlotTitleInput";
            this.PlotTitleInput.Size = new System.Drawing.Size(247, 20);
            this.PlotTitleInput.TabIndex = 1;
            this.PlotTitleInput.Text = "Mole Fraction of Leak";
            this.PlotTitleInput.TextChanged += new System.EventHandler(this.PlotTitleInput_TextChanged);
            // 
            // lblPlotTitle
            // 
            this.lblPlotTitle.AutoSize = true;
            this.lblPlotTitle.Location = new System.Drawing.Point(9, 9);
            this.lblPlotTitle.Name = "lblPlotTitle";
            this.lblPlotTitle.Size = new System.Drawing.Size(48, 13);
            this.lblPlotTitle.TabIndex = 0;
            this.lblPlotTitle.Text = "Plot Title";
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
            this.splitContainer1.Panel1.Controls.Add(this.outputMassFlowRate);
            this.splitContainer1.Panel1.Controls.Add(this.label2);
            this.splitContainer1.Panel1.Controls.Add(this.OutputPictureBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.outputWarning);
            this.splitContainer1.Panel2.Enabled = false;
            this.splitContainer1.Size = new System.Drawing.Size(978, 562);
            this.splitContainer1.SplitterDistance = 513;
            this.splitContainer1.TabIndex = 1;
            // 
            // outputMassFlowRate
            // 
            this.outputMassFlowRate.Location = new System.Drawing.Point(115, 4);
            this.outputMassFlowRate.Name = "outputMassFlowRate";
            this.outputMassFlowRate.ReadOnly = true;
            this.outputMassFlowRate.Size = new System.Drawing.Size(114, 20);
            this.outputMassFlowRate.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Mass flow rate (kg/s)";
            // 
            // OutputPictureBox
            // 
            this.OutputPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.OutputPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.OutputPictureBox.Location = new System.Drawing.Point(0, 0);
            this.OutputPictureBox.Name = "OutputPictureBox";
            this.OutputPictureBox.Size = new System.Drawing.Size(978, 513);
            this.OutputPictureBox.TabIndex = 0;
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
            this.outputWarning.Size = new System.Drawing.Size(978, 45);
            this.outputWarning.TabIndex = 21;
            this.outputWarning.Text = "blank";
            this.outputWarning.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.outputWarning.Visible = false;
            // 
            // PlumeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainTabControl);
            this.Name = "PlumeForm";
            this.Size = new System.Drawing.Size(992, 594);
            this.Load += new System.EventHandler(this.PlumeForm_Load);
            this.mainTabControl.ResumeLayout(false);
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

		private System.Windows.Forms.TabControl mainTabControl;
		private System.Windows.Forms.TabPage inputTab;
		private System.Windows.Forms.TabPage outputTab;
		private System.Windows.Forms.Label lblPlotTitle;
		private System.Windows.Forms.TextBox PlotTitleInput;
		private System.Windows.Forms.Button SubmitBtn;
		private Hyram.PictureBoxWithSave OutputPictureBox;
        private System.Windows.Forms.DataGridView InputGrid;
        private System.Windows.Forms.PictureBox spinnerPictureBox;
        private System.Windows.Forms.Label inputWarning;
        private System.Windows.Forms.ComboBox NozzleSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label outputWarning;
        private System.Windows.Forms.ComboBox PhaseSelector;
        private System.Windows.Forms.Label phaseLabel;
        private System.Windows.Forms.TextBox outputMassFlowRate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label massFlowLabel;
        private System.Windows.Forms.TextBox MassFlowInput;
    }
}
