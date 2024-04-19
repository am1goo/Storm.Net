using Ston.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Ston
{
    public class StonSerializer
    {
        private const char Separator    = ':';
        private const char Equal        = '=';
        private const char BraceStart   = '{';
        private const char BraceEnd     = '}';

        private IReadOnlyList<IStonConverter> _converters = new List<IStonConverter>
        {
            PrimitiveStonConverter.instance,
            UrlStonConverter.instance,
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

            settings.SetCwd(fileInfo.Directory.FullName);
            using (var fs = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(fs))
                {
                    var ston = await sr.ReadToEndAsync();
                    return await DeserializeAsync(ston, settings);
                }
            }
        }

        public Task<StonObject> DeserializeAsync(string ston, StonSettings settings)
        {
            if (settings == null)
                settings = StonSettings.defaultSettings;

            var ctx = new StonContext(this, settings, settings.cwd);
            return DeserializeAsync(ston, ctx);
        }

        internal async Task<StonObject> DeserializeAsync(string ston, StonContext ctx)
        {
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

                var separatorIndex = line.IndexOf(Separator);
                if (separatorIndex < 0)
                {
                    var result = await TryParseObject(i, lines, ctx);
                    if (result.valid)
                    {
                        i = result.index;
                        obj.Add(result.key, result.value);
                    }
                }
                else
                {
                    var result = await TryParseValue(i, lines, ctx);
                    if (result.valid)
                    {
                        i = result.index;
                        obj.Add(result.key, result.value);
                    }
                }
            }
            return obj;
        }

        private async Task<ParsedValue> TryParseObject(int index, string[] lines, StonContext ctx)
        {
            var line = lines[index];

            var equalIndex = line.IndexOf(Equal);
            if (equalIndex < 0)
                return ParsedValue.Invalid();

            var key = line.Substring(0, equalIndex, StonExtensions.SubstringOptions.Trimmed);

            var braceStartIndex = line.IndexOf(BraceStart);
            if (braceStartIndex < 0)
                return ParsedValue.Invalid();

            var intent = 0;
            var ston = default(string);
            StonCache<StringBuilder>.Pop(out var sb);
            for (int i = index; i < lines.Length; ++i)
            {
                index = i;

                line = lines[i];

                var skip = false;
                for (int n = 0; n < line.Length; ++n)
                {
                    var c = line[n];
                    if (c == BraceStart)
                    {
                        skip = intent == 0;
                        intent++;
                    }
                    else if (c == BraceEnd)
                    {
                        intent--;
                        skip = intent == 0;
                    }
                }

                var append = !skip;
                if (append)
                {
                    if (sb.Length > 0)
                        sb.Append(Environment.NewLine);
                    sb.Append(line);
                }

                if (intent != 0)
                    continue;

                ston = sb.ToString();
                break;
            }
            StonCache<StringBuilder>.Push(sb);

            if (ston == null)
                return ParsedValue.Invalid();

            var parsed = await DeserializeAsync(ston, ctx);
            return new ParsedValue(index, key, parsed);
        }

        private async Task<ParsedValue> TryParseValue(int index, string[] lines, StonContext ctx)
        {
            var line = lines[index];
            var separatorIndex = line.IndexOf(Separator);
            if (separatorIndex < 0)
                throw new Exception($"character '{Separator}' wasn't found in line {index}");

            var key = line.Substring(0, separatorIndex, StonExtensions.SubstringOptions.Trimmed);

            var typeAndValueIndex = separatorIndex + 1;
            var typeAndValue = line.Substring(typeAndValueIndex, line.Length - typeAndValueIndex);

            var equalIndex = typeAndValue.IndexOf(Equal);

            var type = typeAndValue.Substring(0, equalIndex - 0, StonExtensions.SubstringOptions.Trimmed);
            if (!TryGetConverter(type, ctx.settings, out var converter))
                throw new Exception($"unsupported type '{type}'");

            var valueIndex = equalIndex + 1;
            var valueStr = typeAndValue.Substring(valueIndex, typeAndValue.Length - valueIndex);

            var stonValue = await converter.DeserializeAsync(type, valueStr, ctx);
            if (stonValue == null)
                throw new Exception($"converter {converter} should to return {nameof(StonValue)} for key '{key}' and type '{type}'");

            return new ParsedValue(index, key, stonValue);
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

        private struct ParsedValue
        {
            public bool valid;
            public int index;
            public string key;
            public IStonValue value;

            public ParsedValue(int index, string key, IStonValue value)
            {
                this.valid = true;
                this.index = index;
                this.key = key;
                this.value = value;
            }

            public static ParsedValue Invalid()
            {
                return new ParsedValue
                {
                    valid = false
                };
            }
        }
    }
}
