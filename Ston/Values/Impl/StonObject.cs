using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Ston
{
    public class StonObject : IStonValue, IStonContainer
    {
        private Dictionary<string, IStonValue> _entries;

        public IStonValue this[string key]
        {
            get
            {
                return _entries[key];
            }
        }

        public StonObject()
        {
            _entries = new Dictionary<string, IStonValue>();
        }

        void IStonContainer.Add(string key, IStonValue value)
        {
            _entries.Add(key, value);
        }

        public void Populate(IStonVariable variable, StonSettings settings)
        {
            var value = Populate(variable.type, settings);
            variable.SetValue(value);
        }

        public T Populate<T>(StonSettings settings = default)
        {
            var type = typeof(T);
            return (T)Populate(type, settings);
        }

        public object Populate(Type type, StonSettings settings)
        {
            if (settings == null)
                settings = StonSettings.defaultSettings;

            var ignoreCase = settings.options.HasFlag(StonSettings.Options.IgnoreCase);

            var obj = Activator.CreateInstance(type);
            var pis = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);
            foreach (var pi in pis)
            {
                if (TryGetValue(pi.Name, ignoreCase, pi.PropertyType, out var value))
                {
                    var fieldOrProperty = new StonFieldOrProperty(obj, pi);
                    value.Populate(fieldOrProperty, settings);
                }
            }
            foreach (var fi in fis)
            {
                if (TryGetValue(fi.Name, ignoreCase, fi.FieldType, out var value))
                {
                    var fieldOrProperty = new StonFieldOrProperty(obj, fi);
                    value.Populate(fieldOrProperty, settings);
                }
            }
            return obj;
        }

        private bool TryGetValue(string key, bool ignoreCase, Type type, out IStonValue result)
        {
            if (ignoreCase)
            {
                var exist = default(IStonValue);
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
                    result = exist;
                    return true;
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
                    result = exist;
                    return true;
                }
                else
                {
                    result = default;
                    return false;
                }
            }
        }

        private static int _toStringIntent = 0;

        public override string ToString()
        {
            var intent = _toStringIntent;
            StonCache<StringBuilder>.Pop(out var sb);

            _toStringIntent++;
            sb.Length = 0;
            if (intent > 0)
                sb.Append(Environment.NewLine);
            foreach (var entry in _entries)
            {
                var key = entry.Key;
                var value = entry.Value;
                sb.Append(' ', intent * 4);
                sb.AppendLine($"{key}\t{value}");
            }
            var str = sb.ToString();
            _toStringIntent--;

            sb.Clear();
            StonCache<StringBuilder>.Push(sb);
            return str;
        }
    }
}
