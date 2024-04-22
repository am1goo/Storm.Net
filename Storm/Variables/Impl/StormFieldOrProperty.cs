using System;
using System.Collections.Generic;
using System.Reflection;

namespace Storm
{
    public struct StormFieldOrProperty : IStormVariable
    {
        private object _obj;
        private FieldOrProperty _type;
        private FieldInfo _fieldInfo;
        private PropertyInfo _propertyInfo;

        public string name
        {
            get
            {
                switch (_type)
                {
                    case FieldOrProperty.Field:
                        return _fieldInfo.Name;
                    case FieldOrProperty.Property:
                        return _propertyInfo.Name;
                    default:
                        throw new Exception($"unsupported type {_type}");
                }
            }
        }

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

        public void GetAttributes(List<Attribute> result)
        {
            switch (_type)
            {
                case FieldOrProperty.Field:
                    {
                        var attrs = _fieldInfo.GetCustomAttributes();
                        foreach (var attr in attrs)
                        {
                            result.Add(attr);
                        }
                        break;
                    }
                case FieldOrProperty.Property:
                    {
                        var attrs = _propertyInfo.GetCustomAttributes();
                        foreach (var attr in attrs)
                        {
                            result.Add(attr);
                        }
                        break;
                    }
                default:
                    throw new Exception($"unsupported type {_type}");
            }
        }

        public StormFieldOrProperty(object obj, FieldInfo fieldInfo)
        {
            _obj = obj;
            _type = FieldOrProperty.Field;
            _fieldInfo = fieldInfo;
            _propertyInfo = null;
        }

        public StormFieldOrProperty(object obj, PropertyInfo propertyInfo)
        {
            _obj = obj;
            _type = FieldOrProperty.Property;
            _fieldInfo = null;
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

                default:
                    throw new Exception($"unsupported type {_type}");
            }
        }

        public object GetValue()
        {
            switch (_type)
            {
                case FieldOrProperty.Field:
                    return _fieldInfo.GetValue(_obj);

                case FieldOrProperty.Property:
                    return _propertyInfo.GetValue(_obj);

                default:
                    throw new Exception($"unsupported type {_type}");
            }
        }

        public enum FieldOrProperty : byte
        {
            Field = 0,
            Property = 1,
        }
    }
}
