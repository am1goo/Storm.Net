using System;

namespace Ston
{
    public class StonValue
    {
        private Type _type;
        public Type type => _type;

        private long _signedValue;
        private ulong _unsignedValue;
        private double _doubleValue;
        private decimal _decimalValue;
        private string _stringValue;

        public StonValue(bool boolValue)
        {
            _type = Type.Boolean;
            _signedValue = boolValue ? 1 : 0;
        }

        public bool AsBool()
        {
            return _signedValue == 1;
        }

        public StonValue(sbyte sbyteValue)
        {
            _type = Type.SByte;
            _signedValue = sbyteValue;
        }

        public sbyte AsSByte()
        {
            return (sbyte)_signedValue;
        }

        public StonValue(short shortValue)
        {
            _type = Type.Int16;
            _signedValue = shortValue;
        }

        public short AsInt16()
        {
            return (short)_signedValue;
        }

        public StonValue(int intValue)
        {
            _type = Type.Int32;
            _signedValue = intValue;
        }

        public int AsInt32()
        {
            return (int)_signedValue;
        }

        public StonValue(long longValue)
        {
            _type = Type.Int64;
            _signedValue = longValue;
        }

        public long AsInt64()
        {
            return (long)_signedValue;
        }

        public StonValue(byte byteValue)
        {
            _type = Type.Byte;
            _unsignedValue = byteValue;
        }

        public byte AsByte()
        {
            return (byte)_unsignedValue;
        }

        public StonValue(ushort ushortValue)
        {
            _type = Type.UInt16;
            _unsignedValue = ushortValue;
        }

        public ushort AsUInt16()
        {
            return (ushort)_unsignedValue;
        }

        public StonValue(uint uintValue)
        {
            _type = Type.UInt32;
            _unsignedValue = uintValue;
        }

        public uint AsUInt32()
        {
            return (uint)_unsignedValue;
        }

        public StonValue(ulong ulongValue)
        {
            _type = Type.UInt64;
            _unsignedValue = ulongValue;
        }

        public ulong AsUInt64()
        {
            return (ulong)_unsignedValue;
        }

        public StonValue(float floatValue)
        {
            _type = Type.Single;
            _doubleValue = floatValue;
        }

        public float AsSingle()
        {
            return (float)_doubleValue;
        }

        public StonValue(double doubleValue)
        {
            _type = Type.Double;
            _doubleValue = doubleValue;
        }

        public double AsDouble()
        {
            return _doubleValue;
        }

        public StonValue(decimal decimalValue)
        {
            _type = Type.Decimal;
            _decimalValue = decimalValue;
        }

        public decimal AsDecimal()
        {
            return _decimalValue;
        }

        public StonValue(string stringValue)
        {
            _type = Type.String;
            _stringValue = stringValue;
        }

        public string AsString()
        {
            return _stringValue;
        }

        public override string ToString()
        {
            switch (_type)
            {
                case Type.Boolean:
                    return AsBool().ToString();
                case Type.Byte:
                    return AsByte().ToString();
                case Type.SByte:
                    return AsSByte().ToString();
                case Type.Int16:
                    return AsInt16().ToString();
                case Type.Int32:
                    return AsInt32().ToString();
                case Type.Int64:
                    return AsInt64().ToString();
                case Type.UInt16:
                    return AsUInt16().ToString();
                case Type.UInt32:
                    return AsUInt32().ToString();
                case Type.UInt64:
                    return AsUInt64().ToString();
                case Type.Single:
                    return AsSingle().ToString();
                case Type.Double:
                    return AsDouble().ToString();
                case Type.Decimal:
                    return AsDecimal().ToString();
                case Type.String:
                    return AsString().ToString();
                default:
                    throw new Exception($"unsupported type {_type}");
            }
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
        }
    }
}
