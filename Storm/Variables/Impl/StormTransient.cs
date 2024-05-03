using System;
using System.Collections.Generic;

namespace Storm
{
    public class StormTransient : IStormVariable
    {
        public string name => "";

        private Type _type;
        public Type type => _type;

        private object _value;
        public object value => _value;

        public StormTransient(Type type)
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
    }
}
