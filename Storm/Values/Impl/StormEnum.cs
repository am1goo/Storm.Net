using System;
using System.Collections.Generic;

namespace Storm
{
    public class StormEnum : IStormValue
    {
        private string _text;

        public StormEnum(string text)
        {
            _text = text;
        }

        public void GetEntries(List<IStormValue> entries)
        {
            //do nothing
        }

        public void Populate(IStormVariable variable, StormContext ctx)
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