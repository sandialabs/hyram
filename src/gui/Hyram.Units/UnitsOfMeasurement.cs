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
using System.Linq;
using System.Runtime.Serialization;

namespace SandiaNationalLaboratories.Hyram
{
    // UnitsOfMeasurement - A collection of units (Pressure, Time, Distance, etc.)
    [Serializable]
    public class UnitsOfMeasurement : ISerializable
    {
        private readonly Dictionary<string, UnitOfMeasurementConverters> _mAgCollection =
            new Dictionary<string, UnitOfMeasurementConverters>();

        public UnitsOfMeasurement()
        {
        }


        public UnitsOfMeasurement(SerializationInfo info, StreamingContext context)
        {
            var theKeys = new string[0];
            theKeys = (string[]) info.GetValue("UnitsOfMeasurement.Keys()", theKeys.GetType());

            for (var index = 0; index < theKeys.Length; index++)
            {
                var key = theKeys[index];

                var thisItem = info.GetValue(key, new object().GetType());
                _mAgCollection[key] = (UnitOfMeasurementConverters) thisItem;
            }
        }


        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var theKeys = _mAgCollection.Keys.ToArray();
            info.AddValue("UnitsOfMeasurement.Keys()", theKeys);
            foreach (var thisKey in theKeys) info.AddValue(thisKey, _mAgCollection[thisKey]);
        }

        public bool ContainsKey(string key)
        {
            return _mAgCollection.ContainsKey(key);
        }

        public UnitOfMeasurementConverters CreateOrGetUnitConverter(string unitOfMeasurementName)
        {
            UnitOfMeasurementConverters result = null;

            if (!_mAgCollection.ContainsKey(unitOfMeasurementName))
            {
                result = new UnitOfMeasurementConverters();
                result.Name = unitOfMeasurementName;
                _mAgCollection.Add(unitOfMeasurementName, result);
            }
            else
            {
                result = _mAgCollection[unitOfMeasurementName];
            }

            return result;
        }


        public void Populate(string unitOfMeasurementName, string[] units, double[] conversionFactors)
        {
            #region EvaluateBasicPopulationRules

            if (string.IsNullOrEmpty(unitOfMeasurementName))
                throw new Exception("Unit of measurement name must be set.");

            if (units == null || conversionFactors == null)
                throw new Exception("Either Units or ConversionFactors argument is not set.");

            if (units.Length == 0 || conversionFactors.Length == 0)
                throw new Exception("Units and ConversionFactors arguments must contain more than zero elements.");

            if (units.Length != conversionFactors.Length)
                throw new Exception("Units and ConversionFactors arguments must be the same length.");


            if (conversionFactors[0] != 1D)
                throw new Exception("First conversion factor for " + unitOfMeasurementName + " (" + units[0] +
                                    ") must be 1.0 and is " + conversionFactors[0] + ".");

            #endregion

            var convertersForThisUnit = new UnitOfMeasurementConverters();
            convertersForThisUnit.Name = unitOfMeasurementName;
            _mAgCollection.Add(unitOfMeasurementName, convertersForThisUnit);

            for (var index = 0; index < units.Length; index++)
            {
                var newNode = new ConversionData();
                newNode.Name = units[index];
                newNode.ConversionFactor = conversionFactors[index];
                var conversionProvider = new DualConversionProvider();
                conversionProvider.ConversionObject = newNode;
                convertersForThisUnit.Add(newNode.Name, conversionProvider);
            }
        }


        public UnitOfMeasurementConverters GetUnitConverter(string unitOfMeasurementName)
        {
            return _mAgCollection[unitOfMeasurementName];
        }
    }

    [Serializable]
    public class UnitOfMeasurementConverters : Dictionary<string, DualConversionProvider>, ISerializable
    {
        private string _mName;

        public UnitOfMeasurementConverters()
        {
        }

        public UnitOfMeasurementConverters(SerializationInfo info, StreamingContext context)
        {
            object thisC = context;


            var theKeys = (string[]) info.GetValue("UnitOfMeasurementConverters.Keys()", new string[0].GetType());

            for (var index = 0; index < theKeys.Length; index++)
            {
                var oThisNode = info.GetValue(theKeys[index], new object().GetType());

                Add(theKeys[index], (DualConversionProvider) oThisNode);
            }
        }

        public string Name
        {
            get => _mName;
            set => _mName = value;
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var theKeys = Keys.ToArray();

            info.AddValue("UnitOfMeasurementConverters.Keys()", theKeys);
            foreach (var thisKey in theKeys) info.AddValue(thisKey, this[thisKey]);
        }

        public double[] ConvertFromSi(string destinationUnitName, double[] value)
        {
            var conversionProvider = GetConverter(destinationUnitName);
            double[] result = null;
            if (conversionProvider.ConversionObject != null)
            {
                result = new double[value.Length];
                for (var index = 0; index < value.Length; index++)
                    result[index] = value[index] / conversionProvider.ConversionObject.ConversionFactor;
            }
            else
            {
                result = conversionProvider.ConversionDelegate.ConvertFrom(value);
            }

            return result;
        }

        public double[] ConvertToSi(string sourceUnitName, double[] value)
        {
            var converter = GetConverter(sourceUnitName);
            if (converter.ConversionDelegate != null)
            {
                return converter.ConversionDelegate.ConvertTo(value);
            }

            var result = new double[value.Length];
            for (var index = 0; index < value.Length; index++)
                result[index] = value[index] * converter.ConversionObject.ConversionFactor;

            return result;
        }

        private DualConversionProvider GetConverter(string typeName)
        {
            return this[typeName];
        }


        public bool HasBadConversionFactor()
        {
            var result = false;

            foreach (var conversionProvider in Values)
                if (conversionProvider.HasBadConversionFactor())
                {
                    result = true;
                    break;
                }

            return result;
        }
    }


    [Serializable]
    public class ConversionData
    {
        private double _mConversionFactor = double.NaN;
        public string Name;
        public string TypeName = null;

        public double ConversionFactor
        {
            get
            {
                if (_mConversionFactor == 0 || double.IsNaN(_mConversionFactor))
                    throw new Exception("Conversion factor of zero or NaN is invalid.");

                return _mConversionFactor;
            }
            set
            {
                if (value == 0 || double.IsNaN(value))
                    throw new Exception("Conversion factor cannot be set to zero or NaN.");

                _mConversionFactor = value;
            }
        }


        public bool HasBadConversionFactor()
        {
            return _mConversionFactor == 0 || double.IsNaN(_mConversionFactor);
        }
    }
}