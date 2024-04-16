using System;
using System.Collections.Generic;
using System.Reflection;

namespace Ston
{
    public class StonObject
    {
        private Dictionary<string, StonValue> _entries;
        public IReadOnlyDictionary<string, StonValue> entries => _entries;

        public StonValue this[string key]
        {
            get
            {
                return _entries[key];
            }
        }

        public StonObject()
        {
            _entries = new Dictionary<string, StonValue>();
        }

        internal void Add(string key, StonValue value)
        {
            _entries.Add(key, value);
        }

        public T Populate<T>(StonSettings settings = default)
        {
            if (settings == null)
                settings = StonSettings.defaultSettings;

            var type = typeof(T);
            var ignoreCase = settings.options.HasFlag(StonSettings.Options.IgnoreCase);

            var obj = Activator.CreateInstance<T>();
            var pis = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);
            foreach (var pi in pis)
            {
                if (TryGetValue(pi.Name, ignoreCase, pi.PropertyType, out var value))
                {
                    pi.SetValue(obj, value);
                }
            }
            foreach (var fi in fis)
            {
                if (TryGetValue(fi.Name, ignoreCase, fi.FieldType, out var value))
                {
                    fi.SetValue(obj, value);
                }
            }
            return obj;
        }

        private bool TryGetValue(string key, bool ignoreCase, Type type, out object result)
        {
            if (ignoreCase)
            {
                var exist = default(StonValue);
                foreach (var entryKey in _entries.Keys)
                {
                    if (key.Equals(entryKey, StringComparison.InvariantCultureIgnoreCase))
                    {
                        exist = _entries[entryKey];
                        break;
                    }
                }

                if (exist != null)
                {
                    return TryGetValue(exist, type, out result);
                }
                else
                {
                    result = default;
                    return false;
                }
            }
            else
            {
                if (_entries.TryGetValue(key, out var exist))
                {
                    return TryGetValue(exist, type, out result);
                }
                else
                {
                    result = default;
                    return false;
                }
            }
        }

        private bool TryGetValue(StonValue stonValue, Type type, out object result)
        {
            var typeCode = Type.GetTypeCode(type);
            var stonType = Convert(typeCode);
            if (stonValue.type != stonType)
            {
                result = default;
                return false;
            }

            result = stonValue.GetValue();
            return true;
        }

        private static StonValue.Type Convert(TypeCode typeCode)
        {
            switch (typeCode)
            {
                case TypeCode.Boolean: return StonValue.Type.Boolean;
                case TypeCode.Byte: return StonValue.Type.Byte;
                case TypeCode.SByte: return StonValue.Type.SByte;
                case TypeCode.Int16: return StonValue.Type.Int16;
                case TypeCode.Int32: return StonValue.Type.Int32;
                case TypeCode.Int64: return StonValue.Type.Int64;
                case TypeCode.UInt16: return StonValue.Type.UInt16;
                case TypeCode.UInt32: return StonValue.Type.UInt32;
                case TypeCode.UInt64: return StonValue.Type.UInt64;
                case TypeCode.Single: return StonValue.Type.Single;
                case TypeCode.Double: return StonValue.Type.Double;
                case TypeCode.Decimal: return StonValue.Type.Decimal;
                case TypeCode.String: return StonValue.Type.String;
                default: throw new Exception($"unsupported type {typeCode}");
            }
        }
    }
}
