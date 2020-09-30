namespace SandiaNationalLaboratories.Hyram {
	partial class ContentPanel {
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
            this.tbNarrative = new System.Windows.Forms.RichTextBox();
            this.ChildPane = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // tbNarrative
            // 
            this.tbNarrative.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbNarrative.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tbNarrative.Dock = System.Windows.Forms.DockStyle.Top;
            this.tbNarrative.Location = new System.Drawing.Point(0, 0);
            this.tbNarrative.Margin = new System.Windows.Forms.Padding(6);
            this.tbNarrative.Name = "tbNarrative";
            this.tbNarrative.ReadOnly = true;
            this.tbNarrative.Size = new System.Drawing.Size(808, 86);
            this.tbNarrative.TabIndex = 4;
            this.tbNarrative.Text = "";
            // 
            // ChildPane
            // 
            this.ChildPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChildPane.Location = new System.Drawing.Point(0, 86);
            this.ChildPane.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.ChildPane.Name = "ChildPane";
            this.ChildPane.Size = new System.Drawing.Size(808, 527);
            this.ChildPane.TabIndex = 6;
            // 
            // ContentPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ChildPane);
            this.Controls.Add(this.tbNarrative);
            this.Name = "ContentPanel";
            this.Size = new System.Drawing.Size(808, 613);
            this.Load += new System.EventHandler(this._ContentPanel_Load);
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox tbNarrative;
		private System.Windows.Forms.Panel ChildPane;
	}
}
