using System;

namespace Ston
{
    public class StonArrayElement : IStonVariable
    {
        private Type _type;
        public Type type => _type;

        private Array _array;
        private int _index;

        public StonArrayElement(Type type, Array array, int index)
        {
            _type = type;
            _array = array;
            _index = index;
        }

        public void SetValue(object value)
        {
            _array.SetValue(value, _index);
        }
    }
}
