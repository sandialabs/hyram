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
