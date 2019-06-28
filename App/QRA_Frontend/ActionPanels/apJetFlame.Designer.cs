namespace QRA_Frontend.ActionPanels
{
    partial class ApJetFlame
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this.btnPlotT = new System.Windows.Forms.Button();
			this.btnQrad = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnPlotT
			// 
			this.btnPlotT.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnPlotT.Location = new System.Drawing.Point(0, 5);
			this.btnPlotT.Name = "btnPlotT";
			this.btnPlotT.Size = new System.Drawing.Size(230, 23);
			this.btnPlotT.TabIndex = 3;
			this.btnPlotT.Text = "Flame Temperature / Trajectory";
			this.btnPlotT.UseVisualStyleBackColor = true;
			this.btnPlotT.Click += new System.EventHandler(this.btnPlotT_Click);
			// 
			// btnQrad
			// 
			this.btnQrad.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.btnQrad.Location = new System.Drawing.Point(0, 36);
			this.btnQrad.Name = "btnQrad";
			this.btnQrad.Size = new System.Drawing.Size(230, 23);
			this.btnQrad.TabIndex = 5;
			this.btnQrad.Text = "Radiative Heat Flux";
			this.btnQrad.UseVisualStyleBackColor = true;
			this.btnQrad.Click += new System.EventHandler(this.btnQrad_Click);
			// 
			// apJetFlame
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.btnQrad);
			this.Controls.Add(this.btnPlotT);
			this.Name = "ApJetFlame";
			this.Size = new System.Drawing.Size(233, 63);
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button btnPlotT;
		private System.Windows.Forms.Button btnQrad;
    }
}
