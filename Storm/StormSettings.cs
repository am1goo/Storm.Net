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

        private int _intentSize;
        public int intentSize => _intentSize;

        public StormSettings(Options options, List<IStormConverter> converters, Encoding encoding, StormEnumFormat defaultEnumFormat, int intentSize)
        {
            _options = options;
            _converters = converters;
            _encoding = encoding;
            _defaultEnumFormat = defaultEnumFormat;
            _intentSize = intentSize;
        }

        public static StormSettings Default()
        {
            return new StormSettings
            (
                options:            0,
                converters:         null,
                encoding:           Encoding.UTF8,
                defaultEnumFormat:  StormEnumFormat.String,
                intentSize:         2
            );
        }

        internal void SetCwd(string cwd)
        {
            _cwd = cwd;
        }

        private string _intent;
        public string intent
        {
            get
            {
                if (_intent == null || _intent.Length != _intentSize)
                    _intent = new string(' ', _intentSize);
                return _intent;
            }
        }

        public enum Options
        {
            IgnoreCase = 0,
        }
    }
}
