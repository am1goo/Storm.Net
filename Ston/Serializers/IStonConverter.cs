namespace Ston.Serializers
{
    public interface IStonConverter
    {
        bool CanConvert(string type);
        StonValue Deserialize(string type, string text);
    }
}
