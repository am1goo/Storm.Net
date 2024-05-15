using System;
using System.Collections.Generic;
using System.Reflection;

namespace Storm
{
    public struct StormPropertyInfo : IStormVariableRW
    {
        public string name => _propertyInfo.Name;
        public Type type => _propertyInfo.PropertyType;

        private object _obj;
        private PropertyInfo _propertyInfo;

        public StormPropertyInfo(object obj, PropertyInfo propertyInfo)
        {
            _obj = obj;
            _propertyInfo = propertyInfo;
        }

        public void GetAttributes(List<Attribute> result)
        {
            var attrs = _propertyInfo.GetCustomAttributes();
            foreach (var attr in attrs)
            {
                result.Add(attr);
            }
        }

        public void SetValue(object value)
        {
            _propertyInfo.SetValue(_obj, value);
        }

        public object GetValue()
        {
            return _propertyInfo.GetValue(_obj);
        }
    }
}
