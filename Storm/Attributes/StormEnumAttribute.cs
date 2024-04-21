using Storm.Serializers;
using System;

namespace Storm.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class StormEnumAttribute : Attribute
    {
        public StormEnumFormat format;
    }
}
