using System;

namespace Storm
{
    public interface IStormVariable
    {
        Type type { get; }
        void SetValue(object value);
    }
}
