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

        public StormSettings(Options options, List<IStormConverter> converters, Encoding encoding, StormEnumFormat defaultEnumFormat)
        {
            _options = options;
            _converters = converters;
            _encoding = encoding;
            _defaultEnumFormat = defaultEnumFormat;
        }

        public static StormSettings Default()
        {
            return new StormSettings
            (
                options:            0,
                converters:         null,
                encoding:           Encoding.UTF8,
                defaultEnumFormat:  StormEnumFormat.String
            );
        }

        internal void SetCwd(string cwd)
        {
            _cwd = cwd;
        }

        public enum Options
        {
            IgnoreCase = 0,
        }
    }
}
