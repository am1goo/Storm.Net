using System;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    public class EnumStormConverter : IStormConverter
    {
        public static readonly EnumStormConverter instance = new EnumStormConverter();

        public bool CanConvert(string type)
        {
            return type.Equals("e", StringComparison.InvariantCultureIgnoreCase);
        }

        public Task<IStormValue> DeserializeAsync(string type, string text, StormContext ctx)
        {
            var trimmed = text.Trim();
            var value = new StormEnum(trimmed);
            return Task.FromResult<IStormValue>(value);
        }
    }
}
