using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    public class PrimitiveStormConverter : IStormConverter
    {
        public static readonly PrimitiveStormConverter instance = new PrimitiveStormConverter();

        private static readonly Dictionary<StormValue.Type, string> _postfixes = new Dictionary<StormValue.Type, string>
        {
            { StormValue.Type.Boolean,   "b"     },
            { StormValue.Type.Byte,      "bt"    },
            { StormValue.Type.SByte,     "sbt"   },
            { StormValue.Type.Int16,     "s"     },
            { StormValue.Type.Int32,     "i"     },
            { StormValue.Type.Int64,     "l"     },
            { StormValue.Type.UInt16,    "us"    },
            { StormValue.Type.UInt32,    "ui"    },
            { StormValue.Type.UInt64,    "ul"    },
            { StormValue.Type.Single,    "f"     },
            { StormValue.Type.Double,    "d"     },
            { StormValue.Type.Decimal,   "dec"   },
            { StormValue.Type.String,    "t"     }
        };
        private static readonly Dictionary<string, StormValue.Type> _types = _postfixes.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key);

        public bool CanConvert(string type)
        {
            return _types.ContainsKey(type);
        }

        public Task<IStormValue> DeserializeAsync(string type, string text, StormContext ctx)
        {
            var stormType = _types[type];
            var obj = Parse(stormType, text, ctx);
            return Task.FromResult<IStormValue>(obj);
        }

        private StormValue Parse(StormValue.Type type, string text, StormContext ctx)
        { 
            var trimmed = text.Trim().ToLowerInvariant();
            switch (type)
            {
                case StormValue.Type.Boolean:
                    var boolValue = default(bool);
                    if (string.Equals(trimmed, "1", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        boolValue = true;
                    }
                    else if (string.Equals(trimmed, "yes", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        boolValue = true;
                    }
                    else if (string.Equals(trimmed, "0", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        boolValue = false;
                    }
                    else if (string.Equals(trimmed, "no", System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        boolValue = false;
                    }
                    else
                    {
                        boolValue = bool.Parse(trimmed);
                    }
                    return new StormValue(boolValue);

                case StormValue.Type.Byte:
                    var byteValue = byte.Parse(trimmed);
                    return new StormValue(byteValue);

                case StormValue.Type.SByte:
                    var sbyteValue = sbyte.Parse(trimmed);
                    return new StormValue(sbyteValue);

                case StormValue.Type.Int16:
                    var shortValue = short.Parse(trimmed);
                    return new StormValue(shortValue);

                case StormValue.Type.Int32:
                    var intValue = int.Parse(trimmed);
                    return new StormValue(intValue);

                case StormValue.Type.Int64:
                    var longValue = long.Parse(trimmed);
                    return new StormValue(longValue);

                case StormValue.Type.UInt16:
                    var ushortValue = ushort.Parse(trimmed);
                    return new StormValue(ushortValue);

                case StormValue.Type.UInt32:
                    var uintValue = uint.Parse(trimmed);
                    return new StormValue(uintValue);

                case StormValue.Type.UInt64:
                    var ulongValue = ulong.Parse(trimmed);
                    return new StormValue(ulongValue);

                case StormValue.Type.Single:
                    var floatValue = float.Parse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                    return new StormValue(floatValue);

                case StormValue.Type.Double:
                    var doubleValue = double.Parse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                    return new StormValue(doubleValue);

                case StormValue.Type.Decimal:
                    var decimalValue = decimal.Parse(trimmed);
                    return new StormValue(decimalValue);

                case StormValue.Type.String:
                    return new StormValue(text);

                case StormValue.Type.Null:
                    return StormValue.nil;

                default:
                    return null;
            }
        }
    }
}
