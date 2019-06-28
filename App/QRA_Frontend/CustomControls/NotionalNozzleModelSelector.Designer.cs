namespace QRA_Frontend.CustomControls {
	partial class NotionalNozzleModelSelector {
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
            this.cbNotionalNozzleModel = new System.Windows.Forms.ComboBox();
            this.lblNotionalNozzleModel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cbNotionalNozzleModel
            // 
            this.cbNotionalNozzleModel.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbNotionalNozzleModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbNotionalNozzleModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbNotionalNozzleModel.FormattingEnabled = true;
            this.cbNotionalNozzleModel.Items.AddRange(new object[] {
            "Birch",
            "Birch2",
            "Ewan/Moodie",
            "Harstad/Bellan",
            "Molkov",
            "Yuceil/Otugen"});
            this.cbNotionalNozzleModel.Location = new System.Drawing.Point(220, 0);
            this.cbNotionalNozzleModel.Name = "cbNotionalNozzleModel";
            this.cbNotionalNozzleModel.Size = new System.Drawing.Size(221, 23);
            this.cbNotionalNozzleModel.TabIndex = 7;
            this.cbNotionalNozzleModel.SelectedIndexChanged += new System.EventHandler(this.cbNotionalNozzleModel_SelectedIndexChanged);
            // 
            // lblNotionalNozzleModel
            // 
            this.lblNotionalNozzleModel.AutoSize = true;
            this.lblNotionalNozzleModel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNotionalNozzleModel.Location = new System.Drawing.Point(1, 6);
            this.lblNotionalNozzleModel.Name = "lblNotionalNozzleModel";
            this.lblNotionalNozzleModel.Size = new System.Drawing.Size(135, 15);
            this.lblNotionalNozzleModel.TabIndex = 8;
            this.lblNotionalNozzleModel.Text = "Notional Nozzle Model:";
            // 
            // NotionalNozzleModelSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cbNotionalNozzleModel);
            this.Controls.Add(this.lblNotionalNozzleModel);
            this.Name = "NotionalNozzleModelSelector";
            this.Size = new System.Drawing.Size(441, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox cbNotionalNozzleModel;
		private System.Windows.Forms.Label lblNotionalNozzleModel;
	}
}
