using Storm.Serializers;
using System.Collections.Generic;
using System.Text;

namespace Storm
{
    public class StormSettings
    {
        private Options _options;
        public Options options => _options;

        private string _cwd;
        public string cwd => _cwd;

        private List<IStormConverter> _converters;
        public IEnumerable<IStormConverter> converters => _converters;

        private Encoding _encoding;
        public Encoding encoding => _encoding;

        private StormEnumFormat _defaultEnumFormat;
        public StormEnumFormat defaultEnumFormat => _defaultEnumFormat;

        private string _numberDecimalSeparator;
        public string numberDecimalSeparator => _numberDecimalSeparator;

        private int _intentSize;
        public int intentSize => _intentSize;

        private Dictionary<int, string> _intentCache = new Dictionary<int, string>();

        public StormSettings(Options options, List<IStormConverter> converters, Encoding encoding, StormEnumFormat defaultEnumFormat, string numberDecimalSeparator, int intentSize)
        {
            _options = options;
            _converters = converters;
            _encoding = encoding;
            _defaultEnumFormat = defaultEnumFormat;
            _numberDecimalSeparator = numberDecimalSeparator;
            _intentSize = intentSize;
        }

        public static StormSettings Default()
        {
            return new StormSettings
            (
                options:                0,
                converters:             null,
                encoding:               Encoding.UTF8,
                defaultEnumFormat:      StormEnumFormat.String,
                numberDecimalSeparator: ",",
                intentSize:             2
            );
        }

        internal void SetCwd(string cwd)
        {
            _cwd = cwd;
        }

        public string GetIntent(int intents)
        {
            if (!_intentCache.TryGetValue(intents, out var exist))
            {
                exist = new string(' ', intents * _intentSize);
                _intentCache.Add(intents, exist);
            }
            return exist;
        }

        public enum Options
        {
            IgnoreCase = 0,
        }
    }
}
