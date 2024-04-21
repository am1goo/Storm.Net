using System;
using System.Collections.Generic;

namespace Storm
{
    public struct StormArrayElement : IStormVariable
    {
        private string _name;
        public string name => _name;

        private Type _type;
        public Type type => _type;

        private Array _array;
        private int _index;

        public StormArrayElement(Type type, Array array, int index)
        {
            _name = string.Empty;
            _type = type;
            _array = array;
            _index = index;
        }

        public void GetAttributes(List<Attribute> result)
        {
            //do nothing
        }

        public void SetValue(object value)
        {
            _array.SetValue(value, _index);
        }
    }
}
