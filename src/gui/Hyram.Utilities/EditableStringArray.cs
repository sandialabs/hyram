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

        /// Insert an array into the Data Array.
        /// </summary>
        /// <param name="linesToInsert">An array containing the data to insert.</param>
        /// <param name="beforeIndex">Point at which to make insertion.</param>
        public void Insert(string[] linesToInsert, int beforeIndex)
        {
            Data = StringFunctions.InsertDataIntoArray(Data, linesToInsert, beforeIndex);
        }

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
        ///     Inserts one line into Data Array.
        /// </summary>
        /// <param name="lineToInsert">Line of data to insert.</param>
        /// <param name="beforeIndex">Index to insert lines before.</param>
        public void Insert(string lineToInsert, int beforeIndex)
        {
            var insertLines = new string[1];
            insertLines[0] = lineToInsert;
            Insert(insertLines, beforeIndex);
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