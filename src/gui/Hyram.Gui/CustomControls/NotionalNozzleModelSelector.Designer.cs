namespace SandiaNationalLaboratories.Hyram {
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
            this.notionalNozzleSelector = new System.Windows.Forms.ComboBox();
            this.notionalNozzleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // notionalNozzleSelector
            // 
            this.notionalNozzleSelector.Dock = System.Windows.Forms.DockStyle.Right;
            this.notionalNozzleSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.notionalNozzleSelector.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notionalNozzleSelector.FormattingEnabled = true;
            this.notionalNozzleSelector.Items.AddRange(new object[] {
            "Birch",
            "Birch2",
            "Ewan/Moodie",
            "Harstad/Bellan",
            "Molkov",
            "Yuceil/Otugen"});
            this.notionalNozzleSelector.Location = new System.Drawing.Point(220, 0);
            this.notionalNozzleSelector.Name = "notionalNozzleSelector";
            this.notionalNozzleSelector.Size = new System.Drawing.Size(221, 23);
            this.notionalNozzleSelector.TabIndex = 7;
            this.notionalNozzleSelector.SelectedIndexChanged += new System.EventHandler(this.notionalNozzleSelector_SelectedIndexChanged);
            // 
            // notionalNozzleLabel
            // 
            this.notionalNozzleLabel.AutoSize = true;
            this.notionalNozzleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.notionalNozzleLabel.Location = new System.Drawing.Point(1, 6);
            this.notionalNozzleLabel.Name = "notionalNozzleLabel";
            this.notionalNozzleLabel.Size = new System.Drawing.Size(135, 15);
            this.notionalNozzleLabel.TabIndex = 8;
            this.notionalNozzleLabel.Text = "Notional Nozzle Model:";
            // 
            // NotionalNozzleModelSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.notionalNozzleSelector);
            this.Controls.Add(this.notionalNozzleLabel);
            this.Name = "NotionalNozzleModelSelector";
            this.Size = new System.Drawing.Size(441, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ComboBox notionalNozzleSelector;
		private System.Windows.Forms.Label notionalNozzleLabel;
	}
}
