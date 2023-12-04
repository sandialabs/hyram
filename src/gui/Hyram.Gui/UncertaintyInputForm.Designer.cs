namespace SandiaNationalLaboratories.Hyram
{
    partial class UncertaintyInputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.submitBtn = new System.Windows.Forms.Button();
            this.paramLabel = new System.Windows.Forms.Label();
            this.DistrSelector = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ValueInput = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ParamALabel = new System.Windows.Forms.Label();
            this.ParamBLabel = new System.Windows.Forms.Label();
            this.UncertaintySelector = new System.Windows.Forms.ComboBox();
            this.ParamAInput = new System.Windows.Forms.TextBox();
            this.ParamBInput = new System.Windows.Forms.TextBox();
            this.alertLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // submitBtn
            // 
            this.submitBtn.Location = new System.Drawing.Point(466, 63);
            this.submitBtn.Name = "submitBtn";
            this.submitBtn.Size = new System.Drawing.Size(75, 23);
            this.submitBtn.TabIndex = 0;
            this.submitBtn.Text = "Close";
            this.submitBtn.UseVisualStyleBackColor = true;
            this.submitBtn.Click += new System.EventHandler(this.submitBtn_Click);
            // 
            // paramLabel
            // 
            this.paramLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.paramLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.paramLabel.Location = new System.Drawing.Point(7, 10);
            this.paramLabel.Name = "paramLabel";
            this.paramLabel.Size = new System.Drawing.Size(534, 24);
            this.paramLabel.TabIndex = 1;
            this.paramLabel.Text = "tank fluid pressure (absolute)";
            this.paramLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DistrSelector
            // 
            this.DistrSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DistrSelector.FormattingEnabled = true;
            this.DistrSelector.Location = new System.Drawing.Point(7, 65);
            this.DistrSelector.Name = "DistrSelector";
            this.DistrSelector.Size = new System.Drawing.Size(102, 21);
            this.DistrSelector.TabIndex = 2;
            this.DistrSelector.SelectionChangeCommitted += new System.EventHandler(this.DistrSelector_SelectionChangeCommitted);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Distribution";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ValueInput
            // 
            this.ValueInput.Location = new System.Drawing.Point(115, 65);
            this.ValueInput.Name = "ValueInput";
            this.ValueInput.Size = new System.Drawing.Size(83, 20);
            this.ValueInput.TabIndex = 4;
            this.ValueInput.TextChanged += new System.EventHandler(this.ValueInput_TextChanged);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(112, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(86, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Nominal value";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(209, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 15);
            this.label3.TabIndex = 6;
            this.label3.Text = "Uncertainty";
            this.label3.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ParamALabel
            // 
            this.ParamALabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ParamALabel.Location = new System.Drawing.Point(290, 42);
            this.ParamALabel.Name = "ParamALabel";
            this.ParamALabel.Size = new System.Drawing.Size(79, 15);
            this.ParamALabel.TabIndex = 7;
            this.ParamALabel.Text = "Lower bound";
            this.ParamALabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ParamBLabel
            // 
            this.ParamBLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ParamBLabel.Location = new System.Drawing.Point(378, 42);
            this.ParamBLabel.Name = "ParamBLabel";
            this.ParamBLabel.Size = new System.Drawing.Size(79, 15);
            this.ParamBLabel.TabIndex = 8;
            this.ParamBLabel.Text = "Upper bound";
            this.ParamBLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // UncertaintySelector
            // 
            this.UncertaintySelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.UncertaintySelector.FormattingEnabled = true;
            this.UncertaintySelector.Location = new System.Drawing.Point(204, 65);
            this.UncertaintySelector.Name = "UncertaintySelector";
            this.UncertaintySelector.Size = new System.Drawing.Size(80, 21);
            this.UncertaintySelector.TabIndex = 9;
            this.UncertaintySelector.SelectionChangeCommitted += new System.EventHandler(this.UncertaintySelector_SelectionChangeCommitted);
            // 
            // ParamAInput
            // 
            this.ParamAInput.Location = new System.Drawing.Point(288, 65);
            this.ParamAInput.Name = "ParamAInput";
            this.ParamAInput.Size = new System.Drawing.Size(83, 20);
            this.ParamAInput.TabIndex = 10;
            this.ParamAInput.TextChanged += new System.EventHandler(this.ParamAInput_TextChanged);
            // 
            // ParamBInput
            // 
            this.ParamBInput.Location = new System.Drawing.Point(377, 65);
            this.ParamBInput.Name = "ParamBInput";
            this.ParamBInput.Size = new System.Drawing.Size(83, 20);
            this.ParamBInput.TabIndex = 11;
            this.ParamBInput.TextChanged += new System.EventHandler(this.ParamBInput_TextChanged);
            // 
            // alertLabel
            // 
            this.alertLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.alertLabel.AutoSize = true;
            this.alertLabel.BackColor = System.Drawing.Color.MistyRose;
            this.alertLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.alertLabel.ForeColor = System.Drawing.Color.Maroon;
            this.alertLabel.Location = new System.Drawing.Point(7, 95);
            this.alertLabel.Name = "alertLabel";
            this.alertLabel.Padding = new System.Windows.Forms.Padding(4);
            this.alertLabel.Size = new System.Drawing.Size(144, 23);
            this.alertLabel.TabIndex = 71;
            this.alertLabel.Text = "Enter a lower bound";
            this.alertLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.alertLabel.Visible = false;
            // 
            // UncertaintyInputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(551, 123);
            this.ControlBox = false;
            this.Controls.Add(this.alertLabel);
            this.Controls.Add(this.ParamBInput);
            this.Controls.Add(this.ParamAInput);
            this.Controls.Add(this.UncertaintySelector);
            this.Controls.Add(this.ParamBLabel);
            this.Controls.Add(this.ParamALabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ValueInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DistrSelector);
            this.Controls.Add(this.paramLabel);
            this.Controls.Add(this.submitBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "UncertaintyInputForm";
            this.Text = "Sensitivity Analysis - Specify Parameter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button submitBtn;
        private System.Windows.Forms.Label paramLabel;
        private System.Windows.Forms.ComboBox DistrSelector;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ValueInput;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label ParamALabel;
        private System.Windows.Forms.Label ParamBLabel;
        private System.Windows.Forms.ComboBox UncertaintySelector;
        private System.Windows.Forms.TextBox ParamAInput;
        private System.Windows.Forms.TextBox ParamBInput;
        private System.Windows.Forms.Label alertLabel;
    }
}