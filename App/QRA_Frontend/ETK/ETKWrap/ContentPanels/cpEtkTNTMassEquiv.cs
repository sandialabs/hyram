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
using DefaultParsing;
using JrConversions;

namespace QRA_Frontend.ETK.ETKWrap.ContentPanels
{
    public partial class CpEtkTntMassEquiv : UserControl
    {
        private MassUnit _mActiveMassOfFlammableVaporUnit = MassUnit.Kilogram;

        private SpecificEnergyUnit _mActiveSpecificEnergyUnit = SpecificEnergyUnit.JouleKg;
        private MassUnit _mActiveTntMassUnit = MassUnit.Kilogram;
        private double _mHeatOfCombustion = double.NaN;
        private double _mMassOfFlammableVapor = double.NaN;
        private double _mTntMassEquivalent = double.NaN;
        private double _mYieldPercentage = double.NaN; // Needs to be converted to fraction before call

        public CpEtkTntMassEquiv()
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
                ddHeatOfCombustion.Converter = StockConverters.GetConverterByName("SpecificEnergy");
                //_mActiveSpecificEnergyUnit = GetDefaultActiveSpecificEnergyUnit();
                _mActiveSpecificEnergyUnit = SpecificEnergyUnit.JouleKg;
                ddHeatOfCombustion.SelectedItem = _mActiveSpecificEnergyUnit;

                ddMassOfFlammableVapor.Converter = StockConverters.GetConverterByName("Mass");
                ddMassOfFlammableVapor.SelectedItem = GetDefaultActiveMassUnit();

                ddMassEquivalent.Converter = StockConverters.GetConverterByName("Mass");
                ddMassEquivalent.SelectedItem = GetDefaultActiveMassUnit();
            }
        }

        private void ddMassEquivalent_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddMassEquivalent.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseMassUnit((string) ddMassEquivalent.SelectedItem);
                _mTntMassEquivalent = ddMassEquivalent.ConvertValue(_mActiveTntMassUnit, newUnit, _mTntMassEquivalent);
                _mActiveTntMassUnit = newUnit;
                if (!double.IsNaN(_mTntMassEquivalent))
                    tbTNTMassEquivalent.Text = Parsing.DoubleToString(_mTntMassEquivalent, "E4");
            }
        }

        private void ddMassOfFlammableVapor_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddMassOfFlammableVapor.SelectedItem != null)
            {
                var newUnit = UnitParser.ParseMassUnit((string) ddMassOfFlammableVapor.SelectedItem);

                _mMassOfFlammableVapor = ddMassOfFlammableVapor.ConvertValue(_mActiveMassOfFlammableVaporUnit,
                    newUnit, _mMassOfFlammableVapor);
                _mActiveMassOfFlammableVaporUnit = newUnit;
                if (!double.IsNaN(_mMassOfFlammableVapor))
                    tbMassOfFlammableVapor.Text = Parsing.DoubleToString(_mMassOfFlammableVapor);
            }
        }


        private void ddHeatOfCombustion_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddHeatOfCombustion.SelectedItem != null)
            {
                var newUnit =
                    UnitParser.ParseSpecificEnergyUnit((string) ddHeatOfCombustion.SelectedItem);

                _mHeatOfCombustion =
                    ddHeatOfCombustion.ConvertValue(_mActiveSpecificEnergyUnit, newUnit, _mHeatOfCombustion);
                _mActiveSpecificEnergyUnit = newUnit;
                if (!double.IsNaN(_mHeatOfCombustion))
                    tbHeatOfCombustion.Text = Parsing.DoubleToString(_mHeatOfCombustion);

                //SetDefaultActiveSpecificEnergyUnit(_mActiveSpecificEnergyUnit);
            }
        }


        private void tbMassOfFlammableVapor_TextChanged(object sender, EventArgs e)
        {
            _mMassOfFlammableVapor = double.NaN;
            Parsing.TryParseDouble(tbMassOfFlammableVapor.Text, out _mMassOfFlammableVapor);
            TryCalculate();
        }


        private void TryCalculate()
        {
            var fail = double.IsNaN(_mMassOfFlammableVapor) || double.IsNaN(_mYieldPercentage) ||
                       double.IsNaN(_mHeatOfCombustion);

            if (!fail)
            {
                var massOfFlammableVaporCu = ddMassOfFlammableVapor.ConvertValue(_mActiveMassOfFlammableVaporUnit,
                    MassUnit.Kilogram, _mMassOfFlammableVapor);
                var heatOfCombustionCu = ddHeatOfCombustion.ConvertValue(_mActiveSpecificEnergyUnit,
                    SpecificEnergyUnit.KjKg, _mHeatOfCombustion);

                _mTntMassEquivalent = massOfFlammableVaporCu * (_mYieldPercentage / 100) * heatOfCombustionCu / 4500;

                if (!double.IsNaN(_mTntMassEquivalent))
                    tbTNTMassEquivalent.Text = Parsing.DoubleToString(_mTntMassEquivalent, "E4");
                else
                    tbTNTMassEquivalent.Text = "NaN";
            }
            else
            {
                tbTNTMassEquivalent.Text = "NaN";
            }
        }


        private void tbTemperature_TextChanged(object sender, EventArgs e)
        {
            _mHeatOfCombustion = double.NaN;
            Parsing.TryParseDouble(tbHeatOfCombustion.Text, out _mHeatOfCombustion);
            TryCalculate();
        }


        private void tbYieldPercentage_TextChanged(object sender, EventArgs e)
        {
            _mYieldPercentage = double.NaN;
            Parsing.TryParseDouble(tbEnergyYield.Text, out _mYieldPercentage);
            TryCalculate();
        }
    }
}