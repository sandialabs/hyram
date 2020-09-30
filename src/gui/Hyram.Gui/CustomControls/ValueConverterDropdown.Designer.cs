namespace SandiaNationalLaboratories.Hyram {
	partial class ValueConverterDropdown {

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            this.cbMain = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cbMain
            // 
            this.cbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbMain.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbMain.FormattingEnabled = true;
            this.cbMain.Location = new System.Drawing.Point(0, 0);
            this.cbMain.Name = "cbMain";
            this.cbMain.Size = new System.Drawing.Size(193, 21);
            this.cbMain.TabIndex = 0;
            this.cbMain.SelectedIndexChanged += new System.EventHandler(this.cbMain_SelectedIndexChanged);
            // 
            // ValueConverterDropdown
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbMain);
            this.Name = "ValueConverterDropdown";
            this.Size = new System.Drawing.Size(193, 22);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox cbMain;
	}
}
