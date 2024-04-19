using System;

namespace Ston.Serializers.Loaders
{
    internal interface IUrlStonLoader
    {
        StonObject Deserialize(Uri uri, StonContext ctx);
    }
}
