using System;

namespace Storm
{
    public class StormValue : IStormValue
    {
        internal static readonly StormValue nil = new StormValue
        {
            _type = Type.Null,
        };

        private Type _type;
        public Type type => _type;

        private long _signedValue;
        private ulong _unsignedValue;
        private double _doubleValue;
        private decimal _decimalValue;
        private string _stringValue;

        public StormValue(bool boolValue)
        {
            _type = Type.Boolean;
            _signedValue = boolValue ? 1 : 0;
        }

        public bool AsBool()
        {
            return _signedValue == 1;
        }

        public StormValue(sbyte sbyteValue)
        {
            _type = Type.SByte;
            _signedValue = sbyteValue;
        }

        public sbyte AsSByte()
        {
            return (sbyte)_signedValue;
        }

        public StormValue(short shortValue)
        {
            _type = Type.Int16;
            _signedValue = shortValue;
        }

        public short AsInt16()
        {
            return (short)_signedValue;
        }

        public StormValue(int intValue)
        {
            _type = Type.Int32;
            _signedValue = intValue;
        }

        public int AsInt32()
        {
            return (int)_signedValue;
        }

        public StormValue(long longValue)
        {
            _type = Type.Int64;
            _signedValue = longValue;
        }

        public long AsInt64()
        {
            return (long)_signedValue;
        }

        public StormValue(byte byteValue)
        {
            _type = Type.Byte;
            _unsignedValue = byteValue;
        }

        public byte AsByte()
        {
            return (byte)_unsignedValue;
        }

        public StormValue(ushort ushortValue)
        {
            _type = Type.UInt16;
            _unsignedValue = ushortValue;
        }

        public ushort AsUInt16()
        {
            return (ushort)_unsignedValue;
        }

        public StormValue(uint uintValue)
        {
            _type = Type.UInt32;
            _unsignedValue = uintValue;
        }

        public uint AsUInt32()
        {
            return (uint)_unsignedValue;
        }

        public StormValue(ulong ulongValue)
        {
            _type = Type.UInt64;
            _unsignedValue = ulongValue;
        }

        public ulong AsUInt64()
        {
            return (ulong)_unsignedValue;
        }

        public StormValue(float floatValue)
        {
            _type = Type.Single;
            _doubleValue = floatValue;
        }

        public float AsSingle()
        {
            return (float)_doubleValue;
        }

        public StormValue(double doubleValue)
        {
            _type = Type.Double;
            _doubleValue = doubleValue;
        }

        public double AsDouble()
        {
            return _doubleValue;
        }

        public StormValue(decimal decimalValue)
        {
            _type = Type.Decimal;
            _decimalValue = decimalValue;
        }

        public decimal AsDecimal()
        {
            return _decimalValue;
        }

        public StormValue(string stringValue)
        {
            _type = Type.String;
            _stringValue = stringValue;
        }

        public string AsString()
        {
            return _stringValue;
        }

        public StormValue()
        {
            _type = Type.Null;
        }

        public object AsNull()
        {
            return null;
        }

        public void Populate(IStormVariable variable, StormSettings settings)
        {
            var value = GetValue();
            variable.SetValue(value);
        }

        public object GetValue()
        {
            switch (_type)
            {
                case Type.Boolean:
                    return AsBool();
                case Type.Byte:
                    return AsByte();
                case Type.SByte:
                    return AsSByte();
                case Type.Int16:
                    return AsInt16();
                case Type.Int32:
                    return AsInt32();
                case Type.Int64:
                    return AsInt64();
                case Type.UInt16:
                    return AsUInt16();
                case Type.UInt32:
                    return AsUInt32();
                case Type.UInt64:
                    return AsUInt64();
                case Type.Single:
                    return AsSingle();
                case Type.Double:
                    return AsDouble();
                case Type.Decimal:
                    return AsDecimal();
                case Type.String:
                    return AsString();
                case Type.Null:
                    return AsNull();
                default:
                    throw new Exception($"unsupported type {_type}");
            }
        }

        public override string ToString()
        {
            var value = GetValue();
            if (value == null)
                return "[null]";

            return value.ToString();
        }

        public enum Type
        {
            Boolean     = 0,
            Byte        = 1,
            SByte       = 2,
            Int16       = 3,
            Int32       = 4,
            Int64       = 5,
            UInt16      = 6,
            UInt32      = 7,
            UInt64      = 8,
            Single      = 9,
            Double      = 10,
            Decimal     = 11,
            String      = 12,
            Null        = 13,
        }
    }
}
