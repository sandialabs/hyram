/*
Copyright 2015-2023 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
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
            UnitOfMeasurementConverters result;
            if (!_mAgCollection.ContainsKey(unitOfMeasurementName))
            {
                result = new UnitOfMeasurementConverters {Name = unitOfMeasurementName};
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
            if (string.IsNullOrEmpty(unitOfMeasurementName) || units == null || conversionFactors == null ||
                units.Length == 0 || conversionFactors.Length == 0 || units.Length != conversionFactors.Length)
            {
                throw new Exception("Invalid unit of measurement setup");
            }
            if (Math.Abs(conversionFactors[0] - 1D) > 1E-10)
            {
                throw new Exception("First conversion factor for " + unitOfMeasurementName + " (" + units[0] + ") must be 1");
            }

            var convertersForThisUnit = new UnitOfMeasurementConverters {Name = unitOfMeasurementName};
            _mAgCollection.Add(unitOfMeasurementName, convertersForThisUnit);

            for (var index = 0; index < units.Length; index++)
            {
                var newNode = new ConversionData {Name = units[index], ConversionFactor = conversionFactors[index]};
                var conversionProvider = new DualConversionProvider {ConversionObject = newNode};
                convertersForThisUnit.Add(newNode.Name, conversionProvider);
            }
        }


        public UnitOfMeasurementConverters GetUnitConverter(string unitOfMeasurementName)
        {
            return _mAgCollection[unitOfMeasurementName];
        }
    }

    [Serializable]
    public class UnitOfMeasurementConverters : Dictionary<string, DualConversionProvider>
    {
        public string Name { get; set; }
    }


    [Serializable]
    public class ConversionData
    {
        private double _mConversionFactor = double.NaN;
        public string Name;

        public double ConversionFactor
        {
            get
            {
                if (Math.Abs(_mConversionFactor) < 1E-17D || double.IsNaN(_mConversionFactor))
                {
                    throw new Exception("Conversion factor of zero or NaN is invalid.");
                }
                return _mConversionFactor;
            }
            set
            {
                if (Math.Abs(value) < 1E-17D || double.IsNaN(value))
                {
                    throw new Exception("Conversion factor cannot be set to zero or NaN.");
                }
                _mConversionFactor = value;
            }
        }
    }
}