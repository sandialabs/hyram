using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandiaNationalLaboratories.Hyram
{
    class UncertainParameter
    {
    }


    /// <summary>
    /// Represents displayed parameter in grid view
    /// </summary>
    public class UncertainParameterInput
    {
        public Parameter Parameter { get; set; }
        public string Label { get; }
        public UnitOfMeasurementConverters Converter { get; set; }
        public double OriginalValue { get; set; }
        public bool IsUncertain { get; set; }

        public Distribution Distr { get; set; }
        public double DistARef { get; set; }
        public double DistBRef { get; set; }

        public UncertainParameterInput(Parameter param, Distribution distr, string label = "", bool isUncertain = false)
        {
            Parameter = param;
            Converter = param.UnitConverters;
            Label = label == "" ? param.Label : label;

            Distr = distr;
            IsUncertain = isUncertain;
        }


//        public static List<UncertainParameterInput> GetUncertainParameterInputList(Parameter[] parameters)
//        {
//            List<UncertainParameterInput> result = new List<UncertainParameterInput>();
//
//            for (int i = 0; i < parameters.Length; i++)
//            {
//                result.Add(new UncertainParameterInput(parameters[i]));
//            }
//            return result;
//        }
    }


}
