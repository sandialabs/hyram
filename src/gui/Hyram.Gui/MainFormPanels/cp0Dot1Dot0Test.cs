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
