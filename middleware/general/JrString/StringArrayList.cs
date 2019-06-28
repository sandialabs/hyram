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

using System.Collections;

namespace EssStringLib
{
    public class StringArrayList : ArrayList
    {
        public string[] ToStringArray()
        {
            var result = (string[]) ToArray(typeof(string));
            return result;
        }

        public void Append(string[] linesToAdd)
        {
            foreach (var thisLine in linesToAdd) Add(thisLine);
        }

        public void Append(StringArrayList listToAppend)
        {
            Append(listToAppend.ToStringArray());
        }
    }
}