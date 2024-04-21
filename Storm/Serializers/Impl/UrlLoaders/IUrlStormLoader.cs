using System;
using System.Threading.Tasks;

namespace Storm.Serializers.Loaders
{
    public interface IUrlStormLoader
    {
        Task<StormObject> DeserializeAsync(Uri uri, StormContext ctx);
    }
}
