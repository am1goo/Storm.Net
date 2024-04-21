using System;
using System.Reflection;

namespace Storm
{
    public class StormFieldOrProperty : IStormVariable
    {
        private object _obj;
        private FieldOrProperty _type;
        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;

        public Type type
        {
            get
            {
                switch (_type)
                {
                    case FieldOrProperty.Field:
                        return _fieldInfo.FieldType;
                    case FieldOrProperty.Property:
                        return _propertyInfo.PropertyType;
                    default:
                        throw new Exception($"unsupported type {_type}");
                }
            }
        }

        public StormFieldOrProperty(object obj, FieldInfo fieldInfo)
        {
            _obj = obj;
            _type = FieldOrProperty.Field;
            _fieldInfo = fieldInfo;
        }

        public StormFieldOrProperty(object obj, PropertyInfo propertyInfo)
        {
            _obj = obj;
            _type = FieldOrProperty.Property;
            _propertyInfo = propertyInfo;
        }

        public void SetValue(object value)
        {
            switch (_type)
            {
                case FieldOrProperty.Field:
                    _fieldInfo.SetValue(_obj, value);
                    break;

                case FieldOrProperty.Property:
                    _propertyInfo.SetValue(_obj, value);
                    break;
            }
        }

        public enum FieldOrProperty : byte
        {
            Field = 0,
            Property = 1,
        }
    }
}
