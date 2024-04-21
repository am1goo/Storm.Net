using Storm.Attributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Storm.Serializers
{
    internal class EnumStormConverter : IStormConverter
    {
        public static readonly EnumStormConverter instance = new EnumStormConverter();

        private const string _type = "e";

        public bool CanConvert(string type)
        {
            return type.Equals(_type, StringComparison.InvariantCultureIgnoreCase);
        }

        public bool CanConvert(Type type)
        {
            return type.IsEnum;
        }

        public Task<IStormValue> DeserializeAsync(string type, string text, StormContext ctx)
        {
            var trimmed = text.Trim();
            var value = new StormEnum(trimmed);
            return Task.FromResult<IStormValue>(value);
        }

        public Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx)
        {
            var objType = variable.type;
            if (!objType.IsEnum)
                throw new Exception($"unsupported type {objType}");

            var format = GetEnumFormat(variable, defaultValue: ctx.settings.defaultEnumFormat);
            var val = default(object);
            switch (format)
            {
                case StormEnumFormat.String:
                    val = Enum.GetName(objType, obj);
                    break;
                case StormEnumFormat.Value:
                    val = Convert.ChangeType(obj, Type.GetTypeCode(objType));
                    break;
                default:
                    throw new Exception($"unsupported type {format}");
            }

            var key = variable.name;
            var str = $"{key}:{_type} = {val}";
            return Task.FromResult(str);
        }

        private static StormEnumFormat GetEnumFormat(IStormVariable variable, StormEnumFormat defaultValue)
        {
            var result = defaultValue;

            StormCache<List<Attribute>>.Pop(out var attrs);
            variable.GetAttributes(attrs);
            foreach (var attr in attrs)
            {
                if (attr is StormEnumAttribute enumAttr)
                {
                    result = enumAttr.format;
                    break;
                }
            }
            StormCache<List<Attribute>>.Push(attrs);

            return result;
        }
    }
}
