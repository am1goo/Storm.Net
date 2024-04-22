using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Storm
{
    public class StormObject : IStormValue, IStormContainer
    {
        private Dictionary<string, IStormValue> _entries;

        public IStormValue this[string key]
        {
            get
            {
                return _entries[key];
            }
        }

        public StormObject()
        {
            _entries = new Dictionary<string, IStormValue>();
        }

        void IStormContainer.Add(string key, IStormValue value)
        {
            _entries.Add(key, value);
        }

        public void Populate(IStormVariable variable, StormSettings settings)
        {
            var value = Populate(variable.type, settings);
            variable.SetValue(value);
        }

        public T Populate<T>(StormSettings settings = default)
        {
            var type = typeof(T);
            return (T)Populate(type, settings);
        }

        public object Populate(Type type, StormSettings settings)
        {
            if (settings == null)
                settings = StormSettings.Default();

            var ignoreCase = settings.options.HasFlag(StormSettings.Options.IgnoreCase);

            var obj = Activator.CreateInstance(type);
            var pis = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);

            StormCache<List<StormFieldOrProperty>>.Pop(out var cache);
            foreach (var pi in pis)
            {
                if (pi.ShouldBeIgnored())
                    continue;

                var fieldOrProperty = new StormFieldOrProperty(obj, pi);
                cache.Add(fieldOrProperty);
            }
            foreach (var fi in fis)
            {
                if (fi.ShouldBeIgnored())
                    continue;

                var fieldOrProperty = new StormFieldOrProperty(obj, fi);
                cache.Add(fieldOrProperty);
            }
            foreach (var variable in cache)
            {
                if (TryGetValue(variable.name, ignoreCase, variable.type, out var value))
                {
                    value.Populate(variable, settings);
                }
            }
            StormCache<List<StormFieldOrProperty>>.Push(cache);
            return obj;
        }

        private bool TryGetValue(string key, bool ignoreCase, Type type, out IStormValue result)
        {
            if (ignoreCase)
            {
                var exist = default(IStormValue);
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
            StormCache<StringBuilder>.Pop(out var sb);

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
            StormCache<StringBuilder>.Push(sb);
            return str;
        }
    }
}
