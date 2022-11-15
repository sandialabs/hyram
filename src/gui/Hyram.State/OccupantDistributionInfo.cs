/*
Copyright 2015-2022 National Technology & Engineering Solutions of Sandia, LLC (NTESS).
Under the terms of Contract DE-NA0003525 with NTESS, the U.S.Government retains certain
rights in this software.

You should have received a copy of the GNU General Public License along with
HyRAM+. If not, see https://www.gnu.org/licenses/.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SandiaNationalLaboratories.Hyram
{
    public enum WorkerDist
    {
        // do not rearrange without updating python as well
        Normal,
        Uniform,
        Constant
    }

    /// <summary>
    /// Describes a group of occupants and positions defined by x,y,z distributions.
    /// Displayed in the GUI as single data row.
    /// </summary>
    public class OccupantDistributionInfo
    {
        private string _mDesc = "";
        private double _hours = 2000;
        private int _nTargets = 1;
        private Parameter _xa = new Parameter(Converters.Distance, 1);
        private Parameter _xb = new Parameter(Converters.Distance, 20);
        private Parameter _ya = new Parameter(Converters.Distance, 1);
        private Parameter _yb = new Parameter(Converters.Distance, 20);
        private Parameter _za = new Parameter(Converters.Distance, 1);
        private Parameter _zb = new Parameter(Converters.Distance, 20);

        public OccupantDistributionInfo(int numTargets, string description,
                                        WorkerDist xDistribution, double xParamA, double xParamB,
                                        WorkerDist yDistribution, double yParamA, double yParamB,
                                        WorkerDist zDistribution, double zParamA, double zParamB,
                                        DistanceUnit paramUnitType, double exposureHours)
        {
            NumTargets = numTargets;
            Desc = description;
            XLocDistribution = xDistribution;
            YLocDistribution = yDistribution;
            ZLocDistribution = zDistribution;

            ParamUnitType = paramUnitType;  // must come before params so they're initialized correctly
            XLocParamA = xParamA;
            YLocParamA = yParamA;
            ZLocParamA = zParamA;

            XLocParamB = xParamB;
            YLocParamB = yParamB;
            ZLocParamB = zParamB;

            ExposureHours = exposureHours;
        }

        public OccupantDistributionInfo()
        {
        }

        public int NumTargets
        {
            get => _nTargets;
            set
            {
                if (value >= 0) _nTargets = value;
            }
        }

        public string Desc
        {
            get => _mDesc;
            set => _mDesc = value;
        }


        public WorkerDist XLocDistribution { get; set; } = WorkerDist.Uniform;
        public double XLocParamA
        {
            get => _xa.GetValue(ParamUnitType);
            set
            {
                if (value >= 0.0) _xa.SetValue(ParamUnitType, value);
                else _xa.SetValue(ParamUnitType, 0.0);
            }
        }
        public double XLocParamB
        {
            get => _xb.GetValue(ParamUnitType);
            set
            {
                if (value >= 0.0) _xb.SetValue(ParamUnitType, value);
                else _xb.SetValue(ParamUnitType, 0.0);
            }
        }

        public WorkerDist YLocDistribution { get; set; } = WorkerDist.Uniform;

        public double YLocParamA
        {
            get => _ya.GetValue(ParamUnitType);
            set
            {
                if (value >= 0.0) _ya.SetValue(ParamUnitType, value);
                else _ya.SetValue(ParamUnitType, 0);
            }
        }

        public double YLocParamB
        {
            get => _yb.GetValue(ParamUnitType);
            set
            {
                if (value >= 0.0) _yb.SetValue(ParamUnitType, value);
                else _yb.SetValue(ParamUnitType, 0.0);
            }
        }

        public WorkerDist ZLocDistribution { get; set; } = WorkerDist.Uniform;

        public double ZLocParamA
        {
            get => _za.GetValue(ParamUnitType);
            set
            {
                if (value >= 0.0) _za.SetValue(ParamUnitType, value);
                else _za.SetValue(ParamUnitType, 0);
            }
        }

        public double ZLocParamB
        {
            get => _zb.GetValue(ParamUnitType);
            set
            {
                if (value >= 0.0) _zb.SetValue(ParamUnitType, value);
                else _zb.SetValue(ParamUnitType, 0.0);
            }
        }

        public DistanceUnit ParamUnitType { get; set; } = DistanceUnit.Meter;

        public double ExposureHours
        {
            get => _hours;
            set
            {
                if (value > 8760.0)
                    _hours = 8760.0;
                else if (value < 0.0)
                    _hours = 0.0;
                else
                    _hours = value;
            }
        }


        /// <summary>
        /// Returns parsed string of occupant data.
        /// </summary>
        /// <returns>string</returns>
        public string GetSimpleString()
        {
            string xDistr = GetDistributionString((WorkerDist)XLocDistribution, "X", _xa, _xb);
            string yDistr = GetDistributionString((WorkerDist)YLocDistribution, "Y", _ya, _yb);
            string zDistr = GetDistributionString((WorkerDist)ZLocDistribution, "Z", _za, _zb);
            string combined = $"{{\"NumTargets\":{_nTargets},\"Desc\":\"{_mDesc}\",\"ExposureHours\":{_hours},{xDistr},{yDistr},{zDistr}}}";
            return combined;
        }

        /// <summary>
        /// Returns string of occupant location distribution and parameters.
        /// </summary>
        /// <param name="distribution"></param>
        /// <param name="letter">Coordinate</param>
        /// <param name="paramA">Distribution parameter A</param>
        /// <param name="paramB">Distribution parameter B</param>
        /// <returns>string</returns>
        private string GetDistributionString(WorkerDist distribution, string letter, Parameter paramA, Parameter paramB)
        {
            string paramAString = paramA.GetValue(DistanceUnit.Meter).ToString();
            string paramBString = paramB.GetValue(DistanceUnit.Meter).ToString();
            return $"\"{letter}LocDistribution\":{(int)distribution},\"{letter}LocParamA\":{paramAString},\"{letter}LocParamB\":{paramBString}";
        }
    }


    /// <summary>
    /// Represents list of occupant data-sets.
    /// </summary>
    [JsonConverter(typeof(OccupantJsonConverter))]
    public class OccupantDistributionInfoCollection : List<OccupantDistributionInfo>
    {
        public OccupantDistributionInfoCollection(bool populateDefaults)
        {
            if (populateDefaults)
            {
                var workersDesc = "Station workers";
                var workers = new OccupantDistributionInfo(9, workersDesc,
                                                            WorkerDist.Uniform, 1, 20,
                                                            WorkerDist.Constant, 0, double.NaN, 
                                                            WorkerDist.Uniform, 1, 12,
                                                            DistanceUnit.Meter, 2000);
                Add(workers);
            }
        }

        public new void Add(OccupantDistributionInfo item)
        {
            if (!Contains(item)) base.Add(item);
        }

        /// <summary>
        /// Returns string describing all occupant data-sets and their parameters.
        /// </summary>
        /// <returns></returns>
        public string GetSimpleString()
        {
            string combined = "[";
            for (int i = 0; i < Count; i++)
            {
                OccupantDistributionInfo occupantSet = this[i];
                if (i > 0)
                {
                    combined += ",";
                }
                combined += $"{occupantSet.GetSimpleString()}";
            }

            combined += "]";
            return combined;
        }
    }


    /// <summary>
    /// Serializes and deserializes occupant data-sets (rows) into JSON array.
    /// </summary>
    public class OccupantJsonConverter : JsonConverter<OccupantDistributionInfoCollection>
    {
        public override OccupantDistributionInfoCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string desc = "", propertyName, unitStr;
            DistanceUnit units = DistanceUnit.Meter;
            double hours = 2000, xa = 1, xb = 20, ya = 0, yb = double.NaN, za = 1, zb = 12;
            int targets = 9, xd = 1, yd = 2, zd = 1;
            WorkerDist xDist = WorkerDist.Uniform;
            WorkerDist yDist = WorkerDist.Constant;
            WorkerDist zDist = WorkerDist.Uniform;
            OccupantDistributionInfo occ = new OccupantDistributionInfo();

            OccupantDistributionInfoCollection coll = new OccupantDistributionInfoCollection(false);

            if (reader.TokenType == JsonTokenType.Null || reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    // all done
                    return coll;
                }

                if (reader.TokenType == JsonTokenType.StartObject)
                {
                    // new occupant data row; set defaults
                    desc = "";
                    units = DistanceUnit.Meter;
                    hours = 2000; xa = 1; xb = 20; ya = 0; yb = double.NaN; za = 1; zb = 12;
                    targets = 9; xd = 1; yd = 2; zd = 1;
                    xDist = WorkerDist.Uniform;
                    yDist = WorkerDist.Constant;
                    zDist = WorkerDist.Uniform;
                    continue;
                }

                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    // finish parsing data row into object
                    occ = new OccupantDistributionInfo
                    {
                        Desc = desc,
                        NumTargets = targets,
                        ParamUnitType = units,
                        ExposureHours = hours,
                        XLocDistribution = xDist,
                        YLocDistribution = yDist,
                        ZLocDistribution = zDist,
                        XLocParamA = xa,
                        XLocParamB = xb,
                        YLocParamA = ya,
                        YLocParamB = yb,
                        ZLocParamA = za,
                        ZLocParamB = zb
                    };

                    coll.Add(occ);
                    continue;
                }

                propertyName = reader.GetString();
//                Console.WriteLine($"propertyName:{propertyName}");
                switch (propertyName)
                {
                    case "Desc":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) desc = reader.GetString();
                        break;
                    case "ExposureHours":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) hours = reader.GetDouble();
                        break;
                    case "NumTargets":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) targets = reader.GetInt32();
                        break;
                    case "ParamUnitType":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            unitStr = reader.GetString();
                            units = UnitParser.ParseDistanceUnit(unitStr);
                        }
                        break;

                    case "XLocDistribution":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            xd = reader.GetInt32();
                            xDist = (WorkerDist) xd;
                        }
                        break;
                    case "XLocParamA":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) xa = reader.GetDouble();
                        break;
                    case "XLocParamB":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) xb = reader.GetDouble();
                        break;

                    case "YLocDistribution":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            yd = reader.GetInt32();
                            yDist = (WorkerDist) yd;
                        }
                        break;
                    case "YLocParamA":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) ya = reader.GetDouble();
                        break;
                    case "YLocParamB":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) yb = reader.GetDouble();
                        break;

                    case "ZLocDistribution":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null)
                        {
                            zd = reader.GetInt32();
                            zDist = (WorkerDist) zd;
                        }
                        break;
                    case "ZLocParamA":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) za = reader.GetDouble();
                        break;
                    case "ZLocParamB":
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Null) zb = reader.GetDouble();
                        break;
                }

            }
            throw new JsonException();  // truncated file
        }

        public override void Write(Utf8JsonWriter writer, OccupantDistributionInfoCollection occupantsCollection, JsonSerializerOptions options)
        {
            Trace.TraceInformation("Writing occupants...");
//            writer.WriteStartObject();

            if (occupantsCollection.Count > 0)
            {
                writer.WriteStartArray();

                foreach (OccupantDistributionInfo elem in occupantsCollection)
                {
                    writer.WriteStartObject();
                    writer.WriteString(nameof(elem.Desc), elem.Desc);
                    writer.WriteNumber(nameof(elem.ExposureHours), elem.ExposureHours);
                    writer.WriteNumber(nameof(elem.NumTargets), elem.NumTargets);
                    writer.WriteString(nameof(elem.ParamUnitType), elem.ParamUnitType.ToString());

                    writer.WriteNumber(nameof(elem.XLocDistribution), (int)elem.XLocDistribution);
                    writer.WriteNumber(nameof(elem.XLocParamA), elem.XLocParamA);
                    writer.WriteNumber(nameof(elem.XLocParamB), elem.XLocParamB);

                    writer.WriteNumber(nameof(elem.YLocDistribution), (int)elem.YLocDistribution);
                    writer.WriteNumber(nameof(elem.YLocParamA), elem.YLocParamA);
                    writer.WriteNumber(nameof(elem.YLocParamB), elem.YLocParamB);

                    writer.WriteNumber(nameof(elem.ZLocDistribution), (int)elem.ZLocDistribution);
                    writer.WriteNumber(nameof(elem.ZLocParamA), elem.ZLocParamA);
                    writer.WriteNumber(nameof(elem.ZLocParamB), elem.ZLocParamB);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
            }
//            writer.WriteEndObject();

            Trace.TraceInformation("Writing complete.");
        }
    }

}