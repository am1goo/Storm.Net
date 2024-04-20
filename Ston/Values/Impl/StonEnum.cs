using System;
using System.Reflection;

namespace Ston
{
    public class StonEnum : IStonValue
    {
        private string _text;

        public StonEnum(string text)
        {
            _text = text;
        }

        public void Populate(StonFieldOrProperty fieldOrProperty, object obj, StonSettings settings)
        {
            var type = fieldOrProperty.targetType;
            if (!type.IsEnum)
                throw new Exception($"target type {fieldOrProperty.targetType} should be enum");

            if (!Enum.TryParse(type, _text, out var parsed))
                throw new Exception($"target type cannot be parsed from string '{_text}'");

            fieldOrProperty.SetValue(obj, parsed);
        }

        public override string ToString()
        {
            return _text;
        }
    }
}