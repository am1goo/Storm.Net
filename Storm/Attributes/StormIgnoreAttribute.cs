using System;

namespace Storm.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormIgnoreAttribute : Attribute
    {
    }
}
