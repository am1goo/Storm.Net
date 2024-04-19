using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Ston.Serializers
{
    public class PrimitiveStonConverter : IStonConverter
    {
        public static readonly PrimitiveStonConverter instance = new PrimitiveStonConverter();

        private static readonly Dictionary<StonValue.Type, string> _postfixes = new Dictionary<StonValue.Type, string>
        {
            { StonValue.Type.Boolean,   "b"     },
            { StonValue.Type.Byte,      "bt"    },
            { StonValue.Type.SByte,     "sbt"   },
            { StonValue.Type.Int16,     "s"     },
            { StonValue.Type.Int32,     "i"     },
            { StonValue.Type.Int64,     "l"     },
            { StonValue.Type.UInt16,    "us"    },
            { StonValue.Type.UInt32,    "ui"    },
            { StonValue.Type.UInt64,    "ul"    },
            { StonValue.Type.Single,    "f"     },
            { StonValue.Type.Double,    "d"     },
            { StonValue.Type.Decimal,   "dec"   },
            { StonValue.Type.String,    "str"   }
        };
        private static readonly Dictionary<string, StonValue.Type> _types = _postfixes.ToDictionary(keySelector => keySelector.Value, valueSelector => valueSelector.Key);

        public bool CanConvert(string type)
        {
            return _types.ContainsKey(type);
        }

        public IStonValue Deserialize(string type, string text, StonContext ctx)
        {
            var stonType = _types[type];
            var trimmed = text.Trim().ToLowerInvariant();
            switch (stonType)
            {
                case StonValue.Type.Boolean:
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
                    return new StonValue(boolValue);

                case StonValue.Type.Byte:
                    var byteValue = byte.Parse(trimmed);
                    return new StonValue(byteValue);

                case StonValue.Type.SByte:
                    var sbyteValue = sbyte.Parse(trimmed);
                    return new StonValue(sbyteValue);

                case StonValue.Type.Int16:
                    var shortValue = short.Parse(trimmed);
                    return new StonValue(shortValue);

                case StonValue.Type.Int32:
                    var intValue = int.Parse(trimmed);
                    return new StonValue(intValue);

                case StonValue.Type.Int64:
                    var longValue = long.Parse(trimmed);
                    return new StonValue(longValue);

                case StonValue.Type.UInt16:
                    var ushortValue = ushort.Parse(trimmed);
                    return new StonValue(ushortValue);

                case StonValue.Type.UInt32:
                    var uintValue = uint.Parse(trimmed);
                    return new StonValue(uintValue);

                case StonValue.Type.UInt64:
                    var ulongValue = ulong.Parse(trimmed);
                    return new StonValue(ulongValue);

                case StonValue.Type.Single:
                    var floatValue = float.Parse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                    return new StonValue(floatValue);

                case StonValue.Type.Double:
                    var doubleValue = double.Parse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                    return new StonValue(doubleValue);

                case StonValue.Type.Decimal:
                    var decimalValue = decimal.Parse(trimmed);
                    return new StonValue(decimalValue);

                case StonValue.Type.String:
                    return new StonValue(text);

                case StonValue.Type.Null:
                    return StonValue.nil;

                default:
                    return null;
            }
        }
    }
}
