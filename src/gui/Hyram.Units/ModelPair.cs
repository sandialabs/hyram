/*
Copyright 2015-2024 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandiaNationalLaboratories.Hyram
{
    /// <summary>
    /// Represents string id with display label, e.g. notional nozzle selection.
    /// </summary>
    [JsonConverter(typeof(ModelPairJsonConverter))]
    public class ModelPair
    {
        private readonly string _key;
        private readonly string _name;

        public ModelPair(string name, string key)
        {
            _name = name;
            _key = key;
        }

        public override string ToString()
        {
            return _name;
        }

        public string GetKey()
        {
            return _key;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ModelPair);
        }

        public bool Equals(ModelPair f)
        {
            if (f is null)
            {
                return false;
            }

            if (ReferenceEquals(this, f))
            {
                return true;
            }

            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (_key == f._key);
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public static bool operator ==(ModelPair lhs, ModelPair rhs)
        {
            if (lhs is null)
            {
                if (rhs is null)
                {
                    // null == null = true.
                    return true;
                }
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ModelPair lhs, ModelPair rhs)
        {
            return !(lhs == rhs);
        }
    }

    public class ModelPairJsonConverter : JsonConverter<ModelPair>
    {
        public override ModelPair Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string propertyName;
            string key = "FAIL";

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
//                Console.WriteLine($"reader.TokenType:{reader.TokenType}");
                propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "Key":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            key = reader.GetString();
                        }
                        break;
                    }
                }
            }

            ModelPair pair = new ModelPair("EMPTY", key);
            return pair;
        }

        public override void Write(Utf8JsonWriter writer, ModelPair pair, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteString("Key", pair.GetKey());
            writer.WriteEndObject();
        }
    }

}
