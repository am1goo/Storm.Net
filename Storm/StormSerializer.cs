using Storm.Serializers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Storm
{
    public class StormSerializer
    {
        private const char Separator    = ':';
        private const char Equal        = '=';
        private const char Quote        = '"';
        private const char BraceStart   = '{';
        private const char BraceEnd     = '}';
        private const char BracketStart = '[';
        private const char BracketEnd   = ']';

        private List<IStormConverter> _converters = new List<IStormConverter>
        {
            EnumStormConverter.instance,
            PrimitiveStormConverter.instance,
        };
        
        public Task SerializeFileAsync(string filePath, object obj, StormSettings settings = default)
        {
            var fileInfo = new FileInfo(filePath);
            return SerializeFileAsync(fileInfo, obj, settings);
        }

        public async Task SerializeFileAsync(FileInfo fileInfo, object obj, StormSettings settings = default)
        {
            if (settings == null)
                settings = StormSettings.Default();

            var storm = await SerializeAsync(obj, settings);

            var exists = fileInfo.Exists;
            using (var fs = exists ? fileInfo.OpenWrite() : fileInfo.Create())
            {
                fs.Seek(0, SeekOrigin.Begin);
                using (var sw = new StreamWriter(fs, settings.encoding))
                {
                    await sw.WriteAsync(storm);
                }
            }
        }

        public Task<string> SerializeAsync(object obj, StormSettings settings = default)
        {
            if (settings == null)
                settings = StormSettings.Default();

            var ctx = new StormContext(this, settings, settings.cwd);
            return SerializeAsync(obj, ctx);
        }

        internal Task<string> SerializeAsync(object obj, StormContext ctx)
        {
            var type = obj.GetType();
            return SerializeAsync(type, obj, ctx);
        }

        internal async Task<string> SerializeAsync(Type type, object obj, StormContext ctx)
        {
            if (obj == null)
                return string.Empty;

            var pis = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.SetProperty);
            var fis = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField | BindingFlags.SetField);

            StormCache<StringBuilder>.Pop(out var sb);
            StormCache<List<StormFieldOrProperty>>.Pop(out var cache);
            foreach (var pi in pis)
            {
                if (pi.ShouldBeIgnored())
                    continue;

                if (pi.IsPrivate() && !pi.ShouldBeIncluded())
                    continue;

                var variable = new StormFieldOrProperty(obj, pi);
                cache.Add(variable);
            }
            foreach (var fi in fis)
            {
                if (fi.ShouldBeIgnored())
                    continue;

                if (fi.IsPrivate && !fi.ShouldBeIncluded())
                    continue;

                var variable = new StormFieldOrProperty(obj, fi);
                cache.Add(variable);
            }
            foreach (var variable in cache)
            {
                var var = variable.GetValue();
                var str = await SerializeAsync(variable, var, ctx);
                if (str == null)
                    continue;

                sb.AppendLine(str);
            }
            StormCache<List<StormFieldOrProperty>>.Push(cache);

            var storm = sb.ToString();
            sb.Clear();
            StormCache<StringBuilder>.Push(sb);
            return storm;
        }

        internal async Task<string> SerializeAsync(IStormVariable variable, object obj, StormContext ctx)
        {
            var type = variable.type;
            if (type.IsArray)
            {
                const int expectedRank = 1;
                var actualRank = type.GetArrayRank();
                if (actualRank != expectedRank)
                    return null;

                if (!type.HasElementType)
                    return null;

                var array = obj as Array;
                if (array == null)
                    return null;

                var elementType = type.GetElementType();

                StormCache<StringBuilder>.Pop(out var sb);
                var intent = ctx.settings.GetIntent(ctx.intent);
                sb.Append(intent).AppendKey(variable.name);
                sb.Append(BracketStart).Append(Environment.NewLine);
                ctx.intent++;
                for (int i = 0; i < array.Length; ++i)
                {
                    var elementValue = array.GetValue(i);
                    var elementVar = new StormArrayElement(elementType, array, i);
                    var elementStr = await SerializeAsync(elementVar, elementValue, ctx);
                    sb.Append(elementStr).Append(Environment.NewLine);
                }
                ctx.intent--;
                sb.Append(intent).Append(BracketEnd);
                var str = sb.ToString();
                sb.Clear();
                StormCache<StringBuilder>.Push(sb);

                return str;
            }
            else if (TryGetConverter(variable.type, ctx.settings, out var converter))
            {
                var intent = ctx.settings.GetIntent(ctx.intent);
                StormCache<StringBuilder>.Pop(out var sb);
                var storm = await converter.SerializeAsync(variable, obj, ctx);
                sb.Append(intent).Append(storm);
                var str = sb.ToString();
                sb.Clear();
                StormCache<StringBuilder>.Push(sb);

                return str;
            }
            else
            {
                ctx.intent++;
                var storm = await SerializeAsync(type, obj, ctx);
                ctx.intent--;
                if (string.IsNullOrEmpty(storm))
                    return null;

                var intent = ctx.settings.GetIntent(ctx.intent);
                ctx.intent++;
                StormCache<StringBuilder>.Pop(out var sb);
                sb.Append(intent).AppendKey(variable.name);
                sb.Append(BraceStart).Append(Environment.NewLine);
                sb.Append(storm);
                sb.Append(intent).Append(BraceEnd);
                var str = sb.ToString();
                sb.Clear();
                StormCache<StringBuilder>.Push(sb);
                ctx.intent--;

                return str;
            }
        }

        public async Task<T> DeserializeFileAsync<T>(string filePath, StormSettings settings = default)
        {
            var obj = await DeserializeFileAsync(filePath, settings);
            return obj.Populate<T>(settings);
        }

        public async Task<object> DeserializeFileAsync(Type type, string filePath, StormSettings settings = default)
        {
            var obj = await DeserializeFileAsync(filePath, settings);
            return obj.Populate(type, settings);
        }

        public Task<StormObject> DeserializeFileAsync(string filePath, StormSettings settings = default)
        {
            var fi = new FileInfo(filePath);
            return DeserializeFileAsync(fi, settings);
        }

        public async Task<T> DeserializeFileAsync<T>(FileInfo fileInfo, StormSettings settings = default)
        {
            var obj = await DeserializeFileAsync(fileInfo, settings);
            return obj.Populate<T>(settings);
        }

        public async Task<object> DeserializeFileAsync(Type type, FileInfo fileInfo, StormSettings settings = default)
        {
            var obj = await DeserializeFileAsync(fileInfo, settings);
            return obj.Populate(type, settings);
        }

        public async Task<StormObject> DeserializeFileAsync(FileInfo fileInfo, StormSettings settings = default)
        {
            if (!fileInfo.Exists)
                return null;

            if (settings == null)
                settings = StormSettings.Default();

            settings.cwd = fileInfo.Directory.FullName;
            using (var fs = fileInfo.OpenRead())
            {
                using (var sr = new StreamReader(fs, settings.encoding))
                {
                    var storm = await sr.ReadToEndAsync();
                    return await DeserializeAsync(storm, settings);
                }
            }
        }

        public async Task<T> DeserializeAsync<T>(string storm, StormSettings settings)
        {
            var obj = await DeserializeAsync(storm, settings);
            return obj.Populate<T>(settings);
        }

        public async Task<object> DeserializeAsync(Type type, string storm, StormSettings settings)
        {
            var obj = await DeserializeAsync(storm, settings);
            return obj.Populate(type, settings);
        }

        public Task<StormObject> DeserializeAsync(string storm, StormSettings settings)
        {
            if (settings == null)
                settings = StormSettings.Default();

            var ctx = new StormContext(this, settings, settings.cwd);
            return DeserializeAsync(storm, ctx);
        }

        internal Task<StormObject> DeserializeAsync(string storm, StormContext ctx)
        {
            var obj = new StormObject();
            return DeserializeAsync(obj, storm, ctx);
        }

        internal async Task<T> DeserializeAsync<T>(T obj, string storm, StormContext ctx) where T : IStormContainer
        {
            var lines = storm.Split(Environment.NewLine);
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
                        var stormObject = await DeserializeAsync(value, ctx);
                        obj.Add(key, stormObject);
                    }
                    else if (TryParseArray(ref i, lines, out key, out value))
                    {
                        var stormArray = new StormArray();
                        stormArray = await DeserializeAsync(stormArray, value, ctx);
                        obj.Add(key, stormArray);
                    }
                }
                else
                {
                    if (TryParseValue(ref i, lines, out var key, out var type, out var value))
                    {
                        if (!TryGetConverter(type, ctx.settings, out var converter))
                            throw new Exception($"unsupported type '{type}'");

                        var stormValue = await converter.DeserializeAsync(type, value, ctx);
                        if (stormValue == null)
                            throw new Exception($"converter {converter} should to return {nameof(StormValue)} for type '{type}'");

                        obj.Add(key, stormValue);
                    }
                }
            }
            return obj;
        }

        private bool TryParseArray(ref int index, string[] lines, out string key, out string value)
        {
            return TryParseText(ref index, lines, BracketStart, BracketEnd, out key, out value);
        }

        private bool TryParseObject(ref int index, string[] lines, out string key, out string value)
        {
            return TryParseText(ref index, lines, BraceStart, BraceEnd, out key, out value);
        }

        private bool TryParseText(ref int index, string[] lines, char charFrom, char charTo, out string key, out string value)
        {
            var line = lines[index];
            var equalIndex = line.IndexOf(Equal);
            if (equalIndex < 0)
            {
                key = default;
                value = default;
                return false;
            }

            var parsedKey = line.Substring(0, equalIndex, StormExtensions.SubstringOptions.Trimmed);

            var i = index;
            if (!TryGetStringBetweenTwoChars(ref i, lines, charFrom, charTo, out var parsedValue))
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
            StormCache<StringBuilder>.Pop(out var sb);
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
            var storm = sb.ToString();
            if (unescape)
                storm = Regex.Unescape(storm);
            sb.Clear();
            StormCache<StringBuilder>.Push(sb);

            if (string.IsNullOrWhiteSpace(storm))
            {
                result = default;
                return false;
            }

            result = storm;
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
            var storm = default(string);
            var unescape = false;
            StormCache<StringBuilder>.Pop(out var sb);
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
                            append = line.Substring(n + 1, line.Length - (n + 1), StormExtensions.SubstringOptions.Trimmed);
                        intent++;
                    }
                    else if (c == charTo)
                    {
                        intent--;
                        if (intent == 0)
                            append = line.Substring(0, n, StormExtensions.SubstringOptions.Trimmed);
                    }
                }

                if (sb.Length > 0)
                    sb.Append(Environment.NewLine);
                sb.Append(append);

                if (intent != 0)
                    continue;

                storm = sb.ToString();
                if (unescape)
                    storm = Regex.Unescape(storm);
                sb.Clear();
                break;
            }
            StormCache<StringBuilder>.Push(sb);
            
            if (string.IsNullOrWhiteSpace(storm))
            {
                result = default;
                return false;
            }
            
            result = storm;
            return true;
        }

        private bool TryParseValue(ref int index, string[] lines, out string key, out string type, out string value)
        {
            var line = lines[index];
            var separatorIndex = line.IndexOf(Separator);
            if (separatorIndex < 0)
                throw new Exception($"character '{Separator}' wasn't found in line {index}");

            var parsedKey = line.Substring(0, separatorIndex, StormExtensions.SubstringOptions.Trimmed);

            var typeAndValueIndex = separatorIndex + 1;
            var typeAndValue = line.Substring(typeAndValueIndex, line.Length - typeAndValueIndex);

            var equalIndex = typeAndValue.IndexOf(Equal);

            var parsedType = typeAndValue.Substring(0, equalIndex - 0, StormExtensions.SubstringOptions.Trimmed);
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

        private bool TryGetConverter(string type, StormSettings settings, out IStormConverter result)
        {
            if (TryGetConverter(_converters, type, out result))
                return true;

            if (TryGetConverter(settings.converters, type, out result))
                return true;

            return false;
        }

        private bool TryGetConverter(Type type, StormSettings settings, out IStormConverter result)
        {
            if (TryGetConverter(_converters, type, out result))
                return true;

            if (TryGetConverter(settings.converters, type, out result))
                return true;

            return false;
        }

        private static bool TryGetConverter(IEnumerable<IStormConverter> list, string type, out IStormConverter result)
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

        private static bool TryGetConverter(IEnumerable<IStormConverter> list, Type type, out IStormConverter result)
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
