using System.Collections.Generic;

namespace Ston
{
    public class StonObject
    {
        private Dictionary<string, StonValue> _values;

        public StonObject()
        {
            _values = new Dictionary<string, StonValue>();
        }

        internal void Add(string key, StonValue value)
        {
            _values.Add(key, value);
        }
    }
}
