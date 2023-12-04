using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SandiaNationalLaboratories.Hyram
{
    public class FormHelpers
    {
        public static void HandleNullableDoubleInputChange(TextBox input, object sender, EventArgs e, out double? stateField)
        {
            stateField = 0;
            if (double.TryParse(input.Text, out double result))
            {
                stateField = result;
            }
            else
            {
                input.Text = stateField.ToString();
            }
        }

        public static void HandleDoubleInputChange(TextBox input, object sender, EventArgs e, out double stateField)
        {
            stateField = 0;
            if (double.TryParse(input.Text, out double result))
            {
                stateField = result;
            }
            else
            {
                input.Text = stateField.ToString();
            }
        }

        public static void HandleParameterValueChange(TextBox input, object sender, EventArgs e, Parameter param)
        {
            if (double.TryParse(input.Text, out double result))
            {
                param.SetValueFromDisplay(result);
            }
            else
            {
                input.Text = param.GetDisplayValue().ToString();
            }
        }

        public static void HandleParameterParamAChange(TextBox input, object sender, EventArgs e, Parameter param)
        {
            if (string.IsNullOrEmpty(input.Text))
            {
                param.SetParamAFromDisplay(null);
                return;
            }

            if (double.TryParse(input.Text, out double result))
            {
                param.SetParamAFromDisplay(result);
            }
            else
            {
                input.Text = param.GetDisplayValue().ToString();
            }
        }

        public static void HandleParameterParamBChange(TextBox input, object sender, EventArgs e, Parameter param)
        {
            if (string.IsNullOrEmpty(input.Text))
            {
                param.SetParamBFromDisplay(null);
                return;
            }

            if (double.TryParse(input.Text, out double result))
            {
                param.SetParamBFromDisplay(result);
            }
            else
            {
                input.Text = param.GetDisplayValue().ToString();
            }
        }
    }
}
