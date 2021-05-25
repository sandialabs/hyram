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
using System.Text;
using EssStringLib;

namespace EssStringLib
{
    class FastStringArray 
    {
        private const int ARRAYINCREMENTAMOUNT=10;
        private string[] mDataArray = new string[0];
        private int mLength = 0;
        public int Length{
            get{
                return mLength;
            }
        }

        public void Insert(string[] DataToInsert, int Position)
        {
            Truncate();
            mDataArray = StringUtils.InsertDataIntoArray(mDataArray, DataToInsert, Position);
            mLength = mDataArray.Length;

        }
        public string this[int Index]{
            get{
                CheckIndex(Index);
                return mDataArray[Index];
            }
            set{
                CheckIndex(Index);
                mDataArray[Index] = value;
            }
        }
        private void CheckIndex(int Index){
            if ((Index<0) || Index>=mLength){
                throw new IndexOutOfRangeException("Array index is out of range in FastStringArray.");
            }
        }

        public void Append(string[] ArrayToAppend){
            int ActualLengthNeeded = mLength + ArrayToAppend.Length;
            if(ActualLengthNeeded > mDataArray.Length){
                int Difference = ActualLengthNeeded - mDataArray.Length;
                if (Difference < ARRAYINCREMENTAMOUNT){
                    Expand();
                }
                else{
                    Expand(Difference);
                }
            }

            int DestIndex = mLength;
            for(int SourceIndex=0;SourceIndex<ArrayToAppend.Length;SourceIndex++){
                mDataArray[DestIndex] = ArrayToAppend[SourceIndex];
                DestIndex++;
            }

            mLength += ArrayToAppend.Length;


        }

        public void Delete(int Position, int NumLines){
            
            if (NumLines <= 0)
            {
                throw new Exception("Delete failed.  You have to specify at least one item to delete.");
            }

            int SourceIndex = 0;
            int DestIndex = 0;

            while (SourceIndex < Position)
            {
                mDataArray[DestIndex] = mDataArray[SourceIndex];
                SourceIndex++; DestIndex++;
            }

            SourceIndex += NumLines;

            while (SourceIndex < mLength)
            {
                mDataArray[DestIndex] = mDataArray[SourceIndex];
                SourceIndex++; DestIndex++;
            }

            mLength -= NumLines;
            

        }

        public void Append(string ValueToAppend)
        {
            if(mLength==mDataArray.Length){
                Expand();
            }
            mDataArray[mLength] = ValueToAppend;
            mLength++;
        }
        private void Expand(int Amount){
            
            int ActualLength = mDataArray.Length + Amount;
            string[] NewDataArray = new string[ActualLength];
            mDataArray.CopyTo(NewDataArray, 0);
            mDataArray = NewDataArray;
        
        }

        private void Expand(){
             if (mDataArray.Length == mLength){
                Expand(ARRAYINCREMENTAMOUNT);
             }
        }

        private void Truncate(){
            string[] Result;
            if(mLength==mDataArray.Length){
                Result = mDataArray;
            }
            else{
                Result = new string[mLength];
                for(int Index=0;Index<mLength;Index++){
                    Result[Index] = mDataArray[Index];
                }
                mDataArray = Result;
            }

            mLength = mDataArray.Length;


            
        }

        public string[] Data
        {
            get{
                if(mDataArray.Length>mLength){
                    Truncate();
                }
                return mDataArray;
            }
            set{
                mDataArray = value;
                mLength = mDataArray.Length;
            }

        }
    }
}
