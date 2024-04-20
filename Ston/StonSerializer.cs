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
                    if (TryParseObject(ref i, lines, out var key, out var value))
                    {
                        var stonObject = await DeserializeAsync(value, ctx);
                        obj.Add(key, stonObject);
                    }
                }
                else
                {
                    if (TryParseValue(ref i, lines, out var key, out var type, out var value))
                    {
                        if (!TryGetConverter(type, ctx.settings, out var converter))
                            throw new Exception($"unsupported type '{type}'");

                        var stonValue = await converter.DeserializeAsync(type, value, ctx);
                        if (stonValue == null)
                            throw new Exception($"converter {converter} should to return {nameof(StonValue)} for type '{type}'");

                        obj.Add(key, stonValue);
                    }
                }
            }
            return obj;
        }

        private bool TryParseObject(ref int index, string[] lines, out string key, out string value)
        {
            var line = lines[index];
            var equalIndex = line.IndexOf(Equal);
            if (equalIndex < 0)
            {
                key = default;
                value = default;
                return false;
            }

            var parsedKey = line.Substring(0, equalIndex, StonExtensions.SubstringOptions.Trimmed);

            var i = index;
            if (!TryGetStringBetweenTwoChars(ref i, lines, BraceStart, BraceEnd, out var parsedValue))
            {
                i = default;
                key = default;
                value = default;
                return false;
            }

            index = i;
            key = parsedKey;
            value = parsedValue;
            return true;
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

        private bool TryParseValue(ref int index, string[] lines, out string key, out string type, out string value)
        {
            var line = lines[index];
            var separatorIndex = line.IndexOf(Separator);
            if (separatorIndex < 0)
                throw new Exception($"character '{Separator}' wasn't found in line {index}");

            var parsedKey = line.Substring(0, separatorIndex, StonExtensions.SubstringOptions.Trimmed);

            var typeAndValueIndex = separatorIndex + 1;
            var typeAndValue = line.Substring(typeAndValueIndex, line.Length - typeAndValueIndex);

            var equalIndex = typeAndValue.IndexOf(Equal);

            var parsedType = typeAndValue.Substring(0, equalIndex - 0, StonExtensions.SubstringOptions.Trimmed);
            var parsedValue = default(string);
            if (TryGetStringBetweenOneChar(ref index, lines, Quote, out var text))
            {
                parsedValue = text;
            }
            else
            {
                var valueIndex = equalIndex + 1;
                parsedValue = typeAndValue.Substring(valueIndex, typeAndValue.Length - valueIndex);
            }

            key = parsedKey;
            type = parsedType;
            value = parsedValue;
            return true;
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
