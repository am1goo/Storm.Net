using System;
using System.Threading.Tasks;

namespace Ston.Serializers.Loaders
{
    public interface IUrlStonLoader
    {
        Task<StonObject> DeserializeAsync(Uri uri, StonContext ctx);
    }
}
