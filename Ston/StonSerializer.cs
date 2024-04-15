using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ston
{
    public class StonSerializer
    {
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

        public Task<StonObject> DeserializeFileAsync(string filePath)
        {
            var fi = new FileInfo(filePath);
            return DeserializeFileAsync(fi);
        }

        public async Task<StonObject> DeserializeFileAsync(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
                return null;

            using (var fs = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var ston = await sr.ReadToEndAsync();
                    return Deserialize(ston);
                }
            }
        }

        public StonObject Deserialize(string ston)
        {
            var obj = new StonObject();
            var lines = ston.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                var separatorIndex = line.IndexOf(':');
                var key = line.Substring(0, separatorIndex).Trim();

                var typeAndValueIndex = separatorIndex + 1;
                var typeAndValue = line.Substring(typeAndValueIndex, line.Length - typeAndValueIndex);

                var equalIndex = typeAndValue.IndexOf('=');
                var type = typeAndValue.Substring(0, equalIndex).Trim();
                if (!_types.TryGetValue(type, out var stonType))
                    throw new Exception($"unsupported type '{type}'");

                var valueIndex = equalIndex + 1;
                var valueStr = typeAndValue.Substring(valueIndex, typeAndValue.Length - valueIndex);
                var valueTrimmed = valueStr.Trim().ToLowerInvariant();

                var stonValue = default(StonValue);
                switch (stonType)
                {
                    case StonValue.Type.Boolean:
                        var boolValue = valueTrimmed == "1" || valueTrimmed == "yes" || bool.Parse(valueTrimmed);
                        stonValue = new StonValue(boolValue);
                        break;

                    case StonValue.Type.Byte:
                        var byteValue = byte.Parse(valueTrimmed);
                        stonValue = new StonValue(byteValue);
                        break;

                    case StonValue.Type.SByte:
                        var sbyteValue = sbyte.Parse(valueTrimmed);
                        stonValue = new StonValue(sbyteValue);
                        break;

                    case StonValue.Type.Int16:
                        var shortValue = short.Parse(valueTrimmed);
                        stonValue = new StonValue(shortValue);
                        break;

                    case StonValue.Type.Int32:
                        var intValue = int.Parse(valueTrimmed);
                        stonValue = new StonValue(intValue);
                        break;

                    case StonValue.Type.Int64:
                        var longValue = long.Parse(valueTrimmed);
                        stonValue = new StonValue(longValue);
                        break;

                    case StonValue.Type.UInt16:
                        var ushortValue = ushort.Parse(valueTrimmed);
                        stonValue = new StonValue(ushortValue);
                        break;

                    case StonValue.Type.UInt32:
                        var uintValue = uint.Parse(valueTrimmed);
                        stonValue = new StonValue(uintValue);
                        break;

                    case StonValue.Type.UInt64:
                        var ulongValue = ulong.Parse(valueTrimmed);
                        stonValue = new StonValue(ulongValue);
                        break;

                    case StonValue.Type.Single:
                        var floatValue = float.Parse(valueTrimmed, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                        stonValue = new StonValue(floatValue);
                        break;

                    case StonValue.Type.Double:
                        var doubleValue = double.Parse(valueTrimmed, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat);
                        stonValue = new StonValue(doubleValue);
                        break;

                    case StonValue.Type.Decimal:
                        var decimalValue = decimal.Parse(valueTrimmed);
                        stonValue = new StonValue(decimalValue);
                        break;

                    case StonValue.Type.String:
                        stonValue = new StonValue(valueStr);
                        break;

                    default:

                        break;
                }
                obj.Add(key, stonValue);
            }
            return obj;
        }
    }
}
