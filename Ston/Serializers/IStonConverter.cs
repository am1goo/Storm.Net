using System.Threading.Tasks;

namespace Ston.Serializers
{
    public interface IStonConverter
    {
        bool CanConvert(string type);
        Task<IStonValue> DeserializeAsync(string type, string text, StonContext ctx);
    }
}
