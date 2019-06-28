namespace QRA_Frontend.ETK.ETKWrap.ContentPanels {
	partial class CpEtkTntMassEquiv {
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
			this.tbMassOfFlammableVapor = new System.Windows.Forms.TextBox();
			this.lblMass = new System.Windows.Forms.Label();
			this.lblYield = new System.Windows.Forms.Label();
			this.tbEnergyYield = new System.Windows.Forms.TextBox();
			this.tbHeatOfCombustion = new System.Windows.Forms.TextBox();
			this.lblCombustionTemperature = new System.Windows.Forms.Label();
			this.tbTNTMassEquivalent = new System.Windows.Forms.TextBox();
			this.lblEquivalentTNTMass = new System.Windows.Forms.Label();
			this.ddMassEquivalent = new QRA_Frontend.ValueConverterDropdown();
			this.ddHeatOfCombustion = new QRA_Frontend.ValueConverterDropdown();
			this.ddMassOfFlammableVapor = new QRA_Frontend.ValueConverterDropdown();
			this.SuspendLayout();
			// 
			// tbMassOfFlammableVapor
			// 
			this.tbMassOfFlammableVapor.Location = new System.Drawing.Point(339, 10);
			this.tbMassOfFlammableVapor.Name = "tbMassOfFlammableVapor";
			this.tbMassOfFlammableVapor.Size = new System.Drawing.Size(107, 20);
			this.tbMassOfFlammableVapor.TabIndex = 41;
			this.tbMassOfFlammableVapor.TextChanged += new System.EventHandler(this.tbMassOfFlammableVapor_TextChanged);
			// 
			// lblMass
			// 
			this.lblMass.AutoSize = true;
			this.lblMass.Location = new System.Drawing.Point(14, 14);
			this.lblMass.Name = "lblMass";
			this.lblMass.Size = new System.Drawing.Size(161, 13);
			this.lblMass.TabIndex = 39;
			this.lblMass.Text = "Flammable Vapor Release Mass:";
			// 
			// lblYield
			// 
			this.lblYield.AutoSize = true;
			this.lblYield.Location = new System.Drawing.Point(153, 42);
			this.lblYield.Name = "lblYield";
			this.lblYield.Size = new System.Drawing.Size(167, 13);
			this.lblYield.TabIndex = 42;
			this.lblYield.Text = "Explosive Energy Yield (%, 0-100):";
			// 
			// tbEnergyYield
			// 
			this.tbEnergyYield.Location = new System.Drawing.Point(339, 38);
			this.tbEnergyYield.Name = "tbEnergyYield";
			this.tbEnergyYield.Size = new System.Drawing.Size(107, 20);
			this.tbEnergyYield.TabIndex = 43;
			this.tbEnergyYield.TextChanged += new System.EventHandler(this.tbYieldPercentage_TextChanged);
			// 
			// tbHeatOfCombustion
			// 
			this.tbHeatOfCombustion.Location = new System.Drawing.Point(339, 66);
			this.tbHeatOfCombustion.Name = "tbHeatOfCombustion";
			this.tbHeatOfCombustion.Size = new System.Drawing.Size(107, 20);
			this.tbHeatOfCombustion.TabIndex = 46;
			this.tbHeatOfCombustion.TextChanged += new System.EventHandler(this.tbTemperature_TextChanged);
			// 
			// lblCombustionTemperature
			// 
			this.lblCombustionTemperature.AutoSize = true;
			this.lblCombustionTemperature.Location = new System.Drawing.Point(14, 70);
			this.lblCombustionTemperature.Name = "lblCombustionTemperature";
			this.lblCombustionTemperature.Size = new System.Drawing.Size(123, 13);
			this.lblCombustionTemperature.TabIndex = 44;
			this.lblCombustionTemperature.Text = "Net Heat of Combustion:";
			// 
			// tbTNTMassEquivalent
			// 
			this.tbTNTMassEquivalent.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbTNTMassEquivalent.Location = new System.Drawing.Point(339, 95);
			this.tbTNTMassEquivalent.Name = "tbTNTMassEquivalent";
			this.tbTNTMassEquivalent.ReadOnly = true;
			this.tbTNTMassEquivalent.Size = new System.Drawing.Size(107, 20);
			this.tbTNTMassEquivalent.TabIndex = 49;
			// 
			// lblEquivalentTNTMass
			// 
			this.lblEquivalentTNTMass.AutoSize = true;
			this.lblEquivalentTNTMass.Location = new System.Drawing.Point(14, 99);
			this.lblEquivalentTNTMass.Name = "lblEquivalentTNTMass";
			this.lblEquivalentTNTMass.Size = new System.Drawing.Size(113, 13);
			this.lblEquivalentTNTMass.TabIndex = 47;
			this.lblEquivalentTNTMass.Text = "Equivalent TNT Mass:";
			// 
			// ddMassEquivalent
			// 
			this.ddMassEquivalent.Converter = null;
			this.ddMassEquivalent.Location = new System.Drawing.Point(183, 94);
			this.ddMassEquivalent.Name = "ddMassEquivalent";
			this.ddMassEquivalent.SelectedItem = null;
			this.ddMassEquivalent.Size = new System.Drawing.Size(142, 22);
			this.ddMassEquivalent.StoredValue = new double[0];
			this.ddMassEquivalent.TabIndex = 48;
			this.ddMassEquivalent.OnSelectedIndexChanged += new System.EventHandler(this.ddMassEquivalent_OnSelectedIndexChanged);
			// 
			// ddHeatOfCombustion
			// 
			this.ddHeatOfCombustion.Converter = null;
			this.ddHeatOfCombustion.Location = new System.Drawing.Point(183, 65);
			this.ddHeatOfCombustion.Name = "ddHeatOfCombustion";
			this.ddHeatOfCombustion.SelectedItem = null;
			this.ddHeatOfCombustion.Size = new System.Drawing.Size(142, 22);
			this.ddHeatOfCombustion.StoredValue = new double[0];
			this.ddHeatOfCombustion.TabIndex = 45;
			this.ddHeatOfCombustion.OnSelectedIndexChanged += new System.EventHandler(this.ddHeatOfCombustion_OnSelectedIndexChanged);
			// 
			// ddMassOfFlammableVapor
			// 
			this.ddMassOfFlammableVapor.Converter = null;
			this.ddMassOfFlammableVapor.Location = new System.Drawing.Point(183, 9);
			this.ddMassOfFlammableVapor.Name = "ddMassOfFlammableVapor";
			this.ddMassOfFlammableVapor.SelectedItem = null;
			this.ddMassOfFlammableVapor.Size = new System.Drawing.Size(142, 22);
			this.ddMassOfFlammableVapor.StoredValue = new double[0];
			this.ddMassOfFlammableVapor.TabIndex = 40;
			this.ddMassOfFlammableVapor.OnSelectedIndexChanged += new System.EventHandler(this.ddMassOfFlammableVapor_OnSelectedIndexChanged);
			// 
			// cpEtkTNTMassEquiv
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tbTNTMassEquivalent);
			this.Controls.Add(this.ddMassEquivalent);
			this.Controls.Add(this.lblEquivalentTNTMass);
			this.Controls.Add(this.tbHeatOfCombustion);
			this.Controls.Add(this.ddHeatOfCombustion);
			this.Controls.Add(this.lblCombustionTemperature);
			this.Controls.Add(this.tbEnergyYield);
			this.Controls.Add(this.lblYield);
			this.Controls.Add(this.tbMassOfFlammableVapor);
			this.Controls.Add(this.ddMassOfFlammableVapor);
			this.Controls.Add(this.lblMass);
			this.Name = "CpEtkTntMassEquiv";
			this.Size = new System.Drawing.Size(461, 133);
			this.Load += new System.EventHandler(this.cpEtkTNTMassEquiv_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbMassOfFlammableVapor;
		private ValueConverterDropdown ddMassOfFlammableVapor;
		private System.Windows.Forms.Label lblMass;
		private System.Windows.Forms.Label lblYield;
		private System.Windows.Forms.TextBox tbEnergyYield;
		private System.Windows.Forms.TextBox tbHeatOfCombustion;
		private ValueConverterDropdown ddHeatOfCombustion;
		private System.Windows.Forms.Label lblCombustionTemperature;
		private System.Windows.Forms.TextBox tbTNTMassEquivalent;
		private ValueConverterDropdown ddMassEquivalent;
		private System.Windows.Forms.Label lblEquivalentTNTMass;
	}
}
