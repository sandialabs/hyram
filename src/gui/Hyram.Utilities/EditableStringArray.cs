/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

namespace SandiaNationalLaboratories.Hyram
{
    public enum ArrayStringConversionOption
    {
        AppendCarriageReturn,
        NoModifications,
        AppendCrlf
    }

    /// <summary>
    ///     Encapsulates a string array that can be resized, searched and manipulated without
    ///     having to call external functions or assign the result of a method call on one array
    ///     to a new one.  The Data property contains the array.
    /// </summary>
    public class EditableStringArray
    {

        /// <summary>
        ///     Create a new instance by referencing a preexisting string array.
        /// </summary>
        /// <param name="startingData">Data to reference.</param>
        public EditableStringArray(string[] startingData)
        {
            Data = startingData;
        }

        /// <summary>
        ///     Default constructor.  A new array with zero elements is created.
        /// </summary>
        public EditableStringArray()
        {
        }

        public string this[int index]
        {
            get => Data[index];
            set => Data[index] = value;
        }

        /// <summary>
        ///     The string array that this class operates upon.
        /// </summary>
        public string[] Data { get; set; } = new string[0];

        public string CombineToString(ArrayStringConversionOption conversionOption)
        {
            var result = "";
            var appendValue = "";

            if (conversionOption == ArrayStringConversionOption.AppendCarriageReturn)
                appendValue = "\r";
            else if (conversionOption == ArrayStringConversionOption.AppendCrlf) appendValue = "\r\n";

            foreach (var thisLine in Data)
                if (result != "")
                    result += appendValue + thisLine;
                else
                    result += thisLine;

            return result;
        }

        /// <summary>
        ///     Add a new line to the end of the Data Array.
        /// </summary>
        /// <param name="elementToAppend">New line to append.</param>
        public void Append(string elementToAppend)
        {
            var elem = new string[1];
            elem[0] = elementToAppend;
            Append(elem);
        }

        /// <summary>
        ///     Add an array of lines to the end of the Data Array.
        /// </summary>
        /// <param name="dataToAppend">The array of lines to append.</param>
        public void Append(string[] dataToAppend)
        {
            var newData = new string[dataToAppend.Length + Data.Length];
            Data.CopyTo(newData, 0);
            dataToAppend.CopyTo(newData, Data.Length);
            Data = newData;
        }
    }
}