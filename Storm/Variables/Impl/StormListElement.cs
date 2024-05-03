using System;
using System.Collections;
using System.Collections.Generic;

namespace Storm
{
    public struct StormListElement : IStormVariable
    {
        private string _name;
        public string name => _name;

        private Type _type;
        public Type type => _type;

        private IList _list;
        private int _index;

        public StormListElement(Type type, IList list, int index)
        {
            _name = string.Empty;
            _type = type;
            _list = list;
            _index = index;
        }

        public void GetAttributes(List<Attribute> result)
        {
            //do nothing
        }

        public void SetValue(object value)
        {
            _list.Insert(_index, value);
        }
    }
}
