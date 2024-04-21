using System.Threading.Tasks;

namespace Storm.Serializers
{
    public interface IStormConverter
    {
        bool CanConvert(string type);
        bool CanConvert(System.Type type);
        Task<IStormValue> DeserializeAsync(string type, string text, StormContext ctx);
        Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx);
    }
}
