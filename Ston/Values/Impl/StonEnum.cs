using System;

namespace Ston
{
    public class StonEnum : IStonValue
    {
        private string _text;

        public StonEnum(string text)
        {
            _text = text;
        }

        public void Populate(IStonVariable variable, StonSettings settings)
        {
            var type = variable.type;
            if (!type.IsEnum)
                throw new Exception($"target type {variable.type} should be enum");

            if (!Enum.TryParse(type, _text, out var parsed))
                throw new Exception($"target type cannot be parsed from string '{_text}'");

            variable.SetValue(parsed);
        }

        public override string ToString()
        {
            return _text;
        }
    }
}