using System;

namespace Ston
{
    public interface IStonVariable
    {
        Type type { get; }
        void SetValue(object value);
    }
}
