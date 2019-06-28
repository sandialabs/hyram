namespace UIHelpers {
	partial class AnyEnumComboSelector {
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
            this.cbEnums = new System.Windows.Forms.ComboBox();
            this.lblCaption = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbEnums
            // 
            this.cbEnums.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbEnums.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbEnums.FormattingEnabled = true;
            this.cbEnums.Location = new System.Drawing.Point(152, 0);
            this.cbEnums.Margin = new System.Windows.Forms.Padding(4);
            this.cbEnums.Name = "cbEnums";
            this.cbEnums.Size = new System.Drawing.Size(297, 24);
            this.cbEnums.TabIndex = 0;
            this.cbEnums.SelectedIndexChanged += new System.EventHandler(this.cbEnums_SelectedIndexChanged);
            // 
            // lblCaption
            // 
            this.lblCaption.AutoSize = true;
            this.lblCaption.Location = new System.Drawing.Point(0, 5);
            this.lblCaption.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCaption.Name = "lblCaption";
            this.lblCaption.Size = new System.Drawing.Size(122, 17);
            this.lblCaption.TabIndex = 1;
            this.lblCaption.Text = "Select from enum:";
            this.lblCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // AnyEnumComboSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblCaption);
            this.Controls.Add(this.cbEnums);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "AnyEnumComboSelector";
            this.Size = new System.Drawing.Size(449, 36);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbEnums;
		private System.Windows.Forms.Label lblCaption;
	}
}
