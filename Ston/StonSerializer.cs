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
            var length = lines.Length;
            for (int i = 0; i < length; ++i)
            {
                var line = lines[i];

                var firstIndex = line.IndexOfAnySymbol();
                if (firstIndex < 0)
                    continue;

                if (line[firstIndex] == '#')
                    continue;

                var separatorIndex = line.IndexOf(':');
                var key = line.Substring(firstIndex, separatorIndex - firstIndex, StonExtensions.SubstringOptions.Trimmed);

                var typeAndValueIndex = separatorIndex + 1;
                var typeAndValue = line.Substring(typeAndValueIndex, line.Length - typeAndValueIndex);

                var equalIndex = typeAndValue.IndexOf('=');

                var type = typeAndValue.Substring(0, equalIndex - 0, StonExtensions.SubstringOptions.Trimmed);
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
            if (TryGetConverter(_converters, type, out result))
                return true;

            if (TryGetConverter(settings.converters, type, out result))
                return true;

            return false;
        }

        private static bool TryGetConverter(IEnumerable<IStonConverter> list, string type, out IStonConverter result)
        {
            if (list == null)
            {
                result = default;
                return false;
            }

            foreach (var converter in list)
            {
                if (converter.CanConvert(type))
                {
                    result = converter;
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}
