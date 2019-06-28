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
using QRAState;
using QRA_Frontend;
using UIHelpers;

namespace QRA_Frontend.ContentPanels {
	public partial class cpHarmModels:UserControl, IQraBaseNotify {
		public cpHarmModels() {
			InitializeComponent();
		}

		void IQraBaseNotify.Notify_LoadComplete() {

			SetupUI();
		}

		private void SetupUI() {
			UIStateRoutines.SetNarrative(tbNarrative, BackColor, QRA_Frontend.Resources.Narratives.harm_models);
			SetDropdownDefaults();

		}

		private void SetDropdownDefaults() {
		



		}

	
	}
}
