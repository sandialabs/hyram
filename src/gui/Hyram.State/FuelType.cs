/*
Copyright 2015-2021 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
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
    /// Representation of fuel species and its properties (concentration as amount, etc.).
    /// </summary>
    [JsonConverter(typeof(FuelTypeJsonConverter))]
    public class FuelType
    {
        private double _amount;
        private bool _active = false;

        // Note: for clarity, this is fine-grained and should only be caught by state handler, not views.
        public static event EventHandler FuelConcentrationChangedEvent;

        public FuelType(int id, string name, string key, double chokedFlowRatio, double criticalP, double amount,
                        double? lfl = null, bool active = false)
        {
            Name = name;
            ChokedFlowRatio = chokedFlowRatio;
            Id = id;
            Key = key;
            CriticalP = criticalP;
            Amount = amount;
            Lfl = lfl;
            Active = active;
        }

        // public and settable to allow grid data-binding
        public string Name { get; }
        public int Id { get; }
        public string Key { get; }

        // Note: blend ChokedFlow and CriticalP are dummy values initially; must be computed from all fuels (see StateContainer)
        public double ChokedFlowRatio { get; set;  }

        // [Pa] (liquid?)
        public double CriticalP { get; set;  }
        public double? Lfl { get; }

        // Active and Amount setters can call each other
        private bool _ignoreInternalChange = false;

        public bool Active
        {
            get => _active;

            set
            {
                // clear amount when disabling
                if (!_ignoreInternalChange && !value)
                {
                    _ignoreInternalChange = true;
                    Amount = 0;
                    _ignoreInternalChange = false;
                }

                _active = value;

            }
        }

        // total amounts across all fuels should sum to 1.0
        public double Amount
        {
            get => _amount;
            set
            {
                if (Math.Abs(value - _amount) > 0.000001)
                {
                    value = Math.Round(value, 6);
                    _amount = value;

                    if (!_ignoreInternalChange)
                    {
                        _ignoreInternalChange = true;
                        Active = (_amount > 0);
                        _ignoreInternalChange = false;
                    }

                    FuelConcentrationChangedEvent?.Invoke(this, EventArgs.Empty);
                }
            }

        }  

        // Note: used by Fuels form
        public string Formula
        {
            get => Key.ToUpper();
        }

        public override string ToString()
        {
            return Name;
        }

        public double GetCriticalPressureMpa()
        {
            return (CriticalP / 1000000.0);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as FuelType);
        }

        public bool Equals(FuelType f)
        {
            if (f is null)
            {
                return false;
            }

            // Optimization for a common success case.
            if (ReferenceEquals(this, f))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (GetType() != f.GetType())
            {
                return false;
            }

            // Return true if the fields match.
            // Note that the base class is not invoked because it is
            // System.Object, which defines Equals as reference equality.
            return (Id == f.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(FuelType lhs, FuelType rhs)
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

        public static bool operator !=(FuelType lhs, FuelType rhs)
        {
            return !(lhs == rhs);
        }
    }

    /// <summary>
    /// Serializes and deserializes FuelType to JSON.
    /// </summary>
    public class FuelTypeJsonConverter : JsonConverter<FuelType>
    {
        public override FuelType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string propertyName;
            string fuelName = "FUELNOTFOUND";
            int id = -1;
            string key = "KEYNOTFOUND";
            double amount = -1;
            double chokedFlow = 0;
            double criticalP = 0;
            double? lfl = null;
            bool active = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }
                propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "Name":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            fuelName = reader.GetString();
                        }
                        break;
                    }
                    case "Id":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            id = reader.GetInt32();
                        }
                        break;
                    }
                    case "Key":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            key = reader.GetString();
                        }
                        break;
                    }
                    case "Amount":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            amount = reader.GetDouble();
                        }
                        break;
                    }
                    case "ChokedFlowRatio":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            chokedFlow = reader.GetDouble();
                        }
                        break;
                    }
                    case "CriticalP":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            criticalP = reader.GetDouble();
                        }
                        break;
                    }
                    case "Lfl":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            lfl = reader.GetDouble();
                        }
                        break;
                    }
                    case "Active":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            active = reader.GetBoolean();
                        }
                        break;
                    }
                }
            }
            FuelType fuel = new FuelType(id, fuelName, key, chokedFlow, criticalP, amount, lfl, active);
            return fuel;
        }

        public override void Write(Utf8JsonWriter writer, FuelType fuel, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(nameof(fuel.Name), fuel.Name);
            writer.WriteNumber(nameof(fuel.Id), fuel.Id);
            writer.WriteString(nameof(fuel.Key), fuel.Key);
            writer.WriteNumber(nameof(fuel.Amount), Math.Round(fuel.Amount, 6));
            writer.WriteNumber(nameof(fuel.ChokedFlowRatio), Math.Round(fuel.ChokedFlowRatio, 6));
            writer.WriteNumber(nameof(fuel.CriticalP), Math.Round(fuel.CriticalP, 6));
            if (fuel.Lfl == null)
            {
                writer.WriteNull(nameof(fuel.Lfl));
            }
            else
            {
                writer.WriteNumber(nameof(fuel.Lfl), Math.Round((double)fuel.Lfl, 6));
            }
            writer.WriteBoolean(nameof(fuel.Active), fuel.Active);

            writer.WriteEndObject();
        }
    }

}
