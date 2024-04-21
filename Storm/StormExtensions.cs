using System;

namespace Storm
{
    public static class StormExtensions
    {
        public static string Substring(this string text, int startIndex, int length, SubstringOptions options)
        {
            if (options.HasFlag(SubstringOptions.Trimmed))
            {
                var firstIndex = IndexOfAnySymbol(text);
                if (firstIndex < 0)
                    return string.Empty;

                var newIndex = Math.Max(startIndex, firstIndex);
                var delta = newIndex - startIndex;
                startIndex = newIndex;

                var lastIndex = LastIndexOfAnySymbol(text, Math.Min(text.Length, startIndex + (length - delta)));
                if (lastIndex < 0)
                    return string.Empty;

                length = Math.Min(length, lastIndex + 1 - startIndex);
            }
            return text.Substring(startIndex, length);
        }

        public static int IndexOfAnySymbol(this string text, int startIndex = 0)
        {
            var length = text.Length;
            for (int i = startIndex; i < length; ++i)
            {
                var c = text[i];
                if (char.IsWhiteSpace(c))
                    continue;

                return i;
            }

            return -1;
        }

        public static int LastIndexOfAnySymbol(this string text, int startIndex = -1)
        {
            var length = text.Length;
            if (startIndex < 0)
                startIndex = length;

            for (int i = startIndex - 1; i >= 0; --i)
            {
                var c = text[i];
                if (char.IsWhiteSpace(c))
                    continue;

                return i;
            }

            return -1;
        }

        private static StormValue.Type ToStormType(this TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return StormValue.Type.Boolean;
                case TypeCode.Byte: return StormValue.Type.Byte;
                case TypeCode.SByte: return StormValue.Type.SByte;
                case TypeCode.Int16: return StormValue.Type.Int16;
                case TypeCode.Int32: return StormValue.Type.Int32;
                case TypeCode.Int64: return StormValue.Type.Int64;
                case TypeCode.UInt16: return StormValue.Type.UInt16;
                case TypeCode.UInt32: return StormValue.Type.UInt32;
                case TypeCode.UInt64: return StormValue.Type.UInt64;
                case TypeCode.Single: return StormValue.Type.Single;
                case TypeCode.Double: return StormValue.Type.Double;
                case TypeCode.Decimal: return StormValue.Type.Decimal;
                case TypeCode.String: return StormValue.Type.String;
                default: throw new Exception($"unsupported type {typeCode}");
            }
        }

        public enum SubstringOptions
        {
            Trimmed = 1 << 0,
        }
    }
}
