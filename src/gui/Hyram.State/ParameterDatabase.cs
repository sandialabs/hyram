/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
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