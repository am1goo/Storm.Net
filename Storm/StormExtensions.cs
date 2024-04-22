using Storm.Attributes;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Storm
{
    public static class StormExtensions
    {
        private static readonly Type[] _includeAttributes = new Type[]
        {
            typeof(StormIncludeAttribute),
        };

        private static readonly Type[] _ignoreAttributes = new Type[]
        {
            typeof(StormIgnoreAttribute),
            typeof(IgnoreDataMemberAttribute),
        };

        public static StringBuilder AppendKey(this StringBuilder sb, string key)
        {
            if (!string.IsNullOrEmpty(key))
                sb.Append(key).Append(' ');
            return sb.Append('=').Append(' ');
        }

        public static bool IsMultiline(this string str)
        {
            return str.Contains("\r") || str.Contains("\n");
        }

        public static bool IsPrivate(this PropertyInfo pi)
        {
            var getMethod = pi.GetMethod;
            if (getMethod == null || getMethod.IsPrivate)
                return true;

            var setMethod = pi.SetMethod;
            if (setMethod == null || setMethod.IsPrivate)
                return true;

            return false;
        }

        public static bool ShouldBeIgnored(this PropertyInfo pi)
        {
            return HasAttributes(pi, _ignoreAttributes);
        }

        public static bool ShouldBeIgnored(this FieldInfo fi)
        {
            return HasAttributes(fi, _ignoreAttributes);
        }

        public static bool ShouldBeIncluded(this PropertyInfo pi)
        {
            return HasAttributes(pi, _includeAttributes);
        }

        public static bool ShouldBeIncluded(this FieldInfo fi)
        {
            return HasAttributes(fi, _includeAttributes);
        }

        public static bool HasAttributes(MemberInfo mi, params Type[] types)
        { 
            foreach (var attr in mi.CustomAttributes)
            {
                foreach (var type in types)
                {
                    if (attr.AttributeType == type)
                        return true;
                }
            }

            return false;
        }

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

        public static bool TryToStormType(this TypeCode typeCode, out StormValue.Type result)
        {
            try
            {
                result = ToStormType(typeCode);
                return true;
            }
            catch
            {
                result = default;
                return false;
            }
        }

        public static StormValue.Type ToStormType(this TypeCode typeCode)
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
