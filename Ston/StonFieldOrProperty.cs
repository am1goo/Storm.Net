using System;
using System.Reflection;

namespace Ston
{
    public class StonFieldOrProperty
    {
        private FieldOrProperty _type;
        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;

        public Type targetType
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

        public StonFieldOrProperty(FieldInfo fieldInfo)
        {
            _type = FieldOrProperty.Field;
            _fieldInfo = fieldInfo;
        }

        public StonFieldOrProperty(PropertyInfo propertyInfo)
        {
            _type = FieldOrProperty.Property;
            _propertyInfo = propertyInfo;
        }

        public void SetValue(object obj, object value)
        {
            switch (_type)
            {
                case FieldOrProperty.Field:
                    _fieldInfo.SetValue(obj, value);
                    break;

                case FieldOrProperty.Property:
                    _propertyInfo.SetValue(obj, value);
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
