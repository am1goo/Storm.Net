using System;
using System.Collections.Generic;

namespace Storm
{
    public interface IStormVariable
    {
        string name { get; }
        Type type { get; }
        void GetAttributes(List<Attribute> result);
        void SetValue(object value);
    }
}
