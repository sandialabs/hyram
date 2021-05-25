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
using System.Collections;
using System.Runtime.Serialization;

namespace SandiaNationalLaboratories.Hyram
{
    [Serializable]
    public class ParameterDatabase : ISerializable
    {
        public bool ContainsKey(string theKey)
        {
            return _mPropsDict.ContainsKey(theKey.ToUpper());
        }

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            var theKeys = Keys;

            info.AddValue("clsProperties.Keys()", theKeys);
            foreach (var thisKey in theKeys) info.AddValue(thisKey, this[thisKey]);
        }

        public ParameterDatabase(SerializationInfo info, StreamingContext context)
        {
            var theKeys = new string[0];

            theKeys = (string[]) info.GetValue("clsProperties.Keys()", theKeys.GetType());

            for (var index = 0; index < theKeys.Length; index++)
            {
                var thisItem = new object();

                var thisType = thisItem.GetType();
                var key = theKeys[index];

                thisItem = info.GetValue(key, thisType);
                this[key] = thisItem;
            }
        }

        public string Name
        {
            get => (string) this["Name"];
            set => this["Name"] = value;
        }

        public bool IsNull(string propertyName)
        {
            var result = this[propertyName] == null;
            if (!result) result = this[propertyName] is DBNull;
            return result;
        }

        public ParameterDatabase(string propName, object propValue)
        {
            this[propName] = propValue;
        }

        private readonly Hashtable _mPropsDict = new Hashtable();


        public ParameterDatabase()
        {
        }

        public bool Contains(string key)
        {
            return _mPropsDict.Contains(key.ToUpper());
        }

        public string[] Keys
        {
            get
            {
                var result = new EditableStringArray();

                foreach (var oKey in _mPropsDict.Keys)
                {
                    var thisKey = (string) oKey;
                    result.Append(thisKey);
                }

                return result.Data;
            }
        }

        public object this[string key]
        {
            get
            {
                object result = null;
                key = key.ToUpper();

                if (_mPropsDict.Contains(key))
                    result = _mPropsDict[key];
                else
                    throw new Exception("Item " + key + " does not exist in collection.");

                return result;
            }
            set => _mPropsDict[key.ToUpper()] = value;
        }


        public void Delete(string key)
        {
            if (_mPropsDict.ContainsKey(key)) _mPropsDict.Remove(key);
        }
    }
}