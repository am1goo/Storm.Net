using System;
using System.Collections.Generic;

namespace Storm
{
    public class StormTransient : IStormVariableRW
    {
        public string name => "";

        private Type _type;
        public Type type => _type;

        private object _value;

        public StormTransient(Type type) : this(type, null)
        {
        }

        public StormTransient(Type type, object value)
        {
            _type = type;
            _value = value;
        }

        public void GetAttributes(List<Attribute> result)
        {
            //do nothing
        }

        public void SetValue(object value)
        {
            _value = value;
        }

        public object GetValue()
        {
            return _value;
        }
    }
}
