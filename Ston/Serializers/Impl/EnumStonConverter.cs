using System;
using System.Threading.Tasks;

namespace Ston.Serializers
{
    public class EnumStonConverter : IStonConverter
    {
        public static readonly EnumStonConverter instance = new EnumStonConverter();

        public bool CanConvert(string type)
        {
            return type.Equals("e", StringComparison.InvariantCultureIgnoreCase);
        }

        public Task<IStonValue> DeserializeAsync(string type, string text, StonContext ctx)
        {
            var trimmed = text.Trim();
            var value = new StonEnum(trimmed);
            return Task.FromResult<IStonValue>(value);
        }
    }
}
