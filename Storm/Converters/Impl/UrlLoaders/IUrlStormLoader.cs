using System;
using System.Threading.Tasks;

namespace Storm.Converters.Loaders
{
    public interface IUrlStormLoader
    {
        Task<StormObject> DeserializeAsync(Uri uri, StormContext ctx);
    }
}
