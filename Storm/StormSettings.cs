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

        public StormSettings(Options options, List<IStormConverter> converters, Encoding encoding)
        {
            _options = options;
            _converters = converters;
            _encoding = encoding;
        }

        public static StormSettings Default()
        {
            return new StormSettings
            (
                options: 0,
                converters: null,
                encoding: Encoding.UTF8
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
