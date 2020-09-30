using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using imported_matlab_models;
using jr_matlab_compat;
using EssStringLib;


namespace QRA_Frontend.ContentPanels {
	public partial class cpBetaStatTest:UserControl, IQraBaseNotify {
		public cpBetaStatTest() {
			InitializeComponent();
		}

		private void cpBetaStatTest_Load(object sender, EventArgs e) {
			this.tbNarrative.Text = @"test
a = 1:6;
b = 1:0.2:2;
[m, v] = betastat (a, b);
expected_m = [0.5000, 0.6250, 0.6818, 0.7143, 0.7353, 0.7500];
expected_v = [0.0833, 0.0558, 0.0402, 0.0309, 0.0250, 0.0208];
assert (m, expected_m, 0.001);
assert (v, expected_v, 0.001);

test
a = 1:6;
[m, v] = betastat (a, 1.5);
expected_m = [0.4000, 0.5714, 0.6667, 0.7273, 0.7692, 0.8000];
expected_v = [0.0686, 0.0544, 0.0404, 0.0305, 0.0237, 0.0188];
assert (m, expected_m, 0.001);
assert (v, expected_v, 0.001);";
		}

		private void btnTest_Click(object sender, EventArgs e) {
			double[] a = null;

			if(!string.IsNullOrWhiteSpace(tbA.Text)) {
				a=MatlabCompat.CreateArray(tbA.Text);
			}
			double[] b = null;
			if(!string.IsNullOrWhiteSpace(TbB.Text)) {
				b = MatlabCompat.CreateArray(TbB.Text);
			}
			
			if(b.Length < a.Length && b.Length==1) {
				double holder = b[0];
				b = new double[a.Length];
				for(int index = 0;index < b.Length;index++) {
					b[index] = holder;
				}
			}
			

			if(!(a == null || b == null)) {
				double[] m = null; double[] v = null;

				GplBetastat.betastat(a, b, out m, out v);

				tbM.Text = ArrayFunctions.JoinDoublesToDelimitedString(m);
				tbV.Text = ArrayFunctions.JoinDoublesToDelimitedString(v);
			}
			


			


			
		}

		

		void IQraBaseNotify.Notify_LoadComplete() {
			
		}
	}
}
