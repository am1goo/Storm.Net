using System;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    public interface IStormSerializer
    {
        bool CanConvert(Type type);
        bool TryParse(ref int index, string[] lines, out string key, out string text);
        Task<IStormValue> DeserializeAsync(string text, StormContext ctx);
        Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx);
    }
}
