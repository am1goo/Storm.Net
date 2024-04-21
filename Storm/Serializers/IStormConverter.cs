using System.Threading.Tasks;

namespace Storm.Serializers
{
    public interface IStormConverter
    {
        bool CanConvert(string type);
        Task<IStormValue> DeserializeAsync(string type, string text, StormContext ctx);
    }
}
