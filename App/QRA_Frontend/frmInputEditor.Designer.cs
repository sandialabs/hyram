namespace QRA_Frontend {
    partial class FrmInputEditor {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmInputEditor));
            this.tlpInputGrids = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tlpInputGrids
            // 
            this.tlpInputGrids.AutoScroll = true;
            this.tlpInputGrids.AutoScrollMargin = new System.Drawing.Size(3, 3);
            this.tlpInputGrids.ColumnCount = 1;
            this.tlpInputGrids.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpInputGrids.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpInputGrids.Location = new System.Drawing.Point(0, 0);
            this.tlpInputGrids.Name = "tlpInputGrids";
            this.tlpInputGrids.RowCount = 1;
            this.tlpInputGrids.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpInputGrids.Size = new System.Drawing.Size(850, 383);
            this.tlpInputGrids.TabIndex = 0;
            this.tlpInputGrids.Scroll += new System.Windows.Forms.ScrollEventHandler(this.tlpInputGrids_Scroll);
            this.tlpInputGrids.Resize += new System.EventHandler(this.tlpInputGrids_Resize);
            // 
            // frmInputEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(850, 383);
            this.Controls.Add(this.tlpInputGrids);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmInputEditor";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Input Editor";
            this.Activated += new System.EventHandler(this.frmInputEditor_Activated);
            this.ResizeBegin += new System.EventHandler(this.frmInputEditor_ResizeBegin);
            this.ResizeEnd += new System.EventHandler(this.frmInputEditor_ResizeEnd);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tlpInputGrids;



    }
}