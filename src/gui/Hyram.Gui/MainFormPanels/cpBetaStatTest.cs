/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC ("NTESS").

Under the terms of Contract DE-AC04-94AL85000, there is a non-exclusive license
for use of this work by or on behalf of the U.S. Government.  Export of this
data may require a license from the United States Government. For five (5)
years from 2/16/2016, the United States Government is granted for itself and
others acting on its behalf a paid-up, nonexclusive, irrevocable worldwide
license in this data to reproduce, prepare derivative works, and perform
publicly and display publicly, by or on behalf of the Government. There
is provision for the possible extension of the term of this license. Subsequent
to that period or any extension granted, the United States Government is
granted for itself and others acting on its behalf a paid-up, nonexclusive,
irrevocable worldwide license in this data to reproduce, prepare derivative
works, distribute copies to the public, perform publicly and display publicly,
and to permit others to do so. The specific term of the license can be
identified by inquiry made to NTESS or DOE.

NEITHER THE UNITED STATES GOVERNMENT, NOR THE UNITED STATES DEPARTMENT OF
ENERGY, NOR NTESS, NOR ANY OF THEIR EMPLOYEES, MAKES ANY WARRANTY, EXPRESS
OR IMPLIED, OR ASSUMES ANY LEGAL RESPONSIBILITY FOR THE ACCURACY, COMPLETENESS,
OR USEFULNESS OF ANY INFORMATION, APPARATUS, PRODUCT, OR PROCESS DISCLOSED, OR
REPRESENTS THAT ITS USE WOULD NOT INFRINGE PRIVATELY OWNED RIGHTS.

Any licensee of HyRAM (Hydrogen Risk Assessment Models) v. 3.1 has the
obligation and responsibility to abide by the applicable export control laws,
regulations, and general prohibitions relating to the export of technical data.
Failure to obtain an export control license or other authority from the
Government may result in criminal liability under U.S. laws.

You should have received a copy of the GNU General Public License along with
HyRAM. If not, see <https://www.gnu.org/licenses/>.
*/

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
