namespace SandiaNationalLaboratories.Hyram {
	partial class TntEquivalenceForm {
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
            this.vaporMassInput = new System.Windows.Forms.TextBox();
            this.massInputLabel = new System.Windows.Forms.Label();
            this.yieldInputLabel = new System.Windows.Forms.Label();
            this.yieldInput = new System.Windows.Forms.TextBox();
            this.netHeatInput = new System.Windows.Forms.TextBox();
            this.netHeatLabel = new System.Windows.Forms.Label();
            this.equivalentMassOutput = new System.Windows.Forms.TextBox();
            this.equivalentMassLabel = new System.Windows.Forms.Label();
            this.equivalentMassUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.netHeatUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.vaporMassUnitSelector = new SandiaNationalLaboratories.Hyram.ValueConverterDropdown();
            this.calculateButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // vaporMassInput
            // 
            this.vaporMassInput.Location = new System.Drawing.Point(339, 10);
            this.vaporMassInput.Name = "vaporMassInput";
            this.vaporMassInput.Size = new System.Drawing.Size(107, 20);
            this.vaporMassInput.TabIndex = 41;
            this.vaporMassInput.TextChanged += new System.EventHandler(this.vaporMassInput_TextChanged);
            // 
            // massInputLabel
            // 
            this.massInputLabel.AutoSize = true;
            this.massInputLabel.Location = new System.Drawing.Point(14, 14);
            this.massInputLabel.Name = "massInputLabel";
            this.massInputLabel.Size = new System.Drawing.Size(161, 13);
            this.massInputLabel.TabIndex = 39;
            this.massInputLabel.Text = "Flammable Vapor Release Mass:";
            // 
            // yieldInputLabel
            // 
            this.yieldInputLabel.AutoSize = true;
            this.yieldInputLabel.Location = new System.Drawing.Point(14, 41);
            this.yieldInputLabel.Name = "yieldInputLabel";
            this.yieldInputLabel.Size = new System.Drawing.Size(167, 13);
            this.yieldInputLabel.TabIndex = 42;
            this.yieldInputLabel.Text = "Explosive Energy Yield (%, 0-100):";
            // 
            // yieldInput
            // 
            this.yieldInput.Location = new System.Drawing.Point(339, 38);
            this.yieldInput.Name = "yieldInput";
            this.yieldInput.Size = new System.Drawing.Size(107, 20);
            this.yieldInput.TabIndex = 43;
            this.yieldInput.TextChanged += new System.EventHandler(this.tbYieldPercentage_TextChanged);
            // 
            // netHeatInput
            // 
            this.netHeatInput.Location = new System.Drawing.Point(339, 66);
            this.netHeatInput.Name = "netHeatInput";
            this.netHeatInput.Size = new System.Drawing.Size(107, 20);
            this.netHeatInput.TabIndex = 46;
            this.netHeatInput.TextChanged += new System.EventHandler(this.temperatureInput_TextChanged);
            // 
            // netHeatLabel
            // 
            this.netHeatLabel.AutoSize = true;
            this.netHeatLabel.Location = new System.Drawing.Point(14, 70);
            this.netHeatLabel.Name = "netHeatLabel";
            this.netHeatLabel.Size = new System.Drawing.Size(123, 13);
            this.netHeatLabel.TabIndex = 44;
            this.netHeatLabel.Text = "Net Heat of Combustion:";
            // 
            // equivalentMassOutput
            // 
            this.equivalentMassOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.equivalentMassOutput.Location = new System.Drawing.Point(339, 121);
            this.equivalentMassOutput.Name = "equivalentMassOutput";
            this.equivalentMassOutput.ReadOnly = true;
            this.equivalentMassOutput.Size = new System.Drawing.Size(107, 20);
            this.equivalentMassOutput.TabIndex = 49;
            // 
            // equivalentMassLabel
            // 
            this.equivalentMassLabel.AutoSize = true;
            this.equivalentMassLabel.Location = new System.Drawing.Point(14, 125);
            this.equivalentMassLabel.Name = "equivalentMassLabel";
            this.equivalentMassLabel.Size = new System.Drawing.Size(113, 13);
            this.equivalentMassLabel.TabIndex = 47;
            this.equivalentMassLabel.Text = "Equivalent TNT Mass:";
            // 
            // equivalentMassUnitSelector
            // 
            this.equivalentMassUnitSelector.Converter = null;
            this.equivalentMassUnitSelector.Location = new System.Drawing.Point(183, 120);
            this.equivalentMassUnitSelector.Name = "equivalentMassUnitSelector";
            this.equivalentMassUnitSelector.SelectedItem = null;
            this.equivalentMassUnitSelector.Size = new System.Drawing.Size(142, 22);
            this.equivalentMassUnitSelector.StoredValue = new double[0];
            this.equivalentMassUnitSelector.TabIndex = 48;
            this.equivalentMassUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.equivalentMassUnitSelector_OnSelectedIndexChanged);
            // 
            // netHeatUnitSelector
            // 
            this.netHeatUnitSelector.Converter = null;
            this.netHeatUnitSelector.Location = new System.Drawing.Point(183, 65);
            this.netHeatUnitSelector.Name = "netHeatUnitSelector";
            this.netHeatUnitSelector.SelectedItem = null;
            this.netHeatUnitSelector.Size = new System.Drawing.Size(142, 22);
            this.netHeatUnitSelector.StoredValue = new double[0];
            this.netHeatUnitSelector.TabIndex = 45;
            this.netHeatUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.netHeatUnitSelector_OnSelectedIndexChanged);
            // 
            // vaporMassUnitSelector
            // 
            this.vaporMassUnitSelector.Converter = null;
            this.vaporMassUnitSelector.Location = new System.Drawing.Point(183, 9);
            this.vaporMassUnitSelector.Name = "vaporMassUnitSelector";
            this.vaporMassUnitSelector.SelectedItem = null;
            this.vaporMassUnitSelector.Size = new System.Drawing.Size(142, 22);
            this.vaporMassUnitSelector.StoredValue = new double[0];
            this.vaporMassUnitSelector.TabIndex = 40;
            this.vaporMassUnitSelector.OnSelectedIndexChanged += new System.EventHandler(this.vaporMassUnitSelector_OnSelectedIndexChanged);
            // 
            // calculateButton
            // 
            this.calculateButton.Enabled = false;
            this.calculateButton.Location = new System.Drawing.Point(339, 92);
            this.calculateButton.Name = "calculateButton";
            this.calculateButton.Size = new System.Drawing.Size(107, 23);
            this.calculateButton.TabIndex = 50;
            this.calculateButton.Text = "Calculate";
            this.calculateButton.UseVisualStyleBackColor = true;
            this.calculateButton.Click += new System.EventHandler(this.calculateButton_Click);
            // 
            // TntEquivalenceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.calculateButton);
            this.Controls.Add(this.equivalentMassOutput);
            this.Controls.Add(this.equivalentMassUnitSelector);
            this.Controls.Add(this.equivalentMassLabel);
            this.Controls.Add(this.netHeatInput);
            this.Controls.Add(this.netHeatUnitSelector);
            this.Controls.Add(this.netHeatLabel);
            this.Controls.Add(this.yieldInput);
            this.Controls.Add(this.yieldInputLabel);
            this.Controls.Add(this.vaporMassInput);
            this.Controls.Add(this.vaporMassUnitSelector);
            this.Controls.Add(this.massInputLabel);
            this.Name = "TntEquivalenceForm";
            this.Size = new System.Drawing.Size(461, 169);
            this.Load += new System.EventHandler(this.cpEtkTNTMassEquiv_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox vaporMassInput;
		private ValueConverterDropdown vaporMassUnitSelector;
		private System.Windows.Forms.Label massInputLabel;
		private System.Windows.Forms.Label yieldInputLabel;
		private System.Windows.Forms.TextBox yieldInput;
		private System.Windows.Forms.TextBox netHeatInput;
		private ValueConverterDropdown netHeatUnitSelector;
		private System.Windows.Forms.Label netHeatLabel;
		private System.Windows.Forms.TextBox equivalentMassOutput;
		private ValueConverterDropdown equivalentMassUnitSelector;
		private System.Windows.Forms.Label equivalentMassLabel;
        private System.Windows.Forms.Button calculateButton;
    }
}
