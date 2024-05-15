using System;
using System.Collections.Generic;
using System.Reflection;

namespace Storm
{
    public struct StormFieldInfo : IStormVariableRW
    {
        public string name => _fieldInfo.Name;
        public Type type => _fieldInfo.FieldType;

        private object _obj;
        private FieldInfo _fieldInfo;

        public StormFieldInfo(object obj, FieldInfo fieldInfo)
        {
            _obj = obj;
            _fieldInfo = fieldInfo;
        }

        public void GetAttributes(List<Attribute> result)
        {
            var attrs = _fieldInfo.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                result.Add(attr);
            }
        }

        public void SetValue(object value)
        {
            _fieldInfo.SetValue(_obj, value);
        }

        public object GetValue()
        {
            return _fieldInfo.GetValue(_obj);
        }
    }
}
