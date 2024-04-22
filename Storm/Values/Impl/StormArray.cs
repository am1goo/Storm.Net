using System;
using System.Collections.Generic;
using System.Text;

namespace Storm
{
    public class StormArray : IStormValue, IStormContainer
    {
        private List<IStormValue> _entries;

        public StormArray()
        {
            _entries = new List<IStormValue>();
        }

        public void Add(string key, IStormValue value)
        {
            _entries.Add(value);
        }

        public void Populate(IStormVariable variable, StormSettings settings)
        {
            var type = variable.type;
            if (!type.IsArray)
                throw new Exception($"target type {variable.type} should be an array");

            const int expectedRank = 1;
            var actualRank = type.GetArrayRank();
            if (actualRank != expectedRank)
                throw new Exception($"unsupported array rank, actual={actualRank}, expected={expectedRank}");

            if (!type.HasElementType)
                throw new Exception($"array should to have element type");

            var elementType = type.GetElementType();
            var elementCount = _entries.Count;
            var array = Array.CreateInstance(elementType, elementCount);

            for (int i = 0; i < elementCount; ++i)
            {
                var entry = _entries[i];
                var elementVar = new StormArrayElement(elementType, array, i);
                entry.Populate(elementVar, settings);
            }
            variable.SetValue(array);
        }

        public override string ToString()
        {
            StormCache<StringBuilder>.Pop(out var sb);
            foreach (var entry in _entries)
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(entry);
            }
            var str = sb.ToString();
            sb.Clear();
            StormCache<StringBuilder>.Push(sb);
            return str;
        }
    }
}
