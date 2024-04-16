using Ston.Serializers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace Ston
{
    public class StonSerializer
    {
        private IReadOnlyList<IStonConverter> _converters = new List<IStonConverter>
        {
            PrimitiveStonConverter.instance,
        };

        public Task<StonObject> DeserializeFileAsync(string filePath, StonSettings settings = default)
        {
            var fi = new FileInfo(filePath);
            return DeserializeFileAsync(fi, settings);
        }

        public async Task<StonObject> DeserializeFileAsync(FileInfo fileInfo, StonSettings settings = default)
        {
            if (!fileInfo.Exists)
                return null;

            using (var fs = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var ston = await sr.ReadToEndAsync();
                    return Deserialize(ston, settings);
                }
            }
        }

        public StonObject Deserialize(string ston, StonSettings settings = default)
        {
            if (settings == null)
                settings = StonSettings.defaultSettings;

            var obj = new StonObject();
            var lines = ston.Split(Environment.NewLine);
            foreach (var line in lines)
            {
                var separatorIndex = line.IndexOf(':');
                var key = line.Substring(0, separatorIndex).Trim();

                var typeAndValueIndex = separatorIndex + 1;
                var typeAndValue = line.Substring(typeAndValueIndex, line.Length - typeAndValueIndex);

                var equalIndex = typeAndValue.IndexOf('=');
                var type = typeAndValue.Substring(0, equalIndex).Trim();
                if (!TryGetConverter(type, settings, out var converter))
                    throw new Exception($"unsupported type '{type}'");

                var valueIndex = equalIndex + 1;
                var valueStr = typeAndValue.Substring(valueIndex, typeAndValue.Length - valueIndex);

                var stonValue = converter.Deserialize(type, valueStr);
                if (stonValue == null)
                    throw new Exception($"converter {converter} should to return {nameof(StonValue)} for key '{key}' and type '{type}'");

                obj.Add(key, stonValue);
            }
            return obj;
        }

        private bool TryGetConverter(string type, StonSettings settings, out IStonConverter result)
        {
            for (int i = 0; i < _converters.Count; ++i)
            {
                var converter = _converters[i];
                if (converter.CanConvert(type))
                {
                    result = converter;
                    return true;
                }
            }

            if (settings.converters != null)
            {
                for (int i = 0; i < settings.converters.Count; ++i)
                {
                    var converter = settings.converters[i];
                    if (converter.CanConvert(type))
                    {
                        result = converter;
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }
    }
}
