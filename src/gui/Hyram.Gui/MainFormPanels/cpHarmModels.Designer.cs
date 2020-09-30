namespace QRA_Frontend.ContentPanels {
	partial class cpHarmModels {
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
			this.SuspendLayout();
			// 
			// tbNarrative
			// 
			this.tbNarrative.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.tbNarrative.Dock = System.Windows.Forms.DockStyle.Top;
			this.tbNarrative.Location = new System.Drawing.Point(0, 0);
			this.tbNarrative.Name = "tbNarrative";
			this.tbNarrative.Size = new System.Drawing.Size(510, 119);
			this.tbNarrative.TabIndex = 9;
			this.tbNarrative.Text = "";
			// 
			// cpHarmModels
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbNarrative);
			this.Name = "cpHarmModels";
			this.Size = new System.Drawing.Size(510, 270);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.RichTextBox tbNarrative;
	}
}
