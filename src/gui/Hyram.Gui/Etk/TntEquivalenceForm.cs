// Copyright 2016 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
// Under the terms of Contract DE-NA0003525 with NTESS, the U.S. Government retains certain rights in this software.
// 
// This file is part of HyRAM (Hydrogen Risk Assessment Models).
// 
// HyRAM is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// HyRAM is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with HyRAM.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public partial class TntEquivalenceForm : UserControl
    {
        private MassUnit _mActiveMassOfFlammableVaporUnit = MassUnit.Kilogram;

        private SpecificEnergyUnit _mActiveSpecificEnergyUnit = SpecificEnergyUnit.JouleKg;
        private MassUnit _mActiveTntMassUnit = MassUnit.Kilogram;
        private double _mHeatOfCombustion = double.NaN;
        private double _mMassOfFlammableVapor = double.NaN;
        private double _mTntMassEquivalent = double.NaN;
        private double _mYieldPercentage = double.NaN; // Needs to be converted to fraction before call

        public TntEquivalenceForm()
        {
            InitializeComponent();
        }

        private void cpEtkTNTMassEquiv_Load(object sender, EventArgs e)
        {
            ProcessLoadEvent(sender, e);
        }

        private MassUnit GetDefaultActiveMassUnit()
        {
            return MassUnit.Kilogram;
        }


        private void ProcessLoadEvent(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                netHeatUnitSelector.Converter = StockConverters.GetConverterByName("SpecificEnergy");
                //_mActiveSpecificEnergyUnit = GetDefaultActiveSpecificEnergyUnit();
                _mActiveSpecificEnergyUnit = SpecificEnergyUnit.JouleKg;
                netHeatUnitSelector.SelectedItem = _mActiveSpecificEnergyUnit;

                vaporMassUnitSelector.Converter = StockConverters.GetConverterByName("Mass");
                vaporMassUnitSelector.SelectedItem = GetDefaultActiveMassUnit();

                equivalentMassUnitSelector.Converter = StockConverters.GetConverterByName("Mass");
                equivalentMassUnitSelector.SelectedItem = GetDefaultActiveMassUnit();
            }
        }

        private void equivalentMassUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (equivalentMassUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseMassUnit((string) equivalentMassUnitSelector.SelectedItem);
                _mTntMassEquivalent = equivalentMassUnitSelector.ConvertValue(_mActiveTntMassUnit, newUnit, _mTntMassEquivalent);
                _mActiveTntMassUnit = newUnit;
                if (!double.IsNaN(_mTntMassEquivalent))
                    equivalentMassOutput.Text = ParseUtility.DoubleToString(_mTntMassEquivalent, "E4");
            }
        }

        private void vaporMassUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (vaporMassUnitSelector.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseMassUnit((string) vaporMassUnitSelector.SelectedItem);

                _mMassOfFlammableVapor = vaporMassUnitSelector.ConvertValue(_mActiveMassOfFlammableVaporUnit,
                    newUnit, _mMassOfFlammableVapor);
                _mActiveMassOfFlammableVaporUnit = newUnit;
                if (!double.IsNaN(_mMassOfFlammableVapor))
                    vaporMassInput.Text = ParseUtility.DoubleToString(_mMassOfFlammableVapor);
            }
        }


        private void netHeatUnitSelector_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (netHeatUnitSelector.SelectedItem != null)
            {
                var newUnit =
                    UnitParser.ParseSpecificEnergyUnit((string) netHeatUnitSelector.SelectedItem);

                _mHeatOfCombustion =
                    netHeatUnitSelector.ConvertValue(_mActiveSpecificEnergyUnit, newUnit, _mHeatOfCombustion);
                _mActiveSpecificEnergyUnit = newUnit;
                if (!double.IsNaN(_mHeatOfCombustion))
                    netHeatInput.Text = ParseUtility.DoubleToString(_mHeatOfCombustion);

                //SetDefaultActiveSpecificEnergyUnit(_mActiveSpecificEnergyUnit);
            }
        }


        private void vaporMassInput_TextChanged(object sender, EventArgs e)
        {
            _mMassOfFlammableVapor = double.NaN;
            ParseUtility.TryParseDouble(vaporMassInput.Text, out _mMassOfFlammableVapor);
            TryCalculate();
        }


        private void TryCalculate()
        {
            var fail = double.IsNaN(_mMassOfFlammableVapor) || double.IsNaN(_mYieldPercentage) ||
                       double.IsNaN(_mHeatOfCombustion);

            if (!fail)
            {
                var massOfFlammableVaporCu = vaporMassUnitSelector.ConvertValue(_mActiveMassOfFlammableVaporUnit,
                    MassUnit.Kilogram, _mMassOfFlammableVapor);
                var heatOfCombustionCu = netHeatUnitSelector.ConvertValue(_mActiveSpecificEnergyUnit,
                    SpecificEnergyUnit.KjKg, _mHeatOfCombustion);

                _mTntMassEquivalent = massOfFlammableVaporCu * (_mYieldPercentage / 100) * heatOfCombustionCu / 4500;

                if (!double.IsNaN(_mTntMassEquivalent))
                    equivalentMassOutput.Text = ParseUtility.DoubleToString(_mTntMassEquivalent, "E4");
                else
                    equivalentMassOutput.Text = "NaN";
            }
            else
            {
                equivalentMassOutput.Text = "NaN";
            }
        }


        private void temperatureInput_TextChanged(object sender, EventArgs e)
        {
            _mHeatOfCombustion = double.NaN;
            ParseUtility.TryParseDouble(netHeatInput.Text, out _mHeatOfCombustion);
            TryCalculate();
        }


        private void tbYieldPercentage_TextChanged(object sender, EventArgs e)
        {
            _mYieldPercentage = double.NaN;
            ParseUtility.TryParseDouble(yieldInput.Text, out _mYieldPercentage);
            TryCalculate();
        }
    }
}