using System;
using System.Collections.Generic;

namespace Storm
{
    public interface IStormVariableRW : IStormVariableW
    {
        object GetValue();
    }

    public interface IStormVariableW : IStormVariable
    {
        void SetValue(object value);
    }

    public interface IStormVariable
    {
        string name { get; }
        Type type { get; }
        void GetAttributes(List<Attribute> result);
        
    }
}
