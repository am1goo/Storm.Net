using Ston.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ston
{
    public class StonSerializer
    {
        private const char Separator    = ':';
        private const char Equal        = '=';
        private const char Quote        = '"';
        private const char BraceStart   = '{';
        private const char BraceEnd     = '}';

        private IReadOnlyList<IStonConverter> _converters = new List<IStonConverter>
        {
            PrimitiveStonConverter.instance,
            EnumStonConverter.instance,
            UrlStonConverter.instance,
        };

        public async Task<T> DeserializeFileAsync<T>(string filePath, StonSettings settings = default)
        {
            var obj = await DeserializeFileAsync(filePath, settings);
            return obj.Populate<T>(settings);
        }

        public async Task<object> DeserializeFileAsync(Type type, string filePath, StonSettings settings = default)
        {
            var obj = await DeserializeFileAsync(filePath, settings);
            return obj.Populate(type, settings);
        }

        public Task<StonObject> DeserializeFileAsync(string filePath, StonSettings settings = default)
        {
            var fi = new FileInfo(filePath);
            return DeserializeFileAsync(fi, settings);
        }

        public async Task<T> DeserializeFileAsync<T>(FileInfo fileInfo, StonSettings settings = default)
        {
            var obj = await DeserializeFileAsync(fileInfo, settings);
            return obj.Populate<T>(settings);
        }

        public async Task<object> DeserializeFileAsync(Type type, FileInfo fileInfo, StonSettings settings = default)
        {
            var obj = await DeserializeFileAsync(fileInfo, settings);
            return obj.Populate(type, settings);
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

        public async Task<T> DeserializeAsync<T>(string ston, StonSettings settings)
        {
            var obj = await DeserializeAsync(ston, settings);
            return obj.Populate<T>(settings);
        }

        public async Task<object> DeserializeAsync(Type type, string ston, StonSettings settings)
        {
            var obj = await DeserializeAsync(ston, settings);
            return obj.Populate(type, settings);
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

        private async Task<ParsedValue<IStonValue>> TryParseObject(int index, string[] lines, StonContext ctx)
        {
            var line = lines[index];
            var equalIndex = line.IndexOf(Equal);
            if (equalIndex < 0)
                return ParsedValue<IStonValue>.Invalid();

            var key = line.Substring(0, equalIndex, StonExtensions.SubstringOptions.Trimmed);

            if (!TryGetStringBetweenTwoChars(ref index, lines, BraceStart, BraceEnd, out var ston))
                return ParsedValue<IStonValue>.Invalid();

            var parsed = await DeserializeAsync(ston, ctx);
            return new ParsedValue<IStonValue>(index, key, parsed);
        }

        private static bool TryGetStringBetweenOneChar(ref int index, string[] lines, char charOne, out string result)
        {
            var line = lines[index];
            var charStartIndex = line.IndexOf(charOne);
            if (charStartIndex < 0)
            {
                result = default;
                return false;
            }

            var started = false;
            var ended = false;
            var unescape = false;
            StonCache<StringBuilder>.Pop(out var sb);
            for (int i = index; i < lines.Length; ++i)
            {
                index = i;

                line = lines[i];

                if (started)
                    sb.Append(Environment.NewLine);

                for (int n = 0; n < line.Length; ++n)
                {
                    var c = line[n];
                    if (c == charOne)
                    {
                        if (n > 0 && line[n - 1] == '\\')
                        {
                            sb.Append(c);
                            unescape |= true;
                            continue;
                        }

                        if (!started)
                        {
                            started = true;
                            continue;
                        }
                        else if (!ended)
                        {
                            ended = true;
                            continue;
                        }
                    }

                    if (ended)
                        break;
                    else if (started)
                        sb.Append(c);
                }

                if (ended)
                    break;
            }
            var ston = sb.ToString();
            if (unescape)
                ston = Regex.Unescape(ston);
            StonCache<StringBuilder>.Push(sb);

            if (string.IsNullOrWhiteSpace(ston))
            {
                result = default;
                return false;
            }

            result = ston;
            return true;
        }

        private static bool TryGetStringBetweenTwoChars(ref int index, string[] lines, char charFrom, char charTo, out string result)
        {
            if (charFrom == charTo)
                throw new Exception($"please use {nameof(TryGetStringBetweenOneChar)} instead this");

            var line = lines[index];
            var charStartIndex = line.IndexOf(charFrom);
            if (charStartIndex < 0)
            {
                result = default;
                return false;
            }

            var intent = 0;
            var ston = default(string);
            var unescape = false;
            StonCache<StringBuilder>.Pop(out var sb);
            for (int i = index; i < lines.Length; ++i)
            {
                index = i;

                line = lines[i];

                var append = line;
                for (int n = 0; n < line.Length; ++n)
                {
                    if (n > 0 && line[n - 1] == '\\')
                    {
                        unescape |= true;
                        continue;
                    }

                    var c = line[n];
                    if (c == charFrom)
                    {
                        if (intent == 0)
                            append = line.Substring(n + 1, line.Length - (n + 1), StonExtensions.SubstringOptions.Trimmed);
                        intent++;
                    }
                    else if (c == charTo)
                    {
                        intent--;
                        if (intent == 0)
                            append = line.Substring(0, n, StonExtensions.SubstringOptions.Trimmed);
                    }
                }

                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(append);

                if (intent != 0)
                    continue;

                ston = sb.ToString();
                if (unescape)
                    ston = Regex.Unescape(ston);
                break;
            }
            StonCache<StringBuilder>.Push(sb);
            
            if (string.IsNullOrWhiteSpace(ston))
            {
                result = default;
                return false;
            }
            
            result = ston;
            return true;
        }

        private async Task<ParsedValue<IStonValue>> TryParseValue(int index, string[] lines, StonContext ctx)
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

            if (TryGetStringBetweenOneChar(ref index, lines, Quote, out var text))
            {
                var stonValue = await converter.DeserializeAsync(type, text, ctx);
                if (stonValue == null)
                    throw new Exception($"converter {converter} should to return {nameof(StonValue)} for key '{key}' and type '{type}'");

                return new ParsedValue<IStonValue>(index, key, stonValue);
            }
            else
            {
                var valueIndex = equalIndex + 1;
                var valueStr = typeAndValue.Substring(valueIndex, typeAndValue.Length - valueIndex);

                var stonValue = await converter.DeserializeAsync(type, valueStr, ctx);
                if (stonValue == null)
                    throw new Exception($"converter {converter} should to return {nameof(StonValue)} for key '{key}' and type '{type}'");

                return new ParsedValue<IStonValue>(index, key, stonValue);
            }
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

        private struct ParsedValue<T>
        {
            public bool valid;
            public int index;
            public string key;
            public T value;

            public ParsedValue(int index, string key, T value)
            {
                this.valid = true;
                this.index = index;
                this.key = key;
                this.value = value;
            }

            public static ParsedValue<T> Invalid()
            {
                return new ParsedValue<T>
                {
                    valid = false
                };
            }
        }
    }
}
