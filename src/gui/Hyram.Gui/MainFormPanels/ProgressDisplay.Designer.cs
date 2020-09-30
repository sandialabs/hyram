namespace SandiaNationalLaboratories.Hyram
{
    partial class ProgressDisplay
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
            this.MainProgressBar = new System.Windows.Forms.ProgressBar();
            this.MainProgressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // MainProgressBar
            // 
            this.MainProgressBar.BackColor = System.Drawing.Color.White;
            this.MainProgressBar.Location = new System.Drawing.Point(170, 251);
            this.MainProgressBar.MarqueeAnimationSpeed = 50;
            this.MainProgressBar.Name = "MainProgressBar";
            this.MainProgressBar.Size = new System.Drawing.Size(542, 41);
            this.MainProgressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.MainProgressBar.TabIndex = 0;
            this.MainProgressBar.Value = 5;
            // 
            // MainProgressLabel
            // 
            this.MainProgressLabel.AutoSize = true;
            this.MainProgressLabel.BackColor = System.Drawing.SystemColors.Control;
            this.MainProgressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainProgressLabel.Location = new System.Drawing.Point(165, 223);
            this.MainProgressLabel.MinimumSize = new System.Drawing.Size(110, 25);
            this.MainProgressLabel.Name = "MainProgressLabel";
            this.MainProgressLabel.Size = new System.Drawing.Size(110, 25);
            this.MainProgressLabel.TabIndex = 1;
            this.MainProgressLabel.Text = "msg";
            // 
            // ProgressDisplay
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.MainProgressLabel);
            this.Controls.Add(this.MainProgressBar);
            this.Name = "ProgressDisplay";
            this.Size = new System.Drawing.Size(863, 614);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar MainProgressBar;
        private System.Windows.Forms.Label MainProgressLabel;
    }
}
