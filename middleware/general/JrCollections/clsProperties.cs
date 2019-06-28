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
using System.Collections;
using System.Runtime.Serialization;
using EssStringLib;

namespace JrCollections
{
    [Serializable]
    public class ClsProperties : ISerializable
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

        public ClsProperties(SerializationInfo info, StreamingContext context)
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

        public ClsProperties(string propName, object propValue)
        {
            this[propName] = propValue;
        }

        private readonly Hashtable _mPropsDict = new Hashtable();

        public ClsProperties Clone()
        {
            return CloneAllBut((string[]) null);
        }

        public ClsProperties CloneAllBut(string[] fieldsToIgnore)
        {
            var theKeys = Keys;

            var result = new ClsProperties();

            foreach (var key in theKeys)
            {
                var addToClone = true;
                if (fieldsToIgnore != null)
                    if (ArrayFunctions.ArrayHasValue(fieldsToIgnore, key, false))
                        addToClone = false;

                if (addToClone) result[key] = this[key];
            }

            return result;
        }


        public ClsProperties()
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
                var result = new ClsEditableStringArray();

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