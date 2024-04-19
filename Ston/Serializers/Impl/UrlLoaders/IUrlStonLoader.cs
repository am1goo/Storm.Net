using System;
using System.Threading.Tasks;

namespace Ston.Serializers.Loaders
{
    internal interface IUrlStonLoader
    {
        Task<StonObject> DeserializeAsync(Uri uri, StonContext ctx);
    }
}
