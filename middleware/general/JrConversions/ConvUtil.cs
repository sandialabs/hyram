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
using System.Collections.Generic;
using System.Linq;
using DefaultParsing;

namespace JrConversions
{
    public static class EnumUtil
    {
        public static IEnumerable<T> GetEnumPossibleValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }

    public class ConvUtil
    {
        public static double[] ParseDoubles(string[] values)
        {
            var result = new double[values.Length];

            for (var index = 0; index < result.Length; index++)
            {
                var theValue = double.NaN;
                if (Parsing.TryParseDouble(values[index], out theValue))
                    result[index] = theValue;
                else
                    result[index] = double.NaN;
            }

            return result;
        }

        public static double ConvertDegreesToRadians(double angle)
        {
            var result = Math.PI * angle / 180.0;
            return result;
        }

        public static double ConvertRadiansToDegrees(double angle)
        {
            return angle * (180.0 / Math.PI);
        }
    }
}