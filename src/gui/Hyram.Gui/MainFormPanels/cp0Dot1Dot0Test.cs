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
using QRAState;
using JrConversions;
using QRA_Frontend;


namespace QRA_Frontend.ContentPanels {


	public partial class cp0Dot1Dot0Test:UserControl, IQraBaseNotify {
		
		private StatesToConsider mStatesToConsider;

		public cp0Dot1Dot0Test() {
			InitializeComponent();
		}

		void IQraBaseNotify.Notify_LoadComplete() {
			GlobalDataRetrievalOptionChanged();

			FillSharedInputs();

			
		}

		private void FillSharedInputs() {
			FillSysParamTab();
			FillVehiclesTab();
			

		}
		private void FillVehiclesTab() {
			FillScalarSharedTB(tbNvehicles, UnitlessUnit.Unitless, "g");
			FillScalarSharedTB(tbNrFuelingsPerVehicleDay, UnitlessUnit.Unitless, "g");
			FillScalarSharedTB(tbNVehicleOperatingDays, JulianTimeConversionUnit.Day, "g");

		}

		private void FillSysParamTab() {
			FillScalarSharedTB(tbExternalPresMPa, PressureUnit.MPa);
			FillScalarSharedTB(tbExternalTempC, TempUnit.Celsius);
			FillScalarSharedTB(tbInternalPresMPa, PressureUnit.MPa);
			FillScalarSharedTB(tbInternalTempC, TempUnit.Celsius);
			FillScalarSharedTB(tbPipeOD, DistanceUnit.Inch);
			FillScalarSharedTB(tbPipeWallThick, DistanceUnit.Inch);
		}
	
		private void FillUserParamsFromScalarTB(TextBox TheTextbox, UnitOfMeasurementConverters Converter, Enum TheUnit) {
			double TbValue = double.NaN;

			

			if(double.TryParse(TheTextbox.Text, out TbValue)) {
				string Key = ((string)TheTextbox.Tag).ToUpper();
				QraStateContainer.Instance.UserSessionValues[Key] = new ndConvertibleValue(Converter, TheUnit, new double[1] {TbValue});
								

			
				

			}
		}

		private void FillScalarSharedTB(TextBox TbToSet, Enum SourceValueUnit, string Format="F4") {
			if(TbToSet.Tag == null) {
				throw new Exception(TbToSet.Name + " tag is not set.");

			}

			string Key = (string)TbToSet.Tag;
			

			ndConvertibleValue Value= QraStateContainer.Instance.GetStateDefinedValueObject(Key, mStatesToConsider);
			string DestinationUnit = SourceValueUnit.ToString();

			double[] Values=Value.GetValue(DestinationUnit);
			TbToSet.Text = Values[0].ToString(Format);
				
			
		}

		private void rbUseDefaults_CheckedChanged(object sender, EventArgs e) {
			GlobalDataRetrievalOptionChanged();
		}

		private void GlobalDataRetrievalOptionChanged() {
			mStatesToConsider =StatesToConsider.Defaults | StatesToConsider.Constants;
			if(rbUseMostCurrent.Checked) {
				mStatesToConsider |= StatesToConsider.User;
				btnRefreshShared.Enabled = false;
			}
			else {
				btnRefreshShared.Enabled = true;
			}
		}

		private void rbUseMostCurrent_CheckedChanged(object sender, EventArgs e) {
			GlobalDataRetrievalOptionChanged();
		}

		private void btnRefreshShared_Click(object sender, EventArgs e) {
			FillSharedInputs();
		}

		private void tbPipeOD_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbPipeOD, StockConverters.DistanceConverter, DistanceUnit.Inch);

		}
		private void tbPipeWallThick_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbPipeWallThick, StockConverters.DistanceConverter, DistanceUnit.Inch);
		}

		private void tbInternalPresMPa_TextChanged(object sender, EventArgs e) {
		
			FillUserParamsFromScalarTB(tbInternalPresMPa, StockConverters.PressureConverter, PressureUnit.MPa);

		}

		private void tbInternalTempC_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbInternalTempC, StockConverters.TemperatureConverter, TempUnit.Celsius);
		}

		private void tbExternalPresMPa_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbExternalPresMPa, StockConverters.PressureConverter, PressureUnit.MPa);

		}

		private void tbExternalTempC_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbExternalTempC, StockConverters.TemperatureConverter, TempUnit.Celsius);
		}
		private int mnVehicles = 0;
		private void tbNvehicles_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbNvehicles, StockConverters.UnitlessConverter, UnitlessUnit.Unitless);
			int.TryParse(tbNvehicles.Text, out mnVehicles);

		}

		private int mNrFuelingsPerVehicleDay = 0;

		private void tbNrFuelingsPerVehicleDay_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbNrFuelingsPerVehicleDay, StockConverters.UnitlessConverter, UnitlessUnit.Unitless);
			int.TryParse(tbNrFuelingsPerVehicleDay.Text, out mNrFuelingsPerVehicleDay);
		}

		private int mNVehicleOperatingDays = 0;
		private void tbNVehicleOperatingDays_TextChanged(object sender, EventArgs e) {
			FillUserParamsFromScalarTB(tbNVehicleOperatingDays, StockConverters.JulianTimeConverter, JulianTimeConversionUnit.Day);
			int.TryParse(tbNVehicleOperatingDays.Text, out mNVehicleOperatingDays);

		}

		private void btnExecute_Click(object sender, EventArgs e) {
			ReleaseFreq_m ReleaseFreqProvider=new ReleaseFreq_m();
			StockLeakFreqData LeakFreqData = LeakFreqIO.GetStockLeakFreqDataObj();
			double[] ReleaseFreqs = ReleaseFreqProvider.Execute(LeakFreqData, mnVehicles, mNrFuelingsPerVehicleDay, mNVehicleOperatingDays);
			// TODO: Fix bug -- ReleaseFreqs is hardcoded.
			ReleaseFreqs = new double[] {0.0348D, 0.050D, 0.0015D, 0.0012D, 0.0008D };
			
		}

		

	
	}
}
