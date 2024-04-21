using Ston.Serializers;
using System.Collections.Generic;
using System.Text;

namespace Ston
{
    public class StonSettings
    {
        private Options _options;
        public Options options => _options;

        private string _cwd;
        public string cwd => _cwd;

        private List<IStonConverter> _converters;
        public IEnumerable<IStonConverter> converters => _converters;

        private Encoding _encoding;
        public Encoding encoding => _encoding;

        public StonSettings(Options options, List<IStonConverter> converters, Encoding encoding)
        {
            _options = options;
            _converters = converters;
            _encoding = encoding;
        }

        public static StonSettings Default()
        {
            return new StonSettings
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
