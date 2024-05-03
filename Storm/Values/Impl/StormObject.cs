using System;
using System.Collections.Generic;
using System.Text;

namespace Storm
{
    public class StormObject : IStormValue, IStormContainer
    {
        private Dictionary<string, IStormValue> _entries;
        public IReadOnlyDictionary<string, IStormValue> entries => _entries;

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

        public void GetEntries(List<IStormValue> result)
        {
            foreach (var entry in _entries.Values)
            {
                result.Add(entry);
            }
        }

        void IStormContainer.Add(string key, IStormValue value)
        {
            _entries.Add(key, value);
        }

        public T Populate<T>(StormContext ctx)
        {
            var type = typeof(T);
            return (T)Populate(type, ctx);
        }

        public object Populate(Type type, StormContext ctx)
        {
            var variable = new StormTransient(type);
            Populate(variable, ctx);
            return variable.value;
        }

        public void Populate(IStormVariable variable, StormContext ctx)
        {
            var type = variable.type;

            if (!ctx.serializer.TryGetSerializer(type, ctx.settings, out var serializer))
                throw new Exception($"serializer not found for type {type}");

            serializer.Populate(variable, this, ctx);
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
