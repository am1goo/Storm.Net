using System;
using System.Collections.Generic;
using System.Text;

namespace Ston
{
    public class StonArray : IStonValue, IStonContainer
    {
        private List<IStonValue> _entries;

        public StonArray()
        {
            _entries = new List<IStonValue>();
        }

        public void Add(string key, IStonValue value)
        {
            _entries.Add(value);
        }

        public void Populate(IStonVariable variable, StonSettings settings)
        {
            var type = variable.type;
            if (!type.IsArray)
                throw new Exception($"target type {variable.type} should be an array");

            const int expectedRank = 1;
            var actualRank = type.GetArrayRank();
            if (actualRank != 1)
                throw new Exception($"unsupported array rank, actual={actualRank}, expected={expectedRank}");

            if (!type.HasElementType)
                throw new Exception($"array should to have element type");

            var elementType = type.GetElementType();
            var elementCount = _entries.Count;
            var array = Array.CreateInstance(elementType, elementCount);

            for (int i = 0; i < elementCount; ++i)
            {
                var entry = _entries[i];
                var elementVar = new StonArrayElement(elementType, array, i);
                entry.Populate(elementVar, settings);
            }
            variable.SetValue(array);
        }

        public override string ToString()
        {
            StonCache<StringBuilder>.Pop(out var sb);
            foreach (var entry in _entries)
            {
                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(entry);
            }
            var str = sb.ToString();
            sb.Clear();
            StonCache<StringBuilder>.Push(sb);
            return str;
        }
    }
}
