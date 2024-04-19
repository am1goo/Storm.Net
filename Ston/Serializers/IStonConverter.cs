namespace Ston.Serializers
{
    public interface IStonConverter
    {
        bool CanConvert(string type);
        IStonValue Deserialize(string type, string text, StonContext ctx);
    }
}
